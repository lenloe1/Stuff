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
using System.Text;

namespace Itron.Metering.TOU
{
    /// <summary>
    /// SCS TOU Schedule class - Used for reading TOU out of the meter
    /// </summary>
    public class SCSTOUSchedule : CTOUSchedule
    {
        #region Constants
        #endregion

        #region Public Methods

        /// <summary>
        /// Default SCSTOUSchedule constructor
        /// </summary>
        public SCSTOUSchedule()
            : base()
        {
            //Build the arrays and collections            
            BuildDayTypes();
            BuildTypicalWeek();
            BuildRates();
            BuildOutputs();
            BuildPatterns();
            BuildSeasons();
            BuildYears();
        }

        #endregion

        #region Private Methods

        // Method to fill in the collection for the rates
        //
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/27/06 MAH 8.00.00	 N/A	Creation of class  
		// 03/28/07 MAH 8.00.22 2732 Added Total Rate for MT200 & CENTRON programs
        private void BuildRates()
        {
            m_astrRates = new List<string>();

            m_astrRates.Add("Rate A");
            m_astrRates.Add("Rate B");
            m_astrRates.Add("Rate C");
            m_astrRates.Add("Rate D");
			m_astrRates.Add("Total Rate");
		}//BuildRates()


        // Method to fill in the collection for the outputs
        //
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/27/06 MAH 8.00.00	 N/A	Creation of class  
        private void BuildOutputs()
        {
            m_astrOutputs = new List<string>();

        }//BuildOutputs()


        // Method to fill in the collection for the normal days and holidays
        // 
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/27/06 MAH N/A	 N/A	Creation of class  
        private void BuildDayTypes()
        {
            //Initialize the normal days and holiday array based on the counts
            m_astrNormalDays = new List<string>();
            m_astrHolidays = new List<string>();

            m_astrNormalDays.Add("Day Type 1");
            m_astrNormalDays.Add("Day Type 2");
            m_astrNormalDays.Add("Day Type 3");
            m_astrNormalDays.Add("Day Type 4");

            m_astrHolidays.Add("Holiday Type 1");
        }//BuildDayTypes()


        // Method to fill in the collection for the seasons
        //
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/27/06 MAH 8.00.00	 N/A	Creation of class  
        // 
        private void BuildSeasons()
        {
            //Initialize the Season Collection
            m_colSeasons = new CSeasonCollection();
        }//BuildSeasons()


        // Method to fill in the collection for the patterns
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/27/06 MAH 8.00.00	 N/A	Creation of class  
        // 
        private void BuildPatterns()
        {
            //Initialize the Pattern Collection
            m_colPatterns = new CPatternCollection();
        }//BuildPatterns()


        // Method to fill in the collection for the typical week
        //
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/27/06 MAH 8.00.00	 N/A	Creation of class  
        // 
        private void BuildTypicalWeek()
        {
            //Initialize the Typical Week Array
            m_astrTypicalWeek = new string[WEEKCOUNT];

            m_astrTypicalWeek[0] = "Sunday";
            m_astrTypicalWeek[1] = "Monday";
            m_astrTypicalWeek[2] = "Tuesday";
            m_astrTypicalWeek[3] = "Wednesday";
            m_astrTypicalWeek[4] = "Thursday";
            m_astrTypicalWeek[5] = "Friday";
            m_astrTypicalWeek[6] = "Saturday";
        }//BuildTypicalWeek()



        // Method to fill in the collection for the years
        //
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/27/06 MAH 8.00.00	 N/A	Creation of class  
        // 
        private void BuildYears()
        {
            //Initialize the year collection 
            m_colYears = new CYearCollection();
        }//BuildYears()

        #endregion
    }
}
