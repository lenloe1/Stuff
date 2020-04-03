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
//                              Copyright © 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    #region Stack Switch Enums

    /// <summary>
    /// Source of the request to switch the stack
    /// </summary>
    public enum STACK_SWITCH_REQUEST_SOURCE : byte
    {
        /// <summary>
        /// No request source
        /// </summary>
        None = 0,
        /// <summary>
        /// Activate RFLAN firmware
        /// </summary>
        ActivateRFLANFW = 1,
        /// <summary>
        /// Check RFLAN in Bootloader mode
        /// </summary>
        ChkRFLANInBootload = 2,
        /// <summary>
        /// Monitor RFLAN Link
        /// </summary>
        MonitorRFLANLink = 3,
        /// <summary>
        /// IPv6 Stack Reset
        /// </summary>
        IPv6StackReset = 4,
        /// <summary>
        /// Mfg Procedure 159
        /// </summary>
        MFGProc159 = 5
    }

    /// <summary>
    /// Stack Type - RFLAN or IP
    /// </summary>
    public enum STACK_TYPE : byte
    {
        /// <summary>
        /// RFLAN/C12.22
        /// </summary>
        [EnumDescription("RFLAN")]
        C1222 = 0,
        /// <summary>
        /// IPv6
        /// </summary>
        [EnumDescription("IPv6")]
        IP = 1,
        /// <summary>
        /// Invalid
        /// </summary>
        [EnumDescription("Invalid")]
        Invalid = 2
    }

    #endregion

    internal class OpenWayMFGTable2640 : AnsiTable
    {
        #region Constants

        private const int TABLE_2640_LENGTH = 42;
        private const int NUMBER_OF_HISTORY_RECORDS = 4;

        #endregion

        #region Definitions

        /// <summary>
        /// State of the stack switch
        /// </summary>
        public enum COMM_STACK_SWITCH_STATE : uint
        {
            StackSwitchIdle = 0,
            StackSwitchStateRequested = 0x53575251,
            StackSwitchInProgress = 0x53575047
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public OpenWayMFGTable2640(CPSEM psem)
            : base(psem, 2640, TABLE_2640_LENGTH)
        {
            m_StackSwitchHistoryRecords = new DualStackSwitchHistoryRecord[4];
        }

        /// <summary>
        /// Full read of 2640 (Mfg 592) from the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "OpenWayMFGTable2640.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_FallbackEnabled = m_Reader.ReadByte();
                m_CurrentStack = (STACK_TYPE)m_Reader.ReadByte();
                m_LastSwitchTime = m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();
                m_StackSwitchCount = m_Reader.ReadUInt32();
                m_StackSwitchState = (COMM_STACK_SWITCH_STATE)m_Reader.ReadUInt32();
                m_SwitchToWhichStack = (STACK_TYPE)m_Reader.ReadByte();
                m_StackAfterLastSwitch = (STACK_TYPE)m_Reader.ReadByte();
                m_StackBeforeLastSwitch = (STACK_TYPE)m_Reader.ReadByte();

                for(int iIndex = 0; iIndex < NUMBER_OF_HISTORY_RECORDS; iIndex++)
                {
                    m_StackSwitchHistoryRecords[iIndex] = new DualStackSwitchHistoryRecord();
                    m_StackSwitchHistoryRecords[iIndex].SwitchRequestSource = (STACK_SWITCH_REQUEST_SOURCE)m_Reader.ReadByte();
                    m_StackSwitchHistoryRecords[iIndex].SwitchRequestTime = m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns whether or not fallback is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public bool FallbackEnabled
        {
            get
            {
                ReadUnloadedTable();
                bool Enabled = false;
                if (m_FallbackEnabled == 1)
                {
                    Enabled = true;
                }

                return Enabled;
            }
        }

        /// <summary>
        /// Which stack is currently active
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public STACK_TYPE CurrentStackType
        {
            get
            {
                ReadUnloadedTable();

                return m_CurrentStack;
            }
        }

        /// <summary>
        /// When did the last stack switch occur
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public DateTime LastSwitchTime
        {
            get
            {
                ReadUnloadedTable();

                return m_LastSwitchTime;
            }
        }

        /// <summary>
        /// How many stack switches have occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public UInt32 StackSwitchCount
        {
            get
            {
                ReadUnloadedTable();

                return m_StackSwitchCount;
            }
        }

        /// <summary>
        /// Reads the state of the stack switch: idle, stack switch requested, or 
        /// stack switch in progress
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public COMM_STACK_SWITCH_STATE CommStackSwitchState
        {
            get
            {
                ReadUnloadedTable();

                return m_StackSwitchState;
            }
        }

        /// <summary>
        /// Reads the type of stack to which the meter will switch
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public STACK_TYPE SwitchToWhichStack
        {
            get
            {
                ReadUnloadedTable();

                return m_SwitchToWhichStack;
            }
        }

        /// <summary>
        /// Reads the type of stack after the last switch.  Should be the same as
        /// the current stack type if there has ever been a switch
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public STACK_TYPE StackAfterLastSwitch
        {
            get
            {
                ReadUnloadedTable();

                return m_StackAfterLastSwitch;
            }
        }

        /// <summary>
        /// Reads the stack that was active before the last switch
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public STACK_TYPE StackBeforeLastSwitch
        {
            get
            {
                ReadUnloadedTable();

                return m_StackBeforeLastSwitch;
            }
        }

        /// <summary>
        /// Reads the Comm stack switch history from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public List<DualStackSwitchHistoryRecord> StackSwitchHistoryRecords
        {
            get
            {
                ReadUnloadedTable();
                List<DualStackSwitchHistoryRecord> StackSwitchRecords = new List<DualStackSwitchHistoryRecord>();

                foreach (DualStackSwitchHistoryRecord rcd in m_StackSwitchHistoryRecords)
                {
                    StackSwitchRecords.Add(rcd);
                }

                return StackSwitchRecords;
            }
        }

        #endregion

        #region Members

        private byte m_FallbackEnabled;
        private STACK_TYPE m_CurrentStack;
        private DateTime m_LastSwitchTime;
        private UInt32 m_StackSwitchCount;
        private COMM_STACK_SWITCH_STATE m_StackSwitchState;
        private STACK_TYPE m_SwitchToWhichStack;
        private STACK_TYPE m_StackAfterLastSwitch;
        private STACK_TYPE m_StackBeforeLastSwitch;
        private DualStackSwitchHistoryRecord[] m_StackSwitchHistoryRecords;

        #endregion

    }

    /// <summary>
    /// Class that represents a single stack switch history record
    /// </summary>
    public class DualStackSwitchHistoryRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public DualStackSwitchHistoryRecord()
        {
            m_SwitchRequestSource = STACK_SWITCH_REQUEST_SOURCE.None;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The source of the stack switch request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public STACK_SWITCH_REQUEST_SOURCE SwitchRequestSource
        {
            get
            {
                return m_SwitchRequestSource;
            }
            set
            {
                m_SwitchRequestSource = value;
            }
        }

        /// <summary>
        /// The time the switch was requested
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //
        public DateTime SwitchRequestTime
        {
            get
            {
                return m_SwitchRequestTime;
            }
            set
            {
                m_SwitchRequestTime = value;
            }
        }

        #endregion

        #region Members

        STACK_SWITCH_REQUEST_SOURCE m_SwitchRequestSource;
        DateTime m_SwitchRequestTime;

        #endregion
    }
}
