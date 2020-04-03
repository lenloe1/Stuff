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
//                              Copyright © 2011 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 2260 (Itron 212)
    /// </summary>
    public class OpenWayMFGTable2260SR30Config : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 0;
        private const ushort TABLE_SIZE = 10;
        private const int TABLE_TIMEOUT = 5000;
        private const byte FAT_ERR_RECOVERY_ENABLED_MASK = 0x01;
        private const byte TAP_ENABLED_MASK = 0x01;
        private const byte ASSET_SYNC_ENABLED_MASK = 0x01;
        private const byte DEFAULT_MASK = 0x80;
        private const byte FER_ENABLED_MASK = 0x80;

        private const byte INVERSION_THRESHOLD_DEFAULT = 18;
        private const byte REMOVAL_THRESHOLD_DEFAULT = 10;
        private const byte TAP_THRESHOLD_DEFAULT = 10;
        private const uint WAKEUP_DURATION_DEFAULT = 3600;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public OpenWayMFGTable2260SR30Config(CPSEM psem)
            : base(psem, 2260, TABLE_OFFSET, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2260.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether or not fatal error recovery is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  03/10/10 RCG 2.40.24        Updating to check for default bit

        public bool FatalErrorRecoveryEnabled
        {
            get
            {
                bool bEnabled = false;

                if (UseFatalErrorRecoveryDefault == false)
                {
                    ReadUnloadedTable();

                    bEnabled = (m_byFatalErrorRecoveryConfig & FAT_ERR_RECOVERY_ENABLED_MASK) == FAT_ERR_RECOVERY_ENABLED_MASK;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Whether or not tap is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  03/10/10 RCG 2.40.24        Updating to check for default bit

        public bool TapEnabled
        {
            get
            {
                bool bEnabled = false;

                if (UseTapDefault == false)
                {
                    ReadUnloadedTable();

                    return (m_byTapControl & TAP_ENABLED_MASK) == TAP_ENABLED_MASK;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// An event is triggered if the acceleration value along the X axis of the accelerometer
        /// is greater than or equal to this threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  03/10/10 RCG 2.40.24        Updating to check for default bit

        public byte InversionThreshold
        {
            get
            {
                byte byThreshold = INVERSION_THRESHOLD_DEFAULT;

                if (UseTapDefault == false)
                {
                    ReadUnloadedTable();

                    byThreshold = m_byInversionThreshold;
                }

                return byThreshold;
            }
        }

        /// <summary>
        /// An event is triggered if average acceleration value on X and Z axes of the accelerometer
        /// is above this threshold.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  03/10/10 RCG 2.40.24        Updating to check for default bit

        public byte RemovalThreshold
        {
            get
            {
                byte byThreshold = REMOVAL_THRESHOLD_DEFAULT;

                if (UseTapDefault == false)
                {
                    ReadUnloadedTable();

                    byThreshold = m_byRemovalThreshold;
                }

                return byThreshold;
            }
        }

        /// <summary>
        /// A tap is detected if average acceleration value on the Y axis of the accelerometer
        /// is above this threshold and falls below it within 400 milliseconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  03/10/10 RCG 2.40.24        Updating to check for default bit

        public byte TapThreshold
        {
            get
            {
                byte byThreshold = TAP_THRESHOLD_DEFAULT;

                if (UseTapDefault == false)
                {
                    ReadUnloadedTable();

                    byThreshold = m_byTapThreshold;
                }

                return byThreshold;
            }
        }

        /// <summary>
        /// The number of seconds to pull data from the accelerometer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  03/10/10 RCG 2.40.24        Updating to check for default bit

        public uint WakeupDurationSecond
        {
            get
            {
                uint uiDuration = WAKEUP_DURATION_DEFAULT;

                if (UseTapDefault == false)
                {
                    ReadUnloadedTable();

                    uiDuration = m_uiWakeupDurationSecond;
                }

                return uiDuration;
            }
        }

        /// <summary>
        /// Gets whether or not the Asset Sync feature is enabled in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/10/10 RCG 2.40.24        Created

        public bool AssetSyncEnabled
        {
            get
            {
                bool bEnabled = false;

                if (UseAssetSyncDefault == false)
                {
                    ReadUnloadedTable();

                    bEnabled = (m_byAssetSynchConfig & ASSET_SYNC_ENABLED_MASK) == ASSET_SYNC_ENABLED_MASK;
                }

                return bEnabled;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  03/10/10 RCG 2.40.24        Updating for table definition changed

        private void ParseData()
        {
            m_byTapControl = m_Reader.ReadByte();
            m_byInversionThreshold = m_Reader.ReadByte();
            m_byRemovalThreshold = m_Reader.ReadByte();
            m_byTapThreshold = m_Reader.ReadByte();
            m_uiWakeupDurationSecond = m_Reader.ReadUInt32();
            m_byFatalErrorRecoveryConfig = m_Reader.ReadByte();
            m_byAssetSynchConfig = m_Reader.ReadByte();
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets whether or not the Tap config default values should be used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/10/10 RCG 2.40.24        Created

        private bool UseTapDefault
        {
            get
            {
                ReadUnloadedTable();

                return (m_byTapControl & DEFAULT_MASK) == DEFAULT_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Fatal Error Recovery defaults should be used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/10/10 RCG 2.40.24        Created

        private bool UseFatalErrorRecoveryDefault
        {
            get
            {
                ReadUnloadedTable();

                return (m_byFatalErrorRecoveryConfig & DEFAULT_MASK) == DEFAULT_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Asset Sync defaults should be used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/10/10 RCG 2.40.24        Created

        private bool UseAssetSyncDefault
        {
            get
            {
                ReadUnloadedTable();

                return (m_byAssetSynchConfig & DEFAULT_MASK) == DEFAULT_MASK;
            }
        }

        #endregion

        #region Members

        private byte m_byFatalErrorRecoveryConfig;
        private byte m_byTapControl;
        private byte m_byInversionThreshold;
        private byte m_byRemovalThreshold;
        private byte m_byTapThreshold;
        private UInt32 m_uiWakeupDurationSecond;
        private byte m_byAssetSynchConfig;

        #endregion
    }

    /// <summary>
    /// MFG Table 2260 (Itron 212)
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  12/06/11 MAH 2.53.14        Renamed class and added retrieval of Load voltage detection delay
    //  04/06/16 AF  4.50.242 664961 Corrected the offset to Power up threshold
    //  04/15/16 AF  4.50.247 673485 Recorrected the offset to power up threshold now that fw has moved
    //                               demand mask outage seconds
    //
    public class OpenWayMFGTable2260ExtendedConfig : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 180;
        private const ushort TABLE_SIZE = 14;
        private const ushort EXTENDED_OUTAGE_CONFIG_OFFSET = 0;
        private const ushort LOAD_VOLTAGE_DETECTION_DELAY_OFFSET = 2;
        private const ushort POWER_UP_THRESHOLD_OFFSET = 10;

        /// <summary>
        /// Definition of outage lengths recognized for extended outages
        /// </summary>
        public enum ExtendedPowerOutageDurationEnum
        {
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("Extended Outage Recognition Off")]
            EXTENDED_POWER_OUTAGE_OFF = 0,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("3 Minutes")]
            EXTENDED_POWER_OUTAGE_3_MIN = 1,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("10 Minutes")]
            EXTENDED_POWER_OUTAGE_10_MIN = 2,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("1 Hour")]
            EXTENDED_POWER_OUTAGE_1_HR = 3,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("12 Hours")]
            EXTENDED_POWER_OUTAGE_12_HR = 4,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("1 Day")]
            EXTENDED_POWER_OUTAGE_1_DAY = 5,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("7 Days")]
            EXTENDED_POWER_OUTAGE_7_DAYS = 6,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("14 Days")]
            EXTENDED_POWER_OUTAGE_14_DAYS = 7,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("30 Days")]
            EXTENDED_POWER_OUTAGE_30_DAYS = 8,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("91 Days")]
            EXTENDED_POWER_OUTAGE_91_DAYS = 9,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("182 Days")]
            EXTENDED_POWER_OUTAGE_182_DAYS = 10,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("365 Days")]
            EXTENDED_POWER_OUTAGE_365_DAYS = 11,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("776 Days")]
            EXTENDED_POWER_OUTAGE_776_DAYS = 12,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("Extended Outage Recognition Disabled")]
            EXTENDED_POWER_OUTAGE_DISABLED = 255,
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MAH 2.50.30        Created
        //
        public OpenWayMFGTable2260ExtendedConfig(CPSEM psem)
            : base(psem, 2260, TABLE_OFFSET, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/25/11 MAH 2.50.30             Created
        //  01/26/16 AF  4.50.244 RTT 586620 Added read of power up threshold
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2260.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_eExtendedOutageConfig = (ExtendedPowerOutageDurationEnum)m_Reader.ReadByte();
                
                m_Reader.BaseStream.Position = LOAD_VOLTAGE_DETECTION_DELAY_OFFSET;
                m_usLoadVoltageDetectionDelay = m_Reader.ReadUInt16();

                m_Reader.BaseStream.Position = POWER_UP_THRESHOLD_OFFSET;
                m_usPowerUpThreshold = m_Reader.ReadUInt16();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether or not fatal error recovery is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MAH 2.50.30        Created
        public ExtendedPowerOutageDurationEnum ExtendedOutageDuration
        {
            get
            {
                ReadUnloadedTable();

                return m_eExtendedOutageConfig;
            }
            set
            {
                 m_DataStream.Position = EXTENDED_OUTAGE_CONFIG_OFFSET;

                 m_Writer.Write((byte)value );

                 base.Write(EXTENDED_OUTAGE_CONFIG_OFFSET, 1);
            }
        }

        /// <summary>
        /// Whether or not fatal error recovery is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/11 MAH 2.50.30        Created
        public ushort LoadVoltageDetectionDelay
        {
            get
            {
                ReadUnloadedTable();

                return m_usLoadVoltageDetectionDelay;
            }
            set
            {
                m_DataStream.Position = LOAD_VOLTAGE_DETECTION_DELAY_OFFSET;

                m_Writer.Write(value);

                base.Write(LOAD_VOLTAGE_DETECTION_DELAY_OFFSET, 2);
            }
        }

        /// <summary>
        /// Gets the value of the power up threshold.  If disabled, the value will be 0.
        /// If not supported, the value will be 0xFF.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/26/16 AF  4.50.244 RTT 586620 Added
        //
        public ushort PowerUpThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_usPowerUpThreshold;
            }
        }

        #endregion

        #region Members

        private ExtendedPowerOutageDurationEnum m_eExtendedOutageConfig;
        private ushort m_usLoadVoltageDetectionDelay;
        private ushort m_usPowerUpThreshold;

        #endregion
    }

}
