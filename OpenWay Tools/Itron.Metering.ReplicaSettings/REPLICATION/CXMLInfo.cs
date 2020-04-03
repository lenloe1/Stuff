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
using Itron.Metering.ReplicaSettings;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Info Class. Used to store information dealing with a product, export file, etc.
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///05/18/05 REM 7.20.XX        Adding support for Version information in Replica files
	///</pre></remarks>
	public class CXMLInfo : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		/// <summary>
		/// CXMLInfo protected member variable
		/// </summary>
		protected CXMLInfoProductInfo m_InfoProductInfo;
		/// <summary>
		/// public const string DEFAULT_FILE_NAME = "Information.xml";
		/// </summary>
		public const string DEFAULT_FILE_NAME = "Information.xml";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strFilePath">File path to use for the .xml file. If "" is passed in the default will be used</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/18/05 REM 7.20.XX        Adding support for Version information in Replica files
		///</pre></remarks>
		public CXMLInfo( string strFilePath )
		{
			m_XMLSettings = new CXMLSettings( DEFAULT_SETTINGS_DIRECTORY + DEFAULT_FILE_NAME, "Info", "" );

			if( null != m_XMLSettings )
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}

			m_InfoProductInfo = new CXMLInfoProductInfo( m_XMLSettings );
		}
		
		/// <summary>
		/// Returns the Product Info object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLInfoProductInfo ProductInfo
		{
			get
			{
				return m_InfoProductInfo;
			}
		}
	}
}
