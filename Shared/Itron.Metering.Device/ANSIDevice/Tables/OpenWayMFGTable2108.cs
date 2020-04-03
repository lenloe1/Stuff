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
//                           Copyright © 2010 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Utilities;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The CHANMfgTable2108 class handles the reading of MFG Table 2108 (MCU status).
    /// From register f/w build 115 and later, this table also contains the 
    /// register version, revision, and build and the display version and revision
    /// as well as the RFLAN and Zigbee information.
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 08/11/06 AF  7.35.00  N/A	Created
    // 09/14/06 AF  7.35.00  N/A	Added member variables for build
    // 11/27/06 AF  8.00.00         Changed the class name to match other
    //                              HAN Mfg tables and moved the code from
    //                              MfgTables
    // 03/25/10 RCG 2.40.28         Changed name and moved to on file since this is not HAN related

    public class OpenWayMfgTable2108 : AnsiTable
    {
        #region Constants

        private const int MCU_TABLE_LENGTH_2108 = 13;
        private const int MCU_TABLE_LENGTH_2108_EXTENDED = 14;
        private const int MCU_TABLE_LENGTH_HAN_APP = 17;
        private const int MCU_TABLE_LENGTH_LOADER_VERSION = 20;

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Constructor, initializes the table
        /// </summary>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CHANMfgTable2108 Table2108 = new CHANMfgTable2108(m_PSEM, FirmwareBuild); 
        /// </code>
        /// </example>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/11/06 AF  7.35.00 N/A    Created 
        // 09/14/06 AF  7.35.00 N/A	   Added member variables for build
        // 11/02/06 AF  7.40.00        Changed call to base constructor to pass
        //                             a length dependent on the build of f/w
        // 02/22/07 AF  8.00.12 2417   Removed dependency on f/w build
        // 03/15/10 AF  2.40.25        Removed the f/w build param.  It's no longer
        //                             needed and caused a LID request
        // 03/15/16 AF  4.50.236 662058 Allow table resizing since length is dependent on fw version
        // 03/16/16 AF  4.50.236 606791 Make the table a full table instead of a subtable
        //
        public OpenWayMfgTable2108(CPSEM psem)
            : base(psem, 2108, MCU_TABLE_LENGTH_2108_EXTENDED)
        {
            InitializeVariables();
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Full read of table 2108 (MCU Status) out of the
        /// meter.
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/11/06 AF  7.35.00 N/A    Created
        // 09/14/06 AF  7.35.00 N/A	   Added member variables for build
        // 11/02/06 AF  7.40.00        Added code that reads the register and
        //                             display versions out of 2108.  These fields
        //                             are available only in builds > 114
        // 02/22/07 AF  8.00.13 2417   Removed the check on builds.  All meters
        //                             should now be running build 115 or later
        // 02/24/11 AF  2.50.05        Removed the read of display build - we read
        //                             only 13 bytes and that is in byte 14
        // 03/16/16 AF  4.50.236 662058 Added read of HAN App and FW Loader ver/rev/build if supported. Also added back
        //                              the display build.
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMfgTable2108.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                m_CommType = m_Reader.ReadByte();
                m_CommFWVer = m_Reader.ReadByte();
                m_CommFWRev = m_Reader.ReadByte();
                m_CommFWBuild = m_Reader.ReadByte();
                m_HanType = m_Reader.ReadByte();
                m_HanFWVer = m_Reader.ReadByte();
                m_HanFWRev = m_Reader.ReadByte();
                m_HanFWBuild = m_Reader.ReadByte();
                m_RegFWVer = m_Reader.ReadByte();
                m_RegFWRev = m_Reader.ReadByte();
                m_RegFWBuild = m_Reader.ReadByte();
                m_DisplayVer = m_Reader.ReadByte();
                m_DisplayRev = m_Reader.ReadByte();
                m_DisplayBuild = m_Reader.ReadByte();

                if (m_DataStream.Length >= MCU_TABLE_LENGTH_HAN_APP)
                {
                    m_HANAppFWVer = m_Reader.ReadByte();
                    m_HANAppFWRev = m_Reader.ReadByte();
                    m_HANAppFWBuild = m_Reader.ReadByte();
                }
                if (m_DataStream.Length >= MCU_TABLE_LENGTH_LOADER_VERSION)
                {
                    m_FWLoaderVer = m_Reader.ReadByte();
                    m_FWLoaderRev = m_Reader.ReadByte();
                    m_FWLoaderBuild = m_Reader.ReadByte();
                }

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Static Methods (Translation Methods)

        /// <summary>
        /// Tranlate the Comm Module Type to a string
        /// </summary>
        /// <param name="byCommModType">The Byte value from the device</param>
        /// <returns>string - Comm Module Type</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/10 AF  2.40.28 151807 Corrected the translation of Comm Module type
        //  04/08/14 AF  3.50.67 WR488011 Added the ICM comm module type
        //
        public static string TranslateCommModuleType(byte byCommModType)
        {
            string strCommModType = "Unknown";

            if (0 == byCommModType)
            {
                strCommModType = "RFLAN";
            }
            else if (1 == byCommModType)
            {
                strCommModType = "IP";
            }
            else if (3 == byCommModType)
            {
                strCommModType = "ICS";
            }
            else
            {
                strCommModType = "Unknown";
            }

            return strCommModType;
        }

        /// <summary>
        /// Translate the Han Module Type
        /// </summary>
        /// <param name="byHanModType">byte value from the device</param>
        /// <returns>string - HAN Module Type</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/10 AF  2.40.28 151807 The HAN module type for ZigBee is 0
        //
        public static string TranslationHanModType(byte byHanModType)
        {
            string strHanModType = "Unknown";

            if (0 == byHanModType)
            {
                strHanModType = "ZigBee";
            }

            else
            {
                strHanModType = "Unknown";
            }

            return strHanModType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the type - RFLAN or IP
        /// </summary>
        ///  <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 08/11/06 AF  7.35.00 N/A	    Created
        // 11/10/06 AF  8.00.00         Changed exception error message to better point to probable
        //                              cause of read failure
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        // 
        public string CommModuleType
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read the RFLAN/Zigbee Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Table 2108.  Table length may be unexpected."));
                    }
                }

                return TranslateCommModuleType(m_CommType);
            }
        }

        /// <summary>
        /// Gets Comm Module type as a byte
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte CommModuleTypeByte
        {
            get
            {
                ReadUnloadedTable();

                return m_CommType;
            }
        }

        /// <summary>
        /// Gets the Zigbee firmware version from the status 
        /// table
        /// </summary>
        ///  <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 08/11/06 AF  7.35.00 N/A		Created
        // 09/14/06 AF  7.35.00 N/A		Added build number version info
        // 10/31/06 AF  7.40.00         Changed the order of the version info to match
        //                              version read from f/w download file
        // 11/10/06 AF  8.00.00         Changed exception error message to better point to probable
        //                              cause of read failure
        // 11/13/06 AF  8.00.00         Changed the order of version info back to orginal now that
        //                              f/w has been corrected.
        // 11/15/06 AF  8.00.00         Added f/w build value to error message  
        // 11/16/06 AF  8.00.00 10      Tracker #10, Field-Pro for Open Way, corrected the formatting
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //  03/15/10 AF 2.40.25         Removed the f/w build from error message. It caused a LID request.
        //
        public string CommModuleVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    //Read the RFLAN/Zigbee Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2108.";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_CommFWVer.ToString(CultureInfo.CurrentCulture) + "." + m_CommFWRev.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the Zigbee firmware build number from the status 
        /// table
        /// </summary>
        ///  <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/15/07 RCG 8.10.05 N/A		Created
        // 04/27/09 AF   2.20.02        Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //  03/15/10 AF 2.40.25         Removed the f/w build from error message. It caused a LID request.

        public string CommModuleBuild
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    //Read the RFLAN/Zigbee Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2108.";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_CommFWBuild.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the Han type from the status table
        /// </summary>
        ///  <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 08/11/06 AF  7.35.00 N/A	Created
        // 11/10/06 AF  8.00.00         Changed exception error message to better point to probable
        //                              cause of read failure
        // 04/27/09 AF   2.20.02        Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public string HanModuleType
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read the RFLAN/Zigbee Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Table 2108.  Table length may be unexpected."));
                    }
                }

                return TranslationHanModType(m_HanType);
            }
        }

        /// <summary>
        /// Gets HAN Module type as a byte
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte HANModuleTypeByte
        {
            get
            {
                ReadUnloadedTable();

                return m_HanType;
            }
        }

        /// <summary>
        /// Gets the f/w version of the Zigbee micro
        /// </summary>
        ///  <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 08/11/06 AF  7.35.00 N/A		Created
        // 09/14/06 AF  7.35.00 N/A		Added build number version info
        // 11/10/06 AF  8.00.00         Changed exception error message to better point to probable
        //                              cause of read failure
        // 11/16/06 AF  8.00.00 10      Tracker #10, Field-Pro for Open Way, corrected the formatting
        // 04/27/09 AF   2.20.02        Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //  03/15/10 AF 2.40.25         Removed the f/w build from error message. It caused a LID request.
        //
        public string HanModuleVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    //Read the RFLAN/Zigbee Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2108.";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_HanFWVer.ToString(CultureInfo.InvariantCulture) + "." + m_HanFWRev.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the f/w version of the Zigbee micro
        /// </summary>
        ///  <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/15/07 RCG 8.10.05 N/A		Created
        // 04/27/09 AF   2.20.02        Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //  03/15/10 AF 2.40.25         Removed the f/w build from error message. It caused a LID request.

        public string HanModuleBuild
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    //Read the RFLAN/Zigbee Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2108.";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_HanFWBuild.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the f/w version of the Register micro
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/02/06 AF  7.40.00 N/A		Created
        // 11/10/06 AF  8.00.00         Changed exception error message to better point to probable
        //                              cause of read failure
        // 04/27/09 AF   2.20.02        Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //  03/15/10 AF 2.40.25         Removed the f/w build from error message. It caused a LID request.
        //
        public string RegModuleVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    //Read the RFLAN/Zigbee Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2108.";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_RegFWVer.ToString(CultureInfo.InvariantCulture) + "." + m_RegFWRev.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the f/w version build number of the Register micro
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/15/07 RCG 8.10.05 N/A		Created
        //  03/15/10 AF 2.40.25         Removed the f/w build from error message. It caused a LID request.

        public string RegModuleBuild
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";
                if (TableState.Unloaded == m_TableState)
                {
                    //Read the RFLAN/Zigbee Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2108.";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_RegFWBuild.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the display version and revision from table 2108
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/02/06 AF  7.40.00 N/A		Created
        // 11/10/06 AF  8.00.00         Changed exception error message to better point to probable
        //                              cause of read failure
        // 11/03/08 AF  2.00.03         Added "d3" formatting param to get the proper value of the revision
        // 04/27/09 AF   2.20.02        Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //  03/15/10 AF 2.40.25         Removed the f/w build from error message. It caused a LID request.
        //
        public string DisplayModuleVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    //Read the MCU Status (2108) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2108.";

                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_DisplayVer.ToString(CultureInfo.CurrentCulture) + "." + m_DisplayRev.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Property to retrieve the Display Firmware build number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/10 AF 2.40.25         Removed the f/w build from error message. It caused a LID request.
        //
        public string DisplayModuleBuild
        {
            get
            {
                // Until we know otherwise, we assume the Build is not available.
                string strDisplayModuleBuild = "";
                PSEMResponse Result = PSEMResponse.Ok;

                try
                {
                    if (TableState.Loaded != m_TableStateExtended)
                    {
                        Result = base.Read(MCU_TABLE_LENGTH_2108, 1);
                        if (PSEMResponse.Ok != Result)
                        {
                            string strMsg = "Error reading Table 2108.";
                            throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                     Result, strMsg));
                        }
                        else
                        {
                            // Get the Display build out of the Reader
                            m_DisplayBuild = m_Reader.ReadByte();
                            m_TableStateExtended = TableState.Loaded;
                        }
                    }

                    strDisplayModuleBuild = m_DisplayBuild.ToString("d3", CultureInfo.CurrentCulture);
                }
                catch (PSEMException)
                {
                    strDisplayModuleBuild = "";
                }

                return strDisplayModuleBuild;
            }
        }

        /// <summary>
        /// Gets the Version of the register firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte RegVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_RegFWVer;
            }
        }

        /// <summary>
        /// Gets the Revision of the register firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte RegRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_RegFWRev;
            }
        }

        /// <summary>
        /// Gets the Build of the register firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte RegBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_RegFWBuild;
            }
        }

        /// <summary>
        /// Gets the Version of the Comm Module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

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
        //  01/12/10 RCG 2.40.04 N/A    Created

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
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte CommBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_CommFWBuild;
            }
        }

        /// <summary>
        /// Gets the Version of the HAN firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

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
        //  01/12/10 RCG 2.40.04 N/A    Created

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
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte HANBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HanFWBuild;
            }
        }

        /// <summary>
        /// Gets the Version of the display firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte DisplayVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_DisplayVer;
            }
        }

        /// <summary>
        /// Gets the Revision of the display firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte DisplayRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_DisplayRev;
            }
        }

        /// <summary>
        /// Gets the Build of the display firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created
        //  02/24/11 AF  2.50.05        Replaced call to ReadUnloadedTable since it
        //                              checks only the m_TableState and not the extended variable
        //
        public byte DisplayBuildOnly
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                try
                {
                    if (TableState.Loaded != m_TableStateExtended)
                    {
                        Result = base.Read(MCU_TABLE_LENGTH_2108, 1);
                        if (PSEMResponse.Ok != Result)
                        {
                            string strMsg = "Error reading Table 2108.";
                            throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                     Result, strMsg));
                        }
                        else
                        {
                            // Get the Display build out of the Reader
                            m_DisplayBuild = m_Reader.ReadByte();
                            m_TableStateExtended = TableState.Loaded;
                        }
                    }
                }
                catch (PSEMException)
                {
                    m_DisplayBuild = 0;
                }

                return m_DisplayBuild;
            }
        }

        /// <summary>
        /// Gets the version of the HAN App
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/15/16 AF  4.50.236 WR 606791  Created
        //
        public byte? HANAppVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HANAppFWVer;
            }
        }

        /// <summary>
        /// Gets the revision of the HAN App 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/15/16 AF  4.50.236 WR 606791  Created
        //
        public byte? HANAppRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HANAppFWRev;
            }
        }

        /// <summary>
        /// Gets the build of the HAN App
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/15/16 AF  4.50.236 WR 606791  Created
        //
        public byte? HANAppBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HANAppFWBuild;
            }
        }

        /// <summary>
        /// Gets the firmware loader version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/15/16 AF  4.50.236 WR 606791  Created
        //
        public byte? FWLoaderVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_FWLoaderVer;
            }
        }

        /// <summary>
        /// Gets the firmware loader revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/15/16 AF  4.50.236 WR 606791  Created
        //
        public byte? FWLoaderRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_FWLoaderRev;
            }
        }

        /// <summary>
        /// Gets the firmware loader build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/15/16 AF  4.50.236 WR 606791  Created
        //
        public byte? FWLoaderBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_FWLoaderBuild;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize Variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/09 AF  2.40.15        Removed the f/w build parameter - it was 
        //                              based on a LID request & no longer needed
        //  03/16/16 AF  4.50.236 662058 Added HAN App and FW loader version variables
        //
        private void InitializeVariables()
        {
            m_CommType = 0;
            m_CommFWVer = 0;
            m_CommFWRev = 0;
            m_CommFWBuild = 0;
            m_HanType = 0;
            m_HanFWVer = 0;
            m_HanFWRev = 0;
            m_HanFWBuild = 0;
            m_RegFWVer = 0;
            m_RegFWRev = 0;
            m_RegFWBuild = 0;
            m_DisplayVer = 0;
            m_DisplayRev = 0;
            m_DisplayBuild = 0;
            m_HANAppFWVer = null;
            m_HANAppFWRev = null;
            m_HANAppFWBuild = null;
            m_FWLoaderVer = null;
            m_FWLoaderRev = null;
            m_FWLoaderBuild = null;
        }
        #endregion

        #region Members

        //The table's member variables which represent the table 
        private byte m_CommType;
        private byte m_CommFWVer;
        private byte m_CommFWRev;
        private byte m_CommFWBuild;
        private byte m_HanType;
        private byte m_HanFWVer;
        private byte m_HanFWRev;
        private byte m_HanFWBuild;
        private byte m_RegFWVer;
        private byte m_RegFWRev;
        private byte m_RegFWBuild;
        private byte m_DisplayVer;
        private byte m_DisplayRev;
        private byte m_DisplayBuild;
        private byte? m_HANAppFWVer;
        private byte? m_HANAppFWRev;
        private byte? m_HANAppFWBuild;
        private byte? m_FWLoaderVer;
        private byte? m_FWLoaderRev;
        private byte? m_FWLoaderBuild;
        private TableState m_TableStateExtended = TableState.Unloaded;

        #endregion
    }
}
