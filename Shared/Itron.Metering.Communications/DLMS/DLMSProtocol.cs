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
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Reflection;
using System.ComponentModel;
using Itron.Metering.Utilities;
using OpenSSLWrapper;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// Security Policies
    /// </summary>
    [Serializable]
    public enum DLMSSecurityPolicy
    {
        /// <summary>No authentication or encryption</summary>
        [EnumDescription("None")]
        None = 0,
        /// <summary>Authenticate messages only</summary>
        [EnumDescription("Authenticate Messages")]
        AuthenticateMessages = 1,
        /// <summary>Encrypt messages only</summary>
        [EnumDescription("Encrypt Messages")]
        EncryptMessages = 2,
        /// <summary>Authenticate and encrypt messages</summary>
        [EnumDescription("Authenticate and Encrypt Messages")]
        AuthenticateAndEncryptMessages = 3,
    }

    /// <summary>
    /// Security Types
    /// </summary>
    [Serializable]
    public enum DLMSSecurityType
    {
        /// <summary>No Security</summary>
        [EnumDescription("None")]
        None = 0,
        /// <summary>Low Level Security</summary>
        [EnumDescription("Low Level Security")]
        LowLevelSecurity = 1,
        /// <summary>High Level Security</summary>
        [EnumDescription("High Level Security (no GMAC)")]
        HighLevelSecurity = 2,
        /// <summary>High Level Security with GMAC Authentication</summary>
        [EnumDescription("High Level Security (with GMAC)")]
        HighLevelSecurityWithGMAC = 3,
    }

    /// <summary>
    /// Encryption Modes
    /// </summary>
    public enum DLMSEncryptionModes
    {
        /// <summary>Global</summary>
        Global = 0,
        /// <summary>Dedicated</summary>
        Dedicated = 1,
    }

    /// <summary>
    /// The DLMS protocol object
    /// </summary>
    public class DLMSProtocol
    {
        #region Constants

        private static readonly TimeSpan RESPONSE_TIMEOUT = TimeSpan.FromMilliseconds(10000);
        private static readonly TimeSpan ACTION_RESPONSE_TIMEOUT = TimeSpan.FromMilliseconds(10000);
        private const int CHALLENGE_LENGTH = 31;

        private const int GMAC_LENGTH = 96;
        private const byte GMAC_SECURITY_CONTROL = 0x10; // Authentication only
        private const int TAG_LENGTH = 12;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comm">The Communications object to use</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DLMSProtocol(IDLMSComm comm)
        {
            m_APDUReceivedHandler = new APDUEventHandler(m_Comm_APDUReceived);
            m_ReceivedMessages = new List<xDLMSAPDU>();
            m_Comm = comm;
            m_Connected = false;
            m_Logger = Logger.TheInstance;

            // Initialize the Security Settings
            m_SecurityType = DLMSSecurityType.None;
            m_SecurityPolicy = DLMSSecurityPolicy.None;
            m_EncryptionMode = DLMSEncryptionModes.Dedicated;

            GlobalEncryptionKey = null;
            AuthenticationKey = null;
            m_FrameCounter = 1;

            GenerateClientApTitle();
            GenerateDedicatedKey();
            GenerateClientChallenge();
        }

        /// <summary>
        /// Open the DLMS connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Connect()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Opening Connection");

                m_Connected = false;

                if (m_Comm.IsOpen == false)
                {
                    m_Comm.Open();
                }

                m_ReceivedMessages.Clear();
                m_Comm.ClearAPDUReceivedHandlers(); // Make sure there is only one handler
                m_Comm.APDUReceived += m_APDUReceivedHandler; 

                // Send the AARQ message
                AARQAPDU AARQ = CreateAARQ();
                AAREAPDU AARE = null;
                ConfirmedServiceErrorAPDU ServiceError = null;

                lock (m_Comm)
                {
                    SendAPDU(AARQ);

                    // Wait for the AARE response
                    DateTime StartTime = DateTime.Now;

                    while (AARE == null && ServiceError == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
                    {
                        lock (m_ReceivedMessages)
                        {
                            if (m_ReceivedMessages.Where(m => m is AAREAPDU).Count() > 0)
                            {
                                // We found the AARE message
                                AARE = m_ReceivedMessages.Where(m => m is AAREAPDU).First() as AAREAPDU;

                                // Make sure we remove the message from the list of Received Messages.
                                m_ReceivedMessages.Remove(AARE);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                            {
                                ServiceError = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;

                                m_ReceivedMessages.Remove(ServiceError);
                            }
                        }

                        if (AARE == null && ServiceError == null)
                        {
                            Thread.Sleep(1);
                        }
                    }
                }

                if (AARE != null)
                {
                    xDLMSAPDU Response = xDLMSAPDU.Create((xDLMSTags)AARE.UserInformation[0]);
                    CipheredAPDU CipheredResponse = Response as CipheredAPDU;
                    MemoryStream DataStream = new MemoryStream(AARE.UserInformation);

                    if (AARE.ApplicationContextName.IsEqual(AARQ.ApplicationContextName))
                    {
                        ServerApTitle = AARE.RespondingApTitle;
                        m_ServerChallenge = AARE.RespondingAuthenticationValue;

                        // Make sure we set the Decrypt Key to the StoC if that is what we are using.
                        if (m_UsingChallengAsAuthenticationKey)
                        {
                            DecryptAuthenticationKey = m_ServerChallenge;
                        }

                        if (AARE.UserInformation != null && AARE.UserInformation.Count() > 0)
                        {
                            // Set up the Security Data if it's a Ciphered APDU
                            if (CipheredResponse != null)
                            {
                                CipheredResponse.BlockCipherKey = GlobalEncryptionKey;
                                CipheredResponse.AuthenticationKey = DecryptAuthenticationKey;
                                CipheredResponse.ApTitle = ServerApTitle;
                            }

                            Response.Parse(DataStream);

                            if (CipheredResponse != null)
                            {
                                // Pull out the Unciphered Response
                                Response = CipheredResponse.UncipheredAPDU;
                            }

                            switch (Response.Tag)
                            {
                                case xDLMSTags.InitiateResponse:
                                {
                                    if (AARE.Result == AssociationResult.Accepted)
                                    {
                                        InitiateResponseAPDU InitiateResponse = Response as InitiateResponseAPDU;

                                        m_ServerMaxAPDUSize = InitiateResponse.ServerMaxReceivePDUSize;

                                        // If the Comm Port's Max APDU Size is smaller we should use that instead
                                        if (m_Comm.MaxAPDUSize < m_ServerMaxAPDUSize)
                                        {
                                            m_ServerMaxAPDUSize = m_Comm.MaxAPDUSize;
                                        }

                                        m_NegotiatedConformance = InitiateResponse.NegotiatedConformance;

                                        m_Connected = true;
                                    }
                                    else
                                    {
                                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Connect Failed - AARQ rejected.");
                                    }

                                    break;
                                }
                                case xDLMSTags.ConfirmedServiceError:
                                {
                                    ServiceError = Response as ConfirmedServiceErrorAPDU;

                                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Connect failed - Confirmed Service Error. Reason: " + ServiceError.ServiceError.ToDescription());
                                    break;
                                }
                                default:
                                {
                                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Connect failed - Unexpected response message in the AARE. Message Type: " + Response.Tag.ToDescription());
                                    break;
                                }
                            }
                        }
                        else
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Connect failed - The AARE did not contain an Initiate Response APDU.");
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Connect failed - The AARE Application Context Name is not valid.");
                    }
                }
                else if (ServiceError != null)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Connect Failed - Confirmed Service Error Received. Error: " + ServiceError.ServiceError.ToDescription());

                    // Clear the data buffers of bad data that may have been received
                    ClearDataBuffers();

                    throw new DLMSResponseException("Confirmed Service Error Received.", ServiceError);
                }
                else
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Connect Failed - Timed out while waiting for the AARE message.");

                    // Clear the data buffers of bad data that may have been received
                    ClearDataBuffers();

                    throw new TimeoutException("The AARE message was not received within the allowed time.");
                }
            }
            finally
            {
                // Make sure we close the Comm Port if it's still open
                if (m_Comm.IsOpen && m_Connected == false)
                {
                    m_Comm.Close();
                }

            }
        }

        /// <summary>
        /// Close the DLMS connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Disconnect()
        {
            if (IsConnected)
            {
                if (m_Comm.IsOpen)
                {
                    // We need to send the RLRQ message
                    RLRQAPDU RLRQ = CreateRLRQ();
                    RLREAPDU RLRE = null;

                    lock (m_Comm)
                    {
                        SendAPDU(RLRQ);

                        // Wait for the RLRE response
                        DateTime StartTime = DateTime.Now;

                        while (RLRE == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
                        {
                            lock (m_ReceivedMessages)
                            {
                                if (m_ReceivedMessages.Where(m => m is RLREAPDU).Count() > 0)
                                {
                                    // We found the AARE message
                                    RLRE = m_ReceivedMessages.Where(m => m is RLREAPDU).First() as RLREAPDU;

                                    // Make sure we remove the message from the list of Received Messages.
                                    m_ReceivedMessages.Remove(RLRE);
                                }
                            }

                            if (RLRE == null)
                            {
                                Thread.Sleep(1);
                            }
                        }
                    }

                    if (RLRE == null)
                    {
                        // We timed out but we still want to do the rest of the disconnect so just log a message
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Timed out while waiting for the RLRE message.");
                    }

                    m_Comm.APDUReceived -= m_APDUReceivedHandler;
                    m_Connected = false;
                }
            }

            // Make sure we always close the port
            if (m_Comm.IsOpen)
            {
                m_Comm.Close();
            }
        }

        /// <summary>
        /// Performs a Normal Get Request
        /// </summary>
        /// <param name="classID">The class ID of the object containing the item to get</param>
        /// <param name="instanceID">The instance ID of the object containing the item to get</param>
        /// <param name="attributeID">The attribute ID of the item to get</param>
        /// <returns>The result of the get</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetDataResult Get(ushort classID, byte[] instanceID, sbyte attributeID)
        {
            return Get(classID, instanceID, attributeID, null);
        }

        /// <summary>
        /// Performs a Normal Get Request
        /// </summary>
        /// <param name="classID">The class ID of the object containing the item to get</param>
        /// <param name="instanceID">The instance ID of the object containing the item to get</param>
        /// <param name="attributeID">The attribute ID of the item to get</param>
        /// <param name="accessSelection">The selective access object</param>
        /// <returns>The result of the get</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public GetDataResult Get(ushort classID, byte[] instanceID, sbyte attributeID, SelectiveAccessDescriptor accessSelection)
        {
            // Get the Invoke ID to use for this request and increment for the next request
            byte InvokeID = CurrentInvokeID;
            CurrentInvokeID++;

            GetDataResult Result = null;

            // Generate the Normal Get Request APDU
            GetRequestAPDU GetRequest = new GetRequestAPDU();
            GetRequestNormal NormalRequest = new GetRequestNormal();
            GetResponseAPDU GetResponse = null;

            GetRequest.RequestType = GetRequestAPDU.GetRequestChoice.Normal;
            GetRequest.Request = NormalRequest;

            NormalRequest.ServiceClass = ServiceClasses.Confirmed;
            NormalRequest.Priority = Priorities.Normal;
            NormalRequest.InvokeID = InvokeID;

            NormalRequest.AttributeDescriptor = new CosemAttributeDescriptor();
            NormalRequest.AttributeDescriptor.ClassID = classID;
            NormalRequest.AttributeDescriptor.InstanceID = instanceID;
            NormalRequest.AttributeDescriptor.AttributeID = attributeID;

            NormalRequest.AccessSelection = accessSelection;

            lock (m_Comm)
            {
                SendAPDU(GetRequest);

                DateTime StartTime = DateTime.Now;

                while (GetResponse == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
                {
                    lock (m_ReceivedMessages)
                    {
                        if (m_ReceivedMessages.Where(m => m is GetResponseAPDU && ((GetResponseAPDU)m).Response.InvokeID == InvokeID).Count() > 0)
                        {
                            // We received the response
                            GetResponse = m_ReceivedMessages.Where(m => m is GetResponseAPDU && ((GetResponseAPDU)m).Response.InvokeID == InvokeID).First() as GetResponseAPDU;
                            m_ReceivedMessages.Remove(GetResponse);
                        }
                        else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                        {
                            // We received an exception response
                            ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                            m_ReceivedMessages.Remove(ExceptionResponse);

                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                                + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));

                            throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                        }
                        else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                        {
                            ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                            m_ReceivedMessages.Remove(ServiceErrorResponse);

                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                                + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());

                            throw new DLMSResponseException("Confirmed Service Error APDU received.", ServiceErrorResponse);
                        }
                    }

                    if (GetResponse == null)
                    {
                        Thread.Sleep(1);
                    }
                }

                if (GetResponse != null)
                {
                    if (GetResponse.ResponseType == GetResponseAPDU.GetResponseChoices.Normal)
                    {
                        GetResponseNormal Response = GetResponse.Response as GetResponseNormal;
                        Result = Response.Result;
                    }
                    else if (GetResponse.ResponseType == GetResponseAPDU.GetResponseChoices.WithDataBlock)
                    {
                        Result = HandleGetBlockResponse(GetResponse.Response as GetResponseWithDatablock);
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Normal Get Failed - Unexpected Response Type received.");
                        throw new ArgumentException("Unexpected Get Response Type.");
                    }
                }
                else
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Normal Get Failed - Get Request has timed out.");

                    // Clear the data buffers of bad data that may have been received
                    ClearDataBuffers();

                    throw new TimeoutException("The Get Response message was not received in the allowed time.");
                }
            }

            return Result;
        }

        /// <summary>
        /// Get With List
        /// </summary>
        /// <param name="attributeDescriptors">The list of attributes to get</param>
        /// <returns>The results of the get</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<GetDataResult> Get(List<CosemAttributeDescriptorWithSelection> attributeDescriptors)
        {
            // Get the Invoke ID to use for this request and increment for the next request
            byte InvokeID = CurrentInvokeID;
            CurrentInvokeID++;

            List<GetDataResult> Result = null;

            // Generate the Normal Get Request APDU
            GetRequestAPDU GetRequest = new GetRequestAPDU();
            GetRequestWithList RequestWithList = new GetRequestWithList();
            GetResponseAPDU GetResponse = null;

            GetRequest.RequestType = GetRequestAPDU.GetRequestChoice.WithList;
            GetRequest.Request = RequestWithList;

            RequestWithList.ServiceClass = ServiceClasses.Confirmed;
            RequestWithList.Priority = Priorities.Normal;
            RequestWithList.InvokeID = InvokeID;

            // Build up the list of items to get
            RequestWithList.AttributeDescriptorList = attributeDescriptors;

            lock (m_Comm)
            {
                SendAPDU(GetRequest);

                DateTime StartTime = DateTime.Now;

                while (GetResponse == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
                {
                    lock (m_ReceivedMessages)
                    {
                        if (m_ReceivedMessages.Where(m => m is GetResponseAPDU && ((GetResponseAPDU)m).Response.InvokeID == InvokeID).Count() > 0)
                        {
                            // We received the response
                            GetResponse = m_ReceivedMessages.Where(m => m is GetResponseAPDU && ((GetResponseAPDU)m).Response.InvokeID == InvokeID).First() as GetResponseAPDU;
                            m_ReceivedMessages.Remove(GetResponse);
                        }
                        else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                        {
                            // We received an exception response
                            ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                            m_ReceivedMessages.Remove(ExceptionResponse);

                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + ExceptionResponse.ServiceError.ToDescription()
                                + " State Error: " + ExceptionResponse.StateError.ToDescription());

                            throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                        }
                        else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                        {
                            ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                            m_ReceivedMessages.Remove(ServiceErrorResponse);

                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                                + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());

                            throw new DLMSResponseException("Confirmed Service Error APDU received.", ServiceErrorResponse);
                        }
                    }

                    if (GetResponse == null)
                    {
                        Thread.Sleep(1);
                    }
                }

                if (GetResponse != null)
                {
                    if (GetResponse.ResponseType == GetResponseAPDU.GetResponseChoices.WithList)
                    {
                        GetResponseWithList Response = GetResponse.Response as GetResponseWithList;
                        Result = Response.Result;
                    }
                    else if (GetResponse.ResponseType == GetResponseAPDU.GetResponseChoices.Normal)
                    {
                        // If only one item is specified it's possible to get a Normal Response
                        GetResponseNormal Response = GetResponse.Response as GetResponseNormal;

                        Result = new List<GetDataResult>();
                        Result.Add(Response.Result);
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Get With List Failed - Unexpected Response Type received.");
                        throw new ArgumentException("Unexpected Get Response Type.");
                    }
                }
                else
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Get With List Failed - Get Request has timed out.");

                    // Clear the data buffers of bad data that may have been received
                    ClearDataBuffers();

                    throw new TimeoutException("The Get With List Response message was not received in the allowed time.");
                }
            }

            return Result;
        }

        /// <summary>
        /// Get With List to be used with attributes within the same instance
        /// </summary>
        /// <param name="classID">The class ID of the object</param>
        /// <param name="instanceID">The instance ID of the object</param>
        /// <param name="attributeIDs">The list of attributes to get</param>
        /// <returns>The results for each item in the get</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public List<GetDataResult> Get(ushort classID, byte[] instanceID, List<sbyte> attributeIDs)
        {
            List<CosemAttributeDescriptorWithSelection> DescriptorList = new List<CosemAttributeDescriptorWithSelection>();

            foreach (sbyte CurrentAttribute in attributeIDs)
            {
                CosemAttributeDescriptorWithSelection CurrentDescriptor = new CosemAttributeDescriptorWithSelection();

                CurrentDescriptor.AttributeDescriptor = new CosemAttributeDescriptor();
                CurrentDescriptor.AttributeDescriptor.ClassID = classID;
                CurrentDescriptor.AttributeDescriptor.InstanceID = instanceID;
                CurrentDescriptor.AttributeDescriptor.AttributeID = CurrentAttribute;
                CurrentDescriptor.AccessSelection = null;

                DescriptorList.Add(CurrentDescriptor);
            }

            return Get(DescriptorList);
        }

        /// <summary>
        /// Performs a Normal Set Request
        /// </summary>
        /// <param name="classID">The class ID of the object containing the item to set</param>
        /// <param name="instanceID">The instance ID of the object containing the item to set</param>
        /// <param name="attributeID">The attribute ID of the item to set</param>
        /// <param name="data">The value to set</param>
        /// <returns>The result of the set</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DataAccessResults Set(ushort classID, byte[] instanceID, sbyte attributeID, COSEMData data)
        {
            // Get the Invoke ID to use for this request and increment for the next request
            byte InvokeID = CurrentInvokeID;
            CurrentInvokeID++;

            DataAccessResults Result = DataAccessResults.Other;

            SetRequestAPDU SetRequest = new SetRequestAPDU();
            SetRequestNormal NormalRequest = new SetRequestNormal();
            SetResponseAPDU SetResponse = null;

            SetRequest.RequestType = SetRequestAPDU.RequestTypes.Normal;
            SetRequest.Request = NormalRequest;

            NormalRequest.InvokeID = InvokeID;
            NormalRequest.Priority = Priorities.Normal;
            NormalRequest.ServiceClass = ServiceClasses.Confirmed;

            NormalRequest.AttributeDescriptor = new CosemAttributeDescriptor();
            NormalRequest.AttributeDescriptor.ClassID = classID;
            NormalRequest.AttributeDescriptor.InstanceID = instanceID;
            NormalRequest.AttributeDescriptor.AttributeID = attributeID;

            NormalRequest.Value = data;

            lock (m_Comm)
            {
                if (SetRequest.Data.Length > MaxSetAPDUSize)
                {
                    SetResponse = SetUsingDataBlocks(NormalRequest);
                }
                else
                {
                    SendAPDU(SetRequest);

                    DateTime StartTime = DateTime.Now;

                    while (SetResponse == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
                    {
                        lock (m_ReceivedMessages)
                        {
                            if (m_ReceivedMessages.Where(m => m is SetResponseAPDU && ((SetResponseAPDU)m).Response.InvokeID == InvokeID).Count() > 0)
                            {
                                // We received the response
                                SetResponse = m_ReceivedMessages.Where(m => m is SetResponseAPDU && ((SetResponseAPDU)m).Response.InvokeID == InvokeID).First() as SetResponseAPDU;
                                m_ReceivedMessages.Remove(SetResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                            {
                                // We received an exception response
                                ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                                m_ReceivedMessages.Remove(ExceptionResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                                    + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));

                                throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                            {
                                ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                                m_ReceivedMessages.Remove(ServiceErrorResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                                    + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());

                                throw new DLMSResponseException("Confirmed Service Error APDU received.", ServiceErrorResponse);
                            }
                        }

                        if (SetResponse == null)
                        {
                            Thread.Sleep(1);
                        }
                    }
                }

                if (SetResponse != null)
                {
                    if (SetResponse.ResponseType == SetResponseAPDU.ResponseTypes.Normal)
                    {
                        SetResponseNormal NormalResponse = SetResponse.Response as SetResponseNormal;
                        Result = NormalResponse.Result;
                    }
                    else if (SetResponse.ResponseType == SetResponseAPDU.ResponseTypes.LastDataBlock)
                    {
                        SetResponseLastDataBlock DataBlockResponse = SetResponse.Response as SetResponseLastDataBlock;
                        Result = DataBlockResponse.Result;
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Normal Set Failed - Unexpected Response Type received.");
                        throw new ArgumentException("Unexpected Set Response Type.");
                    }
                }
                else
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Normal Set Failed - Set Request has timed out.");

                    // Clear the data buffers of bad data that may have been received
                    ClearDataBuffers();

                    throw new TimeoutException("The Set Response message was not received in the allowed time.");
                }
            }

            return Result;
        }

        /// <summary>
        /// Set With List
        /// </summary>
        /// <param name="attributeDescriptors">The list of attributes to set</param>
        /// <param name="data">The list of data to set</param>
        /// <returns>The results of each set</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<DataAccessResults> Set(List<CosemAttributeDescriptorWithSelection> attributeDescriptors, List<COSEMData> data)
        {
            List<DataAccessResults> Result = null;

            if (attributeDescriptors.Count == data.Count)
            {
                // Get the Invoke ID to use for this request and increment for the next request
                byte InvokeID = CurrentInvokeID;
                CurrentInvokeID++;

                SetRequestAPDU SetRequest = new SetRequestAPDU();
                SetRequestWithList RequestWithList = new SetRequestWithList();
                SetResponseAPDU SetResponse = null;

                SetRequest.RequestType = SetRequestAPDU.RequestTypes.WithList;
                SetRequest.Request = RequestWithList;

                RequestWithList.InvokeID = InvokeID;
                RequestWithList.Priority = Priorities.Normal;
                RequestWithList.ServiceClass = ServiceClasses.Confirmed;

                RequestWithList.AttributeDescriptorList = attributeDescriptors;

                RequestWithList.ValueList = data;

                lock (m_Comm)
                {
                    SendAPDU(SetRequest);

                    DateTime StartTime = DateTime.Now;

                    while (SetResponse == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
                    {
                        lock (m_ReceivedMessages)
                        {
                            if (m_ReceivedMessages.Where(m => m is SetResponseAPDU && ((SetResponseAPDU)m).Response.InvokeID == InvokeID).Count() > 0)
                            {
                                // We received the response
                                SetResponse = m_ReceivedMessages.Where(m => m is SetResponseAPDU && ((SetResponseAPDU)m).Response.InvokeID == InvokeID).First() as SetResponseAPDU;
                                m_ReceivedMessages.Remove(SetResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                            {
                                // We received an exception response
                                ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                                m_ReceivedMessages.Remove(ExceptionResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                                    + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));

                                throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                            {
                                ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                                m_ReceivedMessages.Remove(ServiceErrorResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                                    + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());

                                throw new DLMSResponseException("Confirmed Service Error APDU received.", ServiceErrorResponse);
                            }
                        }

                        if (SetResponse == null)
                        {
                            Thread.Sleep(1);
                        }
                    }

                    if (SetResponse != null)
                    {
                        if (SetResponse.ResponseType == SetResponseAPDU.ResponseTypes.WithList)
                        {
                            SetResponseWithList ResponseWithList = SetResponse.Response as SetResponseWithList;
                            Result = ResponseWithList.Results;
                        }
                        else
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set With List Failed - Unexpected Response Type received.");
                            throw new ArgumentException("Unexpected Set Response Type.");
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set With List Failed - Set Request has timed out.");

                        // Clear the data buffers of bad data that may have been received
                        ClearDataBuffers();

                        throw new TimeoutException("The Set With List Response message was not received in the allowed time.");
                    }
                }
            }
            else
            {
                throw new ArgumentException("The length of the descriptor list and the length of the data list must match.");
            }

            return Result;
        }

        /// <summary>
        /// Performs a Set Request With List
        /// </summary>
        /// <param name="classID">The class ID of the object containing the items to set</param>
        /// <param name="instanceID">The instance ID of the object containing the item to set</param>
        /// <param name="attributeIDs">The attribute IDs of the items to set</param>
        /// <param name="data">The values to set</param>
        /// <returns>The result of the set</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<DataAccessResults> Set(ushort classID, byte[] instanceID, List<sbyte> attributeIDs, List<COSEMData> data)
        {
            List<CosemAttributeDescriptorWithSelection> DescriptorList = new List<CosemAttributeDescriptorWithSelection>();

            foreach (sbyte CurrentAttribute in attributeIDs)
            {
                CosemAttributeDescriptorWithSelection CurrentDescriptor = new CosemAttributeDescriptorWithSelection();

                CurrentDescriptor.AttributeDescriptor = new CosemAttributeDescriptor();
                CurrentDescriptor.AttributeDescriptor.ClassID = classID;
                CurrentDescriptor.AttributeDescriptor.InstanceID = instanceID;
                CurrentDescriptor.AttributeDescriptor.AttributeID = CurrentAttribute;
                CurrentDescriptor.AccessSelection = null;

                DescriptorList.Add(CurrentDescriptor);
            }

            return Set(DescriptorList, data);
        }

        /// <summary>
        /// Normal Action Request
        /// </summary>
        /// <param name="classID">The class ID of the method to call</param>
        /// <param name="instanceID">The instance ID of the method to call</param>
        /// <param name="methodID">The ID of the method to call</param>
        /// <param name="parameters">The parameters to the method to call</param>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ActionResponseWithOptionalData Action(ushort classID, byte[] instanceID, sbyte methodID, COSEMData parameters)
        {
            // Get the Invoke ID to use for this request and increment for the next request
            byte InvokeID = CurrentInvokeID;
            CurrentInvokeID++;

            ActionResponseWithOptionalData Result = null;

            ActionRequestAPDU ActionRequest = new ActionRequestAPDU();
            ActionRequestNormal NormalRequest = new ActionRequestNormal();
            ActionResponseAPDU ActionResponse = null;

            ActionRequest.RequestType = ActionRequestAPDU.RequestTypes.Normal;
            ActionRequest.Request = NormalRequest;

            NormalRequest.InvokeID = InvokeID;
            NormalRequest.Priority = Priorities.Normal;
            NormalRequest.ServiceClass = ServiceClasses.Confirmed;

            NormalRequest.MethodDescriptor = new CosemMethodDescriptor();
            NormalRequest.MethodDescriptor.ClassID = classID;
            NormalRequest.MethodDescriptor.InstanceID = instanceID;
            NormalRequest.MethodDescriptor.MethodID = methodID;

            NormalRequest.Parameters = parameters;

            lock (m_Comm)
            {
                if (ActionRequest.Data.Length > MaxSetAPDUSize)
                {
                    ActionResponse = SendActionUsingBlocks(NormalRequest);
                }
                else
                {
                    SendAPDU(ActionRequest);

                    DateTime StartTime = DateTime.Now;

                    while (ActionResponse == null && DateTime.Now - StartTime < ACTION_RESPONSE_TIMEOUT)
                    {
                        lock (m_ReceivedMessages)
                        {
                            if (m_ReceivedMessages.Where(m => m is ActionResponseAPDU && ((ActionResponseAPDU)m).Response.InvokeID == InvokeID).Count() > 0)
                            {
                                // We received the response
                                ActionResponse = m_ReceivedMessages.Where(m => m is ActionResponseAPDU && ((ActionResponseAPDU)m).Response.InvokeID == InvokeID).First() as ActionResponseAPDU;
                                m_ReceivedMessages.Remove(ActionResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                            {
                                // We received an exception response
                                ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                                m_ReceivedMessages.Remove(ExceptionResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                                    + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));

                                throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                            {
                                ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                                m_ReceivedMessages.Remove(ServiceErrorResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                                    + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());

                                throw new DLMSResponseException("Confirmed Service Error APDU received.", ServiceErrorResponse);
                            }
                        }

                        if (ActionResponse == null)
                        {
                            Thread.Sleep(1);
                        }
                    }
                }

                if (ActionResponse != null)
                {
                    if (ActionResponse.ResponseType == ActionResponseAPDU.ResponseTypes.Normal)
                    {
                        ActionResponseNormal NormalResponse = ActionResponse.Response as ActionResponseNormal;
                        Result = NormalResponse.Response;
                    }
                    else if (ActionResponse.ResponseType == ActionResponseAPDU.ResponseTypes.WithBlock)
                    {
                        // There could be more blocks to read as the Action response so lets make sure we handle that
                        Result = HandleActionBlockResponse(ActionResponse.Response as ActionResponseWithBlock);
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Action Failed - Unexpected Response Type received.");
                        throw new ArgumentException("Unexpected Set Response Type.");
                    }
                }
                else
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Action Failed - Action Request has timed out.");

                    // Clear the data buffers of bad data that may have been received
                    ClearDataBuffers();

                    throw new TimeoutException("The Action Response message was not received in the allowed time.");
                }
            }

            return Result;
        }

        /// <summary>
        /// Action With List
        /// </summary>
        /// <param name="methodDescriptors">The list of methods to call</param>
        /// <param name="parameters">The list of parameters for each method</param>
        /// <returns>The list of results for each action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public List<ActionResponseWithOptionalData> Action(List<CosemMethodDescriptor> methodDescriptors, List<COSEMData> parameters)
        {
            List<ActionResponseWithOptionalData> Result = null;

            if (methodDescriptors.Count == parameters.Count)
            {
                // Get the Invoke ID to use for this request and increment for the next request
                byte InvokeID = CurrentInvokeID;
                CurrentInvokeID++;


                ActionRequestAPDU ActionRequest = new ActionRequestAPDU();
                ActionRequestWithList Request = new ActionRequestWithList();
                ActionResponseAPDU ActionResponse = null;

                ActionRequest.RequestType = ActionRequestAPDU.RequestTypes.WithList;
                ActionRequest.Request = Request;

                Request.InvokeID = InvokeID;
                Request.Priority = Priorities.Normal;
                Request.ServiceClass = ServiceClasses.Confirmed;

                Request.MethodDescriptorList = methodDescriptors;
                Request.ParameterList = parameters;

                lock (m_Comm)
                {
                    SendAPDU(ActionRequest);

                    DateTime StartTime = DateTime.Now;

                    while (ActionResponse == null && DateTime.Now - StartTime < ACTION_RESPONSE_TIMEOUT)
                    {
                        lock (m_ReceivedMessages)
                        {
                            if (m_ReceivedMessages.Where(m => m is ActionResponseAPDU && ((ActionResponseAPDU)m).Response.InvokeID == InvokeID).Count() > 0)
                            {
                                // We received the response
                                ActionResponse = m_ReceivedMessages.Where(m => m is ActionResponseAPDU && ((ActionResponseAPDU)m).Response.InvokeID == InvokeID).First() as ActionResponseAPDU;
                                m_ReceivedMessages.Remove(ActionResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                            {
                                // We received an exception response
                                ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                                m_ReceivedMessages.Remove(ExceptionResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                                    + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));
                                throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                            {
                                ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                                m_ReceivedMessages.Remove(ServiceErrorResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                                    + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());

                                throw new DLMSResponseException("Confirmed Service Error APDU received.", ServiceErrorResponse);
                            }
                        }

                        if (ActionResponse == null)
                        {
                            Thread.Sleep(1);
                        }
                    }

                    if (ActionResponse != null)
                    {
                        if (ActionResponse.ResponseType == ActionResponseAPDU.ResponseTypes.WithList)
                        {
                            ActionResponseWithList NormalResponse = ActionResponse.Response as ActionResponseWithList;
                            Result = NormalResponse.Responses;
                        }
                        else
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Action With List Failed - Unexpected Response Type received.");
                            throw new ArgumentException("Unexpected Set Response Type.");
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Action With List Failed - Action Request has timed out.");

                        // Clear the data buffers of bad data that may have been received
                        ClearDataBuffers();

                        throw new TimeoutException("The Action With List Response message was not received in the allowed time.");
                    }
                }
            }
            else
            {
                throw new ArgumentException("The length of the descriptor list and the length of the parameters list must match.");
            }

            return Result;
        }

        /// <summary>
        /// Gets the GMAC that needs to be sent as a response to the Server's challenge
        /// </summary>
        /// <returns>The GMAC response</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/14/13 RCG 2.80.38 N/A    Created
        
        public byte[] GetServerChallengeResponse()
        {
            MemoryStream ResponseStream = new MemoryStream();
            DLMSBinaryWriter ResponseWriter = new DLMSBinaryWriter(ResponseStream);
            byte[] Response = null;
            byte[] GMAC = null;

            // Only calculate the value if we have already connected, we are using HLS, and we have received a challenge
            if (IsConnected && SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC && m_ServerChallenge != null)
            {
                ResponseWriter.Write(GMAC_SECURITY_CONTROL);
                ResponseWriter.Write(m_FrameCounter);

                if (m_UsingChallengAsAuthenticationKey)
                {
                    // Just use SC || StoC
                    GMAC = CalculateGMAC(BuildAuthenticatedData(m_ServerChallenge, new byte[0]), m_ClientApTitle, m_FrameCounter);
                }
                else
                {
                    GMAC = CalculateGMAC(BuildAuthenticatedData(m_ServerChallenge, AuthenticationKey), m_ClientApTitle, m_FrameCounter);
                }

                ResponseWriter.Write(GMAC);

                Response = ResponseStream.ToArray();
            }

            return Response;
        }

        /// <summary>
        /// Validates the response to the Client challenge
        /// </summary>
        /// <param name="challengeResponse">The response to validate</param>
        /// <returns>True if the response is valid. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/14/13 RCG 2.80.38 N/A    Created
        
        public bool ValidateClientChallengeResponse(byte[] challengeResponse)
        {
            bool Valid = true;
            MemoryStream ResponseStream = new MemoryStream(challengeResponse);
            DLMSBinaryReader ResponseReader = new DLMSBinaryReader(ResponseStream);

            if (IsConnected && SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC && challengeResponse != null)
            {
                byte SecurityControl = ResponseReader.ReadByte();
                uint FrameCounter = ResponseReader.ReadUInt32();
                byte[] ActualGMAC = ResponseReader.ReadBytes(TAG_LENGTH);
                byte[] ExpectedGMAC = null;

                // First calculate what we think it should be
                if (m_UsingChallengAsAuthenticationKey)
                {
                    // Just use SC || CtoS
                    ExpectedGMAC = CalculateGMAC(BuildAuthenticatedData(m_ClientChallenge, new byte[0]), ServerApTitle, FrameCounter);
                }
                else
                {
                    ExpectedGMAC = CalculateGMAC(BuildAuthenticatedData(m_ClientChallenge, AuthenticationKey), ServerApTitle, FrameCounter);
                }

                if (ExpectedGMAC.Length == ActualGMAC.Length)
                {
                    for (int iIndex = 0; iIndex < ExpectedGMAC.Length; iIndex++)
                    {
                        if (ExpectedGMAC[iIndex] != ActualGMAC[iIndex])
                        {
                            Valid = false;
                            break;
                        }
                    }
                }
                else
                {
                    Valid = false;
                }
            }

            return Valid;
        }

        /// <summary>
        /// Overrides the Calling AP Title used during communication with the device. 
        /// </summary>
        /// <param name="callingApTitle">The Calling AP Title to use. May be partial or all 8 characters</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/18/13 RCG 3.50.05 N/A    Created

        public void OverrideCallingApTitle(string callingApTitle)
        {
            if (String.IsNullOrEmpty(callingApTitle) == false)
            {
                // First generate the AP Title like we normally would in case the overridden value is too short
                GenerateClientApTitle();

                for (int CurrentIndex = 0; CurrentIndex < m_ClientApTitle.Length && CurrentIndex < callingApTitle.Length; CurrentIndex++)
                {
                    m_ClientApTitle[CurrentIndex] = (byte)callingApTitle[CurrentIndex];
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the GMAC for the given data
        /// </summary>
        /// <param name="data">The data to calculate the GMAC for</param>
        /// <param name="apTitle">The ApTitle to use</param>
        /// <param name="frameCounter">The frame counter</param>
        /// <returns>The GMAC</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private byte[] CalculateGMAC(byte[] data, byte[] apTitle, uint frameCounter)
        {
            byte[] Tag = null;
            Aes128GcmCipher GcmCipher = new Aes128GcmCipher(GlobalEncryptionKey, GetIV(apTitle, frameCounter));

            GcmCipher.Authenticate(data, out Tag);

            return Tag;
        }

        /// <summary>
        /// Gets the IV from the specified ApTitle and Frame Counter
        /// </summary>
        /// <param name="apTitle">The ApTitle to use</param>
        /// <param name="frameCounter">The frame counter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created

        private byte[] GetIV(byte[] apTitle, uint frameCounter)
        {
            MemoryStream IVStream = new MemoryStream();
            DLMSBinaryWriter IVWriter = new DLMSBinaryWriter(IVStream);

            IVWriter.Write(apTitle);
            IVWriter.Write(frameCounter);

            return IVStream.ToArray();
        }

        /// <summary>
        /// Builds the Authentication data to calculate the GMAC
        /// </summary>
        /// <param name="data">The data to calculate the GMAC for</param>
        /// <param name="authKey"></param>
        /// <returns>The authentication data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private byte[] BuildAuthenticatedData(byte[] data, byte[] authKey)
        {
            byte[] Result = null;
            MemoryStream DataStream = new MemoryStream();
            BinaryWriter DataWriter = new BinaryWriter(DataStream);

            // The apdu is not ciphered so it is a part of the Authenticated Data
            DataWriter.Write(GMAC_SECURITY_CONTROL);
            DataWriter.Write(authKey);
            DataWriter.Write(data);

            Result = DataStream.ToArray();

            return Result;
        }

        /// <summary>
        /// Performs a set by breaking up the data into blocks
        /// </summary>
        /// <param name="setRequest">The original set request</param>
        /// <returns>The result of the set</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/31/13 RCG 2.80.36 N/A    Created

        private SetResponseAPDU SetUsingDataBlocks(SetRequestNormal setRequest)
        {
            SetResponseAPDU Response = null;

            if (setRequest != null && setRequest.Value != null)
            {
                bool SetComplete = false;
                uint CurrentIndex = 0;

                do
                {
                    SetRequestAPDU CurrentRequest = GetDataBlockRequest(CurrentIndex, setRequest);
                    SetResponseAPDU CurrentResponse = null;

                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Setting Block " + (CurrentIndex + 1).ToString(CultureInfo.InvariantCulture));
                    SendAPDU(CurrentRequest);

                    DateTime StartTime = DateTime.Now;

                    while (CurrentResponse == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
                    {
                        lock (m_ReceivedMessages)
                        {
                            if (m_ReceivedMessages.Where(m => m is SetResponseAPDU && ((SetResponseAPDU)m).Response.InvokeID == setRequest.InvokeID).Count() > 0)
                            {
                                // We received the response
                                CurrentResponse = m_ReceivedMessages.Where(m => m is SetResponseAPDU && ((SetResponseAPDU)m).Response.InvokeID == setRequest.InvokeID).First() as SetResponseAPDU;
                                m_ReceivedMessages.Remove(CurrentResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                            {
                                // We received an exception response
                                ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                                m_ReceivedMessages.Remove(ExceptionResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                                    + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));

                                throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                            {
                                ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                                m_ReceivedMessages.Remove(ServiceErrorResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                                    + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());

                                throw new DLMSResponseException("Confirmed Service Error APDU received.", ServiceErrorResponse);
                            }
                        }

                        if (CurrentResponse == null)
                        {
                            Thread.Sleep(1);
                        }
                    }

                    if (CurrentResponse != null)
                    {
                        if (CurrentResponse.ResponseType == SetResponseAPDU.ResponseTypes.Normal)
                        {
                            // This is a failure message
                            SetComplete = true;
                            Response = CurrentResponse;
                        }
                        else if (CurrentResponse.ResponseType == SetResponseAPDU.ResponseTypes.LastDataBlock)
                        {
                            // This is the response to the last block
                            SetComplete = true;
                            Response = CurrentResponse;
                        }
                        else if (CurrentResponse.ResponseType == SetResponseAPDU.ResponseTypes.DataBlock)
                        {
                            // We received a response to the current block set
                            if (((SetResponseDataBlock)CurrentResponse.Response).BlockNumber == CurrentIndex + 1) // The block number is 1 based whereas the index is 0 based
                            {
                                CurrentIndex++;
                            }
                            else
                            {
                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Wrong block number received");
                                throw new ArgumentException("Wrong block number received");
                            }
                        }
                        else
                        {
                            // Unexpected response
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Block Data Set Failed - Unexpected Response Type received.");
                            throw new ArgumentException("Unexpected Set Response Type.");
                        }
                    }

                } while (SetComplete == false);
            }
            else
            {
                throw new ArgumentNullException("setRequest", "The set request may not be null and must be a valid set value");
            }

            return Response;
        }

        /// <summary>
        /// Gets the Set Request APDU for the specified block
        /// </summary>
        /// <param name="index">The index of the block</param>
        /// <param name="setRequest">The initial set request</param>
        /// <returns>The block request</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/13 RCG 2.80.39 N/A    Created

        private SetRequestAPDU GetDataBlockRequest(uint index, SetRequestNormal setRequest)
        {
            SetRequestAPDU NewRequest = null;

            if (setRequest != null && setRequest.Value != null)
            {
                byte[] ValueData = setRequest.Value.Data;
                byte[] BlockData = null;

                SetRequestAPDU FirstBlockAPDU = CreateSetFirstBlockAPDU(setRequest);
                SetRequestAPDU NormalBlockAPDU = CreateSetNextBlockAPDU(setRequest);

                DataBlock CurrentDataBlock = null;

                // Determine the maximum length of the typical blocks
                int FirstBlockLength = MaxSetAPDUSize - FirstBlockAPDU.Data.Length;
                int NormalBlockLength = MaxSetAPDUSize - NormalBlockAPDU.Data.Length;

                // Account for the length bytes in the data
                if (FirstBlockLength >= 128)
                {
                    FirstBlockLength -= (int)(Math.Ceiling(Math.Log(FirstBlockLength) / Math.Log(2)) / 8) + 1;
                }

                if (NormalBlockLength >= 128)
                {
                    NormalBlockLength -= (int)(Math.Ceiling(Math.Log(FirstBlockLength) / Math.Log(2)) / 8) + 1;
                }

                int StartIndex = 0;
                int CurrentBlockLength = 0;

                // Determine the location of the data
                if (index == 0)
                {
                    // We are creating the first block
                    StartIndex = 0;
                    CurrentBlockLength = FirstBlockLength;

                    NewRequest = FirstBlockAPDU;
                    CurrentDataBlock = ((SetRequestWithFirstDataBlock)FirstBlockAPDU.Request).DataBlock;
                }
                else
                {
                    StartIndex = (int)(FirstBlockLength + (NormalBlockLength * (index - 1)));
                    CurrentBlockLength = NormalBlockLength;

                    NewRequest = NormalBlockAPDU;
                    CurrentDataBlock = ((SetRequestWithDataBlock)NormalBlockAPDU.Request).DataBlock;
                }

                // Determine if this is the last block
                if (StartIndex + CurrentBlockLength > ValueData.Length)
                {
                    CurrentBlockLength = (int)(ValueData.Length - StartIndex);
                    CurrentDataBlock.LastBlock = true;
                }

                // We should now have enough info to get the block data
                BlockData = new byte[CurrentBlockLength];
                Array.Copy(ValueData, StartIndex, BlockData, 0, CurrentBlockLength);

                CurrentDataBlock.BlockData = BlockData;
                CurrentDataBlock.BlockNumber = index + 1;
            }
            else
            {
                throw new ArgumentNullException("setRequest", "The request data may not be null");
            }

            return NewRequest;
        }

        /// <summary>
        /// Creates a Set Request for the First Data Block based on the specified request without the block data
        /// </summary>
        /// <param name="setRequest">The set request to create from</param>
        /// <returns>The new APDU</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/13 RCG 2.80.39 N/A    Created

        private SetRequestAPDU CreateSetFirstBlockAPDU(SetRequestNormal setRequest)
        {
            SetRequestAPDU NewAPDU = new SetRequestAPDU();
            SetRequestWithFirstDataBlock NewRequest = new SetRequestWithFirstDataBlock();
            DataBlock NewDataBlock = new DataBlock();

            NewRequest.InvokeID = setRequest.InvokeID;
            NewRequest.Priority = setRequest.Priority;
            NewRequest.ServiceClass = setRequest.ServiceClass;
            NewRequest.AccessSelection = setRequest.AccessSelection;
            NewRequest.AttributeDescriptor = setRequest.AttributeDescriptor;

            NewDataBlock.BlockNumber = 0;
            NewDataBlock.LastBlock = false;
            NewDataBlock.BlockData = new byte[0];

            NewRequest.DataBlock = NewDataBlock;

            NewAPDU.RequestType = SetRequestAPDU.RequestTypes.WithFirstDataBlock;
            NewAPDU.Request = NewRequest;

            return NewAPDU;
        }

        /// <summary>
        /// Creates a Set Request for a Data Block based on the specified request without the block data
        /// </summary>
        /// <param name="setRequest">The set request to create from</param>
        /// <returns>The new APDU</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/13 RCG 2.80.39 N/A    Created

        private SetRequestAPDU CreateSetNextBlockAPDU(SetRequestNormal setRequest)
        {
            SetRequestAPDU NewAPDU = new SetRequestAPDU();
            SetRequestWithDataBlock NewRequest = new SetRequestWithDataBlock();
            DataBlock NewDataBlock = new DataBlock();

            NewRequest.InvokeID = setRequest.InvokeID;
            NewRequest.Priority = setRequest.Priority;
            NewRequest.ServiceClass = setRequest.ServiceClass;

            NewDataBlock.BlockNumber = 0;
            NewDataBlock.LastBlock = false;
            NewDataBlock.BlockData = new byte[0];

            NewRequest.DataBlock = NewDataBlock;

            NewAPDU.RequestType = SetRequestAPDU.RequestTypes.WithDataBlock;
            NewAPDU.Request = NewRequest;

            return NewAPDU;
        }

        /// <summary>
        /// Handles an Action Response that contains block data
        /// </summary>
        /// <param name="initialResponse">The initial Action Response</param>
        /// <returns>The final action response with all data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/13 RCG 2.80.39 N/A    Created

        private ActionResponseWithOptionalData HandleActionBlockResponse(ActionResponseWithBlock initialResponse)
        {
            MemoryStream DataBlockStream = new MemoryStream();
            DLMSBinaryWriter DataBlockWriter = new DLMSBinaryWriter(DataBlockStream);
            ActionResponseWithOptionalData FinalResult = null;
            DataBlock CurrentBlock = initialResponse.Block;

            if (initialResponse != null)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Action block response received. Retrieving remaining blocks.");

                while (CurrentBlock != null && CurrentBlock.LastBlock == false)
                {
                    DataBlockWriter.Write(CurrentBlock.BlockData);

                    // Get the next block
                    CurrentBlock = GetNextActionBlock(CurrentBlock.BlockNumber, initialResponse.InvokeID);
                }

                if (CurrentBlock != null && CurrentBlock.LastBlock == true)
                {
                    // Write the last block's data to the stream
                    DataBlockWriter.Write(CurrentBlock.BlockData);

                    // We have all of the data so now we need to create our final result object
                    FinalResult = new ActionResponseWithOptionalData();

                    FinalResult.Result = ActionResults.Success;
                    FinalResult.ReturnParameters = new GetDataResult();
                    FinalResult.ReturnParameters.GetDataResultType = GetDataResultChoices.Data;

                    DataBlockStream.Seek(0, SeekOrigin.Begin);

                    FinalResult.ReturnParameters.DataValue = new COSEMData();
                    FinalResult.ReturnParameters.DataValue.Parse(DataBlockStream);
                }
            }
            else
            {
                throw new ArgumentNullException("initialResponse", "The initialResponse parameter may not be null");
            }

            return FinalResult;
        }

        /// <summary>
        /// Gets the next Action Data Block
        /// </summary>
        /// <param name="blockNumber">The current block number</param>
        /// <param name="invokeID">The invoke ID</param>
        /// <returns>The next data block</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/03/13 RCG 2.80.39 N/A    Created

        private DataBlock GetNextActionBlock(uint blockNumber, byte invokeID)
        {
            DataBlock Result = null;
            DateTime StartTime;

            ActionRequestAPDU NextBlockAPDU = new ActionRequestAPDU();
            ActionRequestNextBlock NextRequest = new ActionRequestNextBlock();
            ActionResponseAPDU NextResponse = null;

            NextRequest.BlockNumber = blockNumber;
            NextRequest.InvokeID = invokeID;
            NextRequest.Priority = Priorities.Normal;
            NextRequest.ServiceClass = ServiceClasses.Confirmed;

            NextBlockAPDU.RequestType = ActionRequestAPDU.RequestTypes.NextBlock;
            NextBlockAPDU.Request = NextRequest;

            SendAPDU(NextBlockAPDU);

            // Wait for the response
            StartTime = DateTime.Now;

            while (NextResponse == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
            {
                lock (m_ReceivedMessages)
                {
                    if (m_ReceivedMessages.Where(m => m is ActionResponseAPDU && ((ActionResponseAPDU)m).Response.InvokeID == invokeID).Count() > 0)
                    {
                        // We received the response
                        NextResponse = m_ReceivedMessages.Where(m => m is ActionResponseAPDU && ((ActionResponseAPDU)m).Response.InvokeID == invokeID).First() as ActionResponseAPDU;
                        m_ReceivedMessages.Remove(NextResponse);
                    }
                    else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                    {
                        // We received an exception response
                        ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                        m_ReceivedMessages.Remove(ExceptionResponse);

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                            + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));

                        throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                    }
                    else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                    {
                        ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                        m_ReceivedMessages.Remove(ServiceErrorResponse);

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                            + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());
                    }
                }

                if (NextResponse == null)
                {
                    Thread.Sleep(1);
                }
            }

            if (NextResponse != null)
            {
                if (NextResponse.ResponseType == ActionResponseAPDU.ResponseTypes.WithBlock)
                {
                    ActionResponseWithBlock BlockResponse = NextResponse.Response as ActionResponseWithBlock;
                    Result = BlockResponse.Block;
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Block " + blockNumber.ToString(CultureInfo.InvariantCulture) + " received");
                }
                else
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Action Next Block Failed - Unexpected Response Type received.");
                    throw new ArgumentException("Unexpected Action Response Type.");
                }
            }
            else
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Action Next Block Failed - Action Request has timed out.");

                // Clear the data buffers of bad data that may have been received
                ClearDataBuffers();

                throw new TimeoutException("The Action Next Block Response message was not received in the allowed time.");
            }

            return Result;
        }

        /// <summary>
        /// Sends an action that can't be sent in one APDU by using Blocks
        /// </summary>
        /// <param name="actionRequest">The action request to send</param>
        /// <returns>The result of the Action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/11/13 RCG 2.80.37 N/A    Created

        private ActionResponseAPDU SendActionUsingBlocks(ActionRequestNormal actionRequest)
        {
            ActionResponseAPDU Response = null;

            if (actionRequest != null && actionRequest.Data != null)
            {
                bool ActionRequestComplete = false;
                uint CurrentIndex = 0;

                do
                {
                    ActionRequestAPDU CurrentRequest = GetActionBlockRequest(CurrentIndex, actionRequest);
                    ActionResponseAPDU CurrentResponse = null;

                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Sending Action Block " + (CurrentIndex + 1).ToString(CultureInfo.InvariantCulture));
                    SendAPDU(CurrentRequest);

                    DateTime StartTime = DateTime.Now;

                    while (CurrentResponse == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
                    {
                        lock (m_ReceivedMessages)
                        {
                            if (m_ReceivedMessages.Where(m => m is ActionResponseAPDU && ((ActionResponseAPDU)m).Response.InvokeID == actionRequest.InvokeID).Count() > 0)
                            {
                                // We received the response
                                CurrentResponse = m_ReceivedMessages.Where(m => m is ActionResponseAPDU && ((ActionResponseAPDU)m).Response.InvokeID == actionRequest.InvokeID).First() as ActionResponseAPDU;
                                m_ReceivedMessages.Remove(CurrentResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                            {
                                // We received an exception response
                                ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                                m_ReceivedMessages.Remove(ExceptionResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                                    + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));

                                throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                            }
                            else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                            {
                                ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                                m_ReceivedMessages.Remove(ServiceErrorResponse);

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                                    + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());

                                throw new DLMSResponseException("Confirmed Service Error APDU received.", ServiceErrorResponse);
                            }
                        }

                        if (CurrentResponse == null)
                        {
                            Thread.Sleep(1);
                        }
                    }

                    if (CurrentResponse != null)
                    {
                        if (CurrentResponse.ResponseType == ActionResponseAPDU.ResponseTypes.Normal
                            || CurrentResponse.ResponseType == ActionResponseAPDU.ResponseTypes.WithBlock)
                        {
                            ActionRequestComplete = true;
                            Response = CurrentResponse;
                        }
                        else if (CurrentResponse.ResponseType == ActionResponseAPDU.ResponseTypes.NextBlock)
                        {
                            // We received a response to the current block set
                            if (((ActionResponseNextBlock)CurrentResponse.Response).BlockNumber == CurrentIndex + 1) // The block number is 1 based whereas the index is 0 based
                            {
                                CurrentIndex++;
                            }
                            else
                            {
                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Wrong block number received");
                                throw new ArgumentException("Wrong block number received");
                            }
                        }
                        else
                        {
                            // Unexpected response
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Block Data Action Failed - Unexpected Response Type received.");
                            throw new ArgumentException("Unexpected Set Response Type.");
                        }
                    }

                } while (ActionRequestComplete == false);
            }
            else
            {
                throw new ArgumentNullException("setRequest", "The set request may not be null and must be a valid set value");
            }

            return Response;
        }

        /// <summary>
        /// Gets the Action Request APDU for the specified block
        /// </summary>
        /// <param name="index">The current block index</param>
        /// <param name="actionRequest">The original Action Request</param>
        /// <returns>The Action Request with a block</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/11/13 RCG 2.80.37 N/A    Created

        private ActionRequestAPDU GetActionBlockRequest(uint index, ActionRequestNormal actionRequest)
        {
            ActionRequestAPDU NewRequest = null;

            if (actionRequest != null && actionRequest.Parameters != null)
            {
                byte[] ValueData = actionRequest.Parameters.Data;
                byte[] BlockData = null;

                ActionRequestAPDU FirstBlockAPDU = CreateActionFirstBlockAPDU(actionRequest);
                ActionRequestAPDU NormalBlockAPDU = CreateActionWithBlockAPDU(actionRequest);

                DataBlock CurrentDataBlock = null;

                // Determine the maximum length of the typical blocks
                int FirstBlockLength = MaxSetAPDUSize - FirstBlockAPDU.Data.Length;
                int NormalBlockLength = MaxSetAPDUSize - NormalBlockAPDU.Data.Length;

                // Account for the length bytes in the data
                if (FirstBlockLength >= 128)
                {
                    FirstBlockLength -= (int)(Math.Ceiling(Math.Log(FirstBlockLength) / Math.Log(2)) / 8) + 1;
                }

                if (NormalBlockLength >= 128)
                {
                    NormalBlockLength -= (int)(Math.Ceiling(Math.Log(FirstBlockLength) / Math.Log(2)) / 8) + 1;
                }

                int StartIndex = 0;
                int CurrentBlockLength = 0;

                // Determine the location of the data
                if (index == 0)
                {
                    // We are creating the first block
                    StartIndex = 0;
                    CurrentBlockLength = FirstBlockLength;

                    NewRequest = FirstBlockAPDU;
                    CurrentDataBlock = ((ActionRequestWithFirstBlock)FirstBlockAPDU.Request).Block;
                }
                else
                {
                    StartIndex = (int)(FirstBlockLength + (NormalBlockLength * (index - 1)));
                    CurrentBlockLength = NormalBlockLength;

                    NewRequest = NormalBlockAPDU;
                    CurrentDataBlock = ((ActionRequestWithBlock)NormalBlockAPDU.Request).Block;
                }

                // Determine if this is the last block
                if (StartIndex + CurrentBlockLength > ValueData.Length)
                {
                    CurrentBlockLength = (int)(ValueData.Length - StartIndex);
                    CurrentDataBlock.LastBlock = true;
                }

                // We should now have enough info to get the block data
                BlockData = new byte[CurrentBlockLength];
                Array.Copy(ValueData, StartIndex, BlockData, 0, CurrentBlockLength);

                CurrentDataBlock.BlockData = BlockData;
                CurrentDataBlock.BlockNumber = index + 1;
            }
            else
            {
                throw new ArgumentNullException("actionRequest", "The request data may not be null");
            }

            return NewRequest;
        }

        /// <summary>
        /// Creates an empty first block APDU
        /// </summary>
        /// <param name="actionRequest">The Action Request to create the APDU from</param>
        /// <returns>The empty APDU</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/11/13 RCG 2.80.37 N/A    Created

        private ActionRequestAPDU CreateActionFirstBlockAPDU(ActionRequestNormal actionRequest)
        {
            ActionRequestAPDU NewAPDU = new ActionRequestAPDU();
            ActionRequestWithFirstBlock NewRequest = new ActionRequestWithFirstBlock();
            DataBlock NewDataBlock = new DataBlock();

            NewRequest.InvokeID = actionRequest.InvokeID;
            NewRequest.Priority = actionRequest.Priority;
            NewRequest.ServiceClass = actionRequest.ServiceClass;
            NewRequest.MethodDescriptor = actionRequest.MethodDescriptor;

            NewDataBlock.BlockNumber = 0;
            NewDataBlock.LastBlock = false;
            NewDataBlock.BlockData = new byte[0];

            NewRequest.Block = NewDataBlock;

            NewAPDU.RequestType = ActionRequestAPDU.RequestTypes.WithFirstBlock;
            NewAPDU.Request = NewRequest;

            return NewAPDU;
        }

        /// <summary>
        /// Creates an empty Action block APDU
        /// </summary>
        /// <param name="actionRequest">The Action Request to create the APDU from</param>
        /// <returns>The empty APDU</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/11/13 RCG 2.80.37 N/A    Created

        private ActionRequestAPDU CreateActionWithBlockAPDU(ActionRequestNormal actionRequest)
        {
            ActionRequestAPDU NewAPDU = new ActionRequestAPDU();
            ActionRequestWithBlock NewRequest = new ActionRequestWithBlock();
            DataBlock NewDataBlock = new DataBlock();

            NewRequest.InvokeID = actionRequest.InvokeID;
            NewRequest.Priority = actionRequest.Priority;
            NewRequest.ServiceClass = actionRequest.ServiceClass;

            NewDataBlock.BlockNumber = 0;
            NewDataBlock.LastBlock = false;
            NewDataBlock.BlockData = new byte[0];

            NewRequest.Block = NewDataBlock;

            NewAPDU.RequestType = ActionRequestAPDU.RequestTypes.WithBlock;
            NewAPDU.Request = NewRequest;

            return NewAPDU;
        }

        /// <summary>
        /// Handles the APDU Received event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created

        private void m_Comm_APDUReceived(object sender, APDUEventArguments e)
        {
            if (e != null && e.APDU != null)
            {
                // No need to handle Ciphered APDUs here because they should
                // be handled at the Comm Level.

                lock (m_ReceivedMessages)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Message Received - " + e.APDU.Tag.ToDescription());
                    m_ReceivedMessages.Add(e.APDU);
                }
            }
        }

        /// <summary>
        /// Send an APDU to the meter
        /// </summary>
        /// <param name="apdu"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private void SendAPDU(xDLMSAPDU apdu)
        {
            if (m_Comm.IsOpen && apdu != null)
            {
                xDLMSAPDU APDUToSend = apdu;

                // Make sure that any unhandled Exception Responses have been cleared out before sending the Request
                lock (m_ReceivedMessages)
                {
                    m_ReceivedMessages.RemoveAll(m => m is ExceptionResponseAPDU);
                }

                // If using HLS and there is an equivalent dedicated key APDU then Cipher the APDU
                if ((m_SecurityType == DLMSSecurityType.HighLevelSecurity || m_SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC))
                {
                    CipheredAPDU Ciphered = null;

                    if (CurrentEncryptionMode == DLMSEncryptionModes.Dedicated && apdu.Tag != CipheredAPDU.GetEquivalentDedicatedCipherTag(apdu.Tag))
                    {
                        Ciphered = CreateNewCipheredAPDUForSend(CipheredAPDU.GetEquivalentDedicatedCipherTag(apdu.Tag), DedicatedEncryptionKey);
                    }
                    else if (CurrentEncryptionMode == DLMSEncryptionModes.Global && apdu.Tag != CipheredAPDU.GetEquivalentGlobalCipherTag(apdu.Tag))
                    {
                        Ciphered = CreateNewCipheredAPDUForSend(CipheredAPDU.GetEquivalentGlobalCipherTag(apdu.Tag), GlobalEncryptionKey);
                    }

                    if (Ciphered != null)
                    {
                        Ciphered.UncipheredAPDU = apdu;
                        APDUToSend = Ciphered;
                    }
                }

                m_Comm.SendAPDU(APDUToSend);
            }
        }

        /// <summary>
        /// Creates a new AARQ APDU
        /// </summary>
        /// <returns>The AARQ APDU created</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private AARQAPDU CreateAARQ()
        {
            AARQAPDU NewAARQ = new AARQAPDU();
            xDLMSAPDU InitiateRequest = CreateInitiateRequest();

            NewAARQ.ProtocolVersion = ProtocolVersions.Version1;

            if (m_SecurityType == DLMSSecurityType.HighLevelSecurity || m_SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC)
            {
                NewAARQ.ApplicationContextName = AARQAPDU.LogicalNameWithCipherApplicationOID;
            }
            else
            {
                NewAARQ.ApplicationContextName = AARQAPDU.LogicalNameWithNoCipherApplicationOID;
            }

            NewAARQ.CallingApTitle = m_ClientApTitle;

            if (m_SecurityType == DLMSSecurityType.None)
            {
                NewAARQ.SenderACSERequirements = ACSERequirements.None;
                NewAARQ.MechanismName = null;
                NewAARQ.CallingAuthenticationValue = null;
            }
            else if (m_SecurityType == DLMSSecurityType.LowLevelSecurity)
            {
                NewAARQ.SenderACSERequirements = ACSERequirements.Authentication;
                NewAARQ.MechanismName = AARQAPDU.LowLevelSecurityMechanismName;
                NewAARQ.CallingAuthenticationValue = AuthenticationKey; // Should be set to the password
            }
            else if (m_SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC)
            {
                NewAARQ.SenderACSERequirements = ACSERequirements.Authentication;
                NewAARQ.MechanismName = AARQAPDU.HighLevelSecurityMechanismName;
                NewAARQ.CallingAuthenticationValue = m_ClientChallenge;
            }

            // The user information is the Initiate Request APDU
            NewAARQ.UserInformation = InitiateRequest.Data;

            return NewAARQ;
        }

        /// <summary>
        /// Creates a RLRQ message
        /// </summary>
        /// <returns>The new RLRQ message</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private RLRQAPDU CreateRLRQ()
        {
            RLRQAPDU NewRLRQ = new RLRQAPDU();
            xDLMSAPDU InitiateRequest = CreateInitiateRequest();

            NewRLRQ.Reason = new byte[] { 0 }; // Reason is Normal

            // Only add the user information if using HLS
            if (SecurityType == DLMSSecurityType.HighLevelSecurity || SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC)
            {
                NewRLRQ.UserInformation = InitiateRequest.Data;
            }

            return NewRLRQ;
        }

        /// <summary>
        /// Creates an initiate request APDU
        /// </summary>
        /// <returns>The new Initiate Request</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private xDLMSAPDU CreateInitiateRequest()
        {
            CipheredAPDU CipheredRequest = null;
            InitiateRequestAPDU UncipheredRequest = new InitiateRequestAPDU();
            xDLMSAPDU InitiateRequestToSend = null;


            if (m_SecurityType == DLMSSecurityType.HighLevelSecurity || m_SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC)
            {
                UncipheredRequest.DedicatedKey = DedicatedEncryptionKey;
            }
            else
            {
                UncipheredRequest.DedicatedKey = null;
            }

            UncipheredRequest.ResponseAllowed = true;
            UncipheredRequest.ProposedQualityOfService = null;
            UncipheredRequest.ProposedDLMSVersion = 6;
            UncipheredRequest.Conformance = new DLMSConformance();
            UncipheredRequest.Conformance.Conformance = DLMSConformanceFlags.Default;
            UncipheredRequest.ClientMaxReceivePDUSize = m_Comm.MaxAPDUSize;

            if (m_SecurityType == DLMSSecurityType.HighLevelSecurity || m_SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC)
            {
                CipheredRequest = CreateNewCipheredAPDUForSend(xDLMSTags.InitiateRequestGlobalCipher, GlobalEncryptionKey);
                CipheredRequest.UncipheredAPDU = UncipheredRequest;

                InitiateRequestToSend = CipheredRequest;
            }
            else
            {
                InitiateRequestToSend = UncipheredRequest;
            }

            return InitiateRequestToSend;
        }

        /// <summary>
        /// Clears all data buffers of it's current data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/16/13 RCG 2.80.31 N/A    Created
        
        private void ClearDataBuffers()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Clearing internal data buffers");

            lock (m_ReceivedMessages)
            {
                m_ReceivedMessages.Clear();
            }

            m_Comm.ClearBuffers();
        }

        /// <summary>
        /// Handles a Get Data Block Response
        /// </summary>
        /// <param name="initialResponse">The initial Data Block Response</param>
        /// <returns>The full response from the meter</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/31/13 RCG 2.80.36 N/A    Created

        private GetDataResult HandleGetBlockResponse(GetResponseWithDatablock initialResponse)
        {
            MemoryStream DataBlockStream = new MemoryStream();
            DLMSBinaryWriter DataBlockWriter = new DLMSBinaryWriter(DataBlockStream);
            GetDataResult FinalResult = null;
            DataBlockResponse CurrentResponse = initialResponse.Result;

            if (initialResponse != null)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Data block response received. Retrieving remaining blocks.");

                while (CurrentResponse != null && CurrentResponse.LastBlock == false)
                {
                    if (CurrentResponse.ResponseType == GetDataResultChoices.Data)
                    {
                        DataBlockWriter.Write(CurrentResponse.BlockData);

                        // Get the next block
                        CurrentResponse = GetNextBlock(CurrentResponse.BlockNumber, initialResponse.InvokeID);
                    }
                    else
                    {
                        // We received some sort of error
                        FinalResult = new GetDataResult();

                        FinalResult.GetDataResultType = GetDataResultChoices.DataAccessResult;
                        FinalResult.DataAccessResult = CurrentResponse.AccessResult;

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Failed to get a data block. Reason: " + EnumDescriptionRetriever.RetrieveDescription(FinalResult.DataAccessResult));
                        break;
                    }
                }

                if (CurrentResponse != null && CurrentResponse.LastBlock == true)
                {
                    // Write the last block's data to the stream
                    DataBlockWriter.Write(CurrentResponse.BlockData);

                    // We have all of the data so now we need to create our final result object
                    FinalResult = new GetDataResult();

                    FinalResult.GetDataResultType = GetDataResultChoices.Data;

                    DataBlockStream.Seek(0, SeekOrigin.Begin);

                    FinalResult.DataValue = new COSEMData();
                    FinalResult.DataValue.Parse(DataBlockStream);
                }
            }
            else
            {
                throw new ArgumentNullException("initialResponse", "The initialResponse parameter may not be null");
            }

            return FinalResult;
        }

        /// <summary>
        /// Gets the Next Block
        /// </summary>
        /// <param name="blockNumber">The next block to get</param>
        /// <param name="invokeID">The Invoke ID to use</param>
        /// <returns>The Next Data Block</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/31/13 RCG 2.80.36 N/A    Created

        private DataBlockResponse GetNextBlock(uint blockNumber, byte invokeID)
        {
            DataBlockResponse Result = null;
            DateTime StartTime;

            GetRequestAPDU GetNextBlockAPDU = new GetRequestAPDU();
            GetRequestNext GetNextRequest = new GetRequestNext();
            GetResponseAPDU GetNextResponse = null;

            GetNextRequest.BlockNumber = blockNumber;
            GetNextRequest.InvokeID = invokeID;
            GetNextRequest.Priority = Priorities.Normal;
            GetNextRequest.ServiceClass = ServiceClasses.Confirmed;

            GetNextBlockAPDU.RequestType = GetRequestAPDU.GetRequestChoice.Next;
            GetNextBlockAPDU.Request = GetNextRequest;

            SendAPDU(GetNextBlockAPDU);

            // Wait for the response
            StartTime = DateTime.Now;

            while (GetNextResponse == null && DateTime.Now - StartTime < RESPONSE_TIMEOUT)
            {
                lock (m_ReceivedMessages)
                {
                    if (m_ReceivedMessages.Where(m => m is GetResponseAPDU && ((GetResponseAPDU)m).Response.InvokeID == invokeID).Count() > 0)
                    {
                        // We received the response
                        GetNextResponse = m_ReceivedMessages.Where(m => m is GetResponseAPDU && ((GetResponseAPDU)m).Response.InvokeID == invokeID).First() as GetResponseAPDU;
                        m_ReceivedMessages.Remove(GetNextResponse);
                    }
                    else if (m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).Count() > 0)
                    {
                        // We received an exception response
                        ExceptionResponseAPDU ExceptionResponse = m_ReceivedMessages.Where(m => m is ExceptionResponseAPDU).First() as ExceptionResponseAPDU;
                        m_ReceivedMessages.Remove(ExceptionResponse);

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Exception Response received. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.ServiceError)
                            + " State Error: " + EnumDescriptionRetriever.RetrieveDescription(ExceptionResponse.StateError));

                        throw new DLMSResponseException("Exception Response APDU received.", ExceptionResponse);
                    }
                    else if (m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).Count() > 0)
                    {
                        ConfirmedServiceErrorAPDU ServiceErrorResponse = m_ReceivedMessages.Where(m => m is ConfirmedServiceErrorAPDU).First() as ConfirmedServiceErrorAPDU;
                        m_ReceivedMessages.Remove(ServiceErrorResponse);

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Confirmed Service Error Received. Service Error: " + ServiceErrorResponse.ServiceError.ToDescription()
                            + " Service Error Choice: " + ServiceErrorResponse.ServiceErrorChoice.ToDescription());
                    }
                }

                if (GetNextResponse == null)
                {
                    Thread.Sleep(1);
                }
            }

            if (GetNextResponse != null)
            {
                if (GetNextResponse.ResponseType == GetResponseAPDU.GetResponseChoices.WithDataBlock)
                {
                    GetResponseWithDatablock BlockResponse = GetNextResponse.Response as GetResponseWithDatablock;
                    Result = BlockResponse.Result as DataBlockResponse;
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Block " + blockNumber.ToString(CultureInfo.InvariantCulture) + " received");
                }
                else
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Get Next Failed - Unexpected Response Type received.");
                    throw new ArgumentException("Unexpected Get Response Type.");
                }
            }
            else
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Get Next Failed - Get Request has timed out.");

                // Clear the data buffers of bad data that may have been received
                ClearDataBuffers();

                throw new TimeoutException("The Get Next Response message was not received in the allowed time.");
            }

            return Result;
        }

        /// <summary>
        /// Generates a random key for use with AES
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private void GenerateDedicatedKey()
        {
            AesManaged AES = new AesManaged();

            AES.KeySize = 128;
            AES.GenerateKey();

            DedicatedEncryptionKey = AES.Key;
        }

        /// <summary>
        /// Generates an ApTitle for this instance
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private void GenerateClientApTitle()
        {
            Process CurrentProcess = Process.GetCurrentProcess();
            string InstanceID = Environment.UserName + CurrentProcess.MachineName + CurrentProcess.Id.ToString(CultureInfo.InvariantCulture);
            byte[] InstanceHash = BitConverter.GetBytes(InstanceID.GetHashCode());

            m_ClientApTitle = new byte[8];

            m_ClientApTitle[0] = (byte)'I';
            m_ClientApTitle[1] = (byte)'T';
            m_ClientApTitle[2] = (byte)'R';
            m_ClientApTitle[3] = 0xAA;

            Array.Copy(InstanceHash, 0, m_ClientApTitle, 4, 4);
        }

        /// <summary>
        /// Generates the Client Challenge
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private void GenerateClientChallenge()
        {
            RandomNumberGenerator NumberGenerator = RandomNumberGenerator.Create();
            m_ClientChallenge = new byte[CHALLENGE_LENGTH];

            NumberGenerator.GetBytes(m_ClientChallenge);
        }

        /// <summary>
        /// Creates a New Ciphered APDU
        /// </summary>
        /// <param name="tag">The APDU tag to use</param>
        /// <param name="encryptionKey">The encryption key to use</param>
        /// <returns>The new Ciphered APDU</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private CipheredAPDU CreateNewCipheredAPDUForSend(xDLMSTags tag, byte[] encryptionKey)
        {
            CipheredAPDU NewAPDU = new CipheredAPDU(tag);

            NewAPDU.BlockCipherKey = encryptionKey;
            NewAPDU.AuthenticationKey = AuthenticationKey;
            NewAPDU.ApTitle = m_ClientApTitle;
            NewAPDU.FrameCounter = m_FrameCounter;
            NewAPDU.Broadcast = false;
            NewAPDU.SecuritySuite = DLMSSecuritySuites.AES128;

            switch (m_SecurityPolicy)
            {
                case DLMSSecurityPolicy.EncryptMessages:
                {
                    NewAPDU.Authenticated = false;
                    NewAPDU.Encrypted = true;
                    break;
                }
                case DLMSSecurityPolicy.AuthenticateMessages:
                {
                    NewAPDU.Authenticated = true;
                    NewAPDU.Encrypted = false;
                    break;
                }
                case DLMSSecurityPolicy.AuthenticateAndEncryptMessages:
                {
                    NewAPDU.Authenticated = true;
                    NewAPDU.Encrypted = true;
                    break;
                }
            }

            m_FrameCounter++;

            return NewAPDU;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the device is currently connected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool IsConnected
        {
            get
            {
                return m_Connected;
            }
        }

        /// <summary>
        /// Gets or sets the Client Port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort ClientPort
        {
            get
            {
                return m_Comm.ClientPort;
            }
            set
            {
                m_Comm.ClientPort = value;
            }
        }

        /// <summary>
        /// Gets or sets the Server Port
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort ServerPort
        {
            get
            {
                return m_Comm.ServerPort;
            }
            set
            {
                m_Comm.ServerPort = value;
            }
        }

        /// <summary>
        /// Gets or sets the Global Encryption Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] GlobalEncryptionKey
        {
            get
            {
                return m_Comm.GlobalEncryptionKey;
            }
            set
            {
                m_Comm.GlobalEncryptionKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the Authentication Key used for Encryption
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] AuthenticationKey
        {
            get
            {
                return m_EncryptAuthenticationKey;
            }
            set
            {
                if (value != null && value.Length == 0)
                {
                    m_UsingChallengAsAuthenticationKey = true;

                    m_EncryptAuthenticationKey = m_ClientChallenge;
                    DecryptAuthenticationKey = m_ServerChallenge;
                }
                else
                {
                    m_UsingChallengAsAuthenticationKey = false;

                    m_EncryptAuthenticationKey = value;
                    DecryptAuthenticationKey = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Authentication Key used for Decryption
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/13/13 RCG 2.80.43 N/A    Created

        public byte[] DecryptAuthenticationKey
        {
            get
            {
                return m_Comm.DecryptAuthenticationKey;
            }
            private set
            {
                m_Comm.DecryptAuthenticationKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the Pending Authentication Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/16/13 RCG 2.85.20 N/A    Created

        public byte[] PendingDecryptAuthenticationKey
        {
            get
            {
                return m_Comm.PendingDecryptAuthenticationKey;
            }
            set
            {
                m_Comm.PendingDecryptAuthenticationKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the security type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public DLMSSecurityType SecurityType
        {
            get
            {
                return m_SecurityType;
            }
            set
            {
                m_SecurityType = value;
            }
        }

        /// <summary>
        /// Gets or sets the Security Policy
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public DLMSSecurityPolicy SecurityPolicy
        {
            get
            {
                return m_SecurityPolicy;
            }
            set
            {
                m_SecurityPolicy = value;
            }
        }

        /// <summary>
        /// Gets or sets the current Encryption Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/08/13 RCG 3.50.01 N/A    Created
        
        public DLMSEncryptionModes CurrentEncryptionMode
        {
            get
            {
                return m_EncryptionMode;
            }
            set
            {
                m_EncryptionMode = value;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets the Current Invoke ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        private byte CurrentInvokeID
        {
            get
            {
                return m_CurrentInvokeID;
            }
            set
            {
                // This value can only be 0 - 15 but lets assign it the value mod 16 so that we can increment the value and have it roll
                // over like a normal byte would do when reaching the max value
                m_CurrentInvokeID = (byte)(value % 16);
            }
        }

        /// <summary>
        /// Gets or sets the Server ApTitle
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private byte[] ServerApTitle
        {
            get
            {
                return m_Comm.ServerApTitle;
            }
            set
            {
                m_Comm.ServerApTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets the Dedicated Encryption Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private byte[] DedicatedEncryptionKey
        {
            get
            {
                return m_Comm.DedicatedEncryptionKey;
            }
            set
            {
                m_Comm.DedicatedEncryptionKey = value;
            }
        }

        /// <summary>
        /// Gets the maximum APDU Size that can be used in a set
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private int MaxSetAPDUSize
        {
            get
            {
                int APDUSize = m_ServerMaxAPDUSize;

                if (SecurityType == DLMSSecurityType.HighLevelSecurity || SecurityType == DLMSSecurityType.HighLevelSecurityWithGMAC)
                {
                    APDUSize -= CipheredAPDU.CalculateAPDUOverhead(SecurityPolicy, APDUSize);
                }

                return APDUSize;
            }
        }

        #endregion

        #region Member Variables

        private IDLMSComm m_Comm;
        private Logger m_Logger;
        private List<xDLMSAPDU> m_ReceivedMessages;
        private APDUEventHandler m_APDUReceivedHandler;
        private bool m_Connected;
        private ushort m_ServerMaxAPDUSize;
        private DLMSConformance m_NegotiatedConformance;
        private byte m_CurrentInvokeID;

        private DLMSSecurityType m_SecurityType;
        private DLMSSecurityPolicy m_SecurityPolicy;
        private DLMSEncryptionModes m_EncryptionMode;

        private byte[] m_ClientApTitle;
        private byte[] m_ClientChallenge;
        private byte[] m_ServerChallenge;
        private byte[] m_EncryptAuthenticationKey;
        private uint m_FrameCounter;
        private bool m_UsingChallengAsAuthenticationKey;

        #endregion
    }
}
