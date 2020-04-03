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
//                           Copyright © 2006 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.AMIConfiguration;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.DST;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;

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
        [EnumDescription("Operation succeeded.")]
        REMOTE_ACTION_SUCCESS = 0,
        /// <summary>
        /// Remote Connect/Disconnect Failed due to Security Error
        /// </summary>
        [EnumDescription("Security error.")]
        SECURITY_VIOLATION = 1,
        /// <summary>
        /// Load Voltage was detected, so operation failed.
        /// </summary>
        [EnumDescription("Load voltage detected.")]
        LOAD_VOLTAGE_PRESENT = 3,
        /// <summary>
        /// The Connect/Disconnect operation failed, but we don't have an exact reason
        /// </summary>
        [EnumDescription("Unknown failure.")]
        REMOTE_CONNECT_FAILED = 4,
        /// <summary>
        /// Load Voltage was not detected after a connect, which could mean that the switch failed.  The base will be put
        /// back into the disconnect state.
        /// </summary>
        [EnumDescription("Load voltage not detected after connection attempt.")]
        LOAD_VOLTAGE_NOT_DETECTED = 5,
        /// <summary>
        /// Remote Connect/Disconnect is not supported on this device
        /// </summary>
        [EnumDescription("Operation not supported.")]
        UNRECOGNIZED_PROCEDURE = 6,
    }

    /// <summary>
    /// Enumeration for the Remote Connection Result Codes
    /// </summary>
    public enum ProcedureFirmwareType : byte
    {
        /// <summary>
        /// Register firmware
        /// </summary>
        REGISTER = 0,
        /// <summary>
        /// Comm Module firmware
        /// </summary>
        COMM_MODULE = 1,
        /// <summary>
        /// HAN Module FW.
        /// </summary>
        HAN_MODULE = 2,
        /// <summary>
        /// AVR Display FW
        /// </summary>
        AVR_DISPLAY = 3,
        /// <summary>
        /// HAN Remote Client FW
        /// </summary>
        HAN_REMOTE_CLIENT = 4,
        /// <summary>
        /// Power Line Carrier Comm Module FW
        /// </summary>
        POWER_LINE_CARRIER_COMM_MODULE = 5,
    }

    /// <summary>
    /// Enumeration for the type of enhanced security keys used
    /// </summary>
    public enum EnhancedSecurityAlgorithmUsed : byte
    {
        /// <summary>
        /// K-curve security keys
        /// </summary>
        [EnumDescription("K-Curve")]
        K_CURVE = 1,
        /// <summary>
        /// P-Curve security keys
        /// </summary>
        [EnumDescription("P-Curve")]
        P_CURVE = 2,
        /// <summary>
        /// Unknown - meaning either the keys have not been injected or the data item is not supported
        /// </summary>
        [EnumDescription("Undefined")]
        UNDEFINED = 0xFF,
    }

    /// <summary>
    /// Status of the HAN network
    /// </summary>
    public enum HANCurrentNetworkStatus : byte
    {
        /// <summary>
        /// HAN Network is Up
        /// </summary>
        NETWORK_UP = 0,
        /// <summary>
        /// HAN Network is Down
        /// </summary>
        NETWORK_DOWN = 1,
        /// <summary>
        /// HAN Network is Forming
        /// </summary>
        NETWORK_FORMING = 2,
        /// <summary>
        /// Invalid HAN Network Status
        /// </summary>
        INVALID = 3
    }

    /// <summary>
    /// Enumeration for making a choice about whether or not to particitpate in a generic activity. 
    /// </summary>
    public enum Opt : byte
    {
        /// <summary>
        /// Unknown participation status.
        /// </summary>
        [EnumDescription("Opt Choice Unknown")]
        Undecided,
        /// <summary>
        /// Choosing not to participate.
        /// </summary>
        [EnumDescription("Opt Out")]
        Out,
        /// <summary>
        /// Choosing to participate
        /// </summary>
        [EnumDescription("Opt In")]
        In,
    }

    /// <summary>
    /// Class representing the CENTRON AMI.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 07/26/06 KRC				    Created
    // 04/02/07 AF  8.00.23 2814    Corrected the capitalization of the meter name
    // 05/14/09 AF  2.20.05         Added constants for hardware versions and device class
    // 07/28/09 AF  2.20.17 137695  Added constant for PrismLite device class
    // 08/07/13 mah 2.85.12 326895  Changed PARAM1 to 56 when causing a fatal 2 or 6
    // 08/26/13 AF  2.85.28 420059  Changed PARAM1 back to 57 - 56 will cause FAT 7.
    // 11/11/13 AF  3.50.02         Changed the parent class from CANSIDevice to ANSIMeter
    //
    public abstract partial class CENTRON_AMI : ANSIMeter, IFirmwareDownload
    {
        #region Constants

        /// <summary>
        /// Meter type identifier
        /// </summary>
        public const string CENTRONAMI = "AMI CENT";
        /// <summary>
        /// Human readable name of meter
        /// </summary>
        private const string CENTRONAMI_NAME = "OpenWay CENTRON";
        /// <summary>
        /// Human readable name for transparent devices.
        /// </summary>
        private const string ITRT_NAME = "OpenWay Transparent Device";
        /// <summary>
        /// Human readable name for host meter devices.
        /// </summary>
        private const string ITRL_NAME = "OpenWay Host Meter Device";
        /// <summary>
        /// Name used in the activity log
        /// </summary>
        private const string LOG_ITR1_NAME = "OW CENTRON";
        /// <summary>
        /// Name used in the activity log for transparent devices.
        /// </summary>
        private const string LOG_ITRT_NAME = "OW Transparent Device";
        /// <summary>
        /// Manufacturer
        /// </summary>
        private const string MANUFACTURER = "ITRN";

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
        /// Secondary Energy Base LID value
        /// </summary>
        protected const uint SEC_ENERGY_LID_BASE = 0x14000080;
        /// <summary>
        /// Function code for the Format Flash procedure
        /// </summary>
        private const byte FORMAT_FLASH_FUNC_CODE = 0xA3;
        /// <summary>
        /// Parameter 1 for the Format Flash procedure
        /// </summary>
        private const uint FORMAT_FLASH_PARAM_1 = 0;
        /// <summary>
        /// Parameter 2 for the Format Flash procedure
        /// </summary>
        private const uint FORMAT_FLASH_PARAM_2 = 0;
        /// <summary>
        /// Function code for the Reset RF procedure
        /// </summary>
        private const byte RESET_RF_FUNC_CODE = 1;
        /// <summary>
        /// Parameter 1 for the Reset RF procedure
        /// </summary>
        private const uint RESET_RF_PARAM_1 = 32;
        /// <summary>
        /// Parameter 2 for the Reset RF procedure
        /// </summary>
        private const uint RESET_RF_PARAM_2 = 0;
        /// <summary>
        /// Mask for the PrismLite nibble of the hardware version.
        /// </summary>
        private const byte PRISM_LITE_HW_MASK = 0x80;
        /// <summary>
        /// Memory offset for the FW Loader version in an ARM based meter
        /// </summary>
        private const uint FW_LOADER_ARM_OFFSET = 0x00100042;
        /// <summary>
        /// Memory offset for the FW Loader version in an M16C based meter
        /// </summary>
        private const uint FW_LOADER_M16C_OFFSET = 0x000FC002;
        /// <summary>
        /// Number of Last Demand Resets in the meter
        /// </summary>
        private const uint NUM_LAST_DEMAND_RESETS = 2;
        /// <summary>
        ///  Load Control Reconnect
        /// </summary>
        private const int LOAD_CONTROL_RECONNECT = 0x80;

        /// <summary>
        /// The list of the TOU Rate modifiers for LIDs
        /// </summary>
        private readonly uint[] TOU_RATES = {(uint)DefinedLIDs.TOU_Data.RATE_A, (uint)DefinedLIDs.TOU_Data.RATE_B, 
                                             (uint)DefinedLIDs.TOU_Data.RATE_C, (uint)DefinedLIDs.TOU_Data.RATE_D, 
                                             (uint)DefinedLIDs.TOU_Data.RATE_E, (uint)DefinedLIDs.TOU_Data.RATE_F,
                                             (uint)DefinedLIDs.TOU_Data.RATE_G };

        private const byte MAX_RATE_LABEL_LEN = 12;
        private const byte MIN_RATE_LABEL_LEN = 1;
        private const byte MAX_PRICE_TRAILING_DIGIT_LEN = 15;
        private const byte MIN_PRICE_TRAILING_DIGIT_LEN = 0;

        /// <summary>
        /// Length of the response for MFG Procedure 46
        /// </summary>
        protected const int MFG_PROC_46_RESPONSE_LENGTH = 42;
        
        /// <summary>
        /// Constant Describing the Hardware Version for 3.0 meters
        /// </summary>
        public const float HW_VERSION_3_0 = 3.000F;
        /// <summary>
        /// Function code for the set time on battery procedure
        /// </summary>
        public const byte TIME_ON_BATT_FUNC_CODE = 166;

        /// <summary>
        /// Parameter 1 for the Soft EPF procedure
        /// </summary>
        private const uint SOFT_EPF_PARAM_1 = 110;
        /// <summary>
        /// Parameter 2 for the Soft EPF procedure
        /// </summary>
        private const uint SOFT_EPF_PARAM_2 = 0;

        /// <summary>
        /// Length in bytes of the activation trigger for pending tables
        /// </summary>
        public const int ACTIVATION_TRIGGER_LEN = 6;
        /// <summary>
        /// The Set Time Date Mask for setting the clock
        /// </summary>
        protected const byte AMI_SET_TIME_DATE_MASK = (byte)(SET_MASK_BFLD.SET_DATE_FLAG | SET_MASK_BFLD.SET_TIME_FLAG
                                                    | SET_MASK_BFLD.SET_TIME_DATE_QUAL);
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
        /// Dataflash page to find the pre-core dump data on HW 3.0 meters.
        /// </summary>
        protected const int PRE_CORE_DUMP_DATAFLASH_PAGE = 3071;
        /// <summary>
        /// Dataflash page to find the pre-core dump data on HW 3.0 meters with 
        /// Lithium firmware versions 3.12.5 - 3.12.26.
        /// </summary>
        protected const int LITHIUM_ALT_PRE_CORE_DUMP_DATAFLASH_PAGE = 4095;
        /// <summary>
        /// The size of dataflash page in meter.
        /// </summary>
        protected const int DATAFLASH_PAGE_SIZE = 264;
        /// <summary>
        /// The size of the pre-core dump.
        /// </summary>
        protected const int PRE_CORE_DUMP_SIZE = 256;
        /// <summary>
        /// Constant describing the length of the optical passwords
        /// </summary>
        public const int OPTICAL_PASSWORD_LEN = 20;

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
        /// Constant describing the firmware version for Lithium+, 3.014
        /// </summary>
        public const float VERSION_LITHIUM_PLUS_3_14 = 3.014F;
        /// <summary>
        /// Constant describing firmware version that is next version greater than Bridge Phase 1 (3.32)
        /// There are no FW versions between 3.32 and 4.0
        /// </summary>
        public const float VERSION_GREATER_THAN_BRIDGE_PHASE1_4_00 = 4.000F;
        /// <summary>
        /// Constant describing the firmware version for Boron, 5.000
        /// </summary>
        public const float VERSION_BORON_5_0 = 5.000F;
        /// <summary>
        /// Constant describing the firmware version for Boron, 5.002
        /// </summary>
        public const float VERSION_BORON_5_2 = 5.002F;
        /// <summary>
        /// Constant describing the firmware version for M2 Gateway, initial release, 1.000
        /// </summary>
        public const float VERSION_M2GTWY_1_0 = 1.000F;
        /// <summary>
        /// Constant describing the firmware version for M2 Gateway Lithium, 2.000
        /// </summary>
        public const float VERSION_M2GTWY_2_0 = 2.000F;
        /// <summary>
        /// Constant describing the firmware version for Michigan
        /// </summary>
        public const float VERSION_MICHIGAN = 6.000F;
        /// <summary>
        /// Constant describing the firmware version for SR 6.6
        /// </summary>
        public const float VERSION_SR6_6_4GLTE = 6.003F;
        /// <summary>
        /// Constant describing the firmware version for Beryllium. 6.005 will only run on HW 3.0/3.1
        /// </summary>
        public const float VERSION_6_005_BERYLLIUM_HW_3_x = 6.005F;
         /// <summary>
        /// Constant describing the firmware version for Beryllium. 4.000 will only run on HW 2.0
        /// </summary>
        public const float VERSION_4_000_BERYLLIUM_HW_2_0 = 4.000F;
        ///<summary>
        /// Constant describing the firmware version for Beryllium
        /// </summary>
        public const float VERSION_BERYLLIUM = 7.000F;
		/// <summary>
        /// Constant for the metrology statistics table number.
        /// </summary>
        protected const ushort METROLOGY_STATISTICS_TABLE = 2112;
        /// <summary>
        /// Constant for the name of the varh quanity.
        /// </summary>
        protected const string VARH_QUANTITY_NAME = "varh";

        private const byte ARITHMETIC = 0;
        private const byte VECTORIAL = 1;
        private const short VA = 1;
        private const short VAR = 2;

        private const uint VARH_Q2_Q3_ENERGY_MASK = 0x0000000C;
        private const bool DISABLE_ZIGBEE = false;
        private const bool ENABLE_ZIGBEE = true;
        private const int HAN_OPT_OUT_RETRIES = 3;
        private const int RF_OPT_OUT_RETRIES = 1;
        private const int DEREGISTER_OPT_OUT_RETRIES = 3;
        private const int REGISTER_OPT_OUT_RETRIES = 3;
        private const int CONFIG_TAG_WRITE_OPT_IN_RETRIES = 3;
        private const int RF_OPT_OUT_PROCEDURE_TIMEOUT_RETRIES = 5;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration of LAN Communication Log Event
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/07/13 jrf 2.80.27 TQ8279 Adding in unenumerated comm events.
        // 12/10/14 jrf	4.00.91 551420 added missing event definition.
        public enum LANEvents
        {
            /// <summary>Mark the beginning of the MFG LAN Events</summary>
            BEGIN_MFG_LAN_EVENTS = 2048,
            /// <summary>RFLAN went from synchronized to not synchronized</summary>
            COMEVENT_C1222_LINK_FAILURE = 1 + 2048,
            /// <summary>Same as COMEVENT_C1222_RFLAN_CELL_CHANGE</summary>
            COMEVENT_C1222_LINK_SWITCH = 2 + 2048,
            /// <summary>RFLAN went from not Synchronized to Synchronized</summary>
            COMEVENT_C1222_LINK_UP = 3 + 2048,
            /// <summary>Link Metrics</summary>
            COMEVENT_C1222_LINK_METRIC = 4 + 2048,
            /// <summary>Send of Periodic Read Failed</summary>
            COMEVENT_C1222_SEND_PERIODIC_READ_FAILED = 5 + 2048,
            /// <summary>Send of Periodic Read Succeeded</summary>
            COMEVENT_C1222_SEND_PERIODIC_READ_SUCCESS = 6 + 2048,
            /// <summary>Send of Exception Failed</summary>
            COMEVENT_C1222_SEND_EXCEPTION_FAILED = 7 + 2048,
            /// <summary>Send of Exception Succeeded</summary>
            COMEVENT_C1222_SEND_EXCEPTION_SUCCESS = 8 + 2048,
            /// <summary>Send of Response Failed</summary>
            COMEVENT_C1222_SEND_RESPONSE_FAILED = 9 + 2048,
            /// <summary>Send of Response Succeeded</summary>
            COMEVENT_C1222_SEND_RESPONSE_SUCCESS = 10 + 2048,
            /// <summary>Sent Segment Bytes</summary>
            COMEVENT_SENT_SEGMENT_BYTES = 11 + 2048,
            /// <summary>Received Segment Bytes</summary>
            COMEVENT_RECEIVED_SEGMENT_BYTES = 12 + 2048,
            /// <summary>Request Received</summary>
            COMEVENT_C1222_RECEIVED_REQUEST = 13 + 2048,
            /// <summary>Response Sent</summary>
            COMEVENT_C1222_SENT_RESPONSE = 14 + 2048,
            /// <summary>The messsage was sent successfully</summary>
            COMEVENT_C1222_SEND_MESSAGE_SUCCEEDED = 15 + 2048,
            /// <summary>The message could not be sent to the comm module</summary>
            COMEVENT_C1222_SENT_MESSAGE_FAILED = 16 + 2048,
            /// <summary>Send Failure Limit Exceeded</summary>
            COMEVENT_C1222_CONFIGURED_SEND_FAILURE_LIMIT_EXCEEDED = 17 + 2048,
            /// <summary>Result of the Registration</summary>
            COMEVENT_C1222_REGISTRATION_RESULT = 18 + 2048,
            /// <summary>Result of the Deregistration</summary>
            COMEVENT_C1222_DEREGISTRATION_RESULT = 19 + 2048,
            /// <summary>Comm Module Link Reset</summary>
            COMEVENT_C1222_COMM_MODULE_LINK_RESET = 20 + 2048,
            /// <summary>Comm Module Link Failed</summary>
            COMEVENT_C1222_COMM_MODULE_LINK_FAILED = 21 + 2048,
            /// <summary>Comm Module Link Up</summary>
            COMEVENT_C1222_COMM_MODULE_LINK_UP = 22 + 2048,
            /// <summary>Level Change</summary>
            COMEVENT_C1222_LEVEL_CHANGE = 23 + 2048,
            /// <summary>Best Father Changed</summary>
            COMEVENT_C1222_BEST_FATHER_CHANGE = 24 + 2048,
            /// <summary>Synch Father Changed</summary>
            COMEVENT_C1222_SYNCH_FATHER_CHANGE = 25 + 2048,
            /// <summary>Process Received Message Timing</summary>
            COMEVENT_C1222_PROCESS_RCVD_MSG_TIMING = 26 + 2048,
            /// <summary>Registration Attempt</summary>
            COMEVENT_C1222_REGISTRATION_ATTEMPT = 27 + 2048,
            /// <summary>Registered</summary>
            COMEVENT_C1222_REGISTERED = 28 + 2048,
            /// <summary>Deregistration Attempt</summary>
            COMEVENT_C1222_DEREGISTRATION_ATTEMPT = 29 + 2048,
            /// <summary>Deregistered</summary>
            COMEVENT_C1222_DEREGISTERED = 30 + 2048,
            /// <summary>RFLAN Cell Changed</summary>
            COMEVENT_C1222_RFLAN_CELL_CHANGE = 31 + 2048,
            /// <summary>Received an Invalid Message</summary>
            COMEVENT_C1222_RECEIVED_INVALID_MESSAGE = 32 + 2048,
            /// <summary>Received a Message from...</summary>
            COMEVENT_RECEIVED_MESSAGE_FROM = 33 + 2048,
            /// <summary>Sent a Message to...</summary>
            COMEVENT_SENT_MESSAGE_TO = 34 + 2048,
            /// <summary>Periodic Read Send Table</summary>
            COMEVENT_PR_SEND_TABLE = 35 + 2048,
            /// <summary>Periodic Read Send Table Failed</summary>
            COMEVENT_PR_SEND_TABLE_FAILED = 36 + 2048,
            /// <summary>Reset</summary>
            COMEVENT_C1222_RESET = 37 + 2048,
            /// <summary>Simple Error Response Sent</summary>
            COMEVENT_C1222_SENT_SIMPLE_ERROR_RESPONSE = 38 + 2048,
            /// <summary>RFLAN Cold Start Event</summary>
            COMEVENT_C1222_RFLAN_COLD_START = 39 + 2048,
            /// <summary>Incoming Segment Discarded in Keep Alive Event</summary>
            COMEVENT_INCOMING_SEGMENT_DISCARDED_IN_KEEPALIVE = 40 + 2048,
            /// <summary>Comm Log Test Event</summary>
            COMEVENT_COMM_LOG_TEST = 41 + 2048,
            /// <summary>Comm Log One Hour Maximum Event</summary>
            COMEVENT_COMM_LOG_ONE_HR_MAX = 42 + 2048,
            /// <summary>Periodic Read Timing Info Event</summary>
            COMEVENT_C1222_PERIODIC_READ_TIMING = 43 + 2048,
            /// <summary>Failed to Add Exception ID Event</summary>
            COMEVENT_C1222_FAILED_ADD_EXCEPTION_ID = 44 + 2048,
            /// <summary>Failed to Add Exception Detail Event</summary>
            COMEVENT_C1222_FAILED_ADD_EXCEPTION_DETAIL = 45 + 2048,
            /// <summary>Successfully Added Exception ID Event</summary>
            COMEVENT_C1222_ADDED_EXCEPTION_ID = 46 + 2048,
            /// <summary>Event Cache Overflow Event</summary>
            COMEVENT_EVENT_CACHE_OVERFLOWED = 47 + 2048,
            /// <summary>SMS Wakeup IDENT Request Failed Event</summary>
            COMEVENT_C1222_SEND_IDENT_REQUEST_FAILED = 48 + 2048,
            /// <summary>SMS Wakeup IDENT Request Enqueued Event</summary>
            COMEVENT_C1222_SEND_IDENT_REQUEST = 49 + 2048,
            /// <summary>CRC Errors Were Seein in FW Blocks Recieved In Last Hour Event</summary>
            COMEVENT_C1222_FIRMWARE_BLOCK_CRC_ERRORS = 50 + 2048, 
            /// <summary>Mark the end of the MFG LAN Events</summary>
            END_MFG_LAN_EVENTS = 51 + 2048,
        }

        /// <summary>
        /// Enumeration of HAN Communication Log Event
        /// </summary>
        public enum HANEvents
        {
            /// <summary>Mark the beginning of the MFG HAN Events</summary>
            BEGIN_MFG_HAN_EVENTS = 100 + 2048,
            /// <summary>ZigBee Message</summary>
            EVENT_ZIGBEE_MSG = 101 + 2048,
            /// <summary>ZigBee Key Established Message</summary>
            EVENT_ZIGBEE_KEY_EST_MSG = 102 + 2048,
            /// <summary>ZigBee Pricing Messsage</summary>
            EVENT_ZIGBEE_PRICE_MSG = 103 + 2048,
            /// <summary>ZigBee DRLS Message</summary>
            EVENT_ZIGBEE_DRLC_MSG = 104 + 2048,
            /// <summary>ZigBee Simple Metering Message</summary>
            EVENT_ZIGBEE_SIMPLE_METERING_MSG = 105 + 2048,
            /// <summary>ZigBee Messaging Message</summary>
            EVENT_ZIGBEE_MESSAGING_MSG = 106 + 2048,
            /// <summary>ZigBee SE Tunneling Message</summary>
            EVENT_ZIGBEE_SE_TUNNELING_MSG = 107 + 2048,
            /// <summary>ZigBee Pre-Pay Message</summary>
            EVENT_ZIGBEE_PREPAY_MSG = 108 + 2048,
            /// <summary>Mark the end of the MFG LAN Events</summary>
            END_MFG_HAN_EVENTS = 109 + 2048,
        }

        /// <summary>
        /// Enumeration for the Enter/Exit Test mode Result Codes
        /// </summary>
        public enum EnterExitTestModeResult
        {
            /// <summary>
            /// Success
            /// </summary>
            SUCCESS = 0,
            /// <summary>
            /// Failed
            /// </summary>
            FAILED = 1,
            /// <summary>
            /// Busy
            /// </summary>
            BUSY = 2,
            /// <summary>
            /// Inappropriate
            /// </summary>
            INAPPROPRIATE = 3,
        }

        /// <summary>
        /// Enumeration for the optional items that can be removed from a periodic read.
        /// </summary>
        [Flags]
        public enum PeriodicReadRemovableItems : byte
        {
            /// <summary>
            /// Remove none of the optional items from the periodic read.  
            /// </summary>
            NONE = 0,
            /// <summary>
            /// Remove the program state data from the periodic read.
            /// </summary>
            PROGRAM_STATE = 1,
            /// <summary>
            /// Remove the MCU inforation from the periodic read.
            /// </summary>
            MCU_INFORMATION = 2,
            /// <summary>
            /// Remove all optional items from the periodic read.  All flags set.
            /// </summary>
            ALL = 255,
        }

        private enum AMI_TOU_Events
        {
            EVENT_NOT_USED = 0x00,
            EVENT_DST_ON = 0x01,
            EVENT_DST_OFF = 0x02,
            EVENT_HOLIDAY = 0x03,
            EVENT_SEASON1 = 0x04,
        }

        /// <summary>
        /// Firmware download status and progress history
        /// </summary>
        public enum HAN_FW_DL_STATUS
        {
            /// <summary>
            /// Initial status of a device that never entered download mode.
            /// Not indicative of whether the device supports FW download or not.
            /// </summary>
            Unknown = 0x0,
            /// <summary>
            /// File size was not correct.  Device will not support download of 
            /// this image
            /// </summary>
            BadFileSize = 0x01,
            /// <summary>
            /// Hardware Revision was not correct.  Device will not support 
            /// download of this image
            /// </summary>
            BadHWRevision = 0x02,
            /// <summary>
            /// Hardware version was not correct.  Device will not support 
            /// download of this image
            /// </summary>
            BadHWVersion = 0x03,
            /// <summary>
            /// Firmware type was incorrect.  HAN device failed to activate the image.
            /// </summary>
            BadFWType = 0x04,
            /// <summary>
            /// HAN device failed to activate the image due to a bad CRC32.
            /// </summary>
            BadCRC = 0x05,
            /// <summary>
            /// This status is set after receiving a device deemed to fail due
            /// to excessive image errors.
            /// </summary>
            RetryFailed = 0x06,
            /// <summary>
            /// HAN device has incomplete image.  Device failed.
            /// </summary>
            InsufficientBlocks = 0x07,
            /// <summary>
            /// This status will be set by the electric meter to indicate that
            /// the device is the one actively downloading firmware
            /// </summary>
            DownloadInProgress = 0x08,
            /// <summary>
            /// Firmware type was invalild.  Device will not support download 
            /// of this image.
            /// </summary>
            InvalidFWType = 0x0A,
            /// <summary>
            /// Device class was incorrect.  Device will not support download 
            /// of this image.
            /// </summary>
            InvalidDeviceClass = 0x13,
            /// <summary>
            /// After trying to initiate download twice with a device known to
            /// support FW download, if no response is heard, the device is moved
            /// to this status.  Device will not support download.
            /// </summary>
            InitiateFailure = 0x14,
            /// <summary>
            /// HAN device is operating with the firmware version that the electric
            /// meter is trying to download.
            /// </summary>
            VersionRunning = 0x80,
            /// <summary>
            /// The next step would be to send time at the next communication with
            /// the device, and promote it to READY FOR ACTIVATION
            /// </summary>
            VersionDownloaded = 0x81,
            /// <summary>
            /// This is a fatal error that is reported to the head end.  The HAN
            /// module and electric meter shall abort download.
            /// </summary>
            VoltageOutOfRange = 0x86,
            /// <summary>
            /// This is a fatal error that is reported to the head end.  The HAN
            /// module and the electric meter shall abort download.
            /// </summary>
            FlashWriteFailure = 0x88,
            /// <summary>
            /// This status is set starting/resuming a download
            /// </summary>
            DownloadSetup = 0x8D,
            /// <summary>
            /// This status is set when a HAN module responds to the last packet
            /// of the last page of the image with a Load Image Packet Response, 
            /// but before a Download Complete Response is received by the electric
            /// meter.
            /// </summary>
            SuccessfulEndOfTransfer = 0x8E,
            /// <summary>
            /// This status is set after sending Activate Download Command.
            /// </summary>
            ActivationSent = 0x8F,
            /// <summary>
            /// The HAN device shuts radio off and does image CRC check.  Upon
            /// waking up, the HAN device will join the same electric meter
            /// hopefully and we will expect 'Version Running', 'Bad CRC', or 
            /// 'Bad FW Type'.
            /// </summary>
            Activating = 0x90,
            /// <summary>
            /// This status is set when a HAN module responds to a Pause command,
            /// or if the IDLE_WAKEUP_SENT timer expires while the HAN module is 
            /// in SYNC_PACKET_SENT status and no Load Image Packet Response is
            /// heard back.  This status is also set if a magnet swipe occurs at
            /// the gas module, or if the meter cannot download to the gas module
            /// because there is another ongoing download.
            /// </summary>
            Paused = 0x92,
            /// <summary>
            /// This status is set after failing to receive a Load Image Packet
            /// Response at the end of the page within an IDLE_PAGE_SEND_TIMER
            /// timer expires.
            /// </summary>
            ResyncNeeded = 0x93,
            /// <summary>
            /// This status is set after sending a HAN module a wake up packet
            /// when the IDLE_PAGE_SEND_TIMER timer expires.
            /// </summary>
            SyncPacketSent = 0x94,
            /// <summary>
            /// This status is set after sending a HAN module a Set Time command
            /// if it was in VERSION DOWNLOADED.  The HAN module must be in READY
            /// FOR ACTIVATION before an Activate Download Command can be sent
            /// </summary>
            ReadyForActivation = 0x95,
            /// <summary>
            /// This status is set when a HAN module is sent a Cancel by the Head
            /// End, and validated when it responds with a Stop Download Response.
            /// </summary>
            HeadEndCancellation = 0x97
        }

        /// <summary>
        /// Test Mode for Basic and Advanced Poly meters
        /// </summary>
        public enum TestMode
        {
            /// <summary>
            /// Enter Test Mode
            /// </summary>
            ENTER_TEST_MODE = 0,
            /// <summary>
            /// Exit test Mode
            /// </summary>
            EXIT_TEST_MODE = 2,
        }

        /// <summary>
        /// Identifier for the specific DES security key to be validated
        /// </summary>
        public enum DESKeys : byte
        {
            /// <summary>
            /// C1222 Key 1 
            /// </summary>
            [EnumDescription("DES Key 1")]
            DESKey1 = 1,
            /// <summary>
            /// C1222 Key 2 
            /// </summary>
            [EnumDescription("DES Key 2")]
            DESKey2 = 2,
            /// <summary>
            /// C1222 Key 3
            /// </summary>
            [EnumDescription("DES Key 3")]
            DESKey3 = 3,
            /// <summary>
            /// C1222 Key 4
            /// </summary>
            [EnumDescription("DES Key 4")]
            DESKey4 = 4,
        }

        /// <summary>
        /// Identifier for the specific HAN security key to be validated
        /// </summary>
        public enum HANKeys : byte
        {
            /// <summary>
            /// Network Key 
            /// </summary>
            [EnumDescription("HAN Network Key")]
            NetworkKey = 1,
            /// <summary>
            /// Link Key 
            /// </summary>
            [EnumDescription("HAN Link Key")]
            LinkKey = 2,
        }

        /// <summary>
        /// Identifier for the specific Enhanced security key to be validated
        /// </summary>
        public enum EnhancedKeys : byte
        {
            /// <summary>
            /// Command Key 1 
            /// </summary>
            [EnumDescription("Enhanced Security Command Key 1")]
            CommandKey1 = 1,
            /// <summary>
            /// Command Key 2 
            /// </summary>
            [EnumDescription("Enhanced Security Command Key 2")]
            CommandKey2 = 2,
            /// <summary>
            /// Command Key 3 
            /// </summary>
            [EnumDescription("Enhanced Security Command Key 3")]
            CommandKey3 = 3,
            /// <summary>
            /// Command Key 4 
            /// </summary>
            [EnumDescription("Enhanced Security Command Key 4")]
            CommandKey4 = 4,
            /// <summary>
            /// Revocation Key 1
            /// </summary>
            [EnumDescription("Enhanced Security Revocation Key 1")]
            RevocationKey1 = 5,
            /// <summary>
            /// Revocation Key 2
            /// </summary>
            [EnumDescription("Enhanced Security Revocation Key 2")]
            RevocationKey2 = 6,
        }

        /// <summary>
        /// Task Name for procedure 2270
        /// </summary>
        public enum TaskName : byte
        {
            /// <summary>
            /// Idle Task 
            /// </summary>
            IDLE_Task = 0,
            /// <summary>
            /// EPF task
            /// </summary>
            EPF_Task = 1,
            /// <summary>
            /// MODEMAN Task
            /// </summary>
            MODEMAN_Task = 2,
            /// <summary>
            /// EVENTMAN Task
            /// </summary>
            EVENTMAN_Task = 3,
            /// <summary>
            /// ANSI_1_Task
            /// </summary>
            ANSI_1_Task = 4,
            /// <summary>
            /// FW_DL Task
            /// </summary>
            FW_DL_Task = 5,
            /// <summary>
            ///  PSEM Task
            /// </summary>
            PSEM_Task = 6,
            /// <summary>
            /// C1222_REG_Task
            /// </summary>
            C1222_REG_Task = 7,
            /// <summary>
            /// C1222_ANSI_Task
            /// </summary>
            C1222_ANSI_or_EXT_FLASH_Task= 8,
            /// <summary>
            /// ZIGBEE_Task
            /// </summary>
            ZIGBEE_Task = 9,
            /// <summary>
            /// ZIGBEE_STACK_Task
            /// </summary>
            ZIGBEE_STACK_Task = 10,
            /// <summary>
            /// Shim Task
            /// </summary>
            SHIM_Task = 11,
            /// <summary>
            /// IP Stack Task
            /// </summary>
            IP_STACK_Task = 12,
            /// <summary>
            /// Stack Reset Task
            /// </summary>
            IP_STACK_RESET_Task = 13,
            /// <summary>
            /// C1222 Stack Task
            /// </summary>
            C1222_STACK_RESET_Task = 14,
        }

        /// <summary>
        /// Identifier for Firwmare Download Event ID Type
        /// </summary>
        public enum FWDLEventIDType : byte
        {
            /// <summary>
            /// Register Firmware Activated
            /// </summary>
            [EnumDescription("RegisterFirmwareActivated")]
            RegisterFirmwareActivated = 0,
            /// <summary>
            /// RFLAN Firmware Activated
            /// </summary>
            [EnumDescription("RFLANFirmwareActivated")]
            RFLANFirmwareActivated = 1,
            /// <summary>
            /// Third Party Firmware Activated
            /// </summary>
            [EnumDescription("Third Party Firmware Activated")]
            ThirdPartyFirmwareActivated = 2,
        }

        /// <summary>
        /// Identifier for Firwmare Download Event Source Type
        /// </summary>
        public enum FWDLEventSourceType : byte
        {
            /// <summary>
            /// Optical Source
            /// </summary>
            [EnumDescription("Optical Source")]
            OpticalSource = 0,
            /// <summary>
            /// Network Source
            /// </summary>
            [EnumDescription("Network Source")]
           NetworkSource = 1,

        }

        /// <summary>
        /// The method used for updating the RIB accumulator value
        /// </summary>
        public enum RIBUpdateMethod : byte
        {
            /// <summary>Replace the current value</summary>
            Replace = 0,
            /// <summary>Add to the current value</summary>
            Add = 1,
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
            [EnumDescription("Primary Optical Password")]
            Primary = 1,
            /// <summary>
            /// C1218 Secondary PWD 
            /// </summary>
            [EnumDescription("Secondary Optical Password")]
            Secondary = 2,
            /// <summary>
            /// C1218 Tertiary PWD 
            /// </summary>
            [EnumDescription("Tertiary Optical Password")]
            Tertiary = 3,
            /// <summary>
            /// C1218 Quaternary PWD 
            /// </summary>
            [EnumDescription("Quaternary Optical Password")]
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
            /// <summary>
            /// Reset Magnetic TAmpers
            /// </summary>
            RESET_NUM_MAGNETIC_TAMPERS = 0x00000080,

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
            [EnumDescription("VARh d")]
            Varh = 0x02,
        }

        /// <summary>
        /// Used to identify quantity calculation methods
        /// </summary>
        public enum RegisterCalculationMethod : byte
        {
            /// <summary>Energy values are read directly from the base</summary>
            BBV = 0,
            /// <summary>Energy values are cal</summary>
            Summation = 1
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Method attempts to disable the RF and the HAN radios for a total opt out. Opting in will
        /// enabled the RF Radio and will modify the config tag so the CE will reenable HAN if it is
        /// configured as such.
        /// </summary>
        /// <param name="OptSetting">Determines whether the radios are to be disabled or not.</param>
        /// <param name="logoffAndLogon">Method to logoff and log back on to meter.</param>
        /// <returns>The result of the operation</returns>
        public ToggleRadioCommsResult ConfigureRadioComms(Opt OptSetting, Func<bool> logoffAndLogon)
        {
            ToggleRadioCommsResult OpResult = ToggleRadioCommsResult.ERROR;

            try
            {
                ProcedureResultCodes CommReconfigureResult = ProcedureResultCodes.INVALID_PARAM;
                ProcedureResultCodes DeregisterResult = ProcedureResultCodes.INVALID_PARAM;
                ProcedureResultCodes RegisterResult = ProcedureResultCodes.INVALID_PARAM;
                ItronDeviceResult ZigBeeReconfigResult = ItronDeviceResult.ERROR;
                bool RFCommsCorrect = false;
                bool HANCommsCorrect = false;
                Opt OppositeOpSetting = Opt.In == OptSetting ? Opt.Out : Opt.In;

                OnShowProgress(new ShowProgressEventArgs(1, 4, "Opt Out", "Reconfiguring Radio Comms..."));

                OnStepProgress(new ProgressEventArgs());

                if (Opt.Out == OptSetting)
                {
                    for (int i = 0; i <= DEREGISTER_OPT_OUT_RETRIES; i++)
                    {
                        try
                        {
                            DeregisterResult = Deregister();

                            if (ProcedureResultCodes.COMPLETED == DeregisterResult || ProcedureResultCodes.NOT_FULLY_COMPLETED == DeregisterResult)
                            {
                                break;
                            }
                        }
                        catch { }
                    }
                }

                for (int i = 0; i <= RF_OPT_OUT_RETRIES && false == RFCommsCorrect; i++)
                {
                    for (int j = 0; j < RF_OPT_OUT_PROCEDURE_TIMEOUT_RETRIES; j++)
                    {
                        try
                        {
                            CommReconfigureResult = ConfigureRFComms(OptSetting);
                        }
                        catch
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Retrying ConfigureRFComms");
                            Thread.Sleep(3000);
                            logoffAndLogon();
                        }
                    }
                        
                    //if reconfigure operation indicates successful or we have another retry available then wait 30 sec.
                    //so wait before verifying or retrying op. just don't want to wait if we failed and have no more retries.
                    if (ProcedureResultCodes.COMPLETED == CommReconfigureResult || i < RF_OPT_OUT_RETRIES)
                    {
                        //Wait 30 seconds before verifiying success
                        SendWait();
                        Thread.Sleep(30000);
                    }

                    if (ProcedureResultCodes.COMPLETED == CommReconfigureResult)
                    {
                        RFCommsCorrect = VerifyRFComms(OptSetting);
                    }

                    if (false == RFCommsCorrect)
                    {
                        for (int j = 0; j < RF_OPT_OUT_PROCEDURE_TIMEOUT_RETRIES; j++)
                        {
                            try
                            {
                                CommReconfigureResult = ConfigureRFComms(OppositeOpSetting);
                            }
                            catch
                            {
                                m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Retrying ConfigureRFComms");
                                Thread.Sleep(3000);
                                logoffAndLogon();
                            }
                        }
                            SendWait();

                        //Wait 30 seconds before trying again
                        Thread.Sleep(30000);
                    }
                }

                OnStepProgress(new ProgressEventArgs());

                if (false == RFCommsCorrect)
                {
                    if (CommReconfigureResult == ProcedureResultCodes.NO_AUTHORIZATION)
                    {
                        OpResult = ToggleRadioCommsResult.SECURITY_ERROR;
                    }
                    else
                    {
                        OpResult = ToggleRadioCommsResult.RF_RADIO_CONFIG_ERROR;
                    }

                    //skip HAN reconfig if we failed already
                }
                else  //Yes! we reconfigured comms successfully. Now onto HAN!!! 
                {
                    if (Opt.Out == OptSetting)
                    {
                        //Is han disabled already
                        bool InitialConfigurationHANEnabled = IsZigBeeEnabled;

                        HANCommsCorrect = (true == HANRadioOff);

                        for (int i = 0; i <= HAN_OPT_OUT_RETRIES && false == HANCommsCorrect; i++)
                        {
                            //disable han if enabled
                            if (InitialConfigurationHANEnabled)
                            {
                                ZigBeeReconfigResult = EnableDisableZigBeePermanently(DISABLE_ZIGBEE);

                                logoffAndLogon();
                            
                                HANCommsCorrect = (true == HANRadioOff);

                                if (false == HANCommsCorrect)
                                {
                                    ZigBeeReconfigResult = EnableDisableZigBeePermanently(ENABLE_ZIGBEE);

                                    logoffAndLogon();
                                }
                            }
                        }

                        OnStepProgress(new ProgressEventArgs());

                        if (true == HANCommsCorrect)
                        {                         
                            OpResult = ToggleRadioCommsResult.SUCCESS;
                        }
                        else
                        {
                            if (ZigBeeReconfigResult == ItronDeviceResult.SECURITY_ERROR)
                            {
                                OpResult = ToggleRadioCommsResult.SECURITY_ERROR;
                            }
                            else
                            {
                                OpResult = ToggleRadioCommsResult.HAN_RADIO_CONFIG_ERROR;
                            }
                        }
                    }
                    else if (Opt.In == OptSetting)
                    {
                        string NewConfigTag = "1Z1111111111111111111111==";
                        bool ConfigTagCorrect = false;

                        for (int i = 0; i <= CONFIG_TAG_WRITE_OPT_IN_RETRIES && false == ConfigTagCorrect; i++)
                        {
                            //Write config tag
                            ConfigureConfigTag(NewConfigTag);

                            //Verify
                            if (NewConfigTag == ConfigTag)
                            {
                                ConfigTagCorrect = true;
                            }
                        }

                        //Stepping twice to be consistent with opt out.
                        OnStepProgress(new ProgressEventArgs());                        

                        if (ConfigTagCorrect)
                        {
                            OpResult = ToggleRadioCommsResult.SUCCESS;
                        }
                        else
                        {
                            OpResult = ToggleRadioCommsResult.CONFIG_TAG_WRITE_ERROR;
                        }
                    }
                }

                if (Opt.Out == OptSetting && ToggleRadioCommsResult.SUCCESS != OpResult)
                {
                    for (int i = 0; i <= REGISTER_OPT_OUT_RETRIES; i++)
                    {
                        try
                        {
                            RegisterResult = Register();

                            if (ProcedureResultCodes.COMPLETED == RegisterResult || ProcedureResultCodes.NOT_FULLY_COMPLETED == RegisterResult)
                            {
                                break;
                            }
                        }
                        catch { }
                    }
                }

                OnStepProgress(new ProgressEventArgs());
            }
            catch { }
            finally
            {
                OnHideProgress(new EventArgs());
            }

            return OpResult;
        }

        /// <summary>
        /// Verifys that RF comm module is consistent with the opt setting.
        /// </summary>
        /// <param name="OptSetting">Determines whether the radios are to be disabled or not.</param>
        /// <returns>Whether or not comm module is consistent with opt setting.</returns>
        public bool VerifyRFComms(Opt OptSetting)
        {
            bool AsExpected = false;

            try
            {
                AsExpected = RFOptStatus == OptSetting;
            }
            catch
            {
                AsExpected = false;
            }

            return AsExpected;
        }

        /// <summary>
        /// RF Opt Out, configure RF comms
        /// Mfg procedure 119
        /// </summary>
        /// <param name="OptSetting">Determines whether the RF comms are to be disabled or not.</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/11 MSC  2.53.03        Created
        public virtual ProcedureResultCodes ConfigureRFComms(Opt OptSetting)
        {
            ProcedureResultCodes result = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam1;
            byte[] ProcResponse1;
            ProcParam1 = new byte[2];
            ProcParam1[0] = 0x01; //will enable the RF comm module
            ProcParam1[1] = 0x00;

            //Undecided is not valid for configuration. Return error.
            if (Opt.Undecided == OptSetting)
            {
                return result;
            }

            if (Opt.Out == OptSetting)
            {
                ProcParam1[0] = 0xFF;   //will disable the RF comm module
            }

            //check if the c1222 link is busy. if so, try to wait until it isn't before performing procedure.
            for (int i = 0; i < 3; i++)
            {
                if (IsC1222LinkBusy)
                {
                    Thread.Sleep(2000);
                }
                else
                {
                    break;
                }
            }

            result = ExecuteProcedure(Procedures.RFLAN_OPT_OUT, ProcParam1, out ProcResponse1);

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

                result = ExecuteProcedure(Procedures.RESET_RF_LAN, ProcParam.ToArray(), out ProcResponse);
            }

            return result;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/06 mrj 7.30.00 N/A    Created
        // 09/28/06 AF  7.40.00 N/A    Added resource manager call
        // 02/17/11 AF  7.50.04        Removed commented out code
        // 09/25/15 jrf 4.21.05 607438 Adding base energy variables.
        public CENTRON_AMI(Itron.Metering.Communications.ICommunications ceComm)
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
            m_HANInfo = new HANInformation(m_PSEM, this);
            m_BaseEnergyConfigurationSupported = new CachedBool();
            m_BaseSuppliedEnergy1 = new CachedValue<BaseEnergies>();
            m_BaseSuppliedEnergy2 = new CachedValue<BaseEnergies>();

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/06 mrj 7.30.00 N/A    Created
        // 09/28/06 AF  7.40.00 N/A    Added resource manager call
        // 02/17/11 AF  7.50.04        Removed commented out code
        // 09/25/15 jrf 4.21.05 607438 Adding base energy variables.
        public CENTRON_AMI(CPSEM PSEM)
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
            m_lstDSTDates = new List<CDSTDatePair>();
            m_TOUSchedule = null;
            m_HANInfo = new HANInformation(PSEM, this);
            m_BaseEnergyConfigurationSupported = new CachedBool();
            m_BaseSuppliedEnergy1 = new CachedValue<BaseEnergies>();
            m_BaseSuppliedEnergy2 = new CachedValue<BaseEnergies>();
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
        /// <param name="Wait">Whether or not to send a wait to the device before calling 
        /// method to validate optical passwords.</param>
        /// <returns>list of bool items</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ------------------------------------------------------------
        //  11/20/09 MMD          QC Tool    Created
        //  10/11/16 jrf 4.60.11 722267 Adding wait in case call to validate optical passwords hangs while 
        //                              attempting to access config file.
        public bool ValidateOpticalPasswords(string ProgName, bool Wait = false)
        {
            bool bKeysValid = true;
            for (int iKey = 0; iKey < 4; iKey++)
            {
                //This was added to attempt to prevent factory QC Tool issues with meter timing out.
                if (true == Wait)
                {
                    SendWait();
                }

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
        /// Resets the SiteScan Diagnostic Counters.
        /// </summary>
        /// <returns>The result of the reset.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#   Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/09 RCG 2.20.07 136014 Adding AMI Tables
        protected virtual ItronDeviceResult ResetDiagnosticCounters()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            //Execute the reset conters MFG procedure 
            byte[] byParameter = BitConverter.GetBytes((uint)AMI_Reset_Counter_Types.RESET_SITESCAN_DIAG);
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
        /// Resets the Number of Removal tampers
        /// </summary>
        /// <returns>ItronDeviceResult.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/20/12 jrf 2.70.19 TQ6835 Created
        //
        public virtual ItronDeviceResult ResetNumberMagneticTampers()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            //Execute the reset counters MFG procedure 
            byte[] byParameter = BitConverter.GetBytes((uint)AMI_Reset_Counter_Types.RESET_NUM_MAGNETIC_TAMPERS);
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
        // 07/16/14 jrf  4.00.47 523549 Procedure response contains time before and after change. Adding 
        //                              this data to the comm log.
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

                        try
                        {
                            //Display the set date time response
                            MemoryStream ResponseStream = new MemoryStream(ProcResponse);
                            PSEMBinaryReader Reader = new PSEMBinaryReader(ResponseStream);
                            DateTime TimeBefore = Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                            DateTime TimeAfter = Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);

                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                           "Date time before = " + TimeBefore);
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                           "Date time after = " + TimeAfter);
                        }
                        catch { }
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
        /// 
        /// </summary>
        /// <param name="InputNormalKh"></param>
        /// <returns></returns>
        public ConfigurationResult ConfigureKh(int InputNormalKh)
        {
            byte[] ProcParam = new byte[0];
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult =
                ProcedureResultCodes.INVALID_PARAM;
            PSEMResponse Result = PSEMResponse.Err;

            ConfigurationResult ConfigResult = ConfigurationResult.ERROR;
            CTable2048_OpenWay OW2048 = Table2048 as CTable2048_OpenWay;
            CENTRON_AMI_Metrology MetTable = OW2048.MetrologyTable;

            // Open the Config
            ProcResult = ExecuteProcedure(Procedures.OPEN_CONFIG_FILE, ProcParam, out ProcResponse);

            // Execute Write of Metrology Table in 2048
            if (ProcedureResultCodes.COMPLETED == ProcResult)
            {
                MetTable.NormalKh = InputNormalKh;
                Result = MetTable.Write();
            }
            else if (ProcedureResultCodes.NO_AUTHORIZATION == ProcResult)
            {
                Result = PSEMResponse.Isc;
            }
            else
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "Open config procedure failed with result = " +
                    ProcResult);
                Result = PSEMResponse.Err;
            }

            if (Result == PSEMResponse.Ok)
            {
                // Close the Config
                // Data reset bits - we don't want to reset any data, so 
                // just initialize them to 0
                ProcParam = new byte[4];
                ProcParam.Initialize();
                ProcResult = ExecuteProcedure(Procedures.CLOSE_CONFIG_FILE, ProcParam, out ProcResponse);

                if (ProcedureResultCodes.COMPLETED != ProcResult)
                {
                    ConfigResult = ConfigurationResult.ERROR;
                }
                else
                {
                    ConfigResult = ConfigurationResult.SUCCESS;
                }
            }
            else
            {
                if (Result == PSEMResponse.Isc)
                {
                    ConfigResult = ConfigurationResult.SECURITY_ERROR;
                }
                else
                {
                    ConfigResult = ConfigurationResult.ERROR;
                }
            }

            return ConfigResult;
        }

        /// <summary>
        /// Switches the currently active service limiting threshold
        /// </summary>
        /// <param name="threshold">The number of the thershold to switch to.</param>
        /// <param name="thresholdPeriod">The amount of time to stay in this thershold.</param>
        /// <returns>The result of the procedure.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/20/08 RCG 2.00.09        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual ProcedureResultCodes SwitchActiveThreshold(byte threshold, TimeSpan thresholdPeriod)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;
            byte[] ProcParam = new byte[1 + Table00.TIMESize];
            byte[] ProcResponse;
            PSEMBinaryWriter PSEMWriter = new PSEMBinaryWriter(new MemoryStream(ProcParam));

            // The size of the TIME object can vary so we need to use the PSEMBinaryWriter
            PSEMWriter.Write(threshold);
            PSEMWriter.WriteTIME(thresholdPeriod, (PSEMBinaryReader.TM_FORMAT)Table00.TimeFormat);

            ProcResult = ExecuteProcedure(Procedures.SWITCH_ACTIVE_THRESHOLD, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Perform a Periodic Read
        ///  NOTE: At this time this procedure just causes the meter to do the action.  It does not
        ///         look at any of the return data.  If this functionality is needed it will need to be added.
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="WindowEndTime"></param>
        /// <param name="Actions"></param>
        /// <returns></returns>
        public ItronDeviceResult PerformPeriodicRead(DateTime StartTime, DateTime EndTime, DateTime WindowEndTime,
                                                        PeriodicReadActions Actions)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            DateTime ReferenceDate = new DateTime(1970, 1, 1);
            TimeSpan Span = new TimeSpan();
            uint Minutes;
            byte[] byMinutes;
            byte[] byAction;
            int iIndex = 0;
            byte[] byParameters = new byte[14];
            UInt16 PeriodicActions = 0;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;

            //Convert the Start Time to minutes and seconds since 1/1/1970					
            Span = StartTime - ReferenceDate;
            Minutes = (uint)Span.TotalMinutes;
            byMinutes = BitConverter.GetBytes(Minutes);
            Array.Copy(byMinutes, 0, byParameters, iIndex, byMinutes.Length);
            iIndex += byMinutes.Length;

            //Convert the End Time to minutes and seconds since 1/1/1970					
            Span = EndTime - ReferenceDate;
            Minutes = (uint)Span.TotalMinutes;
            byMinutes = BitConverter.GetBytes(Minutes);
            Array.Copy(byMinutes, 0, byParameters, iIndex, byMinutes.Length);
            iIndex += byMinutes.Length;

            //Convert the Window Time to minutes and seconds since 1/1/1970					
            Span = WindowEndTime - ReferenceDate;
            Minutes = (uint)Span.TotalMinutes;
            byMinutes = BitConverter.GetBytes(Minutes);
            Array.Copy(byMinutes, 0, byParameters, iIndex, byMinutes.Length);
            iIndex += byMinutes.Length;

            // Build up Periodic Actions
            if (Actions.bOnDemandPeriodicRead == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.ON_DEMAND_PERIODIC_READ;
            }
            if (Actions.bPerformDRAtEndTime == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.PERFROM_DR_AT_ENDTIME;
            }
            if (Actions.bReportDRSnapshot == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_DR_SNAPSHOT;
            }
            if (Actions.bReportFWDLStatusBlock == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_FWDL_STATUS_BLOCK;
            }
            if (Actions.bReportHistoryLog == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_HISTORY_LOG;
            }
            if (Actions.bReportLoadProfile == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_LOAD_PROFILE;
            }
            if (Actions.bReportMostRecentSR == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_MOST_RECENT_SR;
            }
            if (Actions.bReportNetworkStatus == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_NETWORK_STATUS;
            }
            if (Actions.bReportPerformSR == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_PERFORM_SR;
            }
            if (Actions.bReportTable28 == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_TABLE_28;
            }
            if (Actions.bReportTable6 == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_TABLE_06;
            }
            if (Actions.bReportVoltageMonitoring == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_VOLT_MONITORING;
            }
            if (Actions.bReportZigBee == true)
            {
                PeriodicActions += (UInt16)Periodic_Read_Action_Types.REPORT_ZIGBEE;
            }

            // Copy Actions into Paramater Array
            byAction = BitConverter.GetBytes(PeriodicActions);
            Array.Copy(byAction, 0, byParameters, iIndex, byAction.Length);
            iIndex += byAction.Length;

            //Execute the set date time procedure
            ProcResult = ExecuteProcedure(Procedures.PERFORM_PERIODIC_READ,
                                           byParameters,
                                           out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                case ProcedureResultCodes.UNRECOGNIZED_PROC:
                    {
                        Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
                        break;
                    }
                default:
                    {
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;
        }

        /// <summary>
        /// Formats the flash.
        /// </summary>
        /// <returns>
        /// An ItronDeviceResult representing the result of the reset
        /// operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/30/13 MP  2.80.18		Created
        //  10/01/14 AF  4.00.64        Removed the duplicate enum value for proc 2091
        //
        public ItronDeviceResult FormatFlash()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(12);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] ProcResponse;

            ParamWriter.Write(FORMAT_FLASH_FUNC_CODE);
            ParamWriter.Write(FORMAT_FLASH_PARAM_1);
            ParamWriter.Write(FORMAT_FLASH_PARAM_2);

            ProcResult = ExecuteProcedureAndWaitForResult(Procedures.RESET_RF_LAN,
                ProcParam.ToArray(), out ProcResponse);

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
        /// Convert a utc time from the meter to local time for the device
        /// </summary>
        /// <param name="utcTime">UDT time from the meter</param>
        /// <returns>Convertered Device Local Time</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue#         Description
        //  -------- --- ------- -------------  ---------------------------------------
        //  06/23/08 KRC 1.50.41 itron00116601  Change so we only adjust for versions prior to 1.5       
        //
        public override DateTime GetLocalDeviceTime(DateTime utcTime)
        {
            DateTime LocalDateTime = utcTime;

            // We can only do these functions in the 3.5 framework, which currently does not work in CE.
#if (!WindowsCE)
            // if (FWRevision < VERSION_1_5_RELEASE_1)
            if (VersionChecker.CompareTo(FWRevision, VERSION_1_5_RELEASE_1) < 0)
            {
                //We do not need to adjust the time for Version 1.5
                LocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, DeviceTimeZoneInfo);
            }
#else
                LocalDateTime = utcTime;
#endif

            return LocalDateTime;
        }

        /// <summary>
        /// Updates the Network Security Key and the Global Link Key based on the 
        /// encrypted values passed in.
        /// </summary>
        /// <param name="strEncryptedNetworkKey">
        /// The encrypted network security key
        /// </param>
        /// <param name="strEncryptedLinkKey">
        /// The encrypted global link key
        /// </param>
        /// <returns>A PSEM Response for the write to Table 2105</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 AF  1.50.36        Created
        //  06/17/08 AF  1.50.37        Simplified to use SetHANSecurityKeys which
        //                              does not read the table
        //  06/24/08 AF  1.50.42        Simplified even more -- does not need to use HANKeyRcds to
        //                              store the data.
        //
        public PSEMResponse UpdateHANSecurityKeys(string strEncryptedNetworkKey, string strEncryptedLinkKey)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;

            try
            {

                Table2105.SetHANSecurityKeys(DecryptHANKey(strEncryptedNetworkKey), DecryptHANKey(strEncryptedLinkKey));

                PSEMResult = Table2105.Write();
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return PSEMResult;
        }

        /// <summary>
        /// Updates the Network Security Key based on the encrypted values passed in.
        /// </summary>
        /// <param name="strEncryptedNetworkKey"></param>
        /// <returns>A PSEM Response for the write to Table 2105</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  10/24/16 AF  4.70.27  WR 699119  Created. No longer need to consider link key.
        //
        public PSEMResponse UpdateHANNetworkSecurityKey(string strEncryptedNetworkKey)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;

            try
            {
                Table2105.SetHANNetworkSecurityKey(DecryptHANKey(strEncryptedNetworkKey));

                PSEMResult = Table2105.Write();
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return PSEMResult;
        }

        /// <summary>
        /// Updates the Network Security Key and the Global Link Key based on the 
        /// unencrypted values passed in.
        /// </summary>
        /// <param name="byaNetworkKey">network key</param>
        /// <param name="byaLinkKey">link key</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/26/16 AF  4.70.19  WR 712299  Created
        //
        public PSEMResponse UpdateHANSecurityKeys(byte[] byaNetworkKey, byte[] byaLinkKey)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;

            try
            {
                Table2105.SetHANSecurityKeys(byaNetworkKey, byaLinkKey);

                PSEMResult = Table2105.Write();
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return PSEMResult;
        }

        /// <summary>
        /// This method forces the meter to synchronize its time with the RFLAN
        /// if the meter's time is out of sync by a value greater than hysteresis.
        /// </summary>
        /// <param name="usHysteresis">The number of seconds out of sync the meter must be 
        /// greater than for the time sync to be allowed.</param>
        /// <returns>The result of the forced time sync.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/09 jrf 2.20.03        Created
        //  10/11/12 jrf 2.70.28 235804 Setting a default hysteresis of 1.
        //
        public ForceTimeSyncResult ForceTimeSync(UInt16 usHysteresis = 1)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;
            ForceTimeSyncResult Result = ForceTimeSyncResult.ERROR;
            byte[] abyParameter = BitConverter.GetBytes(usHysteresis);
            byte[] abyProcResponse;

            ProcResult = ExecuteProcedure(Procedures.FORCE_TIME_SYNC, abyParameter, out abyProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ForceTimeSyncResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NOT_FULLY_COMPLETED:
                    {
                        //The time sync is in progress
                        Result = ForceTimeSyncResult.TIME_SYNC_IN_PROGRESS;
                        break;
                    }
                case ProcedureResultCodes.INVALID_PARAM:
                    {
                        //Parameters were invalid or table 7 write failed
                        Result = ForceTimeSyncResult.INVALID_PARAMETERS;
                        break;
                    }
                case ProcedureResultCodes.DEVICE_SETUP_CONFLICT:
                    {
                        //Amount time is out of sync is less than hysteresis
                        Result = ForceTimeSyncResult.OUT_OF_SYNC_LESS_THAN_HYSTERESIS;
                        break;
                    }
                case ProcedureResultCodes.TIMING_CONSTRAINT:
                    {
                        //The device was busy
                        Result = ForceTimeSyncResult.DEVICE_BUSY;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ForceTimeSyncResult.SECURITY_ERROR;
                        break;
                    }
                case ProcedureResultCodes.UNRECOGNIZED_PROC:
                    {
                        //Unsupported operation
                        Result = ForceTimeSyncResult.UNSUPPORTED_OPERATION;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ForceTimeSyncResult.ERROR;
                        break;
                    }
            }

            return Result;
        }

        /// <summary>
        /// This method forces the meter to synchronize its time with the RFLAN
        /// if the meter's time is out of sync by a value greater than hysteresis.
        /// </summary>
        /// <param name="usHysteresis">The number of seconds out of sync the meter must be 
        /// greater than for the time sync to be allowed.</param>
        /// <returns>The result of the forced time sync.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/15/10 AF  2.40.07 146825 Created
        public ForceTimeSyncResult ForceTimeSyncAndWaitForResult(UInt16 usHysteresis)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;
            ForceTimeSyncResult Result = ForceTimeSyncResult.ERROR;
            byte[] abyParameter = BitConverter.GetBytes(usHysteresis);
            byte[] abyProcResponse;

            ProcResult = ExecuteProcedureAndWaitForResult(Procedures.FORCE_TIME_SYNC, abyParameter, out abyProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ForceTimeSyncResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NOT_FULLY_COMPLETED:
                    {
                        //The time sync is in progress
                        Result = ForceTimeSyncResult.TIME_SYNC_IN_PROGRESS;
                        break;
                    }
                case ProcedureResultCodes.INVALID_PARAM:
                    {
                        //Success
                        Result = ForceTimeSyncResult.INVALID_PARAMETERS;
                        break;
                    }
                case ProcedureResultCodes.DEVICE_SETUP_CONFLICT:
                    {
                        //Amount time is out of sync is less than hysteresis
                        Result = ForceTimeSyncResult.OUT_OF_SYNC_LESS_THAN_HYSTERESIS;
                        break;
                    }
                case ProcedureResultCodes.TIMING_CONSTRAINT:
                    {
                        //The device was busy
                        Result = ForceTimeSyncResult.DEVICE_BUSY;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ForceTimeSyncResult.SECURITY_ERROR;
                        break;
                    }
                case ProcedureResultCodes.UNRECOGNIZED_PROC:
                    {
                        //Unsupported operation
                        Result = ForceTimeSyncResult.UNSUPPORTED_OPERATION;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ForceTimeSyncResult.ERROR;
                        break;
                    }
            }

            return Result;
        }

        /// <summary>
        /// Issues security using the Signed Authorization key
        /// </summary>
        /// <param name="key">The key to use for authorization</param>
        /// <returns>The result of the procedure.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/16/09 RCG 2.30.10	    Created
        //  02/10/14 jrf 3.50.32 419257 Storing last used signed auth. key
        public ProcedureResultCodes Authenticate(SignedAuthorizationKey key)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;

            if (key != null && key.Data != null)
            {
                ProcResult = ExecuteProcedure(Procedures.AUTHENTICATE, key.Data, out ProcResponse);
                //Storing key data to allow a reauthentication
                SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);
                DataStorage.StoreSecureData(SecureDataStorage.SIGNED_AUTHORIZATION_KEY, key.Data);
            }

            return ProcResult;
        }

        /// <summary>
        /// Issues security using the last used Signed Authorization key
        /// </summary>
        /// <returns>The result of the procedure.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/10/14 jrf 3.50.32 419257 Created.
        public ProcedureResultCodes Reauthenticate()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            byte[] KeyData = null;

            SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);
            KeyData = DataStorage.RetrieveSecureData(SecureDataStorage.SIGNED_AUTHORIZATION_KEY);

            if (KeyData != null)
            {
                ProcResult = ExecuteProcedure(Procedures.AUTHENTICATE, KeyData, out ProcResponse);
            }

            return ProcResult;
        }

        /// <summary>
        /// Disables Signed Authorization for the specified number of minutes.
        /// </summary>
        /// <param name="usMinToDisable">The number of minutes to disable signed authorization for.</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/04/09 RCG 2.30.16	    Created

        public ProcedureResultCodes DisableSignedAuthorization(ushort usMinToDisable)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] Params = BitConverter.GetBytes(usMinToDisable);
            byte[] ProcResponse;

            ProcResult = ExecuteProcedure(Procedures.DISABLE_SIGNED_AUTHORIZATION, Params, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Creates a core dump from the meter if present and saves it to the specified file.
        /// </summary>
        /// <param name="strFileName">The location to save the Core Dump.</param>
        /// <returns>The result of the operation.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        //
        public virtual ItronDeviceResult CreateCoreDump(string strFileName)
        {
            ItronDeviceResult Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
            int iNumberOfSteps = 0;

            if (IsFullCoreDumpPresent)
            {
                FileStream CoreDumpFile = File.Create(strFileName);
                PSEMBinaryWriter Writer = new PSEMBinaryWriter(CoreDumpFile);

                // Determine the number of steps
                iNumberOfSteps = (int)(DetermineFullCoreDumpLength() / CORE_DUMP_BLOCK_SIZE + 3);

                OnShowProgress(new ShowProgressEventArgs(1, iNumberOfSteps, "Creating Core Dump", "Creating Core Dump"));

                // Write the header information to the Core Dump file.
                WriteCoreDumpHeader(Writer);

                OnStepProgress(new ProgressEventArgs());

                Result = WriteCoreDumpTables(Writer);

                CoreDumpFile.Close();
            }

            if (Result != ItronDeviceResult.SUCCESS && File.Exists(strFileName))
            {
                File.Delete(strFileName);
            }

            OnHideProgress(new EventArgs());

            return Result;
        }

        /// <summary>
        /// Reconfigures the C12.18 Over ZigBee Enabled bit
        /// </summary>
        /// <param name="bEnable">Whether to enable or disable C12.18 over ZigBee</param>
        /// <returns>The result of the reconfigure</returns>
        /// <remarks>
        /// NOTE!! This reconfigure will not take effect until after logging off of the meter
        /// as it does not use a procedure or call Open/Close config.
        /// </remarks>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/27/10 RCG 2.44.06 N/A    Created

        public virtual ItronDeviceResult ReconfigureC1218OverZigBee(bool bEnable)
        {
            PSEMResponse Response;
            ItronDeviceResult DeviceResult = ItronDeviceResult.UNSUPPORTED_OPERATION;

            if (Table2193 != null)
            {
                // Make sure we force a read of the table so that it is up to date
                Response = Table2193.Read();

                if (Response == PSEMResponse.Ok)
                {
                    // Reconfigure the bit
                    Table2193.IsC1218OverZigBeeEnabled = bEnable;
                    Response = Table2193.Write();

                    if (Response == PSEMResponse.Ok)
                    {
                        DeviceResult = ItronDeviceResult.SUCCESS;
                    }
                }

                if (Response == PSEMResponse.Isc)
                {
                    DeviceResult = ItronDeviceResult.SECURITY_ERROR;
                }
                else if (Response != PSEMResponse.Ok)
                {
                    DeviceResult = ItronDeviceResult.ERROR;
                }
            }

            return DeviceResult;
        }

        /// <summary>
        /// Reconfigures the C12.18 Over ZigBee Enabled bit
        /// </summary>
        /// <param name="disabled">Whether to disable ZigBee.</param>
        /// <returns>The result of the reconfigure</returns>
        /// <remarks>
        /// NOTE!! This reconfigure will not take effect until after logging off of the meter
        /// as it does not use a procedure or call Open/Close config.
        /// </remarks>
        public virtual ItronDeviceResult ReconfigureZigBeeDisabled(bool disabled)
        {
            PSEMResponse Response;
            ItronDeviceResult DeviceResult = ItronDeviceResult.UNSUPPORTED_OPERATION;

            if (Table2193 != null)
            {
                // Make sure we force a read of the table so that it is up to date
                Response = Table2193.Read();

                if (Response == PSEMResponse.Ok)
                {
                    // Reconfigure the bit
                    Table2193.IsZigBeeDisabled = disabled;
                    Response = Table2193.Write();

                    if (Response == PSEMResponse.Ok)
                    {
                        DeviceResult = ItronDeviceResult.SUCCESS;
                    }
                }

                if (Response == PSEMResponse.Isc)
                {
                    DeviceResult = ItronDeviceResult.SECURITY_ERROR;
                }
                else if (Response != PSEMResponse.Ok)
                {
                    DeviceResult = ItronDeviceResult.ERROR;
                }
            }

            return DeviceResult;
        }

        /// <summary>
        /// Clears the optional items that can removed from a periodic read.  
        /// Initially this is used for removing optional items included as 
        /// part of asset synchronization.
        /// </summary>
        /// <remarks>This procedure requires level 5 security, otherwise it will fail.</remarks>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/29/10 jrf 2.40.46        Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        //
        public virtual ProcedureResultCodes ClearPeriodicReadOptionalData(PeriodicReadRemovableItems ItemsToRemove)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam = new byte[] { (byte)ItemsToRemove };
            byte[] ProcResponse;

            ProcResult = ExecuteProcedure(Procedures.PERIODIC_READ_REMOVAL,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Reconfigures the 25 Year DST Calendar.
        /// </summary>
        /// <param name="dstHour">The DST change hour</param>
        /// <param name="dstMinute">The DST change minute</param>
        /// <param name="dstOffset">The DST change offset</param>
        /// <param name="dstDates">The list of DST dates.</param>
        /// <returns>The result of the reconfigure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public TOUReconfigResult Reconfigure25YearDST(byte dstHour, byte dstMinute, byte dstOffset, List<CDSTDatePair> dstDates)
        {
            TOUReconfigResult Result = TOUReconfigResult.SUCCESS_DST_NOT_SUPPORTED;
            PSEMResponse Response = PSEMResponse.Ok;

            if (Supports25YearDST)
            {
                Table2260DSTConfig.DSTHour = dstHour;
                Table2260DSTConfig.DSTMinute = dstMinute;
                Table2260DSTConfig.DSTOffset = dstOffset;
                Table2260DSTConfig.DSTDates = dstDates;

                Response = Table2260DSTConfig.Write();

                switch (Response)
                {
                    case PSEMResponse.Ok:
                        {
                            Result = TOUReconfigResult.SUCCESS;
                            break;
                        }
                    case PSEMResponse.Isc:
                        {
                            Result = TOUReconfigResult.INSUFFICIENT_SECURITY_ERROR;
                            break;
                        }
                    default:
                        {
                            Result = TOUReconfigResult.PROTOCOL_ERROR;
                            break;
                        }
                }
            }

            return Result;
        }

        /// <summary>
        /// Writes a new ECC certifiate and keys to dataflash.
        /// </summary>
        /// <param name="abytECCCertificate">The ECC certificate that is specific to the meter.</param>
        /// <param name="abytPublicKey">The ECC public key that is not meter specific.</param>
        /// <param name="abytPrivateKey">The ECC private key that is specific to the meter.</param>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/05/11 jrf 2.45.22		Created 
        //
        public ProcedureResultCodes UpdateHANECCCertificate(byte[] abytECCCertificate, byte[] abytPublicKey, byte[] abytPrivateKey)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(92);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] abytProcResponse;
            byte bytFlags = 0x00;

            ParamWriter.Write(abytECCCertificate);
            ParamWriter.Write(abytPublicKey);
            ParamWriter.Write(abytPrivateKey);
            ParamWriter.Write(bytFlags);

            ProcResult = ExecuteProcedure(Procedures.HAN_CERTIFICATE_UPDATE,
                ProcParam.ToArray(), out abytProcResponse);

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
        /// Resets LAN and HAN event logs.
        /// </summary>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/08/11 jrf 2.50.02		Created 
        //
        public ProcedureResultCodes ResetLANHANCommLogs()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] abytProcParam = new byte[0];
            byte[] abytProcResponse;

            ProcResult = ExecuteProcedure(Procedures.RESET_LAN_HAN_COMM_LOGS,
                abytProcParam, out abytProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Configure CPP
        /// </summary>
        /// <param name="startTimeGmt">Start Time (GMT)</param>
        /// <param name="duration">Duration in minute</param>
        /// <returns></returns>
        public virtual ConfigCppResult ConfigCpp(DateTime startTimeGmt, UInt16 duration)
        {
            ConfigCppResult Result = ConfigCppResult.ErrorOrCPPInvalidDuration;
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
                            Result = ConfigCppResult.ErrorOrCPPInvalidDuration;
                            break;
                        }
                        break;
                    }
                    default:
                    {
                        //General Error
                        Result = ConfigCppResult.ErrorOrCPPInvalidDuration;
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
        /// Validate the base energies.
        /// </summary>
        /// <param name="blnEnergyConfigSuppported">Whether or not the configured energies are supported by the base.</param>
        /// <param name="lstSuppliedEnergies">The energies supplied by the base.</param>
        /// <returns>The procedure result.</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/17/11 jrf 2.50.09 n/a    Created.
        //
        public virtual ProcedureResultCodes ValidateBaseEnergies(out bool blnEnergyConfigSuppported, out List<BaseEnergies> lstSuppliedEnergies)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            byte[] abytProcParam = new byte[0];

            ProcResult = ExecuteProcedure(Procedures.VALIDATE_BASE_ENERGIES, abytProcParam, out ProcResponse);

            if (1 <= ProcResponse.Length)
            {
                blnEnergyConfigSuppported = Convert.ToBoolean(ProcResponse[0]);
            }
            else
            {
                blnEnergyConfigSuppported = false;
            }

            lstSuppliedEnergies = new List<BaseEnergies>();

            //Mono has two but poly can return 6 energies.
            for (int i = 1; i < ProcResponse.Length; i++)
            {
                if (true == Enum.IsDefined(typeof(BaseEnergies), ProcResponse[i]))
                {
                    lstSuppliedEnergies.Add((BaseEnergies)ProcResponse[i]);
                }
            }

            return ProcResult;
        }

        /// <summary>
        /// Resets the HAN in HW 3.0 or later meters
        /// </summary>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.
        public ProcedureResultCodes ResetHAN()
        {
            return ResetHAN(HANResetMethod.PerformReset, 0, 0);
        }

        /// <summary>
        /// Clear Reset Limiting Induced Halt Condition in HAN
        /// </summary>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/21/12 PGH 2.70.19        Created.
        public ProcedureResultCodes ClearResetLimitingHaltCondition()
        {
            return ResetHAN(HANResetMethod.ClearResetLimitingHaltCondition, 0, 0);
        }

        /// <summary>
        /// Publishes a recurring price to the meter.
        /// </summary>
        /// <param name="expirationDate">The expiration date of the recurring price</param>
        /// <param name="prices">The list of prices to write to the meter.</param>
        /// <param name="tiers">The list of tiers to write to the meter.</param>
        /// <returns>The result of the write</returns>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  05/16/12 RCG 2.60.24          Created
        public PSEMResponse PublishRecurringHANPricing(DateTime expirationDate, List<AMIHANPriceEntryRcd> prices, List<AMITierLabelEntryRcd> tiers)
        {
            Table2297.Read();

            Table2297.ExpirationDate = expirationDate;

            // Set the Prices
            for (int iIndex = 0; iIndex < Table2297.Prices.Length; iIndex++)
            {
                if (prices != null && iIndex < prices.Count)
                {
                    // We have been given the new price so just load that
                    Table2297.Prices[iIndex] = prices[iIndex];
                }
                else
                {
                    // We don't have a price so lets fill this price with 0's using the constructor
                    Table2297.Prices[iIndex] = new AMIHANPriceEntryRcd();
                }
            }

            // Set the Tiers
            for (int iIndex = 0; iIndex < Table2297.Tiers.Length; iIndex++)
            {
                if (tiers != null && iIndex < tiers.Count)
                {
                    // We have been given the tier so we should use what we were given
                    Table2297.Tiers[iIndex] = tiers[iIndex];
                }
                else
                {
                    // We don't have a tier so set it to all 0's by using the constructor
                    Table2297.Tiers[iIndex] = new AMITierLabelEntryRcd();
                }
            }

            return Table2297.Write();
        }

        /// <summary>
        /// Writes a pricing schedule to a Pending Table
        /// </summary>
        /// <param name="prices">The list of prices to write.</param>
        /// <param name="tiers">The list of tiers to write</param>
        /// <param name="activationDate">The date and time the pending table should activate</param>
        /// <returns>The result of the write</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created
        //  12/13/13 DLG 3.50.15        Updated to use HANInformation object to access Table2134.
        //
        public PSEMResponse PublishPendingHANPricing(List<AMIHANPriceEntryRcd> prices, 
                                                     List<AMITierLabelEntryRcd> tiers, 
                                                     DateTime activationDate)
        {
            PendingEventRecord PendingEvent = new PendingEventRecord(activationDate, 
                                                                     (PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat, 
                                                                     false, false);
            int PricesLength = 0;
            int TiersLength = 0;

            // Read the table first in case we miss something
            m_HANInfo.Table2134.Read();

            PricesLength = m_HANInfo.Table2134.Prices.Length;
            TiersLength = m_HANInfo.Table2134.Tiers.Length;

            // Set the Prices
            for (int iIndex = 0; iIndex < PricesLength; iIndex++)
            {
                if (prices != null && iIndex < prices.Count)
                {
                    // We have been given the new price so just load that
                    m_HANInfo.Table2134.Prices[iIndex] = prices[iIndex];
                }
                else
                {
                    // We don't have a price so lets fill this price with 0's using the constructor
                    m_HANInfo.Table2134.Prices[iIndex] = new AMIHANPriceEntryRcd();
                }
            }

            // Set the Tiers
            for (int iIndex = 0; iIndex < TiersLength; iIndex++)
            {
                if (tiers != null && iIndex < tiers.Count)
                {
                    // We have been given the tier so we should use what we were given
                    m_HANInfo.Table2134.Tiers[iIndex] = tiers[iIndex];
                }
                else
                {
                    // We don't have a tier so set it to all 0's by using the constructor
                    m_HANInfo.Table2134.Tiers[iIndex] = new AMITierLabelEntryRcd();
                }
            }

            return m_HANInfo.Table2134.PendingTableWrite(PendingEvent);
        }

        /// <summary>
        /// Publishes the specified RIB schedule to the meter and commits it for activation.
        /// </summary>
        /// <returns>The result of the publish</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/24/12 RCG 2.60.28        Created
        public ProcedureResultCodes PublishAndCommitRIBSchedule(bool enableBlockPricing, string scheduleID, PublishPriceDataEntryRcd publishPriceData,
            UInt24 multiplier, UInt24 divisor, List<BillingPeriodRcd> billingPeriods, List<NextBlockPeriodRcd> blockPeriods, List<BlockPriceRcd> blockPrices)
        {
            ProcedureResultCodes CommitResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;

            if (SupportsRIB)
            {
                PSEMResponse PublishResponse = PublishRIBScheduleWithoutCommitting(enableBlockPricing, scheduleID, publishPriceData, multiplier,
                    divisor, billingPeriods, blockPeriods, blockPrices);

                if (PublishResponse == PSEMResponse.Ok)
                {
                    CommitResult = HANCommitNextBlockPriceSchedule();
                }
                else
                {
                    CommitResult = ProcedureResultCodes.INVALID_PARAM;
                }
            }
            else
            {
                throw new NotSupportedException("This meter does not support RIB");
            }

            return CommitResult;
        }

        /// <summary>
        /// Publishes the specified RIB schedule to the meter without committing it for activation
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/24/12 RCG 2.60.28        Created        
        public PSEMResponse PublishRIBScheduleWithoutCommitting(bool enableBlockPricing, string scheduleID, PublishPriceDataEntryRcd publishPriceData,
            UInt24 multiplier, UInt24 divisor, List<BillingPeriodRcd> billingPeriods, List<NextBlockPeriodRcd> blockPeriods, List<BlockPriceRcd> blockPrices)
        {
            PSEMResponse Response = PSEMResponse.Err;

            if (Table2441 != null && Table2439 != null)
            {
                Table2439.NextNbrBillingPeriods = (byte)billingPeriods.Count;
                Table2439.NextNbrBlockPeriods = (byte)blockPeriods.Count;
                Table2439.NextNbrBlocks = (byte)publishPriceData.RateLabels.Count;

                Response = Table2439.WriteNextScheduleData();

                if (Response == PSEMResponse.Ok)
                {
                    // The size of the table has likely changed so we need to rebuild it.
                    m_Table2441 = null;

                    ConfigBitFieldRcd ConfigBitField = new ConfigBitFieldRcd();
                    ConfigBitField.BlockPricingEnable = enableBlockPricing;

                    Table2441.Configuration = ConfigBitField;
                    Table2441.ScheduleId = scheduleID;
                    Table2441.PublishPriceData = publishPriceData;
                    Table2441.Multiplier = multiplier;
                    Table2441.Divisor = divisor;
                    Table2441.BillingPeriods = billingPeriods;
                    Table2441.BlockPeriodData = blockPeriods;
                    Table2441.BlockPrices = blockPrices;

                    Response = Table2441.Write();
                }
            }

            return Response;
        }

        /// <summary>
        /// Clears the firmware download event log in HW 3.0 Hydrogen C meters and above
        /// </summary>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/16/11 AF  2.52.03        Created
        //
        public ProcedureResultCodes ClearFirmwareDownloadEventLog()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;

            byte[] ProcResponse;

            byte[] ProcParam = new byte[4];
            ProcParam.Initialize();

            ProcResult = ExecuteProcedure(Procedures.CLEAR_FWDL_EVENT_LOG, ProcParam, out ProcResponse);

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
        public ProcedureResultCodes AuthenticateFWDL(ushort TableID, byte FWType, byte[] HashCode)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcResponse;

            byte[] ProcParam = new byte[35];
            PSEMBinaryWriter PSEMWriter = new PSEMBinaryWriter(new MemoryStream(ProcParam));

            PSEMWriter.Write(TableID);
            PSEMWriter.Write(FWType);
            PSEMWriter.Write(HashCode);

            ProcResult = ExecuteProcedure(Procedures.OPTICAL_FWDL_AUTHENTICATION, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This procedure causes the single phase meter to auto detect the phases it will monitor.
        /// </summary>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/11/11 jrf 2.53.06		Created 
        //
        public ProcedureResultCodes AutoDetectMonoPhases()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] abytProcParam = new byte[0];
            byte[] abytProcResponse;

            ProcResult = ExecuteProcedure(Procedures.MONO_PHASE_AUTO_DETECTION,
                abytProcParam, out abytProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Creates a VMData object from the meter. At this time only enhanced voltage 
        /// monitoring data is retrieved.
        /// </summary>
        /// <returns>The VMData object.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5321  Created
        //  12/20/17 AF  4.73.00 Bug705768 Make sure the number of steps for the progress indicator
        //                               is not negative

        public virtual VMData GetVoltageMonitoringData()
        {
            VMData VoltageData = null;
            ushort usMaxBlocks;
            ushort usUsedBlocks;
            ushort usIntervalsPerBlock;
            ushort usValidIntervals;
            ushort usLastBlock;
            ushort usStartBlock;
            ushort usDivisor;
            ushort usScalar;
            byte byIntervalLength;
            byte byNumPhases;
            List<VMInterval> Intervals = new List<VMInterval>();
            VMStatusFlags LastIntervalStatus;

            // Make sure that the meter contains voltage monitoring data
            if (null != Table2157)
            {
                // Tables are there now check to see if VM is enabled.
                if (true == Table2154.VoltageMonitoringEnabled)
                {
                    // VM is enabled so we should be able to create the object.
                    VoltageData = new VMData();

                    // Get the data necessary for reading the data from 2157
                    usIntervalsPerBlock = Table2153.NumberBlockIntervals;

                    byIntervalLength = Table2153.IntervalLength;
                    VoltageData.IntervalLength = TimeSpan.FromMinutes((double)byIntervalLength);

                    byNumPhases = Table2156.NumberOfPhases;
                    VoltageData.NumberOfPhases = byNumPhases;

                    usMaxBlocks = Table2155.NumberOfBlocks;

                    usUsedBlocks = Table2155.NumberValidBlocks;

                    usValidIntervals = Table2155.NumberValidIntervals;

                    usLastBlock = Table2155.LastBlock;

                    usDivisor = Table2155.Divisor;

                    usScalar = Table2155.Scalar;

                    VoltageData.NominalVoltages = new ushort[byNumPhases];

                    for (int iIndex = 0; iIndex < byNumPhases; iIndex++)
                    {
                        VoltageData.NominalVoltages[iIndex] = Table2156.NominalVoltage[iIndex];
                    }

                    VoltageData.VhLowPercentage = Table2154.VhLowThreshold;
                    VoltageData.VhHighPercentage = Table2154.VhHighThreshold;
                    VoltageData.RMSVoltageLowPercentage = Table2154.RMSVoltLowThreshold;
                    VoltageData.RMSVoltageHighPercentage = Table2154.RMSVoltHighThreshold;

                    // Determine the starting block (We are always assuming circular lists
                    if (usUsedBlocks == usMaxBlocks)
                    {
                        // The data has wrapped
                        usStartBlock = (ushort)((usLastBlock + 1) % usMaxBlocks);
                    }
                    else
                    {
                        // The data has not wrapped so start at 0
                        usStartBlock = 0;
                    }

                    // Make sure the number of steps is not negative
                    int iNumberOfSteps = (usUsedBlocks > 0) ? usUsedBlocks - 1 : 0;

                    OnShowProgress(new ShowProgressEventArgs(1, iNumberOfSteps * usIntervalsPerBlock + usValidIntervals,
                        "", "Retrieving Voltage Monitoring Data..."));

                    // Get the data
                    for (ushort usBlockIndex = 0; usBlockIndex < usUsedBlocks; usBlockIndex++)
                    {
                        DateTime dtBlockEndTime;
                        ushort usActualBlockIndex = (ushort)((usStartBlock + usBlockIndex) % usMaxBlocks);
                        ushort usNumIntervals = 0;
                        VMBlockDataRecord VMBlock = null;

                        if (usActualBlockIndex != usLastBlock)
                        {
                            // Always usIntervalsPerBlock intervals in these blocks
                            usNumIntervals = usIntervalsPerBlock;
                        }
                        else
                        {
                            usNumIntervals = usValidIntervals;
                        }

                        PSEMResponse ReadBlockResult = Table2157.ReadBlock(usActualBlockIndex, usNumIntervals, out VMBlock);

                        if (PSEMResponse.Ok == ReadBlockResult)
                        {
                            dtBlockEndTime = VMBlock.BlockEndTime;

                            // Determine whether the last interval is in DST or not so we know to adjust it properly.
                            LastIntervalStatus = VMBlock.Intervals[usNumIntervals - 1].IntervalStatus;

                            // Get the interval data
                            for (ushort usIntervalIndex = 0; usIntervalIndex < usNumIntervals; usIntervalIndex++)
                            {
                                List<float> fVhDataList = new List<float>();
                                List<float> fVminDataList = new List<float>();
                                List<float> fVmaxDataList = new List<float>();
                                TimeSpan tsTimeDifference = TimeSpan.FromMinutes((double)((usNumIntervals - usIntervalIndex - 1) * byIntervalLength));
                                DateTime dtIntervalEndTime = dtBlockEndTime - tsTimeDifference;
                                VMStatusFlags IntervalStatus = VMBlock.Intervals[usIntervalIndex].IntervalStatus;

                                // Adjust the time if there is a difference in DST status
                                if ((IntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST
                                    && (LastIntervalStatus & VMStatusFlags.DST) != VMStatusFlags.DST)
                                {
                                    // We need to adjust forward an hour since the time we have has been
                                    // adjusted backwards for DST
                                    dtIntervalEndTime = dtIntervalEndTime.Add(new TimeSpan(1, 0, 0));
                                }
                                else if ((IntervalStatus & VMStatusFlags.DST) != VMStatusFlags.DST
                                    && (LastIntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST)
                                {
                                    // We need to adjust back an hour since the time we have has been
                                    // adjusted forward for DST
                                    dtIntervalEndTime = dtIntervalEndTime.Subtract(new TimeSpan(1, 0, 0));
                                }

                                // Get the values
                                for (byte byPhaseIndex = 0; byPhaseIndex < byNumPhases; byPhaseIndex++)
                                {
                                    ushort usValue;

                                    usValue = VMBlock.Intervals[usIntervalIndex].VhData[byPhaseIndex];
                                    fVhDataList.Add(((float)usValue * (float)usScalar) / (float)usDivisor);

                                    usValue = VMBlock.Intervals[usIntervalIndex].VminData[byPhaseIndex];
                                    fVminDataList.Add(((float)usValue * (float)usScalar) / (float)usDivisor);

                                    usValue = VMBlock.Intervals[usIntervalIndex].VmaxData[byPhaseIndex];
                                    fVmaxDataList.Add(((float)usValue * (float)usScalar) / (float)usDivisor);
                                }

                                // The first interval of the DST change is always marked opposite of what we think so
                                // we need to go back and adjust that one if the previous DST status does not match the
                                // current DST status. The first check will prevent adjustments across blocks.
                                if (usIntervalIndex - 1 >= 0 && (IntervalStatus & VMStatusFlags.DST)
                                   != (Intervals[Intervals.Count - 1].IntervalStatus & VMStatusFlags.DST))
                                {
                                    VMInterval PreviousInterval = Intervals[Intervals.Count - 1];

                                    if ((PreviousInterval.IntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST)
                                    {
                                        // Interval was in DST so subtract an hour
                                        Intervals[Intervals.Count - 1] = new VMInterval(PreviousInterval.IntervalStatus,
                                            PreviousInterval.VhData, PreviousInterval.VminData, PreviousInterval.VmaxData,
                                            PreviousInterval.IntervalEndTime.Subtract(new TimeSpan(1, 0, 0)));
                                    }
                                    else
                                    {
                                        // Interval was not in DST so add an hour
                                        Intervals[Intervals.Count - 1] = new VMInterval(PreviousInterval.IntervalStatus,
                                            PreviousInterval.VhData, PreviousInterval.VminData, PreviousInterval.VmaxData,
                                            PreviousInterval.IntervalEndTime.Add(new TimeSpan(1, 0, 0)));
                                    }
                                }

                                Intervals.Add(new VMInterval(IntervalStatus, fVhDataList, fVminDataList, fVmaxDataList, dtIntervalEndTime));
                                OnStepProgress(new ProgressEventArgs());
                            }
                        }
                        else
                        {
                            throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, ReadBlockResult,
                                    "Error reading the voltage monitoring data block #" + usActualBlockIndex));
                        }
                    }

                    VoltageData.Intervals = Intervals;
                }
            }
            //Add support for retrieving legacy voltage monitoring data if desired here...

            OnHideProgress(new EventArgs());

            return VoltageData;
        }

        /// <summary>
        /// Gets all of the Extended Load Profile data from the meter
        /// </summary>
        /// <returns>The Extended Load Profile data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/12/11 RCG 2.53.20		Created 
        public LoadProfileData GetExtendedLoadProfileData()
        {
            return GetProfileData(Table2409, Table2410, Table2411, Table2412);
        }

        /// <summary>
        /// Gets the Extended Load Profile data between the specified dates
        /// </summary>
        /// <param name="startDate">The start date of the Extended Load Profile Data</param>
        /// <param name="endDate">The end date of the Extended Load Profile Data</param>
        /// <returns>The Extended Load Profile Data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/12/11 RCG 2.53.20		Created 
        public LoadProfileData GetExtendedLoadProfileData(DateTime startDate, DateTime endDate)
        {
            return GetProfileData(startDate, endDate, Table2409, Table2410, Table2411, Table2412);
        }

        /// <summary>
        /// Gets the Voltage Monitoring data between the specified dates
        /// </summary>
        /// <param name="startDate">The start date of the Voltage Monitoring Data</param>
        /// <param name="endDate">The end date of the Voltage Monitoring Data</param>
        /// <returns>The Voltage Monitoring Data</returns>
        //  Revision History	
        //  MM/DD/YY who Version  Issue#   Description
        //  -------- --- -------  ------   ---------------------------------------
        //  04/27/15 PGH 4.50.109 SREQ7642 Created 
        public VMData GetVoltageMonitoringData(DateTime startDate, DateTime endDate)
        {
            VMData VoltageData = GetVoltageMonitoringData();

            if (VoltageData != null)
            {
                List<VMInterval> VoltageDataIntervals = VoltageData.Intervals.Where(o => o.IntervalEndTime >= startDate && o.IntervalEndTime <= endDate).ToList<VMInterval>();
                List<VMInterval> Intervals = new List<VMInterval>();
                foreach (VMInterval Interval in VoltageDataIntervals)
                {
                    Intervals.Add(Interval);
                }
                VoltageData.Intervals = Intervals;
            }

            return VoltageData;
        }

        /// <summary>
        /// Gets all of the Instrumentation Profile data from the meter
        /// </summary>
        /// <returns>The Instrumentation Profile data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/12/11 RCG 2.53.20		Created 
        public LoadProfileData GetInstrumentationProfileData()
        {
            return GetProfileData(Table2409, Table2410, Table2411, Table2413);
        }

        /// <summary>
        /// Gets the Instrumentation Profile data between the specified dates
        /// </summary>
        /// <param name="startDate">The start date of the Instrumentation Profile Data</param>
        /// <param name="endDate">The end date of the Instrumentation Profile Data</param>
        /// <returns>The Instrumentation Data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/12/11 RCG 2.53.20		Created 
        public LoadProfileData GetInstrumentationProfileData(DateTime startDate, DateTime endDate)
        {
            return GetProfileData(startDate, endDate, Table2409, Table2410, Table2411, Table2413);
        }

        /// <summary>
        /// Enables Full Fatal Error Checking for the specified duration
        /// </summary>
        /// <param name="minutes">The number of minutes to enable Full Fatal Error Checking</param>
        /// <returns>The result of the Procedure Call</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/12 RCG 2.53.41 N/A    Created
        public ProcedureResultCodes EnableFullFatalErrorChecking(uint minutes)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ParamterData = new byte[4];
            MemoryStream ParameterStream = new MemoryStream(ParamterData);
            PSEMBinaryWriter ParameterWriter = new PSEMBinaryWriter(ParameterStream);
            byte[] ResponseData;

            ParameterWriter.Write(minutes);

            ProcResult = ExecuteProcedure(Procedures.ENABLE_FATAL_ERROR_CHECKING,
                ParamterData, out ResponseData);

            return ProcResult;
        }

        /// <summary>
        /// Retrieves the LID value passed in. Assumes the value is a float (inst value)
        /// </summary>
        /// <param name="LID">Instantaneous LID to retrieve</param>
        /// <returns>Float</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/22/12 JKW 2.60.xx N/A    Created
        public float GetInstLIDValue(Device.LID LID)
        {
            object objValue = null;
            float returnValue = 0;

            if (m_lidRetriever.RetrieveLID(LID, out objValue) == PSEMResponse.Ok)
            {
                returnValue = (float)objValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Causes a new read of standard table 00
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/21/12 AF  2.70.19 202904 Need to re-read table 00 to get an update on
        //                              supported tables after calling mfg proc 159
        //
        public void RereadTable00()
        {
            Table00.Read();
        }

        /// <summary>
        /// Method Disables HAN Price Mode.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/11/12 DC  2.60.xx        Created
        // 05/07/12 jrf 2.60.20 TREQ5994 Added return of procedure result.
        //
        public ProcedureResultCodes HANDisablePricing()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcParam = new byte[0];  // No parameters for this procedure
            byte[] procResponse;

            ProcResult = ExecuteProcedure(Procedures.HAN_DISABLE_PRICING, ProcParam, out procResponse);

            return ProcResult;
        }

        /// <summary>
        /// Method performs a HAN Move Out - clears HAN data at meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/11/12 DC  2.60.xx        Created
        // 05/01/12 jrf 2.60.19 TREQ2893 Added return of procedure result and passing 
        //                             empty byte array instead of null for parameters
        //                             to prevent null reference exception.
        //
        public ProcedureResultCodes HANMoveOut()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcParam = new byte[0];  // No parameters for this procedure
            byte[] response;

            ProcResult = ExecuteProcedure(Procedures.HAN_MOVE_OUT, ProcParam, out response);

            return ProcResult;
        }

        /// <summary>
        /// Method sets SEALED bit in the NextBlockPriceScheduleTable.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/11/12 DC  2.60.xx        Created
        public ProcedureResultCodes HANCommitNextBlockPriceSchedule()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcParam = new byte[0];  // No parameters for this procedure
            byte[] response;

            ProcResult = ExecuteProcedure(Procedures.HAN_COMMIT_NEXT_BLOCK_PRICE_SCHEDULE, ProcParam, out response);

            return ProcResult;
        }

        /// <summary>
        /// Method to update the CurrentBlockPeriodConsumptionDelivered Attribute.  
        /// The parameter can either be added to the current attribute value or it can replace it entirely.
        /// </summary>
        /// <param name="updateMethod">0 = replace, 1 = add to existing value</param>
        /// <param name="currentBlockPeriodConsumptionDelivered">Numeric value to set (or add) to ConsumptionDelivered.</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/11/12 DC  2.60.xx        Created
        //
        public ProcedureResultCodes HANUpdateRIBConsumption(UInt48 currentBlockPeriodConsumptionDelivered, RIBUpdateMethod updateMethod)
        {
            byte[] procParameters = new byte[sizeof(Byte) + UInt48.SizeOf];
            byte[] procResponse;
            ProcedureResultCodes ProcResult;

            PSEMBinaryWriter psemWriter = new PSEMBinaryWriter(new MemoryStream(procParameters));
            psemWriter.WriteUInt48(currentBlockPeriodConsumptionDelivered);
            psemWriter.Write((byte)updateMethod);

            ProcResult = ExecuteProcedure(Procedures.HAN_UPDATE_CURRENT_BLOCK_PERIOD_CONSUMPTION_DELIVERED, procParameters, out procResponse);

            return ProcResult;
        }
        
        /// <summary>
        /// Checks standard table 00 to see if the specified table is supports write access
        /// </summary>
        /// <param name="usTableId">identifier of the table we want to know about</param>
        /// <returns>true if the table is listed in table 00; false, otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/18/13 MAH 2.70.69        Created
        //
        public bool IsTableWriteable(ushort usTableId)
        {
            bool tableWriteSupported = false;

            if (Table00.IsTableWriteable(usTableId))
            {
                tableWriteSupported = true;
            }

            return tableWriteSupported;
        }

#if (!WindowsCE)
        /// <summary>
        /// Validates the encrypted HAN security keys.
        /// </summary>
        /// <param name="Key">The encrypted Network Key</param>
        /// <param name="keyID">KeyID</param>
        /// <param name="blnKeyEncrypted">Whether or not the network key is encrypted.</param>
        /// <returns>result of the Procedure</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/17/08 RCG 1.50.37			Created
        // 07/28/09 AF  2.20.18 137870  Use the Mfg procedure for validating keys
        // 09/24/09 MMD 2.30.03         Changed the method to return the result code
		// 11/05/13 jrf 3.00.27 WR 426660 Handling case if decryption or hashing throws an 
        //                              exception. Adding new parameter to determine if key is encrypted.
        public ProcedureResultCodes ValidateHANSecurityKeys(string Key, HANKeys keyID, bool blnKeyEncrypted = true)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] hashCode = null;
            bool blnInvalidParameter = false;
            byte[] abytHANKey = null;
            int iDiscarded;

            try
            {
                if (true == blnKeyEncrypted)
                {
                    //Decrypt key
                    abytHANKey = DecryptHANKey(Key);
                }
                else
                {
                    //Convert key to byte array
                    abytHANKey = HexEncoding.GetBytes(Key, out iDiscarded);
                }                

                switch (keyID)
                {
                    case HANKeys.NetworkKey:
                        {
                            HANKeyRcd NetworkKey = new HANKeyRcd();
                            NetworkKey.KeyType = HANKeyRcd.HANKeyType.NetworkKey;
                            NetworkKey.HANKey = abytHANKey;
                            hashCode = CreateHash(NetworkKey.HANKey);
                        }
                        break;
                    case HANKeys.LinkKey:
                        {
                            HANKeyRcd LinkKey = new HANKeyRcd();
                            LinkKey.KeyType = HANKeyRcd.HANKeyType.GlobalLinkKey;
                            LinkKey.HANKey = abytHANKey;
                            hashCode = CreateHash(LinkKey.HANKey);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                blnInvalidParameter = true;
            }

            if (false == blnInvalidParameter)
            {
                ProcResult = ValidateSecurityKey(SecurityType.HAN_KEYS, (SecurityKeyID)keyID, hashCode);
            }

            return ProcResult;
        }

        /// <summary>
        /// Validates the DES keys in the meter 
        /// </summary>
        /// <param name="ProgName">Program Name</param>
        /// <param name="Wait">Whether or not to send a wait to the device before calling 
        /// method to validate DES keys.</param>
        /// <returns>list of bool items</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ------------------------------------------------------------
        //  11/20/09 MMD          QC Tool    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  10/11/16 jrf 4.60.11 722267 Adding wait in case call to validate DES keys hangs while 
        //                              attempting to access config file.
        public virtual bool ValidateDESKeys(string ProgName, bool Wait = false)
        {
            bool bKeysValid = true;
            for (int iKey = 0; iKey < 4; iKey++)
            {
                //This was added to attempt to prevent factory QC Tool issues with meter timing out.
                if (true == Wait)
                {
                    SendWait();
                }

                // Validate the key
                if (ValidateDESKeys(ProgName, (CENTRON_AMI.DESKeys)((byte)(iKey + 1))) != ProcedureResultCodes.COMPLETED)
                {
                    bKeysValid = false;
                    break;
                }
            }
            return bKeysValid;
        }

        /// <summary>
        /// Validates the DES Keys.
        /// </summary>
        /// <param name="ProgName">Name of the program</param>
        /// <param name="KeyID">Key ID: Key1, Key2...</param>
        /// <returns>Procedure Result code</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/21/09 MMD  2.30.01        Use the Mfg procedure for validating passwords
        // 04/19/10 AF   2.40.39        Made virtual for M2 Gateway override
        //
        public virtual ProcedureResultCodes ValidateDESKeys(string ProgName, DESKeys KeyID)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;

            //Used this condition to validate DES keys only if the Security Provider & Exception Security Model has C1222 Standard
            if (RequiresDESKeysValidation(ProgName, (SecurityKeyID)KeyID))
            {
                byte[] byData = CENTRON_AMI.GetSecurityCode(ProgName, SecurityType.C1222_KEYS, (SecurityKeyID)KeyID);

                byte[] hashedCode = CreateHash(byData);

                ProcResult = ValidateSecurityKey(SecurityType.C1222_KEYS, (SecurityKeyID)KeyID, hashedCode);
            }
            return ProcResult;
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
        //  01/27/09 RCG 2.10.02        Created
        //  10/02/09 jrf 2.30.05        Modified to only use new validation procedure for SP5 or greater FW.
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  10/11/16 jrf 4.60.11 722267 Adding wait in case call to validate enhanced security keys hangs while 
        //                              attempting to access key file.
        public virtual bool ValidateEnhancedSecurityKeys(string strEnhancedSecurityKeyFile, bool Wait = false)
        {
            bool bAreKeysValid = true;

            //Only use the new procedure in SP5 or greater FW.
            if (VersionChecker.CompareTo(FWRevision, VERSION_2_SP5) >= 0)
            {
                // Pull the keys out of Security File and validate them
                for (int iKey = 0; iKey < 6; iKey++)
                {
                    //This was added to attempt to prevent factory QC Tool issues with meter timing out.
                    if (true == Wait)
                    {
                        SendWait();
                    }

                    // Validate the key
                    if (ValidateEnhancedSecurityKey(strEnhancedSecurityKeyFile, (CENTRON_AMI.EnhancedKeys)((byte)(iKey + 1))) != ProcedureResultCodes.COMPLETED)
                    {
                        bAreKeysValid = false;
                        break;
                    }
                }
            }
            else
            {
                //Pre SP5 FW still must use the old way of doing things.
                bAreKeysValid = Table2127EnhancedSecurityKeys.ValidateKeys(strEnhancedSecurityKeyFile);
            }

            return bAreKeysValid;
        }

        /// <summary>
        /// Validates the Enhanced Security keys in the meter to those in the specified file.
        /// </summary>
        /// <param name="strEnhancedSecurityKeyFile">The file that contains the keys to validate against.</param>
        /// <param name="keyID">keyID</param>
        /// <returns>the result of the Validate Procedure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ------------------------------------------------------------
        //  09/24/09 MMD 2.30.03        Modified to use the new Validate procedure created for SP5

        public ProcedureResultCodes ValidateEnhancedSecurityKey(string strEnhancedSecurityKeyFile, EnhancedKeys keyID)
        {

            DKUSFile KeyFile = new DKUSFile(strEnhancedSecurityKeyFile);
            int iIndex = Convert.ToInt32((byte)(keyID)) - 1;
            //Hash the key
            byte[] hashedCode = CreateHash(KeyFile.Keys[iIndex]);
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            // Validate the key
            ProcResult = ValidateSecurityKey(SecurityType.ENHANCED_SECURITY, (SecurityKeyID)keyID, hashedCode);
            return ProcResult;
        }

#endif

        /// <summary>
        /// Disables or enables the optical port on the meter.
        /// </summary>
        /// <param name="disable">Whether the port should be disabled (true) or enabled (false)</param>
        /// <returns>The result of the procedure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ------------------------------------------------------------
        //  10/19/12 RCG 2.70.31        Created for Ameren demo        
        public ProcedureResultCodes EnableDisableOpticalPort(bool disable)
        {
            byte[] Parameters = new byte[1];
            byte[] Response;
            ProcedureResultCodes ProcResult;

            if (disable)
            {
                Parameters[0] = 1;
            }
            else
            {
                Parameters[0] = 0;
            }

            ProcResult = ExecuteProcedure(Procedures.ENABLE_DISABLE_OPTICAL_PORT, Parameters, out Response);

            return ProcResult;
        }

        /// <summary>
        /// Causes the meter to exit factory test mode
        /// </summary>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/01/12 AF  2.70.35 239637 Created
        //
        public ProcedureResultCodes ExitFactoryTestMode()
        {
            ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[0];

            Result = ExecuteProcedure(Procedures.EXIT_FACTORY_TEST_MODE, ProcParam, out ProcResponse);

            return Result;
        }

        /// <summary>
        /// Refresh the metrology statistics data by causing a new read to occur
        /// </summary>
        /// <returns>void</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public void RefreshMetrologyStatistics()
        {
            if (Table2112 != null)
            {
                Table2112.Refresh();
            }
        }

        /// <summary>
        /// Reads the stack depth using mfg procedire 170
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/12 mah 2.70.23        Created
        //  01/17/13 jkw 2.70.58        Made public so that it can be accessed from the endpoint server
        //
        public ProcedureResultCodes ReadStackDepth(ushort usStackID, out uint unStackSize, out uint unStackDepth, out String strTaskName)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;

            MemoryStream ProcParam = new MemoryStream(2);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);

            ParamWriter.Write(usStackID);

            ProcResult = ExecuteProcedure(Procedures.READ_STACK_DEPTH, ProcParam.ToArray(), out ProcResponse);

            if (ProcResult == ProcedureResultCodes.COMPLETED)
            {
                MemoryStream ProcResponseStream = new MemoryStream(ProcResponse);
                PSEMBinaryReader ResponseReader = new PSEMBinaryReader(ProcResponseStream);

                unStackSize = ResponseReader.ReadUInt32();
                unStackDepth = ResponseReader.ReadUInt32();
                strTaskName = ResponseReader.ReadString(14);

                strTaskName = strTaskName.TrimEnd(' ');
            }
            else
            {
                unStackSize = 0;
                unStackDepth = 0;
                strTaskName = "";
            }

            return ProcResult;
        }

        /// <summary>
        /// Reads 4 bytes of the meter's memory starting at the specified address.
        /// </summary>
        /// <param name="startAddress">The location in memory to start</param>
        /// <param name="data">The raw memory.</param>
        /// <returns>The result of the procedure.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/14/10 RCG 2.40.06 N/A    Created
        public ProcedureResultCodes ReadMemory(uint startAddress, out byte[] data)
        {
            MemoryStream ParameterStream = new MemoryStream();
            PSEMBinaryWriter BinaryWriter = new PSEMBinaryWriter(ParameterStream);
            byte[] ProcResponse;
            byte[] byParameter;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            BinaryWriter.Write(startAddress);
            byParameter = ParameterStream.ToArray();

            ProcResult = ExecuteProcedure(Procedures.MEMORY_READER, byParameter, out ProcResponse);

            if (ProcResult == ProcedureResultCodes.COMPLETED)
            {
                data = ProcResponse;
            }
            else
            {
                data = null;
            }

            return ProcResult;
        }

        /// <summary>
        /// Calls the open config procedure
        /// </summary>
        /// <returns>The result of the procedure</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/29/09 RCG 2.20.03 N/A    Created
        public ProcedureResultCodes OpenConfig()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[0];
            ProcResult = ExecuteProcedure(Procedures.OPEN_CONFIG_FILE,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Calls the close config procedure.
        /// </summary>
        /// <param name="options">The options flags to be used for calling close config</param>
        /// <param name="errors">The error parameter returned by the procedure.</param>
        /// <returns>The result of the procedure call.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/29/09 RCG 2.20.03 N/A    Created
        public ProcedureResultCodes CloseConfig(CloseConfigOptions options, out CloseConfigErrors errors)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ParamStream;
            MemoryStream ResponseStream;
            BinaryWriter BinWriter;
            BinaryReader BinReader;
            byte[] ProcParam;
            byte[] ProcResponse;

            // Set up the calling parameters.
            ProcParam = new byte[4];

            ParamStream = new MemoryStream(ProcParam);
            BinWriter = new BinaryWriter(ParamStream);

            BinWriter.Write((uint)options);

            // Execute the procedure
            ProcResult = ExecuteProcedure(Procedures.CLOSE_CONFIG_FILE,
                ProcParam, out ProcResponse);

            // This procedure has a Uint32 output parameter so we want to return that as well.
            if (ProcResponse.Length == 4)
            {
                ResponseStream = new MemoryStream(ProcResponse);
                BinReader = new BinaryReader(ResponseStream);

                errors = (CloseConfigErrors)BinReader.ReadUInt32();
            }
            else
            {
                errors = CloseConfigErrors.None;
            }

            return ProcResult;
        }

        /// <summary>
        /// This method updates the number of unread entries in the Canadian 
        /// Event Log.  This method does not clear the log, but events that
        /// are marked as having been read can be overwritten by the meter as 
        /// needed.  Use max value to update all entries.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/22/06 mcm 7.35.00  N/A    Created
        //
        public ItronDeviceResult UpdateCanadianEventLog(ushort EntriesToMarkAsRead)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[3];
            ProcParam[0] = (byte)UpdateLastReadEntryListIDs.CANADIAN_EVENT_LOG;
            ProcParam[1] = (byte)EntriesToMarkAsRead;
            ProcParam[2] = (byte)(EntriesToMarkAsRead / 0x100);
            ProcResult = ExecuteProcedure(Procedures.UPDATE_LAST_READ_ENTRY,
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

        } // End ResetDemand()

        /// <summary>
        /// Reads the specified table from the meter.
        /// </summary>
        /// <param name="usTableID">The table ID for the table to read.</param>
        /// <param name="MeterTables">The tables object to read the table into.</param>
        /// <returns>PSEMResponse code.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/21/13 DLG 3.50.07 No WR    Created by taking CENTRON specific parts from ANSIDevice 
        //                                and moving here.
        //  05/08/14 AF  3.50.91 WR503773 Refactored ReadTable2243() and gave it a max offset read parameter
        //  07/28/14 jrf 4.00.49 523361   Added special case for reading mfg. table 2242 with partial reads.
        //  07/21/15 jrf 4.20.18 598314   Added cases for ERT statistics and consumption data tables. They were 
        //                                failing with (0x04 - ONP) for full reads so we need to perform an offset 
        //                                read to get the data.
        //  07/23/15 jrf 4.20.18 598314   The previous fix was incorrect. Instead of adding special cases for ERT statistics 
        //                                and consumption data tables here, this method should be overridden in the ITRK device.
        //                                ITRJ already does this.  Also removing code that is duplicated from base class and 
        //                                calling base.ReadTable(...) in default case to handle that case.
        public override PSEMResponse ReadTable(ushort usTableID, ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            int iReadAttempt = 0;
            bool bRetry = true;

            while (bRetry)
            {
                switch (usTableID)
                {
                    case 2152:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable2152(ref MeterTables);
                            }

                            break;
                        }
                    case 2157:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable2157(ref MeterTables);
                            }

                                break;
                        }
                    case 2162:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable2162(ref MeterTables);
                            }

                            break;
                        }
                    case 2164:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable2164(ref MeterTables);
                            }

                            break;
                        }
                    case 2242:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                uint MaxOffsetReadSize = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                                if (MaxOffsetReadSize > ushort.MaxValue)
                                {
                                    MaxOffsetReadSize = ushort.MaxValue;
                                }

                                PSEMResult = ReadTable2242(ref MeterTables, MaxOffsetReadSize);
                            }

                            break;
                        }
                    case 2243:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                uint MaxOffsetReadSize = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                                if (MaxOffsetReadSize > ushort.MaxValue)
                                {
                                    MaxOffsetReadSize = ushort.MaxValue;
                                }

                                PSEMResult = ReadTable2243(ref MeterTables, MaxOffsetReadSize);
                            }

                            break;
                        }
                    case 2412:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable2412(ref MeterTables);
                            }
                            break;
                        }
                    case 2413:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable2413(ref MeterTables);
                            }

                            break;
                        }
                    default:
                        {
                            PSEMResult = base.ReadTable(usTableID, ref MeterTables);
                            iReadAttempt = 3; //Skipping retries since they will be handled in base class

                            break;
                        }
                }

                iReadAttempt++;

                if (iReadAttempt < 3 && (PSEMResult == PSEMResponse.Bsy || PSEMResult == PSEMResponse.Dnr))
                {
                    bRetry = true;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    bRetry = false;
                }
            }

            return PSEMResult;
        }

        /// <summary>
        /// Reconfigures TOU in the connected meter.
        /// </summary>
        /// <param name="TOUFileName">The filename including path for the 
        /// configuration containing the TOU schedule.</param>
        /// <param name="iSeasonIndex">The number of seasons from the current
        /// season to write.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/21/13 jrf 3.50.07 TQ9523 Created
        //  11/27/13 jrf 3.50.10 TQ9523 Refactored Bridge TOU reconfiguration to COpenWayITRDBridge/COpenWayITRFBridge

        public virtual TOUReconfigResult ReconfigureTOU(string TOUFileName, int iSeasonIndex)
        {
            return ConvertWritePendingTOUResult(WritePendingTOU(TOUFileName, iSeasonIndex));
        }

        /// <summary>
        /// This method enables the HAN events passed in the configuration.  
        /// ****ONLY USED FOR TESTING****
        /// </summary>
        /// <param name="lstUpstreamEvents">The upstream events that should be configured</param>
        /// <param name="lstDownstreamEvents">The downstream events that should be configured
        /// </param>
        /// <returns>Whether or not the HAN events were successfully configured</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/28/11 jrf 2.50.00		Created 
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to Centron_AMI.
        //  11/28/16 PGH 4.70.38 719585 Events are enabled for logging through table 2260
        //
        public bool ConfigureHANEvents(List<UpstreamHANLogEvent> lstUpstreamEvents,
                                       List<DownstreamHANLogEvent> lstDownstreamEvents)
        {
            byte[] HANEventsConfig = Table2260HANEvents.HANEventsConfigured;
            List<ushort> lstEventIDs = new List<ushort>();
            int iByteIndex = 0;
            int iBitIndex = 0;
            byte bytBitMask = 0;
            bool blnConfigurationSuccess = true;

            //Let's reset all the events first
            for (int i = 0; i < HANEventsConfig.Length; i++)
            {
                HANEventsConfig[i] = 0;
            }

            //Collect all the event IDs
            foreach (UpstreamHANLogEvent LogEvent in lstUpstreamEvents)
            {
                lstEventIDs.Add((ushort)(LogEvent.EventID - 2304));
            }

            foreach (DownstreamHANLogEvent LogEvent in lstDownstreamEvents)
            {
                lstEventIDs.Add((ushort)(LogEvent.EventID - 2304));
            }

            //Write the configured events to the array
            foreach (ushort usEventID in lstEventIDs)
            {
                //Determine byte to look at in the array
                //----->byte to look at = bit position of the event / bits per byte
                iByteIndex = usEventID / 8;

                //Determine bit position in that byte
                //----->bit position within the byte = bit position of the event % bits per byte
                iBitIndex = usEventID % 8;

                //Create a mask to set that bit
                bytBitMask = (byte)(1 << iBitIndex);

                //Set the bit
                HANEventsConfig[iByteIndex] = (byte)(HANEventsConfig[iByteIndex] | bytBitMask);
            }

            try
            {
                //Write the configuration to the meter
                Table2260HANEvents.HANEventsConfigured = HANEventsConfig;
            }
            catch
            {
                blnConfigurationSuccess = false;
            }

            return blnConfigurationSuccess;
        }
        
        /// <summary>
        /// Performs a soft EPF
        /// </summary>
        /// <returns>
        /// An ItronDeviceResult representing the result of the reset
        /// operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/13/14 AF  3.70.01 WR 529116 Created
        //
        public ItronDeviceResult SoftEPF()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            MemoryStream ProcParam = new MemoryStream(9);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            PSEMResponse Response = PSEMResponse.Err;

            ParamWriter.Write(RESET_RF_FUNC_CODE);
            ParamWriter.Write(SOFT_EPF_PARAM_1);
            ParamWriter.Write(SOFT_EPF_PARAM_2);

            Table07.Procedure = (ushort)Procedures.RESET_RF_LAN;
            Table07.ParameterData = ProcParam.ToArray();

            Response = Table07.Write();

            switch(Response)
            {
                case PSEMResponse.Ok:
                {
                    Result = ItronDeviceResult.SUCCESS;
                    break;
                }
                case PSEMResponse.Isc:
                {
                    Result = ItronDeviceResult.SECURITY_ERROR;
                    break;
                }
                case PSEMResponse.Sns:
                case PSEMResponse.Onp:
                {
                    Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
                    break;
                }
                default:
                {
                    Result = ItronDeviceResult.ERROR;
                    break;
                }
            }

            return Result;
        }

        /// <summary>
        /// Gets the list of Energies required by the program.
        /// </summary>
        /// <param name="programFile">The path to the program file</param>
        /// <returns>The list of required energies</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/22/11 RCG 2.50.12 N/A    Created
        // 10/15/14 jrf 4.00.73 539220 Made method public for use in QC Tool.
        public virtual List<BaseEnergies> GetRequiredEnergiesFromProgram(string programFile)
        {
            List<BaseEnergies> RequiredEnergies = new List<BaseEnergies>();
            XmlReader Reader;
            XmlReaderSettings ReaderSettings = new XmlReaderSettings();

            // Create the CentronTables object to read the file
            ReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
            ReaderSettings.IgnoreWhitespace = true;
            ReaderSettings.IgnoreComments = true;
            ReaderSettings.CheckCharacters = false;

            Reader = XmlReader.Create(programFile, ReaderSettings);

            CentronTables ProgramTables = new CentronTables();

            ProgramTables.LoadEDLFile(Reader);

            // Get the energy configuration
            for (int iIndex = 0; iIndex < NumberOfSupportedEnergies; iIndex++)
            {
                object objValue = null;
                int[] Indicies = new int[] { iIndex };

                if (ProgramTables.IsCached((long)CentronTblEnum.MFGTBL0_ENERGY_LID, Indicies))
                {
                    ProgramTables.GetValue(CentronTblEnum.MFGTBL0_ENERGY_LID, Indicies, out objValue);

                    // We need to add the Secondary Energy Base value to the byte returned to get the 
                    // actual LID value
                    LID EnergyLid = CreateLID(SEC_ENERGY_LID_BASE + (byte)objValue);

                    switch (EnergyLid.lidQuantity)
                    {
                        case DefinedLIDs.WhichOneEnergyDemand.WH_DELIVERED:
                        case DefinedLIDs.WhichOneEnergyDemand.WH_RECEIVED:
                        case DefinedLIDs.WhichOneEnergyDemand.WH_UNI:
                        case DefinedLIDs.WhichOneEnergyDemand.WH_NET:
                            {
                                // Do nothing because the base always supports Wh and therefore does not need to be configured
                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH:
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.VAhArithmetic) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.VAhArithmetic);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT:
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.VAhVectorial) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.VAhVectorial);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_LAG:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.VAhLag) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.VAhLag);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VARH_DEL:
                        case DefinedLIDs.WhichOneEnergyDemand.VARH_REC:
                        case DefinedLIDs.WhichOneEnergyDemand.VARH_NET:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.VarhVectorial) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.VarhVectorial);
                                }

                                break;
                            }
                    }
                }
            }

            ProgramTables = null;
            Reader.Close();

            return RequiredEnergies;
        }

       
        /// <summary>
        /// Gets the selected secondary energy from the program.
        /// </summary>
        /// <param name="programFile">The path to the program file</param>
        /// <returns>The list of required energies</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/15 jrf 4.22.05 N/A    Created
        public BaseEnergies GetSelectedSecondaryQuantityFromProgram(string programFile)
        {
            BaseEnergies SecondaryQuantity = BaseEnergies.Unknown;
            try
            {
                XmlReader Reader;
                XmlReaderSettings ReaderSettings = new XmlReaderSettings();

                // Create the CentronTables object to read the file
                ReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
                ReaderSettings.IgnoreWhitespace = true;
                ReaderSettings.IgnoreComments = true;
                ReaderSettings.CheckCharacters = false;

                Reader = XmlReader.Create(programFile, ReaderSettings);

                CentronTables ProgramTables = new CentronTables();

                ProgramTables.LoadEDLFile(Reader);

                // Get the selected secondary energy
                object objValue = null;
                if (ProgramTables != null && ProgramTables.IsCached((long)CentronTblEnum.MfgTbl2044VAVARSelection, null))
                {
                    ProgramTables.GetValue(CentronTblEnum.MfgTbl2044VAVARSelection, null, out objValue);
                }

                if (objValue != null && VA == (short)objValue)
                {
                    objValue = null;
                    if (ProgramTables != null && ProgramTables.IsCached((long)CentronTblEnum.MfgTbl2044UserInterfaceVoltAmperesSelection, null))
                    {
                        ProgramTables.GetValue(CentronTblEnum.MfgTbl2044UserInterfaceVoltAmperesSelection, null, out objValue);
                    }

                    if (objValue != null && ARITHMETIC == (byte)objValue)
                    {
                        SecondaryQuantity = BaseEnergies.VAhArithmetic;
                    }
                    else if (objValue != null && VECTORIAL == (byte)objValue)
                    {
                        SecondaryQuantity = BaseEnergies.VAhVectorial;
                    }

                }
                else if (objValue != null && VAR == (short)objValue)
                {
                    SecondaryQuantity = BaseEnergies.VarhVectorial;
                }

                ProgramTables = null;
                Reader.Close();
            }
            catch { }


            return SecondaryQuantity;
        }

        /// <summary>
        /// Reads the load profile memory size from a program
        /// </summary>
        /// <param name="programFile">the program to read</param>
        /// <returns>the size of the load profile memory</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  11/03/15 AF  4.22.01  WR 626806  Created
        //
        public byte GetLPMemorySizeFromProgram(string programFile)
        {
            byte MemorySize = 0;
            object objValue = null;

            XmlReader Reader;
            XmlReaderSettings ReaderSettings = new XmlReaderSettings();

            // Create the CentronTables object to read the file
            ReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
            ReaderSettings.IgnoreWhitespace = true;
            ReaderSettings.IgnoreComments = true;
            ReaderSettings.CheckCharacters = false;

            Reader = XmlReader.Create(programFile, ReaderSettings);

            CentronTables ProgramTables = new CentronTables();

            ProgramTables.LoadEDLFile(Reader);

            if (ProgramTables.IsCached((long)CentronTblEnum.MFGTBL0_LP_MEMORY_SIZE, null))
            {
                ProgramTables.GetValue(CentronTblEnum.MFGTBL0_LP_MEMORY_SIZE, null, out objValue);

                if (objValue != null)
                {
                    MemorySize = (byte)objValue;
                }
            }

            return MemorySize;
        }

        /// <summary>
        /// Changes the Load Profile memory size to the parameter passed in. The CloseConfig()
        /// will then reset Load Profile.
        /// </summary>
        /// <param name="MemorySize">The memory size to change to</param>
        /// <param name="Offset">The offset into mfg table 2048 for the LP memory size field</param>
        /// <returns>The result of the open config and/or close config procedures</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  10/26/15 AF  4.22.00  WR 626806  Created
        //
        public ProcedureResultCodes ResetLoadProfile(byte MemorySize, uint Offset)
        {
            CloseConfigErrors Errors = CloseConfigErrors.None;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;
            PSEMResponse Response = PSEMResponse.Ok;

            byte[] abyMemSize = new byte[1];
            abyMemSize[0] = MemorySize;

            // Call open config so that we can write to it.
            ProcResult = OpenConfig();

            if (ProcResult == ProcedureResultCodes.COMPLETED)
            {
                Response = m_PSEM.OffsetWrite((ushort)2048, (int)Offset, abyMemSize);

                if (Response == PSEMResponse.Ok)
                {
                    ProcResult = CloseConfig(CloseConfigOptions.LoadProfile, out Errors);
                }
                else
                {
                    ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
                }
            }

            if (Errors == CloseConfigErrors.LoadProfileError)
            {
                // Not sure which error code to use but definitely did not succeed
                ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            }

            return ProcResult;
        }

        /// <summary>
        /// Updates the temperature offset in factory data without triggering a cold start
        /// </summary>
        /// <param name="newTemperatureOffset">the offset value to set</param>
        /// <returns>The result of the procedure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/03/16 AF  4.50.234 TC 62919   Created
        //
        public ProcedureResultCodes UpdateTemperatureOffset(Int16 newTemperatureOffset)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;
            byte[] ProcParam;
            byte[] ProcResponse;
            ProcParam = new byte[2];
            ProcParam = BitConverter.GetBytes(newTemperatureOffset);

            ProcResult = ExecuteProcedure(Procedures.UPDATE_TEMPERATURE_OFFSET, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Gets config version from Configuration Table
        /// </summary>
        /// <returns></returns>
        public ushort GetConfigVersion()
        {
            CTable2048Header configinfo = new CTable2048Header(m_PSEM);
            return configinfo.ConfigurationVersion;
        }

        /// <summary>
        /// Read Exteneded Self Read Configuration out of meter
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#    Description
        //  -------- --- ------- ------    -------------------------------------------
        //  08/02/16 MP  4.70.11 WR701234  Created
        //
        public List<SR2InstQuantity> GetExtendedSelfReadConfiguration()
        {
            MFGTable2265ExtendedSelfReadConfig ConfigurationTable = new MFGTable2265ExtendedSelfReadConfig(m_PSEM);
            ConfigurationTable.Read();
            return ConfigurationTable.ESRConfiguration;
        }

        /// <summary>
        /// The method converts a UTC time to a local time based on the meter's specific DST and timezone settings.
        /// </summary>
        /// <param name="UTCTime">The time to convert in UTC.</param>
        /// <returns>The UTC time converted to meter local time.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/17 jrf 4.70.16 WI 710078,710080,714616,714619,714619 Created
        public DateTime ConvertUTCToLocalMeterTime(DateTime UTCTime)
        {
            DateTime LocalTime = UTCTime;
            bool InDST = false;

            if (true == IsDSTApplied)
            {
                List<DateTime[]> DSTDatePair = DSTDates.Where(d => d[0].Year == UTCTime.Year).ToList();

                if (null != DSTDatePair && DSTDatePair.Count > 0)
                {
                    if (UTCTime > DSTDatePair[0][0] && UTCTime < DSTDatePair[0][1])
                    {
                        InDST = true;
                    }
                }

                if (true == InDST)
                {
                    LocalTime = LocalTime + DSTAdjustAmount;
                }
            }

            if (true == IsTimeZoneApplied)
            {
                LocalTime = LocalTime + TimeZoneOffset;
            }

            return LocalTime;
        }

        #endregion

        #region Public Properties

        #region MeterKey Settings

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
        /// Gets the Pole Top Cell Relay meter key bit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/31/09 AF  2.20.19 138931 Created to be able to detect pole top cell
        //                              relay devices
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool MeterKey_PoleTopCellRelaySupported
        {
            get
            {
                return MeterKeyTable.PoleTopCellRelaySupported;
            }
        }

        /// <summary>
        /// Returns whether or not the meter uses the SR1.0 Device Class.
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
        public virtual bool MeterKey_UseSR1DeviceClass
        {
            get
            {
                return MeterKeyTable.UseSR1DeviceClass;
            }
        }

        /// <summary>
        /// Returns whether or not the meter enables ZigBee Debugging.
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
        public virtual bool MeterKey_ZigBeeDebug
        {
            get
            {
                return MeterKeyTable.ZigBeeDebug;
            }
        }

        /// <summary>
        /// Returns whether or not the meter is a transparent device.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/14/09 jrf 2.21.01 n/a    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool MeterKey_TransparentDeviceSupported
        {
            get
            {
                return MeterKeyTable.TransparentDevice;
            }
        }

        /// <summary>
        /// Returns whether or not the meter will Disable Core Dump.
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
        public virtual bool MeterKey_DisableCoreDump
        {
            get
            {
                return MeterKeyTable.DisableCoreDump;
            }
        }

        /// <summary>
        /// Returns whether or not the meter will Disable Core Dump on Total Stack Use Limit.
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
        public virtual bool MeterKey_DisableCoreDumpOnTotalStackUseLimit
        {
            get
            {
                return MeterKeyTable.DisableCoreDumpOnTotalStackUseLimit;
            }
        }

        /// <summary>
        /// Returns whether or not the meter is an Advanced Poly.
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
        public virtual bool MeterKey_AdvancedPolySupported
        {
            get
            {
                return MeterKeyTable.AdvancedPolySupported;
            }
        }

        /// <summary>
        /// Returns the external MCP type used in the meter.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/18/14 jrf 4.00.56 534458	Created
        //  
        public MCPType MeterKey_MCPTypeUsed
        {
            get
            {
                return MeterKeyTable.MCPTypeUsed;
            }
        }

        /// <summary>
        /// Property that determines if the Var Q2 and Var Q3 Energy 1 MeterKey bits are enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Issue# Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 09/01/16 jrf 4.70.16 WI 708332 Created
        public bool VarhQ2VarhQ3MeterKeyBitsEnabled
        {
            get
            {
                bool Result = false;

                if (null != Table2061Energy1MeterKey)
                {
                    if ((Table2061Energy1MeterKey.Energy1MeterKey & VARH_Q2_Q3_ENERGY_MASK) == VARH_Q2_Q3_ENERGY_MASK)
                    {
                        Result = true;
                    }
                }

                return Result;
            }
        }

        #endregion

        #region Device Time Information

        /// <summary>
        /// The current HAN Time.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public DateTime CurrentHANDeviceTime
        {
            get
            {
                DateTime dtHANTime = DateTime.MinValue;

                if (null != Table2440)
                {
                    dtHANTime = Table2440.CurrentHANTime;
                }

                return dtHANTime;
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
        /// Gets the display module's time.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 RCG 2.44.04 N/A    Created

        public DateTime DisplayTime
        {
            get
            {
                return Table2198.DisplayTime;
            }
        }

        /// <summary>
        /// The local time from MFG table 313 (2361).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public DateTime LocalTimeNoSecurityNeeded
        {
            get
            {
                return Table2361.LocalTime;
            }
        }

        /// <summary>
        /// The GMT time from MFG table 313 (2361).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public DateTime GMTTimeNoSecurityNeeded
        {
            get
            {
                return Table2361.GMTTime;
            }
        }

        /// <summary>
        /// Whether the DST Mode is on or off using MFG table 313 (2361).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public bool? UseDSTModeNoSecurityNeeded
        {
            get
            {
                return Table2361.UseDSTMode;
            }
        }

        /// <summary>
        /// The Time Zone Offset from MFG table 313 (2361).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public Int16 TimeZoneOffsetNoSecurityNeeded
        {
            get
            {
                return Table2361.TimeZoneOffset;
            }
        }

        /// <summary>
        /// The DST Adjustment from MFG table 313 (2361).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public UInt16? DSTAdjustmentNoSecurityNeeded
        {
            get
            {
                return Table2361.DSTAdjustment;
            }
        }

        /// <summary>
        /// Whether the time is bad or not from MFG table 313 (2361).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public bool BadTimeNoSecurityNeeded
        {
            get
            {
                return Table2361.BadTime;
            }
        }

        #endregion

        #region Quantities

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
                    "VAR Delivered", Table23.CurrentRegisters);
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
                    "VAR Net", Table23.CurrentRegisters);
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
                    "VAR Net Delivered", Table23.CurrentRegisters);
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
                    "VAR Net Received", Table23.CurrentRegisters);
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
                    "VAR Quadrant 1", Table23.CurrentRegisters);
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
                    "VAR Quadrant 2", Table23.CurrentRegisters);
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
                    "VAR Quadrant 3", Table23.CurrentRegisters);
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
                    "VAR Quadrant 4", Table23.CurrentRegisters);
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
                    "VAR Received", Table23.CurrentRegisters);
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

        #endregion

        #region Instantaneous Values

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

        #endregion

        #region Demand Resets

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

        #endregion

        #region Self Reads

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
        /// Gets the configured daily self read time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 08/21/07 RCG 8.10.21 N/A    Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual string DailySelfReadTime
        {
            get
            {
                CENTRON_AMI_ModeControl AMIModeControl = Table2048.ModeControl as CENTRON_AMI_ModeControl;
                string strSelfReadTime = "Not Supported";

                if (VersionChecker.CompareTo(FWRevision, VERSION_1_RELEASE_1_5) >= 0)
                {
                    if (AMIModeControl != null)
                    {
                        strSelfReadTime = AMIModeControl.DailySelfReadTime;
                    }
                }

                return strSelfReadTime;
            }
        }

        #endregion

        #region Events and Exceptions

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
        //  12/17/15 AF  4.23.00 559019 Set the time format so that we can interpret the event data 
        //                              for set clock events
        //  12/29/15 AF  4.23.01 559019 Use the table 0 time format field instead of psem's
        //
        public override ANSIEventDictionary EventDescriptions
        {
            get
            {
                if (null == m_dicEventDescriptions)
                {
                    m_dicEventDescriptions = (ANSIEventDictionary)(new CENTRON_AMI_EventDictionary());
                    m_dicEventDescriptions.TimeFormat = Table00.TimeFormat;
                }
                return m_dicEventDescriptions;
            }
        }

        /// <summary>
        /// Gets the number of exceptions waiting to be sent in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 jrf 2.40.25        Created
        //
        public byte NumberOfCurrentExceptions
        {
            get
            {
                return Table2114C1222DebugInfo.NumberOfCurrentExceptions;
            }
        }

        #endregion

        #region Tampers

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
        // 02/17/16 AF  4.50.231 WR419822 Updated to use mfg table 712 if it exists
        //
        public virtual uint NumberOfRemovalTampers
        {
            get
            {
                byte[] Data = null;
                uint uiNumRemovals = 0;

                if (null != Table2760)
                {
                    uiNumRemovals = Table2760.RemovalTamperCount;
                }
                else
                {
                    PSEMResponse Result = PSEMResponse.Ok;

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
        // 02/17/16 AF  4.50.231 WR419822 Updated to use mfg table 712 if it exists
        //
        public virtual uint NumberOfInversionTampers
        {
            get
            {
                uint uiNumInversions = 0;

                if (null != Table2760)
                {
                    uiNumInversions = Table2760.InversionTamperCount;
                }
                else
                {
                    byte[] Data = null;
                    PSEMResponse Result = PSEMResponse.Ok;
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
                }

                return uiNumInversions;
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

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
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

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
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

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
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

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual bool AccelerometerConfigError
        {
            get
            {
                bool bError = false;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2262))
                    {
                        bError = Table2262.AccelerometerConfigError;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual bool WakeUpStatus
        {
            get
            {
                bool bStatus = false;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2262))
                    {
                        bStatus = Table2262.WakeUpStatus;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual bool RemovalPDNCheck
        {
            get
            {
                bool bCheck = false;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2262))
                    {
                        bCheck = Table2262.RemovalPDNCheck;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual bool TapDetected
        {
            get
            {
                bool bDetected = false;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2262))
                    {
                        bDetected = Table2262.TapCheck;
                    }
                }

                return bDetected;
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //  01/31/14 jrf 3.00.32 425489 Making this value available to read regardless of HW version if table is 
        //                              supported.
        //
        public virtual bool IsAccelerometerSupported
        {
            get
            {
                bool bSupported = false;

                if (IsTableUsed(2263))
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual float AccReferenceAngleX
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        fAngle = Table2263.ReferenceAngleX;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual float AccReferenceAngleY
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        fAngle = Table2263.ReferenceAngleY;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual float AccReferenceAngleZ
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        fAngle = Table2263.ReferenceAngleZ;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual float AccCurrentAngleX
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        fAngle = Table2263.CurrentAngleX;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual float AccCurrentAngleY
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        fAngle = Table2263.CurrentAngleY;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual float AccCurrentAngleZ
        {
            get
            {
                float fAngle = 0.0f;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        fAngle = Table2263.CurrentAngleZ;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual sbyte MaxDeltaX
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        sbyMaxDelta = Table2263.MaxDeltaX;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual sbyte MaxDeltaY
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        sbyMaxDelta = Table2263.MaxDeltaY;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual sbyte MaxDeltaZ
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        sbyMaxDelta = Table2263.MaxDeltaZ;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual sbyte MaxAvgDeltaTap
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        sbyMaxDelta = Table2263.MaxAvgDeltaTap;
                    }
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
        //  04/23/13 AF  2.80.22 390812 Make sure we don't try to read tamper tap tables for meters that don't 
        //                              support the tables
        //
        public virtual sbyte MaxAvgDeltaTamper
        {
            get
            {
                sbyte sbyMaxDelta = 0;

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    if (IsTableUsed(2263))
                    {
                        sbyMaxDelta = Table2263.MaxAvgDeltaTamper;
                    }
                }

                return sbyMaxDelta;
            }
        }

        /// <summary>
        /// Property to determine if Magnetic Tampers should be supported. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/18/12 jrf 2.70.18 TQ6657 Created
        //  08/15/14 jrf 3.70.03 529314 Modified to use the correct Carbon version Constant.
        public virtual bool MagneticTampersSupported
        {
            get
            {
                bool blnSupported = false;
                bool blnPolyphaseMeter = this is OpenWayBasicPoly;
                bool blnM2Gateway = this is M2_Gateway;

                //Let me start by saying, I wish there was a better way to determine support...
                if ((false == blnPolyphaseMeter) //It needs to be singlephase
                    //and not an M2 Gateway
                    && (false == blnM2Gateway)
                    //Must be HW 3.1 meter
                    && VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_3_6) >= 0
                    //FW must support it. Carbon will and what comes after should.
                    && VersionChecker.CompareTo(FWRevision, VERSION_5_5_CARBON) >= 0)
                {
                    blnSupported = true;
                }

                return blnSupported;
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
        // 02/17/16 AF  4.50.331 WR419822 Updated to use mfg table 712 if it exists
        //
        public virtual uint NumberOfMagneticTampersDetected
        {
            get
            {
                uint uiNumberDetected = 0;

                if (MagneticTampersSupported)
                {
                    if (null != Table2760)
                    {
                        uiNumberDetected = Table2760.MagneticTamperCount;
                    }
                    else if (null != Table2079)
                    {
                        uiNumberDetected = Table2079.MagneticTamperDetectCount;
                    }
                }

                return uiNumberDetected;
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
        // 06/24/13 RCG 3.71.00 413181 Correcting the count returned
        // 02/17/16 AF  4.50.331 WR419822 Updated to use mfg table 712 if it exists
        //
        public virtual byte NumberOfMagneticTampersCleared
        {
            get
            {
                byte byNumberCleared = 0;

                if (MagneticTampersSupported)
                {
                    if (null != Table2760)
                    {
                        byNumberCleared = Table2760.MagneticTamperClearedCount;
                    }
                    else if (null != Table2079)
                    {
                        byNumberCleared = Table2079.MagneticTamperClearedCount;
                    }
                }

                return byNumberCleared;
            }
        }

        #endregion

        #region LED Information

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
        /// Property to retrieve Normal Kh out of the Metrology Table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/08/09 jrf 2.30.26		Added check to remove PrismLite HW mask
        //                              when appropriate.
        //  01/27/10 RCG 2.40.09        Changing to use device class for checks
        //  04/28/11 jrf 2.50.38        Adding ITRT to the single phase meter check.
        //  03/22/11 jrf 2.80.10        Adding support for new ITRJ device object.
        //
        public int NormalKh
        {
            get
            {
                // Single Phase meters should get this data from 2048
                if (DeviceClass.Equals(ITR1_DEVICE_CLASS) || DeviceClass.Equals(ITRN_DEVICE_CLASS)
                    || DeviceClass.Equals(ITRD_DEVICE_CLASS) || DeviceClass.Equals(ITRT_DEVICE_CLASS)
                    || DeviceClass.Equals(ITRJ_DEVICE_CLASS))
                {
                    CTable2048_OpenWay OW2048 = Table2048 as CTable2048_OpenWay;

                    return OW2048.MetrologyTable.NormalKh;
                }
                else // use CPC config table (mfg table 122) 
                {
                    return Convert.ToInt32(Table2170.PulseWeightNormal);
                }
            }
        }

        #endregion

        #region Meter Behavior

        /// <summary>
        /// Gets whether or not the C12.18 passwords are currently hashed in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/09 RCG 2.30.09 N/A    Created

        public bool? AreC1218PasswordsHashed
        {
            get
            {
                bool? ArePasswordsHashed = null;

                if (Table2219 != null)
                {
                    ArePasswordsHashed = Table2219.AreC1218PasswordsHashed;
                }

                return ArePasswordsHashed;
            }
        }

        /// <summary>
        /// Gets whether or not JTAG security has been enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/09 RCG 2.30.09 N/A    Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual bool? IsJTAGSecurityEnabled
        {
            get
            {
                bool? JTAGSecurityEnabled = null;

                if (Table2219 != null)
                {
                    JTAGSecurityEnabled = Table2219.IsJTAGSecurityEnabled;
                }

                return JTAGSecurityEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not Enhanced Security Keys has been injected in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/23/09 MMD 2.30.20 N/A    Created

        public bool? EnhancedSecurityKeysInjected
        {
            get
            {
                bool? EnhanceSecurityKeys = null;

                if (Table2219 != null)
                {
                    EnhanceSecurityKeys = Table2219.EnhancedSecurityKeysInjected;
                }

                return EnhanceSecurityKeys;
            }
        }

        /// <summary>
        /// Gets whether or not Enhanced Security Keys has been injected in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/23/09 MMD 2.30.20 N/A    Created

        public EnhancedSecurityAlgorithmUsed EnhancedSecurityKeyType
        {
            get
            {
                EnhancedSecurityAlgorithmUsed eSecurityAlgorithm = EnhancedSecurityAlgorithmUsed.UNDEFINED;

                if (Table2221 != null)
                {
                    eSecurityAlgorithm = Table2221.KeyType;
                }

                return eSecurityAlgorithm;
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
        /// Gets whether or not the meter is Sealed for Canada.  Uncached 
        /// version created for OpenWay QC Tool.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/28/12 jrf 2.70.24 239723 Created
        //  02/03/17 jrf 4.71.06 WR 742568 Made is sealed an offset read to prevent getting
        //                                 a stale value.
        public bool IsSealedCanadianUncached
        {
            get
            {
                if (Table2220 != null)
                {
                    m_IsSealedCanadian.Value = Table2220.IsSealedCanadianFromOffsetRead;
                }
                else
                {
                    // Try to get this via LID
                    m_IsSealedCanadian.Value = base.IsSealedCanadian;
                }

                return m_IsSealedCanadian.Value;
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
        /// Gets the current security level
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/22/09 RCG 2.30.13	    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual SecurityLevel? CurrentSecurityLevel
        {
            get
            {
                if (m_CurrentSecurityLevel == null)
                {
                    SecurityLevel CurrentLevel = SecurityLevel.Level0;

                    // We should try to get the security level using Procedure 26
                    ProcedureResultCodes ResultCode = GetSecurityLevel(out CurrentLevel);

                    if (ResultCode == ProcedureResultCodes.COMPLETED)
                    {
                        m_CurrentSecurityLevel = CurrentLevel;
                    }
                }

                return m_CurrentSecurityLevel;
            }
        }

        /// <summary>
        /// Gets opt out status in the meter by checking the state of the Opt out flag (Register) and utility ID (RFLAN) or interface settings TLV(RFMESH).
        /// Both the register and comm module settings have to match to give a definitive Opt Out/In status.
        /// </summary>/
        public virtual Opt RFOptStatus
        {
            get
            {
                Opt OptStatus = Opt.Undecided;

                if (Table2061RFLAN != null)
                {
                    //Equal to 1 then opted out, otherwise it's not.
                    if (1 == Table2061RFLAN.RFLANOptOutByte)
                    {
                        OptStatus = Opt.Out;
                    }
                    else
                    {
                        OptStatus = Opt.In;
                    }
                }

                //For RFLAN, we have to check the utilty ID also.
                if (CommModule is RFLANCommModule)
                {
                    if (null != Table2121)
                    {
                        if (Opt.Out == OptStatus && 0 == Table2121.UtilityID)
                        {
                            OptStatus = Opt.Out;
                        }
                        else if (Opt.In == OptStatus && 0 < Table2121.UtilityID)
                        {
                            OptStatus = Opt.In;
                        }
                        else //There is a mismatch between opt out byte and rf lan utility ID.
                        {
                            OptStatus = Opt.Undecided;
                        }
                    }
                    else
                    {
                        //If we can't read utility ID for RFLAN, we can't determine opt out status
                        OptStatus = Opt.Undecided;
                    }
                }

                return OptStatus;
            }
        }

        /// <summary>
        /// Determines if the C1222 link is busy.
        /// </summary>
        public virtual bool IsC1222LinkBusy
        {
            get
            {
                bool Busy = false;
                byte[] Data;
                
                byte LanWanTableInterfaceState = 0;
                ushort MessageSenderSenderWorkingSegmentIndex = 0;
                ushort NumberOfSegments = 0;
                byte TriesLeft = 0;
                byte C1222ApplicationStatusState = 0;
                byte C1222ApplicationStatusLinkControlRunning = 0;
                byte C1222TransportStatusPendingRequest = 0;
                byte C1222TransportStatusState = 0;
                byte C1222TransportStatusNegotiateComplete = 0;
                byte C1222TransportStatusHaveConfiguration = 0;
                byte C1222TransportStatusCommModulePresent = 0;
                byte C1222TransportStatusWaitingForConfigRequest = 0;
                int C1222TransportStatusExtraOffset = 0;

                //c1222 link only appropriate for rflan
                if (CommModule is RFLANCommModule)
                {
                    //We have to account for fields being added in the middle of mfg. table 14(2062) by different firmware versions.
                    if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_MICHIGAN) >= 0)
                    {
                        C1222TransportStatusExtraOffset += 4;
                    }

                    if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_6_005_BERYLLIUM_HW_3_x) >= 0)
                    {
                        C1222TransportStatusExtraOffset += 12;
                    }

                    try
                    {
                        //To save time psem offset reads are performed below to get the table data. In an ideal 
                        //implementation, the ANSITable classes would be updated to handle reading this data 
                        //and provide properties for accessing each of the values below.
                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2114, 0x3B, 1, out Data))
                        {
                            LanWanTableInterfaceState = Data[0];

                            //lanWanTableInterface_state must be 0(IDLE) in ITR 66, if not, it is busy
                            if (LanWanTableInterfaceState != 0) { Busy = true; }
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2114, 0x8C, 2, out Data))
                        {
                            MessageSenderSenderWorkingSegmentIndex = BitConverter.ToUInt16(Data, 0);
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2114, 0x84, 2, out Data))
                        {
                            NumberOfSegments = BitConverter.ToUInt16(Data, 0);
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2114, 0x90, 1, out Data))
                        {
                            TriesLeft = Data[0];
                        }
                        else { Busy = true; }

                        //C1222 Application messageSender.senderworkingSegmentIndex must be >= numberOfSegments, 
                        //or triesLeft is 0 in ITR 66, if not, it is busy
                        if (MessageSenderSenderWorkingSegmentIndex < NumberOfSegments && TriesLeft != 0) { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2062, 0x00, 1, out Data))
                        {
                            C1222ApplicationStatusState = Data[0];

                            //C1222 Application Status.state must be 2 (ALS_IDLE) in ITR 14, if not, it is busy
                            if (C1222ApplicationStatusState != 2) { Busy = true; }
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2062, 0x27, 1, out Data))
                        {
                            C1222ApplicationStatusLinkControlRunning = Data[0];

                            //C1222 Application Status. linkControlRunning must be 0 in ITR 14, if not, it is busy
                            if (C1222ApplicationStatusLinkControlRunning != 0) { Busy = true; }
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2062, 0xE5 + C1222TransportStatusExtraOffset, 1, out Data))
                        {
                            C1222TransportStatusPendingRequest = Data[0];

                            //In ITR 14, C1222Transport status.pendingRequest must be 0, if not, it is busy
                            if (C1222TransportStatusPendingRequest != 0) { Busy = true; }
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2062, 0xDE + C1222TransportStatusExtraOffset, 1, out Data))
                        {
                            C1222TransportStatusState = Data[0];

                            //In ITR 14, C1222Transport status.state must be TLS_NORMAL( value = 2), if not, it is busy
                            if (C1222TransportStatusState != 2) { Busy = true; }

                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2062, 0xDF + C1222TransportStatusExtraOffset, 1, out Data))
                        {
                            C1222TransportStatusNegotiateComplete = Data[0];

                            //In ITR 14, C1222Transport status.negotiateComplete must be 1, if not, it is busy
                            if (C1222TransportStatusNegotiateComplete != 1) { Busy = true; }
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2062, 0xE0 + C1222TransportStatusExtraOffset, 1, out Data))
                        {
                            C1222TransportStatusHaveConfiguration = Data[0];

                            //In ITR 14, C1222Transport status.haveConfiguration must be 1, if not, it is busy
                            if (C1222TransportStatusHaveConfiguration != 1) { Busy = true; }
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2062, 0xE1 + C1222TransportStatusExtraOffset, 1, out Data))
                        {
                            C1222TransportStatusCommModulePresent = Data[0];

                            //In ITR 14, C1222Transport status.commModulePresent must be 1, if not, it is busy
                            if (C1222TransportStatusCommModulePresent != 1) { Busy = true; }
                        }
                        else { Busy = true; }

                        if (!Busy && PSEMResponse.Ok == m_PSEM.OffsetRead(2062, 0xE2 + C1222TransportStatusExtraOffset, 1, out Data))
                        {
                            C1222TransportStatusWaitingForConfigRequest = Data[0];

                            //In ITR 14, C1222Transport status.waitingForConfigRequest must be 0, if not, it is busy
                            if (C1222TransportStatusWaitingForConfigRequest != 0) { Busy = true; }
                        }
                        else { Busy = true; }
                    }
                    catch
                    {
                        Busy = true;
                    }
                }

                return Busy;
            }
        }

        /// <summary>
        /// Gets whether or not the RFLAN is enabled in the meter by checking the state of the Opt out flag and utility ID.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/21/11 MSC 2.52.27         Created
        // 11/11/11 MSC 2.53.03         Modified for testing purposes.
        // 01/19/12 MSC 2.53.30         Changed to check RFLAN Opt Out Byte.
        // 02/14/12 MSC 2.53.39 193781  Will no longer fail on meters without RFLAN Opt Out feature.
        // 04/25/12 mah 2.53.56 197505  Added 0xff as an indication that the RFLAN is enabled
        // 10/02/13 AF  3.00.11 WR392908 Added a backup way to determine if RFLAN is enabled when
        //                              reading table 2061 throws an ISC exception
        //
        public virtual bool IsRFLANEnabled
        {
            get
            {
                bool blnEnabled = true;

                //Only reason RFLAN should not be enabled is if procedure exists and has been used to disable it
                if (IsProcedureUsed((ushort)Procedures.RFLAN_OPT_OUT))
                {
                    try
                    {
                        if (IsProcedureUsed((ushort)Procedures.RFLAN_OPT_OUT) && Table2061RFLAN != null && Table2121 != null)
                        {
                            if (1 == Table2061RFLAN.RFLANOptOutByte && 0 == Table2121.UtilityID)
                            {
                                blnEnabled = false;
                            }
                        }
                    }
                    catch (PSEMException PSEMexp)
                    {
                        // If the user is logged in with a secondary, tertiary, or quaternary password, they
                        // cannot read Factory Data (2061) so use an alternative method of determining if RFLAN is enabled
                        if (PSEMexp.PSEMResponse == Communications.PSEM.PSEMResponse.Isc)
                        {
                            if (Table2121 != null)
                            {
                                blnEnabled = (Table2121.UtilityID != 0);
                            }
                        }
                        else
                        {
                            throw PSEMexp;
                        }
                    }
                }

                return blnEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not the RFMesh is enabled in the meter by checking the state of the Opt out flag.
        /// Returns null if security level prevents reading mfg. table 13.
        /// </summary>
        public virtual bool? IsRFMeshEnabled
        {
            get
            {
                bool? blnEnabled = null;

                //Only reason RFMesh should not be enabled is if procedure exists and has been used to disable it
                if (IsProcedureUsed((ushort)Procedures.RFLAN_OPT_OUT))
                {
                    try
                    {
                        if (IsProcedureUsed((ushort)Procedures.RFLAN_OPT_OUT) && Table2061RFLAN != null)
                        {
                            blnEnabled = (1 != Table2061RFLAN.RFLANOptOutByte);
                        }
                    }
                    catch (PSEMException PSEMexp)
                    {
                        // If the user is logged in with a secondary, tertiary, or quaternary password, they
                        // cannot read Factory Data (2061). Since there is no alternative method of determining if RFMesh is enabled
                        // we will leave value unassigned
                        if (PSEMexp.PSEMResponse == Communications.PSEM.PSEMResponse.Isc)
                        {
                            blnEnabled = null;
                        }
                        else
                        {
                            throw PSEMexp;
                        }
                    }
                }
                else  //if opt out procedure is not supported then assume enabled
                {
                    blnEnabled = true;
                }

                return blnEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is opted out by checking the state of the Opt out flag
        /// </summary>
        public bool FactoryConfigOptOut
        {
            get
            {
                bool OptedOut = false;

                if (Table2061RFLAN != null)
                {
                    if (1 == (int)Table2061RFLAN.RFLANOptOutByte)
                    {
                        OptedOut = true;
                    }
                }

                return OptedOut;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is in quiet mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        //  09/16/13 jrf 2.85.44 WR423624 Only checking this value for Lithium+ or greater firmware.

        public bool IsQuietModeActive
        {
            get
            {
                // If the table doesn't exist we can assume it's disabled.
                bool IsActive = false;

                //Lithium says this table is supported but it lies. To avoid PSEM IAR resposnse we will check firmware version
                //and only attempt a read if FW is lithium or greater.
                if (Table2288 != null && VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_LITHIUM_PLUS_3_14) >= 0)
                {
                    IsActive = Table2288.IsQuietModeActive;
                }

                return IsActive;
            }
        }

        /// <summary>
        /// Gets whether or not the firmware version is Beryllium
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  --------- -------------------------------------------
        //  08/24/15 PGH 4.50.200 REQ574469 Created
        //  09/21/15 PGH 4.50.206 REQ574469 Updated
        //
        public bool IsBeryllium
        {
            get
            {
                return (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_BERYLLIUM) == 0);
            }
        }

        /// <summary>
        /// Gets whether or not the firmware version is Beryllium or greater
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  --------- -------------------------------------------
        //  11/20/15 PGH 4.50.216 REQ574469 Created
        //
        public bool IsBerylliumOrGreater
        {
            get
            {
                return (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_BERYLLIUM) >= 0);
            }
        }

        /// <summary>
        /// Gets whether or not the firmware version is 4GLTE or greater
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  --------- -------------------------------------------
        //  10/14/15 PGH 4.50.207 615332 Created
        //
        public bool Is4GLTEOrGreater
        {
            get
            {
                return (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_SR6_6_4GLTE) >= 0);
            }
        }

        #endregion

        #region Meter Information

        /// <summary>
        /// Gets the Date Programmed out of the header of 2048
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue#     Description
        //  -------- --- ------- ---------------------------------------------
        //  10/28/08 KRC 2.00.03 00121848    Created
        // 		
        public override DateTime DateProgrammed
        {
            get
            {
                DateTime dtTimeProgrammed = MeterConfigurationReferenceTime;

                //Get the Date Programmed out of 2048
                uint usDateProgrammed = Table2048.Table2048Header.DateProgrammed;

                // Value in 2048 is the number of seconds since Jan. 1, 2000, so to get
                //  the value returned to Jan. 1, 2000.
                dtTimeProgrammed = dtTimeProgrammed.AddSeconds((double)usDateProgrammed);

                return dtTimeProgrammed;
            }
        }

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
        /// Gets the HW Revision with the Prism Lite bit filtered out.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/01/10 RCG 2.40.31	    Created
        // 12/12/13 AF  3.50.14         Class re-architecture - promoted to CANSIDevice and overridden here
        //
        public override float HWRevisionFiltered
        {
            get
            {
                float fRevision = HWRevision;

                if (VersionChecker.CompareTo(fRevision, PRISM_LITE_REVISION) >= 0)
                {
                    // It's a Prism Lite meter so subtract to get the real version
                    fRevision -= PRISM_LITE_REVISION;
                }

                if (VersionChecker.CompareTo(fRevision, HW_VERSION_2_1) == 0)
                {
                    // This is an oscillator meter set it to 2.000
                    fRevision = HW_VERSION_2_0;
                }
                else if (VersionChecker.CompareTo(fRevision, HW_VERSION_2_6) == 0)
                {
                    // This is a Poly oscillator meter set it to 2.050
                    fRevision = HW_VERSION_2_5;
                }

                return fRevision;
            }
        }

        /// <summary>
        /// Gets the Firmware Loader Version number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/10 RCG 2.40.07        Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual byte FWLoaderVersion
        {
            get
            {
                if (m_FWLoaderVersion.Cached == false)
                {
                    ReadFWLoaderVersion();
                }

                return m_FWLoaderVersion.Value;
            }
        }

        /// <summary>
        /// Gets the Firmware Loader Revision number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/10 RCG 2.40.07        Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override

        public virtual byte FWLoaderRevision
        {
            get
            {
                if (m_FWLoaderRevision.Cached == false)
                {
                    ReadFWLoaderVersion();
                }

                return m_FWLoaderRevision.Value;
            }
        }

        /// <summary>
        /// Gets the Firmware Loader Build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/10 RCG 2.40.07        Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual byte FWLoaderBuild
        {
            get
            {
                if (m_FWLoaderBuild.Cached == false)
                {
                    ReadFWLoaderVersion();
                }

                return m_FWLoaderBuild.Value;
            }
        }

        /// <summary>
        /// Returns the version.revision of the ATMEGA module
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/13/06 AF  8.00.00        Created
        //
        public float AtmelVerRev
        {
            get
            {
                float fltVerRev = 0.0F;
                PSEMResponse Result = PSEMResponse.Ok;
                object objData;

                Result = m_lidRetriever.RetrieveLID(m_LID.ATMEL_FIRMWARE_VER_REV, out objData);

                if (PSEMResponse.Ok == Result)
                {
                    fltVerRev = (float)objData;
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the ATMEL firmware version"));
                }

                return fltVerRev;
            }
        }


        /// <summary>
        /// Gets the exception security model for the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/08 RCG 2.00.02        Created

        public OpenWayMFGTable2193.SecurityFormat? ExceptionSecurityModel
        {
            get
            {
                OpenWayMFGTable2193.SecurityFormat? SecurityModel = null;

                if (Table2193 != null)
                {
                    SecurityModel = Table2193.ExceptionModel;
                }

                return SecurityModel;
            }
        }

        /// <summary>
        /// Gets whether or not enhanced security is required.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/08 RCG 2.00.02        Created

        public bool? IsEnhancedSecurityRequired
        {
            get
            {
                bool? bSecurityRequired = null;

                if (Table2193 != null)
                {
                    bSecurityRequired = Table2193.IsEnhancedSecurityRequired;
                }

                return bSecurityRequired;
            }
        }

        /// <summary>
        /// Gets the Power Calculation Method for Polyphase meters.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/04/11 RCG 2.50.06        Created

        public PowerCalculationType PowerCalculationMethod
        {
            get
            {
                PowerCalculationType CalculationMethod = PowerCalculationType.Arithmatic;

                if (Table2170 != null)
                {
                    CalculationMethod = Table2170.PowerCalculationMethod;
                }
                else
                {
                    throw new InvalidOperationException("Can not retrieve Power Calculation Method for this device. Table 2170 is not available");
                }

                return CalculationMethod;
            }
        }

        /// <summary>
        /// Property to get the custom schedule name from the device.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/14/07 jrf 8.00.18 2521	Created
        //  
        public override string CustomScheduleName
        {
            get
            {
                throw new NotImplementedException();
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
        // 03/16/07 jrf 8.00.19 2653   Created
        // 09/15/09 AF  2.30.00 137695 Added code to display a host meter device
        // 01/14/10 jrf 2.40.06        Adding check to make sure comm module is present
        //                             before trying to read the comm module's device class.
        //
        public override string MeterName
        {
            get
            {
                string strName = CENTRONAMI_NAME;

                if (DeviceClass == ITRT_DEVICE_CLASS)
                {
                    strName = ITRT_NAME;
                }
                else if (true == ItronCommModulePresent && CommModuleDeviceClass == ITRL_DEVICE_CLASS)
                {
                    strName = ITRL_NAME;
                }

                return strName;
            }
        }

        /// <summary>
        /// Gets the meter name that will be used in the activity log.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/08/09 RCG 2.20.11		Created

        public override string ActivityLogMeterName
        {
            get
            {
                string strName = LOG_ITR1_NAME;

                if (DeviceClass == ITRT_DEVICE_CLASS)
                {
                    strName = LOG_ITRT_NAME;
                }

                return strName;
            }
        }

        /// <summary>
        /// Property to determine the Display Mode
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/05/09 jrf 2.20.08 135495	Overrode this method to determine mode 
        //                              from table 3.
        // 
        public override DisplayMode MeterDisplayMode
        {
            get
            {
                DisplayMode Mode = DisplayMode.NORMAL_MODE;

                // Openway meter only has two supported modes.
                if (true == Table03.InTestMode)
                {
                    Mode = DisplayMode.TEST_MODE;
                }

                return Mode;
            }
        }

        #endregion

        #region CPP

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

                if (VersionChecker.CompareTo(FWRevision, VERSION_HYDROGEN_3_6) >= 0)
                {
                    startTime = Table2360.StartTimeGmt;
                }

                return startTime;
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

                if (VersionChecker.CompareTo(FWRevision, VERSION_HYDROGEN_3_6) >= 0)
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

                if (VersionChecker.CompareTo(FWRevision, VERSION_HYDROGEN_3_6) >= 0)
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

                if (VersionChecker.CompareTo(FWRevision, VERSION_HYDROGEN_3_6) >= 0)
                {
                    rate = Table2360.Rate;
                }

                return rate;
            }
        }

#endif
        #endregion

        #region Fatal Errors and Core Dump

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

        #endregion

        #region HAN Information

        /// <summary>
        /// The amount of time remaining for ZigBee to be disabled. This value returns null
        /// unless the ZigBee State is set to disabled for a time period
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/09 RCG 2.30.09 N/A    Created

        public TimeSpan? ZigBeeDisabledTimeRemaining
        {
            get
            {
                TimeSpan? TimeRemaining = null;

                if (Table2219 != null && Table2219.ZigBeeState == FeatureState.DisabledForPeriod)
                {
                    TimeRemaining = Table2219.ZigBeeDisabledTimeRemaining;
                }

                return TimeRemaining;
            }
        }

        /// <summary>
        /// Gets the current state of ZigBee in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/09 RCG 2.30.09 N/A    Created

        public FeatureState? ZigBeeState
        {
            get
            {
                FeatureState? CurrentState = null;

                if (Table2219 != null)
                {
                    CurrentState = Table2219.ZigBeeState;
                }

                return CurrentState;
            }
        }

        /// <summary>
        /// Gets the Expiration Date of the current Recurring Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/21/12 RCG 2.60.24        Created

        public DateTime? HANRecurringPriceExpirationDate
        {
            get
            {
                DateTime? ExpirationDate = null;

                // The expiration should only ever be in one place or the other
                if (m_HANInfo.Table2134 != null && m_HANInfo.Table2134.ExpirationDate != null)
                {
                    ExpirationDate = m_HANInfo.Table2134.ExpirationDate;
                }
                else if (Table2297 != null && Table2297.ExpirationDate != null)
                {
                    ExpirationDate = Table2297.ExpirationDate;
                }

                return ExpirationDate;
            }
        }

        /// <summary>
        /// Gets the Current HAN OTA File ID from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        public uint HANOTAHeaderFileID
        {
            get
            {
                uint ID = 0;

                if (null != Table2093)
                {
                    ID = Table2093.FieldID;
                }

                return ID;
            }
        }

        /// <summary>
        /// Gets the Formatted HAN OTA Header Version from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderVersion
        {
            get
            {
                string Version = "0.000";

                if (null != Table2093)
                {
                    Version = Table2093.FormattedHeaderVersion;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Header Length from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        public ushort HANOTAHeaderLength
        {
            get
            {
                ushort Length = 0;

                if (null != Table2093)
                {
                    Length = Table2093.HeaderLength;
                }

                return Length;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Header Field Control Security Credential Version Present from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public bool HANOTAHeaderSecurityCredentialVersionPresent
        {
            get
            {
                bool Present = false;

                if (null != Table2093)
                {
                    Present = Table2093.HeaderSecurityCredentialVersionPresent;
                }

                return Present;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Header Field Control Device Specific File from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public bool HANOTAHeaderDeviceSpecificFile
        {
            get
            {
                bool DeviceSpecific = false;

                if (null != Table2093)
                {
                    DeviceSpecific = Table2093.HeaderDeviceSpecificFile;
                }

                return DeviceSpecific;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Header Field Control Hardware Versions Present from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public bool HANOTAHeaderHardwareVersionsPresent
        {
            get
            {
                bool Present = false;

                if (null != Table2093)
                {
                    Present = Table2093.HeaderHardwareVersionsPresent;
                }

                return Present;
            }
        }

        /// <summary>
        /// Gets the HAN OTA manufacturer code from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        public ushort HANOTAHeaderManufacturerCode
        {
            get
            {
                ushort Code = 0;

                if (null != Table2093)
                {
                    Code = Table2093.ManufacturerCode;
                }

                return Code;
            }
        }

        /// <summary>
        /// Gets the HAN OTA image type name from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderImageTypeName
        {
            get
            {
                string Type = "Unknown";

                if (null != Table2093)
                {
                    Type = Table2093.ImageTypeName;
                }

                return Type;
            }
        }

        /// <summary>
        /// Gets the HAN OTA application release version from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderApplicationReleaseVersion
        {
            get
            {
                string Version = "0.000";

                if (null != Table2093)
                {
                    Version = Table2093.ApplicationReleaseVersion;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the HAN OTA application build from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public byte HANOTAHeaderApplicationBuild
        {
            get
            {
                byte Build = 0;

                if (null != Table2093)
                {
                    Build = Table2093.ApplicationBuild;
                }

                return Build;
            }
        }

        /// <summary>
        /// Gets the HAN OTA stack release version from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderStackReleaseVersion
        {
            get
            {
                string Version = "0.000";

                if (null != Table2093)
                {
                    Version = Table2093.StackReleaseVersion;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the HAN OTA stack build from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public byte HANOTAHeaderStackBuild
        {
            get
            {
                byte Build = 0;

                if (null != Table2093)
                {
                    Build = Table2093.StackBuild;
                }

                return Build;
            }
        }

        /// <summary>
        /// Gets the HAN OTA ZigBee Stack version from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderZigBeeStackVersion
        {
            get
            {
                string Version = "Unknown";

                if (null != Table2093)
                {
                    Version = Table2093.ZigBeeStackVersionName;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Header String from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderString
        {
            get
            {
                string HeaderString = "Unknown";

                if (null != Table2093)
                {
                    HeaderString = Table2093.InterpretedHeaderString;
                }

                return HeaderString;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Header Total Image Size from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        public uint HANOTAHeaderTotalImageSize
        {
            get
            {
                uint Size = 0;

                if (null != Table2093)
                {
                    Size = Table2093.ImageSize;
                }

                return Size;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Security Credential version from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderSecurityCredentialVersion
        {
            get
            {
                string Version = "Unknown";

                if (null != Table2093)
                {
                    Version = Table2093.SecurityCredentialVersionName;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Header Upgrade File Destination from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public ulong HANOTAHeaderUpgradeFileDestination
        {
            get
            {
                ulong Destination = 0;

                if (null != Table2093)
                {
                    Destination = Table2093.IEEEAddress;
                }

                return Destination;
            }
        }

        /// <summary>
        /// Gets the HAN OTA minimum hardware version from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderMinHWVersion
        {
            get
            {
                string Version = "Unknown";

                if (null != Table2093)
                {
                    Version = Table2093.FormattedMinHardwareVersion;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the HAN OTA maximum hardware version from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public string HANOTAHeaderMaxHWVersion
        {
            get
            {
                string Version = "Unknown";

                if (null != Table2093)
                {
                    Version = Table2093.FormattedMaxHardwareVersion;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the HAN OTA Firmware Version from Mfg table 2093
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 08/24/16 PGH 4.70.15 701952 Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt32.ToString(System.String)")]
        public virtual string HANOTAFirmwareVersion
        {
            get
            {
                return Table2093.FileVersion.ToString("X8");
            }
        }

        /// <summary>
        /// Gets the Query Jitter from Mfg table 2094
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.17 WI 714619 Created
        public byte HANOTAQueryJitter
        {
            get
            {
                byte Jitter = 0;

                if (null != Table2094)
                {
                    Jitter = Table2094.QueryJitter;
                }

                return Jitter;
            }
        }

        /// <summary>
        /// Gets the Data Size from Mfg table 2094
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.17 WI 714619 Created
        public byte HANOTADataSize
        {
            get
            {
                byte Size = 0;

                if (null != Table2094)
                {
                    Size = Table2094.DataSize;
                }

                return Size;
            }
        }

        /// <summary>
        /// Gets the Current Time from Mfg table 2094
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.17 WI 714619 Created
        //  09/14/16 jrf 4.70.17 WI 714619 Modified to display times as local meter times.
        public DateTime HANOTACurrentTime
        {
            get
            {
                DateTime CurrentTime = new DateTime(2000, 1, 1);

                if (null != Table2094)
                {
                    if (CurrentTime != Table2094.CurrentTime)
                    {
                        CurrentTime = ConvertUTCToLocalMeterTime(Table2094.CurrentTime);
                    }
                }

                return CurrentTime;
            }
        }

        /// <summary>
        /// Gets the Upgrade Time from Mfg table 2094
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.17 WI 714619 Created
        //  09/14/16 jrf 4.70.17 WI 714619 Modified to display times as local meter times.
        public DateTime HANOTAUpgradeTime
        {
            get
            {
                DateTime UpgradeTime = new DateTime(2000, 1, 1);

                if (null != Table2094)
                {
                    if (UpgradeTime != Table2094.UpgradeTime)
                    {
                        UpgradeTime = ConvertUTCToLocalMeterTime(Table2094.UpgradeTime);
                    }
                }

                return UpgradeTime;
            }
        }

        /// <summary>
        /// Gets the number of active poll messages.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/06/16 jrf 4.70.16 WI 710078 Created       
        public ushort NumberOfPollMessages
        {
            get
            {
                ushort MsgCount = 0;

                if (null != Table2096)
                {
                    MsgCount = Table2096.NumberOfPollMessages;
                }

                return MsgCount;
            }
        }

        /// <summary>
        /// Gets the HAN OTA poll messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/06/16 jrf 4.70.16 WI 710078 Created       
        public ReadOnlyCollection<HAN_OTA_Poll_Msg> HANOTAPollMessages
        {
            get
            {
                ReadOnlyCollection<HAN_OTA_Poll_Msg> Messages = null;

                if (null != Table2096)
                {
                    Messages = Table2096.HANOtaPollMsg.AsReadOnly();
                }

                return Messages;
            }
        }

        /// <summary>
        /// Gets the number of ZigBee end devices with FWDL in progress.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/06/16 jrf 4.70.16 WI 710080 Created       
        public ushort NumberOfZigBeeEndDevicesWithFWDLInProgress
        {
            get
            {
                ushort DeviceCount = 0;

                if (null != Table2097)
                {
                    DeviceCount = Table2097.NumberOfDevicesInFWDLProgress;
                }

                return DeviceCount;
            }
        }

        /// <summary>
        /// Gets the ZigBee end device statuses for HAN OTA FWDL.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/06/16 jrf 4.70.16 WI 710080 Created       
        public ReadOnlyCollection<HANOTAStatRcd> HANOTAFWDLDeviceStatuses
        {
            get
            {
                ReadOnlyCollection<HANOTAStatRcd> Statuses = null;

                if (null != Table2097)
                {
                    Statuses = Table2097.HanOtaPollRcd.AsReadOnly();
                }

                return Statuses;
            }
        }

        /// <summary>
        /// Gets wheter or not HAN OTA Activate flag is set.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.17 WI 714616 Created       
        public bool HANOTAActivate
        {
            get
            {
                bool Activate = false;

                if (null != Table2095)
                {
                    Activate = Table2095.HAN_OTA_Activate;
                }

                return Activate;
            }
        }

        /// <summary>
        /// Gets the image notify bit mask.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714616 Created       
        public ushort HANOTAImageNotifyBitMask
        {
            get
            {
                ushort BitMask = 0;

                if (null != Table2095)
                {
                    BitMask = Table2095.ImageNotifyBitMask;
                }

                return BitMask;
            }
        }

        /// <summary>
        /// Gets the image activation bit mask.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714616 Created       
        public ushort HANOTAImageActivationBitMask
        {
            get
            {
                ushort BitMask = 0;

                if (null != Table2095)
                {
                    BitMask = Table2095.ImageActivationBitMask;
                }

                return BitMask;
            }
        }

        /// <summary>
        /// Gets the number of ZigBee end devices with diagnostics.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.16 WI 714616 Created       
        public ushort NumberOfZigBeeEndDevicesWithDiagnostics
        {
            get
            {
                ushort DeviceCount = 0;

                if (null != Table2095)
                {
                    DeviceCount = Table2095.nDevices;
                }

                return DeviceCount;
            }
        }

        /// <summary>
        /// Gets the ZigBee end device diagnostics for HAN OTA FWDL.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.16 WI 714616 Created       
        public ReadOnlyCollection<HANDiagnosticsElementRcd> HANOTADeviceDiagnostics
        {
            get
            {
                ReadOnlyCollection<HANDiagnosticsElementRcd> DeviceDiagnostics = null;

                if (null != Table2095)
                {
                    DeviceDiagnostics = Table2095.rcdDiagElmt.AsReadOnly();
                }

                return DeviceDiagnostics;
            }
        }

        #endregion

        #region HAN Resets

        /// <summary>
        /// Gets the total number of HAN Resets
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/11 RCG 2.50.36        Created
        //
        public uint? HANResetCount
        {
            get
            {
                uint? Count = null;

                if (Table2290 != null)
                {
                    Count = Table2290.TotalHANResets;
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the number of HAN Resets due to Code detectable Fatal Errors
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/11 RCG 2.50.36        Created
        //
        public ushort? HANCodeFatalErrorResetCount
        {
            get
            {
                ushort? Count = null;

                if (Table2290 != null)
                {
                    Count = Table2290.TotalCodeFatalErrors;
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the number of HAN Resets due to Core Fault errors
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/11 RCG 2.50.36        Created
        //
        public ushort? HANCoreFaultResetCount
        {
            get
            {
                ushort? Count = null;

                if (Table2290 != null)
                {
                    Count = Table2290.TotalCoreFaults;
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the number of HAN Resets due to First Use
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/11 RCG 2.50.36        Created
        //
        public ushort? HANFirstUseResetCount
        {
            get
            {
                ushort? Count = null;

                if (Table2290 != null)
                {
                    Count = Table2290.TotalFirstUseResets;
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the number of HAN Resets due to Stack Lockup errors
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/11 RCG 2.50.36        Created
        //
        public ushort? HANStackLockupResetCount
        {
            get
            {
                ushort? Count = null;

                if (Table2290 != null)
                {
                    Count = Table2290.TotalStackLockups;
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the number of HAN Resets due to Watchdog errors
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/11 RCG 2.50.36        Created
        //
        public ushort? HANWatchdogResetCount
        {
            get
            {
                ushort? Count = null;

                if (Table2290 != null)
                {
                    Count = Table2290.TotalWatchdogErrors;
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the date and time of the last HAN Reset
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/11 RCG 2.50.36        Created
        //
        public DateTime? HANLastResetTime
        {
            get
            {
                DateTime? ResetTime = null;

                if (Table2290 != null)
                {
                    ResetTime = Table2290.LastResetTime;
                }

                return ResetTime;
            }
        }

        /// <summary>
        /// Gets the HAN Reset Log
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/11 RCG 2.50.36        Created
        //
        public List<HANResetLogEntry> HANResetLog
        {
            get
            {
                List<HANResetLogEntry> ResetLog = null;

                if (Table2290 != null)
                {
                    ResetLog = Table2290.ResetLogEntries;
                }

                return ResetLog;
            }
        }

        /// <summary>
        /// Gets the Highest Mark
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 MSC 2.51.05        Created
        //
        public ushort? HANHighWaterMark
        {
            get
            {
                ushort? Value = null;

                if (Table2290 != null)
                {
                    Value = Table2290.HighWaterMark;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the Current Reset Limit (Period)
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 MSC 2.51.05        Created
        //
        public ushort? HANCurrentResetLimitPeriod
        {
            get
            {
                ushort? Period = null;

                if (Table2290 != null)
                {
                    Period = Table2290.CurrentResetLimitPeriod;
                }

                return Period;
            }
        }

        /// <summary>
        /// Gets the Current Reset Limit
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 MSC 2.51.05        Created
        //
        public ushort? HANCurrentResetLimitCount
        {
            get
            {
                ushort? Count = null;

                if (Table2290 != null)
                {
                    Count = Table2290.CurrentResetLimitCount;
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the Reset Lockout Bit
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 MSC 2.51.05        Created
        //
        public byte? HANLockoutBit
        {
            get
            {
                byte? Lock = null;

                if (Table2290 != null)
                {
                    Lock = Table2290.OverResetLimit;
                }

                return Lock;
            }
        }

        /// <summary>
        /// Gets the Reset Lockout Bit
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/08/11 MSC 2.51.05        Created
        //  06/17/15 AF  4.20.14 591841 The table 2290 does not always get updated when the halt condition 
        //                              is cleared so we must also check mfg table 59 current network status
        //
        public bool? HANisHaltedDueToOverResetLimit
        {
            get
            {
                bool? Lock = null;

                if (Table2290 != null && m_HANInfo != null)
                {
                    Lock = Table2290.isHaltedDueToOverResetLimit && 
                                    m_HANInfo.CurrentNetworkStatus != (byte)(HANCurrentNetworkStatus.NETWORK_UP) && 
                                    m_HANInfo.CurrentNetworkStatus != (byte)(HANCurrentNetworkStatus.NETWORK_FORMING);
                }

                return Lock;
            }
        }

        /// <summary>
        /// Gets the contents of the HAN Reset Log as a string
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/11/11 RCG 2.53.05        Created
        //
        public string HANResetLogString
        {
            get
            {
                string ResetLogString = "";

                if (Table2290 != null)
                {
                    ResetLogString = Table2290.ToString();
                }

                return ResetLogString;
            }
        }

        #endregion

        #region Extended Registers

        /// <summary>
        /// Gets the Extended Energy Register values
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 03/01/12 MAH 2.53.46 TRQ     Created 

        public List<ExtendedCurrentEntryRecord> ExtendedInstantaneousRegisters
        {
            get
            {
                List<ExtendedCurrentEntryRecord> Registers = null;

                if (Table2422 != null)
                {
                    Registers = Table2422.CurrentExtInstantaneousData.ToList();
                }

                return Registers;
            }
        }

        /// <summary>
        /// Gets the Extended Energy Register values
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/18/12 RCG 2.53.31 TRQ3438 Created 

        public List<ExtendedCurrentEntryRecord> ExtendedEnergyRegisters
        {
            get
            {
                List<ExtendedCurrentEntryRecord> Registers = null;

                if (Table2422 != null)
                {
                    Registers = Table2422.CurrentExtEnergyData.ToList();
                }

                return Registers;
            }
        }

        #endregion

        #region Extended Load Profile

        /// <summary>
        /// Gets a boolean that represents whether or not Extended Load Profile is running.
        /// </summary>
        /// <exception cref="PSEMException">Thrown when a communication error occurs.</exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/03/12 jrf 2.53.53 TREQ2891 Created for unit testing bug fix.
        // 04/18/12 jrf 2.53.55 196403 Unit testing bug fix to work around firmware bug.
        //
        public bool ExtendedLPRunning
        {
            get
            {
                object objValue;
                PSEMResponse Result;
                bool m_bRunning = false;

                if (true == ExtLoadProfileSupported)
                {

                    Result = m_lidRetriever.RetrieveLID(m_LID.EXT_LP_RUNNING, out objValue);

                    //Work around for Firmware issue reading this value when extended load profile is not running
                    if (PSEMResponse.Iar == Result)
                    {
                        m_bRunning = false;
                    }
                    else if (Result != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Extended Load Profile running flag.");
                    }
                    else
                    {
                        // The data returned should be 1 byte
                        m_bRunning = (byte)objValue == (byte)1;
                    }
                }

                return m_bRunning;
            }
        }

        /// <summary>
        /// Gets whether the meter supports extended load profile.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 185305 Created
        //
        public bool ExtLoadProfileSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(2409) && true == Table00.IsTableUsed(2410)
                    && true == Table00.IsTableUsed(2411) && true == Table00.IsTableUsed(2412));

                return blnSupported;
            }
        }

        /// <summary>
        /// The number of valid extended load profile blocks in mfg. LP set 1.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/04/12 jrf 2.53.24 185305 Created.
        // 01/18/12 RCG 2.53.31        Renaming for consistency
        // 04/03/12 jrf 2.53.53 TREQ2891 Unit testing bug fix.
        //
        public ushort ExtendedLoadProfileCurrentNumberOfBlocks
        {
            get
            {
                ushort usNumBlocks = 0;

                if (null != Table2409 && null != Table2409.Set1ActualLimits)
                {
                    usNumBlocks = Table2409.Set1ActualLimits.NumberOfBlocks;
                }

                return usNumBlocks;
            }
        }

        /// <summary>
        /// Gets the interval length for Extended Load Profile
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/04/12 RCG 2.53.24        Created

        public byte ExtendedLoadProfileIntervalLength
        {
            get
            {
                byte IntervalLength = 0;

                if (Table2409 != null && Table2409.Set1ActualLimits != null)
                {
                    IntervalLength = Table2409.Set1ActualLimits.IntervalLength;
                }

                return IntervalLength;
            }
        }

        /// <summary>
        /// Gets the interval length for Extended Load Profile
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/18/12 jrf 2.53.55 196403 Created

        public byte ExtendedLoadProfileNumberOfChannels
        {
            get
            {
                byte byNumberOfChannels = 0;

                if (Table2409 != null && Table2409.Set1ActualLimits != null)
                {
                    byNumberOfChannels = Table2409.Set1ActualLimits.NumberOfChannels;
                }

                return byNumberOfChannels;
            }
        }

        /// <summary>
        /// Gets the maximum number of Extended Load Profile blocks that may be stored in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/04/12 RCG 2.53.24        Created

        public ushort ExtendedLoadProfileMaxNumberOfBlocks
        {
            get
            {
                ushort NumberOfBlocks = 0;

                if (Table2409 != null && Table2409.Set1ActualLimits != null)
                {
                    NumberOfBlocks = Table2409.Set1ActualLimits.NumberOfBlocks;
                }

                return NumberOfBlocks;
            }
        }

        /// <summary>
        /// Gets the maximum number of intervals in a block of Extended Load Profile data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/04/12 RCG 2.53.24        Created

        public ushort ExtendedLoadProfileIntervalsPerBlock
        {
            get
            {
                ushort IntervalsPerBlock = 0;

                if (Table2409 != null && Table2409.Set1ActualLimits != null)
                {
                    IntervalsPerBlock = Table2409.Set1ActualLimits.IntervalsPerBlock;
                }

                return IntervalsPerBlock;
            }
        }

        #endregion

        #region Instrumentation Profile

        /// <summary>
        /// Gets a boolean that represents whether or not Instrumentation Profile is running.
        /// </summary>
        /// <exception cref="PSEMException">Thrown when a communication error occurs.</exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/03/12 jrf 2.53.53 TREQ2891 Created for unit testing bug fix.
        //
        public bool IPRunning
        {
            get
            {
                object objValue;
                PSEMResponse Result;
                bool m_bRunning = false;

                if (true == InstrumentationProfileSupported)
                {
                    Result = m_lidRetriever.RetrieveLID(m_LID.IP_RUNNING, out objValue);

                    if (Result != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Instrumentation Profile running flag.");
                    }
                    else
                    {
                        // The data returned should be 1 byte
                        m_bRunning = (byte)objValue == (byte)1;
                    }
                }

                return m_bRunning;
            }
        }

        /// <summary>
        /// Gets whether the meter supports instrumentation profile.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 185305 Created
        //
        public bool InstrumentationProfileSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(2409) && true == Table00.IsTableUsed(2410)
                    && true == Table00.IsTableUsed(2411) && true == Table00.IsTableUsed(2413)
                    && true == Table00.IsTableUsed(2417));

                return blnSupported;
            }
        }

        /// <summary>
        /// The number of valid extended load profile blocks in mfg. LP set 2.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/04/12 jrf 2.53.24 185305 Created.
        // 01/18/12 RCG 2.53.31        Renaming for consistency
        // 04/03/12 jrf 2.53.53 TREQ2891 Unit testing bug fix.
        //
        public ushort InstrumentationProfileCurrentNumberOfBlocks
        {
            get
            {
                ushort usNumBlocks = 0;

                if (null != Table2409 && null != Table2409.Set2ActualLimits)
                {
                    usNumBlocks = Table2409.Set2ActualLimits.NumberOfBlocks;
                }

                return usNumBlocks;
            }
        }

        /// <summary>
        /// The number of valid extended load profile channels in mfg. LP set 2.
        /// This value is a more reliable indication of whether Inst. Profile is
        /// running.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/23/13 AF  3.00.22 WR426017 Created
        //
        public byte InstrumentationProfileCurrentNumberOfChannels
        {
            get
            {
                byte usNumChannels = 0;

                if (null != Table2409 && null != Table2409.Set2ActualLimits)
                {
                    usNumChannels = Table2409.Set2ActualLimits.NumberOfChannels;
                }

                return usNumChannels;
            }
        }

        /// <summary>
        /// Gets the interval length for Instrumentation Profile
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/04/12 RCG 2.53.24        Created

        public byte InstrumentationProfileIntervalLength
        {
            get
            {
                byte IntervalLength = 0;

                if (Table2409 != null && Table2409.Set2ActualLimits != null)
                {
                    IntervalLength = Table2409.Set2ActualLimits.IntervalLength;
                }

                return IntervalLength;
            }
        }

        /// <summary>
        /// Gets the maximum number of Instrumentation Profile blocks that may be stored in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/04/12 RCG 2.53.24        Created

        public ushort InstrumentationProfileMaxNumberOfBlocks
        {
            get
            {
                ushort NumberOfBlocks = 0;

                if (Table2409 != null && Table2409.Set2ActualLimits != null)
                {
                    NumberOfBlocks = Table2409.Set2ActualLimits.NumberOfBlocks;
                }

                return NumberOfBlocks;
            }
        }

        /// <summary>
        /// Gets the maximum number of intervals in a block of Instrumentation Profile data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/04/12 RCG 2.53.24        Created

        public ushort InstrumentationProfileIntervalsPerBlock
        {
            get
            {
                ushort IntervalsPerBlock = 0;

                if (Table2409 != null && Table2409.Set2ActualLimits != null)
                {
                    IntervalsPerBlock = Table2409.Set2ActualLimits.IntervalsPerBlock;
                }

                return IntervalsPerBlock;
            }
        }

        /// <summary>
        /// Gets Which IP Channels are active (true = active)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/12 MSC 2.53.36         Created
        public virtual bool[] IPChannels
        {
            get
            {
                bool[] blnEnabled = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

                if (Table2417 != null)
                {
                    blnEnabled = Table2417.ChannelsUsed;
                }

                return blnEnabled;
            }
        }

        /// <summary>
        /// Number of Channels being used in IP
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/12 MSC 2.53.36         Created
        public virtual uint IPNumberOfChannels
        {
            get
            {
                uint numberOfChannels = 0;

                if (Table2417 != null)
                {
                    numberOfChannels = Table2417.NumberOfChannels;
                }

                return numberOfChannels;
            }
        }

        /// <summary>
        /// What Phase(s) are being monitored.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/12 MSC 2.53.36         Created
        public virtual IPPhases IPPhaseMonitored
        {
            get
            {
                IPPhases monitored = IPPhases.Invalid;

                if (Table2417 != null)
                {
                    monitored = Table2417.PhasesUsed;
                }

                return monitored;
            }
        }

        #endregion

        #region Max Demands

        /// <summary>
        /// Gets whether the meter supports storing 12 max W r demands.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 jrf 2.53.54 196600 Created
        //  02/23/16 AF  4.50.232 WR236192 Return false if the meter is not configured to store Max W d
        //  06/27/16 AF  4.50.291 WR696786 Disable reading the table if it has invalid values.
        //  06/28/16 AF  4.50.292 WR696786 The previous check did not work. Changed to a check for IsNaN, IsInfinity, 
        //                                 or larger than MaxValue
        //
        public bool TwelveMaxDemandsSupported
        {
            get
            {
                bool blnSupported = Table00.IsTableUsed(2175);

                if (blnSupported)
                {
                    int? MaxWdIndex = FindDemandSelectionIndex(m_LID.DEMAND_MAX_W_DEL);

                    if (null == MaxWdIndex)
                    {
                        blnSupported = false;
                    }
                    else
                    {
                        AMIMDERCD[] Records = MaxDemands;

                        foreach (AMIMDERCD rcd in Records)
                        {
                            // A couple of meters have shown 0xFFFFFFFF for the max demand values
                            // The CE dlls can't handle it and it means the table is messed up
                            if (float.IsNaN(rcd.MaxWattsReceived) || float.IsInfinity(rcd.MaxWattsReceived) || (rcd.MaxWattsReceived > float.MaxValue))
                            {
                                blnSupported = false;
                                break;
                            }
                        }
                    }
                }

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets the array of max watts demand records from the meter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/08/11 MSC 2.53.16         Created
        // 05/18/16 AF  4.50.269 236165 Mark the table as unloaded to make sure it is read again 
        public virtual AMIMDERCD[] MaxDemands
        {
            get
            {
                AMIMDERCD[] result = null;

                if (Table2175 != null)
                {
                    Table2175.State = AnsiTable.TableState.Unloaded;
                    result = Table2175.AMIMDERCDs;
                }

                return result;
            }
        }

        #endregion

        #region Swap Out

        /// <summary>
        /// Gets whether or not the SwapOut table is present in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool IsSwapOutTablePresent
        {
            get
            {
                return Table2168 != null;
            }
        }

        /// <summary>
        /// Gets the meter serial number for the swapped out meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public string SwapOutMeterSerialNumber
        {
            get
            {
                if (Table2168 != null)
                {
                    return Table2168.MeterSerialNumber;
                }
                else
                {
                    throw new InvalidOperationException("Meter Swap Out is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the customer serial number for the swapped out meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public string SwapOutCustomerSerialNumber
        {
            get
            {
                if (Table2168 != null)
                {
                    return Table2168.CustomerSerialNumber;
                }
                else
                {
                    throw new InvalidOperationException("Meter Swap Out is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the Watt hours energy for the swapped out meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public double SwapOutWattHours
        {
            get
            {
                if (Table2168 != null)
                {
                    return Table2168.WattHours;
                }
                else
                {
                    throw new InvalidOperationException("Meter Swap Out is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the Watts demand for the swapped out meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public float SwapOutWattDemand
        {
            get
            {
                if (Table2168 != null)
                {
                    return Table2168.WattDemand;
                }
                else
                {
                    throw new InvalidOperationException("Meter Swap Out is not supported in this meter.");
                }
            }
        }

        /// <summary>
        /// Gets the time the meter was swapped out.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public DateTime SwapOutTime
        {
            get
            {
                if (Table2168 != null)
                {
                    return Table2168.SwapOutTime;
                }
                else
                {
                    throw new InvalidOperationException("Meter Swap Out is not supported in this meter.");
                }
            }
        }

        #endregion

        #region Test Mode

        /// <summary>
        /// Property to retrieve the Number of Minutes the meter is on test mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/09 MMD    		Created 
        //  07/02/10 AF  2.42.01        Made virtual for M2 Gateway override

        public virtual float TimeRemainingInTestMode
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                uint m_MinutesOnTestMode = 0;
                float val;
                object Data = null;

                Result = m_lidRetriever.RetrieveLID(m_LID.TIME_REMAINING_TEST_MODE, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    //Convert the data to seconds
                    m_MinutesOnTestMode = (uint)Data;
                    val = m_MinutesOnTestMode / (float)60;
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the number of minutes left in test mode"));
                }

                return (m_MinutesOnTestMode / (float)60); // to convert the seconds into minutes
            }
        }

        /// <summary>
        /// Property to retrieve the configured/default pulse weight in test mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/25/09 MMD    		    Created
        //  07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        //  09/24/14 AF  4.00.60 423986 Pulse weight should be shown as is and not multiplied by 10 and divided by 40
        //  01/19/15 AF  4.00.92 535491 Renamed per code review to make it more clear what this returns
        //
        public virtual float GetDefaultPulseWeightTest
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                float m_RealPulseWeight = 0.0F;
                object Data = null;

                Result = m_lidRetriever.RetrieveLID(m_LID.REAL_COEFF_KT, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    //Convert the data to a float
                    m_RealPulseWeight = (float)Data;
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the real pulse weight in test mode"));
                }

                return m_RealPulseWeight;
            }
        }

        #endregion

        #region Comm Modules

        /// <summary>
        /// Boolean that indicates if an Itron Communication Module is present in the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 jrf 2.20.19 137693 Created.
        // 09/16/09 AF  2.30.01        Changed the Compare method's comparison
        //                             type to quiet a compiler warning
        // 01/14/10 jrf 2.40.06        Adding code to catch exception thrown when meters 
        //                             with no comm module are queried and fail.
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 01/17/12 AF  2.53.31 183921 Added CiscoCommModule to check for supported comm modules
        // 04/05/13 AF  2.80.17 TR7578 Added support for I-210+c
        // 12/12/13 AF  3.50.14 TQ7644 Promoted the property to CANSIDevice and changed this to override
        //
        public override bool ItronCommModulePresent
        {
            get
            {
                bool bResult = false;

                try
                {
                    // A Communcation Module is always present in Firmware prior to 1.8
                    if (FWRevision < VERSION_1_8_HARDWARE_2_0)
                    {
                        bResult = true;
                    }
                    else
                    {
                        bResult = CommModule != null && (CommModule is RFLANCommModule || CommModule is PLANCommModule || CommModule is CiscoCommModule || CommModule is ICSCommModule);
                    }
                }
                catch
                {
                    bResult = false;
                }

                return bResult;
            }
        }

        /// <summary>
        /// Gets whether or not the meter supports a dual stack.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/13 jrf 2.80.26 TQ8278 Created
        //
        public bool DualStackSupported
        {
            get
            {
                bool blnDualStackSupported = IsTableUsed(2640);

                //Dual stack is not applicable to cellular meters even though table is supported.
                if (CommModule is ICSCommModule)
                {
                    blnDualStackSupported = false;
                }

                return blnDualStackSupported;
            }
        }

        /// <summary>
        /// Gets the current stack running in the register
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //  02/04/13 jkw 2.70.63 312343 Mfg table 612 (dual stack switch state table) was renumbered to be 592
        //
        public STACK_TYPE CurrentStack
        {
            get
            {
                STACK_TYPE CurrStack = STACK_TYPE.Invalid;

                if (Table2640 != null)
                {
                    CurrStack = Table2640.CurrentStackType;
                }

                return CurrStack;
            }
        }

        /// <summary>
        /// Gets the time of the last stack switch
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //  02/04/13 jkw 2.70.63 312343 Mfg table 612 (dual stack switch state table) was renumbered to be 592
        //
        public DateTime? LastStackSwitchTime
        {
            get
            {
                DateTime? SwitchTime = null;

                if (Table2640 != null)
                {
                    SwitchTime = Table2640.LastSwitchTime;
                }

                return SwitchTime;
            }
        }

        /// <summary>
        /// Gets the number of stack switches that have occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //  02/04/13 jkw 2.70.63 312343 Mfg table 612 (dual stack switch state table) was renumbered to be 592
        //
        public UInt32 StackSwitchCount
        {
            get
            {
                UInt32 SwitchCount = 0;

                if (Table2640 != null)
                {
                    SwitchCount = Table2640.StackSwitchCount;
                }

                return SwitchCount;
            }
        }

        /// <summary>
        /// Gets whether the meter supports LAN Log Data Table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/08/13 jrf 2.70.66 288156 Created
        //
        public bool LANLogsSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(2162));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets the minimum number of seconds difference the comm module time and 
        /// register time can be different for an automatic network time adjustment
        /// to occur.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        public byte? MinimumTimeAdjustmentSeconds
        {
            get
            {
                byte? byMinSecs = null;

                if (Table2191 != null)
                {
                    byMinSecs = Table2191.MinDeltaSeconds;
                }

                return byMinSecs;
            }
        }

        /// <summary>
        /// Gets the maximum number of seconds difference the comm module time and 
        /// register time can be different for an automatic network time adjustment
        /// to occur.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        public byte? MaximumTimeAdjustmentSeconds
        {
            get
            {
                byte? byMaxSecs = null;

                if (Table2191 != null)
                {
                    byMaxSecs = Table2191.MaxDeltaSeconds;
                }

                return byMaxSecs;
            }
        }

        /// <summary>
        ///  Gets/Sets whether or not the comm module time polling failure threshold value is 
        /// stored as hours (True) or minutes (False).
        /// </summary>
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/07/13 jrf 2.85.12 TC 12653  Created.
        //
        public bool? CommModuleTimePollFailThresholdInHours
        {
            get
            {
                bool? blnUseHours = null;

                if (Table2191 != null)
                {
                    blnUseHours = Table2191.TimePollFailThresholdUseHours;
                }

                return blnUseHours;
            }
            set
            {
                if (Table2191 != null && value != null)
                {
                    Table2191.TimePollFailThresholdUseHours = value.Value;
                }
            }
        }

        /// <summary>
        ///  Gets/Sets the frequency (in Minutes) the register will attempt to reread 
        ///  the time from the comm module.
        /// </summary>
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/07/13 jrf 2.85.12 TC 12653  Created.
        //
        public byte? CommModuleTimePollingPeriod
        {
            get
            {
                byte? byPeriod = null;

                if (Table2191 != null)
                {
                    byPeriod = Table2191.PollPeriodMinutes;
                }

                return byPeriod;
            }
            set
            {
                if (Table2191 != null && value != null)
                {
                    Table2191.PollPeriodMinutes = value.Value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets the amount of time after which a time adjustment failed event will be recorded
        /// if the comm module time cannot be successfully aquired. Stored as minutes or hours based
        /// on value of CommModuleTimePollFailThresholdInHours.
        /// </summary>
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/07/13 jrf 2.85.12 TC 12653  Created.
        //
        public byte? CommModuleTimePollingFailureThreshold
        {
            get
            {
                byte? byThreshold = null;

                if (Table2191 != null)
                {
                    byThreshold = Table2191.PollFailThresholdMinsHours;
                }

                return byThreshold;
            }
            set
            {
                if (Table2191 != null && value != null)
                {
                    Table2191.PollFailThresholdMinsHours = value.Value;
                }
            }
        }

        #endregion

        #region Load Control

        /// <summary>
        /// Determines if User Intervention is required after a load limiting disconnect
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/20/07 KRC 8.10.14        Adding Load Limiting summary support
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 		
        public virtual string LoadLimitingConnectWithoutUserIntervetion
        {
            get
            {
                string strResult = "";

                // if (FWRevision > VERSION_1_RELEASE_1)
                if (VersionChecker.CompareTo(FWRevision, VERSION_1_RELEASE_1) > 0)
                {
                    // We are in a version of firmware that supports this.
                    // This item is bit 7 of the Demand Type.
                    int iDemandControl = Table2048.DemandConfig.DemandControlByte;

                    strResult = TranslateLoadLimitingConnectWithoutUserIntervetion(iDemandControl);
                }

                return strResult;
            }
        }

        /// <summary>
        /// Determines if Load Control is enabled and what the Threshold is if it is enabled
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/20/07 KRC 8.10.15        Adding Load Limiting summary support
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 	
        public virtual string LoadControlDisconnectThreshold
        {
            get
            {
                string strResult = "";

                // if (FWRevision > VERSION_1_RELEASE_1)
                if (VersionChecker.CompareTo(FWRevision, VERSION_1_RELEASE_1) > 0)
                {
                    // We are in a version of firmware that supports this.
                    // This item is bit 7 of the Demand Type.
                    Single fLoadControlThreshold = Table2048.DemandConfig.DemandThreshold_1_Level;

                    strResult = TranslateLoadControlDisconnectThreshold(fLoadControlThreshold);
                }
                return strResult;
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

        #endregion

        #region Billing Schedules

        /// <summary>
        /// Provides a list of DateTime objects representing the 
        /// billing schedule dates configured in the meter.  If no 
        /// dates are configured, the list will be empty.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/24/06 AF  7.40.00 N/A    Created
        //
        public List<DateTime> BillingScheduleDates
        {
            get
            {
                List<DateTime> lstScheduleDates = new List<DateTime>();
                DateTime dtDate;

                ushort[] ausDates =
                    new ushort[Table2048.BillingSchedConfig.ScheduleLength];

                ausDates = Table2048.BillingSchedConfig.ScheduleDates;

                //Transform the data into date time values
                for (int intIndex = 0; intIndex < ausDates.Length; intIndex++)
                {
                    if (BillingSchedConfig.END_OF_CUST_SCHED != ausDates[intIndex])
                    {
                        dtDate = (base.MeterReferenceTime).AddDays(ausDates[intIndex]);
                        lstScheduleDates.Add(dtDate);
                    }
                    else
                    {
                        break;
                    }
                }

                return lstScheduleDates;
            }
        }

        #endregion

        #region Firmware Downloads

        /// <summary>
        /// Status of the firmware download to HAN devices via the CENTRON
        /// OpenWay meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/05/09 AF  2.20.20        Added code to protect against trying to read
        //                              a non-existant table
        //
        public CENTRON_AMI.HAN_FW_DL_STATUS[] HANFWDLStatuses
        {
            get
            {
                if (Table00.IsTableUsed(2135))
                {
                    return Table2135.HANFwDlStatuses;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the percentage completion of the active transfer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/09 AF  2.20.02        Created
        //
        public uint HANFWDLPercentComplete
        {
            get
            {
                //Make sure we don't divide by zero
                if (0 != Table2135.TotalPagesActiveTransfer)
                {
                    return (uint)((float)(Table2135.LastPageSentActiveTransfer) / ((float)Table2135.TotalPagesActiveTransfer) * 100.0F);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the Last page sent of the active transfer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/09 MMD  2.20.02        Created
        //
        public uint LastPageSentActiveTransfer
        {
            get
            {
                if (0 != Table2135.LastPageSentActiveTransfer)
                {
                    return (uint)((float)(Table2135.LastPageSentActiveTransfer));
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the total pages sent of the active transfer.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/10/10 jrf  2.40.12        Created
        //
        public uint TotalPagesActiveTransfer
        {
            get
            {
                return Table2135.TotalPagesActiveTransfer;
            }
        }

        /// <summary>
        /// Gets the version and revision of the HAN device firmware file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/10/10 jrf 2.40.12        Created
        //
        public string TransferableImageVersion
        {
            get
            {
                return Table2135.TransferableImageVersion;
            }
        }

        ///<summary>
        /// Gets the number of firmware blocks currently downloaded from the C1222 debug table.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/10 jrf 2.40.12        Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual ushort C1222DebugFWDLBlockCount
        {
            get
            {
                return Table2114C1222DebugInfo.FWDLBlockCount;
            }
        }

        /// <summary>
        /// Gets the number total number of firmware blocks to download.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/10 jrf 2.40.12        Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual int TotalFWDLBlockCount
        {
            get
            {
                return Table2114C1222DebugInfo.TotalFWDLBlockCount;
            }
        }

        /// <summary>
        /// Gets whether or not activation is occurring now.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/10 jrf 2.40.12        Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool IsActivateInProgress
        {
            get
            {
                return Table2114C1222DebugInfo.IsActivateInProgress;
            }
        }

        /// <summary>
        /// Gets whether or not firmware download is enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/10 jrf 2.40.12        Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual bool IsFWDLEnabled
        {
            get
            {
                return Table2114C1222DebugInfo.FWDLEnabled;
            }
        }

        /// <summary>
        /// Gets the firmware status table's number of blocks received.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/10 jrf 2.40.12        Created
        //
        public ushort FWStatusBlocksRecieved
        {
            get
            {
                return Table2180.BlocksReceived;
            }
        }

        /// <summary>
        /// Gets the firmware status table's format of its status.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/10 jrf 2.40.12        Created
        //
        public StatusFormat FormatofFirmwareStatus
        {
            get
            {
                return Table2180.FormatOfStatus;
            }
        }

        /// <summary>
        /// Gets the list of Firware Download Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/15/11 MMD                Created
        //  11/24/15 PGH 4.50.218 REQ574469 Filter seal/unseal events

        public ReadOnlyCollection<FWDownloadLogEvent> FirmwareDownloadEvents
        {
            get
            {
                ReadOnlyCollection<FWDownloadLogEvent> Events = null;

                if (Table2382 != null)
                {
                    Events = Table2382.Events.Where(o =>
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.AutoSealMeter &&
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.SealMeter &&
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.UnsealMeter).ToList().AsReadOnly();
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets whether the firmware download event log is supported in the meter
        /// by checking Table 00 to see if the tables are used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 AF  2.51.33        Created
        //  08/22/11 AF  2.52.06        Make sure that all three Hydrogen C tables are present
        //
        public virtual bool FWDLLogSupported
        {
            get
            {
                return (Table00.IsTableUsed(2379) && Table00.IsTableUsed(2382) && Table00.IsTableUsed(2383));
            }
        }


        /// <summary>
        /// Gets whether the temperature log is supported in the meter
        /// by checking Table 00 to see if the tables are used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/16 PGH 4.50.225 RTT556309 Created
        //
        public virtual bool TemperatureLogSupported
        {
            get
            {
                return (Table00.IsTableUsed(2425) && Table00.IsTableUsed(2426) && Table00.IsTableUsed(2427));
            }
        }

        /// <summary>
        /// Gets whether the FWDL CRCs are supported in the meter by checking
        /// Table 00 to see if the tables are used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/18/11 AF  2.52.05        Created
        //  08/22/11 AF  2.52.06        Make sure that all three Hydrogen C tables are present
        //
        public bool FWDLCRCSupported
        {
            get
            {
                return (Table00.IsTableUsed(2379) && Table00.IsTableUsed(2382) && Table00.IsTableUsed(2383));
            }
        }

        /// <summary>
        /// Gets the Firmware Download Events from the firmware download event log.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/15/11 jrf 2.52.03 TC4241 Created
        // 11/24/15 PGH 4.50.218 REQ574469 Filter seal/unseal events
        //
        public ReadOnlyCollection<FWDownloadLogEvent> FWDLEvents
        {
            get
            {
                ReadOnlyCollection<FWDownloadLogEvent> FWDLEvents = null;

                if (null != Table2382)
                {
                    FWDLEvents = Table2382.Events.Where(o =>
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.AutoSealMeter &&
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.SealMeter &&
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.UnsealMeter).ToList().AsReadOnly();
                }

                return FWDLEvents;
            }
        }

        /// <summary>
        /// Gets the Seal/Unseal Events from the firmware download event log.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/24/15 PGH 4.50.218 REQ574469 Created
        //
        public ReadOnlyCollection<FWDownloadLogEvent> SealUnsealEvents
        {
            get
            {
                ReadOnlyCollection<FWDownloadLogEvent> SealUnsealEvents = null;

                if (null != Table2382)
                {
                    SealUnsealEvents = Table2382.Events.Where(o =>
                        o.EventID == (ushort)FWDownloadLogEvent.FWDownloadLogEventID.AutoSealMeter ||
                        o.EventID == (ushort)FWDownloadLogEvent.FWDownloadLogEventID.SealMeter ||
                        o.EventID == (ushort)FWDownloadLogEvent.FWDownloadLogEventID.UnsealMeter).ToList().AsReadOnly();
                }

                return SealUnsealEvents;
            }
        }

        /// <summary>
        /// Reads the number of firmware download events from the fixed fwdl event log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/17/11 AF  2.52.04        Created
        //
        public ushort NumberOfFWDLEvents
        {
            get
            {
                ushort numberOfEvents = 0;

                if (null != Table2382)
                {
                    numberOfEvents = Table2382.NumberOfValidEntries;
                }

                return numberOfEvents;
            }
        }

        /// <summary>
        /// Reads the number of seal/unseal event entries remaining from the fixed fwdl event log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#     Description
        //  -------- --- ------- ---------- -------------
        //  11/30/15 PGH 4.50.218 REQ574469 Created
        //  01/20/16 PGH 4.50.224 REQ574469 Convert hex to int
        //  03/03/16 PGH 4.50.233 REQ574469 Different offset depending on device type
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        public int NumberOfSealUnsealEventsRemaining
        {
            get
            {
                int NumberOfEntriesRemaining = 0;

                if (null != Table2084)
                {
                    string NumRemaining = "";
                    if (IsPolyphaseMeter)
                    {
                        NumRemaining = Table2084.NumberOfSealUnsealEntriesRemainingForPolyPhaseMeter.ToString("X2");
                        NumberOfEntriesRemaining = Convert.ToInt32(NumRemaining, 16);
                    }
                    else
                    {
                        NumRemaining = Table2084.NumberOfSealUnsealEntriesRemainingForSinglePhaseMeter.ToString("X2");
                        NumberOfEntriesRemaining = Convert.ToInt32(NumRemaining, 16);
                    }
                }

                return NumberOfEntriesRemaining;
            }
        }

        /// <summary>
        /// Reads the maximum number of firmware download events that he fixed FWDL event log will hold.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/17/11 AF  4.50.127 587865 Created
        public ushort FWDLEventLogSize
        {
            get
            {
                ushort LogSize = 0;

                if (null != Table2379)
                {
                    LogSize = Table2379.FWDownloadEntryCount;
                }

                return LogSize;
            }
        }

        /// <summary>
        /// Gets the register application's CRC
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/18/11 jrf 2.52.04 TC4241 Created
        //
        public UInt32 RegisterApplicationCRC
        {
            get
            {
                UInt32 uiCRC = 0;

                if (null != Table2383)
                {
                    uiCRC = Table2383.ApplicationCRC;
                }

                return uiCRC;
            }
        }

        /// <summary>
        /// Gets the register bootloader's CRC
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/18/11 jrf 2.52.04 TC4241 Created
        //
        public UInt32 RegisterBootloaderCRC
        {
            get
            {
                UInt32 uiCRC = 0;

                if (null != Table2383)
                {
                    uiCRC = Table2383.RegisterBootLoaderCRC;
                }

                return uiCRC;
            }
        }

        /// <summary>
        /// Gets the extended firmware download status table's current CRC state
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/12 JKW 2.70.xx        Created
        //
        public CRC_STATE CRCState
        {
            get
            {
                CRC_STATE state = CRC_STATE.Invalid;

                if (null != Table2182)
                {
                    state = Table2182.CurrentCRCState;
                }

                return state;
            }
        }

        /// <summary>
        /// Gets the extended firmware download status table's supplied CRC value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/12 JKW 2.70.xx        Created
        //
        public ushort SuppliedCRCValue
        {
            get
            {
                ushort suppliedCRCValue = 0;

                if (null != Table2182)
                {
                    suppliedCRCValue = Table2182.SuppliedCRCValue;
                }

                return suppliedCRCValue;
            }
        }

        /// <summary>
        /// Gets the extended firmware download status table's calculated CRC value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/12 JKW 2.70.xx        Created
        //
        public ushort CalculatedCRCValue
        {
            get
            {
                ushort calculatedCRCValue = 0;

                if (null != Table2182)
                {
                    calculatedCRCValue = Table2182.CalculatedCRCValue;
                }

                return calculatedCRCValue;
            }
        }

        /// <summary>
        /// Gets the extended firmware download status table's activation state
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/12 JKW 2.70.xx        Created
        //
        public ACTIVATION_STATE ActivationState
        {
            get
            {
                ACTIVATION_STATE activationState = ACTIVATION_STATE.Invalid;

                if (null != Table2182)
                {
                    activationState = Table2182.ActivationState;
                }

                return activationState;
            }
        }

        /// <summary>
        /// Gets the extended firmware download status table's activation time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/12 JKW 2.70.xx        Created
        //
        public DateTime ActivationTime
        {
            get
            {
                DateTime activationTime = DateTime.MinValue;

                if (null != Table2182)
                {
                    activationTime = Table2182.ActivationTime;
                }

                return activationTime;
            }
        }

        #endregion

        #region KYZ (I/O)

        /// <summary>
        /// Gets the IO configuration.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/22/09 jrf 2.20.02 N/A    Created
        //
        public KYZData IOData
        {
            get
            {
                CTable2048_OpenWay OW2048 = Table2048 as CTable2048_OpenWay;

                return OW2048.IOConfig.IOData;
            }
        }

        #endregion

        #region Task Execution Information

        /// <summary>
        /// Retrieves the Task Data from table 2270
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/24/11 MMD                 Created
        //
        public AMIHW3TASKRCD[] AMIHW3TaskExecution
        {
            get
            {
                AMIHW3TASKRCD[] m_aAMIHW3TASKRCD = null;
                if (null != Table2270)
                {
                    m_aAMIHW3TASKRCD = Table2270.AMIHW3TASKRCD;
                }

                return m_aAMIHW3TASKRCD;

            }
        }

        /// <summary>
        /// Retrieves the Free Running time from Table 2270
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/24/11 MMD                 Created
        //
        public DateTime FreeRunningTime
        {
            get
            {
                DateTime m_dtRunningTime = new DateTime();
                if (null != Table2270)
                {
                    m_dtRunningTime = Table2270.FreeRunningTime;
                }

                return m_dtRunningTime;

            }
        }

        #endregion

        #region Signed Authorization

        /// <summary>
        /// Gets the current state of Signed Authorization in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/09 RCG 2.30.09 N/A    Created

        public FeatureState? SignedAuthorizationState
        {
            get
            {
                FeatureState? CurrentState = null;

                if (Table2219 != null)
                {
                    CurrentState = Table2219.SignedAuthorizationState;
                }

                return CurrentState;
            }
        }

        /// <summary>
        /// The amount of time remaining for Signed Authorization to be disabled. This value returns null
        /// unless the Signed Authorization State is set to disabled for a time period
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/09 RCG 2.30.09 N/A    Created

        public TimeSpan? SignedAuthorizationDisabledTimeRemaining
        {
            get
            {
                TimeSpan? TimeRemaining = null;

                if (Table2219 != null && Table2219.SignedAuthorizationState == FeatureState.DisabledForPeriod)
                {
                    TimeRemaining = Table2219.SignedAuthorizationDisabledTimeRemaining;
                }

                return TimeRemaining;
            }
        }

        #endregion

        #region DST

        /// <summary>
        /// Gets the Configured DST dates
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/28/11 RCG 2.50.00        Created

        public override List<CDSTDatePair> DST
        {
            get
            {
                List<CDSTDatePair> DSTDates = null;

                // Hydrogen adds the ability to have a 25 year DST calendar which is configured
                // in 2260 but we need to ignore this config for anything prior to that.
                if (VersionChecker.CompareTo(FWRevision, VERSION_HYDROGEN_3_6) >= 0
                    && Table2260DSTConfig != null)
                {
                    DSTDates = Table2260DSTConfig.DSTDates;

                    // If the Calendar config in 2048 has dates that are not in the 25 year config we
                    // need to add those to the list of DST dates
                    foreach (CDSTDatePair Current2048DSTDate in base.DST)
                    {
                        bool bIn25YearConfig = false;

                        // Check to see if the 25 year Config has this year
                        foreach (CDSTDatePair Current25YearDSTDate in DSTDates)
                        {
                            // CompareTo checks to see if the year matches
                            if (Current2048DSTDate.CompareTo(Current25YearDSTDate) == 0)
                            {
                                bIn25YearConfig = true;
                            }
                        }

                        if (bIn25YearConfig == false)
                        {
                            // This year is not in there so lets add it.
                            DSTDates.Add(Current2048DSTDate);
                        }
                    }

                    // Make sure the dates are in order
                    DSTDates.Sort();
                }
                else
                {
                    DSTDates = base.DST;
                }

                return DSTDates;
            }
        }

        /// <summary>
        /// Gets the Configured DST dates in a generic format.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/18/12 jrf 2.60.23        Created
        //
        public List<DateTime[]> DSTDates
        {
            get
            {
                List<DateTime[]> lstDSTDates = new List<DateTime[]>();
                DateTime[] GenericDatePair = null;

                foreach (CDSTDatePair DatePair in DST)
                {
                    GenericDatePair = new DateTime[2];
                    GenericDatePair[0] = DatePair.ToDate;
                    GenericDatePair[1] = DatePair.FromDate;
                    lstDSTDates.Add(GenericDatePair);
                }

                return lstDSTDates;
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
                bool bSupported = false;

                if (Table00.IsTableUsed(2260) && VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_HYDROGEN_3_6) >= 0)
                {
                    bSupported = true;
                }

                return bSupported;
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

        #endregion

        #region Base Information

        /// <summary>
        /// Metrology Statistics - Raised by UART0 ISR when ACK received in IDLE state
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? AckReceived
        {
            get
            {
                bool? ackRcvd = null;

                if (Table2112 != null)
                {
                    ackRcvd = Table2112.ACKReceived;
                }

                return ackRcvd;
            }
        }

        /// <summary>
        ///  Metrology statistics - Raised by UART0 ISR when 42 bytes received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? MsgReceived
        {
            get
            {
                bool? msgRcvd = null;

                if (Table2112 != null)
                {
                    msgRcvd = Table2112.MessageReceived;
                }

                return msgRcvd;
            }
        }

        /// <summary>
        /// Metrology statistics - Wait for SAVE response BLURT
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? WaitResponse
        {
            get
            {
                bool? waitResponse = null;

                if (Table2112 != null)
                {
                    waitResponse = Table2112.WaitResponse;
                }

                return waitResponse;
            }
        }

        /// <summary>
        /// Gets if the blurt message indicates if it is waiting for a retry timeout.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? WaitRetryTimeOut
        {
            get
            {
                bool? waitTO = null;

                if (Table2112 != null)
                {
                    waitTO = Table2112.WaitRetryTimeOut;
                }

                return waitTO;
            }
        }

        /// <summary>
        /// Gets if the blurt message indicates if a retry timeout has occurred.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? RetryTOFlag
        {
            get
            {
                bool? retryTO = null;

                if (Table2112 != null)
                {
                    retryTO = Table2112.RetryTimeOutFlag;
                }

                return retryTO;
            }
        }

        /// <summary>
        /// Gets if the next blurt message should be skipped.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? SkipNextBlurt
        {
            get
            {
                bool? skip = null;

                if (Table2112 != null)
                {
                    skip = Table2112.SkipNextBlurt;
                }

                return skip;
            }
        }

        /// <summary>
        /// Gets whether or not the meter uses base backed values
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/05/11 RCG	2.53.13		    Created

        public bool UsesBaseBackedValues
        {
            get
            {
                bool UsesBBV = false;

                if (Table2112 != null)
                {
                    UsesBBV = Table2112.IsBasedBackedValue;
                }

                return UsesBBV;
            }
        }

        /// <summary>
        /// Gets if the blurt message indicates that clear met busy.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool ClearMetBusy
        {
            get
            {
                bool clear = false;

                if (Table2112 != null)
                {
                    clear = Table2112.ClearMetBusy;
                }

                return clear;
            }
        }

        /// <summary>
        /// Gets if the blurt message is ready.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? IsBlurtReady
        {
            get
            {
                bool? blurtReady = null;

                if (Table2112 != null)
                {
                    blurtReady = Table2112.BlurtReady;
                }

                return blurtReady;
            }
        }

        /// <summary>
        /// Gets if the blurt message's ID count is ready.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? IsBlurtIDCountReady
        {
            get
            {
                bool? msgReady = null;

                if (Table2112 != null)
                {
                    msgReady = Table2112.IDCountReady;
                }

                return msgReady;
            }
        }

        /// <summary>
        /// Gets if the blurt message contains a WDE pulse count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? WDEPulseCnt
        {
            get
            {
                bool? pulseCountPresent = null;

                if (Table2112 != null)
                {
                    pulseCountPresent = Table2112.WDEPulseCount;
                }

                return pulseCountPresent;
            }
        }

        /// <summary>
        /// Gets if the blurt message is the last in an interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? IsBlurtLastInInterval
        {
            get
            {
                bool? isLast = null;

                if (Table2112 != null)
                {
                    isLast = Table2112.EndNextInterval;
                }

                return isLast;
            }
        }

        /// <summary>
        /// Gets whether or not the blurt message is receiving the next interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public bool? IsBlurtReceivingNextInterval
        {
            get
            {
                bool? isReceiving = null;

                if (Table2112 != null)
                {
                    isReceiving = Table2112.NextIntervalReceived;
                }

                return isReceiving;
            }
        }

        /// <summary>
        /// Gets the type of blurt packet coming from the base.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //
        public BlurtMode MetrologyMode
        {
            get
            {
                BlurtMode EnergyType = BlurtMode.Unknown;

                if (null != Table2112)
                {
                    EnergyType = Table2112.MetrologyMode;
                }

                return EnergyType;
            }
        }

        /// <summary>
        /// Gets whether or not the meter's configured energies are supported by the base.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  09/25/15 jrf 4.21.05 607438 Reworked logic for determining this value.
        public bool EnergyConfigurationSupported
        {
            get
            {
                //Assuming true unless meter confirms otherwise.
                bool blnSupported = true;

                //Where to check:
                //1. If procedure 108 is supported use it
                //2. If it is HW 2.0 or less then assume supported.
                //3. Lastly look in the mfg. table 64

                if (false == m_BaseEnergyConfigurationSupported.Cached)
                {
                    if (true == IsProcedureUsed((ushort)Procedures.VALIDATE_BASE_ENERGIES))
                    {
                        StoreProcedureValidatedBaseEnergies();
                    }
                    else if (VersionChecker.CompareTo(HWRevision, HW_VERSION_3_0) < 0 && ITR1_DEVICE_CLASS == DeviceClass)
                    {
                        //Assuming HW 2.0 and less is supported
                        m_BaseEnergyConfigurationSupported.Value = true;
                    }
                    else if (null != Table2112)
                    {
                        m_BaseEnergyConfigurationSupported.Value = Table2112.ConfiguredEnergiesSupported;
                    }
                }

                //If we set it, use it.
                if (true == m_BaseEnergyConfigurationSupported.Cached)
                {
                    blnSupported = m_BaseEnergyConfigurationSupported.Value;
                }

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets the 1st energy quantity supplied by the base.
        /// </summary>
        /// <remarks>Property currently only supports single phase meters.</remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  09/25/15 jrf 4.21.05 607438 Reworked logic for determining this value.
        public virtual BaseEnergies SuppliedEnergy1
        {
            get
            {
                BaseEnergies EnergyType = BaseEnergies.Unknown;

                //Where to check:
                //1. If procedure 108 is supported use it
                //2. If it is HW 2.0 or less then assume supported.
                //3. Lastly look in the mfg. table 64

                if (false == m_BaseSuppliedEnergy1.Cached)
                {
                    if (true == IsProcedureUsed((ushort)Procedures.VALIDATE_BASE_ENERGIES))
                    {
                        StoreProcedureValidatedBaseEnergies();
                    }
                    else if (VersionChecker.CompareTo(HWRevision, HW_VERSION_3_0) < 0 && ITR1_DEVICE_CLASS == DeviceClass)
                    {
                        //Assuming HW 2.0 and less is VAh arithmetic
                        m_BaseSuppliedEnergy1.Value = BaseEnergies.VAhArithmetic;
                    }
                    else if (null != Table2112)
                    {
                        m_BaseSuppliedEnergy1.Value = Table2112.SuppliedEnergy1;
                    }
                }

                //If we set it, use it.
                if (true == m_BaseSuppliedEnergy1.Cached)
                {
                    EnergyType = m_BaseSuppliedEnergy1.Value;
                }

                return EnergyType;
            }
        }

        /// <summary>
        /// Gets the 2nd energy quantity supplied by the base.
        /// </summary>
        /// <remarks>Property currently only supports single phase meters.</remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/17/11 jrf 2.50.09        Created
        //  09/25/15 jrf 4.21.05 607438 Reworked logic for determining this value.
        public BaseEnergies SuppliedEnergy2
        {
            get
            {
                BaseEnergies EnergyType = BaseEnergies.Unknown; 

                //Where to check:
                //1. If procedure 108 is supported use it
                //2. If it is HW 2.0 or less then assume supported.
                //3. Lastly look in the mfg. table 64

                if (false == m_BaseSuppliedEnergy2.Cached)
                {
                    if (true == IsProcedureUsed((ushort)Procedures.VALIDATE_BASE_ENERGIES))
                    {
                        StoreProcedureValidatedBaseEnergies();
                    }
                    else if (VersionChecker.CompareTo(HWRevision, HW_VERSION_3_0) < 0 && ITR1_DEVICE_CLASS == DeviceClass)
                    {
                        //Assuming HW 2.0 and less is Wh
                        m_BaseSuppliedEnergy2.Value = BaseEnergies.Wh;
                    }
                    else if (null != Table2112)
                    {
                        m_BaseSuppliedEnergy2.Value = Table2112.SuppliedEnergy2;
                    }
                }

                //If we set it, use it.
                if (true == m_BaseSuppliedEnergy2.Cached)
                {
                    EnergyType = m_BaseSuppliedEnergy2.Value;
                }

                return EnergyType;
            }
        }

        /// <summary>
        /// Gets the secondary quantity supplied by the base.
        /// </summary>
        /// <remarks>Property currently only supports single phase meters.</remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/15 jrf 4.21.05 607438 Created.
        public BaseEnergies SecondaryQuantity
        {
            get
            {
                BaseEnergies SecondaryQuantity = BaseEnergies.Unknown;

                //Wh is primary so secondary quantity is the other one.
                if (SuppliedEnergy1 != BaseEnergies.Wh)
                {
                    SecondaryQuantity = SuppliedEnergy1;
                }
                else if (SuppliedEnergy2 != BaseEnergies.Wh)
                {
                    SecondaryQuantity = SuppliedEnergy2;
                }

                return SecondaryQuantity;
            }
        }

        /// <summary>
        /// Gets the name of the secondary quantity supplied by the base.
        /// </summary>
        /// <remarks>Property currently only supports single phase meters.</remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/15 jrf 4.21.05 607438 Created.
        public string SecondaryQuantityName
        {
            get
            {
                string Name = SecondaryQuantity.ToDescription();

                //Just showing var to be consistent with CE.
                if (BaseEnergies.VarhArithmetic == SecondaryQuantity
                    || BaseEnergies.VarhVectorial == SecondaryQuantity)
                {
                    Name = VARH_QUANTITY_NAME;
                }
                return Name;
            }
        }

        /// <summary>
        /// Gets whether or not the device is a single phase metering device.
        /// </summary>
        /// <remarks>Property currently only supports single phase meters.</remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/15 jrf 4.21.05 607438 Created.
        //  11/10/15 jrf 4.22.04 625338 Removing PTCRs and transparent devices from this check.
        public bool IsSinglePhaseMeter
        {
            get
            {
                bool IsMono = false;

                //pole top cell relay and transparent devices (range extenders) are not used for metering. 
                if (MeterKey_PoleTopCellRelaySupported || MeterKey_TransparentDeviceSupported)
                {
                    IsMono = false;
                }
                //Checking ITR1 is just in case there is older firmware that does not support
                //the metrology statistics table.  Otherwise, we can use that table to determine
                //if meter is single phase.
                else if (ITR1_DEVICE_CLASS == DeviceClass)
                {
                    IsMono = true;
                }
                //The met stat table check is in here to keep devices like M2 gateway from showing up.
                //Yes, the M2 is technically part of a single phase meter but we can only access the comm 
                //module and not the register's metering data so for all intents and purposes it is not 
                //a single phase meter to us.
                else if (true == IsTableUsed(METROLOGY_STATISTICS_TABLE) && false == IsPolyphaseMeter)
                {
                    IsMono = true;
                }


                return IsMono;
            }
        }

        /// <summary>
        /// Determines if the device is a polyphase meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/15 jrf 4.21.05 607438 Created.
        public bool IsPolyphaseMeter
        {
            get
            {
                bool IsPoly = false;

                //Meter currently uses table 2169 to determine if meter is polyphase.
                //Adding ITR4 and ITR3 device class checks just in case there are
                //older firmware versions that do not support this table
                if (ITR4_DEVICE_CLASS == DeviceClass || ITR3_DEVICE_CLASS == DeviceClass
                    || true == IsTableUsed(2169))
                {
                    IsPoly = true;
                }
                
                return IsPoly; 
            }
        }
        

        /// <summary>
        /// Gets the blurt good message count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? GoodBlurtMessageCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.GoodCount;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the number of blurt save attempts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? SaveAttempts
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.SaveAttempts;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt ack received count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? AckReceivedCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.ACKReceivedCount;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt response received count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? ResponseReceivedCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.ResponseReceivedCount;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the number of failed saves coming from the base.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/21/11 jrf 2.50.10        Created
        //
        public uint? FailedSaves
        {
            get
            {
                uint? uiFailedSaves = null;

                if (null != Table2112)
                {
                    uiFailedSaves = Table2112.FailedSaves;
                }

                return uiFailedSaves;
            }
        }

        /// <summary>
        /// Gets the blurt retry count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? RetryCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.RetryCount;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt no time count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? NoTimeCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.NoTimeCount;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt kill retries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? KillRetries
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.KillRetries;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt minimum time remaining
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? MinimumTimeRemaining
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.MinimumTimeRemaining;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt false temperature count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? FalseTemperatureCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.FalseTemperature;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt false energy count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? FalseEnergyCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.FalseEnergy;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt checksum failed count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? ChecksumFailedCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.ChecksumFailed;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt header fail count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? HeaderFailedCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.HeaderFailed;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the blurt received overruns count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? ReceivedOverrunsCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.ReceivedOverruns;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the UART ISR Rx Received count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? IsrRxMessagesReceived
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.IsrRxReceived;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the UART ISR Rx error count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? IsrRxErrorCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.IsrRxErrorCount;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the number of ISR type F errors coming from the base.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/21/11 jrf 2.50.10        Created
        //
        public uint? ISRTypeFErrors
        {
            get
            {
                uint? uiISRTypeFErrors = null;

                if (null != Table2112)
                {
                    uiISRTypeFErrors = Table2112.IsrErrorTypeF;
                }

                return uiISRTypeFErrors;
            }
        }

        /// <summary>
        /// Gets the number of ISR type O errors coming from the base.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/21/11 jrf 2.50.10        Created
        //
        public uint? ISRTypeOErrors
        {
            get
            {
                uint? uiISRTypeOErrors = null;

                if (null != Table2112)
                {
                    uiISRTypeOErrors = Table2112.IsrErrorTypeO;
                }

                return uiISRTypeOErrors;
            }
        }

        /// <summary>
        /// Gets the number of parity errors coming from the base.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/21/11 jrf 2.50.10        Created
        //
        public uint? ParityErrors
        {
            get
            {
                uint? uiParityErrors = null;

                if (null != Table2112)
                {
                    uiParityErrors = Table2112.ParityErrors;
                }

                return uiParityErrors;
            }
        }

        /// <summary>
        /// Gets the UART ABT error count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public uint? ABTErrorCount
        {
            get
            {
                uint? count = null;

                if (Table2112 != null)
                {
                    count = Table2112.ABTErrors;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the time of last update
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/12 AF  2.70.36        Created
        //
        public DateTime? TimeOfLastBlurtUpdate
        {
            get
            {
                DateTime? dtUpdateTime = null;

                if (Table2112 != null)
                {
                    dtUpdateTime = Table2112.TimeOfLastUpdate;
                }

                return dtUpdateTime;
            }
        }

        /// <summary>
        /// Gets the metrology data coming from the base
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/11 AF  2.51.14        Created
        //
        public OpenWayMFGTable2112MetData MetrologyData
        {
            get
            {
                OpenWayMFGTable2112MetData MetData = null;

                if (null != Table2112)
                {
                    MetData = Table2112.MetrologyData;
                }

                return MetData;
            }
        }

        /// <summary>
        /// Gets whether or not the meter supports configuring the base energies
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/21/12 AF  2.70.06 202030 Created
        //  10/16/14 jrf 4.00.74 539220 Making sure 4G LTE meters are included also.
        public bool ConfigureBaseEnergiesSupported
        {
            get
            {
                bool shouldConfigure = false;

                // Only configure for HW 3.1 ITRD, ITRE, ITRF, ITRJ and ITRK
                if (IsProcedureUsed((ushort)Procedures.CONFIGURE_BASE_ENERGIES) && IsProcedureUsed((ushort)Procedures.VALIDATE_BASE_ENERGIES))
                {
                    if ((VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_3_6) == 0) || (VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_3_8) == 0)
                        || (VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_3_61) == 0) || (VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_3_81) == 0))
                    {
                        shouldConfigure = true;
                    }
                }

                return shouldConfigure;
            }
        }

        #endregion

        #region Power Monitoring

        /// <summary>
        /// Gets the extended voltage monitoring phases monitored by the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5306 Created
        //
        public MonitorPhases PhasesMonitored
        {
            get
            {
                MonitorPhases Phases = MonitorPhases.Unknown;

                if (null != Table2156)
                {
                    Phases = Table2156.PhasesMonitored;
                }

                return Phases;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Watts Delivered quantity from the power monitoring tables tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  06/08/11 jrf 2.51.10 173353 Created
        //
        public InstantaneousQuantity InstantaneousWattsDelivered
        {
            get
            {
                InstantaneousQuantity InsWd = null;

                if (null != Table2370)
                {
                    InsWd = Table2370.InstantaneousWattsDelivered;
                }
                return InsWd;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Watts Received quantity from the power monitoring tables tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  06/08/11 jrf 2.51.10 173353 Created
        //
        public InstantaneousQuantity InstantaneousWattsReceived
        {
            get
            {
                InstantaneousQuantity InsWr = null;

                if (null != Table2370)
                {
                    InsWr = Table2370.InstantaneousWattsReceived;
                }
                return InsWr;
            }
        }

        /// <summary>
        /// Gets whether power monitoring is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  06/08/11 jrf 2.51.10 173353 Created
        //
        public bool? PowerMonitoringEnabled
        {
            get
            {
                bool? blnEnabled = null;

                if (null != Table2369)
                {
                    blnEnabled = Table2369.PowerMonitoringEnabled;
                }

                return blnEnabled;
            }
        }

        /// <summary>
        /// Gets number of seconds before power monitoring begins after power up.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  06/08/11 jrf 2.51.10 173353 Created
        //
        public byte? PowerMonitoringColdLoadTime
        {
            get
            {
                byte? bytColdLoadTime = null;

                if (null != Table2369)
                {
                    bytColdLoadTime = Table2369.ColdLoadTime;
                }

                return bytColdLoadTime;
            }
        }

        #endregion

        #region Voltage Monitoring

        /// <summary>
        /// Gets the detected form of the meter from voltage monitoring data.  For poly meters, it is the actual meter form. 
        /// For mono meters, the value 12 is for a 12S and the value 255 is for a non-12S form (1S, 2S, 3S, 4S).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5308 Created
        //
        public MeterForm Form
        {
            get
            {
                MeterForm Form = MeterForm.NON_FM12S;

                if (null != Table2156)
                {
                    Form = Table2156.Form;
                }

                return Form;
            }
        }

        /// <summary>
        /// Gets the number of phases change action needed flag. This field is valid for mono meters only. 
        /// It is always 0 for poly meters.  True indicates action is requested to change the number of 
        /// phases in the meter for extended voltage monitoring.
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
                bool blnActionNeeded = false;

                if (null != Table2156)
                {
                    blnActionNeeded = Table2156.NumberPhaseChangeActionNeeded;
                }

                return blnActionNeeded;
            }
        }

        /// <summary>
        /// Gets the service type of the meter from the voltage monitoring data.  Currently this only
        /// supports extended voltage monitoring!!!  If desired support could be added for 
        /// checking value in legacy voltage monitoring.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5360 Created
        //
        public MeterServiceType VoltageMonitoringServiceType
        {
            get
            {
                MeterServiceType ServiceType = MeterServiceType.NoValidService;

                if (null != Table2156)
                {
                    ServiceType = Table2156.ServiceType;
                }

                return ServiceType;
            }
        }

        /// <summary>
        /// Gets the meter's nominal voltage.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5360 Created
        //
        public ushort[] NominalVoltage
        {
            get
            {
                ushort[] ausNomialVoltage = null;

                if (null != Table2156)
                {
                    ausNomialVoltage = Table2156.NominalVoltage;
                }

                return ausNomialVoltage;
            }
        }

        /// <summary>
        /// Gets whether the meter supports legacy voltage monitoring.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/08/13 jrf 2.70.66 288156 Created
        //
        public bool LegacyVoltageMonitoringSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(2148) && true == Table00.IsTableUsed(2149)
                    && true == Table00.IsTableUsed(2150) && true == Table00.IsTableUsed(2151)
                    && true == Table00.IsTableUsed(2152));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets whether the meter supports extended voltage monitoring.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/11 jrf 2.53.13 TC5481 Created
        //
        public bool ExtVoltageMonitoringSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(2153) && true == Table00.IsTableUsed(2154)
                    && true == Table00.IsTableUsed(2155) && true == Table00.IsTableUsed(2156)
                    && true == Table00.IsTableUsed(2157));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets/sets whether voltage monitoring is enabled in the meter.  Currently this only
        /// supports extended voltage monitoring!!!  If desired support could be added for 
        /// checking value in legacy voltage monitoring.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/11 jrf 2.53.13 TC5481 Created
        //
        public bool VoltageMonitoringEnabled
        {
            get
            {
                bool blnEnabled = false;

                if (null != Table2154)
                {
                    blnEnabled = Table2154.VoltageMonitoringEnabled;
                }

                return blnEnabled;
            }
            set
            {
                if (null != Table2154)
                {
                    Table2154.VoltageMonitoringEnabled = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets the voltage monitoring interval length in minutes.  Currently this only
        /// supports extended voltage monitoring!!!  If desired support could be added for 
        /// checking value in legacy voltage monitoring.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/11 jrf 2.53.13 TC5481 Created
        //
        public byte VMIntevalLength
        {
            get
            {
                byte bytIntervalLength = 0;

                if (null != Table2153)
                {
                    bytIntervalLength = Table2153.IntervalLength;
                }

                return bytIntervalLength;
            }
            set
            {
                if (null != Table2153)
                {
                    Table2153.IntervalLength = value;
                }
            }
        }

        /// <summary>
        /// Gets number of valid voltage monitoring blocks. 
        /// </summary>
        public ushort VMNumberOfValidBlocks
        {
            get
            {
                ushort NumberOfValidBlocks = 0;

                if (null != Table2155)
                {
                    NumberOfValidBlocks = Table2155.NumberValidBlocks;
                }

                return NumberOfValidBlocks;
            }
        }

        #endregion

        #region IP Comm Module Information

        /// <summary>
        /// Gets the TLV ID requested in the Mfg proc 150 request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/11 AF  2.53.21 184509 Created
        //
        public UInt32 TLVIdRequested
        {
            get
            {
                UInt32 uiTLVId = 0;

                if (null != Table2612)
                {
                    uiTLVId = Table2612.TLVID;
                }

                return uiTLVId;
            }
        }

        /// <summary>
        /// Gets the data for the TLV ID requested with Mfg proc 150
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/11 AF  2.53.21 184509 Created
        //
        public byte[] TLVIdRequestedData
        {
            get
            {
                byte[] abyTLVIdData = null;

                if (null != Table2612)
                {
                    abyTLVIdData = Table2612.TLVData;
                }

                return abyTLVIdData;
            }
        }

        /// <summary>
        /// Reads the number of UDP dropped packets from Mfg table 563
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created for Boron f/w testing
        //  09/21/12 AF  2.70.19 202904 Read table 00 again to make sure whether table
        //                              2611 (563) is supported
        //
        public UInt32 UDPDroppedPacketsCount
        {
            get
            {
                UInt32 uiDroppedPkts = 0;

                // If table 2611 has been made visible, we need a re-read of table 00
                // so that it will show up as supported
                RereadTable00();

                if (null != Table2611)
                {
                    uiDroppedPkts = Table2611.UDPDropped;
                }

                return uiDroppedPkts;
            }
        }

        /// <summary>
        /// Reads the number of IP packets dropped from Mfg table 563
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/12 AF  2.70.08 TC9224 Created for Boron f/w testing
        //  09/21/12 AF  2.70.19 202904 Read table 00 again to make sure whether table
        //                              2611 (563) is supported
        //
        public UInt32 IPDroppedPacketsCount
        {
            get
            {
                UInt32 uiDroppedPkts = 0;

                // If table 2611 has been made visible, we need a re-read of table 00
                // so that it will show up as supported
                RereadTable00();

                if (null != Table2611)
                {
                    uiDroppedPkts = Table2611.IPv6Dropped;
                }

                return uiDroppedPkts;
            }
        }

        /// <summary>
        /// Reads the number of UDP check errors from Mfg table 563
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created for Boron f/w testing
        //
        public UInt32 UDPCheckErrorCount
        {
            get
            {
                UInt32 uiCheckErrs = 0;

                if (null != Table2611)
                {
                    uiCheckErrs = Table2611.UDPCheckError;
                }

                return uiCheckErrs;
            }
        }

        /// <summary>
        /// Reads the number of IPv6 packets dropped from Mfg table 563
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created
        //
        public UInt32 IPv6DroppedPacketCount
        {
            get
            {
                UInt32 uiIPDropped = 0;

                if (null != Table2611)
                {
                    uiIPDropped = Table2611.IPv6Dropped;
                }

                return uiIPDropped;
            }
        }

        #endregion

        #region Extended Self Reads

        /// <summary>
        /// Gets whether the meter supports extended voltage monitoring.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 jrf 2.53.32 TREQ2904 Created
        //
        public bool ExtSelfReadSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(2419) && true == Table00.IsTableUsed(2421)
                    && true == Table00.IsTableUsed(2423));

                return blnSupported;
            }
        }

        /// <summary>
        /// This property retrieves the extended self read data as a list of extended 
        /// self read records in descending date order from the most recent self read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 jrf 2.53.32 TREQ2904 Created
        //
        public List<ExtendedSelfReadRecord> ExtendedSelfReadData
        {
            get
            {
                List<ExtendedSelfReadRecord> ExtSRData = null;

                if (true == ExtSelfReadSupported)
                {
                    ExtSRData = ReorderExtendedSelfReadData(Table2419, Table2421, Table2423);
                }

                return ExtSRData;
            }
        }

        #endregion

        #region RIB

        /// <summary>
        /// Device supports residential inclining block (RIB) pricing.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public bool SupportsRIB
        {
            get
            {
                bool bSupported = false;

                if (Table00.IsTableUsed(2438) && Table00.IsTableUsed(2439) && Table00.IsTableUsed(2440) && Table00.IsTableUsed(2441))
                {
                    bSupported = true;
                }

                return bSupported;
            }
        }

        /// <summary>
        /// Residential inclining block (RIB) pricing is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public bool RIBPricingEnabled
        {
            get
            {
                bool bEnabled = false;

                if (null != Table2440)
                {
                    bEnabled = Table2440.BlockPricingEnabled;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// The schedule ID for the active RIB schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public string ActiveRIBScheduleID
        {
            get
            {
                string strScheduleID = "";

                if (null != Table2440)
                {
                    strScheduleID = Table2440.ScheduleId;
                }

                return strScheduleID;
            }
        }

        /// <summary>
        /// The most recent summed value of energy delivered and consumed in the premises during 
        /// the current block period.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public UInt64 CurrentRIBBlockPeriodConsumptionDelivered
        {
            get
            {
                UInt64 uiConsumption = 0;

                if (null != Table2440)
                {
                    uiConsumption = Table2440.CurrentBlockPeriodConsumptionDelivered;
                }

                return uiConsumption;
            }
        }

        /// <summary>
        /// The value of energy delivered and consumed in the premises during 
        /// the previous block period.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public UInt64 PreviousRIBBlockPeriodConsumptionDelivered
        {
            get
            {
                UInt64 uiConsumption = 0;

                if (null != Table2440)
                {
                    uiConsumption = Table2440.PreviousBlockPeriodConsumptionDelivered;
                }

                return uiConsumption;
            }
        }

        /// <summary>
        /// The value to be multiplied against the thresholds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public uint RIBBlockThresholdMultiplier
        {
            get
            {
                uint uiMultiplier = 0;

                if (null != Table2440)
                {
                    uiMultiplier = Table2440.Multiplier;
                }

                return uiMultiplier;
            }
        }

        /// <summary>
        /// The value to be divided against the thresholds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public uint RIBBlockThresholdDivisor
        {
            get
            {
                uint uiDivisor = 0;

                if (null != Table2440)
                {
                    uiDivisor = Table2440.Divisor;
                }

                return uiDivisor;
            }
        }

        /// <summary>
        /// The published price data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public PublishPriceDataEntryRcd RIBPublishedPriceData
        {
            get
            {
                PublishPriceDataEntryRcd PriceData = null;

                if (null != Table2440)
                {
                    PriceData = Table2440.PublishPriceData;
                }

                return PriceData;
            }
        }

        /// <summary>
        /// The billing period information currently being presented to the HAN.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public BillingPeriodRcd ActiveRIBBillingPeriod
        {
            get
            {
                BillingPeriodRcd BillingPeriod = null;

                if (null != Table2440)
                {
                    BillingPeriod = Table2440.ActiveBillingPeriod;
                }

                return BillingPeriod;
            }
        }

        /// <summary>
        /// The block period information currently being presented to the HAN.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public BlockPeriodRcd ActiveRIBBlockPeriod
        {
            get
            {
                BlockPeriodRcd BlockPeriod = null;

                if (null != Table2440)
                {
                    BlockPeriod = Table2440.ActiveBlockPeriod;
                }

                return BlockPeriod;
            }
        }

        /// <summary>
        /// The billing periods in the current schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public ReadOnlyCollection<BillingPeriodRcd> RIBBillingPeriods
        {
            get
            {
                ReadOnlyCollection<BillingPeriodRcd> BillingPeriods = null;

                if (null != Table2440)
                {
                    BillingPeriods = Table2440.BillingPeriods;
                }

                return BillingPeriods;
            }
        }

        /// <summary>
        /// The block periods in the current schedlule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public ReadOnlyCollection<BlockPeriodRcd> RIBBlockPeriods
        {
            get
            {
                ReadOnlyCollection<BlockPeriodRcd> BlockPeriods = null;

                if (null != Table2440)
                {
                    BlockPeriods = Table2440.BlockPeriods;
                }

                return BlockPeriods;
            }
        }

        /// <summary>
        /// Gets the current state of the Next Block Price Schedule Status
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/27/12 PGH 2.60.           Created

        public CHANMfgTable2441.NextBlockPriceScheduleStatus? NextBlockPriceScheduleStatusState
        {
            get
            {
                CHANMfgTable2441.NextBlockPriceScheduleStatus? CurrentState = null;

                if (Table2441 != null)
                {
                    PSEMResponse Response = Table2441.Read();
                    if (Response.Equals(PSEMResponse.Ok))
                    {
                        CurrentState = Table2441.NextTableState;
                    }
                }

                return CurrentState;
            }
        }

        /// <summary>
        /// The schedule ID for the next RIB schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/24/13 jrf 2.70.60 287724 Created
        //
        public string NextRIBScheduleID
        {
            get
            {
                string strScheduleID = "";

                if (null != Table2441)
                {
                    strScheduleID = Table2441.ScheduleId;
                }

                return strScheduleID;
            }
        }

        /// <summary>
        /// The billing periods in the next schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/24/123 jrf 2.70.60 287724 Created
        //
        public ReadOnlyCollection<BillingPeriodRcd> NextRIBBillingPeriods
        {
            get
            {
                ReadOnlyCollection<BillingPeriodRcd> BillingPeriods = null;

                if (null != Table2441)
                {
                    BillingPeriods = Table2441.BillingPeriods.AsReadOnly();
                }

                return BillingPeriods;
            }
        }

        /// <summary>
        /// The block periods in the next schedlule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/24/123 jrf 2.70.60 287724 Created
        //
        public ReadOnlyCollection<NextBlockPeriodRcd> NextRIBBlockPeriods
        {
            get
            {
                ReadOnlyCollection<NextBlockPeriodRcd> BlockPeriods = null;

                if (null != Table2441)
                {
                    BlockPeriods = Table2441.BlockPeriodData.AsReadOnly();
                }

                return BlockPeriods;
            }
        }

        #endregion

        #region TOU

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

        #endregion

        #region Extended Outages

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
        /// Gets the power up threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/26/16 AF  4.50.224 RTT586620   Created
        //
        public ushort PowerUpThreshold
        {
            get
            {
                if (Table00.IsTableUsed(2260) && Table2260ExtendedConfig != null)
                {
                    return Table2260ExtendedConfig.PowerUpThreshold;
                }
                else
                {
                    // use the value that would be present if the table is supported but the 
                    // power up threshold value is not there.  We will be reading the unused area
                    return 0xFFFF;
                }
            }
        }

        /// <summary>
        /// Determines whether or not the power up threshold item is supported.  First Table 2260
        /// must be supported and then the firmware must be Beryllium or greater.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/26/16 AF  4.50.224 RTT586620  Created
        //  02/03/16 AF  4.50.225 RTT586620  Changed the check on power up threshold value to a firmware check
        //                                   because we can't tell from the value whether or not it is supported.
        //
        public bool PowerUpThresholdSupported
        {
            get
            {
                bool blnSupported = false;

                if (Table00.IsTableUsed(2260) && (Table2260ExtendedConfig != null) && VersionChecker.CompareTo(FWRevision, VERSION_BERYLLIUM) >= 0)
                {
                    blnSupported = true;
                }

                return blnSupported;
            }
        }

        #endregion

        #region Current Per Phase Threshold Exceeded

        /// <summary>
        /// Gets the current per phase exceeded enabled field from Mfg table 217
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/11/13 AF  2.80.07 TR7590 Created
        //  10/17/13 AF  3.00.19 WR426024 Table 2265 may not be present on older meters.
        //                                Added a check for null.
        //
        public bool IsCTEEnabled
        {
            get
            {
                bool enabled = false;

                if (Table2265CTEConfig != null)
                {
                    enabled =  Table2265CTEConfig.CTEEnable;
                }

                return enabled;
            }
        }

        /// <summary>
        /// Gets the current per phase exceeded enabled field from Mfg table 217
        /// and determines if CTE has been configured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/24/13 AF  2.80.23 TR7590 Created
        //  10/17/13 AF  3.00.19 WR426024 Table 2265 may not be present on older meters.
        //                                Added a check for null.
        //  02/18/14 AF   3.00.36 WR428639 Made virtual for override in M2 Gateway
        //
        public virtual bool IsCTEConfigured
        {
            get
            {
                bool enabled = false;

                if (Table2265CTEConfig != null)
                {
                    enabled = Table2265CTEConfig.CTEConfigured;
                }

                return enabled;
            }
        }

        /// <summary>
        ///  Gets the current per phase exceeded threshold field from Mfg table 217
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/11/13 AF  2.80.07 TR7590 Created
        //  10/17/13 AF  3.00.19 WR426024 Table 2265 may not be present on older meters.
        //                                Added a check for null.
        //
        public byte CTEThreshold
        {
            get
            {
                // We should not ever see this value because we should check first that CTE is configured
                byte threshold = 0xFF;

                if (Table2265CTEConfig != null)
                {
                    threshold = Table2265CTEConfig.CTEThreshold;
                }

                return threshold;
            }
        }

        /// <summary>
        ///  Gets the current per phase exceeded hysteresis field from Mfg table 217
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/11/13 AF  2.80.07 TR7590 Created
        //  10/17/13 AF  3.00.19 WR426024 Table 2265 may not be present on older meters.
        //                                Added a check for null.
        //
        public byte CTEHysteresis
        {
            get
            {
                // We should not ever see this value because we should check first that CTE is configured
                byte hysteresis = 0xFF;

                if (Table2265CTEConfig != null)
                {
                    hysteresis = Table2265CTEConfig.CTEHysteresis;
                }

                return hysteresis;
            }
        }

        /// <summary>
        ///  Gets the current per phase exceeded debounce field from Mfg table 217
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/11/13 AF  2.80.07 TR7590 Created
        //  10/17/13 AF  3.00.19 WR426024 Table 2265 may not be present on older meters.
        //                                Added a check for null.
        //
        public byte CTEDebounce
        {
            get
            {
                // We should not ever see this value because we should check first that CTE is configured
                byte debounce = 0xFF;

                if (Table2265CTEConfig != null)
                {
                    debounce = Table2265CTEConfig.CTEDebounce;
                }

                return debounce;
            }
        }

        /// <summary>
        ///  Gets the current per phase exceeded minimum active duration field from Mfg table 217
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/11/13 AF  2.80.07 TR7590 Created
        //  10/17/13 AF  3.00.19 WR426024 Table 2265 may not be present on older meters.
        //                                Added a check for null.
        //
        public UInt16 CTEMinActiveDuration
        {
            get
            {
                // We should not ever see this value because we should check first that CTE is configured
                ushort activeDuration = 0;

                if (Table2265CTEConfig != null)
                {
                    activeDuration = Table2265CTEConfig.CTEMinActiveDuration;
                }

                return activeDuration;
            }
        }

        #endregion

        #region Instantaneous Phase Current

        /// <summary>
        /// Gets the Instantaneous Phase Current from MFG table 2377
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //
        public IPCDataRcd InstantaneousPhaseCurrent
        {
            get
            {
                IPCDataRcd IPCDataRcd = null;

                if (null != Table2377)
                {
                    IPCDataRcd = Table2377.IPCDataRecord;
                }

                return IPCDataRcd;
            }
        }

        #endregion

        #region Bell Weather Configuration

        /// <summary>
        /// Gets the Bell Weather DataSet Configuration
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 12/09/15 PGH 4.50.219 577471  Created 
        //
        public DataSetConfigRcd[] BellWeatherDataSetConfiguration
        {
            get
            {
                DataSetConfigRcd[] aBellWeatherDataSetConfiguration = null;

                if (null != Table2265DataSetConfig)
                {
                    aBellWeatherDataSetConfiguration = Table2265DataSetConfig.DataSetConfiguration;
                }

                return aBellWeatherDataSetConfiguration;
            }
        }

        /// <summary>
        /// Gets the Bell Weather Configuration Record
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 12/09/15 PGH 4.50.219 577471  Created 
        //
        public PushConfigRcd[] BellWeatherConfigRcd
        {
            get
            {
                PushConfigRcd[] aBellWeatherConfigRcd = null;

                if (null != Table2185)
                {
                    aBellWeatherConfigRcd = Table2185.BellWeatherConfigRcd;
                }

                return aBellWeatherConfigRcd;
            }
        }

        /// <summary>
        /// Gets the Bell Weather Group Data Status Record
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 12/09/15 PGH 4.50.219 577471  Created 
        //
        public GroupDataStatusRcd BellWeatherGroupDataStatusRcd
        {
            get
            {
                GroupDataStatusRcd aBellWeatherGroupDataStatusRcd = null;

                if (null != Table2186)
                {
                    aBellWeatherGroupDataStatusRcd = Table2186.BellWeatherGroupDataStatusRcd;
                }

                return aBellWeatherGroupDataStatusRcd;
            }
        }

        /// <summary>
        /// Gets the Bell Weather Enable Record
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 12/09/15 PGH 4.50.219 577471  Created 
        //
        public BubbleUpEnableRcd BellWeatherEnableRcd
        {
            get
            {
                BubbleUpEnableRcd aBellWeatherEnableRcd = null;

                if (null != Table2187)
                {
                    aBellWeatherEnableRcd = Table2187.BellWeatherEnableRcd;
                }

                return aBellWeatherEnableRcd;
            }
        }


        #endregion

        #region Temperature Monitoring

        /// <summary>
        /// Gets the Temperature Configuration
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        //  01/21/16 PGH 4.50.224 RTT556309 Created
        //
        public TemperatureConfigRcd TemperatureConfiguration
        {
            get
            {
                TemperatureConfigRcd TemperatureConfigurationRecord = null;

                if (null != Table2425)
                {
                    TemperatureConfigurationRecord = Table2425.TemperatureConfigRcd;
                }

                return TemperatureConfigurationRecord;
            }
        }

        /// <summary>
        /// For automated fw testing.  Allows us to set the high temperature threshold 1
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/23/16 AF  4.50.232 TC 62857   Created
        //
        public Int16? TemperatureThreshold1
        {
            get
            {
                Int16? HighTemperatureThreshold1 = null;

                if (null != Table2425)
                {
                    HighTemperatureThreshold1 = Table2425.TemperatureConfigRcd.HighTemperatureThreshold1;
                }

                return HighTemperatureThreshold1;
            }
            set
            {
                if ((null != Table2425) && (null != value))
                {
                    Table2425.TemperatureConfigRcd.HighTemperatureThreshold1 = value.Value;
                }
            }
        }

        /// <summary>
        /// For automated fw testing.  Allows us to set the high temperature threshold 2
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/23/16 AF  4.50.232 TC 62857   Created
        //
        public Int16? TemperatureThreshold2
        {
            get
            {
                Int16? HighTemperatureThreshold2 = null;

                if (null != Table2425)
                {
                    HighTemperatureThreshold2 = Table2425.TemperatureConfigRcd.HighTemperatureThreshold2;
                }

                return HighTemperatureThreshold2;
            }
            set
            {
                if ((null != Table2425) && (null != value))
                {
                    Table2425.TemperatureConfigRcd.HighTemperatureThreshold1 = value.Value;
                }
            }
        }

        /// <summary>
        /// Gets the Temperature Data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        //  01/26/16 PGH 4.50.224 RTT556309 Created
        //
        public TemperatureDataRcd TemperatureData
        {
            get
            {
                TemperatureDataRcd TemperatureDataRecord = null;

                if (null != Table2426)
                {
                    TemperatureDataRecord = Table2426.TemperatureDataRcd;
                }

                return TemperatureDataRecord;
            }
        }

        /// <summary>
        /// Gets the Temperature Log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/16 PGH 4.50.224 RTT556309 Created
        //
        public ReadOnlyCollection<TemperatureLogEntry> TemperatureLog
        {
            get
            {
                ReadOnlyCollection<TemperatureLogEntry> TemperatureLogEntries = null;

                if (null != Table2427)
                {
                    TemperatureLogEntries = Table2427.TemperatureLog.TemperatureLogEntries.ToList().AsReadOnly();
                }

                return TemperatureLogEntries;
            }
        }

        
        /// <summary>
        /// Gets the Temperature Log - does a fresh read
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/19/16 AF  4.50.231 TC 62792   Created
        //
        public ReadOnlyCollection<TemperatureLogEntry> TemperatureLogUncached
        {
            get
            {
                ReadOnlyCollection<TemperatureLogEntry> TemperatureLogEntries = null;

                if (null != Table2427)
                {
                    TemperatureLogEntries = Table2427.UncachedTemperatureLog.TemperatureLogEntries.ToList().AsReadOnly();
                }

                return TemperatureLogEntries;
            }
        }

        #endregion

        /// <summary>
        /// DO NOT USE THIS PROPERTY!  Use the CommModule property instead.  That is unless, 
        /// you are writing an automated test and CommModule regularly fails to be created because 
        /// the device class can't be read from tbl. 2064.  In that case use this property to get 
        /// the RFLAN Comm Module object.  
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/10 jrf 2.40.22 N/A    Created
        public RFLANCommModule RFLANModule
        {
            get
            {
                if (m_CommModule == null)
                {
                    m_CommModule = new RFLANCommModule(m_PSEM, this);
                }

                return m_CommModule as RFLANCommModule;
            }
        }

        #endregion

        #region Static Public

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

        /// <summary>
        /// Translates the numeric HAN firmware download status code into a string
        /// </summary>
        /// <param name="status">numerical status code</param>
        /// <returns>the status translated into a string</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/09 AF  2.20.02        Created
        //
        static public string TranslateHANFwDlStatus(CENTRON_AMI.HAN_FW_DL_STATUS status)
        {
            string strResult = "";
            ResourceManager rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, typeof(CENTRON_AMI).Assembly);

            switch (status)
            {
                case HAN_FW_DL_STATUS.Activating:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_ACTIVATING");
                        break;
                    }
                case HAN_FW_DL_STATUS.ActivationSent:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_ACTIVATION_SENT");
                        break;
                    }
                case HAN_FW_DL_STATUS.BadCRC:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_BAD_CRC");
                        break;
                    }
                case HAN_FW_DL_STATUS.BadFileSize:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_BAD_FILE_SIZE");
                        break;
                    }
                case HAN_FW_DL_STATUS.BadFWType:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_BAD_FW_TYPE");
                        break;
                    }
                case HAN_FW_DL_STATUS.BadHWRevision:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_BAD_HW_REV");
                        break;
                    }
                case HAN_FW_DL_STATUS.BadHWVersion:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_BAD_HW_VER");
                        break;
                    }
                case HAN_FW_DL_STATUS.DownloadInProgress:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_DL_IN_PROGRESS");
                        break;
                    }
                case HAN_FW_DL_STATUS.DownloadSetup:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_DL_SETUP");
                        break;
                    }
                case HAN_FW_DL_STATUS.FlashWriteFailure:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_FLASH_WRITE_FAIL");
                        break;
                    }
                case HAN_FW_DL_STATUS.HeadEndCancellation:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_HEAD_END_CANCEL");
                        break;
                    }
                case HAN_FW_DL_STATUS.InitiateFailure:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_INITIATE_FAIL");
                        break;
                    }
                case HAN_FW_DL_STATUS.InsufficientBlocks:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_INSUFF_BLKS");
                        break;
                    }
                case HAN_FW_DL_STATUS.InvalidDeviceClass:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_INVALID_DEV_CLASS");
                        break;
                    }
                case HAN_FW_DL_STATUS.InvalidFWType:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_INVALID_FW_TYPE");
                        break;
                    }
                case HAN_FW_DL_STATUS.Paused:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_PAUSED");
                        break;
                    }
                case HAN_FW_DL_STATUS.ReadyForActivation:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_READY_FOR_ACTIVATION");
                        break;
                    }
                case HAN_FW_DL_STATUS.ResyncNeeded:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_RESYNC_NEEDED");
                        break;
                    }
                case HAN_FW_DL_STATUS.RetryFailed:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_RETRY_FAILED");
                        break;
                    }
                case HAN_FW_DL_STATUS.SuccessfulEndOfTransfer:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_SUCCESS_END_OF_TRANSFER");
                        break;
                    }
                case HAN_FW_DL_STATUS.SyncPacketSent:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_SYNC_PKT_SENT");
                        break;
                    }
                case HAN_FW_DL_STATUS.Unknown:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_UNKNOWN");
                        break;
                    }
                case HAN_FW_DL_STATUS.VersionDownloaded:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_VER_DOWNLOADED");
                        break;
                    }
                case HAN_FW_DL_STATUS.VersionRunning:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_VER_RUNNING");
                        break;
                    }
                case HAN_FW_DL_STATUS.VoltageOutOfRange:
                    {
                        strResult = rmStrings.GetString("HAN_FW_DL_VOLT_OUT_OF_RANGE");
                        break;
                    }
            }
            return strResult;
        }

        /// <summary>
        /// This method reorders the extended self read records so the records from the oldest extended self read come first
        /// followed by next oldest records and so on.
        /// </summary>
        /// <param name="Table2419">The actual mfg. extended self read table.</param>
        /// <param name="Table2421">The list of extended self read records.</param>
        /// <param name="Table2423"></param>
        /// <returns>The next extended self read starting index record.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/20/12 jrf 2.53.32 TREQ3448 Created
        //  02/07/12 jrf 2.53.38 TC7107 Handling case where there are no self read entries.
        //
        public static List<ExtendedSelfReadRecord> ReorderExtendedSelfReadData(OpenWayMFGTable2419 Table2419, OpenWayMFGTable2421 Table2421, OpenWayMFGTable2423 Table2423)
        {
            List<ExtendedSelfReadRecord> ExtSRData = new List<ExtendedSelfReadRecord>(); //For reordering the extended self read data list.

            //Don't try to read if we don't have any entries
            if (0 < Table2419.NumberExtendedSelfReadEntries)
            {
                //Sequence numbers do not wrap, thus the calculations below.  Previous self read start will initially be the last (oldest) self read.
                int iPreviousSRStartingIndex = (int)(Table2421.LastEntrySequenceNumber -
                    ((Table2421.LastEntrySequenceNumber / Table2419.NumberExtendedSelfReadEntries) * Table2419.NumberExtendedSelfReadEntries)) + 1;
                //First self read will be all records with the oldest date.
                int iFirstSRStartingIndex = (int)(Table2421.FirstEntrySequenceNumber -
                    ((Table2421.FirstEntrySequenceNumber / Table2419.NumberExtendedSelfReadEntries) * Table2419.NumberExtendedSelfReadEntries));
                int iNextSRStartingIndex = iFirstSRStartingIndex;
                DateTime dtFirstSRDate = Table2423.ExtendedSelfReadData[iFirstSRStartingIndex].TimeOfOccurence;
                ReadOnlyCollection<ExtendedSelfReadRecord> ExtSRRecords = Table2423.ExtendedSelfReadData;

                do
                {
                    //check if the self read is wrapped around the end of the list.
                    if (iNextSRStartingIndex > iPreviousSRStartingIndex)
                    {
                        //take care of the end of list first------>
                        //[---------------------------------------]
                        for (int i = iNextSRStartingIndex; i < ExtSRRecords.Count; i++)
                        {
                            ExtSRData.Add(ExtSRRecords[i]);
                        }

                        //-------> now get the values at the beginning of the list
                        //[---------------------------------------]
                        for (int i = 0; i < iPreviousSRStartingIndex; i++)
                        {
                            ExtSRData.Add(ExtSRRecords[i]);
                        }
                    }

                    //if self read is wrapped around end of list this section will be skipped.
                    for (int i = iNextSRStartingIndex; i < iPreviousSRStartingIndex; i++)
                    {
                        ExtSRData.Add(ExtSRRecords[i]);
                    }

                    iPreviousSRStartingIndex = iNextSRStartingIndex;
                    iNextSRStartingIndex = GetNextExtSRStartIndex(iPreviousSRStartingIndex, ExtSRRecords);

                    //continue searching until we get back to the beginning of the list.
                } while (iFirstSRStartingIndex != iNextSRStartingIndex);
            }

            return ExtSRData;
        }

        /// <summary>
        /// This method gets the starting index of the next oldest extended self read based on
        /// a given previous self read starting record index and a list of extended self read records.
        /// </summary>
        /// <param name="iPreviousSRRecordStart">The previous extended self read starting record index.</param>
        /// <param name="ExtSRRecords">The list of extended self read records.</param>
        /// <returns>The next extended self read starting index record.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 jrf 2.53.32 TREQ2904 Created
        //
        public static int GetNextExtSRStartIndex(int iPreviousSRRecordStart, ReadOnlyCollection<ExtendedSelfReadRecord> ExtSRRecords)
        {
            int iNextSRRecord = iPreviousSRRecordStart - 1;
            DateTime dtNextSRDate = DateTime.MinValue;

            //Check if we should wrap around.
            if (0 > iNextSRRecord)
            {
                iNextSRRecord = ExtSRRecords.Count - 1;
            }

            //we will use a difference in date to distinguish when the next self read record starts
            dtNextSRDate = ExtSRRecords[iNextSRRecord].TimeOfOccurence;

            while (iNextSRRecord > 0 && ExtSRRecords[iNextSRRecord - 1].TimeOfOccurence == dtNextSRDate)
            {
                iNextSRRecord--;
            }

            //iNextSRRecord == 0, then search from end of list back down
            if (0 == iNextSRRecord && ExtSRRecords[ExtSRRecords.Count - 1].TimeOfOccurence == dtNextSRDate)
            {
                iNextSRRecord = ExtSRRecords.Count - 1;

                while (iNextSRRecord > iPreviousSRRecordStart && ExtSRRecords[iNextSRRecord - 1].TimeOfOccurence == dtNextSRDate)
                {
                    iNextSRRecord--;
                }
            }

            return iNextSRRecord;
        }

        #endregion

        #region Internal Methods

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
        /// Injects a new HAN event into the HAN event logs.
        /// ****ONLY USED FOR TESTING****
        /// </summary>
        /// <param name="byEventNumber">The event's number.</param>
        /// <param name="uiEventID">The event's associated ID.</param>
        /// <param name="uiMACAddress">The MAC address of the device event is comming from.</param>
        /// <param name="abytArgumentData">Data associated with the event.</param>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/05/11 jrf 2.45.22		Created
        //  03/15/13 PGH 2.80.08 327121 Changed method from internal to public
        //
        public ProcedureResultCodes InjectHANEvents(byte byEventNumber, UInt32 uiEventID, UInt64 uiMACAddress, byte[] abytArgumentData)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(18);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] abytProcResponse;

            ParamWriter.Write(byEventNumber);
            ParamWriter.Write(uiEventID);
            ParamWriter.Write(uiMACAddress);
            ParamWriter.Write(abytArgumentData);

            ProcResult = ExecuteProcedure(Procedures.HAN_EVENT_INJECTION,
                ProcParam.ToArray(), out abytProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Injects History and Event Logs using Procedure 106
        /// ****ONLY USED FOR TESTING****
        /// </summary>
        /// <param name="byLogSelect">Log Select</param>
        /// <param name="byEventID">The event's associated ID.</param>
        /// <param name="uiRepeatCount">Repeat Count</param>
        /// <param name="byArgumentFillByte"></param>
        /// <param name="abytArgumentData">Data associated with the event.</param>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/24/11 MMD        		Created
        //  03/15/13 PGH 2.80.08 327121 Changed method from internal to public
        //
        public ProcedureResultCodes InjectEvents(byte byLogSelect, byte byEventID, UInt16 uiRepeatCount, byte byArgumentFillByte, byte[] abytArgumentData)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            int capacity = 5 + abytArgumentData.Length;
            MemoryStream ProcParam = new MemoryStream(capacity);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] abytProcResponse;

            ParamWriter.Write(byLogSelect);
            ParamWriter.Write(byEventID);
            ParamWriter.Write(uiRepeatCount);
            ParamWriter.Write(byArgumentFillByte);
            ParamWriter.Write(abytArgumentData);

            ProcResult = ExecuteProcedure(Procedures.EVENT_INJECTION,
                ProcParam.ToArray(), out abytProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Injects HAN Events using Procedure 106
        /// ****ONLY USED FOR TESTING****
        /// </summary>
        /// <param name="byLogSelect">Log Select</param>
        /// <param name="byEventID">The event's associated ID.</param>
        /// <param name="uiRepeatCount">Repeat Count</param>
        ///  <param name="uiHANEventID">The event's associated ID.</param>
        /// <param name="uiMACAddress">The MAC address of the device event is comming from.</param>
        /// <param name="abytArgumentData">Data associated with the event.</param>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/24/11 MMD        		Created 
        //
        internal ProcedureResultCodes InjectHANEventsWithProc106(byte byLogSelect, byte byEventID, UInt16 uiRepeatCount, UInt32 uiHANEventID, UInt64 uiMACAddress, byte[] abytArgumentData)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            int capacity = 5 + abytArgumentData.Length + 4 + 8;
            MemoryStream ProcParam = new MemoryStream(capacity);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] abytProcResponse;
            byte byArgumentFillByte = 0;

            ParamWriter.Write(byLogSelect);
            ParamWriter.Write(byEventID);
            ParamWriter.Write(uiRepeatCount);
            ParamWriter.Write(byArgumentFillByte);
            ParamWriter.Write(uiHANEventID);
            ParamWriter.Write(uiMACAddress);
            ParamWriter.Write(abytArgumentData);

            ProcResult = ExecuteProcedure(Procedures.EVENT_INJECTION,
                ProcParam.ToArray(), out abytProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Injects FW Download Events into Log using Procedure 115
        /// ****ONLY USED FOR TESTING****
        /// </summary>
        /// <param name="byEventID">The event's associated ID.</param>
        /// <param name="bytEventSource">The event's Source.</param>
        /// <param name="uiImageCRC">Image CRC</param>
        /// <param name="abytImageHash">Image Hash</param>
        /// <param name="abytCurrentImageRevision">Current Image Revision</param>
        /// <param name="abytPreviousImageRevision">Previous Image Revision</param>
        /// <param name="abytArgumentData">Data associated with the event.</param>
        /// <returns>The result of the procedure call.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/10/11 MMD        		Created 
        //
        public ProcedureResultCodes InjectFWDownloadEvents(byte byEventID, byte bytEventSource, UInt32 uiImageCRC, byte[] abytImageHash, byte[] abytCurrentImageRevision, byte[] abytPreviousImageRevision, byte[] abytArgumentData)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            int capacity = 6 + abytImageHash.Length + abytCurrentImageRevision.Length + abytPreviousImageRevision.Length;
            MemoryStream ProcParam = new MemoryStream(capacity);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] abytProcResponse;
            ParamWriter.Write(byEventID);
            ParamWriter.Write(bytEventSource);
            ParamWriter.Write(uiImageCRC);
            ParamWriter.Write(abytImageHash);
            ParamWriter.Write(abytCurrentImageRevision);
            ParamWriter.Write(abytPreviousImageRevision);
            //ParamWriter.Write(abytArgumentData);

            ProcResult = ExecuteProcedure(Procedures.FIRMWARE_DOWNLOAD_EVENT_INJECTION,
                ProcParam.ToArray(), out abytProcResponse);

            return ProcResult;
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
        /// Reconfigures the Multiplier and Divisor used by Simple Metering
        /// </summary>
        /// <param name="multiplier">The multiplier to use</param>
        /// <param name="divisor">The divisor to use</param>
        /// <returns>The result of the write</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/12 RCG 2.60.28        Created
        //  12/17/13 DLG 3.50.16        Updated to use HANInformation object to access table 2106.
        //
        public PSEMResponse ReconfigureSimpleMeteringMultiplierAndDivisor(uint multiplier, uint divisor)
        {
            PSEMResponse Response = PSEMResponse.Onp;

            if (m_HANInfo.Table2106 != null)
            {
                m_HANInfo.Table2106.SimpleMeteringMultiplier = multiplier;
                m_HANInfo.Table2106.SimpleMeteringDivisor = divisor;

                Response = m_HANInfo.Table2106.WriteSimpleMeteringMultiplierAndDivisor();
            }

            return Response;
        }

        /// <summary>
        /// Runs the HAN Reset Test 1 - Joined Device
        /// </summary>
        /// <param name="startDelay">The number of seconds to wait before resetting</param>
        /// <returns>The result of the reset</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        internal ProcedureResultCodes ResetHANTestJoinedDevice(uint startDelay)
        {
            return ResetHAN(HANResetMethod.TestOneJoinedDevice, startDelay, 0);
        }

        /// <summary>
        /// Runs the HAN Reset Test 2 - Code Detectable Reset
        /// </summary>
        /// <param name="startDelay">The number of seconds to wait before resetting</param>
        /// <param name="errorCode">The error code to test</param>
        /// <returns>The result of the reset</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        public ProcedureResultCodes ResetHANTestCodeDetectableReset(uint startDelay, uint errorCode)
        {
            return ResetHAN(HANResetMethod.TestCodeDetectedReset, startDelay, errorCode);
        }

        /// <summary>
        /// Runs the HAN Reset Test 3 - ZigBee Stack Watchdog
        /// </summary>
        /// <param name="startDelay">The number of seconds to wait before resetting</param>
        /// <returns>The result of the reset</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        public ProcedureResultCodes ResetHANTestZigBeeStackWatchdog(uint startDelay)
        {
            return ResetHAN(HANResetMethod.TestZigBeeStackTaskWatchdog, startDelay, 0);
        }

        /// <summary>
        /// Runs the HAN Reset Test 4 - ZigBee Task Watchdog
        /// </summary>
        /// <param name="startDelay">The number of seconds to wait before resetting</param>
        /// <returns>The result of the reset</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        public ProcedureResultCodes ResetHANTestZigBeeTaskWatchdog(uint startDelay)
        {
            return ResetHAN(HANResetMethod.TestZigBeeTaskWatchdog, startDelay, 0);
        }

        /// <summary>
        /// Runs the HAN Reset Test 5 - Processor Fault
        /// </summary>
        /// <param name="startDelay">The number of seconds to wait before resetting</param>
        /// <returns>The result of the reset</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        public ProcedureResultCodes ResetHANTestProcessorFault(uint startDelay)
        {
            return ResetHAN(HANResetMethod.TestProcessorFault, startDelay, 0);
        }

        /// <summary>
        /// Runs the HAN Reset Test 6 - Periodic Reset
        /// </summary>
        /// <param name="frequency">The number of seconds between resets. This should be set to 10s or more.</param>
        /// <returns>The result of the reset</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        public ProcedureResultCodes ResetHANTestPeriodicReset(uint frequency)
        {
            return ResetHAN(HANResetMethod.TestPeriodicReset, frequency, 0);
        }

        /// <summary>
        /// Runs the HAN Reset Test 7 - First Use Reset
        /// </summary>
        /// <param name="startDelay">The number of seconds to wait before the start of the test</param>
        /// <returns>The result of the procedure call</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        public ProcedureResultCodes ResetHANFirstUseReset(uint startDelay)
        {
            return ResetHAN(HANResetMethod.TestFirstUseReset, startDelay, 0);
        }

        /// <summary>
        /// Disables any active HAN Reset Tests
        /// </summary>
        /// <param name="startDelay">The number of seconds to wait before resetting the meter</param>
        /// <returns>The result of the procedure call</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        public ProcedureResultCodes DisableResetHANTest(uint startDelay)
        {
            return ResetHAN(HANResetMethod.DisableTest, startDelay, 0);
        }

        /// <summary>
        /// Clears the HAN Reset Log
        /// </summary>
        /// <returns>The result of the procedure call</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        public ProcedureResultCodes ClearHANResetLogs()
        {
            return ResetHAN(HANResetMethod.ClearResetLog, 0xF2, 0xF3);
        }

        /// <summary>
        /// Cancels the scheduled firmware activation.
        /// </summary>
        /// <returns>
        /// An ProcedureResultCodes representing the result of the operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/28/12 JKW        		Created 
        //
        public virtual ProcedureResultCodes CancelScheduledFirmwareActivation(ProcedureFirmwareType firmwareType)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam1 = new byte[1];
            ProcParam1[0] = (byte)firmwareType;
            byte[] ProcResponse;

            ProcResult = ExecuteProcedure(Procedures.CANCEL_SCHEDULED_FIRMWARE_ACTIVATION, ProcParam1, out ProcResponse);
            return ProcResult;
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

        internal Quantity GetQuantityFromStandardTables(LID energyLID, LID demandLID, string quantityDescription, RegisterDataRecord registers)
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
                    FoundQuantity.TotalMaxDemand.TimeOfOccurrence = TimeOfOccurance;

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
                            FoundQuantity.TOUMaxDemand[iRate].TimeOfOccurrence = TimeOfOccurance;

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
        /// Converts a WritePendingTOUResult to a TOUReconfigResult.
        /// </summary>
        /// <param name="Result">A WritePendingTOUResult</param>
        /// <returns>A TOUReconfigResult.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/21/13 jrf 3.50.07 TQ9523 Created

        internal static TOUReconfigResult ConvertWritePendingTOUResult(WritePendingTOUResult Result)
        {
            TOUReconfigResult ConvertedResult = TOUReconfigResult.ERROR;

            switch (Result)
            {
                case WritePendingTOUResult.SUCCESS:
                    {
                        ConvertedResult = TOUReconfigResult.SUCCESS;
                        break;
                    }
                case WritePendingTOUResult.FILE_NOT_FOUND:
                    {
                        ConvertedResult = TOUReconfigResult.FILE_NOT_FOUND;
                        break;
                    }
                case WritePendingTOUResult.INVALID_EDL_FILE:
                    {
                        ConvertedResult = TOUReconfigResult.ERROR_TOU_NOT_VALID;
                        break;
                    }
                case WritePendingTOUResult.PENDING_BUFFERS_FULL:
                    {
                        ConvertedResult = TOUReconfigResult.PENDING_BUFFERS_FULL;
                        break;
                    }
                case WritePendingTOUResult.PROTOCOL_ERROR:
                    {
                        ConvertedResult = TOUReconfigResult.PROTOCOL_ERROR;
                        break;
                    }
                case WritePendingTOUResult.INSUFFICIENT_SECURITY_ERROR:
                    {
                        ConvertedResult = TOUReconfigResult.INSUFFICIENT_SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        ConvertedResult = TOUReconfigResult.ERROR;
                        break;
                    }
            }

            return ConvertedResult;
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
        /// Gets the list of Scheduled Events
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        public List<ScheduledEvent> ScheduledEvents
        {
            get
            {
                List<ScheduledEvent> Events = new List<ScheduledEvent>();

                if (Table2123 != null)
                {
                    Events = Table2123.ScheduledEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Creates the LID object for specified LID number
        /// </summary>
        /// <param name="uiLIDNumber">The LID number to create</param>
        /// <returns>The object that represents the specified LID</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/21/11 RCG	2.45.24		   Allowing the LID type to be changed to fix issue with Ins VA and Var values

        public override LID CreateLID(uint uiLIDNumber)
        {
            DefinedLIDs Defined = new DefinedLIDs();
            LID CreatedLID = new CentronAMILID(uiLIDNumber);

            // There is a case where the Data Type returned data type for the Instantaneous VA and Var
            // values returned a Uint32 rather than a Single so we need to make sure we interpret those
            // correctly
            if (Defined.INST_VA_ARITH.lidValue == uiLIDNumber
                || Defined.INST_VA_VECT.lidValue == uiLIDNumber
                || Defined.INST_VAR.lidValue == uiLIDNumber
                || Defined.INST_VA_LAG.lidValue == uiLIDNumber)
            {
                // Returned as a UINT between 3.0.140 and 3.7
                if ((VersionChecker.CompareTo(FWRevision, VERSION_3) > 0
                    || (VersionChecker.CompareTo(FWRevision, VERSION_3) == 0 && FirmwareBuild >= 140))
                    && VersionChecker.CompareTo(FWRevision, VERSION_HYDROGEN_3_7) < 0)
                {
                    CreatedLID.lidType = TypeCode.UInt32;
                }
            }

            return CreatedLID;
        }

        #endregion

        #region Protected Methods

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

        /// <summary>
        /// Gets the demand reset value for var d
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/15/13 jkw 2.70.69 322498 Created
        //
        protected override Quantity DRVarDelivered(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_DEL, m_LID.DEMAND_MAX_VAR_DEL,
                    "Var Delivered", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_DEL, m_LID.DEMAND_MAX_VAR_DEL,
                    "Var Delivered", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;

        }

        /// <summary>
        /// Gets the demand reset value for var r
        /// </summary>
        /// <param name="uiIndex">The index of the demand reset to get.</param>
        /// <returns>The demand reset quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/15/13 jkw 2.70.69 322498 Created
        //
        protected override Quantity DRVarReceived(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_REC, m_LID.DEMAND_MAX_VAR_REC,
                    "Var Received", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_REC, m_LID.DEMAND_MAX_VAR_REC,
                    "Var Received", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;

        }

        /// <summary>
        /// Provides access to the DR Var Net Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/22/14 jrf 3.50.81 489923 Created
        protected override Quantity DRVarNet(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET, m_LID.DEMAND_MAX_VAR_NET,
                    "Var Net", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET, m_LID.DEMAND_MAX_VAR_NET,
                    "Var Net", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Provides access to the DR Var Net Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/22/14 jrf 3.50.81 489923 Created
        protected override Quantity DRVarNetDelivered(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET_DEL, m_LID.DEMAND_MAX_VAR_NET_DEL,
                    "Var Net Delivered", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET_DEL, m_LID.DEMAND_MAX_VAR_NET_DEL,
                    "Var Net Delivered", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Provides access to the DR Var Net Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/22/14 jrf 3.50.81 489923 Created
        protected override Quantity DRVarNetReceived(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET_REC, m_LID.DEMAND_MAX_VAR_NET_REC,
                    "Var Net Received", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_NET_REC, m_LID.DEMAND_MAX_VAR_NET_REC,
                    "Var Net Received", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Provides access to the DR Var Quadrant 1 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/22/14 jrf 3.50.81 489923 Created
        protected override Quantity DRVarQuadrant1(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q1, m_LID.DEMAND_MAX_VAR_Q1,
                    "Var Quadrant 1", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q1, m_LID.DEMAND_MAX_VAR_Q1,
                    "Var Quadrant 1", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Provides access to the DR Var Quadrant 2 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/22/14 jrf 3.50.81 489923 Created
        protected override Quantity DRVarQuadrant2(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q2, m_LID.DEMAND_MAX_VAR_Q2,
                    "Var Quadrant 2", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q2, m_LID.DEMAND_MAX_VAR_Q2,
                    "Var Quadrant 2", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Provides access to the DR Var Quadrant 3 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/22/14 jrf 3.50.81 489923 Created
        protected override Quantity DRVarQuadrant3(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q3, m_LID.DEMAND_MAX_VAR_Q3,
                    "Var Quadrant 3", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q3, m_LID.DEMAND_MAX_VAR_Q3,
                    "Var Quadrant 3", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
        }

        /// <summary>
        /// Provides access to the DR Var Quadrant 4 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/22/14 jrf 3.50.81 489923 Created
        protected override Quantity DRVarQuadrant4(uint uiIndex)
        {
            Quantity DRQuantity = null;

            if (uiIndex == 0)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q4, m_LID.DEMAND_MAX_VAR_Q4,
                    "Var Quadrant 4", Table25.DemandResetRegisterData);
            }

#if (!WindowsCE)
            if (uiIndex == 1)
            {
                DRQuantity = GetQuantityFromStandardTables(m_LID.ENERGY_VARH_Q4, m_LID.DEMAND_MAX_VAR_Q4,
                    "Var Quadrant 4", Table2053.DemandResetRegisterData);
            }
#endif
            return DRQuantity;
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
        /// Configures the meter to use the specified base energies
        /// </summary>
        /// <param name="baseEnergies">The base energy values to use.</param>
        /// <returns>The result of the procedure call</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/22/11 RCG 2.50.12 N/A    Created

        public virtual ProcedureResultCodes ConfigureBaseEnergies(List<BaseEnergies> baseEnergies)
        {
            ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcResponse = null;
            byte[] ProcParam = new byte[1];

            // Validate the energies parameter
            if (baseEnergies == null)
            {
                throw new ArgumentNullException("baseEnergies");
            }
            else if (baseEnergies.Count > ProcParam.Length)
            {
                throw new ArgumentException("Only " + ProcParam.Length.ToString(CultureInfo.InvariantCulture) + " energies may be configured", "baseEnergies");
            }

            // Add the energy values
            for (int iIndex = 0; iIndex < baseEnergies.Count; iIndex++)
            {
                ProcParam[iIndex] = (byte)baseEnergies[iIndex];
            }

            Result = ExecuteProcedure(Procedures.CONFIGURE_BASE_ENERGIES, ProcParam, out ProcResponse);

            // Give the meter a second to make sure that the base changes before we do anything else
            Thread.Sleep(1000);

            return Result;
        }

        /// <summary>
        /// Configures the base to use the correct energies if the base supports Energy Configuration
        /// </summary>
        /// <param name="programFile">The program file to use to set the base energies</param>
        /// <returns>The result of the operation</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/22/11 RCG 2.50.12 N/A    Created
        // 05/10/11 jrf 2.50.43        Adding retry to configure base energies procedure since
        //                             it will fail occasionally when the meter is busy.
        // 05/13/11 jrf 2.50.44        Amending previous change to add a couple second wait before retrying.
        // 11/16/15 jrf 4.22.05 632339 Added check of selected secondary quantity in config 
        //                             if no secondary energy is configured.
        protected ProcedureResultCodes ConfigureBase(string programFile)
        {
            ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
            bool SupportsCurrentEnergyConfig = false;
            List<BaseEnergies> ConfiguredEnergies = null;
            List<BaseEnergies> RequiredEnergies = null;
            BaseEnergies SelectedSecondaryQuantity = BaseEnergies.Unknown;

            // First make sure that the meter supports energy configuration
            Result = ValidateBaseEnergies(out SupportsCurrentEnergyConfig, out ConfiguredEnergies);

            if (Result == ProcedureResultCodes.COMPLETED)
            {
                // Get the list of required energies from the program file.
                RequiredEnergies = GetRequiredEnergiesFromProgram(programFile);
                SelectedSecondaryQuantity = GetSelectedSecondaryQuantityFromProgram(programFile);

                //If we find a selected secondary quantity, add it to required list if it isn't there already.
                if (SelectedSecondaryQuantity != BaseEnergies.Unknown 
                    && RequiredEnergies.Contains(SelectedSecondaryQuantity) == false)
                {
                    RequiredEnergies.Add(SelectedSecondaryQuantity);
                }

                if (CheckForValidEnergyConfig(ConfiguredEnergies, RequiredEnergies) == false)
                {
                    // The current base configuration will not work so we need to change it
                    if (RequiredEnergies.Count <= NumberofBaseConfigurableEnergies)
                    {
                        Result = ConfigureBaseEnergies(RequiredEnergies);

                        //This procedure occasionally fails with this result code when meter is busy.
                        //Adding a retry to see if this corrects issue.  If not, this may need to be 
                        //addressed in the firmware.
                        if (ProcedureResultCodes.INVALID_PARAM == Result)
                        {
                            Thread.Sleep(2000);
                            Result = ConfigureBaseEnergies(RequiredEnergies);
                        }
                    }
                    else
                    {
                        // This program will never be able to be configured since the base can't support all of the energies
                        Result = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
                    }
                }
                else
                {
                    // We don't have to change the configuration so we are done
                    Result = ProcedureResultCodes.COMPLETED;
                }
            }

            return Result;
        }

        /// <summary>
        /// Checks to see if the current base configuration is valid for a specific register configuration
        /// </summary>
        /// <param name="BaseConfig">The current base configuration</param>
        /// <param name="RegisterConfig">The requested register configuration</param>
        /// <returns>True if the current base configuration is valid. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/22/11 RCG 2.50.12 N/A    Created

        protected bool CheckForValidEnergyConfig(List<BaseEnergies> BaseConfig, List<BaseEnergies> RegisterConfig)
        {
            bool CurrentEnergyConfigValid = true;

            foreach (BaseEnergies CurrentEnergy in RegisterConfig)
            {
                if (BaseConfig.Contains(CurrentEnergy) == false)
                {
                    CurrentEnergyConfigValid = false;
                }
            }

            return CurrentEnergyConfigValid;
        }
        

        /// <summary>
        /// Resets the HAN
        /// </summary>
        /// <param name="resetMethod">The method that should be used to reset</param>
        /// <param name="param2">Parameter 2 (varies based on method used)</param>
        /// <param name="param3">Parameter 3 (varies based on method used)</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/11 RCG 2.50.26        Created.

        protected ProcedureResultCodes ResetHAN(HANResetMethod resetMethod, uint param2, uint param3)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;

            byte[] ProcResponse;
            MemoryStream ProcParams = new MemoryStream(new byte[12]);
            PSEMBinaryWriter ParamWriter = new PSEMBinaryWriter(ProcParams);

            ParamWriter.Write((uint)resetMethod);
            ParamWriter.Write(param2);
            ParamWriter.Write(param3);

            ProcResult = ExecuteProcedure(Procedures.HAN_RESET, ProcParams.ToArray(), out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Gets the current security level from the meter.
        /// </summary>
        /// <param name="level">The current security level</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/22/09 RCG 2.30.13	    Created

        protected virtual ProcedureResultCodes GetSecurityLevel(out SecurityLevel level)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[0];

            level = SecurityLevel.Level0;

            ProcResult = ExecuteProcedure(Procedures.GET_SECURITY_LEVEL, ProcParam, out ProcResponse);

            if (ProcResult == ProcedureResultCodes.COMPLETED && ProcResponse.Length > 0)
            {
                level = (SecurityLevel)ProcResponse[0];
            }

            return ProcResult;
        }

        /// <summary>
        /// Perform a Ping HAN Device
        /// </summary>
        /// <param name="eui64">Destination HAN device</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/12/12 DC  ?.??.??        Created.
        //
        public ProcedureResultCodes PingHANDevice(UInt64 eui64)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;

            byte[] ProcResponse;
            MemoryStream ProcParams = new MemoryStream(new byte[8]);
            PSEMBinaryWriter ParamWriter = new PSEMBinaryWriter(ProcParams);

            ParamWriter.Write(eui64);
            ProcResult = ExecuteProcedure(Procedures.PING_HAN_DEVICE, ProcParams.ToArray(), out ProcResponse);

            return ProcResult;
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
        /// Reads the Firmware Loader Version from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/10 RCG 2.40.07 N/A    Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 03/16/16 AF  4.50.236 662058 Read the fw loader version from mfg table 60 if supported. If not supported,
        //                              then read the version from mfg procedure 23 if supported. If neither are supported,
        //                              the user will see 0.0.0 which should alert them that it's not right.
        //
        protected virtual ProcedureResultCodes ReadFWLoaderVersion()
        {
            ProcedureResultCodes Result = ProcedureResultCodes.NOT_FULLY_COMPLETED;

            if ((Table2108 != null) && (Table2108.FWLoaderVersionOnly != null) && (Table2108.FWLoaderRevisionOnly != null) && (Table2108.FWLoaderBuildOnly != null))
            {
                m_FWLoaderVersion.Value = Table2108.FWLoaderVersionOnly.Value;
                m_FWLoaderRevision.Value = Table2108.FWLoaderRevisionOnly.Value;
                m_FWLoaderBuild.Value = Table2108.FWLoaderBuildOnly.Value;
            }
            else
            {
                if (Table00.IsProcedureUsed(2071))
                {
                    byte[] byData;

                    if (VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_3_0) >= 0)
                    {
                        Result = ReadMemory(FW_LOADER_MAXIMA_OFFSET, out byData);
                    }
                    else if (VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_2_0) >= 0)
                    {
                        // ARM based meters store the FW Loader version in 0x100042
                        Result = ReadMemory(FW_LOADER_ARM_OFFSET, out byData);
                    }
                    else
                    {
                        // M16C meters store this at 0xFC002
                        Result = ReadMemory(FW_LOADER_M16C_OFFSET, out byData);
                    }

                    if (Result == ProcedureResultCodes.COMPLETED)
                    {
                        m_FWLoaderVersion.Value = byData[0];
                        m_FWLoaderRevision.Value = byData[1];
                        m_FWLoaderBuild.Value = byData[2];
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Writes the Header information to the Core Dump file
        /// </summary>
        /// <param name="writer">The Binary writer to the Core Dump file</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/18/10 RCG 2.40.07 N/A    Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        protected virtual void WriteCoreDumpHeader(PSEMBinaryWriter writer)
        {
            CallingAppInfo AppInfo = new CallingAppInfo();
            string strAppID = AppInfo.ProductName + " ver. " + AppInfo.Version;
            uint uiRFLANMAC = 0;
            byte byRFLANFWLoaderRevision = 0;
            byte byRFLANFWLoaderBuild = 0;

            writer.Seek(0, SeekOrigin.Begin);

            // Write the header information
            writer.Write("OpenWay CoreDump Version 1.0", 60);

            // Write the Register Information
            writer.Write(true); // Have Version Information
            writer.Write(Table2108.RegVersionOnly); // FW Version
            writer.Write(Table2108.RegRevisionOnly); // FW Revision
            writer.Write(Table2108.RegBuildOnly); // FW Build
            writer.Write(Table01.HWVersionOnly); // HW Version
            writer.Write(Table01.HWRevisionOnly); // HW Revision
            writer.Write(DeviceClass, 4); // Device Class

            // This seems to be what is always written for this.
            writer.Write((byte)3); // Core Dump Type - PSEM Tool Core Dump

            writer.Write(VersionChecker.CompareTo(HWRevision, HW_VERSION_1_5) >= 0); // Is 512k

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
            writer.Write(Table2108.CommModuleTypeByte); // Comm Module Type
            writer.Write(Table2108.CommVersionOnly); // Comm Module FW Version
            writer.Write(Table2108.CommRevisionOnly); // Comm Module FW Revision
            writer.Write(Table2108.CommBuildOnly); // Comm Module FW Build
            writer.Write(Table2108.HANModuleTypeByte); // HAN Module Type
            writer.Write(Table2108.HANVersionOnly); // HAN FW Version
            writer.Write(Table2108.HANRevisionOnly); // HAN FW Revision
            writer.Write(Table2108.HANBuildOnly); // HAN FW Build
            writer.Write(Table2108.DisplayVersionOnly); // Display FW Version
            writer.Write(Table2108.DisplayRevisionOnly); // Display FW Revision
            writer.Write(Table2108.DisplayBuildOnly); // Display FW Build

            writer.Write(FWLoaderVersion); // Loader Version
            writer.Write(FWLoaderRevision); // Loader Revision
            writer.Write(FWLoaderBuild); // Loader Build

            writer.Write((byte)0); // RFLAN Loader Version - Always 0

            try
            {
                byRFLANFWLoaderRevision = Table2119.RFLANFWLoaderRevision;
                byRFLANFWLoaderBuild = Table2119.RFLANFWLoaderBuild;
            }
            catch (Exception)
            {
                // Just use 0 if we can't get the value
            }

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

        /// <summary>
        /// Writes the tables portion of the Core Dump for all meter types.
        /// </summary>
        /// <param name="writer">The Binary Writer for the Core Dump file.</param>
        /// <returns>The result of the operation</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/18/10 RCG 2.40.07 N/A    Created
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  10/31/12 jrf 2.70.34 238199 Replacing read of table 2080 with pre-core dump 
        //                              data from table 2127 for HW 3.0 and greater meters.
        //  03/30/16 AF  4.50.240 670673 The pre-core dump data is in 2273 starting with fw versions 6.0
        //                               for HW 3.0 and above meters and 2127 is inaccessible starting
        //                               with 7.0 builds
        //
        protected virtual ItronDeviceResult WriteCoreDumpTables(PSEMBinaryWriter writer)
        {
            byte[] CoreDumpData;
            uint uiOffset = 0;
            uint uiCoreDumpSize = DetermineFullCoreDumpLength();
            PSEMResponse Response = PSEMResponse.Ok;
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            if (HWRevisionFiltered < HW_VERSION_3_0)
            {
                // Write 2080 first (Mini Core Dump or LCD Diagnostics)
                // This table should be 256 bytes.
                Response = m_PSEM.FullRead(2080, out CoreDumpData);

            }
            else //HW 3.0 and greater does not use table 2080, however it does have a useful pre-core dump
            {
                if (VersionChecker.CompareTo(FWRevision, VERSION_6_0_MICHIGAN) >= 0)
                {
                    Response = m_PSEM.FullRead(2273, out CoreDumpData);
                }
                else
                {
                    int iPreCoreDumpDataflashPage = PRE_CORE_DUMP_DATAFLASH_PAGE;

                    if (0 == VersionChecker.CompareTo(VERSION_LITHIUM_3_12, FWRevision)
                        && FirmwareBuild >= 5 && FirmwareBuild <= 26)
                    {
                        //Lithium firmware versions 3.12.5 - 3.12.26 put pre-core dump in alternate location
                        iPreCoreDumpDataflashPage = LITHIUM_ALT_PRE_CORE_DUMP_DATAFLASH_PAGE;
                    }

                    Response = m_PSEM.OffsetRead(2127, iPreCoreDumpDataflashPage * DATAFLASH_PAGE_SIZE, PRE_CORE_DUMP_SIZE, out CoreDumpData);
                }
            }

            if (Response == PSEMResponse.Ok)
            {
                writer.Write(CoreDumpData);
            }

            if (Response != PSEMResponse.Isss && Response != PSEMResponse.Isc)
            {
                // It's possible that this read might fail but we don't want to make this
                // stop us from creating a core dump file. Of course if it's Invalid Service
                // Sequence State or a Security error there is not much we can do.
                Response = PSEMResponse.Ok;
            }

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

        /// <summary>
        /// Creates a list of tables to read from the meter.
        /// </summary>
        /// <param name="IncludedSections">EDL Sections to include</param>
        /// <returns>The list of tables to read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/21/13 DLG 3.50.07 No WR    Overriding here to handle only tables related to CENTRON.
        //  11/25/13 jrf 3.50.08 TQ9527   Including 25 Year TOU schedule table when appropriate.
        //  12/02/13 jrf 3.50.10 TQ9527   Refactored inclusion of 25 Year TOU schedule table to 
        //                                OpenWayITRDBridge/OpenWayAdvPolyITRFBridge.
        //  05/18/16 AF  4.50.269 685402  Add the max demands table (2175) if supported in the meter
        protected override List<ushort> GetTablesToRead(EDLSections IncludedSections)
        {
            List<ushort> TableList = base.GetTablesToRead(IncludedSections);

            TableList.Add(3);           // Mode and Status
            TableList.Add(4);           // Pending Status
            TableList.Add(11);          // Actual Sources
            TableList.Add(12);          // Unit of Measure
            TableList.Add(13);          // Demand Control
            TableList.Add(14);          // Data Control
            TableList.Add(15);          // Constant
            TableList.Add(16);          // Source Definition
            TableList.Add(21);          // Actual Registers
            TableList.Add(22);          // Data selection
            TableList.Add(23);          // Current Register Data
            TableList.Add(25);          // Previous Demand Reset Data
            TableList.Add(26);          // Self Read Data
            TableList.Add(27);          // Present Register Selection
            TableList.Add(28);          // Present Register Data
            TableList.Add(51);          // Actual Time and TOU
            TableList.Add(54);          // Calendar
            TableList.Add(55);          // Clock State

            if ((IncludedSections & EDLSections.LoadProfile) == EDLSections.LoadProfile)
            {
                TableList.Add(61);      // Actual Load Profile
                TableList.Add(62);      // Load Profile Control
                TableList.Add(63);      // Load Profile status
                TableList.Add(64);      // Load Profile data set 1
            }

            if ((IncludedSections & EDLSections.HistoryLog) == EDLSections.HistoryLog)
            {
                TableList.Add(71);      // Actual Log
                TableList.Add(72);      // Events Identification
                TableList.Add(73);      // History Logger Control
                TableList.Add(74);      // History Logger Data
                TableList.Add(75);      // Event logger control
                TableList.Add(76);      // Event logger data
            }

            TableList.Add(2048);
            TableList.Add(2062);        // C12.22 Status Table

            if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            {
                TableList.Add(2064);    //Comm Module General Config Table

                if (CommModule is RFLANCommModule)
                {
                    TableList.Add(2078);    // RFLAN Neighbor Table
                }
            }

            TableList.Add(2090);        // Calendar Config

            if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            {
                TableList.Add(2100);    // HAN Client Configuration
                TableList.Add(2103);    // HAN Recieve Data Table
            }

            TableList.Add(2108);        // MCU Information
            TableList.Add(2138);        // Dimension Limiting Disconnect Switch Table
            TableList.Add(2139);        // Actual Limiting Disconnect Switch Table
            TableList.Add(2140);        // Disconnect Switch Status Table
            TableList.Add(2141);        // Disconnect Seitch Configuration Table
            TableList.Add(2142);        // Disconnect Override Table
            TableList.Add(2143);        // Service Limiting Failsafe Table
            TableList.Add(2148);        // Voltage Monitoring Dimension Limiting Table
            TableList.Add(2149);        // Voltage Monitoring Actual Limiting Table
            TableList.Add(2150);        // Voltage Monitoring Control Table
            TableList.Add(2151);        // Voltage Monitoring Status Table

            if ((IncludedSections & EDLSections.VoltageMonitoring) == EDLSections.VoltageMonitoring)
            {
                TableList.Add(2152);    // Voltage Monitoring Data Set Table 
            }

            TableList.Add(2153);        // Extended Voltage Monitoring Actual Limiting Table
            TableList.Add(2154);        // Extended Voltage Monitoring Control Table
            TableList.Add(2155);        // Extended Voltage Monitoring Status Table
            TableList.Add(2156);        // Extended Voltage Monitoring Extended Status Table

            if ((IncludedSections & EDLSections.VoltageMonitoring) == EDLSections.VoltageMonitoring)
            {
                TableList.Add(2157);    // Extended Voltage Monitoring Data Set Table 
            }

            TableList.Add(2158);        // Communications Log Dimension Limiting Table
            TableList.Add(2159);        // Communications Log Actual Limiting Table
            TableList.Add(2160);        // Communications Events Identification Table
            TableList.Add(2161);        // Communications Log Control Table
            TableList.Add(2163);        // HAN Communications Log Control Table

            if ((IncludedSections & EDLSections.LANandHANLog) == EDLSections.LANandHANLog)
            {
                TableList.Add(2162);    // LAN Log Data Table
                TableList.Add(2164);    // HAN Log Data Table
            }

            TableList.Add(2168);        // Meter Swap Out Table

            if (TwelveMaxDemandsSupported)
            {
                TableList.Add(2175);    // Max Demand Rcd
            }

            TableList.Add(2193);        // Enhanced Security Table
            TableList.Add(2220);        // Factory Data Info Table
            TableList.Add(2260);        // SR 3.0 Config Table
            TableList.Add(2261);        // Fatal Error Recovery Status Table

            //Temporary solution to table 2262 timing out when trying to read it in non-HW3.0 meter.  
            //Accelerometer tables are only valid for HW 3.0 and up anyway.  
            if (0 <= Utilities.VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_3_0))
            {
                TableList.Add(2262);    // Tamper/Tap Status Table
                TableList.Add(2263);    // Tamper/Tap Data Table
            }

            TableList.Add(2264);        // Program State Table
            TableList.Add(2368);        // Power Monitor Dimension Table
            TableList.Add(2369);        // Power Monitor Control Table
            TableList.Add(2370);        // Power Monitor Data Table
            TableList.Add(2379);        // Actual Firmware Download Event Log Table

            if ((IncludedSections & EDLSections.HistoryLog) == EDLSections.HistoryLog)
            {
                TableList.Add(2382);    // Firmware Download Log Data Table
            }

            TableList.Add(2383);        // Firmware Download CRC Table

            TableList.Add(2389);        // MFG Actual Sources Table
            TableList.Add(2390);        // MFG Unit of Measure Table
            TableList.Add(2391);        // MFG Demand Control Table
            TableList.Add(2392);        // MFG Data Control Table
            TableList.Add(2393);        // MFG Constants Table
            TableList.Add(2394);        // MFG Source definition

            if ((IncludedSections & EDLSections.LoadProfile) == EDLSections.LoadProfile
                || (IncludedSections & EDLSections.InstrumentationProfile) == EDLSections.InstrumentationProfile)
            {
                TableList.Add(2409);    //Actual Mfg. Profile 
                TableList.Add(2410);    //Mfg. Profile Control
                TableList.Add(2411);    //Mfg. Profile Status
            }

            if ((IncludedSections & EDLSections.LoadProfile) == EDLSections.LoadProfile
                && true == ExtLoadProfileSupported && ExtendedLoadProfileCurrentNumberOfBlocks > 0)
            {
                TableList.Add(2412);    // Mfg. Extended Load Profile
            }

            if ((IncludedSections & EDLSections.InstrumentationProfile) == EDLSections.InstrumentationProfile
                && true == InstrumentationProfileSupported && InstrumentationProfileCurrentNumberOfChannels > 0)
            {
                TableList.Add(2413);    // Mfg. Instrumentation Profile
                TableList.Add(2417);    // Mfg. Instrumentation Profile Extended Status
            }

            TableList.Add(2419);        //Actual Extended Self Read table
            TableList.Add(2420);        //Extended Self Read Control table
            TableList.Add(2421);        //Extended Self Read Status table
            TableList.Add(2422);        //Extended Current Register Data table
            TableList.Add(2423);        //Extended Self Read Data table            

            TableList.Add(2439);        // Actual HAN RIB Limiting Table
            TableList.Add(2440);        // Active Block Pricing Schedule
            TableList.Add(2441);        // Next Block Pricing Schedule
            TableList.Add(2442);        // RIB Stats Table

            return TableList;
        }

        /// <summary>
        /// Reads Table 2412 from the meter.
        /// </summary>
        /// <param name="MeterTables">The table object for the meter.</param>
        /// <returns>The PSEM response code.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/11 RCG 2.53.20 N/A    Created
        //
        protected PSEMResponse ReadTable2412(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] byaData;
            byte DataSetIndex = 0;

            CentronTables TempTables = new CentronTables();
            uint uiMaxOffsetReadBytes;
            uint uiReadMemorySize;
            uint uiNumberOfReads;
            uint uiBytesToRead;
            uint uiCurrentOffset;
            uint uiBlockOffset;
            uint uiBlockLength;
            uint uiMaxBlocksToRead;
            uint uiMaxBytesToRead;
            ushort usValidBlocks;
            ushort usNumberIntervals;

            ushort usNewValidBlocks;
            ushort usNewNumberIntervals;
            ushort usNewLastBlock;
            int iBlockToRead;

            // This must be initialized to false or you will break the retry logic.
            bool bBlocksReRead = false;

            object objData;

            // Since Load Profile can be very large (144k) it may not be able
            // to be read completely when doing a full read so we need to break
            // it up into multiple offset reads. Table 61 must be read prior to this.

            if (MeterTables.IsAllCached(2411) == true)
            {
                uiMaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                if (uiMaxOffsetReadBytes > ushort.MaxValue)
                {
                    uiMaxOffsetReadBytes = ushort.MaxValue;
                }

                MeterTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, DataSetIndex), null, out objData);
                usValidBlocks = (ushort)objData;

                MeterTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_INT, DataSetIndex), null, out objData);
                usNumberIntervals = (ushort)objData;

                // Determine the size of a Load Profile data block
                MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl364_LP_DATA_SETS, new int[] { 0 },
                    out uiBlockOffset, out uiBlockLength);

                // Determine how many blocks can be read in an offset read
                uiMaxBlocksToRead = uiMaxOffsetReadBytes / uiBlockLength;
                uiMaxBytesToRead = uiMaxBlocksToRead * uiBlockLength;

                // Determine total amount to read
                uiReadMemorySize = usValidBlocks * uiBlockLength;

                // Determine how many reads need to be done
                uiNumberOfReads = usValidBlocks / uiMaxBlocksToRead;

                // Add in a read for any remaining data
                if (usValidBlocks % uiMaxBlocksToRead > 0)
                {
                    uiNumberOfReads++;
                }

                uiCurrentOffset = 0;

                for (uint iIndex = 0; iIndex < uiNumberOfReads && PSEMResult == PSEMResponse.Ok; iIndex++)
                {
                    uiBytesToRead = uiReadMemorySize - uiCurrentOffset;

                    if (uiBytesToRead > uiMaxBytesToRead)
                    {
                        uiBytesToRead = uiMaxBytesToRead;
                    }

                    PSEMResult = m_PSEM.OffsetRead(2412, (int)uiCurrentOffset, (ushort)uiBytesToRead, out byaData);

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMDataStream = new MemoryStream(byaData);
                        MeterTables.SavePSEMStream(2412, PSEMDataStream, uiCurrentOffset);
                        uiCurrentOffset += uiBytesToRead;
                    }

                    OnStepProgress(new ProgressEventArgs());
                }

                // Reread table 63 and make sure no new intervals have occurred while reading
                CentronTables.CopyTable(0, MeterTables, TempTables);
                CentronTables.CopyTable(1, MeterTables, TempTables);
                CentronTables.CopyTable(2409, MeterTables, TempTables);
                CentronTables.CopyTable(2410, MeterTables, TempTables);
                CentronTables.CopyTable(2411, MeterTables, TempTables);

                do
                {
                    ReadTable(2411, ref TempTables);

                    TempTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, DataSetIndex), null, out objData);
                    usNewValidBlocks = (ushort)objData;

                    TempTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_INT, DataSetIndex), null, out objData);
                    usNewNumberIntervals = (ushort)objData;

                    if (usNewNumberIntervals != usNumberIntervals || usNewValidBlocks != usValidBlocks)
                    {
                        // This will limit us to only two tries at this. (if it is already true it will be set
                        // to false which means we won't try this again.)
                        bBlocksReRead = !bBlocksReRead;

                        // A new interval has occurred so we need to reread at least one block
                        // We only want to copy the information for this data set in order to avoid corrupting other profile data sets
                        uint StatusOffset = 0;
                        uint StatusLength = 0;
                        TempTables.GetFieldOffset((long)CentronTblEnum.MfgTbl363_LP_STATUS_SET1, null, out StatusOffset, out StatusLength);
                        MemoryStream StatusStream = new MemoryStream(new byte[StatusLength]);
                        TempTables.BuildPSEMStream(2411, StatusStream, StatusOffset, StatusLength);
                        MeterTables.SavePSEMStream(2411, StatusStream, StatusOffset);

                        MeterTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_LAST_BLOCK_ELEMENT, DataSetIndex), null, out objData);
                        usNewLastBlock = (ushort)objData;

                        // Determine the offset of the block
                        iBlockToRead = (int)usNewLastBlock;
                        MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl364_LP_DATA_SETS, new int[] { iBlockToRead },
                            out uiBlockOffset, out uiBlockLength);

                        PSEMResult = m_PSEM.OffsetRead(2412, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(byaData);
                            MeterTables.SavePSEMStream(2412, PSEMDataStream, uiBlockOffset);

                            // Now if there was also a new block we need to reread the previous block as well.
                            if (usNewValidBlocks != usValidBlocks)
                            {
                                if (usNewLastBlock - 1 < 0)
                                {
                                    iBlockToRead = usNewValidBlocks - 1;
                                }
                                else
                                {
                                    iBlockToRead = usNewLastBlock - 1;
                                }

                                // Determine the offset of the block
                                MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl364_LP_DATA_SETS, new int[] { iBlockToRead },
                                    out uiBlockOffset, out uiBlockLength);

                                PSEMResult = m_PSEM.OffsetRead(2412, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                                if (PSEMResult == PSEMResponse.Ok)
                                {
                                    PSEMDataStream = new MemoryStream(byaData);
                                    MeterTables.SavePSEMStream(2412, PSEMDataStream, uiBlockOffset);
                                }
                            }
                        }

                        // Make sure that we save the new data to the old.
                        usValidBlocks = usNewValidBlocks;
                        usNumberIntervals = usNewNumberIntervals;
                    }
                    else // No new interval occurred
                    {
                        bBlocksReRead = false;
                    }
                } while (bBlocksReRead == true);
            }
            else
            {
                throw new Exception("Table 2409 must be read prior to Table 2412.");
            }
            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2413 from the meter.
        /// </summary>
        /// <param name="MeterTables">The table object for the meter.</param>
        /// <returns>The PSEM response code.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/11 RCG 2.53.20 N/A    Created
        //
        protected PSEMResponse ReadTable2413(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] byaData;
            byte DataSetIndex = 1;

            CentronTables TempTables = new CentronTables();
            uint uiMaxOffsetReadBytes;
            uint uiReadMemorySize;
            uint uiNumberOfReads;
            uint uiBytesToRead;
            uint uiCurrentOffset;
            uint uiBlockOffset;
            uint uiBlockLength;
            uint uiMaxBlocksToRead;
            uint uiMaxBytesToRead;
            ushort usValidBlocks;
            ushort usNumberIntervals;

            ushort usNewValidBlocks;
            ushort usNewNumberIntervals;
            ushort usNewLastBlock;
            int iBlockToRead;

            // This must be initialized to false or you will break the retry logic.
            bool bBlocksReRead = false;

            object objData;

            // Since Load Profile can be very large (144k) it may not be able
            // to be read completely when doing a full read so we need to break
            // it up into multiple offset reads. Table 61 must be read prior to this.

            if (MeterTables.IsAllCached(2411) == true)
            {
                uiMaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                if (uiMaxOffsetReadBytes > ushort.MaxValue)
                {
                    uiMaxOffsetReadBytes = ushort.MaxValue;
                }

                MeterTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, DataSetIndex), null, out objData);
                usValidBlocks = (ushort)objData;

                MeterTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_INT, DataSetIndex), null, out objData);
                usNumberIntervals = (ushort)objData;

                // Determine the size of a Load Profile data block
                MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl365_LP_DATA_SETS, new int[] { 0 },
                    out uiBlockOffset, out uiBlockLength);

                // Determine how many blocks can be read in an offset read
                uiMaxBlocksToRead = uiMaxOffsetReadBytes / uiBlockLength;
                uiMaxBytesToRead = uiMaxBlocksToRead * uiBlockLength;

                // Determine total amount to read
                uiReadMemorySize = usValidBlocks * uiBlockLength;

                // Determine how many reads need to be done
                uiNumberOfReads = usValidBlocks / uiMaxBlocksToRead;

                // Add in a read for any remaining data
                if (usValidBlocks % uiMaxBlocksToRead > 0)
                {
                    uiNumberOfReads++;
                }

                uiCurrentOffset = 0;

                for (uint iIndex = 0; iIndex < uiNumberOfReads && PSEMResult == PSEMResponse.Ok; iIndex++)
                {
                    uiBytesToRead = uiReadMemorySize - uiCurrentOffset;

                    if (uiBytesToRead > uiMaxBytesToRead)
                    {
                        uiBytesToRead = uiMaxBytesToRead;
                    }

                    PSEMResult = m_PSEM.OffsetRead(2413, (int)uiCurrentOffset, (ushort)uiBytesToRead, out byaData);

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMDataStream = new MemoryStream(byaData);
                        MeterTables.SavePSEMStream(2413, PSEMDataStream, uiCurrentOffset);
                        uiCurrentOffset += uiBytesToRead;
                    }

                    OnStepProgress(new ProgressEventArgs());
                }

                // Reread table 63 and make sure no new intervals have occurred while reading
                CentronTables.CopyTable(0, MeterTables, TempTables);
                CentronTables.CopyTable(1, MeterTables, TempTables);
                CentronTables.CopyTable(2409, MeterTables, TempTables);
                CentronTables.CopyTable(2410, MeterTables, TempTables);
                CentronTables.CopyTable(2411, MeterTables, TempTables);

                do
                {
                    ReadTable(2411, ref TempTables);

                    TempTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, DataSetIndex), null, out objData);
                    usNewValidBlocks = (ushort)objData;

                    TempTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_INT, DataSetIndex), null, out objData);
                    usNewNumberIntervals = (ushort)objData;

                    if (usNewNumberIntervals != usNumberIntervals || usNewValidBlocks != usValidBlocks)
                    {
                        // This will limit us to only two tries at this. (if it is already true it will be set
                        // to false which means we won't try this again.)
                        bBlocksReRead = !bBlocksReRead;

                        // A new interval has occurred so we need to reread at least one block
                        // We only want to copy the information for this data set in order to avoid corrupting other profile data sets
                        uint StatusOffset = 0;
                        uint StatusLength = 0;
                        TempTables.GetFieldOffset((long)CentronTblEnum.MfgTbl363_LP_STATUS_SET2, null, out StatusOffset, out StatusLength);
                        MemoryStream StatusStream = new MemoryStream(new byte[StatusLength]);
                        TempTables.BuildPSEMStream(2411, StatusStream, StatusOffset, StatusLength);
                        MeterTables.SavePSEMStream(2411, StatusStream, StatusOffset);

                        MeterTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_LAST_BLOCK_ELEMENT, DataSetIndex), null, out objData);
                        usNewLastBlock = (ushort)objData;

                        // Determine the offset of the block
                        iBlockToRead = (int)usNewLastBlock;
                        MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl365_LP_DATA_SETS, new int[] { iBlockToRead },
                            out uiBlockOffset, out uiBlockLength);

                        PSEMResult = m_PSEM.OffsetRead(2413, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(byaData);
                            MeterTables.SavePSEMStream(2413, PSEMDataStream, uiBlockOffset);

                            // Now if there was also a new block we need to reread the previous block as well.
                            if (usNewValidBlocks != usValidBlocks)
                            {
                                if (usNewLastBlock - 1 < 0)
                                {
                                    iBlockToRead = usNewValidBlocks - 1;
                                }
                                else
                                {
                                    iBlockToRead = usNewLastBlock - 1;
                                }

                                // Determine the offset of the block
                                MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl365_LP_DATA_SETS, new int[] { iBlockToRead },
                                    out uiBlockOffset, out uiBlockLength);

                                PSEMResult = m_PSEM.OffsetRead(2413, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                                if (PSEMResult == PSEMResponse.Ok)
                                {
                                    PSEMDataStream = new MemoryStream(byaData);
                                    MeterTables.SavePSEMStream(2413, PSEMDataStream, uiBlockOffset);
                                }
                            }
                        }

                        // Make sure that we save the new data to the old.
                        usValidBlocks = usNewValidBlocks;
                        usNumberIntervals = usNewNumberIntervals;
                    }
                    else // No new interval occurred
                    {
                        bBlocksReRead = false;
                    }
                } while (bBlocksReRead == true);
            }
            else
            {
                throw new Exception("Table 2409 must be read prior to Table 2413.");
            }
            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2152
        /// </summary>
        /// <param name="MeterTables">The tables for the meter.</param>
        /// <returns>The PSEM response for communications.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/08 RCG 1.50.24 N/A    Created
        //
        protected PSEMResponse ReadTable2152(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] byaData;

            CentronTables TempTables = new CentronTables();
            uint uiMaxOffsetReadBytes;
            uint uiReadMemorySize;
            uint uiNumberOfReads;
            uint uiBytesToRead;
            uint uiCurrentOffset;
            uint uiBlockOffset;
            uint uiBlockLength;
            uint uiMaxBlocksToRead;
            uint uiMaxBytesToRead;
            ushort usValidBlocks;
            ushort usNumberIntervals;

            ushort usNewValidBlocks;
            ushort usNewNumberIntervals;
            ushort usNewLastBlock;
            int iBlockToRead;

            // This must be initialized to false or you will break the retry logic.
            bool bBlocksReRead = false;
            object objData;

            // Since Voltage Monitoring Data can be very large (144k) it may not be able
            // to be read completely when doing a full read so we need to break
            // it up into multiple offset reads. Table 2149 must be read prior to this.

            if (MeterTables.IsCached((long)CentronTblEnum.MFGTBL101_MEMORY_LEN, null) == true)
            {
                uiMaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                if (uiMaxOffsetReadBytes > ushort.MaxValue)
                {
                    uiMaxOffsetReadBytes = ushort.MaxValue;
                }

                MeterTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null, out objData);
                usValidBlocks = (ushort)objData;

                MeterTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_INT, null, out objData);
                usNumberIntervals = (ushort)objData;

                // Determine the size of a Voltage monitoring data block
                MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL104_VM_DATA, new int[] { 0 },
                    out uiBlockOffset, out uiBlockLength);

                // Determine how many blocks can be read in an offset read
                uiMaxBlocksToRead = uiMaxOffsetReadBytes / uiBlockLength;
                uiMaxBytesToRead = uiMaxBlocksToRead * uiBlockLength;

                // Determine total amount to read
                uiReadMemorySize = usValidBlocks * uiBlockLength;

                // Determine how many reads need to be done
                uiNumberOfReads = usValidBlocks / uiMaxBlocksToRead;

                // Add in a read for any remaining data
                if (usValidBlocks % uiMaxBlocksToRead > 0)
                {
                    uiNumberOfReads++;
                }

                uiCurrentOffset = 0;

                for (uint iIndex = 0; iIndex < uiNumberOfReads && PSEMResult == PSEMResponse.Ok; iIndex++)
                {
                    uiBytesToRead = uiReadMemorySize - uiCurrentOffset;

                    if (uiBytesToRead > uiMaxBytesToRead)
                    {
                        uiBytesToRead = uiMaxBytesToRead;
                    }

                    PSEMResult = m_PSEM.OffsetRead(2152, (int)uiCurrentOffset, (ushort)uiBytesToRead, out byaData);

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMDataStream = new MemoryStream(byaData);
                        MeterTables.SavePSEMStream(2152, PSEMDataStream, uiCurrentOffset);
                        uiCurrentOffset += uiBytesToRead;
                    }

                    OnStepProgress(new ProgressEventArgs());
                }

                // Reread table 63 and make sure no new intervals have occurred while reading
                CentronTables.CopyTable(0, MeterTables, TempTables);
                CentronTables.CopyTable(1, MeterTables, TempTables);
                CentronTables.CopyTable(2148, MeterTables, TempTables);
                CentronTables.CopyTable(2149, MeterTables, TempTables);
                CentronTables.CopyTable(2150, MeterTables, TempTables);

                do
                {
                    ReadTable(2151, ref TempTables);

                    TempTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null, out objData);
                    usNewValidBlocks = (ushort)objData;

                    TempTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_INT, null, out objData);
                    usNewNumberIntervals = (ushort)objData;

                    if (usNewNumberIntervals != usNumberIntervals || usNewValidBlocks != usValidBlocks)
                    {
                        // This will limit us to only two tries at this. (if it is already true it will be set
                        // to false which means we won't try this again.)
                        bBlocksReRead = !bBlocksReRead;

                        // A new interval has occurred so we need to reread atleast one block
                        CentronTables.CopyTable(2151, TempTables, MeterTables);

                        MeterTables.GetValue(CentronTblEnum.MFGTBL103_LAST_BLOCK_ELEMENT, null, out objData);
                        usNewLastBlock = (ushort)objData;

                        // Determine the offset of the block
                        iBlockToRead = (int)usNewLastBlock;
                        MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL104_VM_DATA, new int[] { iBlockToRead },
                            out uiBlockOffset, out uiBlockLength);

                        PSEMResult = m_PSEM.OffsetRead(2152, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(byaData);
                            MeterTables.SavePSEMStream(2152, PSEMDataStream, uiBlockOffset);

                            // Now if there was also a new block we need to reread the previous block as well.
                            if (usNewValidBlocks != usValidBlocks)
                            {
                                if (usNewLastBlock - 1 < 0)
                                {
                                    iBlockToRead = usNewValidBlocks - 1;
                                }
                                else
                                {
                                    iBlockToRead = usNewLastBlock - 1;
                                }

                                // Determine the offset of the block
                                MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL104_VM_DATA, new int[] { iBlockToRead },
                                    out uiBlockOffset, out uiBlockLength);

                                PSEMResult = m_PSEM.OffsetRead(2152, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                                if (PSEMResult == PSEMResponse.Ok)
                                {
                                    PSEMDataStream = new MemoryStream(byaData);
                                    MeterTables.SavePSEMStream(2152, PSEMDataStream, uiBlockOffset);
                                }
                            }
                        }

                        // Make sure that we save the new data to the old.
                        usValidBlocks = usNewValidBlocks;
                        usNumberIntervals = usNewNumberIntervals;
                    }
                    else // No new interval occurred
                    {
                        bBlocksReRead = false;
                    }
                } while (bBlocksReRead == true);
            }
            else
            {
                throw new Exception("Table 2149 must be read prior to Table 2152.");
            }

            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2157, extended voltage monitoring data table.
        /// </summary>
        /// <param name="MeterTables">The tables for the meter.</param>
        /// <returns>The PSEM response for communications.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/13/11 jrf 2.53.17 TC2907 Created
        //
        protected PSEMResponse ReadTable2157(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] byaData;

            CentronTables TempTables = new CentronTables();
            uint uiMaxOffsetReadBytes;
            uint uiReadMemorySize;
            uint uiNumberOfReads;
            uint uiBytesToRead;
            uint uiCurrentOffset;
            uint uiBlockOffset;
            uint uiBlockLength;
            uint uiMaxBlocksToRead;
            uint uiMaxBytesToRead;
            ushort usValidBlocks;
            ushort usNumberIntervals;

            ushort usNewValidBlocks;
            ushort usNewNumberIntervals;
            ushort usNewLastBlock;
            int iBlockToRead;

            // This must be initialized to false or you will break the retry logic.
            bool bBlocksReRead = false;
            object objData;

            // Since Voltage Monitoring Data can be very large (144k) it may not be able
            // to be read completely when doing a full read so we need to break
            // it up into multiple offset reads. Table 2149 must be read prior to this.

            if (MeterTables.IsCached((long)CentronTblEnum.MFGTBL105_MEMORY_LEN, null) == true)
            {
                uiMaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                if (uiMaxOffsetReadBytes > ushort.MaxValue)
                {
                    uiMaxOffsetReadBytes = ushort.MaxValue;
                }

                MeterTables.GetValue(CentronTblEnum.MFGTBL107_NBR_VALID_BLOCKS, null, out objData);
                usValidBlocks = (ushort)objData;

                MeterTables.GetValue(CentronTblEnum.MFGTBL107_NBR_VALID_INT, null, out objData);
                usNumberIntervals = (ushort)objData;

                // Determine the size of a Voltage monitoring data block
                MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL109_VM_DATA, new int[] { 0 },
                    out uiBlockOffset, out uiBlockLength);

                // Determine how many blocks can be read in an offset read
                uiMaxBlocksToRead = uiMaxOffsetReadBytes / uiBlockLength;
                uiMaxBytesToRead = uiMaxBlocksToRead * uiBlockLength;

                // Determine total amount to read
                uiReadMemorySize = usValidBlocks * uiBlockLength;

                // Determine how many reads need to be done
                uiNumberOfReads = usValidBlocks / uiMaxBlocksToRead;

                // Add in a read for any remaining data
                if (usValidBlocks % uiMaxBlocksToRead > 0)
                {
                    uiNumberOfReads++;
                }

                uiCurrentOffset = 0;

                for (uint iIndex = 0; iIndex < uiNumberOfReads && PSEMResult == PSEMResponse.Ok; iIndex++)
                {
                    uiBytesToRead = uiReadMemorySize - uiCurrentOffset;

                    if (uiBytesToRead > uiMaxBytesToRead)
                    {
                        uiBytesToRead = uiMaxBytesToRead;
                    }

                    PSEMResult = m_PSEM.OffsetRead(2157, (int)uiCurrentOffset, (ushort)uiBytesToRead, out byaData);

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMDataStream = new MemoryStream(byaData);
                        MeterTables.SavePSEMStream(2157, PSEMDataStream, uiCurrentOffset);
                        uiCurrentOffset += uiBytesToRead;
                    }

                    OnStepProgress(new ProgressEventArgs());
                }

                // Reread table 2155 and make sure no new intervals have occurred while reading
                CentronTables.CopyTable(0, MeterTables, TempTables);
                CentronTables.CopyTable(1, MeterTables, TempTables);
                CentronTables.CopyTable(2153, MeterTables, TempTables);
                CentronTables.CopyTable(2154, MeterTables, TempTables);

                do
                {
                    ReadTable(2155, ref TempTables);

                    TempTables.GetValue(CentronTblEnum.MFGTBL107_NBR_VALID_BLOCKS, null, out objData);
                    usNewValidBlocks = (ushort)objData;

                    TempTables.GetValue(CentronTblEnum.MFGTBL107_NBR_VALID_INT, null, out objData);
                    usNewNumberIntervals = (ushort)objData;

                    if (usNewNumberIntervals != usNumberIntervals || usNewValidBlocks != usValidBlocks)
                    {
                        // This will limit us to only two tries at this. (if it is already true it will be set
                        // to false which means we won't try this again.)
                        bBlocksReRead = !bBlocksReRead;

                        // A new interval has occurred so we need to reread atleast one block
                        CentronTables.CopyTable(2155, TempTables, MeterTables);

                        MeterTables.GetValue(CentronTblEnum.MFGTBL107_LAST_BLOCK_ELEMENT, null, out objData);
                        usNewLastBlock = (ushort)objData;

                        // Determine the offset of the block
                        iBlockToRead = (int)usNewLastBlock;
                        MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL109_VM_DATA, new int[] { iBlockToRead },
                            out uiBlockOffset, out uiBlockLength);

                        PSEMResult = m_PSEM.OffsetRead(2157, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(byaData);
                            MeterTables.SavePSEMStream(2157, PSEMDataStream, uiBlockOffset);

                            // Now if there was also a new block we need to reread the previous block as well.
                            if (usNewValidBlocks != usValidBlocks)
                            {
                                if (usNewLastBlock - 1 < 0)
                                {
                                    iBlockToRead = usNewValidBlocks - 1;
                                }
                                else
                                {
                                    iBlockToRead = usNewLastBlock - 1;
                                }

                                // Determine the offset of the block
                                MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL109_VM_DATA, new int[] { iBlockToRead },
                                    out uiBlockOffset, out uiBlockLength);

                                PSEMResult = m_PSEM.OffsetRead(2157, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                                if (PSEMResult == PSEMResponse.Ok)
                                {
                                    PSEMDataStream = new MemoryStream(byaData);
                                    MeterTables.SavePSEMStream(2157, PSEMDataStream, uiBlockOffset);
                                }
                            }
                        }

                        // Make sure that we save the new data to the old.
                        usValidBlocks = usNewValidBlocks;
                        usNumberIntervals = usNewNumberIntervals;
                    }
                    else // No new interval occurred
                    {
                        bBlocksReRead = false;
                    }
                } while (bBlocksReRead == true);
            }
            else
            {
                throw new Exception("Table 2153 must be read prior to Table 2157.");
            }

            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2162
        /// </summary>
        /// <param name="MeterTables">The meter tables object</param>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/13/12 RCG 2.70.36        Created
        //  01/16/15 AF  4.00.92 556830 Corrected the offset read code to make sure it 
        //                              doesn't try to read beyond the end of the table
        //
        protected PSEMResponse ReadTable2162(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult;
            MemoryStream PSEMDataStream;
            byte[] PSEMData;

            uint MaxOffsetReadSize = 0;
            uint TableSize = 0;
            uint CurrentOffset = 0;

            MaxOffsetReadSize = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

            // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
            if (MaxOffsetReadSize > ushort.MaxValue)
            {
                MaxOffsetReadSize = ushort.MaxValue;
            }

            TableSize = MeterTables.GetTableLength(2162);

            if (TableSize < MaxOffsetReadSize)
            {
                // We can read the whole table in the Full Read
                PSEMResult = m_PSEM.FullRead(2162, out PSEMData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(PSEMData);
                    MeterTables.SavePSEMStream(2162, PSEMDataStream);
                }
            }
            else
            {
                uint NumberUnreadEntriesOffset;
                uint NumberUnreadEntriesLength;
                uint HeaderSize;
                uint EntrySize;
                uint EntryOffset;
                uint MaxEntriesToRead;

                // Determine the length of the header portion
                MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL114_NBR_UNREAD_ENTRIES, null, out NumberUnreadEntriesOffset, out NumberUnreadEntriesLength);

                HeaderSize = NumberUnreadEntriesOffset + NumberUnreadEntriesLength;

                // Read the header
                PSEMResult = m_PSEM.OffsetRead(2162, 0, (ushort)HeaderSize, out PSEMData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(PSEMData);
                    MeterTables.SavePSEMStream(2162, PSEMDataStream, 0);

                    CurrentOffset += HeaderSize;

                    MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL114_ENTRIES, new int[] { 0 }, out EntryOffset, out EntrySize);

                    // Determine the maximum number of entries that can be read in a single read
                    MaxEntriesToRead = MaxOffsetReadSize / EntrySize;

                    // Read the entries
                    while (CurrentOffset < TableSize)
                    {
                        ushort BytesToRead = (ushort)(MaxEntriesToRead * EntrySize);

                        // Make sure we don't try to read beyond the end of the table
                        if (BytesToRead > TableSize - CurrentOffset)
                        {
                            BytesToRead = (ushort)(TableSize - CurrentOffset);
                        }

                        PSEMResult = m_PSEM.OffsetRead(2162, (int)CurrentOffset, BytesToRead, out PSEMData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(PSEMData);
                            MeterTables.SavePSEMStream(2162, PSEMDataStream, CurrentOffset);

                            CurrentOffset += BytesToRead;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }

            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2164
        /// </summary>
        /// <param name="MeterTables">The meter tables object</param>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/13/12 RCG 2.70.36        Created
        //  01/16/15 AF  4.00.92 556830 Corrected the offset read code to make sure it 
        //                              doesn't try to read beyond the end of the table
        //
        protected PSEMResponse ReadTable2164(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult;
            MemoryStream PSEMDataStream;
            byte[] PSEMData;

            uint MaxOffsetReadSize = 0;
            uint TableSize = 0;
            uint CurrentOffset = 0;

            MaxOffsetReadSize = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

            // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
            if (MaxOffsetReadSize > ushort.MaxValue)
            {
                MaxOffsetReadSize = ushort.MaxValue;
            }

            TableSize = MeterTables.GetTableLength(2164);

            if (TableSize < MaxOffsetReadSize)
            {
                // We can read the whole table in the Full Read
                PSEMResult = m_PSEM.FullRead(2164, out PSEMData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(PSEMData);
                    MeterTables.SavePSEMStream(2164, PSEMDataStream);
                }
            }
            else
            {
                uint NumberUnreadEntriesOffset;
                uint NumberUnreadEntriesLength;
                uint HeaderSize;
                uint EntrySize;
                uint EntryOffset;
                uint MaxEntriesToRead;

                // Determine the length of the header portion
                MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL116_NBR_UNREAD_ENTRIES, null, out NumberUnreadEntriesOffset, out NumberUnreadEntriesLength);

                HeaderSize = NumberUnreadEntriesOffset + NumberUnreadEntriesLength;

                // Read the header
                PSEMResult = m_PSEM.OffsetRead(2164, 0, (ushort)HeaderSize, out PSEMData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(PSEMData);
                    MeterTables.SavePSEMStream(2164, PSEMDataStream, 0);

                    CurrentOffset += HeaderSize;

                    MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL116_ENTRIES, new int[] { 0 }, out EntryOffset, out EntrySize);

                    // Determine the maximum number of entries that can be read in a single read
                    MaxEntriesToRead = MaxOffsetReadSize / EntrySize;

                    // Read the entries
                    while (CurrentOffset < TableSize)
                    {
                        ushort BytesToRead = (ushort)(MaxEntriesToRead * EntrySize);

                        // Make sure we don't try to read beyond the end of the table
                        if (BytesToRead > TableSize - CurrentOffset)
                        {
                            BytesToRead = (ushort)(TableSize - CurrentOffset);
                        }

                        PSEMResult = m_PSEM.OffsetRead(2164, (int)CurrentOffset, BytesToRead, out PSEMData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(PSEMData);
                            MeterTables.SavePSEMStream(2164, PSEMDataStream, CurrentOffset);

                            CurrentOffset += BytesToRead;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return PSEMResult;
        }

        /// <summary>
        /// This method caches the values returned from the Valdate Base Energies procedure (mfg. proc 108).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/25/15 jrf 4.21.05 607438 Created
        //
        protected void StoreProcedureValidatedBaseEnergies()
        {
            bool EnergyConfigSupported;
            List<BaseEnergies> Energies;

            if (ProcedureResultCodes.COMPLETED == ValidateBaseEnergies(out EnergyConfigSupported, out Energies))
            {
                m_BaseEnergyConfigurationSupported.Value = EnergyConfigSupported;

                if (null != Energies)
                {
                    if (Energies.Count > 0)
                    {
                        m_BaseSuppliedEnergy1.Value = Energies[0];
                    }

                    if (Energies.Count > 1)
                    {
                        m_BaseSuppliedEnergy2.Value = Energies[1];
                    }
                }
            }
        }

        #endregion

        #region Static Protected Methods

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

        /// <summary>
        /// Gets the number of energies that can be configured to the base by this device
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/08/11 RCG 2.53.15        Created

        protected virtual int NumberofBaseConfigurableEnergies
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the number of energies supported by this device
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/11 RCG 2.50.18        Created

        protected virtual int NumberOfSupportedEnergies
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Gets the meter type "CENTRONAMI"
        /// </summary>		
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/25/06 mrj 7.30.00 N/A    Created
        //
        protected override string DefaultMeterType
        {
            get
            {
                return CENTRONAMI;
            }
        }

        #endregion

        #region Private Methods

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
        /// Sets the Description of Display Items that are not set or need to be something 
        /// other than what is returned from the LID.
        /// </summary>
        /// <param name="Item">The item to handle</param>
        /// <returns>True if the item was handled, false otherwise</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 02/26/13 jrf 2.70.72 323232 Created
        //
        internal override bool HandleIrregularDescription(ANSIDisplayItem Item)
        {
            bool bResult = base.HandleIrregularDescription(Item);
            DefinedLIDs LIDDefinitions = new DefinedLIDs();
            LID DemandLID = null;

            //The previous max demand quantity LIDs do not specify which quantity,
            //so we have to tease out which quantity it is to provide a more meaningful description.
            if (LIDDefinitions.PREVIOUS_MAX_DEMAND_QTY_1 == Item.DisplayLID)
            {
                if (null != Table2048.DemandConfig.Demands && 1 <= Table2048.DemandConfig.Demands.Count)
                {
                    DemandLID = CreateLID(Table2048.DemandConfig.Demands[0]);
                }
            }
            else if (LIDDefinitions.PREVIOUS_MAX_DEMAND_QTY_2 == Item.DisplayLID)
            {
                if (null != Table2048.DemandConfig.Demands && 2 <= Table2048.DemandConfig.Demands.Count)
                {
                    DemandLID = CreateLID(Table2048.DemandConfig.Demands[1]);
                }
            }
            else if (LIDDefinitions.PREVIOUS_MAX_DEMAND_QTY_3 == Item.DisplayLID)
            {
                if (null != Table2048.DemandConfig.Demands && 2 <= Table2048.DemandConfig.Demands.Count)
                {
                    DemandLID = CreateLID(Table2048.DemandConfig.Demands[2]);
                }
            }

            if (null != DemandLID)
            {
                Item.Description = "previous " + DemandLID.lidDescription;
                bResult = true;
            }

            return bResult;
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
        /// Takes an encrypted key in the form of a string as input
        /// and applies the DES encryption algorithm to produce an
        /// unencrypted byte array
        /// </summary>
        /// <param name="strEncryptedKey">Encrypted security key</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/16/08 AF  1.50.37        Created
        //
        private static byte[] DecryptHANKey(string strEncryptedKey)
        {
            int Discarded;

            SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);
            DESCryptoServiceProvider Crypto = new DESCryptoServiceProvider();
            Crypto.Key = DataStorage.RetrieveSecureData(SecureDataStorage.ZIGBEE_KEY_ID);
            Crypto.IV = DataStorage.RetrieveSecureData(SecureDataStorage.ZIGBEE_IV_ID);

            byte[] EncryptedKey = HexEncoding.GetBytes(strEncryptedKey, out Discarded);

            //Create a memory stream to the passed buffer.
            MemoryStream EncryptedStream = new MemoryStream(EncryptedKey);
            MemoryStream DecryptedStream = new MemoryStream();

            Encryption.DecryptData(Crypto, EncryptedStream, DecryptedStream);

            //We must rewind the stream
            DecryptedStream.Position = 0;

            // Create a StreamReader for reading the stream.
            StreamReader sr = new StreamReader(DecryptedStream);

            // Read the stream as a string.
            string strDecryptedKey = sr.ReadLine();

            // Close the streams.
            //Closing sr also closes DecryptedStream
            sr.Close();
            EncryptedStream.Close();

            //Transform the string into a byte array
            return HexEncoding.GetBytes(strDecryptedKey, out Discarded);
        }

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
        //  12/02/09 MMD  2.30.21      Created
        //  02/15/10 MMD  2.40.13      Added a requirement to validate DES Keys based on DES Key Length
        //
        private bool RequiresDESKeysValidation(string ProgName, CENTRON_AMI.SecurityKeyID DESKeyID)
        {
            bool bValidate = true;

            try
            {
                // Load the XML file to get the Security Provider and Exception Security model from the Program file
                object objSecurityProviderValue = null;
                object objExceptionModelValue = null;
                object objDESKeyLength = null;
                int iDESLength = 0;
                XmlReader xmlReader;
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.IgnoreWhitespace = true;
                settings.IgnoreComments = true;
                settings.CheckCharacters = false;
                xmlReader = XmlReader.Create(ProgName, settings);
                CentronTables objCenTables = new CentronTables();
                objCenTables.LoadEDLFile(xmlReader);

                int iIndex = Convert.ToInt32(((byte)DESKeyID) - 1);

                // Get the value Security provider from the program
                objCenTables.GetValue(CentronTblEnum.MFGTBL2045_SECURITY_PROVIDER, null, out objSecurityProviderValue);

                //Get the value Exception model form the program
                objCenTables.GetValue(CentronTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL, null, out objExceptionModelValue);

                // Get the Length of DES/3DES keys
                objCenTables.GetValue(StdTableEnum.STDTBL46_KEY_LENGTH, new int[] { iIndex }, out objDESKeyLength);

                if (objDESKeyLength != null)
                {
                    iDESLength = Convert.ToByte(objDESKeyLength, CultureInfo.CurrentCulture);
                }

                OpenWayMFGTable2193.SecurityFormat? exceptionModelFormat = (OpenWayMFGTable2193.SecurityFormat)objExceptionModelValue;

                //1 = EnhancedItronSecurity for security provider
                if (((int)objSecurityProviderValue == 1 && (exceptionModelFormat == OpenWayMFGTable2193.SecurityFormat.EnhancedItronSecurity)) || iDESLength == 255)
                {
                    bValidate = false;
                }
            }
            catch (Exception)
            {
                bValidate = false;
            }
            return bValidate;
        }

        #endregion

        #region Table Properties

        #region STD Decade 0 - General Configuration Tables

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

        #endregion

        #region STD Decade 1 - Data Source Tables

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

        #endregion

        #region STD Decade 2 - Register Tables

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

                if (null == m_Table24 && Table00.IsTableUsed(24))
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
        public virtual StdTable28 Table28
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

        #endregion

        #region STD Decade 6 - Load Profile Tables

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

        public virtual StdTable62 Table62
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

        #endregion

        #region MFG Table 0

        /// <summary>
        /// This property returns the correct version of the 2048 table for the
        /// Centron Mono meter (Creates it if necessary)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/06 mrj 7.30.00 N/A    Created
        // 11/20/06 KRC 6.00.00 N/A    Changed to Property	
        //
        internal override CTable2048 Table2048
        {
            get
            {
                if (null == m_Table2048)
                {
                    m_Table2048 = new CTable2048_OpenWay(m_PSEM);
                }

                return m_Table2048;
            }
        }

        #endregion

        #region MFG Table 5 - Demand Reset

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

        #endregion

        #region MFG Table 13 - Factory Data Table

        /// <summary>
        /// Gets the HAN Security Keys sub table object.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/17/08 RCG 1.50.37			Created

        private MFGTable2061HANSecurityKeys Table2061HANSecurityKeys
        {
            get
            {
                if (null == m_Table2061HANSecurityKeys)
                {
                    m_Table2061HANSecurityKeys = new MFGTable2061HANSecurityKeys(m_PSEM);
                }

                return m_Table2061HANSecurityKeys;
            }
        }

        /// <summary>
        /// Gets the RFLAN Opt Out sub table object.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  01/18/12 MSC 2.53.30            Created
        private Table2061RFLANOptOut Table2061RFLAN
        {
            get
            {
                if (null == m_Table2061RFLAN)
                {
                    m_Table2061RFLAN = new Table2061RFLANOptOut(m_PSEM);
                }

                return m_Table2061RFLAN;
            }
        }

        /// <summary>
        /// Gets the Energy 1 MeterKey sub table object.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Issue# Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 09/01/16 jrf 4.70.16 WI 708332 Created
        private MFGTable2061Energy1MeterKey Table2061Energy1MeterKey
        {
            get
            {
                if (null == m_Table2061Energy1MeterKey)
                {
                    m_Table2061Energy1MeterKey = new MFGTable2061Energy1MeterKey(m_PSEM);
                }

                return m_Table2061Energy1MeterKey;
            }
        }

        #endregion

        #region MFG Table 31 - EPF Flash Data Table

        /// <summary>
        /// Return the Table Object for Table 2079 (Mirror of Comm Module Table 0)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/18/12 jrf 2.70.18 TQ6657 Created
        //
        internal OpenWayMFGTable2079 Table2079
        {
            get
            {
                if (null == m_Table2079 && IsTableUsed(2079))
                {
                    m_Table2079 = new OpenWayMFGTable2079(m_PSEM);
                }

                return m_Table2079;
            }
        }

        #endregion

        #region MFG Decade 5 - HAN Client Tables

        /// <summary>
        /// Gets the Table CHANMfgTable2098 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/11/09 AF  2.20.07        Created
        //
        private CHANMfgTable2098 Table2098
        {
            get
            {
                if (null == m_Table2098)
                {
                    m_Table2098 = new CHANMfgTable2098(m_PSEM);
                }

                return m_Table2098;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2100 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/22/06 AF  8.00.00        Created
        //  12/13/13 DLG 3.50.15        Updated to use HANInformation object to access Table2099.
        //
        private CHANMfgTable2100 Table2100
        {
            get
            {
                if (null == m_Table2100)
                {
                    m_Table2100 = new CHANMfgTable2100(m_PSEM, m_HANInfo.Table2099);
                }

                return m_Table2100;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2101 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        // 12/13/13 DLG 3.50.15         Updated to use HANInformation object to access Table2099.
        //
        private CHANMfgTable2101 Table2101
        {
            get
            {
                if (null == m_Table2101)
                {
                    m_Table2101 = new CHANMfgTable2101(m_PSEM, m_HANInfo.Table2099);
                }

                return m_Table2101;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2105 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 AF  1.50.36        Created
        //  12/13/13 DLG 3.50.15        Updated to use HANInformation object to access Table2099.
        //
        private CHANMfgTable2105 Table2105
        {
            get
            {
                if (null == m_Table2105 && Table00.IsTableUsed(2105))
                {
                    m_Table2105 = new CHANMfgTable2105(m_PSEM, m_HANInfo.Table2099);
                }

                return m_Table2105;
            }
        }

        /// <summary>
        /// Gets the Table OpenWayMfgTable2108 object - MCU Stats (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        // 11/27/06 AF  8.00.00         Changed the class name to match other
        //                              HAN Mfg tables
        // 03/16/10 AF  2.40.25         Removed the firmware build parameter from the constructor
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        // 12/17/13 DLG 3.50.16         Moved from ANSIDevice back to CENTRON_AMI because this table
        //                              is not supported in ICS_Gateway devices.
        //
        protected OpenWayMfgTable2108 Table2108
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

        /// <summary>
        /// Gets Mfg table 2093
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/24/16 PGH 4.70.15 701952 Created
        // 09/06/16 jrf 4.70.16 WI710078 Added is table used check.
        protected CHANMfgTable2093 Table2093
        {
            get
            {
                if (null == m_Table2093 && Table00.IsTableUsed(2093))
                {
                    m_Table2093 = new CHANMfgTable2093(m_PSEM);
                }

                return m_Table2093;
            }
        }

        /// <summary>
        /// Gets Mfg table 2094
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.17 WI 714619 Created
        protected CHANMfgTable2094 Table2094
        {
            get
            {
                if (null == m_Table2094 && Table00.IsTableUsed(2094))
                {
                    m_Table2094 = new CHANMfgTable2094(m_PSEM);
                }

                return m_Table2094;
            }
        }

        /// <summary>
        /// Gets Mfg table 2095
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/24/16 PGH 4.70.15 701952 Created
        // 09/06/16 jrf 4.70.16 WI710078 Added is table used check.
        protected CHANMfgTable2095 Table2095
        {
            get
            {
                if (null == m_Table2095 && Table00.IsTableUsed(2095))
                {
                    m_Table2095 = new CHANMfgTable2095(m_PSEM);
                }

                return m_Table2095;
            }
        }

        /// <summary>
        /// Gets Mfg table 2096
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/24/16 PGH 4.70.15 701952 Created
        // 09/06/16 jrf 4.70.16 WI710078 Added is table used check.
        protected CHANMfgTable2096 Table2096
        {
            get
            {
                if (null == m_Table2096 && Table00.IsTableUsed(2096))
                {
                    m_Table2096 = new CHANMfgTable2096(m_PSEM);
                }

                return m_Table2096;
            }
        }

        /// <summary>
        /// Gets Mfg table 2097
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/24/16 PGH 4.70.15 701952 Created
        // 09/06/16 jrf 4.70.16 WI710080 Added is table used check.
        protected CHANMfgTable2097 Table2097
        {
            get
            {
                if (null == m_Table2097 && Table00.IsTableUsed(2097))
                {
                    m_Table2097 = new CHANMfgTable2097(m_PSEM);
                }

                return m_Table2097;
            }
        }

        #endregion

        #region MFG Decade 6 - Firmware Download, Metrology Stats, RFLAN RSSI Info

        /// <summary>
        /// Gets the Table OpenWayMFGTable2112MetrologyStatistics object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/17/11 jrf 2.50.09			Created
        // 02/12/13 MSC 2.70.68 312352 Table 2112's size for newer Firmware is dependant upon Poly vs Single Phase
        //
        private OpenWayMFGTable2112MetrologyStatistics Table2112
        {
            get
            {
                if (null == m_Table2112 && Table00.IsTableUsed(2112))
                {
                    m_Table2112 = new OpenWayMFGTable2112MetrologyStatistics(m_PSEM, FWRevision,
                                Table00.IsTableUsed(2169)); //firmware team determines Single/Poly using MFG table 121 
                }

                return m_Table2112;
            }
        }

        #endregion

        #region MFG Decade 7 - RFLAN Network Tables

        /// <summary>
        /// Gets the C1222 Debug Info Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/10 jrf 2.40.12			Created
        // 04/06/10 jrf 2.40.32         Table size and structure is FW dependent.
        //
        private C1222DebugInfoTable2114 Table2114C1222DebugInfo
        {
            get
            {
                if (null == m_Table2114C1222DebugInfo)
                {
                    m_Table2114C1222DebugInfo = new C1222DebugInfoTable2114(m_PSEM, FWRevision);
                }

                return m_Table2114C1222DebugInfo;
            }
        }

        /// <summary>
        /// Gets the RFLAN Information II table 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/14/10 RCG 2.40.06			Created

        private RFLANMFGTable2119 Table2119
        {
            get
            {
                if (m_Table2119 == null)
                {
                    m_Table2119 = new RFLANMFGTable2119(m_PSEM);
                }

                return m_Table2119;
            }
        }

        /// <summary>
        /// Gets the RFLAN Information II table 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/04/11 MSC 2.53.03			Created
        private RFLANMFGTable2121 Table2121
        {
            get
            {
                if (m_Table2121 == null && Table00.IsTableUsed(2121) == true)
                {
                    m_Table2121 = new RFLANMFGTable2121(m_PSEM);
                }

                return m_Table2121;
            }
        }

        /// <summary>
        /// Gets the Table 2123 (Scheduled Events) object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        private OpenWayMFGTable2123 Table2123
        {
            get
            {
                if (m_Table2123 == null && Table00.IsTableUsed(2123))
                {
                    m_Table2123 = new OpenWayMFGTable2123(m_PSEM, Table00);
                }

                return m_Table2123;
            }
        }

#if (!WindowsCE)

        /// <summary>
        /// Gets the Enhanced Security Keys sub table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Created

        private MFGTable2127EnhancedSecurityKeys Table2127EnhancedSecurityKeys
        {
            get
            {
                if (m_Table2127EnhancedSecurityKeys == null)
                {
                    m_Table2127EnhancedSecurityKeys = new MFGTable2127EnhancedSecurityKeys(m_PSEM);
                }

                return m_Table2127EnhancedSecurityKeys;
            }
        }
# endif

        #endregion

        #region MFG Decade 9 - Disconnect Switch Tables

        /// <summary>
        /// Gets the Table CHANMfgTable2135 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/21/09 AF  2.20.02        Created
        //  08/05/09 AF  2.20.20        Added code to protect against trying to read
        //                              a non-existant table
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation. Updated to use the
        //                              HANInfomration object to access table 2119.
        //
        private CHANMfgTable2135 Table2135
        {
            get
            {
                if (null == m_Table2135 && Table00.IsTableUsed(2135))
                {
                    m_Table2135 = new CHANMfgTable2135(m_PSEM, m_HANInfo.Table2129);
                }

                return m_Table2135;
            }
        }

        /// <summary>
        /// Gets the object for MFG table 2139
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.22 N/A    Created

        protected OpenWayMFGTable2139 Table2139
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

        //<summary>
        //Gets the object for MFG table 2140
        //</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/16/08 RCG 1.50.22 N/A    Created
        // 10/07/10 MMD  1.0.0  N/A    Added Virtual for Max Image

        internal OpenWayMFGTable2140 Table2140
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

        /// <summary>
        /// Gets the object for MFG table 2139
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.22 N/A    Created

        protected OpenWayMFGTable2141 Table2141
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
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.22 N/A    Created

        protected OpenWayMFGTable2142 Table2142
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

        #endregion

        #region MFG Decade 10 - Voltage Monitoring

        /// <summary>
        /// Gets the Enhanced Voltage Monitoring Actual Dimension Table and creates it if needed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5306 Created
        //
        private OpenWayMFGTable2153 Table2153
        {
            get
            {
                if (null == m_Table2153 && Table00.IsTableUsed(2153))
                {
                    m_Table2153 = new OpenWayMFGTable2153(m_PSEM);
                }

                return m_Table2153;
            }
        }

        /// <summary>
        /// Gets the Enhanced Voltage Monitoring Control Table and creates it if needed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5306 Created
        //
        private OpenWayMFGTable2154 Table2154
        {
            get
            {
                if (null == m_Table2154 && Table00.IsTableUsed(2154))
                {
                    m_Table2154 = new OpenWayMFGTable2154(m_PSEM);
                }

                return m_Table2154;
            }
        }

        /// <summary>
        /// Gets the Enhanced Voltage Monitoring Status Table and creates it if needed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5306 Created
        //
        private OpenWayMFGTable2155 Table2155
        {
            get
            {
                if (null == m_Table2155 && Table00.IsTableUsed(2155))
                {
                    m_Table2155 = new OpenWayMFGTable2155(m_PSEM);
                }

                return m_Table2155;
            }
        }

        /// <summary>
        /// Gets the Enhanced Voltage Monitoring Extended Status Table and creates it if needed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5306 Created
        //
        private OpenWayMFGTable2156 Table2156
        {
            get
            {
                if (null == m_Table2156 && Table00.IsTableUsed(2156))
                {
                    m_Table2156 = new OpenWayMFGTable2156(m_PSEM);
                }

                return m_Table2156;
            }
        }

        /// <summary>
        /// Gets the Enhanced Voltage Monitoring Data Set Table and creates it if needed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5306 Created
        //
        private OpenWayMFGTable2157 Table2157
        {
            get
            {
                if (null == m_Table2157 && Table00.IsTableUsed(2157))
                {
                    m_Table2157 = new OpenWayMFGTable2157(m_PSEM, Table00.TIMESize, Table00.TimeFormat,
                        Table2155.NumberOfBlocks, (byte)Table2156.PhasesMonitored, Table2153.NumberBlockIntervals);
                }

                return m_Table2157;
            }
        }

        #endregion

        #region MFG Decade 12 - Swapout, LED Config, CPC Config, Max Demands

        /// <summary>
        /// Gets the Meter Swap Out table and creates it if needed. If the meter does not support
        /// this table null will be returned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        private OpenWayMFGTable2168 Table2168
        {
            get
            {
                if (null == m_Table2168 && true == Table00.IsTableUsed(2168))
                {
                    m_Table2168 = new OpenWayMFGTable2168(m_PSEM, Table00);
                }

                return m_Table2168;
            }
        }

        /// <summary>
        /// Gets the CPC Config table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/10 RCG 2.40.09 N/A    Created

        private OpenWayMFGTable2170 Table2170
        {
            get
            {
                if (null == m_Table2170 && Table00.IsTableUsed(2170))
                {
                    m_Table2170 = new OpenWayMFGTable2170(m_PSEM, Table00);
                }

                return m_Table2170;
            }
        }

        /// <summary>
        /// Gets the Max Demands table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/07/11 MSC  2.53.13  N/A    Created
        private OpenWayMFGTable2175 Table2175
        {
            get
            {
                if (null == m_Table2175 && Table00.IsTableUsed(2175))
                {
                    m_Table2175 = new OpenWayMFGTable2175(m_PSEM, Table00);
                }

                return m_Table2175;
            }
        }

        #endregion

        #region MFG Decade 13 - Firmware Download

        /// <summary>
        /// Gets the Actual Firmware Download Limiting Table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12 N/A    Created
        private OpenWayMFGTable2179 Table2179
        {
            get
            {
                if (null == m_Table2179)
                {
                    m_Table2179 = new OpenWayMFGTable2179(m_PSEM);
                }

                return m_Table2179;
            }
        }

        /// <summary>
        /// Gets the Firmware Status Table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12 N/A    Created
        //
        private OpenWayMFGTable2180 Table2180
        {
            get
            {
                if (null == m_Table2180)
                {
                    m_Table2180 = new OpenWayMFGTable2180(m_PSEM, Table2179.FormatOfStatus, Table2179.Size);
                }

                return m_Table2180;
            }
        }

        /// <summary>
        /// Gets the Extended Firmware Status Table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/12 JKW 2.70.xx N/A    Created
        //
        private OpenWayMFGTable2182 Table2182
        {
            get
            {
                if (null == m_Table2182)
                {
                    m_Table2182 = new OpenWayMFGTable2182(m_PSEM, Table00);
                }

                return m_Table2182;
            }
        }

        #endregion

        #region MFG Decade 14 - Comm Module Tables

        /// <summary>
        /// Gets the C12.22 configuration table and creates it if needed. If the meter does not support
        /// this table null will be returned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        protected OpenWayMFGTable2191 Table2191
        {
            get
            {
                if (null == m_Table2191 && true == Table00.IsTableUsed(2191))
                {
                    m_Table2191 = new OpenWayMFGTable2191(m_PSEM);
                }

                return m_Table2191;
            }
        }
        
        #endregion

        #region MFG Decade 15 - LCD Tables

        /// <summary>
        /// Gets the Table 2198 (LCD Diagnostics) table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 RCG 2.44.04 N/A    Created

        private OpenWayMFGTable2198 Table2198
        {
            get
            {
                if (null == m_Table2198 && true == Table00.IsTableUsed(2198))
                {
                    m_Table2198 = new OpenWayMFGTable2198(m_PSEM, Table00);
                }

                return m_Table2198;
            }
        }

        #endregion

        #region MFG Decade 17 - Meter Status Tables

        /// <summary>
        /// Gets the Status Information table and create it if needed. If the meter does not support
        /// this table this property will return null
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/13/09 RCG 2.30.09 N/A    Created

        private OpenWayMFGTable2219 Table2219
        {
            get
            {
                if (null == m_Table2219 && true == Table00.IsTableUsed(2219))
                {
                    m_Table2219 = new OpenWayMFGTable2219(m_PSEM);
                }

                return m_Table2219;
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

        protected OpenWayMFGTable2220 Table2220
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
        /// Gets the Security Information Table and creates it if needed. Returns null if this
        /// table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/13 mah 2.85.03 N/A    Created

        protected OpenWayMFGTable2221 Table2221
        {
            get
            {
                if (m_Table2221 == null && Table00.IsTableUsed(2221))
                {
                    m_Table2221 = new OpenWayMFGTable2221(m_PSEM);
                }

                return m_Table2221;
            }
        }

        #endregion

        #region MFG Decade 19 - HAN Event Logs
        #endregion

        #region MFG Decade 21 - SR 3.0 Tables

        /// <summary>
        /// Gets the SR3 HAN Events Config subtable and creates it if needed.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/23/10 jrf 2.50.06        Created
        //  12/12/13 DLG 3.50.15        Moved from ANSIDevice to Centron_AMI.
        //
        private OpenWayMFGTable2260HANEvents Table2260HANEvents
        {
            get
            {
                if (null == m_Table2260HANEvents && Table00.IsTableUsed(2260))
                {
                    m_Table2260HANEvents = new OpenWayMFGTable2260HANEvents(m_PSEM);
                }

                return m_Table2260HANEvents;
            }
        }

        /// <summary>
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
        protected OpenWayMFGTable2260SR30Config Table2260SR30Config
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
        protected OpenWayMFGTable2260ExtendedConfig Table2260ExtendedConfig
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

        protected OpenWayMFGTable2261 Table2261
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
        protected OpenWayMFGTable2262 Table2262
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
        protected OpenWayMFGTable2263 Table2263
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
        /// Gets the Program State table and creates it if needed
        /// This will return null if the table is not supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/11/10 jrf 2.41.01        Created
        //
        protected OpenWayMFGTable2264 Table2264
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
        /// Gets the Mfg table 217 Current Threshold Exceeded Config subtable and creates it if needed.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //
        internal MFGTable2265CTEConfig Table2265CTEConfig
        {
            get
            {
                if (null == m_Table2265CTEConfig && Table00.IsTableUsed(2265))
                {
                    m_Table2265CTEConfig = new MFGTable2265CTEConfig(m_PSEM);
                }

                return m_Table2265CTEConfig;
            }
        }

        #endregion

        #region MFG Decade 22 - HW 3.0 Debug Tables

        /// <summary>
        /// Gets the Task Execution time Table
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/23/11  MMD                   Created

        private OpenWayMFGTable2270 Table2270
        {
            get
            {
                if (m_Table2270 == null && Table00.IsTableUsed(2270))
                {
                    m_Table2270 = new OpenWayMFGTable2270(m_PSEM, FWRevision, FirmwareBuild);
                }

                return m_Table2270;
            }
        }

        #endregion

        #region MFG Decade 24 - HAN Debug Tables

        /// <summary>
        /// Gets the HAN Stats Two table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created

        private OpenWayMFGTable2288 Table2288
        {
            get
            {
                if (m_Table2288 == null && Table00.IsTableUsed(2288))
                {
                    m_Table2288 = new OpenWayMFGTable2288(m_PSEM);
                }

                return m_Table2288;
            }
        }

        /// <summary>
        /// Gets the HAN Reset Error Log table. Returns null if this table is not supported
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/11 RCG 2.50.37        Created
        //  12/12/11 RCG 2.53.20		Fixing table size change issue
        //
        private OpenWayMFGTable2290 Table2290
        {
            get
            {
                if (m_Table2290 == null && Table00.IsTableUsed(2290))
                {
                    m_Table2290 = new OpenWayMFGTable2290(m_PSEM, FWRevision);
                }

                return m_Table2290;
            }
        }

        /// <summary>
        /// Gets the Table 2297 object (Recurring Price Table)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.24        Created
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to CENTRON_AMI. Updated to use the
        //                              HANInformation object to access table 2129.
        //
        protected OpenWayMFGTable2297 Table2297
        {
            get
            {
                if (m_Table2297 == null && Table00.IsTableUsed(2297))
                {
                    m_Table2297 = new OpenWayMFGTable2297(m_PSEM, m_HANInfo.Table2129);
                }

                return m_Table2297;
            }
        }

        #endregion

        #region MFG Decade 31 - CPP

        /// <summary>
        /// Gets the CPP table and creates it if needed
        /// This will return null if the table is not supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  13/13/10 JB                 Created
        //
        protected OpenWayMFGTable2360 Table2360
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
        /// Gets the Meter Time Table. This table can be ready without security.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        private OpenWayMFGTable2361 Table2361
        {
            get
            {
                if (null == m_Table2361 && IsTableUsed(2361))
                {
                    m_Table2361 = new OpenWayMFGTable2361(m_PSEM);
                }

                return m_Table2361;
            }
        }

        #endregion

        #region MFG Decade 32 - Power Monitoring

        /// <summary>
        /// Gets the Power Monitoring Dimension Table.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        private OpenWayMFGTable2368 Table2368
        {
            get
            {
                if (m_Table2368 == null && Table00.IsTableUsed(2368))
                {
                    m_Table2368 = new OpenWayMFGTable2368(m_PSEM);
                }

                return m_Table2368;
            }
        }

        /// <summary>
        /// Gets the Power Monitoring Control Table.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        private OpenWayMFGTable2369 Table2369
        {
            get
            {
                if (m_Table2369 == null && Table00.IsTableUsed(2369))
                {
                    m_Table2369 = new OpenWayMFGTable2369(m_PSEM);
                }

                return m_Table2369;
            }
        }

        /// <summary>
        /// Gets the Power Monitoring Data Table.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        private OpenWayMFGTable2370 Table2370
        {
            get
            {
                if (m_Table2370 == null && Table00.IsTableUsed(2370))
                {
                    m_Table2370 = new OpenWayMFGTable2370(m_PSEM, Table00, Table2368);
                }

                return m_Table2370;
            }
        }

        #endregion

        #region MFG Decade 33 - Firmware Download Log

        /// <summary>
        /// Gets the Actual Firmware Download Event Log table and creates it if needed.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/15/11 jrf 2.52.03 TC4241 Created.
        //
        private OpenWayMFGTable2379 Table2379
        {
            get
            {
                if (null == m_Table2379 && Table00.IsTableUsed(2379))
                {
                    m_Table2379 = new OpenWayMFGTable2379(m_PSEM);
                }

                return m_Table2379;
            }
        }

        /// <summary>
        /// Gets the Firmware Download Event Log Data table and creates it if needed.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/15/11 jrf 2.52.03 TC4241 Created.
        //
        private OpenWayMFGTable2382 Table2382
        {
            get
            {
                if (null == m_Table2382 && Table00.IsTableUsed(2382))
                {
                    m_Table2382 = new OpenWayMFGTable2382(m_PSEM, Table2379, Table00);
                }

                return m_Table2382;
            }
        }

        /// <summary>
        /// Gets the Firmware Download CRC table and creates it if needed.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/18/11 jrf 2.52.04 TC4241 Created.
        //
        private OpenWayMFGTable2383 Table2383
        {
            get
            {
                if (null == m_Table2383 && Table00.IsTableUsed(2383))
                {
                    m_Table2383 = new OpenWayMFGTable2383(m_PSEM);
                }

                return m_Table2383;
            }
        }

        #endregion

        #region MFG Decade 34 - MFG Register Sources

        /// <summary>
        /// Gets the Table 2389 object - MFG Data Sources Actual Limiting Table
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2389 Table2389
        {
            get
            {
                if (m_Table2389 == null && Table00.IsTableUsed(2389))
                {
                    m_Table2389 = new OpenWayMFGTable2389(m_PSEM);
                }

                return m_Table2389;
            }
        }

        /// <summary>
        /// Gets the Table 2392 object - MFG Data Control Table
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2392 Table2392
        {
            get
            {
                if (m_Table2392 == null && Table2389 != null && Table00.IsTableUsed(2392))
                {
                    m_Table2392 = new OpenWayMFGTable2392(m_PSEM, Table2389);
                }

                return m_Table2392;
            }
        }

        #endregion

        #region MFG Decade 36 - Extended Load Profile and Instrumentation Profile Tables

        /// <summary>
        /// Gets the Table 2409 object - MFG Load Profile Actual Limiting Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2409 Table2409
        {
            get
            {
                if (m_Table2409 == null && Table00.IsTableUsed(2409))
                {
                    m_Table2409 = new OpenWayMFGTable2409(m_PSEM, Table00);
                }

                return m_Table2409;
            }
        }

        /// <summary>
        /// Gets the Table 2410 object - MFG Load Profile Control Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2410 Table2410
        {
            get
            {
                if (m_Table2410 == null && Table00.IsTableUsed(2410) && Table2409 != null)
                {
                    m_Table2410 = new OpenWayMFGTable2410(m_PSEM, Table00, Table2409, Table2392);
                }

                return m_Table2410;
            }
        }

        /// <summary>
        /// Gets the Table 2411 object - MFG Load Profile Status Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2411 Table2411
        {
            get
            {
                if (m_Table2411 == null && Table00.IsTableUsed(2411))
                {
                    m_Table2411 = new OpenWayMFGTable2411(m_PSEM, Table00);
                }

                return m_Table2411;
            }
        }

        /// <summary>
        /// Gets the Table 2412 object - MFG Load Profile Data Set 1
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2412 Table2412
        {
            get
            {
                if (m_Table2412 == null && Table00.IsTableUsed(2412) && Table2409 != null && Table2410 != null)
                {
                    m_Table2412 = new OpenWayMFGTable2412(m_PSEM, Table00, Table2409, Table2410);
                }

                return m_Table2412;
            }
        }

        /// <summary>
        /// Gets the Table 2413 object - MFG Load Profile Data Set 2
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2413 Table2413
        {
            get
            {
                if (m_Table2413 == null && Table00.IsTableUsed(2413) && Table2409 != null && Table2410 != null)
                {
                    m_Table2413 = new OpenWayMFGTable2413(m_PSEM, Table00, Table2409, Table2410);
                }

                return m_Table2413;
            }
        }

        /// <summary>
        /// Gets the Table 2414 object - MFG Load Profile Data Set 3
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2414 Table2414
        {
            get
            {
                if (m_Table2414 == null && Table00.IsTableUsed(2414) && Table2409 != null && Table2410 != null)
                {
                    m_Table2414 = new OpenWayMFGTable2414(m_PSEM, Table00, Table2409, Table2410);
                }

                return m_Table2414;
            }
        }

        /// <summary>
        /// Gets the Table 2415 object - MFG Load Profile Data Set 4
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        protected OpenWayMFGTable2415 Table2415
        {
            get
            {
                if (m_Table2415 == null && Table00.IsTableUsed(2415) && Table2409 != null && Table2410 != null)
                {
                    m_Table2415 = new OpenWayMFGTable2415(m_PSEM, Table00, Table2409, Table2410);
                }

                return m_Table2415;
            }
        }

        /// <summary>
        /// Gets the Table 2417 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/12 RCG 2.53.36 N/A    Created
        protected OpenWayMFGTable2417 Table2417
        {
            get
            {
                if (m_Table2417 == null && Table00.IsTableUsed(2417))
                {
                    m_Table2417 = new OpenWayMFGTable2417(m_PSEM, Table00);
                }

                return m_Table2417;
            }
        }

        #endregion

        #region MFG Decade 37 - Extended Self Read

        /// <summary>
        /// Gets the Table 2419 object - Actual Extended Self Read Limiting Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/18/12 RCG 2.53.31 TRQ3438 Created 

        protected OpenWayMFGTable2419 Table2419
        {
            get
            {
                if (m_Table2419 == null && Table00.IsTableUsed(2419))
                {
                    m_Table2419 = new OpenWayMFGTable2419(m_PSEM);
                }

                return m_Table2419;
            }
        }

        /// <summary>
        /// Gets the Table 2421 object - MFG Extended Self Read Status Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/09/12 jrf 2.53.28 TREQ2904 Created
        //
        protected OpenWayMFGTable2421 Table2421
        {
            get
            {
                if (m_Table2421 == null && Table00.IsTableUsed(2421))
                {
                    m_Table2421 = new OpenWayMFGTable2421(m_PSEM);
                }

                return m_Table2421;
            }
        }

        /// <summary>
        /// Gets the Table 2422 object - Extended Energy and Instantaneous Self Read Data Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/18/12 RCG 2.53.31 TRQ3438 Created 

        protected OpenWayMFGTable2422 Table2422
        {
            get
            {
                if (m_Table2422 == null && Table00.IsTableUsed(2422) && Table2419 != null)
                {
                    m_Table2422 = new OpenWayMFGTable2422(m_PSEM, Table2419);
                }

                return m_Table2422;
            }
        }

        /// <summary>
        /// Gets the Table 2423 object - MFG Extended Self Read Data Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/09/12 jrf 2.53.28 TREQ2904 Created
        //
        protected OpenWayMFGTable2423 Table2423
        {
            get
            {
                if (m_Table2423 == null && Table00.IsTableUsed(2419) && Table00.IsTableUsed(2423))
                {
                    m_Table2423 = new OpenWayMFGTable2423(m_PSEM, Table2419);
                }

                return m_Table2423;
            }
        }

        #endregion

        #region MFG Decade 39 - RIB

        /// <summary>
        /// Gets the Actual HAN RIB Limiting Table.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created

        private CHANMfgTable2439 Table2439
        {
            get
            {
                if (m_table2439 == null && Table00.IsTableUsed(2439))
                {
                    m_table2439 = new CHANMfgTable2439(m_PSEM);
                }

                return m_table2439;
            }
        }

        /// <summary>
        /// Gets the Active Block Price Schedule Table.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created

        private CHANMfgTable2440 Table2440
        {
            get
            {
                if (m_table2440 == null && Table00.IsTableUsed(2440) && null != Table2439)
                {
                    m_table2440 = new CHANMfgTable2440(m_PSEM, Table2439);
                }

                return m_table2440;
            }
        }

        /// <summary>
        /// Gets the Next Block Price Schedule Table.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/29/12 RCG 2.60.28        Created

        private CHANMfgTable2441 Table2441
        {
            get
            {
                if (m_Table2441 == null && Table00.IsTableUsed(2441) && Table2439 != null)
                {
                    m_Table2441 = new CHANMfgTable2441(m_PSEM, Table2439);
                }

                return m_Table2441;
            }
        }

        #endregion

        #region MFG Decade 54 - TLV Tables (Boron)

        /// <summary>
        /// Gets the table 2612 object - Boron TLV Last Read Table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/11 AF  2.53.21 184509 Created
        //  02/03/12 AF  2.53.38 193194 Removed the check on presence in table 0.
        //
        private OpenWayMFGTable2612 Table2612
        {
            get
            {
                if (null == m_Table2612)
                {
                    m_Table2612 = new OpenWayMFGTable2612(m_PSEM);
                }

                return m_Table2612;
            }
        }

        #endregion

        #region MFG Decade 56 - UIP Tables (Boron)

        /// <summary>
        /// Gets the UIP Stack Stats table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/12 AF  2.70.08 TC9229 Created for Boron f/w tests
        //
        private IPMfgTable2611 Table2611
        {
            get
            {
                if (m_Table2611 == null && Table00.IsTableUsed(2611))
                {
                    m_Table2611 = new IPMfgTable2611(m_PSEM, FWRevision);
                }

                return m_Table2611;
            }
        }

        #endregion

        #region MFG Decade 61 - IP Tables (Carbon)

        /// <summary>
        /// Gets the table 2640 object - Dual Stack Switch Status table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/12 AF  2.70.20 TR6840 Created
        //  02/04/13 jkw 2.70.63 312343 Mfg table 612 (dual stack switch state table) was renumbered to be 592
        //
        private OpenWayMFGTable2640 Table2640
        {
            get
            {
                if (null == m_Table2640 && IsTableUsed(2640))
                {
                    m_Table2640 = new OpenWayMFGTable2640(m_PSEM);
                }

                return m_Table2640;
            }
        }

        #endregion

        #region MFG Table 995 - Core Dump

        /// <summary>
        /// Gets the sub table for the Core Dump Header and creates it if needed.
        /// This property returns null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/04/10 RCG 2.40.11 N/A    Created

        protected OpenWayMFGTable3043Header Table3043Header
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

        protected OpenWayMFGTable3043Map Table3043Map
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

        protected OpenWayMFGTable3043Info Table3043Info
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

        #endregion

        #region Bell Weather Meter Tables

        /// <summary>
        /// Gets the DataSet Configuration from Mfg table 2265.
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/02/15 PGH 4.50.212 577471 Created
        //
        protected MFGTable2265DataSetConfiguration Table2265DataSetConfig
        {
            get
            {
                if (null == m_Table2265DataSetConfig && Table00.IsTableUsed(2265))
                {
                    m_Table2265DataSetConfig = new MFGTable2265DataSetConfiguration(m_PSEM);
                }

                return m_Table2265DataSetConfig;
            }
        }

        /// <summary>
        /// Gets Mfg table 2185 if it exists
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/02/15 PGH 4.50.212 577471 Created
        //
        protected OpenWayMFGTable2185 Table2185
        {
            get
            {
                if (null == m_Table2185 && Table00.IsTableUsed(2185))
                {
                    m_Table2185 = new OpenWayMFGTable2185(m_PSEM);
                }

                return m_Table2185;
            }
        }

        /// <summary>
        /// Gets Mfg table 2186 if it exists
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/02/15 PGH 4.50.212 577471 Created
        //
        protected OpenWayMFGTable2186 Table2186
        {
            get
            {
                if (null == m_Table2186 && Table00.IsTableUsed(2186))
                {
                    m_Table2186 = new OpenWayMFGTable2186(m_PSEM);
                }

                return m_Table2186;
            }
        }

        /// <summary>
        /// Gets Mfg table 2187 if it exists
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/02/15 PGH 4.50.212 577471 Created
        //
        protected OpenWayMFGTable2187 Table2187
        {
            get
            {
                if (null == m_Table2187 && Table00.IsTableUsed(2187))
                {
                    m_Table2187 = new OpenWayMFGTable2187(m_PSEM);
                }

                return m_Table2187;
            }
        }

        #endregion

        #region Temperature Data MFG Tables

        /// <summary>
        /// Gets Mfg table 2425 if it exists
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        protected OpenWayMFGTable2425 Table2425
        {
            get
            {
                if (null == m_Table2425 && Table00.IsTableUsed(2425))
                {
                    m_Table2425 = new OpenWayMFGTable2425(m_PSEM);
                }

                return m_Table2425;
            }
        }

        /// <summary>
        /// Gets Mfg table 2426 if it exists
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        protected OpenWayMFGTable2426 Table2426
        {
            get
            {
                if (null == m_Table2426 && Table00.IsTableUsed(2426))
                {
                    m_Table2426 = new OpenWayMFGTable2426(m_PSEM);
                }

                return m_Table2426;
            }
        }

        /// <summary>
        /// Gets Mfg table 2427 if it exists
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        protected OpenWayMFGTable2427 Table2427
        {
            get
            {
                if (null == m_Table2427 && Table00.IsTableUsed(2427))
                {
                    m_Table2427 = new OpenWayMFGTable2427(m_PSEM);
                }

                return m_Table2427;
            }
        }

        #endregion

        #region MFG Table 329 - Inst Phase Current

        /// <summary>
        /// Gets Mfg table 2377 if it exists
        /// This will return null if the table is not supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //
        protected OpenWayMFGTable2377 Table2377
        {
            get
            {
                if (null == m_Table2377 && Table00.IsTableUsed(2377))
                {
                    m_Table2377 = new OpenWayMFGTable2377(m_PSEM);
                }

                return m_Table2377;
            }
        }


        #endregion

        #region MFG Table 712 - Tamper Summary Table

        /// <summary>
        /// Gets Mfg table 2760 if it exists
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        protected OpenWayMFGTable2760 Table2760
        {
            get
            {
                if (null == m_Table2760 && Table00.IsTableUsed(2760))
                {
                    m_Table2760 = new OpenWayMFGTable2760(m_PSEM);
                }

                return m_Table2760;
            }
        }

        #endregion

        #endregion

        #region Members

        private SecurityLevel? m_CurrentSecurityLevel = null;
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
        /// <summary>Is DST Configured</summary>
        protected CachedBool m_DSTConfigured;

        /// <summary>
        /// The HANInformation object. Used to access common HAN information tables, properties, etc
        /// </summary>
        protected HANInformation m_HANInfo = null;

        /// <summary>
        /// Base Energy Configuration Supported
        /// </summary>
        protected CachedBool m_BaseEnergyConfigurationSupported;

        /// <summary>
        /// Supplied Base Energy 1
        /// </summary>
        protected CachedValue<BaseEnergies> m_BaseSuppliedEnergy1;

        /// <summary>
        /// Supplied Base Energy 2
        /// </summary>
        protected CachedValue<BaseEnergies> m_BaseSuppliedEnergy2;



        #region Tables

        #region STD Decade 0 - General Configuration Tables

        private CTable04 m_Table04 = null;

        #endregion

        #region STD Decade 1 - Data Source Tables

        private StdTable11 m_Table11 = null;
        private StdTable14 m_Table14 = null;

        #endregion

        #region STD Decade 2 - Register Tables

        private StdTable21 m_Table21 = null;
        private StdTable22 m_Table22 = null;
        private StdTable23 m_Table23 = null;
        private StdTable24 m_Table24 = null;
        private StdTable25 m_Table25 = null;
        private StdTable26 m_Table26 = null;
        private StdTable27 m_Table27 = null;
        private StdTable28 m_Table28 = null;

        #endregion

        #region STD Decade 6 - Load Profile Tables

        private StdTable61 m_Table61 = null;
        private StdTable62 m_Table62 = null;
        private StdTable63 m_Table63 = null;
        private StdTable64 m_Table64 = null;

        #endregion

        #region MFG Table 5 - Demand Reset

        private MFGTable2053 m_Table2053 = null;

        #endregion

        #region MFG Table 13 - Factory Data Table

        private Table2061RFLANOptOut m_Table2061RFLAN = null;
        private MFGTable2061HANSecurityKeys m_Table2061HANSecurityKeys = null;
        private MFGTable2061Energy1MeterKey m_Table2061Energy1MeterKey = null;

        #endregion

        #region MFG Table 31 - EPF Flash Data Table

        private OpenWayMFGTable2079 m_Table2079 = null;

        #endregion

        #region MFG Decade 5 - HAN Client Tables

        private CHANMfgTable2098 m_Table2098 = null;
        private CHANMfgTable2100 m_Table2100 = null;
        private CHANMfgTable2101 m_Table2101 = null;
        private CHANMfgTable2105 m_Table2105 = null;
        /// <summary>
        /// The table 2108 object. The MCU stats table (read only).
        /// </summary>
        private OpenWayMfgTable2108 m_Table2108 = null;

        private CHANMfgTable2093 m_Table2093 = null;
        private CHANMfgTable2094 m_Table2094 = null;
        private CHANMfgTable2095 m_Table2095 = null;
        private CHANMfgTable2096 m_Table2096 = null;
        private CHANMfgTable2097 m_Table2097 = null;

        #endregion

        #region MFG Decade 6 - Firmware Download, Metrology Stats, RFLAN RSSI Info

        private OpenWayMFGTable2112MetrologyStatistics m_Table2112 = null;
        private C1222DebugInfoTable2114 m_Table2114C1222DebugInfo = null;

        #endregion

        #region MFG Decade 7 - RFLAN Network Tables

        private RFLANMFGTable2119 m_Table2119 = null;
        private RFLANMFGTable2121 m_Table2121 = null;
        private OpenWayMFGTable2123 m_Table2123 = null;
        /// <summary>
        /// The table 2135 object. The HAN Firmward Download Client table (read only)
        /// </summary>
        private CHANMfgTable2135 m_Table2135 = null;

#if (!WindowsCE)
        private MFGTable2127EnhancedSecurityKeys m_Table2127EnhancedSecurityKeys = null;
#endif

        #endregion

        #region MFG Decade 9 - Disconnect Switch Tables

        private OpenWayMFGTable2139 m_Table2139 = null;
        internal OpenWayMFGTable2140 m_Table2140 = null;
        private OpenWayMFGTable2141 m_Table2141 = null;
        private OpenWayMFGTable2142 m_Table2142 = null;

        #endregion

        #region MFG Decade 10 - Voltage Monitoring

        private OpenWayMFGTable2153 m_Table2153 = null;
        private OpenWayMFGTable2154 m_Table2154 = null;
        private OpenWayMFGTable2155 m_Table2155 = null;
        private OpenWayMFGTable2156 m_Table2156 = null;
        private OpenWayMFGTable2157 m_Table2157 = null;

        #endregion

        #region MFG Decade 12 - Swapout, LED Config, CPC Config, Max Demands

        private OpenWayMFGTable2168 m_Table2168 = null;
        private OpenWayMFGTable2170 m_Table2170 = null;
        private OpenWayMFGTable2175 m_Table2175 = null;

        #endregion

        #region MFG Decade 13 - Firmware Download

        private OpenWayMFGTable2179 m_Table2179 = null;
        private OpenWayMFGTable2180 m_Table2180 = null;
        private OpenWayMFGTable2182 m_Table2182 = null;

        #endregion

        #region MFG Decade 14 - Comm Module Tables

        private OpenWayMFGTable2191 m_Table2191 = null;

        #endregion

        #region MFG Decade 15 - LCD Tables

        private OpenWayMFGTable2198 m_Table2198 = null;

        #endregion

        #region MFG Decade 17 - Meter Status Tables

        private OpenWayMFGTable2219 m_Table2219 = null;
        private OpenWayMFGTable2220 m_Table2220 = null;
        private OpenWayMFGTable2221 m_Table2221 = null;

        #endregion

        #region MFG Decade 19 - HAN Event Logs
        #endregion

        #region MFG Decade 21 - SR 3.0 Tables

        /// <summary>
        /// The table 2260 object. The SR3 Config table (writable).
        /// </summary>
        private OpenWayMFGTable2260HANEvents m_Table2260HANEvents = null;

        private OpenWayMFGTable2260DSTCalendar m_Table2260DSTConfig = null;
        private OpenWayMFGTable2260SR30Config m_Table2260SR30Config = null;
        private OpenWayMFGTable2260ExtendedConfig m_Table2260ExtendedConfig = null;
        private OpenWayMFGTable2261 m_Table2261 = null;
        private OpenWayMFGTable2262 m_Table2262 = null;
        private OpenWayMFGTable2263 m_Table2263 = null;
        private OpenWayMFGTable2264 m_Table2264 = null;
        private MFGTable2265CTEConfig m_Table2265CTEConfig = null;

        /// <summary>
        /// The table 2297 object. The AMI HAN Recurring Price table (writable).
        /// </summary>
        private OpenWayMFGTable2297 m_Table2297 = null;

        #endregion

        #region MFG Decade 22 - HW 3.0 Debug Tables

        private OpenWayMFGTable2270 m_Table2270 = null;

        #endregion

        #region MFG Decade 24 - HAN Debug Tables

        private OpenWayMFGTable2288 m_Table2288 = null;
        private OpenWayMFGTable2290 m_Table2290 = null;

        #endregion

        #region MFG Decade 31 - CPP

        private OpenWayMFGTable2360 m_Table2360 = null;
        private OpenWayMFGTable2361 m_Table2361 = null;

        #endregion

        #region MFG Decade 32 - Power Monitoring

        private OpenWayMFGTable2368 m_Table2368 = null;
        private OpenWayMFGTable2369 m_Table2369 = null;
        private OpenWayMFGTable2370 m_Table2370 = null;

        #endregion

        #region MFG Decade 33 - Firmware Download Log

        private OpenWayMFGTable2379 m_Table2379 = null;
        private OpenWayMFGTable2382 m_Table2382 = null;
        private OpenWayMFGTable2383 m_Table2383 = null;

        #endregion

        #region MFG Decade 34 - MFG Register Sources

        private OpenWayMFGTable2389 m_Table2389 = null;
        private OpenWayMFGTable2392 m_Table2392 = null;

        #endregion

        #region MFG Decade 36 - Extended Load Profile and Instrumentation Profile Tables

        private OpenWayMFGTable2409 m_Table2409 = null;
        private OpenWayMFGTable2410 m_Table2410 = null;
        private OpenWayMFGTable2411 m_Table2411 = null;
        private OpenWayMFGTable2412 m_Table2412 = null;
        private OpenWayMFGTable2413 m_Table2413 = null;
        private OpenWayMFGTable2414 m_Table2414 = null;
        private OpenWayMFGTable2415 m_Table2415 = null;
        private OpenWayMFGTable2417 m_Table2417 = null;

        #endregion

        #region MFG Decade 37 - Extended Self Read

        private OpenWayMFGTable2419 m_Table2419 = null;
        private OpenWayMFGTable2421 m_Table2421 = null;
        private OpenWayMFGTable2422 m_Table2422 = null;
        private OpenWayMFGTable2423 m_Table2423 = null;

        #endregion

        #region MFG Decade 39 - RIB

        private CHANMfgTable2439 m_table2439 = null;
        private CHANMfgTable2440 m_table2440 = null;
        private CHANMfgTable2441 m_Table2441 = null;

        #endregion

        #region MFG Decade 54 - TLV Tables (Boron)

        #endregion

        #region MFG Decade 56 - IP Tables (Boron)

        private IPMfgTable2611 m_Table2611 = null;
        private OpenWayMFGTable2612 m_Table2612 = null;

        #endregion

        #region MFG Decade 61 - IP Tables (Carbon)

        private OpenWayMFGTable2640 m_Table2640 = null;

        #endregion

        #region MFG Table 995 - Core Dump

        private OpenWayMFGTable3043Header m_Table3043Header = null;
        private OpenWayMFGTable3043Info m_Table3043Info = null;
        private OpenWayMFGTable3043Map m_Table3043Map = null;

        #endregion

        #region Bell Weather Meter Tables

        private MFGTable2265DataSetConfiguration m_Table2265DataSetConfig = null;
        private OpenWayMFGTable2185 m_Table2185 = null;
        private OpenWayMFGTable2186 m_Table2186 = null;
        private OpenWayMFGTable2187 m_Table2187 = null;

        #endregion

        #region Temperature Data Meter Tables

        private OpenWayMFGTable2425 m_Table2425 = null;
        private OpenWayMFGTable2426 m_Table2426 = null;
        private OpenWayMFGTable2427 m_Table2427 = null;

        #endregion

        #region MFG Table 329 - Inst Phase Current

        private OpenWayMFGTable2377 m_Table2377 = null;

        #endregion

        #region MFG Table 712 - Tamper Summary Table

        private OpenWayMFGTable2760 m_Table2760 = null;

        #endregion

        #endregion

        #endregion
    }
}
