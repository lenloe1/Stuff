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
//                              Copyright © 2004 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Meter Change Out XML Settings class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///01/25/05 mrj 7.10.00        Created
	///</pre></remarks>
	public class CXMLMeterChangeOutSettings : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		//Enumerations
		/// <summary>
		/// CXMLMeterChangeOutSettings public enumeration
		/// </summary>
		public enum MeterChangeOutSettings
		{
			/// <summary>
			/// CreateDataFile = 0,
			/// </summary>
			CreateDataFile = 0,
			/// <summary>
			/// CopyRegisters = 1,
			/// </summary>
			CopyRegisters = 1,
			/// <summary>
			/// EditRegisters = 2,
			/// </summary>
			EditRegisters = 2,
			/// <summary>
			/// DefaultFileLocation = 3,			
			/// </summary>
			DefaultFileLocation = 3,
			/// <summary>
			/// Number Meter Change Out settings
			/// </summary>
			NumberMeterChangeOut			
		}

		//Constants
		/// <summary>
		/// protected const string XML_NODE_METER_CHANGE_OUT = "MeterChangeOut";
		/// </summary>
		protected const string XML_NODE_METER_CHANGE_OUT = "MeterChangeOut";
		/// <summary>
		/// protected const string XML_NODE_CREATE_DATA_FILE = "ChangeOutCreateDataFile";
		/// </summary>
		protected const string XML_NODE_CREATE_DATA_FILE = "ChangeOutCreateDataFile";
		/// <summary>
		/// protected const string XML_NODE_COPY_REGISTERS = "ChangeOutCopyRegisters";
		/// </summary>
		protected const string XML_NODE_COPY_REGISTERS = "ChangeOutCopyRegisters";
		/// <summary>
		/// protected const string XML_NODE_EDIT_REGISTERS = "ChangeOutEditRegisters";
		/// </summary>
		protected const string XML_NODE_EDIT_REGISTERS = "ChangeOutEditRegisters";
		/// <summary>
		/// protected const string XML_NODE_DEFAULT_FILE_LOCATION = "ChangeOutDefaultFileLocation";
		/// </summary>
		protected const string XML_NODE_DEFAULT_FILE_LOCATION = "ChangeOutDefaultFileLocation";
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strFilePath">File Name of XML file to use. If "" is passed in the default is used</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///01/25/05 mrj 7.10.00        Created
		///</pre></remarks>
		public CXMLMeterChangeOutSettings( string strFilePath )
		{
			m_XMLSettings = new CXMLSettings( DEFAULT_SETTINGS_DIRECTORY + "MeterChangeOut.xml", "", "MeterChangeOut" );

			if( null != m_XMLSettings )
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}
		}

		/// <summary>
		/// Choice of the data file type to create when doing a meter change out.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///01/25/05 mrj 7.10.00        Created
		///</pre></remarks>
		public virtual int CreateDataFile
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_CREATE_DATA_FILE, true );				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_CREATE_DATA_FILE, true );
				m_XMLSettings.CurrentNodeInt = value;
			}
		}	

		/// <summary>
		/// Choice whether or not to copy registers during a meter change out.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///01/25/05 mrj 7.10.00        Created
		///</pre></remarks>
		public virtual int CopyRegisters
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_COPY_REGISTERS, true );				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_COPY_REGISTERS, true );
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Choice whether or not to edit registers during a meter change out.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///01/25/05 mrj 7.10.00        Created
		///</pre></remarks>
		public virtual int EditRegisters
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_EDIT_REGISTERS, true );				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_EDIT_REGISTERS, true );
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Synchronization - Master Station's Data Directory
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///01/25/05 mrj 7.10.00        Created
		///</pre></remarks>
		public virtual string DefaultFileLocation
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_DEFAULT_FILE_LOCATION, true );								
				return m_XMLSettings.CurrentNodeString;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_DEFAULT_FILE_LOCATION, true );
				m_XMLSettings.CurrentNodeString = value;
			}
		}
	}
}