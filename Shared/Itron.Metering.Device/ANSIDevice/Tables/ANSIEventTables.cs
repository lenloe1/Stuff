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
//                              Copyright © 2007 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Resources;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.IO;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Standard Table 71
    /// </summary>
    public class StdTable71 : AnsiTable
    {
        #region Constants

        private const int TABLE71_SIZE_VER_10 = 9;
        private const int TABLE71_SIZE_VER_20 = 12;
        private const byte EVENT_NUMBER_FLAG_MASK = 0x01;
        private const byte HIST_DATE_TIME_FLAG_MASK = 0x02;
        private const byte HIST_SEQ_NBR_FLAG_MASK = 0x04;


        private const int LTIME_LENGTH = 5;
        private const int UINT16_LENGTH = 2;
        private const int UINT8_LENGTH = 1;
        private const int IDB_BFLD_LENGTH = 2;
        private const int EC_BFLD_LENGTH = 2;

	    #endregion

	    #region Definitions
	    #endregion

	    #region Public Methods

        /// <summary>
        /// Table 71 - Actual Log Dimensions Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="strRevsion">Standard Revision</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/14/06 KRC 7.35.00 N/A    Created

        public StdTable71(CPSEM psem, string strRevsion)
            : base(psem, 71, GetTableLength(strRevsion))
        {
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="strRevision">The ANSI Standard revison</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/10 RCG 2.40.31 151959 Created

        public StdTable71(PSEMBinaryReader reader, string strRevision)
            : base (71, GetTableLength(strRevision))
        {
            m_Reader = reader;
            ParseData();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Reads table 71 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 KRC 7.35.00 N/A    Created
        ///
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable71.Read");

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

        #region Private Methods

        /// <summary>
        /// Get the data out of the binary reader and into the member variables.
        /// </summary>
        private void ParseData()
        {
            //Populate the member variable that represent the table
            m_byLogFlagBfld = m_Reader.ReadByte();
            m_byNbrStdEvents = m_Reader.ReadByte();
            m_byNbrMfgEvents = m_Reader.ReadByte();
            m_byHistDataLength = m_Reader.ReadByte();
            m_byEventDataLength = m_Reader.ReadByte();
            m_uiNbrHistoryEntries = m_Reader.ReadUInt16();
            m_uiNbrEventEntries = m_Reader.ReadUInt16();
        }

        /// <summary>
        /// Returns the size of Table 71
        /// </summary>
        /// <param name="revision"></param>
        /// <returns></returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/08/07 KRC   
        //
        static private uint GetTableLength(string revision)
        {
            uint uiTableLength = 0;

            if (0 > String.Compare(revision, CTable00.STD_REV_2, StringComparison.Ordinal))
            {
                uiTableLength = TABLE71_SIZE_VER_10;
            }
            else
            {
                uiTableLength = TABLE71_SIZE_VER_20;
            }
            return uiTableLength;
        }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Exposes the History Date Time Flag
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC   7.35			Created
        /// 
        public bool HistoryDateTimeFlag
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                bool bHistDateTimeFlag = false;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading History Log Entries"));
                    }
                }

                

                if (0 != (byte)(m_byLogFlagBfld & HIST_DATE_TIME_FLAG_MASK))
                {
                    bHistDateTimeFlag = true;
                }

                return bHistDateTimeFlag;
            }
        }

        /// <summary>
        /// Exposes the Event Number Flag
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC   7.35			Created
        /// 
        public bool EventNumberFlag
        {
            get
            {
                bool bEventNumberFlag = false;

                if (0 != (byte)(m_byLogFlagBfld & EVENT_NUMBER_FLAG_MASK))
                {
                    bEventNumberFlag = true;
                }
                return bEventNumberFlag;
            }
        }

        /// <summary>
        /// Exposes the History Sequence Number Flag
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC   7.35			Created
        /// 
        public bool HistorySequenceNumberFlag
        {
            get
            {
                bool bHistSeqNbrFlag = false;

                if (0 != (byte)(m_byLogFlagBfld & HIST_SEQ_NBR_FLAG_MASK))
                {
                    bHistSeqNbrFlag = true;
                }
                return bHistSeqNbrFlag;
            }
        }

        /// <summary>
        /// Exposes the History Data Length
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC   7.35			Created
        /// 
        public byte HistoryDataLength
        {
            get
            {
                return m_byHistDataLength;
            }
        }

        /// <summary>
        /// Exposes the Event Data Length
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/11/10 jrf 2.41.01		Created
        /// 
        public byte EventDataLength
        {
            get
            {
                return m_byEventDataLength;
            }
        }

        /// <summary>
        /// Exposes the History Data Length
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC   7.35			Created
        /// 
        public uint NumberHistoryEntries
        {
            get
            {
                return m_uiNbrHistoryEntries;
            }
        }

        /// <summary>
        /// Exposes the Event Data Length
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/11/10 jrf 2.41.01		Created
        /// 
        public uint NumberEventEntries
        {
            get
            {
                return m_uiNbrEventEntries;
            }
        }

        /// <summary>
        /// Property that will return the size of 74 based off the values in 71
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC   7.35			Created
        /// 
        public uint SizeOfTable74
        {
            get
            {
                int iEntryLength = 0;

                // Start size with the constant data: Hist Flags, Nbr valid entries
                //	Last Entry Element, Last Entry Seq Number and # unread entries
                int iTable74Size = 11;

                // Now we need to calculate an Entry Length
                if (true == HistoryDateTimeFlag)
                {
                    iEntryLength += LTIME_LENGTH;
                }
                if (true == EventNumberFlag)
                {
                    iEntryLength += UINT16_LENGTH;
                }
                if (true == HistorySequenceNumberFlag)
                {
                    iEntryLength += UINT16_LENGTH;
                }
                // Now add in two constant values
                iEntryLength += (UINT16_LENGTH + IDB_BFLD_LENGTH);
                // Finally add in the History Arguments
                iEntryLength += HistoryDataLength;

                // Now that we know the Entry Length we can finish calculating
                //	the table size.
                iTable74Size += (int)(iEntryLength * NumberHistoryEntries);

                return (uint) iTable74Size;
            }
        }

        /// <summary>
        /// Property that will return the size of 76 based off the values in 71
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/11/10 jrf 2.41.01		Created
        /// 
        public uint SizeOfTable76
        {
            get
            {
                // Start size with the constant data: List Status, Nbr valid entries
                //	Last Entry Element, Last Entry Seq Number and # unread entries
                uint uiTable76Size = 11;

                // Now we can finish calculating the table size.
                uiTable76Size += (uint)(SizeOfEventEntry * NumberEventEntries);

                return uiTable76Size;
            }
        }

        /// <summary>
        /// Property that will return the size of 76 based off the values in 71
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/11/10 jrf 2.41.01		Created
        /// 
        public uint SizeOfEventEntry
        {
            get
            {
                uint uiEntryLength = 0;

                // Now we need to calculate an Entry Length
                // Event time
                uiEntryLength += LTIME_LENGTH;

                if (true == EventNumberFlag)
                {
                    uiEntryLength += UINT16_LENGTH;
                }
                // Sequence number
                uiEntryLength += UINT16_LENGTH;

                // Now add in two constant values (user ID and event code bitfield)
                uiEntryLength += (UINT16_LENGTH + EC_BFLD_LENGTH);
                // Finally add in the History Arguments
                uiEntryLength += EventDataLength;

                return (uint)uiEntryLength;
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of std events supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.04        Added for support of std table 73
        //
        public uint NumberStandardEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_byNbrStdEvents;
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of mfg events supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.04        Added for support of std table 73
        //
        public uint NumberManufacturerEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_byNbrMfgEvents;
            }
        }

        #endregion

	    #region Members

        private byte m_byLogFlagBfld;
        private byte m_byNbrStdEvents;
        private byte m_byNbrMfgEvents;
        private byte m_byHistDataLength;
        private byte m_byEventDataLength;
        private uint m_uiNbrHistoryEntries;
        private uint m_uiNbrEventEntries;

	    #endregion

    }

    /// <summary>
    /// Standard Table 72
    /// </summary>
    public class StdTable72 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Standard Table 72 - Event Identification table constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="table71">Std table 71 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/10 AF  2.41.04        Added for support of M2 Gateway meter
        //
        public StdTable72(CPSEM psem, StdTable71 table71)
            : base(psem, 72, GetTableLength(table71), TABLE_TIMEOUT)
        {
            m_Table71 = table71;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table71">Standard Table 71 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Created
        //
        public StdTable72(PSEMBinaryReader reader, StdTable71 table71)
            : base(72, GetTableLength(table71))
        {
            m_Table71 = table71;
            m_Reader = reader;

            ParseData();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Reads Standard Table 72 out of the meter
        /// </summary>
        /// <returns>The result code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/10 AF  2.41.04        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable72.Read");

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
        /// Each bit position in the byte array returned represents a std event.
        /// If the event is supported, the bit value is 1; otherwise, 0.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/10 AF  2.41.04        Created
        //
        public byte[] StdEventsSupported
        {
            get
            {
                ReadUnloadedTable();
                
                return m_abyStdEventsSupported;
            }
        }

        /// <summary>
        /// Each bit position in the byte array returned represents an mfg event.
        /// If the event is supported, the bit value is 1; otherwise, 0.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/10 AF  2.41.04        Created
        //
        public byte[] MfgEventsSupported
        {
            get
            {
                ReadUnloadedTable();

                return m_abyMfgEventsSupported;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table.
        /// </summary>
        /// <param name="table71">std table 71</param>
        /// <returns>length in bytes of table 72</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/10 AF  2.41.01        Created
        //
        static private uint GetTableLength(StdTable71 table71)
        {
            return (table71.NumberStandardEvents + table71.NumberManufacturerEvents);
        }

        /// <summary>
        /// Gets the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/10 AF  2.41.01        Created
        //
        private void ParseData()
        {
            m_abyStdEventsSupported = m_Reader.ReadBytes((int)(m_Table71.NumberStandardEvents));
            m_abyMfgEventsSupported = m_Reader.ReadBytes((int)(m_Table71.NumberManufacturerEvents));
        }

        #endregion

        #region Members

        private StdTable71 m_Table71;
        private byte[] m_abyStdEventsSupported;
        private byte[] m_abyMfgEventsSupported;

        #endregion

    }

    /// <summary>
    /// Standard Table 73
    /// </summary>
    public class StdTable73 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Definitions

        /// <summary>The Resource Project strings</summary>
        protected static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                            "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";

        #endregion

        #region Public Methods

        /// <summary>
        /// Standard Table 73 - History Log Control table constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="table72">Std table 72 object</param>
        /// <param name="table71">Std table 71 object</param>
        /// <param name="table00">Std table 00 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.01        Added for support of M2 Gateway meter
        //  06/04/10 AF  2.41.06        Added std table 72 parameter
        //
        public StdTable73(CPSEM psem, StdTable72 table72, StdTable71 table71, CTable00 table00)
            : base(psem, 73, GetTableLength(table71, table00), TABLE_TIMEOUT)
        {
            m_Table00 = table00;
            m_Table71 = table71;
            m_Table72 = table72;
            m_psem = psem;
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                               this.GetType().Assembly);
            m_lstEvents = new List<MFG2048EventItem>();
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table72">Std table 72 object</param>
        /// <param name="table71">Std table 71 objec</param>
        /// <param name="table00">Std table 00 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Created
        //
        public StdTable73(PSEMBinaryReader reader, StdTable72 table72, StdTable71 table71, CTable00 table00)
            : base(73, GetTableLength(table71, table00))
        {
            m_Table00 = table00;
            m_Table71 = table71;
            m_Table72 = table72;
            m_Reader = reader;
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                               this.GetType().Assembly);
            m_lstEvents = new List<MFG2048EventItem>();

            ParseData();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Reads Standard Table 73 out of the meter
        /// </summary>
        /// <returns>The PSEM result code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.01        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable73.Read");

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
        /// Returns a list of supported events with their descriptions and whether 
        /// or not they are monitored.  Designed to provide the same information as 
        /// MFGTable2048.HistoryLogConfig.HistoryLogEventList
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/10 AF  2.41.04        Created
        //  06/04/10 AF  2.41.06        Added a table 72 member variable
        //
        public List<MFG2048EventItem> HistoryLogEventList
        {
            get
            {
                List<EventEntry> lstSupportedEvents = new List<EventEntry>();

                ReadUnloadedTable();

                m_lstEvents.Clear();

                byte[] abyStdEventsSupported = m_Table72.StdEventsSupported;
                byte[] abyMfgEventsSupported = m_Table72.MfgEventsSupported;

                AddSupportedEvents(ref lstSupportedEvents, abyStdEventsSupported, m_abyStdEventsMonitored, false);
                AddSupportedEvents(ref lstSupportedEvents, abyMfgEventsSupported, m_abyMfgEventsMonitored, true);

                foreach (EventEntry entry in lstSupportedEvents)
                {
                    AddEventItem(entry);
                }

                return m_lstEvents;
            }
        }

       

        /// <summary>
        /// Returns a list of monitored events with their descriptions.  Designed to provide
        /// the same type of information as MFGTable2048.HistoryLogConfig.HistoryLogEventList
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/10 AF  2.41.08        Created
        //
        public List<MFG2048EventItem> HistoryLogMonitoredEventList
        {
            get
            {
                List<EventEntry> lstMonitoredEvents = new List<EventEntry>();

                ReadUnloadedTable();

                m_lstEvents.Clear();

                AddMonitoredEvents(ref lstMonitoredEvents, m_abyStdEventsMonitored, false);
                AddMonitoredEvents(ref lstMonitoredEvents, m_abyMfgEventsMonitored, true);

                foreach (EventEntry entry in lstMonitoredEvents)
                {
                    AddEventItem(entry);
                }

                return m_lstEvents;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table.
        /// </summary>
        /// <param name="table71">std table 71</param>
        /// <param name="table00">std table 00</param>
        /// <returns>length in bytes of table 73</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.04        Created
        //
        static private uint GetTableLength(StdTable71 table71, CTable00 table00)
        {
            uint uiTableLength = table71.NumberStandardEvents + table71.NumberManufacturerEvents
                + table00.DimStdTablesUsed + table00.DimMfgTablesUsed
                + table00.DimStdProceduresUsed + table00.DimMfgProceduresUsed;
            return uiTableLength;
        }

        /// <summary>
        /// Gets the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.04        Created
        //
        private void ParseData()
        {
            m_abyStdEventsMonitored = m_Reader.ReadBytes((int)(m_Table71.NumberStandardEvents));
            m_abyMfgEventsMonitored = m_Reader.ReadBytes((int)(m_Table71.NumberManufacturerEvents));
            m_abyStdTablesMonitored = m_Reader.ReadBytes(m_Table00.DimStdTablesUsed);
            m_abyMfgTablesMonitored = m_Reader.ReadBytes(m_Table00.DimMfgTablesUsed);
            m_abyStdProcMonitored = m_Reader.ReadBytes(m_Table00.DimStdProceduresUsed);
            m_abyMfgProcMonitored = m_Reader.ReadBytes(m_Table00.DimMfgProceduresUsed);
        }

        /// <summary>
        /// Produces a list of supported events from the information read from standard
        /// tables 72 and 73
        /// </summary>
        /// <param name="SupportedEvents">The list being constructed</param>
        /// <param name="StdOrMfgEventsSupported">
        /// Byte array, each bit of which represents a standard or manufacturers event.
        /// The value of the bit is 1 if the event is supported; 0, if not.
        /// </param>
        /// <param name="StdOrMfgEventsMonitored">
        /// Byte array, each bit of which represents a std or mfg event.
        /// The value of the bit is 1 if the event is monitored; 0, if not.
        /// </param>
        /// <param name="bIsMfgEvent">
        /// Flag to let us know to add 2048 to the event id.
        /// </param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/10 AF  2.41.04        Created
        //
        private void AddSupportedEvents(ref List<EventEntry> SupportedEvents, byte[] StdOrMfgEventsSupported, byte[] StdOrMfgEventsMonitored, bool bIsMfgEvent)
        {
            byte byBitMask;
            UInt16 iEventNumber;

            for (int iIndex = 0; iIndex < StdOrMfgEventsSupported.Length; iIndex++)
            {
                for (int iBitIndex = 0; iBitIndex < 8; iBitIndex++)
                {
                    EventEntry Event = new EventEntry();

                    iEventNumber = (UInt16)(8 * iIndex + iBitIndex);
                    if (bIsMfgEvent)
                    {
                        iEventNumber += 2048;
                    }
                    byBitMask = (byte)(1 << iBitIndex);
                    if ((StdOrMfgEventsSupported[iIndex] & byBitMask) == byBitMask)
                    {
                        // event is supported - add to the event list
                        Event.HistoryCode = iEventNumber;
                        // check to see if it is monitored
                        if ((StdOrMfgEventsMonitored[iIndex] & byBitMask) == byBitMask)
                        {
                            Event.Monitored = true;
                        }
                        else
                        {
                            Event.Monitored = false;
                        }

                        SupportedEvents.Add(Event);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitoredEvents"></param>
        /// <param name="StdOrMfgEventsMonitored"></param>
        /// <param name="bIsMfgEvent"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        private void AddMonitoredEvents(ref List<EventEntry> MonitoredEvents, byte[] StdOrMfgEventsMonitored, bool bIsMfgEvent)
        {
            byte byBitMask;
            UInt16 iEventNumber;
            for (int iIndex = 0; iIndex < StdOrMfgEventsMonitored.Length; iIndex++)
            {

                for (int iBitIndex = 0; iBitIndex < 8; iBitIndex++)
                {
                    iEventNumber = (UInt16)(8 * iIndex + iBitIndex);

                    if (bIsMfgEvent)
                    {
                        iEventNumber += 2048;
                    }
                    byBitMask = (byte)(1 << iBitIndex);

                    // check to see if it is monitored
                    if ((StdOrMfgEventsMonitored[iIndex] & byBitMask) == byBitMask)
                    {
                        EventEntry Event = new EventEntry();
                        Event.HistoryCode = iEventNumber;
                        Event.Monitored = true;
                        MonitoredEvents.Add(Event);
                    }
                }
            }
        }

        /// <summary>
        /// Takes an event entry item, translates the event id into a text description 
        /// and adds it to the MFG2048EventItem list
        /// </summary>
        /// <param name="entry"></param>
        //  Revision History	
        //  MM/DD/YY Who   Version   Issue#      Description
        //  -------- ---   -------   ------      -------------------------------------------
        //  05/25/10 AF    2.41.04               Created
        //  09/30/10 AF    2.45.00   162279      Added 2 events for the M2 Gateway
        //  03/08/11 jrf   2.50.07               Added case for HAN event cache overflow event.
        //  03/10/11 jrf   2.50.08               Added case for the CPP event
        //  08/11/11 jrf   2.52.02   TREQ2709    Changing Register Firmware Download Status event
        //                                            to Firmware Download Event Log Full event.
        //  03/08/12 AF    2.53.48   177560      Changed the power outage description to primary power down to match
        //                                            the ANSIEventDictionary value
        //  09/20/12 jrf   2.70.18   TQ6658      Added magnetic tamper events.
        //  04/07/14 AF    3.50.62   WR466261    Reworded a couple of descriptions to substitute ICM for SSI
        //  05/30/14 jrf   3.50.97   517744      The name of mfg. event 163 changed from HAN Load Control Event Sent to ICS ERT Event.
        //  05/20/16 MP    4.50.270  wR685690    Added support for EVENT_HARDWARE_ERROR_DETECTION
        //  06/15/16 MP   4.50.284   WR680128    Added On Demand Periodic Read
        //  06/20/16 PGH  4.50.289   680128      Don't add history code to description for unknown event
        //  07/12/16 MP   4.70.7   WR688986      Changed how event descriptions were accessed
        private void AddEventItem(EventEntry entry)
        {
            MFG2048EventItem eventItem = new MFG2048EventItem();
            eventItem.ID = entry.HistoryCode;
           
            // Pull description from resource file
            try
            {
                string Name = Enum.GetName(typeof(CANSIDevice.HistoryEvents), entry.HistoryCode);
                if (!string.IsNullOrEmpty(Name))
                {
                    eventItem.Description = m_rmStrings.GetString(Name);
                }
                else
                {
                    eventItem.Description = "";
                }
            }
            catch (Exception)
            {
                eventItem.Description = "";
            }

  
            // Check if event is monitored or if description was empty.
            eventItem.Enabled = entry.Monitored;

            if (eventItem.Description == "")
            {
                eventItem.Description = m_rmStrings.GetString("UNKNOWN_EVENT");
            }

            m_lstEvents.Add(eventItem);
        }

        #endregion

        #region Members

        private byte[] m_abyStdEventsMonitored;
        private byte[] m_abyMfgEventsMonitored;
        private byte[] m_abyStdTablesMonitored;
        private byte[] m_abyMfgTablesMonitored;
        private byte[] m_abyStdProcMonitored;
        private byte[] m_abyMfgProcMonitored;
        private CPSEM m_psem;
        private CTable00 m_Table00;
        private StdTable71 m_Table71;
        private StdTable72 m_Table72;
        private List<MFG2048EventItem> m_lstEvents;
        /// <summary>The Resource Manager</summary>
        protected System.Resources.ResourceManager m_rmStrings;

        #endregion

    }

    /// <summary>
    /// History Log Class - Table 74
    /// </summary>
    public class StdTable74 : AnsiTable
	{
        #region Constants

        private const int LTIME_LENGTH = 5;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

	    #region Definitions
	    #endregion

	    #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="table71">Table 71 object</param>	
        /// <param name="eventDictionary">Dictionary containing the event descriptions</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CStdTable121 Table121 = new CStdTable127( PSEM ); 
        /// </code>
        /// </example>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/16/06 KRC 7.35.00 N/A    Created
        /// 05/14/13 MSC 2.80.29 TQ7640 Added Support for Comm Module Events
        /// 05/22/13 MSC 2.80.31 TQ7640 Removed previous changes
        /// 
        public StdTable74(CPSEM psem, StdTable71 table71, ANSIEventDictionary eventDictionary)
            : base(psem, 74, table71.SizeOfTable74, TABLE_TIMEOUT)
        {
            m_table71 = table71;
            m_collEntries = new List<HistoryEntry>();
            m_EventDictionary = eventDictionary;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)psem.TimeFormat;
        }

        /// <summary>
        /// Constructor that reads the table from a binary reader
        /// </summary>
        /// <param name="reader">The binary reader that contains the table contents.</param>
        /// <param name="table71">The Table 71 object for the device</param>
        /// <param name="eventDictionary">The event dictionary for the device</param>
        /// <param name="timeFormat">The time format used in the meter.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/10 RCG 2.40.31 151959 Created

        public StdTable74(PSEMBinaryReader reader, StdTable71 table71, ANSIEventDictionary eventDictionary, int timeFormat)
            : base(74, table71.SizeOfTable74)
        {
            m_table71 = table71;
            m_collEntries = new List<HistoryEntry>();
            m_EventDictionary = eventDictionary;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)timeFormat;
            m_Reader = reader;

            ParseFromReader();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Reads table 74 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public override PSEMResponse Read()
        {
            UInt16 uiNbrHistoryEntries = 0;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable74.Read");

            //Read the table			
            //PSEMResponse Result = base.Read();
            PSEMResponse Result = base.Read(0, 11);

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variable that represent the table
                // Read out unused data
                m_byHistFlags = m_Reader.ReadByte();
                m_uiNbrValidEntries = m_Reader.ReadUInt16();
                m_uiLastEntryElement = m_Reader.ReadUInt16();
                m_uiLastEntrySeqNbr = m_Reader.ReadUInt32();
                m_uiNbrUnreadEntries = m_Reader.ReadUInt16();

                //Don't do any more if there are no events
                if (m_uiNbrValidEntries > 0)
                {
                    // Now we know how many events there are so we can read the correct amount.
                    Result = base.Read(11, (ushort)(m_uiNbrValidEntries * HistoryEntryLength));

                    if (PSEMResponse.Ok == Result)
                    {
                        // Now we are going to read the collection
                        m_collEntries.Clear();
                        for (uiNbrHistoryEntries = 0;
                            uiNbrHistoryEntries < m_uiNbrValidEntries;
                            uiNbrHistoryEntries++)
                        {
                            HistoryEntry histEntry = new HistoryEntry(m_table71.HistoryDateTimeFlag,
                                                                      m_table71.EventNumberFlag,
                                                                      m_table71.HistorySequenceNumberFlag,
                                                                      m_table71.HistoryDataLength,
                                                                      m_EventDictionary);

                            if (true == m_table71.HistoryDateTimeFlag)
                            {
                                histEntry.HistoryTime = m_Reader.ReadLTIME(m_TimeFormat);
                            }
                            if (true == m_table71.EventNumberFlag)
                            {
                                histEntry.EventNumber = m_Reader.ReadUInt16();
                            }
                            if (true == m_table71.HistorySequenceNumberFlag)
                            {
                                histEntry.HistorySequenceNumber = m_Reader.ReadUInt16();
                            }
                            histEntry.UserID = m_Reader.ReadUInt16();
                            histEntry.HistoryCode = m_Reader.ReadUInt16();
                            histEntry.HistoryArgument = m_Reader.ReadBytes(m_table71.HistoryDataLength);

                            m_collEntries.Add(histEntry);
                        }
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Provide a way to have us reread the Events
        /// </summary>
        public void Refresh()
        {
            m_TableState = TableState.Expired;
            m_collEntries.Clear();
        }

        #endregion

	    #region Public Properties

        /// <summary>
        /// Access to the History Log Event Collection
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public List<HistoryEntry> HistoryLogEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading History Log Entries"));
                    }
                }
                return m_collEntries;
            }
        }

        /// <summary>
        /// Access to the Number of valid Entries count
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public UInt16 NumberValidEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Number of Valid History Log Entries"));
                    }
                }
                return m_uiNbrValidEntries;
            }
        }

        /// <summary>
        /// Access to the Last Entry Element
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public UInt16 LastEntryElement
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Last History Log Entry"));
                    }
                }
                return m_uiLastEntryElement;
            }
        }

        /// <summary>
        /// Access to the Last Entry Sequence Number
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public UInt32 LastEntrySequenceNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Last History Log Entry Sequence Number"));
                    }
                }
                return m_uiLastEntrySeqNbr;
            }
        }

        /// <summary>
        /// Access to the Number of Unread Entries
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public UInt16 NumberUnreadEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading number of unread History Log Entries"));
                    }
                }
                return m_uiNbrUnreadEntries;
            }
        }

	    #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data from the binary reader. This should not be used when reading data from an actual meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/10 RCG 2.40.31 151959 Created

        private void ParseFromReader()
        {
            m_DataStream.Position = 0;

            //Populate the member variable that represent the table
            // Read out unused data
            m_byHistFlags = m_Reader.ReadByte();
            m_uiNbrValidEntries = m_Reader.ReadUInt16();
            m_uiLastEntryElement = m_Reader.ReadUInt16();
            m_uiLastEntrySeqNbr = m_Reader.ReadUInt32();
            m_uiNbrUnreadEntries = m_Reader.ReadUInt16();

            m_collEntries.Clear();

            // Now we are going to read the collection
            for (uint uiNbrHistoryEntries = 0; uiNbrHistoryEntries < m_uiNbrValidEntries; uiNbrHistoryEntries++)
            {
                HistoryEntry histEntry = new HistoryEntry(m_table71.HistoryDateTimeFlag,
                                                          m_table71.EventNumberFlag,
                                                          m_table71.HistorySequenceNumberFlag,
                                                          m_table71.HistoryDataLength,
                                                          m_EventDictionary);

                if (true == m_table71.HistoryDateTimeFlag)
                {
                    histEntry.HistoryTime = m_Reader.ReadLTIME(m_TimeFormat);
                }

                if (true == m_table71.EventNumberFlag)
                {
                    histEntry.EventNumber = m_Reader.ReadUInt16();
                }

                if (true == m_table71.HistorySequenceNumberFlag)
                {
                    histEntry.HistorySequenceNumber = m_Reader.ReadUInt16();
                }

                histEntry.UserID = m_Reader.ReadUInt16();
                histEntry.HistoryCode = m_Reader.ReadUInt16();
                histEntry.HistoryArgument = m_Reader.ReadBytes(m_table71.HistoryDataLength);

                m_collEntries.Add(histEntry);
            }
        }

        #endregion

        #region Private Properties

        // Calculates the size of a History Entry
        private uint HistoryEntryLength
        {
            get
            {
                uint uiHistoryEntryLength = 0;
                if (true == m_table71.HistoryDateTimeFlag)
                {
                    //KRC:TODO - Determine value based on table 0
                    // Add in space for the LTIME
                    uiHistoryEntryLength += LTIME_LENGTH;
                }
                if (true == m_table71.EventNumberFlag)
                {
                    // Add in space for Event Number
                    uiHistoryEntryLength += sizeof(UInt16);
                }
                if (true == m_table71.HistorySequenceNumberFlag)
                {
                    // Add in space for Sequence Number
                    uiHistoryEntryLength += sizeof(UInt16);
                }
                // Add in space for User ID
                uiHistoryEntryLength += sizeof(UInt16);
                // Add in space for History Code
                uiHistoryEntryLength += sizeof(UInt16);
                // Add in space for History Arguments
                uiHistoryEntryLength += m_table71.HistoryDataLength;

                return uiHistoryEntryLength;
            }
        }

        #endregion

        #region Members

        StdTable71 m_table71;
        private byte m_byHistFlags;
        private UInt16 m_uiNbrValidEntries;
        private UInt16 m_uiLastEntryElement;
        private UInt32 m_uiLastEntrySeqNbr;
        private UInt16 m_uiNbrUnreadEntries;
        private List<HistoryEntry> m_collEntries;
        private ANSIEventDictionary m_EventDictionary;
        private PSEMBinaryReader.TM_FORMAT m_TimeFormat;

	    #endregion
	}

    /// <summary>
    /// Standard Table 75
    /// </summary>
    public class StdTable75 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Definitions

        /// <summary>The Resource Project strings</summary>
        protected static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                            "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";

        #endregion

        #region Public Methods

        /// <summary>
        /// Standard Table 75 - History Log Control table constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="table72">Std table 72 object</param>
        /// <param name="table71">Std table 71 object</param>
        /// <param name="table00">Std table 00 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.01        Added for support of M2 Gateway meter
        //  06/04/10 AF  2.41.06        Added std table 72 parameter
        //
        public StdTable75(CPSEM psem, StdTable72 table72, StdTable71 table71, CTable00 table00)
            : base(psem, 75, GetTableLength(table71, table00), TABLE_TIMEOUT)
        {
            m_Table00 = table00;
            m_Table71 = table71;
            m_Table72 = table72;
            m_psem = psem;
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                               this.GetType().Assembly);
            m_lstEvents = new List<MFG2048EventItem>();
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table72">Std table 72 object</param>
        /// <param name="table71">Std table 71 objec</param>
        /// <param name="table00">Std table 00 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Created
        //
        public StdTable75(PSEMBinaryReader reader, StdTable72 table72, StdTable71 table71, CTable00 table00)
            : base(75, GetTableLength(table71, table00))
        {
            m_Table00 = table00;
            m_Table71 = table71;
            m_Table72 = table72;
            m_Reader = reader;
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                               this.GetType().Assembly);
            m_lstEvents = new List<MFG2048EventItem>();

            ParseData();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Reads Standard Table 73 out of the meter
        /// </summary>
        /// <returns>The PSEM result code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.01        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable75.Read");

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
        /// Returns a list of monitored events with their descriptions.  Designed to provide
        /// the same type of information as MFGTable2048.HistoryLogConfig.HistoryLogEventList
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/10 AF  2.41.08        Created
        //
        public List<MFG2048EventItem> EventLogMonitoredEventList
        {
            get
            {
                List<EventEntry> lstMonitoredEvents = new List<EventEntry>();

                ReadUnloadedTable();

                m_lstEvents.Clear();

                AddMonitoredEvents(ref lstMonitoredEvents, m_abyStdEventsMonitored, false);
                AddMonitoredEvents(ref lstMonitoredEvents, m_abyMfgEventsMonitored, true);

                foreach (EventEntry entry in lstMonitoredEvents)
                {
                    AddEventItem(entry);
                }

                return m_lstEvents;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table.
        /// </summary>
        /// <param name="table71">std table 71</param>
        /// <param name="table00">std table 00</param>
        /// <returns>length in bytes of table 73</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.04        Created
        //
        static private uint GetTableLength(StdTable71 table71, CTable00 table00)
        {
            uint uiTableLength = table71.NumberStandardEvents + table71.NumberManufacturerEvents
                + table00.DimStdTablesUsed + table00.DimMfgTablesUsed
                + table00.DimStdProceduresUsed + table00.DimMfgProceduresUsed;
            return uiTableLength;
        }

        /// <summary>
        /// Gets the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.04        Created
        //
        private void ParseData()
        {
            m_abyStdEventsMonitored = m_Reader.ReadBytes((int)(m_Table71.NumberStandardEvents));
            m_abyMfgEventsMonitored = m_Reader.ReadBytes((int)(m_Table71.NumberManufacturerEvents));
            m_abyStdTablesMonitored = m_Reader.ReadBytes(m_Table00.DimStdTablesUsed);
            m_abyMfgTablesMonitored = m_Reader.ReadBytes(m_Table00.DimMfgTablesUsed);
            m_abyStdProcMonitored = m_Reader.ReadBytes(m_Table00.DimStdProceduresUsed);
            m_abyMfgProcMonitored = m_Reader.ReadBytes(m_Table00.DimMfgProceduresUsed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitoredEvents"></param>
        /// <param name="StdOrMfgEventsMonitored"></param>
        /// <param name="bIsMfgEvent"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        private void AddMonitoredEvents(ref List<EventEntry> MonitoredEvents, byte[] StdOrMfgEventsMonitored, bool bIsMfgEvent)
        {
            byte byBitMask;
            UInt16 iEventNumber;
            for (int iIndex = 0; iIndex < StdOrMfgEventsMonitored.Length; iIndex++)
            {

                for (int iBitIndex = 0; iBitIndex < 8; iBitIndex++)
                {
                    iEventNumber = (UInt16)(8 * iIndex + iBitIndex);

                    if (bIsMfgEvent)
                    {
                        iEventNumber += 2048;
                    }
                    byBitMask = (byte)(1 << iBitIndex);

                    // check to see if it is monitored
                    if ((StdOrMfgEventsMonitored[iIndex] & byBitMask) == byBitMask)
                    {
                        EventEntry Event = new EventEntry();
                        Event.HistoryCode = iEventNumber;
                        Event.Monitored = true;
                        MonitoredEvents.Add(Event);
                    }
                }
            }
        }

        /// <summary>
        /// Takes an event entry item, translates the event id into a text description 
        /// and adds it to the MFG2048EventItem list
        /// </summary>
        /// <param name="entry"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/10 AF  2.41.04        Created
        //  09/30/10 AF  2.45.00 162279 Added 2 events for the M2 Gateway
        //  03/08/11 jrf 2.50.07        Added case for HAN event cache overflow event.
        //  03/10/11 jrf 2.50.08        Added case for the CPP event
        //  08/11/11 jrf  2.52.02 TREQ2709 Changing Register Firmware Download Status event
        //                                to Firmware Download Event Log Full event.
        //  09/20/12 jrf 2.70.18 TQ6658 Added magnetic tamper events.
        //  05/30/14 jrf  3.50.97 517744 The name of mfg. event 163 changed from HAN Load Control Event Sent to ICS ERT Event.
        //  06/15/16 MP   4.50.284 WR680128  Added On Demand Periodic Read
        //  07/12/16 MP   4.70.7   WR688986  Changed how event descriptions were accessed
        private void AddEventItem(EventEntry entry)
        {
            MFG2048EventItem eventItem = new MFG2048EventItem();
            eventItem.ID = entry.HistoryCode;

            // Pull description from resource file
            try
            {
                string Name = Enum.GetName(typeof(CANSIDevice.HistoryEvents), entry.HistoryCode);
                if (!string.IsNullOrEmpty(Name))
                {
                    eventItem.Description = m_rmStrings.GetString(Name);
                }
                else
                {
                    eventItem.Description = "";
                }
            }
            catch (Exception)
            {
                eventItem.Description = "";
            }

            // Check if event is monitored or if description came back empty
            eventItem.Enabled = entry.Monitored;

            if (eventItem.Description == "")
            {
                eventItem.Description = "Unknown Event " + entry.HistoryCode.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            m_lstEvents.Add(eventItem);
        }

        #endregion


        #region Members

        private byte[] m_abyStdEventsMonitored;
        private byte[] m_abyMfgEventsMonitored;
        private byte[] m_abyStdTablesMonitored;
        private byte[] m_abyMfgTablesMonitored;
        private byte[] m_abyStdProcMonitored;
        private byte[] m_abyMfgProcMonitored;
        private CPSEM m_psem;
        private CTable00 m_Table00;
        private StdTable71 m_Table71;
        private StdTable72 m_Table72;
        private List<MFG2048EventItem> m_lstEvents;
        /// <summary>The Resource Manager</summary>
        protected System.Resources.ResourceManager m_rmStrings;

        #endregion

    }

    /// <summary>
    /// Event Log Class - Table 76
    /// </summary>
    public class StdTable76 : AnsiTable
    {
        #region Constants

        private const int LTIME_LENGTH = 5;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="table71">Table 71 object</param>	
        /// <param name="eventDictionary">Dictionary containing the event descriptions</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CStdTable121 Table121 = new CStdTable127( PSEM ); 
        /// </code>
        /// </example>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/11/10 jrf 2.41.01 N/A    Created
        ///
        public StdTable76(CPSEM psem, StdTable71 table71, ANSIEventDictionary eventDictionary)
            : base(psem, 76, table71.SizeOfTable76, 5000)
        {
            m_table71 = table71;
            m_collEntries = new List<HistoryEntry>();
            m_EventDictionary = eventDictionary;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)psem.TimeFormat;
        }

        /// <summary>
        /// Constructor that reads the table from a binary reader
        /// </summary>
        /// <param name="reader">The binary reader that contains the table contents.</param>
        /// <param name="table71">The Table 71 object for the device</param>
        /// <param name="eventDictionary">The event dictionary for the device</param>
        /// <param name="timeFormat">The time format used in the meter.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created

        public StdTable76(PSEMBinaryReader reader, StdTable71 table71, ANSIEventDictionary eventDictionary, int timeFormat)
            : base(76, table71.SizeOfTable76)
        {
            m_table71 = table71;
            m_collEntries = new List<HistoryEntry>();
            m_EventDictionary = eventDictionary;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)timeFormat;
            m_Reader = reader;

            ParseFromReader();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Reads table 76 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable76.Read");

            //Read the table			
            //PSEMResponse Result = base.Read();
            PSEMResponse Result = base.Read(0, 11);

            if (PSEMResponse.Ok == Result)
            {
                ParseHeaderData();

                //Don't do any more if there are no events
                if (m_uiNbrValidEntries > 0)
                {
                    // Now we know how many events there are so we can read the correct amount.
                    Result = base.Read(11, (ushort)(m_uiNbrValidEntries * m_table71.SizeOfEventEntry));

                    if (PSEMResponse.Ok == Result)
                    {
                        ParseEventData();
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Provide a way to have us reread the Events
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created
        //
        public void Refresh()
        {
            m_TableState = TableState.Expired;
            m_collEntries.Clear();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Access to the Event Log Event Collection
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created
        //
        public List<HistoryEntry> EventLogEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Event Log Entries"));
                    }
                }
                return m_collEntries;
            }
        }

        /// <summary>
        /// Access to the Number of valid Entries count
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created
        //
        public UInt16 NumberValidEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Number of Valid Event Log Entries"));
                    }
                }
                return m_uiNbrValidEntries;
            }
        }

        /// <summary>
        /// Access to the Last Entry Element
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created
        //
        public UInt16 LastEntryElement
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Last Event Log Entry"));
                    }
                }
                return m_uiLastEntryElement;
            }
        }

        /// <summary>
        /// Access to the Last Entry Sequence Number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created
        //
        public UInt32 LastEntrySequenceNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Last Event Log Entry Sequence Number"));
                    }
                }
                return m_uiLastEntrySeqNbr;
            }
        }

        /// <summary>
        /// Access to the Number of Unread Entries
        /// </summary>
        /// Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created
        //
        public UInt16 NumberUnreadEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading number of unread Event Log Entries"));
                    }
                }
                return m_uiNbrUnreadEntries;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data from the binary reader. This should not be used when reading data from an actual meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created

        private void ParseFromReader()
        {
            ParseHeaderData();

            ParseEventData();
        }

        /// <summary>
        /// Parses the header data from the binary reader. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created

        private void ParseHeaderData()
        {
            m_DataStream.Position = 0;

            //Populate the member variable that represent the table
            // Read out unused data
            m_byListStatusFlags = m_Reader.ReadByte();
            m_uiNbrValidEntries = m_Reader.ReadUInt16();
            m_uiLastEntryElement = m_Reader.ReadUInt16();
            m_uiLastEntrySeqNbr = m_Reader.ReadUInt32();
            m_uiNbrUnreadEntries = m_Reader.ReadUInt16();
        }


        /// <summary>
        /// Parses the event data from the binary reader.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01 N/A    Created

        private void ParseEventData()
        {
            m_collEntries.Clear();

            // Now we are going to read the collection
            for (uint uiNbrHistoryEntries = 0; 
                uiNbrHistoryEntries < m_uiNbrValidEntries; 
                uiNbrHistoryEntries++)
            {
                HistoryEntry EventEntry = new HistoryEntry(true,
                                                          m_table71.EventNumberFlag,
                                                          true,
                                                          m_table71.EventDataLength,
                                                          m_EventDictionary);

                EventEntry.HistoryTime = m_Reader.ReadLTIME(m_TimeFormat);

                if (true == m_table71.EventNumberFlag)
                {
                    EventEntry.EventNumber = m_Reader.ReadUInt16();
                }

                EventEntry.HistorySequenceNumber = m_Reader.ReadUInt16();
                EventEntry.UserID = m_Reader.ReadUInt16();
                EventEntry.HistoryCode = m_Reader.ReadUInt16();
                EventEntry.HistoryArgument = m_Reader.ReadBytes(m_table71.EventDataLength);

                m_collEntries.Add(EventEntry);
            }
        }

        #endregion

        #region Members

        StdTable71 m_table71;
        private byte m_byListStatusFlags;
        private UInt16 m_uiNbrValidEntries;
        private UInt16 m_uiLastEntryElement;
        private UInt32 m_uiLastEntrySeqNbr;
        private UInt16 m_uiNbrUnreadEntries;
        private List<HistoryEntry> m_collEntries;
        private ANSIEventDictionary m_EventDictionary;
        private PSEMBinaryReader.TM_FORMAT m_TimeFormat;

        #endregion
    }

    /// <summary>
    /// This class represents the individual History Entries
    /// </summary>
    /// Revision History
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------------
    /// 09/18/06 KRC 7.35.00        Created for AMI
    ///
    public class HistoryEntry : IEquatable<HistoryEntry>
    {
        #region Constants

        private const ushort TBL_PROC_MASK = 0x07FF;
        private const ushort STD_V_MFG_MASK = 0x0800;
        private const ushort SELECTOR_MASK = 0xF000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for the History Entry
        /// </summary>
        /// <param name="bHistDateTimeFlag"></param>
        /// <param name="bEventNbrFlag"></param>
        /// <param name="bHistSeqNbrFlag"></param>
        /// <param name="uiHistDataLength"></param>
        /// <param name="dicEventDescriptions"></param>
        public HistoryEntry(bool bHistDateTimeFlag, bool bEventNbrFlag,
                            bool bHistSeqNbrFlag, uint uiHistDataLength,
                            ANSIEventDictionary dicEventDescriptions)
        {
            m_bHistDateTimeFlag = bHistDateTimeFlag;
            m_bEventNbrFlag = bEventNbrFlag;
            m_bHistSeqNbrFlag = bHistSeqNbrFlag;
            m_uiHistDataLength = uiHistDataLength;
            m_dicEventDescriptions = dicEventDescriptions;
        }

        /// <summary>
        /// Adds information to the event description based on the value of the
        /// history argument.  Initially, only loss of potential will be supported.
        /// </summary>
        /// <returns>
        /// Additional description based on the event's history argument
        /// </returns>
        /// 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/07 AF  8.00.24 2675   Created to provide a way to get 
        //                              information from the history argument
        //  04/30/08 KRC 1.50           Start returning translated Event data
        //  03/15/12 jrf 2.53.50 TREQ5571 Switched to pass as argument a HistoryEntry instead of just 
        //                               the history code to TranslatedEventData().  Also removing 
        //                               the unnecessary passing of the event code to this method;  
        //                               this method already has access to the event code internally.
        //
        public String TranslatedEventData()
        {
            MemoryStream TempStream = new MemoryStream(m_abytHistoryArgument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(TempStream);
            return m_dicEventDescriptions.TranslatedEventData(this, ArgumentReader);
        }

        /// <summary>
        /// Determines whether the specified HistoryEntry is equal to the current entry.
        /// </summary>
        /// <param name="other">The HistoryEntry object to check.</param>
        /// <returns>True if the the entries are equal. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/09/08 RCG 2.10.00        Created

        public bool Equals(HistoryEntry other)
        {
            return HistoryCode == other.HistoryCode;
        }

        /// <summary>
        /// Translates the meter's event code into the string that would be reported
        /// by the Collection Engine to the MDM
        /// </summary>
        /// <returns>A string representation of the event</returns>
        //  Revision History	
        //  MM/DD/YY Who  Version Issue#     Description
        //  -------- ---  ------- ------     -------------------------------------------
        //  10/09/09 AF   2.30.08            Created for CRF event files
        //  10/14/09 AF   2.30.09            Replaced LOSS_VOLTAGE_A with its new name
        //  11/06/09 AF   2.30.16            Removed irrelevant comments
        //  08/11/11 jrf  2.52.02 TREQ2709   Changing Register Firmware Download Status event
        //                                      to Firmware Download Event Log Full event.
        //  03/22/12 jrf  2.53.51 TREQ3442   Adding new voltage monitoring threshold events.
        //  09/20/12 jrf  2.70.18 TQ6658     Added magnetic tamper events.
        //  06/13/13 AF   2.80.37 TR7477     Added current threshold exceeded
        //  09/12/13 AF   2.85.42 WR412945   Added the ICS events
        //  04/07/14 AF   3.50.67 WR488012   Changed the event name of ICS events 1556 and 1674 to Inversion Tamper and Removal Tamper
        //                                      so that they will be recognized by IEE and updated some event names to match the CE.
        //  05/30/14 jrf  3.50.97 517744     The name of mfg. event 163 changed from HAN Load Control Event Sent to ICS ERT Event.
        //  04/13/16 PGH  4.50.245 670053    Added On Demand Periodic Read
        //  04/13/16 PGH  4.50.245 670073    Return "Replay Attack" for CANSIDevice.HistoryEvents.SECURITY_EVENT
        //  05/04/16 PGH  4.50.262 680182    Added the ICS events and Temperature events
        //  05/03/16 CFB  4.50.260 WR663771  Return "Time Changed (new time)" for CANSIDevice.HistoryEvents.CLOCK_RESET
        //  05/12/16 MP   4.50.266 WR685323  Changed ON_DEMAND_PERIOD_READ to ON_DEMAND_PERIODIC_READ to match ANSIDeviceStrings resource file.
        //  05/31/16 MP   4.50.274           Changed ICS Event descriptions to match CE
        //  06/01/16 MP   4.50.276 WR690650  Changed "Load Voltage Present during Reconnect" to "Load Voltage Present" 
        //  06/15/16 MP   4.50.284 WR680128  Added Critical peak pricing and extended outage recovery events.
        //  07/01/16 MP   4.50.294 696985    Added description intercept
        //  07/12/16 MP   4.70.7   WR688986  Changed how event descriptions were accessed
        public String TranslateEventCodeForMDM()
        {
            ResourceManager EventStringResourceManager = new ResourceManager("Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings",
                this.GetType().Assembly);

            String strEvent = "";
            uint ICSOffset = 0xE00;

            // Pull description from resource file
            if (m_uiHistoryCode < ICSOffset)
            {
                strEvent = EventStringResourceManager.GetString(Enum.GetName(typeof(CANSIDevice.HistoryEvents), 
                    (CANSIDevice.HistoryEvents)m_uiHistoryCode));

                // if we have generic event then intercept description. May have to be expanded eventually.
                if (m_uiHistoryCode == (ushort)CANSIDevice.HistoryEvents.EVENT_GENERIC_HISTORY_EVENT)
                    InterceptEventDescription(ref strEvent);
            }
            else
            {          
                strEvent = EventStringResourceManager.GetString(Enum.GetName(typeof(ICS_Gateway_EventDictionary.CommModuleHistoryEvents), 
                    (ICS_Gateway_EventDictionary.CommModuleHistoryEvents)m_uiHistoryCode));
            }

            return strEvent;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Date and Time of the History Event
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public DateTime HistoryTime
        {
            get
            {
                if (true == m_bHistDateTimeFlag)
                {
                    // This property is supported, so we can get it
                     return m_dtHistoryTime ;
                }
                else
                {
                    throw(new Exception("History Time is not supported for retrieval"));
                }
            }
            set
            {
                if (true == m_bHistDateTimeFlag)
                {
                    // This property is supported, so we can set it
                    m_dtHistoryTime = value;
                }
                else
                {
                    throw(new Exception("History Time is not supported for setting"));
                }
            }
        }

        /// <summary>
        /// Event Number of the History Event
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public UInt16 EventNumber
        {
            get 
            {
                if (true == m_bEventNbrFlag)
                {
                    // This value is supported so we can retreive it
                    return m_uiEventNumber;
                }
                else
                {
                    throw (new Exception("Event Number is not supported for retrieval"));
                }
            }
            set 
            {
                if (true == m_bEventNbrFlag)
                {
                    // Value is supported so we can set it
                    m_uiEventNumber = value;
                }
                else
                {
                    throw (new Exception("History Time is not supported for setting"));
                }
            }
        }

        /// <summary>
        /// Event Description of the History Event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version   Issue#    Description
        //  -------- --- -------   ------    ---------------------------------------
        //  09/19/06 KRC 7.35.00   N/A       Created
        //  02/16/12 jrf 2.53.41   TREQ5571  Modified to get event description through method call to dictionary.
        //  03/15/12 jrf 2.53.50   TREQ5571  Undoing previous change.
        //  06/20/16 PGH 4.50.289  680128    Don't add history code to description for unknown event
        //  07/01/16 MP  4.50.294  696985    Added description intercept
        //  07/21/16 MP  4.70.9    WR702277  Added check for std/mfg id if current one doesn't work
        //  07/29/16 MP   4.70.11  WR702277  Removed check made in previous revision

        public String EventDescription
        {
            get
            {
                String strEventDescription = "Unknown Event";
                if (m_dicEventDescriptions.ContainsKey((int)m_uiHistoryCode))
                {
                    strEventDescription = m_dicEventDescriptions[(int)m_uiHistoryCode];
                    if (m_uiHistoryCode == (ushort)(CANSIDevice.HistoryEvents.EVENT_GENERIC_HISTORY_EVENT))
                    {
                        InterceptEventDescription(ref strEventDescription);
                    }
                }
                else
                {
                    strEventDescription = "Unknown Event";
                }

                return strEventDescription;
            }
        }

        /// <summary>
        /// Sequence number of the History Event
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public UInt16 HistorySequenceNumber
        {
            get
            {
                if (true == m_bHistSeqNbrFlag)
                {
                    // value is supported so we can retreive it.
                    return m_uiHistorySeqNbr;
                }
                else
                {
                    throw (new Exception("History Sequence Number is not supported for retrieval"));
                }
            }
            set
            {
                if (true == m_bHistSeqNbrFlag)
                {
                    // value is supported so we can set it.
                    m_uiHistorySeqNbr = value;
                }
                else
                {
                    throw (new Exception("History Sequence Number is not supported for setting"));
                }
            }
        }

        /// <summary>
        /// User ID of the History Event
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public UInt16 UserID
        {
            get
            {
                return m_uiUserID;
            }
            set
            {
                m_uiUserID = value;
            }
        }

        /// <summary>
        /// History Code of the History Event
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public UInt16 HistoryCode
        {
            get
            {
                return m_uiHistoryCode;
            }
            set
            {
                m_uiHistoryCode = value;
            }
        }

        /// <summary>
        /// History Argument of the History Event
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/18/06 KRC 7.35.00 N/A    Created
        ///
        public byte[] HistoryArgument
        {
            get
            {
                return m_abytHistoryArgument;
            }
            set
            {
                m_abytHistoryArgument = value;
            }
        }

        /// <summary>
        /// The event number that is logged.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/10/08 jrf 10.00.00 N/A    Created
        ///
        public UInt16 TableProcedureNumber  
        {
            get
            {
                return (UInt16)(m_uiHistoryCode & TBL_PROC_MASK);
            }
        }

        /// <summary>
        /// Whether the event number is standard or manufacturer defined.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/10/08 jrf 10.00.00 N/A    Created
        ///
        public bool StdVsMfgFlag
        {
            get
            {
                if (0 == (m_uiHistoryCode & STD_V_MFG_MASK))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// The selector.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/10/08 jrf 10.00.00 N/A    Created
        ///
        public byte Selector
        {
            get
            {
                UInt16 uiSelector = (UInt16)(m_uiHistoryCode & SELECTOR_MASK);
                return (byte)(uiSelector >> 12);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Intercept event descriptions for events that change description based on sub-event ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version   Issue# Description
        //  -------- --- -------   ------ ---------------------------------------
        //  06/30/16 MP  4.50.294  696985 Created.
        //  08/19/16 jrf 4.70.13   TC 63268  Added description for new sub event Var Q2 Q3 enabled.
        //  09/01/16 jrf 4.70.16   WI 708332 Corrected description for new subevent (9) to match CE description.
        private void InterceptEventDescription(ref string Description)
        {
            switch (m_uiHistoryCode)
            {
                case (ushort)(CANSIDevice.HistoryEvents.EVENT_GENERIC_HISTORY_EVENT):
                    {
                        switch (m_abytHistoryArgument[0])
                        {
                            case 0x01:
                                {
                                    Description = "Firmware Activation Canceled";
                                    break;
                                }
                            case 0x02:
                                {
                                    Description = "Performed Firmware CRC";
                                    break;
                                }
                            case 0x03:
                                {
                                    Description = "IPv6 Comm Module Upgrade";
                                    break;
                                }
                            case 0x04:
                                {
                                    Description = "IPv6 Comm Module SPPP Connection";
                                    break;
                                }
                            case 0x05:
                                {
                                    Description = "IPv6 Comm Module Sync";
                                    break;
                                }
                            case 0x06:
                                {
                                    Description = "802.1x State Change";
                                    break;
                                }
                            case 0x07:
                                {
                                    Description = "IPv6 Link Local PSK Generated";
                                    break;
                                }
                            case 0x08:
                                {
                                    Description = "MSM State Change";
                                    break;
                                }
                            case 0x09:
                                {
                                    Description = "Energy Quantity Change";
                                    break;
                                }
                            default:
                                {
                                    break; // No change to description
                                }
                        }

                        break;
                    }
                // Add new sections for Events with special cases. AFAIK, Event 245 (Generic History Event) is the only
                // one that changes its description depending on its sub-event ID.
                default:
                    {
                        break;
                    }
            }
        }

        #endregion

        #region Members

        private bool m_bHistDateTimeFlag;
        private bool m_bEventNbrFlag;
        private bool m_bHistSeqNbrFlag;
        private uint m_uiHistDataLength;

        private DateTime m_dtHistoryTime;
        private UInt16 m_uiEventNumber;
        private UInt16 m_uiHistorySeqNbr;
        private UInt16 m_uiUserID;
        private UInt16 m_uiHistoryCode;
        private byte[] m_abytHistoryArgument = null;
        private ANSIEventDictionary m_dicEventDescriptions;

        #endregion

    }

    /// <summary>
    /// Class representing a single entry in standard table 73 - History Logger Control
    /// </summary>
    public class EventEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/25/10 AF  2.41.04         Created
        //
        public EventEntry()
        {
            m_uiHistoryCode = 0;
            m_blnMonitored = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event id of the event
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/25/10 AF  2.41.04         Created
        public UInt16 HistoryCode
        {
            get
            {
                return m_uiHistoryCode;
            }
            set
            {
                m_uiHistoryCode = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the event is configured in the meter.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/25/10 AF  2.41.04         Created
        public bool Monitored
        {
            get
            {
                return m_blnMonitored;
            }
            set
            {
                m_blnMonitored = value;
            }
        }

        #endregion

        #region Members

        private UInt16 m_uiHistoryCode;
        private bool m_blnMonitored;

        #endregion
    }
}
