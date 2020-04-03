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
//                            Copyright © 2012 - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Globalization;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The IPMfgTable2549 class handles the reading of the IPv6 Address Information table (501).
    /// </summary>
    internal class IPMfgTable2549 : AnsiTable
    {
        #region Constants

        private const uint TABLE_LENGTH_2549 = 104;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/12 AF  2.53.37 TREQ4719 Created
        //
        public IPMfgTable2549(CPSEM psem)
            : base(psem, 2549, TABLE_LENGTH_2549)
        {
            m_IPv6AddressRaw = new byte[16];
            m_IPv6AddressLong = new char[40];
            m_IPv6AddressShort = new char[40];

            m_IPv6AddressRaw.Initialize();
            m_IPv6AddressLong.Initialize();
            m_IPv6AddressShort.Initialize();
        }

        /// <summary>
        /// Full read of 2549 (Mfg 501) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/12 AF  2.53.37 TREQ4719 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "IPv6AddressInformation.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_IPv6AddressRaw = m_Reader.ReadBytes(m_IPv6AddressRaw.Length);
                m_IPv6AddressLong = m_Reader.ReadChars(m_IPv6AddressLong.Length);
                m_IPv6AddressShort = m_Reader.ReadChars(m_IPv6AddressShort.Length);
                m_IPv6AddressPresent = m_Reader.ReadByte();
                m_IPv6GlobalAddressPresent = m_Reader.ReadByte();
                m_IPv6AddressChangeCount = m_Reader.ReadUInt16();
                m_IPv6AddressAge = m_Reader.ReadSTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Reads the long version of the IP address from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 AF  2.70.34 TC11708 Created
        //
        public string IPv6AddressLong
        {
            get
            {
                Read();

                string strLongIPAddress = new string(m_IPv6AddressLong);

                return strLongIPAddress;
            }
        }

        /// <summary>
        /// Reads the IPv6 short address from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/12 AF  2.53.37 TREQ4719 Created
        //  10/23/12 AF  2.70.33 TR7023   Don't retrieve the cached value - it might change.
        //
        public string IPv6AddressShort
        {
            get
            {
                Read();

                string strShortIPAddress = new string(m_IPv6AddressShort);

                return strShortIPAddress;
            }
        }

        /// <summary>
        /// Reads the IPv6 Global Address Present field from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/12 AF  2.53.37 TREQ4719 Created
        //  10/23/12 AF  2.70.33 TR7023   Don't retrieve the cached value - it might change.
        //
        public bool IPv6GlobalAddressPresent
        {
            get
            {
                bool blnPresent = false;

                Read();

                if (m_IPv6GlobalAddressPresent != 0)
                {
                    blnPresent = true;
                }

                return blnPresent;
            }
        }

        #endregion

        #region Members

        private byte[] m_IPv6AddressRaw;
        private char[] m_IPv6AddressLong;
        private char[] m_IPv6AddressShort;
        private byte m_IPv6AddressPresent;
        private byte m_IPv6GlobalAddressPresent;
        private UInt16 m_IPv6AddressChangeCount;
        private DateTime m_IPv6AddressAge;

        #endregion
    }

    /// <summary>
    /// The IPMfgTable 2559 class handles the reading of the IPv6 Secure PPP Information (511)
    /// </summary>
    internal class IPMfgTable2559 : AnsiTable
    {
        #region Constants

        private const uint TABLE_LENGTH_BORON = 9;
        private const uint TABLE_LENGTH_2559 = 11;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/13 AF  2.70.56        Created
        //  08/26/15 AF  4.21.02 608214 Allow the table to have variable size and remove the
        //                              dependence on fw version
        //
        public IPMfgTable2559(CPSEM psem)
            : base(psem, 2559, TABLE_LENGTH_BORON)
        {
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Full read of 2559 (Mfg 511) out of the meter
        /// </summary>
        /// <returns>the response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/13 AF  2.70.56        Created
        //  08/26/15 AF  4.21.02 608214 Use the length of the data stream to determine the
        //                              fields to read
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "IPv6AddressSecurePPPInformation.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                m_SPPPCurrentlyActivated = m_Reader.ReadByte();
                m_AllowUnsecuredMode = m_Reader.ReadByte();
                m_SPPPOperatingMode = m_Reader.ReadByte();
                m_ECPState = m_Reader.ReadByte();
                m_SPPPState = m_Reader.ReadByte();
                m_SPPPLayerUp = m_Reader.ReadByte();
                m_IPV6CPState = m_Reader.ReadByte();
                m_PSKResident = m_Reader.ReadByte();
                m_SPPPForeverLocked = m_Reader.ReadByte();

                if (m_DataStream.Length >= TABLE_LENGTH_2559)
                {
                    m_SPPPPlatformMode = m_Reader.ReadByte();
                    m_SPPPSessionMode = m_Reader.ReadByte();
                }

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the PSK Resident field from the table.  This tells us if
        /// security is used for the PPP link
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/13 AF  2.70.56        Created
        //
        public bool PSKResident
        {
            get
            {
                bool blnPSKResident = false;
                ReadUnloadedTable();

                if (m_PSKResident == 0)
                {
                    blnPSKResident = false;
                }
                else
                {
                    blnPSKResident = true;
                }

                return blnPSKResident;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Members

        private byte m_SPPPCurrentlyActivated = 0;
        private byte m_AllowUnsecuredMode = 0;
        private byte m_SPPPOperatingMode = 0;
        private byte m_ECPState = 0;
        private byte m_SPPPState = 0;
        private byte m_SPPPLayerUp = 0;
        private byte m_IPV6CPState = 0;
        private byte m_PSKResident = 0;
        private byte m_SPPPForeverLocked = 0;
        private byte m_SPPPPlatformMode = 0;
        private byte m_SPPPSessionMode = 0;

        #endregion

    }

    /// <summary>
    /// The IPMfgTable2580 class handles the reading of the IPv6 Manufacturing Information
    /// table (532).
    /// </summary>
    internal class IPMfgTable2580 : AnsiTable
    {
        #region Constants

        private const uint TABLE_LENGTH_2580 = 41;
        private const uint TABLE_LENGTH_CARBON = 73;
        private const uint TABLE_LENGTH_5_5_19 = 113;
        private const uint TABLE_LENGTH_5_5_27 = 117;
        private const int NEIGHBOR_MAC_ADDR_OFFSET = 30;
        private const int SSID_SIZE = 32;
        private const int NOTCH_LIST_COUNT = 4; 

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/17/12 AF  2.53.31 183921 Created
        //  10/09/12 AF  2.70.28 TC11217 Added the f/w version params because the size
        //                              of the table changed at 5.2.33
        //  08/18/15 AF  4.21.01 606465 Allow the table to have variable size and remove the dependence on fw version
        //
        public IPMfgTable2580(CPSEM psem)
            : base(psem, 2580, TABLE_LENGTH_2580)
        {
            m_ModulePANId = new byte[4];
            m_ModulePANId.Initialize();

            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Full read of 2580 (Mfg 532) out of the meter
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/12 AF  2.70.28 TC11218 Created
        //  12/03/12 AF  2.70.45 WR245533 Table size increased at register f/w build 5.5.19
        //  12/21/12 AF  2.70.53 WR264596 Table size increased (again) at reg f/w build 5.5.27
        //  02/01/13 AF  2.70.62 288194   PAN ID byte order was corrected in Carbon.  Just read it here
        //                                and correct it in the comm module property.
        //  08/18/15 AF  4.21.01 606465   Use the data length to determine if fields are present instead of
        //                                the calculated table size, which didn't work for the IPv6 M2 Gateway
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "IPMfgTable2580.Read");
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                m_RegisterFWInfo = new FirmwareVersionInfo();
                m_RegisterFWInfo.Type = m_Reader.ReadByte();
                m_RegisterFWInfo.Version = m_Reader.ReadByte();
                m_RegisterFWInfo.Revision = m_Reader.ReadByte();
                m_RegisterFWInfo.Build = m_Reader.ReadByte();

                m_RegisterBootloaderInfo = new FirmwareVersionInfo();
                m_RegisterBootloaderInfo.Type = m_Reader.ReadByte();
                m_RegisterBootloaderInfo.Version = m_Reader.ReadByte();
                m_RegisterBootloaderInfo.Revision = m_Reader.ReadByte();
                m_RegisterBootloaderInfo.Build = m_Reader.ReadByte();

                m_ModuleFWInfo = new FirmwareVersionInfo();
                m_ModuleFWInfo.Type = m_Reader.ReadByte();
                m_ModuleFWInfo.Version = m_Reader.ReadByte();
                m_ModuleFWInfo.Revision = m_Reader.ReadByte();
                m_ModuleFWInfo.Build = m_Reader.ReadByte();

                m_ModuleBootloaderInfo = new FirmwareVersionInfo();
                m_ModuleBootloaderInfo.Type = m_Reader.ReadByte();
                m_ModuleBootloaderInfo.Version = m_Reader.ReadByte();
                m_ModuleBootloaderInfo.Revision = m_Reader.ReadByte();
                m_ModuleBootloaderInfo.Build = m_Reader.ReadByte();

                m_RegisterHWVersion = m_Reader.ReadByte();
                m_RegisterHWBuild = m_Reader.ReadByte();

                byte[] byaData = new byte[8];
                byaData = m_Reader.ReadBytes(8);
                Array.Reverse(byaData);
                MemoryStream DataStream = new MemoryStream(byaData);
                PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                m_ModuleMACAddress = Reader.ReadUInt64();

                m_ModulePANId = m_Reader.ReadBytes(4);

                byaData = m_Reader.ReadBytes(8);
                Array.Reverse(byaData);
                DataStream = new MemoryStream(byaData);
                Reader = new PSEMBinaryReader(DataStream);
                m_NeighborMACAddress = Reader.ReadUInt64();

                m_RegisterJTAGStatus = m_Reader.ReadByte();
                m_ModuleJTAGStatus = m_Reader.ReadByte();
                m_ModuleLinkStatus = m_Reader.ReadByte();

                if (m_DataStream.Length >= TABLE_LENGTH_CARBON)
                {
                    char[] aSSId = new char[SSID_SIZE];
                    aSSId = m_Reader.ReadChars(SSID_SIZE);
                    m_SSID = new string(aSSId);
                    //Remove trailing white space
                    m_SSID = m_SSID.Trim();

                    if (m_DataStream.Length >= TABLE_LENGTH_5_5_19)
                    {
                        m_NotchList = new NotchInfo[NOTCH_LIST_COUNT];
                        for (int index = 0; index < NOTCH_LIST_COUNT; index++)
                        {
                            m_NotchList[index] = new NotchInfo();
                            m_NotchList[index].StartChannel = m_Reader.ReadUInt32();
                            m_NotchList[index].StopChannel = m_Reader.ReadUInt32();
                        }

                        m_DwellInfo = new DwellInfo();
                        m_DwellInfo.Window = m_Reader.ReadUInt32();
                        m_DwellInfo.MaxDwell = m_Reader.ReadUInt32();

                        if (m_DataStream.Length >= TABLE_LENGTH_5_5_27)
                        {
                            m_Mode = m_Reader.ReadUInt32();
                        }
                    }
                }

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Reads the Register firmware version information from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public FirmwareVersionInfo RegFWVersionInfo
        {
            get
            {
                ReadUnloadedTable();

                return m_RegisterFWInfo;
            }
        }

        /// <summary>
        /// Reads the Register bootloader version information from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public FirmwareVersionInfo RegBootloaderVersionInfo
        {
            get
            {
                ReadUnloadedTable();

                return m_RegisterBootloaderInfo;
            }
        }

        /// <summary>
        /// Reads the Comm module firmware version information from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public FirmwareVersionInfo ModuleFWVersionInfo
        {
            get
            {
                ReadUnloadedTable();

                return m_ModuleFWInfo;
            }
        }

        /// <summary>
        /// Reads the Comm module bootloader version information from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public FirmwareVersionInfo ModuleBootloaderVersionInfo
        {
            get
            {
                ReadUnloadedTable();

                return m_ModuleBootloaderInfo;
            }
        }

        /// <summary>
        /// Reads the Comm Module MAC address out of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/17/12 AF  2.53.31 183921 Created
        //  10/11/12 AF  2.70.28        Replaced offset reads with a full table read
        //
        public ulong ModuleMACAddress
        {
            get
            {
                ReadUnloadedTable();

                return m_ModuleMACAddress;
            }
        }

        /// <summary>
        /// Reads the Neighbor MAC address out of the table via an offset read
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/26/12 jrf 2.70.24 235138 Created
        //  10/04/12 jrf 2.70.26 235138 Removing debug code.
        //  10/11/12 AF  2.70.28        Corrected the offset read to display the bytes in
        //                              the correct order.
        //
        public ulong NeighborMACAddress
        {
            get
            {
                byte[] byaData;
                PSEMResponse PSEMResult = base.Read(NEIGHBOR_MAC_ADDR_OFFSET, 8);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    // We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Neighbor MAC Address"));
                }
                else
                {
                    byaData = m_Reader.ReadBytes(8);
                    // Convert the bytes read to something useful
                    Array.Reverse(byaData);
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_NeighborMACAddress = Reader.ReadUInt64();
                }

                return m_NeighborMACAddress;
            }
        }

        /// <summary>
        /// Reads the Comm Module PAN Id out of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/12 AF  2.53.37 TREQ4720 Created
        //  10/11/12 AF  2.70.28        Replaced offset reads with a full table read
        //  02/01/13 AF  2.70.62 288194 Changed the data type to byte array to more easily deal
        //                              with byte order issues in the comm module property
        //
        public byte[] ModulePANId
        {
            get
            {
                ReadUnloadedTable();

                return m_ModulePANId;
            }
        }

        /// <summary>
        /// Reads the Comm Module JTAG status out of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/26/12 jrf 2.70.24 235138 Created
        //  10/11/12 AF  2.70.28        Replaced offset reads with a full table read
        //  04/08/15 jrf 4.20.01 577003 Removed unneeded call to read a byte from m_Reader.
        public bool ModuleJTAGSecurityEnabled
        {
            get
            {
                ReadUnloadedTable();

                if (1 == m_ModuleJTAGStatus)
                {
                    m_blnModuleJTAGSecurityEnabled = true;
                }
                else
                {
                    m_blnModuleJTAGSecurityEnabled = false;
                }

                return m_blnModuleJTAGSecurityEnabled;
            }
        }

        /// <summary>
        /// Reads the Comm Module link status out of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/26/12 jrf 2.70.24 235138 Created
        //  10/03/12 jkw 2.70.xx        Renamed to include 'Byte' in the name to distinguish
        //                              between this one used in the factoru QC tool and the 
        //                              enum
        //  10/09/12 AF  2.70.28        Renamed again after deleting the duplicate property
        //  10/11/12 AF  2.70.28        Replaced offset reads with a full table read
        //
        public byte ModuleLinkStatus
        {
            get
            {
                ReadUnloadedTable();

                return m_ModuleLinkStatus;
            }
        }

        /// <summary>
        /// Reads the SSID out of the table.  Warning! Only for firmware 5.2.33 and above.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/08/12 AF  2.70.28        Created
        //  10/11/12 AF  2.70.28        Replaced offset reads with a full table read
        //
        public string SSID
        {
            get
            {
                ReadUnloadedTable();

                return m_SSID;
            }
        }

        #endregion

        #region Private Methods


        #endregion

        #region Members

        private FirmwareVersionInfo m_RegisterFWInfo;
        private FirmwareVersionInfo m_RegisterBootloaderInfo;
        private FirmwareVersionInfo m_ModuleFWInfo;
        private FirmwareVersionInfo m_ModuleBootloaderInfo;
        private byte m_RegisterHWVersion;
        private byte m_RegisterHWBuild;
        private ulong m_ModuleMACAddress;
        //private UInt32 m_ModulePANId;
        private byte[] m_ModulePANId;
        private ulong m_NeighborMACAddress = 0;
        private bool m_blnModuleJTAGSecurityEnabled = false;
        private byte m_RegisterJTAGStatus = 0;
        private byte m_ModuleJTAGStatus = 0;
        private byte m_ModuleLinkStatus;
        private string m_SSID = "";
        private NotchInfo[] m_NotchList;
        private DwellInfo m_DwellInfo;
        private UInt32 m_Mode = 0;

        #endregion
    }

    /// <summary>
    /// The IPMfgTable2588 class handles the reading of the Diagnostics Dimension table (540)
    /// </summary>
    internal class IPMfgTable2588 : AnsiTable
    {
        #region Constants

        private const uint TABLE_LENGTH_2588 = 206;
        private const uint MAX_NUMBER_TLVS = 32;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263052 Added the dimension table (540)
        //
        public IPMfgTable2588(CPSEM psem)
            : base(psem, 2588, TABLE_LENGTH_2588)
        {
            m_CSMPSupportedTLVs = new List<ushort>();
            m_CSMPTLVSizes = new List<ushort>();
            m_CSMPTLVItemCounts = new List<ushort>();
        }

        /// <summary>
        /// Full read of 2588 (Mfg 540) out of the meter
        /// </summary>
        /// <returns>The PSEM response for the table read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263052 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                               "IPv6DiagnosticsDimension.Read");
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_CSMPInfoTableSize = m_Reader.ReadUInt16();
                m_CSMPMgtTableSize = m_Reader.ReadUInt16();
                m_CSMPReadQItemsSize = m_Reader.ReadUInt16();
                m_CSMPReadQNumItems = m_Reader.ReadUInt16();
                m_CSMPWriteQItemsSize = m_Reader.ReadUInt16();
                m_CSMPWriteQNumItems = m_Reader.ReadUInt16();
                m_CSMPTLVListLength = m_Reader.ReadUInt16();

                for (int index = 0; index < MAX_NUMBER_TLVS; index++)
                {
                    m_CSMPSupportedTLVs.Add(m_Reader.ReadUInt16());
                }

                for (int index = 0; index < MAX_NUMBER_TLVS; index++)
                {
                    m_CSMPTLVSizes.Add(m_Reader.ReadUInt16());
                }

                for (int index = 0; index < MAX_NUMBER_TLVS; index++)
                {
                    m_CSMPTLVItemCounts.Add(m_Reader.ReadUInt16());
                }

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of TLVs supported by the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263052 Created
        //
        public ushort TLVListLength
        {
            get
            {
                ReadUnloadedTable();

                return m_CSMPTLVListLength;
            }
        }

        /// <summary>
        /// Gets the list of the supported TLV Ids supported by the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263052 Created
        //
        public List<ushort> CSMPSupportedTLVs
        {
            get
            {
                ReadUnloadedTable();

                return m_CSMPSupportedTLVs;
            }
        }

        /// <summary>
        /// Gets the list of the total size of the TLV data.  Listed in the same order as the
        /// supported TLVs, so that we can use the offset in the supported TLVs to find the 
        /// length in this list.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263052 Created
        //
        public List<ushort> CSMPTLVSizes
        {
            get
            {
                ReadUnloadedTable();

                return m_CSMPTLVSizes;
            }
        }

        /// <summary>
        /// Gets list of sizes of the TLV record.  The offset in this list
        /// will be the same as the offset in the supported TLVs list
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263052 Created
        //
        public List<ushort> CSMPTLVItemCounts
        {
            get
            {
                ReadUnloadedTable();

                return m_CSMPTLVItemCounts;
            }
        }

        #endregion

        #region Members

        private UInt16 m_CSMPInfoTableSize;
        private UInt16 m_CSMPMgtTableSize;
        private UInt16 m_CSMPReadQItemsSize;
        private UInt16 m_CSMPReadQNumItems;
        private UInt16 m_CSMPWriteQItemsSize;
        private UInt16 m_CSMPWriteQNumItems;
        private UInt16 m_CSMPTLVListLength;
        private List<ushort> m_CSMPSupportedTLVs;
        private List<ushort> m_CSMPTLVSizes;
        private List<ushort> m_CSMPTLVItemCounts;

        #endregion

    }

    /// <summary>
    /// The IPMfgTable2597 class handles the reading of the Boron TLV Current Time table
    /// </summary>
    internal class IPMfgTable2597 : AnsiTable
    {
        #region Constants

        private const uint TABLE_LENGTH_2597 = 40;

        private const uint TABLE_LENGTH_2597_CARBON = 44;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/12 AF  2.53.38 TREQ4721 Created
        //  12/06/12 AF  2.70.47 262897   Table size increased by 4 bytes in build 5.5.
        //  09/21/15 AF  4.21.04 616166   Allow the table size to vary and then send the minimum size
        //                                to the base class.  The Read() will handle the rest.
        //
        public IPMfgTable2597(CPSEM psem)
            : base(psem, 2597, TABLE_LENGTH_2597)
        {
            m_iso8601 = new byte[32];
            m_SourceId = 0xFFFFFFFF;
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Full read of 2597 (Mfg 549) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/12 AF  2.53.38 TREQ4721 Created
        //  12/06/12 AF  2.70.47 262897   Added the source id field
        //  12/06/12 AF  2.70.47 262897   Only read the source id field if it is there
        //  09/21/15 AF  4.21.04 616166   Base the read on the actual size of the table and not fw version
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "TLVCurrentTime.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_LastUpdated = m_Reader.ReadUInt32();
                m_PosixTime = m_Reader.ReadTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME);
                m_iso8601 = m_Reader.ReadBytes(m_iso8601.Length);

                if (m_DataStream.Length >= TABLE_LENGTH_2597_CARBON)
                {
                    m_SourceId = m_Reader.ReadUInt32();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the comm module's current time from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/12 AF  2.53.38 TREQ4721 Created
        //  02/16/12 AF  2.53.41 TREQ4721 Make sure that table is read each time since
        //                                the time will always be changing
        //
        public DateTime CommModuleCurrentTime
        {
            get
            {
                Read();

                DateTime ReferenceDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                return (ReferenceDate + m_PosixTime);
            }
        }

        /// <summary>
        /// Last sync source
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/21/15 AF  4.21.04  WR 616166   Created
        //
        public UInt32 SourceID
        {
            get
            {
                ReadUnloadedTable();

                return m_SourceId;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Members

        private UInt32 m_LastUpdated;
        private TimeSpan m_PosixTime;
        private byte[] m_iso8601;
        private UInt32 m_SourceId;

        #endregion
    }

    /// <summary>
    /// The IPMfgTable2602 class handles the reading of the Boron IP Route Metrics table (554)
    /// </summary>
    internal class IPMfgTable2602 : AnsiTable
    {
        #region Constants

        private const ushort IP_ROUTE_METRICS_TLV_ID = 25;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="table2588"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //  12/12/12 AF  2.70.50 263124   Now calculate the table size and offsets based on
        //                                the dimension table (540)
        //
        public IPMfgTable2602(CPSEM psem, IPMfgTable2588 table2588)
            : base(psem, 2602, GetTableSize(table2588))
        {
            m_TLVRouteMetrics = null;
            m_Table2588 = table2588;
            
        }

        /// <summary>
        /// Full read of table 2602 (TLV IP Route Metrics) out of the meter.
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //  12/12/12 AF  2.70.50 263124   The number of records is now retrieved from
        //                                Mfg table 540 instead of being hard coded.
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "IPMfgTable2602.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_LastUpdated = m_Reader.ReadUInt32();

                m_TLVRouteMetrics = new List<TLVRouteMetricsItem>();
                Int32 iIndex;

                do
                {
                    iIndex = m_Reader.ReadInt32();
                    if (iIndex > 0)
                    {
                        TLVRouteMetricsItem RouteMetricsItem = new TLVRouteMetricsItem();


                        RouteMetricsItem.InetCidrRouteIndex = iIndex;
                        RouteMetricsItem.InstanceIndex = m_Reader.ReadInt32();
                        RouteMetricsItem.Rank = m_Reader.ReadInt32();
                        RouteMetricsItem.Hops = m_Reader.ReadInt32();
                        RouteMetricsItem.PathEtx = m_Reader.ReadInt32();
                        RouteMetricsItem.LinkEtx = m_Reader.ReadInt32();
                        RouteMetricsItem.RssiForward = m_Reader.ReadInt32();
                        RouteMetricsItem.RssiReverse = m_Reader.ReadInt32();
                        RouteMetricsItem.LqiForward = m_Reader.ReadInt32();
                        RouteMetricsItem.LqiReverse = m_Reader.ReadInt32();

                        m_TLVRouteMetrics.Add(RouteMetricsItem);
                    }
                } while ((iIndex > 0) && (iIndex < m_Table2588.CSMPTLVItemCounts[m_TLVRouteMetricsIndex]));

                if (m_TLVRouteMetrics.Count == 0)
                {
                    m_TLVRouteMetrics = null;
                }

            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns a list of the IP Route Metrics items
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public List<TLVRouteMetricsItem> IPRouteMetrics
        {
            get
            {
                Read();

                return m_TLVRouteMetrics;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the size of the table
        /// </summary>
        /// <param name="table2588">The size of the table depends on Mfg table 540</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263124 Created
        //
        private static uint GetTableSize(IPMfgTable2588 table2588)
        {
            uint TableSize = 0;

            m_TLVRouteMetricsIndex = table2588.CSMPSupportedTLVs.FindIndex(item => item.Equals(IP_ROUTE_METRICS_TLV_ID) == true);

            if ((m_TLVRouteMetricsIndex > 0) && (m_TLVRouteMetricsIndex < table2588.TLVListLength))
            {
                TableSize = (uint)(table2588.CSMPTLVSizes[m_TLVRouteMetricsIndex] + 4);
            } 

            return TableSize;
        }

        #endregion

        #region Members

        private UInt32 m_LastUpdated;
        private List<TLVRouteMetricsItem> m_TLVRouteMetrics;
        private static int m_TLVRouteMetricsIndex;
        private IPMfgTable2588 m_Table2588;

        #endregion
    }

    /// <summary>
    /// The IPMfgTable2604 class handles the reading of the Boron TLV Neighbors table
    /// </summary>
    internal class IPMfgTable2604 : AnsiTable
    {
        #region Constants

        private const ushort NEIGHBOR_802154G_TLV_ID = 52;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //  12/12/12 AF  2.70.50 263052   Now calculate the table size and offsets based on
        //                                the dimension table (540)
        //
        public IPMfgTable2604(CPSEM psem, IPMfgTable2588 table2588)
            : base(psem, 2604, GetTableSize(table2588))
        {
            m_TLVNeighbors = null;
            m_Table2588 = table2588;
        }

        /// <summary>
        /// Full read of table 2604 (TLV Neighbor Table) out of the meter.
        /// </summary>
        /// <returns>The PSEM response for the table read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //  02/14/12 AF  2.53.41 193884   Reverse the order of the physical address bytes
        //  12/12/12 AF  2.70.50 263052   The number of records is now retrieved from
        //                                Mfg table 540 instead of being hard coded.
        //  01/29/13 AF  2.70.62 288016   The neighbor addresses will no longer be in reverse byte order
        //                                starting with rfmesh f/w 5.2.25.  Now handling the reversal for
        //                                earlier builds in the user control
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "IPMfgTable2604.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_LastUpdated = m_Reader.ReadUInt32();

                m_TLVNeighbors = new List<TLVNeighborsItem>();
                Int32 iIndex;

                do
                {
                    iIndex = m_Reader.ReadInt32();
                    if (iIndex > 0)
                    {
                        TLVNeighborsItem NewNeighbor = new TLVNeighborsItem();

                        NewNeighbor.Index = iIndex;
                        NewNeighbor.PhysicalAddress = m_Reader.ReadBytes(16);

                        NewNeighbor.LastChanged = m_Reader.ReadUInt32();
                        NewNeighbor.RssiForward = m_Reader.ReadInt32();
                        NewNeighbor.RssiReverse = m_Reader.ReadInt32();
                        m_TLVNeighbors.Add(NewNeighbor);
                    }
                } while ((iIndex > 0) && (iIndex < m_Table2588.CSMPTLVItemCounts[m_TLVNeighborsTLVIndex]));

                if (m_TLVNeighbors.Count == 0)
                {
                    m_TLVNeighbors = null;
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns a list of Comm Module neighbor records
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //  02/16/12 AF  2.53.41 TREQ4722 Make sure that table is read each time since
        //                                neighbor data will be dynamic
        //
        public List<TLVNeighborsItem> Neighbors
        {
            get
            {
                Read();

                return m_TLVNeighbors;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the size of the table
        /// </summary>
        /// <param name="table2588">The size of the table depends on Mfg table 540</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263052 Created
        //
        private static uint GetTableSize(IPMfgTable2588 table2588)
        {
            uint TableSize = 0;

            m_TLVNeighborsTLVIndex = table2588.CSMPSupportedTLVs.FindIndex(item => item.Equals(NEIGHBOR_802154G_TLV_ID) == true);

            if ((m_TLVNeighborsTLVIndex > 0) && (m_TLVNeighborsTLVIndex < table2588.TLVListLength))
            {
                TableSize = (uint)(table2588.CSMPTLVSizes[m_TLVNeighborsTLVIndex] + 4);
            }

            return TableSize;
        }

        #endregion

        #region Members

        private List<TLVNeighborsItem> m_TLVNeighbors;
        private UInt32 m_LastUpdated;
        private static int m_TLVNeighborsTLVIndex;
        private IPMfgTable2588 m_Table2588;

        #endregion
    }

    /// <summary>
    /// The IPV6 Status Table
    /// </summary>
    internal class IPMfgTable2608 : AnsiTable
    {
        #region Constants

        private const int TABLE_SIZE = 160;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/14 jrf 3.70.01 529116 Created.
        public IPMfgTable2608(CPSEM psem)
            : base(psem, 2608, TABLE_SIZE)
        {
            m_CommenceTimer = 0;
        }

        /// <summary>
        /// Full read of table 2608 out of the meter.
        /// </summary>
        /// <returns>The PSEM response for the table read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/14 jrf 3.70.01 529116 Created.
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "IPMfgTable2608.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_LastNow = m_Reader.ReadUInt32();
                m_CommenceTimer = m_Reader.ReadInt32();
                //Todo: Implement remaining reads if needed.
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the value of the commence timer.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/14 jrf 3.70.01 529116 Created.
        public Int32 CommenceTimer
        {
            get
            {
                Read();

                return m_CommenceTimer;
            }
        }

        #endregion

        #region Members

        private UInt32 m_LastNow;
        private Int32 m_CommenceTimer;

        #endregion
    }

    /// <summary>
    /// The IPMfgTable2611 class handles the reading of the Boron UIP Stack Statistics table
    /// </summary>
    internal class IPMfgTable2611 : AnsiTable
    {
        #region Constants

        private const int VERSION_5_0_TABLE_2611_SIZE = 164;
        private const int VERSION_5_2_TABLE_2611_SIZE = 192;

        private const int IP_RECEIVED_OFFSET = 0;
        private const int IP_SENT_OFFSET = 4;
        private const int IPV6_DROPPED_OFFSET = 12;
        private const int UDP_DROPPED_OFFSET = 96;
        private const int UDP_CHKERR_OFFSET = 108;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        /// <param name="fltRegFWVersion">The register FW version for the current meter</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/11 AF  2.53.21 TC9229 Created
        //
        public IPMfgTable2611(CPSEM psem, float fltRegFWVersion)
            : base(psem, 2611, GetTableSize(fltRegFWVersion))
        {
            m_fltRegFWVersion = fltRegFWVersion;
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "IPMfgTable2611.Read");

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
        /// Reads the number of UDP packets dropped using an offset read
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created for Boron testing
        //
        public UInt32 UDPDropped
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;
                m_UDPDrop = 0;

                PSEMResult = m_PSEM.OffsetRead(2611, UDP_DROPPED_OFFSET, 4, out byaData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    // We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading UPD Dropped Count"));
                }
                else
                {
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_UDPDrop = Reader.ReadUInt32();
                }

                return m_UDPDrop;
            }
        }

        /// <summary>
        /// Reads the number of IP packets sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 JKW  2.70.xx TC11213 Created for Carbon testing
        //
        public UInt32 IPSent
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;
                m_IPSent = 0;

                PSEMResult = m_PSEM.OffsetRead(2611, IP_SENT_OFFSET, 4, out byaData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    // We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading IP Sent Count"));
                }
                else
                {
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_IPSent = Reader.ReadUInt32();
                }

                return m_IPSent;
            }
        }

        /// <summary>
        /// Reads the number of IP packets reveived
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 JKW  2.70.xx TC11213 Created for Carbon testing
        //
        public UInt32 IPReceived
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;
                m_IPRecv = 0;

                PSEMResult = m_PSEM.OffsetRead(2611, IP_RECEIVED_OFFSET, 4, out byaData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    // We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading IP Received Count"));
                }
                else
                {
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_IPRecv = Reader.ReadUInt32();
                }

                return m_IPRecv;
            }
        }

        /// <summary>
        /// Reads the number of UDP check errors using an offset read
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created for Boron testing
        //
        public UInt32 UDPCheckError
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;
                m_UDPChkerr = 0;

                PSEMResult = m_PSEM.OffsetRead(2611, UDP_CHKERR_OFFSET, 4, out byaData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    // We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading UPD Check Error Count"));
                }
                else
                {
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_UDPChkerr = Reader.ReadUInt32();
                }

                return m_UDPChkerr;
            }
        }

        /// <summary>
        /// Reads the number of IPv6 packets that have been dropped using an offset read
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created for Boron testing
        //
        public UInt32 IPv6Dropped
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;
                m_IPDrop = 0;

                PSEMResult = m_PSEM.OffsetRead(2611, IPV6_DROPPED_OFFSET, 4, out byaData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    // We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading UPD Check Error Count"));
                }
                else
                {
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_IPDrop = Reader.ReadUInt32();
                }

                return m_IPDrop;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the size of the table
        /// </summary>
        /// <param name="fltRegFWRevision">the firmware version running in the meter</param>
        /// <returns>the size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 returns the size of the table in bytes
        //
        private static uint GetTableSize(float fltRegFWRevision)
        {
            uint TableSize = 0;

            if (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_BORON_5_2) >= 0)
            {
                TableSize = VERSION_5_2_TABLE_2611_SIZE;
            }
            else
            {
                TableSize = VERSION_5_0_TABLE_2611_SIZE;
            }


            return TableSize;
        }


        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created
        //
        private void ParseData()
        {
            m_IPRecv = m_Reader.ReadUInt32();
            m_IPSent = m_Reader.ReadUInt32();
            m_IPForwarded = m_Reader.ReadUInt32();
            m_IPDrop = m_Reader.ReadUInt32();
            m_IPVhlerr = m_Reader.ReadUInt32();
            m_IPHblenerr = m_Reader.ReadUInt32();
            m_IPLblenerr = m_Reader.ReadUInt32();
            m_IPFragerr = m_Reader.ReadUInt32();
            m_IPChkerr = m_Reader.ReadUInt32();
            m_IPProtoerr = m_Reader.ReadUInt32();

            m_ICMPRecv = m_Reader.ReadUInt32();
            m_ICMPSent = m_Reader.ReadUInt32();
            m_ICMPDrop = m_Reader.ReadUInt32();
            m_ICMPTypeerr = m_Reader.ReadUInt32();
            m_ICMPChkerr = m_Reader.ReadUInt32();

            m_TCPRecv = m_Reader.ReadUInt32();
            m_TCPSent = m_Reader.ReadUInt32();
            m_TCPDrop = m_Reader.ReadUInt32();
            m_TCPChkerr = m_Reader.ReadUInt32();
            m_TCPAckerr = m_Reader.ReadUInt32();
            m_TCPRst = m_Reader.ReadUInt32();
            m_TCPRexmit = m_Reader.ReadUInt32();
            m_TCPSyndrop = m_Reader.ReadUInt32();
            m_TCPSynrst = m_Reader.ReadUInt32();

            m_UDPDrop = m_Reader.ReadUInt32();
            m_UDPRecv = m_Reader.ReadUInt32();
            m_UDPSent = m_Reader.ReadUInt32();
            m_UDPChkerr = m_Reader.ReadUInt32();

            m_UDPC1222Drop = m_Reader.ReadUInt32();
            m_UDPC1222Recv = m_Reader.ReadUInt32();
            m_UDPC1222Sent = m_Reader.ReadUInt32();
            m_UDPC1222Chkerr = m_Reader.ReadUInt32();

            m_UDPC1222QoSDrop = m_Reader.ReadUInt32();
            m_UDPC1222QoSRecv = m_Reader.ReadUInt32();
            m_UDPC1222QoSSent = m_Reader.ReadUInt32();
            m_UDPC1222QoSChkerr = m_Reader.ReadUInt32();

            m_ND6Drop = m_Reader.ReadUInt32();
            m_ND6CRecv = m_Reader.ReadUInt32();
            m_ND6Sent = m_Reader.ReadUInt32();
            m_ND6CrcError = m_Reader.ReadUInt32();
            m_ND6RxTooBigError = m_Reader.ReadUInt32();

            if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_BORON_5_2) >= 0)
            {
                m_LCPTxCount = m_Reader.ReadUInt32();
                m_LCPRxCount = m_Reader.ReadUInt32();
                m_IPV6CPTxCount = m_Reader.ReadUInt32();
                m_IPV6CPRxCount = m_Reader.ReadUInt32();
                m_SPPPEncryptedTxCount = m_Reader.ReadUInt32();
                m_SPPPDecryptedRxCount = m_Reader.ReadInt32();
                m_SPPPDecryptRxFailures = m_Reader.ReadUInt32();
            }
            else
            {
                m_LCPTxCount = 0;
                m_LCPRxCount = 0;
                m_IPV6CPTxCount = 0;
                m_IPV6CPRxCount = 0;
                m_SPPPEncryptedTxCount = 0;
                m_SPPPDecryptedRxCount = 0;
                m_SPPPDecryptRxFailures = 0;
            }
        }

        #endregion

        #region Members

        private float m_fltRegFWVersion;
        private uint m_IPRecv;
        private uint m_IPSent;
        private uint m_IPForwarded;
        private uint m_IPDrop;
        private uint m_IPVhlerr;
        private uint m_IPHblenerr;
        private uint m_IPLblenerr;
        private uint m_IPFragerr;
        private uint m_IPChkerr;
        private uint m_IPProtoerr;

        private uint m_ICMPRecv;
        private uint m_ICMPSent;
        private uint m_ICMPDrop;
        private uint m_ICMPTypeerr;
        private uint m_ICMPChkerr;

        private uint m_TCPRecv;
        private uint m_TCPSent;
        private uint m_TCPDrop;
        private uint m_TCPChkerr;
        private uint m_TCPAckerr;
        private uint m_TCPRst;
        private uint m_TCPRexmit;
        private uint m_TCPSyndrop;
        private uint m_TCPSynrst;

        private uint m_UDPDrop;
        private uint m_UDPRecv;
        private uint m_UDPSent;
        private uint m_UDPChkerr;

        private uint m_UDPC1222Drop;
        private uint m_UDPC1222Recv;
        private uint m_UDPC1222Sent;
        private uint m_UDPC1222Chkerr;

        private uint m_UDPC1222QoSDrop;
        private uint m_UDPC1222QoSRecv;
        private uint m_UDPC1222QoSSent;
        private uint m_UDPC1222QoSChkerr;

        private uint m_ND6Drop;
        private uint m_ND6CRecv;
        private uint m_ND6Sent;
        private uint m_ND6CrcError;
        private uint m_ND6RxTooBigError;

        private uint m_LCPTxCount;
        private uint m_LCPRxCount;
        private uint m_IPV6CPTxCount;
        private uint m_IPV6CPRxCount;
        private uint m_SPPPEncryptedTxCount;
        private int m_SPPPDecryptedRxCount;
        private uint m_SPPPDecryptRxFailures;

        #endregion
    }

    /// <summary>
    /// Class that represents a single Route Metrics Item
    /// </summary>
    public class TLVRouteMetricsItem
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public TLVRouteMetricsItem()
        {
            m_InetCidrRouteIndex = 0;
            m_InstanceIndex = 0;
            m_Rank = 0;
            m_Hops = 0;
            m_PathEtx = 0;
            m_LinkEtx = 0;
            m_RssiForward = 0;
            m_RssiReverse = 0;
            m_LqiForward = 0;
            m_LqiReverse = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// InetCidrRouteIndex field.  Must be non-zero to be valid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 InetCidrRouteIndex
        {
            get
            {
                return m_InetCidrRouteIndex;
            }
            set
            {
                m_InetCidrRouteIndex = value;
            }
        }

        /// <summary>
        /// Instance Index field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 InstanceIndex
        {
            get
            {
                return m_InstanceIndex;
            }
            set
            {
                m_InstanceIndex = value;
            }
        }

        /// <summary>
        /// Rank field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 Rank
        {
            get
            {
                return m_Rank;
            }
            set
            {
                m_Rank = value;
            }
        }

        /// <summary>
        /// Hops.  Have to add 1 to the value read from the table to determine
        /// the number of hops from the CG Mesh
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 Hops
        {
            get
            {
                return m_Hops;
            }
            set
            {
                m_Hops = value;
            }
        }

        /// <summary>
        /// Path Etx.  Have to divide table value by 256 for display.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 PathEtx
        {
            get
            {
                return m_PathEtx;
            }
            set
            {
                m_PathEtx = value;
            }
        }

        /// <summary>
        /// Link Etx. Have to divide table value by 256 for display.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 LinkEtx
        {
            get
            {
                return m_LinkEtx;
            }
            set
            {
                m_LinkEtx = value;
            }
        }

        /// <summary>
        /// RSSI forward
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 RssiForward
        {
            get
            {
                return m_RssiForward;
            }
            set
            {
                m_RssiForward = value;
            }
        }

        /// <summary>
        /// RSSI reverse
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 RssiReverse
        {
            get
            {
                return m_RssiReverse;
            }
            set
            {
                m_RssiReverse = value;
            }
        }

        /// <summary>
        /// LQI forward
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 LqiForward
        {
            get
            {
                return m_LqiForward;
            }
            set
            {
                m_LqiForward = value;
            }
        }

        /// <summary>
        /// LQI reverse
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //
        public Int32 LqiReverse
        {
            get
            {
                return m_LqiReverse;
            }
            set
            {
                m_LqiReverse = value;
            }
        }

        #endregion

        #region Members

        private Int32 m_InetCidrRouteIndex;
        private Int32 m_InstanceIndex;
        private Int32 m_Rank;
        private Int32 m_Hops;
        private Int32 m_PathEtx;
        private Int32 m_LinkEtx;
        private Int32 m_RssiForward;
        private Int32 m_RssiReverse;
        private Int32 m_LqiForward;
        private Int32 m_LqiReverse;

        #endregion
    }

    /// <summary>
    /// Class that represents a single Comm Module neighbor record.
    /// </summary>
    public class TLVNeighborsItem
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //
        public TLVNeighborsItem()
        {
            m_Index = 0;
            m_PhysicalAddress = new byte[16];
            m_LastChanged = 0;
            m_RssiForward = 0;
            m_RssiReverse = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Index field.  Must be non-zero to be valid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //
        public Int32 Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }

        /// <summary>
        /// Physical address of the neighbor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //
        public byte[] PhysicalAddress
        {
            get
            {
                return m_PhysicalAddress;
            }
            set
            {
                m_PhysicalAddress = value;
            }
        }

        /// <summary>
        /// Last changed field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //
        public UInt32 LastChanged
        {
            get
            {
                return m_LastChanged;
            }
            set
            {
                m_LastChanged = value;
            }
        }

        /// <summary>
        /// RSSI forward
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //
        public Int32 RssiForward
        {
            get
            {
                return m_RssiForward;
            }
            set
            {
                m_RssiForward = value;
            }
        }

        /// <summary>
        /// RSSI reverse
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //
        public Int32 RssiReverse
        {
            get
            {
                return m_RssiReverse;
            }
            set
            {
                m_RssiReverse = value;
            }
        }

        #endregion

        #region Members

        private Int32 m_Index;
        private byte[] m_PhysicalAddress;
        private UInt32 m_LastChanged;
        private Int32 m_RssiForward;
        private Int32 m_RssiReverse;

        #endregion
    }

    /// <summary>
    /// Class to represent the firmware version info object from Mfg table 532
    /// </summary>
    public class FirmwareVersionInfo
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/17/12 AF  2.70.30 TC11218 Created
        //
        public FirmwareVersionInfo()
        {
            m_Type = 0;
            m_Version = 0;
            m_Revision = 0;
            m_Build = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byType">firmware type</param>
        /// <param name="byVersion">firmware version</param>
        /// <param name="byRevision">firmware revision</param>
        /// <param name="byBuild">firmware build</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/12 AF  2.70.30 TC11218 Created
        //
        public FirmwareVersionInfo(byte byType, byte byVersion, byte byRevision, byte byBuild)
        {
            m_Type = byType;
            m_Version = byVersion;
            m_Revision = byRevision;
            m_Build = byBuild;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets the firmware type 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/12 AF  2.70.30 TC11218 Created
        //
        public byte Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }

        /// <summary>
        /// Gets/sets the firmware version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/12 AF  2.70.30 TC11218 Created
        //
        public byte Version
        {
            get
            {
                return m_Version;
            }
            set
            {
                m_Version = value;
            }
        }

        /// <summary>
        /// Gets/sets the firmware revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/12 AF  2.70.30 TC11218 Created
        //
        public byte Revision
        {
            get
            {
                return m_Revision;
            }
            set
            {
                m_Revision = value;
            }
        }

        /// <summary>
        /// Gets/sets the firmware build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/12 AF  2.70.30 TC11218 Created
        //
        public byte Build
        {
            get
            {
                return m_Build;
            }
            set
            {
                m_Build = value;
            }
        }

        #endregion

        #region Members

        byte m_Type;
        byte m_Version;
        byte m_Revision;
        byte m_Build;

        #endregion

    }

    /// <summary>
    /// Class to represent the notch information object from Mfg table 532
    /// </summary>
    public class NotchInfo
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/03/12 AF  2.70.45 WR245533 Created
        //
        public NotchInfo()
        {
            m_StartChannel = 0;
            m_StopChannel = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets the start channel of the notch info
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/03/12 AF  2.70.45 WR245533 Created
        //
        public UInt32 StartChannel
        {
            get
            {
                return m_StartChannel;
            }
            set
            {
                m_StartChannel = value;
            }
        }

        /// <summary>
        /// Gets/sets the stop channel of the notch info
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/03/12 AF  2.70.45 WR245533 Created
        //
        public UInt32 StopChannel
        {
            get
            {
                return m_StopChannel;
            }
            set
            {
                m_StopChannel = value;
            }
        }

        #endregion

        #region Members

        private UInt32 m_StartChannel;
        private UInt32 m_StopChannel;

        #endregion
    }

    /// <summary>
    /// Class to represent the dwell information object from Mfg table 532
    /// </summary>
    public class DwellInfo
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/03/12 AF  2.70.45 WR245533 Created
        //
        public DwellInfo()
        {
            m_Window = 0;
            m_MaxDwell = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets the Window field of the Dwell Info object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/03/12 AF  2.70.45 WR245533 Created
        //
        public UInt32 Window
        {
            get
            {
                return m_Window;
            }
            set
            {
                m_Window = value;
            }
        }

        /// <summary>
        /// Gets/sets the Max Dwell field of the Dwell info object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/03/12 AF  2.70.45 WR245533 Created
        //
        public UInt32 MaxDwell
        {
            get
            {
                return m_MaxDwell;
            }
            set
            {
                m_MaxDwell = value;
            }
        }

        #endregion

        #region Members

        private UInt32 m_Window;
        private UInt32 m_MaxDwell;

        #endregion
    }
}
