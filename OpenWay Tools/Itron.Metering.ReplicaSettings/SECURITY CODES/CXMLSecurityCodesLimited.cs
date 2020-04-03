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
	/// Security Codes + Limited XML Setting Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///02/17/05 REM 7.20.XX N/A    Initial Release
	///</pre></remarks>
	public class CXMLSecurityCodesLimited : Itron.Metering.ReplicaSettings.CXMLSecurityCodesTertiary
	{
		/// <summary>
		/// protected const string XML_NODE_LIMITED = "Limited";
		/// </summary>
		protected const string XML_NODE_LIMITED = "Limited";

		/// <summary>
		/// Constructor		
		/// </summary>
		/// <param name="strMeterType"></param>
		/// <param name="XMLSettings"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX N/A    Initial Release
		///</pre></remarks>
		public CXMLSecurityCodesLimited( string strMeterType, CXMLSettings XMLSettings ) : base( strMeterType, XMLSettings )
		{
		}

		/// <summary>
		/// Device Limited Reconfigure Security Code
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX N/A    Initial Release
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
