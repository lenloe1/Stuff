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
//                              Copyright © 2010 - 2011
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
    /// Sub table used for reading the Core Dump Header information.
    /// </summary>
    public class OpenWayMFGTable3043Header : ANSISubTable
    {
        #region Constants

        private const int SUBTABLE_OFFSET = 0;
        private const ushort SUBTABLE_SIZE = 53;
        private const string CORE_DUMP_ID = "Centron AMI CoreDump";
        private const string M2_GATEWAY_CORE_DUMP_ID = " M2 Gateway CoreDump";
        private readonly DateTime REFERENCE_DATE = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/03/10 RCG 2.40.11 N/A    Created

        public OpenWayMFGTable3043Header(CPSEM psem)
            : base(psem, 3043, SUBTABLE_OFFSET, SUBTABLE_SIZE)
        {
            m_strID = "";
            m_strBuildDate = "";
            m_bIsValidCoreDump = false;
            m_byFWVersion = 0;
            m_byFWRevision = 0;
            m_byFWBuild = 0;
            m_uiCoreDumpTime = 0;
        }

        /// <summary>
        /// Reads the sub table from the meter
        /// </summary>
        /// <returns>The response of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/03/10 RCG 2.40.11 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable3043Header.Read");

            PSEMResponse Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not 3043 contains a valid core dump.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/03/10 RCG 2.40.11 N/A    Created

        public bool IsValidCoreDump
        {
            get
            {
                ReadUnloadedTable();

                return m_bIsValidCoreDump;
            }
        }

        /// <summary>
        /// Gets the FW Version at the time of core dump
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/03/10 RCG 2.40.11 N/A    Created

        public byte FWVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_byFWVersion;
            }
        }

        /// <summary>
        /// Gets the FW Revision at the time of core dump
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/03/10 RCG 2.40.11 N/A    Created

        public byte FWRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_byFWRevision;
            }
        }

        /// <summary>
        /// Gets the FW Build at the time of core dump
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/03/10 RCG 2.40.11 N/A    Created

        public byte FWBuild
        {
            get
            {
                ReadUnloadedTable();

                return m_byFWBuild;
            }
        }

        /// <summary>
        /// Gets the date and time of the core dump in GMT
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/03/10 RCG 2.40.11 N/A    Created

        public DateTime CoreDumpTime
        {
            get
            {
                ReadUnloadedTable();

                return REFERENCE_DATE.AddSeconds(m_uiCoreDumpTime);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read from the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/03/10 RCG 2.40.11 N/A    Created
        // 02/15/11 AF  2.50.04        Added support for the M2 Gateway
        //
        private void ParseData()
        {
            try
            {
                m_strID = m_Reader.ReadString(21);
            }
            catch (Exception)
            {
                m_strID = "";
            }

            // Make sure that we have a valid cored dump before we read anything else.
            m_bIsValidCoreDump = m_strID.Equals(CORE_DUMP_ID);

            if (!m_bIsValidCoreDump)
            {
                // Check to see if it is a valid M2 Gateway core dump
                m_bIsValidCoreDump = m_strID.Equals(M2_GATEWAY_CORE_DUMP_ID);
            }

            if (m_bIsValidCoreDump)
            {
                m_byFWVersion = m_Reader.ReadByte();
                m_byFWRevision = m_Reader.ReadByte();
                m_byFWBuild = m_Reader.ReadByte();
                m_strBuildDate = m_Reader.ReadString(25);
                m_uiCoreDumpTime = m_Reader.ReadUInt32();
            }
        }

        #endregion

        #region Member Variables

        private string m_strID;
        private bool m_bIsValidCoreDump;
        private byte m_byFWVersion;
        private byte m_byFWRevision;
        private byte m_byFWBuild;
        private string m_strBuildDate;
        private uint m_uiCoreDumpTime;

        #endregion
    }

    /// <summary>
    /// Sub table used for reading the Core Dump Info block
    /// </summary>
    public class OpenWayMFGTable3043Info : ANSISubTable
    {
        #region Constants

        private const int SUBTABLE_OFFSET = 256;
        private const ushort SUBTABLE_SIZE = 100;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public OpenWayMFGTable3043Info(CPSEM psem)
            : base(psem, 3043, SUBTABLE_OFFSET, SUBTABLE_SIZE)
        {
        }

        /// <summary>
        /// Reads the sub table from the meter
        /// </summary>
        /// <returns>The response of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable3043Info.Read");

            PSEMResponse Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of map items in the Core Dump
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public ushort NumberOfMapItems
        {
            get
            {
                ReadUnloadedTable();

                return m_usMapItems;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read from the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        private void ParseData()
        {
            m_uiStructSize = m_Reader.ReadUInt32();
            m_uiCurrentTime = m_Reader.ReadUInt32();
            m_uiLevelsOfIRQCount = m_Reader.ReadUInt32();
            m_uiHighPriorityTaskAddress = m_Reader.ReadUInt32();
            m_usEPFCount = m_Reader.ReadUInt16();
            m_usEPFResets = m_Reader.ReadUInt16();
            m_usStateBits = m_Reader.ReadUInt16();
            m_usMapItems = m_Reader.ReadUInt16();
            m_uiCPSR = m_Reader.ReadUInt32();
            m_uiFatalErrorReason = m_Reader.ReadUInt32();
            m_byReleaseBuild = m_Reader.ReadByte();
            m_byaAlignment = m_Reader.ReadBytes(3);
            m_byaFrame = m_Reader.ReadBytes(64);
        }

        #endregion

        #region Member Variables

        private uint m_uiStructSize;
        private uint m_uiCurrentTime;
        private uint m_uiLevelsOfIRQCount;
        private uint m_uiHighPriorityTaskAddress;
        private ushort m_usEPFCount;
        private ushort m_usEPFResets;
        private ushort m_usStateBits;
        private ushort m_usMapItems;
        private uint m_uiCPSR;
        private uint m_uiFatalErrorReason;
        private byte m_byReleaseBuild;
        private byte[] m_byaAlignment;
        private byte[] m_byaFrame;

        #endregion
    }

    /// <summary>
    /// Sub table used for reading the Core Dump Map block
    /// </summary>
    public class OpenWayMFGTable3043Map : ANSISubTable
    {
        #region Constants

        private const int SUBTABLE_OFFSET = 512;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object</param>
        /// <param name="table3043Info">The 3043 Core Dump Info sub table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public OpenWayMFGTable3043Map(CPSEM psem, OpenWayMFGTable3043Info table3043Info)
            : base(psem, 3043, SUBTABLE_OFFSET, GetTableSize(table3043Info))
        {
            m_MapItems = null;
            m_Table3043Info = table3043Info;
        }

        /// <summary>
        /// Reads the sub table from the meter
        /// </summary>
        /// <returns>The response of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable3043Map.Read");

            PSEMResponse Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of Map items
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public List<CoreDumpMapItem> MapItems
        {
            get
            {
                ReadUnloadedTable();

                return m_MapItems;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table.
        /// </summary>
        /// <param name="table3043Info">The 3043 Core Dump Info sub table for the current device</param>
        /// <returns>The size of the table in bytes</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        private static ushort GetTableSize(OpenWayMFGTable3043Info table3043Info)
        {
            ushort usTableSize = 0;

            if (table3043Info != null)
            {
                usTableSize = (ushort)(12 * table3043Info.NumberOfMapItems);
            }

            return usTableSize;
        }

        /// <summary>
        /// Parses the data read from the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        private void ParseData()
        {
            m_MapItems = new List<CoreDumpMapItem>();

            for (int iMapItem = 0; iMapItem < m_Table3043Info.NumberOfMapItems; iMapItem++)
            {
                CoreDumpMapItem NewMapItem = new CoreDumpMapItem();
                NewMapItem.ParseItem(m_Reader);

                m_MapItems.Add(NewMapItem);
            }
        }

        #endregion

        #region Member Variables

        private OpenWayMFGTable3043Info m_Table3043Info;
        private List<CoreDumpMapItem> m_MapItems;

        #endregion
    }

    /// <summary>
    /// Class the represents a Map Item stored in the core dump.
    /// </summary>
    public class CoreDumpMapItem
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public CoreDumpMapItem()
        {
            m_uiItemID = 0;
            m_uiLength = 0;
            m_uiOffset = 0;
        }

        /// <summary>
        /// Parse a map item from the specified binary reader
        /// </summary>
        /// <param name="binaryReader">The binary reader to parse from</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public void ParseItem(PSEMBinaryReader binaryReader)
        {
            m_uiItemID = binaryReader.ReadUInt32();
            m_uiOffset = binaryReader.ReadUInt32();
            m_uiLength = binaryReader.ReadUInt32();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Item ID for the Map item
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public uint ItemID
        {
            get
            {
                return m_uiItemID;
            }
        }

        /// <summary>
        /// Gets the offset for the map item
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public uint Offset
        {
            get
            {
                return m_uiOffset;
            }
        }

        /// <summary>
        /// Gets the length of the map item in bytes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 RCG 2.40.23 N/A    Created

        public uint Length
        {
            get
            {
                return m_uiLength;
            }
        }

        #endregion

        #region Member Variables

        private uint m_uiItemID;
        private uint m_uiOffset;
        private uint m_uiLength;

        #endregion
    }
}
