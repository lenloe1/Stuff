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
//                              Copyright © 2010
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
    /// MFG Table 2262 (Itron 214)
    /// </summary>
    public class OpenWayMFGTable2262 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 6;
        private const int TABLE_TIMEOUT = 5000;
        private const byte ACC_CONFIG_ERR_MASK = 0x01;
        private const byte WAKE_UP_STATUS_MASK = 0x02;
        private const byte REMOVAL_PDN_CHECK_MASK = 0x04;

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
        public OpenWayMFGTable2262(CPSEM psem)
            : base(psem, 2262, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2262.Read");

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
        /// If true, there are errors in accelerometer configuration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public bool AccelerometerConfigError
        {
            get
            {
                ReadUnloadedTable();

                return (m_byTamperTapStatus & ACC_CONFIG_ERR_MASK) == ACC_CONFIG_ERR_MASK;
            }
        }

        /// <summary>
        /// If false, tap and tamper detections are not running
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public bool WakeUpStatus
        {
            get
            {
                ReadUnloadedTable();

                return (m_byTamperTapStatus & WAKE_UP_STATUS_MASK) == WAKE_UP_STATUS_MASK;
            }
        }

        /// <summary>
        /// If true, a removal tamper has been detected and the meter is checking the power down for 10 seconds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public bool RemovalPDNCheck
        {
            get
            {
                ReadUnloadedTable();

                return (m_byTamperTapStatus & REMOVAL_PDN_CHECK_MASK) == REMOVAL_PDN_CHECK_MASK;
            }
        }

        /// <summary>
        /// Represents the internal state machine status.  If non-zero, a tap has been detected.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public bool TapCheck
        {
            get
            {
                Boolean blnTapDetected = false;

                if (m_byTapCheck != 0)
                {
                    blnTapDetected = true;
                }

                return blnTapDetected;
            }
        }

        /// <summary>
        /// The multiplier to apply to the angles in table 2263
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public ushort Scalar
        {
            get
            {
                ReadUnloadedTable();

                return m_usScalar;
            }
        }

        /// <summary>
        /// The divisor to apply to the angles in table 2263
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public ushort Divisor
        {
            get
            {
                ReadUnloadedTable();

                return m_usDivisor;
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
        //  02/03/10 AF  2.40.11        Created
        //
        private void ParseData()
        {
            m_byTamperTapStatus = m_Reader.ReadByte();
            m_byTapCheck = m_Reader.ReadByte();
            m_usScalar = m_Reader.ReadUInt16();
            m_usDivisor = m_Reader.ReadUInt16();
        }

        #endregion

        #region Members

        byte m_byTamperTapStatus;
        byte m_byTapCheck;
        ushort m_usScalar;
        ushort m_usDivisor;

        #endregion
    }

    /// <summary>
    /// MFG Table 2263 (Itron 215)
    /// </summary>
    public class OpenWayMFGTable2263 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 18;
        private const int TABLE_TIMEOUT = 5000;
        private const byte ACC_SUPPORT_MASK = 0x01;
        private const byte ICS_COMM_PRESENT = 0x02; // ICS Comm module present
        private const ushort DEFAULT_SCALAR = 1;
        private const ushort DEFAULT_DIVISOR = 100;

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
        public OpenWayMFGTable2263(CPSEM psem)
            : base(psem, 2263, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_Table2262 = new OpenWayMFGTable2262(psem);
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2263.Read");

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
        /// Whether or not the accelerometer is supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#     Description
        //  -------- --- ------- ------     -------------------------------------------
        //  02/03/10 AF  2.40.11            Created
        //  07/19/13 jkw 2.80.55 WR497791   ACC__SUPPORT_MASK bit means supported on the
        //                                  register board.  The ICS has its own accelerometer
        //                                  meaning the one on the register board is not supported
        //                                  when an ICS module is present
        //
        public bool AccelerometerSupported
        {
            get
            {
                ReadUnloadedTable();

                return ((m_byTampTapSupport & ACC_SUPPORT_MASK) == ACC_SUPPORT_MASK) ||
                        ((m_byTampTapSupport & ICS_COMM_PRESENT) == ICS_COMM_PRESENT);
            }
        }

        /// <summary>
        /// The reference angle of installation of the X axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public float ReferenceAngleX
        {
            get
            {
                ushort usScalar = DEFAULT_SCALAR;
                ushort usDivisor = DEFAULT_DIVISOR;

                ReadUnloadedTable();

                if (m_Table2262 != null)
                {
                    usScalar = m_Table2262.Scalar;
                    usDivisor = m_Table2262.Divisor;
                }

                return (float)(m_sRefAngleX * usScalar) / (float)usDivisor;
            }
        }

        /// <summary>
        /// The reference angle of installation of the Y axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public float ReferenceAngleY
        {
            get
            {
                ushort usScalar = DEFAULT_SCALAR;
                ushort usDivisor = DEFAULT_DIVISOR;

                ReadUnloadedTable();

                if (m_Table2262 != null)
                {
                    usScalar = m_Table2262.Scalar;
                    usDivisor = m_Table2262.Divisor;
                }

                return (float)(m_sRefAngleY * usScalar) / (float)usDivisor;
            }
        }

        /// <summary>
        /// The reference angle of installation of the Z axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public float ReferenceAngleZ
        {
            get
            {
                ushort usScalar = DEFAULT_SCALAR;
                ushort usDivisor = DEFAULT_DIVISOR;

                ReadUnloadedTable();

                if (m_Table2262 != null)
                {
                    usScalar = m_Table2262.Scalar;
                    usDivisor = m_Table2262.Divisor;
                }

                return (float)(m_sRefAngleZ * usScalar) / (float)usDivisor;
            }
        }

        /// <summary>
        /// Current angle of installation of the X axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public float CurrentAngleX
        {
            get
            {
                ushort usScalar = DEFAULT_SCALAR;
                ushort usDivisor = DEFAULT_DIVISOR;

                ReadUnloadedTable();

                if (m_Table2262 != null)
                {
                    usScalar = m_Table2262.Scalar;
                    usDivisor = m_Table2262.Divisor;
                }

                return (float)(m_sCurrAngleX * usScalar) / (float)usDivisor;
            }
        }

        /// <summary>
        /// Current angle of installation of the Y axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public float CurrentAngleY
        {
            get
            {
                ushort usScalar = DEFAULT_SCALAR;
                ushort usDivisor = DEFAULT_DIVISOR;

                ReadUnloadedTable();

                if (m_Table2262 != null)
                {
                    usScalar = m_Table2262.Scalar;
                    usDivisor = m_Table2262.Divisor;
                }

                return (float)(m_sCurrAngleY * usScalar) / (float)usDivisor;
            }
        }

        /// <summary>
        /// Current angle of installation of the Z axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public float CurrentAngleZ
        {
            get
            {
                ushort usScalar = DEFAULT_SCALAR;
                ushort usDivisor = DEFAULT_DIVISOR;

                ReadUnloadedTable();

                if (m_Table2262 != null)
                {
                    usScalar = m_Table2262.Scalar;
                    usDivisor = m_Table2262.Divisor;
                }

                return (float)(m_sCurrAngleZ * usScalar) / (float)usDivisor;
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along X axis since powerup
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public sbyte MaxDeltaX
        {
            get
            {
                ReadUnloadedTable();

                return m_sbyMaxDeltaX;
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along Y axis since powerup
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public sbyte MaxDeltaY
        {
            get
            {
                ReadUnloadedTable();

                return m_sbyMaxDeltaY;
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along Z axis since powerup
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public sbyte MaxDeltaZ
        {
            get
            {
                ReadUnloadedTable();

                return m_sbyMaxDeltaZ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public sbyte MaxAvgDeltaTap
        {
            get
            {
                ReadUnloadedTable();

                return m_sbyMaxAvgDeltaTap;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        public sbyte MaxAvgDeltaTamper
        {
            get
            {
                ReadUnloadedTable();

                return m_sbyMaxAvgDeltaTamper;
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
        //  02/03/10 AF  2.40.11        Created
        //
        private void ParseData()
        {
            m_byTampTapSupport = m_Reader.ReadByte();
            m_sRefAngleX = m_Reader.ReadInt16();
            m_sRefAngleY = m_Reader.ReadInt16();
            m_sRefAngleZ = m_Reader.ReadInt16();
            m_sCurrAngleX = m_Reader.ReadInt16();
            m_sCurrAngleY = m_Reader.ReadInt16();
            m_sCurrAngleZ = m_Reader.ReadInt16();
            m_sbyMaxDeltaX = m_Reader.ReadSByte();
            m_sbyMaxDeltaY = m_Reader.ReadSByte();
            m_sbyMaxDeltaZ = m_Reader.ReadSByte();
            m_sbyMaxAvgDeltaTap = m_Reader.ReadSByte();
            m_sbyMaxAvgDeltaTamper = m_Reader.ReadSByte();
        }

        #endregion

        #region Members

        private byte m_byTampTapSupport;
        private short m_sRefAngleX;
        private short m_sRefAngleY;
        private short m_sRefAngleZ;
        private short m_sCurrAngleX;
        private short m_sCurrAngleY;
        private short m_sCurrAngleZ;
        private sbyte m_sbyMaxDeltaX;
        private sbyte m_sbyMaxDeltaY;
        private sbyte m_sbyMaxDeltaZ;
        private sbyte m_sbyMaxAvgDeltaTap;
        private sbyte m_sbyMaxAvgDeltaTamper;
        private OpenWayMFGTable2262 m_Table2262;

        #endregion

    }
}
