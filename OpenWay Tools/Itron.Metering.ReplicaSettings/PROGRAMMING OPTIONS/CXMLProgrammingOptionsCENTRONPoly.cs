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
	/// CENTRON Poly Programming Options XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
	///</pre></remarks>
	public class CXMLProgrammingOptionsCENTRONPoly : Itron.Metering.ReplicaSettings.CXMLDeviceOptions
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
		/// protected const string XML_NODE_SERVICE_CONFIG = "ServiceConfig";
		/// </summary>
		protected const string XML_NODE_SERVICE_CONFIG = "ServiceConfig";
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
		/// protected const string XML_NODE_BATTERY_CARRYOVER = "BatteryHoldover";
		/// </summary>
		protected const string XML_NODE_BATTERY_CARRYOVER = "BatteryHoldover";
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
		///</pre></remarks>
		public CXMLProgrammingOptionsCENTRONPoly( CXMLSettings XMLSettings ) : base( CShared.METER_TYPE_CENTRON_POLY, XMLSettings )
		{
		}

		/// <summary>
		/// SENTINEL Load Research IDProgramming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		/// SENTINEL Service Config Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		/// SENTINEL SiteScan Diag 1 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		/// CENTRON (V and I) SiteScan Diag 4 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		/// CENTRON (V and I) SiteScan Diag 5 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/25/05 REM 7.20.06        Adding support for Diagnostic #5 for CENTRON (V and I)
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
		/// SENTINEL Primary Security Code Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		/// Q1000 Reset Power Outages Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
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

		/// <summary>
		/// CENTRON (V and I) Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///06/29/05 REM 7.20.XX        Adding support for Battery Carryover
		///08/22/05 REM 7.20.09 1625   Battery Carryover should default to Enable in System Manager
		///</pre></remarks>
		public virtual bool BatteryCarryover
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_BATTERY_CARRYOVER, true );
				
				//REM 08/22/05: Default for BatteryCarrover is true;
				//return m_XMLSettings.CurrentNodeBool;
				return m_XMLSettings.GetCurrentNodeBool( true );
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_BATTERY_CARRYOVER, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}
	}
}
