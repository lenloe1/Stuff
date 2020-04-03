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

namespace Itron.Metering.TOU
{
	/// <summary>
	/// Class represents a season in a TOU Schedule through storing
	/// the season ID, Name, and arrays of Holidays and NormalDays.
	/// The arrays hold the patternID's to use for the particular Holidays
	/// and Normal Days.
	/// </summary>
	public class CSeason
	{
		/// <summary>
		/// Represents the Season ID for the season
		/// </summary>
		private int m_intSeasonID;

		/// <summary>
		/// Represents the name of the season
		/// </summary>
		private string m_strSeasonName;

		/// <summary>
		/// List of Pattern ID's for the normal days within the Season
        /// Note: This should be changed to a List if the project
        /// is switched to the .Net 2.0 Compact Framework.
		/// </summary>
		private Int16Collection m_aintNormalDays;

		/// <summary>
		/// List of Pattern ID's for the holidays within the Season
        /// Note: This should be changed to a List if the project
        /// is switched to the .Net 2.0 Compact Framework.
		/// </summary>
		private Int16Collection m_aintHolidays;

        /// <summary>
        /// The typical week for this season.
        /// </summary>
        private string[] m_TypicalWeek;


		/// <summary>
		/// Creates an instance of a Season object
		/// </summary>
		/// <example>
		/// <code>
		/// CSeason mySeason = new CSeason(2, "Season 1", new int[4], new int[2]);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class 
        /// 04/16/14 jrf 3.50.78 489749 Added setting typical week.
		public CSeason(int intID, string strName, Int16Collection aintNormalDays,
                            Int16Collection aintHolidays, string[] typicalWeek)
		{
			m_intSeasonID = intID;
			m_strSeasonName = strName;
			m_aintNormalDays = aintNormalDays;
			m_aintHolidays = aintHolidays;
            m_TypicalWeek = typicalWeek;
		}//CSeason

		/// <summary>
		/// Property to get the SeasonID
		/// </summary>
		/// <example>
		/// <code>
		/// CSeason mySeason = new CSeason(2, "Season 1", new int[4], new int[2]);
		/// int intID = mySeason.ID;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int ID
		{
			get
			{
				return m_intSeasonID;
			}
		}//ID

		/// <summary>
		/// Property to get the Season name
		/// </summary>
		/// <example>
		/// <code>
		/// CSeason mySeason = new CSeason(2, "Season 1", new int[4], new int[2]);
		/// string strName = mySeason.Name;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public string Name
		{
			get
			{
				return m_strSeasonName;
			}
            set
            {
                m_strSeasonName = value;
            }
		}//Name

		/// <summary>
		/// Property to get the list of normal days for the season
		/// </summary>
		/// <example>
		/// <code>
		/// CSeason mySeason = new CSeason(2, "Season 1", new int[4], new int[2]);
		/// int[] list = mySeason.NormalDays;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 06/28/06 ach N/A     N/A    Added the set property 
		public Int16Collection NormalDays
		{
			get
			{
				return m_aintNormalDays;
			}
            set
            {
                m_aintNormalDays = value;
            }
		}//NormalDays

		/// <summary>
		/// Property to get the list of holidays for the season
		/// </summary>
		/// <example>
		/// <code>
		/// CSeason mySeason = new CSeason(2, "Season 1", new int[4], new int[2]);
		/// int[] list = mySeason.Holidays;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 06/28/06 ach N/A     N/A    Added the set property
		public Int16Collection Holidays
		{
			get
			{
				return m_aintHolidays;
			}
            set
            {
                m_aintHolidays = value;
            }
		}//Holidays

        /// <summary>
        /// Property to get the typical week array.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/16/14 jrf 3.50.78 489749 Created.
        public string[] TypicalWeek
        {
            get
            {
                return m_TypicalWeek;
            }
            set
            {
                m_TypicalWeek = value;
            }
        }//Holidays

        /// <summary>
        /// This method will resize the normal day array so that it will 
        /// hold the correct number of days with the correct pattern id.
        /// Since day types can only be removed from the end, if the new length
        /// is smaller then the array will be made smaller with the same
        /// pattern ids remaining.  If the new length is larger then the
        /// current day types will keep their pattern ids with new day types
        /// being assigned the pattern id the first day type uses.
        /// </summary>
        /// <param name="nLength">
        /// Represents the new length of the array.
        /// </param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/06 ach N/A	 N/A	Added to support removal of day types
        public void ResizeNormalDays(int nLength)
        {
            int nOldSize = m_aintNormalDays.Count;
            int nSmallSize = (nLength <= nOldSize) ? nLength : nOldSize;

            if (0 == nLength)
            {
                m_aintNormalDays.Clear();
            }

            else if (nLength < nOldSize)
            {
                for (int nCount = nLength; nCount < nOldSize; nCount++)
                {
                    try
                    {
                        m_aintNormalDays.RemoveAt(nCount);
                    }
                    catch { }
                }

            }

            else if (nLength > nOldSize)
            {
                for (int nCount = nOldSize; nCount < nLength; nCount++)
                {
                    try
                    {
                        m_aintNormalDays.Add(m_aintNormalDays[0]);
                    }
                    catch { }
                }
            }           

        } // ResizeNormalDays

        /// <summary>
        /// This method will resize the holiday day array so that it will 
        /// hold the correct number of days with the correct pattern id.
        /// Since day types can only be removed from the end, if the new length
        /// is smaller then the array will be made smaller with the same
        /// pattern ids remaining.  If the new length is larger then the
        /// current day types will keep their pattern ids with new day types
        /// being assigned the pattern id the first day type uses.
        /// </summary>
        /// <param name="nLength">
        /// Represents the new length of the array.
        /// </param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/06 ach N/A	 N/A	Added to support removal of day types
        public void ResizeHolidays(int nLength)
        {
            int nOldSize = m_aintHolidays.Count;
            int nSmallSize = (nLength <= nOldSize) ? nLength : nOldSize;

            if (nLength < nOldSize)
            {
                for (int nCount = nLength; nCount < nOldSize; nCount++)
                {
                    m_aintHolidays.RemoveAt(nCount);
                }

            }

            else if (nLength > nOldSize)
            {
                for (int nCount = nOldSize; nCount < nLength; nCount++)
                {
                    m_aintHolidays.Add(m_aintHolidays[0]);
                }
            }           
           

        } // ResizeHolidays
    }
}
