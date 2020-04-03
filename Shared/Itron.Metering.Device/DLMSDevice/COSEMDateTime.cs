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
//                           Copyright © 2012 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device.DLMSDevice
{
    #region Global Definitions

    /// <summary>
    /// Month values used by the COSEM Date objects
    /// </summary>
    public enum COSEMMonth : byte
    {
        /// <summary>January</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "January")]
        January = 1,
        /// <summary>February</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "February")]
        February = 2,
        /// <summary>March</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "March")]
        March = 3,
        /// <summary>April</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "April")]
        April = 4,
        /// <summary>May</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "May")]
        May = 5,
        /// <summary>June</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "June")]
        June = 6,
        /// <summary>July</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "July")]
        July = 7,
        /// <summary>August</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "August")]
        August = 8,
        /// <summary>September</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "September")]
        September = 9,
        /// <summary>October</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "October")]
        October = 10,
        /// <summary>November</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "November")]
        November = 11,
        /// <summary>December</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "December")]
        December = 12,
        /// <summary>The month when Daylight Savings Time ends</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "DSTEndMonth")]
        DaylightSavingsEnd = 0xFD,
        /// <summary>The month when Daylight Savings Time begins</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "DSTStartMonth")]
        DaylightSavingsBegin = 0xFE,
        /// <summary>Not Specified (Wildcard)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NotSpecified")]
        NotSpecified = 0xFF,
    }

    /// <summary>
    /// Day of Month Values use in the COSEM Date objects
    /// </summary>
    public enum COSEMDayOfMonth : byte
    {
        /// <summary>1st</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "First")]
        First = 1,
        /// <summary>2nd</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Second")]
        Second = 2,
        /// <summary>3rd</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Third")]
        Third = 3,
        /// <summary>4th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Fourth")]
        Fourth = 4,
        /// <summary>5th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Fifth")]
        Fifth = 5,
        /// <summary>6th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Sixth")]
        Sixth = 6,
        /// <summary>7th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Seventh")]
        Seventh = 7,
        /// <summary>8th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Eighth")]
        Eighth = 8,
        /// <summary>9th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Ninth")]
        Ninth = 9,
        /// <summary>10th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Tenth")]
        Tenth = 10,
        /// <summary>11th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Eleventh")]
        Eleventh = 11,
        /// <summary>12th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twelfth")]
        Twelfth = 12,
        /// <summary>13th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Thirteenth")]
        Thirteenth = 13,
        /// <summary>14th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Fourteenth")]
        Fourteenth = 14,
        /// <summary>15th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Fifteenth")]
        Fifteenth = 15,
        /// <summary>16th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Sixteenth")]
        Sixteenth = 16,
        /// <summary>17th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Seventeenth")]
        Seventeenth = 17,
        /// <summary>18th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Eighteenth")]
        Eighteenth = 18,
        /// <summary>19th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Nineteenth")]
        Nineteenth = 19,
        /// <summary>20th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentieth")]
        Twentieth = 20,
        /// <summary>21st</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentyfirst")]
        Twentyfirst = 21,
        /// <summary>22nd</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentysecond")]
        Twentysecond = 22,
        /// <summary>23rd</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentythird")]
        Twentythird = 23,
        /// <summary>24th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentyfourth")]
        Twentyfourth = 24,
        /// <summary>25th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentyfifth")]
        Twentyfifth = 25,
        /// <summary>26th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentysixth")]
        Twentysixth = 26,
        /// <summary>27th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentyseventh")]
        Twentyseventh = 27,
        /// <summary>28th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentyeighth")]
        Twentyeighth = 28,
        /// <summary>29th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Twentyninth")]
        Twentyninth = 29,
        /// <summary>30th</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Thirtieth")]
        Thirtieth = 30,
        /// <summary>31st</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Thirtyfirst")]
        Thirtyfirst = 31,
        /// <summary>Second to last day of the month</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "SecondToLastDay")]
        SecondToLastDay = 0xFD,
        /// <summary>The last day of the month</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "LastDay")]
        LastDay = 0xFE,
        /// <summary>Not Specified</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NotSpecified")]
        NotSpecified = 0xFF,
    }

    /// <summary>
    /// Day of the week for the COSEM Date objects
    /// </summary>
    public enum COSEMDayOfWeek : byte
    {
        /// <summary>Monday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Monday")]
        Monday = 1,
        /// <summary>Tuesday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Tuesday")]
        Tuesday = 2,
        /// <summary>Wednesday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Wednesday")]
        Wednseday = 3,
        /// <summary>Thursday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Thursday")]
        Thursday = 4,
        /// <summary>Friday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Friday")]
        Friday = 5,
        /// <summary>Saturday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Saturday")]
        Saturday = 6,
        /// <summary>Sunday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Sunday")]
        Sunday = 7,
        /// <summary>Not Specified</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NotSpecified")]
        NotSpecified = 0xFF,
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class COSEMDateTime
    {
    }

    /// <summary>
    /// COSEM object that represents a date (no time)
    /// </summary>
    public class COSEMDate
    {
        #region Constants

        private const string YEAR = "[YEAR]";
        private const string MONTH = "[MONTH]";
        private const string WEEK_DAY = "[WEEKDAY]";
        private const string DATE = "[DATE]";

        private const int RAW_DATA_SIZE = 5;
        /// <summary>
        /// Value of the year field if the year is not specified
        /// </summary>
        public const ushort YEAR_NOT_SPECIFIED = 0xFFFF;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public COSEMDate()
        {
            m_Year = 1970;
            m_Month = COSEMMonth.January;
            m_DayOfMonth = COSEMDayOfMonth.First;
            m_DayOfWeek = COSEMDayOfWeek.Thursday;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="date">The date time object to create the date from</param>
        public COSEMDate(DateTime date)
        {
            m_Year = (ushort)date.Year;
            m_Month = (COSEMMonth)date.Month;
            m_DayOfMonth = (COSEMDayOfMonth)date.Day;
            m_DayOfWeek = ConvertDateTimeDayOfWeekToCOSEMDayOfWeek(date.DayOfWeek);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawDate">The raw date value as read from the meter</param>
        public COSEMDate(byte[] rawDate)
        {
            if (rawDate == null || rawDate.Length != RAW_DATA_SIZE)
            {
                throw new ArgumentException("The rawDate value can not be null and must be of length " + RAW_DATA_SIZE.ToString(), "rawDate");
            }

            MemoryStream DataStream = new MemoryStream(rawDate);
            BinaryReader DataReader = new BinaryReader(DataStream);

            m_Year = DataReader.ReadUInt16();
            m_Month = (COSEMMonth)DataReader.ReadByte();
            m_DayOfMonth = (COSEMDayOfMonth)DataReader.ReadByte();
            m_DayOfWeek = (COSEMDayOfWeek)DataReader.ReadByte();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <param name="dayOfMonth">Day of Month</param>
        /// <param name="dayOfWeek">Day of Week</param>
        public COSEMDate(ushort year, COSEMMonth month, COSEMDayOfMonth dayOfMonth, COSEMDayOfWeek dayOfWeek)
        {
            m_Year = year;
            m_Month = month;
            m_DayOfMonth = dayOfMonth;
            m_DayOfWeek = dayOfWeek;
        }

        /// <summary>
        /// Gets the first date after the specified date that is represented by the COSEMDate object
        /// </summary>
        /// <param name="date">The date to get from</param>
        /// <returns>The first valid date</returns>
        public DateTime? GetClosestFutureDate(DateTime date)
        {
            DateTime? FutureDate = null;

            if (IsSpecificDate)
            {
                // The date is a specific date so as long as it's in the future we can return the date
                FutureDate = new DateTime(m_Year, (int)m_Month, (int)m_DayOfMonth);

                if (FutureDate < date.Date)
                {
                    // The date is in the past so set it back to null
                    FutureDate = null;
                }
            }
            else
            {
                // Start out with the specified date and figure it out from there
                DateTime NextDate = date.Date;

                if (m_Year != YEAR_NOT_SPECIFIED)
                {
                    if (m_Year < NextDate.Year)
                    {
                        // No valid future date
                        return null;
                    }
                    else
                    {
                        NextDate = NextDate.AddYears(m_Year - NextDate.Year);
                    }
                }

                if (m_Month != COSEMMonth.NotSpecified)
                {
                    if (m_Month == COSEMMonth.DaylightSavingsBegin)
                    {
                        // TODO: Figure out how to get the DST Month here
                    }
                    else if (m_Month == COSEMMonth.DaylightSavingsEnd)
                    {
                        // TODO: Figure out how to get the DST Month here
                    }
                    else
                    {
                        // Add one to the month until we reach the desired month so that the year will change as necessary
                        while (NextDate.Month != (int)m_Month)
                        {
                            NextDate = NextDate.AddMonths(1);
                        }
                    }
                }

                if (m_DayOfMonth != COSEMDayOfMonth.NotSpecified)
                {
                    if (m_DayOfMonth == COSEMDayOfMonth.LastDay || m_DayOfMonth == COSEMDayOfMonth.SecondToLastDay)
                    {
                        // Add one day until the next day causes the month to change
                        while (NextDate.Month == NextDate.AddDays(1).Month)
                        {
                            NextDate = NextDate.AddDays(1);
                        }

                        // For second to last day just subtract a day
                        if (m_DayOfMonth == COSEMDayOfMonth.SecondToLastDay)
                        {
                            NextDate = NextDate.AddDays(-1);
                        }
                    }
                    else
                    {
                        if ((int)m_DayOfMonth >= NextDate.Day && (int)m_DayOfMonth <= DateTime.DaysInMonth(NextDate.Year, NextDate.Month))
                        {
                            // We have a valid date in the current month
                            NextDate = NextDate.AddDays((int)m_DayOfMonth - NextDate.Day);
                        }
                        else if (m_Month == COSEMMonth.NotSpecified)
                        {
                            // The month is not specified and we have already passed the day in the current month so we should move on to the next month
                            NextDate = new DateTime(NextDate.Year, NextDate.Month, 1, 0, 0, 0, NextDate.Kind).AddMonths(1);

                            // Since the length of a month may vary lets just keep adding one day until it matches (if 29, 30 , or 31 is picked)
                            while (NextDate.Day != (int)m_DayOfMonth)
                            {
                                NextDate = NextDate.AddDays(1);
                            }
                        }
                        else if(m_Year == YEAR_NOT_SPECIFIED)
                        {
                            // In most cases we will need to add one year so let's go ahead and do that.
                            NextDate = new DateTime(NextDate.Year, NextDate.Month, 1, 0, 0, 0, NextDate.Kind).AddYears(1);

                            // The month is specified but the year is not. First we should check and see if the day of month is even valid at all for the month
                            // February is the only month that the number of days in the month can vary so lets check that first
                            if (m_Month == COSEMMonth.February)
                            {
                                if (m_DayOfMonth == COSEMDayOfMonth.Twentyninth)
                                {
                                    // Increase the year until there are 29 days in the month
                                    while (DateTime.DaysInMonth(NextDate.Year, 2) != 29)
                                    {
                                        NextDate = NextDate.AddYears(1);
                                    }

                                    // Set the day to the 29th. We should be at the first so add 28 days
                                    NextDate = NextDate.AddDays(28);
                                }
                                else if (m_DayOfMonth <= COSEMDayOfMonth.Twentyeighth)
                                {
                                    // We have already adjusted forward one year so just set the date
                                    NextDate = NextDate.AddDays((int)m_DayOfMonth - 1);
                                }
                                else
                                {
                                    // The date will never be valid for this month
                                    return null;
                                }
                            }
                            else if ((int)m_DayOfMonth <= DateTime.DaysInMonth(NextDate.Year, NextDate.Month))
                            {
                                // We have already adjusted forward one year so just set the date
                                NextDate = NextDate.AddDays((int)m_DayOfMonth - 1);
                            }
                            else
                            {
                                // The date will never be valid for this month
                                return null;
                            }
                        }
                    }
                }

                if (m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
                {
                    if (m_DayOfMonth == COSEMDayOfMonth.NotSpecified)
                    {
                        // Since the date is not specified we should be able to adjust the date forward
                        int DaysToAdjust = 0;
                        COSEMDayOfWeek CurrentDayOfWeek = ConvertDateTimeDayOfWeekToCOSEMDayOfWeek(NextDate.DayOfWeek);

                        if (m_DayOfWeek >= CurrentDayOfWeek)
                        {
                            DaysToAdjust = (int)m_DayOfWeek - (int)CurrentDayOfWeek;
                        }
                        else
                        {
                            // We have to adjust through the end of the week and then to the correct day of the week
                            DaysToAdjust = (int)COSEMDayOfWeek.Sunday - (int)CurrentDayOfWeek + (int)m_DayOfWeek;
                        }

                        if (m_Year != YEAR_NOT_SPECIFIED && m_Year != NextDate.AddDays(DaysToAdjust).Year)
                        {
                            // If the adjustment causes the year to change but a specific year is specified then there are no future dates
                            return null;
                        }
                        else if (m_Month != COSEMMonth.NotSpecified)
                        {
                        }
                    }
                }
            }

            return FutureDate;
        }

        /// <summary>
        /// Gets the first date before the specified date that is represented by the COSEMDate object
        /// </summary>
        /// <param name="date">The date to get from</param>
        /// <returns>The first valid date</returns>
        public DateTime? GetClosestPastDate(DateTime date)
        {
            return null;
        }

        /// <summary>
        /// Converts the date to a string
        /// </summary>
        /// <returns>The string representation of the date</returns>
        public override string ToString()
        {
            string strResult = "";

            if (IsSpecificDate)
            {
                strResult = SpecificDate.Value.ToString("G", CultureInfo.CurrentCulture);
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonth.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.LastDay && m_DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = COSEMResourceStrings.LastDayOfTheMonth;
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonth.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.SecondToLastDay && m_DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = COSEMResourceStrings.SecondToLastDayOfTheMonth;
            }
            else if (m_Year != YEAR_NOT_SPECIFIED && m_Month == COSEMMonth.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.LastDay && m_DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = COSEMResourceStrings.LasDayOfTheMonthInYear.Replace(YEAR, m_Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (m_Year != YEAR_NOT_SPECIFIED && m_Month == COSEMMonth.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.SecondToLastDay && m_DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = COSEMResourceStrings.SecondToLastDayOfTheMonthInYear.Replace(YEAR, m_Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonth.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.LastDay && m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = COSEMResourceStrings.LastWeekdayOfTheMonth.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(m_DayOfWeek));
            }
            else if (m_Year != YEAR_NOT_SPECIFIED && m_Month == COSEMMonth.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.SecondToLastDay && m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = COSEMResourceStrings.LastWeekdayOfTheMonthInYear.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(m_DayOfWeek));
                strResult = strResult.Replace(YEAR, m_Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonth.NotSpecified && m_DayOfMonth != COSEMDayOfMonth.NotSpecified && m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = COSEMResourceStrings.FirstWeekdayAfterDateInEveryMonth.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(m_DayOfWeek));
                strResult = strResult.Replace(DATE, EnumDescriptionRetriever.RetrieveDescription(m_DayOfMonth));
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month != COSEMMonth.NotSpecified && m_DayOfMonth != COSEMDayOfMonth.NotSpecified && m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = COSEMResourceStrings.FirstWeekdayAfterDateInSpecificMonth.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(m_DayOfWeek));
                strResult = strResult.Replace(DATE, EnumDescriptionRetriever.RetrieveDescription(m_DayOfMonth));
                strResult = strResult.Replace(MONTH, EnumDescriptionRetriever.RetrieveDescription(m_Month));
            }


            return strResult;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the date value if the value is a specific date
        /// </summary>
        public DateTime? SpecificDate
        {
            get
            {
                DateTime? NewDate = null;

                if (IsSpecificDate)
                {
                    NewDate = new DateTime(m_Year, (int)m_Month, (int)m_DayOfMonth);
                }

                return NewDate;
            }
        }

        #endregion

        #region Private Methods

        private static COSEMDayOfWeek ConvertDateTimeDayOfWeekToCOSEMDayOfWeek(DayOfWeek dayOfWeek)
        {
            COSEMDayOfWeek ConvertedDayOfWeek = COSEMDayOfWeek.NotSpecified;

            switch(dayOfWeek)
            {
                case DayOfWeek.Monday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Monday;
                    break;
                }
                case DayOfWeek.Tuesday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Tuesday;
                    break;
                }
                case DayOfWeek.Wednesday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Wednseday;
                    break;
                }
                case DayOfWeek.Thursday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Thursday;
                    break;
                }
                case DayOfWeek.Friday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Friday;
                    break;
                }
                case DayOfWeek.Saturday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Saturday;
                    break;
                }
                case DayOfWeek.Sunday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Sunday;
                    break;
                }
            }

            return ConvertedDayOfWeek;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the date specified is a specific date
        /// </summary>
        public bool IsSpecificDate
        {
            get
            {
                return m_Year != YEAR_NOT_SPECIFIED && m_Month >= COSEMMonth.January && m_Month <= COSEMMonth.December
                    && m_DayOfMonth >= COSEMDayOfMonth.First && m_DayOfMonth <= COSEMDayOfMonth.Thirtyfirst;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_Year;
        private COSEMMonth m_Month;
        private COSEMDayOfMonth m_DayOfMonth;
        private COSEMDayOfWeek m_DayOfWeek;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class COSEMTime
    {
    }
}
