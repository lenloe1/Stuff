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
//                           Copyright © 2009 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Globalization;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    public partial class OpenWayBasicPoly
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
        // 03/26/12 AF  2.53.52 196102 Added the missing LED pulse weights
        // 04/19/12 jrf 2.53.56 195674 Modified to only check legacy voltage monitoring items when config is pre-Lithium.
        // 12/18/13 jrf 3.50.16 TQ9560 Refactored retreival of a CentronTables value into its own method.
        //
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
                case (long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD:
                case (long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD:
                case (long)CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD:
                case (long)CentronTblEnum.MFGTBL102_RMS_VOLT_HIGH_THRESHOLD:
                {
                    if (objMeterValue != null)
                    {
                        strDisplayMeterValue = objMeterValue.ToString() + "%";
                    }

                    if (objProgramValue != null)
                    {
                        strDisplayProgramValue = objProgramValue.ToString() + "%";
                    }

                    // Compare the values
                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }
                    else if (true == IsConfigVersionEqualOrGreaterThan(CE_VERSION_LITHIUM_3_9, programTables))
                    {
                        //Ignoring comparison if values do not match and program's CE version is Lithium or greater
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_BLINK_MISSING_PHASES:
                {
                    bool bMeterValue;
                    bool bProgramValue;

                    if (objMeterValue != null)
                    {
                        bMeterValue = (bool)objMeterValue;

                        if (bMeterValue)
                        {
                            strDisplayMeterValue = "Blink Phase Indicator";
                        }
                        else
                        {
                            strDisplayMeterValue = "Turn Off Phase Indicator";
                        }
                    }

                    if (objProgramValue != null)
                    {
                        bProgramValue = (bool)objProgramValue;

                        if (bProgramValue)
                        {
                            strDisplayProgramValue = "Blink Phase Indicator";
                        }
                        else
                        {
                            strDisplayProgramValue = "Turn Off Phase Indicator";
                        }
                    }

                    if (strDisplayProgramValue == strDisplayMeterValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_NORMAL:
                case (long)CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_TEST:
                {
                    LEDQuantity MeterQuantity = null;
                    LEDQuantity ProgramQuantity = null;

                    if (objMeterValue != null)
                    {
                        MeterQuantity = new LEDQuantity((uint)objMeterValue);
                        strDisplayMeterValue = MeterQuantity.Description;
                    }

                    if (objProgramValue != null)
                    {
                        ProgramQuantity = new LEDQuantity((uint)objProgramValue);
                        strDisplayProgramValue = ProgramQuantity.Description;
                    }

                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL121_FORM_1S_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_2S_C200_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_2S_C320_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_3S_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_4S_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_9S_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_12S_C200_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_12S_C320_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_16S_C200_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_16S_C320_PULSE_WEIGHT:
                case (long)CentronTblEnum.MFGTBL121_FORM_45S_PULSE_WEIGHT:
                {
                    float fMeterValue = 0.0f;
                    float fProgramValue = 0.0f;

                    if (objMeterValue != null)
                    {
                        fMeterValue = (ushort)objMeterValue * 0.025f;
                        strDisplayMeterValue = fMeterValue.ToString("F3", CultureInfo.CurrentCulture);
                    }

                    if (objProgramValue != null)
                    {
                        fProgramValue = (ushort)objProgramValue * 0.025f;
                        strDisplayProgramValue = fProgramValue.ToString("F3", CultureInfo.CurrentCulture);
                    }

                    if (fMeterValue == fProgramValue)
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

        protected override List<ushort> GetValidationTablesToRead()
        {
            List<ushort> TablesToRead = base.GetValidationTablesToRead();

            TablesToRead.Add(2169);

            return TablesToRead;

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
        // 10/17/16 AF  4.70.26  702303   Removed the Push Data and temperature items. The meter doesn't support them 
        //                                and the program will not have those configuration items.
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

            // LED
            GetLEDValidationItems(ValidationList);

            return ValidationList;
        }

        /// <summary>
        /// Adds quantity validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version  ID Number Description
        // -------- --- -------  -- ------ ---------------------------------------
        // 06/07/16 jrf 4.50.280 WR 633121 Created.
        //
        protected override void GetQuantityValidationItems(List<EDLValidationItem> ValidationList)
        {
            GetEnergyQuantityValidationItems(ValidationList);
            GetDemandQuantityValidationItems(ValidationList);
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
        }

        /// <summary>
        /// Adds device multiplier validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetDeviceMultiplierValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE,
                                        null,
                                        "Register Fullscale",
                                        "Device Multipliers"));
        }

        /// <summary>
        /// Adds display options validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetAdditionalDisplayOptionsValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_VAR_LOAD_INDICATOR,
                                        null,
                                        "Enable PF Load Indicator",
                                        "Display Options"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENABLE_PHASE_INDICATORS,
                                        null,
                                        "Enable Phase Indicators",
                                        "Display Options"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_BLINK_MISSING_PHASES,
                                        null,
                                        "Missing Phase Indicator",
                                        "Display Options"));
        }

        /// <summary>
        /// Adds LED validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetLEDValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_NORMAL,
                            null,
                            "Normal Mode Quantity",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_TEST,
                            null,
                            "Test Mode Quantity",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_1S_PULSE_WEIGHT,
                           null,
                           "Form 1S Pulse Weight",
                           "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_2S_C200_PULSE_WEIGHT,
                            null,
                            "Form 2S C200 Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_2S_C320_PULSE_WEIGHT,
                            null,
                            "Form 2S C320 Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_3S_PULSE_WEIGHT,
                            null,
                            "Form 3S Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_4S_PULSE_WEIGHT,
                            null,
                            "Form 4S Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_9S_PULSE_WEIGHT,
                            null,
                            "Form 9S Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_12S_C200_PULSE_WEIGHT,
                            null,
                            "Form 12S Class 200 Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_12S_C320_PULSE_WEIGHT,
                            null,
                            "Form 12S Class 320 Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_16S_C200_PULSE_WEIGHT,
                            null,
                            "Form 16S Class 200 Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_16S_C320_PULSE_WEIGHT,
                            null,
                            "Form 16S Class 320 Pulse Weight",
                            "LED"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL121_FORM_45S_PULSE_WEIGHT,
                            null,
                            "Form 45S Pulse Weight",
                            "LED"));
        }

        #endregion
    }
}
