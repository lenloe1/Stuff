using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This CTable2048_Poly class manages the header and config blocks of 2048.
    /// This class is specific to the CENTRON V and I (Poly) meter.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------------
    /// 07/13/05 mrj 7.13.00 N/A    Created
    ///
    internal class CTable2048_Poly : CTable2048_Shared
    {
        /// <summary>
        /// Constructor, create the config blocks specific to the Poly.		
        /// </summary>
        /// <param name="psem">The protocol instance to use</param>
        /// <example><code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable2048_Poly Table2048 = new CTable2048_Poly( PSEM ); 
        /// </code></example>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/06 mrj 7.30.00 N/A    Created
        // 01/15/07 RCG 8.00.06        Adding DisplayConfig

        public CTable2048_Poly(CPSEM psem)
            : base(psem)
        {
            m_ConstantsConfig = new ConstantsConfig(psem,
                            m_2048Header.ConstantsOffset,
                            ConstantsConfig.IMAGE_CONSTANTS_CONFIG_LENGTH);
            m_BillingSchedConfig = new BillingSchedConfig(psem,
                m_2048Header.BillingSchedOffset,
                BillingSchedConfig.IMAGE_BILLING_SCHED_CONFIG_LENGTH);
            m_DisplayConfig = new DisplayConfig_Shared(psem, m_2048Header.DisplayOffset);
            m_HistoryLogConfig = new CENTRON_POLY_HistoryLogConfig(psem, m_2048Header.HistoryLogOffset);

            // Initialize Firmware and/or device specific tables
            InitializeSpecialCases();
        }
    } // CTable2048Poly class

    /// <summary>
    /// Class that represents the history log configuration data stored in table 2048
    /// for the CENTRON POLY meter.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 03/09/07 RCG 8.00.17 N/A    Created
    internal class CENTRON_POLY_HistoryLogConfig : HistoryLogConfig
    {
        #region Public Methods

        /// <summary>
        /// Constructor for CENTRON POLY History Log Config class
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="Offset"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/09/07 KRC 8.00.17
        //
        public CENTRON_POLY_HistoryLogConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the list of History Log event specfic to the CENTRON POLY
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/07 AF  8.00.24 2675   Modified description for LOSS_VOLTAGE_A
        //
        public override List<MFG2048EventItem> HistoryLogEventList
        {
            get
            {
                m_lstEvents = base.HistoryLogEventList;

                // Add Event 0 - 15 bitfield items
                AddEventItem("CLEAR_BILLING_DATA", m_usEvent0_15,
                                (UInt16)(Event_0_15.CLEAR_BILLING_DATA));
                AddEventItem("BILLING_SCHED_EXPIRED", m_usEvent0_15,
                                (UInt16)(Event_0_15.BILLING_SCHED_EXPIRED));
                AddEventItem("DST_TIME_CHANGE", m_usEvent0_15,
                                (UInt16)(Event_0_15.DST_TIME_CHANGE));
                AddEventItem("LOGON_SUCCESSFUL", m_usEvent0_15,
                                (UInt16)(Event_0_15.LOGON_SUCCESSFUL));
                AddEventItem("SECURITY_SUCCESS", m_usEvent0_15,
                                (UInt16)(Event_0_15.SECURITY_SUCCESSFUL));
                AddEventItem("SECURITY_FAILED", m_usEvent0_15,
                                (UInt16)(Event_0_15.SECURITY_FAILED));

                // Add Event 16 - 31 bitfield items
                AddEventItem("TOU_SEASON_CHANGED", m_usEvent16_31,
                                (UInt16)(Event_16_31.TOU_SEASON_CHANGED));
                AddEventItem("RATE_CHANGE", m_usEvent16_31,
                                (UInt16)(Event_16_31.RATE_CHANGE));
                AddEventItem("CUST_SCHEDULE_CHANGED", m_usEvent16_31,
                                (UInt16)(Event_16_31.SITESCAN_OR_CUSTOM_SCHED_ERROR));
                AddEventItem("SITESCAN_ERROR", m_usEvent16_31,
                                (UInt16)(Event_16_31.SITESCAN_OR_PENDING_TABLE_CLEAR));

                // Add Event 32 - 47 bitfield items
                AddEventItem("LOSS_OF_PHASE_RESTORE", m_usEvent32_47,
                                (UInt16)(Event_32_47.LOSS_OF_PHASE_RESTORE));
                AddEventItem("LOSS_OF_POTENTIAL", m_usEvent32_47,
                                (UInt16)(Event_32_47.LOSS_VOLTAGE_A_OR_LOSS_OF_PHASE));
                AddEventItem("REVERSE_POWER_FLOW_RESTORE", m_usEvent32_47,
                                (UInt16)(Event_32_47.REVERSE_POWER_FLOW_RESTORE));

                // Add Event 48 - 63 bitfield items
                AddEventItem("SS_DIAG1_ACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG1_ACTIVE));
                AddEventItem("SS_DIAG2_ACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG2_ACTIVE));
                AddEventItem("SS_DIAG3_ACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG3_ACTIVE));
                AddEventItem("SS_DIAG4_ACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG4_ACTIVE));
                AddEventItem("SS_DIAG5_ACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG5_ACTIVE));
                AddEventItem("SS_DIAG1_INACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG1_INACTIVE));
                AddEventItem("SS_DIAG2_INACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG2_INACTIVE));
                AddEventItem("SS_DIAG3_INACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG3_INACTIVE));
                AddEventItem("SS_DIAG4_INACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG4_INACTIVE));
                AddEventItem("SS_DIAG5_INACTIVE", m_usEvent48_63,
                                (UInt16)(Event_48_63.SS_DIAG5_INACTIVE));

                return m_lstEvents;
            }
        }

        #endregion
    }
}
