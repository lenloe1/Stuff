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
//                           Copyright © 2010 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using Itron.Common.C1219Tables.LandisGyr.Gateway;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Progressable;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    partial class M2_Gateway : IValidateProgram
    {
        #region Constants
        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a list of items that are not consistent between the configuration
        /// of the program and the device.
        /// </summary>
        /// <param name="strProgramName">The name of the program to validate against.</param>
        /// <returns>
        /// A list of items that failed the validation. Returns an empty list if
        /// all items match.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/10 AF  2.42.03        Created
        //
        public override List<ProgramValidationItem> ValidateProgram(string strProgramName)
        {
            PSEMResponse Response = PSEMResponse.Ok;

            List<ProgramValidationItem> InvalidItemsList = new List<ProgramValidationItem>();
            List<EDLValidationItem> ItemsToValidate = GetValidationList();
            List<ushort> ValidationTablesToRead = GetValidationTablesToRead();

            FileStream EDLFileStream = new FileStream(strProgramName, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlTextReader EDLReader = new XmlTextReader(EDLFileStream);

            GatewayTables ProgramTables = new GatewayTables();
            GatewayTables MeterTables = new GatewayTables();

            OnShowProgress(new ShowProgressEventArgs(1, ValidationTablesToRead.Count));

            // Read the data from the meter.
            // NOTE: ReadTable is defined in M2_Gateway_ICreateEDL so this will not compile if
            //       that file is not included. We may want to move this method eventually, but 
            //       we are keeping Interfaces separate in case we wish to support OpenWay in HH-Pro
            foreach (ushort TableID in ValidationTablesToRead)
            {
                OnStepProgress(new ProgressEventArgs());

                if (Response == PSEMResponse.Ok)
                {
                    if (MeterTables.IsTableKnown(TableID) && Table00.IsTableUsed(TableID) && (TableID == 0 || MeterTables.GetTableLength(TableID) > 0))
                    {
                        Response = ReadTable(TableID, ref MeterTables);
                    }
                }
            }

            if (Response != PSEMResponse.Ok)
            {
                throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error reading device for validation.");
            }
            else
            {
                // Load the EDL file.
                ProgramTables.LoadEDLFile(EDLReader);

                // Compare the values
                foreach (EDLValidationItem Item in ItemsToValidate)
                {
                    ProgramValidationItem InvalidItem;

                    // Only compare items where the meter's FW version is greater than or equal to 
                    // the minimum required for that item and the FW version is less than
                    // the version in which it became obsolete.  Max set to high for active items.
                    if ((VersionChecker.CompareTo(FWRevision, Item.MinFWVersion) >= 0) &&
                        (VersionChecker.CompareTo(FWRevision, Item.MaxFWVersion) < 0))
                    {
                        // We need to handle the display items differently than the rest of the items since
                        // there can be a different number of Normal and Test items.
                        if (RequiresSpecialHandling(Item.Item) == false)
                        {
                            InvalidItem = ValidateItem(Item, MeterTables, ProgramTables);

                            // Only add the item if it does not match.
                            if (null != InvalidItem)
                            {
                                InvalidItemsList.Add(InvalidItem);
                            }
                        }
                        else
                        {
                            InvalidItemsList.AddRange(HandleSpecialCases(Item.Item, MeterTables, ProgramTables));
                        }
                    }
                }
            }

            return InvalidItemsList;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the list of validation items.
        /// </summary>
        /// <returns>A list of items that will be validated.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/10 AF  2.42.03        Created
        //  08/09/10 AF  2.42.17        Removed f/w version parameters
        //  08/10/10 AF  2.42.17        Replaced event dictionary with one created for the Gateway
        //  10/22/10 AF  2.45.06 161866 Corrected display items
        //  06/28/11 AF  2.51.16 173770 Corrected the GatewayTblEnum for events
        //  02/23/12 jrf 2.53.43 194414 When HAN LOG CTRL table is not present in the meter,
        //                              validation of HAN events configured there in the 
        //                              program is being removed.
        //  03/27/12 AF  2.53.52 196102 Added the HAN 2 events to the list
        //  02/20/13 AF  2.70.69 322427 Set a max f/w version for InterPAN Mode which became obsolete in Lithium
        // 12/09/13 jrf 3.50.16 TQ 9560   Refactored retrieval of items into unique methods.
        //
        protected override List<EDLValidationItem> GetValidationList()
        {
            List<EDLValidationItem> ValidationList = new List<EDLValidationItem>();

            // TOU/Time 
            GetTOUTimeValidationItems(ValidationList);

            // Security  
            GetSecurityValidationItems(ValidationList);

            // Display
            GetDisplayValidationItems(ValidationList);        

            // Events
            GetEventExceptionValidationItems(ValidationList);

            // Communications
            GetCommunicationsValidationItems(ValidationList);

            return ValidationList;
        }

        /// <summary>
        /// Adds TOU/Time validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetTOUTimeValidationItems(List<EDLValidationItem> ValidationList)
        {

            ValidationList.Add(new EDLValidationItem((long)StdTableEnum.STDTBL53_TIME_ZONE_OFFSET,
                                        null,
                                        "Time Zone",
                                        "TOU/Time"));
        }

        /// <summary>
        /// Adds security validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetSecurityValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY,
                                        null,
                                        "Require Enhanced Security",
                                        "Security"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL,
                                        null,
                                        "Exception Security Model",
                                        "Security"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MfgTbl58SecurityMode,
                                        null,
                                        "HAN Security Profile",
                                        "Security"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MfgTbl58InterPanMode,
                                        null,
                                        "InterPAN Mode",
                                        "Security",
                                        VERSION_M2GTWY_1_0,
                                        VERSION_M2GTWY_2_0));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MfgTbl145RequireSignedAuthorization,
                                        null,
                                        "Require Signed Authentication",
                                        "Security"));
        }

        /// <summary>
        /// Adds display validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetDisplayValidationItems(List<EDLValidationItem> ValidationList)
        {
            // Display list in the M2 Gateway is maintained in standard tables

            for (int iIndex = 0; iIndex < 3; iIndex++)
            {
                ValidationList.Add(new EDLValidationItem((long)StdTableEnum.STDTBL34_SEC_DISP_SOURCES,
                            new int[] { iIndex },
                            "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture),
                            "Display Items"));
            }
        }

        /// <summary>
        /// Adds event/exception validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetEventExceptionValidationItems(List<EDLValidationItem> ValidationList)
        {
            CENTRON_AMI_EventDictionary EventDictionary = new CENTRON_AMI_EventDictionary();
            CENTRON_AMI_CommEventDictionary CommEventDictionary = new CENTRON_AMI_CommEventDictionary();
            
            for (int iIndex = 0; iIndex < 280; iIndex++)
            {
                // Get the description from the EventDictionary
                string strDescription;
                bool bValidString;

                bValidString = EventDictionary.TryGetValue(iIndex, out strDescription);

                if (bValidString == false)
                {
                    // It could be a MFG event since an event number is either STD or MFG but not both
                    bValidString = EventDictionary.TryGetValue(2048 + iIndex, out strDescription);
                }

                if (bValidString == false)
                {
                    // We don't have a description for the item.
                    strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
                }

                ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL0_HISTORY_LOG_MONITORED_FLAGS,
                                                new int[] { iIndex },
                                                strDescription,
                                                "Events"));
            }

            // Exceptions
            ValidationList.Add(new EDLValidationItem((long)StdTableEnum.STDTBL123_EXCEPTION_REPORT,
                                                      null,
                                                      "Exceptions",
                                                      "Exceptions"));

            // Comm Log - Standard LAN Events
            // TODO: Implement Standard LAN Events when they exist

            // Comm Log - Mfg LAN Events
            for (int iIndex = (int)LANEvents.BEGIN_MFG_LAN_EVENTS; iIndex < (int)LANEvents.END_MFG_LAN_EVENTS; iIndex++)
            {
                // Get the description from the EventDictionary
                string strDescription;
                bool bValidString;

                bValidString = CommEventDictionary.TryGetValue(iIndex, out strDescription);

                if (bValidString == false)
                {
                    // We don't have a description for the item.
                    strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
                }

                // Items in the MFG_EVENTS_MONITORED_FLAGS field do not include the MFG Bit.  (Must subtract off 2048)
                ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL113_MFG_EVENTS_MONITORED_FLAGS,
                                                new int[] { iIndex - 2048 },
                                                strDescription,
                                                "Mfg LAN Events"));
            }

            // Comm Log - Standard HAN Events
            // TODO: Implement Standard HAN Events when they exist

            //If table is not supported in the meter then it doesn't make sense to validate these items.
            if (true == Table00.IsTableUsed(115))
            {
                // Comm Log - Mfg HAN Events
                for (int iIndex = (int)HANEvents.BEGIN_MFG_HAN_EVENTS; iIndex < (int)HANEvents.END_MFG_HAN_EVENTS; iIndex++)
                {
                    // Get the description from the EventDictionary
                    string strDescription;
                    bool bValidString;

                    bValidString = CommEventDictionary.TryGetValue(iIndex, out strDescription);

                    if (bValidString == false)
                    {
                        // We don't have a description for the item.
                        strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
                    }

                    // Items in the MFG_EVENTS_MONITORED_FLAGS field do not include the MFG Bit.  (Must subtract off 2048)
                    ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL115_MFG_EVENTS_MONITORED_FLAGS,
                                                    new int[] { iIndex - 2048 },
                                                    strDescription,
                                                    "Mfg HAN Events"));
                }
            }

            // HAN 2 events
            CENTRON_AMI_DownstreamHANEventDictionary DownstreamEvtDictionary = new CENTRON_AMI_DownstreamHANEventDictionary();
            for (int iIndex = 2305; iIndex < 2431; iIndex++)
            {
                string strDescription;
                bool bValidString;

                bValidString = DownstreamEvtDictionary.TryGetValue((ushort)iIndex, out strDescription);

                // We won't worry about unknown events for now
                if (bValidString)
                {
                    ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MfgTbl212Han2LoggerControl,
                                            new int[] { iIndex - 2048 - 256 },
                                            strDescription,
                                            "HAN Events",
                                            VERSION_M2GTWY_2_0));
                }
            }

            CENTRON_AMI_UpstreamHANEventDictionary UpstreamEvtDictionary = new CENTRON_AMI_UpstreamHANEventDictionary();
            for (int iIndex = 2432; iIndex < 2559; iIndex++)
            {
                string strDescription;
                bool bValidString;

                bValidString = UpstreamEvtDictionary.TryGetValue((ushort)iIndex, out strDescription);

                // We won't worry about unknown events for now
                if (bValidString)
                {
                    ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MfgTbl212Han2LoggerControl,
                                            new int[] { iIndex - 2048 - 256 },
                                            strDescription,
                                            "HAN Events",
                                            VERSION_M2GTWY_2_0));
                }
            }
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
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL,
                                        null,
                                        "Lockout: login attempts, optical",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL,
                                        null,
                                        "Lockout: lockout minutes, optical",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN,
                                        null,
                                        "Lockout: login attemtps, lan",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN,
                                        null,
                                        "Lockout: lockout minutes, lan",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT,
                                        null,
                                        "LAN Send message failure limit",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS,
                                        null,
                                        "LAN Link metric (quality) period",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MfgTbl145C1218OverZigBee,
                                        null,
                                        "ANSI C12.18 support over ZigBee Enabled",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MfgTbl145DisableZigBeeRadio,
                                        null,
                                        "Disable ZigBee Radio",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)GatewayTblEnum.MfgTbl145DisableZigBeePrivateProfile,
                                        null,
                                        "Disable ZigBee Private Profile",
                                        "Communications"));
        }

        /// <summary>
        /// Checks to see if the item matches and then creates a ProgramValidationItem if it does not.
        /// </summary>
        /// <param name="item">The item to validate</param>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>Returns the ProgramValidationItem for the value if the items do not match, and null if the values match.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/10 AF  2.42.03        Created
        //  10/22/10 AF  2.45.06 161866 Added display items
        //  04/05/12 jrf 2.53.54 195670 Skipping validation of Inter PAN Mode if the program
        //                              value is null and the program came from a Lithium or greater CE.
        // 12/18/13 jrf 3.50.16 TQ9560 Refactored retreival of a CentronTables value into its own method.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToSingle(System.String)")]
        protected ProgramValidationItem ValidateItem(EDLValidationItem item, GatewayTables meterTables, GatewayTables programTables)
        {
            bool bItemsMatch = false;

            string strDisplayMeterValue = "";
            string strDisplayProgramValue = "";

            object objMeterValue;
            object objProgramValue;

            ProgramValidationItem InvalidItem = null;

            // Get the values
            objMeterValue = GetTableValue(item, meterTables);

            objProgramValue = GetTableValue(item, programTables);

            switch (item.Item)
            {
                case (long)StdTableEnum.STDTBL34_SEC_DISP_SOURCES:
                    {
                        if (objMeterValue != null)
                        {
                            if (String.Compare(objMeterValue.ToString(), "3", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                strDisplayMeterValue = "No Display Item";
                            }
                            else
                            {
                                int iIndex = Convert.ToInt32(objMeterValue.ToString(), CultureInfo.InvariantCulture);
                                strDisplayMeterValue = "Comm. Status Field " + (iIndex + 1).ToString(CultureInfo.InvariantCulture);
                            }
                        }
                        if (objProgramValue != null)
                        {
                            if (String.Compare(objProgramValue.ToString(), "3", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                strDisplayProgramValue = "No Display Item";
                            }
                            else
                            {
                                int iIndex = Convert.ToInt32(objProgramValue.ToString(), CultureInfo.InvariantCulture);
                                strDisplayProgramValue = "Comm. Status Field " + (iIndex + 1).ToString(CultureInfo.InvariantCulture);
                            }
                        }
                        if (strDisplayMeterValue.Equals(strDisplayProgramValue))
                        {
                            bItemsMatch = true;
                        }
                        break;
                    }
                case (long)StdTableEnum.STDTBL123_APTITLE_NOTIFY:
                    {
                        if (objMeterValue != null)
                        {
                            strDisplayMeterValue = ESNConverter.Decode((byte[])objMeterValue);
                        }

                        if (objProgramValue != null)
                        {
                            strDisplayProgramValue = ESNConverter.Decode((byte[])objProgramValue);
                        }

                        if (strDisplayMeterValue.Equals(strDisplayProgramValue))
                        {
                            bItemsMatch = true;
                        }

                        break;
                    }

                case (long)StdTableEnum.STDTBL53_TIME_ZONE_OFFSET:
                    {
                        TimeSpan tsOffset;
                        short sOffset;

                        if (objMeterValue != null)
                        {
                            sOffset = (short)objMeterValue;
                            tsOffset = TimeSpan.FromMinutes((double)sOffset);
                            strDisplayMeterValue = "GMT " + tsOffset.Hours + ":00";
                        }

                        if (objProgramValue != null)
                        {
                            sOffset = (short)objProgramValue;
                            tsOffset = TimeSpan.FromMinutes((double)sOffset);
                            strDisplayProgramValue = "GMT " + tsOffset.Hours + ":00";
                        }

                        // Compare the values
                        if (strDisplayMeterValue == strDisplayProgramValue)
                        {
                            bItemsMatch = true;
                        }

                        break;
                    }

                case (long)GatewayTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL:
                    {
                        OpenWayMFGTable2193.SecurityFormat MeterValue = OpenWayMFGTable2193.SecurityFormat.None;
                        OpenWayMFGTable2193.SecurityFormat ProgramValue = OpenWayMFGTable2193.SecurityFormat.None;

                        if (objMeterValue != null)
                        {
                            MeterValue = (OpenWayMFGTable2193.SecurityFormat)(byte)objMeterValue;
                            strDisplayMeterValue = OpenWayMFGTable2193.GetSecurityFormatString(MeterValue);
                        }

                        if (objProgramValue != null)
                        {
                            ProgramValue = (OpenWayMFGTable2193.SecurityFormat)(byte)objProgramValue;
                            strDisplayProgramValue = OpenWayMFGTable2193.GetSecurityFormatString(ProgramValue);
                        }

                        bItemsMatch = ProgramValue == MeterValue;

                        break;
                    }

                case (long)GatewayTblEnum.MfgTbl145C1218OverZigBee:
                    {
                        bool bMeterValue = false;
                        bool bProgramValue = false;

                        strDisplayProgramValue = null;

                        if (objMeterValue != null)
                        {
                            // We need to use the value of the bit
                            bMeterValue = (bool)objMeterValue;
                        }

                        strDisplayMeterValue = bMeterValue.ToString(CultureInfo.CurrentCulture);

                        if (objProgramValue != null)
                        {
                            bProgramValue = (bool)objProgramValue;
                            strDisplayProgramValue = bProgramValue.ToString(CultureInfo.CurrentCulture);
                        }

                        bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);
                        break;
                    }
                case (long)GatewayTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY:
                    {
                        strDisplayProgramValue = null;

                        if (objMeterValue != null)
                        {
                            strDisplayMeterValue = ((bool)objMeterValue).ToString(CultureInfo.CurrentCulture);
                        }

                        if (objProgramValue != null)
                        {
                            strDisplayProgramValue = ((bool)objProgramValue).ToString(CultureInfo.CurrentCulture);
                        }

                        bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                        break;
                    }
                case (long)GatewayTblEnum.MfgTbl58SecurityMode:
                    {
                        byte bySecurityMode;
                        byte byDeviceAuthMode;
                        byte byCBKEMode;

                        // Get the Meter value
                        if (objMeterValue != null && meterTables.IsCached((long)GatewayTblEnum.MfgTbl58DeviceAuthMode, null)
                            && meterTables.IsCached((long)GatewayTblEnum.MfgTbl58CbkeMode, null))
                        {
                            // We have already retrieved the Security Mode
                            bySecurityMode = (byte)objMeterValue;

                            // Get the other two modes
                            meterTables.GetValue(GatewayTblEnum.MfgTbl58DeviceAuthMode, null, out objMeterValue);
                            byDeviceAuthMode = (byte)objMeterValue;

                            meterTables.GetValue(GatewayTblEnum.MfgTbl58CbkeMode, null, out objMeterValue);
                            byCBKEMode = (byte)objMeterValue;

                            // Get the HAN Profile Name
                            strDisplayMeterValue = CHANMfgTable2106.GetHANSecurityProfile(bySecurityMode, byDeviceAuthMode, byCBKEMode);
                        }

                        // Get the Program value
                        if (objProgramValue != null && programTables.IsCached((long)GatewayTblEnum.MfgTbl58DeviceAuthMode, null)
                            && programTables.IsCached((long)GatewayTblEnum.MfgTbl58CbkeMode, null))
                        {
                            // We have already retrieved the Security Mode
                            bySecurityMode = (byte)objProgramValue;

                            // Get the other two modes
                            programTables.GetValue(GatewayTblEnum.MfgTbl58DeviceAuthMode, null, out objProgramValue);
                            byDeviceAuthMode = (byte)objProgramValue;

                            programTables.GetValue(GatewayTblEnum.MfgTbl58CbkeMode, null, out objProgramValue);
                            byCBKEMode = (byte)objProgramValue;

                            // Get the HAN Profile Name
                            strDisplayProgramValue = CHANMfgTable2106.GetHANSecurityProfile(bySecurityMode, byDeviceAuthMode, byCBKEMode);
                        }

                        bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                        break;
                    }
                case (long)GatewayTblEnum.MfgTbl58InterPanMode:
                    {
                        bool blnSkipComparison = false;

                        // Get the Meter value
                        if (objMeterValue != null)
                        {
                            // Get the Inter PAN Mode description
                            strDisplayMeterValue = CHANMfgTable2106.GetInterPANMode((byte)objMeterValue);
                        }

                        // Get the Program value
                        if (objProgramValue != null)
                        {
                            // Get the Inter PAN Mode description
                            strDisplayProgramValue = CHANMfgTable2106.GetInterPANMode((byte)objProgramValue);
                        }
                        else if (programTables.IsCached((long)GatewayTblEnum.MFGTBL2045_CE_VERSION_NUMBER, null))
                        {
                            object objValue = null;
                            programTables.GetValue(GatewayTblEnum.MFGTBL2045_CE_VERSION_NUMBER, null, out objValue);

                            string strValue = objValue as string;
                            string[] astrCEVersion = strValue.Split(new char[] { ' ', '.', '-' });
                            float fltCEVersion = Convert.ToSingle(astrCEVersion[0] + "." + astrCEVersion[1]);

                            if (0 <= VersionChecker.CompareTo(fltCEVersion, CE_VERSION_LITHIUM_3_9))
                            {
                                //Only skipping comparison if program value is null and program's CE version is Lithium or greater
                                blnSkipComparison = true;
                            }
                        }

                        if (true == blnSkipComparison)
                        {
                            bItemsMatch = true;
                        }
                        else
                        {
                            bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);
                        }

                        break;
                    }
                default:
                    {
                        // The GatewayTables object may return null so make sure we don't
                        // cause an exception first.
                        if (objMeterValue != null)
                        {
                            // Trim spaces and null characters so that they will display and validate correctly
                            strDisplayMeterValue = objMeterValue.ToString().Trim(new char[] { ' ', '\0' });
                        }

                        if (objProgramValue != null)
                        {
                            // Trim spaces and null characters so that they will display and validate correctly
                            strDisplayProgramValue = objProgramValue.ToString().Trim(new char[] { ' ', '\0' });
                        }

                        // Compare the values
                        if (strDisplayMeterValue == strDisplayProgramValue)
                        {
                            bItemsMatch = true;
                        }

                        break;
                    }
            }

            if (bItemsMatch == false)
            {
                // There is a mismatch so add the item.
                InvalidItem = new ProgramValidationItem(item.Category, item.Name, strDisplayProgramValue, strDisplayMeterValue);
            }

            return InvalidItem;
        }

        /// <summary>
        /// Handles and special cases.
        /// </summary>
        /// <param name="item">The item to handle</param>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>The list of invalid items for the special case.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/10 AF  2.42.03        Created
        //
        protected List<ProgramValidationItem> HandleSpecialCases(long item, GatewayTables meterTables, GatewayTables programTables)
        {
            List<ProgramValidationItem> InvalidItems = new List<ProgramValidationItem>();

            switch (item)
            {
                case (long)StdTableEnum.STDTBL123_EXCEPTION_REPORT:
                    {
                        InvalidItems = GetInvalidExceptionItems(meterTables, programTables);
                        break;
                    }
            }

            return InvalidItems;
        }

        /// <summary>
        /// Gets a list of invalid exception items.
        /// </summary>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>The list of invalid items.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/10 AF  2.42.03        Created
        //  03/06/12 AF  2.53.48 193820 We can no longer assume that the meter and program items will
        //                              be in the same order. We have to search both lists for mismatches
        //
        protected virtual List<ProgramValidationItem> GetInvalidExceptionItems(GatewayTables meterTables, GatewayTables programTables)
        {
            List<ProgramValidationItem> InvalidItems = new List<ProgramValidationItem>();
            ProgramValidationItem ValidationItem = null;

            object objMeterValue;
            object objProgramValue;

            ushort usMeterHosts;
            ushort usProgramHosts;
            ushort usMeterEvents;
            ushort usProgramEvents;

            ushort usMaxHosts;
            ushort usMaxEvents;

            // We shouldn't validate this if Table 123 is not cached in the program.
            if (programTables.IsAllCached(123))
            {
                // Get the number of exception hosts
                meterTables.GetValue(StdTableEnum.STDTBL121_NBR_EXCEPTION_HOSTS, null, out objMeterValue);
                programTables.GetValue(StdTableEnum.STDTBL121_NBR_EXCEPTION_HOSTS, null, out objProgramValue);

                usMeterHosts = (ushort)objMeterValue;
                usProgramHosts = (ushort)objProgramValue;

                // Get the number of exception events
                meterTables.GetValue(StdTableEnum.STDTBL121_NBR_EXCEPTION_EVENTS, null, out objMeterValue);
                programTables.GetValue(StdTableEnum.STDTBL121_NBR_EXCEPTION_EVENTS, null, out objProgramValue);

                usMeterEvents = (ushort)objMeterValue;
                usProgramEvents = (ushort)objProgramValue;

                // Determine the maximum values.
                if (usMeterHosts >= usProgramHosts)
                {
                    usMaxHosts = usMeterHosts;
                }
                else
                {
                    usMaxHosts = usProgramHosts;
                }

                if (usMeterEvents >= usProgramEvents)
                {
                    usMaxEvents = usMeterEvents;
                }
                else
                {
                    usMaxEvents = usProgramEvents;
                }

                // Verify each of the hosts
                for (int iHost = 0; iHost < usMaxHosts; iHost++)
                {
                    bool meterValidHost = iHost < usMeterHosts;
                    bool programValidHost = iHost < usProgramHosts;

                    List<ushort> meterEventsList = new List<ushort>();
                    List<ushort> programEventsList = new List<ushort>();
                    List<ushort> invalidMeterItemsList = new List<ushort>();
                    List<ushort> invalidProgramItemsList = new List<ushort>();
                    // the event number is just an index used for labeling the invalid item
                    int iEventNumber = 1;

                    int[] iIndexer = { iHost };
                    string strCategory = "Exception Host " + (iHost + 1).ToString(CultureInfo.CurrentCulture);

                    ValidationItem = ValidateItem(new EDLValidationItem((long)StdTableEnum.STDTBL123_APTITLE_NOTIFY, iIndexer, "Exception Host Aptitle", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)StdTableEnum.STDTBL123_MAX_NUMBER_OF_RETRIES, iIndexer, "Max Number of Retries", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)StdTableEnum.STDTBL123_RETRY_DELAY, iIndexer, "Retry Delay", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)StdTableEnum.STDTBL123_EXCLUSION_PERIOD, iIndexer, "Exclusion Period", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    if (meterValidHost == true)
                    {
                        for (int iEvent = 0; iEvent < usMeterEvents; iEvent++)
                        {
                            meterTables.GetValue(StdTableEnum.STDTBL123_EVENT_REPORTED, new int[] { iHost, iEvent }, out objMeterValue);
                            meterEventsList.Add((ushort)objMeterValue);
                        }
                    }

                    if (programValidHost == true)
                    {
                        for (int iEvent = 0; iEvent < usProgramEvents; iEvent++)
                        {
                            programTables.GetValue(StdTableEnum.STDTBL123_EVENT_REPORTED, new int[] { iHost, iEvent }, out objProgramValue);
                            programEventsList.Add((ushort)objProgramValue);
                        }
                    }

                    foreach (ushort item in meterEventsList)
                    {
                        if (!programEventsList.Contains(item))
                        {
                            invalidMeterItemsList.Add(item);
                        }
                    }

                    foreach (ushort item in invalidMeterItemsList)
                    {
                        string strMeterException = "";

                        if (EventDescriptions.TryGetValue((int)item, out strMeterException) == false)
                        {
                            // The TryGetValue failed so say it is an unknown event.
                            strMeterException = "Unknown Event " + item.ToString(CultureInfo.InvariantCulture);
                        }

                        // We don't know which program item this belongs with so just make the program item blank
                        InvalidItems.Add(new ProgramValidationItem(strCategory, "Event " + iEventNumber.ToString(CultureInfo.CurrentCulture),
                            "", strMeterException));

                        iEventNumber++;
                    }

                    foreach (ushort item in programEventsList)
                    {
                        if (!meterEventsList.Contains(item))
                        {
                            invalidProgramItemsList.Add(item);
                        }
                    }

                    foreach (ushort item in invalidProgramItemsList)
                    {
                        string strProgramException = "";

                        if (EventDescriptions.TryGetValue((int)item, out strProgramException) == false)
                        {
                            // The TryGetValue failed so say it is an unknown event.
                            strProgramException = "Unknown Event " + item.ToString(CultureInfo.InvariantCulture);
                        }

                        // We don't know which meter item this belongs with so just make the meter item blank
                        InvalidItems.Add(new ProgramValidationItem(strCategory, "Event " + iEventNumber.ToString(CultureInfo.CurrentCulture),
                            strProgramException, ""));

                        iEventNumber++;
                    }                    
                }
            }

            return InvalidItems;
        }

        /// <summary>
        /// Determines whether or not an item requires special handling.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the item requires special handling. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/10 AF  2.42.03        Created
        //
        protected override bool RequiresSpecialHandling(long item)
        {
            bool bRequired = false;

            switch (item)
            {
                case (long)StdTableEnum.STDTBL123_EXCEPTION_REPORT:
                {
                    bRequired = true;
                    break;
                }
            }

            return bRequired;
        }

        /// <summary>
        /// Gets the list of tables to read from the meter.
        /// </summary>
        /// <returns>The list of Table IDs</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/10 AF  2.42.03        Created
        //  08/09/10 AF  2.42.17        Updated table list
        //  09/27/10 AF  2.44.06 160787 Removed table 2090 and added tables 31, 34, 41, 42,
        //                              2048, 2061, and 2171
        //  10/21/10 AF  2.45.06 161866 Added back 2048 since it is now supported
        //  03/26/12 AF  2.53.52 196102 Adding Mfg table 212 to the list of validation tables
        //
        protected override List<ushort> GetValidationTablesToRead()
        {
            List<ushort> TableList = new List<ushort>();

            TableList.Add(0);
            TableList.Add(1);
            TableList.Add(6);
            TableList.Add(31);
            TableList.Add(34);
            TableList.Add(41);
            TableList.Add(42);
            TableList.Add(53);
            TableList.Add(71);
            TableList.Add(73);
            TableList.Add(121);
            TableList.Add(123);
            TableList.Add(2048);
            TableList.Add(2061);
            TableList.Add(2106);
            TableList.Add(2159);
            TableList.Add(2161);
            TableList.Add(2163);
            TableList.Add(2171);
            TableList.Add(2190);
            TableList.Add(2193);
            TableList.Add(2260);

            return TableList;
        }
        
        #endregion

        #region Members
        #endregion

    }
}
