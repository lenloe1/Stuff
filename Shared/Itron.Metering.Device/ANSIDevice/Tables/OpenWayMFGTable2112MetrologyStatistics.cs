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
//                         Copyright © 2011 - 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// An indication of what type of blurts are being returned.
    /// </summary>
    public enum BlurtMode : byte
    {
        /// <summary>
        /// Default Blurt
        /// </summary>
        [EnumDescription("Default Blurt")]
        Default = 0,
        /// <summary>
        /// Enhanced Blurt
        /// </summary>
        [EnumDescription("Enhanced Blurt")]
        Enhanced = 1,
        /// <summary>
        /// Illegal Value
        /// </summary>
        [EnumDescription("Illegal Value")]
        Illegal = 2,
        /// <summary>
        /// Advanced Blurt
        /// </summary>
        [EnumDescription("Advanced Blurt")]
        Advanced = 3,
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumDescription("Unknown")]
        Unknown = 4,
    }

    /// <summary>
    /// An indication of what type of energy is returned by the base.
    /// </summary>
    public enum BaseEnergies : byte
    {
        /// <summary>
        /// Wh
        /// </summary>
        [EnumDescription("Wh")]
        Wh = 0x00,
        /// <summary>
        /// VAh (using VA Arithmetic)
        /// </summary>
        [EnumDescription("VAh Arith")]
        VAhArithmetic = 0x10,
        /// <summary>
        /// VAh (using VA Vectorial)
        /// </summary>
        [EnumDescription("VAh Vec")]
        VAhVectorial = 0x11,
        /// <summary>
        /// VAh (using VA Lag)
        /// </summary>
        [EnumDescription("VAh Lag")]
        VAhLag = 0x13,
        /// <summary>
        /// Varh (using VA Arithmetic)
        /// </summary>
        [EnumDescription("VARh (using VA Arith)")]
        VarhArithmetic = 0x14,
        /// <summary>
        /// Varh (using VA Vectorial)
        /// </summary>
        [EnumDescription("VARh (using VA Vec)")]
        VarhVectorial = 0x15,
        /// <summary>
        /// Wh Delivered for Poly meters
        /// </summary>
        [EnumDescription("Wh d")]
        PolyWhDel = 0x30,
        /// <summary>
        /// Wh Received for Poly meters
        /// </summary>
        [EnumDescription("Wh r")]
        PolyWhRec = 0x70,
        /// <summary>
        /// VAh Arith Delivered for Poly meters
        /// </summary>
        [EnumDescription("VAh Arith d")]
        PolyVAhArithDel = 0x31,
        /// <summary>
        /// VAh Arith Received for Poly meters
        /// </summary>
        [EnumDescription("VAh Arith r")]
        PolyVAhArithRec = 0x71,
        /// <summary>
        /// VAh Vect Delivered for Poly meters
        /// </summary>
        [EnumDescription("VAh Vec d")]
        PolyVAhVectDel = 0x32,
        /// <summary>
        /// VAh Vect Received for Poly meters
        /// </summary>
        [EnumDescription("VAh Vec r")]
        PolyVAhVectRec = 0x72,
        /// <summary>
        /// varh Delivered for Poly meters
        /// </summary>
        [EnumDescription("VARh d")]
        PolyVarhDel = 0x34,
        /// <summary>
        /// varh Received for Poly meters
        /// </summary>
        [EnumDescription("VARh r")]
        PolyVarhRec = 0x74,
        /// <summary>
        /// VAh Lag for Poly meters
        /// </summary>
        [EnumDescription("VAh Lag")]
        PolyVAhLag = 0xB3,
        /// <summary>
        /// varh Q1 for Poly meters
        /// </summary>
        [EnumDescription("VARh Q1")]
        PolyVarhQ1 = 0xB5,
        /// <summary>
        /// varh Q2 for Poly meters
        /// </summary>
        [EnumDescription("VARh Q2")]
        PolyVarhQ4 = 0xB8,
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumDescription("Unknown")]
        Unknown = 255,
    }
    
    /// <summary>
    /// MFG Table 64 (2112) - Metrology (Blurt) Statistics Table
    /// </summary>
    public class OpenWayMFGTable2112MetrologyStatistics : AnsiTable
    {

        #region Constants

        private const uint VERSION_3_TABLE_SIZE = 85;
        private const uint VERSION_3_1_TABLE_SIZE = 399;
        private const uint VERSION_3_7_TABLE_SIZE = 428;
        private const uint POLY_3_7_TABLE_SIZE = 556;
        private const ushort ACK_RCVD_MASK = 0x0001;
        private const ushort MSG_RCVD_MASK = 0x0002;
        private const ushort WAIT_RESPONSE_MASK = 0x0004;
        private const ushort WAIT_RETRY_TO_MASK = 0x0008;
        private const ushort RETRY_TO_FLAG_MASK = 0x0010;
        private const ushort SKIP_NEXT_BLURT_MASK = 0x0020;
        private const ushort IS_BBV_MASK = 0x0040;
        private const ushort CLEAR_MET_BUSY_MASK = 0x0080;
        private const ushort BLURT_READY_MASK = 0x0100;
        private const ushort ID_COUNT_READY_MASK = 0x0200;
        private const ushort WDE_PULSE_COUNT_MASK = 0x0400;
        private const ushort END_NEXT_INT_MASK = 0x0800;
        private const ushort NEXT_INT_RCVC_MASK = 0x1000;
        private const ushort ENHANCED_METROLOGY_MODE_MASK = 0x8000;
        private const ushort METROLOGY_MODE_MASK = 0xE000;
        private const int METROLOGY_MODE_SHIFT = 13;
        private const byte ENERGIES_SUPPORTED_BY_BASE = 1;
        private const double INST_VALUES_RESOLUTION = 0.008286408;
        private const double RMS_CURRENT_RESOLUTION = 0.000323688;
        private const double RMS_VOLTAGE_RESOLUTION = 0.1;
        private const uint BLURT_STATUS_VA_TYPE_MASK = 0x200000;
        private const uint BLURT_STATUS_SSQ_TYPE_MASK = 0x2000000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="fltRegFWVersion">The register FW version for the current meter</param>
        /// <param name="blnIsPolyPhase">Determines whether the Meter being used is a PolyPhase or SinglePhase</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  02/12/13 MSC 2.70.68 312352 Table size for newer Firmware is dependant upon Poly vs Single Phase
        //
        public OpenWayMFGTable2112MetrologyStatistics(CPSEM psem, float fltRegFWVersion, bool blnIsPolyPhase)
            : base(psem, 2112, GetTableSize(fltRegFWVersion, blnIsPolyPhase))
        {
            m_fltRegFWVersion = fltRegFWVersion;
            m_MetData = new OpenWayMFGTable2112MetData();
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        /// <param name="fltRegFWVersion">The register FW version for the current meter</param>
        /// <param name="blnIsPolyPhase">Determines whether the Meter being used is a PolyPhase or not</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  02/12/13 MSC 2.70.68 312352 Table size for newer Firmware is dependant upon Poly vs Single Phase
        //
        public OpenWayMFGTable2112MetrologyStatistics(PSEMBinaryReader binaryReader, float fltRegFWVersion, bool blnIsPolyPhase)
            : base(2112, GetTableSize(fltRegFWVersion, blnIsPolyPhase))
        {
            m_fltRegFWVersion = fltRegFWVersion;
            m_MetData = new OpenWayMFGTable2112MetData();

            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2112.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Changes the table state to expired so that a full read will occur on 
        /// next access
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public void Refresh()
        {
            m_TableState = TableState.Expired;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the type of blurt packets the meter supports.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public BlurtMode MetrologyMode
        {
            get
            {
                ReadBlurtFlags();
                byte bytBlurtMode = (byte)((m_usBlurtFlags & METROLOGY_MODE_MASK) >> METROLOGY_MODE_SHIFT);
                BlurtMode MetrologyMode = BlurtMode.Unknown;

                if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_HYDROGEN_3_7) >= 0)
                {
                    if (true == Enum.IsDefined(typeof(BlurtMode), bytBlurtMode))
                    {
                        MetrologyMode = (BlurtMode)bytBlurtMode;
                    }
                }
                else //SR 3.0
                {
                    if (( m_usBlurtFlags & ENHANCED_METROLOGY_MODE_MASK) == ENHANCED_METROLOGY_MODE_MASK)
                    {
                        MetrologyMode = BlurtMode.Enhanced;
                    }
                    else //otherwise it must be default blurts
                    {
                        MetrologyMode = BlurtMode.Default;
                    }
                }

                return MetrologyMode;
            }
        }

        /// <summary>
        /// Gets if an ACK was received from blurt request.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool ACKReceived
        {
            get
            {
                ReadBlurtFlags();
                
                return ((m_usBlurtFlags & ACK_RCVD_MASK) == ACK_RCVD_MASK);
            }
        }

        /// <summary>
        /// Gets if a blurt message was received.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool MessageReceived
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & MSG_RCVD_MASK) == MSG_RCVD_MASK);
            }
        }

        /// <summary>
        /// Gets if the blurt message is a wait for save response.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool WaitResponse
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & WAIT_RESPONSE_MASK) == WAIT_RESPONSE_MASK);
            }
        }

        /// <summary>
        /// Gets if the blurt message indicates if it is waiting for a retry timeout.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool WaitRetryTimeOut
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & WAIT_RETRY_TO_MASK) == WAIT_RETRY_TO_MASK);
            }
        }

        /// <summary>
        /// Gets if the blurt message indicates if a retry timeout has occured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool RetryTimeOutFlag
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & RETRY_TO_FLAG_MASK) == RETRY_TO_FLAG_MASK);
            }
        }

        /// <summary>
        /// Gets if the next blurt message should be skipped.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool SkipNextBlurt
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & SKIP_NEXT_BLURT_MASK) == SKIP_NEXT_BLURT_MASK);
            }
        }

        /// <summary>
        /// Gets whether or not the meter uses base backed values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  12/05/11 RCG 2.53.13        Bit has changed to Is Based Back Value

        public bool IsBasedBackedValue
        {
            get
            {
                bool IsBBV = false;

                // This feature was added in Hydrogen so prior to that it should always be false.
                if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_HYDROGEN_3_7) >= 0)
                {
                    ReadBlurtFlags();
                    IsBBV = ((m_usBlurtFlags & IS_BBV_MASK) == IS_BBV_MASK);
                }

                return IsBBV;
            }
        }

        /// <summary>
        /// Gets if the blurt message indicates that clear met busy.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool ClearMetBusy
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & CLEAR_MET_BUSY_MASK) == CLEAR_MET_BUSY_MASK);
            }
        }

        /// <summary>
        /// Gets if the blurt message is ready.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool BlurtReady
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & BLURT_READY_MASK) == BLURT_READY_MASK);
            }
        }

        /// <summary>
        /// Gets if the blurt message's ID count is ready.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool IDCountReady
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & ID_COUNT_READY_MASK) == ID_COUNT_READY_MASK);
            }
        }

        /// <summary>
        /// Gets if the blurt message contains a WDE pulse count.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool WDEPulseCount
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & WDE_PULSE_COUNT_MASK) == WDE_PULSE_COUNT_MASK);
            }
        }

        /// <summary>
        /// Gets if the blurt message is the last in an interval.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool EndNextInterval
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & END_NEXT_INT_MASK) == END_NEXT_INT_MASK);
            }
        }

        /// <summary>
        /// Gets if the blurt message is recieving the next interval.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool NextIntervalReceived
        {
            get
            {
                ReadBlurtFlags();

                return ((m_usBlurtFlags & NEXT_INT_RCVC_MASK) == NEXT_INT_RCVC_MASK);
            }
        }

        /// <summary>
        /// Gets if the energy values configured are supported by the base.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public bool ConfiguredEnergiesSupported
        {
            get
            {
                ReadUnloadedTable();
                bool blnSupported = false;

                if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_HYDROGEN_3_7) >= 0)
                {
                    blnSupported = (ENERGIES_SUPPORTED_BY_BASE == m_bytEnergyConfigTest);
                }
                else //for SR 3.0 we don't know.  We'll assume true;
                {
                    blnSupported = true;
                }

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets if 1st type of energy supplied by the base.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  11/20/15 jrf 4.22.07 633860 Switched to default to secondary quantity to be consistent 
        //                              with CENTRON_AMI. Added retrieval of SR3.0(3.1.X) SSQ from 
        //                              blurt status flag.
        public BaseEnergies SuppliedEnergy1
        {
            get
            {
                ReadUnloadedTable();
                
                BaseEnergies EnergyType = BaseEnergies.Unknown;

                if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_HYDROGEN_3_7) >= 0)
                {
                    if (true == Enum.IsDefined(typeof(BaseEnergies), m_bytSuppliedEnergy1))
                    {
                        EnergyType = (BaseEnergies)m_bytSuppliedEnergy1;
                    }
                }
                else if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_3_1) > 0)
                {
                    //There should not be any released firmware that falls into this bucket 
                    //(greater than 3.1.xxx but less than 3.7.xxx), but if so mark energy as unknown.
                    EnergyType = BaseEnergies.Unknown;
                }
                else if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_3_1) == 0)
                {
                    //This should be VAh Arithmetic but we can confirm with blurt packet status bits
                    if ((m_BlurtStatus & BLURT_STATUS_SSQ_TYPE_MASK) == 0)
                    {
                        //VA is selected, so figure out what type
                        if ((m_BlurtStatus & BLURT_STATUS_VA_TYPE_MASK) == 0)
                        {
                            EnergyType = BaseEnergies.VAhArithmetic;
                        }
                        else
                        {
                            EnergyType = BaseEnergies.VAhVectorial;
                        }
                    }
                    else
                    {
                        //Var is selected. It is always vectorial.
                        EnergyType = BaseEnergies.VarhVectorial;
                    }

                }
                else //For less than SR3.0 this was always VAh arithmetic
                {
                    EnergyType = BaseEnergies.VAhArithmetic;
                }
                

                return EnergyType;
            }
        }

        /// <summary>
        /// Gets if 2nd type of energy supplied by the base.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  11/20/15 jrf 4.22.07 633860 Switched to default to primary quantity to be consistent 
        //                              with CENTRON_AMI. 
        public BaseEnergies SuppliedEnergy2
        {
            get
            {
                ReadUnloadedTable();

                BaseEnergies EnergyType = BaseEnergies.Unknown;

                if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_HYDROGEN_3_7) >= 0)
                {
                    if (true == Enum.IsDefined(typeof(BaseEnergies), m_bytSuppliedEnergy2))
                    {
                        EnergyType = (BaseEnergies)m_bytSuppliedEnergy2;
                    }
                }
                else //For earlier FW this is always Wh
                {
                    EnergyType = BaseEnergies.Wh;
                }

                return EnergyType;
            }
        }

        /// <summary>
        /// Gets the number of good blurt messages.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint GoodCount
        {
            get
            {
                ReadUnloadedTable();

                return m_uiGoodCount;
            }
        }

        /// <summary>
        /// Gets the number of save attempts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint SaveAttempts
        {
            get
            {
                ReadUnloadedTable();

                return m_uiSaveAttempts;
            }
        }

        /// <summary>
        /// Gets the number of ACKs received.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint ACKReceivedCount
        {
            get
            {
                ReadUnloadedTable();

                return m_uiACKRcvdCount;
            }
        }

        /// <summary>
        /// Gets the number of responses received.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint ResponseReceivedCount
        {
            get
            {
                ReadUnloadedTable();

                return m_uiResponseRcvdCount;
            }
        }

        /// <summary>
        /// Gets the number of failed saves.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint FailedSaves
        {
            get
            {
                ReadUnloadedTable();

                return m_uiFailedSaves;
            }
        }

        /// <summary>
        /// Gets the number of retries.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint RetryCount
        {
            get
            {
                ReadUnloadedTable();

                return m_uiRetryCount;
            }
        }

        /// <summary>
        /// Gets the number of no times.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint NoTimeCount
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNoTimeCount;
            }
        }

        /// <summary>
        /// Gets the number of kill retries.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint KillRetries
        {
            get
            {
                ReadUnloadedTable();

                return m_uiKillRetries;
            }
        }

        /// <summary>
        /// Gets the minimum time remaining.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint MinimumTimeRemaining
        {
            get
            {
                ReadUnloadedTable();

                return m_uiMinTimeRemain;
            }
        }

        /// <summary>
        /// Gets the number of false temperature readings.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint FalseTemperature
        {
            get
            {
                ReadUnloadedTable();

                return m_uiFalseTemperature;
            }
        }

        /// <summary>
        /// Gets the number of false energy readings.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint FalseEnergy
        {
            get
            {
                ReadUnloadedTable();

                return m_uiFalseEnergy;
            }
        }

        /// <summary>
        /// Gets the number of times the checksum failed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint ChecksumFailed
        {
            get
            {
                ReadUnloadedTable();

                return m_uiBlurtCheckSumFailed;
            }
        }

        /// <summary>
        /// Gets the number of times the header failed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint HeaderFailed
        {
            get
            {
                ReadUnloadedTable();

                return m_uiBlurtHeaderFailed;
            }
        }

        /// <summary>
        /// Gets the number of overruns were received.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint ReceivedOverruns
        {
            get
            {
                ReadUnloadedTable();

                return m_uiBlurtRecievedOverruns;
            }
        }

        /// <summary>
        /// Gets the number of Isr Rx messages that were received.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint IsrRxReceived
        {
            get
            {
                ReadUnloadedTable();

                return m_uiISRRxRcvs;
            }
        }

        /// <summary>
        /// Gets the number of Isr Rx messages that had errors.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint IsrRxErrorCount
        {
            get
            {
                ReadUnloadedTable();

                return m_uiISRRxErrorCount;
            }
        }

        /// <summary>
        /// Gets the number of Isr Type F errors.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint IsrErrorTypeF
        {
            get
            {
                ReadUnloadedTable();

                return m_uiISRErrTypeF;
            }
        }

        /// <summary>
        /// Gets the number of Isr Type O errors.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint IsrErrorTypeO
        {
            get
            {
                ReadUnloadedTable();

                return m_uiISRErrTypeO;
            }
        }

        /// <summary>
        /// Gets the number of parity errors.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint ParityErrors
        {
            get
            {
                ReadUnloadedTable();

                return m_uiParityErr;
            }
        }

        /// <summary>
        /// Gets the number of ABT errors.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public uint ABTErrors
        {
            get
            {
                ReadUnloadedTable();

                return m_uiABTErr;
            }
        }

        /// <summary>
        /// Gets the time of the last update.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public DateTime TimeOfLastUpdate
        {
            get
            {
                ReadUnloadedTable();

                return m_dtTimeOfLastUpdate;
            }
        }

        /// <summary>
        /// Gets the metrology data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Applying RESOLUTION constants in this method to
        //                              make it less confusing for others who attempt 
        //                              to use voltage, current or instantaneous metrology 
        //                              values.
        //  03/11/16 AF  4.50.236 651410 Switched to the base class' version of offset read which
        //                               supports retries.
        //
        public OpenWayMFGTable2112MetData MetrologyData
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;

                PSEMResult = base.Read(90, 82);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Metrology Data"));
                }
                else
                {
                    try
                    {
                        //Convert the bytes read to something useful.
                        if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_HYDROGEN_3_7) >= 0)
                        {
                            // Reverse the order of the header bytes
                            byte[] abyTemp = new byte[4];
                            abyTemp = m_Reader.ReadBytes(4);
                            Array.Reverse(abyTemp);
                            m_MetData.Header = BitConverter.ToUInt32(abyTemp, 0);

                            m_MetData.SecondarySavedQuantityDelivered = (ulong)m_Reader.ReadUInt48();
                            m_MetData.SecondarySavedQuantityReceived = (ulong)m_Reader.ReadUInt48();
                            m_MetData.SavedTotalSecondaryWattHoursDelivered = (ulong)m_Reader.ReadUInt48();
                            m_MetData.SavedTotalSecondaryWattHoursReceived = (ulong)m_Reader.ReadUInt48();

                            m_MetData.PhaseA_RMSVoltage = (double)(m_Reader.ReadUInt16()) * RMS_VOLTAGE_RESOLUTION;
                            m_MetData.PhaseB_RMSVoltage = m_Reader.ReadUInt16() * RMS_VOLTAGE_RESOLUTION;
                            m_MetData.PhaseC_RMSVoltage = m_Reader.ReadUInt16() * RMS_VOLTAGE_RESOLUTION;

                            m_MetData.PhaseA_RMSCurrent = m_Reader.ReadUInt32() * RMS_CURRENT_RESOLUTION;
                            m_MetData.PhaseB_RMSCurrent = m_Reader.ReadUInt32() * RMS_CURRENT_RESOLUTION;
                            m_MetData.PhaseC_RMSCurrent = m_Reader.ReadUInt32() * RMS_CURRENT_RESOLUTION;

                            m_MetData.InstantaneousVA = m_Reader.ReadInt32() * INST_VALUES_RESOLUTION;
                            m_MetData.InstantaneousWatts = m_Reader.ReadInt32() * INST_VALUES_RESOLUTION;
                            m_MetData.InstantaneousVAR = m_Reader.ReadInt32() * INST_VALUES_RESOLUTION;

                            m_MetData.PhaseB_Angle = m_Reader.ReadInt16();
                            m_MetData.PhaseC_Angle = m_Reader.ReadInt16();

                            m_MetData.PhaseA_WattHoursDelivered = m_Reader.ReadUInt16();
                            m_MetData.PhaseA_WattHoursReceived = m_Reader.ReadUInt16();

                            m_MetData.PhaseB_WattHoursDelivered = m_Reader.ReadUInt16();
                            m_MetData.PhaseB_WattHoursReceived = m_Reader.ReadUInt16();

                            m_MetData.PhaseC_WattHoursDelivered = m_Reader.ReadUInt16();
                            m_MetData.PhaseC_WattHoursReceived = m_Reader.ReadUInt16();

                            m_MetData.InternalTemperatureCentigrade = m_Reader.ReadUInt16();
                            // Reverse the order of the status bytes
                            abyTemp = m_Reader.ReadBytes(4);
                            Array.Reverse(abyTemp);
                            m_MetData.Status = BitConverter.ToUInt32(abyTemp, 0);
                        }
                    }
                    finally
                    {
                    }
                }

                return m_MetData;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="fltRegFWRevision">The register firmware version for the current meter.</param>
        /// <param name="blnIsPolyPhase">Determines whether the device is PolyPhase or not</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  02/12/13 MSC 2.70.68 312352 Table size for newer Firmware is dependant upon Poly vs Single Phase
        //
        private static uint GetTableSize(float fltRegFWRevision, bool blnIsPolyPhase)
        {
            uint TableSize = 0;

            if (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_3) == 0)
            {
                TableSize = VERSION_3_TABLE_SIZE;
            }
            else if (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_3_1) == 0)
            {
                TableSize = VERSION_3_1_TABLE_SIZE;
            }
            else if (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_HYDROGEN_3_7) >= 0 && VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_BORON_5_0) < 0)
            {
                TableSize = VERSION_3_7_TABLE_SIZE;
            }
            else if (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_BORON_5_0) >= 0)
            {
                if (blnIsPolyPhase)
                {
                    TableSize = POLY_3_7_TABLE_SIZE;
                }
                else
                {
                    TableSize = VERSION_3_7_TABLE_SIZE;
                }
            }

            return TableSize;
        }

        /// <summary>
        /// Reads the Blurt Flags bitfield from the meter
        /// </summary>
        /// <exception cref="PSEMException">Thrown if an error occurs while reading the table.</exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/09/11 RCG 2.53.15 N/A    Created

        protected void ReadBlurtFlags()
        {
            PSEMResponse Result = PSEMResponse.Ok;
            int Offset = 0;

            if (State != TableState.Loaded && m_BlurtFlagsRead == false)
            {
                if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_3) == 0)
                {
                    // In the 3.0 release this table had 3 bytes at the beginning that are no longer there
                    Offset = 3;
                }

                Result = Read(Offset, 2);

                if (Result == PSEMResponse.Ok)
                {
                    m_DataStream.Position = Offset;
                    m_usBlurtFlags = m_Reader.ReadUInt16();
                }
                else
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                        Result, "Error reading table " + m_TableID.ToString(CultureInfo.CurrentCulture));
                }
            }
        }

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  11/20/15 jrf 4.22.07 633860 Added reading blurt status flags for SR3.0(3.1.X) 
        private void ParseData()
        {
            if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_3) == 0)
            {
                m_usBlurtInitCount = m_Reader.ReadUInt16();
                m_bytMessageByteCount = m_Reader.ReadByte();
            }

            //flags
            m_usBlurtFlags = m_Reader.ReadUInt16();

            if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_HYDROGEN_3_7) >= 0)
            {
                //energy validation
                m_bytEnergyConfigTest = m_Reader.ReadByte();
                m_bytSuppliedEnergy1 = m_Reader.ReadByte();
                m_bytSuppliedEnergy2 = m_Reader.ReadByte();
            }

            //counters
            m_uiGoodCount = m_Reader.ReadUInt32();
            m_uiSaveAttempts = m_Reader.ReadUInt32();
            m_uiACKRcvdCount = m_Reader.ReadUInt32();
            m_uiResponseRcvdCount = m_Reader.ReadUInt32();
            m_uiFailedSaves = m_Reader.ReadUInt32();
            m_uiRetryCount = m_Reader.ReadUInt32();
            m_uiNoTimeCount = m_Reader.ReadUInt32();
            m_uiKillRetries = m_Reader.ReadUInt32();
            m_uiMinTimeRemain = m_Reader.ReadUInt32();
            if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_HYDROGEN_3_7) < 0)
            {
                m_uiTimesHit0 = m_Reader.ReadUInt32();
                m_uiSkipped = m_Reader.ReadUInt32();
            }
            else //Hydrogen or greater
            {
                m_uiFalseTemperature = m_Reader.ReadUInt32();
                m_uiFalseEnergy = m_Reader.ReadUInt32();
            }

            //error counters
            m_uiBlurtCheckSumFailed = m_Reader.ReadUInt32();
            m_uiBlurtHeaderFailed = m_Reader.ReadUInt32();
            m_uiBlurtRecievedOverruns = m_Reader.ReadUInt32();

            //uart errors
            m_uiISRRxRcvs = m_Reader.ReadUInt32();
            m_uiISRRxErrorCount = m_Reader.ReadUInt32();
            m_uiISRErrTypeF = m_Reader.ReadUInt32();
            m_uiISRErrTypeO = m_Reader.ReadUInt32();
            m_uiParityErr = m_Reader.ReadUInt32();
            m_uiABTErr = m_Reader.ReadUInt32();

            if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_3_1) >= 0)
            {
                m_dtTimeOfLastUpdate = m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME);
            }

            if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_3_1) == 0)
            {
                //Need to get to the status bytes of the blurt packet.  We know where status bytes are because 
                //we know this is a version 2 (enhanced) blurt packet since this is SR 3.0 (VERSION_3_1) firmware.
                //SR 3.0 reg. FW should only be requesting V2 blurts since the other versions didn't yet exist.
                m_Reader.ReadBytes(52);
                m_BlurtStatus = m_Reader.ReadUInt32();
            }
        }

        #endregion

        #region Member Variables

        private float m_fltRegFWVersion;
        private ushort m_usBlurtInitCount = 0;
        private byte m_bytMessageByteCount = 0;
        private ushort m_usBlurtFlags = 0;
        private byte m_bytEnergyConfigTest = 0;
        private byte m_bytSuppliedEnergy1 = 0;
        private byte m_bytSuppliedEnergy2 = 0;
        private uint m_uiGoodCount = 0;
        private uint m_uiSaveAttempts = 0;
        private uint m_uiACKRcvdCount = 0;
        private uint m_uiResponseRcvdCount = 0;
        private uint m_uiFailedSaves = 0;
        private uint m_uiRetryCount = 0;
        private uint m_uiNoTimeCount = 0;
        private uint m_uiKillRetries = 0;
        private uint m_uiMinTimeRemain = 0;
        private uint m_uiTimesHit0 = 0;
        private uint m_uiSkipped = 0;
        private uint m_uiFalseTemperature = 0;
        private uint m_uiFalseEnergy = 0;
        private uint m_uiBlurtCheckSumFailed = 0;
        private uint m_uiBlurtHeaderFailed = 0;
        private uint m_uiBlurtRecievedOverruns = 0;
        private uint m_uiISRRxRcvs = 0;
        private uint m_uiISRRxErrorCount = 0;
        private uint m_uiISRErrTypeF = 0;
        private uint m_uiISRErrTypeO = 0;
        private uint m_uiParityErr = 0;
        private uint m_uiABTErr = 0;
        private bool m_BlurtFlagsRead = false;
        private DateTime m_dtTimeOfLastUpdate = DateTime.MinValue;
        private OpenWayMFGTable2112MetData m_MetData;
        private uint m_BlurtStatus = 0;

        #endregion

    }

    /// <summary>
    /// Class to encapsulate the metrology data present in Mfg table 64
    /// </summary>
    public class OpenWayMFGTable2112MetData
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public OpenWayMFGTable2112MetData()
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets the header for the metrology data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public uint Header
        {
            get
            {
                return m_uiMetDataHeader;
            }
            set
            {
                m_uiMetDataHeader = value;
            }
        }

        /// <summary>
        /// Gets/sets the secondary saved quantity delivered - either Var or VA
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ulong SecondarySavedQuantityDelivered
        {
            get
            {
                return m_ulSecondarySavedQtyDel;
            }
            set
            {
                m_ulSecondarySavedQtyDel = value;
            }
        }

        /// <summary>
        /// Gets/sets the secondary saved quantity received - either VA or Var
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ulong SecondarySavedQuantityReceived
        {
            get
            {
                return m_ulSecondarySavedQtyRec;
            }
            set
            {
                m_ulSecondarySavedQtyRec = value;
            }
        }

        /// <summary>
        /// Gets/sets the saved total secondary watt hours delivered - Wh d
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ulong SavedTotalSecondaryWattHoursDelivered
        {
            get
            {
                return m_ulSavedTotalSecondaryWattHourDel;
            }
            set
            {
                m_ulSavedTotalSecondaryWattHourDel = value;
            }
        }

        /// <summary>
        /// Gets/sets the saved total secondary Watt hours received - Wh r
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ulong SavedTotalSecondaryWattHoursReceived
        {
            get
            {
                return m_ulSavedTotalSecondaryWattHourRec;
            }
            set
            {
                m_ulSavedTotalSecondaryWattHourRec = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase A rms voltage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        //
        public double PhaseA_RMSVoltage
        {
            get
            {
                return m_dblPhaseARMSVoltage;
            }
            set
            {
                m_dblPhaseARMSVoltage = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase B rms voltage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        public double PhaseB_RMSVoltage
        {
            get
            {
                return m_dblPhaseBRMSVoltage;
            }
            set
            {
                m_dblPhaseBRMSVoltage = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase C rms voltage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        public double PhaseC_RMSVoltage
        {
            get
            {
                return m_dblPhaseCRMSVoltage;
            }
            set
            {
                m_dblPhaseCRMSVoltage = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase A rms current
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        public double PhaseA_RMSCurrent
        {
            get
            {
                return m_dblPhaseARMSCurrent;
            }
            set
            {
                m_dblPhaseARMSCurrent = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase B rms current
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        public double PhaseB_RMSCurrent
        {
            get
            {
                return m_dblPhaseBRMSCurrent;
            }
            set
            {
                m_dblPhaseBRMSCurrent = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase C rms current
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        public double PhaseC_RMSCurrent
        {
            get
            {
                return m_dblPhaseCRMSCurrent;
            }
            set
            {
                m_dblPhaseCRMSCurrent = value;
            }
        }

        /// <summary>
        /// Gets/sets the instantaneous VA
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        public double InstantaneousVA
        {
            get
            {
                return m_dblInstantaneousVA;
            }
            set
            {
                m_dblInstantaneousVA = value;
            }
        }

        /// <summary>
        /// Gets/sets the instantaneous watts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        public double InstantaneousWatts
        {
            get
            {
                return m_dblInstantaneousWatts;
            }
            set
            {
                m_dblInstantaneousWatts = value;
            }
        }

        /// <summary>
        /// Gets/sets the instantaneous Vars
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //  06/20/12 jrf 2.60.35 200157 Converting to a double value.
        //
        public double InstantaneousVAR
        {
            get
            {
                return m_dblInstantaneousVAR;
            }
            set
            {
                m_dblInstantaneousVAR = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase B angle
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public short PhaseB_Angle
        {
            get
            {
                return m_sPhaseBAngle;
            }
            set
            {
                m_sPhaseBAngle = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase C angle
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public short PhaseC_Angle
        {
            get
            {
                return m_sPhaseCAngle;
            }
            set
            {
                m_sPhaseCAngle = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase A Watt hours delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ushort PhaseA_WattHoursDelivered
        {
            get
            {
                return m_usPhaseAWhDel;
            }
            set
            {
                m_usPhaseAWhDel = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase A watt hours received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ushort PhaseA_WattHoursReceived
        {
            get
            {
                return m_usPhaseAWhRec;
            }
            set
            {
                m_usPhaseAWhRec = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase B watt hours delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ushort PhaseB_WattHoursDelivered
        {
            get
            {
                return m_usPhaseBWhDel;
            }
            set
            {
                m_usPhaseBWhDel = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase B watt hours received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ushort PhaseB_WattHoursReceived
        {
            get
            {
                return m_usPhaseBWhRec;
            }
            set
            {
                m_usPhaseBWhRec = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase C watt hours delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ushort PhaseC_WattHoursDelivered
        {
            get
            {
                return m_usPhaseCWhDel;
            }
            set
            {
                m_usPhaseCWhDel = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase C watt hours received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ushort PhaseC_WattHoursReceived
        {
            get
            {
                return m_usPhaseCWhRec;
            }
            set
            {
                m_usPhaseCWhRec = value;
            }
        }

        /// <summary>
        /// Gets/sets the internal temperature in Centigrade
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public ushort InternalTemperatureCentigrade
        {
            get
            {
                return m_usInternalTempCentigrade;
            }
            set
            {
                m_usInternalTempCentigrade = value;
            }
        }

        /// <summary>
        /// Gets/sets the status field.  It is up to the calling method to parse out the
        /// statuses
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public uint Status
        {
            get
            {
                return m_uiStatus;
            }
            set
            {
                m_uiStatus = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_uiMetDataHeader = 0;
        private ulong m_ulSecondarySavedQtyDel = 0;
        private ulong m_ulSecondarySavedQtyRec = 0;
        private ulong m_ulSavedTotalSecondaryWattHourDel = 0;
        private ulong m_ulSavedTotalSecondaryWattHourRec = 0;
        private double m_dblPhaseARMSVoltage = 0;
        private double m_dblPhaseBRMSVoltage = 0;
        private double m_dblPhaseCRMSVoltage = 0;
        private double m_dblPhaseARMSCurrent = 0;
        private double m_dblPhaseBRMSCurrent = 0;
        private double m_dblPhaseCRMSCurrent = 0;
        private double m_dblInstantaneousVA = 0;
        private double m_dblInstantaneousWatts = 0;
        private double m_dblInstantaneousVAR = 0;
        private short m_sPhaseBAngle = 0;
        private short m_sPhaseCAngle = 0;
        private ushort m_usPhaseAWhDel = 0;
        private ushort m_usPhaseAWhRec = 0;
        private ushort m_usPhaseBWhDel = 0;
        private ushort m_usPhaseBWhRec = 0;
        private ushort m_usPhaseCWhDel = 0;
        private ushort m_usPhaseCWhRec = 0;
        private ushort m_usInternalTempCentigrade = 0;
        private uint m_uiStatus = 0;

        #endregion
    }
}
