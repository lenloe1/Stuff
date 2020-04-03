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
using System.Globalization;

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// The logging layers
    /// </summary>
    [Flags]
    public enum EZSPLogLevels
    {
        /// <summary>
        /// Nothing will be logged
        /// </summary>
        None = 0,
        /// <summary>
        /// The raw serial data will be logged
        /// </summary>
        SerialPort = 1,
        /// <summary>
        /// The ASH Protocol details will be logged
        /// </summary>
        ASHProtocol = 2,
        /// <summary>
        /// The EZSP Protocol details will be logged
        /// </summary>
        EZSPProtocol = 4,
        /// <summary>
        /// The Application Layer details will be logged
        /// </summary>
        ApplicationLayer = 8,
        /// <summary>
        /// Logs all layers of detail
        /// </summary>
        All = SerialPort | ASHProtocol | EZSPProtocol | ApplicationLayer,
    }

    /// <summary>
    /// The data direction
    /// </summary>
    public enum EZSPLogDirection
    {
        /// <summary>
        /// Data was sent
        /// </summary>
        Send,
        /// <summary>
        /// Data was received
        /// </summary>
        Receive,
    }

    /// <summary>
    /// Communications logger for the EZSP protocol
    /// </summary>
    public class EZSPLogger
    {
        #region Constants

        private readonly string DATE_FORMAT_STRING = "MM/dd/yyyy hh:mm:ss.fff tt";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  05/28/14 jrf 4.00.15 WR517731 Removed m_LogFileInfo. It wasn't needed as a member variable.
        public EZSPLogger()
        {
            m_LoggedLevels = EZSPLogLevels.None;
            m_LogFileStream = null;
            m_LogFileWriter = null;
            m_LoggingEnabled = false;
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        ~EZSPLogger()
        {
            if (m_LogFileWriter != null)
            {
                try
                {
                    m_LogFileWriter.Close();
                }
                catch
                {
                    // Don't care if this fails
                }
            }

            if (m_LogFileStream != null)
            {
                try
                {
                    m_LogFileStream.Close();
                }
                catch
                {
                    // Don't care if this fails
                }
            }
        }

        /// <summary>
        /// Converts a byte array into a readable string
        /// </summary>
        /// <param name="data">The data to convert</param>
        /// <returns>The data as a string</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public static string ConvertDataToString(byte[] data)
        {
            string DataString = "";

            if (data != null)
            {
                foreach (byte CurrentByte in data)
                {
                    DataString += CurrentByte.ToString("X2", CultureInfo.CurrentCulture) + " ";
                }
            }

            return DataString;
        }

        /// <summary>
        /// Starts logging to the specified file
        /// </summary>
        /// <param name="logFileName">The path to the log file</param>
        /// <param name="loggedLevels">The levels that will be logged in the file</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  05/28/14 jrf 4.00.15 WR517731 Removing use of FileInfo object. It was the only thing that could have been
        //                               accessing the log file and causing the System.IO.IOException during the call
        //                               to the FileStream's constructor.
        public void StartLogging(string logFileName, EZSPLogLevels loggedLevels)
        {
            m_LoggedLevels = loggedLevels;

            if (Directory.Exists(Path.GetDirectoryName(logFileName)) == false && !string.IsNullOrEmpty(Path.GetDirectoryName(logFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFileName));
            }

            m_LogFileStream = new FileStream(logFileName, FileMode.Create, FileAccess.Write);
            m_LogFileWriter = new StreamWriter(m_LogFileStream);
            m_LogFileWriter.AutoFlush = true;

            m_LoggingEnabled = true;
        }

        /// <summary>
        /// Stops all logging
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  05/28/14 jrf 4.00.15 WR517731 Removed m_LogFileInfo. It wasn't needed as a member variable.
        public void StopLogging()
        {
            if (m_LogFileWriter != null)
            {
                m_LogFileWriter.Close();
                m_LogFileWriter = null;
            }

            if (m_LogFileStream != null)
            {
                m_LogFileStream.Close();
                m_LogFileStream = null;
            }

            m_LoggedLevels = EZSPLogLevels.None;
        }

        /// <summary>
        /// Writes the specified line to the log
        /// </summary>
        /// <param name="logLevel">The log level of the line to write</param>
        /// <param name="logText">The text to write to the log</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void WriteLine(EZSPLogLevels logLevel, string logText)
        {
            string Output = DateTime.Now.ToString(DATE_FORMAT_STRING, CultureInfo.InvariantCulture) + " " + logText;

            if (m_LoggingEnabled && ShouldLog(logLevel))
            {
                try
                {
                    lock (m_LogFileWriter)
                    {
                        m_LogFileWriter.WriteLine(Output);
                    }
                }
                catch
                {
                    // We don't care if this fails.
                }
            }
        }

        /// <summary>
        /// Writes the Serial Port Data to the log
        /// </summary>
        /// <param name="direction">The direction of the data</param>
        /// <param name="data">The data to write</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void WriteSerialData(EZSPLogDirection direction, byte[] data)
        {
            string Output = "";

            if (direction == EZSPLogDirection.Send)
            {
                Output += "S: ";
            }
            else
            {
                Output += "R: ";
            }

            foreach (byte CurrentByte in data)
            {
                Output += CurrentByte.ToString("X2", CultureInfo.InvariantCulture) + " ";
            }

            WriteLine(EZSPLogLevels.SerialPort, Output);
        }

        /// <summary>
        /// Writes basic information for the ASH Layer of the protocol
        /// </summary>
        /// <param name="direction">The direction of the frame</param>
        /// <param name="frame">The frame to write</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void WriteASHFrameInfo(EZSPLogDirection direction, ASHFrame frame)
        {
            string Output = "";

            if (direction == EZSPLogDirection.Send)
            {
                Output += "S: ";
            }
            else
            {
                Output += "R: ";
            }

            Output += "- ASH Frame: " + frame.ASHFrameType.ToString() + " - ";

            switch (frame.ASHFrameType)
            {
                case ASHFrame.FrameType.Data:
                {
                    Output += "Frame Number: " + frame.FrameNumber.ToString() + " Ack Number: " + frame.AckNumber.ToString()
                        + " Retransmit: " + frame.IsRetransmitted.ToString() + " Data: ";

                    for (int Index = 0; Index < frame.Data.Length; Index++)
                    {
                        Output += frame.Data[Index].ToString("X2") + " ";
                    }

                    break;
                }
                case ASHFrame.FrameType.Ack:
                case ASHFrame.FrameType.Nak:
                {
                    Output += "Ack Number: " + frame.AckNumber.ToString() + " Not Ready: " + frame.NotReady.ToString();
                    break;
                }
                case ASHFrame.FrameType.Error:
                {
                    Output += "Error Code: ";

                    for (int Index = 0; Index < frame.Data.Length; Index++)
                    {
                        Output += frame.Data[Index].ToString("X2") + " ";
                    }

                    break;
                }
            }

            Output += " CRC Valid: " + frame.IsCRCValid.ToString();

            WriteLine(EZSPLogLevels.ASHProtocol, Output);
        }

        /// <summary>
        /// Writes the EZSP Frame details
        /// </summary>
        /// <param name="commandFrame">The command frame to write</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void WriteEZSPFrame(EZSPCommandFrame commandFrame)
        {
            string Output = "S: -- EZSP Frame - Frame ID: " + commandFrame.FrameID.ToString() + " Sequence: " + commandFrame.SequenceNumber.ToString(CultureInfo.InvariantCulture) 
                + " Sleep Mode: " + commandFrame.SleepMode.ToString();

            WriteLine(EZSPLogLevels.EZSPProtocol, Output);
        }

        /// <summary>
        /// Writes the EZSP Frame details
        /// </summary>
        /// <param name="responseFrame">The response frame to write</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void WriteEZSPFrame(EZSPResponseFrame responseFrame)
        {
            string Output = "R: -- EZSP Frame - Frame ID " + responseFrame.FrameID.ToString() + " Sequence: " + responseFrame.SequenceNumber.ToString(CultureInfo.InvariantCulture) + " Overflow: " + responseFrame.HasOverflown.ToString(CultureInfo.CurrentCulture)
                + " Truncated: " + responseFrame.ResponseTruncated.ToString(CultureInfo.CurrentCulture) + " Callback Pending: " + responseFrame.CallbackPending.ToString(CultureInfo.CurrentCulture);

            WriteLine(EZSPLogLevels.EZSPProtocol, Output);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether or not the item should be logged
        /// </summary>
        /// <param name="logLevel">The level to check</param>
        /// <returns>True if the level is being logged. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private bool ShouldLog(EZSPLogLevels logLevel)
        {
            return (logLevel & m_LoggedLevels) == logLevel;
        }

        #endregion

        #region Member Variables

        private EZSPLogLevels m_LoggedLevels;
        private FileStream m_LogFileStream;
        private StreamWriter m_LogFileWriter;
        private bool m_LoggingEnabled;

        #endregion
    }
}
