using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;

namespace Itron.Metering.Utilities
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

        /// <summary>
        /// Gets the serialized value of the Enum
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns>Null if the value is not marked with the XmlEnum attribute or the serialized value</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/22/14 RCG 3.50.29        Created
        
        public static string ToSerializedValue(this Enum value)
        {
            string SerializedValue = null;
            Type EnumType = value.GetType();
            FieldInfo EnumField = EnumType.GetField(value.ToString());

            XmlEnumAttribute[] XmlEnumAttributes = (XmlEnumAttribute[])EnumField.GetCustomAttributes(typeof(XmlEnumAttribute), false);

            if (XmlEnumAttributes.Length > 0)
            {
                SerializedValue = XmlEnumAttributes[0].Name;
            }
            else
            {
                throw new InvalidOperationException("The enum is not marked with the XmlEnum attribute");
            }

            return SerializedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/22/14 RCG 3.50.29        Created
        
        public static T ParseToEnumFromSerializedValue<T>(this string value)
        {
            T ReturnValue = default(T);

            foreach (T EnumValue in Enum.GetValues(typeof(T)))
            {
                FieldInfo EnumInfo = typeof(T).GetField(EnumValue.ToString());
                XmlEnumAttribute[] SerializedValues = (XmlEnumAttribute[])EnumInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false);

                if (SerializedValues.Length > 0)
                {
                    if (SerializedValues[0].Name == value)
                    {
                        ReturnValue = EnumValue;
                        break;
                    }
                }
                else
                {
                    throw new InvalidOperationException("The enum is not marked with the XmlEnum attribute");
                }
            }

            if (ReturnValue != null)
            {
                return ReturnValue;
            }
            else
            {
                throw new FormatException("No matching serialized value.");
            }
        }

        #endregion
    }
}
