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
using System.IO;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{

    /// <summary>
    /// Mfg table 2069 - Actual Power Quality limiting table.
    /// </summary>
    internal class MfgActualPowerQuality : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_TABLE_2069 = 5;
        private const ushort SAG_LEVEL_BITMASK = 0x0003;
        private const ushort SWELL_LEVEL_BITMASK = 0x000C;
        private const ushort SWELL_LEVEL_SHIFT = 2;
        private const ushort INTERRUPTION_LEVEL_BITMASK = 0x0030;
        private const ushort INTERRUPTION_LEVEL_SHIFT = 4;
        private const ushort V_IMB_LEVEL_BITMASK = 0x00C0;
        private const ushort V_IMB_LEVEL_SHIFT = 6;
        private const ushort I_IMB_LEVEL_BITMASK = 0x0300;
        private const ushort I_IMB_LEVEL_SHIFT = 8;
        private const ushort EXCURSION_LEVEL_BITMASK = 0x0C00;
        private const ushort EXCURSION_LEVEL_SHIFT = 10;
        private const byte UNDER_V_LIMIT_BITMASK = 0x01;
        private const byte OVER_V_LIMIT_BITMASK = 0x02;
        private const byte THD_V_EXCESS_BITMASK = 0x04;
        private const byte THD_I_EXCESS_BITMASK = 0x08;
        private const byte INSTANTANEOUS_THD_TDD_BITMASK = 0x010;
        private const byte THRESHOLD_ACTUAL_BITMASK = 0x020;
        private const byte EVENT_DURATION_BITMASK = 0x01;
        private const byte EVENT_START_TIME_BITMASK = 0x02;
        private const byte EVENT_END_TIME_BITMASK = 0x04;
        private const byte PQ_INHIBIT_OVF_BITMASK = 0x08;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Mfg Table 21 (2069) - This table defines the layout of the VQ
        /// tables in this meter.
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table0">The table 0 object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public MfgActualPowerQuality(CPSEM psem, CTable00 Table0)
            : base(psem, 2069, SIZE_OF_TABLE_2069)
        {
            m_Table0 = Table0;
        }


        /// <summary>
        /// Reads the table if it hasn't been read or if it's been written
        /// since it was last read.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public override PSEMResponse Read()
        {
            // Read the table 
            PSEMResponse Result = PSEMResponse.Ok;


            if (TableState.Loaded != State)
            {
                Result = base.Read();
                m_DataStream.Position = 0;

                if (Result == PSEMResponse.Ok)
                {
                    m_MaxPQEvents = m_Reader.ReadByte();
                    m_PQLevels = m_Reader.ReadUInt16();
                    m_PQFlags1 = m_Reader.ReadByte();
                    m_PQFlags2 = m_Reader.ReadByte();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The maximum number of VQ events the meter stores. Reads the table
        /// if necessary.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public byte MaxPQEvents
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return m_MaxPQEvents;
            }
        }

        /// <summary>
        /// The number of VQ sag levels configured to be monitored. Reads the
        /// table if necessary.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public byte NbrSagLevels
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return (byte)(m_PQLevels & SAG_LEVEL_BITMASK);
            }
        }

        /// <summary>
        /// The number of VQ swell levels configured to be monitored.
        /// </summary>
        ///<remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/12/07 mcm 8.0.11  N/A    Created
        /// 03/12/07 MCM 8.00.18 2573   Returned wrong value which cause table 2070 read failure
        ///</remarks> 
        public byte NbrSwellLevels
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return (byte)((m_PQLevels & SWELL_LEVEL_BITMASK)>>SWELL_LEVEL_SHIFT);
            }
        }

        /// <summary>
        /// The number of VQ levels configured to be monitored.
        /// </summary>
        ///<remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/12/07 mcm 8.0.11  N/A    Created
        /// 03/12/07 MCM 8.00.18 2573   Returned wrong value which cause table 2070 read failure
        ///</remarks> 
        public byte NbrInterruptionLevels
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return (byte)((m_PQLevels & INTERRUPTION_LEVEL_BITMASK) >> INTERRUPTION_LEVEL_SHIFT);
            }
        }

        /// <summary>
        /// The number of VQ Voltage Imbalance levels configured to be monitored.
        /// </summary>
        ///<remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/12/07 mcm 8.0.11  N/A    Created
        /// 03/12/07 MCM 8.00.18 2573   Returned wrong value which cause table 2070 read failure
        ///</remarks> 
        public byte NbrVoltageImbalanceLevels
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return (byte)((m_PQLevels & V_IMB_LEVEL_BITMASK)>>V_IMB_LEVEL_SHIFT);
            }
        }

        /// <summary>
        /// The number of VQ Current Imbalance levels configured to be monitored.
        /// </summary>
        ///<remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/12/07 mcm 8.0.11  N/A    Created
        /// 03/12/07 MCM 8.00.18 2573   Returned wrong value which cause table 2070 read failure
        ///</remarks> 
        public byte NbrCurrentImbalanceLevels
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return (byte)((m_PQLevels & I_IMB_LEVEL_BITMASK) >> I_IMB_LEVEL_SHIFT);
            }
        }

        /// <summary>
        /// The number of VQ Excursion levels configured to be monitored.
        /// </summary>
        ///<remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/12/07 mcm 8.0.11  N/A    Created
        /// 03/12/07 MCM 8.00.18 2573   Returned wrong value which cause table 2070 read failure
        ///</remarks> 
        public byte NbrExcursionLevels
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return (byte)((m_PQLevels & EXCURSION_LEVEL_BITMASK) >> EXCURSION_LEVEL_SHIFT);
            }
        }

        /// <summary>
        /// Does the device support under Voltage limit.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool UnderVoltageLimited
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags1 & UNDER_V_LIMIT_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Does the device support over Voltage limit.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool OverVoltageLimited
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }
 
                if(0 != (m_PQFlags1 & OVER_V_LIMIT_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Does the device support the ability to determine voltage THD excess.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool ThdVoltageExcessSupported
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags1 & THD_V_EXCESS_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Does the device support the ability to determine current THD excess.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool ThdCurrentExcessSupported
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags1 & THD_I_EXCESS_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Does the device support the ability to to calculate instantaneous 
        /// %THD and %TDD quantities.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool InstThdSupported
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags1 & INSTANTANEOUS_THD_TDD_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Does the device represents all threshold values as actual values 
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool ActualThresholdValues
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags1 & THRESHOLD_ACTUAL_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Does the device support the ability to record the power 
        /// quality event duration.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool EventDurationSupported
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags2 & EVENT_DURATION_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Does the device support the ability to record the power 
        /// quality event start time.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool EventStartTimeSupported
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags2 & EVENT_START_TIME_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Does the device support the ability to record the power quality 
        /// event end time.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool EventEndTimeSupported
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags2 & EVENT_END_TIME_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Is the device capable of inhibiting overflow of the Power Quality Log.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool InhibitOverflowSupported
        {
            get 
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_PQFlags2 & PQ_INHIBIT_OVF_BITMASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        #endregion

        #region Members

        private CTable00 m_Table0;
        private byte m_MaxPQEvents;
        private ushort m_PQLevels;
        private byte m_PQFlags1;
        private byte m_PQFlags2;

        #endregion

    }


    /// <summary>
    /// Mfg table 2070. This table is only partially implemented. The meter's
    /// table shows all VQ configuration parameters.  This table only exposes
    /// the Log Near Full Threshold.
    /// </summary>
    internal class MfgPowerQualityParameters : AnsiTable
    {
       #region Constants

        private const uint SIZE_OF_TABLE_2070_HEADER = 4;
        private const uint SIZE_OF_PQ_PARAMETER = 12;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods
       
        /// <summary>
        /// Mfg Table 22 (2070) - This table defines the layout of the VQ
        /// tables in this meter.
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table2069">The Actual Power Quality table that defines
        /// the struction of this table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public MfgPowerQualityParameters(CPSEM psem, MfgActualPowerQuality Table2069)
            : base(psem, 2070, MfgPowerQualityParameters.GetTableSize(Table2069))
        {
            m_Table2069 = Table2069;
        }

        /// <summary>
        /// Gets the size of table 2070
        /// </summary>
        /// <param name="Table2069">The Actual Power Quality table that defines
        /// the struction of this table</param>
        /// <returns>The size of the table.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        private static uint GetTableSize(MfgActualPowerQuality Table2069)
        {
            int NbrParameters = Table2069.NbrSagLevels + 
                                Table2069.NbrSwellLevels +
                                Table2069.NbrInterruptionLevels + 
                                Table2069.NbrVoltageImbalanceLevels +
                                Table2069.NbrCurrentImbalanceLevels + 
                                Table2069.NbrExcursionLevels;

            return (uint)( SIZE_OF_TABLE_2070_HEADER +
                SIZE_OF_PQ_PARAMETER * NbrParameters);
        }
       
        /// <summary>
        /// Reads the table if it hasn't been read or if it's been written
        /// since it was last read.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public override PSEMResponse Read()
        {
            // Read the table 
            PSEMResponse Result = PSEMResponse.Ok;


            if (TableState.Loaded != State)
            {
                Result = base.Read();
                m_DataStream.Position = 0;

                if (Result == PSEMResponse.Ok)
                {
                    m_PQInhibit = m_Reader.ReadUInt16();
                    m_VQCalcMethod = m_Reader.ReadByte();
                    m_LogNearFullThreshold = m_Reader.ReadByte();

                    // TODO: read the rest of the members when they're added
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The configured threshold for triggering a VQ Log Nearly Full event.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public byte LogNearFullThreshold
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return m_LogNearFullThreshold;
            }
        }

        #endregion


        #region Members

        private MfgActualPowerQuality m_Table2069;
        private ushort m_PQInhibit;
        private byte m_VQCalcMethod;
        private byte m_LogNearFullThreshold;
	
        // TODO: implement the rest of this table's members and properties to 
        // access them if and when we need them.

        #endregion


    }


    /// <summary>
    /// Mfg table 2071. This table contains VQ Status data.
    /// </summary>
    internal class MfgPowerQualityStatus : AnsiTable
    {
       #region Constants

        private const uint SIZE_OF_TABLE_2071 = 8;

        private const byte OVERFLOW_FLAG_MASK = 0x01;
	    private const byte INHIBIT_OVERFLOW_FLAG_MASK = 0x02;
	    private const byte PQ_RUNNING_MASK = 0x04;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods
       
        /// <summary>
        /// Mfg Table 23 (2071) - This table defines the layout of the VQ
        /// tables in this meter.
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table2069">The Actual Power Quality table that defines
        /// the struction of this table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public MfgPowerQualityStatus(CPSEM psem, MfgActualPowerQuality Table2069)
            : base(psem, 2071, SIZE_OF_TABLE_2071)
        {
            m_Table2069 = Table2069;
        }

        /// <summary>
        /// Reads the table if it hasn't been read or if it's been written
        /// since it was last read.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public override PSEMResponse Read()
        {
            // Read the table 
            PSEMResponse Result = PSEMResponse.Ok;


            if (TableState.Loaded != State)
            {
                Result = base.Read();
                m_DataStream.Position = 0;

                if (Result == PSEMResponse.Ok)
                {
                    m_StatusFlags = m_Reader.ReadByte();
                    m_NbrEvents = m_Reader.ReadByte();
                    m_LastEntry = m_Reader.ReadByte();
                    m_LastSeqNbr = m_Reader.ReadUInt32();
                    m_NbrUnreadEntries = m_Reader.ReadByte();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Has the log in an overflowed. If the log has overflowed, either new 
        /// events are not being logged because the meter is blocking them, or 
        /// old events that have not been read are being overwritten. Clearing 
        /// VQ Events clears this flag.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool Overflowed
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_StatusFlags & OVERFLOW_FLAG_MASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        /// <summary>
        /// Is the meter configured to keep unread events from being overwritten
        /// when the log gets full?  This value is the same as the corresponding
        /// table 2069 value
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool InhibitOverflow
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_StatusFlags & INHIBIT_OVERFLOW_FLAG_MASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }	   

        /// <summary>
        /// Is PQ currently running.  PQ will stop runnign if the meter can
        /// not determine the service type and when the meter is in test mode.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public bool PQRunning
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                if (0 != (m_StatusFlags & PQ_RUNNING_MASK))
                {
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }	   

        /// <summary>
        /// Number of the events currently logged in table 2072
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public byte NbrEvents
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return m_NbrEvents;
            }
        }

        /// <summary>
        /// Index of the newest event in table 2072
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public byte LastEntry
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return m_LastEntry;
            }
        }

        /// <summary>
        /// Sequence number of the newest event in table 2072
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public UInt32 LastSeqNbr
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return m_LastSeqNbr;
            }
        }

        /// <summary>
        /// How many events in table 2072 are marked unread?
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public byte NbrUnreadEntries
        {
            get
            {
                if (TableState.Loaded != State)
                {
                    Read(); // Throws and exception if it fails
                }

                return m_NbrUnreadEntries;
            }
        }

        #endregion

        #region Members

        private MfgActualPowerQuality m_Table2069;
        private byte m_StatusFlags;
        private byte m_NbrEvents;
        private byte m_LastEntry;
        private UInt32 m_LastSeqNbr;
        private byte m_NbrUnreadEntries;

        #endregion

    }

    /// <summary>
    /// Mfg table 2072. This table contains an array of VQ events.
    /// </summary>
    internal class MfgPowerQualityEvents : AnsiTable
    {
        #region Constants

        private const uint SIZE_OF_TABLE_2072_HEADER = 10;
        private const uint SIZE_OF_PQ_EVENT = 18;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Mfg Table 24 (2069) - This table defines the layout of the VQ
        /// tables in this meter.
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table2069">The Actual Power Quality table that defines
        /// the struction of this table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public MfgPowerQualityEvents(CPSEM psem, MfgActualPowerQuality Table2069)
            : base(psem, 2072, MfgPowerQualityEvents.GetTableSize(Table2069))
        {
            m_Table2069 = Table2069;
//            m_EventList = new List<PowerQualityEvent>();
        }

        /// <summary>
        /// Reads the first NbrEvents from table 2072 and returns them in a 
        /// list. 
        /// </summary>
        /// <param name="StartIndex">0 based event index</param>
        /// <param name="Count">Number of VQ events to read</param>
        /// <returns>A list of PQ events</returns>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public List<PowerQualityEvent> GetEvents(int StartIndex, int Count)
        {
            List<PowerQualityEvent> EventList = new List<PowerQualityEvent>();

            if (StartIndex + Count <= m_Table2069.MaxPQEvents )
            {
                ushort Offset = (ushort)(StartIndex * SIZE_OF_PQ_EVENT);

                Read(Offset, (ushort)(Count * SIZE_OF_PQ_EVENT));

                // mcm 3/9/2007 - SCR 2553: reposition stream for cases
                // where the table might be read mulitple times.
                m_DataStream.Position = 0;

                while (Count-- > 0)
                {
                    try
                    {
                        EventList.Add(new PowerQualityEvent(m_Reader.ReadByte(),
                            m_Reader.ReadUInt32(), m_Reader.ReadSingle(),
                            m_Reader.ReadSingle(), m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat)));
                    }
                    catch (Exception e)
                    {
                        string s = e.Message;
                    }
                }
            }

            return EventList;
        }

        /// <summary>
        /// Gets the size of table 2072
        /// </summary>
        /// <param name="Table2069">The Actual Power Quality table that defines
        /// the struction of this table</param>
        /// <returns>The size of the table.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        private static uint GetTableSize(MfgActualPowerQuality Table2069)
        {
            return Table2069.MaxPQEvents * SIZE_OF_PQ_EVENT;
        }

        #endregion

        #region Public Properties
        #endregion

     
        #region Members

        private MfgActualPowerQuality m_Table2069;
        //private List<PowerQualityEvent> m_EventList;

        #endregion


    }

    /// <summary>
    /// Power Quality Event type is a binary value defined as follows:
    ///      0		= Sag
    ///      1		= Swell
    ///      2		= Interruption
    ///      3		= Voltage Unbalance
    ///      4		= Current Unbalance
    ///      5		= Excursion
    ///      6..7	= Undefined
    /// </summary>
    /// 
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 02/13/07 mcm 8.0.11  N/A    Created
    public enum PQEventType
    {
        /// <summary>
        /// Sag event occurs when voltage drops below a configured value
        /// </summary>
        Sag,
        /// <summary>
        /// Swell event occurs when voltage rises above a configured value
        /// </summary>
        Swell,
        /// <summary>
        /// Interruption event occurs when voltage drops below a configured 
        /// value for a configured duration
        /// </summary>
        Interruption,
        /// <summary>
        /// Voltage Unbalance event occurs when a phase voltage drops below 
        /// a configured difference from the average voltage.
        /// </summary>
        VoltageUnbalance,
        /// <summary>
        /// Current Unbalance event occurs when a phase current drops below 
        /// a configured difference from the average current.
        /// </summary>
        CurrentUnbalance,
        /// <summary>
        /// An Excursion is when the entire family goes on holiday
        /// </summary>
        Excursion
    }


    /// <summary>
    /// Structure representing a single Power Quality Event.  Note that this
    /// struct is set up to match the Sentinel Saturn's PQ events.  It does not
    /// currently support dynamic structuring based on the table 2069 parameters.
    /// </summary>
    public struct PowerQualityEvent
    {
        #region Constants

        private const byte PHASE_MASK = 0x03; // Bits (0..1)
        private const byte TYPE_MASK = 0x1C; // Bits (2..4)
        private const byte TYPE_SHIFT = 2; 
        private const byte LEVEL_MASK = 0x60; // Bits (5..6)
        private const byte LEVEL_SHIFT = 5;
        private const byte SATURATION_MASK = 0x80; // Bits (7)

        private const byte AGGREGATE_PHASE = 0; 
        private const byte A_PHASE = 1; 
        private const byte B_PHASE = 2; 
        private const byte C_PHASE = 3; 


        #endregion

        /// <summary>
        /// Constructs a PowerQualityEvent
        /// </summary>
        /// <param name="EventType">EventType</param>
        /// <param name="Duration">Duration</param>
        /// <param name="EventData1">EventData1</param>
        /// <param name="EventData2">EventData2</param>
        /// <param name="EndTime">Event end time</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public PowerQualityEvent(byte EventType, UInt32 Duration,
            float EventData1, float EventData2, DateTime EndTime)
        {
            this.m_EventData = EventType;
            this.m_Duration = Duration;
            this.m_EventData1 = EventData1;
            this.m_EventData2 = EventData2;
            this.m_EndTime = EndTime;       
        }

        #region Properties

        /// <summary>
        /// Event duration in miliseconds
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public UInt32 Duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        /// <summary>
        /// Event Data is particular to the event type. If EVENT_TYPE is a sag, 
        /// swell, interruption, or excursion then this field represents the
        /// minimum or maximum voltage amplitude during power quality event. 
        /// If EVENT_TYPE is a voltage unbalance then this field represents the 
        /// average voltage during the power quality event.
        /// 
        /// Use the EventData1Description property to get a nicely formatted
        /// description of the data.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public float Data1
        {
            get { return m_EventData1; }
            set { m_EventData1 = value; }
        }

        /// <summary>
        /// Event Data is particular to the event type. This property returns
        /// the Event Data formatted with the units and type of value.  For
        /// example, event data 1 for a Sag event would be "116.4 V min".
        /// </summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/13/07 mcm 8.0.11  N/A    Created
        /// 03/12/07 mcm 8.00.18 2561   0 values are not formatted correctly
        /// </remarks>
        public string Data1Description
        {
            get 
            {
                string Description;

                switch (Type)
                {
                    case PQEventType.Sag:
                    case PQEventType.Interruption:
                    {
                        Description = m_EventData1.ToString("##0.0 V min", CultureInfo.CurrentCulture);
                        break;
                    }
                    case PQEventType.Swell:
                    case PQEventType.Excursion:
                    {
                        Description = m_EventData1.ToString("##0.0 V max", CultureInfo.CurrentCulture);
                        break;
                    }
                    case PQEventType.VoltageUnbalance:
                    {
                        Description = m_EventData1.ToString("##0.0 V avg", CultureInfo.CurrentCulture);
                        break;
                    }
                    case PQEventType.CurrentUnbalance:
                    {
                        Description = m_EventData1.ToString("#0.000 A avg", CultureInfo.CurrentCulture);
                        break;
                    }
                    default:
                    {
                        Description = "Undefined Event";
                        break;
                    }
                }

                return Description; 
            }
        }

        /// <summary>
        /// Event Data is particular to the event type. If EVENT_TYPE is a sag,
        /// swell, interruption, or excursion then this field represents the 
        /// peak current during the power quality event. If EVENT_TYPE is a
        /// current unbalance or a voltage unbalance then this field represents
        /// the peak percent deviation during the power quality event.
        /// 
        /// Use the EventData2Description property to get a nicely formatted
        /// description of the data.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public float Data2
        {
            get { return m_EventData2; }
            set { m_EventData2 = value; }
        }

        /// <summary>
        /// Event Data is particular to the event type. This property returns
        /// the Event Data formatted with the units and type of value.  For
        /// example, event data 2 for a Sag event would be "14.378 A".
        /// </summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/13/07 mcm 8.0.11  N/A    Created
        /// 03/12/07 mcm 8.00.18 2561   0 values are not formatted correctly
        /// </remarks>
        public string Data2Description
        {
            get 
            {
                string Description;

                switch (Type)
                {
                    case PQEventType.Sag:
                    case PQEventType.Swell:
                    case PQEventType.Interruption:
                    case PQEventType.Excursion:
                    {
                        Description = m_EventData2.ToString("#0.000 A", CultureInfo.CurrentCulture);
                        break;
                    }
                    case PQEventType.VoltageUnbalance:
                    case PQEventType.CurrentUnbalance:
                    {
                        Description = m_EventData2.ToString("#0.00", CultureInfo.CurrentCulture) + "%";
                        break;
                    }
                    default:
                    {
                        Description = "Undefined Event";
                        break;
                    }
                }

                //m_EventData1
                return Description;
            }
        }

        /// <summary>
        /// Event End Time
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public DateTime EndTime
        {
            get { return m_EndTime; }
            set { m_EndTime = value; }
        }

        /// <summary>
        /// Returns the calculated event start time based on the end time and
        /// the event duration.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public DateTime StartTime
        {
            get 
            { 
                TimeSpan tsDuration = new TimeSpan(0,0,0,0, (int)Duration);

                return m_EndTime - tsDuration; 
            }
        }

        /// <summary>
        /// Event type is a binary value defined as follows:
        ///      0		= Sag
        ///      1		= Swell
        ///      2		= Interruption
        ///      3		= Voltage Unbalance
        ///      4		= Current Unbalance
        ///      5		= Excursion
        ///      6..7	= Undefined
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public PQEventType Type
        {
            get 
            { 
                return (PQEventType)((m_EventData & TYPE_MASK) >> TYPE_SHIFT); 
            }
            set 
            { 
                m_EventData = (byte)(m_EventData & 
                    (((byte)value << TYPE_SHIFT) & TYPE_MASK)); 
            }
        }

        /// <summary>
        /// Returns the PQEventType in string form
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public string Description
        {
            get 
            { 
                string Desc;

                switch(Type)
                {
                    case PQEventType.Sag:
                    {
                        Desc = "Sag";
                        break;
                    }
                    case PQEventType.Swell:
                    {
                        Desc = "Swell";
                        break;
                    }
                    case PQEventType.Interruption:
                    {
                        Desc = "Interruption";
                        break;
                    }
                    case PQEventType.CurrentUnbalance:
                    {
                        Desc = "Current Unbalance";
                        break;
                    }
                    case PQEventType.VoltageUnbalance:
                    {
                        Desc = "Voltage Unbalance";
                        break;
                    }
                    case PQEventType.Excursion:
                    {
                        Desc = "Excursion";
                        break;
                    }
                    default:
                    {
                        Desc = "Undefined";
                        break;
                    }
                }

                return Desc; 
            }
        }

        /// <summary>
        /// Returns the phase the event is associated with
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public string Phase
        {
            get
            {
                byte byPhase = (byte)(m_EventData & PHASE_MASK);

                if(AGGREGATE_PHASE == byPhase)
                {
                    return "Agg";
                }
                else if(A_PHASE == byPhase)
                {
                    return "A";
                }
                else if(B_PHASE == byPhase)
                {
                    return "B";
                }
                else
                {
                    return "C";
                }
            }
        }

        /// <summary>
        /// Returns the Level (or Class for Interruptions) that the event is 
        /// associated with.
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public string Level
        {
            get
            {
                return ((m_EventData & LEVEL_MASK)>>LEVEL_SHIFT).ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Was there current saturation during the event?
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/07 mcm 8.0.11  N/A    Created
        public string Saturation
        {
            get
            {
                if (0 == (m_EventData & SATURATION_MASK))
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
        }

        #endregion Properties


        #region Private Members

        private UInt32 m_Duration;
        private float m_EventData1;
        private float m_EventData2;
        private DateTime m_EndTime;   // 5 byte LTIME_DATE in stream        
        private byte m_EventData;     // First item in struct, moved for byte alignment

        #endregion Private Members
    }
}
