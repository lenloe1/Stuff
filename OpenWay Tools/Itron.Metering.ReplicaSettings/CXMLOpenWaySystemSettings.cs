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
//                              Copyright © 2013
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
    /// <summary>
    /// Used to select which Meter ID will be used when constructing the file name of a Data File
    /// </summary>
    public enum DataFileMeterID
    {
        /// <summary>
        /// Use MFG Serial Number
        /// </summary>
        [EnumDescription("Manufacturer Serial Number")]
        MFGSerialNumber = 0,
        /// <summary>
        /// Use Customer Serial Number
        /// </summary>
        [EnumDescription("Customer Serial Number")]
        CustomerSerialNumber = 1,
        /// <summary>
        /// Use ESN
        /// </summary>
        [EnumDescription("Electronic Serial Number")]
        ESN = 2,
        /// <summary>
        /// Use the Unit ID
        /// </summary>
        [EnumDescription("Unit ID")]
        UnitID = 3,
    }

    /// <summary>
    /// OpenWay system settings XML class.  It must use the Field-Pro settings
    /// as the base class because of how Field-Pro and Conrols use these 
    /// settings.
    /// </summary>	  
    public class CXMLOpenWaySystemSettings : Itron.Metering.ReplicaSettings.CXMLFieldProSettings
    {
        #region Constants

        private const string REPLICA_REG_KEY = "OpenWay Replica";
        private const string XML_OPENWAY_FILE_NAME = "OpenWaySystemSettings.xml";
        private const string XML_SETTINGS_TAG = "OpenWaySystemSettings";

        private const string XML_NODE_CURRENT_PWD = "CurrentPWD";
        private const string XML_NODE_PREVIOUS_PWD = "PreviousPWD";
        private const string XML_NODE_SECURITY_CODE_REGION_PWD = "SecurityCodeRegionPWD";
        private const string XML_NODE_SECURITY_CODE_REGION_NAME = "SecurityCodeRegionName";
        private const string XML_NODE_SECURITY_CODE_REGION_COUNT = "SecurityCodeRegionCount";
        private const string XML_NODE_SECURITY_CODE_REGIONS_ENABLED = "SecurityCodeRegionsEnabled";
        private const string XML_NODE_CURRENT_ICS_PWD = "CurrentICSPWD";
        private const string XML_NODE_PREVIOUS_ICS_PWD = "PreviousICSPWD";
        private const string XML_NODE_EDL_CREATE_DIR = "EDLCreationDirectory";
        private const string XML_NODE_EDL_VIEW_DIR = "EDLViewDirectory";
        private const string XML_NODE_CRF_CREATE_DIR = "CRFCreationDirectory";
        private const string XML_NODE_CRF_VIEW_DIR = "CRFViewDirectory";
        private const string XML_NODE_HHF_CREATE_DIR = "HHFCreationDirectory";
        private const string XML_NODE_HHF_VIEW_DIR = "HHFViewDirectory";
        private const string XML_NODE_EDL_INCL_HISTORY = "EDLIncludeHistory";
        private const string XML_NODE_EDL_INCL_LP = "EDLIncludeLoadProfile";
        private const string XML_NODE_EDL_INCL_VM = "EDLIncludeVoltageMonitoring";
        private const string XML_NODE_EDL_INCL_NETWORK = "EDLIncludeNetworkTables";
        private const string XML_NODE_EDL_INCL_LANHANLOG = "EDLIncludeLANandHANLogs";
        private const string XML_NODE_EDL_INCL_INSTR_PROFILE = "EDLIncludeInstrumentationProfile";
        private const string XML_NODE_ZIGBEE_SCAN_LIMIT = "ZigBeeScanLimit";
        private const string XML_NODE_ENHANCED_SECURITY_FILE_PATH = "EnhancedSecurityFilePath";
        private const string XML_NODE_ENABLE_C1218_SESSION_TIMEOUT = "EnableC1218SessionTimeout";
        private const string XML_NODE_C1218_SESSION_TIMEOUT = "C1281SessionTimeout";
        private const string XML_NODE_USE_AUTHORIZATION = "UseSignedAuthentication";
        private const string XML_NODE_AUTHORIZATION_SERVER = "SignedAuthenticationServer";
        private const string XML_NODE_AUTH_SECURITY_METHOD = "SignedAuthenticationSecurityMethod";
        private const string XML_NODE_VERIFY_PROG_SIG_ON_IMPORT = "VerifyProgramSignatureOnImport";
        private const string XML_NODE_VERIFY_PROG_SIG_ON_CONFIG = "VerifyProgramSignatureOnConfig";
        private const string XML_NODE_HIDE_IGNORED_ERRORS = "HideWarningsForIgnoredErrors";
        private const string XML_NODE_EXTERNAL_SYSTEM_ID = "ExternalSystemID";
        private const string XML_NODE_DATA_FILE_METER_ID = "DateFileMeterID";
        private const string XML_NODE_SIGNING_CERT_DISTINGUISHED_NAME = "DataFileSigningCertDistinguishedName";
        private const string XML_NODE_SIGN_DATA_FILES = "SignDataFiles";
        private const string XML_NODE_UTILIDY_IDS = "UtilityIDs";
        private const string XML_NODE_RFLAN_UTILITY_ID = "UtilityID";
        private const string XML_NODE_UTILITY_IDENTIFICATION_GROUP_NAME = "UtilityIdentificationGroupName";
        private const string XML_NODE_UTILITY_IDENTIFICATION_GROUP_DESCRIPTION = "UtilityIdentificationGroupDescription";
        private const string XML_NODE_CELLULAR_GATEWAY_ADDRESS = "CellularGatewayAddress";
        private const string XML_NODE_CELLULAR_GATEWAY_PORT = "CellularGatewayPort"; 
        private const string XML_NODE_CELLULAR_ERT_UTILITY_ID = "CellularERTID";
        private const string XML_NODE_UTILIDY_ID1 = "UtilityID1";
        private const string XML_NODE_UTILIDY_ID1D = "UtilityID1Description";
        private const string XML_NODE_UTILIDY_ID2 = "UtilityID2";
        private const string XML_NODE_UTILIDY_ID2D = "UtilityID2Description";
        private const string XML_NODE_UTILIDY_ID3 = "UtilityID3";
        private const string XML_NODE_UTILIDY_ID3D = "UtilityID3Description";
        private const string XML_NODE_UTILIDY_ID4 = "UtilityID4";
        private const string XML_NODE_UTILIDY_ID4D = "UtilityID4Description";
        private const string XML_NODE_UTILIDY_ID5 = "UtilityID5";
        private const string XML_NODE_UTILIDY_ID5D = "UtilityID5Description";
        private const string XML_NODE_UTILITY_ID_DESCRIPTION_SUFFIX = "Description";
        private const string XML_NODE_CONFIGURE_NEXT_TOU_SEASON = "ConfigureNextTOUSeason";
        private const string XML_NODE_SELECTED_TEST_QUANTITY = "SelectedTestQuantity";
        private const string XML_NODE_TEST_MODE_END = "TestModeEndTime";

        private const int MAX_UTILITY_IDENTIFICATION_GROUPS = 5;

        #endregion

        #region Definitions

        /// <summary>
        /// Utility Identifiation Settings Types
        /// </summary>
        private enum UtilityIdentificationSettingsType
        {

            /// <summary>
            /// The no utiity identification settings specified.
            /// </summary>
            [EnumDescription("Unspecified")]
            Unspecified,
            /// <summary>
            /// RFLAN Utility ID setting
            /// </summary>
            [EnumDescription("RF LAN Utility ID")]
            RFLANUtilityID,
            /// <summary>
            /// Cellular Gateway setting
            /// </summary>
            [EnumDescription("Cellular Gateway")]
            CellularGateway,
            /// <summary>
            /// Cellular ERT Utility ID setting
            /// </summary>
            [EnumDescription("Cellular ERT Utility ID")]
            CellularERTID,
        }

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
        public CXMLOpenWaySystemSettings(string strFilePath)
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

        /// <summary>
        /// Method to get the specified region's password.
        /// </summary>
        public string GetSecurityCodeRegionPWD(int regionNumber)
        {
            return CShared.DecodeString(GetString(XML_NODE_SECURITY_CODE_REGION_PWD + regionNumber.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Method to get the specified region's password.
        /// </summary>
        public string GetSecurityCodeRegionPWD(string regionName)
        {
            int? RegionIndex = null;
            string SecurityCode = null;

            for (int i = 0; i < SecurityCodePasswordRegionCount; i++)
            {
                if (regionName == GetSecurityCodeRegionName(i))
                {
                    RegionIndex = i;
                    break;
                }
            }

            if (null != RegionIndex) SecurityCode = GetSecurityCodeRegionPWD((int)RegionIndex);

            return SecurityCode;
        }

        /// <summary>
        /// Method to set the specified region's password.
        /// </summary>
        public void SetSecurityCodeRegionPWD(int regionNumber, string password)
        {
            SetString(XML_NODE_SECURITY_CODE_REGION_PWD + regionNumber.ToString(CultureInfo.InvariantCulture), CShared.EncodeString(password));
        }

        /// <summary>
        /// Method to get the specified region's name.
        /// </summary>
        public string GetSecurityCodeRegionName(int regionNumber)
        {
            return GetString(XML_NODE_SECURITY_CODE_REGION_NAME + regionNumber.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Method to set the specified region's name.
        /// </summary>
        public void SetSecurityCodeRegionName(int regionNumber, string regionName)
        {
            SetString(XML_NODE_SECURITY_CODE_REGION_NAME + regionNumber.ToString(CultureInfo.InvariantCulture), regionName);
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
        /// Property to get/set the current ICS password. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 AF  2.80.11 TR7641 Created
        //
        public string CurrentICSPWD
        {
            get
            {
                return CShared.DecodeString(GetString(XML_NODE_CURRENT_ICS_PWD));
            }
            set
            {
                SetString(XML_NODE_CURRENT_ICS_PWD, CShared.EncodeString(value));
            }
        }

        /// <summary>
        /// Property to get/set the previous ICS password. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 AF  2.80.11 TR7641 Created
        //
        public string PreviousICSPWD
        {
            get
            {
                return CShared.DecodeString(GetString(XML_NODE_PREVIOUS_ICS_PWD));
            }
            set
            {
                SetString(XML_NODE_PREVIOUS_ICS_PWD, CShared.EncodeString(value));
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
        /// Master Station's CRF creation directory.  This is the location
        /// that OpenWay Field-Pro will create CRF files in.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/28/15 PGH 4.50.125		Created
        //  
        public virtual string CRFCreationDirectory
        {
            get
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_CREATE_DIR, true);
                string strReturn = m_XMLSettings.CurrentNodeString;

                if (null == strReturn || "" == strReturn)
                {
                    strReturn = EDLCreationDirectory;
                }

                return strReturn;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_CREATE_DIR, true);
                m_XMLSettings.CurrentNodeString = value;
            }
        }

        /// <summary>
        /// Master Station's CRF view directory.  This is the location
        /// that Shop Manager will view CRF files from.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/28/15 PGH 4.50.125		Created
        //  
        public virtual string CRFViewDirectory
        {
            get
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_VIEW_DIR, true);
                string strReturn = m_XMLSettings.CurrentNodeString;

                if (null == strReturn || "" == strReturn)
                {
                    strReturn = EDLViewDirectory;
                }

                return strReturn;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_VIEW_DIR, true);
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
        /// Gets or sets the default value for including Voltage Monitoring data in an EDL file
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/14/11 RCG 2.53.20		Created

        public virtual bool EDLIncludeInstrumentationProfile
        {
            get
            {
                return GetBool(XML_NODE_EDL_INCL_INSTR_PROFILE);
            }
            set
            {
                SetBool(XML_NODE_EDL_INCL_INSTR_PROFILE, value);
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
        /// Gets or sets whether or not security code regions are enabled.
        /// </summary>
        public bool EnableSecurityCodeRegions
        {
            get
            {
                return GetBool(XML_NODE_SECURITY_CODE_REGIONS_ENABLED);
            }
            set
            {
                SetBool(XML_NODE_SECURITY_CODE_REGIONS_ENABLED, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of security code password regions.
        /// </summary>
        public int SecurityCodePasswordRegionCount
        {
            get
            {
                return GetInt(XML_NODE_SECURITY_CODE_REGION_COUNT);
            }
            set
            {
                SetInt(XML_NODE_SECURITY_CODE_REGION_COUNT, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not C1218 Session timeouts should be used.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/03/13 MAH 2.80.25	    Created

        public bool EnableC1218SessionTimeouts
        {
            get
            {
                return GetBool(XML_NODE_ENABLE_C1218_SESSION_TIMEOUT);
            }
            set
            {
                SetBool(XML_NODE_ENABLE_C1218_SESSION_TIMEOUT, value);
            }
        }


        /// <summary>
        /// Gets or sets the C1218 session timeout used by FieldPro.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/03/13 MAH 2.80.25	    Created

        public int C1218SessionTimeout
        {
            get
            {
                int iValue = GetInt(XML_NODE_C1218_SESSION_TIMEOUT);

                // We need to make sure the value is greater than 0 to prevent a case
                // where the timeout is invalid
                if (iValue <= 0)
                {
                    iValue = 30;
                }

                return iValue;
            }
            set
            {
                SetInt(XML_NODE_C1218_SESSION_TIMEOUT, value);
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
        /// Gets or sets whether ignored errors should display warnings
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  10/18/16 CFB 4.70.27 WR682857 Created

        public bool HideWarningsForIgnoredErrors
        {
            get
            {
                return GetBool(XML_NODE_HIDE_IGNORED_ERRORS);
            }
            set
            {
                SetBool(XML_NODE_HIDE_IGNORED_ERRORS, value);
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

                if (String.IsNullOrEmpty(strSystemID))
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

        /// <summary>
        /// Gets the Meter ID type that should be used to create Data Files
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/13/10 RCG 2.44.04 160413 Created

        public DataFileMeterID MeterIDUsedInFilename
        {
            get
            {
                // Get Int returns 0 if the node does not exist which will default us to MFG Serial
                int iValue = GetInt(XML_NODE_DATA_FILE_METER_ID);

                // Make sure that we don't retrieve a value that is not defined
                if (Enum.IsDefined(typeof(DataFileMeterID), iValue) == false)
                {
                    iValue = 0;
                }

                return (DataFileMeterID)iValue;
            }
            set
            {
                SetInt(XML_NODE_DATA_FILE_METER_ID, (int)value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to enable Data File signing
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- ---------------------------------------------
        //  06/04/12 RCG 2.54.00 TRQ6158 Created

        public bool SignDataFiles
        {
            get
            {
                return GetBool(XML_NODE_SIGN_DATA_FILES);
            }
            set
            {
                SetBool(XML_NODE_SIGN_DATA_FILES, value);
            }
        }

        /// <summary>
        /// Gets or sets the Distinguished Name for the certificate used to sign data files
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- ---------------------------------------------
        //  06/04/12 RCG 2.54.00 TRQ6158 Created

        public string SigningCertificateDistinguishedName
        {
            get
            {
                return GetString(XML_NODE_SIGNING_CERT_DISTINGUISHED_NAME);
            }
            set
            {
                SetString(XML_NODE_SIGNING_CERT_DISTINGUISHED_NAME, value);
            }
        }

        /// <summary>
        /// Gets or sets the utility identification settings in the replcia file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- ---------------------------------------------
        //  04/22/13 jrf 2.80.23 TQ8285  Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed.
        //
        public UtilityIdentificationSettingsGroupCollection UtilityIdentificationSettingsGroups
        {
            get
            {
                return BuildUtilityIdentificationSettingsGroups();
            }
            set
            {
                StoreUtilityIdentificationSettingsGroups(value);
            }
        }

        /// <summary>
        /// Gets the utility identification settings in the replcia file that have 
        /// cellular gateways configured.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- ---------------------------------------------
        //  04/29/13 jrf 2.80.25 TQ8284  Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed.
        //
        public UtilityIdentificationSettingsGroupCollection CellularGatewaySettingsGroups
        {
            get
            {
                return BuildUtilityIdentificationSettingsGroups(UtilityIdentificationSettingsType.CellularGateway);
            }
        }

        /// <summary>
        /// Gets the utility identification settings in the replcia file that have 
        /// RFLAN utility IDs configured.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- ---------------------------------------------
        //  05/07/13 jrf 2.80.27 TQ8284  Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed.
        //
        public UtilityIdentificationSettingsGroupCollection RFLANUtilitySettingsGroups
        {
            get
            {
                return BuildUtilityIdentificationSettingsGroups(UtilityIdentificationSettingsType.RFLANUtilityID);
            }
        }

        /// <summary>
        /// Gets the utility identification settings in the replcia file that have 
        /// ERT utility IDs configured.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- ---------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8285 Created.
        //
        public UtilityIdentificationSettingsGroupCollection ERTUtilitySettingsGroups
        {
            get
            {
                return BuildUtilityIdentificationSettingsGroups(UtilityIdentificationSettingsType.CellularERTID);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to enable modification of utility identification system settings.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- ---------------------------------------------
        //  09/14/12 MSC 2.70.14 TRQ6603 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed.
        //
        public bool AllowUtilityIdentificationSystemModification
        {
            get
            {
                return GetBool(XML_NODE_UTILIDY_IDS);
            }
            set
            {
                SetBool(XML_NODE_UTILIDY_IDS, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to configure the next TOU Season.
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/17 jrf 4.72.00 669810 Created
        public bool ConfigureNextTOUSeason
        {
            get
            {
                return GetBool(XML_NODE_CONFIGURE_NEXT_TOU_SEASON);
            }
            set
            {
                SetBool(XML_NODE_CONFIGURE_NEXT_TOU_SEASON, value);
            }
        }

        /// <summary>
        /// Gets or sets the most recently used test quantity
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/28/17 CFB 4.72.04 767058 Created
        public string SelectedTestQuantity
        {
            get
            {
                return GetString(XML_NODE_SELECTED_TEST_QUANTITY);
            }
            set
            {
                SetString(XML_NODE_SELECTED_TEST_QUANTITY, value);
            }
        }

        /// <summary>
        /// Gets or sets the most recently used test quantity
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/05/17 CFB 4.72.04 767058 Created
        public string TestModeEndTime
        {
            get
            {
                return GetString(XML_NODE_TEST_MODE_END);
            }
            set
            {
                SetString(XML_NODE_TEST_MODE_END, value);
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Builds a collection of utility identification settings groups from the replica file.
        /// </summary>
        /// <param name="SettingsType">The type of utility identification setting that should be defined in 
        /// settings groups retrieved.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/24/13 jrf 2.80.23 TQ8285 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renaming changes and added option to retrieve settings groups
        //                              with defined cellular ERT IDs.
        //
        private UtilityIdentificationSettingsGroupCollection BuildUtilityIdentificationSettingsGroups(
            UtilityIdentificationSettingsType SettingsType = UtilityIdentificationSettingsType.Unspecified)
        {
            UtilityIdentificationSettingsGroupCollection SettingsGroups = new UtilityIdentificationSettingsGroupCollection();
            UtilityIdentificatonSettingsGroupDefinition SettingsGroupDefinition = null;
            string strNodeName = "";
            bool blnKeepLooking = true; //Keep looking for more head end systems until there is a reason not to.

            for (int i = 1; i <= MAX_UTILITY_IDENTIFICATION_GROUPS && blnKeepLooking; i++)
            {
                try
                {
                    SettingsGroupDefinition = new UtilityIdentificatonSettingsGroupDefinition();

                    strNodeName = XML_NODE_UTILITY_IDENTIFICATION_GROUP_NAME + i.ToString(CultureInfo.InvariantCulture);
                    SettingsGroupDefinition.Name = GetString(strNodeName);

                    strNodeName = XML_NODE_UTILITY_IDENTIFICATION_GROUP_DESCRIPTION + i.ToString(CultureInfo.InvariantCulture);
                    if (NodeExists(strNodeName))
                    {
                        SettingsGroupDefinition.Description = GetString(strNodeName);
                    }
                    //Try to populate with the precursor's equivalent description. 
                    //This way users won't loose their previously configured descriptions.
                    else
                    {
                        strNodeName = XML_NODE_RFLAN_UTILITY_ID + i.ToString(CultureInfo.InvariantCulture) + XML_NODE_UTILITY_ID_DESCRIPTION_SUFFIX;
                        
                        if (NodeExists(strNodeName))
                        {
                            SettingsGroupDefinition.Description = GetString(strNodeName);
                        }
                    }

                    strNodeName = XML_NODE_RFLAN_UTILITY_ID + i.ToString(CultureInfo.InvariantCulture);
                    string strUtilityID = GetString(strNodeName);
                    byte byUtilityID;

                    if (byte.TryParse(strUtilityID, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byUtilityID))
                    {
                        SettingsGroupDefinition.RFLANUtilityID = byUtilityID;
                    }

                    strNodeName = XML_NODE_CELLULAR_GATEWAY_ADDRESS + i.ToString(CultureInfo.InvariantCulture);
                    IPAddress GatewayAddress;
                    if (IPAddress.TryParse(GetString(strNodeName), out GatewayAddress))
                    {
                        SettingsGroupDefinition.CellularGatewayAddress = GatewayAddress;
                    }

                    strNodeName = XML_NODE_CELLULAR_GATEWAY_PORT + i.ToString(CultureInfo.InvariantCulture);
                    SettingsGroupDefinition.CellularGatewayPort = (ushort)GetInt(strNodeName);


                    strNodeName = XML_NODE_CELLULAR_ERT_UTILITY_ID + i.ToString(CultureInfo.InvariantCulture);
                    strUtilityID = GetString(strNodeName);
                    ushort usUtilityID;

                    if (ushort.TryParse(strUtilityID, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out usUtilityID))
                    {
                        SettingsGroupDefinition.ERTUtilityID = usUtilityID;
                    }

                    if (true == SettingsGroupDefinition.Configured)
                    {
                        if (UtilityIdentificationSettingsType.Unspecified == SettingsType)
                        {
                            SettingsGroups.Add(SettingsGroupDefinition);
                        }
                        else if (UtilityIdentificationSettingsType.CellularGateway == SettingsType && SettingsGroupDefinition.CellularGatewayConfigured)
                        {
                            SettingsGroups.Add(SettingsGroupDefinition);
                        }
                        else if (UtilityIdentificationSettingsType.RFLANUtilityID == SettingsType && SettingsGroupDefinition.RFLANUtilityIDConfigured)
                        {
                            SettingsGroups.Add(SettingsGroupDefinition);
                        }
                        else if (UtilityIdentificationSettingsType.CellularERTID == SettingsType && SettingsGroupDefinition.ERTUtilityIDConfigured)
                        {
                            SettingsGroups.Add(SettingsGroupDefinition);
                        }
                    }
                    else
                    {
                        //When we reach a system that has no valid connection settings then we're done.
                        //Settings will only be stored if they have valid connection settings and will be 
                        //stored contiguously.
                        blnKeepLooking = false;
                    }
                }
                catch
                {
                    blnKeepLooking = false;
                }
            }

            return SettingsGroups;
        }

        /// <summary>
        /// Stores a collection of utility identification settings groups in the replica file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version ID Number  Description
        //  -------- --- ------- -- ------ ---------------------------------------------
        //  04/24/13 jrf 2.80.23 TQ 8285   Created
        //  06/11/13 jrf 2.80.37 TQ 8285   Renaming changes and added option to store cellular ERT ID setting.
        //  08/08/13 jrf 2.85.14 WR 418010 Adding handling if null was passed in. 
        //
        private void StoreUtilityIdentificationSettingsGroups(UtilityIdentificationSettingsGroupCollection SettingsGroups)
        {
            string strNodeName = "";
            int iSettingsGroupIndex = 1;

            if (null != SettingsGroups)
            {
                foreach (UtilityIdentificatonSettingsGroupDefinition Definition in SettingsGroups)
                {
                    try
                    {
                        //Only storing if there is at least one valid utility identification setting. 
                        //Otherwise there is really no point.
                        if (true == Definition.Configured)
                        {
                            strNodeName = XML_NODE_UTILITY_IDENTIFICATION_GROUP_NAME + iSettingsGroupIndex.ToString(CultureInfo.InvariantCulture);
                            SetString(strNodeName, Definition.Name);

                            strNodeName = XML_NODE_UTILITY_IDENTIFICATION_GROUP_DESCRIPTION + iSettingsGroupIndex.ToString(CultureInfo.InvariantCulture);
                            SetString(strNodeName, Definition.Description);

                            strNodeName = XML_NODE_RFLAN_UTILITY_ID + iSettingsGroupIndex.ToString(CultureInfo.InvariantCulture);
                            SetString(strNodeName, Definition.RFLANUtilityID.ToString("X2", CultureInfo.InvariantCulture));

                            strNodeName = XML_NODE_CELLULAR_GATEWAY_ADDRESS + iSettingsGroupIndex.ToString(CultureInfo.InvariantCulture);
                            if (null != Definition.CellularGatewayAddress)
                            {
                                SetString(strNodeName, Definition.CellularGatewayAddress.ToString());
                            }
                            else
                            {
                                SetString(strNodeName, string.Empty);
                            }

                            strNodeName = XML_NODE_CELLULAR_GATEWAY_PORT + iSettingsGroupIndex.ToString(CultureInfo.InvariantCulture);
                            SetInt(strNodeName, Definition.CellularGatewayPort);

                            strNodeName = XML_NODE_CELLULAR_ERT_UTILITY_ID + iSettingsGroupIndex.ToString(CultureInfo.InvariantCulture);
                            SetString(strNodeName, Definition.ERTUtilityID.ToString("X2", CultureInfo.InvariantCulture));

                            //We have successfull stored settings for a head-end system, move index on to next.
                            iSettingsGroupIndex++;
                        }
                    }
                    catch
                    {
                        //There was a problem. Keep trying...
                    }
                }
            }

            //Clear out the remaining utility identification settings group storage slots.
            for (int i = iSettingsGroupIndex; i <= MAX_UTILITY_IDENTIFICATION_GROUPS; i++)
            {
                strNodeName = XML_NODE_UTILITY_IDENTIFICATION_GROUP_NAME + i.ToString(CultureInfo.InvariantCulture);
                SetString(strNodeName, string.Empty);

                strNodeName = XML_NODE_UTILITY_IDENTIFICATION_GROUP_DESCRIPTION + i.ToString(CultureInfo.InvariantCulture);
                SetString(strNodeName, string.Empty);

                strNodeName = XML_NODE_RFLAN_UTILITY_ID + i.ToString(CultureInfo.InvariantCulture);
                SetString(strNodeName, "00");

                strNodeName = XML_NODE_CELLULAR_GATEWAY_ADDRESS + i.ToString(CultureInfo.InvariantCulture);
                SetString(strNodeName, string.Empty);

                strNodeName = XML_NODE_CELLULAR_GATEWAY_PORT + i.ToString(CultureInfo.InvariantCulture);
                SetInt(strNodeName, 0);

                strNodeName = XML_NODE_CELLULAR_ERT_UTILITY_ID + iSettingsGroupIndex.ToString(CultureInfo.InvariantCulture);
                SetString(strNodeName, "00");
            }
        }

        #endregion
    }
}
