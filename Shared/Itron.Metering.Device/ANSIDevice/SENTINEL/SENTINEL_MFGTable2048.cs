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
//                              Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This CTable2048_Sentinel class manages the header and config blocks of 2048.
    /// This class is specific to the Sentinel meter.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------------
    /// 07/13/05 mrj 7.13.00 N/A    Created
    ///
    internal class CTable2048_Sentinel : CTable2048_Shared
    {
        /// <summary>
        /// Constructor, create the config blocks specific to the Sentinel.
        /// </summary>
        /// <param name="psem">The protocol instance to use</param>
        /// <param name="FW_Rev">Firmware rev of this meter</param>
        /// <example><code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable2048_Sentinel Table2048 = new CTable2048_Sentinel( PSEM ); 
        /// </code></example>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00 N/A    Created
        ///
        public CTable2048_Sentinel(CPSEM psem, float FW_Rev)
            : base(psem)
        {
            m_ConstantsConfig = new ConstantsConfig(psem,
                        m_2048Header.ConstantsOffset,
                        ConstantsConfig.SENTINEL_CONSTANTS_CONFIG_LENGTH);
            m_BillingSchedConfig = new BillingSchedConfig(psem,
                        m_2048Header.BillingSchedOffset,
                        BillingSchedConfig.SENTINEL_BILLING_SCHED_CONFIG_LENGTH);
            m_DisplayConfig = new SENTINEL_DisplayConfig(psem, m_2048Header.DisplayOffset);
            m_HistoryLogConfig = new SENTINEL_HistoryLogConfig(psem, m_2048Header.HistoryLogOffset);
            if (SATURN_FIRMWARE_REV > FW_Rev)
            {
                // The default case in the base class handles Saturn and Image 
                // meters
                m_MaxCalYears = PRE_SATURN_CALENDAR_YEARS;
            }

            // Initialize Firmware and/or device specific tables
            InitializeSpecialCases();
        }

        #region private definitions

        private const float SATURN_FIRMWARE_REV = 5.0F;
        private const byte PRE_SATURN_CALENDAR_YEARS = 20;

        #endregion private definitions

    } // CTable2048Sentinel class

    /// <summary>
    /// Class that represents the history log configuration data stored in table 2048
    /// for the SENTINEL meter.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 03/09/07 RCG 8.00.17 N/A    Created
    internal class SENTINEL_HistoryLogConfig : HistoryLogConfig
    {
        #region Public Methods

        /// <summary>
        /// Constructor for SENTINEL History Log Config class
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="Offset"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/09/07 KRC 8.00.17
        //
        public SENTINEL_HistoryLogConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the list of History Log event specfic to the SENTIENL
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/09/07 KRC 8.00.17
        //  04/03/07 AF  8.00.24 2673   Added SITESCAN_OR_PENDING_TABLE_CLEAR
        //  04/04/07 AF  8.00.24 2675
        //  07/12/16 MP  4.70.7  WR688986 Changed how event descriptions were accessed
        //  07/13/16 MP  4.70.7  WR688986 Changed LOSS_OF_PHASE to hard-coded LOSS_OF_POTENTIAL, since it is different from AMI devices.

        public override List<MFG2048EventItem> HistoryLogEventList
        {
            get
            {
                m_lstEvents = base.HistoryLogEventList;

                // Add Event 0 - 15 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.BILLING_DATA_CLEARED), m_usEvent0_15, (UInt16)(Event_0_15.BILLING_DATA_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.DST_TIME_CHANGE), m_usEvent0_15, (UInt16)(Event_0_15.DST_TIME_CHANGE));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.DEMAND_THRESHOLD_EXCEEDED), m_usEvent0_15, (UInt16)(Event_0_15.DEMAND_THRESHOLD_EXCEEDED));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.DEMAND_THRESHOLD_RESTORED), m_usEvent0_15, (UInt16)(Event_0_15.DEMAND_THRESHOLD_RESTORED));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.LOGON_SUCCESSFUL), m_usEvent0_15, (UInt16)(Event_0_15.LOGON_SUCCESSFUL));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.SECURITY_SUCCESSFUL), m_usEvent0_15, (UInt16)(Event_0_15.SECURITY_SUCCESSFUL));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.SECURITY_FAILED), m_usEvent0_15, (UInt16)(Event_0_15.SECURITY_FAILED));

                // Add Event 16 - 31 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.INPUT_CHANNEL_HI), m_usEvent16_31, (UInt16)(Event_16_31.INPUT_CHANNEL_HI));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.INPUT_CHANNEL_LO), m_usEvent16_31, (UInt16)(Event_16_31.INPUT_CHANNEL_LO));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.TOU_SEASON_CHANGED), m_usEvent16_31, (UInt16)(Event_16_31.TOU_SEASON_CHANGED));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.RATE_CHANGE), m_usEvent16_31, (UInt16)(Event_16_31.RATE_CHANGE));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.EXTERNAL_EVENT), m_usEvent16_31, (UInt16)(Event_16_31.EXTERNAL_EVENT));
                //SITESCAN_OR_PENDING_TABLE_CLEAR is SiteScan Error for SENTINEL devices
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT), m_usEvent16_31, (UInt16)(Event_16_31.PENDING_TABLE_CLEAR));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.VQ_LOG_NEARLY_FULL), m_usEvent16_31, (UInt16)(Event_16_31.VQ_LOG_NEARLY_FULL));

                // Add Event 32 - 47 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.LOSS_OF_PHASE_RESTORE), m_usEvent32_47, (UInt16)(Event_32_47.LOSS_OF_PHASE_RESTORE));
                AddEventItem("LOSS_OF_POTENTIAL", m_usEvent32_47, (UInt16)(Event_32_47.LOSS_OF_PHASE)); // Special case where AMI and Sentinel conflict
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.VQ_LOG_CLEARED), m_usEvent32_47, (UInt16)(Event_32_47.VQ_LOG_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.REVERSE_POWER_FLOW_RESTORE), m_usEvent32_47, (UInt16)(Event_32_47.REVERSE_POWER_FLOW_RESTORE));

                // Add Event 48 - 63 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG1_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG1_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG2_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG2_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG3_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG3_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG4_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG4_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG5_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG5_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG6_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG6_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG1_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG1_INACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG2_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG2_INACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG3_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG3_INACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG4_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG4_INACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG5_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG5_INACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG6_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG6_INACTIVE));

                return m_lstEvents;
            }
        }

        #endregion
    }
    /// <summary>
    /// Class that represents the display configuration data stored in table 2048
    /// for the SENTINEL meter.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 01/23/07 RCG 8.00.08 N/A    Created
    internal class SENTINEL_DisplayConfig : DisplayConfig
    {
        #region Constants

        private const ushort DISPLAY_CONFIG_SIZE = 732;
        private const int MAX_NORMAL_ITEMS = 32;
        private const int MAX_ALTERNATE_ITEMS = 32;
        private const int MAX_TEST_ITEMS = 16;
        private const int BYTES_PER_ITEM = 9;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for the Display Configuration piece of 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.36.00 N/A    Created
        // 
        public SENTINEL_DisplayConfig(CPSEM psem, ushort Offset)
            : this(psem, Offset, DISPLAY_CONFIG_SIZE)
        {
        }

        /// <summary>
        /// Constructor for the Display Configuration piece of 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.36.00 N/A    Created
        // 
        protected SENTINEL_DisplayConfig(CPSEM psem, ushort Offset, ushort Size)
            : base(psem, Offset, Size)
        {
        }

        /// <summary>
        /// Reads the sub table and builds the list of display items
        /// </summary>
        /// <returns>The response code for the read.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "DisplayConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "DisplayConfig.Read succeeded");

                m_DataStream.Position = 0;

                //Populate the member variables that represent the R300 config
                m_byDisplayTime = m_Reader.ReadByte();
                m_byNormalLength = m_Reader.ReadByte();
                m_byAltLength = m_Reader.ReadByte();
                m_byTestLength = m_Reader.ReadByte();
                m_byDisplayControl = m_Reader.ReadByte();
                m_byScrollNonFatal = m_Reader.ReadByte();
                m_byLockNonFatal = m_Reader.ReadByte();
                m_byScrollDiag = m_Reader.ReadByte();
                m_byLockDiag = m_Reader.ReadByte();

                // Build up the Display Lists
                BuildDisplayLists();

                m_byScrollNonFatal2 = m_Reader.ReadByte();
                m_byLockNonFatal2 = m_Reader.ReadByte();
            }

            return Result;
        }

        #endregion

        #region Protected Methods

        protected override void BuildDisplayLists()
        {
            base.BuildDisplayLists();

            // Parse the Normal display items
            for (int iIndex = 0; iIndex < MAX_NORMAL_ITEMS; iIndex++)
            {

                if (iIndex < (int)m_byNormalLength)
                {
                    // We only want to add the items that are configured
                    UInt32 uiLID = m_Reader.ReadUInt32();
                    string strDispID = m_Reader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                    char[] charTrim = { '\0' };
                    strDispID = strDispID.Trim(charTrim);
                    ushort usFormat = m_Reader.ReadUInt16();
                    byte byDim = m_Reader.ReadByte();
                    ANSIDisplayData dispData = new ANSIDisplayData(uiLID, strDispID, usFormat, byDim);
                    m_NormalDisplayData.Add(dispData);
                }
                else
                {
                    // We still need to remove the items from the Binary Readers buffer
                    byte[] byJunk;

                    byJunk = m_Reader.ReadBytes(BYTES_PER_ITEM);
                }
            }

            // Parse the Alternate display items
            for (int iIndex = 0; iIndex < MAX_ALTERNATE_ITEMS; iIndex++)
            {

                if (iIndex < (int)m_byAltLength)
                {
                    // We only want to add the items that are configured
                    UInt32 uiLID = m_Reader.ReadUInt32();
                    string strDispID = m_Reader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                    char[] charTrim = { '\0' };
                    strDispID = strDispID.Trim(charTrim);
                    ushort usFormat = m_Reader.ReadUInt16();
                    byte byDim = m_Reader.ReadByte();
                    ANSIDisplayData dispData = new ANSIDisplayData(uiLID, strDispID, usFormat, byDim);
                    m_AltDisplayData.Add(dispData);
                }
                else
                {
                    // We still need to remove the items from the Binary Readers buffer
                    byte[] byJunk;

                    byJunk = m_Reader.ReadBytes(BYTES_PER_ITEM);
                }
            }

            // Parse the Test display items
            for (int iIndex = 0; iIndex < MAX_TEST_ITEMS; iIndex++)
            {

                if (iIndex < (int)m_byTestLength)
                {
                    // We only want to add the items that are configured
                    UInt32 uiLID = m_Reader.ReadUInt32();
                    string strDispID = m_Reader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                    char[] charTrim = { '\0' };
                    strDispID = strDispID.Trim(charTrim);
                    ushort usFormat = m_Reader.ReadUInt16();
                    byte byDim = m_Reader.ReadByte();
                    ANSIDisplayData dispData = new ANSIDisplayData(uiLID, strDispID, usFormat, byDim);
                    m_TestDisplayData.Add(dispData);
                }
                else
                {
                    // We still need to remove the items from the Binary Readers buffer
                    byte[] byJunk;

                    byJunk = m_Reader.ReadBytes(BYTES_PER_ITEM);
                }
            }
        }

        #endregion

        #region Members

        private byte m_byDisplayTime;
        private byte m_byNormalLength;
        private byte m_byAltLength;
        private byte m_byTestLength;
        private byte m_byDisplayControl;
        private byte m_byScrollNonFatal;
        private byte m_byLockNonFatal;
        private byte m_byScrollDiag;
        private byte m_byLockDiag;
        private byte m_byScrollNonFatal2;
        private byte m_byLockNonFatal2;

        #endregion

    }

    /// <summary>
    /// ANSIDisplayData - struct to hold Display info
    /// </summary>
    public struct ANSIDisplayData
    {
        /// <summary>
        /// ANISDisplayData - Data read from the Display Configuration Table to be used to create the Display Item
        /// </summary>
        /// <param name="uiLID">Numerical value of the LID</param>
        /// <param name="strDispID">The Display ID in string format</param>
        /// <param name="usFormat">The format information packaged in meter format</param>
        /// <param name="byDim">The dimension information packaged in meter format</param>
        public ANSIDisplayData(UInt32 uiLID, string strDispID, ushort usFormat, byte byDim)
        {
            m_uiLID = uiLID;
            m_strDispID = strDispID;
            m_usFormat = usFormat;
            m_byDim = byDim;
        }

        #region Public Properties

        /// <summary>
        /// Returns the Numeric LID value
        /// </summary>
        public UInt32 NumericLID
        {
            get
            {
                return m_uiLID;
            }
        }

        /// <summary>
        /// Returns the Display ID
        /// </summary>
        public string DisplayID
        {
            get
            {
                return m_strDispID;
            }
        }

        /// <summary>
        /// Returns the Display Format packaged in meter format
        /// </summary>
        public ushort DisplayFormat
        {
            get
            {
                return m_usFormat;
            }
        }

        /// <summary>
        /// Returns the Display Dimension Field packaged in Meter Format
        /// </summary>
        public byte DisplayDimension
        {
            get
            {
                return m_byDim;
            }
        }

        #endregion

        #region Members

        private UInt32 m_uiLID;
        private string m_strDispID;
        private ushort m_usFormat;
        private byte m_byDim;

        #endregion
    }
}
