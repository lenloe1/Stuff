using System;
using System.Windows.Forms;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Programming Options XML Settings Base Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
    public class CXMLDeviceOptions : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		//Constants
		/// <summary>
		/// XML_NODE_TRANSFORMER_RATIO = "TransformerRatio";
		/// </summary>
		protected const string XML_NODE_TRANSFORMER_RATIO = "TransformerRatio";
		/// <summary>
		/// XML_NODE_RESET_TIME_ON_BATTERY = "ResetTimeOnBattery";
		/// </summary>
		protected const string XML_NODE_RESET_TIME_ON_BATTERY = "ResetTimeOnBattery";
		/// <summary>
		/// XML_NODE_USER_DATA_1 = "UserData1";
		/// </summary>
		protected const string XML_NODE_USER_DATA_1 = "UserData1";
		/// <summary>
		/// XML_NODE_USER_DATA_2 = "UserData2";
		/// </summary>
		protected const string XML_NODE_USER_DATA_2 = "UserData2";
		/// <summary>
		/// XML_NODE_USER_DATA_3 = "UserData3";
		/// </summary>
		protected const string XML_NODE_USER_DATA_3 = "UserData3";
		/// <summary>
		/// XML_NODE_UNIT_ID = "UnitID";
		/// </summary>
		protected const string XML_NODE_UNIT_ID = "UnitID";
		/// <summary>
		/// XML_NODE_METER_SERIAL_NUMBER = "MeterSerialNumber";
		/// </summary>
		protected const string XML_NODE_METER_SERIAL_NUMBER = "MeterSerialNumber";

		/// <summary>
		/// CXMLDeviceOptions member variable for meter type
		/// </summary>
		protected readonly string XML_NODE_METER_TYPE;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strMeterType">Meter Type to use</param>
		/// <param name="XMLSettings">CXMLSettings instance to use</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLDeviceOptions( string strMeterType, CXMLSettings XMLSettings )
		{
			m_XMLSettings = XMLSettings;
			XML_NODE_METER_TYPE = strMeterType;
		}

		/// <summary>
		/// Transformer Ratio Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool TransformerRatio
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TRANSFORMER_RATIO, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TRANSFORMER_RATIO, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}
		
		/// <summary>
		/// Reset Time on Battery Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool ResetTimeOnBattery
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_RESET_TIME_ON_BATTERY, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_RESET_TIME_ON_BATTERY, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}
		
		/// <summary>
		/// User Data 1 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool UserData1
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_1, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_1, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// User Data 2 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool UserData2
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_2, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_2, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// User Data 3 Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool UserData3
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_3, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_3, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Unit ID Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual int UnitID
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_UNIT_ID, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_UNIT_ID, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Meter Serial Number Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual int MeterSerialNumber
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_METER_SERIAL_NUMBER, true );
				
				//REM 01/25/05: Converting type from Boolean to Integer
				//return m_XMLSettings.CurrentNodeBool;
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_METER_SERIAL_NUMBER, true );

				//REM 01/25/05: Converting type from Boolean to Integer
				//m_XMLSettings.CurrentNodeBool = value;
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Gets a Bool value for the Meter Type
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual bool GetBoolProgrammingOption( string strXMLNode )
		{
			bool blnReturn = false;
			
			m_XMLSettings.SetCurrentToAnchor();

			m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
			m_XMLSettings.SelectNode( strXMLNode, true );
					
			blnReturn = m_XMLSettings.CurrentNodeBool;
			
			return blnReturn;
		}

		/// <summary>
		/// Sets a Bool value for the Meter Type
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual void SetBoolProgrammingOption( string strXMLNode, bool blnValue )
		{			
			m_XMLSettings.SetCurrentToAnchor();

			m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
			m_XMLSettings.SelectNode( strXMLNode, true );
				
			m_XMLSettings.CurrentNodeBool = blnValue;			
		}
	}
}
