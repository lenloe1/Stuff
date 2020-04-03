///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                              Copyright © 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// This class contains a helper function to parse TLV response data.
    /// </summary>
    public static class TLV
    {
        #region Constants

        private const long VENDOR_DEFINED_TYPE = 127;
        private const long ITRON_IANAEN = 1233;
        private const long COAP_WIRE_TYPE_MASK = 0x07;

        #endregion

        #region Definitions

        /// <summary>
        /// Wire type provides the information needed to determine the length of the field in a TLV
        /// </summary>
        public enum WireType : byte
        {
            /// <summary>
            /// Used for int32, int64, uint32, uint64, sint32, sint64, bool, enum
            /// </summary>
            VARINT = 0,
            /// <summary>
            /// Used for fixed64, sfixed64, double
            /// </summary>
            I64_BIT = 1,
            /// <summary>
            /// Used for string, bytes, embedded messages, packed repeated fields
            /// </summary>
            LENGTH_DELIMITED = 2,
            /// <summary>
            /// groups (deprecated)
            /// </summary>
            START_GROUP = 3,
            /// <summary>
            /// groups (deprecated)
            /// </summary>
            END_GROUP = 4,
            /// <summary>
            /// Used for fixed32, sfixed32, float
            /// </summary>
            I32_BIT = 5,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses the content of the buffer passed in and appends the data to the list
        /// passed in.
        /// </summary>
        /// <param name="buffer">the packet data to be parsed</param>
        /// <param name="TLVList">data structure that will hold the parsed data</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/03/12 AF  2.60.40 TREQ6552 Created
        //  08/03/12 AF  2.60.52 201575  Code changes to allow us to deal with repeated fields
        //  09/25/14 AF  4.00.61 523894  Work around for a Cisco defect. TLV 11 returns an extra byte that cannot be
        //                               decoded.
        //  11/08/16 RCG                Added overloaded method to allow us to specify the start position 
        public static void ParseTLVData(byte[] buffer, ref List<TLVData> TLVList)
        {
            ParseTLVData(buffer, ref TLVList, 4);
        }

        /// <summary>
        /// Parses the content of the buffer passed in and appends the data to the list
        /// passed in.
        /// </summary>
        /// <param name="buffer">the packet data to be parsed</param>
        /// <param name="TLVList">data structure that will hold the parsed data</param>
        /// <param name="position">The starting positon of the TLV data</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/08/16 RCG                Added so that this code can be shared with MUSE devices 
        //                              which have a different start position
        public static void ParseTLVData(byte[] buffer, ref List<TLVData> TLVList, ushort position)
        {
            string TLVId = "";
            long length;
            long remainingData;
            TLVOptionsData lastOption = null;

            while (position < buffer.Length)
            {
                TLVData data = new TLVData();

                byte count;
                long varIntData = VarInt.DecodeVarInt(buffer, ref position, out count);

                if (varIntData == VENDOR_DEFINED_TYPE)
                {
                    // this is a vendor defined TLV
                    // +--------+--------+--------+--------+--------
                    // | 127 | IANAEN | Type | Length | Value…
                    // +--------+--------+--------+--------+--------

                    varIntData = VarInt.DecodeVarInt(buffer, ref position, out count);
                    if (varIntData == ITRON_IANAEN)
                    {
                        TLVId = "e1233.";

                        varIntData = VarInt.DecodeVarInt(buffer, ref position, out count);

                        TLVId += varIntData.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    //    +--------+--------+--------
                    //    | Type   | Length | Value…
                    //    +--------+--------+--------

                    TLVId = varIntData.ToString(CultureInfo.InvariantCulture);
                }

                data.TLVID = TLVId;

                length = VarInt.DecodeVarInt(buffer, ref position, out count);
                remainingData = length;

                if (buffer.Length >= position + length)
                {
                    // There has to be at least 2 bytes of data for us to parse - 1 byte for the tag/wire type and 
                    // at least one byte of data
                    do
                    {
                        if (remainingData > 1)
                        {
                            // read and deal with the options
                            lastOption = ParseNextOption(buffer, ref position, ref remainingData);

                            if (lastOption != null)
                            {
                                data.Options.Add(lastOption);
                            }
                        }
                    } while (remainingData > 1 && lastOption != null);

                    TLVList.Add(data);
                }
            }
        }

        /// <summary>
        /// Parses the Next Option value
        /// </summary>
        /// <param name="buffer">The buffer containing the data</param>
        /// <param name="position">The current position within the buffer</param>
        /// <param name="remainingData">The amount of data remaining to be parsed</param>
        /// <returns>The parsed TLV option</returns>
        public static TLVOptionsData ParseNextOption(byte[] buffer, ref ushort position, ref long remainingData)
        {
            TLVOptionsData OptionsData = null;
            long dataField = 0;
            byte count = 0;


            long fieldValue = VarInt.DecodeVarInt(buffer, ref position, out count);

            WireType wireType = (WireType)(fieldValue & COAP_WIRE_TYPE_MASK);
            long fieldNumber = fieldValue >> 3;

            remainingData -= count;

            switch (wireType)
            {
                case WireType.VARINT:
                {
                    dataField = VarInt.DecodeVarInt(buffer, ref position, out count);
                    OptionsData = new TLVOptionsData(BitConverter.GetBytes(dataField), (byte)fieldNumber);
                    remainingData -= count;
                    break;
                }
                case WireType.I64_BIT:
                {
                    byte[] byaData = new byte[sizeof(Int64)];
                    Array.Copy(buffer, position, byaData, 0, sizeof(Int64));
                    OptionsData = new TLVOptionsData(byaData, (byte)fieldNumber);
                    position += sizeof(Int64);
                    remainingData -= sizeof(Int64);
                    break;
                }
                case WireType.LENGTH_DELIMITED:
                {
                    long lengthDelimitedLength = VarInt.DecodeVarInt(buffer, ref position, out count);
                    remainingData -= count;

                    byte[] byaData = new byte[lengthDelimitedLength];

                    Array.Copy(buffer, position, byaData, 0, lengthDelimitedLength);
                    OptionsData = new TLVOptionsData(byaData, (byte)fieldNumber);
                    position += (ushort)lengthDelimitedLength;
                    remainingData -= (ushort)lengthDelimitedLength;
                    break;
                }
                case WireType.I32_BIT:
                {
                    byte[] byaData = new byte[sizeof(Int32)];
                    Array.Copy(buffer, position, byaData, 0, sizeof(Int32));
                    OptionsData = new TLVOptionsData(byaData, (byte)fieldNumber);
                    position += sizeof(Int32);
                    remainingData -= sizeof(Int32);
                    break;
                }
                default:
                {
                    // deprecated or unknown wire type
                    // TODO - what should we do if we encounter one of these?
                    Debug.WriteLine("TLV.ParseNextOption() Warning: Unknown Wire Type detected");
                    break;
                }
            }

            return OptionsData;
        }

        #endregion
    }

    /// <summary>
    /// This class provides a method for decoding a varint from a buffer of bytes.
    /// </summary>
    public static class VarInt
    {
        #region Public Methods

        /// <summary>
        /// This method decodes a varint encoded byte array. Each byte in a varint, except the last byte, has the most significant bit (msb) set – this
        /// indicates that there are further bytes to come. The lower 7 bits of each byte are used to store
        /// the two's complement representation of the number in groups of 7 bits, least significant
        /// group first.
        /// </summary>
        /// <param name="buffer">the byte array containing the varint</param>
        /// <param name="position">keeps track of where we are in the byte array</param>
        /// <param name="count">records how many bytes are taken up by the varint</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- ------   -------------------------------------------
        //  06/29/12 AF  2.60.40 TREQ6552 Adapted from PPP Serial Monitor
        //  07/09/12 AF  2.60.41 TREQ6552 Corrected the max size constant
        //
        public static long DecodeVarInt(byte[] buffer, ref ushort position, out byte count)
        {
            const byte CONTINUE_MASK = 0x80;
            const byte DATA_MASK = 0x7F;
            const ushort MAXIMUM_SIZE = 10;
            long Value = 0;

            count = 0;
            bool done = false;
            byte temp = 0;

            while (!done)
            {
                // Check position for end of buffer
                if ((position < buffer.Length) && (count < MAXIMUM_SIZE))
                {
                    temp = buffer[position];
                    if ((temp & CONTINUE_MASK) != CONTINUE_MASK)
                    {
                        done = true;
                    }

                    temp = (byte)(temp & DATA_MASK);
                    byte shift = (byte)(count * 7);
                    Value |= (long)temp << shift;
                    position++;
                    count++;
                }
                else
                {
                    done = true;
                }
            }

            return Value;
        }

        #endregion

    }

    /// <summary>
    /// Class designed to hold TLV data.
    /// </summary>
    public class TLVData
    {
        #region Public Methods

        /// <summary>
        /// Constructor.  Creates the dictionary object.  The key for each entry is the field number and the data 
        /// is stored in a byte array.  The XML definition file will be used to translate the byte array into a
        /// meaningful value.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/03/12 AF  2.60.40 TREQ6552 Created
        //  08/03/12 AF  2.60.52 201575  Changes to support repeated fields in a TLV
        //
        public TLVData()
        {
            OptionsData = new List<TLVOptionsData>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Dictionary object that holds the options.  The key for each entry is the field number 
        /// and the value is stored in a byte array.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/03/12 AF  2.60.40 TREQ6552 Created
        //  08/03/12 AF  2.60.52 201575  Changes to support repeated fields in a TLV
        //
        public List<TLVOptionsData> Options
        {
            get
            {
                return OptionsData;
            }
        }

        /// <summary>
        /// Gets or sets the TLV Id in the form of a string.  It may seem strange to
        /// store these as strings.  Most of the TLV Ids are numeric but the vendor
        /// defined TLVs are not.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/03/12 AF  2.60.40 TREQ6552 Created
        //
        public string TLVID
        {
            get
            {
                return TLVId;
            }
            set
            {
                TLVId = value;
            }
        }

        #endregion

        #region Members

        // TLV identifier
        private string TLVId;

        // The OptionsData object holds the values for the TLV fields.
        private List<TLVOptionsData> OptionsData;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class TLVOptionsData
    {
        #region Public Methods

        /// <summary>
        /// Class to hold the options data for a TLV
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/12 AF  2.60.52 201575 Created
        //
        public TLVOptionsData()
        {
        }

        /// <summary>
        /// Constructor.  This is the constructor to use since it allows you to assign
        /// the data to the field value object.
        /// </summary>
        /// <param name="fieldValue">the value for this field</param>
        /// <param name="fieldNumber">the field number for this option</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/12 AF  2.60.52 201575 Created
        //
        public TLVOptionsData(byte[] fieldValue, byte fieldNumber)
        {
            m_FieldValue = new byte[fieldValue.Length];
            Array.Copy(fieldValue, m_FieldValue, fieldValue.Length);
            m_FieldNumber = fieldNumber;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Field number - tells us which field this is
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/12 AF  2.60.52 201575 Created
        //
        public byte FieldNumber
        {
            get
            {
                return m_FieldNumber;
            }
            set
            {
                m_FieldNumber = value;
            }
        }

        /// <summary>
        /// Field value is the data for the field.  We will use the definition file
        /// to decode the byte array into a more readable data type.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/12 AF  2.60.52 201575 Created
        //
        public byte[] FieldValue
        {
            get
            {
                return m_FieldValue;
            }
        }

        #endregion

        #region Members

        private byte m_FieldNumber = 0;
        private byte[] m_FieldValue;

        #endregion
    }
}
