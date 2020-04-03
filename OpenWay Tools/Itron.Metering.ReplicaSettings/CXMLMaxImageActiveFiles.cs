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
//                              Copyright © 2008 - 2010
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
	public class XMLMaxImageActiveFiles : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		#region Constants

		private const string REPLICA_REG_KEY = "Replica";
        private const string XML_REPLICA_FILE_NAME = "PCProActiveFiles.xml";
		private const string XML_ACTIVE_TAG = "ActiveFiles";

        private const string XML_PROGRAMS_TAG = "ProgramsFiles";
		private const string XML_TOU_TAG = "TOUFiles";
		private const string XML_FW_TAG = "RegisterFW";
		private const string XML_ZIGBEE_FW_TAG = "ZigBeeFW";
        private const string XML_DISPLAY_FW_TAG = "DisplayFW";
    
        private const string XML_HW_3_0 = "HW_3_0";
        private readonly string[] XML_HW_LIST = { XML_HW_3_0 };
		
		#endregion Constants
		
		#region Public Methods

		/// <summary>
		/// Constructor.
		/// </summary>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  09/20/10 SCW 1.00.00		Created
		//  
		public XMLMaxImageActiveFiles()
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
        //  09/20/10 SCW  1.00.00       Created
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
        //  09/20/10 SCW  1.00.00       Created
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
        //  09/20/10 SCW  1.00.00       Created
        //
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
        //  09/20/10 SCW  1.00.00       Created/
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
        //  09/20/10 SCW  1.00.00       Created
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
        //  09/20/10 SCW  1.00.00       Created
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
        //  09/20/10 SCW  1.00.00       Created
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
        //  09/20/10 SCW  1.00.00       Created
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
		/// Property used to get the list of active ZigBee firmware files from the 
		/// XML settings file.  This list contains the full path to the files.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
        //  09/20/10 SCW  1.00.00       Created
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
        //  09/20/10 SCW  1.00.00       Created
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
        //  09/20/10 SCW  1.00.00       Created
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
        //  09/20/10 SCW  1.00.00       Created
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
        /// Gets the list of active firmware sets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/20/10 SCW  1.00.00       Created
        //
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
    public class MaxImageFirmwareSet
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
            /// HW 3.0 MaxImage device
            /// </summary>
            HW30Single
        }

        #endregion

        #region Constants

        private const string XML_REG_FW_TAG = "RegisterFW";
        private const string XML_ZIGBEE_FW_TAG = "ZigBeeFW";
        private const string XML_DISPLAY_FW_TAG = "DisplayFW";

        private const string XML_HW_3_0 = "HW_3_0";

        private const string XML_SINGLE = "Single_Ph"; 
        private const string XML_VALUE = "Value";

        private const float HW_VERSION_3_0 = 3.000F;
  
        private const string ITRA_DEVICE_CLASS = "ITRA";
      
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/20/10 SCW  1.00.00       Created
        //
        public MaxImageFirmwareSet()
        {
            m_strDisplayFWFile = null;
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
        //  09/20/10 SCW  1.00.00       Created
        //
        public MaxImageFirmwareSet(XmlNode node)
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
        /// <returns>The meter type.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/20/10 SCW  1.00.00       Created
        //
        public static MeterType DetermineMeterType(string strDeviceClass, float fHWVersion, bool bTransDevMeterKey)
        {
            MeterType SelectedMeterType = MeterType.Unknown;

            if (VersionChecker.CompareTo(fHWVersion, HW_VERSION_3_0) > 0)
            {
                if (strDeviceClass == ITRA_DEVICE_CLASS)
                {
                    SelectedMeterType = MeterType.HW30Single;
                }
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
        //  09/20/10 SCW  1.00.00       Created
        //        
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
        /// Gets or sets the ZigBee firmware file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/20/10 SCW  1.00.00       Created
        //
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
        //  09/20/10 SCW  1.00.00       Created
        //
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
        //  09/20/10 SCW  1.00.00       Created
        //
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
        //  09/20/10 SCW  1.00.00       Created
        //
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
        //  09/20/10 SCW  1.00.00       Created
        //
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
        //  09/20/10 SCW  1.00.00       Created
        //
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
        //  09/20/10 SCW  1.00.00       Created
        //
        private string GetMeterTypeNodeName()
        {
            string strNodeName = null;

            switch (m_MeterType)
            {
                
                case MeterType.HW30Single:
                {
                    strNodeName = XML_SINGLE;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException("Could not get node name: MeterType not supported");
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
        //  09/20/10 SCW  1.00.00       Created
        //
        private string GetHWNodeName()
        {
            string strHWNodeName = null;

            switch (m_MeterType)
            {
                case MeterType.HW30Single:
                {
                    strHWNodeName = XML_HW_3_0;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException("Could not get node name: MeterType not supported");
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
        //  09/20/10 SCW  1.00.00       Created
        //
        private void DetermineSetType(XmlNode Node)
        {
            MeterType DeterminedType = MeterType.Unknown;

            if (Node.ParentNode != null)
            {
                switch(Node.ParentNode.Name)
                {
                    case XML_HW_3_0:
                    {
                        switch (Node.Name)
                        {
                            case XML_SINGLE:
                            {
                                DeterminedType = MeterType.HW30Single;
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
        private string m_strZigBeeFWFile;
        private string m_strDisplayFWFile;
        private MeterType m_MeterType;

        #endregion
    }
}
