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
//                           Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Fail safe reasons
    /// </summary>
    public enum FailsafeReasons : byte
    {
        /// <summary>
        /// The meter is not in failsafe mode.
        /// </summary>
        NotInFailsafe = 0,
        /// <summary>
        /// Procedure 80 was called and a duration was specified.
        /// </summary>
        ProcedureWithDuration = 1,
        /// <summary>
        /// Firmware was activated.
        /// </summary>
        FirmwareActivated = 2,
        /// <summary>
        /// The meter was initialized.
        /// </summary>
        MeterInitialized = 3,
        /// <summary>
        /// Service Limiting was reconfigured.
        /// </summary>
        ServiceLimitingConfigured = 4,
        /// <summary>
        /// The Demands were reconfigured.
        /// </summary>
        DemandConfigured = 5,
        /// <summary>
        /// Procedure 80 was called with 65535 duration meaning it will never leave failsafe mode.
        /// </summary>
        ProcedureNoDuration = 6,
        /// <summary>
        /// Failsafe mode is not supported.
        /// </summary>
        NotSupported = 255,
    }

    /// <summary>
    /// Threshold object for remote service limiting.
    /// </summary>
    // TODO: Add property to get the name of the threshold quantity
    public class DisconnectThreshold
    {
        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public DisconnectThreshold()
            : this(0, 0)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byQuantity">The demand quantity index for the threshold.</param>
        /// <param name="fThreshold">The value of the threshold.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public DisconnectThreshold(byte byQuantity, float fThreshold)
        {
            m_byQuantity = byQuantity;
            m_fThreshold = fThreshold;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the index of the demand quantity for this threshold.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public byte DemandQuantityIndex
        {
            get
            {
                return m_byQuantity;
            }
            set
            {
                m_byQuantity = value;
            }
        }

        /// <summary>
        /// Gets or sets the threshold value.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public float Threshold
        {
            get
            {
                return m_fThreshold;
            }
            set
            {
                m_fThreshold = value;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byQuantity;
        private float m_fThreshold;

        #endregion
    }

    /// <summary>
    /// MFG Table 2139 - Actual Limiting Disconnect Switch Table
    /// </summary>
    public class OpenWayMFGTable2139 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 1;
        private const byte HARDWARE_EXISTS_MASK = 0x01;
        private const byte THRESHOLD_MASK = 0x06;
        private const byte THRESHOLD_SHIFT = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public OpenWayMFGTable2139(CPSEM psem)
            : base(psem, 2139, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2139.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_bySwitchFlags = m_Reader.ReadByte();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the Disconnect hardware exists in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public bool DoesHardwareExist
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Switch Table Data"));
                    }
                }

                return (m_bySwitchFlags & HARDWARE_EXISTS_MASK) == HARDWARE_EXISTS_MASK;
            }
        }

        /// <summary>
        /// Gets the number of thresholds used by the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public byte NumberOfThresholds
        {
            get
            {
                byte byThresholds = 0;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Switch Table Data"));
                    }
                }

                byThresholds = (byte)((m_bySwitchFlags & THRESHOLD_MASK) >> THRESHOLD_SHIFT);

                return byThresholds;
            }
        }

        #endregion

        #region Member Variables

        private byte m_bySwitchFlags;

        #endregion
    }

    /// <summary>
    /// MFG Table 2140 - Disconnect Switch Status Table
    /// </summary>
    public class OpenWayMFGTable2140 : AnsiTable
    {
        #region Constants

        private const byte STATUS_CONNECTED_MASK = 0x01;
        private const byte STATUS_FUNCTIONING_MASK = 0x02;
        private const byte STATUS_LV_PRESENT_MASK = 0x04;
        private const byte STATUS_ATTEMPT_FAILED_MASK = 0x08;
        private const byte STATUS_METER_ARMED = 0x10;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table00">Table 0 object for the current device</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created


        public OpenWayMFGTable2140(CPSEM psem, CTable00 Table00)
            : base(psem, 2140, OpenWayMFGTable2140.GetTableSize(Table00), TABLE_TIMEOUT)
        {
        }


        /// <summary> 
        /// protected Constructor used by OpenWayMFGTable2140_2009
        /// </summary>
        /// <param name="psem">PSEM Protocol</param>
        /// <param name="Size">The calculated size of the table</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD          N/A    Created

        protected OpenWayMFGTable2140(CPSEM psem, uint Size)
            : base(psem, 2140, Size, TABLE_TIMEOUT)
        {

        }

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table00">Table 0 object for the current device.</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public static uint GetTableSize(CTable00 Table00)
        {
            uint uiTableSize = 0;

            // SWITCH_STATUS_FLAG
            uiTableSize += 1;
            // REMAINING_DAILY_SWITCHED
            uiTableSize += 1;
            // ACTIVE_THRESHOLD
            uiTableSize += 1;
            // REMAING_TIME_IN_ACTIVE_TIER
            uiTableSize += Table00.TIMESize;
            // RAMAING_TIME_FOR_SWITCH_OPEN
            uiTableSize += Table00.TIMESize;

            return uiTableSize;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2140.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the meter is currently connected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public bool IsConnected
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySwitchStatus & STATUS_CONNECTED_MASK) == STATUS_CONNECTED_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the hardware is currently functioning
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public bool IsFunctioning
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySwitchStatus & STATUS_FUNCTIONING_MASK) == STATUS_FUNCTIONING_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Load Voltage is currently present
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public bool IsLoadVoltagePresent
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySwitchStatus & STATUS_LV_PRESENT_MASK) == STATUS_LV_PRESENT_MASK;

            }
        }

        /// <summary>
        /// Gets whether or not the last attempt failed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public bool LastAttemptFailed
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySwitchStatus & STATUS_ATTEMPT_FAILED_MASK) == STATUS_ATTEMPT_FAILED_MASK;

            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently armed for connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public bool IsMeterArmed
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySwitchStatus & STATUS_METER_ARMED) == STATUS_METER_ARMED;
            }
        }

        /// <summary>
        /// Gets the remaing number of daily switched.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public byte RemainingDailySwitches
        {
            get
            {
                ReadUnloadedTable();

                return m_byRemainingDailySwitches;
            }
        }

        /// <summary>
        /// Gets the currently active threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public byte ActiveThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_byActiveThreshold;
            }
        }

        /// <summary>
        /// Gets the amount of time remaining for the current threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public TimeSpan RemainingTimeForActiveTier
        {
            get
            {
                ReadUnloadedTable();

                return m_tsRemainingTimeInTier;
            }
        }

        /// <summary>
        /// Gets the amount of time remaining for siwtch open
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/19/08 RCG 1.50.26 N/A    Created

        public TimeSpan RemainingTimeForSwitchOpen
        {
            get
            {
                ReadUnloadedTable();

                return m_tsRemainingTimeInTier;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/09 RCG 2.21.01 N/A    Created

        protected virtual void ParseData()
        {
            m_DataStream.Position = 0;

            m_bySwitchStatus = m_Reader.ReadByte();
            m_byRemainingDailySwitches = m_Reader.ReadByte();
            m_byActiveThreshold = m_Reader.ReadByte();
            m_tsRemainingTimeInTier = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_tsRemainingTimeForSwitchOpen = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// Switch Status byte
        /// </summary>
        protected byte m_bySwitchStatus;
        /// <summary>
        /// Number of remaining daily switches
        /// </summary>
        protected byte m_byRemainingDailySwitches;
        /// <summary>
        /// The active threshold
        /// </summary>
        protected byte m_byActiveThreshold;
        /// <summary>
        /// The amount of time remaining in the tier
        /// </summary>
        protected TimeSpan m_tsRemainingTimeInTier;
        /// <summary>
        /// The amount of time before the switch will close
        /// </summary>
        protected TimeSpan m_tsRemainingTimeForSwitchOpen;

        #endregion
    }

    /// <summary>
    ///  Table 2140 - This is the 2140 Table as finally balloted in 2009 for SP5 release.
    /// </summary>

    internal class OpenWayMFGTable2140SP5 : OpenWayMFGTable2140
    {
        #region Constants

        private const byte STATUS_FAILSAFE_MASK = 0x20;
        private const byte LAST_DISCONNECT_MASK = 0x01;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor implemented for new data size
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table00">Table 0 object for the current device</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD          N/A    Created


        public OpenWayMFGTable2140SP5(CPSEM psem, CTable00 Table00)
            : base(psem, OpenWayMFGTable2140SP5.GetTableSizeSP5(Table00))
        {
        }

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table00">Table 0 object for the current device.</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD          N/A    Created

        public static uint GetTableSizeSP5(CTable00 Table00)
        {
            uint uiTableSize = 0;

            // SWITCH_STATUS_FLAG
            uiTableSize += 1;
            // REMAINING_DAILY_SWITCHED
            uiTableSize += 1;
            // ACTIVE_THRESHOLD
            uiTableSize += 1;
            // REMAING_TIME_IN_ACTIVE_TIER
            uiTableSize += Table00.TIMESize;
            // RAMAING_TIME_FOR_SWITCH_OPEN
            uiTableSize += Table00.TIMESize;
            // SL FAILSAFE REASON
            uiTableSize += 1;
            //REMAINING_TIME IN FAILSAFE MODE
            uiTableSize += Table00.TIMESize;
            //SL DISCONNECT ENABLE STATE
            uiTableSize += 1;
            //LAST DISCONNECT DUE TO SL
            uiTableSize += 1;
            //SL CURRENT DEMAND VALUE
            uiTableSize += 4;
            //SL CURRENT DEMAND VALUE
            uiTableSize += 4;

            return uiTableSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Failsafe Reason
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD            N/A    Created

        public FailsafeReasons FailsafeReason
        {
            get
            {
                ReadUnloadedTable();

                return (FailsafeReasons)m_byFailsafeReason;
            }
        }

        /// <summary>
        /// Gets whether or not the last disconnect was due to Service Limiting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD            N/A    Created

        public bool WasLastDisconnectDueToSL
        {
            get
            {
                ReadUnloadedTable();

                return (m_byLastDisconnectSL & LAST_DISCONNECT_MASK) == LAST_DISCONNECT_MASK;
            }
        }

        /// <summary>
        /// Gets the amount of time remaining for the FailSafe 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD             N/A    Created

        public TimeSpan RemainingTimeForFailsafe
        {
            get
            {
                ReadUnloadedTable();

                return m_tsRemainingTimeInFailsafe;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently in failsafe mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 MMD             N/A    Created

        public bool IsInFailsafeMode
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySwitchStatus & STATUS_FAILSAFE_MASK) == STATUS_FAILSAFE_MASK;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/09 RCG 2.21.01 N/A    Created

        protected override void ParseData()
        {
            m_DataStream.Position = 0;

            m_bySwitchStatus = m_Reader.ReadByte();
            m_byRemainingDailySwitches = m_Reader.ReadByte();
            m_byActiveThreshold = m_Reader.ReadByte();
            m_tsRemainingTimeInTier = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_tsRemainingTimeForSwitchOpen = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_byFailsafeReason = m_Reader.ReadByte();
            m_tsRemainingTimeInFailsafe = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_byLastDisconnectSL = m_Reader.ReadByte();
            m_isInFailsafeMode = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private byte m_byFailsafeReason;
        private TimeSpan m_tsRemainingTimeInFailsafe;
        private byte m_byLastDisconnectSL;
        private byte m_isInFailsafeMode;

        #endregion
    }

    /// <summary>
    /// MFG Table 2141 - Disconnect Switch Config Table
    /// </summary>
    public class OpenWayMFGTable2141 : AnsiTable
    {

        #region Constants

        private const uint THRESHOLD_SIZE = 5; // UINT8 + FLOAT32
        private const byte CONNECT_INTERVENTION_MASK = 0x01;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table00">Table 0 object for the current device</param>
        /// <param name="Table2139">Table 2139 object for the current device</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public OpenWayMFGTable2141(CPSEM psem, CTable00 Table00, OpenWayMFGTable2139 Table2139)
            : base(psem, 2141, OpenWayMFGTable2141.GetTableSize(Table00, Table2139))
        {
            m_byNumberOfThresholds = Table2139.NumberOfThresholds;
            m_Thresholds = new List<DisconnectThreshold>();
        }

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table00">Table 0 object for the current device.</param>
        /// <param name="Table2139">Table 2139 object for the current device</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public static uint GetTableSize(CTable00 Table00, OpenWayMFGTable2139 Table2139)
        {
            uint uiTableSize = 0;

            // RECONNECT_OPTION
            uiTableSize += 1;
            // MAX_SWITCHES
            uiTableSize += 1;
            // MAX_SWITCH_PERIOD
            uiTableSize += Table00.TIMESize;
            // RANDOMIZATION_ALARM
            uiTableSize += Table00.TIMESize;
            // RESTORATION_START_DELAY
            uiTableSize += Table00.TIMESize;
            // RESTORATION_RANDOM_DELAY
            uiTableSize += Table00.TIMESize;
            // OPEN_TIME
            uiTableSize += Table00.TIMESize;
            // RETRY_ATTEMPTS
            uiTableSize += 1;
            // THRESHOLDS
            uiTableSize += Table2139.NumberOfThresholds * THRESHOLD_SIZE;

            return uiTableSize;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns> The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2141.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_byReconnectOption = m_Reader.ReadByte();
                m_byMaxSwitches = m_Reader.ReadByte();
                m_tsMaxSwitchPeriod = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_tsRandomizationAlarm = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_tsRestorationStartDelay = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_tsRestorationRandomDelay = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_tsOpenTime = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_byRetryAttempts = m_Reader.ReadByte();

                for (byte byIndex = 0; byIndex < m_byNumberOfThresholds; byIndex++)
                {
                    DisconnectThreshold CurrentThreshold = new DisconnectThreshold();

                    CurrentThreshold.DemandQuantityIndex = m_Reader.ReadByte();
                    CurrentThreshold.Threshold = m_Reader.ReadSingle();

                    m_Thresholds.Add(CurrentThreshold);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the meter is configured to use User Intervention on connect.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public bool ConnectsUsingUserIntervention
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Config Table Data"));
                    }
                }

                return (m_byReconnectOption & CONNECT_INTERVENTION_MASK) == CONNECT_INTERVENTION_MASK;
            }
        }

        /// <summary>
        /// Gets the maximum number of disconnect switches that will occur within the switch period.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public byte MaxSwitches
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Config Table Data"));
                    }
                }

                return m_byMaxSwitches;
            }
        }

        /// <summary>
        /// Gets the max period of time for switches to occur
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public TimeSpan MaxSwitchPeriod
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Config Table Data"));
                    }
                }

                return m_tsMaxSwitchPeriod;
            }
        }

        /// <summary>
        /// Gets the period of time where an alarm will be raised.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public TimeSpan RandomizationAlarmPeriod
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Config Table Data"));
                    }
                }

                return m_tsRandomizationAlarm;
            }
        }

        /// <summary>
        /// Gets the period of time to wait until connection will start to be restored
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public TimeSpan RestorationStartDelay
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Config Table Data"));
                    }
                }

                return m_tsRestorationStartDelay;
            }
        }

        /// <summary>
        /// Gets the period of time that the restoration will randomly occur in.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public TimeSpan RestorationRandomDelay
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Config Table Data"));
                    }
                }

                return m_tsRestorationRandomDelay;
            }
        }

        /// <summary>
        /// Gets the length of time to keep the switch open.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public TimeSpan OpenTime
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Config Table Data"));
                    }
                }

                return m_tsOpenTime;
            }
        }

        /// <summary>
        /// Gets the number of times to reattempt to connect or disconnect
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/08 RCG 1.50.22 N/A    Created

        public List<DisconnectThreshold> Thresholds
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Config Table Data"));
                    }
                }

                return m_Thresholds;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byNumberOfThresholds;
        private byte m_byReconnectOption;
        private byte m_byMaxSwitches;
        private TimeSpan m_tsMaxSwitchPeriod;
        private TimeSpan m_tsRandomizationAlarm;
        private TimeSpan m_tsRestorationStartDelay;
        private TimeSpan m_tsRestorationRandomDelay;
        private TimeSpan m_tsOpenTime;
        private byte m_byRetryAttempts;
        private List<DisconnectThreshold> m_Thresholds;

        #endregion
    }

    /// <summary>
    /// MFG Table 2142 - Disconnect Override Table
    /// </summary>
    public class OpenWayMFGTable2142 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.22 N/A    Created

        public OpenWayMFGTable2142(CPSEM psem)
            : base(psem, 2142, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.22 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2142.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_byOverride = m_Reader.ReadByte();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/Sets whether or not Service Limiting disconnects have been disabled in the meter.
        /// If service limiting has been overriden in the meter the this means the 
        /// remote disconnect switch has been disabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.22 N/A    Created
        //  05/18/09 jrf 2.20.05 133921 Adding ability to disable the remote disconnect switch.
        //
        public bool IsServiceLimitingOverriden
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Disconnect Override Table Data"));
                    }
                }


                return Convert.ToBoolean(m_byOverride);
            }

            set
            {
                m_byOverride = Convert.ToByte(value);

                m_TableState = TableState.Dirty;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byOverride;

        #endregion
    }
}
