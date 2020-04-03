///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//   embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//storage or retrieval system without the permission in writing from Itron, Inc.
//
//                           Copyright © 2009 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using Itron.Common.C1219Tables.Centron;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    public partial class OpenWayAdvPoly
    {
        #region Protected Methods

        /// <summary>
        /// Checks to see if the item matches and then creates a ProgramValidationItem if it does not.
        /// </summary>
        /// <param name="item">The item to validate</param>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>Returns the ProgramValidationItem for the value if the items do not match, and null if the values match.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/23/07 RCG	8.10.05		   Created
        // 12/18/13 jrf 3.50.16 TQ9560 Refactored retreival of a CentronTables value into its own method.

        protected override ProgramValidationItem ValidateItem(EDLValidationItem item, CentronTables meterTables, CentronTables programTables)
        {
            bool bItemsMatch = false;
            bool bCheckedInBaseClass = false;

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
                case (long)CentronTblEnum.MFGTBL0_LOCK_SITESCAN_ERROR:
                    {
                        object ScrollMeterValue;
                        object ScrollProgramValue;

                        // Get the scroll values
                        meterTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_SITESCAN_ERROR,
                            item.Index, out ScrollMeterValue);
                        programTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_SITESCAN_ERROR,
                            item.Index, out ScrollProgramValue);

                        strDisplayMeterValue = ConvertDisplayableErrors((bool)objMeterValue, (bool)ScrollMeterValue);
                        strDisplayProgramValue = ConvertDisplayableErrors((bool)objProgramValue, (bool)ScrollProgramValue);

                        if (strDisplayMeterValue == strDisplayProgramValue)
                        {
                            bItemsMatch = true;
                        }

                        break;
                    }
                default:
                    {
                        InvalidItem = base.ValidateItem(item, meterTables, programTables);
                        bCheckedInBaseClass = true;
                        break;
                    }
            }

            if (bItemsMatch == false && bCheckedInBaseClass == false)
            {
                // There is a mismatch so add the item.
                InvalidItem = new ProgramValidationItem(item.Category, item.Name, strDisplayProgramValue, strDisplayMeterValue);
            }

            return InvalidItem;
        }

        /// <summary>
        /// Gets the list of tables to read from the meter.
        /// </summary>
        /// <returns>The list of Table IDs</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created
        // 09/19/11 jrf 2.52.18 175979 Adding validation of power monitoring enabled.
        // 09/19/11 jrf 2.52.18 177455 Adding validation of ZigBee power level.
        // 09/19/11 jrf 2.52.18 180287 Adding a minimum firmware version for enable asset 
        //                             synchronization, enable fatal error recovery
        //                             and enable power monitoring validation items.
        //
        protected override List<ushort> GetValidationTablesToRead()
        {
            return base.GetValidationTablesToRead();
        }

        /// <summary>
        /// Creates the list of validation items.
        /// </summary>
        /// <returns>A list of items that will be validated.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05	    	  Created
        // 05/20/09 AF  2.20.05           Corrected a typo in a string
        // 02/23/12 jrf 2.53.43    194414 When HAN LOG CTRL table is not present in the meter,
        //                                validation of HAN events configured there in the 
        //                                program is being removed.
        // 03/26/12 AF  2.53.52    196102 Added missing LED pulse weights and HAN 2 events
        // 02/15/13 jrf 2.70.69    243289 Adding appropriate code to exclude items added in Lithium
        //                                (extended voltage monitoring, instrumentation profile) when 
        //                                they are not appropriate.  Removed extended energies from validation
        //                                since only appropriate to ITRE & ITRF.
        // 02/20/13 AF  2.70.69    322427 Set a max f/w version for InterPAN Mode which became obsolete in Lithium
        // 08/09/13 jrf 2.85.14 TQ 7657   Adding validation of items new to Michigan.
        // 12/09/13 jrf 3.50.16 TQ 9560   Refactored retrieval of items into unique methods.
        // 05/11/16 PGH 4.50.266 683886   Added Push Data and Temperature
        //
        protected override List<EDLValidationItem> GetValidationList()
        {
            List<EDLValidationItem> ValidationList = new List<EDLValidationItem>();
            CENTRON_AMI_EventDictionary EventDictionary = new CENTRON_AMI_EventDictionary();
            CENTRON_AMI_CommEventDictionary CommEventDictionary = new CENTRON_AMI_CommEventDictionary();

            // TOU/Time configuration
            GetTOUTimeValidationItems(ValidationList);

            // Security
            GetSecurityValidationItems(ValidationList);

            // Quantities
            GetQuantityValidationItems(ValidationList);            
            
            // Register Operations
            GetRegisterOperationValidationItems(ValidationList);

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

            // User Data
            GetUserDataValidationList(ValidationList);

            // Display
            GetDisplayValidationItems(ValidationList);

            // Events/Exceptions
            GetEventExceptionValidationItems(ValidationList);

            // Communications
            GetCommunicationsValidationItems(ValidationList);

            // SiteScan
            GetSiteScanValidationItems(ValidationList);

            // LED
            GetLEDValidationItems(ValidationList);

            // Push Set
            GetPushSetValidationItems(ValidationList);

            // Push Group
            GetPushGroupValidationItems(ValidationList);

            // Temperature
            GetTemperatureValidationItems(ValidationList);

            return ValidationList;
        }
        

        /// <summary>
        /// Adds demand quantity validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetDemandQuantityValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_DEFINITION,
                                        new int[] { 0 },
                                        "Demand 1 Quantity",
                                        "Quantities"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_DEFINITION,
                                        new int[] { 1 },
                                        "Demand 2 Quantity",
                                        "Quantities"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_DEFINITION,
                                        new int[] { 2 },
                                        "Demand 3 Quantity",
                                        "Quantities"));
        }

        /// <summary>
        /// Adds display errors validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetDisplayErrorsValidationItems(List<EDLValidationItem> ValidationList)
        {
            base.GetDisplayErrorsValidationItems(ValidationList);
            
            //Add one more.
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_SITESCAN_ERROR,
                                        null,
                                        "Non-Fatal Error #9 - SiteScan Error",
                                        "Displayable Errors"));
        }

        /// <summary>
        /// Adds display errors validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetSiteScanValidationItems(List<EDLValidationItem> ValidationList)
        {
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_SITE_SCAN_1_ENABLE,
                                            null,
                                            "Enable Diag 1 (Cross Phase, Polarity, Energy Flow Error)",
                                            "SiteScan"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_SITE_SCAN_2_ENABLE,
                                            null,
                                            "Enable Diag 2 (Phase Voltage Deviation Error)",
                                            "SiteScan"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_SITE_SCAN_3_ENABLE,
                                            null,
                                            "Enable Diag 3 (Inactive Phase Current Error)",
                                            "SiteScan"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_SITE_SCAN_4_ENABLE,
                                            null,
                                            "Enable Diag 4 (Phase Angle Displacement Error)",
                                            "SiteScan"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_SCROLL_POLARITY,
                                            null,
                                            "Scroll Diagnostic 1 Error",
                                            "SiteScan"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_SCROLL_PHASE_VOLTAGE_DEVIATION,
                                            null,
                                            "Scroll Diagnostic 2 Error",
                                            "SiteScan"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_SCROLL_INACTIVE_PHASE_CURRENT,
                                            null,
                                            "Scroll Diagnostic 3 Error",
                                            "SiteScan"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_SCROLL_PHASE_ANGLE_DEVIATION,
                                            null,
                                            "Scroll Diagnostic 4 Error",
                                            "SiteScan"));
            }
        }


        #endregion
    }
}
