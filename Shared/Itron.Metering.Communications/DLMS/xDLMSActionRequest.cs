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
//                              Copyright © 2012
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
    /// Action Request APDU
    /// </summary>
    public class ActionRequestAPDU : xDLMSAPDU
    {
        #region Definitions

        /// <summary>
        /// The Action Request Types
        /// </summary>
        public enum RequestTypes : byte
        {
            /// <summary>Normal</summary>
            Normal = 1,
            /// <summary>Next Block</summary>
            NextBlock = 2,
            /// <summary>With List</summary>
            WithList = 3,
            /// <summary>With First Block</summary>
            WithFirstBlock = 4,
            /// <summary>With List and First Block</summary>
            WithListAndFirstBlock = 5,
            /// <summary>With Block</summary>
            WithBlock = 6,
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

        public ActionRequestAPDU()
        {
            m_Tag = xDLMSTags.ActionRequest;

            m_RequestType = RequestTypes.Normal;
            m_Request = new ActionRequestNormal();
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

            RequestType = DataReader.ReadEnum<RequestTypes>();

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

        public static Type GetExpectedRequestType(RequestTypes choice)
        {
            Type ExpectedType = null;

            switch (choice)
            {
                case RequestTypes.Normal:
                {
                    ExpectedType = typeof(ActionRequestNormal);
                    break;
                }
                case RequestTypes.NextBlock:
                {
                    ExpectedType = typeof(ActionRequestNextBlock);
                    break;
                }
                case RequestTypes.WithList:
                {
                    ExpectedType = typeof(ActionRequestWithList);
                    break;
                }
                case RequestTypes.WithFirstBlock:
                {
                    ExpectedType = typeof(ActionRequestWithFirstBlock);
                    break;
                }
                case RequestTypes.WithListAndFirstBlock:
                {
                    ExpectedType = typeof(ActionRequestWithListAndFirstBlock);
                    break;
                }
                case RequestTypes.WithBlock:
                {
                    ExpectedType = typeof(ActionRequestWithBlock);
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

            DataWriter.WriteEnum<RequestTypes>(m_RequestType);
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

        public RequestTypes RequestType
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
                    case RequestTypes.Normal:
                    {
                        m_Request = new ActionRequestNormal();
                        break;
                    }
                    case RequestTypes.NextBlock:
                    {
                        m_Request = new ActionRequestNextBlock();
                        break;
                    }
                    case RequestTypes.WithList:
                    {
                        m_Request = new ActionRequestWithList();
                        break;
                    }
                    case RequestTypes.WithFirstBlock:
                    {
                        m_Request = new ActionRequestWithFirstBlock();
                        break;
                    }
                    case RequestTypes.WithListAndFirstBlock:
                    {
                        m_Request = new ActionRequestWithListAndFirstBlock();
                        break;
                    }
                    case RequestTypes.WithBlock:
                    {
                        m_Request = new ActionRequestWithBlock();
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

        public ActionRequest Request
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

        private RequestTypes m_RequestType;
        private ActionRequest m_Request;

        #endregion
    }

    /// <summary>
    /// The Action Request data
    /// </summary>
    public class ActionRequest
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

        public ActionRequest()
        {
            m_InvokeIDAndPriority = 0;
        }

        /// <summary>
        /// Parses the Action Request from the stream
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
        /// Generates the raw data for the Action request
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
        /// Gets the raw data for the Action Request
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
    /// Action Request Normal
    /// </summary>
    public class ActionRequestNormal : ActionRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ActionRequestNormal()
        {
            m_MethodDescriptor = new CosemMethodDescriptor();
            m_Parameters = null;
        }

        /// <summary>
        /// Parses the Action Request from the stream
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

            m_MethodDescriptor.Parse(dataStream);

            // The Parameters are optional so read the usage flag
            if (DataReader.ReadUsageFlag() == true)
            {
                m_Parameters = new COSEMData();
                m_Parameters.Parse(dataStream);
            }
            else
            {
                m_Parameters = null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the Raw Data for the request
        /// </summary>
        /// <returns>The Memory Stream containing the data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_MethodDescriptor.Data);

            if (m_Parameters != null)
            {
                DataWriter.WriteUsageFlag(true);
                DataWriter.Write(m_Parameters.Data);
            }
            else
            {
                DataWriter.WriteUsageFlag(false);
            }

            return DataStream;
        }

        #endregion

        #region Public Propetries

        /// <summary>
        /// Gets or sets the method descriptor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public CosemMethodDescriptor MethodDescriptor
        {
            get
            {
                return m_MethodDescriptor;
            }
            set
            {
                if (value != null)
                {
                    m_MethodDescriptor = value;
                }
                else
                {
                    throw new ArgumentNullException("The Method Descriptor may not be set to null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Parameters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMData Parameters
        {
            get
            {
                return m_Parameters;
            }
            set
            {
                m_Parameters = value;
            }
        }

        #endregion

        #region Member Variables

        private CosemMethodDescriptor m_MethodDescriptor;
        private COSEMData m_Parameters;

        #endregion
    }

    /// <summary>
    /// Action Request Data with the next block
    /// </summary>
    public class ActionRequestNextBlock : ActionRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ActionRequestNextBlock()
        {
            m_BlockNumber = 0;
        }

        /// <summary>
        /// Parses the Action Request from the stream
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
        /// Generates the Raw Data for the request
        /// </summary>
        /// <returns>The Memory Stream containing the data</returns>
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

        #region Public Propetries

        /// <summary>
        /// Gets or sets the Block number for the action
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
    /// Action Request data with a list
    /// </summary>
    public class ActionRequestWithList : ActionRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ActionRequestWithList()
        {
            m_MethodDescriptorList = new List<CosemMethodDescriptor>();
            m_ParameterList = new List<COSEMData>();
        }

        /// <summary>
        /// Parses the request from the stream
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

            // Parse the Method Descriptor List
            m_MethodDescriptorList = new List<CosemMethodDescriptor>();

            int Length = DataReader.ReadLength();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                CosemMethodDescriptor NewMethodDescriptor = new CosemMethodDescriptor();
                NewMethodDescriptor.Parse(dataStream);

                m_MethodDescriptorList.Add(NewMethodDescriptor);
            }

            // Parse the Parameter list
            m_ParameterList = new List<COSEMData>();

            Length = DataReader.ReadLength();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                COSEMData NewData = new COSEMData();
                NewData.Parse(dataStream);

                m_ParameterList.Add(NewData);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the request
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

            // Write the length first
            DataWriter.WriteLength(m_MethodDescriptorList.Count);

            // Write the Descriptors
            foreach(CosemMethodDescriptor CurrentDescriptor in m_MethodDescriptorList)
            {
                DataWriter.Write(CurrentDescriptor.Data);
            }

            // Write the Parameter length
            DataWriter.WriteLength(m_ParameterList.Count);

            // Write the parameters
            foreach(COSEMData CurrentData in m_ParameterList)
            {
                DataWriter.Write(CurrentData.Data);
            }

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Method Descriptor List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public List<CosemMethodDescriptor> MethodDescriptorList
        {
            get
            {
                return m_MethodDescriptorList;
            }
            set
            {
                if (value != null)
                {
                    m_MethodDescriptorList = value;
                }
                else
                {
                    throw new ArgumentNullException("The Method Descriptor List may not be set to null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Parameter List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public List<COSEMData> ParameterList
        {
            get
            {
                return m_ParameterList;
            }
            set
            {
                if (value != null)
                {
                    m_ParameterList = value;
                }
                else
                {
                    throw new ArgumentNullException("The Parameter List may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<CosemMethodDescriptor> m_MethodDescriptorList;
        private List<COSEMData> m_ParameterList;

        #endregion
    }

    /// <summary>
    /// Action Request with the first block
    /// </summary>
    public class ActionRequestWithFirstBlock : ActionRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ActionRequestWithFirstBlock()
        {
            m_MethodDescriptor = new CosemMethodDescriptor();
            m_Block = new DataBlock();
        }

        /// <summary>
        /// Parses the request from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Parse(Stream dataStream)
        {
            base.Parse(dataStream);

            m_MethodDescriptor = new CosemMethodDescriptor();
            m_MethodDescriptor.Parse(dataStream);

            m_Block = new DataBlock();
            m_Block.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the request
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

            DataWriter.Write(m_MethodDescriptor.Data);
            DataWriter.Write(m_Block.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Method Descriptor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public CosemMethodDescriptor MethodDescriptor
        {
            get
            {
                return m_MethodDescriptor;
            }
            set
            {
                if (value != null)
                {
                    m_MethodDescriptor = value;
                }
                else
                {
                    throw new ArgumentNullException("The Method Descriptor may not be set to null");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the Block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DataBlock Block
        {
            get
            {
                return m_Block;
            }
            set
            {
                if (value != null)
                {
                    m_Block = value;
                }
                else
                {
                    throw new ArgumentNullException("The Block may not be set to null.");
                }
            }
        }

        #endregion

        #region Member Variables

        private CosemMethodDescriptor m_MethodDescriptor;
        private DataBlock m_Block;

        #endregion
    }

    /// <summary>
    /// Action request with a list and the first block
    /// </summary>
    public class ActionRequestWithListAndFirstBlock : ActionRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ActionRequestWithListAndFirstBlock()
        {
            m_MethodDescriptorList = new List<CosemMethodDescriptor>();
            m_Block = new DataBlock();
        }

        /// <summary>
        /// Parses the request from the stream
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

            // Parse the Method Descriptor List
            m_MethodDescriptorList = new List<CosemMethodDescriptor>();

            int Length = DataReader.ReadLength();

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                CosemMethodDescriptor NewMethodDescriptor = new CosemMethodDescriptor();
                NewMethodDescriptor.Parse(dataStream);

                m_MethodDescriptorList.Add(NewMethodDescriptor);
            }

            m_Block = new DataBlock();
            m_Block.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the request
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

            // Write the length first
            DataWriter.WriteLength(m_MethodDescriptorList.Count);

            // Write the Descriptors
            foreach(CosemMethodDescriptor CurrentDescriptor in m_MethodDescriptorList)
            {
                DataWriter.Write(CurrentDescriptor.Data);
            }

            DataWriter.Write(m_Block.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Method Descriptor List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<CosemMethodDescriptor> MethodDescriptorList
        {
            get
            {
                return m_MethodDescriptorList;
            }
            set
            {
                if (value != null)
                {
                    m_MethodDescriptorList = value;
                }
                else
                {
                    throw new ArgumentNullException("The Method Descriptor List may not be set to null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DataBlock Block
        {
            get
            {
                return m_Block;
            }
            set
            {
                if (value != null)
                {
                    m_Block = value;
                }
                else
                {
                    throw new ArgumentNullException("The Block may not be set to null.");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<CosemMethodDescriptor> m_MethodDescriptorList;
        private DataBlock m_Block;

        #endregion
    }

    /// <summary>
    /// Action request with a block
    /// </summary>
    public class ActionRequestWithBlock : ActionRequest
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ActionRequestWithBlock()
        {
            m_Block = new DataBlock();
        }

        /// <summary>
        /// Parses the request from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Parse(Stream dataStream)
        {
            base.Parse(dataStream);

            m_Block = new DataBlock();
            m_Block.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the request
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

            DataWriter.Write(m_Block.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DataBlock Block
        {
            get
            {
                return m_Block;
            }
            set
            {
                if (value != null)
                {
                    m_Block = value;
                }
                else
                {
                    throw new ArgumentNullException("The Block may not be set to null.");
                }
            }
        }

        #endregion

        #region Member Variables

        private DataBlock m_Block;

        #endregion
    }

    /// <summary>
    /// Action Response APDU
    /// </summary>
    public class ActionResponseAPDU : xDLMSAPDU
    {
        #region Definitions

        /// <summary>
        /// The Action Request Types
        /// </summary>
        public enum ResponseTypes : byte
        {
            /// <summary>Normal</summary>
            Normal = 1,
            /// <summary>With Block</summary>
            WithBlock = 2,
            /// <summary>With List</summary>
            WithList = 3,
            /// <summary>With First Block</summary>
            NextBlock = 4,
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

        public ActionResponseAPDU()
        {
            m_Tag = xDLMSTags.ActionResponse;

            m_ResponseType = ResponseTypes.Normal;
            m_Response = new ActionResponseNormal();
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

            ResponseType = DataReader.ReadEnum<ResponseTypes>();

            m_Response.Parse(apduStream);
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

        public static Type GetExpectedRequestType(ResponseTypes choice)
        {
            Type ExpectedType = null;

            switch (choice)
            {
                case ResponseTypes.Normal:
                {
                    ExpectedType = typeof(ActionResponseNormal);
                    break;
                }
                case ResponseTypes.WithBlock:
                {
                    ExpectedType = typeof(ActionResponseWithBlock);
                    break;
                }
                case ResponseTypes.WithList:
                {
                    ExpectedType = typeof(ActionResponseWithList);
                    break;
                }
                case ResponseTypes.NextBlock:
                {
                    ExpectedType = typeof(ActionResponseNextBlock);
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

            DataWriter.WriteEnum<ResponseTypes>(m_ResponseType);
            DataWriter.Write(m_Response.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the response type. Note: Setting this will clear the current response object
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

                // Set the Action Response object to the appropriate instance
                switch (m_ResponseType)
                {
                    case ResponseTypes.Normal:
                    {
                        m_Response = new ActionResponseNormal();
                        break;
                    }
                    case ResponseTypes.NextBlock:
                    {
                        m_Response = new ActionResponseNextBlock();
                        break;
                    }
                    case ResponseTypes.WithList:
                    {
                        m_Response = new ActionResponseWithList();
                        break;
                    }
                    case ResponseTypes.WithBlock:
                    {
                        m_Response = new ActionResponseWithBlock();
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

        public ActionResponse Response
        {
            get
            {
                return m_Response;
            }
            set
            {
                if (value != null)
                {
                    if (value.GetType() == GetExpectedRequestType(m_ResponseType))
                    {
                        m_Response = value;
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

        private ResponseTypes m_ResponseType;
        private ActionResponse m_Response;

        #endregion
    }

    /// <summary>
    /// Action Response Data
    /// </summary>
    public class ActionResponse
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

        public ActionResponse()
        {
            m_InvokeIDAndPriority = 0;
        }

        /// <summary>
        /// Parses the Action Response from the stream
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
        /// Generates the raw data for the Action Reponse
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
        /// Gets the raw data for the Action Response
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
    /// Normal Action Response Data
    /// </summary>
    public class ActionResponseNormal : ActionResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ActionResponseNormal()
        {
            m_Response = new ActionResponseWithOptionalData();
        }

        /// <summary>
        /// Parses the request from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Parse(Stream dataStream)
        {
            base.Parse(dataStream);

            m_Response = new ActionResponseWithOptionalData();
            m_Response.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data form the Action Request
        /// </summary>
        /// <returns>The Memory Stream containing the Raw Data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_Response.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Response
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ActionResponseWithOptionalData Response
        {
            get
            {
                return m_Response;
            }
            set
            {
                if (m_Response != null)
                {
                    m_Response = value;
                }
                else
                {
                    throw new ArgumentNullException("The Response may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private ActionResponseWithOptionalData m_Response;

        #endregion
    }

    /// <summary>
    /// Action Response With Block Data
    /// </summary>
    public class ActionResponseWithBlock : ActionResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ActionResponseWithBlock()
        {
            m_Block = new DataBlock();
        }

        /// <summary>
        /// Parses the request from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Parse(Stream dataStream)
        {
            base.Parse(dataStream);

            m_Block = new DataBlock();
            m_Block.Parse(dataStream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the request
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

            DataWriter.Write(m_Block.Data);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DataBlock Block
        {
            get
            {
                return m_Block;
            }
            set
            {
                if (value != null)
                {
                    m_Block = value;
                }
                else
                {
                    throw new ArgumentNullException("The Block may not be set to null.");
                }
            }
        }

        #endregion

        #region Member Variables

        private DataBlock m_Block;

        #endregion
    }

    /// <summary>
    /// Action Response With List Data
    /// </summary>
    public class ActionResponseWithList : ActionResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ActionResponseWithList()
        {
            m_Responses = new List<ActionResponseWithOptionalData>();
        }

        /// <summary>
        /// Parses the request from the stream
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

            m_Responses = new List<ActionResponseWithOptionalData>();

            int Length = DataReader.ReadLength();

            for(int iIndex = 0; iIndex < Length; iIndex++)
            {
                ActionResponseWithOptionalData NewData = new ActionResponseWithOptionalData();
                NewData.Parse(dataStream);

                m_Responses.Add(NewData);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the request
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

            DataWriter.WriteLength(m_Responses.Count);

            foreach(ActionResponseWithOptionalData CurrentData in m_Responses)
            {
                DataWriter.Write(CurrentData.Data);
            }

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the list of Responses
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public List<ActionResponseWithOptionalData> Responses
        {
            get
            {
                return m_Responses;
            }
            set
            {
                if (value != null)
                {
                    m_Responses = value;
                }
                else
                {
                    throw new ArgumentNullException("The Responses may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private List<ActionResponseWithOptionalData> m_Responses;

        #endregion
    }

    /// <summary>
    /// Action Response Next Block Data
    /// </summary>
    public class ActionResponseNextBlock : ActionResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ActionResponseNextBlock()
        {
            m_BlockNumber = 0;
        }

        /// <summary>
        /// Parses the Action Request from the stream
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
        /// Generates the Raw Data for the request
        /// </summary>
        /// <returns>The Memory Stream containing the data</returns>
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

        #region Public Propetries

        /// <summary>
        /// Gets or sets the Block number for the action
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
}
