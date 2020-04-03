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
//                              Copyright © 2007 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////


using System;
using System.Xml;
using Itron.Metering.Utilities;

namespace Itron.Metering.TOU
{
	/// <summary>
	/// This class holds all of the holiday lists and supports the
	/// addition, deletion, and modification of individual lists
	/// and holidays.  All holidays are stored in this class and
    /// are accessed through selecting a 'current' list by it's ID
    /// or name and having the methods to add, delete, and modify 
    /// holidays apply to the current list.
	/// </summary>
	public class CHolidayLists
    {
    #region variables

        const String TOU_SCHEDULE = "Calendar Editor";
		/// <summary>
		/// Represents the Holiday List XML file
		/// </summary>
		private XmlDocument m_xmldomColl;

		/// <summary>
		/// Holds all the holiday lists
		/// </summary>
		private XmlNodeList m_xmlnodelistLists;

		/// <summary>
		/// Represents the holiday list currently being accessed.
		/// </summary>
		private XmlNode m_xmlnodeCurrentList;

		/// <summary>
		/// Represents the holiday currently being accessed.
		/// </summary>
		private XmlNode m_xmlnodeCurrentHoliday;

		/// <summary>
		/// Represents the root of all the holiday lists
		/// </summary>
		private XmlNode m_xmlnodeRoot;


        /// <summary>
        /// Represents the next available ID a holiday or holiday list
        /// can use.  All lists and holidays must have a unique ID for
        /// compatability with previous TOU Schedules.  No list may have
        /// the same ID as another list or as a holiday within any list.
        /// </summary>
        private int m_intNextID;

    #endregion


    #region methods

        /// <summary>
		/// Creates an instance of the Holiday Lists
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
        /// 06/19/06 ach N/A     N/A    Updated ID management for compatability
        ///                             with older TOU Schedules
		public CHolidayLists()
		{
			// Local Variables
			XmlNodeList xmlnodelistTempRoot;

			m_xmldomColl = new XmlDocument( );

            String sPathName = CRegistryHelper.GetDataDirectory(TOU_SCHEDULE) + "\\HolList.dat";
			m_xmldomColl.Load(sPathName);

            // Update the Xml Format
            UpdateXmlFormat();

			// Get The Root of the all the holiday lists 
			xmlnodelistTempRoot = m_xmldomColl.GetElementsByTagName("HolidayData");
			m_xmlnodeRoot = xmlnodelistTempRoot[0];

			// Get all the Holiday Lists from the Root
			m_xmlnodelistLists = m_xmldomColl.GetElementsByTagName("HolidayList");

            // Remove all apostrophes and quotes from the xml tags 
            foreach (XmlNode list in m_xmlnodelistLists)
            {
                m_xmlnodeCurrentList = list;
                Name = Name;
            }

			// Set the first list as the current list by default
			m_xmlnodeCurrentList = m_xmlnodelistLists[0];

            // Set the next ID
            UpdateNextID();

		}//HolidayLists


        /// <summary>
        /// This class sets the current list to null to indicate that 
        /// no list is currently being accessed.  This will be used when
        /// adding a new list as no list should be accessed when a new list
        /// is being added to the collection.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/02/06 ach N/A	 N/A	Creation of Class
        public void ClearCurrentList()
        {
            m_xmlnodeCurrentList = null;
        }//ClearCurrentList


        /// <summary>
        /// This class returns whether or not a list is currently being
        /// accessed.  If no list is being accessed true is returned, if
        /// a list is being accessed false is returned.
        /// </summary>
        /// <returns>
        /// True if no list is being accessed, false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/02/06 ach N/A	 N/A	Creation of Class
        public bool NoListAccess()
        {
            return (null == m_xmlnodeCurrentList);
        }//NoListAccess


		/// <summary>
		/// This class takes a holiday list as an xml node and 
		/// adds it to the collection of holiday lists.
		/// </summary>
		/// <param name="listName">
		/// Represents the name of the new holiday list being added
		/// </param>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
		public void AddList(string listName)
		{
            listName = listName.Replace("'", "&apos");
            listName = listName.Replace("\"", "&quot");

			// Ensure no other holiday list uses the same name
			string strXPath = "child::HolidayList/Name[text() = '" + 
				listName + "']";
			if (null != m_xmlnodeRoot.SelectSingleNode(strXPath))
			{
				throw new ArgumentException("Name provided is already used", 
					"string strName");
			}

			// Use the AppendChild method to add the list
			m_xmlnodeRoot.AppendChild(m_xmldomColl.CreateElement("HolidayList"));

            // Set the Currently Accessed List to this List
			m_xmlnodeCurrentList = m_xmlnodeRoot.LastChild;

            // Create the List ID Node
            m_xmlnodeCurrentList.AppendChild(
                    m_xmldomColl.CreateElement("HolidayListID"));
            m_xmlnodeCurrentList.LastChild.InnerText = m_intNextID.ToString();

            // Update the Next ID to being one greater than this ID
            m_intNextID++;

            // Create the Name node
            m_xmlnodeCurrentList.AppendChild(
                    m_xmldomColl.CreateElement("Name"));
            m_xmlnodeCurrentList.LastChild.InnerText = listName;

			// Update the list collection to reflect addition
			m_xmlnodelistLists = m_xmldomColl.GetElementsByTagName("HolidayList");

		}//AddList

		
		/// <summary>
		/// This class take a holiday list name and
		/// deletes the list from the collection of holiday lists.  If
		/// the List to be deleted is not in the collection of lists
		/// an ArgumentException will be thrown by the call to RemoveChild.
		/// </summary>
		/// <param name="ListName">
		/// Represents the name of the holiday list to delete
		/// </param>
		/// <exception cref="ArgumentException">
		/// The list to be deleted is not in the collection of lists.
		/// </exception>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
		public void DeleteList(string ListName)
		{
            ListName = ListName.Replace("'", "&apos");
            ListName = ListName.Replace("\"", "&quot");

            // Create the XPath to find the list to be deleted
            String strXPath = "HolidayList/Name[text() = '" + ListName + "']";

            // Select The List to be deleted
            XmlNode ListToDel = m_xmlnodeRoot.SelectSingleNode(strXPath).ParentNode;

			// Use the RemoveChild method to delete the list and 
			// throw an ArgumentException if needed
			m_xmlnodeRoot.RemoveChild(ListToDel);

			// Update the List Collection to reflect deletion
			m_xmlnodelistLists = m_xmldomColl.GetElementsByTagName("HolidayList");

            // Update the next ID to be correct reflecting the removal of this list
            UpdateNextID();

		}//DeleteList


        /// <summary>
        /// This method removes all the children from the currently
        /// accessed list and adds back the given name and id.  It
        /// effectively removes all the holidays from the list so the
        /// list can be easily rebuilt.
        /// </summary>
        /// <param name="ListName">
        /// Holds the name of the list that is being emptied.
        /// </param>
        /// <param name="ListID">
        /// Holds the ID of the list that is being emptied.
        /// </param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/09/06 ach N/A	 N/A	Created method to assist in saving lists
        public void EmptyList(string ListName, int ListID)
        {
            ListName = ListName.Replace("'", "&apos");
            ListName = ListName.Replace("\"", "&quot");

            // Get the ID for this list
            int id = ID;
            
            // Remove all the children of the holiday list
            m_xmlnodeCurrentList.RemoveAll();

            // Add back the List ID node
            m_xmlnodeCurrentList.AppendChild(
                            m_xmldomColl.CreateElement("HolidayListID"));
            m_xmlnodeCurrentList.FirstChild.InnerText = id.ToString();

            // Add back the List Name node
            m_xmlnodeCurrentList.AppendChild(
                            m_xmldomColl.CreateElement("Name"));
            m_xmlnodeCurrentList.LastChild.InnerText = ListName;

            // Set the Next ID to be one greater than the list ID
            //m_intNextID = id + 1;

        }//EmptyList


		/// <summary>
		/// This class adds the holiday to the currently accessed holiday
		/// list.
		/// </summary>
		/// <param name="HolidayToAdd">
		/// Represents a Holiday type to be added to the currently accessed list.
		/// </param>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
		public void AddHoliday(CHoliday HolidayToAdd)
		{
			// Put the Holiday into an Xml Node
			XmlNode newHol = HolidayToXml(HolidayToAdd);

			// Use the AppendChild method to add the Holiday to current list
			m_xmlnodeCurrentList.AppendChild(newHol);

			m_xmlnodeCurrentHoliday = m_xmlnodeCurrentList.LastChild;

            // Increment the Next ID
            m_intNextID++;
		}//AddHoliday


		/// <summary>
		/// This method takes a Holiday Object and creates a 
		/// corresponding xml node.
		/// </summary>
		/// <param name="Holiday">
		/// Represents the Holiday who information will be entered
		/// into a xml node.
		/// </param>
		/// <returns>
		/// An xml node representing the Holiday. 
		/// </returns>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
		private XmlNode HolidayToXml(CHoliday Holiday)
		{
			// XmlNode Representation of HolidayToAdd
			XmlNode newHol = m_xmldomColl.CreateElement("Holiday");

			// Add the Holiday ID
			newHol.AppendChild(m_xmldomColl.CreateElement("HolidayID"));
			newHol.LastChild.InnerText = Holiday.ID.ToString();

			// Add the Holiday Name
			newHol.AppendChild(m_xmldomColl.CreateElement("Name"));
			newHol.LastChild.InnerText = Holiday.Name;

			// Add the Holiday Day Type
			newHol.AppendChild(m_xmldomColl.CreateElement("HolidayDayTypeIndex"));
			newHol.LastChild.InnerText = Holiday.Index.ToString();

			// Add the Strategy
			newHol.AppendChild(m_xmldomColl.CreateElement("Strategy"));
			XmlNode strat = newHol.LastChild;

			// Add the Fixed Holiday to Strategy
			strat.AppendChild(m_xmldomColl.CreateElement("FixedHoliday"));
			XmlNode fixedHol = strat.LastChild;

			// Add the Month to Fixed Holiday
			fixedHol.AppendChild(m_xmldomColl.CreateElement("Month"));
			fixedHol.LastChild.InnerText = Holiday.Date.Month.ToString();

			// Add the Day to Fixed Holiday
			fixedHol.AppendChild(m_xmldomColl.CreateElement("Day"));
			fixedHol.LastChild.InnerText = Holiday.Date.Day.ToString();

			// Add the Frequency
			newHol.AppendChild(m_xmldomColl.CreateElement("Frequency"));
			XmlNode frequency = newHol.LastChild;

			// If the Holiday occurs in a single year add to Frequency
			if (eFrequency.SINGLE == Holiday.Frequency)
			{
				frequency.AppendChild(m_xmldomColl.CreateElement("SingleYear"));
				frequency.LastChild.InnerText = Holiday.Date.Year.ToString();
			}
			// Else the holiday occurs every year add it to Frequency
			else 
			{
				frequency.AppendChild(m_xmldomColl.CreateElement("EveryYear"));
                XmlAttribute moveSat = m_xmldomColl.CreateAttribute("MoveSat");
                moveSat.Value = Holiday.MoveSaturday.ToString();
                XmlAttribute moveSun = m_xmldomColl.CreateAttribute("MoveSun");
                moveSun.Value = Holiday.MoveSunday.ToString();
                frequency.LastChild.Attributes.SetNamedItem(moveSat);
                frequency.LastChild.Attributes.SetNamedItem(moveSun);
			}

			return newHol;
		}//HolidayToXml


		/// <summary>
		/// This class takes a xml node of a holiday and converts it
		/// to a holiday type.
		/// </summary>
		/// <param name="HolNode">
		/// Represents a xml node of a holiday.
		/// </param>
		/// <returns>
		/// A holiday type that corresponds to the xml node parameter.
		/// </returns>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
		private CHoliday XmlToHoliday(XmlNode HolNode)
		{
			// Local Names
			string name;
			int id;
			int type;
			int month;
			int day;
			int year;
            eFrequency frequency;
            eMoveHoliday moveSat = eMoveHoliday.DONT;
            eMoveHoliday moveSun = eMoveHoliday.DONT;

			// Get the Holiday ID
			id = int.Parse(HolNode.FirstChild.InnerText);

			// Get The Holiday Name
			name = HolNode.FirstChild.NextSibling.InnerText;

			// Get The Holiday Day Type Index
			type = int.Parse(HolNode.FirstChild.NextSibling.NextSibling.InnerText);

			// Get The year if Holiday is a single year
			if ("SingleYear" == HolNode.LastChild.LastChild.Name)
			{
				frequency = eFrequency.SINGLE;
				year = int.Parse(HolNode.LastChild.LastChild.InnerText);
			}
			// Otherwise set the year to default to 2000
			else
			{
				frequency = eFrequency.MULTI;
				year = 2000;
                switch (HolNode.LastChild.LastChild.Attributes[0].Value)
                {
                    case "DONT":
                        moveSat = eMoveHoliday.DONT;
                        break;

                    case "FRI":
                        moveSat = eMoveHoliday.FRI;
                        break;

                    case "MON":
                        moveSat = eMoveHoliday.MON;
                        break;

                    default:
                        break;
                }

                switch (HolNode.LastChild.LastChild.Attributes[1].Value)
                {
                    case "DONT":
                        moveSun = eMoveHoliday.DONT;
                        break;

                    case "FRI":
                        moveSun = eMoveHoliday.FRI;
                        break;

                    case "MON":
                        moveSun = eMoveHoliday.MON;
                        break;

                    default:
                        break;
                }
               
			}

			// Get the month and day
			XmlNode fixedHol = HolNode.LastChild.PreviousSibling.LastChild;
			month = int.Parse(fixedHol.FirstChild.InnerText);
			day = int.Parse(fixedHol.LastChild.InnerText);

			return new CHoliday(id, name, new DateTime(year, month, day), 
                                type, frequency, moveSat, moveSun);
		}//XmlToHoliday


		/// <summary>
		/// This method selects the list located at the specified index.
		/// </summary>
		/// <param name="intIndex">
		/// The index of the list to be returned
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If the provided index is less than zero or greater than the
		/// number of lists in the collection.
		/// </exception>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
		public void ListAtIndex(int intIndex)
		{
			// Check the range of index parameter
			if (0 > intIndex || m_xmlnodelistLists.Count < intIndex)
			{
				throw new ArgumentOutOfRangeException("Index is out of range", 
					"int intIndex");
			}

			// Set the current list to the one being accessed
			m_xmlnodeCurrentList = m_xmlnodelistLists[intIndex];

		}//ListAtIndex


		/// <summary>
		/// This class selects the list by the given name.  If the name
        /// provided is not in the current list is set to null.
		/// </summary>
		/// <param name="strName">
		/// Represents the name of the list to be selected.
		/// </param>
        /// Revision History
        /// M/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/02/06 ach N/A	 N/A	Creation of Class
		public void ListByName(string strName)
		{
            
            strName = strName.Replace("'", "&apos");
            strName = strName.Replace("\"", "&quot");

			// Check to see if the provided name is a name used by a list
			string strXPath = "child::HolidayList/Name[text() = '" + strName + "']";
			if (null == m_xmlnodeRoot.SelectSingleNode(strXPath))
			{
                m_xmlnodeCurrentList = null;
                return;
			}
			
			// Set the current list to the list with matching name
			m_xmlnodeCurrentList = m_xmlnodeRoot.SelectSingleNode(strXPath).ParentNode;

		}//ListByName


        /// <summary>
        /// This method selects the list by the given ID.  If there is no
        /// list with a matching ID then current list is set to null.
        /// </summary>
        /// <param name="listID">
        /// Represents the id of the list to be selected.
        /// </param>
        /// Revision History
        /// M/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/02/06 ach N/A	 N/A	Creation of Class
        public void ListByID(int listID)
        {
            // Check to see if the provided name is a name used by a list
            string strXPath = "child::HolidayList/HolidayListID[text() = '" + 
                                listID.ToString() + "']";
            if (null == m_xmlnodeRoot.SelectSingleNode(strXPath))
            {
                m_xmlnodeCurrentList = null;
                return;
            }

            // Set the current list to the list with matching ID
            m_xmlnodeCurrentList = m_xmlnodeRoot.SelectSingleNode(strXPath).ParentNode;

        }//ListByID


		/// <summary>
		/// This class returns how many lists are in the collection.
		/// </summary>
		/// <returns>
		/// The number of lists in the collection.
		/// </returns>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
		public int Count( )
		{
			return m_xmlnodelistLists.Count;
		}//Count

        /// <summary>
        /// This class returns an array of holidays contained in the current list.
        /// Note: This array is a copy of what the list contains and thus any 
        /// changes made in the array once retrieved will not be reflected in the
        /// list unless the modify holiday method is used when a holiday is changed.
        /// </summary>
        /// <returns>
        /// An array of Holidays that the current list contains.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/02/06 ach N/A	 N/A	Creation of Class
        public CHoliday[] GetHolidays()
        {
            // Local Variables
            try
            {
                XmlNodeList holidayListNodes = m_xmlnodeCurrentList.ChildNodes;
                CHoliday[] holidayList = new CHoliday[holidayListNodes.Count - 2];
                int intCount = 0;

                // Convert each holiday from an xml node 
                // to a CHoliday and add to holidayList 
                foreach (XmlNode holiday in holidayListNodes)
                {
                    if (holiday.Name == "Holiday")
                    {
                        holidayList[intCount] = XmlToHoliday(holiday);
                        intCount++;
                    }
                }

                return holidayList;
            }
            catch
            {
                return null;
            }
        }//GetHolidays


        /// <summary>
        /// This method saves the current list to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/02/06 ach N/A	 N/A	Added save ability for lists
        public void Save()
        {
            String sPathName = CRegistryHelper.GetDataDirectory(TOU_SCHEDULE) + "\\HolList.dat";
            m_xmldomColl.Save(sPathName);
        }//Save


        /// <summary>
        /// This method resets all the IDs starting at 1 so that if any list
        /// or holiday was deleted there are no gaps between IDs.  The method
        /// will give each list and holiday a new ID so that each are unique
        /// and there will be no spaces between holidays or lists if there
        /// are deletions.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/19/06 ach N/A	 N/A    Updated ID management for compatability
        ///                             with older TOU Schedules.
        public void UpdateNextID()
        {
            int largestID = 1;

            // Select all the List ID Nodes
            XmlNodeList IDNodes = m_xmlnodeRoot.SelectNodes("HolidayList/HolidayListID");
            // Traverse all the List and Holiday ID Nodes 
            // and reset starting from a value of 1
            foreach (XmlNode ListIDNode in IDNodes)
            {
                ListIDNode.InnerText = largestID.ToString();
                largestID++;

                // Get the parent of the current List ID Node
                XmlNode currentList = ListIDNode.ParentNode;
                XmlNodeList HolidayIDs =
                            currentList.SelectNodes("Holiday/HolidayID");

                // Traverse all the Holiday ID Nodes in the current
                // list to update the ID
                foreach (XmlNode HolidayIDNode in HolidayIDs)
                {
                    HolidayIDNode.InnerText = largestID.ToString();
                    largestID++;
                }

            }

            m_intNextID = largestID;
        }//UpdateNextID


        /// <summary>
        /// This method updates the Xml format so that the node for 'EveryYear'
        /// will have attributes concerning where to move the holiday if it 
        /// falls on a Saturday or Sunday.  This method will find all the nodes
        /// of 'EveryYear' and if they do not contain attributes will create 
        /// attributes for moving Saturday and Sunday holidays to whatever the
        /// data items stored for moving Saturday and Sunday holidays exist 
        /// from the previous editor.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 ach N/A	 N/A    Updated to have individual holidays 
        ///                             store where to move weekend holidays
        private void UpdateXmlFormat()
        {

            XmlNodeList EveryYearNodes = 
                            m_xmldomColl.GetElementsByTagName("EveryYear");

            // Look through all the 'EveryYear' nodes
            foreach (XmlNode year in EveryYearNodes)
            {
                // If the node does not have any attributes create them
                if (0 == year.Attributes.Count)
                {
                    XmlAttribute moveSat;
                    XmlAttribute moveSun;

                    moveSat = m_xmldomColl.CreateAttribute("MoveSat");
                    moveSun = m_xmldomColl.CreateAttribute("MoveSun");

                    // Get the data items to find out where to move holidays
                    XmlNodeList dataItems = 
                            m_xmldomColl.GetElementsByTagName("DataItem");

                    // Set the attribute values to how data items move holidays
                    switch (int.Parse(dataItems[0].LastChild.InnerText))
                    {
                        case 1:
                            moveSat.Value = eMoveHoliday.DONT.ToString();
                            break;

                        case 2:
                            moveSat.Value = eMoveHoliday.FRI.ToString();
                            break;

                        case 3:
                            moveSat.Value = eMoveHoliday.MON.ToString();
                            break;

                        default:
                            break;
                    }

                    switch (int.Parse(dataItems[1].LastChild.InnerText))
                    {
                        case 1:
                            moveSun.Value = eMoveHoliday.DONT.ToString();
                            break;

                        case 2:
                            moveSun.Value = eMoveHoliday.FRI.ToString();
                            break;

                        case 3:
                            moveSun.Value = eMoveHoliday.MON.ToString();
                            break;

                        default:
                            break;
                    }

                    // Add the attributes
                    year.Attributes.SetNamedItem(moveSat);
                    year.Attributes.SetNamedItem(moveSun);
                }

            }
        }//UpdateXmlFormat

    #endregion 


    #region properties
        /// <summary>
		/// Property to get and set (rename) the name of the 
		/// currently accessed list.  
        /// Warning: Programmer should check to ensure no other list has
        /// the same name before using the set property as this could cause
        /// errors.
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/02/06 ach N/A	 N/A	Creation of Class
        /// 02/06/07 ach 8.0     134    Added support for quotes and appostrophes
		public string Name
		{
			get
			{
                try
                {
                    string name = m_xmlnodeCurrentList.FirstChild.NextSibling.InnerText.Replace("&apos", "'");
                    name = name.Replace("&quot", "\"");
                    return name;
                }
                catch
                {
                    return null;
                }
			}
			set
			{
                value = value.Replace("'", "&apos");
                value = value.Replace("\"", "&quot");
				m_xmlnodeCurrentList.FirstChild.NextSibling.InnerText = value;
			}
		}//Name

        /// <summary>
        /// Property to get the next ID that can be used for a holiday or 
        /// a holiday list.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/19/06 ach N/A	 N/A    Updated ID management for compatability
        ///                             with older TOU Schedules.
        public int NextID
        {
            get
            {
                return m_intNextID;
            }
        }//NextID

        /// <summary>
        /// Property to get the ID of the currently accessed list
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/09/06 ach N/A	 N/A	Added property to get ID
        public int ID
        {
            get
            {
                return int.Parse(m_xmlnodeCurrentList.FirstChild.InnerText);
            }
        }//ID

        /// <summary>
        /// Property to get the number of fixed holidays.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 ach N/A	 N/A	Added property
        public int NumberOfFixedHolidays
        {
            get
            {
                XmlNodeList fixedHolidays =                        
                        m_xmlnodeCurrentList.SelectNodes(
                            "Holiday/Frequency/SingleYear");
                return fixedHolidays.Count;
                
            }
        } // NumberOfFixedHolidays

        /// <summary>
        /// Property to get the number of recurring holidays.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 ach N/A	 N/A	Added property
        public int NumberOfRecurringHolidays
        {
            get
            {
                XmlNodeList recurringHolidays =
                        m_xmlnodeCurrentList.SelectNodes(
                            "Holiday/Frequency/EveryYear");

                return recurringHolidays.Count;
            }
        } // NumberOfRecurringHolidays

    #endregion
    }
}
