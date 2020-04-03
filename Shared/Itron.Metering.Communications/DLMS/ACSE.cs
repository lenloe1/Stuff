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
    /// The ASN.1 Class types
    /// </summary>
    public enum ASN1Class : byte
    {
        /// <summary>Universal</summary>
        Universal = 0x00,
        /// <summary>Application specific</summary>
        Application = 0x40,
        /// <summary>Context Specific</summary>
        ContextSpecific = 0x80,
        /// <summary>Private</summary>
        Private = 0xC0,
    }

    /// <summary>
    /// The ASN.1 data type
    /// </summary>
    public enum ASN1DataType : byte
    {
        /// <summary>Primitive</summary>
        Primitive = 0x00,
        /// <summary>Constructed</summary>
        Constructed = 0x20,
    }

    /// <summary>
    /// Universal Tags
    /// </summary>
    public enum UniversalTags : byte
    {
        /// <summary>End of Content</summary>
        EndOfContent = 0,
        /// <summary>Boolean</summary>
        Boolean = 1,
        /// <summary>Integer</summary>
        Integer = 2,
        /// <summary>Bit String</summary>
        BitString = 3,
        /// <summary>Octet String</summary>
        OctetString = 4,
        /// <summary>Null</summary>
        Null = 5,
        /// <summary>Object Identifier</summary>
        ObjectIdentifier = 6,
        /// <summary>External</summary>
        External = 8,
        /// <summary>Float</summary>
        Float = 9,
        /// <summary>Enumerated</summary>
        Enumerated = 10,
        /// <summary>Embedded PDV</summary>
        EmbeddedPDV = 11,
        /// <summary>UTF8 String</summary>
        UTF8String = 12,
        /// <summary>Relative OID</summary>
        RelativeOID = 13,
        /// <summary>Sequence</summary>
        Sequence = 16,
        /// <summary>Set</summary>
        Set = 17,
        /// <summary>Numeric String</summary>
        NumericString = 18,
        /// <summary>Printable String</summary>
        PrintableString = 19,
        /// <summary>T61 String</summary>
        T61String = 20,
        /// <summary>Videotex String</summary>
        VideotexString = 21,
        /// <summary>IA5 String</summary>
        IA5String = 22,
        /// <summary>UTC Time</summary>
        UTCTime = 23,
        /// <summary>Generalized Time</summary>
        GeneralizedTime = 24,
        /// <summary>Graphic String</summary>
        GraphicString = 25,
        /// <summary>Visible String</summary>
        VisibleString = 26,
        /// <summary>General String</summary>
        GeneralString = 27,
        /// <summary>Universal String</summary>
        UniversalString = 28,
        /// <summary>Character String</summary>
        CharacterString = 29,
        /// <summary>BMP String</summary>
        BMPString = 30,
    }

    /// <summary>
    /// Choices for the Authentication Value
    /// </summary>
    public enum AuthenticationValueChoices : byte
    {
        /// <summary>Character string</summary>
        CharacterString = 0,
        /// <summary>Bit string</summary>
        BitString = 1,
        /// <summary>External</summary>
        External = 2,
        /// <summary>Other</summary>
        Other = 3,
    }

    /// <summary>
    /// Application tags for the ACSE APDUs
    /// </summary>
    public enum ACSEAPDUTags
    {
        /// <summary>AARQ</summary>
        AARQ = 0,
        /// <summary>AARE</summary>
        AARE = 1,
        /// <summary>RLRQ</summary>
        RLRQ = 2,
        /// <summary>RLRE</summary>
        RLRE = 3,
    }

    /// <summary>
    /// The DLMS protocol version
    /// </summary>
    [Flags]
    public enum ProtocolVersions : byte
    {
        /// <summary>Version 1</summary>
        Version1 = 0x80,
    }

    /// <summary>
    /// ACSE Requirement options
    /// </summary>
    [Flags]
    public enum ACSERequirements : byte
    {
        /// <summary>No requirements</summary>
        None = 0x00,
        /// <summary>Requires Authentication</summary>
        Authentication = 0x80,
    }

    /// <summary>
    /// Association Results
    /// </summary>
    public enum AssociationResult : short
    {
        /// <summary>Accepted</summary>
        Accepted = 0,
        /// <summary>Rejected - Permanent</summary>
        RejectedPermanent = 1,
        /// <summary>Rejected - Transient</summary>
        RejectedTransient = 2,
    }

    /// <summary>
    /// A BER encoded field object
    /// </summary>
    public class BEREncodedField
    {
        #region Constants

        /// <summary>Mask for the identifier class</summary>
        protected const byte CLASS_MASK = 0xC0;
        /// <summary>Mask for the identifier type</summary>
        protected const byte TYPE_MASK = 0x20;
        /// <summary>Mask for the identifier tag</summary>
        protected const byte TAG_MASK = 0x1F;
        /// <summary>Mask for the 7 least significant bits in a byte</summary>
        protected const byte LSB_SEVEN_MASK = 0x07F;
        /// <summary>Mask for the most significant bit in a byte</summary>
        protected const byte MSB_MASK = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public BEREncodedField()
        {
            m_Class = ASN1Class.Universal;
            m_DataType = ASN1DataType.Primitive;
            m_Tag = 0;
            m_PrimitiveFieldData = null;
            m_ConstructedFields = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawData">The raw message data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public BEREncodedField(byte[] rawData)
        {
            int CurrentIndex = 0;

            Parse(rawData, ref CurrentIndex);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawData">The raw message data</param>
        /// <param name="currentIndex">The current location in the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public BEREncodedField(byte[] rawData, ref int currentIndex)
        {
            Parse(rawData, ref currentIndex);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Parses the Identifier information out of the data
        /// </summary>
        /// <param name="rawData">The data to parse from</param>
        /// <param name="currentIndex">The current index in the data</param>
        /// <param name="apduClass">The parsed APDU Class</param>
        /// <param name="apduType">The parsed APDU Type</param>
        /// <param name="apduTag">The parsed APDU tag</param>
        /// <returns>True if the identifier was parsed. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        internal static bool TryParseIdentifierBytes(byte[] rawData, ref int currentIndex, out ASN1Class apduClass, out ASN1DataType apduType, out ushort apduTag)
        {
            bool Parsed = false;
            apduClass = ASN1Class.Universal;
            apduType = ASN1DataType.Primitive;
            apduTag = 0;

            if (currentIndex < rawData.Length)
            {
                apduClass = (ASN1Class)(rawData[currentIndex] & CLASS_MASK);
                apduType = (ASN1DataType)(rawData[currentIndex] & TYPE_MASK);
                apduTag = (ushort)(rawData[currentIndex] & TAG_MASK);

                currentIndex++;

                if (apduTag == 31)
                {
                    // The tag is actually stored in the next few bytes
                    apduTag = 0;

                    while (currentIndex < rawData.Length && (rawData[currentIndex] & MSB_MASK) == MSB_MASK)
                    {
                        apduTag = (ushort)((apduTag << 7) | (rawData[currentIndex] & LSB_SEVEN_MASK));
                        currentIndex++;
                    }

                    if (currentIndex < rawData.Length)
                    {
                        // The current byte is now the last byte in the tag
                        apduTag = (ushort)((apduTag << 7) | (rawData[currentIndex] & LSB_SEVEN_MASK));
                        currentIndex++;

                        Parsed = true;
                    }
                }
                else
                {
                    // We have all of the information
                    Parsed = true;
                }
            }

            return Parsed;
        }

        /// <summary>
        /// Parses the data length from the specified data
        /// </summary>
        /// <param name="rawData">The data to parse from</param>
        /// <param name="currentIndex">The current index in the data</param>
        /// <param name="length">The length of the payload data</param>
        /// <returns>True if the length was parsed. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        internal static bool TryParseDataLength(byte[] rawData, ref int currentIndex, out uint length)
        {
            bool Parsed = false;
            length = 0;

            if (currentIndex < rawData.Length)
            {
                if (rawData[currentIndex] <= 127)
                {
                    // We have the short form of the length value which is just the 7 least significant bits
                    length = (uint)(rawData[currentIndex] & LSB_SEVEN_MASK);
                    currentIndex++;

                    Parsed = true;
                }
                else
                {
                    // The current byte contains the number of bytes that makes up the length
                    int LengthBytes = rawData[currentIndex] & LSB_SEVEN_MASK;
                    currentIndex++;

                    for (int LengthIndex = currentIndex; LengthIndex < currentIndex + LengthBytes && LengthIndex < rawData.Length; LengthIndex++)
                    {
                        length = (uint)((length << 7) | (uint)(rawData[LengthIndex] & LSB_SEVEN_MASK));
                    }

                    currentIndex += LengthBytes;
                    Parsed = true;
                }
            }

            return Parsed;
        }

        /// <summary>
        /// Gets the object Identifier bytes for the specified tag
        /// </summary>
        /// <param name="apduClass">The Class type of the object</param>
        /// <param name="apduType">The Data type of the object</param>
        /// <param name="tag">The tag of the object</param>
        /// <returns>The raw byte format of the identifier</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        internal static List<byte> GetIdentifierBytes(ASN1Class apduClass, ASN1DataType apduType, ushort tag)
        {
            List<byte> Data = new List<byte>();

            // Determine the tag bytes
            byte Identifier = (byte)((byte)apduClass | (byte)apduType);

            if (tag < 31)
            {
                // The ID is only one byte long
                Identifier |= (byte)tag;
                Data.Add(Identifier);
            }
            else
            {
                // The ID requires multiple bytes so set the first byte's tag to 31
                Identifier |= (byte)31;
                Data.Add(Identifier);

                int TagBytes = tag / 127;

                for (int CurrentByte = TagBytes; CurrentByte >= 0; CurrentByte--)
                {
                    if (CurrentByte == 0)
                    {
                        // This is the last byte in the tag so bit 8 should be 0 and the
                        // other 7 values should be the remaining part of the tag
                        Data.Add((byte)(tag & LSB_SEVEN_MASK));
                    }
                    else
                    {
                        // There are multiple bytes in the tag so bit 8 should be set to 1
                        // and the rest of the byte should be the next most significant 7 bits
                        Data.Add((byte)(((tag >> (7 * CurrentByte)) & LSB_SEVEN_MASK) | MSB_MASK));
                    }
                }
            }

            return Data;
        }

        /// <summary>
        /// Gets the bytes for a specified length value
        /// </summary>
        /// <param name="length">The data length to get</param>
        /// <returns>The bytes that correspond to the length</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        internal static List<byte> GetLengthBytes(uint length)
        {
            List<byte> Data = new List<byte>();

            if (length <= 127)
            {
                Data.Add((byte)length);
            }
            else
            {
                // We need to break up the value into multiple bytes
                uint LengthBytes = (length / 127) + 1;

                // We cant specify a length that requires 127 or more length bytes
                if (LengthBytes < 127)
                {
                    // The first byte specifies the number of following bytes.
                    // Bit 8 needs to be set to indicate the number of length bytes is being specified
                    Data.Add((byte)(LengthBytes | MSB_MASK));

                    for (uint CurrentByte = LengthBytes; CurrentByte > 0; CurrentByte--)
                    {
                        // The bytes that follow will have bit 8 set to 0 and the remaining bits
                        // will contain the next 7 bits of the length value
                        Data.Add((byte)((length >> (int)(7 * CurrentByte)) & LSB_SEVEN_MASK));
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("length");
                }
            }

            return Data;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data into the APDUField object
        /// </summary>
        /// <param name="rawData">The raw data to parse</param>
        /// <param name="currentIndex">The current index into the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected void Parse(byte[] rawData, ref int currentIndex)
        {
            uint DataLength = 0;

            if (rawData != null)
            {
                // Parse the identifier
                if (TryParseIdentifierBytes(rawData, ref currentIndex, out m_Class, out m_DataType, out m_Tag) == false)
                {
                    throw new ArgumentException("The data specified does not have a valid Field Identifier", "rawData");
                }

                // Parse the length
                if (TryParseDataLength(rawData, ref currentIndex, out DataLength) == false)
                {
                    throw new ArgumentException("The data specified does not have a valid Field Data Length", "rawData");
                }

                // Parse the field data
                if (currentIndex + DataLength <= rawData.Length)
                {
                    if (m_DataType == ASN1DataType.Primitive)
                    {
                        m_PrimitiveFieldData = new byte[DataLength];
                        Array.Copy(rawData, currentIndex, m_PrimitiveFieldData, 0, DataLength);

                        currentIndex += (int)DataLength;
                    }
                    else
                    {
                        m_ConstructedFields = new List<BEREncodedField>();
                        long ConstructedFieldStop = currentIndex + DataLength;

                        // Parse each of the constructed fields in the data
                        while (currentIndex < ConstructedFieldStop)
                        {
                            BEREncodedField NewField = new BEREncodedField(rawData, ref currentIndex);
                            m_ConstructedFields.Add(NewField);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("The data given is not the correct length", "rawData");
                }
            }
            else
            {
                throw new ArgumentNullException("rawData", "The field cannot be constructed from null data");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the primitive data contained in the field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] PrimitiveFieldData
        {
            get
            {
                return m_PrimitiveFieldData;
            }
            set
            {
                m_PrimitiveFieldData = value;
            }
        }

        /// <summary>
        /// Gets or sets the constructed data fields contained in the field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public List<BEREncodedField> ConstructedFields
        {
            get
            {
                return m_ConstructedFields;
            }
            set
            {
                m_ConstructedFields = value;
            }
        }

        /// <summary>
        /// Gets or sets the Tag identifier of the APDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }

        /// <summary>
        /// Gets or sets the Class of the APDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ASN1Class Class
        {
            get
            {
                return m_Class;
            }
            set
            {
                m_Class = value;
            }
        }

        /// <summary>
        /// Gets or sets the data type of the 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ASN1DataType DataType
        {
            get
            {
                return m_DataType;
            }
            set
            {
                m_DataType = value;

                if (m_DataType == ASN1DataType.Constructed)
                {
                    m_ConstructedFields = new List<BEREncodedField>();
                    m_PrimitiveFieldData = null;
                }
                else
                {
                    m_ConstructedFields = null;
                }
            }
        }

        /// <summary>
        /// Gets the raw bytes for the field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] RawData
        {
            get
            {
                List<byte> Data = new List<byte>();

                Data.AddRange(GetIdentifierBytes(m_Class, m_DataType, m_Tag));

                if (m_DataType == ASN1DataType.Primitive)
                {
                    // Add the length of the data
                    Data.AddRange(GetLengthBytes((uint)m_PrimitiveFieldData.Length));

                    // Add the data
                    Data.AddRange(m_PrimitiveFieldData);
                }
                else
                {
                    // Combine the data for each of the fields
                    List<byte> FieldData = new List<byte>();

                    foreach (BEREncodedField CurrentField in m_ConstructedFields)
                    {
                        FieldData.AddRange(CurrentField.RawData);
                    }

                    // Add the length
                    Data.AddRange(GetLengthBytes((uint)FieldData.Count));

                    // Add the data
                    Data.AddRange(FieldData);
                }

                return Data.ToArray();
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// The field's data
        /// </summary>
        protected byte[] m_PrimitiveFieldData;
        /// <summary>
        /// The field's list of constructed fields.
        /// </summary>
        protected List<BEREncodedField> m_ConstructedFields;
        /// <summary>
        /// The field's identifier tag
        /// </summary>
        protected ushort m_Tag;
        /// <summary>
        /// The field's identifier class
        /// </summary>
        protected ASN1Class m_Class;
        /// <summary>
        /// The field's identifier type
        /// </summary>
        protected ASN1DataType m_DataType;

        #endregion
    }

    /// <summary>
    /// ACSE APDU base class
    /// </summary>
    public abstract class ACSEAPDU : xDLMSAPDU
    {
        #region Constants

        /// <summary>Mask for the 7 least significant bits in a byte</summary>
        protected const byte LSB_SEVEN_MASK = 0x07F;
        /// <summary>Mask for the most significant bit in a byte</summary>
        protected const byte MSB_MASK = 0x80;

        #endregion

        #region Public Methods

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

            int Length = ParseDataLength(apduStream);
            int CurrentIndex = 0;

            byte[] BERFieldData = DataReader.ReadBytes(Length);

            m_ConstructedFields = new List<BEREncodedField>();

            while (CurrentIndex < Length)
            {
                BEREncodedField NewField = new BEREncodedField(BERFieldData, ref CurrentIndex);
                m_ConstructedFields.Add(NewField);
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
        
        protected ACSEAPDU()
        {
            m_ConstructedFields = new List<BEREncodedField>();
        }

        /// <summary>
        /// Parses the data length from the specified data
        /// </summary>
        /// <param name="rawData">The data to parse from</param>
        /// <returns>The length in number of bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        protected static int ParseDataLength(Stream rawData)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(rawData);
            byte CurrentByte;
            int Length = 0;

            if (rawData.Position < rawData.Length)
            {
                CurrentByte = DataReader.ReadByte();

                if (CurrentByte <= 127)
                {
                    // We have the short form of the length value which is just the 7 least significant bits
                    Length =CurrentByte & LSB_SEVEN_MASK;
                }
                else
                {
                    // The current byte contains the number of bytes that makes up the length
                    int LengthBytes = CurrentByte & LSB_SEVEN_MASK;

                    for (int LengthIndex = 0; LengthIndex < LengthBytes; LengthIndex++)
                    {
                        CurrentByte = DataReader.ReadByte();

                        Length = (Length << 7) | (CurrentByte & LSB_SEVEN_MASK);
                    }
                }
            }

            return Length;
        }

        /// <summary>
        /// Generates the raw data for the APDU
        /// </summary>
        /// <returns>The MemoryStream containing the raw data</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        protected override MemoryStream GenerateRawData()
        {
            MemoryStream APDUStream = base.GenerateRawData();
            MemoryStream FieldStream = new MemoryStream();
            DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(FieldStream);
            byte[] BERFieldData = null;

            // First we need generate the data for all of the BER encoded fields to determine the length
            foreach (BEREncodedField CurrentField in m_ConstructedFields)
            {
                DataWriter.Write(CurrentField.RawData);
            }

            BERFieldData = FieldStream.ToArray();

            DataWriter.Close();

            // Write the data to the APDU Stream
            DataWriter = new DLMSBinaryWriter(APDUStream);

            DataWriter.Write(BEREncodedField.GetLengthBytes((uint)BERFieldData.Length).ToArray());
            DataWriter.Write(BERFieldData);

            return APDUStream;
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// The APDU's list of constructed fields.
        /// </summary>
        protected List<BEREncodedField> m_ConstructedFields;

        #endregion
    }

    /// <summary>
    /// APDU for the AARQ (association request) message
    /// </summary>
    public class AARQAPDU : ACSEAPDU
    {
        #region Definitions

        /// <summary>
        /// Tags for the AARQ fields
        /// </summary>
        private enum FieldTags : ushort
        {
            ProtocolVersion = 0,
            ApplicationContextName = 1,
            CalledAPTitle = 2,
            CalledAEQualifier = 3,
            CalledAPInvocationID = 4,
            CalledAEInvocationID = 5,
            CallingAPTitle = 6,
            CallingAEQualifier = 7,
            CallingAPInvocationID = 8,
            CallingAEInvocationID = 9,
            SenderACSERequirements = 10,
            MechanismName = 11,
            CallingAuthenticationValue = 12,
            ImplementationInformation = 29,
            UserInformation = 30,
        }

        /// <summary>
        /// Application OID for ACSE using Logical Names and No Ciphering
        /// </summary>
        public static readonly byte[] LogicalNameWithNoCipherApplicationOID = new byte[] { 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, 0x01 };
        /// <summary>
        /// Application OID for ACSE using Logical Name and Ciphering
        /// </summary>
        public static readonly byte[] LogicalNameWithCipherApplicationOID = new byte[] { 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, 0x03 };

        /// <summary>
        /// Application OID for ACSE using Short Names
        /// </summary>
        public static readonly byte[] ShortNameWithNoCipherApplicationOID = new byte[] { 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, 0x02 };
        /// <summary>
        /// Application OID for ACSE using Short Names and Ciphering
        /// </summary>
        public static readonly byte[] ShortNameWithCipherApplicationOID = new byte[] { 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, 0x04 };

        /// <summary>
        /// Mechanism Name for Low Level Security
        /// </summary>
        public static readonly byte[] LowLevelSecurityMechanismName = new byte[] { 0x60, 0x85, 0x74, 0x05, 0x08, 0x02, 0x01 };
        /// <summary>
        /// Mechanism Name for High Level Security
        /// </summary>
        public static readonly byte[] HighLevelSecurityMechanismName = new byte[] { 0x60, 0x85, 0x74, 0x05, 0x08, 0x02, 0x05 }; 

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public AARQAPDU()
            : base()
        {
            m_Tag = xDLMSTags.AARQ;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the protocol version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ProtocolVersions ProtocolVersion
        {
            get
            {
                ProtocolVersions Version = ProtocolVersions.Version1;
                BEREncodedField Field = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.ProtocolVersion);

                if (Field != null && Field.PrimitiveFieldData != null && Field.PrimitiveFieldData.Length == 2)
                {
                    Version = (ProtocolVersions)Field.PrimitiveFieldData[1];
                }

                return Version;
            }
            set
            {
                // First remove any existing values
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.ProtocolVersion);

                // The default value for this is Version 1 so we don't need to add it if setting to Version 1
                if (value != ProtocolVersions.Version1)
                {
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Primitive;
                    NewField.Tag = (ushort)FieldTags.ProtocolVersion;

                    // First byte indicates the number of unused bits in the second. This should be updated
                    // if it is ever used.
                    NewField.PrimitiveFieldData = new byte[] { (byte)0x07, (byte)value };

                    m_ConstructedFields.Add(NewField);

                    // Make sure the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Application Context Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] ApplicationContextName
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.ApplicationContextName);
                BEREncodedField IdentifierField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The Context Name is required and should always be the first field listed.
                    IdentifierField = ConstructedField.ConstructedFields[0];

                    if (IdentifierField.DataType == ASN1DataType.Primitive && IdentifierField.Tag == (ushort)UniversalTags.ObjectIdentifier)
                    {
                        Value = IdentifierField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                if (value != null)
                {
                    // Clear out any existing fields
                    m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.ApplicationContextName);

                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.ApplicationContextName;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.Universal;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)UniversalTags.ObjectIdentifier;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
                else
                {
                    throw new ArgumentNullException("value", "The Application Context Name may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Calling ApTitle
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/20/13 RCG 2.80.39 N/A    Created
        
        public byte[] CallingApTitle
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.CallingAPTitle);
                BEREncodedField IdentifierField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The ApTitle is required and should always be the first field listed.
                    IdentifierField = ConstructedField.ConstructedFields[0];

                    if (IdentifierField.DataType == ASN1DataType.Primitive && IdentifierField.Tag == (ushort)UniversalTags.OctetString)
                    {
                        Value = IdentifierField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.CallingAPTitle);

                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.CallingAPTitle;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.Universal;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)UniversalTags.OctetString;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Sender ACSE Requirements
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ACSERequirements SenderACSERequirements
        {
            get
            {
                ACSERequirements Requirements = ACSERequirements.None;
                BEREncodedField Field = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.SenderACSERequirements);

                if (Field != null && Field.PrimitiveFieldData != null && Field.PrimitiveFieldData.Length == 2)
                {
                    Requirements = (ACSERequirements)Field.PrimitiveFieldData[1];
                }

                return Requirements;
            }
            set
            {
                // First remove any existing values
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.SenderACSERequirements);

                // The default value is None so if it is set to this then we don't need to add the field
                if (value != ACSERequirements.None)
                {
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Primitive;
                    NewField.Tag = (ushort)FieldTags.SenderACSERequirements;

                    // First byte indicates the number of unused bits in the second.
                    NewField.PrimitiveFieldData = new byte[] { (byte)0x07, (byte)value };

                    m_ConstructedFields.Add(NewField);

                    // Make sure the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Mechanism Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] MechanismName
        {
            get
            {
                byte[] Value = null;
                BEREncodedField Field = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.MechanismName);

                if (Field != null && Field.DataType == ASN1DataType.Primitive)
                {
                    Value = Field.PrimitiveFieldData;
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.MechanismName);
                
                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Primitive;
                    NewField.Tag = (ushort)FieldTags.MechanismName;

                    NewField.PrimitiveFieldData = value;

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Application Context Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] CallingAuthenticationValue
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.CallingAuthenticationValue);
                BEREncodedField AuthenticationValueField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The Context Name is required and should always be the first field listed.
                    AuthenticationValueField = ConstructedField.ConstructedFields[0];

                    if (AuthenticationValueField.DataType == ASN1DataType.Primitive && AuthenticationValueField.Tag == (ushort)AuthenticationValueChoices.CharacterString)
                    {
                        Value = AuthenticationValueField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.CallingAuthenticationValue);

                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.CallingAuthenticationValue;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.ContextSpecific;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)AuthenticationValueChoices.CharacterString;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the User Information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] UserInformation
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.UserInformation);
                BEREncodedField UserInformationField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The Context Name is required and should always be the first field listed.
                    UserInformationField = ConstructedField.ConstructedFields[0];

                    if (UserInformationField.DataType == ASN1DataType.Primitive && UserInformationField.Tag == (ushort)UniversalTags.OctetString)
                    {
                        Value = UserInformationField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.UserInformation);

                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.UserInformation;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.Universal;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)UniversalTags.OctetString;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// APDU for the AARE (association response) message
    /// </summary>
    public class AAREAPDU : ACSEAPDU
    {
        #region Definitions

        /// <summary>
        /// The tags for each of the fields in the APDU
        /// </summary>
        private enum FieldTags : ushort
        {
            ProtocolVersion = 0,
            ApplicationContextName = 1,
            Result = 2,
            ResultSourceDiagnostic = 3,
            RespondingAPTitle = 4,
            RespondingAEQualifier = 5,
            RespondingAPInvocationID = 6,
            RespondingAEInvocationID = 7,
            ResponderACSERequirements = 8,
            MechanismName = 9,
            RespondingAuthenticationValue = 10,
            ImplementationInformation = 29,
            UserInformation = 30,
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

        public AAREAPDU()
            : base()
        {
            m_Tag = xDLMSTags.AARE;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the protocol version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ProtocolVersions ProtocolVersion
        {
            get
            {
                ProtocolVersions Version = ProtocolVersions.Version1;
                BEREncodedField Field = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.ProtocolVersion);

                if (Field != null && Field.PrimitiveFieldData != null && Field.PrimitiveFieldData.Length == 2)
                {
                    Version = (ProtocolVersions)Field.PrimitiveFieldData[1];
                }

                return Version;
            }
            set
            {
                // First remove any existing values
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.ProtocolVersion);

                // The default value for this is Version 1 so we don't need to add it if setting to Version 1
                if (value != ProtocolVersions.Version1)
                {
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Primitive;
                    NewField.Tag = (ushort)FieldTags.ProtocolVersion;

                    // First byte indicates the number of unused bits in the second. This should be updated
                    // if it is ever used.
                    NewField.PrimitiveFieldData = new byte[] { (byte)0x07, (byte)value };

                    m_ConstructedFields.Add(NewField);

                    // Make sure the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Application Context Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] ApplicationContextName
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.ApplicationContextName);
                BEREncodedField IdentifierField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The Context Name is required and should always be the first field listed.
                    IdentifierField = ConstructedField.ConstructedFields[0];

                    if (IdentifierField.DataType == ASN1DataType.Primitive && IdentifierField.Tag == (ushort)UniversalTags.ObjectIdentifier)
                    {
                        Value = IdentifierField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                if (value != null)
                {
                    // Clear out any existing fields
                    m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.ApplicationContextName);

                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.ApplicationContextName;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.Universal;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)UniversalTags.ObjectIdentifier;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
                else
                {
                    throw new ArgumentNullException("value", "The Application Context Name may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Responding ApTitle
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/20/13 RCG 2.80.39 N/A    Created

        public byte[] RespondingApTitle
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.RespondingAPTitle);
                BEREncodedField IdentifierField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The ApTitle is required and should always be the first field listed.
                    IdentifierField = ConstructedField.ConstructedFields[0];

                    if (IdentifierField.DataType == ASN1DataType.Primitive && IdentifierField.Tag == (ushort)UniversalTags.OctetString)
                    {
                        Value = IdentifierField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.RespondingAPTitle);

                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.RespondingAPTitle;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.Universal;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)UniversalTags.OctetString;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the result of the association
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public AssociationResult Result
        {
            get
            {
                AssociationResult Value = AssociationResult.RejectedPermanent;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.Result);
                BEREncodedField PrimitiveField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The Result is required and should always be the first field listed.
                    PrimitiveField = ConstructedField.ConstructedFields[0];

                    if (PrimitiveField.DataType == ASN1DataType.Primitive && PrimitiveField.Tag == (ushort)UniversalTags.Integer && PrimitiveField.PrimitiveFieldData.Length == 1)
                    {
                        Value = (AssociationResult)PrimitiveField.PrimitiveFieldData[0];
                    }
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.Result);

                // Create the Application Context field
                BEREncodedField NewField = new BEREncodedField();
                NewField.Class = ASN1Class.ContextSpecific;
                NewField.DataType = ASN1DataType.Constructed;
                NewField.Tag = (ushort)FieldTags.Result;

                // Since the context value itself is constructed we need to put it inside another APDUField
                BEREncodedField ConstructedField = new BEREncodedField();
                ConstructedField.Class = ASN1Class.Universal;
                ConstructedField.DataType = ASN1DataType.Primitive;
                ConstructedField.Tag = (ushort)UniversalTags.Integer;

                ConstructedField.PrimitiveFieldData = new byte[1];

                // The Result is store as an Integer of length 1
                ConstructedField.PrimitiveFieldData[0] = (byte)value;

                NewField.ConstructedFields.Add(ConstructedField);

                m_ConstructedFields.Add(NewField);

                // Make sure that the fields are properly sorted
                m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
            }
        }

        /// <summary>
        /// Gets or sets the Responder ACSE Requirements
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ACSERequirements ResponderACSERequirements
        {
            get
            {
                ACSERequirements Requirements = ACSERequirements.None;
                BEREncodedField Field = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.ResponderACSERequirements);

                if (Field != null && Field.PrimitiveFieldData != null && Field.PrimitiveFieldData.Length == 2)
                {
                    Requirements = (ACSERequirements)Field.PrimitiveFieldData[1];
                }

                return Requirements;
            }
            set
            {
                // First remove any existing values
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.ResponderACSERequirements);

                // The default value is None so if it is set to this then we don't need to add the field
                if (value != ACSERequirements.None)
                {
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Primitive;
                    NewField.Tag = (ushort)FieldTags.ResponderACSERequirements;

                    // First byte indicates the number of unused bits in the second.
                    NewField.PrimitiveFieldData = new byte[] { (byte)0x07, (byte)value };

                    m_ConstructedFields.Add(NewField);

                    // Make sure the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Mechanism Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] MechanismName
        {
            get
            {
                byte[] Value = null;
                BEREncodedField Field = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.MechanismName);

                if (Field != null && Field.DataType == ASN1DataType.Primitive)
                {
                    Value = Field.PrimitiveFieldData;
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.MechanismName);
                
                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Primitive;
                    NewField.Tag = (ushort)FieldTags.MechanismName;

                    NewField.PrimitiveFieldData = value;

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Application Context Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] RespondingAuthenticationValue
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.RespondingAuthenticationValue);
                BEREncodedField AuthenticationValueField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The Context Name is required and should always be the first field listed.
                    AuthenticationValueField = ConstructedField.ConstructedFields[0];

                    if (AuthenticationValueField.DataType == ASN1DataType.Primitive && AuthenticationValueField.Tag == (ushort)AuthenticationValueChoices.CharacterString)
                    {
                        Value = AuthenticationValueField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.RespondingAuthenticationValue);

                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.RespondingAuthenticationValue;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.ContextSpecific;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)AuthenticationValueChoices.CharacterString;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the User Information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] UserInformation
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.UserInformation);
                BEREncodedField UserInformationField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The Context Name is required and should always be the first field listed.
                    UserInformationField = ConstructedField.ConstructedFields[0];

                    if (UserInformationField.DataType == ASN1DataType.Primitive && UserInformationField.Tag == (ushort)UniversalTags.OctetString)
                    {
                        Value = UserInformationField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.UserInformation);

                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.UserInformation;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.Universal;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)UniversalTags.OctetString;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// APDU for the RLRQ (release request) message
    /// </summary>
    public class RLRQAPDU : ACSEAPDU
    {
        #region Definitions

        /// <summary>
        /// The tags for each of the fields in the APDU
        /// </summary>
        private enum FieldTags : ushort
        {
            Reason = 0,
            UserInformation = 30,
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

        public RLRQAPDU()
            : base()
        {
            m_Tag = xDLMSTags.RLRQ;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the result of the association
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] Reason
        {
            get
            {
                byte[] Value = null;
                BEREncodedField PrimitiveField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.Reason);

                if (PrimitiveField != null && PrimitiveField.DataType == ASN1DataType.Primitive)
                {
                    Value = PrimitiveField.PrimitiveFieldData;
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.Reason);

                // The field is optional so if null is set we should just not add it
                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Primitive;
                    NewField.Tag = (ushort)FieldTags.Reason;

                    NewField.PrimitiveFieldData = value;

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the User Information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] UserInformation
        {
            get
            {
                byte[] Value = null;
                BEREncodedField ConstructedField = m_ConstructedFields.FirstOrDefault(f => f.Tag == (ushort)FieldTags.UserInformation);
                BEREncodedField UserInformationField = null;

                if (ConstructedField != null && ConstructedField.DataType == ASN1DataType.Constructed && ConstructedField.ConstructedFields.Count > 0)
                {
                    // The Context Name is required and should always be the first field listed.
                    UserInformationField = ConstructedField.ConstructedFields[0];

                    if (UserInformationField.DataType == ASN1DataType.Primitive && UserInformationField.Tag == (ushort)UniversalTags.OctetString)
                    {
                        Value = UserInformationField.PrimitiveFieldData;
                    }
                }

                return Value;
            }
            set
            {
                // Clear out any existing fields
                m_ConstructedFields.RemoveAll(f => f.Tag == (ushort)FieldTags.UserInformation);

                if (value != null)
                {
                    // Create the Application Context field
                    BEREncodedField NewField = new BEREncodedField();
                    NewField.Class = ASN1Class.ContextSpecific;
                    NewField.DataType = ASN1DataType.Constructed;
                    NewField.Tag = (ushort)FieldTags.UserInformation;

                    // Since the context value itself is constructed we need to put it inside another APDUField
                    BEREncodedField ConstructedField = new BEREncodedField();
                    ConstructedField.Class = ASN1Class.Universal;
                    ConstructedField.DataType = ASN1DataType.Primitive;
                    ConstructedField.Tag = (ushort)UniversalTags.OctetString;

                    ConstructedField.PrimitiveFieldData = value;

                    NewField.ConstructedFields.Add(ConstructedField);

                    m_ConstructedFields.Add(NewField);

                    // Make sure that the fields are properly sorted
                    m_ConstructedFields = m_ConstructedFields.OrderBy(f => f.Tag).ToList();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// APDU for the RLRE (release response) message
    /// </summary>
    public class RLREAPDU : RLRQAPDU
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public RLREAPDU()
            : base()
        {
            m_Tag = xDLMSTags.RLRE;
        }

        #endregion
    }
}
