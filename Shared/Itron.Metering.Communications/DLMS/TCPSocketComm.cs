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
//                              Copyright © 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;
using System.IO;
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// Handles communication over a network socket
    /// </summary>
    public class TCPSocketComm : IDLMSComm
    {
        #region Constants

        private const int DATA_CHECK_TIME = 75;

        #endregion

        #region Events

        /// <summary>
        /// Event that occurs when data has been received by the meter.
        /// </summary>
        public event EventHandler DataReceived;

        /// <summary>
        /// Event that occurs when an APDU has been received by the meter.
        /// </summary>
        public event APDUEventHandler APDUReceived;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostName">The address of the host to connect to</param>
        /// <param name="port">The port to connect to</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public TCPSocketComm(string hostName, int port)
        {
            m_Socket = null;
            m_HostName = hostName;
            m_Port = port;

            m_GlobalEncryptionKey = null;
            m_DedicatedEncryptionKey = null;
            m_AuthenticationKey = null;
            m_PendingAuthenticationKey = null;
            m_ServerApTitle = null;
            m_LastFrameCounterReceived = 0;

            m_LowLevelDataBuffer = new List<byte>();
            m_HighLevelDataBuffer = new List<byte>();

            m_DataReceivedEventHandler = new EventHandler(m_Comm_DataReceived);
            m_CheckDataTimer = new Timer(CheckForData);

            m_Logger = Logger.TheInstance;
        }

        /// <summary>
        /// Opens the TCP connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public void Open()
        {
            IPAddress HostAddress;
            IPEndPoint HostEndpoint;

            // If we are given a straight IP address as the host name we can parse it. If not we need to look it up from the DNS server.
            if (IPAddress.TryParse(m_HostName, out HostAddress) == false)
            {
                // The parse failed so lets look it up from the DNS server
                IPHostEntry HostDNSEntry = Dns.GetHostEntry(m_HostName);

                // The DNS server may return multiple entries so attempt to connect to each one
                foreach (IPAddress CurrentIPAddress in HostDNSEntry.AddressList)
                {
                    if (CurrentIPAddress.AddressFamily == AddressFamily.InterNetwork
                        || CurrentIPAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        IPEndPoint TestEndpoint = new IPEndPoint(CurrentIPAddress, m_Port);
                        Socket TestSocket = new Socket(CurrentIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        TestSocket.Connect(TestEndpoint);

                        if (TestSocket.Connected)
                        {
                            // We found the correct address
                            HostAddress = CurrentIPAddress;
                            HostEndpoint = TestEndpoint;
                            m_Socket = TestSocket;
                        }
                    }
                }
            }
            else
            {
                HostEndpoint = new IPEndPoint(HostAddress, m_Port);
                m_Socket = new Socket(HostAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.Connect(HostEndpoint);
            }

            if (IsOpen)
            {
                // Set the Receive Buffer size to 16k for larger reads and writes
                m_Socket.ReceiveBufferSize = 16384;
                m_Socket.SendBufferSize = 16384;

                m_LowLevelDataBuffer.Clear();
                m_HighLevelDataBuffer.Clear();

                // Start the data check timer
                m_DataReceivedEventInProgress = false;

                DataReceived += null; // Make sure we clear out an existing handlers
                DataReceived += m_DataReceivedEventHandler;

                m_CheckDataTimer.Change(DATA_CHECK_TIME, DATA_CHECK_TIME);

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Socket opened - Host Name: " + m_HostName + " Port: " + m_Port.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Failed to open socket - Host Name: " + m_HostName + " Port: " + m_Port.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Closes an open socket connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Close()
        {
            if (IsOpen)
            {
                // Stop the timer
                m_CheckDataTimer.Change(Timeout.Infinite, Timeout.Infinite);
                DataReceived -= m_DataReceivedEventHandler;

                m_Socket.Close();
                m_Socket = null;
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
            if (IsOpen && apdu != null)
            {
                WPDU Wrapper = new WPDU(ClientPort, ServerPort, apdu.Data);

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Sending APDU. Type: " + EnumDescriptionRetriever.RetrieveDescription(apdu.Tag));

                Send(Wrapper.Data);
            }
        }

        /// <summary>
        /// Clears any data from the current buffers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created
        
        public void ClearBuffers()
        {
            lock (m_LowLevelDataBuffer)
            {
                m_LowLevelDataBuffer.Clear();
            }

            lock (m_HighLevelDataBuffer)
            {
                m_HighLevelDataBuffer.Clear();
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
        /// Raises the Data Received event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private void OnDataReceived()
        {
            if (DataReceived != null)
            {
                // When we raise the event it will block in this location until the event handlers are complete so we can
                // set and clear this flag so that we aren't constantly raising the event while the handler is still processing
                // the data
                m_DataReceivedEventInProgress = true;

                DataReceived(this, new EventArgs());

                m_DataReceivedEventInProgress = false;
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

        /// <summary>
        /// Checks for available data on the port and raises the event if new data has been found
        /// </summary>
        /// <param name="state">The state object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private void CheckForData(object state)
        {
            if (m_Socket != null && IsOpen)
            {
                if (m_Socket.Available > 0 && m_DataReceivedEventInProgress == false)
                {
                    // New Data has been received
                    OnDataReceived();
                }
            }
        }

        /// <summary>
        /// Handles the Data Received event
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        private void m_Comm_DataReceived(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                lock (m_LowLevelDataBuffer)
                {
                    m_LowLevelDataBuffer.AddRange(Receive());
                }

                // Try to Parse any messages we may have received
                ParseWPDUData();
                ParseAPDUData();
            }
        }

        /// <summary>
        /// Parses any WPDU's found in the data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        private void ParseWPDUData()
        {
            lock (m_LowLevelDataBuffer)
            {
                while (m_LowLevelDataBuffer.Count > 0)
                {
                    int WPDUStart = WPDU.FindStartOfWPDU(ServerPort, ClientPort, m_LowLevelDataBuffer);

                    if (WPDUStart > 0)
                    {
                        // We have some extra data at the start that we can go ahead and throw out
                        m_LowLevelDataBuffer.RemoveRange(0, WPDUStart);

                        // Set the start index to 0 since we got rid of the extra data
                        WPDUStart = 0;
                    }

                    if (WPDUStart == 0)
                    {
                        try
                        {
                            MemoryStream DataStream = new MemoryStream(m_LowLevelDataBuffer.ToArray());

                            WPDU NewWPDU = new WPDU();
                            NewWPDU.Parse(DataStream);

                            // The High Level Buffer contains the raw APDU data
                            lock (m_HighLevelDataBuffer)
                            {
                                m_HighLevelDataBuffer.AddRange(NewWPDU.Payload);
                            }

                            m_LowLevelDataBuffer.RemoveRange(0, (int)DataStream.Position);

                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "WPDU Received.");
                        }
                        catch (Exception ex)
                        {
                            // We failed to parse the WPDU which must mean we don't have enough data so we can break out of the loop
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "WPDU Parse failure. Reason: " + ex.Message);
                            break;
                        }
                    }
                }
            }
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
                lock (m_HighLevelDataBuffer)
                {
                    while (m_HighLevelDataBuffer.Count > 0)
                    {
                        // The first byte should be the APDU tag
                        xDLMSAPDU NewAPDU = xDLMSAPDU.Create((xDLMSTags)m_HighLevelDataBuffer[0]);
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

                                m_LastFrameCounterReceived = NewCipheredAPDU.FrameCounter;
                            }

                            // We found a valid APDU tag so we can try to parse it. We already checked the first byte so just use the rest
                            MemoryStream DataStream = new MemoryStream(m_HighLevelDataBuffer.ToArray());

                            NewAPDU.Parse(DataStream);

                            // Remove the data that has been parsed
                            m_HighLevelDataBuffer.RemoveRange(0, (int)DataStream.Position);

                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "APDU Received. Type: " + EnumDescriptionRetriever.RetrieveDescription(NewAPDU.Tag));

                            if (NewCipheredAPDU != null)
                            {
                                xDLMSAPDU UncipheredAPDU = null;

                                if (PendingDecryptAuthenticationKey != null)
                                {
                                    // We are in the middle of a key exchange so we could get a message using either the pending key or
                                    // the previous key. We need to attempt the Pending key first
                                    try
                                    {
                                        UncipheredAPDU = NewCipheredAPDU.UncipheredAPDU;
                                    }
                                    catch (Exception)
                                    {
                                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Unciphering failed using the Pending Authentication Key. Using previous key.");
                                    }

                                    if (UncipheredAPDU == null)
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
                            m_HighLevelDataBuffer.RemoveAt(0);
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
        /// Sends the data to the socket
        /// </summary>
        /// <param name="data">The data to send</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        private void Send(byte[] data)
        {
            if (IsOpen)
            {
                m_Socket.Send(data);

                m_Logger.WriteProtocol(Logger.ProtocolDirection.Send, data);
            }
            else
            {
                throw new InvalidOperationException("Data may only be sent while the socket is open");
            }
        }

        /// <summary>
        /// Receives the data from the socket
        /// </summary>
        /// <returns>The data read from the meter</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        private byte[] Receive()
        {
            if (IsOpen)
            {
                byte[] ReceivedData = new byte[m_Socket.Available];

                int BytesRead = m_Socket.Receive(ReceivedData);

                m_Logger.WriteProtocol(Logger.ProtocolDirection.Receive, ReceivedData);

                return ReceivedData;
            }
            else
            {
                throw new InvalidOperationException("Data may only be received while the socket is open");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the TCP socket is open
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool IsOpen
        {
            get
            {
                bool Open = false;

                if (m_Socket != null)
                {
                    Open = m_Socket.Connected;
                }

                return Open;
            }
        }

        /// <summary>
        /// Gets the Host Name for the connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public string HostName
        {
            get
            {
                return m_HostName;
            }
        }

        /// <summary>
        /// Gets the port number for the connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public int Port
        {
            get
            {
                return m_Port;
            }
        }

        /// <summary>
        /// Gets the number of bytes that are currently available to be read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public int DataAvailable
        {
            get
            {
                int BytesAvailable = 0;

                if (IsOpen)
                {
                    BytesAvailable = m_Socket.Available;
                }

                return BytesAvailable;
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
                return ushort.MaxValue;
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
                return m_AuthenticationKey;
            }
            set
            {
                m_AuthenticationKey = value;
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
                return m_PendingAuthenticationKey;
            }
            set
            {
                m_PendingAuthenticationKey = value;
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

        #endregion

        #region Member Variables

        private Socket m_Socket;
        private string m_HostName;
        private int m_Port;
        private Logger m_Logger;
        private Timer m_CheckDataTimer;
        private bool m_DataReceivedEventInProgress;
        private ushort m_ClientPort;
        private ushort m_ServerPort;
        private List<byte> m_LowLevelDataBuffer;
        private List<byte> m_HighLevelDataBuffer;
        private EventHandler m_DataReceivedEventHandler;

        private byte[] m_GlobalEncryptionKey;
        private byte[] m_DedicatedEncryptionKey;
        private byte[] m_AuthenticationKey;
        private byte[] m_PendingAuthenticationKey;
        private byte[] m_ServerApTitle;
        private uint m_LastFrameCounterReceived;

        #endregion
    }
}
