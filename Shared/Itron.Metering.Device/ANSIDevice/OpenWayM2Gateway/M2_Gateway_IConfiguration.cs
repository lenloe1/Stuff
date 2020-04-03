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
//                           Copyright © 2010 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.AMIConfiguration;
using Itron.Metering.Progressable;
using Itron.Metering.Communications.PSEM;
using Itron.Metering;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    public partial class M2_Gateway : IConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Configures the meter with the specified program
        /// </summary>
        /// <param name="sProgramName">The path to the program file</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/08/10 AF  2.41.07		Created
#if (!WindowsCE)
        public override ConfigurationResult Configure(string sProgramName)
#else
		public ConfigurationResult Configure(string sProgramName)
#endif
        {
            AMIConfigureM2Gateway ConfigureDevice = new AMIConfigureM2Gateway(m_PSEM);
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            // Set up the progress bar event handlers
            ConfigureDevice.ShowProgressEvent += this.ShowProgressEvent;
            ConfigureDevice.StepProgressEvent += this.StepProgressEvent;
            ConfigureDevice.HideProgressEvent += this.HideProgressEvent;

            ConfigureDevice.IsCanadian = IsCanadian;

            // We always need to set the Prompt for data so we should just use what is
            // currently in the meter.

            ConfigureDevice.UnitID = UnitID;
            ConfigureDevice.CustomerSerialNumber = SerialNumber;
            ConfigureDevice.InitialDateTime = null;

            ConfigError = ConfigureDevice.Configure(sProgramName);

            // Translate to the ItronDevice ConfigurationResult error code since 
            // the factory is using ConfigurationError and we do not want to always
            // rely on having the version in AMIConfiguration.dll

            return TranslateConfigError(ConfigError);
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
        /// 06/08/10 AF  2.41.07		Created
        /// 02/02/11 AF  2.50.04        Added check for null to quiet compiler warning
#if (!WindowsCE)
        public override ConfigurationResult Configure(string sProgramName, PromptForData PFData)
#else
		public ConfigurationResult Configure(string sProgramName, PromptForData PFData)
#endif
        {
            AMIConfigureM2Gateway ConfigureDevice = new AMIConfigureM2Gateway(m_PSEM);
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            // Set up the progress bar event handlers
            ConfigureDevice.ShowProgressEvent += this.ShowProgressEvent;
            ConfigureDevice.StepProgressEvent += this.StepProgressEvent;
            ConfigureDevice.HideProgressEvent += this.HideProgressEvent;

            ConfigureDevice.IsCanadian = IsCanadian;

            if (PFData != null)
            {
                ConfigureDevice.UnitID = PFData.UnitID;
                ConfigureDevice.CustomerSerialNumber = PFData.SerialNumber;
                ConfigureDevice.InitialDateTime = PFData.InitialDateTime;
            }

            ConfigError = ConfigureDevice.Configure(sProgramName);

            // Translate to the ItronDevice ConfigurationResult error code since 
            // the factory is using ConfigurationError and we do not want to always
            // rely on having the version in AMIConfiguration.dll

            return TranslateConfigError(ConfigError);
        }

        #endregion Public Methods

        #region Events

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

        #endregion Events

        #region Internal Methods

        /// <summary>
        /// Raises the event to show the progress bar.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/08/10 AF  2.41.07		   Created
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
        // 06/08/10 AF  2.41.07		   Created
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
        // 06/08/10 AF  2.41.07		   Created
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