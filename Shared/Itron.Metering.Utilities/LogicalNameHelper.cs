using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Logical Name Helper methods
    /// </summary>
    public static class LogicalNameHelper
    {
        /// <summary>
        /// Gets the string representation of a logical name
        /// </summary>
        /// <param name="ln">The logical name to get</param>
        /// <returns>The string representation of the LN</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public static string LogicalNameString(byte[] ln)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (ln != null)
            {
                if (ln.Length == 6)
                {
                    stringBuilder.Append(ln[0].ToString(CultureInfo.InvariantCulture));
                    stringBuilder.Append("-");
                    stringBuilder.Append(ln[1].ToString(CultureInfo.InvariantCulture));
                    stringBuilder.Append(":");
                    stringBuilder.Append(ln[2].ToString(CultureInfo.InvariantCulture));
                    stringBuilder.Append(".");
                    stringBuilder.Append(ln[3].ToString(CultureInfo.InvariantCulture));
                    stringBuilder.Append(".");
                    stringBuilder.Append(ln[4].ToString(CultureInfo.InvariantCulture));
                    stringBuilder.Append("*");
                    stringBuilder.Append(ln[5].ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    throw new ArgumentException("The Logical Name must have a length of 6", "ln");
                }
            }
            else
            {
                throw new ArgumentNullException("ln", "The Logical Name may not be null");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Parses the logical name from it's string format
        /// </summary>
        /// <param name="logicalName">The logical name in string format</param>
        /// <returns>The logical name as a byte array</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/21/12 RCG 2.80.01 N/A    Created

        public static byte[] ParseLogicalName(string logicalName)
        {
            string[] Values = logicalName.Split('-', ':', '.', '*');
            byte[] LNValue = new byte[6];

            if (Values.Length == 6)
            {
                for (int iIndex = 0; iIndex < Values.Length; iIndex++)
                {
                    LNValue[iIndex] = Byte.Parse(Values[iIndex], CultureInfo.InvariantCulture);
                }
            }
            else
            {
                throw new ArgumentException("The specified string is not a logical name", "logicalName");
            }

            return LNValue;
        }

        /// <summary>
        /// Extension method that converts a logical name byte array to an OBIS Code string
        /// </summary>
        /// <param name="logicalName">The Logical Name byte array</param>
        /// <returns>The OBIS Code string</returns>
        public static string ToObisCode(this byte[] logicalName)
        {
            return LogicalNameString(logicalName);
        }

        /// <summary>
        /// Extension method that converts an OBIS code string to a byte array
        /// </summary>
        /// <param name="obisCode">The OBIS Code string</param>
        /// <returns>The OBIS Code as a byte array</returns>
        public static byte[] ToLogicalName(this string obisCode)
        {
            return ParseLogicalName(obisCode);
        }
    }
}
