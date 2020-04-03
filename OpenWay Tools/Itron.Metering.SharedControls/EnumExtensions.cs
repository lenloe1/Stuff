using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Utilities;

namespace Itron.Metering.SharedControls
{
    /// <summary>
    /// Extension methods for enums
    /// </summary>
    public static class EnumExtensions
    {
        #region Public Methods

        /// <summary>
        /// Extension method for enumerations that will return the description specified using a DescriptionAttribute modifier.
        /// </summary>
        /// <param name="value">The enumeration value.</param>
        /// <returns>The description of the value.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/09 RCG 2.20.03        Created

        public static string ToDescription(this Enum value)
        {
            return EnumDescriptionRetriever.RetrieveDescription(value);
        }

        /// <summary>
        /// Parses a string into an enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="strValue">The description of the enum value to parse.</param>
        /// <returns>The enum value.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/09 RCG 2.20.03        Created

        public static T ParseToEnum<T>(this string strValue)
        {
            return EnumDescriptionRetriever.ParseToEnum<T>(strValue);
        }

        #endregion
    }
}
