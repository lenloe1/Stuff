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
//                              Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    #region StdTable21

    /// <summary>
    /// Standard Table 21 - Actual Register
    /// </summary>
    public class StdTable21 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 10;
        private const int TABLE_TIMEOUT = 4000;

        // Register Function 1 Masks
        private const byte SEASON_INFO_MASK = 0x01;
        private const byte DATE_TIME_MASK = 0x02;
        private const byte DEMAND_RESET_CTR_MASK = 0x04;
        private const byte DEMAND_RESET_LOCK_MASK = 0x08;
        private const byte CUM_DEMAND_MASK = 0x10;
        private const byte CONT_CUM_DEMAND_MASK = 0x20;
        private const byte TIME_REMAINING_MASK = 0x40;

        // Register Funtion 2 Masks
        private const byte SELF_READ_INHIBIT_MASK = 0x01;
        private const byte SELF_READ_SEQ_MASK = 0x02;
        private const byte DAILY_SELF_READ_MASK = 0x04;
        private const byte WEEKLY_SELF_READ_MASK = 0x08;
        private const byte SELF_READ_DEMAND_RESET_MASK = 0x30;

        private const int SELF_READ_DEMAND_RESET_SHIFT = 4;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration for the Self Read and Demand Reset capabilities
        /// </summary>
        public enum SRDRType : byte
        {
            /// <summary>
            /// The meter does not perform a Self Read on every Demand Reset nor
            /// a Demand Reset on every Self Read.
            /// </summary>
            Neither = 0,
            /// <summary>
            /// The meter only performs a Self Read on every Demand Reset.
            /// </summary>
            SelfReadOnDemandReset = 1,
            /// <summary>
            /// The meter only performs a Demand Reset on every Self Read.
            /// </summary>
            DemandResetOnSelfRead = 2,
            /// <summary>
            /// The meter performs a Self Read on every Demand Reset and
            /// a Demand Reset on every Self Read.
            /// </summary>
            Both = 3,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public StdTable21(CPSEM psem)
            : base(psem, 21, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public StdTable21(PSEMBinaryReader reader)
            : base(21, TABLE_SIZE)
        {
            State = TableState.Loaded;
            m_Reader = reader;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable21.Read");

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
        /// Gets whether or not the season information is included in the data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool IncludeSeasonInfo
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc1Flags & SEASON_INFO_MASK) == SEASON_INFO_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the date and time is being included in the data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool IncludeDateTime
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc1Flags & DATE_TIME_MASK) == DATE_TIME_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Demand Reset counter is being included in the data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool IncludeDemandResetCounter
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc1Flags & DEMAND_RESET_CTR_MASK) == DEMAND_RESET_CTR_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Demand Reset lockout is enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool DemandResetLockEnabled
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc1Flags & DEMAND_RESET_LOCK_MASK) == DEMAND_RESET_LOCK_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Cumulative Demand is included in the data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool IncludeCumulativeDemand
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc1Flags & CUM_DEMAND_MASK) == CUM_DEMAND_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Continuous Cumulative Demand is included in the data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool IncludeContinuousCumulativeDemand
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc1Flags & CONT_CUM_DEMAND_MASK) == CONT_CUM_DEMAND_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the time remaing in a demand interval is included in the data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool IncludeTimeRemaining
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc1Flags & TIME_REMAINING_MASK) == TIME_REMAINING_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the device will inhibit self reads if an overflow occurs
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool InhibitSelfReadAfterOverflow
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc2Flags & SELF_READ_INHIBIT_MASK) == SELF_READ_INHIBIT_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Self Read Sequence number should be included for each entry
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool IncludeSelfReadSequenceNumber
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc2Flags & SELF_READ_SEQ_MASK) == SELF_READ_SEQ_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not daily self reads are enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool SupportsDailySelfRead
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc2Flags & DAILY_SELF_READ_MASK) == DAILY_SELF_READ_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not weekly self reads are enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool SupportsWeeklySelfRead
        {
            get
            {
                ReadUnloadedTable();
                return (m_byRegFunc2Flags & WEEKLY_SELF_READ_MASK) == WEEKLY_SELF_READ_MASK;
            }
        }

        /// <summary>
        /// Gets the setting for whether or not Self Reads will be performed on a Demand Reset
        /// and Demand Reset will be be performed on a Self Read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public SRDRType SelfReadDemandReset
        {
            get
            {
                byte byCapability;

                ReadUnloadedTable();

                // Pull the bits from the bitfield and convert to the enumeration
                byCapability = (byte)(m_byRegFunc2Flags & SELF_READ_DEMAND_RESET_MASK);
                byCapability = (byte)(byCapability >> SELF_READ_DEMAND_RESET_SHIFT);
                return (SRDRType)byCapability;
            }
        }

        /// <summary>
        /// Gets the number of Self Reads.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfSelfReads
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfSelfReads;
            }
        }

        /// <summary>
        /// Gets the number of summations (Energies)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfSummations
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfSummations;
            }
        }

        /// <summary>
        /// Gets the number of demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfDemands
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfDemands;
            }
        }

        /// <summary>
        /// Gets the number of coincident values
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfCoincidentValues
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfCoincidentValues;
            }
        }

        /// <summary>
        /// Gets the number of occurances
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfOccurances
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfOccurances;
            }
        }

        /// <summary>
        /// Gets the number of TOU tiers
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfTiers
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfTiers;
            }
        }

        /// <summary>
        /// Gets the number of present demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfPresentDemands
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfPresentDemands;
            }
        }

        /// <summary>
        /// Gets the number of present values
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfPresentValues
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfPresentValues;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses that data that has been read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            m_byRegFunc1Flags = m_Reader.ReadByte();
            m_byRegFunc2Flags = m_Reader.ReadByte();
            m_byNumberOfSelfReads = m_Reader.ReadByte();
            m_byNumberOfSummations = m_Reader.ReadByte();
            m_byNumberOfDemands = m_Reader.ReadByte();
            m_byNumberOfCoincidentValues = m_Reader.ReadByte();
            m_byNumberOfOccurances = m_Reader.ReadByte();
            m_byNumberOfTiers = m_Reader.ReadByte();
            m_byNumberOfPresentDemands = m_Reader.ReadByte();
            m_byNumberOfPresentValues = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private byte m_byRegFunc1Flags;
        private byte m_byRegFunc2Flags;
        private byte m_byNumberOfSelfReads;
        private byte m_byNumberOfSummations;
        private byte m_byNumberOfDemands;
        private byte m_byNumberOfCoincidentValues;
        private byte m_byNumberOfOccurances;
        private byte m_byNumberOfTiers;
        private byte m_byNumberOfPresentDemands;
        private byte m_byNumberOfPresentValues;

        #endregion
    }

    #endregion

    #region StdTable22

    /// <summary>
    /// Standard Table 22 - Data Selection
    /// </summary>
    public class StdTable22 : AnsiTable
    {
        #region Constants

        private const byte MIN_MAX_MASK = 0x01;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Definitions

        /// <summary>
        /// Specifies the type of the demand selection
        /// </summary>
        public enum DemandType : byte
        {
            /// <summary>
            /// The demand is a minimum value.
            /// </summary>
            Min = 0,
            /// <summary>
            /// The demand is a maximum value.
            /// </summary>
            Max = 1,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public StdTable22(CPSEM psem, StdTable21 Table21)
            : base(psem, 22, DetermineTableSize(Table21), TABLE_TIMEOUT)
        {
            m_Table21 = Table21;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table21">The table 21 object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public StdTable22(PSEMBinaryReader reader, StdTable21 table21)
            : base(22, DetermineTableSize(table21))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table21 = table21;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable22.Read");

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
        /// Gets the list of summation (energy) selections as an index into table 14/16
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte[] SummationSelections
        {
            get
            {
                ReadUnloadedTable();
                return m_SummationSelections;
            }
        }

        /// <summary>
        /// Gets the list of demand selections as an index into table 14/16
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte[] DemandSelections
        {
            get
            {
                ReadUnloadedTable();
                return m_DemandSelections;
            }
        }

        /// <summary>
        /// Gets the type of demand for each demand selection.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public DemandType[] DemandTypes
        {
            get
            {
                DemandType[] demandTypes = null;

                ReadUnloadedTable();

                if (m_Table21.NumberOfDemands > 0)
                {
                    demandTypes = new DemandType[m_Table21.NumberOfDemands];

                    for (int iIndex = 0; iIndex < m_Table21.NumberOfDemands; iIndex++)
                    {
                        int iMinMaxIndex = iIndex / 8;
                        int iFlagBitShift = iIndex % 8;
                        byte byFlag = (byte)((m_MinMaxFlags[iMinMaxIndex] >> iFlagBitShift) & MIN_MAX_MASK);

                        demandTypes[iIndex] = (DemandType)byFlag;
                    }
                }

                return demandTypes;
            }
        }

        /// <summary>
        /// Gets the list of Coincident Selection as an index into table 16
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte[] CoincidentSelection
        {
            get
            {
                ReadUnloadedTable();
                return m_CoincidentSelections;
            }
        }

        /// <summary>
        /// Gets the list of Coincident Associations as an index into the Demand Selections
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte[] CoincidentDemandAssocations
        {
            get
            {
                ReadUnloadedTable();
                return m_ConcidentDemandAssociations;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of Standard Table 22
        /// </summary>
        /// <param name="Table21">The table 21 object for the current device</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private static uint DetermineTableSize(StdTable21 Table21)
        {
            uint uiTableSize = 0;

            // Summations
            uiTableSize += Table21.NumberOfSummations;
            // Demand
            uiTableSize += Table21.NumberOfDemands;
            // Demand Min/Max Flags
            uiTableSize += (uint)((Table21.NumberOfDemands + 7) / 8);
            // Coincident Select
            uiTableSize += Table21.NumberOfCoincidentValues;
            // CoincidentAssociations
            uiTableSize += Table21.NumberOfCoincidentValues;

            return uiTableSize;
        }

        /// <summary>
        /// Parses the data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            ParseSummations();
            ParseDemands();
            ParseCoincidents();
        }

        /// <summary>
        /// Parses the Summation (Energy) data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseSummations()
        {
            byte byNumSummations = m_Table21.NumberOfSummations;

            if (byNumSummations > 0)
            {
                m_SummationSelections = m_Reader.ReadBytes(byNumSummations);
            }
            else
            {
                m_SummationSelections = null;
            }
        }

        /// <summary>
        /// Parses the demand data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseDemands()
        {
            byte byNumDemands = m_Table21.NumberOfDemands;

            if (byNumDemands > 0)
            {
                m_DemandSelections = m_Reader.ReadBytes(byNumDemands);
                m_MinMaxFlags = m_Reader.ReadBytes((byNumDemands + 7) / 8);
            }
            else
            {
                m_DemandSelections = null;
                m_MinMaxFlags = null;
            }
        }

        /// <summary>
        /// Parses the coincident data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseCoincidents()
        {
            byte byNumCoincidents = m_Table21.NumberOfCoincidentValues;

            if (byNumCoincidents > 0)
            {
                m_CoincidentSelections = m_Reader.ReadBytes(byNumCoincidents);
                m_ConcidentDemandAssociations = m_Reader.ReadBytes(byNumCoincidents);
            }
            else
            {
                m_CoincidentSelections = null;
                m_ConcidentDemandAssociations = null;
            }
        }

        #endregion

        #region Member Variables

        private StdTable21 m_Table21;
        private byte[] m_SummationSelections;
        private byte[] m_DemandSelections;
        private byte[] m_MinMaxFlags;
        private byte[] m_CoincidentSelections;
        private byte[] m_ConcidentDemandAssociations;

        #endregion
    }

    #endregion

    #region StdTable23

    /// <summary>
    /// Standard Table 23 - Current Register Data
    /// </summary>
    public class StdTable23 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="Table21">The Table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public StdTable23(CPSEM psem,  CTable00 Table0, StdTable21 Table21)
            : base (psem, 23, DetermineTableSize(Table0, Table21), TABLE_TIMEOUT) 
        {
            m_CurrentRegisters = null;
            m_Table21 = Table21;
            m_Table0 = Table0;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table0">The table 0 object</param>
        /// <param name="table21">The table 21 object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public StdTable23(PSEMBinaryReader reader, CTable00 table0, StdTable21 table21)
            : base(23, DetermineTableSize(table0, table21))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table21 = table21;
            m_Table0 = table0;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable23.Read");

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
        /// Gets the current register data record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public RegisterDataRecord CurrentRegisters
        {
            get
            {
                ReadUnloadedTable();
                return m_CurrentRegisters;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determine the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="Table21">The Table 21 object for the current device.</param>
        /// <returns>The size of the table as an uint</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private static uint DetermineTableSize(CTable00 Table0, StdTable21 Table21)
        {
            return RegisterDataRecord.Size(Table0, Table21);
        }

        /// <summary>
        /// Parses the data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            m_CurrentRegisters = new RegisterDataRecord(m_Table0, m_Table21);
            m_CurrentRegisters.Parse(m_Reader);
        }

        #endregion

        #region Member Variables

        private StdTable21 m_Table21;
        private CTable00 m_Table0;

        private RegisterDataRecord m_CurrentRegisters;
        
        #endregion
    }

    #endregion

    #region StdTable24

    /// <summary>
    /// Standard Table 24 - Previous Season Data
    /// </summary>
    public class StdTable24 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public StdTable24(CPSEM psem, CTable00 Table0, StdTable21 Table21)
            : base (psem, 24, DetermineTableSize(Table0, Table21), TABLE_TIMEOUT)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;

            m_PreviousSeasonRegisterData = null;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table0">The table 0 object</param>
        /// <param name="table21">The table 21 object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public StdTable24(PSEMBinaryReader reader, CTable00 table0, StdTable21 table21)
            : base(24, DetermineTableSize(table0, table21))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table21 = table21;
            m_Table0 = table0;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable24.Read");

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
        /// Gets the end date and time for the previous season.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public DateTime PreviousSeasonEndDate
        {
            get
            {
                ReadUnloadedTable();
                return m_EndDateTime;
            }
        }

        /// <summary>
        /// Gets the season index for the previous season.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte PreviousSeasonIndex
        {
            get
            {
                ReadUnloadedTable();
                return m_SeasonIndex;
            }
        }

        /// <summary>
        /// Gets the register data for the previous season.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public RegisterDataRecord PreviousSeasonRegisterData
        {
            get
            {
                ReadUnloadedTable();
                return m_PreviousSeasonRegisterData;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines that size of the table.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private static uint DetermineTableSize(CTable00 Table0, StdTable21 Table21)
        {
            uint uiTableSize = 0;

            // End Time
            if (Table21.IncludeDateTime)
            {
                uiTableSize += Table0.STIMESize;
            }

            // Season index
            if (Table21.IncludeSeasonInfo)
            {
                uiTableSize += 1;
            }

            // Previous Season Data
            uiTableSize += RegisterDataRecord.Size(Table0, Table21);

            return uiTableSize;
        }

        /// <summary>
        /// Parse that data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            if (m_Table21.IncludeDateTime)
            {
                m_EndDateTime = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
            }

            if (m_Table21.IncludeSeasonInfo)
            {
                m_SeasonIndex = m_Reader.ReadByte();
            }

            m_PreviousSeasonRegisterData = new RegisterDataRecord(m_Table0, m_Table21);

            m_PreviousSeasonRegisterData.Parse(m_Reader);
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable21 m_Table21;

        private DateTime m_EndDateTime;
        private byte m_SeasonIndex;
        private RegisterDataRecord m_PreviousSeasonRegisterData;

        #endregion
    }

    #endregion

    #region StdTable25

    /// <summary>
    /// Standard Table 25 - Previous Demand Reset Data
    /// </summary>
    public class StdTable25 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public StdTable25(CPSEM psem, CTable00 Table0, StdTable21 Table21)
            : base(psem, 25, DetermineTableSize(Table0, Table21), TABLE_TIMEOUT)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;

            m_DemandResetRegisterData = null;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table0">The table 0 object</param>
        /// <param name="table21">The table 21 object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public StdTable25(PSEMBinaryReader reader, CTable00 table0, StdTable21 table21)
            : base(25, DetermineTableSize(table0, table21))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table21 = table21;
            m_Table0 = table0;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable25.Read");

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
        /// Gets the end date and time of the last demand reset.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public DateTime DemandResetDate
        {
            get
            {
                ReadUnloadedTable();
                return m_EndDateTime;
            }
        }

        /// <summary>
        /// Gets the season index for the last demand reset.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte DemandResetSeasonIndex
        {
            get
            {
                ReadUnloadedTable();
                return m_SeasonIndex;
            }
        }

        /// <summary>
        /// Gets the register data for the last demand reset.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public RegisterDataRecord DemandResetRegisterData
        {
            get
            {
                ReadUnloadedTable();
                return m_DemandResetRegisterData;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines that size of the table.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private static uint DetermineTableSize(CTable00 Table0, StdTable21 Table21)
        {
            uint uiTableSize = 0;

            // End Time
            if (Table21.IncludeDateTime)
            {
                uiTableSize += Table0.STIMESize;
            }

            // Season index
            if (Table21.IncludeSeasonInfo)
            {
                uiTableSize += 1;
            }

            // Demand Reset Data
            uiTableSize += RegisterDataRecord.Size(Table0, Table21);

            return uiTableSize;
        }

        /// <summary>
        /// Parse that data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            if (m_Table21.IncludeDateTime)
            {
                m_EndDateTime = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
            }

            if (m_Table21.IncludeSeasonInfo)
            {
                m_SeasonIndex = m_Reader.ReadByte();
            }

            m_DemandResetRegisterData = new RegisterDataRecord(m_Table0, m_Table21);

            m_DemandResetRegisterData.Parse(m_Reader);
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable21 m_Table21;

        private DateTime m_EndDateTime;
        private byte m_SeasonIndex;
        private RegisterDataRecord m_DemandResetRegisterData;

        #endregion
    }

    #endregion

    #region StdTable26

    /// <summary>
    /// Standard Table 26 - Self Read Data
    /// </summary>
    public class StdTable26 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 30000;
        private const uint STATUS_DATA_SIZE = 6;

        // List Status Masks
        private const byte ORDER_MASK = 0x01;
        private const byte OVERFLOW_MASK = 0x02;
        private const byte LIST_TYPE_MASK = 0x04;
        private const int LIST_TYPE_SHIFT = 2;
        private const byte INHIBIT_OVERFLOW_MASK = 0x08;

        #endregion

        #region Definitions

        /// <summary>
        /// Describes the sort order of the self reads.
        /// </summary>
        public enum Order : byte
        {
            /// <summary>
            /// The self reads are sorted in ascending order by date.
            /// </summary>
            Ascending = 0,
            /// <summary>
            /// The self reads are sorted in descending order by date.
            /// </summary>
            Descending = 1,
        }

        /// <summary>
        /// Describes the type of list used to store the self reads
        /// </summary>
        public enum ListType : byte
        {
            /// <summary>
            /// The list is stored as first in first out 
            /// </summary>
            FIFO = 0,
            /// <summary>
            /// The list is stored circularly.
            /// </summary>
            Circular = 1,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public StdTable26(CPSEM psem, CTable00 Table0, StdTable21 Table21)
            : base(psem, 26, DetermineTableSize(Table0, Table21), TABLE_TIMEOUT)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table0">The table 0 object</param>
        /// <param name="table21">The table 21 object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public StdTable26(PSEMBinaryReader reader, CTable00 table0, StdTable21 table21)
            : base(26, DetermineTableSize(table0, table21))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table21 = table21;
            m_Table0 = table0;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable26.Read");

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
        /// Gets the sort order of the self read list.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public Order SelfReadOrder
        {
            get
            {
                ReadUnloadedTable();
                return (Order)(m_byListStatus & ORDER_MASK);
            }
        }

        /// <summary>
        /// Gets whether or not the self read list has overflown.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool HasOverflown
        {
            get
            {
                ReadUnloadedTable();
                return (m_byListStatus & OVERFLOW_MASK) == OVERFLOW_MASK;
            }
        }

        /// <summary>
        /// Gets the type of list used to store the self reads.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public ListType TypeOfList
        {
            get
            {
                ReadUnloadedTable();
                return (ListType)((m_byListStatus & LIST_TYPE_MASK) >> LIST_TYPE_SHIFT);
            }
        }

        /// <summary>
        /// Gets whether or not self read overflow is inhibited.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool InhibitOverflow
        {
            get
            {
                ReadUnloadedTable();
                return (m_byListStatus & INHIBIT_OVERFLOW_MASK) == INHIBIT_OVERFLOW_MASK;
            }
        }

        /// <summary>
        /// Gets the number of valid self reads stored in the table.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfValidEntries
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfValidEntries;
            }
        }

        /// <summary>
        /// Gets the element index of the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte LastEntryElement
        {
            get
            {
                ReadUnloadedTable();
                return m_byLastEntryElement;
            }
        }

        /// <summary>
        /// Gets the sequence number of the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public ushort LastEntrySequenceNumber
        {
            get
            {
                ReadUnloadedTable();
                return m_usLastEntrySequenceNumber;
            }
        }

        /// <summary>
        /// Gets the number of self read entries that have not been read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfUnreadEntries
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfUnreadEntries;
            }
        }

        /// <summary>
        /// Gets the list of self read entries.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public SelfReadDataRecord[] SelfReadEntries
        {
            get
            {
                ReadUnloadedTable();
                return m_SelfReadEntries;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private static uint DetermineTableSize(CTable00 Table0, StdTable21 Table21)
        {
            uint uiSize = STATUS_DATA_SIZE;

            uiSize += Table21.NumberOfSelfReads * SelfReadDataRecord.Size(Table0, Table21);

            return uiSize;
        }

        /// <summary>
        /// Parses the data that was just read from the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            m_byListStatus = m_Reader.ReadByte();
            m_byNumberOfValidEntries = m_Reader.ReadByte();
            m_byLastEntryElement = m_Reader.ReadByte();
            m_usLastEntrySequenceNumber = m_Reader.ReadUInt16();
            m_byNumberOfUnreadEntries = m_Reader.ReadByte();

            m_SelfReadEntries = new SelfReadDataRecord[m_Table21.NumberOfSelfReads];

            for (int iIndex = 0; iIndex < m_Table21.NumberOfSelfReads; iIndex++)
            {
                m_SelfReadEntries[iIndex] = new SelfReadDataRecord(m_Table0, m_Table21);
                m_SelfReadEntries[iIndex].Parse(m_Reader);
            }
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable21 m_Table21;

        private byte m_byListStatus;
        private byte m_byNumberOfValidEntries;
        private byte m_byLastEntryElement;
        private ushort m_usLastEntrySequenceNumber;
        private byte m_byNumberOfUnreadEntries;
        private SelfReadDataRecord[] m_SelfReadEntries;

        #endregion
    }

    #endregion

    #region StdTable27

    /// <summary>
    /// Standard Table 27 - Present Register Selection
    /// </summary>
    public class StdTable27 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public StdTable27(CPSEM psem, StdTable21 Table21)
            : base(psem, 27, DetermineTableSize(Table21))
        {
            m_Table21 = Table21;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table21">The table 21 object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public StdTable27(PSEMBinaryReader reader, StdTable21 table21)
            : base(27, DetermineTableSize(table21))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table21 = table21;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable27.Read");

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
        /// Gets the list of Present Demand selections
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte[] PresentDemandSelections
        {
            get
            {
                ReadUnloadedTable();
                return m_PresentDemandSelections;
            }
        }

        /// <summary>
        /// Gets the list of Present Value selections
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte[] PresentValueSelections
        {
            get
            {
                ReadUnloadedTable();
                return m_PresentValueSelections;
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
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            m_PresentDemandSelections = m_Reader.ReadBytes(m_Table21.NumberOfPresentDemands);
            m_PresentValueSelections = m_Reader.ReadBytes(m_Table21.NumberOfPresentValues);
        }

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private static uint DetermineTableSize(StdTable21 Table21)
        {
            uint uiTableSize = 0;

            // Present Demands
            uiTableSize += Table21.NumberOfPresentDemands;

            // Present Values
            uiTableSize += Table21.NumberOfPresentValues;

            return uiTableSize;
        }

        #endregion

        #region Member Variables

        private StdTable21 m_Table21;

        private byte[] m_PresentDemandSelections;
        private byte[] m_PresentValueSelections;

        #endregion
    }

    #endregion

    #region StdTable28

    /// <summary>
    /// Standard Table 28 - Present Register Data
    /// </summary>
    public class StdTable28 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public StdTable28(CPSEM psem, CTable00 Table0, StdTable21 Table21)
            : base (psem, 28, DetermineTableSize(Table0, Table21), TABLE_TIMEOUT)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table0">The table 0 object</param>
        /// <param name="table21">The table 21 object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public StdTable28(PSEMBinaryReader reader, CTable00 table0, StdTable21 table21)
            : base(28, DetermineTableSize(table0, table21))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table21 = table21;
            m_Table0 = table0;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable28.Read");

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
        /// Gets the list of present demand record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public PresentDemandRecord[] PresentDemands
        {
            get
            {
                ReadUnloadedTable();
                return m_PresentDemands;
            }
        }

        /// <summary>
        /// Gets the list of present values
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public double[] PresentValues
        {
            get
            {
                ReadUnloadedTable();
                return m_PresentValues;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data that has just been read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            // Read the present demands.
            m_PresentDemands = new PresentDemandRecord[m_Table21.NumberOfPresentDemands];

            for (int iIndex = 0; iIndex < m_Table21.NumberOfPresentDemands; iIndex++)
            {
                m_PresentDemands[iIndex] = new PresentDemandRecord(m_Table0, m_Table21);
                m_PresentDemands[iIndex].Parse(m_Reader);
            }

            // Read the present values.
            m_PresentValues = new double[m_Table21.NumberOfPresentValues];

            for (int iIndex = 0; iIndex < m_Table21.NumberOfPresentValues; iIndex++)
            {
                m_PresentValues[iIndex] = m_Reader.ReadDouble();
            }
        }

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private static uint DetermineTableSize(CTable00 Table0, StdTable21 Table21)
        {
            uint uiTableSize = 0;

            uiTableSize += Table21.NumberOfPresentDemands * PresentDemandRecord.Size(Table0, Table21);

            // Present Values NI_FMAT1 (Float64)
            uiTableSize += (uint)(Table21.NumberOfPresentValues * 8);

            return uiTableSize;
        }

        #endregion

        #region Member Variables

        private StdTable21 m_Table21;
        private CTable00 m_Table0;

        private PresentDemandRecord[] m_PresentDemands;
        private double[] m_PresentValues;

        #endregion
    }

    #endregion

    #region DemandRecord

    /// <summary>
    /// The demand record used by the 2's decade.
    /// </summary>
    public class DemandRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal DemandRecord(CTable00 Table0, StdTable21 Table21)
        {
            m_TimeOfOccurances = null;
            m_fDemands = null;
            m_Table21 = Table21;
            m_Table0 = Table0;
        }

        /// <summary>
        /// Parses the demand record from the binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the demand record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public void Parse(PSEMBinaryReader Reader)
        {
            // Read the time of occurances
            if (m_Table21.IncludeDateTime)
            {
                m_TimeOfOccurances = new DateTime[m_Table21.NumberOfOccurances];

                for (int iIndex = 0; iIndex < m_Table21.NumberOfOccurances; iIndex++)
                {
                    m_TimeOfOccurances[iIndex] = 
                        Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
                }
            }

            // Read the Cum Demand
            if (m_Table21.IncludeCumulativeDemand)
            {
                m_dCum = Reader.ReadDouble();
            }

            // Read the CCum demand
            if (m_Table21.IncludeContinuousCumulativeDemand)
            {
                m_dCCum = Reader.ReadDouble();
            }

            // Read the demand values
            m_fDemands = new float[m_Table21.NumberOfOccurances];

            for (int iIndex = 0; iIndex < m_Table21.NumberOfOccurances; iIndex++)
            {
                m_fDemands[iIndex] = Reader.ReadSingle();
            }
        }

        /// <summary>
        /// Determines the size of the demand record.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the demand record in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal static uint Size(CTable00 Table0, StdTable21 Table21)
        {
            uint uiSize = 0;

            // Time of occurances
            if (Table21.IncludeDateTime)
            {
                uiSize += Table21.NumberOfOccurances * Table0.STIMESize;
            }

            if (Table21.IncludeCumulativeDemand)
            {
                // Cum is NI_FMAT1 (FLOAT64)
                uiSize += 8;
            }

            if (Table21.IncludeContinuousCumulativeDemand)
            {
                // CCum is NI_FMAT1 (FLOAT64)
                uiSize += 8;
            }

            // Demands are an array of NI_FMAT2 (FLOAT32)
            uiSize += (uint)(Table21.NumberOfOccurances * 4);

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the time of occurances for the current demand.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public DateTime[] TimeOfOccurances
        {
            get
            {
                return m_TimeOfOccurances;
            }
        }

        /// <summary>
        /// Gets the cummulative demand value.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public double Cum
        {
            get
            {
                return m_dCum;
            }
        }

        /// <summary>
        /// Gets the continuously cummulative demand value.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public double CCum
        {
            get
            {
                return m_dCCum;
            }
        }

        /// <summary>
        /// Gets all of the demand values for the current Demand.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public float[] Demands
        {
            get
            {
                return m_fDemands;
            }
        }

        #endregion

        #region Member Variables

        private StdTable21 m_Table21;
        private CTable00 m_Table0;

        private DateTime[] m_TimeOfOccurances;
        private double m_dCum;
        private double m_dCCum;
        private float[] m_fDemands;

        #endregion
    }

    #endregion

    #region CoincidentRecord

    /// <summary>
    /// The coincident record used by the 2's decade
    /// </summary>
    public class CoincidentRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Table21">The Table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal CoincidentRecord(StdTable21 Table21)
        {
            m_Coincidents = null;
            m_Table21 = Table21;
        }

        /// <summary>
        /// Parses the record from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public void Parse (PSEMBinaryReader Reader)
        {
            m_Coincidents = new float[m_Table21.NumberOfOccurances];

            for (int iIndex = 0; iIndex < m_Table21.NumberOfOccurances; iIndex++)
            {
                m_Coincidents[iIndex] = Reader.ReadSingle();
            }
        }

        /// <summary>
        /// Determines the size of the record.
        /// </summary>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the record in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal static uint Size(StdTable21 Table21)
        {
            return (uint)(Table21.NumberOfOccurances * 4);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the coincident values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public float[] Coincidents
        {
            get
            {
                return m_Coincidents;
            }
        }

        #endregion

        #region Member Variables

        private StdTable21 m_Table21;
        private float[] m_Coincidents;

        #endregion
    }
    #endregion

    #region DataBlockRecord

    /// <summary>
    /// The data block record used by the 2's decade
    /// </summary>
    public class DataBlockRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal DataBlockRecord(CTable00 Table0, StdTable21 Table21)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;

            m_Coincidents = null;
            m_Demands = null;
            m_Summations = null;
        }

        /// <summary>
        /// Parses the record from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the record data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public void Parse(PSEMBinaryReader Reader)
        {
            int iIndex;

            // Parse the Summations
            m_Summations = new double[m_Table21.NumberOfSummations];

            for (iIndex = 0; iIndex < m_Table21.NumberOfSummations; iIndex++)
            {
                m_Summations[iIndex] = Reader.ReadDouble();
            }

            // Parse the Demands
            m_Demands = new DemandRecord[m_Table21.NumberOfDemands];

            for (iIndex = 0; iIndex < m_Table21.NumberOfDemands; iIndex++)
            {
                m_Demands[iIndex] = new DemandRecord(m_Table0, m_Table21);
                m_Demands[iIndex].Parse(Reader);
            }

            // Parse the Coincidents
            m_Coincidents = new CoincidentRecord[m_Table21.NumberOfCoincidentValues];

            for (iIndex = 0; iIndex < m_Table21.NumberOfCoincidentValues; iIndex++)
            {
                m_Coincidents[iIndex] = new CoincidentRecord(m_Table21);
                m_Coincidents[iIndex].Parse(Reader);
            }
        }

        /// <summary>
        /// Determines the size of the record.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the record in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal static uint Size(CTable00 Table0, StdTable21 Table21)
        {
            uint uiSize = 0;

            // Summation size NI_FMAT1 (Float64)
            uiSize += (uint)(Table21.NumberOfSummations * 8);

            // Demands size
            uiSize += Table21.NumberOfDemands * DemandRecord.Size(Table0, Table21);

            // Coincidents size
            uiSize += Table21.NumberOfCoincidentValues * CoincidentRecord.Size(Table21);

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the summation (energy) values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public double[] Summations
        {
            get
            {
                return m_Summations;
            }
        }

        /// <summary>
        /// Gets the demand records.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public DemandRecord[] Demands
        {
            get
            {
                return m_Demands;
            }
        }

        /// <summary>
        /// Gets the coincident records.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public CoincidentRecord[] Coincidents
        {
            get
            {
                return m_Coincidents;
            }
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable21 m_Table21;

        private double[] m_Summations;
        private DemandRecord[] m_Demands;
        private CoincidentRecord[] m_Coincidents;

        #endregion
    }

    #endregion

    #region RegisterDataRecord

    /// <summary>
    /// The register data record used by the 2's decade.
    /// </summary>
    public class RegisterDataRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal RegisterDataRecord(CTable00 Table0, StdTable21 Table21)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;

            m_TierDataBlocks = null;
            m_TotalDataBlock = null;
        }

        /// <summary>
        /// Parses the record from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data for the record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public void Parse(PSEMBinaryReader Reader)
        {
            // Parse the demand reset counter
            if (m_Table21.IncludeDemandResetCounter)
            {
                m_byDemandResetCount = Reader.ReadByte();
            }

            // Parse the Total data block
            m_TotalDataBlock = new DataBlockRecord(m_Table0, m_Table21);
            m_TotalDataBlock.Parse(Reader);

            // Parse the Tier data blocks
            m_TierDataBlocks = new DataBlockRecord[m_Table21.NumberOfTiers];

            for (int iIndex = 0; iIndex < m_Table21.NumberOfTiers; iIndex++)
            {
                m_TierDataBlocks[iIndex] = new DataBlockRecord(m_Table0, m_Table21);
                m_TierDataBlocks[iIndex].Parse(Reader);
            }
        }

        /// <summary>
        /// Determines the size of the record.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the record in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal static uint Size(CTable00 Table0, StdTable21 Table21)
        {
            uint uiSize = 0;

            // Demand Reset count
            if (Table21.IncludeDemandResetCounter)
            {
                uiSize += 1;
            }

            // Total Data Block
            uiSize += DataBlockRecord.Size(Table0, Table21);

            // Tier Data Blocks
            uiSize += Table21.NumberOfTiers * DataBlockRecord.Size(Table0, Table21);

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the demand reset count.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte DemandResetCount
        {
            get
            {
                return m_byDemandResetCount;
            }
        }

        /// <summary>
        /// Gets the data record for the current total registers.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public DataBlockRecord TotalDataBlock
        {
            get
            {
                return m_TotalDataBlock;
            }
        }

        /// <summary>
        /// Gets the data records for each of the TOU rates.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public DataBlockRecord[] TierDataBlocks
        {
            get
            {
                return m_TierDataBlocks;
            }
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable21 m_Table21;

        private byte m_byDemandResetCount;
        private DataBlockRecord m_TotalDataBlock;
        private DataBlockRecord[] m_TierDataBlocks;

        #endregion
    }

    #endregion

    #region SelfReadDataRecord

    /// <summary>
    /// The data record for Self Reads used by the 2's decade.
    /// </summary>
    public class SelfReadDataRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public SelfReadDataRecord(CTable00 Table0, StdTable21 Table21)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;

            m_SelfReadRegisters = null;
        }

        /// <summary>
        /// Parses the record from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data for the record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public void Parse(PSEMBinaryReader Reader)
        {
            // Sequence number
            if (m_Table21.IncludeSelfReadSequenceNumber)
            {
                m_usSequenceNumber = Reader.ReadUInt16();
            }

            // Self Read time
            if (m_Table21.IncludeDateTime)
            {
                m_dtSelfReadTime = Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
            }

            // Season Index
            if (m_Table21.IncludeSeasonInfo)
            {
                m_bySeasonIndex = Reader.ReadByte();
            }

            // Self Read register data.
            m_SelfReadRegisters = new RegisterDataRecord(m_Table0, m_Table21);
            m_SelfReadRegisters.Parse(Reader);
        }

        /// <summary>
        /// Determines the size of the Self Read data record.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public static uint Size(CTable00 Table0, StdTable21 Table21)
        {
            uint uiSize = 0;

            // Sequence number
            if (Table21.IncludeSelfReadSequenceNumber)
            {
                uiSize += 2;
            }

            // Self Read time
            if (Table21.IncludeDateTime)
            {
                uiSize += Table0.STIMESize;
            }

            // Season Index
            if (Table21.IncludeSeasonInfo)
            {
                uiSize += 1;
            }

            // Self Read register data.
            uiSize += RegisterDataRecord.Size(Table0, Table21);

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the sequence number for the Self Read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public ushort SequenceNumber
        {
            get
            {
                return m_usSequenceNumber;
            }
        }

        /// <summary>
        /// Gets the date and time the Self Read occurred.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public DateTime SelfReadDate
        {
            get
            {
                return m_dtSelfReadTime;
            }
        }

        /// <summary>
        /// Gets the season index when the Self Read occurred.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte SeasonIndex
        {
            get
            {
                return m_bySeasonIndex;
            }
        }

        /// <summary>
        /// Gets the registers for the Self Read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public RegisterDataRecord SelfReadRegisters
        {
            get
            {
                return m_SelfReadRegisters;
            }
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable21 m_Table21;

        private ushort m_usSequenceNumber;
        private DateTime m_dtSelfReadTime;
        private byte m_bySeasonIndex;
        private RegisterDataRecord m_SelfReadRegisters;

        #endregion
    }

    #endregion

    #region PresentDemandRecord
    
    /// <summary>
    /// Represents the Present Demand Data Record used by the 2's decade.
    /// </summary>
    public class PresentDemandRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal PresentDemandRecord(CTable00 Table0, StdTable21 Table21)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data to be parsed.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public void Parse(PSEMBinaryReader Reader)
        {
            if (m_Table21.IncludeTimeRemaining)
            {
                m_TimeRemaining = Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
            }

            m_DemandValue = Reader.ReadSingle();
        }

        /// <summary>
        /// Determines the size of the Present Demand record.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the record in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        internal static uint Size(CTable00 Table0, StdTable21 Table21)
        {
            uint uiSize = 0;

            if (Table21.IncludeTimeRemaining)
            {
                uiSize += Table0.TIMESize;
            }

            // Demand value is NI_FMAT2 (Float32)
            uiSize += 4;

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the amount of time remaining in the demand interval.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public TimeSpan TimeRemaining
        {
            get
            {
                return m_TimeRemaining;
            }
        }

        /// <summary>
        /// Gets the value of the present demand.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public float DemandValue
        {
            get
            {
                return m_DemandValue;
            }
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable21 m_Table21;

        private TimeSpan m_TimeRemaining;
        private float m_DemandValue;

        #endregion
    }

    #endregion

}
