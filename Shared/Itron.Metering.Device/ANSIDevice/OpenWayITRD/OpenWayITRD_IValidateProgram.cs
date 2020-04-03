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
//                           Copyright © 2007 - 2013
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
    partial class OpenWayITRD : IValidateProgram
    {
        #region Protected Methods

        /// <summary>
        /// Creates the list of validation items.
        /// </summary>
        /// <returns>A list of items that will be validated.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 02/08/12 RCG	2.53.39	TQ 3445   Created
        // 02/23/12 jrf 2.53.43    194414 When HAN LOG CTRL table is not present in the meter,
        //                                validation of HAN events configured there in the 
        //                                program is being removed.
        // 02/23/12 jrf 2.53.43    194414 When HAN LOG CTRL table is not present in the meter,
        //                                validation of HAN events configured there in the 
        //                                program is being removed.
        // 03/27/12 AF  2.53.52    196102 Added the HAN 2 events to the list
        // 02/15/13 jrf 2.70.69    243289 Adding appropriate code to exclude items added in Lithium
        //                                (extended voltage monitoring, instrumentation profile,
        //                                extended self read) when they are not appropriate.  
        //                                Removed extended energies from validation
        //                                since only appropriate to ITRE & ITRF.
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

            // Extended Self Read
            GetExtendedSelfReadValidationItems(ValidationList);

            // User Data
            GetUserDataValidationList(ValidationList);

            // Display
            GetDisplayValidationItems(ValidationList);

            // Events/Exceptions
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
        #endregion
    }
}
