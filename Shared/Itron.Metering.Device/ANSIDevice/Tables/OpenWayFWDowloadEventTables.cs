///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                           Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;


namespace Itron.Metering.Device
{
    /// <summary>
    /// FW Download Log Event
    /// </summary>
    public class FWDownloadLogEvent  : IEquatable<FWDownloadLogEvent>
    {
        #region Definitions

        /// <summary>
        /// The event number that identifies a particular firmware Download log event.
        /// </summary>
        public enum FWDownloadLogEventID : ushort
        {
            /// <summary>
            /// Register Firmware Activated
            /// </summary>
            [EnumDescription("RegisterFirmwareActivated")]
            RegisterFirmwareActivated = 2560,
            /// <summary>
            /// RFLAN Firmware Activated
            /// </summary>
            [EnumDescription("CommFirmwareActivated")]
            CommFirmwareActivated = 2561,
            /// <summary>
            /// Third Party Firmware Activated
            /// </summary>
            [EnumDescription("ThirdPartyFirmwareActivated")]
            ThirdPartyFirmwareActivated = 2562,
            /// <summary>
            /// Auto Seal Meter
            /// </summary>
            [EnumDescription("AutoSealMeter")]
            AutoSealMeter = 2769,
            /// <summary>
            /// Seal Meter
            /// </summary>
            [EnumDescription("SealMeter")]
            SealMeter = 2771,
            /// <summary>
            /// Unseal Meter
            /// </summary>
            [EnumDescription("UnsealMeter")]
            UnsealMeter = 2772,
        }

                
        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the Firmware Download event object based on the event ID
        /// </summary>
        /// <param name="eventID">The event ID for the event to create</param>
        /// <returns>The new Upstream HAN event object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public static FWDownloadLogEvent Create(ushort eventID)
        {
            FWDownloadLogEvent NewEvent = null;

            NewEvent = new FWDownloadLogEvent(eventID);
           

            return NewEvent;
        }

        /// <summary>
        /// Determines if the Firmware download events are equal
        /// </summary>
        /// <param name="other">The Upstream event to compare to</param>
        /// <returns>True if the event numbers are the same. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public bool Equals(FWDownloadLogEvent other)
        {
            bool bEqual = false;

            if (other != null)
            {
                bEqual = m_EventID == other.m_EventID;
            }

            return bEqual;
        }

        /// <summary>
        /// Translates the argument data to a readable string
        /// </summary>
        /// <returns>The argument data as a readable string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created
        //  08/18/11 MMD                Added code to read Argument and retrieve description
        //  08/19/11 AF  2.52.05        Removed "Source Type: " from the string returned

        public virtual string TranslateArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            string strArgumentData = "";

            try
            {

                CENTRON_AMI.FWDLEventSourceType Attribute = (CENTRON_AMI.FWDLEventSourceType)ArgumentReader.ReadUInt32();

                strArgumentData = EnumDescriptionRetriever.RetrieveDescription(Attribute);


            }
            catch (Exception)
            {
                // If we can't read the data for some reason just go with what we have.
            }

            //Closing ArgumentReader also closes ArgumentStream
            ArgumentReader.Dispose();

            return strArgumentData;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Description of the event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public string Description
        {
            get
            {
                return m_strDescription;
            }
        }

        /// <summary>
        /// Gets the Event Number of the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public ushort EventID
        {
            get
            {
                return m_EventID;
            }
            set
            {
                m_EventID = value;
                DetermineDescription();
            }
        }

        /// <summary>
        /// Gets the Date and Time the event occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public DateTime TimeOccurred
        {
            get
            {
                return m_TimeOccurred;
            }
            set
            {
                m_TimeOccurred = value;
            }
        }

        /// <summary>
        /// Gets the event number in the log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public ushort EventNumber
        {
            get
            {
                return m_EventNumber;
            }
            set
            {
                m_EventNumber = value;
            }
        }

        /// <summary>
        /// Gets the sequence number in the log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public ushort SequenceNumber
        {
            get
            {
                return m_SequenceNumber;
            }
            set
            {
                m_SequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets the User ID of the user that caused the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public ushort UserID
        {
            get
            {
                return m_UserID;
            }
            internal set
            {
                m_UserID = value;
            }
        }

        /// <summary>
        /// Gets the Image CRC for the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public uint ImageCRC
        {
            get
            {
                return m_ImageCRC;
            }
            set
            {
                m_ImageCRC = value;
            }
        }

        /// <summary>
        /// Gets the Image Hash
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public byte[] ImageHash
        {
            get
            {
                return (byte[])m_ImageHash.Clone();
            }
            set
            {
                m_ImageHash = value;
            }
        }

        /// <summary>
        /// Gets the Image Version Current
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created
        //  08/16/11 AF  2.52.04         Corrected the set typo

        public byte[] ImageVersionCurrent
        {
            get
            {
                return (byte[])m_ImageVersionCurrent.Clone();
            }
            set
            {
                m_ImageVersionCurrent = value;
            }
        }

        /// <summary>
        /// Gets the Image Version Previous
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public byte[] ImageVersionPrevious
        {
            get
            {
                return (byte[])m_ImageVersionPrevious.Clone();
            }
            set
            {
                m_ImageVersionPrevious = value;
            }
        }

        /// <summary>
        /// Gets the event arguments
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public byte[] Argument
        {
            get
            {
                return (byte[])m_Argument.Clone();
            }
            set
            {
                m_Argument = value;
                ParseArgumentData();
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventID">The event number</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public FWDownloadLogEvent(ushort eventID)
        {
            m_EventID = eventID;
            DetermineDescription();
            m_TimeOccurred = DateTime.MinValue;
            m_EventNumber = 0;
            m_SequenceNumber = 0;
            m_UserID = 0;
            m_ImageCRC = 0;
            m_ImageHash = null;
            m_ImageVersionCurrent = null;
            m_ImageVersionPrevious = null;
            m_Argument = null;
        }

        /// <summary>
        /// Gets or sets the time format to use
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        internal PSEMBinaryReader.TM_FORMAT TimeFormat
        {
            get
            {
                return m_TimeFormat;
            }
            set
            {
                m_TimeFormat = value;
            }
        }

        /// <summary>
        /// Gets the size of an individual FW Download event log entry
        /// </summary>
        /// <param name="table2379">The Table 2379 object for the current device</param>
        /// <param name="ltimeSize">The size of an LTIME data type</param>
        /// <returns>The size of the entry in bytes.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        internal static uint GetEntrySize(OpenWayMFGTable2379 table2379, uint ltimeSize)
        {
            uint EntrySize = 0;

            if (table2379 != null)
            {
                if (table2379.IsLoggingDateAndTime)
                {
                    EntrySize += ltimeSize;
                }

                if (table2379.IsLoggingEventNumber)
                {
                    EntrySize += 2;
                }

                if (table2379.IsLoggingSequenceNumber)
                {
                    EntrySize += 2;
                }

                EntrySize += 46;

                EntrySize += table2379.FWDownloadArgumentDataLength;
            }

            return EntrySize;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the argument data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        protected virtual void ParseArgumentData()
        {
            // This method will be overridden by classes that inherit from this class to parse
            // specific argument data
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the description for the event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        private void DetermineDescription()
        {
            CENTRON_AMI_FWDownloadEventDictionary EventDictionary = new CENTRON_AMI_FWDownloadEventDictionary();

            if (EventDictionary.ContainsKey(m_EventID))
            {
                m_strDescription = EventDictionary[m_EventID];
            }
            else
            {
                m_strDescription = "Unknown Event " + m_EventID.ToString(CultureInfo.CurrentCulture);
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// Time Format
        /// </summary>
        protected PSEMBinaryReader.TM_FORMAT m_TimeFormat;
        /// <summary>
        /// Event ID
        /// </summary>
        protected ushort m_EventID;
        /// <summary>
        /// Description
        /// </summary>
        protected string m_strDescription;
        /// <summary>
        /// Time the event occurred
        /// </summary>
        protected DateTime m_TimeOccurred;
        /// <summary>
        /// The event number
        /// </summary>
        protected ushort m_EventNumber;
        /// <summary>
        /// The sequence number
        /// </summary>
        protected ushort m_SequenceNumber;
        /// <summary>
        /// The user ID
        /// </summary>
        protected ushort m_UserID;
        /// <summary>
        /// The Image CRC
        /// </summary>
        protected uint m_ImageCRC;
        /// <summary>
        /// The Image Hash
        /// </summary>
        protected byte[] m_ImageHash;
        /// <summary>
        /// The Image Version Current
        /// </summary>
        protected byte[] m_ImageVersionCurrent;
        /// <summary>
        /// The Image Version Previous
        /// </summary>
        protected byte[] m_ImageVersionPrevious;
        /// <summary>
        /// The event arguments
        /// </summary>
        protected byte[] m_Argument;

        #endregion

    }

    /// <summary>
    /// MFG Table 331 (2379) - Actual FW Download Log Table
    /// </summary>
    public class OpenWayMFGTable2379 : AnsiTable
    {
        #region Constants
       
        private const uint TABLE_SIZE = 5;

        #endregion

        #region Definitions

        [Flags]
        private enum FWDownloadLogFlags : byte
        {
            LogEventNumber = 0x01,
            LogDateAndTime = 0x02,
            LogSequenceNumber = 0x04,
            LogInhibitOverflow = 0x08,
           
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public OpenWayMFGTable2379(CPSEM psem)
            : base(psem, 2379, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public OpenWayMFGTable2379(PSEMBinaryReader binaryReader)
            : base(2379, TABLE_SIZE)
        {
            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2379.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the FW download Event log is logging the event number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public bool IsLoggingEventNumber
        {
            get
            {
                ReadUnloadedTable();

                return (m_FWDownloadLogFlags & FWDownloadLogFlags.LogEventNumber) == FWDownloadLogFlags.LogEventNumber;
            }
        }

        /// <summary>
        /// Gets whether or not the FW download Event is logging the date and time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public bool IsLoggingDateAndTime
        {
            get
            {
                ReadUnloadedTable();

                return (m_FWDownloadLogFlags & FWDownloadLogFlags.LogDateAndTime) == FWDownloadLogFlags.LogDateAndTime;
            }
        }

        /// <summary>
        /// Gets whether or not the FW download Event is logging the sequence number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public bool IsLoggingSequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return (m_FWDownloadLogFlags & FWDownloadLogFlags.LogSequenceNumber) == FWDownloadLogFlags.LogSequenceNumber;
            }
        }

        /// <summary>
        /// Gets whether or not the FW download log Inhibits Overflow
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public bool IsFWDownloadLogInhibitingOverflow
        {
            get
            {
                ReadUnloadedTable();

                return (m_FWDownloadLogFlags & FWDownloadLogFlags.LogInhibitOverflow) == FWDownloadLogFlags.LogInhibitOverflow;
            }
        }

        
    

        /// <summary>
        /// Gets the argument data length of an FW download Event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public byte FWDownloadArgumentDataLength 
        {
            get
            {
                ReadUnloadedTable();

                return m_byFWDownloadArgumentDataLength;
            }
        }

        /// <summary>
        /// Gets the count of FW download events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/15 jrf 4.20.05 583950  Created
        public ushort FWDownloadEntryCount
        {
            get
            {
                ReadUnloadedTable();

                return m_uiFWDownloadEnteriesNumber;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        private void ParseData()
        {
            m_FWDownloadLogFlags = (FWDownloadLogFlags)m_Reader.ReadByte();
            m_byFWDownloadEventNumbers = m_Reader.ReadByte();
            m_byFWDownloadArgumentDataLength = m_Reader.ReadByte();
            m_uiFWDownloadEnteriesNumber = m_Reader.ReadUInt16();

        }

        #endregion

        #region Member Variables

        private FWDownloadLogFlags m_FWDownloadLogFlags;
        private byte m_byFWDownloadArgumentDataLength;
        private byte m_byFWDownloadEventNumbers;
        private ushort m_uiFWDownloadEnteriesNumber;
  

        #endregion
    }

    /// <summary>
    /// MFG Table 334 (2382) - FW Download Log Table
    /// </summary>
    public class OpenWayMFGTable2382 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2379">The Table 2379 object for the current meter</param>
        /// <param name="table0">That Table 0 object for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public OpenWayMFGTable2382(CPSEM psem, OpenWayMFGTable2379 table2379, CTable00 table0)
            : base(psem, 2382, GetTableSize(table2379, table0), TABLE_TIMEOUT)
        {
            m_Table2379 = table2379;
            m_Table0 = table0;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        ///  <param name="table2379">The Table 2379 object for the current meter</param>
        /// <param name="table0">That Table 0 object for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public OpenWayMFGTable2382(PSEMBinaryReader binaryReader, OpenWayMFGTable2379 table2379, CTable00 table0)
            : base(2379, GetTableSize(table2379, table0))
        {
            m_Table2379 = table2379;
            m_Table0 = table0;

            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2382.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of valid log entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created
        //  08/17/11 AF  2.52.04         Changed this to an offset read for better performance
        //                               when clearing the log

        public ushort NumberOfValidEntries
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;

                PSEMResult = m_PSEM.OffsetRead(2382, 1, 2, out byaData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Number of Firmware Download Events"));
                }
                else
                {
                    //Convert the bytes read to something useful.
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_NumValidEntries = Reader.ReadUInt16();
                }

                return m_NumValidEntries;
            }
        }

        /// <summary>
        /// Gets the index of the last event entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public ushort LastEntryIndex
        {
            get
            {
                ReadUnloadedTable();

                return m_LastEntryIndex;
            }
        }

        /// <summary>
        /// Gets the list of events 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        public ReadOnlyCollection<FWDownloadLogEvent> Events
        {
            get
            {
                ReadUnloadedTable();

                return m_Events.AsReadOnly();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="table2379">The Actual FW Download Log Table for the current meter.</param>
        /// <param name="table0">That Table 0 object for the current meter</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created
        //  05/07/15 jrf 4.20.05 583950  Updated to retrieve the FWDL event log entry count from 
        //                               the Actual FW Download Log Table (2379)
        private static uint GetTableSize(OpenWayMFGTable2379 table2379, CTable00 table0)
        {
            uint TableSize = 11;

            if (table2379 != null)
            {
                TableSize += table2379.FWDownloadEntryCount * FWDownloadLogEvent.GetEntrySize(table2379, table0.LTIMESize);
            }

            return TableSize;
        }

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 MMD                 Created

        private void ParseData()
        {
            m_LogFlags = m_Reader.ReadByte();
            m_NumValidEntries = m_Reader.ReadUInt16();
            m_LastEntryIndex = m_Reader.ReadUInt16();
            m_LastEntrySequenceNumber = m_Reader.ReadUInt32();
            m_NumUnreadEntries = m_Reader.ReadUInt16();

            m_Events = new List<FWDownloadLogEvent>();

            for (int Entry = 0; Entry < m_NumValidEntries; Entry++)
            {
                DateTime TimeOccurred = DateTime.MinValue;
                ushort EventNumber = 0;
                ushort SequenceNumber = 0;
                ushort UserID;
                ushort EventID;
                uint ImageCRC;
                byte[] ImageHash;
                byte[] ImageVersionCurrent;
                byte[] ImageVersionPrevious;
                byte[] ArgumentData;

                FWDownloadLogEvent CurrentEntry = null;

                if (m_Table2379.IsLoggingDateAndTime)
                {
                    TimeOccurred = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
                }

                if (m_Table2379.IsLoggingEventNumber)
                {
                    EventNumber = m_Reader.ReadUInt16();
                }

                if (m_Table2379.IsLoggingSequenceNumber)
                {
                    SequenceNumber = m_Reader.ReadUInt16();
                }

                UserID = m_Reader.ReadUInt16();
                EventID = m_Reader.ReadUInt16();
            
                ImageVersionCurrent = m_Reader.ReadBytes(3);
                ImageVersionPrevious = m_Reader.ReadBytes(3);
                ImageCRC = m_Reader.ReadUInt32();
                ImageHash = m_Reader.ReadBytes(32);
                ArgumentData = m_Reader.ReadBytes(m_Table2379.FWDownloadArgumentDataLength);

                CurrentEntry = FWDownloadLogEvent.Create(EventID);

                CurrentEntry.TimeFormat = (PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat;
                CurrentEntry.TimeOccurred = TimeOccurred;
                CurrentEntry.EventNumber = EventNumber;
                CurrentEntry.SequenceNumber = SequenceNumber;
                CurrentEntry.UserID = UserID;
                CurrentEntry.ImageCRC = ImageCRC;
                CurrentEntry.ImageHash = ImageHash;
                CurrentEntry.ImageVersionCurrent = ImageVersionCurrent;
                CurrentEntry.ImageVersionPrevious = ImageVersionPrevious;
                CurrentEntry.Argument = ArgumentData;

                m_Events.Add(CurrentEntry);
            }
        }

        #endregion

        #region Member Variables

        private OpenWayMFGTable2379 m_Table2379;
        private CTable00 m_Table0;

        private byte m_LogFlags;
        private ushort m_NumValidEntries;
        private ushort m_LastEntryIndex;
        private uint m_LastEntrySequenceNumber;
        private ushort m_NumUnreadEntries;
        
        private List<FWDownloadLogEvent> m_Events;

        #endregion
    }

    /// <summary>
    /// MFG Table 335 (2383) - FW Download CRC Table
    /// </summary>
    public class OpenWayMFGTable2383 : AnsiTable
    {
        #region Constants

        private const int Table2383Length = 8;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM Communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/10/11 AF  2.52.01        Created
        //
        public OpenWayMFGTable2383(CPSEM psem)
            : base(psem, 2383, Table2383Length, 300)
        {
        }

        /// <summary>
        /// Constructor used by EDL file.
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/11/11 jrf 2.52.02        Created
        //
        public OpenWayMFGTable2383(PSEMBinaryReader binaryReader)
            : base(2383, Table2383Length)
        {
            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the table from the meter
        /// </summary>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/10/11 AF  2.52.01        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2383.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the application CRC
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/10/11 AF  2.52.01        Created
        //
        public UInt32 ApplicationCRC
        {
            get
            {
                ReadUnloadedTable();

                return m_ApplicationCRC;
            }
        }

        /// <summary>
        /// Gets the register bootloader CRC
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/10/11 AF  2.52.01        Created
        //
        public UInt32 RegisterBootLoaderCRC
        {
            get
            {
                ReadUnloadedTable();

                return m_RegisterBootLoaderCRC;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/11/11 jrf                 Created

        private void ParseData()
        {
            m_RegisterBootLoaderCRC = m_Reader.ReadUInt32();
            m_ApplicationCRC = m_Reader.ReadUInt32();
        }

        #endregion

        #region Members

        private UInt32 m_ApplicationCRC;
        private UInt32 m_RegisterBootLoaderCRC;

        #endregion

    }
}
