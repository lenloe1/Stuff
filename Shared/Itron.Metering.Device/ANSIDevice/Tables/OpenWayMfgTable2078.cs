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
//                           Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Globalization;


namespace Itron.Metering.Device
{
    /// <summary>
    /// The CRFLANMfgTable2078 class handles the reading of the RFLAN Neighbor
    /// table.
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    public class OpenWayMfgTable2078 : AnsiTable
    {
        #region Constants

        /// <summary>
        /// Length of the neighbor table.
        /// </summary>
        public const int ACT_RFLAN_NEIGHBOR_LIST_TBL_LENGTH = 70;
        private const int RFLAN_NEIGHBOR_COUNT = 10;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 jrf 1.50.23        Created
        //
        public OpenWayMfgTable2078(CPSEM psem)
            : this(psem, ACT_RFLAN_NEIGHBOR_LIST_TBL_LENGTH)
        {
        }

        /// <summary>
        /// Constructor used when reading from an EDL file.
        /// </summary>
        /// <param name="Binaryreader">Binary reader associated with the tables data array.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/06/08 jrf 1.50.23        Created
        // 
        public OpenWayMfgTable2078(PSEMBinaryReader Binaryreader)
            : this(Binaryreader, ACT_RFLAN_NEIGHBOR_LIST_TBL_LENGTH)
        {
        }

        /// <summary>
        /// Full read of table 2078 (RFLAN Neighbor Table) out of the meter.
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 jrf 1.50.23        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMfgTable2078.Read");

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
        /// Returns an array of RFLAN Neighbor records.
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 jrf 1.50.23        Created
        //
        public List<RFLANNeighborEntryRcd> Neighbors
        {
            get
            {
                ReadUnloadedTable();

                return m_RFLANNeighbors;
            }
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="uiSize">The size of the Neighbor Table</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        protected OpenWayMfgTable2078(CPSEM psem, uint uiSize)
            : base(psem, 2078, uiSize, TABLE_TIMEOUT)
        {
            m_RFLANNeighbors = new List<RFLANNeighborEntryRcd>();
        }

        /// <summary>
        /// Constructor used when reading from an EDL file.
        /// </summary>
        /// <param name="Binaryreader">Binary reader associated with the tables data array.</param>
        /// <param name="uiSize">The size of the Neighbor Table</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        protected OpenWayMfgTable2078(PSEMBinaryReader Binaryreader, uint uiSize)
            : base(2078, uiSize)
        {
            m_RFLANNeighbors = new List<RFLANNeighborEntryRcd>();

            m_Reader = Binaryreader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Method to get data out of the Binary Reader and into member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/06/08 jrf 1.50.23        Created

        protected virtual void ParseData()
        {
            m_RFLANNeighbors = new List<RFLANNeighborEntryRcd>();

            for (int i = 0; i < RFLAN_NEIGHBOR_COUNT; i++)
            {
                RFLANNeighborEntryRcd NewNeighbor = new RFLANNeighborEntryRcd();

                NewNeighbor.MACAddress = m_Reader.ReadUInt32();
                NewNeighbor.SynchMerit = m_Reader.ReadUInt16();
                NewNeighbor.RSSI = m_Reader.ReadSByte();

                if (NewNeighbor.MACAddress > 0)
                {
                    m_RFLANNeighbors.Add(NewNeighbor);
                }
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Neighbors
        /// </summary>
        protected List<RFLANNeighborEntryRcd> m_RFLANNeighbors;

        #endregion
    }

    /// <summary>
    /// The 2078 class that handles the reading of the RFLAN Neighbor Table for
    /// High Data Rate RFLAN Modules.
    /// </summary>
    public class OpenWayMfgTable2078HDR : OpenWayMfgTable2078
    {
        #region Constants

        private const int HDR_NEIGHBORS = 12;

        /// <summary>
        /// The length of the HDR Neighbor table in bytes
        /// </summary>
        public const int HDR_TABLE_LENGTH = HDR_NEIGHBORS * 11;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        public OpenWayMfgTable2078HDR(CPSEM psem)
            : base(psem, HDR_TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Constructor used when reading from an EDL file.
        /// </summary>
        /// <param name="Binaryreader">Binary reader associated with the tables data array.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        public OpenWayMfgTable2078HDR(PSEMBinaryReader Binaryreader)
            : base(Binaryreader, HDR_TABLE_LENGTH)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data that was just read from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/06/08 jrf 1.50.23        Created

        protected override void ParseData()
        {
            m_RFLANNeighbors = new List<RFLANNeighborEntryRcd>();

            for (int i = 0; i < HDR_NEIGHBORS; i++)
            {
                RFLANNeighborEntryHDRRecord NewNeighbor = new RFLANNeighborEntryHDRRecord();

                NewNeighbor.MACAddress = m_Reader.ReadUInt32();
                NewNeighbor.SynchMerit = m_Reader.ReadUInt16();
                NewNeighbor.RSSI = m_Reader.ReadSByte();
                NewNeighbor.LPD = m_Reader.ReadByte();
                NewNeighbor.Level = m_Reader.ReadByte();
                NewNeighbor.TransmitWindow = m_Reader.ReadByte();
                NewNeighbor.ReceiveRate = m_Reader.ReadByte();

                if (NewNeighbor.MACAddress > 0)
                {
                    m_RFLANNeighbors.Add(NewNeighbor);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Class that represents a single RFLAN neighbor entry record.
    /// </summary>
    public class RFLANNeighborEntryRcd
    {
        #region Constants
        /// <summary>Constant indicate the value used to represent No Synch Merit, meaning the device is not Synchronzized</summary>
        public const ushort NO_SYNCH_MERIT = 65535;
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 jrf 1.50.23        Created
        //
        public RFLANNeighborEntryRcd()
        {
            m_uiMACAddress = 0;
            m_uiSynchMerit = 0;
            m_sbyRSSI = 0;
        }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// The MAC address of the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 jrf 1.50.23        Created
        //
        public UInt32 MACAddress
        {
            get
            {
                return m_uiMACAddress;
            }
            set
            {
                m_uiMACAddress = value;
            }
        }

        /// <summary>
        /// The SynchMerit of the neighbor.  A complex calculation taking into 
        /// account the level of the neighbor and relative position to the current
        /// meter.  The lower the number the better.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 jrf 1.50.23        Created
        //
        public UInt16 SynchMerit
        {
            get
            {
                return m_uiSynchMerit;
            }
            set
            {
                m_uiSynchMerit = value;
            }
        }

        /// <summary>
        /// Signed byte containing the RSSI.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 jrf 1.50.23        Created
        //
        public sbyte RSSI
        {
            get
            {
                return m_sbyRSSI;
            }
            set
            {
                m_sbyRSSI = value;
            }
        }

        #endregion

        #region Members

        private UInt32 m_uiMACAddress;
        private UInt16 m_uiSynchMerit;
        private sbyte m_sbyRSSI;

        #endregion
    }

    /// <summary>
    /// Class that represents a single RFLAN neighbor entry for High Dat Rate
    /// </summary>
    public class RFLANNeighborEntryHDRRecord : RFLANNeighborEntryRcd
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        public RFLANNeighborEntryHDRRecord()
            :base()
        {
            m_byLPD = 0;
            m_byLevel = 0;
            m_byTxWindow = 0;
            m_byRxRate = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the LPD value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        public byte LPD
        {
            get
            {
                return m_byLPD;
            }
            set
            {
                m_byLPD = value;
            }
        }

        /// <summary>
        /// Gets or sets the Level
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        public byte Level
        {
            get
            {
                return m_byLevel;
            }
            set
            {
                m_byLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the Transmit Window
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        public byte TransmitWindow
        {
            get
            {
                return m_byTxWindow;
            }
            set
            {
                m_byTxWindow = value;
            }
        }

        /// <summary>
        /// Gets or sets the Receive Rate
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/10 RCG 2.40.28        Created

        public byte ReceiveRate
        {
            get
            {
                return m_byRxRate;
            }
            set
            {
                m_byRxRate = value;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byLPD;
        private byte m_byLevel;
        private byte m_byTxWindow;
        private byte m_byRxRate;

        #endregion
    }

}
