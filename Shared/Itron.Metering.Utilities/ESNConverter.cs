///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential  
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or  
//  otherwise. Including photocopying and recording or in connection with any 
//  information storage or retrieval system without the permission in writing 
//  from Itron, Inc.
//
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Handles conversion of the Electronic Serial Number between the string 
    /// representation and the encoded representation that is stored in the meter.
    /// </summary>
    public class ESNConverter
    {
        #region Constants
        /// <summary>
        /// Bit mask for the most significant bit
        /// </summary>
        private const byte HIGH_BIT = 0x80;

        /// <summary>
        /// Bit mask for the 7 least significant bits
        /// </summary>
        private const byte LOW_BITS = 0x7F;

        /// <summary>
        /// Byte code that says that this value is an Electronic Serial Number
        /// </summary>
        private const byte ESN_ID_CODE = 0x06;

        /// <summary>
        /// Byte code that says this is and ESN or Aptitle with relative object
        /// identifier
        /// </summary>
        private const byte ESN_ID_CODE_2 = 0x0D;


        /// <summary>
        /// The number of bits to shift when decoding
        /// </summary>
        private const int BITS_TO_SHIFT = 7;

        /// <summary>
        /// The length of the Electronic Serial Number in bytes
        /// </summary>
        private const int ESN_LENGTH = 20;

        #endregion





        #region Public Methods

        /// <summary>
        /// Encodes a (UInt64) term of the ESN using BER Basic Encoding Rules for base 128
        /// </summary>
        /// <param name="term">UInt64 value of the term for encoding</param>
        /// <returns>The encoded term as a byte array.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/20/12 BLC         N/A    Created
        public static byte[] EncodeTerm128(UInt64 term)
        {
            List<byte> encoded = new List<byte>();
            byte remander = 0;
            UInt64 newTerm = 0;
            bool firstByte = true;
            byte nextByte = 0;

            while (term != 0)
            {
                remander = (byte)(term % 128);
                newTerm = term / 128;
                if (firstByte)
                {
                    nextByte = remander;
                    firstByte = false;
                }
                else
                {
                    if (newTerm < 2)
                    {
                        if (remander > 0)
                        {
                            nextByte = (byte)(remander | 0x80);
                        }
                        else
                        {
                            nextByte = (byte)(newTerm | 0x80);
                        }
                    }
                    else
                    {
                        nextByte = (byte)(remander | 0x80);
                    }

                }
                encoded.Add(nextByte);
                term = newTerm;
            }
            encoded.Reverse();
            return(encoded.ToArray());
        }


        ///// <summary>
        ///// Encodes an Electronic Serial Number String into a byte format for
        ///// storage in the meter or program.
        ///// </summary>
        ///// <param name="strDecodedESN">String representation of the Electronic Serial Number.</param>
        ///// <returns>The encoded Electronic Serial Number as a byte array.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/15/06 RCG 7.40.00 N/A    Created
        //****************************************************************
        // TODO: NEED TO SUPPORT UINT64 FOR uiCurrentTerm BEFORE USING.***
        //****************************************************************
        //public static byte[] Encode(string strDecodedESN)
        //{
        //    List<byte> byEncodedList = new List<byte>();
        //    Stack<byte> byEncodingStack = new Stack<byte>();
        //    byte[] byaEncodedESN;
        //    string[] strTermStrings = strDecodedESN.Split('.');
        //    uint uiFirstTerm, uiSecondTerm, uiCurrentTerm;
        //    int iIndex = 0;

        //    if (strTermStrings.Length > 2)
        //    {
        //        // Make sure that the first two numbers are valid
        //        uiFirstTerm = Convert.ToUInt32(strTermStrings[iIndex++], CultureInfo.InvariantCulture);
        //        uiSecondTerm = Convert.ToUInt32(strTermStrings[iIndex++], CultureInfo.InvariantCulture);

        //        if ((uiFirstTerm >= 0 && uiFirstTerm <= 2) && (uiSecondTerm >= 0 && uiSecondTerm <= 39))
        //        {
        //            // The first numbers are valid so the first octet is 40x<First> + <Second>
        //            byEncodedList.Add((byte)(40 * uiFirstTerm + uiSecondTerm));

        //            // Now go through the rest of the numbers and encode them
        //            while (iIndex < strTermStrings.Length)
        //            {
        //                uiCurrentTerm = Convert.ToUInt32(strTermStrings[iIndex++], CultureInfo.InvariantCulture);

        //                // We need to determine the bytes from left to right so put them
        //                // in a stack
        //                while (uiCurrentTerm / 128 > 0)
        //                {
        //                    byEncodingStack.Push((byte)(uiCurrentTerm % 128));
        //                    uiCurrentTerm /= 128;
        //                }

        //                // Push the last 7 bits (this also ensures that we handle the case 
        //                // where a term is 0)
        //                byEncodingStack.Push((byte)(uiCurrentTerm % 128));

        //                // All but the last byte needs to have the most significant bit
        //                // set to 1
        //                while (byEncodingStack.Count > 1)
        //                {
        //                    byEncodedList.Add((byte)(byEncodingStack.Pop() | HIGH_BIT));
        //                }

        //                // Add the last byte with a 0 for the most significant bit
        //                // which is ensured by the mod 128 (7 bits)
        //                byEncodedList.Add(byEncodingStack.Pop());
        //            }
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Invalid Electronic Serial Number specified.", "strDecodedESN");
        //        }
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Invalid Electronic Serial Number specified.", "strDecodedESN");
        //    }

        //    // Finally add the ESN code and the size and then return the encoded value
        //    byaEncodedESN = new byte[ESN_LENGTH];

        //    byaEncodedESN[0] = ESN_ID_CODE;
        //    byaEncodedESN[1] = (byte)byEncodedList.Count;

        //    Array.Copy(byEncodedList.ToArray(), 0, byaEncodedESN, 2, byEncodedList.Count);

        //    return byaEncodedESN;

        //}

        /// <summary>
        /// Decodes the Electronic Serial Number in the string form entered by the user
        /// </summary>
        /// <param name="byaEncodedESN">The encoded Electronic Serial Number</param>
        /// <returns>The Electronic Serial Number in string form</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/15/06 RCG 7.35.00 N/A    Created
        // 10/10/06 AF  7.40.00 N/A    Modified to enable relative object identifiers
        //                             and changed accessibility level to make 
        //                             visible to other classes in this assembly

        public static string Decode(byte[] byaEncodedESN)
        {
            byte[] byaESNData;
            int iIndex;
            UInt64 uiCurrentTermValue;
            uint uiTerm1;
            uint uiTerm2;
            List<string> strTermList = new List<string>();
            string strDecodedESN;

            if (byaEncodedESN.Length > 2)
            {
                // First make sure that we are Decoding an ESN
                if ((byaEncodedESN[0] != ESN_ID_CODE) &&
                    (byaEncodedESN[0] != ESN_ID_CODE_2) &&
                    (byaEncodedESN[0] != 0))
                {
                    throw new ArgumentException("INVALID_ESN");
                }

                // Make sure that we have all of the data
                if (byaEncodedESN.Length < (byaEncodedESN[1] + 2))
                {
                    throw new ArgumentException("INVALID_ESN");
                }

                // Extract the data we need
                byaESNData = new byte[byaEncodedESN[1]];
                Array.Copy(byaEncodedESN, 2, byaESNData, 0, byaEncodedESN[1]);

                // Create the strings
                uiCurrentTermValue = 0;

                for (iIndex = 0; iIndex < byaESNData.Length; iIndex++)
                {
                    // The first byte represents the first two terms so we need a special case
                    //if (iIndex == 0)
                    if ((0 == iIndex) && (ESN_ID_CODE == byaEncodedESN[0]))
                    {
                        // Since the first octet is 40 * Term1 + Term2, Term2 is the value % 40
                        // and Term1 is (value - Term2) / 40
                        uiTerm2 = (uint)(byaESNData[iIndex] % 40);
                        uiTerm1 = (uint)((byaESNData[iIndex] - uiTerm2) / 40);

                        // Add the terms to the list
						strTermList.Add(uiTerm1.ToString(CultureInfo.InvariantCulture));
						strTermList.Add(uiTerm2.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        if ((byaESNData[iIndex] & HIGH_BIT) == HIGH_BIT)
                        {
                            // This term has more octets
                            uiCurrentTermValue = uiCurrentTermValue << BITS_TO_SHIFT;
                            uiCurrentTermValue += (UInt64)(byaESNData[iIndex] & LOW_BITS);
                        }
                        else
                        {
                            // This is the last byte in the term
                            uiCurrentTermValue = uiCurrentTermValue << BITS_TO_SHIFT;
                            uiCurrentTermValue += (UInt64)(byaESNData[iIndex] & LOW_BITS);

                            // Add the string to the list
							strTermList.Add(uiCurrentTermValue.ToString(CultureInfo.InvariantCulture));

                            // Reset the current Term value
                            uiCurrentTermValue = 0;
                        }
                    }
                }

                if (0 != byaESNData.Length)
                {
                    // Now that we have all of the terms as strings join the terms together
                    strDecodedESN = String.Join(".", strTermList.ToArray());
                }
                else
                {
                    strDecodedESN = "";
                }
            }
            else
            {
                throw new ArgumentException("INVALID_ARRAY_SIZE");
            }

            return strDecodedESN;
        }

        #endregion
    }
}