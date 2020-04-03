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
    /// Binary Reader for DLMS data types
    /// </summary>
    public class DLMSBinaryReader : BinaryReader
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="input">The stream to read from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DLMSBinaryReader(Stream input)
            : base(input)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="input">The stream to read from</param>
        /// <param name="encoding">The string encoding to use</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public DLMSBinaryReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }

        /// <summary>
        /// Reads a 16-bit signed integer
        /// </summary>
        /// <returns>The value read from the stream</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override short ReadInt16()
        {
            byte[] Value = ReadBytes(2);

            Array.Reverse(Value);

            return BitConverter.ToInt16(Value, 0);
        }

        /// <summary>
        /// Reads a 32-bit signed integer
        /// </summary>
        /// <returns>The value read from the stream</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override int ReadInt32()
        {
            byte[] Value = ReadBytes(4);

            Array.Reverse(Value);

            return BitConverter.ToInt32(Value, 0);
        }

        /// <summary>
        /// Reads a 64-bit signed integer
        /// </summary>
        /// <returns>The value read from the stream</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override long ReadInt64()
        {
            byte[] Value = ReadBytes(8);

            Array.Reverse(Value);

            return BitConverter.ToInt64(Value, 0);
        }

        /// <summary>
        /// Reads a 16-bit signed integer
        /// </summary>
        /// <returns>The value read from the stream</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override ushort ReadUInt16()
        {
            byte[] Value = ReadBytes(2);

            Array.Reverse(Value);

            return BitConverter.ToUInt16(Value, 0);
        }

        /// <summary>
        /// Reads a 32-bit signed integer
        /// </summary>
        /// <returns>The value read from the stream</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override uint ReadUInt32()
        {
            byte[] Value = ReadBytes(4);

            Array.Reverse(Value);

            return BitConverter.ToUInt32(Value, 0);
        }

        /// <summary>
        /// Reads a 64-bit signed integer
        /// </summary>
        /// <returns>The value read from the stream</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override ulong ReadUInt64()
        {
            byte[] Value = ReadBytes(8);

            Array.Reverse(Value);

            return BitConverter.ToUInt64(Value, 0);
        }

        /// <summary>
        /// Reads a boolean value
        /// </summary>
        /// <returns>The boolean value read from the stream</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override bool ReadBoolean()
        {
            byte Value = ReadByte();

            // 0x00 is False and for DLMS 0xFF is true. 
            // When reading lets count anything that is not 0x00 as true just to be safe
            return Value > 0;
        }

        /// <summary>
        /// Reads the usage flag for an optional field
        /// </summary>
        /// <returns>True if the field is used. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public bool ReadUsageFlag()
        {
            // This is separate from the ReadBoolean because DLMS uses 0xFF for true for booleans and 0x01 for true for
            // usage flags
            byte Value = ReadByte();

            return Value > 0;
        }

        /// <summary>
        /// Reads an Enum value from the stream
        /// </summary>
        /// <typeparam name="T">The Enumeration Type to read</typeparam>
        /// <returns>The enumerated value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public T ReadEnum<T>()
        {
            if (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(byte))
            {
                byte Value = ReadByte();

                // Can't cast directly to a generic type so you have to cast to an object first
                return (T)Enum.ToObject(typeof(T), Value);
            }
            else
            {
                throw new ArgumentException("ReadEnum can only read enumerations with an underlying type of byte.");
            }
        }

        /// <summary>
        /// Converts a byte[] version of a bit string to an enum
        /// </summary>
        /// <param name="bitString">The bit string to convert</param>
        /// <returns>The bit string as an enum</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public static T ConvertBitStringToEnum<T>(byte[] bitString)
        {
            Type EnumType = typeof(T);
            ulong Value = 0;

            if (EnumType.IsEnum && EnumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
            {
                foreach (byte CurrentByte in bitString)
                {
                    Value = (ulong)(Value << 8) | (ulong)CurrentByte;
                }

                // Can't cast directly to a generic type so you have to cast to an object first
                return (T)Enum.ToObject(EnumType, Value);
            }
            else
            {
                throw new ArgumentException("ConvertBitStringToEnum can only read objects of type Enum marked with the Flags attribute.");
            }
        }

        /// <summary>
        /// Reads a variable length bit string from the stream
        /// </summary>
        /// <typeparam name="T">The Enumeration Type to read</typeparam>
        /// <returns>The enumerated value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public T ReadBitString<T>()
        {
            Type EnumType = typeof(T);

            if (EnumType.IsEnum && EnumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
            {
                byte[] BitStringBytes = ReadBitString();

                if (BitStringBytes.Length <= 8)
                {
                    return ConvertBitStringToEnum<T>(BitStringBytes);
                }
                else
                {
                    // Move the stream position back one byte so that it can be read through other means
                    BaseStream.Seek(-1 * BitStringBytes.Length, SeekOrigin.Current);
                    throw new ArgumentException("ReadBitString can not be used to read bit strings longer than 64 bits");
                }
            }
            else
            {
                throw new ArgumentException("ReadBitString can only read objects of type Enum marked with the Flags attribute.");
            }
        }

        /// <summary>
        /// Reads a fixed length bit string from the stream
        /// </summary>
        /// <param name="size">The number of bits in the bit string</param>
        /// <typeparam name="T">The Enumeration Type to read</typeparam>
        /// <returns>The enumerated value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public T ReadBitString<T>(int size)
        {
            Type EnumType = typeof(T);

            if (EnumType.IsEnum && EnumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
            {
                byte[] BitStringBytes = ReadBitString(size);

                // Since this method is intended to work on enumerations we are limited to bit strings no longer
                // than 8 bytes
                if (BitStringBytes.Length <= 8)
                {
                    return ConvertBitStringToEnum<T>(BitStringBytes);
                }
                else
                {
                    // Move the stream position back one byte so that it can be read through other means
                    BaseStream.Seek(-1 * BitStringBytes.Length, SeekOrigin.Current);
                    throw new ArgumentException("ReadBitString can not be used to read bit strings longer than 64 bits");
                }

            }
            else
            {
                throw new ArgumentException("ReadBitString can only read objects of type Enum marked with the Flags attribute.");
            }
        }

        /// <summary>
        /// Reads a variable length bit string as a byte array
        /// </summary>
        /// <returns>The bit string read from the meter.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] ReadBitString()
        {
            int BitsToRead = ReadLength();
            int BytesToRead = BitsToRead / 8;
            byte[] Value = null;

            if (BitsToRead % 8 > 0)
            {
                BytesToRead++;
            }

            Value = ReadBytes(BytesToRead);

            return Value;
        }

        /// <summary>
        /// Reads a fixed length bit string as a byte array
        /// </summary>
        /// <param name="size">The number of bits to read.</param>
        /// <returns>The bit string as a byte array</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public byte[] ReadBitString(int size)
        {
            int BytesToRead = size / 8;
            byte[] Value = null;

            // If the number of bits is not evenly divisible add 1 byte for the remaining bits
            if (size % 8 > 0)
            {
                BytesToRead++;
            }

            Value = ReadBytes(BytesToRead);

            // Can't cast directly to a generic type so you have to cast to an object first
            return Value;
        }

        /// <summary>
        /// Reads a byte string from stream that includes the length
        /// </summary>
        /// <returns>The byte string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] ReadBytesWithLength()
        {
            byte LengthByte = ReadByte();
            byte[] Value = null;

            // If the most significant bit of the first byte is not set then it is the length
            // If the bit is set then it specifies the number of bytes in the length
            if (LengthByte < 128)
            {
                Value = ReadBytes(LengthByte);
            }
            else if (LengthByte - 128 <= 4)
            {
                // The length is currently the number of bytes (plus 128) in the length
                int ActualLength = 0;
                byte[] LengthBytes = ReadBytes(LengthByte - 128);

                foreach (byte CurrentByte in LengthBytes)
                {
                    ActualLength = (LengthByte << 8) | CurrentByte;
                }

                if (ActualLength > 0)
                {
                    Value = ReadBytes(ActualLength);
                }
                else
                {
                    BaseStream.Seek(-1, SeekOrigin.Current);
                    throw new ArgumentException("An unsupported byte string length was read from the stream.");
                }
            }
            else
            {
                BaseStream.Seek(-1, SeekOrigin.Current);
                throw new ArgumentException("ReadBytesWithLength can not be used to read byte strings with a length longer than 4 bytes.");
            }

            return Value;
        }

        /// <summary>
        /// Reads a Visible String from the stream
        /// </summary>
        /// <returns>The resulting string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override string ReadString()
        {
            byte[] StringBytes = ReadBytesWithLength();

            return ASCIIEncoding.ASCII.GetString(StringBytes);
        }

        /// <summary>
        /// Reads a length from the stream
        /// </summary>
        /// <returns>The length</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public int ReadLength()
        {
            byte LengthByte = ReadByte();
            int Length = 0;

            if (LengthByte < 128)
            {
                // The length is only one byte so we can go ahead and read the data
                Length = LengthByte;
            }
            else
            {
                // The length byte indicates the number of bytes in the actual length
                byte[] RawLength = ReadBytes(LengthByte - 128);

                foreach (byte CurrentByte in RawLength)
                {
                    Length = (Length << 8) | CurrentByte;
                }
            }

            return Length;
        }

        /// <summary>
        /// Reads a COSEM Date Time object from the stream
        /// </summary>
        /// <returns>The COSEM Date Time that was read</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDateTime ReadCOSEMDateTime()
        {
            return new COSEMDateTime(ReadBytes(COSEMDateTime.DATE_TIME_LENGTH));
        }

        /// <summary>
        /// Reads a COSEM Date object from the stream
        /// </summary>
        /// <returns>The COSEM date object that was read</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMDate ReadCOSEMDate()
        {
            return new COSEMDate(ReadBytes(COSEMDate.DATE_SIZE));
        }

        /// <summary>
        /// Reads a COSEM Time object from the stream
        /// </summary>
        /// <returns>The COSEM Time object that was read</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public COSEMTime ReadCOSEMTime()
        {
            return new COSEMTime(ReadBytes(COSEMTime.TIME_LENGTH));
        }

        #endregion
    }
}
