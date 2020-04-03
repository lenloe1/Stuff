///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//   embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//storage or retrieval system without the permission in writing from Itron, Inc.
//
//                              Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Itron.Metering.Utilities;
using System.Globalization;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// Address lengths
    /// </summary>
    [Serializable]
    public enum HDLCAddressLength
    {
        /// <summary>1 byte address</summary>
        [EnumDescription("1")]
        One = 1,
        /// <summary>2 byte address</summary>
        [EnumDescription("2")]
        Two = 2,
        /// <summary>4 byte address</summary>
        [EnumDescription("4")]
        Four = 4,
    }

    /// <summary>
    /// HDLC Communications Port
    /// </summary>
    public class HDLCComm : IDLMSComm
    {
        #region Events

        /// <summary>
        /// Event that is raised when an APDU has been received
        /// </summary>
        public event APDUEventHandler APDUReceived;

        #endregion

        #region Constants

        private const int SLEEP_LENGTH = 1;

        private const byte SNRM_FORMAT_ID = 0x81;
        private const byte SNRM_GROUP_ID = 0x80;
        private const byte SNRM_TX_INFO_LENGTH = 0x05;
        private const byte SNRM_RX_INFO_LENGTH = 0x06;
        private const byte SNRM_TX_WINDOW = 0x07;
        private const byte SNRM_RX_WINDOW = 0x08;

        private const byte SNRM_WINDOW_PARAM_LENGTH = 4;

        private const ushort DEFAULT_INFO_LENGTH = 768;
        private const uint DEFAULT_WINDOW_SIZE = 1;
        private const int DEFAULT_KEEP_ALIVE = 30000;

        private const int DATA_CHECK_TIME = 75;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">The name of the port to use</param>
        /// <param name="baudRate">The baud rate to use</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public HDLCComm(string port, int baudRate)
        {
            m_Port = new SerialPort(port, baudRate);

            m_Port.Parity = Parity.None;
            m_Port.StopBits = StopBits.One;
            m_Port.DataBits = 8;

            m_Port.DtrEnable = true;
            m_Port.RtsEnable = true;

            m_HDLCBuffer = new List<byte>();
            m_ReceivedFrames = new List<HDLCFrame>();
            m_InformationBuffer = new List<byte>();
            m_DataReceivedHandler = new SerialDataReceivedEventHandler(m_Port_DataReceived);

            m_InterPacketTimeout = DEFAULT_KEEP_ALIVE;
            m_KeepAliveTimer = new Timer(new TimerCallback(KeepAlive), null, Timeout.Infinite, Timeout.Infinite);

            m_GlobalEncryptionKey = null;
            m_DedicatedEncryptionKey = null;
            m_DecryptAuthenticationKey = null;
            m_PendingDecryptAuthenticationKey = null;
            m_ServerApTitle = null;
            m_LastFrameCounterReceived = 0;

            m_SendSequenceNumber = 0;
            m_ReceiveSequenceNumber = 0;
            m_UseNumberedInformationFrames = false;
            m_SendSNRMCommand = true;
            m_KeepAliveDisabled = false;

            m_Logger = Logger.TheInstance;
        }

        /// <summary>
        /// Opens the port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public void Open()
        {
            if (m_Port.IsOpen == false)
            {
                try
                {
                    m_Port.Open();

                    // Clear out any existing data
                    m_HDLCBuffer.Clear();
                    m_ReceivedFrames.Clear();

                    m_Port.DataReceived += null; // Clear out any existing handlers
                    m_Port.DataReceived += m_DataReceivedHandler;

                    m_TransmitMaxPacketSize = DEFAULT_INFO_LENGTH;
                    m_ReceiveMaxPacketSize = DEFAULT_INFO_LENGTH;

                    m_TransmitWindowLength = DEFAULT_WINDOW_SIZE;
                    m_ReceiveWindowLength = DEFAULT_WINDOW_SIZE;

                    // Send setup frames
                    if (SendSNRMCommand)
                    {
                        SendSNRM();
                        m_IsOpen = true;
                    }
                    else
                    {
                        // We aren't sending the SNRM command so we are connected.
                        m_IsOpen = true;
                    }

                    if (IsOpen)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "HDLC Settings Used:");
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "\tTransmit Window Length:  " + m_TransmitWindowLength.ToString(CultureInfo.InvariantCulture));
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "\tReceive Window Length: " + m_ReceiveWindowLength.ToString(CultureInfo.InvariantCulture));
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "\tTransmit Max Packet Size: " + m_TransmitMaxPacketSize.ToString(CultureInfo.InvariantCulture));
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "\tReceive Max Packet Size: " + m_ReceiveMaxPacketSize.ToString(CultureInfo.InvariantCulture));

                        EnableKeepAlive();
                    }
                }
                catch (Exception)
                {
                    // Make sure we close the port if something bad happens
                    if (m_Port.IsOpen)
                    {
                        m_Port.Close();
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Closes the port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public void Close()
        {
            if (m_Port.IsOpen)
            {
                // Send Disconnect message
                try
                {
                    SendDisconnectRequest();
                }
                catch (TimeoutException)
                {
                    // Don't really care if this times out since we are disconnecting anyway
                }

                DisableKeepAlive();

                m_Port.DataReceived -= m_DataReceivedHandler;

                m_Port.Close();

                // Clear out any existing data
                m_HDLCBuffer.Clear();
                m_ReceivedFrames.Clear();

                m_IsOpen = false;
            }
        }

        /// <summary>
        /// Sends an APDU to the connected device
        /// </summary>
        /// <param name="apdu">The APDU to send</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created

        public void SendAPDU(xDLMSAPDU apdu)
        {
            if (IsOpen)
            {
                HDLCFrame APDUFrame = null;
                LPDU LPDUFrame = new LPDU();

                // The AARQ needs to be sent as a numbered frame
                if (UseNumberedInformationFrames)
                {
                    APDUFrame = new InformationFrame(ReceiveSequenceNumber, SendSequenceNumber);
                    SendSequenceNumber++;
                }
                else
                {
                    APDUFrame = new UnnumberedFrame(UnnumberedCommands.UnnumberedInformation);
                }

                LPDUFrame.Information = apdu.Data;

                APDUFrame.Payload = LPDUFrame.Data;
                APDUFrame.DestinationAddress = HDLCFrame.GenerateAddressBytes(ServerPort, PhysicalAddress, AddressLength);
                APDUFrame.SourceAddress = HDLCFrame.GenerateAddressBytes(ClientPort);

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Sending APDU. Type: " + EnumDescriptionRetriever.RetrieveDescription(apdu.Tag));

                DisableKeepAlive();

                SendFrame(APDUFrame);

                EnableKeepAlive();
            }
        }

        /// <summary>
        /// Clears the current data buffers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created
        
        public void ClearBuffers()
        {
            lock (m_HDLCBuffer)
            {
                m_HDLCBuffer.Clear();
            }

            lock (m_InformationBuffer)
            {
                m_InformationBuffer.Clear();
            }

            lock (m_ReceivedFrames)
            {
                m_ReceivedFrames.Clear();
            }
        }

        /// <summary>
        /// Clears all handlers currently attached to the APDU Received event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/16/13 RCG 2.85.44 N/A    Created

        public void ClearAPDUReceivedHandlers()
        {
            APDUReceived = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Enables the Keep Alive timer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private void EnableKeepAlive()
        {
            if (KeepAliveDisabled == false)
            {
                m_KeepAliveTimer.Change(m_InterPacketTimeout, m_InterPacketTimeout);
            }
        }

        /// <summary>
        /// Disables the Keep Alive timer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private void DisableKeepAlive()
        {
            m_KeepAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Sends an HDLC frame
        /// </summary>
        /// <param name="frame">The frame to send</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private void SendFrame(HDLCFrame frame)
        {
            if (m_Port.IsOpen)
            {
                if (frame != null)
                {
                    byte[] Data = frame.Frame;

                    if (Data != null)
                    {
                        if (Data.Length <= TransmitMaxPacketSize)
                        {
                            m_Logger.WriteProtocol(Logger.ProtocolDirection.Send, Data);
                            m_Port.Write(Data, 0, Data.Length);
                        }
                        else
                        {
                            throw new ArgumentException("The specified frame is too long to be sent.", "frame");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The frame data is null", "frame");
                    }
                }
                else
                {
                    throw new ArgumentNullException("frame", "The frame parameter may not be null");
                }
            }
        }

        /// <summary>
        /// Sends the SNRM command to the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private void SendSNRM()
        {
            if (m_Port.IsOpen)
            {
                UnnumberedFrame SNRMFrame = new UnnumberedFrame(UnnumberedCommands.SetNormalResponseMode);
                DateTime StartTime = DateTime.UtcNow;
                TimeSpan TimeOut = TimeSpan.FromMilliseconds(m_InterPacketTimeout);
                bool AckReceived = false;
                bool DisconnectModeReceived = false;

                // Reset the state variables
                SendSequenceNumber = 0;
                ReceiveSequenceNumber = 0;

                SNRMFrame.DestinationAddress = HDLCFrame.GenerateAddressBytes(ServerPort, PhysicalAddress, AddressLength);
                SNRMFrame.SourceAddress = HDLCFrame.GenerateAddressBytes(ClientPort);

                SNRMFrame.Payload = GenerateSNRMParameterData();

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Sending SNRM");

                DisableKeepAlive();

                SendFrame(SNRMFrame);

                while (DateTime.UtcNow - StartTime < TimeOut && AckReceived == false && DisconnectModeReceived == false)
                {
                    IEnumerable<HDLCFrame> UnnumberedFrames = m_ReceivedFrames.Where(f => f is UnnumberedFrame);

                    foreach (HDLCFrame CurrentFrame in UnnumberedFrames)
                    {
                        UnnumberedFrame CurrentUnnumberedFrame = CurrentFrame as UnnumberedFrame;

                        if (CurrentUnnumberedFrame.Command == UnnumberedCommands.UnnumberedAcknowledge)
                        {
                            // We are now ready to use SNRM
                            AckReceived = true;
                            ParseSNRMParameterData(CurrentUnnumberedFrame.Payload);
                            break;
                        }
                        else if (CurrentUnnumberedFrame.Command == UnnumberedCommands.DisconnectMode)
                        {
                            // SNRM was rejected by the device
                            DisconnectModeReceived = true;
                            break;
                        }
                    }

                    // Wait a little while before checking again
                    Thread.Sleep(SLEEP_LENGTH);
                }

                EnableKeepAlive();

                if (AckReceived)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "SNRM Acknowledged");

                    // We are now in SNRM mode and ready to communicate
                    lock (m_ReceivedFrames)
                    {
                        m_ReceivedFrames.RemoveAll(f => f is UnnumberedFrame && ((UnnumberedFrame)f).Command == UnnumberedCommands.UnnumberedAcknowledge);
                    }
                }
                else if (DisconnectModeReceived)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Disconnect Mode Received in response to SNRM");

                    // SNRM is not supported so we can't communicate to the device
                    lock (m_ReceivedFrames)
                    {
                        m_ReceivedFrames.RemoveAll(f => f is UnnumberedFrame && ((UnnumberedFrame)f).Command == UnnumberedCommands.DisconnectMode);
                    }

                    throw new NotSupportedException("SNRM mode is not supported on this device");
                }
                else
                {
                    throw new TimeoutException("The SNRM response was not received within the allowed time");
                }
            }
        }

        /// <summary>
        /// Generates the Parameter Data for the SNRM message
        /// </summary>
        /// <returns>The Parameter Data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/23/13 RCG 2.80.33 N/A    Created
        
        private byte[] GenerateSNRMParameterData()
        {
            MemoryStream ParameterStream = new MemoryStream();
            DLMSBinaryWriter ParameterWriter = new DLMSBinaryWriter(ParameterStream);
            byte[] GroupData = GenerateSNRMGroupData();

            // Format Identifier
            ParameterWriter.Write(SNRM_FORMAT_ID);
            
            // Group Identifier
            ParameterWriter.Write(SNRM_GROUP_ID);

            // Group Length
            ParameterWriter.Write((byte)GroupData.Length);

            // Group Data
            ParameterWriter.Write(GroupData);

            return ParameterStream.ToArray();
        }

        /// <summary>
        /// Generates the Parameter Group Data for the SNRM message
        /// </summary>
        /// <returns>The Parameter Group Data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/23/13 RCG 2.80.33 N/A    Created
        
        private byte[] GenerateSNRMGroupData()
        {
            MemoryStream ParameterStream = new MemoryStream();
            DLMSBinaryWriter ParameterWriter = new DLMSBinaryWriter(ParameterStream);

            // Transmit Max Info Length
            ParameterWriter.Write(SNRM_TX_INFO_LENGTH); // Parameter ID

            if (m_TransmitMaxPacketSize <= byte.MaxValue)
            {
                // We can send the value as 1 byte
                ParameterWriter.Write((byte)1); // Parameter Length
                ParameterWriter.Write((byte)m_TransmitMaxPacketSize);
            }
            else
            {
                // Send both bytes
                ParameterWriter.Write((byte)2);
                ParameterWriter.Write(m_TransmitMaxPacketSize);
            }

            // Received Max Info Length
            ParameterWriter.Write(SNRM_RX_INFO_LENGTH); // Parameter ID

            if (m_ReceiveMaxPacketSize <= byte.MaxValue)
            {
                // We can send the value as 1 byte
                ParameterWriter.Write((byte)1); // Parameter Length
                ParameterWriter.Write((byte)m_ReceiveMaxPacketSize);
            }
            else
            {
                // Send both bytes
                ParameterWriter.Write((byte)2);
                ParameterWriter.Write(m_ReceiveMaxPacketSize);
            }

            // Transmit Window Size
            ParameterWriter.Write(SNRM_TX_WINDOW); // Parameter ID
            ParameterWriter.Write(SNRM_WINDOW_PARAM_LENGTH); // Parameter Length
            ParameterWriter.Write(m_TransmitWindowLength);

            // Receive Window Size
            ParameterWriter.Write(SNRM_RX_WINDOW); // Parameter ID
            ParameterWriter.Write(SNRM_WINDOW_PARAM_LENGTH); // Parameter Length
            ParameterWriter.Write(m_TransmitWindowLength);

            return ParameterStream.ToArray();
        }

        /// <summary>
        /// Parses the parameter data from the SNRM response
        /// </summary>
        /// <param name="data">The parameter data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/23/13 RCG 2.80.33 N/A    Created
        
        private void ParseSNRMParameterData(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                MemoryStream ParameterStream = new MemoryStream(data);
                DLMSBinaryReader ParameterReader = new DLMSBinaryReader(ParameterStream);
                byte CurrentByte;

                CurrentByte = ParameterReader.ReadByte();

                // Make sure that it's the correct format
                if (CurrentByte == SNRM_FORMAT_ID)
                {
                    CurrentByte = ParameterReader.ReadByte();

                    // Make sure that it's the correct Group ID
                    if (CurrentByte == SNRM_GROUP_ID)
                    {
                        CurrentByte = ParameterReader.ReadByte(); // Group Length

                        // Read out each of the parameter fields
                        while (ParameterStream.Position < ParameterStream.Length)
                        {
                            byte ParameterID = ParameterReader.ReadByte();

                            switch(ParameterID)
                            {
                                case SNRM_TX_INFO_LENGTH:
                                {
                                    CurrentByte = ParameterReader.ReadByte(); // Length

                                    if (CurrentByte == 1)
                                    {
                                        m_TransmitMaxPacketSize = ParameterReader.ReadByte();
                                    }
                                    else if (CurrentByte == 2)
                                    {
                                        m_TransmitMaxPacketSize = ParameterReader.ReadUInt16();
                                    }
                                    else
                                    {
                                        // This shouldn't happen but lets read the data anyway
                                        byte[] ParameterValue = ParameterReader.ReadBytes(CurrentByte);
                                        Array.Reverse(ParameterValue);
                                        m_TransmitMaxPacketSize = Convert.ToUInt16(ParameterValue);
                                    }

                                    break;
                                }
                                case SNRM_RX_INFO_LENGTH:
                                {
                                    CurrentByte = ParameterReader.ReadByte(); // Length

                                    if (CurrentByte == 1)
                                    {
                                        m_ReceiveMaxPacketSize = ParameterReader.ReadByte();
                                    }
                                    else if (CurrentByte == 2)
                                    {
                                        m_ReceiveMaxPacketSize = ParameterReader.ReadUInt16();
                                    }
                                    else
                                    {
                                        // This shouldn't happen but lets read the data anyway
                                        byte[] ParameterValue = ParameterReader.ReadBytes(CurrentByte);
                                        Array.Reverse(ParameterValue);
                                        m_ReceiveMaxPacketSize = Convert.ToUInt16(ParameterValue);
                                    }

                                    break;
                                }
                                case SNRM_TX_WINDOW:
                                {
                                    CurrentByte = ParameterReader.ReadByte(); // Length

                                    if (CurrentByte == 4)
                                    {
                                        m_TransmitWindowLength = ParameterReader.ReadUInt32();
                                    }
                                    else
                                    {
                                        // This shouldn't happen but lets read the data anyway
                                        byte[] ParameterValue = ParameterReader.ReadBytes(CurrentByte);
                                        Array.Reverse(ParameterValue);
                                        m_TransmitWindowLength = Convert.ToUInt32(ParameterValue);
                                    }
                                
                                    break;
                                }
                                case SNRM_RX_WINDOW:
                                {
                                    CurrentByte = ParameterReader.ReadByte(); // Length

                                    if (CurrentByte == 4)
                                    {
                                        m_ReceiveWindowLength = ParameterReader.ReadUInt32();
                                    }
                                    else
                                    {
                                        // This shouldn't happen but lets read the data anyway
                                        byte[] ParameterValue = ParameterReader.ReadBytes(CurrentByte);
                                        Array.Reverse(ParameterValue);
                                        m_ReceiveWindowLength = Convert.ToUInt32(ParameterValue);
                                    }

                                    break;
                                }
                                default:
                                {
                                    // Not sure what this is but we need to read past it
                                    CurrentByte = ParameterReader.ReadByte(); // Length
                                    ParameterReader.ReadBytes(CurrentByte);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sends the disconnect request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private void SendDisconnectRequest()
        {
            if (m_Port.IsOpen)
            {
                UnnumberedFrame DisconnectFrame = new UnnumberedFrame(UnnumberedCommands.Disconnect);
                DateTime StartTime = DateTime.UtcNow;
                TimeSpan TimeOut = TimeSpan.FromMilliseconds(m_InterPacketTimeout);
                bool ResponseReceived = false;

                DisconnectFrame.DestinationAddress = HDLCFrame.GenerateAddressBytes(ServerPort, PhysicalAddress, AddressLength);
                DisconnectFrame.SourceAddress = HDLCFrame.GenerateAddressBytes(ClientPort);

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Sending Disconnect");

                DisableKeepAlive();

                SendFrame(DisconnectFrame);

                while (DateTime.UtcNow - StartTime < TimeOut && ResponseReceived == false)
                {
                    IEnumerable<HDLCFrame> UnnumberedFrames = m_ReceivedFrames.Where(f => f is UnnumberedFrame);

                    foreach (HDLCFrame CurrentFrame in UnnumberedFrames)
                    {
                        UnnumberedFrame CurrentUnnumberedFrame = CurrentFrame as UnnumberedFrame;

                        if (CurrentUnnumberedFrame.Command == UnnumberedCommands.UnnumberedAcknowledge
                            || CurrentUnnumberedFrame.Command == UnnumberedCommands.DisconnectMode)
                        {
                            // We are now disconnected
                            ResponseReceived = true;
                            break;
                        }
                    }

                    // Wait a little while before checking again
                    Thread.Sleep(SLEEP_LENGTH);
                }

                EnableKeepAlive();

                if (ResponseReceived)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Disconnect Response received");
                    // We are now disconnected so clear out any received frames
                    lock (m_ReceivedFrames)
                    {
                        m_ReceivedFrames.Clear();
                    }

                    // Reset the state variables
                    SendSequenceNumber = 0;
                    ReceiveSequenceNumber = 0;
                }
                else
                {
                    throw new TimeoutException("The disconnect response was not received within the allowed time");
                }
            }
        }

        /// <summary>
        /// Performs the keep alive operation
        /// </summary>
        /// <param name="state">The timer state</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private void KeepAlive(object state)
        {
            try
            {
                if (KeepAliveDisabled == false)
                {
                    if (SendSNRMCommand)
                    {
                        SendReceiveReady(); // We don't care about the response any communications should do
                    }
                    else
                    {
                        // We don't know what the sequence number will be so let's just send an empty UI frame
                        UnnumberedFrame KeepAliveFrame = new UnnumberedFrame(UnnumberedCommands.UnnumberedInformation);

                        KeepAliveFrame.Payload = new byte[0];
                        KeepAliveFrame.DestinationAddress = HDLCFrame.GenerateAddressBytes(ServerPort, PhysicalAddress, AddressLength);
                        KeepAliveFrame.SourceAddress = HDLCFrame.GenerateAddressBytes(ClientPort);

                        SendFrame(KeepAliveFrame);

                        // No need to look for the response.
                    }
                }
            }
            catch (Exception)
            {
                // Catch and ignore exceptions here since we don't want an unhandled exception because of threading issues
            }
        }

        /// <summary>
        /// Handles the data received event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private void m_Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (m_Port.IsOpen && m_Port.BytesToRead > 0)
            {
                lock (m_Port)
                {
                    int BytesToReadLast = 0;
                    int BytesToReadCurrent = 0;

                    // Keep checking the amount of data on the port until we stop seeing new data so we can read it all at once
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Receiving Data");

                    do
                    {
                        BytesToReadLast = BytesToReadCurrent;

                        Thread.Sleep(20);
                        BytesToReadCurrent = m_Port.BytesToRead;
                    }
                    while (BytesToReadLast != BytesToReadCurrent);

                    // Read all data currently on the Serial Port
                    byte[] Data = new byte[BytesToReadCurrent];
                    m_Port.Read(Data, 0, Data.Length);

                    m_Logger.WriteProtocol(Logger.ProtocolDirection.Receive, Data);

                    lock (m_HDLCBuffer)
                    {
                        m_HDLCBuffer.AddRange(Data);
                    }

                    // Parse the frames in a new thread so we can be ready to read more data
                    Task.Factory.StartNew(() => ParseFrames());
                }
            }
        }

        /// <summary>
        /// Sends the receive ready frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        private bool SendReceiveReady()
        {
            SupervisoryFrame ReceiveReadyFrame = new SupervisoryFrame(SupervisoryCommands.ReceiveReady, ReceiveSequenceNumber);
            DateTime StartTime = DateTime.UtcNow;
            TimeSpan Timeout = TimeSpan.FromMilliseconds(m_InterPacketTimeout);
            HDLCFrame Response = null;
            bool Ready = false;

            if (IsOpen)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Sending Receive Ready");

                DisableKeepAlive();

                ReceiveReadyFrame.DestinationAddress = HDLCFrame.GenerateAddressBytes(ServerPort, PhysicalAddress, AddressLength);
                ReceiveReadyFrame.SourceAddress = HDLCFrame.GenerateAddressBytes(ClientPort);

                SendFrame(ReceiveReadyFrame);

                while (DateTime.UtcNow - StartTime < Timeout && Response == null)
                {
                    IEnumerable<HDLCFrame> SupervisoryFrames = m_ReceivedFrames.Where(f => f is SupervisoryFrame);

                    foreach (HDLCFrame CurrentFrame in SupervisoryFrames)
                    {
                        SupervisoryFrame CurrentSupervisoryFrame = CurrentFrame as SupervisoryFrame;

                        if (CurrentSupervisoryFrame.Command == SupervisoryCommands.ReceiveReady)
                        {
                            // We received a read ready response
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Receive Ready response received");
                            Response = CurrentFrame;
                            Ready = true;
                            break;
                        }
                        else if (CurrentSupervisoryFrame.Command == SupervisoryCommands.ReceiveNotReady)
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Receive Not Ready response received");
                            Response = CurrentFrame;
                            Ready = false;
                            break;
                        }
                    }

                    // Wait a little while before checking again
                    Thread.Sleep(SLEEP_LENGTH);
                }

                EnableKeepAlive();

                if (Response != null)
                {
                    // Remove the response
                    lock (m_ReceivedFrames)
                    {
                        m_ReceivedFrames.Remove(Response);
                    }
                }
                else
                {
                    // We didn't receive a response so time out
                    throw new TimeoutException("The Receive Ready response was not received within the allowed time");
                }
            }

            return Ready;
        }

        /// <summary>
        /// Handles the Frame Received event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private void HandleFrameReceived()
        {
            // Check for any Unnumbered Information frames
            lock (m_ReceivedFrames)
            {
                List<HDLCFrame> FramesToRemove = new List<HDLCFrame>();

                foreach (HDLCFrame CurrentFrame in m_ReceivedFrames)
                {
                    if (CurrentFrame is InformationFrame 
                        || (CurrentFrame is UnnumberedFrame && ((UnnumberedFrame)CurrentFrame).Command == UnnumberedCommands.UnnumberedInformation))
                    {
                        if (CurrentFrame is InformationFrame)
                        {
                            ReceiveSequenceNumber = ((InformationFrame)CurrentFrame).SendSequenceNumber;
                        }

                        FramesToRemove.Add(CurrentFrame);

                        lock (m_InformationBuffer)
                        {
                            m_InformationBuffer.AddRange(CurrentFrame.Payload);
                        }
                    }
                }

                m_ReceivedFrames.RemoveAll(f => FramesToRemove.Contains(f));
            }

            ParseAPDUData();
        }

        /// <summary>
        /// Parses the data for any APDU's
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        private void ParseAPDUData()
        {
            try
            {
                lock (m_InformationBuffer)
                {
                    while (m_InformationBuffer.Count > 3)
                    {
                        // We need to parse out the LPDU first
                        LPDU NewLPDU = LPDU.Parse(m_InformationBuffer.ToArray());

                        if(NewLPDU.DestinationLSAP == LPDU.DEFAULT_LSAP && NewLPDU.SourceLSAP == LPDU.DEFAULT_RECEIVED_LSAP && NewLPDU.Quality == LPDU.DEFAULT_QUALITY)
                        {
                            // We found the start of a proper LPDU so the first byte of the information should be the APDU tag
                            xDLMSAPDU NewAPDU = xDLMSAPDU.Create((xDLMSTags)NewLPDU.Information[0]);
                            CipheredAPDU NewCipheredAPDU = NewAPDU as CipheredAPDU;

                            if (NewAPDU != null)
                            {
                                // Set up the security settings for a Ciphered APDU
                                if (NewCipheredAPDU != null)
                                {
                                    if (CipheredAPDU.IsTagGlobalCipher(NewCipheredAPDU.Tag))
                                    {
                                        NewCipheredAPDU.BlockCipherKey = GlobalEncryptionKey;
                                    }
                                    else if (CipheredAPDU.IsTagDedicatedCipher(NewCipheredAPDU.Tag))
                                    {
                                        NewCipheredAPDU.BlockCipherKey = DedicatedEncryptionKey;
                                    }

                                    if (PendingDecryptAuthenticationKey != null)
                                    {
                                        NewCipheredAPDU.AuthenticationKey = PendingDecryptAuthenticationKey;
                                    }
                                    else
                                    {
                                        NewCipheredAPDU.AuthenticationKey = DecryptAuthenticationKey;
                                    }

                                    NewCipheredAPDU.ApTitle = ServerApTitle;

                                    // Make sure we store the last frame counter in case we need to calculate the GMAC
                                    m_LastFrameCounterReceived = NewCipheredAPDU.FrameCounter;
                                }

                                // We found a valid APDU tag so we can try to parse it. We already checked the first byte so just use the rest
                                MemoryStream DataStream = new MemoryStream(NewLPDU.Information);

                                NewAPDU.Parse(DataStream);

                                // Remove the data that has been parsed
                                m_InformationBuffer.RemoveRange(0, (int)(DataStream.Position + 3));

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "APDU Received. Type: " + EnumDescriptionRetriever.RetrieveDescription(NewAPDU.Tag));

                                if (NewCipheredAPDU != null)
                                {
                                    xDLMSAPDU UncipheredAPDU = null;

                                    if(PendingDecryptAuthenticationKey != null)
                                    {
                                        // We are in the middle of a key exchange so we could get a message using either the pending key or
                                        // the previous key. We need to attempt the Pending key first
                                        try
                                        {
                                            UncipheredAPDU = NewCipheredAPDU.UncipheredAPDU;
                                        }
                                        catch(Exception)
                                        {
                                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Unciphering failed using the Pending Authentication Key. Using previous key.");
                                        }

                                        if(UncipheredAPDU == null)
                                        {
                                            // Try it with the previous key
                                            NewCipheredAPDU.AuthenticationKey = DecryptAuthenticationKey;
                                            UncipheredAPDU = NewCipheredAPDU.UncipheredAPDU;
                                        }
                                    }
                                    else
                                    {
                                        UncipheredAPDU = NewCipheredAPDU.UncipheredAPDU;
                                    }

                                    // Send up the Unciphered APDU
                                    OnAPDUReceived(UncipheredAPDU);
                                }
                                else
                                {
                                    OnAPDUReceived(NewAPDU);
                                }
                            }
                            else
                            {
                                // The tag is not valid so lets get rid of the byte and move on
                                m_InformationBuffer.RemoveAt(0);
                            }
                        }
                        else
                        {
                            // This is not a valid start of an LPDU so remove the byte
                            m_InformationBuffer.RemoveAt(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // This most likely means that we don't have all of the data yet for a specific message
                // so do nothing and hope the rest of the data comes in soon.
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception while parsing an APDU. Waiting for more data. Message: " + e.Message);
            }
        }

        /// <summary>
        /// Parses any frames that have been read by the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private void ParseFrames()
        {
            HDLCFrame LastFrame = null;
            bool FrameReceived = false;

            lock (m_HDLCBuffer)
            {
                do
                {
                    MemoryStream DataStream = new MemoryStream(m_HDLCBuffer.ToArray());

                    try
                    {
                        LastFrame = HDLCFrame.Parse(DataStream);

                        if (LastFrame != null)
                        {
                            lock (m_ReceivedFrames)
                            {
                                m_ReceivedFrames.Add(LastFrame);
                                FrameReceived = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Unable to parse frame so frame is being skipped. Reason: " + e.Message);
                    }

                    // Remove any data that has been read from the buffer
                    if (DataStream.Position > 0)
                    {
                        m_HDLCBuffer.RemoveRange(0, (int)DataStream.Position);
                    }
                } while (LastFrame != null);
            }

            if (FrameReceived)
            {
                HandleFrameReceived();
            }
        }

        /// <summary>
        /// Raises the APDU Received event
        /// </summary>
        /// <param name="apdu">The apdu that was received</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created

        private void OnAPDUReceived(xDLMSAPDU apdu)
        {
            if (APDUReceived != null && apdu != null)
            {
                APDUReceived(this, new APDUEventArguments(apdu));
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Port name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public string Port
        {
            get
            {
                return m_Port.PortName;
            }
        }

        /// <summary>
        /// Gets or sets the baud rate used by the port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public int BaudRate
        {
            get
            {
                return m_Port.BaudRate;
            }
        }

        /// <summary>
        /// Gets whether or not HDLC is currently connected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public bool IsOpen
        {
            get
            {
                return m_IsOpen;
            }
        }

        /// <summary>
        /// Gets or sets the maximum packet size allowed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public ushort TransmitMaxPacketSize
        {
            get
            {
                return m_TransmitMaxPacketSize;
            }
            set
            {
                m_TransmitMaxPacketSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the inter packet timeout in milliseconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public int InterPacketTimeout
        {
            get
            {
                return m_InterPacketTimeout;
            }
            set
            {
                m_InterPacketTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the Client Port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ushort ClientPort
        {
            get
            {
                return m_ClientPort;
            }
            set
            {
                m_ClientPort = value;
            }
        }

        /// <summary>
        /// Gets or sets the Server Port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ushort ServerPort
        {
            get
            {
                return m_ServerPort;
            }
            set
            {
                m_ServerPort = value;
            }
        }

        /// <summary>
        /// Gets or sets the Logical Port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/23/13 RCG 2.80.33 N/A    Created
        
        public ushort PhysicalAddress
        {
            get
            {
                return m_PhysicalAddress;
            }
            set
            {
                m_PhysicalAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the Address Length
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/27/13 RCG 2.80.44 N/A    Created
        
        public HDLCAddressLength AddressLength
        {
            get
            {
                return m_AddressLength;
            }
            set
            {
                m_AddressLength = value;
            }
        }

        /// <summary>
        /// Gets the Maximum size of an APDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created
        
        public ushort MaxAPDUSize
        {
            get
            {
                UnnumberedFrame APDUFrame = new UnnumberedFrame(UnnumberedCommands.UnnumberedInformation);
                LPDU LPDU = new DLMS.LPDU();

                APDUFrame.DestinationAddress = HDLCFrame.GenerateAddressBytes(ServerPort, PhysicalAddress, AddressLength);
                APDUFrame.SourceAddress = HDLCFrame.GenerateAddressBytes(ClientPort);
                APDUFrame.Payload = LPDU.Data;

                return (ushort)(TransmitMaxPacketSize - APDUFrame.Frame.Length);
            }
        }

        /// <summary>
        /// Gets or sets the Global Encryption Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] GlobalEncryptionKey
        {
            get
            {
                return m_GlobalEncryptionKey;
            }
            set
            {
                m_GlobalEncryptionKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the Dedicated Encryption Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] DedicatedEncryptionKey
        {
            get
            {
                return m_DedicatedEncryptionKey;
            }
            set
            {
                m_DedicatedEncryptionKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the Authentication Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] DecryptAuthenticationKey
        {
            get
            {
                return m_DecryptAuthenticationKey;
            }
            set
            {
                m_DecryptAuthenticationKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the Pending Authentication Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/16/13 RCG 2.85.20 N/A    Created
        
        public byte[] PendingDecryptAuthenticationKey
        {
            get
            {
                return m_PendingDecryptAuthenticationKey;
            }
            set
            {
                m_PendingDecryptAuthenticationKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the Server ApTitle
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] ServerApTitle
        {
            get
            {
                return m_ServerApTitle;
            }
            set
            {
                m_ServerApTitle = value;
            }
        }

        /// <summary>
        /// Gets the Last Frame Counter Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public uint LastFrameCounterReceived
        {
            get
            {
                return m_LastFrameCounterReceived;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the protocol should use numbered information frames
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/26/13 RCG 2.85.02 N/A    Created
        
        public bool UseNumberedInformationFrames
        {
            get
            {
                return m_UseNumberedInformationFrames;
            }
            set
            {
                m_UseNumberedInformationFrames = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the SNRM command should be sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/12/13 RCG 2.85.41 N/A    Created

        public bool SendSNRMCommand
        {
            get
            {
                return m_SendSNRMCommand;
            }
            set
            {
                m_SendSNRMCommand = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the Keep Alive should be disabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/12/13 RCG 2.85.41 N/A    Created
        
        public bool KeepAliveDisabled
        {
            get
            {
                return m_KeepAliveDisabled;
            }
            set
            {
                m_KeepAliveDisabled = value;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets the Send Sequence Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private byte SendSequenceNumber
        {
            get
            {
                return m_SendSequenceNumber;
            }
            set
            {
                // The value is the bits so the max is 8
                m_SendSequenceNumber = (byte)(value % 8);
            }
        }

        /// <summary>
        /// Gets or sets the Receive Sequence Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private byte ReceiveSequenceNumber
        {
            get
            {
                return m_ReceiveSequenceNumber;
            }
            set
            {
                m_ReceiveSequenceNumber = (byte)(value % 8);
            }
        }

        #endregion

        #region Member Variables

        private SerialPort m_Port;
        private bool m_IsOpen;
        private List<byte> m_HDLCBuffer;
        private List<HDLCFrame> m_ReceivedFrames;
        private List<byte> m_InformationBuffer;
        private Logger m_Logger;
        private Timer m_KeepAliveTimer;
        private SerialDataReceivedEventHandler m_DataReceivedHandler;

        private byte m_SendSequenceNumber;
        private byte m_ReceiveSequenceNumber;
        private bool m_UseNumberedInformationFrames;
        private bool m_SendSNRMCommand;
        private bool m_KeepAliveDisabled;

        private ushort m_TransmitMaxPacketSize;
        private ushort m_ReceiveMaxPacketSize;
        private int m_InterPacketTimeout;
        private uint m_TransmitWindowLength;
        private uint m_ReceiveWindowLength;

        private ushort m_PhysicalAddress;
        private ushort m_ClientPort;
        private ushort m_ServerPort;
        private HDLCAddressLength m_AddressLength;

        private byte[] m_GlobalEncryptionKey;
        private byte[] m_DedicatedEncryptionKey;
        private byte[] m_DecryptAuthenticationKey;
        private byte[] m_PendingDecryptAuthenticationKey;
        private byte[] m_ServerApTitle;
        private uint m_LastFrameCounterReceived;

        #endregion

    }

    /// <summary>
    /// LLC Protocol Data Unit
    /// </summary>
    public class LPDU
    {
        #region Constants

        /// <summary>
        /// The default LSAP for HDLC
        /// </summary>
        public const byte DEFAULT_LSAP = 0xE6;
        /// <summary>
        /// The default source LSAP when receiving a value
        /// </summary>
        public const byte DEFAULT_RECEIVED_LSAP = 0xE7;
        /// <summary>
        /// The default quality for COSEM
        /// </summary>
        public const byte DEFAULT_QUALITY = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created

        public LPDU()
        {
            m_DestinationLSAP = DEFAULT_LSAP;
            m_SourceLSAP = DEFAULT_LSAP;
            m_Quality = DEFAULT_QUALITY;
            m_Information = new byte[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="destinationLSAP">The destination LSAP</param>
        /// <param name="sourceLSAP">The source LSAP</param>
        /// <param name="quality">The quality</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created
        
        public LPDU(byte destinationLSAP, byte sourceLSAP, byte quality)
        {
            m_DestinationLSAP = destinationLSAP;
            m_SourceLSAP = sourceLSAP;
            m_Quality = quality;
            m_Information = new byte[0];
        }

        /// <summary>
        /// Creates an LPDU from the specified data
        /// </summary>
        /// <param name="data">The data to parse from</param>
        /// <returns>The resulting LPDU</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created
        
        public static LPDU Parse(byte[] data)
        {
            LPDU Result = null;

            if (data != null && data.Length >= 3)
            {
                Result = new LPDU(data[0], data[1], data[2]);

                if(data.Length > 3)
                {
                    byte[] Information = new byte[data.Length - 3];

                    Array.Copy(data, 3, Information, 0, Information.Length);

                    Result.Information = Information;
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Destination LSAP
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created
        
        public byte DestinationLSAP
        {
            get
            {
                return m_DestinationLSAP;
            }
            set
            {
                m_DestinationLSAP = value;
            }
        }

        /// <summary>
        /// Gets or sets the Source LSAP
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created
        
        public byte SourceLSAP
        {
            get
            {
                return m_SourceLSAP;
            }
            set
            {
                m_SourceLSAP = value;
            }
        }

        /// <summary>
        /// Gets or sets the Quality
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created

        public byte Quality
        {
            get
            {
                return m_Quality;
            }
            set
            {
                m_Quality = value;
            }
        }

        /// <summary>
        /// Gets or sets the Information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created
        
        public byte[] Information
        {
            get
            {
                return m_Information;
            }
            set
            {
                m_Information = value;
            }
        }

        /// <summary>
        /// Gets the LPDU Data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/13 RCG 2.80.39 N/A    Created
        
        public byte[] Data
        {
            get
            {
                int Length = 3;
                byte[] CurrentData;

                if (m_Information != null)
                {
                    Length += m_Information.Length;
                }

                CurrentData = new byte[Length];

                CurrentData[0] = m_DestinationLSAP;
                CurrentData[1] = m_SourceLSAP;
                CurrentData[2] = m_Quality;

                if (m_Information != null && m_Information.Length > 0)
                {
                    Array.Copy(m_Information, 0, CurrentData, 3, m_Information.Length);
                }

                return CurrentData;
            }
        }

        #endregion

        #region Member Variables

        private byte m_DestinationLSAP;
        private byte m_SourceLSAP;
        private byte m_Quality;
        private byte[] m_Information;

        #endregion
    }

    /// <summary>
    /// Basic frame for the HDLC protocol
    /// </summary>
    public class HDLCFrame
    {
        #region Constants

        private const byte FRAME_BOUNDARY = 0x7E;
        private const byte POLL_FINAL_BIT = 0x10;
        private const byte BIT_0 = 0x01;
        private const byte BIT_1 = 0x02;

        // Frame Format Constants
        private const ushort FORMAT_TYPE = 0xA000;
        private const ushort SEGMENTATION_MASK = 0x0800;
        private const ushort LENGTH_MASK = 0x07FF;

        private const ushort ADDRESS_SPLIT_VALUE = 0x7F;

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses a new HDLC frame from the stream
        /// </summary>
        /// <param name="dataStream">The stream containing the data</param>
        /// <returns>The first valid HDLC frame in the stream</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public static HDLCFrame Parse(Stream dataStream)
        {
            HDLCFrame NewFrame = null;
            long StartPosition = -1;

            // Find the start of the frame
            while (dataStream.Position < dataStream.Length)
            {
                // ReadByte returns an int and returns -1 if nothing left to read but we are checking the position so that should never happen
                byte CurrentByte = (byte)dataStream.ReadByte();

                if (CurrentByte == FRAME_BOUNDARY)
                {
                    StartPosition = dataStream.Position;
                    break;
                }
            }

            if (StartPosition >= 0)
            {
                // We found the start of a frame so we can continue parsing

                if (dataStream.Length - dataStream.Position >= 2) // Need at least two bytes to read the Frame Format
                {
                    dataStream.Seek(StartPosition, SeekOrigin.Begin);
                    DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

                    ushort ReadHCS = 0;
                    ushort ReadFCS = 0;
                    List<byte> AddressBytes = new List<byte>();
                    HDLCFrame FrameRead = new HDLCFrame();

                    FrameRead.FrameFormat = DataReader.ReadUInt16();

                    // We should have the frame length at this point so we need to verify that we have enough data to continue parsing

                    if (FrameRead.Length + 1 <= dataStream.Length - StartPosition) // Add one for the terminating frame boundary
                    {
                        // The Destination Address can be variable length so read bytes until we find one with bit 0 set
                        do
                        {
                            AddressBytes.Add(DataReader.ReadByte());
                        } while ((byte)(AddressBytes[AddressBytes.Count - 1] & BIT_0) != BIT_0);

                        FrameRead.m_DestinationAddress = AddressBytes.ToArray();

                        // The same is true for the Source Address
                        AddressBytes.Clear();

                        do
                        {
                            AddressBytes.Add(DataReader.ReadByte());
                        } while ((byte)(AddressBytes[AddressBytes.Count - 1] & BIT_0) != BIT_0);

                        FrameRead.m_SourceAddress = AddressBytes.ToArray();

                        FrameRead.Control = DataReader.ReadByte();

                        // Read and Check the HCS
                        ReadHCS = (ushort)(DataReader.ReadByte() | (DataReader.ReadByte() << 8)); // The bytes are reversed from how the DLMS Binary Reader reads a UINT16

                        // Check to see if there is any additional data that needs to be read
                        if (dataStream.Position - StartPosition < FrameRead.Length)
                        {
                            // If there is a payload there will always be an FCS so subtract 2 bytes FCS plus final Frame Boundary
                            FrameRead.m_Payload = DataReader.ReadBytes((int)(FrameRead.Length - (dataStream.Position - StartPosition) - 2));

                            ReadFCS = (ushort)(DataReader.ReadByte() | (DataReader.ReadByte() << 8)); // The bytes are reversed from how the DLMS Binary Reader reads a UINT16

                            if (ReadFCS != FrameRead.CurrentFCS)
                            {
                                throw new InvalidDataException("The FCS data in the stream does not match the data that has been read.");
                            }
                        }

                        // Check the HCS last because we need to read the entire frame first
                        if (ReadHCS != FrameRead.CurrentHCS)
                        {
                            throw new InvalidDataException("The HCS data in the stream does not match the data that has been read.");
                        }

                        // Read the last Frame Boundary byte
                        if (DataReader.ReadByte() != FRAME_BOUNDARY)
                        {
                            throw new InvalidDataException("Frame boundary not found at end of frame");
                        }

                        // Convert the HDLC Frame to the appropriate type
                        if ((FrameRead.Control & BIT_0) == 0)
                        {
                            // Information frame
                            NewFrame = new InformationFrame(FrameRead);
                        }
                        else if ((FrameRead.Control & BIT_1) == 0)
                        {
                            // Supervisory Frame
                            NewFrame = new SupervisoryFrame(FrameRead);
                        }
                        else
                        {
                            // Unnumbered Frame
                            NewFrame = new UnnumberedFrame(FrameRead);
                        }
                    }
                    else
                    {
                        // We must be missing some data so lets move it back to the starting position
                        dataStream.Seek(StartPosition - 1, SeekOrigin.Begin);
                    }
                }
                else
                {
                    // We must be missing some data so lets move it back to the starting position
                    dataStream.Seek(StartPosition - 1, SeekOrigin.Begin);
                }
            }

            return NewFrame;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        protected HDLCFrame()
        {
            m_DestinationAddress = HDLCFrame.GenerateAddressBytes(0, 0, HDLCAddressLength.One);
            m_SourceAddress = HDLCFrame.GenerateAddressBytes(0);
            m_Segmentation = false;
            m_Control = 0;
            m_Payload = null;
            m_Length = 0;

            IsPollFinalBitSet = true;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other">The HDLCFrame object to copy</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        protected HDLCFrame(HDLCFrame other)
        {
            m_DestinationAddress = other.m_DestinationAddress;
            m_SourceAddress = other.m_SourceAddress;
            m_Segmentation = other.m_Segmentation;
            m_Control = other.m_Control;
            m_Payload = other.m_Payload;
            m_Length = other.Length;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the destination address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public byte[] DestinationAddress
        {
            get
            {
                return m_DestinationAddress;
            }
            set
            {
                m_DestinationAddress = value;

                m_Length = CalculateCurrentLength();
            }
        }

        /// <summary>
        /// Gets or sets the source address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public byte[] SourceAddress
        {
            get
            {
                return m_SourceAddress;
            }
            set
            {
                m_SourceAddress = value;

                m_Length = CalculateCurrentLength();
            }
        }

        /// <summary>
        /// Gets or sets the control byte
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public byte Control
        {
            get
            {
                return m_Control;
            }
            set
            {
                m_Control = value;
            }
        }

        /// <summary>
        /// Gets or sets the payload data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public byte[] Payload
        {
            get
            {
                return m_Payload;
            }
            set
            {
                m_Payload = value;

                m_Length = CalculateCurrentLength();
            }
        }

        /// <summary>
        /// Gets the raw frame data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public byte[] Frame
        {
            get
            {
                MemoryStream FrameStream = new MemoryStream();
                DLMSBinaryWriter FrameWriter = new DLMSBinaryWriter(FrameStream);

                // Write the frame start
                FrameWriter.Write(FRAME_BOUNDARY);

                // Write the frame data
                FrameWriter.Write(CreateFrameData());

                // Write the frame end
                FrameWriter.Write(FRAME_BOUNDARY);

                return FrameStream.ToArray();
            }
        }

        /// <summary>
        /// Gets whether or not the Poll/Final bit is currently set
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public bool IsPollFinalBitSet
        {
            get
            {
                return (m_Control & POLL_FINAL_BIT) == POLL_FINAL_BIT;
            }
            set
            {
                if (value)
                {
                    m_Control = (byte)(m_Control | POLL_FINAL_BIT);
                }
                else
                {
                    m_Control = (byte)(m_Control & ~POLL_FINAL_BIT);
                }
            }
        }

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created
        
        public ushort Length
        {
            get
            {
                return m_Length;
            }
            set
            {
                if (value <= LENGTH_MASK)
                {
                    m_Length = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The Length must be less than or equal to " + LENGTH_MASK.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// Gets the HCS for the current frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public ushort CurrentHCS
        {
            get
            {
                return CRC.CalculateCRCCCIT(CreateHeaderData());
            }
        }

        /// <summary>
        /// Gets the FCS for the current frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public ushort CurrentFCS
        {
            get
            {
                ushort FCS = 0;

                // There is no FCS if there is no payload
                if (m_Payload != null && m_Payload.Length > 0)
                {
                    MemoryStream FrameStream = new MemoryStream();
                    BinaryWriter FrameWriter = new BinaryWriter(FrameStream); // This must be a regular binary writer to get the correct byte order for CRCs

                    FrameWriter.Write(CreateHeaderData());

                    // Calculate the HCS
                    FrameWriter.Write(CRC.CalculateCRCCCIT(FrameStream.ToArray()));

                    FrameWriter.Write(m_Payload);

                    // Calculate the FCS
                    FCS = CRC.CalculateCRCCCIT(FrameStream.ToArray());
                }

                return FCS;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the current length of the frame
        /// </summary>
        /// <returns>The length of the frame in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private ushort CalculateCurrentLength()
        {
            ushort CurrentLength = (ushort)(m_DestinationAddress.Length + m_SourceAddress.Length + 5);

            if (m_Payload != null && m_Payload.Length > 0)
            {
                CurrentLength += (ushort)(2 + m_Payload.Length);
            }

            return CurrentLength;
        }

        /// <summary>
        /// Creates the bulk of the frame data
        /// </summary>
        /// <returns>The frame data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        private byte[] CreateFrameData()
        {
            MemoryStream FrameStream = new MemoryStream();
            BinaryWriter FrameWriter = new BinaryWriter(FrameStream); // This must be a regular binary writer so the CRC's byte order is reversed

            FrameWriter.Write(CreateHeaderData());

            // Calculate the HCS
            FrameWriter.Write(CRC.CalculateCRCCCIT(FrameStream.ToArray()));

            if (m_Payload != null && m_Payload.Count() > 0)
            {
                FrameWriter.Write(m_Payload);

                // Calculate the FCS
                FrameWriter.Write(CRC.CalculateCRCCCIT(FrameStream.ToArray()));
            }

            return FrameStream.ToArray();
        }

        /// <summary>
        /// Creates the Header data
        /// </summary>
        /// <returns>The Header data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        private byte[] CreateHeaderData()
        {
            MemoryStream FrameStream = new MemoryStream();
            DLMSBinaryWriter FrameWriter = new DLMSBinaryWriter(FrameStream);

            FrameWriter.Write(FrameFormat);
            FrameWriter.Write(m_DestinationAddress);
            FrameWriter.Write(m_SourceAddress);
            FrameWriter.Write(m_Control);

            return FrameStream.ToArray();
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets the Frame Format
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created
        
        private ushort FrameFormat
        {
            get
            {
                ushort Format = FORMAT_TYPE;

                if(m_Segmentation)
                {
                    Format |= SEGMENTATION_MASK;
                }

                Format |= (ushort)(m_Length & LENGTH_MASK);

                return Format;
            }
            set
            {
                m_Segmentation = (value & SEGMENTATION_MASK) == SEGMENTATION_MASK;
                m_Length = (ushort)(value & LENGTH_MASK);
            }
        }

        /// <summary>
        /// Gets the Address Bytes from the Logical and Physical address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created

        public static byte[] GenerateAddressBytes(ushort logicalAddress, ushort physicalAddress, HDLCAddressLength length)
        {
            byte[] Destination = null;

            // Bit 0 of each byte indicates whether there is more data 0 = more data 1 = last byte
            // We need to shift the data around so that it fits this format.

            switch(length)
            {
                case HDLCAddressLength.One:
                {
                    Destination = new byte[1];

                    Destination[0] = (byte)((logicalAddress << 1) | 0x01);
                    break;
                }
                case HDLCAddressLength.Two:
                {
                    Destination = new byte[2];

                    Destination[0] = (byte)(logicalAddress << 1);
                    Destination[1] = (byte)((physicalAddress << 1) | 0x01); // Set the last bit
                    break;
                }
                case HDLCAddressLength.Four:
                {
                    Destination = new byte[4];

                    // Logical Address
                    Destination[0] = (byte)((logicalAddress >> 7) << 1);
                    Destination[1] = (byte)(logicalAddress << 1);

                    // Physical Address
                    Destination[2] = (byte)((physicalAddress >> 7) << 1);
                    Destination[3] = (byte)((physicalAddress << 1) | 0x01); // Set the last bit
                    break;
                }
            }

            return Destination;
        }

        /// <summary>
        /// Gets the Source Address Bytes (only use when sending data)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created

        public static byte[] GenerateAddressBytes(ushort clientAddress)
        {
            byte[] Data = null;

            if (clientAddress <= ADDRESS_SPLIT_VALUE)
            {
                Data = new byte[1];
                Data[0] = (byte)((clientAddress << 1) | 0x01);
            }
            else
            {
                Data = new byte[2];
                Data[0] = (byte)((clientAddress >> 7) << 1);
                Data[1] = (byte)((clientAddress << 1) | 0x01); // Set the last bit
            }

            return Data;
        }

        #endregion

        #region Member Variables

        private bool m_Segmentation;
        private byte[] m_DestinationAddress;
        private byte[] m_SourceAddress;
        private ushort m_Length;
        /// <summary>
        /// The control byte for the message
        /// </summary>
        protected byte m_Control;
        /// <summary>
        /// The payload data for the message
        /// </summary>
        protected byte[] m_Payload;

        #endregion
    }

    /// <summary>
    /// Information Frame for the HDLC Protocol
    /// </summary>
    public class InformationFrame : HDLCFrame
    {
        #region Constants

        private const byte RECEIVE_SEQUENCE_MASK = 0xE0;
        private const int RECEIVE_SEQUENCE_SHIFT = 5;
        private const byte SEND_SEQUENCE_MASK = 0x0E;
        private const int SEND_SEQUENCE_SHIFT = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public InformationFrame()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="other">The HDLCFrame to create the object from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public InformationFrame(HDLCFrame other)
            : base(other)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="receiveSequenceNumber">The receive sequence number</param>
        /// <param name="sendSequenceNumber">The send sequence number</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public InformationFrame(byte receiveSequenceNumber, byte sendSequenceNumber)
            : base()
        {
            ReceiveSequenceNumber = receiveSequenceNumber;
            SendSequenceNumber = sendSequenceNumber;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Receive sequence number of the frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public byte ReceiveSequenceNumber
        {
            get
            {
                return (byte)((m_Control & RECEIVE_SEQUENCE_MASK) >> RECEIVE_SEQUENCE_SHIFT);
            }
            set
            {
                byte MaxValue = (byte)(RECEIVE_SEQUENCE_MASK >> RECEIVE_SEQUENCE_SHIFT);
                byte NewValue = (byte)(value % (MaxValue + 1));

                m_Control = (byte)((m_Control & ~RECEIVE_SEQUENCE_MASK) | ((NewValue << RECEIVE_SEQUENCE_SHIFT) & RECEIVE_SEQUENCE_MASK));
            }
        }

        /// <summary>
        /// Gets or sets the Send sequence number of the frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public byte SendSequenceNumber
        {
            get
            {
                return (byte)((m_Control & SEND_SEQUENCE_MASK) >> SEND_SEQUENCE_SHIFT);
            }
            set
            {
                byte MaxValue = (byte)(SEND_SEQUENCE_MASK >> SEND_SEQUENCE_SHIFT);
                byte NewValue = (byte)(value % (MaxValue + 1));

                m_Control = (byte)((m_Control & ~SEND_SEQUENCE_MASK) | ((NewValue << SEND_SEQUENCE_SHIFT) & SEND_SEQUENCE_MASK));
            }

        }

        #endregion
    }

    /// <summary>
    /// Supervisory Command types
    /// </summary>
    public enum SupervisoryCommands : byte
    {
        /// <summary>Receive Ready</summary>
        ReceiveReady = 0x01,
        /// <summary>Receive Not Ready</summary>
        ReceiveNotReady = 0x05,
    }

    /// <summary>
    /// Supervisory Frame for the HDLC Protocol
    /// </summary>
    public class SupervisoryFrame : HDLCFrame
    {
        #region Constants

        private const byte RECEIVE_SEQUENCE_MASK = 0xE0;
        private const int RECEIVE_SEQUENCE_SHIFT = 5;
        private const byte COMMAND_MASK = 0x0F;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public SupervisoryFrame()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="other">The HDLCFrame to create the object from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public SupervisoryFrame(HDLCFrame other)
            : base(other)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The Supervisory command to send</param>
        /// <param name="receiveSequenceNumber">The receive sequence number</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public SupervisoryFrame(SupervisoryCommands command, byte receiveSequenceNumber)
        {
            ReceiveSequenceNumber = receiveSequenceNumber;
            Command = command;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Receive sequence number of the frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public byte ReceiveSequenceNumber
        {
            get
            {
                return (byte)((m_Control & RECEIVE_SEQUENCE_MASK) >> RECEIVE_SEQUENCE_SHIFT);
            }
            set
            {
                byte MaxValue = (byte)(RECEIVE_SEQUENCE_MASK >> RECEIVE_SEQUENCE_SHIFT);
                byte NewValue = (byte)(value % (MaxValue + 1));

                m_Control = (byte)((m_Control & ~RECEIVE_SEQUENCE_SHIFT) | ((NewValue << RECEIVE_SEQUENCE_SHIFT) & RECEIVE_SEQUENCE_MASK));
            }
        }

        /// <summary>
        /// Gets or sets the Command
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public SupervisoryCommands Command
        {
            get
            {
                return (SupervisoryCommands)(m_Control & COMMAND_MASK);
            }
            set
            {
                m_Control = (byte)((m_Control & ~COMMAND_MASK) | (byte)value);
            }
        }

        #endregion
    }

    /// <summary>
    /// Unnumbered frame commands
    /// </summary>
    public enum UnnumberedCommands : byte
    {
        /// <summary>Set Normal Response Mode</summary>
        SetNormalResponseMode = 0x83,
        /// <summary>Disconnect</summary>
        Disconnect = 0x43,
        /// <summary>Unnumbered Acknowledge</summary>
        UnnumberedAcknowledge = 0x63,
        /// <summary>Disconnect Mode</summary>
        DisconnectMode = 0x0F,
        /// <summary>Frame Reject</summary>
        FrameReject = 0x87,
        /// <summary>Unnumbered Information</summary>
        UnnumberedInformation = 0x03,
    }

    /// <summary>
    /// Unnumbered Frame for the HDLC Protocol (may be a command or response)
    /// </summary>
    public class UnnumberedFrame : HDLCFrame
    {
        #region Constants

        private const byte COMMAND_MASK = 0xEF;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public UnnumberedFrame()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="other">The HDLCFrame to create the object from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created

        public UnnumberedFrame(HDLCFrame other)
            : base(other)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The unnumbered command</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public UnnumberedFrame(UnnumberedCommands command)
        {
            Command = command;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/28/13 RCG 2.80.05 N/A    Created
        
        public UnnumberedCommands Command
        {
            get
            {
                return (UnnumberedCommands)(m_Control & COMMAND_MASK);
            }
            set
            {
                m_Control = (byte)((m_Control & ~COMMAND_MASK) | (byte)value);
            }
        }

        #endregion
    }
}
