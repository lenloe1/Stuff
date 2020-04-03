///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                              Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;
using Itron.Metering.TOU;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This CTable2048_OpenWay class manages the header and config blocks of 2048.
    /// This class is specific to the OpenWay meter.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  10/02/07 KRC 7.35.00 N/A    Created
    //  09/23/10 SCW 1.00.00        Modified CTable2048_OpenWay for CTable2048_MaxImage     
	//
    internal class CTable2048_MaxImage : CTable2048
    {
        #region Constants
      
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor, create the config blocks specific to the OpenWay.
        /// </summary>
        /// <param name="psem">The protocol instance to use</param>
        /// <example><code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable2048_OpenWayl Table2048 = new CTable2048_OpenWay( PSEM, FW_Rev ); 
        /// </code></example>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //  10/23/06 AF  7.40.xx N/A    Added billing schedule component
        //  10/30/06 AF  7.40.xx        Added history log component
        //  04/21/09 jrf 2.20.02 N/A    Added IO config component.
        // 
        public CTable2048_MaxImage(CPSEM psem)
            : base(psem)
        {
            m_DemandConfig = new CENTRON2_MONO_DemandConfig(psem, m_2048Header.DemandOffset);
            m_CoefficientsConfig = new CoefficientsConfig(psem, m_2048Header.CoefficientsOffset);
            m_ConstantsConfig = new ConstantsConfig(psem, m_2048Header.ConstantsOffset, 
                ConstantsConfig.OPENWAY_CONSTANTS_CONFIG_LENGTH);
            m_DisplayConfig = new CENTRON2_MONO_DisplayConfig(psem, m_2048Header.DisplayOffset);

            if (0 != m_2048Header.TOUOffset)
            {
                m_TOUConfig = new CENTRON2_MONO_TOUConfig(psem, m_2048Header.TOUOffset);
            }
            else
            {
                m_TOUConfig = null;
            }

            if (0 != m_2048Header.CalendarOffset)
            {
                m_CalendarConfig = new CENTRON2_MONO_CalendarConfig(psem, m_2048Header.CalendarOffset,
                    CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_SIZE, CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_YEARS);
            }
            else
            {
                m_CalendarConfig = null;
            }

            m_ModeControl = new CENTRON2_MONO_ModeControl(psem, m_2048Header.ModeControlOffset);
            m_BillingSchedConfig = new BillingSchedConfig(psem, m_2048Header.BillingSchedOffset,
                    BillingSchedConfig.CENTRONII_BILLING_SCHED_CONFIG_LENGTH);

            m_HistoryLogConfig = new CENTRON2_MONO_HistoryLogConfig(psem, m_2048Header.HistoryLogOffset);

            if (0 != m_2048Header.CPCOffset)
            {
                m_MetrologyTable = new CENTRON2_MONO_Metrology(m_PSEM, m_2048Header.CPCOffset);
            }

            if (0 != m_2048Header.IOOffset)
            {
                m_IOConfig = new CENTRON2_MONO_IOConfig(m_PSEM, m_2048Header.IOOffset);
            }
            
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property to return Metrology Table
        /// </summary>
        public CENTRON2_MONO_Metrology MetrologyTable
        {
            get
            {
                return m_MetrologyTable;
            }
        }

        /// <summary>
        /// Returns true if the meter is configured for DST
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 11/20/10 SCW         N/A    Created
        public override bool HasDST
        {
            get
            {
                return HaveDST;
            }
        }

        /// <summary>
        /// Returns the configured TOU ID.  Value is 0 if meter wasn't configured
        /// for TOU.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/25/06 mcm 7.35.07 113    # Rates is not sufficient for determining if 
        //                             TOU was configured
        // 03/15/07 RCG 8.00.18        Removing redundant read code since the property reads the table
        // 12/04/10 SCW 9.70.13        Added TOU_ID to support CENTRON2_MONO
        public override ushort TOU_ID
        {
            get
            {
                ushort uintTOU_ID = 0;

                // If we don't have a calendar configuration 
                // (PRE_SATURN SENTINELS), we won't have DST.
                if (m_2048Header.CalendarOffset > 0)
                {
                    if (m_CalendarConfig != null)
                        uintTOU_ID = m_CalendarConfig.CalendarID;
                }
                return uintTOU_ID;
            }
        }

        #endregion
        
        #region Members

        private CENTRON2_MONO_Metrology m_MetrologyTable;
        private bool HaveDST = false;

        #endregion
    }

    /// <summary>
    /// Class that represents the history log configuration data stored in table 2048
    /// for the CENTRON_AMI meter.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 10/14/10 jrf 2.45.04 N/A    Switching to derive this class from CENTRON_AMI_HistoryLogConfig.
    //
    public class CENTRON2_MONO_HistoryLogConfig : CENTRON_AMI_HistoryLogConfig
    {
        #region Public Methods

        /// <summary>
        /// Constructor for CENTRON_AMI History Log Config class
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="Offset"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/09/07 KRC 8.00.17
        //
        public CENTRON2_MONO_HistoryLogConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        /// <summary>
        /// Constructor used to get Event Data from the EDL file
        /// </summary>
        /// <param name="EDLBinaryReader"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/05/08 KRC 1.50.00
        //
        public CENTRON2_MONO_HistoryLogConfig(PSEMBinaryReader EDLBinaryReader)
            : base(EDLBinaryReader)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the list of History Log event specfic to the CENTRON_AMI
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 07/01/08 KRC 1.51.01 Several Removed Events that were not being logged
        // 11/24/10 jrf 9.70.09 Updating events displayed for CENTRON II.
        //
        public override List<MFG2048EventItem> HistoryLogEventList
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading History Log Config"));
                    }
                }

                //clear out the list
                m_lstEvents.Clear();

                AddEventItem("ANSI_LOGIN", m_usEvent0_15,
                                (UInt16)(Event_0_15.LOGON_SUCCESSFUL), EventID.LOGON_SUCCESSFUL);
                AddEventItem("ANSI_SECURITY_FAILED", m_usEvent0_15,
                                (UInt16)(Event_0_15.SECURITY_FAILED), EventID.SECURITY_FAILED);
                AddEventItem("BASE_MODE_ERROR", m_usEvent128_143,
                                (UInt16)(Event_128_143.BASE_MODE_ERROR), EventID.BASE_MODE_ERROR);
                AddEventItem("BILLING_SCHED_CHANGE", m_usEvent208_223,
                                (UInt16)(Event_208_223.BILLING_SCHEDULE_CHANGE), EventID.BILLING_SCHEDULE_CHANGE);
                AddEventItem("BILLING_DATA_CLEARED", m_usEvent0_15,
                                (UInt16)(Event_0_15.CLEAR_BILLING_DATA), EventID.CLEAR_BILLING_DATA);
                AddEventItem("BILLING_SCHED_EXPIRED", m_usEvent0_15,
                                (UInt16)(Event_0_15.BILLING_SCHED_EXPIRED), EventID.BILLING_SCHED_EXPIRED);
                AddEventItem("ILLEGAL_CONFIG_ERROR", m_usEvent32_47,
                                (UInt16)(Event_32_47.ILLEGAL_CONFIG_ERROR), EventID.ILLEGAL_CONFIG_ERROR);
                AddEventItem("CRITICAL_PEAK_PRICING", m_usEvent192_207,
                                (UInt16)(Event_192_207.CRITICAL_PEAK_PRICING), EventID.CRITICAL_PEAK_PRICING);
                AddEventItem("DEMAND_RESET", m_usEvent16_31,
                                (UInt16)(Event_16_31.DEMAND_RESET), EventID.DEMAND_RESET);
                AddEventItem("DEMAND_THRESHOLD_EXCEEDED", m_usEvent0_15,
                                (UInt16)(Event_0_15.DEMAND_THRESHOLD_EXCEEDED), EventID.DEMAND_THRESHOLD_EXCEEDED);
                AddEventItem("DEMAND_THRESHOLD_RESTORED", m_usEvent0_15,
                                (UInt16)(Event_0_15.DEMAND_THRESHOLD_RESTORED), EventID.DEMAND_THRESHOLD_RESTORED);
                AddEventItem("DST_TIME_CHANGE", m_usEvent0_15,
                                (UInt16)(Event_0_15.DST_TIME_CHANGE), EventID.DST_TIME_CHANGE);
                AddEventItem("EVENT_CACHE_OVERFLOW", m_usEvent240_255,
                                (UInt16)(Event_240_255.EVENT_CACHE_OVERFLOW), EventID.EVENT_CACHE_OVERFLOW);
                AddEventItem("EVENT_LOG_CLEARED", m_usEvent16_31,
                                (UInt16)(Event_16_31.EVENT_LOG_CLEARED), EventID.EVENT_LOG_CLEARED);
                AddEventItem("EVENT_TAMPER_CLEARED", m_usEvent128_143,
                                (UInt16)(Event_128_143.EVENT_TAMPER_CLEARED), EventID.EVENT_TAMPER_CLEARED);
                AddEventItem("FIRMWARE_DOWNLOAD_ABORTED", m_usEvent176_191,
                                (UInt16)(Event_176_191.FW_DWLD_ABORT), EventID.FW_DWLD_ABORT);
                AddEventItem("HISTORY_LOG_CLEARED", m_usEvent16_31,
                                (UInt16)(Event_16_31.HIST_LOG_CLEARED), EventID.HIST_LOG_CLEARED);
                AddEventItem("INVERSION_TAMPER", m_usEvent48_63,
                                (UInt16)(Event_48_63.INVERSION_TAMPER), EventID.INVERSION_TAMPER);
                AddEventItem("MASS_MEMORY_ERROR", m_usEvent32_47,
                                (UInt16)(Event_32_47.MASS_MEMORY_ERROR), EventID.MASS_MEMORY_ERROR);
                AddEventItem("LOSS_OF_PHASE", m_usEvent32_47,
                                (UInt16)(Event_32_47.LOSS_VOLTAGE_A_OR_LOSS_OF_PHASE), EventID.LOSS_VOLTAGE_A_OR_LOSS_OF_PHASE);
                AddEventItem("LOSS_OF_PHASE_RESTORE", m_usEvent32_47,
                                (UInt16)(Event_32_47.LOSS_OF_PHASE_RESTORE), EventID.LOSS_OF_PHASE_RESTORE);
                AddEventItem("LOW_BATTERY", m_usEvent32_47,
                                (UInt16)(Event_32_47.LOW_BATTERY), EventID.LOW_BATTERY);
                AddEventItem("METER_RECONFIG", m_usEvent32_47,
                               (UInt16)(Event_32_47.DEVICE_REPROGRAMMED), EventID.DEVICE_REPROGRAMMED);
                AddEventItem("PENDING_RECONFIGURE", m_usEvent128_143,
                                (UInt16)(Event_128_143.PENDING_RECONFIGURE), EventID.PENDING_RECONFIGURE);
                AddEventItem("PENDING_TABLE_ERROR_EVENT", m_usEvent240_255,
                                (UInt16)(Event_240_255.PENDING_TABLE_ERROR), EventID.PENDING_TABLE_ERROR);
                AddEventItem("PENDING_TABLE_ACTIVATE_FAIL", m_usEvent160_175,
                                (UInt16)(Event_160_175.PENDING_TABLE_ACTIVATE_FAILED), EventID.PENDING_TABLE_ACTIVATE_FAILED);
                AddEventItem("PENDING_TABLE_CLEAR_FAILED", m_usEvent224_239,
                                (UInt16)(Event_224_239.PENDING_TABLE_CLEAR_FAILED), EventID.PENDING_TABLE_CLEAR_FAILED);
                AddEventItem("PENDING_TABLE_FULL", m_usEvent224_239,
                                (UInt16)(Event_224_239.PENDING_TABLE_FULL), EventID.PENDING_TABLE_FULL);
                AddEventItem("PRIMARY_POWER_DOWN", m_usEvent0_15,
                                (UInt16)(Event_0_15.POWER_OUTAGE), EventID.POWER_OUTAGE);
                AddEventItem("PRIMARY_POWER_UP", m_usEvent0_15,
                                (UInt16)(Event_0_15.POWER_RESTORED), EventID.POWER_RESTORED);
                AddEventItem("RATE_CHANGE", m_usEvent16_31,
                                (UInt16)(Event_16_31.RATE_CHANGE), EventID.RATE_CHANGE);
                AddEventItem("REGISTER_FULL_SCALE", m_usEvent32_47,
                                (UInt16)(Event_32_47.REGISTER_FULL_SCALE), EventID.REGISTER_FULL_SCALE);
                AddEventItem("REMOVAL_TAMPER", m_usEvent48_63,
                                (UInt16)(Event_48_63.REMOVAL_TAMPER), EventID.REMOVAL_TAMPER);
                AddEventItem("REVERSE_ROTATION_DETECTED", m_usEvent48_63,
                                (UInt16)(Event_48_63.REVERSE_POWER_FLOW), EventID.REVERSE_POWER_FLOW);
                AddEventItem("REVERSE_ROTATION_RESTORE", m_usEvent32_47,
                                (UInt16)(Event_32_47.REVERSE_ROTATION_RESTORE), EventID.REVERSE_ROTATION_RESTORE);
                AddEventItem("TOU_SEASON_CHANGED", m_usEvent16_31,
                                (UInt16)(Event_16_31.TOU_SEASON_CHANGED), EventID.TOU_SEASON_CHANGED);
                AddEventItem("SECURITY_EVENT", m_usEvent240_255,
                                (UInt16)(Event_240_255.SECURITY_EVENT), EventID.SECURITY_EVENT);
                AddEventItem("KEY_ROLLOVER_PASS", m_usEvent240_255,
                                (UInt16)(Event_240_255.KEY_ROLLOVER_PASS), EventID.KEY_ROLLOVER_PASS);
                AddEventItem("SIGN_KEY_REPLACE_PROCESSING_PASS", m_usEvent240_255,
                                (UInt16)(Event_240_255.SIGN_KEY_REPLACE_PROCESSING_PASS), EventID.SIGN_KEY_REPLACE_PROCESSING_PASS);
                AddEventItem("SYMMETRIC_KEY_REPLACE_PROCESSING_PASS", m_usEvent240_255,
                                (UInt16)(Event_240_255.SYMMETRIC_KEY_REPLACE_PROCESSING_PASS), EventID.SYMMETRIC_KEY_REPLACE_PROCESSING_PASS);
                AddEventItem("SELF_READ_OCCURRED", m_usEvent16_31,
                                (UInt16)(Event_16_31.SELF_READ_OCCURRED), EventID.SELF_READ_OCCURRED);
                AddEventItem("TABLE_WRITTEN_EVENT", m_usEvent128_143,
                                (UInt16)(Event_128_143.TABLE_WRITTEN), EventID.TABLE_WRITTEN);
                AddEventItem("TIME_ADJUSTMENT_FAILED", m_usEvent240_255,
                               (UInt16)(Event_240_255.TIME_ADJUSTMENT_FAILED), EventID.TIME_ADJUSTMENT_FAILED);
                AddEventItem("TIME_CHANGED", m_usEvent0_15,
                                (UInt16)(Event_0_15.CLOCK_RESET), EventID.CLOCK_RESET);
                AddEventItem("TOU_SCHED_ERROR", m_usEvent32_47,
                                (UInt16)(Event_32_47.TOU_SCHEDULE_ERROR), EventID.TOU_SCHEDULE_ERROR);
                AddEventItem("VOLT_HOUR_BELOW_LOW_THRESHOLD", m_usEvent240_255,
                                (UInt16)(Event_240_255.VOLT_HOUR_BELOW_LOW_THRESHOLD), EventID.VOLT_HOUR_BELOW_LOW_THRESHOLD);
                AddEventItem("VOLT_HOUR_ABOVE_HIGH_THRESHOLD", m_usEvent240_255,
                                (UInt16)(Event_240_255.VOLT_HOUR_ABOVE_HIGH_THRESHOLD), EventID.VOLT_HOUR_ABOVE_HIGH_THRESHOLD);
                AddEventItem("RMS_VOLTAGE_BELOW_LOW_THRESHOLD", m_usEvent240_255,
                                (UInt16)(Event_240_255.RMS_VOLTAGE_BELOW_LOW_THRESHOLD), EventID.RMS_VOLTAGE_BELOW_LOW_THRESHOLD);
                AddEventItem("RMS_VOLTAGE_ABOVE_HIGH_THRESHOLD", m_usEvent240_255,
                                (UInt16)(Event_240_255.RMS_VOLTAGE_ABOVE_HIGH_THRESHOLD), EventID.RMS_VOLTAGE_ABOVE_HIGH_THRESHOLD);

                return m_lstEvents;
            }
        }

        #endregion


        #region Protected Methods

        /// <summary>
        /// Adds an event item to the event list.
        /// </summary>
        /// <param name="strResourceString">The description of the event</param>
        /// <param name="usEventField">The raw data from the meter</param>
        /// <param name="usEventMask">The mask to apply to determine whether or not
        /// the event is enabled</param>
        /// <param name="ID">The ID of the event</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/24/10 jrf 9.70.09		   Created
        protected void AddEventItem(string strResourceString, UInt16 usEventField, UInt16 usEventMask, EventID ID)
        {
            MFG2048EventItem eventItem = GetEventItem(strResourceString, usEventField, usEventMask);
            eventItem.ID = (int)ID;
            m_lstEvents.Add(eventItem);
        }

        #endregion
    }

    /// <summary>
    /// This DemandConfig class handles the reading of the demand config 
    /// block of 2048. The reading of this table in the meter will be implicit.
    /// (read-only)
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 10/02/06 KRC 7.35.00 N/A    Created for OpenWay
    //
    internal class CENTRON2_MONO_DemandConfig : DemandConfig
    {
        #region Constants

        private const int DEMAND_CONFIG_BLOCK_LENGTH = 88;

        #endregion

        #region Definitions


        #endregion

        #region Public Methods

        /// <summary>Constructor</summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/06 KRC 7.35.00 N/A    Created
        // 
        public CENTRON2_MONO_DemandConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset, DEMAND_CONFIG_BLOCK_LENGTH)
        {
        }

        /// <summary>
        /// Writes the m_Data contents to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/11/06 RCG 7.40.00        Created
        // 05/11/07 RCG 8.10.04 2785   Adding back thresholds 2-4

        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of DemandConfig Table ");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandControl = " + m_byDemandControl);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "NumSubIntervals = " + m_byNumSubIntervals);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "IntervalLength = " + m_byIntervalLength);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "TestModeNumSubIntervals = " + m_byTestModeNumSubIntervals);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "TestModeIntervalLength = " + m_byTestModeIntervalLength);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "SchedControl = " + m_SchedControl);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandResetHour = " + m_byDemandResetHour);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandResetMinute = " + m_byDemandResetMinute);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandResetDay = " + m_byDemandResetDay);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition1 = " + m_uiDemandDefinition1);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdSource1 = " + m_uiThresholdSource1);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdLevel1 = " + m_fThresholdLevel1);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdSource2 = " + m_uiThresholdSource2);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdLevel2 = " + m_fThresholdLevel2);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdSource3 = " + m_uiThresholdSource3);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdLevel3 = " + m_fThresholdLevel3);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdSource4 = " + m_uiThresholdSource4);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdLevel4 = " + m_fThresholdLevel4);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "RegisterFullScale = " + m_fRegisterFullScale);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutageLength = " + m_usOutageLength);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ColdLoadPickupTime = " + m_byColdLoadPickupTime);

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of DemandConfig Table ");
            }
            catch (Exception e)
            {
                try
                {
                    m_Logger.WriteException(this, e);
                }
                catch
                {
                    // No exceptions thrown from this debugging method
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of configured demands as LID numbers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override List<uint> Demands
        {
            get
            {
                List<uint> DemandList = new List<uint>();
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Threshold"));
                    }
                }

                DemandList.Add(m_uiDemandDefinition1);

                return DemandList;
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Parses the data for the AMI meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created
        // 05/11/07 RCG 8.10.04 2785   Adding back thresholds 2-4

        protected override void ParseData()
        {
            //Populate the member variables that represent the table
            m_byDemandControl = m_Reader.ReadByte();
            m_byNumSubIntervals = m_Reader.ReadByte();
            m_byIntervalLength = m_Reader.ReadByte();
            m_byTestModeNumSubIntervals = m_Reader.ReadByte();
            m_byTestModeIntervalLength = m_Reader.ReadByte();
            m_SchedControl = m_Reader.ReadByte();
            m_byDemandResetHour = m_Reader.ReadByte();
            m_byDemandResetMinute = m_Reader.ReadByte();
            m_byDemandResetDay = m_Reader.ReadByte();
            m_uiDemandDefinition1 = m_Reader.ReadUInt32();
            m_uiThresholdSource1 = m_Reader.ReadUInt32();
            m_fThresholdLevel1 = m_Reader.ReadSingle();
            m_uiThresholdSource2 = m_Reader.ReadUInt32();
            m_fThresholdLevel2 = m_Reader.ReadSingle();
            m_uiThresholdSource3 = m_Reader.ReadUInt32();
            m_fThresholdLevel3 = m_Reader.ReadSingle();
            m_uiThresholdSource4 = m_Reader.ReadUInt32();
            m_fThresholdLevel4 = m_Reader.ReadSingle();
            m_fRegisterFullScale = m_Reader.ReadSingle();
            m_usOutageLength = m_Reader.ReadUInt16();
            m_byColdLoadPickupTime = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables
        // These should eventually be moved up to the base class

       #endregion

    } // DemandConfig class

     /// <summary>
    /// The DisplayConfig class represents the display configuration data in 2048.
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  10/02/06 KRC 7.35.00 N/A    Created
    //
    public class CENTRON2_MONO_DisplayConfig : DisplayConfig_Shared
    {
        #region Public Methods

        /// <summary>
        /// Constructor for the Display Configuration piece of 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        // 
        public CENTRON2_MONO_DisplayConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        /// <summary>
        /// Constructor for the Display Configuration table - used with file based structure.
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="Offset"></param>
        public CENTRON2_MONO_DisplayConfig(PSEMBinaryReader BinaryReader, ushort Offset)
            : base(BinaryReader, Offset)
        {
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the reference time for the the display items
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/15/07 RCG 8.00.06 N/A    Created

        protected override DateTime ReferenceTime
        {
            get
            {
                return new DateTime(1970, 1, 1);
            }
        }

        #endregion
    }

    	/// <summary>
	/// The TOUConfig class represents the TOU Configuration data block in
	/// table 2048. The TOU portion of the configuration defines the seasons
	/// that are applied across the years of the TOU schedule. Seasons are
	/// applied to years in the CalendarConfig portion of the configuration.
	/// </summary>
	/// 
	// Revision History	
	// MM/DD/YY who Version Issue# Description
	// -------- --- ------- ------ -------------------------------------------
	// 10/18/06 KRC 7.36.00 N/A    Created
	//
    public class CENTRON2_MONO_TOUConfig : TOUConfig
    {
        #region Constants

        private const uint NUM_SUPPORTED_SEASONS = 8; // 1 in Openway
        /// <summary>
        /// Size of the TOU Table
        /// </summary>
        public const ushort TOU_CONFIG_SIZE = 1560; // 50 in Openway, 2 byte chksum
        private const int EVENTS_PER_DAYTYPE = 24; // 6 in Openway
       
        #endregion

        #region public methods
        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Offset of this subtable within table 2048</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/18/06 KRC 7.36.00 N/A    Created
        //
        public CENTRON2_MONO_TOUConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset, TOU_CONFIG_SIZE)
        {
        }

        /// <summary>
        /// Constructor for CENTRON AMI TOU Configuration table used with file based structure
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="Offset"></param>
        public CENTRON2_MONO_TOUConfig(PSEMBinaryReader BinaryReader, ushort Offset)
            : base(BinaryReader, Offset)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the number of Supported Seasons
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Changed to a Public Property

        public override uint NumberOfSupportedSeasons
        {
            get
            {
                return NUM_SUPPORTED_SEASONS;
            }
        }

        /// <summary>
        /// Provides access to the number of Events Per day Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Changed to a Public Property

        public override int EventsPerDayType
        {
            get
            {
                return EVENTS_PER_DAYTYPE;
            }
        }        

        #endregion

        #region Protected Methods
        /// <summary>
        /// Parses the Data during the read
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/23/06 KRC 7.36.00 N/A	Created
        // 
        protected override void ParseData()
        {
            for (int Index = 0; Index < Seasons.Length; Index++)
            {
                Seasons[Index].IsProgrammed = m_Reader.ReadByte();
                Seasons[Index].Daytypes = m_Reader.ReadUInt16();

                for (int DaytypeIndex = 0;
                    DaytypeIndex < Seasons[Index].DayTypesPerSeason; DaytypeIndex++)
                {
                    for (int EventIndex = 0;
                        EventIndex < Seasons[Index].EventsPerDayType; EventIndex++)
                    {
                        Seasons[Index].TimeOfDayEvents[DaytypeIndex,
                                                         EventIndex].Event
                                        = m_Reader.ReadUInt16();
                    }
                }
            }
        } 
        #endregion

    }
    	
    /// <summary>
	/// The CENTRON_AMI_CalendarConfig class represents the Calendar Configuration data 
	/// block in table 2048.
	/// </summary>
	/// 
	// Revision History	
	// MM/DD/YY who Version Issue# Description
	// -------- --- ------- ------ ---------------------------------------
	// 10/18/06 KRC 7.36.00 N/A	Created
	// 
    public class CENTRON2_MONO_CalendarConfig : CalendarConfig
    {
        #region Constants

        /// <summary>
        /// The size of the OpenWay Calendar
        /// </summary>
        public const ushort CENTRON2_MONO_CAL_SIZE = 2231;
        /// <summary>
        /// Number of Years in the OpenWay Calendar
        /// </summary>
        public const byte CENTRON2_MONO_CAL_YEARS = 25;
        private const int EVENTS_PER_YEAR = 44;

        #endregion

        #region public methods

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years this meter can 
        /// support</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/18/06 KRC 7.36.00 N/A	Created
        // 
        public CENTRON2_MONO_CalendarConfig(CPSEM psem, ushort Offset, ushort Size, byte MaxCalYears)
            : base(psem, Offset, Size, MaxCalYears)
        {
        }

        /// <summary>
        /// Calendar ConfiguratioN Construcutor used for file based structure
        /// </summary>
        /// <param name="BinaryReader">The binary Reader containing the data stream</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max celndar years this meter can support</param>
        public CENTRON2_MONO_CalendarConfig(PSEMBinaryReader BinaryReader, ushort Offset, ushort Size, byte MaxCalYears)
            : base(BinaryReader, Offset, Size, MaxCalYears)
        {
        }

        /// <summary>
        /// Clears MOST of the CalendarConfig table.  This is usually done 
        /// prior to reconfiguration, so unused portions don't explicitly have 
        /// to be updated. The Control value is not cleared because it's not
        /// in the TOU schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/16/07 RCG 8.00.19 N/A	Created

        public override void Clear()
        {
            m_CalendarID.Value = 0;
            m_DSTOffset = 0;

            // mcm - DO NOT CLEAR the Control value.  It is not  available
            // in the TOU schedule, so we will use the existing value.

            for (int YearIndex = 0; YearIndex < m_Years.Length; YearIndex++)
            {
                m_Years[YearIndex].Year = 0;

                for (int EventIndex = 0;
                     EventIndex < m_Years[YearIndex].Events.Length; EventIndex++)
                {
                    m_Years[YearIndex].Events[EventIndex].Event =
                        (ushort)AMICalendarEvent.AMICalendarEventType.NOT_USED;
                }
            }
        }

        /// <summary>
        /// Translates the AMICalendarEventType for a Calendar event into an eEventType
        /// for the TOUSchedule class.
        /// </summary>
        /// <returns>The TOU Schedule object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 03/16/07 RCG 8.00.19		    Created
        // 03/11/08 KRC 1.50.03         Move to Table class
        // 12/01/10 DEO 9.70.12         fix for CQ  165070
        
        public override eEventType GetEventType(ushort usEvent) 
        {
            eEventType eType;
            usEvent -= 1;
            switch (usEvent)
            {
                case (ushort)CalendarEvent.CalendarEventType.HOLIDAY:
                    {
                        eType = eEventType.HOLIDAY;
                        break;
                    }
                case (ushort)CalendarEvent.CalendarEventType.SEASON1:
                case (ushort)CalendarEvent.CalendarEventType.SEASON2:
                case (ushort)CalendarEvent.CalendarEventType.SEASON3:
                case (ushort)CalendarEvent.CalendarEventType.SEASON4:
                case (ushort)CalendarEvent.CalendarEventType.SEASON5:
                case (ushort)CalendarEvent.CalendarEventType.SEASON6:
                case (ushort)CalendarEvent.CalendarEventType.SEASON7:
                case (ushort)CalendarEvent.CalendarEventType.SEASON8:
                    {
                        eType = eEventType.SEASON;
                        break;
                    }
                default:
                    {
                        eType = eEventType.NO_EVENT;
                        break;
                    }
            }
            return eType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the Number of Events per Year
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Changed to a Public Property

        public override int EventsPerYear
        {
            get
            {
                return EVENTS_PER_YEAR;
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Parses the data read by the call to Read. This is used so
        /// that we do not try to parse to much data in the CENTRON_AMI
        /// version of the TOUConfig table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/20/06 KRC 7.36.00 N/A    Created

        protected override void ParseData()
        {
            //Populate the member variables that represent the table
            m_CalendarID.Value = m_Reader.ReadUInt16();
            m_Control = m_Reader.ReadByte();  // newly added for DemandReset flag
            m_DSTHour = m_Reader.ReadByte();
            m_DSTMinute = m_Reader.ReadByte();
            m_DSTOffset = m_Reader.ReadByte();

            for (int Index = 0; Index < m_Years.Length; Index++)
            {
                m_Years[Index].Year = m_Reader.ReadByte();

                for (int EventIndex = 0; EventIndex < EventsPerYear; EventIndex++)
                {
                    m_Years[Index].Events[EventIndex] = new AMICalendarEvent(m_Reader.ReadUInt16());
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// The CENTRON_AMI_DSTCalendarConfig class represents the Calendar Configuration data 
    /// block in table 2048.
    /// </summary>
    /// 
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 10/18/06 KRC 7.36.00 N/A	Created
    // 
    public class CENTRON2_MONO_DSTCalendarConfig : CalendarConfig
    {
        #region Constants

        /// <summary>
        /// The size of the CentronII DST Calendar
        /// </summary>
        public const ushort CENTRON2_DSTCALENDAR_SIZE = 128;
        /// <summary>
        /// Number of Years in the CentronII DST Calendar
        /// </summary>
        public const byte CENTRON2_DSTCALENDAR__YEARS = 25;
        /// <summary>
        /// Number of DST Events in the CentronII DST Calendar
        /// </summary>
        private const int CENTRON2_DSTEVENTS_PER_YEAR = 2;
        /// <summary>
        /// DST Calendar Table - MFG 2260
        /// </summary>
        private const int CENTRON2_DSTCALENDAR_TABLE = 2260;
        /// <summary>
        /// DST Calendar Table Offset
        /// </summary>
        private const int CENTRON2_DSTCALENDAR_OFFSET = 16;
       
        #endregion

        #region public methods

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/10 SCW         N/A	   Created
        // 
        public CENTRON2_MONO_DSTCalendarConfig(CPSEM psem)
            : base(psem, CENTRON2_DSTCALENDAR_TABLE, CENTRON2_DSTCALENDAR_OFFSET, CENTRON2_DSTCALENDAR_SIZE, CENTRON2_DSTCALENDAR__YEARS)
        {
        }

        /// <summary>
        /// Calendar ConfiguratioN Construcutor used for file based structure
        /// </summary>
        /// <param name="BinaryReader">The binary Reader containing the data stream</param>
     
        public CENTRON2_MONO_DSTCalendarConfig(PSEMBinaryReader BinaryReader)
            : base(BinaryReader, CENTRON2_DSTCALENDAR_TABLE, CENTRON2_DSTCALENDAR_OFFSET, CENTRON2_DSTCALENDAR_SIZE, CENTRON2_DSTCALENDAR__YEARS)
        {
        }

        /// <summary>
        /// Clears MOST of the CalendarConfig table.  This is usually done 
        /// prior to reconfiguration, so unused portions don't explicitly have 
        /// to be updated. The Control value is not cleared because it's not
        /// in the TOU schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 11/20/10 SCW         N/A	    Created
        // 
        public override void Clear()
        {
            m_CalendarID.Value = 0;
            m_DSTOffset = 0;

            // mcm - DO NOT CLEAR the Control value.  It is not  available
            // in the TOU schedule, so we will use the existing value.

            for (int YearIndex = 0; YearIndex < m_Years.Length; YearIndex++)
            {
                m_Years[YearIndex].Year = 0;

                for (int EventIndex = 0;
                     EventIndex < m_Years[YearIndex].Events.Length; EventIndex++)
                {
                    m_Years[YearIndex].Events[EventIndex].Event =
                        (ushort)AMICalendarEvent.AMICalendarEventType.NOT_USED;
                }
            }
        }

        /// <summary>
        /// Translates the AMICalendarEventType for a Calendar event into an eEventType
        /// for the TOUSchedule class.
        /// </summary>
        /// <returns>The TOU Schedule object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 11/20/10 SCW         N/A	    Created
        //
        public override eEventType GetEventType(ushort usEvent)
        {
            eEventType eType = eEventType.HOLIDAY;

            switch (usEvent)
            {
                case (ushort)AMICalendarEvent.AMICalendarEventType.DST_ON:
                    {
                        eType = eEventType.TO_DST;
                        break;
                    }
                case (ushort)AMICalendarEvent.AMICalendarEventType.DST_OFF:
                    {
                        eType = eEventType.FROM_DST;
                        break;
                    }
                case (ushort)AMICalendarEvent.AMICalendarEventType.NOT_USED:
                    {
                        eType = eEventType.NO_EVENT;
                        break;
                    }
                case (ushort)AMICalendarEvent.AMICalendarEventType.HOLIDAY:
                    {
                        eType = eEventType.HOLIDAY;
                        break;
                    }
            }

            return eType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the Number of Events per Year
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 11/20/10 SCW          N/A	Created
        //
        public override int EventsPerYear
        {
            get
            {
                return CENTRON2_DSTEVENTS_PER_YEAR;
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Parses the data read by the call to Read. This is used so
        /// that we do not try to parse to much data in the CENTRON_AMI
        /// version of the TOUConfig table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/20/10 SCW         N/A	   Created
        // 
        protected override void ParseData()
        {
            //Populate the member variables that represent the table
            m_DSTHour = m_Reader.ReadByte();
            m_DSTMinute = m_Reader.ReadByte();
            m_DSTOffset = m_Reader.ReadByte();

            for (int Index = 0; Index < m_Years.Length; Index++)
            {
                m_Years[Index].Year = m_Reader.ReadByte();

                for (int EventIndex = 0; EventIndex < EventsPerYear; EventIndex++)
                {
                    m_Years[Index].Events[EventIndex] = new AMICalendarEvent(m_Reader.ReadUInt16());
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Class that represents the Metrology Table in OpenWay (Partial Table Implementation)
    /// </summary>
    internal class CENTRON2_MONO_Metrology : ANSISubTable
    {
        #region Definitions

        private const ushort METROLOGY_CONFIG_TBL_SIZE = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="offset">The offset of the table in 2048.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        public CENTRON2_MONO_Metrology(CPSEM psem, ushort offset)
            : base(psem, 2048, offset, METROLOGY_CONFIG_TBL_SIZE)
        {
        }

        /// <summary>
        /// Reads the Coefficients Config block out of 2048
        /// </summary>
        /// <returns>AA PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "CENTRON2_MONO_MetrologyConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                m_iNormalKh = m_Reader.ReadInt16();
            }

            return Result;
        }

        /// <summary>
        /// Writes the Coefficients Config block to table 2048. The member
        /// variables must contain the data that is to be written to the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "MetrologyConfig.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write((short)m_iNormalKh);

            return base.Write();
        }

        /// <summary>
        /// Writes the data to the log file for debugging purposes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created

        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of CoefficientsConfig Table");


                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of CoefficientsConfig Table ");
            }
            catch (Exception e)
            {
                try
                {
                    m_Logger.WriteException(this, e);
                }
                catch
                {
                    // No exceptions thrown from this debugging method
                }
            }
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets the CT Multiplier field in table 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created
        //
        public int NormalKh
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading NormalKh"));
                    }
                }

                return m_iNormalKh;
            }
            set
            {
                m_iNormalKh = value;
            }
        }

        #endregion

        #region Member Variables

        private int m_iNormalKh;

        #endregion Member Variables
    }

    /// <summary>
    /// The mode control table for the AMI meter.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  10/09/06 RCG 7.40.00 N/A    Created

    internal class CENTRON2_MONO_ModeControl : ModeControl
    {
        #region Constants

        /// <summary>Mask for the Daily Self Read Hour</summary>
        public const byte DSRT_HR_MASK = 0x1F;
        /// <summary>Mask for the Daily Self Read Minutes</summary>
        public const byte DSRT_MIN_MASK = 0xE0;

        #endregion

        #region Definitions

        /// <summary>Defintion for the Daily Self Read Minutes</summary>
        public enum DSRTMinutes : byte
        {
            /// <summary>0 Minutes</summary>
            MIN_0 = 0x00,
            /// <summary>10 Minutes</summary>
            MIN_10 = 0x20,
            /// <summary>15 Minutes</summary>
            MIN_15 = 0x40,
            /// <summary>20 Minutes</summary>
            MIN_20 = 0x60,
            /// <summary>30 Minutes</summary>
            MIN_30 = 0x80,
            /// <summary>40 Minutes</summary>
            MIN_40 = 0xA0,
            /// <summary>45 Minutes</summary>
            MIN_45 = 0xC0,
            /// <summary>50 Minutes</summary>
            MIN_50 = 0xE0,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device</param>
        /// <param name="Offset">The offset of the sub table in 2048</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public CENTRON2_MONO_ModeControl(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Daily Self Read Time value as a string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created
        //  02/21/08 KRC 10.0.0         Break logic out to static method so it can be used from EDFile
        //
        public string DailySelfReadTime
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                
                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Daily Self Read Time"));
                    }
                }
                                
                return DetermineDailySelfRead(m_byDisableSwitches);
            }
        }

        /// <summary>
        /// Method that take shte Daily Self Read Byte and returns a string
        ///   containing the Daily Self Read time in human readable format.
        /// </summary>
        /// <param name="byDailySelfRead">The Byte from the Meter that has the values</param>
        /// <returns>string - Daily Self Read Time</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/12/08 RCG 1.50.35 00116049 Fixed several issues to make this return a correct value.

        public static string DetermineDailySelfRead(byte byDailySelfRead)
        {
            DateTime dtSelfReadTime;
            DSRTMinutes Minutes;
            string strSelfReadTime = "Not Enabled";

            // The diable switches byte has been replaced by the daily self read time on the
            // AMI meter
            if (byDailySelfRead != 0)
            {
                dtSelfReadTime = new DateTime();

                // Add the hours
                dtSelfReadTime = dtSelfReadTime.AddHours(byDailySelfRead & DSRT_HR_MASK);

                // Determine the number of minutes
                Minutes = (DSRTMinutes)(byDailySelfRead & DSRT_MIN_MASK);

                switch (Minutes)
                {
                    case DSRTMinutes.MIN_0:
                        {
                            break;
                        }
                    case DSRTMinutes.MIN_10:
                        {
                            dtSelfReadTime = dtSelfReadTime.AddMinutes(10);
                            break;
                        }
                    case DSRTMinutes.MIN_15:
                        {
                            dtSelfReadTime = dtSelfReadTime.AddMinutes(15);
                            break;
                        }
                    case DSRTMinutes.MIN_20:
                        {
                            dtSelfReadTime = dtSelfReadTime.AddMinutes(20);
                            break;
                        }
                    case DSRTMinutes.MIN_30:
                        {
                            dtSelfReadTime = dtSelfReadTime.AddMinutes(30);
                            break;
                        }
                    case DSRTMinutes.MIN_40:
                        {
                            dtSelfReadTime = dtSelfReadTime.AddMinutes(40);
                            break;
                        }
                    case DSRTMinutes.MIN_45:
                        {
                            dtSelfReadTime = dtSelfReadTime.AddMinutes(45);
                            break;
                        }
                    case DSRTMinutes.MIN_50:
                        {
                            dtSelfReadTime = dtSelfReadTime.AddMinutes(50);
                            break;
                        }
                }

                strSelfReadTime = dtSelfReadTime.ToShortTimeString();
            }

            return strSelfReadTime;
        }

        #endregion
    }

    /// <summary>
    /// CalendarEvent class for the AMI meter. This is overriden to account for the
    /// differences in the event types for the AMI meter.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 03/16/07 RCG 8.00.19 N/A    Created

    internal class MaxImageCalendarEvent : CalendarEvent
    {
        #region Constants

        private const ushort TYPE_MASK = 0x000F;
        private const ushort CLEAR_TYPE = 0xFFF0;

        #endregion

        #region Definitions

        /// <summary>
        /// The list of Calendar event types for the AMI meter. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A    Created

        public enum MaxImageCalendarEventType : ushort
        {
            NOT_USED = 0x0000,
            DST_ON = 0x0001,
            DST_OFF = 0x0002,
            HOLIDAY = 0x0003,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default Constructor.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A    Created

        public MaxImageCalendarEvent()
            : this(0)
        {
        }

        /// <summary>
        /// Constructor. Takes the event data bit field as a paramater.
        /// </summary>
        /// <param name="usEvent">The event data from the Calendar Config in 2048</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A    Created

        public MaxImageCalendarEvent(ushort usEvent)
            : base(usEvent)
        {
        }

        /// <summary>
        /// Returns true if this event is a ToDST or FromDST event
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A    Created

        public override bool IsDST()
        {
            if ((ushort)MaxImageCalendarEventType.DST_ON != (m_usEvent & TYPE_MASK) ||
                (ushort)MaxImageCalendarEventType.DST_OFF != (m_usEvent & TYPE_MASK))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if this event is a season change event
        /// </summary>
        /// <returns>
        /// Always returns false since the AMI meter does not support Season
        /// change events.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A    Created

        public override bool IsSeason()
        {
            return false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event type for this event.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A    Created

        public override byte Type
        {
            get
            {
                return base.Type;
            }
            set
            {
                if (value <= (ushort)MaxImageCalendarEventType.HOLIDAY)
                {
                    // Clear then set the event bits
                    m_usEvent = (ushort)(m_usEvent & CLEAR_TYPE);
                    m_usEvent = (ushort)(m_usEvent | value);
                }
            }
        }

        /// <summary>
        /// Gets the event type as a string
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A    Created

        public override string TranslatedType
        {
            get
            {
                string Type = "";

                switch (m_usEvent & TYPE_MASK)
                {
                    case (ushort)MaxImageCalendarEventType.NOT_USED:
                    {
                        Type = "NOT_USED";
                        break;
                    }
                    case (ushort)MaxImageCalendarEventType.DST_ON:
                    {
                        Type = "DST_ON";
                        break;
                    }
                    case (ushort)MaxImageCalendarEventType.DST_OFF:
                    {
                        Type = "DST_OFF";
                        break;
                    }
                    case (ushort)MaxImageCalendarEventType.HOLIDAY:
                    {
                        Type = "HOLIDAY";
                        break;
                    }
                    default:
                    {
                        Type = "invalid";
                        break;
                    }
                }

                return Type;
            }
        }

        #endregion
    }

    /// <summary>
    /// This CENTRON OpenWay IO Config class handles the reading of the IO config 
    /// block of 2048. The reading of this table in the meter will be implicit 
    /// while the writing of this table will need to be explicitly called.  
    /// (read/write)	
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 04/21/09 jrf 2.20.02 N/A    Created.
    //
    public class CENTRON2_MONO_IOConfig : IOConfig
    {
        #region Constants

        /// <summary>
        /// Pulse weight mulitplier.  Pulse weight is stored in units of 0.01.
        /// </summary>
        private const float PULSE_WEIGHT_MULTIPLIER = 0.01f;

        #endregion

        #region Public Methods

        /// <summary>Constructor.</summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/21/09 jrf 2.20.02 N/A    Created.
        // 
        public CENTRON2_MONO_IOConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        /// <summary>
        /// Constructor used for file based structures.
        /// </summary>
        /// <param name="EDLBinaryReader"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 04/23/09 jrf 2.20.02 N/A    Created.
        //
        public CENTRON2_MONO_IOConfig(PSEMBinaryReader EDLBinaryReader)
            : base(EDLBinaryReader)
        {
        }

        #endregion


        #region Protected Methods

        /// <summary>
        /// This method retrieves the KYZ configuration. 
        /// </summary>
        /// <param name="KYZConfig">A reference to the KYZ configuration object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 jrf 2.20.02 N/A    Created
        //
        protected override void RetrieveKYZConfiguration(ref KYZData KYZConfig)
        {
            if (0 == m_byOutputCh1TestMode)
            {
                KYZConfig.DisableOutputInTestMode = true;
            }
            else
            {
                KYZConfig.DisableOutputInTestMode = false;
            }
            
            KYZConfig.KYZ1OutputType = m_byOutputCh1Type;
            KYZConfig.KYZ1EnergyID = m_byOutputCh1Energy;
            KYZConfig.KYZ1EventID = m_byOutputCh1Event;
            KYZConfig.KYZ1PulseWeight = (float)m_uiOutputCh1PulseWeight * PULSE_WEIGHT_MULTIPLIER;
            KYZConfig.KYZ1PulseWidth = m_uiOutputCh1PulseWidth;

            KYZConfig.KYZ2OutputType = m_byOutputCh2Type;
            KYZConfig.KYZ2EnergyID = m_byOutputCh2Energy;
            KYZConfig.KYZ2EventID = m_byOutputCh2Event;
            KYZConfig.KYZ2PulseWeight = (float)m_uiOutputCh2PulseWeight * PULSE_WEIGHT_MULTIPLIER;
            KYZConfig.KYZ2PulseWidth = m_uiOutputCh2PulseWidth;

            KYZConfig.LC1OutputType = m_byOutputCh3Type;
            KYZConfig.LC1EventID = m_byOutputCh3Event;
            KYZConfig.LC1PulseWidth = m_uiOutputCh3PulseWidth;
            
        }

        /// <summary>
        /// This method populates the KYZ configuration. 
        /// </summary>
        /// <param name="KYZConfig">A KYZ configuration object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 jrf 2.20.02 N/A    Created
        // 04/01/10 KGO 2.40.31 151980 Corrected floating point accuracy issue
        //
        protected override void PopulateKYZConfiguration(KYZData KYZConfig)
        {
            byte byOutputTestMode = 0;
            float fltPulseWeight = 0.0f;

            if (false == KYZConfig.DisableOutputInTestMode)
            {
                byOutputTestMode = 1;
            }
            
            m_byOutputCh1Type = KYZConfig.KYZ1OutputType;
            m_byOutputCh1Event = KYZConfig.KYZ1EventID;
            m_byOutputCh1Energy = KYZConfig.KYZ1EnergyID;
            m_uiOutputCh1PulseWidth = KYZConfig.KYZ1PulseWidth;
            fltPulseWeight = KYZConfig.KYZ1PulseWeight / PULSE_WEIGHT_MULTIPLIER + 0.5f;
            m_uiOutputCh1PulseWeight = (UInt16)fltPulseWeight;
            m_byOutputCh1TestMode = byOutputTestMode;

            m_byOutputCh2Type = KYZConfig.KYZ2OutputType;
            m_byOutputCh2Event = KYZConfig.KYZ2EventID;
            m_byOutputCh2Energy = KYZConfig.KYZ2EnergyID;
            m_uiOutputCh2PulseWidth = KYZConfig.KYZ2PulseWidth;
            fltPulseWeight = KYZConfig.KYZ2PulseWeight / PULSE_WEIGHT_MULTIPLIER + 0.5f;
            m_uiOutputCh2PulseWeight = (UInt16)fltPulseWeight;
            m_byOutputCh2TestMode = byOutputTestMode;

            m_byOutputCh3Type = KYZConfig.LC1OutputType;
            m_byOutputCh3Event = KYZConfig.LC1EventID;
            m_byOutputCh3Energy = 0;
            m_uiOutputCh3PulseWidth = KYZConfig.LC1PulseWidth;
            m_uiOutputCh3PulseWeight = 0;
            m_byOutputCh3TestMode = byOutputTestMode;
        }

        #endregion

    } // CENTRON_AMI_IOConfig class

}
