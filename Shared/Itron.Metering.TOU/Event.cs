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
	/// Represents an event that takes place during a year.  This information
    /// includes the type of event (Season or Holiday), the date, and name.
	/// </summary>
	public class CEvent : IComparable
    {
        #region Member Variables

        /// <summary>
		/// Represents the date of the event
		/// </summary>
		private DateTime m_Date;

		/// <summary>
		/// Represents the index of the event in the Season or Holiday Collection
		/// depending upon the event type
		/// </summary>
		private int m_intIndex;

		/// <summary>
		/// Represents the type of event, rather it be a Season or a Holiday
		/// </summary>
		private eEventType m_eEventType;

		/// <summary>
		/// Represents the name of the event(if the event is a Season the name will 
		///	be season, if the even is a holiday the name will be the name of the 
		///	holiday)
		/// </summary>
		private string m_strName;

        #endregion

        #region Public Methods

        /// <summary>
		/// Creates an instance of the Event object
		/// </summary>
		/// <example>
		/// <code>
		/// CEvent event = new CEvent(new DateTime(2006,1,1), HOLIDAY, 2, "Christmas");
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		/// 04/26/06 rrr N/A	 N/A	Added a string parameter and instance variable 
		///								initialization
		public CEvent(DateTime objDate, eEventType eType, int intIndex, string strName)
		{
			m_strName = strName;
			m_Date = objDate;
			m_intIndex = intIndex;
			m_eEventType = eType;
		} //CEvent


		/// <summary>
		/// Used to be able to compare two CEvent objects based on the date
		/// </summary>
		/// <param name="obj">
		/// The event to compare to the current event
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
			CEvent objCompare = (CEvent) obj; 
			return this.Date.CompareTo(objCompare.Date);
		} //CompareTo

        #endregion

        #region Properties

        /// <summary>
		/// Property to get/set the Event Date
		/// </summary>
		/// <example>
		/// <code>
		/// CEvent event = new CEvent(new DateTime(2006,1,1), HOLIDAY, 2, "Christmas");
		/// DateTime temp = event.Time;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 06/29/06 mah                Added set method
		public DateTime Date
		{
			get
			{
				return m_Date;
			}
            set
            {
                m_Date = value;
            }
		}//Date


		/// <summary>
		/// Property to get the Event Type (Holiday or Season)
		/// </summary>
		/// <example>
		/// <code>
		/// CEvent event = new CEvent(new DateTime(2006,1,1), HOLIDAY, 2, "Christmas");
		/// eEventType temp = event.Type;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public eEventType Type
		{
			get
			{
				return m_eEventType;
			}
		}//Type


		/// <summary>
		/// Property to get the Event Index.  This index will tell you the 
		/// index to look at in the Season or Holiday collection depending on the
		/// Event Type
		/// </summary>
		/// <example>
		/// <code>
		/// CEvent event = new CEvent(new DateTime(2006,1,1), HOLIDAY, 2, "Christmas");
		/// int intIndex = event.Index;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class 
        /// 08/07/06 ach N/A     N/A    added set property
		public int Index
		{
			get
			{
				return m_intIndex;
			}
            set
            {
                m_intIndex = value;
            }
		}//Index

		
		/// <summary>
		/// Property to get the name of the event.  Will return the word season is we
		/// have a season or the name of a holiday
		/// </summary>
		/// <example>
		/// <code>
		/// CEvent event = new CEvent(new DateTime(2006,1,1), HOLIDAY, 2, "Christmas");
		/// string strName = event.Name;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/26/06 rrr N/A	 N/A	Added property to get the value of the new
		///								name instance variable 
		public string Name
		{
			get
			{
				return m_strName;
			}
		}//Name

        #endregion
    }
}
