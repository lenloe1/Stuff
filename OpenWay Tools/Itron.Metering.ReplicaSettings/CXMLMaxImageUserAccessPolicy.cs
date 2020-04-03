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
//                           Copyright © 2008 - 2009
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Xml;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// The OpenWay user access policy XML class.  This method provides read/write 
    /// access to the OpenWay user access policy settings.
	/// </summary>
	public class CXMLMaxImageUserAccessPolicy : Itron.Metering.ReplicaSettings.CXMLEncryptedSettingsAccess
	{
		#region Constants

		private const string REPLICA_REG_KEY = "Replica";
		private const string XML_REPLICA_FILE_NAME = "PCProUserAccessPolicy.xml";
		private const string XML_CAPABILITIES_TAG = "Capabilities";
        private const string XML_USER_ACCESS_POLICY_TAG = "UserAccessPolicy";

        private const string XML_MANAGE_SYS_SETTINGS_TAG = "ManageSystemSettings";
        private const string XML_MANAGE_CONFIG_FILES_TAG = "ManageConfigFiles";
        private const string XML_MANAGE_DATA_FILES_TAG = "ManageDataFiles";
        private const string XML_RFLAN_OPERATIONS_TAG = "RFLANOperations";
        private const string XML_HAN_OPERATIONS_TAG = "HANOperations";
        private const string XML_METER_SWITCH_OPERATIONS_TAG = "MeterSwitchOperations";
        private const string XML_METER_INIT_OPERATIONS_TAG = "MeterInitialization";
        private const string XML_METER_RECONFIG_TAG = "MeterReconfiguration";
        private const string XML_FW_DOWNLOAD_TAG = "FirmwareDownload";
        private const string XML_RESET_DEMAND_TAG = "ResetDemand";
        private const string XML_RESET_BILLING_TAG = "ResetBilling";
        private const string XML_RESET_TAMPERS_TAG = "ResetTampers";
        private const string XML_RESET_ACTIVITY_STATUS = "ResetActivityStatus";
        private const string XML_ADJUST_CLOCK = "AdjustClock";
        private const string XML_CLEAR_METER_DATA = "ClearMeterData";
        private const string XML_ENTER_EXIST_TEST_MODE = "EnterExitTestMode";

        private const string MAXIMAGE_TOOLS_ADMINISTRATORS = "CentronII Tools Administrators";
        private const string MAXIMAGE_TOOLS_POWER_USERS = "CentronII Tools Power Users";
        private const string MAXIMAGE_TOOLS_USERS = "CentronII Tools Users";
        private const string MAXIMAGE_NORMAL_USERS_2 = "CentronII Normal Users 2";
        private const string MAXIMAGE_NORMAL_USERS_3 = "CentronII Normal Users 3";
		
		#endregion Constants

        #region Definitions

        /// <summary>
        /// The available groups of functions that can have their access controlled.
        /// </summary>
        public enum FunctionalCapability
        {
            /// <summary>
            /// Manage System Settings
            /// </summary>
            ManageSystemSettings,
            /// <summary>
            /// Manage Configuration Files
            /// </summary>
            ManageConfigFiles,
            /// <summary>
            /// Manage Data Files
            /// </summary>
            ManageDataFiles,
            /// <summary>
            /// RFLAN Operations
            /// </summary>
            RFLANOperations,
            /// <summary>
            /// HAN Operations
            /// </summary>
            HANOperations,
            /// <summary>
            /// Meter Switch Operations
            /// </summary>
            MeterSwitchOperations,
            /// <summary>
            /// Meter Initialization
            /// </summary>
            MeterInitialization,
            /// <summary>
            /// Reconfiguration
            /// </summary>
            Reconfiguration,
            /// <summary>
            /// Firmware Download
            /// </summary>
            FirmwareDownload,
            /// <summary>
            /// Reset Demand Registers
            /// </summary>
            ResetDemandRegisters,
            /// <summary>
            /// Reset Billing Registers
            /// </summary>
            ResetBillingRegisters,
            /// <summary>
            /// Reset Tampers
            /// </summary>
            ResetTampers,
            /// <summary>
            /// Reset Activity Status
            /// </summary>
            ResetActivityStatus,
            /// <summary>
            /// Adjust Clock
            /// </summary>
            AdjustClock,
            /// <summary>
            /// Clear All Meter Data
            /// </summary>
            ClearMeterData,
            /// <summary>
            /// Enter/Exit Test Mode
            /// </summary>
            EnterExitTestMode,
        }

        /// <summary>
        /// The available user grous which can access various function groups.
        /// </summary>
        [Flags]
        public enum UserGroup
        {
            /// <summary>
            /// No non-admin users are allowed access.
            /// </summary>
            NoUsers = 0,
            /// <summary>
            /// Arbitrary user group 1.
            /// </summary>
            OpenWayToolsPowerUsers = 1,
            /// <summary>
            /// Arbitrary user group 2.
            /// </summary>
            OpenWayToolsUsers = 2,
            /// <summary>
            /// Arbitrary user group 3.
            /// </summary>
            OpenWayNormalUsers2 = 4,
            /// <summary>
            /// Arbitrary user group 4.
            /// </summary>
            OpenWayNormalUsers3 = 8,
        }

        #endregion

        #region Public Methods

        /// <summary>
		/// Constructor.
		/// </summary>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  05/23/08 jrf 1.50.28		Created
		//  
		public CXMLMaxImageUserAccessPolicy()
			: base()
		{
			string strFilePath = CRegistryHelper.GetFilePath(REPLICA_REG_KEY) + XML_REPLICA_FILE_NAME;
            m_EncryptedXMLSettings = new CXMLEncryptedSettings(strFilePath, "", XML_CAPABILITIES_TAG);
            m_EncryptedXMLSettings.XMLFileName = strFilePath;			
		}

        /// <summary>
        /// This method checks if the currently logged on user is in an appropriate 
        /// user group to access the supplied function group.
        /// </summary>
        /// <param name="Capability">The function group who's access is in question.</param>
        /// <returns>A bool indicating whether access was granted.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/08 jrf 1.50.28		Created
        //  01/15/09 jrf 2.10.01 125939 Dividing the meter reset operations functional capability
        //                              into five new function groups.
        //  01/16/09 jrf 2.10.01 125939 Added additional new function group, clear meter data.
        // 
        public bool CheckUserAccess(FunctionalCapability Capability)
        {
            bool blnAccessGranted = true;
            List<string> lststrRoles; 

            //Get the user groups that can access each capability
            switch (Capability)
            {
                case FunctionalCapability.ManageSystemSettings:
                {
                    lststrRoles = GetUserGroups(XML_MANAGE_SYS_SETTINGS_TAG);
                    break;
                }
                case FunctionalCapability.ManageConfigFiles:
                {
                    lststrRoles = GetUserGroups(XML_MANAGE_CONFIG_FILES_TAG);
                    break;
                }
                case FunctionalCapability.ManageDataFiles:
                {
                    lststrRoles = GetUserGroups(XML_MANAGE_DATA_FILES_TAG);
                    break;
                }
                case FunctionalCapability.RFLANOperations:
                {
                    lststrRoles = GetUserGroups(XML_RFLAN_OPERATIONS_TAG);
                    break;
                }
                case FunctionalCapability.HANOperations:
                {
                    lststrRoles = GetUserGroups(XML_HAN_OPERATIONS_TAG);
                    break;
                }
                case FunctionalCapability.MeterSwitchOperations:
                {
                    lststrRoles = GetUserGroups(XML_METER_SWITCH_OPERATIONS_TAG);
                    break;
                }
                case FunctionalCapability.MeterInitialization:
                {
                    lststrRoles = GetUserGroups(XML_METER_INIT_OPERATIONS_TAG);
                    break;
                }
                case FunctionalCapability.Reconfiguration:
                {
                    lststrRoles = GetUserGroups(XML_METER_RECONFIG_TAG);
                    break;
                }
                case FunctionalCapability.FirmwareDownload:
                {
                    lststrRoles = GetUserGroups(XML_FW_DOWNLOAD_TAG);
                    break;
                }
                case FunctionalCapability.ResetDemandRegisters:
                {
                    lststrRoles = GetUserGroups(XML_RESET_DEMAND_TAG);
                    break;
                }
                case FunctionalCapability.ResetBillingRegisters:
                {
                    lststrRoles = GetUserGroups(XML_RESET_BILLING_TAG);
                    break;
                }
                case FunctionalCapability.ResetTampers:
                {
                    lststrRoles = GetUserGroups(XML_RESET_TAMPERS_TAG);
                    break;
                }
                case FunctionalCapability.ResetActivityStatus:
                {
                    lststrRoles = GetUserGroups(XML_RESET_ACTIVITY_STATUS);
                    break;
                }
                case FunctionalCapability.AdjustClock:
                {
                    lststrRoles = GetUserGroups(XML_ADJUST_CLOCK);
                    break;
                }
                case FunctionalCapability.ClearMeterData:
                {
                    lststrRoles = GetUserGroups(XML_CLEAR_METER_DATA);
                    break;
                }
                case FunctionalCapability.EnterExitTestMode:
                {
                    lststrRoles = GetUserGroups(XML_ENTER_EXIST_TEST_MODE);
                    break;
                }
                default:
                    {
                        throw new ArgumentException("Unexpected Capability: " + Capability.ToString());
                    }
            }

            //Verify the user is a member of one of the user groups
            blnAccessGranted = VerifyUser(UserAccessPolicy, lststrRoles);

            return blnAccessGranted;
        }

        /// <summary>
        /// This method checks if the given user group is allowed access to the  
        /// supplied function group.
        /// </summary>
        /// <param name="Capability">The function group who's access is in question.</param>
        /// <param name="Group">The specific user group that is having its access checked.</param>
        /// <returns>A bool indicating whether the user group has access.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/08 jrf 1.50.28		Created
        //  01/15/09 jrf 2.10.01 125939 Dividing the meter reset operations functional capability
        //                              into five new function groups.
        //  01/16/09 jrf 2.10.01 125939 Added additional new function group, clear meter data.
        //
        public bool CheckGroupAccess(FunctionalCapability Capability, UserGroup Group)
        {
            bool blnAccessGranted = false;
            string strXMLTag = "";

            //Get the user groups that can access each capability
            switch (Capability)
            {
                case FunctionalCapability.ManageSystemSettings:
                {
                    strXMLTag = XML_MANAGE_SYS_SETTINGS_TAG;
                    break;
                }
                case FunctionalCapability.ManageConfigFiles:
                {
                    strXMLTag = XML_MANAGE_CONFIG_FILES_TAG;
                    break;
                }
                case FunctionalCapability.ManageDataFiles:
                {
                    strXMLTag = XML_MANAGE_DATA_FILES_TAG;
                    break;
                }
                case FunctionalCapability.RFLANOperations:
                {
                    strXMLTag = XML_RFLAN_OPERATIONS_TAG;
                    break;
                }
                case FunctionalCapability.HANOperations:
                {
                    strXMLTag = XML_HAN_OPERATIONS_TAG;
                    break;
                }
                case FunctionalCapability.MeterSwitchOperations:
                {
                    strXMLTag = XML_METER_SWITCH_OPERATIONS_TAG;
                    break;
                }
                case FunctionalCapability.MeterInitialization:
                {
                    strXMLTag = XML_METER_INIT_OPERATIONS_TAG;
                    break;
                }
                case FunctionalCapability.Reconfiguration:
                {
                    strXMLTag = XML_METER_RECONFIG_TAG;
                    break;
                }
                case FunctionalCapability.FirmwareDownload:
                {
                    strXMLTag = XML_FW_DOWNLOAD_TAG;
                    break;
                }
                case FunctionalCapability.ResetDemandRegisters:
                {
                    strXMLTag = XML_RESET_DEMAND_TAG;
                    break;
                }
                case FunctionalCapability.ResetBillingRegisters:
                {
                    strXMLTag = XML_RESET_BILLING_TAG;
                    break;
                }
                case FunctionalCapability.ResetTampers:
                {
                    strXMLTag = XML_RESET_TAMPERS_TAG;
                    break;
                }
                case FunctionalCapability.ResetActivityStatus:
                {
                    strXMLTag = XML_RESET_ACTIVITY_STATUS;
                    break;
                }
                case FunctionalCapability.AdjustClock:
                {
                    strXMLTag = XML_ADJUST_CLOCK;
                    break;
                }
                case FunctionalCapability.ClearMeterData:
                {
                    strXMLTag = XML_CLEAR_METER_DATA;
                    break;
                }
                case FunctionalCapability.EnterExitTestMode:
                {
                    strXMLTag = XML_ENTER_EXIST_TEST_MODE;
                    break;
                }
                default:
                {
                    throw new ArgumentException("Unexpected Capability: " + Capability.ToString());
                }
            }

            m_EncryptedXMLSettings.SelectNodeFromAnchor(strXMLTag, true);

            if ((Group & (UserGroup)m_EncryptedXMLSettings.CurrentNodeInt) != UserGroup.NoUsers)
            {
                blnAccessGranted = true;
            }

            return blnAccessGranted;
        }

        /// <summary>
        /// This method assigns a user group(s) access to a particular function group.
        /// </summary>
        /// <param name="Capability">The capability to set.</param>
        /// <param name="Group">The user group(s) to assign access.</param>	
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/08 jrf 1.50.28		Created
        //  01/15/09 jrf 2.10.01 125939 Dividing the meter reset operations functional capability
        //                              into five new function groups.
        //  01/16/09 jrf 2.10.01 125939 Added additional new function group, clear meter data.
        //
        public void SetUserAccess(FunctionalCapability Capability, UserGroup Group)
        {
            switch (Capability)
            {
                case FunctionalCapability.ManageSystemSettings:
                {
                    SetUserGroups(XML_MANAGE_SYS_SETTINGS_TAG, Group);
                    break;
                }
                case FunctionalCapability.ManageConfigFiles:
                {
                    SetUserGroups(XML_MANAGE_CONFIG_FILES_TAG, Group);
                    break;
                }
                case FunctionalCapability.ManageDataFiles:
                {
                    SetUserGroups(XML_MANAGE_DATA_FILES_TAG, Group);
                    break;
                }
                case FunctionalCapability.RFLANOperations:
                {
                    SetUserGroups(XML_RFLAN_OPERATIONS_TAG, Group);
                    break;
                }
                case FunctionalCapability.HANOperations:
                {
                    SetUserGroups(XML_HAN_OPERATIONS_TAG, Group);
                    break;
                }
                case FunctionalCapability.MeterSwitchOperations:
                {
                    SetUserGroups(XML_METER_SWITCH_OPERATIONS_TAG, Group);
                    break;
                }
                case FunctionalCapability.MeterInitialization:
                {
                    SetUserGroups(XML_METER_INIT_OPERATIONS_TAG, Group);
                    break;
                }
                case FunctionalCapability.Reconfiguration:
                {
                    SetUserGroups(XML_METER_RECONFIG_TAG, Group);
                    break;
                }
                case FunctionalCapability.FirmwareDownload:
                {
                    SetUserGroups(XML_FW_DOWNLOAD_TAG, Group);
                    break;
                }
                case FunctionalCapability.ResetDemandRegisters:
                {
                    SetUserGroups(XML_RESET_DEMAND_TAG, Group);
                    break;
                }
                case FunctionalCapability.ResetBillingRegisters:
                {
                    SetUserGroups(XML_RESET_BILLING_TAG, Group);
                    break;
                }
                case FunctionalCapability.ResetTampers:
                {
                    SetUserGroups(XML_RESET_TAMPERS_TAG, Group);
                    break;
                }
                case FunctionalCapability.ResetActivityStatus:
                {
                    SetUserGroups(XML_RESET_ACTIVITY_STATUS, Group);
                    break;
                }
                case FunctionalCapability.AdjustClock:
                {
                    SetUserGroups(XML_ADJUST_CLOCK, Group);
                    break;
                }
                case FunctionalCapability.ClearMeterData:
                {
                    SetUserGroups(XML_CLEAR_METER_DATA, Group);
                    break;
                }
                case FunctionalCapability.EnterExitTestMode:
                {
                    SetUserGroups(XML_ENTER_EXIST_TEST_MODE, Group);
                    break;
                }
                default:
                {
                    throw new ArgumentException("Unexpected Capability: " + Capability.ToString());
                }
            }
        }

		/// <summary>
		/// This method is used to persist the current user's group SIDs so that 
		/// the system can validate user access when disconnected from the domain
		/// controller
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/25/08 mah 1.50.28		Created
        //  10/02/09 AF  2.30.06        Added 2 more user groups
		//
		public void UpdateAllGroupSIDs()
		{
			try
			{
				AppDomain myDomain = Thread.GetDomain();

				// myDomain.ApplicationTrust

				string strDomainName = Environment.UserDomainName;
				string strMachineName = Environment.MachineName;

				myDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
				WindowsPrincipal myPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;

				//If not logged onto a domain, the user domain name will return the name of the
				//computer.  We need to verify that the user is not logged on locally.
				if (strDomainName != strMachineName)
				{
					String strGroupName = strDomainName + @"\" + MAXIMAGE_TOOLS_ADMINISTRATORS;
					StoreGroupSID(myPrincipal, strGroupName, MAXIMAGE_TOOLS_ADMINISTRATORS);

					strGroupName = strDomainName + @"\" + MAXIMAGE_TOOLS_POWER_USERS;
					StoreGroupSID(myPrincipal, strGroupName, MAXIMAGE_TOOLS_POWER_USERS);

					strGroupName = strDomainName + @"\" + MAXIMAGE_TOOLS_USERS;
					StoreGroupSID(myPrincipal, strGroupName, MAXIMAGE_TOOLS_USERS);

                    strGroupName = strDomainName + @"\" + MAXIMAGE_NORMAL_USERS_2;
                    StoreGroupSID(myPrincipal, strGroupName, MAXIMAGE_NORMAL_USERS_2);

                    strGroupName = strDomainName + @"\" + MAXIMAGE_NORMAL_USERS_3;
                    StoreGroupSID(myPrincipal, strGroupName, MAXIMAGE_NORMAL_USERS_3);
				}
			}
			catch
			{
				//Do nothing.  If the groups have not been set up then no harm was done.
			}
		}
		
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets and sets whether the user access policy is in force.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/08 jrf 1.50.28		Created
        //  01/15/09 jrf 2.10.01 125939 Changed to make sure security policy is enabled if 
        //                              no node for the user access policy is found.
        //
        public bool UserAccessPolicy
        {
            get
            {
                if (false == m_EncryptedXMLSettings.SelectNodeFromRoot(XML_USER_ACCESS_POLICY_TAG, false))
                {
                    //Always enable the user access policy if the node does not exist.
                    m_EncryptedXMLSettings.SelectNodeFromRoot(XML_USER_ACCESS_POLICY_TAG, true);

                    m_EncryptedXMLSettings.CurrentNodeBool = true;
                }
                                
                return m_EncryptedXMLSettings.CurrentNodeBool;
            }
            set
            {
                m_EncryptedXMLSettings.SelectNodeFromRoot(XML_USER_ACCESS_POLICY_TAG, true);

                m_EncryptedXMLSettings.CurrentNodeBool = value;
            }
        }

        /// <summary>
        /// Gets whether the currenly logged on user is a member of a valid user group.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/08 jrf 1.50.28		Created
        //  10/05/09 AF  2.30.06        Added 2 more user groups
        //
        public bool VerifiedOpenWayUser
        {
            get
            {
                return VerifyUser(UserAccessPolicy, new List<string>() 
                    { MAXIMAGE_TOOLS_ADMINISTRATORS, MAXIMAGE_TOOLS_POWER_USERS, MAXIMAGE_TOOLS_USERS, 
                        MAXIMAGE_NORMAL_USERS_2, MAXIMAGE_NORMAL_USERS_3 });
            }
        }

        /// <summary>
        /// Gets whether the currenly logged on user is an OpenWay Administrator.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/29/08 jrf 1.50.28		Created
        //
        public bool VerifiedOpenWayAdministrator
        {
            get
            {
                //Always check for the administrator role regardless of whether the UserAccessPolicy 
                //is in force or not.
                return VerifyUser(true, new List<string>() { MAXIMAGE_TOOLS_ADMINISTRATORS });
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the valid user groups for a particular capability.
		/// </summary>
        /// <param name="strXMLTag">The tag for the capability.</param>
        /// <returns>A list of string names for each user group.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/08 jrf 1.50.28		Created
        //
		private List<string> GetUserGroups(string strXMLTag)
		{
			int iDefaultValue = 0;
            UserGroup Groups = UserGroup.NoUsers;
            List<string> lststrRoles;

            m_EncryptedXMLSettings.SelectNodeFromAnchor(strXMLTag, true);

            Groups = (UserGroup)m_EncryptedXMLSettings.GetCurrentNodeInt(iDefaultValue);

			lststrRoles = TranslateUserGroups(Groups);

            //Administrators are valid for every capability
            lststrRoles.Insert(0, MAXIMAGE_TOOLS_ADMINISTRATORS);

            return lststrRoles;
		}

        /// <summary>
        /// Translates the value of the enumeration variable into a list containing the 
        /// string name of each user group included.
        /// </summary>
        /// <param name="Groups">The user group(s).</param>	
        /// <returns>A list of string names for each user group.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/08 jrf 1.50.28		Created
        //  06/09/08 jrf 1.50.32 115870 Changed != to == in the if statements to correctly
        //                              add user groups.
        //  10/02/09 AF  2.30.05        Added 2 more user groups
        //
        private List<string> TranslateUserGroups(UserGroup Groups)
        {
            List<string> lststrRoles = new List<string>();

            if ((Groups & UserGroup.OpenWayToolsPowerUsers) == UserGroup.OpenWayToolsPowerUsers)
            {
                lststrRoles.Add(MAXIMAGE_TOOLS_POWER_USERS);
            }

            if ((Groups & UserGroup.OpenWayToolsUsers) == UserGroup.OpenWayToolsUsers)
            {
                lststrRoles.Add(MAXIMAGE_TOOLS_USERS);
            }

            if ((Groups & UserGroup.OpenWayNormalUsers2) == UserGroup.OpenWayNormalUsers2)
            {
                lststrRoles.Add(MAXIMAGE_NORMAL_USERS_2);
            }

            if ((Groups & UserGroup.OpenWayNormalUsers3) == UserGroup.OpenWayNormalUsers3)
            {
                lststrRoles.Add(MAXIMAGE_NORMAL_USERS_3);
            }

            return lststrRoles;
        }


		/// <summary>
		/// Sets the user groups that can access a particular capability.
		/// </summary>
		/// <param name="strXMLTag">The tag for the capability.</param>
        /// <param name="Groups">The user group(s).</param>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  05/23/08 jrf 1.50.28 		Created
		//  
		private void SetUserGroups(string strXMLTag, UserGroup Groups)
		{
            //Ensure only the appropriate user is setting system settings
            if (CheckUserAccess(FunctionalCapability.ManageSystemSettings))
            {
                m_EncryptedXMLSettings.SelectNodeFromAnchor(strXMLTag, true);

                m_EncryptedXMLSettings.CurrentNodeInt = (int)Groups;
            }
		}

        /// <summary>
        /// This method verifies the currently logged on user is a member of one of 
        /// the given user groups.
        /// </summary>
        /// <param name="blnEnforceAccessPolicy">Determines whether the access policy should
        /// be enforced.</param>
        /// <param name="lststrRoles">The list of user groups.</param>
        /// <returns></returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/28/08 jrf 1.50.28		Created
        //  06/02/08 jrf 1.50.29        Added check to make sure user was not logged on 
        //                              locally.
        //  06/03/08 jrf 1.50.29        Added parameter to determine whether to enforce
        //                              the access policy.
        //
        private bool VerifyUser(bool blnEnforceAccessPolicy, List<string> lststrRoles)
        {
            bool blnAccessGranted = false;

            //See if the security policy should be enforced.
            if (blnEnforceAccessPolicy)
            {
                try
                {
                    AppDomain myDomain = Thread.GetDomain();

					// myDomain.ApplicationTrust

                    string strDomainName = Environment.UserDomainName;
                    string strMachineName = Environment.MachineName;

                    myDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                    WindowsPrincipal myPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
					
                    //If not logged onto a domain, the user domain name will return the name of the
                    //computer.  We need to verify that the user is not logged on locally.
                    if (strDomainName != strMachineName)
                    {
                        foreach (string strRole in lststrRoles)
                        {
							String strGroupName = strDomainName + @"\" + strRole;
							
							if (!blnAccessGranted)
                            {
								try
								{
									blnAccessGranted = myPrincipal.IsInRole(strGroupName);

									if (blnAccessGranted)
									{
										StoreGroupSID(myPrincipal, strGroupName, strRole);

										m_EncryptedXMLSettings.SaveSettings(m_EncryptedXMLSettings.XMLFileName);
									}
								}
								
								catch 
								{
									// Note that 'IsInRole' can throw a SystemException when disconnected from
									// the domain controller.  In this case we have to go to our fallback plan,
									// Manually search the current user's identity for the security access group.
									// This is the equivalent of IsInRole but should work in a disconnected 
									// environment

									String strSID;

									if (RetrieveGroupSID(GetSIDName(strRole), out strSID))
									{
										SecurityIdentifier groupSID = new SecurityIdentifier(strSID);

										blnAccessGranted = myPrincipal.IsInRole(groupSID);
									}
								}
                            }
                        }
                    }
                }
                catch
                {
                    //Do nothing.
                }
            }
            else
            {
                //The user is always granted access if the security policy is not enforced.
                blnAccessGranted = true;
            }

            return blnAccessGranted;
        }

		/// <summary>
		/// This method stores the SID of the current user access group    
		/// </summary>
		/// <param name="myPrincipal" type="System.Security.Principal.WindowsPrincipal">
		/// The current users identity
		/// </param>
		/// <param name="strGroupName" type="string">
		/// The name of the accounr or group that the user currently belongs to
		/// </param>
		/// <param name="strRoleName" type="string">
		/// The predefined Openway role that we are going to persist
		/// </param>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/25/08 mah 1.50.28		Created
		//
		private void StoreGroupSID(WindowsPrincipal myPrincipal, String strGroupName, String strRoleName)
		{
			try
			{
				WindowsIdentity identity = myPrincipal.Identity as WindowsIdentity;

				foreach (IdentityReference identRef in identity.Groups)
				{
					IdentityReference account = identRef.Translate(typeof(NTAccount));

					if ( account.Value.Contains( strRoleName ))
					{
						IdentityReference groupID = identRef.Translate(typeof(SecurityIdentifier));

						String strSID = groupID.Value;

						m_EncryptedXMLSettings.SetCurrentToRoot();

						if (m_EncryptedXMLSettings.SelectNode( GetSIDName( strGroupName ), true))
						{
							m_EncryptedXMLSettings.CurrentNodeString = strSID;
						}

						break;
					}
				}
			}
			
			catch
			{
				// Do nothing...
			}
		}

		/// <summary>
		/// This method returns the SID (in string form) of the given access group    
		/// </summary>
		/// <param name="strSIDName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="strSID" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A bool value indicating whether or not the given access group was found 
		///     in the persistent store
		/// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/25/08 mah 1.50.28		Created
		//
		private Boolean RetrieveGroupSID(String strSIDName, out String strSID)
		{
			Boolean boolFoundSID;

			try
			{
				m_EncryptedXMLSettings.SetCurrentToRoot();

				if (m_EncryptedXMLSettings.SelectNode(strSIDName, false))
				{
					strSID = m_EncryptedXMLSettings.CurrentNodeString;

					boolFoundSID = (strSID.Length > 0);
				}
				else
				{
					boolFoundSID = false;
					strSID = "";
				}
			}

			catch
			{
				strSID = "";
				boolFoundSID = false;
			}

			return boolFoundSID;
		}

		/// <summary>
		/// This method assigns a name to each of the predefined Openway access groups. 
		/// This indirection allows flexibility in naming the access groups plus allows 
		/// tokens to be used in the name that would not be permissible in XML
		/// </summary>
		/// <param name="strGroupName" type="string">
		/// </param>
		/// <returns>
		///     A string value representing the XML tag used to store the SID for the 
		///     given group
		/// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/25/08 mah 1.50.28		Created
        //  10/05/09 AF  2.30.06        Added 2 more user groups
		//
		private static String GetSIDName(String strGroupName)
		{
			String strSIDName;

			if ( strGroupName.Contains ( MAXIMAGE_TOOLS_ADMINISTRATORS ))
			{
				strSIDName = "AdminGroup";
			}
			else if ( strGroupName.Contains( MAXIMAGE_TOOLS_POWER_USERS ))
			{
				strSIDName = "PowerGroup";
			}
			else if ( strGroupName.Contains( MAXIMAGE_TOOLS_USERS ))
			{
				strSIDName = "UserGroup";
			}
            else if (strGroupName.Contains(MAXIMAGE_NORMAL_USERS_2))
            {
                strSIDName = "UserGroup2";
            }
            else if (strGroupName.Contains(MAXIMAGE_NORMAL_USERS_3))
            {
                strSIDName = "UserGroup3";
            }
            else
            {
                strSIDName = "";
            }

			return strSIDName;
		}

		#endregion Private Methods

	}
}
