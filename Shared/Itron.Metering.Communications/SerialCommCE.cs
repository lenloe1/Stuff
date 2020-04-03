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
//                              Copyright © 2006 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Resources;
using Itron.Metering;
using Itron.Metering.Utilities;


namespace Itron.Metering.Communications
{
    #region "SerialCommCE class"

    /// <summary>
    /// Class supporting all communication types such as a comm port.  
    /// </summary>
    /// <example>
    /// <code>
    /// Communication comm = new Communication();
    /// </code>
    /// </example>
    /// Revision History
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 08/01/05 bdm 7.13.00 N/A	Created
	/// 02/04/08 mrj				Changed to use Mike's new communication
	///								code.
	public class SerialCommCE : Itron.Metering.Communications.ICommunications
    {
        //Handheld dll
        [DllImport("Q200api.dll",
                    EntryPoint = "comSetPort2PowerOut",
                    SetLastError = true)]
        private static extern bool comSetPort2PowerOut(bool powerOut);

        #region delegates and events

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

        #region variable declarations

        //Port members:
        private string m_strPortName = "";
        private IntPtr m_hPort = (IntPtr)PortAPI.INVALID_HANDLE_VALUE;

        //Port parameters:
        private DCB m_DCB = null;
        private DetailedPortSettings m_portSettings;
        private int m_intDTR = -1;   //DTR state
        private uint m_uiBaudRate = (uint)BaudRates.CBR_9600;

        //Port threading and events:
        private Thread m_rxThread = null;
        private ManualResetEvent m_rxThreadStartedEvent = null;
        private IntPtr m_ptrCloseEvent;
        private string m_strCloseEventName = "CloseEvent";

        //Buffer members:
        private const int DEFAULT_BUFFER_SIZE = 4098;
        private uint m_uintRxBufSz = DEFAULT_BUFFER_SIZE;
        private byte[] m_abytRxBuf = null;
        private uint m_uintRxBufIx = 0;  //index into the rx buffer
        private uint m_uintRxThrsh = 1;
        private uint m_uintTxBufSz = DEFAULT_BUFFER_SIZE;
        private byte[] m_abytTxBuf = null;
        private uint m_uintTxBufIx = 0; //index into the tx buffer
        private uint m_uintTxThrsh = 1;
        private Mutex m_mtxRxBufBsy = null;
        private uint m_uintInputLen = 1;

        //Resource string support:
        private System.Resources.ResourceManager m_rmStrings = null;
        private static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                       "Itron.Metering.Communications.CommStrings";

        private static Logger m_hLogFile;

        #endregion


        /// <summary>
        /// Constructor.  
        /// </summary>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public SerialCommCE()
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                this.GetType().Assembly);
            m_mtxRxBufBsy = new Mutex();
            m_DCB = new DCB();
            m_rxThreadStartedEvent = new ManualResetEvent(false);
            m_hLogFile = Logger.TheInstance;

            if ((null == m_rmStrings) || (null == m_mtxRxBufBsy) ||
                 (null == m_DCB) || (null == m_rxThreadStartedEvent))
            {
                throw (new Exception("Error creating communication object."));
            }
        }

        /// <summary>
        /// Destructor.  
        /// </summary>
        /// <remarks>
        /// The communication class event thread blocks until the port handle 
        /// is closed. A destructor is implemented to make sure 
        /// that clean up is performed as soon as possible.
        ///</remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        ~SerialCommCE()
        {
            if (IsOpen)
            {
                ClosePort();
            }

            m_rmStrings.ReleaseAllResources();
            m_rmStrings = null;

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
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public void OpenPort(string portName)
        {
            byte[] abytReadBuf = new Byte[m_uintRxBufSz];
            int intBytesRead = 0;
            string strDesc;

            try
            {
                //Only support one port at a time.
                if (IsOpen)
                {
                    strDesc = string.Format(m_rmStrings.GetString("PORT_ALREADY_OPEN"));
                    throw (new Exception(strDesc));
                }

                m_strPortName = portName;

                Init();

                //This must be performed after the Init() and before the Open();
                m_portSettings = new HandshakeNone();

                Open();

                //Allow the open port response to be received by the buffer and
                //then ReadFile to remove any of the open port bytes received
                System.Threading.Thread.Sleep(100);

                PortAPI.ReadFile(m_hPort, abytReadBuf,
                                 m_uintRxBufSz, ref intBytesRead);

                m_abytRxBuf.Initialize();
                m_uintRxBufIx = 0;
            }
            catch (Exception e)
            {
                strDesc = string.Format(m_rmStrings.GetString("OPEN_PORT_FAILED"),
                                        e.Message);
                throw (new CommPortException(strDesc, e));
            }
        }



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
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public bool IsOpen
        {
            get
            {
                return ((IntPtr)PortAPI.INVALID_HANDLE_VALUE != m_hPort);
            }
        }


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
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public void ClosePort()
        {
            string strDesc;

            try
            {
                if (IsOpen)
                {
                    Close();
                }
            }
            catch (Exception e)
            {
                strDesc = string.Format(m_rmStrings.GetString("CLOSE_PORT_FAILED"),
                                                              e.Message);
                throw (new CommPortException(strDesc, e));
            }
        }


        /// <summary>
        /// Method to send data out of the open port. 
        /// </summary>
        /// <param name="data">
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
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public void Send(byte[] data)
        {
            string strErrDesc;

            try
            {
                if (!IsOpen)
                {
                    strErrDesc = m_rmStrings.GetString("PORT_NOT_OPEN");
                    throw (new Exception(strErrDesc));
                }

                if (0 < data.Length)
                {
                    this.Output = data;

                    m_hLogFile.WriteProtocol(Logger.ProtocolDirection.Send,
                                              data);

                    //Raise the event to anyone listening
                    if (null != DataSent)
                    {
                        DataSent(data);
                    }
                }
            }
            catch (Exception e)
            {
                strErrDesc = m_rmStrings.GetString("SEND_FAILED");
                strErrDesc = string.Format(strErrDesc, e.Message);
                throw (new CommPortException(strErrDesc, e));
            }
        }


        /// <summary>
        /// Method to read data from the communication port into the 
        /// input buffer. 
        /// </summary>
        /// <param name="bytesToRead">
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
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public int Read(uint bytesToRead, int iTimeout)
        {
            int intBytesRead = -1;
            byte[] abytReadBuf = new Byte[m_uintRxBufSz];
            bool blnResult = true;
            string strErrDesc;

            try
            {

                if (!IsOpen)
                {
                    strErrDesc = m_rmStrings.GetString("PORT_NOT_OPEN");
                    throw (new Exception(strErrDesc));
                }

                if (0 == bytesToRead)
                {
                    bytesToRead = (uint)abytReadBuf.Length;
                }

                blnResult = PortAPI.ReadFile(m_hPort,
                                             abytReadBuf,
                                             bytesToRead,
                                             ref intBytesRead);

                if ((true == blnResult) && (0 < intBytesRead))
                {
                    if (intBytesRead >= m_abytRxBuf.GetUpperBound(0))
                    {
                        //Data beyond the buffer is lost!
                        if (null != RxOverrun)
                            RxOverrun();
                    }
                    else
                    {
                        m_mtxRxBufBsy.WaitOne();
                        for (int iIndex = 0; iIndex < intBytesRead; iIndex++)
                        {
                            m_abytRxBuf[m_uintRxBufIx] = abytReadBuf[iIndex];
                            m_uintRxBufIx++;
                        }

                        m_uintInputLen = m_uintRxBufIx;
                        m_mtxRxBufBsy.ReleaseMutex();

                        m_hLogFile.WriteProtocol(
                                            Logger.ProtocolDirection.Receive, abytReadBuf, intBytesRead);

                        if (m_uintRxBufIx % m_uintRxThrsh == 0)
                        {
                            //Raise the event to anyone listening
                            if (null != DataReceived)
                            {
                                byte[] bytTmp = new byte[intBytesRead];
                                Array.Copy(m_abytRxBuf,
                                           0,
                                           bytTmp,
                                           0,
                                           bytTmp.Length);


                                DataReceived(bytTmp);
                            }
                        }
                    }
                }

                if (false == blnResult)
                {
                    int intErr = Marshal.GetLastWin32Error();
                    strErrDesc = m_rmStrings.GetString("READ_FILE_FAILED");
                    strErrDesc = string.Format(strErrDesc, intErr); ;
                    throw (new Exception(strErrDesc));
                }

            }
            catch (Exception e)
            {
                strErrDesc = m_rmStrings.GetString("READ_FAILED");
                strErrDesc = string.Format(strErrDesc, e.Message);
                throw (new CommPortException(strErrDesc, e));
            }

            return intBytesRead;
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
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public byte[] Input
        {
            get
            {
                //There are known issues regarding setting member variables and
                //making method calls within a property.  At this point, 
                //the existing code does not seem to have adverse effects.

                string strErrDesc;
                byte[] abytData = null;

                try
                {

                    if (!IsOpen)
                    {
                        strErrDesc = m_rmStrings.GetString("PORT_NOT_OPEN");
                        throw (new Exception(strErrDesc));
                    }


                    if (m_uintRxBufIx > 0)
                    {
                        abytData = new byte[m_uintRxBufIx];

                        //Prevent the rx thread from adding to the buffer while
                        //it is being used.
                        m_mtxRxBufBsy.WaitOne();

                        //Copy the buffer to an output variable for 
                        //m_uintInputLen bytes
                        Array.Copy(m_abytRxBuf, 0, abytData, 0, abytData.Length);

                        //Shift the data in the rx buffer to remove
                        //m_uintInputLen bytes
                        Array.Copy(m_abytRxBuf,
                                   abytData.Length,
                                   m_abytRxBuf,
                                   0,
                                   (int)(m_abytRxBuf.GetUpperBound(0) - abytData.Length));

                        m_uintRxBufIx = 0;
                        m_uintInputLen = 0;

                        // release the mutex so the Rx thread can work
                        m_mtxRxBufBsy.ReleaseMutex();
                    }
                }
                catch (Exception e)
                {
                    strErrDesc = m_rmStrings.GetString("INPUT_FAILED");
                    strErrDesc = string.Format(strErrDesc, e.Message);
                    throw (new CommPortException(strErrDesc, e));
                }

                return abytData;
            }
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
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public uint InputLen
        {
            get
            {
                return m_uintInputLen;
            }
            set
            {
                m_uintInputLen = value;
            }
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
        public uint BaudRate
        {
            get
            {
                return m_uiBaudRate;
            }
            set
            {
                if (!IsOpen)
                {
                    //The port is not open so set the new baud rate
                    m_uiBaudRate = value;
                }
                else
                {
                    //Currently we do not allow changing the baud rate on an
                    //open port.  It looks like we could call GetCommState,
                    //change the baud rate, and then call SetCommState.  For
                    //now we will throw an exception if the baud rate is changed.
                    if (m_uiBaudRate != value)
                    {
                        string strDesc = m_rmStrings.GetString("BAUD_RATE_CHANGE_FAILED");
                        throw (new Exception(strDesc));
                    }
                }
            }
        }

        /// <summary>
        /// Property that gets or sets the Optical Probe Type.  Currently not
        /// supported on the handheld.
        /// </summary>
        /// <returns>
        /// The Optical Probe Type
        /// </returns>        
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	08/17/06 mrj 7.35.00 N/A    Created
        /// 
        public OpticalProbeTypes OpticalProbe
        {
            get
            {
                //Need to return an optical probe.  This is needed by CreateHHF
				//or it will fail.
				return OpticalProbeTypes.GENERIC_1_NO_DTR;
            }
            set
            {
                throw (new NotImplementedException());
            }
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
        public string PortName
        {
            get
            {
                return m_strPortName;
            }
        }

        /// <summary>
        /// Property that takes its assigned value and sends its contents to 
        /// the communication port.
        /// </summary>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private byte[] Output
        {
            set
            {

                //There are known issues regarding setting member variables and
                //making method calls within a property.  At this point, 
                //the existing code does not seem to have adverse effects.

                bool blnResult = true;
                int intBytesWritten = 0;
                string strErrDesc;

                try
                {
                    //Clear the input buffer before we send a command
                    blnResult = PortAPI.PurgeComm(m_hPort, PurgeCommFlags.PURGE_RXCLEAR);
                    
                    //Verify number of bytes to send is not more than threshold
                    if (value.GetLength(0) > m_uintTxThrsh)
                    {
                        //Send anything already in the buffer
                        if (m_uintTxBufIx > 0)
                        {
                            blnResult = PortAPI.WriteFile(m_hPort,
                                m_abytTxBuf,
                                m_uintTxBufIx,
                                ref intBytesWritten);
                            m_uintTxBufIx = 0;
                        }

                        if (true == blnResult)
                        {
                            blnResult = PortAPI.WriteFile(m_hPort,
                                value,
                                (uint)value.GetLength(0),
                                ref intBytesWritten);
                        }
                    }

                    if ((true == blnResult) &&
                        (value.GetLength(0) <= m_uintTxThrsh))
                    {
                        //Copy it to the tx buffer
                        value.CopyTo(m_abytTxBuf, (int)m_uintTxBufIx);
                        m_uintTxBufIx += (uint)value.Length;

                        //If the buffer is above m_uintTxThrsh, send it
                        if (m_uintTxBufIx >= m_uintTxThrsh)
                        {
                            blnResult = PortAPI.WriteFile(m_hPort,
                                m_abytTxBuf,
                                m_uintTxBufIx,
                                ref intBytesWritten);
                            m_uintTxBufIx = 0;
                        }
                    }

                    if (false == blnResult)
                    {
                        int intErr = Marshal.GetLastWin32Error();
                        strErrDesc = m_rmStrings.GetString("WRITE_FILE_FAILED");
                        strErrDesc = string.Format(strErrDesc, intErr); ;
                        throw (new Exception(strErrDesc));
                    }
                }
                catch (Exception e)
                {
                    strErrDesc = m_rmStrings.GetString("OUTPUT_FAILED");
                    strErrDesc = string.Format(strErrDesc, e.Message);
                    throw (new CommPortException(strErrDesc, e));
                }
            }
        }


        /// <summary>
        /// Initializes the communication port.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private void Init()
        {
            // create a system event for synchronizing Closing
            m_ptrCloseEvent = PortAPI.CreateEvent(true,
                                                  false,
                                                  m_strCloseEventName);

            // create the transmit and receive buffers
            m_abytRxBuf = new byte[m_uintRxBufSz];
            m_abytTxBuf = new byte[m_uintTxBufSz];

            //Default using detailed port settings;
            m_portSettings = new DetailedPortSettings();
        }


        /// <summary>
        /// Creates the communication port underlying file.
        /// </summary>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private void CreateFile()
        {
            string strErrDesc;

            try
            {
                m_hPort = PortAPI.CreateFile(m_strPortName);

                if (!IsOpen)
                {
                    int intErr = Marshal.GetLastWin32Error();

                    if ((int)APIErrors.ERROR_ACCESS_DENIED == intErr)
                    {
                        strErrDesc = m_rmStrings.GetString("PORT_NOT_AVAILABLE");
                    }
                    else
                    {
                        strErrDesc = m_rmStrings.GetString("CREATE_FILE_FAILED");
                    }

                    strErrDesc = string.Format(strErrDesc, intErr);
                    throw (new Exception(strErrDesc));
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }


        /// <summary>
        /// Sets the communication port buffer sizes
        /// </summary>
        /// <remarks>Assumption made that port is open.</remarks>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private void SetComm()
        {
            int intErr;
            string strErrDesc;

            try
            {
                if (false == PortAPI.SetupComm(m_hPort,
                                               m_uintRxBufSz,
                                               m_uintTxBufSz))
                {
                    intErr = Marshal.GetLastWin32Error();
                    strErrDesc = m_rmStrings.GetString("SET_COMM_FAILED");
                    strErrDesc = string.Format(strErrDesc, intErr);
                    throw (new Exception(strErrDesc));
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }


        /// <summary>
        /// Sets the communication port DCB comm state.
        /// </summary>
        /// <remarks>Assumption made that port is open.</remarks>
        /// <exception cref="System.Exception">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        ///	03/31/06 mrj 7.30.00 N/A    Updated
        ///	
        private void SetCommState()
        {
            string strErrDesc;

            try
            {
                //Transfer the port settings to a DCB structure
                m_DCB.BaudRate = BaudRate;

                m_DCB.ByteSize = m_portSettings.BasicSettings.ByteSize;
                m_DCB.EofChar = (sbyte)m_portSettings.EOFChar;
                m_DCB.ErrorChar = (sbyte)m_portSettings.ErrorChar;
                m_DCB.EvtChar = (sbyte)m_portSettings.EVTChar;
                m_DCB.fAbortOnError = m_portSettings.AbortOnError;
                m_DCB.fBinary = true;
                m_DCB.fDsrSensitivity = m_portSettings.DSRSensitive;
                m_DCB.fDtrControl = (DCB.DtrControlFlags)m_portSettings.DTRControl;
                m_DCB.fErrorChar = m_portSettings.ReplaceErrorChar;
                m_DCB.fInX = m_portSettings.InX;
                m_DCB.fNull = m_portSettings.DiscardNulls;
                m_DCB.fOutX = m_portSettings.OutX;
                m_DCB.fOutxCtsFlow = m_portSettings.OutCTS;
                m_DCB.fOutxDsrFlow = m_portSettings.OutDSR;

                m_DCB.fParity = (m_portSettings.BasicSettings.Parity == Parity.none) ? false : true;
                m_DCB.fRtsControl = (DCB.RtsControlFlags)m_portSettings.RTSControl;
                m_DCB.fTXContinueOnXoff = m_portSettings.TxContinueOnXOff;

                m_DCB.Parity = (byte)m_portSettings.BasicSettings.Parity;
                m_DCB.StopBits = (byte)m_portSettings.BasicSettings.StopBits;

                m_DCB.XoffChar = (sbyte)m_portSettings.XoffChar;
                m_DCB.XonChar = (sbyte)m_portSettings.XonChar;

                m_DCB.XonLim = m_DCB.XoffLim = (ushort)(m_uintRxBufSz / 10);

                if (false == PortAPI.SetCommState(m_hPort, m_DCB))
                {
                    strErrDesc = string.Format("SetCommState failed: {0}",
                        Marshal.GetLastWin32Error());
                    throw (new Exception(strErrDesc));
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }


        /// <summary>
        /// Sets the communication port timeouts.
        /// </summary>
        /// <remarks>Assumption made that port is open.</remarks>
        /// <exception cref="System.Exception">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private void SetCommTimeouts()
        {
            int intErr;
            string strErrDesc;

            try
            {
                //Set the Comm timeouts based on C12.18
                CommTimeouts ct = new CommTimeouts();
                ct.ReadIntervalTimeout = 500;  //Inter-char timeout
                ct.ReadTotalTimeoutMultiplier = 1;
                ct.ReadTotalTimeoutConstant = 1; //miTimeout;
                ct.WriteTotalTimeoutMultiplier = 0;
                ct.WriteTotalTimeoutConstant = 0;

                if (false == PortAPI.SetCommTimeouts(m_hPort, ct))
                {
                    intErr = Marshal.GetLastWin32Error();
                    strErrDesc = m_rmStrings.GetString("SET_COMM_TMOUTS_FAILED");
                    strErrDesc = string.Format(strErrDesc, intErr);
                    throw (new Exception(strErrDesc));
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Starts the communication port receive thread.
        /// </summary>
        /// <remarks>Assumption made that port is open.</remarks>
        /// <exception cref="System.Exception">
        /// Thrown when a thread create and start fails.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private void StartReceiveThread()
        {
            try
            {
                // start the receive thread
                m_rxThread = new Thread(new ThreadStart(CommEventThread));
                m_rxThread.Priority = ThreadPriority.AboveNormal;
                m_rxThread.Start();

                // wait for the thread to actually get spun up
                m_rxThreadStartedEvent.WaitOne();
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Perform all the required calls to create, setup and open the 
        /// communication port.
        /// </summary>
        /// <exception cref="System.Exception">
        /// Thrown when a failure occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private void Open()
        {

            try
            {
                CreateFile();

                //Set the FC200 hand held power out.
                comSetPort2PowerOut(true);

                SetComm();

                SetCommState();

                SetCommTimeouts();

                StartReceiveThread();

                if (m_DCB.fDtrControl == DCB.DtrControlFlags.Enable)
                {
                    m_intDTR = 1;
                }
                else
                {
                    m_intDTR = 0;
                }
                SetDTRState(false);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }


        /// <summary>
        /// Closes the communication port. 
        /// </summary>
        /// <exception cref="System.Exception">
        /// Thrown when a communication port error occurs or a 
        /// system exception occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private void Close()
        {
            int intErr;
            string strErrDesc;

            try
            {
                if (IsOpen)
                {
                    if (PortAPI.CloseHandle(m_hPort))
                    {
                        PortAPI.SetEvent(m_ptrCloseEvent);
                        comSetPort2PowerOut(false);
                    }
                    else
                    {
                        intErr = Marshal.GetLastWin32Error();
                        strErrDesc = m_rmStrings.GetString("CLOSE_FAILED");
                        strErrDesc = string.Format(strErrDesc, intErr);
                        throw (new Exception(strErrDesc));
                    }

                    //Set to invalid handle even if port close failed.
                    m_hPort = (IntPtr)PortAPI.INVALID_HANDLE_VALUE;
                }
            }
            catch (Exception e)
            {
                m_hPort = (IntPtr)PortAPI.INVALID_HANDLE_VALUE;
                throw (e);
            }
        }


        /// <summary>
        /// Sets the communication port DTR state.
        /// </summary>
        /// <remarks>Assumption made that port is open.</remarks>
        /// <exception cref="System.Exception">
        /// Thrown when a communication port error occurs or a 
        /// system exception occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        private void SetDTRState(bool Enable)
        {

            bool blnResult = true;
            string strErrDesc;

            try
            {
                //m_intDTR must be set to an initial value prior to calling this routine. 
                blnResult = (m_intDTR > 0) && (IsOpen);

                if (true == blnResult)
                {
                    if (true == Enable)
                    {
                        //set DTR
                        blnResult = PortAPI.EscapeCommFunction(m_hPort,
                                                               CommEscapes.SETDTR);
                        if (true == blnResult)
                        {
                            m_intDTR = 1;
                        }
                    }
                    else
                    {
                        //clear DTR
                        blnResult = PortAPI.EscapeCommFunction(m_hPort,
                                                               CommEscapes.CLRDTR);
                        if (true == blnResult)
                        {
                            m_intDTR = 0;
                        }
                    }
                }

                if (false == blnResult)
                {
                    strErrDesc = m_rmStrings.GetString("SET_DTR_STATE_FAILED");
                    throw (new Exception(strErrDesc));
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Runs solely on a separate thread waiting on events to come in 
        /// from the communication port (OS) and handles those events.
        /// </summary>
        /// <remarks>Assumption made that port is open.</remarks>
        /// <exception cref="System.Exception">
        /// Thrown when a communication port error or a 
        /// system exception occurs.
        /// </exception>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        /// 08/17/06 mrj 7.35.00        Removed unused events
		/// 03/02/07 mrj 8.00.16		Changed when the FlagCharReceived event
		///								is fired.
		///								
        private void CommEventThread()
        {
            CommEventFlags eventFlags = new CommEventFlags();
            byte[] readbuffer = new Byte[m_uintRxBufSz];
            string strErrDesc;
            int intErr;


            try
            {
                //Specify the set of events to be monitored for the port.
                PortAPI.SetCommMask(m_hPort, CommEventFlags.ALL);

                //Let Open() know the tread has started
                m_rxThreadStartedEvent.Set();

                while (IsOpen)
                {
                    //Wait for a Comm event to happen
                    if (false == PortAPI.WaitCommEvent(m_hPort, ref eventFlags))
                    {
                        //WaitCommEvent failed - find out why
                        intErr = Marshal.GetLastWin32Error();

                        if (intErr == (int)APIErrors.ERROR_IO_PENDING)
                        {
                            //IO pending so just wait and try again.
                            //Suspend thread to allow other threads to execute.
                            Thread.Sleep(0);
                            continue;
                        }

                        if (intErr == (int)APIErrors.ERROR_INVALID_HANDLE)
                        {
                            //Calling Communication.Close() causes m_hPort to 
                            //become invalid.
                            //Since Thread.Abort() is unsupported in the CF,
                            //we must accept that calling Close will throw 
                            //an error here.

                            //Close signals the m_ptrCloseEvent, so wait on it
                            //We wait 1 second, though Close should happen 
                            //much sooner
                            int intEventResult = PortAPI.WaitForSingleObject(m_ptrCloseEvent, 1000);

                            if (intEventResult == (int)APIConstants.WAIT_OBJECT_0)
                            {
                                // the event was set so close was called
                                m_hPort = (IntPtr)PortAPI.INVALID_HANDLE_VALUE;

                                // reset our ResetEvent for the next call 
                                //to Open()
                                m_rxThreadStartedEvent.Reset();

                                return;
                            }
                        }

                        //WaitCommEvent failed!
                        strErrDesc = m_rmStrings.GetString("WAIT_COMM_EVENT_FAILED");
                        strErrDesc = string.Format(strErrDesc, intErr);
                        throw (new Exception(strErrDesc));
                    }

                    //Re-specify the set of events to be monitored for the port.
                    PortAPI.SetCommMask(m_hPort, CommEventFlags.ALL);

                    //Check the event for errors
                    if (((uint)eventFlags & (uint)CommEventFlags.ERR) != 0)
                    {
                        CommErrorFlags errorFlags = new CommErrorFlags();
                        CommStat commStat = new CommStat();

                        //Get the error status
                        if (false == PortAPI.ClearCommError(m_hPort,
                                                             ref errorFlags,
                                                             commStat))
                        {
                            //ClearCommError failed!
                            intErr = Marshal.GetLastWin32Error();
                            strErrDesc = m_rmStrings.GetString("CLEAR_COMM_ERROR_FAILED");
                            strErrDesc = string.Format(strErrDesc, intErr);
                            throw (new Exception(strErrDesc));
                        }

                        if (((uint)errorFlags & (uint)CommErrorFlags.BREAK) != 0)
                        {
                            //BREAK can set an error, so make sure the BREAK bit
                            //is set an continue
                            eventFlags |= CommEventFlags.BREAK;
                        }
                        else
                        {
                            //We have an error.  Build a meaningful string and throw 
                            //an exception
                            StringBuilder strMsg = new StringBuilder("UART Error: ", 80);
                            if ((errorFlags & CommErrorFlags.FRAME) != 0)
                            { strMsg = strMsg.Append("Framing,"); }
                            if ((errorFlags & CommErrorFlags.IOE) != 0)
                            { strMsg = strMsg.Append("IO,"); }
                            if ((errorFlags & CommErrorFlags.OVERRUN) != 0)
                            { strMsg = strMsg.Append("Overrun,"); }
                            if ((errorFlags & CommErrorFlags.RXOVER) != 0)
                            { strMsg = strMsg.Append("Receive Overflow,"); }
                            if ((errorFlags & CommErrorFlags.RXPARITY) != 0)
                            { strMsg = strMsg.Append("Parity,"); }
                            if ((errorFlags & CommErrorFlags.TXFULL) != 0)
                            { strMsg = strMsg.Append("Transmit Overflow,"); }

                            //No known bits are set
                            if (strMsg.Length == 12)
                            { strMsg = strMsg.Append("Unknown"); }

                            //Raise an error event to anyone listening
                            //if (null != OnError)
                                //OnError(strMsg.ToString());

                            continue;
                        }
                    } //End if( ( (uint)eventFlags & (uint)CommEventFlags.ERR ) != 0 )

                    //Check for status changes
                    //08-12-05 bdm: Frankly, not sure for what purpose status = 0.
                    //              But hesitate to modify since the code was borrowed
                    //              and there does not exist a client that catches
                    //              these events.
                    //uint status = 0;

                    //Check for RXFLAG
                    if (((uint)eventFlags & (uint)CommEventFlags.RXFLAG) != 0)
                    {
                        //if (null != FlagCharReceived)
                            //FlagCharReceived();
                    }
                    
                    //Check for RXCHAR
                    if ((eventFlags & CommEventFlags.RXCHAR) != 0)
                    {                        
						//Let the client know that we have received a character
						if (null != FlagCharReceived)
							FlagCharReceived();						
                    }
                } // while(true)
            } // try
            catch (Exception e)
            {
                throw (e);
            }
        }

    }  //end Communication class

    #endregion
}

