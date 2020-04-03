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
using System.Security.Cryptography;
using OpenSSLWrapper;
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// Supported Security Suites
    /// </summary>
    public enum DLMSSecuritySuites : byte
    {
        /// <summary>AES-GCM-128 for authentication and AES-128 for key wrapping</summary>
        [EnumDescription("AES-128")]
        AES128 = 0,
    }

    /// <summary>
    /// A Ciphered APDU
    /// </summary>
    public class CipheredAPDU : xDLMSAPDU
    {
        #region Constants

        private const byte SECURITY_SUITE_MASK = 0x0F;
        private const byte AUTHENTICATED_MASK = 0x10;
        private const byte ENCRYPTED_MASK = 0x20;
        private const byte KEY_SET_MASK = 0x40;
        private const int SECURITY_HEADER_LENGTH = 5;
        private const int AUTHENTICATION_TAG_LENGTH = 12;
        private const int BASE_OVERHEAD = 7;

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines the amount of overhead caused by the use of a Ciphered APDU
        /// </summary>
        /// <param name="policy">The current security policy</param>
        /// <param name="maxLength">The maximum length of the payload</param>
        /// <returns>The number of bytes of overhead</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public static int CalculateAPDUOverhead(DLMSSecurityPolicy policy, int maxLength)
        {
            int Overhead = BASE_OVERHEAD;

            // Add the maximum data length
            Overhead += (int)(Math.Ceiling(Math.Log(maxLength) / Math.Log(2)) / 8) + 1;

            if (policy == DLMSSecurityPolicy.AuthenticateAndEncryptMessages || policy == DLMSSecurityPolicy.AuthenticateMessages)
            {
                Overhead += AUTHENTICATION_TAG_LENGTH;
            }

            return Overhead;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tag">The Cipher tag</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public CipheredAPDU(xDLMSTags tag)
        {
            m_Tag = tag;
            m_UncipheredAPDU = null;
            m_FrameCounter = 0;
            m_SecurityControl = 0;

            m_BlockCipherKey = null;
            m_AuthenticationKey = null;
            m_ApTitle = null;
        }

        /// <summary>
        /// Parses the APDU from the stream
        /// </summary>
        /// <param name="apduStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created

        public override void Parse(Stream apduStream)
        {
            if (m_BlockCipherKey == null || m_AuthenticationKey == null || m_ApTitle == null)
            {
                throw new InvalidOperationException("All encryption data must be set prior to parsing a Ciphered APDU");
            }

            base.Parse(apduStream);

            // The APDU is written as on Octet String with a length so lets read that out first
            DLMSBinaryReader APDUReader = new DLMSBinaryReader(apduStream);
            byte[] Data = APDUReader.ReadBytesWithLength();

            // Now we can parse out the Data
            MemoryStream DataStream = new MemoryStream(Data);
            DLMSBinaryReader DataReader = new DLMSBinaryReader(DataStream);
            byte[] CipheredAPDU = null;
            byte[] UncipheredAPDU = null;
            byte[] Tag = null;

            m_SecurityControl = DataReader.ReadByte();
            m_FrameCounter = DataReader.ReadUInt32();

            if (Authenticated)
            {
                CipheredAPDU = DataReader.ReadBytes(Data.Length - SECURITY_HEADER_LENGTH - AUTHENTICATION_TAG_LENGTH);
                Tag = DataReader.ReadBytes(AUTHENTICATION_TAG_LENGTH);
            }
            else
            {
                CipheredAPDU = DataReader.ReadBytes(Data.Length - SECURITY_HEADER_LENGTH);
            }

            UncipheredAPDU = UncipherData(CipheredAPDU, Tag);

            if (UncipheredAPDU != null && UncipheredAPDU.Length > 0)
            {
                m_UncipheredAPDU = xDLMSAPDU.Create((xDLMSTags)UncipheredAPDU[0]);

                m_UncipheredAPDU.Parse(new MemoryStream(UncipheredAPDU));
            }
            else
            {
                throw new InvalidDataException("The APDU could not be unciphered");
            }
        }

        /// <summary>
        /// Gets the equivalent global cipher tag
        /// </summary>
        /// <param name="tag">That tag to get the equivalent of</param>
        /// <returns>The equivalent global cipher tag</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public static xDLMSTags GetEquivalentGlobalCipherTag(xDLMSTags tag)
        {
            xDLMSTags EquivalentTag = tag;

            switch(tag)
            {
                case xDLMSTags.InitiateRequest:
                {
                    EquivalentTag = xDLMSTags.InitiateRequestGlobalCipher;
                    break;
                }
                case xDLMSTags.InitiateResponse:
                {
                    EquivalentTag = xDLMSTags.InitiateResponseGlobalCipher;
                    break;
                }
                case xDLMSTags.ConfirmedServiceError:
                {
                    EquivalentTag = xDLMSTags.ConfirmedServiceErrorGlobalCipher;
                    break;
                }
                case xDLMSTags.GetRequest:
                {
                    EquivalentTag = xDLMSTags.GetRequestGlobalCipher;
                    break;
                }
                case xDLMSTags.GetResponse:
                {
                    EquivalentTag = xDLMSTags.GetResponseGlobalCipher;
                    break;
                }
                case xDLMSTags.SetRequest:
                {
                    EquivalentTag = xDLMSTags.SetRequestGlobalCipher;
                    break;
                }
                case xDLMSTags.SetResponse:
                {
                    EquivalentTag = xDLMSTags.SetResponseGlobalCipher;
                    break;
                }
                case xDLMSTags.ActionRequest:
                {
                    EquivalentTag = xDLMSTags.ActionRequestGlobalCipher;
                    break;
                }
                case xDLMSTags.ActionResponse:
                {
                    EquivalentTag = xDLMSTags.ActionResponseGlobalCipher;
                    break;
                }
            }

            return EquivalentTag;
        }

        /// <summary>
        /// Gets the equivalent dedicated cipher tag
        /// </summary>
        /// <param name="tag">That tag to get the equivalent of</param>
        /// <returns>The equivalent dedicated cipher tag</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created

        public static xDLMSTags GetEquivalentDedicatedCipherTag(xDLMSTags tag)
        {
            xDLMSTags EquivalentTag = tag;

            switch (tag)
            {
                case xDLMSTags.ConfirmedServiceError:
                {
                    EquivalentTag = xDLMSTags.ConfirmedServiceErrorDedicatedCiper;
                    break;
                }
                case xDLMSTags.GetRequest:
                {
                    EquivalentTag = xDLMSTags.GetRequestDedicatedCipher;
                    break;
                }
                case xDLMSTags.GetResponse:
                {
                    EquivalentTag = xDLMSTags.GetResponseDedicatedCipher;
                    break;
                }
                case xDLMSTags.SetRequest:
                {
                    EquivalentTag = xDLMSTags.SetRequestDedicatedCipher;
                    break;
                }
                case xDLMSTags.SetResponse:
                {
                    EquivalentTag = xDLMSTags.SetResponseDedicatedCipher;
                    break;
                }
                case xDLMSTags.ActionRequest:
                {
                    EquivalentTag = xDLMSTags.ActionRequestDedicatedCipher;
                    break;
                }
                case xDLMSTags.ActionResponse:
                {
                    EquivalentTag = xDLMSTags.ActionResponseDedicatedCipher;
                    break;
                }
            }

            return EquivalentTag;
        }

        /// <summary>
        /// Gets the equivalent unciphered tag
        /// </summary>
        /// <param name="tag">That tag to get the equivalent of</param>
        /// <returns>The equivalent unciphered tag</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created

        public static xDLMSTags GetEquivalentUncipheredTag(xDLMSTags tag)
        {
            xDLMSTags EquivalentTag = tag;

            switch (tag)
            {
                case xDLMSTags.InitiateRequestGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.InitiateRequest;
                    break;
                }
                case xDLMSTags.InitiateResponseGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.InitiateResponse;
                    break;
                }
                case xDLMSTags.ConfirmedServiceErrorGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.ConfirmedServiceError;
                    break;
                }
                case xDLMSTags.GetRequestDedicatedCipher:
                case xDLMSTags.GetRequestGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.GetRequest;
                    break;
                }
                case xDLMSTags.GetResponseDedicatedCipher:
                case xDLMSTags.GetResponseGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.GetResponse;
                    break;
                }
                case xDLMSTags.SetRequestDedicatedCipher:
                case xDLMSTags.SetRequestGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.SetRequest;
                    break;
                }
                case xDLMSTags.SetResponseDedicatedCipher:
                case xDLMSTags.SetResponseGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.SetResponse;
                    break;
                }
                case xDLMSTags.ActionRequestDedicatedCipher:
                case xDLMSTags.ActionRequestGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.ActionRequest;
                    break;
                }
                case xDLMSTags.ActionResponseDedicatedCipher:
                case xDLMSTags.ActionResponseGlobalCipher:
                {
                    EquivalentTag = xDLMSTags.ActionResponse;
                    break;
                }
            }

            return EquivalentTag;
        }

        /// <summary>
        /// Gets whether or not the tag is a global cipher tag
        /// </summary>
        /// <param name="tag">That tag to check</param>
        /// <returns>True if the tag is a global cipher tag. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created

        public static bool IsTagGlobalCipher(xDLMSTags tag)
        {
            bool IsGlobalCipher = false;

            switch (tag)
            {
                case xDLMSTags.InitiateRequestGlobalCipher:
                case xDLMSTags.InitiateResponseGlobalCipher:
                case xDLMSTags.ConfirmedServiceErrorGlobalCipher:
                case xDLMSTags.GetRequestGlobalCipher:
                case xDLMSTags.GetResponseGlobalCipher:
                case xDLMSTags.SetRequestGlobalCipher:
                case xDLMSTags.SetResponseGlobalCipher:
                case xDLMSTags.ActionRequestGlobalCipher:
                case xDLMSTags.ActionResponseGlobalCipher:
                {
                    IsGlobalCipher = true;
                    break;
                }
            }

            return IsGlobalCipher;
        }

        /// <summary>
        /// Gets whether or not the tag is a dedicated cipher tag
        /// </summary>
        /// <param name="tag">That tag to check</param>
        /// <returns>True if the tag is a dedicated cipher tag. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created

        public static bool IsTagDedicatedCipher(xDLMSTags tag)
        {
            bool IsDedicatedCipher = false;

            switch (tag)
            {
                case xDLMSTags.GetRequestDedicatedCipher:
                case xDLMSTags.GetResponseDedicatedCipher:
                case xDLMSTags.SetRequestDedicatedCipher:
                case xDLMSTags.SetResponseDedicatedCipher:
                case xDLMSTags.ActionRequestDedicatedCipher:
                case xDLMSTags.ActionResponseDedicatedCipher:
                {
                    IsDedicatedCipher = true;
                    break;
                }
            }

            return IsDedicatedCipher;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates the raw data
        /// </summary>
        /// <returns>The Memory Stream containing the Ciphered APDU's raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        protected override MemoryStream GenerateRawData()
        {
            MemoryStream RawDataStream = base.GenerateRawData();
            DLMSBinaryWriter RawDataWriter = new DLMSBinaryWriter(RawDataStream);

            // The APDU is just an octet string so we need to build up the data separately
            MemoryStream APDUStream = new MemoryStream();
            DLMSBinaryWriter APDUWriter = new DLMSBinaryWriter(APDUStream);

            // The Security Header
            APDUWriter.Write(m_SecurityControl);
            APDUWriter.Write(m_FrameCounter);

            // The Ciphered APDU and Tag
            APDUWriter.Write(CipherData());

            // Write the Octet String
            RawDataWriter.WriteBytesWithLength(APDUStream.ToArray());

            return RawDataStream;
        }

        /// <summary>
        /// Ciphers the data
        /// </summary>
        /// <returns>The Ciphered APDU and Tag</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private byte[] CipherData()
        {
            byte[] APDUData = m_UncipheredAPDU.Data;
            byte[] IV = GetIV();
            byte[] AAD = BuildAuthenticatedData(APDUData);
            byte[] CipheredAPDUData = null;
            byte[] Tag = null;

            Aes128GcmCipher GCMCipher = new Aes128GcmCipher(m_BlockCipherKey, IV);

            MemoryStream ResultStream = new MemoryStream();
            BinaryWriter ResultWriter = new BinaryWriter(ResultStream);

            if (Encrypted & Authenticated)
            {
                GCMCipher.Encrypt(APDUData, AAD, out CipheredAPDUData, out Tag);

                ResultWriter.Write(CipheredAPDUData);
                ResultWriter.Write(Tag);
            }
            else if (Encrypted)
            {
                GCMCipher.Encrypt(APDUData, out CipheredAPDUData);

                ResultWriter.Write(CipheredAPDUData);
            }
            else if (Authenticated)
            {
                GCMCipher.Authenticate(AAD, out Tag);

                ResultWriter.Write(APDUData);
                ResultWriter.Write(Tag);
            }
            else
            {
                ResultWriter.Write(APDUData);
            }

            return ResultStream.ToArray();
        }

        /// <summary>
        /// Unciphers the specified data
        /// </summary>
        /// <param name="cipheredAPDU">The Ciphered APDU</param>
        /// <param name="tag">The Authentication tag</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private byte[] UncipherData(byte[] cipheredAPDU, byte[] tag)
        {
            byte[] IV = GetIV();
            byte[] AAD = BuildAuthenticatedData(cipheredAPDU);
            byte[] Result = null;

            Aes128GcmCipher GCMCipher = new Aes128GcmCipher(m_BlockCipherKey, IV);

            if (Encrypted && Authenticated)
            {
                GCMCipher.Decrypt(cipheredAPDU, AAD, tag, out Result);
            }
            else if (Encrypted)
            {
                GCMCipher.Decrypt(cipheredAPDU, out Result);
            }
            else if (Authenticated)
            {
                // Make sure the tags match
                byte[] CalculatedTag = null;

                GCMCipher.Authenticate(AAD, out CalculatedTag);

                if (CalculatedTag != null && tag.Length == CalculatedTag.Length)
                {
                    for (int iIndex = 0; iIndex < tag.Length; iIndex++)
                    {
                        if (tag[iIndex] != CalculatedTag[iIndex])
                        {
                            throw new CryptographicException("Authentication Tag does not match the data");
                        }
                    }
                }
                else
                {
                    throw new CryptographicException("Authentication Tag is not valid");
                }

                Result = cipheredAPDU;
            }
            else
            {
                Result = cipheredAPDU;
            }

            return Result;
        }

        /// <summary>
        /// Builds the authentication data
        /// </summary>
        /// <param name="apdu">The apdu data</param>
        /// <returns>The authentication data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        private byte[] BuildAuthenticatedData(byte[] apdu)
        {
            byte[] Result = null;
            MemoryStream DataStream = new MemoryStream();
            BinaryWriter DataWriter = new BinaryWriter(DataStream);

            if (Authenticated && Encrypted)
            {
                DataWriter.Write(m_SecurityControl);
                DataWriter.Write(m_AuthenticationKey);

                Result = DataStream.ToArray();
            }
            else if (Authenticated)
            {
                // The apdu is not ciphered so it is a part of the Authenticated Data
                DataWriter.Write(m_SecurityControl);
                DataWriter.Write(m_AuthenticationKey);
                DataWriter.Write(apdu);

                Result = DataStream.ToArray();
            }

            return Result;
        }

        /// <summary>
        /// Gets the IV from the specified ApTitle and Frame Counter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created

        private byte[] GetIV()
        {
            MemoryStream IVStream = new MemoryStream();
            DLMSBinaryWriter IVWriter = new DLMSBinaryWriter(IVStream);

            IVWriter.Write(m_ApTitle);
            IVWriter.Write(m_FrameCounter);

            return IVStream.ToArray();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Unciphered APDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public xDLMSAPDU UncipheredAPDU
        {
            get
            {
                return m_UncipheredAPDU;
            }
            set
            {
                if (value == null || GetEquivalentUncipheredTag(m_Tag) == value.Tag)
                {
                    m_UncipheredAPDU = value;
                }
                else
                {
                    throw new ArgumentException("The Ciphered tag does not match the tag of the Unciphered APDU");
                }
            }
        }

        /// <summary>
        /// Gets or set the frame counter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public uint FrameCounter
        {
            get
            {
                return m_FrameCounter;
            }
            set
            {
                m_FrameCounter = value;
            }
        }

        /// <summary>
        /// Gets or sets the security suite ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public DLMSSecuritySuites SecuritySuite
        {
            get
            {
                return (DLMSSecuritySuites)(m_SecurityControl & SECURITY_SUITE_MASK);
            }
            set
            {
                m_SecurityControl = (byte)((m_SecurityControl & ~SECURITY_SUITE_MASK) | ((byte)value & SECURITY_SUITE_MASK));
            }
        }

        /// <summary>
        /// Gets or sets whether or not the APDU should be authenticated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public bool Authenticated
        {
            get
            {
                return (byte)(m_SecurityControl & AUTHENTICATED_MASK) == AUTHENTICATED_MASK;
            }
            set
            {
                if (value)
                {
                    m_SecurityControl = (byte)(m_SecurityControl | AUTHENTICATED_MASK);
                }
                else
                {
                    m_SecurityControl = (byte)(m_SecurityControl & ~AUTHENTICATED_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not the APDU should be encrypted
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public bool Encrypted
        {
            get
            {
                return (byte)(m_SecurityControl & ENCRYPTED_MASK) == ENCRYPTED_MASK;
            }
            set
            {
                if (value)
                {
                    m_SecurityControl = (byte)(m_SecurityControl | ENCRYPTED_MASK);
                }
                else
                {
                    m_SecurityControl = (byte)(m_SecurityControl & ~ENCRYPTED_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the key is Broadcast (true) or Unicast (false)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public bool Broadcast
        {
            get
            {
                return (byte)(m_SecurityControl & KEY_SET_MASK) == KEY_SET_MASK;
            }
            set
            {
                if (value)
                {
                    m_SecurityControl = (byte)(m_SecurityControl | KEY_SET_MASK);
                }
                else
                {
                    m_SecurityControl = (byte)(m_SecurityControl & ~KEY_SET_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Block Cipher Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] BlockCipherKey
        {
            get
            {
                return m_BlockCipherKey;
            }
            set
            {
                if (value != null)
                {
                    m_BlockCipherKey = value;
                }
                else
                {
                    throw new ArgumentNullException("value", "The Block Cipher Key may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Authentication Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] AuthenticationKey
        {
            get
            {
                return m_AuthenticationKey;
            }
            set
            {
                if (value != null)
                {
                    m_AuthenticationKey = value;
                }
                else
                {
                    throw new ArgumentNullException("value", "The Authentication Key may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the ApTitle
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/13/13 RCG 2.80.38 N/A    Created
        
        public byte[] ApTitle
        {
            get
            {
                return m_ApTitle;
            }
            set
            {
                if (value != null)
                {
                    m_ApTitle = value;
                }
                else
                {
                    throw new ArgumentNullException("value", "The ApTitle may not be null");
                }
            }
        }

        #endregion

        #region Member Variables

        private xDLMSAPDU m_UncipheredAPDU;
        private uint m_FrameCounter;
        private byte m_SecurityControl;

        private byte[] m_BlockCipherKey;
        private byte[] m_AuthenticationKey;
        private byte[] m_ApTitle;

        #endregion
    }
}
