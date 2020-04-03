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
//                              Copyright © 2008
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using System;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
    /// <summary>
	/// OpenWay system settings XML class.  It must use the Field-Pro settings
	/// as the base class because of how Field-Pro and Conrols use these 
	/// settings.
    /// </summary>	  
	public class CXMLMaxImageSystemSettings : Itron.Metering.ReplicaSettings.CXMLFieldProSettings
    {
        #region Constants

		private const string REPLICA_REG_KEY = "Replica";
        private const string XML_OPENWAY_FILE_NAME = "FieldProSettings.xml";
		// private const string XML_SETTINGS_TAG = "CentronIISystemSettings";
        private const string XML_SETTINGS_TAG = "FieldProSettings";
		
		private const string XML_NODE_CURRENT_PWD = "CurrentPWD";
		private const string XML_NODE_PREVIOUS_PWD = "PreviousPWD";		
		private const string XML_NODE_EDL_CREATE_DIR = "EDLCreationDirectory";
		private const string XML_NODE_EDL_VIEW_DIR = "EDLViewDirectory";
		private const string XML_NODE_HHF_CREATE_DIR = "HHFCreationDirectory";
		private const string XML_NODE_HHF_VIEW_DIR = "HHFViewDirectory";
        private const string XML_NODE_EDL_INCL_HISTORY = "EDLIncludeHistory";
        private const string XML_NODE_EDL_INCL_LP = "EDLIncludeLoadProfile";
        private const string XML_NODE_EDL_INCL_VM = "EDLIncludeVoltageMonitoring";
        private const string XML_NODE_EDL_INCL_NETWORK = "EDLIncludeNetworkTables";
        private const string XML_NODE_EDL_INCL_LANHANLOG = "EDLIncludeLANandHANLogs";
        private const string XML_NODE_ZIGBEE_SCAN_LIMIT = "ZigBeeScanLimit";
        private const string XML_NODE_ENHANCED_SECURITY_FILE_PATH = "EnhancedSecurityFilePath";
        private const string XML_NODE_USE_AUTHORIZATION = "UseSignedAuthentication";
        private const string XML_NODE_AUTHORIZATION_SERVER = "SignedAuthenticationServer";
        private const string XML_NODE_AUTH_SECURITY_METHOD = "SignedAuthenticationSecurityMethod";
        private const string XML_NODE_VERIFY_PROG_SIG_ON_IMPORT = "VerifyProgramSignatureOnImport";
        private const string XML_NODE_VERIFY_PROG_SIG_ON_CONFIG = "VerifyProgramSignatureOnConfig";
        private const string XML_NODE_EXTERNAL_SYSTEM_ID = "ExternalSystemID";
		
        #endregion

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
		public CXMLMaxImageSystemSettings(string strFilePath)	
			: base()
		{
			m_XMLSettings = new CXMLSettings(CRegistryHelper.GetFilePath(REPLICA_REG_KEY) + XML_OPENWAY_FILE_NAME, "", XML_SETTINGS_TAG);

			if (null != m_XMLSettings)
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}

			m_FieldProAllDevices = new CXMLFieldProSettingsAllDevices(m_XMLSettings);
			m_FieldProLogonOptions = new CXMLFieldProSettingsLogonOptions(m_XMLSettings);
		}

        #endregion Public Methods

		#region Public Properties

		/// <summary>
		/// Property to get/set the current OpenWay password.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public string CurrentPWD
		{
			get
			{
				return CShared.DecodeString(GetString(XML_NODE_CURRENT_PWD));
			}
			set
			{
				SetString(XML_NODE_CURRENT_PWD, CShared.EncodeString(value));
			}		
		}

		/// <summary>
		/// Property to get/set the previous OpenWay password.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public string PreviousPWD
		{
			get
			{
				return CShared.DecodeString(GetString(XML_NODE_PREVIOUS_PWD));
			}
			set
			{
				SetString(XML_NODE_PREVIOUS_PWD, CShared.EncodeString(value));
			}
		}

		/// <summary>
		/// Master Station's EDL creation directory.  This is the location
		/// that OpenWay Field-Pro will create EDL files in.  It is also the location
		/// that HH-Pro for OpenWay will synchronize EDL files to.
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  04/22/08 mrj 1.50.00		Created for OpenWay Tools 1.5
		//  
		public virtual string EDLCreationDirectory
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_EDL_CREATE_DIR, true);
				string strReturn = m_XMLSettings.CurrentNodeString;

				if (null == strReturn || "" == strReturn)
				{
#if (!WindowsCE)
					strReturn = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
#else
					strReturn = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
#endif
				}

				return strReturn;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_EDL_CREATE_DIR, true);
				m_XMLSettings.CurrentNodeString = value;
			}
		}

		/// <summary>
		/// Master Station's EDL view directory.  This is the location
		/// that Shop Manager will view EDL files from.
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  04/22/08 mrj 1.50.00		Created for OpenWay Tools 1.5
		//  
		public virtual string EDLViewDirectory
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_EDL_VIEW_DIR, true);
				string strReturn = m_XMLSettings.CurrentNodeString;

				if (null == strReturn || "" == strReturn)
				{
#if (!WindowsCE)
					strReturn = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
#else
					strReturn = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
#endif
				}

				return strReturn;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_EDL_VIEW_DIR, true);
				m_XMLSettings.CurrentNodeString = value;
			}
		}

		/// <summary>
		/// Master Station's HHF creation directory.  This is the location
		/// that OpenWay Field-Pro will create HHF files in.  It is also the location
		/// that HH-Pro for OpenWay will synchronize HHF files to.
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  04/22/08 mrj 1.50.00		Created for OpenWay Tools 1.5
		//  
		public virtual string HHFCreationDirectory
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_HHF_CREATE_DIR, true);
				string strReturn = m_XMLSettings.CurrentNodeString;

				if (null == strReturn || "" == strReturn)
				{
#if (!WindowsCE)
					strReturn = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
#else
					strReturn = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
#endif
				}

				return strReturn;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_HHF_CREATE_DIR, true);
				m_XMLSettings.CurrentNodeString = value;
			}
		}

		/// <summary>
		/// Master Station's HHF view directory.  This is the location
		/// that Shop Manager will view HHF files from.
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  04/22/08 mrj 1.50.00		Created for OpenWay Tools 1.5
		//  
		public virtual string HHFViewDirectory
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_HHF_VIEW_DIR, true);
				string strReturn = m_XMLSettings.CurrentNodeString;

				if (null == strReturn || "" == strReturn)
				{
#if (!WindowsCE)
					strReturn = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
#else
					strReturn = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
#endif
				}

				return strReturn;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_HHF_VIEW_DIR, true);
				m_XMLSettings.CurrentNodeString = value;
			}
		}

        /// <summary>
        /// Gets or sets the file path for the Enhanced Security file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/23/09 RCG 2.10.01 126335 Created

        public virtual string EnhancedSecurityFilePath
        {
            get
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_ENHANCED_SECURITY_FILE_PATH, true);
                string strReturn = m_XMLSettings.CurrentNodeString;

                if (null == strReturn)
                {
                    strReturn = "";
                }

                return strReturn;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_ENHANCED_SECURITY_FILE_PATH, true);
                m_XMLSettings.CurrentNodeString = value;
            }
        }

        /// <summary>
        /// Gets or sets the default value for including History in an EDL file
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/08 RCG 1.50.19		Created

        public virtual bool EDLIncludeHistory
        {
            get
            {
                return GetBool(XML_NODE_EDL_INCL_HISTORY);
            }
            set
            {
                SetBool(XML_NODE_EDL_INCL_HISTORY, value);
            }
        }

        /// <summary>
        /// Gets or sets the default value for including Load Profile data in an EDL file
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/08 RCG 1.50.19		Created

        public virtual bool EDLIncludeLoadProfile
        {
            get
            {
                return GetBool(XML_NODE_EDL_INCL_LP);
            }
            set
            {
                SetBool(XML_NODE_EDL_INCL_LP, value);
            }
        }

        /// <summary>
        /// Gets or sets the default value for including Network tables in an EDL file
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/08 RCG 1.50.19		Created

        public virtual bool EDLIncludeNetworkTables
        {
            get
            {
                return GetBool(XML_NODE_EDL_INCL_NETWORK);
            }
            set
            {
                SetBool(XML_NODE_EDL_INCL_NETWORK, value);
            }
        }

        /// <summary>
        /// Gets or sets the default value for including LAN and HAN logs in an EDL file
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/08 RCG 1.50.19		Created

        public virtual bool EDLIncludeLANandHANLogTables
        {
            get
            {
                return GetBool(XML_NODE_EDL_INCL_LANHANLOG);
            }
            set
            {
                SetBool(XML_NODE_EDL_INCL_LANHANLOG, value);
            }
        }

        /// <summary>
        /// Gets or sets the default value for including Voltage Monitoring data in an EDL file
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/08 RCG 1.50.19		Created

        public virtual bool EDLIncludeVoltageMonitoring
        {
            get
            {
                return GetBool(XML_NODE_EDL_INCL_VM);
            }
            set
            {
                SetBool(XML_NODE_EDL_INCL_VM, value);
            }
        }

        /// <summary>
        /// Gets or sets the default value for the ZigBee scan limit.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/17/08 RCG 1.51.05		Created

        public int ZigBeeScanLimit
        {
            get
            {
                int iValue = GetInt(XML_NODE_ZIGBEE_SCAN_LIMIT);

                // We need to make sure the value is greater than 0 to prevent a case
                // where there may be no scans.
                if (iValue <= 0)
                {
                    iValue = 4;
                }

                return iValue;
            }
            set
            {
                SetInt(XML_NODE_ZIGBEE_SCAN_LIMIT, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not Signed Authorization should be used.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/12/09 RCG 2.30.05	    Created

        public bool UseSignedAuthorization
        {
            get
            {
                return GetBool(XML_NODE_USE_AUTHORIZATION);
            }
            set
            {
                SetBool(XML_NODE_USE_AUTHORIZATION, value);
            }
        }

        /// <summary>
        /// Gets or sets the Authorization server URI
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/12/09 RCG 2.30.05	    Created

        public string AuthorizationServer
        {
            get
            {
                return GetString(XML_NODE_AUTHORIZATION_SERVER);
            }
            set
            {
                SetString(XML_NODE_AUTHORIZATION_SERVER, value);
            }
        }

        /// <summary>
        /// Gets or sets the security method for the Signed Authorization server.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public string AuthorizationSecurityMethod
        {
            get
            {
                string strSecurityMethod = GetString(XML_NODE_AUTH_SECURITY_METHOD);

                if (String.IsNullOrEmpty(strSecurityMethod))
                {
                    // The default should be Windows Login
                    strSecurityMethod = "SignedAuthorizationKerberosClient";
                }

                return strSecurityMethod;
            }
            set
            {
                SetString(XML_NODE_AUTH_SECURITY_METHOD, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not Programs Signatures should be verified on Import
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/30/10 RCG 2.40.30	    Created

        public bool VerifyProgramSignatureOnImport
        {
            get
            {
                return GetBool(XML_NODE_VERIFY_PROG_SIG_ON_IMPORT);
            }
            set
            {
                SetBool(XML_NODE_VERIFY_PROG_SIG_ON_IMPORT, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not Programs Signatures should be verified on Import
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/30/10 RCG 2.40.30	    Created

        public bool VerifyProgramSignatureOnConfig
        {
            get
            {
                return GetBool(XML_NODE_VERIFY_PROG_SIG_ON_CONFIG);
            }
            set
            {
                SetBool(XML_NODE_VERIFY_PROG_SIG_ON_CONFIG, value);
            }
        }

        /// <summary>
        /// Gets or sets the External System ID to be used for CRF generation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/13/10 RCG 2.42.03	    Created

        public string ExternalSystemID
        {
            get
            {
                string strSystemID = GetString(XML_NODE_EXTERNAL_SYSTEM_ID);

                if(String.IsNullOrEmpty(strSystemID))
                {
                    strSystemID = "Itron Openway";
                }

                return strSystemID;
            }
            set
            {
                SetString(XML_NODE_EXTERNAL_SYSTEM_ID, value);
            }
        }

		#endregion Public Properties
    }
}
