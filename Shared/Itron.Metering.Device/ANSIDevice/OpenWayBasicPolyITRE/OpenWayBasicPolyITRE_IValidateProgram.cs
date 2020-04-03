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
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    public partial class OpenWayBasicPolyITRE
    {
        #region Protected Methods

        /// <summary>
        /// Creates the list of validation items.
        /// </summary>
        /// <returns>A list of items that will be validated.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05	  	      Created
        // 05/20/09 AF  2.20.05           Corrected a typo in a string
        // 02/23/12 jrf 2.53.43    194414 When HAN LOG CTRL table is not present in the meter,
        //                                validation of HAN events configured there in the 
        //                                program is being removed.
        // 03/26/12 AF  2.53.52    196102 Added missing LED pulse weights and HAN 2 events
        // 02/15/13 jrf 2.70.69    243289 Adding appropriate code to exclude items added in Lithium
        //                                (extended voltage monitoring, instrumentation profile, 
        //                                extended energy/load profile, extended self read) when 
        //                                they are not appropriate.
        // 02/20/13 AF  2.70.69    322427 Set a max f/w version for InterPAN Mode which became obsolete in Lithium
        // 08/09/13 jrf 2.85.14 TQ 7657   Adding validation of items new to Michigan.
        // 12/09/13 jrf 3.50.16 TQ 9560   Refactored retrieval of items into unique methods.
        // 05/11/16 PGH 4.50.266 683886   Added Push Data and Temperature
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

            // Extended Energy Quantities
            GetExtendedEnergyQuantityValidationItems(ValidationList);

            // Register Operations
            GetRegisterOperationValidationItems(ValidationList);

            // Device Multipliers
            GetDeviceMultiplierValidationItems(ValidationList);

            // Load Profile
            GetLoadProfileValidationItems(ValidationList);

            // Extended Load Profile
            GetExtendedLoadProfileValidationItems(ValidationList);

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
            GetEventExceptionValidationItems(ValidationList);

            // Communications
            GetCommunicationsValidationItems(ValidationList);

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
        /// Adds extended energy quantity validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetExtendedEnergyQuantityValidationItems(List<EDLValidationItem> ValidationList)
        {
            if (ExtLoadProfileSupported) //Extended energies only supported if extended LP is also.
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillableEnergyId,
                                            new int[] { 0 },
                                            "Extended Energy 1 Quantity",
                                            "Extended Energy Quantities",
                                                        VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillableEnergyId,
                                            new int[] { 1 },
                                            "Extended Energy 2 Quantity",
                                            "Extended Energy Quantities",
                                                        VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillableEnergyId,
                                            new int[] { 2 },
                                            "Extended Energy 3 Quantity",
                                            "Extended Energy Quantities",
                                                        VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillableEnergyId,
                                            new int[] { 3 },
                                            "Extended Energy 4 Quantity",
                                            "Extended Energy Quantities",
                                                        VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillableEnergyId,
                                            new int[] { 4 },
                                            "Extended Energy 5 Quantity",
                                            "Extended Energy Quantities",
                                                        VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillableEnergyId,
                                            new int[] { 5 },
                                            "Extended Energy 6 Quantity",
                                            "Extended Energy Quantities",
                                                        VERSION_LITHIUM_3_12));
            }
        }

        /// <summary>
        /// Adds extended load profile validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetExtendedLoadProfileValidationItems(List<EDLValidationItem> ValidationList)
        {
            if (ExtLoadProfileSupported)
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier,
                                            new int[] { 0 },
                                            "Quantity 1",
                                            "Extended Load Profile",
                                                    VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeight,
                                            new int[] { 0 },
                                            "Pulse Weight 1",
                                            "Extended Load Profile",
                                                    VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier,
                                            new int[] { 1 },
                                            "Quantity 2",
                                            "Extended Load Profile",
                                                    VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeight,
                                            new int[] { 1 },
                                            "Pulse Weight 2",
                                            "Extended Load Profile",
                                                    VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier,
                                            new int[] { 2 },
                                            "Quantity 3",
                                            "Extended Load Profile",
                                                    VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeight,
                                            new int[] { 2 },
                                            "Pulse Weight 3",
                                            "Extended Load Profile",
                                                    VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier,
                                            new int[] { 3 },
                                            "Quantity 4",
                                            "Extended Load Profile",
                                                VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeight,
                                            new int[] { 3 },
                                            "Pulse Weight 4",
                                            "Extended Load Profile",
                                                VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier,
                                            new int[] { 4 },
                                            "Quantity 5",
                                            "Extended Load Profile",
                                                VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeight,
                                            new int[] { 4 },
                                            "Pulse Weight 5",
                                            "Extended Load Profile",
                                                VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier,
                                            new int[] { 5 },
                                            "Quantity 6",
                                            "Extended Load Profile",
                                                VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeight,
                                            new int[] { 5 },
                                            "Pulse Weight 6",
                                            "Extended Load Profile",
                                                VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileIntervalLength,
                                            null,
                                            "Interval Length",
                                            "Extended Load Profile",
                                                VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileMinimumOutage,
                                            null,
                                            "Outage Length",
                                            "Extended Load Profile",
                                                VERSION_LITHIUM_3_12));
            }

        }

        #endregion
    }
}
