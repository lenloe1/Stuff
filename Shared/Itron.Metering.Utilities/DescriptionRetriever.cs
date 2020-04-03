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
//                           Copyright © 2013 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using System.Globalization;
using System.ComponentModel;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Attribute used for Descriptions of Enumeration values
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumDescriptionAttribute : Attribute
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">The description of the enum value</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/01/09 RCG 2.20.03        Created

        public EnumDescriptionAttribute(string description)
        {
            m_Description = description;
            m_ResourceBaseName = null;
            m_ResourceType = null;
            m_StringName = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceBaseName">The name of the resource file to use.</param>
        /// <param name="resourceType">The type of the resources object</param>
        /// <param name="stringName">The name of the string that contains the description</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/12 RCG 2.70.10 N/A    Created
        
        public EnumDescriptionAttribute(string resourceBaseName, Type resourceType, string stringName)
        {
            m_ResourceBaseName = resourceBaseName;
            m_ResourceType = resourceType;
            m_StringName = stringName;
            m_Description = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the description
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/01/09 RCG 2.20.03        Created

        public string Description
        {
            get
            {
                string EnumDescription = "";

                if (m_ResourceBaseName != null && m_StringName != null)
                {
                    EnumDescription = GetStringFromResourceFile(m_ResourceBaseName, m_ResourceType.Assembly, m_StringName);
                }
                else
                {
                    EnumDescription = m_Description;
                }

                return EnumDescription;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a string from the specified resource file
        /// </summary>
        /// <param name="resourceBaseName">The base name of the resource file</param>
        /// <param name="resourceAssembly">The assembly containing the resource file</param>
        /// <param name="stringName">The name of the string to retrieve</param>
        /// <returns>The specified string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/12 RCG 2.70.10 N/A    Created

        private static string GetStringFromResourceFile(string resourceBaseName, Assembly resourceAssembly, string stringName)
        {
            string ResourceString = "";
            ResourceManager Resources = new ResourceManager(resourceBaseName, resourceAssembly);

            try
            {
                ResourceString = Resources.GetString(stringName, CultureInfo.CurrentCulture);
            }
            catch
            {
                // Just return an empty string if it can't be retrieved. Unless we are in debug mode
#if DEBUG
                throw;
#endif
            }

            return ResourceString;
        }

        #endregion

        #region Member Variables

        private string m_Description;
        private string m_ResourceBaseName;
        private Type m_ResourceType;
        private string m_StringName;

        #endregion
    }


    /// <summary>
    /// Attribute used to determine which Event Table the event is injected
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumEventInfoAttribute : Attribute
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableID">The tableID to which the event is injected</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/02/11 MMD                Created

        public EnumEventInfoAttribute(string tableID)
        {
            m_TableID = tableID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the description
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/01/09 RCG 2.20.03        Created

        public string TableID
        {
            get
            {
                return m_TableID;
            }
        }

        #endregion

        #region Member Variables

        private string m_TableID;

        #endregion
    }

    /// <summary>
    /// Methods to be used for retrieving Descriptions
    /// </summary>
    public static class EnumDescriptionRetriever
    {
        #region Public Methods
        /// <summary>
        /// Extension method for enumerations that will return the description specified using a DescriptionAttribute modifier.
        /// </summary>
        /// <param name="value">The enumeration value.</param>
        /// <param name="separator">separator string</param>
        /// <returns>The description of the value.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/09 RCG 2.20.03        Created

        public static string RetrieveDescription(Enum value, string separator = null)
        {
            if(value == null)
            {
                throw new ArgumentNullException("value");
            }

            string strDescription = value.ToString();
            Type EnumType = value.GetType();

            List<FieldInfo> lstEnumFields = new List<FieldInfo>();
            List<string> lstDescriptions = new List<string>();

            FieldInfo EnumField = EnumType.GetField(strDescription);

            if (EnumField != null)
            {
                // Got the enum
                lstEnumFields.Add(EnumField);

                // IsDefined is much faster than GetCustomAttributes so lets call that first
                if (EnumField.IsDefined(typeof(DescriptionAttribute), false))
                {
                    // Check for the Description attribute. The EnumDescription attribute
                    // will overwrite this below if it is set.
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])EnumField.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attributes != null && attributes.Length > 0)
                    {
                        strDescription = attributes[0].Description;
                    }
                }

                lstDescriptions.Add(strDescription);

            }
            else if (strDescription.Contains(","))
            {
                // An enum with the [Flags] attribute set with more than one flag set
                string[] flagsArray = strDescription.Split(new char[] { ',' });

                for (int index = 0; flagsArray != null && index < flagsArray.Length; index++)
                {
                    lstDescriptions.Add(flagsArray[index].Trim());
                    lstEnumFields.Add(EnumType.GetField(flagsArray[index].Trim()));
                }
            }
            
            try
            {
                // Update the list of descriptions from the EnumDescription attribute
                for (int ndx = 0; ndx < lstEnumFields.Count; ndx++)
                {
                    FieldInfo currentField = lstEnumFields[ndx];

                    if (currentField.IsDefined(typeof(EnumDescriptionAttribute), false))
                    {
                        EnumDescriptionAttribute[] DescriptionAttributes = (EnumDescriptionAttribute[])currentField.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

                        if (DescriptionAttributes.Length > 0)
                        {
                            lstDescriptions[ndx] = DescriptionAttributes[0].Description;
                        }
                    }
                }

                // Build the string to return if no exception was thrown. 
                StringBuilder str = new StringBuilder();
                for (int ndx = 0; ndx < lstDescriptions.Count; ndx++)
                {
                    str.Append(lstDescriptions[ndx]);

                    if (string.IsNullOrEmpty(separator) == false && ndx < lstDescriptions.Count - 1)
                    {
                        str.Append(separator);
                    }
                }

                strDescription = str.ToString();
            }
            catch (Exception)
            {
                // This probably means that the enum is not marked with the attribute so we should just return the default
            }

            return strDescription;
        }

        /// <summary>
        /// Extension method for enumerations that will return the Event table info specified using a EventTableInfoAttribute modifier.
        /// </summary>
        /// <param name="value">The enumeration value.</param>
        /// <returns>The description of the value.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/28/09 RCG 2.20.03        Created

        public static string RetrieveEventTableInfo(Enum value)
        {
            string strTableID = value.ToString();
            Type EnumType = value.GetType();
            FieldInfo EnumField = EnumType.GetField(value.ToString());

            try
            {
                EnumEventInfoAttribute[] EventTableID = (EnumEventInfoAttribute[])EnumField.GetCustomAttributes(typeof(EnumEventInfoAttribute), false);

                if (EventTableID.Length > 0)
                {
                    strTableID = EventTableID[0].TableID;
                }
            }
            catch (Exception)
            {
                // This probably means that the enum is not marked with the attribute so we should just return the default
            }

            return strTableID;
        }

#if(!WindowsCE)
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

        public static T ParseToEnum<T>(string strValue)
        {
            T ReturnValue = default(T);

            foreach (T EnumValue in Enum.GetValues(typeof(T)))
            {
                FieldInfo EnumInfo = typeof(T).GetField(EnumValue.ToString());
                EnumDescriptionAttribute[] Descriptions = (EnumDescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

                if (Descriptions.Length > 0)
                {
                    if (Descriptions[0].Description == strValue)
                    {
                        ReturnValue = EnumValue;
                        break;
                    }
                }
            }

            if (ReturnValue != null)
            {
                return ReturnValue;
            }
            else
            {
                throw new FormatException("Could not parse to Enum: No matching description.");
            }
        }
#endif

        /// <summary>
        /// Gets the list of possible values for an enumeration
        /// </summary>
        /// <returns>The list of values</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/07/11 RCG 2.50.02        Created

        public static IEnumerable<T> GetValues<T>()
        {
            List<T> Values = new List<T>();
            T EnumObject = (T)Activator.CreateInstance(typeof(T));

            // The Compact Framework does not support Enum.GetValues() so this will give us the equivalent results.
            foreach (FieldInfo CurrentFieldInfo in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                Values.Add((T)CurrentFieldInfo.GetValue(EnumObject));
            }

            return Values;
        }

        /// <summary>
        /// Gets the descriptions for all values in an enum
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <returns>The list of descriptions</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public static IEnumerable<string> GetValueDescriptions<T>()
        {
            List<string> Descriptions = new List<string>();
            IEnumerable<T> Values = GetValues<T>();

            foreach (T CurrentValue in Values)
            {
                Enum EnumValue = (Enum)(object)CurrentValue;

                Descriptions.Add(EnumDescriptionRetriever.RetrieveDescription(EnumValue));
            }

            return Descriptions;
        }

        #endregion
    }
}
