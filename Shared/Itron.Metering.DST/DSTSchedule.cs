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
using System.Xml;
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.DST
{
	/// <summary>
	/// Class represents the information contained in the DST.xml file
	/// </summary>
	public class CDSTSchedule
	{
		/// <summary>
		/// Collection of CDSTDatePair objects
		/// </summary>
		private CDSTDatePairCollection m_colDSTDatePairs;

		/// <summary>
		/// Variable used to reprsent the path of the xml file
		/// </summary>
		private string m_strFilePath;

		/// <summary>
		/// Variable used to represent xml file
		/// </summary>
		protected XmlDocument m_xmldomDST;

        private static readonly DateTime DEFAULT_DATE = new DateTime(2000, 1, 1, 0, 0, 0);

		//Private const variables
		private const string JUMP_LENGTH		= "Jump_Length";
		private const string FROM_TIME			= "From_Time";
		private const string TO_TIME			= "To_Time";
		private const string YEAR_CHANGE		= "Year_Change";
		private const string TO_DATE			= "To_Date";
		private const string FROM_DATE			= "From_Date";
		private const string YEAR				= "Year";
		private const string MONTH				= "Month";
		private const string DAY				= "Day";
		private const string REG_DST_FILE_PATH	= "DST";
        private const string REG_APP_HH_PRO = "HH-Pro";
		private const string DST_FILE			= "DST.xml";

		/// <summary>
		/// Constructor - Load the DST xml file in the XmlDocument, and begin to
		/// build the collection of DSTDatePair objects.  Will use the Registry Access
		/// Namespace's CRegistryHelper class and the GetFilePath method to obtain
		/// FilePath for the DST xml file.
		/// If there is an error with the XmlDocument then an XmlException will be
		///  thrown.  
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTSchedule objDST = new CDSTSchedule();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public CDSTSchedule()
		{
			//Get the file path for the xml file
#if (!WindowsCE)
            m_strFilePath = CRegistryHelper.GetFilePath(REG_DST_FILE_PATH);
#else
            m_strFilePath = CRegistryHelper.GetFilePath(REG_APP_HH_PRO);
#endif

			m_strFilePath = m_strFilePath + DST_FILE;

            //Create and load the XmlDocument
			m_xmldomDST = new XmlDocument();
			m_xmldomDST.Load(m_strFilePath);

			BuildDSTDatePairs();
		}

		/// <summary>
		/// Constructor - Load the DST xml file in the XmlDocument, and begin to
		/// build the collection of DSTDatePair objects. If there is an error 
        /// with the XmlDocument then an XmlException will be thrown.  
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTSchedule objDST = new CDSTSchedule();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public CDSTSchedule(string FileName)
		{
			//Get the file path for the xml file
			m_strFilePath = FileName;

			//Create and load the XmlDocument
			m_xmldomDST = new XmlDocument();
			m_xmldomDST.Load(m_strFilePath);

			BuildDSTDatePairs();
		}

		/// <summary>
		/// Builds the CDSTDatePairCollection
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/13/06 mcm 7.30.?? N/A	Creation 
		public bool FindDSTIndex(int Year, out byte DSTIndex)
		{
			bool Found = false;
			
			
			for( DSTIndex = 0; DSTIndex < m_colDSTDatePairs.Count; DSTIndex++ )
			{
				if( m_colDSTDatePairs[DSTIndex].ToDate.Year == Year )
				{
					Found = true;
					break;
				}
			}

			return Found;

		} // FindDSTIndex

		/// <summary>
		/// Builds the CDSTDatePairCollection
		/// </summary>
        /// <remarks>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class
        /// 02/28/08 AF  1.01.07        Fixed code analysis warning on int.Parse
        ///</remarks>
		private void BuildDSTDatePairs()
		{
			//Local variables
			XmlNodeList xmlnodelistDSTDatePair	= null;
			XmlNode xmlnodeToDate				= null;
			XmlNode xmlnodeFromDate				= null;
			int intYear							= 0;
			int intMonth						= 0;
			int intDay							= 0;
			DateTime dtToDate;
			DateTime dtFromDate;

			//Create the collection
			m_colDSTDatePairs = new CDSTDatePairCollection();

			//Get a list of all Year Change nodes
			xmlnodelistDSTDatePair = m_xmldomDST.GetElementsByTagName(YEAR_CHANGE);
			foreach(XmlNode xmlnodeYear in xmlnodelistDSTDatePair)
			{
				//Get the To Date values from the xml file
				xmlnodeToDate = xmlnodeYear.FirstChild;
				foreach(XmlNode xmlnodeChild in xmlnodeToDate.ChildNodes)
				{
					if(YEAR.Equals(xmlnodeChild.Name))
					{
						intYear = int.Parse(xmlnodeChild.InnerText, CultureInfo.InvariantCulture);
					}
					else if(MONTH.Equals(xmlnodeChild.Name))
					{
                        intMonth = int.Parse(xmlnodeChild.InnerText, CultureInfo.InvariantCulture);
					}
					else
					{
                        intDay = int.Parse(xmlnodeChild.InnerText, CultureInfo.InvariantCulture);
					}
				}

				//Create the To Date
				dtToDate = new DateTime(intYear,intMonth,intDay);

				//Get the From Date values from the xml file
				xmlnodeFromDate = xmlnodeYear.LastChild;
				foreach(XmlNode xmlnodeChild in xmlnodeFromDate.ChildNodes)
				{
					if(YEAR.Equals(xmlnodeChild.Name))
					{
                        intYear = int.Parse(xmlnodeChild.InnerText, CultureInfo.InvariantCulture);
					}
					else if(MONTH.Equals(xmlnodeChild.Name))
					{
                        intMonth = int.Parse(xmlnodeChild.InnerText, CultureInfo.InvariantCulture);
					}
					else
					{
                        intDay = int.Parse(xmlnodeChild.InnerText, CultureInfo.InvariantCulture);
					}
				}

				//Create the From Date
				dtFromDate = new DateTime(intYear,intMonth,intDay);

				//Add the new CDSTDatePair to the collection
				m_colDSTDatePairs.Add(new CDSTDatePair(dtToDate,dtFromDate));
			}
			
			//Sort the Collection
			m_colDSTDatePairs.Sort();

		}//BuildDSTDatePair()

		/// <summary>
		/// Property to return the FilePath of the xml file
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public string FilePath
		{
			get
			{
				return m_strFilePath;
			}
		}

		/// <summary>
		/// Property to get the CDSTDatePairCollection
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTSchedule objDST = new CDSTSchedule();
		/// CDSTDatePairCollection col = objDST.DSTDatePairs;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public CDSTDatePairCollection DSTDatePairs
		{
			get
			{
				return m_colDSTDatePairs;
			}
		}

		/// <summary>
		/// Property to get the Jump Length
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTSchedule objDST = new CDSTSchedule();
		/// int intJump = objDST.JumpLength;
		/// </code>
		/// </example>
        /// <remarks>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class 
        /// 02/28/08 AF  1.01.07        Fixed code analysis warning on int.Parse
        /// </remarks>
		public int JumpLength 
		{
			get
			{
				XmlNodeList xmlnodelistJump;

				xmlnodelistJump = m_xmldomDST.GetElementsByTagName(JUMP_LENGTH);
                return int.Parse(xmlnodelistJump[0].InnerText, CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Property to get the From Time
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTSchedule objDST = new CDSTSchedule();
		/// int intFrom = objDST.FromTime;
		/// </code>
		/// </example>
        /// <remarks>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class
        /// 02/28/08 AF  1.01.07        Fixed code analysis warning on int.Parse
        /// </remarks>
		public int FromTime
		{
			get
			{
				XmlNodeList xmlnodelistFrom;

				xmlnodelistFrom = m_xmldomDST.GetElementsByTagName(FROM_TIME);
                return int.Parse(xmlnodelistFrom[0].InnerText, CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Property to get the To Time
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTSchedule objDST = new CDSTSchedule();
		/// int intTo = objDST.ToTime;
		/// </code>
		/// </example>
        /// <remarks>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class
        /// 02/28/08 AF  1.01.07        Fixed code analysis warning on int.Parse
        /// </remarks>
		public int ToTime
		{
			get
			{
				XmlNodeList xmlnodelistTo;

				xmlnodelistTo = m_xmldomDST.GetElementsByTagName(TO_TIME);
                return int.Parse(xmlnodelistTo[0].InnerText, CultureInfo.InvariantCulture);
			}
		}

        /// <summary>
        /// Property to get the next DST from date that will occur.
        /// </summary>
        /// <remarks>If date is not found in the schedule then a default date 
        /// of 1/1/2000 12:00:00AM will be returned.</remarks>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/09 jrf 2.10.03 N/A	   Created.
        public DateTime NextDSTFromDate
        {
            get
            {
                byte bytDSTIndex;
                DateTime dtNextFromDate = DEFAULT_DATE;
                int iCurrentYear = DateTime.Now.Year;

                if (FindDSTIndex(iCurrentYear, out bytDSTIndex))
                {
                    if (m_colDSTDatePairs[bytDSTIndex].FromDate > DateTime.Now)
                    {
                        //We have our date
                        dtNextFromDate = m_colDSTDatePairs[bytDSTIndex].FromDate;
                        dtNextFromDate = dtNextFromDate.AddSeconds((double)FromTime);
                    }
                    else
                    {
                        //From date is in the past.  So try the next year.
                        if (FindDSTIndex(iCurrentYear + 1, out bytDSTIndex))
                        {
                            //Ok, now we have our date
                            dtNextFromDate = m_colDSTDatePairs[bytDSTIndex].FromDate;
                            dtNextFromDate = dtNextFromDate.AddSeconds((double)FromTime);
                        }
                    }
                }

                return dtNextFromDate;
            }
        }

        /// <summary>
        /// Property to get the next DST to date that will occur.
        /// </summary>
        /// <remarks>If date is not found in the schedule then a default date 
        /// of 1/1/2000 12:00:00AM will be returned.</remarks>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/30/09 jrf 2.10.03 N/A	   Created.
        public DateTime NextDSTToDate
        {
            get
            {
                byte bytDSTIndex;
                DateTime dtNextToDate = DEFAULT_DATE;
                int iCurrentYear = DateTime.Now.Year;

                if (FindDSTIndex(iCurrentYear, out bytDSTIndex))
                {
                    if (m_colDSTDatePairs[bytDSTIndex].ToDate > DateTime.Now)
                    {
                        //We have our date
                        dtNextToDate = m_colDSTDatePairs[bytDSTIndex].ToDate;
                        dtNextToDate = dtNextToDate.AddSeconds((double)ToTime);
                    }
                    else
                    {
                        //From date is in the past.  So try the next year.
                        if (FindDSTIndex(iCurrentYear + 1, out bytDSTIndex))
                        {
                            //Ok, now we have our date
                            dtNextToDate = m_colDSTDatePairs[bytDSTIndex].ToDate;
                            dtNextToDate = dtNextToDate.AddSeconds((double)ToTime);
                        }
                    }
                }

                return dtNextToDate;
            }
        }
	}
}
