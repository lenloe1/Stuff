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
//                              Copyright © 2006 - 2016
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
    // 
    internal class CTable2048_OpenWay : CTable2048
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
        public CTable2048_OpenWay(CPSEM psem)
            : base(psem)
        {
            m_DemandConfig = new CENTRON_AMI_DemandConfig(psem, m_2048Header.DemandOffset);
            m_CoefficientsConfig = new CoefficientsConfig(psem, m_2048Header.CoefficientsOffset);
            m_ConstantsConfig = new ConstantsConfig(psem,
                        m_2048Header.ConstantsOffset,
                        ConstantsConfig.OPENWAY_CONSTANTS_CONFIG_LENGTH);
            m_DisplayConfig = new CENTRON_AMI_DisplayConfig(psem, m_2048Header.DisplayOffset);

            if (0 != m_2048Header.TOUOffset)
            {
                m_TOUConfig = new CENTRON_AMI_TOUConfig(psem, m_2048Header.TOUOffset);
            }
            else
            {
                m_TOUConfig = null;
            }

            if (0 != m_2048Header.CalendarOffset)
            {
                m_CalendarConfig = new CENTRON_AMI_CalendarConfig(psem, m_2048Header.CalendarOffset,
                    CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_SIZE, CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_YEARS);
            }
            else
            {
                m_CalendarConfig = null;
            }

            m_ModeControl = new CENTRON_AMI_ModeControl(psem, m_2048Header.ModeControlOffset);
            m_BillingSchedConfig = new BillingSchedConfig(psem, m_2048Header.BillingSchedOffset,
                    BillingSchedConfig.AMI_BILLING_SCHED_CONFIG_LENGTH);

            m_HistoryLogConfig = new CENTRON_AMI_HistoryLogConfig(psem, m_2048Header.HistoryLogOffset);

            if (0 != m_2048Header.CPCOffset)
            {
                m_MetrologyTable = new CENTRON_AMI_Metrology(m_PSEM, m_2048Header.CPCOffset);
            }

            if (0 != m_2048Header.IOOffset)
            {
                m_IOConfig = new CENTRON_AMI_IOConfig(m_PSEM, m_2048Header.IOOffset);
            }

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property to return Metrology Table
        /// </summary>
        public CENTRON_AMI_Metrology MetrologyTable
        {
            get
            {
                return m_MetrologyTable;
            }
        }

        #endregion

        #region Members

        private CENTRON_AMI_Metrology m_MetrologyTable;

        #endregion
    }

    /// <summary>
    /// Class that represents the history log configuration data stored in table 2048
    /// for the CENTRON_AMI meter.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 03/09/07 RCG 8.00.17 N/A    Created
    // 04/03/07 AF  8.00.24 2675   Added SITESCAN_ERROR, which is in a different
    //                             bit position for OpenWay meters.  
    //                             SITESCAN_OR_PENDING_TABLE_CLEAR is the pending
    //                             table cleared event for AMI
    //
    public class CENTRON_AMI_HistoryLogConfig : HistoryLogConfig
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
        public CENTRON_AMI_HistoryLogConfig(CPSEM psem, ushort Offset)
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
        public CENTRON_AMI_HistoryLogConfig(PSEMBinaryReader EDLBinaryReader)
            : base(EDLBinaryReader)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the list of History Log event specfic to the CENTRON_AMI
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version   Issue#        Description
        // -------- --- -------   ------        -------------------------------------------
        // 07/01/08 KRC 1.51.01                 Several Removed Events that were not being logged
        // 03/10/11 jrf 2.50.08                 Adding CPP Event.
        // 08/12/11 jrf 2.52.02   TREQ2709      Changing Register Firmware Download Status event to
        //                                         Firmware Download Event Log Full event.
        // 03/12/12 jrf 2.53.49   192582/192583 Adding missing events to the list.
        // 09/20/12 jrf 2.70.18   TQ6658        Added magnetic tamper events.
        // 06/13/13 AF  2.80.37   TR7477        Added the current threshold exceeded event.
        // 11/18/13 jrf 3.50.06   TQ 9482       Added TOU Season Changed event.
        // 02/04/16 PGH 4.50.226  RTT556309     Added Temperature events
        // 04/21/16 AF  4.50.252  WR604349      Changed HAN_LOAD_CONTROL_EVENT_SENT to ERT_242_COMMAND_REQUEST
        // 05/12/16 MP  4.50.266  WR685323      Added On Demand Periodic Read to list
        // 05/19/16 AF  4.50.270  WR685741      Corrected the resource string for PENDING_TABLE_ERROR
        // 05/20/16 MP  4.50.270   WR685690     Added support for EVENT_HARDWARE_ERROR_DETECTION
        // 07/12/16 MP  4.70.7    WR688986      Changed how event descriptions were accessed
        // 07/18/16 MP  4.70.8    WR600059      Added definition for event 137 (NETWORK_TIME_UNAVAILABLE)
        // 07/21/16 MP  4.70.9    WR701234      Changed VOLT_HOUR_ABOVE_THRESHOLD. Was previously a copy paste error and
        //                                          was being added twice
        // 07/29/16 MP   4.70.11  WR704220   Added support for events 213 and 214 (WRONG_CONFIG_CRC and CHECK_CONFIG_CRC)
        // 10/26/16 jrf	4.70.28   WR230427      Added missing event PERIODIC_READ (125).
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

                //build the event list
                // Add Event 0 - 15 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.PRIMARY_POWER_DOWN), m_usEvent0_15, (UInt16)(Event_0_15.PRIMARY_POWER_DOWN));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.PRIMARY_POWER_UP), m_usEvent0_15, (UInt16)(Event_0_15.PRIMARY_POWER_UP));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.BILLING_DATA_CLEARED), m_usEvent0_15, (UInt16)(Event_0_15.BILLING_DATA_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.BILLING_SCHED_EXPIRED), m_usEvent0_15, (UInt16)(Event_0_15.BILLING_SCHED_EXPIRED));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.CLOCK_RESET), m_usEvent0_15, (UInt16)(Event_0_15.CLOCK_RESET));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.SECURITY_FAILED), m_usEvent0_15, (UInt16)(Event_0_15.SECURITY_FAILED));

                // Add Event 16 - 31 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.HIST_LOG_CLEARED), m_usEvent16_31, (UInt16)(Event_16_31.HIST_LOG_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.DEMAND_RESET), m_usEvent16_31, (UInt16)(Event_16_31.DEMAND_RESET));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.SELF_READ_OCCURRED), m_usEvent16_31, (UInt16)(Event_16_31.SELF_READ_OCCURRED));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.TOU_SEASON_CHANGED), m_usEvent16_31, (UInt16)(Event_16_31.TOU_SEASON_CHANGED));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.PENDING_TABLE_ACTIVATION), m_usEvent16_31, (UInt16)(Event_16_31.PENDING_TABLE_ACTIVATION));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.PENDING_TABLE_CLEAR), m_usEvent16_31, (UInt16)(Event_16_31.PENDING_TABLE_CLEAR));

                // Add Event 32 - 47 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.METER_REPROGRAMMED), m_usEvent32_47, (UInt16)(Event_32_47.METER_REPROGRAMMED));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.ILLEGAL_CONFIG_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.ILLEGAL_CONFIG_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.CPC_COMM_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.CPC_COMM_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.TOU_SCHEDULE_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.TOU_SCHEDULE_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.MASS_MEMORY_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.MASS_MEMORY_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.LOW_BATTERY), m_usEvent32_47, (UInt16)(Event_32_47.LOW_BATTERY));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.REGISTER_FULL_SCALE), m_usEvent32_47, (UInt16)(Event_32_47.REGISTER_FULL_SCALE));

                // Add Event 48 - 63 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.REVERSE_POWER_FLOW), m_usEvent48_63, (UInt16)(Event_48_63.REVERSE_POWER_FLOW));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.INVERSION_TAMPER), m_usEvent48_63, (UInt16)(Event_48_63.INVERSION_TAMPER));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.REMOVAL_TAMPER), m_usEvent48_63, (UInt16)(Event_48_63.REMOVAL_TAMPER));

                // Add Event 64 - 79 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_64_79), Event_64_79.REG_DWLD_FAILED), m_usEvent64_79, (UInt16)(Event_64_79.REG_DWLD_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_64_79), Event_64_79.REG_DWLD_SUCCEEDED), m_usEvent64_79, (UInt16)(Event_64_79.REG_DWLD_SUCCEEDED));
                AddEventItem(Enum.GetName(typeof(Event_64_79), Event_64_79.RFLAN_DWLD_SUCCEEDED), m_usEvent64_79, (UInt16)(Event_64_79.RFLAN_DWLD_SUCCEEDED));
                AddEventItem(Enum.GetName(typeof(Event_64_79), Event_64_79.ZIGBEE_DWLD_SUCCEEDED), m_usEvent64_79, (UInt16)(Event_64_79.ZIGBEE_DWLD_SUCCEEDED));
                AddEventItem(Enum.GetName(typeof(Event_64_79), Event_64_79.METER_FW_DWLD_SUCCEDED), m_usEvent64_79, (UInt16)(Event_64_79.METER_FW_DWLD_SUCCEDED));
                AddEventItem(Enum.GetName(typeof(Event_64_79), Event_64_79.METER_DWLD_FAILED), m_usEvent64_79, (UInt16)(Event_64_79.METER_DWLD_FAILED));

                // Add Event 80 - 95 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_80_95), Event_80_95.ZIGBEE_DWLD_FAILED), m_usEvent80_95, (UInt16)(Event_80_95.ZIGBEE_DWLD_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_80_95), Event_80_95.RFLAN_DWLD_FAILED), m_usEvent80_95, (UInt16)(Event_80_95.RFLAN_DWLD_FAILED));


                // Add Event 96 - 111 bitfield items
                //AddEventItem("RESET_COUNTERS", m_usEvent96_111,
                //                (UInt16)(Event_96_111.RESET_COUNTERS));

                // Add Event 112 - 127 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_112_127), Event_112_127.FATAL_ERROR), m_usEvent112_127, (UInt16)(Event_112_127.FATAL_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_112_127), Event_112_127.PERIODIC_READ), m_usEvent112_127, (UInt16)(Event_112_127.PERIODIC_READ));
                AddEventItem(Enum.GetName(typeof(Event_112_127), Event_112_127.SERVICE_LIMITING_ACTIVE_TIER_CHANGED), m_usEvent112_127, (UInt16)(Event_112_127.SERVICE_LIMITING_ACTIVE_TIER_CHANGED));

                // Add Event 128 - 143
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.TABLE_WRITTEN), m_usEvent128_143, (UInt16)(Event_128_143.TABLE_WRITTEN));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.BASE_MODE_ERROR), m_usEvent128_143, (UInt16)(Event_128_143.BASE_MODE_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.PENDING_RECONFIGURE), m_usEvent128_143, (UInt16)(Event_128_143.PENDING_RECONFIGURE));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.MAGNETIC_TAMPER_DETECTED), m_usEvent128_143, (UInt16)(Event_128_143.MAGNETIC_TAMPER_DETECTED));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.MAGNETIC_TAMPER_CLEARED), m_usEvent128_143, (UInt16)(Event_128_143.MAGNETIC_TAMPER_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.NETWORK_TIME_UNAVAILABLE), m_usEvent128_143, (UInt16)(Event_128_143.NETWORK_TIME_UNAVAILABLE));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.CTE_EVENT), m_usEvent128_143, (UInt16)(Event_128_143.CTE_EVENT));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.EVENT_TAMPER_CLEARED), m_usEvent128_143, (UInt16)(Event_128_143.EVENT_TAMPER_CLEARED));

                // Add Event 144 - 159
                AddEventItem(Enum.GetName(typeof(Event_144_159), Event_144_159.LAN_HAN_LOG_RESET), m_usEvent144_159, (UInt16)(Event_144_159.LAN_HAN_LOG_RESET));

                // Add Event 160 - 175 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.PENDING_TABLE_ACTIVATE_FAIL), m_usEvent160_175, (UInt16)(Event_160_175.PENDING_TABLE_ACTIVATE_FAIL));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_DEVICE_STATUS_CHANGE), m_usEvent160_175, (UInt16)(Event_160_175.HAN_DEVICE_STATUS_CHANGE));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.ERT_242_COMMAND_REQUEST), m_usEvent160_175, (UInt16)(Event_160_175.ERT_242_COMMAND_REQUEST));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_LOAD_CONTROL_EVENT_STATUS), m_usEvent160_175, (UInt16)(Event_160_175.HAN_LOAD_CONTROL_EVENT_STATUS));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_LOAD_CONTROL_EVENT_OPT_OUT), m_usEvent160_175, (UInt16)(Event_160_175.HAN_LOAD_CONTROL_EVENT_OPT_OUT));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_MESSAGING_EVENT), m_usEvent160_175, (UInt16)(Event_160_175.HAN_MESSAGING_EVENT));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.HAN_DEVICE_ADDED_OR_REMOVED), m_usEvent160_175, (UInt16)(Event_160_175.HAN_DEVICE_ADDED_OR_REMOVED));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.REG_DWLD_INITIATED), m_usEvent160_175, (UInt16)(Event_160_175.REG_DWLD_INITIATED));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.RFLAN_DWLD_INITIATED), m_usEvent160_175, (UInt16)(Event_160_175.RFLAN_DWLD_INITIATED));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.ZIGBEE_DWLD_INITIATED), m_usEvent160_175, (UInt16)(Event_160_175.ZIGBEE_DWLD_INITIATED));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.REG_DWLD_INITIATION_FAILED), m_usEvent160_175, (UInt16)(Event_160_175.REG_DWLD_INITIATION_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.RFLAN_DWLD_INITIATION_FAILED), m_usEvent160_175, (UInt16)(Event_160_175.RFLAN_DWLD_INITIATION_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.ZIGBEE_DWLD_INITIATION_FAILED), m_usEvent160_175, (UInt16)(Event_160_175.ZIGBEE_DWLD_INITIATION_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_160_175), Event_160_175.FW_DWLD_EVENT_LOG_FULL), m_usEvent160_175, (UInt16)(Event_160_175.FW_DWLD_EVENT_LOG_FULL));

                // Add Event 176 - 191 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.RFLAN_FW_DWLD_STATUS), m_usEvent176_191, (UInt16)(Event_176_191.RFLAN_FW_DWLD_STATUS));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.ZIGBEE_FW_DWLD_STATUS), m_usEvent176_191, (UInt16)(Event_176_191.ZIGBEE_FW_DWLD_STATUS));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.REG_DWLD_ALREADY_ACTIVE), m_usEvent176_191, (UInt16)(Event_176_191.REG_DWLD_ALREADY_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.RFLAN_DWLD_ALREADY_ACTIVE), m_usEvent176_191, (UInt16)(Event_176_191.RFLAN_DWLD_ALREADY_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.EXTENDED_OUTAGE_RECOVERY_MODE_ENTERED), m_usEvent176_191, (UInt16)(Event_176_191.EXTENDED_OUTAGE_RECOVERY_MODE_ENTERED));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.THIRD_PARTY_HAN_FW_DWLD_STATUS), m_usEvent176_191, (UInt16)(Event_176_191.THIRD_PARTY_HAN_FW_DWLD_STATUS));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.REMOTE_CONNECT_FAILED), m_usEvent176_191, (UInt16)(Event_176_191.REMOTE_CONNECT_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.REMOTE_DISCONNECT_FAILED), m_usEvent176_191, (UInt16)(Event_176_191.REMOTE_DISCONNECT_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.REMOTE_DISCONNECT_RELAY_ACTIVATED), m_usEvent176_191, (UInt16)(Event_176_191.REMOTE_DISCONNECT_RELAY_ACTIVATED));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.REMOTE_CONNECT_RELAY_ACTIVATED), m_usEvent176_191, (UInt16)(Event_176_191.REMOTE_CONNECT_RELAY_ACTIVATED));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.REMOTE_CONNECT_RELAY_INITIATED), m_usEvent176_191, (UInt16)(Event_176_191.REMOTE_CONNECT_RELAY_INITIATED));
                AddEventItem(Enum.GetName(typeof(Event_176_191), Event_176_191.TABLE_CONFIGURATION), m_usEvent176_191, (UInt16)(Event_176_191.TABLE_CONFIGURATION));
                //AddEventItem("ZIGBEE_DL_ALREADY_ACTIVE", m_usEvent176_191,(UInt16)(Event_176_191.ZIGBEE_DWLD_ALREADY_ACTIVE));
                //AddEventItem("RFLAN_DL_TERMINATED", m_usEvent176_191,(UInt16)(Event_176_191.RFLAN_DWLD_TERMINATED));
                //AddEventItem("ZIGBEE_DL_TERMINATED", m_usEvent176_191,(UInt16)(Event_176_191.ZIGBEE_DWLD_TERMINATED));

                // Add Event 192 - 207 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_192_207), Event_192_207.CPP_EVENT), m_usEvent192_207, (UInt16)(Event_192_207.CPP_EVENT));
                AddEventItem(Enum.GetName(typeof(Event_192_207), Event_192_207.VOLT_HOUR_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL), m_usEvent192_207, (UInt16)(Event_192_207.VOLT_HOUR_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL));
                AddEventItem(Enum.GetName(typeof(Event_192_207), Event_192_207.VOLT_HOUR_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL), m_usEvent192_207, (UInt16)(Event_192_207.VOLT_HOUR_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL));
                AddEventItem(Enum.GetName(typeof(Event_192_207), Event_192_207.RMS_VOLTAGE_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL), m_usEvent192_207, (UInt16)(Event_192_207.RMS_VOLTAGE_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL));
                AddEventItem(Enum.GetName(typeof(Event_192_207), Event_192_207.RMS_VOLTAGE_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL), m_usEvent192_207, (UInt16)(Event_192_207.RMS_VOLTAGE_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL));

                // Add Event 208 - 223 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_208_223), Event_208_223.WRONG_CONFIG_CRC), m_usEvent208_223, (UInt16)(Event_208_223.WRONG_CONFIG_CRC));
                AddEventItem(Enum.GetName(typeof(Event_208_223), Event_208_223.CHECK_CONFIG_CRC), m_usEvent208_223, (UInt16)(Event_208_223.CHECK_CONFIG_CRC));
                AddEventItem(Enum.GetName(typeof(Event_208_223), Event_208_223.EVENT_HARDWARE_ERROR_DETECTION), m_usEvent208_223, (UInt16)(Event_208_223.EVENT_HARDWARE_ERROR_DETECTION));
                AddEventItem(Enum.GetName(typeof(Event_208_223), Event_208_223.TEMPERATURE_EXCEEDS_THRESHOLD1), m_usEvent208_223, (UInt16)(Event_208_223.TEMPERATURE_EXCEEDS_THRESHOLD1));

                // Add Event 224 - 239 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.TEMPERATURE_EXCEEDS_THRESHOLD2), m_usEvent224_239, (UInt16)(Event_224_239.TEMPERATURE_EXCEEDS_THRESHOLD2));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.TEMPERATURE_RETURNED_TO_NORMAL), m_usEvent224_239, (UInt16)(Event_224_239.TEMPERATURE_RETURNED_TO_NORMAL));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.NETWORK_HUSH_STARTED), m_usEvent224_239, (UInt16)(Event_224_239.NETWORK_HUSH_STARTED));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.LOAD_VOLT_PRESENT), m_usEvent224_239, (UInt16)(Event_224_239.LOAD_VOLT_PRESENT));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.PENDING_TABLE_CLEAR_FAIL), m_usEvent224_239, (UInt16)(Event_224_239.PENDING_TABLE_CLEAR_FAIL));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.FIRMWARE_PENDING_TABLE_FULL), m_usEvent224_239, (UInt16)(Event_224_239.FIRMWARE_PENDING_TABLE_FULL));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.FIRMWARE_PENDING_TABLE_HEADER_SWAPPED), m_usEvent224_239, (UInt16)(Event_224_239.FIRMWARE_PENDING_TABLE_HEADER_SWAPPED));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.C12_22_DEREGISTRATION_ATTEMPT), m_usEvent224_239, (UInt16)(Event_224_239.C12_22_DEREGISTRATION_ATTEMPT));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.C12_22_DEREGISTERED), m_usEvent224_239, (UInt16)(Event_224_239.C12_22_DEREGISTERED));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.C12_22_REGISTRATION_ATTEMPT), m_usEvent224_239, (UInt16)(Event_224_239.C12_22_REGISTRATION_ATTEMPT));
                AddEventItem(Enum.GetName(typeof(Event_224_239), Event_224_239.C12_22_REGISTERED), m_usEvent224_239, (UInt16)(Event_224_239.C12_22_REGISTERED));
                //AddEventItem("C12_22_RFLAN_CELL_ID_CHANGE", m_usEvent224_239, (UInt16)(Event_224_239.C12_22_RFLAN_CELL_ID_CHANGE));
                //AddEventItem("C12_22_REGISTRATION_ATTEMPT", m_usEvent224_239,(UInt16)(Event_224_239.C12_22_REGISTRATION_ATTEMPT));
                //AddEventItem("C12_22_REGISTERED", m_usEvent224_239,(UInt16)(Event_224_239.C12_22_REGISTERED));

                // Add Event 240 - 255 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.TIME_ADJUSTMENT_FAILED), m_usEvent240_255, (UInt16)(Event_240_255.TIME_ADJUSTMENT_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.EVENT_CACHE_OVERFLOW), m_usEvent240_255, (UInt16)(Event_240_255.EVENT_CACHE_OVERFLOW));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.EVENT_GENERIC_HISTORY_EVENT), m_usEvent240_255, (UInt16)(Event_240_255.EVENT_GENERIC_HISTORY_EVENT));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.ON_DEMAND_PERIODIC_READ), m_usEvent240_255, (UInt16)(Event_240_255.ON_DEMAND_PERIODIC_READ));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.RMS_VOLTAGE_BELOW_LOW_THRESHOLD), m_usEvent240_255, (UInt16)(Event_240_255.RMS_VOLTAGE_BELOW_LOW_THRESHOLD));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.VOLT_RMS_ABOVE_THRESHOLD), m_usEvent240_255, (UInt16)(Event_240_255.VOLT_RMS_ABOVE_THRESHOLD));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.VOLT_HOUR_BELOW_LOW_THRESHOLD), m_usEvent240_255, (UInt16)(Event_240_255.VOLT_HOUR_BELOW_LOW_THRESHOLD));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.VOLT_HOUR_ABOVE_THRESHOLD), m_usEvent240_255, (UInt16)(Event_240_255.VOLT_HOUR_ABOVE_THRESHOLD));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.PENDING_TABLE_ERROR), m_usEvent240_255, (UInt16)(Event_240_255.PENDING_TABLE_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.SECURITY_EVENT), m_usEvent240_255, (UInt16)(Event_240_255.SECURITY_EVENT));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.KEY_ROLLOVER_PASS), m_usEvent240_255, (UInt16)(Event_240_255.KEY_ROLLOVER_PASS));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.SIGN_KEY_REPLACE_PROCESSING_PASS), m_usEvent240_255, (UInt16)(Event_240_255.SIGN_KEY_REPLACE_PROCESSING_PASS));
                AddEventItem(Enum.GetName(typeof(Event_240_255), Event_240_255.SYMMETRIC_KEY_REPLACE_PROCESSING_PASS), m_usEvent240_255, (UInt16)(Event_240_255.SYMMETRIC_KEY_REPLACE_PROCESSING_PASS));

                return m_lstEvents;
            }
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
    internal class CENTRON_AMI_DemandConfig : DemandConfig
    {
        #region Constants

        private const int DEMAND_CONFIG_BLOCK_LENGTH = 52;

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
        public CENTRON_AMI_DemandConfig(CPSEM psem, ushort Offset)
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
    public class CENTRON_AMI_DisplayConfig : DisplayConfig_Shared
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
        public CENTRON_AMI_DisplayConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        /// <summary>
        /// Constructor for the Display Configuration table - used with file based structure.
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="Offset"></param>
        public CENTRON_AMI_DisplayConfig(PSEMBinaryReader BinaryReader, ushort Offset)
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
    public class CENTRON_AMI_TOUConfig : TOUConfig
    {
        #region Constants

        private const uint NUM_SUPPORTED_SEASONS = 1;
        /// <summary>
        /// Size of the TOU Table
        /// </summary>
        public const ushort TOU_CONFIG_SIZE = 52;
        private const int EVENTS_PER_DAYTYPE = 6;

        #endregion

        #region public methods
        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Offset of this subtable within table 2048</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 10/18/06 KRC 7.36.00 N/A    Created
        // 11/15/13 jrf 3.50.04 TQ 9478 Added m_blnHasSeasonProgrammedByte.
        // 11/20/13 jrf 3.50.06 TQ 9478 Moving data setup to InitializeData().
        //
        public CENTRON_AMI_TOUConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset, TOU_CONFIG_SIZE)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Offset of this subtable within table 2048</param>
        /// <param name="ConfigSize">Size of Configuration.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 11/15/13 jrf 3.50.04 TQ 9478   Created.
        // 11/20/13 jrf 3.50.06 TQ 9478 Moving data setup to InitializeData().
        //
        public CENTRON_AMI_TOUConfig(CPSEM psem, ushort Offset, ushort ConfigSize)
            : base(psem, Offset, ConfigSize)
        {

        }

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="TableID">ID of this table</param>
        /// <param name="Offset">Offset of this subtable within table 2048</param>
        /// <param name="ConfigSize">Size of Configuration.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  01/10/14 jrf 3.50.23 TQ 9478  Created.
        // 
        public CENTRON_AMI_TOUConfig(CPSEM psem, ushort TableID, ushort Offset, ushort ConfigSize)
            : base(psem, TableID, Offset, ConfigSize)
        {

        }

        /// <summary>
        /// Constructor for CENTRON AMI TOU Configuration table used with file based structure
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="Offset"></param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 10/18/06 KRC 7.36.00 N/A    Created
        // 11/15/13 jrf 3.50.04 TQ 9478 Added m_blnHasSeasonProgrammedByte.
        // 11/20/13 jrf 3.50.06 TQ 9478 Moving data setup to InitializeData().
        //
        public CENTRON_AMI_TOUConfig(PSEMBinaryReader BinaryReader, ushort Offset)
            : base(BinaryReader, Offset)
        {
        }

        /// <summary>
        /// Constructor for CENTRON AMI TOU Configuration table used with file based structure
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="TableID">ID of this table</param>
        /// <param name="Offset"></param>
        /// <param name="ConfigSize">Size of TOU config.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  01/10/14 jrf 3.50.23 TQ 9478  Created.
        // 
        public CENTRON_AMI_TOUConfig(PSEMBinaryReader BinaryReader, ushort TableID, ushort Offset, ushort ConfigSize)
            : base(BinaryReader, TableID, Offset, ConfigSize)
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
        /// Setup data items
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/20/13 jrf 3.50.06 TQ 9478 Created. 
        //
        protected override void InitializeData()
        {
            base.InitializeData();

            m_blnHasSeasonProgrammedByte = false;
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
    public class CENTRON_AMI_CalendarConfig : CalendarConfig
    {
        #region Constants

        /// <summary>
        /// The size of the OpenWay Calendar
        /// </summary>
        public const ushort CENTRON_AMI_CAL_SIZE = 72;
        /// <summary>
        /// Number of Years in the OpenWay Calendar
        /// </summary>
        public const byte CENTRON_AMI_CAL_YEARS = 2;
        private const int EVENTS_PER_YEAR = 16;

        #endregion

        #region public methods

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/18/06 KRC 7.36.00 N/A	Created
        // 
        public CENTRON_AMI_CalendarConfig(CPSEM psem, ushort Offset, ushort Size,
                              byte MaxCalYears)
            : base(psem, Offset, Size, MaxCalYears)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="TableID">ID of this table</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  01/10/14 jrf 3.50.23 TQ 9478  Created.
        // 
        public CENTRON_AMI_CalendarConfig(CPSEM psem, ushort TableID, ushort Offset, ushort Size,
                              byte MaxCalYears)
            : base(psem, TableID, Offset, Size, MaxCalYears)
        {
        }

        /// <summary>
        /// Calendar ConfiguratioN Construcutor used for file based structure
        /// </summary>
        /// <param name="BinaryReader">The binary Reader containing the data stream</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        public CENTRON_AMI_CalendarConfig(PSEMBinaryReader BinaryReader, ushort Offset, ushort Size,
                                byte MaxCalYears)
            : base(BinaryReader, Offset, Size, MaxCalYears)
        {
        }

        /// <summary>
        /// Calendar ConfiguratioN Construcutor used for file based structure
        /// </summary>
        /// <param name="BinaryReader">The binary Reader containing the data stream</param>
        /// <param name="TableID">ID of this table</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  01/10/14 jrf 3.50.23 TQ 9478  Created.
        // 
        public CENTRON_AMI_CalendarConfig(PSEMBinaryReader BinaryReader, ushort TableID, ushort Offset, ushort Size,
                                byte MaxCalYears)
            : base(BinaryReader, TableID, Offset, Size, MaxCalYears)
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
        //  11/15/13 jrf 3.50.04 TQ 9478 Added check for control byte availability.

        public override void Clear()
        {
            m_CalendarID.Value = 0;
            m_DSTOffset = 0;

            //Only clear the control byte if it is available.
            if (true == m_blnHasControlByte)
            {
                m_Control = 0;
            }

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
        // 11/18/13 jrf 3.50.04 TQ 9478 Added conversion of seasons 1-8.
        //
        public override eEventType GetEventType(ushort usEvent)
        {
            eEventType eType = eEventType.NO_EVENT;

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
                case (ushort)AMICalendarEvent.AMICalendarEventType.SEASON_1:
                case (ushort)AMICalendarEvent.AMICalendarEventType.SEASON_2:
                case (ushort)AMICalendarEvent.AMICalendarEventType.SEASON_3:
                case (ushort)AMICalendarEvent.AMICalendarEventType.SEASON_4:
                case (ushort)AMICalendarEvent.AMICalendarEventType.SEASON_5:
                case (ushort)AMICalendarEvent.AMICalendarEventType.SEASON_6:
                case (ushort)AMICalendarEvent.AMICalendarEventType.SEASON_7:
                case (ushort)AMICalendarEvent.AMICalendarEventType.SEASON_8:
                    {
                        eType = eEventType.SEASON;
                        break;
                    }
            }

            return eType;
        }

        /// <summary>
        /// Translates the CalendarEventType for a Calendar event into an eEventType
        /// for the TOUSchedule class.
        /// </summary>
        /// <returns>The TOU Schedule object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 11/18/13 jrf 3.50.04 TQ 9478 Created.

        public override int GetSeasonIndex(ushort usEvent)
        {
            int iSeasonIndex = 0;

            iSeasonIndex = usEvent - (int)AMICalendarEvent.AMICalendarEventType.SEASON_1;

            return iSeasonIndex;
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
        // 11/15/13 jrf 3.50.04 TQ 9478 Added check for control byte availability.

        protected override void ParseData()
        {
            //Populate the member variables that represent the table
            m_CalendarID.Value = m_Reader.ReadUInt16();

            if (true == m_blnHasControlByte)
            {
                m_Control = m_Reader.ReadByte();
            }

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

        /// <summary>
        /// Setup data items
        /// </summary>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/19/13 jrf 3.50.06 TQ 9478 Created. 
        //
        protected override void InitializeData(byte MaxCalYears)
        {
            base.InitializeData(MaxCalYears);

            m_blnHasControlByte = false;
        }

        #endregion

        #region Members

        //OpenWay TOU does not changes seasons, so no control byte is needed.
        /// <summary>
        /// Parameter that determines if a season change control byte is defined.
        /// </summary>
        protected bool m_blnHasControlByte;

        #endregion
    }

    /// <summary>
    /// Class that represents the Metrology Table in OpenWay (Partial Table Implementation)
    /// </summary>
    internal class CENTRON_AMI_Metrology : ANSISubTable
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
        public CENTRON_AMI_Metrology(CPSEM psem, ushort offset)
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
                                "CENTRON_AMI_MetrologyConfig.Read");

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

    internal class CENTRON_AMI_ModeControl : ModeControl
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

        public CENTRON_AMI_ModeControl(CPSEM psem, ushort Offset)
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

    internal class AMICalendarEvent : CalendarEvent
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
        // 11/15/13 jrf 3.50.04 TQ 9478 Added seasons 1-8.

        public enum AMICalendarEventType : ushort
        {
            NOT_USED = 0x0000,
            DST_ON = 0x0001,
            DST_OFF = 0x0002,
            HOLIDAY = 0x0003,
            SEASON_1 = 0x0004,
            SEASON_2 = 0x0005,
            SEASON_3 = 0x0006,
            SEASON_4 = 0x0007,
            SEASON_5 = 0x0008,
            SEASON_6 = 0x0009,
            SEASON_7 = 0x000A,
            SEASON_8 = 0x000B,
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

        public AMICalendarEvent()
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

        public AMICalendarEvent(ushort usEvent)
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
            if ((ushort)AMICalendarEventType.DST_ON != (m_usEvent & TYPE_MASK) ||
                (ushort)AMICalendarEventType.DST_OFF != (m_usEvent & TYPE_MASK))
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
        // 11/15/13 jrf 3.50.04 TQ 9478 Added check for seasons 1-8.

        public override bool IsSeason()
        {
            if (((ushort)AMICalendarEventType.SEASON_1 <= (m_usEvent & TYPE_MASK)) &&
                ((ushort)AMICalendarEventType.SEASON_8 >= (m_usEvent & TYPE_MASK)))
            {
                return true;
            }
            else
            {
                return false;
            }
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
        // 11/15/13 jrf 3.50.04 TQ 9478 Added Season 8 type max.

        public override byte Type
        {
            get
            {
                return base.Type;
            }
            set
            {
                if (value <= (ushort)AMICalendarEventType.SEASON_8)
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
        // 11/15/13 jrf 3.50.04 TQ 9478 Added translation of seasons 1-8.
        //
        public override string TranslatedType
        {
            get
            {
                string Type = "";

                switch (m_usEvent & TYPE_MASK)
                {
                    case (ushort)AMICalendarEventType.NOT_USED:
                        {
                            Type = "NOT_USED";
                            break;
                        }
                    case (ushort)AMICalendarEventType.DST_ON:
                        {
                            Type = "DST_ON";
                            break;
                        }
                    case (ushort)AMICalendarEventType.DST_OFF:
                        {
                            Type = "DST_OFF";
                            break;
                        }
                    case (ushort)AMICalendarEventType.HOLIDAY:
                        {
                            Type = "HOLIDAY";
                            break;
                        }
                    case (ushort)AMICalendarEventType.SEASON_1:
                        {
                            Type = "SEASON1";
                            break;
                        }
                    case (ushort)AMICalendarEventType.SEASON_2:
                        {
                            Type = "SEASON2";
                            break;
                        }
                    case (ushort)AMICalendarEventType.SEASON_3:
                        {
                            Type = "SEASON3";
                            break;
                        }
                    case (ushort)AMICalendarEventType.SEASON_4:
                        {
                            Type = "SEASON4";
                            break;
                        }
                    case (ushort)AMICalendarEventType.SEASON_5:
                        {
                            Type = "SEASON5";
                            break;
                        }
                    case (ushort)AMICalendarEventType.SEASON_6:
                        {
                            Type = "SEASON6";
                            break;
                        }
                    case (ushort)AMICalendarEventType.SEASON_7:
                        {
                            Type = "SEASON7";
                            break;
                        }
                    case (ushort)AMICalendarEventType.SEASON_8:
                        {
                            Type = "SEASON8";
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
    public class CENTRON_AMI_IOConfig : IOConfig
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
        public CENTRON_AMI_IOConfig(CPSEM psem, ushort Offset)
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
        public CENTRON_AMI_IOConfig(PSEMBinaryReader EDLBinaryReader)
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
        // 02/27/12 jrf 2.53.44 162615 Forcing rounding of pulse weights here to prevent 
        //                             rounding errors later on.
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
            //Forcing rounding to two fractional digits here to prevent rounding errors later.
            KYZConfig.KYZ1PulseWeight = (float)Math.Round((double)KYZConfig.KYZ1PulseWeight, 2);
            KYZConfig.KYZ1PulseWidth = m_uiOutputCh1PulseWidth;

            KYZConfig.KYZ2OutputType = m_byOutputCh2Type;
            KYZConfig.KYZ2EnergyID = m_byOutputCh2Energy;
            KYZConfig.KYZ2EventID = m_byOutputCh2Event;
            KYZConfig.KYZ2PulseWeight = (float)m_uiOutputCh2PulseWeight * PULSE_WEIGHT_MULTIPLIER;
            //Forcing rounding to two fractional digits here to prevent rounding errors later.
            KYZConfig.KYZ2PulseWeight = (float)Math.Round((double)KYZConfig.KYZ2PulseWeight, 2);
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
