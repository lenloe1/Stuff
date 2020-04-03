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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The Fatal Error Recovery Status flags
    /// </summary>
    [Flags]
    public enum FatalErrorRecoveryStatus : byte
    {
        /// <summary>
        /// No status
        /// </summary>
        None = 0x00,
        /// <summary>
        /// The meter is configured to use Fatal Recovery
        /// </summary>
        ActiveConfig = 0x03,
        /// <summary>
        /// The meter is currently in Recovery mode
        /// </summary>
        CurrentlyEnabled = 0x04,
        /// <summary>
        /// A Core Dump is available
        /// </summary>
        CoreDumpAvailable = 0x08,
        /// <summary>
        /// Recovery is in process
        /// </summary>
        InProgress = 0x10,
    }

    /// <summary>
    /// Fatal Error bitfield
    /// </summary>
    [Flags]
    public enum FatalErrors : byte
    {
        /// <summary>
        /// No Errors
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Fatal Error 1 - MCU Flash
        /// </summary>
        FatalError1 = 0x01,
        /// <summary>
        /// Fatal Error 2 - RAM
        /// </summary>
        FatalError2 = 0x02,
        /// <summary>
        /// Fatal Error 3 - Data Flash
        /// </summary>
        FatalError3 = 0x04,
        /// <summary>
        /// Fatal Error 4 - CPC
        /// </summary>
        FatalError4 = 0x08,
        /// <summary>
        /// Fatal Error 5 - Bad EPF Data
        /// </summary>
        FatalError5 = 0x10,
        /// <summary>
        /// Fatal Error 6 - File System
        /// </summary>
        FatalError6 = 0x20,
        /// <summary>
        /// Fatal Error 7 - Operating System
        /// </summary>
        FatalError7 = 0x40,
        /// <summary>
        /// Fatal Error Present
        /// </summary>
        ErrorPresent = 0x80,
    }

    /// <summary>
    /// Historical information for a Fatal Error
    /// </summary>
    public class FatalErrorHistoryData
    {
        #region Constants

        private readonly DateTime REFERENCE_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public FatalErrorHistoryData()
        {
        }

        /// <summary>
        /// Parses the data read from the meter.
        /// </summary>
        /// <param name="reader">The PSEM Binary Reader that contains the Fatal Error data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public void ParseData(PSEMBinaryReader reader)
        {
            m_uiTimeOfOccurance = reader.ReadUInt32();
            m_byFatalError = reader.ReadByte();
            m_byRecoveryStatus = reader.ReadByte();
            m_usReasonCode = reader.ReadUInt16();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Date and Time that the Fatal Error occurred.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public DateTime TimeOfOccurance
        {
            get
            {
                DateTime CurrentOccurrance = REFERENCE_TIME;

                if (m_uiTimeOfOccurance != 0xFFFFFFFF)
                {
                    CurrentOccurrance = CurrentOccurrance.AddSeconds(m_uiTimeOfOccurance);
                }

                return CurrentOccurrance;
            }
        }

        /// <summary>
        /// Gets the Errors that occurred for this Recovery Event
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public FatalErrors Error
        {
            get
            {
                FatalErrors CurrentErrors = FatalErrors.None;

                if (m_byFatalError != 0xFF)
                {
                    CurrentErrors = (FatalErrors)m_byFatalError;
                }

                return CurrentErrors;
            }
        }

        /// <summary>
        /// Gets the Recovery Status for the error.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public FatalErrorRecoveryStatus RecoveryStatus
        {
            get
            {
                FatalErrorRecoveryStatus CurrentStatus = FatalErrorRecoveryStatus.None;

                if (m_byRecoveryStatus != 0xFF)
                {
                    CurrentStatus = (FatalErrorRecoveryStatus)m_byRecoveryStatus;
                }

                return CurrentStatus;
            }
        }

        /// <summary>
        /// Gets the reason the Fatal Error occurred.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public ushort Reason
        {
            get
            {
                return m_usReasonCode;
            }
        }

        #endregion

        #region Member Variables

        private uint m_uiTimeOfOccurance;
        private byte m_byFatalError;
        private byte m_byRecoveryStatus;
        private ushort m_usReasonCode;

        #endregion
    }

    /// <summary>
    /// MFG Table 2261 (Itron 213)
    /// </summary>
    public class OpenWayMFGTable2261 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 120;
        private const int TABLE_TIMEOUT = 5000;
        private const int MAX_NUM_HISTORY_ENTRIES = 14;

        #endregion

        #region Public Members

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public OpenWayMFGTable2261(CPSEM psem)
            : base(psem, 2261, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used by EDL file.
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/20/13 jrf 2.70.68 288152 Created
        //
        public OpenWayMFGTable2261(PSEMBinaryReader binaryReader)
            : base(2261, TABLE_SIZE)
        {
            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }
        
        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2261.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current recovery status
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public FatalErrorRecoveryStatus CurrentRecoveryStatus
        {
            get
            {
                FatalErrorRecoveryStatus CurrentStatus = FatalErrorRecoveryStatus.None;

                ReadUnloadedTable();

                if (m_byRecoveryStatus != 0xFF)
                {
                    // The last status is valid
                    CurrentStatus = (FatalErrorRecoveryStatus)m_byRecoveryStatus;
                }

                return CurrentStatus;
            }
        }

        /// <summary>
        /// Gets the number of times the meter has entered Fatal Error Recovery mode
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public byte FatalErrorRecoveryCount
        {
            get
            {
                ReadUnloadedTable();

                return m_byRecoveryCount;
            }
        }

        /// <summary>
        /// Gets the list of Fatal Errors that have occurred in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        public List<FatalErrorHistoryData> FatalErrorHistory
        {
            get
            {
                ReadUnloadedTable();

                return m_HistoryData;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data that was just read. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/10 RCG 2.40.09 N/A    Created

        private void ParseData()
        {
            m_byRecoveryStatus = m_Reader.ReadByte();
            
            m_byLastValidEntryIndex = m_Reader.ReadByte();
            m_byRecoveryCount = m_Reader.ReadByte();
            m_byFiller1 = m_Reader.ReadByte();
            m_byValidationLower = m_Reader.ReadByte();
            m_byValidationUpper = m_Reader.ReadByte();
            m_usFiller2 = m_Reader.ReadUInt16();

            m_HistoryData = new List<FatalErrorHistoryData>();

            // We shouldn't try to read the entries if this index is higher than
            // the allowed number of entries. This probably means there are no entries.
            if (m_byLastValidEntryIndex < MAX_NUM_HISTORY_ENTRIES)
            {
                for (int iIndex = 0; iIndex <= m_byLastValidEntryIndex; iIndex++)
                {
                    FatalErrorHistoryData CurrentError = new FatalErrorHistoryData();

                    CurrentError.ParseData(m_Reader);
                    m_HistoryData.Add(CurrentError);
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_byRecoveryStatus;
        private byte m_byLastValidEntryIndex;
        private byte m_byRecoveryCount;
        private byte m_byFiller1;
        private byte m_byValidationLower;
        private byte m_byValidationUpper;
        private ushort m_usFiller2;
        private List<FatalErrorHistoryData> m_HistoryData;

        #endregion
    }
}
