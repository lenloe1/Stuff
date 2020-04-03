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
//                              Copyright © 2005
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// CENTRON Mono Programming Options XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
	///</pre></remarks>
	public class CXMLProgrammingOptionsCENTRONMono : Itron.Metering.ReplicaSettings.CXMLDeviceOptions
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
		public CXMLProgrammingOptionsCENTRONMono( CXMLSettings XMLSettings ) : base( CShared.METER_TYPE_CENTRON_MONO, XMLSettings )
		{
		}

		/// <summary>
		/// CENTRON Mono Load Research IDProgramming Option
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
		/// CENTRON Mono Employee ID Programming Option
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
		/// CENTRON Mono Load Profile Pulse Weights Programming Option
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
		/// CENTRON Mono Registe Full Scale Programming Option
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
		/// CENTRON Mono Primary Security Code Programming Option
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
		/// CENTRON Mono Limited Security Code Programming Option
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
		/// CENTRON Mono Secondary Security Code Programming Option
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
		/// CENTRON Mono Tertiary Security Code Programming Option
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
		/// CENTRON Mono Time On Battery Programming Option
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
		/// CENTRON Mono Meter Serial Number Programming Option
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
		/// CENTRON (C12.19) Programming Option
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
