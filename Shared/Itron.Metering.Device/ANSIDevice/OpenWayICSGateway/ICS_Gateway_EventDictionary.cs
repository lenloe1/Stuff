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
//                        Copyright © 2012 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Constructs a dictionary of Centron AMI specific events
    /// </summary>
    public class ICS_Gateway_EventDictionary : CENTRON_AMI_EventDictionary
    {
        #region Constants

        /// <summary>
        /// ICS Offset (3584)
        /// </summary>
        public const int ICS_OFFSET = 0xE00;

        #endregion

        #region Definitions

        /// <summary>
        /// History Events Enumeration
        /// </summary>
        /// <remarks>
        /// If you add any events to this enumeration, be sure to add them to
        /// HistoryLogEventList property in respective MFGTable2048 classes as well
        /// as adding a dictionary entry in the appropriate EventDictionary.
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY Who Version    Issue#        Description
        //  -------- --- -------    ------        -------------------------------------------
        //  03/22/13 MSC 2.80.09    TR7640        Created
        //  05/06/13 MSC 2.80.28    TR7640        Updated with Table Definition
        //  07/11/13 AF  2.80.51    WR417110      Updated with more events
        //  07/12/13 AF  2.80.51    WR417110      Updated with more events
        //  08/12/13 AF  2.85.16    No WR         We had the wrong id for the event log cleared event.
        //  04/07/14 AF  3.50.62    WR466261      Updated with more events
        //  05/16/14 jrf 3.50.95    WR516785      Added two new events.
        //  05/16/14 jrf 3.50.95    WR504256      Switched the event numbers for CC1121PartNumber 
        //                                           and SucessfullyInitializedSPIDriver events.
        //  04/05/16 CFB 4.50.244   WR603380      Modified names of several 100G specific meter events
        //                                           to generic names to match CE
        //  05/31/16 MP  4.50.274                 Swapped Events 114 and 116. We had them backwards
        //  09/02/16 AF  4.70.16    No WR         Corrected merge problems.
        //
        [DataContract(Name = "CommModuleHistoryEvents")]
        public enum CommModuleHistoryEvents
        {
            /// <summary>
            /// UNINTELLIGIBLE MESSAGE RECEIVED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            UNINTELLIGIBLE_MESSAGE_RECEIVED = ICS_OFFSET,
            /// <summary>
            /// FAILURE TO SET METER TIME
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FAILURE_TO_SET_METER_TIME = ICS_OFFSET + 1,
            /// <summary>
            /// SYNCHRONIZATION TIME ERROR IS TOO LARGE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SYNCHRONIZATION_TIME_ERROR_IS_TOO_LARGE = ICS_OFFSET + 2,
            /// <summary>
            /// METER COMMUNICATION FAULT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_COMMUNICATION_FAULT = ICS_OFFSET + 4,
            /// <summary>
            /// METER COMMUNICATION REESTABLISHED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_COMMUNICATION_REESTABLISHED = ICS_OFFSET + 6,
            /// <summary>
            /// POWER OUTAGE DETECTED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_OUTAGE_DETECTED = ICS_OFFSET + 7,
            /// <summary>
            /// METER ERROR GREATER THAN MAXIMUM CORRECTABLE TIME ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_ERROR_GREATER_THAN_MAXIMUM_CORRECTABLE_TIME_ERROR = ICS_OFFSET + 8,
            /// <summary>
            /// TAMPER DETECTION - PASSWORD RECOVERY DETECTED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_PASSWORD_RECOVERY_DETECTED = ICS_OFFSET + 9,
            /// <summary>
            /// TOTAL DEMAND RESETS
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_TOTAL_DEMAND_RESETS = ICS_OFFSET + 10,
            /// <summary>
            /// OPTICAL PORT SESSION IN PROGRESS
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_OPTICAL_PORT_SESSION_IN_PROGRESS = ICS_OFFSET + 11,
            /// <summary>
            /// DEVICE RECONFIGURED REPROGRAMMED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_DEVICE_RECONFIGURED_REPROGRAMMED = ICS_OFFSET + 12,
            /// <summary>
            /// SERVICE ERROR DETECTED (KV2C ONLY)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_SERVICE_ERROR_DETECTED = ICS_OFFSET + 13,
            /// <summary>
            /// PASSWORD FAILURE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_PASSWORD_FAILURE = ICS_OFFSET + 14,
            /// <summary>
            /// POWER OUTAGES RECOGNIZED BY ICM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_POWER_OUTAGES_RECOGNIZED_BY_SSI_MODULE = ICS_OFFSET + 15,
            /// <summary>
            /// POWER OUTAGES DUE TO LOSS OF AC POWER
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_POWER_OUTAGES_DUE_TO_LOSS_OF_AC_POWER = ICS_OFFSET + 16,
            /// <summary>
            /// TILT SWITCH SET
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_TILT_SWITCH_SET = ICS_OFFSET + 17,
            /// <summary>
            /// TILT SWITCH CLEARED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_TILT_SWITCH_CLEARED = ICS_OFFSET + 18,
            /// <summary>
            /// REMOTE DISCONNECT SWITCH BYPASS
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_REMOTE_DISCONNECT_SWITCH_BYPASS = ICS_OFFSET + 19,
            /// <summary>
            /// MODULE INVERTED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_DETECTION_MODULE_INVERTED = ICS_OFFSET + 20,
            /// <summary>
            /// EXTREME TEMPERATURE SHUTDOWN
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            EXTREME_TEMPERATURE_SHUTDOWN = ICS_OFFSET + 21,
            /// <summary>
            /// EXTREME TEMPERATURE INSERVICE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            EXTREME_TEMPERATURE_INSERVICE = ICS_OFFSET + 22,
            /// <summary>
            /// FIRMWARE IMAGE CORRUPTED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FIRMWARE_IMAGE_CORRUPTED = ICS_OFFSET + 23,
            /// <summary>
            /// POWER QUALITY DETECTION POLARITY, CROSS-PHASE, AND ENERGY FLOW (DIAGNOSTIC 1)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_DIAGNOSTIC_1 = ICS_OFFSET + 24,
            /// <summary>
            /// POWER QUALITY DETECTION VOLTAGE IMBALANCE (DIAGNOSTIC 2)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_DIAGNOSTIC_2 = ICS_OFFSET + 25,
            /// <summary>
            /// POWER QUALITY DETECTION INACTIVE PHASE CURRENT (DIAGNOSTIC 3)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_DIAGNOSTIC_3 = ICS_OFFSET + 26,
            /// <summary>
            /// POWER QUALITY DETECTION CURRENT PHASE ANGLE (DIAGNOSTIC 4)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_DIAGNOSTIC_4 = ICS_OFFSET + 27,
            /// <summary>
            /// POWER QUALITY DETECTION HIGH DISTORTION (DIAGNOSTIC 5)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_DIAGNOSTIC_5 = ICS_OFFSET + 28,
            /// <summary>
            /// POWER QUALITY DETECTION UNDER VOLTAGE TEST (DIAGNOSTIC 6)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_DIAGNOSTIC_6 = ICS_OFFSET + 29,
            /// <summary>
            /// POWER QUALITY DETECTION OVER VOLTAGE TEST (DIAGNOSTIC 7)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_DIAGNOSTIC_7 = ICS_OFFSET + 30,
            /// <summary>
            /// POWER QUALITY DETECTION HIGH IMPUTED NEUTRAL CURRENT (DIAGNOSTIC 8)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_DIAGNOSTIC_8 = ICS_OFFSET + 31,
            /// <summary>
            /// POWER QUALITY DETECTION VOLTAGE SAG
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_VOLTAGE_SAG = ICS_OFFSET + 32,
            /// <summary>
            /// POWER QUALITY DETECTION VOLTAGE SWELL
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_VOLTAGE_SWELL = ICS_OFFSET + 33,
            /// <summary>
            /// METER PASSWORDS OUT OF SYNC
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_PASSWORDS_OUT_OF_SYNC = ICS_OFFSET + 34,
            /// <summary>
            /// RCDC FAULT RCDC COMM ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RCDC_FAULT_RCDC_COMM_ERROR = ICS_OFFSET + 35,
            /// <summary>
            /// RCDC FAULT SWITCH CONTROLLER ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RCDC_FAULT_SWITCH_CONTROLLER_ERROR = ICS_OFFSET + 36,
            /// <summary>
            /// RCDC FAULT SWITCHED FAILED TO CLOSE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RCDC_FAULT_SWITCHED_FAILED_TO_CLOSE = ICS_OFFSET + 37,
            /// <summary>
            /// RCDC FAULT ALTERNATE SOURCE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RCDC_FAULT_ALTERNATE_SOURCE = ICS_OFFSET + 38,
            /// <summary>
            /// RCDC FAULT BYPASSED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RCDC_FAULT_BYPASSED = ICS_OFFSET + 39,
            /// <summary>
            /// RCDC FAULT SWITCH FAILED TO OPEN
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RCDC_FAULT_SWITCH_FAILED_TO_OPEN = ICS_OFFSET + 40,
            /// <summary>
            /// RCDC FAULT PPM ALERT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RCDC_FAULT_PPM_ALERT = ICS_OFFSET + 41,
            /// <summary>
            /// RCDC FAULT MANUAL ARM TIMED OUT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RCDC_FAULT_MANUAL_ARM_TIMED_OUT = ICS_OFFSET + 42,
            /// <summary>
            /// AUTO REGISTRATION (CORE)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            AUTO_REGISTRATION = ICS_OFFSET + 43,
            /// <summary>
            /// SUCCESSFUL TIME SYNC TIME CHANGE OCCURED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SUCCESSFUL_TIME_SYNC_TIME_CHANGE_OCCURRED = ICS_OFFSET + 44,
            /// <summary>
            /// POWER RESTORATION DETECTED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_RESTORATION_DETECTED = ICS_OFFSET + 45,
            /// <summary>
            /// IP ADDRESS REPORT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            IP_ADDRESS_REPORT = ICS_OFFSET + 46,
            /// <summary>
            /// DELAYED RESET SSI MODULE ALARM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            DELAYED_RESET_SSI_MODULE_ALARM = ICS_OFFSET + 47,
            /// <summary>
            /// METER COMMUNICATION STATUS ALARM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            METER_COMMUNICATION_STATUS_ALARM = ICS_OFFSET + 48,
            /// <summary>
            /// EXTENSIBLE FIRMWARE DOWNLOAD STATUS
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            EXTENSIBLE_FIRMWARE_DOWNLOAD_STATUS = ICS_OFFSET + 49,
            /// <summary>
            /// SET ACTIVE FIRMWARE ALARM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SET_ACTIVE_FIRMWARE_ALARM = ICS_OFFSET + 50,
            /// <summary>
            /// FIRMWARE UPGRADE DOWNLOAD ALARM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FIRMWARE_UPGRADE_DOWNLOAD_ALARM = ICS_OFFSET + 51,
            /// <summary>
            /// FIRMWARE UPGRADE ACTIVE ALARM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FIRMWARE_UPGRADE_ACTIVE_ALARM = ICS_OFFSET + 52,
            /// <summary>
            /// FIRMWARE DOWNLOAD COPYING FILE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_COPYING_FILE = ICS_OFFSET + 53,
            /// <summary>
            /// FIRMWARE DOWNLOAD CANCELING
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_CANCELING = ICS_OFFSET + 54,
            /// <summary>
            /// FIRMWARE DOWNLOAD CANCELED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_CANCELED = ICS_OFFSET + 55,
            /// <summary>
            /// FIRMWARE DOWNLOAD TOTAL TIME
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_TOTAL_TIME = ICS_OFFSET + 56,
            /// <summary>
            /// FIRMWARE DOWNLOAD SUCCESSFUL
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_SUCCESSFUL = ICS_OFFSET + 57,
            /// <summary>
            /// FIRMWARE DOWNLOAD REMOVING INACTIVE FILES
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_REMOVING_INACTIVE_FILES = ICS_OFFSET + 58,
            /// <summary>
            /// FIRMWARE DOWNLOAD RETRIES EXCEEDED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_RETRIES_EXCEEDED = ICS_OFFSET + 59,
            /// <summary>
            /// FIRMWARE DOWNLOAD FAILED WILL RETRY
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_FAILED_WILL_RETRY = ICS_OFFSET + 60,
            /// <summary>
            /// FIRMWARE DOWNLOAD INCORRECT VERSION
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_INCORRECT_VERSION = ICS_OFFSET + 61,
            /// <summary>
            /// FIRMWARE DOWNLOAD FILE EXISTS
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_FILE_EXISTS = ICS_OFFSET + 62,
            /// <summary>
            /// FIRMWARE DOWNLOAD ACTIVATING
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_ACTIVATING = ICS_OFFSET + 63,
            /// <summary>
            /// FIRMWARE DOWNLOAD SET ACTIVE REBOOT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            [EnumMember()]
            FIRMWARE_DOWNLOAD_SET_ACTIVE_REBOOT = ICS_OFFSET + 64,
            /// <summary>
            /// COMM LINK FAILURE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            COMM_LINK_FAILURE = ICS_OFFSET + 65,
            /// <summary>
            /// ICM REBOOT MODEM NOT RESPONDING
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_REBOOT_MODEM_NOT_RESPONDING = ICS_OFFSET + 66,
            /// <summary>
            /// INITIAL MODEM PROVISION
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            INITIAL_MODEM_PROVISION = ICS_OFFSET + 67,
            /// <summary>
            /// MODEM PROVISION FAILED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MODEM_PROVISION_FAILED = ICS_OFFSET + 68,
            /// <summary>
            /// MODEM PROVISION SUCCESSFUL
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MODEM_PROVISION_SUCCESSFUL = ICS_OFFSET + 69,
            /// <summary>
            /// MODEM IDENTITY ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MODEM_IDENTITY_ERROR = ICS_OFFSET + 70,
            /// <summary>
            /// CDMA SUBSCRIPTION ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CDMA_SUBSCRIPTION_ERROR = ICS_OFFSET + 71,
            /// <summary>
            /// MDN LOGIN ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MDN_LOGIN_ERROR = ICS_OFFSET + 72,
            /// <summary>
            /// RECEIVED SMS
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RECEIVED_SMS = ICS_OFFSET + 73,
            /// <summary>
            /// GATEWAY CHANGED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            GATEWAY_CHANGED = ICS_OFFSET + 74,
            /// <summary>
            /// CELLUAR NON-COMM TIMEOUT SENT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CELLUAR_TIMEOUT_SENT = ICS_OFFSET + 75,
            /// <summary>
            /// CELLUAR NON-COMM TIMEOUT RECEIVED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CELLUAR_TIMEOUT_RECEIVED = ICS_OFFSET + 76,
            /// <summary>
            /// CAN'T UPDATE DISPLAY UNKNOWN METER TYPE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            UNKNOWN_METER_TYPE = ICS_OFFSET + 77,
            /// <summary>
            /// METER MODEL I-210C
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_MODEL_I_210C = ICS_OFFSET + 78,
            /// <summary>
            /// METER MODEL I-210
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_MODEL_I_210 = ICS_OFFSET + 79,
            /// <summary>
            /// METER MODEL KV2C
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_MODEL_KV2C = ICS_OFFSET + 80,
            /// <summary>
            /// METER INVERSION DETECTED ON STARTUP
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_INVERSION_DETECTED_ON_STARTUP = ICS_OFFSET + 81,
            /// <summary>
            /// MODULE POWER FAIL
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MODULE_POWER_FAIL = ICS_OFFSET + 82,
            /// <summary>
            /// QUALIFIED POWER FAIL
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            QUALIFIED_POWER_FAIL = ICS_OFFSET + 83,
            /// <summary>
            /// ICM HEATER ENABLED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_HEATER_ENABLED = ICS_OFFSET + 84,
            /// <summary>
            /// ICM HEATER DISABLED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_HEATER_DISABLED = ICS_OFFSET + 85,
            /// <summary>
            /// CLI PASSWORD FAILED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CLI_PASSWORD_FAILED = ICS_OFFSET + 86,
            /// <summary>
            /// ICM ENTERING QUIET MODE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_ENTERING_QUIET_MODE = ICS_OFFSET + 87,
            /// <summary>
            /// ICM LEAVING QUIET MODE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_LEAVING_QUIET_MODE = ICS_OFFSET + 88,
            /// <summary>
            /// MAGNETIC SWIPE IN MANUFACTURING MODE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MAGNETIC_SWIPE_IN_MANUFACTURING_MODE = ICS_OFFSET + 89,
            /// <summary>
            /// MAGNETIC SWIPE IN PRODUCTION MODE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MAGNETIC_SWIPE_IN_PRODUCTION_MODE = ICS_OFFSET + 90,
            /// <summary>
            /// MAGNETIC SWIPE IGNORED NOT IN NON-COMM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MAGNETIC_SWIPE_IGNORED_NOT_IN_NON_COMM = ICS_OFFSET + 91,
            /// <summary>
            /// MAGNETIC SWIPE IGNORED CONDITION NOT MET
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MAGNETIC_SWIPE_IGNORED_CONDITION_NOT_MET = ICS_OFFSET + 92,
            /// <summary>
            /// METER LOGIN VIA OPTICAL PORT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_LOGIN_VIA_OPTICAL_PORT = ICS_OFFSET + 93,
            /// <summary>
            /// METER LOGOFF VIA OPTICAL PORT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_LOGOFF_VIA_OPTICAL_PORT = ICS_OFFSET + 94,
            /// <summary>
            /// CLI LOGIN VIA OPTICAL PORT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CLI_LOGIN_VIA_OPTICAL_PORT = ICS_OFFSET + 95,
            /// <summary>
            /// CLI LOGIN ATTEMPTED VIA OPTICAL PORT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CLI_LOGIN_ATTEMPTED_VIA_OPTICAL_PORT = ICS_OFFSET + 96,
            /// <summary>
            /// CLI COMMAND EXECUTED VIA OPTICAL PORT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CLI_COMMAND_EXECUTED_VIA_OPTICAL_PORT = ICS_OFFSET + 97,
            /// <summary>
            /// CLI LOCKED OUT TOO MANY LOGIN ATTEMPTS VIA OPTICAL PORT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CLI_LOCKED_OUT_TOO_MANY_LOGIN_ATTEMPTS_VIA_OPTICAL_PORT = ICS_OFFSET + 98,
            /// <summary>
            /// ZIGBEE OPTICAL PASSTHROUGH STARTED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ZIGBEE_OPTICAL_PASSTHROUGH_STARTED = ICS_OFFSET + 99,
            /// <summary>
            /// MODEM OPTICAL PASSTHROUGH STARTED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MODEM_OPTICAL_PASSTHROUGH_STARTED = ICS_OFFSET + 100,
            /// <summary>
            /// STARTED C12.18 SESSION VIA OPTICAL PORT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            STARTED_C1218_SESSION_VIA_OPTICAL_PORT = ICS_OFFSET + 101,
            /// <summary>
            /// ICM CONFIGURATION CHANGE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_CONFIGURATION_CHANGE = ICS_OFFSET + 102,
            /// <summary>
            /// ICM STATE CHANGED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_STATE_CHANGED = ICS_OFFSET + 103,
            /// <summary>
            /// ICM TIME UPDATED FROM NETWORK
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_TIME_UPDATED_FROM_NETWORK = ICS_OFFSET + 104,
            /// <summary>
            /// ICM TIME SET FROM METER
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_TIME_SET_FROM_METER = ICS_OFFSET + 105,
            /// <summary>
            /// TIMESYNCH STATE CHANGED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TIMESYNCH_STATE_CHANGED = ICS_OFFSET + 106,
            /// <summary>
            /// CLI IS DISABLED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CLI_IS_DISABLED = ICS_OFFSET + 107,
            /// <summary>
            /// CLI IS REVERT ONLY
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CLI_IS_REVERT_ONLY = ICS_OFFSET + 108,
            /// <summary>
            /// LOADSIDE VOLTAGE WHILE SWITCH OPEN
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN = ICS_OFFSET + 109,
            /// <summary>
            /// LOADSIDE VOLTAGE WHILE SWITCH OPEN CLEAR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN_CLEAR = ICS_OFFSET + 110,
            /// <summary>
            /// CELL TOWER CHANGES EXCEED THRESHOLD
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CELL_TOWER_CHANGES_EXCEED_THRESHOLD = ICS_OFFSET + 111,
            /// <summary>
            /// SECTOR CHANGES EXCEED THRESHOLD
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SECTOR_CHANGES_EXCEED_THRESHOLD = ICS_OFFSET + 112,
            /// <summary>
            /// ACCUMULATOR READ FAILURE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ACCUMULATOR_READ_FAILURE = ICS_OFFSET + 113,
            /// <summary>
            /// CELLULAR CONNECTION TIMEOUT ALARM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CELLULAR_CONNECTION_TIMEOUT_ALARM = ICS_OFFSET + 114,
            /// <summary>
            /// SMS WAKEUP RECEIVED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SMS_WAKEUP_RECEIVED = ICS_OFFSET + 115,
            /// <summary>
            /// METER MODEL OW CENTRON
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METER_MODEL_OW_CENTRON = ICS_OFFSET + 116,
            /// <summary>
            /// ERT SUCCESSFULLY INITIALIZED SPI DRIVER
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_SUCCESSFULLY_INITIALIZED_SPI_DRIVER = ICS_OFFSET + 117,
            /// <summary>
            /// ERT CC1121 PART NUMBER
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_CC1121_PART_NUMBER = ICS_OFFSET + 118,
            /// <summary>
            /// ERT HW SUCCESSFULLY INITIALIZED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_ERT_HW_SUCCESSFULLY_INITIALIZED = ICS_OFFSET + 119,
            /// <summary>
            /// ERT RADIO TURNED OFF IN ERT CFG DATA TABLE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_ERT_RADIO_TURNED_OFF_IN_ERT_CFG_DATA_TABLE = ICS_OFFSET + 120,
            /// <summary>
            /// ERT CC1121 MANUAL CALIBRATION SUCCESSFUL
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_ERT_CC1121_MANUAL_CALIBRATION_SUCCESSFUL = ICS_OFFSET + 121,
            /// <summary>
            /// ERT MASTER LIST INITIALIZATION CREATION FAILED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_ERT_MASTER_LIST_INITIALIZATION_CREATION_FAILED = ICS_OFFSET + 122,
            /// <summary>
            /// ICM camping channel
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_ICM_CAMPING_CHANNEL = ICS_OFFSET + 123,
            /// <summary>
            /// ERT: ADDING AN ERT METER READING TO OUR MASTER LIST
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_ADDING_AN_ERT_METER_READING_TO_OUR_MASTER_LIST = ICS_OFFSET + 124,
            /// <summary>
            /// ERT: REACHED MAX NUMBER OF ERT METERS
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_REACHED_MAX_NUMBER_OF_ERT_METERS = ICS_OFFSET + 125,
            /// <summary>
            /// ERT: INCOMING ERT PACKET CRC ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_INCOMING_ERT_PACKET_CRC_ERROR = ICS_OFFSET + 126,
            /// <summary>
            /// ERT: CHANGING ERT RESTING CHANNEL
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_CHANGING_ERT_RESTING_CHANNEL = ICS_OFFSET + 127,
            /// <summary>
            /// ERT: ERT RADIO OFF FREEZE PROC IMC REJECTED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_ERT_RADIO_OFF_FREEZE_PROC_IMC_REJECTED = ICS_OFFSET + 128,
            /// <summary>
            /// ERT: NUMBER OF ERTS WHOSE RECORDS WERE FROZEN
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_NUMBER_OF_ERTS_WHOSE_RECORDS_WERE_FROZEN = ICS_OFFSET + 129,
            /// <summary>
            /// ERT: RECEIVED INVALID TIME FROM NTP TASK
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_RECEIVED_INVALID_TIME_FROM_NTP_TASK = ICS_OFFSET + 130,
            /// <summary>
            /// ERT: UNSUCCESSFUL PDOID READ
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_UNSUCCESSFUL_PDOID_READ = ICS_OFFSET + 131,
            /// <summary>
            /// ERT 242 TX FAILED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_242_TX_FAILED = ICS_OFFSET + 132,
            /// <summary>
            /// Added ERT to managed list
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ADDED_ERT_TO_MANAGED_LIST = ICS_OFFSET + 133,
            /// <summary>
            /// ERT time set failed
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_TIME_SET_FAILED = ICS_OFFSET + 134,
            /// <summary>
            /// ICM EVENT LOG CLEARED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ICM_EVENT_LOG_CLEARED = ICS_OFFSET + 135,
            /// <summary>
            /// POWER QUALITY DETECTION MOMENTARY INTERRUPTION
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_MOMENTARY_INTERRUPTION = ICS_OFFSET + 136,
            /// <summary>
            /// POWER QUALITY DETECTION SUSTAINED INTERRUPTION
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_QUALITY_DETECTION_SUSTAINED_INTERRUPTION = ICS_OFFSET + 137,
            /// <summary>
            /// TAMPER TILT SET ON OUTAGE (REMOVAL TAMPER)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TAMPER_TILT_SET_ON_OUTAGE = ICS_OFFSET + 138,
            /// <summary>
            /// CONFIGURATION COMMIT
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CONFIGURATION_COMMIT = ICS_OFFSET + 139,
            /// <summary>
            /// Firmware Download: Initialization Failure
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FIRMWARE_DOWNLOAD_INITIALIZATION_FAILURE = ICS_OFFSET +140,
            /// <summary>
            /// HAN Firmware Download Failure
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            HAN_FIRMWARE_DOWNLOAD_FAILURE = ICS_OFFSET + 141,
            /// <summary>
            /// ERT 242 Command Request
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_242_COMMAND_REQUEST = ICS_OFFSET + 142,
            /// <summary>
            /// SMS Wakeup Identity Request Sent
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SMS_WAKEUP_IDENTITY_REQUEST_SENT = ICS_OFFSET + 143,
            /// <summary>
            /// SMS Wakeup Identity Request Not Sent Because Not Registered
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SMS_WAKEUP_IDENTITY_NOT_SENT_BECAUSE_NOT_REGISTERED = ICS_OFFSET + 144,
            /// <summary>
            /// SMS Wakeup Identity Request Not Sent Because Not Synchronized
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SMS_WAKEUP_IDENTITY_REQUEST_NOT_SENT_BECAUSE_NOT_SYNCHRONIZED = ICS_OFFSET + 145,
            /// <summary>
            /// Failed Security Key Verification
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FAILED_SECURITY_KEY_VERIFICATION = ICS_OFFSET + 146,
            /// <summary>
            /// Failed CE configuration verification
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FAILED_CE_CONFIGURATION_VERIFICATION = ICS_OFFSET + 147,
            /// <summary>
            /// Migration State Change
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MIGRATION_STATE_CHANGE = ICS_OFFSET + 148,
            /// <summary>
            /// Critical Peak Pricing Status 
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CRITICAL_PEAK_PRICING_STATUS = ICS_OFFSET + 149,
            /// <summary>
            /// Security Event
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SECURITY_EVENT = ICS_OFFSET + 150,
            /// <summary>
            /// ERT Meter Stolen
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_METER_STOLEN = ICS_OFFSET + 151,
            /// <summary>
            /// ERT Meter Removed
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_METER_REMOVED = ICS_OFFSET + 152,
            /// <summary>
            /// ERT Connection Downtime  Time Exceeded
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_CONNECTION_DOWNTIME_TIME_EXCEEDED = ICS_OFFSET + 153,
            /// <summary>
            /// ERT Predictor List Time Modified
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_PREDICTOR_LIST_TIME_MODIFIED = ICS_OFFSET + 154,
            /// <summary>
            /// ERT CC1121 Manual Calibration Failed
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_MANUAL_CALIBRATION_FAILED = ICS_OFFSET + 155,
            /// <summary>
            /// ICM tracking 100G failed
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ERT_100G_TRACKING_FAILED = ICS_OFFSET + 156,
            /// <summary>
            /// Time Source Unavailable
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TIME_SOURCE_UNAVAILABLE = ICS_OFFSET + 165,
            /// <summary>
            /// Signing key update success
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SIGNING_KEY_UPDATE_SUCCESS = ICS_OFFSET + 166,
            /// <summary>
            /// Symmetric key update success
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SYMMETRIC_KEY_UPDATE_SUCCESS = ICS_OFFSET + 167,
            /// <summary>
            /// Key rollover success
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            KEY_ROLLOVER_SUCCESS = ICS_OFFSET + 168,
            /// <summary>
            /// UNPROGRAMMED
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            UNPROGRAMMED = ICS_OFFSET + 448,
            /// <summary>
            /// RAM FAILURE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RAM_FAILURE = ICS_OFFSET + 451,
            /// <summary>
            /// ROM FAILURE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            ROM_FAILURE = ICS_OFFSET + 452,
            /// <summary>
            /// NONVOLATILE MEMORY FAILURE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            NONVOLATILE_MEMORY_FAILURE = ICS_OFFSET + 453,
            /// <summary>
            /// CLOCK ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            CLOCK_ERROR = ICS_OFFSET + 454,
            /// <summary>
            /// MEASUREMENT ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            MEASUREMENT_ERROR = ICS_OFFSET + 455,
            /// <summary>
            /// LOW BATTERY
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            LOW_BATTERY = ICS_OFFSET + 456,
            /// <summary>
            /// LOW LOSS POTENTIAL
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            LOW_LOSS_POTENTIAL = ICS_OFFSET + 457,
            /// <summary>
            /// DEMAND OVERLOAD
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            DEMAND_OVERLOAD = ICS_OFFSET + 458,
            /// <summary>
            /// POWER FAILURE
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            POWER_FAILURE = ICS_OFFSET + 459,
            /// <summary>
            /// BAD PASSWORD
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            BAD_PASSWORD = ICS_OFFSET + 460,
            /// <summary>
            /// METERING ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            METERING_ERROR = ICS_OFFSET + 472,
            /// <summary>
            /// DC DETECTED (I-210) OR TIME CHANGED (kV2c)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            DC_DETECTED_OR_TIME_CHANGED = ICS_OFFSET + 473,
            /// <summary>
            /// SYSTEM ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            SYSTEM_ERROR = ICS_OFFSET + 474,
            /// <summary>
            /// RECEIVED KWH
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            RECEIVED_KWH = ICS_OFFSET + 475,
            /// <summary>
            /// LEADING KVARH
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            LEADING_KVARH = ICS_OFFSET + 476,
            /// <summary>
            /// LOSS OF PROGRAM
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            LOSS_OF_PROGRAM = ICS_OFFSET + 477,
            /// <summary>
            /// HIGH TEMP (I-210) OR FLASH CODE ERROR (kV2c)
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            HIGH_TEMP_OR_FLASH_CODE_ERROR = ICS_OFFSET + 478,
            /// <summary>
            /// Time Changed Status
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            TIME_CHANGED_STATUS = ICS_OFFSET + 479,
            /// <summary>
            /// FLASH CODE ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FLASH_CODE_ERROR = ICS_OFFSET + 480,
            /// <summary>
            /// FLASH DATA ERROR
            /// </summary>
            [EnumEventInfoAttribute("Table2524")]
            FLASH_DATA_ERROR = ICS_OFFSET + 481,
        }

        /// <summary>
        /// Reached the MAX Number of 100G Meters Event argument data
        /// </summary>
        private enum ERTMaxCountInfo: byte
        {
            /// <summary>
            /// Unmanaged List
            /// </summary>
            [EnumDescription("Unmanaged List")]
            UnmanagedList = 0,
            /// <summary>
            /// Managed List
            /// </summary>
            [EnumDescription("Managed List")]
            ManagedList = 1,
            /// <summary>
            /// Consumption Data
            /// </summary>
            [EnumDescription("Consumption Data")]
            ConsumptionData = 2,
        }

        /// <summary>
        /// ERT Predictor List Time Modified Event argument data
        /// </summary>
        private enum ERTPredictorListTimeModifiedInfo : byte
        {
            /// <summary>
            /// Force Time Sync
            /// </summary>
            [EnumDescription("Force Time Sync")]
            ForceTimeSync = 0,
            /// <summary>
            /// NTP Time Drift
            /// </summary>
            [EnumDescription("NTP Time Drift")]
            NTPTimeDrift = 1,
        }

        /// <summary>
        /// ERT Device Class Event argument data
        /// </summary>
        private enum ERTDeviceClass : byte
        {
            /// <summary>
            /// 100W
            /// </summary>
            [EnumDescription("100W")]
            ERT100W = 0x94,
            /// <summary>
            /// 100G
            /// </summary>
            [EnumDescription("100G")]
            ERT100G = 0x9C,
            /// <summary>
            /// 100W+
            /// </summary>
            [EnumDescription("100W+")]
            ERT100WPlus = 0xAB,
            
        }

        /// <summary>
        /// Migration State Event argument data
        /// </summary>
        private enum MigrationState : byte
        {
            /// <summary>
            /// Initial state, using TMS
            /// </summary>
            [EnumDescription("Initial State - Using TMS")]
            TMS = 0x00,
            /// <summary>
            /// Start migration
            /// </summary>
            [EnumDescription("Start Migration")]
            Start = 0x01,
            /// <summary>
            /// Verify security
            /// </summary>
            [EnumDescription("Verify Security")]
            VerifySecurity = 0x02,
            /// <summary>
            /// Verify configuration
            /// </summary>
            [EnumDescription("Verify Configuration")]
            VerifyConfig = 0x03,
            /// <summary>
            /// Register and authenticate with CE
            /// </summary>
            [EnumDescription("Register and Authenticate with CE")]
            CERegister = 0x04,
            /// <summary>
            /// Verify registeration with CE was successful
            /// </summary>
            [EnumDescription("Verify Registration with CE")]
            VerifyRegistration = 0x05,
            /// <summary>
            /// Disable TMS
            /// </summary>
            [EnumDescription("Disable TMS")]
            DisableTMS = 0x06,
            /// <summary>
            /// Using CE
            /// </summary>
            [EnumDescription("Using CE")]
            UsingCE = 0x10,
            /// <summary>
            /// Error state
            /// </summary>
            [EnumDescription("Error State")]
            Error = 0xFE,
            /// <summary>
            /// Unknown state
            /// </summary>
            [EnumDescription("Unknown State")]
            Unknown = 0xFF,
        }

        /// <summary>
        /// CPP Status Event argument data
        /// </summary>
        private enum CPPStatus : byte
        {
            /// <summary>
            /// Configured
            /// </summary>
            [EnumDescription("Configured")]
            Configured = 0x01,
            /// <summary>
            /// Waiting
            /// </summary>
            [EnumDescription("Waiting")]
            Waiting = 0x02,
            /// <summary>
            /// Waiting for End of Interval
            /// </summary>
            [EnumDescription("Waiting for End of Interval")]
            WaitingEOI = 0x03,
            /// <summary>
            /// Active
            /// </summary>
            [EnumDescription("Active")]
            Active = 0x04,
            /// <summary>
            /// Done awaiting EOI
            /// </summary>
            [EnumDescription("Done Waiting for End of Interval")]
            DoneEOI = 0x05,
            /// <summary>
            /// Done
            /// </summary>
            [EnumDescription("Done")]
            Done = 0x06,
            /// <summary>
            /// Not initialized
            /// </summary>
            [EnumDescription("Not Initialized")]
            NotInit = 0x07,
        }

        /// <summary>
        /// Firmware Type Event argument data
        /// </summary>
        private enum FWType : byte
        {
            /// <summary>
            /// Comm Module Firmware
            /// </summary>
            [EnumDescription("Comm Module")]
            CommModule = 32,
            /// <summary>
            /// Cellular Modem firmware
            /// </summary>
            [EnumDescription("Cellular Modem")]
            CellularModem = 33,
            /// <summary>
            /// GE I210/KV2c Meter Firmware
            /// </summary>
            [EnumDescription("GE I-210/kV2c Meter")]
            GEMeter = 34,
            /// <summary>
            /// GE I210/KV2c Meter Program
            /// </summary>
            [EnumDescription("GE I-210/kV2c Meter Program")]
            GEMeterProgram = 35,
        }

        /// <summary>
        /// Firmware Failure Reason Event argument data
        /// </summary>
        private enum FirmwareFailureReason : byte
        {
            /// <summary>
            /// Bad File Size
            /// </summary>
            [EnumDescription("Bad File Size")]
            BadFileSize = 1,
            /// <summary>
            /// Bad Firmware Type
            /// </summary>
            [EnumDescription("Bad FW Type")]
            BadFWType = 4,
            /// <summary>
            /// Bad CRC
            /// </summary>
            [EnumDescription("Bad CRC")]
            BadCRC = 5,
            /// <summary>
            /// Retry Failure
            /// </summary>
            [EnumDescription("Retry Failure")]
            RetryFailure = 6,
            /// <summary>
            /// Insufficient Blocks
            /// </summary>
            [EnumDescription("Insufficient Blocks")]
            InsufficientBlocks = 7,
            /// <summary>
            /// Bad Hash
            /// </summary>
            [EnumDescription("Bad Hash")]
            BadHash = 24,
            /// <summary>
            /// Bad Interface
            /// </summary>
            [EnumDescription("Bad Interface")]
            BadInterface = 35,
            /// <summary>
            /// Same Version
            /// </summary>
            [EnumDescription("Same Version")]
            SameVersion = 36,
            /// <summary>
            /// Activation In Progress
            /// </summary>
            [EnumDescription("Activation In Progress")]
            ActivationInProgress = 37,
            /// <summary>
            /// File Prefill Failure
            /// </summary>
            [EnumDescription("File Prefill Failure")]
            FilePrefillFailure = 38,
            /// <summary>
            /// HAN Activation Failure
            /// </summary>
            [EnumDescription("HAN Activation Failure")]
            HANActivationFailure = 39,
            /// <summary>
            /// HAN Activation in Progress
            /// </summary>
            [EnumDescription("HAN Activation in Progress")]
            HANActivationInProgress = 41,
        }

        #endregion

        #region Public Methods

        /// <summary>Constructs a dictionary of M3 Gateway specific events</summary>
        //  Revision History	
        //  MM/DD/YY Who Version    Issue#      Description
        //  -------- --- -------    ------      -------------------------------------------
        //  03/18/13 MSC 2.80.08    TR7640      Created
        //  05/06/13 MSC 2.80.28    TR7640      Updated with Table Definition
        //  05/29/13 MSC 2.80.33    TR7640      Updated with a total of 113 events
        //  07/03/13 AF  2.80.45    TR7640      Updated with more events
        //  07/11/13 AF  2.80.51    WR417110    Updated with more events
        //  07/12/13 AF  2.80.51    WR417110    Updated with more events
        //  07/26/13 AF  2.85.03    WR418370    Updated with missing ICS events
        //  04/07/14 AF  3.50.62    WR466261    Updated with more events
        //  04/24/14 AF  3.50.83    WR488012    Renamed one event to match the CE UI
        //  05/12/14 AF  3.50.92    WR516242    Added the three missing events, 3716, 3717, and 3718 
        //  05/16/14 jrf 3.50.95    WR516785    Making changes to add two new events.
        //  10/27/14 AF  4.00.80    WR503425    Added key management security success events\
        //  04/05/16 CFB 4.50.244   WR603380    Modified names of several 100G specific meter events
        //                                         to generic names to match CE
        //  07/11/16 MP  4.70.??    WR688986    Changed to a foreach.
        //  07/14/16 MP  4.70.7     WR688986    Removed commented code
        //  10/05/16 AF  4.70.21    WR716010    Added check to make sure event is not already in the list
        //                                      and check the description for null in case it's not in the enum
        //
        public ICS_Gateway_EventDictionary()
            : base()
        {
            foreach (CommModuleHistoryEvents Event in Enum.GetValues(typeof(CommModuleHistoryEvents)))
            {
                if (!ContainsKey((int)Event))
                {
                    string strEventDescription = m_rmStrings.GetString(Enum.GetName(typeof(CommModuleHistoryEvents), Event));

                    if (null == strEventDescription)
                    {
                        strEventDescription = m_rmStrings.GetString("UNKNOWN_EVENT");
                    }

                    Add((int)Event, strEventDescription);
                }
            }
        }

        /// <summary>
        /// TranslateEventData - Takes the Event Data and translates it to something human readable
        /// </summary>
        /// <param name="HistoryEvent">The Event Code of the Event the Data belongs to</param>
        /// <param name="ArgumentReader">The raw data we are translating</param>
        /// <returns>The Human Readable text</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 05/13/14 jrf 3.50.92 WR 504256 Added implementation of this method.
        // 05/16/14 jrf 3.50.95 WR 504256 Corrected displaying device ID argument as uint32 and
        //                                not showing argument for event ERT Connection Downtime Time Exceeded.
        // 05/16/14 jrf 3.50.95 WR 516785 Making changes to support one new and one changed event argument.
        // 10/27/14 AF  4.00.80 WR 503425 Added cases for key management security success events
        // 10/31/14 AF  4.00.82 WR 503425 Removed the cases for key management security success events - we
        //                                don't want to expose the arguments in case of hackers
        //  04/05/16 CFB 4.50.244 WR603380 Modified names of several 100G specific meter events
        //                                to generic names to match CE
        //
        public override string TranslatedEventData(HistoryEntry HistoryEvent, PSEMBinaryReader ArgumentReader)
        {
            String strData = "";

            if (null != ArgumentReader)
            {
                switch (HistoryEvent.HistoryCode)
                {                    
                    case (ushort)CommModuleHistoryEvents.ERT_ADDING_AN_ERT_METER_READING_TO_OUR_MASTER_LIST:
                    case (ushort)CommModuleHistoryEvents.ERT_242_TX_FAILED:
                    case (ushort)CommModuleHistoryEvents.ADDED_ERT_TO_MANAGED_LIST:
                    case (ushort)CommModuleHistoryEvents.ERT_TIME_SET_FAILED:
                    case (ushort)CommModuleHistoryEvents.ERT_METER_STOLEN:
                    case (ushort)CommModuleHistoryEvents.ERT_METER_REMOVED:
                    case (ushort)CommModuleHistoryEvents.ERT_100G_TRACKING_FAILED:
                    {
                        strData = "Device ID: " + ArgumentReader.ReadUInt32().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.ERT_ICM_CAMPING_CHANNEL:
                    {
                        strData = "Camping Channel: " + ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.ERT_REACHED_MAX_NUMBER_OF_ERT_METERS:
                    {
                        byte Argument = ArgumentReader.ReadByte();
                        if (Enum.IsDefined(typeof(ERTMaxCountInfo), Argument))
                        {
                            strData = ((ERTMaxCountInfo)Argument).ToDescription();
                        }
                        else
                        {
                            strData = Argument.ToString(CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.ERT_CHANGING_ERT_RESTING_CHANNEL:
                    {
                        strData = "Channel Number: " + ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.ERT_NUMBER_OF_ERTS_WHOSE_RECORDS_WERE_FROZEN:
                    {
                        strData = ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.ERT_UNSUCCESSFUL_PDOID_READ:
                    {
                        strData = "Table ID: " + ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.ERT_PREDICTOR_LIST_TIME_MODIFIED:
                    {
                        byte Argument = ArgumentReader.ReadByte();
                        if (Enum.IsDefined(typeof(ERTPredictorListTimeModifiedInfo), Argument))
                        {
                            strData = ((ERTPredictorListTimeModifiedInfo)Argument).ToDescription();
                        }
                        else
                        {
                            strData = Argument.ToString(CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.POWER_OUTAGE_DETECTED:
                    case (ushort)CommModuleHistoryEvents.POWER_RESTORATION_DETECTED:
                    {
                        strData = "Outage Supported Version: " + ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture)
                            + ", Outage ID Counter: " + ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.ERT_CC1121_PART_NUMBER:
                    {
                        strData = "Part Number: " + ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.ERT_242_COMMAND_REQUEST:
                    {
                        strData = "Device ID: " + ArgumentReader.ReadUInt32().ToString(CultureInfo.InvariantCulture);

                        strData += ", Device Class: ";

                        byte Argument = ArgumentReader.ReadByte();
                        if (Enum.IsDefined(typeof(ERTDeviceClass), Argument))
                        {
                            strData += ((ERTDeviceClass)Argument).ToDescription();
                        }
                        else
                        {
                            strData = Argument.ToString("X2", CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.MIGRATION_STATE_CHANGE:
                    {
                        byte Argument = ArgumentReader.ReadByte();
                        if (Enum.IsDefined(typeof(MigrationState), Argument))
                        {
                            strData = ((MigrationState)Argument).ToDescription();
                        }
                        else
                        {
                            strData = Argument.ToString("X2", CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.CRITICAL_PEAK_PRICING_STATUS:
                    {
                        byte Argument = ArgumentReader.ReadByte();
                        if (Enum.IsDefined(typeof(CPPStatus), Argument))
                        {
                            strData = ((CPPStatus)Argument).ToDescription();
                        }
                        else
                        {
                            strData = Argument.ToString("X2", CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.SECURITY_EVENT:
                    {
                        strData = "Return Code: " + ArgumentReader.ReadByte().ToString("X2", CultureInfo.InvariantCulture);
                        strData = ", EISM Call: " + ArgumentReader.ReadByte().ToString("X2", CultureInfo.InvariantCulture);
                        strData = ", Calling Routine: " + ArgumentReader.ReadByte().ToString("X2", CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.FIRMWARE_UPGRADE_DOWNLOAD_ALARM:
                    case (ushort)CommModuleHistoryEvents.FIRMWARE_UPGRADE_ACTIVE_ALARM:
                    case (ushort)CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_CANCELED:
                    case (ushort)CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_SUCCESSFUL:
                    case (ushort)CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_REMOVING_INACTIVE_FILES:
                    {
                        byte Argument = ArgumentReader.ReadByte();

                        strData = "FW Type: ";

                        if (Enum.IsDefined(typeof(FWType), Argument))
                        {
                            strData += ((FWType)Argument).ToDescription();
                        }
                        else
                        {
                            strData += Argument.ToString(CultureInfo.InvariantCulture);
                        }                        

                        switch(Argument)
                        {
                            case (byte)FWType.CommModule:
                            case (byte)FWType.CellularModem:                            
                            {
                                strData += ", FW Version: ";
                                strData += ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture) + "."; //Version
                                strData += ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture) + "."; //Revision
                                strData += ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture); //Build
                                break;
                            }
                            case (byte)FWType.GEMeterProgram:
                            {
                                ushort ProgramID = ArgumentReader.ReadByte();

                                strData += ", Program ID: ";

                                //Program ID is MSB first
                                ProgramID = (ushort)(ProgramID << 2);
                                ProgramID = (ushort)(ProgramID & (ushort)ArgumentReader.ReadByte());
                                break;
                            }
                            case (byte)FWType.GEMeter:
                            default:
                            {
                                strData += ", FW Version: ";
                                strData += ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture) + "."; //Version
                                strData += ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture) + "."; //Revision
                                strData += ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture) + "."; //Patch Version
                                strData += ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture); //Patch Revision
                                break;
                            }
                        }


                        break;
                    }
                    case (ushort)CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_RETRIES_EXCEEDED:
                    case (ushort)CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_INITIALIZATION_FAILURE:
                    {
                        byte Argument = ArgumentReader.ReadByte();

                        strData = "FW Type: ";

                        if (Enum.IsDefined(typeof(FWType), Argument))
                        {
                            strData += ((FWType)Argument).ToDescription();
                        }
                        else
                        {
                            strData += Argument.ToString(CultureInfo.InvariantCulture);
                        }

                        strData = ", FW Failure Reason: ";

                        if (Enum.IsDefined(typeof(FirmwareFailureReason), Argument))
                        {
                            strData += ((FirmwareFailureReason)Argument).ToDescription();
                        }
                        else
                        {
                            strData += Argument.ToString(CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                    default:
                    {
                        strData = base.TranslatedEventData(HistoryEvent, ArgumentReader);
                        break;
                    }
                }
            }

             return strData;
        }

        #endregion
    }

    /// <summary>
    /// Class that represents the ICS history log configuration data stored in table 2523
    /// </summary>
    public class ICS_Gateway_HistoryLogConfig : CommModuleLogConfig
    {
        #region Public Methods

        /// <summary>
        /// Constructor for ICS History Log Config class
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="Offset"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //
        public ICS_Gateway_HistoryLogConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
        }

        /// <summary>
        /// Constructor used to get Event Data from the EDL file
        /// </summary>
        /// <param name="EDLBinaryReader"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //
        public ICS_Gateway_HistoryLogConfig(PSEMBinaryReader EDLBinaryReader)
            : base(EDLBinaryReader)
        {
        }

        #endregion

        #region Public Properties
        #endregion
    }

}
