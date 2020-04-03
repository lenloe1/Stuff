using System;
using System.Xml;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Default Values - XML Settings class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
    public class CXMLDefaultValues : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		//Constants
		/// <summary>
		/// XML_NODE_KH_VALUES = "KhValues";
		/// </summary>
		protected const string XML_NODE_KH_VALUES = "KhValues";
		/// <summary>
		/// XML_NODE_PDR_VALUES = "PDRValues";
		/// </summary>
		protected const string XML_NODE_PDR_VALUES = "PDRValues";
		/// <summary>
		/// XML_NODE_PULSE_WEIGHT_VALUES = "PulseWeightValues";
		/// </summary>
		protected const string XML_NODE_PULSE_WEIGHT_VALUES = "PulseWeightValues";
		/// <summary>
		/// XML_NODE_PHONE_HOME_NUMBERS = "PhoneHomeNumbers";
		/// </summary>
		protected const string XML_NODE_PHONE_HOME_NUMBERS = "PhoneHomeNumbers";
		/// <summary>
		/// XML_NODE_CT_RATIOS = "CTRatios";
		/// </summary>
		protected const string XML_NODE_CT_RATIOS = "CTRatios";
		/// <summary>
		/// XML_NODE_VT_RATIOS = "VTRatios";
		/// </summary>
		protected const string XML_NODE_VT_RATIOS = "VTRatios";
		/// <summary>
		/// XML_NODE_ENERGY = "Energy";
		/// </summary>
		protected const string XML_NODE_ENERGY = "Energy";
		/// <summary>
		/// XML_NODE_DEMAND = "Demand";
		/// </summary>
		protected const string XML_NODE_DEMAND = "Demand";
		/// <summary>
		/// XML_NODE_CUMM_DEMAND = "CummDemand";
		/// </summary>
		protected const string XML_NODE_CUMM_DEMAND = "CummDemand";
		/// <summary>
		/// XML_NODE_VOLTS = "Volts";
		/// </summary>
		protected const string XML_NODE_VOLTS = "Volts";
		/// <summary>
		/// XML_NODE_AMPS = "Amps";
		/// </summary>
		protected const string XML_NODE_AMPS = "Amps";
		/// <summary>
		/// XML_NODE_THD = "THD";
		/// </summary>
		protected const string XML_NODE_THD = "THD";
		/// <summary>
		/// XML_NODE_POWER_FACTOR = "PowerFactor";
		/// </summary>
		protected const string XML_NODE_POWER_FACTOR = "PowerFactor";

		//Display Data Display Types
		/// <summary>
		/// DISPLAY_DATA_TYPE_ENERGY = "Energy";
		/// </summary>
		protected const string DISPLAY_DATA_TYPE_ENERGY = "Energy";
		/// <summary>
		/// DISPLAY_DATA_TYPE_DEMAND = "Demand";
		/// </summary>
		protected const string DISPLAY_DATA_TYPE_DEMAND = "Demand";
		/// <summary>
		/// DISPLAY_DATA_TYPE_CUMM_DEMAND = "CummDemand";
		/// </summary>
		protected const string DISPLAY_DATA_TYPE_CUMM_DEMAND = "CummDemand";
		/// <summary>
		/// DISPLAY_DATA_TYPE_VOLTS = "Volts";
		/// </summary>
		protected const string DISPLAY_DATA_TYPE_VOLTS = "Volts";
		/// <summary>
		/// DISPLAY_DATA_TYPE_AMPS = "Amps";
		/// </summary>
		protected const string DISPLAY_DATA_TYPE_AMPS = "Amps";
		/// <summary>
		/// DISPLAY_DATA_TYPE_THD = "THD";
		/// </summary>
		protected const string DISPLAY_DATA_TYPE_THD = "THD";
		/// <summary>
		/// DISPLAY_DATA_TYPE_POWER_FACTOR = "PowerFactor";
		/// </summary>
		protected const string DISPLAY_DATA_TYPE_POWER_FACTOR = "PowerFactor";

		//Member Variables
		/// <summary>
		/// CXMLDefaultValues member variable
		/// </summary>
		protected CXMLDisplayData[] m_XMLDisplayData;
		/// <summary>
		/// CXMLDefaultValues member variable
		/// </summary>
		protected CXMLStandardKhValues m_XMLStandardKhValues;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strFilePath">File Path of .xml file. If "" is passed in, Default path is used</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLDefaultValues( string strFilePath )
		{
			m_XMLSettings = new CXMLSettings( DEFAULT_SETTINGS_DIRECTORY + "Default Values.xml", "", "DefaultValues" );

			if( null != m_XMLSettings )
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}

			m_XMLDisplayData = new CXMLDisplayData[ (int)DisplayTypes.NumberDisplayTypes ];
			m_XMLDisplayData[ (int)DisplayTypes.Energy ] = new CXMLDisplayData( DISPLAY_DATA_TYPE_ENERGY, m_XMLSettings );
			m_XMLDisplayData[ (int)DisplayTypes.Demand ] = new CXMLDisplayData( DISPLAY_DATA_TYPE_DEMAND, m_XMLSettings );
			m_XMLDisplayData[ (int)DisplayTypes.CummDemand ] = new CXMLDisplayData( DISPLAY_DATA_TYPE_CUMM_DEMAND, m_XMLSettings );
			m_XMLDisplayData[ (int)DisplayTypes.Volts ] = new CXMLDisplayData( DISPLAY_DATA_TYPE_VOLTS, m_XMLSettings );
			m_XMLDisplayData[ (int)DisplayTypes.Amps ] = new CXMLDisplayData( DISPLAY_DATA_TYPE_AMPS, m_XMLSettings );
			m_XMLDisplayData[ (int)DisplayTypes.THD ] = new CXMLDisplayData( DISPLAY_DATA_TYPE_THD, m_XMLSettings );
			m_XMLDisplayData[ (int)DisplayTypes.PowerFactor ] = new CXMLDisplayData( DISPLAY_DATA_TYPE_POWER_FACTOR, m_XMLSettings );
			m_XMLStandardKhValues = new CXMLStandardKhValues( m_XMLSettings );
		}

		/// <summary>
		/// All Kh Values
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float[] KhValues
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_KH_VALUES, true );
				
				return m_XMLSettings.CurrentNodeFloatArray;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_KH_VALUES, true );

				m_XMLSettings.CurrentNodeFloatArray = value;
			}
		}

		/// <summary>
		/// All P/DR Values
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float[] PDRValues
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_PDR_VALUES, true );
				
				return m_XMLSettings.CurrentNodeFloatArray;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_PDR_VALUES, true );

				m_XMLSettings.CurrentNodeFloatArray = value;
			}
		}
		
		/// <summary>
		/// All Pulse Weight (Ke) Values
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float[] PulseWeightValues
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_PULSE_WEIGHT_VALUES, true );
				
				return m_XMLSettings.CurrentNodeFloatArray;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_PULSE_WEIGHT_VALUES, true );

				m_XMLSettings.CurrentNodeFloatArray = value;
			}
		}
		
		/// <summary>
		/// All Phone Home Numbers
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string[] PhoneHomeNumbers
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_PHONE_HOME_NUMBERS, true );
				
				return m_XMLSettings.CurrentNodeStringArray;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_PHONE_HOME_NUMBERS, true );

				m_XMLSettings.CurrentNodeStringArray = value;
			}
		}
		
		/// <summary>
		/// All CT Ratios
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float[] CTRatios
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_CT_RATIOS, true );
				
				return m_XMLSettings.CurrentNodeFloatArray;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_CT_RATIOS, true );

				m_XMLSettings.CurrentNodeFloatArray = value;
			}
		}

		/// <summary>
		/// All VT Ratios Values
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float[] VTRatios
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_VT_RATIOS, true );
				
				return m_XMLSettings.CurrentNodeFloatArray;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_VT_RATIOS, true );

				m_XMLSettings.CurrentNodeFloatArray = value;
			}
		}
		
		/// <summary>
		/// All Energy Default Display Formats
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLDisplayData Energy
		{
			get
			{
				return m_XMLDisplayData[ (int)DisplayTypes.Energy ];
			}
		}
		
		/// <summary>
		/// All Demand Default Display Formats
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLDisplayData Demand
		{
			get
			{
				return m_XMLDisplayData[ (int)DisplayTypes.Demand ];
			}
		}

		/// <summary>
		/// All Cumm Demand Default Display Formats
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLDisplayData CummDemand
		{
			get
			{
				return m_XMLDisplayData[ (int)DisplayTypes.CummDemand ];
			}
		}
		
		/// <summary>
		/// All Volts Default Display Formats
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLDisplayData Volts
		{
			get
			{
				return m_XMLDisplayData[ (int)DisplayTypes.Volts ];
			}
		}

		/// <summary>
		/// All Amps Default Display Formats
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLDisplayData Amps
		{
			get
			{
				return m_XMLDisplayData[ (int)DisplayTypes.Amps ];
			}
		}

		/// <summary>
		/// All THD Default Display Formats
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLDisplayData THD
		{
			get
			{
				return m_XMLDisplayData[ (int)DisplayTypes.THD ];
			}
		}
		
		/// <summary>
		/// All Power Factor Default Display Formats
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLDisplayData PowerFactor
		{
			get
			{
				return m_XMLDisplayData[ (int)DisplayTypes.PowerFactor ];
			}
		}

		/// <summary>
		/// All Standard Kh Values
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLStandardKhValues StandardKhValues
		{
			get
			{
				return m_XMLStandardKhValues;
			}
		}
	}
}
