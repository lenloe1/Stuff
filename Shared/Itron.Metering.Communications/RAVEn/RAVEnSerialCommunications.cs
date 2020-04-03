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
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Resources;
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications.RAVEn
{
    /// <summary>
    /// Communication object for the RAVEn ZigBee Module
    /// </summary>
    public class RAVEnSerialCommunications : IDisposable
    {
        #region Constants

        private static readonly string RESOURCE_FILE_PROJECT_STRINGS = "Itron.Metering.Communications.CommStrings";
        private static readonly string INITIALIZE_STRING = ">>>>>";

        #endregion

        #region Public Events

        /// <summary>
        /// Event raised when data has been received
        /// </summary>
        public event EventHandler DataReceived;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEnSerialCommunications()
        {
            m_DataReceivedHandler = new SerialDataReceivedEventHandler(DataReceivedEventHandler);
            m_Port = new SerialPort();
            m_PortMutex = new Mutex();
            m_ReceivedElements = new List<XElement>();

            m_Logger = Logger.TheInstance;
            m_ResourceManager = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, GetType().Assembly);
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        ~RAVEnSerialCommunications()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

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

            if (m_PortMutex != null)
            {
                m_PortMutex.Dispose();
                m_PortMutex = null;
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
        //  05/25/11 RCG 2.51.00        Created

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
            }
            else
            {
                throw new ObjectDisposedException("RAVEnSerialCommunications");
            }
        }

        /// <summary>
        /// Closes the port.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void ClosePort()
        {
            if (m_Port != null)
            {
                m_DataReceivedHandler -= m_DataReceivedHandler;

                if (m_Port.IsOpen)
                {
                    m_Port.Close();
                }
            }
            else
            {
                throw new ObjectDisposedException("RAVEnSerialCommunications");
            }
        }

        /// <summary>
        /// Initializes the session by clearing out any prior unfinished commands
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void Initialize()
        {
            if (m_Port != null)
            {
                if (IsOpen == false)
                {
                    throw new InvalidOperationException(m_ResourceManager.GetString("PORT_NOT_OPEN", CultureInfo.CurrentCulture));
                }

                m_PortMutex.WaitOne();

                m_Port.Write(INITIALIZE_STRING);
                m_Logger.WriteProtocol(Logger.ProtocolDirection.Send, INITIALIZE_STRING);

                m_PortMutex.ReleaseMutex();
            }
            else
            {
                throw new ObjectDisposedException("RAVEnSerialCommunications");
            }
        }

        /// <summary>
        /// Sends the XML fragment to the meter.
        /// </summary>
        /// <param name="xmlElement">The XML fragment to send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void SendFragment(XElement xmlElement)
        {
            string strFragment;

            if (m_Port != null)
            {
                if (IsOpen == false)
                {
                    throw new InvalidOperationException(m_ResourceManager.GetString("PORT_NOT_OPEN", CultureInfo.CurrentCulture));
                }

                if (xmlElement != null)
                {
                    // Block in case we are receiving something
                    m_PortMutex.WaitOne();

                    strFragment = xmlElement.ToString();

                    m_Port.Write(strFragment);
                    m_Logger.WriteProtocol(Logger.ProtocolDirection.Send, xmlElement.ToString(SaveOptions.DisableFormatting));

                    // Release the Mutex
                    m_PortMutex.ReleaseMutex();
                }
            }
            else
            {
                throw new ObjectDisposedException("RAVEnSerialCommunications");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the Port is currently open.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

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

        /// <summary>
        /// Gets the current list of received elements
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public List<XElement> ReceivedElements
        {
            get
            {
                return m_ReceivedElements;
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
        //  05/25/11 RCG 2.51.00        Created

        private void OnDataReceived()
        {
            if (DataReceived != null)
            {
                DataReceived(this, new EventArgs());
            }
        }

        /// <summary>
        /// Sets up the parameters for the Serial Port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void SetupSerialPort()
        {
            m_Port.BaudRate = 115200;
            m_Port.Handshake = Handshake.None;
            m_Port.Parity = Parity.None;
            m_Port.DataBits = 8;
            m_Port.StopBits = StopBits.One;
            m_Port.WriteTimeout = 1000;
            m_Port.ReadTimeout = 1000;
        }

        /// <summary>
        /// Handles the Data Received Event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            ReadCurrentData();
        }

        /// <summary>
        /// Reads whatever is in the buffer and adds any valid elements to the received list.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        private void ReadCurrentData()
        {
            m_PortMutex.WaitOne();

            string strCurrentData = m_Port.ReadExisting();
            m_Logger.WriteProtocol(Logger.ProtocolDirection.Receive, strCurrentData.Replace("\r\n", ""));

            m_UnreadData += strCurrentData;

            m_PortMutex.ReleaseMutex();

            ParseNewElements();
        }

        /// <summary>
        /// Parses any new elements from the data stream
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        private void ParseNewElements()
        {
            MemoryStream TextStream = null;
            XmlReader Reader = null;

            string strEndCommand = "";
            List<XElement> NewElements = new List<XElement>();

            if (m_UnreadData.Contains('<'))
            {
                // Cut off anything at the beginning not in a valid tag
                m_UnreadData = m_UnreadData.Substring(m_UnreadData.IndexOf('<'));

                try
                {
                    TextStream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(m_UnreadData));

                    XmlReaderSettings ReaderSettings = new XmlReaderSettings();
                    ReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

                    Reader = XmlReader.Create(TextStream, ReaderSettings);

                    while (Reader.EOF == false)
                    {
                        XElement NewElement = null;

                        try
                        {
                            Reader.MoveToContent();
                            NewElement = XElement.ReadFrom(Reader) as XElement;
                        }
                        catch (Exception)
                        {
                            // If what we read is invalid we should save it for later.
                        }

                        if (NewElement != null)
                        {
                            NewElements.Add(NewElement);
                        }
                        else
                        {
                            // This means that there is probably an incomplete fragment that we need to wait
                            // for more data so we should move on.
                            break;
                        }
                    }
                }
                finally
                {
                    if (null != Reader)
                    {
                        //Closing Reader also closes TextStream
                        Reader.Close();
                    }
                    else if (null != TextStream)
                    {
                        TextStream.Close();
                    }
                }

                if (NewElements.Count > 0)
                {
                    strEndCommand = "</" + NewElements[NewElements.Count - 1].Name.LocalName + ">";
                    m_UnreadData = m_UnreadData.Substring(m_UnreadData.LastIndexOf(strEndCommand, StringComparison.Ordinal) + strEndCommand.Length);

                    m_ReceivedElements.AddRange(NewElements);
                    OnDataReceived();
                }
            }
        }

        #endregion

        #region Member Variables

        private SerialPort m_Port;
        private List<XElement> m_ReceivedElements;
        private Mutex m_PortMutex;
        private Logger m_Logger;
        private ResourceManager m_ResourceManager;
        private SerialDataReceivedEventHandler m_DataReceivedHandler;
        private string m_UnreadData;

        #endregion

    }
}
