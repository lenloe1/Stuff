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
//                              Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// Set Request APDU
    /// </summary>
    public class SetRequestAPDU : xDLMSAPDU
    {
        #region Definitions

        /// <summary>
        /// Set Request Types
        /// </summary>
        public enum RequestTypes : byte
        {
            /// <summary>Normal</summary>
            Normal = 1,
            /// <summary>With the first data block</summary>
            WithFirstDataBlock = 2,
            /// <summary>With a data block</summary>
            WithDataBlock = 3,
            /// <summary>With a list</summary>
            WithList = 4,
            /// <summary>With a list and first data block</summary>
            WithListAndFirstDataBlock = 5,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetRequestAPDU()
        {
            m_Tag = xDLMSTags.SetRequest;

            m_RequestType = RequestTypes.Normal;
            m_Request = new SetRequestNormal();
        }

        /// <summary>
        /// Parses the Set Request from the stream
        /// </summary>
        /// <param name="apduStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream apduStream)
        {
            base.Parse(apduStream);
            DLMSBinaryReader DataReader = new DLMSBinaryReader(apduStream);

            // Setting via the property will create a new instance of the correct Request class
            RequestType = DataReader.ReadEnum<RequestTypes>();

            m_Request.Parse(apduStream);
        }

        /// <summary>
        /// Gets the type that should be expected as the request for a specific Request Type
        /// </summary>
        /// <param name="requestType">The Expected request type</param>
        /// <returns>The type of the request</returns>
        public static Type GetExpectedRequestType(RequestTypes requestType)
        {
            Type TypeExpected = null;

            switch (requestType)
            {
                case RequestTypes.Normal:
                {
                    TypeExpected = typeof(SetRequestNormal);
                    break;
                }
                case RequestTypes.WithDataBlock:
                {
                    TypeExpected = typeof(SetRequestWithDataBlock);
                    break;
                }
                case RequestTypes.WithFirstDataBlock:
                {
                    TypeExpected = typeof(SetRequestWithFirstDataBlock);
                    break;
                }
                case RequestTypes.WithList:
                {
                    TypeExpected = typeof(SetRequestWithList);
                    break;
                }
                case RequestTypes.WithListAndFirstDataBlock:
                {
                    TypeExpected = typeof(SetRequestWithListAndFirstDataBlock);
                    break;
                }
            }

            return TypeExpected;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the Set Request
        /// </summary>
        /// <returns>The Memory Stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteEnum<RequestTypes>(m_RequestType);
            DataWriter.Write(m_Request.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the request type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public RequestTypes RequestType
        {
            get
            {
                return m_RequestType;
            }
            set
            {
                m_RequestType = value;

                switch (m_RequestType)
                {
                    case RequestTypes.Normal:
                    {
                        m_Request = new SetRequestNormal();
                        break;
                    }
                    case RequestTypes.WithDataBlock:
                    {
                        m_Request = new SetRequestWithDataBlock();
                        break;
                    }
                    case RequestTypes.WithFirstDataBlock:
                    {
                        m_Request = new SetRequestWithFirstDataBlock();
                        break;
                    }
                    case RequestTypes.WithList:
                    {
                        m_Request = new SetRequestWithList();
                        break;
                    }
                    case RequestTypes.WithListAndFirstDataBlock:
                    {
                        m_Request = new SetRequestWithListAndFirstDataBlock();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetRequest Request
        {
            get
            {
                return m_Request;
            }
            set
            {
                if (value != null)
                {
                    if (value.GetType() == GetExpectedRequestType(m_RequestType))
                    {
                        m_Request = value;
                    }
                    else
                    {
                        throw new ArgumentException("The Request data type must match the specified request type");
                    }
                }
                else
                {
                    throw new ArgumentNullException("The Request may not be set to null.");
                }
            }
        }

        #endregion

        #region Member Variables

        private RequestTypes m_RequestType;
        private SetRequest m_Request;

        #endregion
    }

    /// <summary>
    /// Set Request data
    /// </summary>
    public class SetRequest
    {
        #region Constants

        private const byte INVOKE_ID_MASK = 0x0F;
        private const byte SERVICE_CLASS_MASK = 0x40;
        private const byte PRIORITY_MASK = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetRequest()
        {
            m_InvokeIDAndPriority = 0;
        }

        /// <summary>
        /// Parses the Set Request from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse the request from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public virtual void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_InvokeIDAndPriority = DataReader.ReadByte();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the set request
        /// </summary>
        /// <returns>The stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected virtual MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = new MemoryStream();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_InvokeIDAndPriority);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Invoke ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte InvokeID
        {
            get
            {
                return (byte)(m_InvokeIDAndPriority & INVOKE_ID_MASK);
            }
            set
            {
                // This value can only be 0 - 15 but lets assign it the value mod 16 so that we can increment the value and have it roll
                // over like a normal byte would do when reaching the max value
                m_InvokeIDAndPriority = (byte)((m_InvokeIDAndPriority & ~INVOKE_ID_MASK) | (value % 16));
            }
        }

        /// <summary>
        /// Gets or sets the Service Class
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ServiceClasses ServiceClass
        {
            get
            {
                return (ServiceClasses)(m_InvokeIDAndPriority & SERVICE_CLASS_MASK);
            }
            set
            {
                m_InvokeIDAndPriority = (byte)((m_InvokeIDAndPriority & ~SERVICE_CLASS_MASK) | (byte)value);
            }
        }

        /// <summary>
        /// Gets or sets the Priority
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public Priorities Priority
        {
            get
            {
                return (Priorities)(m_InvokeIDAndPriority & PRIORITY_MASK);
            }
            set
            {
                m_InvokeIDAndPriority = (byte)((m_InvokeIDAndPriority & ~PRIORITY_MASK) | (byte)value);
            }
        }

        /// <summary>
        /// Gets the raw data for the Set Request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] Data
        {
            get
            {
                return GenerateRawData().ToArray();
            }
        }

        #endregion

        #region Member Variables

        private byte m_InvokeIDAndPriority;

        #endregion
    }

    /// <summary>
    /// Normal Set Request 
    /// </summary>
    public class SetRequestNormal : SetRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetRequestNormal()
        {
            m_AttributeDescriptor = new CosemAttributeDescriptor();
            m_AccessSelection = null;
            m_Value = new COSEMData();
        }

        /// <summary>
        /// Parses the data from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            base.Parse(dataStream);

            m_AttributeDescriptor = new CosemAttributeDescriptor();
            m_AttributeDescriptor.Parse(dataStream);

            // The selector is optional so read the usage flag
            if (DataReader.ReadUsageFlag() == true)
            {
                m_AccessSelection = new SelectiveAccessDescriptor();
                m_AccessSelection.Parse(dataStream);
            }
            else
            {
                m_AccessSelection = null;
            }

            m_Value = new COSEMData();
            m_Value.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the set request
        /// </summary>
        /// <returns>The Memory Stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_AttributeDescriptor.Data);

            // The Access Descriptor is optional
            if (m_AccessSelection == null)
            {
                DataWriter.WriteUsageFlag(false);
            }
            else
            {
                DataWriter.WriteUsageFlag(true);
                DataWriter.Write(m_AccessSelection.Data);
            }

            DataWriter.Write(m_Value.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Attribute Descriptor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public CosemAttributeDescriptor AttributeDescriptor
        {
            get
            {
                return m_AttributeDescriptor;
            }
            set
            {
                if (value != null)
                {
                    m_AttributeDescriptor = value;
                }
                else
                {
                    throw new ArgumentNullException("The Attribute Descriptor may not be set to null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Access Selection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SelectiveAccessDescriptor AccessSelection
        {
            get
            {
                return m_AccessSelection;
            }
            set
            {
                m_AccessSelection = value;
            }
        }

        /// <summary>
        /// Gets or sets the Value to set
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public COSEMData Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                if (value != null)
                {
                    m_Value = value;
                }
                else
                {
                    throw new ArgumentNullException("The Value may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private CosemAttributeDescriptor m_AttributeDescriptor;
        private SelectiveAccessDescriptor m_AccessSelection;
        private COSEMData m_Value;

        #endregion
    }

    /// <summary>
    /// Set Request with first data block
    /// </summary>
    public class SetRequestWithFirstDataBlock : SetRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetRequestWithFirstDataBlock()
        {
            m_AttributeDescriptor = new CosemAttributeDescriptor();
            m_AccessSelection = null;
            m_DataBlock = new DataBlock();
        }

        /// <summary>
        /// Parses the data from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            base.Parse(dataStream);

            m_AttributeDescriptor = new CosemAttributeDescriptor();
            m_AttributeDescriptor.Parse(dataStream);

            // The selector is optional so read the usage flag
            if (DataReader.ReadUsageFlag() == true)
            {
                m_AccessSelection = new SelectiveAccessDescriptor();
                m_AccessSelection.Parse(dataStream);
            }
            else
            {
                m_AccessSelection = null;
            }

            m_DataBlock = new DataBlock();
            m_DataBlock.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the set request
        /// </summary>
        /// <returns>The Memory Stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_AttributeDescriptor.Data);

            // The Access Descriptor is optional
            if (m_AccessSelection == null)
            {
                DataWriter.WriteUsageFlag(false);
            }
            else
            {
                DataWriter.WriteUsageFlag(true);
                DataWriter.Write(m_AccessSelection.Data);
            }

            DataWriter.Write(m_DataBlock.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Attribute Descriptor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public CosemAttributeDescriptor AttributeDescriptor
        {
            get
            {
                return m_AttributeDescriptor;
            }
            set
            {
                if (value != null)
                {
                    m_AttributeDescriptor = value;
                }
                else
                {
                    throw new ArgumentNullException("The Attribute Descriptor may not be set to null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Access Selection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SelectiveAccessDescriptor AccessSelection
        {
            get
            {
                return m_AccessSelection;
            }
            set
            {
                m_AccessSelection = value;
            }
        }

        /// <summary>
        /// Gets or sets the Data Block to set
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DataBlock DataBlock
        {
            get
            {
                return m_DataBlock;
            }
            set
            {
                if (value != null)
                {
                    m_DataBlock = value;
                }
                else
                {
                    throw new ArgumentNullException("The Data Block may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private CosemAttributeDescriptor m_AttributeDescriptor;
        private SelectiveAccessDescriptor m_AccessSelection;
        private DataBlock m_DataBlock;

        #endregion
    }

    /// <summary>
    /// Set Request with data block
    /// </summary>
    public class SetRequestWithDataBlock : SetRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetRequestWithDataBlock()
        {
            m_DataBlock = new DataBlock();
        }

        /// <summary>
        /// Parses the data from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            base.Parse(dataStream);

            m_DataBlock = new DataBlock();
            m_DataBlock.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the set request
        /// </summary>
        /// <returns>The Memory Stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_DataBlock.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Data Block to set
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DataBlock DataBlock
        {
            get
            {
                return m_DataBlock;
            }
            set
            {
                if (value != null)
                {
                    m_DataBlock = value;
                }
                else
                {
                    throw new ArgumentNullException("The Data Block may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private DataBlock m_DataBlock;

        #endregion
    }

    /// <summary>
    /// Set Request with list
    /// </summary>
    public class SetRequestWithList : SetRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetRequestWithList()
        {
            m_AttributeDescriptorList = new List<CosemAttributeDescriptorWithSelection>();
            m_ValueList = new List<COSEMData>();
        }

        /// <summary>
        /// Parses the data from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            base.Parse(dataStream);

            int Length = DataReader.ReadLength();

            m_AttributeDescriptorList = new List<CosemAttributeDescriptorWithSelection>();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                CosemAttributeDescriptorWithSelection NewDescriptor = new CosemAttributeDescriptorWithSelection();
                NewDescriptor.Parse(dataStream);

                m_AttributeDescriptorList.Add(NewDescriptor);
            }

            Length = DataReader.ReadLength();

            m_ValueList = new List<COSEMData>();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                COSEMData NewData = new COSEMData();
                NewData.Parse(dataStream);

                m_ValueList.Add(NewData);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the set request
        /// </summary>
        /// <returns>The Memory Stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteLength(m_AttributeDescriptorList.Count);

            foreach (CosemAttributeDescriptorWithSelection CurrentDescriptor in m_AttributeDescriptorList)
            {
                DataWriter.Write(CurrentDescriptor.Data);
            }

            DataWriter.WriteLength(m_ValueList.Count);

            foreach (COSEMData CurrentData in m_ValueList)
            {
                DataWriter.Write(CurrentData.Data);
            }

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Attribute Descriptor List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<CosemAttributeDescriptorWithSelection> AttributeDescriptorList
        {
            get
            {
                return m_AttributeDescriptorList;
            }
            set
            {
                if (value != null)
                {
                    m_AttributeDescriptorList = value;
                }
                else
                {
                    throw new ArgumentNullException("The Attribute Descriptor List may not be set to null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Value List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<COSEMData> ValueList
        {
            get
            {
                return m_ValueList;
            }
            set
            {
                if (value != null)
                {
                    m_ValueList = value;
                }
                else
                {
                    throw new ArgumentNullException("The Value List may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<CosemAttributeDescriptorWithSelection> m_AttributeDescriptorList;
        private List<COSEMData> m_ValueList;

        #endregion
    }

    /// <summary>
    /// Set Request with list and first data block
    /// </summary>
    public class SetRequestWithListAndFirstDataBlock : SetRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetRequestWithListAndFirstDataBlock()
        {
            m_AttributeDescriptorList = new List<CosemAttributeDescriptorWithSelection>();
            m_DataBlock = new DataBlock();
        }

        /// <summary>
        /// Parses the data from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            base.Parse(dataStream);

            int Length = DataReader.ReadLength();

            m_AttributeDescriptorList = new List<CosemAttributeDescriptorWithSelection>();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                CosemAttributeDescriptorWithSelection NewDescriptor = new CosemAttributeDescriptorWithSelection();
                NewDescriptor.Parse(dataStream);

                m_AttributeDescriptorList.Add(NewDescriptor);
            }

            m_DataBlock = new DataBlock();
            m_DataBlock.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the set request
        /// </summary>
        /// <returns>The Memory Stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteLength(m_AttributeDescriptorList.Count);

            foreach (CosemAttributeDescriptorWithSelection CurrentDescriptor in m_AttributeDescriptorList)
            {
                DataWriter.Write(CurrentDescriptor.Data);
            }

            DataWriter.Write(m_DataBlock.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Attribute Descriptor List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<CosemAttributeDescriptorWithSelection> AttributeDescriptorList
        {
            get
            {
                return m_AttributeDescriptorList;
            }
            set
            {
                if (value != null)
                {
                    m_AttributeDescriptorList = value;
                }
                else
                {
                    throw new ArgumentNullException("The Attribute Descriptor List may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<CosemAttributeDescriptorWithSelection> m_AttributeDescriptorList;
        private DataBlock m_DataBlock;

        #endregion
    }

    /// <summary>
    /// Set Response APDU
    /// </summary>
    public class SetResponseAPDU : xDLMSAPDU
    {
        #region Definitions

        /// <summary>
        /// The Set Response Types
        /// </summary>
        public enum ResponseTypes : byte
        {
            /// <summary>Normal</summary>
            Normal = 1,
            /// <summary>Data Block</summary>
            DataBlock = 2,
            /// <summary>Last Data Block</summary>
            LastDataBlock = 3,
            /// <summary>Last Data Block with List</summary>
            LastDataBlockWithList = 4,
            /// <summary>With List</summary>
            WithList = 5,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetResponseAPDU()
        {
            m_Tag = xDLMSTags.SetResponse;
            ResponseType = ResponseTypes.Normal;
        }

        /// <summary>
        /// Parses the Set Response APDU from the stream
        /// </summary>
        /// <param name="apduStream">The stream to parse the APDU from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream apduStream)
        {
            base.Parse(apduStream);
            DLMSBinaryReader DataReader = new DLMSBinaryReader(apduStream);

            ResponseType = DataReader.ReadEnum<ResponseTypes>();

            m_Response.Parse(apduStream);
        }

        /// <summary>
        /// Gets the Type of the Response for the specified Response Type
        /// </summary>
        /// <param name="responseType">The Response Type to check</param>
        /// <returns>The Type of the Response</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public static Type GetExpectedResponseType(ResponseTypes responseType)
        {
            Type ExpectedType = null;

            switch (responseType)
            {
                case ResponseTypes.Normal:
                {
                    ExpectedType = typeof(SetResponseNormal);
                    break;
                }
                case ResponseTypes.DataBlock:
                {
                    ExpectedType = typeof(SetResponseDataBlock);
                    break;
                }
                case ResponseTypes.LastDataBlock:
                {
                    ExpectedType = typeof(SetResponseLastDataBlock);
                    break;
                }
                case ResponseTypes.LastDataBlockWithList:
                {
                    ExpectedType = typeof(SetResponseLastDataBlockWithList);
                    break;
                }
                case ResponseTypes.WithList:
                {
                    ExpectedType = typeof(SetResponseWithList);
                    break;
                }
            }

            return ExpectedType;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the Set Response
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteEnum<ResponseTypes>(m_ResponseType);
            DataWriter.Write(m_Response.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Response Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ResponseTypes ResponseType
        {
            get
            {
                return m_ResponseType;
            }
            set
            {
                m_ResponseType = value;

                switch (m_ResponseType)
                {
                    case ResponseTypes.Normal:
                    {
                        m_Response = new SetResponseNormal();
                        break;
                    }
                    case ResponseTypes.DataBlock:
                    {
                        m_Response = new SetResponseDataBlock();
                        break;
                    }
                    case ResponseTypes.LastDataBlock:
                    {
                        m_Response = new SetResponseLastDataBlock();
                        break;
                    }
                    case ResponseTypes.LastDataBlockWithList:
                    {
                        m_Response = new SetResponseLastDataBlockWithList();
                        break;
                    }
                    case ResponseTypes.WithList:
                    {
                        m_Response = new SetResponseWithList();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Response
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetResponse Response
        {
            get
            {
                return m_Response;
            }
            set
            {
                if (value != null)
                {
                    if (value.GetType() == GetExpectedResponseType(m_ResponseType))
                    {
                        m_Response = value;
                    }
                    else
                    {
                        throw new ArgumentException("The type of the Response does not match the expected Response Type");
                    }
                }
                else
                {
                    throw new ArgumentNullException("");
                }
            }
        }

        #endregion

        #region Member Variables

        private ResponseTypes m_ResponseType;
        private SetResponse m_Response;

        #endregion
    }

    /// <summary>
    /// The Set Response
    /// </summary>
    public class SetResponse
    {
        #region Constants

        private const byte INVOKE_ID_MASK = 0x0F;
        private const byte SERVICE_CLASS_MASK = 0x40;
        private const byte PRIORITY_MASK = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetResponse()
        {
            m_InvokeIDAndPriority = 0;
        }

        /// <summary>
        /// Parses the Set Response from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse the response from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public virtual void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_InvokeIDAndPriority = DataReader.ReadByte();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the set response
        /// </summary>
        /// <returns>The stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected virtual MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = new MemoryStream();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_InvokeIDAndPriority);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Invoke ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte InvokeID
        {
            get
            {
                return (byte)(m_InvokeIDAndPriority & INVOKE_ID_MASK);
            }
            set
            {
                // This value can only be 0 - 15 but lets assign it the value mod 16 so that we can increment the value and have it roll
                // over like a normal byte would do when reaching the max value
                m_InvokeIDAndPriority = (byte)((m_InvokeIDAndPriority & ~INVOKE_ID_MASK) | (value % 16));
            }
        }

        /// <summary>
        /// Gets or sets the Service Class
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ServiceClasses ServiceClass
        {
            get
            {
                return (ServiceClasses)(m_InvokeIDAndPriority & SERVICE_CLASS_MASK);
            }
            set
            {
                m_InvokeIDAndPriority = (byte)((m_InvokeIDAndPriority & ~SERVICE_CLASS_MASK) | (byte)value);
            }
        }

        /// <summary>
        /// Gets or sets the Priority
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public Priorities Priority
        {
            get
            {
                return (Priorities)(m_InvokeIDAndPriority & PRIORITY_MASK);
            }
            set
            {
                m_InvokeIDAndPriority = (byte)((m_InvokeIDAndPriority & ~PRIORITY_MASK) | (byte)value);
            }
        }

        /// <summary>
        /// Gets the raw data for the Set Response
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] Data
        {
            get
            {
                return GenerateRawData().ToArray();
            }
        }

        #endregion

        #region Member Variables

        private byte m_InvokeIDAndPriority;

        #endregion
    }

    /// <summary>
    /// Normal Set Response
    /// </summary>
    public class SetResponseNormal : SetResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetResponseNormal()
        {
            m_Result = DataAccessResults.Success;
        }

        /// <summary>
        /// Parses the response from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);
            base.Parse(dataStream);

            m_Result = DataReader.ReadEnum<DataAccessResults>();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the response
        /// </summary>
        /// <returns>The MemoryStream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteEnum<DataAccessResults>(m_Result);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the result
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DataAccessResults Result
        {
            get
            {
                return m_Result;
            }
            set
            {
                m_Result = value;
            }
        }

        #endregion

        #region Member Variables

        private DataAccessResults m_Result;

        #endregion
    }

    /// <summary>
    /// Set Response with Data Block
    /// </summary>
    public class SetResponseDataBlock : SetResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetResponseDataBlock()
        {
            m_BlockNumber = 0;
        }

        /// <summary>
        /// Parses the response from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);
            base.Parse(dataStream);

            m_BlockNumber = DataReader.ReadUInt32();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the response
        /// </summary>
        /// <returns>The MemoryStream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_BlockNumber);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the block number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public uint BlockNumber
        {
            get
            {
                return m_BlockNumber;
            }
            set
            {
                m_BlockNumber = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_BlockNumber;

        #endregion
    }

    /// <summary>
    /// Set Response with last data block
    /// </summary>
    public class SetResponseLastDataBlock : SetResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetResponseLastDataBlock()
        {
            m_Result = DataAccessResults.Success;
            m_BlockNumber = 0;
        }

        /// <summary>
        /// Parses the response from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);
            base.Parse(dataStream);

            m_Result = DataReader.ReadEnum<DataAccessResults>();
            m_BlockNumber = DataReader.ReadUInt32();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the response
        /// </summary>
        /// <returns>The MemoryStream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteEnum<DataAccessResults>(m_Result);
            DataWriter.Write(m_BlockNumber);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the result
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DataAccessResults Result
        {
            get
            {
                return m_Result;
            }
            set
            {
                m_Result = value;
            }
        }

        /// <summary>
        /// Gets or sets the block number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public uint BlockNumber
        {
            get
            {
                return m_BlockNumber;
            }
            set
            {
                m_BlockNumber = value;
            }
        }

        #endregion

        #region Member Variables

        private DataAccessResults m_Result;
        private uint m_BlockNumber;

        #endregion
    }

    /// <summary>
    /// Set Response with last data block and list
    /// </summary>
    public class SetResponseLastDataBlockWithList : SetResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetResponseLastDataBlockWithList()
        {
            m_Results = new List<DataAccessResults>();
            m_BlockNumber = 0;
        }

        /// <summary>
        /// Parses the response from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);
            base.Parse(dataStream);

            m_Results = new List<DataAccessResults>();
            int Length = DataReader.ReadLength();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                m_Results.Add(DataReader.ReadEnum<DataAccessResults>());
            }

            m_BlockNumber = DataReader.ReadUInt32();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the response
        /// </summary>
        /// <returns>The MemoryStream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteLength(m_Results.Count);

            foreach (DataAccessResults CurrentResult in m_Results)
            {
                DataWriter.WriteEnum<DataAccessResults>(CurrentResult);
            }

            DataWriter.Write(m_BlockNumber);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the block number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public uint BlockNumber
        {
            get
            {
                return m_BlockNumber;
            }
            set
            {
                m_BlockNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Results
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<DataAccessResults> Results
        {
            get
            {
                return m_Results;
            }
            set
            {
                if (value != null)
                {
                    m_Results = value;
                }
                else
                {
                    throw new ArgumentNullException("The Results value may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<DataAccessResults> m_Results;
        private uint m_BlockNumber;

        #endregion
    }

    /// <summary>
    /// Set Response with list
    /// </summary>
    public class SetResponseWithList : SetResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public SetResponseWithList()
        {
            m_Results = new List<DataAccessResults>();
        }

        /// <summary>
        /// Parses the response from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);
            base.Parse(dataStream);

            m_Results = new List<DataAccessResults>();
            int Length = DataReader.ReadLength();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                m_Results.Add(DataReader.ReadEnum<DataAccessResults>());
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the response
        /// </summary>
        /// <returns>The MemoryStream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteLength(m_Results.Count);

            foreach (DataAccessResults CurrentResult in m_Results)
            {
                DataWriter.WriteEnum<DataAccessResults>(CurrentResult);
            }

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Results
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<DataAccessResults> Results
        {
            get
            {
                return m_Results;
            }
            set
            {
                if (value != null)
                {
                    m_Results = value;
                }
                else
                {
                    throw new ArgumentNullException("The Results value may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<DataAccessResults> m_Results;

        #endregion
    }
}
