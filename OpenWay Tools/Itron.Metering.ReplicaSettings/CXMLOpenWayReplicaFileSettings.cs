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
//                           Copyright © 2008 - 2015
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
    /// <summary>
    /// OpenWay replica settings XML class
    /// </summary>
    public class CXMLOpenWayReplicaFileSettings : Itron.Metering.ReplicaSettings.CXMLReplicaFileSettings
    {
        #region Constants
		
		private const string REPLICA_REG_KEY = "OpenWay Replica";
		private const string XML_REPLICA_FILE_NAME = "OpenWayReplicaSettings.xml";
		private const string XML_SETTINGS_TAG = "OpenWayReplicaSettings";

		private const string XML_NODE_TOU_FILES = "TOUFiles";
		private const string XML_NODE_FW_FILES = "FWFiles";

        #endregion Constants

        #region Definitions

        //Enumerations
        /// <summary>
        /// CXMLReplicaFileSettings public enumeration
        /// </summary>
        public new enum ReplicaFileSettings
        {
			/// <summary>
			/// Settings = 0
			/// </summary>
			Settings,
            /// <summary>
			/// Programs = 1
            /// </summary>
            Programs,			
            /// <summary>
			/// TouFiles = 2
            /// </summary>
            TouFiles,
			/// <summary>
			/// FWFiles = 3
			/// </summary>
			FWFiles,
            /// <summary>
            /// CommModCfgFiles = 4
            /// </summary>
            CommModCfgFiles,
            /// <summary>
            /// CommModPPPFiles = 5
            /// </summary>
            CommModPPPFiles,
            /// <summary>
            /// NetTblReaderScripts = 6
            /// </summary>
            NetTblReaderScripts,
            /// <summary>
            /// TableDefinitionFiles = 7
            /// </summary>
            TableDefinitionFiles,
            /// <summary>
            /// Number Replica File Settings
            /// </summary>
            NumberReplicaFileSettings, //Last Replica File Settings + 1
            /// <summary>
            /// PASSWORD_PROTECTED = 1000 //This is not a Replica File Component so don't include it
            /// </summary>
            PASSWORD_PROTECTED = 1000 //This is not a Replica File Component so don't include it
        }

        #endregion Definitions

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strFilePath">
		/// XML file path to use. If "" is passed in, then the default is used
		/// </param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public CXMLOpenWayReplicaFileSettings(string strFilePath)
			: base()
		{
			m_XMLSettings = new CXMLSettings(CRegistryHelper.GetFilePath(REPLICA_REG_KEY) + XML_REPLICA_FILE_NAME, "", XML_SETTINGS_TAG);
			if (null != m_XMLSettings)
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}
		}

        #endregion Public Methods

        #region Public Properties
		       
		/// <summary>
		/// Choice of whether or not to include TOU files in Replica files
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public bool TouFiles
		{
			get
			{
				return GetBool(XML_NODE_TOU_FILES);
			}
			set
			{
				SetBool(XML_NODE_TOU_FILES, value);
			}
		}

		/// <summary>
		/// Choice of whether or not to include FW files in Replica files
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public bool FWFiles
		{
			get
			{
				return GetBool(XML_NODE_FW_FILES);
			}
			set
			{
				SetBool(XML_NODE_FW_FILES, value);
			}
		}

        #endregion Public Properties
    }
}
