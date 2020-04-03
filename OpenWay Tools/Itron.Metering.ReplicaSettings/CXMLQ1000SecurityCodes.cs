using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Q1000 Security Codes XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///05/13/05 REM 7.20.XX		   Security Codes only contains upto Secondary now
	///</pre></remarks>
	//public class CXMLQ1000SecurityCodes : Itron.Metering.ReplicaSettings.CXMLSecurityCodes
	public class CXMLQ1000SecurityCodes : Itron.Metering.ReplicaSettings.CXMLSecurityCodesTertiary
	{
		/// <summary>
		/// protected const string XML_NODE_FIRMWARE = "Firmware";
		/// </summary>
		protected const string XML_NODE_FIRMWARE = "Firmware";

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
		public CXMLQ1000SecurityCodes( CXMLSettings XMLSettings ) : base( "Q1000", XMLSettings )
		{
		}

		/// <summary>
		/// Device Firmware Download Security Code
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string Firmware
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_FIRMWARE, true );

				return CShared.DecodeString(m_XMLSettings.CurrentNodeString);
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_FIRMWARE, true );

				m_XMLSettings.CurrentNodeString = CShared.EncodeString(value);
			}
		}
	}
}
