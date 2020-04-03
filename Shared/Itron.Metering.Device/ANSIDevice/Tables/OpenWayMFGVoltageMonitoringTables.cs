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
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Resources;
using System.Text;
using System.Threading;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Used to enumerate the monitored phases of the meter.
    /// </summary>
    public enum MonitorPhases : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumDescription("Unknown")]
        Unknown = 0,
        /// <summary>
        /// Phase A
        /// </summary>
        [EnumDescription("Phase A")]
        A = 1,
        /// <summary>
        /// Phases A and C
        /// </summary>
        [EnumDescription("Phases A & C")]
        AC = 2,
        /// <summary>
        /// Phases A, B and C
        /// </summary>
        [EnumDescription("Phases A, B & C")]
        ABC = 3,
    }

    /// <summary>
    /// Used to enumerate service types supported by the polyphase meter.
    /// </summary>
    public enum MeterServiceType : byte
    {
        /// <summary>
        /// WYE_3ELEMENT_3PHASE_4WIRE
        /// </summary>
        [EnumDescription("WYE 3 Element 3 Phase 4-wire")]
        ThreeElFourWY = 0,
        /// <summary>
        /// WYE_2_5ELEMENT_3PHASE_4WIRE
        /// </summary>
        [EnumDescription("WYE 2.5 Element 3 Phase 4-wire")]
        TwoHalfFourWY = 1,
        /// <summary>
        /// NETWORK_2ELEMENT
        /// </summary>
        [EnumDescription("Network 2 Element")]
        TwoElNet = 2,
        /// <summary>
        /// DELTA_3ELEMENT_3PHASE_4WIRE
        /// </summary>
        [EnumDescription("Delta 3 Element 3 Phase 4-wire")]
        ThreeElFourWD = 3,
        /// <summary>
        /// WYE_2ELEMENT_3PHASE_4WIRE
        /// </summary>
        [EnumDescription("WYE 2 Element 3 Phase 4-wire")]
        TwoElFourWY = 4,
        /// <summary>
        /// DELTA_2ELEMENT_3PHASE_3WIRE
        /// </summary>
        [EnumDescription("Delta 2 Element 3 Phase 3-wire")]
        TwoElThreeWD = 5,
        /// <summary>
        /// DELTA_2ELEMENT_3PHASE_4WIRE
        /// </summary>
        [EnumDescription("Delta 2 Element 3 Phase 4-wire")]
        TwoElFourWD = 6,
        /// <summary>
        /// SINGLEPHASE_2ELEMENT
        /// </summary>
        [EnumDescription("Single Phase 2 Element")]
        TwoElSingle = 7,
        /// <summary>
        /// Single Phase 3-wire - 2S/4S
        /// </summary>
        [EnumDescription("Single Phase 3-wire - 2S/4S")]
        OneElThreeWire = 8,
        /// <summary>
        /// Single Phase 2-wire - 3S
        /// </summary>
        [EnumDescription("Single Phase 2-wire - 3S")]
        OneElTwoWire = 9,
        /// <summary>
        /// 9S doing 46S metering
        /// </summary>
        [EnumDescription("9S Doing 46S Metering")]
        TwoHalfFourWY_9S = 10,
        /// <summary>
        /// Invalid Service type
        /// </summary>
        [EnumDescription("Invalid Service Type")]
        NoValidService = 255,
    }

    /// <summary>
    /// Used to enumerate meter forms supported by the polyphase meter.
    /// </summary>
    public enum MeterForm : byte
    {
        /// <summary>
        /// 1S
        /// </summary>
        [EnumDescription("1S")]
        Form1S = 1,
        /// <summary>
        /// 2S
        /// </summary>
        [EnumDescription("2S")]
        Form2S = 2,
        /// <summary>
        /// 3S
        /// </summary>
        [EnumDescription("3S")]
        Form3S = 3,
        /// <summary>
        /// 4S
        /// </summary>
        [EnumDescription("4S")]
        Form4S = 4,
        /// <summary>
        /// 5S
        /// </summary>
        [EnumDescription("5S")]
        Form5S = 5,
        /// <summary>
        /// 9S
        /// </summary>
        [EnumDescription("9S")]
        Form9S = 9,
        /// <summary>
        /// 10S
        /// </summary>
        [EnumDescription("10S")]
        Form10S = 10,
        /// <summary>
        /// 12S
        /// </summary>
        [EnumDescription("12S")]
        Form12S = 12,
        /// <summary>
        /// 13S
        /// </summary>
        [EnumDescription("13S")]
        Form13S = 13,
        /// <summary>
        /// 16S
        /// </summary>
        [EnumDescription("16S")]
        Form16S = 16,
        /// <summary>
        /// 22 Weco COSEM
        /// </summary>
        [EnumDescription("22 Weco COSEM")]
        Form22 = 22,
        /// <summary>
        /// 25S
        /// </summary>
        [EnumDescription("25S")]
        Form25S = 25,
        /// <summary>
        /// 26S
        /// </summary>
        [EnumDescription("26S")]
        Form26S = 26,
        /// <summary>
        /// 29S
        /// </summary>
        [EnumDescription("29S")]
        Form29S = 29,
        /// <summary>
        /// 36S
        /// </summary>
        [EnumDescription("36S")]
        Form36S = 36,
        /// <summary>
        /// 45S
        /// </summary>
        [EnumDescription("45S")]
        Form45S = 45,
        /// <summary>
        /// 46S
        /// </summary>
        [EnumDescription("46S")]
        Form46S = 46,
        /// <summary>
        /// 48S
        /// </summary>
        [EnumDescription("48S")]
        Form48S = 48,
        /// <summary>
        /// 56S
        /// </summary>
        [EnumDescription("56S")]
        Form56S = 56,
        /// <summary>
        /// 66S
        /// </summary>
        [EnumDescription("66S")]
        Form66S = 66,
        /// <summary>
        /// 96S
        /// </summary>
        [EnumDescription("96S")]
        Form96S = 96,
        /// <summary>
        /// 1J
        /// </summary>
        [EnumDescription("1J")]
        Form1J = 100, // 100 is arbitrary
        /// <summary>
        /// Non-12S
        /// </summary>
        [EnumDescription("Non-12S")]
        NON_FM12S = 255,
    }



    /// <summary>
    /// MFG Table 2153 (Itron 105)
    /// Enhanced Voltage Monitoring Actual Dimension Table.
    /// </summary>
    public class OpenWayMFGTable2153 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 8;
        private const int TABLE_TIMEOUT = 1000;
        private const ushort INTERVAL_LENGTH_OFFSET = 6;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public OpenWayMFGTable2153(CPSEM psem)
            : base(psem, 2153, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public OpenWayMFGTable2153(PSEMBinaryReader reader)
            : base(2153, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2153.Read");

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
        /// Maximum number of intervals per block which can be contained in the data table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 NumberBlockIntervals
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNumberOfIntervals;
            }
        }


        /// <summary>
        /// Maximum time in minutes for Voltage Monitoring interval duration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public byte IntervalLength
        {
            get
            {
                ReadUnloadedTable();

                return m_byIntervalLength;
            }
            set
            {
                m_byIntervalLength = value;

                m_DataStream.Position = INTERVAL_LENGTH_OFFSET;

                m_Writer.Write(value);

                base.Write(INTERVAL_LENGTH_OFFSET, 1);
            }
        }

        /// <summary>
        /// The number of seconds to confirm that the instantaneous voltage is 
        /// above/below the threshold before a corresponding alarm is reported. 
        /// The value 0 means immediate reporting.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public byte VRMSAlarmMinSeconds
        {
            get
            {
                ReadUnloadedTable();

                return m_byVRMSAlarmMinSeconds;
            }
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        private void ParseData()
        {
            m_uiMemorySize = m_Reader.ReadUInt32();
            m_uiNumberOfIntervals = m_Reader.ReadUInt16();
            m_byIntervalLength = m_Reader.ReadByte();
            m_byVRMSAlarmMinSeconds = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        UInt32 m_uiMemorySize = 0;
        UInt16 m_uiNumberOfIntervals = 0;
        byte m_byIntervalLength = 0;
        byte m_byVRMSAlarmMinSeconds = 0;

        #endregion
    }
    
    /// <summary>
    /// MFG Table 2154 (Itron 106)
    /// Enhanced Voltage Monitoring Control Table.
    /// </summary>
    public class OpenWayMFGTable2154 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 9;
        private const int TABLE_TIMEOUT = 1000;
        private const byte VM_ENABLED = 0x01;
        private const byte SET_VM = 0x01;
        private const byte CLEAR_VM = 0xFE;
        private const ushort ENABLE_VM_OFFSET = 0; 

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public OpenWayMFGTable2154(CPSEM psem)
            : base(psem, 2154, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public OpenWayMFGTable2154(PSEMBinaryReader reader)
            : base(2154, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2154.Read");

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
        /// Voltage Monitoring is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public bool VoltageMonitoringEnabled
        {
            get
            {
                ReadUnloadedTable();

                return ((VM_ENABLED & m_byEnableVM) == VM_ENABLED);
            }

            set
            {
                if (true == value)
                {
                    m_byEnableVM = (byte)(SET_VM | m_byEnableVM);
                }
                else
                {
                    m_byEnableVM = (byte)(CLEAR_VM & m_byEnableVM);
                }

                m_DataStream.Position = ENABLE_VM_OFFSET;

                m_Writer.Write(m_byEnableVM);

                base.Write(ENABLE_VM_OFFSET, 1);
            }
        }


        /// <summary>
        /// An integer value representing the low threshold percentage related to the 
        /// nominal Volt hour in an interval. An event is triggered if the volt hour value 
        /// for Voltage Monitoring interval duration is below this threshold x nominal Vh.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 VhLowThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_uiVhLowThreshold;
            }
        }

        /// <summary>
        /// An integer value representing the high threshold percentage related to the 
        /// nominal Volt hour in an interval. An event is triggered if the volt hour value 
        /// for Voltage Monitoring interval duration is above this threshold x nominal Vh.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 VhHighThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_uiVhHighThreshold;
            }
        }

        /// <summary>
        /// An integer value [0, 100] representing the low threshold percentage related 
        /// to the nominal voltage. An event is triggered if minimum instantaneous 
        /// RMS voltage during Voltage Monitoring interval duration is below this 
        /// threshold x nominal voltage for a configurable number of seconds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 RMSVoltLowThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_uiRMSVoltLowThreshold;
            }
        }

        /// <summary>
        /// An integer value [0, 100] representing the high threshold percentage related 
        /// to the nominal voltage. An event is triggered if minimum instantaneous 
        /// RMS voltage during Voltage Monitoring interval duration is above this 
        /// threshold x nominal voltage for a configurable number of seconds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 RMSVoltHighThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_uiRMSVoltHighThreshold;
            }
        }
        

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        private void ParseData()
        {
            m_byEnableVM = m_Reader.ReadByte();
            m_uiVhLowThreshold = m_Reader.ReadUInt16();
            m_uiVhHighThreshold = m_Reader.ReadUInt16();
            m_uiRMSVoltLowThreshold = m_Reader.ReadUInt16();
            m_uiRMSVoltHighThreshold = m_Reader.ReadUInt16();
        }

        #endregion

        #region Members

        byte m_byEnableVM = 0;
        UInt16 m_uiVhLowThreshold = 0;
        UInt16 m_uiVhHighThreshold = 0;
        UInt16 m_uiRMSVoltLowThreshold = 0;
        UInt16 m_uiRMSVoltHighThreshold = 0;

        #endregion
    }
    
    /// <summary>
    /// MFG Table 2155 (Itron 107)
    /// Enhanced Voltage Monitoring Status Table.
    /// </summary>
    public class OpenWayMFGTable2155 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 19;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public OpenWayMFGTable2155(CPSEM psem)
            : base(psem, 2155, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public OpenWayMFGTable2155(PSEMBinaryReader reader)
            : base(2155, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2155.Read");

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
        /// The scalar applied to all phases for Vh data item intervals and end readings.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 Scalar
        {
            get
            {
                ReadUnloadedTable();
                
                return m_uiScalar;
            }
        }

        /// <summary>
        /// The divisor applied to all phases for Vh data item intervals and end readings.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 Divisor
        {
            get
            {
                ReadUnloadedTable();

                return m_uiDivisor;
            }
        }

        /// <summary>
        /// The maximum number of blocks that can be contained in the data table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 NumberOfBlocks
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNumberOfBlocks;
            }
        }

        /// <summary>
        /// The number of valid voltage monitoring data blocks in the data table.
        /// The range is zero (meaning no blocks in the VM data table) to the actual 
        /// dimension of the number of VM data blocks.  The block is considered valid 
        /// when at least one interval is written.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 NumberValidBlocks
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNumberValidBlocks;
            }
        }

        /// <summary>
        /// Array element of the newest valid data block in the Voltage Monitoring data array. 
        /// This field is valid only if the number of valid blocks is greater than zero.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 LastBlock
        {
            get
            {
                ReadUnloadedTable();

                return m_uiLastBlock;
            }
        }

        /// <summary>
        /// Sequence number of the last block. It continues to increment even if it 
        /// exceeds the maximum number of blocks available in storage memory.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt32 LastBlockSequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return m_uiLastBlockSequenceNumber;
            }
        }

        /// <summary>
        /// Number of valid intervals stored in the last Voltage Monitoring block array. 
        /// The range is zero (meaning no interval in the array) to the actual dimension 
        /// of the number of intervals per block.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16 NumberValidIntervals
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNumberValidIntervals;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        private void ParseData()
        {
            m_bySetStatusFlags = m_Reader.ReadByte();
            m_uiScalar = m_Reader.ReadUInt16();
            m_uiDivisor = m_Reader.ReadUInt16();
            m_uiNumberOfBlocks = m_Reader.ReadUInt16();
            m_uiNumberValidBlocks = m_Reader.ReadUInt16();
            m_uiLastBlock = m_Reader.ReadUInt16();
            m_uiLastBlockSequenceNumber = m_Reader.ReadUInt32();
            m_uiNumberUnreadBlocks = m_Reader.ReadUInt16();
            m_uiNumberValidIntervals = m_Reader.ReadUInt16();
        }

        #endregion

        #region Members

        byte m_bySetStatusFlags = 0;
        UInt16 m_uiScalar = 0;
        UInt16 m_uiDivisor = 0;
        UInt16 m_uiNumberOfBlocks = 0;
        UInt16 m_uiNumberValidBlocks = 0;
        UInt16 m_uiLastBlock = 0;
        UInt32 m_uiLastBlockSequenceNumber = 0;
        UInt16 m_uiNumberUnreadBlocks = 0;
        UInt16 m_uiNumberValidIntervals = 0;

        #endregion
    }


    /// <summary>
    /// MFG Table 2156 (Itron 108)
    /// Enhanced Voltage Monitoring Extended Status Table.
    /// </summary>
    public class OpenWayMFGTable2156 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 10;
        private const int TABLE_TIMEOUT = 1000;
        private const int NUM_NOMINAL_VOLTAGES = 3;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/11 jrf 2.53.10 TC5306 Created
        //
        public OpenWayMFGTable2156(CPSEM psem)
            : base(psem, 2156, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file.
        /// </summary>
        /// <param name="reader">The PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/11 jrf 2.53.10 TC5306 Created
        //
        public OpenWayMFGTable2156(PSEMBinaryReader reader)
            : base(2156, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/11 jrf 2.53.10 TC5306 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2156.Read");

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
        /// The nominal voltage for each phase.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/11 jrf 2.53.10 TC5306 Created
        //
        public ushort[] NominalVoltage
        {
            get
            {
                ReadUnloadedTable();

                return (ushort[])m_auiNominalVoltages.Clone();
            }
        }

        /// <summary>
        /// The phases that are being monitored by the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/11 jrf 2.53.10 TC5306 Created
        //
        public MonitorPhases PhasesMonitored
        {
            get
            {
                ReadUnloadedTable();
                MonitorPhases Phases = MonitorPhases.Unknown;

                if (Enum.IsDefined(typeof(MonitorPhases), m_byMonitoringPhases))
                {
                    Phases = (MonitorPhases)m_byMonitoringPhases;
                }

                return Phases;
            }
        }

        /// <summary>
        /// The number of phases that are being monitored by the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5306 Created
        //
        public byte NumberOfPhases
        {
            get
            {
                ReadUnloadedTable();
                byte byNumPhases = 0;

                switch (PhasesMonitored)
                {
                    case MonitorPhases.A:
                        {
                            byNumPhases = 1;
                            break;
                        }
                    case MonitorPhases.AC:
                        {
                            byNumPhases = 2;
                            break;
                        }
                    case MonitorPhases.ABC:
                        {
                            byNumPhases = 3;
                            break;
                        }
                    default:
                        {
                            byNumPhases = 0;
                            break;
                        }
                }

                return byNumPhases;
            }
        }

        /// <summary>
        /// The form of the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/11 jrf 2.53.10 TC5306 Created
        //
        public MeterForm Form
        {
            get
            {
                ReadUnloadedTable();
                MeterForm Form = MeterForm.NON_FM12S;

                if (Enum.IsDefined(typeof(MeterForm), m_byMeterForm))
                {
                    Form = (MeterForm)m_byMeterForm;
                }

                return Form;
            }
        }


        /// <summary>
        /// The service type of the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/11 jrf 2.53.10 TC5306 Created
        //
        public MeterServiceType ServiceType
        {
            get
            {
                ReadUnloadedTable();
                MeterServiceType ServiceType = MeterServiceType.NoValidService;

                if (Enum.IsDefined(typeof(MeterServiceType), m_byServiceType))
                {
                    ServiceType = (MeterServiceType)m_byServiceType;
                }

                return ServiceType;
            }
        }

        /// <summary>
        /// The number of phases change action needed flag. This field is valid for mono meters only. 
        /// It is always 0 for poly meters.  True indicates action is requested to change the number of 
        /// phases in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5310 Created
        //
        public bool NumberPhaseChangeActionNeeded
        {
            get
            {
                ReadUnloadedTable();
                bool blnActionNeeded = false;

                if (0 != m_byPhaseChangeActionNeeded)
                {
                    blnActionNeeded = true;
                }

                return blnActionNeeded;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/11 jrf 2.53.10 TC5306 Created
        //
        private void ParseData()
        {
            for (int i = 0; i < NUM_NOMINAL_VOLTAGES; i++)
            {
                m_auiNominalVoltages[i] = m_Reader.ReadUInt16();
            }

            m_byMonitoringPhases = m_Reader.ReadByte();
            m_byMeterForm = m_Reader.ReadByte();
            m_byServiceType = m_Reader.ReadByte();
            m_byPhaseChangeActionNeeded = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        UInt16[] m_auiNominalVoltages = new UInt16[NUM_NOMINAL_VOLTAGES];
        byte m_byMonitoringPhases = 0;
        byte m_byMeterForm = 0;
        byte m_byServiceType = (byte)MeterServiceType.NoValidService;  //Not defaulting to zero since it is a valid service type.
        byte m_byPhaseChangeActionNeeded = 0;

        #endregion
    }

    /// <summary>
    /// MFG Table 2157 (Itron 109)
    /// Enhanced Voltage Monitoring Data Table.
    /// </summary>
    public class OpenWayMFGTable2157 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        /// <param name="uiSTIMESize">the size of an STIME type depending on the time format specified
        /// in the meter from Table 0.</param>
        /// <param name="iTimeFormat">the Time Format for this meter from Table 0.</param>
        /// <param name="uiNumberBlocks">The maximum number of valid blocks possible from Table 2155.</param>
        /// <param name="byPhasesMonitored">The number of phases being monitored in the meter from Table 2156.</param>
        /// <param name="uiNumberBlockIntervals">The maximum number of intervals per block from Table 2153.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public OpenWayMFGTable2157(CPSEM psem, uint uiSTIMESize, int iTimeFormat, UInt16 uiNumberBlocks, byte byPhasesMonitored, UInt16 uiNumberBlockIntervals)
            : base(psem, 2157, DetermineTableSize(uiSTIMESize, uiNumberBlocks, byPhasesMonitored, uiNumberBlockIntervals), TABLE_TIMEOUT)
        {
            m_uiSTIMESize = uiSTIMESize;
            m_iTimeFormat = iTimeFormat;
            m_uiNumberBlocks = uiNumberBlocks;
            m_byPhasesMonitored = byPhasesMonitored;
            m_uiNumberBlockIntervals = uiNumberBlockIntervals;

        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public override PSEMResponse Read()
        {
            throw new NotSupportedException("This table does not support full reads.");
        }

        /// <summary>
        /// Reads a single block of voltage monitoring data from the meter.
        /// </summary>
        /// <param name="uiBlockToRead">The index of the block that should be read.</param>
        /// <param name="uiValidIntervals">The number of valid intervals in the block.</param>
        /// <param name="BlockData">The block read from the meter.</param>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public virtual PSEMResponse ReadBlock(ushort uiBlockToRead, ushort uiValidIntervals, out VMBlockDataRecord BlockData)
        {
            VMBlockDataRecord ReadBlock = null;
            PSEMResponse Response = PSEMResponse.Err;
            int iBlockOffset = 0;
            ushort usBlockLength = 0;


            if (uiBlockToRead < m_uiNumberBlocks)
            {
                // Determine the data that needs to be read.
                usBlockLength = (ushort)VMBlockDataRecord.Size(m_uiSTIMESize, m_byPhasesMonitored, m_uiNumberBlockIntervals);
                iBlockOffset = uiBlockToRead * usBlockLength;

                Response = Read(iBlockOffset, usBlockLength);

                if (Response == PSEMResponse.Ok)
                {
                    ReadBlock = new VMBlockDataRecord(m_byPhasesMonitored, m_uiNumberBlockIntervals, uiValidIntervals, m_iTimeFormat); 
                    ReadBlock.Parse(m_Reader);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("uiBlockToRead", "Invalid block requested.");
            }


            BlockData = ReadBlock;
            return Response;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method determines the size of the table.
        /// </summary>
        /// <param name="uiSTIMESize">the size of an STIME type depending on the time format specified
        /// in the meter from Table 0.</param>
        /// <param name="uiNumberBlocks">The maximum number of blocks from Table 2155.</param>
        /// <param name="byPhasesMonitored">The number of phases being monitored in the meter from Table 2156.</param>
        /// <param name="uiNumberBlockIntervals">The maximum number of intervals per block from Table 2153.</param>
        /// <returns>The size of the table.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        private static uint DetermineTableSize(uint uiSTIMESize, UInt16 uiNumberBlocks, byte byPhasesMonitored, UInt16 uiNumberBlockIntervals)
        {
            uint uiTableSize = 0;

            uiTableSize += uiNumberBlocks * VMBlockDataRecord.Size(uiSTIMESize, byPhasesMonitored, uiNumberBlockIntervals);

            return uiTableSize;
        }

        #endregion

        #region Members

        private UInt16 m_uiNumberBlocks = 0;
        private uint m_uiSTIMESize = 0;
        private int m_iTimeFormat = 0;
        private byte m_byPhasesMonitored = 0;
        private UInt16 m_uiNumberBlockIntervals = 0;

        #endregion

        
    }


    /// <summary>
    /// The voltage monitoring enhanced data block object.
    /// </summary>
    public class VMBlockDataRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byPhasesMonitored">The number of phases being monitored in the meter from Table 2156.</param>
        /// <param name="uiMaxIntervalsPerBlock">The maximum number of intervals possible in the block.</param>
        /// <param name="uiValidIntervalsPerBlock">The number of valid intervals in the block.</param>
        /// <param name="iTimeFormat">the Time Format for this meter from Table 0.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public VMBlockDataRecord(byte byPhasesMonitored, UInt16 uiMaxIntervalsPerBlock, UInt16 uiValidIntervalsPerBlock,  int iTimeFormat)
        {
            m_byPhasesMonitored = byPhasesMonitored;
            m_uiValidIntervalsPerBlock = uiValidIntervalsPerBlock;
            m_uiMaxIntervalsPerBlock = uiMaxIntervalsPerBlock;
            m_iTimeFormat = iTimeFormat;
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data to parse.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public void Parse(PSEMBinaryReader Reader)
        {
            
            // Read the block end time.
            m_dtBlockEndTime = Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat);

            // Read the end readings
            m_uiEndReadings = new UInt32[m_byPhasesMonitored];
                       
            for (int iPhase = 0; iPhase < m_byPhasesMonitored; iPhase++)
            {
                m_uiEndReadings[iPhase] = Reader.ReadUInt32();
            }

            // Read the simple interval status
            m_abySimpleIntervalStatus = Reader.ReadBytes((m_uiMaxIntervalsPerBlock + 7) / 8);

            // Read intervals
            m_Intervals = new VMIntervalDataRecord[m_uiValidIntervalsPerBlock];

            for (int iInterval = 0; iInterval < m_uiValidIntervalsPerBlock; iInterval++)
            {
                VMIntervalDataRecord Interval = new VMIntervalDataRecord(m_byPhasesMonitored);
                Interval.Parse(Reader);

                m_Intervals[iInterval] = Interval;
            }
            
        }

        /// <summary>
        /// Gets the size of a data block.
        /// </summary>
        /// <param name="uiSTIMESize">the size of an STIME type depending on the time format specified
        /// in the meter from Table 0.</param>
        /// <param name="byPhasesMonitored">The number of phases being monitored in the meter.</param>
        /// <param name="uiIntervalsPerBlock">The number of intervals in this block.</param>
        /// <returns>The size of the data block.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public static uint Size(uint uiSTIMESize, byte byPhasesMonitored, UInt16 uiIntervalsPerBlock)
        {
            uint uiSize = 0;

            // Block end time
            uiSize += uiSTIMESize;

            // Block end readings
            uiSize += (uint)(byPhasesMonitored * 4);

            // Simple interval status
            uiSize += (uint)((uiIntervalsPerBlock + 7) / 8);

            uiSize += uiIntervalsPerBlock * VMIntervalDataRecord.Size(byPhasesMonitored);

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the end time of the block
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public DateTime BlockEndTime
        {
            get
            {
                return m_dtBlockEndTime;
            }
        }

        /// <summary>
        /// Gets the end readings for the block per phase.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public UInt32[] BlockEndReadings
        {
            get
            {
                return (UInt32[])(m_uiEndReadings.Clone());
            }
        }

        

        /// <summary>
        /// Gets the number of valid intervals for the block.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public ushort NumberOfValidIntervals
        {
            get
            {
                return m_uiValidIntervalsPerBlock;
            }
        }

        /// <summary>
        /// Gets the simple interval statuses.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public byte[] SimpleIntervalStatuses
        {
            get
            {
                return (byte[])(m_abySimpleIntervalStatus.Clone());
            }
        }

        /// <summary>
        /// Gets the intervals stored in the block.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public ReadOnlyCollection<VMIntervalDataRecord> Intervals
        {
            get
            {
                return Array.AsReadOnly<VMIntervalDataRecord>(m_Intervals);
            }
        }

        #endregion

        #region Member Variables

        private byte m_byPhasesMonitored = 0;
        private UInt16 m_uiValidIntervalsPerBlock = 0;
        private UInt16 m_uiMaxIntervalsPerBlock = 0;
        private int m_iTimeFormat = 0;

        private DateTime m_dtBlockEndTime;
        private UInt32[] m_uiEndReadings;
        private byte[] m_abySimpleIntervalStatus;
        private VMIntervalDataRecord[] m_Intervals;

        #endregion
    }


    /// <summary>
    /// Interval object for an enhanced voltage monitoring interval.
    /// </summary>
    public class VMIntervalDataRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="byNumberOfPhases">The number of phases being monitored in the meter.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public VMIntervalDataRecord(byte byNumberOfPhases)
        {
            m_byNumberOfPhases = byNumberOfPhases;
        }

        /// <summary>
        /// Parses the interval data from the binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public void Parse(PSEMBinaryReader Reader)
        {
            // Read the interval status.
            m_IntervalStatus = (VMStatusFlags)(Reader.ReadUInt16());

            // Read the Vh data.
            m_VhData = new UInt16[m_byNumberOfPhases];

            for (int iIndex = 0; iIndex < m_byNumberOfPhases; iIndex++)
            {
                m_VhData[iIndex] = Reader.ReadUInt16();                
            }

            // Read the Vmin data.
            m_VminData = new UInt16[m_byNumberOfPhases];

            for (int iIndex = 0; iIndex < m_byNumberOfPhases; iIndex++)
            {
                m_VminData[iIndex] = Reader.ReadUInt16();
            }

            // Read the Vmax data.
            m_VmaxData = new UInt16[m_byNumberOfPhases];

            for (int iIndex = 0; iIndex < m_byNumberOfPhases; iIndex++)
            {
                m_VmaxData[iIndex] = Reader.ReadUInt16();
            }
        }

        /// <summary>
        /// Gets the size of an enhanced voltage monitoring interval.
        /// </summary>
        /// <param name="byNumberOfPhases">The number of phases being monitored in the meter.</param>
        /// <returns>The size of an interval.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public static uint Size(byte byNumberOfPhases)
        {
            uint uiSize = 0;

            // interval status
            uiSize += 2;

            // Vh data
            uiSize += (uint)(2 * byNumberOfPhases);
            // Vmin data
            uiSize += (uint)(2 * byNumberOfPhases);
            // Vmax data
            uiSize += (uint)(2 * byNumberOfPhases);

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the interval statuses.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        public VMStatusFlags IntervalStatus
        {
            get
            {
                return m_IntervalStatus;
            }
        }

        /// <summary>
        /// Gets the Vh interval data for each phase.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/28/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16[] VhData
        {
            get
            {
                return (UInt16[])(m_VhData.Clone());
            }
        }

        /// <summary>
        /// Gets the Vmin interval data for each phase.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/28/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16[] VminData
        {
            get
            {
                return (UInt16[])(m_VminData.Clone());
            }
        }

        /// <summary>
        /// Gets the Vmax interval data for each phase.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/28/11 jrf 2.53.10 TC5321 Created
        //
        public UInt16[] VmaxData
        {
            get
            {
                return (UInt16[])(m_VmaxData.Clone());
            }
        }

        #endregion

        #region Member Variables

        private byte m_byNumberOfPhases = 0;
        private UInt16[] m_VhData = null;
        private UInt16[] m_VminData = null;
        private UInt16[] m_VmaxData = null;
        private VMStatusFlags m_IntervalStatus;

        #endregion
    }
}
