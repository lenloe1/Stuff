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
//                              Copyright © 2009-2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Device
{
    public partial class CENTRON_AMI
    {
        #region Public Properties
        /// <summary>
        /// Gets whether or not the Service Limiting table is present in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created
        //  02/18/14 AF  3.00.36 WR428639 M2 Gateway has a table 2139 but it is not a service limiting table
        //
        public bool IsServiceLimitingTablePresent
        {
            get
            {
                bool blnM2Gateway = this is M2_Gateway;
                bool blnIsSupported = false;

                if (!blnM2Gateway)
                {
                    blnIsSupported = (Table2139 != null);
                }
                else
                {
                    blnIsSupported = false;
                }

                return blnIsSupported;
            }
        }

        /// <summary>
        /// Gets whether or not the disconnect hardware exists in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public bool DisconnectHardwareExists
        {
            get
            {
                if (Table2139 != null)
                {
                    return Table2139.DoesHardwareExist;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently connected.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public bool IsConnected
        {
            get
            {
                if (Table2140 != null)
                {
                    return Table2140.IsConnected;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently connected with a fresh table read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  01/18/16 jrf 4.23.02 WR 645946 Created
        public bool IsConnectedUncached
        {
            get
            {
                if (Table2140 != null)
                {
                    Table2140.Read();
                    return Table2140.IsConnected;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not the disconnect hardware is functioning
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public bool IsDisconnectHardwareFunctioning
        {
            get
            {
                if (Table2140 != null)
                {
                    return Table2140.IsFunctioning;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently armed for activation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public bool IsMeterArmed
        {
            get
            {
                if (Table2140 != null)
                {
                    return Table2140.IsMeterArmed;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }


        /// <summary>
        /// Gets the Time remaining in Failsafe
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD         N/A    Created

        public TimeSpan TimeInFailsafe
        {
            get
            {
                TimeSpan tsInFailSafe;

                if (Table2140 != null)
                {
                    OpenWayMFGTable2140SP5 NewTable2140 = Table2140 as OpenWayMFGTable2140SP5;
                    if (NewTable2140 != null)
                    {
                        tsInFailSafe = NewTable2140.RemainingTimeForFailsafe;
                    }
                    else
                    {
                        tsInFailSafe = new TimeSpan(0);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }

                return tsInFailSafe;
            }
        }

        /// <summary>
        /// Gets the Failsafe Reason
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD         N/A    Created

        public bool IsInFailSafe
        {
            get
            {
                bool bResult = false;

                if (Table2140 != null)
                {
                    OpenWayMFGTable2140SP5 NewTable2140 = Table2140 as OpenWayMFGTable2140SP5;
                    if (NewTable2140 != null)
                    {
                        bResult = NewTable2140.IsInFailsafeMode;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }

                return bResult;
            }
        }

        /// <summary>
        /// Gets the amount of time remaining in failsafe mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/09 RCG 2.21.01 139277 Created

        public TimeSpan TimeRemainingInFailsafe
        {
            get
            {
                TimeSpan TimeRemaining = new TimeSpan(0, 0, 0);

                if (Table2140 != null)
                {
                    OpenWayMFGTable2140SP5 NewTable2140 = Table2140 as OpenWayMFGTable2140SP5;
                    if (NewTable2140 != null)
                    {
                        TimeRemaining = NewTable2140.RemainingTimeForFailsafe;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }

                return TimeRemaining;
            }
        }

        /// <summary>
        /// tells whether the meter is in failsafe
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD         N/A    Created

        public string FailsafeReason
        {
            get
            {
                FailsafeReasons Reason = FailsafeReasons.NotSupported;
                string strReason = "";

                if (Table2140 != null)
                {
                    OpenWayMFGTable2140SP5 NewTable2140 = Table2140 as OpenWayMFGTable2140SP5;
                    if (NewTable2140 != null)
                    {
                        Reason = NewTable2140.FailsafeReason;
                    }

                    strReason = TranslateFailSafeReason(Reason);
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }

                return strReason;
            }
        }

        /// <summary>
        /// Gets the Last Disconnect due to Service Limiting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD         N/A    Created

        public bool WasLastDisconnectDueToSL
        {
            get
            {
                bool bValue = false;

                if (Table2140 != null)
                {
                    OpenWayMFGTable2140SP5 NewTable2140 = Table2140 as OpenWayMFGTable2140SP5;
                    if (NewTable2140 != null)
                    {
                        bValue = NewTable2140.WasLastDisconnectDueToSL;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }

                return bValue;
            }
        }

        /// <summary>
        /// Gets whether or not load voltage is currently present.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public bool IsLoadVoltagePresent
        {
            get
            {
                if (Table2140 != null)
                {
                    return Table2140.IsLoadVoltagePresent;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not load voltage is currently present with a fresh table read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  01/18/16 jrf 4.23.02 WR 645946 Created
        public bool IsLoadVoltagePresentUncached
        {
            get
            {
                if (Table2140 != null)
                {
                    Table2140.Read();
                    return Table2140.IsLoadVoltagePresent;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not the last disconnect attempt failed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public bool DidLastDisconnectAttemptFail
        {
            get
            {
                if (Table2140 != null)
                {
                    return Table2140.LastAttemptFailed;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not user intervention is required after a connection.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public bool ConnectsUsingUserIntervention
        {
            get
            {
                if (Table2141 != null)
                {
                    return Table2141.ConnectsUsingUserIntervention;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the maximum number of disconnects allowed in the configured period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public byte MaxDisconnects
        {
            get
            {
                if (Table2141 != null)
                {
                    return Table2141.MaxSwitches;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the number of remaining daily switches
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/09 RCG 2.10.02 N/A    Created

        public byte RemainingDailySwitches
        {
            get
            {
                if (Table2140 != null)
                {
                    return Table2140.RemainingDailySwitches;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the period of time when the alarm will be raised after a disconnect.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public TimeSpan DisconnectRandomizationAlarmPeriod
        {
            get
            {
                if (Table2141 != null)
                {
                    return Table2141.RandomizationAlarmPeriod;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the minimum amount of time to wait before reconnecting.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public TimeSpan ReconnectStartDelay
        {
            get
            {
                if (Table2141 != null)
                {
                    return Table2141.RestorationStartDelay;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the period of time where the meter will be reconnected a
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public TimeSpan ReconnectRandomDelay
        {
            get
            {
                if (Table2141 != null)
                {
                    return Table2141.RestorationRandomDelay;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the amount of time the switch will remain open after a service limiting disconnect.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public TimeSpan DisconnectOpenDelay
        {
            get
            {
                if (Table2141 != null)
                {
                    return Table2141.OpenTime;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the quantity for the normal mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public string NormalModeThresholdQuantity
        {
            get
            {
                if (Table2141 != null)
                {
                    uint uiLidNumber;
                    LID QuantityLid;
                    string strDescription = "None";

                    if (Table2139.NumberOfThresholds >= 1 &&
                        Table2141.Thresholds[0].DemandQuantityIndex < Table2048.DemandConfig.Demands.Count)
                    {
                        uiLidNumber = Table2048.DemandConfig.Demands[Table2141.Thresholds[0].DemandQuantityIndex];
                        QuantityLid = CreateLID(uiLidNumber);

                        strDescription = QuantityLid.lidDescription;
                    }

                    return strDescription;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the threshold value for normal mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public float NormalModeThreshold
        {
            get
            {
                float fThreshold = 0.0F;

                if (Table2141 != null)
                {
                    if (Table2139.NumberOfThresholds >= 1)
                    {
                        fThreshold = Table2141.Thresholds[0].Threshold;
                    }

                    return fThreshold;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the quantity for the critical mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public string CriticalModeThresholdQuantity
        {
            get
            {
                if (Table2141 != null)
                {
                    uint uiLidNumber;
                    LID QuantityLid;
                    string strDescription = "None";

                    if (Table2139.NumberOfThresholds >= 2 &&
                        Table2141.Thresholds[1].DemandQuantityIndex < Table2048.DemandConfig.Demands.Count)
                    {
                        uiLidNumber = Table2048.DemandConfig.Demands[Table2141.Thresholds[1].DemandQuantityIndex];
                        QuantityLid = CreateLID(uiLidNumber);

                        strDescription = QuantityLid.lidDescription;
                    }

                    return strDescription;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the threshold value for critical mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public float CriticalModeThreshold
        {
            get
            {
                float fThreshold = 0.0F;

                if (Table2141 != null)
                {
                    if (Table2139.NumberOfThresholds >= 2)
                    {
                        fThreshold = Table2141.Thresholds[1].Threshold;
                    }

                    return fThreshold;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the quantity for the emergency mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public string EmergencyModeThresholdQuantity
        {
            get
            {
                if (Table2141 != null)
                {
                    uint uiLidNumber;
                    LID QuantityLid;
                    string strDescription = "None";

                    if (Table2139.NumberOfThresholds >= 3 &&
                        Table2141.Thresholds[2].DemandQuantityIndex < Table2048.DemandConfig.Demands.Count)
                    {
                        uiLidNumber = Table2048.DemandConfig.Demands[Table2141.Thresholds[2].DemandQuantityIndex];
                        QuantityLid = CreateLID(uiLidNumber);

                        strDescription = QuantityLid.lidDescription;
                    }

                    return strDescription;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the threshold value for emergency mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public float EmergencyModeThreshold
        {
            get
            {
                float fThreshold = 0.0F;

                if (Table2141 != null)
                {
                    if (Table2139.NumberOfThresholds >= 3)
                    {
                        fThreshold = Table2141.Thresholds[2].Threshold;
                    }

                    return fThreshold;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not service limiting has been overriden in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public bool IsServiceLimitingOverriden
        {
            get
            {
                if (Table2142 != null)
                {
                    return Table2142.IsServiceLimitingOverriden;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the currently active service limiting threshold.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/22/09 RCG 2.10.01 126265 Created

        public byte ActiveServiceLimitingThreshold
        {
            get
            {
                if (Table2140 != null)
                {
                    return Table2140.ActiveThreshold;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the amount of time left in the currently active service limiting threshold.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/22/09 RCG 2.10.01 126265 Created

        public TimeSpan TimeRemainingInServiceLimitingThreshold
        {
            get
            {
                if (Table2140 != null)
                {
                    return Table2140.RemainingTimeForActiveTier;
                }
                else
                {
                    throw new InvalidOperationException("Service Limiting is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the failsafe reason if the is in failsafe period 
        /// </summary>
        /// <param name="reason">The reason to translate.</param>
        /// <returns>The reason for the failsafe mode</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03 137680 Adding support for Connect/Disconnect status to EDL Viewer

        public static string TranslateFailSafeReason(FailsafeReasons reason)
        {
            string strReason = "";

            switch (reason)
            {
                case FailsafeReasons.NotInFailsafe:
                {
                    strReason = "Disabled";
                    break;
                }
                case FailsafeReasons.ProcedureWithDuration:
                {
                    strReason = "Enabled: External command with duration.";
                    break;
                }
                case FailsafeReasons.FirmwareActivated:
                {
                    strReason = "Enabled: Firmware activated";
                    break;
                }
                case FailsafeReasons.MeterInitialized:
                {
                    strReason = "Enabled: Meter initialized";
                    break;
                }
                case FailsafeReasons.ServiceLimitingConfigured:
                {
                    strReason = "Enabled: Service Limiting reconfigured";
                    break;
                }
                case FailsafeReasons.DemandConfigured:
                {
                    strReason = "Enabled: Demand reconfigured";
                    break;
                }
                case FailsafeReasons.ProcedureNoDuration:
                {
                    strReason = "Enabled: External command with no duration.";
                    break;
                }
                case FailsafeReasons.NotSupported:
                {
                    strReason = "Disabled: Not supported.";
                    break;
                }
                default:
                {
                    strReason = "Error";
                    break;
                }
            }

            return strReason;
        }

        #endregion
    }
}
