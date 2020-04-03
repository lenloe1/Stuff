using System;
using System.Windows.Forms;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Security Codes Tertiary XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///05/13/05 REM 7.20.XX		   Adding support for 16-bit devices
	///</pre></remarks>
	public class CXMLSecurityCodesTertiary : Itron.Metering.ReplicaSettings.CXMLSecurityCodes
	{
		//Constants
		/// <summary>
		/// protected const string XML_NODE_TERTIARY = "Tertiary";
		/// </summary>
		protected const string XML_NODE_TERTIARY = "Tertiary";

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
		///05/13/05 REM 7.20.XX		   Adding support for 16-bit devices
		///</pre></remarks>
		public CXMLSecurityCodesTertiary( string strMeterType, CXMLSettings XMLSettings ) : base( strMeterType, XMLSettings )
		{
		}

		/// <summary>
		/// Device Tertiary Security Code
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string Tertiary
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TERTIARY, true );

				return CShared.DecodeString(m_XMLSettings.CurrentNodeString);
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_TERTIARY, true );

				m_XMLSettings.CurrentNodeString = CShared.EncodeString(value);
			}
		}
	}
}
