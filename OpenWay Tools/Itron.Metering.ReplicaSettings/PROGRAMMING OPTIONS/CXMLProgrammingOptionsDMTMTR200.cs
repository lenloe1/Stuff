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
	/// Programming Options for 200 Series meters (D/MT/MTR 200)
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
	///11/10/05 mrj 7.20.26 1624   Removed I/O Pulse Weights
	///</pre></remarks>
	public class CXMLProgrammingOptionsDMTMTR200 : Itron.Metering.ReplicaSettings.CXMLDeviceOptions
	{
		/// <summary>
		/// protected const string XML_NODE_EMPLOYEE_ID = "EmployeeID";
		/// </summary>
		protected const string XML_NODE_EMPLOYEE_ID = "EmployeeID";
		/// <summary>
		/// protected const string XML_NODE_NORMAL_MODE_KH = "NormalModeKh";
		/// </summary>
		protected const string XML_NODE_NORMAL_MODE_KH = "NormalModeKh";
		/// <summary>
		/// protected const string XML_NODE_SERIAL_NUMBER_LENGTH = "SerialNumberLength";
		/// </summary>
		protected const string XML_NODE_SERIAL_NUMBER_LENGTH = "SerialNumberLength";
		/// <summary>
		/// protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		/// </summary>
		protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		/// <summary>
		/// protected const string XML_NODE_CONFIGURE_OUTPUT_BOARD = "ConfigureOutputBoard";
		/// </summary>
		protected const string XML_NODE_CONFIGURE_OUTPUT_BOARD = "ConfigureOutputBoard";
		/// <summary>
		/// protected const string XML_NODE_LOAD_PROFILE_PULSE_WEIGHTS = "LoadProfilePulseWeights";
		/// </summary>
		protected const string XML_NODE_LOAD_PROFILE_PULSE_WEIGHTS = "LoadProfilePulseWeights";
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
		///</pre></remarks>
		public CXMLProgrammingOptionsDMTMTR200( CXMLSettings XMLSettings ) : base( CShared.METER_TYPE_DMTMTR200, XMLSettings )
		{
		}

		/// <summary>
		/// D/MT/MTR 200 Employee ID Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
		///</pre></remarks>
		public virtual int EmployeeID
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_EMPLOYEE_ID, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_EMPLOYEE_ID, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}


		/// <summary>
		/// D/MT/MTR 200 Normal Mode Kh Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
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
		/// D/MT/MTR 200 Meter Serial Number Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
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
		/// D/MT/MTR 200 Serial Number Length Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
		///</pre></remarks>
		public virtual int SerialNumberLength
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SERIAL_NUMBER_LENGTH, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SERIAL_NUMBER_LENGTH, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// D/MT/MTR 200 Unit ID Start Position Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
		///</pre></remarks>
		public virtual int UnitIDStartPosition
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_UNIT_ID_START_POSITION, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_UNIT_ID_START_POSITION, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// D/MT/MTR 200 Configure Output Board Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
		///</pre></remarks>
		public virtual bool ConfigureOutputBoard
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_CONFIGURE_OUTPUT_BOARD, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_CONFIGURE_OUTPUT_BOARD, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// D/MT/MTR 200 Load Profile Pulse Weights Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
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
	}
}
