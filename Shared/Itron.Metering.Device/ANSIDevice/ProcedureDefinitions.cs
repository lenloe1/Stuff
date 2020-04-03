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
//                              Copyright © 2006 - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{    
    /// <summary>
    /// Supported C12.19 standard and MFG procedures
    /// </summary>
    public enum Procedures
    {
        /// <summary>
        /// A value for an unknown procedure.
        /// </summary>
        UNKNOWN = -1,
        /// <summary>
        /// Std Procedure 0, performs a Cold Start that resets the meter.
        /// </summary>
        COLD_START = 0,
        /// <summary>
        /// Std Procedure 2, saves the active configuration, no parameters
        /// </summary>
        SAVE_CONFIGURATION = 2,
        /// <summary>
        /// Std procedure 3, clears billing data, no parameters
        /// </summary>
        CLEAR_DATA = 3,
        /// <summary>
        /// Std procedure 4, parameter is one byte specifying
        /// 1 => clear event log
        /// 2 => clear self read data table
        /// 8 => clear history log
        /// 255 => clear self read, LP, and history log
        /// </summary>
        RESET_LIST_PTRS = 4,
        /// <summary>
        /// Std procedure 5
        /// </summary>
        UPDATE_LAST_READ_ENTRY = 5,
        /// <summary>
        /// Std procedure 7, clears table 3 status bits
        /// </summary>
        CLEAR_STD_STATUS = 7,
        /// <summary>
        /// Std procedure 8, clears table 3 status bits
        /// </summary>
        CLEAR_MFG_STATUS = 8,
        /// <summary>
        /// Std procedure 9, demand reset
        /// </summary>
        REMOTE_RESET = 9,
        /// <summary>
        /// Std procedure 10, set date/time
        /// </summary>
        SET_DATE_TIME = 10,
        /// <summary>
        /// Std procedure 12, activate all pending tables
        /// </summary>
        ACTIVATE_ALL_PENDING_TABLES = 12,
        /// <summary>
        /// Std procedure 13, activate specific pending table
        /// </summary>
        ACTIVATE_PENDING_TABLE = 13,
        /// <summary>
        /// Std procedure 14, clear all pending tables
        /// </summary>
        CLEAR_ALL_PENDING_TABLES = 14,
        /// <summary>
        /// Std procedure 15, clear specific pending table
        /// </summary>
        CLEAR_SPECIFIC_PENDING_TABLE = 15,
        /// <summary>
        /// Std procedure 23, typically used on the local port, is used to 
        /// initiate the registration service.  The Registration service is used
        /// to add and maintain (“keep-alive”) routing information of C12.22 Relays. 
        /// To be part of C12.22 Network, a node shall send a registration service
        /// to one of the C12.22 Relays. 
        /// </summary>
        REGISTER = 23,
        /// <summary>
        /// Std procedure 24, deregister the node on the network.
        /// The deregistration service is used to remove routing information in
        /// C12.22 Relays. This procedure, typically used on the local port 
        /// (ANSI C12.18), is used to initiate this process
        /// </summary>
        DEREGISTER = 24,
        /// <summary>
        /// MFG01 OPEN_CONFIG_FILE	= 2049 - Opens 2048 for writing
        /// </summary>
        OPEN_CONFIG_FILE = 2049,
        /// <summary>
        /// MFG02 CLOSE_CONFIG_FILE	= 2050 - Closes 2048 and reconfigures
        /// the meter
        /// </summary>
        CLOSE_CONFIG_FILE = 2050,
        /// <summary>
        /// MFG03 - Change the display to Normal or test mode.
        /// </summary>
        CHANGE_DISPLAY_MODE = 2051,
        /// <summary>
        /// MFG05 RESET_PASSWORDS = 2053 - Resets all passwords to null 
        /// preceding a password reconfigure
        /// </summary>
        RESET_PASSWORDS = 2053,
        /// <summary>
        /// MFG06 RESET_COUNTERS = 2054 - Resets various MFG device counters
        /// </summary>
        RESET_COUNTERS = 2054,
        /// <summary>
        /// MFG13 BORN_AGAIN = 2061 - Born Again
        /// </summary>
        BORN_AGAIN = 2061,
        /// <summary>
        /// MFG14 RESET_COUNTERS = 2062 - Seal Canadian
        /// </summary>
        SEAL_CANADIAN = 2062,
        /// <summary>
        /// MFG17 GATEWAY_PROC_CALL = 2065 - Procedure for any Gateway specific procedures
        /// </summary>
        GATEWAY_PROC_CALL = 2065,
        // If we need to support Get Option Board Random Pass Code, uncomment
        // the following, but beware!  Don't put LOCK_OPTICAL_PORT and
        // GET_OPTION_BRD_RANDOM_PASS_CODE in the same case statement
        ///// <summary>
        ///// MFG17 GET_OPTION_BRD_RANDOM_PASS_CODE = 2065
        ///// </summary>
        //GET_OPTION_BRD_RANDOM_PASS_CODE = 2065,
        /// <summary>
        /// MFG procedure 21
        /// </summary>
        CLEAR_OPT_BOARD_EVENTS = 2069,
        /// <summary>
        /// MFG procedure 22
        /// </summary>
        GET_LP_START_BLOCK = 2070,
        /// <summary>
        /// MFG procedure 23 - Memory Reader
        /// </summary>
        MEMORY_READER = 2071,
        /// <summary>
        /// MFG procedure 24
        /// </summary>
        SET_NEXT_CALL_TIME = 2072,	// Schedule Option Board Event
        /// <summary>
        /// MFG procedure 26 - Gets the current security level
        /// </summary>
        GET_SECURITY_LEVEL = 2074,
        /// <summary>
        /// MFG Procedure 27 - Turbo Test
        /// </summary>
        TURBO_TEST = 2075,
        /// <summary>
        /// MFG procedure 28
        /// </summary>
        CLEAR_VQ_LOG = 2076,	// Clears all VQ events
		/// <summary>
		/// MFG procedure 29
		/// </summary>
		CLEAR_SITESCAN_SNAPSHOTS = 2077,	// Clears all SiteScan Snapshots
        /// <summary>
        /// MFG procedure 37, Initiate firmware loader setup
        /// </summary>
        INITIATE_FW_LOADER_SETUP = 2085,
        /// <summary>
        /// MFG procedure 38, Connect/Disconnect
        /// </summary>
        REMOTE_CONNECT_DISCONNECT = 2086,
        /// <summary>
        /// MFG Procedure 39 - Perform Periodic Read
        /// </summary>
        PERFORM_PERIODIC_READ = 2087,
        /// <summary>
        /// MFG procedure 43, Reset RFLAN
        /// </summary>
        RESET_RF_LAN = 2091,
        /// <summary>
        /// MFG procedure 45, HAN Procedure.
        /// The request type can be:
        ///     Decommission and Halt,
        ///     Decommission and Commission, 
        ///     Drop Node, 
        ///     Register Node, 
        ///     Activate AlternateKey, 
        ///     Join Control,
        ///     Reset FW Download Client to Pending,
        ///     Activate FW on HAN Clients, and
        ///     Cancel FW Download and Activation
        /// </summary>
        HAN_PROCEDURE = 2093,
        /// <summary>
        /// MFG procedure 46, Get 1st 20 missing blocks
        /// </summary>
        GET_FIRST_20_MISSING_BLOCKS = 2094,
        /// <summary>
        /// MFG procedure 47, Performs RFLAN functions
        /// </summary>
        RFLAN_PROCEDURE = 2095,
        /// <summary>
        /// MFG procedure 50 - Clears the base data
        /// </summary>
        CLEAR_BASE = 2098,
        /// <summary>
        /// MFG procedure 52 - Clears the pending firmware download table by ID
        /// </summary>
        CLEAR_PENDING_TABLE_BY_ID = 2100,
        /// <summary>
        /// MFG procedure 55, Performs RFLAN debug functions
        /// </summary>
        RFLAN_DEBUG_PROCEDURE = 2103,
        /// <summary>
        /// MFG procedure 56 - Determines if Load Side Voltage is present - Takes 15 seconds to execute
        /// </summary>
        LOAD_SIDE_VOLTAGE_DETECTION = 2104,
        /// <summary>
        /// MFG Procedure 57 - Factory Remote Disconnect Switch test
        /// </summary>
        FACTORY_REMOTE_SWITCH_TEST = 2105,
        /// <summary>
        /// MFG procedure 60 - Reconfigures the active LED quantity
        /// </summary>
        RECONFIGURE_LED_QUANTITY = 2108,
        /// <summary>
        /// MFG procedure 61 - Switches the active Service Limiting threshold.
        /// </summary>
        SWITCH_ACTIVE_THRESHOLD = 2109,
        /// <summary>
        /// MFG procedure 63 - Preloads the register with the information it
        /// needs to allow a new device to join the HAN.  The 2 parameters are:
        ///     Device EUI - UINT64, EUI of the device added
        ///     Preconfigured Key - BINARY(16), the initial link key for this device
        /// </summary>
        ADD_AMI_HAN_DEVICE = 2111,
        /// <summary>
        /// MFG procedure 65 - Clears the LAN and HAN communication logs.  This will reset
        /// both the HAN2 upstream and downstream logs if they exist.
        /// </summary>
        RESET_LAN_HAN_COMM_LOGS = 2113,
        /// <summary>
        /// MFG procedure 69 - Activate a firmware download pending table.
        /// </summary>
        ACTIVATE_FWDL_PENDING_TABLE = 2117,
        /// <summary>
        /// Mfg procedure 75 - Key injection procedure. This should be used for automated 
        /// testing ONLY.
        /// </summary>
        KEY_INJECTION_PROCEDURE = 2123,
        /// <summary>
        /// MFG procedure 76 - Update temperature offset. Allows us to write the offset to factory data
        /// without causing a cold start
        /// </summary>
        UPDATE_TEMPERATURE_OFFSET = 2124,
        /// <summary>
        /// MFG procedure 77 - This procedure saves a new set of ECC certificate and 
        /// keys into dataflash.  It is used to load the Zigbee MCU and the Register 
        /// with information it needs to perform a Certificate Based Key Establishment 
        /// with Smart Energy devices supporting it.  The 4 parameters are: 
        ///     Device Certificate - UINT8[48], ECC certificate (meter specific).
        ///     CA Public Key - UINT8[22], ECC public key (not meter specific).
        ///     Device Private Key = UINT8[21], ECC private key (meter specific).
        ///     Flags = UINT8, Not used but set to 0x00.
        /// </summary>
        HAN_CERTIFICATE_UPDATE = 2125,
        /// <summary>
        /// MFG procedure 78 - Forces the meter to synchronize its time with the RFLAN
        /// if the meter's time is out of sync by a value greater than hysteresis.  The 
        /// parameter is:
        ///     Hysteresis - UINT16, the value, in seconds, that the meter's time must 
        ///                  be out of sync more than for the time sync to succeed.
        /// </summary>
        FORCE_TIME_SYNC = 2126,
        /// <summary>
        /// MFG procedure 80 - This function allows the user to start, extend, or cancel 
        /// a service limiting failsafe period.  The parameter is the two byte unsigned 
        /// number of minutes for the failsafe period to continue.  
        /// The specified duration starts when the message is received.
        ///     timeInFailSafe - UINT16, the value, in minutes, that the meter must be in failsafe.
        /// </summary>
        SERVICE_LIMITING_FAIL_SAFE = 2128,
        /// <summary>
        /// MFG procedure 81 - This function allows the user to enter or exit test mode
        /// This method takes 4 parameters, test mode type, duration in test mode, 
        /// test mode quantity and test mode kh value based on the meter form 
        /// </summary>
        ENTER_EXIT_TEST_MODE = 2129,
        /// <summary>
        /// MFG procedure 83 - Allows the user to validate security keys
        /// This procedure takes 3 parameters, security type, key id, and the hash 
        /// of the specified key.  The meter responds with a PROC_COMPLETE if the 
        /// validation passes.  Otherwise the meter responds with a PROC_INV_PARM 
        /// if the validation fails
        /// </summary>
        VALIDATE_SECURITY_DATA = 2131,
        /// <summary>
        /// MFG procedure 84 - Allows the user to encrypt the C12.18 passwords 
        /// stored in the meter.  This procedure takes no parameters.  The meter responds
        /// with a PROC_COMPLETE if the encryption procedure is successful.  If any
        /// of the encyrption functions fail it responds with ERR_CNST.
        /// </summary>
        ENCRYPT_C1218_PASSWORDS = 2132,
        /// <summary>
        /// MFG procedure 85 - Issue Signed Authorization. Takes the authorization key
        /// as a paramter. Returns PROC_COMPLETE if successful, PROC_NO_AUTH if it can not
        /// be processed, PROC_INV_PARM if the key is not valid, PROC_CONFLICT if the meter
        /// is not set up to use Signed Authorization, or PROC_IN_PROCESS if the command is
        /// still processing.
        /// </summary>
        AUTHENTICATE = 2133,
        /// <summary>
        /// MFG Procedure 87 - Disables Signed Authorization for a specified amount of time.
        /// </summary>
        DISABLE_SIGNED_AUTHORIZATION = 2135,
        /// <summary>
        /// MFG Procedure 91 - Modifies the operation of Fatal Error Recovery and can be used to clear
        /// the status bits.
        /// </summary>
        FATAL_ERROR_RECOVERY = 2139,
        /// <summary>
        /// MFG Procedure 93 - Clears the max difference in accelerometer angles since power up
        /// </summary>
        CLEAR_TAMPER_TAP_MAX_STATS = 2141,
        /// <summary>
        /// MFG Procedure 94 - Injects HAN events into the HAN2 event logs.
        /// </summary>
        HAN_EVENT_INJECTION = 2142,
        /// <summary>
        /// MFG Procedure 95 - Removes optional fields from the periodic read.
        /// </summary>
        PERIODIC_READ_REMOVAL = 2143,
        /// <summary>
        /// MFG Procedure 101 - Config Critical Peak Pricing
        /// </summary>
        CONFIG_CPP = 2149,
        /// <summary>
        /// MFG Procedure 103 - Ping HAN Device
        /// </summary>
        PING_HAN_DEVICE = 2151,
        /// <summary>
        /// MFG Procedure 104 - Config Critical Peak Pricing with HAN Data
        /// </summary>
        CONFIG_CPP_WITH_HAN_DATA = 2152,
         /// <summary>
        /// MFG procedure 105, AUTOMATION_TEST_COMMAND for Retry
        /// </summary>
        REMOTE_CONNECT_DISCONNECT_RETRY = 2153,
        /// <summary>
        /// MFG procedure 106, Event Injection Procedure
        /// </summary>
        EVENT_INJECTION = 2154,
        /// <summary>
        /// MFG procedure 107 - Configure Base Energies
        /// </summary>
        CONFIGURE_BASE_ENERGIES = 2155,
        /// <summary>
        /// MFG procedure 108 - Validate Base Energies
        /// </summary>
        VALIDATE_BASE_ENERGIES = 2156,
        /// <summary>
        /// MFG procedure 109 - Reconfigure LED Quantity for Poly meters
        /// </summary>
        POLY_LED_RECONFIGURE = 2157,
        /// <summary>
        /// MFG Procedure 111 - HAN Reset for HW 3.0 or later meters
        /// </summary>
        HAN_RESET = 2159,
        /// <summary>
        /// MFG Procedure 113 - Reset task execution times
        /// </summary>
        RESET_TASK_EXECUTION_TIME = 2161,
        /// <summary>
        /// MFG Procedure 115 - Firmware Download Event Injection
        /// </summary>
        FIRMWARE_DOWNLOAD_EVENT_INJECTION = 2163,
        /// <summary>
        /// MFG Procedure 116 - Clear the firmware download event log
        /// </summary>
        CLEAR_FWDL_EVENT_LOG = 2164,
        /// <summary>
        /// MFG Procedure 117 - Authenticate the optical firmware download by sending 
        /// 32-byte hash code to the meter
        /// </summary>
        OPTICAL_FWDL_AUTHENTICATION = 2165,
        /// <summary>
        /// MFG Procedure 119 - Disables RFLAN.
        /// </summary>
        RFLAN_OPT_OUT = 2167,
        /// <summary>
        /// MFG Procedure 141 - Causes the meter to detect which phases to monitor 
        /// in a single phase meter.
        /// </summary>
        MONO_PHASE_AUTO_DETECTION = 2189,
        /// <summary>
        /// MFG Procedure 150 - IP Stack Diagnostic and Test Procedure 
        /// </summary>
        IP_DIAGNOSTICS = 2198,
        /// <summary>
        /// Mfg Procedure 151 - Set PPP Key Procedure
        /// </summary>
        SET_PPP_KEY = 2199,
        /// <summary>
        /// Mfg Procedure 152 - Clear PPP Key Procedure
        /// </summary>
        CLEAR_PPP_KEY = 2200,
        /// <summary>
        /// Mfg Procedure 153 - Send PPP Key Procedure
        /// </summary>
        SEND_PPP_KEY = 2201,
        /// <summary>
        /// MFG Procedure 159 - IP Diagnotics Procedure 159
        /// </summary>
        IP_DIAGNOSTICS_159 = 2207,
        /// <summary>
        /// MFG Procedure 161 - Enable Full Fatal Error Checking
        /// </summary>
        ENABLE_FATAL_ERROR_CHECKING = 2209,
        /// <summary>
        /// MFG Procedure 164 - Switch between ChoiceConnect and OpenWay Communications Operational Modes
        /// </summary>
        SWITCH_CHOICE_CONNECT_OPENWAY_COMM_MODE = 2212,
        /// <summary>
        /// MFG Procedure 165 - HAN Disable Pricing
        /// This procedure is used to disable the current pricing mode.
        /// </summary>
        HAN_DISABLE_PRICING = 2213,
        /// <summary>
        /// MFG Procedure 166 - HAN Move Out command
        /// This procedure is used to clear the current HAN prices, messages, and DRLC events and to prevent 
        /// access to historical Load Profile and Block Period Consumption data.
        /// </summary>
        HAN_MOVE_OUT = 2214,
        /// <summary>
        /// MFG Procedure 167 - HAN Commit Next Block Price Schedule
        /// This procedure is used to commit the changes to Table 2441- Next Bock Price Schedule Table.
        /// </summary>
        HAN_COMMIT_NEXT_BLOCK_PRICE_SCHEDULE = 2215,
        /// <summary>
        /// MFG Procedure 168 - HAN Update CurrentBlockPeriodConsumptionDelivered.
        /// </summary>
        HAN_UPDATE_CURRENT_BLOCK_PERIOD_CONSUMPTION_DELIVERED = 2216,
        /// <summary>
        /// MFG Procedure 169 - HAN Get Current Block Price Data
        /// </summary>
        HAN_GET_CURRENT_BLOCK_PRICE_DATA = 2217,
        /// <summary>
        /// MFG Procedure 170 - Read stack depth
        /// </summary>
        READ_STACK_DEPTH = 2218,
        /// <summary>
        /// MFG Procedure 171 - Exit factory test mode
        /// </summary>
        EXIT_FACTORY_TEST_MODE = 2219,
        /// <summary>
        /// MFG Procedure 174 - Enable/Disable Cert Mode
        /// </summary>
        TOGGLE_ZIGBEE_CERTIFICATION_MODE = 2222,
        /// <summary>
        /// MFG Procedure 176 - Cancel Scheduled Firmware Activation
        /// </summary>
        CANCEL_SCHEDULED_FIRMWARE_ACTIVATION = 2224,
        /// <summary>
        /// MFG Procedure 177 - Enable or Disable Optical Port
        /// </summary>
        ENABLE_DISABLE_OPTICAL_PORT = 2225,
        /// <summary>
        /// MFG Procedure 200 - ICS Module Procedure
        /// </summary>
        ICS_MODULE_PROCEDURE = 2248,
        /// <summary>
        /// MFG Procedure 205 - HAN OTA Certification
        /// </summary>
        HAN_OTA_Certification = 2253,
    }

    /// <summary>
    /// Data Structure for calling Periodic Read
    /// </summary>
    public struct PeriodicReadActions
    {
        #region Constructor

        /// <summary>
        /// Constructor For Period read Actions Struct
        /// </summary>
        /// <param name="bDefault"></param>
        public PeriodicReadActions(bool bDefault)
        {
            bReportZigBee = bDefault;
            bReportDRSnapshot = bDefault;
            bReportPerformSR = bDefault;
            bReportLoadProfile = bDefault;
            bReportHistoryLog = bDefault;
            bReportTable28 = bDefault;
            bReportNetworkStatus = bDefault;
            bPerformDRAtEndTime = bDefault;
            bReportMostRecentSR = bDefault;
            bReportVoltageMonitoring = bDefault;
            bReportFWDLStatusBlock = bDefault;
            bOnDemandPeriodicRead = bDefault;
            bReportTable6 = bDefault;
        }

        #endregion

        #region Members
        /// <summary>
        /// Report ZigBee
        /// </summary>
        public bool bReportZigBee;
        /// <summary>
        /// Report Demand reset Snapshot
        /// </summary>
        public bool bReportDRSnapshot;
        /// <summary>
        /// Report Perform Self Read
        /// </summary>
        public bool bReportPerformSR;
        /// <summary>
        /// Report Load Profile
        /// </summary>
        public bool bReportLoadProfile;
        /// <summary>
        /// Report History Log
        /// </summary>
        public bool bReportHistoryLog;
        /// <summary>
        /// Report Table 28
        /// </summary>
        public bool bReportTable28;
        /// <summary>
        /// Report Network status
        /// </summary>
        public bool bReportNetworkStatus;
        /// <summary>
        /// Perform Demand Reset at End Time
        /// </summary>
        public bool bPerformDRAtEndTime;
        /// <summary>
        /// Report Most Recent Self read
        /// </summary>
        public bool bReportMostRecentSR;
        /// <summary>
        /// Report Voltage Monitoring
        /// </summary>
        public bool bReportVoltageMonitoring;
        /// <summary>
        /// Report Firmware Download Status Block
        /// </summary>
        public bool bReportFWDLStatusBlock;
        /// <summary>
        /// On Demand Periodic read
        /// </summary>
        public bool bOnDemandPeriodicRead;
        /// <summary>
        /// Report Table 06
        /// </summary>
        public bool bReportTable6;
        #endregion
    }

    /// <summary>
    /// The bit defintions for the various Period Read Actions
    /// </summary>
    internal enum Periodic_Read_Action_Types
    {
        REPORT_ZIGBEE               = 0x0001,
        REPORT_DR_SNAPSHOT          = 0x0002,
        REPORT_PERFORM_SR           = 0x0004,
        REPORT_LOAD_PROFILE         = 0x0008,
        REPORT_HISTORY_LOG          = 0x0010,
        REPORT_TABLE_28             = 0x0020,
        REPORT_NETWORK_STATUS       = 0x0040,
        PERFROM_DR_AT_ENDTIME       = 0x0080,
        REPORT_MOST_RECENT_SR       = 0x0100,
        REPORT_VOLT_MONITORING      = 0x0200,
        REPORT_FWDL_STATUS_BLOCK    = 0x4000,
        ON_DEMAND_PERIODIC_READ     = 0x0800,
        REPORT_TABLE_06             = 0x1000,
    }

    /// <summary>
    /// The bits indicating which item to reset via MFG procedure 6
    /// </summary>
    public enum Reset_Counter_Types
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
        /// Reset diagnostic counters
        /// </summary>
        RESET_NUM_DIAG_COUNTERS = 0x00000008,
    }

    /// <summary>
    /// Parameters for STD Procedure 4
    /// </summary>
    public enum ResetListPointerTypes : byte
    {
        /// <summary>
        /// Resets the Event Logger pointer
        /// </summary>
        EventLogger = 1,
        /// <summary>
        /// Resets the Self Read pointer
        /// </summary>
        SelfReads = 2,
        /// <summary>
        /// Reset the Load Profile Data Set 1 pointer
        /// </summary>
        LPDataSet1 = 3,
        /// <summary>
        /// Reset the Load Profile Data Set 2 pointer
        /// </summary>
        LPDataSet2 = 4,
        /// <summary>
        /// Reset the Load Profile Data Set 3 pointer
        /// </summary>
        LPDataSet3 = 5,
        /// <summary>
        /// Reset the Load Profile Data Set 4 pointer
        /// </summary>
        LPDataSet4 = 6,
        /// <summary>
        /// Reset all Load Profile pointers
        /// </summary>
        AllLPData = 7,
        /// <summary>
        /// Reset the History Logger pointer
        /// </summary>
        HistoryLogger = 8,
        /// <summary>
        /// Reset all but the History Logger pointer
        /// </summary>
        AllButHistoryLogger = 255,
    }

    /// <summary>
    /// Result codes that are returned in table 08 after a procedure has
    /// been written to table 07
    /// </summary>
    public enum ProcedureResultCodes
    {
        /// <summary>
        /// COMPLETED - Procedure success
        /// </summary>
        [EnumDescription("COMPLETED - Procedure success")]
        COMPLETED = 0,
        /// <summary>
        /// NOT_FULLY_COMPLETED - Procedure in-progress
        /// </summary>
        [EnumDescription("NOT_FULLY_COMPLETED - Procedure in-progress")]
        NOT_FULLY_COMPLETED = 1,
        /// <summary>
        /// INVALID_PARAM - Procedure ignored
        /// </summary>
        [EnumDescription("INVALID_PARAM - Procedure ignored")]
        INVALID_PARAM = 2,
        /// <summary>
        /// DEVICE_SETUP_CONFLICT - Procedure ignored
        /// </summary>
        [EnumDescription("DEVICE_SETUP_CONFLICT - Procedure ignored")]
        DEVICE_SETUP_CONFLICT = 3,
        /// <summary>
        /// TIMING_CONSTRAINT - Procedure ignored
        /// </summary>
        [EnumDescription("TIMING_CONSTRAINT - Procedure ignored")]
        TIMING_CONSTRAINT = 4,
        /// <summary>
        /// NO_AUTHORIZATION - Procedure ignored
        /// </summary>
        [EnumDescription("NO_AUTHORIZATION - Procedure ignored")]
        NO_AUTHORIZATION = 5,
        /// <summary>
        /// UNRECOGNIZED_PROC - Procedure ignored
        /// </summary>
        [EnumDescription("UNRECOGNIZED_PROC - Procedure ignored")]
        UNRECOGNIZED_PROC = 6
    }

    /// <summary>
    /// Flag values to be used in any combination with std procedure 09
    /// to perform remote resets
    /// </summary>
    public enum RemoteResetProcedureFlags
    {
        /// <summary>
        /// Demand Reset
        /// </summary>
        DEMAND_RESET = 0x01,
        /// <summary>
        /// Perform self read 
        /// </summary>
        SELF_READ = 0x02,
        /// <summary>
        /// Change to season indicated by bits (3..6)
        /// </summary>
        SEASON_CHANGE = 0x04,
    }

    /// <summary>
    /// List pointers that can be updated with std procedure 05
    /// </summary>
    public enum UpdateLastReadEntryListIDs
    {
        /// <summary>
        /// Clears the Canadian event log 
        /// </summary>
        CANADIAN_EVENT_LOG = 1,
    }

    /// <summary>
    /// RFLAN debug procedures that can be called with mfg procedure 55.
    /// </summary>
    internal enum RFLANDebugProcedures
    {
        /// <summary>
        /// RSSI PDF start
        /// </summary>
        RSSI_PDF_START = 0,
        /// <summary>
        /// RSSI PDF end
        /// </summary>
        RSSI_PDF_END = 1,
        /// <summary>
        /// RSSI autocor start
        /// </summary>
        RSSI_AUTOCOR_START = 2,
        /// <summary>
        /// RSSI autocor end
        /// </summary>
        RSSI_AUTOCOR_END = 3,
        /// <summary>
        /// Locks the RF channel
        /// </summary>
        LOCK_RF_CHANNEL = 4,
        /// <summary>
        /// Resumes RF hopping sequence/unlocks RF channel
        /// </summary>
        RESUME_RF_HOPPING_SEQUENCE = 5,
        /// <summary>
        /// Halts stack execution
        /// </summary>
        HALT_STACK_EXECUTION = 6,
        /// <summary>
        /// Enables stack execution
        /// </summary>
        ENABLE_STACK_EXECUTION = 7,
        /// <summary>
        /// Forced cell out
        /// </summary>
        FORCED_CELL_OUT = 8,
        /// <summary>
        /// Sets the read routing table's base index
        /// </summary>
        SET_READ_ROUTING_TABLE_BASE_INDEX = 9,
        /// <summary>
        /// Sets the ITP time
        /// </summary>
        SET_ITP_TIME = 10,
        /// <summary>
        /// Begins an outage
        /// </summary>
        BEGIN_OUTAGE = 11,
        /// <summary>
        /// Ends an outage
        /// </summary>
        END_OUTAGE = 12,
        /// <summary>
        /// Sets the preferred cell ID
        /// </summary>
        SET_PREFERRED_CELL = 13,
        /// <summary>
        /// Sets the endpoint type
        /// </summary>
        SET_ENDPOINT_TYPE = 14,
        /// <summary>
        /// Pokes the memory
        /// </summary>
        MEMORY_POKE = 15,
        /// <summary>
        /// Forces an outage
        /// </summary>
        FORCE_OUTAGE = 16,
        /// <summary>
        /// Forces a soft net registration
        /// </summary>
        FORCE_SOFT_NET_REGISTRATION = 17,
        /// <summary>
        /// Resets the C12.22 stack
        /// </summary>
        C1222_STACK_RESET = 18,
    }

    /// <summary>
    /// RFLAN procedures that can be called with mfg procedure 47.
    /// </summary>
    internal enum RFLANProcedure
    {
        /// <summary>
        /// Jumps to bootloader mode
        /// </summary>
        JUMP_TO_BOOTLOADER = 0,
        /// <summary>
        /// Transmits RF message on low channel
        /// </summary>
        RF_MSG_TX_LOW_CHANNEL = 1,
        /// <summary>
        /// Transmits RF message on high channel
        /// </summary>
        RF_MSG_TX_HIGH_CHANNEL = 2,
        /// <summary>
        /// Recieves RF message on low channel
        /// </summary>
        RF_MSG_RX_LOW_CHANNEL = 3,
        /// <summary>
        /// Recieves RF message on high channel
        /// </summary>
        RF_MSG_RX_HIGH_CHANNEL = 4,
        /// <summary>
        /// Transmits RF message on low channel and then on high channel
        /// </summary>
        RF_MSG_TX_LOW_THEN_HIGH_CHANNEL = 6,
        /// <summary>
        /// Resets debug events
        /// </summary>
        RESET_DEBUG_EVENTS = 7,
        /// <summary>
        /// Resets RFLAN MCU
        /// </summary>
        RESET_RFLAN_MCU = 9,
        /// <summary>
        /// Resets MAC
        /// </summary>
        MAC_RESET = 13,
        /// <summary>
        /// Causes a core dump
        /// </summary>
        CORE_DUMP = 16,
        /// <summary>
        /// Clears a core dump
        /// </summary>
        CLEAR_CORE_DUMP = 17,
        /// <summary>
        /// Sends notification of a factory config change
        /// </summary>
        FACTORY_CONFIG_CHANGE_NOTIFY = 21,
        /// <summary>
        /// Forces a NET registration
        /// </summary>
        FORCE_NET_REGISTER = 22,
        /// <summary>
        /// Clears all statistics
        /// </summary>
        CLEAR_STATISTICS = 23,
        /// <summary>
        /// Reauthorizes all cells
        /// </summary>
        REAUTHORIZE_ALL_CELLS = 24,
        /// <summary>
        /// Forces ITP synchronization
        /// </summary>
        FORCE_ITP_SYNCHRONIZATION = 25,
        /// <summary>
        /// Forces cell leaving
        /// </summary>
        FORCE_CELL_LEAVING = 26,
        /// <summary>
        /// Clears flash lock bits
        /// </summary>
        CLEAR_FLASH_LOCK_BITS = 27,
    }

    /// <summary>
    /// Function codes for MFG proc 150
    /// </summary>
    internal enum IPDiagnosticsFunction150ID
    {
        /// <summary>
        /// This function id puts diagnostic messages on the meter's display
        /// </summary>
        SET_DIAGNOSTICS_DISPLAYS_ACTIVATION = 1,
        /// <summary>
        /// This function id enables or disables the visibility of the 500 and 600 tables.
        /// This function id is obsolete but must be kept for register f/w 5.0 meters
        /// </summary>
        SET_DIAGNOSTICS_TABLES_ACTIVATION = 2,
        /// <summary>
        /// Send Direct COAP
        /// </summary>
        SEND_RAW_CSMP = 6,
        /// <summary>
        /// Get List of TLVs supported by the register
        /// </summary>
        GET_LIST_OF_REG_SUPPORTED_TLVS = 7,
        /// <summary>
        /// Initiate Secure PPP Connection
        /// </summary>
        INITIATE_SECURE_PPP = 13,
    }

    internal enum IPDiagnosticsFunction159ID
    {
        /// <summary>
        /// This function id enables or disables the visibility of the 500 and 600 tables
        /// </summary>
        SET_DIAGNOSTICS_TABLES_ACTIVATION = 2,
        /// <summary>
        /// Set TLV to module
        /// </summary>
        SET_TLV_TO_MODULE = 3,
        /// <summary>
        /// Trigger Module TLV Read
        /// </summary>
        TRIGGER_MODULE_TLV_READ = 10,
        /// <summary>
        /// IP and COMM Reset Test Function
        /// </summary>
        IP_AND_COMM_RESET_TEST_FUNCTION = 11,
        /// <summary>
        /// Enable/Disable QoS
        /// </summary>
        ENABLE_DISABLE_QOS = 12,
        /// <summary>
        /// Generate PPP PSK
        /// </summary>
        GENERATE_PPP_PSK = 13,
        /// <summary>
        /// Unlock Register Secure PPP
        /// </summary>
        UNLOCK_REGISTER_SECURE_PPP = 14,
        /// <summary>
        /// Unlock COMM Module Secure PPP
        /// </summary>
        UNLOCK_COMM_MODULE_SECURE_PPP = 15,
        /// <summary>
        /// Set the stack type
        /// </summary>
        SET_STACK_TYPE = 16,
    }

    /// <summary>
    /// Action codes for Mfg procedure 159, subfunction 11
    /// </summary>
    internal enum IPResetActionaID
    {
        /// <summary>
        /// IP Reset counts
        /// </summary>
        IP_RESET_COUNTS = 0,
        /// <summary>
        /// IP Reset start/stop UIP
        /// </summary>
        IP_RESET_START_STOP_UIP = 1,
        /// <summary>
        /// IP Reset UIP task restart
        /// </summary>
        IP_RESET_UIP_TASK_RESTART = 2,
        /// <summary>
        /// IP Reset comm module restart
        /// </summary>
        IP_RESET_COMM_MODULE_RESTART = 3,
        /// <summary>
        /// IP Reset UIP stack frozen
        /// </summary>
        IP_RESET_UIP_STACK_FROZEN = 4,
    }

    /// <summary>
    /// ICS procedures that can be called with mfg procedure 200.
    /// </summary>
    internal enum ICSProcedure
    {
        /// <summary>
        /// Execute a diagnostic command for the ICS module
        /// </summary>
        EXECUTE_CLI_COMMAND = 1,
        /// <summary>
        /// Update the ERT data tables (460, 462 and 463)
        /// </summary>
        UPDATE_ERT_DATA_TABLES = 2,
        /// <summary>
        /// Verifies the passwords in the comm module.
        /// </summary>
        VERIFY_METER_PASSWORDS = 3,
        /// <summary>
        /// Commits the cellular configuration values to flash.
        /// </summary>
        COMMIT_CONFIGURATION = 4,
        /// <summary>
        /// Clears the ICS event log
        /// </summary>
        RESET_EVENT_LOG = 6,
        /// <summary>
        /// Freezes the comm module event tables for the specified date range.
        /// </summary>
        UPDATE_EVENT_TABLES = 7,
        /// <summary>
        /// Filters the ICS Events by either Customer or Diagnostic events
        /// </summary>
        ICM_SELECT_EVENT_FILTER = 9,
        /// <summary>
        /// Clear configuration status bits
        /// </summary>
        CLEAR_CONFIGURATION_STATUS_BITS = 10,
        /// <summary>
        /// ERT 242 Command Data
        /// </summary>
        ERT_242_COMMAND_DATA = 11,
        /// <summary>
        /// ERT Add/Remove Meter
        /// </summary>
        ERT_ADD_REMOVE_METER = 12,
        /// <summary>
        /// Set ZigBee Network Key (I-210/kV2c only)
        /// </summary>
        SET_ZIGBEE_NETWORK_KEY = 13,
        /// <summary>
        /// Get ZigBee Network Key (I-210/kV2c only)
        /// </summary>
        GET_ZIGBEE_NETWORK_KEY = 14,
        /// <summary>
        /// Set Meter Password
        /// </summary>
        SET_METER_PASSWORD = 15,
        /// <summary>
        /// Initiate retrieval of debug logs
        /// </summary>
        INITIATE_FILE_RETRIEVAL = 16,
        /// <summary>
        /// Continue retrieval of debug logs
        /// </summary>
        CONTINUE_FILE_RETRIEVAL = 17,
    }

    /// <summary>
    /// ICS Filter Selection used for Procedure 200. This is the second parameter passed in when using
    /// MFG Procedure 200 sub-function 9.
    /// </summary>
    public enum ICSFilterSelection
    {
        /// <summary>
        /// Does not filter the ICS Events.
        /// </summary>
        NO_FILTER = 0,
        /// <summary>
        /// Filters events to only allow Customer Events.
        /// </summary>
        CUSTOMER_FILTER = 1,
        /// <summary>
        /// Filters events to only allow Diagnostic Events.
        /// </summary>
        DIAGNOSTIC_FILTER = 2,
    }

    /// <summary>
    /// Bitfield for method HANChangePriceMode()
    /// </summary>
    public enum HANPriceModes
    {
        /// <summary>Time Of Use</summary>
        SIMPLE_TOU_PRICING = 1,
        /// <summary>RIB - Block</summary>
        BLOCK_PRICING = 2,
        /// <summary>Recurring Pricing</summary>
        RECURRING_PRICING = 4
    }
}
