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
//                              Copyright © 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// This class is not extensively tested, so use it at your own risk.
    /// This class is currently used for debugging and development.
    /// </summary>
    public class HexEncoding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static int GetByteCount(string hexString)
        {
            int numHexChars = 0;
            char c;


            if (null != hexString)
            {

                // remove all none A-F, 0-9, characters
                for (int i = 0; i < hexString.Length; i++)
                {
                    c = hexString[i];
                    if (IsHexDigit(c))
                        numHexChars++;
                }
                // if odd number of characters, discard last character
                if (numHexChars % 2 != 0)
                {
                    numHexChars--;
                }
            }

            return numHexChars / 2; // 2 characters per byte
        }

        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored. 
        /// </summary>
        /// <param name="hexString">string to convert to byte array</param>
        /// <param name="inputBuffer">Original buffer for offset writes</param>
        /// <param name="offset">Offset into the original buffer</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        public static byte[] GetBytes(string hexString, byte[] inputBuffer, int offset, out int discarded)
        {
            discarded = 0;
            List<char> validCharacters = new List<char>();
            char c;

            if (null != hexString)
            {
                // remove all none A-F, 0-9, characters
                for (int i = 0; i < hexString.Length; i++)
                {
                    c = hexString[i];
                    if (IsHexDigit(c))
                        validCharacters.Add(c);
                    else
                        discarded++;
                }
            }

            // if odd number of characters, discard last character
            if (validCharacters.Count % 2 != 0)
            {
                discarded++;
                validCharacters.RemoveAt(validCharacters.Count -1);
            }

            int byteLength = (validCharacters.Count / 2) + offset;
            byte[] bytes = new byte[byteLength];
            bytes = inputBuffer;
            string hex;
            int j = 0;
            for (int i=offset; i < byteLength; i++)
            {
                hex = new String(new Char[] {validCharacters[j], validCharacters[j+1]});
                bytes[i] = HexToByte(hex);
                j = j+2;
            }
            return bytes;
        }

        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored. 
        /// </summary>
        /// <param name="hexString">string to convert to byte array</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        public static byte[] GetBytes(string hexString, out int discarded)
        {
            byte[] bytes = null;
            char c;

            discarded = 0;
            List<char> validCharacters = new List<char>();

            if (null != hexString)
            {
                // remove all none A-F, 0-9, characters
                for (int i = 0; i < hexString.Length; i++)
                {
                    c = hexString[i];
                    if (IsHexDigit(c))
                        validCharacters.Add(c);
                    else
                        discarded++;
                }

                // if odd number of characters, discard last character
                if (validCharacters.Count % 2 != 0)
                {
                    discarded++;
                    validCharacters.RemoveAt(validCharacters.Count - 1);
                }

                int byteLength = validCharacters.Count / 2;
                bytes = new byte[byteLength];
                string hex;
                int j = 0;

                for (int i = 0; i < bytes.Length; i++)
                {
                    hex = new String(new Char[] { validCharacters[j], validCharacters[j + 1] });
                    bytes[i] = HexToByte(hex);
                    j = j + 2;
                }
            }

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="AddSpaces"></param>
        /// <returns></returns>
        public static string ToString(byte[] bytes, bool AddSpaces)
        {
            StringBuilder hexString = new StringBuilder();
            for (int i=0; i<bytes.Length; i++)
            {
                hexString.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));

                if (AddSpaces)
                {
                    hexString.Append(" ");
                }
            }
            return hexString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="iStartIndex"></param>
        /// <param name="iCount"></param>
        /// <param name="AddSpaces"></param>
        /// <returns></returns>
        public static string ToString(byte[] bytes, int iStartIndex, 
            int iCount, bool AddSpaces)
        {
            StringBuilder hexString = new StringBuilder();

            if (bytes.Length >= iStartIndex + iCount)
            {
                for (int i = iStartIndex; i < iStartIndex + iCount; i++)
                {
                    hexString.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));

                    if (AddSpaces)
                    {
                        hexString.Append(" ");
                    }
                }
            }

            return hexString.ToString();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="iStartIndex"></param>
        /// <param name="iCount"></param>
        /// <param name="AddSpaces"></param>
        /// <returns></returns>
        public static string ToReverseString(byte[] bytes, int iStartIndex, 
            int iCount, bool AddSpaces)
        {
            StringBuilder hexString = new StringBuilder();

            if (bytes.Length >= iStartIndex + iCount)
            {
                for (int i = iStartIndex + iCount - 1; i >= iStartIndex; i--)
                {
                    hexString.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));

                    if (AddSpaces)
                    {
                        hexString.Append(" ");
                    }
                }
            }

            return hexString.ToString();
        }
        
        /// <summary>
        /// Creates string with space between bytes of a specified length
        /// </summary>
        /// <param name="bytes">length of the array you want included</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToString(byte[] bytes, int length)
        {
            StringBuilder hexString = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                hexString.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));
                hexString.Append(" ");
            }
            return hexString.ToString();
        }

        /// <summary>
        /// Determines if given string is in proper hexadecimal string format
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static bool InHexFormat(string hexString)
        {
            bool hexFormat = true;

            if (null != hexString)
            {
                foreach (char digit in hexString)
                {
                    if (!IsHexDigit(digit))
                    {
                        hexFormat = false;
                        break;
                    }
                }
            }

            return hexFormat;
        }

        /// <summary>
        /// Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>true if hex digit, false if not</returns>
        public static bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A', CultureInfo.InvariantCulture);
            int num1 = Convert.ToInt32('0', CultureInfo.InvariantCulture);
            c = Char.ToUpper(c, CultureInfo.InvariantCulture);
            numChar = Convert.ToInt32(c, CultureInfo.InvariantCulture);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }

        /// <summary>
        /// Converts 1 or 2 character string into equivalant byte value
        /// </summary>
        /// <param name="hex">1 or 2 character string</param>
        /// <returns>byte</returns>
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            return newByte;
        }
    }
}
