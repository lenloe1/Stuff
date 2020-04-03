#region Copyright Itron, Inc.
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
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// General purpose log class.  Can be used for logging debug info, errors,
    /// and protocol streams.
    /// </summary>
    public class Logger
    {
        #region Constants

        private const byte MAX_PROTOCOL_BYTES_PER_LINE = 16;
        private const string RCVD_MARKER = "R: ";
        private const string SENT_MARKER = "S: ";
        private const string PROTOCOL_MULTI_LINE_PADDING = "   ";

        #endregion

        #region Definitions

        /// <summary>
        /// LoggingLevels are used to initialize the Logger with the maximum 
        /// level that should be included in generated log files.  They are 
        /// also implicitly or explicitly associated with every message written
        /// to the log file.
        /// </summary>
        /// <remarks>
        /// The higher the LoggingLevel, the more information will be included
        /// in the log file.  Lower levels filter out details.
        /// </remarks>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public enum LoggingLevel : byte
        {
            /// <summary>
            /// No data will be logged.  No log file will be saved.  This is
            /// the default or unitialized logger level.  It doesn't make sense
            /// to initialize a logger with this level or write to it with
            /// this level.
            /// </summary>
            NoLogging = 0,
            /// <summary>
            /// Minimal log data.  No data will be added to the device objects
            /// to support FCS Reconfiguration with this logging level.
            /// </summary>
            Minimal = 1,
            /// <summary>
            /// High level functional messages.  Typically device driver
            /// messages like “Updating DST”.  This level of logging includes 
            /// all Minimal log level messages.
            /// </summary>
            Functional = 2,
            /// <summary>
            /// Errors and details of functions.  This level of logging 
            /// includes all Functional log level messages.
            /// </summary>
            Detailed = 3,
            /// <summary>
            /// Full protocol stream. This level of logging includes all 
            /// Detailed log level messages.
            /// </summary>
            Protocol = 4,
            /// <summary>
            /// All ZigBee communcation with the radio. This level of logging
            /// includes all Protocol log level messages. This level of 
            /// logging includes all ZigBee/C177 profile wrapping and all
            /// primatives sent back and forth to the radio.  This level makes
            /// Protocol level look concise.
            /// </summary>
            ZigBeeProtocol = 5
        }


        /// <summary>
        /// Enumerates protocol message marking for easier interpretation.
        /// </summary>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public enum ProtocolDirection : byte
        {
            /// <summary>
            /// Sent data will be prepended with a "(S) " to ease 
            /// interpretation.
            /// </summary>
            Send = 0,
            /// <summary>
            /// Sent data will be prepended with a "(R) " to ease 
            /// interpretation.
            /// </summary>
            Receive = 1
        }


        /// <summary>
        /// Enumerates the two possible actions to be performed on closing.
        /// </summary>
        /// <remarks>
        /// The logger should always be closed by the managing client.  If the
        /// client dies a horrible death, the logger's destructor will attempt
        /// to close and write the file as marches towards that final sunset.
        /// </remarks>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public enum CloseAction : byte
        {
            /// <summary>
            /// All data will be flushed, file will be saved and closed.
            /// </summary>
            SaveFile,
            /// <summary>
            /// File will be closed and deleted.
            /// </summary>
            DeleteFile
        }

        /// <summary>
        /// Enumeration for the states of the logger.
        /// 
        /// NOTE: Pausing the logger does not suspend the logger's timestamps.
        /// </summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/13/06 mrj 7.35.00        Created
        /// 
        /// </remarks>
        public enum LoggingState
        {
            /// <summary>
            /// The logger will write all text to the file, based on the
            /// LoggingLevel.
            /// </summary>
            RUNNING = 0,
            /// <summary>
            /// The logger will not write text with the ProtocolDirection set to
            /// Send.
            /// </summary>
            PROTOCOL_SENDS_SUSPENDED = 1,
            /// <summary>
            /// The logger will not write anything to the log.
            /// </summary>
            LOGGER_PAUSED = 2,
        }

        /// <summary>
        /// The mode to use for logging the time
        /// </summary>
        public enum TimeMode
        {
            /// <summary>
            /// Use the amount of time since logging was started
            /// </summary>
            TimeElapsed = 0,
            /// <summary>
            /// Use the PC's current time
            /// </summary>
            CurrentTime = 1,
            /// <summary>
            /// Do not write a timestamp
            /// </summary>
            NoTimeStamp = 2,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the logger for use.  This method must be called before
        /// writing to the log.
        /// </summary>
        /// <param name="LogLevel">Sets the level of detail to be stored in the log file</param>
        /// <param name="FileName">The name of the log file to be created</param>
        /// <param name="mode">The time mode of the log entries</param>
        /// <returns>
        /// true if successful.  A return value of false probably means that
        /// that the file couldn't be created for some reason.  Check to see
        /// if a read-only copy already exists.
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteLine(LoggingLevel.Functional, "Text to log");
        /// 	</code>
        /// </example>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue#    Description
        /// -------- --- ------- ------    ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	   Created
        /// 08/30/06 mrj 7.35.00           Changed the file to be open with the
        ///                                    file sharing on.
        /// 07/28/16 MP  4.70.10 WR617210  Removed overload constructor and changed time mode to optional parameter
        ///                                    with default of "CurrentTime"
        public bool Initialize(LoggingLevel LogLevel, string FileName, TimeMode mode = TimeMode.CurrentTime)
        {
            bool Successful = false;

            try
            {
                m_StartTime = Environment.TickCount;
                m_TimeMode = mode;

                if (!m_Initialized)
                {
                    if ((LogLevel > LoggingLevel.NoLogging) &&
                        (0 < FileName.Length))
                    {
                        //mrj 08/30/06, changed file to be opened so that other
                        //objects (TIMRunner) can write to the file while it is
                        //opened.
                        m_FileStream = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        m_StreamWriter = new StreamWriter(m_FileStream);


                        if (null != m_StreamWriter)
                        {
                            m_StreamWriter.AutoFlush = true;
                            m_LogLevel = LogLevel;
                            m_FileName = FileName;
                            m_Initialized = true;
                        }
                    }
                    else
                    {
                        m_LogLevel = LoggingLevel.NoLogging;
                        m_Initialized = true;
                    }
                }

                //Since the logger is being initialized, make sure the logger
                //is not paused
                LoggerState = LoggingState.RUNNING;

                Successful = m_Initialized;
            }
            catch (Exception e)
            {
                WriteException(this, e);
            }

            return Successful;

        } // Initialize

        /// <summary>
        /// Closes and saves or deletes the file.  This method should be called
        /// when you're done logging.
        /// </summary>
        /// <remarks>
        /// The file is created during Initialize.  This method will delete the 
        /// file if requested.  If this is not called the file will be saved by
        /// default.
        /// </remarks>
        /// <param name="Action">Save or Delete the file upon closing</param>
        /// <returns>
        /// true if successful
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 09/13/06 mrj 7.35.00        Added support for pausing the logger
        /// 
        public bool Close(CloseAction Action)
        {
            bool Succeeded = false;

            try
            {
                m_Initialized = false;
                LoggerState = LoggingState.RUNNING;

                if (m_StreamWriter != null)
                {
                    m_StreamWriter.Close();
                }

                if (CloseAction.DeleteFile == Action)
                {
                    File.Delete(m_FileName);
                }
                if ((CloseAction.SaveFile == Action) && File.Exists(m_FileName))
                {
                    Succeeded = true;
                }
            }
            catch (Exception e)
            {
                if (LoggingLevel.Detailed <= m_EchoLevel)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return Succeeded;
        } // Close


        /// <summary>
        /// Generic function that writes a line exactly as given to the log 
        /// file.
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// 
        /// Where appropriate, the WriteDetail, WriteException, and 
        /// WriteProtocol methods should be used instead of this method in to
        /// standardize the format of those messages. 
        /// </remarks>
        /// <param name="Level">Logging Detail level</param>
        /// <param name="Line">The message to write</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteLine(LoggingLevel.Functional, "Text to log");
        /// 	</code>
        /// </example>
        /// <overloads>
        /// WriteLine(LoggingLevel Level)
        /// </overloads>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public void WriteLine(LoggingLevel Level, string Line)
        {
            lock (TheInstance)
            {
                Write(Level, Line);
            }
        }


        /// <summary>
        /// Writes a blank line with the specified LoggingLevel
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// </remarks>
        /// <param name="Level">The Detail level of the message</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteLine(LoggingLevel.Functional);
        /// 	</code>
        /// </example>
        /// <overloads>
        /// WriteLine(LoggingLevel Level, string Line)
        /// </overloads>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public void WriteLine(LoggingLevel Level)
        {
            lock (TheInstance)
            {
                Write(Level, "");
            }
        }


        /// <summary>
        /// Writes a message with LoggingLevel = Detailed to the log file.
        /// Writes class and method name information from the caller and
        /// inserts it into the message.
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// </remarks>
        /// <param name="Caller">The object that called the method</param>
        /// <param name="Line">The message to write to the log</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteDetail(this, "Text to log");
        /// 	</code>
        /// </example>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public void WriteDetail(Object Caller, string Line)
        {
            lock (TheInstance)
            {
                try
                {
                    Line = Caller.GetType().ToString() + ": " + Line;

                    Write(LoggingLevel.Detailed, Line);
                }
                catch (Exception e)
                {
                    // Try with whatever made it into the Line so far
                    try
                    {
                        Write(LoggingLevel.Detailed, Line);
                        Write(LoggingLevel.Detailed,
                              "Logger.WriteDetail Exception: " + e.Message);
                    }
                    catch
                    {
                        // Give up.  The logger's not essential.  It can fail.
                    }
                }
            }
        }


        /// <summary>
        /// Writes a message with LoggingLevel = Detailed to the log file.
        /// Writes class and method name information from the caller and
        /// and exception details to the log file.
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// </remarks>
        /// <param name="Caller">The object that called the metho</param>
        /// <param name="e">The exception to log</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteException(this, e);
        /// 	</code>
        /// </example>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public void WriteException(Object Caller, Exception e)
        {
            string Line = "";

            lock (TheInstance)
            {
                try
                {
                    Line = Caller.GetType().ToString() + ": " + e.Message;
                    Write(LoggingLevel.Detailed, Line);
                }
                catch (Exception eLocal)
                {
                    // Try with whatever made it into the Line so far
                    try
                    {
                        Write(LoggingLevel.Detailed, Line);
                        Write(LoggingLevel.Detailed,
                            "Logger.WriteException Exception: " + eLocal.Message);
                    }
                    catch
                    {
                        // Give up.  The logger's not essential.  It can fail.
                    }
                }
            }
        }


        /// <summary>
        /// Writes all of the given data to the log file.  Data is formatted
        /// for easier reading.  Protocol messages are only written if the 
        /// logger's logging level is set at Protocol
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// </remarks>
        /// <param name="Direction">The direction of the data to log</param>
        /// <param name="Data">The data to log</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteProtocol(ProtocolDirection.Send, Data);
        /// 	</code>
        /// </example>
        /// <overloads>
        /// WriteProtocol(ProtocolDirection Direction, byte[] Data, int MaxBytes) 
        /// </overloads>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 09/13/06 mrj 7.35.00        Added support for pausing the logger
        /// 
        public void WriteProtocol(ProtocolDirection Direction, byte[] Data)
        {
            StringBuilder Line = new StringBuilder();

            if (LoggingState.PROTOCOL_SENDS_SUSPENDED == LoggerState &&
                ProtocolDirection.Send == Direction)
            {
                //Do not log the send message
            }
            else
            {
                lock (TheInstance)
                {
                    try
                    {
                        // Implicitly tests for initialization
                        if (LoggingLevel.Protocol <= m_LogLevel)
                        {
                            if (ProtocolDirection.Send == Direction)
                            {
                                Line.Append(SENT_MARKER);
                            }
                            else
                            {
                                Line.Append(RCVD_MARKER);
                            }

                            for (int i = 0; i < Data.Length; i++)
                            {
                                Line.Append(Data[i].ToString("X2", CultureInfo.InvariantCulture));
                                Line.Append(" ");

                                if (0 == (i + 1) % MAX_PROTOCOL_BYTES_PER_LINE)
                                {
                                    Write(LoggingLevel.Protocol, Line.ToString());

                                    Line = new StringBuilder();
                                    Line.Append(PROTOCOL_MULTI_LINE_PADDING);
                                }
                            }

                            if (Line.Length > 5)
                            {
                                Write(LoggingLevel.Protocol, Line.ToString());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Try with whatever made it into the Line so far
                        try
                        {
                            Write(LoggingLevel.Protocol, Line.ToString());
                            Write(LoggingLevel.Protocol,
                                  "Logger.WriteProtocol1 Exception: " + e.Message);
                        }
                        catch
                        {
                            // Give up.  The logger's not essential.  It can fail.
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Writes up to MaxBytes of the given data to the log file.  Data is
        /// formatted for easier reading.  Protocol messages are only written
        /// if the logger's logging level is set at Protocol.
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// </remarks>
        /// <param name="Direction">The Direction of the data to log</param>
        /// <param name="Data">The data to log</param>
        /// <param name="MaxBytes">The maximum number of bytes to log</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteProtocol(ProtocolDirection.Send, Data, 20);
        /// 	</code>
        /// </example>
        /// <overloads>
        /// WriteProtocol(ProtocolDirection Direction, byte[] Data) 
        /// </overloads>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public void WriteProtocol(ProtocolDirection Direction, byte[] Data, int MaxBytes)
        {
            StringBuilder Line = new StringBuilder();

            if (LoggingState.PROTOCOL_SENDS_SUSPENDED == LoggerState &&
                ProtocolDirection.Send == Direction)
            {
                //Do not log the send message
            }
            else
            {
                lock (TheInstance)
                {
                    try
                    {
                        // Implicitly tests for initialization
                        if (LoggingLevel.Protocol <= m_LogLevel)
                        {
                            if (ProtocolDirection.Send == Direction)
                            {
                                Line.Append(SENT_MARKER);
                            }
                            else
                            {
                                Line.Append(RCVD_MARKER);
                            }

                            for (int i = 0; (i < Data.Length) && (i < MaxBytes); i++)
                            {
                                Line.Append(Data[i].ToString("X2", CultureInfo.InvariantCulture));
                                Line.Append(" ");

                                if (0 == (i + 1) % MAX_PROTOCOL_BYTES_PER_LINE)
                                {
                                    Write(LoggingLevel.Protocol, Line.ToString());

                                    Line = new StringBuilder();
                                    Line.Append(PROTOCOL_MULTI_LINE_PADDING);
                                }
                            }

                            if (Line.Length > 5)
                            {
                                Write(LoggingLevel.Protocol, Line.ToString());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Try with whatever made it into the Line so far
                        try
                        {
                            Write(LoggingLevel.Protocol, Line.ToString());
                            Write(LoggingLevel.Protocol,
                                "Logger.WriteProtocol2 Exception: " + e.Message);
                        }
                        catch
                        {
                            // Give up.  The logger's not essential.  It can fail.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes a string as the protocol data
        /// </summary>
        /// <param name="Direction">The direction the data is being sent</param>
        /// <param name="strValue">The string to write</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/11 RCG 2.50.43        Created

        public void WriteProtocol(ProtocolDirection Direction, string strValue)
        {
            StringBuilder Line = new StringBuilder();

            if (LoggingState.PROTOCOL_SENDS_SUSPENDED == LoggerState &&
                ProtocolDirection.Send == Direction)
            {
                //Do not log the send message
            }
            else
            {
                lock (TheInstance)
                {
                    try
                    {
                        // Implicitly tests for initialization
                        if (LoggingLevel.Protocol <= m_LogLevel)
                        {
                            if (ProtocolDirection.Send == Direction)
                            {
                                Line.Append(SENT_MARKER);
                            }
                            else
                            {
                                Line.Append(RCVD_MARKER);
                            }

                            Line.Append(strValue);

                            Write(LoggingLevel.Protocol, Line.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        // Try with whatever made it into the Line so far
                        try
                        {
                            Write(LoggingLevel.Protocol, Line.ToString());
                            Write(LoggingLevel.Protocol,
                                "Logger.WriteProtocol2 Exception: " + e.Message);
                        }
                        catch
                        {
                            // Give up.  The logger's not essential.  It can fail.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes all of the given data to the log file.  Data is formatted
        /// for easier reading.  Protocol messages are only written if the 
        /// logger's logging level is set at ZigBeeProtocol
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// </remarks>
        /// <param name="Direction">The direction of the data to log</param>
        /// <param name="Data">The data to log</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteZigBeeProtocol(ProtocolDirection.Send, Data);
        /// 	</code>
        /// </example>
        /// <overloads>
        /// WriteZigBeeProtocol(ProtocolDirection Direction, 
        ///                     byte[] Data, int MaxBytes) 
        /// </overloads>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/24/08 mcm 1.00.x  Extra level for ZigBee details 
        /// 
        /// 
        public void WriteZigBeeProtocol(ProtocolDirection Direction, byte[] Data)
        {
            StringBuilder Line = new StringBuilder();

            if (LoggingState.PROTOCOL_SENDS_SUSPENDED == LoggerState &&
                ProtocolDirection.Send == Direction)
            {
                //Do not log the send message
            }
            else
            {
                lock (TheInstance)
                {
                    try
                    {
                        // Implicitly tests for initialization
                        if (LoggingLevel.ZigBeeProtocol <= m_LogLevel)
                        {
                            if (ProtocolDirection.Send == Direction)
                            {
                                Line.Append(SENT_MARKER);
                            }
                            else
                            {
                                Line.Append(RCVD_MARKER);
                            }

                            for (int i = 0; i < Data.Length; i++)
                            {
                                Line.Append(Data[i].ToString("X2", CultureInfo.InvariantCulture));
                                Line.Append(" ");

                                if (0 == (i + 1) % MAX_PROTOCOL_BYTES_PER_LINE)
                                {
                                    Write(LoggingLevel.ZigBeeProtocol, Line.ToString());

                                    Line = new StringBuilder();
                                    Line.Append(PROTOCOL_MULTI_LINE_PADDING);
                                }
                            }

                            if (Line.Length > 5)
                            {
                                Write(LoggingLevel.ZigBeeProtocol, Line.ToString());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Try with whatever made it into the Line so far
                        try
                        {
                            Write(LoggingLevel.ZigBeeProtocol, Line.ToString());
                            Write(LoggingLevel.ZigBeeProtocol,
                                  "Logger.WriteProtocol1 Exception: " + e.Message);
                        }
                        catch
                        {
                            // Give up.  The logger's not essential.  It can fail.
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Writes up to MaxBytes of the given data to the log file.  Data is
        /// formatted for easier reading.  Protocol messages are only written
        /// if the logger's logging level is set at ZigBeeProtocol.
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// </remarks>
        /// <param name="Direction">The direction of the data to log</param>
        /// <param name="Data">The data to log</param>
        /// <param name="MaxBytes">The maximum number of bytes to log</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteZigBeeProtocol(ProtocolDirection.Send, Data, 20);
        /// 	</code>
        /// </example>
        /// <overloads>
        /// WriteZigBeeProtocol(ProtocolDirection Direction, byte[] Data) 
        /// </overloads>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public void WriteZigBeeProtocol(ProtocolDirection Direction, 
            byte[] Data, int MaxBytes)
        {
            StringBuilder Line = new StringBuilder();

            if (LoggingState.PROTOCOL_SENDS_SUSPENDED == LoggerState &&
                ProtocolDirection.Send == Direction)
            {
                //Do not log the send message
            }
            else
            {
                lock (TheInstance)
                {
                    try
                    {
                        // Implicitly tests for initialization
                        if (LoggingLevel.ZigBeeProtocol <= m_LogLevel)
                        {
                            if (ProtocolDirection.Send == Direction)
                            {
                                Line.Append(SENT_MARKER);
                            }
                            else
                            {
                                Line.Append(RCVD_MARKER);
                            }

                            for (int i = 0; (i < Data.Length) && (i < MaxBytes); i++)
                            {
                                Line.Append(Data[i].ToString("X2", CultureInfo.InvariantCulture));
                                Line.Append(" ");

                                if (0 == (i + 1) % MAX_PROTOCOL_BYTES_PER_LINE)
                                {
                                    Write(LoggingLevel.ZigBeeProtocol, Line.ToString());

                                    Line = new StringBuilder();
                                    Line.Append(PROTOCOL_MULTI_LINE_PADDING);
                                }
                            }

                            if (Line.Length > 5)
                            {
                                Write(LoggingLevel.ZigBeeProtocol, Line.ToString());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Try with whatever made it into the Line so far
                        try
                        {
                            Write(LoggingLevel.ZigBeeProtocol, Line.ToString());
                            Write(LoggingLevel.ZigBeeProtocol,
                                "Logger.WriteProtocol2 Exception: " + e.Message);
                        }
                        catch
                        {
                            // Give up.  The logger's not essential.  It can fail.
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets the message level to echo to the console. The default is 
        /// NoLogging.
        /// </summary>
        /// <remarks>
        /// You probably don't want to echo Protocol messages.   
        /// Echoing significantly affects performance.
        /// </remarks>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        public LoggingLevel EchoLevel
        {
            set
            {
                m_EchoLevel = value;
            }
        }

        /// <summary>
        /// Returns the current file name
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/29/06 mrj 7.35.00 N/A	Created
        /// 
        public string FileName
        {
            get
            {
                return m_FileName;
            }
        }

        /// <summary>
        /// Returns whether or not the logger has been iniailized.  This can be
        /// used to determine if the logger is currently logging information or
        /// not.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/29/06 mrj 7.35.00 N/A	Created
        ///
        public bool Initialized
        {
            get
            {
                return m_Initialized;
            }
        }

        /// <summary>
        /// Property used to set the state of the logger.
        /// 
        /// NOTE: You cannot set the logger to PROTOCOL_SENDS_SUSPENDED if it
        /// is currently LOGGER_PAUSED.  Need to set to RUNNING prior to the
        /// change.
        /// 
        /// Also, pausing the logger does not suspend the logger's time.
        /// </summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/13/06 mrj 7.35.00 N/A	Created
        /// 
        /// </remarks>
        public LoggingState LoggerState
        {
            get
            {
                return m_LoggingState;
            }
            set
            {
                if (LoggingState.LOGGER_PAUSED == m_LoggingState &&
                    LoggingState.PROTOCOL_SENDS_SUSPENDED == value)
                {
                    //Do not change the state, need to resume logging before
                    //setting to suspend sends
                }
                else
                {
                    m_LoggingState = value;
                }
            }
        }

        /// <summary>
        /// Property used to the FileStream being used by the logger. Can be 
        /// used to access the currently open file contents.
        /// </summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/21/07 mcm         N/A	Created
        /// 
        /// </remarks>
        public FileStream Stream
        {
            get
            {
                return m_FileStream;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This private constructor is implicitly called the first time an 
        /// instance of the logger is assigned.
        /// </summary>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 08/29/06 mrj 7.35.00        Set the start time in the case that the
        ///                             file is initialized with the append
        /// 09/13/06 mrj 7.35.00        Added support for pausing the logger
        /// 
        private Logger()
        {
            m_Initialized = false;
            m_LogLevel = 0;

            m_StartTime = Environment.TickCount;

            m_LoggingState = LoggingState.RUNNING;
        }

        /// <summary>
        /// The destructor will attempt to close the log file, but it should 
        /// not be relied upon to do so.
        /// </summary>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 
        ~Logger()
        {
            if (null != m_StreamWriter)
            {
                try
                {
                    m_StreamWriter.Close();
                }
                catch (Exception e)
                {
                    if (LoggingLevel.Detailed <= m_EchoLevel)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }


        /// <summary>
        /// Returns time elapsed since the logger was initialized.
        /// </summary>
        /// <returns>
        /// Elapsed time formatted as mm:ss:fff, where f is milliseconds
        /// </returns>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/07/06 mcm 7.30.00 N/A	Created
        /// 
        private string GetTimestamp()
        {
            string TimeStamp = "";

            switch (m_TimeMode)
            {
                case TimeMode.TimeElapsed:
                {
                    long Ticks = 10000 * (Environment.TickCount - m_StartTime);
                    DateTime elapsedTime = new DateTime(Ticks);
                    TimeStamp = elapsedTime.ToString("mm:ss:fff ", CultureInfo.InvariantCulture);

                    break;
                }
                case TimeMode.CurrentTime:
                {
                    TimeStamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt ", CultureInfo.CurrentCulture);

                    break;
                }
                case TimeMode.NoTimeStamp:
                    break;
            }

            return TimeStamp;
        }

        /// <summary>
        /// Handles writing for all of the public write methods. Inserts a 
        /// timestamp if the LoggingLevel is Detailed or Protocol.
        /// </summary>
        /// <remarks>
        /// Echos the line to the console if the Echo level is set at or higher
        /// than the specified LoggingLevel
        /// </remarks>
        /// <param name="Level">The logging level</param>
        /// <param name="Line">The message to log</param>
        /// <returns>
        /// none
        /// </returns>
        /// <exception>
        /// This class catches all of its exceptions.  The idea is that logging
        /// is a bonus feature.  If it fails, don't let it prevent other 
        /// processing.
        /// </exception>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 mcm 7.30.00 N/A	Created
        /// 08/30/06 mrj 7.35.00        Changed the file to be open with the
        ///                             file sharing on.
        /// 09/13/06 mrj 7.35.00        Added support for pausing the logger
        /// 09/06/07 mcm 8.10.25        Added lock around write to file
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        private void Write(LoggingLevel Level, string Line)
        {
            if (LoggingState.LOGGER_PAUSED != LoggerState)
            {
                try
                {
                    // Implicitly tests for initialization
                    if ((Level <= m_LogLevel) &&
                        (LoggingLevel.NoLogging != m_LogLevel))
                    {
                        if (LoggingLevel.Detailed <= m_LogLevel)
                        {
                            Line = GetTimestamp() + Line;
                        }

                        if (0 < Line.Length)
                        {
                            //mrj 8/30/06, need to seek the end of the file 
                            //in case other objects (TIMRunner) are writting

                            if (m_StreamWriter.BaseStream != null)
                            {
                                m_StreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                            }

                            m_StreamWriter.WriteLine(Line);

                            if (Level <= m_EchoLevel)
                            {
                                Console.WriteLine(Line);
                            }
                        }
                        else
                        {
                            //mrj 8/30/06, need to seek the end of the file 
                            //in case other objects (TIMRunner) are writting
                            if (m_StreamWriter.BaseStream != null)
                            {
                                m_StreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                            }

                            // Let's not echo blank lines                        
                            m_StreamWriter.WriteLine();
                        }
                    }
                }
                catch (Exception e)
                {
                    // Try with whatever made it into the Line so far
                    try
                    {
                        if ((Level <= m_LogLevel) &&
                            (LoggingLevel.NoLogging != m_LogLevel))
                        {
                            //mrj 8/30/06, need to seek the end of the file
                            //in case other objects (TIMRunner) are writting
                            if (m_StreamWriter.BaseStream != null)
                            {
                                m_StreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                            }

                            m_StreamWriter.WriteLine(Line);
                            Line = "Logger.Write Exception: " + e.ToString();
                            m_StreamWriter.WriteLine(Line);

                            if (Level <= m_EchoLevel)
                            {
                                Console.WriteLine(Line);
                            }
                        }
                    }
                    catch
                    {
                        // Give up.  The logger's not essential.  It can fail.
                    }
                }
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Use this field to initialize a local instance of the logger.  Only
        /// one instance of this object is created.  The manager of the logger
        /// should call the Initialize() method before any other methods are 
        /// called by any users of the logger.
        /// </summary>
        /// <returns>
        /// A reference to the current Logger object
        /// </returns>
        /// <exception>
        /// None
        /// </exception>
        /// <example>
        ///		<code>
        /// 	m_LogFile = Logger.TheInstance;
        /// 	m_LogFile.Initialize(LoggingLevel.Protocol, "MyLogFile.txt");
        /// 	
        /// 	m_LogFile.WriteLine(LoggingLevel.Functional, "Text to log");
        /// 	</code>
        /// </example>
        /// <seealso>
        /// Singleton pattern description
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnpatterns/html/ImpSingletonInCsharp.asp
        /// </seealso>		
        public static readonly Logger TheInstance = new Logger();

        /// <summary>
        /// DLMS decripted logger instance
        /// </summary>
        public static readonly Logger TheDLMSDecryptedLoggerInstance = new Logger();

        private bool m_Initialized;
        private FileStream m_FileStream = null;
        private StreamWriter m_StreamWriter;
        private LoggingLevel m_LogLevel;
        private string m_FileName;
        private long m_StartTime;
        private LoggingLevel m_EchoLevel = LoggingLevel.NoLogging;

        private LoggingState m_LoggingState;
        private TimeMode m_TimeMode;

        #endregion
    }
}
