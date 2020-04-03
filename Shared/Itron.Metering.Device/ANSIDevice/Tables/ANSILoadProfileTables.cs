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
//                              Copyright © 2006 - 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Globalization;

namespace Itron.Metering.Device
{
    #region Definitions

    /// <summary>
    /// Supported load profile formats.
    /// </summary>
    [Flags]
    public enum LPDataFormats : ushort
    {
        /// <summary>
        /// The data format is UINT8 (byte)
        /// </summary>
        UINT8 = 0x0001,
        /// <summary>
        /// The data format is UINT16 (ushort)
        /// </summary>
        UINT16 = 0x0002,
        /// <summary>
        /// The data format is UINT32 (uint)
        /// </summary>
        UINT32 = 0x0004,
        /// <summary>
        /// The data format is INT8 (sbyte)
        /// </summary>
        INT8 = 0x0008,
        /// <summary>
        /// The data format is INT16 (short)
        /// </summary>
        INT16 = 0x0010,
        /// <summary>
        /// The data format is INT32 (int)
        /// </summary>
        INT32 = 0x0020,
        /// <summary>
        /// The data format is NI_FMAT1 (double)
        /// </summary>
        NI_FMAT1 = 0x0040,
        /// <summary>
        /// The data format is NI_FMAT2 (float)
        /// </summary>
        NI_FMAT2 = 0x0080,
        /// <summary>
        /// The data format is UINT24
        /// </summary>
        UINT24 = 0x0100,
        /// <summary>
        /// The data format is INT24
        /// </summary>
        INT24 = 0x0200,
    }

    /// <summary>
    /// Tables used for each load profile data set.
    /// </summary>
    public enum LPDataSet : ushort
    {
        /// <summary>
        /// Table for set 1
        /// </summary>
        Set1 = 64,
        /// <summary>
        /// Table for set 2
        /// </summary>
        Set2 = 65,
        /// <summary>
        /// Table for set 3
        /// </summary>
        Set3 = 66,
        /// <summary>
        /// Table for set 4
        /// </summary>
        Set4 = 67,
    }

    /// <summary>
    /// The statuses for the extended interval status.
    /// </summary>
    [Flags]
    public enum ExtendedIntervalStatus : byte
    {
        /// <summary>
        /// A DST change occurred during this interval
        /// </summary>
        DSTChange = 0x10,
        /// <summary>
        /// A power failure occurred during this interval
        /// </summary>
        PowerFailure = 0x20,
        /// <summary>
        /// The clock was adjusted forward during this interval
        /// </summary>
        ClockAdjustForward = 0x40,
        /// <summary>
        /// The clock was adjusted backwards during this interval
        /// </summary>
        ClockAdjustBackward = 0x80,
    }

    /// <summary>
    /// The statuses for the extended channel status.
    /// </summary>
    public enum ExtendedChannelStatus : byte
    {
        /// <summary>
        /// No channel status
        /// </summary>
        None = 0x00,
        /// <summary>
        /// An overflow occurred on this channel
        /// </summary>
        Overflow = 0x01,
        /// <summary>
        /// The interval was too short.
        /// </summary>
        Partial = 0x02,
        /// <summary>
        /// The interval was too long.
        /// </summary>
        Long = 0x03,
        /// <summary>
        /// The interval was skipped.
        /// </summary>
        Skipped = 0x04,
        /// <summary>
        /// This was a test interval.
        /// </summary>
        Test = 0x05,
        /// <summary>
        /// The configuration changed during this interval
        /// </summary>
        ConfigurationChanged = 0x06,
        /// <summary>
        /// Load Profile stopped recording during this interval.
        /// </summary>
        LPStopped = 0x07,
        /// <summary>
        /// Power was restored during this interval
        /// </summary>
        PowerRestoration = 0x08,
    }

    #endregion

    #region StdTable61
    /// <summary>
    /// Table 61 - Actual Load Profile
    /// </summary>
    public class StdTable61 : AnsiTable
    {
        #region Constants
        /// <summary>
        /// The size in bytes of the header information for table 61
        /// </summary>
        protected const uint TABLE_61_HEADER_SIZE = 7;

        /// <summary>
        /// The size in bytes for the information stored for each set
        /// </summary>
        protected const uint TABLE_61_SET_INFO_SIZE = 6;

        // LP Flags bitfield masks
        private const ushort SET1_INHIBIT_OVERFLOW_MASK = 0x0001;
        private const ushort SET2_INHIBIT_OVERFLOW_MASK = 0x0002;
        private const ushort SET3_INHIBIT_OVERFLOW_MASK = 0x0004;
        private const ushort SET4_INHIBIT_OVERFLOW_MASK = 0x0008;
        private const ushort BLOCK_END_READ_MASK = 0x0010;
        private const ushort BLOCK_END_PULSE_MASK = 0x0020;
        private const ushort SET1_SCALAR_DIVISOR_MASK = 0x0040;
        private const ushort SET2_SCALAR_DIVISOR_MASK = 0x0080;
        private const ushort SET3_SCALAR_DIVISOR_MASK = 0x0100;
        private const ushort SET4_SCALAR_DIVISOR_MASK = 0x0200;
        private const ushort EXTENDED_INTERVAL_STATUS_MASK = 0x0400;
        private const ushort SIMPLE_INTERVAL_STATUS_MASK = 0x0800;
        private const ushort CLOSURE_STATUS_MASK = 0x1000;

        #endregion

        #region Public Methods
        /// <summary>
        /// Table 61 - Actual Load Profile Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table0">The Table 0 object for this meter.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public StdTable61(CPSEM psem, CTable00 Table0)
            : this(psem, Table0, 61, StdTable61.DetermineTableSize(Table0))
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The PSEM result from the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Table " + m_TableID.ToString(CultureInfo.InvariantCulture) + " Read");

            // Read the table 
            PSEMResponse Result = base.Read();
            m_DataStream.Position = 0;

            if (Result == PSEMResponse.Ok)
            {
                m_uiMemoryLength = m_Reader.ReadUInt32();
                m_usFlags = m_Reader.ReadUInt16();
                m_UsedFormats = (LPDataFormats)m_Reader.ReadByte();

                if (m_Table0.IsTableUsed(MapLPSetToTable(LPDataSet.Set1)) == true)
                {
                    m_Set1ActualLimits = new LPSetActualLimits();
                    m_Set1ActualLimits.Parse(m_Reader);
                    m_Set1ActualLimits.InhibitOverflow = InhibitSet1Overflow;
                    m_Set1ActualLimits.IncludeScalarDivisor = IncludeSet1ScalarDivisor;
                }
                else
                {
                    m_Set1ActualLimits = null;
                }

                if (m_Table0.IsTableUsed(MapLPSetToTable(LPDataSet.Set2)) == true)
                {
                    m_Set2ActualLimits = new LPSetActualLimits();
                    m_Set2ActualLimits.Parse(m_Reader);
                    m_Set2ActualLimits.InhibitOverflow = InhibitSet2Overflow;
                    m_Set2ActualLimits.IncludeScalarDivisor = IncludeSet2ScalarDivisor;
                }
                else
                {
                    m_Set2ActualLimits = null;
                }

                if (m_Table0.IsTableUsed(MapLPSetToTable(LPDataSet.Set3)) == true)
                {
                    m_Set3ActualLimits = new LPSetActualLimits();
                    m_Set3ActualLimits.Parse(m_Reader);
                    m_Set3ActualLimits.InhibitOverflow = InhibitSet3Overflow;
                    m_Set3ActualLimits.IncludeScalarDivisor = IncludeSet3ScalarDivisor;
                }
                else
                {
                    m_Set3ActualLimits = null;
                }

                if (m_Table0.IsTableUsed(MapLPSetToTable(LPDataSet.Set4)) == true)
                {
                    m_Set4ActualLimits = new LPSetActualLimits();
                    m_Set4ActualLimits.Parse(m_Reader);
                    m_Set4ActualLimits.InhibitOverflow = InhibitSet4Overflow;
                    m_Set4ActualLimits.IncludeScalarDivisor = IncludeSet4ScalarDivisor;
                }
                else
                {
                    m_Set4ActualLimits = null;
                }

                m_TableState = TableState.Loaded;

            }

            return Result;
        }

        /// <summary>
        /// Gets the set limits for the selected data set.
        /// </summary>
        /// <param name="DataSet">The data set to get.</param>
        /// <returns>The set limits for the selected data set.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 7.40.00 N/A    Created

        public LPSetActualLimits GetSetLimits(LPDataSet DataSet)
        {
            LPSetActualLimits SelectedSetLimit = null;

            switch (DataSet)
            {
                case LPDataSet.Set1:
                {
                    SelectedSetLimit = Set1ActualLimits;
                    break;
                }
                case LPDataSet.Set2:
                {
                    SelectedSetLimit = Set2ActualLimits;
                    break;
                }
                case LPDataSet.Set3:
                {
                    SelectedSetLimit = Set3ActualLimits;
                    break;
                }
                case LPDataSet.Set4:
                {
                    SelectedSetLimit = Set4ActualLimits;
                    break;
                }
            }

            return SelectedSetLimit;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the size of the Load Profile memory in bytes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public uint MemoryLength
        {
            get
            {
                ReadUnloadedTable();
                return m_uiMemoryLength;
            }
        }

        /// <summary>
        /// Gets the formats that are being used by load profile.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public LPDataFormats UsedFormats
        {
            get
            {
                ReadUnloadedTable();
                return m_UsedFormats;
            }
        }

        /// <summary>
        /// Gets whether or not block-end register reading information is included in a block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public bool IncludeBlockEndRead
        {
            get
            {
                ReadUnloadedTable();
                return (m_usFlags & BLOCK_END_READ_MASK) == BLOCK_END_READ_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not block-end pulse data is included in a block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public bool IncludeBlockEndPulse
        {
            get
            {
                ReadUnloadedTable();
                return (m_usFlags & BLOCK_END_PULSE_MASK) == BLOCK_END_PULSE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the extended interval status is included in the data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public bool IncludeExtendedIntervalStatus
        {
            get
            {
                ReadUnloadedTable();
                return (m_usFlags & EXTENDED_INTERVAL_STATUS_MASK) == EXTENDED_INTERVAL_STATUS_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the simple interval status is included in the data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public bool IncludeSimpleIntervalStatus
        {
            get
            {
                ReadUnloadedTable();
                return (m_usFlags & SIMPLE_INTERVAL_STATUS_MASK) == SIMPLE_INTERVAL_STATUS_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the block end readings are cumulative.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public bool IncludeClosureStatus
        {
            get
            {
                ReadUnloadedTable();
                return (m_usFlags & CLOSURE_STATUS_MASK) == CLOSURE_STATUS_MASK;
            }
        }

        /// <summary>
        /// Gets the limits for Set 1
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public LPSetActualLimits Set1ActualLimits
        {
            get
            {
                ReadUnloadedTable();
                return m_Set1ActualLimits;
            }
        }

        /// <summary>
        /// Gets the limits for Set 2
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public LPSetActualLimits Set2ActualLimits
        {
            get
            {
                ReadUnloadedTable();
                return m_Set2ActualLimits;
            }
        }

        /// <summary>
        /// Gets the limits for Set 3
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public LPSetActualLimits Set3ActualLimits
        {
            get
            {
                ReadUnloadedTable();
                return m_Set3ActualLimits;
            }
        }

        /// <summary>
        /// Gets the limits for Set 4
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        public LPSetActualLimits Set4ActualLimits
        {
            get
            {
                ReadUnloadedTable();
                return m_Set4ActualLimits;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Table 61 - Constructor that should be used by tables that inherit from this table
        /// </summary>
        /// <param name="psem">The PSEM Communications object</param>
        /// <param name="table0">The Table 0 object for the current device</param>
        /// <param name="tableID">The table number used to read that data</param>
        /// <param name="tableSize">The size of the table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected StdTable61(CPSEM psem, CTable00 table0, ushort tableID, uint tableSize)
            : base(psem, tableID, tableSize)
        {
            m_Table0 = table0;
        }

        /// <summary>
        /// Maps the Load Profile Data Set to the table number
        /// </summary>
        /// <param name="set">The data set to map</param>
        /// <returns>The table number for the specified data set</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        internal virtual ushort MapLPSetToTable(LPDataSet set)
        {
            return (ushort)set;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets whether or not scalars and devisors are included with set 1
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        protected bool IncludeSet1ScalarDivisor
        {
            get
            {
                return (m_usFlags & SET1_SCALAR_DIVISOR_MASK) == SET1_SCALAR_DIVISOR_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not scalars and devisors are included with set 2
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        protected bool IncludeSet2ScalarDivisor
        {
            get
            {
                return (m_usFlags & SET2_SCALAR_DIVISOR_MASK) == SET2_SCALAR_DIVISOR_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not scalars and devisors are included with set 3
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        protected bool IncludeSet3ScalarDivisor
        {
            get
            {
                return (m_usFlags & SET3_SCALAR_DIVISOR_MASK) == SET3_SCALAR_DIVISOR_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not scalars and devisors are included with set 4
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        protected bool IncludeSet4ScalarDivisor
        {
            get
            {
                return (m_usFlags & SET4_SCALAR_DIVISOR_MASK) == SET4_SCALAR_DIVISOR_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not set 1 is inhibiting on an overflow.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        protected bool InhibitSet1Overflow
        {
            get
            {
                return (m_usFlags & SET1_INHIBIT_OVERFLOW_MASK) == SET1_INHIBIT_OVERFLOW_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not set 2 is inhibiting on an overflow.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        protected bool InhibitSet2Overflow
        {
            get
            {
                return (m_usFlags & SET2_INHIBIT_OVERFLOW_MASK) == SET2_INHIBIT_OVERFLOW_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not set 3 is inhibiting on an overflow.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        protected bool InhibitSet3Overflow
        {
            get
            {
                return (m_usFlags & SET3_INHIBIT_OVERFLOW_MASK) == SET3_INHIBIT_OVERFLOW_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not set 4 is inhibiting on an overflow.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/26/08 RCG 2.00.00 N/A    Created

        protected bool InhibitSet4Overflow
        {
            get
            {
                return (m_usFlags & SET4_INHIBIT_OVERFLOW_MASK) == SET4_INHIBIT_OVERFLOW_MASK;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the size of table 61 based on the values in table 0
        /// </summary>
        /// <param name="Table0">The Table 0 object.</param>
        /// <returns>The size of table 61.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0)
        {
            uint uiTableSize = TABLE_61_HEADER_SIZE;

            if (Table0.IsTableUsed((ushort)LPDataSet.Set1) == true)
            {
                // Table Set 1 is used
                uiTableSize += TABLE_61_SET_INFO_SIZE;
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set2) == true)
            {
                // Table Set 2 is used
                uiTableSize += TABLE_61_SET_INFO_SIZE;
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set3) == true)
            {
                // Table Set 3 is used
                uiTableSize += TABLE_61_SET_INFO_SIZE;
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set4) == true)
            {
                // Table Set 4 is used
                uiTableSize += TABLE_61_SET_INFO_SIZE;
            }

            return uiTableSize;
        }

        #endregion

        #region Member Variables
        /// <summary>Table 0 object</summary>
        protected CTable00 m_Table0;

        /// <summary>Memory Length</summary>
        protected uint m_uiMemoryLength;
        /// <summary>Flags</summary>
        protected ushort m_usFlags;
        /// <summary>Data formats</summary>
        protected LPDataFormats m_UsedFormats;

        /// <summary>Data Set 1 Limits</summary>
        protected LPSetActualLimits m_Set1ActualLimits;
        /// <summary>Data Set 2 Limits</summary>
        protected LPSetActualLimits m_Set2ActualLimits;
        /// <summary>Data Set 3 Limits</summary>
        protected LPSetActualLimits m_Set3ActualLimits;
        /// <summary>Data Set 4 Limits</summary>
        protected LPSetActualLimits m_Set4ActualLimits;

        #endregion
    }

    #endregion

    #region StdTable62

    /// <summary>
    /// Standard Table 62 - Load Profile Control Table
    /// </summary>
    public class StdTable62 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="Table61">The Table 61 object for the current device.</param>
        /// <param name="Table14">The Table 14 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public StdTable62(CPSEM psem, CTable00 Table0, StdTable61 Table61, StdTable14 Table14)
            : this(psem, Table0, Table61, Table14, 62, DetermineTableSize(Table0, Table61))
        {
            m_UsesTwoByteFormat = false;
        }

        /// <summary>
        /// Reads the table from the device.
        /// </summary>
        /// <returns>The result of the read operation.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable62.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Gets the data slection object for the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set to get.</param>
        /// <returns>The data selection object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        public LPSetDataSelection GetDataSelection(LPDataSet dataSet)
        {
            LPSetDataSelection SelectedDataSet = null;

            switch (dataSet)
            {
                case LPDataSet.Set1:
                {
                    SelectedDataSet = Set1DataSelection;
                    break;
                }
                case LPDataSet.Set2:
                {
                    SelectedDataSet = Set2DataSelection;
                    break;
                }
                case LPDataSet.Set3:
                {
                    SelectedDataSet = Set3DataSelection;
                    break;
                }
                case LPDataSet.Set4:
                {
                    SelectedDataSet = Set4DataSelection;
                    break;
                }
            }

            return SelectedDataSet;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the data selection object for Set 1
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        public LPSetDataSelection Set1DataSelection
        {
            get
            {
                ReadUnloadedTable();
                return m_Set1Selection;
            }
        }

        /// <summary>
        /// Gets the data selection object for Set 2
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        public LPSetDataSelection Set2DataSelection
        {
            get
            {
                ReadUnloadedTable();
                return m_Set2Selection;
            }
        }

        /// <summary>
        /// Gets the data selection object for Set 3
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        public LPSetDataSelection Set3DataSelection
        {
            get
            {
                ReadUnloadedTable();
                return m_Set3Selection;
            }
        }

        /// <summary>
        /// Gets the data selection object for Set 4
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        public LPSetDataSelection Set4DataSelection
        {
            get
            {
                ReadUnloadedTable();
                return m_Set4Selection;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Table 62 - Constructor that should be used by tables that inherit from this table
        /// </summary>
        /// <param name="psem">The PSEM Communications object</param>
        /// <param name="table0">The Table 0 object for the current device</param>
        /// <param name="table61">The Load Profile Actual Limiting table for the current device</param>
        /// <param name="table14">The Table 14 object for the current device</param>
        /// <param name="tableID">The table number used to read that data</param>
        /// <param name="tableSize">The size of the table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected StdTable62(CPSEM psem, CTable00 table0, StdTable61 table61, StdTable14 table14, ushort tableID, uint tableSize)
            : base(psem, tableID, tableSize)
        {
            m_Table0 = table0;
            m_Table61 = table61;
            m_Table14 = table14;
        }

        /// <summary>
        /// Determines the source LID for each of the channels.
        /// </summary>
        /// <returns>The list of channel names.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected uint[] GetChannelLIDs(LPDataSet dataSet)
        {
            uint[] ChannelLids = null;
            LPSetActualLimits setLimits = m_Table61.GetSetLimits(dataSet);
            LPSetDataSelection setDataSelection = GetDataSelection(dataSet);

            if (setDataSelection != null && setLimits != null)
            {
                ChannelLids = new uint[setLimits.NumberOfChannels];

                for (int iChannel = 0; iChannel < setLimits.NumberOfChannels; iChannel++)
                {
                    uint SourceID = m_Table14.SourceIDs[setDataSelection.SourceSelections[iChannel]];
                    ChannelLids[iChannel] = SourceID;
                }
            }

            return ChannelLids;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="Table61">The Table 61 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, StdTable61 Table61)
        {
            uint uiTableSize = 0;
            LPSetActualLimits SetLimits;

            if (Table0.IsTableUsed((ushort)LPDataSet.Set1))
            {
                SetLimits = Table61.Set1ActualLimits;
                uiTableSize += LPSetDataSelection.Size(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, false);
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set2))
            {
                SetLimits = Table61.Set2ActualLimits;
                uiTableSize += LPSetDataSelection.Size(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, false);
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set3))
            {
                SetLimits = Table61.Set3ActualLimits;
                uiTableSize += LPSetDataSelection.Size(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, false);
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set4))
            {
                SetLimits = Table61.Set4ActualLimits;
                uiTableSize += LPSetDataSelection.Size(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, false);
            }

            return uiTableSize;
        }

        /// <summary>
        /// Parses the data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created
        // 12/06/11 RCG 2.53.20 N/A    Modified to add support for Extended LP and IP

        private void ParseData()
        {
            LPSetActualLimits SetLimits;

            if (m_Table0.IsTableUsed(m_Table61.MapLPSetToTable(LPDataSet.Set1)))
            {
                SetLimits = m_Table61.Set1ActualLimits;
                m_Set1Selection = new LPSetDataSelection(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, m_UsesTwoByteFormat);
                m_Set1Selection.Parse(m_Reader);
                m_Set1Selection.SourceLIDs = GetChannelLIDs(LPDataSet.Set1);
            }
            else
            {
                m_Set1Selection = null;
            }

            if (m_Table0.IsTableUsed(m_Table61.MapLPSetToTable(LPDataSet.Set2)))
            {
                SetLimits = m_Table61.Set2ActualLimits;
                m_Set2Selection = new LPSetDataSelection(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, m_UsesTwoByteFormat);
                m_Set2Selection.Parse(m_Reader);
                m_Set2Selection.SourceLIDs = GetChannelLIDs(LPDataSet.Set2);
            }
            else
            {
                m_Set2Selection = null;
            }

            if (m_Table0.IsTableUsed(m_Table61.MapLPSetToTable(LPDataSet.Set3)))
            {
                SetLimits = m_Table61.Set3ActualLimits;
                m_Set3Selection = new LPSetDataSelection(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, m_UsesTwoByteFormat);
                m_Set3Selection.Parse(m_Reader);
                m_Set3Selection.SourceLIDs = GetChannelLIDs(LPDataSet.Set3);
            }
            else
            {
                m_Set3Selection = null;
            }

            if (m_Table0.IsTableUsed(m_Table61.MapLPSetToTable(LPDataSet.Set4)))
            {
                SetLimits = m_Table61.Set4ActualLimits;
                m_Set4Selection = new LPSetDataSelection(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, m_UsesTwoByteFormat);
                m_Set4Selection.Parse(m_Reader);
                m_Set4Selection.SourceLIDs = GetChannelLIDs(LPDataSet.Set4);
            }
            else
            {
                m_Set4Selection = null;
            }
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable61 m_Table61;
        /// <summary>
        /// Table 14 object for the current device.
        /// </summary>
        protected StdTable14 m_Table14;
        /// <summary>
        /// Whether or not the table uses a two byte data format
        /// </summary>
        protected bool m_UsesTwoByteFormat;


        private LPSetDataSelection m_Set1Selection;
        private LPSetDataSelection m_Set2Selection;
        private LPSetDataSelection m_Set3Selection;
        private LPSetDataSelection m_Set4Selection;

        #endregion
    }

    #endregion

    #region StdTable63
    /// <summary>
    /// Table 63 - Load Profile Status
    /// </summary>
    public class StdTable63 : AnsiTable
    {
        #region Constants
        /// <summary>
        /// The size of a Load Profile Set Status Record
        /// </summary>
        protected const uint LP_SET_STATUS_RECORD_SIZE = 13;
        private const int TABLE_TIMEOUT = 1000;
        #endregion

        #region Public Methods
        /// <summary>
        /// Table 63 - Load Profile Status Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table0">The table 0 object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created
        // 12/06/11 RCG 2.53.20 N/A    Modified to add support for Extended LP and IP

        public StdTable63(CPSEM psem, CTable00 Table0)
            : this(psem, Table0, 63, DetermineTableSize(Table0))
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created
        // 12/06/11 RCG 2.53.20 N/A    Modified to add support for Extended LP and IP

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable63.Read");

            // Read the table 
            PSEMResponse Result = base.Read();
            m_DataStream.Position = 0;

            if (Result == PSEMResponse.Ok)
            {

                if (m_Table0.IsTableUsed(MapLPSetToTable(LPDataSet.Set1)) == true)
                {
                    m_Set1StatusRecord = new LPSetStatusRecord();
                    m_Set1StatusRecord.Parse(m_Reader);
                }
                else
                {
                    m_Set1StatusRecord = null;
                }

                if (m_Table0.IsTableUsed(MapLPSetToTable(LPDataSet.Set2)) == true)
                {
                    m_Set2StatusRecord = new LPSetStatusRecord();
                    m_Set2StatusRecord.Parse(m_Reader);
                }
                else
                {
                    m_Set2StatusRecord = null;
                }

                if (m_Table0.IsTableUsed(MapLPSetToTable(LPDataSet.Set3)) == true)
                {
                    m_Set3StatusRecord = new LPSetStatusRecord();
                    m_Set3StatusRecord.Parse(m_Reader);
                }
                else
                {
                    m_Set3StatusRecord = null;
                }

                if (m_Table0.IsTableUsed(MapLPSetToTable(LPDataSet.Set4)) == true)
                {
                    m_Set4StatusRecord = new LPSetStatusRecord();
                    m_Set4StatusRecord.Parse(m_Reader);
                }
                else
                {
                    m_Set4StatusRecord = null;
                }

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        /// <summary>
        /// Gets the status record for the specified set.
        /// </summary>
        /// <param name="dataSet">The set to get the status for.</param>
        /// <returns>The requested sets status record.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public LPSetStatusRecord GetSetStatusRecord(LPDataSet dataSet)
        {
            LPSetStatusRecord SelectedSetStatus = null;

            switch (dataSet)
            {
                case LPDataSet.Set1:
                {
                    SelectedSetStatus = Set1StatusRecord;
                    break;
                }
                case LPDataSet.Set2:
                {
                    SelectedSetStatus = Set2StatusRecord;
                    break;
                }
                case LPDataSet.Set3:
                {
                    SelectedSetStatus = Set3StatusRecord;
                    break;
                }
                case LPDataSet.Set4:
                {
                    SelectedSetStatus = Set4StatusRecord;
                    break;
                }
            }
            return SelectedSetStatus;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status record for set 1
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public LPSetStatusRecord Set1StatusRecord
        {
            get
            {
                ReadUnloadedTable();
                return m_Set1StatusRecord;
            }
        }

        /// <summary>
        /// Gets the status record for set 2
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public LPSetStatusRecord Set2StatusRecord
        {
            get
            {
                ReadUnloadedTable();
                return m_Set2StatusRecord;
            }
        }

        /// <summary>
        /// Gets the status record for set 3
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public LPSetStatusRecord Set3StatusRecord
        {
            get
            {
                ReadUnloadedTable();
                return m_Set3StatusRecord;
            }
        }

        /// <summary>
        /// Gets the status record for set 4
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public LPSetStatusRecord Set4StatusRecord
        {
            get
            {
                ReadUnloadedTable();
                return m_Set4StatusRecord;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor that should only be used by classes that inherit from this table
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table0">The Table 0 object for the current device</param>
        /// <param name="tableID">The Table ID for the table to read</param>
        /// <param name="tableSize">The size of the table to read</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected StdTable63(CPSEM psem, CTable00 table0, ushort tableID, uint tableSize)
            : base(psem, tableID, tableSize, TABLE_TIMEOUT)
        {
            m_Table0 = table0;

            m_Set1StatusRecord = null;
            m_Set2StatusRecord = null;
            m_Set3StatusRecord = null;
            m_Set4StatusRecord = null;
        }

        /// <summary>
        /// Maps the Load Profile Data Set to the table number
        /// </summary>
        /// <param name="set">The data set to map</param>
        /// <returns>The table number for the specified data set</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        internal virtual ushort MapLPSetToTable(LPDataSet set)
        {
            return (ushort)set;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of table 63
        /// </summary>
        /// <param name="Table0">The table 0 object.</param>
        /// <returns>The size of the table.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0)
        {
            uint uiTableSize = 0;

            if (Table0.IsTableUsed((ushort)LPDataSet.Set1) == true)
            {
                uiTableSize += LP_SET_STATUS_RECORD_SIZE;
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set2) == true)
            {
                uiTableSize += LP_SET_STATUS_RECORD_SIZE;
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set3) == true)
            {
                uiTableSize += LP_SET_STATUS_RECORD_SIZE;
            }

            if (Table0.IsTableUsed((ushort)LPDataSet.Set4) == true)
            {
                uiTableSize += LP_SET_STATUS_RECORD_SIZE;
            }

            return uiTableSize;
        }

        #endregion

        #region Member Variables
        private CTable00 m_Table0;

        private LPSetStatusRecord m_Set1StatusRecord;
        private LPSetStatusRecord m_Set2StatusRecord;
        private LPSetStatusRecord m_Set3StatusRecord;
        private LPSetStatusRecord m_Set4StatusRecord;

        #endregion
    }

    #endregion

    #region StdTable64

    /// <summary>
    /// Standard Table 64 - Load Profile Data Set 1
    /// </summary>
    public class StdTable64 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table61">The table 61 object for the current device.</param>
        /// <param name="Table62">The table 62 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created
        // 12/06/11 RCG 2.53.20 N/A    Modified to add support for Extended LP and IP

        public StdTable64(CPSEM psem, CTable00 Table0, StdTable61 Table61, StdTable62 Table62)
            : this(psem, 64, DetermineTableSize(Table0, Table61, Table62), Table0, Table61, Table62, LPDataSet.Set1)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public override PSEMResponse Read()
        {
            throw new NotSupportedException("This table does not support full reads.");
        }

        /// <summary>
        /// Reads a single block from the meter.
        /// </summary>
        /// <param name="blockToRead">The block that should be read.</param>
        /// <param name="validIntervals">The number of valid intervals in the block</param>
        /// <param name="blockData">The block read from the meter.</param>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created
        // 12/06/11 RCG 2.53.20 N/A    Modified to add support for Extended LP and IP

        public virtual PSEMResponse ReadBlock(ushort blockToRead, ushort validIntervals, out LPBlockDataRecord blockData)
        {
            LPBlockDataRecord ReadBlock = null;
            PSEMResponse Response = PSEMResponse.Err;
            int iBlockOffset;
            ushort usBlockLength;
            LPSetActualLimits SetLimits = m_Table61.GetSetLimits(m_DataSet);

            if (SetLimits != null)
            {
                if (blockToRead >= 0 && blockToRead < SetLimits.NumberOfBlocks)
                {
                    // Determine the data that needs to be read.
                    usBlockLength = (ushort)LPBlockDataRecord.Size(m_Table0, m_Table61, m_Table62, m_DataSet);
                    iBlockOffset = blockToRead * usBlockLength;

                    Response = Read(iBlockOffset, usBlockLength);

                    if (Response == PSEMResponse.Ok)
                    {
                        ReadBlock = new LPBlockDataRecord(m_Table0, m_Table61, m_Table62, validIntervals, m_DataSet);
                        ReadBlock.Parse(m_Reader);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("blockToRead", "Invalid block requested.");
                }
            }
            else
            {
                throw new InvalidOperationException("Data set not supported");
            }

            blockData = ReadBlock;
            return Response;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the data set that applies to this table
        /// </summary>
        public LPDataSet DataSet
        {
            get
            {
                return m_DataSet;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Protected constructor that will only be used by the Load Profile data sets that
        /// inherit from this class.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="usTableID">The table ID to use for reading.</param>
        /// <param name="size">The size of the table.</param>
        /// <param name="table0">The table 0 object for the current device.</param>
        /// <param name="table61">The table 61 object for the current device.</param>
        /// <param name="table62">The table 62 object for the current device.</param>
        /// <param name="dataSet">The data set that applies to this table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        protected StdTable64(CPSEM psem, ushort usTableID, uint size, CTable00 table0, StdTable61 table61, StdTable62 table62, LPDataSet dataSet)
            : base(psem, usTableID, size)
        {
            m_Table61 = table61;
            m_Table62 = table62;
            m_Table0 = table0;
            m_DataSet = dataSet;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="Table61">The Table 61 object for the current device.</param>
        /// <param name="Table62">The table 62 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, StdTable61 Table61, StdTable62 Table62)
        {
            uint uiTableSize = 0;

            uiTableSize += Table61.Set1ActualLimits.NumberOfBlocks
                * LPBlockDataRecord.Size(Table0, Table61, Table62, LPDataSet.Set1);

            return uiTableSize;
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// Table 0 object for the current device.
        /// </summary>
        protected CTable00 m_Table0;
        /// <summary>
        /// Table 61 object for the current device.
        /// </summary>
        protected StdTable61 m_Table61;
        /// <summary>
        /// Table 62 object for the current device.
        /// </summary>
        protected StdTable62 m_Table62;

        /// <summary>
        /// The Data set for the current table.
        /// </summary>
        protected LPDataSet m_DataSet;

        #endregion
    }

    #endregion

    #region StdTable65

    /// <summary>
    /// Standard Table 65 - Load Profile Data Set 2
    /// </summary>
    public class StdTable65 : StdTable64
    {
        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table61">The table 61 object for the current device.</param>
        /// <param name="Table62">The table 62 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public StdTable65(CPSEM psem, CTable00 Table0, StdTable61 Table61, StdTable62 Table62)
            : base(psem, 65, DetermineTableSize(Table0, Table61, Table62), Table0, Table61, Table62, LPDataSet.Set2)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="Table61">The Table 61 object for the current device.</param>
        /// <param name="Table62"> The Table 62 object for the current device</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, StdTable61 Table61, StdTable62 Table62)
        {
            uint uiTableSize = 0;

            uiTableSize += Table61.Set2ActualLimits.NumberOfBlocks
                * LPBlockDataRecord.Size(Table0, Table61, Table62, LPDataSet.Set2);

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region StdTable66

    /// <summary>
    /// Standard Table 66 - Load Profile Data Set 3
    /// </summary>
    public class StdTable66 : StdTable64
    {
        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table61">The table 61 object for the current device.</param>
        /// <param name="Table62">The table 62 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public StdTable66(CPSEM psem, CTable00 Table0, StdTable61 Table61, StdTable62 Table62)
            : base(psem, 66, DetermineTableSize(Table0, Table61, Table62), Table0, Table61, Table62, LPDataSet.Set3)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="Table61">The Table 61 object for the current device.</param>
        /// <param name="Table62"> The Table 62 object for the current device</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, StdTable61 Table61, StdTable62 Table62)
        {
            uint uiTableSize = 0;

            uiTableSize += Table61.Set3ActualLimits.NumberOfBlocks
                * LPBlockDataRecord.Size(Table0, Table61, Table62, LPDataSet.Set3);

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region StdTable67

    /// <summary>
    /// Standard Table 67 - Load Profile Data Set 4
    /// </summary>
    public class StdTable67 : StdTable64
    {
        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table61">The table 61 object for the current device.</param>
        /// <param name="Table62">The table 62 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public StdTable67(CPSEM psem, CTable00 Table0, StdTable61 Table61, StdTable62 Table62)
            : base(psem, 67, DetermineTableSize(Table0, Table61, Table62), Table0, Table61, Table62, LPDataSet.Set4)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="Table61">The Table 61 object for the current device.</param>
        /// <param name="Table62"> The Table 62 object for the current device</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, StdTable61 Table61, StdTable62 Table62)
        {
            uint uiTableSize = 0;

            uiTableSize += Table61.Set4ActualLimits.NumberOfBlocks
                * LPBlockDataRecord.Size(Table0, Table61, Table62, LPDataSet.Set4);

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region LPSetActualLimits

    /// <summary>
    /// Represent the actual limits of a Load Profile set.
    /// </summary>
    public class LPSetActualLimits
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public LPSetActualLimits()
        {
        }

        /// <summary>
        /// Parses the data from the binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public void Parse(PSEMBinaryReader Reader)
        {
            m_NumberOfBlocks = Reader.ReadUInt16();
            m_IntervalsPerBlock = Reader.ReadUInt16();
            m_NumberOfChannels = Reader.ReadByte();
            m_IntervalLength = Reader.ReadByte();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the maximum number of blocks in the set.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public ushort NumberOfBlocks
        {
            get
            {
                return m_NumberOfBlocks;
            }
        }

        /// <summary>
        /// Gets the number of intervals per block in the set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public ushort IntervalsPerBlock
        {
            get
            {
                return m_IntervalsPerBlock;
            }
        }

        /// <summary>
        /// Gets the number of channels in the set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public byte NumberOfChannels
        {
            get
            {
                return m_NumberOfChannels;
            }
        }

        /// <summary>
        /// Gets the interval length of the set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public byte IntervalLength
        {
            get
            {
                return m_IntervalLength;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the set can inhibit overflow.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public bool InhibitOverflow
        {
            get
            {
                return m_InhibitOverFlow;
            }
            internal set
            {
                m_InhibitOverFlow = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the scalar and divisor is included.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public bool IncludeScalarDivisor
        {
            get
            {
                return m_IncludeScalarDivisor;
            }
            internal set
            {
                m_IncludeScalarDivisor = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_NumberOfBlocks;
        private ushort m_IntervalsPerBlock;
        private byte m_NumberOfChannels;
        private byte m_IntervalLength;
        private bool m_InhibitOverFlow;
        private bool m_IncludeScalarDivisor;

        #endregion
    }

    #endregion

    #region LPSetDataSelection

    /// <summary>
    /// Load Profile Data Selection used in table 62
    /// </summary>
    public class LPSetDataSelection
    {
        #region Constants

        private const byte CHANNEL_END_READING_MASK = 0x01;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numberOfChannels">The number of channels in the set.</param>
        /// <param name="includeScalarDivisor">Whether or not the set includes the scalar and divisor information</param>
        /// <param name="usesTwoByteFormat">Whether or not that table uses a two byte data format</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        public LPSetDataSelection(byte numberOfChannels, bool includeScalarDivisor, bool usesTwoByteFormat)
        {
            m_NumberOfChannels = numberOfChannels;
            m_IncludeScalarDivisor = includeScalarDivisor;

            m_ChannelFlags = null;
            m_SourceSelections = null;
            m_EndReadingSourceSelections = null;
            m_DataFormat = null;
            m_Scalars = null;
            m_Divisors = null;
            m_SourceLIDs = null;
            m_UsesTwoByteFormat = usesTwoByteFormat;
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="reader">The binary reader that contains the set's data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        public void Parse(PSEMBinaryReader reader)
        {
            int iIndex;
            byte FormatLSB = 0;
            byte FormatMSB = 0;

            // Channel source selections
            m_ChannelFlags = new byte[m_NumberOfChannels];
            m_SourceSelections = new byte[m_NumberOfChannels];
            m_EndReadingSourceSelections = new byte[m_NumberOfChannels];

            for (iIndex = 0; iIndex < m_NumberOfChannels; iIndex++)
            {
                m_ChannelFlags[iIndex] = reader.ReadByte();
                m_SourceSelections[iIndex] = reader.ReadByte();
                m_EndReadingSourceSelections[iIndex] = reader.ReadByte();
            }

            // Data format
            FormatLSB = reader.ReadByte();

            if (m_UsesTwoByteFormat)
            {
                FormatMSB = reader.ReadByte();
            }

            m_DataFormat = (LPDataFormats)(FormatMSB << 8 | FormatLSB);

            // Scalar and Divisor
            if (m_IncludeScalarDivisor)
            {
                m_Scalars = new ushort[m_NumberOfChannels];
                m_Divisors = new ushort[m_NumberOfChannels];

                for (iIndex = 0; iIndex < m_NumberOfChannels; iIndex++)
                {
                    m_Scalars[iIndex] = reader.ReadUInt16();
                }

                for (iIndex = 0; iIndex < m_NumberOfChannels; iIndex++)
                {
                    m_Divisors[iIndex] = reader.ReadUInt16();
                }
            }
        }

        /// <summary>
        /// Determines the size of the data selection data for a single set.
        /// </summary>
        /// <param name="numberOfChannels">The number of channels for the set.</param>
        /// <param name="includeScalarDivisor">Whether or not scalar and divisor information is included.</param>
        /// <param name="usesTwoByteFormat">Whether or not the table uses a two byte data format</param>
        /// <returns>The size of the set's selection data.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        public static uint Size(byte numberOfChannels, bool includeScalarDivisor, bool usesTwoByteFormat)
        {
            uint uiSize = 0;

            // Source selection records
            uiSize += (uint)(numberOfChannels * 3);

            // Format
            uiSize += 1;

            if (usesTwoByteFormat)
            {
                uiSize += 1;
            }

            // Scalar and Divisor
            if (includeScalarDivisor)
            {
                uiSize += (uint)(numberOfChannels * 4);
            }

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not each channel in the set has an end reading.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created

        public bool[] HasChannelEndReading
        {
            get
            {
                bool[] HasEndReading = new bool[m_NumberOfChannels];

                for (int iIndex = 0; iIndex < m_NumberOfChannels; iIndex++)
                {
                    HasEndReading[iIndex] = (m_ChannelFlags[iIndex] & CHANNEL_END_READING_MASK)
                        == CHANNEL_END_READING_MASK;
                }

                return HasEndReading;
            }
        }

        /// <summary>
        /// Gets the data source index for each channel
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created

        public byte[] SourceSelections
        {
            get
            {
                return m_SourceSelections;
            }
        }

        /// <summary>
        /// Gets the list of LIDs that apply to the sources for each channel
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/13/11 RCG 2.53.20 N/A    Created

        public uint[] SourceLIDs
        {
            get
            {
                return m_SourceLIDs;
            }
            internal set
            {
                m_SourceLIDs = value;
            }
        }

        /// <summary>
        /// Gets the end reading data source index for each channel
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created

        public byte[] EndReadingSourceSelections
        {
            get
            {
                return m_EndReadingSourceSelections;
            }
        }

        /// <summary>
        /// Gets the data format used
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created

        public LPDataFormats? DataFormat
        {
            get
            {
                return m_DataFormat;
            }
        }

        /// <summary>
        /// Gets the scalar values for each channel
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created

        public ushort[] Scalars
        {
            get
            {
                return m_Scalars;
            }
        }

        /// <summary>
        /// Gets the divisor values for each channel
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/08 RCG 2.00.00 N/A    Created

        public ushort[] Divisors
        {
            get
            {
                return m_Divisors;
            }
        }

        #endregion

        #region Member Variables

        private byte m_NumberOfChannels;
        private bool m_IncludeScalarDivisor;
        private bool m_UsesTwoByteFormat;

        private byte[] m_ChannelFlags;
        private byte[] m_SourceSelections;
        private byte[] m_EndReadingSourceSelections;
        private LPDataFormats? m_DataFormat;
        private ushort[] m_Scalars;
        private ushort[] m_Divisors;
        private uint[] m_SourceLIDs;

        #endregion
    }

    #endregion

    #region LPSetStatusRecord Class
    /// <summary>
    /// Contains data that describes the current status of a Load Profile data set
    /// </summary>
    public class LPSetStatusRecord
    {
        #region Constants
        private const byte BLOCK_ORDER_MASK = 0x01;
        private const byte OVERFLOW_MASK = 0x02;
        private const byte LIST_TYPE_MASK = 0x04;
        private const byte BLOCK_INHIBIT_OVERFLOW_MASK = 0x08;
        private const byte INTERVAL_ORDER_MASK = 0x10;
        private const byte ACTIVE_MODE_MASK = 0x20;
        private const byte TEST_MODE_MASK = 0x40;

        #endregion

        #region Definitions

        /// <summary>
        /// Specifies the block order in memory
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public enum BlockOrder
        {
            /// <summary>
            /// The blocks are in ascending order in memory
            /// </summary>
            Ascending = 0,
            /// <summary>
            /// The blocks are in descending order in memory
            /// </summary>
            Descending = 1,
        }

        /// <summary>
        /// Specifies the interval order in a block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public enum IntervalOrder
        {
            /// <summary>
            /// The intervals are in ascending order in a block
            /// </summary>
            Ascending = 0,
            /// <summary>
            /// The intervals are in descending order in a block
            /// </summary>
            Descending = 1,
        }

        /// <summary>
        /// The list type for the data stored in memory
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public enum ListType
        {
            /// <summary>
            /// The blocks are stored in first in first out order
            /// </summary>
            FIFO = 0,
            /// <summary>
            /// The blocks are stored circularly
            /// </summary>
            Circular = 1,
        }

        /// <summary>
        /// The mode for the current data set
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// The meter is in Normal mode
            /// </summary>
            Normal = 0,
            /// <summary>
            /// The meter is in Test mode
            /// </summary>
            Test = 1,
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Default Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public LPSetStatusRecord()
        {
        }

        /// <summary>
        /// Parse the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data from the device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public void Parse(PSEMBinaryReader Reader)
        {
            m_bySetStatusFlags = Reader.ReadByte();
            m_usNumberOfValidBlocks = Reader.ReadUInt16();
            m_usLastBlockElement = Reader.ReadUInt16();
            m_uiLastBlockSequenceNumber = Reader.ReadUInt32();
            m_usNumberOfUnreadBlocks = Reader.ReadUInt16();
            m_usNumberOfValidIntervals = Reader.ReadUInt16();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the block order for the data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public BlockOrder DataBlockOrder
        {
            get
            {
                BlockOrder Order;

                if ((m_bySetStatusFlags & BLOCK_ORDER_MASK) == 0)
                {
                    Order = BlockOrder.Ascending;
                }
                else
                {
                    Order = BlockOrder.Descending;
                }

                return Order;
            }
        }

        /// <summary>
        /// Gets whether or not the data set has overflown
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public bool HasOverflown
        {
            get
            {
                return (m_bySetStatusFlags & OVERFLOW_MASK) == OVERFLOW_MASK;
            }
        }

        /// <summary>
        /// Gets the list type for the data set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public ListType DataListType
        {
            get
            {
                ListType Type;

                if ((m_bySetStatusFlags & LIST_TYPE_MASK) == 0)
                {
                    Type = ListType.FIFO;
                }
                else
                {
                    Type = ListType.Circular;
                }

                return Type;
            }
        }

        /// <summary>
        /// Gets whether or not the device is capable of inhibiting overflow for the data set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public bool BlockInhibitOverflow
        {
            get
            {
                return (m_bySetStatusFlags & BLOCK_INHIBIT_OVERFLOW_MASK) == BLOCK_INHIBIT_OVERFLOW_MASK;
            }
        }

        /// <summary>
        /// Gets the interval order for the data set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public IntervalOrder DataIntervalOrder
        {
            get
            {
                IntervalOrder Order;

                if ((m_bySetStatusFlags & INTERVAL_ORDER_MASK) == 0)
                {
                    Order = IntervalOrder.Ascending;
                }
                else
                {
                    Order = IntervalOrder.Descending;
                }

                return Order;
            }
        }

        /// <summary>
        /// Gets whether or not the current data set is collecting data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public bool IsActive
        {
            get
            {
                return (m_bySetStatusFlags & ACTIVE_MODE_MASK) == ACTIVE_MODE_MASK;
            }
        }

        /// <summary>
        /// Gets the mode of the data set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public Mode DataMode
        {
            get
            {
                Mode TestMode;

                if ((m_bySetStatusFlags & TEST_MODE_MASK) == 0)
                {
                    TestMode = Mode.Normal;
                }
                else
                {
                    TestMode = Mode.Test;
                }

                return TestMode;
            }
        }

        /// <summary>
        /// Gets the number of valid blocks
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public ushort NumberOfValidBlocks
        {
            get
            {
                return m_usNumberOfValidBlocks;
            }
        }

        /// <summary>
        /// Gets the array element of the newest valid data block in the data set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public ushort LastBlockElement
        {
            get
            {
                return m_usLastBlockElement;
            }
        }

        /// <summary>
        /// Gets the sequence number of the last element in the data set
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public uint LastBlockSequenceNumber
        {
            get
            {
                return m_uiLastBlockSequenceNumber;
            }
        }

        /// <summary>
        /// Gets the number of unread blocks in the data set.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public ushort NumberOfUnreadBlocks
        {
            get
            {
                return m_usNumberOfUnreadBlocks;
            }
        }

        /// <summary>
        /// Gets the number of valid intervals in the last block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public ushort NumberOfValidIntervals
        {
            get
            {
                return m_usNumberOfValidIntervals;
            }
        }

        #endregion

        #region Member Variables
        private byte m_bySetStatusFlags;
        private ushort m_usNumberOfValidBlocks;
        private ushort m_usLastBlockElement;
        private uint m_uiLastBlockSequenceNumber;
        private ushort m_usNumberOfUnreadBlocks;
        private ushort m_usNumberOfValidIntervals;

        #endregion
    }

    #endregion

    #region LPBlockDataRecord

    /// <summary>
    /// The load profile data block object.
    /// </summary>
    public class LPBlockDataRecord
    {
        #region Definitions

        /// <summary>
        /// Block closure statuses.
        /// </summary>
        public enum ClosureStatus : ushort
        {
            /// <summary>
            /// The block has not been closed.
            /// </summary>
            BlockNotClosed = 0,
            /// <summary>
            /// The block closed normally.
            /// </summary>
            NormalClosure = 1,
            /// <summary>
            /// The block was closed for an unspecified reason.
            /// </summary>
            Unspecified = 2,
            /// <summary>
            /// The block was closed due to an overflow.
            /// </summary>
            OverflowDetected = 3,
            /// <summary>
            /// The block was closed due to test mode.
            /// </summary>
            TestMode = 4,
            /// <summary>
            /// The block was closed due to a power failure.
            /// </summary>
            PowerFailure = 5,
            /// <summary>
            /// The block was closed due to a forward clock adjustment
            /// </summary>
            ClockAdjustForward = 6,
            /// <summary>
            /// The block was closed due to a backwards clock adjustment
            /// </summary>
            ClockAdjustBackward = 7,
            /// <summary>
            /// The block was closed because it is no longer recording.
            /// </summary>
            RecorderStopped = 8,
            /// <summary>
            /// The block was closed because the configuration changed.
            /// </summary>
            ConfigurationChanged = 9,
        }

        #endregion

        #region Constants

        private const ushort CLOSURE_STATUS_MASK = 0x000F;
        private const int CLOSURE_INTERVAL_SHIFT = 4;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table61">The table 61 object for the current device.</param>
        /// <param name="Table62">The table 62 object for the current device.</param>
        /// <param name="validIntervals">The number of valid intervals in the block.</param>
        /// <param name="DataSet">The data set the block belongs to.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public LPBlockDataRecord(CTable00 Table0, StdTable61 Table61, StdTable62 Table62, ushort validIntervals, LPDataSet DataSet)
        {
            m_Table0 = Table0;
            m_Table61 = Table61;
            m_Table62 = Table62;
            m_DataSet = DataSet;
            m_ValidIntervals = validIntervals;

            m_EndReadings = null;
            m_EndPulses = null;
            m_SimpleIntervalStatus = null;
            m_Intervals = null;
            m_NumberOfValidIntervals = null;
            m_ClosureStatuses = null;
            m_BlockEndTime = null;
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data to parse.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public void Parse(PSEMBinaryReader Reader)
        {
            LPSetActualLimits SetLimits = m_Table61.GetSetLimits(m_DataSet);

            if (SetLimits != null)
            {
                // Read the block end time. The End Time is in local time so we need to make sure that we specify that it's local time
                m_BlockEndTime = DateTime.SpecifyKind(Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat), DateTimeKind.Local);

                if (m_Table61.IncludeBlockEndRead)
                {
                    m_EndReadings = new double[SetLimits.NumberOfChannels];
                }
                else
                {
                    m_EndReadings = null;
                }

                if (m_Table61.IncludeBlockEndPulse)
                {
                    m_EndPulses = new uint[SetLimits.NumberOfChannels];
                }
                else
                {
                    m_EndPulses = null;
                }

                // Read the end readings
                for (int iChannel = 0; iChannel < SetLimits.NumberOfChannels; iChannel++)
                {
                    if (m_Table61.IncludeBlockEndRead)
                    {
                        m_EndReadings[iChannel] = Reader.ReadDouble();
                    }

                    if (m_Table61.IncludeBlockEndPulse)
                    {
                        m_EndPulses[iChannel] = Reader.ReadUInt32();
                    }
                }

                // Read the closure statuses
                if (m_Table61.IncludeClosureStatus)
                {
                    m_ClosureStatuses = new ClosureStatus[SetLimits.NumberOfChannels];
                    m_NumberOfValidIntervals = new ushort[SetLimits.NumberOfChannels];

                    for (int iChannel = 0; iChannel < SetLimits.NumberOfChannels; iChannel++)
                    {
                        ushort usClosureBitField = Reader.ReadUInt16();

                        m_ClosureStatuses[iChannel] = (ClosureStatus)(usClosureBitField & CLOSURE_STATUS_MASK);
                        m_NumberOfValidIntervals[iChannel] = (ushort)(usClosureBitField >> CLOSURE_INTERVAL_SHIFT);
                    }
                }
                else
                {
                    m_ClosureStatuses = null;
                    m_NumberOfValidIntervals = null;
                }

                // Read the simple interval status
                if (m_Table61.IncludeSimpleIntervalStatus)
                {
                    m_SimpleIntervalStatus = Reader.ReadBytes((SetLimits.IntervalsPerBlock + 7) / 8);
                }
                else
                {
                    m_SimpleIntervalStatus = null;
                }

                // Read all of the intervals valid and invalid in case the reader contains multiple blocks 
                m_Intervals = new LPIntervalDataRecord[m_ValidIntervals];

                for (int iInterval = 0; iInterval < m_ValidIntervals; iInterval++)
                {
                    LPDataFormats DataFormat = m_Table62.GetDataSelection(m_DataSet).DataFormat.Value;
                    LPIntervalDataRecord Interval = new LPIntervalDataRecord(SetLimits.NumberOfChannels, m_Table61.IncludeExtendedIntervalStatus, DataFormat);
                    Interval.Parse(Reader);

                    // We only need to keep the valid intervals.
                    if (iInterval < m_ValidIntervals)
                    {
                        m_Intervals[iInterval] = Interval;
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Device does not support selected data set.");
            }
        }

        /// <summary>
        /// Gets the size of a Data block.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table61">The table 61 object for the current device.</param>
        /// <param name="Table62">The table 62 object for the current device.</param>
        /// <param name="DataSet">The data that the block is in.</param>
        /// <returns>The size of the data block</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public static uint Size(CTable00 Table0, StdTable61 Table61, StdTable62 Table62, LPDataSet DataSet)
        {
            uint uiSize = 0;
            LPSetActualLimits SetLimits = Table61.GetSetLimits(DataSet);
            LPSetDataSelection SetDataSelection = Table62.GetDataSelection(DataSet);

            // Block end time
            uiSize += Table0.STIMESize;

            // Block end readings
            if (Table61.IncludeBlockEndRead)
            {
                uiSize += (uint)(SetLimits.NumberOfChannels * 8);
            }

            // Block end pulses
            if (Table61.IncludeBlockEndPulse)
            {
                uiSize += (uint)(SetLimits.NumberOfChannels * 4);
            }

            // Closure status
            if (Table61.IncludeClosureStatus)
            {
                uiSize += (uint)(SetLimits.NumberOfChannels * 2);
            }

            // Simple interval status
            if (Table61.IncludeSimpleIntervalStatus)
            {
                uiSize += (uint)((SetLimits.IntervalsPerBlock + 7) / 8);
            }

            uiSize += SetLimits.IntervalsPerBlock * LPIntervalDataRecord.Size(SetLimits.NumberOfChannels, Table61.IncludeExtendedIntervalStatus, SetDataSelection.DataFormat.Value);

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the end time of the block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public DateTime? BlockEndTime
        {
            get
            {
                return m_BlockEndTime;
            }
        }

        /// <summary>
        /// Gets the end readings for each channel of the block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public double[] BlockEndReadings
        {
            get
            {
                return m_EndReadings;
            }
        }

        /// <summary>
        /// Gets the end pulse readings for each channel of the block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public uint[] BlockEndPulses
        {
            get
            {
                return m_EndPulses;
            }
        }

        /// <summary>
        /// Gets the closure status for each channel of the block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public ClosureStatus[] ClosureStatuses
        {
            get
            {
                return m_ClosureStatuses;
            }
        }

        /// <summary>
        /// Gets the number of valid intervals for each channel of the block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public ushort[] NumberOfValidIntervals
        {
            get
            {
                return m_NumberOfValidIntervals;
            }
        }

        /// <summary>
        /// Gets the simple interval statuses.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public byte[] SimpleIntervalStatuses
        {
            get
            {
                return m_SimpleIntervalStatus;
            }
        }

        /// <summary>
        /// Gets the intervals stored in the block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public LPIntervalDataRecord[] Intervals
        {
            get
            {
                return m_Intervals;
            }
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable61 m_Table61;
        private StdTable62 m_Table62;
        private LPDataSet m_DataSet;

        private DateTime? m_BlockEndTime;
        private double[] m_EndReadings;
        private uint[] m_EndPulses;
        private byte[] m_SimpleIntervalStatus;
        private LPIntervalDataRecord[] m_Intervals;
        private ushort[] m_NumberOfValidIntervals;
        private ClosureStatus[] m_ClosureStatuses;
        private ushort m_ValidIntervals;

        #endregion
    }

    #endregion

    #region LPIntervalDataRecord

    /// <summary>
    /// Interval object for load profile.
    /// </summary>
    public class LPIntervalDataRecord
    {
        #region Constants

        private const byte FIRST_NIBBLE_MASK = 0xF0;
        private const byte LAST_NIBBLE_MASK = 0x0F;
        private const int NIBBLE_SHIFT = 4;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="NumberOfChannels">The number of channels used.</param>
        /// <param name="IncludeExtendedStatus">Wether or nor the Extended Interval status is included.</param>
        /// <param name="DataFormat">The data format used for the interval data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public LPIntervalDataRecord(byte NumberOfChannels, bool IncludeExtendedStatus, LPDataFormats DataFormat)
        {
            m_IntervalData = null;
            m_ChannelStatuses = null;
            m_IntervalStatus = null;
            m_NumberOfChannels = NumberOfChannels;
            m_IncludeExtendedStatus = IncludeExtendedStatus;
            m_DataFormat = DataFormat;
        }

        /// <summary>
        /// Parses the interval data from the binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public void Parse(PSEMBinaryReader Reader)
        {
            byte[] StatusData;
            // Read the extended interval statuses.
            if (m_IncludeExtendedStatus == true)
            {
                StatusData = Reader.ReadBytes(m_NumberOfChannels / 2 + 1);

                // The interval status is always the first nibble of the first byte.
                // The enumeration has been set to use the first nibble for the various
                // flags so all we need to do is mask off the nibble and cast it to the enum
                m_IntervalStatus = (ExtendedIntervalStatus)(StatusData[0] & FIRST_NIBBLE_MASK);

                m_ChannelStatuses = new ExtendedChannelStatus[m_NumberOfChannels];

                for (int iChannel = 0; iChannel < m_NumberOfChannels; iChannel++)
                {
                    int iNibble = iChannel % 2;
                    int iByteIndex = (iChannel + 1) / 2;

                    if (iNibble == 0)
                    {
                        // Channel status is in the last nibble
                        m_ChannelStatuses[iChannel] = (ExtendedChannelStatus)(StatusData[iByteIndex] & LAST_NIBBLE_MASK);
                    }
                    else
                    {
                        // Channel status is in the first nibble
                        m_ChannelStatuses[iChannel] = (ExtendedChannelStatus)(StatusData[iByteIndex] >> NIBBLE_SHIFT);
                    }
                }
            }
            else
            {
                m_IntervalStatus = 0;
                m_ChannelStatuses = null;
            }

            // Read the interval data.
            m_IntervalData = new double[m_NumberOfChannels];

            for (int iIndex = 0; iIndex < m_NumberOfChannels; iIndex++)
            {
                switch (m_DataFormat)
                {
                    case LPDataFormats.UINT8:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadByte();
                        break;
                    }
                    case LPDataFormats.INT8:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadSByte();
                        break;
                    }
                    case LPDataFormats.UINT16:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadUInt16();
                        break;
                    }
                    case LPDataFormats.INT16:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadInt16();
                        break;
                    }
                    case LPDataFormats.UINT32:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadUInt32();
                        break;
                    }
                    case LPDataFormats.INT32:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadInt32();
                        break;
                    }
                    case LPDataFormats.NI_FMAT1:
                    {
                        m_IntervalData[iIndex] = Reader.ReadDouble();
                        break;
                    }
                    case LPDataFormats.NI_FMAT2:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadSingle();
                        break;
                    }
                    case LPDataFormats.UINT24:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadUInt24();
                        break;
                    }
                    case LPDataFormats.INT24:
                    {
                        m_IntervalData[iIndex] = (double)Reader.ReadInt24();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the size of a Load Profile interval.
        /// </summary>
        /// <param name="NumberOfChannels">The number of channels in the interval.</param>
        /// <param name="IncludeExtendedStatus">Whether or not the Extended interval status is included.</param>
        /// <param name="DataFormat">The data format used for the interval data.</param>
        /// <returns>The size of an interval.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public static uint Size(byte NumberOfChannels, bool IncludeExtendedStatus, LPDataFormats DataFormat)
        {
            uint uiSize = 0;

            // Extended interval status
            if (IncludeExtendedStatus)
            {
                uiSize += (uint)(NumberOfChannels / 2 + 1);
            }

            // Interval Data
            uiSize += (uint)(NumberOfChannels * LPIntervalDataRecord.DataSize(DataFormat));

            return uiSize;
        }

        /// <summary>
        /// Gets the size of an individual piece of data.
        /// </summary>
        /// <param name="DataFormat">The data format used.</param>
        /// <returns>The size of the data in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/30/09 RCG 2.20.10 N/A    Created

        public static uint DataSize(LPDataFormats DataFormat)
        {
            uint uiSize = 0;

            switch (DataFormat)
            {
                case LPDataFormats.UINT8:
                case LPDataFormats.INT8:
                {
                    uiSize = 1;
                    break;
                }
                case LPDataFormats.UINT16:
                case LPDataFormats.INT16:
                {
                    uiSize = 2;
                    break;
                }
                case LPDataFormats.UINT24:
                case LPDataFormats.INT24:
                {
                    uiSize = 3;
                    break;
                }
                case LPDataFormats.UINT32:
                case LPDataFormats.INT32:
                case LPDataFormats.NI_FMAT2:
                {
                    uiSize = 4;
                    break;
                }
                case LPDataFormats.NI_FMAT1:
                {
                    uiSize = 8;
                    break;
                }
            }

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the interval statuses.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public ExtendedIntervalStatus? IntervalStatus
        {
            get
            {
                return m_IntervalStatus;
            }
        }

        /// <summary>
        /// Gets the channel statuses for each channel
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/08 RCG 2.00.00 N/A    Created

        public ExtendedChannelStatus[] ChannelStatuses
        {
            get
            {
                return m_ChannelStatuses;
            }
        }

        /// <summary>
        /// Gets the interval data for each channel
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/01/08 RCG 2.00.00 N/A    Created

        public double[] IntervalData
        {
            get
            {
                return m_IntervalData;
            }
        }

        #endregion

        #region Member Variables

        private double[] m_IntervalData;

        private ExtendedIntervalStatus? m_IntervalStatus;
        private ExtendedChannelStatus[] m_ChannelStatuses;

        private byte m_NumberOfChannels;
        private bool m_IncludeExtendedStatus;
        private LPDataFormats m_DataFormat;

        #endregion
    }

    #endregion
}

