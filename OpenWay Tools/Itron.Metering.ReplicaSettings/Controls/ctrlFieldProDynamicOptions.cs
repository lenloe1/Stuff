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
//                            Copyright © 2006 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Itron.Metering.Utilities;
using System.Globalization;

namespace Itron.Metering.ReplicaSettings
{
    /// <summary>
    /// Control used for the dynamic Field-Pro options
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  11/29/06 mrj 8.00.00		Created
    //
    public partial class ctrlFieldProDynamicOptions : UserControl
	{
		#region Constants
		
		//These 3 constants should only be used by the System Manager
		private const string FILE_PATH_REPLICA = "Replica";
		private const string DEVICE_FEATURE_XML_FILE = "DeviceFeatureStructure.xml";
		private const string DEVICE_MENU_XML_FILE = "DeviceMenuStructure.xml";

		private const int FEATURE_NAME_ATTRIBUTE = 0;
		private const int FEATURE_HANDLER_ID_ATTRIBUTE = 1;
		private const int MAX_MENU_FUNCTION_KEYS = 8;
		private const string FEATURE_DESC_TAG = "Desc";
		private const string CONFIRMATION_TAG = "Confirmation";

        private const string DEVICE_MENU = "DeviceMenu";
        private const string SUB_MENU = "Submenu";
        private const string MENU_ID = "MenuID";
        private const string MENU_TITLE = "MenuTitle";
        private const string KEY_HANDLER = "KeyHandler";
        private const string FUNCTION_KEY = "FunctionKey";
        private const string KEY_TEXT = "KeyText";
        private const string CHILD_MENU_ID = "ChildMenuID";
        private const string HANDLER_ID = "HandlerID";
        private const string TYPE = "Type";
        private const string GROUP = "Group";

		#endregion
		
		#region Public Methods

		/// <summary>
		/// Contructor
		/// </summary>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/29/06 mrj 8.00.00		Created
		//
		public ctrlFieldProDynamicOptions()
		{
			InitializeComponent();

			//Initialize these variables but they can be changed through properties 
			m_strReplicaKey = FILE_PATH_REPLICA;
			m_strDeviceFeatureFile = DEVICE_FEATURE_XML_FILE;
			m_strDeviceMenuFile = DEVICE_MENU_XML_FILE;
			
			m_bChangesMade = false;

			//Create the selected feature list.  This list keeps track of which
			//features the user has selected.
			m_strSelectedFeatureList = new StringCollection();
			
        }//ctrlFieldProDynamicOptions

        /// <summary>
        /// Load the layout from the current version of the menu layout file into the
        /// right-hand tree view
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 rdb 8.00.00		Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TreeNode.#ctor(System.String)")]
        public void LoadLayout()
		{
			if (m_UIDefinitionFile == null)
			{
				//Get the definition file if it has not been created yet
				m_UIDefinitionFile = new UIDefinitionFile(CRegistryHelper.GetFilePath(m_strReplicaKey) + m_strDeviceMenuFile);
			}

			LayoutTreeView.BeginUpdate();

			//Remove any existing nodes
			LayoutTreeView.Nodes.Clear();
						
			//Clear the selected feature list
			ClearSelectedFeatureList();


			//List of function key assignments in which the index of the function key
			//assignment corresponds to the Menu ID of that function key assignment
			List<FunctionKeyAssignment> lKeyAssignments;

			int iDefaultKey;
			String strMenuTitle;

			//stack full of tree nodes used to write the layout that is being loaded
			Stack<TreeNode> stackNodes = new Stack<TreeNode>();

			//stack full of menu IDs that keeps track of the menu ID of each node in the
			//tree view
			Stack<int> stackIDs = new Stack<int>();

			//do an initial read of the main menu
			m_UIDefinitionFile.ReadMenu(1, out strMenuTitle,
				out lKeyAssignments, out iDefaultKey);

			//add the main menu to the tree and push it onto both stacks
			LayoutTreeView.Nodes.Add(strMenuTitle);
			LayoutTreeView.Nodes[0].Tag = new DeviceFeature(strMenuTitle, 0, ""); 
			stackNodes.Push(LayoutTreeView.Nodes[0]);

            //push a 1 for the main menu's menu ID; it will always be 1
			stackIDs.Push(1);

			while (stackNodes.Count > 0)
			{
				//pop the stack to get the node and ID of the node on top
				TreeNode tnParent = stackNodes.Pop();
				int iParentID = stackIDs.Pop();

				//read the menu that was just popped
				m_UIDefinitionFile.ReadMenu(iParentID, out strMenuTitle,
								  out lKeyAssignments, out iDefaultKey);

				TreeNode tnNew;
                bool bFillEmpties = true;
                int iCurrentKey = 1;

                //go through each key assignment...put in empties between keys that
                //are not right next to each other
                for (int iKeyCount = 0; iKeyCount < lKeyAssignments.Count; iKeyCount++)
                {

                    FunctionKeyAssignment fkaKey = lKeyAssignments[iKeyCount];

                    while (iCurrentKey != fkaKey.FunctionKey)
                    {
                        tnNew = new TreeNode(
                                "F" + (iCurrentKey).ToString(CultureInfo.InvariantCulture) + " - " +
                                Properties.Resources.EMPTY);
                        tnNew.ForeColor = SystemColors.InactiveCaptionText;
                        tnParent.Nodes.Add(tnNew);
                        iCurrentKey++;
                    }

                    if (fkaKey.Type == FunctionKeyType.MENU)
                    {
                        tnNew = new TreeNode("F" + fkaKey.FunctionKey.ToString(CultureInfo.InvariantCulture) +
                                            " - " + fkaKey.KeyLabel);

                        DeviceFeature devFeature = new DeviceFeature(
                                fkaKey.KeyLabel, 0, "");
                        tnNew.Tag = devFeature;

                        tnParent.Nodes.Add(tnNew);

                        //push the new key on both stacks
                        stackNodes.Push(tnNew);
                        stackIDs.Push(fkaKey.SubmenuID);
                    }
                    else if (fkaKey.Type == FunctionKeyType.FUNCTION)
                    {

                        tnNew = new TreeNode("F" + fkaKey.FunctionKey.ToString(CultureInfo.InvariantCulture) +
                                            " - " + fkaKey.KeyLabel);

                        DeviceFeature devFeature = new DeviceFeature(
                                fkaKey.KeyLabel,
                                fkaKey.HandlerID, "");						

                        //these stacks help make the tree under the device feature
                        //if the function has menus under it that are always present
                        Stack<int> stackMenuID = new Stack<int>();
                        Stack<DeviceFeature> stackFeatures =
                            new Stack<DeviceFeature>();

                        stackMenuID.Push(fkaKey.SubmenuID);
                        stackFeatures.Push(devFeature);

                        while (stackMenuID.Count > 0)
                        {
                            String strTitle;
                            List<FunctionKeyAssignment> lAssignments;
                            int iDefault;

                            //get the current menu ID and feature from the top of the
                            //stack
                            int curSubMenu = stackMenuID.Pop();
                            DeviceFeature curFeature = stackFeatures.Pop();

                            //read the menu for the current menu ID
                            m_UIDefinitionFile.ReadMenu(curSubMenu, out strTitle,
                                out lAssignments, out iDefault);

                            //push all of the current menu's submenus on both stacks
                            //and make device features for them
                            foreach (FunctionKeyAssignment key in lAssignments)
                            {
                                stackMenuID.Push(key.SubmenuID);

                                DeviceFeature newFeature = new DeviceFeature(
                                    key.KeyLabel, key.HandlerID, "");

                                curFeature.Children.Add(newFeature);
                                stackFeatures.Push(newFeature);
                            }
                        }

                        tnNew.Tag = devFeature;

                        tnParent.Nodes.Add(tnNew);

						//Add the feature to the selected list
						m_strSelectedFeatureList.Add(tnNew.Text.Substring(5));
						

                        //push the new key on both stacks
                        stackNodes.Push(tnNew);
                        stackIDs.Push(fkaKey.SubmenuID);
                    }
                    else if (fkaKey.Type == FunctionKeyType.CONFIRM)
                    {
                        bFillEmpties = false;
                    }

                    iCurrentKey++;

                }
                if (((DeviceFeature)tnParent.Tag).HandlerID == 0 ||
                    iCurrentKey > 1 && bFillEmpties)
                {
                    while (iCurrentKey <= MAX_MENU_FUNCTION_KEYS)
                    {
                        tnNew = new TreeNode(
                                "F" + (iCurrentKey).ToString(CultureInfo.InvariantCulture) + " - " +
								Properties.Resources.EMPTY);
                        tnNew.ForeColor = SystemColors.InactiveCaptionText;
                        tnParent.Nodes.Add(tnNew);
                        iCurrentKey++;
                    }
                }

			}

			LayoutTreeView.Nodes[0].Expand();
			LayoutTreeView.SelectedNode = LayoutTreeView.Nodes[0];

			LayoutTreeView.EndUpdate();

			//Set the changed flag to false
			m_bChangesMade = false;

			//Set the selected features to bold
			SetSelectedFeatureIndicators();
		}//LoadLayout

        /// <summary>
        /// Writes the menu structure to the xml file
        /// </summary>
        /// <returns>
        /// True if able to write the file
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 rdb 8.00.00		Created
        //	12/13/06 mrj 8.00.00		Re-worked to allow duplicate functions and menus.
        //  10/25/16 AF  4.70.27 724185 The writer object has to be closed before we access the
        //                              UI definition file
        //
        public void WriteFile()		
		{
			int iSubMenuCount = 0;
			int iMenuTitleIndex;
			Queue<TreeNode> qNodes = new Queue<TreeNode>();
			TreeNode curNode = LayoutTreeView.Nodes[0];
            XmlTextWriter writer = null;

            try
            {
                //Clear the child menu ID's from all the layout nodes
                ClearChildMenuIDs(curNode);


                writer = new XmlTextWriter(CRegistryHelper.GetFilePath(m_strReplicaKey) + m_strDeviceMenuFile, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;

                //Start the xml document
                writer.WriteStartDocument();
                writer.WriteStartElement(DEVICE_MENU);

                qNodes.Enqueue(curNode);

                //Traverse all of the nodes in level order
                while (qNodes.Count > 0)
                {
                    curNode = qNodes.Dequeue();

                    //The node is a function with one or more static submenus under it
                    if (curNode.Tag != null &&
                       ((DeviceFeature)curNode.Tag).Children.Count > 0)
                    {
                        //This queue goes through the sub tree in this node's tag
                        Queue<DeviceFeature> qFeatures = new Queue<DeviceFeature>();

                        qFeatures.Enqueue((DeviceFeature)curNode.Tag);

                        //Traverse the tree in the node's tag in level order
                        while (qFeatures.Count > 0)
                        {
                            DeviceFeature curFeature = qFeatures.Dequeue();

                            //If the current feature is a menu then it will have to be
                            //written to the xml document
                            if (curFeature.Children.Count > 0)
                            {
                                writer.WriteStartElement(SUB_MENU);

                                //Get the ChildMenuID if present
                                iMenuTitleIndex = curFeature.ChildMenuID;

                                if (0 != iMenuTitleIndex)
                                {
                                    //If the menu has already been assigned a menu ID then 
                                    //use that menu ID								
                                    writer.WriteAttributeString(MENU_ID, (iMenuTitleIndex).ToString(CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    //If the menu has not yet been assigned a menu ID then 
                                    //increment the sub menu count and assign the chile menu ID									
                                    iSubMenuCount++;
                                    curFeature.ChildMenuID = iSubMenuCount;

                                    writer.WriteAttributeString(MENU_ID, iSubMenuCount.ToString(CultureInfo.InvariantCulture));
                                }

                                writer.WriteAttributeString(MENU_TITLE, curFeature.KeyText);

                                //Write each child of the current feature to the file and
                                //enqueue it in the queue
                                for (int iChild = 0; iChild < curFeature.Children.Count; iChild++)
                                {
                                    qFeatures.Enqueue(curFeature.Children[iChild]);

                                    writer.WriteStartElement(KEY_HANDLER);
                                    writer.WriteAttributeString(FUNCTION_KEY, (iChild + 1).ToString(CultureInfo.InvariantCulture));
                                    writer.WriteAttributeString(KEY_TEXT, curFeature.Children[iChild].KeyText);

                                    //If the child itself is also a menu then write a child menu ID
                                    if (curFeature.Children[iChild].Children.Count > 0)
                                    {
                                        iMenuTitleIndex = curFeature.Children[iChild].ChildMenuID;

                                        if (0 != iMenuTitleIndex)
                                        {
                                            //The child menu ID was set so write it
                                            writer.WriteAttributeString(CHILD_MENU_ID, (iMenuTitleIndex).ToString(CultureInfo.InvariantCulture));
                                        }
                                        else
                                        {
                                            //This child menu ID was not set so increment the count, set the
                                            //ID, and write it
                                            iSubMenuCount++;
                                            curFeature.Children[iChild].ChildMenuID = iSubMenuCount;

                                            writer.WriteAttributeString(CHILD_MENU_ID, iSubMenuCount.ToString(CultureInfo.InvariantCulture));
                                        }
                                    }

                                    //Write the handler ID for the child if it has one
                                    if (curFeature.Children[iChild].HandlerID.ToString(CultureInfo.InvariantCulture) != "0")
                                    {
                                        writer.WriteAttributeString(HANDLER_ID, curFeature.Children[iChild].HandlerID.ToString(CultureInfo.InvariantCulture));
                                    }

                                    //Write the type of the child - always a confirm in 
                                    //this case
                                    writer.WriteAttributeString(TYPE, ((int)FunctionKeyType.CONFIRM).ToString(CultureInfo.InvariantCulture));
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }
                        }
                    }
                    //the node is a menu
                    else if (curNode.Tag != null && curNode.Nodes.Count > 0)
                    {
                        //Write the new sub menu
                        writer.WriteStartElement(SUB_MENU);


                        //See if the current node has a child menu ID						
                        iMenuTitleIndex = ((DeviceFeature)curNode.Tag).ChildMenuID;

                        if (0 != iMenuTitleIndex)
                        {
                            //The child menu ID was set so write it
                            writer.WriteAttributeString(MENU_ID, (iMenuTitleIndex).ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            //This child menu ID was not set so increment the count, set the
                            //ID, and write it
                            iSubMenuCount++;
                            ((DeviceFeature)curNode.Tag).ChildMenuID = iSubMenuCount;

                            writer.WriteAttributeString(MENU_ID, iSubMenuCount.ToString(CultureInfo.InvariantCulture));
                        }

                        //Take the "F# - " (function key) off the beginning of the key text and
                        //write the menu title
                        if (curNode.Text.StartsWith("F", StringComparison.Ordinal))
                        {
                            writer.WriteAttributeString(MENU_TITLE, curNode.Text.Substring(5));
                        }
                        //there is no function key at the beginning of the key text
                        else
                        {
                            writer.WriteAttributeString(MENU_TITLE, curNode.Text);
                        }

                        //Write each child of the current node to the xml file
                        for (int iNode = 0; iNode < curNode.Nodes.Count; iNode++)
                        {
                            if (curNode.Nodes[iNode].Text.Substring(5) != Properties.Resources.EMPTY)
                            {
                                writer.WriteStartElement(KEY_HANDLER);
                                writer.WriteAttributeString(FUNCTION_KEY, (iNode + 1).ToString(CultureInfo.InvariantCulture));
                                writer.WriteAttributeString(KEY_TEXT, curNode.Nodes[iNode].Text.Substring(5));

                                //Check to see if this child is a menu or has any confirmation screens
                                if (curNode.Nodes[iNode].Nodes.Count > 0 ||
                                   ((DeviceFeature)curNode.Nodes[iNode].Tag).Children.Count > 0)
                                {
                                    //Get the child menu ID
                                    iMenuTitleIndex = ((DeviceFeature)curNode.Nodes[iNode].Tag).ChildMenuID;

                                    if (0 != iMenuTitleIndex)
                                    {
                                        //The child menu ID was set so write it
                                        writer.WriteAttributeString(CHILD_MENU_ID, (iMenuTitleIndex).ToString(CultureInfo.InvariantCulture));
                                    }
                                    else
                                    {
                                        //This child menu ID was not set so increment the count, set the
                                        //ID, and write it
                                        iSubMenuCount++;
                                        ((DeviceFeature)curNode.Nodes[iNode].Tag).ChildMenuID = iSubMenuCount;

                                        writer.WriteAttributeString(CHILD_MENU_ID, iSubMenuCount.ToString(CultureInfo.InvariantCulture));
                                    }
                                }

                                //Write the handler ID if it has one
                                if (((DeviceFeature)curNode.Nodes[iNode].Tag).HandlerID.ToString(CultureInfo.InvariantCulture) != "0")
                                {
                                    writer.WriteAttributeString(HANDLER_ID, ((DeviceFeature)curNode.Nodes[iNode].Tag).HandlerID.ToString(CultureInfo.InvariantCulture));
                                }

                                //Write the type
                                if (curNode.Nodes[iNode].Nodes.Count > 0)
                                {
                                    writer.WriteAttributeString(TYPE, ((int)FunctionKeyType.MENU).ToString(CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    writer.WriteAttributeString(TYPE, ((int)FunctionKeyType.FUNCTION).ToString(CultureInfo.InvariantCulture));
                                }
                                writer.WriteEndElement();
                            }
                        }

                        writer.WriteEndElement();

                    }
                    //Enqueue all the current node's children so they will be processed
                    foreach (TreeNode child in curNode.Nodes)
                    {
                        if (child.Tag != null)
                        {
                            qNodes.Enqueue(child);
                        }
                    }
                }

                //End the xml file
                writer.WriteEndElement();
            }
            finally
            {
                writer.Close();

                //Make sure that we have the latest definition file now that it has been saved
                m_UIDefinitionFile = null;
                m_UIDefinitionFile = new UIDefinitionFile(CRegistryHelper.GetFilePath(m_strReplicaKey) + m_strDeviceMenuFile);

                //Set the changed flag to false
                m_bChangesMade = false;
            }
        }//WriteFile

        #endregion

		#region Public Properties

		/// <summary>
		/// Property to get the changes made flag
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/05/06 mrj 8.00.00		Created
		//
		public bool ChangesMade
		{
			get
			{
				return m_bChangesMade;
			}
		}

		/// <summary>
		/// Property to set the Replica registry key name
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/07/08 mrj 1.00.00		Created (OpenWay Tools project)
		//  
		public string ReplicaKey
		{
			set
			{
				m_strReplicaKey = value;				
			}
		}

		/// <summary>
		/// Property to set the device feature list file name
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/07/08 mrj 1.00.00		Created (OpenWay Tools project)
		//  
		public string DeviceFeatureFileName
		{
			set
			{
				m_strDeviceFeatureFile = value;			
			}
		}

		/// <summary>
		/// Property to set the device menu xml file name
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/07/08 mrj 1.00.00		Created (OpenWay Tools project)
		//  
		public string DeviceMenuFileName
		{
			set
			{
				m_strDeviceMenuFile = value;
			}
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds a feature node to the layout tree
        /// </summary>
        /// <param name="FeatureNode">Feature node</param>
        /// <param name="LayoutNode">Layout node</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 mrj 8.00.00		Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TreeNode.set_Text(System.String)")]
        private void AddFeature(TreeNode FeatureNode, TreeNode LayoutNode)
        {
			bool bChangesMade = false;

            LayoutTreeView.BeginUpdate();

            if (null == LayoutNode.Parent)
            {
                //This is the main menu so add to the open function key
                //This is a menu, find an open function key for this node
				for (int iIndex = 0; iIndex < LayoutNode.Nodes.Count; iIndex++)
                {
                    if (null == LayoutNode.Nodes[iIndex].Tag)
                    {
                        //We found an open function key															
                        LayoutNode.Nodes[iIndex].Text = "F" + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " - " + FeatureNode.Text;
						LayoutNode.Nodes[iIndex].Tag = new DeviceFeature(((DeviceFeature)FeatureNode.Tag).KeyText, ((DeviceFeature)FeatureNode.Tag).HandlerID, "");
						((DeviceFeature)LayoutNode.Nodes[iIndex].Tag).Children = ((DeviceFeature)FeatureNode.Tag).Children;
						LayoutNode.Nodes[iIndex].ForeColor = SystemColors.WindowText;

						bChangesMade = true;
                        break;
                    }
                }
            }
            else if (null == LayoutNode.Tag)
            {
                //This is a blank function key so add the new feature here					
                LayoutNode.Text = "F" + (LayoutNode.Index + 1).ToString(CultureInfo.InvariantCulture) + " - " + FeatureNode.Text;
                LayoutNode.Tag = new DeviceFeature(((DeviceFeature)FeatureNode.Tag).KeyText, ((DeviceFeature)FeatureNode.Tag).HandlerID, "");
				((DeviceFeature)LayoutNode.Tag).Children = ((DeviceFeature)FeatureNode.Tag).Children;
				LayoutNode.ForeColor = SystemColors.WindowText;

				bChangesMade = true;
            }
            else
            {
                //This is not a blank so we need to check to see if it is a feature or a menu
                DeviceFeature DevFeature = (DeviceFeature)LayoutNode.Tag;

                if (0 == DevFeature.HandlerID)
                {
                    //This is a menu, find an open function key for this node
					for (int iIndex = 0; iIndex < LayoutNode.Nodes.Count; iIndex++)
                    {
                        if (null == LayoutNode.Nodes[iIndex].Tag)
                        {							
							//Expand the node 
							LayoutNode.Expand();						

                            //We found an open function key															
                            LayoutNode.Nodes[iIndex].Text = "F" + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " - " + FeatureNode.Text;
							LayoutNode.Nodes[iIndex].Tag = new DeviceFeature(((DeviceFeature)FeatureNode.Tag).KeyText, ((DeviceFeature)FeatureNode.Tag).HandlerID, "");
							((DeviceFeature)LayoutNode.Nodes[iIndex].Tag).Children = ((DeviceFeature)FeatureNode.Tag).Children;
							LayoutNode.Nodes[iIndex].ForeColor = SystemColors.WindowText;

							bChangesMade = true;
                            break;
                        }
                    }
                }
                else
                {
                    //This is a node so replace it

					//Remove the feature selected indicator (i.e. bold)
					RemoveSelectedFeatureIndicator(LayoutNode.Text.Substring(5));

					//Set the new layout node
                    LayoutNode.Text = "F" + (LayoutNode.Index + 1).ToString(CultureInfo.InvariantCulture) + " - " + FeatureNode.Text;
					LayoutNode.Tag = new DeviceFeature(((DeviceFeature)FeatureNode.Tag).KeyText, ((DeviceFeature)FeatureNode.Tag).HandlerID, "");
					((DeviceFeature)LayoutNode.Tag).Children = ((DeviceFeature)FeatureNode.Tag).Children;
					LayoutNode.ForeColor = SystemColors.WindowText;				

					bChangesMade = true;
                }
            }

            LayoutTreeView.EndUpdate();

            //Update the buttons
            UpdateButtons(LayoutTreeView.SelectedNode);


			if (bChangesMade)
			{
				//Changes have been made so fire the event.
				LayoutChanged();

								
				//Update the feature list to show the user that they have selected a feature				
				FeaturesTreeView.BeginUpdate();
				FontStyle newfontstyle = FontStyle.Bold;
				Font newfont = new Font(FeaturesTreeView.Font.FontFamily, FeaturesTreeView.Font.Size, newfontstyle);
				FeatureNode.NodeFont = newfont;
				FeaturesTreeView.EndUpdate();

				//Add the feature to the selected list
				m_strSelectedFeatureList.Add(FeatureNode.Text);
			}
        }//AddFeature

        /// <summary>
        /// Handler for the add button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //
        private void btnAdd_Click(object sender, EventArgs e)
        {			
			if (null != FeaturesTreeView.SelectedNode &&
				null != LayoutTreeView.SelectedNode &&
				null != FeaturesTreeView.SelectedNode.Tag)
			{
				AddFeature(FeaturesTreeView.SelectedNode, LayoutTreeView.SelectedNode);
			}			
        }//btnAdd_Click

        /// <summary>
        /// Handler for the Add menu button press.  Users can only add menus to 
        /// empty function keys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 mrj 8.00.00		Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TreeNode.set_Text(System.String)")]
        private void btnAddMenu_Click(object sender, EventArgs e)
        {
            if (null != LayoutTreeView.SelectedNode &&
                null == LayoutTreeView.SelectedNode.Tag)
            {
                frmFieldProAddMenu MenuForm = null;

                try
                {
                    //This is an empty function key so add the menu
                    MenuForm = new frmFieldProAddMenu(true);
                    if (DialogResult.OK == MenuForm.ShowDialog())
                    {
                        LayoutTreeView.BeginUpdate();

                        //Add the new group node					
                        LayoutTreeView.SelectedNode.Text = "F" + (LayoutTreeView.SelectedNode.Index + 1).ToString(CultureInfo.InvariantCulture) +
                                                           " - " + MenuForm.MenuTitle;
                        LayoutTreeView.SelectedNode.Tag = new DeviceFeature(MenuForm.MenuTitle, 0, "");
                        LayoutTreeView.SelectedNode.ForeColor = SystemColors.WindowText;

                        //Add the blank nodes				
                        for (int iBlankIndex = 0; iBlankIndex < MAX_MENU_FUNCTION_KEYS; iBlankIndex++)
                        {
                            string sName = "F" + (iBlankIndex + 1).ToString(CultureInfo.InvariantCulture) + " - " + Properties.Resources.EMPTY;

                            TreeNode BlankNode = new TreeNode();
                            BlankNode.Tag = null;
                            BlankNode.Text = sName;
                            BlankNode.ForeColor = SystemColors.InactiveCaptionText;

                            LayoutTreeView.SelectedNode.Nodes.Add(BlankNode);
                        }

                        LayoutTreeView.EndUpdate();

                        //Update the buttons
                        UpdateButtons(LayoutTreeView.SelectedNode);

                        //Changes have been made so fire the event.
                        LayoutChanged();
                    }
                }
                finally
                {
                    MenuForm.Close();
                }
            }
        }//btnAddMenu_Click

        /// <summary>
        /// Handler for the clear button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //
        private void btnClear_Click(object sender, EventArgs e)
        {
			DeleteNode();
        }//btnClear_Click

        /// <summary>
        /// Handler for the delete menu button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //
        private void btnDeleteMenu_Click(object sender, EventArgs e)
        {
			if (null != LayoutTreeView.SelectedNode &&
				null != LayoutTreeView.SelectedNode.Parent &&
				null != LayoutTreeView.SelectedNode.Tag)
			{
				DeviceFeature DevFeature = (DeviceFeature)LayoutTreeView.SelectedNode.Tag;

				if (0 == DevFeature.HandlerID)
				{			
					//This is a menu so remove the child nodes and the menu node
					for (int iIndex = 0; iIndex < LayoutTreeView.SelectedNode.Nodes.Count; iIndex++)
					{						
						//Remove the feature selected indicator (i.e. bold)
						RemoveMenuSelectedFeature(LayoutTreeView.SelectedNode.Nodes[iIndex]);

						LayoutTreeView.SelectedNode.Nodes[iIndex].Tag = null;						
					}

					//Remove the menu node
					DeleteNode();

					//Update the buttons
					UpdateButtons(LayoutTreeView.SelectedNode);
				}
			}
        }//btnDeleteGroup_Click

		/// <summary>
		/// Recursively goes through the nodes and remove the selected feature
		/// indicators from the feature list.
		/// </summary>
		/// <param name="node"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/14/06 mrj 8.00.00		Created
		//
		private void RemoveMenuSelectedFeature(TreeNode node)
		{
			if (node.Nodes.Count > 0)
			{
				//This node is a menu so we need to remove the feature
				//selected indicator for each node
				for (int iIndex = 0; iIndex < node.Nodes.Count; iIndex++)
				{
					RemoveMenuSelectedFeature(node.Nodes[iIndex]);
				}
			}
			else
			{
				//Remove the feature selected indicator (i.e. bold)
				RemoveSelectedFeatureIndicator(node.Text.Substring(5));
			}
		}

        /// <summary>
        /// Handler for the edit menu button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TreeNode.set_Text(System.String)")]
        private void btnEditMenu_Click(object sender, EventArgs e)
        {
			if (null != LayoutTreeView.SelectedNode &&
				null != LayoutTreeView.SelectedNode.Tag)
			{
				DeviceFeature DevFeature = (DeviceFeature)LayoutTreeView.SelectedNode.Tag;

				if (0 == DevFeature.HandlerID)
				{
                    frmFieldProAddMenu MenuForm = null;

                    try
                    {
                        //This is a menu so put up a dialog to let the user change the title
                        MenuForm = new frmFieldProAddMenu(false);
                        MenuForm.MenuTitle = LayoutTreeView.SelectedNode.Text.Substring(5);

                        if (DialogResult.OK == MenuForm.ShowDialog())
                        {
                            //Change the menu's title
                            LayoutTreeView.SelectedNode.Text = "F" + (LayoutTreeView.SelectedNode.Index + 1).ToString(CultureInfo.InvariantCulture) +
                                                               " - " + MenuForm.MenuTitle;

                            //Changes have been made so fire the event.
                            LayoutChanged();
                        }
                    }
                    finally
                    {
                        MenuForm.Close();
                    }
				}
			}
        }//btnEditGroup_Click

        /// <summary>
        /// Handler for the move down button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TreeNode.set_Text(System.String)")]
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
			if (null != LayoutTreeView.SelectedNode &&
				null != LayoutTreeView.SelectedNode.Tag)
			{
				TreeNode ParentNode = LayoutTreeView.SelectedNode.Parent;
				int iNodeIndex = LayoutTreeView.SelectedNode.Index;
				TreeNode Node = ParentNode.Nodes[iNodeIndex];
				TreeNode NextNode = ParentNode.Nodes[iNodeIndex + 1];

				if (null != Node && null != NextNode)
				{
					LayoutTreeView.BeginUpdate();

					//Update the node names (index is 0 based and function keys are 1 based
                    Node.Text = "F" + (iNodeIndex + 2).ToString(CultureInfo.InvariantCulture) + " - " + Node.Text.Substring(5);
                    NextNode.Text = "F" + (iNodeIndex + 1).ToString(CultureInfo.InvariantCulture) + " - " + NextNode.Text.Substring(5);

					//Swap the nodes
					ParentNode.Nodes.RemoveAt(iNodeIndex + 1);								
					ParentNode.Nodes.RemoveAt(iNodeIndex);
					ParentNode.Nodes.Insert(iNodeIndex, NextNode);
					ParentNode.Nodes.Insert(iNodeIndex + 1, Node);

					LayoutTreeView.SelectedNode = Node;
					LayoutTreeView.EndUpdate();

					//Update the buttons
					UpdateButtons(LayoutTreeView.SelectedNode);

					//Changes have been made so fire the event.
					LayoutChanged();
				}
			}			
        }//btnMoveDown_Click       

        /// <summary>
        /// Handler for the move up button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TreeNode.set_Text(System.String)")]
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (null != LayoutTreeView.SelectedNode &&
                null != LayoutTreeView.SelectedNode.Tag)
            {
                TreeNode ParentNode = LayoutTreeView.SelectedNode.Parent;
                int iNodeIndex = LayoutTreeView.SelectedNode.Index;
                TreeNode Node = ParentNode.Nodes[iNodeIndex];
                TreeNode PrevNode = ParentNode.Nodes[iNodeIndex - 1];

                if (null != Node && null != PrevNode)
                {
					LayoutTreeView.BeginUpdate();

					//Update the node names (index is 0 based and function keys are 1 based
                    Node.Text = "F" + iNodeIndex.ToString(CultureInfo.InvariantCulture) + " - " + Node.Text.Substring(5);
                    PrevNode.Text = "F" + (iNodeIndex + 1).ToString(CultureInfo.InvariantCulture) + " - " + PrevNode.Text.Substring(5);

                    //Swap the nodes
                    ParentNode.Nodes.RemoveAt(iNodeIndex);
                    ParentNode.Nodes.RemoveAt(iNodeIndex - 1);                    
                    ParentNode.Nodes.Insert(iNodeIndex - 1, Node);
					ParentNode.Nodes.Insert(iNodeIndex, PrevNode);

                    LayoutTreeView.SelectedNode = Node;
                    LayoutTreeView.EndUpdate();

                    //Update the buttons
                    UpdateButtons(LayoutTreeView.SelectedNode);

					//Changes have been made so fire the event.
					LayoutChanged();
                }
            }
        }//btnMoveUp_Click
		  

		/// <summary>
        /// Handler for the load event.  Loads the features and the layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //  11/30/06 rdb 8.00.00        Added functionality to load the menu structure
        //                              from the XML file on load
        private void ctrlFieldProDynamicOptions_Load(object sender, EventArgs e)
        {
            LoadFeatures();
			LoadLayout();
						
			//Set the initial tool tip description
			ToolTipEventArgs toolTipEventArgs = new ToolTipEventArgs();
			toolTipEventArgs.ToolTipTitle = Properties.Resources.DYNAMIC_OPTIONS;
			toolTipEventArgs.ToolTipHelp = Properties.Resources.DYNAMIC_UI_DESCRIPTION;
			OnToolTipHelpEvent(toolTipEventArgs);			
        }//ctrlFieldProDynamicOptions_Load

		/// <summary>
		/// Fires the ToolTipHelpEvent
		/// </summary>
		/// <param name="e"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/07/08 mrj 1.00.00		Created (OpenWay Tools project)
		//  
		private void OnToolTipHelpEvent(ToolTipEventArgs e)
		{
			if (ToolTipHelpEvent != null)
			{
				ToolTipHelpEvent(this, e);
			}
		}

        /// <summary>
        /// Deletes the selected node
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 mrj 8.00.00		Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TreeNode.set_Text(System.String)")]
        private void DeleteNode()
        {
            LayoutTreeView.BeginUpdate();

            if (null != LayoutTreeView.SelectedNode &&
                null != LayoutTreeView.SelectedNode.Parent &&
                null != LayoutTreeView.SelectedNode.Tag)
            {
                DeviceFeature DevFeature = (DeviceFeature)LayoutTreeView.SelectedNode.Tag;

                if (0 == DevFeature.HandlerID)
                {
                    //This is a menu node so only delete if empty
                    bool bEmpty = true;
					for (int iIndex = 0; iIndex < LayoutTreeView.SelectedNode.Nodes.Count; iIndex++)
                    {
                        if (null != LayoutTreeView.SelectedNode.Nodes[iIndex].Tag)
                        {
                            bEmpty = false;
                            break;
                        }
                    }

                    if (bEmpty)
                    {
                        //The menu is empty
                        LayoutTreeView.SelectedNode.Nodes.Clear();

                        //Change the group node to an empty node 
                        int iKeyIndex = LayoutTreeView.SelectedNode.Index + 1;
                        LayoutTreeView.SelectedNode.Text = "F" + (LayoutTreeView.SelectedNode.Index + 1).ToString(CultureInfo.InvariantCulture) + " - " + Properties.Resources.EMPTY;
                        LayoutTreeView.SelectedNode.Tag = null;
                        LayoutTreeView.SelectedNode.ForeColor = SystemColors.InactiveCaptionText;
                    }
                }
                else
                {					
					//Remove the feature selected indicator (i.e. bold)
					RemoveSelectedFeatureIndicator(LayoutTreeView.SelectedNode.Text.Substring(5));
					
					
                    //This is not a menu node or an empty node so delete it
                    int iKeyIndex = LayoutTreeView.SelectedNode.Index + 1;
                    LayoutTreeView.SelectedNode.Text = "F" + (LayoutTreeView.SelectedNode.Index + 1).ToString(CultureInfo.InvariantCulture) + " - " + Properties.Resources.EMPTY;
                    LayoutTreeView.SelectedNode.Tag = null;
                    LayoutTreeView.SelectedNode.ForeColor = SystemColors.InactiveCaptionText;
                }

				//Changes have been made so fire the event.
				LayoutChanged();
            }

            LayoutTreeView.EndUpdate();

            //Update the buttons
            UpdateButtons(LayoutTreeView.SelectedNode);
        }//DeleteNode

        /// <summary>
        /// Handles the after select event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/04/06 mrj 8.00.00		Created
		//
        private void FeaturesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (null != LayoutTreeView.SelectedNode)
            {
                UpdateButtons(LayoutTreeView.SelectedNode);
            }

			//Update the help on the bottom of the screen
			UpdateToolTipHelp();
        }//FeaturesTreeView_AfterSelect

        /// <summary>
		/// Handles dragging an item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/29/06 mrj 8.00.00		Created
		//
		private void FeaturesTreeView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			DoDragDrop(e.Item, DragDropEffects.Move);
        }//FeaturesTreeView_ItemDrag

        /// <summary>
        /// Handles the after select event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //
        private void LayoutTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			UpdateButtons(e.Node);
        }//LayoutTreeView_AfterSelect

        /// <summary>
        /// Handles drag drop event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/29/06 mrj 8.00.00		Created
        //
        private void LayoutTreeView_DragDrop(object sender, DragEventArgs e)
        {
			if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
			{
				Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
				TreeNode LayoutNode = ((TreeView)sender).GetNodeAt(pt);
				TreeNode FeatureNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

				if (null != LayoutNode &&
					null != FeatureNode.Tag &&
					LayoutNode.TreeView != FeatureNode.TreeView)
				{
					//Add the new feature
					AddFeature(FeatureNode, LayoutNode);	
				}
			}
        }//LayoutTreeView_DragDrop

		/// <summary>
		/// Handles drag enter event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/29/06 mrj 8.00.00		Created
		//
		private void LayoutTreeView_DragEnter(object sender, DragEventArgs e)
		{			
			if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
			{			
				TreeNode FeatureNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

				if (null != FeatureNode.Tag)					
				{
					e.Effect = DragDropEffects.Move;
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}		
        }//LayoutTreeView_DragEnter

		/// <summary>
		/// Handles the keydown event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/29/06 mrj 8.00.00		Created
		//
		private void LayoutTreeView_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Delete:
				{
					DeleteNode();

					e.Handled = true;
					break;
				}
			}
        }//LayoutTreeView_KeyDown

        /// <summary>
        /// Load the features that can be put in the menu layout into the left-hand
        /// tree layout control
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 rdb 8.00.00		Created
        private void LoadFeatures()
        {
			FeaturesTreeView.BeginUpdate();

            //load the device features file into an xml document object
            XmlDocument xmldomFeatures = new XmlDocument();
			xmldomFeatures.Load(CRegistryHelper.GetFilePath(m_strReplicaKey) + m_strDeviceFeatureFile);
                
            //get a list of all xml nodes that represent feature groups
            XmlNodeList lnodesGroups = xmldomFeatures.GetElementsByTagName(GROUP);

            //for each group...
            foreach (XmlNode group in lnodesGroups)
            {
                //add a tree node for the group
				TreeNode treeNode = new TreeNode(
                    group.Attributes[FEATURE_NAME_ATTRIBUTE].Value);
                FeaturesTreeView.Nodes.Add(treeNode);

                //for each of the group's child features...
                foreach (XmlNode node in group.ChildNodes)
                {
                    //add a tree node for the feature under the group
					TreeNode tNode = new TreeNode(
                        node.Attributes[FEATURE_NAME_ATTRIBUTE].Value);
					XmlNode FeatureDescription = node.SelectSingleNode(FEATURE_DESC_TAG);
                    DeviceFeature feature = new DeviceFeature(tNode.Text,
                        Int16.Parse(node.Attributes[FEATURE_HANDLER_ID_ATTRIBUTE].Value, CultureInfo.InvariantCulture),
                        FeatureDescription.InnerText);

                    //use two stacks to store the entire subtree below a feature
                    //in that feature's tree node's tag
                    Stack<XmlNode> xmlStack = new Stack<XmlNode>();
                    Stack<DeviceFeature> featureStack = new Stack<DeviceFeature>();

                    xmlStack.Push(node);
                    featureStack.Push(feature);

                    //while there are still nodes on the stack that have not been 
                    //processed...
                    while (xmlStack.Count > 0)
                    {
                        //get the next node
                        XmlNode curNode = xmlStack.Pop();
                        DeviceFeature curFeature = featureStack.Pop();

                        //build the tree under the current node so it can be put in the
                        //tag of the tree node representing the feature
                        foreach(XmlNode childNode in curNode.ChildNodes)						
                        {						
							if (CONFIRMATION_TAG == childNode.Name)
							{						
								xmlStack.Push(childNode);

								DeviceFeature newFeature = new DeviceFeature(
									childNode.Attributes[FEATURE_NAME_ATTRIBUTE].Value,
									Int16.Parse(childNode.Attributes[
                                    FEATURE_HANDLER_ID_ATTRIBUTE].Value, CultureInfo.InvariantCulture), "");

								featureStack.Push(newFeature);
								curFeature.Children.Add(newFeature);							
							}							
                        }
                    }
                    tNode.Tag = feature;
                    treeNode.Nodes.Add(tNode);				
                }

				FeaturesTreeView.EndUpdate();
            }			
        }//LoadFeatures
		
		/// <summary>
		/// Enables/Disables the buttons based on the state of the tree controls.
		/// </summary>
		/// <param name="LayoutNode">The current node that is selected in the layout tree view</param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/04/06 mrj 8.00.00		Created
        //  05/06/09 AF  2.20.03        Now allow menus to be 4 levels deep
		//
		private void UpdateButtons(TreeNode LayoutNode)
		{
			if (null == LayoutNode.Parent)
			{
				//This is the main menu node
				btnAddMenu.Enabled = false;
				btnEditMenu.Enabled = false;
				btnDeleteMenu.Enabled = false;

				btnAdd.Enabled = false;
				for (int iIndex = 0; iIndex < LayoutNode.Nodes.Count; iIndex++)
				{
					if (null == LayoutNode.Nodes[iIndex].Tag)
					{
						if (null != FeaturesTreeView.SelectedNode &&
							null != FeaturesTreeView.SelectedNode.Tag)
						{
							//Only allow adding if there is an empty function
							//key and a feature is selected
							btnAdd.Enabled = true;
						}
					}				
				}

				btnClear.Enabled = false;
				btnMoveUp.Enabled = false;
				btnMoveDown.Enabled = false;
			}
			else if (null == LayoutNode.Tag)
			{
				//This is a blank function key
				if (null != LayoutNode.Parent &&
					null != LayoutNode.Parent.Parent &&
					null != LayoutNode.Parent.Parent.Parent &&
                    null != LayoutNode.Parent.Parent.Parent.Parent)
				{
					//Do not allow more than four levels deep
					btnAddMenu.Enabled = false;
				}
				else
				{
					btnAddMenu.Enabled = true;
				}

				btnEditMenu.Enabled = false;
				btnDeleteMenu.Enabled = false;

				if (null != FeaturesTreeView.SelectedNode &&
					null != FeaturesTreeView.SelectedNode.Tag)
				{
					btnAdd.Enabled = true;
				}
				else
				{
					btnAdd.Enabled = false;
				}

				btnClear.Enabled = false;
				btnMoveUp.Enabled = false;
				btnMoveDown.Enabled = false;
			}
			else
			{
				//Need to check to see if this is a menu or a function key
				DeviceFeature DevFeature = (DeviceFeature)LayoutNode.Tag;

				if (0 == DevFeature.HandlerID)
				{
					//This is a menu
					btnAddMenu.Enabled = false;
					btnEditMenu.Enabled = true;
					btnDeleteMenu.Enabled = true;

					btnAdd.Enabled = false;
					btnClear.Enabled = true;
					for (int iIndex = 0; iIndex < LayoutNode.Nodes.Count; iIndex++)
					{
						if (null == LayoutNode.Nodes[iIndex].Tag)
						{
							if (null != FeaturesTreeView.SelectedNode &&
								null != FeaturesTreeView.SelectedNode.Tag)
							{
								//Only allow adding if there is an empty function
								//key and a feature is selected
								btnAdd.Enabled = true;
							}
						}
						else
						{
							//Only allow clearing of an empty menu
							btnClear.Enabled = false;
						}
					}

					if (0 == LayoutNode.Index)
					{
						btnMoveUp.Enabled = false;
					}
					else
					{
						btnMoveUp.Enabled = true;
					}

					if ((MAX_MENU_FUNCTION_KEYS - 1) == LayoutNode.Index)
					{
						btnMoveDown.Enabled = false;
					}
					else
					{
						btnMoveDown.Enabled = true;
					}
				}
				else
				{
					//This is a function key
					btnAddMenu.Enabled = false;
					btnEditMenu.Enabled = false;
					btnDeleteMenu.Enabled = false;

					if (null != FeaturesTreeView.SelectedNode &&
						null != FeaturesTreeView.SelectedNode.Tag)
					{
						btnAdd.Enabled = true;
					}
					else
					{
						btnAdd.Enabled = false;
					}

					btnClear.Enabled = true;

					if (0 == LayoutNode.Index)
					{
						btnMoveUp.Enabled = false;
					}
					else
					{
						btnMoveUp.Enabled = true;
					}

					if ((MAX_MENU_FUNCTION_KEYS - 1) == LayoutNode.Index)
					{
						btnMoveDown.Enabled = false;
					}
					else
					{
						btnMoveDown.Enabled = true;
					}
				}
			}
        }//UpdateButtons
		
		/// <summary>
		/// Fires the ChangeMadeEvent and set a flag to let the application know
		/// that it has been changed.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/05/06 mrj 8.00.00		Created
		//
		private void LayoutChanged()
		{
			if (ChangeMadeEvent != null)
			{
				//Changes have been made so fire the event.
				ChangeMadeEvent();
			}

			m_bChangesMade = true;
		}

		/// <summary>
		/// Fires the ToolTipHelp event to update the help at the bottom of System
		/// Manager.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/06/06 mrj 8.00.00		Created
		//
		private void UpdateToolTipHelp()
		{
			ToolTipEventArgs toolTipEventArgs = new ToolTipEventArgs();
			toolTipEventArgs.ToolTipTitle = Properties.Resources.DYNAMIC_OPTIONS;
			toolTipEventArgs.ToolTipHelp = Properties.Resources.DYNAMIC_UI_DESCRIPTION;

			if( null != FeaturesTreeView.SelectedNode )
			{
				if( null != FeaturesTreeView.SelectedNode.Tag )
				{
					DeviceFeature DevFeature = (DeviceFeature)FeaturesTreeView.SelectedNode.Tag;
					toolTipEventArgs.ToolTipTitle = DevFeature.KeyText;
					toolTipEventArgs.ToolTipHelp = DevFeature.KeyDescription;					
				}

				//Fire the event
				OnToolTipHelpEvent(toolTipEventArgs);
			}			
		}

		/// <summary>
		/// Fires the ShowHelpEvent
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="hlpevent"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/06/06 mrj 8.00.00		Created
		//
		private void ctrlFieldProDynamicOptions_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			if (ShowHelpEvent != null)
			{
				ShowHelpEvent();
			}
		}

		/// <summary>
		/// Removes the selected feature indicator (bold text) from the FeatureTreeView.
		/// </summary>
		/// <param name="strFeature">The feature to remove</param>
		/// <remarks>
		/// If the feature has been selected multiple times then it will be removed
		/// from the list but it will remain bold in the FeatureTreeView.
		/// </remarks>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/13/06 mrj 8.00.00		Created
		//
		private void RemoveSelectedFeatureIndicator(string strFeature)
		{
			if (Properties.Resources.EMPTY != strFeature)
			{
				//Remove the feature from the selected list
				m_strSelectedFeatureList.Remove(strFeature);

				//Check to see if the feature is still in the list (had been 
				//selected more than once)
				if (!m_strSelectedFeatureList.Contains(strFeature))
				{
					//Remove the bold text, need to find the correct feature and then un-bold it.					
					FeaturesTreeView.BeginUpdate();

					foreach (TreeNode GroupNode in FeaturesTreeView.Nodes)
					{
						bool bFound = false;
						
						foreach (TreeNode FeatureNode in GroupNode.Nodes)
						{
							if (strFeature == FeatureNode.Text)
							{
								//Set the node font to null, which sets it back to
								//the default, not bold								
								FeatureNode.NodeFont = null;
								bFound = true;
								break;
							}
						}
						if (bFound)
						{
							break;
						}
					}

					FeaturesTreeView.EndUpdate();
				}				
			}
		}

		/// <summary>
		/// Sets the selected feature indicator (bold text) to the FeatureTreeView
		/// for the features that are in the layout
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/13/06 mrj 8.00.00		Created
		//
		private void SetSelectedFeatureIndicators()
		{
			string strSelectedFeature;


			FeaturesTreeView.BeginUpdate();

			//Loop through each feature and make them bold in the FeatureTreeView
			for (int iIndex = 0; iIndex < m_strSelectedFeatureList.Count; iIndex++)
			{
				strSelectedFeature = m_strSelectedFeatureList[iIndex];

				foreach (TreeNode GroupNode in FeaturesTreeView.Nodes)
				{
					bool bFound = false;

					foreach (TreeNode FeatureNode in GroupNode.Nodes)
					{
						if (strSelectedFeature == FeatureNode.Text)
						{
							//Set the found feature to bold
							FontStyle newfontstyle = FontStyle.Bold;
							Font newfont = new Font(FeaturesTreeView.Font.FontFamily, FeaturesTreeView.Font.Size, newfontstyle);
							FeatureNode.NodeFont = newfont;

							bFound = true;
							break;
						}
					}
					if (bFound)
					{
						break;
					}
				}
			}

			FeaturesTreeView.EndUpdate();
		}

		/// <summary>
		/// Clears the selected feature list and sets features to not be bold
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/13/06 mrj 8.00.00		Created
		//
		private void ClearSelectedFeatureList()
		{			
			FeaturesTreeView.BeginUpdate();

			foreach (TreeNode GroupNode in FeaturesTreeView.Nodes)
			{
				foreach (TreeNode FeatureNode in GroupNode.Nodes)
				{
					//Set the node font to null, which sets it back to
					//the default, not bold								
					FeatureNode.NodeFont = null;
				}				
			}

			FeaturesTreeView.EndUpdate();

			//Clear the selected list
			m_strSelectedFeatureList.Clear();		
		}

		/// <summary>
		/// Clears the ChildMenuIDs from the node tags of every layout
		/// node.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/13/06 mrj 8.00.00		Created
		//
		private void ClearChildMenuIDs(TreeNode curNode)
		{
			if (curNode.Nodes.Count > 0)
			{
				for (int iIndex = 0; iIndex < curNode.Nodes.Count; iIndex++)
				{
					//Recursively clear child menu IDs
					ClearChildMenuIDs(curNode.Nodes[iIndex]);
				}

				if (null != curNode.Tag)
				{
					//Clear the ChildMenuID
					((DeviceFeature)curNode.Tag).ChildMenuID = 0;
				}
			}
			else
			{
				if (null != curNode.Tag)
				{
					//Clear the ChildMenuID
					((DeviceFeature)curNode.Tag).ChildMenuID = 0;
				}
			}
		}

		#endregion

		#region Members

		/// <summary>
		/// The delegate for the ShowHelp event handler.
		/// </summary>
		public delegate void ShowHelp();
		/// <summary>
		/// ShowHelpEvent event fired if the user hits F1
		/// </summary>
		public event ShowHelp ShowHelpEvent;

		/// <summary>
		/// System Manager public delegate
		/// </summary>
		public delegate void ChangeMade();
		/// <summary>
		/// The ChangeMade event is fired if the user makes changes to the layout.
		/// </summary>
		public event ChangeMade ChangeMadeEvent;

		/// <summary>
		/// Delegate for the ToolTipHelp event handler.
		/// </summary>
		public delegate void ToolTipHelp(object sender, ToolTipEventArgs e);
		/// <summary>
		/// ToolTipHelpEvent is fired when features are selected
		/// </summary>
		public event ToolTipHelp ToolTipHelpEvent;
		
		private UIDefinitionFile m_UIDefinitionFile;
		private bool m_bChangesMade;

		StringCollection m_strSelectedFeatureList;

		private string m_strReplicaKey;
		private string m_strDeviceFeatureFile;
		private string m_strDeviceMenuFile;

		#endregion

	}//ctrlFieldProDynamicOptions


	/// <summary>
	/// Object that represents a device feature in Field-Pro
	/// </summary>
	public class DeviceFeature
	{
		#region Public Methods

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="strKeyText"></param>
		/// <param name="iHandlerID"></param>
		/// <param name="strKeyDesc"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/04/06 rdb 8.00.00		Created
		//
		public DeviceFeature(string strKeyText, int iHandlerID, string strKeyDesc)
		{
			m_strKeyText = strKeyText;
			m_iHandlerID = iHandlerID;
			m_lChildFeatures = new List<DeviceFeature>();
			m_strKeyDescription = strKeyDesc;

			//Set the ChildMenuID to 0.  This is only used when writing the xml file.
			m_iChildMenuID = 0;			
		}//DeviceFeature

		#endregion

		#region Public Properties

		/// <summary>
		/// List of child features
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/04/06 rdb 8.00.00		Created
		//
		public List<DeviceFeature> Children
		{
			get
			{
				return m_lChildFeatures;
			}
			set
			{
				m_lChildFeatures = value;
			}
		}//Children

		/// <summary>
		/// The text for the feature
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/04/06 rdb 8.00.00		Created
		//
		public string KeyText
		{
			get
			{
				return m_strKeyText;
			}
		}//KeyText

		/// <summary>
		/// The handler ID for the feature
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/04/06 rdb 8.00.00		Created
		//
		public int HandlerID
		{
			get
			{
				return m_iHandlerID;
			}
		}//HandlerID

		/// <summary>
		/// Gets the key's description
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/06/06 mrj 8.00.00		Created
		//
		public string KeyDescription
		{
			get
			{
				return m_strKeyDescription;
			}
		}
				
		/// <summary>
		/// The child menu ID is only used when writing the XML file.  It is used to
		/// keep track of what the child menu ID will be for the given features child
		/// screens.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/13/06 mrj 8.00.00		Created
		//
		public int ChildMenuID
		{
			get
			{
				return m_iChildMenuID;
			}
			set
			{
				m_iChildMenuID = value;
			}
		}		

		#endregion

		#region Members

		private string m_strKeyText;
		private int m_iHandlerID;
		private List<DeviceFeature> m_lChildFeatures;
		private string m_strKeyDescription;	
		private int m_iChildMenuID;
		
		#endregion

	}//DeviceFeature


	/// <summary>
	/// Event arguments for a progress event
	/// </summary>
	public class ToolTipEventArgs : EventArgs
	{

		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/06/06 mrj 8.00.00		Created
		//
		public ToolTipEventArgs()
			: base()
		{
			m_strToolTipTitle = null;
			m_strToolTip = null;
		}//ToolTipEventArgs

		/// <summary>
		/// Constructor that sets the status
		/// </summary>
		/// <param name="strToolTipTitle">title</param>
		/// <param name="strToolTip">tool tip</param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/06/06 mrj 8.00.00		Created
		//
		public ToolTipEventArgs(string strToolTipTitle, string strToolTip)
			: base()
		{
			m_strToolTipTitle = strToolTipTitle;
			m_strToolTip = strToolTip;
		}//ToolTipEventArgs

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the tool tip title string
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/06/06 mrj 8.00.00		Created
		//
		public string ToolTipTitle
		{
			get
			{
				return m_strToolTipTitle;
			}
			set
			{
				m_strToolTipTitle = value;
			}
		}

		/// <summary>
		/// Gets or sets the tool tip string
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/06/06 mrj 8.00.00		Created
		//
		public string ToolTipHelp
		{
			get
			{
				return m_strToolTip;
			}
			set
			{
				m_strToolTip = value;
			}
		}

		#endregion

		#region Protected Members

		/// <summary>
		/// The title of the tool tip help
		/// </summary>
		protected string m_strToolTipTitle;

		/// <summary>
		/// The tool tip help that will be displayed
		/// </summary>
		protected string m_strToolTip;		
		
		#endregion

	}//ToolTipEventArgs

}//Itron.Metering.SystemManager
