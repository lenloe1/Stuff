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
using System.Collections.Generic;

namespace Itron.Metering.TOU
{
	/// <summary>
	/// This class represents a day type in the TOU Schedule through 
	/// what type of day it is (holiday or normal) and what index 
	/// the day type can be located at in holiday or normal day arrays.
	/// </summary>
	public class CDayType
    {
        #region members 

        /// <summary>
		/// Represents the Type of the DayType (Holiday or Normal)
		/// </summary>
		private eDayType m_eDayType;

		/// <summary>
		/// Represents the Index of the DayType.  This will index into
		/// holiday and normal day arrays.
		/// </summary>
		private int m_intDayIndex;

        #endregion

        #region public methods

        /// <summary>
		/// Constructor to create a CDayType object and set member variables
		/// </summary>
		/// <param name="eType">
		/// The Type of DayType
		/// </param>
		/// <param name="intIndex">
		/// The Index of the DayType
		/// </param>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CDayType(eDayType eType, int intIndex)
		{
			m_eDayType = eType;
			m_intDayIndex = intIndex;
		}// CDayType

        /// <summary>
        /// This method looks to see if this day type is being used within
        /// the typical week of the given TOU schedule.
        /// </summary>
        /// <param name="sched">
        /// Represents the TOU Schedule to search within.
        /// </param>
        /// <returns>
        /// Returns true if this day type is used in the typical week and
        /// false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/28/06 ach N/A	 N/A	Moved method from Day Types Form.cs  
        public Boolean UsedByTypicalWeek(CTOUSchedule sched)
        {
            // Local Variables
            String[] week = sched.TypicalWeek;
            List<string> days;
            Boolean result = false;

            // Find out which array to look in for a match
            if (eDayType.HOLIDAY == Type)
            {
                days = sched.Holidays;
            }
            else
            {
                days = sched.NormalDays;
            }

            // Search the typical week to see if this day is used
            for (int intCount = 0; intCount < week.Length; intCount++)
            {
                if (week[intCount] == days[Index])
                {
                    result = true;
                }
            }

            return result;
        }// UsedByTypicalWeek

        /// <summary>
        /// This method looks to see if this day type is being used in a 
        /// holiday within the given TOU Schedule
        /// </summary>
        /// <param name="sched">
        /// Represents the TOU Schedule to search within.
        /// </param>
        /// <returns>
        /// Returns true if this day type is used in a holiday and
        /// false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/28/06 ach N/A	 N/A	Moved method from Day Types Form.cs  
        public Boolean UsedByHoliday(CTOUSchedule sched)
        {
            // Local Variables
            CYearCollection years = sched.Years;
            CEventCollection events;
            String name = sched.Holidays[Index];
            Boolean result = false;

            // If this day type is not a holiday then stop 
            // and return false
            if (eDayType.NORMAL == Type)
            {
                return false;
            }

            // Search through all the years within the schedule
            foreach (CYear year in years)
            {
                events = year.Events;

                // Search each event in the year
                foreach (CEvent yearEvent in events)
                {
                    if (eEventType.HOLIDAY == yearEvent.Type &&
                        name == yearEvent.Name)
                    {
                        result = true;
                    }
                }

            }

            return result;
        }// UsedByHoliday

        #endregion

        #region properties

        /// <summary>
		/// Property to get the type of the DayType
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public eDayType Type
		{
			get
			{
				return m_eDayType;
			}
		}// Type

		/// <summary>
		/// Property to get the index of the DayType
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int Index
		{
			get
			{
				return m_intDayIndex;
			}
		}// Index

        #endregion

       
	}
}
