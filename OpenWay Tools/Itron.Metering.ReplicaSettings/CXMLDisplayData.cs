using System;
using System.Xml;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
    /// ReplicaSettings global enumeration
	/// </summary>
	public enum DisplayUnitChoices
	{
		/// <summary>
		/// Unit = 0,
		/// </summary>
		Unit = 0,
		/// <summary>
		/// Kilo = 1,
		/// </summary>
		Kilo = 1,
		/// <summary>
		/// Mega = 2
		/// </summary>
		Mega = 2
	}

	/// <summary>
    /// ReplicaSettings global enumeration
	/// </summary>
	public enum DisplayTypes
	{
		/// <summary>
		/// Energy = 0,
		/// </summary>
		Energy = 0,
		/// <summary>
		/// Demand = 1,
		/// </summary>
		Demand = 1,
		/// <summary>
		/// CummDemand = 2,
		/// </summary>
		CummDemand = 2,
		/// <summary>
		/// Volts = 3,
		/// </summary>
		Volts = 3,
		/// <summary>
		/// Amps = 4,
		/// </summary>
		Amps = 4,
		/// <summary>
		/// THD = 5,
		/// </summary>
		THD = 5,
		/// <summary>
		/// PowerFactor = 6,
		/// </summary>
		PowerFactor = 6,
		/// <summary>
		/// NumberDisplayTypes = 7 //Last Display Type + 1
		/// </summary>
		NumberDisplayTypes = 7 //Last Display Type + 1
	}

	/// <summary>
	/// Default Values - SENTINEL Default Display Data - XML Settings class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class CXMLDisplayData
	{
		//Constants
		/// <summary>
		/// protected const string XML_NODE_TOTAL_DIGITS = "TotalDigits";
		/// </summary>
		protected const string XML_NODE_TOTAL_DIGITS = "TotalDigits";
		/// <summary>
		/// protected const string XML_NODE_DECIMAL_DIGITS = "DecimalDigits";
		/// </summary>		
		protected const string XML_NODE_DECIMAL_DIGITS = "DecimalDigits";
		/// <summary>
		/// protected const string XML_NODE_DISPLAY_UNITS = "DisplayUnits";
		/// </summary>
		protected const string XML_NODE_DISPLAY_UNITS = "DisplayUnits";
		/// <summary>
		/// protected const string XML_NODE_LEADING_ZEROS = "LeadingZeros";
		/// </summary>
		protected const string XML_NODE_LEADING_ZEROS = "LeadingZeros";
		/// <summary>
		/// protected const string XML_NODE_DISPLAY_ANNUNCIATOR = "DisplayAnnunciator";
		/// </summary>
		protected const string XML_NODE_DISPLAY_ANNUNCIATOR = "DisplayAnnunciator";
		/// <summary>
		/// protected const string XML_NODE_FLOATING_DECIMAL = "FloatingDecimal";
		/// </summary>
		protected const string XML_NODE_FLOATING_DECIMAL = "FloatingDecimal";
		/// <summary>
		/// CXMLDisplayData protected member variable
		/// </summary>
		protected readonly string XML_NODE_DISPLAY_TYPE;
		/// <summary>
		/// CXMLDisplayData protected member variable
		/// </summary>
		protected CXMLSettings m_XMLSettings;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strDisplayType">Display Type to use</param>
		/// <param name="XMLSettings">CXMLSettings instance to use</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLDisplayData( string strDisplayType, CXMLSettings XMLSettings )
		{
			XML_NODE_DISPLAY_TYPE = strDisplayType;
			
			//We should use the existing DOM passed in
			m_XMLSettings = XMLSettings;
		}

		/// <summary>
		/// Gets/Sets the Total Digits for the Display Type selected
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual int TotalDigits
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TOTAL_DIGITS, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TOTAL_DIGITS, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Gets/Sets the Decimal Digits for the Display Type selected
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual int DecimalDigits
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_DECIMAL_DIGITS, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_DECIMAL_DIGITS, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Gets/Sets the Display Units for the Display Type selected
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual int DisplayUnits
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_UNITS, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_UNITS, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Gets/Sets the Leading Zeros for the Display Type selected
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool LeadingZeros
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LEADING_ZEROS, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LEADING_ZEROS, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Gets/Sets the Display Annunciator for the Display Type selected
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool DisplayAnnunciator
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_ANNUNCIATOR, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_ANNUNCIATOR, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}

		/// <summary>
		/// Gets/Sets the Floating Decimal for the Display Type selected
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool FloatingDecimal
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_FLOATING_DECIMAL, true );
				
				return m_XMLSettings.CurrentNodeBool;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_DISPLAY_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_FLOATING_DECIMAL, true );

				m_XMLSettings.CurrentNodeBool = value;
			}
		}
	}
}
