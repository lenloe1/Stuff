
///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
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
//                        Copyright © 2010 - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Xml;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.DST;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the M2 Gateway.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 
    //
    public partial class SL7000_Gateway : CENTRON_AMI
    {
        #region Constants

        /// <summary>
        /// Meter type identifier
        /// </summary>
        public const string M2GATEWAY = "SL7000 Gateway";
        /// <summary>
        /// Device class string for M2 Gateway devices
        /// </summary>
        public const string LIS1_DEVICE_CLASS = "ITRS";
        /// <summary>
        /// Human readable name of meter
        /// </summary>
        private const string M2GATEWAY_NAME = "OpenWay SL7000 Gateway";
        /// <summary>
        /// Name used in the activity log
        /// </summary>
        private const string LOG_ITR1_NAME = "OW SL7000 Gateway";
        /// <summary>
        /// Constant Describing the Hardware Version for 1.0 meters
        /// </summary>
        public const float GTWY_HW_VERSION_1_0 = 1.00F;
        /// <summary>
        /// Number of pages in an M2 Gateway core dump
        /// </summary>
        private const UInt32 NUMBER_COREDUMP_PAGES = 325;
        /// <summary>
        /// Number of bytes in an M2 Gateway data flash page
        /// </summary>
        private const UInt32 NUMBER_BYTES_PER_PAGE = 256;
        /// <summary>
        /// Page in data flash where the M2 Gateway core dump starts
        /// </summary>
        private const UInt32 COREDUMP_START_PAGE = 262;

        private const byte FOCUS_OPTICAL_PORT_CONTROL = 0x01;

        private const int MAX_NUM_ERRORS = 8;

        private const byte FATAL_1_MASK = 0x01;
        private const byte FATAL_2_MASK = 0x02;
        private const byte FATAL_3_MASK = 0x04;
        private const byte FATAL_4_MASK = 0x08;
        private const byte FATAL_5_MASK = 0x10;
        private const byte FATAL_6_MASK = 0x20;
        private const byte FATAL_7_MASK = 0x40;

        
        #endregion

        #region Definitions

        /// <summary>
        /// Fatal error status bitfield from Gateway table 157
        /// </summary>
        protected enum Fatal_Error_Status : byte
        {
            /// <summary>
            /// If set, a valid core dump exists
            /// </summary>
            FATAL_ERROR_CORE_DUMP_VALID = 0x02,
            /// <summary>
            /// If set, fatal error recovery is enabled
            /// </summary>
            FATAL_RECOVERY_ENABLED = 0x08,
            /// <summary>
            /// If set, meter is in fatal error recovery mode
            /// </summary>
            FATAL_RECOVERY_MODE = 0x10,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public SL7000_Gateway(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                        this.GetType().Assembly);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public SL7000_Gateway(CPSEM PSEM)
            : base(PSEM)
        {

            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                        this.GetType().Assembly);        
        }

        /// <summary>
        /// Resets the SiteScan Diagnostic Counters.
        /// </summary>
        /// <returns>The result of the reset.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#   Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created 
        //
        protected override ItronDeviceResult ResetDiagnosticCounters()
        {
            throw (new NotSupportedException("Reset Diagnostic Counters is not supported"));
        }

        /// <summary>
        /// This method will execute the Connect.
        /// </summary>
        /// <returns>An RemoteConnectResult representing the result of the reset
        /// operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
		// 01/20/11 RCG 2.45.23        Updating to support Connect/Disconnect Procedure Enhancement

        public override RemoteConnectResult RemoteConnect(ConnectType connectType, out ConnectDisconnectResponse responseCode)
        {
            throw (new NotSupportedException("Remote Connect is not supported"));
        }

        /// <summary>
        /// This method executes the remote connect.
        /// </summary>
        /// <param name="connectType">The connection type.</param>
        /// <param name="ignoreLoadVoltage">Whether or not load voltage should be ignored.</param>
        /// <param name="responseCode">The response data for the connect request</param>
        /// <returns>The result of the connect operation.</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
		// 01/20/11 RCG 2.45.23        Updating to support Connect/Disconnect Procedure Enhancement

        public override RemoteConnectResult RemoteConnect(ConnectType connectType, bool ignoreLoadVoltage, out ConnectDisconnectResponse responseCode)
        {
            throw (new NotSupportedException("Remote Connect is not supported"));
        }

        /// <summary>
        /// This method will execute the Disconnect.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
		// 01/20/11 RCG 2.45.23        Updating to support Connect/Disconnect Procedure Enhancement

        public override RemoteConnectResult RemoteDisconnect(out ConnectDisconnectResponse disconnectResponse)
        {
            throw (new NotSupportedException("Remote Disconnect is not supported"));

        } // End RemoteDisconnect()

        /// <summary>
        /// This method will execute the Load Voltage Detection.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the Load Voltage Detection
        /// operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public override LoadVoltageDetectionResult LoadSideVoltageDetection()
        {
            throw (new NotSupportedException("Load Side Voltage Detection is not supported"));

        } // End LoadSideVoltageDetection()

        /// <summary>
        /// Switches the currently active service limiting threshold
        /// </summary>
        /// <param name="threshold">The number of the thershold to switch to.</param>
        /// <param name="thresholdPeriod">The amount of time to stay in this thershold.</param>
        /// <returns>The result of the procedure.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public override ProcedureResultCodes SwitchActiveThreshold(byte threshold, TimeSpan thresholdPeriod)
        {
            throw (new NotSupportedException("Switch Active Threshold is not supported"));
        }

        /// <summary>
        /// Resets the Number of Inversion tampers
        /// </summary>
        /// <returns>ItronDeviceResult.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public override ItronDeviceResult ResetNumberInversionTampers()
        {
            throw (new NotSupportedException("Reset Number of Inversion Tampers is not supported"));
        }

        /// <summary>
        /// Resets the Number of Removal tampers
        /// </summary>
        /// <returns>ItronDeviceResult.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/11/06 KRC 7.35.00 N/A    Created
        // 10/13/06 AF  7.40.00 N/A    Removed flush of m_NumOutages
        //
        public override ItronDeviceResult ResetNumberRemovalTampers()
        {
            throw (new NotSupportedException("Reset Number of Removal Tampers is not supported"));
        }

        /// <summary>
        /// Performs a clock adjust on the connected meter
        /// </summary>
        /// <param name="iOffset">The offset from meter time (seconds)</param>
        /// <returns>A ClockAdjustResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/16/10 AF  2.41.10        Removed the check on clock running.  It
        //                              requires a LID request.
        //  11/03/10 AF  2.45.10        Adjusting the M2 Gateway clock is not supported
        //
        public override ClockAdjustResult AdjustClock(int iOffset)
        {
            throw (new NotSupportedException("Adjust Clock is not supported"));
        }

        /// <summary>
        /// Gets whether or not the meter is Sealed for Canada
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/21/10 AF  2.44.03        Created
        //
        public override bool IsSealedCanadian
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// This method causes a Self Read to occur on a connected ANSI device.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/31/10 AF  2.40.31         Created
        ///
        public override ItronDeviceResult PerformSelfRead()
        {
            throw new NotSupportedException("Perform Self Read is not supported");

        } // End PerformSelfRead()

        /// <summary>
        /// Convert a utc time from the meter to local time for the device
        /// </summary>
        /// <param name="utcTime">UDT time from the meter</param>
        /// <returns>Convertered Device Local Time</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue#         Description
        //  -------- --- ------- -------------  ---------------------------------------
        //  10/26/10 AF  2.45.07                Updated       
        //
        public override DateTime GetLocalDeviceTime(DateTime utcTime)
        {
            DateTime LocalDateTime = utcTime;

            // We can only do these functions in the 3.5 framework, which currently does not work in CE.
#if (!WindowsCE)

            //We do not need to adjust the time for Version 1.5
            LocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, DeviceTimeZoneInfo);
#else
            LocalDateTime = utcTime;
#endif

            return LocalDateTime;
        }

        /// <summary>
        /// This method forces the meter enable or disable servicelimiting by 
        /// entering into the failsafe mode
        /// </summary>
        /// <param name="minsInFailSafe">The number of minutes the meter stays in the failsafe mode.</param>
        /// <returns>The result of the service limiting.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        public override ItronDeviceResult ServiceLimitingFailSafe(UInt16 minsInFailSafe)
        {
            throw new NotSupportedException("Service Limiting and Fail Safe are not supported");
        }

        /// <summary>
        /// This method configures the remote disconnect switch to be enabled or disabled.
        /// </summary>
        /// <param name="SwitchSetting">Whether or not the switch is enabled</param>
        /// <returns>The result of the configuration.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public override ItronDeviceResult ConfigureRemoteDisconnectSwitch(DisconnectSwitchSettings SwitchSetting)
        {
            throw new NotSupportedException("Configure Remote Disconnect Switch is not supported");
        }

        /// <summary>
        /// This method will encrypt the C12.18 passwords stored in the meter.
        /// </summary>
        /// <returns>Results of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //
        public override ProcedureResultCodes EncryptC1218Passwords()
        {
            throw new NotSupportedException("Encrypt C12.18 Passwords is not supported");
        }

        /// <summary>
        /// Changes the LED to pulse the specified quantity.
        /// </summary>
        /// <param name="quantity">The Quantity to change to.</param>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //
        public override ProcedureResultCodes ReconfigureLEDQuantity(LEDQuantityOption quantity)
        {
            throw new NotSupportedException("Reconfigure LED Quantity is not supported");
        }

        /// <summary>
        /// Clears the max readings from table 2263 (Mfg 215)
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //
        public override ProcedureResultCodes ClearTamperTapMaxStats()
        {
            throw new NotSupportedException("Clear Tamper Tap Max Statistics is not supported");
        }

        /// <summary>
        /// Allows L+G security level 5 optical port access to the Focus meter for
        /// a specified period of time
        /// </summary>
        /// <param name="uiMinutes">Number of minutes to allow level 5 access.  0x0000 unlocks
        /// the port until another command is received.  0xffff unlocks the port until 
        /// another command is received</param>
        /// <returns>The result of the procedure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/11 AF  2.50.06        Created
        //
        public ProcedureResultCodes OpticalPortLockout(UInt16 uiMinutes)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[3];

            ProcParam[0] = FOCUS_OPTICAL_PORT_CONTROL;
            byte[] byParameter = BitConverter.GetBytes(uiMinutes);
            Array.Copy(byParameter, 0, ProcParam, 1, 2);

            ProcResult = ExecuteProcedure(Procedures.GATEWAY_PROC_CALL, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Calls standard procedure 2 - save configuration
        /// </summary>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/07/12 AF  2.53.48 174481 We have to call a save config after reconfiguring
        //                              C12.18 over ZigBee
        //
        public ProcedureResultCodes SaveConfiguration()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[0];

            ProcResult = ExecuteProcedure(Procedures.SAVE_CONFIGURATION, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Gets the firmware build
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/15/14 AF  3.50.78 WR489415 Refactored from ANSIDevice.  Needed a method that
        //                              did not make a LID request
        // 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToUInt32(System.String)")]
        public override uint FirmwareBuild
        {
            get
            {
                if (false == m_uiFWBuild.Cached)
                {
                    m_uiFWBuild.Value = Convert.ToUInt32(RegModBuild);
                }

                return m_uiFWBuild.Value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the meter is a Canadian meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //
        public override bool IsCanadian
        {
            get
            {
                //TODO - if this can be supported with a table read, we could implement it
                // but for now this is ok because there are no Canadian M2 meters
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently in Fatal Error Recovery Mode
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //  10/27/10 AF  2.45.10 160590 Fatal Error Recovery now supported
        //  11/12/10 AF  2.45.12 160590 Table 2205 is not there in builds earlier than 24.
        //  11/15/10 AF  2.45.12        fixed globalization warning
        //  12/23/10 AF  2.45.22        Changed according to suggestions in code review
        //  02/04/11 AF  2.50.04        Changed due to redesign of table 2205
        //
        public override bool IsInFatalErrorRecoveryMode
        {
            get
            {
                return ((M2GatewayTable2205.FatalErrorStatus & (byte)Fatal_Error_Status.FATAL_RECOVERY_MODE) == (byte)Fatal_Error_Status.FATAL_RECOVERY_MODE);
            }
        }

        /// <summary>
        /// Gets whether or not the meter currently has a Full Core Dump available
        /// and further core dumps are blocked.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //  10/27/10 AF  2.45.10 160590 Fatal Error Recovery now supported
        //  11/12/10 AF  2.45.12 160590 Table 2205 is not there in builds earlier than 24.
        //  11/15/10 AF  2.45.12        fixed globalization warning
        //  12/23/10 AF  2.45.22        Changed according to suggestions in code review
        //  02/04/11 AF  2.50.04        Modified to support new structure of 2205
        //
        public override bool IsFullCoreDumpBlocked
        {
            get
            {
                return ((M2GatewayTable2205.FatalErrorStatus & (byte)Fatal_Error_Status.FATAL_ERROR_CORE_DUMP_VALID)
                        == (byte)Fatal_Error_Status.FATAL_ERROR_CORE_DUMP_VALID);
            }
        }

        /// <summary>
        /// Gets whether or not Fatal Error Recovery is enabled in the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //  10/27/10 AF  2.45.07 160590 Fatal error recovery is now supported
        //  11/12/10 AF  2.45.12 160590 Table 2205 is not there in builds earlier than 24.
        //  11/15/10 AF  2.45.12        fixed globalization warning
        //  02/03/11 AF  2.50.04        We can now retrieve the data from mfg table 2205
        //
        public override bool IsFatalErrorRecoveryEnabled
        {
            get
            {
                bool bFatalErrorRecoveryEnabled = false;

                if ((M2GatewayTable2108.GatewayModuleVersion == "0.000") 
                    && (Convert.ToByte(M2GatewayTable2108.GatewayModuleBuild, CultureInfo.InvariantCulture) < 25))
                {
                    bFatalErrorRecoveryEnabled = false;
                }
                else
                {
                    if ((M2GatewayTable2205.FatalErrorStatus & (byte)Fatal_Error_Status.FATAL_RECOVERY_ENABLED) == (byte)Fatal_Error_Status.FATAL_RECOVERY_ENABLED)
                    {
                        bFatalErrorRecoveryEnabled = true;
                    }
                }
                return bFatalErrorRecoveryEnabled;
            }
        }

        /// <summary>
        /// Gets the list of fatal errors in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/11/11 AF  2.51.23 174464 Created so that we can show the fatal errors
        //
        public override string[] ErrorsList
        {
            get
            {
                string[] strErrorList = new string[MAX_NUM_ERRORS];
                int iErrorCount = 0;

                if (M2GatewayTable2205.IsFatalErrorPresent)
                {
                    byte byFatalErrors = M2GatewayTable2205.FatalErrors;

                    if ((byte)(byFatalErrors & FATAL_1_MASK) == FATAL_1_MASK)
                    {
                        strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_1");
                    }
                    if ((byte)(byFatalErrors & FATAL_2_MASK) == FATAL_2_MASK)
                    {
                        strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_2");
                    }
                    if ((byte)(byFatalErrors & FATAL_3_MASK) == FATAL_3_MASK)
                    {
                        strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_3");
                    }
                    if ((byte)(byFatalErrors & FATAL_4_MASK) == FATAL_4_MASK)
                    {
                        strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_4");
                    }
                    if ((byte)(byFatalErrors & FATAL_5_MASK) == FATAL_5_MASK)
                    {
                        strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_5");
                    }
                    if ((byte)(byFatalErrors & FATAL_6_MASK) == FATAL_6_MASK)
                    {
                        strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_6");
                    }
                    if ((byte)(byFatalErrors & FATAL_7_MASK) == FATAL_7_MASK)
                    {
                        strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_7");
                    }
                }

                //Create the return list
                string[] strReturnList = new string[iErrorCount];
                if (iErrorCount > 0)
                {
                    Array.Copy(strErrorList, 0, strReturnList, 0, iErrorCount);
                }

                return strReturnList;
            }
        }

        /// <summary>
        /// Gets the last Fatal Error that occurred in the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/11/11 AF  2.50.04        Created  
        //
        public override string LastFatalError
        {
            get
            {
                string strLastError = "Not Available";

                if (LastFatalErrorData != null)
                {
                    FatalErrors LastError = LastFatalErrorData.Error;

                    if ((LastError & FatalErrors.FatalError1) == FatalErrors.FatalError1)
                    {
                        strLastError = m_rmStrings.GetString("FATAL_1");
                    }
                    else if ((LastError & FatalErrors.FatalError2) == FatalErrors.FatalError2)
                    {
                        strLastError = m_rmStrings.GetString("FATAL_2");
                    }
                    else if ((LastError & FatalErrors.FatalError3) == FatalErrors.FatalError3)
                    {
                        strLastError = m_rmStrings.GetString("FATAL_3");
                    }
                    else if ((LastError & FatalErrors.FatalError4) == FatalErrors.FatalError4)
                    {
                        strLastError = m_rmStrings.GetString("FATAL_4");
                    }
                    else if ((LastError & FatalErrors.FatalError5) == FatalErrors.FatalError5)
                    {
                        strLastError = m_rmStrings.GetString("FATAL_5");
                    }
                    else if ((LastError & FatalErrors.FatalError6) == FatalErrors.FatalError6)
                    {
                        strLastError = m_rmStrings.GetString("FATAL_6");
                    }
                    else if ((LastError & FatalErrors.FatalError7) == FatalErrors.FatalError7)
                    {
                        strLastError = m_rmStrings.GetString("FATAL_7");
                    }
                    else if ((LastError & FatalErrors.ErrorPresent) == FatalErrors.ErrorPresent)
                    {
                        // This means a Fatal Error is present but which one is not indicated.
                        // In this case we should translate this to a Fatal 7
                        strLastError = m_rmStrings.GetString("FATAL_7");
                    }
                }

                return strLastError;
            }
        }

        /// <summary>
        /// Gets the reason for the last Fatal Error that occurred in the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/11/11 AF  2.50.04        Created
        //  02/23/11 AF  2.50.05        Added "0x" to reason to make it clear that it's hex
        //
        public override string LastFatalErrorReason
        {
            get
            {
                string strLastErrorReason = "Not Available";

                if (LastFatalErrorData != null)
                {
                    strLastErrorReason = "0x" + LastFatalErrorData.Reason.ToString("X4", CultureInfo.CurrentCulture);
                }

                return strLastErrorReason;
            }
        }

        /// <summary>
        /// Gets whether or not Asset Sync is enabled. Currently
        /// not supported in the Gateway
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/17/10 AF  2.41.10	    Created
        //
        public override bool IsAssetSyncEnabled
        {
            get
            {
                // Not yet supported for the Gateway
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not C12.18 over ZigBee is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/28/10 AF  2.40.44        Added 
        // 05/09/14 jrf 3.50.91 504003 Refactored data retrieval to HANInformation object.
        public override bool IsC1218OverZigBeeEnabled
        {
            get
            {
                return (null != m_HANInfo) && m_HANInfo.IsC1218OverZigBeeEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee Private Profile is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -----------------------------------------------------------------
        // 04/28/10 AF  2.40.44         Added
        // 05/09/14 jrf 3.50.91 504003 Refactored data retrieval to HANInformation object.
        public override bool IsZigBeePrivateProfileEnabled
        {
            get
            {
                return (null != m_HANInfo) && m_HANInfo.IsC1218OverZigBeeEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/28/10 AF  2.40.44         Added
        // 05/09/14 jrf 3.50.91 504003 Refactored data retrieval to HANInformation object.
        public override bool IsZigBeeEnabled
        {
            get
            {
                return (null != m_HANInfo) && m_HANInfo.IsZigBeeEnabled;
            }
        }

        /// <summary>
        /// Gets the current security level
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //
        public override SecurityLevel? CurrentSecurityLevel
        {
            get
            {
                throw new NotSupportedException("Current Security Level is not supported");
            }
        }

        /// <summary>
        /// Gets whether or not the SwapOut table is present in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //
        public override bool IsSwapOutTablePresent
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Property to retrieve the Number of Inversion tampers
        /// </summary>
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public override uint NumberOfInversionTampers
        {
            get
            {
                throw new NotSupportedException("Number of Inversion Tampers is not supported");
            }
        }

        /// <summary>
        /// Property to determine if Magnetic Tampers should be supported. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/18/12 jrf 2.70.18 TQ6657 Created
        //
        public override bool MagneticTampersSupported
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Property to retrieve the Number of detected Magnetic Tampers
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/18/12 jrf 2.70.18 TQ6657 Created
        //
        public override uint NumberOfMagneticTampersDetected
        {
            get
            {
                throw new NotSupportedException("Number of Magnetic Tampers Detected is not supported");
            }
        }

        /// <summary>
        /// Property to retrieve the Number of cleared magnetic tampers.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/18/12 jrf 2.70.18 TQ6657 Created
        //
        public override byte NumberOfMagneticTampersCleared
        {
            get
            {
                throw new NotSupportedException("Number of Magnetic Tampers Cleared is not supported");
            }
        }

        /// <summary>
        /// Property to retrieve the Number of Minutes the meter is on test mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/31/10 AF  2.40.31         Created 
        //
        public override float TimeRemainingInTestMode
        {
            get
            {
                throw new NotSupportedException("Time Remaining in Test Mode is not supported");
            }
        }

        /// <summary>
        /// Property to retrieve the configured/default pulse weight in test mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/31/10 AF  2.40.31         Created
        //  01/19/15 AF  4.00.92 535491 Renamed per code review to make it more clear what this returns
        //
        public override float GetDefaultPulseWeightTest
        {
            get
            {
                throw new NotSupportedException("Default/Configured Pulse Weight in Test Mode is not supported");
            }
        }

        /// <summary>
        /// Property to retrieve the Number of Removal tampers
        /// </summary>
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public override uint NumberOfRemovalTampers
        {
            get
            {
                throw new NotSupportedException("Number of Removal Tampers is not supported");
            }
        }

        /// <summary>
        /// Boolean that indicates if an Itron Communication Module is present in the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created
        //
        public override bool ItronCommModulePresent
        {
            get
            {
                bool bResult = false;

                try
                {
                    bResult = CommModule != null && (CommModule is CiscoCommModule);
                }
                catch
                {
                    bResult = false;
                }

                return bResult;
            }
        }

        /// <summary>
        /// Returns the string that represents the Comm module device class
        /// Allows us to be able to distinguish ITRL devices.  Overridden in
        /// this device class to avoid a bug fix for HW 1.5 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/10 AF  2.41.06        Created 
        //
        public override string CommModuleDeviceClass
        {
            get
            {
                string strDeviceClass = "";

                // MAH - DEBUG
                strDeviceClass = "ITRS";

                //if (Table2064 != null)
                //{
                //    strDeviceClass = Table2064.DeviceClass;
                //}

                return strDeviceClass;
            }
        }

        /// <summary>
        /// Gets the Comm module type (IP or RFLAN)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/02/10 AF  2.41.06			Created
        //
        public override string CommModType
        {
            get
            {
                return M2GatewayTable2108.CommModuleType;
            }
        }

        /// <summary>
        /// Gets the Comm module version.revision
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/02/10 AF  2.41.06			Created
        //
        public override string CommModVer
        {
            get
            {
                return M2GatewayTable2108.CommModuleVersion;
            }
        }

        /// <summary>
        /// Gets the Comm module build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/03/10 AF  2.41.06 		Created
        //
        public override string CommModBuild
        {
            get
            {
                return M2GatewayTable2108.CommModuleBuild;
            }
        }

        /// <summary>
        /// Gets the Han module type (Zigbee)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/03/10 AF  2.41.06 		Created
        //
        public override string HanModType
        {
            get
            {
                return M2GatewayTable2108.HanModuleType;
            }
        }

        /// <summary>
        /// Gets the Han module version.revision
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/03/10 AF  2.41.06			Created
        //
        public override string HanModVer
        {
            get
            {
                return M2GatewayTable2108.HanModuleVersion;
            }
        }

        /// <summary>
        /// Gets the HAN module version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/10 AF  2.41.06        Created
        //
        public override float HanModFirmwareVersion
        {
            get
            {
                return M2GatewayTable2108.HANVersionOnly + M2GatewayTable2108.HANRevisionOnly / 1000.0f;
            }
        }

        /// <summary>
        /// Gets the HAN module build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/10 AF  2.41.06        Created
        //
        public override byte HANModFirmwareBuild
        {
            get
            {
                return M2GatewayTable2108.HANBuildOnly;
            }
        }

        /// <summary>
        /// Gets the Han module build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/03/10 AF  2.41.06			Created
        //
        public override string HanModBuild
        {
            get
            {
                return M2GatewayTable2108.HanModuleBuild;
            }
        }

        /// <summary>
        /// Gets the L+G register version from Mfg Table 2108
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public string LGRegModVer
        {
            get
            {
                return M2GatewayTable2108.M2ModuleVersion;
            }
        }

        /// <summary>
        /// Gets the L+G register build from Mfg Table 2108
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public string LGRegModBuild
        {
            get
            {
                return M2GatewayTable2108.M2ModuleBuild;
            }
        }

        /// <summary>
        /// L+G Firmware Engineering Version 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/08/11 AF  2.51.08 174361 Created
        //
        public string LGRegEngVersion
        {
            get
            {
                return M2GatewayTable2108.M2ModuleEngVer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string RegModVer
        {
            get
            {
                return M2GatewayTable2108.GatewayModuleVersion;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string RegModBuild
        {
            get
            {
                return M2GatewayTable2108.GatewayModuleBuild;
            }
        }

        /// <summary>
        /// Gets the dst enabled flag
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 	04/01/10 AF  2.40.31        Created
        //  07/02/10 AF  2.42.01        Modified to get the flag from standard table 52
        // 		
        public override bool DSTEnabled
        {
            get
            {
                //return M2GatewayTable2048.DSTEnabled;
                return (bool)IsDSTApplied;
            }
        }

        /// <summary>
        /// Returns the number of Last Demand Resets in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/01/10 AF  2.40.31        Created
        // 		
        public override uint NumberofLastDemandResets
        {
            get
            {
                throw new NotSupportedException("Number of Last Demand Resets is not supported");
            }
        }

        /// <summary>
        /// Gets the Date Programmed out of 2048
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue#     Description
        //  -------- --- ------- ---------------------------------------------
        //  07/01/10 AF  2.42.01            Created
        // 		
        public override DateTime DateProgrammed
        {
            get
            {
                return M2GatewayTable2048.DateProgrammed;
            }
        }

        /// <summary>
        /// Retrieves the instantaneous secondary Volts RMS Phase A from the meter.
        /// The firmware folks say this should be considered to be the service voltage.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override float ServiceVoltage
        {
            get
            {
                throw new NotSupportedException("Number of Last Demand Resets is not supported");
            }
        }

        /// <summary>
        /// Property used to get the human readable meter name 
        /// (string).  Use this property when 
        /// displaying the name of the meter to the user.  
        /// This should not be confused with the MeterType 
        /// which is used for meter determination and comparison.
        /// </summary>
        /// <returns>A string representing the human readable name of the 
        /// meter.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override string MeterName
        {
            get
            {
                return M2GATEWAY_NAME;
            }
        }

        /// <summary>
        /// Gets the meter name that will be used in the activity log.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created

        public override string ActivityLogMeterName
        {
            get
            {
                return LOG_ITR1_NAME;
            }
        }

        /// <summary>
        /// Builds the list of Event descriptions and returns the dictionary 
        /// </summary>
        /// <returns>
        /// Dictionary of Event Descriptions
        /// </returns> 
        //  Revision History	
        //  MM/DD/YY who Version Issue#    Description
        //  -------- --- ------- ------    ---------------------------------------
        //  09/29/10 AF  2.44.07 162279	   Created
        //  08/05/16 MP  4.70.11 WR674048  Set timeformat when making new dictionary
        public override ANSIEventDictionary EventDescriptions
        {
            get
            {
                if (null == m_dicEventDescriptions)
                {
                    m_dicEventDescriptions = new M2_Gateway_EventDictionary()
                    {
                        TimeFormat = Table00.TimeFormat
                    };
                }
                return (ANSIEventDictionary)m_dicEventDescriptions;
            }
        }

        /// <summary>
        /// Gets the Disconnect Status
        /// </summary>
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/01/10 AF  2.40.31        Created
        // 		
        public override bool MeterKey_DisconnectAvailable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the Pole Top Cell Relay meter key bit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override bool MeterKey_PoleTopCellRelaySupported
        {
            get
            {
                throw new NotSupportedException("Pole Top Cell relay Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Returns whether or not Enhanced Blurts are supported.
        /// </summary> 
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  
        public override bool MeterKey_EnhancedBlurtsSupported
        {
            get
            {
                throw new NotSupportedException("Enhanced Blurt Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Returns whether or not the meter uses the SR1.0 Device Class.
        /// </summary> 
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  
        public override bool MeterKey_UseSR1DeviceClass
        {
            get
            {
                throw new NotSupportedException("SR1.0 Device Class Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Returns whether or not the meter enables ZigBee Debugging.
        /// </summary> 
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  
        public override bool MeterKey_ZigBeeDebug
        {
            get
            {
                throw new NotSupportedException("ZigBee Debug Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Returns whether or not the meter is a transparent device.
        /// </summary>
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created 
        //
        public override bool MeterKey_TransparentDeviceSupported
        {
            get
            {
                throw new NotSupportedException("Transparent Device Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Returns whether or not the meter will Disable Core Dump.
        /// </summary> 
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  
        public override bool MeterKey_DisableCoreDump
        {
            get
            {
                throw new NotSupportedException("Disable Core Dump Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Returns whether or not the meter will Disable Core Dump on Total Stack Use Limit.
        /// </summary> 
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  
        public override bool MeterKey_DisableCoreDumpOnTotalStackUseLimit
        {
            get
            {
                throw new NotSupportedException("Disable Core Dump on Total Stack Use Limit Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Returns whether or not the meter is an Advanced Poly.
        /// </summary> 
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  
        public override bool MeterKey_AdvancedPolySupported
        {
            get
            {
                throw new NotSupportedException("Advanced Poly Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Returns an OpenWay device type string based on the meter key settings.
        /// </summary> 
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  
        public override string MeterKey_QuantityConfiguration
        {
            get
            {
                throw new NotSupportedException("Quantity Configuration Meter Key Bit is not supported");
            }
        }

        /// <summary>
        /// Determines if User Intervention is required after a load limiting disconnect
        /// </summary>
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/01/10 AF  2.40.31        Created
        // 		
        public override string LoadLimitingConnectWithoutUserIntervetion
        {
            get
            {
                throw new NotSupportedException("Load Limiting Connect Without User Intervention is not supported");
            }
        }

        /// <summary>
        /// Determines if Load Control is enabled and what the Threshold is if it is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/01/10 AF  2.40.31        Created
        // 	
        public override string LoadControlDisconnectThreshold
        {
            get
            {
                return "Not Enabled";
            }
        }

        /// <summary>
        /// Gets the configured daily self read time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 04/01/10 AF  2.40.31        Created

        public override string DailySelfReadTime
        {
            get
            {
                return "Not Supported";
            }
        }

        /// <summary>
        /// Gets the Neutral Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity AmpsNeutral
        {
            get
            {
                throw new NotSupportedException("Neutral Amps is not supported");
            }
        }

        /// <summary>
        /// Gets the Phase A Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity AmpsPhaseA
        {
            get
            {
                throw new NotSupportedException("Amps Phase A is not supported");
            }
        }

        /// <summary>
        /// Gets the Phase B Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity AmpsPhaseB
        {
            get
            {
                throw new NotSupportedException("Amps Phase B is not supported");
            }
        }

        /// <summary>
        /// Gets the Phase C Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity AmpsPhaseC
        {
            get
            {
                throw new NotSupportedException("Amps Phase C is not supported");
            }
        }

        /// <summary>
        /// Gets the Amps squared from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity AmpsSquared
        {
            get
            {
                throw new NotSupportedException("Amps Squared is not supported");
            }
        }

        /// <summary>
        /// Gets the Power Factor from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity PowerFactor
        {
            get
            {
                throw new NotSupportedException("Power Factor is not supported");
            }
        }

        /// <summary>
        /// Gets the Q Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity QDelivered
        {
            get
            {
                throw new NotSupportedException("Q Delivered is not supported");
            }
        }

        /// <summary>
        /// Gets the Qh Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity QReceived
        {
            get
            {
                throw new NotSupportedException("Q Received is not supported");
            }
        }

        /// <summary>
        /// Gets the VA Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VADelivered
        {
            get
            {
                throw new NotSupportedException("VA Delivered is not supported");
            }
        }

        /// <summary>
        /// Gets the Lagging VA from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VALagging
        {
            get
            {
                throw new NotSupportedException("VA Lagging is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarDelivered
        {
            get
            {
                throw new NotSupportedException("Var Delivered is not supported");
            }
        }

        /// <summary>
        /// Gets the VA Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VAReceived
        {
            get
            {
                throw new NotSupportedException("VA Received is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Net from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarNet
        {
            get
            {
                throw new NotSupportedException("Var Net is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Net delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarNetDelivered
        {
            get
            {
                throw new NotSupportedException("Var Net Delivered is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Net Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarNetReceived
        {
            get
            {
                throw new NotSupportedException("Var Net Received is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Q1 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarQuadrant1
        {
            get
            {
                throw new NotSupportedException("Var Quadrant 1 is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Q2 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarQuadrant2
        {
            get
            {
                throw new NotSupportedException("Var Quadrant 2 is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Q3 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarQuadrant3
        {
            get
            {
                throw new NotSupportedException("Var Quadrant 3 is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Q4 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarQuadrant4
        {
            get
            {
                throw new NotSupportedException("Var Quadrant 4 is not supported");
            }
        }

        /// <summary>
        /// Gets the Var Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VarReceived
        {
            get
            {
                throw new NotSupportedException("Var Received is not supported");
            }
        }

        /// <summary>
        /// Gets the Average Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VoltsAverage
        {
            get
            {
                throw new NotSupportedException("Volts Average is not supported");
            }
        }

        /// <summary>
        /// Gets the Phase A Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VoltsPhaseA
        {
            get
            {
                throw new NotSupportedException("Volts (a) is not supported");
            }
        }

        /// <summary>
        /// Gets the Phase B Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VoltsPhaseB
        {
            get
            {
                throw new NotSupportedException("Volts (b) is not supported");
            }
        }

        /// <summary>
        /// Gets the Phase C Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VoltsPhaseC
        {
            get
            {
                throw new NotSupportedException("Volts (c) is not supported");
            }
        }

        /// <summary>
        /// Gets the Volts squared from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity VoltsSquared
        {
            get
            {
                throw new NotSupportedException("Volts Squared is not supported");
            }
        }

        /// <summary>
        /// Gets the Watts Delivered quantity from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity WattsDelivered
        {
            get
            {
                throw new NotSupportedException("Watts Delivered is not supported");
            }
        }

        /// <summary>
        /// Gets the Watts Revieved quantity from the standard tables
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity WattsReceived
        {
            get
            {
                throw new NotSupportedException("Watts Received is not supported");
            }
        }

        /// <summary>
        /// Gets the Watts Net quantity from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity WattsNet
        {
            get
            {
                throw new NotSupportedException("Watts Net is not supported");
            }
        }

        /// <summary>
        /// Gets the Unidirectional Watts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override Quantity WattsUni
        {
            get
            {
                throw new NotSupportedException("Unidirectional Watts is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase A
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override float InsCurrentPhaseA
        {
            get
            {
                throw new NotSupportedException("Instantaneous Current Phase A is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase B
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override float InsCurrentPhaseB
        {
            get
            {
                throw new NotSupportedException("Instantaneous Current Phase B is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase C
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override float InsCurrentPhaseC
        {
            get
            {
                throw new NotSupportedException("Instantaneous Current Phase C is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous Voltage for Phase A
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override float InsVoltagePhaseA
        {
            get
            {
                throw new NotSupportedException("Instantaneous Voltage Phase A is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous Voltage for Phase B
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override float InsVoltagePhaseB
        {
            get
            {
                throw new NotSupportedException("Instantaneous Voltage Phase B is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous Voltage for Phase C
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override float InsVoltagePhaseC
        {
            get
            {
                throw new NotSupportedException("Instantaneous Voltage Phase C is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous Watts
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override float InsW
        {
            get
            {
                throw new NotSupportedException("Instantaneous Watts is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous VA Arith
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Arith")]
        public override float InsVAArith
        {
            get
            {
                throw new NotSupportedException("Instantaneous VA Arith is not supported");
            }
        }

        /// <summary>
        /// Gets the Instantaneous VA Vect
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Vect")]
        public override float InsVAVect
        {
            get
            {
                throw new NotSupportedException("Instantaneous VA Vect is not supported");
            }
        }

        /// <summary>
        /// Proves access to a list of Self Read Collections
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override List<QuantityCollection> SelfReadRegisters
        {
            get
            {
                throw new NotSupportedException("Self Read Registers are not supported");

            }
        }

        /// <summary>
        /// Property to determine the Display Mode
        /// </summary>
        /// <exception>
        /// Throws: NotSupportedException
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/01/10 AF  2.40.31        Created
        // 
        public override DisplayMode MeterDisplayMode
        {
            get
            {
                throw new NotSupportedException("Meter Display Mode is not supported");
            }
        }

        /// <summary>
        /// Gets the Firmware Loader Version number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override byte FWLoaderVersion
        {
            get
            {
                throw new NotSupportedException("Firmware Loader Version is not supported");
            }
        }

        /// <summary>
        /// Gets the Firmware Loader Revision number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override byte FWLoaderRevision
        {
            get
            {
                throw new NotSupportedException("Firmware Loader Revision is not supported");
            }
        }

        /// <summary>
        /// Gets the Firmware Loader Build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override byte FWLoaderBuild
        {
            get
            {
                throw new NotSupportedException("Firmware Loader Build is not supported");
            }
        }

        /// <summary>
        /// An event is triggered if the acceleration value along the X axis of the accelerometer
        /// is greater than or equal to this threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override byte InversionThreshold
        {
            get
            {
                throw new NotSupportedException("Inversion Threshold is not supported");
            }
        }

        /// <summary>
        /// An event is triggered if average acceleration value on X and Z axes of the accelerometer
        /// is above this threshold.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override byte RemovalThreshold
        {
            get
            {
                throw new NotSupportedException("Removal Threshold is not supported");
            }
        }

        /// <summary>
        /// A tap is detected if average acceleration value on the Y axis of the accelerometer
        /// is above this threshold and falls below it within 400 milliseconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override byte TapThreshold
        {
            get
            {
                throw new NotSupportedException("Tap Threshold is not supported");
            }
        }

        /// <summary>
        ///  The number of seconds to pull data from the accelerometer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override UInt32 WakeupDuration
        {
            get
            {
                throw new NotSupportedException("Wakeup Duration is not supported");
            }
        }

        /// <summary>
        /// If true, there are errors in accelerometer configuration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override bool AccelerometerConfigError
        {
            get
            {
                throw new NotSupportedException("Accelerometer Configuration Error is not supported");
            }
        }

        /// <summary>
        ///  If false, tap and tamper detections are not running
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override bool WakeUpStatus
        {
            get
            {
                throw new NotSupportedException("Wakeup Status is not supported");
            }
        }

        /// <summary>
        /// If true, a removal tamper has been detected and the meter is checking 
        /// the power down for 10 seconds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override bool RemovalPDNCheck
        {
            get
            {
                throw new NotSupportedException("Removal PDN Check is not supported");
            }
        }

        /// <summary>
        /// Checks the tap check field of table 2262 to see if a tap has been detected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override bool TapDetected
        {
            get
            {
                throw new NotSupportedException("Tap Detected is not supported");
            }
        }

        /// <summary>
        /// Whether or not the accelerometer is supported 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override bool IsAccelerometerSupported
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The reference angle of installation of the X axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Acc")]
        public override float AccReferenceAngleX
        {
            get
            {
                throw new NotSupportedException("Acc Reference Angle X is not supported");
            }
        }

        /// <summary>
        /// The reference angle of installation of the Y axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Acc")]
        public override float AccReferenceAngleY
        {
            get
            {
                throw new NotSupportedException("Acc Reference Angle Y is not supported");
            }
        }

        /// <summary>
        /// The reference angle of installation of the Z axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Acc")]
        public override float AccReferenceAngleZ
        {
            get
            {
                throw new NotSupportedException("Acc Reference Angle Z is not supported");
            }
        }

        /// <summary>
        /// Current angle of installation of the X axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Acc")]
        public override float AccCurrentAngleX
        {
            get
            {
                throw new NotSupportedException("Acc Current Angle X is not supported");
            }
        }

        /// <summary>
        /// Current angle of installation of the Y axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Acc")]
        public override float AccCurrentAngleY
        {
            get
            {
                throw new NotSupportedException("Acc Current Angle Y is not supported");
            }
        }

        /// <summary>
        /// Current angle of installation of the Z axis. Note that the scalar
        /// and divisor from table 2262 have been applied.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Acc")]
        public override float AccCurrentAngleZ
        {
            get
            {
                throw new NotSupportedException("Acc Current Angle Z is not supported");
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along X axis since powerup
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override sbyte MaxDeltaX
        {
            get
            {
                throw new NotSupportedException("Max Delta X is not supported");
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along Y axis since powerup
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override sbyte MaxDeltaY
        {
            get
            {
                throw new NotSupportedException("Max Delta Y is not supported");
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along Z axis since powerup
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override sbyte MaxDeltaZ
        {
            get
            {
                throw new NotSupportedException("Max Delta Z is not supported");
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override sbyte MaxAvgDeltaTap
        {
            get
            {
                throw new NotSupportedException("Max Average Delta Tap is not supported");
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //
        public override sbyte MaxAvgDeltaTamper
        {
            get
            {
                throw new NotSupportedException("Max Average Delta Tamper is not supported");
            }
        }

        ///<summary>
        /// Gets the number of firmware blocks currently downloaded from the C1222 debug table.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override ushort C1222DebugFWDLBlockCount
        {
            get
            {
                throw new NotSupportedException("Firmware Download Block Count is not supported");
            }
        }

        /// <summary>
        /// Gets the number total number of firmware blocks to download.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override int TotalFWDLBlockCount
        {
            get
            {
                throw new NotSupportedException("Total Firmware Download Block Count is not supported");
            }
        }

        /// <summary>
        /// Gets whether or not activation is occurring now.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override bool IsActivateInProgress
        {
            get
            {
                throw new NotSupportedException("Activation in Progress is not supported");
            }
        }

        /// <summary>
        /// Gets whether or not firmware download is enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        public override bool IsFWDLEnabled
        {
            get
            {
                throw new NotSupportedException("Firmware Download Enabled is not supported");
            }
        }

        /// <summary>
        /// Gets Vender Field 1 Display data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override string DisplayVenderField1
        {
            get
            {
                throw new NotSupportedException("Comm Module Level is not supported");
            }
        }

        /// <summary>
        /// Gets Vender Field 2 Display data
        /// </summary> 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override string DisplayVenderField2
        {
            get
            {
                throw new NotSupportedException("Comm Module Sync Status is not supported");
            }
        }

        /// <summary>
        /// Gets Vender Field 3 Display data
        /// </summary> 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override string DisplayVenderField3
        {
            get
            {
                //TODO - read "is registered" in Mfg table 26
                throw new NotSupportedException("Comm Module Registration Status is not supported");
            }
        }

        /// <summary>
        /// Determines whether or not the meter is currently in test mode.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/16/10 AF  2.40.38        Created.
        //
        public override bool IsInTestMode
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if IO is supported, false if not.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 04/16/10 AF 2.40.38 N/A    Created
        ///
        public override bool IOSupported
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// We don't have a way of determining whether or not the Gateway 
        /// supports or is using TOU so just return false.
        /// </summary>
        /// 
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/19/10 AF  2.40.38        Created
        // 		
        public override bool TOUEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get the software version from the meter
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //   Revision History	
        //   MM/DD/YY who Version Issue# Description
        //   -------- --- ------- ------ ---------------------------------------
        //   05/27/10 AF  2.41.06        Created
        //
        public override string SWRevision
        {
            get
            {
                return Table06.SWVersion;
            }
        }

        /// <summary>
        /// Indicates whether or not the meter is currently recording
        /// load profile data.  We can't determine this with the Gateway
        /// so return false.
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  06/08/10 AF  2.41.08        Created
        /// </remarks>
        /// 
        public override bool LPRunning
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the meter supports the 25 year DST Calendar
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public override bool Supports25YearDST
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the firmware revision. This is the same as "FWRevision" but was created for the 
        /// purpose of using as a parameter to pass in on table creation. Some Tables are different
        /// sizes in different firmware versions. This should only be used as a parameter to pass
        /// in when creating a table. This allows us to override only a property and not the
        /// creation of a table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/16/13 DLG 3.50.16          Created.
        //  
        public override float FWRevisionForTableCreation
        {
            get
            {
                return CENTRON_AMI.VERSION_3;
            }
        }

        /// <summary>
        /// Gets the hardware revision. This is the same as "HWRevision" but was created for the 
        /// purpose of using as a parameter to pass in on table creation. Some Tables are different
        /// sizes in different hardware versions. This should only be used as a parameter to pass
        /// in when creating a table. Reference table 2128 for example.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/16/13 DLG 3.50.16          Created.
        //  
        public override float HWRevisionForTableCreation
        {
            get
            {
                return CENTRON_AMI.HW_VERSION_3_0;
            }
        }

        #endregion

        #region Static Public (Translation Method)

#if (!WindowsCE)
        

        /// <summary>
        /// Validates the DES Keys.
        /// </summary>
        /// <param name="ProgName">Name of the program</param>
        /// <param name="KeyID">Key ID: Key1, Key2...</param>
        /// <returns>Procedure Result code</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override ProcedureResultCodes ValidateDESKeys(string ProgName, DESKeys KeyID)
        {
            throw new NotSupportedException("Validate DES Keys is not supported");
        }

        /// <summary>
        /// Validates the Enhanced Security keys in the meter to those in the specified file.
        /// </summary>
        /// <param name="strEnhancedSecurityKeyFile">The file that contains the keys to validate against.</param>
        /// <param name="Wait">Whether or not to send a wait to the device before calling 
        /// method to validate enhanced security keys.</param>
        /// <returns>list of bool items</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ------------------------------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  10/11/16 jrf 4.60.11 722267 Adding wait in case call to validate enhanced security keys hangs while 
        //                              attempting to access key file.
        public override bool ValidateEnhancedSecurityKeys(string strEnhancedSecurityKeyFile, bool Wait = false)
        {
            bool bAreKeysValid = true;

            // Pull the keys out of Security File and validate them
            for (int iKey = 0; iKey < 6; iKey++)
            {
                //This was added to attempt to prevent factory QC Tool issues with meter timing out.
                if (true == Wait)
                {
                    SendWait();
                }

                // Validate the key
                if (ValidateEnhancedSecurityKey(strEnhancedSecurityKeyFile, (M2_Gateway.EnhancedKeys)((byte)(iKey + 1))) != ProcedureResultCodes.COMPLETED)
                {
                    bAreKeysValid = false;
                    break;
                }
            }

            return bAreKeysValid;
        }

#endif

        #endregion

        #region Internal Methods

        /// <summary>
        /// Causes Fatal Error on an OpenWay CENTRON meter.
        /// </summary>
        /// <returns>
        /// An ProcedureResultCodes representing the result of the operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 03/31/10 AF  2.40.31         Created 
        //
        public override ProcedureResultCodes CauseFatalError()
        {
            throw new NotSupportedException("Cause Fatal Error is not supported");
        }

        /// <summary>
        /// Creates a new DisplayItem
        /// </summary>
        /// <returns>ANSIDisplayItem</returns>
        // Revision History	
        // MM/DD/YY who Version  Issue# Description
        // -------- --- -------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        internal override ANSIDisplayItem CreateDisplayItem(LID lid, string strDisplayID, ushort usFormat, byte byDim)
        {
            throw new NotSupportedException("Create Display Item is not supported");
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the current security level from the meter.
        /// </summary>
        /// <param name="level">The current security level</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/01/10 AF  2.40.31        Created

        protected override ProcedureResultCodes GetSecurityLevel(out SecurityLevel level)
        {
            throw new NotSupportedException("Get Security Level is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Neutral Amps
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRAmpsNeutral(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Amps Neutral is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Amps (a)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRAmpsPhaseA(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Amps Phase A is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Amps (b)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRAmpsPhaseB(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Amps Phase B is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Amps (c)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRAmpsPhaseC(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Amps Phase C is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Amps Squared
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRAmpsSquared(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Amps Squared is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Power Factor
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRPowerFactor(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Power Factor is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Q Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRQDelivered(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Amps Neutral is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Q Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRQReceived(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Q Received is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for VA Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVADelivered(uint uiIndex)
        {
            throw new NotSupportedException("Self Read VA Delivered is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for VA Lagging
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVALagging(uint uiIndex)
        {
            throw new NotSupportedException("Self Read VA Lagging is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarDelivered(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Delivered is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for VA Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVAReceived(uint uiIndex)
        {
            throw new NotSupportedException("Self Read VA Received is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Net.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarNet(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Net is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Net Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarNetDelivered(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Net Delivered is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Net Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarNetReceived(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Net Received is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Q1.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarQuadrant1(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Quadrant 1 is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Q2.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarQuadrant2(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Quadrant 2 is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Q3.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarQuadrant3(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Quadrant 3 is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Q4.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarQuadrant4(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Quadrant 4 is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Var Received.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVarReceived(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Var Received is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Volts Average.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVoltsAverage(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Volts Average is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Volts (a).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVoltsPhaseA(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Volts Phase A is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Volts(b).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVoltsPhaseB(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Volts Phase B is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Volts (c).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVoltsPhaseC(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Volts Phase C is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Volts Squared.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRVoltsSquared(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Volts Squared is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Watts Delivered.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRWattsDelivered(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Watts Delivered is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Watts Net.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRWattsNet(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Watts Net is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Watts Received.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRWattsReceived(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Watts Received is not supported");
        }

        /// <summary>
        /// Gets the Self Read values for Unidirectional Watts.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity SRWattsUni(uint uiIndex)
        {
            throw new NotSupportedException("Self Read Watts Unidirectional is not supported");
        }

        /// <summary>
        /// Gets the Self Read date for the specified index.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The date of the Self Read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override DateTime DateTimeOfSelfRead(uint uiIndex)
        {
            throw new NotSupportedException("Date/Time of Self Read is not supported");
        }

        /// <summary>
        /// Gets the Demand Reset date for the specified index.
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The date of the demand reset.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override DateTime DateTimeOfDemandReset(uint uiIndex)
        {
            throw new NotSupportedException("Date/Time of Demand Reset is not supported");
        }

        /// <summary>
        /// Gets the last demand reset date.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        public override DateTime DateLastDemandReset
        {
            get
            {
                throw new NotSupportedException("Date/Time of Last Demand Reset is not supported");
            }
        }

        /// <summary>
        /// Gets the demand reset value for VA d
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity DRVADelivered(uint uiIndex)
        {
            throw new NotSupportedException("Demand Reset VA Delivered is not supported");
        }

        /// <summary>
        /// Gets the demand reset value for VA r
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity DRVAReceived(uint uiIndex)
        {
            throw new NotSupportedException("Demand Reset VA Received is not supported");
        }

        /// <summary>
        /// Gets the demand reset value for Volts (a)
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity DRVoltsPhaseA(uint uiIndex)
        {
            throw new NotSupportedException("Demand Reset Volts Phase A is not supported");
        }

        /// <summary>
        /// Gets the demand reset value for Wh d
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity DRWattsDelivered(uint uiIndex)
        {
            throw new NotSupportedException("Demand Reset Watts Delivered is not supported");

        }

        /// <summary>
        /// Gets the demand reset value for Wh Net
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity DRWattsNet(uint uiIndex)
        {
            throw new NotSupportedException("Demand Reset Watts Net is not supported");
        }

        /// <summary>
        /// Gets the demand reset value for Wh r
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created

        protected override Quantity DRWattsReceived(uint uiIndex)
        {
            throw new NotSupportedException("Demand Reset Watts Received is not supported");
        }

        /// <summary>
        /// Gets the demand reset value for Wh
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        protected override Quantity DRWattsUni(uint uiIndex)
        {
            throw new NotSupportedException("Demand Reset Watts Unidirectional is not supported");
        }

        /// <summary>
        /// Reads the Firmware Loader Version from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/10 AF  2.40.31        Created
        // 

        protected override ProcedureResultCodes ReadFWLoaderVersion()
        {
            throw new NotSupportedException("Read of FW Loader Version is not supported");
        }

        /// <summary>
        /// Writes the Header information to the Core Dump file
        /// </summary>
        /// <param name="writer">The Binary writer to the Core Dump file</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  10/26/10 AF  2.45.07 160590 Added implementation
        //
        protected override void WriteCoreDumpHeader(PSEMBinaryWriter writer)
        {
            CallingAppInfo AppInfo = new CallingAppInfo();
            string strAppID = AppInfo.ProductName + " ver. " + AppInfo.Version;
            uint uiRFLANMAC = 0;
            byte byRFLANFWLoaderRevision = 0;
            byte byRFLANFWLoaderBuild = 0;

            if (writer != null)
            {
                writer.Seek(0, SeekOrigin.Begin);

                // Write the header information
                writer.Write("OpenWay CoreDump Version 1.0", 60);

                // Write the Register Information
                writer.Write(true); // Have Version Information
                writer.Write(M2GatewayTable2108.M2GVersionOnly); // FW Version
                writer.Write(M2GatewayTable2108.M2GRevisionOnly); // FW Revision
                writer.Write(M2GatewayTable2108.M2GBuildOnly); // FW Build
                writer.Write(Table01.HWVersionOnly); // HW Version
                writer.Write(Table01.HWRevisionOnly); // HW Revision
                writer.Write(DeviceClass, 4); // Device Class

                // This seems to be what is always written for this.
                writer.Write((byte)3); // Core Dump Type - PSEM Tool Core Dump

                writer.Write(VersionChecker.CompareTo(HWRevision, HW_VERSION_1_0) >= 0); // Is 512k

                try
                {
                    if (CommModule != null)
                    {
                        uiRFLANMAC = CommModule.MACAddress;
                    }
                }
                catch (Exception)
                {
                    // We failed to get the MAC Address so just put 0 in as a placeholder
                }

                writer.Write(uiRFLANMAC); // RFLAN MAC Address
                writer.Write(MFGSerialNumber, 16); // MFG Serial Number
                writer.Write(M2GatewayTable2108.CommModuleTypeByte); // Comm Module Type
                writer.Write(M2GatewayTable2108.CommVersionOnly); // Comm Module FW Version
                writer.Write(M2GatewayTable2108.CommRevisionOnly); // Comm Module FW Revision
                writer.Write(M2GatewayTable2108.CommBuildOnly); // Comm Module FW Build
                writer.Write(M2GatewayTable2108.HANModuleTypeByte); // HAN Module Type
                writer.Write(M2GatewayTable2108.HANVersionOnly); // HAN FW Version
                writer.Write(M2GatewayTable2108.HANRevisionOnly); // HAN FW Revision
                writer.Write(M2GatewayTable2108.HANBuildOnly); // HAN FW Build
                writer.Write(M2GatewayTable2108.M2ModuleVersionOnly); // L+G FW Version
                writer.Write(M2GatewayTable2108.M2ModuleRevisionOnly); // L+G FW Revision
                writer.Write(M2GatewayTable2108.M2ModuleBuildOnly); // L+G FW Build

                //writer.Write(FWLoaderVersion); // Loader Version
                //writer.Write(FWLoaderRevision); // Loader Revision
                //writer.Write(FWLoaderBuild); // Loader Build
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);

                writer.Write((byte)0); // RFLAN Loader Version - Always 0

                writer.Write(byRFLANFWLoaderRevision); // RFLAN Loader Revision
                writer.Write(byRFLANFWLoaderBuild); // RFLAN Loader Build

                // Write the application ID
                if (strAppID.Length > 40)
                {
                    // Cut off the end of the name if it's too long
                    strAppID = strAppID.Substring(0, 40);
                }

                writer.Write(strAppID);

                // The Header is 256 bytes so seek to the start of the data 
                writer.Seek(CORE_DUMP_HEADER_SIZE, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Writes the tables portion of the Core Dump for all meter types.
        /// </summary>
        /// <param name="writer">The Binary Writer for the Core Dump file.</param>
        /// <returns>The result of the operation</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/01/10 AF  2.40.31        Created
        //  10/26/10 AF  2.45.07 160590 Added implementation
        //  02/03/11 AF  2.50.01        Changed implementation after redesign by
        //                              the firmware team
        //
        protected override ItronDeviceResult WriteCoreDumpTables(PSEMBinaryWriter writer)
        {
            byte[] CoreDumpData;
            uint uiOffset = 0;
            uint uiCoreDumpSize = DetermineFullCoreDumpLength();
            PSEMResponse Response = PSEMResponse.Ok;
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            // Seek in the file to make sure the Full Core Dump starts in the right place
            writer.Seek(FULL_CORE_DUMP_OFFSET, SeekOrigin.Begin);

            OnStepProgress(new ProgressEventArgs());

            // We don't want to check the response here
            while (uiOffset < uiCoreDumpSize && Response == PSEMResponse.Ok)
            {
                if (uiOffset + CORE_DUMP_BLOCK_SIZE < uiCoreDumpSize)
                {
                    Response = m_PSEM.OffsetRead(3043, (int)uiOffset, CORE_DUMP_BLOCK_SIZE, out CoreDumpData);
                    uiOffset += CORE_DUMP_BLOCK_SIZE;
                }
                else
                {
                    // There is less than a whole block remaining
                    Response = m_PSEM.OffsetRead(3043, (int)uiOffset, (ushort)(uiCoreDumpSize - uiOffset), out CoreDumpData);
                    uiOffset = uiCoreDumpSize;
                }

                if (Response == PSEMResponse.Ok)
                {
                    writer.Write(CoreDumpData);
                }

                OnStepProgress(new ProgressEventArgs());
            }

            if (Response == PSEMResponse.Isc)
            {
                Result = ItronDeviceResult.SECURITY_ERROR;
            }
            else if (Response != PSEMResponse.Ok)
            {
                Result = ItronDeviceResult.ERROR;
            }

            return Result;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the M2GatewayTable2048 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/28/10 AF  2.41.06        Created
        //
        internal M2GatewayTable2048 M2GatewayTable2048
        {
            get
            {
                if (null == m_M2GatewayTable2048)
                {
                    m_M2GatewayTable2048 = new M2GatewayTable2048(m_PSEM);
                }

                return m_M2GatewayTable2048;
            }
        }

        /// <summary>
        /// Gets the M2GatewayTable2108 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/02/10 AF  2.41.06        Created
        //
        internal M2GatewayTable2108 M2GatewayTable2108
        {
            get
            {
                if (null == m_M2GatewayTable2108)
                {
                    m_M2GatewayTable2108 = new M2GatewayTable2108(m_PSEM);
                }

                return m_M2GatewayTable2108;
            }
        }

        /// <summary>
        /// Gets the M2GatewayTable2205 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/10 AF  2.45.06 160590 Created
        //  12/23/10 AF  2.45.22        Added parameters to table 2205 constructor to
        //                              implement changes suggested in code review
        //  04/26/11 AF  2.50.34 171415 Used the M2GatewayTable2108 property to be sure it has
        //                              been read.
        //  03/02/12 AF  2.53.47 192934 Removed firmware version parameters - no longer needed
        //
        internal M2GatewayTable2205 M2GatewayTable2205
        {
            get
            {
                if (null == m_M2GatewayTable2205)
                {
                    m_M2GatewayTable2205 = new M2GatewayTable2205(m_PSEM);
                }

                return m_M2GatewayTable2205;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the meter type "M2GATEWAY"
        /// </summary>		
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/01/10 AF  2.40.31        Created
        //
        protected override string DefaultMeterType
        {
            get
            {
                return M2GATEWAY;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the Enhanced Security Keys sub table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/29/10 AF  2.40.45        Created

        private MFGTable2127EnhancedSecurityKeys Table2127EnhancedSecurityKeys
        {
            get
            {
                throw new NotSupportedException("Table 2127 is not supported");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method will Load the program file and extract Security Provider, Exception Security Model, DESKey Length in order to decide 
        /// whether to Validate DES keys or not
        /// </summary>
        /// <param name="ProgName">
        /// Program Name
        ///</param>
        /// <param name="DESKeyID">
        /// DESKeyID
        ///</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 
        //
        private bool RequiresDESKeysValidation(string ProgName, M2_Gateway.SecurityKeyID DESKeyID)
        {
            return false;
        }

        #endregion

        # region Private Static Methods

        /// <summary>
        /// This method will Load the program file and extract optical and DES keys from it
        /// </summary>
        /// <param name="ProgName">
        /// Program Name
        ///</param>
        /// <param name="SecType">
        /// Type of Security (i.e Optical Passwords or DES Keys
        /// </param>
        /// <param name="PasswordLevel">
        /// KeyID of Security
        /// </param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/21/09 MMD  2.30.01       Created
        //
        private static byte[] GetSecurityCode(string ProgName, M2_Gateway.SecurityType SecType, M2_Gateway.SecurityKeyID PasswordLevel)
        {
            object objValue = null;
            try
            {
                //Load the program file in xmlReader

                XmlReader xmlReader;
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.IgnoreWhitespace = true;
                settings.IgnoreComments = true;
                settings.CheckCharacters = false;
                xmlReader = XmlReader.Create(ProgName, settings);

                //Instantiate Centron Tables in order to extract passwords from tables 

                CentronTables objCenTables = new CentronTables();
                objCenTables.LoadEDLFile(xmlReader);
                xmlReader.Close();

                int iIndex=0;

                // if Security Type is DES extract DES keys from table 46 or else 
                // extract optical passwords form table 42

                if (SecType == M2_Gateway.SecurityType.C1222_KEYS)
                {
                    iIndex = Convert.ToInt32(((byte)PasswordLevel) - 1);
                    byte length;
                    objCenTables.GetValue(StdTableEnum.STDTBL46_KEY_LENGTH, new int[] { iIndex }, out objValue);
                    if (objValue != null)
                    {
                        length = Convert.ToByte(objValue, CultureInfo.CurrentCulture);
                        objCenTables.GetValue(StdTableEnum.STDTBL46_KEY, new int[] { iIndex }, out objValue);
                        byte[] filevalue = objValue as byte[];
                        if ((filevalue != null) && (length != 0 && length != 255))
                        {
                            //byte[] realValue = new byte[length];
                            //Array.Copy(filevalue, realValue, length);
                            objValue = filevalue;
                        }
                        else
                        {
                            byte[] realValue = new byte[filevalue.Length];
                            objValue = realValue;

                        }
                    }

                }
                else if (SecType == M2_Gateway.SecurityType.C1218_PASSWORDS)
                {
                    iIndex = Convert.ToInt32(4 - ((byte)PasswordLevel));
                    objCenTables.GetValue(StdTableEnum.STDTBL42_PASSWORD, new int[] { iIndex }, out objValue);
                }
                else
                {
                    throw (new Exception("The requested Security code is not the expected Security Type"));
                }

                return ((byte[])objValue);
            }
            catch (Exception)
            {

                return ((byte[])objValue);
            }

        }

        #endregion

        #region Members

        private M2GatewayTable2048 m_M2GatewayTable2048;
        private M2GatewayTable2108 m_M2GatewayTable2108;
        private M2GatewayTable2205 m_M2GatewayTable2205;

        #endregion
    }

}

