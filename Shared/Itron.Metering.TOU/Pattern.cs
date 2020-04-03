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
	/// Represents a pattern in a TOU Schedule through holding the
	/// pattern ID and Name as well as a collection of switchpoints
	/// that represent what rate or outputs are used at what times.
	/// </summary>
	public class CPattern: IEquatable<CPattern>
	{
		/// <summary>
		/// Represents the PatternID
		/// </summary>
		private int m_intPatternID;

		/// <summary>
		/// Represents the PatternName
		/// </summary>
		private string m_strPatternName;

		/// <summary>
		/// List of SwitchPoint objects which has the start time, stop time,
		/// rateoutput index, and the type (Rate or Output).  This list will
		/// be sorted by the start time.
		/// </summary>
		private CSwitchPointCollection m_colSwitchPoints;


		/// <summary>
		/// Creates an instace of a Pattern object
		/// </summary>
		/// <example>
		/// <code>
		/// CPattern patt = new CPattern(2, "Pattern  1", new CSwitchPointCollection());
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CPattern(int intID, string strName, CSwitchPointCollection colSwitchPoints)
		{
			m_intPatternID = intID;
			m_strPatternName = strName;
			m_colSwitchPoints = colSwitchPoints;
		}//CPattern

        /// <summary>
        /// This method checks to see if the given and current patterns 
        /// contain the same switchpoints.
        /// </summary>
        /// <param name="Pattern"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/01/08 jrf 9.50.16        Created.
        //
        public bool Equals(CPattern Pattern)
        {
            bool blnEquals = true;

            //Check to see this pattern contains all switchpoints in the comparison 
            //pattern
            foreach (CSwitchPoint SwitchPoint in Pattern.SwitchPoints)
            {
                if (false == this.SwitchPoints.Contains(SwitchPoint))
                {
                    blnEquals = false;
                    break;
                }
            }

            //Check to see that the comparison pattern contains all the switchpoints
            //in this pattern
            foreach (CSwitchPoint SwitchPoint in this.SwitchPoints)
            {
                if (false == Pattern.SwitchPoints.Contains(SwitchPoint))
                {
                    blnEquals = false;
                    break;
                }
            }

            return blnEquals;
        }

		/// <summary>
		/// Property to get the Pattern ID
		/// </summary>
		/// <example>
		/// <code>
		/// CPattern myPattern = new CPattern(2, "Pattern  1", new CSwitchPointCollection());
		/// int intID = myPattern.ID;
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
				return m_intPatternID;
			}
		}//ID


		/// <summary>
		/// Property to get and set Pattern Name
		/// </summary>
		/// <example>
		/// <code>
		/// CPattern myPattern = new CPattern(2, "Pattern  1", new CSwitchPointCollection());
		/// string strName = myPattern.Name;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class 
        /// 07/26/06 ach N/A     N/A    Added set property
		public string Name
		{
			get
			{
				return m_strPatternName;
			}
            set
            {
                m_strPatternName = value;
            }
		}//Name


		/// <summary>
		/// Property to get and set the SwitchPoints of the Pattern sorted by start time
		/// </summary>
		/// <example>
		/// <code>
		/// CPattern myPattern = new CPattern(2, "Pattern  1", new CSwitchPointCollection());
		/// CSwitchPointCollection coll = myPattern.SwitchPoints;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 07/26/06 ach N/A     N/A    Added set property
		public CSwitchPointCollection SwitchPoints
		{
			get
			{
				return m_colSwitchPoints;
			}
            set
            {
                m_colSwitchPoints = value;
            }
		}//SwitchPoints

        /// <summary>
        /// Property to get if the pattern has overlapping rates
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/26/07 ach 8.0     124    Added get property
        public Boolean OverlappingRates
        {
            get
            {
                CSwitchPoint previousSwitchpoint = null;
                SwitchPoints.Sort();

                foreach (CSwitchPoint switchpoint in SwitchPoints)
                {
                    if (eSwitchPointType.RATE == switchpoint.SwitchPointType)
                    {
                        if (null != previousSwitchpoint)
                        {
                            if (switchpoint.StartTime < previousSwitchpoint.StopTime)
                            {
                                return true;
                            }
                        }
                        previousSwitchpoint = switchpoint;
                    }
                }

                return false;
            }
        } // OverlappingRates

        /// <summary>
        /// Property to get if the pattern covers all 24 hours
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/26/07 ach 8.0     124    Added get property
        public Boolean FullCoverage
        {
            get
            {
                TimeSpan tsCoverage = new TimeSpan(0, 0, 0);

                foreach (CSwitchPoint switchpoint in SwitchPoints)
                {
                    // We need to the 24 hour coverage to be fulfilled by rates only
                    if (eSwitchPointType.RATE == switchpoint.SwitchPointType)
                    {
                        TimeSpan tsDiff = switchpoint.TimeOfStop.Subtract(switchpoint.TimeOfStart);


                        // Since we have already checked to make sure the end time is after the
                        // start time, if the hours are negative we know that this is the end of 
                        // the day (0:00) subtracting the start time to get a negative number
                        // By adding 24 hours to the time span's hours we will get the real difference
                        if (tsDiff.Hours < 0)
                        {
                            tsDiff = new TimeSpan(tsDiff.Hours + 24, tsDiff.Minutes, tsDiff.Seconds);
                        }

                        tsCoverage = new TimeSpan(tsCoverage.Hours + tsDiff.Hours, tsCoverage.Minutes + tsDiff.Minutes, tsCoverage.Seconds + tsDiff.Seconds);

                    }
                }

                if (1 != tsCoverage.Days)
                {
                    return false;
                }

                return true;
            }
        } // FullCoverage
	}
}
