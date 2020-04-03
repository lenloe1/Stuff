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
//                           Copyright © 2006 - 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Progressable;
using Itron.Common.C1219Tables.ANSIStandardII;
using Itron.Common.C1219Tables.CentronII;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.AMIConfiguration
{
    /// <summary>
    /// Handles the configuration of an AMI meter.
    /// </summary>
    public class AMIConfigureCentronII : IProgressable
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
        /// TOU Reconfiguration
        /// </summary>
        protected const uint TOU_RECONFIG = 0x00004000;

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

        ///// <summary>
        ///// Table 2048 Calendar, TOU, Schedules data buffer
        ///// Please refer timemain.h in Register_MCU
        ///// Calendar structure - header 6 bytes + 25 year calendar (89 bytes per year) Total 2231 bytes
        ///// <summary>
        private const int CALENDAR_SIZE = 0x08B7; // 2231 bytes

        ///// <summary>
        ///// TOU structure -  8 seasons (195 bytes per season) Total 1560 bytes
        ///// <summary>
        private const int TOU_SIZE = 0x0618; // 1560 bytes

        ///// <summary>
        ///// Bill Schedule structure -  25 years (12 events [2 bytes] per year)  Total 600 bytes
        ///// <summary>
        private const int SCHEDULE_SIZE = 0x0258; // 600 bytes

        ///// <summary>
        ///// DST Calendar structure -  25 years Total 128 bytes
        ///// <summary>
        private const int DST_SIZE = 0x080; // 128 bytes
        
        ///// <summary>
        ///// Flag for Table 2090 if been loaded
        ///// <summary>
        private bool Table2090Loaded = false; 

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

        public AMIConfigureCentronII(CPSEM PSEMObject)
        {
            m_PSEM = PSEMObject;
            m_CentronTables  = new CentronTables ();
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
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 09/07/06 RCG 7.40.00 N/A    Added SetClock before configuring so that the
        //                             clock is not the reference date on configuration
        //                             of a 3 button reset meter.
        // 03/27/08 RCG 1.50.11        Removing prompt for dialog.
        // 07/08/10 AF  2.42.03        Made virtual for use in M2 Gateway
        // 09/09/10 jrf 2.44.00 158657 Removing the second set clock after writing the 
        //                             configuration.

        public virtual ConfigurationError Configure(string strEDLFileName)
        {
            FileStream EDLFileStream;
            XmlTextReader EDLXMLReader;
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;
            DateTime dtTOUStartTime;
            DateTime dtTOUNextSeasonStartTime;
            bool bTOUDemandReset;
            bool bTOUSelfRead;

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
                        m_CentronTables.LoadEDLFile(EDLXMLReader);
                    }
                    catch (ArgumentNullException e)
                    {
                        System.Diagnostics.Debug.Write(e.Message);
                    }
                    catch (Exception)
                    {
                        ConfigError = ConfigurationError.INVALID_PROGRAM;
                    }
                }

                // Retrieve data buffers for Calendar, TOU, and Schedules from Table 2090
                LoadTOUConfiguration();

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

                    try
                    {
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

                        // Update the TOU schedule to use the current season.
                        m_CentronTables .UpdateTOUSeasonFromStandardTables(TOUConfigTime, 0,
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
                    // Set the clock so that 3 button reset meters
                    // do not have events in 1970
                    ConfigError = SetClock();
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
                    // Clear the status information
                    ConfigError = ClearStatusFlags();
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
        /// Partial reconfigures the meter with the specified TOU dataset.
        /// </summary>
        /// <param name="TOUDataSet">TOU Data Set.</param>
        /// <param name="CalendarOffset">Calendar Offset.</param>
        /// <param name="TOUOffset">TOU Offset.</param>
        /// /// <returns>bool code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/05/10 SCW 9.70.14        Created.

        public virtual bool ReconfigureTOU(TableData[] TOUDataSet, UInt16 CalendarOffset, UInt16 TOUOffset)
        {
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;
            PSEMResponse PSEMResult = new PSEMResponse();
            PSEMResult = PSEMResponse.Ok;
           
            // Step 1: Allocate  
            if (TOUDataSet.Length > 0)
            {
                List<byte> TOUDataBuffer = new List<byte>();
                MemoryStream PSEMDataStream;

                // Write the stream to the meter
                foreach (TableData CurrentTableData in TOUDataSet)
                {
                    PSEMDataStream = (MemoryStream)CurrentTableData.PSEM;
                    TOUDataBuffer.AddRange(PSEMDataStream.ToArray());
                }
                TOUDataBuffer.CopyTo(0, CalendarBuffer, 0, CALENDAR_SIZE);
                TOUDataBuffer.CopyTo(CALENDAR_SIZE, TOUBuffer, 0, TOU_SIZE);
            }

            // Step 2: Set the clock so that 3 button reset meters do not have events in 1970
            ConfigError = SetClock();
            if (ConfigError == ConfigurationError.SUCCESS)
            {
                // Step 3 Open 2048 for writing
                ConfigError = OpenConfiguration();
              
                // Step 4 Write Calendar and TOU
                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    try
                    {
                        PSEMResult = m_PSEM.OffsetWrite(2048, CalendarOffset, CalendarBuffer);
                        System.Threading.Thread.Sleep(200);
                        m_PSEM.Wait(255);

                        PSEMResult = m_PSEM.OffsetWrite(2048, TOUOffset, TOUBuffer);
                        System.Threading.Thread.Sleep(200);
                        m_PSEM.Wait(255);
                    }
                    catch (TimeOutException)
                    {
                        throw new ArgumentException("Failure in reconfiguring Calendar and TOU !!");
                    }

                    ConfigError = InterpretPSEMResult(PSEMResult);
                }

                // Step 5 Close 2048 for TOU Reconfiguration
                if (ConfigError == ConfigurationError.SUCCESS)
                {

                    ConfigError = CloseTOUReconfiguration();
                }
            }
            return ((ConfigError == ConfigurationError.SUCCESS) ? true : false);
        }

        /// <summary>
        /// Partial reconfigures the meter with the specified DST dataset.
        /// </summary>
        /// <param name="DSTDataSet">TOU Data Set.</param>
        /// /// <returns>bool code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/05/10 SCW 9.70.14        Created.

        public virtual bool ReconfigureDST(TableData[] DSTDataSet)
        {
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;
            PSEMResponse PSEMResult = new PSEMResponse();
            PSEMResult = PSEMResponse.Ok;

            // Step 1: Allocate  
            if (DSTDataSet.Length > 0)
            {
                List<byte> DSTDataBuffer = new List<byte>();
                MemoryStream PSEMDataStream;

                // Write the stream to the meter
                foreach (TableData CurrentTableData in DSTDataSet)
                {
                    PSEMDataStream = (MemoryStream)CurrentTableData.PSEM;
                    DSTDataBuffer.AddRange(PSEMDataStream.ToArray());
                }
                DSTDataBuffer.CopyTo(16, DSTBuffer, 0, DST_SIZE);
            }

            try
            {
                PSEMResult = m_PSEM.OffsetWrite(2260, 16, DSTBuffer);
                System.Threading.Thread.Sleep(200);
                m_PSEM.Wait(255);
            }
            catch (TimeOutException)
            {
                throw new ArgumentException("Failure in reconfiguring DST !!");
            }
            ConfigError = InterpretPSEMResult(PSEMResult);

            return ((ConfigError == ConfigurationError.SUCCESS) ? true : false);
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

        public virtual ConfigurationError WriteTable(ushort usTableID)
        {
            TableData[] AllTableData;
            MemoryStream PSEMDataStream;
            PSEMResponse PSEMResult = new PSEMResponse();

            PSEMResult = PSEMResponse.Ok;
           
            // Get the PSEM streams
            AllTableData = m_CentronTables.BuildPSEMStreams(usTableID);
            
            if (AllTableData.Length > 0)
            {
                long currPosition = 0;
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
                                PSEMDataStream = (MemoryStream) CurrentTableData.PSEM;
                                PSEMResult = m_PSEM.FullWrite(usTableID, PSEMDataStream.ToArray());
                           }
                           else
                           {
                               // Write TOU Configurations - Not: However the sequences of writings in Table 2048 
                               // have to be in order; so we have to locate the proper slot to write TOU configuration
                               if ((0x0800 == usTableID) && Table2090Loaded)
                               {
                                   // Meter needs some time to process the incoming Calendar data packets for each
                                   if (CurrentTableData.Offset > CalendarOffset)
                                   {
                                       PSEMResult = m_PSEM.OffsetWrite(usTableID, CalendarOffset, CalendarBuffer);
                                       System.Threading.Thread.Sleep(200);
                                       m_PSEM.Wait(255);
                           
                                       PSEMResult = m_PSEM.OffsetWrite(usTableID, TOUOffset, TOUBuffer);
                                       System.Threading.Thread.Sleep(200);
                                       m_PSEM.Wait(255);
                                     
                                       currPosition = (CalendarOffset + CALENDAR_SIZE + TOU_SIZE);
                                   }
                               }

                               // Perform an offset write
                               if (0x0800 == usTableID)
                               {
                                   if (CurrentTableData.Offset >= currPosition)
                                   {
                                       PSEMDataStream = (MemoryStream) CurrentTableData.PSEM;
                                       PSEMResult = m_PSEM.OffsetWrite(usTableID, (int) CurrentTableData.Offset, PSEMDataStream.ToArray());
                                       currPosition = (CurrentTableData.Offset + (PSEMDataStream.ToArray().Length));
                                   }
                               }
                               else
                               {
                                   PSEMDataStream = (MemoryStream) CurrentTableData.PSEM;
                                   PSEMResult = m_PSEM.OffsetWrite(usTableID, (int) CurrentTableData.Offset, PSEMDataStream.ToArray());
                               }
                           }
                        }
                        catch (TimeOutException)
                        {
                            return ConfigurationError.TIMEOUT;
                        }
                    }
                }

                // Make sure to load Billing Schedule in the last step
                if ((0x0800 == usTableID) && Table2090Loaded)
                {
                    // Meter needs some time to process the incoming Billing Schedule data packets for each
                    if (SchedulesOffset > currPosition)
                    {
                        PSEMResult = m_PSEM.OffsetWrite(usTableID, SchedulesOffset, SchedulesBuffer);
                        System.Threading.Thread.Sleep(200);
                        m_PSEM.Wait(255);
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
            MemoryStream PSEMDataStream;
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
                PSEMDataStream = new MemoryStream(byaData);
                m_CentronTables.SavePSEMStream(usTableID, PSEMDataStream);
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

        #region Protected Methods

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
                m_CentronTables.SetValue(StdTableEnum.STDTBL6_DEVICE_ID, null, m_strUnitID);

                // Load the Customer Serial Number
                m_CentronTables.SetValue(StdTableEnum.STDTBL6_UTIL_SER_NO, null, m_strCustomerSerialNumber);
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

            m_CentronTables.SetupProcedureRequest((ushort)ANSIProcedures.SET_DATE_TIME);

            // Set up the Set Mask bit field
            m_CentronTables.SetValue(StdTableEnum.STDTBL7_PROC10_SET_MASK, null, SET_MASK);

            // Set up the Time Date Qual bit field

            // The DayOfWeek Type directly corresponds to the day values in the meter 0 = Sunday, 1 = Moday, ...
            m_CentronTables.SetValue(StdTableEnum.STDTBL7_PROC10_DAY_OF_WEEK, null, (byte)dtCurrentTime.DayOfWeek);
            m_CentronTables.SetValue(StdTableEnum.STDTBL7_PROC10_DST_FLAG, null, false);
            m_CentronTables.SetValue(StdTableEnum.STDTBL7_PROC10_GMT_FLAG, null, true);
            m_CentronTables.SetValue(StdTableEnum.STDTBL7_PROC10_TM_ZN_APPLIED_FLAG, null, false);
            m_CentronTables.SetValue(StdTableEnum.STDTBL7_PROC10_DST_APPLIED_FLAG, null, false);

            // Set the time
            m_CentronTables.SetValue(StdTableEnum.STDTBL7_PROC10_DATE_TIME, null, dtCurrentTime);

            // Call the Procedure
            ConfigError = WriteTable(7);

            if (ConfigurationError.SUCCESS == ConfigError)
            {
                // Check the Procedure results
                ConfigError = ReadTable(8);
                if (ConfigurationError.SUCCESS == ConfigError)
                {
                    m_CentronTables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
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
            m_CentronTables.ClearTable(7);
            m_CentronTables.ClearTable(8);

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

        protected virtual ConfigurationError CloseConfiguration()
        {
            ConfigurationError ConfigError;
            DateTime dtStartTime;
            TimeSpan tsSpan;
            object objValue;
            byte byReturnCode = (byte)ProcedureResultCodes.NOT_FULLY_COMPLETED;

            // Set up table 7 and 8 for MFG Procedure 2
            m_CentronTables.SetupProcedureRequest(2050);

            if (m_bIsCanadian == true)
            {
                // Make the meter Canadian and close the configuration
                m_CentronTables.SetValue(CentronTblEnum.PROCRQST02_CLOSE_REQUEST, null, COMPLETE_CANADIAN_CONFIG);
            }
            else
            {
                // Close the configuration
                m_CentronTables.SetValue(CentronTblEnum.PROCRQST02_CLOSE_REQUEST, null, COMPLETE_CONFIG);
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
                    System.Threading.Thread.Sleep(700);

                    ConfigError = ReadTable(8);
                    m_PSEM.Wait(255);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        m_CentronTables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
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
        /// Close 2048 for TOU Reconfiguration
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 06/14/10 AF  2.41.09        Changed the access modifier and mad

        protected virtual ConfigurationError CloseTOUReconfiguration()
        {
            ConfigurationError ConfigError;
            DateTime dtStartTime;
            TimeSpan tsSpan;
            object objValue;
            byte byReturnCode = (byte)ProcedureResultCodes.NOT_FULLY_COMPLETED;

            // Set up table 7 and 8 for MFG Procedure 2
            m_CentronTables.SetupProcedureRequest(2050);

            if (m_bIsCanadian == true)
            {
                // Make the meter Canadian and close the configuration
                m_CentronTables.SetValue(CentronTblEnum.PROCRQST02_CLOSE_REQUEST, null, COMPLETE_CANADIAN_CONFIG);
            }
            else
            {
                // Close the configuration
                m_CentronTables.SetValue(CentronTblEnum.PROCRQST02_CLOSE_REQUEST, null, TOU_RECONFIG);
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
                    System.Threading.Thread.Sleep(700);

                    ConfigError = ReadTable(8);
                    m_PSEM.Wait(255);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        m_CentronTables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
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
			m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VENDOR, null, VENDOR_NAME);
			m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VENDOR, null, VENDOR_NAME);

			 // Add the software version information
            m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.ProductMajorPart);
            m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.ProductMajorPart);
            m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.ProductMinorPart);
            m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.ProductMinorPart);
            m_CentronTables.SetValue(CentronTblEnum.MFGTBL0_SW_VERSION, null, (byte)AssemblyVersion.ProductMajorPart);
            m_CentronTables.SetValue(CentronTblEnum.MFGTBL0_SW_REVISION, null, (byte)AssemblyVersion.ProductMinorPart);
#else
			//Get the assembly verion for CE
			Assembly assm = Assembly.GetExecutingAssembly();
			Version AssemblyVersion = assm.GetName().Version;

			// Add the software Vendor information
			m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VENDOR, null, VENDOR_NAME);
			m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VENDOR, null, VENDOR_NAME);

			// Add the software version information
			m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.Major);
			m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_VERSION_NUMBER, null, (byte)AssemblyVersion.Major);
			m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX1_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.Minor);
			m_CentronTables.SetValue(StdTableEnum.STDTBL6_EX2_SW_REVISION_NUMBER, null, (byte)AssemblyVersion.Minor);
			m_CentronTables.SetValue(CentronTblEnum.MFGTBL0_SW_VERSION, null, (byte)AssemblyVersion.Major);
			m_CentronTables.SetValue(CentronTblEnum.MFGTBL0_SW_REVISION, null, (byte)AssemblyVersion.Minor);
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

            m_CentronTables.SetValue(CentronTblEnum.MFGTBL0_CONFIG_TIME, null, (uint)Span.TotalSeconds);
        }
       
        /// <summary>
        /// Writes the configuration tables to the meter
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
		// 03/13/07 mrj 8.00.18		   Removed wait, keep alive is now used.
        // 07/08/10 AF  2.42.03        Made virtual for use in M2 Gateway

        protected virtual ConfigurationError WriteConfiguration()
        {
            List<ushort> listConfigurationTables;
            ConfigurationError ConfigErrorCode = ConfigurationError.SUCCESS;
            
            // Get the list of tables to configure
            listConfigurationTables = GetTablesToConfigure();

            OnShowProgress(new ShowProgressEventArgs(1, listConfigurationTables.Count, "Device Configuration", "Writing Configuration..."));

            // Write each of the configuration tables
            foreach (ushort usTableID in listConfigurationTables)
            {
                bool isConfigurable = ((ConfigErrorCode == ConfigurationError.SUCCESS && m_CentronTables.IsTableKnown(usTableID) &&
                                      IsTableSupported(usTableID)));
                if ( isConfigurable )
                {
                    if ((usTableID == 2048))
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
                        if (usTableID == 2048) 
                        {
                        
                        }
                        ConfigErrorCode = WriteTable(usTableID);
                    }

                    if (ConfigErrorCode == ConfigurationError.ITEM_NOT_FOUND && !IsCriticalTable(usTableID))
                    {
                        // This is not a critical table so it does not have to be written in order for
                        // configuration to succeed
                        ConfigErrorCode = ConfigurationError.SUCCESS;
                    }

                    if ((usTableID == 2048) && (ConfigErrorCode == ConfigurationError.SUCCESS))
                    {
                        // Close 2048
                     
                        ConfigErrorCode = CloseConfiguration();
                    }
                }

                if (ConfigErrorCode == ConfigurationError.INNAPROPRIATE_ACTION_REQUEST)
                {
                    ushort TabID = usTableID;
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
        // 06/01/06 RCG 7.40.00 N/A	   Created
        // 07/08/10 AF  2.42.03        Made virtual for use in M2 Gateway

        protected virtual List<ushort> GetTablesToConfigure()
        {
            List<ushort> listTables = new List<ushort>();

            // Manually add the tables in the order that they will be configured.
            listTables.Add(6);      // Utility Information Table
            listTables.Add(42);     // Security Table
            listTables.Add(46);     // Key Table
            listTables.Add(82);     // User Defined Tables List Table
            listTables.Add(83);     // User Definded Tables Selection Table

            // Prior to the SP5.1 there was an issue that could cause read rate problems if the exceptions
            // are configured by the factory so we should not configure these items prior to SP5.1
            if (m_CentronTables.IsAllCached(123) && VersionChecker.CompareTo(m_fFWVersion, 2.006f) >= 0)
            {
                listTables.Add(121);    // Actual Network Table
                listTables.Add(123);    // Exception Report Table
            }

            // listTables.Add(2090);   // TOU Table
            // listTables.Add(2106);   // HAN Config Parameters
            // listTables.Add(2141);   // Service Limiting Config Table
            // listTables.Add(2142);   // Service Limiting Override Table
            // listTables.Add(2143);   // Service Limiting Failsafe Duration Table
            listTables.Add(2149);   // Actual Voltage Monitoring Table
            listTables.Add(2150);   // Voltage Monitoring Table
            // listTables.Add(2159);   // Actual Communication Log Table
            // listTables.Add(2161);   // LAN Control Table
            // listTables.Add(2163);   // HAN Control Table
            // listTables.Add(2169);   // LED Config table - only for Poly
            listTables.Add(2190);   // Communications Config Table
            listTables.Add(2193);   // Security Activation Table
            listTables.Add(2260);   // SR 3.0 Config Table
            listTables.Add(2048);   // Configuration Table
            // listTables.Add(2090);   // TOU Table
            // listTables.Add(6186);   // TOU Pending Table

            return listTables;
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

            if (m_CentronTables.IsAllCached(1))
            {
                m_CentronTables.GetValue(StdTableEnum.STDTBL1_FW_VERSION_NUMBER, null, out objValue);
                byVersion = (byte)objValue;

                m_CentronTables.GetValue(StdTableEnum.STDTBL1_FW_REVISION_NUMBER, null, out objValue);
                byRevision = (byte)objValue;

                m_fFWVersion = byVersion + (byRevision / 1000.0f);
            }
            else
            {
                m_fFWVersion = 0.0f;
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
                    m_CentronTables.GetValue(StdTableEnum.STDTBL0_STD_TBLS_USED, new int[] { TableID }, out objValue);
                }
                else
                {
                    m_CentronTables.GetValue(StdTableEnum.STDTBL0_MFG_TBLS_USED, new int[] { TableID % 2048 }, out objValue);
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
        /// Raises a ShowProgressEvent for this control
        /// </summary>
        /// <param name="e">The event arguments</param>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/29/06 RCG 7.40.00 N/A	   Created
        // 06/11/10 AF  2.41.09        Changed the access modifier
        
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

        /// <summary>
        /// Raises a HideProgressEvent for this control
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        
        protected void LoadTOUConfiguration()
        {
            // Get the PSEM streams
            TableData[] AllTableData = m_CentronTables.BuildPSEMStreams(2090);
            MemoryStream PSEMDataStream;

            if (AllTableData.Length > 0)
            {
                List<byte> AllTableDataBuffer = new List<byte>();

                // Write the stream to the meter
                foreach (TableData CurrentTableData in AllTableData)
                {
                    PSEMDataStream = (MemoryStream)CurrentTableData.PSEM;
                    AllTableDataBuffer.AddRange(PSEMDataStream.ToArray());
                }
                AllTableDataBuffer.CopyTo(0, CalendarBuffer, 0, CALENDAR_SIZE);
                AllTableDataBuffer.CopyTo(CALENDAR_SIZE, TOUBuffer, 0, TOU_SIZE);
                AllTableDataBuffer.CopyTo((CALENDAR_SIZE+TOU_SIZE), SchedulesBuffer, 0, SCHEDULE_SIZE);
                Table2090Loaded = true;
            }
            else
            {
                Table2090Loaded = false;
            }
        }

        #endregion

        #region Private Methods

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

            // Get the location of the Offsets
            m_CentronTables.GetFieldOffset((long)CentronTblEnum.MFGTBL0_SUBTABLE_OFFSETS, null, out uiStartOffset, out uiLength);

            abyOffsetData = new byte[uiLength];

            // Read the offsets from the meter
            PSEMResponse = m_PSEM.OffsetRead(2048, (int)uiStartOffset, (ushort)uiLength, out abyOffsetData);

            if (PSEMResponse == PSEMResponse.Ok)
            {
                // Now give the meter tables the correct offsets
                m_CentronTables.SavePSEMStream(2048, new MemoryStream(abyOffsetData), uiStartOffset);

                // Retrieve offsets of Calendar, TOU, and Schedules
                CalendarOffset = BitConverter.ToInt16(abyOffsetData, 26);
                TOUOffset = BitConverter.ToInt16(abyOffsetData, 28);
                SchedulesOffset = BitConverter.ToInt16(abyOffsetData, 42);
            }

            return InterpretPSEMResult(PSEMResponse);
        }

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
            if (!m_CentronTables.IsCached((long)CentronTblEnum.MfgTbl145C1218OverZigBee, null))
            {
                m_CentronTables.SetValue((long)CentronTblEnum.MfgTbl145C1218OverZigBee, null, 0);
            }
        }

        /// <summary>
        /// Calls the procedure to clear Standard and Manufacturer status flags
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/30/06 RCG 8.00.00 N/A	   Created

        private ConfigurationError ClearStatusFlags()
        {
            ConfigurationError ConfigError;
            object objValue;

            m_CentronTables.ClearTable(7);

            // Set up table 7 and 8 for Standard Procedure 7 to reset the Standard Status Flags
            m_CentronTables.SetupProcedureRequest(7);

            // Std Procedure 3 does not take any parameters so call the procedure
            ConfigError = WriteTable(7);

            if (ConfigError == ConfigurationError.SUCCESS)
            {
                // Check the Procedure results
                ConfigError = ReadTable(8);

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    m_CentronTables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
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
                m_CentronTables.ClearTable(7);

                // Set up table 7 and 8 for Standard Procedure 8 to reset the Manufacturer Status Flags
                m_CentronTables.SetupProcedureRequest(8);

                // Std Procedure 3 does not take any parameters so call the procedure
                ConfigError = WriteTable(7);

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    // Check the Procedure results
                    ConfigError = ReadTable(8);

                    if (ConfigError == ConfigurationError.SUCCESS)
                    {
                        m_CentronTables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
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

        private ConfigurationError OpenConfiguration()
        {
            ConfigurationError ConfigError;
            object objValue;

            // Set up table 7 and 8 for MFG Procedure 1
            m_CentronTables.SetupProcedureRequest(2049);

            // MFG Procedure 1 does not take any parameters so call the procedure
            ConfigError = WriteTable(7);

            if (ConfigError == ConfigurationError.SUCCESS)
            {
                // Check the Procedure results
                ConfigError = ReadTable(8);

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    m_CentronTables.GetValue(StdTableEnum.STDTBL8_RESULT_CODE, null, out objValue);
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

        #endregion

        #region Member Variables

        /// <summary>
        /// handles reading from and writing to EDL files
        /// </summary>
        protected CentronTables  m_CentronTables ;

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

        // Table 2048 Calendar, TOU, Schedules Offsets
        private int CalendarOffset;
        private int TOUOffset;
        private int SchedulesOffset;

        // Table 2048 Calendar, TOU, Schedules data buffer
        private byte [] CalendarBuffer = new byte[CALENDAR_SIZE];
        private byte [] TOUBuffer = new byte[TOU_SIZE];
        private byte [] SchedulesBuffer = new byte[SCHEDULE_SIZE];
        private byte [] DSTBuffer = new byte[DST_SIZE];

        #endregion

    }
}
