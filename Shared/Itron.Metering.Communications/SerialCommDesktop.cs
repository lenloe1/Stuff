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
//                              Copyright © 2005 - 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Resources;
using System.IO.Ports;
using System.IO;
using Itron.Metering.Utilities;
using System.Globalization;

namespace Itron.Metering.Communications
{
    #region "SerialCommDesktop class"

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
    public class SerialCommDesktop : ICommunications
    {
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

        /// Serial port communications object
        private SerialPort m_SerialPort;

        private ManualResetEvent m_ReadEvent = null;
        private uint m_uiBytesToRead;

        //Port members:
        private string m_strPortName = "";

        private OpticalProbeTypes m_OpticalProbe;

        private const int DEFAULT_BUFFER_SIZE = 4098;
        private const int INITIAL_BAUDRATE = 9600;
        private bool m_blnRTSStatus = false;
        private bool m_blnDTRStatus = false;        

        //Buffer members:
        private uint m_uintRxBufSz = DEFAULT_BUFFER_SIZE;
        private byte[] m_abytRxBuf = null;
        private uint m_uintRxBufIx = 0;  //index into the rx buffer
        private uint m_uintRxThrsh = 1;
        private uint m_uintTxBufSz = DEFAULT_BUFFER_SIZE;
        private byte[] m_abytTxBuf = null;
        private uint m_uintTxBufIx = 0; //index into the tx buffer
        private Mutex m_mtxRxBufBsy = null;
        private uint m_uintInputLen = 1;

        //Resource string support:
        private System.Resources.ResourceManager m_rmStrings = null;
        private static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                       "Itron.Metering.Communications.CommStrings";

        private static Logger m_Logger;

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
        public SerialCommDesktop()
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                this.GetType().Assembly);
            m_SerialPort = new SerialPort();
            m_mtxRxBufBsy = new Mutex();
            m_ReadEvent = new ManualResetEvent(false);
            m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(this.serialPort_DataReceived);

            m_OpticalProbe = OpticalProbeTypes.GENERIC_1_NO_DTR;

            m_Logger = Logger.TheInstance;

            if ((null == m_rmStrings) || (null == m_mtxRxBufBsy) ||
                 (null == m_ReadEvent))
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
        ~SerialCommDesktop()
        {
            if (m_SerialPort.IsOpen)
            {
                m_SerialPort.Close();
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
            string strDesc;

            try
            {
                //Only support one port at a time.
                if (m_SerialPort.IsOpen)
                {
                    strDesc = string.Format(CultureInfo.CurrentCulture, m_rmStrings.GetString("PORT_ALREADY_OPEN"));
                    throw (new Exception(strDesc));
                }

                m_strPortName = portName;

                m_SerialPort.PortName = m_strPortName;
                //This must be performed after the Init() and before the Open();
                SetupSerialPort();

                // create the transmit and receive buffers
                m_abytRxBuf = new byte[m_uintRxBufSz];
                m_abytTxBuf = new byte[m_uintTxBufSz];

                m_SerialPort.Open();

                //Allow the open port response to be received by the buffer and
                //then ReadExisting to remove any of the open port bytes received
                System.Threading.Thread.Sleep(100);

                m_SerialPort.ReadExisting();

                m_uintRxBufIx = 0;
            }
            catch (Exception e)
            {
                strDesc = string.Format(CultureInfo.CurrentCulture, m_rmStrings.GetString("OPEN_PORT_FAILED"),
                                        e.Message);
                throw (new CommPortException(strDesc, e));
            }
        }

        /// <summary>
        /// ReOpenPort - Used in Factory code, we can can turn control of the port over to the factory.
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="BaudRate"></param>
        public void ReOpenPort(string portName, int BaudRate)
        {
            byte[] abytReadBuf = new Byte[m_uintRxBufSz];
            string strDesc;

            try
            {
                //Only support one port at a time.
                if (m_SerialPort.IsOpen)
                {
                    strDesc = string.Format(CultureInfo.CurrentCulture, m_rmStrings.GetString("PORT_ALREADY_OPEN"));
                    throw (new Exception(strDesc));
                }

                m_strPortName = portName;

                m_SerialPort.PortName = m_strPortName;
                //This must be performed after the Init() and before the Open();
                SetupSerialPort();
                // Since we are reopening a COM Port we are already logged on at a given rate, we do not want to start at 9600
                m_SerialPort.BaudRate = BaudRate;

                // create the transmit and receive buffers
                m_abytRxBuf = new byte[m_uintRxBufSz];
                m_abytTxBuf = new byte[m_uintTxBufSz];

                m_SerialPort.Open();

                //Allow the open port response to be received by the buffer and
                //then ReadExisting to remove any of the open port bytes received
                System.Threading.Thread.Sleep(100);

                m_SerialPort.ReadExisting();

                m_uintRxBufIx = 0;
            }
            catch (Exception e)
            {
                strDesc = string.Format(CultureInfo.CurrentCulture, m_rmStrings.GetString("OPEN_PORT_FAILED"),
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
                return (m_SerialPort.IsOpen);
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
                if (m_SerialPort.IsOpen)
                {
                    m_SerialPort.Close();
                }
            }
            catch (Exception e)
            {
                strDesc = string.Format(CultureInfo.CurrentUICulture, m_rmStrings.GetString("CLOSE_PORT_FAILED"),
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
                if (!m_SerialPort.IsOpen)
                {
                    strErrDesc = m_rmStrings.GetString("PORT_NOT_OPEN");
                    throw (new Exception(strErrDesc));
                }

                if (0 < data.Length)
                {
                    this.Output = data;

                    m_Logger.WriteProtocol(Logger.ProtocolDirection.Send,
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
                strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, e.Message);
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
        /// <param name="iTimeout"></param>
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

                if (!m_SerialPort.IsOpen)
                {
                    strErrDesc = m_rmStrings.GetString("PORT_NOT_OPEN");
                    throw (new Exception(strErrDesc));
                }

                if (0 == bytesToRead)
                {
                    bytesToRead = (uint)m_SerialPort.BytesToRead;
                }


                m_uiBytesToRead = bytesToRead;

                // Do we have the amout of data we are looking for?  If so move on.
                if (bytesToRead > m_SerialPort.BytesToRead)
                {
                    //Wait on the data received event
                    m_ReadEvent.Reset();
                    m_ReadEvent.WaitOne(iTimeout, false);
                }

                //Read from the input buffer                
                intBytesRead = m_SerialPort.Read(abytReadBuf, 0, (int)m_uiBytesToRead);
                m_uiBytesToRead = 0;


                if (0 < intBytesRead)
                {
                    if (intBytesRead >= abytReadBuf.GetUpperBound(0))
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

                        m_Logger.WriteProtocol(Logger.ProtocolDirection.Receive, abytReadBuf, intBytesRead);


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
                    strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, intErr); ;
                    throw (new Exception(strErrDesc));
                }

            }
            catch (TimeoutException)
            {
                intBytesRead = 0;
            }
            catch (Exception e)
            {
                strErrDesc = m_rmStrings.GetString("READ_FAILED");
                strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, e.Message);
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

                    if (!m_SerialPort.IsOpen)
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
                    strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, e.Message);
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
                return (uint)m_SerialPort.BaudRate;
            }
            set
            {
                m_SerialPort.BaudRate = (int)value;
            }
        }

        /// <summary>
        /// Property that gets or sets the Optical Probe Type
        /// </summary>
        /// <returns>
        /// The Optical Probe Type
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// if ( false == comm.IsOpen() )
        /// {
        ///     comm.OpticalProbe = OpticalProbeTypes.SCHLUMBERGER
        ///		comm.BaudRate = 9600;
        ///		comm.OpenPort("COM4:");
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	08/17/06 KRC 7.35.00 N/A    Created
        /// 
        public OpticalProbeTypes OpticalProbe
        {
            get
            {
                return m_OpticalProbe;
            }
            set
            {
                m_OpticalProbe = value;
                switch (m_OpticalProbe)
                {
                    case OpticalProbeTypes.SCHLUMBERGER:
                    case OpticalProbeTypes.SCHLUMBERGER_FRANCE:
                    case OpticalProbeTypes.SCHLUMBERGER_SPAIN:
                    case OpticalProbeTypes.GENERIC_2_DTR:
                        {
                            m_blnDTRStatus = true;
                            m_blnRTSStatus = false;
                            break;
                        }
                    case OpticalProbeTypes.US_MICROTEL_PM_300:
                    case OpticalProbeTypes.US_MICROTEL_PM_500:
                    case OpticalProbeTypes.US_MICROTEL_PM_600:
                    case OpticalProbeTypes.GE_SMARTCOUPLER_SC1:
                    case OpticalProbeTypes.GENERIC_1_NO_DTR:
                        {
                            m_blnDTRStatus = false;
                            m_blnRTSStatus = false;
                            break;
                        }
                }

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
        /// Property that accesses the serial port's RtsEnable line. NOTE that
        /// the class will set the this line low when opening the port. If you 
        /// want it high, call this after opening the port.
        /// </summary>
        /// <returns>
        /// The serial port's RtsEnable state
        /// </returns>        
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	12/10/07 mcm 10.0.1  N/A    Support for belt clip radios
        /// 
        public bool RtsEnable
        {
            get { return m_SerialPort.RtsEnable; }
            set { m_SerialPort.RtsEnable = value; }
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
                return 1024;
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
                string strErrDesc;

                try
                {
                    //Clear the input buffer before we send a command
                    m_SerialPort.DiscardInBuffer();

                    //Send anything already in the buffer
                    if (m_uintTxBufIx > 0)
                    {
                        m_SerialPort.Write(m_abytTxBuf, 0, (int)m_uintTxBufIx);
                        m_uintTxBufIx = 0;
                    }

                    if (true == blnResult)
                    {
                        m_SerialPort.Write(value, 0, value.Length);
                    }

                    if (false == blnResult)
                    {
                        int intErr = Marshal.GetLastWin32Error();
                        strErrDesc = m_rmStrings.GetString("WRITE_FILE_FAILED");
                        strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, intErr); ;
                        throw (new Exception(strErrDesc));
                    }
                }
                catch (Exception e)
                {
                    strErrDesc = m_rmStrings.GetString("OUTPUT_FAILED");
                    strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, e.Message);
                    throw (new CommPortException(strErrDesc, e));
                }
            }
        }

        /// <summary>
        /// Sets up the default settings for the SerialPort object
        /// </summary>        
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/11/06 KRC 7.35.00        Created for new SerialPort
        /// 
        private void SetupSerialPort()
        {
            m_SerialPort.BaudRate = INITIAL_BAUDRATE;
            m_SerialPort.Handshake = Handshake.None;
            m_SerialPort.Parity = Parity.None;
            m_SerialPort.DataBits = 8;
            m_SerialPort.StopBits = StopBits.One;
            m_SerialPort.WriteTimeout = 1000;
            m_SerialPort.ReadTimeout = 500;
            m_SerialPort.RtsEnable = m_blnRTSStatus;
            m_SerialPort.DtrEnable = m_blnDTRStatus;
        }

        /// <summary>
        /// Handler for the DataReceived event
        /// </summary>        
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/11/06 mrj 7.35.00        Created for new SerialPort
        /// 
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (m_SerialPort.BytesToRead >= m_uiBytesToRead)
            {
                //Signal the semaphore so we can start reading since the port
                //has the expected number of bytes
                m_ReadEvent.Set();
            }

            if (null != FlagCharReceived)
            {
                FlagCharReceived();
            }
        }

    }  //end Communication class

    #endregion
}

