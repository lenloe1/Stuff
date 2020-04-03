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
//                           Copyright © 2010 - 2012
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
using Itron.Metering.AMIConfiguration;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.DST;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Response parameter for the connect/disconnect procedure
    /// </summary>
    public enum ConnectDisconnectResponse : byte
    {
        /// <summary>
        /// Connect/Disconnect Successful
        /// </summary>
        [EnumDescription("Successful.")]
        Successful = 0,
        /// <summary>
        /// Failed because the switch was already closed
        /// </summary>
        [EnumDescription("Meter is already connected.")]
        SwitchAlreadyClosed = 1,
        /// <summary>
        /// Failed because the switch was already open
        /// </summary>
        [EnumDescription("Meter is already disconnected.")]
        SwitchAlreadyOpened = 2,
        /// <summary>
        /// Failed because the switch is currently disabled
        /// </summary>
        [EnumDescription("Remote Connect/Disconnect Switch is disabled.")]
        SwitchDisabled = 3,
        /// <summary>
        /// Failed because of insufficient security
        /// </summary>
        [EnumDescription("Insufficient Security.")]
        InsufficientSecurity = 4,
        /// <summary>
        /// Failed because the procedure data was invalid
        /// </summary>
        [EnumDescription("Invalid Procedure Data.")]
        InvalidProcedureData = 5,
        /// <summary>
        /// Failed because the connect/disconnect message was not point to point
        /// </summary>
        [EnumDescription("Procedure may only be performed point to point.")]
        NotPointToPointMessage = 6,
        /// <summary>
        /// Failed because the metrology is not ready
        /// </summary>
        [EnumDescription("Meter is not ready.")]
        NotReady = 7,
        /// <summary>
        /// Failed because switch operation is being overridden by Service Limiting
        /// </summary>
        [EnumDescription("Service Limiting is overriding Connect/Disconnect functionality.")]
        ServiceLimitingOverride = 8,
        /// <summary>
        /// Failed because the capacitor is not charged
        /// </summary>
        [EnumDescription("Capacitor is not charged.")]
        CapacitorNotCharged = 9,
        /// <summary>
        /// Failed because no load side voltage was present after a connect
        /// </summary>
        [EnumDescription("Load Side Voltage not detected.")]
        NoLoadSideVoltagePresentAfterConnect = 10,
        /// <summary>
        /// Failed because load side voltage was detected after a disconnect
        /// </summary>
        [EnumDescription("Load Side Voltage detected.")]
        LoadSideVoltagePresentAfterDisconnect = 11,
        /// <summary>
        /// Failed because there is no switch in the meter
        /// </summary>
        [EnumDescription("Not Supported.")]
        NoSwitchPresent = 12,
        /// <summary>
        /// Successful but waiting on user intervention to connect
        /// </summary>
        [EnumDescription("Successful. Press the meter connection button to complete the connection.")]
        SuccessfulInterventionRequired = 13,
    }

    /// <summary>
    /// Enumeration for Remote Connection Types
    /// </summary>
    public enum ConnectType
    {
        /// <summary>
        /// Do a Connect that does not require user intervention
        /// </summary>
        CONNECT_NO_USER_INTERVENTION = 0,
        /// <summary>
        /// Do a Connect that requires user intervention
        /// </summary>
        CONNECT_USER_INTERVENTION = 1,
    }

    /// <summary>
    /// Enumeration for the Load Side Voltage Detection Result Codes
    /// </summary>
    public enum LoadVoltageDetectionResult
    {
        /// <summary>
        /// No Load Voltage Present
        /// </summary>
        LOAD_SIDE_VOLTAGE_NOT_PRESENT = 0,
        /// <summary>
        /// Load Side Voltage Present
        /// </summary>
        LOAD_SIDE_VOLTAGE_PRESENT = 1,
    }

    /// <summary>
    /// Enumeration for the settings that can be configured for the 
    /// disconnect switch.
    /// </summary>
    public enum DisconnectSwitchSettings
    {
        /// <summary>
        /// The switch is disabled.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// The switch is enabled.
        /// </summary>
        Enabled = 1,
    }

    /// <summary>
    /// Enumeration for the Remote Connection Result Codes
    /// </summary>
    public enum RemoteConnectResult
    {
        /// <summary>
        /// Remote Connect/Disconnect Succeeded
        /// </summary>
        REMOTE_ACTION_SUCCESS = 0,
        /// <summary>
        /// Remote Connect/Disconnect Failed due to Security Error
        /// </summary>
        SECURITY_VIOLATION = 1,
        /// <summary>
        /// Load Voltage was detected, so operation failed.
        /// </summary>
        LOAD_VOLTAGE_PRESENT = 3,
        /// <summary>
        /// The Connect/Disconnect operation failed, but we don't have an exact reason
        /// </summary>
        REMOTE_CONNECT_FAILED = 4,
        /// <summary>
        /// Load Voltage was not detected after a connect, which could mean that the switch failed.  The base will be put
        /// back into the disconnect state.
        /// </summary>
        LOAD_VOLTAGE_NOT_DETECTED = 5,
        /// <summary>
        /// Remote Connect/Disconnect is not supported on this device
        /// </summary>
        UNRECOGNIZED_PROCEDURE = 6,
    }


    /// <summary>
    /// Class representing the REGISTER_BASE.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 09/23/10 MMD				    Created
    public partial class REGISTER_BASE : CANSIDevice
    {

        #region Definitions

        /// <summary>
        /// Perform a self read, if possible, before pending table is activated
        /// </summary>
        private const byte SELF_READ_FLAG = 1 << 4;
        /// <summary>
        /// Perform a demand reset, if capable, before pending table is activated
        /// </summary>
        private const byte DEMAND_RESET_FLAG = 1 << 5;
        /// <summary>
        /// Table identifiers in the range of 6144 to 8183 provide access to 
        /// Manufacturer-Pending tables 0 to 2039.  RegisterFWTbl = 2109 is 
        /// actually table 2109 + 4096 or 6205
        /// </summary>
        protected const ushort PENDING_BIT = 4096;
        /// <summary>
        /// Number of Last Demand Resets in the meter
        /// </summary>
        private const uint NUM_LAST_DEMAND_RESETS = 2;
        /// <summary>
        /// Length of the response for MFG Procedure 46
        /// </summary>
        protected const int MFG_PROC_46_RESPONSE_LENGTH = 42;
        /// <summary>
        /// Length in bytes of the activation trigger for pending tables
        /// </summary>
        public const int ACTIVATION_TRIGGER_LEN = 6;
        /// <summary>
        /// Constant describing the firmware version for SR 3.0
        /// </summary>
        public const float VERSION_3 = 3.000F;
        /// <summary>
        /// Constant describing the firmware version for SR 3.0 HW 3.0 release
        /// </summary>
        public const float VERSION_3_1 = 3.001F;
        /// <summary>
        /// Constant describing the firmware version for Hydrogen/Helium 3.006
        /// </summary>
        public const float VERSION_HYDROGEN_3_6 = 3.006F;
        /// <summary>
        /// Constant describing the firmware version for Hydrogen/Helium 3.007
        /// </summary>
        public const float VERSION_HYDROGEN_3_7 = 3.007F;
        /// <summary>
        /// Constant describing the firmware version for Hydrogen/Helium 3.008 (HW 3.X release)
        /// </summary>
        public const float VERSION_HYDROGEN_3_8 = 3.008F;
        /// <summary>
        /// Constant describing the firmware version for Hydrogen C, 3.010
        /// </summary>
        public const float VERSION_HYDROGEN_3_10 = 3.010F;
        /// <summary>
        /// Constant describing the firmware version for Lithium, 3.011
        /// </summary>
        public const float VERSION_LITHIUM_3_11 = 3.011F;
        /// <summary>
        /// Constant describing the firmware version for Lithium, 3.012
        /// </summary>
        public const float VERSION_LITHIUM_3_12 = 3.012F;
        /// <summary>
        /// Constant describing the firmware version for Lithium+, 3.014
        /// </summary>
        public const float VERSION_LITHIUM_PLUS_3_14 = 3.014F;
        /// <summary>
        /// Constant describing the firmware version for Boron, 5.000
        /// </summary>
        public const float VERSION_BORON_5_0 = 5.000F;
        /// <summary>
        /// Constant describing the firmware version for Boron, 5.002
        /// </summary>
        public const float VERSION_BORON_5_2 = 5.002F;
        /// <summary>
        /// Constant describing the firmware version for M2 Gateway Lithium, 2.000
        /// </summary>
        public const float VERSION_M2GTWY_2_0 = 2.000F;
        /// <summary>
        /// The Set Time Date Mask for setting the clock
        /// </summary>
        protected const byte AMI_SET_TIME_DATE_MASK = (byte)(SET_MASK_BFLD.SET_DATE_FLAG | SET_MASK_BFLD.SET_TIME_FLAG
                                                    | SET_MASK_BFLD.SET_TIME_DATE_QUAL);
        /// <summary>
        /// Function code for the Cause Fatal error procedure
        /// </summary>
        private const byte CAUSE_FATAL_FUNC_CODE = 1;
        /// <summary>
        /// Parameter 1 for the Cause Fatal error procedure
        /// </summary>
        private const uint CAUSE_FATAL_PARAM_1 = 56;
        /// <summary>
        /// Parameter 2 for the Cause Fatal error procedure
        /// </summary>
        private const uint CAUSE_FATAL_PARAM_2 = 0;
        /// <summary>
        /// Parameter 1 for the Cause Fatal error 2 procedure
        /// </summary>
        private const uint CAUSE_FATAL2_PARAM_1 = 57;
        /// <summary>
        /// Parameter 2 for the Cause Fatal error 2 procedure
        /// </summary>
        private const uint CAUSE_FATAL2_PARAM_2 = 2;
        /// <summary>
        /// Parameter 1 for the Cause Fatal error 2 procedure
        /// </summary>
        private const uint CAUSE_FATAL6_PARAM_1 = 57;
        /// <summary>
        /// Parameter 2 for the Cause Fatal error 2 procedure
        /// </summary>
        private const uint CAUSE_FATAL6_PARAM_2 = 32;
        /// <summary>
        /// Memory offset for the FW Loader version in MAXIMA meters
        /// </summary>
        protected const uint FW_LOADER_MAXIMA_OFFSET = 0x001000B4;
        /// <summary>
        /// The size of the block to read.dee
        /// </summary>
        protected const ushort CORE_DUMP_BLOCK_SIZE = 1024;
        /// <summary>
        /// The size of the Core Dump Header
        /// </summary>
        protected const int CORE_DUMP_HEADER_SIZE = 256;
        /// <summary>
        /// The Offset in the Core Dump file to write the full core dump
        /// </summary>
        protected const int FULL_CORE_DUMP_OFFSET = CORE_DUMP_HEADER_SIZE + 256;
        /// <summary>
        /// Manufacturer
        /// </summary>
        private const string MANUFACTURER = "ITRN";
        /// <summary>
        /// MFG defined event number.  This can be anything between 1 and 254.
        /// It will be used to identify pending table 2109 for activation and/or
        /// clearing.
        /// </summary>
        protected const byte REGISTER_EVENT_NUMBER = 1;
        /// <summary>
        /// MFG defined event number.  This can be anything between 1 and 254.
        /// It will be used to identify pending table 2110 for activation and/or
        /// clearing.
        /// </summary>
        protected const byte ZIGBEE_EVENT_NUMBER = 2;
        /// <summary>
        /// MFG defined event number.  This can be anything between 1 and 254.
        /// It will be used to identify pending table 2111 for activation and/or
        /// clearing.
        /// </summary>
        protected const byte COMM_EVENT_NUMBER = 3;
        /// <summary>
        /// MFG defined event number.  This can be anything between 1 and 254.
        /// It will be used to identify pending table 2181 for activation and/or
        /// clearing.
        /// </summary>
        protected const byte HAN_DEV_EVENT_NUMBER = 4;
        /// <summary>
        /// Size of chunks to break f/w download file into
        /// </summary>
        protected const ushort BLOCK_SIZE = 128;    // Changed from 14247
        /// <summary>
        /// Length of the header in the firmware file
        /// </summary>
        protected const int FW_HEADER_LENGTH = 19;
        /// <summary>
        /// Number of bytes in the parameter list for MFG procedure 37
        /// </summary>
        protected const int INIT_FW_DOWNLOAD_PARAM_LEN = 17;
        /// <summary>
        /// Number of bytes in the parameter list for MFG procedure 37 when writing Third Party HAN FW
        /// </summary>
        protected const int INIT_FW_DOWNLOAD_THIRD_PARTY_LEN = 21;
        /// <summary>
        /// Size of field for firmware file size in parameter list for mfg 
        /// procedure 37
        /// </summary>
        protected const int IMAGE_SIZE_FIELD_LEN = 4;
        /// <summary>
        /// Size of field for size of blocks in parameter list for mfg 
        /// procedure 37
        /// </summary>
        protected const int CHUNK_SIZE_FIELD_LEN = 2;
        /// <summary>
        /// Size of the Device Class field in the parameter list for MFG Proc 37
        /// </summary>
        protected const int DEVICE_CLASS_LENGTH = 4;
        /// <summary>
        /// Constant Describing the Hardware Version for 3.0 meters
        /// </summary>
        public const float HW_VERSION_3_0 = 3.000F;
        /// <summary>
        /// Function code for the set time on battery procedure
        /// </summary>
        public const byte TIME_ON_BATT_FUNC_CODE = 166;
        /// <summary>
        /// The list of the TOU Rate modifiers for LIDs
        /// </summary>
        private readonly uint[] TOU_RATES = {(uint)DefinedLIDs.TOU_Data.RATE_A, (uint)DefinedLIDs.TOU_Data.RATE_B, 
											 (uint)DefinedLIDs.TOU_Data.RATE_C, (uint)DefinedLIDs.TOU_Data.RATE_D, 
											 (uint)DefinedLIDs.TOU_Data.RATE_E, (uint)DefinedLIDs.TOU_Data.RATE_F,
											 (uint)DefinedLIDs.TOU_Data.RATE_G };

        /// <summary>
        /// Enumeration for CPP (Critical Peak Pricing)
        /// </summary>
        public enum ConfigCppResult
        {
            /// <summary>
            /// Ok
            /// </summary>
            ConfiguredOk = 0,
            /// <summary>
            /// Invalid Start Time
            /// </summary>
            InvalidStartTime = 1,
            /// <summary>
            /// Ok
            /// </summary>
            ClearedOk = 2,
            /// <summary>
            /// Is Active
            /// </summary>
            CppIsActive = 3,
            /// <summary>
            /// Error 
            /// </summary>
            Error = 4
        }

        /// <summary>
        ///Response Codes for Remote Connect/Disconnect enhancement
        /// </summary>
        public enum RemoteConnectDisconnectResponseCode
        {
            /// <summary>
            /// Ok
            /// </summary>
            Success = 0,
            /// <summary>
            /// Switch is Already Close
            /// </summary>
            SwitchClose = 1,
            /// <summary>
            /// Switch is Already Open
            /// </summary>
            SwitchOpen = 2,
            /// <summary>
            /// Meter Key for Remote Connect/Disconnect disabled
            /// </summary>
            MeterKeyDisabled = 3,
            /// <summary>
            /// Failed Due to Security
            /// </summary>
            InsufficientSecurity = 4,
            /// <summary>
            /// Invalid Data 
            /// </summary>
            InvalidData = 5,
            /// <summary>
            /// Not Point-to-Point
            /// </summary>
            NotPointtoPoint = 6,
            /// <summary>
            /// Metrology Not Ready
            /// </summary>
            MetrologyNotReady = 7,
            /// <summary>
            /// ServiceLimiting override
            /// </summary>
            ServiceLimitingOverriden = 8,
            /// <summary>
            /// RCD Cap not charged
            /// </summary>
            RCDCapNotCharged = 9,
            /// <summary>
            /// LoadSide Voltage Not Present
            /// </summary>
            LoadSideBVoltageNotPresent = 10,
            /// <summary>
            /// LoadSide Voltage  Present
            /// </summary>
            LoadSideBVoltagePresent = 11,
            /// <summary>
            /// Switch Not Present
            /// </summary>
            SwitchNotPresent = 12,
            /// <summary>
            /// SUCCESS Connect User Intervention
            /// </summary>
            SuccessConnectUserIntevention = 13

        }
        /// <summary>
        /// Enumeration of the types of security keys that can be validated
        /// </summary>
        public enum SecurityType : byte
        {
            /// <summary>
            /// Enhanced Security public keys
            /// </summary>
            ENHANCED_SECURITY = 1,
            /// <summary>
            /// C1218 passwords
            /// </summary>
            C1218_PASSWORDS = 2,
            /// <summary>
            /// C1222 DES/3DES keys
            /// </summary>
            C1222_KEYS = 3,
            /// <summary>
            /// ZigBee keys
            /// </summary>
            HAN_KEYS = 4,
        }

        /// <summary>
        /// Identifier for the specific security key to be validated
        /// </summary>
        public enum SecurityKeyID : byte
        {
            /// <summary>
            /// Command Key 1 / C1218 Primary PWD / C1222 Key 1 / HAN Network Key
            /// </summary>
            KEYID1 = 1,
            /// <summary>
            /// Command Key 2 / C1218 Secondary PWD / C1222 Key 2 / HAN Global Link Key
            /// </summary>
            KEYID2 = 2,
            /// <summary>
            /// Command Key 3 / C1218 Tertiary PWD / C1222 Key 3
            /// </summary>
            KEYID3 = 3,
            /// <summary>
            /// Command Key 4 / C1218 Quaternary PWD / C1222 Key 4
            /// </summary>
            KEYID4 = 4,
            /// <summary>
            /// Revocation Key 1
            /// </summary>
            KEYID5 = 5,
            /// <summary>
            /// Revocation Key 2
            /// </summary>
            KEYID6 = 6,
        }


        /// <summary>
        /// The options for MFG Procedure 91 that are used for changing Fatal Error Recovery
        /// </summary>
        public enum FatalRecoveryOption : byte
        {
            /// <summary>
            /// Disable Fatal Error Recovery
            /// </summary>
            Disable = 0,
            /// <summary>
            /// Enable Fatal Error Recovery
            /// </summary>
            Enable = 1,
            /// <summary>
            /// Clear the Fatal Error Recovery Mode status bit in Table 3
            /// </summary>
            ClearRecoveryMode = 2,
            /// <summary>
            /// Clear the Fatal Error Core Dump status bit in Table 3
            /// </summary>
            ClearCoreDump = 3,
        }

        /// <summary>
        /// Identifier for the Optical level to be validated
        /// </summary>
        public enum OpticalPasswords : byte
        {
            /// <summary>
            /// C1218 Primary PWD 
            /// </summary>
            Primary = 1,
            /// <summary>
            /// C1218 Secondary PWD 
            /// </summary>
            Secondary = 2,
            /// <summary>
            /// C1218 Tertiary PWD 
            /// </summary>
            Tertiary = 3,
            /// <summary>
            /// C1218 Quaternary PWD 
            /// </summary>
            Quaternary = 4,
        }

        /// <summary>
        /// The bits indicating which item to reset via MFG procedure 6
        /// </summary>
        private enum AMI_Reset_Counter_Types
        {
            /// <summary>
            /// Reset # times programmed
            /// </summary>
            RESET_NUM_TIMES_PROGRAMMED = 0x00000001,
            /// <summary>
            /// Reset # demand resets
            /// </summary>
            RESET_NUM_DEMAND_RESETS = 0x00000002,
            /// <summary>
            /// Reset # power outages
            /// </summary>
            RESET_NUM_POWER_OUTAGES = 0x00000004,
            /// <summary>
            /// Reset Inversion tampers
            /// </summary>
            RESET_NUM_INVERSION_TAMPERS = 0x00000008,
            /// <summary>
            /// Reset Removal Tampers
            /// </summary>
            RESET_NUM_REMOVAL_TAMPERS = 0x00000010,
            /// <summary>
            /// Reset Reverse Rotation Tampers
            /// </summary>
            RESET_REVERSE_ROTATION_TAMPERS = 0x00000020,
            /// <summary>
            /// Reset SiteScan Diagnostics
            /// </summary>
            RESET_SITESCAN_DIAG = 0x00000040,
        }

        /// <summary>
        /// MFG procedure 60 options that are used for selecting which quantity to pulse on the LED
        /// </summary>
        public enum LEDQuantityOption : byte
        {
            /// <summary>
            /// Wh
            /// </summary>
            [EnumDescription("Wh d")]
            Wh = 0x00,
            /// <summary>
            /// VAh
            /// </summary>
            [EnumDescription("VAh d")]
            VAh = 0x01,
            /// <summary>
            /// Varh
            /// </summary>
            [EnumDescription("varh d")]
            Varh = 0x02,
        }

        // Load Control Reconnect
        private const int LOAD_CONTROL_RECONNECT = 0x80;
        /// <summary>
        /// Constant describing the length of the optical passwords
        /// </summary>
        public const int OPTICAL_PASSWORD_LEN = 20;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/23/10 MMD				    Created
        //
        public REGISTER_BASE(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
            //Use the Centron LIDs
            m_LID = new CentronMonoDefinedLIDs();
            m_DSTConfigured = new CachedBool();
            m_fltFWLoaderVerRev = new CachedFloat();
            m_FWLoaderVersion = new CachedByte();
            m_FWLoaderRevision = new CachedByte();
            m_FWLoaderBuild = new CachedByte();
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                        this.GetType().Assembly);
            m_lstDSTDates = new List<CDSTDatePair>();
            m_TOUSchedule = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/23/10 MMD				    Created
        //
        public REGISTER_BASE(CPSEM PSEM)
            : base(PSEM)
        {
            //Use the Centron LIDs
            m_LID = new CentronMonoDefinedLIDs();
            m_DSTConfigured = new CachedBool();
            m_fltFWLoaderVerRev = new CachedFloat();
            m_FWLoaderVersion = new CachedByte();
            m_FWLoaderRevision = new CachedByte();
            m_FWLoaderBuild = new CachedByte();
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                        this.GetType().Assembly);
        }

        /// <summary>
        /// This method causes a Self Read to occur on a connected ANSI device.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/25/08 KRC 2.00.00  N/A    Created
        /// 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        ///
        public virtual ItronDeviceResult PerformSelfRead()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)RemoteResetProcedureFlags.SELF_READ;
            ProcResult = ExecuteProcedure(Procedures.REMOTE_RESET,
                ProcParam, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    //Success
                    Result = ItronDeviceResult.SUCCESS;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    //Isc error
                    Result = ItronDeviceResult.SECURITY_ERROR;
                    break;
                }
                default:
                {
                    //General Error
                    Result = ItronDeviceResult.ERROR;
                    break;
                }
            }

            return Result;

        } // End PerformSelfRead()

#if (!WindowsCE)
        /// <summary>
        /// Validates the Optical Passwords in the meter 
        /// </summary>
        /// <param name="ProgName">Program Name</param>
        /// <returns>list of bool items</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ------------------------------------------------------------
        //  11/20/09 MMD          QC Tool    Created

        public bool ValidateOpticalPasswords(string ProgName)
        {
            bool bKeysValid = true;
            for (int iKey = 0; iKey < 4; iKey++)
            {
                // Validate the key
                if (ValidateOpticalPasswords(ProgName, (CENTRON_AMI.OpticalPasswords)((byte)(iKey + 1))) != ProcedureResultCodes.COMPLETED)
                {
                    bKeysValid = false;
                    break;
                }
            }
            return bKeysValid;
        }

        /// <summary>
        /// Validates the optical passwords.
        /// </summary>
        /// <param name="ProgName">Name of the program</param>
        /// <param name="PasswordLevel">Level of Password ex: Primary, Secondary...</param>
        /// <returns>Procedure Result code</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/21/09 MMD  2.30.01        Use the Mfg procedure for validating passwords

        public ProcedureResultCodes ValidateOpticalPasswords(string ProgName, OpticalPasswords PasswordLevel)
        {

            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            byte[] byData = CENTRON_AMI.GetSecurityCode(ProgName, SecurityType.C1218_PASSWORDS, (SecurityKeyID)PasswordLevel);
            string strPassword = "";
            for (int i = 0; i < byData.Length; i++)
            {
                strPassword += ((char)byData[i]).ToString();
            }

            byte[] hashedCode = CreateHash(strPassword, OPTICAL_PASSWORD_LEN);

            ProcResult = ValidateSecurityKey(SecurityType.C1218_PASSWORDS, (SecurityKeyID)PasswordLevel, hashedCode);


            return ProcResult;
        }
#endif

        /// <summary>
        /// Resets the Number of Inversion tampers
        /// </summary>
        /// <returns>ItronDeviceResult.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/11/06 KRC 7.35.00 N/A    Created
        // 10/13/06 AF  7.40.00 N/A    Removed flush of m_NumOutages
        // 07/02/10 AF  2.42.04        Made virtual for M2 Gateway override
        //
        public virtual ItronDeviceResult ResetNumberInversionTampers()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;


            //Execute the reset counters MFG procedure 
            byte[] byParameter = BitConverter.GetBytes((uint)AMI_Reset_Counter_Types.RESET_NUM_INVERSION_TAMPERS);
            ProcResult = ExecuteProcedure(Procedures.RESET_COUNTERS, byParameter, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    //Success
                    Result = ItronDeviceResult.SUCCESS;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    //Isc error
                    Result = ItronDeviceResult.SECURITY_ERROR;
                    break;
                }
                default:
                {
                    //General Error
                    Result = ItronDeviceResult.ERROR;
                    break;
                }
            }


            return Result;
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
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        //
        public virtual ItronDeviceResult ResetNumberRemovalTampers()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;


            //Execute the reset counters MFG procedure 
            byte[] byParameter = BitConverter.GetBytes((uint)AMI_Reset_Counter_Types.RESET_NUM_REMOVAL_TAMPERS);
            ProcResult = ExecuteProcedure(Procedures.RESET_COUNTERS, byParameter, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    //Success
                    Result = ItronDeviceResult.SUCCESS;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    //Isc error
                    Result = ItronDeviceResult.SECURITY_ERROR;
                    break;
                }
                default:
                {
                    //General Error
                    Result = ItronDeviceResult.ERROR;
                    break;
                }
            }


            return Result;
        }

        /// <summary>
        /// Performs a clock adjust on the connected meter
        /// </summary>
        /// <param name="iOffset">The offset from meter time (seconds)</param>
        /// <returns>A ClockAdjustResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/06/06 RCG         N/A    Created

        public override ClockAdjustResult AdjustClock(int iOffset)
        {
            ClockAdjustResult Result = ClockAdjustResult.SUCCESS;
            DateTime MeterTime;
            DateTime ReferenceDate = new DateTime(1970, 1, 1);
            CDSTDatePair DSTDate = null;
            TimeSpan Span = new TimeSpan();
            uint MeterMinutes;
            byte[] byMinutes;
            byte MeterSeconds;
            byte[] byParameters = new byte[SET_TIME_DATE_PROC_SIZE];
            int iIndex = 0;
            int iDSTIndex = 0;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;


            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting Adjust Clock");

                if (ClockRunning)
                {
                    //The clock is running so do the clock adjust.  Setup the 
                    //parameters for Procedure 10

                    if (iOffset > MAX_TIME_ADJUST_SECONDS)
                    {
                        //We can only adjust up to 24 hours
                        iOffset = MAX_TIME_ADJUST_SECONDS;
                    }
                    else if (iOffset < (-1 * MAX_TIME_ADJUST_SECONDS))
                    {
                        //We can only adjust down to 24 hours
                        iOffset = (-1 * MAX_TIME_ADJUST_SECONDS);
                    }

                    //Determine the new time for the meter
                    MeterTime = DeviceTime;
                    MeterTime = MeterTime.AddSeconds(iOffset);


                    //Convert the time to minutes and seconds since 1/1/1970					
                    Span = MeterTime - ReferenceDate;
                    MeterMinutes = (uint)Span.TotalMinutes;
                    MeterSeconds = (byte)Span.Seconds;

                    //The first byte is the Set_Mask (time and date)
                    byParameters[iIndex++] = AMI_SET_TIME_DATE_MASK;

                    //Minutes since 1/1/1970
                    byMinutes = BitConverter.GetBytes(MeterMinutes);
                    Array.Copy(byMinutes, 0, byParameters, iIndex, byMinutes.Length);
                    iIndex += byMinutes.Length;

                    //Add the seconds
                    byParameters[iIndex++] = MeterSeconds;

                    //Set up the time date qual bitfield
                    TIME_DATE_QUAL_BFLD TimeDateQual = 0x00;

                    if (Table52.IsTimeZoneApplied == true)
                    {
                        TimeDateQual |= TIME_DATE_QUAL_BFLD.TM_ZN_APPLIED_FLAG;
                    }

                    if (Table52.IsDSTApplied == true)
                    {
                        TimeDateQual |= TIME_DATE_QUAL_BFLD.DST_APPLIED_FLAG;

                        // Find the DST dates for this year
                        while (iDSTIndex < DST.Count && DST[iDSTIndex].ToDate.Year != MeterTime.Year)
                        {
                            iDSTIndex++;
                        }

                        if (iDSTIndex < DST.Count)
                        {
                            DSTDate = DST[iDSTIndex];
                        }

                        // Determine if the new time is in DST
                        if (DSTDate != null)
                        {
                            if (IsMeterInDST == true && (MeterTime <= DSTDate.FromDate && MeterTime >= DSTDate.ToDate))
                            {
                                // The meter is still in DST
                                TimeDateQual |= TIME_DATE_QUAL_BFLD.DST_FLAG;
                            }
                            else if (IsMeterInDST == true || (IsMeterInDST == false && (MeterTime <= DSTDate.FromDate && MeterTime >= DSTDate.ToDate)))
                            {
                                // Trying to adjust over a DST change
                                Result = ClockAdjustResult.ERROR_NO_ADJUST_OVER_DST;
                            }
                        }
                        else
                        {
                            // We don't have a DST date for the current year so assume the time
                            // will be the same
                            if (IsMeterInDST == true)
                            {
                                TimeDateQual |= TIME_DATE_QUAL_BFLD.DST_FLAG;
                            }
                        }
                    }

                    if (Table52.IsGMT == true)
                    {
                        TimeDateQual |= TIME_DATE_QUAL_BFLD.GMT_FLAG;
                    }

                    TimeDateQual |= (TIME_DATE_QUAL_BFLD)((byte)MeterTime.DayOfWeek & (byte)TIME_DATE_QUAL_BFLD.DAY_OF_WEEK);

                    byParameters[iIndex] = (byte)TimeDateQual;

                    if (Result == ClockAdjustResult.SUCCESS || Result == ClockAdjustResult.SUCCESS_24_HOUR_MAXIMUM_ADJUST)
                    {
                        //Execute the set date time procedure
                        ProcResult = ExecuteProcedure(Procedures.SET_DATE_TIME,
                                                       byParameters,
                                                       out ProcResponse);

                        switch (ProcResult)
                        {
                            case ProcedureResultCodes.COMPLETED:
                            {
                                if (MAX_TIME_ADJUST_SECONDS == iOffset ||
                                    (MAX_TIME_ADJUST_SECONDS * -1) == iOffset)
                                {
                                    Result = ClockAdjustResult.SUCCESS_24_HOUR_MAXIMUM_ADJUST;
                                }
                                else
                                {
                                    Result = ClockAdjustResult.SUCCESS;
                                }
                                break;
                            }
                            case ProcedureResultCodes.NO_AUTHORIZATION:
                            {
                                Result = ClockAdjustResult.SECURITY_ERROR;
                                break;
                            }
                            case ProcedureResultCodes.UNRECOGNIZED_PROC:
                            {
                                Result = ClockAdjustResult.UNSUPPORTED_OPERATION;
                                break;
                            }
                            default:
                            {
                                Result = ClockAdjustResult.ERROR;
                                break;
                            }
                        }

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                           "Set date time Result = " + ProcResult);
                    }
                }
                else
                {
                    //The clock is not running
                    Result = ClockAdjustResult.ERROR_CLOCK_NOT_RUNNING;
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return Result;
        }

        /// <summary>
        /// This method will encrypt the C12.18 passwords stored in the meter.
        /// </summary>
        /// <returns>Results of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/20/09 jrf 2.21.03        Created
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        //
        public virtual ProcedureResultCodes EncryptC1218Passwords()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam = new byte[0];
            byte[] ProcResponse;

            ProcResult = ExecuteProcedure(Procedures.ENCRYPT_C1218_PASSWORDS, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Performs a Set Clock - ONLY USE FOR TESTS
        /// </summary>
        /// <param name="dtNewMeterTime">the time to adjust to</param>
        /// <param name="bCheckClockRunning">
        /// Whether or not we care if the clock is running.  Mostly we do care
        /// but we need to be able to set the clock after an extended outage when the meter becomes demand only
        /// </param>
        /// <param name="bCheckAdjustOverDST">
        /// Whether or not we want to check if the meter is in DST transition.
        /// </param>
        /// <returns>ClockAdjustResult</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/05/11 AF  2.51.20 175182 Refactored to prevent three copies of almost the same code
        //                              This is the original SetClock with checks to see if we want to
        //                              check on the clock running and on DST transition
        //
        public ClockAdjustResult SetClock(DateTime dtNewMeterTime, bool bCheckClockRunning, bool bCheckAdjustOverDST)
        {
            ClockAdjustResult Result = ClockAdjustResult.SUCCESS;
            DateTime ReferenceDate = new DateTime(1970, 1, 1);
            CDSTDatePair DSTDate = null;
            TimeSpan Span = new TimeSpan();
            uint MeterMinutes;
            byte[] byMinutes;
            byte MeterSeconds;
            byte[] byParameters = new byte[SET_TIME_DATE_PROC_SIZE];
            int iIndex = 0;
            int iDSTIndex = 0;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            bool bContinue = true;

            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting Set Clock");

                if (bCheckClockRunning && !ClockRunning)
                {
                    bContinue = false;
                }

                if (bContinue)
                {
                    //Setup the parameters for Procedure 10

                    if (dtNewMeterTime.Kind == DateTimeKind.Utc)
                    {
                        ReferenceDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    }

                    //Convert the time to minutes and seconds since 1/1/1970					
                    Span = dtNewMeterTime - ReferenceDate;
                    MeterMinutes = (uint)Span.TotalMinutes;
                    MeterSeconds = (byte)Span.Seconds;

                    //The first byte is the Set_Mask (time and date)
                    byParameters[iIndex++] = AMI_SET_TIME_DATE_MASK;

                    //Minutes since 1/1/1970
                    byMinutes = BitConverter.GetBytes(MeterMinutes);
                    Array.Copy(byMinutes, 0, byParameters, iIndex, byMinutes.Length);
                    iIndex += byMinutes.Length;

                    //Add the seconds
                    byParameters[iIndex++] = MeterSeconds;

                    //Set up the time date qual bitfield
                    TIME_DATE_QUAL_BFLD TimeDateQual = 0x00;

                    if (dtNewMeterTime.Kind == DateTimeKind.Utc)
                    {
                        // Set it up for UTC
                        TimeDateQual = TIME_DATE_QUAL_BFLD.GMT_FLAG;
                    }
                    else
                    {
                        // Set it up for local time.

                        if (Table52.IsTimeZoneApplied == true)
                        {
                            TimeDateQual |= TIME_DATE_QUAL_BFLD.TM_ZN_APPLIED_FLAG;
                        }

                        if (Table52.IsDSTApplied == true)
                        {
                            TimeDateQual |= TIME_DATE_QUAL_BFLD.DST_APPLIED_FLAG;

                            // Find the DST dates for this year
                            while (iDSTIndex < DST.Count && DST[iDSTIndex].ToDate.Year != dtNewMeterTime.Year)
                            {
                                iDSTIndex++;
                            }

                            if (iDSTIndex < DST.Count)
                            {
                                DSTDate = DST[iDSTIndex];
                            }

                            // Determine if the new time is in DST
                            if (DSTDate != null)
                            {
                                if (IsMeterInDST == true && (dtNewMeterTime <= DSTDate.FromDate && dtNewMeterTime >= DSTDate.ToDate))
                                {
                                    // The meter is still in DST
                                    TimeDateQual |= TIME_DATE_QUAL_BFLD.DST_FLAG;
                                }
                                else if (bCheckAdjustOverDST)
                                {
                                    if (IsMeterInDST == true || (IsMeterInDST == false && (dtNewMeterTime <= DSTDate.FromDate && dtNewMeterTime >= DSTDate.ToDate)))
                                    {
                                        // Trying to adjust over a DST change
                                        Result = ClockAdjustResult.ERROR_NO_ADJUST_OVER_DST;
                                    }
                                }
                            }
                            else
                            {
                                // We don't have a DST date for the current year so assume the time
                                // will be the same
                                if (IsMeterInDST == true)
                                {
                                    TimeDateQual |= TIME_DATE_QUAL_BFLD.DST_FLAG;
                                }
                            }
                        }
                    }

                    TimeDateQual |= (TIME_DATE_QUAL_BFLD)((byte)dtNewMeterTime.DayOfWeek & (byte)TIME_DATE_QUAL_BFLD.DAY_OF_WEEK);

                    byParameters[iIndex] = (byte)TimeDateQual;

                    if (Result == ClockAdjustResult.SUCCESS || Result == ClockAdjustResult.SUCCESS_24_HOUR_MAXIMUM_ADJUST)
                    {
                        //Execute the set date time procedure
                        ProcResult = ExecuteProcedure(Procedures.SET_DATE_TIME,
                                                       byParameters,
                                                       out ProcResponse);

                        switch (ProcResult)
                        {
                            case ProcedureResultCodes.COMPLETED:
                            {
                                Result = ClockAdjustResult.SUCCESS;
                                break;
                            }
                            case ProcedureResultCodes.NO_AUTHORIZATION:
                            {
                                Result = ClockAdjustResult.SECURITY_ERROR;
                                break;
                            }
                            case ProcedureResultCodes.UNRECOGNIZED_PROC:
                            {
                                Result = ClockAdjustResult.UNSUPPORTED_OPERATION;
                                break;
                            }
                            default:
                            {
                                Result = ClockAdjustResult.ERROR;
                                break;
                            }
                        }

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                           "Set date time Result = " + ProcResult);
                    }
                }
                else
                {
                    //The clock is not running
                    Result = ClockAdjustResult.ERROR_CLOCK_NOT_RUNNING;
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return Result;
        }

        /// <summary>
        /// Performs a Set Clock - ONLY USE FOR TESTS
        /// </summary>
        /// <param name="dtNewMeterTime"></param>
        /// <returns>ClockAdjustResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/20/09 KRC         N/A    Adding Set Clock for our Firmware Tests
        //  07/05/11 AF  2.51.20 175182 Refactored to prevent duplicate code
        //
        public ClockAdjustResult SetClock(DateTime dtNewMeterTime)
        {
            return SetClock(dtNewMeterTime, true, true);
        }

        /// <summary>
        /// Performs a Set Clock - ONLY USE FOR TESTS
        /// This version does not check if the clock is running.  After an
        /// extended outage, the meter becomes a demand only meter until we set the clock
        /// </summary>
        /// <param name="dtNewMeterTime">the time the meter should have</param>
        /// <returns>ClockAdjustResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/30/11 AF  2.51.20 175182 Don't check on ClockRunning but do check DST transition  
        //
        public ClockAdjustResult SetClockAfterExtendedOutage(DateTime dtNewMeterTime)
        {
            return SetClock(dtNewMeterTime, false, true);
        }

        /// <summary>
        /// Performs a Set Clock - ONLY USE FOR TESTS
        /// </summary>
        /// <param name="dtNewMeterTime"></param>
        /// <returns>ClockAdjustResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/22/10 MMD        N/A    Adding Set Clock Over DST for our Firmware Tests
        //  07/05/11 AF  2.51.20 175182 Refactored to prevent duplicate code
        //
        public ClockAdjustResult SetClockOverDST(DateTime dtNewMeterTime)
        {
            return SetClock(dtNewMeterTime, true, false);
        }

        /// <summary>
        /// This method will give the number of retries for a failed Switch Operation --- TEST PURPOSE
        /// </summary>
        /// <returns>An ProcedureResultCodes representing the result of the operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/01/06 MMD            N/A    Created

        public virtual ProcedureResultCodes SwitchRetryAutomatedTestProcedure(out bool ResponseCode)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];
            ProcParam[0] = 0x00; //Get Switch Retry Status
            ProcResult = ExecuteProcedure(Procedures.REMOTE_CONNECT_DISCONNECT_RETRY, ProcParam, out ProcResponse);
            ResponseCode = Convert.ToBoolean(ProcResponse[0]);
            return ProcResult;
        }

        /// <summary>
        /// This method will execute the Disconnect with the Response Code. --- For TESTING ONLY
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/04/10 MMD                Created for the new firmware enhancement
        //
        public virtual RemoteConnectResult RemoteDisconnectWithInvalidData(out ConnectDisconnectResponse ResponseCode, byte[] ProcParam)
        {
            RemoteConnectResult Result = RemoteConnectResult.REMOTE_ACTION_SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;

            ProcResult = ExecuteProcedure(Procedures.REMOTE_CONNECT_DISCONNECT,
                ProcParam, out ProcResponse);

            // Meters from Hydrogen on will return a 1 byte response but prior meters will not
            // The response code really only applies to a successful response from the procedure
            if (ProcResponse.Length > 0)
            {
                ResponseCode = (ConnectDisconnectResponse)ProcResponse[0];
            }
            else
            {
                // Lets just set this to success so that it is easy to interpret responses
                // for pre and post Hydrogen meters.
                ResponseCode = ConnectDisconnectResponse.Successful;
            }

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    //Success
                    Result = RemoteConnectResult.REMOTE_ACTION_SUCCESS;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    //Isc error
                    Result = RemoteConnectResult.SECURITY_VIOLATION;
                    break;
                }
                case ProcedureResultCodes.DEVICE_SETUP_CONFLICT:
                {
                    //This generally happens because there is a load voltage present.
                    Result = RemoteConnectResult.LOAD_VOLTAGE_PRESENT;
                    break;
                }
                case ProcedureResultCodes.INVALID_PARAM:
                {
                    // This happens because no Voltage is detected, which means the switch failed.  Device is returned
                    //  to Disconnect state.
                    Result = RemoteConnectResult.LOAD_VOLTAGE_NOT_DETECTED;
                    break;
                }
                case ProcedureResultCodes.UNRECOGNIZED_PROC:
                {
                    // This happens because the procedure has been called on a meter that does not
                    // support disconnects.
                    Result = RemoteConnectResult.UNRECOGNIZED_PROCEDURE;
                    break;
                }
                default:
                {
                    //General Error
                    Result = RemoteConnectResult.REMOTE_CONNECT_FAILED;
                    break;
                }
            }

            return Result;

        } // End RemoteDisconnect()

        /// <summary>
        /// This method will execute the Disconnect with the Response Code.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/04/10 MMD                Created for the new firmware enhancement
        // 01/20/11 RCG 2.45.23        Updating to support Connect/Disconnect Procedure Enhancement

        public virtual RemoteConnectResult RemoteDisconnect(out ConnectDisconnectResponse ResponseCode)
        {
            RemoteConnectResult Result = RemoteConnectResult.REMOTE_ACTION_SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];  // 1 Parameter Needed.
            // Bit 0 = 0x01 - Connect; 0x00 Disconnect;
            // Bit 1 - 0x01 - user intervention; 0x00 no intervention;
            ProcParam[0] = 0x00;

            ProcResult = ExecuteProcedure(Procedures.REMOTE_CONNECT_DISCONNECT,
                ProcParam, out ProcResponse);

            // Meters from Hydrogen on will return a 1 byte response but prior meters will not
            // The response code really only applies to a successful response from the procedure
            if (ProcResponse.Length > 0)
            {
                ResponseCode = (ConnectDisconnectResponse)ProcResponse[0];
            }
            else
            {
                // Lets just set this to success so that it is easy to interpret responses
                // for pre and post Hydrogen meters.
                ResponseCode = ConnectDisconnectResponse.Successful;
            }

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    //Success
                    Result = RemoteConnectResult.REMOTE_ACTION_SUCCESS;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    //Isc error
                    Result = RemoteConnectResult.SECURITY_VIOLATION;
                    break;
                }
                case ProcedureResultCodes.DEVICE_SETUP_CONFLICT:
                {
                    //This generally happens because there is a load voltage present.
                    Result = RemoteConnectResult.LOAD_VOLTAGE_PRESENT;
                    break;
                }
                case ProcedureResultCodes.INVALID_PARAM:
                {
                    // This happens because no Voltage is detected, which means the switch failed.  Device is returned
                    //  to Disconnect state.
                    Result = RemoteConnectResult.LOAD_VOLTAGE_NOT_DETECTED;
                    break;
                }
                case ProcedureResultCodes.UNRECOGNIZED_PROC:
                {
                    // This happens because the procedure has been called on a meter that does not
                    // support disconnects.
                    Result = RemoteConnectResult.UNRECOGNIZED_PROCEDURE;
                    break;
                }
                default:
                {
                    //General Error
                    Result = RemoteConnectResult.REMOTE_CONNECT_FAILED;
                    break;
                }
            }

            return Result;

        } // End RemoteDisconnect()

        /// <summary>
        /// This method will execute the Connect.
        /// </summary>
        /// <returns>An RemoteConnectResult representing the result of the reset
        /// operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/01/06 KRC 7.35.00  N/A   Created
        // 11/20/08 RCG 2.00.09  N/A   Changed to call new RemoteConnect method
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        // 01/20/11 RCG 2.45.23        Updating to support Connect/Disconnect Procedure Enhancement

        public virtual RemoteConnectResult RemoteConnect(ConnectType connectType, out ConnectDisconnectResponse responseCode)
        {
            return RemoteConnect(connectType, false, out responseCode);
        } // End Connect()

        /// <summary>
        /// This method executes the remote connect and returns Response Code
        /// </summary>
        /// <param name="connectType">The connection type.</param>
        /// <param name="ignoreLoadVoltage">Whether or not load voltage should be ignored.</param>
        /// <param name="ResponseCode">Response Code Returned</param>
        /// <returns>The result of the connect operation.</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/05/11 MMD          N/A   Created
        // 01/20/11 RCG 2.45.23        Updating to support Connect/Disconnect Procedure Enhancement

        public virtual RemoteConnectResult RemoteConnect(ConnectType connectType, bool ignoreLoadVoltage, out ConnectDisconnectResponse ResponseCode)
        {
            RemoteConnectResult Result = RemoteConnectResult.REMOTE_ACTION_SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];  // 1 Parameter Needed.
            // Bit 0 = 0x01 - Connect; 0x00 Disconnect;
            // Bit 1 - 0x02 - user intervention; 0x00 no intervention;
            // Bit 7 - 0x80 - ignore load voltage; 0x00 don't ignore load voltage
            ProcParam[0] = (byte)(0x01 | ((byte)connectType << 1));

            if (ignoreLoadVoltage)
            {
                ProcParam[0] |= 0x80;
            }

            ProcResult = ExecuteProcedure(Procedures.REMOTE_CONNECT_DISCONNECT,
                ProcParam, out ProcResponse);

            // Meters from Hydrogen on will return a 1 byte response but prior meters will not
            // The response code really only applies to a successful response from the procedure
            if (ProcResponse.Length > 0)
            {
                ResponseCode = (ConnectDisconnectResponse)ProcResponse[0];
            }
            else
            {
                // Lets just set this to success so that it is easy to interpret responses
                // for pre and post Hydrogen meters.
                ResponseCode = ConnectDisconnectResponse.Successful;
            }

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    //Success
                    Result = RemoteConnectResult.REMOTE_ACTION_SUCCESS;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    //Isc error
                    Result = RemoteConnectResult.SECURITY_VIOLATION;
                    break;
                }
                case ProcedureResultCodes.DEVICE_SETUP_CONFLICT:
                {
                    //This generally happens because there is a load voltage present.
                    Result = RemoteConnectResult.LOAD_VOLTAGE_PRESENT;
                    break;
                }
                default:
                {
                    //General Error
                    Result = RemoteConnectResult.REMOTE_CONNECT_FAILED;
                    break;
                }
            }

            return Result;
        }

        /// <summary>
        /// This method will execute the Load Voltage Detection.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the Load Voltage Detection
        /// operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/06/07 KRC 8.10.12  3024    Detecting Load Side Voltage
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        //
        public virtual LoadVoltageDetectionResult LoadSideVoltageDetection()
        {
            LoadVoltageDetectionResult Result = LoadVoltageDetectionResult.LOAD_SIDE_VOLTAGE_NOT_PRESENT;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[0];  // No parameters for this procedure
            ProcResult = ExecuteProcedure(Procedures.LOAD_SIDE_VOLTAGE_DETECTION,
                ProcParam, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    if (ProcResponse[0] == 0)
                    {
                        Result = LoadVoltageDetectionResult.LOAD_SIDE_VOLTAGE_NOT_PRESENT;
                    }
                    else
                    {
                        Result = LoadVoltageDetectionResult.LOAD_SIDE_VOLTAGE_PRESENT;
                    }
                    break;
                }
                default:
                {
                    //General Error
                    Result = LoadVoltageDetectionResult.LOAD_SIDE_VOLTAGE_NOT_PRESENT;
                    break;
                }
            }

            return Result;

        } // End LoadSideVoltageDetection()


        /// <summary>
        /// Configure CPP
        /// </summary>
        /// <param name="startTimeGmt">Start Time (GMT)</param>
        /// <param name="duration">Duration in minute</param>
        /// <returns></returns>
        public virtual ConfigCppResult ConfigCpp(DateTime startTimeGmt, UInt16 duration)
        {
            ConfigCppResult Result = ConfigCppResult.Error;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            MemoryStream ProcParam = null;
            PSEMBinaryWriter ParamWriter = null;

            try
            {
                ProcParam = new MemoryStream(6);
                ParamWriter = new PSEMBinaryWriter(ProcParam);

                ParamWriter.WriteSTIME(startTimeGmt, PSEMBinaryReader.TM_FORMAT.UINT32_TIME);
                ParamWriter.Write((UInt16)duration);

                ProcResult = ExecuteProcedure(Procedures.CONFIG_CPP, ProcParam.ToArray(), out ProcResponse);

                switch (ProcResult)
                {
                    case ProcedureResultCodes.COMPLETED:
                    {
                        switch (ProcResponse[0])
                        {
                            case 0:
                            Result = ConfigCppResult.ConfiguredOk;
                            break;
                            case 1:
                            Result = ConfigCppResult.InvalidStartTime;
                            break;
                            case 2:
                            Result = ConfigCppResult.ClearedOk;
                            break;
                            case 3:
                            Result = ConfigCppResult.CppIsActive;
                            break;
                            default:
                            Result = ConfigCppResult.Error;
                            break;
                        }
                        break;
                    }
                    default:
                    {
                        //General Error
                        Result = ConfigCppResult.Error;
                        break;
                    }
                }
            }
            finally
            {
                if (null != ProcParam)
                {
                    ProcParam.Dispose();
                }
            }

            return Result;
        }

        /// <summary>
        /// Causes Fatal Error 7 on an OpenWay CENTRON meter.
        /// </summary>
        /// <returns>
        /// An ProcedureResultCodes representing the result of the operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/24/10 MMD        		Created 
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual ProcedureResultCodes CauseFatalError()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(9);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] ProcResponse;

            ParamWriter.Write(CAUSE_FATAL_FUNC_CODE);
            ParamWriter.Write(CAUSE_FATAL_PARAM_1);
            ParamWriter.Write(CAUSE_FATAL_PARAM_2);

            ProcResult = ExecuteProcedure(Procedures.RESET_RF_LAN, ProcParam.ToArray(), out ProcResponse);
            return ProcResult;
        }

        /// <summary>
        /// Causes Fatal Error 2 on an OpenWay CENTRON meter.
        /// </summary>
        /// <returns>
        /// An ProcedureResultCodes representing the result of the operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/08/11 MAH        		Created 
        //
        public virtual ProcedureResultCodes CauseFatalError2()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(9);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] ProcResponse;

            ParamWriter.Write(CAUSE_FATAL_FUNC_CODE);
            ParamWriter.Write(CAUSE_FATAL2_PARAM_1);
            ParamWriter.Write(CAUSE_FATAL2_PARAM_2);

            ProcResult = ExecuteProcedure(Procedures.RESET_RF_LAN, ProcParam.ToArray(), out ProcResponse);
            return ProcResult;
        }

        /// <summary>
        /// Causes Fatal Error 6 on an OpenWay CENTRON meter.
        /// </summary>
        /// <returns>
        /// An ProcedureResultCodes representing the result of the operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/08/11 MAH        		Created 
        //
        public virtual ProcedureResultCodes CauseFatalError6()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(9);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] ProcResponse;

            ParamWriter.Write(CAUSE_FATAL_FUNC_CODE);
            ParamWriter.Write(CAUSE_FATAL6_PARAM_1);
            ParamWriter.Write(CAUSE_FATAL6_PARAM_2);

            ProcResult = ExecuteProcedure(Procedures.RESET_RF_LAN, ProcParam.ToArray(), out ProcResponse);
            return ProcResult;
        }

        /// <summary>
        /// Clears the Recoverable Fatal Error status bits in Std Table 3
        /// </summary>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/05/10 RCG 2.40.00 N/A    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  02/03/11 AF  2.50.04        Removed virtual since M2 Gateway now uses the same procedure
        //
        public ProcedureResultCodes ClearFatalErrorRecoveryMode()
        {
            byte[] ProcResponse;
            byte[] byParameter = new byte[] { (byte)FatalRecoveryOption.ClearRecoveryMode };
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            ProcResult = ExecuteProcedure(Procedures.FATAL_ERROR_RECOVERY, byParameter, out ProcResponse);

            return ProcResult;
        }


        /// <summary>
        /// Clears the Core Dump status bits in Std Table 3
        /// </summary>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/05/10 RCG 2.40.00 N/A    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  02/03/11 AF  2.50.04        Removed virtual since M2 Gateway now uses the same procedure
        //
        public ProcedureResultCodes ClearCoreDumpStatus()
        {
            byte[] ProcResponse;
            byte[] byParameter = new byte[] { (byte)FatalRecoveryOption.ClearCoreDump };
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            ProcResult = ExecuteProcedure(Procedures.FATAL_ERROR_RECOVERY, byParameter, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This method forces the meter enable or disable service limiting by 
        /// entering into the failsafe mode
        /// </summary>
        /// <param name="minsInFailSafe">The number of minutes the meter stays in the failsafe mode.</param>
        /// <returns>The result of the service limiting.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/18/09 MMD                Created
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        //
        public virtual ItronDeviceResult ServiceLimitingFailSafe(UInt16 minsInFailSafe)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] abyParameter = BitConverter.GetBytes(minsInFailSafe);
            byte[] abyProcResponse;

            ProcResult = ExecuteProcedure(Procedures.SERVICE_LIMITING_FAIL_SAFE, abyParameter, out abyProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    //Success
                    Result = ItronDeviceResult.SUCCESS;
                    break;
                }
                case ProcedureResultCodes.UNRECOGNIZED_PROC:
                {
                    Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    //Security Error
                    Result = ItronDeviceResult.SECURITY_ERROR;
                    break;
                }
                default:
                {
                    //General Error
                    Result = ItronDeviceResult.ERROR;
                    break;
                }
            }

            return Result;
        }

        /// <summary>
        /// This method will validate any of the 4 types of security keys
        /// </summary>
        /// <param name="SecType">
        /// The type of key: Enhanced Security, C1218 Passwords, C1222 Keys, or HAN Keys
        ///</param>
        /// <param name="SecID">
        /// For the given key type, which key to validate:
        /// 1 = "Command Key 1/C1218 Primary PWD/C1222 Key 1/HAN Network Key",
        /// 2 = "Command Key 2/C1218 Secondary PWD/C1222 Key 2/HAN Global Link Key",
        /// 3 = "Command Key 3/C1218 Tertiary PWD/C1222 Key 3",
        /// 4 = "Command Key 4/C1218 Quaternary PWD/C1222 Key 4",
        /// 5 = "Revocation Key 1",
        /// 6 = "Revocation Key 2"
        /// </param>
        /// <param name="hashedKey">The hashed value of the key to validate</param>
        /// <returns>Results of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/08/09 AF  2.20.11        Created
        //
        public ProcedureResultCodes ValidateSecurityKey(SecurityType SecType, SecurityKeyID SecID, byte[] hashedKey)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam = new byte[34];
            byte[] ProcResponse;
            PSEMBinaryWriter PSEMWriter = new PSEMBinaryWriter(new MemoryStream(ProcParam));

            PSEMWriter.Write((byte)SecType);
            PSEMWriter.Write((byte)SecID);
            PSEMWriter.Write(hashedKey);

            ProcResult = ExecuteProcedure(Procedures.VALIDATE_SECURITY_DATA, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Issues the PSEM security command
        /// </summary>
        /// <param name="Passwords">A list of passwords to be issued to the 
        /// meter. An empty string should be supplied if a null password is 
        /// to be attempted.</param>
        /// <returns>A ItronDeviceResult</returns>		
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/28/06 KRC  7.35.00 N/A   Adding AMI Tables
        // 11/02/06 AF   7.40.00       Changed code for instantiating the table 2108 object
        //
        public override ItronDeviceResult Security(System.Collections.Generic.List<string> Passwords)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;


            Result = base.Security(Passwords);

            return Result;
        }


        /// <summary>
        /// This method configures the remote disconnect switch to be enabled or disabled.
        /// </summary>
        /// <param name="SwitchSetting">Whether or not the switch is enabled</param>
        /// <returns>The result of the configuration.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/18/09 jrf 2.20.05 133921 Created
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        //
        public virtual ItronDeviceResult ConfigureRemoteDisconnectSwitch(DisconnectSwitchSettings SwitchSetting)
        {
            PSEMResponse Result = PSEMResponse.Err;
            ItronDeviceResult ConfigResult = ItronDeviceResult.ERROR;

            switch (SwitchSetting)
            {
                // A disabled switch means that service limiting has been overridden.
                case DisconnectSwitchSettings.Disabled:
                {
                    Table2142.IsServiceLimitingOverriden = true;
                    break;
                }
                case DisconnectSwitchSettings.Enabled:
                {
                    Table2142.IsServiceLimitingOverriden = false;
                    break;
                }
                default:
                {
                    // Unrecognized setting.  Do not change anything.
                    break;
                }
            }

            Result = Table2142.Write();

            if (Result == PSEMResponse.Ok)
            {
                ConfigResult = ItronDeviceResult.SUCCESS;
            }
            else if (Result == PSEMResponse.Isc)
            {
                ConfigResult = ItronDeviceResult.SECURITY_ERROR;
            }
            else
            {
                ConfigResult = ItronDeviceResult.ERROR;
            }

            return ConfigResult;
        }

        /// <summary>
        /// Changes the LED to pulse the specified quantity.
        /// </summary>
        /// <param name="quantity">The Quantity to change to.</param>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/10 RCG 2.40.09        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual ProcedureResultCodes ReconfigureLEDQuantity(LEDQuantityOption quantity)
        {
            byte[] ProcResponse;
            byte[] byParameter = new byte[] { (byte)quantity };
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            ProcResult = ExecuteProcedure(Procedures.RECONFIGURE_LED_QUANTITY, byParameter, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Clears the max readings from table 2263 (Mfg 215)
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        //
        public virtual ProcedureResultCodes ClearTamperTapMaxStats()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[0];  // No parameters for this procedure
            ProcResult = ExecuteProcedure(Procedures.CLEAR_TAMPER_TAP_MAX_STATS,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Clears the maximum task execution times
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/07/17 mah 2.51.20        Created
        //
        public ProcedureResultCodes ResetTaskExecutionTimes()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[0];  // No parameters for this procedure
            ProcResult = ExecuteProcedure(Procedures.RESET_TASK_EXECUTION_TIME,
                ProcParam, out ProcResponse);

            return ProcResult;
        }
        /// <summary>
        /// Gets the Date of the TOU Expiration
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/06 KRC 7.35.00 N/A    Created
        //  01/03/07 RCG 8.00.00 N/A    Fixing the handling of the LID data so that
        //                              it handles the 6 byte format unique to this LID
        // 		
        public override DateTime TOUExpirationDate
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                DateTime dtTOUExpire = MeterConfigurationReferenceTime;
                uint uiUnusedData;
                ushort usExpirationYears;
                byte[] Data = null;

                if (!m_TOUExpireDate.Cached)
                {
                    Result = m_lidRetriever.RetrieveLID(m_LID.TOU_EXPIRATION_DATE, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        //Convert the data to seconds
                        MemoryStream TempStream = new MemoryStream(Data);
                        BinaryReader TempBReader = new BinaryReader(TempStream);

                        // The result of this LID request returns 3 2 byte values we only need
                        // the last value which contains the number of years
                        uiUnusedData = TempBReader.ReadUInt32();
                        usExpirationYears = TempBReader.ReadUInt16();

                        // Add the number of years to the reference time (1/1/2000)
                        m_TOUExpireDate.Value = dtTOUExpire.AddYears((int)usExpirationYears);
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading TOU Expiration date"));
                    }
                }

                return m_TOUExpireDate.Value;
            }
        }

        /// <summary>
        /// Sets the time on battery and the count of prior registrations.
        /// For use in automated testing.
        /// </summary>
        /// <param name="numberOfMinutes">The number of minutes that the meter will think it has been on battery</param>
        /// <param name="registrationCount">The number of registrations the meter will think it has had</param>
        /// <returns>The procedure result code</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/01/01 AF  2.51.32        Created for time sync testing
        //
        public ProcedureResultCodes SetTimeOnBatteryAndRegistrationCount(uint numberOfMinutes, uint registrationCount)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(9);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] ProcResponse;

            ParamWriter.Write(TIME_ON_BATT_FUNC_CODE);
            ParamWriter.Write(numberOfMinutes);
            ParamWriter.Write(registrationCount);

            ProcResult = ExecuteProcedure(Procedures.RESET_RF_LAN, ProcParam.ToArray(), out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Authenticates the firmware download using the fw file hash code and 
        /// Mfg procedure 117
        /// </summary>
        /// <param name="TableID">the table id of the fwdl pending table</param>
        /// <param name="FWType">firmware type - register, RFLAN, etc</param>
        /// <param name="HashCode">the 32-byte hash code for the firmware file being used</param>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/18/11 AF  2.52.05        Created
        //
        public virtual ProcedureResultCodes AuthenticateFWDL(ushort TableID, byte FWType, byte[] HashCode)
        {
            return ProcedureResultCodes.UNRECOGNIZED_PROC;
        }

        /// <summary>
        /// RFLAN Opt Out, disable RFLAN
        /// Mfg procedure 119
        /// </summary>
        /// <param name="Enabled">Determines whether the RFLAN is to be disabled or not.</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/11 MSC  2.53.03        Created
        public virtual ProcedureResultCodes RFLANOptOut(bool Enabled)
        {
            ProcedureResultCodes result = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam1;
            byte[] ProcResponse1;
            ProcParam1 = new byte[2];
            ProcParam1[0] = 0x01;
            ProcParam1[1] = 0x00;

            if (Enabled == true)
            {
                ProcParam1[0] = 0xFF;   //will disable the RFLAN
            }

            result = ExecuteProcedureAndWaitForResult(Procedures.RFLAN_OPT_OUT, ProcParam1, out ProcResponse1);

            if (result == ProcedureResultCodes.COMPLETED)
            {
                MemoryStream ProcParam = new MemoryStream(9);
                BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
                byte[] ProcResponse;
                const byte RESET_RF_FUNC_CODE = 1;
                const uint RESET_RF_PARAM_1 = 32;
                const uint RESET_RF_PARAM_2 = 0;

                ParamWriter.Write(RESET_RF_FUNC_CODE);
                ParamWriter.Write(RESET_RF_PARAM_1);
                ParamWriter.Write(RESET_RF_PARAM_2);

                result = ExecuteProcedureAndWaitForResult(Procedures.RESET_RF_LAN, ProcParam.ToArray(), out ProcResponse);

            }

            return result;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the Customer Serial Number from Table 6 rather than 2048.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/06 RCG 7.40.00 N/A    Created
        //
        public override string SerialNumber
        {
            get
            {
                return Table06.UtilitySerialNumber;
            }
        }

        /// <summary>
        /// Gets the register module version.revision from MFG Table 2108
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 09/23/10 MMD				    Created
        //
        public virtual string RegModVer
        {
            get
            {
                return Table2108.RegModuleVersion;
            }
        }

        /// <summary>
        /// Gets the register module build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/23/10 MMD				    Created
        //
        public virtual string RegModBuild
        {
            get
            {
                return Table2108.RegModuleBuild;
            }
        }

        /// <summary>
        /// Gets the display version.revision from MFG Table 2108
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 09/23/10 MMD				    Created
        //
        public virtual string DisplayModVer
        {
            get
            {
                return Table2108.DisplayModuleVersion;
            }
        }

        /// <summary>
        /// Gets the display Build from MFG Table 2108
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 09/23/10 MMD				    Created
        //
        public virtual string DisplayModBuild
        {
            get
            {
                return Table2108.DisplayModuleBuild;
            }
        }

        /// <summary>
        /// Property to retrieve the firmware version and revision from the meter.
        /// </summary>
        public float FirmwareLoaderVerRev
        {
            get
            {
                if (false == m_fltFWLoaderVerRev.Cached)
                {
                    uint LDRVersion;
                    uint LDRRevsion;
                    byte[] Data = null;
                    PSEMResponse Result = m_lidRetriever.RetrieveLID(m_LID.FIRMWARE_LOADER_VERSION, out Data);

                    if (PSEMResponse.Ok == Result)
                    {
                        LDRVersion = (uint)Data[0];
                        Data = null;
                        Result = m_lidRetriever.RetrieveLID(m_LID.FIRMWARE_LOADER_REVISION, out Data);
                        if (PSEMResponse.Ok == Result)
                        {
                            LDRRevsion = (uint)Data[0];

                            m_fltFWLoaderVerRev.Value = LDRVersion + (float)(LDRRevsion / 1000.0F);
                        }
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                                    "Error reading Firmware Loader Version and Revision"));
                    }
                }

                return m_fltFWLoaderVerRev.Value;
            }
        }

        /// <summary>
        /// Gets the number sessions in which tables have been written as part 
        /// of asset synchronization. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/10 jrf 2.41.01        Created
        //
        public ushort ConfigurationCount
        {
            get
            {
                return Table2264.ProgramCount;
            }
        }

        /// <summary>
        /// Gets the HW Revision with the Prism Lite bit filtered out.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/01/10 RCG 2.40.31	    Created

        public virtual float HWRevisionFiltered
        {
            get
            {
                float fRevision = HWRevision;
                return fRevision;
            }
        }

        /// <summary>
        /// Gets whether or not the meter currently has a Full Core Dump available
        /// and further core dumps are blocked by the Table 3 status bit.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/06/10 RCG 2.40.02	    Created
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        //
        public virtual bool IsFullCoreDumpBlocked
        {
            get
            {
                bool bLocked = false;

                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) >= 0)
                {
                    bLocked = Table03.IsFullCoreDumpAvailable;
                }

                return bLocked;
            }
        }

        /// <summary>
        /// Gets whether or not a core dump is currently present in the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/04/10 RCG 2.40.11	    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  02/04/11 AF  2.50.04        Removed virtual now that Gateway uses table 3043
        //
        public bool IsFullCoreDumpPresent
        {
            get
            {
                bool bCoreDumpPresent = false;

                if (Table3043Header != null)
                {
                    bCoreDumpPresent = Table3043Header.IsValidCoreDump;
                }

                return bCoreDumpPresent;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently in Fatal Error Recovery Mode
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/06/10 RCG 2.40.02	    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool IsInFatalErrorRecoveryMode
        {
            get
            {
                bool bIsInRecoveryMode = false;

                // This only applies to SR 3.0 meters so we should first check the FW version
                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) >= 0)
                {
                    bIsInRecoveryMode = Table03.IsInFatalErrorRecoveryMode;
                }

                return bIsInRecoveryMode;
            }
        }

        /// <summary>
        /// Gets whether or not Fatal Error Recovery is enabled in the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/06/10 RCG 2.40.02	    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool IsFatalErrorRecoveryEnabled
        {
            get
            {
                bool bEnabled = false;

                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) >= 0)
                {
                    bEnabled = Table03.IsFatalErrorRecoveryEnabled;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets the Fatal Recovery Data for the last fatal error.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/28/10 RCG 2.40.10	    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  02/04/11 AF  2.50.04        Removed virtual - no longer needed
        //
        public FatalErrorHistoryData LastFatalErrorData
        {
            get
            {
                FatalErrorHistoryData LastError = null;

                if (Table2261 != null)
                {
                    if (Table2261.FatalErrorHistory != null && Table2261.FatalErrorHistory.Count > 0)
                    {
                        LastError = Table2261.FatalErrorHistory[Table2261.FatalErrorHistory.Count - 1];
                    }
                }

                return LastError;
            }
        }

        /// <summary>
        /// Gets whether or not Asset Sync is enabled.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/23/10 RCG 2.40.28	    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual bool IsAssetSyncEnabled
        {
            get
            {
                bool bEnabled = false;

                if (Table2260SR30Config != null)
                {
                    bEnabled = Table2260SR30Config.AssetSyncEnabled;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not accelerometer tap is enabled in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool IsTapEnabled
        {
            get
            {
                bool bEnabled = false;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    bEnabled = Table2260SR30Config.TapEnabled;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual byte InversionThreshold
        {
            get
            {
                byte byThreshold = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    byThreshold = Table2260SR30Config.InversionThreshold;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual byte RemovalThreshold
        {
            get
            {
                byte byThreshold = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    byThreshold = Table2260SR30Config.RemovalThreshold;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual byte TapThreshold
        {
            get
            {
                byte byThreshold = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    byThreshold = Table2260SR30Config.TapThreshold;
                }

                return byThreshold;
            }
        }

        /// <summary>
        ///  The number of seconds to pull data from the accelerometer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual UInt32 WakeupDuration
        {
            get
            {
                UInt32 uiSeconds = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    uiSeconds = Table2260SR30Config.WakeupDurationSecond;
                }

                return uiSeconds;
            }
        }

        /// <summary>
        /// If true, there are errors in accelerometer configuration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool AccelerometerConfigError
        {
            get
            {
                bool bError = false;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    bError = Table2262.AccelerometerConfigError;
                }

                return bError;
            }
        }

        /// <summary>
        ///  If false, tap and tamper detections are not running
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool WakeUpStatus
        {
            get
            {
                bool bStatus = false;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    bStatus = Table2262.WakeUpStatus;
                }

                return bStatus;
            }
        }

        /// <summary>
        /// If true, a removal tamper has been detected and the meter is checking 
        /// the power down for 10 seconds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool RemovalPDNCheck
        {
            get
            {
                bool bCheck = false;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    bCheck = Table2262.RemovalPDNCheck;
                }

                return bCheck;
            }
        }

        /// <summary>
        /// Checks the tap check field of table 2262 to see if a tap has been detected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool TapDetected
        {
            get
            {
                bool bDetected = false;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    bDetected = Table2262.TapCheck;
                }

                return bDetected;
            }
        }

        /// <summary>
        /// Gets the last Fatal Error that occurred in the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/06/10 RCG 2.40.02	    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual string LastFatalError
        {
            get
            {
                string strLastError = "Not Available";

                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) >= 0)
                {
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
                }

                return strLastError;
            }
        }

        /// <summary>
        /// Gets the Date and Time that the last Fatal Error Occurred.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/29/10 RCG 2.40.10	    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  02/04/11 AF  2.50.04        Removed virtual - no longer needed now that Gateway mirrors
        //                              OpenWay CENTRON fatal error recovery
        //
        public DateTime LastFatalErrorDate
        {
            get
            {
                DateTime ErrorDate = MeterReferenceTime;

                if (LastFatalErrorData != null)
                {
                    ErrorDate = LastFatalErrorData.TimeOfOccurance;
                }

                return ErrorDate;
            }
        }

        /// <summary>
        /// Gets the reason for the last Fatal Error that occurred in the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/06/10 RCG 2.40.02	    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  02/23/11 AF  2.50.05        Added "0x" to reason to make it clear that it's hex
        //
        public virtual string LastFatalErrorReason
        {
            get
            {
                string strLastErrorReason = "Not Available";

                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) >= 0)
                {
                    if (LastFatalErrorData != null)
                    {
                        strLastErrorReason = "0x" + LastFatalErrorData.Reason.ToString("X4", CultureInfo.CurrentCulture);
                    }
                }

                return strLastErrorReason;
            }
        }

        /// <summary>
        /// Whether or not the accelerometer is supported 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool IsAccelerometerSupported
        {
            get
            {
                bool bSupported = false;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    bSupported = Table2263.AccelerometerSupported;
                }

                return bSupported;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual float AccReferenceAngleX
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    fAngle = Table2263.ReferenceAngleX;
                }

                return fAngle;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual float AccReferenceAngleY
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    fAngle = Table2263.ReferenceAngleY;
                }

                return fAngle;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual float AccReferenceAngleZ
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    fAngle = Table2263.ReferenceAngleZ;
                }

                return fAngle;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual float AccCurrentAngleX
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    fAngle = Table2263.CurrentAngleX;
                }

                return fAngle;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual float AccCurrentAngleY
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    fAngle = Table2263.CurrentAngleY;
                }

                return fAngle;
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
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual float AccCurrentAngleZ
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    fAngle = Table2263.CurrentAngleZ;
                }

                return fAngle;
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along X axis since power up
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual sbyte MaxDeltaX
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    sbyMaxDelta = Table2263.MaxDeltaX;
                }

                return sbyMaxDelta;
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along Y axis since power up
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual sbyte MaxDeltaY
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    sbyMaxDelta = Table2263.MaxDeltaY;
                }

                return sbyMaxDelta;
            }
        }

        /// <summary>
        /// The maximum absolute difference between acceleration value and reference
        /// value along Z axis since power up
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual sbyte MaxDeltaZ
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    sbyMaxDelta = Table2263.MaxDeltaZ;
                }

                return sbyMaxDelta;
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual sbyte MaxAvgDeltaTap
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    sbyMaxDelta = Table2263.MaxAvgDeltaTap;
                }

                return sbyMaxDelta;
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual sbyte MaxAvgDeltaTamper
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, REGISTER_BASE.HW_VERSION_3_0) >= 0)
                {
                    sbyMaxDelta = Table2263.MaxAvgDeltaTamper;
                }

                return sbyMaxDelta;
            }
        }

#if (!WindowsCE)

        /// <summary>
        /// Return the CPP start time (GMT)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/13/10 JB                 Created
        //  02/24/11 AF  2.50.05        Added a firmware version check because table 2360
        //                              was not supported until 3.6.13
        //  03/16/11 jrf 2.50.09        Removed HW version check since this table is supported on 
        //                              all HW versions.
        //
        public virtual DateTime CppStartTimeGmt
        {
            get
            {
                DateTime startTime = DateTime.MinValue;

                if (VersionChecker.CompareTo(FWRevision, REGISTER_BASE.VERSION_HYDROGEN_3_6) >= 0)
                {
                    startTime = Table2360.StartTimeGmt;
                }

                return startTime;
            }
        }

        /// <summary>
        /// Return the CPP status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/13/10 JB                 Created
        //  02/16/11 AF  2.50.04        Added a firmware version check because table 2360
        //                              was not supported until 3.6.13
        //  03/16/11 jrf 2.50.09        Removed HW version check since this table is supported on 
        //                              all HW versions.
        //  03/22/12 RCG 2.50.51 195340 CppStatus.Unknown is now CPPStatus.Configured so this code has been updated with a better default value

        public virtual CppStatus CppStatus
        {
            get
            {
                CppStatus status = CppStatus.Cpp_Zero_Invalid;

                if (VersionChecker.CompareTo(FWRevision, REGISTER_BASE.VERSION_HYDROGEN_3_6) >= 0)
                {
                    status = Table2360.Status;
                }

                return status;
            }
        }

        /// <summary>
        /// Return the CPP duration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/13/10 JB                 Created
        //  02/21/11 AF  2.50.04        Added a firmware version check because table 2360
        //                              was not supported until 3.6.13
        //  03/16/11 jrf 2.50.09        Removed HW version check since this table is supported on 
        //                              all HW versions.
        //
        public virtual ushort CppDuration
        {
            get
            {
                ushort duration = 0;

                if (VersionChecker.CompareTo(FWRevision, REGISTER_BASE.VERSION_HYDROGEN_3_6) >= 0)
                {
                    duration = Table2360.Duration;
                }

                return duration;
            }
        }

        /// <summary>
        /// Return the CPP rate
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/13/10 JB                 Created
        //  02/24/11 AF  2.50.05        Added a firmware version check because table 2360
        //                              was not supported until 3.6.13
        //  03/16/11 jrf 2.50.09        Removed HW version check since this table is supported on 
        //                              all HW versions.
        //
        public virtual Rate CppRate
        {
            get
            {
                Rate rate = 0;

                if (VersionChecker.CompareTo(FWRevision, REGISTER_BASE.VERSION_HYDROGEN_3_6) >= 0)
                {
                    rate = Table2360.Rate;
                }

                return rate;
            }
        }

#endif

        /// <summary>
        /// Property to retrieve the Number of Removal tampers
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/14/06 KRC  7.35.00		Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        //
        public virtual uint NumberOfRemovalTampers
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;
                uint uiNumRemovals = 0;

                Result = m_lidRetriever.RetrieveLID(m_LID.NUMBER_REMOVAL_TAMPERS, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    uiNumRemovals = (uint)Data[0];
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the number of removal tampers"));
                }


                return uiNumRemovals;
            }
        }

        /// <summary>
        /// Property to retrieve the Number of Inversion tampers
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/14/06 KRC  7.35.00		Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        //
        public virtual uint NumberOfInversionTampers
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;
                uint uiNumInversions = 0;

                Result = m_lidRetriever.RetrieveLID(m_LID.NUMBER_INVERSION_TAMPERS, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    uiNumInversions = (uint)Data[0];
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the number of inversion tampers"));
                }


                return uiNumInversions;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently pulsing Var
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/16/10 RCG 2.40.25	    Created

        public bool? IsLEDPulsingVar
        {
            get
            {
                bool? bPulsing = null;

                if (Table2220 != null)
                {
                    bPulsing = Table2220.IsLEDPulsingVar;
                }

                return bPulsing;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently pulsing VA
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/16/10 RCG 2.40.25	    Created

        public bool? IsLEDPulsingVA
        {
            get
            {
                bool? bPulsing = null;

                if (Table2220 != null)
                {
                    bPulsing = Table2220.IsLEDPulsingVA;
                }

                return bPulsing;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is a Canadian meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/25/10 RCG 2.40.08	    Created

        public override bool IsCanadian
        {
            get
            {
                if (!m_IsCanadian.Cached)
                {
                    if (Table2220 != null)
                    {
                        m_IsCanadian.Value = Table2220.IsCanadian;
                    }
                    else
                    {
                        // Try to get this via LID
                        m_IsCanadian.Value = base.IsCanadian;
                    }
                }

                return m_IsCanadian.Value;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is Sealed for Canada
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/18/10 RCG 2.40.26 N/A    Created

        public override bool IsSealedCanadian
        {
            get
            {
                if (!m_IsSealedCanadian.Cached)
                {
                    if (Table2220 != null)
                    {
                        m_IsSealedCanadian.Value = Table2220.IsSealedCanadian;
                    }
                    else
                    {
                        // Try to get this via LID
                        m_IsSealedCanadian.Value = base.IsSealedCanadian;
                    }
                }

                return m_IsSealedCanadian.Value;
            }
        }

        /// <summary>
        /// Property to determine if the meter is in DST
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/06 KRC  7.35.00			Created
        // 
        public override bool IsMeterInDST
        {
            get
            {
                return Table52.IsInDST;
            }
        }

        /// <summary>
        /// Returns the current time from the standard table rather than a LID
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/06 RCG 7.40.00 N/A    Created
        //
        public override DateTime DeviceTime
        {
            get
            {
                return Table52.CurrentTime;
            }
        }

        /// <summary>
        /// Gets a TimeSpan object that represents the Time Zone Offset
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/06 RCG 7.40.00 N/A    Created
        //
        public TimeSpan TimeZoneOffset
        {
            get
            {
                return Table53.TimeZoneOffset;
            }
        }

        /// <summary>
        /// Gets the Neutral Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity AmpsNeutral
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_AH_NEUTRAL, m_LID.DEMAND_MAX_A_NEUTRAL,
                    "Neutral Amps", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Phase A Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity AmpsPhaseA
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_AH_PHA, m_LID.DEMAND_MAX_A_PHA,
                    "Amps (a)", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Phase B Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity AmpsPhaseB
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_AH_PHB, m_LID.DEMAND_MAX_A_PHB,
                    "Amps (b)", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Phase C Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity AmpsPhaseC
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_AH_PHC, m_LID.DEMAND_MAX_A_PHC,
                    "Amps (c)", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Amps squared from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity AmpsSquared
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_I2H_AGG, m_LID.DEMAND_MAX_I2_AGG,
                    "Amps Squared", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Power Factor from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity PowerFactor
        {
            get
            {
                Quantity PF;

                // There is not PF energy so just check the demand
                PF = GetQuantityFromStandardTables(null, m_LID.DEMAND_MIN_PF_INTERVAL_ARITH,
                    "Power Factor", Table23.CurrentRegisters);

                // Also try the vectorial PF
                if (PF == null)
                {
                    PF = GetQuantityFromStandardTables(null, m_LID.DEMAND_MIN_PF_INTERVAL_VECT,
                        "Power Factor", Table23.CurrentRegisters);
                }

                return PF;
            }
        }

        /// <summary>
        /// Gets the Q Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity QDelivered
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_QH_DEL, m_LID.DEMAND_MAX_Q_DEL,
                    "Q Delivered", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Qh Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity QReceived
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_QH_REC, m_LID.DEMAND_MAX_Q_REC,
                    "Q Received", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the VA Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VADelivered
        {
            get
            {
                Quantity VA;

                // Try getting Arithmatic first.
                VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_DEL_ARITH, m_LID.DEMAND_MAX_VA_DEL_ARITH,
                    "VA Delivered", Table23.CurrentRegisters);

                // Try  getting Vectoral
                if (VA == null)
                {
                    VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_DEL_VECT, m_LID.DEMAND_MAX_VA_DEL_VECT,
                        "VA Delivered", Table23.CurrentRegisters);
                }

                return VA;
            }
        }

        /// <summary>
        /// Gets the Lagging VA from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VALagging
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VAH_LAG, m_LID.DEMAND_MAX_VA_LAG,
                    "VA Lagging", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Var Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarDelivered
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_DEL, m_LID.DEMAND_MAX_VAR_DEL,
                    "Var Delivered", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the VA Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VAReceived
        {
            get
            {
                Quantity VA;

                // Try getting Arithmetic first.
                VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_REC_ARITH, m_LID.DEMAND_MAX_VA_REC_ARITH,
                    "VA Received", Table23.CurrentRegisters);

                // Try  getting Vectorial
                if (VA == null)
                {
                    VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_REC_VECT, m_LID.DEMAND_MAX_VA_REC_VECT,
                        "VA Received", Table23.CurrentRegisters);
                }

                return VA;
            }
        }

        /// <summary>
        /// Gets the Var Net from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarNet
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET, m_LID.DEMAND_MAX_VAR_NET,
                    "Var Net", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Var Net delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarNetDelivered
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET_DEL, m_LID.DEMAND_MAX_VAR_NET_DEL,
                    "Var Net Delivered", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Var Net Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarNetReceived
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET_REC, m_LID.DEMAND_MAX_VAR_NET_REC,
                    "Var Net Received", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Var Q1 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarQuadrant1
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q1, m_LID.DEMAND_MAX_VAR_Q1,
                    "Var Quadrant 1", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Var Q2 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarQuadrant2
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q2, m_LID.DEMAND_MAX_VAR_Q2,
                    "Var Quadrant 2", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Var Q3 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarQuadrant3
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q3, m_LID.DEMAND_MAX_VAR_Q3,
                    "Var Quadrant 3", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Var Q4 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarQuadrant4
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q4, m_LID.DEMAND_MAX_VAR_Q4,
                    "Var Quadrant 4", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Var Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VarReceived
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VARH_REC, m_LID.DEMAND_MAX_VAR_REC,
                    "Var Received", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Average Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VoltsAverage
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VH_AVG, m_LID.DEMAND_MAX_V_AVG,
                    "Volts Average", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Phase A Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VoltsPhaseA
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VH_PHA, m_LID.DEMAND_MAX_V_PHA,
                    "Volts (a)", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Phase B Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VoltsPhaseB
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VH_PHB, m_LID.DEMAND_MAX_V_PHB,
                    "Volts (b)", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Phase C Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VoltsPhaseC
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_VH_PHC, m_LID.DEMAND_MAX_V_PHC,
                    "Volts (c)", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Volts squared from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity VoltsSquared
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_V2H_AGG, m_LID.DEMAND_MAX_V2_AGG,
                    "Volts Squared", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Watts Delivered quantity from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity WattsDelivered
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_WH_DEL, m_LID.DEMAND_MAX_W_DEL,
                    "Watts Delivered", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Watts Received quantity from the standard tables
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity WattsReceived
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_WH_REC, m_LID.DEMAND_MAX_W_REC,
                    "Watts Received", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Watts Net quantity from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity WattsNet
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_WH_NET, m_LID.DEMAND_MAX_W_NET,
                    "Watts Net", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Unidirectional Watts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        public override Quantity WattsUni
        {
            get
            {
                return GetQuantityFromStandardTables(m_LID.ENERGY_WH_UNI, m_LID.DEMAND_MAX_W_UNI,
                    "Unidirectional Watts", Table23.CurrentRegisters);
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase A
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/09 RCG 2.10.03 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float InsCurrentPhaseA
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.INST_CURRENT_PHASE_A, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous Current Phase A LID");
                }

                return (float)objValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase B
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/09 RCG 2.10.03 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float InsCurrentPhaseB
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.INST_CURRENT_PHASE_B, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous Current Phase B LID");
                }

                return (float)objValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase C
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/09 RCG 2.10.03 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float InsCurrentPhaseC
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.INST_CURRENT_PHASE_C, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous Current Phase C LID");
                }

                return (float)objValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Voltage for Phase A
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/09 jrf 2.30.16 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float InsVoltagePhaseA
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.INST_VOLTAGE_PHASE_A, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous Voltage Phase A LID");
                }

                return (float)objValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Voltage for Phase B
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/09 jrf 2.30.16 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float InsVoltagePhaseB
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.INST_VOLTAGE_PHASE_B, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous Voltage Phase B LID");
                }

                return (float)objValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Voltage for Phase C
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/09 jrf 2.30.16 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float InsVoltagePhaseC
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.INST_VOLTAGE_PHASE_C, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous Voltage Phase C LID");
                }

                return (float)objValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Watts
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/09 RCG 2.10.03 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float InsW
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.INST_W, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous Watts LID");
                }

                return (float)objValue;
            }
        }

        /// <summary>
        /// Returns whether or not Enhanced Blurts are supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  
        public virtual bool MeterKey_EnhancedBlurtsSupported
        {
            get
            {
                return MeterKeyTable.EnhancedBlurtsSupported;
            }
        }

        /// <summary>
        /// Returns whether or not Advanced Blurts are supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/21/11 mah         n/a	Created
        //  
        public virtual bool MeterKey_AdvancedBlurtsSupported
        {
            get
            {
                return MeterKeyTable.AdvancedBlurtsSupported;
            }
        }

        /// <summary>
        /// Gets the Instantaneous VA Arith
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/09 RCG 2.10.03 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        // 03/11/11 RCG 2.50.08        Making sure that this will get interpreted and read correctly for all FW versions

        public virtual float InsVAArith
        {
            get
            {
                float ActualValue = 0.0f;
                object objValue;
                // Use CreateLID rather than the defined LID to make sure we set the correct data type
                // due to it being incorrect in the firmware for a period of time.
                LID VALid = CreateLID(m_LID.INST_VA_ARITH.lidValue);
                PSEMResponse Result = m_lidRetriever.RetrieveLID(VALid, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous VA Arith LID");
                }

                // Make sure we cast the value to the correct data type
                if (objValue is float)
                {
                    ActualValue = (float)objValue;
                }
                else if (objValue is uint)
                {
                    ActualValue = (uint)objValue;
                }

                return ActualValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous VA Vect
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/09 RCG 2.10.03 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        // 03/11/11 RCG 2.50.08        Making sure that this will get interpreted and read correctly for all FW versions

        public virtual float InsVAVect
        {
            get
            {
                float ActualValue = 0.0f;
                object objValue;
                // Use CreateLID rather than the defined LID to make sure we set the correct data type
                // due to it being incorrect in the firmware for a period of time.
                LID VALid = CreateLID(m_LID.INST_VA_VECT.lidValue);
                PSEMResponse Result = m_lidRetriever.RetrieveLID(VALid, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous VA Vect LID");
                }

                // Make sure we cast the value to the correct data type
                if (objValue is float)
                {
                    ActualValue = (float)objValue;
                }
                else if (objValue is uint)
                {
                    ActualValue = (uint)objValue;
                }

                return ActualValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Var
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/22/11 jrf 2.50.10 N/A    Created

        public virtual float InsVar
        {
            get
            {
                float ActualValue = 0.0f;
                object objValue;
                // Use CreateLID rather than the defined LID to make sure we set the correct data type
                // due to it being incorrect in the firmware for a period of time.
                LID VarLid = CreateLID(m_LID.INST_VAR.lidValue);
                PSEMResponse Result = m_lidRetriever.RetrieveLID(VarLid, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading Instantaneous Var LID");
                }

                // Make sure we cast the value to the correct data type
                if (objValue is float)
                {
                    ActualValue = (float)objValue;
                }
                else if (objValue is uint)
                {
                    ActualValue = (uint)objValue;
                }

                return ActualValue;
            }
        }

        /// <summary>
        /// Gets the last demand reset date.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        public override DateTime DateLastDemandReset
        {
            get
            {
                return DateTimeOfDemandReset(0);
            }
        }

        /// <summary>
        /// Proves access to a list of Self Read Collections
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        public override List<QuantityCollection> SelfReadRegisters
        {
            get
            {
                List<QuantityCollection> SelfReadQtys = new List<QuantityCollection>();
                Quantity Qty;
                uint uiNumSelfReads = Table26.NumberOfValidEntries;

                for (uint uiIndex = 0; uiIndex < uiNumSelfReads; uiIndex++)
                {
                    QuantityCollection SRQuantities = new QuantityCollection();
                    // Add Watts Del
                    Qty = SRWattsDelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Watts Rec
                    Qty = SRWattsReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Watts Net
                    Qty = SRWattsNet(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Watts Uni
                    Qty = SRWattsUni(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add VA Del
                    Qty = SRVADelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add VA Rec
                    Qty = SRVAReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add VA Lag
                    Qty = SRVALagging(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Del
                    Qty = SRVarDelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Rec
                    Qty = SRVarReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Net
                    Qty = SRVarNet(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Net Del
                    Qty = SRVarNetDelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Net Rec
                    Qty = SRVarNetReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Q1
                    Qty = SRVarQuadrant1(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Q2
                    Qty = SRVarQuadrant2(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Q3
                    Qty = SRVarQuadrant3(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Q4
                    Qty = SRVarQuadrant4(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add A (a)
                    Qty = SRAmpsPhaseA(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add A (b)
                    Qty = SRAmpsPhaseB(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add A (c)
                    Qty = SRAmpsPhaseC(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Neutral Amps
                    Qty = SRAmpsNeutral(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add A^2
                    Qty = SRAmpsSquared(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V (a)
                    Qty = SRVoltsPhaseA(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V (b)
                    Qty = SRVoltsPhaseB(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V (c)
                    Qty = SRVoltsPhaseC(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V Avg
                    Qty = SRVoltsAverage(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V^2)
                    Qty = SRVoltsSquared(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add PF
                    Qty = SRPowerFactor(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Q d
                    Qty = SRQDelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Q r
                    Qty = SRQReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }

                    SRQuantities.Quantities.AddRange(SRCoincidentValues(uiIndex));

                    //Add the Time of the Self Read
                    SRQuantities.DateTimeOfReading = DateTimeOfSelfRead(uiIndex);

                    SelfReadQtys.Add(SRQuantities);
                }

                return SelfReadQtys;

            }
        }

        /// <summary>
        /// Builds the list of Event descriptions and returns the dictionary 
        /// </summary>
        /// <returns>
        /// Dictionary of Event Descriptions
        /// </returns> 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/09/07 KRC  8.00.17			Created 
        //  05/22/07 mcm 8.10.05        Changed from method to property and used
        //                              new dictionary class
        public override ANSIEventDictionary EventDescriptions
        {
            get
            {
                if (null == m_dicEventDescriptions)
                {
                    //     m_dicEventDescriptions = new ANSIEventDictionary();
                    m_dicEventDescriptions = (ANSIEventDictionary)(new CENTRON_AMI_EventDictionary());
                }
                return m_dicEventDescriptions;
            }
        }

        /// <summary>
        /// Gets the Disconnect Status
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/09/07 KRC 8.10.13        Determine if the disconnect MeterKey bit is set.
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 		
        public virtual bool MeterKey_DisconnectAvailable
        {
            get
            {
                return MeterKeyTable.DisconnectAvailable;
            }
        }

        /// <summary>
        /// Property to retrieve the Real Pulse Weight in Test mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/25/09 MMD    		Created
        //  07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float GetRealPulseWeightTest
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                float m_RealPulseWeight = 0.0F;
                object Data = null;

                Result = m_lidRetriever.RetrieveLID(m_LID.REAL_COEFF_KT, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    //Convert the data to seconds
                    m_RealPulseWeight = (float)Data;
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the real pulse weight in test mode"));
                }


                return ((m_RealPulseWeight * 10) / 40); // to convert the seconds into minutes
            }
        }

        /// <summary>
        /// Gets the DST enabled flag
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 	09/21/06 KRC  7.30.00		Created
        // 		
        public override bool DSTEnabled
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;

                if (false == m_DSTConfigured.Cached)
                {
                    // We need to retrieve the value from LIDs
                    Result = m_lidRetriever.RetrieveLID(m_LID.DST_CONFIGURED, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        if (0 == Data[0])
                        {
                            m_DSTConfigured.Value = false;
                        }
                        else
                        {
                            m_DSTConfigured.Value = true;
                        }

                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading DST Configured Flag"));
                    }
                }

                return m_DSTConfigured.Value;
            }
        }

        /// <summary>
        /// Gets the amount of time the meter will adjust while in DST 
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 	10/18/10 RCG 2.45.06		Created

        public TimeSpan DSTAdjustAmount
        {
            get
            {
                TimeSpan Offset = new TimeSpan(1, 0, 0);

                if (Table53 != null)
                {
                    Offset = Table53.DSTAdjustAmount;
                }

                return Offset;
            }
        }

        /// <summary>
        /// Returns the number of Last Demand Resets in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/29/06 KRC 7.35.00 N/A    Created
        // 		
        public override uint NumberofLastDemandResets
        {
            get
            {
                return NUM_LAST_DEMAND_RESETS;
            }
        }

        /// <summary>
        /// Retrieves the instantaneous secondary Volts RMS Phase A from the meter.
        /// The firmware folks say this should be considered to be the service voltage.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/25/06 AF  7.40.00 N/A    Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual float ServiceVoltage
        {
            get
            {
                float fltServiceVoltage = 0.0F;
                PSEMResponse Result = PSEMResponse.Ok;
                object objData;

                Result = m_lidRetriever.RetrieveLID(m_LID.SERVICE_VOLTAGE, out objData);

                if (PSEMResponse.Ok == Result)
                {
                    fltServiceVoltage = (float)objData;
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the service voltage"));
                }


                return fltServiceVoltage;
            }
        }

        /// <summary>
        /// Gets the TOU Schedule ID from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ --------------------------------------
        //  01/03/07 RCG 8.00.00 N/A    Created
        //  01/24/07 mrj 8.00.08		Moved to AMI device, this now overrides
        //								the ANSIDevice
        //  06/11/08 KRC 1.50.34 116044 TOU ID doesn't make sense in OpenWay so just indicate it is enabled
        //  03/27/12 RCG 2.53.52 195665 Changing the TOU ID so that it is set to "Enabled" when Tariff ID or Calendar ID is set
        
        public override string TOUScheduleID
        {
            get
            {
                string TOUID = "";
                string TOUIDValue = Table06.TarriffID;
                ushort CalendarID = 0;

                if (Table2048 != null && Table2048.CalendarConfig != null)
                {
                    CalendarID = Table2048.CalendarConfig.CalendarID;
                }

                if (TOUID.Length > 0 || CalendarID > 0)
                {
                    TOUIDValue = "TOU Enabled";
                }
                else
                {
                    TOUID = "";
                }
                return TOUID;
            }
        }


        /// <summary>
        /// Returns an OpenWay device type string based on the meter key settings.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  
        public virtual string MeterKey_QuantityConfiguration
        {
            get
            {
                return MeterKeyTable.QuantityConfiguration;
            }
        }

        /// <summary>
        /// Gets whether or not the meter supports the 25 year DST Calendar
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public virtual bool Supports25YearDST
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the extended outage duration configuration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/28/11 AF  2.51.22 174848 Created
        //
        public virtual OpenWayMFGTable2260ExtendedConfig.ExtendedPowerOutageDurationEnum ExtendedOutageDuration
        {
            get
            {
                return Table2260ExtendedConfig.ExtendedOutageDuration;
            }
        }

        /// <summary>
        /// Determines whether or not the meter is in extended outage recovery mode.
        /// Note: The current code is not a definitive test.  If a meter is configured for 
        /// extended outage recovery and is not configured for load profile, this will return
        /// true even if there hasn't been an extended outage.  See CQ177029.  Gennady is
        /// investigating other ways of getting this information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/28/11 AF  2.51.22 174848 Created
        //  07/22/11 AF  2.51.28 174848 Added a firmware version check
        //
        public bool InExtendedOutageRecovery
        {
            get
            {
                bool bInExtendedOutageRecovery = false;

                if ((HWVersionOnly >= 3) && (VersionChecker.CompareTo(FWRevision, 3.009f) >= 0))
                {
                    OpenWayMFGTable2260ExtendedConfig.ExtendedPowerOutageDurationEnum eDuration = ExtendedOutageDuration;

                    if (eDuration != OpenWayMFGTable2260ExtendedConfig.ExtendedPowerOutageDurationEnum.EXTENDED_POWER_OUTAGE_OFF)
                    {
                        if (!LPRunning)
                        {
                            // If the meter is configured for extended outage recovery and load profile is not running, assume
                            // the meter is in extended outage recovery mode
                            bInExtendedOutageRecovery = true;
                        }
                    }
                }

                return bInExtendedOutageRecovery;
            }
        }

        /// <summary>
        /// Gets the load voltage detection delay
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/11 MAH  2.53.14       Created
        //
        public virtual ushort LoadVoltageDetectionDelay
        {
            get
            {
                ushort usLVDDelay = 65535;  // Disabled by default

                if (VersionChecker.CompareTo(FWRevision, 3.012f) >= 0) // Only present in Lithium or greater
                {
                    usLVDDelay = Table2260ExtendedConfig.LoadVoltageDetectionDelay;
                }

                return usLVDDelay;
            }
        }


        /// <summary>
        /// Gets whether the firmware download event log is supported in the meter
        /// by checking Table 00 to see if the table is used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 AF  2.51.33        Created
        //
        public virtual bool FWDLLogSupported
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Static Public (Translation Method)

        /// <summary>
        /// Translates the security validation result into a string
        /// </summary>
        /// <param name="result">procedure result code</param>
        /// <returns>the status translated into a string</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/29/09 MMD  2.30.02        Created
        //

        public static string TranslateSecurityValidationResult(ProcedureResultCodes result)
        {
            string transResult = "";
            switch (result)
            {
                case ProcedureResultCodes.NO_AUTHORIZATION:
                transResult = "Insufficient Security Clearance";
                break;
                case ProcedureResultCodes.COMPLETED:
                transResult = "Passed";
                break;
                default:
                transResult = "Failed";
                break;

            }

            return transResult;
        }

        /// <summary>
        /// Method that takes the Daily Self Read Byte and returns a string
        ///   containing the Daily Self Read time in human readable format.
        /// </summary>
        /// <param name="byDailySelfRead">The Byte from the Meter that has the values</param>
        /// <returns>string - Daily Self Read Time</returns>
        public static string DetermineDailySelfRead(byte byDailySelfRead)
        {
            return CENTRON_AMI_ModeControl.DetermineDailySelfRead(byDailySelfRead);
        }

        /// <summary>
        /// Translate the Load Limiting value to something that is human readable
        /// </summary>
        /// <param name="iDemandControl">Demand Control Value from device</param>
        /// <returns>string - human readable result</returns>
        public static string TranslateLoadLimitingConnectWithoutUserIntervetion(int iDemandControl)
        {
            string strResult = "";
            ResourceManager rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, typeof(CENTRON_AMI).Assembly);

            if ((LOAD_CONTROL_RECONNECT & iDemandControl) == LOAD_CONTROL_RECONNECT)
            {
                strResult = rmStrings.GetString("USER_INTERVENTION_NOT_REQUIRED");
            }
            else
            {
                strResult = rmStrings.GetString("USER_INTERVENTION_REQUIRED");
            }

            return strResult;
        }

        /// <summary>
        /// Tranlate the Load Control Threshold to a human readable format
        /// </summary>
        /// <param name="fLoadControlThreshold">Load Control value</param>
        /// <returns>string - human readable</returns>
        public static string TranslateLoadControlDisconnectThreshold(float fLoadControlThreshold)
        {
            string strResult = "";

            // This value is set to zero if Load Control is not enabled in the meter.
            if (fLoadControlThreshold <= 0.0)
            {
                strResult = "Not Enabled";
            }
            else
            {
                // Need to get this value to kilo
                fLoadControlThreshold = fLoadControlThreshold / 1000;
                strResult = fLoadControlThreshold.ToString("F2", CultureInfo.CurrentCulture) + " kW";
            }

            return strResult;
        }

        #endregion

        #region Internal Property

        /// <summary>
        /// The Base Date for calculating dates in the AMI Device
        /// </summary>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/30/06 KRC 7.35.00 N/A    Created
        //
        internal override DateTime MeterReferenceTime
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            }
        }

        /// <summary>
        /// The Base Date for calculating dates in the Configuration Portion (Table 2048) of AMI Device
        /// </summary>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/28/08 KRC 2.00.03 00121848    Created
        //
        internal DateTime MeterConfigurationReferenceTime
        {
            get
            {
                return new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
            }
        }
        /// <summary>
        /// The Base Date for calculating dates in the AMI Device when the
        /// time comes back as UTC.
        /// </summary>
        /// <returns></returns>
        //	Revision History	
        //	MM/DD/YY who Version Issue# Description
        //	-------- --- ------- ------ ---------------------------------------
        //  05/14/08 mrj 1.50.25		Created for itron00107508
        // 
        internal override DateTime UTCMeterReferenceTime
        {
            get
            {
                DateTime dtRefTime = MeterReferenceTime;
                dtRefTime = DateTime.SpecifyKind(dtRefTime, DateTimeKind.Utc);

                return dtRefTime;
            }
        }

        /// <summary>
        /// Gets the Table 04 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        //
        internal CTable04 Table04
        {
            get
            {
                if (null == m_Table04)
                {
                    m_Table04 = new CTable04(m_PSEM, Table00.DimStdTablesUsed,
                                    Table00.DimMfgTablesUsed, Table00.NumberPending);
                }

                return m_Table04;
            }
        }

        /// <summary>
        /// Gets the Table 11 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable11 Table11
        {
            get
            {
                if (null == m_Table11)
                {
                    m_Table11 = new StdTable11(m_PSEM);
                }

                return m_Table11;
            }
        }

        /// <summary>
        /// Gets the Table 14 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable14 Table14
        {
            get
            {
                if (null == m_Table14)
                {
                    m_Table14 = new StdTable14(m_PSEM, Table11);
                }

                return m_Table14;
            }
        }

        /// <summary>
        /// Gets the Table 21 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable21 Table21
        {
            get
            {
                if (null == m_Table21)
                {
                    m_Table21 = new StdTable21(m_PSEM);
                }

                return m_Table21;
            }
        }

        /// <summary>
        /// Gets the Table 22 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable22 Table22
        {
            get
            {
                // If the table has expired, we need to rebuild it because its size may have changed.
                if (Table21.State == AnsiTable.TableState.Expired)
                {
                    m_Table22 = null;
                }

                if (null == m_Table22)
                {
                    m_Table22 = new StdTable22(m_PSEM, Table21);
                }

                return m_Table22;
            }
        }

        /// <summary>
        /// Gets the Table 23 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable23 Table23
        {
            get
            {
                // If the table has expired, we need to rebuild it because its size may have changed.
                if (Table21.State == AnsiTable.TableState.Expired)
                {
                    m_Table23 = null;
                }

                if (null == m_Table23)
                {
                    m_Table23 = new StdTable23(m_PSEM, Table00, Table21);
                }

                return m_Table23;
            }
        }

        /// <summary>
        /// Gets the Table 24 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable24 Table24
        {
            get
            {
                // If the table has expired, we need to rebuild it because its size may have changed.
                if (Table21.State == AnsiTable.TableState.Expired)
                {
                    m_Table24 = null;
                }

                if (null == m_Table24)
                {
                    m_Table24 = new StdTable24(m_PSEM, Table00, Table21);
                }

                return m_Table24;
            }
        }

        /// <summary>
        /// Gets the Table 25 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable25 Table25
        {
            get
            {
                // If the table has expired, we need to rebuild it because its size may have changed.
                if (Table21.State == AnsiTable.TableState.Expired)
                {
                    m_Table25 = null;
                }

                if (null == m_Table25)
                {
                    m_Table25 = new StdTable25(m_PSEM, Table00, Table21);
                }

                return m_Table25;
            }
        }

        /// <summary>
        /// Gets the Table 26 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable26 Table26
        {
            get
            {
                // If the table has expired, we need to rebuild it because its size may have changed.
                if (Table21.State == AnsiTable.TableState.Expired)
                {
                    m_Table23 = null;
                }

                if (null == m_Table26)
                {
                    m_Table26 = new StdTable26(m_PSEM, Table00, Table21);
                }

                return m_Table26;
            }
        }

        /// <summary>
        /// Gets the Table 27 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable27 Table27
        {
            get
            {
                if (null == m_Table27)
                {
                    m_Table27 = new StdTable27(m_PSEM, Table21);
                }

                return m_Table27;
            }
        }

        /// <summary>
        /// Gets the Table 28 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable28 Table28
        {
            get
            {
                // If the table has expired, we need to rebuild it because its size may have changed.
                if (Table21.State == AnsiTable.TableState.Expired)
                {
                    m_Table28 = null;
                }

                if (null == m_Table28)
                {
                    m_Table28 = new StdTable28(m_PSEM, Table00, Table21);
                }

                return m_Table28;
            }
        }

        /// <summary>
        /// Gets the Table 61 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable61 Table61
        {
            get
            {
                if (null == m_Table61)
                {
                    m_Table61 = new StdTable61(m_PSEM, Table00);
                }

                return m_Table61;
            }
        }

        /// <summary>
        /// Gets the Table 62 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 12/12/11 RCG 2.53.20        Adding support for Instrumentation Profile Data

        internal virtual StdTable62 Table62
        {
            get
            {
                if (null == m_Table62)
                {
                    m_Table62 = new StdTable62(m_PSEM, Table00, Table61, Table14);
                }

                return m_Table62;
            }
        }

        /// <summary>
        /// Gets the Table 63 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable63 Table63
        {
            get
            {
                if (null == m_Table63)
                {
                    m_Table63 = new StdTable63(m_PSEM, Table00);
                }

                return m_Table63;
            }
        }
        /// <summary>
        /// Gets the Table 64 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        internal virtual StdTable64 Table64
        {
            get
            {
                if (null == m_Table64 && Table00.IsTableUsed(64))
                {
                    m_Table64 = new StdTable64(m_PSEM, Table00, Table61, Table62);
                }

                return m_Table64;
            }
        }

        //<summary>
        //Gets the object for MFG table 2139
        //</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/16/08 RCG 1.50.22 N/A    Created
        // 10/07/10 MMD  1.0.0  N/A    Added Virtual for Max Image

        internal virtual OpenWayMFGTable2140 Table2140
        {
            get
            {
                if (null == m_Table2140 && Table00.IsTableUsed(2140))
                {
                    if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5) < 0)
                    {
                        m_Table2140 = new OpenWayMFGTable2140(m_PSEM, Table00);
                    }
                    else
                    {
                        m_Table2140 = new OpenWayMFGTable2140SP5(m_PSEM, Table00);
                    }
                }

                return m_Table2140;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Creates a LID object from the given 32-bit number
        /// </summary>
        /// <param name="uiLIDNumber">The 32-bit number that represents the LID</param>
        /// <returns>The LID object for the specified LID</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/07/07 RCG 8.00.11 N/A    Created

        internal override LID CreateLID(uint uiLIDNumber)
        {
            return new CentronAMILID(uiLIDNumber);
        }

        /// <summary>
        /// Creates a new DisplayItem
        /// </summary>
        /// <returns>ANSIDisplayItem</returns>
        // Revision History	
        // MM/DD/YY who Version  Issue# Description
        // -------- --- -------- ------ ---------------------------------------
        // 04/02/08 RCG 10.00.00        Created

        internal override ANSIDisplayItem CreateDisplayItem(LID lid, string strDisplayID, ushort usFormat, byte byDim)
        {
            ANSIDisplayItem DispItem = new OpenWayDisplayItem(lid, strDisplayID, usFormat, byDim);
            DispItem = ModifyLID(DispItem);

            return DispItem;
        }

        /// <summary>
        /// Handle any irregular formatting for display items.
        /// </summary>
        /// <param name="Item">The display item to check</param>
        /// <param name="objData">The value of the data.</param>
        /// <returns>True if the item was handled. False otherwise.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/22/09 RCG 2.20.08 136629 Created

        internal override bool HandleIrregularFormatting(ANSIDisplayItem Item, object objData)
        {
            bool bResult = false;
            uint uiLIDValue = Item.DisplayLID.lidValue;

            switch (uiLIDValue)
            {
                // The Last Programmed date uses the 2000 reference date not 1970
                case ((uint)DefinedLIDs.BaseLIDs.MISC_DATA | (uint)DefinedLIDs.Misc_Data.MISC_LAST_CONFIG_TIME):
                {
                    // We need to convert the Date/Time items from Uint32 to DateTime
                    DateTime ReferenceTime = new DateTime(2000, 1, 1);
                    DateTime dtDateTime = ReferenceTime;

                    dtDateTime = dtDateTime.AddSeconds((uint)objData);

                    // If the time is still the reference date we want to display 00-00-00
                    if (dtDateTime != ReferenceTime)
                    {
                        Item.FormatDateTime(dtDateTime);
                    }
                    else
                    {
                        // Set the string to match the display since the reference date is 1/1/2000
                        if (Item.DisplayFormat == (ushort)ANSIDisplayItem.DisplayType.TIME_HH_MM_SS)
                        {
                            Item.Value = "00:00:00";
                        }
                        else
                        {
                            Item.Value = "00-00-00";
                        }
                    }

                    bResult = true;
                    break;
                }
                default:
                {
                    bResult = base.HandleIrregularFormatting(Item, objData);
                    break;
                }
            }

            return bResult;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the multiplier used to calculate the Load Profile Pulse Weight
        /// </summary>		
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/11/07 KRC 8.00.27 2864   Created
        //
        protected override float LPPulseWeightMultiplier
        {
            get
            {
                return 0.01f;
            }
        }
        /// <summary>
        /// Determines the length of the Full Core Dump based on the Core Dump Map
        /// </summary>
        /// <returns>The length of the core dump in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/03/10 RCG 2.40.23 N/A    Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  02/25/11 AF  2.50.05        Removed virtual - not needed
        //
        protected uint DetermineFullCoreDumpLength()
        {
            uint uiCoreDumpLength = 0;

            // We can only determine the size if the Full Core Dump is present
            if (IsFullCoreDumpPresent)
            {
                List<CoreDumpMapItem> MapItems = Table3043Map.MapItems;

                foreach (CoreDumpMapItem CurrentMapItem in MapItems)
                {
                    if (CurrentMapItem != null && (CurrentMapItem.Offset + CurrentMapItem.Length > uiCoreDumpLength))
                    {
                        uiCoreDumpLength = CurrentMapItem.Offset + CurrentMapItem.Length;
                    }
                }

                // The offsets in the map section are relative to the first block after the Map so we need to add 768 bytes
                uiCoreDumpLength += 768;
            }

            return uiCoreDumpLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        protected bool? IsTimeZoneApplied
        {
            get
            {
                return Table52.IsTimeZoneApplied;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        protected bool? IsDSTApplied
        {
            get
            {
                return Table52.IsDSTApplied;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        protected bool? IsGMT
        {
            get
            {
                return Table52.IsGMT;
            }
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the Self Read values for Neutral Amps
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRAmpsNeutral(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_AH_NEUTRAL, m_LID.DEMAND_MAX_A_NEUTRAL,
                    "Neutral Amps", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Amps (a)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRAmpsPhaseA(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_AH_PHA, m_LID.DEMAND_MAX_A_PHA,
                    "Amps (a)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Amps (b)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRAmpsPhaseB(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_AH_PHB, m_LID.DEMAND_MAX_A_PHB,
                    "Amps (b)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Amps (c)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRAmpsPhaseC(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_AH_PHC, m_LID.DEMAND_MAX_A_PHC,
                    "Amps (c)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Amps Squared
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRAmpsSquared(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_I2H_AGG, m_LID.DEMAND_MAX_I2_AGG,
                    "Amps Squared", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Power Factor
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRPowerFactor(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                // There is not PF energy so just check the demand
                DRQuantity = GetQuantityFromStandardTables(null, m_LID.DEMAND_MIN_PF_INTERVAL_ARITH,
                    "Power Factor", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);

                // Also try the vertoral PF
                if (DRQuantity == null)
                {
                    DRQuantity = GetQuantityFromStandardTables(null, m_LID.DEMAND_MIN_PF_INTERVAL_VECT,
                        "Power Factor", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
                }
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Q Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRQDelivered(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_QH_DEL, m_LID.DEMAND_MAX_Q_DEL,
                    "Q Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Q Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRQReceived(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_QH_REC, m_LID.DEMAND_MAX_Q_REC,
                    "Q Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for VA Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVADelivered(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                // Try getting Arithmatic first.
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_DEL_ARITH, m_LID.DEMAND_MAX_VA_DEL_ARITH,
                    "VA Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);

                // Try  getting Vectoral
                if (DRQuantity == null)
                {
                    DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_DEL_VECT, m_LID.DEMAND_MAX_VA_DEL_VECT,
                        "VA Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
                }
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for VA Lagging
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVALagging(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_LAG, m_LID.DEMAND_MAX_VA_LAG,
                    "VA Lagging", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarDelivered(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_DEL, m_LID.DEMAND_MAX_VAR_DEL,
                    "Var Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for VA Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVAReceived(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                // Try getting Arithmatic first.
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_REC_ARITH, m_LID.DEMAND_MAX_VA_REC_ARITH,
                    "VA Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);

                // Try  getting Vectoral
                if (DRQuantity == null)
                {
                    DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_REC_VECT, m_LID.DEMAND_MAX_VA_REC_VECT,
                        "VA Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
                }
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Net.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarNet(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET, m_LID.DEMAND_MAX_VAR_NET,
                    "Var Net", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Net Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarNetDelivered(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET_DEL, m_LID.DEMAND_MAX_VAR_NET_DEL,
                    "Var Net Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Net Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarNetReceived(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET_REC, m_LID.DEMAND_MAX_VAR_NET_REC,
                    "Var Net Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Q1.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarQuadrant1(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q1, m_LID.DEMAND_MAX_VAR_Q1,
                    "Var Quadrant 1", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Q2.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarQuadrant2(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q2, m_LID.DEMAND_MAX_VAR_Q2,
                    "Var Quadrant 2", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Q3.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarQuadrant3(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q3, m_LID.DEMAND_MAX_VAR_Q3,
                    "Var Quadrant 3", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Q4.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarQuadrant4(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q4, m_LID.DEMAND_MAX_VAR_Q4,
                    "Var Quadrant 4", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Received.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVarReceived(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_REC, m_LID.DEMAND_MAX_VAR_REC,
                    "Var Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts Average.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVoltsAverage(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VH_AVG, m_LID.DEMAND_MAX_V_AVG,
                    "Volts Average", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts (a).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVoltsPhaseA(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VH_PHA, m_LID.DEMAND_MAX_V_PHA,
                    "Volts (a)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts(b).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVoltsPhaseB(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VH_PHB, m_LID.DEMAND_MAX_V_PHB,
                    "Volts (b)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts (c).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVoltsPhaseC(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VH_PHC, m_LID.DEMAND_MAX_V_PHC,
                    "Volts (c)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts Squared.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRVoltsSquared(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_V2H_AGG, m_LID.DEMAND_MAX_V2_AGG,
                    "Volts Squared", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Watts Delivered.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRWattsDelivered(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_DEL, m_LID.DEMAND_MAX_W_DEL,
                    "Watts Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Watts Net.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRWattsNet(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_NET, m_LID.DEMAND_MAX_W_NET,
                    "Watts Net", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Watts Received.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRWattsReceived(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_REC, m_LID.DEMAND_MAX_W_REC,
                    "Watts Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Unidirectional Watts.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override Quantity SRWattsUni(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_UNI, m_LID.DEMAND_MAX_W_UNI,
                    "Unidirectional Watts", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return DRQuantity;
        }

        /// <summary>
        /// Gets the Self Read date for the specified index.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The date of the Self Read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override DateTime DateTimeOfSelfRead(uint uiIndex)
        {
            DateTime SRDate = MeterReferenceTime;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRDate = Table26.SelfReadEntries[uiIndex].SelfReadDate;
            }

            return SRDate;
        }
        /// <summary>
        /// Gets the Demand Reset date for the specified index.
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The date of the demand reset.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected override DateTime DateTimeOfDemandReset(uint uiIndex)
        {
            DateTime DRDate = MeterReferenceTime;

            if (uiIndex == 0)
            {
                DRDate = Table25.DemandResetDate;
            }
#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRDate = Table2053.DemandResetDate;
            }
#endif
            return DRDate;
        }


        /// <summary>
        /// Gets the demand reset value for VA d
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        protected override Quantity DRVADelivered(uint uiIndex)
        {
            Quantity VA = null;

            if (uiIndex == 0)
            {
                // Try getting Arithmatic first.
                VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_DEL_ARITH, m_LID.DEMAND_MAX_VA_DEL_ARITH,
                    "VA Delivered", Table25.DemandResetRegisterData);

                // Try  getting Vectoral
                if (VA == null)
                {
                    VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_DEL_VECT, m_LID.DEMAND_MAX_VA_DEL_VECT,
                        "VA Delivered", Table25.DemandResetRegisterData);
                }
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                // Try getting Arithmatic first.
                VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_DEL_ARITH, m_LID.DEMAND_MAX_VA_DEL_ARITH,
                    "VA Delivered", Table2053.DemandResetRegisterData);

                // Try  getting Vectoral
                if (VA == null)
                {
                    VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_DEL_VECT, m_LID.DEMAND_MAX_VA_DEL_VECT,
                        "VA Delivered", Table2053.DemandResetRegisterData);
                }
            }
#endif
            return VA;
        }

        /// <summary>
        /// Gets the demand reset value for VA r
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        protected override Quantity DRVAReceived(uint uiIndex)
        {
            Quantity VA = null;

            if (uiIndex == 0)
            {
                // Try getting Arithmatic first.
                VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_REC_ARITH, m_LID.DEMAND_MAX_VA_REC_ARITH,
                    "VA Received", Table25.DemandResetRegisterData);

                // Try  getting Vectoral
                if (VA == null)
                {
                    VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_REC_VECT, m_LID.DEMAND_MAX_VA_REC_VECT,
                        "VA Received", Table25.DemandResetRegisterData);
                }
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                // Try getting Arithmatic first.
                VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_REC_ARITH, m_LID.DEMAND_MAX_VA_REC_ARITH,
                    "VA Received", Table2053.DemandResetRegisterData);

                // Try  getting Vectoral
                if (VA == null)
                {
                    VA = GetQuantityFromStandardTables(m_LID.ENERGY_VAH_REC_VECT, m_LID.DEMAND_MAX_VA_REC_VECT,
                        "VA Received", Table2053.DemandResetRegisterData);
                }
            }
#endif
            return VA;
        }

        /// <summary>
        /// Gets the demand reset value for Volts (a)
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        protected override Quantity DRVoltsPhaseA(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VH_PHA, m_LID.DEMAND_MAX_V_PHA,
                    "Volts (a)", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VH_PHA, m_LID.DEMAND_MAX_V_PHA,
                    "Volts (a)", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Gets the demand reset value for Wh d
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        protected override Quantity DRWattsDelivered(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_DEL, m_LID.DEMAND_MAX_W_DEL,
                    "Watts Delivered", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_DEL, m_LID.DEMAND_MAX_W_DEL,
                    "Watts Delivered", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;

        }

        /// <summary>
        /// Gets the demand reset value for Wh Net
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        protected override Quantity DRWattsNet(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_NET, m_LID.DEMAND_MAX_W_NET,
                    "Watts Net", Table25.DemandResetRegisterData);
            }
#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_NET, m_LID.DEMAND_MAX_W_NET,
                    "Watts Net", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Gets the demand reset value for Wh r
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        protected override Quantity DRWattsReceived(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_REC, m_LID.DEMAND_MAX_W_REC,
                    "Watts Received", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_REC, m_LID.DEMAND_MAX_W_REC,
                    "Watts Received", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Gets the demand reset value for Wh
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created
        // 12/09/09 jrf 2.30.26 146333 Correcting to pull this value from the demand reset 
        //                             register data table.
        protected override Quantity DRWattsUni(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_UNI, m_LID.DEMAND_MAX_W_UNI,
                    "Unidirectional Watts", Table25.DemandResetRegisterData);
            }
#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_WH_UNI, m_LID.DEMAND_MAX_W_UNI,
                    "Unidirectional Watts", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        #endregion

        #region Static Property Methods

        /// <summary>
        /// Translate the response from Configure
        /// </summary>
        /// <param name="ConfigError">ConfigurationError - Result from AMIConfigureDevice.Configure</param>
        /// <returns>ConfigurationResult - Result expect from call to our method</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/08/10 AF  2.41.06        Changed access level so that Gateway can use it.
        //
        protected static ConfigurationResult TranslateConfigError(ConfigurationError ConfigError)
        {
            switch (ConfigError)
            {
                case ConfigurationError.SUCCESS:
                {
                    return ConfigurationResult.SUCCESS;
                }
                case ConfigurationError.FILE_NOT_FOUND:
                {
                    return ConfigurationResult.PROGRAM_NOT_FOUND;
                }
                case ConfigurationError.INVALID_PROGRAM:
                {
                    return ConfigurationResult.PROGRAM_NOT_VALID;
                }
                case ConfigurationError.USER_ABORT:
                {
                    return ConfigurationResult.USER_ABORT;
                }
                case ConfigurationError.SECURITY_ERROR:
                {
                    return ConfigurationResult.SECURITY_ERROR;
                }
                case ConfigurationError.TIMEOUT:
                {
                    return ConfigurationResult.IO_TIMEOUT;
                }
                case ConfigurationError.COMMUNICATION_ERROR:
                {
                    return ConfigurationResult.NETWORK_ERROR;
                }
                case ConfigurationError.DATA_LOCKED:
                case ConfigurationError.DATA_NOT_READY:
                case ConfigurationError.DEVICE_BUSY:
                case ConfigurationError.INNAPROPRIATE_ACTION_REQUEST:
                case ConfigurationError.INVALID_SERVICE_SEQUENCE_STATE:
                case ConfigurationError.OPERATION_NOT_POSSIBLE:
                case ConfigurationError.RENEGOTIATE_REQUEST:
                case ConfigurationError.SERVICE_NOT_SUPPORTED:
                {
                    return ConfigurationResult.PROTOCOL_ERROR;
                }
                default:
                {
                    return ConfigurationResult.ERROR;
                }
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the Program State table and creates it if needed
        /// This will return null if the table is not supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/11/10 jrf 2.41.01        Created
        //
        private OpenWayMFGTable2264 Table2264
        {
            get
            {
                if (null == m_Table2264 && Table00.IsTableUsed(2264))
                {
                    m_Table2264 = new OpenWayMFGTable2264(m_PSEM);
                }

                return m_Table2264;
            }
        }

        /// <summary>
        /// Gets the object for MFG table 2139
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.22 N/A    Created

        private OpenWayMFGTable2139 Table2139
        {
            get
            {
                if (null == m_Table2139 && Table00.IsTableUsed(2139))
                {
                    m_Table2139 = new OpenWayMFGTable2139(m_PSEM);
                }

                return m_Table2139;
            }
        }


        /// <summary>
        /// Gets the Factory Data Information Table and creates it if needed. Returns null if this
        /// table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/16/10 RCG 2.40.25 N/A    Created

        private OpenWayMFGTable2220 Table2220
        {
            get
            {
                if (m_Table2220 == null && Table00.IsTableUsed(2220))
                {
                    m_Table2220 = new OpenWayMFGTable2220(m_PSEM);
                }

                return m_Table2220;
            }
        }

        /// <summary>
        /// Gets the object for MFG table 2139
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.22 N/A    Created

        private OpenWayMFGTable2141 Table2141
        {
            get
            {
                if (null == m_Table2141 && Table00.IsTableUsed(2141))
                {
                    m_Table2141 = new OpenWayMFGTable2141(m_PSEM, Table00, Table2139);
                }

                return m_Table2141;
            }
        }

        /// <summary>
        /// Gets the object for MFG table 2139
        /// </summary>

        private OpenWayMFGTable2142 Table2142
        {
            get
            {
                if (null == m_Table2142 && Table00.IsTableUsed(2142))
                {
                    m_Table2142 = new OpenWayMFGTable2142(m_PSEM);
                }

                return m_Table2142;
            }
        }

        ///<summary>
        ///Gets the object for MFG table 2053
        ///</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created

        protected MFGTable2053 Table2053
        {
            get
            {
                if (null == m_Table2053 && Table00.IsTableUsed(2053))
                {
                    m_Table2053 = new MFGTable2053(m_PSEM, Table00, Table21);
                }

                return m_Table2053;
            }
        }

        ///<summary>
        ///Gets the object for MFG table 2053
        ///</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created

        protected OpenWayMFGTable2260DSTCalendar Table2260DSTConfig
        {
            get
            {
                if (null == m_Table2260DSTConfig && Table00.IsTableUsed(2260))
                {
                    m_Table2260DSTConfig = new OpenWayMFGTable2260DSTCalendar(m_PSEM);
                }

                return m_Table2260DSTConfig;
            }
        }

        /// <summary>
        /// Gets the SR3 Config table and creates it if needed.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        private OpenWayMFGTable2260SR30Config Table2260SR30Config
        {
            get
            {
                if (null == m_Table2260SR30Config && Table00.IsTableUsed(2260))
                {
                    m_Table2260SR30Config = new OpenWayMFGTable2260SR30Config(m_PSEM);
                }

                return m_Table2260SR30Config;
            }
        }

        /// <summary>
        /// Gets the Extended Outage Config subtable and creates it if needed.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/27/11 AF  2.51.15 174848 Created
        //
        private OpenWayMFGTable2260ExtendedConfig Table2260ExtendedConfig
        {
            get
            {
                if (null == m_Table2260ExtendedConfig && Table00.IsTableUsed(2260))
                {
                    m_Table2260ExtendedConfig = new OpenWayMFGTable2260ExtendedConfig(m_PSEM);
                }

                return m_Table2260ExtendedConfig;
            }
        }

        /// <summary>
        /// Gets the Fatal Error Recovery Status table and creates it if needed.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/28/10 RCG 2.40.10 N/A    Created

        private OpenWayMFGTable2261 Table2261
        {
            get
            {
                if (null == m_Table2261 && Table00.IsTableUsed(2261))
                {
                    m_Table2261 = new OpenWayMFGTable2261(m_PSEM);
                }

                return m_Table2261;
            }
        }

        /// <summary>
        /// Gets the Tamper Tap Status table and creates it if needed
        /// This will return null if the table is not supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        private OpenWayMFGTable2262 Table2262
        {
            get
            {
                if (null == m_Table2262 && Table00.IsTableUsed(2262))
                {
                    m_Table2262 = new OpenWayMFGTable2262(m_PSEM);
                }

                return m_Table2262;
            }
        }

        /// <summary>
        /// Gets the Tamper Tap Data table and creates it if needed
        /// This will return null if the table is not supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/10 AF  2.40.11        Created
        //
        private OpenWayMFGTable2263 Table2263
        {
            get
            {
                if (null == m_Table2263 && Table00.IsTableUsed(2263))
                {
                    m_Table2263 = new OpenWayMFGTable2263(m_PSEM);
                }

                return m_Table2263;
            }
        }

        /// <summary>
        /// Gets the CPP table and creates it if needed
        /// This will return null if the table is not supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  13/13/10 JB                 Created
        //
        private OpenWayMFGTable2360 Table2360
        {
            get
            {
                if (null == m_Table2360 && Table00.IsTableUsed(2360))
                {
                    m_Table2360 = new OpenWayMFGTable2360(m_PSEM, Table00);
                }

                return m_Table2360;
            }
        }

        /// <summary>
        /// Gets the sub table for the Core Dump Header and creates it if needed.
        /// This property returns null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/04/10 RCG 2.40.11 N/A    Created

        private OpenWayMFGTable3043Header Table3043Header
        {
            get
            {
                if (m_Table3043Header == null)
                {
                    m_Table3043Header = new OpenWayMFGTable3043Header(m_PSEM);
                }

                return m_Table3043Header;
            }
        }

        /// <summary>
        /// Gets the sub table for the Core Dump Map and creates it if needed.
        /// This property returns null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/10 RCG 2.40.23 N/A    Created

        private OpenWayMFGTable3043Map Table3043Map
        {
            get
            {
                if (m_Table3043Map == null && IsFullCoreDumpPresent)
                {
                    m_Table3043Map = new OpenWayMFGTable3043Map(m_PSEM, Table3043Info);
                }

                return m_Table3043Map;
            }
        }

        /// <summary>
        /// Gets the sub table for the Core Dump Info and creates it if needed.
        /// This property returns null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/10 RCG 2.40.23 N/A    Created

        private OpenWayMFGTable3043Info Table3043Info
        {
            get
            {
                if (m_Table3043Info == null && IsFullCoreDumpPresent)
                {
                    m_Table3043Info = new OpenWayMFGTable3043Info(m_PSEM);
                }

                return m_Table3043Info;
            }
        }

        /// <summary>
        /// Gets the Table 51 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        //
        private StdTable51 Table51
        {
            get
            {
                if (null == m_Table51)
                {
                    m_Table51 = new StdTable51(m_PSEM);
                }

                return m_Table51;
            }
        }

        /// <summary>
        /// Gets the Table 52 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        //
        private StdTable52 Table52
        {
            get
            {
                if (null == m_Table52)
                {
                    m_Table52 = new StdTable52(m_PSEM, Table00);
                }

                return m_Table52;
            }
        }

        /// <summary>
        /// Gets the Table 53 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        //
        private StdTable53 Table53
        {
            get
            {
                if (null == m_Table53)
                {
                    m_Table53 = new StdTable53(m_PSEM, Table00, Table51);
                }

                return m_Table53;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2108 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        // 11/27/06 AF  8.00.00         Changed the class name to match other
        //                              HAN Mfg tables
        // 03/16/10 AF  2.40.25         Removed the firmware build parameter from the constructor
        //
        private OpenWayMfgTable2108 Table2108
        {
            get
            {
                if (null == m_Table2108)
                {
                    m_Table2108 = new OpenWayMfgTable2108(m_PSEM);
                }

                return m_Table2108;
            }
        }


        #endregion

        #region Private Methods

#if (!WindowsCE)
        /// <summary>
        /// Computes the SHA256 hash of the key passed in
        /// </summary>
        /// <param name="strKey">the security key to hash</param>
        /// <param name="iKeyLength">
        /// The maximum length of the key.
        /// </param>
        /// <returns>The hashed key</returns>

        private byte[] CreateHash(string strKey, int iKeyLength)
        {
            byte[] data = new byte[iKeyLength];

            for (int i = 0; i < strKey.Length; i++)
            {
                data[i] = Convert.ToByte((char)strKey[i]);
            }

            for (int i = strKey.Length; i < iKeyLength; i++)
            {
                data[i] = 0;
            }

            return CreateHash(data);
        }

        /// <summary>
        /// Computes the SHA256 hash of the key passed in
        /// </summary>
        /// <param name="Key">The key to hash</param>
        /// <returns>The hashed key</returns>

        private byte[] CreateHash(byte[] Key)
        {
            byte[] hashedKey = new byte[32];
            SHA256 shaM = new SHA256Managed();

            hashedKey = shaM.ComputeHash(Key);

            return hashedKey;
        }
#endif

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
        private static byte[] GetSecurityCode(string ProgName, CENTRON_AMI.SecurityType SecType, CENTRON_AMI.SecurityKeyID PasswordLevel)
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
                int iIndex = 0;

                // if Security Type is DES extract DES keys from table 46 or else 
                // extract optical passwords form table 42

                if (SecType == CENTRON_AMI.SecurityType.C1222_KEYS)
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
                            objValue = filevalue;
                        }
                        else
                        {
                            byte[] realValue = new byte[filevalue.Length];
                            objValue = realValue;

                        }
                    }

                }
                else if (SecType == CENTRON_AMI.SecurityType.C1218_PASSWORDS)
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

        /// <summary>
        /// Finds the index of the source if supported by the meter.
        /// </summary>
        /// <param name="sourceLID">The quantity to search for.</param>
        /// <returns>Null if the quantity is not supported or the index into the source definition if supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private int? FindEnergySourceIndex(LID sourceLID)
        {
            uint[] SourceIDs = Table14.SourceIDs;
            int? SourceIndex = null;

            // Check to see if the quantity is supported.
            for (int iSourceIndex = 0; iSourceIndex < SourceIDs.Length; iSourceIndex++)
            {
                // Table 14 stores the raw LID values but we use secondary everywhere else so we need to convert.
                if ((SourceIDs[iSourceIndex] | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA) == sourceLID.lidValue)
                {
                    SourceIndex = iSourceIndex;
                }
            }

            return SourceIndex;
        }

        /// <summary>
        /// Finds the index of the source if supported by the meter.
        /// </summary>
        /// <param name="sourceLID">The quantity to search for.</param>
        /// <returns>Null if the quantity is not supported or the index into the source definition if supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private int? FindDemandSourceIndex(LID sourceLID)
        {
            uint[] SourceIDs = Table14.SourceIDs;
            int? SourceIndex = null;
            uint MaxDemand;
            uint MinDemand;

            // Check to see if the quantity is supported.
            for (int iSourceIndex = 0; iSourceIndex < SourceIDs.Length; iSourceIndex++)
            {
                MaxDemand = SourceIDs[iSourceIndex] | (uint)DefinedLIDs.Demand_Data.MAX_DEMAND | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA;
                MinDemand = SourceIDs[iSourceIndex] | (uint)DefinedLIDs.Demand_Data.MIN_DEMAND | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA;

                // Table 14 stores the raw LID values but we use secondary everywhere else so we need to convert.
                if (MaxDemand == sourceLID.lidValue || MinDemand == sourceLID.lidValue)
                {
                    SourceIndex = iSourceIndex;
                }
            }

            return SourceIndex;
        }

        /// <summary>
        /// Handle any special conversions before building the display Item.
        /// </summary>
        /// <param name="DispItem">The Display Item we may need to modify</param>
        /// <returns>The new Display item (Item passed in if not changes were made)</returns>
        private ANSIDisplayItem ModifyLID(ANSIDisplayItem DispItem)
        {
            if (DispItem.DisplayLID.lidValue == (m_LID.CURRENT_DATE).lidValue)
            {
                // We cannot use the DATE LID, because the LID refers to GMT time, so the date could be wrong.
                //  We can use the TIME LID, to get the correct date, but we do not want to change the description.
                string Description = DispItem.Description;
                // The Calendar Date value LID is in GMT, but the Calendar Time value is local.  We want local.
                DispItem.DisplayLID = m_LID.CURRENT_TIME;
                DispItem.DisplayLID.lidDescription = Description;
                DispItem.Description = Description;
            }
            return DispItem;
        }

        /// <summary>
        /// Gets the quantity object described by the Energy and Demand LIDs.
        /// </summary>
        /// <param name="energyLID">The energy LID for the quantity.</param>
        /// <param name="demandLID">The demand LID for the quantity.</param>
        /// <param name="quantityDescription">The description of the quantity.</param>
        /// <param name="registers">The registers to create the quantity from.</param>
        /// <returns>The quantity if it is supported by the meter. Null if the quantity is not supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private Quantity GetQuantityFromStandardTables(LID energyLID, LID demandLID, string quantityDescription, RegisterDataRecord registers)
        {
            Quantity FoundQuantity = null;
            int? EnergySelectionIndex = null;
            int? DemandSelectionIndex = null;
            double Value;
            DateTime TimeOfOccurance;

            // First find the selection indexes so we know what to retrieve.
            if (energyLID != null)
            {
                EnergySelectionIndex = FindEnergySelectionIndex(energyLID);
            }

            if (demandLID != null)
            {
                DemandSelectionIndex = FindDemandSelectionIndex(demandLID);
            }

            if (EnergySelectionIndex != null || DemandSelectionIndex != null)
            {
                // The meter supports the Quantity so we can start creating it.
                FoundQuantity = new Quantity(quantityDescription);
                if (EnergySelectionIndex != null)
                {
                    // Add the energy data items
                    Value = registers.TotalDataBlock.Summations[(int)EnergySelectionIndex];
                    FoundQuantity.TotalEnergy = new Measurement(Value, energyLID.lidDescription);
                    FoundQuantity.TOUEnergy = new List<Measurement>();

                    for (int iRate = 0; iRate < Table21.NumberOfTiers; iRate++)
                    {
                        Value = registers.TierDataBlocks[iRate].Summations[(int)EnergySelectionIndex];
                        FoundQuantity.TOUEnergy.Add(new Measurement(Value, GetTOUEnergyLID(energyLID, iRate).lidDescription));
                    }
                }

                if (DemandSelectionIndex != null)
                {
                    LID CumDemandLID = GetCumDemandLID(demandLID);
                    LID CCumDemandLID = GetCCumDemandLID(demandLID);
                    DemandRecord CurrentDemandRecord = registers.TotalDataBlock.Demands[(int)DemandSelectionIndex];

                    // Add the demand data items
                    // The quantity object only supports 1 occurrence so always use occurrence 0
                    Value = CurrentDemandRecord.Demands[0];
                    TimeOfOccurance = CurrentDemandRecord.TimeOfOccurances[0];

                    FoundQuantity.TotalMaxDemand = new DemandMeasurement(Value, demandLID.lidDescription);
                    FoundQuantity.TotalMaxDemand.TimeOfOccurance = TimeOfOccurance;

                    Value = CurrentDemandRecord.Cum;
                    FoundQuantity.CummulativeDemand = new Measurement(Value, CumDemandLID.lidDescription);

                    Value = CurrentDemandRecord.CCum;
                    FoundQuantity.ContinuousCummulativeDemand = new Measurement(Value, CCumDemandLID.lidDescription);

                    // Add TOU rates
                    if (Table21.NumberOfTiers > 0)
                    {
                        FoundQuantity.TOUMaxDemand = new List<DemandMeasurement>();
                        FoundQuantity.TOUCummulativeDemand = new List<Measurement>();
                        FoundQuantity.TOUCCummulativeDemand = new List<Measurement>();

                        for (int iRate = 0; iRate < Table21.NumberOfTiers; iRate++)
                        {
                            CurrentDemandRecord = registers.TierDataBlocks[iRate].Demands[(int)DemandSelectionIndex];

                            Value = CurrentDemandRecord.Demands[0];
                            TimeOfOccurance = CurrentDemandRecord.TimeOfOccurances[0];

                            FoundQuantity.TOUMaxDemand.Add(new DemandMeasurement(Value, GetTOUDemandLid(demandLID, iRate).lidDescription));
                            FoundQuantity.TOUMaxDemand[iRate].TimeOfOccurance = TimeOfOccurance;

                            Value = CurrentDemandRecord.Cum;
                            FoundQuantity.TOUCummulativeDemand.Add(new Measurement(Value, GetTOUDemandLid(CumDemandLID, iRate).lidDescription));

                            Value = CurrentDemandRecord.CCum;
                            FoundQuantity.TOUCCummulativeDemand.Add(new Measurement(Value, GetTOUDemandLid(CCumDemandLID, iRate).lidDescription));
                        }
                    }
                }
            }

            return FoundQuantity;
        }

        /// <summary>
        /// Finds the selection index of the specified energy.
        /// </summary>
        /// <param name="energyLID">The LID for the quantity to search for.</param>
        /// <returns>Null if the quantity is not supported or the selection index if supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private int? FindEnergySelectionIndex(LID energyLID)
        {
            byte[] SummationSelections = Table22.SummationSelections;
            int? EnergySourceIndex = null;
            int? EnergySelectionIndex = null;

            // Find the source index first.
            EnergySourceIndex = FindEnergySourceIndex(energyLID);

            // Now make sure the energy source is used.
            if (EnergySourceIndex != null)
            {
                for (int iSelectionIndex = 0; iSelectionIndex < SummationSelections.Length; iSelectionIndex++)
                {
                    if (SummationSelections[iSelectionIndex] == EnergySourceIndex)
                    {
                        EnergySelectionIndex = iSelectionIndex;
                    }
                }
            }

            return EnergySelectionIndex;
        }

        /// <summary>
        /// Gets the Cumulative Demand LID for the specified base Demand.
        /// </summary>
        /// <param name="demandLID">The base demand LID.</param>
        /// <returns>The Cumulative demand LID.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private LID GetCumDemandLID(LID demandLID)
        {
            uint LIDNumber = (demandLID.lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK_OUT)
                | (uint)DefinedLIDs.Demand_Data.CUM_DEMAND;

            return CreateLID(LIDNumber);
        }

        /// <summary>
        /// Gets the Continuously Cumulative Demand LID for the specified base Demand.
        /// </summary>
        /// <param name="demandLID">The base demand LID.</param>
        /// <returns>The Continuously Cumulative demand LID.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private LID GetCCumDemandLID(LID demandLID)
        {
            uint LIDNumber = (demandLID.lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK_OUT)
                | (uint)DefinedLIDs.Demand_Data.CONT_CUM_DEMAND;

            return CreateLID(LIDNumber);
        }

        /// <summary>
        /// Gets the TOU LID for the specified energy and rate.
        /// </summary>
        /// <param name="energyLID">The base energy LID for the quantity</param>
        /// <param name="rate">The TOU rate to get.</param>
        /// <returns>The LID for the energy.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private LID GetTOUEnergyLID(LID energyLID, int rate)
        {
            uint LIDNumber = (energyLID.lidValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT)
                                | (uint)DefinedLIDs.BaseLIDs.TOU_DATA
                                | (uint)DefinedLIDs.TOU_Rate_Data.TOU_ENERGY
                                | TOU_RATES[rate];

            return CreateLID(LIDNumber);
        }

        /// <summary>
        /// Gets the TOU LID for the specified demand and rate.
        /// </summary>
        /// <param name="demandLID">The base demand LID for the quantity.</param>
        /// <param name="rate">The TOU rate to get.</param>
        /// <returns>The LID for the demand.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private LID GetTOUDemandLid(LID demandLID, int rate)
        {
            uint LIDNumber = (demandLID.lidValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT)
                                | (uint)DefinedLIDs.BaseLIDs.TOU_DATA
                                | (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND
                                | TOU_RATES[rate];

            return CreateLID(LIDNumber);
        }

        /// <summary>
        /// Finds the selection index of the specified demand.
        /// </summary>
        /// <param name="demandLID">The LID for the quantity to search for.</param>
        /// <returns>Null if the quantity is not supported or the selection index if supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private int? FindDemandSelectionIndex(LID demandLID)
        {
            byte[] DemandSelections = Table22.DemandSelections;
            int? DemandSourceIndex = null;
            int? DemandSelectionIndex = null;

            // Find the source index first.
            DemandSourceIndex = FindDemandSourceIndex(demandLID);

            // Make sure the demand source is used
            if (DemandSourceIndex != null)
            {
                for (int iSelectionIndex = 0; iSelectionIndex < DemandSelections.Length; iSelectionIndex++)
                {
                    if (DemandSelections[iSelectionIndex] == DemandSourceIndex)
                    {
                        DemandSelectionIndex = iSelectionIndex;
                    }
                }
            }

            return DemandSelectionIndex;
        }

        #endregion

        #region Members

        private DateTime m_dtLastActivationDate;
        private List<PendingEventActivationRecord> m_lstPendingTableRecords;

        private CTable04 m_Table04 = null;

        private StdTable11 m_Table11 = null;
        private StdTable14 m_Table14 = null;
        private StdTable21 m_Table21 = null;
        private StdTable22 m_Table22 = null;
        private StdTable23 m_Table23 = null;
        private StdTable24 m_Table24 = null;
        private StdTable25 m_Table25 = null;
        private StdTable26 m_Table26 = null;
        private StdTable27 m_Table27 = null;
        private StdTable28 m_Table28 = null;

        private StdTable61 m_Table61 = null;
        private StdTable62 m_Table62 = null;
        private StdTable63 m_Table63 = null;
        private StdTable64 m_Table64 = null;

        private OpenWayMfgTable2108 m_Table2108 = null;

        private OpenWayMFGTable2139 m_Table2139 = null;
        internal OpenWayMFGTable2140 m_Table2140 = null;
        private OpenWayMFGTable2141 m_Table2141 = null;
        private OpenWayMFGTable2142 m_Table2142 = null;

        private OpenWayMFGTable2220 m_Table2220 = null;
        private OpenWayMFGTable2260SR30Config m_Table2260SR30Config = null;
        private OpenWayMFGTable2260ExtendedConfig m_Table2260ExtendedConfig = null;
        private OpenWayMFGTable2261 m_Table2261 = null;
        private OpenWayMFGTable2262 m_Table2262 = null;
        private OpenWayMFGTable2263 m_Table2263 = null;
        private OpenWayMFGTable2264 m_Table2264 = null;
        private OpenWayMFGTable2360 m_Table2360 = null;

        private OpenWayMFGTable3043Header m_Table3043Header = null;
        private OpenWayMFGTable3043Info m_Table3043Info = null;
        private OpenWayMFGTable3043Map m_Table3043Map = null;

        private StdTable51 m_Table51 = null;
        private StdTable52 m_Table52 = null;
        private StdTable53 m_Table53 = null;

        private MFGTable2053 m_Table2053 = null;
        private OpenWayMFGTable2260DSTCalendar m_Table2260DSTConfig = null;


        /// <summary>Is DST Configured</summary>
        protected CachedBool m_DSTConfigured;
        /// <summary>Firmware Loader Version and Revision</summary>
        protected CachedFloat m_fltFWLoaderVerRev;
        /// <summary>
        /// Firmware Loader Version
        /// </summary>
        protected CachedByte m_FWLoaderVersion;
        /// <summary>
        /// Firmware Loader Revision
        /// </summary>
        protected CachedByte m_FWLoaderRevision;
        /// <summary>
        /// Firmware Loader Build
        /// </summary>
        protected CachedByte m_FWLoaderBuild;



        #endregion
    }
}
