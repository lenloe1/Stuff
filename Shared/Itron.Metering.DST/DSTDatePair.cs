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

namespace Itron.Metering.DST
{
	/// <summary>
	/// Represents the To DST Date and From DST Date for a single year.    Each date is accessable
	/// through public properties
	/// </summary>
	public class CDSTDatePair : IComparable	
	{
		/// <summary>
		/// Represents the To Date of the object
		/// </summary>
		private DateTime m_dtToDate;

		/// <summary>
		/// Represents the From Date of the object
		/// </summary>
		private DateTime m_dtFromDate;

        
        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/26/06 KRC 7.36.00
        //
        public CDSTDatePair()
        {
            m_dtToDate = new DateTime();
            m_dtFromDate = new DateTime();
        }

		/// <summary>
		/// Constructor - takes the parameters and set the instance variables.  
		/// </summary>
		/// <param name="dtToDate">
		/// The To Date of the object
		/// </param>
		/// <param name="dtFromDate">
		/// The From Date of the object
		/// </param>
		/// <example>
		/// <code>
		/// CDSTDatePair myYear = new CDSTDatePair(new DateTime(2006,4,2), new DateTime(2006,10,29));
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public CDSTDatePair(DateTime dtToDate, DateTime dtFromDate)
		{
			m_dtToDate = dtToDate;
			m_dtFromDate = dtFromDate;
		}

		/// <summary>
		/// Property to get the objects To Date
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTDatePair myYear = new CDSTDatePair(new DateTime(2006,4,2), new DateTime(2006,10,29));
		/// DateTime dtDate = myYear.ToDate;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public DateTime ToDate
		{
			get
			{
				return m_dtToDate;
			}
            set
			{
                m_dtToDate = value;
			}

		}

		/// <summary>
		/// Property to get the objects From Date
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTDatePair myYear = new CDSTDatePair(new DateTime(2006,4,2), new DateTime(2006,10,29));
		/// DateTime dtDate = myYear.FromDate;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public DateTime FromDate
		{
			get
			{
				return m_dtFromDate;
			}
   			set
			{
				m_dtFromDate = value;
			}

		}

		/// <summary>
		/// Method that allows to CDSTDatePair objects to be compared
		/// </summary>
		/// <param name="obj">
		/// The object to compare the current CDSTDatePair too
		/// </param>
		/// <returns>
		/// An int that tells if objects are equal, less than, or greater than
		/// </returns>
        /// <remarks>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
        /// </remarks>
		public int CompareTo(object obj)
		{
			CDSTDatePair objCompare = (CDSTDatePair) obj; 
			return this.ToDate.Year.CompareTo(objCompare.ToDate.Year);
		}

        /// <summary>
        /// This method indicates whether the given dstDate pair is compliant with the Energy
        /// Policy Act of 2005
        /// </summary>
        /// <param name="dstDates">A single pair of DST dates</param>
        /// <returns>True if the both the To DST date and the From DST date comply with the
        /// Energy Policy Act of 2005, False is returned if either date differs from the Act's mandate.
        /// A value of True will be returned for all DST date pairs prior to 2007
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -------------------------------------------
        /// 01/15/07 MAH 8.00.00 Created
        /// </remarks>
        static public Boolean EPACompliant(CDSTDatePair dstDates)
        {
            Boolean boolCompliant = true; //innocent until proven guilty

            // The EPA Act of 2005 is not concerned with DST dates prior to 2007 therefore we will
            // simply say that all dates prior to 2007 are compliant

            if (dstDates.FromDate.Year >= 2007)
            {
                // Start by creating the EPA dates for the current year

                // The energy policy act calls for the to DST date to occur on the second Sunday in March.
                // In order to find the date, set the calendar to March 8th, if it doesn't fall on a Sunday, and add a day until
                // we finally reach a Sunday.

                DateTime dateEPAToDST = new DateTime(dstDates.FromDate.Year, 3, 8);

                while (dateEPAToDST.DayOfWeek != DayOfWeek.Sunday)
                {
                    dateEPAToDST = dateEPAToDST.AddDays(1);
                }

                // The from DST date will occur on the first Sunday in November.  In this case set the calendar to 
                // March 1st and keep adding days until we reach the first Sunday

                DateTime dateEPAFromDST = new DateTime(dstDates.ToDate.Year, 11, 1);

                while (dateEPAFromDST.DayOfWeek != DayOfWeek.Sunday)
                {
                    dateEPAFromDST = dateEPAFromDST.AddDays(1);
                }

                boolCompliant = (dateEPAToDST.Month == dstDates.ToDate.Month) &&
                                                    (dateEPAToDST.Day == dstDates.ToDate.Day) &&
                                                    (dateEPAFromDST.Month == dstDates.FromDate.Month) &&
                                                    (dateEPAFromDST.Day == dstDates.FromDate.Day);
            }

            return boolCompliant;
        }
	}
}
