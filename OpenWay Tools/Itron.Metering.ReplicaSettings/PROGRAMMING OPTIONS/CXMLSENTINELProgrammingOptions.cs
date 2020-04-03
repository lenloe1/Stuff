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
//                              Copyright © 2004 - 2005
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// ReplicaSettings public enumeration
	/// </summary>
	public enum ProgrammingOptionsUnitIDChoices
	{
		/// <summary>
		/// DO_NOT_PROMPT = 0,
		/// </summary>
		DO_NOT_PROMPT = 0,
		/// <summary>
		/// USE_EXISTING = 1,
		/// </summary>
		USE_EXISTING = 1,
		/// <summary>
		/// USE_METER_SERIAL_NUMBER = 2,
		/// </summary>
		USE_METER_SERIAL_NUMBER = 2,
		/// <summary>
		/// USE_MANUFACTURERS_SERIAL_NUMBER = 3
		/// </summary>
		USE_MANUFACTURERS_SERIAL_NUMBER = 3
	}

	/// <summary>
	/// ReplicaSettings public enumeration
	/// </summary>
	public enum ProgrammingOptionsMeterSerialNumberChoices
	{
		/// <summary>
		/// DO_NOT_PROMPT = 0,
		/// </summary>
		DO_NOT_PROMPT = 0,
		/// <summary>
		/// USE_EXISTING = 1,
		/// </summary>
		USE_EXISTING = 1,
		/// <summary>
		/// USE_MANUFACTURERS_SERIAL_NUMBER = 2
		/// </summary>
		USE_MANUFACTURERS_SERIAL_NUMBER = 2
	}

	/// <summary>
	/// SENTINEL Programming Options XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class CXMLSENTINELProgrammingOptions : Itron.Metering.ReplicaSettings.CXMLDeviceOptions
	{
		/// <summary>
		/// protected const string XML_NODE_LOAD_RESEARCH_ID = "LoadResearchID";
		/// </summary>
		protected const string XML_NODE_LOAD_RESEARCH_ID = "LoadResearchID";
		/// <summary>
		/// protected const string XML_NODE_EMPLOYEE_ID = "EmployeeID";
		/// </summary>
		protected const string XML_NODE_EMPLOYEE_ID = "EmployeeID";
		/// <summary>
		/// protected const string XML_NODE_LOAD_PROFILE_PULSE_WEIGHTS = "LoadProfilePulseWeights";
		/// </summary>
		protected const string XML_NODE_LOAD_PROFILE_PULSE_WEIGHTS = "LoadProfilePulseWeights";
		/// <summary>
		/// protected const string XML_NODE_REGISTER_FULL_SCALE = "RegisterFullScale";
		/// </summary>
		protected const string XML_NODE_REGISTER_FULL_SCALE = "RegisterFullScale";
		/// <summary>
		/// protected const string XML_NODE_OPTION_BOARD_CONFIG = "OptionBoardConfig";
		/// </summary>
		protected const string XML_NODE_OPTION_BOARD_CONFIG = "OptionBoardConfig";
		/// <summary>
		/// protected const string XML_NODE_SERVICE_CONFIG = "ServiceConfig";
		/// </summary>
		protected const string XML_NODE_SERVICE_CONFIG = "ServiceConfig";
		/// <summary>
		/// protected const string XML_NODE_PEAK_DEMAND_CURRENT = "PeakDemandCurrent";
		/// </summary>
		protected const string XML_NODE_PEAK_DEMAND_CURRENT = "PeakDemandCurrent";
		/// <summary>
		/// protected const string XML_NODE_SET_CUSTOM_SCHED_NAME = "SetCustomSchedName";
		/// </summary>
		protected const string XML_NODE_SET_CUSTOM_SCHED_NAME = "SetCustomSchedName";
		/// <summary>
		/// protected const string XML_NODE_SITE_SCAN_DIAG_6 = "SiteScanDiag6";
		/// </summary>
		protected const string XML_NODE_SITE_SCAN_DIAG_6 = "SiteScanDiag6";
		/// <summary>
		/// protected const string XML_NODE_RESET_POWER_OUTAGES = "ResetPowerOutages";
		/// </summary>
		protected const string XML_NODE_RESET_POWER_OUTAGES = "ResetPowerOutages";
		/// <summary>
		/// protected const string XML_NODE_RESET_DEMAND_RESETS = "ResetDemandResets";
		/// </summary>
		protected const string XML_NODE_RESET_DEMAND_RESETS = "ResetDemandResets";
		/// <summary>
		/// protected const string XML_NODE_PRIMARY_SECURITY_CODE = "PrimarySecurityCode";
		/// </summary>
		protected const string XML_NODE_PRIMARY_SECURITY_CODE = "PrimarySecurityCode";
		/// <summary>
		/// protected const string XML_NODE_LIMITED_SECURITY_CODE = "LimitedSecurityCode";
		/// </summary>
		protected const string XML_NODE_LIMITED_SECURITY_CODE = "LimitedSecurityCode";
		/// <summary>
		/// protected const string XML_NODE_SECONDARY_SECURITY_CODE = "SecondarySecurityCode";
		/// </summary>
		protected const string XML_NODE_SECONDARY_SECURITY_CODE = "SecondarySecurityCode";
		/// <summary>
		/// protected const string XML_NODE_TERTIARY_SECURITY_CODE = "TertiarySecurityCode";
		/// </summary>
		protected const string XML_NODE_TERTIARY_SECURITY_CODE = "TertiarySecurityCode";
		/// <summary>
		/// protected const string XML_NODE_NORMAL_MODE_KH = "NormalModeKh";
		/// </summary>
		protected const string XML_NODE_NORMAL_MODE_KH = "NormalModeKh";
		/// <summary>
		/// protected const string XML_NODE_TEST_MODE_KH = "TestModeKh";
		/// </summary>
		protected const string XML_NODE_TEST_MODE_KH = "TestModeKh";
		/// <summary>
		/// protected const string XML_NODE_IO_PULSE_WEIGHTS = "IOPulseWeights";
		/// </summary>
		protected const string XML_NODE_IO_PULSE_WEIGHTS = "IOPulseWeights";
		/// <summary>
		/// protected const string XML_NODE_SITE_SCAN_DIAG_1 = "SiteScanDiag1";
		/// </summary>
		protected const string XML_NODE_SITE_SCAN_DIAG_1 = "SiteScanDiag1";
		/// <summary>
		/// protected const string XML_NODE_SITE_SCAN_DIAG_2 = "SiteScanDiag2";
		/// </summary>
		protected const string XML_NODE_SITE_SCAN_DIAG_2 = "SiteScanDiag2";
		/// <summary>
		/// protected const string XML_NODE_SITE_SCAN_DIAG_3 = "SiteScanDiag3";
		/// </summary>
		protected const string XML_NODE_SITE_SCAN_DIAG_3 = "SiteScanDiag3";
		/// <summary>
		/// protected const string XML_NODE_SITE_SCAN_DIAG_4 = "SiteScanDiag4";
		/// </summary>
		protected const string XML_NODE_SITE_SCAN_DIAG_4 = "SiteScanDiag4";
		/// <summary>
		/// protected const string XML_NODE_SITE_SCAN_DIAG_5 = "SiteScanDiag5";
		/// </summary>
		protected const string XML_NODE_SITE_SCAN_DIAG_5 = "SiteScanDiag5";
		/// <summary>
		/// protected const string XML_NODE_TIME_ON_BATTERY = "TimeOnBattery";
		/// </summary>
		protected const string XML_NODE_TIME_ON_BATTERY = "TimeOnBattery";
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLSENTINELProgrammingOptions( CXMLSettings XMLSettings ) : base( "SENTINEL", XMLSettings )
		{
		}

		/// <summary>
		/// SENTINEL Load Research IDProgramming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool LoadResearchID
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LOAD_RESEARCH_ID, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LOAD_RESEARCH_ID, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Employee ID Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool EmployeeID
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_EMPLOYEE_ID, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_EMPLOYEE_ID, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Load Profile Pulse Weights Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool LoadProfilePulseWeights
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LOAD_PROFILE_PULSE_WEIGHTS, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LOAD_PROFILE_PULSE_WEIGHTS, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Registe Full Scale Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool RegisterFullScale
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_REGISTER_FULL_SCALE, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_REGISTER_FULL_SCALE, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Option Board Config Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool OptionBoardConifg
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_OPTION_BOARD_CONFIG, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_OPTION_BOARD_CONFIG, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Service Config Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool ServiceConfig
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SERVICE_CONFIG, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SERVICE_CONFIG, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Peak Demand Current Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool PeakDemandCurrent
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PEAK_DEMAND_CURRENT, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PEAK_DEMAND_CURRENT, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Set Custom Schedule Name Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SetCustomSchedName
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SET_CUSTOM_SCHED_NAME, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SET_CUSTOM_SCHED_NAME, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL SiteScan Diag 1 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SiteScanDiag1
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_1, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_1, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL SiteScan Diag 2 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SiteScanDiag2
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_2, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_2, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL SiteScan Diag 3 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SiteScanDiag3
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_3, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_3, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL SiteScan Diag 4 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SiteScanDiag4
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_4, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_4, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL SiteScan Diag 5 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SiteScanDiag5
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_5, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_5, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL SiteScan Diag 6 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SiteScanDiag6
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_6, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SITE_SCAN_DIAG_6, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Primary Security Code Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool PrimarySecurityCode
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PRIMARY_SECURITY_CODE, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PRIMARY_SECURITY_CODE, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Limited Security Code Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool LimitedSecurityCode
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LIMITED_SECURITY_CODE, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LIMITED_SECURITY_CODE, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Secondary Security Code Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SecondarySecurityCode
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SECONDARY_SECURITY_CODE, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SECONDARY_SECURITY_CODE, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Tertiary Security Code Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool TertiarySecurityCode
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TERTIARY_SECURITY_CODE, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TERTIARY_SECURITY_CODE, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Normal Mode Kh Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool NormalModeKh
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_NORMAL_MODE_KH, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_NORMAL_MODE_KH, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Test Mode Kh Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool TestModeKh
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TEST_MODE_KH, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TEST_MODE_KH, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL I/O Pulse Weights Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool IOPulseWeights
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_IO_PULSE_WEIGHTS, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_IO_PULSE_WEIGHTS, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Q1000 Reset Power Outages Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool ResetPowerOutages
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_RESET_POWER_OUTAGES, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_RESET_POWER_OUTAGES, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}
		
		/// <summary>
		/// Q1000 Reset Demand Resets Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool ResetDemandResets
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_RESET_DEMAND_RESETS, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_RESET_DEMAND_RESETS, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// SENTINEL Time On Battery Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///11/03/04 REM 7.00.27 N/A    Initial Release
		///</pre></remarks>
		public virtual bool TimeOnBattery
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TIME_ON_BATTERY, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TIME_ON_BATTERY, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}
		
		/// <summary>
		/// SENTINEL Meter Serial Number Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///01/25/05 REM 7.10.06 1281   Add the ability to automatically fill in unit ID or Meter serial number
		///</pre></remarks>
		public override int MeterSerialNumber
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_METER_SERIAL_NUMBER, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_METER_SERIAL_NUMBER, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}
	}
}
