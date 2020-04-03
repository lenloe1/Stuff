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
    /// Get Request APDU
    /// </summary>
    public class GetRequestAPDU : xDLMSAPDU
    {
        #region Definitions

        /// <summary>
        /// The Request Choice type
        /// </summary>
        public enum GetRequestChoice : byte
        {
            /// <summary>Get Request Normal</summary>
            Normal = 1,
            /// <summary>Get Request Next</summary>
            Next = 2,
            /// <summary>Get Request With List</summary>
            WithList = 3,
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

        public GetRequestAPDU()
        {
            m_Tag = xDLMSTags.GetRequest;

            m_RequestType = GetRequestChoice.Normal;
            m_Request = new GetRequestNormal();
        }

        /// <summary>
        /// Parses the APDU from the stream
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

            RequestType = DataReader.ReadEnum<GetRequestChoice>();

            m_Request.Parse(apduStream);
        }

        /// <summary>
        /// Gets the type that the Request value should be for the specified Request choice
        /// </summary>
        /// <param name="choice">The choice to get the type for</param>
        /// <returns>The Type for the Request object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public static Type GetExpectedRequestType(GetRequestChoice choice)
        {
            Type ExpectedType = null;

            switch (choice)
            {
                case GetRequestChoice.Normal:
                {
                    ExpectedType = typeof(GetRequestNormal);
                    break;
                }
                case GetRequestChoice.Next:
                {
                    ExpectedType = typeof(GetRequestNext);
                    break;
                }
                case GetRequestChoice.WithList:
                {
                    ExpectedType = typeof(GetRequestWithList);
                    break;
                }
            }

            return ExpectedType;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the get request
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

            DataWriter.WriteEnum<GetRequestChoice>(m_RequestType);
            DataWriter.Write(m_Request.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the request type. Note: Setting this will clear the current Request object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetRequestChoice RequestType
        {
            get
            {
                return m_RequestType;
            }
            set
            {
                m_RequestType = value;

                // Set the Get Request object to the appropriate instance
                switch (m_RequestType)
                {
                    case GetRequestChoice.Normal:
                    {
                        m_Request = new GetRequestNormal();
                        break;
                    }
                    case GetRequestChoice.Next:
                    {
                        m_Request = new GetRequestNext();
                        break;
                    }
                    case GetRequestChoice.WithList:
                    {
                        m_Request = new GetRequestWithList();
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

        public GetRequest Request
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
                        throw new ArgumentException("The type of the Request must match the current RequestType");
                    }
                }
                else
                {
                    throw new ArgumentNullException("The Request object may not be null");
                }
            }
        }

        #endregion

        #region Member Variables

        private GetRequestChoice m_RequestType;
        private GetRequest m_Request;

        #endregion
    }

    /// <summary>
    /// Base class for a Get Request 
    /// </summary>
    public abstract class GetRequest
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

        public GetRequest()
        {
            m_InvokeIDAndPriority = 0;
        }

        /// <summary>
        /// Parses the Get Request from the stream
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
        /// Generates the raw data for the get request
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
        /// Gets the raw data for the Get Request
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
    /// Normal Get Request data
    /// </summary>
    public class GetRequestNormal : GetRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetRequestNormal()
        {
            m_AttributeDescriptor = new CosemAttributeDescriptor();
            m_AccessSelection = null;
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
            // Parse the Invoke ID and Priority bit first
            base.Parse(dataStream);

            // Parse the Get Request Normal specific items
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_AttributeDescriptor = new CosemAttributeDescriptor();
            m_AttributeDescriptor.Parse(dataStream);

            // The Access Selection is optional so we need to read the usage flag
            if (DataReader.ReadUsageFlag() == true)
            {
                m_AccessSelection = new SelectiveAccessDescriptor();
                m_AccessSelection.Parse(dataStream);
            }
            else
            {
                m_AccessSelection = null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the Get Request
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

            if (m_AccessSelection == null)
            {
                // The Access Selection is not included so just write the usage flag
                DataWriter.WriteUsageFlag(false);
            }
            else
            {
                // It's included so write the usage flag and access selection
                DataWriter.WriteUsageFlag(true);
                DataWriter.Write(m_AccessSelection.Data);
            }

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
                    throw new ArgumentNullException("The Attribute Descriptor may not be null");
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

        #endregion

        #region Member Variables

        private CosemAttributeDescriptor m_AttributeDescriptor;
        private SelectiveAccessDescriptor m_AccessSelection;

        #endregion
    }

    /// <summary>
    /// Get Request Next data
    /// </summary>
    public class GetRequestNext : GetRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetRequestNext()
        {
            m_BlockNumber = 0;
        }

        /// <summary>
        /// Parses the Get Request from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            // Parse the invoke ID and priority first
            base.Parse(dataStream);

            // Parse the Get Request Next specific values
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_BlockNumber = DataReader.ReadUInt32();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the get request
        /// </summary>
        /// <returns>The Memory Stream containing the Get Request</returns>
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
        /// Gets or sets the block number to get
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
    /// Get Request with list data
    /// </summary>
    public class GetRequestWithList : GetRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetRequestWithList()
        {
            m_AttributeDescriptorList = new List<CosemAttributeDescriptorWithSelection>();
        }

        /// <summary>
        /// Parses the Get Request from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse the data from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            base.Parse(dataStream);

            // Read the length
            int Length = DataReader.ReadLength();

            // Read the descriptors
            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                CosemAttributeDescriptorWithSelection CurrentDescriptor = new CosemAttributeDescriptorWithSelection();
                CurrentDescriptor.Parse(dataStream);

                m_AttributeDescriptorList.Add(CurrentDescriptor);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the Get Request
        /// </summary>
        /// <returns>The Memory stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            // Write the length
            DataWriter.WriteLength(m_AttributeDescriptorList.Count);

            // Write the entries
            foreach (CosemAttributeDescriptorWithSelection CurrentDescriptor in m_AttributeDescriptorList)
            {
                DataWriter.Write(CurrentDescriptor.Data);
            }

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the attribute descriptor list
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
                    throw new ArgumentNullException("The Attribute Descriptor List may not be null");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<CosemAttributeDescriptorWithSelection> m_AttributeDescriptorList;

        #endregion
    }

    /// <summary>
    /// APDU for the Get Response
    /// </summary>
    public class GetResponseAPDU : xDLMSAPDU
    {
        #region Definitions

        /// <summary>
        /// The Get Response choices
        /// </summary>
        public enum GetResponseChoices : byte
        {
            /// <summary>Normal</summary>
            Normal = 1,
            /// <summary>With Data Block</summary>
            WithDataBlock = 2,
            /// <summary>With List</summary>
            WithList = 3,
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

        public GetResponseAPDU()
        {
            m_Tag = xDLMSTags.GetResponse;

            m_ResponseType = GetResponseChoices.Normal;
            m_Response = new GetResponseNormal();
        }

        /// <summary>
        /// Parses the APDU from the stream
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

            ResponseType = DataReader.ReadEnum<GetResponseChoices>();

            // The Response data will be set by the ResponseType property so we can just call parse
            m_Response.Parse(apduStream);
        }

        /// <summary>
        /// Gets the type of response that should be returned for a specific Response Type
        /// </summary>
        /// <param name="choice">The response type to get the type for</param>
        /// <returns>The type of the response</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public static Type GetExpectedType(GetResponseChoices choice)
        {
            Type ExpectedType = null;

            switch (choice)
            {
                case GetResponseChoices.Normal:
                {
                    ExpectedType = typeof(GetResponseNormal);
                    break;
                }
                case GetResponseChoices.WithDataBlock:
                {
                    ExpectedType = typeof(GetResponseWithDatablock);
                    break;
                }
                case GetResponseChoices.WithList:
                {
                    ExpectedType = typeof(GetResponseWithList);
                    break;
                }
            }

            return ExpectedType;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the specified object
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

            DataWriter.WriteEnum<GetResponseChoices>(m_ResponseType);
            DataWriter.Write(m_Response.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the response type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetResponseChoices ResponseType
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
                    case GetResponseChoices.Normal:
                    {
                        m_Response = new GetResponseNormal();
                        break;
                    }
                    case GetResponseChoices.WithDataBlock:
                    {
                        m_Response = new GetResponseWithDatablock();
                        break;
                    }
                    case GetResponseChoices.WithList:
                    {
                        m_Response = new GetResponseWithList();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the response
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetResponse Response
        {
            get
            {
                return m_Response;
            }
            set
            {
                if (value != null)
                {
                    if (value.GetType() == GetExpectedType(m_ResponseType))
                    {
                        m_Response = value;
                    }
                    else
                    {
                        throw new ArgumentException("The Response value must be set to the correct Get Response class for the current Request Type");
                    }
                }
                else
                {
                    throw new ArgumentNullException("The response may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private GetResponseChoices m_ResponseType;
        private GetResponse m_Response;

        #endregion
    }

    /// <summary>
    /// Get Response data
    /// </summary>
    public class GetResponse
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

        public GetResponse()
        {
            m_InvokeIDAndPriority = 0;
        }

        /// <summary>
        /// Parses the Get Response from the stream
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
        /// Generates the raw data for the get response
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
        /// Gets the raw data for the Get Request
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
    /// Normal Get Response
    /// </summary>
    public class GetResponseNormal : GetResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetResponseNormal()
        {
            m_Result = new GetDataResult();
        }

        /// <summary>
        /// Parses the Get Response from the stream
        /// </summary>
        /// <param name="dataStream">The data stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            base.Parse(dataStream);

            m_Result = new GetDataResult();
            m_Result.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the Get Response
        /// </summary>
        /// <returns>The memory stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_Result.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the result data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetDataResult Result
        {
            get
            {
                return m_Result;
            }
            set
            {
                if (value != null)
                {
                    m_Result = value;
                }
                else
                {
                    throw new ArgumentNullException("The Result value may not be null");
                }
            }
        }

        #endregion

        #region Member Variables

        private GetDataResult m_Result;

        #endregion
    }

    /// <summary>
    /// Get Response with data block
    /// </summary>
    public class GetResponseWithDatablock : GetResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetResponseWithDatablock()
        {
            m_Result = new DataBlockResponse();
        }

        /// <summary>
        /// Parses the Get Response from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse the Get Response from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Parse(Stream dataStream)
        {
            base.Parse(dataStream);

            m_Result = new DataBlockResponse();
            m_Result.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the Get Response
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

            DataWriter.Write(m_Result.Data);

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

        public DataBlockResponse Result
        {
            get
            {
                return m_Result;
            }
            set
            {
                if (value != null)
                {
                    m_Result = value;
                }
                else
                {
                    throw new ArgumentNullException("The Result may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private DataBlockResponse m_Result;

        #endregion
    }

    /// <summary>
    /// Get Response with list
    /// </summary>
    public class GetResponseWithList : GetResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetResponseWithList()
        {
            m_Result = new List<GetDataResult>();
        }

        /// <summary>
        /// Parses the Get Response from the stream
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

            // Get the length of the list
            int Length = DataReader.ReadLength();

            m_Result = new List<GetDataResult>();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                GetDataResult NewResult = new GetDataResult();
                NewResult.Parse(dataStream);

                m_Result.Add(NewResult);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the Get Response
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

            DataWriter.WriteLength(m_Result.Count);

            foreach (GetDataResult CurrentResult in m_Result)
            {
                DataWriter.Write(CurrentResult.Data);
            }

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Result
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<GetDataResult> Result
        {
            get
            {
                return m_Result;
            }
            set
            {
                if (value != null)
                {
                    m_Result = value;
                }
                else
                {
                    throw new ArgumentNullException("The Result may not be set to null.");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<GetDataResult> m_Result;

        #endregion
    }
}
