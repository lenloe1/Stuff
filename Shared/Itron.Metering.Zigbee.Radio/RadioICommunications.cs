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
//                           Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;
using Itron.Metering.Communications;
using Itron.Metering.Utilities;


namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// Represents an abstract Zigbee Radio.  At least 2 radios will eventually
    /// inherit from this class, the Integration Associates Zigbee Dongle, and
    /// the FC200 Zibee Radio.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract partial class Radio : ICommunications
    {
        #region public delegates and events

        /// <summary>
        /// Event raised when communication port input buffer has been read. 
        /// </summary>
        public event CommEventData DataReceived;

        /// <summary>
        /// Event raised when communication port output buffer has sent data.
        /// </summary>
        public event CommEventData DataSent;

        /// <summary>
        /// Event raised when the communication port receive buffer is overrun.
        /// </summary>
        public event CommEvent RxOverrun;

        /// <summary>
        /// Event raised when the communication port character receive flag is 
        /// set.
        /// </summary>
        public event CommEvent FlagCharReceived;

        #endregion

        #region Constants

        /// <summary>
        /// 
        /// </summary>
        protected const int MAX_RX_PACKETS = 10;

        private const int MAX_C1219_PACKET_SIZE = 64;

        private const int WAIT_BETWEEN_PACKETS = 25;

        #endregion Constants

        #region public ICommunication methods

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/08/06 mcm 1.0.0   Initial Release
        ///</remarks>
        public Radio()
        {
            DataReceived += null;
            RxOverrun += null;
            FlagCharReceived += null;
            m_ReceiveDataEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Opens the port passed in as a parameter.
        /// </summary>
        /// <param name="portName">
        /// The communication port to open.
        /// </param>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/11/08 mcm 1.00.00 N/A	Created
        public virtual void OpenPort(string portName) { }

        /// <summary>
        /// Closes the communication port. 
        /// </summary>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// comm.ClosePort();
        /// comm.Dispose();
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/11/08 mcm 1.00.00 N/A	Created
        public virtual void ClosePort()
        {
            Stop();
            Disconnect();
        }


        /// <summary>
        /// Method to send data out of the open port. 
        /// </summary>
        /// <param name="Data">
        /// The data to send over the communication port.
        /// </param>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// byte[] bytData = new byte[5]{0x01, 0x02, 0x03, 0x04, 0x05};
        /// comm.Send(bytData);
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/11/08 mcm 1.00.00 N/A	Created
        public virtual void Send(byte[] Data)
        {
            ZigbeeResult Result;
            byte[] C1219_Packet;
            int BytesToSend = Data.Length;
            bool WaitBetweenPackets;

            // Clear the receive buffer
            m_RxBuffer = null;
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                                "RadioICommunications.Send");
            m_Logger.WriteProtocol(Logger.ProtocolDirection.Send, Data);

            while (BytesToSend > 0)
            {
                if (BytesToSend > MAX_C1219_PACKET_SIZE)
                {
                    C1219_Packet = new byte[MAX_C1219_PACKET_SIZE];
                    Array.Copy(Data, Data.Length - BytesToSend,
                        C1219_Packet, 0, MAX_C1219_PACKET_SIZE);
                    BytesToSend -= MAX_C1219_PACKET_SIZE;
                    WaitBetweenPackets = true;
                }
                else
                {
                    C1219_Packet = new byte[BytesToSend];
                    Array.Copy(Data, Data.Length - BytesToSend,
                        C1219_Packet, 0, BytesToSend);
                    BytesToSend = 0;
                    WaitBetweenPackets = true;
                }

                Result = SendDataRequest(m_TargetShortAddress, C1219_Packet);

                if (WaitBetweenPackets)
                {
					//We have 2 ZigBee packets for every C12.18 packet.  This
					//wait is needed because we would often get packets dropped
					//by the meter's radio.
                    System.Threading.Thread.Sleep(WAIT_BETWEEN_PACKETS);
                }

                if (null != DataSent)
                {
                    DataSent(Data);
                }
            }
        }


        /// <summary>
        /// Method to read data from the communication port into the 
        /// input buffer. 
        /// </summary>
        /// <param name="BytesToRead">
        /// Number of bytes to read. If bytesToRead equals 0, all bytes 
        /// in input buffer are read.
        /// </param>
        /// <param name="iTimeout">
        /// Unused parameter, need for desktop implementation
        /// </param>
        /// <returns>Returns number of bytes read from the communication
        /// port and stored into the input buffer.
        /// </returns>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// if ( 0 != comm.Read(0) )
        /// {
        ///		byte[] inputBuffer = new byte[comm.InputLen];
        ///		Array.Copy(comm.Input, 0, inputBuffer, 0, inputBuffer.Length);
        ///	}
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/15/07 mcm 1.0.x          Support reading electric meters
        //  04/03/08 AF  1.5.15         Use a ManualResetEvent instead of Sleep()
		//	04/07/08 mrj 1.50.00		Re-wrote the method to be more like the 
		//								serial port code.
        // 
        public virtual int Read(uint BytesToRead, int iTimeout)
        {			
            int ReturnedLength = 0;
			
			if (iTimeout == 0)
			{
				//Polling was putting a burden on the CPU so up the timeout and
				//we will wait on received data
				iTimeout = 100;
			}
			
            try
            {
				uint uiTotalBytes = TotalBytesAvailable();
				bool blnDataReceived = false;
											
				if (0 == BytesToRead)
				{
					//If 0 bytes requested then send back all that we have
					BytesToRead = uiTotalBytes;
				}
								
				if (BytesToRead > uiTotalBytes)
				{
					//Wait on the data received event
					m_ReceiveDataEvent.Reset();
					blnDataReceived = m_ReceiveDataEvent.WaitOne(iTimeout, false);
				}
				else
				{
					//We have the amout of data that is needed so move on.
					blnDataReceived = true;
				}

				if (blnDataReceived)
				{
					//Read from the input buffer                
					ReturnedLength = GetInputBuffer(BytesToRead);					
				}				
            }
            catch (Exception e)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                    "RadioICommunications.Read Error");
                m_Logger.WriteException(this, e);
            }

            if (0 < ReturnedLength)
            {
                m_Logger.WriteProtocol(Logger.ProtocolDirection.Receive,
                    m_InputBuffer);
            }
						
            return ReturnedLength;
        } // Read

        #endregion public ICommunication methods

        #region public ICommunication properties

        /// <summary>
        /// Whether or not the communication port is open.
        /// </summary>
        /// <returns>
        /// Boolean indicating whether or not the communication port is open.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// if ( false == comm.IsOpen() )
        /// {
        ///		comm.OpenPort("COM4:");
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/11/08 mcm 1.00.00 N/A	Created
        public abstract bool IsOpen
        {
            get;
        }

        /// <summary>
        /// Property to retrieve the bytes read from the communication port 
        /// input buffer.
        /// </summary>
        /// <returns>Returns a byte[] of the data.</returns>		
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// if ( 0 != comm.Read(0) )
        /// {
        ///		byte[] inputBuffer = new byte[comm.InputLen];
        ///		Array.Copy(comm.Input, 0, inputBuffer, 0, inputBuffer.Length);
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/11/08 mcm 1.00.00 N/A	Created
        public virtual byte[] Input
        {
            // ASSUMES that the caller always calls Read() first.
            get
            {
                return m_InputBuffer;
            }
            set { throw new ZigbeeException("Not supported"); }
        }

        /// <summary>
        /// Property that gets or sets the input buffer length.
        /// </summary>
        /// <returns>Returns the number of bytes in the input buffer.</returns>		
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// if ( 0 != comm.Read(0) )
        /// {
        ///		byte[] inputBuffer = new byte[comm.InputLen];
        ///		Array.Copy(comm.Input, 0, inputBuffer, 0, inputBuffer.Length);
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/11/08 mcm 1.00.00 N/A	Created
        public virtual uint InputLen
        {
            get
            {
                return (uint)m_InputBuffer.Length;
            }
            set { throw new ZigbeeException("Not supported"); }
        }

        /// <summary>
        /// Property that gets or sets the baud rate.  The baud rate can only be
        /// set to a port that is not opened.
        /// </summary>
        /// <returns>
        /// The baud rate (uint).
        /// </returns>
        /// <exception cref="CommPortException">
        /// Thrown if the port is already open.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// if ( false == comm.IsOpen() )
        /// {
        ///		comm.BaudRate = 9600;
        ///		comm.OpenPort("COM4:");
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	03/31/06 mrj 7.30.00 N/A    Created
        /// 
        public virtual uint BaudRate
        {
            get { throw new ZigbeeException("Not supported"); }
            set { throw new ZigbeeException("Not supported"); }
        }

        /// <summary>
        /// Property that gets or sets the Optical Probe Type
        /// </summary>
        /// <returns>
        /// The Optical Probe Type
        /// </returns>        
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	08/17/06 KRC 7.35.00 N/A    Created
        /// 
        public virtual OpticalProbeTypes OpticalProbe
        {
            get { throw new ZigbeeException("Not supported"); }
            set { throw new ZigbeeException("Not supported"); }
        }

        /// <summary>
        /// Property that gets the current port name
        /// </summary>
        /// <returns>
        /// The current port name
        /// </returns>        
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	08/29/06 mrj 7.35.00 N/A    Created
        /// 
        public virtual string PortName
        {
            get { return "ZigBee Radio"; }
        }

        /// <summary>
        /// Gets the Max Supported Packet Size supported by the transport protocol
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/13/12 RCG 2.70.36 N/A    Created

        public ushort MaxSupportedPacketSize
        {
            get
            {
                return MAX_C1219_PACKET_SIZE;
            }
        }

        #endregion public ICommunication properties

        #region Protected Methods

        /// <summary>
        /// Throws the Data Recieved Event
        /// </summary>
        /// <param name="data">The data that was received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/10 RCG 2.45.12        Created

        protected void OnDataRecieved(byte[] data)
        {
            if (DataReceived != null)
            {
                DataReceived(data);
            }
        }

        /// <summary>
        /// Throws the Data Sent event
        /// </summary>
        /// <param name="data">The data that was sent</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/10 RCG 2.45.12        Created

        protected void OnDataSent(byte[] data)
        {
            if (DataSent != null)
            {
                DataSent(data);
            }
        }

        /// <summary>
        /// Throws the RX Overrun event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/10 RCG 2.45.12        Created

        protected void OnRxOverrun()
        {
            if (RxOverrun != null)
            {
                RxOverrun();
            }
        }

        /// <summary>
        /// Throws the Flag Char Received event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/10 RCG 2.45.12        Created

        protected void OnFlagCharRecieved()
        {
            if (FlagCharReceived != null)
            {
                FlagCharReceived();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/24/08 mcm 1.0.x          Support for talking with electric meter
        // 08/19/08 AF  9.50.05        .Net CF 2.0 does not support Array.Resize()
        // 
        protected void AddToRxBuffer(byte[] NewData)
        {
            lock (m_BufferLockObj)
            {
                if (null == m_RxBuffer)
                {
                    m_RxBuffer = NewData; 
                }
                else
                {
                    try
                    {
                        int OldSize = m_RxBuffer.Length;

                        //Array.Resize(ref m_RxBuffer, OldSize + NewData.Length);
                        //Array.Copy(NewData, 0, m_RxBuffer, OldSize, NewData.Length);

                        // Resize the data array
                        byte[] ResizedData = new byte[OldSize + NewData.Length];
                        Array.Copy(m_RxBuffer, 0, ResizedData, 0, m_RxBuffer.Length);
                        Array.Copy(NewData, 0, ResizedData, OldSize, NewData.Length); 
                        m_RxBuffer = ResizedData;

                    }
                    catch (Exception e)
                    {
                        m_Logger.WriteException(this, e);
                    }
                }

                m_ReceiveDataEvent.Set();
            }
        }

        /// <summary>
        /// This method gets data from the input buffer.
        /// </summary>
        /// <param name="Count">Number of bytes to return</param>
        /// <returns>The number of bytes read</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/24/08 mcm 1.0.x   Support for talking with electric meter
        //	04/07/08 mrj 1.50.00		Changed method to return the number of
		//								bytes available if less that requested.
		//								This matches the serial port read method.
        //  07/11/08 AF  1.51.04        Changed parameters for Array.Copy for CE
		//
		protected int GetInputBuffer(uint Count)
        {			
			int iBytesRead = 0;			

            lock (m_BufferLockObj)
            {
                if ((null != m_RxBuffer) && (m_RxBuffer.Length != 0))
                {
                    if ((Count == 0) || (Count == m_RxBuffer.Length))
                    {
                        // They asked for everything we have.                        
						try
						{							
							m_InputBuffer = new byte[m_RxBuffer.Length];
							Array.Copy(m_RxBuffer, m_InputBuffer, m_RxBuffer.Length);
							iBytesRead = m_RxBuffer.Length;
							m_RxBuffer = null;
						}
						catch
						{
						}						
                    }
                    else
                    {	
						//Try to give them the number of bytes that they want
						if (Count > m_RxBuffer.Length)
						{
							//We don't have enough but we will send what we got
							Count = (uint)m_RxBuffer.Length;
						}

                        try
                        {
							m_InputBuffer = new byte[Count];

							Array.Copy(m_RxBuffer, m_InputBuffer, (int)Count);

							byte[] tmp = new byte[m_RxBuffer.Length - Count];
							Array.Copy(m_RxBuffer, (int)Count, tmp, 0, tmp.Length);
							m_RxBuffer = new byte[tmp.Length];
							Array.Copy(tmp, m_RxBuffer, tmp.Length);
							
							iBytesRead = m_InputBuffer.Length;							
                        }
                        catch
                        {
                        }
                    }
                }
            }
			
			return iBytesRead;		
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/10/08 mcm 1.0.x   Support for talking with electric meter
        // 
        protected uint TotalBytesAvailable()
        {
            uint BytesAvailable = 0;

            lock (m_BufferLockObj)
            {
                if (null != m_RxBuffer)
                {
                    BytesAvailable = (uint)m_RxBuffer.Length;
                }
            }

            return BytesAvailable;
        }

        #endregion Protected Methods

        #region Members

        /// <summary>
        /// 
        /// </summary>
        protected byte[] m_RxBuffer;

        /// <summary>
        /// 
        /// </summary>
        protected byte[] m_InputBuffer;

        /// <summary>
        /// 
        /// </summary>
        protected Object m_BufferLockObj = new Object();

        /// <summary>
        /// 
        /// </summary>
        protected ManualResetEvent m_ReceiveDataEvent;

        #endregion Members

    }
}
