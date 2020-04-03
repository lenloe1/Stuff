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
    /// Phases used by Meter
    /// </summary>
    [Flags]
    public enum IPPhases : uint
    {
        /// <summary>
        /// Invalid Phase
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Phase A
        /// </summary>
        PHASE_A = 0x00000001,
        /// <summary>
        /// Phases A and C
        /// </summary>
        PHASE_AC = 0x00000010,
        /// <summary>
        /// Phase A, B, and C
        /// </summary>
        PHASE_ABC = 0x00000011,
    }

    #region Definitions

    /// <summary>
    /// Tables used for each load profile data set.
    /// </summary>
    internal enum MFGLPDataSet : ushort
    {
        /// <summary>
        /// Table for set 1
        /// </summary>
        Set1 = 2412,
        /// <summary>
        /// Table for set 2
        /// </summary>
        Set2 = 2413,
        /// <summary>
        /// Table for set 3
        /// </summary>
        Set3 = 2414,
        /// <summary>
        /// Table for set 4
        /// </summary>
        Set4 = 2415,
    }

    #endregion

    #region OpenWayMFGTable2409

    /// <summary>
    /// MFG Table 361 (2409) - Actual MFG Load Profile Limit
    /// </summary>
    public class OpenWayMFGTable2409 : StdTable61
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM Communications object for the current session</param>
        /// <param name="table0">The Table 0 object for the current device</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        public OpenWayMFGTable2409(CPSEM psem, CTable00 table0)
            : base(psem, table0, 2409, DetermineTableSize(table0))
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
                // The MFG tables add an extra byte for UINT24 and INT24 data types so we need to combine both bytes
                m_UsedFormats = (LPDataFormats)((ushort)m_Reader.ReadByte() | ((ushort)m_Reader.ReadByte() << 8));

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

        #endregion

        #region Internal Methods

        /// <summary>
        /// Maps the specified table set to the corresponding table number
        /// </summary>
        /// <param name="set">The table set to map</param>
        /// <returns>The table number that maps to the set</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        internal override ushort MapLPSetToTable(LPDataSet set)
        {
            ushort TableID = 0;

            switch (set)
            {
                case LPDataSet.Set1:
                {
                    TableID = (ushort)MFGLPDataSet.Set1;
                    break;
                }
                case LPDataSet.Set2:
                {
                    TableID = (ushort)MFGLPDataSet.Set2;
                    break;
                }
                case LPDataSet.Set3:
                {
                    TableID = (ushort)MFGLPDataSet.Set3;
                    break;
                }
                case LPDataSet.Set4:
                {
                    TableID = (ushort)MFGLPDataSet.Set4;
                    break;
                }
            }

            return TableID;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the size of table 2408 based on the values in table 0
        /// </summary>
        /// <param name="Table0">The Table 0 object.</param>
        /// <returns>The size of table 2408.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0)
        {
            uint uiTableSize = TABLE_61_HEADER_SIZE + 1;

            if (Table0.IsTableUsed((ushort)MFGLPDataSet.Set1) == true)
            {
                // Table Set 1 is used
                uiTableSize += TABLE_61_SET_INFO_SIZE;
            }

            if (Table0.IsTableUsed((ushort)MFGLPDataSet.Set2) == true)
            {
                // Table Set 2 is used
                uiTableSize += TABLE_61_SET_INFO_SIZE;
            }

            if (Table0.IsTableUsed((ushort)MFGLPDataSet.Set3) == true)
            {
                // Table Set 3 is used
                uiTableSize += TABLE_61_SET_INFO_SIZE;
            }

            if (Table0.IsTableUsed((ushort)MFGLPDataSet.Set4) == true)
            {
                // Table Set 4 is used
                uiTableSize += TABLE_61_SET_INFO_SIZE;
            }

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region OpenWayMFGTable2410

    /// <summary>
    /// MFG Table 362 (2410) - Load Profile Control Table 
    /// </summary>
    public class OpenWayMFGTable2410 : StdTable62
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM Communications object for the current session</param>
        /// <param name="table0">The Table 0 object for the current device</param>
        /// <param name="table2409">The Table 2408 object for the current device</param>
        /// <param name="Table2392">The Table 2392 object for the current device</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        public OpenWayMFGTable2410(CPSEM psem, CTable00 table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2392 Table2392)
            : base(psem, table0, table2409, Table2392, 2410, DetermineTableSize(table0, table2409))
        {
            // This table uses a two byte format so we need to make sure this is set in order to read the table properly.
            m_UsesTwoByteFormat = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="table0">The Table 0 object for the current device.</param>
        /// <param name="table2409">The Table 61 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        private static uint DetermineTableSize(CTable00 table0, OpenWayMFGTable2409 table2409)
        {
            uint uiTableSize = 0;
            LPSetActualLimits SetLimits;

            if (table0.IsTableUsed((ushort)MFGLPDataSet.Set1))
            {
                SetLimits = table2409.Set1ActualLimits;
                uiTableSize += LPSetDataSelection.Size(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, true);
            }

            if (table0.IsTableUsed((ushort)MFGLPDataSet.Set2))
            {
                SetLimits = table2409.Set2ActualLimits;
                uiTableSize += LPSetDataSelection.Size(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, true);
            }

            if (table0.IsTableUsed((ushort)MFGLPDataSet.Set3))
            {
                SetLimits = table2409.Set3ActualLimits;
                uiTableSize += LPSetDataSelection.Size(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, true);
            }

            if (table0.IsTableUsed((ushort)MFGLPDataSet.Set4))
            {
                SetLimits = table2409.Set4ActualLimits;
                uiTableSize += LPSetDataSelection.Size(SetLimits.NumberOfChannels, SetLimits.IncludeScalarDivisor, true);
            }

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region OpenWayMFGTable2411

    /// <summary>
    /// MFG Table 363 (2411) - Load Profile Status Table
    /// </summary>
    public class OpenWayMFGTable2411 : StdTable63
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM Communications object for the current session</param>
        /// <param name="table0">The Table 0 object for the current device</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        public OpenWayMFGTable2411(CPSEM psem, CTable00 table0)
            : base(psem, table0, 2411, DetermineTableSize(table0))
        {
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Maps the specified table set to the corresponding table number
        /// </summary>
        /// <param name="set">The table set to map</param>
        /// <returns>The table number that maps to the set</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        internal override ushort MapLPSetToTable(LPDataSet set)
        {
            ushort TableID = 0;

            switch (set)
            {
                case LPDataSet.Set1:
                {
                    TableID = (ushort)MFGLPDataSet.Set1;
                    break;
                }
                case LPDataSet.Set2:
                {
                    TableID = (ushort)MFGLPDataSet.Set2;
                    break;
                }
                case LPDataSet.Set3:
                {
                    TableID = (ushort)MFGLPDataSet.Set3;
                    break;
                }
                case LPDataSet.Set4:
                {
                    TableID = (ushort)MFGLPDataSet.Set4;
                    break;
                }
            }

            return TableID;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the size of table 2408 based on the values in table 0
        /// </summary>
        /// <param name="Table0">The Table 0 object.</param>
        /// <returns>The size of table 2408.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0)
        {
            uint uiTableSize = 0;

            if (Table0.IsTableUsed((ushort)MFGLPDataSet.Set1) == true)
            {
                uiTableSize += LP_SET_STATUS_RECORD_SIZE;
            }

            if (Table0.IsTableUsed((ushort)MFGLPDataSet.Set2) == true)
            {
                uiTableSize += LP_SET_STATUS_RECORD_SIZE;
            }

            if (Table0.IsTableUsed((ushort)MFGLPDataSet.Set3) == true)
            {
                uiTableSize += LP_SET_STATUS_RECORD_SIZE;
            }

            if (Table0.IsTableUsed((ushort)MFGLPDataSet.Set4) == true)
            {
                uiTableSize += LP_SET_STATUS_RECORD_SIZE;
            }

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region OpenWayMFGTable2412

    /// <summary>
    /// MFG Table 364 (2412) - Load Profile Data Set 1
    /// </summary>
    public class OpenWayMFGTable2412 : StdTable64
    {
        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="table0">The table 0 object for the current device.</param>
        /// <param name="table2409">The table 61 object for the current device.</param>
        /// <param name="table2410">The table 62 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        public OpenWayMFGTable2412(CPSEM psem, CTable00 table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2410 table2410)
            : base(psem, 2412, DetermineTableSize(table0, table2409, table2410), table0, table2409, table2410, LPDataSet.Set1)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="table2409">The Table 61 object for the current device.</param>
        /// <param name="table2410"> The Table 62 object for the current device</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2410 table2410)
        {
            uint uiTableSize = 0;

            uiTableSize += table2409.Set1ActualLimits.NumberOfBlocks
                * LPBlockDataRecord.Size(Table0, table2409, table2410, LPDataSet.Set1);

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region OpenWayMFGTable2413

    /// <summary>
    /// MFG Table 365 (2413) - Load Profile Data Set 2
    /// </summary>
    public class OpenWayMFGTable2413 : StdTable64
    {
        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="table0">The table 0 object for the current device.</param>
        /// <param name="table2409">The table 61 object for the current device.</param>
        /// <param name="table2410">The table 62 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        public OpenWayMFGTable2413(CPSEM psem, CTable00 table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2410 table2410)
            : base(psem, 2413, DetermineTableSize(table0, table2409, table2410), table0, table2409, table2410, LPDataSet.Set2)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="table2409">The Table 61 object for the current device.</param>
        /// <param name="table2410"> The Table 62 object for the current device</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2410 table2410)
        {
            uint uiTableSize = 0;

            uiTableSize += table2409.Set2ActualLimits.NumberOfBlocks
                * LPBlockDataRecord.Size(Table0, table2409, table2410, LPDataSet.Set2);

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region OpenWayMFGTable2414

    /// <summary>
    /// MFG Table 366 (2414) - Load Profile Data Set 3
    /// </summary>
    public class OpenWayMFGTable2414 : StdTable64
    {
        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="table0">The table 0 object for the current device.</param>
        /// <param name="table2409">The table 61 object for the current device.</param>
        /// <param name="table2410">The table 62 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        public OpenWayMFGTable2414(CPSEM psem, CTable00 table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2410 table2410)
            : base(psem, 2414, DetermineTableSize(table0, table2409, table2410), table0, table2409, table2410, LPDataSet.Set3)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="table2409">The Table 61 object for the current device.</param>
        /// <param name="table2410"> The Table 62 object for the current device</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2410 table2410)
        {
            uint uiTableSize = 0;

            uiTableSize += table2409.Set3ActualLimits.NumberOfBlocks
                * LPBlockDataRecord.Size(Table0, table2409, table2410, LPDataSet.Set3);

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region OpenWayMFGTable2415

    /// <summary>
    /// MFG Table 367 (2415) - Load Profile Data Set 4
    /// </summary>
    public class OpenWayMFGTable2415 : StdTable64
    {
        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="table0">The table 0 object for the current device.</param>
        /// <param name="table2409">The table 61 object for the current device.</param>
        /// <param name="table2410">The table 62 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        public OpenWayMFGTable2415(CPSEM psem, CTable00 table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2410 table2410)
            : base(psem, 2415, DetermineTableSize(table0, table2409, table2410), table0, table2409, table2410, LPDataSet.Set4)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        /// <param name="table2409">The Table 61 object for the current device.</param>
        /// <param name="table2410"> The Table 62 object for the current device</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.14 N/A    Created

        private static uint DetermineTableSize(CTable00 Table0, OpenWayMFGTable2409 table2409, OpenWayMFGTable2410 table2410)
        {
            uint uiTableSize = 0;

            uiTableSize += table2409.Set4ActualLimits.NumberOfBlocks
                * LPBlockDataRecord.Size(Table0, table2409, table2410, LPDataSet.Set4);

            return uiTableSize;
        }

        #endregion
    }

    #endregion

    #region OpenWayMFGTable2417

    /// <summary>
    /// MFG Table 369 (2417) - Instrumentation Profile Channel
    /// </summary>
    public class OpenWayMFGTable2417 : AnsiTable
    {
        #region Constants

        /// <summary>
        /// Size of Table
        /// </summary>
        public const int TABLESIZE = 4;

        #endregion

        #region Public Methods

        /// <summary>
        /// Public Constructor to be used when creating this table.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="table0">The table 0 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/12 MSC 2.53.36 N/A    Created
        public OpenWayMFGTable2417(CPSEM psem, CTable00 table0)
            : base(psem, 2417, TABLESIZE)
        {
            m_blnChannels = new bool[16];
            m_uiChannels = 0;
            m_uiPhases = 0;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The PSEM result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 MSC 2.53.36 N/A    Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                uint data = m_Reader.ReadByte();

                m_blnChannels[0] = ((data & 0x00000001) != 0);
                m_blnChannels[1] = ((data & 0x00000010) != 0);
                m_blnChannels[2] = ((data & 0x00000100) != 0);
                m_blnChannels[3] = ((data & 0x00001000) != 0);
                m_blnChannels[4] = ((data & 0x00010000) != 0);
                m_blnChannels[5] = ((data & 0x00100000) != 0);
                m_blnChannels[6] = ((data & 0x01000000) != 0);
                m_blnChannels[7] = ((data & 0x10000000) != 0);

                data = m_Reader.ReadByte();

                m_blnChannels[8] = ((data & 0x00000001) != 0);
                m_blnChannels[9] = ((data & 0x00000010) != 0);
                m_blnChannels[10] = ((data & 0x00000100) != 0);
                m_blnChannels[11] = ((data & 0x00001000) != 0);
                m_blnChannels[12] = ((data & 0x00010000) != 0);
                m_blnChannels[13] = ((data & 0x00100000) != 0);
                m_blnChannels[14] = ((data & 0x01000000) != 0);
                m_blnChannels[15] = ((data & 0x10000000) != 0);

                //number of channels used
                m_uiChannels = m_Reader.ReadByte();

                //phases used
                m_uiPhases = m_Reader.ReadByte();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Channels Used
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/12 MSC 2.53.36 N/A    Created
        public bool[] ChannelsUsed
        {
            get
            {
                ReadUnloadedTable();

                return m_blnChannels;
            }
        }

        /// <summary>
        /// Number of Channels Used
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/12 MSC 2.53.36 N/A    Created
        public uint NumberOfChannels
        {
            get
            {
                ReadUnloadedTable();

                return m_uiChannels;
            }
        }

        /// <summary>
        /// Which Phases are being recorded
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/12 MSC 2.53.36 N/A    Created
        public IPPhases PhasesUsed
        {
            get
            {
                ReadUnloadedTable();

                return (IPPhases)m_uiPhases;
            }
        }

        #endregion

        #region Members

        private bool[] m_blnChannels;
        private uint m_uiChannels;
        private uint m_uiPhases;

        #endregion
    }

    #endregion

}
