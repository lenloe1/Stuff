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
using System.Linq;
using System.Text;
using System.Globalization;
using Itron.Metering.Communications.DLMS;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device.DLMSDevice
{
    /// <summary>
    /// Definition for an Array data object
    /// </summary>
    public class ArrayObjectDefinition : ObjectDefinition
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemName">The name of the item</param>
        /// <param name="elementDefinition">The definition of the elements of the array</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public ArrayObjectDefinition(string itemName, ObjectDefinition elementDefinition)
            : base(itemName, COSEMDataTypes.Array)
        {
            m_ElementDefinition = elementDefinition;
            m_Elements = new List<ObjectDefinition>();
        }

        /// <summary>
        /// Gets the value as a COSEMData object
        /// </summary>
        /// <returns>The value of the object as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public override COSEMData GetCOSEMDataValue()
        {
            COSEMData ArrayCOSEMData = null;

            if (m_Elements != null)
            {
                ArrayCOSEMData = new COSEMData();
                COSEMData[] ElementData = new COSEMData[m_Elements.Count];

                for (int iIndex = 0; iIndex < m_Elements.Count; iIndex++)
                {
                    // First let's make sure that the element type is correct
                    if (m_Elements[iIndex].Equals(m_ElementDefinition))
                    {
                        ElementData[iIndex] = m_Elements[iIndex].GetCOSEMDataValue();
                    }
                    else
                    {
                        throw new InvalidOperationException("The Elements of the array object do not match the Element definition");
                    }
                }

                ArrayCOSEMData.DataType = COSEMDataTypes.Array;
                ArrayCOSEMData.Value = ElementData;
            }

            return ArrayCOSEMData;
        }

        /// <summary>
        /// Gets whether or not the objects are of the same type
        /// </summary>
        /// <param name="other">The array object definition to compare to</param>
        /// <returns>True if the objects are of the same type. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public bool Equals(ArrayObjectDefinition other)
        {
            // We already know that it's an array so we just need to make sure the element definition matches
            return ElementDefinition.Equals(other.ElementDefinition);
        }

        /// <summary>
        /// Gets whether or not the objects are of the same type
        /// </summary>
        /// <param name="other">The array object definition to compare to</param>
        /// <returns>True if the objects are of the same type. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public override bool Equals(ObjectDefinition other)
        {
            bool IsEqual = false;
            ArrayObjectDefinition Definition = other as ArrayObjectDefinition;

            if (Definition != null)
            {
                IsEqual = this.Equals(Definition);
            }

            return IsEqual;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the element definition
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public ObjectDefinition ElementDefinition
        {
            get
            {
                return m_ElementDefinition;
            }
            set
            {
                if (value != null)
                {
                    m_ElementDefinition = value;
                }
                else
                {
                    throw new ArgumentNullException("The Element Definition may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the current list of elements
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public List<ObjectDefinition> Elements
        {
            get
            {
                return m_Elements;
            }
            set
            {
                if (value != null)
                {
                    m_Elements = value;
                }
                else
                {
                    throw new ArgumentNullException("Elements may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private ObjectDefinition m_ElementDefinition;
        private List<ObjectDefinition> m_Elements;

        #endregion
    }

    /// <summary>
    /// Definition for a Structure data object
    /// </summary>
    public class StructureObjectDefinition : ObjectDefinition
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemName">The item name</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public StructureObjectDefinition(string itemName)
            : base(itemName, COSEMDataTypes.Structure)
        {
            m_StructureDefinition = new List<ObjectDefinition>();
        }

        /// <summary>
        /// Gets the value as a COSEMData object
        /// </summary>
        /// <returns>The value of the object as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public override COSEMData GetCOSEMDataValue()
        {
            COSEMData StructureData = null;

            if (m_StructureDefinition != null && m_StructureDefinition.Count > 0)
            {
                StructureData = new COSEMData();
                COSEMData[] Items = new COSEMData[m_StructureDefinition.Count];

                for (int iIndex = 0; iIndex < m_StructureDefinition.Count; iIndex++)
                {
                    Items[iIndex] = m_StructureDefinition[iIndex].GetCOSEMDataValue();
                }

                StructureData.DataType = COSEMDataTypes.Structure;
                StructureData.Value = Items;
            }

            return StructureData;
        }

        /// <summary>
        /// Gets whether or not the objects are of the same type
        /// </summary>
        /// <param name="other">The object definition to compare to</param>
        /// <returns>True if the objects are of the same type. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public override bool Equals(ObjectDefinition other)
        {
            bool IsEqual = false;
            StructureObjectDefinition StructureObject = other as StructureObjectDefinition;

            // Check to see if other is a structure object
            if (StructureObject != null)
            {
                // It is so lets call the more specific Equals operator
                IsEqual = Equals(StructureObject);
            }

            return IsEqual;
        }

        /// <summary>
        /// Gets whether or not the structure objects are of the same type
        /// </summary>
        /// <param name="other">The structure object definition to compare to</param>
        /// <returns>True if the objects are of the same type. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public bool Equals(StructureObjectDefinition other)
        {
            bool IsEqual = true;

            // We already know it's a structure type but we need to make sure that all
            // of the objects in the structure definition are the same
            if (StructureDefinition.Count == other.StructureDefinition.Count)
            {
                for (int iIndex = 0; iIndex < StructureDefinition.Count; iIndex++)
                {
                    if (StructureDefinition[iIndex].Equals(other.StructureDefinition[iIndex]) == false)
                    {
                        // The structures don't match
                        IsEqual = false;
                        break;
                    }
                }
            }
            else
            {
                IsEqual = false;
            }

            return IsEqual;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the structure definition
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public List<ObjectDefinition> StructureDefinition
        {
            get
            {
                return m_StructureDefinition;
            }
            set
            {
                if (m_StructureDefinition != null)
                {
                    m_StructureDefinition = value;
                }
                else
                {
                    throw new ArgumentNullException("The Structure Definition may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<ObjectDefinition> m_StructureDefinition;

        #endregion
    }

    /// <summary>
    /// Definition for an Enum data object
    /// </summary>
    public class EnumObjectDefinition : ObjectDefinition, IEquatable<EnumObjectDefinition>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemName">The name of the item</param>
        /// <param name="enumType">The type of the enumeration</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public EnumObjectDefinition(string itemName, Type enumType)
            : base(itemName, COSEMDataTypes.Enum)
        {
            if (enumType.IsEnum || enumType.Equals(typeof(byte)))
            {
                m_EnumType = enumType;
            }
            else
            {
                throw new ArgumentException("enumType is not a valid Enumeration", "enumType");
            }
        } 

        /// <summary>
        /// Parses the value of the enum from a string
        /// </summary>
        /// <param name="value">The value to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public void ParseValue(string value)
        {
            if (m_EnumType.Equals(typeof(byte)))
            {
                Value = byte.Parse(value);
            }
            else
            {
                foreach (Enum CurrentValue in Enum.GetValues(m_EnumType))
                {
                    if (CurrentValue.ToDescription().Equals(value))
                    {
                        Value = CurrentValue;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the two Enum Objects are of the same type
        /// </summary>
        /// <param name="other">The Enum object to compare to</param>
        /// <returns>True if the definitions are of the same type. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public override bool Equals(ObjectDefinition other)
        {
            bool IsEqual = false;
            EnumObjectDefinition Definition = other as EnumObjectDefinition;

            if (Definition != null)
            {
                IsEqual = this.Equals(Definition);
            }

            return IsEqual;
        }

        /// <summary>
        /// Determines whether or not the two Enum Objects are of the same type
        /// </summary>
        /// <param name="other">The Enum object to compare to</param>
        /// <returns>True if the definitions are of the same type. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public bool Equals(EnumObjectDefinition other)
        {
            return this.EnumType.Equals(other.EnumType);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Type of the enumeration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public Type EnumType
        {
            get
            {
                return m_EnumType;
            }
        }

        /// <summary>
        /// Gets the value of the object as a COSEMData object
        /// </summary>
        /// <returns>The value as a COSEMData object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public override COSEMData GetCOSEMDataValue()
        {
            COSEMData Data = new COSEMData();

            Data.DataType = COSEMDataTypes.Enum;
            Data.Value = Convert.ToByte(Value);

            return Data;
        }

        /// <summary>
        /// Gets the list of valid values for the enumeration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public List<string> EnumValues
        {
            get
            {
                List<string> Values = new List<string>();

                if (m_EnumType.Equals(typeof(byte)))
                {
                    for (byte iIndex = 0; iIndex < byte.MaxValue; iIndex++)
                    {
                        Values.Add(iIndex.ToString(CultureInfo.InvariantCulture));
                    }
                }
                else
                {
                    foreach (Enum CurrentValue in Enum.GetValues(m_EnumType))
                    {
                        Values.Add(EnumDescriptionRetriever.RetrieveDescription(CurrentValue));
                    }
                }

                return Values;
            }
        }

        #endregion

        #region Member Variables

        private Type m_EnumType;

        #endregion
    }

    /// <summary>
    /// Definition for a data object
    /// </summary>
    public class ObjectDefinition : IEquatable<ObjectDefinition>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemName">The name of the item</param>
        /// <param name="dataType">The value of the item</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public ObjectDefinition(string itemName, COSEMDataTypes dataType)
        {
            m_ItemName = itemName;
            m_DataType = dataType;
        }

        /// <summary>
        /// Gets the value as a COSEMData object
        /// </summary>
        /// <returns>The value of the object as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public virtual COSEMData GetCOSEMDataValue()
        {
            COSEMData Data = null;

            if (m_Value != null || m_DataType == COSEMDataTypes.NullData)
            {
                Data = new COSEMData();
                
                switch(m_DataType)
                {
                    // The Date objects need to be converted to Octet Strings
                    case COSEMDataTypes.DateTime:
                    {
                        Data.DataType = COSEMDataTypes.OctetString;
                        Data.Value = ((COSEMDateTime)m_Value).Data;
                        break;
                    }
                    case COSEMDataTypes.Date:
                    {
                        Data.DataType = COSEMDataTypes.OctetString;
                        Data.Value = ((COSEMDate)m_Value).Data;
                        break;
                    }
                    case COSEMDataTypes.Time:
                    {
                        Data.DataType = COSEMDataTypes.OctetString;
                        Data.Value = ((COSEMTime)m_Value).Data;
                        break;
                    }
                    default:
                    {
                        Data.DataType = m_DataType;
                        Data.Value = m_Value;
                        break;
                    }
                }
            }

            return Data;
        }

        /// <summary>
        /// Gets whether or not the objects are of the same type
        /// </summary>
        /// <param name="other">The object definition to compare to</param>
        /// <returns>True if the objects are of the same type. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created

        public virtual bool Equals(ObjectDefinition other)
        {
            return DataType == other.DataType;
        }

        /// <summary>
        /// Creates an object definition from a COSEM Data object
        /// </summary>
        /// <param name="itemName">The name of the item</param>
        /// <param name="data">The COSEM Data object to create the definition from</param>
        /// <returns>The created object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created
        
        public static ObjectDefinition CreateFromCOSEMData(string itemName, COSEMData data)
        {
            ObjectDefinition Definition = null;

            if (data != null)
            {
                switch(data.DataType)
                {
                    case COSEMDataTypes.Array:
                    {
                        COSEMData[] ArrayData = data.Value as COSEMData[];
                        ArrayObjectDefinition ArrayDefinition = null;

                        if (ArrayData != null && ArrayData.Count() > 0)
                        {
                            ArrayDefinition = new ArrayObjectDefinition(itemName, CreateFromCOSEMData("Element Type", ArrayData[0]));

                            for(int iIndex = 0; iIndex < ArrayData.Count(); iIndex++)
                            {
                                ArrayDefinition.Elements.Add(CreateFromCOSEMData("[" + iIndex.ToString() + "]", ArrayData[iIndex]));
                            }
                        }
                        else if (ArrayData != null)
                        {
                            // The current count is 0 so we don't actually know the data type
                            ArrayDefinition = new ArrayObjectDefinition(itemName, null);
                        }

                        Definition = ArrayDefinition;
                        break;
                    }
                    case COSEMDataTypes.Structure:
                    {
                        COSEMData[] StructureData = data.Value as COSEMData[];
                        StructureObjectDefinition StructureDefinition = new StructureObjectDefinition(itemName);

                        if (StructureData != null)
                        {
                            for (int iIndex = 0; iIndex < StructureData.Count(); iIndex++)
                            {
                                StructureDefinition.StructureDefinition.Add(CreateFromCOSEMData("Element " + iIndex.ToString(), StructureData[iIndex]));
                            }
                        }

                        Definition = StructureDefinition;
                        break;
                    }
                    case COSEMDataTypes.Enum:
                    {
                        // We won't really know if there is an actual enumeration associated so we should treat this as a byte value
                        EnumObjectDefinition EnumDefinition = new EnumObjectDefinition(itemName, typeof(byte));
                        EnumDefinition.Value = data.Value;

                        Definition = EnumDefinition;
                        break;
                    }
                    default:
                    {
                        Definition = new ObjectDefinition(itemName, data.DataType);
                        Definition.Value = data.Value;
                        break;
                    }
                }
            }

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the item name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public string ItemName
        {
            get
            {
                return m_ItemName;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    m_ItemName = value;
                }
                else
                {
                    throw new ArgumentException("The Item Name may not be null or an empty string");
                }
            }
        }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public COSEMDataTypes DataType
        {
            get
            {
                return m_DataType;
            }
            set
            {
                m_DataType = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/22/13 RCG 2.80.22 N/A    Created
        
        public object Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        #endregion

        #region Member Variables

        private string m_ItemName;
        private COSEMDataTypes m_DataType;
        private object m_Value;

        #endregion
    }
}
