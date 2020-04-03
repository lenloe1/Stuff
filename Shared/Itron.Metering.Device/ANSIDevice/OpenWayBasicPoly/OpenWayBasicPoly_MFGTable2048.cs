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
//                            Copyright © 2009 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    #region Enumerations

    /// <summary>
    /// Phase rotations.
    /// </summary>
    public enum PhaseRotation : byte
    {
        /// <summary>
        /// ABC Phase Rotation.
        /// </summary>
        ABC = 0,
        /// <summary>
        /// CBA Phase Rotation.
        /// </summary>
        CBA = 1,
        /// <summary>
        /// No phase roation (Single Phase)
        /// </summary>
        NoRotation = 2,
        /// <summary>
        /// Can use any phase rotation
        /// </summary>
        Any = 3,
    }

    /// <summary>
    /// Service Types
    /// </summary>
    public enum ServiceTypes : byte
    {
        /// <summary>
        /// 3 Element 3 Phase 4 Wire WYE
        /// </summary>
        [EnumDescription("3 Element 3 Phase 4 Wire WYE")]
        ThreeElem3Phase4WireWYE = 0,
        /// <summary>
        /// 2.5 Element 3 Phase 4 Wire WYE
        /// </summary>
        [EnumDescription("2.5 Element 3 Phase 4 Wire WYE (6S/46S)")]
        TwoAndHalfElem3Phase4WireWYE6S46S = 1,
        /// <summary>
        /// 2 Element Network
        /// </summary>
        [EnumDescription("2 Element Network")]
        TwoElemNetwork = 2,
        /// <summary>
        /// 3 Element 3 Phase 4 Wire Delta
        /// </summary>
        [EnumDescription("3 Element 3 Phase 4 Wire Delta")]
        ThreeElem3Phase4WireDelta = 3,
        /// <summary>
        /// 2 Element 3 Phase 4 Wire WYE
        /// </summary>
        [EnumDescription("2 Element 3 Phase 4 Wire WYE")]
        TwoElem3Phase4WireWYE = 4,
        /// <summary>
        /// 2 Element 3 Phase 3 Wire Delta
        /// </summary>
        [EnumDescription("2 Element 3 Phase 3 Wire Delta")]
        TwoElem3Phase3WireDelta = 5,
        /// <summary>
        /// 2 Element 3 Phase 4 Wire Delta
        /// </summary>
        [EnumDescription("2 Element 3 Phase 4 Wire Delta")]
        TwoElem3Phase4WireDelta = 6,
        /// <summary>
        /// 2 Element Single Phase
        /// </summary>
        [EnumDescription("2 Element Single Phase")]
        TwoElemSinglePhase = 7,
        /// <summary>
        /// 1 Element Single Phase 3 Wire 
        /// </summary>
        [EnumDescription("1 Element Single Phase 3 Wire")]
        OneElemSinglePhase3Wire = 8,
        /// <summary>
        /// 1 Element Single Phase 2 Wire
        /// </summary>
        [EnumDescription("1 Element Single Phase 2 Wire")]
        OneElemSinglePhase2Wire = 9,
        /// <summary>
        /// 2.5 Element 3 Phase 4 Wire Wye – 9S
        /// </summary>
        [EnumDescription("2.5 Element 3 Phase 4 Wire WYE (9S)")]
        TwoAndHalfElem3Phase4WireWYE9S = 10,
        /// <summary>
        /// Auto Service Sense
        /// </summary>
        [EnumDescription("Auto Service Sense")]
        AutoServiceSense = 255,
    }

    #endregion

    /// <summary>
    /// Table 2048 class for the OpenWay PolyPhase meters.
    /// </summary>

    internal class CTable2048_OpenWayPoly : CTable2048_OpenWay
    {
        #region Public Methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public CTable2048_OpenWayPoly(CPSEM psem)
            : base(psem)
        {
            m_SiteScanConfig = new OpenWayPolySiteScanConfig(psem, m_2048Header.SiteScanOffset);
            m_HistoryLogConfig = new OpenWayBasicPoly_HistoryLogConfig(psem, m_2048Header.HistoryLogOffset);
            m_DemandConfig = new OpenWayBasicPoly_DemandConfig(psem, m_2048Header.DemandOffset);
        }

        #endregion

        #region Public Properties

        public OpenWayPolySiteScanConfig SiteScanConfig
        {
            get
            {
                return m_SiteScanConfig;
            }
        }

        #endregion

        #region Member Variables

        private OpenWayPolySiteScanConfig m_SiteScanConfig;

        #endregion
    }

    /// <summary>
    /// Sub table for the SiteScan configuration.
    /// </summary>

    internal class OpenWayPolySiteScanConfig : ANSISubTable
    {
        #region Constants

        private const ushort TABLE_SIZE = 38;

        // Options Masks
        private const byte DIAG1_ENABLE_MASK = 0x01;
        private const byte DIAG2_ENABLE_MASK = 0x02;
        private const byte DIAG3_ENABLE_MASK = 0x04;
        private const byte DIAG4_ENABLE_MASK = 0x08;
        private const byte DIAG5_ENABLE_MASK = 0x10;
        private const byte DIAG6_ENABLE_MASK = 0x20;

        // Phase Masks
        private const byte PHASE_A_MASK = 0x01;
        private const byte PHASE_B_MASK = 0x02;
        private const byte PHASE_C_MASK = 0x04;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="offset">The offset of the sub table into 2048</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public OpenWayPolySiteScanConfig(CPSEM psem, ushort offset)
            : base(psem, 2048, offset, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayPolySiteScanConfig.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the subtable to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "OpenWayPolySiteScanConfig.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_byOptions);
            m_Writer.Write(m_usCurrentLeadTolerance);
            m_Writer.Write(m_usCurrentLagTolerance);
            m_Writer.Write(m_usVoltageLeadTolerance);
            m_Writer.Write(m_usVoltageLagTolerance);
            m_Writer.Write(m_byVoltagePercentDeviation);
            m_Writer.Write(m_fCurrentThreshold);
            m_Writer.Write(m_usCurrentPhaseDeviation);
            m_Writer.Write(m_byPhaseEnable);
            m_Writer.Write(m_usVoltageThreshold);
            m_Writer.Write(m_byPhaseRotation);
            m_Writer.Write(m_byServiceType);
            m_Writer.Write(m_byDelayUntillServiceSense);
            m_Writer.Write(m_byMinCurrent);
            m_Writer.Write(m_usNominalVoltage);
            m_Writer.Write(m_bySnapshotTrigger);
            m_Writer.Write(m_usMinDuration);
            m_Writer.Write(m_fMaxTHDV);
            m_Writer.Write(m_fMaxTDDI);
            m_Writer.Write(m_usDCVoltageDetectionThreshold);

            return base.Write();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not Diagnostic 1 is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag1Enabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byOptions & DIAG1_ENABLE_MASK) == DIAG1_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 2 is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag2Enabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byOptions & DIAG2_ENABLE_MASK) == DIAG2_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 3 is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag3Enabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byOptions & DIAG3_ENABLE_MASK) == DIAG3_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 4 is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag4Enabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byOptions & DIAG4_ENABLE_MASK) == DIAG4_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 5 is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag5Enabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byOptions & DIAG5_ENABLE_MASK) == DIAG5_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 6 is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDaig6Enabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byOptions & DIAG6_ENABLE_MASK) == DIAG6_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 1 Snapshot is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag1SnapshotEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySnapshotTrigger & DIAG1_ENABLE_MASK) == DIAG1_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 2 Snapshot is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag2SnapshotEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySnapshotTrigger & DIAG2_ENABLE_MASK) == DIAG2_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 3 Snapshot is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag3SnapshotEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySnapshotTrigger & DIAG3_ENABLE_MASK) == DIAG3_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 4 Snapshot is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag4SnapshotEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySnapshotTrigger & DIAG4_ENABLE_MASK) == DIAG4_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 5 Snapshot is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDiag5SnapshotEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySnapshotTrigger & DIAG5_ENABLE_MASK) == DIAG5_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 6 Snapshot is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsDaig6SnapshotEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySnapshotTrigger & DIAG6_ENABLE_MASK) == DIAG6_ENABLE_MASK;
            }
        }

        /// <summary>
        /// Gets or sets the Current Lead Tolerance for Diag 1
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag1CurrentLeadTolerance
        {
            get
            {
                ReadUnloadedTable();

                return m_usCurrentLeadTolerance / 10.0f;
            }
            set
            {
                m_usCurrentLeadTolerance = Convert.ToUInt16(value * 10.0);
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Current Lag Tolerance for Daig 1
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag1CurrentLagTolerance
        {
            get
            {
                ReadUnloadedTable();

                return m_usCurrentLagTolerance;
            }
            set
            {
                m_usCurrentLagTolerance = Convert.ToUInt16(value * 10.0);
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Voltage Lead Tolerance for Diag 1
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag1VoltageLeadTolerance
        {
            get
            {
                ReadUnloadedTable();

                return m_usVoltageLeadTolerance / 10.0f;
            }
            set
            {
                m_usVoltageLeadTolerance = Convert.ToUInt16(value * 10.0);
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Voltage Lag Tolerance for Diag 1
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag1VoltageLagTolerance
        {
            get
            {
                ReadUnloadedTable();

                return m_usVoltageLagTolerance / 10.0f;
            }
            set
            {
                m_usVoltageLagTolerance = Convert.ToUInt16(value * 10.0);
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Voltage Percent Deviation for Diag 2
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public byte Diag2VoltagePercentDeviation
        {
            get
            {
                ReadUnloadedTable();

                return m_byVoltagePercentDeviation;
            }
            set
            {
                m_byVoltagePercentDeviation = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Current Threshold for Diag 3
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag3CurrentThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_fCurrentThreshold;
            }
            set
            {
                m_fCurrentThreshold = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Current Phase Deviation for Diag 4
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag4CurrentPhaseDeviation
        {
            get
            {
                ReadUnloadedTable();

                return m_usCurrentPhaseDeviation / 10.0f;
            }
            set
            {
                m_usCurrentPhaseDeviation = Convert.ToUInt16(value * 10.0);
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets whether or not Phase A is enabled for Diag 3
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsPhaseAEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byPhaseEnable & PHASE_A_MASK) == PHASE_A_MASK;
            }
            set
            {
                if (value == true)
                {
                    m_byPhaseEnable = (byte)(m_byPhaseEnable | PHASE_A_MASK);
                }
                else
                {
                    m_byPhaseEnable = (byte)(m_byPhaseEnable & ~PHASE_A_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not Phase B is enabled for Diag 3
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsPhaseBEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byPhaseEnable & PHASE_B_MASK) == PHASE_B_MASK;
            }
            set
            {
                if (value == true)
                {
                    m_byPhaseEnable = (byte)(m_byPhaseEnable | PHASE_B_MASK);
                }
                else
                {
                    m_byPhaseEnable = (byte)(m_byPhaseEnable & ~PHASE_B_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not Phase C is enabled for Diag 3
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsPhaseCEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byPhaseEnable & PHASE_C_MASK) == PHASE_C_MASK;
            }
            set
            {
                if (value == true)
                {
                    m_byPhaseEnable = (byte)(m_byPhaseEnable | PHASE_C_MASK);
                }
                else
                {
                    m_byPhaseEnable = (byte)(m_byPhaseEnable & ~PHASE_C_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Voltage Threshold used for Missing Phase Non Fatal
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float VoltageThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_usVoltageThreshold / 40.0f;
            }
            set
            {
                m_usVoltageThreshold = Convert.ToUInt16(value * 40.0);
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Phase Rotation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public PhaseRotation PhaseRotation
        {
            get
            {
                ReadUnloadedTable();

                return (PhaseRotation)m_byPhaseRotation;
            }
            set
            {
                m_byPhaseRotation = (byte)value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Service Type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public ServiceTypes ServiceType
        {
            get
            {
                ReadUnloadedTable();

                return (ServiceTypes)m_byServiceType;
            }
            set
            {
                m_byServiceType = (byte)value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time to delay before service sense
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public byte DelayUntillServiceSense
        {
            get
            {
                ReadUnloadedTable();

                return m_byDelayUntillServiceSense;
            }
            set
            {
                m_byDelayUntillServiceSense = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Minimum Current for diag 4
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag4MinCurrent
        {
            get
            {
                ReadUnloadedTable();

                return m_byMinCurrent / 10.0f;
            }
            set
            {
                if (value >= 0.5 && value <= 5.0)
                {
                    m_byMinCurrent = Convert.ToByte(value * 10.0);
                    State = TableState.Dirty;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Minimum Current must be between 0.5 and 5.0");
                }
            }
        }

        /// <summary>
        /// Gets or sets the nominal voltage. 0 means sense nominal voltage.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float NominalVoltage
        {
            get
            {
                ReadUnloadedTable();

                return m_usNominalVoltage / 40.0f;
            }
            set
            {
                m_usNominalVoltage = Convert.ToUInt16(value * 40.0);
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the minimum duration for Diag 6
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public ushort Diag6MinDuration
        {
            get
            {
                ReadUnloadedTable();

                return m_usMinDuration;
            }
            set
            {
                m_usMinDuration = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the maximum THD V per phase
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag6MaxTHDV
        {
            get
            {
                ReadUnloadedTable();

                return m_fMaxTHDV;
            }
            set
            {
                m_fMaxTHDV = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the maximum TDD I per phase for Diag 6
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag6MaxTDDI
        {
            get
            {
                ReadUnloadedTable();

                return m_fMaxTDDI;
            }
            set
            {
                m_fMaxTDDI = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the DC Voltage Detection Threshold for Diag 5
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public ushort Diag5DCVoltageDetectionThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_usDCVoltageDetectionThreshold;
            }
            set
            {
                m_usDCVoltageDetectionThreshold = value;
                State = TableState.Dirty;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data from the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        private void ParseData()
        {
            m_byOptions = m_Reader.ReadByte();
            m_usCurrentLeadTolerance = m_Reader.ReadUInt16();
            m_usCurrentLagTolerance = m_Reader.ReadUInt16();
            m_usVoltageLeadTolerance = m_Reader.ReadUInt16();
            m_usVoltageLagTolerance = m_Reader.ReadUInt16();
            m_byVoltagePercentDeviation = m_Reader.ReadByte();
            m_fCurrentThreshold = m_Reader.ReadSingle();
            m_usCurrentPhaseDeviation = m_Reader.ReadUInt16();
            m_byPhaseEnable = m_Reader.ReadByte();
            m_usVoltageThreshold = m_Reader.ReadUInt16();
            m_byPhaseRotation = m_Reader.ReadByte();
            m_byServiceType = m_Reader.ReadByte();
            m_byDelayUntillServiceSense = m_Reader.ReadByte();
            m_byMinCurrent = m_Reader.ReadByte();
            m_usNominalVoltage = m_Reader.ReadUInt16();
            m_bySnapshotTrigger = m_Reader.ReadByte();
            m_usMinDuration = m_Reader.ReadUInt16();
            m_fMaxTHDV = m_Reader.ReadSingle();
            m_fMaxTDDI = m_Reader.ReadSingle();
            m_usDCVoltageDetectionThreshold = m_Reader.ReadUInt16();
        }

        #endregion

        #region Member Variables

        private byte m_byOptions;
        private ushort m_usCurrentLeadTolerance;
        private ushort m_usCurrentLagTolerance;
        private ushort m_usVoltageLeadTolerance;
        private ushort m_usVoltageLagTolerance;
        private byte m_byVoltagePercentDeviation;
        private float m_fCurrentThreshold;
        private ushort m_usCurrentPhaseDeviation;
        private byte m_byPhaseEnable;
        private ushort m_usVoltageThreshold;
        private byte m_byPhaseRotation;
        private byte m_byServiceType;
        private byte m_byDelayUntillServiceSense;
        private byte m_byMinCurrent;
        private ushort m_usNominalVoltage;
        private byte m_bySnapshotTrigger;
        private ushort m_usMinDuration;
        private float m_fMaxTHDV;
        private float m_fMaxTDDI;
        private ushort m_usDCVoltageDetectionThreshold;

        #endregion
    }

    /// <summary>
    /// Class that represents the history log configuration data stored in table 2048
    /// for the CENTRON_AMI meter.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  05/07/09 RCG 2.20.03    N/A    Created

    public class OpenWayBasicPoly_HistoryLogConfig : CENTRON_AMI_HistoryLogConfig
    {
        #region Public Methods

        /// <summary>
        /// Constructor for CENTRON_AMI History Log Config class
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="Offset"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created

        public OpenWayBasicPoly_HistoryLogConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        /// <summary>
        /// Constructor used to get Event Data from the EDL file
        /// </summary>
        /// <param name="EDLBinaryReader"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created

        public OpenWayBasicPoly_HistoryLogConfig(PSEMBinaryReader EDLBinaryReader)
            : base(EDLBinaryReader)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the list of History Log event specfic to the CENTRON_AMI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version    Issue#        Description
        //  -------- --- -------    ------        -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A           Created
        //  08/12/11 jrf 2.52.02    TREQ2709      Changing Register Firmware Download Status event to
        //                                        Firmware Download Event Log Full event.
        //  03/12/12 jrf 2.53.49    192582/192583 Adding missing events to the list.
        //  11/18/13 jrf 3.50.06    TQ 9482       Added TOU Season Changed event.
        //  06/24/15 AF  4.20.14    593126        Added Generic event
        //  02/04/16 PGH 4.50.226   RTT556309     Added Temperature events
        //  04/21/16 AF  4.50.252   WR604349      Changed HAN_LOAD_CONTROL_EVENT_SENT to ERT_242_COMMAND_REQUEST
        //  05/20/16 MP  4.50.270   WR685690      Added support for EVENT_HARDWARE_ERROR_DETECTION
        //  07/12/16 MP  4.70.7     WR688986      Changed how event descriptions were accessed
        //  07/14/16 MP  4.70.7     WR688986      Fixed copy-paste error on VOLT_HOUR_ABOVE_THRESHOLD
        //  07/27/16 MP  4.70.9     WR600059      Added definition for event 137 (NETWORK_TIME_UNAVAILABLE)
        //  07/29/16 MP  4.70.11    WR704220      Added support for events 213 and 214 (WRONG_CONFIG_CRC and CHECK_CONFIG_CRC)
        //  10/26/16 jrf 4.70.28    WR230427      Added missing event PERIODIC_READ(125) and CTE_EVENT(140).
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

                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT), m_usEvent16_31, (UInt16)(Event_16_31.SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.HIST_LOG_CLEARED), m_usEvent16_31, (UInt16)(Event_16_31.HIST_LOG_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.DEMAND_RESET), m_usEvent16_31, (UInt16)(Event_16_31.DEMAND_RESET));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.SELF_READ_OCCURRED), m_usEvent16_31, (UInt16)(Event_16_31.SELF_READ_OCCURRED));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.TOU_SEASON_CHANGED), m_usEvent16_31, (UInt16)(Event_16_31.TOU_SEASON_CHANGED));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.PENDING_TABLE_ACTIVATION), m_usEvent16_31, (UInt16)(Event_16_31.PENDING_TABLE_ACTIVATION));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.PENDING_TABLE_CLEAR), m_usEvent16_31, (UInt16)(Event_16_31.PENDING_TABLE_CLEAR));


                // Add Event 32 - 47 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.CPC_COMM_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.CPC_COMM_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.LOSS_OF_PHASE_RESTORE), m_usEvent32_47, (UInt16)(Event_32_47.LOSS_OF_PHASE_RESTORE));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.LOSS_OF_PHASE), m_usEvent32_47, (UInt16)(Event_32_47.LOSS_OF_PHASE));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.ENTER_TEST_MODE), m_usEvent32_47, (UInt16)(Event_32_47.ENTER_TEST_MODE));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.EXIT_TEST_MODE), m_usEvent32_47, (UInt16)(Event_32_47.EXIT_TEST_MODE));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.METER_REPROGRAMMED), m_usEvent32_47, (UInt16)(Event_32_47.METER_REPROGRAMMED));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.ILLEGAL_CONFIG_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.ILLEGAL_CONFIG_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.TOU_SCHEDULE_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.TOU_SCHEDULE_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.MASS_MEMORY_ERROR), m_usEvent32_47, (UInt16)(Event_32_47.MASS_MEMORY_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.LOW_BATTERY), m_usEvent32_47, (UInt16)(Event_32_47.LOW_BATTERY));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.REGISTER_FULL_SCALE), m_usEvent32_47, (UInt16)(Event_32_47.REGISTER_FULL_SCALE));


                // Add Event 48 - 63 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.REVERSE_POWER_FLOW), m_usEvent48_63, (UInt16)(Event_48_63.REVERSE_POWER_FLOW));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.INVERSION_TAMPER), m_usEvent48_63, (UInt16)(Event_48_63.INVERSION_TAMPER));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.REMOVAL_TAMPER), m_usEvent48_63, (UInt16)(Event_48_63.REMOVAL_TAMPER));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG1_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG1_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG2_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG2_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG3_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG3_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG4_ACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG4_ACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG1_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG1_INACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG2_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG2_INACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG3_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG3_INACTIVE));
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.SS_DIAG4_INACTIVE), m_usEvent48_63, (UInt16)(Event_48_63.SS_DIAG4_INACTIVE));



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
                AddEventItem(Enum.GetName(typeof(Event_80_95), Event_80_95.SITESCAN_ERROR_CLEARED), m_usEvent80_95, (UInt16)(Event_80_95.SITESCAN_ERROR_CLEARED));

                // Add Event 112 - 127 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_112_127), Event_112_127.FATAL_ERROR), m_usEvent112_127, (UInt16)(Event_112_127.FATAL_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_112_127), Event_112_127.PERIODIC_READ), m_usEvent112_127, (UInt16)(Event_112_127.PERIODIC_READ));
                AddEventItem(Enum.GetName(typeof(Event_112_127), Event_112_127.SERVICE_LIMITING_ACTIVE_TIER_CHANGED), m_usEvent112_127, (UInt16)(Event_112_127.SERVICE_LIMITING_ACTIVE_TIER_CHANGED));

                // Add Event 128 - 143
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.TABLE_WRITTEN), m_usEvent128_143, (UInt16)(Event_128_143.TABLE_WRITTEN));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.BASE_MODE_ERROR), m_usEvent128_143, (UInt16)(Event_128_143.BASE_MODE_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.PENDING_RECONFIGURE), m_usEvent128_143, (UInt16)(Event_128_143.PENDING_RECONFIGURE));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.CTE_EVENT), m_usEvent128_143, (UInt16)(Event_128_143.CTE_EVENT));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.EVENT_TAMPER_CLEARED), m_usEvent128_143, (UInt16)(Event_128_143.EVENT_TAMPER_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_128_143), Event_128_143.NETWORK_TIME_UNAVAILABLE), m_usEvent128_143, (UInt16)(Event_128_143.NETWORK_TIME_UNAVAILABLE));


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

                // Add Event 208 - 223 bitfield itesm
                AddEventItem(Enum.GetName(typeof(Event_208_223), Event_208_223.WRONG_CONFIG_CRC), m_usEvent208_223, (UInt16)(Event_208_223.WRONG_CONFIG_CRC));
                AddEventItem(Enum.GetName(typeof(Event_208_223), Event_208_223.CHECK_CONFIG_CRC), m_usEvent208_223, (UInt16)(Event_208_223.CHECK_CONFIG_CRC));
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
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 11/11/09 RCG 2.30.06 144877 Created

    internal class OpenWayBasicPoly_DemandConfig : DemandConfig
    {
        #region Constants

        private const int DEMAND_CONFIG_BLOCK_LENGTH = 60;

        #endregion

        #region Public Methods

        /// <summary>Constructor</summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/11/09 RCG 2.30.06 144877 Created

        public OpenWayBasicPoly_DemandConfig(CPSEM psem, ushort Offset)
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
        // -------- --- ------- ------ -------------------------------------------
        // 11/11/09 RCG 2.30.06 144877 Created

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
                    "DemandDefinition2 = " + m_uiDemandDefinition2);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition3 = " + m_uiDemandDefinition3);
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
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/11/09 RCG 2.30.06 144877 Created

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
                DemandList.Add(m_uiDemandDefinition2);
                DemandList.Add(m_uiDemandDefinition3);

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
        // 11/11/09 RCG 2.30.06 144877 Created

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
            m_uiDemandDefinition2 = m_Reader.ReadUInt32();
            m_uiDemandDefinition3 = m_Reader.ReadUInt32();
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
    }
}
