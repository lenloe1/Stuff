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
    /// Binary Writer for DLMS data types
    /// </summary>
    public class DLMSBinaryWriter : BinaryWriter
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DLMSBinaryWriter()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output">The stream to write to</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DLMSBinaryWriter(Stream output)
            : base(output)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output">The stream to write to</param>
        /// <param name="encoding">The string encoding to use</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public DLMSBinaryWriter(Stream output, Encoding encoding)
            : base(output, encoding)
        {
        }

        /// <summary>
        /// Writes a 16-bit signed integer
        /// </summary>
        /// <param name="value">The value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Write(short value)
        {
            byte[] ValueBytes = BitConverter.GetBytes(value);

            Array.Reverse(ValueBytes);

            Write(ValueBytes);
        }

        /// <summary>
        /// Writes a 32-bit signed integer
        /// </summary>
        /// <param name="value">The value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public override void Write(int value)
        {
            byte[] ValueBytes = BitConverter.GetBytes(value);

            Array.Reverse(ValueBytes);

            Write(ValueBytes);
        }

        /// <summary>
        /// Writes a 64-bit signed integer
        /// </summary>
        /// <param name="value">The value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Write(long value)
        {
            byte[] ValueBytes = BitConverter.GetBytes(value);

            Array.Reverse(ValueBytes);

            Write(ValueBytes);
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer
        /// </summary>
        /// <param name="value">The value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Write(ushort value)
        {
            byte[] ValueBytes = BitConverter.GetBytes(value);

            Array.Reverse(ValueBytes);

            Write(ValueBytes);
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer
        /// </summary>
        /// <param name="value">The value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Write(uint value)
        {
            byte[] ValueBytes = BitConverter.GetBytes(value);

            Array.Reverse(ValueBytes);

            Write(ValueBytes);
        }

        /// <summary>
        /// Writes a 64-bit unsigned integer
        /// </summary>
        /// <param name="value">The value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Write(ulong value)
        {
            byte[] ValueBytes = BitConverter.GetBytes(value);

            Array.Reverse(ValueBytes);

            Write(ValueBytes);
        }

        /// <summary>
        /// Writes a boolean value
        /// </summary>
        /// <param name="value">The value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Write(bool value)
        {
            byte ValueByte = 0x00;

            if (value == true)
            {
                ValueByte = 0xFF;
            }

            Write(ValueByte);
        }

        /// <summary>
        /// Writes the usage flag for an optional field
        /// </summary>
        /// <param name="value">Whether or not the field is used</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void WriteUsageFlag(bool value)
        {
            byte ValueByte = 0x00;

            if (value == true)
            {
                ValueByte = 0x01;
            }

            Write(ValueByte);
        }

        /// <summary>
        /// Writes the specified enumeration to the stream
        /// </summary>
        /// <param name="value">The enumerated value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public void WriteEnum<T>(T value)
        {
            Type EnumType = value.GetType();

            if (EnumType.IsEnum && EnumType.GetEnumUnderlyingType() == typeof(byte))
            {
                Write(Convert.ToByte((object)value));
            }
            else
            {
                throw new ArgumentException("WriteEnum may only be performed on enumerations with an underlying type of byte");
            }
        }

        /// <summary>
        /// Writes the specified variable length bit string to the stream
        /// </summary>
        /// <param name="value">The enumerated bit string to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public void WriteBitString<T>(T value)
        {
            Type EnumType = value.GetType();

            if (EnumType.IsEnum)
            {
                ulong RawValue = (ulong)(object)value;
                int LeastSignificantBit = 0;
                int MostSignificanBit = 0;
                byte TotalBits = 0;
                int BytesToWrite = 0;
                byte[] DataToWrite;
                byte[] RawValueBytes = BitConverter.GetBytes(RawValue);

                // We need to determine the minimum and maximum used values
                T[] EnumValues = Enum.GetValues(EnumType).Cast<T>().ToArray();

                // The first and last values should be the smallest and largest values respectively.
                // Find the smallest used bit
                for (int CurrentBit = 0; CurrentBit < 64; CurrentBit++)
                {
                    ulong CurrentMask = (ulong)(1 << CurrentBit);

                    if ((Convert.ToUInt64((object)EnumValues[0]) & CurrentMask) == CurrentMask)
                    {
                        // We found the least significant bit
                        LeastSignificantBit = CurrentBit;
                    }
                }

                // Find the largest used bit
                for (int CurrentBit = 63; CurrentBit >= 0; CurrentBit--)
                {
                    ulong CurrentMask = (ulong)(1 << CurrentBit);

                    if ((Convert.ToUInt64((object)EnumValues[EnumValues.Length - 1]) & CurrentMask) == CurrentMask)
                    {
                        // We found the most significant bit
                        MostSignificanBit = CurrentBit;
                    }
                }

                // The most significant bit should always be evenly divisible by 8 since bit strings should be filled in
                // from MSB to LSB. If it's not then we need to adjust the value so it is.
                if (MostSignificanBit % 8 > 0)
                {
                    MostSignificanBit += 8 - (MostSignificanBit % 8);
                }

                TotalBits = (byte)(MostSignificanBit - LeastSignificantBit);

                BytesToWrite = (TotalBits / 8) + 1; // Add one for the length

                // Add one byte if the size is not evenly divisible by 8
                if (TotalBits % 8 > 0)
                {
                    BytesToWrite++;
                }

                DataToWrite = new byte[BytesToWrite];

                // The first byte is the length. We should never have more than 64-bits since due to limitations of enums
                // so this will always just be one byte
                DataToWrite[0] = TotalBits;

                // The MSB will always be evenly divisible by 8. We subtract 1 from BytesToWrite because of the length byte
                Array.Copy(RawValueBytes, RawValueBytes.Length - (MostSignificanBit / 8), DataToWrite, 0, BytesToWrite - 1);

                Write(DataToWrite);
            }
            else
            {
                throw new ArgumentException("WriteBitString may only be performed on enumerations.");
            }
        }

        /// <summary>
        /// Writes the specified fixed size bit string to the stream
        /// </summary>
        /// <param name="value">The enumerated bit string to write</param>
        /// <param name="size">The size of the bit string</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public void WriteBitString<T>(T value, int size)
        {
            Type EnumType = value.GetType();

            if (EnumType.IsEnum)
            {
                ulong RawValue = Convert.ToUInt64((object)value);

                if (size <= 64)
                {
                    int BytesToWrite = size / 8;
                    byte[] DataToWrite;

                    // GetBytes will convert to a big endian byte[] so reverse it
                    byte[] RawValueBytes = BitConverter.GetBytes(RawValue).Reverse().ToArray(); 

                    // Add one byte if the size is not evenly divisible by 8
                    if (size % 8 > 0)
                    {
                        BytesToWrite++;
                    }

                    DataToWrite = new byte[BytesToWrite];

                    // Copy the bytes from the raw value starting from the end to make sure we copy the correct bytes
                    for (int IndexFromEnd = 0; IndexFromEnd < DataToWrite.Length; IndexFromEnd++)
                    {
                        DataToWrite[DataToWrite.Length - IndexFromEnd - 1] = RawValueBytes[RawValueBytes.Length - IndexFromEnd - 1];
                    }

                    Write(DataToWrite);
                }
                else
                {
                    throw new ArgumentException("WriteBitString can not be used to write bit strings longer than 64 bits");
                }
            }
            else
            {
                throw new ArgumentException("WriteBitString may only be performed on enumerations.");
            }
        }

        /// <summary>
        /// Writes the fixed length bit string to the stream
        /// </summary>
        /// <param name="value">The bit string to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void WriteBitString(byte[] value)
        {
            // There is no length so just write the entire array
            Write(value);
        }

        /// <summary>
        /// Writes the variable length bit string to the stream
        /// </summary>
        /// <param name="value">The bit string to write</param>
        /// <param name="size">The size of the bit string in bits</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void WriteBitString(byte[] value, int size)
        {
            if (value != null && size > 0)
            {
                int DataLength = size / 8;

                if (size % 8 > 0)
                {
                    DataLength++;
                }

                if (DataLength != value.Length)
                {
                    throw new ArgumentException("The size value is incorrect for the given bit string");
                }

                WriteLength(size);

                // We have written the length so now we can write the bit string
                Write(value);
            }
        }

        /// <summary>
        /// Writes the specified bytes string including the length of the array
        /// </summary>
        /// <param name="value">The byte string to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void WriteBytesWithLength(byte[] value)
        {
            if (value != null)
            {
                WriteLength(value.Length);

                // Write the data
                Write(value);
            }
        }

        /// <summary>
        /// Writes a visible string to the stream
        /// </summary>
        /// <param name="value">The value to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public override void Write(string value)
        {
            WriteBytesWithLength(ASCIIEncoding.ASCII.GetBytes(value));
        }

        /// <summary>
        /// Writes a length value to the stream
        /// </summary>
        /// <param name="value">The length to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void WriteLength(int value)
        {
            if (value < 128)
            {
                // The length is only one byte
                Write((byte)value);
            }
            else
            {
                int LengthBytes = (int)(Math.Ceiling(Math.Log(value) / Math.Log(2)) / 8) + 1;

                // Write the length
                Write((byte)(LengthBytes + 128));

                // Now write the actual length
                for (int iIndex = LengthBytes - 1; iIndex >= 0; iIndex--)
                {
                    Write((byte)(value >> (8 * iIndex)));
                }
            }
        }

        /// <summary>
        /// Writes the COSEM date time to the stream
        /// </summary>
        /// <param name="dateTime">The COSEM Date Time to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Write(COSEMDateTime dateTime)
        {
            Write(dateTime.Data);
        }

        /// <summary>
        /// Writes the COSEM date to the stream
        /// </summary>
        /// <param name="date">The COSEM date to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Write(COSEMDate date)
        {
            Write(date.Data);
        }

        /// <summary>
        /// Writes the COSEM time to the stream
        /// </summary>
        /// <param name="time">The COSEM time to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Write(COSEMTime time)
        {
            Write(time.Data);
        }

        #endregion
    }
}
