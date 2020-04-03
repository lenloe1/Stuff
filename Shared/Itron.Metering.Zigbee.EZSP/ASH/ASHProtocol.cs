///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Itron.Metering.Utilities;

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// This class represents the low level protocol for EZSP.
    /// </summary>
    /// <remarks>ASH Protocol documentation can be found here: http://www.ember.com/pdf/120-3010-000_UARTGatewayProtocolReference.pdf </remarks>
    public class ASHProtocol
    {
        #region Definitions

        /// <summary>
        /// Error codes
        /// </summary>
        public enum ASHError : byte
        {
            /// <summary>
            /// Reset: Unknown
            /// </summary>
            UnknownReset = 0x00,
            /// <summary>
            /// Reset: External
            /// </summary>
            ExternalReset = 0x01,
            /// <summary>
            /// Reset: Power-on
            /// </summary>
            PowerOnReset = 0x02,
            /// <summary>
            /// Reset: Watchdog
            /// </summary>
            WatchdogReset = 0x03,
            /// <summary>
            /// Reset: Assert
            /// </summary>
            AssertReset = 0x06,
            /// <summary>
            /// Reset: Bootloader
            /// </summary>
            BootLoaderReset = 0x09,
            /// <summary>
            /// Reset: Software
            /// </summary>
            SoftwareReset = 0x0B,
            /// <summary>
            /// Exceeded maximum ack timeout count
            /// </summary>
            AckTimeout = 0x51,
        }

        #endregion

        #region Constants

        // Reserved bytes for the UART protocol
        private const byte FLAG_BYTE = 0x7E;
        private const byte ESCAPE_BYTE = 0x7D;
        private const byte XON = 0x11;
        private const byte XOFF = 0x13;
        private const byte SUBSTITUTE_BYTE = 0x18;
        private const byte CANCEL_FLAG = 0x1A;
        private static readonly byte[] SLIP_DISABLE = { 0xC0, 0x50, 0x60, 0xC0 };

        // Control byte masks
        private const byte FRAME_NUMBER_MASK = 0x70;
        private const byte ACK_NUMBER_MASK = 0x07;

        private const byte BIT_5 = 0x20;

        // ASH Constants
        private const int T_RSTACK_MAX = 2500; // Time out to receive the reset ack
        private const int RESET_RETRIES = 5;
        private const int ACK_TIMEOUT = 800;
        private const int ACK_RETRIES = 3;

        #endregion

        #region Public Events

        /// <summary>
        /// Event that occurs whenever a new frame has been received
        /// </summary>
        public event EventHandler FrameReceived;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comm">The serial port communications object to be used.</param>
        /// <param name="logger">The EZSP logger</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ASHProtocol(EZSPSerialCommunications comm, EZSPLogger logger)
        {
            m_SerialComm = comm;
            m_DataReceivedHandler = new EventHandler(m_SerialComm_DataReceived);

            m_Resetting = false;
            m_Connected = false;

            m_Connected = false;
            m_HostFrameNumber = 0;
            m_NCPFrameNumber = 0;
            m_LastError = 0;

            m_Logger = logger;

            m_ReceivedFrames = new List<ASHFrame>();
            m_SentFrames = new List<ASHFrame>();
        }

        /// <summary>
        /// Starts the ASH protocol
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void Connect()
        {
            if (m_SerialComm.IsOpen && m_Connected == false)
            {                
                m_SerialComm.DataReceived += m_DataReceivedHandler;

                // In order to enter the connected state we need to successfully reset the NCP
                if (Reset())
                {
                    m_Connected = true;
                    m_HostFrameNumber = 0;
                    m_NCPFrameNumber = 0;

                    m_ReceivedFrames = new List<ASHFrame>();
                    m_SentFrames = new List<ASHFrame>();

                    m_AckCheckTimer = new Timer(new TimerCallback(CheckForUnacknowledgedFrame));
                }
                else
                {
                    // The reset failed so we should assume we did not connect
                    m_Connected = false;
                    m_Resetting = false;
                    m_SerialComm.DataReceived -= m_DataReceivedHandler;
                }
            }
        }

        /// <summary>
        /// Stops the ASH protocol
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void Disconnect()
        {
            if (m_Connected)
            {
                m_SerialComm.DataReceived -= m_DataReceivedHandler;
                m_Connected = false;
            }
        }

        /// <summary>
        /// Sends the specified data
        /// </summary>
        /// <param name="data">The data to send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SendData(byte[] data)
        {
            if (m_Connected)
            {
                ASHFrame NewDataFrame = new ASHFrame();

                NewDataFrame.ASHFrameType = ASHFrame.FrameType.Data;
                NewDataFrame.FrameNumber = m_HostFrameNumber;
                NewDataFrame.AckNumber = m_NCPFrameNumber;
                NewDataFrame.Data = data;

                m_HostFrameNumber = (byte)((m_HostFrameNumber + 1) % 8);

                SendFrame(NewDataFrame);
            }
            else
            {
                throw new InvalidOperationException("ASH needs to be connected before data can be sent");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not ASH is currently connected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool Connected
        {
            get
            {
                return m_Connected;
            }
        }

        /// <summary>
        /// Gets the list of received frames
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public List<ASHFrame> ReceivedFrames
        {
            get
            {
                return m_ReceivedFrames;
            }
        }

        /// <summary>
        /// Gets the last error code received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ASHError LastErrorCode
        {
            get
            {
                return (ASHError)m_LastError;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks for any unacknowledged frames
        /// </summary>
        /// <param name="state">Timer state</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void CheckForUnacknowledgedFrame(object state)
        {
            foreach (ASHFrame CurrentFrame in m_SentFrames.Where(f => (DateTime.Now - f.TimeStamp).TotalMilliseconds > T_RSTACK_MAX))
            {
            }
        }

        /// <summary>
        /// Raises the Frame Received event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnFrameReceived()
        {
            if (FrameReceived != null)
            {
                FrameReceived(this, new EventArgs());
            }
        }

        /// <summary>
        /// Sends the Cancel Message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/14 RCG 3.50.73 487800 Created
        
        private void SendCancel()
        {
            // Send the Cancel Flag to clear any remaining commands
            m_SerialComm.SendData(new byte[] { CANCEL_FLAG });

            Thread.Sleep(25);
        }

        /// <summary>
        /// Sends the Disable Slip command
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/14 RCG 3.50.73 487800 Created
        
        private void DisableSlip()
        {
            m_SerialComm.SendData(SLIP_DISABLE);
            Thread.Sleep(25);
        }

        /// <summary>
        /// Resets the NCP.
        /// </summary>
        /// <returns>True if the reset was successful. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private bool Reset()
        {
            bool HasReset = false;
            int RetryAttempt = 0;

            DisableSlip();

            while (HasReset == false && RetryAttempt < RESET_RETRIES)
            {
                SendCancel();
                Thread.Sleep(10);
                SendCancel();
                Thread.Sleep(10);
                SendCancel();
                Thread.Sleep(10);

                // Clear any data we may have received
                m_SerialComm.ClearBuffer();

                // Send the Reset Frame
                ASHFrame ResetFrame = new ASHFrame();
                ResetFrame.ASHFrameType = ASHFrame.FrameType.Reset;

                m_Resetting = true;

                SendFrame(ResetFrame);

                // Wait for the Reset Ack
                while ((DateTime.Now - ResetFrame.TimeStamp).TotalMilliseconds < T_RSTACK_MAX)
                {
                    if (m_ReceivedFrames.Where(f => f.ASHFrameType == ASHFrame.FrameType.ResetAck).Count() > 0)
                    {
                        // We received the Reset Ack
                        HasReset = true;
                        m_Resetting = false;

                        // Clear the received items
                        m_ReceivedFrames.Clear();
                        break;
                    }
                    else
                    {
                        // We are still waiting for the ack
                        Thread.Sleep(25);
                    }
                }

                if (HasReset == false)
                {
                    // Haven't found it so we will need to retry
                    RetryAttempt++;
                }
            }

            if (HasReset)
            {
                // Send an Ack to indicate we are good to go
                ASHFrame AckFrame = new ASHFrame();
                AckFrame.ASHFrameType = ASHFrame.FrameType.Ack;

                SendFrame(AckFrame);
            }

            return HasReset;
        }

        /// <summary>
        /// Sends the specified frame
        /// </summary>
        /// <param name="frame">The frame to send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void SendFrame(ASHFrame frame)
        {
            if (m_SerialComm.IsOpen)
            {
                byte[] StuffedFrame = ASHFrame.StuffFrame(frame.RawFrame);
                byte[] DataToSend = new byte[StuffedFrame.Length + 1];
                frame.TimeStamp = DateTime.Now;

                Array.Copy(StuffedFrame, DataToSend, StuffedFrame.Length);
                DataToSend[DataToSend.Length - 1] = FLAG_BYTE;

                if (m_Logger != null)
                {
                    m_Logger.WriteASHFrameInfo(EZSPLogDirection.Send, frame);
                }

                // Send the Frame
                lock (m_SerialComm)
                {
                    m_SerialComm.SendData(DataToSend);
                }

                switch(frame.ASHFrameType)
                {
                    case ASHFrame.FrameType.Data:
                    {
                        if (m_SentFrames.Contains(frame) == false)
                        {
                            m_SentFrames.Add(frame);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Data Received event
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void m_SerialComm_DataReceived(object sender, EventArgs e)
        {
            // We received data so we should attempt to read the next frame
            if (m_SerialComm.IsOpen)
            {
                byte[] NewFrameData = null;

                do
                {
                    lock (m_SerialComm)
                    {
                        NewFrameData = m_SerialComm.ReadNextFrame();
                    }

                    if (NewFrameData != null)
                    {
                        ASHFrame NewFrame = new ASHFrame(ASHFrame.UnstuffFrame(NewFrameData));
                        NewFrame.TimeStamp = DateTime.Now;

                        if (m_Logger != null)
                        {
                            m_Logger.WriteASHFrameInfo(EZSPLogDirection.Receive, NewFrame);
                        }

                        if (m_Resetting)
                        {
                            // Ignore everything but the reset Ack
                            if (NewFrame.ASHFrameType == ASHFrame.FrameType.ResetAck)
                            {
                                // The Reset method should be looking for this response so add it to the list of received frames
                                m_ReceivedFrames.Add(NewFrame);
                            }
                        }
                        else
                        {
                            switch (NewFrame.ASHFrameType)
                            {
                                case ASHFrame.FrameType.Data:
                                {
                                    // Check to make sure that the CRC is valid and that the frame number is correct
                                    if (NewFrame.IsCRCValid && NewFrame.FrameNumber == m_NCPFrameNumber)
                                    {
                                        ASHFrame AckFrame = new ASHFrame();
                                        AckFrame.ASHFrameType = ASHFrame.FrameType.Ack;
                                        AckFrame.AckNumber = (byte)((NewFrame.FrameNumber + 1) % 8);

                                        // Send the ack
                                        SendFrame(AckFrame);

                                        m_NCPFrameNumber = (byte)((m_NCPFrameNumber + 1) % 8);

                                        // Add the data frame and notify
                                        m_ReceivedFrames.Add(NewFrame);
                                        RemoveAcknowledgedFrames(NewFrame.AckNumber);

                                        OnFrameReceived();
                                    }
                                    else if (NewFrame.IsCRCValid && NewFrame.IsRetransmitted)
                                    {
                                        ASHFrame AckFrame = new ASHFrame();
                                        AckFrame.ASHFrameType = ASHFrame.FrameType.Ack;
                                        AckFrame.AckNumber = NewFrame.FrameNumber;

                                        // Send the ack
                                        SendFrame(AckFrame);

                                        // If the frame currently exists remove it
                                        m_ReceivedFrames.RemoveAll(f => f.FrameNumber == NewFrame.FrameNumber);

                                        // Add the data frame and notify
                                        m_ReceivedFrames.Add(NewFrame);
                                        RemoveAcknowledgedFrames(NewFrame.AckNumber);

                                        OnFrameReceived();
                                    }
                                    else
                                    {
                                        if (m_RejectCondition == false)
                                        {
                                            m_RejectCondition = true;

                                            ASHFrame NakFrame = new ASHFrame();
                                            NakFrame.ASHFrameType = ASHFrame.FrameType.Nak;
                                            NakFrame.AckNumber = NewFrame.AckNumber;

                                            SendFrame(NakFrame);
                                        }
                                    }

                                    break;
                                }
                                case ASHFrame.FrameType.Ack:
                                {
                                    // A frame was acknowledged so it should be cleared from the list of pending items
                                    RemoveAcknowledgedFrames(NewFrame.AckNumber);
                                    break;
                                }
                                case ASHFrame.FrameType.Nak:
                                {
                                    // We received a Nak for something sent so we need to retransmit
                                    byte CurrentAckNumber = NewFrame.AckNumber;

                                    while (CurrentAckNumber != m_HostFrameNumber)
                                    {
                                        if (m_SentFrames.Where(f => f.FrameNumber == CurrentAckNumber).Count() > 0)
                                        {
                                            // Update and retransmit the frame
                                            ASHFrame RetransmittedFrame = m_SentFrames.First(f => f.FrameNumber == CurrentAckNumber);
                                            RetransmittedFrame.IsRetransmitted = true;

                                            SendFrame(RetransmittedFrame);
                                        }

                                        CurrentAckNumber = (byte)((CurrentAckNumber + 1) % 8);
                                    }
                                    break;
                                }
                                case ASHFrame.FrameType.Error:
                                {
                                    // Errors are fatal meaning this sessions is now over
                                    m_LastError = NewFrame.Data[1];
                                    Disconnect();
                                    break;
                                }
                                default:
                                {
                                    // We should not be receiving Resets or Reset Acks now this should be considered an error
                                    Disconnect();
                                    break;
                                }
                            }
                        }
                    }
                } while (NewFrameData != null);
            }
        }

        /// <summary>
        /// Removes and acknowledged frames from the list
        /// </summary>
        /// <param name="ackNumber">The frame number of the highest frame to remove</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void RemoveAcknowledgedFrames(byte ackNumber)
        {
            if (m_HostFrameNumber < ackNumber)
            {
                m_SentFrames.RemoveAll(f => f.FrameNumber > m_HostFrameNumber && f.FrameNumber <= ackNumber);
            }
            else
            {
                m_SentFrames.RemoveAll(f => f.FrameNumber <= ackNumber);
            }
        }

        /// <summary>
        /// Retransmits the specified frame
        /// </summary>
        /// <param name="frame">The frame to retransmit</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void RetransmitFrame(ASHFrame frame)
        {
            frame.AckNumber = m_NCPFrameNumber;
            frame.IsRetransmitted = true;
            frame.RetransmitCount++;

            SendFrame(frame);
        }

        #endregion

        #region Member Variables

        private EZSPSerialCommunications m_SerialComm;
        private List<ASHFrame> m_ReceivedFrames;
        private List<ASHFrame> m_SentFrames;
        private byte m_HostFrameNumber;
        private byte m_NCPFrameNumber;
        private bool m_Connected;
        private bool m_Resetting;
        private bool m_RejectCondition;
        private EventHandler m_DataReceivedHandler;
        private byte m_LastError;
        private Timer m_AckCheckTimer;
        private EZSPLogger m_Logger;

        #endregion

    }

    /// <summary>
    /// Represents a UART frame
    /// </summary>
    public class ASHFrame
    {
        #region Definitions

        /// <summary>
        /// The frame types
        /// </summary>
        public enum FrameType
        {
            /// <summary>
            /// The frame contains data
            /// </summary>
            Data,
            /// <summary>
            /// The frame is an Ack
            /// </summary>
            Ack,
            /// <summary>
            /// The frame is a Nak
            /// </summary>
            Nak,
            /// <summary>
            /// The frame is a reset command
            /// </summary>
            Reset,
            /// <summary>
            /// The frame is a reset ack
            /// </summary>
            ResetAck,
            /// <summary>
            /// The frame is an error
            /// </summary>
            Error,
        }

        #endregion

        #region Constants

        // Reserved bytes
        private const byte FLAG_BYTE = 0x7E;
        private const byte ESCAPE_BYTE = 0x7D;
        private const byte XON = 0x11;
        private const byte XOFF = 0x13;
        private const byte SUBSTITUTE_BYTE = 0x18;
        private const byte CANCEL_FLAG = 0x1A;

        // Individual bit masks
        private const byte BIT_7 = 0x80;
        private const byte BIT_6 = 0x40;
        private const byte BIT_5 = 0x20;
        private const byte BIT_3 = 0x08;
        private const byte BIT_0 = 0x01;

        // Base Control byte values
        private const byte DATA_CONTROL = 0x00;
        private const byte ACK_CONTROL = 0x80;
        private const byte NACK_CONTROL = 0xA0;
        private const byte RESET_CONTROL = 0xC0;
        private const byte RESET_ACK_CONTROL = 0xC1;
        private const byte ERROR_CONTROL = 0xC2;

        // Control byte masks
        private const byte FRAME_NUMBER_MASK = 0x70;
        private const byte ACK_NUMBER_MASK = 0x07;

        private const byte RANDOM_DATA_SEED = 0x42;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ASHFrame()
        {
            m_ControlByte = 0;
            m_Data = null;
            m_CRC = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawFrame">The raw data that contains the entire frame</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ASHFrame(byte[] rawFrame)
        {
            if (rawFrame != null && rawFrame.Length >= 3)
            {
                m_ControlByte = rawFrame[0];
                m_CRC = (ushort)((rawFrame[rawFrame.Length - 2] << 8) + rawFrame[rawFrame.Length - 1]);

                if (ASHFrameType == FrameType.Data || ASHFrameType == FrameType.Error || ASHFrameType == FrameType.ResetAck)
                {
                    byte[] RawData = new byte[rawFrame.Length - 3];

                    Array.Copy(rawFrame, 1, RawData, 0, rawFrame.Length - 3);

                    // The raw frame should still have the data randomized so we should derandomize it by calling
                    // the randomize method on it to reverse the process
                    m_Data = RandomizeData(RawData);
                }
            }
        }

        /// <summary>
        /// Stuffs the frame with bytes if any of the reserved bytes are found in the frame
        /// </summary>
        /// <param name="unstuffedFrame">The frame to stuff</param>
        /// <returns>The stuffed frame.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public static byte[] StuffFrame(byte[] unstuffedFrame)
        {
            byte[] StuffedFrame = null;

            if (unstuffedFrame != null)
            {
                List<byte> StuffedBytes = new List<byte>();

                for (int Index = 0; Index < unstuffedFrame.Length; Index++)
                {
                    switch (unstuffedFrame[Index])
                    {
                        case FLAG_BYTE:
                        case ESCAPE_BYTE:
                        case XON:
                        case XOFF:
                        case SUBSTITUTE_BYTE:
                        case CANCEL_FLAG:
                        {
                            // This byte needs to be stuffed so add the escape byte and then the byte with bit 5 inverted;
                            StuffedBytes.Add(ESCAPE_BYTE);
                            StuffedBytes.Add((byte)(unstuffedFrame[Index] ^ BIT_5));
                            break;
                        }
                        default:
                        {
                            // Just add the byte
                            StuffedBytes.Add(unstuffedFrame[Index]);
                            break;
                        }
                    }
                }

                StuffedFrame = StuffedBytes.ToArray();
            }

            return StuffedFrame;
        }

        /// <summary>
        /// Unstuffs the raw frame that has been read from the device.
        /// </summary>
        /// <param name="stuffedFrame">The raw frame in stuffed format</param>
        /// <returns>The unstuffed frame</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public static byte[] UnstuffFrame(byte[] stuffedFrame)
        {
            byte[] UnstuffedFrame = null;
            bool UnstuffNextByte = false;

            if (stuffedFrame != null)
            {
                List<byte> UnstuffedBytes = new List<byte>();

                for (int Index = 0; Index < stuffedFrame.Length; Index++)
                {
                    if (UnstuffNextByte)
                    {
                        // If the byte has been stuffed then we can undo it by inverting (exclusive or) bit 5
                        UnstuffedBytes.Add((byte)(stuffedFrame[Index] ^ BIT_5));
                        UnstuffNextByte = false;
                    }
                    else if (stuffedFrame[Index] == ESCAPE_BYTE)
                    {
                        // Escape character so we need to unstuff the next byte and don't need to add the byte
                        UnstuffNextByte = true;
                    }
                    else
                    {
                        // It's a normal byte so just add it
                        UnstuffedBytes.Add(stuffedFrame[Index]);
                    }
                }

                UnstuffedFrame = UnstuffedBytes.ToArray();
            }

            return UnstuffedFrame;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the number of times the frame has been retransmitted
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public int RetransmitCount
        {
            get
            {
                return m_RetransmitCount;
            }
            set
            {
                m_RetransmitCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the time that the frame was sent or received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public DateTime TimeStamp
        {
            get
            {
                return m_TimeStamp;
            }
            set
            {
                m_TimeStamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the frame type. Setting this will clear the frame number
        /// </summary>
        /// <exception cref="FormatException">Thrown during a get if the Control Byte is not valid.</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public FrameType ASHFrameType
        {
            get
            {
                FrameType CurrentFrameType;

                if ((m_ControlByte & BIT_7) == 0)
                {
                    CurrentFrameType = FrameType.Data;
                }
                else if ((m_ControlByte & BIT_6) == 0)
                {
                    // ACK or NACK
                    if ((m_ControlByte & BIT_5) == 0)
                    {
                        CurrentFrameType = FrameType.Ack;
                    }
                    else
                    {
                        CurrentFrameType = FrameType.Nak;
                    }
                }
                else
                {
                    switch(m_ControlByte)
                    {
                        case RESET_CONTROL:
                        {
                            CurrentFrameType = FrameType.Reset;
                            break;
                        }
                        case RESET_ACK_CONTROL:
                        {
                            CurrentFrameType = FrameType.ResetAck;
                            break;
                        }
                        case ERROR_CONTROL:
                        {
                            CurrentFrameType = FrameType.Error;
                            break;
                        }
                        default:
                        {
                            throw new FormatException("The frame control byte is not formatted correctly");
                        }
                    }
                }

                return CurrentFrameType;
            }
            set
            {
                switch(value)
                {
                    case FrameType.Data:
                    {
                        m_ControlByte = 0x00;
                        m_Data = new byte[0];
                        break;
                    }
                    case FrameType.Ack:
                    {
                        m_ControlByte = ACK_CONTROL;
                        m_Data = null;
                        break;
                    }
                    case FrameType.Nak:
                    {
                        m_ControlByte = NACK_CONTROL;
                        m_Data = null;
                        break;
                    }
                    case FrameType.Reset:
                    {
                        m_ControlByte = RESET_CONTROL;
                        m_Data = null;
                        break;
                    }
                    case FrameType.ResetAck:
                    {
                        m_ControlByte = RESET_ACK_CONTROL;
                        m_Data = new byte[2];
                        m_Data[0] = 0x04;
                        break;
                    }
                    case FrameType.Error:
                    {
                        m_ControlByte = ERROR_CONTROL;
                        m_Data = new byte[2];
                        m_Data[0] = 0x04;
                        break;
                    }
                }

                m_CRC = CalculateCurrentCRC();
            }
        }

        /// <summary>
        /// Gets the frame number. This value is only valid for Data frames
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the current frame type is not a Data Frame</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is out of range. Valid range: 0-7</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte FrameNumber
        {
            get
            {
                if (ASHFrameType == FrameType.Data)
                {
                    return (byte)((m_ControlByte & FRAME_NUMBER_MASK) >> 4);
                }
                else
                {
                    throw new InvalidOperationException("The frame number is only valid for data frames");
                }
            }
            set
            {
                if (ASHFrameType == FrameType.Data)
                {
                    if (value <= 7)
                    {
                        // Clear the existing frame number if any
                        byte NewControl = (byte)(m_ControlByte & ~FRAME_NUMBER_MASK);

                        // Add the new frame number
                        NewControl |= (byte)((value << 4) & FRAME_NUMBER_MASK);

                        m_ControlByte = NewControl;

                        m_CRC = CalculateCurrentCRC();
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("FrameNumber", "The allowed values for the frame number are 0-7");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The frame number is only valid for data frames");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Ack Number. This value is only valid for Data, Ack, and Nak frames
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the current frame type is not a Data, Ack, or Nak Frame</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is out of range. Valid range: 0-7</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte AckNumber
        {
            get
            {
                switch (ASHFrameType)
                {
                    case FrameType.Data:
                    case FrameType.Ack:
                    case FrameType.Nak:
                    {
                        return (byte)(m_ControlByte & ACK_NUMBER_MASK);
                    }
                    default:
                    {
                        throw new InvalidOperationException("The Ack number is only valid for data, Ack, and Nak frames");
                    }
                }
            }
            set
            {
                switch (ASHFrameType)
                {
                    case FrameType.Data:
                    case FrameType.Ack:
                    case FrameType.Nak:
                    {
                        if (value <= 7)
                        {
                            // Clear the existing frame number if any
                            byte NewControl = (byte)(m_ControlByte & ~ACK_NUMBER_MASK);

                            // Add the new frame number
                            NewControl |= (byte)(value & ACK_NUMBER_MASK);

                            m_ControlByte = NewControl;

                            m_CRC = CalculateCurrentCRC();
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("AckNumber", "The allowed values for the Ack number are 0-7");
                        }

                        break;
                    }
                    default:
                    {
                        throw new InvalidOperationException("The Ack number is only valid for data, Ack, and Nak frames");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not the frame is retransmitted
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the current frame type is not a Data Frame</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool IsRetransmitted
        {
            get
            {
                if (ASHFrameType == FrameType.Data)
                {
                    return (m_ControlByte & BIT_3) == BIT_3;
                }
                else
                {
                    throw new InvalidOperationException("The Retransmitted flag is only valid for data frames");
                }
            }
            set
            {
                if (ASHFrameType == FrameType.Data)
                {
                    // Clear the flag for both cases

                    if (value)
                    {
                        // The set bit 3
                        m_ControlByte = (byte)(m_ControlByte | BIT_3);
                    }
                    else
                    {
                        // Clear bit 3
                        m_ControlByte = (byte)(m_ControlByte & ~BIT_3);
                    }

                    m_CRC = CalculateCurrentCRC();
                }
                else
                {
                    throw new InvalidOperationException("The Retransmitted flag is only valid for data frames");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not the host is inhibiting callbacks. This item is only valid for Ack and Nak frames
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the current frame type is not an Ack, or Nak Frame</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool NotReady
        {
            get
            {
                if (ASHFrameType == FrameType.Ack || ASHFrameType == FrameType.Nak)
                {
                    return (m_ControlByte & BIT_3) == BIT_3;
                }
                else
                {
                    throw new InvalidOperationException("The Retransmitted flag is only valid for Ack and Nak frames");
                }
            }
            set
            {
                if (ASHFrameType == FrameType.Ack || ASHFrameType == FrameType.Nak)
                {
                    // Clear the flag for both cases

                    if (value)
                    {
                        // The set bit 3
                        m_ControlByte = (byte)(m_ControlByte | BIT_3);
                    }
                    else
                    {
                        // Clear bit 3
                        m_ControlByte = (byte)(m_ControlByte & ~BIT_3);
                    }

                    m_CRC = CalculateCurrentCRC();
                }
                else
                {
                    throw new InvalidOperationException("The Retransmitted flag is only valid for Ack and Nak frames");
                }
            }
        }

        /// <summary>
        /// Gets the data contained in the frame
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the current frame type is not a Data or Error Frame</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] Data
        {
            get
            {
                if (ASHFrameType == FrameType.Data || ASHFrameType == FrameType.Error || ASHFrameType == FrameType.ResetAck)
                {
                    return m_Data;
                }
                else
                {
                    throw new InvalidOperationException("Data is only valid for Data and Error frames");
                }
            }
            set
            {
                if (ASHFrameType == FrameType.Data || ASHFrameType == FrameType.Error || ASHFrameType == FrameType.ResetAck)
                {
                    m_Data = value;
                    m_CRC = CalculateCurrentCRC();
                }
                else
                {
                    throw new InvalidOperationException("Data is only valid for Data and Error frames");
                }
            }
        }

        /// <summary>
        /// Gets the frame as raw bytes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] RawFrame
        {
            get
            {
                byte[] Frame;

                if (m_Data == null)
                {
                    Frame = new byte[3];
                }
                else
                {
                    // Before we add the data we need to go ahead and randomize it
                    byte[] RandomizedData = RandomizeData(m_Data);
                    Frame = new byte[3 + RandomizedData.Length];

                    Array.Copy(RandomizedData, 0, Frame, 1, RandomizedData.Length);
                }

                Frame[0] = m_ControlByte;

                Frame[Frame.Length - 1] = (byte)m_CRC;
                Frame[Frame.Length - 2] = (byte)(m_CRC >> 8);

                return Frame;
            }
        }

        /// <summary>
        /// Gets whether or not the CRC is valid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public bool IsCRCValid
        {
            get
            {
                return CalculateCurrentCRC() == m_CRC;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the current CRC for the data
        /// </summary>
        /// <returns>The current CRC</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private ushort CalculateCurrentCRC()
        {
            byte[] DataToCRC;
            int CRCDataLength = 1;

            if (m_Data != null)
            {
                CRCDataLength += m_Data.Length;
            }

            DataToCRC = new byte[CRCDataLength];

            DataToCRC[0] = m_ControlByte;

            if (DataToCRC.Length > 1)
            {
                byte[] RandomizedData = RandomizeData(m_Data);
                Array.Copy(RandomizedData, 0, DataToCRC, 1, RandomizedData.Length);
            }

            return CRC.CalculateCRC16(DataToCRC);
        }

        /// <summary>
        /// Randomizes the data to avoid cases that would cause large amounts of byte stuffing.
        /// If randomized data is run through this method it will be restored to it's original form.
        /// </summary>
        /// <param name="data">The data to randomize</param>
        /// <returns>The randomized data</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private byte[] RandomizeData(byte[] data)
        {
            byte[] RandomizedData = null;

            if (data != null)
            {
                byte[] RandomSequence = GetRandomSequence(data.Length);
                RandomizedData = new byte[data.Length];

                for (int Index = 0; Index < data.Length; Index++)
                {
                    RandomizedData[Index] = (byte)(data[Index] ^ RandomSequence[Index]);
                }
            }

            return RandomizedData;
        }

        /// <summary>
        /// Gets a byte array with the pseudo random sequence
        /// </summary>
        /// <param name="length">The length of the sequence</param>
        /// <returns>The pseudo random sequence</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private byte[] GetRandomSequence(int length)
        {
            byte[] Sequence = new byte[length];

            Sequence[0] = RANDOM_DATA_SEED;

            for (int Index = 1; Index < length; Index++)
            {
                if ((Sequence[Index - 1] & BIT_0) == BIT_0)
                {
                    Sequence[Index] = (byte)((Sequence[Index - 1] >> 1) ^ 0xB8);
                }
                else
                {
                    Sequence[Index] = (byte)(Sequence[Index - 1] >> 1);
                }
            }

            return Sequence;
        }

        #endregion

        #region Member Variables

        private byte m_ControlByte;
        private byte[] m_Data;
        private ushort m_CRC;
        private DateTime m_TimeStamp;
        private int m_RetransmitCount;

        #endregion
    }
}
