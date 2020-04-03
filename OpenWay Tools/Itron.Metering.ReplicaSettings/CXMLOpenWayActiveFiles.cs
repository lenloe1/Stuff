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
//                              Copyright © 2008 - 2017
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;
using System.Collections.Generic;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// OpenWay replica settings XML class
	/// </summary>
	public class XMLOpenWayActiveFiles : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		#region Constants

		private const string REPLICA_REG_KEY = "OpenWay Replica";
		private const string XML_REPLICA_FILE_NAME = "OpenWayActiveFiles.xml";
		private const string XML_ACTIVE_TAG = "ActiveFiles";

		private const string XML_PROGRAMS_TAG = "ProgramsFiles";
		private const string XML_TOU_TAG = "TOUFiles";
		private const string XML_FW_TAG = "RegisterFW";
		private const string XML_LAN_FW_TAG = "LANFW";
		private const string XML_ZIGBEE_FW_TAG = "ZigBeeFW";
        private const string XML_DISPLAY_FW_TAG = "DisplayFW";
        private const string XML_GAS_MODULE_FW_TAG = "GasModuleFW";
        private const string XML_GAS_RANGE_EXTENDER_FW_TAG = "GasRangeExtenderFW";
        private const string XML_PLAN_FW_TAG = "PLANFW";
        private const string XML_GTWY_FW_TAG = "GTWYFW";
        private const string XML_CISCO_COMM_FW_TAG = "CiscoCommFW";
        private const string XML_CISCO_CONFIG_TAG = "CiscoConfig";
        private const string XML_CC_FW_TAG = "ChoiceConnectFW";
        private const string XML_ICS_FW_TAG = "ICSFW";
        private const string XML_VERIZON_MODEM_FW_TAG = "ICSModemVerizonFW";
        private const string XML_ATT_ROGERS_BELL_MODEM_FW_TAG = "ICSModemATTRogersBellFW";

        private const string XML_HW_1_0 = "HW_1_0";
        private const string XML_HW_1_5 = "HW_1_5";
        private const string XML_HW_2_0 = "HW_2_0";
        private const string XML_HW_3_0 = "HW_3_0";
        private const string XML_HW_3_6 = "HW_3_6";
        private const string XML_GTWY_1_0 = "GTWY1_0";
        private const string XML_GTWY_3_1 = "GTWY3_1";
        private const string XML_SL7000 = "SL7000";
        private readonly string[] XML_HW_LIST = { XML_HW_1_0, XML_HW_1_5, XML_HW_2_0, XML_HW_3_0, XML_HW_3_6, XML_GTWY_1_0, XML_GTWY_3_1, XML_SL7000 };
		
		#endregion Constants
		
		#region Public Methods

		/// <summary>
		/// Constructor.
		/// </summary>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public XMLOpenWayActiveFiles()
			: base()
		{
			string strFilePath = CRegistryHelper.GetFilePath(REPLICA_REG_KEY) + XML_REPLICA_FILE_NAME;
            m_XMLSettings = new CXMLSettings(strFilePath, "", XML_ACTIVE_TAG);
			m_XMLSettings.XMLFileName = strFilePath;			
		}

        /// <summary>
        /// Sets the location in the active files file to the desired hardware type node
        /// </summary>
        /// <param name="strXmlTag">the node to find</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/13/09 AF  2.20           Created
        //
        public void XMLSetCurrentHWNode(string strXmlTag)
        {
            //Set the current node to the desired HW type node
            m_XMLSettings.SelectHWNode(strXmlTag, true);
        }

        /// <summary>
        /// Sets the location to the correct sub-node in the active files file
        /// </summary>
        /// <param name="strXmlTag">the node to find</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/13/09 AF  2.20           Created
        //
        public void XMLSetCurrentHWSubNode(string strXmlTag)
        {
            m_XMLSettings.SelectHWSubNode(strXmlTag, true);
        }

        /// <summary>
        /// Gets the firmware set for the specified meter type
        /// </summary>
        /// <param name="type">The meter type to get the firmware set for.</param>
        /// <returns>The firmware set.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        public FirmwareSet GetFirmwareSet(FirmwareSet.MeterType type)
        {
            FirmwareSet SelectedSet = null;

            foreach (FirmwareSet CurrentSet in ActiveFirmwareSets)
            {
                if (CurrentSet.FirmwareSetType == type)
                {
                    SelectedSet = CurrentSet;
                    break;
                }
            }

            return SelectedSet;
        }

		#endregion Public Methods

		#region Public Properties

		/// <summary>
		/// Property used to get the list of active programs from the XML settings
		/// file.  This list contains the full path to the files.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public List<String> ActivePrograms		
		{
			get
			{				
				return GetActiveList(XML_PROGRAMS_TAG);
			}
			set
			{
				SetActiveList(XML_PROGRAMS_TAG, value);
			}		
		}

        /// <summary>
        /// Property used to get the list of active programs file names from the XML 
        /// settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/08 mrj 1.50.00		Created
        //  
        public List<String> ActiveProgramsNames
        {
            get
            {
                List<String> lstActiveFiles = GetActiveList(XML_PROGRAMS_TAG);
                List<String> lstActiveFilesNames = new List<string>();

                foreach (string strFile in lstActiveFiles)
                {
                    int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
                    lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
                }

                return lstActiveFilesNames;
            }
        }

		/// <summary>
		/// Property used to get the list of active TOU files from the XML settings
		/// file.  This list contains the full path to the files.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public List<String> ActiveTOUFiles
		{
			get
			{				
				return GetActiveList(XML_TOU_TAG);
			}
			set
			{
				SetActiveList(XML_TOU_TAG, value);
			}			
		}

		/// <summary>
		/// Property used to get the list of active register firmware files from the 
		/// XML settings file.  This list contains the full path to the files.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public List<String> ActiveRegisterFW
		{
			get
			{
                return GetActiveList(XML_FW_TAG);
			}
			set
			{
				SetActiveList(XML_FW_TAG, value);
			}
		}

		/// <summary>
		/// Property used to get the list of active register firmware file names from 
		/// the XML settings file.  This list does not contain the file paths.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  04/15/08 mrj 1.50.00		Created
		//  
		public List<String> ActiveRegisterFWNames
		{
			get
			{
				List<String> lstActiveFiles = GetActiveList(XML_FW_TAG);
				List<String> lstActiveFilesNames = new List<string>();

				foreach (string strFile in lstActiveFiles)
				{
					int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
					lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
				}

				return lstActiveFilesNames;
			}
		}

		/// <summary>
		/// Property used to get the list of active LAN firmware files from the 
		/// XML settings file.  This list contains the full path to the files.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public List<String> ActiveLANFW
		{
			get
			{
                return GetActiveList(XML_LAN_FW_TAG);
			}
			set
			{
				SetActiveList(XML_LAN_FW_TAG, value);
			}
		}

        /// <summary>
        /// Property used to get the list of active ICS firmware files from the 
        /// XML settings file.  This list contains the full path to the files.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/05/13 jrf 2.80.26 TQ7653 Created.
        //  
        public List<String> ActiveICSFW
        {
            get
            {
                return GetActiveList(XML_ICS_FW_TAG);
            }
            set
            {
                SetActiveList(XML_ICS_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get or set the list of active Verizon Modem firmware files
        /// from the XML settings file. This list contains the full path to the files.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/10/17 AF  4.71.09  WR 749833 Added support for modem firmware for 4G ITRJ and ITRK Verizon devices
        //  12/05/17 AF  4.73.00 Task 469253 Added back the verizon modem fwdl
        //
        public List<string> ActiveVerizonModemFW
        {
            get
            {
                return GetActiveList(XML_VERIZON_MODEM_FW_TAG);
            }
            set
            {
                SetActiveList(XML_VERIZON_MODEM_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get or set the list of active ATT/Rogers Modem firmware files
        /// from the XML settings file. This list contains the full path to the files.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/05/17 AF  4.73.00 Task 469254 Added support for ATT/Rogers modem fwdl
        //
        public List<string> ActiveATTRogersBellModemFW
        {
            get
            {
                return GetActiveList(XML_ATT_ROGERS_BELL_MODEM_FW_TAG);
            }
            set
            {
                SetActiveList(XML_ATT_ROGERS_BELL_MODEM_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active LAN firmware file names from 
        /// the XML settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/15/08 mrj 1.50.00		Created
        //  
        public List<String> ActiveLANFWNames
		{
			get
			{
				List<String> lstActiveFiles = GetActiveList(XML_LAN_FW_TAG);
				List<String> lstActiveFilesNames = new List<string>();

				foreach (string strFile in lstActiveFiles)
				{
					int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
					lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
				}

				return lstActiveFilesNames;
			}
		}

		/// <summary>
		/// Property used to get the list of active ZigBee firmware files from the 
		/// XML settings file.  This list contains the full path to the files.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/08 mrj 1.00.00		Created
		//  
		public List<String> ActiveZigBeeFW
		{
			get
			{
                return GetActiveList(XML_ZIGBEE_FW_TAG);
			}
			set
			{
				SetActiveList(XML_ZIGBEE_FW_TAG, value);
			}
		}

		/// <summary>
		/// Property used to get the list of active ZigBee firmware file names from 
		/// the XML settings file.  This list does not contain the file paths.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  04/15/08 mrj 1.50.00		Created
		//  
		public List<String> ActiveZigBeeFWNames
		{
			get
			{
				List<String> lstActiveFiles = GetActiveList(XML_ZIGBEE_FW_TAG);
				List<String> lstActiveFilesNames = new List<string>();

				foreach (string strFile in lstActiveFiles)
				{
					int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
					lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
				}

				return lstActiveFilesNames;
			}
		}

        /// <summary>
        /// Property used to get the list of active Display firmware files from the
        /// XML settings file.  This list contains the full path to the files.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  10/06/08 AF  2.00             Created
        //
        public List<String> ActiveDisplayFW
        {
            get
            {
                return GetActiveList(XML_DISPLAY_FW_TAG);
            }
            set
            {
                SetActiveList(XML_DISPLAY_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active Display firmware file names from
        /// the XML settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  10/06/08 AF  2.00             Created
        //
        public List<String> ActiveDisplayFWNames
        {
            get
            {
                List<String> lstActiveFiles = GetActiveList(XML_DISPLAY_FW_TAG);
                List<String> lstActiveFilesNames = new List<string>();

                foreach (string strFile in lstActiveFiles)
                {
                    int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
                    lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
                }

                return lstActiveFilesNames;
            }
        }

        /// <summary>
        /// Property used to get the list of active Display firmware files from the
        /// XML settings file.  This list contains the full path to the files.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  04/07/09 AF  2.20.00          Created
        //
        public List<String> ActiveGasModuleFW
        {
            get
            {
                return GetActiveList(XML_GAS_MODULE_FW_TAG);
            }
            set
            {
                SetActiveList(XML_GAS_MODULE_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active Display firmware file names from
        /// the XML settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  04/07/09 AF  2.20.00           Created
        //
        public List<String> ActiveGasModuleFWNames
        {
            get
            {
                List<String> lstActiveFiles = GetActiveList(XML_GAS_MODULE_FW_TAG);
                List<String> lstActiveFilesNames = new List<string>();

                foreach (string strFile in lstActiveFiles)
                {
                    int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
                    lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
                }

                return lstActiveFilesNames;
            }
        }

        /// <summary>
        /// Property used to get the list of active Display firmware files from the
        /// XML settings file.  This list contains the full path to the files.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  04/07/09 AF  2.20.00          Created
        //
        public List<String> ActiveGasRangeExtenderFW
        {
            get
            {
                return GetActiveList(XML_GAS_RANGE_EXTENDER_FW_TAG);
            }
            set
            {
                SetActiveList(XML_GAS_RANGE_EXTENDER_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active Display firmware file names from
        /// the XML settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  04/07/09 AF  2.20.00           Created
        //
        public List<String> ActiveGasRangeExtenderFWNames
        {
            get
            {
                List<String> lstActiveFiles = GetActiveList(XML_GAS_RANGE_EXTENDER_FW_TAG);
                List<String> lstActiveFilesNames = new List<string>();

                foreach (string strFile in lstActiveFiles)
                {
                    int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
                    lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
                }

                return lstActiveFilesNames;
            }
        }


        /// <summary>
        /// Property used to get the list of active PLAN firmware files from the
        /// XML settings file.  This list contains the full path to the files.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  02/10/10 RCG 2.40.12          Created

        public List<String> ActivePLANFW
        {
            get
            {
                return GetActiveList(XML_PLAN_FW_TAG);
            }
            set
            {
                SetActiveList(XML_PLAN_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active PLAN firmware file names from
        /// the XML settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  02/10/10 RCG 2.40.12           Created

        public List<String> ActivePLANFWNames
        {
            get
            {
                List<String> lstActiveFiles = GetActiveList(XML_PLAN_FW_TAG);
                List<String> lstActiveFilesNames = new List<string>();

                foreach (string strFile in lstActiveFiles)
                {
                    int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
                    lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
                }

                return lstActiveFilesNames;
            }
        }

        /// <summary>
        /// Property used to get the list of active M2 Gateway firmware files from
        /// the XML settings file.  This list contains the full path to the file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/10 AF  2.40.37        Created
        //
        public List<String> ActiveGatewayFW
        {
            get
            {
                return GetActiveList(XML_GTWY_FW_TAG);
            }
            set
            {
                SetActiveList(XML_GTWY_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active M2 Gateway firmware file names from 
        /// the XML settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/10 AF  2.40.37        Created
        //
        public List<String> ActiveGatewayFWNames
        {
            get
            {
                List<String> lstActiveFiles = GetActiveList(XML_GTWY_FW_TAG);
                List<String> lstActiveFilesNames = new List<string>();

                foreach (string strFile in lstActiveFiles)
                {
                    int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
                    lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
                }

                return lstActiveFilesNames;
            }
        }

        /// <summary>
        /// Property used to get the list of active Cisco Comm Module firmware files from the 
        /// XML settings file.  This list contains the full path to the files.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/11 AF  2.52.22        Added for Cisco support
        //
        public List<String> ActiveCiscoFW
        {
            get
            {
                return GetActiveList(XML_CISCO_COMM_FW_TAG);
            }
            set
            {
                SetActiveList(XML_CISCO_COMM_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active Cisco Comm Module config files from the 
        /// XML settings file.  This list contains the full path to the files.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/27/11 AF  2.53.00        Created
        //
        public List<String> ActiveCiscoConfig
        {
            get
            {
                return GetActiveList(XML_CISCO_CONFIG_TAG);
            }
            set
            {
                SetActiveList(XML_CISCO_CONFIG_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active Cisco Comm Module firmware file names from 
        /// the XML settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/11 AF  2.52.22        Added for Cisco support
        //
        public List<String> ActiveCiscoFWNames
        {
            get
            {
                List<String> lstActiveFiles = GetActiveList(XML_CISCO_COMM_FW_TAG);
                List<String> lstActiveFileNames = new List<string>();

                foreach (string strFile in lstActiveFiles)
                {
                    int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
                    lstActiveFileNames.Add(strFile.Substring(iIndex + 1));
                }

                return lstActiveFileNames;
            }
        }

        /// <summary>
        /// Property used to get the list of active ChoiceConnect firmware files from the
        /// XML settings file.  This list contains the full path to the files.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  03/15/12 JJJ 2.60.xx          Created

        public List<String> ActiveChoiceConnectFW
        {
            get
            {
                return GetActiveList(XML_CC_FW_TAG);
            }
            set
            {
                SetActiveList(XML_CC_FW_TAG, value);
            }
        }

        /// <summary>
        /// Property used to get the list of active ChoiceConnect firmware file names from
        /// the XML settings file.  This list does not contain the file paths.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  03/15/12 JJJ 2.60.xx           Created

        public List<String> ActiveChoiceConnectFWNames
        {
            get
            {
                List<String> lstActiveFiles = GetActiveList(XML_CC_FW_TAG);
                List<String> lstActiveFilesNames = new List<string>();

                foreach (string strFile in lstActiveFiles)
                {
                    int iIndex = strFile.LastIndexOf(@"\", StringComparison.Ordinal);
                    lstActiveFilesNames.Add(strFile.Substring(iIndex + 1));
                }

                return lstActiveFilesNames;
            }
        }

        /// <summary>
        /// Gets the list of active firmware sets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        public List<FirmwareSet> ActiveFirmwareSets
        {
            get
            {
                return GetActiveFirmwareSets();
            }
            set
            {
                SetActiveFirmwareSets(value);
            }
        }

		#endregion

		#region Private Methods

        /// <summary>
        /// Saves the list of firmware sets.
        /// </summary>
        /// <param name="firmwareSets">The list of firmware sets</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        private void SetActiveFirmwareSets(List<FirmwareSet> firmwareSets)
        {
            m_XMLSettings.SetCurrentToRoot();
            m_XMLSettings.SelectNode(XML_ACTIVE_TAG, true);
            m_XMLSettings.SetAnchorToCurrent();

            foreach (FirmwareSet CurrentSet in firmwareSets)
            {
                CurrentSet.AddToNode(m_XMLSettings.CurrentNode);
            }

            m_XMLSettings.SaveSettings("");
        }

        /// <summary>
        /// Gets the list of active firmware sets
        /// </summary>
        /// <returns>The list of firmware sets.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        private List<FirmwareSet> GetActiveFirmwareSets()
        {
            List<FirmwareSet> FirmwareSets = new List<FirmwareSet>();

            foreach (string strHWNode in XML_HW_LIST)
            {
                m_XMLSettings.SelectHWNode(strHWNode, true);

                foreach (XmlNode CurrentChild in m_XMLSettings.CurrentNode.ChildNodes)
                {
                    FirmwareSet NewSet = new FirmwareSet(CurrentChild);

                    if (NewSet != null && NewSet.FirmwareSetType != FirmwareSet.MeterType.Unknown)
                    {
                        FirmwareSets.Add(NewSet);
                    }
                }
            }

            return FirmwareSets;
        }

		/// <summary>
		/// Gets the list of active files base
		/// </summary>
		/// <param name="strXMLTag">The tag for the list of active files</param>
		/// <returns>The list of files</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/11/08 mrj 1.00.00		Created
		//  
		private List<string> GetActiveList(string strXMLTag)
		{
			m_XMLSettings.SetCurrentToAnchor();
			m_XMLSettings.SelectNode(strXMLTag, true);

			return m_XMLSettings.CurrentNodeStringList;
		}

		/// <summary>
		/// Sets the list of active files base
		/// </summary>
		/// <param name="strXMLTag">The tag for the list of active files</param>
		/// <param name="lstrValues">The list of files to add</param>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/11/08 mrj 1.00.00		Created
		//  
		private void SetActiveList(string strXMLTag, List<string> lstrValues)
		{
			m_XMLSettings.SetCurrentToAnchor();
			m_XMLSettings.SelectNode(strXMLTag, true);

			m_XMLSettings.CurrentNodeStringList = lstrValues;

			//Save the settings so the client doesn't have to
			m_XMLSettings.SaveSettings("");
		}

		#endregion Private Methods
	}

    /// <summary>
    /// Class representing a firmware set.
    /// </summary>5
    public class FirmwareSet
    {
        #region Definitions

        /// <summary>
        /// The firmware set's type
        /// </summary>
        public enum MeterType
        {
            /// <summary>
            /// The type is unknown
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// HW 1.0 Poly meter
            /// </summary>
            HW10Poly = 1,
            /// <summary>
            /// HW 1.0 Transparent device
            /// </summary>
            HW10Transparent = 2,
            /// <summary>
            /// HW 1.5 Single Phase meter
            /// </summary>
            HW15Single = 3,
            /// <summary>
            /// HW 2.0 Basic Poly meter
            /// </summary>
            HW20BasicPoly = 4,
            /// <summary>
            /// HW 2.0 Advanced Poly meter
            /// </summary>
            HW20AdvPoly = 5,
            /// <summary>
            /// HW 2.0 Single phase meter
            /// </summary>
            HW20Single = 6,
            /// <summary>
            /// HW 2.0 Host meter
            /// </summary>
            HW20HostMeter = 7,
            /// <summary>
            /// HW 2.0 Transparent device
            /// </summary>
            HW20Transparent = 8,
            /// <summary>
            /// HW 3.0 Single phase meter
            /// </summary>
            HW30Single = 9,
            /// <summary>
            /// HW 3.0 Basic Poly meter
            /// </summary>
            HW30BasicPoly = 10,
            /// <summary>
            /// HW 3.0 Advanced Poly meter
            /// </summary>
            HW30AdvPoly = 11,
            /// <summary>
            /// HW 3.1 Single Phase Meter (ITRD)
            /// </summary>
            HW36SingleITRD = 12,
            /// <summary>
            /// HW 3.1 Basic Poly Meter (ITRE)
            /// </summary>
            HW36BasicPoly = 13,
            /// <summary>
            /// HW 3.1 Advanced Poly Meter (ITRF)
            /// </summary>
            HW36AdvPoly = 14,
            /// <summary>
            /// HW 3.1 Transparent device
            /// </summary>
            HW36Transparent = 15,
            /// <summary>
            /// HW 3.1 Single Phase Cellular 3G Meter (ITRJ)
            /// </summary>
            HW36SingleITRJ3G = 16,
            /// <summary>
            /// HW 3.1 Single Phase Cellular 4G Meter (ITRJ)
            /// </summary>
            HW36SingleITRJ4G = 17,
            /// <summary>
            /// HW 3.1 Advanced Poly Cellular 4G Meter (ITRK)
            /// </summary>
            HW36PolyITRK4G = 18,
            /// <summary>
            /// M2 Gateway HW 1.0 Single Phase
            /// </summary>
            GTWY10Single = 19,
            /// <summary>
            /// HAN Device
            /// </summary>
            HANDevice = 20,
            /// <summary>
            /// ICM Gateway Single Phase Cellular
            /// </summary>
            ICMGTWYSingle = 21,
            /// <summary>
            /// ICM Gateway Polyphase Cellular
            /// </summary>
            ICMGTWYPoly = 22,

        }

        #endregion

        #region Constants

        private const string XML_REG_FW_TAG = "RegisterFW";
        private const string XML_LAN_FW_TAG = "LANFW";
        private const string XML_ZIGBEE_FW_TAG = "ZigBeeFW";
        private const string XML_DISPLAY_FW_TAG = "DisplayFW";
        private const string XML_PLAN_FW_TAG = "PLANFW";
        private const string XML_CISCO_COMM_FW_TAG = "CiscoCommFW";
        private const string XML_CC_FW_TAG = "ChoiceConnectFW";
        private const string XML_ICS_FW_TAG = "ICSFW";

        private const string XML_HW_1_0 = "HW_1_0";
        private const string XML_HW_1_5 = "HW_1_5";
        private const string XML_HW_2_0 = "HW_2_0";
        private const string XML_HW_3_0 = "HW_3_0";
        private const string XML_HW_3_6 = "HW_3_6";
        private const string XML_GTWY1_0 = "GTWY1_0";
        private const string XML_HAN_DEV = "HAN_DEV";
        private const string XML_ICM_GTWY = "ICM_GTWY";

        private const string XML_POLY = "Polyphase";
        private const string XML_TRANS = "Transparent";
        private const string XML_SINGLE = "Single_Ph";
        private const string XML_SINGLE_ITRD = "Single_Ph_ITRD"; 
        private const string XML_BASIC_POLY = "Poly_Bas";
        private const string XML_ADV_POLY = "Poly_Adv";
        private const string XML_HOST = "Host_Meter";
        private const string XML_SINGLE_ITRJ_3G = "Single_Ph_ITRJ";
        private const string XML_SINGLE_ITRJ_4G = "Single_Ph_ITRJ_4G";
        private const string XML_POLY_ITRK_4G = "Poly_ITRK_4G"; 
        

        private const string XML_VALUE = "Value";

        private const float HW_VERSION_1_5 = 1.015F;
        private const float HW_VERSION_2_0 = 2.000F;
        private const float HW_VERSION_2_5 = 2.050F;
        private const float HW_VERSION_3_0 = 3.000F;
        private const float HW_VERSION_3_5 = 3.050F;
        private const float HW_VERSION_3_6 = 3.060F;
        private const float HW_VERSION_3_8 = 3.080F;
        private const float HW_VERSION_3_61 = 3.061F;
        private const float HW_VERSION_3_81 = 3.081F;   

        private const string ITRN_DEVICE_CLASS = "ITRN";
        private const string ITR1_DEVICE_CLASS = "ITR1";
        private const string ITR3_DEVICE_CLASS = "ITR3";
        private const string ITR4_DEVICE_CLASS = "ITR4";
        private const string ITRT_DEVICE_CLASS = "ITRT";
        private const string ITRL_DEVICE_CLASS = "ITRL";
        private const string ITRD_DEVICE_CLASS = "ITRD";
        private const string ITRE_DEVICE_CLASS = "ITRE";
        private const string ITRF_DEVICE_CLASS = "ITRF";
        private const string ITRJ_DEVICE_CLASS = "ITRJ";
        private const string ITRK_DEVICE_CLASS = "ITRK";
        private const string LIS1_DEVICE_CLASS = "LIS1";
        private const string ITRU_DEVICE_CLASS = "ITRU";
        private const string ITRV_DEVICE_CLASS = "ITRV";

        private const byte PRISM_LITE_MASK = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created
        //  11/03/14 jrf 4.00.84 542970 Added new comm fw file variables.
        public FirmwareSet()
        {
            m_strDisplayFWFile = null;
            m_strLANFWFile = null;
            m_CiscoCommFWFile = null;
            m_PLANFWFile = null;
            m_CCFWFile = null;
            m_ICSFWFile = null;
            m_strRegisterFWFile = null;
            m_strZigBeeFWFile = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">The node to get the set from</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        public FirmwareSet(XmlNode node)
            : base()
        {
            GetFromNode(node);
        }


        /// <summary>
        /// Determines the meter type.
        /// </summary>
        /// <param name="strDeviceClass">The device class of the meter.</param>
        /// <param name="fHWVersion">The hardware version</param>
        /// <param name="bTransDevMeterKey">The meter key bit for transparent devices</param>
        /// <param name="IsHANDevice">Whether or not a HAN device is desired.</param>
        /// <returns>The meter type.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created
        //  02/17/11 RCG 2.50.04        Adding Support for HW 3.0 and HW 3.1 (ITRD, ITRE, ITRF) meters
        //  03/03/11 RCG 2.50.06        Changing HW Versions to 3.060 and 3.080 for ITRD, ITRE, ITRF meters
        //  12/05/14 jrf 4.00.91 542970 Adding previously unsupported meter types.
        //  11/22/17 AF  4.73.00 Task469275 Adding code for ITRD HW 3.61 devices
        //
        public static MeterType DetermineMeterType(string strDeviceClass, float fHWVersion, bool bTransDevMeterKey, bool IsHANDevice = false)
        {
            MeterType SelectedMeterType = MeterType.Unknown;

            if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_1_5) < 0)
            {
                if (strDeviceClass == ITR1_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW10Poly;
                }
                else
                {
                    SelectedMeterType = MeterType.HW10Transparent;
                }
            }
            else if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_1_5) == 0)
            {
                SelectedMeterType = MeterType.HW15Single;
            }
            else if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_2_0) == 0)
            {
                if (strDeviceClass == ITRT_DEVICE_CLASS || bTransDevMeterKey)
                {
                    SelectedMeterType = MeterType.HW20Transparent;
                }
                else if (strDeviceClass == ITR1_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW20Single;
                }
            }
            else if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_2_5) == 0)
            {
                if (strDeviceClass == ITR3_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW20BasicPoly;
                }
                else if (strDeviceClass == ITR4_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW20AdvPoly;
                }
            }
            else if(VersionChecker.CompareTo(fHWVersion, HW_VERSION_3_0) == 0)
            {
                SelectedMeterType = MeterType.HW30Single;
            }
            else if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_3_5) == 0)
            {
                if (strDeviceClass == ITR3_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW30BasicPoly;
                }
                else if (strDeviceClass == ITR4_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW30AdvPoly;
                }
            }
            else if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_3_6) == 0)
            {
                if (strDeviceClass == ITRT_DEVICE_CLASS || bTransDevMeterKey)
                {
                    SelectedMeterType = MeterType.HW36Transparent;
                }
                else if (strDeviceClass == ITRD_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW36SingleITRD;
                }
                else if (strDeviceClass == ITRJ_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW36SingleITRJ3G;
                }                
            }
            else if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_3_61) == 0)
            {
                if (strDeviceClass == ITRD_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW36SingleITRD;
                }
                else
                {
                    SelectedMeterType = MeterType.HW36SingleITRJ4G;
                }
            }
            else if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_3_8) == 0)
            {
                if (strDeviceClass == ITRE_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW36BasicPoly;
                }
                else if(strDeviceClass == ITRF_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW36AdvPoly;
                }
            }
            else if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_3_81) == 0)
            {
                SelectedMeterType = MeterType.HW36PolyITRK4G;
            }
            else if (((byte)(fHWVersion) & PRISM_LITE_MASK) == PRISM_LITE_MASK)
            {
                SelectedMeterType = MeterType.HW20HostMeter;
            }
            else if (strDeviceClass == LIS1_DEVICE_CLASS)
            {
                SelectedMeterType = MeterType.GTWY10Single;
            }
            else if (strDeviceClass == ITRU_DEVICE_CLASS)
            {
                SelectedMeterType = MeterType.ICMGTWYSingle;
            }
            else if (strDeviceClass == ITRV_DEVICE_CLASS)
            {
                SelectedMeterType = MeterType.ICMGTWYPoly;
            }
            else if (true == IsHANDevice)
            {
                SelectedMeterType = MeterType.HANDevice;
            }

            return SelectedMeterType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the register FW file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        public string RegisterFWFile
        {
            get
            {
                return m_strRegisterFWFile;
            }
            set
            {
                m_strRegisterFWFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the LAN FW file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        public string LANFWFile
        {
            get
            {
                return m_strLANFWFile;
            }
            set
            {
                m_strLANFWFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the PLAN FW file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/14 jrf 4.00.83 542970 Created
        public string PLANFWFile
        {
            get
            {
                return m_PLANFWFile;
            }
            set
            {
                m_PLANFWFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the Cisco Comm FW file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/14 jrf 4.00.83 542970 Created
        public string CiscoCommFWFile
        {
            get
            {
                return m_CiscoCommFWFile;
            }
            set
            {
                m_CiscoCommFWFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICS FW file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/14 jrf 4.00.83 542970 Created
        public string ICSFWFile
        {
            get
            {
                return m_ICSFWFile;
            }
            set
            {
                m_ICSFWFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the ChoiceConnect FW file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/14 jrf 4.00.83 542970 Created
        public string ChoiceConnectFWFile
        {
            get
            {
                return m_CCFWFile;
            }
            set
            {
                m_CCFWFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the ZigBee firmware file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        public string ZigBeeFWFile
        {
            get
            {
                return m_strZigBeeFWFile;
            }
            set
            {
                m_strZigBeeFWFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the display firmware file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        public string DisplayFWFile
        {
            get
            {
                return m_strDisplayFWFile;
            }
            set
            {
                m_strDisplayFWFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the meter type.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        public MeterType FirmwareSetType
        {
            get
            {
                return m_MeterType;
            }
            set
            {
                m_MeterType = value;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the active firmware file paths from the xml node
        /// </summary>
        /// <param name="Node">The xml node to get the values from</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created
        //  11/03/14 jrf 4.00.84 542970 Storing additional active comm fw files.
        internal void GetFromNode(XmlNode Node)
        {
            // Determine the type
            DetermineSetType(Node);

            // Get the firmware files
            foreach(XmlNode CurrentChild in Node.ChildNodes)
            {
                switch(CurrentChild.Name)
                {
                    case XML_REG_FW_TAG:
                    {
                        if (CurrentChild.FirstChild != null)
                        {
                            m_strRegisterFWFile = CurrentChild.FirstChild.InnerText;
                        }

                        break;
                    }
                    case XML_LAN_FW_TAG:
                    {
                        if (CurrentChild.FirstChild != null)
                        {
                            m_strLANFWFile = CurrentChild.FirstChild.InnerText;
                        }

                        break;
                    }
                    case XML_PLAN_FW_TAG:
                    {
                        if (CurrentChild.FirstChild != null)
                        {
                            m_PLANFWFile = CurrentChild.FirstChild.InnerText;
                        }

                        break;
                    }
                    case XML_CISCO_COMM_FW_TAG:
                    {
                        if (CurrentChild.FirstChild != null)
                        {
                            m_CiscoCommFWFile = CurrentChild.FirstChild.InnerText;
                        }

                        break;
                    }
                    case XML_CC_FW_TAG:
                    {
                        if (CurrentChild.FirstChild != null)
                        {
                            m_CCFWFile = CurrentChild.FirstChild.InnerText;
                        }

                        break;
                    }
                    case XML_ICS_FW_TAG:
                    {
                        if (CurrentChild.FirstChild != null)
                        {
                            m_ICSFWFile = CurrentChild.FirstChild.InnerText;
                        }

                        break;
                    }
                    case XML_ZIGBEE_FW_TAG:
                    {
                        if (CurrentChild.FirstChild != null)
                        {
                            m_strZigBeeFWFile = CurrentChild.FirstChild.InnerText;
                        }

                        break;
                    }
                    case XML_DISPLAY_FW_TAG:
                    {
                        if (CurrentChild.FirstChild != null)
                        {
                            m_strDisplayFWFile = CurrentChild.FirstChild.InnerText;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the firmware file set to a XmlNode object
        /// </summary>
        /// <param name="Node">The xml node to start from</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        internal void AddToNode(XmlNode Node)
        {
            // First get or create the HW version node below the specified node
            XmlNode HWNode = FindOrCreateNode(Node, GetHWNodeName());
            XmlNode MeterTypeNode = FindOrCreateNode(HWNode, GetMeterTypeNodeName());
            XmlNode FWFileNode;
            XmlNode ValueNode;

            // Set the Register FW file
            FWFileNode = FindOrCreateNode(MeterTypeNode, XML_REG_FW_TAG);
            ValueNode = FindOrCreateNode(FWFileNode, XML_VALUE);

            if (RegisterFWFile != null || RegisterFWFile != "")
            {
                ValueNode.InnerText = RegisterFWFile;
            }
            else
            {
                MeterTypeNode.RemoveChild(FWFileNode);
            }

            // Set the LAN FW file
            FWFileNode = FindOrCreateNode(MeterTypeNode, XML_LAN_FW_TAG);
            ValueNode = FindOrCreateNode(FWFileNode, XML_VALUE);

            if (LANFWFile != null || LANFWFile != "")
            {
                ValueNode.InnerText = LANFWFile;
            }
            else
            {
                MeterTypeNode.RemoveChild(FWFileNode);
            }

            // Set the ZigBee FW file
            FWFileNode = FindOrCreateNode(MeterTypeNode, XML_ZIGBEE_FW_TAG);
            ValueNode = FindOrCreateNode(FWFileNode, XML_VALUE);

            if (ZigBeeFWFile != null || ZigBeeFWFile != "")
            {
                ValueNode.InnerText = ZigBeeFWFile;
            }
            else
            {
                MeterTypeNode.RemoveChild(FWFileNode);
            }

            // Set the Display FW file
            FWFileNode = FindOrCreateNode(MeterTypeNode, XML_DISPLAY_FW_TAG);
            ValueNode = FindOrCreateNode(FWFileNode, XML_VALUE);

            if (DisplayFWFile != null || DisplayFWFile != "")
            {
                ValueNode.InnerText = DisplayFWFile;
            }
            else
            {
                MeterTypeNode.RemoveChild(FWFileNode);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds or creates the selected node in the parent node.
        /// </summary>
        /// <param name="parent">The parent node to search through</param>
        /// <param name="strNodeToFind">The name of the node to find</param>
        /// <returns>The selected node.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created

        private XmlNode FindOrCreateNode(XmlNode parent, string strNodeToFind)
        {
            XmlNode SelectedNode = parent.SelectSingleNode(strNodeToFind);

            if (SelectedNode == null)
            {
                // We could not find the node so we need to create it
                SelectedNode = parent.OwnerDocument.CreateElement(strNodeToFind);
                parent.AppendChild(SelectedNode);
            }

            return SelectedNode;
        }

        /// <summary>
        /// Gets the meter type node name for the selected firmware set
        /// </summary>
        /// <returns>The name of the node.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created
        //  02/17/11 RCG 2.50.04        Adding Support for HW 3.0 and HW 3.1 (ITRD, ITRE, ITRF) meters
        //  03/03/11 RCG 2.50.06        Changing HW Versions to 3.060 and 3.080 for ITRD, ITRE, ITRF meters
        //  12/05/14 jrf 4.00.91 542970 Adding previously unsupported meter types.
        private string GetMeterTypeNodeName()
        {
            string strNodeName = null;

            switch (m_MeterType)
            {
                case MeterType.HW10Poly:
                case MeterType.ICMGTWYPoly:
                {
                    strNodeName = XML_POLY;
                    break;
                }
                case MeterType.HW10Transparent:
                case MeterType.HW20Transparent:
                case MeterType.HW36Transparent:
                {
                    strNodeName = XML_TRANS;
                    break;
                }
                case MeterType.HW15Single:
                case MeterType.HW20Single:
                case MeterType.HW30Single:
                case MeterType.GTWY10Single:
                case MeterType.ICMGTWYSingle:
                {
                    strNodeName = XML_SINGLE;
                    break;
                }
                case MeterType.HW36SingleITRD:
                {
                    strNodeName = XML_SINGLE_ITRD;
                    break;
                }
                case MeterType.HW36SingleITRJ3G:
                {
                    strNodeName = XML_SINGLE_ITRJ_3G;
                    break;
                }
                case MeterType.HW36SingleITRJ4G:
                {
                    strNodeName = XML_SINGLE_ITRJ_4G;
                    break;
                }
                case MeterType.HW20AdvPoly:
                case MeterType.HW30AdvPoly:
                case MeterType.HW36AdvPoly:
                {
                    strNodeName = XML_ADV_POLY;
                    break;
                }
                case MeterType.HW20BasicPoly:
                case MeterType.HW30BasicPoly:
                case MeterType.HW36BasicPoly:
                {
                    strNodeName = XML_BASIC_POLY;
                    break;
                }
                case MeterType.HW36PolyITRK4G:
                {
                    strNodeName = XML_POLY_ITRK_4G;
                    break;
                }
                case MeterType.HW20HostMeter:
                {
                    strNodeName = XML_HOST;
                    break;
                }
                case MeterType.HANDevice:
                {
                    strNodeName = XML_HAN_DEV;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException("Could not get node name: Meter Type not supported");
                }
            }

            return strNodeName;
        }

        /// <summary>
        /// Gets the hardware node name for the current firmware set
        /// </summary>
        /// <returns>The hardware node name</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created
        //  02/17/11 RCG 2.50.04        Adding Support for HW 3.0 and HW 3.1 (ITRD, ITRE, ITRF) meters
        //  12/05/14 jrf 4.00.91 542970 Adding previously unsupported meter types.
        private string GetHWNodeName()
        {
            string strHWNodeName = null;

            switch (m_MeterType)
            {
                case MeterType.HW10Poly:
                case MeterType.HW10Transparent:
                {
                    strHWNodeName = XML_HW_1_0;
                    break;
                }
                case MeterType.HW15Single:
                {
                    strHWNodeName = XML_HW_1_5;
                    break;
                }
                case MeterType.HW20AdvPoly:
                case MeterType.HW20BasicPoly:
                case MeterType.HW20HostMeter:
                case MeterType.HW20Single:
                case MeterType.HW20Transparent:
                {
                    strHWNodeName = XML_HW_2_0;
                    break;
                }
                case MeterType.HW30Single:
                case MeterType.HW30BasicPoly:
                case MeterType.HW30AdvPoly:
                {
                    strHWNodeName = XML_HW_3_0;
                    break;
                }
                case MeterType.HW36SingleITRD:
                case MeterType.HW36BasicPoly:
                case MeterType.HW36AdvPoly:
                case MeterType.HW36Transparent:
                case MeterType.HW36SingleITRJ3G:
                case MeterType.HW36SingleITRJ4G:
                case MeterType.HW36PolyITRK4G:
                {
                    strHWNodeName = XML_HW_3_6;
                    break;
                }
                case MeterType.GTWY10Single:
                {
                    strHWNodeName = XML_GTWY1_0;
                    break;
                }
                case MeterType.HANDevice:
                {
                    strHWNodeName = XML_HAN_DEV;
                    break;
                }
                case MeterType.ICMGTWYSingle:
                case MeterType.ICMGTWYPoly:
                {
                    strHWNodeName = XML_ICM_GTWY;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException("Could not get node name: Meter Type not supported");
                }
            }

            return strHWNodeName;
        }

        /// <summary>
        /// Determines the firmware set type from the specified node.
        /// </summary>
        /// <param name="Node">The node to check</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.00        Created
        //  02/17/11 RCG 2.50.04        Adding Support for HW 3.0 and HW 3.1 (ITRD, ITRE, ITRF) meters
        //  03/03/11 RCG 2.50.06        Changing HW Versions to 3.060 and 3.080 for ITRD, ITRE, ITRF meters
        //  12/05/14 jrf 4.00.91 542970 Adding previously unsupported meter types.
        private void DetermineSetType(XmlNode Node)
        {
            MeterType DeterminedType = MeterType.Unknown;

            if (Node.ParentNode != null)
            {
                switch(Node.ParentNode.Name)
                {
                    case XML_HW_1_0:
                    {
                        switch (Node.Name)
                        {
                            case XML_POLY:
                            {
                                DeterminedType = MeterType.HW10Poly;
                                break;
                            }
                            case XML_TRANS:
                            {
                                DeterminedType = MeterType.HW10Transparent;
                                break;
                            }
                        }
                        break;
                    }
                    case XML_HW_1_5:
                    {
                        switch (Node.Name)
                        {
                            case XML_SINGLE:
                            {
                                DeterminedType = MeterType.HW15Single;
                                break;
                            }
                        }
                        break;
                    }
                    case XML_HW_2_0:
                    {
                        switch (Node.Name)
                        {
                            case XML_BASIC_POLY:
                            {
                                DeterminedType = MeterType.HW20BasicPoly;
                                break;
                            }
                            case XML_ADV_POLY:
                            {
                                DeterminedType = MeterType.HW20AdvPoly;
                                break;
                            }
                            case XML_SINGLE:
                            {
                                DeterminedType = MeterType.HW20Single;
                                break;
                            }
                            case XML_HOST:
                            {
                                DeterminedType = MeterType.HW20HostMeter;
                                break;
                            }
                            case XML_TRANS:
                            {
                                DeterminedType = MeterType.HW20Transparent;
                                break;
                            }
                        }
                        break;
                    }
                    case XML_HW_3_0:
                    {
                        switch(Node.Name)
                        {
                            case XML_BASIC_POLY:
                            {
                                DeterminedType = MeterType.HW30BasicPoly;
                                break;
                            }
                            case XML_ADV_POLY:
                            {
                                DeterminedType = MeterType.HW30AdvPoly;
                                break;
                            }
                            case XML_SINGLE:
                            {
                                DeterminedType = MeterType.HW30Single;
                                break;
                            }
                        }

                        break;
                    }
                    case XML_HW_3_6:
                    {
                        switch (Node.Name)
                        {
                            case XML_BASIC_POLY:
                            {
                                DeterminedType = MeterType.HW36BasicPoly;
                                break;
                            }
                            case XML_ADV_POLY:
                            {
                                DeterminedType = MeterType.HW36AdvPoly;
                                break;
                            }
                            case XML_SINGLE_ITRD:
                            {
                                DeterminedType = MeterType.HW36SingleITRD;
                                break;
                            }
                            case XML_TRANS:
                            {
                                DeterminedType = MeterType.HW36Transparent;
                                break;
                            }
                            case XML_SINGLE_ITRJ_3G:
                            {
                                DeterminedType = MeterType.HW36SingleITRJ3G;
                                break;
                            }
                            case XML_SINGLE_ITRJ_4G:
                            {
                                DeterminedType = MeterType.HW36SingleITRJ4G;
                                break;
                            }
                            case XML_POLY_ITRK_4G:
                            {
                                DeterminedType = MeterType.HW36PolyITRK4G;
                                break;
                            }
                        }

                        break;
                    }
                    case XML_GTWY1_0:
                    {
                        DeterminedType = MeterType.GTWY10Single;
                        break;
                    }
                    case XML_HAN_DEV:
                    {
                        DeterminedType = MeterType.HANDevice;
                        break;
                    }
                    case XML_ICM_GTWY:
                    {
                        switch (Node.Name)
                        {
                            case XML_POLY:
                                {
                                    DeterminedType = MeterType.ICMGTWYPoly;
                                    break;
                                }
                            case XML_SINGLE:
                                {
                                    DeterminedType = MeterType.ICMGTWYSingle;
                                    break;
                                }
                        }

                        break;
                    }
                }
            }

            m_MeterType = DeterminedType;
        }

        #endregion

        #region Member Variables

        private string m_strRegisterFWFile;
        private string m_strLANFWFile;
        private string m_CiscoCommFWFile;
        private string m_PLANFWFile;
        private string m_CCFWFile;
        private string m_ICSFWFile;
        private string m_strZigBeeFWFile;
        private string m_strDisplayFWFile;
        private MeterType m_MeterType;

        #endregion
    }
}
