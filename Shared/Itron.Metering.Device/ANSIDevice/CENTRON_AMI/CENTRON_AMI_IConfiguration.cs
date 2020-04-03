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
//                              Copyright © 20?? - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.AMIConfiguration;
using Itron.Metering.Progressable;
using Itron.Metering.Communications.PSEM;
using Itron.Metering;
using Itron.Metering.DeviceDataTypes;
using Itron.Common.C1219Tables.Centron;

namespace Itron.Metering.Device
{
    public partial class CENTRON_AMI : IConfiguration
    {

        #region Public Events
        // These events are overridden so that the event handlers can be passed to
        // the configuration object, rather than handling the event here and then
        // raising it again. This also requires the OnShowProgress, OnStepProgress
        // and OnHideProgress methods to be overriden, otherwise the base class events
        // will be raised.

        /// <summary>
        /// Event that shows the progress bar
        /// </summary>
        public override event ShowProgressEventHandler ShowProgressEvent;

        /// <summary>
        /// Event that hides the progress bar
        /// </summary>
        public override event HideProgressEventHandler HideProgressEvent;

        /// <summary>
        /// Event that causes the progress bar to perform a step
        /// </summary>
        public override event StepProgressEventHandler StepProgressEvent;

        #endregion Public Events

        #region Public Methods
        /// <summary>
        /// Configures the meter with the specified program
        /// </summary>
        /// <param name="sProgramName">The path to the program file</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/29/06 RCG				Created
#if (!WindowsCE)
        public override ConfigurationResult Configure(string sProgramName)
#else
		public ConfigurationResult Configure(string sProgramName)
#endif
        {
            PromptForData PFData = new PromptForData();

            PFData.UnitID = UnitID;
            PFData.SerialNumber = SerialNumber;
            PFData.InitialDateTime = null;

            return Configure(sProgramName, PFData);
        }

        /// <summary>
        /// Configures the meter with the specified program
        /// </summary>
        /// <param name="sProgramName">The path to the program file</param>
        /// <param name="PFData">Prompt For data Object</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/29/08 KRC				Created
        /// 08/21/12 AF  2.70.06 202030 We can no longer count on using isBBV to determine
        ///                             if the meter supports configuring the base energies
#if (!WindowsCE)
        public override ConfigurationResult Configure(string sProgramName, PromptForData PFData)
#else
		public ConfigurationResult Configure(string sProgramName, PromptForData PFData)
#endif        
        {
            ConfigurationResult Result = ConfigurationResult.SUCCESS;
            AMIConfigureDevice ConfigureDevice = new AMIConfigureDevice(m_PSEM);
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;

            // Set up the progress bar event handlers
            ConfigureDevice.ShowProgressEvent += this.ShowProgressEvent;
            ConfigureDevice.StepProgressEvent += this.StepProgressEvent;
            ConfigureDevice.HideProgressEvent += this.HideProgressEvent;

            ConfigureDevice.IsCanadian = IsCanadian;

            // We always need to set the Prompt for data so we should just use what is
            // currently in the meter.

            ConfigureDevice.UnitID = PFData.UnitID;
            ConfigureDevice.CustomerSerialNumber = PFData.SerialNumber;
            ConfigureDevice.InitialDateTime = PFData.InitialDateTime;

            // If the meter uses Base Backed Values we need to configure the base energies
            if (UsesBaseBackedValues || ConfigureBaseEnergiesSupported)
            {
                ProcResult = ConfigureBase(sProgramName);

                if (ProcResult != ProcedureResultCodes.COMPLETED)
                {
                    switch (ProcResult)
                    {
                        case ProcedureResultCodes.DEVICE_SETUP_CONFLICT:
                        {
                            Result = ConfigurationResult.INVALID_CONFIG;
                            break;
                        }
                        case ProcedureResultCodes.NO_AUTHORIZATION:
                        {
                            Result = ConfigurationResult.SECURITY_ERROR;
                            break;
                        }
                        case ProcedureResultCodes.UNRECOGNIZED_PROC:
                        {
                            Result = ConfigurationResult.UNSUPPORTED_FUNCTION;
                            break;
                        }
                        default:
                        {
                            Result = ConfigurationResult.ERROR;
                            break;
                        }
                    }
                }
            }

            if (Result == ConfigurationResult.SUCCESS)
            {
                ConfigError = ConfigureDevice.Configure(sProgramName);

                // Translate to the ItronDevice ConfigurationResult error code since 
                // the factory is using ConfigurationError and we do not want to always
                // rely on having the version in AMIConfiguration.dll
                Result = TranslateConfigError(ConfigError);
            }

            return Result;
        }

        /// <summary>
        /// Configures the meter instrumentation profile config block 
        /// with the specified program
        /// </summary>
        /// <param name="sProgramName">The path to the program file</param>
        /// <param name="configItems"></param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/06/12 JKW				Created - Lithium
        /// 
        public ConfigurationResult PartialConfiguration(string sProgramName, BlocksForPartialConfiguration configItems)
        {
            ConfigurationResult Result = ConfigurationResult.SUCCESS;
            AMIConfigureDevice ConfigureDevice = new AMIConfigureDevice(m_PSEM);

            // Load the EDL file into the table set
            Result = TranslateConfigError(ConfigureDevice.LoadEDLFileToTableSet(sProgramName));

            // check each flag within the enumeration reconfiguring the blocks specified
            if (Result == ConfigurationResult.SUCCESS && configItems.HasFlag(BlocksForPartialConfiguration.InstrumentationProfile))
            {
                Result = ConfigureInstrumentationProfile(ConfigureDevice);
            }

            if (Result == ConfigurationResult.SUCCESS && configItems.HasFlag(BlocksForPartialConfiguration.ExtendedEnergyAndLoadProfile))
            {
                Result = ConfigureNonBillingEnergyandLoadProfile(ConfigureDevice);
            }

            if (Result == ConfigurationResult.SUCCESS && configItems.HasFlag(BlocksForPartialConfiguration.ExtendedVoltageMonitoring))
            {
                Result = ConfigureVoltageMonitoring(ConfigureDevice);
            }

            if (Result == ConfigurationResult.SUCCESS && configItems.HasFlag(BlocksForPartialConfiguration.ExtendedSelfRead))
            {
                Result = ConfigureExtendedSelfRead(ConfigureDevice);
            }

            return Result;
        }

        /// <summary>
        /// Configures the meter instrumentation profile config block 
        /// with the specified program
        /// </summary>
        /// <param name="ConfigureDevice">AMIConfigureDevice object with the correct table set already loaded</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/06/12 JKW				Created - Lithium
        /// 
        protected ConfigurationResult ConfigureInstrumentationProfile(AMIConfigureDevice ConfigureDevice)
        {
            ConfigurationResult Result = ConfigurationResult.SUCCESS;
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            if (Table00.IsTableUsed(2265))
            {
                ConfigError = ConfigureDevice.WriteTableByElementRange(
                    CentronTblEnum.MfgTbl217InstrumentationProfileIntervalLength,
                    null,
                    CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeightSetTwo,
                    new int[1] { ConfigureDevice.GetFieldArrayLastIndex(CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeightSetTwo) });
            }
            else
            {
                ConfigError = ConfigurationError.OPERATION_NOT_POSSIBLE;
            }

            // Translate to the ItronDevice ConfigurationResult error code since 
            // the factory is using ConfigurationError and we do not want to always
            // rely on having the version in AMIConfiguration.dll
            Result = TranslateConfigError(ConfigError);

            return Result;
        }

        /// <summary>
        /// Configures the meter non-billing energy and non-billing LP config block 
        /// with the specified program
        /// </summary>
        /// <param name="ConfigureDevice">AMIConfigureDevice object with the correct table set already loaded</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/06/12 JKW				Created - Lithium
        /// 03/22/13 jrf 2.80.10        Switched to use device class constants.
        /// 07/20/15 AF  4.50.169 585299 Added ITRK to the acceptable devices.
        /// 07/29/15 AF  4.50.178 585299 Corrected the logic for ITRK
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToUpper")]
        protected ConfigurationResult ConfigureNonBillingEnergyandLoadProfile(AMIConfigureDevice ConfigureDevice)
        {
            ConfigurationResult Result = ConfigurationResult.SUCCESS;
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            if (Table00.IsTableUsed(2265) &&
                (Table00.DeviceClass.ToUpper().Equals(ITRE_DEVICE_CLASS) || Table00.DeviceClass.ToUpper().Equals(ITRF_DEVICE_CLASS)) ||
                (Table00.DeviceClass.ToUpper().Equals(ITRK_DEVICE_CLASS)))
            {
                ConfigError = ConfigureDevice.WriteTableByElementRange(
                    CentronTblEnum.MfgTbl217NonBillableEnergyId,
                    new int[1] { 0 },
                    CentronTblEnum.MfgTbl217NonBillableEnergyId,
                    new int[1] { ConfigureDevice.GetFieldArrayLastIndex(CentronTblEnum.MfgTbl217NonBillableEnergyId) });

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    ConfigError = ConfigureDevice.WriteTableByElementRange(
                        CentronTblEnum.MfgTbl217NonBillingLoadProfileIntervalLength,
                        null,
                        CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeightSetTwo,
                        new int[1] { ConfigureDevice.GetFieldArrayLastIndex(CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeightSetTwo) });
                }
            }
            else
            {
                ConfigError = ConfigurationError.OPERATION_NOT_POSSIBLE;
            }

            // Translate to the ItronDevice ConfigurationResult error code since 
            // the factory is using ConfigurationError and we do not want to always
            // rely on having the version in AMIConfiguration.dll
            Result = TranslateConfigError(ConfigError);

            return Result;
        }

        /// <summary>
        /// Configures the meter VM tables with the specified program. Reads
        /// table 0 to see which VM tables are supported 
        /// (101, 102, 105, 106 = Lithium, 101, 102 = pre-Lithium)
        /// </summary>
        /// <param name="ConfigureDevice">AMIConfigureDevice object with the correct table set already loaded</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/06/12 JKW				Created - Lithium
        /// 
        protected ConfigurationResult ConfigureVoltageMonitoring(AMIConfigureDevice ConfigureDevice)
        {
            ConfigurationResult Result = ConfigurationResult.SUCCESS;
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            ConfigureDevice.IsCanadian = IsCanadian;

            // pre-Lithium only uses 2149 + 2150 
            if (Table00.IsTableUsed(2149) && Table00.IsTableUsed(2150))
            {
                ConfigError = ConfigureDevice.WriteTable(2149);

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    ConfigError = ConfigureDevice.WriteTable(2150);
                }
            }

            // Lithium uses 2153 + 2154 
            if (Table00.IsTableUsed(2153) && Table00.IsTableUsed(2154))
            {
                ConfigError = ConfigureDevice.WriteTable(2153);

                if (ConfigError == ConfigurationError.SUCCESS)
                {
                    ConfigError = ConfigureDevice.WriteTable(2154);
                }
            }

            // Translate to the ItronDevice ConfigurationResult error code since 
            // the factory is using ConfigurationError and we do not want to always
            // rely on having the version in AMIConfiguration.dll
            Result = TranslateConfigError(ConfigError);

            return Result;
        }

        /// <summary>
        /// Configures the extended self read config block (self read two)
        /// with the specified program
        /// </summary>
        /// <param name="ConfigureDevice">AMIConfigureDevice object with the correct table set already loaded</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/06/12 JKW				Created - Lithium
        /// 03/22/13 jrf 2.80.10        Adding ITRJ device class and also switched to use device class constants.
        /// 07/20/15 AF  4.50.169 585299 Adding ITRK device class
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToUpper")]
        protected ConfigurationResult ConfigureExtendedSelfRead(AMIConfigureDevice ConfigureDevice)
        {
            ConfigurationResult Result = ConfigurationResult.SUCCESS;
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            if (Table00.IsTableUsed(2265) &&
                (Table00.DeviceClass.ToUpper().Equals(ITRD_DEVICE_CLASS) ||
                 Table00.DeviceClass.ToUpper().Equals(ITRE_DEVICE_CLASS) ||
                 Table00.DeviceClass.ToUpper().Equals(ITRF_DEVICE_CLASS) ||
                 Table00.DeviceClass.ToUpper().Equals(ITRJ_DEVICE_CLASS) ||
                 Table00.DeviceClass.ToUpper().Equals(ITRK_DEVICE_CLASS)))
            {
                ConfigError = ConfigureDevice.WriteTableByElementRange(
                    CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier,
                    new int[1] { 0 },
                    CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifierQualifier,
                    new int[1] { ConfigureDevice.GetFieldArrayLastIndex(CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifierQualifier) });
            }
            else
            {
                ConfigError = ConfigurationError.OPERATION_NOT_POSSIBLE;
            }

            // Translate to the ItronDevice ConfigurationResult error code since 
            // the factory is using ConfigurationError and we do not want to always
            // rely on having the version in AMIConfiguration.dll
            Result = TranslateConfigError(ConfigError);

            return Result;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Raises the event to show the progress bar.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created
        // 01/06/14 DLG 3.50.19        Changed from protected to internal.
        //
        internal override void OnShowProgress(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that causes the progress bar to perform a step
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created
        // 01/06/14 DLG 3.50.19        Changed from protected to internal.
        //
        internal override void OnStepProgress(ProgressEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that hides or closes the progress bar
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created
        // 01/06/14 DLG 3.50.19        Changed from protected to internal.
        //
        internal override void OnHideProgress(EventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(this, e);
            }
        }

        #endregion Internal Methods
    }
}