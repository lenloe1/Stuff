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
	/// Class to represent all the data contained in a year of a TOU Schedule.
	/// It holds the number of the year and a collection of events that occur
	/// within that year.
	/// </summary>
	public class CYear : IComparable
	{
		/// <summary>
		/// Represents the year of the current object
		/// </summary>
		private int m_intYear;

		/// <summary>
		/// Collection that holds all the events of the current Year
		/// This collection will be sorted by the Date
		/// </summary>
		private CEventCollection m_colEvents;

		/// <summary>
		/// Creates an instace of a Year object
		/// </summary>
		/// <example>
		/// <code>
		/// CYear myYear = new CYear(2006, new CEventCollection());
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CYear(int intYear, CEventCollection colEvents)
		{
			m_intYear = intYear;
			m_colEvents = colEvents;
		} // CYear


		/// <summary>
		/// Used to be able to compare two CYear objects based on the year
		/// </summary>
		/// <param name="obj">
		/// The year to compare to the current year
		/// </param>
		/// <returns>
		/// An int that represents the objects being equal, less than, or greater than
		/// </returns>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int CompareTo(object obj)
		{
			CYear objCompare = (CYear) obj; 
			return this.Year.CompareTo(objCompare.Year);
		} // CompareTo


		/// <summary>
		/// Property to get the year
		/// </summary>
		/// <example>
		/// <code>
		/// CYear myYear = new CYear(2006, new CEventCollection());
		/// int intYear = myYear.Year;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 02/15/07 ach 8.0     167    Added set property
		public int Year
		{
			get
			{
				return m_intYear;
			}
            set
            {
                m_intYear = value;
            }
		} // Year


		/// <summary>
		/// Property to get the events for the current year.  These events will be 
		/// sorted by date.
		/// </summary>
		/// <example>
		/// <code>
		/// CYear myYear = new CYear(2006, new CEventCollection());
		/// CEventCollection coll = myYear.Events;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CEventCollection Events
		{
			get
			{
				return m_colEvents;
			}
		} // Events


        /// <summary>
        /// Property to get the number of holiday events in the year.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/25/06 ach N/A	 N/A	Added property 
        public int HolidayCount
        {
            get
            {
                int nCount = 0;

                foreach (CEvent yrEvent in Events)
                {
                    if (eEventType.HOLIDAY == yrEvent.Type)
                    {
                        nCount++;
                    }
                }

                return nCount;
            }
        } // HolidayCount

        /// <summary>
        /// Property to get the number of season events in the year.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/25/06 ach N/A	 N/A	Added property 
        public int SeasonCount
        {
            get
            {
                int nCount = 0;

                foreach (CEvent yrEvent in Events)
                {
                    if (eEventType.SEASON == yrEvent.Type)
                    {
                        nCount++;
                    }
                }

                return nCount;
            }
        } // SeasonCount

	}
}
