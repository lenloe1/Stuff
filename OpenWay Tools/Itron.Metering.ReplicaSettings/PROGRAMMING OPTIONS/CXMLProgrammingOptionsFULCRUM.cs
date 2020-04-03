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
	/// Programming Options for FULCRUM meters
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///05/13/05 REM 7.20.XX        Adding support for FULCRUM
	///</pre></remarks>
	public class CXMLProgrammingOptionsFULCRUM : Itron.Metering.ReplicaSettings.CXMLDeviceOptions
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
		/// protected const string XML_NODE_SERIAL_NUMBER_LENGTH = "SerialNumberLength";
		/// </summary>
		protected const string XML_NODE_SERIAL_NUMBER_LENGTH = "SerialNumberLength";
		/// <summary>
		/// protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		/// </summary>
		protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		/// <summary>
		/// protected const string XML_NODE_PHONE_HOME_NUMBER = "PhoneHomeNumber";
		/// </summary>
		protected const string XML_NODE_PHONE_HOME_NUMBER = "PhoneHomeNumber";
		/// <summary>
		/// protected const string XML_NODE_IO_PULSE_WEIGHTS = "IOPulseWeights";
		/// </summary>
		protected const string XML_NODE_IO_PULSE_WEIGHTS = "IOPulseWeights";
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
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
		///</pre></remarks>
		public CXMLProgrammingOptionsFULCRUM( CXMLSettings XMLSettings ) : base( CShared.METER_TYPE_FULCRUM, XMLSettings )
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
		/// FULCRUM Serial Number Length Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
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
		/// FULCRUM Unit ID Start Position Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
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
		/// FULCRUM Phone Home Number Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
		///</pre></remarks>
		public virtual bool PhoneHomeNumber
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PHONE_HOME_NUMBER, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PHONE_HOME_NUMBER, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// FULCRUM I/O Pulse Weights Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/05/05 REM 7.20.XX        Adding support for I/O Pulse Weights
		///</pre></remarks>
		public virtual bool IOPulseWeights
		{
			get
			{
				return GetBoolProgrammingOption( XML_NODE_IO_PULSE_WEIGHTS );
			}
			set
			{
				SetBoolProgrammingOption( XML_NODE_IO_PULSE_WEIGHTS, value );
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
		public virtual bool LoadProfilePulseWeights
		{
			get
			{
				return GetBoolProgrammingOption( XML_NODE_LOAD_PROFILE_PULSE_WEIGHTS );
			}
			set
			{
				SetBoolProgrammingOption( XML_NODE_LOAD_PROFILE_PULSE_WEIGHTS, value );
			}
		}
	}
}
