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
//                              Copyright © 2013 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the ICS Gateway
    /// </summary>
    public abstract partial class ICS_Gateway : CANSIDevice
    {
        #region Constants

        /// <summary>
        /// Standard table 01 Model
        /// </summary>
        public const string ICS_MODEL = "ICSGEN2";
        /// <summary>
        /// 
        /// </summary>
        public const string ICS_DEVICE_CLASS = "ITRH";
        /// <summary>
        /// 
        /// </summary>
        private const string GATEWAY_NAME = "OpenWay ICS Gateway";
        /// <summary>
        /// 
        /// </summary>
        private const string LOG_ITR1_NAME = "OW ICS Gateway";
        /// <summary>
        /// 
        /// </summary>
        private const int MAX_NUM_ERRORS = 1;

        /// <summary>
        /// ICS comm module restricts the offset read size.
        /// </summary>
        private const ushort ICS_MAX_OFFSET_READ_SIZE = 1400;

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 03/18/13 AF  2.80.08 TR 7578   Created
        // 01/09/14 DLG 3.50.23 TR 9993   Also related to TREQ 9996. Added argument to new parameter
        //                                for ICSTableReader when it's instantiated.
        // 
        public ICS_Gateway(CPSEM PSEM)
            : base(PSEM)
        {
            m_EventDictionary = new ICS_Gateway_EventDictionary();
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                        this.GetType().Assembly);
            m_ICSTableReader = new ICSTableReader(PSEM, this);
        }

        /// <summary>
        /// Convert a utc time from the meter to local time for the device
        /// </summary>
        /// <param name="utcTime">UDT time from the meter</param>
        /// <returns>Convertered Device Local Time</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue#         Description
        //  -------- --- ------- -------------  ---------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created
        //
        public override DateTime GetLocalDeviceTime(DateTime utcTime)
        {
            DateTime LocalDateTime = utcTime;

            // We can only do these functions in the 3.5 framework, which currently does not work in CE.
#if (!WindowsCE)

            //We do not need to adjust the time for Version 1.5
            LocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, DeviceTimeZoneInfo);
#else
            LocalDateTime = utcTime;
#endif

            return LocalDateTime;
        }

        /// <summary>
        /// Calls standard procedure 2 - save configuration
        /// </summary>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //
        public ProcedureResultCodes SaveConfiguration()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[0];

            ProcResult = ExecuteProcedure(Procedures.SAVE_CONFIGURATION, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// ITRU and ITRV do not support pending tables.  We need this at present for firmware 
        /// download to be enabled. This method is defined here only for use in Device Restrictions.
        /// Restrictions.
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/08/13 AF  3.50.02 TQ9508,9514 Created
        //
        public bool FWTableStillPendingExists()
        {
            return false;
        }

        /// <summary>
        /// Gets the firmware revision. This is the same as "FWRevision" but was created for the 
        /// purpose of using as a parameter to pass in on table creation. Some Tables are different
        /// sizes in different firmware versions. This should only be used as a parameter to pass
        /// in when creating a table. This allows us to override only a property and not the
        /// creation of a table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/16/13 DLG 3.50.16          Created.
        //  
        public override float FWRevisionForTableCreation
        {
            get
            {
                return CENTRON_AMI.VERSION_3;
            }
        }

        /// <summary>
        /// Gets the hardware revision. This is the same as "HWRevision" but was created for the 
        /// purpose of using as a parameter to pass in on table creation. Some Tables are different
        /// sizes in different hardware versions. This should only be used as a parameter to pass
        /// in when creating a table. Reference table 2128 for example.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //  
        public override float HWRevisionForTableCreation
        {
            get
            {
                return CENTRON_AMI.HW_VERSION_3_0;
            }
        }

        /// <summary>
        /// Creates a list of tables to read from the meter.
        /// </summary>
        /// <param name="IncludedSections">EDL Sections to include</param>
        /// <returns>The list of tables to read.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 11/21/13 DLG 3.50.07           Overriding here to add ICS related tables.
        // 01/15/14 DLG 3.50.25           Added table 2529.
        // 03/14/14 AF  3.50.49 WR 464163 Call SetUpForICMEvents instead of Events to prevent the tables from being read twice.
        // 05/12/14 AF  3.50.92 WR 503772 Removed table 2529 from the list. It's not supported by the CE dll so no point in adding it.
        // 
        protected override List<ushort> GetTablesToRead(EDLSections IncludedSections)
        {
            List<ushort> TableList = base.GetTablesToRead(IncludedSections);
            DateTime startTime = new DateTime(2000, 1, 1);
            byte byDataRecordCount = 0;
            byte byStatRecordCount = 0;
            byte byDataRecordSize = 0;

            ICSCommModule ICSModule = CommModule as ICSCommModule;

            if (ICSModule != null)
            {
                TableList.Add(2510);            // ICS ERT Dimension Table                 
                TableList.Add(2509);            // ICS ERT Configuration Table   

                try
                {
                    // Send the command to the meter to update the ERT data tables.
                    ICSModule.UpdateERTDataTables(out byDataRecordCount, out byStatRecordCount, out byDataRecordSize);
                }
                catch
                {
                    //Something went wrong. Make sure counts are zero.
                    byDataRecordCount = 0;
                    byStatRecordCount = 0;
                    byDataRecordSize = 0;
                }

                //Only add the tables if procedure indicates that there is data in them.
                if (0 < byDataRecordCount)
                {
                    TableList.Add(2508);        // ICS ERT Data Table
                }

                if (0 < byStatRecordCount)
                {
                    TableList.Add(2511);        // ICS ERT Statistics Table
                }

                if ((IncludedSections & EDLSections.HistoryLog) == EDLSections.HistoryLog)
                {
                    if (ProcedureResultCodes.COMPLETED == SetUpForICMEvents())
                    {
                        TableList.Add(2521);    // ICS Events Actual
                        TableList.Add(2522);    // ICS Events ID
                        TableList.Add(2523);    // ICS Events Log Control
                        TableList.Add(2524);    // ICS Log Data Table
                    }
                }

                TableList.Add(2512);            // ICS Module Configuration Table
                TableList.Add(2515);            // ICS Module Data Table
                TableList.Add(2516);            // ICS Module Status Table
                TableList.Add(2517);            // ICS Cellular Configuration Table
                TableList.Add(2518);            // ICS Cellular Data Table
                TableList.Add(2519);            // ICS Cellular Status Table
            }

            return TableList;
        }

        /// <summary>
        /// Handles the special case table reads
        /// </summary>
        /// <param name="usTableID">id of the table to read</param>
        /// <param name="MeterTables">reference to the CE dll object</param>
        /// <returns>the PSEM response to the read attempt</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/21/13 DLG 3.50.07 No WR    Overriding here to handle ICS related tables.
        //  12/18/13 AF  3.50.16 WR444069 Removed the call to the base class ReadTable.  It was causing a full
        //                                read of table 2524, which is not supported.
        //  05/08/14 AF  3.50.91 WR503773 Added cases for tables 2242 and 2243
        //
        public override PSEMResponse ReadTable(ushort usTableID, ref CentronTables MeterTables)
        {
            MemoryStream PSEMDataStream;
            PSEMResponse PSEMResult = PSEMResponse.Ok;           
            byte[] byaData;
            int iReadAttempt = 0;
            bool bRetry = true;

            while (bRetry)
            {
                switch (usTableID)
                {
                    case 2242:
                    {
                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMResult = ReadTable2242(ref MeterTables, ICS_MAX_OFFSET_READ_SIZE);
                        }
                        break;
                    }
                    case 2243:
                    {
                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMResult = ReadTable2243(ref MeterTables, ICS_MAX_OFFSET_READ_SIZE);
                        }

                        break;
                    }
                    case 2508:
                    {
                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMResult = m_ICSTableReader.ReadTable2508(ref MeterTables);
                        }

                        break;
                    }
                    case 2511:
                    {
                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMResult = m_ICSTableReader.ReadTable2511(ref MeterTables);
                        }

                        break;
                    }
                    case 2524:
                    {
                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMResult = m_ICSTableReader.ReadTable2524(ref MeterTables);
                        }

                        break;
                    }
                    default:
                    {
                        PSEMResult = m_PSEM.FullRead(usTableID, out byaData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(byaData, 0, byaData.Length, true, true);
                            MeterTables.SavePSEMStream(usTableID, PSEMDataStream);
                        }

                        break;
                    }
                }

                iReadAttempt++;

                if (iReadAttempt < 3 && (PSEMResult == PSEMResponse.Bsy || PSEMResult == PSEMResponse.Dnr))
                {
                    bRetry = true;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    bRetry = false;
                }
            }

            return PSEMResult;
        }

        /// <summary>
        /// Calls the procedures needed to populate the ICM event tables (2521 and 2524).  Does not read the tables.
        /// </summary>
        /// <returns>The result of the procedure calls</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  03/14/14 AF  3.50.49 WR 464163 Created so that we can populate the event table without reading it
        //
        public ProcedureResultCodes SetUpForICMEvents()
        {
            ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
            ICSCommModule ICSModule = CommModule as ICSCommModule;
            byte[] commandResponse;

            if (ICSModule != null)
            {
                if (ICSModule.FilterICSEvents() == ProcedureResultCodes.COMPLETED)
                {
                    Result = ICSModule.UpdateEventTables(new DateTime(1970, 1, 1), DateTime.MaxValue, out commandResponse);
                }
            }

            return Result;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets whether or not the meter is currently in Fatal Error Recovery Mode. This method is 
        /// defined here only for use in Device Restrictions.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //
        public bool IsInFatalErrorRecoveryMode
        {
            get
            {
                // Not supported by the ICS Gateway
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the meter currently has a Full Core Dump available
        /// and further core dumps are blocked. This method is defined here only for use in Device 
        /// Restrictions.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //
        public bool IsFullCoreDumpBlocked
        {
            get
            {
                // Not supported by the ICS Gateway
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not Fatal Error Recovery is enabled in the meter. This method is defined 
        /// here only for use in Device Restrictions.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //
        public bool IsFatalErrorRecoveryEnabled
        {
            get
            {
                // Not supported by the ICS Gateway
                return false;
            }
        }

        /// <summary>
        /// Gets the list of fatal errors in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //
        public override string[] ErrorsList
        {
            get
            {
                string[] strErrorList = new string[MAX_NUM_ERRORS];
                int iErrorCount = 0;

                //TODO - create the error list if supported
                
                //Create the return list
                string[] strReturnList = new string[iErrorCount];
                if (iErrorCount > 0)
                {
                    Array.Copy(strErrorList, 0, strReturnList, 0, iErrorCount);
                }

                return strReturnList;
            }
        }

        /// <summary>
        /// Returns the Customer Serial Number from Table 6 rather than 2048.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  03/26/13 AF  2.80.08 TR7578 Created
        //  12/12/13 AF  3.50.14        Class re-architecture - read from std table 06
        //
        public override string SerialNumber
        {
            get
            {
                return Table06.UtilitySerialNumber;
            }
        }

        /// <summary>
        /// Property to determine if Magnetic Tampers should be supported. This method is defined 
        /// here only for use in Device Restrictions.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //
        public bool MagneticTampersSupported
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Boolean that indicates if any Communication Module is present in the device.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //  12/09/13 AF  3.50.14 Class re-architecture.  Added override after
        //                       CommModulePresent moved to CANSIDevice
        //
        public override bool CommModulePresent
        {
            get
            {
                bool bResult = false;

                try
                {
                    bResult = CommModule != null;
                }
                catch (Exception)
                {
                    bResult = false;
                }

                return bResult;
            }
        }

        /// <summary>
        /// Returns the string that represents the Comm module device class
        /// Allows us to be able to distinguish ITRL devices.  Overridden in
        /// this device class to avoid a bug fix for HW 1.5 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //
        public override string CommModuleDeviceClass
        {
            get
            {
                string strDeviceClass = "";
                
                if (Table00 != null)
                {
                    strDeviceClass = Table00.DeviceClass;
                }

                return strDeviceClass;
            }
        }

        /// <summary>
        /// Gets the dst enabled flag
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created
        //		
        public override bool DSTEnabled
        {
            get
            {
                return (bool)IsDSTApplied;
            }
        }

        /// <summary>
        /// Property used to get the human readable meter name 
        /// (string).  Use this property when 
        /// displaying the name of the meter to the user.  
        /// This should not be confused with the MeterType 
        /// which is used for meter determination and comparison.
        /// </summary>
        /// <returns>A string representing the human readable name of the 
        /// meter.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created
        //
        public override string MeterName
        {
            get
            {
                return GATEWAY_NAME;
            }
        }

        /// <summary>
        /// Gets the meter name that will be used in the activity log.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created
        //
        public override string ActivityLogMeterName
        {
            get
            {
                return LOG_ITR1_NAME;
            }
        }

        /// <summary>
        /// Builds the list of Event descriptions and returns the dictionary 
        /// </summary>
        /// <returns>
        /// Dictionary of Event Descriptions
        /// </returns> 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/18/13 AF  2.80.08 TR7578 Created
        //  03/20/13 MSC 2.80.08 TR7640 Updated for ICS event dictionary
        //
        public override ANSIEventDictionary EventDescriptions
        {
            get
            {
                if (null == m_dicEventDescriptions)
                {
                    m_dicEventDescriptions = (ANSIEventDictionary)(new ICS_Gateway_EventDictionary());
                }
                return m_dicEventDescriptions;
            }
        }

        /// <summary>
        /// Retrieves the Comm Module history log configuration from the meter.  The list
        /// includes all possible supported events with a description and a boolean
        /// indicating whether or not the event is monitored in the meter.  This version
        /// reads the config from tables 2522 and 2523.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //
        public List<MFG2048EventItem> CommModuleEventConfigured
        {
            get
            {
                return Table2523.ICSHistoryLogEventList;
            }
        }

        /// <summary>
        /// Retrieves the Comm Module Events that were recorded from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  06/21/13 AF  2.80.40 TR7640 Replaced CommModuleHistoryEntry with HistoryEntry
        //
        public List<HistoryEntry> CommModuleEvents
        {
            get
            {
                return Table2524.CommModuleHistoryEventEntries;
            }
        }

        /// <summary>
        /// Retrieves all events from Comm Module (Table 2524).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/06/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876  Created.
        //  
        public override List<HistoryEntry> Events
        {
            get
            {
                List<HistoryEntry> listOfCommEvents = new List<HistoryEntry>();
                ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
                ICSCommModule ICSModule = CommModule as ICSCommModule;
                byte[] commandResponse;

                if (ICSModule != null)
                {
                    if (ICSModule.FilterICSEvents() == ProcedureResultCodes.COMPLETED)
                    {
                        Result = ICSModule.UpdateEventTables(new DateTime(1970, 1, 1), DateTime.MaxValue, out commandResponse);

                        if (Result == ProcedureResultCodes.COMPLETED)
                        {
                            listOfCommEvents.AddRange(ICSModule.CommModuleEvents);
                        }
                    }
                }

                return listOfCommEvents;
            }
        }

        /// <summary>
        /// Get the list of configured events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/07/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876  Created.
        //  
        public override List<MFG2048EventItem> HistoryLogEventList
        {
            get
            {
                List<MFG2048EventItem> eventList = new List<MFG2048EventItem>();
                ICSCommModule ICSModule = CommModule as ICSCommModule;
                
                if (ICSModule != null)
                {
                    eventList.AddRange(ICSModule.CommModuleEventConfigured);
                }

                return eventList;
            }
        }

        /// <summary>
        /// Gets whether the meter supports History Log, more specifically the ICS Events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/15/13 DLG 3.50.04 TR9520, 7876 Overriding to check if ICS Event tables are used.
        //
        public override bool HistoryLogSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(2521) && true == Table00.IsTableUsed(2522)
                    && true == Table00.IsTableUsed(2523) && true == Table00.IsTableUsed(2524));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets whether ZigBee is enabled based on table 2512 (Mfg 464) values
        /// </summary>
        /// <remarks>A value of 0 means that ZigBee is enabled; 1 means it is disabled</remarks>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/30/14 AF  3.50.30 WR444483 Created
        //
        public bool ICMConfigurationTableZigBeeEnabled
        {
            get
            {
                return (Table2512.ZigBeeAccess == 0);
            }
        }

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Gets the meter type "M2GATEWAY"
        /// </summary>		
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ --------------------------------------------- 
        //  03/18/13 AF  2.80.08 TR7578 Created
        //
        protected override string DefaultMeterType
        {
            get
            {
                return ICS_MODEL;
            }
        }

        /// <summary>
        /// Gets the Table 2529 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created.   
        protected ICMMfgTable2529CommLanInfo Table2529
        {
            get
            {
                if (null == m_Table2529)
                {
                    m_Table2529 = new ICMMfgTable2529CommLanInfo(m_PSEM);
                }

                return m_Table2529;
            }
        }

        #endregion Protected Properties

        #region Private Properties

        /// <summary>
        /// Gets the Table 2512 object and creates it if needed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/30/14 AF  3.50.30 WR444483 Created
        //
        private ICMMfgTable2512ModuleConfiguration Table2512
        {
            get
            {
                if (m_Table2512 == null)
                {
                    m_Table2512 = new ICMMfgTable2512ModuleConfiguration(m_PSEM);
                }

                return m_Table2512;
            }
        }

        /// <summary>
        /// Gets the Table 2521 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  04/04/14 jrf 3.50.61 461982 Removed passing unneeded version to 
        //                              table 2521's constructor.
        private ICSMfgTable2521 Table2521
        {
            get
            {
                if (null == m_Table2521)
                {
                    m_Table2521 = new ICSMfgTable2521(m_PSEM); 
                }

                return m_Table2521;
            }
        }

        /// <summary>
        /// Gets the Table 2522 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //
        private ICSMfgTable2522 Table2522
        {
            get
            {
                if (null == m_Table2522)
                {
                    m_Table2522 = new ICSMfgTable2522(m_PSEM, Table2521);
                }

                return m_Table2522;
            }
        }

        /// <summary>
        /// Gets the Table 2523 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  07/29/16 AF  4.60.02 623194 Added a fw version parameter for table 2523 because the event
        //                              list supported is different between 3G and 4G meters.
        //
        private ICSMfgTable2523 Table2523
        {
            get
            {
                if (null == m_Table2523)
                {
                    m_Table2523 = new ICSMfgTable2523(m_PSEM, Table2522, Table2521, CommModuleVersion);
                }

                return m_Table2523;
            }
        }

        /// <summary>
        /// Gets the Table 2524 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //
        private ICSMfgTable2524 Table2524
        {
            get
            {
                if (null == m_Table2524)
                {
                    m_Table2524 = new ICSMfgTable2524(m_PSEM, Table2521, m_EventDictionary);
                }

                return m_Table2524;
            }
        }

        #endregion Private Properties

        #region Members

        /// <summary>
        /// Table 2130 object
        /// </summary>
        protected CHANMfgTable2130 m_Table2130 = null;

        /// <summary>
        /// The table 2512 object.  The ICM Configuration Table
        /// </summary>
        private ICMMfgTable2512ModuleConfiguration m_Table2512 = null;

        /// <summary>
        /// Table 2521 Object
        /// </summary>
        private ICSMfgTable2521 m_Table2521 = null;

        /// <summary>
        /// Table 2522 Object
        /// </summary>
        private ICSMfgTable2522 m_Table2522 = null;

        /// <summary>
        /// Table 2523 Object
        /// </summary>
        private ICSMfgTable2523 m_Table2523 = null;

        /// <summary>
        /// Table 2524 Object
        /// </summary>
        private ICSMfgTable2524 m_Table2524 = null;

        /// <summary>
        /// Table 2529 Object.
        /// </summary>
        private ICMMfgTable2529CommLanInfo m_Table2529 = null;

        /// <summary>
        /// ICS Gateway EventDictionary
        /// </summary>
        private ICS_Gateway_EventDictionary m_EventDictionary;

        /// <summary>
        /// Helper class that contains "read table" methods to obtain ERT data from an ICM.
        /// </summary>
        protected ICSTableReader m_ICSTableReader;

        #endregion Members
    }
}
