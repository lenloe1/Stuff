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
//                           Copyright © 2004 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Linq;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Class for Methods that could be used by various projects
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------------
    // 07/29/04 REM 7.00.15 N/A    Initial Release
    // 11/09/04 REM 7.00.28 776    Program Editor is not checking for other applications
    // 01/08/07 mrj 8.00.05		   Changes for new Field-Pro 
    // 02/27/07 AF  8.00.14 2316   Eliminated member variable m_astrApps
    //
    public static class CSharedMethods
    {
        #region Constants

        /// <summary>
        /// CSharedMethods protected member variable for the folder browser dialog
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1504:ReviewMisleadingFieldNames")]
        private static FolderBrowserDialog m_FolderBrowserDialog = new FolderBrowserDialog();
        /// <summary>
        /// public static string CHARACTER_DECIMAL = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
        /// </summary>
        private static readonly string CHARACTER_DECIMAL = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
        /// <summary>
        /// public static string CHARACTER_NEGATIVE = System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign; 
        /// </summary>
        private static readonly string CHARACTER_NEGATIVE = System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign;
        /// <summary>
        /// The index of the to DST date.
        /// </summary>
        private static readonly int TO_DATE = 0;
        /// <summary>
        /// The index of the from DST date
        /// </summary>
        private static readonly int FROM_DATE = 1;
        /// <summary>
        /// The BOM (Byte Order Mark)
        /// </summary>
        private static readonly string BYTE_ORDER_MARK = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
        /// <summary>
        /// String constant for a tab character
        /// </summary>
        public static readonly string TAB = new string((char)9, 1);

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes the BOM (Byte Order Mark) from the beginning of a string.
        /// </summary>
        /// <param name="str">The string to remove the BOM from.</param>
        /// <returns>The string without the BOM.</returns>
        public static string RemoveByteOrderMark(this string str)
        {
            string StringToReview = str;

            if (StringToReview != null && StringToReview.StartsWith(BYTE_ORDER_MARK, StringComparison.Ordinal))
            {
                StringToReview = StringToReview.Remove(0, BYTE_ORDER_MARK.Length);
            }

            return StringToReview;
        }

        /// <summary>
        /// Converts the image to a Base64String.
        /// </summary>
        /// <param name="image">The image to convert.</param>
        /// <returns>The image as a Base64String</returns>
        public static string ToBase64String(this Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image", "image cannot be null.");
            }

            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified System.String object occurs
        /// within this string.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="substring">The string to seek.</param>
        /// <param name="comp">One of the enumeration values that specifies the rules to use in the comparison.</param>
        /// <returns>true if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 07/13/16 DLG       		   Adding support for String Comparison enumeration.
        //
        public static bool Contains(this String str, String substring, StringComparison comp)
        {
            if (substring == null)
            {
                throw new ArgumentNullException("substring", "substring cannot be null.");
            }
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
            {
                throw new ArgumentException("comp is not a member of StringComparison", "comp");
            }

            return str.IndexOf(substring, comp) >= 0;
        }

        /// <summary>
        /// Display a dialog that allows the user to browse for a folder
        /// </summary>
        /// <param name="strDescription">Description to be displayed in the Browse dialog</param>
        /// <param name="strFolder">Default folder to use in the Browse dialog</param>
        /// <returns>Directory the user selected</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 09/20/04 REM 7.00.19		   Adding support for Export Field-Pro Settings
        //
        public static string BrowseForFolder(string strDescription, string strFolder)
        {
            string strReturn = strFolder;

            if (null != m_FolderBrowserDialog)
            {
                m_FolderBrowserDialog.Description = strDescription;
                m_FolderBrowserDialog.SelectedPath = strFolder;
                if (DialogResult.OK == m_FolderBrowserDialog.ShowDialog())
                {
                    strReturn = m_FolderBrowserDialog.SelectedPath;
                }
            }

            return strReturn;
        }

        /// <summary>
        /// Converts a UTC time to a local time based on the TimeZone settings in the meter.
        /// </summary>
        /// <param name="timeToConvert">The UTC time to convert.</param>
        /// <param name="blnDSTEnabled">Whether DST is enabled</param>
        /// <param name="lstDSTDates">List of DST date pairs as list of date arrays with two dates in each array.</param>
        /// <param name="tsDSTAdjustmentAmount">Timespan to adjust DST by.</param>
        /// <param name="tsTimeZoneOffset">timespan to Adjust the timezone by.</param>
        /// <returns>The time as a local time.</returns>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 	10/18/10 RCG 2.45.06		Created
        //  05/18/12 jrf 2.60.23        Refactored method here to share it with another subcontrol.
        //
        public static DateTime ConvertTimeToLocal(DateTime timeToConvert, bool blnDSTEnabled, List<DateTime[]> lstDSTDates,
            TimeSpan tsDSTAdjustmentAmount, TimeSpan tsTimeZoneOffset)
        {
            DateTime NewTime = timeToConvert;
            List<TimeZoneInfo.AdjustmentRule> AdjustmentRules = new List<TimeZoneInfo.AdjustmentRule>();

            if (blnDSTEnabled && lstDSTDates != null && lstDSTDates.Count > 0)
            {
                foreach (DateTime[] adtDSTDatePair in lstDSTDates)
                {
                    TimeZoneInfo.TransitionTime DSTStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(
                        new DateTime(1, 1, 1, adtDSTDatePair[TO_DATE].Hour, adtDSTDatePair[TO_DATE].Minute, adtDSTDatePair[TO_DATE].Second),
                        adtDSTDatePair[TO_DATE].Month, adtDSTDatePair[TO_DATE].Day);
                    TimeZoneInfo.TransitionTime DSTEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(
                        new DateTime(1, 1, 1, adtDSTDatePair[FROM_DATE].Hour, adtDSTDatePair[FROM_DATE].Minute, adtDSTDatePair[FROM_DATE].Second),
                        adtDSTDatePair[FROM_DATE].Month, adtDSTDatePair[FROM_DATE].Day);

                    TimeZoneInfo.AdjustmentRule Rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(adtDSTDatePair[TO_DATE].Year, 1, 1, 0, 0, 0),
                        new DateTime(adtDSTDatePair[TO_DATE].Year, 12, 31, 0, 0, 0), tsDSTAdjustmentAmount, DSTStart, DSTEnd);

                    AdjustmentRules.Add(Rule);
                }
            }

            TimeZoneInfo MeterTimeZone = TimeZoneInfo.CreateCustomTimeZone("Meter Time", tsTimeZoneOffset, "Meter Time", "Meter Time",
                "Meter Time", AdjustmentRules.ToArray(), !blnDSTEnabled);

            NewTime = TimeZoneInfo.ConvertTimeFromUtc(timeToConvert, MeterTimeZone);


            return NewTime;
        }

        /// <summary>
        /// Extension method used for comparing two byte arrays
        /// </summary>
        /// <param name="value">The value of the current object</param>
        /// <param name="other">The value of the object to compare</param>
        /// <returns>True if the two byte arrays match. False otherwise.</returns>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 	01/18/13 RCG 2.70.59		Created

        public static bool IsEqual(this byte[] value, byte[] other)
        {
            bool Equal = false;

            if (value == null && other == null)
            {
                Equal = true;
            }
            else if (value != null && other != null && value.Length == other.Length)
            {
                bool DataMatches = true;

                for (int iIndex = 0; iIndex < value.Length; iIndex++)
                {
                    if (value[iIndex] != other[iIndex])
                    {
                        DataMatches = false;
                        break;
                    }
                }

                Equal = DataMatches;
            }

            return Equal;
        }

        /// <summary>
        /// Extension method to return both the class and name.
        /// </summary>
        /// <param name="method">The method of the current object.</param>
        /// <returns>The class name and method name.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 05/13/14 DLG 4.00.06 NA        Created.
        // 
        public static string ClassAndName(this MethodBase method)
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}.{1}", method.ReflectedType, method.Name);
        }

        /// <summary>
        /// Extension method to return the next leap year in the format 02/29/YYYY 12:00:00 A.M.
        /// </summary>
        /// <param name="date">The DateTime of the current object.</param>
        /// <returns>The DateTime of the next leap year starting at midnight.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 10/16/14 DLG 4.00.72 TC 60787  Created.
        // 
        public static DateTime GetNextLeapYear(this DateTime date)
        {
            const int LEAP_YEAR_OCCURRANCE = 4;
            const int LEAP_YEAR_MONTH = 2;
            const int LEAP_YEAR_DAY = 29;

            for (int YearIndex = 0; YearIndex <= LEAP_YEAR_OCCURRANCE; YearIndex++)
            {
                DateTime futureDate = date.AddYears(YearIndex);

                if (DateTime.IsLeapYear(futureDate.Year))
                {
                    if (YearIndex == 0)
                    {
                        if (futureDate.Month > LEAP_YEAR_MONTH)
                        {
                            continue; // Leap day has already passed
                        }
                        if (futureDate.Month == LEAP_YEAR_MONTH && futureDate.Day == LEAP_YEAR_DAY)
                        {
                            continue; // Today is leap day
                        }
                    }

                    date = new DateTime(futureDate.Year, LEAP_YEAR_MONTH, LEAP_YEAR_DAY);
                    break;
                }
            }

            return date;
        }

        /// <summary>
        /// Extension method to round the time to the nearest given ticks
        /// </summary>
        /// <param name="date">The date and time to round.</param>
        /// <param name="roundTicks">The ticks to be rounded.</param>
        /// <example>
        ///     DateTime.Now.Trim(TimeSpan.TicksPerDay);
        ///     DateTime.Now.Trim(TimeSpan.TicksPerHour);
        ///     DateTime.Now.Trim(TimeSpan.TicksPerMillisecond);
        ///     DateTime.Now.Trim(TimeSpan.TicksPerMinute);
        ///     DateTime.Now.Trim(TimeSpan.TicksPerSecond);
        /// </example>
        /// <returns>Date and Time rounded by the given ticks.</returns>
        // Revision History	
        // MM/DD/YY Who Version  ID Number Description
        // -------- --- -------- -- ------ ----------------------------------------------------------
        // 08/05/15 DLG 4.50.185 TC 62250  Created.
        // 
        public static DateTime Trim(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks);
        }

        /// <summary>
        /// Converts the datetime to a Unix time-stamp representing context time from 1970.
        /// </summary>
        /// <param name="date">The date and time to convert.</param>
        /// <returns>The number of seconds since 1970.</returns>
        // Revision History	
        // MM/DD/YY Who Version  ID Number Description
        // -------- --- -------- -- ------ ----------------------------------------------------------
        // 08/05/15 DLG 4.50.185 TC 62250  Created.
        // 
        public static double ConvertToUnixTimestamp(this DateTime date)
        {
            return (date - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Assuming the double is a unix time stamp, converts to a date time object.
        /// </summary>
        /// <param name="unixTimeStamp">The unix time stamp</param>
        /// <returns>The unix time stamp as a date time object.</returns>
        // Revision History	
        // MM/DD/YY Who Version     ID Number Description
        // -------- --- ----------- -- ------ ----------------------------------------------------------
        // 08/23/16 DLG 1.2016.8.67 WR 668710 Created.
        // 
        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(unixTimeStamp);
        }

        /// <summary>
        /// Assuming the long is a unix time stamp, converts to a date time object.
        /// </summary>
        /// <param name="unixTimeStamp">The unix time stamp</param>
        /// <returns>The unix time stamp as a date time object.</returns>
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            return ((double)unixTimeStamp).UnixTimeStampToDateTime();
        }

        /// <summary>
        /// Assuming the long is a unix time stamp, converts to a date time object.
        /// </summary>
        /// <param name="unixTimeStamp">The unix time stamp</param>
        /// <returns>The unix time stamp as a date time object.</returns>
        public static DateTime UnixTimeStampToDateTime(this ulong unixTimeStamp)
        {
            return ((double)unixTimeStamp).UnixTimeStampToDateTime();
        }

        /// <summary>
        /// Helper method to retrieve the date and time from a table row's column.
        /// </summary>
        /// <param name="unixTimeStamp">The unixtime stamp.</param>
        /// <param name="useLocalTime">True to use local time, False to use UTC time.</param>
        /// <returns>The date and time.</returns>
        public static DateTime? UnixTimeStampToDateTime(this string unixTimeStamp, bool useLocalTime = false)
        {
            double ParsedDouble = 0;
            DateTime? ParsedDateTime = null;

            if (double.TryParse(unixTimeStamp, out ParsedDouble))
            {
                ParsedDateTime = ParsedDouble.UnixTimeStampToDateTime();

                if (useLocalTime)
                {
                    ParsedDateTime = ParsedDateTime.Value.ToLocalTime();
                }
            }

            return ParsedDateTime;
        }

        /// <summary>
        /// Rounds the given date and time up by the given TimeSpan.
        /// </summary>
        /// <param name="dateTime">The date and time to round up.</param>
        /// <param name="timeSpan">The time span used to round up.</param>
        /// <returns>The date and time after being round up.</returns>
        public static DateTime RoundUp(this DateTime dateTime, TimeSpan timeSpan)
        {
            var delta = timeSpan.Ticks - (dateTime.Ticks % timeSpan.Ticks);
            return new DateTime(dateTime.Ticks + delta, dateTime.Kind);
        }

        /// <summary>
        /// Rounds up to the nearest decimal place.
        /// </summary>
        /// <param name="decInput"></param>
        /// <param name="places"></param>
        /// <returns>The decimal round to the nearest decimal place.</returns>
        public static decimal RoundUp(this decimal decInput, int places)
        {
            decimal multiplier = Convert.ToDecimal(Math.Pow(10, Convert.ToDouble(places)));
            return Math.Ceiling(decimal.Multiply(decInput, Convert.ToDecimal(multiplier))) / multiplier;
        }

        /// <summary>
        /// Rounds the given date and time down by the given TimeSpan.
        /// </summary>
        /// <param name="dateTime">The date and time to round down.</param>
        /// <param name="timeSpan">The time span used to round down.</param>
        /// <returns>The date and time after being round down.</returns>
        public static DateTime RoundDown(this DateTime dateTime, TimeSpan timeSpan)
        {
            var delta = dateTime.Ticks % timeSpan.Ticks;
            return new DateTime(dateTime.Ticks - delta, dateTime.Kind);
        }

        /// <summary>
        /// Extension method that returns a random date and time base on the given range.
        /// </summary>
        /// <param name="date">The DateTime of the current object</param>
        /// <param name="fromDate">The oldest date in the date range.</param>
        /// <param name="toDate">The newest date in the date range.</param>
        /// <returns>A random date and time.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 10/16/14 DLG 4.00.74 TC 60810  Created.
        // 
        public static DateTime GenerateRandomDate(this DateTime date, DateTime fromDate, DateTime toDate)
        {
            Random RandomNumber = new Random();
            var Range = toDate - fromDate;
            var RandomTimeSpan = new TimeSpan((long)(RandomNumber.NextDouble() * Range.Ticks));

            date = fromDate + RandomTimeSpan;

            return date;
        }

        /// <summary>
        /// Compares two byte arrays
        /// </summary>
        /// <param name="value">The first byte[]</param>
        /// <param name="other">The second byte[]</param>
        /// <returns>0 if equal, negative if less than, positive if greater than</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/21/14 RCG 3.50.36 N/A    Created

        public static int CompareTo(this byte[] value, byte[] other)
        {
            int Comparison = 0;

            if (value == null && other != null)
            {
                Comparison = -1;
            }
            else if (value != null && other == null)
            {
                Comparison = 1;
            }
            else if (value != null && other != null)
            {
                if (value.Length == other.Length)
                {
                    for (int Index = 0; Index < value.Length; Index++)
                    {
                        Comparison = value[Index].CompareTo(other[Index]);

                        if (Comparison != 0)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Comparison = value.Length.CompareTo(other.Length);
                }
            }

            return Comparison;
        }

        /// <summary>
        /// Converts a byte[] to readable hex string
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The byte[] as a hex string</returns>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 	01/18/13 RCG 2.70.59		Created

        public static string ToHexString(this byte[] value)
        {
            StringBuilder HexString = new StringBuilder();

            if (value != null)
            {
                for (int iIndex = 0; iIndex < value.Length; iIndex++)
                {
                    HexString.Append(value[iIndex].ToString("X2", CultureInfo.InvariantCulture));

                    // Add a space after all but the last byte
                    if (iIndex != value.Length - 1)
                    {
                        HexString.Append(" ");
                    }
                }
            }

            return HexString.ToString();
        }

        /// <summary>
        /// Converts a string of hex characters to a user friendly ASCII string.
        /// </summary>
        /// <param name="hexValue">The hex string to convert.</param>
        /// <returns>The formatted ASCII string.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 03/10/15 DLG 4.50.76           Created.
        // 
        public static string HexToASCII(this string hexValue)
        {
            const int HEX_VALUE_SIZE = 2;
            const int HEX_BASE = 16;
            const int START_INDEX = 0;

            string ValueInASCII = string.Empty;

            // Loop through each hex value, convert it to ASCII, and add to the string.
            while (hexValue.Length > START_INDEX)
            {
                ValueInASCII += System.Convert.ToChar(System.Convert.ToUInt32(hexValue.Substring(START_INDEX, HEX_VALUE_SIZE), HEX_BASE)).ToString(CultureInfo.InvariantCulture);
                hexValue = hexValue.Substring(HEX_VALUE_SIZE, hexValue.Length - HEX_VALUE_SIZE);
            }

            return ValueInASCII;
        }

        /// <summary>
        /// Converts a user friendly ASCII string to hex characters.
        /// </summary>
        /// <param name="asciiValue">The ASCII string to convert</param>
        /// <returns>The formatted hex string.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 03/10/15 DLG 4.50.76           Created.
        // 
        public static string ASCIIToHex(this string asciiValue)
        {
            string ValueInHex = string.Empty;

            // Loop through each char, convert to Hex, and add to the string.
            foreach (char c in asciiValue)
            {
                int tmp = c;
                ValueInHex += String.Format(CultureInfo.InvariantCulture, "{0:x2}",
                    (uint)System.Convert.ToUInt32(tmp.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
            }

            return ValueInHex;
        }

        /// <summary>
        /// Converts a hex string value to a byte array.
        /// </summary>
        /// <param name="hexString">The hex string.</param>
        /// <returns>A byte array.</returns>
        public static byte[] HexStringToByteArray(this string hexString)
        {
            byte[] ByteArray = null;

            if (!string.IsNullOrEmpty(hexString))
            {
                ByteArray = Enumerable.Range(0, hexString.Length / 2)
                    .Select(x => Convert.ToByte(hexString.Substring(x * 2, 2), 16))
                    .ToArray();
            }

            return ByteArray;
        }

        /// <summary>
        /// Converts a byte[] to readable bit string
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The byte[] as a bit string</returns>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 	06/12/13 RCG 2.80.37		Created

        public static string ToBitString(this byte[] value)
        {
            string BitString = "";

            for (int iIndex = 0; iIndex < value.Length; iIndex++)
            {
                for (int iBit = 7; iBit >= 0; iBit--)
                {
                    byte CurrentValue = (byte)((value[iIndex] >> iBit) & 0x01);

                    BitString += CurrentValue.ToString(CultureInfo.InvariantCulture);

                    if (iBit == 4)
                    {
                        BitString += " ";
                    }
                }

                // Add a space after all but the last byte
                if (iIndex != value.Length - 1)
                {
                    BitString += " ";
                }
            }

            return BitString;
        }

        /// <summary>
        /// This method changes a given input value to fall on a specified increment value.
        /// </summary>
        /// <param name="decInputValue">The input value.</param>
        /// <param name="decIncrement">The increment value.</param>
        /// <returns>The modified input value.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a	Created
        // 02/04/15 jrf 4.50.50 557098 Refactored method here to share it with another subcontrol.
        public static decimal AlignInputValueToIncrement(decimal decInputValue, decimal decIncrement)
        {
            decimal decOutputValue = decInputValue;
            int iMultiplier = 1;

            if (0 != decInputValue % decIncrement)
            {
                iMultiplier = (int)(decInputValue / decIncrement);

                decOutputValue = decIncrement * iMultiplier;
            }

            return decOutputValue;
        }

        /// <summary>
        /// Extension to randomize a list
        /// </summary>
        /// <typeparam name="T">Type of List</typeparam>
        /// <param name="list">List to randomize</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random RandomNumberGenerator = new Random();
            int ListIndex = list.Count;
            while (ListIndex > 1)
            {
                ListIndex--;
                int RandomIndex = RandomNumberGenerator.Next(ListIndex + 1);
                T value = list[RandomIndex];
                list[RandomIndex] = list[ListIndex];
                list[ListIndex] = value;
            }
        }

        /// <summary>
        /// Extension to determine if the number is even.
        /// </summary>
        /// <param name="number">The number to be checked.</param>
        /// <returns>True if even, otherwise false.</returns>
        // Revision History	
        // MM/DD/YY Who Version  ID Number Description
        // -------- --- -------- -- ------ ----------------------------------------------------------
        // 08/28/15 DLG 4.50.201 TC 60321  Created.
        // 
        public static bool IsEven(this int number)
        {
            return number % 2 == 0;
        }

        /// <summary>
        /// Compares two version strings
        /// </summary>
        /// <param name="first">The first string</param>
        /// <param name="second">The second string</param>
        /// <returns>-1 if first less than second, 0 if first equal second, 1 if first greater than second</returns>
        public static int CompareVersionString(string first, string second)
        {
            int Value = 0;

            if (string.IsNullOrEmpty(first) == false && string.IsNullOrEmpty(second) == false)
            {
                string[] FirstSplit = first.Split('.');
                string[] SecondSplit = second.Split('.');

                for (int Index = 0; Index < Math.Max(first.Length, second.Length); Index++)
                {
                    if (Index < FirstSplit.Length && Index < SecondSplit.Length)
                    {
                        int FirstValue = int.Parse(FirstSplit[Index], CultureInfo.InvariantCulture);
                        int SecondValue = int.Parse(SecondSplit[Index], CultureInfo.InvariantCulture);

                        int Comparison = FirstValue.CompareTo(SecondValue);

                        if (Comparison != 0)
                        {
                            // We found a difference in a version number so we can return the comparison. 
                            // Otherwise we should continue through the rest of the numbers.
                            Value = Comparison;
                            break;
                        }
                    } // else - One of the values is shorter than the other so handle this as all values are equal
                }

            } // else one of the values is a "Don't Care" so we will return equal

            return Value;
        }

        /// <summary>
        /// Converts an XDocument to an XmlDocument
        /// </summary>
        /// <param name="xDocument">The XDocument to convert.</param>
        /// <returns>The converted XmlDocument.</returns>
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        /// <summary>
        /// Converts an XmlDocument to an XDocument.
        /// </summary>
        /// <param name="xmlDocument">The XmlDocument to convert.</param>
        /// <returns>The converted XDocument.</returns>
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the product version as a double
        /// </summary>
        /// <returns>Product Version</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/19/05 REM 7.20.XX        Adding support for version information in .Replica files
        // 07/24/05 REM 7.20.06        Fixing problem with incorrectly getting the version information
        //
        public static float ProductVersion
        {
            get
            {
                float fltReturn = 0;
                Version version = null;

                version = Assembly.GetCallingAssembly().GetName().Version;

                fltReturn = version.Major;
                fltReturn += (float)(version.Minor / 100.0);

                return fltReturn;
            }
        }

        #endregion

    }
}
