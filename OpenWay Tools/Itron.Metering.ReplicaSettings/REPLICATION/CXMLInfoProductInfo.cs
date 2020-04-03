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
	/// Product Info XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///05/18/05 REM 7.20.XX N/A    Adding support for Version information in Replica files
	///</pre></remarks>
	public class CXMLInfoProductInfo : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		/// <summary>
		/// protected const string XML_NODE_PRODUCT_INFO = "ProductInfo";
		/// </summary>
		protected const string XML_NODE_PRODUCT_INFO = "ProductInfo";
		/// <summary>
		/// protected const string XML_NODE_VERSION = "Version";
		/// </summary>
		protected const string XML_NODE_VERSION = "Version";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings">CXMLSettings instance to use</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/18/05 REM 7.20.XX N/A    Adding support for Version information in Replica files
		///</pre></remarks>
		public CXMLInfoProductInfo( CXMLSettings XMLSettings )
		{
			m_XMLSettings = XMLSettings;
		}

		/// <summary>
		/// Product Version Information
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/18/05 REM 7.20.XX N/A    Adding support for Version information in Replica files
		///</pre></remarks>
		public virtual float Version
		{
			get
			{
				m_XMLSettings.SetCurrentToRoot();

				m_XMLSettings.SelectNode( XML_NODE_PRODUCT_INFO, true );
				m_XMLSettings.SelectNode( XML_NODE_VERSION, true );
				
				return m_XMLSettings.CurrentNodeFloat;
			}
			set
			{
				m_XMLSettings.SetCurrentToRoot();

				m_XMLSettings.SelectNode( XML_NODE_PRODUCT_INFO, true );
				m_XMLSettings.SelectNode( XML_NODE_VERSION, true );

				m_XMLSettings.CurrentNodeFloat = value;
			}
		}

	}
}
