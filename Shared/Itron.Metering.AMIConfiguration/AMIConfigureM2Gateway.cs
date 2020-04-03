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
//                           Copyright © 2010 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.LandisGyr.Gateway;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Progressable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Itron.Metering.AMIConfiguration
{
    /// <summary>
    /// Handles the configuration of the M2 Gateway module
    /// </summary>
    public class AMIConfigureM2Gateway : AMIConfigureDevice
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEMObject">PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/10 AF  2.41.09        Created
        //  07/08/10 AF  2.42.03        Updated
        //
        public AMIConfigureM2Gateway(CPSEM PSEMObject) : base(PSEMObject)
        {
            m_GatewayTables = new GatewayTables();
        }

        /// <summary>
        /// Configures the meter with the specified EDL file and displays a dialog
        /// to retrieve the prompt for values.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file</param>
        /// <returns>ConfigurationError code</returns>
        //  Revision History	
        //  MM/DD/YY Who Version     Issue# Description
        //  -------- --- ----------- ------ -------------------------------------------
        //  06/11/10 AF  2.41.09            Created
        //  07/08/10 AF  2.42.03            Updated to use C1219Tables.LandisGyr.Gateway dll
        //  06/08/17 DLG 1.2017.6.23        Updated logic around FileStream to make sure we dispose the object.
        //
        public override ConfigurationError Configure(string strEDLFileName)
        {
            XmlTextReader EDLXMLReader;
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            if (File.Exists(strEDLFileName))
            {
                using(var EDLFileStream = new FileStream(strEDLFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    EDLXMLReader = new XmlTextReader(EDLFileStream);

                    // Read table 0 to prepare for loading the EDL file
                    ConfigError = ReadTable(0);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        try
                        {
                            // Load the EDL File
                            m_GatewayTables.LoadEDLFile(EDLXMLReader);
                        }
                        catch (Exception)
                        {
                            ConfigError = ConfigurationError.INVALID_PROGRAM;
                        }
                    }
                }

                // Read table 0 again, now that the file is loaded
                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    ConfigError = ReadTable(0);
                }

                // Read table 1
                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    ConfigError = ReadTable(1);
                }

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    m_PSEM.Wait(255);
                
                    // Load the prompt for items
                    ConfigError = LoadFactoryPromptForItems();
                }

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    // Set the Version Information
                    SetSoftwareVersionInformation();

                    SetDateLastProgrammed();

                    // Write the Configuration to the meter
                    ConfigError = WriteConfiguration();
                }
            }
            else
            {
                // File does not exist
                ConfigError = ConfigurationError.FILE_NOT_FOUND;
            }


            return ConfigError;
        }

        /// <summary>
        /// Reads the specified table from the meter and stores it in the program tables
        /// </summary>
        /// <param name="usTableID">The table number of the table to read.</param>
        /// <returns>ConfigurationError code</returns>
        // Revision History:
        // MM/DD/YY who Version     Issue# Description
        // -------- --- ----------- ------ ---------------------------------------
        // 08/05/10 AF  2.42.13            Created
        // 06/08/17 DLG 1.2017.6.23        Updated logic around MemoryStream to make sure we dispose the object.
        public override ConfigurationError ReadTable(ushort usTableID)
        {
            PSEMResponse PSEMResult;
            byte[] byaData;

            try
            {
                PSEMResult = m_PSEM.FullRead(usTableID, out byaData);
            }
            catch (TimeOutException)
            {
                return ConfigurationError.TIMEOUT;
            }

            if (PSEMResult == PSEMResponse.Ok)
            {
                using (var PSEMDataStream = new MemoryStream(byaData))
                {
                    m_GatewayTables.SavePSEMStream(usTableID, PSEMDataStream);
                }
            }

            return InterpretPSEMResult(PSEMResult);
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets the software version information into the program
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/16/10 AF  2.41.10        Created
        // 07/08/10 AF  2.42.03        Updated to use C1219Tables.LandisGyr.Gateway dll
        //
        protected override void SetSoftwareVersionInformation()
        {
#if (!WindowsCE)
            FileVersionInfo AssemblyVersion;

            // Get this assembly's version information using reflection
            AssemblyVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            // Add the software Vendor information
            m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VENDOR, null, VENDOR_NAME);
            m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VENDOR, null, VENDOR_NAME);

            // Add the software version information
            m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.ProductMajorPart);
            m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.ProductMajorPart);
            m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.ProductMinorPart);
            m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.ProductMinorPart);
#else
			//Get the assembly verion for CE
			Assembly assm = Assembly.GetExecutingAssembly();
			Version AssemblyVersion = assm.GetName().Version;

			// Add the software Vendor information
			m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VENDOR, null, VENDOR_NAME);
			m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VENDOR, null, VENDOR_NAME);

			// Add the software version information
			m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.Major);
			m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.Major);
			m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.Minor);
			m_GatewayTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.Minor);
#endif
        }

        /// <summary>
        /// Writes the configuration tables to the meter
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/11/10 AF          	   Created
        // 07/08/10 AF  2.42.03        Updated to use C1219Tables.LandisGyr.Gateway dll
        //
        protected override ConfigurationError WriteConfiguration()
        {
            List<ushort> listConfigurationTables;
            ConfigurationError ConfigErrorCode = ConfigurationError.SUCCESS;

            // Get the list of tables to configure
            listConfigurationTables = GetTablesToConfigure();

            OnShowProgress(new ShowProgressEventArgs(1, listConfigurationTables.Count, "Device Configuration", "Writing Configuration..."));

            // Write each of the configuration tables
            foreach (ushort usTableID in listConfigurationTables)
            {
                if (ConfigErrorCode == ConfigurationError.SUCCESS && m_GatewayTables.IsTableKnown(usTableID) && IsTableSupported(usTableID))
                {
                    ConfigErrorCode = WriteTable(usTableID);

                    if (ConfigErrorCode == ConfigurationError.ITEM_NOT_FOUND)
                    {
                        // This is not a critical table so it does not have to be written in order for
                        // configuration to succeed
                        ConfigErrorCode = ConfigurationError.SUCCESS;
                    }

                    if (usTableID == 2048 && ConfigErrorCode == ConfigurationError.SUCCESS)
                    {
                        ConfigErrorCode = CloseConfiguration();
                    }
                }


                OnStepProgress(new ProgressEventArgs());
            }

            OnHideProgress(new EventArgs());

            return ConfigErrorCode;
        }

        /// <summary>
        /// Generates a list of table numbers that are in the order that the meter
        /// will be configured.
        /// </summary>
        /// <returns>List of table numbers</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/11/10 AF          	   Created
        // 07/08/10 AF  2.42.03        Updated to use C1219Tables.LandisGyr.Gateway dll
        // 10/14/10 AF  2.45.04 161866 Added table 34 and removed 2090 and 2260
        // 10/21/10 AF  2.45.06 161866 Added table 2171
        // 04/01/11 AF  2.50.20 171132 Added table 2260
        //
        protected override List<ushort> GetTablesToConfigure()
        {
            List<ushort> listTables = new List<ushort>();

            // Manually add the tables in the order that they will be configured.
            listTables.Add(6);      // Utility Information Table
            listTables.Add(34);
            listTables.Add(42);     // Security Table

            listTables.Add(53);     // Time offset table

            //listTables.Add(73);     // Event configuration
            //listTables.Add(75);     // History configuration

            if (m_GatewayTables.IsAllCached(123))
            {
                listTables.Add(121);    // Actual Network Table
                listTables.Add(122);    // Interface Control Table
                listTables.Add(123);    // Exception Report Table
            }

            listTables.Add(2061);   // Factory Configuration Table
            listTables.Add(2106);   // HAN Config Parameters
            listTables.Add(2159);   // Actual Communication Log Table
            listTables.Add(2161);   // LAN Control Table
            listTables.Add(2163);   // HAN Control Table
            listTables.Add(2171);   // Meter Exception Report Table
            listTables.Add(2190);   // Communications Config Table
            listTables.Add(2193);   // Security Activation Table
            listTables.Add(2260);   // SR 3.0 Config Table
            listTables.Add(2048);   // Configuration Table

            return listTables;
        }

        /// <summary>
        /// Calls the procedure to save the configuration
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/10 AF  2.41.09        Created
        // 07/08/10 AF  2.42.03        Updated to use C1219Tables.LandisGyr.Gateway dll
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetValue")]
        protected override ConfigurationError CloseConfiguration()
        {
            ConfigurationError ConfigError;
            DateTime dtStartTime;
            TimeSpan tsSpan;
            object objValue;
            byte byReturnCode = (byte)ProcedureResultCodes.NOT_FULLY_COMPLETED;
            byte[] ProcParam;
            ProcParam = new byte[0];

            // Set up table 7 and 8 for Std Procedure 2
            m_GatewayTables.SetupProcedureRequest((ushort)ANSIProcedures.SAVE_CONFIGURATION);

            // MFG Procedure 2 does not take any parameters so call the procedure
            ConfigError = WriteTable(7);
            m_PSEM.Wait(255);

            if (ConfigError == ConfigurationError.SUCCESS || ConfigError == ConfigurationError.TIMEOUT)
            {
                // Since this procedure can take a while to complete a TimeOut here may not really be a
                // TimeOut so we need te test to see if the meter is back.
                dtStartTime = DateTime.Now;

                do
                {
                    // Give the meter some time to process
                    System.Threading.Thread.Sleep(700);

                    ConfigError = ReadTable(8);
                    m_PSEM.Wait(255);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        m_GatewayTables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
                        if (objValue is byte)
                        {
                            byReturnCode = (byte)objValue;
                            if ((byte)objValue == (byte)ProcedureResultCodes.NO_AUTHORIZATION)
                            {
                                ConfigError = ConfigurationError.SECURITY_ERROR;
                            }
                            else if (byReturnCode != (byte)ProcedureResultCodes.COMPLETED && byReturnCode != (byte)ProcedureResultCodes.NOT_FULLY_COMPLETED)
                            {
                                ConfigError = ConfigurationError.FAILED_PROCEDURE;
                            }
                        }
                        else
                        {
                            throw new InvalidCastException("Unexpected type returned by GetValue.");
                        }
                    }

                    tsSpan = DateTime.Now - dtStartTime;

                } while (byReturnCode == (byte)ProcedureResultCodes.NOT_FULLY_COMPLETED && tsSpan.TotalSeconds < 30);
            }

            return ConfigError;
        }

        /// <summary>
        /// Loads the Factory prompt for values into the program tables
        /// </summary>
        /// <returns>ConfigurationError Code</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/08/10 AF  2.42.02        Created
        // 07/08/10 AF  2.42.03        Updated to use C1219Tables.LandisGyr.Gateway dll
        //
        protected override ConfigurationError LoadFactoryPromptForItems()
        {
            ConfigurationError ConfigError = ConfigurationError.SUCCESS;

            try
            {
                // Load the Unit ID
                m_GatewayTables.SetValue(StdTableEnum.STDTBL6_DEVICE_ID, null, m_strUnitID);

                // Load the Customer Serial Number
                m_GatewayTables.SetValue(StdTableEnum.STDTBL6_UTIL_SER_NO, null, m_strCustomerSerialNumber);
            }
            catch (Exception)
            {
                ConfigError = ConfigurationError.ITEM_NOT_FOUND;
            }

            return ConfigError;
        }

        /// <summary>
        /// Sets the date last programmed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/10 AF  2.42.02        Created
        //  07/08/10 AF  2.42.03        Updated to use C1219Tables.LandisGyr.Gateway dll
        //  08/04/10 AF  2.42.12        Date last programmed item takes a DateTime object
        //
        protected override void SetDateLastProgrammed()
        {

            DateTime ProgrammingDate = DateTime.Now;

            m_GatewayTables.SetValue(GatewayTblEnum.MFGTBL0_CONFIGURATION_TIME_DATE, null, ProgrammingDate);
        }

        /// <summary>
        /// Determines if the table is supported by the meter.
        /// </summary>
        /// <param name="TableID">The ID of the table to check.</param>
        /// <returns>True if the table is supported false otherwise.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/08/10 AF  2.42.02        Created
        // 07/08/10 AF  2.42.03        Updated to use C1219Tables.LandisGyr.Gateway dll
        //
        protected override bool IsTableSupported(ushort TableID)
        {
            object objValue = null;
            bool bTableUsed = false;

            try
            {
                if (TableID < 2048)
                {
                    m_GatewayTables.GetValue(StdTableEnum.STDTBL0_STD_TBLS_USED, new int[] { TableID }, out objValue);
                }
                else
                {
                    m_GatewayTables.GetValue(StdTableEnum.STDTBL0_MFG_TBLS_USED, new int[] { TableID % 2048 }, out objValue);
                }

                if (objValue != null && objValue is bool)
                {
                    bTableUsed = (bool)objValue;
                }
            }
            catch (Exception)
            {
                // The table is not supported
                bTableUsed = false;
            }

            return bTableUsed;
        }

        /// <summary>
        /// Writes the specified table to the meter.
        /// </summary>
        /// <param name="usTableID">Table number for the table to write</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/08/10 AF  2.42.02        Created
        // 07/08/10 AF  2.42.03        Updated to use C1219Tables.LandisGyr.Gateway dll
        //
        public override ConfigurationError WriteTable(ushort usTableID)
        {
            TableData[] AllTableData;
            MemoryStream PSEMDataStream;
            PSEMResponse PSEMResult = new PSEMResponse();

            PSEMResult = PSEMResponse.Ok;

            // Get the PSEM streams
            AllTableData = m_GatewayTables.BuildPSEMStreams(usTableID);

            if (AllTableData.Length > 0)
            {
                // Write the stream to the meter
                foreach (TableData CurrentTableData in AllTableData)
                {
                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        try
                        {
                            if (CurrentTableData.FullTable == true)
                            {
                                // Write the whole table at once
                                PSEMDataStream = (MemoryStream)CurrentTableData.PSEM;
                                PSEMResult = m_PSEM.FullWrite(usTableID, PSEMDataStream.ToArray());
                            }
                            else
                            {
                                // Perform an offset write
                                PSEMDataStream = (MemoryStream)CurrentTableData.PSEM;
                                PSEMResult = m_PSEM.OffsetWrite(usTableID, (int)CurrentTableData.Offset, PSEMDataStream.ToArray());
                            }
                        }
                        catch (TimeOutException)
                        {
                            return ConfigurationError.TIMEOUT;
                        }
                    }
                }
            }
            else
            {
                // The table is not cached
                return ConfigurationError.ITEM_NOT_FOUND;
            }

            return InterpretPSEMResult(PSEMResult);
        }

        #endregion

        #region Members

        /// <summary>
        /// handles reading from and writing to M2 Gateway EDL files
        /// </summary>
        protected GatewayTables m_GatewayTables; 

        #endregion
    }
}
