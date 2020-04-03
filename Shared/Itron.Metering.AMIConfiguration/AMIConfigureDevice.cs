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
//                           Copyright © 2006 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.Threading;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Progressable;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.AMIConfiguration
{
    /// <summary>
    /// Handles the configuration of an AMI meter.
    /// </summary>
    public class AMIConfigureDevice : IProgressable
    {
        #region Public Events
        /// <summary>
        /// Event that hides the progress bar
        /// </summary>
        public event HideProgressEventHandler HideProgressEvent;

        /// <summary>
        /// Event that shows the progress bar
        /// </summary>
        public event ShowProgressEventHandler ShowProgressEvent;

        /// <summary>
        /// Event that causes the progress bar to perform a step
        /// </summary>
        public event StepProgressEventHandler StepProgressEvent;

        #endregion
        
        #region Constants

        /// <summary>
        /// Vendor name to program into the meter
        /// </summary>
        protected const string VENDOR_NAME = "ITRN";

        /// <summary>
        /// Flags used for setting the meter time
        /// </summary>
        protected const byte SET_MASK = 0x07;

        // The following are constants used for calling MFG Procedure 2 and indicate what
        // values should be reset

        /// <summary>
        /// Clear all registers upon configuration
        /// </summary>
        protected const uint COMPLETE_CONFIG = 0x80000000;

        /// <summary>
        /// Clear all registers and make the meter Canadian
        /// </summary>
        protected const uint COMPLETE_CANADIAN_CONFIG = 0xC0000000;

        ///// <summary>
        ///// Do not clear the energy registers upon configuration
        ///// </summary>
        //private const uint CONFIG_SAVE_ENERGY_REGISTERS = 0x00129AE8;

        ///// <summary>
        ///// Do not clear the billing registers upon configuration
        ///// </summary>
        //private const uint CONFIG_SAVE_BILLING_REGISTERS = 0x00129AE0;

        ///// <summary>
        ///// Sets the meter as Canadian (does not seal the meter)
        ///// </summary>
        //private const uint CANADIAN_CONFIG = 0x40000000;

        private const byte COMMIT_ICS_CONFIG = 4;
        private const ushort GATEWAY_ADDRESS_DEFAULT_PORT = 1153;
        private const int IP_ADDRESS_LENGTH = 4;
        private const int PORT_LENGTH = 2;
        private const int GATEWAY_ADDRESS_PORT_OFFSET = 4;
        private const int IP_ADDRESS_TYPE = 3;
        private const int GATEWAY_ADDRESS_LENGTH = IP_ADDRESS_LENGTH + PORT_LENGTH;
        
        private readonly List<int> ICS_CONFIG_TABLES = new List<int>() { 2509, 2512, 2517, 2523, 2536, 2537 };                

        #endregion

        #region Definitions

        /// <summary>
        /// Options to indicate what data to configure.
        /// </summary>
        public enum ConfigurationOptions
        {
            /// <summary>
            /// Full configuration, all data should be configured.
            /// </summary>
            Full,
            /// <summary>
            /// ICS Register only configuration, only ICS register data should be configured
            /// </summary>
            ICSRegisterOnly,
            /// <summary>
            /// ICS Comm Module only configuration, only ICS comm module data should be configured.
            /// </summary>
            ICSCommModuleOnly,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEMObject">PSEM protocol object that is currently logged on to the meter.</param>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/23/06 RCG 7.40.00 N/A    Created

        public AMIConfigureDevice(CPSEM PSEMObject)
        {
            m_PSEM = PSEMObject;
            m_CentronAMITables = new CentronTables();
            m_bIsCanadian = false;
            m_strUnitID = "";
            m_strCustomerSerialNumber = "";
            m_InitialDateTime = null;
        }

        /// <summary>
        /// Configures the meter with the specified EDL file and displays a dialog
        /// to retrieve the prompt for values.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00    N/A	  Created
        // 09/07/06 RCG 7.40.00    N/A    Added SetClock before configuring so that the
        //                                clock is not the reference date on configuration
        //                                of a 3 button reset meter.
        // 03/27/08 RCG 1.50.11           Removing prompt for dialog.
        // 07/08/10 AF  2.42.03           Made virtual for use in M2 Gateway
        // 09/09/10 jrf 2.44.00    158657 Removing the second set clock after writing the 
        //                                configuration.
        // 04/08/13 jrf 2.80.19 TQ 7652   Added call to activate ICS config if necessary.
        // 05/09/13 jrf 2.80.28 TQ 7652   Moved call to activate ICS config to very end of method
        //                                and now setting method's result based on outcome of activating.
        // 06/18/13 jrf 2.80.38    N/A    Updating to suppport separate factory configurations of the 
        //                                register and comm module for ICS meters.
        // 07/18/13 jrf 2.80.54 WR 417794 Adding configuration of new field to factory configure of ICS comm module.
        // 08/06/13 jrf 2.85.11 WR 419350 Setting m_blnActivateICSConfiguration to true when configuration is 
        //                                ICS Module only. This will force commit config procedure to be called and 
        //                                this procedure will fail when ICS comm module is not attached.
        // 12/11/13 jrf 3.50.14 TQ 9477   Added call to method to generate the 25 year TOU calendar. 
        // 01/07/14 jrf 3.50.19 WR 444803 The call to generate 25 year TOU schedule data was corrupting mfg TOU config
        //                                table 2090 data. So moved it before call to update mfg. TOU config table.  
        // 01/08/14 jrf 3.50.22 TQ 9477  Throwing error when meter supports 25 Year TOU calendar but the configuration
        //                               does not.
        // 04/29/14 jrf 3.50.86 WR 502828 Updated check on wheter or not to generate 25 year calendar data for configuration. 
        public virtual ConfigurationError Configure(string strEDLFileName)
        {
            FileStream EDLFileStream = null;
            XmlTextReader EDLXMLReader;
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;
            DateTime dtTOUStartTime;
            DateTime dtTOUNextSeasonStartTime;
            bool bTOUDemandReset;
            bool bTOUSelfRead;

            try
            {

                if (File.Exists(strEDLFileName))
                {
                    EDLFileStream = new FileStream(strEDLFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    EDLXMLReader = new XmlTextReader(EDLFileStream);

                    // Read table 0 to prepare for loading the EDL file
                    ConfigError = ReadTable(0);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        try
                        {
                            // Load the EDL File
                            m_CentronAMITables.LoadEDLFile(EDLXMLReader);
                        }
                        catch (Exception)
                        {
                            ConfigError = ConfigurationError.INVALID_PROGRAM;
                        }
                    }

                    // Correct any Program Issues
                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        CorrectProgramforMissingItems();
                    }

                    // Read table 0 again now that the file is loaded
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

                        GetRegFWVersion();

                        DateTime TOUConfigTime = DateTime.Now;

                        if (InitialDateTime != null)
                        {
                            // We should use the specified time to initialize the meter with
                            if (InitialDateTime.Value.Kind == DateTimeKind.Utc)
                            {
                                TOUConfigTime = InitialDateTime.Value.ToLocalTime();
                            }
                            else
                            {
                                TOUConfigTime = InitialDateTime.Value;
                            }
                        }

                        //Only bother generating 25 year TOU data if meter supports it AND
                        //  1.it is a factory configuration (In this case we always need to configure it)
                        //  or
                        //  2.the configuration file supports 25 year calendar data
                        if (IsTableSupported(2437) && (IsFactoryConfig || m_CentronAMITables.Supports25YearCalendarFromStandardTables(TOUConfigTime)))
                        {
                            try
                            {
                                //Call CE Dll method to generate the 25 year TOU schedule in mfg. table 2437
                                m_CentronAMITables.Create25YearCalendarFromStandardTables(TOUConfigTime, true);

                            }
                            catch (MfgTbl389TouConvertException)
                            {
                                // The TOU schedule defined in the program is not supported by the 25 year TOU schedule
                                ConfigError = ConfigurationError.INVALID_PROGRAM;
                            }
                            catch (Exception)
                            {
                                // There is something wrong with the TOU
                                ConfigError = ConfigurationError.INVALID_PROGRAM;
                            }
                        }

                        //WR 444803 - The call to update the TOU schedule to use the current season needs to come after the 
                        //call to update the 25 year TOU calendar for now. Otherwise the data in mfg. table 2090 is not populated
                        //correctly and results in a Non-Fatal Error #3 (TOU/Clock error) in meter.
                        try
                        {
                            // Update the TOU schedule to use the current season.
                            m_CentronAMITables.UpdateTOUSeasonFromStandardTables(TOUConfigTime, 0,
                                out dtTOUStartTime, out bTOUDemandReset, out bTOUSelfRead, out dtTOUNextSeasonStartTime);

                        }
                        catch (ArgumentException)
                        {
                            // The program does not contain TOU or has the old TOU format so we can just continue
                        }
                        catch (Exception)
                        {
                            // There is something wrong with the TOU
                            ConfigError = ConfigurationError.INVALID_PROGRAM;
                        }

                    }

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        // Load the prompt for items
                        ConfigError = LoadFactoryPromptForItems();
                    }

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        //when factory is configuring just the ICS comm module there are 
                        //a few non-config file values we need to configure.
                        if (ConfigurationOptions.ICSCommModuleOnly == FactoryConfigType)
                        {
                            ConfigError = SetICSGatewayAddress();

                            if (ConfigError == ConfigurationError.SUCCESS)
                            {
                                ConfigError = SetICSERTUtilityID();
                            }

                            if (ConfigError == ConfigurationError.SUCCESS)
                            {
                                ConfigError = SetICSIsERTPopulated();
                            }

                            m_blnActivateICSConfiguration = true;
                        }
                        else // Write clock value to register when not a ICS comm only configuration
                        {
                            // Set the clock so that 3 button reset meters
                            // do not have events in 1970
                            ConfigError = SetClock();
                        }
                    }

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        // Set the Version Information
                        SetSoftwareVersionInformation();

                        SetDateLastProgrammed();

                        // Write the Configuration to the meter
                        ConfigError = WriteConfiguration();
                    }

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        //Prevent a register table write when configuring ICS comm module only.
                        if (ConfigurationOptions.ICSCommModuleOnly != FactoryConfigType)
                        {
                            // Clear the status information
                            ConfigError = ClearStatusFlags();
                        }
                    }

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        //Only true if ICS config tables were successfully written.
                        if (true == m_blnActivateICSConfiguration)
                        {
                            ConfigError = ActivateICSConfiguration();
                        }
                    }
                }
                else
                {
                    // File does not exist
                    ConfigError = ConfigurationError.FILE_NOT_FOUND;
                }

                return ConfigError;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                if (null != EDLFileStream)
                {
                    EDLFileStream.Close();
                }
            }
        }

        /// <summary>
        /// Loads the EDL file into the table set. Use before any partial configuraitons.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <returns>ConfigurationError code.</returns>
        /// Revison History:
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/09/12 jkw 7.40.00 N/A	   Created
        /// 
        public virtual ConfigurationError LoadEDLFileToTableSet(string strEDLFileName)
        {
            FileStream EDLFileStream = null;
            XmlTextReader EDLXMLReader;
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            try
            {
                if (File.Exists(strEDLFileName))
                {
                    EDLFileStream = new FileStream(strEDLFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    EDLXMLReader = new XmlTextReader(EDLFileStream);

                    // Read table 0 to prepare for loading the EDL file
                    ConfigError = ReadTable(0);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        try
                        {
                            // Load the EDL File
                            m_CentronAMITables.LoadEDLFile(EDLXMLReader);
                        }
                        catch (Exception)
                        {
                            ConfigError = ConfigurationError.INVALID_PROGRAM;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (null != EDLFileStream)
                {
                    EDLFileStream.Close();
                }
            }

            return ConfigError;
        }

        /// <summary>
        /// Loads the EDL file into the table set. Use before any partial configuraitons.
        /// </summary>
        /// <param name="startElement">Start table element</param>
        /// <param name="anStartIndex">Start table element index array. null if none.</param>
        /// <param name="endElement">End table element. Must be in the same table</param>
        /// <param name="anEndIndex">End table element index array. null if none.</param>
        /// <returns>ConfigurationError code.</returns>
        /// Revison History:
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/09/12 jkw 7.40.00 N/A	   Created
        /// 
        public virtual ConfigurationError WriteTableByElementRange(CentronTblEnum startElement, int[] anStartIndex, CentronTblEnum endElement, int[] anEndIndex)
        {
            TableData[] AllTableData;
            MemoryStream PSEMDataStream;
            PSEMResponse PSEMResult = new PSEMResponse();

            PSEMResult = PSEMResponse.Ok;

            // Get the PSEM streams
            AllTableData = m_CentronAMITables.BuildPSEMStreams((long)startElement, anStartIndex, (long)endElement, anEndIndex);

            if (AllTableData != null && AllTableData.Length > 0)
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
                                PSEMResult = m_PSEM.FullWrite(CurrentTableData.TableID, PSEMDataStream.ToArray());
                            }
                            else
                            {
                                // Perform an offset write
                                PSEMDataStream = (MemoryStream)CurrentTableData.PSEM;
                                PSEMResult = m_PSEM.OffsetWrite(CurrentTableData.TableID, (int)CurrentTableData.Offset, PSEMDataStream.ToArray());
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

        /// <summary>
        /// Returns the last index of the field array
        /// </summary>
        /// <param name="element">Field array table element</param>
        /// <returns>Last index</returns>
        /// Revison History:
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/09/12 jkw 2.60.XX N/A	   Created - Lithium
        /// 
        public int GetFieldArrayLastIndex(CentronTblEnum element)
        {
            int returnValue = 0;
            int[] limits = m_CentronAMITables.GetElementLimits(element);

            if (limits != null && limits.Length > 0)
            {
                returnValue = limits[0] - 1;
            }

            return returnValue;
        }

        /// <summary>
        /// Writes the specified table to the meter.
        /// </summary>
        /// <param name="usTableID">Table number for the table to write</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 07/08/10 AF  2.42.03        Made virtual for use in M2 Gateway
        // 12/30/13 AF  3.50.17 WR444630 Made sure that writes to table 7 are always full table writes
        //
        public virtual ConfigurationError WriteTable(ushort usTableID)
        {
            TableData[] AllTableData;
            MemoryStream PSEMDataStream;
            PSEMResponse PSEMResult = new PSEMResponse();
            int RetryCount = 0;

            PSEMResult = PSEMResponse.Ok;

            // Get the PSEM streams
            AllTableData = m_CentronAMITables.BuildPSEMStreams(usTableID);

            if (AllTableData.Length > 0)
            {
                // Write the stream to the meter
                foreach (TableData CurrentTableData in AllTableData)
                {
                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        try
                        {
                            // Make sure we always do a full table write for table 7
                            if ((CurrentTableData.FullTable == true) || (usTableID == 7))
                            {
                                // Write the whole table at once
                                PSEMDataStream = (MemoryStream)CurrentTableData.PSEM;
                                PSEMResult = m_PSEM.FullWrite(usTableID, PSEMDataStream.ToArray());

                                //Try to recover if we get a busy or data not ready response.
                                while ((PSEMResponse.Bsy == PSEMResult || PSEMResponse.Dnr == PSEMResult)
                                    && RetryCount++ < 3)
                                {
                                    Thread.Sleep(2000);

                                    //Resend the procedure to the meter
                                    PSEMResult = m_PSEM.FullWrite(usTableID, PSEMDataStream.ToArray());
                                }
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

        /// <summary>
        /// Reads the specified table from the meter and stores it in the program tables
        /// </summary>
        /// <param name="usTableID">The table number of the table to read.</param>
        /// <returns>ConfigurationError code</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 08/05/10 AF  2.42.13        Made virtual to allow override for the M2 Gateway

        public virtual ConfigurationError ReadTable(ushort usTableID)
        {
            MemoryStream PSEMDataStream = null; 
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

            try
            {
                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(byaData);
                    m_CentronAMITables.SavePSEMStream(usTableID, PSEMDataStream);
                }
            }
            catch { }
            finally
            {
                if (null != PSEMDataStream)
                {
                    PSEMDataStream.Close();
                }
            }

            return InterpretPSEMResult(PSEMResult);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets a boolean that determines if the meter is Canadian
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        public bool IsCanadian
        {
            get
            {
                return m_bIsCanadian;
            }
            set
            {
                m_bIsCanadian = value;
            }
        }

        /// <summary>
        /// Gets or sets the Unit ID
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        public string UnitID
        {
            get
            {
                return m_strUnitID;
            }
            set
            {
                m_strUnitID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Customer Serial Number
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        public string CustomerSerialNumber
        {
            get
            {
                return m_strCustomerSerialNumber;
            }
            set
            {
                m_strCustomerSerialNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the date and time to initialize the meter with.
        /// If this value is null the current time will be used.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/20/09 RCG 2.10.02 N/A	   Created

        public DateTime? InitialDateTime
        {
            get
            {
                return m_InitialDateTime;
            }
            set
            {
                m_InitialDateTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the PSEM object
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        public CPSEM PSEM
        {
            get
            {
                return m_PSEM;
            }
            set
            {
                m_PSEM = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the factory interface is being used.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/09/10 RCG 2.40.23 N/A	   Created

        public bool IsFactoryConfig
        {
            get
            {
                return m_bIsFactoryConfig;
            }
            set
            {
                m_bIsFactoryConfig = value;
            }
        }

        /// <summary>
        /// Gets or sets the Gateway address to configure in the ICS comm module.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/18/13 jrf 2.80.38 N/A	   Created

        public IPAddress GatewayAddress
        {
            get
            {
                return m_GatewayAddress;
            }
            set
            {
                m_GatewayAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the ERT utility ID to configure in the ICS comm module.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/18/13 jrf 2.80.38 N/A	   Created

        public ushort? ERTUtilityID
        {
            get
            {
                return m_usERTUtilityID;
            }
            set
            {
                m_usERTUtilityID = value;
            }
        }

        /// <summary>
        /// Gets or sets the ERT utility ID to configure in the ICS comm module.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 07/18/13 jrf 2.80.54 WR 417794 Created

        public byte? IsERTPopulated
        {
            get
            {
                return m_byIsERTPopulated;
            }
            set
            {
                m_byIsERTPopulated = value;
            }
        }

        /// <summary>
        /// Gets or sets the what data should be configured.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/18/13 jrf 2.80.38 N/A	   Created

        public ConfigurationOptions FactoryConfigType
        {
            get
            {
                return m_ConfigType;
            }
            set
            {
                m_ConfigType = value;
            }
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Translates a PSEM ResultCode into a ConfigurationError
        /// </summary>
        /// <param name="PSEMResult">PSEMResultCode to translate</param>
        /// <returns>The corresponding ConfigurationError</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        internal static ConfigurationError InterpretPSEMResult(PSEMResponse PSEMResult)
        {
            switch (PSEMResult)
            {
                case PSEMResponse.Ok:
                {
                    return ConfigurationError.SUCCESS;
                }
                case PSEMResponse.Sns:
                {
                    return ConfigurationError.SERVICE_NOT_SUPPORTED;
                }
                case PSEMResponse.Isc:
                {
                    return ConfigurationError.SECURITY_ERROR;
                }
                case PSEMResponse.Onp:
                {
                    return ConfigurationError.OPERATION_NOT_POSSIBLE;
                }
                case PSEMResponse.Iar:
                {
                    return ConfigurationError.INNAPROPRIATE_ACTION_REQUEST;
                }
                case PSEMResponse.Bsy:
                {
                    return ConfigurationError.DEVICE_BUSY;
                }
                case PSEMResponse.Dnr:
                {
                    return ConfigurationError.DATA_NOT_READY;
                }
                case PSEMResponse.Dlk:
                {
                    return ConfigurationError.DATA_LOCKED;
                }
                case PSEMResponse.Rno:
                {
                    return ConfigurationError.RENEGOTIATE_REQUEST;
                }
                case PSEMResponse.Isss:
                {
                    return ConfigurationError.INVALID_SERVICE_SEQUENCE_STATE;
                }
                default:
                {
                    return ConfigurationError.GENERAL_ERROR;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check for missing configuration items and fix them
        /// </summary>
        /// <returns>ConfigurationError Code</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/15/09 KRC 2.21.13         Look for missing C12.18 over ZigBee and default it to zero if it is missing
        private void CorrectProgramforMissingItems()
        {
            // With programs created prior to SP5, the C12.18 over ZigBee bit was not defined in the configuration
            //  file.  When we try to build table 2193, the build stream fails because all of the bit elements
            //  are not defined in the EDL file.  Therefore, we have added a check here to see if the item exists. 
            //  If it exists we leave it alone, but if it doesn't we set the value to zero which is the default.
            if (!m_CentronAMITables.IsCached((long)CentronTblEnum.MfgTbl145C1218OverZigBee, null))
            {
                m_CentronAMITables.SetValue((long)CentronTblEnum.MfgTbl145C1218OverZigBee, null, 0);
            }
        }

        /// <summary>
        /// Loads the Factory prompt for values into the program tables
        /// </summary>
        /// <returns>ConfigurationError Code</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/17/06 RCG 7.40.00 N/A	   Created
        // 06/11/10 AF  2.41.09        Changed the access modifier
        // 07/08/10 AF  2.42.03        Made virtual

        protected virtual ConfigurationError LoadFactoryPromptForItems()
        {
            ConfigurationError ConfigError = ConfigurationError.SUCCESS;

            try
            {
                // Load the Unit ID
                m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_DEVICE_ID, null, m_strUnitID);

                // Load the Customer Serial Number
                m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_UTIL_SER_NO, null, m_strCustomerSerialNumber);
            }
            catch (Exception)
            {
                ConfigError = ConfigurationError.ITEM_NOT_FOUND;
            }

            return ConfigError;
        }

        /// <summary>
        /// Calls the procedure necessary to set the meter's clock
        /// </summary>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 06/11/10 AF  2.41.09        Changed the access modifier

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetValue")]
        protected ConfigurationError SetClock()
        {
            DateTime dtCurrentTime = DateTime.UtcNow;
            ConfigurationError ConfigError;
            object objValue;

            if (InitialDateTime != null)
            {
                // We should use the specified time to initialize the meter with
                if (InitialDateTime.Value.Kind != DateTimeKind.Utc)
                {
                    dtCurrentTime = InitialDateTime.Value.ToUniversalTime();
                }
                else
                {
                    dtCurrentTime = InitialDateTime.Value;
                }
            }

            m_CentronAMITables.SetupProcedureRequest((ushort)ANSIProcedures.SET_DATE_TIME);

            // Set up the Set Mask bit field
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL7_PROC10_SET_MASK, null, SET_MASK);

            // Set up the Time Date Qual bit field

            // The DayOfWeek Type directly corresponds to the day values in the meter 0 = Sunday, 1 = Moday, ...
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL7_PROC10_DAY_OF_WEEK, null, (byte)dtCurrentTime.DayOfWeek);
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL7_PROC10_DST_FLAG, null, false);
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL7_PROC10_GMT_FLAG, null, true);
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL7_PROC10_TM_ZN_APPLIED_FLAG, null, false);
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL7_PROC10_DST_APPLIED_FLAG, null, false);

            // Set the time
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL7_PROC10_DATE_TIME, null, dtCurrentTime);

            // Call the Procedure
            ConfigError = WriteTable(7);

            if (ConfigurationError.SUCCESS == ConfigError)
            {
                // Check the Procedure results
                ConfigError = ReadTable(8);
                if (ConfigurationError.SUCCESS == ConfigError)
                {
                    m_PSEM.Wait(255);

                    m_CentronAMITables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
                    if (objValue is byte)
                    {
                        if ((byte)objValue == (byte)ProcedureResultCodes.NO_AUTHORIZATION)
                        {
                            ConfigError = ConfigurationError.SECURITY_ERROR;
                        }
                        else if ((byte)objValue != (byte)ProcedureResultCodes.COMPLETED)
                        {
                            //TODO: add a more specific error code
                            ConfigError = ConfigurationError.FAILED_PROCEDURE;
                        }
                    }
                    else
                    {
                        throw new InvalidCastException("Unexpected type returned by GetValue.");
                    }
                }
            }

            // Clean up the Procedure tables
            m_CentronAMITables.ClearTable(7);
            m_CentronAMITables.ClearTable(8);

            return ConfigError;
        }

        /// <summary>
        /// Calls the procedure to clear Standard and Manufacturer status flags
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/30/06 RCG 8.00.00 N/A	   Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetValue")]
        private ConfigurationError ClearStatusFlags()
        {
            ConfigurationError ConfigError;
            object objValue;

            m_CentronAMITables.ClearTable(7);

            // Set up table 7 and 8 for Standard Procedure 7 to reset the Standard Status Flags
            m_CentronAMITables.SetupProcedureRequest(7);

            // Std Procedure 3 does not take any parameters so call the procedure
            ConfigError = WriteTable(7);

            if (ConfigError == ConfigurationError.SUCCESS)
            {
                // Check the Procedure results
                ConfigError = ReadTable(8);

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    m_CentronAMITables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
                    if (objValue is byte)
                    {
                        if ((byte)objValue != (byte)ProcedureResultCodes.COMPLETED)
                        {
                            ConfigError = ConfigurationError.FAILED_PROCEDURE;
                        }
                    }
                    else
                    {
						throw new InvalidCastException("Unexpected type returned by GetValue.");
                    }
                }
            }

            if (ConfigError == ConfigurationError.SUCCESS)
            {
                m_CentronAMITables.ClearTable(7);

                // Set up table 7 and 8 for Standard Procedure 8 to reset the Manufacturer Status Flags
                m_CentronAMITables.SetupProcedureRequest(8);

                // Std Procedure 3 does not take any parameters so call the procedure
                ConfigError = WriteTable(7);

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    // Check the Procedure results
                    ConfigError = ReadTable(8);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        m_CentronAMITables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
                        if (objValue is byte)
                        {
                            if ((byte)objValue != (byte)ProcedureResultCodes.COMPLETED)
                            {
                                ConfigError = ConfigurationError.FAILED_PROCEDURE;
                            }
                        }
                        else
                        {
							throw new InvalidCastException("Unexpected type returned by GetValue.");
                        }
                    }
                }
            }

            return ConfigError;
        }

        /// <summary>
        /// Calls the procedure to Open 2048 for writing
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetValue")]
        private ConfigurationError OpenConfiguration()
        {
            ConfigurationError ConfigError;
            object objValue;

            // Set up table 7 and 8 for MFG Procedure 1
            m_CentronAMITables.SetupProcedureRequest(2049);

            // MFG Procedure 1 does not take any parameters so call the procedure
            ConfigError = WriteTable(7);

            if (ConfigError == ConfigurationError.SUCCESS)
            {
                // Check the Procedure results
                ConfigError = ReadTable(8);

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    m_CentronAMITables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
                    if (objValue is byte)
                    {
                        if ((byte)objValue == (byte)ProcedureResultCodes.NO_AUTHORIZATION)
                        {
                            ConfigError = ConfigurationError.SECURITY_ERROR;
                        }
                        else if ((byte)objValue != (byte)ProcedureResultCodes.COMPLETED)
                        {
                            //TODO: add a more specific error code
                            ConfigError = ConfigurationError.FAILED_PROCEDURE;
                        }
                    }
                    else
                    {
						throw new InvalidCastException("Unexpected type returned by GetValue.");
                    }
                }
            }

            return ConfigError;
        }

        /// <summary>
        /// Calls the procedure to Open 2048 for writing
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 06/14/10 AF  2.41.09        Changed the access modifier and made virtual
        // 02/10/16 AF  4.50.228 WR647265 Firmware has requested that, on close config, we wait up to 80 seconds.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetValue")]
        protected virtual ConfigurationError CloseConfiguration()
        {
            ConfigurationError ConfigError;
            DateTime dtStartTime;
            TimeSpan tsSpan;
            object objValue;
            byte byReturnCode = (byte)ProcedureResultCodes.NOT_FULLY_COMPLETED;

            // Set up table 7 and 8 for MFG Procedure 2
            m_CentronAMITables.SetupProcedureRequest(2050);

            if (m_bIsCanadian == true)
            {
                // Make the meter Canadian and close the configuration
                m_CentronAMITables.SetValue(CentronTblEnum.PROCRQST02_CLOSE_REQUEST, null, COMPLETE_CANADIAN_CONFIG);
            }
            else
            {
                // Close the configuration
                m_CentronAMITables.SetValue(CentronTblEnum.PROCRQST02_CLOSE_REQUEST, null, COMPLETE_CONFIG);
            }

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
                    System.Threading.Thread.Sleep(1000);

                    ConfigError = ReadTable(8);
                    m_PSEM.Wait(255);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        m_CentronAMITables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
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
                } while (byReturnCode == (byte)ProcedureResultCodes.NOT_FULLY_COMPLETED && tsSpan.TotalSeconds < 80);
            }

            return ConfigError;
        }

        /// <summary>
        /// Calls the procedure necessary to activate configuration values in the ICS comm module.
        /// </summary>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/08/13 jrf 2.80.19 TQ7652 Created.
        // 05/02/13 jrf 2.80.25 TQ7652 Added setting function code for procedure 200 to commit config.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetValue")]
        protected ConfigurationError ActivateICSConfiguration()
        {
            //DateTime dtCurrentTime = DateTime.UtcNow;
            ConfigurationError ConfigError;
            object objValue;

            // Set up table 7 and 8 for MFG Procedure 200
            m_CentronAMITables.SetupProcedureRequest(2248);

            // Set up the sub procedure to commit the ICS configuration to flash
            m_CentronAMITables.SetValue(CentronTblEnum.ProcedureRequest200FunctionCode, null, COMMIT_ICS_CONFIG);

            // Call the Procedure
            ConfigError = WriteTable(7);
            
            if (ConfigurationError.SUCCESS == ConfigError)
            {
                // Check the Procedure results
                ConfigError = ReadTable(8);
                if (ConfigurationError.SUCCESS == ConfigError)
                {
                    m_PSEM.Wait(255);

                    m_CentronAMITables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
                    if (objValue is byte)
                    {
                        if ((byte)objValue == (byte)ProcedureResultCodes.NO_AUTHORIZATION)
                        {
                            ConfigError = ConfigurationError.SECURITY_ERROR;
                        }
                        else if ((byte)objValue != (byte)ProcedureResultCodes.COMPLETED)
                        {
                            ConfigError = ConfigurationError.FAILED_PROCEDURE;
                        }
                    }
                    else
                    {
                        throw new InvalidCastException("Unexpected type returned by GetValue.");
                    }
                }
            }

            // Clean up the Procedure tables
            m_CentronAMITables.ClearTable(7);
            m_CentronAMITables.ClearTable(8);

            m_blnActivateICSConfiguration = false;

            return ConfigError;
        }

        /// <summary>
        /// Sets the software version information into the program
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 06/11/10 AF  2.41.09        Changed the access modifier
        // 07/08/10 AF  2.42.03        Made virtual for use in M2 Gateway

        protected virtual void SetSoftwareVersionInformation()
        {
#if (!WindowsCE)
            FileVersionInfo AssemblyVersion;

            // Get this assembly's version information using reflection
            AssemblyVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

			// Add the software Vendor information
			m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VENDOR, null, VENDOR_NAME);
			m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VENDOR, null, VENDOR_NAME);

			 // Add the software version information
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.ProductMajorPart);
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.ProductMajorPart);
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX1_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.ProductMinorPart);
            m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX2_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.ProductMinorPart);
            m_CentronAMITables.SetValue(CentronTblEnum.MFGTBL0_SW_VERSION, null, (byte)AssemblyVersion.ProductMajorPart);
            m_CentronAMITables.SetValue(CentronTblEnum.MFGTBL0_SW_REVISION, null, (byte)AssemblyVersion.ProductMinorPart);
#else
			//Get the assembly verion for CE
			Assembly assm = Assembly.GetExecutingAssembly();
			Version AssemblyVersion = assm.GetName().Version;

			// Add the software Vendor information
			m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VENDOR, null, VENDOR_NAME);
			m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VENDOR, null, VENDOR_NAME);

			// Add the software version information
			m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.Major);
			m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.Major);
			m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX1_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.Minor);
			m_CentronAMITables.SetValue(StdTableEnum.STDTBL6_EX2_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.Minor);
			m_CentronAMITables.SetValue(CentronTblEnum.MFGTBL0_SW_VERSION, null, (byte)AssemblyVersion.Major);
			m_CentronAMITables.SetValue(CentronTblEnum.MFGTBL0_SW_REVISION, null, (byte)AssemblyVersion.Minor);
#endif          
        }

        /// <summary>
        /// Sets the date last programmed
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue#      Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/01/06 RCG 7.40.00 N/A	        Created
        // 10/28/08 KRC 2.00.03 00121848    Refernce Time is from 2000 in 2048
        // 06/11/10 AF  2.41.09             Changed the access modifier
        // 07/08/10 AF  2.42.03             Made virtual for use in M2 Gateway
        //
        protected virtual void SetDateLastProgrammed()
        {
            DateTime ReferenceDate = new DateTime(2000, 1, 1);
            TimeSpan Span = DateTime.Now - ReferenceDate;

            if (InitialDateTime != null)
            {
                // We should use the specified time to initialize the meter with
                if (InitialDateTime.Value.Kind == DateTimeKind.Utc)
                {
                    Span = InitialDateTime.Value.ToLocalTime() - ReferenceDate;
                }
                else
                {
                    Span  = InitialDateTime.Value - ReferenceDate;
                }
            }

            m_CentronAMITables.SetValue(CentronTblEnum.MFGTBL0_CONFIG_TIME, null, (uint)Span.TotalSeconds);
        }

        /// <summary>
        /// Sets the ICS Gateway Address.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue#      Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/18/13 jrf 2.80.38 N/A	   Created.
        // 05/18/15 jrf 4.20.07 WR 585332 Defaulting result to success in case this value is not configured.
        protected virtual ConfigurationError SetICSGatewayAddress()
        {
            ConfigurationError Result = ConfigurationError.SUCCESS;

            if (null != GatewayAddress)
            {
                try
                {
                    byte[] abyAddress = new byte[GATEWAY_ADDRESS_LENGTH];
                    byte[] abyPort = BitConverter.GetBytes(GATEWAY_ADDRESS_DEFAULT_PORT);

                    Array.Copy(GatewayAddress.GetAddressBytes(), abyAddress, IP_ADDRESS_LENGTH);
                    Array.Copy(abyPort, 0, abyAddress, GATEWAY_ADDRESS_PORT_OFFSET, PORT_LENGTH); 

                    m_CentronAMITables.SetValue(CentronTblEnum.MfgTbl464GatewayAddressType, null, IP_ADDRESS_TYPE);
                    m_CentronAMITables.SetValue(CentronTblEnum.MfgTbl464GatewayAddressLength, null, GATEWAY_ADDRESS_LENGTH);
                    m_CentronAMITables.SetValue(CentronTblEnum.MfgTbl464GatewayAddress, null, abyAddress);
                }
                catch
                {
                    Result = ConfigurationError.GENERAL_ERROR;
                }
            }

            return Result;
        }

        /// <summary>
        /// Sets the ICS ERT Utility ID.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue#      Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/18/13 jrf 2.80.38 N/A	   Created.
        // 05/18/15 jrf 4.20.07 WR 585332 Defaulting result to success in case this value is not configured.
        protected virtual ConfigurationError SetICSERTUtilityID()
        {
            ConfigurationError Result = ConfigurationError.SUCCESS;

            if (null != ERTUtilityID)
            {
                try
                {
                    m_CentronAMITables.SetValue(CentronTblEnum.MfgTbl461UtilityId, null, ERTUtilityID.Value);
                }
                catch
                {
                    Result = ConfigurationError.GENERAL_ERROR;
                }
            }

            return Result;
        }

        /// <summary>
        /// Sets the ICS Is ERT Populated field.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version ID Issue# Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 07/18/13 jrf 2.80.54 WR 417794 Created.
        // 08/13/13 jrf 2.85.16 WR 417794 Setting IsERTPopulated field.
        // 05/18/15 jrf 4.20.07 WR 585332 Defaulting result to success in case this value is not configured.
        protected virtual ConfigurationError SetICSIsERTPopulated()
        {
            ConfigurationError Result = ConfigurationError.SUCCESS;

            if (null != IsERTPopulated)
            {
                try
                {
                    m_CentronAMITables.SetValue(CentronTblEnum.MfgTbl464IsERTPopulated, null, IsERTPopulated.Value);
                }
                catch
                {
                    Result = ConfigurationError.GENERAL_ERROR;
                }
            }

            return Result;
        }
        
        /// <summary>
        /// Reads the Sub Table offsets for 2048 from the meter and places them 
        /// in the Program table so that the data locations loaded by the EDL 
        /// file will match those in the meter.
        /// </summary>
        /// <returns>ConfigurationError Code</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        private ConfigurationError Read2048Offsets()
        {
            PSEMResponse PSEMResponse = new PSEMResponse();
            uint uiStartOffset;
            uint uiLength;
            byte[] abyOffsetData;
            MemoryStream PSEMStream = null;

            try
            {
                // Get the location of the Offsets
                m_CentronAMITables.GetFieldOffset((long)CentronTblEnum.MFGTBL0_SUBTABLE_OFFSETS, null, out uiStartOffset, out uiLength);

                abyOffsetData = new byte[uiLength];

                // Read the offsets from the meter
                PSEMResponse = m_PSEM.OffsetRead(2048, (int)uiStartOffset, (ushort)uiLength, out abyOffsetData);

                if (PSEMResponse == PSEMResponse.Ok)
                {
                    PSEMStream = new MemoryStream(abyOffsetData);
                    // Now give the meter tables the correct offsets
                    m_CentronAMITables.SavePSEMStream(2048, PSEMStream, uiStartOffset);
                }
            }
            catch { }
            finally
            {
                if (null != PSEMStream)
                {
                    PSEMStream.Close();
                }
            }

            return InterpretPSEMResult(PSEMResponse);
        }

        /// <summary>
        /// Writes the configuration tables to the meter
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // Revison History:
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00    N/A	  Created
		// 03/13/07 mrj 8.00.18		      Removed wait, keep alive is now used.
        // 07/08/10 AF  2.42.03           Made virtual for use in M2 Gateway
        // 04/08/12 jrf 2.80.19 TQ 7652   Added setting variable to signal to execute the 
        //                                activate ICS config procedure when writing ICS 
        //                                config tables.
        // 06/18/13 jrf 2.80.38 WR 411414 Moved when activate ICS config variable is set.
        // 08/06/13 jrf 2.85.11 WR 419417 Removed setting m_blnActivateICSConfiguration to false when
        //                                result of WriteTable(usTableID) is not success and table is 
        //                                an ICS table. Any successful table write should cause the 
        //                                activation of ICS config regardless of whether other table
        //                                writes are unsuccessful.
        // 09/10/14 AF  4.00.54 WR 532781 Added exception handling to deal with timeout exceptions. If we get
        //                                a timeout on building the list of tables to configure, we should not
        //                                continue.
        // 09/12/14 AF  4.00.54 WR 532781 Changed the timeout exception handler to catch the Itron.Metering.Communications.TimeOutException
        //
        protected virtual ConfigurationError WriteConfiguration()
        {
            List<ushort> listConfigurationTables;
            ConfigurationError ConfigErrorCode = ConfigurationError.SUCCESS;

            try
            {
                // Get the list of tables to configure
                listConfigurationTables = GetTablesToConfigure();

                OnShowProgress(new ShowProgressEventArgs(1, listConfigurationTables.Count, "Device Configuration", "Writing Configuration..."));

                // Write each of the configuration tables
                foreach (ushort usTableID in listConfigurationTables)
                {
                    if (ConfigErrorCode == ConfigurationError.SUCCESS && m_CentronAMITables.IsTableKnown(usTableID) && IsTableSupported(usTableID))
                    {
                        if (usTableID == 2048)
                        {
                            // Open 2048 for writing
                            ConfigErrorCode = Read2048Offsets();

                            if (ConfigErrorCode == ConfigurationError.SUCCESS)
                            {
                                ConfigErrorCode = OpenConfiguration();
                            }
                        }

                        if (ConfigErrorCode == ConfigurationError.SUCCESS)
                        {
                            ConfigErrorCode = WriteTable(usTableID);
                        }

                        //This needs to be checked here before The item not found error is turned into success.
                        //Otherwise it will look like ICS config table write was successful when it was not even
                        //attempted because config file did not contain table.
                        if (IsICSConfigTable(usTableID))
                        {
                            //Set variable to signal execution of ICS config activation procedure
                            if (ConfigErrorCode == ConfigurationError.SUCCESS)
                            {
                                m_blnActivateICSConfiguration = true;
                            }
                        }

                        if (ConfigErrorCode == ConfigurationError.ITEM_NOT_FOUND && !IsCriticalTable(usTableID))
                        {
                            // This is not a critical table so it does not have to be written in order for
                            // configuration to succeed
                            ConfigErrorCode = ConfigurationError.SUCCESS;
                        }

                        if (usTableID == 2048 && ConfigErrorCode == ConfigurationError.SUCCESS)
                        {
                            // Close 2048
                            ConfigErrorCode = CloseConfiguration();
                        }

                    }


                    OnStepProgress(new ProgressEventArgs());
                }
            }
            catch (TimeOutException)
            {
                ConfigErrorCode = ConfigurationError.TIMEOUT;
            }
            finally
            {
                OnHideProgress(new EventArgs());
            }

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
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 07/08/10 AF  2.42.03        Made virtual for use in M2 Gateway
        // 06/08/11 jrf 2.51.08 175480 Adding configuration of Power Monitoring.
        // 12/13/11 jrf 2.53.17 TREQ3444 Adding configuration of new tables for Lithium project. 
        // 03/06/13 jrf 2.80.06 TQ6662 Adding configuration of table 2191.
        // 04/08/13 jrf 2.80.18 TQ7652 Adding support for cellular config tables.
        // 05/21/13 jrf 2.80.32 TQ7652 Adding table for ICS event config.
        // 06/18/13 jrf 2.80.38 N/A    Updating to suppport separate factory configurations of the 
        //                             register and comm module for ICS meters.
        // 06/24/13 jrf 2.80.42 TQ7652 Including mfg. table 2517 (cellular config table). CE will add 
        //                             configuration of cellular data timeout field from this table.
        // 08/13/13 jrf 2.80.42 WR 417794 Conditionally including the ERT configuration table based on 
        //                                whether or not it is or will be supported.
        // 11/14/13 jrf 3.50.03 TQ 9477 Including mfg. table 2437 (25 Year TOU Calendar table).
        // 09/10/14 AF  4.00.54 WR 532781 Added exception handling to deal with timeout exceptions
        // 04/27/15 AF  4.20.03 WR 577895 Added ICM network tables for configuring exceptions
        // 11/03/15 PGH 4.50.212 WR 577471 Added Bell Weather Meter tables
        // 01/21/16 PGH 4.50.225 RTT556309 Added Temperature Configuration
        //
        protected virtual List<ushort> GetTablesToConfigure()
        {
            List<ushort> listTables = new List<ushort>();
            try
            {
                if (FactoryConfigType != ConfigurationOptions.ICSCommModuleOnly)
                {
                    // Manually add the tables in the order that they will be configured.
                    listTables.Add(6);      // Utility Information Table
                    listTables.Add(42);     // Security Table
                    listTables.Add(46);     // Key Table
                    listTables.Add(82);     // User Defined Tables List Table
                    listTables.Add(83);     // User Definded Tables Selection Table

                    // Prior to the SP5.1 there was an issue that could cause read rate problems if the exceptions
                    // are configured by the factory so we should not configure these items prior to SP5.1
                    if (m_CentronAMITables.IsAllCached(123) && VersionChecker.CompareTo(m_fFWVersion, 2.006f) >= 0)
                    {
                        listTables.Add(121);    // Actual Network Table
                        listTables.Add(123);    // Exception Report Table
                    }

                    listTables.Add(2090);   // TOU Table
                    listTables.Add(2106);   // HAN Config Parameters
                    listTables.Add(2141);   // Service Limiting Config Table
                    listTables.Add(2142);   // Service Limiting Override Table
                    listTables.Add(2143);   // Service Limiting Failsafe Duration Table
                    listTables.Add(2149);   // Actual Voltage Monitoring Table
                    listTables.Add(2150);   // Voltage Monitoring Control Table
                    listTables.Add(2153);   // Extended Voltage Monitoring Actual Table
                    listTables.Add(2154);   // Extended Voltage Monitoring Control Table 
                    listTables.Add(2159);   // Actual Communication Log Table
                    listTables.Add(2161);   // LAN Control Table
                    listTables.Add(2163);   // HAN Control Table
                    listTables.Add(2169);   // LED Config table
                    listTables.Add(2185);   // Bell Weather Meter Config Table
                    listTables.Add(2187);   // Bell Weather Enable Table
                    listTables.Add(2190);   // Communications Config Table
                    listTables.Add(2191);   // C12.22 Config Table
                    listTables.Add(2193);   // Security Activation Table
                    listTables.Add(2260);   // SR 3.0 Config Table
                    listTables.Add(2265);   // Non-Metrological Configuration Table
                    listTables.Add(2369);   // Power Monitoring Configuration Table
                    listTables.Add(2425);   // Temperature Configuration Table
                    listTables.Add(2437);   // 25 Year TOU Calendar Table
                }

                if (FactoryConfigType != ConfigurationOptions.ICSRegisterOnly)
                {

                    listTables.Add(2512);   // ICS Module Configuration Table 
                    listTables.Add(2517);   // ICS Cellular Configuration Table
                    listTables.Add(2523);   // ICS Event Log Control Table.
                    listTables.Add(2536);   // ICM Actual Network Table
                    listTables.Add(2537);   // ICM Exception Report Table

                    if (true == IsICSERTDataSupported())
                    {
                        //This table needs to be configured after 2512. Table 2512 has a field that 
                        //determines if this table is supported, so you may not be able to write table 
                        //until after 2512 is configured to support it.
                        listTables.Add(2509);   // ICS ERT Configuraton Table 
                    }
                }

                if (FactoryConfigType != ConfigurationOptions.ICSCommModuleOnly)
                {
                    listTables.Add(6186);   // TOU Pending Table
                    listTables.Add(2048);   // Configuration Table
                }
            }
            catch (TimeOutException e)
            {
                throw (e);
            }
            catch (Exception)
            {
                // If not a timeout, do nothing. We just won't add the ICS ERT config table
            }

            return listTables;
        }

        /// <summary>
        /// This method determines if ICS ERT data is or is going to be supported.
        /// </summary>
        /// <returns>Boolean indicating whether or not ICS ERT data can be written to the meter.</returns>
        // Revison History:
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 08/13/13 jrf 2.85.16 WR 417794 Created
        // 09/10/14 AF  4.00.54 WR 532781 Added exception handling to deal with timeout exceptions
        // 09/26/14 AF  4.00.62 WR 523633 Substituted an offset read of mfg 464 instead of a full table read
        //                                to help with the timeout issue
        //
        protected bool IsICSERTDataSupported()
        {
            bool blnSupported = false;

            if (true == IsTableSupported(2512))
            {
                CentronTables MeterTables = new CentronTables();

                //If we are not specifically configuring the ICS module's ERT data populated field
                //then we need to read the meter in order to determine if the ERT Data is suppported.
                if (FactoryConfigType != ConfigurationOptions.ICSCommModuleOnly || null == IsERTPopulated)
                {
                    PSEMResponse PSEMResult = PSEMResponse.Err;
                    object objValue;
                    byte byIsERTPopulated = 0;

                    uint uiStartOffset;
                    uint uiLength;
                    byte[] abyOffsetData;
                    MemoryStream PSEMStream = null;

                    try
                    {
                        MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl464IsERTPopulated, null, out uiStartOffset, out uiLength);
                        PSEMResult = m_PSEM.OffsetRead(2512, (int)uiStartOffset, (ushort)uiLength, out abyOffsetData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMStream = new MemoryStream(abyOffsetData);
                            MeterTables.SavePSEMStream(2512, PSEMStream, uiStartOffset);

                            MeterTables.GetValue(CentronTblEnum.MfgTbl464IsERTPopulated, null, out objValue);
                            byIsERTPopulated = (byte)objValue;

                            if (1 == byIsERTPopulated)
                            {
                                blnSupported = true;
                            }
                        }
                    }
                    catch (TimeOutException e)
                    {
                        throw (e);
                    }
                    catch (Exception)
                    {
                        blnSupported = false;
                    }
                    finally
                    {
                        if (null != PSEMStream)
                        {
                            PSEMStream.Close();
                        }
                    }
                }
                //Otherwise we are specifically configuring the ICS module's ERT data populated field
                //so we just need to check the value that we are configuring it to.
                else
                {
                    if (1 == IsERTPopulated.Value)
                    {
                        blnSupported = true;
                    }
                }
            }

            return blnSupported;
        }

        /// <summary>
        /// Gets the register FW version from Table 1
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/11/10 RCG 2.40.24 	   Created
        // 06/11/10 AF  2.41.09        Changed the access modifier
        // 07/08/10 AF  2.42.03        Made virtual for use in M2 Gateway

        protected virtual void GetRegFWVersion()
        {
            object objValue;
            byte byVersion;
            byte byRevision;

            if (m_CentronAMITables.IsAllCached(1))
            {
                m_CentronAMITables.GetValue(StdTableEnum.STDTBL1_FW_VERSION_NUMBER, null, out objValue);
                byVersion = (byte)objValue;

                m_CentronAMITables.GetValue(StdTableEnum.STDTBL1_FW_REVISION_NUMBER, null, out objValue);
                byRevision = (byte)objValue;

                m_fFWVersion = byVersion + (byRevision / 1000.0f);
            }
            else
            {
                m_fFWVersion = 0.0f;
            }
        }

        /// <summary>
        /// Determines whether or not a table is critical to the Configuration
        /// </summary>
        /// <param name="usTableID">The table number.</param>
        /// <returns>
        /// True if the table is a critical table.
        /// False otherwise.
        /// </returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        private static bool IsCriticalTable(ushort usTableID)
        {
            switch (usTableID)
            {
                //case 6:
                //case 42:
                //case 46:
                //case 82:
                //case 83:
                //case 122:
                //case 2048:
                //    {
                //        return true;
                //    }
                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Determines if the table is supported by the meter.
        /// </summary>
        /// <param name="TableID">The ID of the table to check.</param>
        /// <returns>True if the table is supported false otherwise.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/08 RCG 2.00.02 122143 Created
        // 06/11/10 AF  2.41.09        Changed the access modifier
        // 07/08/10 AF  2.42.03        Made virtual for use in M2 Gateway

        protected virtual bool IsTableSupported(ushort TableID)
        {
            object objValue = null;
            bool bTableUsed = false;

            try
            {
                if (TableID < 2048)
                {
                    m_CentronAMITables.GetValue(StdTableEnum.STDTBL0_STD_TBLS_USED, new int[] { TableID }, out objValue);
                }
                else
                {
                    m_CentronAMITables.GetValue(StdTableEnum.STDTBL0_MFG_TBLS_USED, new int[] { TableID % 2048 }, out objValue);
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
        /// Determines if the table is an ICS configuration table.
        /// </summary>
        /// <param name="TableID">The ID of the table to check.</param>
        /// <returns>True if the table is supported false otherwise.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/08/13 jrf 2.80.18 TQ7652 Created
        // 06/18/13 jrf 2.80.38 N/A    Made method ICS configuration table specific
        protected bool IsICSConfigTable(ushort TableID)
        {
            bool blnICSConfigTable = false;

            if (ICS_CONFIG_TABLES.Contains(TableID))
            {
                blnICSConfigTable = true;
            }

            return blnICSConfigTable;
        }        

        /// <summary>
        /// Raises a ShowProgressEvent for this control
        /// </summary>
        /// <param name="e">The event arguments</param>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/29/06 RCG 7.40.00 N/A	   Created
        // 06/11/10 AF  2.41.09        Changed the access modifier
        //
        protected void OnShowProgress(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises a StepProgressEvent for this control
        /// </summary>
        /// <param name="e">The event arguments</param>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/29/06 RCG 7.40.00 N/A	   Created
        // 06/11/10 AF  2.41.09        Changed the access modifier
        
        protected void OnStepProgress(ProgressEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises a HideProgressEvent for this control
        /// </summary>
        /// <param name="e">The event arguments</param>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/29/06 RCG 7.40.00 N/A	   Created
        // 06/11/10 AF  2.41.09        Changed the access modifier
        
        protected void OnHideProgress(EventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(this, e);
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// handles reading from and writing to EDL files
        /// </summary>
        protected CentronTables m_CentronAMITables;

        //protected C1219Tables.ANSIStandard.
        /// <summary>
        /// whether or not the meter is Canadian
        /// </summary>
        protected bool m_bIsCanadian;
        /// <summary>
        /// User configurable unit id
        /// </summary>
        protected string m_strUnitID;
        /// <summary>
        /// User configurable customer serial number
        /// </summary>
        protected string m_strCustomerSerialNumber;
        /// <summary>
        /// PSEM object
        /// </summary>
        protected CPSEM m_PSEM;
        /// <summary>
        /// date and time to initialize the meter with
        /// </summary>
        protected DateTime? m_InitialDateTime;
        private bool m_bIsFactoryConfig;
        private float m_fFWVersion;
        private bool m_blnActivateICSConfiguration = false;
        private IPAddress m_GatewayAddress = null;
        private ushort? m_usERTUtilityID = null;
        private byte? m_byIsERTPopulated = null;
        private ConfigurationOptions m_ConfigType = ConfigurationOptions.Full;

        #endregion

    }
}
