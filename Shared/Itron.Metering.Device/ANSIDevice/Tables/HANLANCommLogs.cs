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
//                           Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.IO;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class represents MFG Table 2159, which is the
    /// Communications Actual Log Limiting Table
    /// </summary>
    public class MFGTable2159 : AnsiTable
    {
        #region Constants

        private const int COMM_ACTUAL_LIMITING_TABLE_SIZE = 9;
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for Tabel 2159 Object
        /// </summary>
        /// <param name="BinaryReader">A binary Reader contain the stream for 2159</param>
        public MFGTable2159(PSEMBinaryReader BinaryReader)
            : base(2159, COMM_ACTUAL_LIMITING_TABLE_SIZE)
        {
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads table 2159 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable2159.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Returns the Event Number Flag
        /// </summary>
        /// <returns>bool</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public bool EventNumberFlag
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_blnEventNumberFlag;
            }
        }

        /// <summary>
        /// Returns the Number of MFG Events
        /// </summary>
        /// <returns>byte</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public byte NumberMFGEvents
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_byNbrMFGEvents;
            }
        }

        /// <summary>
        /// Returns the Number of Standard Events
        /// </summary>
        /// <returns>byte</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public byte NumberSTDEvents
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_byNbrSTDEvents;
            }
        }

        /// <summary>
        /// Returns the LAN Data Length
        /// </summary>
        /// <returns>uint</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public uint LANDataLength
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return (uint)m_byLANDataLength;
            }
        }

        /// <summary>
        /// Returns the HAN Data Length
        /// </summary>
        /// <returns>uint</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public uint HANDataLength
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return (uint)m_byHANDataLength;
            }
        }

        /// <summary>
        /// Returns the Number of LAN Entries
        /// </summary>
        /// <returns>short</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public ushort NumberLANEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_usNbrLANEntries;
            }
        }

        /// <summary>
        /// Returns the Number of Standard Events
        /// </summary>
        /// <returns>short</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public ushort NumberHANEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_usNbrHANEntries;
            }
        }

        /// <summary>
        /// This property calculates the size of the LAN Log Record to help us when contructing Table 2162
        /// </summary>
        public uint SizeOfLANLogRecord
        {
            get
            {
                // First calculate the Size of a LAN Entry
                //  LAN Entry always has 9 bytes then an additional two if the Event Number Flag is set.
                //  Finally we add the LAN Data Length
                uint iSizeofLANEntry = 9 + LANDataLength;
                if (EventNumberFlag)
                {
                    iSizeofLANEntry += 2;
                }

                // The LAN Record starts at 11 and then contains an array of LAN Entries.  The number of
                //  entries is defined as NumberLANEntries
                uint iSizeofLANLogRecord = 11 + (NumberLANEntries + iSizeofLANEntry);

                return iSizeofLANLogRecord;
            }
        }

        /// <summary>
        /// This property calculates the size of the HAN Log Record to help us when contructing Table 2164
        /// </summary>
        public uint SizeOfHANLogRecord
        {
            get
            {
                // First calculate the Size of a GAN Entry
                //  LAN Entry always has 9 bytes then an additional two if the Event Number Flag is set.
                //  Finally we add the LAN Data Length
                uint iSizeofHANEntry = 9 + HANDataLength;
                if (EventNumberFlag)
                {
                    iSizeofHANEntry += 2;
                }

                // The LAN Record starts at 11 and then contains an array of LAN Entries.  The number of
                //  entries is defined as NumberLANEntries
                uint iSizeofHANLogRecord = 11 + (NumberHANEntries + iSizeofHANEntry);

                return iSizeofHANLogRecord;
            }
        }

        /// <summary>
        /// Calculate the size of the LAN Control Table
        /// </summary>
        public uint SizeofControlTable
        {
            get
            {
                return (uint)(NumberMFGEvents + NumberSTDEvents);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        private void ParseData()
        {
            // Populate the member variables that represent the table
            byte bytLogFlagBfld = m_Reader.ReadByte();
            // Mask off the various fields in the bit field
            m_blnEventNumberFlag = (0 != (bytLogFlagBfld & 0x01));
            m_blnLANInhibitOverflowFlag = (0 != (bytLogFlagBfld & 0x02));
            m_blnHANInhibitOverflowFlag = (0 != (bytLogFlagBfld & 0x04));
            m_blnLANListTypeFlag = (0 != (bytLogFlagBfld & 0x08));
            m_blnHANListTypeFlag = (0 != (bytLogFlagBfld & 0x10));

            m_byNbrSTDEvents = m_Reader.ReadByte();
            m_byNbrMFGEvents = m_Reader.ReadByte();
            m_byLANDataLength = m_Reader.ReadByte();
            m_byHANDataLength = m_Reader.ReadByte();
            m_usNbrLANEntries = m_Reader.ReadUInt16();
            m_usNbrHANEntries = m_Reader.ReadUInt16();
        }

        #endregion Private Method

        #region Members

        private bool m_blnEventNumberFlag;
        private bool m_blnLANInhibitOverflowFlag;
        private bool m_blnHANInhibitOverflowFlag;
        private bool m_blnLANListTypeFlag;
        private bool m_blnHANListTypeFlag;
        private byte m_byNbrSTDEvents;
        private byte m_byNbrMFGEvents;
        private byte m_byLANDataLength;
        private byte m_byHANDataLength;
        private ushort m_usNbrLANEntries;
        private ushort m_usNbrHANEntries;

        #endregion Members

    }

    /// <summary>
    /// This class represents MFG Table 2160, which is the
    /// Communications Events Identifcation Table
    /// </summary>
    public class MFGTable2160 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor for the 2160 - Communications Events Identification Table
        /// </summary>
        /// <param name="BinaryReader">Binary Reader containing Data Stream for table 2161</param>
        /// <param name="tbl2159">Table 2159 Object</param>
        public MFGTable2160(PSEMBinaryReader BinaryReader, MFGTable2159 tbl2159)
            : base(2160, tbl2159.SizeofControlTable)
        {
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            m_Table2159 = tbl2159;
            m_StdEventsSupported = new List<CommLogEvent>();
            m_MfgEventsSupported = new List<CommLogEvent>();
            ParseData();
        }

        /// <summary>
        /// Reads table 2161 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable2160.Read");

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
        /// Property that tells us which standard Events the meter supports
        /// </summary>
        public List<CommLogEvent> StdEventSupported
        {
            get
            {
                bool bResult = false;
                int iIndex;
                byte byBitMask;
                int iMaxNumStdEvents = 8 * m_Table2159.NumberSTDEvents - 1;

                for (int iStdEvent = 0; iStdEvent <= iMaxNumStdEvents; iStdEvent++)
                {
                    // Determine which index the table is in
                    iIndex = iStdEvent / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (iStdEvent % 8));

                    bResult = (byBitMask == (m_abyStdEventsUsed[iIndex] & byBitMask));

                    if (bResult)
                    {
                        CommLogEvent STDEvent = new CommLogEvent(iStdEvent);
                        m_StdEventsSupported.Add(STDEvent);
                    }
                }

                return m_StdEventsSupported;
            }
        }

        /// <summary>
        /// Property that tells us which standard Events the meter supports
        /// </summary>
        public List<CommLogEvent> MfgEventSupported
        {
            get
            {
                bool bResult = false;
                int iIndex;
                byte byBitMask;
                int iMaxNumMfgEvents = 8 * m_Table2159.NumberMFGEvents - 1;

                for (int iMfgEvent = 0; iMfgEvent <= iMaxNumMfgEvents; iMfgEvent++)
                {
                    // Determine which index the table is in
                    iIndex = iMfgEvent / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (iMfgEvent % 8));

                    bResult = (byBitMask == (m_abyMfgEventsUsed[iIndex] & byBitMask));

                    if (bResult)
                    {
                        CommLogEvent MFGEvent = new CommLogEvent(2048 + iMfgEvent);
                        m_MfgEventsSupported.Add(MFGEvent);
                    }
                }

                return m_MfgEventsSupported;
            }
        }

        #endregion

        #region Private Methods

        private void ParseData()
        {
            // Populate the member variables that represent the table
            m_abyStdEventsUsed = m_Reader.ReadBytes((int)m_Table2159.NumberSTDEvents);
            m_abyMfgEventsUsed = m_Reader.ReadBytes((int)m_Table2159.NumberMFGEvents);
        }

        #endregion

        #region Members

        private MFGTable2159 m_Table2159;
        private byte[] m_abyStdEventsUsed;
        private byte[] m_abyMfgEventsUsed;
        List<CommLogEvent> m_StdEventsSupported;
        List<CommLogEvent> m_MfgEventsSupported;

        #endregion
    }

    /// <summary>
    /// This class represetns MFG Table 2161, which is the 
    /// LAN Communications Log Control Table
    /// </summary>
    public class MFGTable2161 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor for the 2161 - LAN Communications Log Control table
        /// </summary>
        /// <param name="BinaryReader">Binary Reader containing Data Stream for table 2161</param>
        /// <param name="tbl2159">Table 2159 Object</param>
        public MFGTable2161(PSEMBinaryReader BinaryReader, MFGTable2159 tbl2159)
            : base(2161, tbl2159.SizeofControlTable)
        {
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            m_Table2159 = tbl2159;
            m_StdEventsMonitored = new List<CommLogEvent>();
            m_MfgEventsMonitored = new List<CommLogEvent>();
            ParseData();
        }

        /// <summary>
        /// Reads table 2161 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable2161.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Property that returns the list of Monitored LAN MFG Events
        /// </summary>
        public List<CommLogEvent> StdEventsMonitored
        {
           get
           {
                bool bResult = false;
                int iIndex;
                byte byBitMask;
                int iMaxNumStdEvents = 8 * m_Table2159.NumberSTDEvents - 1;

                for (int iStdEvent = 0; iStdEvent <= iMaxNumStdEvents; iStdEvent++)
                {
                    // Determine which index the table is in
                    iIndex = iStdEvent / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (iStdEvent % 8));

                    bResult = (byBitMask == (m_abyStdEventsMonitored[iIndex] & byBitMask));

                    if (bResult)
                    {
                        CommLogEvent STDEvent = new CommLogEvent(iStdEvent);
                        m_StdEventsMonitored.Add(STDEvent);
                    }
                }

                return m_StdEventsMonitored;
            }
        }

        /// <summary>
        /// Property that returns the list of Monitored LAN MFG Events
        /// </summary>
        public List<CommLogEvent> MfgEventsMonitored
        {
            get
            {
                bool bResult = false;
                int iIndex;
                byte byBitMask;
                int iMaxNumMfgEvents = 8 * m_Table2159.NumberMFGEvents - 1;

                for (int iMfgEvent = 0; iMfgEvent <= iMaxNumMfgEvents; iMfgEvent++)
                {
                    // Determine which index the table is in
                    iIndex = iMfgEvent / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (iMfgEvent % 8));

                    bResult = (byBitMask == (m_abyMfgEventsMonitored[iIndex] & byBitMask));

                    if (bResult)
                    {
                        CommLogEvent MFGEvent = new CommLogEvent(2048 + iMfgEvent);
                        m_MfgEventsMonitored.Add(MFGEvent);
                    }
                }



                return m_MfgEventsMonitored;
            }
        }

        #endregion

        #region Private Methods

        private void ParseData()
        {
            // Populate the member variables that represent the table
            m_abyStdEventsMonitored = m_Reader.ReadBytes((int)m_Table2159.NumberSTDEvents);
            m_abyMfgEventsMonitored = m_Reader.ReadBytes((int)m_Table2159.NumberMFGEvents);
        }

        #endregion

        #region Members

        private MFGTable2159 m_Table2159;
        private byte[] m_abyStdEventsMonitored;
        private byte[] m_abyMfgEventsMonitored;
        List<CommLogEvent> m_StdEventsMonitored;
        List<CommLogEvent> m_MfgEventsMonitored;

        #endregion
    }

    /// <summary>
    /// This class represents MFG Table 2162, which is the 
    /// LAN Log Data Table
    /// </summary>
    public class MFGTable2162 : AnsiTable
    {
        #region Constants
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for Tabel 2162 Object
        /// </summary>
        /// <param name="BinaryReader">A binary Reader contain the stream for 2162</param>
        /// <param name="tbl2159">The Actual Limiting Table for HAN/LAN logging</param>
        public MFGTable2162(PSEMBinaryReader BinaryReader, MFGTable2159 tbl2159)
            : base(2162, tbl2159.SizeOfLANLogRecord)
        {
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            m_Table2159 = tbl2159;
            ParseData();
        }

        /// <summary>
        /// Reads table 2162 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable2162.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Returns the Number of Valid LAN Entries in the list
        /// </summary>
        public ushort NumberValidLANEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_usNbrValidEntries;
            }
        }

        /// <summary>
        /// Returns the Sequence number of the last valid entry
        /// </summary>
        public long LastEntrySequenceNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_lLastEntrySequenceNumber;
            }
        }

        /// <summary>
        /// Returns the list of LAN Entries
        /// </summary>
        public List<LANEntry> LANEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_LANEntries;
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        private void ParseData()
        {
            int iActualStartIndex;
            LANEntry[] Entries = new LANEntry[0];

            // Populate the member variables that represent the table
            m_byLANFlags = m_Reader.ReadByte();
            m_usNbrValidEntries = m_Reader.ReadUInt16();
            m_usLastEntryElement = m_Reader.ReadUInt16();
            m_lLastEntrySequenceNumber = m_Reader.ReadUInt32();
            m_usUnreadEntries = m_Reader.ReadUInt16();

            if (m_usNbrValidEntries > 0)
            {
                // Since this is a circular list determine the actual starting index
                iActualStartIndex = (m_usLastEntryElement + 1) % m_usNbrValidEntries;

                Entries = new LANEntry[m_usNbrValidEntries];

                // Loop through the LAN Entires populating the List
                for (int iIndex = 0; iIndex < m_usNbrValidEntries; iIndex++)
                {
                    LANEntry Entry = new LANEntry();
                    Entry.LANEntryTime = m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME);
                    if (m_Table2159.EventNumberFlag)
                    {
                        Entry.LANEntryNumber = m_Reader.ReadUInt16();
                    }
                    Entry.LANUserID = m_Reader.ReadUInt16();
                    Entry.LANCode = m_Reader.ReadUInt16();
                    Entry.LANArgument = m_Reader.ReadBytes((int)m_Table2159.LANDataLength);

                    // Since index 0 in the meter may not be the same as the actual index 0 in a circular list 
                    // we need to determine where we really are in the list. Given any index the 
                    // actual index should be (count - actual start + index) % count. This will give us the list
                    // of events in the correct order.
                    Entries[(m_usNbrValidEntries - iActualStartIndex + iIndex) % m_usNbrValidEntries] = Entry;
                }
            }

            m_LANEntries = new List<LANEntry>(Entries);
        }

        #endregion Private Method

        #region Members

        private MFGTable2159 m_Table2159;
        private byte m_byLANFlags;
        private ushort m_usNbrValidEntries;
        private ushort m_usLastEntryElement;
        private long m_lLastEntrySequenceNumber;
        private ushort m_usUnreadEntries;
        private List<LANEntry> m_LANEntries;

        #endregion
    }

    /// <summary>
    /// This class represents MFG Table 2163, which is the
    /// HAN Communications Log Control Table
    /// </summary>
    public class MFGTable2163 : AnsiTable
        {
        #region Public Methods

        /// <summary>
        /// Constructor for the 2163 - HAN Communications Log Control table
        /// </summary>
        /// <param name="BinaryReader">Binary Reader containing Data Stream for table 2161</param>
        /// <param name="tbl2159">Table 2159 Object</param>
        public MFGTable2163(PSEMBinaryReader BinaryReader, MFGTable2159 tbl2159)
            : base(2163, tbl2159.SizeofControlTable)
        {
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            m_Table2159 = tbl2159;
            m_StdEventsMonitored = new List<CommLogEvent>();
            m_MfgEventsMonitored = new List<CommLogEvent>();
            ParseData();
        }

        /// <summary>
        /// Reads table 2161 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable2163.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Property that returns the list of Monitored LAN MFG Events
        /// </summary>
        public List<CommLogEvent> StdEventsMonitored
        {
            get
            {
                bool bResult = false;
                int iIndex;
                byte byBitMask;
                int iMaxNumStdEvents = 8 * m_Table2159.NumberSTDEvents - 1;

                for (int iStdEvent = 0; iStdEvent <= iMaxNumStdEvents; iStdEvent++)
                {
                    // Determine which index the table is in
                    iIndex = iStdEvent / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (iStdEvent % 8));

                    bResult = (byBitMask == (m_abyStdEventsMonitored[iIndex] & byBitMask));

                    if (bResult)
                    {
                        CommLogEvent STDEvent = new CommLogEvent(iStdEvent);
                        m_StdEventsMonitored.Add(STDEvent);
                    }
                }

                return m_StdEventsMonitored;
            }
        }

        /// <summary>
        /// Property that returns the list of Monitored HAN MFG Events
        /// </summary>
        public List<CommLogEvent> MfgEventsMonitored
        {
            get
            {
                bool bResult = false;
                int iIndex;
                byte byBitMask;
                int iMaxNumMfgEvents = 8 * m_Table2159.NumberMFGEvents - 1;

                for (int iMfgEvent = 0; iMfgEvent <= iMaxNumMfgEvents; iMfgEvent++)
                {
                    // Determine which index the table is in
                    iIndex = iMfgEvent / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (iMfgEvent % 8));

                    bResult = (byBitMask == (m_abyMfgEventsMonitored[iIndex] & byBitMask));

                    if (bResult)
                    {
                        CommLogEvent MFGEvent = new CommLogEvent(2048 + iMfgEvent);
                        m_MfgEventsMonitored.Add(MFGEvent);
                    }
                }

                return m_MfgEventsMonitored;
            }
        }

        #endregion

        #region Private Methods

        private void ParseData()
        {
            // Populate the member variables that represent the table
            m_abyStdEventsMonitored = m_Reader.ReadBytes((int)m_Table2159.NumberSTDEvents);
            m_abyMfgEventsMonitored = m_Reader.ReadBytes((int)m_Table2159.NumberMFGEvents);
        }

        #endregion

        #region Members

        private MFGTable2159 m_Table2159;
        private byte[] m_abyStdEventsMonitored;
        private byte[] m_abyMfgEventsMonitored;
        List<CommLogEvent> m_StdEventsMonitored;
        List<CommLogEvent> m_MfgEventsMonitored;
        #endregion
    }

    /// <summary>
    /// This class represents MFG Table 2164, which is the
    /// HAN Log Data Table
    /// </summary>
    public class MFGTable2164 : AnsiTable
    {
        #region Constants
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for Tabel 2164 Object
        /// </summary>
        /// <param name="BinaryReader">A binary Reader contain the stream for 2164</param>
        /// <param name="tbl2159">The Actual Limiting Table for HAN/LAN logging</param>
        public MFGTable2164(PSEMBinaryReader BinaryReader, MFGTable2159 tbl2159)
            : base(2164, tbl2159.SizeOfHANLogRecord)
        {
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            m_Table2159 = tbl2159;
            ParseData();
        }

        /// <summary>
        /// Reads table 2162 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/08 KRC 1.50.23
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable2162.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Returns the Number of Valid HAN Entries in the list
        /// </summary>
        public ushort NumberValidHANEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_usNbrValidEntries;
            }
        }

        /// <summary>
        /// Returns the Sequence number of the last valid entry
        /// </summary>
        public long LastEntrySequenceNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_lLastEntrySequenceNumber;
            }
        }

        /// <summary>
        /// Returns the list of HAN Entries
        /// </summary>
        public List<HANEntry> HANEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_HANEntries;
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        private void ParseData()
        {
            int iActualStartIndex;
            HANEntry[] Entries = new HANEntry[0];

            // Populate the member variables that represent the table
            m_byHANFlags = m_Reader.ReadByte();
            m_usNbrValidEntries = m_Reader.ReadUInt16();
            m_usLastEntryElement = m_Reader.ReadUInt16();
            m_lLastEntrySequenceNumber = m_Reader.ReadUInt32();
            m_usUnreadEntries = m_Reader.ReadUInt16();

            if (m_usNbrValidEntries > 0)
            {
                // Since this is a circular list determine the actual starting index
                iActualStartIndex = (m_usLastEntryElement + 1) % m_usNbrValidEntries;

                Entries = new HANEntry[m_usNbrValidEntries];

                // Loop through the HAN Entires populating the List
                for (int iIndex = 0; iIndex < m_usNbrValidEntries; iIndex++)
                {
                    HANEntry Entry = new HANEntry();
                    Entry.HANEntryTime = m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME);
                    if (m_Table2159.EventNumberFlag)
                    {
                        Entry.HANEntryNumber = m_Reader.ReadUInt16();
                    }
                    Entry.HANUserID = m_Reader.ReadUInt16();
                    Entry.HANCode = m_Reader.ReadUInt16();
                    Entry.HANArgument = m_Reader.ReadBytes((int)m_Table2159.HANDataLength);

                    // Since index 0 in the meter may not be the same as the actual index 0 in a circular list 
                    // we need to determine where we really are in the list. Given any index the 
                    // actual index should be (count - actual start + index) % count. This will give us the list
                    // of events in the correct order.
                    Entries[(m_usNbrValidEntries - iActualStartIndex + iIndex) % m_usNbrValidEntries] = Entry;
                }
            }

            m_HANEntries = new List<HANEntry>(Entries);
        }

        #endregion Private Method

        #region Members

        private MFGTable2159 m_Table2159;
        private byte m_byHANFlags;
        private ushort m_usNbrValidEntries;
        private ushort m_usLastEntryElement;
        private long m_lLastEntrySequenceNumber;
        private ushort m_usUnreadEntries;
        private List<HANEntry> m_HANEntries;

        #endregion
    }

    /// <summary>
    /// Base class for our Communication Events
    /// </summary>
    public class CommLogEvent : IEquatable<CommLogEvent>
    {
        /// <summary>
        /// Constructor for Communication Event
        /// </summary>
        /// <param name="iEventCode"></param>
        public CommLogEvent(int iEventCode)
        {
            m_iEventCode = iEventCode;
        }

        /// <summary>
        /// Property that returns the Event Code for a Communication Event
        /// </summary>
        public int CommEventCode
        {
            get
            {
                return m_iEventCode;
            }
        }

        /// <summary>
        /// Property to return the Description of the LAN Event
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/30/13 jrf 2.80.31 TQ8280 Adding a description for undefined events.
        //
        public string CommEventDescription
        {
            get
            {
                string strDescription = "";
                CENTRON_AMI_CommEventDictionary CommDictionary = new CENTRON_AMI_CommEventDictionary();
                if (CommDictionary.ContainsKey(CommEventCode))
                {
                    strDescription = CommDictionary[CommEventCode].ToString();
                }
                else
                {
                    System.Resources.ResourceManager rmStrings = new System.Resources.ResourceManager("Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings",
                                                               this.GetType().Assembly);
                    strDescription = rmStrings.GetString("UNKNOWN_EVENT") + " " + CommEventCode.ToString(CultureInfo.InvariantCulture);
                }

                return strDescription;
            }
        }

        /// <summary>
        /// Determines if the CommLogEvent object is equal to the current object
        /// </summary>
        /// <param name="other">The ZigBeeDevice object to compare</param>
        /// <returns>True if the objects are equal false otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public bool Equals(CommLogEvent other)
        {
            bool bIsEqual = false;

            if (CommEventCode == other.CommEventCode)
            {
                bIsEqual = true;
            }

            return bIsEqual;
        }

        private int m_iEventCode;

    }

  
    /// <summary>
    /// Class that contains the contents of a LAN Entry
    /// </summary>
    public class LANEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor for a LAN Entry
        /// </summary>
        public LANEntry()
        {
            m_CommEventDictionary = new CENTRON_AMI_CommEventDictionary();
        }

        /// <summary>
        /// Adds information to the event description based on the value of the
        /// history argument.  Initially, only loss of potential will be supported.
        /// </summary>
        /// <param name="uiLANEventCode">
        /// event code for the LAN Comm Event in question
        /// </param>
        /// <returns>
        /// Additional description based
        /// </returns>
        /// 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/13/08 KRC 1.50.24        returning translated Event data
        //
        public String TranslatedEventData(ushort uiLANEventCode)
        {
            MemoryStream TempStream = new MemoryStream(LANArgument);
            BinaryReader ArgumentReader = new BinaryReader(TempStream);
            return m_CommEventDictionary.TranslatedEventData(uiLANEventCode, ArgumentReader);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/Set the Time of the LAN Entry
        /// </summary>
        public DateTime LANEntryTime
        {
            set
            {
                m_dtEntryTime = value;
            }
            get
            {
                return m_dtEntryTime;
            }
        }

        /// <summary>
        /// Get/Set the Event Number (Common to both LAN and HAN Logs)
        /// </summary>
        public ushort LANEntryNumber
        {
            set
            {
                m_usEntryNumber = value;
            }
            get
            {
                return m_usEntryNumber;
            }
        }

        /// <summary>
        /// Get/Set the User ID associated with this LAN Log entry.
        /// </summary>
        public ushort LANUserID
        {
            set
            {
                m_usUserID = value;
            }
            get
            {
                return m_usUserID;
            }
        }

        /// <summary>
        /// Get/Set the LAN Event Code
        /// </summary>
        public ushort LANCode
        {
            set
            {
                m_usLANCode = value;
            }
            get
            {
                return m_usLANCode;
            }
        }

        /// <summary>
        /// Get the Text Description of the LAN Event
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/10/08 RCG	2.00.06	122581 Fixing issue with retrieving the description

        public string LANEventDescription
        {
            get
            {
                String strEventDescription;
                if (m_CommEventDictionary.ContainsKey((int)m_usLANCode))
                {
                    strEventDescription = m_CommEventDictionary[(int)m_usLANCode];
                }
                else
                {
                    strEventDescription = "Unknown LAN Event: " + m_usLANCode.ToString(CultureInfo.InvariantCulture);
                }

                return strEventDescription;
            }
        }

        /// <summary>
        /// Returns the Argument Data for the LAN Entry
        /// </summary>
        public byte[] LANArgument
        {
            set 
            {
                byarArgument = value;
            }
            get
            {
                return byarArgument;
            }
        }

        #endregion Public Properties

        #region Members

        private DateTime m_dtEntryTime;
        private ushort m_usEntryNumber;
        private ushort m_usUserID;
        private ushort m_usLANCode;
        private byte[] byarArgument;
        private CENTRON_AMI_CommEventDictionary m_CommEventDictionary;

        #endregion
    }

    /// <summary>
    /// Class Representing a HAN Entry
    /// </summary>
    public class HANEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor for a HAN Entry
        /// </summary>
        public HANEntry()
        {
            m_CommEventDictionary = new CENTRON_AMI_CommEventDictionary();
        }

        /// <summary>
        /// Adds information to the event description based on the value of the
        /// history argument.  Initially, only loss of potential will be supported.
        /// </summary>
        /// <param name="uiHANEventCode">
        /// event code for the HAN Comm Event in question
        /// </param>
        /// <returns>
        /// Additional description based
        /// </returns>
        /// 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/13/08 KRC 1.50.24        returning translated Event data
        //
        public String TranslatedEventData(ushort uiHANEventCode)
        {
            MemoryStream TempStream = new MemoryStream(HANArgument);
            BinaryReader ArgumentReader = new BinaryReader(TempStream);
            return m_CommEventDictionary.TranslatedEventData(uiHANEventCode, ArgumentReader);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/Set the Time of the HAN Entry
        /// </summary>
        public DateTime HANEntryTime
        {
            set
            {
                m_dtEntryTime = value;
            }
            get
            {
                return m_dtEntryTime;
            }
        }

        /// <summary>
        /// Get/Set the Event Number (Common to both LAN and HAN Logs)
        /// </summary>
        public ushort HANEntryNumber
        {
            set
            {
                m_usEntryNumber = value;
            }
            get
            {
                return m_usEntryNumber;
            }
        }

        /// <summary>
        /// Get/Set the User ID associated with this HAN Log entry.
        /// </summary>
        public ushort HANUserID
        {
            set
            {
                m_usUserID = value;
            }
            get
            {
                return m_usUserID;
            }
        }

        /// <summary>
        /// Get/Set the HAN Event Code
        /// </summary>
        public ushort HANCode
        {
            set
            {
                m_usHANCode = value;
            }
            get
            {
                return m_usHANCode;
            }
        }

        /// <summary>
        /// Get the Text Description of the HAN Event
        /// </summary>
        public string HANEventDescription
        {
            get
            {
                String strEventDescription;
                if (m_CommEventDictionary.ContainsKey((int)m_usHANCode))
                {
                    strEventDescription = m_CommEventDictionary[(int)m_usHANCode];
                }
                else
                {
                    strEventDescription = "Unknown HAN Event: " + m_usHANCode.ToString(CultureInfo.InvariantCulture);
                }

                return strEventDescription;
            }
        }

        /// <summary>
        /// Returns the Argument Data for the HAN Entry
        /// </summary>
        public byte[] HANArgument
        {
            set
            {
                byarArgument = value;
            }
            get
            {
                return byarArgument;
            }
        }

        #endregion Public Properties

        #region Members

        private DateTime m_dtEntryTime;
        private ushort m_usEntryNumber;
        private ushort m_usUserID;
        private ushort m_usHANCode;
        private byte[] byarArgument;
        private CENTRON_AMI_CommEventDictionary m_CommEventDictionary;

        #endregion
    }

}
