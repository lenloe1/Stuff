using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Q1000 Programming Options XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class CXMLQ1000ProgrammingOptions : Itron.Metering.ReplicaSettings.CXMLDeviceOptions
	{
		/// <summary>
		/// protected const string XML_NODE_RESET_POWER_OUTAGES = "ResetPowerOutages";
		/// </summary>
		protected const string XML_NODE_RESET_POWER_OUTAGES = "ResetPowerOutages";
		/// <summary>
		/// protected const string XML_NODE_RESET_DEMAND_RESETS = "ResetDemandResets";
		/// </summary>
		protected const string XML_NODE_RESET_DEMAND_RESETS = "ResetDemandResets";
		/// <summary>
		/// protected const string XML_NODE_RESET_FIELD_TESTS = "ResetFieldTests";
		/// </summary>
		protected const string XML_NODE_RESET_FIELD_TESTS = "ResetFieldTests";
		/// <summary>
		/// protected const string XML_NODE_PORT_CONFIGURATION = "PortConfiguration";
		/// </summary>
		protected const string XML_NODE_PORT_CONFIGURATION = "PortConfiguration";
		/// <summary>
		/// protected const string XML_NODE_SERVICE_CONFIGURATION = "ServiceConfiguration";
		/// </summary>
		protected const string XML_NODE_SERVICE_CONFIGURATION = "ServiceConfiguration";
		/// <summary>
		/// protected const string XML_NODE_EVENT_CONFIGURATION = "EventConfiguration";
		/// </summary>
		protected const string XML_NODE_EVENT_CONFIGURATION = "EventConfiguration";
		/// <summary>
		/// protected const string XML_NODE_SLC_CONFIGURATION = "SLCConfiguration";
		/// </summary>
		protected const string XML_NODE_SLC_CONFIGURATION = "SLCConfiguration";
		/// <summary>
		/// protected const string XML_NODE_ADV_PROTOCOL_CONFIGURATION = "AdvProtocolConfiguration";
		/// </summary>
		protected const string XML_NODE_ADV_PROTOCOL_CONFIGURATION = "AdvProtocolConfiguration";
		/// <summary>
		/// protected const string XML_NODE_USER_DATA_4 = "UserData4";
		/// </summary>
		protected const string XML_NODE_USER_DATA_4 = "UserData4";
		/// <summary>
		/// protected const string XML_NODE_USER_DATA_5 = "UserData5";
		/// </summary>
		protected const string XML_NODE_USER_DATA_5 = "UserData5";
		/// <summary>
		/// protected const string XML_NODE_SERIAL_NUMBER_LENGTH = "SerialNumberLength";
		/// </summary>
		protected const string XML_NODE_SERIAL_NUMBER_LENGTH = "SerialNumberLength";
		/// <summary>
		/// protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		/// </summary>
		protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
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
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings">CXMLSettings instance to use</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLQ1000ProgrammingOptions( CXMLSettings XMLSettings ) : base( "Q1000", XMLSettings )
		{
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
		/// Q1000 Reset Field Tests Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool ResetFieldTests
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_RESET_FIELD_TESTS, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_RESET_FIELD_TESTS, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Q1000 Port Configuration Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool PortConfiguration
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PORT_CONFIGURATION, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PORT_CONFIGURATION, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Q1000 Service Configuration Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool ServiceConfiguration
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SERVICE_CONFIGURATION, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SERVICE_CONFIGURATION, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Q1000 Event Configuration Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool EventConfiguration
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_EVENT_CONFIGURATION, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_EVENT_CONFIGURATION, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Q1000 SLC Configuration Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SLCConfiguration
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SLC_CONFIGURATION, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SLC_CONFIGURATION, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Q1000 Advanced Protocol Configuration Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool AdvProtocolConfiguration
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_ADV_PROTOCOL_CONFIGURATION, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_ADV_PROTOCOL_CONFIGURATION, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Q1000 User Data 4 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
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
		/// Q1000 User Data 5 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool UserData5
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_5, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_5, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Q1000 Serial Number Length Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
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
		/// Q1000 Unit ID Start Position Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
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
		/// Q1000 Normal Mode Kh Programming Option
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
		/// Q1000 Test Mode Kh Programming Option
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
		/// Q1000 I/O Pulse Weigths Programming Option
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
	}
}
