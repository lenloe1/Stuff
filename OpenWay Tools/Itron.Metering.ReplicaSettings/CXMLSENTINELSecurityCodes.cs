using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// SENTINL Security Codes Setting Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class CXMLSENTINELSecurityCodes : Itron.Metering.ReplicaSettings.CXMLSecurityCodes
	{
        /// <summary>
        /// protected const string XML_NODE_LIMITED = "Limited";
        /// </summary>
		protected const string XML_NODE_LIMITED = "Limited";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLSENTINELSecurityCodes( CXMLSettings XMLSettings ) : base( "SENTINEL", XMLSettings )
		{
		}

		/// <summary>
		/// Device Limited Reconfigure Security Code
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string Limited
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LIMITED, true );

				return CShared.DecodeString(m_XMLSettings.CurrentNodeString);
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_LIMITED, true );

				m_XMLSettings.CurrentNodeString = CShared.EncodeString(value);
			}
		}	
	}
}
