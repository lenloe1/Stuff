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
	/// Class represents a switch point in a pattern of a TOU Schedule.
	/// Contains a start and stop time, whether the type is a rate or output
	/// and an index that represents what rate or output in the arrays to use.
	/// </summary>
	public class CSwitchPoint : IComparable, IEquatable<CSwitchPoint>
	{
		/// <summary>
		/// Variable represents the start time in minutes since 
        /// midnight.
		/// </summary>
		private int m_intStartTime;

		/// <summary>
		/// Variable represents the stop time in minute since
        /// midnight
		/// </summary>
		private int m_intStopTime;

		/// <summary>
		/// Variable represents the the Rate or Output index used
        /// to reference into the rate or output arrays depending
        /// on switchpoint type.
		/// </summary>
		private int m_intRateOutputIndex;

		/// <summary>
		/// variable represents the switch point type (Rate or Output)
		/// </summary>
		private eSwitchPointType m_eSwitchPointType;

		/// <summary>
		/// Creates an instance of the SwitchPoint object
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPoint myPoint = new CSwitchPoint(0, 120, 2, RATE);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CSwitchPoint(int intStart, int intStop, int intIndex, 
			eSwitchPointType eType)
		{
			m_intStartTime = intStart;
			m_intStopTime = intStop;
			m_intRateOutputIndex = intIndex;
			m_eSwitchPointType = eType;
		}//CSwitchPoint

        /// <summary>
        /// This method checks to see if the current switchpoint's properties
        /// match those of the given switchpoint.
        /// </summary>
        /// <param name="SwitchPoint">The switchpoint to compare to.</param>
        /// <returns></returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/01/08 jrf 9.50.16        Created.
        //
        public bool Equals(CSwitchPoint SwitchPoint)
        {
            bool blnEquals = false;

            //Check to see if the core properties of the switchpoint 
            //match
            blnEquals = (SwitchPoint.StartTime == this.StartTime);

            if (blnEquals)
            {
                blnEquals = (SwitchPoint.StopTime == this.StopTime);
            }

            if (blnEquals)
            {
                blnEquals = (SwitchPoint.RateOutputIndex == this.RateOutputIndex);
            }

            if (blnEquals)
            {
                blnEquals = (SwitchPoint.SwitchPointType == this.SwitchPointType);
            }

            return blnEquals;
        }

		/// <summary>
		/// Used to be able to compare two CSwitchPoint objects based on the start time
		/// </summary>
		/// <param name="obj">
		/// The switch point to compare to the current switch point
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
			CSwitchPoint objCompare = (CSwitchPoint) obj; 
			return this.StartTime.CompareTo(objCompare.StartTime);
		}//CompareTo


		/// <summary>
		/// Property to get the StartTime which is represented as 
		/// minutes since midnight
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPoint myPoint = new CSwitchPoint(0, 120, 2, RATE);
		/// int intTime = myPoint.StartTime;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 01/26/07 ach N/A     N/A    Added set property
		public int StartTime
		{
			get
			{
				return m_intStartTime;
			}
            set
            {
                m_intStartTime = value;
            }
		}//StartTime


		/// <summary>
		/// Property to get the Stop Time which is represented as minutes
		/// since midnight
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPoint myPoint = new CSwitchPoint(0, 120, 2, RATE);
		/// int intTime = myPoint.StopTime;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 01/22/07 ach 8.0     N/A    Modified get property to return 1440 if
        ///                             stop time is 0
		public int StopTime
		{
			get
			{
                // If the stop time is 0 that means it is set to the 0:00 hour of 
                // the next day.  When this is the case we want to store the time as
                // being 24 hours after midnight of the previous day rather than being
                // 0 hours since the next day
                if (0 == m_intStopTime)
                {
                    return 1440;
                }
                else
                {
                    return m_intStopTime;
                }
                    
			}
			set
			{
                m_intStopTime = value;
			}
		}//StopTime


		/// <summary>
		/// Property to set/get the Rate or Output Index
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPoint myPoint = new CSwitchPoint(0, 120, 2, RATE);
		/// int intIndex = myPoint.RateOutputIndex;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int RateOutputIndex
		{
			get
			{
				return m_intRateOutputIndex;
			}
            set
            {
                m_intRateOutputIndex = value;
            }
		}//RateOutputIndex


		/// <summary>
		/// Property to get and set the Switch Point Type
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPoint myPoint = new CSwitchPoint();
		/// eSwitchPointType myType = myPoint.SwitchPointType;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 01/25/07 ach 8.0     N/A    Added set property
		public eSwitchPointType SwitchPointType
		{
			get
			{
				return m_eSwitchPointType;
			}
            set
            {
                m_eSwitchPointType = value;
            }
		}//SwitchPointType


		/// <summary>
		/// Property to get the Time of the start point of the Switch Point Type
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPoint myPoint = new CSwitchPoint();
		/// DateTime timeStart = myPoint.TimeOfStart;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/26/06 rrr N/A	 N/A	Added to convert the start time (which is in
		///								minutes since midnight) to a DateTime object 
        ///	06/19/06 mah                Added set method to allow updating of Switch Points
        ///	
		public DateTime TimeOfStart
		{
			get
			{
				//Get the hours and minutes by dividing and moding by 60
				int intHour = m_intStartTime / 60;
				int intMinute = m_intStartTime % 60;

				//If we reach the 24 hour period then we are back at midnight
				if(24 == intHour)
				{
					intHour = 0;
				}

				//Return a date time object with the new time and default date
				return new DateTime(2005,1,1,intHour,intMinute,0);
			}

            set
            {
                m_intStartTime = (value.Hour * 60) + (value.Minute);
            }
		}//TimeOfStart

		/// <summary>
		/// Property to get the Time of the stop point of the Switch Point Type
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPoint myPoint = new CSwitchPoint();
		/// DateTime timeStop = myPoint.TimeOfStop;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/26/06 rrr N/A	 N/A	Added to convert the stop time (which is in
		///								minutes since midnight) to a DateTime object 
        ///	01/25/07 ach 8.0     N/A    changed the set to set the stop time to 1440
        ///	                            if the stop time hour and minute are both 0
		public DateTime TimeOfStop
		{
			get
			{
				//Get the hours and minutes my dividing and moding by 60
				int intHour = m_intStopTime / 60;
				int intMinute = m_intStopTime % 60;

				//If we reach the 24 hour period then we are back at midnight
				if(24 == intHour)
				{
					intHour = 0;
				}							

				//Return a date time object with the new time and default date
				return new DateTime(2005, 1,1,intHour,intMinute,0);
			}
            
            set
            {
                // this is the end time of the day (0:00 of the next day = 24:00)
                if (0 == value.Hour && 0 == value.Minute)
                {
                    m_intStopTime = 1440;
                }
                else
                {
                    m_intStopTime = (value.Hour * 60) + (value.Minute);
                }                
            }
		}//TimeOfStop

	}
}
