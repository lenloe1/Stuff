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
//                           Copyright © 2006 - 2008
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.IO;
using System.Globalization;
using Itron.Metering.Utilities;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.DeviceDataTypes;


namespace Itron.Metering.Device
{
	/// <summary>
	/// Class representing the VECTRON meter.
	/// </summary>
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------
	//  04/27/06 mrj 7.30.00  N/A	Created
	//  05/25/06 jrf 7.30.00  N/A	Modified
    //  04/02/07 AF  8.00.23  2814  Corrected the capitalization of the meter name
    //
    public partial class VECTRON : SCSDevice, ISiteScan
	{
		#region Constants
		
		// Constants
		private const int VECTRON_MAX_UPLOAD_SIZE = 256;
		private const int VECTRON_MAX_DOWNLOAD_SIZE = 16;

		private const float VEC_V_ANGLE_A = 0.0f;
		private const int VEC_SYSTEM_ERROR_LENGTH = 1;
		private const int VEC_TOOLBOX_DATA_LENGTH = 44;
		private const int VEC_SERIAL_NUMBER_LENGTH = 18;
		private const int VEC_DIAGNOSTIC_1_TO_4_COUNTS_LENGTH = 4;
		private const int VEC_DIAGNOSTIC_5_COUNTS_LENGTH = 4;
		private const int VEC_DIAGNOSTICS_COUNTS_LENGTH = 13;
		private const int VEC_DIAGNOSTICS_STATUS_LENGTH = 1;
		private const int VEC_SERVICE_TYPE_LENGTH = 1;
		private const int VEC_REG_MAPPINGS_LENGTH = 6;
		private const int VEC_REG_1_DEM_READ_LENGTH = 12;
		private const int VEC_REG_1_TOU_DEM_READ_LENGTH = 52;
		private const int VEC_REG_1_TOU_DEM_READ_RATE_A_OFFSET = 0;
		private const int VEC_REG_1_TOU_DEM_READ_RATE_B_OFFSET = 12;
		private const int VEC_REG_1_TOU_DEM_READ_RATE_C_OFFSET = 28;
		private const int VEC_REG_1_TOU_DEM_READ_RATE_D_OFFSET = 44;
		private const int VEC_REG_2_DEM_EN_READ_LENGTH = 8;
		private const int VEC_REG_2_TOU_DEM_EN_READ_LENGTH = 28;
		private const int VEC_REG_2_TOU_DEM_2_READ_LENGTH = 16;
		private const int VEC_REG_3_DEM_EN_READ_LENGTH = 8;
		private const int VEC_REG_4_DEM_EN_READ_LENGTH = 8;
		private const int VEC_SR_DATA_BLOCK_LENGTH = 116;
		private const int VEC_DEM_READING_LENGTH = 8;
		private const int VEC_ENERGY_READING_LENGTH = 7;
		private const int VEC_REG_1_READ_RATE_E_MAX_OFFSET = 4;
		private const int VEC_REG_2_READ_RATE_E_ENERGY_OFFSET = 1;
		private const int VEC_REG_2_READ_RATE_B_MAX_OFFSET = 14;
		private const int VEC_REG_2_READ_2_RATE_D_MAX_OFFSET = 8;
		private const int VEC_MAX_TOU_RATES = 4;
		private const int VEC_DEM_READING_VALUE_LENGTH = 4;
		private const int VEC_DEM_READING_TOO_LENGTH = 4;
		private const int VEC_DEM_READING_TOO_MONTH_OFFSET = 4;
		private const int VEC_DEM_READING_TOO_DAY_OFFSET = 5;
		private const int VEC_DEM_READING_TOO_HOUR_OFFSET = 6;
		private const int VEC_DEM_READING_TOO_MINUTE_OFFSET = 7;
		private const int VEC_SR_BLOCK_TIME_MONTH_OFFSET = 0;
		private const int VEC_SR_BLOCK_TIME_DAY_OFFSET = 1;
		private const int VEC_SR_BLOCK_TIME_HOUR_OFFSET = 2;
		private const int VEC_SR_BLOCK_TIME_MINUTE_OFFSET = 3;
		private const int VEC_SR_REG_1_RATE_E_MAX_OFFSET = 4;
		private const int VEC_SR_REG_1_RATE_A_MAX_OFFSET = 60;
		private const int VEC_SR_REG_1_RATE_B_MAX_OFFSET = 67;
		private const int VEC_SR_REG_1_RATE_C_MAX_OFFSET = 74;
		private const int VEC_SR_REG_1_RATE_D_MAX_OFFSET = 81;
		private const int VEC_SR_REG_2_RATE_E_MAX_OFFSET = 11;
		private const int VEC_SR_REG_2_RATE_A_MAX_OFFSET = 88;
		private const int VEC_SR_REG_2_RATE_B_MAX_OFFSET = 98;
		private const int VEC_SR_REG_2_RATE_C_MAX_OFFSET = 102;
		private const int VEC_SR_REG_2_RATE_D_MAX_OFFSET = 109;
		private const int VEC_SR_REG_2_RATE_E_ENERGY_OFFSET = 11;
		private const int VEC_SR_REG_2_RATE_A_ENERGY_OFFSET = 88;
		private const int VEC_SR_REG_3_RATE_E_MAX_OFFSET = 18;
		private const int VEC_SR_REG_3_RATE_E_ENERGY_OFFSET = 18;
		private const int VEC_SR_REG_4_RATE_E_MAX_OFFSET = 25;
		private const int VEC_SR_REG_4_RATE_E_ENERGY_OFFSET = 25;
		private const int VEC_SR_DEM_READING_VALUE_LENGTH = 3;
		private const int VEC_SR_ENERGY_READING_LENGTH = 3;
		private const int VEC_SR_ENERGY_READING_RESV_LENGTH = 7;
		private const string VEC_WATTS = "Watts Delivered";
		private const string VEC_KWH = "kWh";
		private const string VEC_KW = "kW";
		private const string VEC_MAX_KW = "max kW";
		private const string VEC_CUM_KW = "cum kW";
		private const string VEC_CCUM_KW = "ccum kW";
		private const string VEC_VAR_RECEIVED = "Var Received";
		private const string VEC_KVARH = "kvarh";
		private const string VEC_MAX_KVAR = "max kvar";
		private const string VEC_CUM_KVAR = "cum kvar";
		private const string VEC_CCUM_KVAR = "ccum kvar";
		private const string VEC_KVAR = "kvar";
		private const string VEC_VAR_DELIVERED = "Var Delivered";
		private const string VEC_VA_RECEIVED = "VA Received";
		private const string VEC_KVAH = "kVAh";
		private const string VEC_MAX_KVA = "max kVA";
		private const string VEC_CUM_KVA = "cum kVA";
		private const string VEC_CCUM_KVA = "ccum kVA";
		private const string VEC_KVA = "kVA";
		private const string VEC_VA_TOTAL = "VA Total";
		private const string VEC_RATE_A = "Rate A";
		private const string VEC_RATE_B = "Rate B";
		private const string VEC_RATE_C = "Rate C";
		private const string VEC_RATE_D = "Rate D";
		private const string VEC_MAX = "max";
		private const string VEC_LAG = "lag";
		private const string VEC_LEAD = "lead";
		private const string VEC_PEAK_2 = "Peak 2";

		private const byte VA_CALCULATION_MASK = 0x08;

        private const string VECTRON_TYPE = "VECTRON";
        private const string VECTRON_NAME = "VECTRON";

		#endregion Constants

		#region Definitions

		/// <summary>
		/// VECAddresses enumeration encapsulates the VECTRON basepage 
		/// addresses.
		/// </summary>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00 N/A	Created
		/// 08/15/06 mah 7.35.00 N/A    Added demand reset and reset billing flags
		/// 01/05/07 mah 8.00.00 N/A  Added numerous addresses for displayable values
		/// </remarks>
		private enum VECAddresses : int
		{
			HANGUP_FLAG = 0x1B03,
			PROCESSOR_REVISION = 0x1B05,
			INS_KW = 0x1B07,
			PRESENT_KW = 0x1B0B,
			TEST_MODE_KWH = 0x1B50,
			TEST_MODE_KW = 0x1B54,
			TEST_MODE_KVARH_LAG = 0x1B5D,
			TEST_MODE_KVAR_LAG = 0x1B61,
			TEST_MODE_KVARH_LEAD = 0x1B65,
			TEST_MODE_KVAH_LAG = 0x1B69,
			TEST_MODE_KVA_LAG = 0x1B6D,
			TEST_MODE_KVA_TOTAL = 0x1B71,
			PRESENT_KVAR_LAG = 0x1B75,
			PRESENT_KVA_LAG = 0x1B79,
			PRESENT_KVA_TOTAL = 0x1B7D,
			TOOLBOX_DATA = 0x1B1F,
			DIAGNOSTICS_STATUS = 0x1B58,
			SERVICE_TYPE = 0x1B8D,
			PRIMARY_PASSWORD = 0x1D00,
			SECONDARY_PASSWORD = 0x1D08,
			TERTIARY_PASSWORD = 0x1D10,
			UNIT_ID = 0x1D1B,
			METER_CONFIGURATION = 0x1D23,
			DEMAND_THRESHOLD = 0x1D24,
			REGISTER_MULTIPLIER = 0x1D28,
			COMMUNICATIONS_TIMEOUT = 0x1D41,
			REGISTER_MAPPINGS = 0x1D98,
			FORM_FACTOR = 0x1F66,
			TEST_MODE_FLAG = 0x20E9,
			DEMAND_RESET_FLAG = 0x20EA,
			STOP_METER_FLAG = 0x20EC,
			CLEAR_BILLING_FLAG = 0x20ED,
			CLOCK_RUN_FLAG = 0x20EE,
			MM_RUN_FLAG = 0x20EF,
			TOU_RUN_FLAG = 0x20F0,
			CLOCK_RECONFIGURE_FLAG = 0x20F1,
			TOU_RECONFIGURE_FLAG = 0x20F3,
			MM_AJUST_TIME_FLAG = 0x20F4,
			SYSTEM_ERROR = 0x20F8,
			REAL_TIME = 0x20F9,
			DAY_OF_WEEK = 0x20FF,
			MODEL_TYPE = 0x2110,
			REG_2_RATE_E_MAX = 0x2118,
			REG_1_RATE_E_CUM = 0x2120,
			REG_1_RATE_E_MAX = 0x2124,
			LAST_RESET_DATE = 0x212C,
			LAST_RESET_TIME = 0x212E,
			RESET_COUNT = 0x2130,
			OUTAGE_COUNT = 0x2132,
			REG_3_RATE_E_CUM = 0x2134,
			REG_1_RATE_A_MAX = 0x2138,
			REG_4_RATE_E_CUM = 0x2140,
			REG_1_RATE_B_MAX = 0x2144,
			DAYS_SINCE_LAST_RESET = 0x214C,
			TRANSFORMER_RATIO = 0x214D,
			DISPLAY_OPTIONS = 0x214F,
			REG_1_RATE_C_MAX = 0x2154,
			REG_1_RATE_D_MAX = 0x2164,
			REGISTER_FULL_SCALE = 0x216C,
			TEST_MODE_TIMEOUT = 0x2170,
			REG_2_RATE_A_MAX = 0x2172,
			REG_2_RATE_E_CUM = 0x217A,
			REG_2_RATE_B_MAX = 0x2180,
			DEMAND_CONFIGURATION = 0x218E,
			SUBINT_LENGTH = 0x218F,
			TEST_SUBINT_LENGTH = 0x2190,
			OPERATING_SETUP = 0x2196,
			MINUTES_ON_BATTERY = 0x2197,
			PROGRAM_COUNT = 0x219A,
			LAST_PROGRAMMED_DATE = 0x219C,
			LAST_PROGRAMMED_TIME = 0x219E,
			REG_2_RATE_C_MAX = 0x21B0,
			REG_2_RATE_D_MAX = 0x21B8,
			NORMAL_KH = 0x21C1,
			COLD_LOAD_PICKUP_TIME = 0x21C6,
			KYZ_1_PULSE_WEIGHT = 0x21C8,
			TEST_MODE_KH = 0x21CC,
			REG_3_RATE_E_MAX = 0x21D0,
			REG_4_RATE_E_MAX = 0x21D8,
			DIAGNOSTICS_COUNTS = 0x21E0,
			DIAGNOSTIC_5_COUNTS = 0x21E9,
			SOFTWARE_REVISION = 0x2201,
			FIRMWARE_REVISION = 0x2203,
			USERDEFINED_FIELD1 = 0x2205,
			USERDEFINED_FIELD2 = 0x220E,
			USERDEFINED_FIELD3 = 0x2217,
			PROGRAM_ID = 0x2220,
			SERIAL_NUMBER = 0x2222,
			METER_ID_2 = 0x222B,
			ENERGYFORMAT_ADDRESS = 0x2234,
			FIRMWARE_OPTIONS = 0x223C,
			DISPLAY_TABLE = 0x223D,
			KYZ_2_PULSE_WEIGHT = 0x2237,
			LAST_SEASON_REGISTERS = 0x2460,
			LAST_SEASON_REG_3_RATE_E_MAX = 0x2461,
			LAST_SEASON_REG_2_RATE_E_MAX = 0x2481,
			LAST_SEASON_REG_1_RATE_E_MAX = 0x2484,
			LAST_SEASON_REG_2_RATE_A_MAX = 0x2487,
			LAST_SEASON_REG_1_RATE_A_MAX = 0x248A,
			LAST_SEASON_REG_2_RATE_B_MAX = 0x248D,
			LAST_SEASON_REG_1_RATE_B_MAX = 0x2490,
			LAST_SEASON_REG_2_RATE_C_MAX = 0x2493,
			LAST_SEASON_REG_1_RATE_C_MAX = 0x2496,
			LAST_SEASON_REG_2_RATE_D_MAX = 0x2499,
			LAST_SEASON_REG_1_RATE_D_MAX = 0x249C,
			LAST_SEASON_REG_1_RATE_E_CUM = 0x24B0,
			LAST_SEASON_REG_2_RATE_E_CUM = 0x24B3,
			LAST_SEASON_REG_3_RATE_E_CUM = 0x24B6,
			LAST_SEASON_REG_4_RATE_E_CUM = 0x24B9,
			LAST_SEASON_REG_4_RATE_E_MAX = 0x24BC,
			YEARLY_SCHEDULE = 0x24CB,
			TOU_EXPIRATION_DATE = 0x24D1,
			TOU_SCHEDULE_ID = 0x24D5,
			TOU_BASE = 0x24D7,
			MM_INTERVAL_LENGTH = 0x2504,
            LOAD_RESEARCH_ID = 0x250C,
			NUMBER_OF_CHANNELS = 0x2535,
			MODEM_PASSWORD = 0x274B,
			REG_1_RATE_A_CUM = 0x27C8,
			REG_1_RATE_B_CUM = 0x27CC,
			REG_1_RATE_C_CUM = 0x27D0,
			REG_1_RATE_D_CUM = 0x27D4,
			REG_2_RATE_A_CUM = 0x27D8,
			REG_2_RATE_B_CUM = 0x27DC,
			REG_2_RATE_C_CUM = 0x27E0,
			REG_2_RATE_D_CUM = 0x27E4,
			LAST_SEASON_REG_1_RATE_A_CUM = 0x27E8,
			LAST_SEASON_REG_1_RATE_B_CUM = 0x27EB,
			LAST_SEASON_REG_1_RATE_C_CUM = 0x27EE,
			LAST_SEASON_REG_1_RATE_D_CUM = 0x27F1,
			LAST_SEASON_REG_2_RATE_A_CUM = 0x27F4,
			LAST_SEASON_REG_2_RATE_B_CUM = 0x27F7,
			LAST_SEASON_REG_2_RATE_C_CUM = 0x27FA,
			LAST_SEASON_REG_2_RATE_D_CUM = 0x27FD,
			SR_DATA_BLOCK_1 = 0x2800,
			SR_DATA_BLOCK_2 = 0x2874,
			SR_DATA_BLOCK_3 = 0x28E8,
			SR_DATA_BLOCK_4 = 0x295C
		};

		/// <summary>
		/// VECErrors enumeration encapsulates the VECTRON system errors 
		/// bitmasks.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00 N/A	Created
		/// 
		private enum VECErrors : byte
		{
			LOW_BATTERY = 0x10,
			REGISTER_FULL_SCALE = 0x20,
			CLOCK_TOU_MM_ERROR = 0x40,
			REVERSE_POWER_FLOW = 0x80
		};

		/// <summary>
		/// VECDiagnostics enumeration encapsulates the VECTRON diagnostics 
		/// status bitmasks.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00 N/A	Created
		/// 
		private enum VECDiagnostics : byte
		{
			PHASOR_CHECK = 0x01,
			VOLTAGE_CHECK = 0x02,
			CURRENT_CHECK = 0x04,
			POWER_FACTOR_CHECK = 0x08,
			DC_DETECT = 0x10
		};

		/// <summary>
		/// VECDiagIndex enumeration encapsulates the VECTRON diagnostics 
		/// indicies based on a read of the diagnostic counts from the VECTRON.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00 N/A	Created
		/// 
		private enum VECDiagIndex : int
		{
			DIAG_1 = 0,
			DIAG_2 = 1,
			DIAG_3 = 2,
			DIAG_4 = 3,
			DIAG_5T = 9,
			DIAG_5A = 10,
			DIAG_5B = 11,
			DIAG_5C = 12
		};

		/// <summary>
		/// VECServiceType enumeration encapsulates the VECTRON service type
		/// identifiers.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00 N/A	Created
		/// 
		private enum VECServiceType : int
		{
			ID_1 = 0,
			ID_2 = 1,
			ID_3 = 2,
			ID_4 = 3,
			ID_5 = 4,
			ID_6 = 5,
			ID_7 = 6,
			ID_8 = 7
		};

		/// <summary>
		/// VECRegisters enumeration encapsulates the VECTRON registers which 
		/// are mapped to specific quantities.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/21/06 jrf 8.00.00 N/A	Created
		/// 
		private enum VECRegisters : int
		{
			NO_REG = -1,
			REG_1_RATE_E = 0,
			REG_1_TOU = 1,
			REG_2_RATE_E = 2,
			REG_2_TOU = 3,
			REG_3_RATE_E = 4,
			REG_4_RATE_E = 5
		};

		/// <summary>
		/// VECRegisterType enumeration encapsulates the VECTRON register type
		/// used, energy or demand.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/22/06 jrf 8.00.00 N/A	Created
		/// 
		private enum VECRegisterType : int
		{
			DEMAND = 0,
			ENERGY = 1
		};

		/// <summary>
		/// VECRegQuantities enumeration encapsulates the VECTRON register quantities
		/// which can be assigned to a particular register.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00 N/A	Created
		/// 
		private enum VECRegQuantities : int
		{
			NO_QTY = 0,
			WATT_HOUR = 1,
			VAR_HOUR_LAG = 2,
			VAR_HOUR_LEAD = 3,
			VA_HOUR_LAG = 4,
			WATT = 5,
			WATT_TOU = 6,
			VAR_LAG = 7,
			VA_LAG = 8,
			VA_TOTAL = 9
		};

		#endregion Definitions

		#region Public Methods

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="SerialPort">The communication object used for the 
		/// serial port communications.</param>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// VECTRON VECTRON = new VECTRON(Comm);
		/// </code>
		/// </example>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		/// </remarks>
		public VECTRON(ICommunications SerialPort)
			:
			base(SerialPort)
		{
			m_Toolbox = new Toolbox();
			m_Diag = new CDiagnostics(false);
			m_serviceType = new CachedString();
			m_MeterForm = new CachedInt();
			m_VACalculation = new CachedBool();

			m_eWhMapping = VECRegisters.NO_REG;
			m_eVarhLagMapping = VECRegisters.NO_REG;
			m_eVarhLeadMapping = VECRegisters.NO_REG;
			m_eVAhLagMapping = VECRegisters.NO_REG;
			m_eWMapping = VECRegisters.NO_REG;
			m_eWTOUMapping = VECRegisters.NO_REG;
			m_eVarLagMapping = VECRegisters.NO_REG;
			m_eVALagMapping = VECRegisters.NO_REG;
			m_eVATotalMapping = VECRegisters.NO_REG;
			m_blnRetreivedRegMapping = false;
			m_eRegQuantities = new VECRegQuantities[6];
			m_eRegQuantities[0] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[1] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[2] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[3] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[4] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[5] = VECRegQuantities.NO_QTY;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="protocol">The SCS protocol object used for 
		/// communications with SCS devices.</param>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// VECTRON VECTRON = new VECTRON(scsProtocol);
		/// </code>
		/// </example>
		/// <remarks >
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/09/06 jrf 7.30.00  N/A   Created
		/// 06/19/06 jrf 7.30.00  N/A   Changed to pass protocol object to base 
		///								Constructor
		/// </remarks>
		public VECTRON(SCSProtocol protocol)
			:
			base(protocol)
		{
			m_Toolbox = new Toolbox();
			m_Diag = new CDiagnostics(false);
			m_serviceType = new CachedString();
			m_MeterForm = new CachedInt();
			m_VACalculation = new CachedBool();

			m_eWhMapping = VECRegisters.NO_REG;
			m_eVarhLagMapping = VECRegisters.NO_REG;
			m_eVarhLeadMapping = VECRegisters.NO_REG;
			m_eVAhLagMapping = VECRegisters.NO_REG;
			m_eWMapping = VECRegisters.NO_REG;
			m_eWTOUMapping = VECRegisters.NO_REG;
			m_eVarLagMapping = VECRegisters.NO_REG;
			m_eVALagMapping = VECRegisters.NO_REG;
			m_eVATotalMapping = VECRegisters.NO_REG;
			m_blnRetreivedRegMapping = false;
			m_eRegQuantities = new VECRegQuantities[6];
			m_eRegQuantities[0] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[1] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[2] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[3] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[4] = VECRegQuantities.NO_QTY;
			m_eRegQuantities[5] = VECRegQuantities.NO_QTY;

			// Note that the VECTRON uses a different max packet size for uploads
			// and downloads
			protocol.MaxDownloadSize = VECTRON_MAX_DOWNLOAD_SIZE;
			protocol.MaxUploadSize = VECTRON_MAX_UPLOAD_SIZE;
		}

		#endregion Public Methods

		#region Public Properties

		/// <summary>
		/// Provides access to a list of measured quantities
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  11/27/06 jrf 8.00.00 N/A    Created
		///  12/08/06 jrf 8.00.00 N/A    Watts property was renamed to WattsDelivered
		/// </remarks>
		override public List<Quantity> CurrentRegisters
		{
			get
			{
				List<Quantity> QuantityList = new List<Quantity>();
				Quantity Qty;

				Qty = WattsDelivered;
				if (null != Qty)
				{
					QuantityList.Add(Qty);
				}

				Qty = VarReceived;
				if (null != Qty)
				{
					QuantityList.Add(Qty);
				}

				Qty = VarDelivered;
				if (null != Qty)
				{
					QuantityList.Add(Qty);
				}

				Qty = VAReceived;
				if (null != Qty)
				{
					QuantityList.Add(Qty);
				}

				Qty = VATotal;
				if (null != Qty)
				{
					QuantityList.Add(Qty);
				}

				return QuantityList;
			}
		}

		/// <summary>
		/// Proves access to a list of Self Reads.  
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  11/27/06 jrf 8.00.00 N/A    Created
		/// </remarks>
		override public List<QuantityCollection> SelfReadRegisters
		{
			get
			{
				List<QuantityCollection> SelfReads = new List<QuantityCollection>();

				QuantityCollection SelfReadBlock;

				SelfReadBlock = new QuantityCollection();
				ReadSelfReadBlock(VECAddresses.SR_DATA_BLOCK_1, ref SelfReadBlock);
				SelfReads.Add(SelfReadBlock);

				// For demand only meters, only one self read block is maintained.
				ReadModelType();
				if (SCSModelTypes.DemandOnly != m_Model)
				{
					SelfReadBlock = new QuantityCollection();
					ReadSelfReadBlock(VECAddresses.SR_DATA_BLOCK_2, ref SelfReadBlock);
					SelfReads.Add(SelfReadBlock);

					SelfReadBlock = new QuantityCollection();
					ReadSelfReadBlock(VECAddresses.SR_DATA_BLOCK_3, ref SelfReadBlock);
					SelfReads.Add(SelfReadBlock);

					SelfReadBlock = new QuantityCollection();
					ReadSelfReadBlock(VECAddresses.SR_DATA_BLOCK_4, ref SelfReadBlock);
					SelfReads.Add(SelfReadBlock);
				}

				return SelfReads;
			}
		}

		/// <summary>
		/// Provides access to the Watts Quantity
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  11/21/06 jrf 8.00.00 N/A    Adding support to get Watts
		///  12/08/06 jrf 8.00.00 N/A    Added TOU register checks for demand and energy values,
		///                              Corrected register that's read during TOU energy read,
		///                              and changed quantity name to WattsDelivered.
		///  01/05/07 mah 8.00.00 N/A Added support for cum and ccum values
		/// </remarks>
		public Quantity WattsDelivered
		{
			get
			{
				Quantity WattsQty = null;
				VECRegisters eEnergyRegister = VECRegisters.NO_REG;
				VECRegisters eDemandRegister = VECRegisters.NO_REG;

				ReadRegisterMappings();

				//Read Energy Values
				if (VECRegisters.NO_REG != m_eWhMapping)
				{
					WattsQty = new Quantity(VEC_WATTS);

					eEnergyRegister = m_eWhMapping;

					// If the quantity was mapped to TOU register then it 
					// also is mapped to a cooresponding non-tou register
					// Don't check register 1; it is demand only
					if (VECRegisters.REG_2_TOU == m_eWhMapping)
					{
						eEnergyRegister = VECRegisters.REG_2_RATE_E;
					}

					ReadRegisterData(eEnergyRegister, ref WattsQty, VECRegisterType.ENERGY);
					WattsQty.TotalEnergy.Description = VEC_KWH;
				}
				//Read Demand Values
				if (VECRegisters.NO_REG != m_eWMapping)
				{
					if (null == WattsQty)
					{
						WattsQty = new Quantity(VEC_WATTS);
					}

					eDemandRegister = m_eWMapping;

					if (VECRegisters.REG_1_TOU == m_eWMapping)
					{
						eDemandRegister = VECRegisters.REG_1_RATE_E;
					}
					else if (VECRegisters.REG_2_TOU == m_eWMapping)
					{
						eDemandRegister = VECRegisters.REG_2_RATE_E;
					}

					ReadRegisterData(eDemandRegister, ref WattsQty, VECRegisterType.DEMAND);
					WattsQty.TotalMaxDemand.Description = VEC_MAX_KW;

					if (WattsQty.CummulativeDemand != null)
					{
						WattsQty.CummulativeDemand.Description = VEC_CUM_KW;
					}

					if (WattsQty.ContinuousCummulativeDemand != null)
					{
						WattsQty.ContinuousCummulativeDemand.Description = VEC_CCUM_KW;
					}
				}
				if (TOUEnabled)
				{
					//Read TOU Energy Values
					if (VECRegisters.REG_2_TOU == m_eWhMapping)
					{
						ReadRegisterData(m_eWhMapping, ref WattsQty, VECRegisterType.ENERGY);
						WattsQty.TOUEnergy[0].Description = VEC_KWH + " " + VEC_RATE_A;
						WattsQty.TOUEnergy[1].Description = VEC_KWH + " " + VEC_RATE_B;
						WattsQty.TOUEnergy[2].Description = VEC_KWH + " " + VEC_RATE_C;
						WattsQty.TOUEnergy[3].Description = VEC_KWH + " " + VEC_RATE_D;
					}
					//Read TOU Demand Values
					if (VECRegisters.NO_REG != m_eWTOUMapping)
					{
						ReadRegisterData(m_eWTOUMapping, ref WattsQty, VECRegisterType.DEMAND);
						WattsQty.TOUMaxDemand[0].Description = VEC_MAX_KW + " " + VEC_RATE_A;
						WattsQty.TOUMaxDemand[1].Description = VEC_MAX_KW + " " + VEC_RATE_B;
						WattsQty.TOUMaxDemand[2].Description = VEC_MAX_KW + " " + VEC_RATE_C;
						WattsQty.TOUMaxDemand[3].Description = VEC_MAX_KW + " " + VEC_RATE_D;

						if (WattsQty.TOUCummulativeDemand != null)
						{
							WattsQty.TOUCummulativeDemand[0].Description = VEC_CUM_KW + " " + VEC_RATE_A;
							WattsQty.TOUCummulativeDemand[1].Description = VEC_CUM_KW + " " + VEC_RATE_B;
							WattsQty.TOUCummulativeDemand[2].Description = VEC_CUM_KW + " " + VEC_RATE_C;
							WattsQty.TOUCummulativeDemand[3].Description = VEC_CUM_KW + " " + VEC_RATE_D;
						}

						if (WattsQty.TOUCCummulativeDemand != null)
						{
							WattsQty.TOUCCummulativeDemand[0].Description = VEC_CCUM_KW + " " + VEC_RATE_A;
							WattsQty.TOUCCummulativeDemand[1].Description = VEC_CCUM_KW + " " + VEC_RATE_B;
							WattsQty.TOUCCummulativeDemand[2].Description = VEC_CCUM_KW + " " + VEC_RATE_C;
							WattsQty.TOUCCummulativeDemand[3].Description = VEC_CCUM_KW + " " + VEC_RATE_D;
						}

					}
				}
				return WattsQty;
			}
		}

		/// <summary>
		/// Provides access to the Var Received Quantity
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  11/27/06 jrf 8.00.00 N/A    Adding support to get Var Received
		///  12/08/06 jrf 8.00.00 N/A    Added "Lag" into the description
		///  01/05/07 mah 8.00.00 N/A Added support for cum and ccum values
		/// </remarks>
		public Quantity VarReceived
		{
			get
			{
				Quantity VarRec = null;
				VECRegisters eEnergyRegister = VECRegisters.NO_REG;
				VECRegisters eDemandRegister = VECRegisters.NO_REG;

				ReadRegisterMappings();

				//Read Energy Values
				if (VECRegisters.NO_REG != m_eVarhLagMapping)
				{
					VarRec = new Quantity(VEC_VAR_RECEIVED);

					eEnergyRegister = m_eVarhLagMapping;

					// If the quantity was mapped to TOU register then it 
					// also is mapped to a cooresponding non-tou register
					// Don't check register 1; it is demand only
					if (VECRegisters.REG_2_TOU == m_eVarhLagMapping)
					{
						eEnergyRegister = VECRegisters.REG_2_RATE_E;
					}
					ReadRegisterData(eEnergyRegister, ref VarRec, VECRegisterType.ENERGY);
					VarRec.TotalEnergy.Description = VEC_KVARH + " " + VEC_LAG;
				}
				//Read Demand Values
				if (VECRegisters.NO_REG != m_eVarLagMapping)
				{
					if (null == VarRec)
					{
						VarRec = new Quantity(VEC_VAR_RECEIVED);
					}

					eDemandRegister = m_eVarLagMapping;

					if (VECRegisters.REG_1_TOU == m_eVarLagMapping)
					{
						eDemandRegister = VECRegisters.REG_1_RATE_E;
					}
					else if (VECRegisters.REG_2_TOU == m_eVarLagMapping)
					{
						eDemandRegister = VECRegisters.REG_2_RATE_E;
					}

					ReadRegisterData(eDemandRegister, ref VarRec, VECRegisterType.DEMAND);
					VarRec.TotalMaxDemand.Description = VEC_MAX_KVAR + " " + VEC_LAG;

					if (VarRec.CummulativeDemand != null)
					{
						VarRec.CummulativeDemand.Description = VEC_CUM_KVAR;
					}

					if (VarRec.ContinuousCummulativeDemand != null)
					{
						VarRec.ContinuousCummulativeDemand.Description = VEC_CCUM_KVAR;
					}
				}
				if (TOUEnabled)
				{
					//Read TOU Energy Values                   
					if (VECRegisters.REG_2_TOU == m_eVarhLagMapping)
					{
						ReadRegisterData(m_eVarhLagMapping, ref VarRec, VECRegisterType.ENERGY);
						VarRec.TOUEnergy[0].Description = VEC_KVARH + " " + VEC_LAG + " " + VEC_RATE_A;
						VarRec.TOUEnergy[1].Description = VEC_KVARH + " " + VEC_LAG + " " + VEC_RATE_B;
						VarRec.TOUEnergy[2].Description = VEC_KVARH + " " + VEC_LAG + " " + VEC_RATE_C;
						VarRec.TOUEnergy[3].Description = VEC_KVARH + " " + VEC_LAG + " " + VEC_RATE_D;
					}

					//Read TOU Demand Values
					if (VECRegisters.REG_1_TOU == m_eVarLagMapping ||
						VECRegisters.REG_2_TOU == m_eVarLagMapping)
					{
						ReadRegisterData(m_eVarLagMapping, ref VarRec, VECRegisterType.DEMAND);
						VarRec.TOUMaxDemand[0].Description = VEC_MAX_KVAR + " " + VEC_LAG + " " + VEC_RATE_A;
						VarRec.TOUMaxDemand[1].Description = VEC_MAX_KVAR + " " + VEC_LAG + " " + VEC_RATE_B;
						VarRec.TOUMaxDemand[2].Description = VEC_MAX_KVAR + " " + VEC_LAG + " " + VEC_RATE_C;
						VarRec.TOUMaxDemand[3].Description = VEC_MAX_KVAR + " " + VEC_LAG + " " + VEC_RATE_D;

						if (VarRec.TOUCummulativeDemand != null)
						{
							VarRec.TOUCummulativeDemand[0].Description = VEC_CUM_KVAR + " " + VEC_LAG + " " + VEC_RATE_A;
							VarRec.TOUCummulativeDemand[1].Description = VEC_CUM_KVAR + " " + VEC_LAG + " " + VEC_RATE_B;
							VarRec.TOUCummulativeDemand[2].Description = VEC_CUM_KVAR + " " + VEC_LAG + " " + VEC_RATE_C;
							VarRec.TOUCummulativeDemand[3].Description = VEC_CUM_KVAR + " " + VEC_LAG + " " + VEC_RATE_D;
						}

						if (VarRec.TOUCCummulativeDemand != null)
						{
							VarRec.TOUCCummulativeDemand[0].Description = VEC_CCUM_KVAR + " " + VEC_LAG + " " + VEC_RATE_A;
							VarRec.TOUCCummulativeDemand[1].Description = VEC_CCUM_KVAR + " " + VEC_LAG + " " + VEC_RATE_B;
							VarRec.TOUCCummulativeDemand[2].Description = VEC_CCUM_KVAR + " " + VEC_LAG + " " + VEC_RATE_C;
							VarRec.TOUCCummulativeDemand[3].Description = VEC_CCUM_KVAR + " " + VEC_LAG + " " + VEC_RATE_D;
						}
					}
				}

				return VarRec;
			}
		}

		/// <summary>
		/// Provides access to the Var Delivered Quantity
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  11/27/06 jrf 8.00.00 N/A    Adding support to get Var Delivered
		///  12/08/06 jrf 8.00.00 N/A    Added "Lead" into description
		///  01/05/07 mah 8.00.00 N/A Added support for cum and ccum values
		///
		/// </remarks>
		public Quantity VarDelivered
		{
			get
			{
				Quantity VarDel = null;
				VECRegisters eEnergyRegister = VECRegisters.NO_REG;

				ReadRegisterMappings();

				// This quantity only measures energy values
				// Read Energy Values
				if (VECRegisters.NO_REG != m_eVarhLeadMapping)
				{
					VarDel = new Quantity(VEC_VAR_DELIVERED);

					eEnergyRegister = m_eVarhLeadMapping;

					// If the quantity was mapped to TOU register then it 
					// also is mapped to a cooresponding non-tou register
					// Don't check register 1; it is demand only
					if (VECRegisters.REG_2_TOU == m_eVarhLeadMapping)
					{
						eEnergyRegister = VECRegisters.REG_2_RATE_E;
					}
					ReadRegisterData(eEnergyRegister, ref VarDel, VECRegisterType.ENERGY);
					VarDel.TotalEnergy.Description = VEC_KVARH + " " + VEC_LEAD;
				}

				if (TOUEnabled)
				{
					//Read TOU Energy Values
					if (VECRegisters.REG_2_TOU == m_eVarhLeadMapping)
					{
						ReadRegisterData(m_eVarhLeadMapping, ref VarDel, VECRegisterType.ENERGY);
						VarDel.TOUEnergy[0].Description = VEC_KVARH + " " + VEC_LEAD + " " + VEC_RATE_A;
						VarDel.TOUEnergy[1].Description = VEC_KVARH + " " + VEC_LEAD + " " + VEC_RATE_B;
						VarDel.TOUEnergy[2].Description = VEC_KVARH + " " + VEC_LEAD + " " + VEC_RATE_C;
						VarDel.TOUEnergy[3].Description = VEC_KVARH + " " + VEC_LEAD + " " + VEC_RATE_D;
					}
				}

				return VarDel;
			}
		}

		/// <summary>
		/// Provides access to the VA Received Quantity
		/// </summary>
		/// <remarks >
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  11/27/06 jrf 8.00.00 N/A    Adding support to get VA Received
		///  12/08/06 jrf 8.00.00 N/A    Added "Lag" into the description
		///  01/05/07 mah 8.00.00 N/A Added support for cum and ccum values
		/// </remarks>
		public Quantity VAReceived
		{
			get
			{
				Quantity VARec = null;
				VECRegisters eEnergyRegister = VECRegisters.NO_REG;
				VECRegisters eDemandRegister = VECRegisters.NO_REG;

				ReadRegisterMappings();

				//Read Energy Values
				if (VECRegisters.NO_REG != m_eVAhLagMapping)
				{
					VARec = new Quantity(VEC_VA_RECEIVED);

					eEnergyRegister = m_eVAhLagMapping;

					// If the quantity was mapped to TOU register then it 
					// also is mapped to a cooresponding non-tou register
					// Don't check register 1; it is demand only
					if (VECRegisters.REG_2_TOU == m_eVAhLagMapping)
					{
						eEnergyRegister = VECRegisters.REG_2_RATE_E;
					}
					ReadRegisterData(eEnergyRegister, ref VARec, VECRegisterType.ENERGY);
					VARec.TotalEnergy.Description = VEC_KVAH + " " + VEC_LAG;
				}

				//Read Demand Values
				if (VECRegisters.NO_REG != m_eVALagMapping)
				{
					if (null == VARec)
					{
						VARec = new Quantity(VEC_VA_RECEIVED);
					}

					eDemandRegister = m_eVALagMapping;
					if (VECRegisters.REG_1_TOU == m_eVALagMapping)
					{
						eDemandRegister = VECRegisters.REG_1_RATE_E;
					}
					else if (VECRegisters.REG_2_TOU == m_eVALagMapping)
					{
						eDemandRegister = VECRegisters.REG_2_RATE_E;
					}
					ReadRegisterData(eDemandRegister, ref VARec, VECRegisterType.DEMAND);
					VARec.TotalMaxDemand.Description = VEC_MAX_KVA + " " + VEC_LAG;

					if (VARec.CummulativeDemand != null)
					{
						VARec.CummulativeDemand.Description = VEC_CUM_KVA + " " + VEC_LAG;
					}

					if (VARec.ContinuousCummulativeDemand != null)
					{
						VARec.ContinuousCummulativeDemand.Description = VEC_CCUM_KVA + " " + VEC_LAG;
					}

				}

				if (TOUEnabled)
				{
					//Read TOU Energy Values
					if (VECRegisters.REG_2_TOU == m_eVAhLagMapping)
					{
						ReadRegisterData(m_eVAhLagMapping, ref VARec, VECRegisterType.ENERGY);
						VARec.TOUEnergy[0].Description = VEC_KVAH + " " + VEC_LAG + " " + VEC_RATE_A;
						VARec.TOUEnergy[1].Description = VEC_KVAH + " " + VEC_LAG + " " + VEC_RATE_B;
						VARec.TOUEnergy[2].Description = VEC_KVAH + " " + VEC_LAG + " " + VEC_RATE_C;
						VARec.TOUEnergy[3].Description = VEC_KVAH + " " + VEC_LAG + " " + VEC_RATE_D;
					}

					//Read TOU Demand Values
					if (VECRegisters.REG_1_TOU == m_eVALagMapping ||
						VECRegisters.REG_2_TOU == m_eVALagMapping)
					{
						ReadRegisterData(m_eVALagMapping, ref VARec, VECRegisterType.DEMAND);
						VARec.TOUMaxDemand[0].Description = VEC_MAX_KVA + " " + VEC_LAG + " " + VEC_RATE_A;
						VARec.TOUMaxDemand[1].Description = VEC_MAX_KVA + " " + VEC_LAG + " " + VEC_RATE_B;
						VARec.TOUMaxDemand[2].Description = VEC_MAX_KVA + " " + VEC_LAG + " " + VEC_RATE_C;
						VARec.TOUMaxDemand[3].Description = VEC_MAX_KVA + " " + VEC_LAG + " " + VEC_RATE_D;

						if (VARec.TOUCummulativeDemand != null)
						{
							VARec.TOUCummulativeDemand[0].Description = VEC_CUM_KVA + " " + VEC_LAG + " " + VEC_RATE_A;
							VARec.TOUCummulativeDemand[1].Description = VEC_CUM_KVA + " " + VEC_LAG + " " + VEC_RATE_B;
							VARec.TOUCummulativeDemand[2].Description = VEC_CUM_KVA + " " + VEC_LAG + " " + VEC_RATE_C;
							VARec.TOUCummulativeDemand[3].Description = VEC_CUM_KVA + " " + VEC_LAG + " " + VEC_RATE_D;
						}

						if (VARec.TOUCCummulativeDemand != null)
						{
							VARec.TOUCCummulativeDemand[0].Description = VEC_CCUM_KVA + " " + VEC_LAG + " " + VEC_RATE_A;
							VARec.TOUCCummulativeDemand[1].Description = VEC_CCUM_KVA + " " + VEC_LAG + " " + VEC_RATE_B;
							VARec.TOUCCummulativeDemand[2].Description = VEC_CCUM_KVA + " " + VEC_LAG + " " + VEC_RATE_C;
							VARec.TOUCCummulativeDemand[3].Description = VEC_CCUM_KVA + " " + VEC_LAG + " " + VEC_RATE_D;
						}
					}
				}

				return VARec;
			}
		}

		/// <summary>
		/// Provides access to the VA Total Quantity
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  11/27/06 jrf 8.00.00 N/A    Adding support to get VA Total
		///  01/05/07 mah 8.00.00 N/A Added support for cum and ccum values
		/// </remarks>
		public Quantity VATotal
		{
			get
			{
				Quantity VATot = null;
				VECRegisters eDemandRegister = VECRegisters.NO_REG;

				ReadRegisterMappings();

				// This quantity only measures demand values
				//Read Demand Values
				if (VECRegisters.NO_REG != m_eVATotalMapping)
				{
					VATot = new Quantity(VEC_VA_TOTAL);

					eDemandRegister = m_eVATotalMapping;

					// If the quantity was mapped to TOU register then it 
					// also is mapped to a cooresponding non-tou register
					if (VECRegisters.REG_1_TOU == m_eVATotalMapping)
					{
						eDemandRegister = VECRegisters.REG_1_RATE_E;
					}
					else if (VECRegisters.REG_2_TOU == m_eVATotalMapping)
					{
						eDemandRegister = VECRegisters.REG_2_RATE_E;
					}
					ReadRegisterData(eDemandRegister, ref VATot, VECRegisterType.DEMAND);
					VATot.TotalMaxDemand.Description = VEC_MAX_KVA;

					if (VATot.CummulativeDemand != null)
					{
						VATot.CummulativeDemand.Description = VEC_CUM_KVA;
					}

					if (VATot.ContinuousCummulativeDemand != null)
					{
						VATot.ContinuousCummulativeDemand.Description = VEC_CCUM_KVA;
					}

				}
				if (TOUEnabled)
				{
					//Read TOU Demand Values
					if (VECRegisters.REG_1_TOU == m_eVATotalMapping ||
						VECRegisters.REG_2_TOU == m_eVATotalMapping)
					{
						ReadRegisterData(m_eVATotalMapping, ref VATot, VECRegisterType.DEMAND);
						VATot.TOUMaxDemand[0].Description = VEC_MAX_KVA + " " + VEC_RATE_A;
						VATot.TOUMaxDemand[1].Description = VEC_MAX_KVA + " " + VEC_RATE_B;
						VATot.TOUMaxDemand[2].Description = VEC_MAX_KVA + " " + VEC_RATE_C;
						VATot.TOUMaxDemand[3].Description = VEC_MAX_KVA + " " + VEC_RATE_D;

						if (VATot.TOUCummulativeDemand != null)
						{
							VATot.TOUCummulativeDemand[0].Description = VEC_CUM_KVA + " " + VEC_RATE_A;
							VATot.TOUCummulativeDemand[1].Description = VEC_CUM_KVA + " " + VEC_RATE_B;
							VATot.TOUCummulativeDemand[2].Description = VEC_CUM_KVA + " " + VEC_RATE_C;
							VATot.TOUCummulativeDemand[3].Description = VEC_CUM_KVA + " " + VEC_RATE_D;
						}

						if (VATot.TOUCCummulativeDemand != null)
						{
							VATot.TOUCCummulativeDemand[0].Description = VEC_CCUM_KVA + " " + VEC_RATE_A;
							VATot.TOUCCummulativeDemand[1].Description = VEC_CCUM_KVA + " " + VEC_RATE_B;
							VATot.TOUCCummulativeDemand[2].Description = VEC_CCUM_KVA + " " + VEC_RATE_C;
							VATot.TOUCCummulativeDemand[3].Description = VEC_CCUM_KVA + " " + VEC_RATE_D;
						}
					}
				}

				return VATot;
			}
		}		

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// This method verifies that the derived device type matches 
		/// the SCS device's type 
		/// </summary>
		/// <returns>
		/// a boolean indicating whether or not the device type is correct
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/19/06 jrf 7.30.00  N/A   Created
		/// 
		override protected bool VerifyDeviceType()
		{
			return (DeviceType == ExpectedDeviceType);
		}

		/// <summary>
		/// This gets the errors from the meter and converts them to an 
		/// array of strings.
		/// </summary>
		/// <param name="strErrors">A string array that will be filled 
		/// with the errors</param>
		/// <exception cref="SCSException">
		/// Thrown when the errors cannot be retreived from the meter.
		/// </exception>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/25/06 jrf 7.30.00  N/A   Created
		/// 
		protected override void ReadErrors(out string[] strErrors)
		{
			byte[] byErrors;
			ArrayList ErrorList = new ArrayList();
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading System Errors");
			// Get the system errors
			ProtocolResponse = m_SCSProtocol.Upload(
				(int)VECAddresses.SYSTEM_ERROR,
				VEC_SYSTEM_ERROR_LENGTH,
				out byErrors);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				// Check for each error and add to list as appropriate
				if (0 != (byErrors[0] & (byte)VECErrors.LOW_BATTERY))
				{
					ErrorList.Add(m_rmStrings.GetString("LOW_BATTERY"));
				}
				if (0 != (byErrors[0] & (byte)VECErrors.REGISTER_FULL_SCALE))
				{
					ErrorList.Add(m_rmStrings.GetString("REGISTER_FULL_SCALE"));
				}
				if (0 != (byErrors[0] & (byte)VECErrors.CLOCK_TOU_MM_ERROR))
				{
					ErrorList.Add(m_rmStrings.GetString("CLOCK_TOU_MM_ERROR"));
				}
				if (0 != (byErrors[0] & (byte)VECErrors.REVERSE_POWER_FLOW))
				{
					ErrorList.Add(m_rmStrings.GetString("REVERSE_POWER_FLOW"));
				}
			}
			else
			{
				SCSException objSCSException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.SYSTEM_ERROR,
					m_rmStrings.GetString("SYSTEM_ERRORS"));
				throw objSCSException;
			}

			strErrors = (string[])ErrorList.ToArray(typeof(string));
		} // End ReadErrors()

		/// <summary>
		/// This method stops and starts the metering operation of the VECTRON
		/// </summary>
		/// <param name="disableMeter">The boolean to determine if the meter needs
		/// to be disabled or enabled</param>
		/// <returns>A SCSProtocolResponse</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/07/07 KRC 8.00.10        Add stop metering for Edit registers.
		/// 
		override protected SCSProtocolResponse StopMetering(bool disableMeter)
		{
			SCSProtocolResponse objProtocolResponse = SCSProtocolResponse.NoResponse;
			byte[] abytFlag = new byte[1];

			if (disableMeter)
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Stopping Metering");
				abytFlag[0] = SCS_FLAG_ON;
			}
			else
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Starting Metering");
				abytFlag[0] = SCS_FLAG_OFF;
			}

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set Stop Meter Flag");
			objProtocolResponse = m_SCSProtocol.Download(
									(int)VECAddresses.STOP_METER_FLAG,
									SCS_FLAG_LENGTH,
									ref abytFlag);

			return objProtocolResponse;
		}

		#endregion Protected Methods

		#region Protected Properties

		/// <summary>This property gets the expected device type "VEC".</summary>
		/// <returns>
		/// A string representing the expected device type.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
		protected override string ExpectedDeviceType
		{
			get
			{
				return m_rmStrings.GetString("VEC_DEVICE_TYPE");
			}
		}

		/// <summary>This property gets the demand reset flag address.</summary>
		/// <returns>
		/// An int representing the demand reset flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/15/06 mah 7.35.00  N/A   Created
		///	
		protected override int DemandResetFlagAddress
		{
			get
			{
				return (int)VECAddresses.DEMAND_RESET_FLAG;
			}
		}

		/// <summary>This property gets the clear billing data flag address.</summary>
		/// <returns>
		/// An int representing the clear billing data flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/15/06 mah 7.35.00  N/A   Created
		///	
		protected override int ClearBillingDataFlagAddress
		{
			get
			{
				return (int)VECAddresses.CLEAR_BILLING_FLAG;
			}
		}

		/// <summary>This property gets the address of the test mode flag address.</summary>
		/// <returns>
		/// An int representing the test mode flag address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 12/21/06 mah 8.00.00  N/A   Created
		///	</remarks>
		protected override int RemoteTestModeFlagAddress
		{
			get
			{
				return (int)VECAddresses.TEST_MODE_FLAG;
			}
		}


		/// <summary>This property gets the hang up flag address.</summary>
		/// <returns>
		/// An int representing the hang up flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
		protected override int CommunicationsHangUpFlagAddress
		{
			get
			{
				return (int)VECAddresses.HANGUP_FLAG;
			}
		}

		/// <summary>This property gets the stop clock flag address.</summary>
		/// <returns>
		/// An int representing the stop clock flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
		protected override int StopClockFlagAddress
		{
			get
			{
				return (int)VECAddresses.CLOCK_RUN_FLAG;
			}
		}

		/// <summary>This property gets the TOU run flag address.</summary>
		/// <returns>
		/// An int representing the TOU run flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
		protected override int TOURunFlagAddress
		{
			get
			{
				return (int)VECAddresses.TOU_RUN_FLAG;
			}
		}

		/// <summary>This property gets the clock reconfigure flag address.
		/// </summary>
		/// <returns>
		/// An int representing the clock reconfigure flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
		protected override int ClockReconfigureFlagAddress
		{
			get
			{
				return (int)VECAddresses.CLOCK_RECONFIGURE_FLAG;
			}
		}

		/// <summary>This property gets the TOU reconfigure flag address.
		/// </summary>
		/// <returns>
		/// An int representing the TOU reconfigure flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
		protected override int TOUReconfigureFlagAddress
		{
			get
			{
				return (int)VECAddresses.TOU_RECONFIGURE_FLAG;
			}
		}

		/// <summary>This property gets the real time clock address.</summary>
		/// <returns>
		/// An int representing the real time clock address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
		protected override int RealTimeClockAddress
		{
			get
			{
				return (int)VECAddresses.REAL_TIME;
			}
		}

		/// <summary>This property gets the model type address.</summary>
		/// <returns>
		/// An int representing the model type address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
		protected override int ModelTypeAddress
		{
			get
			{
				return (int)VECAddresses.MODEL_TYPE;
			}
		}

		/// <summary>This property gets the last reset date address.</summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/21/06 mah 7.35.00  N/A   Created
		///
		protected override int LastResetDateAddress
		{
			get
			{
				return (int)VECAddresses.LAST_RESET_DATE;
			}
		}

		/// <summary>This property gets the demand reset count address.</summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/21/06 mah 7.35.00  N/A   Created
		///
		protected override int NumResetsAddress
		{
			get
			{
				return (int)VECAddresses.RESET_COUNT;
			}
		}

		/// <summary>This property gets the outage count address.</summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/21/06 mah 7.35.00  N/A   Created
		///
		protected override int NumOutagesAddress
		{
			get
			{
				return (int)VECAddresses.OUTAGE_COUNT;
			}
		}

		/// <summary>This property gets the last programmed date address.</summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/21/06 mah 7.35.00  N/A   Created
		///
		protected override int LastProgrammedDateAddress
		{
			get
			{
				return (int)VECAddresses.LAST_PROGRAMMED_DATE;
			}
		}


		/// <summary>This property gets the cold load pickup time address.</summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/17/06 mah 8.00.00  N/A   Created
		/// </remarks>
		protected override int ColdLoadPickupTimeAddress
		{
			get
			{
				return (int)VECAddresses.COLD_LOAD_PICKUP_TIME;
			}
		}

		/// <summary>This property gets the address of the program count.</summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/21/06 mah 7.35.00  N/A   Created
		///
		protected override int NumTimesProgrammedAddress
		{
			get
			{
				return (int)VECAddresses.PROGRAM_COUNT;
			}
		}

		/// <summary>This property returns the address of the number of minutes the 
		/// device was on battery power.
		/// </summary>
		/// <returns>
		/// An int representing the basepage address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------  
		/// 09/08/06 mah 7.35.00  N/A   Created
		/// </remarks>
		protected override int NumOfMinutesOnBatteryAddress
		{
			get
			{
				return (int)VECAddresses.MINUTES_ON_BATTERY;
			}
		}

		/// <summary>
		/// This property gets the address of the first byte of demand 
		/// configuration data
		/// </summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/17/06 mah 8.00.00  N/A   Created
		/// </remarks>
		protected override int DemandConfigurationAddress
		{
			get
			{
				return (int)VECAddresses.DEMAND_CONFIGURATION;
			}
		}


		/// <summary>This property gets the operating setup address.</summary>
		/// <returns>
		/// An int representing the operating setup address.
		/// </returns>
		/// <remarks >
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		/// </remarks>
		protected override int OperatingSetupAddress
		{
			get
			{
				return (int)VECAddresses.OPERATING_SETUP;
			}
		}

		/// <summary>This property gets the firmware version address.</summary>
		/// <returns>
		/// An int representing the firmware version address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		/// </remarks>
		protected override int FWVersionAddress
		{
			get
			{
				return (int)VECAddresses.FIRMWARE_REVISION;
			}
		}

		/// <summary>This property gets the software version address.</summary>
		/// <returns>
		/// An int representing the software version address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		/// </remarks>
		protected override int SWVersionAddress
		{
			get
			{
				return (int)VECAddresses.SOFTWARE_REVISION;
			}
		}

		/// <summary>This property gets the program ID address.</summary>
		/// <returns>
		/// An int representing the program ID address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		/// </remarks>
		protected override int ProgramIDAddress
		{
			get
			{
				return (int)VECAddresses.PROGRAM_ID;
			}
		}

		/// <summary>This property gets the address of the TOU Schedule ID.</summary>
		/// <returns>
		/// An int representing the TOU Schedule ID address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 12/14/06 mah 8.00.00  N/A   Created
		///	</remarks>
		///	
		protected override int TOUScheduleIDAddress
		{
			get
			{
				return (int)VECAddresses.TOU_SCHEDULE_ID;
			}
		}

		/// <summary>This property gets the TOU calendar address.</summary>
		/// <returns>
		/// An int representing the TOU calendar address.
		/// </returns>
		/// <remarks>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		/// </remarks>
		protected override int TOUCalendarAddress
		{
			get
			{
				if (!m_touCalendarStartAddress.Cached)
				{
					SCSProtocolResponse ProtocolResponse;
					byte[] Data;

					ProtocolResponse = m_SCSProtocol.Upload(
						(int)VECAddresses.TOU_BASE,
						SCS_TOU_CALENDAR_LENGTH, out Data);

					if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
					{
						m_touCalendarStartAddress.Value = Data[0] * 0x100 + Data[1];
					}
					else
					{
						throw (new SCSException(
						SCSCommands.SCS_S,
						ProtocolResponse,
						0,
						m_rmStrings.GetString("TOU_BASE_ADDRESS")));
					}

				}

				return m_touCalendarStartAddress.Value;
			}
		}

		/// <summary>This property gets the TOU Yearly Schedule address</summary>
		/// <returns>
		/// An int representing the TOU address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///		
		protected override int YearlyScheduleAddress
		{
			get
			{
				return (int)VECAddresses.YEARLY_SCHEDULE;
			}
		}

		/// <summary>This property gets the Last Season data address.
		/// Not all have this item in their base page.</summary>
		/// <returns>
		/// An int representing the TOU address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/10/06 mcm 7.30.00  N/A   Created
		///		
		protected override int LastSeasonDataAddress
		{
			get
			{
				return (int)VECAddresses.LAST_SEASON_REGISTERS;
			}
		}

		/// <summary>This property gets the communication timeout address.
		/// </summary>
		/// <returns>
		/// An int representing the communication timeout address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/25/06 jrf 7.30.00  N/A	Created
		///
		protected override int CommunicationTimeoutAddress
		{
			get
			{
				return (int)VECAddresses.COMMUNICATIONS_TIMEOUT;
			}
		}

		/// <summary>This property gets the interval length address.</summary>
		/// <returns>
		/// An int representing the interval length address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/25/06 jrf 7.30.00  N/A	Created
		///
		protected override int IntervalLengthAddress
		{
			get
			{
				return (int)VECAddresses.MM_INTERVAL_LENGTH;
			}
		}

		/// <summary>This property gets the serial number address.</summary>
		/// <returns>
		/// An int representing the serial number address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/25/06 jrf 7.30.00  N/A	Created
		///
		protected override int SerialNumberAddress
		{
			get
			{
				return (int)VECAddresses.SERIAL_NUMBER;
			}
		}

		/// <summary>
		/// This property gets the address of the display format flags.</summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 12/07/06 mah 8.00.00  N/A   Created
		/// </remarks>
		protected override int EnergyFormatAddress
		{
			get
			{
				return (int)VECAddresses.ENERGYFORMAT_ADDRESS;
			}
		}

		/// <summary>
		/// This property gets the address of the meter's display options.
		/// </summary>
		/// <returns>
		/// An int representing a basepage address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 12/07/06 mah 8.00.00  N/A   Created
		/// </remarks>
		protected override int DisplayOptionsAddress
		{
			get
			{
				return (int)VECAddresses.DISPLAY_OPTIONS;
			}
		}


		/// <summary>This property gets the serial number length.</summary>
		/// <returns>
		/// An int representing the serial number length.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/25/06 jrf 7.30.00  N/A	Created
		///
		protected override int SerialNumberLength
		{
			get
			{
				return VEC_SERIAL_NUMBER_LENGTH;
			}
		}

		/// <summary>This property gets the Firmware Options address.</summary>
		/// <returns>
		/// An int representing the Firmware Options address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/04/06 mcm 7.30.00  N/A   Created
		///				
		protected override int FirmwareOptionsAddress
		{
			get
			{
				return (int)VECAddresses.FIRMWARE_OPTIONS;
			}
		}

		/// <summary>This property gets the unit ID address.</summary>
		/// <returns>
		/// An int representing the unit ID address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A	Created
		/// 05/25/06 jrf 7.30.00  N/A	Created
		///
		protected override int UnitIDAddress
		{
			get
			{
				return (int)VECAddresses.UNIT_ID;
			}
		}


		/// <summary>This property gets the address of the first user defined data field.</summary>
		/// <returns>
		/// An int representing the unit ID address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/16/06 mah 8.00.00  N/A	Created
		/// </remarks>
		protected override int UserDataBlockAddress
		{
			get
			{
				return (int)VECAddresses.USERDEFINED_FIELD1;
			}
		}

		/// <summary>This property gets the address of the start of the display table.</summary>
		/// <returns>
		/// An int representing the first display item address.
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 12/05/06 mah 8.00.00  N/A   Created
		///	</remarks>
		///	
		protected override int DisplayTableAddress
		{
			get
			{
				return (int)VECAddresses.DISPLAY_TABLE;
			}
		}

		/// <summary>
		/// This property returns the address of the number of load profile channels
		/// </summary>
		/// <returns>
		/// An int representing a basepage address 
		/// </returns>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 12/05/06 mah 8.00.00  N/A   Created
		///	</remarks>
		///	
		protected override int ChannelCountAddress
		{
			get
			{
				return (int)VECAddresses.NUMBER_OF_CHANNELS;
			}
		}

        /// <summary>
        /// Property used to get the meter type (string).  Use
        /// this property for meter determination and comparison.  
        /// This property should not be confused with MeterName which
        /// is used to obtain a human readable name of the meter.
        /// </summary>
		/// <returns>
		/// A string representing the meter type.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/25/06 jrf 7.30.00  N/A   Created
        /// 03/16/07 jrf 8.00.18        Changed from resource string to constant
		///	
		public override string MeterType
		{
			get
			{
				return VECTRON_TYPE;
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
        //
        public override string MeterName
        {
            get
            {
                return VECTRON_NAME;
            }
        }

		/// <summary>
		/// This property gets the load profile run flag address.
		/// </summary>
		/// <returns>
		/// An int representing the load profile run flag.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/31/06 jrf 7.30.00  N/A   Created
		///		
		protected override int LoadProfileFlagAddress
		{
			get
			{
				return (int)VECAddresses.MM_RUN_FLAG;
			}
		}

		/// <summary>
		/// This property gets the load profile adjust time flag address.
		/// </summary>
		/// <returns>
		/// An int representing the load profile run flag.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/31/06 jrf 7.30.00  N/A   Created
		///		
		protected override int LoadProfileAdjustTimeFlagAddress
		{
			get
			{
				return (int)VECAddresses.MM_AJUST_TIME_FLAG;
			}
		}

		/// <summary>
		/// This property gets the tou expiration address.
		/// </summary>
		/// <returns>
		/// An int representing the tou expiration address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/12/06 jrf 7.30.00  N/A   Created
		///		
		protected override int TOUExpirationAddress
		{
			get
			{
				return (int)VECAddresses.TOU_EXPIRATION_DATE;
			}
		}

		/// <summary>This property gets the clock run flag address.</summary>
		/// <returns>
		/// An int representing the clock run flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/13/06 jrf 7.30.00  N/A   Created
		///
		protected override int ClockRunFlagAddress
		{
			get
			{
				return (int)VECAddresses.CLOCK_RUN_FLAG;
			}
		}

		/// <summary>Returns the address of the primary security code.</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/16/06 mcm 7.35.00  N/A   Created
		///				
		protected override int PrimaryPasswordAddress
		{
			get
			{
				return (int)VECAddresses.PRIMARY_PASSWORD;
			}
		}

		/// <summary>Returns the address of the Secondary security code.</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/17/06 mcm 7.35.00  N/A   Created
		///				
		protected override int SecondaryPasswordAddress
		{
			get
			{
				return (int)VECAddresses.SECONDARY_PASSWORD;
			}
		}

		/// <summary>Returns the address of the Tertiary security code.</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/17/06 mcm 7.35.00  N/A   Created
		///				
		protected override int TertiaryPasswordAddress
		{
			get
			{
				return (int)VECAddresses.TERTIARY_PASSWORD;
			}
		}

		/// <summary>Returns the address of the Modem security code. A copy of 
		/// the tertiary password should be written here.
		/// </summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/17/06 mcm 7.35.00  N/A   Created
		///				
		protected override int ModemPasswordAddress
		{
			get
			{
				return (int)VECAddresses.MODEM_PASSWORD;
			}
		}

		/// <summary>This meter has a separate modem password?  A copy of the 
		/// tertiary password should be written to the modem password field.
		/// </summary>
		///<remarks>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/17/06 mcm 7.30.00  N/A   Created
		/// </remarks> 
		protected override bool HasModemPassword
		{
			get
			{
				return true;
			}
		}

		/// <summary>This property gets the transformer ratio address.</summary>
		/// <returns>
		/// An int representing the transformer ratio address.
		/// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/07 mrj 8.00.11		Created
		//  
		protected override int TransformerRatioAddress
		{
			get
			{
				return (int)VECAddresses.TRANSFORMER_RATIO;
			}
		}

        /// <summary>This property gets the Load Research ID address.</summary>
        /// <returns>
        /// An int representing the Load Research ID address.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/13/07 mcm 8.00.18		Created
        //  
        protected override int LoadResearchIDAddress
        {
            get
            {
                return (int)VECAddresses.LOAD_RESEARCH_ID;
            }
        }

		#endregion Protected Properties

		#region Private Methods

		/// <summary>
		/// This method reads the toolbox data from the SCS Device.
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the toolbox data cannot be retreived from the meter.
		/// </exception>
		//  Revision History  
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/25/06 jrf 7.30.00  N/A   Created
		//  06/20/06 jrf 7.30.00  N/A   Changed to use BinaryReader to process
		//								floats.
		//  02/09/07 mrj 8.00.11		Added support for calculating the 
		//								instantaneous quantities.
		//  
		private void ReadToolboxData()
		{
			byte[] byToolboxData;
			byte[] byFloat = new byte[4];
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading ToolBox Data");
			// Get the toolbox data
			ProtocolResponse = m_SCSProtocol.Upload(
				(int)VECAddresses.TOOLBOX_DATA,
				VEC_TOOLBOX_DATA_LENGTH,
				out byToolboxData);


			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				ReorderFloats(ref byToolboxData);
				MemoryStream TempStream = new MemoryStream(byToolboxData);
				BinaryReader TempBReader = new BinaryReader(TempStream);

				// Interpret the toolbox data
				m_Toolbox.m_fVoltsA = (double)TempBReader.ReadSingle();
				m_Toolbox.m_fVoltsB = (double)TempBReader.ReadSingle();
				m_Toolbox.m_fVoltsC = (double)TempBReader.ReadSingle();

				m_Toolbox.m_fCurrentA = (double)TempBReader.ReadSingle();
				m_Toolbox.m_fCurrentB = (double)TempBReader.ReadSingle();
				m_Toolbox.m_fCurrentC = (double)TempBReader.ReadSingle();

				m_Toolbox.m_fIAngleA = (double)TempBReader.ReadSingle();
				m_Toolbox.m_fIAngleB = (double)TempBReader.ReadSingle();
				m_Toolbox.m_fIAngleC = (double)TempBReader.ReadSingle();

				m_Toolbox.m_fVAngleA = (double)VEC_V_ANGLE_A;
				m_Toolbox.m_fVAngleB = (double)TempBReader.ReadSingle();
				m_Toolbox.m_fVAngleC = (double)TempBReader.ReadSingle();

				//Calculate the instantaneous registers
				CalculateInstRegisters();
			}
			else
			{
				SCSException objSCSException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.TOOLBOX_DATA,
					m_rmStrings.GetString("TOOLBOX_DATA"));
				throw objSCSException;
			}
		} // End ReadToolboxData()

		/// <summary>
		/// This method calculates the instantaneous registers needed for toolbox.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/07 mrj 8.00.11		Created
		//  
		private void CalculateInstRegisters()
		{
			//Need to get the meter form
			if (!m_MeterForm.Cached)
			{
				ReadMeterForm();
			}

			CalculateInsKW();
			CalculateInsKVar();
			CalculateInsKVA();
			CalculateInsPF();
		}
				
		/// <summary>
		/// Calculates instantaneous KW, this code was copied from the Vectron
		/// device server.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/09/07 mrj 8.00.11		Created
		//  
		private void CalculateInsKW()
		{
			if (m_MeterForm.Value == 0) 		// 9s/8s or 16S/15S ( 3 elements )
			{
				m_Toolbox.m_dInsKW = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA * Math.Cos(Radians(m_Toolbox.m_fIAngleA))) +
									 (m_Toolbox.m_fVoltsB * m_Toolbox.m_fCurrentB * Math.Cos(Radians(m_Toolbox.m_fIAngleB) - Radians(m_Toolbox.m_fVAngleB))) +
									 (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC * Math.Cos(Radians(m_Toolbox.m_fIAngleC) - Radians(m_Toolbox.m_fVAngleC)));
			}
			else if (m_MeterForm.Value == 1)  // 6S (2-1/2 elements )
			{
				m_Toolbox.m_dInsKW = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA * Math.Cos(Radians(m_Toolbox.m_fIAngleA))) +
									 (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC * Math.Cos(Radians(m_Toolbox.m_fIAngleC) - Radians(m_Toolbox.m_fVAngleC))) +
									 (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentB * Math.Cos(Radians(m_Toolbox.m_fIAngleB) - Radians(180.0))) +
									 (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentB * Math.Cos(Radians(m_Toolbox.m_fIAngleB) - Radians(m_Toolbox.m_fVAngleC) + Radians(180.0)));
			}
			else if (m_MeterForm.Value == 2)  // 5S or 12S (2 elements )
			{
				m_Toolbox.m_dInsKW = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA * Math.Cos(Radians(m_Toolbox.m_fIAngleA))) +
									 (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC * Math.Cos(Radians(m_Toolbox.m_fIAngleC) - Radians(m_Toolbox.m_fVAngleC)));
			} 

			//Convert to Kilo
			m_Toolbox.m_dInsKW = m_Toolbox.m_dInsKW / 1000.0;
		}

		/// <summary>
		/// Calculates instantaneous KVar, this code was copied from the Vectron
		/// device server.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/09/07 mrj 8.00.11		Created
		// 
		private void CalculateInsKVar()
		{

			if (m_MeterForm.Value == 0) 		// 9s/8s or 16S/15S ( 3 elements )
			{
				m_Toolbox.m_dInsKVar = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA * Math.Sin(Radians(m_Toolbox.m_fIAngleA))) +
									   (m_Toolbox.m_fVoltsB * m_Toolbox.m_fCurrentB * Math.Sin(Radians(m_Toolbox.m_fIAngleB) - Radians(m_Toolbox.m_fVAngleB))) +
									   (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC * Math.Sin(Radians(m_Toolbox.m_fIAngleC) - Radians(m_Toolbox.m_fVAngleC)));
			}
			else if (m_MeterForm.Value == 1)  // 6S (2-1/2 elements )
			{
				m_Toolbox.m_dInsKVar = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA * Math.Sin(Radians(m_Toolbox.m_fIAngleA))) +
									   (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC * Math.Sin(Radians(m_Toolbox.m_fIAngleC) - Radians(m_Toolbox.m_fVAngleC))) +
									   (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentB * Math.Sin(Radians(m_Toolbox.m_fIAngleB) - Radians(180.0))) +
									   (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentB * Math.Sin(Radians(m_Toolbox.m_fIAngleB) - Radians(m_Toolbox.m_fVAngleC) + Radians(180.0)));
			}
			else if (m_MeterForm.Value == 2)  // 5S or 12S (2 elements )
			{
				m_Toolbox.m_dInsKVar = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA * Math.Sin(Radians(m_Toolbox.m_fIAngleA))) +
									   (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC * Math.Sin(Radians(m_Toolbox.m_fIAngleC) - Radians(m_Toolbox.m_fVAngleC)));
			}        

			//Convert to Kilo
			m_Toolbox.m_dInsKVar = m_Toolbox.m_dInsKVar / 1000.0;
		}

		/// <summary>
		/// Calculates instantaneous KVA, this code was copied from the Vectron
		/// device server.
		/// </summary>
		/// <remarks>
		/// This method must be called after CalculateInsKW and CalculateInsKVar.
		/// </remarks>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/09/07 mrj 8.00.11		Created
		// 
		private void CalculateInsKVA()
		{
			if (!m_VACalculation.Cached)
			{
				ReadMeterConfiguration();
			}
			if (!m_serviceType.Cached)
			{
				ReadServiceType();
			}

            // Calculate the Vecotrial value
            //Convert watts and vars back to units (from kilo)
            double dWatts = m_Toolbox.m_dInsKW * 1000.0;
            double dVar = m_Toolbox.m_dInsKVar * 1000.0;

            m_Toolbox.m_dInsKVAVect = Math.Sqrt(((dWatts * dWatts) + (dVar * dVar)));

			if (m_MeterForm.Value == 0 ) 		// 9s/8s or 16S/15S ( 3 elements )
			{
                // Calculate the Arithmatic value
                m_Toolbox.m_dInsKVAArith = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA) +
										   (m_Toolbox.m_fVoltsB * m_Toolbox.m_fCurrentB) +
										   (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC);

				if ((m_serviceType.Value != m_rmStrings.GetString("SERVICE_TYPE_4")) ||      //3 element 4 wire delta
					!m_VACalculation.Value)
				{
                    m_Toolbox.m_dInsKVA = m_Toolbox.m_dInsKVAArith;
				}
				else
				{
                    m_Toolbox.m_dInsKVA = m_Toolbox.m_dInsKVAVect;
                }			  
			}
			else if (m_MeterForm.Value == 1)  // 6S (2-1/2 elements )
			{
                m_Toolbox.m_dInsKVAArith = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA) +
									  (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC) +
		   							  (Math.Sqrt((m_Toolbox.m_fVoltsA * m_Toolbox.m_fVoltsA) + 
		   										 (m_Toolbox.m_fVoltsC * m_Toolbox.m_fVoltsC) +
		    									 (2.0 * m_Toolbox.m_fVoltsA * m_Toolbox.m_fVoltsC * Math.Cos(m_Toolbox.m_fVAngleC))) * 
												  m_Toolbox.m_fCurrentB); //Arithmetic calculation only

                m_Toolbox.m_dInsKVA = m_Toolbox.m_dInsKVAArith;
			}
			else if (m_MeterForm.Value == 2)  // 5S or 12S (2 elements )
			{
                if (m_serviceType.Value == m_rmStrings.GetString("SERVICE_TYPE_1"))   //2 element 3 wire delta   
                {
                    m_Toolbox.m_dInsKVAArith = Math.Cos(Radians(30)) *
                                          ((m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA) +
                                          (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC));
                }
                else if (m_serviceType.Value == m_rmStrings.GetString("SERVICE_TYPE_7"))  //2 element 4 wire delta
                {
                    m_Toolbox.m_dInsKVAArith = Math.Cos(Radians(30)) *
                                          (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA) +
                                          (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC);
                }
                else
                {
                    m_Toolbox.m_dInsKVAArith = (m_Toolbox.m_fVoltsA * m_Toolbox.m_fCurrentA) +
                                          (m_Toolbox.m_fVoltsC * m_Toolbox.m_fCurrentC); //Arithmetic calculation
                }

				if ((m_serviceType.Value != m_rmStrings.GetString("SERVICE_TYPE_1")) ||   //2 element 3 wire delta   
					!!m_VACalculation.Value)
				{
                    m_Toolbox.m_dInsKVA = m_Toolbox.m_dInsKVAArith;
				}
				else            
				{
                    m_Toolbox.m_dInsKVA = m_Toolbox.m_dInsKVAVect;
				}			  
			}

			//Convert to Kilo
            m_Toolbox.m_dInsKVAArith = m_Toolbox.m_dInsKVAArith / 1000.0;
            m_Toolbox.m_dInsKVAVect = m_Toolbox.m_dInsKVAVect / 1000.0;
			m_Toolbox.m_dInsKVA = m_Toolbox.m_dInsKVA / 1000.0;
		}

		/// <summary>
		/// Calculates PF, this code was copied from the Vectron device server.
		/// </summary>
		/// <remarks>
		/// This method must be called after CalculateInsKW and CalculateInsKVA.
		/// </remarks>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/09/07 mrj 8.00.11		Created
		// 
		private void CalculateInsPF()
		{
			double dWatts = m_Toolbox.m_dInsKW * 1000.0;
			double dVA = m_Toolbox.m_dInsKVA * 1000.0;

			//Comment from device server, "Based on the DGS for EnergyTrac Rev. 1.6"
			if (dVA != 0)
			{
				m_Toolbox.m_dInsPF = Math.Abs(dWatts / dVA);
				if (m_Toolbox.m_dInsPF > 1.0)
				{
					m_Toolbox.m_dInsPF = 1.0;
				}
			}
			else
			{
				if (dWatts == 0)
				{
					m_Toolbox.m_dInsPF = 0.0;
				}
				else
				{
					m_Toolbox.m_dInsPF = 1.0;
				}
			}	
		}

		/// <summary>
		/// Converts degrees to radians
		/// </summary>
		/// <param name="dDegrees">degrees to be converted</param>
		/// <returns>radians (double)</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/09/07 mrj 8.00.11		Created
		//  
		private double Radians(double dDegrees)
		{
			return ((dDegrees * Math.PI) / 180.0);
		}

		/// <summary>
		/// This method reads the diagnostics from the SCS Device.
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the diagnostics cannot be retreived from the meter.
		/// </exception>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/25/06 jrf 7.30.00  N/A   Created
        /// 03/29/07 mcm 8.00.22 2703   Active status doesn't get updated
		/// 
		private void ReadDiagnostics()
		{
			byte[] byDiagnostics;
			int iExceptionAddress = (int)VECAddresses.DIAGNOSTICS_STATUS;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Dianostic Status");
			// Get the Diagnostic Status
			ProtocolResponse = m_SCSProtocol.Upload(
				(int)VECAddresses.DIAGNOSTICS_STATUS,
				VEC_DIAGNOSTICS_STATUS_LENGTH,
				out byDiagnostics);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				// Check the status byte for each diagnostic and set active as appropriate
				if (0 != (byDiagnostics[0] & (byte)VECDiagnostics.PHASOR_CHECK))
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_1].Active = true;
				}
                else
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_1].Active = false;
				}
				if (0 != (byDiagnostics[0] & (byte)VECDiagnostics.VOLTAGE_CHECK))
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_2].Active = true;
				}
                else
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_2].Active = false;
				}
				if (0 != (byDiagnostics[0] & (byte)VECDiagnostics.CURRENT_CHECK))
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_3].Active = true;
				}
                else
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_3].Active = false;
				}
				if (0 != (byDiagnostics[0] & (byte)VECDiagnostics.POWER_FACTOR_CHECK))
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_4].Active = true;
				}
                else
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_4].Active = false;
				}
				if (0 != (byDiagnostics[0] & (byte)VECDiagnostics.DC_DETECT))
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5A].Active = true;
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5B].Active = true;
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5C].Active = true;
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5T].Active = true;
				}
                else
				{
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5A].Active = false;
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5B].Active = false;
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5C].Active = false;
					m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5T].Active = false;
				}

				if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
				{
					m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Diagnostic Counts");
					// Get the Diagnostic Counts
					ProtocolResponse = m_SCSProtocol.Upload(
						(int)VECAddresses.DIAGNOSTICS_COUNTS,
						VEC_DIAGNOSTICS_COUNTS_LENGTH,
						out byDiagnostics);
					iExceptionAddress = (int)VECAddresses.DIAGNOSTICS_COUNTS;
				}

				//Assign all diagnostic counts
				m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_1].Count =
					byDiagnostics[(int)VECDiagIndex.DIAG_1];

				m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_2].Count =
					byDiagnostics[(int)VECDiagIndex.DIAG_2];

				m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_3].Count =
					byDiagnostics[(int)VECDiagIndex.DIAG_3];

				m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_4].Count =
					byDiagnostics[(int)VECDiagIndex.DIAG_4];

				m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5A].Count =
					byDiagnostics[(int)VECDiagIndex.DIAG_5A];

				m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5B].Count =
					byDiagnostics[(int)VECDiagIndex.DIAG_5B];

				m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5C].Count =
					byDiagnostics[(int)VECDiagIndex.DIAG_5C];

				m_Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5T].Count =
					byDiagnostics[(int)VECDiagIndex.DIAG_5T];
			}
			else
			{
				SCSException objSCSException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					iExceptionAddress,
					m_rmStrings.GetString("DIAGNOSTICS"));
				throw objSCSException;
			}
		} // END ReadDiagnostics()

		/// <summary>
		/// This method reads the service type from the SCS Device.
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the service type cannot be retreived from the meter.
		/// </exception>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00  N/A   Created
		/// 06/20/06 jrf 7.30.00  N/A   Modified to set a cached value for 
		///								service type.
		/// 
		private void ReadServiceType()
		{
			byte[] byServiceType;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Service Type");
			// Get the service type byte
			ProtocolResponse = m_SCSProtocol.Upload(
				(int)VECAddresses.SERVICE_TYPE,
				VEC_SERVICE_TYPE_LENGTH,
				out byServiceType);

			// Assign the appropriate service type string
			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				switch (byServiceType[0])
				{
					case (byte)VECServiceType.ID_1:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_1");
					break;
					case (byte)VECServiceType.ID_2:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_2");
					break;
					case (byte)VECServiceType.ID_3:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_3");
					break;
					case (byte)VECServiceType.ID_4:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_4");
					break;
					case (byte)VECServiceType.ID_5:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_5");
					break;
					case (byte)VECServiceType.ID_6:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_6");
					break;
					case (byte)VECServiceType.ID_7:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_7");
					break;
					case (byte)VECServiceType.ID_8:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_8");
					break;
					default:
					m_serviceType.Value =
						m_rmStrings.GetString("SERVICE_TYPE_UNKNOWN");
					break;
				}
			}
			else
			{
				SCSException objSCSException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.SERVICE_TYPE,
					m_rmStrings.GetString("SERVICE_TYPE"));
				throw objSCSException;
			}
		} // End ReadServiceType()

		/// <summary>
		/// This method reads the register mapping values from the VECTRON.
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the register mappings cannot be retreived from the meter.
		/// </exception>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/21/06 jrf 7.30.00  N/A   Created
		/// 
		private void ReadRegisterMappings()
		{
			byte[] abytRegisterMappings;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			if (!m_blnRetreivedRegMapping)
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
												"Read Register Mappings");

				ProtocolResponse = m_SCSProtocol.Upload(
								(int)VECAddresses.REGISTER_MAPPINGS,
								VEC_REG_MAPPINGS_LENGTH, out abytRegisterMappings);

				if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
				{
					VECRegisters eRegMapping = VECRegisters.REG_1_RATE_E;

					for (int iIndex = 0; iIndex < VEC_REG_MAPPINGS_LENGTH; iIndex++)
					{
						switch (abytRegisterMappings[iIndex])
						{
							case 1: // Wh
							{
								m_eWhMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.WATT_HOUR;
								break;
							}
							case 2: // Varh lag
							{
								m_eVarhLagMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.VAR_HOUR_LAG;
								break;
							}
							case 3: // Varh lead
							{
								m_eVarhLeadMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.VAR_HOUR_LEAD;
								break;
							}
							case 4: // VAh lag
							{
								m_eVAhLagMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.VA_HOUR_LAG;
								break;
							}
							case 5: // W
							{
								m_eWMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.WATT;
								break;
							}
							case 6: // WTOU
							{
								m_eWTOUMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.WATT_TOU;
								break;
							}
							case 7: // Var lag
							{
								m_eVarLagMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.VAR_LAG;
								break;
							}
							case 8: // VA lag
							{
								m_eVALagMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.VA_LAG;
								break;
							}
							case 9: // VA total
							{
								m_eVATotalMapping = eRegMapping;
								m_eRegQuantities[iIndex] = VECRegQuantities.VA_TOTAL;
								break;
							}
							default:
							{
								break;
							}
						}

						eRegMapping++;
					}

					m_blnRetreivedRegMapping = true;
				}
				else
				{
					SCSException scsException = new SCSException(
						SCSCommands.SCS_U,
						ProtocolResponse,
						(int)VECAddresses.REGISTER_MAPPINGS,
						m_rmStrings.GetString("REGISTER_MAPPINGS"));
					throw scsException;
				}
			}

		}

		/// <summary>
		/// This method selects the appropriate register data to read from the meter.
		/// </summary>
		/// <param name="eRegister">The register from which to read data</param>
		/// <param name="QuantityToRead">The quantity that will store the data that is read</param>
		/// <param name="eRegisterType">The type of register, energy or demand, that is being read</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/22/06 jrf 8.00.00  N/A   Created
		/// 
		private void ReadRegisterData(VECRegisters eRegister, ref Quantity QuantityToRead, VECRegisterType eRegisterType)
		{
			switch (eRegister)
			{
				case VECRegisters.REG_1_RATE_E:
				{
					ReadReg1Demand(ref QuantityToRead);
					break;
				}
				case VECRegisters.REG_1_TOU:
				{
					ReadReg1TOUDemand(ref QuantityToRead);
					break;
				}
				case VECRegisters.REG_2_RATE_E:
				{
					ReadReg2DemandAndEnergy(ref QuantityToRead, eRegisterType);
					break;
				}
				case VECRegisters.REG_2_TOU:
				{
					ReadReg2TOUDemandAndEnergy(ref QuantityToRead, eRegisterType);
					break;
				}
				case VECRegisters.REG_3_RATE_E:
				{
					ReadReg3DemandAndEnergy(ref QuantityToRead, eRegisterType);
					break;
				}
				case VECRegisters.REG_4_RATE_E:
				{
					ReadReg4DemandAndEnergy(ref QuantityToRead, eRegisterType);
					break;
				}
				default:
				{
					break;
				}
			}
		}

		/// <summary>
		/// This method reads register 1 demand data from the meter.
		/// </summary>
		/// <param name="QuantityToRead">The quantity that will store the data that is read</param>
		/// <exception cref="SCSException">
		/// Thrown when Read Register 1 Demand Data cannot be retreived from the meter.
		/// </exception>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/22/06 jrf 8.00.00  N/A   Created
		/// 01/04/07 MAH 8.00.00 Added support for retrieving cum demand value
		/// </remarks>
		private void ReadReg1Demand(ref Quantity QuantityToRead)
		{
			byte[] abytRegisterData;
			byte[] abytTempRegisterData = new byte[VEC_DEM_READING_LENGTH];
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
											"Read Register 1 Demand Data");

			ProtocolResponse = m_SCSProtocol.Upload(
							(int)VECAddresses.REG_1_RATE_E_CUM,
							VEC_REG_1_DEM_READ_LENGTH, out abytRegisterData);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				double dDemandValue;
				double dCumDemandValue;
				DateTime TimeOfOccurence;

				dCumDemandValue = BCD.FloatingBCDtoDouble(ref abytRegisterData, 4);

				Array.Copy(abytRegisterData,
						   VEC_REG_1_READ_RATE_E_MAX_OFFSET,
						   abytTempRegisterData,
						   0,
						   VEC_DEM_READING_LENGTH);

				ReadDemandMeasurement(out dDemandValue,
									  out TimeOfOccurence,
									  abytTempRegisterData);

				QuantityToRead.TotalMaxDemand = new DemandMeasurement();
				QuantityToRead.TotalMaxDemand.Value = dDemandValue;
				QuantityToRead.TotalMaxDemand.TimeOfOccurance = TimeOfOccurence;

				QuantityToRead.CummulativeDemand = new Measurement();
				QuantityToRead.CummulativeDemand.Value = dCumDemandValue;

				QuantityToRead.ContinuousCummulativeDemand = new Measurement();
				QuantityToRead.ContinuousCummulativeDemand.Value = dCumDemandValue + dDemandValue; ;
			}
			else
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.REG_1_RATE_E_CUM,
					"Register 1 Demand Data");
				throw scsException;
			}
		}

		/// <summary>
		/// This method reads register 1 tou demand data from the meter.
		/// </summary>
		/// <param name="QuantityToRead">The quantity that will store the data that is read</param>
		/// <exception cref="SCSException">
		/// Thrown when Read Register 1 TOU Demand Data cannot be retreived from the meter.
		/// </exception> 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/22/06 jrf 8.00.00  N/A   Created
		/// 12/08/06 jrf 8.00.00  N/A   Added intitialization of QuantityToRead.TOUMaxDemand
		/// 12/08/06 jrf 8.00.00  N/A   Changed offsets for read from enumeration to constants
		/// 
		private void ReadReg1TOUDemand(ref Quantity QuantityToRead)
		{
			byte[] abytRegisterData;
			byte[] abytTempRegisterData;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
											"Read Register 1 TOU Demand Data");

			ProtocolResponse = m_SCSProtocol.Upload(
							(int)VECAddresses.REG_1_RATE_A_MAX,
							VEC_REG_1_TOU_DEM_READ_LENGTH, out abytRegisterData);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				abytTempRegisterData = new byte[VEC_DEM_READING_LENGTH];
				int[] iReadIndex = new int[4];
				int[] iCumAddress = new int[4];

				iReadIndex[0] = VEC_REG_1_TOU_DEM_READ_RATE_A_OFFSET;
				iReadIndex[1] = VEC_REG_1_TOU_DEM_READ_RATE_B_OFFSET;
				iReadIndex[2] = VEC_REG_1_TOU_DEM_READ_RATE_C_OFFSET;
				iReadIndex[3] = VEC_REG_1_TOU_DEM_READ_RATE_D_OFFSET;

				iCumAddress[0] = (int)VECAddresses.REG_1_RATE_A_CUM;
				iCumAddress[1] = (int)VECAddresses.REG_1_RATE_B_CUM;
				iCumAddress[2] = (int)VECAddresses.REG_1_RATE_C_CUM;
				iCumAddress[3] = (int)VECAddresses.REG_1_RATE_D_CUM;

				QuantityToRead.TOUMaxDemand = new List<DemandMeasurement>();
				QuantityToRead.TOUCummulativeDemand = new List<Measurement>();
				QuantityToRead.TOUCCummulativeDemand = new List<Measurement>();

				for (int iIndex = 0; iIndex < VEC_MAX_TOU_RATES; iIndex++)
				{
					Array.Copy(abytRegisterData, iReadIndex[iIndex], abytTempRegisterData, 0, VEC_DEM_READING_LENGTH);
					ReadTOUDemandMeasurement(ref QuantityToRead, abytTempRegisterData);

					Measurement CumDemandValue = new Measurement();
					CumDemandValue.Value = double.Parse(ReadFloatingBCDValue(iCumAddress[iIndex], 4), CultureInfo.CurrentCulture);

					Measurement CCumDemandValue = new Measurement();
					CCumDemandValue.Value = QuantityToRead.TOUMaxDemand[iIndex].Value + CumDemandValue.Value;

					QuantityToRead.TOUCummulativeDemand.Add(CumDemandValue);
					QuantityToRead.TOUCCummulativeDemand.Add(CCumDemandValue);
				}
			}
			else
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.REG_1_RATE_A_MAX,
					"Register 1 TOU Demand Data");
				throw scsException;
			}
		}

		/// <summary>
		/// This method reads register 2 demand/energy data from the meter.
		/// </summary>
		/// <param name="QuantityToRead">The quantity that will store the data that is read</param>
		/// <param name="eRegisterType">The type of quantity, energy or demand, being retreived</param>
		/// <exception cref="SCSException">
		/// Thrown when Read Register 2 Demand/Energy Data cannot be retreived from the meter.
		/// </exception>  
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/22/06 jrf 8.00.00  N/A   Created
		/// 
		private void ReadReg2DemandAndEnergy(ref Quantity QuantityToRead, VECRegisterType eRegisterType)
		{
			byte[] abytRegisterData;
			byte[] abytTempRegisterData;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
											"Read Register 2 Demand/Energy Data");

			ProtocolResponse = m_SCSProtocol.Upload(
							(int)VECAddresses.REG_2_RATE_E_MAX,
							VEC_REG_2_DEM_EN_READ_LENGTH, out abytRegisterData);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				if (VECRegisterType.DEMAND == eRegisterType)
				{
					double dDemandValue;
					DateTime TimeOfOccurence;

					ReadDemandMeasurement(out dDemandValue,
										  out TimeOfOccurence,
										  abytRegisterData);

					QuantityToRead.TotalMaxDemand = new DemandMeasurement();
					QuantityToRead.TotalMaxDemand.Value = dDemandValue;
					QuantityToRead.TotalMaxDemand.TimeOfOccurance = TimeOfOccurence;

					double dCumDemandValue = double.Parse(ReadFloatingBCDValue((int)VECAddresses.REG_2_RATE_E_CUM, 4), CultureInfo.CurrentCulture);

					QuantityToRead.CummulativeDemand = new Measurement();
					QuantityToRead.CummulativeDemand.Value = dCumDemandValue;

					QuantityToRead.ContinuousCummulativeDemand = new Measurement();
					QuantityToRead.ContinuousCummulativeDemand.Value = dCumDemandValue + dDemandValue; ;
				}
				else //Get energy data
				{
					abytTempRegisterData = new byte[VEC_ENERGY_READING_LENGTH];

					Array.Copy(abytRegisterData,
							   VEC_REG_2_READ_RATE_E_ENERGY_OFFSET,
							   abytTempRegisterData,
							   0,
							   VEC_ENERGY_READING_LENGTH);

					QuantityToRead.TotalEnergy = new Measurement();
					QuantityToRead.TotalEnergy.Value = BCD.FixedBCDtoFloat(ref abytTempRegisterData);
				}

			}
			else
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.REG_2_RATE_E_MAX,
					"Register 2 Demand/Energy Data");
				throw scsException;
			}
		}

		/// <summary>
		/// This method reads register 2 TOU demand/energy data from the meter.
		/// </summary>
		/// <param name="QuantityToRead">The quantity that will store the data that is read</param>
		/// <param name="eRegisterType">The type of quantity, energy or demand, being retreived</param>
		/// <exception cref="SCSException">
		/// Thrown when Read Register 2 TOU Demand/Energy Data cannot be retreived from the meter.
		/// </exception>  
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 12/08/06 jrf 8.00.00  N/A   Added initialization of QuantityToRead.TOUMaxDemand
		///                             and QuantityToRead.TOUEnergy
		/// 
		private void ReadReg2TOUDemandAndEnergy(ref Quantity QuantityToRead, VECRegisterType eRegisterType)
		{
			byte[] abytRegisterData;
			byte[] abytTempRegisterData;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
											"Read Register 2 TOU Demand/Energy Data");

			ProtocolResponse = m_SCSProtocol.Upload(
							(int)VECAddresses.REG_2_RATE_A_MAX,
							VEC_REG_2_TOU_DEM_EN_READ_LENGTH, out abytRegisterData);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				if (VECRegisterType.DEMAND == eRegisterType)
				{
					QuantityToRead.TOUMaxDemand = new List<DemandMeasurement>();

					// Read Rate A 
					abytTempRegisterData = new byte[VEC_DEM_READING_LENGTH];
					Array.Copy(abytRegisterData, abytTempRegisterData, VEC_DEM_READING_LENGTH);
					ReadTOUDemandMeasurement(ref QuantityToRead, abytTempRegisterData);

					// Read Rate B
					Array.Copy(abytRegisterData,
							   VEC_REG_2_READ_RATE_B_MAX_OFFSET,
							   abytTempRegisterData,
							   0,
							   VEC_DEM_READING_LENGTH);
					ReadTOUDemandMeasurement(ref QuantityToRead, abytTempRegisterData);

					// For Rates C & D
					ProtocolResponse = m_SCSProtocol.Upload(
							(int)VECAddresses.REG_2_RATE_C_MAX,
							VEC_REG_2_TOU_DEM_2_READ_LENGTH, out abytRegisterData);

					if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
					{
						// Read Rate C
						Array.Copy(abytRegisterData, abytTempRegisterData, VEC_DEM_READING_LENGTH);
						ReadTOUDemandMeasurement(ref QuantityToRead, abytTempRegisterData);

						// Read Rate D
						Array.Copy(abytRegisterData,
								   VEC_REG_2_READ_2_RATE_D_MAX_OFFSET,
								   abytTempRegisterData,
								   0,
								   VEC_DEM_READING_LENGTH);
						ReadTOUDemandMeasurement(ref QuantityToRead, abytTempRegisterData);
					}
					else
					{
						SCSException scsException = new SCSException(
							SCSCommands.SCS_U,
							ProtocolResponse,
							(int)VECAddresses.REG_2_RATE_C_MAX,
							"Register 2 TOU Demand Data");
						throw scsException;
					}


				}
				else //Get energy data
				{
					abytTempRegisterData = new byte[VEC_ENERGY_READING_LENGTH];
					Measurement TOUEnergyMeasurement;

					QuantityToRead.TOUEnergy = new List<Measurement>();

					for (int iIndex = 0; iIndex < VEC_MAX_TOU_RATES; iIndex++)
					{
						TOUEnergyMeasurement = new Measurement();

						Array.Copy(abytRegisterData,
								   iIndex * VEC_ENERGY_READING_LENGTH,
								   abytTempRegisterData,
								   0,
								   VEC_ENERGY_READING_LENGTH);

						TOUEnergyMeasurement.Value = BCD.FixedBCDtoFloat(ref abytTempRegisterData);
						QuantityToRead.TOUEnergy.Add(TOUEnergyMeasurement);
					}

				}
			}
			else
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.REG_2_RATE_A_MAX,
					"Register 2 TOU Demand/Energy Data");
				throw scsException;
			}
		}

		/// <summary>
		/// This method reads register 3 demand/energy data from the meter.
		/// </summary>
		/// <param name="QuantityToRead">The quantity that will store the data that is read</param>
		/// <param name="eRegisterType">The type of quantity, energy or demand, being retreived</param>
		/// <exception cref="SCSException">
		/// Thrown when Read Register 3 Demand/Energy Data cannot be retreived from the meter.
		/// </exception>  
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 
		private void ReadReg3DemandAndEnergy(ref Quantity QuantityToRead, VECRegisterType eRegisterType)
		{
			byte[] abytRegisterData;
			byte[] abytTempRegisterData;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
											"Read Register 3 Demand/Energy Data");

			ProtocolResponse = m_SCSProtocol.Upload(
							(int)VECAddresses.REG_3_RATE_E_MAX,
							VEC_REG_3_DEM_EN_READ_LENGTH, out abytRegisterData);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				if (VECRegisterType.DEMAND == eRegisterType)
				{
					double dDemandValue;
					DateTime TimeOfOccurence;

					ReadDemandMeasurement(out dDemandValue,
										  out TimeOfOccurence,
										  abytRegisterData);

					QuantityToRead.TotalMaxDemand = new DemandMeasurement();
					QuantityToRead.TotalMaxDemand.Value = dDemandValue;
					QuantityToRead.TotalMaxDemand.TimeOfOccurance = TimeOfOccurence;

                    double dCumDemandValue = double.Parse(ReadFloatingBCDValue((int)VECAddresses.REG_3_RATE_E_CUM, 4), CultureInfo.InvariantCulture);

					QuantityToRead.CummulativeDemand = new Measurement();
					QuantityToRead.CummulativeDemand.Value = dCumDemandValue;

					QuantityToRead.ContinuousCummulativeDemand = new Measurement();
					QuantityToRead.ContinuousCummulativeDemand.Value = dCumDemandValue + dDemandValue; ;

				}
				else //Get energy data
				{
					abytTempRegisterData = new byte[VEC_ENERGY_READING_LENGTH];

					Array.Copy(abytRegisterData, abytTempRegisterData, VEC_ENERGY_READING_LENGTH);

					QuantityToRead.TotalEnergy = new Measurement();
					QuantityToRead.TotalEnergy.Value = BCD.FixedBCDtoFloat(ref abytTempRegisterData);
				}

			}
			else
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.REG_3_RATE_E_MAX,
					"Register 3 Demand/Energy Data");
				throw scsException;
			}
		}

		/// <summary>
		/// This method reads register 4 demand/energy data from the meter.
		/// </summary>
		/// <param name="QuantityToRead">The quantity that will store the data that is read</param>
		/// <param name="eRegisterType">The type of quantity, energy or demand, being retreived</param>
		/// <exception cref="SCSException">
		/// Thrown when Read Register 4 Demand/Energy Data cannot be retreived from the meter.
		/// </exception> 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 
		private void ReadReg4DemandAndEnergy(ref Quantity QuantityToRead, VECRegisterType eRegisterType)
		{
			byte[] abytRegisterData;
			byte[] abytTempRegisterData;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
											"Read Register 4 Demand/Energy Data");

			ProtocolResponse = m_SCSProtocol.Upload(
							(int)VECAddresses.REG_4_RATE_E_MAX,
							VEC_REG_4_DEM_EN_READ_LENGTH, out abytRegisterData);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				if (VECRegisterType.DEMAND == eRegisterType)
				{
					double dDemandValue;
					DateTime TimeOfOccurence;

					ReadDemandMeasurement(out dDemandValue,
										  out TimeOfOccurence,
										  abytRegisterData);

					QuantityToRead.TotalMaxDemand = new DemandMeasurement();
					QuantityToRead.TotalMaxDemand.Value = dDemandValue;
					QuantityToRead.TotalMaxDemand.TimeOfOccurance = TimeOfOccurence;

                    double dCumDemandValue = double.Parse(ReadFloatingBCDValue((int)VECAddresses.REG_4_RATE_E_CUM, 4), CultureInfo.CurrentCulture);

					QuantityToRead.CummulativeDemand = new Measurement();
					QuantityToRead.CummulativeDemand.Value = dCumDemandValue;

					QuantityToRead.ContinuousCummulativeDemand = new Measurement();
					QuantityToRead.ContinuousCummulativeDemand.Value = dCumDemandValue + dDemandValue; ;
				}
				else //Get energy data
				{
					abytTempRegisterData = new byte[VEC_ENERGY_READING_LENGTH];

					Array.Copy(abytRegisterData, abytTempRegisterData, VEC_ENERGY_READING_LENGTH);

					QuantityToRead.TotalEnergy = new Measurement();
					QuantityToRead.TotalEnergy.Value = BCD.FixedBCDtoFloat(ref abytTempRegisterData);
				}

			}
			else
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)VECAddresses.REG_4_RATE_E_MAX,
					"Register 4 Demand/Energy Data");
				throw scsException;
			}
		}

		/// <summary>
		/// This method extracts one demand mesurement and time of occurance from an array of bytes
		/// read from the meter.
		/// </summary>
		/// <param name="dMeasurement">The demand measurement</param>
		/// <param name="TimeOfOccurence">The time that the demand measurement occured</param>
		/// <param name="abytRegisterData">Array of bytes containing the demand measurement data</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/22/06 jrf 8.00.00  N/A   Created
		/// 12/08/06 jrf 8.00.00  N/A   If time of occurence would be in future, set the
		///                             year to last year
		/// 
		private void ReadDemandMeasurement(out double dMeasurement, out DateTime TimeOfOccurence, byte[] abytRegisterData)
		{

			byte[] abytMeasData = new byte[VEC_DEM_READING_VALUE_LENGTH];
			int iYear = DateTime.Now.Year;
			int iMonth = 0;
			int iDay = 0;
			int iHour = 0;
			int iMinute = 0;
			int iSecond = 0;

			Array.Copy(abytRegisterData, abytMeasData, VEC_DEM_READING_VALUE_LENGTH);
			dMeasurement = BCD.FloatingBCDtoDouble(ref abytMeasData, VEC_DEM_READING_VALUE_LENGTH);

			iMonth = BCD.BCDtoByte(abytRegisterData[VEC_DEM_READING_TOO_MONTH_OFFSET]);
			iDay = BCD.BCDtoByte(abytRegisterData[VEC_DEM_READING_TOO_DAY_OFFSET]);
			iHour = BCD.BCDtoByte(abytRegisterData[VEC_DEM_READING_TOO_HOUR_OFFSET]);
			iMinute = BCD.BCDtoByte(abytRegisterData[VEC_DEM_READING_TOO_MINUTE_OFFSET]);

			try
			{
				TimeOfOccurence = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);
			}
			catch (ArgumentOutOfRangeException)
			{
				// Somethings not right, set to default time.
				TimeOfOccurence = new DateTime(1980, 1, 1);
			}

			// The time of occurence must be in the past, assume it was last year
			if (DateTime.Now < TimeOfOccurence)
			{
				TimeOfOccurence = TimeOfOccurence.AddYears(-1);

			}

		}

		/// <summary>
		/// This method extracts one TOU demand mesurement from an array of bytes
		/// read from the meter.
		/// </summary>
		/// <param name="QuantityToRead">The TOU demand measurement</param>
		/// <param name="abyRegisterData">Array of bytes containing the demand measurement data</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 
		private void ReadTOUDemandMeasurement(ref Quantity QuantityToRead, byte[] abyRegisterData)
		{
			DemandMeasurement TOUDemMeasurement = new DemandMeasurement();
			double dDemandValue;
			DateTime TimeOfOccurence;

			ReadDemandMeasurement(out dDemandValue,
								  out TimeOfOccurence,
								  abyRegisterData);

			TOUDemMeasurement.Value = dDemandValue;
			TOUDemMeasurement.TimeOfOccurance = TimeOfOccurence;
			QuantityToRead.TOUMaxDemand.Add(TOUDemMeasurement);
		}

		/// <summary>
		/// This method extracts self read register readings from a particular
		/// Self Read block indicated by the address paramater.
		/// </summary>
		/// <param name="eBlockAddress">The address of the Self Read block to read</param>
		/// <param name="SRBlock">The Self Read data to pass back to caller</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 12/08/06 jrf 8.00.00  N/A   If time of self read would be in future, set the
		///                             year to last year
		/// 
		private void ReadSelfReadBlock(VECAddresses eBlockAddress, ref QuantityCollection SRBlock)
		{
			byte[] abytRegisterData;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
											"Read Self Read Block");

			ProtocolResponse = m_SCSProtocol.Upload(
							(int)eBlockAddress,
							VEC_SR_DATA_BLOCK_LENGTH, out abytRegisterData);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				ReadRegisterMappings();

				int iYear = DateTime.Now.Year;
				int iMonth = 0;
				int iDay = 0;
				int iHour = 0;
				int iMinute = 0;
				int iSecond = 0;

				iMonth = BCD.BCDtoByte(abytRegisterData[VEC_SR_BLOCK_TIME_MONTH_OFFSET]);
				iDay = BCD.BCDtoByte(abytRegisterData[VEC_SR_BLOCK_TIME_DAY_OFFSET]);
				iHour = BCD.BCDtoByte(abytRegisterData[VEC_SR_BLOCK_TIME_HOUR_OFFSET]);
				iMinute = BCD.BCDtoByte(abytRegisterData[VEC_SR_BLOCK_TIME_MINUTE_OFFSET]);

				try
				{
					SRBlock.DateTimeOfReading = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);
				}
				catch (ArgumentOutOfRangeException)
				{
					// Somethings not right, set to default time.
					SRBlock.DateTimeOfReading = new DateTime(1980, 1, 1);
				}

				// The time of occurence must be in the past, assume it was last year
				if (DateTime.Now < SRBlock.DateTimeOfReading)
				{
					SRBlock.DateTimeOfReading = SRBlock.DateTimeOfReading.AddYears(-1);
				}

				Quantity SRRegQty;

				ReadReg1SRQuantity(out SRRegQty, abytRegisterData);

				if (null != SRRegQty)
				{
					SRBlock.Quantities.Add(SRRegQty);
				}

				ReadReg2SRQuantity(out SRRegQty, abytRegisterData);

				if (null != SRRegQty)
				{
					SRBlock.Quantities.Add(SRRegQty);
				}

				ReadReg3SRQuantity(out SRRegQty, abytRegisterData);

				if (null != SRRegQty)
				{
					SRBlock.Quantities.Add(SRRegQty);
				}

				ReadReg4SRQuantity(out SRRegQty, abytRegisterData);

				if (null != SRRegQty)
				{
					SRBlock.Quantities.Add(SRRegQty);
				}
			}
			else
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_U,
					ProtocolResponse,
					(int)eBlockAddress,
					"Self Read Data Block");
				throw scsException;
			}

		}

		/// <summary>
		/// This method extracts self read register 1 quantity from the self read block
		/// </summary>
		/// <param name="SRRegQty1">The quantity to read from self read block</param>
		/// <param name="abytSRRegData">The data from the self read block</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 12/08/06 jrf 8.00.00  N/A   Added initialization of SRRegQty1.TOUMaxDemand
		/// 
		private void ReadReg1SRQuantity(out Quantity SRRegQty1, byte[] abytSRRegData)
		{
			String strQtyDescription;
			String strRegDescription;
			VECRegisterType eRegisterType;
			byte[] abytTempSRRegData;
			double dMeasurement;
			DateTime TimeOfOccurence;

			SRRegQty1 = null;

			if (VECRegQuantities.NO_QTY != m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E])
			{
				DetermineQtyDescRegDescAndRegType(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E],
												  out strQtyDescription,
												  out strRegDescription,
												  out eRegisterType);
				SRRegQty1 = new Quantity(strQtyDescription);

				abytTempSRRegData = new byte[VEC_DEM_READING_LENGTH];

				// self read demand values are 3 bytes long instead of 4, add extra zero byte so 
				// we can use ReadDemandMeasurement() which reads a 4 byte demand measurement
				abytTempSRRegData[3] = 0;

				Array.Copy(abytSRRegData,
						   VEC_SR_REG_1_RATE_E_MAX_OFFSET,
						   abytTempSRRegData,
						   0,
						   VEC_SR_DEM_READING_VALUE_LENGTH);
				Array.Copy(abytSRRegData,
						   VEC_SR_REG_1_RATE_E_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
						   abytTempSRRegData,
						   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
						   VEC_DEM_READING_TOO_LENGTH);

				ReadDemandMeasurement(out dMeasurement, out TimeOfOccurence, abytTempSRRegData);

				SRRegQty1.TotalMaxDemand = new DemandMeasurement();
				SRRegQty1.TotalMaxDemand.Value = dMeasurement;
				SRRegQty1.TotalMaxDemand.TimeOfOccurance = TimeOfOccurence;
				SRRegQty1.TotalMaxDemand.Description = VEC_MAX + " " + strRegDescription;

				if (TOUEnabled && VECRegQuantities.NO_QTY != m_eRegQuantities[(int)VECRegisters.REG_1_TOU])
				{
					abytTempSRRegData = new byte[VEC_DEM_READING_LENGTH];
					SRRegQty1.TOUMaxDemand = new List<DemandMeasurement>();

					// Read Rate A
					abytTempSRRegData[3] = 0;
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_1_RATE_A_MAX_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_DEM_READING_VALUE_LENGTH);
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_1_RATE_A_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
							   abytTempSRRegData,
							   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
							   VEC_DEM_READING_TOO_LENGTH);

					ReadTOUDemandMeasurement(ref SRRegQty1, abytTempSRRegData);

					SRRegQty1.TOUMaxDemand[0].Description = VEC_MAX + " " + strRegDescription + " " + VEC_RATE_A;

					// Read Rate B
					abytTempSRRegData[3] = 0;
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_1_RATE_B_MAX_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_DEM_READING_VALUE_LENGTH);
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_1_RATE_B_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
							   abytTempSRRegData,
							   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
							   VEC_DEM_READING_TOO_LENGTH);

					ReadTOUDemandMeasurement(ref SRRegQty1, abytTempSRRegData);

					SRRegQty1.TOUMaxDemand[1].Description = VEC_MAX + " " + strRegDescription + " " + VEC_RATE_B;

					// Read Rate C
					abytTempSRRegData[3] = 0;
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_1_RATE_C_MAX_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_DEM_READING_VALUE_LENGTH);
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_1_RATE_C_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
							   abytTempSRRegData,
							   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
							   VEC_DEM_READING_TOO_LENGTH);

					ReadTOUDemandMeasurement(ref SRRegQty1, abytTempSRRegData);

					SRRegQty1.TOUMaxDemand[2].Description = VEC_MAX + " " + strRegDescription + " " + VEC_RATE_C;

					// Read Rate D
					abytTempSRRegData[3] = 0;
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_1_RATE_D_MAX_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_DEM_READING_VALUE_LENGTH);
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_1_RATE_D_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
							   abytTempSRRegData,
							   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
							   VEC_DEM_READING_TOO_LENGTH);

					ReadTOUDemandMeasurement(ref SRRegQty1, abytTempSRRegData);

					SRRegQty1.TOUMaxDemand[3].Description = VEC_MAX + " " + strRegDescription + " " + VEC_RATE_D;
				}

			}
		}

		/// <summary>
		/// This method extracts self read register 2 quantity from the self read block
		/// </summary>
		/// <param name="SRRegQty2">The quantity to read from self read block</param>
		/// <param name="abytSRRegData">The data from the self read block</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 12/08/06 jrf 8.00.00  N/A   Added inititalization of SRRegQty2.TOUMaxDemand
		///                             and SRRegQty2.TOUEnergy
		/// 
		private void ReadReg2SRQuantity(out Quantity SRRegQty2, byte[] abytSRRegData)
		{
			String strQtyDescription;
			String strRegDescription;
			VECRegisterType eRegisterType;
			byte[] abytTempSRRegData;
			double dMeasurement;
			DateTime TimeOfOccurence;

			SRRegQty2 = null;

			if (VECRegQuantities.NO_QTY != m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E])
			{
				DetermineQtyDescRegDescAndRegType(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E],
												  out strQtyDescription,
												  out strRegDescription,
												  out eRegisterType);
				SRRegQty2 = new Quantity(strQtyDescription);

				if (VECRegisterType.DEMAND == eRegisterType)
				{
					abytTempSRRegData = new byte[VEC_DEM_READING_LENGTH];

					// self read demand values are 3 bytes long instead of 4, add extra zero byte so 
					// we can use ReadDemandMeasurement() which reads a 4 byte demand measurement
					abytTempSRRegData[3] = 0;

					Array.Copy(abytSRRegData,
							   VEC_SR_REG_2_RATE_E_MAX_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_DEM_READING_VALUE_LENGTH);
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_2_RATE_E_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
							   abytTempSRRegData,
							   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
							   VEC_DEM_READING_TOO_LENGTH);

					ReadDemandMeasurement(out dMeasurement, out TimeOfOccurence, abytTempSRRegData);

					SRRegQty2.TotalMaxDemand = new DemandMeasurement();
					SRRegQty2.TotalMaxDemand.Value = dMeasurement;
					SRRegQty2.TotalMaxDemand.TimeOfOccurance = TimeOfOccurence;
					SRRegQty2.TotalMaxDemand.Description = VEC_MAX + " " + strRegDescription;


					if (TOUEnabled && VECRegQuantities.NO_QTY != m_eRegQuantities[(int)VECRegisters.REG_2_TOU])
					{
						abytTempSRRegData = new byte[VEC_DEM_READING_LENGTH];
						SRRegQty2.TOUMaxDemand = new List<DemandMeasurement>();

						// Read Rate A
						abytTempSRRegData[3] = 0;
						Array.Copy(abytSRRegData,
								   VEC_SR_REG_2_RATE_A_MAX_OFFSET,
								   abytTempSRRegData,
								   0,
								   VEC_SR_DEM_READING_VALUE_LENGTH);
						Array.Copy(abytSRRegData,
								   VEC_SR_REG_2_RATE_A_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
								   abytTempSRRegData,
								   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
								   VEC_DEM_READING_TOO_LENGTH);

						ReadTOUDemandMeasurement(ref SRRegQty2, abytTempSRRegData);

						SRRegQty2.TOUMaxDemand[0].Description = VEC_MAX + " " + strRegDescription + " " + VEC_RATE_A;

						// Read Rate B
						abytTempSRRegData[3] = 0;
						Array.Copy(abytSRRegData,
								   VEC_SR_REG_2_RATE_B_MAX_OFFSET,
								   abytTempSRRegData,
								   0,
								   VEC_SR_DEM_READING_VALUE_LENGTH);
						Array.Copy(abytSRRegData,
								   VEC_SR_REG_2_RATE_B_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
								   abytTempSRRegData,
								   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
								   VEC_DEM_READING_TOO_LENGTH);

						ReadTOUDemandMeasurement(ref SRRegQty2, abytTempSRRegData);

						SRRegQty2.TOUMaxDemand[1].Description = VEC_MAX + " " + strRegDescription + " " + VEC_RATE_B;

						// Read Rate C
						abytTempSRRegData[3] = 0;
						Array.Copy(abytSRRegData,
								   VEC_SR_REG_2_RATE_C_MAX_OFFSET,
								   abytTempSRRegData,
								   0,
								   VEC_SR_DEM_READING_VALUE_LENGTH);
						Array.Copy(abytSRRegData,
								   VEC_SR_REG_2_RATE_C_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
								   abytTempSRRegData,
								   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
								   VEC_DEM_READING_TOO_LENGTH);

						ReadTOUDemandMeasurement(ref SRRegQty2, abytTempSRRegData);

						SRRegQty2.TOUMaxDemand[2].Description = VEC_MAX + " " + strRegDescription + " " + VEC_RATE_C;

						// Read Rate D
						abytTempSRRegData[3] = 0;
						Array.Copy(abytSRRegData,
								   VEC_SR_REG_2_RATE_D_MAX_OFFSET,
								   abytTempSRRegData,
								   0,
								   VEC_SR_DEM_READING_VALUE_LENGTH);
						Array.Copy(abytSRRegData,
								   VEC_SR_REG_2_RATE_D_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
								   abytTempSRRegData,
								   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
								   VEC_DEM_READING_TOO_LENGTH);

						ReadTOUDemandMeasurement(ref SRRegQty2, abytTempSRRegData);

						SRRegQty2.TOUMaxDemand[3].Description = VEC_MAX + " " + strRegDescription + " " + VEC_RATE_D;
					}
				}
				else //Get energy data
				{
					abytTempSRRegData = new byte[VEC_SR_ENERGY_READING_LENGTH];

					Array.Copy(abytSRRegData,
							   VEC_SR_REG_2_RATE_E_ENERGY_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_ENERGY_READING_LENGTH);

					SRRegQty2.TotalEnergy = new Measurement();
					SRRegQty2.TotalEnergy.Value = BCD.FloatingBCDtoDouble(ref abytTempSRRegData,
																		  VEC_SR_ENERGY_READING_LENGTH);
					SRRegQty2.TotalEnergy.Description = strRegDescription;

					if (TOUEnabled && VECRegQuantities.NO_QTY != m_eRegQuantities[(int)VECRegisters.REG_2_TOU])
					{
						abytTempSRRegData = new byte[VEC_SR_ENERGY_READING_LENGTH];
						Measurement TOUEnergyMeasurement;
						SRRegQty2.TOUEnergy = new List<Measurement>();

						for (int iIndex = 0; iIndex < VEC_MAX_TOU_RATES; iIndex++)
						{
							TOUEnergyMeasurement = new Measurement();

							Array.Copy(abytSRRegData,
									   VEC_SR_REG_2_RATE_A_ENERGY_OFFSET +
									   (iIndex * VEC_SR_ENERGY_READING_RESV_LENGTH),
									   abytTempSRRegData,
									   0,
									   VEC_SR_ENERGY_READING_LENGTH);

							TOUEnergyMeasurement.Value = BCD.FloatingBCDtoDouble(ref abytTempSRRegData,
																				 VEC_SR_ENERGY_READING_LENGTH);
							SRRegQty2.TOUEnergy.Add(TOUEnergyMeasurement);
						}

						SRRegQty2.TOUEnergy[0].Description = strRegDescription + " " + VEC_RATE_A;
						SRRegQty2.TOUEnergy[1].Description = strRegDescription + " " + VEC_RATE_B;
						SRRegQty2.TOUEnergy[2].Description = strRegDescription + " " + VEC_RATE_C;
						SRRegQty2.TOUEnergy[3].Description = strRegDescription + " " + VEC_RATE_D;
					}
				}

			}
		}

		/// <summary>
		/// This method extracts self read register 3 quantity from the self read block
		/// </summary>
		/// <param name="SRRegQty3">The quantity to read from self read block</param>
		/// <param name="abytSRRegData">The data from the self read block</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 
		private void ReadReg3SRQuantity(out Quantity SRRegQty3, byte[] abytSRRegData)
		{
			String strQtyDescription;
			String strRegDescription;
			VECRegisterType eRegisterType;
			byte[] abytTempSRRegData;
			double dMeasurement;
			DateTime TimeOfOccurence;

			SRRegQty3 = null;

			if (VECRegQuantities.NO_QTY != m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E])
			{
				DetermineQtyDescRegDescAndRegType(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E],
												  out strQtyDescription,
												  out strRegDescription,
												  out eRegisterType);
				SRRegQty3 = new Quantity(strQtyDescription);

				if (VECRegisterType.DEMAND == eRegisterType)
				{
					abytTempSRRegData = new byte[VEC_DEM_READING_LENGTH];

					// self read demand values are 3 bytes long instead of 4, add extra zero byte so 
					// we can use ReadDemandMeasurement() which reads a 4 byte demand measurement
					abytTempSRRegData[3] = 0;

					Array.Copy(abytSRRegData,
							   VEC_SR_REG_3_RATE_E_MAX_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_DEM_READING_VALUE_LENGTH);
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_3_RATE_E_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
							   abytTempSRRegData,
							   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
							   VEC_DEM_READING_TOO_LENGTH);

					ReadDemandMeasurement(out dMeasurement, out TimeOfOccurence, abytTempSRRegData);

					SRRegQty3.TotalMaxDemand = new DemandMeasurement();
					SRRegQty3.TotalMaxDemand.Value = dMeasurement;
					SRRegQty3.TotalMaxDemand.TimeOfOccurance = TimeOfOccurence;
					SRRegQty3.TotalMaxDemand.Description = VEC_MAX + " " + strRegDescription;
				}
				else //Get energy data
				{
					abytTempSRRegData = new byte[VEC_SR_ENERGY_READING_LENGTH];

					Array.Copy(abytSRRegData,
							   VEC_SR_REG_3_RATE_E_ENERGY_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_ENERGY_READING_LENGTH);

					SRRegQty3.TotalEnergy = new Measurement();
					SRRegQty3.TotalEnergy.Value = BCD.FloatingBCDtoDouble(ref abytTempSRRegData,
																		  VEC_SR_ENERGY_READING_LENGTH);
					SRRegQty3.TotalEnergy.Description = strRegDescription;
				}
			}
		}

		/// <summary>
		/// This method extracts self read register 4 quantity from the self read block
		/// </summary>
		/// <param name="SRRegQty4">The quantity to read from self read block</param>
		/// <param name="abytSRRegData">The data from the self read block</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 
		private void ReadReg4SRQuantity(out Quantity SRRegQty4, byte[] abytSRRegData)
		{
			String strQtyDescription;
			String strRegDescription;
			VECRegisterType eRegisterType;
			byte[] abytTempSRRegData;
			SRRegQty4 = null;
			double dMeasurement;
			DateTime TimeOfOccurence;

			if (VECRegQuantities.NO_QTY != m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E])
			{
				DetermineQtyDescRegDescAndRegType(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E],
												  out strQtyDescription,
												  out strRegDescription,
												  out eRegisterType);
				SRRegQty4 = new Quantity(strQtyDescription);

				if (VECRegisterType.DEMAND == eRegisterType)
				{
					abytTempSRRegData = new byte[VEC_DEM_READING_LENGTH];

					// self read demand values are 3 bytes long instead of 4, add extra zero byte so 
					// we can use ReadDemandMeasurement() which reads a 4 byte demand measurement
					abytTempSRRegData[3] = 0;

					Array.Copy(abytSRRegData,
							   VEC_SR_REG_4_RATE_E_MAX_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_DEM_READING_VALUE_LENGTH);
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_4_RATE_E_MAX_OFFSET + VEC_SR_DEM_READING_VALUE_LENGTH,
							   abytTempSRRegData,
							   VEC_SR_DEM_READING_VALUE_LENGTH + 1,
							   VEC_DEM_READING_TOO_LENGTH);
					ReadDemandMeasurement(out dMeasurement, out TimeOfOccurence, abytTempSRRegData);

					SRRegQty4.TotalMaxDemand = new DemandMeasurement();
					SRRegQty4.TotalMaxDemand.Value = dMeasurement;
					SRRegQty4.TotalMaxDemand.TimeOfOccurance = TimeOfOccurence;
					SRRegQty4.TotalMaxDemand.Description = VEC_MAX + " " + strRegDescription;
				}
				else //Get energy data
				{
					abytTempSRRegData = new byte[VEC_SR_ENERGY_READING_LENGTH];
					Array.Copy(abytSRRegData,
							   VEC_SR_REG_4_RATE_E_ENERGY_OFFSET,
							   abytTempSRRegData,
							   0,
							   VEC_SR_ENERGY_READING_LENGTH);
					SRRegQty4.TotalEnergy = new Measurement();
					SRRegQty4.TotalEnergy.Value = BCD.FloatingBCDtoDouble(ref abytTempSRRegData,
																		  VEC_SR_ENERGY_READING_LENGTH);
					SRRegQty4.TotalEnergy.Description = strRegDescription;
				}
			}
		}

		/// <summary>
		/// This method determines the quantity description, register description
		/// and the register type based on the given quantity type
		/// </summary>
		/// <param name="eQtyType">The quantity type used to determine the description 
		/// and register type</param>
		/// <param name="strQtyDescription">The description of the quantity</param>
		/// <param name="strRegDescription">The description of the register</param>
		/// <param name="eRegisterType">The register type of the quantity, energy or demand</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/27/06 jrf 8.00.00  N/A   Created
		/// 12/12/06 jrf 8.00.00  N/A   Added "lag" and "lead" to descriptions
		/// 
		private void DetermineQtyDescRegDescAndRegType(VECRegQuantities eQtyType,
			out String strQtyDescription, out String strRegDescription, out VECRegisterType eRegisterType)
		{
			switch (eQtyType)
			{
				case VECRegQuantities.WATT_HOUR:
				{
					strQtyDescription = VEC_WATTS;
					strRegDescription = VEC_KWH;
					eRegisterType = VECRegisterType.ENERGY;
					break;
				}
				case VECRegQuantities.VAR_HOUR_LAG:
				{
					strQtyDescription = VEC_VAR_RECEIVED;
					strRegDescription = VEC_KVARH + " " + VEC_LAG;
					eRegisterType = VECRegisterType.ENERGY;
					break;
				}
				case VECRegQuantities.VAR_HOUR_LEAD:
				{
					strQtyDescription = VEC_VAR_DELIVERED;
					strRegDescription = VEC_KVARH + " " + VEC_LEAD;
					eRegisterType = VECRegisterType.ENERGY;
					break;
				}
				case VECRegQuantities.VA_HOUR_LAG:
				{
					strQtyDescription = VEC_VA_RECEIVED;
					strRegDescription = VEC_KVAH + " " + VEC_LAG;
					eRegisterType = VECRegisterType.ENERGY;
					break;
				}
				case VECRegQuantities.WATT:
				{
					strQtyDescription = VEC_WATTS;
					strRegDescription = VEC_KW;
					eRegisterType = VECRegisterType.DEMAND;
					break;
				}
				case VECRegQuantities.WATT_TOU:
				{
					strQtyDescription = VEC_WATTS;
					strRegDescription = VEC_KW;
					eRegisterType = VECRegisterType.DEMAND;
					break;
				}
				case VECRegQuantities.VAR_LAG:
				{
					strQtyDescription = VEC_VAR_RECEIVED;
					strRegDescription = VEC_KVAR + " " + VEC_LAG;
					eRegisterType = VECRegisterType.DEMAND;
					break;
				}
				case VECRegQuantities.VA_LAG:
				{
					strQtyDescription = VEC_VA_RECEIVED;
					strRegDescription = VEC_KVA + " " + VEC_LAG;
					eRegisterType = VECRegisterType.DEMAND;
					break;
				}
				case VECRegQuantities.VA_TOTAL:
				{
					strQtyDescription = VEC_VA_TOTAL;
					strRegDescription = VEC_KVA;
					eRegisterType = VECRegisterType.DEMAND;
					break;
				}
				default:
				{
					strQtyDescription = "";
					strRegDescription = "";
					eRegisterType = VECRegisterType.DEMAND;
					break;
				}
			}
		}

		/// <summary>
		/// This method reads the form factor from the SCS Device.
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the form factor cannot be retreived from the meter.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/06/07 mrj 8.00.10		Created
		//  
		private void ReadMeterForm()
		{
			byte[] byFormFactor;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Form Factor");
			// Get the form factor byte
			ProtocolResponse = m_SCSProtocol.Upload((int)VECAddresses.FORM_FACTOR, 1, out byFormFactor);

			// Assign the appropriate form factor string
			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				m_MeterForm.Value = byFormFactor[0];
			}
			else
			{
				SCSException objSCSException = new SCSException(SCSCommands.SCS_U,
																ProtocolResponse,
																(int)VECAddresses.FORM_FACTOR,
																"Form Factor");
				throw objSCSException;
			}
		}

		/// <summary>
		/// This method reads the meter configuration from the SCS device. 
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the transformer ratio cannot be retreived from the meter.
		/// </exception>
		/// <remarks>
		/// This method currently reads only the VA calculation.  This bit is only
		/// recognized by the meter if the meter is in a delta service.
		/// </remarks>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/09/07 mrj 8.00.11		Created
		//  
		protected virtual void ReadMeterConfiguration()
		{
			byte[] byMeterConfiguration;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Meter Configuration");
			ProtocolResponse = m_SCSProtocol.Upload((int)VECAddresses.METER_CONFIGURATION, 1, out byMeterConfiguration);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				//True is vectorial and false is arith (RMS)
				m_VACalculation.Value = (0 != (byMeterConfiguration[0] & VA_CALCULATION_MASK));
			}
			else
			{
				SCSException scsException = new SCSException(SCSCommands.SCS_U,
															 ProtocolResponse,
															 TransformerRatioAddress,
															 "Meter Configuration");
				throw scsException;
			}
		}

		#endregion Private Methods

		#region Members

		// Member Variables
		private Toolbox m_Toolbox;
		private CDiagnostics m_Diag;
		private CachedString m_serviceType;		
		private CachedInt m_MeterForm;		
		private CachedBool m_VACalculation;


		private VECRegisters m_eWhMapping;
		private VECRegisters m_eVarhLagMapping;
		private VECRegisters m_eVarhLeadMapping;
		private VECRegisters m_eVAhLagMapping;
		private VECRegisters m_eWMapping;
		private VECRegisters m_eWTOUMapping;
		private VECRegisters m_eVarLagMapping;
		private VECRegisters m_eVALagMapping;
		private VECRegisters m_eVATotalMapping;
		private bool m_blnRetreivedRegMapping;
		private VECRegQuantities[] m_eRegQuantities;

		#endregion Members

    } //End class VECTRON

} //End namespace Itron.Metering.Device
