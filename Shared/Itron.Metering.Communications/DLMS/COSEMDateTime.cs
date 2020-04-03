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

namespace Itron.Metering.Communications.DLMS
{
    #region Global Definitions

    /// <summary>
    /// Month values used by the COSEM Date objects
    /// </summary>
    public enum COSEMMonths : byte
    {
        /// <summary>January</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "January")]
        January = 1,
        /// <summary>February</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "February")]
        February = 2,
        /// <summary>March</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "March")]
        March = 3,
        /// <summary>April</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "April")]
        April = 4,
        /// <summary>May</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "May")]
        May = 5,
        /// <summary>June</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "June")]
        June = 6,
        /// <summary>July</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "July")]
        July = 7,
        /// <summary>August</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "August")]
        August = 8,
        /// <summary>September</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "September")]
        September = 9,
        /// <summary>October</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "October")]
        October = 10,
        /// <summary>November</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "November")]
        November = 11,
        /// <summary>December</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "December")]
        December = 12,
        /// <summary>The month when Daylight Savings Time ends</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "DSTEndMonth")]
        DaylightSavingsEnd = 0xFD,
        /// <summary>The month when Daylight Savings Time begins</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "DSTStartMonth")]
        DaylightSavingsBegin = 0xFE,
        /// <summary>Not Specified (Wildcard)</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "NotSpecified")]
        NotSpecified = 0xFF,
    }

    /// <summary>
    /// Day of Month Values use in the COSEM Date objects
    /// </summary>
    public enum COSEMDayOfMonth : byte
    {
        /// <summary>1st</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "First")]
        First = 1,
        /// <summary>2nd</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Second")]
        Second = 2,
        /// <summary>3rd</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Third")]
        Third = 3,
        /// <summary>4th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Fourth")]
        Fourth = 4,
        /// <summary>5th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Fifth")]
        Fifth = 5,
        /// <summary>6th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Sixth")]
        Sixth = 6,
        /// <summary>7th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Seventh")]
        Seventh = 7,
        /// <summary>8th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Eighth")]
        Eighth = 8,
        /// <summary>9th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Ninth")]
        Ninth = 9,
        /// <summary>10th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Tenth")]
        Tenth = 10,
        /// <summary>11th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Eleventh")]
        Eleventh = 11,
        /// <summary>12th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twelfth")]
        Twelfth = 12,
        /// <summary>13th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Thirteenth")]
        Thirteenth = 13,
        /// <summary>14th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Fourteenth")]
        Fourteenth = 14,
        /// <summary>15th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Fifteenth")]
        Fifteenth = 15,
        /// <summary>16th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Sixteenth")]
        Sixteenth = 16,
        /// <summary>17th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Seventeenth")]
        Seventeenth = 17,
        /// <summary>18th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Eighteenth")]
        Eighteenth = 18,
        /// <summary>19th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Nineteenth")]
        Nineteenth = 19,
        /// <summary>20th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentieth")]
        Twentieth = 20,
        /// <summary>21st</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentyfirst")]
        Twentyfirst = 21,
        /// <summary>22nd</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentysecond")]
        Twentysecond = 22,
        /// <summary>23rd</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentythird")]
        Twentythird = 23,
        /// <summary>24th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentyfourth")]
        Twentyfourth = 24,
        /// <summary>25th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentyfifth")]
        Twentyfifth = 25,
        /// <summary>26th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentysixth")]
        Twentysixth = 26,
        /// <summary>27th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentyseventh")]
        Twentyseventh = 27,
        /// <summary>28th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentyeighth")]
        Twentyeighth = 28,
        /// <summary>29th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Twentyninth")]
        Twentyninth = 29,
        /// <summary>30th</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Thirtieth")]
        Thirtieth = 30,
        /// <summary>31st</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Thirtyfirst")]
        Thirtyfirst = 31,
        /// <summary>Second to last day of the month</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "SecondToLastDay")]
        SecondToLastDay = 0xFD,
        /// <summary>The last day of the month</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "LastDay")]
        LastDay = 0xFE,
        /// <summary>Not Specified</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "NotSpecified")]
        NotSpecified = 0xFF,
    }

    /// <summary>
    /// Day of the week for the COSEM Date objects
    /// </summary>
    public enum COSEMDayOfWeek : byte
    {
        /// <summary>Monday</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Monday")]
        Monday = 1,
        /// <summary>Tuesday</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Tuesday")]
        Tuesday = 2,
        /// <summary>Wednesday</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Wednesday")]
        Wednseday = 3,
        /// <summary>Thursday</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Thursday")]
        Thursday = 4,
        /// <summary>Friday</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Friday")]
        Friday = 5,
        /// <summary>Saturday</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Saturday")]
        Saturday = 6,
        /// <summary>Sunday</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "Sunday")]
        Sunday = 7,
        /// <summary>Not Specified</summary>
        [EnumDescription("Itron.Metering.Communications.DLMS.DLMSCommStrings", typeof(DLMSCommStrings), "NotSpecified")]
        NotSpecified = 0xFF,
    }

    /// <summary>
    /// The Clock Statuses
    /// </summary>
    [Flags]
    public enum COSEMClockStatus : byte
    {
        /// <summary>No Status</summary>
        [EnumDescription("None")]
        None = 0x00,
        /// <summary>Clock value is invalid</summary>
        [EnumDescription("Invalid Value")]
        InvalidValue = 0x01,
        /// <summary>Clock value is doubtful</summary>
        [EnumDescription("Doubtful Value")]
        DoubtfulValue = 0x02,
        /// <summary>Clock value is using a different clock base</summary>
        [EnumDescription("Different Clock Base")]
        DifferentClockBase = 0x04,
        /// <summary>The status is invalid</summary>
        [EnumDescription("Invalid Clock Status")]
        InvalidClockStatus = 0x08,
        /// <summary>The clock value is in Daylight Savings time</summary>
        [EnumDescription("Daylight Savings Active")]
        DaylightSavingsActive = 0x80,
    }

    #endregion

    /// <summary>
    /// COSEM object that represents a date and time
    /// </summary>
    public class COSEMDateTime : IEquatable<COSEMDateTime>
    {
        #region Constants

        /// <summary>
        /// The length of the Date Time
        /// </summary>
        internal const int DATE_TIME_LENGTH = 12;

        private const string YEAR = "[YEAR]";
        private const string MONTH = "[MONTH]";
        private const string WEEK_DAY = "[WEEKDAY]";
        private const string DATE = "[DATE]";
        private const string TIME = "[TIME]";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDateTime()
        {
            m_Date = new COSEMDate();
            m_Time = new COSEMTime();
            m_Deviation = 0;
            m_ClockStatus = COSEMClockStatus.None;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="date">The DateTime object containing the date and time</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDateTime(DateTime date)
        {
            m_Date = new COSEMDate(date);
            m_Time = new COSEMTime(date);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The raw data for the COSEM Date Time</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDateTime(byte[] data)
        {
            if (data != null)
            {
                if (data.Length == DATE_TIME_LENGTH)
                {
                    MemoryStream DataStream = new MemoryStream(data);
                    DLMSBinaryReader DataReader = new DLMSBinaryReader(DataStream);

                    m_Date = DataReader.ReadCOSEMDate();
                    m_Time = DataReader.ReadCOSEMTime();
                    m_Deviation = DataReader.ReadInt16();
                    m_ClockStatus = DataReader.ReadEnum<COSEMClockStatus>();
                }
                else
                {
                    throw new ArgumentException("The data must be 12 bytes long", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null");
            }
        }

        /// <summary>
        /// Converts the date to a string
        /// </summary>
        /// <returns>The string representation of the date</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override string ToString()
        {
            string strResult = "";

            if (IsSpecificDateTime)
            {
                strResult = SpecificDateTime.Value.ToString("G", CultureInfo.CurrentCulture);
            }
            else if (IsNotSpecified)
            {
                strResult = DLMSCommStrings.NotSpecified;
            }
            else if (Year == COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth == COSEMDayOfMonth.NotSpecified && DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.EveryDayAtTime.Replace(TIME, m_Time.ToString());
            }
            else if (Year == COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth == COSEMDayOfMonth.LastDay && DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.LastDayOfTheMonthWithTime.Replace(TIME, m_Time.ToString());
            }
            else if (Year == COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth == COSEMDayOfMonth.SecondToLastDay && DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.SecondToLastDayOfTheMonthWithTime.Replace(TIME, m_Time.ToString());
            }
            else if (Year != COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth == COSEMDayOfMonth.LastDay && DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.LasDayOfTheMonthInYearWithTime.Replace(TIME, m_Time.ToString());
                strResult = strResult.Replace(YEAR, Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (Year != COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth == COSEMDayOfMonth.SecondToLastDay && DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.SecondToLastDayOfTheMonthInYearWithTime.Replace(TIME, m_Time.ToString());
                strResult = strResult.Replace(YEAR, Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (Year == COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth == COSEMDayOfMonth.LastDay && DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.LastWeekdayOfTheMonthWithTime.Replace(TIME, m_Time.ToString());
                strResult = strResult.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(DayOfWeek));
            }
            else if (Year != COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth == COSEMDayOfMonth.SecondToLastDay && DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.LastWeekdayOfTheMonthInYearWithTime.Replace(TIME, m_Time.ToString());
                strResult = strResult.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(DayOfWeek));
                strResult = strResult.Replace(YEAR, Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (Year == COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth != COSEMDayOfMonth.NotSpecified && DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.FirstWeekdayAfterDateInEveryMonthWithTime.Replace(TIME, m_Time.ToString());
                strResult = strResult.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(DayOfWeek));
                strResult = strResult.Replace(DATE, EnumDescriptionRetriever.RetrieveDescription(DayOfMonth));
            }
            else if (Year == COSEMDate.YEAR_NOT_SPECIFIED && Month != COSEMMonths.NotSpecified && DayOfMonth != COSEMDayOfMonth.NotSpecified && DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.FirstWeekdayAfterDateInSpecificMonthWithTime.Replace(TIME, m_Time.ToString());
                strResult = strResult.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(DayOfWeek));
                strResult = strResult.Replace(DATE, EnumDescriptionRetriever.RetrieveDescription(DayOfMonth));
                strResult = strResult.Replace(MONTH, EnumDescriptionRetriever.RetrieveDescription(Month));
            }
            else
            {
                // This is not ideal but we need to show something
                strResult = DLMSCommStrings.Month + ": " + EnumDescriptionRetriever.RetrieveDescription(Month)
                    + " " + DLMSCommStrings.DayOfMonth + ": " + EnumDescriptionRetriever.RetrieveDescription(DayOfMonth)
                    + " " + DLMSCommStrings.DayOfWeek + ": " + EnumDescriptionRetriever.RetrieveDescription(DayOfWeek)
                    + " " + DLMSCommStrings.Year + ": ";

                if (Year == COSEMDate.YEAR_NOT_SPECIFIED)
                {
                    strResult += DLMSCommStrings.NotSpecified;
                }
                else
                {
                    strResult += Year.ToString(CultureInfo.InvariantCulture);
                }

                strResult += " " + DLMSCommStrings.Time + ": " + m_Time.ToString();
            }


            return strResult;
        }

        /// <summary>
        /// Gets the COSEM Date Time as a COSEMData object
        /// </summary>
        /// <returns>The Date Time as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public COSEMData ToCOSEMData()
        {
            return ToCOSEMData(COSEMDataTypes.OctetString);
        }

        /// <summary>
        /// Gets the COSEM Date Time as a COSEMData object
        /// </summary>
        /// <param name="dataType">The type of COSEMData to get (DateTime or OctetString only)</param>
        /// <returns>The COSEMData object for the date time</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public COSEMData ToCOSEMData(COSEMDataTypes dataType)
        {
            COSEMData DataValue = null;

            switch(dataType)
            {
                case COSEMDataTypes.OctetString:
                {
                    DataValue = new COSEMData();
                    DataValue.DataType = COSEMDataTypes.OctetString;
                    DataValue.Value = Data;
                    break;
                }
                case COSEMDataTypes.DateTime:
                {
                    DataValue = new COSEMData();
                    DataValue.DataType = COSEMDataTypes.DateTime;
                    DataValue.Value = this;
                    break;
                }
                default:
                {
                    throw new ArgumentException("The data type must be Date Time or Octet String", "dataType");
                }
            }

            return DataValue;
        }

        /// <summary>
        /// Gets whether or not the COSEM Date Time object is equal to the current COSEM Date Time
        /// </summary>
        /// <param name="other">The COSEM Date Time object to compare to</param>
        /// <returns>True if the objects are equal. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/19/13 RCG 2.85.22 N/A    Created
        
        public bool Equals(COSEMDateTime other)
        {
            bool IsEqual = false;

            if (other != null)
            {
                IsEqual = m_Date.Equals(other.m_Date) && m_Time.Equals(other.m_Time);
            }

            return IsEqual;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the raw data for the 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] Data
        {
            get
            {
                MemoryStream DataStream = new MemoryStream();
                DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

                DataWriter.Write(m_Date);
                DataWriter.Write(m_Time);
                DataWriter.Write(m_Deviation);
                DataWriter.WriteEnum<COSEMClockStatus>(m_ClockStatus);

                return DataStream.ToArray();
            }
        }

        /// <summary>
        /// Gets whether or not the Date Time is a specific date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool IsSpecificDateTime
        {
            get
            {
                return m_Date.IsSpecificDate && m_Time.IsSpecificTime;
            }
        }

        /// <summary>
        /// Gets whether or not the Date Time is not specified
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool IsNotSpecified
        {
            get
            {
                return m_Date.IsNotSpecified && m_Time.IsNotSpecified;
            }
        }

        /// <summary>
        /// Gets the Specific Date Time object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DateTime? SpecificDateTime
        {
            get
            {
                DateTime? SpecificValue = null;

                if (IsSpecificDateTime)
                {
                    DateTime Date = m_Date.SpecificDate.Value;
                    DateTime Time = m_Time.SpecificTime.Value;

                    return new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second, Time.Millisecond, DateTimeKind.Utc);
                }

                return SpecificValue;
            }
        }

        /// <summary>
        /// Gets or sets the year
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ushort Year
        {
            get
            {
                return m_Date.Year;
            }
            set
            {
                m_Date.Year = value;
            }
        }

        /// <summary>
        /// Gets or sets the month
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public COSEMMonths Month
        {
            get
            {
                return m_Date.Month;
            }
            set
            {
                m_Date.Month = value;
            }
        }

        /// <summary>
        /// Gets or sets the day of the month
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public COSEMDayOfMonth DayOfMonth
        {
            get
            {
                return m_Date.DayOfMonth;
            }
            set
            {
                m_Date.DayOfMonth = value;
            }
        }

        /// <summary>
        /// Gets or sets the day of the week
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public COSEMDayOfWeek DayOfWeek
        {
            get
            {
                return m_Date.DayOfWeek;
            }
            set
            {
                m_Date.DayOfWeek = value;
            }
        }

        /// <summary>
        /// Gets or sets the hour
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte Hours
        {
            get
            {
                return m_Time.Hours;
            }
            set
            {
                m_Time.Hours = value;
            }
        }

        /// <summary>
        /// Get or sets the minute
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte Minutes
        {
            get
            {
                return m_Time.Minutes;
            }
            set
            {
                m_Time.Minutes = value;
            }
        }

        /// <summary>
        /// Get or sets the seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte Seconds
        {
            get
            {
                return m_Time.Seconds;
            }
            set
            {
                m_Time.Seconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the hundredths
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte Hundredths
        {
            get
            {
                return m_Time.Hundredths;
            }
            set
            {
                m_Time.Hundredths = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public short Deviation
        {
            get
            {
                return m_Deviation;
            }
            set
            {
                m_Deviation = value;
            }
        }

        /// <summary>
        /// Gets or sets the Clock Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMClockStatus ClockStatus
        {
            get
            {
                return m_ClockStatus;
            }
            set
            {
                m_ClockStatus = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMDate m_Date;
        private COSEMTime m_Time;
        private short m_Deviation;
        private COSEMClockStatus m_ClockStatus;

        #endregion
    }

    /// <summary>
    /// COSEM object that represents a date (no time)
    /// </summary>
    public class COSEMDate : IEquatable<COSEMDate>
    {
        #region Constants

        private const string YEAR = "[YEAR]";
        private const string MONTH = "[MONTH]";
        private const string WEEK_DAY = "[WEEKDAY]";
        private const string DATE = "[DATE]";

        /// <summary>
        /// The size of the date object
        /// </summary>
        internal const int DATE_SIZE = 5;
        /// <summary>
        /// Value of the year field if the year is not specified
        /// </summary>
        public const ushort YEAR_NOT_SPECIFIED = 0xFFFF;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDate()
        {
            m_Year = YEAR_NOT_SPECIFIED;
            m_Month = COSEMMonths.NotSpecified;
            m_DayOfMonth = COSEMDayOfMonth.NotSpecified;
            m_DayOfWeek = COSEMDayOfWeek.NotSpecified;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="date">The date time object to create the date from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDate(DateTime date)
        {
            m_Year = (ushort)date.Year;
            m_Month = (COSEMMonths)date.Month;
            m_DayOfMonth = (COSEMDayOfMonth)date.Day;
            m_DayOfWeek = ConvertDateTimeDayOfWeekToCOSEMDayOfWeek(date.DayOfWeek);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawDate">The raw date value as read from the meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDate(byte[] rawDate)
        {
            if (rawDate == null || rawDate.Length != DATE_SIZE)
            {
                throw new ArgumentException("The rawDate value can not be null and must be of length " + DATE_SIZE.ToString(), "rawDate");
            }

            MemoryStream DataStream = new MemoryStream(rawDate);
            DLMSBinaryReader DataReader = new DLMSBinaryReader(DataStream);

            m_Year = DataReader.ReadUInt16();
            m_Month = (COSEMMonths)DataReader.ReadByte();
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
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDate(ushort year, COSEMMonths month, COSEMDayOfMonth dayOfMonth, COSEMDayOfWeek dayOfWeek)
        {
            m_Year = year;
            m_Month = month;
            m_DayOfMonth = dayOfMonth;
            m_DayOfWeek = dayOfWeek;
        }

        /// <summary>
        /// Converts the date to a string
        /// </summary>
        /// <returns>The string representation of the date</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override string ToString()
        {
            string strResult = "";

            if (IsSpecificDate)
            {
                strResult = SpecificDate.Value.ToShortDateString();
            }
            else if (IsNotSpecified)
            {
                strResult = DLMSCommStrings.NotSpecified;
            }
            else if (Year == COSEMDate.YEAR_NOT_SPECIFIED && Month == COSEMMonths.NotSpecified && DayOfMonth == COSEMDayOfMonth.NotSpecified && DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.EveryDay;
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonths.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.LastDay && m_DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.LastDayOfTheMonth;
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonths.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.SecondToLastDay && m_DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.SecondToLastDayOfTheMonth;
            }
            else if (m_Year != YEAR_NOT_SPECIFIED && m_Month == COSEMMonths.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.LastDay && m_DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.LasDayOfTheMonthInYear.Replace(YEAR, m_Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (m_Year != YEAR_NOT_SPECIFIED && m_Month == COSEMMonths.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.SecondToLastDay && m_DayOfWeek == COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.SecondToLastDayOfTheMonthInYear.Replace(YEAR, m_Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonths.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.LastDay && m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.LastWeekdayOfTheMonth.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(m_DayOfWeek));
            }
            else if (m_Year != YEAR_NOT_SPECIFIED && m_Month == COSEMMonths.NotSpecified && m_DayOfMonth == COSEMDayOfMonth.SecondToLastDay && m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.LastWeekdayOfTheMonthInYear.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(m_DayOfWeek));
                strResult = strResult.Replace(YEAR, m_Year.ToString(CultureInfo.CurrentCulture));
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonths.NotSpecified && m_DayOfMonth != COSEMDayOfMonth.NotSpecified && m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.FirstWeekdayAfterDateInEveryMonth.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(m_DayOfWeek));
                strResult = strResult.Replace(DATE, EnumDescriptionRetriever.RetrieveDescription(m_DayOfMonth));
            }
            else if (m_Year == YEAR_NOT_SPECIFIED && m_Month != COSEMMonths.NotSpecified && m_DayOfMonth != COSEMDayOfMonth.NotSpecified && m_DayOfWeek != COSEMDayOfWeek.NotSpecified)
            {
                strResult = DLMSCommStrings.FirstWeekdayAfterDateInSpecificMonth.Replace(WEEK_DAY, EnumDescriptionRetriever.RetrieveDescription(m_DayOfWeek));
                strResult = strResult.Replace(DATE, EnumDescriptionRetriever.RetrieveDescription(m_DayOfMonth));
                strResult = strResult.Replace(MONTH, EnumDescriptionRetriever.RetrieveDescription(m_Month));
            }
            else
            {
                // This is not ideal but we need to show something
                strResult = DLMSCommStrings.Month + ": " + EnumDescriptionRetriever.RetrieveDescription(Month)
                    + " " + DLMSCommStrings.DayOfMonth + ": " + EnumDescriptionRetriever.RetrieveDescription(DayOfMonth)
                    + " " + DLMSCommStrings.DayOfWeek + ": " + EnumDescriptionRetriever.RetrieveDescription(DayOfWeek)
                    + " " + DLMSCommStrings.Year + ": ";

                if (Year == COSEMDate.YEAR_NOT_SPECIFIED)
                {
                    strResult += DLMSCommStrings.NotSpecified;
                }
                else
                {
                    strResult += Year.ToString(CultureInfo.InvariantCulture);
                }
            }

            return strResult;
        }

        /// <summary>
        /// Gets the COSEM Date as a COSEMData object
        /// </summary>
        /// <returns>The Date as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public COSEMData ToCOSEMData()
        {
            return ToCOSEMData(COSEMDataTypes.OctetString);
        }

        /// <summary>
        /// Gets the COSEM Date as a COSEMData object
        /// </summary>
        /// <param name="dataType">The type of COSEMData to get (Date or OctetString only)</param>
        /// <returns>The COSEMData object for the date</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public COSEMData ToCOSEMData(COSEMDataTypes dataType)
        {
            COSEMData DataValue = null;

            switch(dataType)
            {
                case COSEMDataTypes.OctetString:
                {
                    DataValue = new COSEMData();
                    DataValue.DataType = COSEMDataTypes.OctetString;
                    DataValue.Value = Data;
                    break;
                }
                case COSEMDataTypes.Date:
                {
                    DataValue = new COSEMData();
                    DataValue.DataType = COSEMDataTypes.Date;
                    DataValue.Value = this;
                    break;
                }
                default:
                {
                    throw new ArgumentException("The data type must be Date or Octet String", "dataType");
                }
            }

            return DataValue;
        }

        /// <summary>
        /// Gets whether or not the two COSEM Date objects are equal
        /// </summary>
        /// <param name="other">The COSEM Date object to compare to</param>
        /// <returns>True if the COSEM Date's are equal. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/19/13 RCG 2.85.21 N/A    Created
        
        public bool Equals(COSEMDate other)
        {
            bool IsEqual = false;

            if (other != null)
            {
                IsEqual = m_Year == other.m_Year && m_Month == other.m_Month && m_DayOfMonth == other.m_DayOfMonth && m_DayOfWeek == other.m_DayOfWeek;
            }

            return IsEqual;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts the DateTiem DayOfWeek to the COSEM version
        /// </summary>
        /// <param name="dayOfWeek">The DateTime DayOfWeek to convert</param>
        /// <returns>The COSEM Day of Week</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private static COSEMDayOfWeek ConvertDateTimeDayOfWeekToCOSEMDayOfWeek(System.DayOfWeek dayOfWeek)
        {
            COSEMDayOfWeek ConvertedDayOfWeek = COSEMDayOfWeek.NotSpecified;

            switch(dayOfWeek)
            {
                case System.DayOfWeek.Monday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Monday;
                    break;
                }
                case System.DayOfWeek.Tuesday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Tuesday;
                    break;
                }
                case System.DayOfWeek.Wednesday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Wednseday;
                    break;
                }
                case System.DayOfWeek.Thursday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Thursday;
                    break;
                }
                case System.DayOfWeek.Friday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Friday;
                    break;
                }
                case System.DayOfWeek.Saturday:
                {
                    ConvertedDayOfWeek = COSEMDayOfWeek.Saturday;
                    break;
                }
                case System.DayOfWeek.Sunday:
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
        /// Gets the date value if the value is a specific date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DateTime? SpecificDate
        {
            get
            {
                DateTime? NewDate = null;

                if (IsSpecificDate)
                {
                    NewDate = new DateTime(m_Year, (int)m_Month, (int)m_DayOfMonth, 0, 0, 0, DateTimeKind.Utc);
                }

                return NewDate;
            }
        }

        /// <summary>
        /// Gets whether or not the date specified is a specific date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool IsSpecificDate
        {
            get
            {
                // The DateTime object cannot support having a year of 0 so we will just say it is not specific to avoid the issue.
                return m_Year != YEAR_NOT_SPECIFIED  && m_Year != 0 && m_Month >= COSEMMonths.January && m_Month <= COSEMMonths.December
                    && m_DayOfMonth >= COSEMDayOfMonth.First && m_DayOfMonth <= COSEMDayOfMonth.Thirtyfirst;
            }
        }

        /// <summary>
        /// Gets whether or not the date is not specified
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool IsNotSpecified
        {
            get
            {
                return m_Year == YEAR_NOT_SPECIFIED && m_Month == COSEMMonths.NotSpecified
                    && m_DayOfMonth == COSEMDayOfMonth.NotSpecified;
            }
        }

        /// <summary>
        /// Gets or sets the year
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort Year
        {
            get
            {
                return m_Year;
            }
            set
            {
                m_Year = value;
            }
        }

        /// <summary>
        /// Gets or sets the month
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMMonths Month
        {
            get
            {
                return m_Month;
            }
            set
            {
                m_Month = value;
            }
        }

        /// <summary>
        /// Gets or sets the day of the month
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDayOfMonth DayOfMonth
        {
            get
            {
                return m_DayOfMonth;
            }
            set
            {
                m_DayOfMonth = value;
            }
        }

        /// <summary>
        /// Gets or sets the day of the week
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDayOfWeek DayOfWeek
        {
            get
            {
                return m_DayOfWeek;
            }
            set
            {
                m_DayOfWeek = value;
            }
        }

        /// <summary>
        /// Gets the raw data for the date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] Data
        {
            get
            {
                MemoryStream DataStream = new MemoryStream();
                DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

                DataWriter.Write(m_Year);
                DataWriter.Write((byte)m_Month);
                DataWriter.Write((byte)m_DayOfMonth);
                DataWriter.Write((byte)m_DayOfWeek);

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private ushort m_Year;
        private COSEMMonths m_Month;
        private COSEMDayOfMonth m_DayOfMonth;
        private COSEMDayOfWeek m_DayOfWeek;

        #endregion
    }

    /// <summary>
    /// COSEM object that represent a time
    /// </summary>
    public class COSEMTime : IEquatable<COSEMTime>
    {
        #region Constants

        /// <summary>
        /// The value for a field when it is not specified
        /// </summary>
        public const byte NOT_SPECIFIED = 0xFF;
        /// <summary>
        /// The size of the time object
        /// </summary>
        internal const int TIME_LENGTH = 4;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMTime()
        {
            m_Hour = NOT_SPECIFIED;
            m_Minute = NOT_SPECIFIED;
            m_Second = NOT_SPECIFIED;
            m_Hundredths = NOT_SPECIFIED;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="time">A DateTime object containing the time</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMTime(DateTime time)
        {
            m_Hour = (byte)time.Hour;
            m_Minute = (byte)time.Minute;
            m_Second = (byte)time.Second;
            m_Hundredths = (byte)(time.Millisecond / 10);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The raw data containing the time</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMTime(byte[] data)
        {
            if (data != null)
            {
                if (data.Length == TIME_LENGTH)
                {
                    m_Hour = data[0];
                    m_Minute = data[1];
                    m_Second = data[2];
                    m_Hundredths = data[3];
                }
                else
                {
                    throw new ArgumentException("The length of the data must be 4", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Converts the COSEMTime object to a string
        /// </summary>
        /// <returns>The string representation of the time</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts the COSEMTime object to a string
        /// </summary>
        /// <param name="format">The time format string</param>
        /// <param name="formatProvider">The format provider</param>
        /// <returns>The formatted string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public string ToString(string format, IFormatProvider formatProvider)
        {
            string FormatString;

            if (format.Length == 1)
            {
                // This is a Standard format string so we need to get the custom format string for the standard
                FormatString = GetStandardFormatString(format, formatProvider);
            }
            else
            {
                // It must be a custom format string so use it as is
                FormatString = format;
            }

            // Remove any items in the format string that are not specified.
            FormatString = UpdateFormatString(FormatString);

            return BaseTime.ToString(FormatString, formatProvider);
        }

        /// <summary>
        /// Gets the COSEM Time as a COSEMData object
        /// </summary>
        /// <returns>The Time as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public COSEMData ToCOSEMData()
        {
            return ToCOSEMData(COSEMDataTypes.OctetString);
        }

        /// <summary>
        /// Gets the COSEM Time as a COSEMData object
        /// </summary>
        /// <param name="dataType">The type of COSEMData to get (Time or OctetString only)</param>
        /// <returns>The COSEMData object for the time</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public COSEMData ToCOSEMData(COSEMDataTypes dataType)
        {
            COSEMData DataValue = null;

            switch(dataType)
            {
                case COSEMDataTypes.OctetString:
                {
                    DataValue = new COSEMData();
                    DataValue.DataType = COSEMDataTypes.OctetString;
                    DataValue.Value = Data;
                    break;
                }
                case COSEMDataTypes.Time:
                {
                    DataValue = new COSEMData();
                    DataValue.DataType = COSEMDataTypes.Time;
                    DataValue.Value = this;
                    break;
                }
                default:
                {
                    throw new ArgumentException("The data type must be Time or Octet String", "dataType");
                }
            }

            return DataValue;
        }

        /// <summary>
        /// Gets whether or not the two COSEM Time objects are equal
        /// </summary>
        /// <param name="other">The COSEM Time object to compare to</param>
        /// <returns>True if the COSEM Time's are equal. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/19/13 RCG 2.85.21 N/A    Created

        public bool Equals(COSEMTime other)
        {
            bool IsEqual = false;

            if (other != null)
            {
                IsEqual = m_Hour == other.m_Hour && m_Minute == other.m_Minute && m_Second == other.m_Second && m_Hundredths == other.m_Hundredths;
            }

            return IsEqual;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the format string so that anything that is not specified has been removed
        /// </summary>
        /// <param name="formatString">The format string to update.</param>
        /// <returns>The updated format string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private string UpdateFormatString(string formatString)
        {
            string Format = formatString;

            // We have the current culture's format so let's replace everything that is not specified

            if (m_Hour == NOT_SPECIFIED)
            {
                Format = Format.Replace("HH", "XX");
                Format = Format.Replace("hh", "XX");
                Format = Format.Replace("H", "XX");
                Format = Format.Replace("h", "XX");

                // We won't know the AM/PM designator either so it needs to be removed
                Format = Format.Replace("t", "");
            }

            if (m_Minute == NOT_SPECIFIED)
            {
                Format = Format.Replace("mm", "XX");
                Format = Format.Replace("m", "XX");
            }

            if (m_Second == NOT_SPECIFIED)
            {
                Format = Format.Replace("ss", "XX");
                Format = Format.Replace("s", "XX");
            }

            if (m_Hundredths == NOT_SPECIFIED)
            {
                Format.Replace("f", "X");
                Format.Replace("F", "X");
            }

            return Format;
        }

        /// <summary>
        /// Gets the custom format string for a standard time format string
        /// </summary>
        /// <param name="format">The Standard format string</param>
        /// <param name="formatProvider">The format provider</param>
        /// <returns>The custom format string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private string GetStandardFormatString(string format, IFormatProvider formatProvider)
        {
            DateTimeFormatInfo FormatInfo = DateTimeFormatInfo.GetInstance(formatProvider);
            string Format = null;

            // Since this is a time only format we are only supporting "t" and "T" standard formats
            if (format.Equals("t"))
            {
                Format = FormatInfo.ShortTimePattern;
            }
            else if (format.Equals("T"))
            {
                Format = FormatInfo.LongTimePattern;
            }
            else
            {
                // This is not a valid standard format string
                throw new ArgumentException("The format string specified is not a valid Standard format string", "format");
            }

            return Format;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the hour
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte Hours
        {
            get
            {
                return m_Hour;
            }
            set
            {
                if ((0 <= value && value <= 23) || value == NOT_SPECIFIED)
                {
                    m_Hour = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The hours must be between 0 and 23 or 0xFF");
                }
            }
        }

        /// <summary>
        /// Get or sets the minute
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte Minutes
        {
            get
            {
                return m_Minute;
            }
            set
            {
                if ((0 <= value && value <= 59) || value == NOT_SPECIFIED)
                {
                    m_Minute = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The minutes must be between 0 and 59 or 0xFF");
                }
            }
        }

        /// <summary>
        /// Get or sets the seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte Seconds
        {
            get
            {
                return m_Second;
            }
            set
            {
                if ((0 <= value && value <= 59) || value == NOT_SPECIFIED)
                {
                    m_Second = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The seconds must be between 0 and 59 or 0xFF");
                }
            }
        }

        /// <summary>
        /// Gets or sets the hundredths
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte Hundredths
        {
            get
            {
                return m_Hundredths;
            }
            set
            {
                if ((0 <= value && value <= 99) || value == NOT_SPECIFIED)
                {
                    m_Hundredths = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The hundredths must be between 0 and 59 or 0xFF");
                }
            }
        }

        /// <summary>
        /// Gets the raw data for the Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] Data
        {
            get
            {
                byte[] RawData = new byte[TIME_LENGTH];

                RawData[0] = m_Hour;
                RawData[1] = m_Minute;
                RawData[2] = m_Second;
                RawData[3] = m_Hundredths;

                return RawData;
            }
        }

        /// <summary>
        /// Gets the specific time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DateTime? SpecificTime
        {
            get
            {
                DateTime? Time = null;

                if (IsSpecificTime)
                {
                    int Milliseconds = 0;

                    if (m_Hundredths != NOT_SPECIFIED)
                    {
                        Milliseconds = m_Hundredths * 10;
                    }

                    Time = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, m_Hour, m_Minute, m_Second, Milliseconds, DateTimeKind.Utc);
                }

                return Time;
            }
        }

        /// <summary>
        /// Gets whether or not the time is a specific time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool IsSpecificTime
        {
            get
            {
                return m_Hour != NOT_SPECIFIED && m_Minute != NOT_SPECIFIED && m_Second != NOT_SPECIFIED;
            }
        }

        /// <summary>
        /// Gets whether or not the time is not specified
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public bool IsNotSpecified
        {
            get
            {
                return m_Hour == NOT_SPECIFIED && m_Minute == NOT_SPECIFIED && m_Second == NOT_SPECIFIED && m_Hundredths == NOT_SPECIFIED;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the time as a DateTime setting all not specified values to zero
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        private DateTime BaseTime
        {
            get
            {
                int CurrentHour = 0;
                int CurrentMinute = 0;
                int CurrentSecond = 0;
                int CurrentMillisecond = 0;

                if (m_Hour != NOT_SPECIFIED)
                {
                    CurrentHour = m_Hour;
                }

                if (m_Minute != NOT_SPECIFIED)
                {
                    CurrentMinute = m_Minute;
                }

                if (m_Second != NOT_SPECIFIED)
                {
                    CurrentSecond = m_Second;
                }

                if (m_Hundredths != NOT_SPECIFIED)
                {
                    CurrentMillisecond = m_Hundredths * 10;
                }

                return new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, CurrentHour, CurrentMinute, CurrentSecond, CurrentMillisecond, DateTimeKind.Utc);
            }
        }

        #endregion

        #region Member Variables

        private byte m_Hour;
        private byte m_Minute;
        private byte m_Second;
        private byte m_Hundredths;

        #endregion
    }
}
