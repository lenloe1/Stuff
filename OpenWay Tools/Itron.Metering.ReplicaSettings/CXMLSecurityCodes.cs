using System;
using System.Windows.Forms;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Security Codes XML Settings Base Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///05/13/05 REM 7.20.XX		   Adding support for 16-bit drivers. Moved Tertiary to its own class
	///</pre></remarks>
	public class CXMLSecurityCodes : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		//Constants
		/// <summary>
		/// protected const string XML_NODE_PRIMARY = "Primary";
		/// </summary>
		protected const string XML_NODE_PRIMARY = "Primary";
		/// <summary>
		/// protected const string XML_NODE_SECONDARY = "Secondary";
		/// </summary>
		protected const string XML_NODE_SECONDARY = "Secondary";
		/// <summary>
		/// protected const string XML_NODE_PREVIOUS = "Previous";
		/// </summary>
		protected const string XML_NODE_PREVIOUS = "Previous";

		/// <summary>
		/// CXMLSecurityCodes protected member variable
		/// </summary>
		protected readonly string XML_NODE_METER_TYPE;

		//Methods
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
		public CXMLSecurityCodes( string strMeterType, CXMLSettings XMLSettings )
		{
			m_XMLSettings = XMLSettings;
			XML_NODE_METER_TYPE = strMeterType;
		}

		/// <summary>
		/// Device Primary Security Code
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string Primary
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PRIMARY, true );

				return CShared.DecodeString(m_XMLSettings.CurrentNodeString);
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PRIMARY, true );

				m_XMLSettings.CurrentNodeString = CShared.EncodeString(value);
			}
		}

		/// <summary>
		/// Device Secondary Security Code
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string Secondary
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SECONDARY, true );

				return CShared.DecodeString(m_XMLSettings.CurrentNodeString);
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_SECONDARY, true );

				m_XMLSettings.CurrentNodeString = CShared.EncodeString(value);
			}
		}

		/// <summary>
		/// Device Previous Security Code
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string Previous
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PREVIOUS, true );

				return CShared.DecodeString(m_XMLSettings.CurrentNodeString);
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_PREVIOUS, true );

				m_XMLSettings.CurrentNodeString = CShared.EncodeString(value);
			}
		}
	}
}
