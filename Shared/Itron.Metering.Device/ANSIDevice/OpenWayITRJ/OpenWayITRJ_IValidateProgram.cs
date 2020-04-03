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
//                           Copyright © 2013 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using Itron.Common.C1219Tables.Centron;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Progressable;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    partial class OpenWayITRJ : IValidateProgram
    {
        #region Constants

        /// <summary>
        /// The lowest supported event id for ITRJ's table 2523
        /// </summary>
        private const int START_ICM_EVENT_ID = 1536;
        /// <summary>
        /// The highest support event id for ITRJ's table 2523
        /// </summary>
        private const int END_ICM_EVENT_ID = 1704;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the list of validation items.
        /// </summary>
        /// <returns>A list of items that will be validated.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 08/09/13 jrf 2.85.14 TQ 7657   Created
        // 08/14/13 jrf 2.85.17 TQ 7657   Modified to only validate ERT Radio field if ICS
        //                                ERT data is populated.
        // 12/09/13 jrf 3.50.16 TQ 9560   Refactored retrieval of items into unique methods.
        // 05/11/16 PGH 4.50.266 683886   Added Push Data and Temperature
        // 08/02/16 AF  4.60.02 WR 704376 Separated the event list into its own method so that the ICM events can be concatenated
        //
        protected override List<EDLValidationItem> GetValidationList()
        {
            List<EDLValidationItem> ValidationList = new List<EDLValidationItem>();

            // TOU/Time configuration
            GetTOUTimeValidationItems(ValidationList);

            // Security
            GetSecurityValidationItems(ValidationList);

            // Quantities
            GetQuantityValidationItems(ValidationList);                        

            // Register Operations
            GetRegisterOperationValidationItems(ValidationList);

            //WAN            
            GetWANValidationItems(ValidationList);

            // Device Multipliers
            GetDeviceMultiplierValidationItems(ValidationList);

            // Load Profile
            GetLoadProfileValidationItems(ValidationList);

            // Instrumentation Profile
            GetInstrumentaionProfileValidationItems(ValidationList);

            // Voltage Monitor
            GetVoltageMonitoringValidationItems(ValidationList);

            // Enhanced Voltage Monitoring
            GetEnhancedVoltageMonitoringValidationItems(ValidationList);

            // Extended Self Read
            GetExtendedSelfReadValidationItems(ValidationList);

            // User Data
            GetUserDataValidationList(ValidationList);

            // Display
            GetDisplayValidationItems(ValidationList);

            // Events
            GetEventValidationItems(ValidationList);
            GetEventExceptionValidationItems(ValidationList);

            // Service Limiting
            GetServiceLimitingValidationItems(ValidationList);

            // Communications
            GetCommunicationsValidationItems(ValidationList);

            // Push Set
            GetPushSetValidationItems(ValidationList);

            // Push Group
            GetPushGroupValidationItems(ValidationList);

            // Temperature
            GetTemperatureValidationItems(ValidationList);

            return ValidationList;
        }

        /// <summary>
        /// Adds communications validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetCommunicationsValidationItems(List<EDLValidationItem> ValidationList)
        {
            base.GetCommunicationsValidationItems(ValidationList);

            if (true == IsICSERTDataPopulated())
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl461ERTRadio,
                                            null,
                                            "Disable ERT Radio",
                                            "Communications",
                                            VERSION_MICHIGAN));
            }
        }

        /// <summary>
        /// Gets the base class list of validation items plus the ICM events
        /// </summary>
        /// <param name="ValidationList"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  08/03/16 AF  4.60.02  WR 704376  Separated out from GetEventExceptionValidationItems so that
        //                                   ICM events will be contiguous with register events
        //
        protected override void GetEventValidationItems(List<EDLValidationItem> ValidationList)
        {
            base.GetEventValidationItems(ValidationList);

            // Remove the inversion tamper event
            for (int index = 0; index < ValidationList.Count; index++)
            {
                if (String.Compare(m_rmStrings.GetString("INVERSION_TAMPER"), ValidationList[index].Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ValidationList.RemoveAt(index);
                    break;
                }
            }

            // Remove the removal tamper event. This has to be a separate loop because the indexes are adjusted after the previous removal
            for (int index = 0; index < ValidationList.Count; index++)
            {
                if (String.Compare(m_rmStrings.GetString("REMOVAL_TAMPER"), ValidationList[index].Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ValidationList.RemoveAt(index);
                    break;
                }
            }


            ICS_Gateway_EventDictionary EventDictionary = new ICS_Gateway_EventDictionary();

            for (int iIndex = START_ICM_EVENT_ID; iIndex < END_ICM_EVENT_ID; iIndex++)
            {
                // Get the description from the EventDictionary
                string strDescription;
                bool bValidString;

                // The event ids in the dictionary are for mfg table 2524, which are the 2523 ids plus 2048
                bValidString = EventDictionary.TryGetValue(iIndex + 2048, out strDescription);

                if (bValidString == false)
                {
                    // We don't have a description for the item.
                    strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
                }

                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL475_MFG_EVENTS_MONITORED_FLAGS,
                                                new int[] { iIndex },
                                                strDescription,
                                                "Events"));
            }
        }

        /// <summary>
        /// Gets the base class list of validation items plus the ICM exception report configuration items
        /// </summary>
        /// <param name="ValidationList">The validation list to populate</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        protected override void GetEventExceptionValidationItems(List<EDLValidationItem> ValidationList)
        {
            base.GetEventExceptionValidationItems(ValidationList);

            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL489_EXCEPTION_REPORT,
                                            null,
                                            "ICM Exceptions",
                                            "ICM Exceptions",
                                            VERSION_SR6_6_4GLTE));
        }

        /// <summary>
        /// Adds WAN validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        // 07/01/15 AF  4.20.17 WR 593937 Removed the Power Fail Time validation. It's not in the program
        //
        protected virtual void GetWANValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464NtpAddress,
                                       null,
                                       "NTP Address",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464NtpUpdateFrequency,
                                       null,
                                       "NTP Update Frequency (hours)",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464NtpValidTime,
                                       null,
                                       "NTP Valid Time (minutes)",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl469CellularDataTimeoutTimeout,
                                       null,
                                       "Cellular Data Timeout",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl469CellularDataTimeoutUnits,
                                       null,
                                       "Cellular Data Timeout Units",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464LinkFailuresThreshold,
                                       null,
                                       "Link Failures Threshold",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464LinkFailuresResetFrequency,
                                       null,
                                       "Link Failures Reset Frequency (hours)",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464TowerChangesThreshold,
                                       null,
                                       "Tower Changes Threshold",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464TowerChangesResetFrequency,
                                       null,
                                       "Tower Changes Reset Frequency (hours)",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464SectorIDChangesThreshold,
                                       null,
                                       "Sector ID Changes Threshold",
                                       "WAN",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl464SectorIDChangesResetFrequency,
                                       null,
                                       "Sector ID Changes Reset Frequency (hours)",
                                       "WAN",
                                       VERSION_MICHIGAN));
        }

        /// <summary>
        /// Returns the base class list of validation tables plus the ITRJ specific ICS exception report configuration tables
        /// </summary>
        /// <returns>the list of tables to validate</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //  08/03/16 AF  4.60.02  WR 704376  Added ICM event tables
        //
        protected override List<ushort> GetValidationTablesToRead()
        {
            List<ushort> TableList = new List<ushort>();

            TableList = base.GetValidationTablesToRead();

            TableList.Add(2521);
            TableList.Add(2522);
            TableList.Add(2523);
            TableList.Add(2536);
            TableList.Add(2537);

            return TableList;
        }

        #endregion
    }
}
