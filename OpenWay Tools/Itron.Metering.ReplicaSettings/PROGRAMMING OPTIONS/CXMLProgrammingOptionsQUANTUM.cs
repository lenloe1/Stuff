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
	/// Programming Options for QUANTUM (STQ) meters
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///05/13/05 REM 7.20.XX        Adding support for FULCRUM
	///11/11/05 REM 7.20.25 2033   User Data #5 should not appear in System Manager for QUANTUM
	///</pre></remarks>
	public class CXMLProgrammingOptionsQUANTUM : Itron.Metering.ReplicaSettings.CXMLDeviceOptions
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
		/// protected const string XML_NODE_TEST_MODE_KH = "TestModeKh";
		/// </summary>
		protected const string XML_NODE_TEST_MODE_KH = "TestModeKh";
		/// <summary>
		/// protected const string XML_NODE_USER_DATA_4 = "UserData4";
		/// </summary>
		protected const string XML_NODE_USER_DATA_4 = "UserData4";
		/// <summary>
		/// protected const string XML_NODE_SERIAL_NUMBER_LENGTH = "SerialNumberLength";
		/// </summary>
		protected const string XML_NODE_SERIAL_NUMBER_LENGTH = "SerialNumberLength";
		/// <summary>
		/// protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		/// </summary>
		protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		/// <summary>
		/// protected const string XML_NODE_NUMBER_FIELD_TESTS = "ResetFieldTest";
		/// </summary>
		//REM 12/14/05: Need to change the constant to fix unit testing issue
		protected const string XML_NODE_NUMBER_FIELD_TESTS = "ResetFieldTests";
		/// <summary>
		/// protected const string XML_NODE_RESET_POWER_OUTAGES = "ResetPowerOutages";
		/// </summary>
		protected const string XML_NODE_RESET_POWER_OUTAGES = "ResetPowerOutages";
		/// <summary>
		/// protected const string XML_NODE_RESET_DEMAND_RESETS = "ResetDemandResets";
		/// </summary>
		protected const string XML_NODE_RESET_DEMAND_RESETS = "ResetDemandResets";
		/// <summary>
		/// protected const string XML_NODE_TRANSFORMER_LOSS_CONSTANTS = "TransformerLossConstants";
		/// </summary>
		protected const string XML_NODE_TRANSFORMER_LOSS_CONSTANTS = "TransformerLossConstants";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
		///</pre></remarks>
		public CXMLProgrammingOptionsQUANTUM( CXMLSettings XMLSettings ) : base( CShared.METER_TYPE_QUANTUM, XMLSettings )
		{
		}

		/// <summary>
		/// FULCRUM Employee ID Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
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
		/// FULCRUM Normal Mode Kh Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
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
		/// FULCRUM Test Mode Kh Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
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
		/// User Data 4 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
		///</pre></remarks>
		public virtual bool UserData4
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_4, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_4, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// QUANTUM Serial Number Length Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for QUANTUM
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
		/// QUANTUM Unit ID Start Position Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for QUANTUM
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
		/// QUANTUM Number Field Tests Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for QUANTUM
		///</pre></remarks>
		public virtual bool NumberFieldTests
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_NUMBER_FIELD_TESTS, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_NUMBER_FIELD_TESTS, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// QUANTUM Reset Power Outages Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for QUANTUM
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
		/// QUANTUM Reset Demand Resets Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for QUANTUM
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
		/// QUANTUM Transformer Loss Constants Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///10/13/05 REM 7.20.XX        Adding support for QUANTUM
		///</pre></remarks>
		public virtual bool TransformerLossConstants
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TRANSFORMER_LOSS_CONSTANTS, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TRANSFORMER_LOSS_CONSTANTS, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}
	}
}
