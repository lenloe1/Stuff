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
//                           Copyright © 2010 - 2016
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
    /// M2 Gateway's Mfg table 2048.  This table is different from other OpenWay meters.
    /// It has only the event configuration, the configuration date, DST enabled, 
    /// and customer serial number
    /// </summary>
    internal class M2GatewayTable2048 : AnsiTable
    {
        #region Constants

        private const int NBR_MFG_EVENT_BYTES = 35;
        private const int SERIAL_NBR_LENGTH = 20;
        private const int TABLE_LENGTH_2048 = 6 + NBR_MFG_EVENT_BYTES + NBR_MFG_EVENT_BYTES + SERIAL_NBR_LENGTH;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/27/10 AF  2.41.06        Created
        //  07/01/10 AF  2.42.01        Modified for new table layout
        //
        public M2GatewayTable2048(CPSEM psem)
            : base(psem, 2048, TABLE_LENGTH_2048)
        {
            m_abyHistoryLogEventsMonitored = new byte[NBR_MFG_EVENT_BYTES];
            m_abyEventLogEventsMonitored = new byte[NBR_MFG_EVENT_BYTES];
            m_CustomerSerialNum = new char[SERIAL_NBR_LENGTH];
        }

        /// <summary>
        /// Reads Mfg table 2048 out of the meter
        /// </summary>
        /// <returns>Result code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/27/10 AF  2.41.06        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

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
        /// Customer Serial Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/27/10 AF  2.41.06        Created
        //
        public string CustomerSerialNumber
        {
            get
            {
                ReadUnloadedTable();

                string strSerialNum = new string(m_CustomerSerialNum);

                return strSerialNum;
            }
        }

        /// <summary>
        /// Gets the date of the last configuration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/27/10 AF  2.41.06        Created
        //  07/01/10 AF  2.42.01        Changed the data type to a DateTime
        //
        public DateTime DateProgrammed
        {
            get
            {
                ReadUnloadedTable();

                return m_ConfigDate;
            }
        }

        /// <summary>
        /// Returns whether or not DST is enabled in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/01/10 AF  2.42.01        Created
        //
        public bool DSTEnabled
        {
            get
            {
                bool bEnabled = false;

                ReadUnloadedTable();

                if (0 == m_DSTEnabled)
                {
                    bEnabled = false;
                }
                else
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the contents of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/27/10 AF  2.41.06        Created
        //  07/01/10 AF  2.42.01        Modified for new table layout
        //
        private void ParseData()
        {
            m_abyHistoryLogEventsMonitored = m_Reader.ReadBytes(NBR_MFG_EVENT_BYTES);
            m_abyEventLogEventsMonitored = m_Reader.ReadBytes(NBR_MFG_EVENT_BYTES);
            m_ConfigDate = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_DSTEnabled = m_Reader.ReadByte();
            m_CustomerSerialNum = m_Reader.ReadChars(20);
        }

        #endregion

        #region Members

        private byte[] m_abyHistoryLogEventsMonitored;
        private byte[] m_abyEventLogEventsMonitored;
        private DateTime m_ConfigDate;
        private byte m_DSTEnabled;
        private char[] m_CustomerSerialNum;

        #endregion

    }
        
    /// <summary>
    /// M2 Gateway's Mfg table 2108
    /// </summary>
    internal class M2GatewayTable2108 : AnsiTable
    {
        #region Constants

        private const int TABLE_LENGTH_2108 = 16;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/10 AF  2.41.07        Created
        //
        public M2GatewayTable2108(CPSEM psem)
            : base(psem, 2108, TABLE_LENGTH_2108)
        {

        }

        /// <summary>
        /// Full read of M2 Gateway's table 2108 out of the meter
        /// </summary>
        /// <returns>Result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/10 AF  2.41.07        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

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
        /// Returns the type - RFLAN or IP
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public string CommModuleType
        {
            get
            {
                ReadUnloadedTable();

                return OpenWayMfgTable2108.TranslateCommModuleType(m_CommType);
            }
        }

        /// <summary>
        /// Gets Comm Module type as a byte
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public byte CommModuleTypeByte
        {
            get
            {
                ReadUnloadedTable();

                return m_CommType;
            }
        }

        /// <summary>
        /// Gets the Comm Module version.revision from 2108
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public string CommModuleVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_CommFWVer.ToString(CultureInfo.CurrentCulture) + "." 
                            + m_CommFWRev.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the Comm Module firmware build number from 2108
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public string CommModuleBuild
        {
            get
            {
                ReadUnloadedTable();

                return m_CommFWBuild.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the Version of the Comm Module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  
        public byte CommVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_CommFWVer;
            }
        }

        /// <summary>
        /// Gets the Revision of the Comm Module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  
        public byte CommRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_CommFWRev;
            }
        }

        /// <summary>
        /// Gets the Build of the Comm Module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  
        public byte CommBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_CommFWBuild;
            }
        }

        /// <summary>
        /// Gets the HAN type from table 2108
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public string HanModuleType
        {
            get
            {
                ReadUnloadedTable();

                return OpenWayMfgTable2108.TranslationHanModType(m_HanType);
            }
        }

        /// <summary>
        /// Gets the HAN Module type as a byte
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public byte HanModuleTypeByte
        {
            get
            {
                ReadUnloadedTable();

                return m_HanType;
            }
        }

        /// <summary>
        /// Gets the firmware version of the ZigBee module
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created  
        //
        public string HanModuleVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_HanFWVer.ToString(CultureInfo.CurrentCulture) + "." 
                            + m_HanFWRev.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the firmware build of the ZigBee module
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public string HanModuleBuild
        {
            get
            {
                ReadUnloadedTable();

                return m_HanFWBuild.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the firmware version.revision of the Gateway module
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created  
        //
        public string GatewayModuleVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_GatewayFWVer.ToString(CultureInfo.CurrentCulture) + "."
                            + m_GatewayFWRev.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the Gateway module firmware build number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public string GatewayModuleBuild
        {
            get
            {
                ReadUnloadedTable();

                return m_GatewayFWBuild.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the L+G M2 module register firmware version.revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public string M2ModuleVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_LGRegFWVer.ToString(CultureInfo.CurrentCulture) + "."
                            + m_LGRegFWRev.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the Version of the L+G M2 module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/03/10 AF  2.45.10 160590 Created  
        //
        public byte M2ModuleVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_LGRegFWVer;
            }
        }

        /// <summary>
        /// Gets the Revision of the L+G M2 module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/03/10 AF  2.45.10 160590 Created   
        //
        public byte M2ModuleRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_LGRegFWRev;
            }
        }


        /// <summary>
        /// Gets the L+G M2 module register firmware build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.07        Created
        //
        public string M2ModuleBuild
        {
            get
            {
                ReadUnloadedTable();

                return m_LGRegFWBuild.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the Build of the L+G M2 module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/27/10 AF  2.45.10 160590 Created  
        //
        public byte M2ModuleBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_LGRegFWBuild;
            }
        }

        /// <summary>
        /// Gets HAN Module type as a byte
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/27/10 AF  2.45.10 160590 Created 
        //
        public byte HANModuleTypeByte
        {
            get
            {
                ReadUnloadedTable();

                return m_HanType;
            }
        }

        /// <summary>
        /// Gets the Version of the HAN firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/10 AF  2.41.07    Created

        public byte HANVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HanFWVer;
            }
        }

        /// <summary>
        /// Gets the Revision of the HAN firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/10 AF  2.41.07 N/A    Created
        //
        public byte HANRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HanFWRev;
            }
        }

        /// <summary>
        /// Gets the Build of the HAN firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/10 AF  2.41.07 N/A    Created
        //
        public byte HANBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HanFWBuild;
            }
        }

        /// <summary>
        /// Gets the Version of the M2 Gateway firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/27/10 AF  2.45.10 160590 Created
        //
        public byte M2GVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_GatewayFWVer;
            }
        }

        /// <summary>
        /// Gets the Revision of the M2 Gateway firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/27/10 AF  2.45.10 160590 Created  
        //
        public byte M2GRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_GatewayFWRev;
            }
        }

        /// <summary>
        /// Gets the build of the M2 Gateway firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/27/10 AF  2.45.10 160590 Created  
        //
        public byte M2GBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_GatewayFWBuild;
            }
        }

        /// <summary>
        /// L+G Firmware Engineering Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/08/11 AF  2.51.08 174361 Created
        //
        public byte M2ModuleEngVerOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_MeterFWEngVer;
            }
        }

        /// <summary>
        /// L+G Firmware Engineering Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/08/11 AF  2.51.08 174361 Created
        //
        public string M2ModuleEngVer
        {
            get
            {
                ReadUnloadedTable();

                return m_MeterFWEngVer.ToString("d3", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// L+G firmware FCN
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/08/11 AF  2.51.08 174361 Created
        //
        public byte M2ModuleFCNOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_MeterFWFCN;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the contents of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/10 AF  2.41.07        Created
        //
        private void ParseData()
        {
            m_CommType = m_Reader.ReadByte();
            m_CommFWVer = m_Reader.ReadByte();
            m_CommFWRev = m_Reader.ReadByte();
            m_CommFWBuild = m_Reader.ReadByte();
            m_HanType = m_Reader.ReadByte();
            m_HanFWVer = m_Reader.ReadByte();
            m_HanFWRev = m_Reader.ReadByte();
            m_HanFWBuild = m_Reader.ReadByte();
            m_GatewayFWVer = m_Reader.ReadByte();
            m_GatewayFWRev = m_Reader.ReadByte();
            m_GatewayFWBuild = m_Reader.ReadByte();
            m_LGRegFWVer = m_Reader.ReadByte();
            m_LGRegFWRev = m_Reader.ReadByte();
            m_LGRegFWBuild = m_Reader.ReadByte();
            m_MeterFWEngVer = m_Reader.ReadByte();
            m_MeterFWFCN = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        private byte m_CommType;
        private byte m_CommFWVer;
        private byte m_CommFWRev;
        private byte m_CommFWBuild;
        private byte m_HanType;
        private byte m_HanFWVer;
        private byte m_HanFWRev;
        private byte m_HanFWBuild;
        private byte m_GatewayFWVer;
        private byte m_GatewayFWRev;
        private byte m_GatewayFWBuild;
        private byte m_LGRegFWVer;
        private byte m_LGRegFWRev;
        private byte m_LGRegFWBuild;
        private byte m_MeterFWEngVer;
        private byte m_MeterFWFCN;

        #endregion

    }

    /// <summary>
    /// M2 Gateway's Mfg table 2205
    /// </summary>
    internal class M2GatewayTable2205 : AnsiTable
    {
        #region Constants

        private const int TABLE_LENGTH_2205 = 16;
        private const byte FATAL_ERROR_PRESENT = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/10 AF  2.45.07 160590 Created
        //  03/02/12 AF  2.53.47 192934 Removed firmware version parameters - no longer needed
        //
        public M2GatewayTable2205(CPSEM psem)
            : base(psem, 2205, TABLE_LENGTH_2205)
        {
        }


        /// <summary>
        /// Full read of M2 Gateway's table 2205 out of the meter
        /// </summary>
        /// <returns>Result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/10 AF  2.45.07 160590 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

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
        /// Determines whether a fatal error has occurred in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/10 AF  2.45.07 160590 Create
        //  12/23/10 AF  2.45.22        Changed according to suggestions in code review
        //  02/02/11 AF  2.50.04        Raised the minimum build after redesign of the table
        //  03/02/12 AF  2.53.47 192934 Removed firmware version check - no longer needed
        //
        public bool IsFatalErrorPresent
        {
            get
            {
                bool blnPresent;

                ReadUnloadedTable();

                blnPresent = ((m_FatalErrors & FATAL_ERROR_PRESENT) == FATAL_ERROR_PRESENT);

                return blnPresent;
            }
        }

        /// <summary>
        /// Gets the reason the Fatal Error occurred.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/10 AF  2.45.10 160590 Created
        //  12/23/10 AF  2.45.22        Changed according to suggestions in code review
        //  02/02/11 AF  2.50.04        Raised the minimum build after redesign of the table
        //  03/02/12 AF  2.53.47 192934 Removed firmware version check - no longer needed
        //
        public ushort FatalErrorReason
        {
            get
            {
                ushort usReason = 0;

                ReadUnloadedTable();

                usReason = m_FatalErrorReason;

                return usReason;
            }
        }

        /// <summary>
        /// Gets the fatal errors field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/10 AF  2.45.10 160590 Created
        //  12/23/10 AF  2.45.22        Changed according to suggestions in code review
        //  02/02/11 AF  2.50.04        Raised the minimum build after redesign of the table
        //  03/02/12 AF  2.53.47 192934 Removed firmware version check - no longer needed
        //
        public byte FatalErrors
        {
            get
            {
                byte byFatalErrors;

                ReadUnloadedTable();
                byFatalErrors = m_FatalErrors;

                return byFatalErrors;
            }
        }

        /// <summary>
        /// Gets the status of the fatal error. Can be interpreted to determine if
        /// a core dump exists, if fatal error recovery is enabled, and if the meter
        /// is in fatal error recovery mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/11 AF  2.50.01        Created
        //  03/02/12 AF  2.53.47 192934 Removed firmware version check - no longer needed
        //
        public byte FatalErrorStatus
        {
            get
            {
                byte byFatalErrorStatus;

                ReadUnloadedTable();
                byFatalErrorStatus = m_FatalErrorStatus;

                return byFatalErrorStatus;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the contents of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/10 AF  2.45.05 160590 Created
        //  02/02/11 AF  2.50.04        Rewritten due to redesign of the table
        //
        private void ParseData()
        {
            m_EPFDataVersion = m_Reader.ReadByte();
            m_EPFDataRevision = m_Reader.ReadByte();
            m_EPFCount = m_Reader.ReadUInt16();
            m_TotallsrMouseTrapCount = m_Reader.ReadUInt32();
            m_FatalErrorReason = m_Reader.ReadUInt16();
            m_FatalErrors = m_Reader.ReadByte();
            m_FatalErrorStatus = m_Reader.ReadByte();
            m_FatalRecoveryCount = m_Reader.ReadByte();
            m_FatalRecoveryError = m_Reader.ReadByte();
            m_FatalRecoverStatus = m_Reader.ReadByte();
            m_EPFShutdownReason = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        private byte m_EPFDataVersion;
        private byte m_EPFDataRevision;
        private UInt16 m_EPFCount;
        private UInt32 m_TotallsrMouseTrapCount;
        private UInt16 m_FatalErrorReason;
        private byte m_FatalErrors;
        private byte m_FatalErrorStatus;
        private byte m_FatalRecoveryCount;
        private byte m_FatalRecoveryError;
        private byte m_FatalRecoverStatus;
        private byte m_EPFShutdownReason;
                   
        #endregion
    }

    /// <summary>
    /// Class that represents the history log configuration data stored in table 2048
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue#    Description
    //  -------- --- ------- ------    -------------------------------------------
    //  04/21/11 AF  2.50.31 167587    Changed the parent class to ANSISubTable
    //  08/05/16 MP  4.70.11 WR674048  Changed Power outage and power restored to "Primary Power Down" and "Primary Power Up",
    //                                    also removed the "ed" from pending table activate/clear failed
    public class M2_Gateway_HistoryLogConfig : ANSISubTable
    {
        #region Constants

        /// <summary>
        /// 
        /// </summary>
        public const ushort EVENT_CONFIG_SIZE = 35;

        #endregion

        #region Definitions

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_0_15
        {
            /// <summary>Power Outage - Index 1</summary>
            PRIMARY_POWER_DOWN = 0x02,
            /// <summary>Power Restored - Index 2</summary>
            PRIMARY_POWER_UP = 0x04,
            /// <summary>Clear Billing Data - Index 3</summary>
            CLEAR_BILLING_DATA = 0x08,
            /// <summary>Billing Schedule Expirted - Index 4</summary>
            BILLING_SCHED_EXPIRED = 0x10,
            /// <summary>DST Time Change - Index 5</summary>
            DST_TIME_CHANGE = 0x20,
            /// <summary>Clock Reset - Index 6</summary>
            CLOCK_RESET = 0x40,
            /// <summary>Demand Threshold Exceeded - Index 7</summary>
            DEMAND_THRESHOLD_EXCEEDED = 0x80,
            /// <summary>Demand Threshold Restored - Index 8</summary>
            DEMAND_THRESHOLD_RESTORED = 0x0100,
            /// <summary>Logon Successful - Index 10</summary>
            LOGON_SUCCESSFUL = 0x0400,
            /// <summary>Security Successful - Index 12</summary>
            SECURITY_SUCCESSFUL = 0x1000,
            /// <summary>Security Failed - Index 13</summary>
            SECURITY_FAILED = 0x2000,
            /// <summary>Load Profile Reset - Index 14</summary>
            LOAD_PROFILE_RESET = 0x4000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_16_31
        {
            /// <summary>History Log Cleared - Index 16</summary>
            HIST_LOG_CLEARED = 0x01,
            /// <summary>History Pointers Updated - Index 17</summary>
            HIST_PTRS_UPDATED = 0x02,
            /// <summary>Event Log Cleared - Index 18</summary>
            EVENT_LOG_CLEARED = 0x04,
            /// <summary>Event Log Pointers Updated - Index 19</summary>
            EVENT_LOG_PTRS_UPDATED = 0x08,
            /// <summary>Demand Reset - Index 20</summary>
            DEMAND_RESET = 0x10,
            /// <summary>Self Read Occurred - Index 21</summary>
            SELF_READ_OCCURRED = 0x20,
            /// <summary>Input Channel Hi - Index 22</summary>
            INPUT_CHANNEL_HI = 0x40,
            /// <summary>Input Channel Low - Index 23</summary>
            INPUT_CHANNEL_LO = 0x80,
            /// <summary>TOU Season Changed - Index 24</summary>
            TOU_SEASON_CHANGED = 0x100,
            /// <summary>TOU Rate Change - Index 25</summary>
            RATE_CHANGE = 0x200,
            /// <summary>External Event - Index 26</summary>
            EXTERNAL_EVENT = 0x400,
            /// <summary>SiteScan for AMI; Custom Schedule Changed - V and I and C12.19 - Index 27</summary>
            SITESCAN_OR_CUSTOM_SCHED_ERROR = 0x800,
            /// <summary>Pending table Activation - Index 28</summary>
            PENDING_TABLE_ACTIVATION = 0x1000,
            /// <summary>SiteScan for Non-AMI; Pending Table Clear for AMI - Index 29</summary>
            SITESCAN_OR_PENDING_TABLE_CLEAR = 0x2000,
            /// <summary>VQ Log Pointers Updated - Index 30</summary>
            VQ_LOG_PTRS_UPDATED = 0x4000,
            /// <summary>VQ Log Nearly Full - Index 31</summary>
            VQ_LOG_NEARLY_FULL = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_32_47
        {
            /// <summary>Enter Test Mode - Index 32</summary>
            ENTER_TEST_MODE = 0x01,
            /// <summary>Exit Test Mode - Index 33</summary>
            EXIT_TEST_MODE = 0x02,
            /// <summary>ABC Phase Rotation Active - Index 34</summary>
            ABC_PH_ROTATION_ACTIVE = 0x04,
            /// <summary>CBA Phase Rotation Active - Index 35</summary>
            CBA_PH_ROTATION_ACTIVE = 0x08,
            /// <summary>Meter Reprogrammed - Index 36</summary>
            METER_REPROGRAMMED = 0x10,
            /// <summary>Illegal Configuration Error - Index 37</summary>
            ILLEGAL_CONFIG_ERROR = 0x20,
            /// <summary>CPC Communication Error - Index 38</summary>
            CPC_COMM_ERROR = 0x40,
            /// <summary>Reverse Rotation Restored - Index 39</summary>
            REVERSE_ROTATION_RESTORE = 0x80,
            /// <summary>VQ Log Cleared - Index 40</summary>
            VQ_LOG_CLEARED = 0x100,
            /// <summary>TOU Schedule Error - Index 41</summary>
            TOU_SCHEDULE_ERROR = 0x200,
            /// <summary>Mass Memory Error - Index 42</summary>
            MASS_MEMORY_ERROR = 0x400,
            /// <summary>Loss of Phase Restore - Index 43</summary>
            LOSS_OF_PHASE_RESTORE = 0x800,
            /// <summary>Low Battery - Index 44</summary>
            LOW_BATTERY = 0x1000,
            /// <summary>Loss of Voltage phase A or Loss of Phase for OpenWay- Index 45</summary>
            LOSS_VOLTAGE_A_OR_LOSS_OF_PHASE = 0x2000,
            /// <summary>Register Full Scale - Index 46</summary>
            REGISTER_FULL_SCALE = 0x4000,
            /// <summary>Reverse Power Flow Restore - Index 47</summary>
            REVERSE_POWER_FLOW_RESTORE = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_48_63
        {
            /// <summary>Reverse Power Flow - Index 48</summary>
            REVERSE_POWER_FLOW = 0x01,
            /// <summary>Site Scan Diagnostic 1 Active - Index 49</summary>
            SS_DIAG1_ACTIVE = 0x02,
            /// <summary>Site Scan Diagnostic 2 Active - Index 50</summary>
            SS_DIAG2_ACTIVE = 0x04,
            /// <summary>Site Scan Diagnostic 3 Active - Index 51</summary>
            SS_DIAG3_ACTIVE = 0x08,
            /// <summary>Site Scan Diagnostic 4 Active - Index 52</summary>
            SS_DIAG4_ACTIVE = 0x10,
            /// <summary>Site Scan Diagnostic 5 Active - Index 53</summary>
            SS_DIAG5_ACTIVE = 0x20,
            /// <summary>Site Scan Diagnostic 1 Inactive - Index 54</summary>
            SS_DIAG1_INACTIVE = 0x40,
            /// <summary>Site Scan Diagnostic 2 Inactive - Index 55</summary>
            SS_DIAG2_INACTIVE = 0x80,
            /// <summary>Site Scan Diagnostic 3 Inactive - Index 56</summary>
            SS_DIAG3_INACTIVE = 0x100,
            /// <summary>Site Scan Diagnostic 4 Inactive - Index 57</summary>
            SS_DIAG4_INACTIVE = 0x200,
            /// <summary>Site Scan Diagnostic 5 Inactive - Index 58</summary>
            SS_DIAG5_INACTIVE = 0x400,
            /// <summary>Site Scan Diagnostic 6 Active - Index 59</summary>
            SS_DIAG6_ACTIVE = 0x800,
            /// <summary>Site Scan Diagnostic 6 Inactive - Index 60</summary>
            SS_DIAG6_INACTIVE = 0x1000,
            /// <summary>Self Read Cleared - Index 61</summary>
            SELF_READ_CLEARED = 0x2000,
            /// <summary>Inversion Tamper - Index 62</summary>
            INVERSION_TAMPER = 0x4000,
            /// <summary>Removal tamper - Index 63</summary>
            REMOVAL_TAMPER = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_64_79
        {
            /// <summary>Register Firmware Download Failed - Index 66</summary>
            REG_DWLD_FAILED = 0x04,
            /// <summary>Register Firmware Download Succeeded - Index 67</summary>
            REG_DWLD_SUCCEEDED = 0x08,
            /// <summary>RFLAN Firmware Download Succeeded - Index 68</summary>
            RFLAN_DWLD_SUCCEEDED = 0x10,
            /// <summary>ZigBee Firmware Download Succeeded - Index 69</summary>
            ZIGBEE_DWLD_SUCCEEDED = 0x20,
            /// <summary>Display Firmware Download Succeeded - Index 72</summary>
            DISP_DWLD_SUCCEEDED = 0x100,
            /// <summary>Display Firmware Download Failed - Index 73</summary>
            DISP_DWLD_FAILED = 0x200,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_80_95
        {
            /// <summary>ZigBee Firmware Download Failed - Inxed 81</summary>
            ZIGBEE_DWLD_FAILED = 0x02,
            /// <summary>RFLAN Firmware Download Failed - Index 82</summary>
            RFLAN_DWLD_FAILED = 0x04,
            /// <summary>SiteScan Error Cleared - Index 84</summary>
            SITESCAN_ERROR_CLEAR = 0x10,
            /// <summary>Load Firmare - Index 85</summary>
            LOAD_FIRMWARE = 0x20,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_96_111
        {
            /// <summary>Reset Counter - Index 101</summary>
            RESET_COUNTERS = 0x20,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_112_127
        {
            /// <summary>Fatal Error - Index 121</summary>
            FATAL_ERROR = 0x200,
            /// <summary>Service Limiting Active Tier Changed - Index 126</summary>
            SERVICE_LIMITING_ACTIVE_TIER_CHANGED = 0x4000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_128_143
        {
            /// <summary>Table Written</summary>
            TABLE_WRITTEN = 0x4,
            /// <summary>Base Mode Error</summary>
            BASE_MODE_ERROR = 0x8,
            /// <summary>Pending Table Activated</summary>
            PENDING_RECONFIGURE = 0x40,
            /// <summary>Event Tamper Cleared - Index 141</summary>
            EVENT_TAMPER_CLEARED = 0x2000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_144_159
        {
            /// <summary>HAN/LAN Log Reset - Index 148</summary>
            LAN_HAN_LOG_RESET = 0x10,
            /// <summary>Display Firmware Download Initiated - Index 151</summary>
            DISP_DWLD_INITIATED = 0x80,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_160_175
        {
            /// <summary>Pending Table Activate Failed</summary>
            PENDING_TABLE_ACTIVATE_FAIL = 0x02,
            /// <summary>HAN Device Status Change</summary>
            HAN_DEVICE_STATUS_CHANGE = 0x04,
            /// <summary>HAN Load Control Event Sent</summary>
            HAN_LOAD_CONTROL_EVENT_SENT = 0x08,
            /// <summary>HAN Load Control Event Status</summary>
            HAN_LOAD_CONTROL_EVENT_STATUS = 0x10,
            /// <summary>HAN Load Control Event Opt Out</summary>
            HAN_LOAD_CONTROL_EVENT_OPT_OUT = 0x20,
            /// <summary>HAN Messaging Event</summary>
            HAN_MESSAGING_EVENT = 0x40,
            /// <summary>HAN Device Added or Removed</summary>
            HAN_DEVICE_ADDED_OR_REMOVED = 0x80,
            /// <summary>Display Firmware Download Initiation Failed - Index 168</summary>
            DISP_DWLD_INITIATION_FAILED = 0x100,
            /// <summary>Register Firmware Download Initiated - Index 169</summary>
            REG_DWLD_INITIATED = 0x200,
            /// <summary>RFLAN Firmware Download Initiated - Index 170</summary>
            RFLAN_DWLD_INITIATED = 0x400,
            /// <summary>ZigBee Firmware Download Initiated - Index 171</summary>
            ZIGBEE_DWLD_INITIATED = 0x800,
            /// <summary>Register Firmware Download Initiation Failed - Index 172</summary>
            REG_DWLD_INITIATION_FAILED = 0x1000,
            /// <summary>RFLAN Firmware Download Initiation Failed - Index 173</summary>
            RFLAN_DWLD_INITIATION_FAILED = 0x2000,
            /// <summary>ZigBee Firmware Download Initiation Failed - Index 174</summary>
            ZIGBEE_DWLD_INITIATION_FAILED = 0x4000,
            /// <summary>Register Firmware Download Status - Index 175</summary>
            REG_FW_DWLD_STATUS = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_176_191
        {
            /// <summary>RFLAN Firmawre Download Status - Index 176</summary>
            RFLAN_FW_DWLD_STATUS = 0x01,
            /// <summary>ZigBee Firmware Download Status - Index 177</summary>
            ZIGBEE_FW_DWLD_STATUS = 0x02,
            /// <summary>Register Firmware Download Alreaday Active - Index 178</summary>
            REG_DWLD_ALREADY_ACTIVE = 0x04,
            /// <summary>RFLAN Firmware Download Already Active - Index 179</summary>
            RFLAN_DWLD_ALREADY_ACTIVE = 0x08,
            /// <summary>Third Party HAN Device Firmware Download Status - Index 181</summary>
            THIRD_PARTY_HAN_FW_DWLD_STATUS = 0x20,
            /// <summary>Firmware Download Abort - Index 184</summary>
            FW_DWLD_ABORT = 0x100,
            /// <summary>Remote Connect Failed - Index 185</summary>
            REMOTE_CONNECT_FAILED = 0x200,
            /// <summary>Remote Disconnected Failed - Index 196</summary>
            REMOTE_DISCONNECT_FAILED = 0x400,
            /// <summary>Remote Disconnect Relay Activated - Index 187</summary>
            REMOTE_DISCONNECT_RELAY_ACTIVATED = 0x800,
            /// <summary>Remote Connect Relay Activated - Index 188</summary>
            REMOTE_CONNECT_RELAY_ACTIVATED = 0x1000,
            /// <summary>Remote Connect Relay Initiated - Index 189/// </summary>
            REMOTE_CONNECT_RELAY_INITIATED = 0x2000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_192_207
        {
            /// <summary>Critical Peak Pricing - Index 192</summary>
            CRITICAL_PEAK_PRICING = 0x01,

        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_208_223
        {
            /// <summary>Billing Schedule Change - Index 213</summary>
            BILLING_SCHEDULE_CHANGE = 0x20,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_224_239
        {
            /// <summary>Network Hush Started</summary>
            NETWORK_HUSH_STARTED = 0x10,
            /// <summary>Load Voltage Preset - Index 230</summary>
            LOAD_VOLT_PRESENT = 0x40,
            /// <summary>Firmware Download Aborted - Index 231</summary>
            PENDING_TABLE_CLEAR_FAIL = 0x80,
            /// <summary>Pending Table Full - Index 232</summary>
            PENDING_TABLE_FULL = 0x100,
            /// <summary>Pending Table Swap - Index 233</summary>
            PENDING_TABLE_SWAP = 0x200,
            /// <summary>Event Scheduling Rejected - Index 234</summary>
            EVENT_SCHEDULING_REJECTED = 0x400,
            /// <summary>C12.22 Registration Attempt - Index 235</summary>
            C12_22_REGISTRATION_ATTEMPT = 0x800,
            /// <summary>C12.22 Registered - Index 236</summary>
            C12_22_REGISTERED = 0x1000,
            /// <summary>C12.22 Deregistration Attempt - Index 237</summary>
            C12_22_DEREGISTRATION_ATTEMPT = 0x2000,
            /// <summary>C12.22 Deregistered - Index 238</summary>
            C12_22_DEREGISTERED = 0x4000,
            /// <summary>C12.22 RFLAN Cell ID Change - Index 239/// </summary>
            C12_22_RFLAN_CELL_ID_CHANGE = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_240_255
        {
            /// <summary>Time Adjustment Failed</summary>
            TIME_ADJUSTMENT_FAILED = 0x01,
            /// <summary>Event Cache Overflow - Index 241</summary>
            EVENT_CACHE_OVERFLOW = 0x02,
            /// <summary>RMS Voltage Bleow Low Threshold</summary>
            RMS_VOLTAGE_BELOW_LOW_THRESHOLD = 0x40,
            /// <summary>Volt(RMS) Above Threshold</summary>
            VOLT_RMS_ABOVE_THRESHOLD = 0x80,
            /// <summary>Volt Hour Below Low Threshold</summary>
            VOLT_HOUR_BELOW_LOW_THRESHOLD = 0x100,
            /// <summary>VOlt Hour Above Threshold</summary>
            VOLT_HOUR_ABOVE_THRESHOLD = 0x200,
            /// <summary>Pending Table Error</summary>
            PENDING_TABLE_ERROR = 0x400,
            /// <summary>Security Event</summary>
            SECURITY_EVENT = 0x1000,
            /// <summary>Key Rollover Pass</summary>
            KEY_ROLLOVER_PASS = 0x2000,
            /// <summary>Sign Key Rpelace Processing Pass</summary>
            SIGN_KEY_REPLACE_PROCESSING_PASS = 0x4000,
            /// <summary>Symmetric Key Replace Processing Pass</summary>
            SYMMETRIC_KEY_REPLACE_PROCESSING_PASS = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event. This range is
        /// for the M2 Gateway only
        /// </summary>
        protected enum Event_256_271
        {
            /// <summary>Gateway Configuration Download</summary>
            GW_CONFIGURATION_DOWNLOAD = 0x01,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event.  This range is
        /// for the M2 Gateway only
        /// </summary>
        protected enum Event_272_287
        {
            /// <summary>placeholder for future events</summary>
            GW_PLACEHOLDER_EVENT = 0X01,
        }

        /// <summary>
        /// The IDs associated with each event
        /// </summary>
        protected enum EventID : int
        {
            /// <summary>Power Outage - Index 1</summary>
            POWER_OUTAGE = 1,
            /// <summary>Power Restored - Index 2</summary>
            POWER_RESTORED = 2,
            /// <summary>Clear Billing Data - Index 3</summary>
            CLEAR_BILLING_DATA = 3,
            /// <summary>Billing Schedule Expirted - Index 4</summary>
            BILLING_SCHED_EXPIRED = 4,
            /// <summary>DST Time Change - Index 5</summary>
            DST_TIME_CHANGE = 5,
            /// <summary>Clock Reset - Index 6</summary>
            CLOCK_RESET = 6,
            /// <summary>Demand Threshold Exceeded - Index 7</summary>
            DEMAND_THRESHOLD_EXCEEDED = 7,
            /// <summary>Demand Threshold Restored - Index 8</summary>
            DEMAND_THRESHOLD_RESTORED = 8,
            /// <summary>Logon Successful - Index 10</summary>
            LOGON_SUCCESSFUL = 10,
            /// <summary>Security Successful - Index 12</summary>
            SECURITY_SUCCESSFUL = 12,
            /// <summary>Security Failed - Index 13</summary>
            SECURITY_FAILED = 13,
            /// <summary>Load Profile Reset - Index 14</summary>
            LOAD_PROFILE_RESET = 14,
            /// <summary>History Log Cleared - Index 16</summary>
            HIST_LOG_CLEARED = 16,
            /// <summary>History Pointers Updated - Index 17</summary>
            HIST_PTRS_UPDATED = 17,
            /// <summary>Event Log Cleared - Index 18</summary>
            EVENT_LOG_CLEARED = 18,
            /// <summary>Event Log Pointers Updated - Index 19</summary>
            EVENT_LOG_PTRS_UPDATED = 19,
            /// <summary>Demand Reset - Index 20</summary>
            DEMAND_RESET = 20,
            /// <summary>Self Read Occurred - Index 21</summary>
            SELF_READ_OCCURRED = 21,
            /// <summary>Input Channel Hi - Index 22</summary>
            INPUT_CHANNEL_HI = 22,
            /// <summary>Input Channel Low - Index 23</summary>
            INPUT_CHANNEL_LO = 23,
            /// <summary>TOU Season Changed - Index 24</summary>
            TOU_SEASON_CHANGED = 24,
            /// <summary>TOU Rate Change - Index 25</summary>
            RATE_CHANGE = 25,
            /// <summary>External Event - Index 26</summary>
            EXTERNAL_EVENT = 26,
            /// <summary>SiteScan for AMI; Custom Schedule Changed - V and I and C12.19 - Index 27</summary>
            SITESCAN_OR_CUSTOM_SCHED_ERROR = 27,
            /// <summary>Pending table Activation - Index 28</summary>
            PENDING_TABLE_ACTIVATION = 28,
            /// <summary>SiteScan for Non-AMI; Pending Table Clear for AMI - Index 29</summary>
            SITESCAN_OR_PENDING_TABLE_CLEAR = 29,
            /// <summary>VQ Log Pointers Updated - Index 30</summary>
            VQ_LOG_PTRS_UPDATED = 30,
            /// <summary>VQ Log Nearly Full - Index 31</summary>
            VQ_LOG_NEARLY_FULL = 31,
            /// <summary>Enter Test Mode - Index 32</summary>
            ENTER_TEST_MODE = 32,
            /// <summary>Exit Test Mode - Index 33</summary>
            EXIT_TEST_MODE = 33,
            /// <summary>ABC Phase Rotation Active - Index 34</summary>
            ABC_PH_ROTATION_ACTIVE = 34,
            /// <summary>CBA Phase Rotation Active - Index 35</summary>
            CBA_PH_ROTATION_ACTIVE = 35,
            /// <summary>Meter Reprogrammed - Index 36</summary>
            METER_REPROGRAMMED = 36,
            /// <summary>Illegal Configuration Error - Index 37</summary>
            ILLEGAL_CONFIG_ERROR = 37,
            /// <summary>CPC Communication Error - Index 38</summary>
            CPC_COMM_ERROR = 38,
            /// <summary>Reverse Rotation Restored - Index 39</summary>
            REVERSE_ROTATION_RESTORE = 39,
            /// <summary>VQ Log Cleared - Index 40</summary>
            VQ_LOG_CLEARED = 40,
            /// <summary>TOU Schedule Error - Index 41</summary>
            TOU_SCHEDULE_ERROR = 41,
            /// <summary>Mass Memory Error - Index 42</summary>
            MASS_MEMORY_ERROR = 42,
            /// <summary>Loss of Phase Restore - Index 43</summary>
            LOSS_OF_PHASE_RESTORE = 43,
            /// <summary>Low Battery - Index 44</summary>
            LOW_BATTERY = 44,
            /// <summary>Loss of Voltage phase A or Loss of Phase for OpenWay- Index 45</summary>
            LOSS_VOLTAGE_A_OR_LOSS_OF_PHASE = 45,
            /// <summary>Register Full Scale - Index 46</summary>
            REGISTER_FULL_SCALE = 46,
            /// <summary>Reverse Power Flow Restore - Index 47</summary>
            REVERSE_POWER_FLOW_RESTORE = 47,
            /// <summary>Reverse Power Flow - Index 48</summary>
            REVERSE_POWER_FLOW = 48,
            /// <summary>Site Scan Diagnostic 1 Active - Index 49</summary>
            SS_DIAG1_ACTIVE = 49,
            /// <summary>Site Scan Diagnostic 2 Active - Index 50</summary>
            SS_DIAG2_ACTIVE = 50,
            /// <summary>Site Scan Diagnostic 3 Active - Index 51</summary>
            SS_DIAG3_ACTIVE = 51,
            /// <summary>Site Scan Diagnostic 4 Active - Index 52</summary>
            SS_DIAG4_ACTIVE = 52,
            /// <summary>Site Scan Diagnostic 5 Active - Index 53</summary>
            SS_DIAG5_ACTIVE = 53,
            /// <summary>Site Scan Diagnostic 1 Inactive - Index 54</summary>
            SS_DIAG1_INACTIVE = 54,
            /// <summary>Site Scan Diagnostic 2 Inactive - Index 55</summary>
            SS_DIAG2_INACTIVE = 55,
            /// <summary>Site Scan Diagnostic 3 Inactive - Index 56</summary>
            SS_DIAG3_INACTIVE = 56,
            /// <summary>Site Scan Diagnostic 4 Inactive - Index 57</summary>
            SS_DIAG4_INACTIVE = 57,
            /// <summary>Site Scan Diagnostic 5 Inactive - Index 58</summary>
            SS_DIAG5_INACTIVE = 58,
            /// <summary>Site Scan Diagnostic 6 Active - Index 59</summary>
            SS_DIAG6_ACTIVE = 59,
            /// <summary>Site Scan Diagnostic 6 Inactive - Index 60</summary>
            SS_DIAG6_INACTIVE = 60,
            /// <summary>Self Read Cleared - Index 61</summary>
            SELF_READ_CLEARED = 61,
            /// <summary>Inversion Tamper - Index 62</summary>
            INVERSION_TAMPER = 62,
            /// <summary>Removal tamper - Index 63</summary>
            REMOVAL_TAMPER = 63,
            /// <summary>Register Firmware Download Failed - Index 66</summary>
            REG_DWLD_FAILED = 66,
            /// <summary>Register Firmware Download Succeeded - Index 67</summary>
            REG_DWLD_SUCCEEDED = 67,
            /// <summary>RFLAN Firmware Download Succeeded - Index 68</summary>
            RFLAN_DWLD_SUCCEEDED = 68,
            /// <summary>ZigBee Firmware Download Succeeded - Index 69</summary>
            ZIGBEE_DWLD_SUCCEEDED = 69,
            /// <summary>Display Firmware Download Succeeded - Index 72</summary>
            DISP_DWLD_SUCCEEDED = 72,
            /// <summary>Display Firmware Download Failed - Index 73</summary>
            DISP_DWLD_FAILED = 73,
            /// <summary>ZigBee Firmware Download Failed - Inxed 81</summary>
            ZIGBEE_DWLD_FAILED = 81,
            /// <summary>RFLAN Firmware Download Failed - Index 82</summary>
            RFLAN_DWLD_FAILED = 82,
            /// <summary>SiteScan Error Cleared - Index 84</summary>
            SITESCAN_ERROR_CLEAR = 84,
            /// <summary>Load Firmare - Index 85</summary>
            LOAD_FIRMWARE = 85,
            /// <summary>Reset Counter - Index 101</summary>
            RESET_COUNTERS = 101,
            /// <summary>Fatal Error - Index 121</summary>
            FATAL_ERROR = 121,
            /// <summary>Service Limiting Active Tier Changed - Index 126</summary>
            SERVICE_LIMITING_ACTIVE_TIER_CHANGED = 126,
            /// <summary>Table Written</summary>
            TABLE_WRITTEN = 130,
            /// <summary>Base Mode Error</summary>
            BASE_MODE_ERROR = 131,
            /// <summary>Pending Table Activated</summary>
            PENDING_RECONFIGURE = 134,
            /// <summary>Event Tamper Cleared - Index 141</summary>
            EVENT_TAMPER_CLEARED = 141,
            /// <summary>HAN/LAN Log Reset - Index 148</summary>
            LAN_HAN_LOG_RESET = 148,
            /// <summary>Display Firmware Download Initiated - Index 151</summary>
            DISP_DWLD_INITIATED = 151,
            /// <summary>Pending Table Activate Failed</summary>
            PENDING_TABLE_ACTIVATE_FAILED = 161,
            /// <summary>HAN Device Status Change</summary>
            HAN_DEVICE_STATUS_CHANGE = 162,
            /// <summary>HAN Load Control Event Sent</summary>
            HAN_LOAD_CONTROL_EVENT_SENT = 163,
            /// <summary>HAN Load Control Event Status</summary>
            HAN_LOAD_CONTROL_EVENT_STATUS = 164,
            /// <summary>HAN Load Control Event Opt Out</summary>
            HAN_LOAD_CONTROL_EVENT_OPT_OUT = 165,
            /// <summary>HAN Messaging Event</summary>
            HAN_MESSAGING_EVENT = 166,
            /// <summary>HAN Device Added or Removed</summary>
            HAN_DEVICE_ADDED_OR_REMOVED = 167,
            /// <summary>Display Firmware Download Initiation Failed - Index 168</summary>
            DISP_DWLD_INITIATION_FAILED = 168,
            /// <summary>Register Firmware Download Initiated - Index 169</summary>
            REG_DWLD_INITIATED = 169,
            /// <summary>RFLAN Firmware Download Initiated - Index 170</summary>
            RFLAN_DWLD_INITIATED = 170,
            /// <summary>ZigBee Firmware Download Initiated - Index 171</summary>
            ZIGBEE_DWLD_INITIATED = 171,
            /// <summary>Register Firmware Download Initiation Failed - Index 172</summary>
            REG_DWLD_INITIATION_FAILED = 172,
            /// <summary>RFLAN Firmware Download Initiation Failed - Index 173</summary>
            RFLAN_DWLD_INITIATION_FAILED = 173,
            /// <summary>ZigBee Firmware Download Initiation Failed - Index 174</summary>
            ZIGBEE_DWLD_INITIATION_FAILED = 174,
            /// <summary>Register Firmware Download Status - Index 175</summary>
            REG_FW_DWLD_STATUS = 175,
            /// <summary>RFLAN Firmawre Download Status - Index 176</summary>
            RFLAN_FW_DWLD_STATUS = 176,
            /// <summary>ZigBee Firmware Download Status - Index 177</summary>
            ZIGBEE_FW_DWLD_STATUS = 177,
            /// <summary>Register Firmware Download Alreaday Active - Index 178</summary>
            REG_DWLD_ALREADY_ACTIVE = 178,
            /// <summary>RFLAN Firmware Download Already Active - Index 179</summary>
            RFLAN_DWLD_ALREADY_ACTIVE = 179,
            /// <summary>Third Party HAN Device Firmware Download Status - Index 181</summary>
            THIRD_PARTY_HAN_FW_DWLD_STATUS = 181,
            /// <summary>Firmware Download Abort - Index 184</summary>
            FW_DWLD_ABORT = 184,
            /// <summary>Remote Connect Failed - Index 185</summary>
            REMOTE_CONNECT_FAILED = 185,
            /// <summary>Remote Disconnected Failed - Index 186</summary>
            REMOTE_DISCONNECT_FAILED = 186,
            /// <summary>Remote Disconnect Relay Activated - Index 187</summary>
            REMOTE_DISCONNECT_RELAY_ACTIVATED = 187,
            /// <summary>Remote Connect Relay Activated - Index 188</summary>
            REMOTE_CONNECT_RELAY_ACTIVATED = 188,
            /// <summary>Remote Connect Relay Initiated - Index 189/// </summary>
            REMOTE_CONNECT_RELAY_INITIATED = 189,
            /// <summary>Critical Peak Pricing - Index 192/// </summary>
            CRITICAL_PEAK_PRICING = 192,
            /// <summary>Billing Schedule Change - Index 213</summary>
            BILLING_SCHEDULE_CHANGE = 213,
            /// <summary>Network Hush Started</summary>
            NETWORK_HUSH_STARTED = 228,
            /// <summary>Load Voltage Preset - Index 230</summary>
            LOAD_VOLT_PRESENT = 230,
            /// <summary>Firmware Download Aborted - Index 231</summary>
            PENDING_TABLE_CLEAR_FAILED = 231,
            /// <summary>Pending Table Full - Index 232</summary>
            PENDING_TABLE_FULL = 232,
            /// <summary>Pending Table Swap - Index 233</summary>
            PENDING_TABLE_SWAP = 233,
            /// <summary>Event Scheduling Rejected - Index 234</summary>
            EVENT_SCHEDULING_REJECTED = 234,
            /// <summary>C12.22 Registration Attempt - Index 235</summary>
            C12_22_REGISTRATION_ATTEMPT = 235,
            /// <summary>C12.22 Registered - Index 236</summary>
            C12_22_REGISTERED = 236,
            /// <summary>C12.22 Deregistration Attempt - Index 237</summary>
            C12_22_DEREGISTRATION_ATTEMPT = 237,
            /// <summary>C12.22 Deregistered - Index 238</summary>
            C12_22_DEREGISTERED = 238,
            /// <summary>C12.22 RFLAN Cell ID Change - Index 239/// </summary>
            C12_22_RFLAN_CELL_ID_CHANGE = 239,
            /// <summary>Time Adjustment Failed</summary>
            TIME_ADJUSTMENT_FAILED = 240,
            /// <summary>Event Cache Overflow - Index 241</summary>
            EVENT_CACHE_OVERFLOW = 241,
            /// <summary>RMS Voltage Bleow Low Threshold</summary>
            RMS_VOLTAGE_BELOW_LOW_THRESHOLD = 246,
            /// <summary>Volt(RMS) Above Threshold</summary>
            VOLT_RMS_ABOVE_THRESHOLD = 247,
            /// <summary>Volt Hour Below Low Threshold</summary>
            VOLT_HOUR_BELOW_LOW_THRESHOLD = 248,
            /// <summary>VOlt Hour Above Threshold</summary>
            VOLT_HOUR_ABOVE_THRESHOLD = 249,
            /// <summary>Pending Table Error</summary>
            PENDING_TABLE_ERROR = 250,
            /// <summary>Security Event</summary>
            SECURITY_EVENT = 252,
            /// <summary>Key Rollover Pass</summary>
            KEY_ROLLOVER_PASS = 253,
            /// <summary>Sign Key Rpelace Processing Pass</summary>
            SIGN_KEY_REPLACE_PROCESSING_PASS = 254,
            /// <summary>Symmetric Key Replace Processing Pass</summary>
            SYMMETRIC_KEY_REPLACE_PROCESSING_PASS = 255,
            /// <summary>M2 Gateway only event - configuration download</summary>
            GW_CONFIGURATION_DOWNLOAD = 256,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for M2 Gateway History Log Config class
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The offset of this config block</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/10 AF  2.45.07 161866 Created 
        //  04/21/11 AF  2.50.31 167587 Changes due to change in base class
        //
        public M2_Gateway_HistoryLogConfig(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, EVENT_CONFIG_SIZE)
        {
            m_lstEventConfiguration = new List<MFG2048EventItem>();
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                               this.GetType().Assembly);
        }

        /// <summary>
        /// Constructor used to get Event Data from the EDL file
        /// </summary>
        /// <param name="EDLBinaryReader"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/10 AF  2.45.07 161866 Created
        //  04/21/11 AF  2.50.31 167587 Changes due to change in base class
        //
        public M2_Gateway_HistoryLogConfig(PSEMBinaryReader EDLBinaryReader)
            : base(2048, EVENT_CONFIG_SIZE)
        {
            m_lstEventConfiguration = new List<MFG2048EventItem>();
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                               this.GetType().Assembly);
            m_Reader = EDLBinaryReader;
            m_TableState = TableState.Loaded;
            ParseData();
        }

        /// <summary>
        /// Reads the HistoryLogConfig component and populates its fields
        /// </summary>
        /// <returns>PSEMResponse</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/19/11 AF  2.50.30  170677 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "HistoryLogConfig.Read");
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
        /// History Configuration - Only lists items that are in the Configuration 
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version   Issue#    Description
        //  -------- --- -------   ------    ---------------------------------------------
        //  10/25/10 AF  2.45.07   161866    Created
        //  11/04/10 AF  2.45.10             Removed unsupported events
        //  04/21/11 AF  2.50.31   167587    Commented out events that aren't visible in the CE
        //  05/03/11 AF  2.50.40             Added back commented out events
        //  05/19/16 AF  4.50.270  685741    Corrected the resource string for PENDING_TABLE_ERROR
        //  05/20/16 AF  4.50.270  687675    Changed the resource string for HAN_LOAD_CONTROL_EVENT_SENT
        //  08/05/16 MP  4.70.11   WR674048  Changed how we accessed string names, no more hardcoded strings.
        //                                      also re-organized events so they are in order
        public List<MFG2048EventItem> HistoryConfiguration
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading History Log Config"));
                    }
                }

                //clear out the list
                m_lstEventConfiguration.Clear();

                AddEventConfigItem(Enum.GetName(typeof(Event_0_15), Event_0_15.LOGON_SUCCESSFUL), m_usEvent0_15, (UInt16)(Event_0_15.LOGON_SUCCESSFUL));
                AddEventConfigItem(Enum.GetName(typeof(Event_0_15), Event_0_15.SECURITY_FAILED), m_usEvent0_15, (UInt16)(Event_0_15.SECURITY_FAILED));
                AddEventConfigItem(Enum.GetName(typeof(Event_0_15), Event_0_15.SECURITY_SUCCESSFUL), m_usEvent0_15, (UInt16)(Event_0_15.SECURITY_SUCCESSFUL));
                AddEventConfigItem(Enum.GetName(typeof(Event_0_15), Event_0_15.PRIMARY_POWER_DOWN), m_usEvent0_15, (UInt16)(Event_0_15.PRIMARY_POWER_DOWN));
                AddEventConfigItem(Enum.GetName(typeof(Event_0_15), Event_0_15.PRIMARY_POWER_UP), m_usEvent0_15, (UInt16)(Event_0_15.PRIMARY_POWER_UP));
                AddEventConfigItem(Enum.GetName(typeof(Event_0_15), Event_0_15.CLOCK_RESET), m_usEvent0_15, (UInt16)(Event_0_15.CLOCK_RESET));

                AddEventConfigItem(Enum.GetName(typeof(Event_16_31), Event_16_31.HIST_LOG_CLEARED), m_usEvent16_31, (UInt16)(Event_16_31.HIST_LOG_CLEARED));

                AddEventConfigItem(Enum.GetName(typeof(Event_32_47), Event_32_47.ILLEGAL_CONFIG_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.ILLEGAL_CONFIG_ERROR));

                AddEventConfigItem(Enum.GetName(typeof(Event_112_127), Event_112_127.FATAL_ERROR), m_usEvent112_127, (UInt16)(Event_112_127.FATAL_ERROR));

                AddEventConfigItem(Enum.GetName(typeof(Event_144_159), Event_144_159.LAN_HAN_LOG_RESET), m_usEvent144_159, (UInt16)(Event_144_159.LAN_HAN_LOG_RESET));

                AddEventConfigItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_LOAD_CONTROL_EVENT_OPT_OUT), m_usEvent160_175, (UInt16)(Event_160_175.HAN_LOAD_CONTROL_EVENT_OPT_OUT));
                AddEventConfigItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_LOAD_CONTROL_EVENT_SENT), m_usEvent160_175, (UInt16)(Event_160_175.HAN_LOAD_CONTROL_EVENT_SENT));
                AddEventConfigItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_LOAD_CONTROL_EVENT_STATUS), m_usEvent160_175, (UInt16)(Event_160_175.HAN_LOAD_CONTROL_EVENT_STATUS));
                AddEventConfigItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_DEVICE_STATUS_CHANGE), m_usEvent160_175, (UInt16)(Event_160_175.HAN_DEVICE_STATUS_CHANGE));
                AddEventConfigItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_MESSAGING_EVENT), m_usEvent160_175, (UInt16)(Event_160_175.HAN_MESSAGING_EVENT));
                AddEventConfigItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_DEVICE_ADDED_OR_REMOVED), m_usEvent160_175, (UInt16)(Event_160_175.HAN_DEVICE_ADDED_OR_REMOVED));
                AddEventConfigItem(Enum.GetName(typeof(Event_160_175), Event_160_175.PENDING_TABLE_ACTIVATE_FAIL), m_usEvent160_175, (UInt16)(Event_160_175.PENDING_TABLE_ACTIVATE_FAIL));

                AddEventConfigItem(Enum.GetName(typeof(Event_224_239), Event_224_239.C12_22_DEREGISTRATION_ATTEMPT), m_usEvent224_239, (UInt16)(Event_224_239.C12_22_DEREGISTRATION_ATTEMPT));
                AddEventConfigItem(Enum.GetName(typeof(Event_224_239), Event_224_239.C12_22_DEREGISTERED), m_usEvent224_239, (UInt16)(Event_224_239.C12_22_DEREGISTERED));
                AddEventConfigItem(Enum.GetName(typeof(Event_224_239), Event_224_239.PENDING_TABLE_CLEAR_FAIL), m_usEvent224_239, (UInt16)(Event_224_239.PENDING_TABLE_CLEAR_FAIL));
                AddEventConfigItem(Enum.GetName(typeof(Event_224_239), Event_224_239.NETWORK_HUSH_STARTED), m_usEvent224_239, (UInt16)(Event_224_239.NETWORK_HUSH_STARTED));

                AddEventConfigItem(Enum.GetName(typeof(Event_240_255), Event_240_255.PENDING_TABLE_ERROR), m_usEvent240_255, (UInt16)(Event_240_255.PENDING_TABLE_ERROR));
                AddEventConfigItem(Enum.GetName(typeof(Event_240_255), Event_240_255.EVENT_CACHE_OVERFLOW), m_usEvent240_255, (UInt16)(Event_240_255.EVENT_CACHE_OVERFLOW));
                AddEventConfigItem(Enum.GetName(typeof(Event_240_255), Event_240_255.SECURITY_EVENT), m_usEvent240_255, (UInt16)(Event_240_255.SECURITY_EVENT));
                AddEventConfigItem(Enum.GetName(typeof(Event_240_255), Event_240_255.TIME_ADJUSTMENT_FAILED), m_usEvent240_255, (UInt16)(Event_240_255.TIME_ADJUSTMENT_FAILED));
                AddEventConfigItem(Enum.GetName(typeof(Event_240_255), Event_240_255.KEY_ROLLOVER_PASS), m_usEvent240_255, (UInt16)(Event_240_255.KEY_ROLLOVER_PASS));
                AddEventConfigItem(Enum.GetName(typeof(Event_240_255), Event_240_255.SIGN_KEY_REPLACE_PROCESSING_PASS), m_usEvent240_255, (UInt16)(Event_240_255.SIGN_KEY_REPLACE_PROCESSING_PASS));
                AddEventConfigItem(Enum.GetName(typeof(Event_240_255), Event_240_255.SYMMETRIC_KEY_REPLACE_PROCESSING_PASS), m_usEvent240_255, (UInt16)(Event_240_255.SYMMETRIC_KEY_REPLACE_PROCESSING_PASS));

                AddEventConfigItem(Enum.GetName(typeof(Event_256_271), Event_256_271.GW_CONFIGURATION_DOWNLOAD), m_usEvent256_271, (UInt16)(Event_256_271.GW_CONFIGURATION_DOWNLOAD));

                return m_lstEventConfiguration;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Adds an event item to the Configuration list.
        /// </summary>
        /// <param name="strResourceString">The description of the event</param>
        /// <param name="usEventField">The raw data from the meter</param>
        /// <param name="usEventMask">The mask to apply to determine whether or not
        /// the event is enabled</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/10 AF  2.45.07 161866 Created
        //
        protected void AddEventConfigItem(string strResourceString, UInt16 usEventField, UInt16 usEventMask)
        {
            MFG2048EventItem eventItem = GetEventItem(strResourceString, usEventField, usEventMask);
            m_lstEventConfiguration.Add(eventItem);
        }

        /// <summary>
        /// Gets the Event Item
        /// </summary>
        /// <param name="strResourceString"> The Description of the Event</param>
        /// <param name="usEventField">The raw data from the device</param>
        /// <param name="usEventMask">The mast to apply to determ if the event is enabled</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/21/11 AF  2.50.31 167587 Created
        //
        protected MFG2048EventItem GetEventItem(string strResourceString, UInt16 usEventField, UInt16 usEventMask)
        {
            MFG2048EventItem eventItem = new MFG2048EventItem();
            eventItem.Description = m_rmStrings.GetString(strResourceString);
            eventItem.Enabled = false;
            if (0 != (usEventField & usEventMask))
            {
                eventItem.Enabled = true;
            }

            return eventItem;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the data out of the PSEMBinaryRead and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/21/11 AF  2.50.31 167587 Created
        //
        private void ParseData()
        {
            m_usEvent0_15 = m_Reader.ReadUInt16();
            m_usEvent16_31 = m_Reader.ReadUInt16();
            m_usEvent32_47 = m_Reader.ReadUInt16();
            m_usEvent48_63 = m_Reader.ReadUInt16();
            m_usEvent64_79 = m_Reader.ReadUInt16();
            m_usEvent80_95 = m_Reader.ReadUInt16();
            m_usEvent96_111 = m_Reader.ReadUInt16();
            m_usEvent112_127 = m_Reader.ReadUInt16();
            m_usEvent128_143 = m_Reader.ReadUInt16();
            m_usEvent144_159 = m_Reader.ReadUInt16();
            m_usEvent160_175 = m_Reader.ReadUInt16();
            m_usEvent176_191 = m_Reader.ReadUInt16();
            m_usEvent192_207 = m_Reader.ReadUInt16();
            m_usEvent208_223 = m_Reader.ReadUInt16();
            m_usEvent224_239 = m_Reader.ReadUInt16();
            m_usEvent240_255 = m_Reader.ReadUInt16();
            m_usEvent256_271 = m_Reader.ReadUInt16();
            m_usEvent272_279 = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        /// <summary>Events 0-15</summary>
        protected UInt16 m_usEvent0_15;
        /// <summary>Events 16-31</summary>
        protected UInt16 m_usEvent16_31;
        /// <summary>Events 32-47</summary>
        protected UInt16 m_usEvent32_47;
        /// <summary>Events 48-63</summary>
        protected UInt16 m_usEvent48_63;
        /// <summary>Events 64-79</summary>
        protected UInt16 m_usEvent64_79;
        /// <summary>Events 80-95</summary>
        protected UInt16 m_usEvent80_95;
        /// <summary>Events 96-111</summary>
        protected UInt16 m_usEvent96_111;
        /// <summary>Events 112-127</summary>
        protected UInt16 m_usEvent112_127;
        /// <summary>Events 128-143</summary>
        protected UInt16 m_usEvent128_143;
        /// <summary>Events 144-159</summary>
        protected UInt16 m_usEvent144_159;
        /// <summary>Events 160-175</summary>
        protected UInt16 m_usEvent160_175;
        /// <summary>Events 176-191</summary>
        protected UInt16 m_usEvent176_191;
        /// <summary>Events 192-207</summary>
        protected UInt16 m_usEvent192_207;
        /// <summary>Events 208-223</summary>
        protected UInt16 m_usEvent208_223;
        /// <summary>Events 224- 239</summary>
        protected UInt16 m_usEvent224_239;
        /// <summary>Events 240-255</summary>
        protected UInt16 m_usEvent240_255;
        /// <summary>Events 256-271</summary>
        protected UInt16 m_usEvent256_271;
        /// <summary>Events 272-279</summary>
        protected UInt16 m_usEvent272_279;
        
        
        ///// <summary>The Event List</summary>
        //protected List<MFG2048EventItem> m_lstEvents;
        /// <summary>The Resource Manager</summary>
        protected System.Resources.ResourceManager m_rmStrings;
        /// <summary>The Resource Project strings</summary>
        protected static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                            "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";
        /// <summary>Event Configuration list</summary>
        protected List<MFG2048EventItem> m_lstEventConfiguration;

        #endregion
    }
}
