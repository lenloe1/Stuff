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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Threading;

namespace Itron.Metering.Device
{
    #region Public Definitions

    /// <summary>
    /// The Tx Gain values for the PLAN Comm Module
    /// </summary>
    public enum PLANTxGain : byte
    {
        /// <summary>
        /// 0 dB
        /// </summary>
        [EnumDescription("0 dB")]
        dB0 = 0,
        /// <summary>
        /// -3 dB
        /// </summary>
        [EnumDescription("-3 dB")]
        dBNeg3 = 1,
        /// <summary>
        /// -6 dB
        /// </summary>
        [EnumDescription("-6 dB")]
        dBNeg6 = 2,
        /// <summary>
        /// -9 dB
        /// </summary>
        [EnumDescription("-9 dB")]
        dBNeg9 = 3,
        /// <summary>
        /// -12 dB
        /// </summary>
        [EnumDescription("-12 dB")]
        dBNeg12 = 4,
        /// <summary>
        /// -15 dB
        /// </summary>
        [EnumDescription("-15 dB")]
        dBNeg15 = 5,
        /// <summary>
        /// -18 dB
        /// </summary>
        [EnumDescription("-18 dB")]
        dBNeg18 = 6,
        /// <summary>
        /// -21 dB
        /// </summary>
        [EnumDescription("-21 dB")]
        dBNeg21 = 7,
    }

    /// <summary>
    /// The Gain values for the PLAN Comm Module
    /// </summary>
    public enum PLANGain : byte
    {
        /// <summary>
        /// 0 dB
        /// </summary>
        [EnumDescription("0 dB")]
        dB0 = 0,
        /// <summary>
        /// 6 dB
        /// </summary>
        [EnumDescription("6 dB")]
        dB6 = 1,
        /// <summary>
        /// 12 dB
        /// </summary>
        [EnumDescription("12 dB")]
        dB12 = 2,
        /// <summary>
        /// 18 dB
        /// </summary>
        [EnumDescription("18 dB")]
        dB18 = 3,
        /// <summary>
        /// 24 dB
        /// </summary>
        [EnumDescription("24 dB")]
        dB24 = 4,
        /// <summary>
        /// 30 dB
        /// </summary>
        [EnumDescription("30 dB")]
        dB30 = 5,
        /// <summary>
        /// 36 dB
        /// </summary>
        [EnumDescription("36 dB")]
        dB36 = 6,
        /// <summary>
        /// 42 dB
        /// </summary>
        [EnumDescription("42 dB")]
        dB42 = 7,
        /// <summary>
        /// No Limit
        /// </summary>
        [EnumDescription("No Limit")]
        NoLimit = 8,
    }

    /// <summary>
    /// The various Repeater Modes for the PLAN Comm Module
    /// </summary>
    public enum PLANRepeaterModes : byte
    {
        /// <summary>
        /// Module is not a Repeater
        /// </summary>
        [EnumDescription("Not Repeater")]
        NotRepeater = 0,
        /// <summary>
        /// Module is Always a Repeater
        /// </summary>
        [EnumDescription("Always Repeater")]
        AlwayRepeater = 1,
        /// <summary>
        /// Module is not a Repeater (Repeater Call mode)
        /// </summary>
        [EnumDescription("Not Repeater - Repeater Call")]
        NotRepeaterRepeaterCall = 2,
        /// <summary>
        /// Module is a Repeater (Repeater Call mode)
        /// </summary>
        [EnumDescription("Repeater - Repeater Call")]
        RepeaterRepeaterCall = 3,
    }

    /// <summary>
    /// The Electrical Phase Delta for the PLAN Comm Module
    /// </summary>
    public enum PLANElectricalPhase : byte
    {
        /// <summary>
        /// Delta is Unknown
        /// </summary>
        [EnumDescription("Unknown")]
        Unknown = 0,
        /// <summary>
        /// 0 Degrees
        /// </summary>
        [EnumDescription("0 Degrees")]
        Degrees0 = 1,
        /// <summary>
        /// 60 Degrees
        /// </summary>
        [EnumDescription("60 Degrees")]
        Degrees60 = 2,
        /// <summary>
        /// 120 Degrees
        /// </summary>
        [EnumDescription("120 Degrees")]
        Degrees120 = 3,
        /// <summary>
        /// 180 Degrees
        /// </summary>
        [EnumDescription("180 Degrees")]
        Degrees180 = 4,
        /// <summary>
        /// -120 Degrees
        /// </summary>
        [EnumDescription("-120 Degrees")]
        DegreesNeg120 = 5,
        /// <summary>
        /// -60 Degrees
        /// </summary>
        [EnumDescription("-60 Degrees")]
        DegreesNeg60 = 6,
    }

    /// <summary>
    /// The Demodulation Methods used in the PLAN
    /// </summary>
    public enum PLANDemodMethod : byte
    {
        /// <summary>
        /// No Method
        /// </summary>
        NoMethod = 0,
        /// <summary>
        /// ASK0
        /// </summary>
        ASK0 = 1,
        /// <summary>
        /// ASK1
        /// </summary>
        ASK1 = 2,
        /// <summary>
        /// FSK
        /// </summary>
        FSK = 3,
    }

    /// <summary>
    /// Gets the Comm Module Mode
    /// </summary>
    public enum PLANCommModuleMode : byte
    {
        /// <summary>
        /// C12.22
        /// </summary>
        C1222 = 1,
        /// <summary>
        /// RFLAN
        /// </summary>
        RFLAN = 2,
        /// <summary>
        /// C12.22 Off
        /// </summary>
        C12122Off = 3,
    }

    #endregion

    /// <summary>
    /// PLAN Table 2210 (ITRP 162) - PLAN Firmware Version
    /// </summary>
    internal class PLANMFGTable2210 : OpenWayMFGTable2195
    {
        #region Constants

        private const uint TABLE_SIZE = 12;
        private const int TABLE_TIMEOUT = Timeout.Infinite;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2194">The Table 2194 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public PLANMFGTable2210(CPSEM psem, OpenWayMFGTable2194 table2194)
            : base(psem, TABLE_SIZE, TABLE_TIMEOUT, 2210, table2194)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Boot Loader version for the PLAN board
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte ARM7BootLoaderVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_byARM7BootLoaderVersion;
            }
        }

        /// <summary>
        /// Gets the Boot Loader revision for the PLAN board
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte ARM7BootLoaderRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_byARM7BootLoaderRevision;
            }
        }

        /// <summary>
        /// Gets the firmware version for the PLAN board
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte ARM7FWVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_byARM7AppVersion;
            }
        }

        /// <summary>
        /// Gets the firmware revision for the PLAN board
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte ARM7FWRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_byARM7AppRevsion;
            }
        }

        /// <summary>
        /// Gets the stack version for the PLAN board
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte DSPFWVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_byDSPFWVersion;
            }
        }

        /// <summary>
        /// Gets the stack revision for the PLAN board
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte DSPFWRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_byDSPFWRevision;
            }
        }

        /// <summary>
        /// Gets the EEPROM version for the PLAN board
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte DSPEEPROMVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_byDSPEEPROMVersion;
            }
        }

        /// <summary>
        /// Gets the EEPROM revision for the PLAN board
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte DSPEEPROMRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_byDSPEEPROMRevision;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data that was just read from the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        protected override void ParseData()
        {
            m_byARM7BootFirmwareType = m_Reader.ReadByte();
            m_byARM7BootLoaderVersion = m_Reader.ReadByte();
            m_byARM7BootLoaderRevision = m_Reader.ReadByte();
            m_byARM7AppFirmwareType = m_Reader.ReadByte();
            m_byARM7AppVersion = m_Reader.ReadByte();
            m_byARM7AppRevsion = m_Reader.ReadByte();
            m_byDSPFirmwareType = m_Reader.ReadByte();
            m_byDSPFWVersion = m_Reader.ReadByte();
            m_byDSPFWRevision = m_Reader.ReadByte();
            m_byDSPEEPROMType = m_Reader.ReadByte();
            m_byDSPEEPROMVersion = m_Reader.ReadByte();
            m_byDSPEEPROMRevision = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private byte m_byARM7BootFirmwareType;
        private byte m_byARM7BootLoaderVersion;
        private byte m_byARM7BootLoaderRevision;
        private byte m_byARM7AppFirmwareType;
        private byte m_byARM7AppVersion;
        private byte m_byARM7AppRevsion;
        private byte m_byDSPFirmwareType;
        private byte m_byDSPFWVersion;
        private byte m_byDSPFWRevision;
        private byte m_byDSPEEPROMType;
        private byte m_byDSPEEPROMVersion;
        private byte m_byDSPEEPROMRevision;

        #endregion
    }

    /// <summary>
    /// PLAN Table 2211 (ITRP 163) - PLAN Factory Data
    /// </summary>
    internal class PLANMFGTable2211 : OpenWayMFGTable2195
    {
        #region Constants

        private const uint TABLE_SIZE = 65;
        private const int TABLE_TIMEOUT = Timeout.Infinite;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2194">The Table 2194 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public PLANMFGTable2211(CPSEM psem, OpenWayMFGTable2194 table2194)
            : base(psem, TABLE_SIZE, TABLE_TIMEOUT, 2211, table2194)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Max Transmitting Gain
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public PLANTxGain MaxTxGain
        {
            get
            {
                ReadUnloadedTable();

                return m_PhyMaxTxGain;
            }
        }

        /// <summary>
        /// Gets the Max Recieving Gain
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public PLANGain MaxRxGain
        {
            get
            {
                ReadUnloadedTable();

                return m_PhyMaxRxGain;
            }
        }

        /// <summary>
        /// Gets the Search Initiator Gain
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public PLANGain SearchInitiatorGain
        {
            get
            {
                ReadUnloadedTable();

                return m_PhySearchInitiatorGain;
            }
        }

        /// <summary>
        /// Gets the Repeater Mode of the comm module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public PLANRepeaterModes RepeaterMode
        {
            get
            {
                ReadUnloadedTable();

                return m_PhyRepeater;
            }
        }

        /// <summary>
        /// Gets the Sync Confirm Timeout in seconds
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public ushort SyncConfirmTimeout
        {
            get
            {
                ReadUnloadedTable();

                return m_usPhySyncConfirmTimeout;
            }
        }

        /// <summary>
        /// Gets the Frame Not Ok Timeout in seconds
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public ushort FrameNotOkTimeout
        {
            get
            {
                ReadUnloadedTable();

                return m_usPhyTimeoutFrameNotOk;
            }
        }

        /// <summary>
        /// Gets the Not Addressed Timeout in minutes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public ushort NotAddressedTimeout
        {
            get
            {
                ReadUnloadedTable();

                return m_usPhyTimeoutNotAddressed;
            }
        }

        /// <summary>
        /// Gets the Search Initiator Timeout in minutes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public ushort SearchInitiatorTimeout
        {
            get
            {
                ReadUnloadedTable();

                return m_usPhyTimeoutSearchInitiator;
            }
        }

        /// <summary>
        /// Gets the Local MAC Address of the Comm Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public ushort LocalMACAddress
        {
            get
            {
                ReadUnloadedTable();

                return m_usLocalMACAddress;
            }
        }

        /// <summary>
        /// Gets the Local System Title of the Comm Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte[] LocalSystemTitle
        {
            get
            {
                ReadUnloadedTable();

                return m_LocalSystemTitle;
            }
        }

        /// <summary>
        /// Gets the Initiator MAC Address
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public ushort InitiatorMACAddress
        {
            get
            {
                ReadUnloadedTable();

                return m_usPLCInitiatiorMACAddress;
            }
        }

        /// <summary>
        /// Gets the Inititator System Title
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte[] InitiatorSystemTitle
        {
            get
            {
                ReadUnloadedTable();

                return m_InitiatorSystemTitle;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data that was just read from the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        protected override void ParseData()
        {
            m_usLocalMACAddress = m_Reader.ReadUInt16();
            m_LocalSystemTitle = m_Reader.ReadBytes(6);
            m_uiRFMACAddress = m_Reader.ReadUInt32();
            m_byUtilityID = m_Reader.ReadByte();
            m_usPLCInitiatiorMACAddress = m_Reader.ReadUInt16();
            m_InitiatorSystemTitle = m_Reader.ReadBytes(6);
            m_byPhyVersion = m_Reader.ReadByte();
            m_byPhyRevision = m_Reader.ReadByte();
            m_PhyMaxTxGain = (PLANTxGain)m_Reader.ReadByte();
            m_PhyMaxRxGain = (PLANGain)m_Reader.ReadByte();
            m_PhySearchInitiatorGain = (PLANGain)m_Reader.ReadByte();
            m_PhyRepeater = (PLANRepeaterModes)m_Reader.ReadByte();
            m_usPhyDphi = m_Reader.ReadUInt16();
            m_usPhyAlpha = m_Reader.ReadUInt16();
            m_usPhyBeta = m_Reader.ReadUInt16();
            m_usPhyTGlitch = m_Reader.ReadUInt16();
            m_usPhySyncConfirmTimeout = m_Reader.ReadUInt16();
            m_usPhyTimeoutFrameNotOk = m_Reader.ReadUInt16();
            m_usPhyTimeoutNotAddressed = m_Reader.ReadUInt16();
            m_usPhyTimeoutSearchInitiator = m_Reader.ReadUInt16();
            m_byPhyLASP1 = m_Reader.ReadByte();
            m_byPhyLSAP2 = m_Reader.ReadByte();
            m_byPhyLSAPSelector = m_Reader.ReadByte(); 
            m_usNoFrameFromNetworkRxTimeout = m_Reader.ReadUInt16();
            m_usEaitLoadCongifRequestTimeout = m_Reader.ReadUInt16();
            m_usGetAllSFSKPeriod = m_Reader.ReadUInt16();
            m_usGetAllMIBPeriod = m_Reader.ReadUInt16();
            m_usGetMainDetailsPeriod = m_Reader.ReadUInt16();
            m_usGetSyncDetailsPeriod = m_Reader.ReadUInt16();
            m_byResetDSPSimilarSFSKThreshold = m_Reader.ReadByte();
            m_usPayloadC1222Negotiation = m_Reader.ReadUInt16();
            m_usWaitC1222ConfigTimeout = m_Reader.ReadUInt16();
            m_usWaitC1222TableReadResponse = m_Reader.ReadUInt16();
        }

        #endregion

        #region Member Variables

        private ushort m_usLocalMACAddress;
        private byte[] m_LocalSystemTitle;
        private uint m_uiRFMACAddress;
        private byte m_byUtilityID;
        private ushort m_usPLCInitiatiorMACAddress;
        private byte[] m_InitiatorSystemTitle;
        private byte m_byPhyVersion;
        private byte m_byPhyRevision;
        private PLANTxGain m_PhyMaxTxGain;
        private PLANGain m_PhyMaxRxGain;
        private PLANGain m_PhySearchInitiatorGain;
        private PLANRepeaterModes m_PhyRepeater;
        private ushort m_usPhyDphi;
        private ushort m_usPhyAlpha;
        private ushort m_usPhyBeta;
        private ushort m_usPhyTGlitch;
        private ushort m_usPhySyncConfirmTimeout;
        private ushort m_usPhyTimeoutFrameNotOk;
        private ushort m_usPhyTimeoutNotAddressed;
        private ushort m_usPhyTimeoutSearchInitiator;
        private byte m_byPhyLASP1;
        private byte m_byPhyLSAP2;
        private byte m_byPhyLSAPSelector;
        private ushort m_usNoFrameFromNetworkRxTimeout;
        private ushort m_usEaitLoadCongifRequestTimeout;
        private ushort m_usGetAllSFSKPeriod;
        private ushort m_usGetAllMIBPeriod;
        private ushort m_usGetMainDetailsPeriod;
        private ushort m_usGetSyncDetailsPeriod;
        private byte m_byResetDSPSimilarSFSKThreshold;
        private ushort m_usPayloadC1222Negotiation;
        private ushort m_usWaitC1222ConfigTimeout;
        private ushort m_usWaitC1222TableReadResponse;

        #endregion
    }

    /// <summary>
    /// PLAN Table 2212 (ITRP 164) - Electrical Status
    /// </summary>
    internal class PLANMFGTable2212 : OpenWayMFGTable2195
    {
        #region Constants

        private const uint TABLE_SIZE = 29;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2194">The Table 2194 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public PLANMFGTable2212(CPSEM psem, OpenWayMFGTable2194 table2194)
            : base(psem, TABLE_SIZE, TABLE_TIMEOUT, 2212, table2194)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Electrical Phase Delta
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.14 N/A    Created

        public PLANElectricalPhase DeltaPhase
        {
            get
            {
                ReadUnloadedTable();

                return m_DeltaPhase;
            }
        }

        /// <summary>
        /// Gets the Link attenutation for Channel 0 in dB
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.14 N/A    Created

        public byte Channel0LinkAttenuation
        {
            get
            {
                ReadUnloadedTable();

                return m_byAttenuationChannel0;
            }
        }

        /// <summary>
        /// Gets the Noise level for Channel 0 in dBuV
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.14 N/A    Created

        public byte Channel0Noise
        {
            get
            {
                ReadUnloadedTable();

                return m_byNoiseChannel0;
            }
        }

        /// <summary>
        /// Gets the Link attenutation for Channel 1 in dB
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.14 N/A    Created

        public byte Channel1LinkAttenuation
        {
            get
            {
                ReadUnloadedTable();

                return m_byAttenuationChannel1;
            }
        }

        /// <summary>
        /// Gets the Noise level for Channel 1 in dBuV
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.14 N/A    Created

        public byte Channel1Noise
        {
            get
            {
                ReadUnloadedTable();

                return m_byNoiseChannel1;
            }
        }

        /// <summary>
        /// Gets the Credit Level
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/24/10 RCG 2.40.14 N/A    Created

        public byte CreditLevel
        {
            get
            {
                ReadUnloadedTable();

                return m_byCreditLevel;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data that was just read from the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        protected override void ParseData()
        {
            m_DeltaPhase = (PLANElectricalPhase)m_Reader.ReadByte();
            m_SyncDeltaPhaseAccurate = m_Reader.ReadUInt16();
            m_SyncDeltaChip = m_Reader.ReadSByte();
            m_DemodMethod = (PLANDemodMethod)m_Reader.ReadByte();
            m_byDemodReceptionGain = m_Reader.ReadByte();
            m_usDemodChannel0Signal = m_Reader.ReadUInt16();
            m_usDemodChannel0Noise = m_Reader.ReadUInt16();
            m_usDemodChannel1Signal = m_Reader.ReadUInt16();
            m_usDemodChannel1Noise = m_Reader.ReadUInt16();
            m_usDemodASKThreshold = m_Reader.ReadUInt16();
            m_usACMainFrequency = m_Reader.ReadUInt16();
            m_usACMainGlitchCounter = m_Reader.ReadUInt16();
            m_usACMainCutCounter = m_Reader.ReadUInt16();
            m_byCreditLevel = m_Reader.ReadByte();
            m_byLastICRx = m_Reader.ReadByte();
            m_byLastCCRx = m_Reader.ReadByte();
            m_byAttenuationChannel0 = m_Reader.ReadByte();
            m_byAttenuationChannel1 = m_Reader.ReadByte();
            m_byNoiseChannel0 = m_Reader.ReadByte();
            m_byNoiseChannel1 = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private PLANElectricalPhase m_DeltaPhase;
        private ushort m_SyncDeltaPhaseAccurate;
        private sbyte m_SyncDeltaChip;
        private PLANDemodMethod m_DemodMethod;
        private byte m_byDemodReceptionGain;
        private ushort m_usDemodChannel0Signal;
        private ushort m_usDemodChannel0Noise;
        private ushort m_usDemodChannel1Signal;
        private ushort m_usDemodChannel1Noise;
        private ushort m_usDemodASKThreshold;
        private ushort m_usACMainFrequency;
        private ushort m_usACMainGlitchCounter;
        private ushort m_usACMainCutCounter;
        private byte m_byCreditLevel;
        private byte m_byLastICRx;
        private byte m_byLastCCRx;
        private byte m_byAttenuationChannel0;
        private byte m_byAttenuationChannel1;
        private byte m_byNoiseChannel0;
        private byte m_byNoiseChannel1;

        #endregion
    }

    /// <summary>
    /// PLAN Table 2215 (ITRP 167) - Comm Module Information
    /// </summary>
    internal class PLANMFGTable2215 : OpenWayMFGTable2195
    {
        #region Constants

        private const int TABLE_TIMEOUT = Timeout.Infinite;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2194">The Table 2194 object for the current device.</param>
        /// <param name="table0">The Table 0 object for the current device</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public PLANMFGTable2215(CPSEM psem, OpenWayMFGTable2194 table2194, CTable00 table0)
            : base(psem, GetTableSize(table0), TABLE_TIMEOUT, 2215, table2194)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the CellID of the Comm Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public byte[] CellID
        {
            get
            {
                ReadUnloadedTable();

                return m_CellID;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data that was just read from the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        protected override void ParseData()
        {
            m_CommModuleTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_byIsCommModule = m_Reader.ReadByte();
            m_byIsRelay = m_Reader.ReadByte();
            m_byEndpointType = m_Reader.ReadByte();
            m_RunMode = (PLANCommModuleMode)m_Reader.ReadByte();
            m_TimeRefITP = m_Reader.ReadBytes(5);
            m_CellID = m_Reader.ReadBytes(2);
            m_LastCellID = m_Reader.ReadBytes(2);
            m_MyNativeAddress = m_Reader.ReadBytes(6);
            m_NativeAddress = m_Reader.ReadBytes(6);
            m_UtiltiyID = m_Reader.ReadByte();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="table0">The Table 0 object for the current device</param>
        /// <returns>The size of the table in bytes</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        private static uint GetTableSize(CTable00 table0)
        {
            return 26 + table0.LTIMESize;
        }

        #endregion

        #region Member Variables

        private DateTime m_CommModuleTime;
        private byte m_byIsCommModule;
        private byte m_byIsRelay;
        private byte m_byEndpointType;
        private PLANCommModuleMode m_RunMode;
        private byte[] m_TimeRefITP;
        private byte[] m_CellID;
        private byte[] m_LastCellID;
        private byte[] m_MyNativeAddress;
        private byte[] m_NativeAddress;
        private byte m_UtiltiyID;

        #endregion
    }
}
