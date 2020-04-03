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
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// The xDLMS APDU tags
    /// </summary>
    public enum xDLMSTags : byte
    {
        /// <summary>Invalid or no APDU specified</summary>
        [EnumDescription("None")]
        None = 0,

        // Standardized APDU's - No ciphering

        /// <summary>Initiate Request</summary>
        [EnumDescription("Initiate Request")]
        InitiateRequest = 1,
        /// <summary>Read Request</summary>
        [EnumDescription("Read Request")]
        ReadRequest = 5,
        /// <summary>Write Request</summary>
        [EnumDescription("Write Request")]
        WriteRequest = 6,
        /// <summary>Initiate Response</summary>
        [EnumDescription("Initiate Response")]
        InitiateResponse = 8,
        /// <summary>Read Response</summary>
        [EnumDescription("Read Response")]
        ReadResponse = 12,
        /// <summary>Write Response</summary>
        [EnumDescription("Write Response")]
        WriteResponse = 13,
        /// <summary>Confirmed Service Error</summary>
        [EnumDescription("Confirmed Service Error")]
        ConfirmedServiceError = 14,
        /// <summary>Unconfirmed Write Request</summary>
        [EnumDescription("Unconfirmed Write Request")]
        UnconfirmedWriteRequest = 22,
        /// <summary>Information Report Request</summary>
        [EnumDescription("Information Report Request")]
        InformationReportRequest = 24,

        // Standardized APDU's - Global ciphering
        /// <summary>Initiate Request (Global Cipher)</summary>
        [EnumDescription("Initiate Request (Global Cipher)")]
        InitiateRequestGlobalCipher = 33,
        /// <summary>Initiate Response Global Cipher</summary>
        [EnumDescription("Initiate Response (Global Cipher)")]
        InitiateResponseGlobalCipher = 40,
        /// <summary>Confirmed Service Error (Global Cipher)</summary>
        [EnumDescription("Confirmed Service Error (Global Cipher)")]
        ConfirmedServiceErrorGlobalCipher = 46,

        // Standardized APDU's - Dedicated ciphering
        // TODO: Add 65 - 88 when needed
        /// <summary>Confirmed Service Error (Dedicated Cipher)</summary>
        [EnumDescription("Confirmed Service Error (Dedicated Cipher)")]
        ConfirmedServiceErrorDedicatedCiper = 78,

        // ACSE APDU's

        /// <summary>AARQ</summary>
        [EnumDescription("AARQ")]
        AARQ = 96,
        /// <summary>AARE</summary>
        [EnumDescription("AARE")]
        AARE = 97,
        /// <summary>RLRQ</summary>
        [EnumDescription("RLRQ")]
        RLRQ = 98,
        /// <summary>RLRE</summary>
        [EnumDescription("RLRE")]
        RLRE = 99,

        // LN APDU's - No ciphering

        /// <summary>LN Get Request</summary>
        [EnumDescription("Get Request")]
        GetRequest = 192,
        /// <summary>LN Set Request</summary>
        [EnumDescription("Set Request")]
        SetRequest = 193,
        /// <summary>LN Event Notification Request</summary>
        [EnumDescription("Event Notification Request")]
        EventNotificationRequest = 194,
        /// <summary>LN Action Request</summary>
        [EnumDescription("Action Request")]
        ActionRequest = 195,
        /// <summary>LN Get Response</summary>
        [EnumDescription("Get Response")]
        GetResponse = 196,
        /// <summary>LN Set Response</summary>
        [EnumDescription("Set Response")]
        SetResponse = 197,
        /// <summary>LN Action Response</summary>
        [EnumDescription("Action Response")]
        ActionResponse = 199,

        // LN APDU's - Global ciphering
        /// <summary>Get Request (Global Cipher)</summary>
        [EnumDescription("Get Request (Global Cipher)")]
        GetRequestGlobalCipher = 200,
        /// <summary>Set Request (Global Cipher)</summary>
        [EnumDescription("Set Request (Global Cipher)")]
        SetRequestGlobalCipher = 201,
        /// <summary>Action Request (Global Cipher)</summary>
        [EnumDescription("Action Request (Global Cipher)")]
        ActionRequestGlobalCipher = 203,
        /// <summary>Get Response (Global Cipher)</summary>
        [EnumDescription("Get Response (Global Cipher)")]
        GetResponseGlobalCipher = 204,
        /// <summary>Set Response (Global Cipher)</summary>
        [EnumDescription("Set Response (Global Cipher)")]
        SetResponseGlobalCipher = 205,
        /// <summary>Action Response (Global Cipher)</summary>
        [EnumDescription("Action Response (Global Cipher)")]
        ActionResponseGlobalCipher = 207,

        // LN APDU's - Dedicated ciphering
        /// <summary>Get Request (Dedicated Cipher)</summary>
        [EnumDescription("Get Request (Dedicated Cipher)")]
        GetRequestDedicatedCipher = 208,
        /// <summary>Set Request (Dedicated Cipher)</summary>
        [EnumDescription("Set Request (Dedicated Cipher)")]
        SetRequestDedicatedCipher = 209,
        /// <summary>Action Request (Dedicated Cipher)</summary>
        [EnumDescription("Action Request (Dedicated Cipher)")]
        ActionRequestDedicatedCipher = 211,
        /// <summary>Get Response (Dedicated Cipher)</summary>
        [EnumDescription("Get Response (Dedicated Cipher)")]
        GetResponseDedicatedCipher = 212,
        /// <summary>Set Response (Dedicated Cipher)</summary>
        [EnumDescription("Set Response (Dedicated Cipher)")]
        SetResponseDedicatedCipher = 213,
        /// <summary>Action Response (Dedicated Cipher)</summary>
        [EnumDescription("Action Response (Dedicated Cipher)")]
        ActionResponseDedicatedCipher = 215,

        // Exception Response
        /// <summary>Exception Response</summary>
        [EnumDescription("Exception Response")]
        ExceptionResponse = 216,
    }

    /// <summary>
    /// The data types used in COSEM
    /// </summary>
    public enum COSEMDataTypes : byte
    {
        /// <summary>The value is null</summary>
        [EnumDescription("Null")]
        NullData = 0,
        /// <summary>The value is an array</summary>
        [EnumDescription("Array")]
        Array = 1,
        /// <summary>The value is a structure</summary>
        [EnumDescription("Structure")]
        Structure = 2,
        /// <summary>The value is a bool</summary>
        [EnumDescription("Boolean")]
        Boolean = 3,
        /// <summary>The value is a sequence of bool values</summary>
        [EnumDescription("Bit String")]
        BitString = 4,
        /// <summary>32-bit integer (int)</summary>
        [EnumDescription("Double Long")]
        DoubleLong = 5,
        /// <summary>Unsigned 32-bit integer (uint)</summary>
        [EnumDescription("Double Long Unsigned")]
        DoubleLongUnsigned = 6,
        /// <summary>32-bit floating point (float)</summary>
        [EnumDescription("Floating Point")]
        FloatingPoint = 7,
        /// <summary>Sequence of octets (byte[])</summary>
        [EnumDescription("Octet String")]
        OctetString = 9,
        /// <summary>Sequence of ASCII characters</summary>
        [EnumDescription("Visible String")]
        VisibleString = 10,
        /// <summary>Sequence of UTF8 characters</summary>
        [EnumDescription("UTF-8 String")]
        UTF8String = 12,
        /// <summary>Binary Coded Decimal</summary>
        [EnumDescription("BCD")]
        BCD = 13,
        /// <summary>8-bit integer (sbyte)</summary>
        [EnumDescription("Integer")]
        Integer = 15,
        /// <summary>16-bit integer (short)</summary>
        [EnumDescription("Long")]
        Long = 16,
        /// <summary>Unsigned 8-bit integer (byte)</summary>
        [EnumDescription("Unsigned")]
        Unsigned = 17,
        /// <summary>Unsigned 16-bit integer (ushort)</summary>
        [EnumDescription("Long Unsigned")]
        LongUnsigned = 18,
        /// <summary>Compact array</summary>
        [EnumDescription("Compact Array")]
        CompactArray = 19,
        /// <summary>64-bit integer (long)</summary>
        [EnumDescription("Long 64")]
        Long64 = 20,
        /// <summary>Unsigned 64-bit integer (ulong)</summary>
        [EnumDescription("Long 64 Unsigned")]
        Long64Unsigned = 21,
        /// <summary>An enumerated value</summary>
        [EnumDescription("Enum")]
        Enum = 22,
        /// <summary>32-bit float (float)</summary>
        [EnumDescription("Float 32")]
        Float32 = 23,
        /// <summary>64-bit float (double)</summary>
        [EnumDescription("Float 64")]
        Float64 = 24,
        /// <summary>Date and Time</summary>
        [EnumDescription("Date-Time")]
        DateTime = 25,
        /// <summary>Date</summary>
        [EnumDescription("Date")]
        Date = 26,
        /// <summary>Time</summary>
        [EnumDescription("Time")]
        Time = 27,
        /// <summary>Don't Care</summary>
        [EnumDescription("Don't Care")]
        DontCare = 255,
    }

    /// <summary>
    /// Service Class
    /// </summary>
    public enum ServiceClasses : byte
    {
        /// <summary>Unconfirmed</summary>
        [EnumDescription("Unconfirmed")]
        Unconfirmed = 0x00,
        /// <summary>Confirmed</summary>
        [EnumDescription("Confirmed")]
        Confirmed = 0x40,
    }

    /// <summary>
    /// Priority
    /// </summary>
    public enum Priorities : byte
    {
        /// <summary>Normal</summary>
        Normal = 0x00,
        /// <summary>High</summary>
        High = 0x80,
    }

    /// <summary>
    /// The choice options for the get data result
    /// </summary>
    public enum GetDataResultChoices : byte
    {
        /// <summary>Contains a data object</summary>
        Data = 0,
        /// <summary>Contains a data access result</summary>
        DataAccessResult = 1,
    }

    /// <summary>
    /// Data Access Results
    /// </summary>
    public enum DataAccessResults : byte
    {
        /// <summary>Success</summary>
        [EnumDescription("Success")]
        Success = 0,
        /// <summary>Hardware Fault</summary>
        [EnumDescription("Hardware Fault")]
        HardwareFault = 1,
        /// <summary>Temporary Failure</summary>
        [EnumDescription("Temporary Failure")]
        TemporaryFailure = 2,
        /// <summary>Read Write Denied</summary>
        [EnumDescription("Read/Write Denied")]
        ReadWriteDenied = 3,
        /// <summary>Object Undefined</summary>
        [EnumDescription("Object Undefined")]
        ObjectUndefined = 4,
        /// <summary>Object Class Inconsistent</summary>
        [EnumDescription("Object Class Inconsistent")]
        ObjectClassInconsistent = 9,
        /// <summary>Object Unavailable</summary>
        [EnumDescription("Object Unavailable")]
        ObjectUnavailable = 11,
        /// <summary>Type Unmatched</summary>
        [EnumDescription("Type Unmatched")]
        TypeUnmatched = 12,
        /// <summary>Scope of Access Violated</summary>
        [EnumDescription("Scope Of Access Violated")]
        ScopeOfAccessViolated = 13,
        /// <summary>Data Block Unavailable</summary>
        [EnumDescription("Data Block Unavailable")]
        DataBlockUnavailable = 14,
        /// <summary>Long Get Aborted</summary>
        [EnumDescription("Long Get Aborted")]
        LongGetAborted = 15,
        /// <summary>No Long Get in Progress</summary>
        [EnumDescription("No Long Get in Progress")]
        NoLongGetInProgress = 16,
        /// <summary>Long Set Aborted</summary>
        [EnumDescription("Long Set Aborted")]
        LongSetAborted = 17,
        /// <summary>No Long Set in Progress</summary>
        [EnumDescription("No Long Set in Progress")]
        NoLongSetInProgress = 18,
        /// <summary>Data Block Number Invalid</summary>
        [EnumDescription("Data Block Number Invalid")]
        DataBlockNumberInvalid = 19,
        /// <summary>Other</summary>
        [EnumDescription("Other")]
        Other = 250,
    }

    /// <summary>
    /// Action Result Codes
    /// </summary>
    public enum ActionResults : byte
    {
        /// <summary>Success</summary>
        [EnumDescription("Success")]
        Success = 0,
        /// <summary>Hardware Fault</summary>
        [EnumDescription("Hardware Fault")]
        HardwareFault = 1,
        /// <summary>Temporary Failure</summary>
        [EnumDescription("Temporary Failure")]
        TemporaryFailure = 2,
        /// <summary>Read Write Denied</summary>
        [EnumDescription("Read/Write Denied")]
        ReadWriteDenied = 3,
        /// <summary>Object Undefined</summary>
        [EnumDescription("Object Undefined")]
        ObjectUndefined = 4,
        /// <summary>Object Class Inconsistent</summary>
        [EnumDescription("Object Class Inconsistent")]
        ObjectClassInconsistent = 9,
        /// <summary>Object Unavailable</summary>
        [EnumDescription("Object Unavailable")]
        ObjectUnavailable = 11,
        /// <summary>Type Unmatched</summary>
        [EnumDescription("Type Unmatched")]
        TypeUnmatched = 12,
        /// <summary>Scope of Access Violated</summary>
        [EnumDescription("Scope of Access Violated")]
        ScopeOfAccessViolated = 13,
        /// <summary>Data Block Unavailable</summary>
        [EnumDescription("Data Block Unavailable")]
        DataBlockUnavailable = 14,
        /// <summary>Long Get Aborted</summary>
        [EnumDescription("Long Get Aborted")]
        LongGetAborted = 15,
        /// <summary>No Long Get in Progress</summary>
        [EnumDescription("No Long Get In Progress")]
        NoLongGetInProgress = 16,
        /// <summary>Other</summary>
        [EnumDescription("Other")]
        Other = 250,
    }

    /// <summary>
    /// Conformance Bit String Definition
    /// </summary>
    [Flags]
    public enum DLMSConformanceFlags
    {
        /// <summary>Reserved</summary>
        ReservedZero = 0x800000,
        /// <summary>Reserved</summary>
        ReservedOne = 0x400000,
        /// <summary>Reserved</summary>
        ReservedTwo = 0x200000,
        /// <summary>Read Available</summary>
        Read = 0x100000,
        /// <summary>Write Available</summary>
        Write = 0x080000,
        /// <summary>Unconfirmed Write Available</summary>
        UnconfirmedWrite = 0x040000,
        /// <summary>Reserved</summary>
        ReservedSix = 0x020000,
        /// <summary>Reserved</summary>
        ReservedSeven = 0x010000,
        /// <summary>Attrubute 0 with set</summary>
        AttributeZeroSupportedWithSet = 0x008000,
        /// <summary>Priority Management</summary>
        PriorityManagementSupported = 0x004000,
        /// <summary>Attribute 0 with get</summary>
        AttributeZeroSupportedWithGet = 0x002000,
        /// <summary>Block Transfer with Get or Read</summary>
        BlockTransferWithGetOrRead = 0x001000,
        /// <summary>Block Tranfer with Set or Write</summary>
        BlockTransferWithSetOrWrite = 0x000800,
        /// <summary>Block Tranfer with Action</summary>
        BlockTransferWithAction = 0x000400,
        /// <summary>Multiple References</summary>
        MultipleReferences = 0x000200,
        /// <summary>Information Report</summary>
        InformationReport = 0x000100,
        /// <summary>Reserved</summary>
        ReservedSixteen = 0x000080,
        /// <summary>Reserved</summary>
        ReservedSeventeen = 0x000040,
        /// <summary>Parameterized Access</summary>
        ParameterizedAccess = 0x000020,
        /// <summary>Get</summary>
        Get = 0x000010,
        /// <summary>Set</summary>
        Set = 0x000008,
        /// <summary>Selective Access</summary>
        SelectiveAccess = 0x000004,
        /// <summary>Event Notification</summary>
        EventNotification = 0x000002,
        /// <summary>Action</summary>
        Action = 0x000001,

        /// <summary>The most commonly used flags for the client side conformance</summary>
        Default = PriorityManagementSupported | AttributeZeroSupportedWithGet | BlockTransferWithGetOrRead | BlockTransferWithSetOrWrite
            | BlockTransferWithAction | MultipleReferences | Get | Set | SelectiveAccess | Action,
    }

    /// <summary>
    /// xDLMS APDU object
    /// </summary>
    public class xDLMSAPDU
    {
        #region Public Methods

        /// <summary>
        /// Creates a new instance of the APDU object based on the tag
        /// </summary>
        /// <param name="tag">The tag of the APDU to create</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public static xDLMSAPDU Create(xDLMSTags tag)
        {
            xDLMSAPDU NewAPDU = null;

            switch(tag)
            {
                case xDLMSTags.AARQ:
                {
                    NewAPDU = new AARQAPDU();
                    break;
                }
                case xDLMSTags.AARE:
                {
                    NewAPDU = new AAREAPDU();
                    break;
                }
                case xDLMSTags.RLRQ:
                {
                    NewAPDU = new RLRQAPDU();
                    break;
                }
                case xDLMSTags.RLRE:
                {
                    NewAPDU = new RLREAPDU();
                    break;
                }
                case xDLMSTags.InitiateRequest:
                {
                    NewAPDU = new InitiateRequestAPDU();
                    break;
                }
                case xDLMSTags.InitiateResponse:
                {
                    NewAPDU = new InitiateResponseAPDU();
                    break;
                }
                case xDLMSTags.GetRequest:
                {
                    NewAPDU = new GetRequestAPDU();
                    break;
                }
                case xDLMSTags.GetResponse:
                {
                    NewAPDU = new GetResponseAPDU();
                    break;
                }
                case xDLMSTags.SetRequest:
                {
                    NewAPDU = new SetRequestAPDU();
                    break;
                }
                case xDLMSTags.SetResponse:
                {
                    NewAPDU = new SetResponseAPDU();
                    break;
                }
                case xDLMSTags.ActionRequest:
                {
                    NewAPDU = new ActionRequestAPDU();
                    break;
                }
                case xDLMSTags.ActionResponse:
                {
                    NewAPDU = new ActionResponseAPDU();
                    break;
                }
                case xDLMSTags.ConfirmedServiceError:
                {
                    NewAPDU = new ConfirmedServiceErrorAPDU();
                    break;
                }
                case xDLMSTags.ExceptionResponse:
                {
                    NewAPDU = new ExceptionResponseAPDU();
                    break;
                }
                case xDLMSTags.InitiateRequestGlobalCipher:
                case xDLMSTags.InitiateResponseGlobalCipher:
                case xDLMSTags.ConfirmedServiceErrorGlobalCipher:
                case xDLMSTags.GetRequestDedicatedCipher:
                case xDLMSTags.GetRequestGlobalCipher:
                case xDLMSTags.GetResponseDedicatedCipher:
                case xDLMSTags.GetResponseGlobalCipher:
                case xDLMSTags.SetRequestDedicatedCipher:
                case xDLMSTags.SetRequestGlobalCipher:
                case xDLMSTags.SetResponseDedicatedCipher:
                case xDLMSTags.SetResponseGlobalCipher:
                case xDLMSTags.ActionRequestDedicatedCipher:
                case xDLMSTags.ActionRequestGlobalCipher:
                case xDLMSTags.ActionResponseDedicatedCipher:
                case xDLMSTags.ActionResponseGlobalCipher:
                {
                    NewAPDU = new CipheredAPDU(tag);
                    break;
                }
            }

            return NewAPDU;
        }

        /// <summary>
        /// Parses the APDU from the specified stream
        /// </summary>
        /// <param name="apduStream"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public virtual void Parse(Stream apduStream)
        {
            byte TagByte = (byte)apduStream.ReadByte();

            if (TagByte != (byte)m_Tag)
            {
                throw new InvalidOperationException("Could not parse the xDLMS APDU. The Expected tag does not match the parsed tag.");
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected xDLMSAPDU()
        {
            m_Tag = xDLMSTags.None;
        }

        /// <summary>
        /// Updates the data field from the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        protected virtual MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = new MemoryStream();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            // Write the tag
            DataWriter.Write((byte)m_Tag);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the tag of the APDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public xDLMSTags Tag
        {
            get
            {
                return m_Tag;
            }
        }

        /// <summary>
        /// Gets the data for the message.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] Data
        {
            get
            {
                MemoryStream RawData = GenerateRawData();

                return RawData.ToArray();
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// The APDU tag
        /// </summary>
        protected xDLMSTags m_Tag;

        #endregion
    }

    /// <summary>
    /// APDU for the initiate request message
    /// </summary>
    public class InitiateRequestAPDU : xDLMSAPDU
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public InitiateRequestAPDU()
        {
            m_Tag = xDLMSTags.InitiateRequest;

            m_DedicatedKey = null;
            m_ResponseAllowed = true;
            m_ProposedQualityOfService = null;
            m_ProposedDLMSVersion = 6;
            m_ProposedConformance = new DLMSConformance();
            m_ClientMaxReceivePDUSize = UInt16.MaxValue;
        }

        /// <summary>
        /// Parses the APDU Data from the specified stream
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

            // The Dedicated Key is optional so it has a usage flag that needs to be read first
            if (DataReader.ReadUsageFlag() == true)
            {
                m_DedicatedKey = DataReader.ReadBytesWithLength();
            }
            else
            {
                // Make sure we set this to null so we don't have leftover data from a previous set or parse
                m_DedicatedKey = null;
            }

            // The Response Allowed value has a default value so we need to check the usage flag
            if (DataReader.ReadUsageFlag() == true)
            {
                m_ResponseAllowed = DataReader.ReadBoolean();
            }
            else
            {
                // Use the default value (true)
                m_ResponseAllowed = true;
            }

            // Proposed Quality of Service is an optional value
            if (DataReader.ReadUsageFlag() == true)
            {
                m_ProposedQualityOfService = DataReader.ReadSByte();
            }
            else
            {
                m_ProposedQualityOfService = null;
            }

            m_ProposedDLMSVersion = DataReader.ReadByte();
            m_ProposedConformance = DLMSConformance.Parse(DataReader);
            m_ClientMaxReceivePDUSize = DataReader.ReadUInt16();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the APDU
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

            // The dedicated key is optional so set the usage flag and the key
            if (m_DedicatedKey == null)
            {
                // Not Used
                DataWriter.WriteUsageFlag(false);
            }
            else
            {
                // Used
                DataWriter.WriteUsageFlag(true);
                DataWriter.WriteBytesWithLength(m_DedicatedKey);
            }

            // The Response Allowed field has a default value (true) so a usage flag is required
            if (m_ResponseAllowed == true)
            {
                // Default value used
                DataWriter.WriteUsageFlag(false);
            }
            else
            {
                // Value is specified
                DataWriter.WriteUsageFlag(true);
                DataWriter.Write(m_ResponseAllowed);
            }

            // The Proposed Quality of Service field is optional so set the usage flag and value
            if (m_ProposedQualityOfService == null)
            {
                DataWriter.WriteUsageFlag(false);
            }
            else
            {
                DataWriter.WriteUsageFlag(true);
                DataWriter.Write(m_ProposedQualityOfService.Value);
            }

            DataWriter.Write(m_ProposedDLMSVersion);
            DataWriter.Write(m_ProposedConformance.Data);
            DataWriter.Write(m_ClientMaxReceivePDUSize);

            return DataStream;
        }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the dedicated key (optional)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] DedicatedKey
        {
            get
            {
                return m_DedicatedKey;
            }
            set
            {
                m_DedicatedKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the Response Allowed flag
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool ResponseAllowed
        {
            get
            {
                return m_ResponseAllowed;
            }
            set
            {
                m_ResponseAllowed = value;
            }
        }

        /// <summary>
        /// Gets or sets the Proposed Quality of Service value (optional)
        /// </summary>
        public sbyte? ProposedQualityOfService
        {
            get
            {
                return m_ProposedQualityOfService;
            }
            set
            {
                m_ProposedQualityOfService = value;
            }
        }

        /// <summary>
        /// Gets or sets the proposed DLMS Version number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte ProposedDLMSVersion
        {
            get
            {
                return m_ProposedDLMSVersion;
            }
            set
            {
                m_ProposedDLMSVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the Conformance
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DLMSConformance Conformance
        {
            get
            {
                return m_ProposedConformance;
            }
            set
            {
                if (value != null)
                {
                    m_ProposedConformance = value;
                }
                else
                {
                    throw new ArgumentNullException("The Conformance value may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum size of a received APDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort ClientMaxReceivePDUSize
        {
            get
            {
                return m_ClientMaxReceivePDUSize;
            }
            set
            {
                m_ClientMaxReceivePDUSize = value;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_DedicatedKey;
        private bool m_ResponseAllowed;
        private sbyte? m_ProposedQualityOfService;
        private byte m_ProposedDLMSVersion;
        private DLMSConformance m_ProposedConformance;
        private ushort m_ClientMaxReceivePDUSize;

        #endregion
    }

    /// <summary>
    /// The Initiate Response APDU
    /// </summary>
    public class InitiateResponseAPDU : xDLMSAPDU
    {
        #region Constants

        /// <summary>VAA Name for LN referencing</summary>
        public const ushort LN_VAA_NAME = 0x0007;
        /// <summary>VAA Name for SN referencing</summary>
        public const ushort SN_VAA_NAME = 0xFA00;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public InitiateResponseAPDU()
        {
            m_Tag = xDLMSTags.InitiateResponse;

            m_NegotiatedQualityOfService = null;
            m_NegotiatedDLMSVersion = 6;
            m_NegotiatedConformance = new DLMSConformance();
            m_ServerMaxReceivePDUSize = UInt16.MaxValue;
            m_VAAName = 0x0007;
        }

        /// <summary>
        /// Parses the Initiate Response from the stream
        /// </summary>
        /// <param name="apduStream">The stream to parse the data from.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Parse(Stream apduStream)
        {
            base.Parse(apduStream);
            DLMSBinaryReader DataReader = new DLMSBinaryReader(apduStream);

            // The negotiated quality of service is optional so we need to read the usage flag
            if (DataReader.ReadUsageFlag() == true)
            {
                m_NegotiatedQualityOfService = DataReader.ReadSByte();
            }
            else
            {
                // Field is not present so set it to null
                m_NegotiatedQualityOfService = null;
            }

            m_NegotiatedDLMSVersion = DataReader.ReadByte();
            m_NegotiatedConformance = DLMSConformance.Parse(DataReader);
            m_ServerMaxReceivePDUSize = DataReader.ReadUInt16();
            m_VAAName = DataReader.ReadUInt16();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the APDU
        /// </summary>
        /// <returns>The raw data for the APDU</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            if (m_NegotiatedQualityOfService != null)
            {
                // The Negotiated Quality of service is present so we need to write the usage flag and the value
                DataWriter.WriteUsageFlag(true);
                DataWriter.Write(m_NegotiatedQualityOfService.Value);
            }
            else
            {
                // The value is not present
                DataWriter.WriteUsageFlag(false);
            }

            DataWriter.Write(m_NegotiatedDLMSVersion);
            DataWriter.Write(m_NegotiatedConformance.Data);
            DataWriter.Write(m_ServerMaxReceivePDUSize);
            DataWriter.Write(m_VAAName);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Negotiated Quality of Service
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public sbyte? NegotiatedQualityOfService
        {
            get
            {
                return m_NegotiatedQualityOfService;
            }
            set
            {
                m_NegotiatedQualityOfService = value;
            }
        }

        /// <summary>
        /// Gets or sets the negotiated DLMS version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte NegotiatedDLMSVersion
        {
            get
            {
                return m_NegotiatedDLMSVersion;
            }
            set
            {
                m_NegotiatedDLMSVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the negotiated conformance
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DLMSConformance NegotiatedConformance
        {
            get
            {
                return m_NegotiatedConformance;
            }
            set
            {
                if (value != null)
                {
                    m_NegotiatedConformance = value;
                }
                else
                {
                    throw new ArgumentNullException("The Conformance value may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the server's maximum receive PDU Size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort ServerMaxReceivePDUSize
        {
            get
            {
                return m_ServerMaxReceivePDUSize;
            }
            set
            {
                m_ServerMaxReceivePDUSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the VAA Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort VAAName
        {
            get
            {
                return m_VAAName;
            }
            set
            {
                m_VAAName = value;
            }
        }

        #endregion

        #region Member Variables

        private sbyte? m_NegotiatedQualityOfService;
        private byte m_NegotiatedDLMSVersion;
        private DLMSConformance m_NegotiatedConformance;
        private ushort m_ServerMaxReceivePDUSize;
        private ushort m_VAAName;

        #endregion
    }

    /// <summary>
    /// Confirmed Service Error APDU
    /// </summary>
    public class ConfirmedServiceErrorAPDU : xDLMSAPDU
    {
        #region Definitions

        /// <summary>
        /// Service Error choices
        /// </summary>
        public enum ServiceErrorChoices : byte
        {
            /// <summary>Initiate Error</summary>
            [EnumDescription("Initiate Error")]
            InitiateError = 1,
            /// <summary>Get Status</summary>
            [EnumDescription("Get Status")]
            GetStatus = 2,
            /// <summary>Get Name List</summary>
            [EnumDescription("Get Name List")]
            GetNameList = 3,
            /// <summary>Get Variable Attribute</summary>
            [EnumDescription("Get Variable Attribute")]
            GetVariableAttribute = 4,
            /// <summary>Read</summary>
            [EnumDescription("Read")]
            Read = 5,
            /// <summary>Write</summary>
            [EnumDescription("Write")]
            Write = 6,
            /// <summary>Get Data Set Attribute</summary>
            [EnumDescription("Get Dataset Attribute")]
            GetDataSetAttribute = 7,
            /// <summary>Get TIA Attribute</summary>
            [EnumDescription("Get TI Attribute")]
            GetTIAAttribute = 8,
            /// <summary>Change Scope</summary>
            [EnumDescription("Change Scope")]
            ChangeScope = 9,
            /// <summary>Start</summary>
            [EnumDescription("Start")]
            Start = 10,
            /// <summary>Stop</summary>
            [EnumDescription("Stop")]
            Stop = 11,
            /// <summary></summary>
            [EnumDescription("Resume")]
            Resume = 12,
            /// <summary>Resume</summary>
            [EnumDescription("Make Usable")]
            MakeUsable = 13,
            /// <summary>Initiate Load</summary>
            [EnumDescription("Initiate Load")]
            InitiateLoad = 14,
            /// <summary>Load Segment</summary>
            [EnumDescription("Load Segment")]
            LoadSegment = 15,
            /// <summary>Terminate Load</summary>
            [EnumDescription("Terminate Load")]
            TerminiateLoad = 16,
            /// <summary>Initiate Upload</summary>
            [EnumDescription("Initiate Upload")]
            InitiateUpload = 17,
            /// <summary>Upload Segment</summary>
            [EnumDescription("Upload Segment")]
            UploadSegment = 18,
            /// <summary>Terminate Upload</summary>
            [EnumDescription("Terminate Upload")]
            TerminateUpload = 19,
        }

        /// <summary>
        /// The error types
        /// </summary>
        public enum ServiceErrors : ushort
        {
            // In the definition these errors are broken out into separate 1 byte enums with a choice
            // We can simplify things a bit by combining them into one 2 byte enum

            // Application Reference (Choice 0)
            /// <summary>Application Reference - Other</summary>
            [EnumDescription("Application Reference: Other")]
            ApplicationReferenceOther = 0x0000,
            /// <summary>Application Reference - Time Elapsed</summary>
            [EnumDescription("Application Reference: Time Elapsed")]
            ApplicationReferenceTimeElapsed = 0x0001,
            /// <summary>Application Reference - Application Unreachable</summary>
            [EnumDescription("Application Reference: Application Unreachable")]
            ApplicationReferenceApplicationUnreachable = 0x0002,
            /// <summary>Application Reference - Application Reference Invalid</summary>
            [EnumDescription("Application Reference: Reference Invalid")]
            ApplicationReferenceApplicationReferenceInvalid = 0x0003,
            /// <summary>Application Reference - Application Context Unsupported</summary>
            [EnumDescription("Application Reference: Context Unsupported")]
            ApplicationReferenceApplicationContextUnsupported = 0x0004,
            /// <summary>Application Reference - Provider Communication Error</summary>
            [EnumDescription("Application Reference: Communication Error")]
            ApplicationReferenceProviderCommunicationError = 0x0005,
            /// <summary>Application Reference - Deciphering Error</summary>
            [EnumDescription("Application Reference: Deciphering Error")]
            ApplicationreferenceDecipheringError = 0x0006,

            // Hardware Resource (Choice 1)
            /// <summary>Hardware Resource - Other</summary>
            [EnumDescription("Hardware Resource: Other")]
            HardwareResourceOther = 0x0100,
            /// <summary>Hardware Resource - Memory Unavailable</summary>
            [EnumDescription("Hardware Resource: Memory Unavailable")]
            HardwareResourceMemoryUnavailable = 0x0101,
            /// <summary>Hardware Resource - Processor Resource Unavailable</summary>
            [EnumDescription("Hardware Resource: Resource Unavailable")]
            HardwareResourceProcessorResourceUnavailable = 0x0102,
            /// <summary>Hardware Resource - Mass Storage Unavailable</summary>
            [EnumDescription("Hardware Resource: Mass Storage Unavailable")]
            HardwareResourceMassStorageUnavailable = 0x0103,
            /// <summary>Hardware Resource - Other Resource Unavailable</summary>
            [EnumDescription("Hardware Resource: Other Resource Unavailable")]
            HardwareResourceOtherResourceUnavailable = 0x0104,

            // VDE State Error (Choice 2)
            /// <summary>VDE State Error - Other</summary>
            [EnumDescription("VDE State Error: Other")]
            VDEStateErrorOther = 0x0200,
            /// <summary>VDE State Error - No DLMS Context</summary>
            [EnumDescription("VDE State Error: No DLMS Context")]
            VDEStateErrorNoDLMSContext = 0x0201,
            /// <summary>VDE State Error - Loading Data Set</summary>
            [EnumDescription("VDE State Error: Loading Dataset")]
            VDEStateErrorLoadingDataSet = 0x0202,
            /// <summary>VDE State Error - Status - No Change</summary>
            [EnumDescription("VDE State Error: Status - No Change")]
            VDEStateErrorStatusNoChange = 0x0203,
            /// <summary>VDE State Error - Status - Inoperable</summary>
            [EnumDescription("VDE State Error: Status - Inoperable")]
            VDEStateErrorStatusInoperable = 0x0204,

            // Service (Choice 3)
            /// <summary>Service - Other</summary>
            [EnumDescription("Service: Other")]
            ServiceOther = 0x0300,
            /// <summary>Service - PDU Size</summary>
            [EnumDescription("Service: PDU Size")]
            ServicePDUSize = 0x0301,
            /// <summary>Service - Service Unsupported</summary>
            [EnumDescription("Service: Service Unsupported")]
            ServiceServiceUnsupported = 0x0302,

            // Definition (Choice 4)
            /// <summary>Definition - Other</summary>
            [EnumDescription("Definition: Other")]
            DefinitionOther = 0x0400,
            /// <summary>Definition - Object Undefined</summary>
            [EnumDescription("Definition: Object Undefined")]
            DefinitionObjectUndefined = 0x0401,
            /// <summary>Definition - Object Class Inconsistent</summary>
            [EnumDescription("Definition: Object Class Inconsistent")]
            DefinitionObjectClassInconsistent = 0x0402,
            /// <summary>Definition - Attribute Inconsistent</summary>
            [EnumDescription("Definition: Object Attribute Inconsistent")]
            DefinitionObjectAttributeInconsistent = 0x0403,

            // Access (Choice 5)
            /// <summary>Access - Other</summary>
            [EnumDescription("Access: Other")]
            AccessOther = 0x0500,
            /// <summary>Access - Scope of access violated</summary>
            [EnumDescription("Access: Scope of Access Violated")]
            AccessScopeOfAccessViolated = 0x0501,
            /// <summary>Access - Object Access Violated</summary>
            [EnumDescription("Access: Object Access Violated")]
            AccessObjectAccessViolated = 0x0502,
            /// <summary>Access - Hardware Fault</summary>
            [EnumDescription("Access: Hardware Fault")]
            AccessHardwareFault = 0x0503,
            /// <summary>Access - Object Unavailable</summary>
            [EnumDescription("Access: Object Unavailable")]
            AccessObjectUnavailable = 0x0504,

            // Initiate (Choice 6)
            /// <summary>Initiate - Other</summary>
            [EnumDescription("Initiate: Other")]
            InitiateOther = 0x0600,
            /// <summary>Initiate - DLMS Version Too Low</summary>
            [EnumDescription("Initiate: DLMS Version Too Low")]
            InitiateDLMSVersionTooLow = 0x0601,
            /// <summary>Initiate - Incompatible Conformance</summary>
            [EnumDescription("Initiate: Incompatible Conformance")]
            InitiateIncompatibleConformance = 0x0602,
            /// <summary>Initiate - PDU Size Too Short</summary>
            [EnumDescription("Initiate: PDU Size Too Short")]
            InitiatePDUSizeTooShort = 0x0603,
            /// <summary>Initiate - Refused by the VDE Handler</summary>
            [EnumDescription("Initiate: Refused by the VDE Handler")]
            InitiateRefusedByTheVDEHandler = 0x0604,

            // Load Data Set (Choice 7)
            /// <summary>Load Data Set - Other</summary>
            [EnumDescription("Load Dataset: Other")]
            LoadDataSetOther = 0x0700,
            /// <summary>Load Data Set - Primitive out of sequence</summary>
            [EnumDescription("Load Dataset: Primitive Out of Sequence")]
            LoadDataSetPrimitiveOutOfSequence = 0x0701,
            /// <summary>Load Data Set - Not loadable</summary>
            [EnumDescription("Load Dataset: Not Loadable")]
            LoadDataSetNotLoadable = 0x0702,
            /// <summary>Load Data Set - Size too large</summary>
            [EnumDescription("Load Dataset: Dataset Size Too Large")]
            LoadDataSetDataSetSizeTooLarge = 0x0703,
            /// <summary>Load Data Set - Not Awaited Segment</summary>
            [EnumDescription("Load Dataset: Not Awaited Segment")]
            LoadDataSetNotAwaitedSegment = 0x0704,
            /// <summary>Load Data Set - Interpretation Failure</summary>
            [EnumDescription("Load Dataset: Interpretation Failure")]
            LoadDataSetInterpretationFailure = 0x0705,
            /// <summary>Load Data Set - Storage Failure</summary>
            [EnumDescription("Load Dataset: Storage Failure")]
            LoadDataSetStorageFailure = 0x0706,
            /// <summary>Load Data Set - Data Set Not Ready</summary>
            [EnumDescription("Load Dataset: Dataset Not Ready")]
            LoadDataSetDataSetNotReady = 0x0707,

            // ChangeScope (Choice 8)
            /// <summary></summary>
            [EnumDescription("Change Scope")]
            ChangeScope = 0x0800,

            // Task (Choice 9)
            /// <summary>Task - Other</summary>
            [EnumDescription("Task: Other")]
            TaskOther = 0x0900,
            /// <summary>Task - No Remote Control</summary>
            [EnumDescription("Task: No Remote Control")]
            TaskNoRemoteControl = 0x0901,
            /// <summary>Task - TI Stopped</summary>
            [EnumDescription("Task: TI Stopped")]
            TaskTIStopped = 0x0902,
            /// <summary>Task - TI Running</summary>
            [EnumDescription("Task: TI Running")]
            TaskTIRunning = 0x0903,
            /// <summary>Task - TI Unusable</summary>
            [EnumDescription("Task: TI Unusable")]
            TaskTIUnusable = 0x0904,

            // Other (Choice 10)
            /// <summary>Other </summary>
            [EnumDescription("Other")]
            Other = 0x0A00,
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
        
        public ConfirmedServiceErrorAPDU()
        {
            m_Tag = xDLMSTags.ConfirmedServiceError;

            m_ServiceErrorChoice = ServiceErrorChoices.InitiateError;
            m_ServiceErrors = ServiceErrors.Other;
        }

        /// <summary>
        /// Parses the APDU
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

            // The first byte is the Confirmed Service Error choice
            m_ServiceErrorChoice = DataReader.ReadEnum<ServiceErrorChoices>();

            // The second (Service Error Choice) and third bytes (Service Error Enum) have been combined for simplicity
            m_ServiceErrors = (ServiceErrors)DataReader.ReadUInt16();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the APDU
        /// </summary>
        /// <returns>A stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = base.GenerateRawData();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.WriteEnum<ServiceErrorChoices>(m_ServiceErrorChoice);
            DataWriter.WriteEnum((ushort)m_ServiceErrors);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the service error choice
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ServiceErrorChoices ServiceErrorChoice
        {
            get
            {
                return m_ServiceErrorChoice;
            }
            set
            {
                m_ServiceErrorChoice = value;
            }
        }

        /// <summary>
        /// Gets or sets the service error
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ServiceErrors ServiceError
        {
            get
            {
                return m_ServiceErrors;
            }
            set
            {
                m_ServiceErrors = value;
            }
        }

        #endregion

        #region Member Variables

        private ServiceErrorChoices m_ServiceErrorChoice;
        private ServiceErrors m_ServiceErrors;

        #endregion
    }

    /// <summary>
    /// Event Notification Request APDU
    /// </summary>
    public class EventNotificationRequestAPDU : xDLMSAPDU
    {
        // TODO: Need to complete once Data and Time objects are moved/complete
    }

    /// <summary>
    /// Exception Response APDU
    /// </summary>
    public class ExceptionResponseAPDU : xDLMSAPDU
    {
        #region Definitions

        /// <summary>
        /// State Error Codes
        /// </summary>
        public enum StateErrors : byte
        {
            /// <summary>Service Not Allowed</summary>
            [EnumDescription("Service Not Allowed")]
            ServiceNotAllowed = 1,
            /// <summary>Service Unknown</summary>
            [EnumDescription("Service Unknown")]
            ServiceUnknown = 2,
        }

        /// <summary>
        /// Service Error Codes
        /// </summary>
        public enum ServiceErrors : byte
        {
            /// <summary>Operation Not Possible</summary>
            [EnumDescription("Operation Not Possible")]
            OperationNotPossible = 1,
            /// <summary>Service Not Supported</summary>
            [EnumDescription("Service Not Supported")]
            ServiceNotSupported = 2,
            /// <summary>Other</summary>
            [EnumDescription("Other")]
            Other = 3,
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
        
        public ExceptionResponseAPDU()
        {
            m_Tag = xDLMSTags.ExceptionResponse;

            m_ServiceError = ServiceErrors.Other;
            m_StateError = StateErrors.ServiceUnknown;
        }

        /// <summary>
        /// Parses the APDU from the stream
        /// </summary>
        /// <param name="apduStream"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Parse(Stream apduStream)
        {
            base.Parse(apduStream);
            DLMSBinaryReader DataReader = new DLMSBinaryReader(apduStream);

            // TODO: Figure out if the ASN.1 spec means sequence or choice
            m_StateError = DataReader.ReadEnum<StateErrors>();
            m_ServiceError = DataReader.ReadEnum<ServiceErrors>();

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the APDU
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

            DataWriter.WriteEnum<StateErrors>(m_StateError);
            DataWriter.WriteEnum<ServiceErrors>(m_ServiceError);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the state error
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public StateErrors StateError
        {
            get
            {
                return m_StateError;
            }
            set
            {
                m_StateError = value;
            }
        }

        /// <summary>
        /// Gets or sets the service error
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ServiceErrors ServiceError
        {
            get
            {
                return m_ServiceError;
            }
            set
            {
                m_ServiceError = value;
            }
        }

        #endregion

        #region Member Variables

        private StateErrors m_StateError;
        private ServiceErrors m_ServiceError;

        #endregion
    }

    /// <summary>
    /// Get data result
    /// </summary>
    public class GetDataResult
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public GetDataResult()
        {
            m_DataValue = null;
            m_DataAccessResult = DataAccessResults.Success;
        }

        /// <summary>
        /// Parses the Get Data Result from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_GetDataResultType = DataReader.ReadEnum<GetDataResultChoices>();

            if (m_GetDataResultType == GetDataResultChoices.Data)
            {
                m_DataValue = new COSEMData();
                m_DataValue.Parse(dataStream);

                // Set the Access Result to success even though it's not used
                m_DataAccessResult = DataAccessResults.Success;
            }
            else
            {
                m_DataAccessResult = DataReader.ReadEnum<DataAccessResults>();

                // Set the Value to null since it is not used
                m_DataValue = null;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the data result type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public GetDataResultChoices GetDataResultType
        {
            get
            {
                return m_GetDataResultType;
            }
            set
            {
                m_GetDataResultType = value;

                if (m_GetDataResultType == GetDataResultChoices.Data)
                {
                    m_DataValue = new COSEMData();
                    m_DataAccessResult = DataAccessResults.Success;
                }
                else
                {
                    m_DataValue = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the data value. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMData DataValue
        {
            get
            {
                return m_DataValue;
            }
            set
            {
                if (m_GetDataResultType == GetDataResultChoices.Data)
                {
                    if (value != null)
                    {
                        m_DataValue = value;
                    }
                    else
                    {
                        throw new ArgumentNullException("The Data Value may not be null when the Data Result Type is set to Data.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The Data Value may not be set while the Data Result Type is set to Access Result.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Data Access Result
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DataAccessResults DataAccessResult
        {
            get
            {
                return m_DataAccessResult;
            }
            set
            {
                if (m_GetDataResultType == GetDataResultChoices.DataAccessResult)
                {
                    m_DataAccessResult = value;
                }
                else
                {
                    throw new InvalidOperationException("The Data Access Result may not be set while the Data Result Type is set to Data.");
                }
            }
        }

        /// <summary>
        /// Gets the Raw data for the object
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
                DLMSBinaryWriter DataWriter = new DLMSBinaryWriter();

                // Write the choice first
                DataWriter.WriteEnum<GetDataResultChoices>(m_GetDataResultType);

                if (m_GetDataResultType == GetDataResultChoices.Data)
                {
                    // Write the data
                    DataWriter.Write(m_DataValue.Data);
                }
                else
                {
                    // Write the Access Result
                    DataWriter.WriteEnum<DataAccessResults>(m_DataAccessResult);
                }

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private GetDataResultChoices m_GetDataResultType;
        private COSEMData m_DataValue;
        private DataAccessResults m_DataAccessResult;

        #endregion
    }

    /// <summary>
    /// Data block used by the set request and action request and responses
    /// </summary>
    public class DataBlock
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DataBlock()
        {
            m_LastBlock = false;
            m_BlockNumber = 0;
            m_BlockData = null;
        }

        /// <summary>
        /// Parses the data block from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse the data from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public virtual void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_LastBlock = DataReader.ReadBoolean();
            m_BlockNumber = DataReader.ReadUInt32();
            m_BlockData = DataReader.ReadBytesWithLength();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the Data Block
        /// </summary>
        /// <returns>The Memory Stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        protected virtual MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = new MemoryStream();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_LastBlock);
            DataWriter.Write(m_BlockNumber);
            DataWriter.WriteBytesWithLength(m_BlockData);

            return DataStream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether or not the block is the last block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool LastBlock
        {
            get
            {
                return m_LastBlock;
            }
            set
            {
                m_LastBlock = value;
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

        /// <summary>
        /// Gets or sets the block data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] BlockData
        {
            get
            {
                return m_BlockData;
            }
            set
            {
                m_BlockData = value;
            }
        }

        /// <summary>
        /// Gets the raw data for the Data Block
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

        /// <summary>Whether or not this is the last block</summary>
        protected bool m_LastBlock;
        /// <summary>The current block number</summary>
        protected uint m_BlockNumber;
        /// <summary>The data for the block</summary>
        protected byte[] m_BlockData;

        #endregion
    }

    /// <summary>
    /// Data Block used by the Get Response
    /// </summary>
    public class DataBlockResponse : DataBlock
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DataBlockResponse()
        {
            m_ResponseType = GetDataResultChoices.DataAccessResult;
            m_AccessResult = DataAccessResults.Success;
        }

        /// <summary>
        /// Parses the data block from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_LastBlock = DataReader.ReadBoolean();
            m_BlockNumber = DataReader.ReadUInt32();
            m_ResponseType = DataReader.ReadEnum<GetDataResultChoices>();

            if (m_ResponseType == GetDataResultChoices.Data)
            {
                m_BlockData = DataReader.ReadBytesWithLength();
                m_AccessResult = DataAccessResults.Success;
            }
            else
            {
                m_BlockData = null;
                m_AccessResult = DataReader.ReadEnum<DataAccessResults>();
            }
        }  

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates the raw data for the data block
        /// </summary>
        /// <returns>The stream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        protected override MemoryStream GenerateRawData()
        {
            MemoryStream DataStream = new MemoryStream();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

            DataWriter.Write(m_LastBlock);
            DataWriter.Write(m_BlockNumber);
            DataWriter.WriteEnum<GetDataResultChoices>(m_ResponseType);

            if (m_ResponseType == GetDataResultChoices.Data)
            {
                DataWriter.WriteBytesWithLength(m_BlockData);
            }
            else
            {
                DataWriter.WriteEnum<DataAccessResults>(m_AccessResult);
            }

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
        
        public GetDataResultChoices ResponseType
        {
            get
            {
                return m_ResponseType;
            }
            set
            {
                m_ResponseType = value;

                if (m_ResponseType == GetDataResultChoices.Data)
                {
                    m_AccessResult = DataAccessResults.Success;
                }
                else
                {
                    m_BlockData = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Access Result
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DataAccessResults AccessResult
        {
            get
            {
                return m_AccessResult;
            }
            set
            {
                if (m_ResponseType == GetDataResultChoices.DataAccessResult)
                {
                    m_AccessResult = value;
                }
                else
                {
                    throw new ArgumentException("The Access Result may not be set while the Response type is Data");
                }
            }
        }

        #endregion

        #region Member Variables

        private GetDataResultChoices m_ResponseType;
        private DataAccessResults m_AccessResult;

        #endregion
    }

    /// <summary>
    /// COSEM Attribute Descriptor
    /// </summary>
    public class CosemAttributeDescriptor
    {
        #region Constants

        private const int INSTANCE_ID_LENGTH = 6;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public CosemAttributeDescriptor()
        {
            m_ClassID = 0;
            m_InstanceID = new byte[INSTANCE_ID_LENGTH];
            m_AttributeID = 0;
        }

        /// <summary>
        /// Parses the Cosem Attribute Descriptor from the stream
        /// </summary>
        /// <param name="dataStream"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_ClassID = DataReader.ReadUInt16();
            m_InstanceID = DataReader.ReadBytes(INSTANCE_ID_LENGTH);
            m_AttributeID = DataReader.ReadSByte();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Class ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Instance ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] InstanceID
        {
            get
            {
                return m_InstanceID;
            }
            set
            {
                if (value != null && value.Length == INSTANCE_ID_LENGTH)
                {
                    m_InstanceID = value;
                }
                else
                {
                    throw new ArgumentException("The Instance ID may not be null and must be 6 bytes long");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Attribute ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public sbyte AttributeID
        {
            get
            {
                return m_AttributeID;
            }
            set
            {
                m_AttributeID = value;
            }
        }

        /// <summary>
        /// Gets the Raw Data for the Cosem Attribute Descriptor
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

                DataWriter.Write(m_ClassID);
                DataWriter.Write(m_InstanceID);
                DataWriter.Write(m_AttributeID);

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ClassID;
        private byte[] m_InstanceID;
        private sbyte m_AttributeID;

        #endregion
    }

    /// <summary>
    /// COSEM Method Descriptor
    /// </summary>
    public class CosemMethodDescriptor
    {
        #region Constants

        private const int INSTANCE_ID_LENGTH = 6;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public CosemMethodDescriptor()
        {
            m_ClassID = 0;
            m_InstanceID = new byte[INSTANCE_ID_LENGTH];
            m_MethodID = 0;
        }

        /// <summary>
        /// Parses the Cosem Attribute Descriptor from the stream
        /// </summary>
        /// <param name="dataStream"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_ClassID = DataReader.ReadUInt16();
            m_InstanceID = DataReader.ReadBytes(INSTANCE_ID_LENGTH);
            m_MethodID = DataReader.ReadSByte();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Class ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Instance ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] InstanceID
        {
            get
            {
                return m_InstanceID;
            }
            set
            {
                if (value != null && value.Length == INSTANCE_ID_LENGTH)
                {
                    m_InstanceID = value;
                }
                else
                {
                    throw new ArgumentException("The Instance ID may not be null and must be 6 bytes long");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Method ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public sbyte MethodID
        {
            get
            {
                return m_MethodID;
            }
            set
            {
                m_MethodID = value;
            }
        }

        /// <summary>
        /// Gets the Raw Data for the Cosem Method Descriptor
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

                DataWriter.Write(m_ClassID);
                DataWriter.Write(m_InstanceID);
                DataWriter.Write(m_MethodID);

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ClassID;
        private byte[] m_InstanceID;
        private sbyte m_MethodID;

        #endregion
    }

    /// <summary>
    /// Selective Access Descriptor
    /// </summary>
    public class SelectiveAccessDescriptor
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public SelectiveAccessDescriptor()
        {
            m_AccessParameters = new COSEMData();
        }

        /// <summary>
        /// Parses the Selective Access Descriptor from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse the data from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_AccessSelector = DataReader.ReadByte();
            m_AccessParameters = new COSEMData();
            m_AccessParameters.Parse(dataStream);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the access selector
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte AccessSelector
        {
            get
            {
                return m_AccessSelector;
            }
            set
            {
                m_AccessSelector = value;
            }
        }

        /// <summary>
        /// Gets or sets the Access Parameters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMData AccessParameters
        {
            get
            {
                return m_AccessParameters;
            }
            set
            {
                if (value != null)
                {
                    m_AccessParameters = value;
                }
                else
                {
                    throw new ArgumentNullException("The Access Parameters may not be null");
                }
            }
        }

        /// <summary>
        /// Gets the raw data for the Selective Access Descriptor
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

                DataWriter.Write(m_AccessSelector);
                DataWriter.Write(m_AccessParameters.Data);

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private byte m_AccessSelector;
        private COSEMData m_AccessParameters;

        #endregion
    }

    /// <summary>
    /// Class containing a Cosem Attribute Descriptor and Selective Access Descriptor
    /// </summary>
    public class CosemAttributeDescriptorWithSelection
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public CosemAttributeDescriptorWithSelection()
        {
            m_AttributeDescriptor = new CosemAttributeDescriptor();
            m_AccessSelection = null;
        }

        /// <summary>
        /// Parses the object from the specified stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_AttributeDescriptor = new CosemAttributeDescriptor();
            m_AttributeDescriptor.Parse(dataStream);

            // The Access Selection is optional so we need to check the usage flag
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

        /// <summary>
        /// Gets the raw data for the object
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

                DataWriter.Write(m_AttributeDescriptor.Data);

                // Write the usage flag for the Access Selection
                if (m_AccessSelection != null)
                {
                    DataWriter.WriteUsageFlag(true);
                    DataWriter.Write(m_AccessSelection.Data);
                }
                else
                {
                    DataWriter.WriteUsageFlag(false);
                }

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private CosemAttributeDescriptor m_AttributeDescriptor;
        private SelectiveAccessDescriptor m_AccessSelection;

        #endregion
    }

    /// <summary>
    /// Data object
    /// </summary>
    public class COSEMData: IEquatable<COSEMData>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMData()
        {
            m_DataType = COSEMDataTypes.DontCare;
            m_Value = null;
        }

        /// <summary>
        /// Gets the .Net data type that will be returned for the COSEM Data type
        /// </summary>
        /// <param name="dataType">The COSEM data type to check</param>
        /// <returns>The .Net data type</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public static Type GetExpectedDataType(COSEMDataTypes dataType)
        {
            Type ExpectedType = null;

            switch(dataType)
            {
                case COSEMDataTypes.NullData:
                case COSEMDataTypes.DontCare:
                {
                    ExpectedType = null;
                    break;
                }
                case COSEMDataTypes.Array:
                case COSEMDataTypes.Structure:
                {
                    ExpectedType = typeof(COSEMData[]);
                    break;
                }
                case COSEMDataTypes.Boolean:
                {
                    ExpectedType = typeof(bool);
                    break;
                }
                case COSEMDataTypes.BitString:
                case COSEMDataTypes.OctetString:
                {
                    ExpectedType = typeof(byte[]);
                    break;
                }
                case COSEMDataTypes.DoubleLong:
                {
                    ExpectedType = typeof(int);
                    break;
                }
                case COSEMDataTypes.DoubleLongUnsigned:
                {
                    ExpectedType = typeof(uint);
                    break;
                }
                case COSEMDataTypes.FloatingPoint:
                case COSEMDataTypes.Float32:
                {
                    ExpectedType = typeof(Single);
                    break;
                }
                case COSEMDataTypes.VisibleString:
                {
                    ExpectedType = typeof(string);
                    break;
                }
                case COSEMDataTypes.BCD:
                case COSEMDataTypes.Integer:
                {
                    ExpectedType = typeof(sbyte);
                    break;
                }
                case COSEMDataTypes.Long:
                {
                    ExpectedType = typeof(short);
                    break;
                }
                case COSEMDataTypes.Unsigned:
                case COSEMDataTypes.Enum:
                {
                    ExpectedType = typeof(byte);
                    break;
                }
                case COSEMDataTypes.LongUnsigned:
                {
                    ExpectedType = typeof(ushort);
                    break;
                }
                case COSEMDataTypes.CompactArray:
                {
                    // TODO: Support Compact Array
                    break;
                }
                case COSEMDataTypes.Long64:
                {
                    ExpectedType = typeof(long);
                    break;
                }
                case COSEMDataTypes.Long64Unsigned:
                {
                    ExpectedType = typeof(ulong);
                    break;
                }
                case COSEMDataTypes.Float64:
                {
                    ExpectedType = typeof(double);
                    break;
                }
                case COSEMDataTypes.DateTime:
                {
                    ExpectedType = typeof(COSEMDateTime);
                    break;
                }
                case COSEMDataTypes.Date:
                {
                    ExpectedType = typeof(COSEMDate);
                    break;
                }
                case COSEMDataTypes.Time:
                {
                    ExpectedType = typeof(COSEMTime);
                    break;
                }
            }

            return ExpectedType;
        }

        /// <summary>
        /// Parses the data value from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse the data from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_DataType = DataReader.ReadEnum<COSEMDataTypes>();

            switch(m_DataType)
            {
                case COSEMDataTypes.NullData:
                case COSEMDataTypes.DontCare:
                {
                    // Nothing to read
                    m_Value = null;
                    break;
                }
                case COSEMDataTypes.Array:
                case COSEMDataTypes.Structure:
                {
                    // Arrays and structures work the same way. The only restriction is that Arrays should all be the same type
                    int ItemsToRead = DataReader.ReadLength();
                    COSEMData[] Value;

                    Value = new COSEMData[ItemsToRead];

                    for (int iIndex = 0; iIndex < ItemsToRead; iIndex++ )
                    {
                        Value[iIndex] = new COSEMData();
                        Value[iIndex].Parse(dataStream);
                    }

                    m_Value = Value;

                    break;
                }
                case COSEMDataTypes.Boolean:
                {
                    m_Value = DataReader.ReadBoolean();
                    break;
                }
                case COSEMDataTypes.BitString:
                {
                    m_Value = DataReader.ReadBitString();
                    break;
                }
                case COSEMDataTypes.DoubleLong:
                {
                    m_Value = DataReader.ReadInt32();
                    break;
                }
                case COSEMDataTypes.DoubleLongUnsigned:
                {
                    m_Value = DataReader.ReadUInt32();
                    break;
                }
                case COSEMDataTypes.FloatingPoint:
                case COSEMDataTypes.Float32:
                {
                    m_Value = DataReader.ReadSingle();
                    break;
                }
                case COSEMDataTypes.OctetString:
                {
                    m_Value = DataReader.ReadBytesWithLength();
                    break;
                }
                case COSEMDataTypes.VisibleString:
                {
                    m_Value = DataReader.ReadString();
                    break;
                }
                case COSEMDataTypes.BCD:
                case COSEMDataTypes.Integer:
                {
                    m_Value = DataReader.ReadSByte();
                    break;
                }
                case COSEMDataTypes.Long:
                {
                    m_Value = DataReader.ReadInt16();
                    break;
                }
                case COSEMDataTypes.Unsigned:
                {
                    m_Value = DataReader.ReadByte();
                    break;
                }
                case COSEMDataTypes.LongUnsigned:
                {
                    m_Value = DataReader.ReadUInt16();
                    break;
                }
                case COSEMDataTypes.CompactArray:
                {
                    // TODO: Support Compact Array
                    break;
                }
                case COSEMDataTypes.Long64:
                {
                    m_Value = DataReader.ReadInt64();
                    break;
                }
                case COSEMDataTypes.Long64Unsigned:
                {
                    m_Value = DataReader.ReadUInt64();
                    break;
                }
                case COSEMDataTypes.Enum:
                {
                    m_Value = DataReader.ReadByte();
                    break;
                }
                case COSEMDataTypes.Float64:
                {
                    m_Value = DataReader.ReadDouble();
                    break;
                }
                case COSEMDataTypes.DateTime:
                {
                    m_Value = DataReader.ReadCOSEMDateTime();
                    break;
                }
                case COSEMDataTypes.Date:
                {
                    m_Value = DataReader.ReadCOSEMDate();
                    break;
                }
                case COSEMDataTypes.Time:
                {
                    m_Value = DataReader.ReadCOSEMTime();
                    break;
                }
            }
        }

        /// <summary>
        /// Determines whether or not the COSEM Data values are equal
        /// </summary>
        /// <param name="other">The COSEM Data Value to compare against</param>
        /// <returns>True if the values are equal. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/19/13 RCG 2.85.21 N/A    Created
        
        public bool Equals(COSEMData other)
        {
            bool IsEqual = false;

            if (other != null)
            {
                if (DataType == other.DataType)
                {
                    switch(DataType)
                    {
                        case COSEMDataTypes.NullData:
                        case COSEMDataTypes.DontCare:
                        {
                            // No need to compare the values
                            IsEqual = true;
                            break;
                        }
                        case COSEMDataTypes.Array:
                        case COSEMDataTypes.Structure:
                        {
                            COSEMData[] ThisData = Value as COSEMData[];
                            COSEMData[] OtherData = other.Value as COSEMData[];

                            if (ThisData == null && OtherData == null)
                            {
                                IsEqual = true;
                            }
                            else if (ThisData != null && OtherData != null)
                            {
                                if (ThisData.Length == OtherData.Length)
                                {
                                    IsEqual = true;

                                    for (int iIndex = 0; iIndex < ThisData.Length; iIndex++)
                                    {
                                        if (ThisData[iIndex].Equals(OtherData[iIndex]) == false)
                                        {
                                            IsEqual = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            break;
                        }
                        case COSEMDataTypes.Boolean:
                        {
                            IsEqual = (bool)Value == (bool)other.Value;
                            break;
                        }
                        case COSEMDataTypes.BitString:
                        case COSEMDataTypes.OctetString:
                        {
                            byte[] ThisValue = Value as byte[];
                            byte[] OtherValue = Value as byte[];

                            IsEqual = ThisValue.IsEqual(OtherValue);
                            break;
                        }
                        case COSEMDataTypes.DoubleLong:
                        {
                            IsEqual = (int)Value == (int)other.Value;
                            break;
                        }
                        case COSEMDataTypes.DoubleLongUnsigned:
                        {
                            IsEqual = (uint)Value == (uint)other.Value;
                            break;
                        }
                        case COSEMDataTypes.FloatingPoint:
                        case COSEMDataTypes.Float32:
                        {
                            IsEqual = (Single)Value == (Single)other.Value;
                            break;
                        }
                        case COSEMDataTypes.VisibleString:
                        {
                            IsEqual = ((string)Value).Equals((string)other.Value);
                            break;
                        }
                        case COSEMDataTypes.BCD:
                        case COSEMDataTypes.Integer:
                        {
                            IsEqual = (sbyte)Value == (sbyte)other.Value;
                            break;
                        }
                        case COSEMDataTypes.Long:
                        {
                            IsEqual = (short)Value == (short)other.Value;
                            break;
                        }
                        case COSEMDataTypes.Unsigned:
                        case COSEMDataTypes.Enum:
                        {
                            IsEqual = (byte)Value == (byte)other.Value;
                            break;
                        }
                        case COSEMDataTypes.LongUnsigned:
                        {
                            IsEqual = (ushort)Value == (ushort)other.Value;
                            break;
                        }
                        case COSEMDataTypes.CompactArray:
                        {
                            // TODO: Support Compact Array
                            break;
                        }
                        case COSEMDataTypes.Long64:
                        {
                            IsEqual = (long)Value == (long)other.Value;
                            break;
                        }
                        case COSEMDataTypes.Long64Unsigned:
                        {
                            IsEqual = (ulong)Value == (ulong)other.Value;
                            break;
                        }
                        case COSEMDataTypes.Float64:
                        {
                            IsEqual = (double)Value == (double)other.Value;
                            break;
                        }
                        case COSEMDataTypes.DateTime:
                        {
                            IsEqual = ((COSEMDateTime)Value).Equals((COSEMDateTime)other.Value);
                            break;
                        }
                        case COSEMDataTypes.Date:
                        {
                            IsEqual = ((COSEMDate)Value).Equals((COSEMDate)other.Value);
                            break;
                        }
                        case COSEMDataTypes.Time:
                        {
                            IsEqual = ((COSEMTime)Value).Equals((COSEMTime)other.Value);
                            break;
                        }
                    }
                }
            }

            return IsEqual;
        }

        /// <summary>
        /// Gets the COSEM Data object as a string
        /// </summary>
        /// <returns>The string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/19/13 RCG 2.85.21 N/A    Created

        public override string ToString()
        {
            string strValue = DataType.ToDescription() + " - ";

            switch (DataType)
            {
                case COSEMDataTypes.NullData:
                case COSEMDataTypes.DontCare:
                {
                    // No need to compare the values
                    strValue += "null";
                    break;
                }
                case COSEMDataTypes.Array:
                {
                    COSEMData[] StructureData = Value as COSEMData[];

                    strValue += "[ ";

                    for (int iIndex = 0; iIndex < StructureData.Length; iIndex++)
                    {
                        strValue += StructureData[iIndex].ToString();

                        if (iIndex + 1 < StructureData.Length)
                        {
                            strValue += ", ";
                        }
                    }

                    strValue += " ]";
                    break;
                }
                case COSEMDataTypes.Structure:
                {
                    COSEMData[] StructureData = Value as COSEMData[];

                    strValue += "{ ";

                    for (int iIndex = 0; iIndex < StructureData.Length; iIndex++)
                    {
                        strValue += StructureData[iIndex].ToString();

                        if (iIndex + 1 < StructureData.Length)
                        {
                            strValue += ", ";
                        }
                    }

                    strValue += " }";
                    break;
                }
                case COSEMDataTypes.Boolean:
                {
                    strValue = ((bool)Value).ToString();
                    break;
                }
                case COSEMDataTypes.BitString:
                case COSEMDataTypes.OctetString:
                {
                    byte[] ThisValue = Value as byte[];

                    strValue += ThisValue.ToHexString();
                    break;
                }
                case COSEMDataTypes.DoubleLong:
                {
                    strValue = ((int)Value).ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case COSEMDataTypes.DoubleLongUnsigned:
                {
                    strValue = ((uint)Value).ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case COSEMDataTypes.FloatingPoint:
                case COSEMDataTypes.Float32:
                {
                    strValue = ((Single)Value).ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case COSEMDataTypes.VisibleString:
                {
                    strValue = (string)Value;
                    break;
                }
                case COSEMDataTypes.BCD:
                case COSEMDataTypes.Integer:
                {
                    strValue = ((sbyte)Value).ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case COSEMDataTypes.Long:
                {
                    strValue = ((short)Value).ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case COSEMDataTypes.Unsigned:
                case COSEMDataTypes.Enum:
                {
                    strValue = ((byte)Value).ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case COSEMDataTypes.LongUnsigned:
                {
                    strValue = ((ushort)Value).ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case COSEMDataTypes.CompactArray:
                {
                    // TODO: Support Compact Array
                    break;
                }
                case COSEMDataTypes.Long64:
                {
                    strValue = ((long)Value).ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case COSEMDataTypes.Long64Unsigned:
                {
                    strValue = ((ulong)Value).ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case COSEMDataTypes.Float64:
                {
                    strValue = ((int)Value).ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case COSEMDataTypes.DateTime:
                {
                    strValue = ((COSEMDateTime)Value).ToString();
                    break;
                }
                case COSEMDataTypes.Date:
                {
                    strValue = ((COSEMDate)Value).ToString();
                    break;
                }
                case COSEMDataTypes.Time:
                {
                    strValue = ((COSEMTime)Value).ToString();
                    break;
                }
            }

            return strValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Data Type. Note: Setting this value will clear the data field.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDataTypes DataType
        {
            get
            {
                return m_DataType;
            }
            set
            {
                m_DataType = value;
                m_Value = null;
            }
        }

        /// <summary>
        /// Gets or sets the data value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public object Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                if (value.GetType() == GetExpectedDataType(m_DataType))
                {
                    m_Value = value;
                }
                else
                {
                    throw new ArgumentException("The type for the Value being set does not match the expected data type");
                }
            }
        }

        /// <summary>
        /// Gets the raw data for the COSEM data object
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

                DataWriter.WriteEnum<COSEMDataTypes>(m_DataType);

                switch (m_DataType)
                {
                    case COSEMDataTypes.NullData:
                    case COSEMDataTypes.DontCare:
                    {
                        // Nothing to write
                        break;
                    }
                    case COSEMDataTypes.Array:
                    case COSEMDataTypes.Structure:
                    {
                        // Arrays and structures work the same way. The only restriction is that Arrays should all be the same type
                        COSEMData[] Value = m_Value as COSEMData[];

                        if (Value != null)
                        {
                            DataWriter.WriteLength(Value.Length);

                            foreach (COSEMData CurrentItem in Value)
                            {
                                DataWriter.Write(CurrentItem.Data);
                            }

                            m_Value = Value;
                        }

                        break;
                    }
                    case COSEMDataTypes.Boolean:
                    {
                        DataWriter.Write((bool)m_Value);
                        break;
                    }
                    case COSEMDataTypes.BitString:
                    {
                        DataWriter.WriteBitString((byte[])m_Value);
                        break;
                    }
                    case COSEMDataTypes.DoubleLong:
                    {
                        DataWriter.Write((int)m_Value);
                        break;
                    }
                    case COSEMDataTypes.DoubleLongUnsigned:
                    {
                        DataWriter.Write((uint)m_Value);
                        break;
                    }
                    case COSEMDataTypes.FloatingPoint:
                    case COSEMDataTypes.Float32:
                    {
                        DataWriter.Write((Single)m_Value);
                        break;
                    }
                    case COSEMDataTypes.OctetString:
                    {
                        DataWriter.WriteBytesWithLength((byte[])m_Value);
                        break;
                    }
                    case COSEMDataTypes.VisibleString:
                    {
                        DataWriter.Write((string)m_Value);
                        break;
                    }
                    case COSEMDataTypes.BCD:
                    case COSEMDataTypes.Integer:
                    {
                        DataWriter.Write((sbyte)m_Value);
                        break;
                    }
                    case COSEMDataTypes.Long:
                    {
                        DataWriter.Write((short)m_Value);
                        break;
                    }
                    case COSEMDataTypes.Unsigned:
                    case COSEMDataTypes.Enum:
                    {
                        DataWriter.Write((byte)m_Value);
                        break;
                    }
                    case COSEMDataTypes.LongUnsigned:
                    {
                        DataWriter.Write((ushort)m_Value);
                        break;
                    }
                    case COSEMDataTypes.CompactArray:
                    {
                        // TODO: Support Compact Array
                        break;
                    }
                    case COSEMDataTypes.Long64:
                    {
                        DataWriter.Write((long)m_Value);
                        break;
                    }
                    case COSEMDataTypes.Long64Unsigned:
                    {
                        DataWriter.Write((ulong)m_Value);
                        break;
                    }
                    case COSEMDataTypes.Float64:
                    {
                        DataWriter.Write((double)m_Value);
                        break;
                    }
                    case COSEMDataTypes.DateTime:
                    {
                        DataWriter.Write((COSEMDateTime)m_Value);
                        break;
                    }
                    case COSEMDataTypes.Date:
                    {
                        DataWriter.Write((COSEMDate)m_Value);
                        break;
                    }
                    case COSEMDataTypes.Time:
                    {
                        DataWriter.Write((COSEMTime)m_Value);
                        break;
                    }
                }

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private COSEMDataTypes m_DataType;
        private object m_Value;

        #endregion
    }

    /// <summary>
    /// DLMS Conformance object
    /// </summary>
    public class DLMSConformance
    {
        #region Constants

        private static readonly byte[] CONFORMANCE_TAG = new byte[] { 0x5F, 0x1F };
        private const byte CONFORMANCE_LENGTH = 4;
        private const byte CONFORMANCE_UNUSED_BITS = 0;
        private const int CONFORMANCE_BITS = 24;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DLMSConformance()
        {
            m_Conformance = DLMSConformanceFlags.Default;
        }

        /// <summary>
        /// Parse a conformance object from the specified binary reader
        /// </summary>
        /// <param name="reader">The reader to use to parse the conformance value</param>
        /// <returns>The DLMS Conformance value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public static DLMSConformance Parse(DLMSBinaryReader reader)
        {
            DLMSConformance ParsedValue = null;
            byte[] Tag = reader.ReadBytes(2);

            if (Tag[0] == CONFORMANCE_TAG[0] && Tag[1] == CONFORMANCE_TAG[1])
            {
                // The Conformance value is BER encoded so we have a length and unused bits value included
                byte Length = reader.ReadByte();

                if (Length == CONFORMANCE_LENGTH)
                {
                    byte UnusedBits = reader.ReadByte();

                    if (UnusedBits == CONFORMANCE_UNUSED_BITS)
                    {
                        ParsedValue = new DLMSConformance();

                        ParsedValue.Conformance = reader.ReadBitString<DLMSConformanceFlags>(CONFORMANCE_BITS);
                    }
                    else
                    {
                        throw new ArgumentException("The number of unused bits in the Conformance value is incorrect");
                    }
                }
                else
                {
                    throw new ArgumentException("The length of the Conformance value is incorrect");
                }
            }
            else
            {
                reader.BaseStream.Seek(-2, SeekOrigin.Current);

                throw new ArgumentException("The next value in the Binary Reader is not a Conformance value");
            }

            return ParsedValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Conformance Bit String
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DLMSConformanceFlags Conformance
        {
            get
            {
                return m_Conformance;
            }
            set
            {
                m_Conformance = value;
            }
        }

        /// <summary>
        /// Gets the raw data for the Conformance value
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

                DataWriter.Write(CONFORMANCE_TAG);
                DataWriter.Write(CONFORMANCE_LENGTH);
                DataWriter.Write(CONFORMANCE_UNUSED_BITS);
                DataWriter.WriteBitString<DLMSConformanceFlags>(m_Conformance, CONFORMANCE_BITS);

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private DLMSConformanceFlags m_Conformance;

        #endregion
    }

    /// <summary>
    /// Action Response With Optional Data
    /// </summary>
    public class ActionResponseWithOptionalData
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ActionResponseWithOptionalData()
        {
            m_Result = ActionResults.Success;
            m_ReturnParameters = null;
        }

        /// <summary>
        /// Parses the Action Response from the stream
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);

            m_Result = DataReader.ReadEnum<ActionResults>();

            // The Return Parameter is optional so we need to read the usage flag
            if (DataReader.ReadUsageFlag() == true)
            {
                // The Return Parameters are present
                m_ReturnParameters = new GetDataResult();
                m_ReturnParameters.Parse(dataStream);
            }
            else
            {
                // No Return Parameters
                m_ReturnParameters = null;
            }
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
        
        public ActionResults Result
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
        /// Gets or sets the Return Parameters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public GetDataResult ReturnParameters
        {
            get
            {
                return m_ReturnParameters;
            }
            set
            {
                m_ReturnParameters = value;
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
                MemoryStream DataStream = new MemoryStream();
                DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

                DataWriter.WriteEnum<ActionResults>(m_Result);
                
                // The Return Parameters are optional so we need to write the usage flag and data
                if (m_ReturnParameters != null)
                {
                    DataWriter.WriteUsageFlag(true);
                    DataWriter.Write(m_ReturnParameters.Data);
                }
                else
                {
                    DataWriter.WriteUsageFlag(false);
                }

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private ActionResults m_Result;
        private GetDataResult m_ReturnParameters;

        #endregion
    }
}