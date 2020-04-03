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
//                              Copyright © 2008 - 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    #region StdTable11

    /// <summary>
    /// Standard Table 11 - Actual Sources Limiting
    /// </summary>
    public class StdTable11 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 8;

        // Source Flags Masks
        private const byte PF_EXCLUDE_MASK = 0x01;
        private const byte RESET_EXCLUDE_MASK = 0x02;
        private const byte BLOCK_DEMAND_MASK = 0x04;
        private const byte SLIDING_DEMAND_MASK = 0x08;
        private const byte THERMAL_DEMAND_MASK = 0x10;
        private const byte SET1_PRESENT_MASK = 0x20;
        private const byte SET2_PRESENT_MASK = 0x40;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration for the Constants Selector types
        /// </summary>
        public enum ConstantTypes : byte
        {
            /// <summary>
            /// Use AGA3 constant structure
            /// </summary>
            GasAGA3 = 0,
            /// <summary>
            /// Use AGA7 constant structure
            /// </summary>
            GasAGA7 = 1,
            /// <summary>
            /// Use Electric constant structure
            /// </summary>
            Electric = 2,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The psem communications object for the current session.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public StdTable11(CPSEM psem)
            : this(psem, 11)
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
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public StdTable11(PSEMBinaryReader reader)
            : this(reader, 11)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable11.Read");

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
        /// Gets the number of Unit Of Measure Entries in Table 12
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumUnitOfMeasureEntries
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumUnitOfMeasureEntries;
            }
        }

        /// <summary>
        /// Gets the number of Demand Control entries in Table 13
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumDemandControlEntries
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumDemandControlEntries;
            }
        }

        /// <summary>
        /// Gets the length of a Data Control element
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte DataControlLength
        {
            get
            {
                ReadUnloadedTable();
                return m_byDataControlLength;
            }
        }

        /// <summary>
        /// Gets the number of Data Control entries in Table 14
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumDataControlEntries
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumDataControlEntries;
            }
        }

        /// <summary>
        /// Gets the number of Constant entries in Table 15
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumConstantEntries
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumConstantEntries;
            }
        }

        /// <summary>
        /// Gets the selector for the Constants record structure
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public ConstantTypes ConstantsSelector
        {
            get
            {
                ReadUnloadedTable();
                return (ConstantTypes)m_byConstantsSelector;
            }
        }

        /// <summary>
        /// Gets the number of sources in Table 16
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public byte NumberOfSources
        {
            get
            {
                ReadUnloadedTable();
                return m_byNumberOfSources;
            }
        }

        /// <summary>
        /// Gets whether or not Power Fail Exclusion is used.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool UsePowerFailExclusion
        {
            get
            {
                ReadUnloadedTable();
                return (m_bySourceFlags & PF_EXCLUDE_MASK) == PF_EXCLUDE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or nor Demand Reset Exclusion is used.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool UseResetExclusion
        {
            get
            {
                ReadUnloadedTable();
                return (m_bySourceFlags & RESET_EXCLUDE_MASK) == RESET_EXCLUDE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Block Demand is used.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool UseBlockDemand
        {
            get
            {
                ReadUnloadedTable();
                return (m_bySourceFlags & BLOCK_DEMAND_MASK) == BLOCK_DEMAND_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Sliding Demand is used.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool UseSlidingDemand
        {
            get
            {
                ReadUnloadedTable();
                return (m_bySourceFlags & SLIDING_DEMAND_MASK) == SLIDING_DEMAND_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Thermal Demand is used.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool UseThermalDemand
        {
            get
            {
                ReadUnloadedTable();
                return (m_bySourceFlags & THERMAL_DEMAND_MASK) == THERMAL_DEMAND_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Set 1 constants are present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool Set1ConstantsPresent
        {
            get
            {
                ReadUnloadedTable();
                return (m_bySourceFlags & SET1_PRESENT_MASK) == SET1_PRESENT_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Set 2 constants are present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public bool Set2ConstantsPresent
        {
            get
            {
                ReadUnloadedTable();
                return (m_bySourceFlags & SET2_PRESENT_MASK) == SET2_PRESENT_MASK;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The psem communications object for the current session.</param>
        /// <param name="tableID">The Table ID number</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20 N/A    Created

        public StdTable11(CPSEM psem, ushort tableID)
            : base(psem, tableID, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="tableID">The Table ID number</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20 N/A    Created

        public StdTable11(PSEMBinaryReader reader, ushort tableID)
            : base(tableID, TABLE_SIZE)
        {
            State = TableState.Loaded;
            m_Reader = reader;
            ParseData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses that data after a read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private void ParseData()
        {
            m_bySourceFlags = m_Reader.ReadByte();
            m_byNumUnitOfMeasureEntries = m_Reader.ReadByte();
            m_byNumDemandControlEntries = m_Reader.ReadByte();
            m_byDataControlLength = m_Reader.ReadByte();
            m_byNumDataControlEntries = m_Reader.ReadByte();
            m_byNumConstantEntries = m_Reader.ReadByte();
            m_byConstantsSelector = m_Reader.ReadByte();
            m_byNumberOfSources = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private byte m_bySourceFlags;
        private byte m_byNumUnitOfMeasureEntries;
        private byte m_byNumDemandControlEntries;
        private byte m_byDataControlLength;
        private byte m_byNumDataControlEntries;
        private byte m_byNumConstantEntries;
        private byte m_byConstantsSelector;
        private byte m_byNumberOfSources;

        #endregion
    }

    #endregion

    #region StdTable14

    /// <summary>
    /// Standard Table 14 - Data Control
    /// </summary>
    public class StdTable14 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table11">The actual sources limiting table.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public StdTable14(CPSEM psem, StdTable11 Table11)
            : this(psem, Table11, 14)
        {
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table11">Table 11 object for the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public StdTable14(PSEMBinaryReader reader, StdTable11 table11)
            : this(reader, table11, 14)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable14.Read");

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
        /// Gets the list of Source IDs as a LID number.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        public uint[] SourceIDs
        {
            get
            {
                ReadUnloadedTable();
                return m_SourceIDs;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="table11">The actual sources limiting table.</param>
        /// <param name="tableID">The Table ID Number</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20 N/A    Created

        public StdTable14(CPSEM psem, StdTable11 table11, ushort tableID)
            : base(psem, tableID, StdTable14.DetermineTableSize(table11))
        {
            m_Table11 = table11;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table11">Table 11 object for the meter</param>
        /// <param name="tableID">The Table ID Number</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20 N/A    Created

        public StdTable14(PSEMBinaryReader reader, StdTable11 table11, ushort tableID)
            : base(tableID, StdTable14.DetermineTableSize(table11))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table11 = table11;
            ParseData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table11">The actual sources limiting table.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        private static uint DetermineTableSize(StdTable11 Table11)
        {
            return (uint)Table11.NumDataControlEntries * (uint)Table11.DataControlLength;
        }

        /// <summary>
        /// Parses the data that was last read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Std")]
        private void ParseData()
        {
            // These values should always be LIDs which are UINT32's
            if (m_Table11.DataControlLength == sizeof(uint))
            {
                m_SourceIDs = new uint[m_Table11.NumDataControlEntries];

                for (int iIndex = 0; iIndex < m_Table11.NumDataControlEntries; iIndex++)
                {
                    m_SourceIDs[iIndex] = m_Reader.ReadUInt32();
                }
            }
            else
            {
                m_SourceIDs = null;
                throw new InvalidOperationException("Unexpected Data Control Length in Std Table 11");
            }
        }

        #endregion

        #region Member Variables

        private StdTable11 m_Table11;
        private uint[] m_SourceIDs;

        #endregion
    }

    #endregion
}
