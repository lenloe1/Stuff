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
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// HAN reset methods
    /// </summary>
    public enum HANResetMethod : uint
    {
        /// <summary>
        /// User reset
        /// </summary>
        PerformReset = 0,
        /// <summary>
        /// Test 1 - At least one HAN device joined
        /// </summary>
        TestOneJoinedDevice = 1,
        /// <summary>
        /// Test 2 - Code detected reset
        /// </summary>
        TestCodeDetectedReset = 2,
        /// <summary>
        /// Test 3 - Reset from ZigBee Stack Task watchdog
        /// </summary>
        TestZigBeeStackTaskWatchdog = 3,
        /// <summary>
        /// Test 4 - Reset from ZigBee Task watchdog
        /// </summary>
        TestZigBeeTaskWatchdog = 4,
        /// <summary>
        /// Test 5 - Processor Fault reset
        /// </summary>
        TestProcessorFault = 5,
        /// <summary>
        /// Test 6 - Periodic Reset using Test 5 method
        /// </summary>
        TestPeriodicReset = 6,
        /// <summary>
        /// Test 7 - First Use Reset
        /// </summary>
        TestFirstUseReset = 7,
        /// <summary>
        /// Disable Tests
        /// </summary>
        DisableTest = 8,
        /// <summary>
        /// Clear Reset Limiting Halt Condition
        /// </summary>
        ClearResetLimitingHaltCondition = 0xF0,
        /// <summary>
        /// Clears the Reset Log
        /// </summary>
        ClearResetLog = 0xF1,
    }

    /// <summary>
    /// HAN Reset Types
    /// </summary>
    public enum HANResetType : byte
    {
        /// <summary>
        /// Normal Reset - By User
        /// </summary>
        [EnumDescription("Normal Reset")]
        NormalReset = 0,
        /// <summary>
        /// Detected a Fatal Error
        /// </summary>
        [EnumDescription("Detected Fatal Error")]
        DetectedFatalError = 1,
        /// <summary>
        /// Watchdog Reset
        /// </summary>
        [EnumDescription("Watchdog")]
        Watchdog = 2,
        /// <summary>
        /// Processor Core Fault
        /// </summary>
        [EnumDescription("Core Fault")]
        CoreFault = 3,
        /// <summary>
        /// ZigBee Stack Lockup
        /// </summary>
        [EnumDescription("Stack Lockup")]
        StackLockup = 4,
        /// <summary>
        /// First Used Reset
        /// </summary>
        [EnumDescription("First Used Reset")]
        FirstUsedReset = 5,
        /// <summary>
        /// Periodic Reset
        /// </summary>
        [EnumDescription("Periodic Reset")]
        PeriodicReset = 6,
        /// <summary>
        /// Caused whenever the HAN is disabled
        /// </summary>
        [EnumDescription("Disabled Zigbee")]
        DisableZigbee = 7,
        /// <summary>
        /// 
        /// </summary>
        [EnumDescription("Empty Periodic Reset")]
        EmptyPeroidicReset = 8,
        /// <summary>
        /// Used to halt on a critical bug where we want the HAN to halt even if not at reset limiting, but we don't want it to fatal error.
        /// </summary>
        [EnumDescription("Diagnostic Stop")]
        DiagnosticStop = 9,
        /// <summary>
        /// Used to restart the task for facilitating network restart (mostly for debug)
        /// </summary>
        [EnumDescription("Network Restart")]
        NetworkRestart = 0xA,
        /// <summary>
        /// Catch all
        /// </summary>
        HAN_RESET_TYPE_CODE_INVALID = 0xFF

    }

    /// <summary>
    /// MFG Table 2290 (242) - HAN Reset Log
    /// </summary>
    public class OpenWayMFGTable2290 : AnsiTable
    {
        #region Constants

        private const ushort TABLE_SIZE_PRE_LI = 20476;
        private const ushort TABLE_SIZE_LI = 20474;
        private const int TABLE_TIMEOUT = 10000;
        private const ushort BASIC_INFO_LENGTH = 25;
        private const double BASIC_INFO_TIMEOUT = 1.0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="firmwareVersion">The firmware version of the meter.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created
        //  12/06/11 RCG 2.53.20 		Fixing table size issue

        public OpenWayMFGTable2290(CPSEM psem, float firmwareVersion)
            : base(psem, 2290, GetTableSize(firmwareVersion), TABLE_TIMEOUT)
        {
            m_LastBasicInfoReadTime = DateTime.MinValue;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2290.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Formats the contents of the table into a string
        /// </summary>
        /// <returns></returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/11/11 RCG 2.53.05        Created

        public override string ToString()
        {
            string TableString = "";

            // Make sure we have read all of the data first
            Read();

            TableString += "MFG Table 242 (2290) - ZigBee Reset Info\r\n";

            TableString += "   Reset Statistics:\r\n";
            TableString += "      Total ZigBee Resets = " + TotalHANResets.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      Last Reset Time = " + LastResetTime.ToString("G", CultureInfo.CurrentCulture) + "\r\n";
            TableString += "      Total Code Fatals = " + TotalCodeFatalErrors.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      Total Watchdogs = " + TotalWatchdogErrors.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      Total Faults = " + TotalCoreFaults.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      Total Stack Lockups = " + TotalStackLockups.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      Total Initial Reg Table Add = " + TotalFirstUseResets.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      High Water Mark For Reset Limiting = " + HighWaterMark.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      Current Reset Limiting Period = " + CurrentResetLimitPeriod.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      Current Reset Limiting Count = " + CurrentResetLimitCount.ToString(CultureInfo.InvariantCulture) + "\r\n";
            TableString += "      Halt Due to Over Reset Limit = " + isHaltedDueToOverResetLimit.ToString(CultureInfo.InvariantCulture) + "\r\n";

            for (int Index = 0; Index < ValidEntrySequenceNumbers.Count(); Index++)
            {
                TableString += "   Valid Entry Sequence [" + Index.ToString(CultureInfo.InvariantCulture) + "] = " + ValidEntrySequenceNumbers[Index].ToString(CultureInfo.InvariantCulture) + "\r\n";
            }

            for (int Index = 0; Index < ResetLogEntries.Count(); Index++)
            {
                TableString += "   Entries [" + Index.ToString(CultureInfo.InvariantCulture) + "]\r\n";
                TableString += ResetLogEntries[Index].ToString();
            }

            return TableString;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the total number of HAN Resets
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public uint TotalHANResets
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_uiTotalHANResets;
            }
        }

        /// <summary>
        /// Gets the date and time of the last reset
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public DateTime LastResetTime
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_LastResetTime;
            }
        }

        /// <summary>
        /// Gets the total number of code detectable errors
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public ushort TotalCodeFatalErrors
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_usTotalCodeFatals;
            }
        }

        /// <summary>
        /// Gets the total number of watchdog errors
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public ushort TotalWatchdogErrors
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_usTotalWatchdogs;
            }
        }

        /// <summary>
        /// Gets the total number of processor faults
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public ushort TotalCoreFaults
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_usTotalProcessorFaults;
            }
        }

        /// <summary>
        /// Gets the total number of stack lockups
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public ushort TotalStackLockups
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_usTotalStackLockups;
            }
        }

        /// <summary>
        /// Gets the total number of First Use Resets
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public ushort TotalFirstUseResets
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_usTotalInitialRegistrationTableAdds;
            }
        }

        /// <summary>
        /// Gets the High Mark For Reset Limiting
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 MSC 2.51.05        Created

        public ushort HighWaterMark
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_usHighWaterMarkForResetLimiting;
            }
        }

        /// <summary>
        /// Gets the Current Reset Limit Period
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 MSC 2.51.05        Created

        public ushort CurrentResetLimitPeriod
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_usCurrentResetLimitingPeriodSetting;
            }
        }

        /// <summary>
        /// Gets the Current Reset Limit Count
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 MSC 2.51.05        Created

        public ushort CurrentResetLimitCount
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_usCurrentResetLimitingCountSetting;
            }
        }

        /// <summary>
        /// Gets the Current Reset Limit Count
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 MSC 2.51.05        Created

        public byte OverResetLimit
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                return m_bHaltedDueToOverResetLimit;
            }
        }

        /// <summary>
        /// Gets the Current Reset Limit Count
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/08/11 MSC 2.51.05        Created
        //  03/17/16 PGH 4.50.237 602369 Check for not initialized

        public bool isHaltedDueToOverResetLimit
        {
            get
            {
                // We are much more likely to read the counts outside of the remainder of the log so we should do an offset read
                ReadBasicInformation();

                byte NotInitialized = 0xFF;

                if (m_bHaltedDueToOverResetLimit == 0 || m_bHaltedDueToOverResetLimit == NotInitialized)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Gets the Current value of Diagnostic Stop byte
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/28/13 MP  2.80.10        Created

        public bool isHaltedDueToDiagnosticStop
        {
            get
            {
                ReadBasicInformation();

                if (m_bHaltedDueToDiagnosticStop == 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the entry sequence numbers
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public byte[] ValidEntrySequenceNumbers
        {
            get
            {
                ReadUnloadedTable();

                return m_ValidEntrySequenceNumbers;
            }
        }

        /// <summary>
        /// Gets the list of HAN Reset log entries
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public List<HANResetLogEntry> ResetLogEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_HANResetLog;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the 
        /// </summary>
        /// <param name="version">The firmware version of the meter.</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/09/11 RCG 2.53.20        Created

        private static ushort GetTableSize(float version)
        {
            ushort TableSize = 0;

            if (VersionChecker.CompareTo(version, CENTRON_AMI.VERSION_LITHIUM_3_12) < 0)
            {
                TableSize = TABLE_SIZE_PRE_LI;
            }
            else
            {
                TableSize = TABLE_SIZE_LI;
            }

            return TableSize;
        }

        /// <summary>
        /// Reads just the basic information from the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/26/11 RCG 2.51.02        Created

        private void ReadBasicInformation()
        {
            // To reduce the number of offset reads we should keep track of the last read time
            if ((DateTime.Now - m_LastBasicInfoReadTime).TotalSeconds >= BASIC_INFO_TIMEOUT)
            {
                PSEMResponse Response = Read(0, BASIC_INFO_LENGTH);

                if (Response == PSEMResponse.Ok)
                {
                    ParseBasicInformation();
                    m_LastBasicInfoReadTime = DateTime.Now;
                }
                else
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "HAN Reset Counts");
                }
            }
        }

        /// <summary>
        /// Parses the data that was just read
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        private void ParseData()
        {
            ParseBasicInformation();

            // There is extra space here that needs to be read.
            m_Reader.ReadBytes(39);

            m_ValidEntrySequenceNumbers = m_Reader.ReadBytes(10);

            m_HANResetLog = new List<HANResetLogEntry>();

            // Read Each of the Entries
            for (int iIndex = 0; iIndex < 10; iIndex++)
            {
                HANResetLogEntry NewEntry = new HANResetLogEntry();
                NewEntry.ParseEntry(m_Reader, (PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);

                m_HANResetLog.Add(NewEntry);
            }
        }

        /// <summary>
        /// Parses the Basic Information from the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/26/11 RCG 2.51.02        Created

        private void ParseBasicInformation()
        {
            m_uiTotalHANResets = m_Reader.ReadUInt32();
            m_LastResetTime = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_usTotalCodeFatals = m_Reader.ReadUInt16();
            m_usTotalWatchdogs = m_Reader.ReadUInt16();
            m_usTotalProcessorFaults = m_Reader.ReadUInt16();
            m_usTotalStackLockups = m_Reader.ReadUInt16();
            m_usTotalInitialRegistrationTableAdds = m_Reader.ReadUInt16();
            m_usHighWaterMarkForResetLimiting = m_Reader.ReadUInt16();
            m_usCurrentResetLimitingPeriodSetting = m_Reader.ReadUInt16();
            m_usCurrentResetLimitingCountSetting = m_Reader.ReadUInt16();
            m_bHaltedDueToOverResetLimit = m_Reader.ReadByte();
            m_bHaltedDueToDiagnosticStop = m_Reader.ReadByte();

        }

        #endregion

        #region Member Variables

        private uint m_uiTotalHANResets;
        private DateTime m_LastResetTime;
        private ushort m_usTotalCodeFatals;
        private ushort m_usTotalWatchdogs;
        private ushort m_usTotalProcessorFaults;
        private ushort m_usTotalStackLockups;
        private ushort m_usTotalInitialRegistrationTableAdds;
        private ushort m_usHighWaterMarkForResetLimiting;
        private ushort m_usCurrentResetLimitingPeriodSetting;
        private ushort m_usCurrentResetLimitingCountSetting;
        private byte m_bHaltedDueToOverResetLimit;
        private byte m_bHaltedDueToDiagnosticStop;

        private byte[] m_ValidEntrySequenceNumbers;
        private List<HANResetLogEntry> m_HANResetLog;
        private DateTime m_LastBasicInfoReadTime;

        #endregion
    }

    /// <summary>
    /// HAN Reset Log Entry
    /// </summary>
    public class HANResetLogEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public HANResetLogEntry()
        {
        }

        /// <summary>
        /// Formats the contents of the Entry as a string
        /// </summary>
        /// <returns>The entry as a string</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/11/11 RCG 2.53.05        Created

        public override string ToString()
        {
            string EntryString = "";

            EntryString += "      Fatal Error Reset Type = " + EnumDescriptionRetriever.RetrieveDescription(ResetType) + "\r\n";
            EntryString += "      Fatal Error Subcode = " + SubCode.ToString(CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "      Current Task ID = " + m_CurrentTaskID.ToString(CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "      Reset State = " + m_ResetState.ToString(CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "      Reset Time = " + ResetTime.ToString("G", CultureInfo.CurrentCulture) + "\r\n";

            for (int Index = 0; Index < m_CondensedRegistrationTable.Count(); Index++)
            {
                EntryString += "      Registration Table Data [" + Index.ToString(CultureInfo.InvariantCulture) + "]\r\n";
                EntryString += m_CondensedRegistrationTable[Index].ToString();
            }

            for (int Index = 0; Index < m_ResetErrors.Count(); Index++)
            {
                EntryString += "      ZigBee Error Log Entry [" + Index.ToString(CultureInfo.InvariantCulture) + "]\r\n";
                EntryString += m_ResetErrors[Index].ToString();
            }

            for (int Index = 0; Index < m_ResetEvents.Count(); Index++)
            {
                EntryString += "      ZigBee Event Log Entry [" + Index.ToString(CultureInfo.InvariantCulture) + "]\r\n";
                EntryString += m_ResetEvents[Index].ToString();
            }

            EntryString += "      Error Specific Data = ";

            foreach (byte CurrentByte in m_SpecificErrorData)
            {
                EntryString += CurrentByte.ToString("X2", CultureInfo.InvariantCulture) + " ";
            }

            EntryString += "\r\n";

            EntryString += "      Mini Core Dump\r\n";
            EntryString += m_MiniCoreDump.ToString();

            return EntryString;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the HAN Reset Type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public HANResetType ResetType
        {
            get
            {
                return m_ResetType;
            }
        }

        /// <summary>
        /// Gets the error sub code
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public byte SubCode
        {
            get
            {
                return m_ErrorSubcode;
            }
        }

        /// <summary>
        /// Gets the reset time
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public DateTime ResetTime
        {
            get
            {
                return m_ResetTime;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Parses the Reset Log Entry
        /// </summary>
        /// <param name="reader">The binary reader that contains the data</param>
        /// <param name="timeFormat">The time format of the meter</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        internal void ParseEntry(PSEMBinaryReader reader, PSEMBinaryReader.TM_FORMAT timeFormat)
        {
            m_ResetType = (HANResetType)reader.ReadByte();
            m_ErrorSubcode = reader.ReadByte();
            m_CurrentTaskID = reader.ReadByte();
            m_ResetState = reader.ReadByte();
            m_ResetTime = reader.ReadSTIME(timeFormat);

            m_CondensedRegistrationTable = new List<CondensedRegistrationTableEntry>();

            for (int iIndex = 0; iIndex < 10; iIndex++)
            {
                CondensedRegistrationTableEntry NewEntry = new CondensedRegistrationTableEntry();
                NewEntry.ParseEntry(reader);

                m_CondensedRegistrationTable.Add(NewEntry);
            }

            m_ResetErrors = new List<ZigBeeErrorLogEntry>();

            for (int iIndex = 0; iIndex < 8; iIndex++)
            {
                ZigBeeErrorLogEntry NewEntry = new ZigBeeErrorLogEntry();
                NewEntry.ParseEntry(reader, timeFormat);

                m_ResetErrors.Add(NewEntry);
            }

            m_ResetEvents = new List<ZigBeeEventLogEntry>();

            for (int iIndex = 0; iIndex < 32; iIndex++)
            {
                ZigBeeEventLogEntry NewEntry = new ZigBeeEventLogEntry();
                NewEntry.ParseEntry(reader);

                m_ResetEvents.Add(NewEntry);
            }

            m_SpecificErrorData = reader.ReadBytes(256);

            m_MiniCoreDump = new ZigBeeMiniCoreDump();
            m_MiniCoreDump.Parse(reader);

            // There is a large amount of padding at the end of the entry that we should skip over
            reader.BaseStream.Seek(241 * sizeof(uint), System.IO.SeekOrigin.Current);
        }

        #endregion

        #region Member Variables

        private HANResetType m_ResetType;
        private byte m_ErrorSubcode;
        private byte m_CurrentTaskID;
        private byte m_ResetState;
        private DateTime m_ResetTime;
        private List<CondensedRegistrationTableEntry> m_CondensedRegistrationTable;
        private List<ZigBeeErrorLogEntry> m_ResetErrors;
        private List<ZigBeeEventLogEntry> m_ResetEvents;
        private byte[] m_SpecificErrorData;
        private ZigBeeMiniCoreDump m_MiniCoreDump;

        #endregion
    }

    /// <summary>
    /// Mini Core Dump for ZigBee
    /// </summary>
    public class ZigBeeMiniCoreDump
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public ZigBeeMiniCoreDump()
        {
        }

        /// <summary>
        /// Formats the contents of the Entry as a string
        /// </summary>
        /// <returns>The entry as a string</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/11/11 RCG 2.53.05        Created

        public override string ToString()
        {
            string MiniCoreDumpString = "";

            MiniCoreDumpString += "         CPU Frame\r\n";
            MiniCoreDumpString += m_CPUFrame.ToString();

            for (int Index = 0; Index < m_TaskStack.Count(); Index++)
            {
                MiniCoreDumpString += "         Task Stack [" + Index.ToString(CultureInfo.InvariantCulture) + "] = " + m_TaskStack[Index].ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            }

            return MiniCoreDumpString;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Parses the Mini Core dump from the table
        /// </summary>
        /// <param name="reader">The binary reader containing the table data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        internal void Parse(PSEMBinaryReader reader)
        {
            m_CPUFrame = new CPUFrame();
            m_CPUFrame.Parse(reader);

            m_TaskStack = new uint[32];

            for (int iIndex = 0; iIndex < 32; iIndex++)
            {
                m_TaskStack[iIndex] = reader.ReadUInt32();
            }
        }

        #endregion

        #region Member Variables

        private CPUFrame m_CPUFrame;
        private uint[] m_TaskStack;

        #endregion
    }

    /// <summary>
    /// ZigBee CPU Frame
    /// </summary>
    public class CPUFrame
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public CPUFrame()
        {
        }

        /// <summary>
        /// Formats the contents of the Entry as a string
        /// </summary>
        /// <returns>The entry as a string</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/11/11 RCG 2.53.05        Created

        public override string ToString()
        {
            string FrameString = "";

            FrameString += "            CPSR = " + m_CPSR.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            PKSNUM = " + m_PKSNUM.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R1 = " + m_R1.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R2 = " + m_R2.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R3 = " + m_R3.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R4 = " + m_R4.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R5 = " + m_R5.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R6 = " + m_R6.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R7 = " + m_R7.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R8 = " + m_R8.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R9 = " + m_R9.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R10 = " + m_R10.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R11 = " + m_R11.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R12 = " + m_R12.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            R13 = " + m_R13.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";
            FrameString += "            PC = " + m_PC.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";

            return FrameString;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Parses the CPU Frame
        /// </summary>
        /// <param name="reader">The binary reader containing the table data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        internal void Parse(PSEMBinaryReader reader)
        {
            m_CPSR = reader.ReadUInt32();
            m_PKSNUM = reader.ReadUInt32();
            m_R1 = reader.ReadUInt32();
            m_R2 = reader.ReadUInt32();
            m_R3 = reader.ReadUInt32();
            m_R4 = reader.ReadUInt32();
            m_R5 = reader.ReadUInt32();
            m_R6 = reader.ReadUInt32();
            m_R7 = reader.ReadUInt32();
            m_R8 = reader.ReadUInt32();
            m_R9 = reader.ReadUInt32();
            m_R10 = reader.ReadUInt32();
            m_R11 = reader.ReadUInt32();
            m_R12 = reader.ReadUInt32();
            m_R13 = reader.ReadUInt32();
            m_PC = reader.ReadUInt32();
        }

        #endregion

        #region Member Variables

        private uint m_CPSR;
        private uint m_PKSNUM;
        private uint m_R1;
        private uint m_R2;
        private uint m_R3;
        private uint m_R4;
        private uint m_R5;
        private uint m_R6;
        private uint m_R7;
        private uint m_R8;
        private uint m_R9;
        private uint m_R10;
        private uint m_R11;
        private uint m_R12;
        private uint m_R13;
        private uint m_PC;

        #endregion
    }

    /// <summary>
    /// ZigBee Error Log Entry
    /// </summary>
    public class ZigBeeErrorLogEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public ZigBeeErrorLogEntry()
        {
        }

        /// <summary>
        /// Formats the contents of the Entry as a string
        /// </summary>
        /// <returns>The entry as a string</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/11/11 RCG 2.53.05        Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString(System.String)")]
        public override string ToString()
        {
            string EntryString = "";

            EntryString += "         Time = " + m_ErrorTime.ToString("G") + "\r\n";
            EntryString += "         Error Code = " + m_usErrorCode.ToString(CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "         Generic Parameter 2 = " + m_usGenericParameter2.ToString("X4", CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "         Generic Parameter = " + m_uiGenericParameter.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";

            return EntryString;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Parses the Error Log entry
        /// </summary>
        /// <param name="reader">The binary reader containing the table data</param>
        /// <param name="timeFormat">The time format of the meter</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        internal void ParseEntry(PSEMBinaryReader reader, PSEMBinaryReader.TM_FORMAT timeFormat)
        {
            m_ErrorTime = reader.ReadSTIME(timeFormat);
            m_usErrorCode = reader.ReadUInt16();
            m_usGenericParameter2 = reader.ReadUInt16();
            m_uiGenericParameter = reader.ReadUInt32();
        }

        #endregion

        #region Member Variables

        private DateTime m_ErrorTime;
        private ushort m_usErrorCode;
        private ushort m_usGenericParameter2;
        private uint m_uiGenericParameter;

        #endregion
    }

    /// <summary>
    /// ZigBee Event Log Entry
    /// </summary>
    public class ZigBeeEventLogEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public ZigBeeEventLogEntry()
        {
        }

        /// <summary>
        /// Formats the contents of the Entry as a string
        /// </summary>
        /// <returns>The entry as a string</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/11/11 RCG 2.53.05        Created

        public override string ToString()
        {
            string EntryString = "";

            EntryString += "         Time Stamp = " + m_EventSeconds.ToString(CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "         Event Code = " + m_usEventCode.ToString(CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "         Generic Parameter 2 = " + m_usGenericParameter2.ToString("X4", CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "         Generic Parameter = " + m_uiGenericParameter.ToString("X8", CultureInfo.InvariantCulture) + "\r\n";

            return EntryString;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Parses the entry from the table data
        /// </summary>
        /// <param name="reader">The binary reader containing the table data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        internal void ParseEntry(PSEMBinaryReader reader)
        {
            m_EventSeconds = reader.ReadUInt32();
            m_usEventCode = reader.ReadUInt16();
            m_usGenericParameter2 = reader.ReadUInt16();
            m_uiGenericParameter = reader.ReadUInt32();
        }

        #endregion

        #region Member Variables

        private uint m_EventSeconds;
        private ushort m_usEventCode;
        private ushort m_usGenericParameter2;
        private uint m_uiGenericParameter;

        #endregion
    }

    /// <summary>
    /// Condensed Registration Table Entry
    /// </summary>
    public class CondensedRegistrationTableEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        public CondensedRegistrationTableEntry()
        {
        }

        /// <summary>
        /// Formats the content of the Entry into a string
        /// </summary>
        /// <returns>The entry as a string</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/11/11 RCG 2.53.05        Created

        public override string ToString()
        {
            string EntryString = "";

            EntryString += "         MAC Address = " + m_MACAddress.ToString("X16", CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "         Last Heard Time = " + m_LastHeardSeconds.ToString(CultureInfo.InvariantCulture) + "\r\n";
            EntryString += "         Registration Status =" + m_byRegistrationStatus.ToString(CultureInfo.InvariantCulture) + "\r\n";

            return EntryString;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Parses the entry from the table data
        /// </summary>
        /// <param name="reader">The binary reader containg the table data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.32        Created

        internal void ParseEntry(PSEMBinaryReader reader)
        {
            m_MACAddress = reader.ReadUInt64(); ;
            m_LastHeardSeconds = reader.ReadUInt32();
            m_byRegistrationStatus = reader.ReadByte();

            // There is a 1 byte pad at the end
            reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private UInt64 m_MACAddress;
        private uint m_LastHeardSeconds;
        private byte m_byRegistrationStatus;

        #endregion
    }
}
