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
	/// Represents a holiday that takes place during the year
	/// </summary>
	public class CHoliday
    {
    #region LocalVariables

        /// <summary>
		/// Represents the ID of the holiday
		/// </summary>
		private int m_intHolidayID;

		/// <summary>
		/// Represents the name of the holiday
		/// </summary>
		private string m_strHolidayName;

		/// <summary>
		/// DateTime object that represents the month, day, year of the holiday.
		/// If the Holiday is yearly, year will hold a default value of 2000.
		/// </summary>
		private DateTime m_dtHolidayDate;		

		/// <summary>
		/// Represents what type of Holiday this day is (Type 1 or Type 2)
		/// </summary>
		private eTypeIndex m_HolidayTypeIndex;

		/// <summary>
		/// Represents if the holiday occurs in a single year or every year.
		/// </summary>
		private eFrequency m_YearFrequency;

        /// <summary>
        /// Represents where to move a Saturday holiday to
        /// (Don't, Previous Friday, or Following Monday)
        /// </summary>
        private eMoveHoliday m_MoveFromSat;

        /// <summary>
        /// Represents where to move a Sunday holiday to
        /// (Don't, Previous Friday, or Following Monday)
        /// </summary>
        private eMoveHoliday m_MoveFromSun;

    #endregion

    #region constructors

        /// <summary>
		/// Creates an instance of a Holiday object that does not move holidays.
        /// This is used in conjunction with adding a fixed holiday which does 
        /// not need to be moved as the date is specifically picked for a
        /// specific day.
		/// </summary>
		/// <example>
		/// <code>
		/// DateTime Date = new DateTime(2006, 5, 23);
		/// bool singleYear = true;
		/// CHoliday myHoliday = new CHoliday(2, "BDay", Date, 0, eFrequency.SINGLE);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/23/06 ach N/A	 N/A	Creation of class  
		public CHoliday(int ID, string Name, DateTime date, 
                        int index, eFrequency yearFrequency)
		{
			m_intHolidayID = ID;
			m_strHolidayName = Name;
			m_dtHolidayDate = date;
			if (0 == index)
			{
				m_HolidayTypeIndex = eTypeIndex.TYPE_1;
			}
			else
			{
				m_HolidayTypeIndex = eTypeIndex.TYPE_2;
			}

		    m_YearFrequency = yearFrequency;

            m_MoveFromSat = eMoveHoliday.DONT;
            m_MoveFromSun = eMoveHoliday.DONT;

		} //CHoliday

        /// <summary>
        /// Creates an instance of a Holiday object that moves holidays based
        /// upon given parameters.  This is used in conjuction with adding a
        /// recurring holiday which can be moved if it falls on a Saturday or
        /// Sunday.
        /// </summary>
        /// <example>
        /// <code>
        /// DateTime Date = new DateTime(2006, 5, 23);
        /// bool singleYear = true;
        /// CHoliday myHoliday = new CHoliday(2, "BDay", Date, 0, eFrequency.SINGLE
        ///                             eMoveHoliday.FRI, eMoveHoliday.MON);
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 ach N/A	 N/A	Creation of class  
        public CHoliday(int ID, string Name, DateTime date,
                        int index, eFrequency yearFrequency,
                        eMoveHoliday Sat, eMoveHoliday Sun)
        {
            m_intHolidayID = ID;
            m_strHolidayName = Name;
            m_dtHolidayDate = date;
            if (0 == index)
            {
                m_HolidayTypeIndex = eTypeIndex.TYPE_1;
            }
            else
            {
                m_HolidayTypeIndex = eTypeIndex.TYPE_2;
            }

            m_YearFrequency = yearFrequency;

            m_MoveFromSat = Sat;
            m_MoveFromSun = Sun;

        } //CHoliday

    #endregion

    #region properties
        /// <summary>
		/// Property to get the ID
		/// </summary>
		/// <example>
		/// <code>
		/// DateTime Date = new DateTime(2006, 5, 23);
		/// bool singleYear = true;
		/// CHoliday hol = new CHoliday(2, "holiday", Date, 0, singleYear);
		/// int intID = hol.ID;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ----------------------------------------
		/// 05/23/06 ach N/A     N/A    Creation of class
		public int ID
		{
			get
			{
				return m_intHolidayID;
			}
		} //ID


		/// <summary>
		/// Properties to get and set the name
		/// </summary>
		/// <example>
		/// <code>
		/// DateTime Date = new DateTime(2006, 5, 23);
		/// bool singleYear = true;
		/// CHoliday hol = new CHoliday(2, "holiday", Date, 0, singleYear);
		/// string strName = hol.Name;
		/// hol.Name = "holiday2";
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ----------------------------------------
		/// 05/23/06 ach N/A     N/A    Creation of class
		public string Name
		{
			get
			{
				return m_strHolidayName;
			}
			set
			{
				m_strHolidayName = value;
			}
		} //Name


		/// <summary>
		/// Properties to get and set the Date
		/// </summary>
		/// <example>
		/// <code>
		/// DateTime Date = new DateTime(2006, 5, 23);
		/// bool singleYear = true;
		/// CHoliday hol = new CHoliday(2, "holiday", Date, 0, singleYear);
		/// DateTime date = hol.Date;
		/// hol.Date = date;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ----------------------------------------
		/// 05/23/06 ach N/A     N/A    Creation of class
		public DateTime Date
		{
			get
			{
				return m_dtHolidayDate;
			}
			set
			{
				m_dtHolidayDate = value;
			}
		} //Date


		/// <summary>
		/// Properties to get and set the Type Index
		/// </summary>
		/// <example>
		/// <code>
		/// DateTime Date = new DateTime(2006, 5, 23);
		/// bool singleYear = true;
		/// CHoliday hol = new CHoliday(2, "holiday", Date, 0, singleYear);
		/// int intTypeIndex = hol.Index;
		/// hol.Index = 0;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ----------------------------------------
		/// 05/23/06 ach N/A     N/A    Creation of class
		public int Index
		{
			get
			{
				if (eTypeIndex.TYPE_1 == m_HolidayTypeIndex)
				{
					return 0;
				}
				else
				{
					return 1;
				}
				
			}
			set
			{
				if (1 == value)
				{
					m_HolidayTypeIndex = eTypeIndex.TYPE_1;
				}
				else
				{
					m_HolidayTypeIndex = eTypeIndex.TYPE_2;
				}
				
			}
		} //Index


		/// <summary>
		/// Properties to get and set whether this holiday occurs in a single year
		/// or multiple years.
		/// </summary>
		/// <example>
		/// <code>
		/// DateTime Date = new DateTime(2006, 5, 23);
		/// CHoliday hol = new CHoliday(2, "holiday", Date, 0, eFrequency.SINGLE);
		/// hol.Frequency = eFrequency.SINGLE;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ----------------------------------------
		/// 05/23/06 ach N/A     N/A    Creation of class
		public eFrequency Frequency
		{
			get
			{
                return m_YearFrequency;				
			}
			set
			{
                m_YearFrequency = value;				
			}
		} //Frequency

        /// <summary>
        /// Property to get if and where a Saturday Holiday will be moved.
        /// </summary>
        /// <example>
        /// <code>
        /// CHoliday hol = new CHoliday(2, "holiday", Date, 0, false);
        /// eMoveHoliday MoveSaturday = hol.MoveSaturday;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ----------------------------------------
        /// 07/04/06 ach N/A     N/A    Added ability to store moving individual
        ///                             holidays 
        public eMoveHoliday MoveSaturday
        {
            get
            {
                return m_MoveFromSat;
            }
        } //MoveSaturday

        /// <summary>
        /// Property to get if and where a Sunday Holiday will be moved.
        /// </summary>
        /// <example>
        /// <code>
        /// CHoliday hol = new CHoliday(2, "holiday", Date, 0, false);
        /// eMoveHoliday MoveSunday = hol.MoveSunday;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ----------------------------------------
        /// 07/04/06 ach N/A     N/A    Added ability to store moving individual
        ///                             holidays 
        public eMoveHoliday MoveSunday
        {
            get
            {
                return m_MoveFromSun;
            }
        } //MoveSunday

    #endregion
	}
}
