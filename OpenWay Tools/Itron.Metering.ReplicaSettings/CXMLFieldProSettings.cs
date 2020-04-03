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
//                              Copyright © 2005-2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Field-Pro Settings Settings Access Class
	/// </summary>	
	//  Revision History
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------------
	//  07/29/04 REM 7.00.15 N/A    Initial Release
	//  02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
	//  01/04/07 mrj 8.00.04		Changes for new Field-Pro
	//  
	public class CXMLFieldProSettings : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		/// <summary>
		/// protected const string XML_FILE_NAME = "FieldProSettings.xml";
		/// </summary>
		protected const string XML_FILE_NAME = "FieldProSettings.xml";
		/// <summary>
		/// protected const string XML_PARENT_NODE = "FieldProSettings";
		/// </summary>
		protected const string XML_PARENT_NODE = "FieldProSettings";				
		/// <summary>
		/// CXMLFieldProSettings protected member variable
		/// </summary>
		protected CXMLFieldProSettingsAllDevices m_FieldProAllDevices;
		/// <summary>
		/// CXMLFieldProSettings protected member variable
		/// </summary>
		protected CXMLFieldProSettingsLogonOptions m_FieldProLogonOptions;

		/// <summary>
		/// Default empty contructor, for use be derived classes.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  
		//  
		public CXMLFieldProSettings()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strFilePath">XML file path to use. If "" is passed in, then the default is used</param>		
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  07/29/04 REM 7.00.15 N/A    Initial Release
		//  02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
		//  01/04/07 mrj 8.00.04		Changes for new Field-Pro
		//  
		public CXMLFieldProSettings(string strFilePath)
		{			
			m_XMLSettings = new CXMLSettings(DEFAULT_SETTINGS_DIRECTORY + XML_FILE_NAME, "", XML_PARENT_NODE);

			if( null != m_XMLSettings )
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}
						
			m_FieldProAllDevices = new CXMLFieldProSettingsAllDevices( m_XMLSettings );
			m_FieldProLogonOptions = new CXMLFieldProSettingsLogonOptions( m_XMLSettings );			
		}
				
		/// <summary>
		/// Returns the All Devices Field-Pro Settings Object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLFieldProSettingsAllDevices AllDevices
		{
			get
			{
				return m_FieldProAllDevices;
			}
		}

		/// <summary>
		/// Returns the Logon Options Field-Pro Settings Object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLFieldProSettingsLogonOptions LogonOptions
		{
			get
			{
				return m_FieldProLogonOptions;
			}
		}
	}
}
