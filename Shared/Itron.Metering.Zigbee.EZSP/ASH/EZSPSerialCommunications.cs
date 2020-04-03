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
using System.IO;
using System.IO.Ports;
using System.Resources;
using System.Globalization;
using System.Threading;

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// Serial Communications object for use with EZSP ZigBee dongles
    /// </summary>
    public class EZSPSerialCommunications : IDisposable
    {
        #region Constants

        private static readonly string RESOURCE_FILE_PROJECT_STRINGS = "Itron.Metering.Communications.CommStrings";
        private const int BUFFER_SIZE = 1024;

        private const byte CANCEL_FLAG = 0x1A;
        private const byte FLAG_BYTE = 0x7E;
        private const byte SUBSTITUTE_BYTE = 0x18;

        private const int WATCHDOG_TIME = 200;

        #endregion

        #region Public Events

        /// <summary>
        /// Event raised when data has been received
        /// </summary>
        public event EventHandler DataReceived;
        /// <summary>
        /// Event raised when a Cancel Flag is received
        /// </summary>
        public event EventHandler CancelFlagReceived;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">The EZSP logger</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EZSPSerialCommunications(EZSPLogger logger)
        {
            m_Port = new SerialPort();
            m_ResourceManager = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, GetType().Assembly);
            m_DataReceivedHandler = new SerialDataReceivedEventHandler(DataReceivedEventHandler);
            m_ReadBuffer = new byte[BUFFER_SIZE];
            m_ValidDataStart = 0;
            m_ValidDataLength = 0;
            m_SubstituteFlagOccurred = false;

            m_Logger = logger;
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        ~EZSPSerialCommunications()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void Dispose()
        {
            if (m_Port != null)
            {
                if (m_Port.IsOpen)
                {
                    m_Port.Close();
                }

                m_Port.Dispose();
                m_Port = null;
            }

            if (m_ResourceManager != null)
            {
                m_ResourceManager.ReleaseAllResources();
                m_ResourceManager = null;
            }
        }

        /// <summary>
        /// Opens the specified port
        /// </summary>
        /// <param name="portName">The name of the port to open</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void OpenPort(string portName)
        {
            if (m_Port != null)
            {
                // Make sure the requested port is not already open
                if (IsOpen)
                {
                    throw new InvalidOperationException(m_ResourceManager.GetString("PORT_ALREADY_OPEN", CultureInfo.CurrentCulture));
                }

                m_Port.PortName = portName;
                SetupSerialPort();

                m_Port.Open();

                //Allow the open port response to be received by the buffer and
                //then ReadExisting to remove any of the open port bytes received
                Thread.Sleep(100);

                m_Port.ReadExisting();
                m_Port.DataReceived += m_DataReceivedHandler;

                if (m_Logger != null)
                {
                    m_Logger.WriteLine(EZSPLogLevels.SerialPort, "Opened Port: " + portName);
                }
            }
            else
            {
                throw new ObjectDisposedException("EZSPSerialCommunications");
            }
        }

        /// <summary>
        /// Closes the port.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void ClosePort()
        {
            if (m_Port != null)
            {
                m_DataReceivedHandler -= m_DataReceivedHandler;

                if (m_Port.IsOpen)
                {
                    m_Port.Close();

                    if (m_Logger != null)
                    {
                        m_Logger.WriteLine(EZSPLogLevels.SerialPort, "Closed Port");
                    }
                }
            }
            else
            {
                throw new ObjectDisposedException("EZSPSerialCommunications");
            }
        }

        /// <summary>
        /// Writes the data to the port
        /// </summary>
        /// <param name="data">The data that should be written</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void SendData(byte[] data)
        {
            if (m_Port.IsOpen == false)
            {
                throw new InvalidOperationException(m_ResourceManager.GetString("PORT_NOT_OPEN"));
            }

            if (0 < data.Length)
            {
                lock (m_Port)
                {
                    m_Port.Write(data, 0, data.Length);
                }

                if (m_Logger != null)
                {
                    m_Logger.WriteSerialData(EZSPLogDirection.Send, data);
                }
            }
        }

        /// <summary>
        /// Reads the next frame from the data buffer
        /// </summary>
        /// <returns></returns>
        public byte[] ReadNextFrame()
        {
            byte[] Frame = null;

            if (m_ValidDataLength > 0)
            {
                int FlagByteLocation = FindFlagByte();

                if (FlagByteLocation >= 0)
                {
                    // Determine the length of the data to read
                    int FrameLength = 0;

                    if (FlagByteLocation < m_ValidDataStart)
                    {
                        // This means that the buffer has wrapped
                        FrameLength = m_ReadBuffer.Length - m_ValidDataStart + FlagByteLocation;
                    }
                    else
                    {
                        FrameLength = FlagByteLocation - m_ValidDataStart;
                    }

                    // Read out the frame
                    Frame = new byte[FrameLength];

                    for (int Index = 0; Index < FrameLength; Index++)
                    {
                        int ActualIndex = (m_ValidDataStart + Index) % m_ReadBuffer.Length;

                        Frame[Index] = m_ReadBuffer[ActualIndex];
                    }

                    m_ValidDataStart = (FlagByteLocation + 1) % m_ReadBuffer.Length;
                    m_ValidDataLength -= FrameLength + 1;
                }
            }

            return Frame;
        }

        /// <summary>
        /// Resets the Read Buffer
        /// </summary>
        public void ClearBuffer()
        {
            m_ValidDataStart = 0;
            m_ValidDataLength = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the Port is currently open.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public bool IsOpen
        {
            get
            {
                bool bOpen = false;

                if (m_Port != null)
                {
                    bOpen = m_Port.IsOpen;
                }

                return bOpen;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raises the Data Received event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnDataReceived()
        {
            if (DataReceived != null)
            {
                DataReceived(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the Cancel Flag Received event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnCancelFlagReceived()
        {
            if (CancelFlagReceived != null)
            {
                CancelFlagReceived(this, new EventArgs());
            }
        }

        /// <summary>
        /// Sets up the parameters for the Serial Port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void SetupSerialPort()
        {
            m_Port.BaudRate = 115200;
            m_Port.StopBits = StopBits.One;
            m_Port.RtsEnable = true;
            m_Port.Handshake = Handshake.None;
            m_Port.Parity = Parity.None;
            m_Port.DataBits = 8;
            m_Port.WriteTimeout = 500;
            m_Port.ReadTimeout = 500;
        }

        /// <summary>
        /// Handles the Data Received Event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int BytesToRead = m_Port.BytesToRead;

            if (BytesToRead > 0)
            {
                byte[] ReadBytes = new byte[BytesToRead];

                lock (m_Port)
                {
                    // Read the data from the device.
                    m_Port.Read(ReadBytes, 0, BytesToRead);
                }

                if (m_Logger != null)
                {
                    m_Logger.WriteSerialData(EZSPLogDirection.Receive, ReadBytes);
                }

                // Copy the data into the circular buffer
                for (int Index = 0; Index < ReadBytes.Length; Index++)
                {
                    if (m_SubstituteFlagOccurred == false)
                    {
                        if (ReadBytes[Index] == CANCEL_FLAG)
                        {
                            // We've received a cancel flag so we should throw away any data prior to this
                            m_ValidDataLength = 0;
                            OnCancelFlagReceived();
                        }
                        else if (ReadBytes[Index] == SUBSTITUTE_BYTE)
                        {
                            m_ValidDataLength = 0;
                            m_SubstituteFlagOccurred = true;
                        }
                        else
                        {
                            int ByteLocation = (m_ValidDataStart + m_ValidDataLength) % m_ReadBuffer.Length;

                            // Check to make sure our new location doesn't cross the start
                            if (m_ValidDataLength != 0 && ByteLocation == m_ValidDataStart)
                            {
                                // We have run out of buffer space
                                throw new OutOfMemoryException("The EZSP read buffer is out of space");
                            }
                            else
                            {
                                m_ReadBuffer[ByteLocation] = ReadBytes[Index];
                                m_ValidDataLength++;
                            }
                        }
                    }
                    else if (ReadBytes[Index] == FLAG_BYTE)
                    {
                        // The substitute byte indicates that we should clear all current data and ignore data until we receive
                        // the next flag byte. We've received the Flag Byte so resume normal operation for the next byte
                        m_SubstituteFlagOccurred = false;
                    }
                }

                OnDataReceived();
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Finds the location of the Flag Byte in the Valid data
        /// </summary>
        /// <returns>-1 if the Flag Byte was not found. The index of the flag byte otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created        
        
        private int FindFlagByte()
        {
            int FlagByteIndex = -1;

            for (int Index = 0; Index < m_ValidDataLength; Index++)
            {
                int ActualLocation = (m_ValidDataStart + Index) % m_ReadBuffer.Length;

                if (m_ReadBuffer[ActualLocation] == FLAG_BYTE)
                {
                    FlagByteIndex = ActualLocation;
                    break;
                }
            }

            return FlagByteIndex;
        }

        #endregion

        #region Member Variables

        private SerialPort m_Port;
        private ResourceManager m_ResourceManager;
        private SerialDataReceivedEventHandler m_DataReceivedHandler;
        private byte[] m_ReadBuffer;
        private int m_ValidDataStart;
        private int m_ValidDataLength;
        private bool m_SubstituteFlagOccurred;

        private EZSPLogger m_Logger;

        #endregion
    }
}
