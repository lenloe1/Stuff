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
//                           Copyright © 2006 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DST;
using Itron.Metering.Progressable;
using Itron.Metering.ReplicaSettings;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;
using Itron.Metering.Zigbee;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading;
using System.Xml;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Enumeration used to determine the type of decommission to perform
    /// </summary>
    public enum DecommissionType : byte
    {
        /// <summary>
        /// Bring down the HAN (Zigbee) network and reform after a delay
        /// </summary>
        DECOMMISSION_AND_HALT = 7,
        /// <summary>
        /// Bring down the HAN (Zigbee) network and reform immediately
        /// </summary>
        DECOMMISSION_AND_COMMISSION = 8,
        /// <summary>
        /// Remove a single node from the network
        /// </summary>
        DROP_NODE = 0x0E,
    }

	/// <summary>
	/// CANSIDevice class - This is the "device server" for the ANSI device.
	/// </summary>
    // Revision History	
    // MM/DD/YY Who Version ID Number Description
    // -------- --- ------- -- ------ ----------------------------------------------------------
    // 07/13/05 mrj 7.13.00 N/A       Created
    // 11/14/13 AF  3.50.03           Class re-architecture - promoted INetwork interface from CENTRON_AMI
    // 01/16/14 DLG 3.50.26           Moved the DisplayMode Enum from ItronDevice to CANSIDevice.
    // 
    public abstract partial class CANSIDevice : ItronDevice, ICreateEDL
	{
		#region Constants

		/// <summary>
		/// Can be used to mark all Canadian Events (table 76) as having been read.
		/// </summary>
		public const ushort ALL_CANADIAN_EVENTS = 0xFFFF;

		private const string MFG_SCH = "SCH";
		private const string MFG_ITRN = "ITRN";
        private const string MFG_ICS = " ICS";
        private const string LIS1 = "LIS1";

		//This is the maximum wait time in seconds that the meter may not
		//respond after a closing of the config file (2048)
		private const int CONFIG_MAX_WAIT_TIME_SEC = 60;

		/// <summary>
		/// Maximum amount of time to adjust the clock
		/// </summary>
		protected const int MAX_TIME_ADJUST_SECONDS = 86400;
		private const int MAX_NUM_ERRORS = 14;

		//Masks for non-fatal errors
		private const byte NON_FATAL_1_MASK = 0x01;
		private const byte NON_FATAL_2_MASK = 0x02;
		private const byte NON_FATAL_3_MASK = 0x04;
		private const byte NON_FATAL_4_MASK = 0x08;
		private const byte NON_FATAL_5_MASK = 0x10;
		private const byte NON_FATAL_6_MASK = 0x20;
		private const byte NON_FATAL_9_MASK = 0x04;

		//Masks for fatal errors
		private const byte FATAL_1_MASK = 0x01;
		private const byte FATAL_2_MASK = 0x02;
		private const byte FATAL_3_MASK = 0x04;
		private const byte FATAL_4_MASK = 0x08;
		private const byte FATAL_5_MASK = 0x10;
		private const byte FATAL_6_MASK = 0x20;
		private const byte FATAL_7_MASK = 0x40;
		private const byte FATAL_8_MASK = 0x80;

		//Mask for setting the time and date in the meter (Procedure 10)
		private const byte SET_TIME_DATE_MASK = (byte)(SET_MASK_BFLD.SET_DATE_FLAG | SET_MASK_BFLD.SET_TIME_FLAG);

		/// <summary>
		/// Size of the Set Time Date procedure data
		/// </summary>
		protected const int SET_TIME_DATE_PROC_SIZE = 7;

		//Error masks used to determine the close config procedure results
		private const uint CLOSE_CONFIG_COEFF_ERROR_MASK = 0x00000001;
		private const uint CLOSE_CONFIG_ENERGY_ERROR_MASK = 0x00000004;
		private const uint CLOSE_CONFIG_DEMAND_ERROR_MASK = 0x00000008;
		private const uint CLOSE_CONFIG_DISPLAY_ERROR_MASK = 0x00000010;
		private const uint CLOSE_CONFIG_HISTLOG_ERROR_MASK = 0x00000020;
		private const uint CLOSE_CONFIG_IO_ERROR_MASK = 0x00000100;
		private const uint CLOSE_CONFIG_LP_ERROR_MASK = 0x00000200;
		private const uint CLOSE_CONFIG_SR_ERROR_MASK = 0x00000800;
		private const uint CLOSE_CONFIG_SS_ERROR_MASK = 0x00001000;
		private const uint CLOSE_CONFIG_CAL_ERROR_MASK = 0x00002000;
		private const uint CLOSE_CONFIG_TOU_ERROR_MASK = 0x00004000;
		private const uint CLOSE_CONFIG_MODE_ERROR_MASK = 0x00020000;
		private const uint CLOSE_CONFIG_EVTLOG_ERROR_MASK = 0x00080000;
		private const uint CLOSE_CONFIG_OPTBRD_ERROR_MASK = 0x00100000;
		private const uint CLOSE_CONFIG_TOT_ERROR_MASK = 0x00400000;
		private const uint CLOSE_CONFIG_OLDSW_ERROR_MASK = 0x40000000;
		private const uint CLOSE_CONFIG_CANLOG_ERROR_MASK = 0x80000000;

		// Max time to retry a procedure whose result is NOT_FULLY_COMPLETED
		private const int MAX_PROCEDURE_RETRIES = 20;
		private const int PROCEDURE_WAIT_MS = 100;

		private const int SATURN_ADVANCED_RATES = 7;
		private const byte MAX_NUMBER_OF_PACKETS = 0xFE; // 254
		private const uint DEFAULT_BAUD_RATE = 9600;

		private const int SUPPORTED_PASSWORDS = 4;
		private const ushort STANDARD_SECURITY_TABLE = 42;
		private const int TOTAL_CANADIAN_EVENTS = 79;
		private const ushort CANADIAN_EVENT_TABLE = 76;
		private const ushort UNREAD_CANADIAN_ENTRIES_OFFSET = 9;		

		/// <summary>SIZE_OF_ANSI_PASSWORD name says it all</summary>
		protected const int SIZE_OF_ANSI_PASSWORD = 20;

		private const int MFG_OFFSET = 2048;

		private const byte HW_UPPER_NIBBLE_MASK = 0x0F;

        /// <summary>
        /// Constant describing the Prism light Firmware Revision
        /// </summary>
        public const float PRISM_LITE_REVISION = 128.0f;

        /// <summary>
        /// Constant describing the Firmware Version of Version 1 Release 1 meters
        /// </summary>
        public const float VERSION_1_RELEASE_1 = 1.0F;
        /// <summary>
        /// Constant describing the Firmware Version of Version 1 Release 1.5 meters (SCE Updates)
        /// </summary>
        public const float VERSION_1_RELEASE_1_5 = 1.001F;
        /// <summary>
        /// Constant describing the Firmware Version of Version 1 Release 2 meters (PR 1.3) (SR 1.0 - HW 1.5)
        /// </summary>
        public const float VERSION_1_RELEASE_3 = 1.003F;
        /// <summary>
        /// 1.005 release of FW
        /// </summary>
        public const float VERSION_1_5_RELEASE_1 = 1.005F;
        /// <summary>
        /// Firmware Version for the Hardware 2.00 Project (1.008)
        /// </summary>
        public const float VERSION_1_8_HARDWARE_2_0 = 1.008F;
        /// <summary>
        /// Constant describing the Firmware Version of Version 2 Release 1 meters (New Design)
        /// </summary>
        public const float VERSION_2_RELEASE_1 = 2.0F;
        /// <summary>
        /// Constant Describing the Firmware Version 2.0 SP3
        /// </summary>
        public const float VERSION_2_SP3 = 2.001F;
        /// <summary>
        /// Constant Describing the Firmware Version for SR 2.0 SP5
        /// </summary>
        public const float VERSION_2_SP5 = 2.005F;
        /// <summary>
        /// Constant describing the firmware version for SR 2.0 SP5.1
        /// </summary>
        public const float VERSION_2_SP5_1 = 2.006F;
        /// <summary>
        /// Constant describing the firmware version for SR 3.7 Hydrogen C
        /// </summary>
        public const float VERSION_3_7_HYDROGEN_C = 3.010F;
        /// <summary>
        /// Constant describing the firmware version for the earliest Lithium release
        /// </summary>
        public const float VERSION_3_12_LITHIUM = 3.012F;
        /// <summary>
        /// Constant describing the firmware version for Carbon
        /// </summary>
        public const float VERSION_CARBON = 5.006F;
        /// <summary>
        /// Constant describing the firmware version for HW 3.x Carbon
        /// </summary>
        public const float VERSION_5_5_CARBON = 5.005F;
        /// <summary>
        /// Constant describing the firmware version for HW 3.x Carbon
        /// </summary>
        public const float VERSION_5_8_BRIDGE_PHASE_2 = 5.008F;
        /// <summary>
        /// Constant describing the firmware version for the combined Carbon/Bridge RAM Robustness firmware.
        /// </summary>
        public const float VERSION_5_9_CARBON_BRIDGE_ROBUST_RAM = 5.009F;
        /// <summary>
        /// Constant describing the firmware version for HW 3.x Michigan
        /// </summary>
        public const float VERSION_6_0_MICHIGAN = 6.000F;
        /// <summary>
        /// Constant Describing the Hardware Version for 1.5 meters
        /// </summary>
        public const float HW_VERSION_1_5 = 1.015F;
        /// <summary>
        /// Constant Describing the Hardware Version for 2.0 meters 
        /// </summary>
        public const float HW_VERSION_2_0 = 2.000F;
        /// <summary>
        /// Constant Describing Hardware 2.0 oscillator meters
        /// </summary>
        public const float HW_VERSION_2_1 = 2.010F;
        /// <summary>
        /// Constant Describing the Hardware Version for 2.050 meters
        /// </summary>
        public const float HW_VERSION_2_5 = 2.050F;
        /// <summary>
        /// Constant Describing Hardware 2.5 oscillator meters
        /// </summary>
        public const float HW_VERSION_2_6 = 2.060F;
        /// <summary>
        /// Constant Describing the Hardware Version for 1.0 meters
        /// </summary>
        public const float HW_VERSION_1_0 = 1.01F;
        /// <summary>
        /// Constant Describing the Hardware Version for 3.050 meters
        /// </summary>
        public const float HW_VERSION_3_5 = 3.050F;
        /// <summary>
        /// Constant Describing the Hardware Version for 3.060 meters
        /// </summary>
        public const float HW_VERSION_3_6 = 3.060F;
        /// <summary>
        /// Constant Describing the Hardware Version for 3.061 meters
        /// </summary>
        public const float HW_VERSION_3_61 = 3.061F;
        /// <summary>
        /// Constant Describing the Hardware Version for 3.080 meters
        /// </summary>
        public const float HW_VERSION_3_8 = 3.080F;
        /// <summary>
        /// Constant Describing the Hardware Version for 3.081 meters
        /// </summary>
        public const float HW_VERSION_3_81 = 3.081F;
        /// <summary>
        /// Constant describing the firmware version for SR 3.0
        /// </summary>
        public const float VERSION_3 = 3.000F;
        /// <summary>
        /// Constant describing the firmware version for Lithium, 3.012
        /// </summary>
        public const float VERSION_LITHIUM_3_12 = 3.012F;
        /// <summary>
        /// Constant describing the firmware version for M2 Landis Gyr RF Mesh Single Phase
        /// </summary>
        public const float VERSION_M2RFMESH_2_1 = 2.001F;
        
        /// <summary>
        /// Constant describing the max hardware version for ICM Superior modules
        /// </summary>
        public const float ICM_HW_VERSION_2_255 = 2.255F;

        /// <summary>
        /// Constant describing the hardware version for 4G CATM1 Modules
        /// </summary>
        public const float FOURG_CATM1_HW_VERSION = 106.001F;

        /// <summary>
        /// Device class string for HW 1.0 OpenWay Centron meters.
        /// </summary>
        public const string ITRN_DEVICE_CLASS = "ITRN";
        /// <summary>
        /// Device class string for HW 1.5 OpenWay Centron meters.
        /// </summary>
        public const string ITR1_DEVICE_CLASS = "ITR1";
        /// <summary>
        /// Device class string for Basic Polyphase meters.
        /// </summary>
        public const string ITR3_DEVICE_CLASS = "ITR3";
        /// <summary>
        /// Device class string for Advanced Polyphase meters.
        /// </summary>
        public const string ITR4_DEVICE_CLASS = "ITR4";
        /// <summary>
        /// Device class string for transparent devices.
        /// </summary>
        public const string ITRT_DEVICE_CLASS = "ITRT";
        /// <summary>
        /// Device class string for PrismLite devices
        /// </summary>
        public const string ITRL_DEVICE_CLASS = "ITRL";
        /// <summary>
        /// Device class string for ITRD single phase meters.
        /// </summary>
        public const string ITRD_DEVICE_CLASS = "ITRD";
        /// <summary>
        /// Device class string for ITRE basic polyphase meters.
        /// </summary>
        public const string ITRE_DEVICE_CLASS = "ITRE";
        /// <summary>
        /// Device class string for ITRF advanced polyphase meters.
        /// </summary>
        public const string ITRF_DEVICE_CLASS = "ITRF";
        /// <summary>
        /// Device class string for SL7000 Gateway.
        /// </summary>
        public const string ITRS_DEVICE_CLASS = "ITRS";
        /// <summary>
        /// Device class string for single phase ICS Cellular ITRH gateway.
        /// </summary>
        public const string ITRH_DEVICE_CLASS = "ITRH";
        /// <summary>
        /// Device class string for single phase ICS Cellular meter.
        /// </summary>
        public const string ITRJ_DEVICE_CLASS = "ITRJ";
        /// <summary>
        /// Device class string for polyphase ICS Cellular meter.
        /// </summary>
        public const string ITRK_DEVICE_CLASS = "ITRK";
        /// <summary>
        /// Device class string for single phase ICS Cellular ITRU gateway
        /// </summary>
        public const string ITRU_DEVICE_CLASS = "ITRU";
        /// <summary>
        /// Device class string for polyphase ICS Cellular ITRV gateway
        /// </summary>
        public const string ITRV_DEVICE_CLASS = "ITRV";

        /// <summary>
        /// Meter type string for ICS Cellular gateways
        /// </summary>
        public const string ICS_GATEWAY = " ICSGEN2";
        /// <summary>
        /// Constant describing the CE version for Lithium, 3.9.0.0
        /// </summary>
        public const float CE_VERSION_LITHIUM_3_9 = 3.9F;

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
        /// Table identifiers in the range of 6144 to 8183 provide access to 
        /// Manufacturer-Pending tables 0 to 2039.  RegisterFWTbl = 2109 is 
        /// actually table 2109 + 4096 or 6205
        /// </summary>
        internal const ushort PENDING_BIT = 4096;
        /// <summary>
        /// MFG defined event number.  This can be anything between 1 and 254.
        /// It will be used to identify pending table 2111 for activation and/or
        /// clearing.
        /// </summary>
        internal const byte COMM_EVENT_NUMBER = 3;
        /// <summary>
        /// MFG defined event number.  This can be anything between 1 and 254.
        /// It will be used to identify pending table 2181 for activation and/or
        /// clearing.
        /// </summary>
        protected const byte HAN_DEV_EVENT_NUMBER = 4;
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
        /// Size of chunks to break f/w download file into
        /// </summary>
        private const ushort BLOCK_SIZE = 128;    // Changed from 14247
        /// <summary>
        /// Overhead for firware download block
        /// </summary>
        private const ushort FWDL_BLOCK_OVERHEAD = 23;
        /// <summary>
        /// Length of the header in the firmware file
        /// </summary>
        internal const int FW_HEADER_LENGTH = 19;
        /// <summary>
        /// Number of bytes in the parameter list for MFG procedure 37
        /// </summary>
        protected const int INIT_FW_DOWNLOAD_PARAM_LEN = 17;
        /// <summary>
        /// Number of bytes in the parameter list for MFG procedure 37 when writing Third Party HAN FW
        /// </summary>
        internal const int INIT_FW_DOWNLOAD_THIRD_PARTY_LEN = 21;
        /// <summary>
        /// Size of field for firmware file size in parameter list for mfg 
        /// procedure 37
        /// </summary>
        internal const int IMAGE_SIZE_FIELD_LEN = 4;
        /// <summary>
        /// Size of field for size of blocks in parameter list for mfg 
        /// procedure 37
        /// </summary>
        internal const int CHUNK_SIZE_FIELD_LEN = 2;
        /// <summary>
        /// Size of the Device Class field in the parameter list for MFG Proc 37
        /// </summary>
        protected const int DEVICE_CLASS_LENGTH = 4;
        /// <summary>
        /// Manufacturer
        /// </summary>
        private const string MANUFACTURER = "ITRN";
        /// <summary>
        /// Packet overhead size. Used when reading tables.
        /// </summary>
        public const uint PACKET_OVERHEAD_SIZE = 8;
        /// <summary>
        /// Packets per read. Used when reading tables.
        /// </summary>
        public const uint PACKETS_PER_READ = 254;

        const byte JOIN_CONTROL_REQUEST_TYPE = 0x13;
        const byte ACTIVATE_ALTERNATIVE_KEY_REQUEST_TYPE = 0x28;
        const byte ACTIVATE_HAN_FW_REQUEST_TYPE = 0x15;
        const byte SET_HAN_MULTIPLIER_AND_DIVISOR_REQUEST_TYPE = 0x12;
        const byte ENABLE_DISABLE_HAN_REQUEST_TYPE = 0x20;
        const byte MINUTES_TO_DISABLE = 60;
        const byte ZERO = 0x00;
        internal const string AMI_GATEWAY = "AMI GTWY";
        private const string L_G_FOCUSAXD = "FOCUSAXD";
        private const string L_G_FOCUSAXR = "FOCUSAXR";
        private const int DEFAULT_MAX_PACKET_LENGTH = 1024;
        private const string GE_I210_MODEL = "I210+C  ";
        private const string GE_kV2_MODEL = "KV2     ";
        private const int GE_IDENTITY_BYTE = 0x80;
        private const int CREATE_EDL_RETRIES = 10;
        private const int CREATE_EDL_LOGON_RETRIES = 4;

        #endregion Constants

        #region Definitions

        /// <summary>
        /// Display Mode for Meter Display Mode
        /// </summary>
        public enum DisplayMode
        {
            /// <summary>
            /// Normal Display Mode
            /// </summary>
            NORMAL_MODE = 0,
            /// <summary>
            /// Alt Display Mode
            /// </summary>
            ALT_MODE = 1,
            /// <summary>
            /// Test Display Mode
            /// </summary>
            TEST_ALT_MODE = 2,
            /// <summary>
            /// Test Alt Mode
            /// </summary>
            TEST_MODE = 3,
            /// <summary>
            /// Used to retrieve the Editable Registers (Normal with modifications for Primary)
            /// </summary>
            EDIT_MODE = 4,
        }

		/// <summary>
		/// Available options for the Close config procedure.
		/// </summary>
		[Flags]
		public enum CloseConfigOptions: uint
		{
			/// <summary>
			/// No options selected
			/// </summary>
			None = 0x00000000,
			/// <summary>
			/// Clear energy data
			/// </summary>
			Energy = 0x00000004,
			/// <summary>
			/// Clear demand data
			/// </summary>
			Demand = 0x00000008,
			/// <summary>
			/// Clear history log data
			/// </summary>
			HistoryLog = 0x00000020,
			/// <summary>
			/// Clear CPC data
			/// </summary>
			CPC = 0x00000040,
			/// <summary>
			/// Clear State Monitor data
			/// </summary>
			StateMonitor = 0x00000080,
			/// <summary>
			/// Clear load profile data
			/// </summary>
			LoadProfile = 0x00000200,
			/// <summary>
			/// Clear self reads
			/// </summary>
			SelfRead = 0x00000800,
			/// <summary>
			/// Clear SiteScan data
			/// </summary>
			SiteScan = 0x00001000,
			/// <summary>
			/// Clear power quality data
			/// </summary>
			PowerQuality = 0x00010000,
			/// <summary>
			/// Clear mode control data
			/// </summary>
			ModeControl = 0x00020000,
			/// <summary>
			/// Clear option board data
			/// </summary>
			OptionBoard = 0x00100000,
			/// <summary>
			/// Make the meter Canadian
			/// </summary>
			CanadianMeter = 0x40000000,
			/// <summary>
			/// Clear all data
			/// </summary>
			ResetAllData = 0x80000000,
		}

		/// <summary>
		/// Result parameters for the close config procedure
		/// </summary>
		[Flags]
		public enum CloseConfigErrors : uint
		{
			/// <summary>
			/// No Errors
			/// </summary>
			None = 0x00000000,
			/// <summary>
			/// An error occurred changing Coefficients
			/// </summary>
			CoeffError = 0x00000001,
			/// <summary>
			/// An error occurred changing Constants
			/// </summary>
			ConstError = 0x00000002,
			/// <summary>
			/// An error occurred changing Energies
			/// </summary>
			EnergyError = 0x00000004,
			/// <summary>
			/// An error occurred changing Demands
			/// </summary>
			DemandError = 0x00000008,
			/// <summary>
			/// An error occurred changing the Display
			/// </summary>
			DisplayError = 0x00000010,
			/// <summary>
			/// An error occurred changing History log
			/// </summary>
			HistLogError = 0x00000020,
			/// <summary>
			/// An error occurred changing IO
			/// </summary>
			IOError = 0x00000100,
			/// <summary>
			/// An error occurred changing Load Profile
			/// </summary>
			LoadProfileError = 0x00000200,
			/// <summary>
			/// An error occurred changing Self Read
			/// </summary>
			SelfReadError = 0x00000800,
			/// <summary>
			/// An error occurred changing SiteScan
			/// </summary>
			SiteScanError = 0x00001000,
			/// <summary>
			/// An error occurred changing Calendar
			/// </summary>
			CalendarError = 0x00002000,
			/// <summary>
			/// An error occurred changing TOU
			/// </summary>
			TOUError = 0x00004000,
			/// <summary>
			/// An error occurred changing Mode
			/// </summary>
			ModeError = 0x00020000,
			/// <summary>
			/// An error occurred changing Event Log
			/// </summary>
			EventLogError = 0x00080000,
			/// <summary>
			/// An error occurred changing Option Board
			/// </summary>
			OptionBoardError = 0x00100000,
			/// <summary>
			/// An error occurred changing Billing Schedule
			/// </summary>
			BillingScheduleError = 0x00200000,
			/// <summary>
			/// An error occurred changing Totalization
			/// </summary>
			TotalizationError = 0x00400000,
			/// <summary>
			/// An error occurred due to old software
			/// </summary>
			OldSWError = 0x40000000,
			/// <summary>
			/// An error occurred due to Canadian event log
			/// </summary>
			CanEventLogError = 0x80000000,
		}

		private enum ErrorType
		{
			NON_FATAL_1 = 0,
			NON_FATAL_2 = 1,
			FATAL = 2
		}

        /// <summary>
        /// History Events Enumeration
        /// </summary>
        /// <remarks>
        /// If you add any events to this enumeration, be sure to add them to
        /// HistoryLogEventList property in respective MFGTable2048 classes as well
        /// as adding a dictionary entry in the appropriate EventDictionary.  Furthermore,
        /// add the event to the HistoryEvents enum of the EndpointServerMeterEvents class
        /// in the file ServiceResponseData.cs of Itron.Metering.MeterServer project
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue#     Description
        //  -------- --- -------  ------     -------------------------------------------
        //  07/02/09 AF  2.20.11  135878     Renamed CUST_SCHED_CHANGED to
        //                                   SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT
        //                                   because this event is a sitescan error
        //                                   for OpenWay poly meters 
        // 09/15/09 MMD  2.21.12  140581     Added event Network Hush Stared
        // 10/12/09 MMD  2.30.09  141987     Removed Mfg event 45, as it doesnot exist
        // 05/11/10 jrf  2.40.01             Added the Table Configuration event.
        // 12/12/10 JB                       Added the Critical Peak Pricing
        // 06/03/11 AF   2.51.04  175033     Added extended outage recovery event
        // 08/11/11 jrf  2.52.02  TREQ2709   Changing Register Firmware Download Status event
        //                                      to Firmware Download Event Log Full event.
        // 11/28/11 jrf  2.52.02  TC5361     Added new voltage monitoring events.
        // 03/07/12 AF   2.53.48  195229     Added HAN device joined exception event
        // 03/13/12 jrf  2.53.49  194582     Correcting BILLING_SCHED_EXPIRED event ID.
        // 04/30/13 PGH  2.80.24  327121     Added additional events common to the CE for use with the End-Point-Server
        // 05/30/14 jrf  3.50.97  517744     The name of mfg. event 163 changed from HAN Load Control Event Sent to ICS ERT Event.
        // 02/10/15 AF   4.10.03  561372     Added the HAN Next Block Price Commit Timeout event
        // 02/03/16 PGH  4.50.226 RTT556309  Added temperature events
        // 04/08/16 CFB  4.50.243 WR670050   Applied MFG OFFSET to On "Demand Periodic Read"
        // 05/12/16 MP   4.50.266 WR685323   Changed ON_DEMAND_PERIOD_READ to ON_DEMAND_PERIODIC_READ to match ANSIDeviceStrings resource file.
        // 05/20/16 MP   4.50.270 WR685690   Added enum for EVENT_HARDWARE_ERROR_DETECTION
        // 07/12/16 MP   4.70.7   WR688986   Changed first and second event to PRIMARY_POWER_DOWN and PRIMARY_POWER_UP respectively
        // 07/18/16 MP   4.70.8   WR600059   Added definition for event 137 (NETWORK_TIME_UNAVAILABLE)
        // 07/29/16 MP   4.70.11  WR704220   Added definition for events 213 and 214 (WRONG_CONFIG_CRC and CHECK_CONFIG_CRC)
        // 07/29/16 MP   4.70.11  WR702277   Made CARBON_ON_DEMAND_PERIODIC_READ because event Id switched from 244 to 2292 after Be
        // 08/08/16 MP   4.70.12  WR704905   Added Event 155 - IO Out Reconfigure
        // 09/26/16 AF   4.70.20  WR716311   Renamed DEVICE_DATA_RESET to BILLING_DATA_CLEARED
        // 11/10/16 PGH  4.70.33  WR726575   Added Event Logging Suspended and Logging Resumed
        public enum HistoryEvents
        {
            /// <summary>
            /// Power Outage
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            PRIMARY_POWER_DOWN = 1,
            /// <summary>
            /// Power Restored
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            PRIMARY_POWER_UP = 2,
            /// <summary>
            /// Device Data Reset
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            BILLING_DATA_CLEARED = MFG_OFFSET + 3,
            /// <summary>
            /// Billing Schedule Expired
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
             BILLING_SCHED_EXPIRED = MFG_OFFSET + 4,
            /// <summary>
            /// DST Time Change
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            DST_TIME_CHANGE = MFG_OFFSET + 5,
            /// <summary>
            /// Clock Reset
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            CLOCK_RESET = 6,
            /// <summary>
            /// Demand Threshold Exceeded
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            DEMAND_THRESHOLD_EXCEEDED = MFG_OFFSET + 7,
            /// <summary>
            /// Demand Threshold Restored
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            DEMAND_THRESHOLD_RESTORED = MFG_OFFSET + 8,
            /// <summary>
            /// Logon Successful
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            LOGON_SUCCESSFUL = MFG_OFFSET + 10,
            /// <summary>
            /// Security Successful
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            SECURITY_SUCCESSFUL = MFG_OFFSET + 12,
            /// <summary>
            /// Security Failed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SECURITY_FAILED = MFG_OFFSET + 13,
            /// <summary>
            /// Load Profile Data Reset
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            LOAD_PROFILE_RESET = MFG_OFFSET + 14,
            /// <summary>
            /// History Log Cleared
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            HIST_LOG_CLEARED = 16,
            /// <summary>
            /// History Log Pointers Updated
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HIST_PTRS_UPDATED = 17,
            /// <summary>
            /// Event Log Cleared
            /// </summary>
           [EnumEventInfoAttribute("Table76")]
            EVENT_LOG_CLEARED = 18,
            /// <summary>
            /// Event Log Pointers Updated
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            EVENT_LOG_PTRS_UPDATED = 19,
            /// <summary>
            /// Demand Reset
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            DEMAND_RESET = 20,
            /// <summary>
            /// Self Read Occurred
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SELF_READ_OCCURRED = 21,
            /// <summary>
            /// Input Channel Hi
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            INPUT_CHANNEL_HI = MFG_OFFSET + 22,
            /// <summary>
            /// Input Channel Lo
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            INPUT_CHANNEL_LO = MFG_OFFSET + 23,
            /// <summary>
            /// TOU Season Changed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            TOU_SEASON_CHANGED = 24,
            /// <summary>
            /// TOU Rate Change
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            RATE_CHANGE = 25,
            /// <summary>
            /// External Event
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            EXTERNAL_EVENT = MFG_OFFSET + 26,
            /// <summary>
            /// Custom Schedule Changed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT = MFG_OFFSET + 27,
            /// <summary>
            /// Pending Table Activated
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            PENDING_TABLE_ACTIVATION = 28,
            /// <summary>
            /// Standard Event 29 - Pending Table Cleared
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            PENDING_TABLE_CLEAR = 29,
            /// <summary>
            /// Mfg Event 29 (2077) - SiteScan Error
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            SITESCAN_ERROR = MFG_OFFSET + 29,
            /// <summary>
            /// Voltage Quality Log Pointers Updated
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            VQ_LOG_PTRS_UPDATED = MFG_OFFSET + 30,
            /// <summary>
            /// Voltage Quality Log Nearly Full
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            VQ_LOG_NEARLY_FULL = MFG_OFFSET + 31,
            /// <summary>
            /// Enter Test Mode
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            ENTER_TEST_MODE = 32,
            /// <summary>
            /// Exit Test mode
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            EXIT_TEST_MODE = 33,
            /// <summary>
            /// ABC Phase Rotation
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            ABC_PH_ROTATION_ACTIVE = MFG_OFFSET + 34,
            /// <summary>
            /// CBA Phase Rotation
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            CBA_PH_ROTATION_ACTIVE = MFG_OFFSET + 35,
            /// <summary>
            /// Device Reporgrammed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            METER_REPROGRAMMED = 36,
            /// <summary>
            /// Illegal Configuration Error
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
             CONFIGURATION_ERROR = 37,
            /// <summary>
            /// CPC Communication Error
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            CPC_COMM_ERROR = MFG_OFFSET + 38,
            /// <summary>
            /// VQ Log Cleared
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            VQ_LOG_CLEARED = MFG_OFFSET + 40,
            /// <summary>
            /// TOU Schedule Error
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            TOU_SCHEDULE_ERROR = MFG_OFFSET + 41,
            /// <summary>
            /// Mass Memory Error
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            MASS_MEMORY_ERROR = MFG_OFFSET + 42,
            /// <summary>
            /// Loss of Phase Restored
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            LOSS_OF_PHASE_RESTORE = MFG_OFFSET + 43,
            /// <summary>
            /// Low Batteru
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            LOW_BATTERY = 44,
            /// <summary>
            /// Loss of Phase
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            LOSS_OF_PHASE = 45,
            /// <summary>
            /// Register Full Scale
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REGISTER_FULL_SCALE = 46,
            /// <summary>
            /// Reverse Power Flow Restore
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            REVERSE_POWER_FLOW_RESTORE = MFG_OFFSET + 47,
            /// <summary>
            /// Reverse Power Flow
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REVERSE_POWER_FLOW = 48,
            /// <summary>
            /// SiteScan Diag 1 Active
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SS_DIAG1_ACTIVE = MFG_OFFSET + 49,
            /// <summary>
            /// SiteScan Diag 2 Active
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SS_DIAG2_ACTIVE = MFG_OFFSET + 50,
            /// <summary>
            /// SiteScan Diag 3 Active
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SS_DIAG3_ACTIVE = MFG_OFFSET + 51,
            /// <summary>
            /// SiteScan Diag 4 Active
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            SS_DIAG4_ACTIVE = MFG_OFFSET + 52,
            /// <summary>
            /// SiteScan Diag 5 Active
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SS_DIAG5_ACTIVE = MFG_OFFSET + 53,
            /// <summary>
            /// SiteScan Diag 1 Inactive
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SS_DIAG1_INACTIVE = MFG_OFFSET + 54,
            /// <summary>
            /// SiteScan Diag 2 Inactive
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SS_DIAG2_INACTIVE = MFG_OFFSET + 55,
            /// <summary>
            /// SiteScan Diag 3 Inactive
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SS_DIAG3_INACTIVE = MFG_OFFSET + 56,
            /// <summary>
            /// SiteScan Diag 4 Inactive
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            SS_DIAG4_INACTIVE = MFG_OFFSET + 57,
            /// <summary>
            /// SiteScan Diag 5 Inactive
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            SS_DIAG5_INACTIVE = MFG_OFFSET + 58,
            /// <summary>
            /// SiteScan Diag 6 Active
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            SS_DIAG6_ACTIVE = MFG_OFFSET + 59,
            /// <summary>
            /// SiteScan Diag 6 Inactive
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            SS_DIAG6_INACTIVE = MFG_OFFSET + 60,
            /// <summary>
            /// Self Read Cleared
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            SELF_READ_CLEARED = MFG_OFFSET + 61,
            /// <summary>
            /// Inversion Tamper
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            INVERSION_TAMPER = MFG_OFFSET + 62,
            /// <summary>
            /// Removal tamper
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            REMOVAL_TAMPER = MFG_OFFSET + 63,
            /// <summary>
            /// Register Download Failed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            REG_DWLD_FAILED = MFG_OFFSET + 66,
            /// <summary>
            /// Register Download Succeeded
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REG_DWLD_SUCCEEDED = MFG_OFFSET + 67,
            /// <summary>
            /// RFLAN Download Succeeded
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            RFLAN_DWLD_SUCCEEDED = MFG_OFFSET + 68,
            /// <summary>
            /// Zigbee Download Succeeded
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            ZIGBEE_DWLD_SUCCEEDED = MFG_OFFSET + 69,
            /// <summary>
            /// Meter Firmware Download Succeeded
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            METER_FW_DWLD_SUCCEDED = MFG_OFFSET + 72,
            /// <summary>
            /// Meter Firmware Download Failed 
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            METER_DWLD_FAILED = MFG_OFFSET + 73,
            /// <summary>
            /// Zigbee Download Failed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            ZIGBEE_DWLD_FAILED = MFG_OFFSET + 81,
            /// <summary>
            /// RFLAN Download Failed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            RFLAN_DWLD_FAILED = MFG_OFFSET + 82,
            /// <summary>
            /// SiteScan Error Cleared
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            SITESCAN_ERROR_CLEARED = MFG_OFFSET + 84,
            /// <summary>
            /// Load Firmware
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            LOAD_FIRMWARE = MFG_OFFSET + 85,
            /// <summary>
            /// Reset Counters
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            RESET_COUNTERS = MFG_OFFSET + 101,
            /// <summary>
            /// Fatal Error Occurred
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            FATAL_ERROR = MFG_OFFSET + 121,
            /// <summary>
            /// A Periodic Read has occurred
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            PERIODIC_READ = MFG_OFFSET + 125,
            /// <summary>
            /// Service Limiting Active Tier Changed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SERVICE_LIMITING_ACTIVE_TIER_CHANGED = MFG_OFFSET + 126,
            /// <summary>
            /// Service Limiting Connect Switch
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
             SERVICE_LIMITING_CONNECT_SWITCH = 127,
             /// <summary>
             /// Service Limiting Switch Period
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             SERVICE_LIMITING_SWITCH_PERIOD = 129,
             /// <summary>
             /// Prior Demand Reset
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             PRIOR_DEMAND_RESET = 132,
             /// <summary>
             /// Prior Self Read
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             PRIOR_SELF_READ = 133,
             /// <summary>
             /// Safe Initialization After Fatal
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             SAFE_INITIALIZATION_AFTER_FATAL = 142,
             /// <summary>
             /// Base Switch Command
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             BASE_SWITCH_COMMAND = 144,
             /// <summary>
             /// Voltage Monitoring EOI
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             VOLTAGE_MONITORING_EOI = 145,
             /// <summary>
             /// Commit LAN Log
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             COMMIT_LAN_LOG = 146,
             /// <summary>
             /// Commit HAN Log
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             COMMIT_HAN_LOG = 147,
             /// <summary>
             /// LAN/HAN Locate by Event
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             LAN_HAN_LOCATE_BY_EVENT = 149,
             /// <summary>
             /// LAN/HAN Locate by Time
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             LAN_HAN_LOCATE_BY_TIME = 150,
            /// <summary>
            /// IO Out Reconfigure - CB2 Meters Get next season
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            IO_OUT_RECONFIGURE = 155,
            /// <summary>
            /// TOU Day Type 1
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
             TOU_DAY_TYPE_1 = 157,
             /// <summary>
             /// TOU Day Type 2
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             TOU_DAY_TYPE_2 = 158,
             /// <summary>
             /// TOU Day Type 3
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             TOU_DAY_TYPE_3 = 159,
             /// <summary>
             /// TOU Day Type 4
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             TOU_DAY_TYPE_4 = 160,
             /// <summary>
             /// Pending Table Failure
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             PENDING_TABLE_ACTIVATE_FAIL = 161,
             /// <summary>
             /// Third Party Reserved
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             THIRD_PARTY_RESERVED = 182,
             /// <summary>
             /// ZigBee Download Terminate
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             ZIGBEE_DOWNLOAD_TERMINATE = 183,
            /// <summary>
            /// Event logging suspended
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            EVENT_LOGGING_SUSPENDED = 206,
            /// <summary>
            /// Event logging suspended
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            LOGGING_RESUMED = 207,
            /// <summary>
            /// End Device Sealed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
             END_DEVICE_SEALED = 211,
             /// <summary>
             /// End Device Unsealed
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             END_DEVICE_UNSEALED = 212,
            /// <summary>
            /// Wrong Config CRC - Event is logged when configuration CRC is wrong
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            WRONG_CONFIG_CRC= 213 + MFG_OFFSET,
            /// <summary>
            /// Check Config CRC - Event occurs every 24 hrs to check the configuration CRC. On Wrong CRC it logs
            /// WRONG_CONFIG_CRC (Event 213)
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            CHECK_CONFIG_CRC = 214 + MFG_OFFSET,
            /// <summary>
            /// Firmware Activate in Progress
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
             FIRMWARE_ACTIVATE_IN_PROGRESS = 229,
             /// <summary>
             /// Connect Forced
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             CONNECT_FORCED = 242,
             /// <summary>
             /// Disconnect Forced
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             DISCONNECT_FORCED = 243,
             /// <summary>
             /// On Demand Periodic Read
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             ON_DEMAND_PERIODIC_READ = MFG_OFFSET + 244,
            /// <summary>
            /// On Demand Periodic Read - Post carbon event it was changed to 244 + mfg offset 
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            CARBON_ON_DEMAND_PERIODIC_READ = 244,
            /// <summary>
            /// Firmware Download Debug
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
             FIRMWARE_DOWNLOAD_DEBUG = 251,
            /// <summary>
            /// Table Written Event
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            TABLE_WRITTEN = MFG_OFFSET + 130,
            /// <summary>
            /// Base Mode Error
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            BASE_MODE_ERROR = MFG_OFFSET + 131,
            /// <summary>
            /// Pending Reconfigure
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            PENDING_RECONFIGURE = MFG_OFFSET + 134,
             /// <summary>
             /// Magnetic Tamper Detected
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             MAGNETIC_TAMPER_DETECTED = MFG_OFFSET + 135,
             /// <summary>
             /// Magnetic Tamper Cleared
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             MAGNETIC_TAMPER_CLEARED = MFG_OFFSET + 136,
             /// <summary>
             /// Network Time Unavailable
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             NETWORK_TIME_UNAVAILABLE = 137 + MFG_OFFSET,
             /// <summary>
             /// Current Threshold Exceeded
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
             CTE_EVENT = MFG_OFFSET + 140,
            /// <summary>
            /// Event Tamper Cleared
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            EVENT_TAMPER_CLEARED = MFG_OFFSET + 141,
            /// <summary>
            /// LAN HAN Log Reset
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            LAN_HAN_LOG_RESET = MFG_OFFSET + 148,
            ///// <summary>
            ///// Display Download Initiated
            ///// </summary>
            //DISPLAY_DWLD_INITIATED = MFG_OFFSET + 151,
             ///<summary>
             ///Pending Table Activate Failed
             ///</summary>
             [EnumEventInfoAttribute("Table74")]
            PENDING_TBL_ACTIVATE_FAIL = MFG_OFFSET + 161,
            /// <summary>
            /// HAN Device Status Change
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            HAN_DEVICE_STATUS_CHANGE = MFG_OFFSET + 162,
            /// <summary>
            /// ICS ERT Event
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            ICS_ERT_EVENT = MFG_OFFSET + 163,
            /// <summary>
            /// HAN Load Control Event Status
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HAN_LOAD_CONTROL_EVENT_STATUS = MFG_OFFSET + 164,
            /// <summary>
            /// HAN Load Control Event Opt Out
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HAN_LOAD_CONTROL_EVENT_OPT_OUT = MFG_OFFSET + 165,
            /// <summary>
            /// HAN Messaging Event
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            HAN_MESSAGING_EVENT = MFG_OFFSET + 166,
            /// <summary>
            /// HAN Device added or removed event
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            HAN_DEVICE_ADDED_OR_REMOVED = MFG_OFFSET + 167,
            /// <summary>
            /// HAN Event Cache Overflow
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HAN_EVENT_CACHE_OVERFLOW = MFG_OFFSET + 168,
             ///<summary>
             ///Register Download Initiated
             ///</summary>
             [EnumEventInfoAttribute("Table74")]
            REG_DWLD_INITIATED = MFG_OFFSET + 169,
            /// <summary>
            /// RFLAN Download Initiated
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            RFLAN_DWLD_INITIATED = MFG_OFFSET + 170,
            /// <summary>
            /// Zigbee Download Initiated
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            ZIGBEE_DWLD_INITIATED = MFG_OFFSET + 171,
            /// <summary>
            /// Register download Initiation Failed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REG_DWLD_INITIATION_FAILED = MFG_OFFSET + 172,
            /// <summary>
            /// RFLAN download Initiation Failed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            RFLAN_DWLD_INITIATION_FAILED = MFG_OFFSET + 173,
            /// <summary>
            /// Zigbee download Initiation Failed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            ZIGBEE_DWLD_INITIATION_FAILED = MFG_OFFSET + 174,
            /// <summary>
            /// Firmware Download Event Log Full
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            FW_DWLD_EVENT_LOG_FULL = MFG_OFFSET + 175,
            /// <summary>
            /// RFLAN Firmware Download Status
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            RFLAN_FW_DWLD_STATUS = MFG_OFFSET + 176,
            /// <summary>
            /// Zigbee Firmware Download Status
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            ZIGBEE_FW_DWLD_STATUS = MFG_OFFSET + 177,
            /// <summary>
            /// Register Download Already Active
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REG_DWLD_ALREADY_ACTIVE = MFG_OFFSET + 178,
            /// <summary>
            /// RFLAN Download Already Active
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            RFLAN_DWLD_ALREADY_ACTIVE = MFG_OFFSET + 179,
            /// <summary>
            /// Extended Outage Recovery Mode Entered
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            EXTENDED_OUTAGE_RECOVERY_MODE_ENTERED = MFG_OFFSET + 180,
            /// <summary>
            /// Register Download Terminated
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            THIRD_PARTY_HAN_FW_DWLD_STATUS = MFG_OFFSET + 181,
            /// <summary>
            /// FW Load aborted
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            FIRMWARE_DWLD_ABORTED = MFG_OFFSET + 184,
            /// <summary>
            /// Remote Connect Failed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REMOTE_CONNECT_FAILED = MFG_OFFSET + 185,
            /// <summary>
            /// Remote Disconnect Failed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REMOTE_DISCONNECT_FAILED = MFG_OFFSET + 186,
            /// <summary>
            /// Remoate Diconnect Relay Activated
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REMOTE_DISCONNECT_RELAY_ACTIVATED = MFG_OFFSET + 187,
            /// <summary>
            /// Remote Connect Relay Activated
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            REMOTE_CONNECT_RELAY_ACTIVATED = MFG_OFFSET + 188,
            /// <summary>
            /// Remote Connect Relay Initiated
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            REMOTE_CONNECT_RELAY_INITIATED = MFG_OFFSET + 189,
            /// <summary>
            /// Table written
            /// </summary>
            [EnumEventInfoAttribute("Table76")]
            TABLE_CONFIGURATION = MFG_OFFSET + 191,
            /// <summary>
            /// Critical Peak Pricing Event
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            CPP_EVENT = MFG_OFFSET + 192,
           /// <summary>
           /// RMS Voltage restores from below low threshold to the normal range
           /// </summary>
           [EnumEventInfoAttribute("Table74")]
           RMS_VOLTAGE_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL = MFG_OFFSET + 194,
           /// <summary>
           /// RMS Voltage restores from above high threshold to the normal range
           /// </summary>
           [EnumEventInfoAttribute("Table74")]
           RMS_VOLTAGE_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL = MFG_OFFSET + 195,
           /// <summary>
           /// VH restores from below low threshold to the normal range
           /// </summary>
           [EnumEventInfoAttribute("Table74")]
           VOLT_HOUR_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL = MFG_OFFSET + 196,
           /// <summary>
           /// VH restores from above high threshold to the normal range
           /// </summary>
           [EnumEventInfoAttribute("Table74")]
           VOLT_HOUR_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL = MFG_OFFSET + 197,
            /// <summary>
            /// Hardware Error
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
           EVENT_HARDWARE_ERROR_DETECTION = MFG_OFFSET + 215,
           /// <summary>
           /// Temperature Exceeds Threshold1
           /// </summary>
           [EnumEventInfoAttribute("Table74")]
           TEMPERATURE_EXCEEDS_THRESHOLD1 = MFG_OFFSET + 223,
           /// <summary>
           /// Temperature Exceeds Threshold2
           /// </summary>
           [EnumEventInfoAttribute("Table74")]
           TEMPERATURE_EXCEEDS_THRESHOLD2 = MFG_OFFSET + 224,
           /// <summary>
           /// Temperature Returned to normal
           /// </summary>
           [EnumEventInfoAttribute("Table74")]
           TEMPERATURE_RETURNED_TO_NORMAL = MFG_OFFSET + 225,
            /// <summary>
            /// Network Hush Started
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            NETWORK_HUSH_STARTED = MFG_OFFSET + 228,
            /// <summary>
            /// Meter receives a f/w activate while activating some other f/w
            /// </summary>
           [EnumEventInfoAttribute("Table74")]
            ACTIVATE_IN_PROGRESS = MFG_OFFSET + 229,
            /// <summary>
            /// Load Voltage Preset at time of Connect
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            LOAD_VOLT_PRESENT = MFG_OFFSET + 230,
            /// <summary>
            /// Pending Table Clear Failed
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            PENDING_TABLE_CLEAR_FAIL = MFG_OFFSET + 231,
            /// <summary>
            /// Firmware Pending Table is full when request to add to it is made
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            FIRMWARE_PENDING_TABLE_FULL = MFG_OFFSET + 232,
            /// <summary>
            /// Firmware Pending table header is swapped with new header
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            FIRMWARE_PENDING_TABLE_HEADER_SWAPPED = MFG_OFFSET + 233,
            /// <summary>
            /// DEBUG EVENT - Event Scheduling Rejected
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            EVENT_SCHEDULING_REJECTED = MFG_OFFSET + 234,
            /// <summary>
            /// C12.22 Registration Attempt
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            C12_22_REGISTRATION_ATTEMPT = MFG_OFFSET + 235,
            /// <summary>
            /// C12.22 Registered
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            C12_22_REGISTERED = MFG_OFFSET + 236,
            /// <summary>
            /// C12.22 Deregistration Attempt
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            C12_22_DEREGISTRATION_ATTEMPT = MFG_OFFSET + 237,
            /// <summary>
            /// C12.22 Deregistered
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            C12_22_DEREGISTERED = MFG_OFFSET + 238,
            /// <summary>
            /// C12.22 RFLAN ID Changed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            C12_22_RFLAN_ID_CHANGED = MFG_OFFSET + 239,
            /// <summary>
            /// Time Adjustment Failed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            TIME_ADJUSTMENT_FAILED = MFG_OFFSET + 240,
            /// <summary>
            /// Event Cache Overflow
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            EVENT_CACHE_OVERFLOW = MFG_OFFSET + 241,
             /// <summary>
             /// Event Generic History Event
             /// </summary>
             [EnumEventInfoAttribute("Table74")]
            EVENT_GENERIC_HISTORY_EVENT = MFG_OFFSET + 245,
             /// <summary>
            /// RMS Voltage Below Low Threshold
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            RMS_VOLTAGE_BELOW_LOW_THRESHOLD = MFG_OFFSET + 246,
            /// <summary>
             /// Volt(RMS) Above Threshold
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
             VOLT_RMS_ABOVE_THRESHOLD = MFG_OFFSET + 247,
            /// <summary>
            /// Volt hour Below Low Threshold
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            VOLT_HOUR_BELOW_LOW_THRESHOLD = MFG_OFFSET + 248,
            /// <summary>
            /// Volt hour Above Threshold
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            VOLT_HOUR_ABOVE_THRESHOLD = MFG_OFFSET + 249,
            /// <summary>
            /// An Error occurred during a Pending Table Procedure
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
             PENDING_TABLE_ERROR = MFG_OFFSET + 250,
            /// <summary>
            /// Security Event
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SECURITY_EVENT = MFG_OFFSET + 252,
            /// <summary>
            /// Key Rollover Pass
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            KEY_ROLLOVER_PASS = MFG_OFFSET + 253,
            /// <summary>
            /// Sign Key Replace Processing Pass
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SIGN_KEY_REPLACE_PROCESSING_PASS = MFG_OFFSET + 254,
            /// <summary>
            /// Symmetric Key Replace Processing Pass
            /// </summary>
             [EnumEventInfoAttribute("Table74")]
            SYMMETRIC_KEY_REPLACE_PROCESSING_PASS = MFG_OFFSET + 255,
            /// <summary>
            /// M2 Gateway only event - configuration download
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            GW_CONFIGURATION_DOWNLOAD = MFG_OFFSET + 256,
            /// <summary>
            /// HAN Next Block Price Commit Timeout
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HAN_NEXT_BLOCK_PRICE_COMMIT_TIMEOUT = MFG_OFFSET + 272,
            /// <summary>
            /// HAN Load Control Opt Out Exception
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HAN_LOAD_CTRL_OPT_OUT_EXCEPTION = MFG_OFFSET + 386,
            /// <summary>
            /// HAN Device Status Changed
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HAN_DEVICE_STATUS_CHANGED = MFG_OFFSET + 388,
            /// <summary>
            /// HAN Device Added Exception
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HAN_DEVICE_ADDED = MFG_OFFSET + 389,
            /// <summary>
            /// HAN Device Joined Exception
            /// </summary>
            [EnumEventInfoAttribute("Table74")]
            HAN_DEVICE_JOINED_EXCEPTION = MFG_OFFSET + 398,
		}

		/// <summary>
		/// The indices into table 42 for each of the passwords.
		/// </summary>
		protected enum ANSIStdPasswordIndex : int
		{
			/// <summary>
			/// Index of the Primary password
			/// </summary>1
			PRIMARY = 3,
			/// <summary>
			/// Index of the Limited Reconfigure password
			/// </summary>
			LIMITED_RECONFIG = 2,
			/// <summary>
			/// Index of the secondary password
			/// </summary>
			SECONDARY = 1,
			/// <summary>
			/// Index of the tertiary password
			/// </summary>
			TERTIARY = 0,
		}

		/// <summary>
		/// Display Type for Change Display Mode
		/// </summary>
		protected enum DisplayType
		{
			/// <summary>
			/// Test Display Mode
			/// </summary>
			TEST_DISPLAY = 0,
			/// <summary>
			/// Enter Test Alt Mode from Test
			/// </summary>
			TEST_MODE_TO_TEST_ALT = 1,
			/// <summary>
			/// Normal From Test Display Mode
			/// </summary>
			EXIT_TEST_TO_NORMAL = 2,
			/// <summary>
			/// Goto Normal Mode (From Alt)
			/// </summary>
			NORMAL_MODE = 3,
			/// <summary>
			/// Goto Alt Mode
			/// </summary>
			ALT_MODE = 4,
			/// <summary>
			/// Goto Toolbox Mode
			/// </summary>
			TOOLBOX_MODE = 5,
			/// <summary>
			/// Scroll Lock the Display
			/// </summary>
			SCROLL_LOCK = 6,
		}

		/// <summary>
		/// Bitfield definition for the Set Time/Date mask bitfield
		/// </summary>
		[Flags]
		protected enum SET_MASK_BFLD : byte
		{
			/// <summary>
			/// Flag for setting the time
			/// </summary>
			SET_TIME_FLAG = 0x01,
			/// <summary>
			/// Flag for setting the date
			/// </summary>
			SET_DATE_FLAG = 0x02,
			/// <summary>
			/// Flag for setting the Time Date Qualifier field
			/// </summary>
			SET_TIME_DATE_QUAL = 0x04,
		}

		/// <summary>
		/// Bitfield definition for the Set Time Date Qual bitfield
		/// </summary>
		protected enum TIME_DATE_QUAL_BFLD : byte
		{
			/// <summary>
			/// The current day of the week
			/// </summary>
			DAY_OF_WEEK = 0x07,
			/// <summary>
			/// Whether or not the time is in DST
			/// </summary>
			DST_FLAG = 0x08,
			/// <summary>
			/// Whether or not the time is in GMT
			/// </summary>
			GMT_FLAG = 0x10,
			/// <summary>
			/// Whther or not the time zone has been applied
			/// </summary>
			TM_ZN_APPLIED_FLAG = 0x20,
			/// <summary>
			/// Whether or not DST has been applied
			/// </summary>
			DST_APPLIED_FLAG = 0x40,
		}

        /// <summary>
        /// Enum of the table ids of the supported pending tables
        /// </summary>
        internal enum FWDLTableIds
        {
            /// <summary>
            /// FWDL table id for TOU config table
            /// </summary>
            TOUConfigTbl = 2090,
            /// <summary>
            /// FWDL table id for register firmware
            /// </summary>
            RegisterFWTbl = 2109,
            /// <summary>
            /// FWDL table id for RFLAN firmware
            /// </summary>
            CommModuleFWTbl = 2110,
            /// <summary>
            /// FWDL table id for Zigbee firmware
            /// </summary>
            HANModuleFWTbl = 2111,
            /// <summary>
            /// FWDL table ID for HAN Pricing
            /// </summary>
            HANPricingTable = 2134,
            /// <summary>
            /// FWDL table id for HAN device firmware
            /// </summary>
            HANDeviceFWTbl = 2181,
        }

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
            /// ErrorOrCPPInvalidDuration 
            /// </summary>
            ErrorOrCPPInvalidDuration = 4
        }

		#endregion Definitions

		#region Public Methods

		/// <summary>
		/// The CANSIDevice constructor
		/// </summary>
		/// <param name="ceComm">Communications object to use</param>
		/// <example>
		/// <code>
		/// Communication comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// CANSIDevice ANSIDevice = new CANSIDevice(comm);
		/// </code>
		/// </example>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  07/13/05 mrj 7.13.00 N/A    Created
		//  03/16/07 KRC  8.00.19        Adding Reference Time to PSEM
        //  12/27/13 jrf 3.50.16 TQ9562 Moved instantiation of m_rmStrings to where it is declared,
        //                              so it can be used in static methods.
		//
		public CANSIDevice(Itron.Metering.Communications.ICommunications ceComm)
		{
			m_PSEM = new CPSEM(ceComm);

			// Since PSEM knows about the time format, he should also know about the Reference Time.
			m_PSEM.ReferenceTime = MeterReferenceTime;

			m_lstDSTDates = new List<CDSTDatePair>();
			m_TOUSchedule = null;

			InitializeData();
		}

		/// <summary>
		/// The CANSIDevice constructor
		/// </summary>
		/// <param name="PSEM">Protocol instance to use</param>
		/// <example><code>
		/// Communication comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// PSEM psem( comm )
		/// CANSIDevice ANSIDevice = new CANSIDevice(psem);
		/// </code></example>
		/// 
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/01/06     7.30.00 N/A    Created
		//  02/28/07 AF  8.00.14 2432   Added member variable to keep track of
		//                              logged on state
		//  03/16/07 KRC  8.00.19        Adding Reference Time to PSEM
        //  12/27/13 jrf 3.50.16 TQ9562 Moved instantiation of m_rmStrings to where it is declared,
        //                              so it can be used in static methods.
		//
		public CANSIDevice(CPSEM PSEM)
		{
			m_PSEM = PSEM;
			// Since PSEM knows about the time format, he should also know about the Reference Time.
			m_PSEM.ReferenceTime = MeterReferenceTime;

			m_lstDSTDates = new List<CDSTDatePair>();
			m_TOUSchedule = null;

			InitializeData();
		}

        /// <summary>
        /// Creates the appropriate device object. MUST BE LOGGED ON!!!
        /// </summary>
        /// <param name="CommPort">The open communication port</param>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="key">Signed authorization security key</param>
        /// <returns>The Comm Module object.</returns>
        // Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        // -------- --- -------   -- ------ ---------------------------------------
        // 12/04/13 jrf 3.50.10             Created
        // 01/02/14 mah 3.50.17             Added support for creating ICS gateway devices when already logged on to the
        //                                  comm module
        // 10/03/14 AF  4.00.67  TQ  15236  Added support for ITRK polyphase cellular devices
        // 10/31/14 jrf 4.00.82  WR 542694 Added support for identifying Bridge meter with signed authorizaion.
        // 11/04/14 jrf 4.00.85 WR 542694 Adding missing  null checks for XMLOpenWaySettings and NewDevice and using 
        //                                ITRF device in the ITRF case.
        public static CANSIDevice CreateDevice(ICommunications CommPort, CPSEM psem, SignedAuthorizationKey key = null)
        {
            CANSIDevice NewDevice = null;
            string strMeterType = "";
            string strDeviceClass = ""; 

            if (null != psem)
            {
                try
                {
                    //Get the meter type
                    CTable00 Table0 = new CTable00(psem);
                    CTable01 Table1 = new CTable01(psem, Table0.StdVersion);

                    strMeterType = Table1.Model.ToUpper(CultureInfo.InvariantCulture);
                    strDeviceClass = Table0.DeviceClass;

                    Logger.TheInstance.WriteDetail( psem, "Device class = " + strDeviceClass );
                    Logger.TheInstance.WriteDetail(psem, "Meter type = " + strMeterType);
                }
                catch (TimeOutException e)
                {
                    throw (e);
                }
                catch (Exception e)
                {
                    Logger.TheInstance.WriteException(psem, e);
                }

                switch (strMeterType)
                {
                    case CENTRON_AMI.CENTRONAMI:
                        {
                            // OpenWay meters have the same model but can vary based on device class.
                            switch (strDeviceClass)
                            {
                                case CENTRON_AMI.ITRN_DEVICE_CLASS:
                                case CENTRON_AMI.ITR1_DEVICE_CLASS:
                                case CENTRON_AMI.ITRT_DEVICE_CLASS:
                                    {
                                        // This is a single phase, blurt based polyphase meter,
                                        // or pole top cell relay (RFLAN extender)
                                        NewDevice = new OpenWayITR1(psem);
                                        break;
                                    }
                                case CENTRON_AMI.ITR3_DEVICE_CLASS:
                                    {
                                        // This is a basic polyphase meter
                                        NewDevice = new OpenWayBasicPoly(psem);
                                        break;
                                    }
                                case CENTRON_AMI.ITR4_DEVICE_CLASS:
                                    {
                                        // This is an advanced polyphase meter.
                                        NewDevice = new OpenWayAdvPoly(psem);
                                        break;
                                    }
                                case CENTRON_AMI.ITRD_DEVICE_CLASS:
                                    {
                                        bool SecurityError = false;
                                        bool IsBridgeMeter = COpenWayITRDBridge.IsBridgeMeter(psem, out SecurityError);

                                        //We got a security error, so chances are this meter has signed authorization and we need to authenticate
                                        //before we can truly tell that this is a bridge meter.
                                        if (true == SecurityError)
                                        {
                                            CXMLOpenWaySystemSettings XMLOpenWaySettings = new CXMLOpenWaySystemSettings("");

                                            // We know it is at least an ITRD Single Phase meter
                                            NewDevice = new OpenWayITRD(psem);

                                            if (null != XMLOpenWaySettings && null != NewDevice
                                                && XMLOpenWaySettings.UseSignedAuthorization == true
                                                && ((OpenWayITRD)NewDevice).SignedAuthorizationState != null
                                                && ((OpenWayITRD)NewDevice).SignedAuthorizationState.Value != FeatureState.Disabled)
                                            {
                                                // Signed Authorization feels so right at this point, let's try it.
                                                ProcedureResultCodes ProcResult = ((OpenWayITRD)NewDevice).Authenticate(key);

                                                if (ProcedureResultCodes.COMPLETED == ProcResult)
                                                {
                                                    IsBridgeMeter = COpenWayITRDBridge.IsBridgeMeter(psem, out SecurityError);
                                                }
                                            }
                                        }

                                        if (IsBridgeMeter)
                                        {
                                            // ITRD Single Phase Bridge meter
                                            NewDevice = new COpenWayITRDBridge(psem);
                                        }
                                        else
                                        {
                                            // ITRD Single Phase meter
                                            NewDevice = new OpenWayITRD(psem);
                                        }
                                        break;
                                    }
                                case CENTRON_AMI.ITRE_DEVICE_CLASS:
                                    {
                                        // ITRE Basic Polyphase meter
                                        NewDevice = new OpenWayBasicPolyITRE(psem);
                                        break;
                                    }
                                case CENTRON_AMI.ITRF_DEVICE_CLASS:
                                    {
                                        bool SecurityError = false;
                                        bool IsBridgeMeter = COpenWayAdvPolyITRFBridge.IsBridgeMeter(psem, out SecurityError);

                                        //We got a security error, so chances are this meter has signed authorization and we need to authenticate
                                        //before we can truly tell that this is a bridge meter.
                                        if (true == SecurityError)
                                        {
                                            CXMLOpenWaySystemSettings XMLOpenWaySettings = new CXMLOpenWaySystemSettings("");

                                            // We know it is at least an ITRF Polyphase meter
                                            NewDevice = new OpenWayAdvPolyITRF(psem);

                                            if (null != XMLOpenWaySettings && null != NewDevice
                                                && XMLOpenWaySettings.UseSignedAuthorization == true
                                                && ((OpenWayAdvPolyITRF)NewDevice).SignedAuthorizationState != null
                                                && ((OpenWayAdvPolyITRF)NewDevice).SignedAuthorizationState.Value != FeatureState.Disabled)
                                            {
                                                // Signed Authorization feels so right at this point, let's try it.
                                                ProcedureResultCodes ProcResult = ((OpenWayAdvPolyITRF)NewDevice).Authenticate(key);

                                                if (ProcedureResultCodes.COMPLETED == ProcResult)
                                                {
                                                    IsBridgeMeter = COpenWayAdvPolyITRFBridge.IsBridgeMeter(psem, out SecurityError);
                                                }
                                            }
                                        }

                                        if (IsBridgeMeter)
                                        {
                                            // ITRF Polyphase Bridge meter
                                            NewDevice = new COpenWayAdvPolyITRFBridge(psem);
                                        }
                                        else
                                        {
                                            // ITRF Advanced Polyphase meter
                                            NewDevice = new OpenWayAdvPolyITRF(psem);
                                        }
                                        break;
                                    }
                                case CENTRON_AMI.ITRJ_DEVICE_CLASS:
                                    {
                                        // ITRJ Single Phase cellular meter
                                        NewDevice = new OpenWayITRJ(psem);
                                        break;
                                    }
                                case CENTRON_AMI.ITRK_DEVICE_CLASS:
                                    {
                                        // ITRK Polyphase cellular meter
                                        NewDevice = new OpenWayPolyITRK(psem);
                                        break;
                                    }
                                default:
                                    {
                                        NewDevice = null;
                                        break;
                                    }
                            }
                            break;
                        }
                    case AMI_GATEWAY:
                        {
                            // OpenWay gateways have the same model but can vary based on device class.
                            switch (strDeviceClass)
                            {
                                case CENTRON_AMI.ITRS_DEVICE_CLASS:
                                    {
                                        NewDevice = new SL7000_Gateway(psem);
                                        break;
                                    }
                                default:
                                    {
                                        // The only way this case would happen is if we are logged on 
                                        // to an M2 Gateway, log off, and then log back on before the
                                        // pass through command timeout occurs
                                        NewDevice = new M2_Gateway(psem);

                                        // Set the max wait time to the pass through timeout time
                                        // to make sure that a keep alive is sent in time to keep
                                        // the session going.
                                        psem.MaxWaitTime = M2_Gateway.M2_GATEWAY_MAX_WAIT;
                                        break;
                                    }
                            }

                            break;
                        }
                    case L_G_FOCUSAXD:
                    case L_G_FOCUSAXR:
                        {
                            psem.Logoff();
                            psem.Terminate();
                            psem = null;

                            // This is the Landis+Gyr meter.  Send the pass through command
                            LGPassThrough PassThru = new LGPassThrough(CommPort);
                            LGPassThrough.LGPassThruResponse LGResponse = LGPassThrough.LGPassThruResponse.LG_ERROR;

                            LGResponse = PassThru.SendPassThroughCmd();

                            if (LGResponse == LGPassThrough.LGPassThruResponse.LG_ACK)
                            {
                                // This is a Landis+Gyr meter with a Gateway module
                                PassThru = null;

                                // We need to re-do all the logon steps because we are
                                // now talking directly to the M2 Gateway board
                                psem = new CPSEM(CommPort);

                                // Set the max wait time to the pass through timeout time
                                // to make sure that a keep alive is sent in time to keep
                                // the session going.
                                psem.MaxWaitTime = M2_Gateway.M2_GATEWAY_MAX_WAIT;

                                PSEMResponse PSEMResult = PSEMResponse.Err;

                                PSEMResult = psem.Identify();

                                if (PSEMResponse.Ok == PSEMResult)
                                {
                                    PSEMResult = psem.Negotiate(DEFAULT_MAX_PACKET_LENGTH,
                                                            CPSEM.DEFAULT_MAX_NUMBER_OF_PACKETS,
                                                            (uint)38400);
                                    //(uint)SystemManagerSettings.BaudRate);
                                }

                                if (PSEMResponse.Ok == PSEMResult)
                                {
                                    PSEMResult = psem.Logon("", CPSEM.DEFAULT_HH_PRO_USER_ID);
                                }

                                if (PSEMResponse.Ok == PSEMResult)
                                {
                                    NewDevice = new M2_Gateway(psem);
                                }
                                else
                                {
                                    NewDevice = null;
                                }
                            }
                            else
                            {
                                NewDevice = null;
                            }

                            break;
                        }
                    case GE_I210_MODEL:
                    case GE_kV2_MODEL:
                        {
                            // Log off; set the identity byte to 0x80; and log on - we should get the comm module at that point
                            psem.Logoff();
                            psem.Terminate();
                            psem = null;

                            // We need to re-do all the logon steps because we are
                            // now talking directly to the ICS module
                            psem = new CPSEM(CommPort);
                            psem.IdentityByte = GE_IDENTITY_BYTE;

                            PSEMResponse PSEMResult = PSEMResponse.Err;

                            PSEMResult = psem.Identify();

                            if (PSEMResponse.Ok == PSEMResult)
                            {
                                PSEMResult = psem.Negotiate(DEFAULT_MAX_PACKET_LENGTH,
                                                        CPSEM.DEFAULT_MAX_NUMBER_OF_PACKETS,
                                                        (uint)38400);
                            }

                            if (PSEMResponse.Ok == PSEMResult)
                            {
                                PSEMResult = psem.Logon("", CPSEM.DEFAULT_HH_PRO_USER_ID);
                            }

                            if (PSEMResponse.Ok == PSEMResult)
                            {
                                try
                                {
                                    // Since we are now talking to the ICM we need to read Table 0 again
                                    // to retrieve the meters device type.
                                    CTable00 Table0 = new CTable00(psem);

                                    strDeviceClass = Table0.DeviceClass;

                                    switch (strDeviceClass)
                                    {
                                        case ICS_Gateway.ITRU_DEVICE_CLASS:
                                            NewDevice = new OpenWayITRU(psem);
                                            break;
                                        case ICS_Gateway.ITRV_DEVICE_CLASS:
                                            NewDevice = new OpenWayITRV(psem);
                                            break;
                                        default:
                                            NewDevice = new OpenWayITRH(psem);
                                            break;
                                    }
                                }
                                catch (TimeOutException e)
                                {
                                    throw (e);
                                }
                            }
                            

                            break;
                        }
                    case ICS_GATEWAY:
                        {
                            // OpenWay gateways have the same model but can vary based on device class.
                            switch (strDeviceClass)
                            {
                                case ICS_Gateway.ITRU_DEVICE_CLASS:
                                    NewDevice = new OpenWayITRU(psem);
                                    break;
                                case ICS_Gateway.ITRV_DEVICE_CLASS:
                                    NewDevice = new OpenWayITRV(psem);
                                    break;
                                default:
                                    Logger.TheInstance.WriteDetail(psem, "Unknown device class = " + strDeviceClass);
                                    NewDevice = null;
                                    break;
                            }

                            break;
                        }
                    default:
                        {
                            Logger.TheInstance.WriteDetail(psem, "Unknown meter type = " + strMeterType);

                            NewDevice = null;
                            break;
                        }
                }//switch                               
            }            

            return NewDevice;
        }

		/// <summary>
		/// Logs on the the meter
		/// </summary>
		/// <returns>A ItronDeviceResult</returns>
		/// <example><code>
		/// Communication comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// CANSIDevice ANSIDevice = new CANSIDevice(comm);
		/// ANSIDevice.Logon();
		/// </code></example>
		/// <remarks>Field-Pro does NOT use this function.  Field-Pro logs on
		/// using IdentifyMeter.IdentifyAndLogon.</remarks>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  07/13/05 mrj 7.13.00 N/A    Created
		//  08/07/06 AF  7.35.00 N/A	Added code for calling new Table01 constructor
		//  08/17/06 KRC 7.35.00 N/A    Added support for calling full logon sequence
		//  02/28/07 AF  8.00.14 2432   Added member variable to keep track of
		//                              logged on state
		//  02/02/09 RCG 2.10.04 N/A    Moved code to logon with baud rate.
        //  02/10/14 jrf 3.50.31 419257 Allowing code to choose PSEM object's baud rate if it is 
        //                              already set higher than the default.
		public override ItronDeviceResult Logon()
		{
            uint uiBaudRate = DEFAULT_BAUD_RATE;

            if (m_PSEM.BaudRate > DEFAULT_BAUD_RATE)
            {
                uiBaudRate = m_PSEM.BaudRate;
            }

            return Logon(uiBaudRate);
		}

		/// <summary>
		/// Logs on to the meter and negotiates to the specified Baud Rate.
		/// </summary>
		/// <param name="BaudRate">The baud rate to negotiate to.</param>
		/// <returns>The result of the logon</returns>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  02/02/09 RCG 2.10.04 N/A    Created
        //  02/10/14 jrf 3.50.31 419257 Modified to use DEFAULT_MAX_PACKET_LENGTH.
		public ItronDeviceResult Logon(uint BaudRate)
		{
			PSEMResponse PSEMResult;
			ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

			try
			{
				// Logon to the meter
				PSEMResult = m_PSEM.Identify();

				if (PSEMResponse.Isss == PSEMResult)
				{
					PSEMResult = m_PSEM.Identify();
				}
				if (PSEMResponse.Ok == PSEMResult)
				{
					PSEMResult = m_PSEM.Negotiate(DEFAULT_MAX_PACKET_LENGTH,
												MAX_NUMBER_OF_PACKETS,
												BaudRate);
				}
				if (PSEMResponse.Ok == PSEMResult)
				{
					PSEMResult = m_PSEM.Logon("", 2);
				}

				if (PSEMResponse.Ok != PSEMResult)
				{
					Result = ItronDeviceResult.ERROR;
				}
			}
			catch (Exception)
			{
				//If we cannot logon, then just return an error
				Result = ItronDeviceResult.ERROR;
			}

			return Result;
		}

		/// <summary>
		/// Logs off the meter
		/// </summary>
		/// <example><code>
		/// Communication comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// CANSIDevice ANSIDevice = new CANSIDevice(comm);
		/// ANSIDevice.Logon();
		/// ANSIDevice.Security( "strPassword" );
		/// ANSIDevice.Logoff();
		/// </code></example>
		/// 
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  07/13/05 mrj 7.13.00 N/A    Created
		//  06/22/06 jrf 7.30.00 N/A	Changed return type to void
		//  08/03/06 AF	 7.35.00 N/A	Added m_Table0
		//  02/28/07 AF  8.00.14 2432   Added member variable to keep track of
		//                              logged on state.  If you call m_PSEM.Logoff()
		//                              when you are already logged off, it causes an
		//                              unnecessary delay.
		//  10/23/09 RCG 2.30.14        Removed logged on state variable as it causes problems
		//                              with Signed Authorization.
		public override void Logoff()
		{
			try
			{
				m_PSEM.Logoff();
			}
			catch (Exception)
			{
				//If we cannot logoff, then the meter is in the wrong state or
				//has timed out.  In any case we will let the meter recover on
				//it's own.
			}

            try
            {
                m_PSEM.Terminate();
            }
            catch (Exception)
            {
                //If we cannot terminate, then the meter is in the wrong state or
                //has timed out.  In any case we will let the meter recover on
                //it's own.
            }

            //Destroy the tables
            m_Table0 = null;
			m_Table1 = null;
			m_Table3 = null;
			m_Table5 = null;
			m_Table2048 = null;
			m_MeterKeyTable = null;
            m_Table2084 = null;
		}

        /// <summary>
        /// Calls the PSEM terminate command. Use this for times that the meter has timed out
        /// and we want to get back to base state.  It will reset the baud rate to 9600 and the
        /// meter won't try to commit changes that were pending.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/07/16 AF  4.50.243 WR 622562  Created
        //
        public void Terminate()
        {
            try
            {
                m_PSEM.Terminate();
            }
            catch (Exception)
            {
                //If we cannot logoff, then the meter is in the wrong state or
                //has timed out.  In any case we will let the meter recover on
                //it's own.
            }

            //Destroy the tables
            m_Table0 = null;
            m_Table1 = null;
            m_Table3 = null;
            m_Table5 = null;
            m_Table2048 = null;
            m_MeterKeyTable = null;
            m_Table2084 = null;
        }

        /// <summary>
        /// Issues the PSEM security command
        /// </summary>
        /// <param name="Passwords">A list of passwords to be issued to the 
        /// meter. An empty string should be supplied if a null password is 
        /// to be attempted.</param>
        /// <param name="ShowSecurityCode">A boolean to make security codes
        /// accessible in log files during testing</param>
        /// <returns>A ItronDeviceResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version    Issue# Description
        //  -------- --- -------    ------ ---------------------------------------
        //  07/27/05 mrj 7.13.00    N/A    Created
        //  08/07/06 AF  7.35.00    N/A 	Added code for calling new Table01 constructor
        //  08/21/06 mrj 7.35.00    N/A    Changed to take a list of passwords.
        //  08/29/06 mrj 7.35.00           Store off the current security code
        //  09/13/06 mrj 7.35.00           Do not log the security codes
        //  10/06/16 AF  4.70.21    718340 Added a try/catch/finally to make sure that logging 
        //                                 is turned back on even if the code throws an exception
        //  02/17/17 SB 1.2017.2.47 744643 Added conditions to allow security codes to be available in 
        //                                 comm logs during testing 
        public ItronDeviceResult Security(List<string> Passwords, bool ShowSecurityCode = false)
		{
			PSEMResponse PSEMResult = PSEMResponse.Err;
			ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

			for (int iIndex = 0; iIndex < Passwords.Count && PSEMResponse.Ok != PSEMResult; iIndex++)
			{
				//Issue the password
				string strPassword = Passwords[iIndex];

                Logger.LoggingState CurrentState = m_Logger.LoggerState;


                if (ShowSecurityCode == false)
                {
                    //Do not show the security code in the log file 
                    m_Logger.LoggerState = Logger.LoggingState.PROTOCOL_SENDS_SUSPENDED;
                }

                try
                {
                    //Issue Security to the meter
                    PSEMResult = m_PSEM.Security(strPassword);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if(ShowSecurityCode == false)
                    {
                        //Resume logging
                        m_Logger.LoggerState = CurrentState;
                    }

                }

				if (PSEMResponse.Ok == PSEMResult)
				{
                    //Store off the current security code
                    byte[] bytesPassword = new System.Text.ASCIIEncoding().GetBytes(strPassword);
                    SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);
                    DataStorage.StoreSecureData(SecureDataStorage.OPTICAL_PASSWORD, bytesPassword);
                }
			}

			if (PSEMResponse.Ok != PSEMResult)
			{
				Result = ItronDeviceResult.SECURITY_ERROR;
			}

			return Result;
		}

        /// <summary>
        /// Reissues the PSEM security command
        /// </summary>
        /// <returns>A ItronDeviceResult</returns>
        public ItronDeviceResult ReissueSecurity()
        {
            PSEMResponse PSEMResult = PSEMResponse.Err;
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            byte[] BytePassword = null;

            SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);
            BytePassword = DataStorage.RetrieveSecureData(SecureDataStorage.OPTICAL_PASSWORD);

            Logger.LoggingState CurrentState = m_Logger.LoggerState;


            m_Logger.LoggerState = Logger.LoggingState.PROTOCOL_SENDS_SUSPENDED;

            try
            {
                //Issue Security to the meter
                PSEMResult = m_PSEM.Security(new System.Text.ASCIIEncoding().GetString(BytePassword));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                m_Logger.LoggerState = CurrentState;

            }

            if (PSEMResponse.Ok != PSEMResult)
            {
                Result = ItronDeviceResult.SECURITY_ERROR;
            }

            return Result;
        }

        /// <summary>
        /// Issues the PSEM security command
        /// </summary>
        /// <param name="Passwords">A list of passwords to be issued to the 
        /// meter. An empty string should be supplied if a null password is 
        /// to be attempted.</param>
        /// <param name="ShowSecurityCode">A boolean to make security codes
        /// accessible in log files during testing</param>
        /// <returns>A ItronDeviceResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version    Issue# Description
        //  -------- --- -------    ------ ---------------------------------------
        //  05/16/14 MDP                   Overloaded method to send in password as a byte[]. mostly for GE i210
        //  10/06/16 AF  4.70.21    718340 Added a try/catch/finally to make sure that logging 
        //                                 is turned back on even if the code throws an exception
        //  02/17/17 SB 1.2017.2.47 744643 Added conditions to allow security codes to be available in 
        //                                 comm logs during testing 
        public ItronDeviceResult Security(List<byte[]> Passwords, bool ShowSecurityCode = false)
        {
            PSEMResponse PSEMResult = PSEMResponse.Err;
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            for (int iIndex = 0; iIndex < Passwords.Count && PSEMResponse.Ok != PSEMResult; iIndex++)
            {
                //Issue the password
                byte[] strPassword = Passwords[iIndex];


                Logger.LoggingState CurrentState = m_Logger.LoggerState;


                if (ShowSecurityCode == false)
                {
                    //Do not show the security code in the log file 
                    m_Logger.LoggerState = Logger.LoggingState.PROTOCOL_SENDS_SUSPENDED;
                }
                try
                {
                    //Issue Security to the meter
                    PSEMResult = m_PSEM.Security(strPassword);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (ShowSecurityCode == false)
                    {
                        //Resume logging
                        m_Logger.LoggerState = CurrentState;
                    }
                }

                if (PSEMResponse.Ok == PSEMResult)
                {
                    //Store off the current security code
                    SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);
                    DataStorage.StoreSecureData(SecureDataStorage.OPTICAL_PASSWORD, strPassword);
                }
            }

            if (PSEMResponse.Ok != PSEMResult)
            {
                Result = ItronDeviceResult.SECURITY_ERROR;
            }

            return Result;
        }

		/// <summary>
		/// Clears the history log (std table 74)
		/// </summary>
		/// <returns>ItronDeviceResult</returns>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  10/13/06 AF  7.40.00 N/A    Created
		//  10/11/10 RCG 2.45.03 160144 Fixing parameter length		
		public virtual ItronDeviceResult ClearEventLog()
		{
			ItronDeviceResult Result = ItronDeviceResult.ERROR;
			byte[] ProcResponse;
			ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

			//Execute the standard procedure 04 - reset list pointers procedure
			byte[] byParameter = new byte[] {(byte)ResetListPointerTypes.HistoryLogger};
			ProcResult = ExecuteProcedure(Procedures.RESET_LIST_PTRS, byParameter, out ProcResponse);

			switch (ProcResult)
			{
				case ProcedureResultCodes.COMPLETED:
					{
						//Success
						Result = ItronDeviceResult.SUCCESS;
						// Refresh the Event Table so we will see the new count now that it has been cleared.
						Table74.Refresh();
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
						// Refresh the Event Table so we will see if the count has changed.
						Table74.Refresh();
						break;
					}
			}

			return Result;
		}

        /// <summary>
        /// Clears the standard status bits in Std Table 3
        /// </summary>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/20/13 jrf 2.70.68 288152 Created
        //
        public ProcedureResultCodes ClearStandardStatus()
        {
            byte[] ProcResponse;
            byte[] byParameter = new byte[0];
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            ProcResult = ExecuteProcedure(Procedures.CLEAR_STD_STATUS, byParameter, out ProcResponse);

            return ProcResult;
        }
        
        /// <summary>
		/// Clears the manufacturer's status bits in Std Table 3
		/// </summary>
		/// <returns>The result of the procedure call</returns>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  01/05/10 RCG 2.40.00 N/A    Created

		public ProcedureResultCodes ClearManufacturerStatus()
		{
			byte[] ProcResponse;
			byte[] byParameter = new byte[0];
			ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
			
			ProcResult = ExecuteProcedure(Procedures.CLEAR_MFG_STATUS, byParameter, out ProcResponse);

			return ProcResult;
		}

		/// <summary>
		/// Performs a clock adjust on the connected meter
		/// </summary>
		/// <param name="iOffset">The offset from meter time (seconds)</param>
		/// <returns>A ClockAdjustResult</returns>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  06/01/06 mrj 7.30.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - promoted from CENTRON_AMI
		// 
		public virtual ClockAdjustResult AdjustClock(int iOffset)
		{
			ClockAdjustResult Result = ClockAdjustResult.ERROR;
			DateTime MeterTime;
			DateTime ReferenceDate = new DateTime(1970, 1, 1);
			TimeSpan Span = new TimeSpan();
			uint MeterMinutes;
			byte[] byMinutes;
			byte MeterSeconds;
			byte[] byParameters = new byte[SET_TIME_DATE_PROC_SIZE];
			int iIndex = 0;
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
					byParameters[iIndex++] = SET_TIME_DATE_MASK;

					//Minutes since 1/1/1970
					byMinutes = BitConverter.GetBytes(MeterMinutes);
					Array.Copy(byMinutes, 0, byParameters, iIndex, byMinutes.Length);
					iIndex += byMinutes.Length;

					//Add the seconds
					byParameters[iIndex++] = MeterSeconds;

					//Last byte is always 0 (Time_Date_Qual)
					byParameters[iIndex] = 0;

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
		/// The PasswordReconfigResult reconfigures passwords.  Requires primary 
		/// password level access.  This method resets all passwords to null and 
		/// then sets up to 4 passwords starting with the primary password.
		/// </summary>
		/// <param name="Passwords">A list of passwords to write to the meter. 
		/// The Primary password should be listed first followed by the secondary
		/// password and so on.  Use empty strings for null passwords.  Passwords
		/// will be truncated or null filled as needed to fit in the device.</param>
		/// <returns>A PasswordReconfigResult object</returns>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/21/06 mcm 7.35.00 N/A    Created
		/// 09/06/06 mcm 7.35.00 N/A    Default to C12.19 implementation for derived classes
        /// 11/13/13 AF  3.50.03        Moved definition from ItronDevice
		///	
		public virtual PasswordReconfigResult ReconfigurePasswords(
							System.Collections.Generic.List<string> Passwords)
		{
			return STDReconfigurePasswords(Passwords);
		}

		/// <summary>
		/// Reconfigures the tertiary password.
		/// </summary>
		/// <param name="strTertiaryPassword">The password to reconfigure.</param>
		/// <returns>A the result of the reconfigure.</returns>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 02/22/07 RCG 8.00.13    N/A Created
		public virtual PasswordReconfigResult ReconfigureTertiaryPassword(string strTertiaryPassword)
		{
			return STDReconfigureTertiaryPassword(strTertiaryPassword);
		}

		/// <summary>
		/// Sends a PSEM wait command. Note that there is a timer runing that
		/// should handle this in most cases, but in some scenarios like when
		/// a window is slow to populate, the timer is late.  It's thread is
		/// probably starving.
		/// </summary>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/02/07 mcm 8.00.23 2743	The KeepAlive timer is firing late
		public void SendWait()
		{
			m_PSEM.Wait(CPSEM.MAX_WAIT_TIME);
		}

		/// <summary>
		/// Convert a meter time from the meter to local time for the device
		/// </summary>
		/// <param name="MeterTime">Time from the meter</param>
		/// <returns>Convertered Device Local Time</returns>
		public virtual DateTime GetLocalDeviceTime(DateTime MeterTime)
		{
			return MeterTime;
		}

		/// <summary>
		/// Retrieves the events that occurred within a specified time period
		/// </summary>
		/// <param name="startDate">No events older than this date should be retrieved</param>
		/// <param name="endDate">No events newer than this date should be retrieved</param>
		/// <returns>A list of event objects</returns>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//  09/28/09 AF  2.30.04        Created for CRF files
		//
		public List<HistoryEntry> GetEvents(DateTime startDate, DateTime endDate)
		{
			List<HistoryEntry> EventsList = Events;
			List<HistoryEntry> EventListByDate = new List<HistoryEntry>();

			foreach (HistoryEntry histEntry in EventsList)
			{
				if ((histEntry.HistoryTime >= startDate) && (histEntry.HistoryTime <= endDate))
				{
					EventListByDate.Add(histEntry);
				}
			}

			return EventListByDate;
		}

        /// <summary>
        /// Gets whether or not the specified procedure is used by the meter
        /// </summary>
        /// <param name="procedureID">The ID of the procedure to check</param>
        /// <returns>True if the procedure is used. False otherwise</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/12 RCG 2.53.41 N/A    Created
        public bool IsProcedureUsed(ushort procedureID)
        {
            bool ProcedureUsed = false;

            if (Table00 != null)
            {
                ProcedureUsed = Table00.IsProcedureUsed(procedureID);
            }

            return ProcedureUsed;
        }

        /// <summary>
        /// Checks standard table 00 to see if the specified table is supported
        /// </summary>
        /// <param name="usTableId">identifier of the table we want to know about</param>
        /// <returns>true if the table is listed in table 00; false, otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/17/12 AF  2.60.45 201071 Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
        public bool IsTableUsed(ushort usTableId)
        {
            bool tableUsed = false;

            if (Table00.IsTableUsed(usTableId))
            {
                tableUsed = true;
            }

            return tableUsed;
        }

        /// <summary>
        /// Resets the RFLAN processor on an OpenWay CENTRON meter.
        /// </summary>
        /// <returns>
        /// An ItronDeviceResult representing the result of the reset
        /// operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/07 RCG 8.10.04		Created 
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
        public ItronDeviceResult ResetRFLAN()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(9);
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);
            byte[] ProcResponse;

            ParamWriter.Write(RESET_RF_FUNC_CODE);
            ParamWriter.Write(RESET_RF_PARAM_1);
            ParamWriter.Write(RESET_RF_PARAM_2);

            ProcResult = ExecuteProcedure(Procedures.RESET_RF_LAN,
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
        /// Reset the RFMesh module
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  07/27/15 AF  4.20.18  WR 597249  Created
        //
        public ItronDeviceResult ResetRFMesh()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            MemoryStream ProcParam = new MemoryStream(2);
            PSEMBinaryWriter BinaryWriter = new PSEMBinaryWriter(ProcParam);
            byte[] ProcResponse;

            BinaryWriter.Write((byte)IPDiagnosticsFunction159ID.IP_AND_COMM_RESET_TEST_FUNCTION);
            BinaryWriter.Write((byte)IPResetActionaID.IP_RESET_COMM_MODULE_RESTART);

            ProcResult = ExecuteProcedureForBoron((ushort)Procedures.IP_DIAGNOSTICS_159, ProcParam.ToArray(), out ProcResponse);

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
        /// Activate the firmware download table
        /// </summary>
        /// <param name="TableID">Pending table id of the table to be activated</param>
        /// <param name="FWType">Firmware type</param>
        /// <param name="Version">Version of the firmware</param>
        /// <param name="Revision">Revision of the firmware</param>
        /// <param name="Build">Build of the firmware</param>
        /// <param name="Patch">Pad byte of the firmware header (can be used to store the patch value)</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/13 AF  3.50.02 TQ9508,9514 Created for ITRU and ITRV devices
        //  11/15/13 AF  3.50.04	    Class re-architecture - moved from CENTRON_AMI
        //  02/21/14 AF  3.50.36 WR460307 Add date to the activate parameter list and the patch
        //
        public ProcedureResultCodes ActivateFWDLTable(ushort TableID, byte FWType, byte Version, byte Revision, byte Build, byte Patch)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[11];
            Array.Copy(BitConverter.GetBytes(TableID), ProcParam, 2);
            ProcParam[2] = FWType;
            ProcParam[3] = Version;
            ProcParam[4] = Revision;
            ProcParam[5] = Build;

            DateTime ReferenceDate = new DateTime(2000, 1, 1);
            TimeSpan Span = DateTime.Now - ReferenceDate;

            Array.Copy(BitConverter.GetBytes((uint)Span.TotalSeconds), 0, ProcParam, 6, 4);
            ProcParam[10] = Patch;

            ProcResult = ExecuteProcedure(Procedures.ACTIVATE_FWDL_PENDING_TABLE, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Builds a PendingEventRecord object for a Non Time Activated pending event
        /// with the specified parameters.
        /// </summary>
        /// <param name="bSelfRead">Whether or not a Self Read should be performed on activation.</param>
        /// <param name="bDemandReset">Whether or not a Demand Reset should be performed on activation.</param>
        /// <param name="byMfgEventCode">The manufacturer event code for the pending event.</param>
        /// <param name="eCode">The event code for the pending event.</param>
        /// <returns>The EventRecord object.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/07 RCG 8.10.07        Created
        //  02/05/09 AF  -.--.00        Changed the access modifier from protected to public 
        //                              for automated firmware testing project
        //  11/15/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
        public PendingEventRecord BuildPendingEventRecord(bool bSelfRead, bool bDemandReset,
            byte byMfgEventCode, PendingEventRecord.PendingEventCode eCode)
        {
            PendingEventRecord EventRecord = new PendingEventRecord();
            ASCIIEncoding AE = new ASCIIEncoding();
            byte[] abyManufacturer = new byte[5];

            // Set up the event selector bitfield
            EventRecord.EventCode = eCode;
            EventRecord.PerformSelfRead = bSelfRead;
            EventRecord.PerformDemandReset = bDemandReset;

            // Copy the manufacturer ID to the device storage
            AE.GetBytes(MANUFACTURER).CopyTo(abyManufacturer, 0);

            // Copy the manufaturer event code
            abyManufacturer[abyManufacturer.Length - 1] = byMfgEventCode;
            EventRecord.EventStorage = abyManufacturer;

            return EventRecord;
        }

        /// <summary>
        /// Builds a PendingEventRecord object for a Relative Time Trigger
        /// with the specified parameters.  For use with automated tests
        /// </summary>
        /// <param name="bSelfRead">Whether or not a Self Read should be performed on activation.</param>
        /// <param name="bDemandReset">Whether or not a Demand Reset should be performed on activation.</param>
        /// <param name="byMfgEventCode">The manufacturer event code for the pending event.</param>
        /// <param name="timeSpan">timespan to wait before activating</param>
        /// <returns>The EventRecord object.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/02/12 JKW         Created
        //  11/15/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
        public PendingEventRecord BuildPendingEventRecord(bool bSelfRead, bool bDemandReset,
            byte byMfgEventCode, TimeSpan timeSpan)
        {
            PendingEventRecord EventRecord = new PendingEventRecord();
            ASCIIEncoding AE = new ASCIIEncoding();
            byte[] abyManufacturer = new byte[5];

            // Set up the event selector bitfield
            EventRecord.EventCode = PendingEventRecord.PendingEventCode.RelativeTimeTrigger;
            EventRecord.PerformSelfRead = bSelfRead;
            EventRecord.PerformDemandReset = bDemandReset;
            EventRecord.ActivationTime = timeSpan;

            // Copy the manufacturer ID to the device storage
            AE.GetBytes(MANUFACTURER).CopyTo(abyManufacturer, 0);

            // Copy the manufaturer event code
            abyManufacturer[abyManufacturer.Length - 1] = byMfgEventCode;
            EventRecord.EventStorage = abyManufacturer;

            return EventRecord;
        }

        /// <summary>
        /// The Registration service is used to add and maintain (“keep-alive”)
        /// routing information of C12.22 Relays. To be part of C12.22 Network,
        /// a node shall send a registration service to one of the C12.22 Relays.
        /// This procedure, typically used on the local port (ANSI C12.18), is 
        /// used to initiate this process
        /// </summary>
        /// <returns>ProcedureResultCodes</returns>
        public ProcedureResultCodes Register()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;

            //Send the Std procedure to register the node
            ProcParam = new byte[0];
            ProcResult = ExecuteProcedure(Procedures.REGISTER, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// The Registration service is used to add and maintain (“keep-alive”)
        /// routing information of C12.22 Relays. To be part of C12.22 Network,
        /// a node shall send a registration service to one of the C12.22 Relays.
        /// This procedure, typically used on the local port (ANSI C12.18), is 
        /// used to initiate this process
        /// </summary>
        /// <returns>ProcedureResultCodes</returns>
        public ProcedureResultCodes RegisterAndWaitForResult()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;

            //Send the Std procedure to register the node
            ProcParam = new byte[0];
            ProcResult = ExecuteProcedureAndWaitForResult(Procedures.REGISTER, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// The deregistration service is used to remove routing information in
        /// C12.22 Relays. This procedure, typically used on the local port 
        /// (ANSI C12.18), is used to initiate this process
        /// </summary>
        /// <returns></returns>
        public ProcedureResultCodes Deregister()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;

            //Send the Std procedure to deregister the node
            ProcParam = new byte[0];
            ProcResult = ExecuteProcedure(Procedures.DEREGISTER, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// The deregistration service is used to remove routing information in
        /// C12.22 Relays. This procedure, typically used on the local port 
        /// (ANSI C12.18), is used to initiate this process
        /// </summary>
        /// <returns></returns>
        public ProcedureResultCodes DeregisterAndWaitForResult()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;

            //Send the Std procedure to deregister the node
            ProcParam = new byte[0];
            ProcResult = ExecuteProcedureAndWaitForResult(Procedures.DEREGISTER, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// The decommission HAN (Zigbee) network procedure can bring down and 
        /// reform the HAN network or can remove a single node from the network
        /// </summary>
        /// <param name="eType">
        /// Type of decommission to perform
        /// </param>
        /// <param name="ulMACAddress"></param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/14/06 AF  8.00.00 SCR 50 Created
        //  02/06/07 AF  8.00.10        Reversed the order of the bytes in the MAC address.
        //                              It has to be in MSB order here.
        //  05/29/08 AF  1.50.28        Renamed the procedure enumeration since it is for 
        //                              more than decommissioning
        //
        public ProcedureResultCodes DecommissionHANNetwork(DecommissionType eType, UInt64 ulMACAddress)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;
            byte[] abyTemp;

            try
            {
                ProcParam = new byte[9];

                abyTemp = BitConverter.GetBytes(ulMACAddress);
                Array.Reverse(abyTemp);
                Array.Copy(abyTemp, 0, ProcParam, 1, 8);
                ProcParam[0] = (byte)eType;
                ProcResult = ExecuteProcedure(Procedures.HAN_PROCEDURE, ProcParam, out ProcResponse);
            }
            catch (Exception e)
            {
                //Log it and pass it up
                m_Logger.WriteException(this, e);
            }

            return ProcResult;
        }

        /// <summary>
        /// This procedure controls whether or not the HAN allows new devices to join
        /// </summary>
        /// <param name="byWhen">
        /// How long to allow joining:
        /// 0x00 - No joining allowed
        /// 0x01 - 0xFE - Minutes to Allow Joining
        /// 0xFF - Allow until turned off
        /// </param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/29/08 AF  1.50.29        Created
        //
        public ProcedureResultCodes JoinControl(byte byWhen)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;

            try
            {
                ProcParam = new byte[9];
                ProcParam[0] = JOIN_CONTROL_REQUEST_TYPE;
                ProcParam[1] = byWhen;
                ProcResult = ExecuteProcedure(Procedures.HAN_PROCEDURE, ProcParam, out ProcResponse);
            }
            catch (Exception e)
            {
                //Log it and pass it up
                m_Logger.WriteException(this, e);
            }

            return ProcResult;
        }

        /// <summary>
        /// This procedure controls whether or not the HAN allows new device to join
        /// </summary>
        /// <param name="minutes">The number of minutes to enable HAN joining</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  09/26/12 RCG 2.70.22 TC11283 Created

        public ProcedureResultCodes ExtendedJoinControl(ushort minutes)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;

            try
            {
                ProcParam = new byte[9];

                ProcParam[0] = JOIN_CONTROL_REQUEST_TYPE;
                ProcParam[1] = (byte)1; // Needs to be between 1 and 254

                // The extended join control parameter is big endian
                ProcParam[2] = (byte)(minutes >> 8);
                ProcParam[3] = (byte)minutes;

                ProcResult = ExecuteProcedure(Procedures.HAN_PROCEDURE, ProcParam, out ProcResponse);
            }
            catch (Exception e)
            {
                //Log it and pass it up
                m_Logger.WriteException(this, e);
            }

            return ProcResult;
        }

        /// <summary>
        /// This procedure tells the electric meter to send activation information 
        /// to the HAN device (gas module) after firmware download
        /// </summary>
        /// <param name="strFWFilePath">The firmware file to activate.  Passed in so
        /// that we can calculate the CRC32 value for it.</param>
        /// <param name="ActivationTime">The time at which the firmware should be
        /// activated in the HAN devices</param>
        /// <param name="bActivateNow">whether or not to activate the firmware immediately
        /// on download</param>
        /// <returns>The result returned by the meter</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/15/09 AF  2.20.02        Created for gas module f/w download
        //
        public ProcedureResultCodes ActivateFWOnHANClients(String strFWFilePath, DateTime ActivationTime, bool bActivateNow)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;
            bool bSuccess;
            UInt32 uiCRC32;
            byte[] abyTemp;

            try
            {
                ProcParam = new byte[9];
                ProcParam[0] = ACTIVATE_HAN_FW_REQUEST_TYPE;

                if (bActivateNow)
                {
                    // Setting the time to 0 tells the gas module to activate
                    // the firmware immediately
                    abyTemp = BitConverter.GetBytes(0);
                }
                else
                {
                    DateTime ReferenceDate = new DateTime(1970, 1, 1);
                    TimeSpan Span = ActivationTime - ReferenceDate;

                    abyTemp = BitConverter.GetBytes((uint)Span.TotalSeconds);
                }
                Array.Copy(abyTemp, 0, ProcParam, 1, 4);

                bSuccess = CRC.CalculateFirmwareCRCForHanActivation(strFWFilePath, out uiCRC32);
                if (bSuccess)
                {
                    abyTemp = BitConverter.GetBytes(uiCRC32);
                    Array.Copy(abyTemp, 0, ProcParam, 5, 4);

                    ProcResult = ExecuteProcedure(Procedures.HAN_PROCEDURE, ProcParam, out ProcResponse);
                }
            }
            catch (Exception e)
            {
                //Log it and pass it up
                m_Logger.WriteException(this, e);
            }

            return ProcResult;
        }

        /// <summary>
        /// This procedure forces the HAN to switch to the new keys designated in
        /// the HAN security table (MFG table 2105)
        /// </summary>
        /// <param name="AlternativeKey">The alternative key to activate</param>
        /// <returns></returns>
        /// <remarks>
        /// WARNING!!!  This procedure has NOT been tested!!!!
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/09/08 AF  1.50.33        Created
        //
        public ProcedureResultCodes ActivateAlternativeKey(byte[] AlternativeKey)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;

            try
            {
                ProcParam = new byte[17];
                ProcParam[0] = ACTIVATE_ALTERNATIVE_KEY_REQUEST_TYPE;

                Array.Copy(AlternativeKey, 0, ProcParam, 1, 16);

                ProcResult = ExecuteProcedure(Procedures.HAN_PROCEDURE, ProcParam, out ProcResponse);
            }
            catch (Exception e)
            {
                //Log it and pass it up
                m_Logger.WriteException(this, e);
            }

            return ProcResult;
        }

        /// <summary>
        /// The set HAN multiplier and divisor procedure allows the setting of 
        /// the multiplier and divisor that are used on primary quantities.
        /// </summary>
        /// <param name="uiMultiplier">The HAN multiplier.</param>
        /// <param name="uiDivisor">The HAN divisor.</param>
        /// <returns>ProcedureResultCodes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 jrf 2.20.03        Created
        //
        public ProcedureResultCodes SetHANMultiplierAndDivisor(UInt32 uiMultiplier,
                                                    UInt32 uiDivisor)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;
            byte[] abyTemp;

            try
            {
                ProcParam = new byte[9];
                ProcParam[0] = SET_HAN_MULTIPLIER_AND_DIVISOR_REQUEST_TYPE;

                abyTemp = BitConverter.GetBytes(uiMultiplier);
                Array.Copy(abyTemp, 0, ProcParam, 1, 4);

                abyTemp = BitConverter.GetBytes(uiDivisor);
                Array.Copy(abyTemp, 0, ProcParam, 5, 4);

                ProcResult = ExecuteProcedure(Procedures.HAN_PROCEDURE, ProcParam, out ProcResponse);
            }
            catch (Exception e)
            {
                //Log it and pass it up
                m_Logger.WriteException(this, e);
            }

            return ProcResult;
        }

        /// <summary>
        /// The Enable/Disable HAN procedure starts or stops the ZigBee network
        /// </summary>
        /// <param name="byStartStop">either the value 0xFF which will force a 
        /// decommission then disable the Zigbee network, or 0x00 will force a 
        /// decommission then enable the Zigbee network.</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 AF  2.20.03        Created
        //  11/17/09 AF  2.30.17 144270 The procedure to disable now takes a time parameter.
        //                              We will use 60 minutes.
        //  12/11/09 AF  2.30.27 146533 The firmware has changed and is interpreting the
        //                              Minutes to Disable as a big endian value so we
        //                              had to reverse the order of the bytes.
        //
        public ProcedureResultCodes EnableDisableHAN(byte byStartStop)
        {
            return EnableDisableHAN(byStartStop, MINUTES_TO_DISABLE);
        }

        /// <summary>
        /// The Enable/Disable HAN procedure starts or stops the ZigBee network
        /// </summary>
        /// <param name="byStartStop">either the value 0xFF which will force a 
        /// decommission then disable the Zigbee network, or 0x00 will force a 
        /// decommission then enable the Zigbee network.</param>
        /// <param name="byMinutesToDisable">The number of minutes before the Zigbee 
        /// network will automatically re-enable</param>
        /// <returns>The result returned by the meter</returns>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 AF  2.20.03        Created
        //  11/17/09 AF  2.30.17 144270 The procedure to disable now takes a time parameter.
        //                              We will use 60 minutes.
        //  12/11/09 AF  2.30.27 146533 The firmware has changed and is interpreting the
        //                              Minutes to Disable as a big endian value so we
        //                              had to reverse the order of the bytes.
        //
        public ProcedureResultCodes EnableDisableHAN(byte byStartStop, byte byMinutesToDisable)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.DEVICE_SETUP_CONFLICT;
            byte[] ProcParam;
            byte[] ProcResponse;

            try
            {
                ProcParam = new byte[9];
                ProcParam.Initialize();
                PSEMBinaryWriter PSEMWriter = new PSEMBinaryWriter(new MemoryStream(ProcParam));
                PSEMWriter.Write(ENABLE_DISABLE_HAN_REQUEST_TYPE);
                PSEMWriter.Write(byStartStop);
                // The following code just changes the endian-ness of 60.
                PSEMWriter.Write(ZERO);
                PSEMWriter.Write(byMinutesToDisable);

                ProcResult = ExecuteProcedure(Procedures.HAN_PROCEDURE, ProcParam, out ProcResponse);
            }
            catch (Exception e)
            {
                //Log it and pass it up
                m_Logger.WriteException(this, e);
            }

            return ProcResult;
        }

        /// <summary>
        /// This method will either take down the ZigBee network permanently or reenable permanently
        /// depending on the bool parameter passed in.  Pass true to enable; false, to disable.
        /// </summary>
        /// <param name="blnEnable">tells whether the ZigBee network should be disabled or enabled.</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/23/13 AF  2.71.02 WR417928 Created
        //  07/24/13 AF  2.71.02 WR417928 Added missing result per code review
        //
        public ItronDeviceResult EnableDisableZigBeePermanently(bool blnEnable)
        {
            PSEMResponse Result = PSEMResponse.Err;
            ItronDeviceResult ConfigResult = ItronDeviceResult.ERROR;

            Result = Table2193.EnableDisableZigBeePermanently(blnEnable);

            if (Result == PSEMResponse.Ok)
            {
                ConfigResult = ItronDeviceResult.SUCCESS;
            }
            else if (Result == PSEMResponse.Isc)
            {
                ConfigResult = ItronDeviceResult.SECURITY_ERROR;
            }
            else if (Result == PSEMResponse.Onp)
            {
                ConfigResult = ItronDeviceResult.UNSUPPORTED_OPERATION;
            }
            else
            {
                ConfigResult = ItronDeviceResult.ERROR;
            }

            return ConfigResult;
        }

        /// <summary>
        /// This method will reconfigure the config tag to a new value. Original intention is to 
        /// update the value so CE will flag change and update the meter's configuration.
        /// </summary>
        /// <param name="configTag">the value to set</param>
        /// <returns></returns>
        public ItronDeviceResult ConfigureConfigTag(string configTag)
        {
            PSEMResponse Result = PSEMResponse.Err;
            ItronDeviceResult ConfigResult = ItronDeviceResult.ERROR;

            Result = Table06.ConfigureMiscID(configTag);

            if (Result == PSEMResponse.Ok)
            {
                ConfigResult = ItronDeviceResult.SUCCESS;
            }
            else if (Result == PSEMResponse.Isc)
            {
                ConfigResult = ItronDeviceResult.SECURITY_ERROR;
            }
            else if (Result == PSEMResponse.Onp)
            {
                ConfigResult = ItronDeviceResult.UNSUPPORTED_OPERATION;
            }
            else
            {
                ConfigResult = ItronDeviceResult.ERROR;
            }

            return ConfigResult;
        }

        /// <summary>
        /// This method will return whether an error is currently set to be ignored
        /// </summary>
        /// <param name="error">string containing the error in question.</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  10/20/16 CFB 4.70.27 WR682857 Created
        //  12/01/16 CFB 4.70.40 WR682857 Corrected Logic in if statements
        public bool IsNonFatalErrorIgnored(string error)
        {
            byte[] NonFatalErrorStatus = NonFatalScrollLockBits;
            if (error == m_rmStrings.GetString("NON_FATAL_1"))
            {
                if (((NonFatalErrorStatus[0] | NonFatalErrorStatus[1]) & NON_FATAL_1_MASK) == 0)
                {
                    return true;
                }
            }
            else if (error == m_rmStrings.GetString("NON_FATAL_2"))
            {
                if (((NonFatalErrorStatus[0] | NonFatalErrorStatus[1]) & NON_FATAL_2_MASK) == 0)
                {
                    return true;
                }
            }
            else if (error == m_rmStrings.GetString("NON_FATAL_3"))
            {
                if (((NonFatalErrorStatus[0] | NonFatalErrorStatus[1]) & NON_FATAL_3_MASK) == 0)
                {
                    return true;
                }
            }
            else if (error == m_rmStrings.GetString("NON_FATAL_4"))
            {
                if (((NonFatalErrorStatus[0] | NonFatalErrorStatus[1]) & NON_FATAL_4_MASK) == 0)
                {
                    return true;
                }
            }
            else if (error == m_rmStrings.GetString("NON_FATAL_5"))
            {
                if (((NonFatalErrorStatus[0] | NonFatalErrorStatus[1]) & NON_FATAL_5_MASK) == 0)
                {
                    return true;
                }
            }
            else if (error == m_rmStrings.GetString("NON_FATAL_6"))
            {
                if (((NonFatalErrorStatus[0] | NonFatalErrorStatus[1]) & NON_FATAL_6_MASK) == 0)
                {
                    return true;
                }
            }
            else if (error == m_rmStrings.GetString("NON_FATAL_9"))
            {
                if (((NonFatalErrorStatus[2] | NonFatalErrorStatus[3]) & NON_FATAL_9_MASK) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Boolean that indicates if any Communication Module is present in the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 KRC  2.00.00			Created
        // 07/24/09 jrf  2.20.16 137694 Restricting to ITR2 and ITRL 
        //                              comm modules.
        // 07/30/09 jrf  2.20.19 137693 Moved checks for certain comm modules to 
        //                              ItronCommModulePresent property.
        // 09/16/09 AF   2.30.01        Changed the Compare method's comparison
        //                              type to quiet a compiler warning
        // 01/14/10 jrf  2.40.06        Adding code to catch exception thrown when meters 
        //                              with no comm module are queried and fail.
        //  04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  11/27/17 AF  4.73.00 Task 469275 For SBR meters, CommModule is not null but it
        //                              does not have a comm module. Us the comm module id to distinguish
        //
        public virtual bool CommModulePresent
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
                        bResult = CommModule != null;
                        // If we have a third party or missing comm module, the comm module id will throw an exception
                        string strCommModuleID = CommModule.CommModuleIdentification;
                    }
                }
                catch (Exception)
                {
                    bResult = false;
                }

                return bResult;
            }
        }

		#endregion Public Methods

		#region Public Properties

        /// <summary>
        /// Gets whether or not the meter is currently logged on via ZigBee
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ------------------------------------------------------------
        //  10/19/12 RCG 2.70.31        Created for Ameren demo
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //
        public bool IsLoggedOnViaZigBee
        {
            get
            {
                return m_PSEM.m_CommPort is Itron.Metering.Zigbee.Radio;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently logged on via ZigBee using a Belt Clip Radio
        /// or Telegesis dongle
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //	10/15/14 AF  4.00.73  WR 230745  Added to give the firmware download handler the ability
        //                                   to determine if we are using a Belt Clip Radio or a
        //                                   Telegesis dongle to communicate via ZigBee
        //
        public bool IsLoggedOnViaBeltClipRadio
        {
            get
            {
                return m_PSEM.m_CommPort is BeltClipRadio;
            }
        }

        /// <summary>
        /// Gets the communication port that is currently in use
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //	10/15/14 AF  4.00.73  WR 230745  Add to give the firmware download handler the ability
        //                                   to determine which comm port is in use
        //
        public ICommunications CommPort
        {
            get
            {
                return m_PSEM.m_CommPort;
            }
            set
            {
                m_PSEM.m_CommPort = value;
            }
        }

        /// <summary>
        /// WARNING! This property will allow you to change the PSEM baud rate for a session.
        /// Use it only when the meter has timed out, will not communicate, and you want to reset
        /// the baud rate to the default of 9600.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/16/16 AF  4.50.268 WR 622562  Created to allow resetting the baud rate on timeouts
        //
        public uint BaudRate
        {
            get
            {
                return m_PSEM.BaudRate;
            }
            set
            {
                m_PSEM.BaudRate = value;
            }
        }

		/// <summary>
		/// Property to get the hardware version.revision from table 01. The hardware
		/// version is specific to ANSI meters; SCS meters do not
		/// need this item.
		/// </summary>
		// Revision History
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------------
		// 08/03/06 AF  7.35.00	    Created
		public float HWRevision
		{
			get
			{
				//Get the hardware version.revision out of table 01
				return Table01.HW_Rev;
			}
		}

        /// <summary>
        /// Property to get just the hardware revision from table 01. The hardware
        /// version is specific to ANSI meters; SCS meters do not
        /// need this item.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 03/31/11 jrf 2.50.00	    Created
        //
        public byte HWRevisionOnly
        {
            get
            {
                return Table01.HWRevisionOnly;
            }
        }

		/// <summary>
		/// Property to get the hardware version from Table 01. Does not retrieve
		/// the hardware revision.
		/// </summary>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//  02/04/10 AF  2.40.12        Created
		//
		public byte HWVersionOnly
		{
			get
			{
				return Table01.HWVersionOnly;
			}
		}

		/// <summary>
		/// This basically returns the lower nibble of the hardware version.
		/// We can use this to compare the true hardware version of the meter.
		/// </summary>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//  02/08/10 AF  2.40.12        Created
		//
		public byte HWVersionNibble
		{
			get
			{
				return (byte)(Table01.HWVersionOnly & HW_UPPER_NIBBLE_MASK);
			}
		}

        /// <summary>
        /// Gets the HW Revision with the Prism Lite bit filtered out.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/01/10 RCG 2.40.31	    Created
        // 12/12/13 AF  3.50.14         Class re-architecture - promoted from CENTRON_AMI which now has an override
        public virtual float HWRevisionFiltered
        {
            get
            {
                return HWRevision;
            }
        }

		/// <summary>
		/// Gets the Number of Valid Event Entries
		/// </summary>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  09/20/06 KRC  7.35.00			Created 
		//  
		public UInt16 NumberOfValidEventEntries
		{
			get
			{
				return Table74.NumberValidEntries;
			}
		}

		/// <summary>
		/// Gets the Last Entry Sequence Number
		/// </summary>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  09/20/06 KRC  7.35.00			Created 
		//  
		public UInt32 LastEntrySequenceNumber
		{
			get
			{
				return Table74.LastEntrySequenceNumber;
			}
		}

		/// <summary>
		/// Gets the Number of Valid Event Entries
		/// </summary>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  09/20/06 KRC  7.35.00			Created 
        //  11/06/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876 Made virtual so we can override the property.
        //
        public virtual List<HistoryEntry> Events
        {
            get
            {
                return Table74.HistoryLogEntries;
            }
        }

		/// <summary>
		/// Gets the event log events from table 76.
		/// </summary>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/12/10 jrf 2.41.01	    Created 
		//
		public List<HistoryEntry> EventLogEvents
		{
			get
			{
				return Table76.EventLogEntries;
			}
		}

		/// <summary>
		/// Retrieves the history log configuration from the meter.  The list
		/// includes all possible supported events with a description and a boolean
		/// indicating whether or not the event is enabled in the meter
		/// </summary>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 10/31/06 AF  7.40.00 N/A    Created
        // 11/06/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876 Made virtual so we can override the property.
        //
        public virtual List<MFG2048EventItem> HistoryLogEventList
		{
			get
			{
				return Table2048.HistoryLogConfig.HistoryLogEventList;
			}
		}    

		/// <summary>
		/// Retrieves the history log configuration from the meter.  The list
		/// includes all possible supported events with a description and a boolean
		/// indicating whether or not the event is monitored in the meter.  This version
		/// reads the config from standard tables 72 and 73.
		/// </summary>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//  05/20/10 AF  2.41.04        Created
		//
		public List<MFG2048EventItem> StdTbl73HistoryLogEventList
		{
			get
			{
				return Table73.HistoryLogEventList;
			}
		}

        /// <summary>
        /// Retrieves the history log configuration from the meter.  The list
        /// includes all possible monitored events with a description and a boolean
        /// indicating whether or not the event is monitored in the meter.  This version
        /// reads the config from standard tables 72 and 73.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/10 AF  2.41.04        Created
        //
        public List<MFG2048EventItem> StdTbl73HistoryLogEventListMonitored
        {
            get
            {
                return Table73.HistoryLogMonitoredEventList;
            }
        }

        /// <summary>
        /// Retrieves the event log configuration from the meter.  The list
        /// includes all possible monitored events with a description and a boolean
        /// indicating whether or not the event is monitored in the meter.  This version
        /// reads the config from standard tables 75.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/10 AF  2.41.04        Created
        //
        public List<MFG2048EventItem> StdTbl75EventLogEventListMonitored
        {
            get
            {
                return Table75.EventLogMonitoredEventList;
            }
        }

		/// <summary>
		/// Property used to get the unit ID (string) from the meter
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/18/06 mrj 7.30.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
		// 
		public virtual string UnitID
		{
			get
			{
				//Get the meter ID out of Table 05
				string strMeterID = Table05.MeterID;

				//Trim the end, need to specifically remove nulls
				char[] trim = new char[2] { ' ', '\0' };
				strMeterID = strMeterID.TrimEnd(trim);

				return strMeterID;
			}
		}

		/// <summary>
		/// Gets the device time
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/23/06 mrj 7.30.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - removed override
		//  01/02/14 mah 3.50.18        Changed from a LID implementation to use table 52
		public virtual DateTime DeviceTime
		{
			get
			{
                if (Table52 != null)
                {
                    return Table52.CurrentTime;
                }
                else  // We have to return a date so use the minimum value to indicate a glaring error
                {
                    return DateTime.MinValue;
                }
			}
		}

		/// <summary>
		/// Gets the firmware revision
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/23/06 mrj 7.30.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
		//  
		public virtual float FWRevision
		{
			get
			{
				return Table01.FW_Rev;
			}
		}

        /// <summary>
        /// Gets the firmware version only from table 01
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/18/15 AF  4.50.222 RTT 587962 Created
        //
        public virtual byte FWVersionOnly
        {
            get
            {
                return Table01.FWVersionOnly;
            }
        }

        /// <summary>
        /// Gets the firmware revision only from table 01
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/18/15 AF  4.50.222 RTT 587962 Created
        //
        public virtual byte FWRevisionOnly
        {
            get
            {
                return Table01.FWRevisionOnly;
            }
        }

        /// <summary>
        /// Gets the firmware revision. This is the same as "FWRevision" but was created for the 
        /// purpose of using as a parameter to pass in on table creation. Some Tables are different
        /// sizes in different firmware versions. This should only be used as a parameter to pass
        /// in when creating a table. Reference table 2129 in SL7000 device class for example.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/16/13 DLG 3.50.16          Created.
        //  
        public virtual float FWRevisionForTableCreation
        {
            get
            {
                return FWRevision;
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
        //  12/18/13 DLG 3.50.16          Created.
        //  
        public virtual float HWRevisionForTableCreation
        {
            get
            {
                return HWRevision;
            }
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
		//  10/11/06 KRC 7.36.00 N/A    Created
        //  04/15/14 AF  3.50.78 WR489415 Made virtual for override in the SL7000, which
        //                                doesn't support LID requests
		// 
		public virtual uint FirmwareBuild
		{
			get
			{
				if (false == m_uiFWBuild.Cached)
				{
					byte[] Data = null;
					PSEMResponse Result = m_lidRetriever.RetrieveLID(m_LID.FIRMWARE_BUILD, out Data);

					if (PSEMResponse.Ok == Result)
					{
						m_uiFWBuild.Value = (uint)Data[0];
					}
					else
					{
						throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
													"Error reading Firmware Build"));
					}
				}

				return m_uiFWBuild.Value;
			}
		}

        /// <summary>
        /// Gets the firmware build fresh from the meter each time.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/12 jrf 2.53.38 TC7107 Created
        // 
        public uint FirmwareBuildUncached
        {
            get
            {
                uint uiFWBuild = 0;
                byte[] Data = null;
                PSEMResponse Result = m_lidRetriever.RetrieveLID(m_LID.FIRMWARE_BUILD, out Data);

                if (PSEMResponse.Ok == Result)
                {
                    uiFWBuild = (uint)Data[0];
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                                "Error reading Firmware Build"));
                }

                return uiFWBuild;
            }
        }

		/// <summary>
		/// Gets the errors list.
		/// </summary>	
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/24/06 mrj 7.30.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
		// 	
		public virtual string[] ErrorsList
		{
			get
			{
				string[] strErrorList = new string[MAX_NUM_ERRORS];
				PSEMResponse Result = PSEMResponse.Ok;
				int iErrorCount = 0;
				byte[] Data = null;
				LID[] lids = new LID[3];

				//Create the array of LIDs that we want to get
				lids[0] = m_LID.STATEMON_NON_FATAL_ERRORS;
				lids[1] = m_LID.STATEMON_NON_FATAL_ERRORS2;
				lids[2] = m_LID.STATEMON_FATAL_ERRORS;

				//Get the value from the LIDs	
				Result = m_lidRetriever.RetrieveMulitpleLIDs(lids, out Data);
				if (PSEMResponse.Ok == Result)
				{
					//Read the data to determine if errors are set
					GetErrors(ErrorType.NON_FATAL_1, Data[0], ref strErrorList, ref iErrorCount);

					//Read the data to determine if errors are set
					GetErrors(ErrorType.NON_FATAL_2, Data[1], ref strErrorList, ref iErrorCount);

					//Read the data to determine if errors are set
					GetErrors(ErrorType.FATAL, Data[2], ref strErrorList, ref iErrorCount);
				}
				else
				{
					throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
							"Error reading device errors"));
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
        /// Gets the non-fatal errors list via read of LID values.
        /// </summary>	
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/18/13 jrf 2.70.74 288152 Created
        // 	
        public string[] LIDNonFatalErrorsList
        {
            get
            {
                string[] strErrorList = new string[MAX_NUM_ERRORS];
                PSEMResponse Result = PSEMResponse.Ok;
                int iErrorCount = 0;
                byte[] Data = null;
                LID[] lids = new LID[2];

                //Create the array of LIDs that we want to get
                lids[0] = m_LID.STATEMON_NON_FATAL_ERRORS;
                lids[1] = m_LID.STATEMON_NON_FATAL_ERRORS2;

                //Get the value from the LIDs	
                Result = m_lidRetriever.RetrieveMulitpleLIDs(lids, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    //Read the data to determine if errors are set
                    GetErrors(ErrorType.NON_FATAL_1, Data[0], ref strErrorList, ref iErrorCount);

                    //Read the data to determine if errors are set
                    GetErrors(ErrorType.NON_FATAL_2, Data[1], ref strErrorList, ref iErrorCount);
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading device errors"));
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
        /// Gets the non-fatal and fatal errors list via read of std. table 3.
        /// </summary>	
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/18/13 jrf 2.70.74 288152 Created
        // 	05/18/16 AF  4.50.269 234815 Mark the table unloaded so ensure it will be read
        public string[] StdTable3ErrorsList
        {
            get
            {
                string[] strErrorList = null;
                List<string> lstErrors = new List<string>();

                if (null != Table03)
                {
                    Table03.State = AnsiTable.TableState.Unloaded;
                    lstErrors = new List<string>(Table03.ErrorsList);
                }

                strErrorList = lstErrors.ToArray();

                return strErrorList;
            }
        }
        
        /// <summary>
        /// Gets the non-fatal errors list via read of std. table 3.
        /// </summary>	
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/18/13 jrf 2.70.74 288152 Created
        // 	
        public string[] StdTable3NonFatalErrorsList
        {
            get
            {
                string[] strErrorList = null;
                List<string> lstErrors = new List<string>();

                if (null != Table03)
                {
                    lstErrors = new List<string>(Table03.NonFatalErrorsList);
                }

                strErrorList = lstErrors.ToArray();

                return strErrorList;
            }
        }
        
        /// <summary>
		/// Gets the serial number out of the constants block of 2048 since
		/// table 6 was not supported in pre-Saturn meters.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/22/06 mrj 7.30.00 N/A    Created
		//  07/24/06 mrj 7.30.35 16     Get serial number from 2048 instead of
		// 								MFG serial number in table 01.
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
		// 		
		public virtual string SerialNumber
		{
			get
			{
				//Get the serial number out of 2048
				string strSerialNumber = Table2048.ConstantsConfig.CustomerSerialNumber;

				//Trim the end, need to specifically remove nulls
				char[] trim = new char[2] { ' ', '\0' };
				strSerialNumber = strSerialNumber.TrimEnd(trim);

				return strSerialNumber;
			}
		}

		/// <summary>
		/// Get the mfg serial number out of table 1.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  02/21/07 mrj 8.00.13		Created		
		// 		
		public string MFGSerialNumber
		{
			get
			{
				//Get the serial number out of Table 01
				string strSerialNumber = Table01.SerialNumber;

				//Trim the front and end, need to specifically remove nulls
				char[] trim = new char[2] { ' ', '\0' };
				strSerialNumber = strSerialNumber.TrimStart(trim);
				strSerialNumber = strSerialNumber.TrimEnd(trim);

				return strSerialNumber;
			}
		}

		/// <summary>
		/// Property used to get the Device Class of the meter.
		/// This is what used to be called Manufacturer in Table00.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY who Version Issue#          Description
		//  -------- --- ------- -------------- ---------------------------------------------
		//  07/07/08 KRC 1.51.02 itron00117020   Adding Device Class
		// 
		public string DeviceClass
		{
			get
			{
				return Table00.DeviceClass;
			}
		}

		/// <summary>
		/// Property used to get the meter type (string).  Use
		/// this property for meter determination and comparison.  
		/// This property should not be confused with MeterName which
		/// is used to obtain a human readable name of the meter.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  04/25/06 mrj 7.30.00 N/A    Created
        //  03/26/13 AF  2.80.12 TR7641 Added support for I210c gateways
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
		// 	
		public virtual string MeterType
		{
			get
			{
				string strMeterType = "";

				try
				{
					string strMFG = Table01.Manufacturer;
					strMFG = strMFG.TrimEnd();

					if (MFG_SCH == strMFG.ToUpper(CultureInfo.InvariantCulture) ||
						MFG_ITRN == strMFG.ToUpper(CultureInfo.InvariantCulture) ||
                        MFG_ICS == strMFG.ToUpper(CultureInfo.InvariantCulture))
					{
						//Get the meter type from table 1
						strMeterType = Table01.Model;
						strMeterType = strMeterType.ToUpper(CultureInfo.InvariantCulture);
					}
				}
				catch (TimeOutException e)
				{
					throw (e);
				}
				catch
				{
					strMeterType = DefaultMeterType;
				}

				return strMeterType;
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
        // 11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
		//
		public virtual string MeterName
		{
			get
			{
				return "";
			}
		}

        /// <summary>
        /// Gets the Meter Name that is to be used in the activity log. This name is limited to 25 characters.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved from ItronDevice

        public virtual string ActivityLogMeterName
        {
            get
            {
                return MeterName;
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
		public virtual bool IsMeterInDST
		{
			get
			{
				PSEMResponse Result = PSEMResponse.Ok;
				byte[] Data = null;

				if (!m_MeterInDST.Cached)
				{
					Result = m_lidRetriever.RetrieveLID(m_LID.METER_IN_DST, out Data);
					if (PSEMResponse.Ok == Result)
					{
						if (1 == Data[0])
						{
							m_MeterInDST.Value = true;
						}
						else
						{
							m_MeterInDST.Value = false;
						}

					}
					else
					{
						throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
								"Error reading is meter in DST"));
					}
				}

				return m_MeterInDST.Value;
			}
		}
        /// <summary>
        /// Gets whether DST is configured in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/07/11 AF  2.51.21 174848 Created
        //
        public bool DSTConfigured
        {
            get
            {
                if (m_bDSTEnabled.Cached == false)
                {
                    // try to get the value from 2048
                    m_bDSTEnabled.Value = Table2048.HasDST;
                }

                return m_bDSTEnabled.Value;
            }
        }

		/// <summary>
		/// Gets the dst enabled flag
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		// 								Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
		// 		
		public virtual bool DSTEnabled
		{
			get
			{
				PSEMResponse Response = PSEMResponse.Err;
				object objValue;

				if (m_bDSTEnabled.Cached == false)
				{
					Response = m_lidRetriever.RetrieveLID(m_LID.DST_CONFIGURED, out objValue);

					if (Response == PSEMResponse.Ok)
					{
						if ((byte)objValue == 1)
						{
							m_bDSTEnabled.Value = true;
						}
						else
						{
							m_bDSTEnabled.Value = false;
						}
					}
					else
					{
						// try to get the value from 2048
						m_bDSTEnabled.Value = Table2048.HasDST;
					}
				}

				return m_bDSTEnabled.Value;
			}
		}

        /// <summary>
		/// Determines if the meter has any active Fatal Errors
		/// </summary>		
		// Revision History
		// MM/DD/YY who Version Issue#          Description
		// -------- --- ------- -------------- ---------------------------------------------
		// 07/14/08 KRC 1.51.04 itron00117404   Check for Fatal Error
		//
		public virtual bool IsFatalErrorPresent
		{
			get
			{
				string[] strErrorList = new string[MAX_NUM_ERRORS];
				PSEMResponse Result = PSEMResponse.Ok;
				int iErrorCount = 0;
				byte[] Data = null;
				bool bFatalErrorPresent = false;

				//Get the value from the LIDs	
				Result = m_lidRetriever.RetrieveLID(m_LID.STATEMON_FATAL_ERRORS, out Data);
				if (PSEMResponse.Ok == Result)
				{
					//Read the data to determine if Fatal errors are set
					GetErrors(ErrorType.FATAL, Data[0], ref strErrorList, ref iErrorCount);

					if (strErrorList.Length != 0)
					{
						if (strErrorList[0] != null)
						{
							bFatalErrorPresent = true;
						}
					}
				}
				else
				{
					throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
							"Error reading device errors"));
				}

				return bFatalErrorPresent;
			}
		}

		/// <summary>
		/// Gets the clock running lid
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  07/02/06 mrj 7.30.00 N/A    Created
		//  03/13/07 mcm 8.00.18 2454   Made public and moved to public properties
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
		public virtual bool ClockRunning
		{
			get
			{
				PSEMResponse Result = PSEMResponse.Ok;
				if (!m_ClockRunning.Cached)
				{
					byte[] Data = null;

					//Get the clock running flag
					Result = m_lidRetriever.RetrieveLID(m_LID.CLOCK_RUNNING, out Data);
					if (PSEMResponse.Ok == Result)
					{
						if (1 == Data[0])
						{
							m_ClockRunning.Value = true;
						}
						else
						{
							m_ClockRunning.Value = false;
						}
					}
					else
					{
						throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
								"Error reading Clock Running"));
					}
				}

				return m_ClockRunning.Value;
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
		//  03/09/07 KRC 8.00.17		Created 
		//  04/03/07 AF  8.00.24 2675   Added description for SITESCAN_ERROR 
		//                              and removed the one for SITESCAN_OR_PENDING_TABLE_CLEAR,
		//                              which is really just the pending table cleared event.
		//  04/04/07 AF  8.00.24 2675   Changed the description for LOSS_VOLTAGE_A -- removed
		//                              phase A part
		//  05/22/07 mcm 8.10.05        Changed from method to property and used
		//                              new dictionary class
        //  12/17/15 AF  4.23.00 559019 Set the time format for use in interpreting the argument data for the time changed event
        //  12/29/15 AF  4.23.01 559019 Use the table 0 time format field instead of psem's
        //
		public virtual ANSIEventDictionary EventDescriptions
		{
			get
			{
				if (null == m_dicEventDescriptions)
				{
					m_dicEventDescriptions = new ANSIEventDictionary();
                    m_dicEventDescriptions.TimeFormat = Table00.TimeFormat;
				}

				return m_dicEventDescriptions;
			}
		}

        /// <summary>
        /// Gets whether the meter supports network tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/08/13 jrf 2.70.66 288156 Created
        //
        public bool NetworkingSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(120) && true == Table00.IsTableUsed(121)
                    && true == Table00.IsTableUsed(122) && true == Table00.IsTableUsed(123)
                    && true == Table00.IsTableUsed(125) && true == Table00.IsTableUsed(126)
                    && true == Table00.IsTableUsed(127));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets whether the meter supports History Log.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/08/13 jrf 2.70.66 288156 Created
        //  11/15/13 DLG 3.50.04 TR9520, 7876 Made property virtual so that we can override in ICS_Gateway.
        //
        public virtual bool HistoryLogSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(71) && true == Table00.IsTableUsed(72)
                    && true == Table00.IsTableUsed(73) && true == Table00.IsTableUsed(74));

                return blnSupported;
            }
        }

        /// <summary>
        /// Returns the string that represents the Comm module device class
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/29/09 AF  2.20.19 137695 Created to be able to distinguish ITRL devices
        //  05/25/10 RCG 2.41.04 155141 Fixing error that could cause problems with HW 1.5 meters
        //  06/03/10 AF  2.41.06        Made virtual so that M2 Gateway can override
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
        public virtual string CommModuleDeviceClass
        {
            get
            {
                string strDeviceClass = "";

                if (VersionChecker.CompareTo(HWRevision, HW_VERSION_1_5) <= 0)
                {
                    // A bug in the RFLAN FW for HW 1.5 and earlier meters causes the read of 2064 to fail
                    // We can safely assume that all HW 1.5 meters have an RFLAN module so just return ITR2
                    strDeviceClass = CommModuleBase.ITR2_DEVICE_CLASS;
                }
                else if (Table2064 != null)
                {
                    strDeviceClass = Table2064.DeviceClass;
                }

                return strDeviceClass;
            }
        }

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
        // 12/11/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //
        public virtual bool ItronCommModulePresent
        {
            get
            {
                bool bResult = false;

                try
                {
                    bResult = CommModule != null && (CommModule is RFLANCommModule || CommModule is PLANCommModule || CommModule is CiscoCommModule || CommModule is ICSCommModule);
                }
                catch
                {
                    bResult = false;
                }

                return bResult;
            }
        }

        /// <summary>
        /// Gets the amount of time the meter will adjust while in DST 
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 	10/18/10 RCG 2.45.06		Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
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
        /// Gets a TimeSpan object that represents the Time Zone Offset
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/06 RCG 7.40.00 N/A    Created
        // 11/14/13 AF  3.50.03	       Class re-architecture - promoted from CENTRON_AMI
        //
        public TimeSpan TimeZoneOffset
        {
            get
            {
                return Table53.TimeZoneOffset;
            }
        }

        /// <summary>
        /// Gets the Comm Module object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/19/10 RCG 2.40.15 N/A    Created

        public CommModuleBase CommModule
        {
            get
            {
                if (m_CommModule == null)
                {
                    // Create the Comm Module
                    m_CommModule = CommModuleBase.CreateCommModule(m_PSEM, this);
                }

                return m_CommModule;
            }
        }

        /// <summary>
        /// Gets the Std table 52 IsDSTApplied value
        /// </summary>
        /// <returns>bool</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??                          Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  05/26/15 AF  4.20.08 585297 Made public to be available to adjust clock subcontrol
        //
        public bool IsDSTApplied
        {
            get
            {
                return Table52.IsDSTApplied;
            }
        }

        /// <summary>
        /// The tables reported as supported from std. table 0.
        /// </summary>
        /// <returns>An enumerator of the table numbers.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Issue# Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 01/05/16 jrf 4.50.?? RQ 587913 Created.
        public IEnumerable<ushort> SupportedTables
        {
            get
            {
                List<ushort> SupportedTables = new List<ushort>();
                ushort MaxTableID = 0;

                try
                {
                    //Get Standard Table support
                    MaxTableID = (ushort)(Table00.DimStdTablesUsed * 8);

                    for (ushort i = 0; i < MaxTableID; i++)
                    {
                        if (Table00.IsTableUsed(i) == true)
                        {
                            SupportedTables.Add(i);
                        }
                    }

                    //Get Manufacturer Table support
                    MaxTableID = (ushort)((Table00.DimMfgTablesUsed * 8));

                    SendWait();

                    for (ushort i = 0; i < MaxTableID; i++)
                    {
                        if (Table00.IsTableUsed((ushort)(i + 2048)) == true)
                        {
                            SupportedTables.Add((ushort)(i + 2048));
                        }
                    }

                }
                catch { }

                return SupportedTables;
            }
        }

        /// <summary>
        /// Returns the device's PSEM object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  08/15/16 AF  4.60.03  WR 707390  Created
        //
        public CPSEM PSEM
        {
            get
            {
                return m_PSEM;
            }
        }

        /// <summary>
        /// Returns a byte array containing Scroll and Lock Bits for NonFatal errors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  11/03/16 CFB 4.70.29  WR 682857  Created
        //
        public byte[] NonFatalScrollLockBits
        {
            get
            {
                return Table2048.DisplayConfig.NonFatalScrollLockBits();
            }
        }

        /// <summary>
        /// Get the config tag from the meter. The config tag is the misc id!
        /// </summary>
        public string ConfigTag
        {
            get
            {
                string TheConfigTag = null;

                if (null != Table06)
                {
                    TheConfigTag = Table06.MiscID;
                }

                return TheConfigTag;
            }
        }

        #endregion Properties

        #region Internal Methods

        /// <summary>
        /// Handles the reading of Display Items that can not be read using ReadDisplayData
        /// </summary>
        /// <param name="Item">The item to read.</param>
        /// <returns>True if the item was handled, false otherwise.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/24/07 RCG 8.00.09 N/A    Created
        //
        internal virtual bool HandleIrregularRead(ANSIDisplayItem Item)
		{
			bool bResult = false;
			uint uiLIDValue = Item.DisplayLID.lidValue;
			uint uiBaseLIDValue = uiLIDValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK;

			if (0x00000000 == uiLIDValue && Item.DisplayFormat == 0x0000)
			{
				//This display item is not programmed, so dont' attempt to read it.
				// CT Ratio is LID 0x00000000 so we need to check the format as well
				Item.Value = "";
				bResult = true;
			}

			if (false == bResult)
			{
				switch (uiBaseLIDValue)
				{
					case (uint)DefinedLIDs.BaseLIDs.MISC_DATA:
						{
							uint uiMiscData = uiLIDValue & (uint)DefinedLIDs.Misc_Data.MISC_DATA_MASK;
							if ((uint)DefinedLIDs.Misc_Data.MISC_SEGMENT_TEST == uiMiscData)
							{
								// This is a segment test and cannot be retrieved from the meter
								Item.Value = "888888";
								bResult = true;
							}
							break;
						}
					case (uint)DefinedLIDs.BaseLIDs.CALENDAR_DATA:
						{
							uint uiCalData = uiLIDValue & (uint)DefinedLIDs.Calendar_Data.CLD_MASK;

							if ((uint)DefinedLIDs.Calendar_Data.CLD_EXPIRE_DATE == uiCalData)
							{
								byte[] byData;
								MemoryStream LIDStream;
								BinaryReader LIDBReader;
								DateTime dtExpDate = MeterReferenceTime;
								uint uiJunk;
								ushort usYears;

								// The result of this LID request returns 3 2 byte values we only need
								// the last value which contains the number of years
								m_lidRetriever.RetrieveLID(Item.DisplayLID, out byData);
								LIDStream = new MemoryStream(byData);
								LIDBReader = new BinaryReader(LIDStream);

								uiJunk = LIDBReader.ReadUInt32();
								usYears = LIDBReader.ReadUInt16();

								dtExpDate = dtExpDate.AddYears((int)usYears);

								Item.FormatDateTime(dtExpDate);

								bResult = true;
							}

							break;
						}
				}
			}

			return bResult;
		}

		/// <summary>
		/// Sets the Description of Display Items that can not be retrieved or are incorrect
		/// when using ConvertLidToQuantity
		/// </summary>
		/// <param name="Item">The item to handle</param>
		/// <returns>True if the item was handled, false otherwise</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/24/07 RCG 8.00.09 N/A    Created
        //
		internal virtual bool HandleIrregularDescription(ANSIDisplayItem Item)
		{
			bool bResult = false;
			uint uiLIDValue = Item.DisplayLID.lidValue;
			uint uiBaseLIDValue = uiLIDValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK;

			if (0x00000000 == uiLIDValue && Item.DisplayFormat == 0x0000)
			{
				//This display item is not programmed, so just return blank.
				// CT Ratio is LID 0x00000000 so we need to check the format as well
				Item.Description = "";
				bResult = true;
			}

			if (TypeCode.Empty == Item.DisplayLID.lidType)
			{
				// If the TypeCode is Empty then we did not find the LID.  Set the Description so we can make
				//  the user aware of the unknown LID.
				Item.Description = "Unknown Item: " + Item.DisplayLID.lidValue.ToString(CultureInfo.CurrentCulture);
			}

			if (false == bResult)
			{
				switch (uiBaseLIDValue)
				{
					case (uint)DefinedLIDs.BaseLIDs.MISC_DATA:
						{
							uint uiMiscData = uiLIDValue & (uint)DefinedLIDs.Misc_Data.MISC_DATA_MASK;
							if ((uint)DefinedLIDs.Misc_Data.MISC_SEGMENT_TEST == uiMiscData)
							{
								// This is a segment test and cannot be retrieved from the meter
								Item.Description = "Segment Test";
								bResult = true;
							}
							break;
						}
				}
			}

			if (false == bResult)
			{
				//  We have not found anything yet, so now check for special data types.
				if (TypeCode.DateTime == Item.DisplayLID.lidType)
				{
					string strDescription = "";
					string strReplacing = m_rmStrings.GetString("DATE_AND_TIME_OF");

					strDescription = Item.DisplayLID.lidDescription;

					// If we have a Time of Ocurrance Quantity we need to change the description
					// based on the display format.

					if (strDescription.StartsWith(strReplacing, StringComparison.Ordinal))
					{
						ushort usType = (ushort)(Item.DisplayFormat & ANSIDisplayItem.TypeMask);

						switch (usType)
						{
							case (ushort)ANSIDisplayItem.DisplayType.TIME_HH_MM_SS:
								{
									strDescription = strDescription.Replace(strReplacing, m_rmStrings.GetString("TIME_OF"));
									break;
								}
							case (ushort)ANSIDisplayItem.DisplayType.DATE_DD_MM_YY:
							case (ushort)ANSIDisplayItem.DisplayType.DATE_MM_DD_YY:
							case (ushort)ANSIDisplayItem.DisplayType.DATE_YY_MM_DD:
								{
									strDescription = strDescription.Replace(strReplacing, m_rmStrings.GetString("DATE_OF"));
									break;
								}
						}
					}

					// We need to do the same for items that end in "Date and Time"

					strReplacing = m_rmStrings.GetString("DATE_AND_TIME");

					if (strDescription.EndsWith(strReplacing, StringComparison.Ordinal))
					{
						ushort usType = (ushort)(Item.DisplayFormat & ANSIDisplayItem.TypeMask);

						switch (usType)
						{
							case (ushort)ANSIDisplayItem.DisplayType.TIME_HH_MM_SS:
								{
									strDescription = strDescription.Replace(strReplacing, m_rmStrings.GetString("TIME"));
									break;
								}
							case (ushort)ANSIDisplayItem.DisplayType.DATE_DD_MM_YY:
							case (ushort)ANSIDisplayItem.DisplayType.DATE_MM_DD_YY:
							case (ushort)ANSIDisplayItem.DisplayType.DATE_YY_MM_DD:
								{
									strDescription = strDescription.Replace(strReplacing, m_rmStrings.GetString("DATE"));
									break;
								}
						}
					}

					Item.Description = strDescription;

					bResult = true;
				}
			}

			return bResult;
		}

		/// <summary>
		/// Formats irregular Display Item data that is not correctly handled when calling FormatData
		/// </summary>
		/// <param name="Item">The Display Item for the data</param>
		/// <param name="objData">The data to be formatted</param>
		/// <returns>True if the item was handled, false otherwise</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/24/07 RCG 8.00.09 N/A    Created
		//  03/14/07 RCG 8.00.18 2569   Adding check to for the ReferenceDate to make sure
		//                              the displayed dates match the display
        //  12/04/14 jrf 4.00.90 549051 Correcting spelling of level in comm field display item.
		internal virtual bool HandleIrregularFormatting(ANSIDisplayItem Item, object objData)
		{
			bool bResult = false;
			uint uiLIDValue = Item.DisplayLID.lidValue;
			uint uiBaseLIDValue = uiLIDValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK;
            DefinedLIDs LIDValues = new DefinedLIDs();

			if (0x00000000 == uiLIDValue && Item.DisplayFormat == 0x0000)
			{
				//This display item is not programmed, so just return blank.
				// CT Ratio is LID 0x00000000 so we need to check the format as well
				Item.Value = "";
				bResult = true;
			}

            //Correct spelling of level since display cannot use v's
            if (uiLIDValue == LIDValues.DISP_VENDOR_FIELD_1.lidValue
                || uiLIDValue == LIDValues.DISP_VENDOR_FIELD_2.lidValue
                || uiLIDValue == LIDValues.DISP_VENDOR_FIELD_3.lidValue)
            {
                string Value = (string)objData;

                if (Value.Contains("Leuel"))
                {
                    Value = Value.Replace("Leuel", m_rmStrings.GetString("LEVEL"));
                }

                Item.Value = Value;
                bResult = true;
            }

			if (false == bResult)
			{
				switch (uiBaseLIDValue)
				{
					case (uint)DefinedLIDs.BaseLIDs.MISC_DATA:
						{
							uint uiMiscData = uiLIDValue & (uint)DefinedLIDs.Misc_Data.MISC_DATA_MASK;
							if ((uint)DefinedLIDs.Misc_Data.MISC_SEGMENT_TEST == uiMiscData)
							{
								// This is a segment test and cannot be retrieved from the meter
								Item.Value = "888888";
								bResult = true;
							}
							break;
						}
					case (uint)DefinedLIDs.BaseLIDs.DEMAND_DATA:
						{
							if ((uiLIDValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK) == (uint)DefinedLIDs.Demand_Data.MISC_DEMAND)
							{
								uint uiMiscDemand = uiLIDValue & (uint)DefinedLIDs.MiscDemand.MISC_DEMAND_MASK;

								switch (uiMiscDemand)
								{
									case (uint)DefinedLIDs.MiscDemand.TOO_TIME_REMAINING:
										{
											uint uiHours;
											uint uiMinutes;
											uint uiSeconds;
											uint uiTotalSeconds;
											string strData;

											// Change the data to look like the display does for Subinterval time remaining

											uiTotalSeconds = (uint)objData;

											uiHours = uiTotalSeconds / 3600;
											uiTotalSeconds -= uiTotalSeconds % 3600;

											uiMinutes = uiTotalSeconds / 60;
											uiSeconds = uiTotalSeconds % 60;

											strData = uiHours.ToString("00", CultureInfo.InvariantCulture) + ":" +
														uiMinutes.ToString("00", CultureInfo.InvariantCulture) + ":" +
														uiSeconds.ToString("00", CultureInfo.InvariantCulture);

											Item.Value = strData;

											break;
										}
								}
							}

							break;
						}
				}
			}

			if (false == bResult)
			{
				//  We have not found anything yet, so now check for special data types.
				if (TypeCode.DateTime == Item.DisplayLID.lidType)
				{
					// We need to convert the Date/Time items from Uint32 to DateTime
					DateTime dtDateTime = MeterReferenceTime;

					dtDateTime = dtDateTime.AddSeconds((uint)objData);

					// If the time is still the reference date we want to display 00-00-00
					if (dtDateTime != MeterReferenceTime)
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
				}
			}

			return bResult;
		}

		/// <summary>Executes standard and manufacturer procedures via standard
		/// tables 7 and 8.</summary>
		/// <param name="Proc">Standard or MFG procedure to execute</param>
		/// <param name="Parameters">Procedure parameters</param>
		/// <param name="ResponseData"></param>
		/// <returns>A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.</returns>
		/// <example><code>
		/// // Input parameters, Data reset bits - we don't want to rest 
		///	// any data, so just initialize them to 0.
		///	ProcParam = new byte[4];
		///	ProcParam.Initialize();
		///
		///	ProcResult = ExecuteProcedure( Procedure.CLOSE_CONFIG_FILE, 
		///						 ProcParam, out ProcResponse );
		///	if( Procedures.ProcedureResultCodes.COMPLETED != ProcResult )
		///	{
		///		Result = DSTUpdateResult.ERROR;
		///	}		
		/// </code></example>
		/// 
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  06/12/06 mcm 7.30.00    N/A Created
		//  02/23/07 RCG 8.00.13        The read does not seem to be enough time when waiting on
		//                              a procedure to complete at higher baud rates so a wait was added.
		//  0/05/07  KRC 8.10.12 3024   Change timing for Procedure not complete
		//  10/28/09 jrf 2.30.15        Adding check of table 07 write result and setting procedure 
		//                              result accordingly.
		//  10/30/09 RCG 2.30.16        Adding special code for Authenticate procedure
		//  03/08/10 RCG 2.40.23 150013 Changing to throw an exception when the write fails
		//  03/18/10 jrf 2.40.26        Making method internal so comm modules can call procedures.

		public ProcedureResultCodes ExecuteProcedure(Procedures Proc,
														 byte[] Parameters,
														 out byte[] ResponseData)
		{
			return ExecuteProcedure((ushort)Proc, Parameters, out ResponseData);
		} // ExecuteProcedure

		/// <summary>
		/// Executes standard and manufacturer procedures via standard
		/// tables 7 and 8.  This version will be used primarily to execute
		/// procedures not included in our enum
		/// </summary>
		/// <param name="ProcId">Procedure id for the Standard or MFG procedure to execute</param>
		/// <param name="Parameters">Procedure parameters</param>
		/// <param name="ResponseData">Response returned by the meter</param>
		/// <returns>A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.</returns>
		//  Revision History	
		//  MM/DD/YY Who Version Issue#  Description
		//  -------- --- ------- ------  -------------------------------------------
		//  09/08/10 AF  2.43.06         Created
        //  04/28/11 jrf 2.50.37         Adding up to 3 retries if we get a busy or data not ready response
        //                               from a table 07 write.
        //  05/03/11 AF  2.50.40         Added an initial sleep after FW download initiate
        //  05/05/11 AF  2.50.42 173253  M2 Gateway passthrough command will time out on a 10 sec sleep so exclude it
        //                               from the previous fix
        //  07/13/11 AF  2.51.24 176825  HW 2.050 poly meters seems to be slow on set clock.  Added an extra second of
        //                               initial wait to allow the meter time to respond.
        //  07/18/13 DLG 2.80.54 TC15657 Added case for ICS procedure to retry if not fully completed.
        //  08/09/13 DLG 2.85.14 TC15756 Removed the case for the ICS_MODULE_PROCEDURE. It is not needed now 
        //                               since the ICS events are working correctly with the newest ICS FW.
        //  12/06/13 AF  3.50.12 WR443125 Added a case back for the ICS_MODULE_PROCEDURE.  The freeze event log
        //                                subfunction will need more time to fully complete
        //  03/12/14 AF  3.50.49 WR442864 Added an initial sleep for the ICS_MODULE_PROCEDURE.  Needed for the freeze procedure.
        //  02/09/16 AF  4.50.227 No WR   Added more retries for the ICS_MODULE_PROCEDURE.  Needed for the close config procedure.
        //  02/10/16 AF  4.50.228 No WR   Added more retries for CLOSE_CONFIG_FILE and changed the retry delay to 1 second.
        //  03/10/16 AF  4.50.236 WR651410 Enabling retries when the result code for the table 8 read is busy.
        //  04/14/16 AF  4.50.245 WR670976 Extending the initial wait for set clock from 3 seconds to 4 seconds (May still not be long enough)
        //  04/22/16 AF  4.50.253 WR622562 Increasing the wait between table 7 write requests for mfg proc 200 when we get a response code
        //                                 of busy or dnr
        //
		public ProcedureResultCodes ExecuteProcedure(ushort ProcId, byte[] Parameters, out byte[] ResponseData)
		{
			ProcedureResultCodes Result = ProcedureResultCodes.NOT_FULLY_COMPLETED;
			PSEMResponse PSEMResult = PSEMResponse.Err;
			int RetryCount = 0;
			int MaxRetryCount = MAX_PROCEDURE_RETRIES;
			int RetryDelay = PROCEDURE_WAIT_MS;
            int InitialSleep = 0;
            bool bSendWait = true;

			// We need to make sure the HWVersion is read prior to calling a procedure so
			// that we do not do any reads when we don't want to.
			float fHWVersion = HWRevision;

            string strDeviceClass = DeviceClass;

			// To satisfy compiler error
			ResponseData = null;

			//Setup table 7 for the write
			Table07.Procedure = ProcId;
			Table07.ParameterData = Parameters;

			//Send the procedure to the meter
			PSEMResult = Table07.Write();

            //Try to recover if we get a busy or data not ready response.
            while ((PSEMResponse.Bsy == PSEMResult || PSEMResponse.Dnr == PSEMResult) 
                && RetryCount++ < 3)
			{
                if (ProcId == (ushort)Procedures.ICS_MODULE_PROCEDURE)
                {
                    Thread.Sleep(5000);
                }
                else
                {
                    Thread.Sleep(2000);
                }

                //Resend the procedure to the meter
			    PSEMResult = Table07.Write();
            }

			if (PSEMResponse.Ok != PSEMResult)
			{
				throw new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE, PSEMResult, "Procedure " + ProcId.ToString(CultureInfo.CurrentCulture));
			}

            //Reset this count
            RetryCount = 0;

			// Different methods have different requirements for how long to wait for a response.
			// Most methods will return a result within 2 seconds.  That means that if we get a 
			// "Procedure In Progress" we should keep trying as often as reasonable (every 100 ms)
			// for up to 2 seconds (20 iterations).
			// Other methods will take longer.  Close Config can take 5 - 6 seconds to complete. 
			// (Maybe longer in the future as we add more data to configuration.)  The remote
			// Connect\Disconnect for OpenWay can take over 15 seconds since it must charge the
			// capacitors.
			// Therefore we will define a default wait (100 ms; 20 times) and then for special cases
			// we will change the delay and number of iterations accordingly.

            switch (ProcId)
            {
                case (ushort)Procedures.CLOSE_CONFIG_FILE:
                {
                    // The meter has to validate and switch config blocks.  It
                    // takes a little time.  Be patient. 
                    // 4.5 seconds seems like a long time to me too, but our 
                    // timeout is 6 seconds, and the meters take 4.5-5.2 
                    // seconds to validate and swap config blocks. Testing 
                    // shows it runs 0.5 seconds faster WITH this wait than
                    // without it.
                    InitialSleep = 4500;

                    // After this we should try to read every half second and keep this up for about 15 more seconds
                    //  (This is the max time that a SATURN meter can take to do a full initialize.) Remember, this is
                    //  only used if the meter is reporting Procedure in Progress.  As soon as it is done, we will return.
                    MaxRetryCount = 80;
                    RetryDelay = 1000;
                    break;
                }
                case (ushort)Procedures.REGISTER:
                case (ushort)Procedures.DEREGISTER:    
                {
                    // For the Register and Deregister command we need to wait a few seconds before doing the read.
                    // Once we do the read, we should expect one of three responses.  Complete, Not Fully Complete,
                    //  or Device Busy.  Complete is great, Not Fully Completed indicates that the procedure was accepted,
                    //  but still is being processed.  In either of these two cases we should tell the user that the
                    //  command was received.  If we get a Device Busy that means the command was not accepted and the user
                    //  should issue the command again.
                    InitialSleep = 4000;

                    // We don't need to retry at this point.  Read the result and move on.
                    MaxRetryCount = 1;
                    break;
                }
                case (ushort)Procedures.REMOTE_CONNECT_DISCONNECT:
                case (ushort)Procedures.LOAD_SIDE_VOLTAGE_DETECTION:
                case (ushort)Procedures.FACTORY_REMOTE_SWITCH_TEST: 
                {
                    // The Connect and Disconnect and Load Side Voltage Detection require a capacitor to be charged, 
                    // which can take up to 60 seconds.  Therefore, we will read table 08 every five seconds for up to 65 seconds.  
                    // (If it is still processing after that we have a problem.)
                    MaxRetryCount = 13;
                    RetryDelay = 5000;
                    break;
                }
                case (ushort)Procedures.FORCE_TIME_SYNC:
                {
                    // The Force Time Sync can take up to a minute or two to complete.  We will read table 08 every 
                    //second for up to 3 seconds.  If we get a success great.  Otherwise we will just report the 
                    //response that we have received.
                    MaxRetryCount = 3;
                    RetryDelay = 1000;
                    break;
                }
                case (ushort)Procedures.AUTHENTICATE:  //AUTHENTICATE:
                {
                    // For HW 1.5 or HW 1.0 Poly meters the meter may not respond right away to any reads of table 8 so we should give
                    // it some time to process so we don't time out.
                    if (this is CENTRON_AMI && ((VersionChecker.CompareTo(fHWVersion, CENTRON_AMI.HW_VERSION_1_5) >= 0) || (VersionChecker.CompareTo(fHWVersion, CENTRON_AMI.HW_VERSION_1_0) >= 0)))
                    {
                        InitialSleep = 15000;
                    }

                    // On a HW 1.5 or HW 1.0 Poly OpenWay meter this procedure can take up to 10 secs. We will read table 8 every
                    // second for up to 15 seconds in order to give the meter enough time to process the authorization
                    // request.
                    MaxRetryCount = 15;
                    RetryDelay = 1000;
                    break;
                }
                case (ushort)Procedures.TURBO_TEST:
                {
                    // This is for automated testing only
                    // This procedure takes a very long time to complete. The following will not
                    // be long enough but it's ok unless we really want the procedure to succeed.
                    MaxRetryCount = 10;
                    RetryDelay = 5000;
                    break;
                }
                case (ushort)Procedures.INITIATE_FW_LOADER_SETUP:
                {
                    // On HW 3.0 meter this procedure might take 10 seconds so we want to retry the read
                    // CQ185086 - Backed out changed to increase # of retries
                    MaxRetryCount = 20;
                    RetryDelay = 500;

                    // We can't have a 10 second sleep if we are logged on to an M2 Gateway, so
                    // exclude that device class from the wait
                    if (string.Compare(strDeviceClass, LIS1, true, CultureInfo.CurrentCulture) != 0)
                    {
                        InitialSleep = 10000;
                    }
                    break;
                }
                case (ushort)Procedures.SET_DATE_TIME:
                {
                    // The meter doesn't always respond to wait properly so we shouldn't send it. This of course means we should wait less than 6s to avoid a timeout.
                    bSendWait = false;
                    InitialSleep = 4000;
                    MaxRetryCount = 10;
                    RetryDelay = 500;
                    break;
                }
                case (ushort)Procedures.ICS_MODULE_PROCEDURE:
                {
                    // With a slower clock speed in the ICM, the freeze event table procedure could take longer than 10 seconds to complete
                    // and the close config procedure can take over a minute.
                    InitialSleep = 5000;
                    MaxRetryCount = 60;
                    RetryDelay = 1000;
                    break;
                }
            }

            // (This seems to be a good thing.  This makes the operation look just like ITRON+)
            if (bSendWait)
            {
                SendWait();
            }

            if (InitialSleep > 0)
            {
                Thread.Sleep(InitialSleep);
            }

			while ((PSEMResponse.Ok == PSEMResult || PSEMResponse.Bsy == PSEMResult) &&
				  (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
				   ProcedureResultCodes.TIMING_CONSTRAINT == Result) &&
				  (RetryCount++ < MaxRetryCount))
			{
				//Read the table 8 response
				PSEMResult = Table08.Read();

				if (PSEMResponse.Ok == PSEMResult)
				{
					//Get the result code
					Result = Table08.ResultCode;

					//Get the result data
					ResponseData = new byte[Table08.ResultData.Length];
					Array.Copy(Table08.ResultData, 0, ResponseData, 0, ResponseData.Length);

					if (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
						ProcedureResultCodes.TIMING_CONSTRAINT == Result)
					{
						// TEMP CODE!!!  (Don't know if this is necessary)
                        if (bSendWait)
                        {
                            SendWait();
                        }

						Thread.Sleep(RetryDelay);
					}
				}
				else if (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
						 ProcedureResultCodes.TIMING_CONSTRAINT == Result)
				{
					// TEMP CODE!!!  (Don't know if this is necessary)
                    if (bSendWait)
                    {
                        SendWait();
                    }

					Thread.Sleep(RetryDelay);
				}
			}

			return Result;
		}

        /// <summary>
        /// Executes standard and manufacturer procedures via standard
        /// tables 7 and 8.  This version is designed primarily for Boron testing
        /// </summary>
        /// <param name="ProcId">Procedure id for the Standard or MFG procedure to execute</param>
        /// <param name="Parameters">Procedure parameters</param>
        /// <param name="ResponseData">Response returned by the meter</param>
        /// <returns>PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/11 AF  2.53.00        Created
        //  12/23/11 AF  2.53.23 184509 Reduced the number of retries - doesn't seem to help
        //
        internal ProcedureResultCodes ExecuteProcedureForBoron(ushort ProcId, byte[] Parameters, out byte[] ResponseData)
        {
            ProcedureResultCodes Result = ProcedureResultCodes.NOT_FULLY_COMPLETED;
            PSEMResponse PSEMResult = PSEMResponse.Err;
            int RetryCount = 0;
            int RetryDelay = PROCEDURE_WAIT_MS;
            int InitialSleep = 0;
            bool bSendWait = true;

            // We need to make sure the HWVersion is read prior to calling a procedure so
            // that we do not do any reads when we don't want to.
            float fHWVersion = HWRevision;

            string strDeviceClass = DeviceClass;

            // To satisfy compiler error
            ResponseData = null;

            //Setup table 7 for the write
            Table07.Procedure = ProcId;
            Table07.ParameterData = Parameters;

            //Send the procedure to the meter
            PSEMResult = Table07.Write();

            //Try to recover if we get a busy or data not ready response.
            while ((PSEMResponse.Bsy == PSEMResult || PSEMResponse.Dnr == PSEMResult)
                && RetryCount++ < 1)
            {
                Thread.Sleep(2000);

                //Resend the procedure to the meter
                PSEMResult = Table07.Write();
            }

            //Reset this count
            RetryCount = 0;

            // (This seems to be a good thing.  This makes the operation look just like ITRON+)
            if (bSendWait)
            {
                SendWait();
            }

            if (InitialSleep > 0)
            {
                Thread.Sleep(InitialSleep);
            }

            //Read the table 8 response
            PSEMResult = Table08.Read();

            if (PSEMResponse.Ok == PSEMResult)
            {
                //Get the result code
                Result = Table08.ResultCode;

                //Get the result data
                ResponseData = new byte[Table08.ResultData.Length];
                Array.Copy(Table08.ResultData, 0, ResponseData, 0, ResponseData.Length);

                if (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
                    ProcedureResultCodes.TIMING_CONSTRAINT == Result)
                {
                    // TEMP CODE!!!  (Don't know if this is necessary)
                    if (bSendWait)
                    {
                        SendWait();
                    }

                    Thread.Sleep(RetryDelay);
                }
            }
            else if (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
                        ProcedureResultCodes.TIMING_CONSTRAINT == Result)
            {
                if (bSendWait)
                {
                    SendWait();
                }

                Thread.Sleep(RetryDelay);
            }

            return Result;

        }

        #endregion Internal Methods

        #region Internal Properties

        /// <summary>
		/// Returns the Table00 Object; Creates it if it has not been created
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/17/06 KRC 8.00.00 N/A
		//
		public CTable00 Table00
		{
			get
			{
				if (null == m_Table0)
				{
					m_Table0 = new CTable00(m_PSEM);
				}

				return m_Table0;
			}
		}

		/// <summary>
		/// Returns the Table05 Object; Creates it if it has not been created
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/17/06 KRC 8.00.00 N/A
		//
		internal CTable05 Table05
		{
			get
			{
				if (null == m_Table5)
				{
					m_Table5 = new CTable05(m_PSEM);
				}

				return m_Table5;
			}
		}

		/// <summary>
		/// Returns the Table06 Object; Creates it if it has not been created
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/17/06 KRC 8.00.00 N/A
		//
		internal CTable06 Table06
		{
			get
			{
				if (null == m_Table6)
				{
					m_Table6 = new CTable06(m_PSEM, Table00.StdVersion);
				}

				return m_Table6;
			}
		}

		/// <summary>
		/// Returns the Table07 Object; Creates it if it has not been created
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/17/06 KRC 8.00.00 N/A
		//
		internal CTable07 Table07
		{
			get
			{
				if (null == m_Table7)
				{
					m_Table7 = new CTable07(m_PSEM);
				}

				return m_Table7;
			}
		}

		/// <summary>
		/// Returns the Table08 Object; Creates it if it has not been created
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/17/06 KRC 8.00.00 N/A
        //  09/10/13 jrf 2.85.40 WR422369 Property made public so ATIApp project would compile.
		//
		public CTable08 Table08
		{
			get
			{
				if (null == m_Table8)
				{
					m_Table8 = new CTable08(m_PSEM);
				}

				return m_Table8;
			}
		}

        /// <summary>
		/// This method creates table 2048. It is expected to be overridden by
		/// derived devices
		/// </summary>
		/// <remarks>
		/// This method must be overriden by the device classes.
		/// </remarks>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/22/06 mrj 7.30.00 N/A    Created
		// 	
		internal virtual CTable2048 Table2048
		{
			get
			{
				return null;
			}
		}

        /// <summary>
		/// This property returns Table 2084. (Creates it if needed)
		/// </summary>
		/// <remarks>
		/// This method must be overriden by the device classes.
		/// </remarks>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  11/20/06 KRC 8.00.00 N/A    Created
		// 
		internal CTable2084 Table2084
		{
			get
			{
				if (null == m_Table2084)
				{
					m_Table2084 = new CTable2084(m_PSEM);
				}

				return m_Table2084;
			}
		}

        /// <summary>
        /// Builds the pending table header
        /// </summary>
        /// <param name="strmPSEM">Stream to which the data is written</param>
        /// <param name="bSelfRead">tells whether or not to perform a self read 
        /// before table is activated</param>
        /// <param name="bDemandReset">tells whether or not to perform a demand 
        /// reset before table is activated</param>
        /// <param name="byMfgEventCode">Mfg assigned code identifying event for 
        /// activating pending table</param>
        /// <param name="eCode">Event code for status bitfield.  2 => non-time activated</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/16/06 AF 7.35.00  N/A    Created
        // 10/04/06 AF 7.40.00  N/A    Moved from CENTRON_AMI.cs
        // 06/08/07 RCG 8.10.07        Changed to call BuildEventRecord
        // 11/15/13 AF  3.50.04	       Class re-architecture - promoted from CENTRON_AMI
        // 01/06/14 DLG 3.50.19        Changed from protected to internal.
        //
        internal void BuildPendingHeader(ref MemoryStream strmPSEM, bool bSelfRead, bool bDemandReset,
            byte byMfgEventCode, PendingEventRecord.PendingEventCode eCode)
        {
            PendingEventRecord EventRecord = new PendingEventRecord();

            // Build the event record
            EventRecord = BuildPendingEventRecord(bSelfRead, bDemandReset, byMfgEventCode, eCode);

            // Write the event record to the stream
            strmPSEM.Write(EventRecord.EntireRecord, (int)strmPSEM.Position, EventRecord.EntireRecord.Length);
        }

        /// <summary>
        /// Retrieves the optimal firmware download block size based on the negotiated
        /// PSEM packet size.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/14 jrf 3.50.32 419257 Created
        //
        internal ushort FWDLBlockSize
        {
            get
            {
                ushort usFWDLBlockSize = BLOCK_SIZE;

                try
                {
                    usFWDLBlockSize = (ushort)(m_PSEM.PacketSize - FWDL_BLOCK_OVERHEAD);

                    //If size is not even, make it even.
                    if (usFWDLBlockSize % 2 != 0)
                    {
                        usFWDLBlockSize--;
                    }

                    if (usFWDLBlockSize < BLOCK_SIZE)
                    {
                        usFWDLBlockSize = BLOCK_SIZE;
                    }
                }
                catch
                {
                    usFWDLBlockSize = BLOCK_SIZE;
                }

                return usFWDLBlockSize;
            }
        }

        #endregion Internal Properties

        #region Protected Methods 

        /// <summary>
        /// Initialize all of our member variables for the Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //
        protected virtual void InitializeData()
        {
            m_lidRetriever = new LIDRetriever(m_PSEM);

            m_Logger = Logger.TheInstance;
            m_ClockRunning = new CachedBool();
            m_NumTimesProgrammed = new CachedInt();
            m_NumOutages = new CachedInt();
            m_DateLastOutage = new CachedDate();
            m_DateLastCalibration = new CachedDate();
            m_DateLastTest = new CachedDate();
            m_IsCanadian = new CachedBool();
            m_IsSealedCanadian = new CachedBool();
            m_MinutesOnBattery = new CachedUint();
            m_DayOfTheWeek = new CachedString();
            m_TOUExpireDate = new CachedDate();
            m_MeterInDST = new CachedBool();
            m_uiNumTOURates = new CachedUint();
            m_uiNumEnergies = new CachedUint();
            m_uiNumDemands = new CachedUint();
            m_uiFWBuild = new CachedUint();
            m_bDSTEnabled = new CachedBool();
        }

        /// <summary>
        /// Used to determine which tables will be written to the EDL file.
        /// </summary>
        /// <param name="usTableID">Table ID to check.</param>
        /// <returns>True if the table can be written, false otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created
        protected virtual bool AllowTableExport(ushort usTableID)
        {
            // We are going to control the tables that are written
            // to the EDL file by the tables that we read. This
            // way we only need to change one place whenever new
            // tables are added or removed.
            return true;
        }

        /// <summary>
        /// Determines which fields may be written to the EDL file.
        /// </summary>
        /// <param name="idElement">The field to check.</param>
        /// <param name="anIndex">The indexes into the field.</param>
        /// <returns>True if the field may  be written to the EDL file. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created
        //
        protected virtual bool AllowFieldExport(long idElement, int[] anIndex)
        {
            bool bAllowExport = false;

            // Currently there are no fields that we wish to exclude
            switch (idElement)
            {
                // These items are in 2048 but are also stored elsewhere
                // so we do not need to export them twice
                case (long)CentronTblEnum.MFGTBL0_UNKNOWN_BLOCK:
                case (long)CentronTblEnum.MFGTBL0_DECADE_0:
                case (long)CentronTblEnum.MFGTBL0_DECADE_8:
                    {
                        bAllowExport = false;
                        break;
                    }
                default:
                    {
                        bAllowExport = true;
                        break;
                    }
            }

            return bAllowExport;
        }

        /// <summary>
        /// This method checks to determine if a particular table has a length greater than zero.
        /// </summary>
        /// <param name="usTableID">The table ID to check.</param>
        /// <param name="MeterTables">Meter tables data.</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/12 jrf	2.53.35 192864 Created
        // 08/10/15 AF  4.20.21 603216 Added a case for table 2412 (extended load profile)
        // 08/10/15 AF  4.20.21 603216 Changed the fields we check to see if extended load profile
        //                             is configured on the advice of Steve Bardey
        // 08/11/15 AF  4.20.21 603216 Removed the check on the number of channels in set 2 - that's
        //                             for instrumentation profile, not extended LP.
        //
        protected bool DoesTableHaveLength(ushort usTableID, CentronTables MeterTables)
        {
            bool blnTableHasLength = false;

            switch (usTableID)
            {
                //Handling voltage monitoring data table special cases.  C1219 dlls will throw exception if
                //GetTableLength() is called with one of these tables and it has no data in it.
                case 2152: //legacy voltage monitoring data.  
                    {
                        object objData;
                        ushort usNumberIntervals = 0;
                        bool bEnabled = false;

                        if (MeterTables.IsCached((long)CentronTblEnum.MFGTBL102_ENABLE_FLAG, null) == true)
                        {
                            MeterTables.GetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, out objData);
                            bEnabled = (bool)objData;
                        }

                        if (true == bEnabled && true == MeterTables.IsCached((long)CentronTblEnum.MFGTBL103_NBR_VALID_INT, null))
                        {
                            MeterTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_INT, null, out objData);
                            usNumberIntervals = (ushort)objData;

                            if (0 < usNumberIntervals)
                            {
                                blnTableHasLength = true;
                            }
                        }

                        break;
                    }
                case 2157:  //extended voltage monitoring data. 
                    {
                        object objData;
                        ushort usNumberIntervals = 0;
                        bool bEnabled = false;

                        if (MeterTables.IsCached((long)CentronTblEnum.MFGTBL106_ENABLE_FLAG, null) == true)
                        {
                            MeterTables.GetValue(CentronTblEnum.MFGTBL106_ENABLE_FLAG, null, out objData);
                            bEnabled = (bool)objData;
                        }

                        if (true == bEnabled && true == MeterTables.IsCached((long)CentronTblEnum.MFGTBL107_NBR_VALID_INT, null))
                        {

                            MeterTables.GetValue(CentronTblEnum.MFGTBL107_NBR_VALID_INT, null, out objData);
                            usNumberIntervals = (ushort)objData;

                            if (0 < usNumberIntervals)
                            {
                                blnTableHasLength = true;
                            }
                        }

                        break;
                    }
                //Handling extended load profile data table special cases.  C1219 dlls will throw exception if
                //GetTableLength() is called with one of these tables and it has no data in it.
                case 2412:  // extended load profile data
                    {
                        object objData;
                        byte byNumChannelsSet1 = 0;

                        if (MeterTables.IsCached((long)CentronTblEnum.MfgTbl361_NBR_CHNS_SET1, null) == true)
                        {
                            MeterTables.GetValue(CentronTblEnum.MfgTbl361_NBR_CHNS_SET1, null, out objData);
                            byNumChannelsSet1 = (byte)objData;
                        }

                        // If no channels are present,then extended LP is not configured
                        if (0 < byNumChannelsSet1)
                        {
                            blnTableHasLength = true;
                        }
                        break;
                    }
                default:
                    {
                        if (MeterTables.GetTableLength(usTableID) > 0)
                        {
                            blnTableHasLength = true;
                        }

                        break;
                    }
            }

            return blnTableHasLength;
        }

        /// <summary>
        /// This method peforms a full table read.
        /// </summary>
        /// <param name="table">The ID of the table to read.</param>
        /// <param name="data">The data read from the table.</param>
        /// <returns>The PSEM Response indicating the success or failure result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version  ID  Number Description
        // -------- --- -------- --- ------ ---------------------------------------
        // 12/02/15 jrf	4.50.217 REQ 587913 Created
        public PSEMResponse ReadTable(ushort table, out byte[] data)
        {
            return m_PSEM.FullRead(table, out data);
        }

        /// <summary>
        /// This method peforms an offset table read.
        /// </summary>
        /// <param name="table">The ID of the table to read.</param>
        /// <param name="offset">The offset of the data to read.</param>
        /// <param name="count">The number of bytes of data to read.</param>
        /// <param name="data">The data read from the table.</param>
        /// <returns>The PSEM Response indicating the success or failure result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version  ID  Number Description
        // -------- --- -------- --- ------ ---------------------------------------
        // 12/02/15 jrf	4.50.217 REQ 587913 Created
        public PSEMResponse OffsetRead(ushort table, int offset, ushort count, out byte[] data)
        {
            return m_PSEM.OffsetRead(table, offset, count, out data);
        }

        /// <summary>
        /// This method performs a full table write
        /// </summary>
        /// <param name="table">The ID of the table to write</param>
        /// <param name="data">The data to write to the table</param>
        /// <returns>The PSEM Response indicating the success or failure of the write</returns>
        // Revision History	
        // MM/DD/YY who Version  ID  Number Description
        // -------- --- -------- --- ------ ---------------------------------------
        // 12/02/15 AF  4.50.218 RTT 576947 Created
        //
        public PSEMResponse WriteTable(ushort table, byte[] data)
        {
            return m_PSEM.FullWrite(table, data);
        }

        /// <summary>
        /// This method performs an offset write to a table
        /// </summary>
        /// <param name="table">The ID of the table to write</param>
        /// <param name="offset">The offset of the data to write</param>
        /// <param name="data">The data to write to the meter</param>
        /// <returns>The PSEM Response indicating the success or failure of the write</returns>
        // Revision History	
        // MM/DD/YY who Version  ID  Number Description
        // -------- --- -------- --- ------ ---------------------------------------
        // 12/02/15 AF  4.50.218 RTT 576947 Created
        //
        public PSEMResponse OffsetWrite(ushort table, int offset, byte[] data)
        {
            return m_PSEM.OffsetWrite(table, offset, data);
        }

        /// <summary>
        /// Reads the specified table from the meter.
        /// </summary>
        /// <param name="usTableID">The table ID for the table to read.</param>
        /// <param name="MeterTables">The tables object to read the table into.</param>
        /// <returns>PSEMResponse code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created
        // 01/11/07 RCG 8.00.05         Removed code that would do a full read since
        //                              the meter no longer supports full reads of 64
        // 12/13/11 jrf 2.53.17 TC2907  Adding special case for table 2157, extended VM data table.
        // 12/15/11 RCG 2.53.20         Adding support for Instrumentation Profile Data
        // 06/27/13 AF  2.80.44 TR7648  Added case for table 2524 (ICS event table)
        // 08/16/13 AF  2.85.19         Using a different MemoryStream constructor in default case 
        //                              to get better debugging data on failure.
        // 09/03/13 jrf 2.85.34 WR418110 Added cases for ERT tables 2508 and 2511.
        //
        public virtual PSEMResponse ReadTable(ushort usTableID, ref CentronTables MeterTables)
        {
            MemoryStream PSEMDataStream;
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] byaData;
            int iReadAttempt = 0;
            bool bRetry = true;

            while (bRetry)
            {
                switch (usTableID)
                {
                    case 64:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable64(ref MeterTables);
                            }

                            break;
                        }
                    default:
                        {
                            PSEMResult = m_PSEM.FullRead(usTableID, out byaData);

                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMDataStream = new MemoryStream(byaData, 0, byaData.Length, true, true);
                                MeterTables.SavePSEMStream(usTableID, PSEMDataStream);
                            }

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
        /// Reads Table 64 from the meter.
        /// </summary>
        /// <param name="MeterTables">The table object for the meter.</param>
        /// <returns>The PSEM response code.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/08 RCG 1.50.24 N/A    Created
        //
        protected PSEMResponse ReadTable64(ref CentronTables MeterTables)
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

            // Since Load Profile can be very large (144k) it may not be able
            // to be read completely when doing a full read so we need to break
            // it up into multiple offset reads. Table 61 must be read prior to this.

            if (MeterTables.IsCached((long)StdTableEnum.STDTBL61_LP_MEMORY_LEN, null) == true)
            {
                uiMaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                if (uiMaxOffsetReadBytes > ushort.MaxValue)
                {
                    uiMaxOffsetReadBytes = ushort.MaxValue;
                }

                MeterTables.GetValue((long)StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null, out objData);
                usValidBlocks = (ushort)objData;

                MeterTables.GetValue((long)StdTableEnum.STDTBL63_NBR_VALID_INT, null, out objData);
                usNumberIntervals = (ushort)objData;

                // Determine the size of a Load Profile data block
                MeterTables.GetFieldOffset((long)StdTableEnum.STDTBL64_LP_DATA_SETS, new int[] { 0 },
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

                    PSEMResult = m_PSEM.OffsetRead(64, (int)uiCurrentOffset, (ushort)uiBytesToRead, out byaData);

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMDataStream = new MemoryStream(byaData);
                        MeterTables.SavePSEMStream(64, PSEMDataStream, uiCurrentOffset);
                        uiCurrentOffset += uiBytesToRead;
                    }

                    OnStepProgress(new ProgressEventArgs());
                }

                // Reread table 63 and make sure no new intervals have occurred while reading
                CentronTables.CopyTable(0, MeterTables, TempTables);
                CentronTables.CopyTable(1, MeterTables, TempTables);
                CentronTables.CopyTable(60, MeterTables, TempTables);
                CentronTables.CopyTable(61, MeterTables, TempTables);
                CentronTables.CopyTable(62, MeterTables, TempTables);

                do
                {
                    ReadTable(63, ref TempTables);

                    TempTables.GetValue((long)StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null, out objData);
                    usNewValidBlocks = (ushort)objData;

                    TempTables.GetValue((long)StdTableEnum.STDTBL63_NBR_VALID_INT, null, out objData);
                    usNewNumberIntervals = (ushort)objData;

                    if (usNewNumberIntervals != usNumberIntervals || usNewValidBlocks != usValidBlocks)
                    {
                        // This will limit us to only two tries at this. (if it is already true it will be set
                        // to false which means we won't try this again.)
                        bBlocksReRead = !bBlocksReRead;

                        // A new interval has occurred so we need to reread at least one block
                        CentronTables.CopyTable(63, TempTables, MeterTables);

                        MeterTables.GetValue((long)StdTableEnum.STDTBL63_LAST_BLOCK_ELEMENT, null, out objData);
                        usNewLastBlock = (ushort)objData;

                        // Determine the offset of the block
                        iBlockToRead = (int)usNewLastBlock;
                        MeterTables.GetFieldOffset((long)StdTableEnum.STDTBL64_LP_DATA_SETS, new int[] { iBlockToRead },
                            out uiBlockOffset, out uiBlockLength);

                        PSEMResult = m_PSEM.OffsetRead(64, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(byaData);
                            MeterTables.SavePSEMStream(64, PSEMDataStream, uiBlockOffset);

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
                                MeterTables.GetFieldOffset((long)StdTableEnum.STDTBL64_LP_DATA_SETS, new int[] { iBlockToRead },
                                    out uiBlockOffset, out uiBlockLength);

                                PSEMResult = m_PSEM.OffsetRead(64, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                                if (PSEMResult == PSEMResponse.Ok)
                                {
                                    PSEMDataStream = new MemoryStream(byaData);
                                    MeterTables.SavePSEMStream(64, PSEMDataStream, uiBlockOffset);
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
                throw new Exception("Table 61 must be read prior to Table 64.");
            }
            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2242
        /// </summary>
        /// <param name="MeterTables">The meter tables object</param>
        /// <param name="MaxOffsetReadSize">The maximum size of an offset read</param>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/14 AF  3.50.91 WR503773 Cloned from CENTRON_AMI but modified to take a
        //                                max offset read parameter
        //  01/16/15 AF  4.00.92 556830 Corrected the offset read code to make sure it 
        //                              doesn't try to read beyond the end of the table
        //
        protected PSEMResponse ReadTable2242(ref CentronTables MeterTables, uint MaxOffsetReadSize)
        {
            PSEMResponse PSEMResult;
            MemoryStream PSEMDataStream;
            byte[] PSEMData;

            uint TableSize = 0;
            uint CurrentOffset = 0;

            TableSize = MeterTables.GetTableLength(2242);

            if (TableSize < MaxOffsetReadSize)
            {
                // We can read the whole table in the Full Read
                PSEMResult = m_PSEM.FullRead(2242, out PSEMData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(PSEMData);
                    MeterTables.SavePSEMStream(2242, PSEMDataStream);
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
                MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl194UpLogNumberUnreadEntries, null, out NumberUnreadEntriesOffset, out NumberUnreadEntriesLength);

                HeaderSize = NumberUnreadEntriesOffset + NumberUnreadEntriesLength;

                // Read the header
                PSEMResult = m_PSEM.OffsetRead(2242, 0, (ushort)HeaderSize, out PSEMData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(PSEMData);
                    MeterTables.SavePSEMStream(2242, PSEMDataStream, 0);

                    CurrentOffset += HeaderSize;

                    MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl194Entries, new int[] { 0 }, out EntryOffset, out EntrySize);

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

                        PSEMResult = m_PSEM.OffsetRead(2242, (int)CurrentOffset, BytesToRead, out PSEMData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(PSEMData);
                            MeterTables.SavePSEMStream(2242, PSEMDataStream, CurrentOffset);

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
        /// Reads Table 2243
        /// </summary>
        /// <param name="MeterTables">The meter tables object</param>
        /// <param name="MaxOffsetReadSize">The maximum size of an offset read</param>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/14 AF  3.50.91 WR503773 Cloned from CENTRON_AMI but modified to take a
        //                                max offset read parameter
        //  01/16/15 AF  4.00.92 WR556830 Corrected the offset read code to make sure it 
        //                                doesn't try to read beyond the end of the table
        //
        protected PSEMResponse ReadTable2243(ref CentronTables MeterTables, uint MaxOffsetReadSize)
        {
            PSEMResponse PSEMResult;
            MemoryStream PSEMDataStream;
            byte[] PSEMData;

            uint TableSize = 0;
            uint CurrentOffset = 0;

            TableSize = MeterTables.GetTableLength(2243);

            if (TableSize < MaxOffsetReadSize)
            {
                // We can read the whole table in the Full Read
                PSEMResult = m_PSEM.FullRead(2243, out PSEMData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(PSEMData);
                    MeterTables.SavePSEMStream(2243, PSEMDataStream);
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
                MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl195DownLogNumberUnreadEntries, null, out NumberUnreadEntriesOffset, out NumberUnreadEntriesLength);

                HeaderSize = NumberUnreadEntriesOffset + NumberUnreadEntriesLength;

                // Read the header
                PSEMResult = m_PSEM.OffsetRead(2243, 0, (ushort)HeaderSize, out PSEMData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(PSEMData);
                    MeterTables.SavePSEMStream(2243, PSEMDataStream, 0);

                    CurrentOffset += HeaderSize;

                    MeterTables.GetFieldOffset((long)CentronTblEnum.MfgTbl195Entries, new int[] { 0 }, out EntryOffset, out EntrySize);

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

                        PSEMResult = m_PSEM.OffsetRead(2243, (int)CurrentOffset, BytesToRead, out PSEMData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(PSEMData);
                            MeterTables.SavePSEMStream(2243, PSEMDataStream, CurrentOffset);

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
        /// Creates a list of tables to read from the meter.
        /// </summary>
        /// <returns>The list of tables to read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created
        // 05/05/08 jrf 1.50.22 114166  When network tables are not included 
        //                              tables 121 and 123 still need to be included
        //                              since they are part of the EDL configuration.
        //                              This keeps the EDL viewer from throwing an exception.
        // 05/20/08 KRC 1.50.26 115111  Remove Table 24 from EDL creation since it causes an error
        //                              in the firmware and is not needed since it is not supported.
        // 10/10/08 KRC 2.00.00         Add 2062 - C12.22 Status Table
        // 08/04/09 jrf 2.20.20 137693  Adding 2064 - Comm Module General Config Table.
        // 08/23/10 AF  2.43.00 160255  The hardware version check for the tamper tap tables needs
        //                              to take PrismLite into account
        // 01/19/11 RCG 2.45.23         Adding HAN Event log tables
        // 05/31/11 jrf 2.50.50 173598  Including all voltage monitoring tables but the data table when 
        //                              voltage monitoring data is not selected for inclusion in the 
        //                              data file. This allows correct VM configuration to always be 
        //                              displayed.
        // 06/09/11 jrf 2.51.10 173353  Adding the power monitoring tables to the EDL file.
        // 07/08/11 jrf 2.51.22 173801  Including tables that show how comm logs are configured in all
        //                              datafiles.  Now only the comm log data table are conditional.
        // 08/11/11 jrf 2.52.02 TREQ 2705 Including mfg table 2379 and 2382 so the fwdl event log can 
        //                                be retrieved from the EDL file.
        // 08/11/11 jrf 2.52.02 TREQ 2711 Including mfg. table 2383 so the fwdl CRCs can be retrieved 
        //                                from the EDL file.
        // 08/11/11 jrf 2.52.02 TREQ 2712 Including mfg table 2220 so it can be determined from the EDL 
        //                                file if the meter is Canadian.
        // 08/24/11 jrf 2.52.07 TREQ 2705 Making inclusion of mfg table 2382 (FWDL event log) conditional 
        //                                to the history log section inclusion.
        // 12/13/11 jrf 2.52.17 TREQs 2891,2896,2907,3440&3447 Populating the EDL fle with new tables
        //                                pertinent to the Lithium project.
        // 12/12/11 RCG 2.53.20			Adding support for Instrumentation Profile Data
        // 01/04/12 jrf 2.53.24 CQ185301 Removing addition of mfg. table 375 (2423) until CQ185301 is resolved.
        // 01/04/12 jrf 2.53.24 CQ185305 Only including data exteneded LP data and instrumentation profile data tables when 
        //                               there are valid blocks of data available.
        // 01/06/12 jrf 2.53.26 CQ185305 Correcting error with previous check-in.  Making sure ext. LP and IP tables are supported
        //                               before checking values in them.
        // 01/06/12 jrf 2.53.26 CQ185301 Adding mfg. table 375 (2423) back since issue has been resolved.
        // 05/14/12 RCG 2.60.22 TRQ6031  Adding support for the RIB tables  
        // 03/06/13 jrf 2.80.06 TREQ7646 Adding table 2191.
        // 04/19/13 jrf 2.80.21 TQ8279 Adding ICS module config table 2512.
        // 05/17/13 jkw 2.80.32 TREQs 7871, 7873, 7650, 7649, 7872 and 7874 include required ICS tables.
        // 05/31/13 jrf 2.80.36 TQ8279 Adding ICS history event configuration.
        // 06/26/13 AF  2.80.43 TR7648  Added the ICS history event data if it is to be included
        // 07/02/13 AF  2.80.45 TR7640  Exclude table 2048 for the I-210 and kV2c. The table doesn't match ITRJ's table
        //                              and causes problems when we try to load the file in the viewer
        // 07/10/13 jrf 2.71.01 417155  Removing table 2175 (12 max demands) for the short term to prevent
        //                              the failure of EDL File creation in Lithium/Lithium+ meters.
        // 08/14/13 jrf 2.85.18 TQ7656  Adding tables 2509 (ERT) and 2517 (cellular) so values in them 
        //                              can be validated during data file validation.
        // 08/28/13 jrf 2.85.34 WR 418110 Adding call to populate ERT data tables when they should be 
        //                                included in the EDL file.
        // 09/05/13 jrf 2.85.36 WR 422079 I-210 is reporting that ERT data is populated, added other checks to 
        //                                make sure update ERT tables proc isn't called on ICS Gateway and doesn't 
        //                                cause EDL creation failure if the proc fails.
        // 10/23/13 AF  3.00.22 WR426017 Changed the check for Instrumentation Profile to check the number of channels
        //                              instead of the number of blocks.  The number of blocks can be left at a previous
        //                              value when a config that does not configure Inst. Prof. replaces one that does.
        // 11/07/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876 Updated the logic for when to add the ICS Event 
        //                              related tables.
        // 01/20/14 DLG 3.50.28 TREQs 9518, 9519, 9531 - Removed the conditional statement for adding
        //                              the HAN data. This is now displayed in the Configuration Data.
        // 01/27/16 PGH 4.50.224 627380 Added table 2377
        // 01/27/16 PGH 4.50.224 RTT556309 Added tables 2425, 2426, 2427
        // 02/17/16 AF  4.50.231 WR419822 Added table 2760 (Tamper Summary Table)
        // 02/23/16 AF  4.50.232 WR236192 Added back table 2175
        // 05/23/17 CFB 4.72.00  WR741238 Added tables 2536 and 2537 
        //
        protected virtual List<ushort> GetTablesToRead(EDLSections IncludedSections)
        {
            List<ushort> TableList = new List<ushort>();

            TableList.Add(0);           // General Configuration
            TableList.Add(1);           // Manufacturer Identification
            TableList.Add(5);           // Device Identification
            TableList.Add(6);           // Utility Information
            TableList.Add(52);          // Clock
            TableList.Add(53);          // Time offset

            TableList.Add(120);         // Dim Network Table
            TableList.Add(121);         // Actual Network Table
            TableList.Add(123);         // Exception Report Table

            if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            {
                TableList.Add(122);     // Interface Control Table
                TableList.Add(125);     // Interface Status Table
                TableList.Add(126);     // Registration Status Table
                TableList.Add(127);     // Network Statistics Table
            }

            TableList.Add(2098);    // HAN Dimension Limiting Table
            TableList.Add(2099);    // Actual HAN Limiting Table
            TableList.Add(2102);    // HAN Transmit Data Table                
            TableList.Add(2104);    // HAN Network Info Table
            TableList.Add(2106);    // HAN Config Paramaters

            TableList.Add(2190);        // Communications Config
            TableList.Add(2191);        // C12.22 Config Table

            TableList.Add(2425);        // Temperature Configuration
            TableList.Add(2426);        // Instantaneous Temperature Data
            TableList.Add(2427);        // Temperature Log

            TableList.Add(2239);        // Actual HAN Event Log Table
            TableList.Add(2240);        // HAN Events Identification Table
            TableList.Add(2241);        // HAN Events Control Table

            if ((IncludedSections & EDLSections.LANandHANLog) == EDLSections.LANandHANLog)
            {
                TableList.Add(2242);    // HAN Upstream Log Table
                TableList.Add(2243);    // HAN Downstream Log Table
            }

            TableList.Add(2265);        // Non Metrological Configuration Data Table
            TableList.Add(2377);        // Instantaneous Phase Current Table
            TableList.Add(2760);        // Tamper Summary Table

            TableList.Add(2536);        //ICM Actual Network Table
            TableList.Add(2537);        //ICM Exception Report Configuration Table

            return TableList;
        }

        /// <summary>
        /// Copies the string password into the byte array.  The string is truncated to the 
        /// specified Length if necessary.  If the string is shorter than the requested length, the 
        /// byte array will be null filled.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws a ArgumentException if the byte array length does not
        /// match the specified length
        /// </exception>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 08/17/06 mcm 7.35.00           Created
        // 01/16/14 DLG 3.50.26           Moved from ItronDevice to CANSIDevice because this is not 
        //                                needed or used in the DLMS device.
        // 
        protected virtual void NullFillPassword(string Password, int Length,
                                                ref byte[] byPassword)
        {
            if (Length != byPassword.Length)
            {
                throw new ArgumentException("Byte array not allocated to requested length");
            }
            else
            {
                for (int Index = 0; Index < Length; Index++)
                {
                    if (Index < Password.Length)
                    {
                        byPassword[Index] = (byte)Password[Index];
                    }
                    else
                    {
                        byPassword[Index] = 0;
                    }
                }
            }
        }

        #endregion Protected Methods

        #region Protected Properties

        /// <summary>
		/// Returns the Table01 Object; Creates it if it has not been created
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/17/06 KRC 8.00.00 N/A
		//
		protected CTable01 Table01
		{
			get
			{
				if (null == m_Table1)
				{
					m_Table1 = new CTable01(m_PSEM, Table00.StdVersion);
				}

				return m_Table1;
			}
		}

        /// <summary>
		/// Returns the Table01 Object; Creates it if it has not been created
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
		//  06/05/09 jrf 2.20.08 135495 Created.
        //  07/27/15 jrf 4.20.18 WR 599965 Passing in parameter to determine how 
        //                                 many manufacturer status bytes there are
        //                                 since this is not constant.
        // 07/30/15 jrf 4.50.178 WR 599965 Per code review passing in table 0.
		protected CTable03 Table03
		{
			get
			{
				if (null == m_Table3)
				{
					m_Table3 = new CTable03(m_PSEM, Table00);
				}

				return m_Table3;
			}
		}

		/// <summary>
		/// Gets the default meter type, overriden by inherited classes
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  04/25/06 mrj 7.30.00 N/A    Created
		// 	
		protected virtual string DefaultMeterType
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// The Base Date for calculating dates in ANSI Devices
		/// </summary>
		/// <returns></returns>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  08/30/06 KRC 7.35.00 N/A    Created
		// 
		internal virtual DateTime MeterReferenceTime
		{
			get
			{
				return new DateTime(2000, 1, 1);
			}
		}

		/// <summary>
		/// The Base Date for calculating dates from LID reads in ANSI Devices
		/// </summary>
		/// <returns></returns>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/14/08 mrj 1.50.25		Created for itron00107508
		// 
		internal virtual DateTime UTCMeterReferenceTime
		{
			get
			{
				return MeterReferenceTime;
			}
		}

        // TimeZoneInfo is only supported in .NET 3.5 Framework
#if (!WindowsCE)
        /// <summary>
        /// Gets the Time Zone Info class for the time zone the meter is programmed for.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??                          Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
        protected TimeZoneInfo DeviceTimeZoneInfo
        {
            get
            {
                // Declare necessary TimeZone2.AdjustmentRule objects for time zone

                TimeSpan delta = new TimeSpan(1, 0, 0);
                TimeZoneInfo.AdjustmentRule adjustment;
                List<TimeZoneInfo.AdjustmentRule> adjustmentList = new List<TimeZoneInfo.AdjustmentRule>();
                TimeZoneInfo.TransitionTime transitionRuleStart, transitionRuleEnd;

                if (DSTEnabled)
                {
                    // Define rule (1987-2006)  
                    transitionRuleStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 01, DayOfWeek.Sunday);
                    transitionRuleEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 10, 5, DayOfWeek.Sunday);
                    adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1987, 1, 1), new DateTime(2006, 12, 31), delta, transitionRuleStart, transitionRuleEnd);
                    adjustmentList.Add(adjustment);

                    // Define rules for (2007 - )
                    transitionRuleStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 03, 02, DayOfWeek.Sunday);
                    transitionRuleEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 01, DayOfWeek.Sunday);
                    adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2007, 01, 01), DateTime.MaxValue.Date, delta, transitionRuleStart, transitionRuleEnd);
                    adjustmentList.Add(adjustment);
                }
                TimeSpan tzOffset = TimeZoneOffset;

                TimeZoneInfo DeviceTZInfo = TimeZoneInfo.CreateCustomTimeZone("Local Time Zone", tzOffset, "Local Time Zone",
                                    "Standard Time", "Daylight Time", adjustmentList.ToArray(), !DSTEnabled);

                return DeviceTZInfo;
            }
        }
#endif

        /// <summary>
        /// Gets the Table 51 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        //
        protected StdTable51 Table51
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
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        //
        protected StdTable52 Table52
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
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        //
        protected StdTable53 Table53
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
        /// Return the Table Object for Table 2064 (Mirror of Comm Module Table 0)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/29/08 KRC 2.00.02			Created
        // 06/03/10 AF  2.41.06         Changed the access level so that it can be
        //                              accessed by M2 Gateway
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        //
        protected OpenWayCommModule_2064 Table2064
        {
            get
            {
                if (null == m_Table2064)
                {
                    m_Table2064 = new OpenWayCommModule_2064(m_PSEM);
                }

                return m_Table2064;
            }
        }

        /// <summary>
        /// Gets the Enhanced security table and creates it if needed. If the meter does not support
        /// this table null will be returned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/08 RCG 1.50.22 N/A    Created
        //  04/28/10 AF  2.40.44        Changed the access level to protected so that
        //                              the M2 Gateway can call it
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
        protected OpenWayMFGTable2193 Table2193
        {
            get
            {
                if (null == m_Table2193 && true == Table00.IsTableUsed(2193))
                {
                    m_Table2193 = new OpenWayMFGTable2193(m_PSEM);
                }

                return m_Table2193;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect State table and creates it if needed.
        /// If the meter does not support this table null will be returned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/06/12 JJJ 2.60.xx  N/A   Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //
        protected OpenWayMFGTable2428 Table2428
        {
            get
            {
                if (null == m_Table2428 && true == Table00.IsTableUsed(2428))
                {
                    m_Table2428 = new OpenWayMFGTable2428(m_PSEM);
                }

                return m_Table2428;
            }
        }

        /// <summary>
        /// Represents the comm module object - DO NOT use this property directly, always use the accessor instead
        /// </summary>
        protected CommModuleBase m_CommModule = null;

        #endregion Protected Properties

        #region Private Methods

        /// <summary>
		/// This method decipher the error byte to determine which non-fatal or
		/// fatal errors are set. 
		/// </summary>
		/// <param name="errorType">The type of error byte passed in</param>
		/// <param name="byError">The error byte to translate</param>
		/// <param name="strErrorList">List to put the error strings in</param>
		/// <param name="iErrorCount">Count of errors in the list</param>		
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/24/06 mrj 7.30.00 N/A    Created
		// 	
		private void GetErrors(ErrorType errorType, byte byError, ref string[] strErrorList, ref int iErrorCount)
		{
			if (ErrorType.NON_FATAL_1 == errorType)
			{
				if (NON_FATAL_1_MASK == (byte)(byError & NON_FATAL_1_MASK))
				{
					//Low battery
					strErrorList[iErrorCount++] = m_rmStrings.GetString("NON_FATAL_1");
				}
				if (NON_FATAL_2_MASK == (byte)(byError & NON_FATAL_2_MASK))
				{
					//Loss of phase
					strErrorList[iErrorCount++] = m_rmStrings.GetString("NON_FATAL_2");
				}
				if (NON_FATAL_3_MASK == (byte)(byError & NON_FATAL_3_MASK))
				{
					//Clock, TOU error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("NON_FATAL_3");
				}
				if (NON_FATAL_4_MASK == (byte)(byError & NON_FATAL_4_MASK))
				{
					//Reverse power flow
					strErrorList[iErrorCount++] = m_rmStrings.GetString("NON_FATAL_4");
				}
				if (NON_FATAL_5_MASK == (byte)(byError & NON_FATAL_5_MASK))
				{
					//Load profile error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("NON_FATAL_5");
				}
				if (NON_FATAL_6_MASK == (byte)(byError & NON_FATAL_6_MASK))
				{
					//Full scale overflow
					strErrorList[iErrorCount++] = m_rmStrings.GetString("NON_FATAL_6");
				}
			}
			else if (ErrorType.NON_FATAL_2 == errorType)
			{
				if (NON_FATAL_9_MASK == (byte)(byError & NON_FATAL_9_MASK))
				{
					//SiteScan Error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("NON_FATAL_9");
				}
			}
			else if (ErrorType.FATAL == errorType)
			{
				if (FATAL_1_MASK == (byte)(byError & FATAL_1_MASK))
				{
					//Processor Flash Error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_1");
				}
				if (FATAL_2_MASK == (byte)(byError & FATAL_2_MASK))
				{
					//Processor RAM Error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_2");
				}
				if (FATAL_3_MASK == (byte)(byError & FATAL_3_MASK))
				{
					//Data Flash Error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_3");
				}
				if (FATAL_4_MASK == (byte)(byError & FATAL_4_MASK))
				{
					//Metrology Communications Error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_4");
				}
				if (FATAL_5_MASK == (byte)(byError & FATAL_5_MASK))
				{
					//Power Down Error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_5");
				}
				if (FATAL_6_MASK == (byte)(byError & FATAL_6_MASK))
				{
					//File System Error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_6");
				}
				if (FATAL_7_MASK == (byte)(byError & FATAL_7_MASK)
					|| FATAL_8_MASK == byError) // Fatal Error Present but no error is specified
				{
					//Operating System Error
					strErrorList[iErrorCount++] = m_rmStrings.GetString("FATAL_7");
				}
			}
		}

		/// <summary>Executes standard and manufacturer procedures via standard
		/// tables 7 and 8. This Method waits until the meter posts an actual response to the procedure.</summary>
		/// <param name="Proc">Standard or MFG procedure to execute</param>
		/// <param name="Parameters">Procedure parameters</param>
		/// <param name="ResponseData"></param>
		/// <returns>A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.</returns>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  03/13/09 RCG 2.10.07 N/A    Created
        //
		protected ProcedureResultCodes ExecuteProcedureAndWaitForResult(Procedures Proc,
														 byte[] Parameters,
														 out byte[] ResponseData)
		{
			ProcedureResultCodes Result = ProcedureResultCodes.NOT_FULLY_COMPLETED;
			PSEMResponse PSEMResult = PSEMResponse.Err;
			int RetryDelay = 1000;

			// To satisfy compiler error
			ResponseData = null;

			//Setup table 7 for the write
			Table07.Procedure = (ushort)Proc;
			Table07.ParameterData = Parameters;

			//Send the procedure to the meter
			PSEMResult = Table07.Write();

			// TEMP CODE!!! (This seems to be a good thing.  This makes the operation look just like ITRON+)
			SendWait();

			while ((PSEMResponse.Ok == PSEMResult) &&
				  (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
				   ProcedureResultCodes.TIMING_CONSTRAINT == Result))
			{
				//Read the table 8 response
				PSEMResult = Table08.Read();

				if (PSEMResponse.Ok == PSEMResult)
				{
					//Get the result code
					Result = Table08.ResultCode;

					//Get the result data
					ResponseData = new byte[Table08.ResultData.Length];
					Array.Copy(Table08.ResultData, 0, ResponseData, 0, ResponseData.Length);

					if (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
						ProcedureResultCodes.TIMING_CONSTRAINT == Result)
					{
						// TEMP CODE!!!  (Don't know if this is necessary)
						SendWait();
						Thread.Sleep(RetryDelay);
					}
				}
				else if (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
						 ProcedureResultCodes.TIMING_CONSTRAINT == Result)
				{
					// TEMP CODE!!!  (Don't know if this is necessary)
					SendWait();
					Thread.Sleep(RetryDelay);
				}
			}

			return Result;
		}

        /// <summary>Executes standard and manufacturer procedures via standard
        /// tables 7 and 8. This Method waits until the meter posts an actual response to the procedure.</summary>
        /// <param name="ResponseData"></param>
        /// <param name="maxWaitTime"></param>
        /// <returns>A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.</returns>
        public ProcedureResultCodes WaitForResultOfExecutedProcedure(out byte[] ResponseData, TimeSpan? maxWaitTime = null)
        {
            ProcedureResultCodes Result = ProcedureResultCodes.NOT_FULLY_COMPLETED;
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            int RetryDelay = 1000;
            DateTime StartOfWait = DateTime.Now;

            // To satisfy compiler error
            ResponseData = null;

            // TEMP CODE!!! (This seems to be a good thing.  This makes the operation look just like ITRON+)
            //SendWait();

            while ( (null == maxWaitTime || (DateTime.Now - StartOfWait) < maxWaitTime) &&
                    (PSEMResponse.Ok == PSEMResult) &&
                  (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
                   ProcedureResultCodes.TIMING_CONSTRAINT == Result))
            {
                //Read the table 8 response
                PSEMResult = Table08.Read();

                if (PSEMResponse.Ok == PSEMResult)
                {
                    //Get the result code
                    Result = Table08.ResultCode;

                    //Get the result data
                    ResponseData = new byte[Table08.ResultData.Length];
                    Array.Copy(Table08.ResultData, 0, ResponseData, 0, ResponseData.Length);

                    if (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
                        ProcedureResultCodes.TIMING_CONSTRAINT == Result)
                    {
                        // TEMP CODE!!!  (Don't know if this is necessary)
                        SendWait();
                        Thread.Sleep(RetryDelay);
                    }
                }
                else if (ProcedureResultCodes.NOT_FULLY_COMPLETED == Result ||
                         ProcedureResultCodes.TIMING_CONSTRAINT == Result)
                {
                    // TEMP CODE!!!  (Don't know if this is necessary)
                    SendWait();
                    Thread.Sleep(RetryDelay);
                }
            }

            return Result;
        }

        /// <summary>
        /// The PasswordReconfigResult reconfigures passwords via standard table 42.
        /// </summary>
        /// <param name="Passwords">A list of passwords to write to the meter. 
        /// The Primary password should be listed first followed by the secondary
        /// password and so on.  Use empty strings for null passwords.  Passwords
        /// will be truncated or null filled as needed to fit in the device.</param>
        /// <returns>A PasswordReconfigResult object</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 08/16/06 mcm 7.35.00 N/A    Created
        /// 02/22/07 RCG 8.00.13        Changed to use the PasswordIndex enum
        ///	
        protected PasswordReconfigResult STDReconfigurePasswords(
							System.Collections.Generic.List<string> Passwords)
		{
			PasswordReconfigResult Result = PasswordReconfigResult.ERROR;
			ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
			PSEMResponse ProtocolResponse = PSEMResponse.Err;
			byte[] byPassword = new byte[SIZE_OF_ANSI_PASSWORD];
			byte[] byParameter = new byte[1];
			byte[] ProcResponse;

			// We have to clear the passwords because the meter rejects duplicates,
			// so if our new set of passwords contain one of the old passwords this 
			// will still work.
			byParameter[0] = 0;
			ProcResult = ExecuteProcedure(Procedures.RESET_PASSWORDS, byParameter,
				out ProcResponse);

			switch (ProcResult)
			{
				case ProcedureResultCodes.COMPLETED:
					{
						//Success
						ProtocolResponse = PSEMResponse.Ok;
						break;
					}
				case ProcedureResultCodes.NO_AUTHORIZATION:
					{
						//Isc error
						ProtocolResponse = PSEMResponse.Isc;
						break;
					}
				default:
					{
						//General Error
						ProtocolResponse = PSEMResponse.Err;
						break;
					}
			}

			// No need to write null passwords since we cleared them.
			if ((PSEMResponse.Ok == ProtocolResponse) &&
				(1 <= Passwords.Count) && (0 < Passwords[0].Length))
			{
				NullFillPassword(Passwords[0], SIZE_OF_ANSI_PASSWORD, ref byPassword);
				ProtocolResponse = m_PSEM.OffsetWrite(STANDARD_SECURITY_TABLE,
					(SIZE_OF_ANSI_PASSWORD + 1) * (int)ANSIStdPasswordIndex.PRIMARY, byPassword);
			}
			if ((PSEMResponse.Ok == ProtocolResponse) &&
				(2 <= Passwords.Count) && (0 < Passwords[1].Length))
			{
				NullFillPassword(Passwords[1], SIZE_OF_ANSI_PASSWORD, ref byPassword);
				ProtocolResponse = m_PSEM.OffsetWrite(STANDARD_SECURITY_TABLE,
					(SIZE_OF_ANSI_PASSWORD + 1) * (int)ANSIStdPasswordIndex.LIMITED_RECONFIG, byPassword);
			}
			if ((PSEMResponse.Ok == ProtocolResponse) &&
				(3 <= Passwords.Count) && (0 < Passwords[2].Length))
			{
				NullFillPassword(Passwords[2], SIZE_OF_ANSI_PASSWORD, ref byPassword);
				ProtocolResponse = m_PSEM.OffsetWrite(STANDARD_SECURITY_TABLE,
				   (SIZE_OF_ANSI_PASSWORD + 1) * (int)ANSIStdPasswordIndex.SECONDARY, byPassword);
			}
			if ((PSEMResponse.Ok == ProtocolResponse) &&
				(4 <= Passwords.Count) && (0 < Passwords[3].Length))
			{
				NullFillPassword(Passwords[3], SIZE_OF_ANSI_PASSWORD, ref byPassword);
				ProtocolResponse = m_PSEM.OffsetWrite(STANDARD_SECURITY_TABLE,
					(SIZE_OF_ANSI_PASSWORD + 1) * (int)ANSIStdPasswordIndex.TERTIARY, byPassword);
			}

			// Translate Protocol result
			if (PSEMResponse.Ok == ProtocolResponse)
			{
				Result = PasswordReconfigResult.SUCCESS;
			}
			else if (PSEMResponse.Isc == ProtocolResponse)
			{
				Result = PasswordReconfigResult.SECURITY_ERROR;
			}
			else // (PSEMResponse. == ProtocolResponse)
			{
				Result = PasswordReconfigResult.PROTOCOL_ERROR;
			}

			return Result;

		} // STDReconfigurePasswords

		/// <summary>
		/// Reconfigures the tertiary password using the standard tables
		/// </summary>
		/// <param name="strTertiaryPassword">The password to reconfigure.</param>
		/// <returns>A the result of the reconfigure.</returns>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 02/22/07 RCG 8.00.13    N/A Created
        //
		protected PasswordReconfigResult STDReconfigureTertiaryPassword(string strTertiaryPassword)
		{
			PasswordReconfigResult ReconfigResult = PasswordReconfigResult.ERROR;
			PSEMResponse Response = PSEMResponse.Err;
			byte[] byPassword = new byte[SIZE_OF_ANSI_PASSWORD];

			NullFillPassword(strTertiaryPassword, SIZE_OF_ANSI_PASSWORD, ref byPassword);
			Response = m_PSEM.OffsetWrite(STANDARD_SECURITY_TABLE,
				(SIZE_OF_ANSI_PASSWORD + 1) * (int)ANSIStdPasswordIndex.TERTIARY, byPassword);

			// Translate Protocol result
			if (PSEMResponse.Ok == Response)
			{
				ReconfigResult = PasswordReconfigResult.SUCCESS;
			}
			else if (PSEMResponse.Isc == Response)
			{
				ReconfigResult = PasswordReconfigResult.SECURITY_ERROR;
			}
			else // (PSEMResponse. == Response)
			{
				ReconfigResult = PasswordReconfigResult.PROTOCOL_ERROR;
			}

			return ReconfigResult;
		}

        #endregion Private Methods

        #region Private Properties

        /// <summary>
        /// Gets the Table 71 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/20/06 KRC 8.00.00			Created
        //
        private StdTable71 Table71
		{
			get
			{
				if (null == m_Table71)
				{
					m_Table71 = new StdTable71(m_PSEM, Table00.StdVersion);
				}

				return m_Table71;
			}
		}

		/// <summary>
		/// Gets the Table 72 object (Creates it if needed)
		/// </summary>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//  06/04/10 AF  2.41.06        Created
		//
		private StdTable72 Table72
		{
			get
			{
				if (null == m_Table72)
				{
					m_Table72 = new StdTable72(m_PSEM, Table71);
				}

				return m_Table72;
			}
		}

		/// <summary>
		/// Gets the Table 73 object (Creates it if needed)
		/// </summary>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//  06/04/10 AF  2.41.06        Added std table 72 parameter to constructor
		//
		private StdTable73 Table73
		{
			get
			{
				if (null == m_Table73)
				{
					m_Table73 = new StdTable73(m_PSEM, Table72, Table71, Table00);
				}

				return m_Table73;
			}
		}

		/// <summary>
		/// Gets the Table 74 object (Creates it if needed)
		/// </summary>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 11/20/06 KRC 8.00.00			Created
		// 12/04/13 DLG 3.50.10 WR422624 Changed from private to protected.
        //
		protected StdTable74 Table74
		{
			get
			{
				if (null == m_Table74)
				{
					m_Table74 = new StdTable74(m_PSEM, Table71, EventDescriptions);
				}

				return m_Table74;
			}
		}

        /// <summary>
        /// Gets the Table 75 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Added std table 72 parameter to constructor
        //
        private StdTable75 Table75
        {
            get
            {
                if (null == m_Table75)
                {
                    m_Table75 = new StdTable75(m_PSEM, Table72, Table71, Table00);
                }

                return m_Table75;
            }
        }
		/// <summary>
		/// Gets the Table 76 object (Creates it if needed)
		/// </summary>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/12/10 jrf 2.41.01 N/A    Created
		//
		private StdTable76 Table76
		{
			get
			{
				if (null == m_Table76)
				{
					m_Table76 = new StdTable76(m_PSEM, Table71, EventDescriptions);
				}

				return m_Table76;
			}
		}

        #endregion Private Properties

        #region Member Variables

        /// <summary>
		/// The ANSI Table 00 object, used for reading
		/// </summary>
		internal CTable00 m_Table0 = null;
		/// <summary>
		/// The ANSI Table 01 object, used for reading
		/// </summary>
		protected CTable01 m_Table1 = null;
        /// <summary>
		/// The ANSI Table 03 object, used for reading
		/// </summary>
		protected CTable03 m_Table3 = null;
		/// <summary>
		/// The ANSI Table 05 object, used for reading
		/// </summary>
		internal CTable05 m_Table5 = null;
		/// <summary>
		/// The ANSI Table 06 object, used for reading
		/// </summary>
		internal CTable06 m_Table6 = null;
		/// <summary>
		/// The C12.19 Standard Table 7, Procedure Initiate Table
		/// </summary>
		internal CTable07 m_Table7 = null;
		/// <summary>
		/// The C12.19 Standard Table 8, Procedure Response Table
		/// </summary>
		internal CTable08 m_Table8 = null;
		/// <summary>
		/// The MFG Table 2048 object, used for reading and writing
		/// </summary>
		internal CTable2048 m_Table2048 = null;
		/// <summary>
		/// The MFG Table 2054 (MeterKey) object
		/// </summary>
		internal CMeterKeyTable m_MeterKeyTable = null;
		/// <summary>
		/// The MFG Table 2084 (Multiple Custom Schedules) object, we only read
		/// the status.
		/// </summary>
		internal CTable2084 m_Table2084 = null;

		/// <summary>
		/// The PSEM protocol object.
		/// </summary>
		protected CPSEM m_PSEM = null;

		/// <summary>
		/// The LID retriever object used to get values from the meter via
		/// tables 2049 and 2050.
		/// </summary>
		internal LIDRetriever m_lidRetriever = null;

		/// <summary>
		/// LID object, the inherited device object creates the correct LIDs object
		/// </summary>
		internal DefinedLIDs m_LID = null;

		/// <summary>
		/// Debug file logger
		/// </summary>
		protected Logger m_Logger;

		/// <summary>
		/// Flag indicating whether or not the clock is running in the meter
		/// </summary>
		protected CachedBool m_ClockRunning;

		/// <summary>
		/// Counter for the Number of Times the meter has been programmed
		/// </summary>
		protected CachedInt m_NumTimesProgrammed;

		/// <summary>
		/// Counter for the Number of Outages the meter has been programmed
		/// </summary>
		protected CachedInt m_NumOutages;

		/// <summary>
		/// Date and Time of the Last Outage
		/// </summary>
		protected CachedDate m_DateLastOutage;

		/// <summary>
		/// Date and Time of the Last Calibration
		/// </summary>
		protected CachedDate m_DateLastCalibration;

		/// <summary>
		/// Date and Time of the meter last entered test mode
		/// </summary>
		protected CachedDate m_DateLastTest;

		/// <summary>
		/// Whether or not the meter is Canadian
		/// </summary>
		protected CachedBool m_IsCanadian;

		/// <summary>
		/// Whether or not the meter is sealed for Canada
		/// </summary>
		protected CachedBool m_IsSealedCanadian;

		/// <summary>
		/// Number of Minutes on Battery
		/// </summary>
		protected CachedUint m_MinutesOnBattery;

		/// <summary>
		/// Day of Week
		/// </summary>
		protected CachedString m_DayOfTheWeek;

		/// <summary>
		/// TOU Expiration Date
		/// </summary>
		protected CachedDate m_TOUExpireDate;

		/// <summary>
		/// Number of TOU Rates
		/// </summary>
		protected CachedUint m_uiNumTOURates;

		/// <summary>
		/// Number of Energy Registers in the meter
		/// </summary>
		protected CachedUint m_uiNumEnergies;

		/// <summary>
		/// Number of Demand registers in the meter
		/// </summary>
		protected CachedUint m_uiNumDemands;

		/// <summary>
		/// Firmware Build Number
		/// </summary>
		protected CachedUint m_uiFWBuild;

		/// <summary>
		/// Is Meter in DST
		/// </summary>
		protected CachedBool m_MeterInDST;

		/// <summary>
		/// Whether or not the meter supports DST
		/// </summary>
		protected CachedBool m_bDSTEnabled;

		/// <summary>
		/// The TOU Schedule object for the TOU Data
		/// </summary>
		protected CTOUSchedule m_TOUSchedule;

		/// <summary>
		/// The DST Dates in the meter.
		/// </summary>
		protected List<CDSTDatePair> m_lstDSTDates;

		/// <summary>
		/// Event Description Dictionary
		/// </summary>
		protected ANSIEventDictionary m_dicEventDescriptions;

        internal static readonly string RESOURCE_FILE_PROJECT_STRINGS =
									"Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";
        internal static System.Resources.ResourceManager m_rmStrings = 
            new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, typeof(Itron.Metering.Device.CANSIDevice).Assembly);

        private StdTable51 m_Table51 = null;
        private StdTable52 m_Table52 = null;
        private StdTable53 m_Table53 = null;
		private StdTable71 m_Table71 = null;
		private StdTable72 m_Table72 = null;
		private StdTable73 m_Table73 = null;
		private StdTable74 m_Table74 = null;
        private StdTable75 m_Table75 = null;
		private StdTable76 m_Table76 = null;

        private OpenWayCommModule_2064 m_Table2064 = null;
        /// <summary>Table 2129 object</summary>
        private OpenWayMFGTable2193 m_Table2193 = null;
        private OpenWayMFGTable2428 m_Table2428 = null;

		#endregion Member Variables

        #region ICreateEDL implementation

        /// <summary>
        /// Creates an EDL file that contains all of the data for a meter.
        /// </summary>
        /// <param name="FileName">The file name that will be used to store the file.</param>
        /// <returns>CreateEDLResult code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created
        // 07/06/10 AF  2.42.02         Made virtual for use in the M2 Gateway
        // 12/12/11 RCG 2.53.20			Adding support for Instrumentation Profile Data
        //
        public virtual CreateEDLResult CreateEDLFromMeter(string FileName)
        {
            EDLSections AllSections = EDLSections.HistoryLog
                | EDLSections.LoadProfile
                | EDLSections.NetworkTables
                | EDLSections.VoltageMonitoring
                | EDLSections.LANandHANLog
                | EDLSections.InstrumentationProfile;

            return CreateEDLFromMeter(FileName, AllSections);
        }

        /// <summary>
        /// Creates an EDL file with the specified sections.
        /// </summary>
        /// <param name="fileName">Path to the file where the EDL file will be written.</param>
        /// <param name="includedSections">The sections to include in the EDL file.</param>
        /// <returns>CreateEDLResult Code.</returns>
        public virtual CreateEDLResult CreateEDLFromMeter(string fileName, EDLSections includedSections)
        {
            return CreateEDLFromMeter(fileName, includedSections, null);
        }

        /// <summary>
        /// Creates an EDL file with the specified sections.
        /// </summary>
        /// <param name="fileName">Path to the file where the EDL file will be written.</param>
        /// <param name="includedSections">The sections to include in the EDL file.</param>
        /// <param name="logoffAndLogon">Method to logoff and log back on to meter.</param>
        /// <returns>CreateEDLResult Code.</returns>
            // Revision History	
            // MM/DD/YY who Version Issue# Description
            // -------- --- ------- ------ ---------------------------------------
            // 04/25/08 RCG	1.50.19			Created
            // 07/06/10 AF  2.42.02         Made virtual for use in the M2 Gateway
            // 01/30/12 jrf	2.53.35 192864  Modified to use new method, DoesTableHaveLength().
            // 02/19/13 jrf 2.70.68 288152  Calling two procedures before creating EDL to improve the
            //                              accuracy of error data from standard table 3.
            // 04/18/13 jrf 2.70.74 288152  Modified to only call clear status procedures when std.
            //                              table 3 did not accurately report errors and then added 
            //                              check to make sure std table 3 bits were set again if 
            //                              appropriate before reading that table for EDL.
            // 03/14/14 AF  3.50.49 464163  Added a progress bar to the early steps of this method. It was taking a long
            //                              time to get through GetTablesToRead() and there was no indication of activity.
            // 02/23/16 AF  4.50.232 236192 Added MeterTables.IsTableKnown to make sure we aren't using a table known to the register but
            //                              unknown to the CE dll
            //
        public virtual CreateEDLResult CreateEDLFromMeter(string fileName, EDLSections includedSections, Func<bool> logoffAndLogon)
        {
            List<ushort> TablesToRead;
            int iFileNameStart;
            string strDirectory;
            CreateEDLResult Result = CreateEDLResult.SUCCESS;
            PSEMResponse PSEMResult = PSEMResponse.Err;
            List<ushort> TablesToReadSeparately = new List<ushort>();
            List<ushort> TablesReadComplete = new List<ushort>();
            CentronTables MeterTables = new CentronTables();

            try
            {
                OnShowProgress(new ShowProgressEventArgs(1, 2, "Determining tables to read...", "Determining tables to read..."));

                // First check to make sure we can create the file
                iFileNameStart = fileName.LastIndexOf(@"\", StringComparison.Ordinal);

                if (iFileNameStart > 0)
                {
                    strDirectory = fileName.Substring(0, iFileNameStart);

                    if (Directory.Exists(strDirectory) == false)
                    {
                        Result = CreateEDLResult.INVALID_PATH;
                    }
                }

                // Make sure we will be able to write to the file
                if (Result == CreateEDLResult.SUCCESS && File.Exists(fileName) == true)
                {
                    FileInfo OutputFile = new FileInfo(fileName);

                    if ((OutputFile.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        Result = CreateEDLResult.INVALID_PATH;
                    }
                }

                if (Result == CreateEDLResult.SUCCESS)
                {
                    bool blnRereadTable3 = false;
                    string[] astrLIDNonFatalErrorsList = null;

                    // Standard table 3 is not supported in the I-210 or kV2c, so don't run this code on meters that don't support it
                    if (Table00.IsTableUsed(3) && Table00.IsProcedureUsed((ushort)Procedures.CLEAR_STD_STATUS) && Table00.IsProcedureUsed((ushort)Procedures.CLEAR_MFG_STATUS))
                    {
                        //So in EDL file we get non-fatal and fatal error info from table 3.  Problem is 
                        //that these status bits are sticky (may remain set when error is no longer present).
                        if (StdTable3ErrorsList.Length > ErrorsList.Length)
                        {
                            //Store the non-fatal error lid values as base for comparison
                            astrLIDNonFatalErrorsList = LIDNonFatalErrorsList;

                            //The following two procedures should clear the sticky status bits if they are no longer present.
                            ClearStandardStatus();
                            ClearManufacturerStatus();

                            //Problem is though once cleared the non-fatal error bits may take a while 
                            //before showing back up in std. table 3.     
                            if (StdTable3NonFatalErrorsList.Length < astrLIDNonFatalErrorsList.Length)
                            {
                                //If this is the case then we will have a check after reading all other tables
                                //at then end to wait on the non-fatal errors to sync back up and then 
                                //reread table 3.
                                blnRereadTable3 = true;
                            }
                        }
                    }

                    OnStepProgress(new ProgressEventArgs());

                    // Read the data from the meter
                    TablesToRead = GetTablesToRead(includedSections);

                    OnHideProgress(new EventArgs());

                    OnShowProgress(new ShowProgressEventArgs(1, TablesToRead.Count, "Creating EDL file...", "Creating EDL file..."));

                    if (null != logoffAndLogon)
                    {
                        //Separate out profile data tables (LP, VM, IP, ELP) to read in a separate read. When full these tables are
                        //causing timeouts reading them (Bug 1565113). Separating these tables out and reading them last seems to help.
                        if (TablesToRead.Contains(64)) { TablesToRead.Remove(64); TablesToReadSeparately.Add(64);}
                        if (TablesToRead.Contains(2157)) { TablesToRead.Remove(2157); TablesToReadSeparately.Add(2157);}
                        if (TablesToRead.Contains(2412)) { TablesToRead.Remove(2412); TablesToReadSeparately.Add(2412);}
                        if (TablesToRead.Contains(2413)) { TablesToRead.Remove(2413); TablesToReadSeparately.Add(2413);}
                    }

                    int Retries = CREATE_EDL_RETRIES;
                    //Adding in retries to help out when meter times out or does not reply with the correct PSEM responses (Bug 1565113).
                    for (int Retry = 0; Retry < Retries && PSEMResponse.Ok != PSEMResult; Retry++)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                            "Create EDL Attempt #" + (Retry + 1).ToString(CultureInfo.InvariantCulture));
                        try
                        {
                            PSEMResult = ReadEDLTables(TablesToRead, ref TablesReadComplete, ref MeterTables);

                            //A logoff and logon seems to help before reading the profile data tables (Bug 1565113).
                            if (PSEMResponse.Ok == PSEMResult && null != logoffAndLogon && logoffAndLogon())
                            {
                                PSEMResult = ReadEDLTables(TablesToReadSeparately, ref TablesReadComplete, ref MeterTables);
                            }
                        }
                        catch
                        {
                            PSEMResult = PSEMResponse.Err;
                        }
                        finally
                        {
                            if (PSEMResponse.Ok != PSEMResult)
                            {
                                bool LoggedOn = false;
                                for (int i = 0; i < CREATE_EDL_LOGON_RETRIES && false == LoggedOn; i++)
                                {
                                    m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                                        "Logon Attempt #" + (i + 1).ToString(CultureInfo.InvariantCulture));
                                    if (null != logoffAndLogon && false == logoffAndLogon())
                                    {
                                        if (i + 1 < CREATE_EDL_LOGON_RETRIES) Thread.Sleep(60000);
                                    }
                                    else { LoggedOn = true; }
                                }
                            }
                        }

                    }

                //Special check to make sure non-fatal errors in table 3 sync back up with what is reported via LIDs 
                //before committing std. table 3 to the EDL file.
                if (PSEMResult == PSEMResponse.Ok && true == blnRereadTable3)
                    {
                        int iMaxWait = 30; //seconds
                        bool blnKeepWaiting = (StdTable3NonFatalErrorsList.Length < astrLIDNonFatalErrorsList.Length);

                        for (int i = 0; (i < iMaxWait && blnKeepWaiting); i = i + 2)
                        {
                            //Recheck every 2 seconds
                            System.Threading.Thread.Sleep(2000);

                            blnKeepWaiting = (StdTable3NonFatalErrorsList.Length < astrLIDNonFatalErrorsList.Length);
                        }

                        PSEMResult = ReadTable(3, ref MeterTables);
                    }
                    if (PSEMResult == PSEMResponse.Isc)
                    {
                        Result = CreateEDLResult.SECURITY_ERROR;
                    }
                    else if (PSEMResult != PSEMResponse.Ok)
                    {
                        Result = CreateEDLResult.PROTOCOL_ERROR;
                    }
                }

                // Generate the EDL file
                if (Result == CreateEDLResult.SUCCESS)
                {
                    XmlWriterSettings WriterSettings = new XmlWriterSettings();
                    WriterSettings.Encoding = Encoding.ASCII;
                    WriterSettings.Indent = true;
                    WriterSettings.CheckCharacters = false;

                    XmlWriter EDLWriter = XmlWriter.Create(fileName, WriterSettings);

                    MeterTables.SaveEDLFile(EDLWriter, null, AllowTableExport, AllowFieldExport);
                }

                OnHideProgress(new EventArgs());
            }
            finally
            {
                OnHideProgress(new EventArgs());
            }

            return Result;
        }

        #endregion ICreateEDL implementation

        /// <summary>
        /// Reads a group of tables and adds them to a CentronTables object.
        /// </summary>
        /// <param name="tablesToRead">List of tables to read.</param>
        /// <param name="tablesReadComplete">List of tables where read is successful.</param>
        /// <param name="meterTables">CentronTables object that contains table data.</param>
        /// <returns>PSEMResponse of table last table read.</returns>
        private PSEMResponse ReadEDLTables(List<ushort> tablesToRead, ref List<ushort> tablesReadComplete, ref CentronTables meterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;

            try
            {
                foreach (ushort TableID in tablesToRead)
                {
                    if (PSEMResult == PSEMResponse.Ok && false == tablesReadComplete.Contains((ushort)TableID))
                    {
                        // Read the table if it exists
                        if ((Table00.IsTableUsed(TableID) == true) && (meterTables.IsTableKnown(TableID)))
                        {
                            if (meterTables.GetTableDependencies(TableID).Contains(TableID) || DoesTableHaveLength(TableID, meterTables))
                            {
                                PSEMResult = ReadTable(TableID, ref meterTables);

                                if (PSEMResult == PSEMResponse.Bsy
                                    || PSEMResult == PSEMResponse.Dnr
                                    || PSEMResult == PSEMResponse.Iar
                                    || PSEMResult == PSEMResponse.Onp
                                    || PSEMResult == PSEMResponse.Err)
                                {
                                    // We can't read the table but we should be able to continue we just need to
                                    // clear out anything that is there.
                                    meterTables.ClearTable(TableID);
                                    PSEMResult = PSEMResponse.Ok;
                                }

                                if (PSEMResponse.Ok == PSEMResult)
                                {
                                    tablesReadComplete.Add((ushort)TableID);
                                }
                            }
                        }

                        OnStepProgress(new ProgressEventArgs());
                    }
                }
            }
            catch { PSEMResult = PSEMResponse.Err; }

            return PSEMResult;
        }
    }
    

    /// <summary>
    /// This structure represents a Network Statistic. 
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 08/15/06 KRC 7.35.00    N/A Created
    ///
    public class CStatistic
    {
        /// <summary>
        /// The Statistic Desciption
        /// </summary>
        public String m_strStatName;
        /// <summary>
        /// String representing the Statistic Value.
        /// </summary>
        public string m_strStatValue;

        private bool m_blnStdStat = false;

        /// <summary>
        /// Property to get and set Statistic Name
        /// </summary>
        /// <example>
        /// <code>
        /// CStatistic myStat = new CStatistic("Statistc  1", value);
        /// string strName = myStat.StatName;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 KRC N/A	 N/A	Creation of class 
        public string StatName
        {
            get
            {
                return m_strStatName;
            }
            set
            {
                m_strStatName = value;
            }
        }//StatName

        /// <summary>
        /// Property to get and set Statistic Value
        /// </summary>
        /// <example>
        /// <code>
        /// CStatistic myStat = new CStatistic("Statistc  1", value);
        /// string strName = myStat.Name;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 KRC N/A	 N/A	Creation of class 
        public string StatValue
        {
            get
            {
                return m_strStatValue;
            }
            set
            {
                m_strStatValue = value;
            }
        }//StatValue

        /// <summary>
        /// Property to get and set whether the statistic is a standard statistic.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 jrf 2.20.19 137693 Created.
        public bool IsStandard
        {
            get
            {
                return m_blnStdStat;
            }
            set
            {
                m_blnStdStat = value;
            }
        }
    }
}
