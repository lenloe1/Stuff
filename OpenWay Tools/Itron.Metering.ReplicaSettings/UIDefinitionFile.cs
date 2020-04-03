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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Xml;
using System.Globalization;
using Itron.Metering.ReplicaSettings.Properties;

namespace Itron.Metering.ReplicaSettings
{

	/// <summary>
    /// Enumeration of function key types
    /// </summary>
    public enum FunctionKeyType
    {
        /// <summary>
        /// a function; can't add anything under a function in the menu layout editor
        /// </summary>
        FUNCTION = 0,
        /// <summary>
        /// a menu; there should be functions below a menu
        /// </summary>
        MENU = 1,
        /// <summary>
        /// confirmations of functions that do not show up in the menu layout editor
        /// </summary>
        CONFIRM = 2,
    }//FunctionKeyType

    /// <summary>
    /// Enumeration of subverted menus
    /// </summary>
    public enum SubvertedMenu : int
    {
        /// <summary>
        /// Opt Out Menu
        /// </summary>
        OptOut = -1,
    }

    /// <summary>
    /// The FunctionKeyAssignment class represents the 'KeyHandler' tag in the user interface definition file.
    /// For each possible menu item (function key) it identifies the key's label, the logic that should be exercised
    /// when the key is pressed, and the next menu that should be displayed after it is pressed
    /// </summary>
    public class FunctionKeyAssignment
	{

		#region Public Methods

		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nFunctionKeyID"></param>
        /// <param name="strKeyLabel"></param>
        /// <param name="nHandlerID"></param>
        /// <param name="nSubmenuID"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        public FunctionKeyAssignment( int nFunctionKeyID, String strKeyLabel, int nHandlerID, int nSubmenuID)
        {
            FunctionKey = nFunctionKeyID;
            KeyLabel = strKeyLabel;
            HandlerID = nHandlerID;
            SubmenuID = nSubmenuID;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nFunctionKeyID"></param>
        /// <param name="strKeyLabel"></param>
        /// <param name="nHandlerID"></param>
        /// <param name="nSubmenuID"></param>
        /// <param name="eType"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        public FunctionKeyAssignment(int nFunctionKeyID, String strKeyLabel, int nHandlerID, int nSubmenuID, FunctionKeyType eType)
        {
            FunctionKey = nFunctionKeyID;
            KeyLabel = strKeyLabel;
            HandlerID = nHandlerID;
            SubmenuID = nSubmenuID;
            Type = eType;
		}

		#endregion

		#region Public Properties

		/// <summary>
        /// Property for the function key
        /// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        public int FunctionKey
        {
            get
            {
                return m_nFunctionKeyID;
            }
            set
            {
                m_nFunctionKeyID = value;
            }
        }

        /// <summary>
        /// Property for the key text
        /// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        public String KeyLabel
        {
            get
            {
                return m_strKeyLabel;
            }
            set
            {
                m_strKeyLabel = value;
            }
        }

        /// <summary>
        /// Property for the handler ID
        /// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        public int HandlerID
        {
            get
            {
                return m_nHandlerID;
            }
            set
            {
                m_nHandlerID = value;
            }
        }

        /// <summary>
        /// Property for the submemu ID
        /// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        public int SubmenuID
        {
            get
            {
                return m_nSubmenuID;
            }
            set
            {
                m_nSubmenuID = value;
            }
        }

        /// <summary>
        /// Property for the function type
        /// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        public FunctionKeyType Type
        {
            get
            {
                return m_eType;
            }
            set
            {
                m_eType = value;
            }
		}

		#endregion

		#region Members

		private int m_nFunctionKeyID;
        private String m_strKeyLabel;
        private int m_nHandlerID;
        private int m_nSubmenuID;
        private FunctionKeyType m_eType;

		#endregion

    }//FunctionKeyAssignment
    
    /// <summary>
    /// The UIDefinitionFile represents that XML file that describes the menu structure to be used in the calling
    /// application.  The menu structure is defined using the following tags: 
    /// Submenu - used to identify each menu structure
    /// DefaultKey - used to identify the keyhandler that should be executed when the submenu is initially displayed
    /// KeyHandler - used to define each function key that is included in the submenu
    ///
    /// The UIDefinitionFile will be located in the application's data directory
    /// </summary>
    public class UIDefinitionFile
    {

        #region Constants

        private const string SUBMENU = "Submenu";
        private const string MENU_ID = "MenuID";
        private const string MENU_TITLE = "MenuTitle";
        private const string KEY_HANDLER = "KeyHandler";
        private const string FUNCTION_KEY = "FunctionKey";
        private const string KEY_TEXT = "KeyText";
        private const string DEFAULT_KEY = "DefaultKey";
        private const string CHILD_MENU_ID = "ChildMenuID";
        private const string HANDLER_ID = "HandlerID";
        private const string TYPE = "Type";

        #endregion

        #region Constructors

        /// <summary>
        /// This constructor opens the UIDefinition file and caches the list of submenu
        /// nodes.
        /// </summary>
        /// <param name="strFileName"></param>
        public UIDefinitionFile( String strFileName )
        {
            //set the File Name and create and load the XmlDocument to hold 
            //the xml file
            m_strUIDefinitionName = strFileName;
            m_xmldomUIDefinitions = new XmlDocument();
            m_xmldomUIDefinitions.Load(m_strUIDefinitionName);

            // Read and cache all of menu structures

            //Local Variables
            m_xmlMenuNodeList = null;

            // read and cache all of the sub menus
            m_xmlMenuNodeList = m_xmldomUIDefinitions.GetElementsByTagName(SUBMENU);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to read the complete definition of a submeny from
        /// the UI Definition file.  Note that the starting menuID will always be defined
        /// as 1.
        /// </summary>
        /// <param name="nMenuID">The ID of the menu to read.</param>
        /// <param name="strMenuTitle">The title of the menu.</param>
        /// <param name="KeyAssignments">The assignments for keys in the menu.</param>
        /// <param name="nDefaultKey">The default key that should be pressed when keys are loaded.</param>
        /// <returns>Whether or not the menu was read.</returns>	
        public Boolean ReadMenu(int nMenuID,
            out String strMenuTitle,
            out List<FunctionKeyAssignment> KeyAssignments,
            out int nDefaultKey)
        {
            Boolean boolMenuFound = false;

            strMenuTitle = "";
            nDefaultKey = 0;

            if (IsMenuSpecial(nMenuID))
            {
                boolMenuFound = ReadSpecialMenu(nMenuID, out strMenuTitle, out KeyAssignments, out nDefaultKey);
            }
            else
            {

                KeyAssignments = new List<FunctionKeyAssignment>();

                // Search through all the submenu nodes until we find
                // the menu we are looking for by comparing the menu IDs

                foreach (XmlNode menuNode in m_xmlMenuNodeList)
                {
                    String strMenuID = GetAttribute(menuNode, MENU_ID);

                    if (strMenuID == nMenuID.ToString(CultureInfo.InvariantCulture))
                    {
                        boolMenuFound = true;

                        // Extract the menu title

                        strMenuTitle = GetAttribute(menuNode, MENU_TITLE);

                        // Next extract the key handler definitions

                        XmlNodeList xmlMenuItemNodeList = menuNode.ChildNodes;

                        foreach (XmlNode childNode in xmlMenuItemNodeList)
                        {
                            if (childNode.Name == KEY_HANDLER)
                            {
                                int nKeyID = Int16.Parse(GetAttribute(childNode, FUNCTION_KEY), CultureInfo.InvariantCulture);
                                String strKeyLabel = GetAttribute(childNode, KEY_TEXT);
                                int nHandlerID = ReadHandlerID(childNode);
                                int nSubmenuID = ReadSubMenuID(childNode);
                                FunctionKeyType eType = ReadType(childNode);
                                KeyAssignments.Add(new FunctionKeyAssignment(nKeyID, strKeyLabel, nHandlerID, nSubmenuID, eType));
                            }
                            else if (childNode.Name == DEFAULT_KEY)
                            {
                                nDefaultKey = Int16.Parse(childNode.InnerText, CultureInfo.InvariantCulture);
                            }
                        }

                        break;
                    }
                }
            }

            return boolMenuFound;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if the menu ID is special.
        /// </summary>
        /// <param name="menuID">The ID of the menu to check</param>
        /// <returns>Whether or not menu is special</returns>
        private bool IsMenuSpecial(int menuID)
        {
            //Menu IDs less than zero don't interfere with regular customizable menu ids,
            //so we are using them for special cases.
            return menuID < 0;
        }
        /// <summary>
        /// This method is used when the definition of a submenu from
        /// the UI Definition file is being subverted and this new menu structure will take its place.  
        /// </summary>
        /// <param name="nMenuID">The ID of the menu to read.</param>
        /// <param name="strMenuTitle">The title of the menu.</param>
        /// <param name="KeyAssignments">The assignments for keys in the menu.</param>
        /// <param name="nDefaultKey">The default key that should be pressed when keys are loaded.</param>
        /// <returns>Whether or not the menu was read.</returns>	
        private Boolean ReadSpecialMenu(int nMenuID,
            out String strMenuTitle,
            out List<FunctionKeyAssignment> KeyAssignments,
            out int nDefaultKey)
        {
            Boolean boolMenuFound = false;

            strMenuTitle = "";
            nDefaultKey = 0;

            KeyAssignments = new List<FunctionKeyAssignment>();

            switch ((SubvertedMenu)nMenuID)
            {
                case SubvertedMenu.OptOut:
                {
                    strMenuTitle = Resources.OptOut;
                    KeyAssignments.Add(new FunctionKeyAssignment(1, Resources.Enable, 346, 0, FunctionKeyType.CONFIRM));
                    KeyAssignments.Add(new FunctionKeyAssignment(2, Resources.Disable, 347, 0, FunctionKeyType.CONFIRM));
                    nDefaultKey = 0;
                    boolMenuFound = true;
                    break;
                }
                default:
                {
                    break;
                }
            }

            return boolMenuFound;
        }

        /// <summary>
        /// Reads sub menu id
        /// </summary>
        /// <param name="childNode"></param>
        /// <returns></returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //								Created
        //
        private int ReadSubMenuID(XmlNode childNode )
        {
            int nSubmenuID = 0;

            String strChildMenu = GetAttribute(childNode, CHILD_MENU_ID);

            if (strChildMenu.Length > 0)
            {
                nSubmenuID = Int16.Parse(strChildMenu, CultureInfo.InvariantCulture);
            }
            return nSubmenuID;
        }

        /// <summary>
        /// Reads handler id
        /// </summary>
        /// <param name="childNode"></param>
        /// <returns></returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        private int ReadHandlerID(XmlNode childNode)
        {
            int nHandlerID = 0; // The default handler

            String strHandlerID = GetAttribute(childNode, HANDLER_ID);

            if (strHandlerID.Length > 0)
            {
                nHandlerID = Int16.Parse(strHandlerID, CultureInfo.InvariantCulture);
            }
            return nHandlerID;
        }

        /// <summary>
        /// Reads function type
        /// </summary>
        /// <param name="childNode"></param>
        /// <returns></returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        private FunctionKeyType ReadType(XmlNode childNode)
        {
            FunctionKeyType eType = FunctionKeyType.CONFIRM;

            String strType = GetAttribute(childNode, TYPE);

            if (strType.Length > 0)
            {
                eType = (FunctionKeyType)Int16.Parse(strType, CultureInfo.InvariantCulture);
            }
            return eType;
        }

        /// <summary>
        /// Gets attribute
        /// </summary>
        /// <param name="node"></param>
        /// <param name="strAttributeName"></param>
        /// <returns></returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//
        private String GetAttribute(XmlNode node, String strAttributeName)
        {
            String strAttributeValue;
            
            XmlNode attributeNode = node.Attributes.GetNamedItem( strAttributeName );

            if (attributeNode != null)
            {
                strAttributeValue = attributeNode.Value;
            }
            else
            {
                strAttributeValue = "";
            }

            return strAttributeValue;
        }
        
        #endregion

        #region Members

        /// <summary>
        /// Variable used to represent the xml file
        /// </summary>
        protected System.Xml.XmlDocument m_xmldomUIDefinitions;

        /// <summary>
        /// 
        /// </summary>
        protected XmlNodeList m_xmlMenuNodeList;

        /// <summary>
        /// The name of the XML file that contains a description of the menu structure
        /// </summary>
        private String m_strUIDefinitionName;

        #endregion

    }//UIDefinitionFile

}//Itron.Metering.ReplicaSettings