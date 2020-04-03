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
using System.IO;
using System.Resources;
using System.Xml;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;


namespace Itron.Metering.TOU
{

    #region Definitions

    /// <summary>
    /// Enumeration to represent the days of the week
    /// </summary>
    public enum eTypicalDay
    {
        /// <summary>
        /// Represents Sunday's position in the array
        /// </summary>
        SUNDAY = 0,

        /// <summary>
        /// Represents Monday's position in the array
        /// </summary>
        MONDAY = 1,

        /// <summary>
        /// Represents Tuesday's position in the array
        /// </summary>
        TUESDAY = 2,

        /// <summary>
        /// Represents Wednesday's position in the array
        /// </summary>
        WEDNESDAY = 3,

        /// <summary>
        /// Represents Thursday's position in the array
        /// </summary>
        THURSDAY = 4,

        /// <summary>
        /// Represents Friday's position in the array
        /// </summary>
        FRIDAY = 5,

        /// <summary>
        /// Represents Saturday's position in the array
        /// </summary>
        SATURDAY = 6
    }

    /// <summary>
    /// Enumeration to represent the two DayTypes (Normal and Holiday)
    /// </summary>
    public enum eDayType
    {
        /// <summary>
        /// Normal day type
        /// </summary>
        NORMAL = 1,

        /// <summary>
        /// Holiday day type
        /// </summary>
        HOLIDAY = 2
    }

    /// <summary>
    /// Enumeration to represent the two SwitchPointTypes (Rate and Output);
    /// </summary>
    public enum eSwitchPointType
    {
        /// <summary>
        /// Rate switch point type
        /// </summary>
        RATE = 0,

        /// <summary>
        /// output switch point type
        /// </summary>
        OUTPUT = 1
    }

    /// <summary>
    /// Enumeration to represent the two Event Types for a Year (Holiday and 
    /// Season)
    /// </summary>
    public enum eEventType
    {
        /// <summary>
        /// Season event type
        /// </summary>
        [EnumDescription("Season")]
        SEASON = 0,

        /// <summary>
        /// Holiday event type
        /// </summary>
        [EnumDescription("Holiday")]
        HOLIDAY = 1,

        /// <summary>
        /// Used to indcate that there is no event
        /// </summary>
        [EnumDescription("No Event")]
        NO_EVENT = 2,

        /// <summary>
        /// The Start of DST
        /// </summary>
        [EnumDescription("To DST")]
        TO_DST = 3,

        /// <summary>
        /// The End of DST
        /// </summary>
        [EnumDescription("From DST")]
        FROM_DST = 4,
    }

    /// <summary>
    /// enum to hold the holiday day types (Type 1 or Type 2)
    /// </summary>
    public enum eTypeIndex
    {
        /// <summary>
        /// Represents the index for Holiday Type 1
        /// </summary>
        TYPE_1,

        /// <summary>
        /// Represents the index for Holiday Type 2
        /// </summary>
        TYPE_2
    }

    /// <summary>
    /// enum to hold how often the holiday occurs (Single or Multi)
    /// </summary>
    public enum eFrequency
    {
        /// <summary>
        /// Represents a holiday that occurs in a single year
        /// </summary>
        SINGLE,

        /// <summary>
        /// Represents a holiday that occurs every year (MULTIple years)
        /// </summary>
        MULTI
    }

    /// <summary>
    /// enum to hold where a holiday that occurs over multiple years
    /// is moved to if it falls on a weekend
    /// </summary>
    public enum eMoveHoliday
    {
        /// <summary>
        /// Represents not moving the holiday if it falls on a weekend
        /// </summary>
        DONT,

        /// <summary>
        /// Represents moving the holiday to the previous Friday
        /// </summary>
        FRI,

        /// <summary>
        /// Represents moving the holiday to the following Monday
        /// </summary>
        MON
    }


#endregion Definitions

	/// <summary>
	/// Class to represent a TOU Schedule
	/// </summary>
	public class CTOUSchedule: IProgressable
    {
    #region Constants

        //Constant variables

        // Strings representing node names in file
        private const string SUPPORTED_DEVICE = "SupportedDevice";
        private const string DAY_TYPE = "Daytype";
        private const string TYPICAL_WEEK = "TypicalWeek";
        private const string TOU_ID = "TOU_ID";
        private const string TOU_EXPORT_COUNT = "ExportCount";
        private const string TOU_NAME = "Name";
        private const string RATE = "Rate";
        private const string OUTPUT = "Output";
        private const string PATTERN = "Pattern";
        private const string SEASON = "Season";
        private const string YEAR = "Year";
        private const string APPLIED_HOLIDAY = "AppliedHolidays";
        private const string APPLIED_SEASON = "AppliedSeasons";
        private const string YEAR_NUMBER = "YearNumber";
        private const string SEASON_ID = "SeasonID";
        private const string NAME = "Name";
        private const string APPLIED_PATTERNS = "AppliedPatterns";
        private const string PATTERN_ID = "PatternID";
        private const string SWITCHPOINT = "SwitchPoint";
        private const string START_TIME = "StartTime";
        private const string STOP_TIME = "StopTime";
        private const string INDEX = "RateOutputIndex";
        private const string TYPE = "SwitchPointType";
        private const string STARTYEAR = "StartYear";
        private const string DURATION = "Duration";
        private const string DESCRIPTION = "Description";
        private const string DATECREATED = "DateCreated";
        private const string DATEMODIFIED = "DateModified";
        private const string HOLIDAYLISTID = "HolidayListID";
        private const string DATE_FORMAT_INFO = "Date_Format_Info";
        
        /// <summary>
        /// Number of Days in the Week
        /// </summary>
        protected const int WEEKCOUNT = 7;

   // Supported Device String Constants

        /// <summary>
        /// Represents the name of the Q1000
        /// </summary>
        public const string Q1000_DEVICE = "Q1000";
        /// <summary>
        /// Represents the name of Sentinel Advanced
        /// </summary>
        public const string SENTINEL_ADV_DEVICE = "SENTINEL - Advanced";
        /// <summary>
        /// Represents the name of Sentinel Basic
        /// </summary>
        public const string SENTINEL_BAS_DEVICE = "SENTINEL - Basic";
        /// <summary>
        /// Represents the name of Vectron
        /// </summary>
        public const string VECTRON_DEVICE = "VECTRON";
        /// <summary>
        /// Represents the name of Centron
        /// </summary>
        public const string CENTRON_DEVICE = "CENTRON";
        /// <summary>
        /// Represents the name of Centron Mono
        /// </summary>
        public const string CENTRON_MONO_DEVICE = "CENTRON (C12.19)";
        /// <summary>
        /// Represents the name of Centron Poly
        /// </summary>
        public const string CENTRON_POLY_DEVICE = "CENTRON (V&&I)";
        /// <summary>
        /// Represents the name of Centron Poly
        /// </summary>
        public const string CENTRON_POLY_DEVICE_OBSOLETE = "CENTRON (V&I)";
        /// <summary>
        /// Represents the name of Fulcrum
        /// </summary>
        public const string FULCRUM_DEVICE = "FULCRUM";
        /// <summary>
        /// Represents the name of Quantum
        /// </summary>
        public const string QUANTUM_DEVICE = "QUANTUM";
        /// <summary>
        /// Represents the name of the 200 Series
        /// </summary>
        public const string DMTMTR200_DEVICE = "200 Series";     

        // Device Specific Limits

        // Sentinel Limits
        private const int MAX_SENTINEL_DAYTYPES = 3;
        private const int MAX_SENTINEL_BAS_HOLTYPES = 1;
        private const int MAX_SENTINEL_ADV_HOLTYPES = 1;
        private const int MAX_SENTINEL_BAS_RATES = 4;
        private const int MAX_SENTINEL_ADV_RATES = 7;
        private const int MAX_SENTINEL_BAS_OUTPUTS = 4;
        private const int MAX_SENTINEL_ADV_OUTPUTS = 4;
        private const int MAX_SENTINEL_SWPTS_PER_PATTERN = 24;
        private const int MAX_SENTINEL_SEASONS = 8;
        private const int MAX_SENTINEL_SEASONS_PER_YEAR = 8;
        private const int MAX_SENTINEL_APPLIED_PER_YEAR = 42;
        // Q1000 Limits
        private const int MAX_Q1000_DAYTYPES = 7;
        private const int MAX_Q1000_HOLTYPES = 2;
        private const int MAX_Q1000_HOL_PER_YEAR = 22;
        private const int MAX_Q1000_RATES = 7;
        private const int MAX_Q1000_OUTPUTS = 16;
        private const int MAX_Q1000_SEASONS_PER_SCHED = 255;
        // Centron, Vectron, Quantum, Fulcrum Limits
        private const int MAX_CENTRON_DAYTYPES = 3;
        private const int MAX_CENTRON_HOLTYPES = 1;
        private const int MAX_CENTRON_RATES = 4;
        private const int MAX_CENTRON_OUTPUTS = 4;
        private const int MAX_CENTRON_SEASONS = 8;

        // Miscellaneous Constants
        private const int DEFAULT_RATE_OUTPUTS_INDEX = 0;
        private const int MAX_SWPT_TIME = 1439;
        private const int MIN_REG_DAYTYPES = 1;
        private const int MAX_REG_DAYTYPES = 7;
        private const int MIN_HOL_DAYTYPES = 1;
        private const int MAX_HOL_DAYTYPES = 2;
        private const int MIN_RATES = 0;
        private const int MAX_RATES = 7;
        private const int MIN_OUTPUTS = 0;
        private const int MAX_OUTPUTS = 16;
        private const int MIN_START_YEAR = 2000;
        private const int MAX_START_YEAR = 2050;
        private const int NO_RESTRICTION = int.MaxValue;
        private const int HOURS_IN_DAY = 24;
        private const int SAVE_PROGRESS_STEPS = 8;
        private const string RESOURCE_FILE_PROJECT_STRINGS =
                                    "Itron.Metering.TOU.Properties.Resources";

    #endregion Constants

    #region Member Variables

        /// <summary>
        /// Array that will have 7 strings to represent each of the 
        /// seven days of the week.  The week starts on Sunday so index zero
        /// will represent Sunday.  The strings contained within the array will
        /// be the day type of that particular day (i.e. Sunday, Saturday, Weekday)
        /// </summary>
        protected string[] m_astrTypicalWeek;

        /// <summary>
        /// StringCollection that contians the names of all of the normal days
        /// that are available in the TOU Schedule (i.e. Sunday, Weekday)
        /// </summary>
        protected List<string> m_astrNormalDays;

        /// <summary>
        /// StringCollection that holds the names of all the types of holidays
        /// that are available in the TOU Schedule (i.e. Holiday Type 1)
        /// </summary>
        protected List<string> m_astrHolidays;

        /// <summary>
        /// StringCollection that holds the names of all of the Rates available
        /// </summary>
        protected List<string> m_astrRates;

        /// <summary>
        /// Array that holds the names of all available outputs
        /// </summary>
        protected List<string> m_astrOutputs;

        /// <summary>
        /// Collection of year objects that will hold one year object for 
        /// each of the years in the TOU Schedule.  This collection will be sorted
        /// by year
        /// </summary>
        protected CYearCollection m_colYears;

        /// <summary>
        /// Collection of season objects that will hold one season object for 
        /// each of the available seasons in a TOU Schedule
        /// </summary>
        protected CSeasonCollection m_colSeasons;

        /// <summary>
        /// Collecton of pattern objects that will hold one pattern object for
        /// each of the available patterns in a TOU Schedule 
        /// </summary>
        protected CPatternCollection m_colPatterns;

        /// <summary>
        /// StringCollection of the names of the devices supported
        /// for the current TOU Schedule.
        /// </summary>
        protected List<string> m_astrSupportedDevices;

        /// <summary>
        /// Array of the names of all of the possible device types supported
        /// by this version of the TOU File.  This may be a superset
        /// of the device types supported by any given instance of the TOU Schedule.
        /// </summary>
        protected string[] m_astrDeviceTypeNames =  {
                            CTOUSchedule.Q1000_DEVICE,
                            CTOUSchedule.SENTINEL_ADV_DEVICE,
                            CTOUSchedule.SENTINEL_BAS_DEVICE,
                            CTOUSchedule.VECTRON_DEVICE,
                            CTOUSchedule.CENTRON_DEVICE,
                            CTOUSchedule.CENTRON_MONO_DEVICE,
                            CTOUSchedule.CENTRON_POLY_DEVICE,
                            CTOUSchedule.FULCRUM_DEVICE,
                            CTOUSchedule.QUANTUM_DEVICE,
                            CTOUSchedule.DMTMTR200_DEVICE };

        /// <summary>
        /// Variable used to represent the xml file
        /// </summary>
        protected System.Xml.XmlDocument m_xmldomSchedule;

        /// <summary>
        /// Variable used to represent the file name of the current schedule
        /// </summary>
        protected string m_strFileName;

        /// <summary>
        /// Variable used to represent the TOU name of the current schedule
        /// </summary>
        protected string m_strTOUName;

        /// <summary>
        /// Variable used to represent the TOU ID for the current schedule
        /// </summary>
        protected int m_intTOUID;

        /// <summary>
        /// Variable used to represent the Export Version used in FCS Export.
        /// </summary>
        protected int m_intTOUExportCount;

        /// <summary>
        /// variable used to represent the Start Year for the current schedule
        /// </summary>
        protected int m_intStartYear;

        /// <summary>
        /// Variable used to represent the Duration in years for the current schedule
        /// </summary>
        protected int m_intDuration;

        /// <summary>
        /// Variable used to represent the the Description for the current schedule
        /// </summary>
        protected string m_strDescription;

        /// <summary>
        /// Variable used to represent the Date Created for the current schedule
        /// </summary>
        protected DateTime m_dtDateCreated;

        /// <summary>
        /// Variable used to represent the Date Modified for the current schedule
        /// </summary>
        protected DateTime m_dtDateModified;

        /// <summary>
        /// Variable used to represent the applied Holiday List ID 
        /// for the current schedule
        /// </summary>
        protected int m_intHolidayListID;

        /// <summary>
        /// Variable used to write the TOU Schedule out to file
        /// </summary>
        protected XmlTextWriter m_xmlWriter;

        /// <summary>
        /// Variable used to store whether to use a specific 
        /// culture format with dates
        /// </summary>
        protected bool m_blnUseInvariant;

        /// <summary>
        /// Variable to hold what culture information to use with date formating
        /// </summary>
        protected IFormatProvider m_CultureFormat;

        /// <summary>
        /// Variable to hold the product name for file writing purposes
        /// </summary>
        protected String m_strProductName;

        /// <summary>
        /// Variable to hold the product version for file writing purposes
        /// </summary>
        protected String m_strVersionNumber;

        private ResourceManager m_rmStrings
            = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, typeof(Itron.Metering.TOU.CTOUSchedule).Assembly);

   #endregion Member Variables

        #region Events

        /// <summary>
        ///     Event inherited from the IProgressable interface
        /// </summary>
        public event ShowProgressEventHandler ShowProgressEvent;

        /// <summary>
        ///     Event inherited from the IProgressable interface
        /// </summary>
        public event HideProgressEventHandler HideProgressEvent;

        /// <summary>
        ///     Event inherited from the IProgressable interface
        /// </summary>
        public event StepProgressEventHandler StepProgressEvent;

        #endregion

    #region Public Methods

        /// <summary>
        /// Constructor to create an instacnce to the TOU Schedule class.  Opens
        /// the TOU Schedule passed in as a parameter.  Starts to fill in the 
        /// collections to hold the schedule information.  If there is a problem 
        /// with loading the XmlDocument an XmlException will be thrown
        /// </summary>
        /// <param name="strFileName">
        /// Represents the file name of the schedule that will be opened.
        /// </param>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// </code>
        /// </example>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/15/06 rrr N/A	 N/A	Creation of class  
        public CTOUSchedule(string strFileName)
        {
            //set the File Name and create and load the XmlDocument to hold 
            //the xml file
            m_strFileName = strFileName;
            m_xmldomSchedule = new XmlDocument();
            m_xmldomSchedule.Load(m_strFileName);

            m_intHolidayListID = 0;

            //Build the arrays and collections            
            BuildSupportedDevices();
            BuildDayTypes();
            BuildTypicalWeek();
            BuildRates();
            BuildOutputs();
            BuildPatterns();
            BuildSeasons();
            GetTOUInformation();
            BuildYears();

        }// CTouSchedule

        /// <summary>
        /// Default Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/06 KRC 7.36.00 
        //
        protected CTOUSchedule()
        {
        }

        /// <summary>
        /// Checks if the given device name is supported by the TOU Schedule
        /// </summary>
        /// <param name="strDeviceName">
        /// The name of the device to check for
        /// </param>
        /// <returns>
        /// true if the device is supported and false otherwise
        /// </returns>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// bool blnSupported = IsSupported("CENTRON");
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 10/12/06 mcm 7.35.04 59,66  Support both versions of the CENTRON Poly string
        /// 
        public bool IsSupported(string strDeviceName)
        {
            //Search the Supported Devices array for the given name and 
            //return true if found
            for (int intCount = 0; intCount < m_astrSupportedDevices.Count;
                intCount++)
            {
                if (strDeviceName == m_astrSupportedDevices[intCount])
                {
                    return true;
                }
               else if(((CENTRON_POLY_DEVICE == strDeviceName) ||
                        (CENTRON_POLY_DEVICE_OBSOLETE == strDeviceName)) &&
                       ((CENTRON_POLY_DEVICE == m_astrSupportedDevices[intCount]) ||
                        (CENTRON_POLY_DEVICE_OBSOLETE == m_astrSupportedDevices[intCount])))
                {
                    // Depending on the version of the msxml DOM object the file was
                    // saved with, the CENTRON (V&I) string can have 2 formats. 
                    // Support both
                    return true;
                }
            }

            //If name was not found then return false
            return false;
        }// IsSupported

        /// <summary>
        /// Used to take in the name of a DayType and get the Type and Name for
        /// that DayType
        /// </summary>
        /// <param name="strName">
        /// The name of the DayType to look up
        /// </param>
        /// <returns>
        /// A CDayType object that will have the Type and Index of the DayType.
        /// If the oject is not found then a null object will be returned.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        public CDayType GetDayType(string strName)
        {
            //Search the Normal Day List for the given name
            for (int intCount = 0; intCount < m_astrNormalDays.Count; intCount++)
            {
                if (strName == m_astrNormalDays[intCount])
                {
                    return new CDayType(eDayType.NORMAL, intCount);
                }
            }

            //Search the holiday list for the given name
            for (int intCount = 0; intCount < m_astrHolidays.Count; intCount++)
            {
                if (strName == m_astrHolidays[intCount])
                {
                    return new CDayType(eDayType.HOLIDAY, intCount);
                }
            }

            return null;
        }// GetDayType

        /// <summary>
        /// This method adds the day type to either the normal day or 
        /// holiday array depending on the day type.  If the array 
        /// holding these values is not large enough, it will be extended
        /// to hold the new day.  
        /// </summary>
        /// <param name="day">
        /// Represents the day type to be added.
        /// </param>
        /// <param name="strName">
        /// Represents the name of the day type to be added.
        /// </param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/28/06 ach N/A	 N/A	Moved from Day Types Form.cs 
        public void AddDayType(CDayType day, string strName)
        {

            // If the day to be added is normal
            if (eDayType.NORMAL == day.Type)
            {
                if (day.Index < NormalDays.Count)
                {
                    m_astrNormalDays[day.Index] = strName;
                }
                else if (day.Index == NormalDays.Count)
                {
                    m_astrNormalDays.Insert(day.Index, strName);
                }

            }

            // otherwise the day type is a holiday
            else
            {
                if (day.Index < Holidays.Count)
                {
                    m_astrHolidays[day.Index] = strName;
                }
                else if (day.Index == Holidays.Count)
                {
                    m_astrHolidays.Insert(day.Index, strName);
                }
            }
           

        }// AddDayType

        /// <summary>
        /// This method checks to see if the given season is used within 
        /// the schedule.
        /// </summary>
        /// <param name="nSeasonID">
        /// Represents the ID of the season to be searched for.
        /// </param>
        /// <returns>
        /// True if the season is being used, false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/30/06 mah N/A	 N/A	Added method 
        public Boolean IsSeasonUsed(int nSeasonID)
        {
            // First get the pattern ID from the name

            foreach (CYear objYear in m_colYears)
            {
                foreach (CEvent objEvent in objYear.Events )
                {
                    if (eEventType.SEASON == objEvent.Type)
                    {
                        if (nSeasonID == Seasons[objEvent.Index].ID)
                            return true;
                    }
                }
            }

            // If we got to this point then the pattern ID is not in use

            return false;
        }// IsPatternUsed

        /// <summary>
        /// This method checks to see if the given pattern is used within 
        /// the schedule.
        /// </summary>
        /// <param name="nPatternID">
        /// Represents the ID of the pattern to be searched for.
        /// </param>
        /// <returns>
        /// True if the pattern is being used, false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/30/06 mah N/A	 N/A	Added method 
        public Boolean IsPatternUsed(int nPatternID)
        {
            // First get the pattern ID from the name

            foreach (CSeason season in m_colSeasons)
            {
                foreach (Int16 nNormalDayPattern in season.NormalDays)
                {
                    if (nPatternID == nNormalDayPattern)
                        return true;
                }

                foreach (Int16 nHolidayPattern in season.Holidays)
                {
                    if (nPatternID == nHolidayPattern)
                        return true;
                }
            }

            // If we got to this point then the pattern ID is not in use

            return false;
        }// IsPatternUsed

        /// <summary>
        /// This method checks to see if the given rate is used within 
        /// the schedule.
        /// </summary>
        /// <param name="strRate">
        /// Represents the name of the rate to be searched for.
        /// </param>
        /// <returns>
        /// True if the Rate is being used, false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/28/06 ach N/A	 N/A	Moved from Rates Form.cs 
        public Boolean IsRateUsed(string strRate)
        {
            Boolean result = false;
            CPatternCollection patterns = Patterns;
            List<string> rates = Rates;

            foreach (CPattern pattern in patterns)
            {
                CSwitchPointCollection switchpoints = pattern.SwitchPoints;
                
                foreach (CSwitchPoint switchpoint in switchpoints)
                {
                    if (eSwitchPointType.RATE == switchpoint.SwitchPointType)
                    {
                        if (strRate == rates[switchpoint.RateOutputIndex])                        
                        {
                            return true;
                        }
                    }
                }
            }

            return result;
        }// IsRateUsed

        /// <summary>
        /// This method checks to see if the given Output is used within 
        /// the schedule.
        /// </summary>
        /// <param name="strOutput">
        /// Represents the name of the Output to be searched for.
        /// </param>
        /// <returns>
        /// True if the Output is being used, false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/28/06 ach N/A	 N/A	Moved from Output Form.cs 
        public Boolean IsOutputUsed(string strOutput)
        {
            Boolean result = false;

            CPatternCollection patterns = Patterns;
            List<string> outputs = Outputs;

            foreach (CPattern pattern in patterns)
            {
                CSwitchPointCollection switchpoints = pattern.SwitchPoints;

                foreach (CSwitchPoint switchpoint in switchpoints)
                {
                    if (eSwitchPointType.OUTPUT == switchpoint.SwitchPointType)
                    {
                        if (strOutput == outputs[switchpoint.RateOutputIndex])
                        {
                            return true;
                        }
                    }
                }
            }

            return result;

        }// IsOutputUsed

        /// <summary>
        /// This method returns whether or not a normal day type is used
        /// in the schedule.
        /// </summary>
        /// <param name="strName">
        /// Represents the name of the normal day type.
        /// </param>
        /// <returns>
        /// True if the normal day type is used, false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 ach N/A	 N/A	Added support for searching day type use
        public Boolean IsNormalDayTypeUsed(string strName)
        {
            Boolean result = false;

            for (int nCount = 0; nCount < TypicalWeek.Length; nCount++)
            {
                if (strName == TypicalWeek[nCount])
                {
                    result = true;
                }
            }

            return result;
        } // IsNormalDayTypeUsed              

        /// <summary>
        /// This method returns whether or not a holiday day type is used
        /// in the schedule.
        /// </summary>
        /// <param name="nIndex">
        /// Represents the index of the holiday type.
        /// </param>
        /// <returns>
        /// True if the holiday day type is used, false otherwise.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 ach N/A	 N/A	Added support for searching day type use
        public Boolean IsHolidayTypeUsed(int nIndex)
        {
            Boolean result = false;

            foreach (CYear year in Years)
            {
                foreach (CEvent yearEvent in year.Events)
                {
                    if (eEventType.HOLIDAY == yearEvent.Type)
                    {
                        if (nIndex == yearEvent.Index)
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        } // IsHolidayTypeUsed

        /// <summary>
        /// This method clears all the holiday events from every year
        /// in the schedule.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/28/06 ach N/A	 N/A	Moved from frmMain.cs 
        public void ClearHolidayEvents()
        {
            int end = StartYear + Duration;

            // For every year in the schedule get the events
            foreach (CYear year in Years)
            {
               
                CEventCollection events = year.Events;

                // go through all the events in this year
                for (int intCount = 0; intCount < events.Count; intCount++)
                {
                    // If the event is a holiday set it to null
                    if (eEventType.HOLIDAY == events[intCount].Type)
                    {
                        events.RemoveAt(intCount);
                        intCount--;
                    }
                }
            }

        }// ClearHolidayEvents

        /// <summary>
        /// This method safely deletes a season from the TOU schedule by 
        /// clearing all occurrences of associated season change events 
        /// and by deleting the season entry itself
        /// </summary>
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/03/06 mah N/A	 N/A	Added method 
        /// </remarks>
        public void DeleteSeason(int nSeasonIndex)
        {
            // Start by validating the season index

            if (nSeasonIndex < 0 || nSeasonIndex >= Seasons.Count)
            {
                throw (new ArgumentOutOfRangeException());
            }

            // Season change dates are identified by the event type
            // and the season index.  Go through each year in the TOU 
            // schedule hunting for season events with the same index
            // as the given season and remove the event

            int nSeasonID = Seasons[nSeasonIndex].ID;

            int end = StartYear + Duration;

            foreach (CYear year in Years)
            {
                // go through all the events in this year
                
                for (int nEventIndex = 0; nEventIndex < year.Events.Count; nEventIndex++)
                {
                    // If the event is a holiday set it to null
                    if ((eEventType.SEASON == year.Events[nEventIndex].Type) &&
                        (year.Events[nEventIndex].Index == nSeasonIndex))
                    {
                        year.Events.RemoveAt(nEventIndex);
                        nEventIndex--;
                    }
                }
            }

            // next remove the season itself from the season list

            Seasons.RemoveAt(nSeasonIndex);           

            // we have to decrement the indexing for the events to reflect
            // that a season definition has been removed

            foreach (CYear year in Years)
            {
                for (int nEventIndex = 0; nEventIndex < year.Events.Count; nEventIndex++)
                {
                    if (eEventType.SEASON == year.Events[nEventIndex].Type &&
                        nSeasonIndex < year.Events[nEventIndex].Index)
                    {
                        year.Events[nEventIndex].Index--;
                    }
                }
            }

        }// DeleteSeason

        /// <summary>
        /// Given a valid rate name this method returns the index of the
        /// rate
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mah N/A	 N/A	Added method 
        public int GetRateIndex(String sRateName)
        {
            int nRateIndex = 0;

            foreach (String sCurrentRate in m_astrRates)
            {
                if (sCurrentRate == sRateName)
                    return nRateIndex;
                else
                    nRateIndex++;
            }

            // If we go to this point then we did not find the given rate name
            // and we need to throw an exception indicating that the rate name
            // was out of range

            throw (new ArgumentOutOfRangeException());
        } // GetRateIndex

        /// <summary>
        /// This method returns an index to the season that contains the
        /// given target date
        /// </summary>
        /// <param name="dateTarget">
        /// The target date
        /// </param>
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/30/06 mah 8.00	 N/A	Added method
		/// 11/26/07 mrj 9.00.27 3447	Fixed issue with finding the season
        /// </remarks>
        public int GetSeasonIndex(DateTime dateTarget)
        {
            int nSeasonIndex = -1;  // the default return value if we could not find the appropriate season

            CYear currentYear = Years[ Years.SearchYear( dateTarget.Year )];

            // Search for the correct season

            Boolean boolSeasonFound = false;

            foreach (CEvent touEvent in currentYear.Events)
            {
                // Since there can be multiple seasons per year, all we need to do is find
                // the season change date that immediately preceeds the target date.  We can
                // do this by trolling through the event list and recording each season index
                // until we pass the date.  The last recorded season index will be the correct
                // one.

                if (eEventType.SEASON  == touEvent.Type)
                {
                    if ((touEvent.Date.Month < dateTarget.Month) ||
						((touEvent.Date.Month == dateTarget.Month) && (touEvent.Date.Day <= dateTarget.Day)))
                    {
                        boolSeasonFound = true;
                        nSeasonIndex = touEvent.Index;
                    }
                }
            }

            // If we didn't find the correct season start date then we probably
            // have a start date that preceeds any of the season start dates in 
            // the current year.  We need to use the last season start date of the
            // prior year instead.

            if (!boolSeasonFound && ( StartYear < dateTarget.Year))
            {
                CYear priorYear = Years[ Years.SearchYear(dateTarget.Year - 1)];

                foreach (CEvent touEvent in priorYear.Events)
                {
                    // By setting the season index each time we find another season
                    // we guarantee that the index will be set for the last season
                    // start date of the year.

                    if (touEvent.Type == eEventType.SEASON)
                    {
                        nSeasonIndex = touEvent.Index;
                        boolSeasonFound = true;
                    }
                }
            }

            return nSeasonIndex;
        }

        /// <summary>
        /// Given a valid output name this method returns the index of the
        /// rate
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mah N/A	 N/A	Added method 
        public int GetOutputIndex(String sOutputName)
        {
            int nOutputIndex = 0;

            foreach (String sCurrentOutput in m_astrOutputs)
            {
                if (sCurrentOutput == sOutputName)
                    return nOutputIndex;
                else
                    nOutputIndex++;
            }

            // If we go to this point then we did not find the given rate name
            // and we need to throw an exception indicating that the rate name
            // was out of range

            throw (new ArgumentOutOfRangeException());
        } // GetOutputIndex

        /// <summary>
        /// Enables saving a schedule to the same location.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 ach N/A	 N/A	Added method 
        /// 10/07/08 jrf 9.50.16        Adding showing progress during a save.
        public void Save()
        {
            OnShowProgress(new ShowProgressEventArgs(1, SAVE_PROGRESS_STEPS, "", m_rmStrings.GetString("SAVING_TOU_SCHEDULE")));

            WriteTOUInformation(m_strFileName);

            OnHideProgress(new EventArgs());
        } // Save

        /// <summary>
        /// Enables saving a schedule to a given location
        /// </summary>
        /// <param name="strFileName">
        /// Represents the location to be saved to.
        /// </param>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 ach N/A	 N/A	Added method 
        public void SaveAs(string strFileName)
        {
            m_strFileName = strFileName;
            WriteTOUInformation(strFileName);
        } // SaveAs

		/// <summary>
		/// This method finds a pattern for a given date.
		/// </summary>
		/// <param name="dtDate">The date for which to find the pattern.</param>
		/// <returns>CPattern for the given date, "null" if date not found.</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  10/05/07 mrj 9.00.00		Created
		//  
		public CPattern FindPattern(DateTime dtDate)
		{
			CPattern Pattern = null;

			//Figure out what season we are in
			int iSeasonIndex = GetSeasonIndex(dtDate);
			if (iSeasonIndex != -1)
			{
				CSeason Season = m_colSeasons[iSeasonIndex];

				//Figure out if this date is a holiday or not
				int iHolidayIndex = IsHoliday(dtDate);
				if (iHolidayIndex != -1)
				{
					//This is a holiday so get the pattern
					int iPatternIndex = m_colPatterns.SearchID(Season.Holidays[iHolidayIndex]);
					Pattern = m_colPatterns[iPatternIndex];					
				}
				else
				{
					//This is not a holiday, so get the day from the typical week
					string strSelectedDayType = m_astrTypicalWeek[(int)dtDate.DayOfWeek];

					int iDayTypeIndex = 0;
					foreach (String strDayTypeName in NormalDays)
					{
						if (string.Compare(strDayTypeName, strSelectedDayType) == 0)
						{
							//We found the daytype so use the index to find the pattern
							int iPatternIndex = m_colPatterns.SearchID(Season.NormalDays[iDayTypeIndex]);
							Pattern = m_colPatterns[iPatternIndex];
							break;
						}
						else
						{
							iDayTypeIndex++;
						}
					}
				}
			}

			return Pattern;
		}

        /// <summary>
        /// This static method gets the lowest unused TOU Schedule ID.
        /// </summary>
        /// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/19/08 jrf 9.50.05	       Created.  
        public static ushort GetNextTOUScheduleID()
        {
            List<int> lstIDs = GetScheduleIDs();
            int iNextScheduleID = 1;

            while (lstIDs.Contains(iNextScheduleID))
            {
                iNextScheduleID++;
            }

            return (ushort)iNextScheduleID;
                       
        }

        /// <summary>
        /// This static method checks to see if the given TOU schedule ID exists.
        /// </summary>
        /// <param name="usTOUID">The TOU schedule ID to check.</param>
        /// <returns>A bool indicating if the TOU Schedule exists.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/19/08 jrf 9.50.05	       Created. 
        public static bool TOUScheduleIDExists(ushort usTOUID)
        {
            List<int> lstIDs = GetScheduleIDs();

            return lstIDs.Contains(usTOUID);

        }

        /// <summary>
        /// This method gets the next season start date after the given time.
        /// </summary>
        /// <param name="CurrentTime">The time to use to determine the next season.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/13/14 jrf 4.00.27 TC59708 Created.
        public DateTime GetNextSeasonStartDate(DateTime CurrentTime)
        {
            DateTime NextStartDate = DateTime.MinValue;

            foreach (DateTime item in SeasonStartDates)
            {
                if (item > CurrentTime)
                {
                    NextStartDate = item;
                    break;
                }
            }

            return NextStartDate;
        }

        /// <summary>
        /// This method gets the next DST change event after the given time.
        /// </summary>
        /// <param name="CurrentTime">The time to use to determine the next DST transition date.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/16/14 jrf 4.00.28 TC59708 Created.
        public CEvent GetNextDSTEvent(DateTime CurrentTime)
        {
            CEvent NextDSTEvent = null;

            foreach (CEvent DSTEvent in DSTEvents)
            {
                if (DSTEvent.Date > CurrentTime)
                {
                    NextDSTEvent = DSTEvent;
                    break;
                }
            }

            return NextDSTEvent;
        }

    #endregion Public Methods

    #region Properties

        /// <summary>
        /// Property to get the Pattern Collection
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// CPatternCollection coll = sched.Patterns;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        public CPatternCollection Patterns
        {
            get
            {
                return m_colPatterns;
            }
        }//Patterns


        /// <summary>
        /// Property to get the Season Collection
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// CSeasonCollection coll = sched.Seasons;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        public CSeasonCollection Seasons
        {
            get
            {
                return m_colSeasons;
            }
        }// Seasons


        /// <summary>
        /// Property to get the Year Collection
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// CYearCollection coll = sched.Years;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        public CYearCollection Years
        {
            get
            {
                return m_colYears;
            }
        }//Years


        /// <summary>
        /// Property to get and set the Normal Days String COllection
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// StringCollection normal = sched.NormalDays;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 07/25/06 ach N/A     N/A    Modified to use String Collection
        public List<string> NormalDays
        {
            get
            {
                return m_astrNormalDays;
            }
            set
            {
                m_astrNormalDays = value;
            }
        }//NormalDays


        /// <summary>
        /// Property to get and set the Holidays String Collection
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// StringCollection holidays = sched.Holidays;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 07/25/06 ach N/A     N/A    modified to use String Collection
        public List<string> Holidays
        {
            get
            {
                return m_astrHolidays;
            }
            set
            {
                m_astrHolidays = value;
            }
        }//Holidays


        /// <summary>
        /// Property to get and set the Rates String Collection
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// StringCollection rates = sched.Rates;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 07/24/06 ach N/A     N/A    modified to use String Collection
        public List<string> Rates
        {
            get
            {
                return m_astrRates;
            }
            set
            {
                m_astrRates = value;
            }
        }//Rates


        /// <summary>
        /// Property to get and set the Outputs String Collection
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// StringCollection outputs = sched.Outputs;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 07/24/06 ach N/A     N/A    modified to use String Collection
        public List<string> Outputs
        {
            get
            {
                return m_astrOutputs;
            }
            set
            {
                m_astrOutputs = value;
            }
        }//Outputs


        /// <summary>
        /// Property to get the Typical Week array
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// string[] week = sched.TypicalWeek;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 06/28/06 ach N/A     N/A    Added set property
        public string[] TypicalWeek
        {
            get
            {
                return m_astrTypicalWeek;
            }
            set
            {
                m_astrTypicalWeek = value;
            }
        }//TypicalWeek


        /// <summary>
        /// Property to get and set the Supported Devices StringCollection
        /// </summary>
        /// <example>
        /// <code>
        /// CTOUSchedule sched = new CTOUSchedule("C:\\Documents\\0001test.xml");
        /// StringCollection Devices = sched.SupportedDevices;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 07/25/06 ach N/A     N/A    Modified to use StringCollection
        public List<string> SupportedDevices
        {
            get
            {
                return m_astrSupportedDevices;
            }
            set
            {
                m_astrSupportedDevices = value;
            }
        }//SupportedDevices


        /// <summary>
        /// Property to get a list of all of the currently defined 
        /// device types.  This list does not indicate whether a given
        /// device type is supported by a given instance of the schedule
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mah N/A	 N/A	Added this method  
        public string[] DeviceTypeNames
        {
            get
            {
                return m_astrDeviceTypeNames;
            }
        }//DeviceTypeNames  
        
        
        /// <summary>
        /// Property return the file name of the TOU schedule (minus the path)
        /// and to set the file name of the TOU schedule.
        /// </summary>
        public String FileName
        {
            get
            {
                return Path.GetFileName( m_strFileName );
            }
            set
            {
                m_strFileName = value;
            }
        }//FileName


        /// <summary>
        /// Property to return the path in which TOU schedule currently resides.
        /// </summary>
        public String DirectoryName
        {
            get
            {
                return Path.GetDirectoryName(m_strFileName);
            }
        }//DirectoryName


        /// <summary>
        /// Property to get and set the Date Created
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                return m_dtDateCreated;
            }
            set
            {
                m_dtDateCreated = value;
            }
        }//DateCreated


        /// <summary>
        /// Property to get and set the Date Modified
        /// </summary>
        public DateTime DateModified
        {
            get
            {
                return m_dtDateModified;
            }
            set
            {
                m_dtDateModified = value;
            }
        }//DateModified


        /// <summary>
        /// Property to get and setthe TOU Name
        /// </summary>
        public string TOUName
        {
            get
            {
                return m_strTOUName;
            }
            set
            {
                m_strTOUName = value;
            }
        }//TOUName


        /// <summary>
        /// Property to get and setthe TOU ID
        /// </summary>
        public int TOUID
        {
            get
            {
                return m_intTOUID;
            }
            set
            {
                m_intTOUID = value;
            }
        }//TOUID

        /// <summary>
        /// Propert to get and set the Export Version for FCS Export
        /// </summary>
        public int TOUExportCount
        {
            get
            {
                return m_intTOUExportCount;
            }
            set
            {
                m_intTOUExportCount = value;
            }
        }

        /// <summary>
        /// Property to get and set the Start Year
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/07 ach 8.0     167    Added set property
        public int StartYear
        {
            get
            {
                Years.Sort();

                if (Years.Count > 0)
                {
                    m_intStartYear = Years[0].Year;
                    return m_intStartYear;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (Years[0].Year < value)
                {
                    while (Years[0].Year < value)
                    {
                        Years.RemoveAt(0);
                    }
                }

            }
        }//StartYear

        /// <summary>
        /// Gets the year that the TOU schedule ends
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/29/11 RCG 2.50.17        Created

        public int EndYear
        {
            get
            {
                int iYear = 0;

                if (Years.Count > 0)
                {
                    Years.Sort();

                    iYear = Years[Years.Count - 1].Year;
                }

                return iYear;
            }
        }


        /// <summary>
        /// Property to get and set the Duration
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/07 ach 8.0     167    Added set method.
        /// 10/08/08 jrf 9.50.16        Fixed error when setting duration below
        ///                             the current years count.
        public int Duration
        {
            get
            {
                Years.Sort();

                if (Years.Count > 0)
                {
                    m_intDuration = (Years[Years.Count - 1].Year -
                                                        Years[0].Year) + 1;
                    return m_intDuration;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                
                // The only time the duration should be set is
                // when creating a new schedule
                Years.Sort();

                // The duration has been extended
                if (Years.Count < value)
                {
                    for (int nCount = Years.Count; nCount < value; nCount++)
                    {
                        Years.Add(new CYear(Years[nCount - 1].Year + 1, new CEventCollection()));
                    }
                }

                else if (Years.Count > value)
                {
                    for (int nCount = value; nCount <= Years.Count; nCount++)
                    {
                        Years.RemoveAt(Years.Count - 1);
                    }
                }

                m_intDuration = value;
                

            }
        }//Duration


        /// <summary>
        /// Property to get and set the Description
        /// </summary>
        public string Description
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }//Description


        /// <summary>
        /// Property to get and set the Holiday List ID
        /// </summary>
        public int HolidayListID
        {
            get
            {
                return m_intHolidayListID;
            }
            set
            {
                m_intHolidayListID = value;
            }
        }//HolidayListID


        /// <summary>
        /// Property to get the culture used
        /// </summary>
        public IFormatProvider Culture
        {
            get
            {
                return m_CultureFormat;
            }
        } // Culture


        /// <summary>
        /// Property to get whether or not rates in the schedule overlap
        /// </summary>
        public Boolean RatesOverlap
        {
            get
            {

                foreach (CPattern pattern in Patterns)
                {
                    CSwitchPoint previousSwitchpoint = null;
                    pattern.SwitchPoints.Sort();

                    foreach (CSwitchPoint switchpoint in pattern.SwitchPoints)
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
                }

                return false;
            }

        } // RatesOverlap


        /// <summary>
        /// Property to get whether or not all patterns in the schedule cover all 24 hours
        /// </summary>
        public Boolean FullCoverage
        {
            get
            {

                // Check each pattern in the schedule
                foreach (CPattern pattern in Patterns)
                {
                    TimeSpan tsCoverage = new TimeSpan(0, 0, 0);

                    foreach (CSwitchPoint switchpoint in pattern.SwitchPoints)
                    {
                        TimeSpan tsDiff;

                        // We need to the 24 hour coverage to be fulfilled by rates only
                        if (eSwitchPointType.RATE == switchpoint.SwitchPointType)
                        {
                            tsDiff = switchpoint.TimeOfStop.Subtract(switchpoint.TimeOfStart);
                            
                            // Since we have already checked to make sure the end time is after the
                            // start time, if the hours are 0 or negative we know that this is the end of 
                            // the day (0:00) subtracting the start time to get a non positive number
                            // By adding 24 hours to the time span's hours we will get the real difference
                            if (tsDiff.Hours <= 0)
                            {
                                tsDiff = new TimeSpan(tsDiff.Hours + HOURS_IN_DAY, tsDiff.Minutes, tsDiff.Seconds);
                            }

                            tsCoverage = new TimeSpan(tsCoverage.Days + tsDiff.Days, tsCoverage.Hours + tsDiff.Hours, tsCoverage.Minutes + tsDiff.Minutes, tsCoverage.Seconds + tsDiff.Seconds);
                                                        
                        }
                    }

                    // If 24 hours aren't covered then return false
                    // Note that if a meter comes around that requires 24 hour coverage
                    // with overlapping rates then a change in logic will be needed in 
                    // this property to ensure 24 hours are all covered as this checks
                    // the time spans of each switchpoint, which if they overlap could
                    // be the same time, thus not covering all 24 hours.
                    if (1 > tsCoverage.Days)
                    {
                        return false;
                    }
                }

              return true;
            }

            

        } // FullCoverage


        /// <summary>
        /// Property to get and set the Product Name for what is using the schedule
        /// </summary>
        public String ProductName
        {
            get
            {
                return m_strProductName;
            }
            set
            {
                m_strProductName = value;
            }
        } // ProductName


        /// <summary>
        /// Property to get and set the version of the product using the schedule
        /// </summary>
        public String ProductVersion
        {
            get
            {
                return m_strVersionNumber;
            }
            set
            {
                m_strVersionNumber = value;
            }
        } // ProductVersion

        /// <summary>
        /// Property to get all of the season start dates from the schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/14 jrf 4.00.24 TC59708 Created.
        public List<DateTime> SeasonStartDates
        {
            get
            {
                List<DateTime> StartDates = new List<DateTime>();

                foreach (CYear itemYear in Years)
                {
                    foreach (CEvent itemEvent in itemYear.Events)
                    {
                        if (eEventType.SEASON == itemEvent.Type)
                        {
                            StartDates.Add(itemEvent.Date);
                        }
                    }
                }

                StartDates.Sort();

                return StartDates;
            }
        }

        /// <summary>
        /// Property to get all of the DST change dates from the schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/16/14 jrf 4.00.28 TC59710 Created.
        public List<CEvent> DSTEvents
        {
            get
            {
                List<CEvent> DSTEventList = new List<CEvent>();

                foreach (CYear itemYear in Years)
                {
                    foreach (CEvent itemEvent in itemYear.Events)
                    {
                        if (eEventType.FROM_DST == itemEvent.Type
                            || eEventType.TO_DST == itemEvent.Type)
                        {
                            DSTEventList.Add(itemEvent);
                        }
                    }
                }

                DSTEventList.Sort();

                return DSTEventList;
            }
        }

        /// <summary>
        /// Property to get all of the Season change dates from the schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/14 jrf 4.00.28 TC59709 Created.
        public List<CEvent> SeasonChangeEvents
        {
            get
            {
                List<CEvent> SeasonChangeEventList = new List<CEvent>();

                foreach (CYear itemYear in Years)
                {
                    foreach (CEvent itemEvent in itemYear.Events)
                    {
                        if (eEventType.SEASON == itemEvent.Type)
                        {
                            SeasonChangeEventList.Add(itemEvent);
                        }
                    }
                }

                SeasonChangeEventList.Sort();

                return SeasonChangeEventList;
            }
        }

    #endregion Properties

    #region Private Methods

        /// <summary>
        /// Converts a given Season ID into the index of that season based on
        /// the Season Collection
        /// </summary>
        /// <param name="intSeasonID">
        /// The ID of the season to get the index of
        /// </param>
        /// <returns>
        /// The index of the Season ID based on the Season Collection
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        private int ConvertSeasonID(int intSeasonID)
        {
            //Get the Index from the season collection
            return m_colSeasons.SearchID(intSeasonID);
        }//ConvertSeasonID


        /// <summary>
        /// Takes a day type and an index to retrieve the name that matches
        /// these parameters.
        /// </summary>
        /// <param name="eType">
        /// The type of day to be looked up.  Tells whether to look in the
        /// Normal or Holiday collection
        /// </param>
        /// <param name="intIndex">
        /// The index to check in the collection to get the name 
        /// </param>
        /// <returns>
        /// The name of the day
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 02/05/07 ach 8.0     96     Added Try-Catch to attempt to reduce program crashes
        private string GetDayName(eDayType eType, int intIndex)
        {
            //If the type is normal get from normal array otherwise get from 
            //holiday array
            try
            {
                if (eDayType.NORMAL == eType)
                {
                    return m_astrNormalDays[intIndex];
                }
                else
                {
                    return m_astrHolidays[intIndex];
                }
            }
            catch
            {
                return null;
            }
        }//GetDayName


        /// <summary>
        /// Method to fill in the collection for the typical week from the 
        /// xml file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 06/02/06 ach N/A     N/A    Updated comments explaining GetDayName use
        private void BuildTypicalWeek()
        {
            //Local Variables
            XmlNodeList xmlnodelistWeeks = null;
            XmlNodeList xmlnodelistTypicalDays = null;
            XmlNode xmlnodeWeek = null;
            int intDayCount = 0;

            //Initialize the Typical Week Array
            m_astrTypicalWeek = new string[WEEKCOUNT];

            //Get a list of all nodes with Tag TypicalWeek which should only be 1
            xmlnodelistWeeks = m_xmldomSchedule.GetElementsByTagName(TYPICAL_WEEK);
            xmlnodeWeek = xmlnodelistWeeks.Item(xmlnodelistWeeks.Count - 1);

            //Get the child nodes of the Typical Week
            xmlnodelistTypicalDays = xmlnodeWeek.ChildNodes;

            //For each child node get the DayName represented by the Type and Index
            foreach (XmlNode xmlnodeDay in xmlnodelistTypicalDays)
            {
                if (xmlnodeDay.HasChildNodes)
                {
                    // Use GetDayName to fill in what kind of day this 
                    // particulary typical day will be
                    m_astrTypicalWeek[intDayCount] = GetDayName((eDayType)
                        int.Parse(xmlnodeDay.LastChild.PreviousSibling.InnerText),
                        int.Parse(xmlnodeDay.LastChild.InnerText));
                    intDayCount++;
                }
            }
        }//BuildTypicalWeek()


        /// <summary>
        /// Method to fill in the collection for the years from the xml file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class 
        /// 04/26/06 rrr N/A	 N/A	Added retrieving of holiday name from xml file
        ///								and setting of season name as season 
        private void BuildYears()
        {
            //Local variable to manipulate the XML file
            XmlNodeList xmlnodelistYears = null;
            XmlNodeList xmlnodelistChildren = null;
            XmlNodeList xmlnodelistEvents = null;
            XmlNode xmlnodeChild = null;

            //Local variables for objects with the namespace
            CEventCollection colEvents = null;

            //Local storage variables
            eEventType eType;
            int intIndex = 0;
            int intYear = 0;
            int intCount = 0;
            string strName = null;

            //Initialize the year collection 
            m_colYears = new CYearCollection();

            //Get all nodes with the Year tag
            xmlnodelistYears = m_xmldomSchedule.GetElementsByTagName(YEAR);

            //Go through each of the Year nodes
            foreach (XmlNode xmlnodeYear in xmlnodelistYears)
            {
                if (xmlnodeYear.HasChildNodes)
                {
                    //Get all of the child nodes for the current year node
                    xmlnodelistChildren = xmlnodeYear.ChildNodes;

                    //Go through the child nodes and pull out the information 
                    //needed for the Year object
                    for (intCount = 0; intCount < xmlnodelistChildren.Count;
                        intCount++)
                    {
                        xmlnodeChild = xmlnodelistChildren[intCount];
                        if (YEAR_NUMBER == xmlnodeChild.Name)
                        {
                            //Get the year of the current year node
                            intYear = int.Parse(xmlnodeChild.InnerText);
                        }
                        else if (APPLIED_HOLIDAY == xmlnodeChild.Name)
                        {
                            //If the event collection is not initialized then 
                            //initialize it
                            if (null == colEvents)
                            {
                                colEvents = new CEventCollection();
                            }

                            //Get all of the applied holiday nodes
                            xmlnodelistEvents = xmlnodeChild.ChildNodes;

                            //Go though each of the applied holidays
                            foreach (XmlNode xmlnodeEvent in xmlnodelistEvents)
                            {
                                //Set the type to Holiday
                                eType = eEventType.HOLIDAY;

                                //Get the name of the Holiday
                                strName = xmlnodeEvent.FirstChild.InnerText;

                                //Get the index 
                                intIndex = int.Parse(
                                    xmlnodeEvent.FirstChild.NextSibling.InnerText);

                                //Create the Event Object and add it to collection
                                colEvents.Add(CreateEvent(eType, intIndex,
                                    xmlnodeEvent.LastChild.InnerText, strName));
                            }
                        }
                        else if (APPLIED_SEASON == xmlnodeChild.Name)
                        {
                            //If the event collection is not initialized then 
                            //initialize it
                            if (null == colEvents)
                            {
                                colEvents = new CEventCollection();
                            }

                            //Get all the Applied Season nodes
                            xmlnodelistEvents = xmlnodeChild.ChildNodes;
                            foreach (XmlNode xmlnodeEvent in xmlnodelistEvents)
                            {
                                //Set the event type to Season
                                eType = eEventType.SEASON;

                                //Set season name
                                strName = "Season";

                                //get the index of the Season ID from the collection
                                intIndex = m_colSeasons.SearchID(int.Parse(
                                    xmlnodeEvent.FirstChild.InnerText));

                                //Create the Event Object and add it to collection
                                colEvents.Add(CreateEvent(eType, intIndex,
                                    xmlnodeEvent.LastChild.InnerText, strName));
                            }
                        }
                    }

                    //If all the nodes have been seen create the Year object
                    //and add it to the collection
                    if (xmlnodelistChildren.Count == intCount)
                    {
                        colEvents.Sort();
                        m_colYears.Add(new CYear(intYear, colEvents));
                        colEvents = null;
                    }
                }
            }
        }//BuildYears()


        /// <summary>
        /// Creates a new CEvent object based on the given info 
        /// </summary>
        /// <param name="eType">
        /// The eEventType for the CEvent
        /// </param>
        /// <param name="intIndex">
        /// The index of the CEvent
        /// </param>
        /// <param name="strDate">
        /// The date of the CEvent
        /// </param>
        /// <param name="strName">
        /// The name of the CEvent
        /// </param>
        /// <returns>
        /// A new CEvent object with the given info</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 04/26/06 rrr N/A	 N/A	Added string for the name of the event
        /// 06/02/06 ach N/A     N/A    Modified to use culture specific date 
        ///								formatting
        private CEvent CreateEvent(eEventType eType, int intIndex, string strDate,
            string strName)
        {
            // If an invariant is defined use that specified date format
            if (m_blnUseInvariant)
            {
                return new CEvent(DateTime.Parse(strDate, m_CultureFormat),
                                  eType, intIndex, strName);
            }

            // If  no invariant defined use the system to format the date
            return new CEvent(DateTime.Parse(strDate), eType, intIndex, strName);

            /* Old Code Temporarily Kept
            string[] astrDate;
            int intMonth = 0;
            int intDay = 0;
            int intYear = 0;

            //Get the date string and split it into the parts
            astrDate = strDate.Split('/');
            intMonth = int.Parse(astrDate[0]);
            intDay = int.Parse(astrDate[1]);
            intYear = int.Parse(astrDate[2]);

            //Return the new Event object
            return new CEvent(new DateTime(intYear, intMonth, intDay), 
                eType, intIndex, strName);
            */

        }//CreateEvent()


        /// <summary>
        /// Method to fill in the collection for the rates from the xml file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        private void BuildRates()
        {
            //Local Variables
            XmlNodeList xmlnodelistRates = null;

            //Get all of the Rate nodes
            xmlnodelistRates = m_xmldomSchedule.GetElementsByTagName(RATE);

            //Initialize the Rates array
            m_astrRates = new List<string>();

            //Get the name from each rate node and insert at the index
            foreach (XmlNode xmlnodeRate in xmlnodelistRates)
            {
                int intIndex = int.Parse(
                    xmlnodeRate.LastChild.PreviousSibling.InnerText);                
                m_astrRates.Add(xmlnodeRate.LastChild.InnerText);
            }
        }//BuildRates()


        /// <summary>
        /// Method to fill in the collection for the outputs from the xml file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        private void BuildOutputs()
        {
            //Local variable
            XmlNodeList xmlnodelistOutputs = null;

            //Get all output nodes and initialize the array
            xmlnodelistOutputs = m_xmldomSchedule.GetElementsByTagName(OUTPUT);
            //m_astrOutputs = new string[xmlnodelistOutputs.Count];
            m_astrOutputs = new List<string>();

            //Get the name from each output node and insert at the index
            foreach (XmlNode xmlnodeOutput in xmlnodelistOutputs)
            {
                int intIndex = int.Parse(
                    xmlnodeOutput.LastChild.PreviousSibling.InnerText);
                m_astrOutputs.Add(xmlnodeOutput.LastChild.InnerText);
            }
        }//BuildOutputs()


        /// <summary>
        /// Method to fill in the collection for the normal days and holidays
        /// from the xml file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        private void BuildDayTypes()
        {
            //Local Variables
            XmlNodeList xmlnodelistDayTypes = null;
            int intNormalCount = 0;
            int intHolidayCount = 0;
            int intIndex = 0;

            //Get all of the Daytype nodes
            xmlnodelistDayTypes = m_xmldomSchedule.GetElementsByTagName(DAY_TYPE);

            //Go through the Daytype nodes and get the number 
            // of normal and number of holiday nodes
            foreach (XmlNode xmlnodeDayType in xmlnodelistDayTypes)
            {
                if (int.Parse(xmlnodeDayType.FirstChild.InnerText) ==
                    (int)eDayType.NORMAL)
                {
                    intNormalCount++;
                }
                else
                {
                    intHolidayCount++;
                }
            }

            //Initialize the normal days and holiday array based on the counts
            m_astrNormalDays = new List<string>();
            m_astrHolidays = new List<string>();

            //Put each of the strings in the xml nodes into the array at the 
            //given index
            foreach (XmlNode xmlnodeDayType in xmlnodelistDayTypes)
            {
                intIndex = int.Parse(
                    xmlnodeDayType.FirstChild.NextSibling.InnerText);
                if (int.Parse(xmlnodeDayType.FirstChild.InnerText) ==
                    (int)eDayType.NORMAL)
                {
                    m_astrNormalDays.Add(xmlnodeDayType.LastChild.InnerText);
                }
                else
                {
                    m_astrHolidays.Add(xmlnodeDayType.LastChild.InnerText);
                }
            }
        }//BuildDayTypes()


        /// <summary>
        /// Method to fill in the collection for the seasons from the xml file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class 
        /// 04/16/14 jrf 3.50.78 489749 Modified to add the typical week when constructing season.
        private void BuildSeasons()
        {
            //Local Variables
            XmlNodeList xmlnodelistSeasons = null;
            XmlNodeList xmlnodelistChildren = null;
            XmlNodeList xmlnodelistPatterns = null;
            XmlNode xmlnodeChild = null;
            Int16Collection aintNormalDays;
            Int16Collection aintHolidays;
            string strSeasonName = null;
            int intSeasonID = 0;
            int intCount = 0;
            int intIndex = 0;
            int intID = 0;

            //Initialize the Season Collection
            m_colSeasons = new CSeasonCollection();

            //Get all of the nodes with the Season tag
            xmlnodelistSeasons = m_xmldomSchedule.GetElementsByTagName(SEASON);
            foreach (XmlNode xmlnodeSeason in xmlnodelistSeasons)
            {
                if (xmlnodeSeason.HasChildNodes)
                {
                    //Get all child nodes of the current season
                    xmlnodelistChildren = xmlnodeSeason.ChildNodes;

                    //Get all of the season attributes
                    for (intCount = 0; intCount < xmlnodelistChildren.Count;
                        intCount++)
                    {
                        xmlnodeChild = xmlnodelistChildren[intCount];
                        if (SEASON_ID == xmlnodeChild.Name)
                        {
                            //Get the Season ID
                            intSeasonID = int.Parse(xmlnodeChild.InnerText);
                        }
                        else if (NAME == xmlnodeChild.Name)
                        {
                            //Get the Season Name
                            strSeasonName = xmlnodeChild.InnerText;
                        }
                        else
                        {
                            //Initialize the normal and holiday arrays
                            aintNormalDays = new Int16Collection();
                            aintHolidays = new Int16Collection();

                            //Get all of the applied patterns
                            xmlnodelistPatterns = xmlnodeChild.ChildNodes;
                            foreach (XmlNode xmlnodePattern in xmlnodelistPatterns)
                            {
                                intIndex = int.Parse(
                                    xmlnodePattern.LastChild.InnerText);
                                intID = int.Parse(
                                    xmlnodePattern.FirstChild.InnerText);

                                //Add the Pattern ID to the normal array
                                if (int.Parse(
                                    xmlnodePattern.FirstChild.NextSibling.InnerText)
                                    == (int)eDayType.NORMAL)
                                {
                                    aintNormalDays.Add((Int16)intID);
                                }
                                //Add thePattern ID to the holiday array
                                else
                                {
                                    aintHolidays.Add((Int16)intID);
                                }
                            }

                            //Create a seson object and it to the collection
                            m_colSeasons.Add(new CSeason(intSeasonID, strSeasonName,
                                aintNormalDays, aintHolidays, TypicalWeek));
                        }
                    }
                }
            }
        }//BuildSeasons()


        /// <summary>
        /// Method to fill in the collection for the patterns from the xml file
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        private void BuildPatterns()
        {
            //Local Variables
            XmlNodeList xmlnodelistPatterns = null;
            XmlNodeList xmlnodelistChildren = null;
            XmlNodeList xmlnodelistSwitchPoints = null;
            XmlNode xmlnodeChild = null;
            CSwitchPointCollection colSwitchPoints = null;
            int intPatternID = 0;
            string strPatternName = null;
            int intStart = 0;
            int intStop = 0;
            int intIndex = 0;
            eSwitchPointType eType = 0;
            int intChildCount = 0;
            int intCount = 0;

            //Initialize the Pattern Collection
            m_colPatterns = new CPatternCollection();

            //Get all of the Pattern nodes
            xmlnodelistPatterns = m_xmldomSchedule.GetElementsByTagName(PATTERN);
            foreach (XmlNode xmlnodePattern in xmlnodelistPatterns)
            {
                if (xmlnodePattern.HasChildNodes)
                {
                    //Get all child nodes of the current Pattern
                    xmlnodelistChildren = xmlnodePattern.ChildNodes;
                    intChildCount = xmlnodelistChildren.Count;

                    //Initialize the switchpoint collection
                    colSwitchPoints = new CSwitchPointCollection();

                    for (intCount = 0; intCount < intChildCount; intCount++)
                    {
                        xmlnodeChild = xmlnodelistChildren[intCount];
                        if (xmlnodeChild.Name == PATTERN_ID)
                        {
                            //Get the ID of the Pattern
                            intPatternID = int.Parse(xmlnodeChild.InnerText);
                        }
                        else if (xmlnodeChild.Name == NAME)
                        {
                            //Get the name of the pattern
                            strPatternName = xmlnodeChild.InnerText;
                        }
                        else
                        {
                            //Get the switchpoint nodes for the pattern
                            xmlnodelistSwitchPoints = xmlnodeChild.ChildNodes;
                            foreach (XmlNode xmlnodeSwitchPoint in
                                xmlnodelistSwitchPoints)
                            {
                                if (xmlnodeSwitchPoint.Name == START_TIME)
                                {
                                    //Get the Start Time
                                    intStart = int.Parse(
                                        xmlnodeSwitchPoint.InnerText);
                                }
                                else if (xmlnodeSwitchPoint.Name == STOP_TIME)
                                {
                                    //Get the Stop Time
                                    intStop = int.Parse(
                                        xmlnodeSwitchPoint.InnerText);
                                }
                                else if (xmlnodeSwitchPoint.Name == INDEX)
                                {
                                    //Get the Rate or Output Index
                                    intIndex = int.Parse(
                                        xmlnodeSwitchPoint.InnerText);
                                }
                                else
                                {
                                    //Get the SwitchPointType 
                                    eType = (eSwitchPointType)int.Parse(
                                        xmlnodeSwitchPoint.InnerText);

                                    //Create the SwitchPoint object and add
                                    colSwitchPoints.Add(new CSwitchPoint(
                                        intStart, intStop, intIndex, eType));
                                }
                            }
                        }
                    }
                    if (intCount == intChildCount)
                    {
                        //Create the Pattern and add it to the collection
                        m_colPatterns.Add(new CPattern(
                            intPatternID, strPatternName, colSwitchPoints));
                    }
                }
            }
        }//BuildPatterns()


        /// <summary>
        /// Method to fill in the array for the supported devices from the xml file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        private void BuildSupportedDevices()
        {
            //Local Variables
            XmlNodeList xmlnodelistDevices = null;

            //Get a list of xml nodes with the tag SupportedDevice
            xmlnodelistDevices = m_xmldomSchedule.GetElementsByTagName(
                SUPPORTED_DEVICE);

            //Create the string array to be the length of the list of xml nodes
            m_astrSupportedDevices = new List<string>();

            //Put each of the strings in the xml nodes into the array
            for (int intIterator = 0; intIterator < xmlnodelistDevices.Count;
                intIterator++)
            {
                m_astrSupportedDevices.Add(
                            xmlnodelistDevices[intIterator].InnerText);
            }
        }//BuildSupportedDevices


        /// <summary>
        /// Retrieves the general TOU Information from xml file.  This includes
        /// Name, ID, Start Year, Duration, Holiday List, Description, Date Created,
        /// Date Modified, and Date Time Invariant. 
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/28/06 rrr N/A	 N/A	Added to get the TOU_Information section
        ///								of the TOU xml file 
        ///	06/02/06 ach N/A     N/A    Added functionality to read information
        ///								concerning date formatting
        private void GetTOUInformation()
        {
            //Local Variables
            XmlNodeList xmlnodelistCurrent = null;

            //Get the Name of the TOU Schedule
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(NAME);
            m_strTOUName = xmlnodelistCurrent[0].InnerText;

            //Get the TOU ID
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(TOU_ID);
            m_intTOUID = int.Parse(xmlnodelistCurrent[0].InnerText);

            // Get the TOU Export Count (If it is not there we will return zero, since it has never been exported)
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(TOU_EXPORT_COUNT);
            if (xmlnodelistCurrent[0] != null)
            {
                m_intTOUExportCount = int.Parse(xmlnodelistCurrent[0].InnerText);
            }
            else
            {
                m_intTOUExportCount = 0;
            }

            //Get the Start Year
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(STARTYEAR);
            m_intStartYear = int.Parse(xmlnodelistCurrent[0].InnerText);

            //Get the Duration
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(DURATION);
            m_intDuration = int.Parse(xmlnodelistCurrent[0].InnerText);

            //Get the Holiday List ID
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(HOLIDAYLISTID);
            m_intHolidayListID = int.Parse(xmlnodelistCurrent[0].InnerText);

            //Get the Description
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(DESCRIPTION);
            m_strDescription = xmlnodelistCurrent[0].InnerText;

            // ach 06-02-06 start
            // Try to get the Date Time Format Info if available
            m_blnUseInvariant = false;
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(DATE_FORMAT_INFO);
            if (xmlnodelistCurrent.Count > 0)
            {
                m_blnUseInvariant = true;
                m_CultureFormat = new CultureInfo(xmlnodelistCurrent[0].InnerText, true);
            }
            // ach 06-02-06 end

            //Get the Date Created
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(DATECREATED);
            m_dtDateCreated = StringToDate(xmlnodelistCurrent[0].InnerText);

            //Get the Date Modified
            xmlnodelistCurrent = m_xmldomSchedule.GetElementsByTagName(DATEMODIFIED);
            m_dtDateModified = StringToDate(xmlnodelistCurrent[0].InnerText);

        }//GetTOUInformation


        /// <summary>
        /// This method writes the TOU Information and Supported 
        /// Devices information out to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -----------
        /// 05/31/06 ach N/A     N/A    Added functionality to write
        ///								TOU Schedule to file
        private void WriteTOUInformation(string strFileName)
        {
            m_xmlWriter = new XmlTextWriter(strFileName, null);
            m_xmlWriter.Formatting = Formatting.Indented;
            m_xmlWriter.Indentation = 1;
            m_xmlWriter.IndentChar = '\t';

            string strProcessingInstr = "type = \"text/xsl\" href=\"Detail.xsl\"";

            // XML File Header Information
            m_xmlWriter.WriteStartDocument();
            m_xmlWriter.WriteProcessingInstruction("xml-stylesheet", strProcessingInstr);

            // Write Version Information
            if (null != ProductName && null != ProductVersion)
            {
                m_xmlWriter.WriteComment("Created by " + ProductName + " Version " + ProductVersion);
            }
            
            // TOU_FILE node
            m_xmlWriter.WriteStartElement("TOU_File");
            m_xmlWriter.WriteAttributeString("xmlns:xsi",
                "http://www.w3.org/1999/XMLSWchema-instance");
            m_xmlWriter.WriteAttributeString("FileType", "TOU_SCHEDULE");

            // Schedule Data and TOU_Information nodes
            m_xmlWriter.WriteStartElement("ScheduleData");
            m_xmlWriter.WriteStartElement("TOU_Information");

            // TOU Schedule Name and ID
            m_xmlWriter.WriteElementString("Name", m_strTOUName);
            m_xmlWriter.WriteElementString("TOU_ID", m_intTOUID.ToString());
            
            // TOU Export Count (Used by FCS Export)
            m_xmlWriter.WriteElementString("ExportCount", m_intTOUExportCount.ToString());

            // Supported Devices
            m_xmlWriter.WriteStartElement("SupportedDevices");
            for (int intCount = 0; intCount < m_astrSupportedDevices.Count;
                intCount++)
            {
                m_xmlWriter.WriteElementString("SupportedDevice",
                    m_astrSupportedDevices[intCount]);
            }
            m_xmlWriter.WriteEndElement();

            // TOU Schedule Start Year, Duration, Description
            m_xmlWriter.WriteElementString("StartYear", StartYear.ToString());
            m_xmlWriter.WriteElementString("Duration", Duration.ToString());
            m_xmlWriter.WriteElementString("Description", m_strDescription);

            // Write the invariant used for date formatting
            // If an invariant used then write that invariant
            if (m_blnUseInvariant)
            {
                m_xmlWriter.WriteElementString("Date_Format_Info",
                    ((CultureInfo)m_CultureFormat).Name);
            }
            // If no invariant used, write the current system's culture
            else
            {
                m_xmlWriter.WriteElementString("Date_Format_Info",
                    CultureInfo.CurrentUICulture.Name);
                m_CultureFormat = DateTimeFormatInfo.CurrentInfo;
            }

            // Update the date modified to the moment that we are saving

            m_dtDateModified = DateTime.Now;

            // Write the Date Created, Date Modified, and the Holiday List ID
            m_xmlWriter.WriteElementString("DateCreated", m_dtDateCreated.ToString("G", m_CultureFormat));
            m_xmlWriter.WriteElementString("DateModified", m_dtDateModified.ToString("G", m_CultureFormat));
            m_xmlWriter.WriteElementString("HolidayListID", m_intHolidayListID.ToString("G", m_CultureFormat));

            // Insert Data Item Writing Information
            m_xmlWriter.WriteStartElement("DataItems");

            // Read Only Info ? 
            m_xmlWriter.WriteStartElement("DataItem");
            m_xmlWriter.WriteElementString("ItemID", "6");
            m_xmlWriter.WriteElementString("Value", "0");
            m_xmlWriter.WriteEndElement();

            // Number of Rates ?
            m_xmlWriter.WriteStartElement("DataItem");
            m_xmlWriter.WriteElementString("ItemID", "13");
            m_xmlWriter.WriteElementString("Value", Rates.Count.ToString());
            m_xmlWriter.WriteEndElement();

            // Number of Outputs ? 
            m_xmlWriter.WriteStartElement("DataItem");
            m_xmlWriter.WriteElementString("ItemID", "14");
            m_xmlWriter.WriteElementString("Value", Outputs.Count.ToString());
            m_xmlWriter.WriteEndElement();

            // Number of Normal Days ? 
            m_xmlWriter.WriteStartElement("DataItem");
            m_xmlWriter.WriteElementString("ItemID", "15");
            m_xmlWriter.WriteElementString("Value", NormalDays.Count.ToString());
            m_xmlWriter.WriteEndElement();

            // Number of Holidays ?
            m_xmlWriter.WriteStartElement("DataItem");
            m_xmlWriter.WriteElementString("ItemID", "16");
            m_xmlWriter.WriteElementString("Value", Holidays.Count.ToString());
            m_xmlWriter.WriteEndElement();

            m_xmlWriter.WriteEndElement();

            // Close TOU_Information and write Typical Week
            m_xmlWriter.WriteEndElement();

            OnStepProgress(new ProgressEventArgs());

            WriteTypicalWeek();

        }//WriteTouInformation


        /// <summary>
        /// Method to write the Typical Week information to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -----------
        /// 05/31/06 ach N/A     N/A    Added functionality to write
        ///								TypicalWeek to file
        private void WriteTypicalWeek()
        {
            m_xmlWriter.WriteStartElement("TypicalWeek");

            string[] typicalDay = 
                        { "TypicalSunday",
                          "TypicalMonday",
                          "TypicalTuesday",
                          "TypicalWednesday",
                          "TypicalThursday",
                          "TypicalFriday",
                          "TypicalSaturday"
                        };

            // Write the information for each tyipcal day in the typical week
            for (int intCount = 0; intCount < m_astrTypicalWeek.Length; intCount++)
            {
                m_xmlWriter.WriteStartElement(typicalDay[intCount]);
                m_xmlWriter.WriteElementString("DaytypeType", "1");
                m_xmlWriter.WriteElementString("DaytypeIndex",
                    GetDayType(m_astrTypicalWeek[intCount]).Index.ToString());
                m_xmlWriter.WriteEndElement();
            }

            // Close TypicalWeek and write Years
            m_xmlWriter.WriteEndElement();

            OnStepProgress(new ProgressEventArgs());

            WriteYears();

        }//WriteTypicalWeek


        /// <summary>
        /// Method to write the Years information to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -----------
        /// 05/31/06 ach N/A     N/A    Added functionality to write
        ///								Years to file		
        private void WriteYears()
        {
            // Local Variables
            CYear year = null;
            CEventCollection yearEvents = null;
            CEvent WriteEvent = null;
            m_xmlWriter.WriteStartElement("Years");

            // Write out information for all years
            for (int intCount = 0; intCount < m_colYears.Count; intCount++)
            {
                year = m_colYears[intCount];
                yearEvents = year.Events;

                m_xmlWriter.WriteStartElement("Year");
                m_xmlWriter.WriteElementString("YearNumber", year.Year.ToString());
                m_xmlWriter.WriteStartElement("AppliedHolidays");

                // Write out all the applied holidays for the year
                for (int innerCount = 0; innerCount < yearEvents.Count; innerCount++)
                {
                    WriteEvent = yearEvents[innerCount];

                    // If the event is a holiday write the information
                    if (eEventType.HOLIDAY == WriteEvent.Type)
                    {
                        m_xmlWriter.WriteStartElement("AppliedHoliday");
                        m_xmlWriter.WriteElementString("Name", WriteEvent.Name);
                        m_xmlWriter.WriteElementString("HolidayDayTypeIndex",
                                                        WriteEvent.Index.ToString());
                        m_xmlWriter.WriteElementString("Date",
                                                        WriteEvent.Date.ToString());
                        m_xmlWriter.WriteEndElement();
                    }

                }

                // Close the Applied Holidays tag
                m_xmlWriter.WriteEndElement();

                m_xmlWriter.WriteStartElement("AppliedSeasons");

                // Write out all the season changes for the year
                for (int innerCount = 0; innerCount < yearEvents.Count; innerCount++)
                {
                    WriteEvent = yearEvents[innerCount];

                    // If the event is a season, write the information
                    if (eEventType.SEASON == WriteEvent.Type)
                    {
                        CSeason season = m_colSeasons[WriteEvent.Index];
                        m_xmlWriter.WriteStartElement("AppliedSeason");
                        m_xmlWriter.WriteElementString("SeasonID",
                                                    season.ID.ToString());
                        m_xmlWriter.WriteElementString("StartDate",
                                                    WriteEvent.Date.ToString());
                        m_xmlWriter.WriteEndElement();
                    }

                }

                // Close the AppliedSeasons tag
                m_xmlWriter.WriteEndElement();
                // Close the year tag
                m_xmlWriter.WriteEndElement();

            }

            // Close Years and write Rates
            m_xmlWriter.WriteEndElement();

            OnStepProgress(new ProgressEventArgs());

            WriteRates();

        }//WriteYear


        /// <summary>
        /// Method to write the rates information to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -----------
        /// 05/31/06 ach N/A     N/A    Added functionality to write
        ///								the rates to file
        private void WriteRates()
        {
            m_xmlWriter.WriteStartElement("Rates");

            // Write all the rates to file
            for (int intCount = 0; intCount < m_astrRates.Count; intCount++)
            {
                m_xmlWriter.WriteStartElement("Rate");
                m_xmlWriter.WriteElementString("Index", intCount.ToString());
                m_xmlWriter.WriteElementString("Name", m_astrRates[intCount]);
                m_xmlWriter.WriteEndElement();
            }

            // Close Rates and write Outputs
            m_xmlWriter.WriteEndElement();

            OnStepProgress(new ProgressEventArgs());

            WriteOutputs();

        }//WriteRates


        /// <summary>
        /// Method to write the outputs information to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -----------
        /// 05/31/06 ach N/A     N/A    Added functionality to write
        ///								the outputs to file
        private void WriteOutputs()
        {
            m_xmlWriter.WriteStartElement("Outputs");

            // Write all the outputs to file
            for (int intCount = 0; intCount < m_astrOutputs.Count; intCount++)
            {
                m_xmlWriter.WriteStartElement("Output");
                m_xmlWriter.WriteElementString("Index", intCount.ToString());
                m_xmlWriter.WriteElementString("Name", m_astrOutputs[intCount]);
                m_xmlWriter.WriteEndElement();
            }

            // Close Outputs and write DayTypes
            m_xmlWriter.WriteEndElement();

            OnStepProgress(new ProgressEventArgs());

            WriteDayTypes();

        }//WriteOutputs


        /// <summary>
        /// Method to write the day types information to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -----------
        /// 05/31/06 ach N/A     N/A    Added functionality to write
        ///								day types to file
        private void WriteDayTypes()
        {          

            m_xmlWriter.WriteStartElement("Daytypes");

            // Write Normal Day Types
            for (int intCount = 0; intCount < m_astrNormalDays.Count; intCount++)
            {
                m_xmlWriter.WriteStartElement("Daytype");
                m_xmlWriter.WriteElementString("DaytypeType", "1");
                m_xmlWriter.WriteElementString("DaytypeIndex", intCount.ToString());
                m_xmlWriter.WriteElementString("Name", m_astrNormalDays[intCount]);
                m_xmlWriter.WriteEndElement();
            }

            // Write Holiday Day Types
            for (int intCount = 0; intCount < m_astrHolidays.Count; intCount++)
            {
                m_xmlWriter.WriteStartElement("Daytype");
                m_xmlWriter.WriteElementString("DaytypeType", "2");
                m_xmlWriter.WriteElementString("DaytypeIndex", intCount.ToString());
                m_xmlWriter.WriteElementString("Name", m_astrHolidays[intCount]);
                m_xmlWriter.WriteEndElement();
            }

            // Close DayTypes and write Seasons
            m_xmlWriter.WriteEndElement();

            OnStepProgress(new ProgressEventArgs());

            WriteSeasons();

        }//WriteDayTypes


        /// <summary>
        /// Method to write the season informatino to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -----------
        /// 05/31/06 ach N/A     N/A    Added functionality to write
        ///								seasons to file
        private void WriteSeasons()
        {
            // Local Variables
            Int16Collection NormalPatterns;
            Int16Collection HolidayPatterns;
            CSeason WriteSeason = null;

            m_xmlWriter.WriteStartElement("Seasons");

            // Write Information out for all seasons
            for (int intCount = 0; intCount < m_colSeasons.Count; intCount++)
            {
                WriteSeason = m_colSeasons[intCount];
                NormalPatterns = WriteSeason.NormalDays;
                HolidayPatterns = WriteSeason.Holidays;

                m_xmlWriter.WriteStartElement("Season");
                m_xmlWriter.WriteElementString("SeasonID", WriteSeason.ID.ToString());
                m_xmlWriter.WriteElementString("Name", WriteSeason.Name);

                m_xmlWriter.WriteStartElement("AppliedPatterns");

                // Write all the normal day patterns
                for (int innerCount = 0; innerCount < NormalPatterns.Count;
                    innerCount++)
                {
                    m_xmlWriter.WriteStartElement("AppliedPattern");
                    m_xmlWriter.WriteElementString("PatternID",
                                        NormalPatterns[innerCount].ToString());
                    m_xmlWriter.WriteElementString("DaytypeType", "1");
                    m_xmlWriter.WriteElementString("DaytypeIndex", innerCount.ToString());
                    m_xmlWriter.WriteEndElement();

                }

                // Write all the holiday patterns
                for (int innerCount = 0; innerCount < HolidayPatterns.Count;
                    innerCount++)
                {
                    m_xmlWriter.WriteStartElement("AppliedPattern");
                    m_xmlWriter.WriteElementString("PatternID",
                                        HolidayPatterns[innerCount].ToString());
                    m_xmlWriter.WriteElementString("DaytypeType", "2");
                    m_xmlWriter.WriteElementString("DaytypeIndex", innerCount.ToString());
                    m_xmlWriter.WriteEndElement();

                }

                // Close Applied Patterns and Season
                m_xmlWriter.WriteEndElement();
                m_xmlWriter.WriteEndElement();

            }

            // Close Seasons and write Patterns
            m_xmlWriter.WriteEndElement();

            OnStepProgress(new ProgressEventArgs());

            WritePatterns();

        }//WriteSeasons


        /// <summary>
        /// Method to write the pattern information to file.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -----------
        /// 05/31/06 ach N/A     N/A    Added functionality to write
        ///								patterns to file
        private void WritePatterns()
        {
            // Local Variables
            CPattern pattern = null;
            CSwitchPointCollection spCol = null;
            CSwitchPoint point = null;

            m_xmlWriter.WriteStartElement("Patterns");

            // Write out information for each pattern in the collection
            for (int intCount = 0; intCount < m_colPatterns.Count; intCount++)
            {
                pattern = m_colPatterns[intCount];
                spCol = pattern.SwitchPoints;

                m_xmlWriter.WriteStartElement("Pattern");
                m_xmlWriter.WriteElementString("PatternID", pattern.ID.ToString());
                m_xmlWriter.WriteElementString("Name", pattern.Name);

                // Write the information for each switch point contained in the pattern
                for (int innerCount = 0; innerCount < spCol.Count; innerCount++)
                {
                    point = spCol[innerCount];
                    m_xmlWriter.WriteStartElement("SwitchPoint");
                    m_xmlWriter.WriteElementString("StartTime",
                                    point.StartTime.ToString());
                    m_xmlWriter.WriteElementString("StopTime",
                                    point.StopTime.ToString());
                    m_xmlWriter.WriteElementString("RateOutputIndex",
                                    point.RateOutputIndex.ToString());
                    if (eSwitchPointType.RATE == point.SwitchPointType)
                    {
                        m_xmlWriter.WriteElementString("SwitchPointType", "0");
                    }
                    else
                    {
                        m_xmlWriter.WriteElementString("SwitchPointType", "1");
                    }
                    m_xmlWriter.WriteEndElement();
                }

                // Close pattern
                m_xmlWriter.WriteEndElement();

            }

            // Close patterns
            m_xmlWriter.WriteEndElement();

            // Close Schedule Data and TOU_Informatoin
            m_xmlWriter.WriteEndElement();
            m_xmlWriter.WriteEndElement();

            // Close the document
            m_xmlWriter.WriteEndDocument();
            m_xmlWriter.Close();

        }//WritePatterns


        /// <summary>
        /// This method converts a string representation of a date into a
        /// DateTime object with culture specific information.
        /// </summary>
        /// <param name="strDate">
        /// String representation of the Date.
        /// </param>
        /// <returns>
        /// A DateTime object representing strDate.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/28/06 rrr N/A	 N/A	Added to convert the Date string to a DateTime
        ///								object  
        ///	06/28/06 ach N/A     N/A    Modified to use culture specific date 
        ///								formatting
        private DateTime StringToDate(string strDate)
        {
            // if an invariant was specified use that formatting for the date
            if (m_blnUseInvariant)
            {
                return DateTime.Parse(strDate, m_CultureFormat);
            }

            // if no invariant specified then use the current sytem to format date
            return DateTime.Parse(strDate);


            /* Old Code Temporarily Kept
            //Local Variables
            string[] strResults = null;
            string[] strDatePart = null;
            string[] strTime = null;
            int intYear = 0;
            int intMonth = 0;
            int intDay = 0;
            int intHour = 0;
            int intMinute = 0;
            int intSecond = 0;

            //Split the string into the Date and Time parts
            strResults = strDate.Split(' ');
            strDatePart = strResults[0].Split('/');
            strTime = strResults[1].Split(':');

            //Get the Date
            intMonth = int.Parse(strDatePart[0]);
            intDay = int.Parse(strDatePart[1]);
            intYear = int.Parse(strDatePart[2]);

            //Get the Time
            intHour = int.Parse(strTime[0]);
            intMinute = int.Parse(strTime[1]);
            intSecond = int.Parse(strTime[2]);

            //If the string contains PM and is not noon then add 12 to the hour 
            if(0 < strDate.IndexOf("PM") && intHour != 12)
            {
                intHour = intHour + 12;
            }

            //Return the new DateTime object
            return new DateTime(intYear,intMonth,intDay,intHour,
                intMinute,intSecond);
            */

        }//StringToDate

        /// <summary>
        /// This method checks to see if the supplied date is a hoilday.  If it
        /// is then it returns the holiday index.  If not it returns -1.
        /// </summary>
        /// <param name="dtDate">The supplied date to look for.</param>
        /// <returns>
        /// Returns the holiday index if the date is a holiday, else it returns -1.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/07 mrj 9.00.00		Created
        //  
        private int IsHoliday(DateTime dtDate)
        {
            int iIndex = -1;

            //Get the year for this date			
            int iYearIndex = m_colYears.SearchYear(dtDate.Year);
            CYear Year = m_colYears[iYearIndex];

            //Go through the event colletion and see if this date is a holiday
            CEventCollection EventCollection = Year.Events;
            foreach (CEvent Event in EventCollection)
            {
                if (Event.Date.Month == dtDate.Month &&
                    Event.Date.Day == dtDate.Day)
                {
                    if (Event.Type == eEventType.HOLIDAY)
                    {
                        //This is a holiday so return the index
                        iIndex = Event.Index;
                        break;
                    }
                }
            }

            return iIndex;
        }

        /// <summary>
        /// This static method retreives the list of IDs from all TOU schedules.
        /// </summary>
        /// <returns>A list of TOU schedule IDs.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/19/08 jrf 9.50.05	       Created. 
        private static List<int> GetScheduleIDs()
        {
            CTOUScheduleFileCollection TOUFiles = new CTOUScheduleFileCollection();
            List<int> lstIDs = new List<int>();

            for (int nCount = 0; nCount < TOUFiles.Count; nCount++)
            {
                lstIDs.Add(TOUFiles[nCount].ID);
            }

            return lstIDs;
        }


        /// <summary>
        /// Raises the event to show the progress bar.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/24/08 jrf 9.50.13        Created.
        private void OnShowProgress(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(null, e);
            }
        }

        /// <summary>
        /// Raises the event that causes the progress bar to perform a step
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/24/08 jrf 9.50.13        Created.
        private void OnStepProgress(ProgressEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(null, e);
            }
        }

        /// <summary>
        /// Raises the event that hides or closes the progress bar
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/24/08 jrf 9.50.13        Created.
        private void OnHideProgress(EventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(null, e);
            }
        }

    #endregion Private Methods
	}
}
