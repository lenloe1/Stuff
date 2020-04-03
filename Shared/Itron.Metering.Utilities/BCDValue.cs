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
//                              Copyright © 2006 - 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Globalization;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// This class provides static methods to help convert BCD (Binary coded 
    /// Decimal) values into native C# data types
    /// </summary>
    public class BCD
    {
		/// <summary>
		/// Private constructor to prevent anyone from attempting to create an 
		/// object from this class
		/// </summary>
		private BCD()
		{
		}


        /// <summary>
        /// Used to convert a single byte BCD value into a byte value
        /// </summary>
        /// <param name="bcdValue">
        /// The BCD digits to convert
        /// </param>
        /// <returns>
        /// A byte containing the numeric value of the BCD values
        /// </returns>
        static public byte BCDtoByte(byte bcdValue)
        {
            // Lower nibble transfers directly while upper nibble is the tens digit

            return (byte)((bcdValue & 0x0F) + ((bcdValue >> 4) * 10));
        }

        /// <summary>
        /// Converts a BCD array to an integer value
        /// </summary>
        /// <param name="bcdArray">
        /// The BCD array to convert
        /// </param>
        /// <param name="BytesToConvert">
        /// The number of BCD bytes to convert.
        /// </param>
        /// <returns>
        /// The equivalent integer value 
        /// </returns>
        /// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 11/11/08 mah 9.50.00  N/A   Corrected CQ # 121181 by changing the loop limit
		/// </remarks>
        static public int BCDtoInt(ref byte[] bcdArray, int BytesToConvert)
        {
            int result = 0;
            int Multiplier;
            int PowerOfTen = 0;

            if (BytesToConvert > bcdArray.Length)
            {
                throw new ArgumentException(
                    "The number of bytes to convert is larger than the number of bytes available");
            }

            for (int Index = BytesToConvert - 1 ; Index >= 0; Index--)
            {
                Multiplier = (int)Math.Pow(10.0, PowerOfTen);
                result += (BCDtoByte(bcdArray[Index]) * Multiplier);
                PowerOfTen += 2;    // 2 decimal digits represented by each byte
            }

            return result;
        }

        /// <summary>
        /// Converts a fixed floating point BCD value into 
        /// a true floating point value
        /// </summary>
        /// <param name="bcdArray">
        /// The incoming BCD value in the format of XX XX XX. XX XX XX XX
        /// where the decimal point is implied
        /// </param>
        /// <returns>
        /// The equivalent floating point value
        /// </returns>
        static public float FixedBCDtoFloat(ref byte[] bcdArray)
        {
            float result = (float)0.0;

            // will be converting XX XX XX . XX XX XX XX

            result = (float)(BCDtoByte(bcdArray[0]) * 10000.0);
            result += (float)(BCDtoByte(bcdArray[1]) * 100.0);
            result += (float)(BCDtoByte(bcdArray[2]));
            result += (float)(BCDtoByte(bcdArray[3]) / 100.0);
            result += (float)(BCDtoByte(bcdArray[4]) / 10000.0);
            result += (float)(BCDtoByte(bcdArray[5]) / 1000000.0);
            result += (float)(BCDtoByte(bcdArray[6]) / 100000000.0);

            return result;
        }

        /// <summary>
        /// This method converts a double to a BCD values in the form of :
        ///    XX XX XX. XX XX XX XX Decimal implied
        /// </summary>
        /// <param name="dblValue">The double to convert</param>
        /// <param name="nTotalBytes">The length of the BCD array in bytes</param>
        /// <param name="nDecimalBytes">The number of BCD bytes right of the
        /// implied decimal point</param>
        /// <returns>An array containing BCD Value</returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/26/07 KRC 8.00.09         Adding Edit Registers
        /// 07/05/07 mcm 8.10.11        Rewrote because it only worked for one size
        /// </remarks>
        static public byte[] DoubleToFixedBCD(double dblValue, int nTotalBytes, 
            int nDecimalBytes)
        {
            string strValue = dblValue.ToString("0.0", CultureInfo.CurrentCulture );

            return StringToFixedBCD(strValue, nTotalBytes, nDecimalBytes);
        }

        /// <summary>
        /// This method converts a string to a BCD values in the form of :
        ///    XX XX XX. XX XX XX XX Decimal implied
        /// </summary>
        /// <param name="strValue">The string to convert</param>
        /// <param name="nTotalBytes">The length of the BCD array in bytes</param>
        /// <param name="nDecimalBytes">The number of BCD bytes right of the
        /// implied decimal point</param>
        /// <returns>An array containing BCD Value</returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/26/07 KRC 8.00.09         Adding Edit Registers
        /// 07/05/07 mcm 8.10.11        Rewrote because it only worked for one size
        /// </remarks>
        static public byte[] StringToFixedBCD(string strValue, int nTotalBytes, 
            int nDecimalBytes)
        {
            byte[] byArray = new byte[nTotalBytes];
            int nLeftDigit = 0;
            int nRightDigit = 0;
            int nByteIndex = 0;
            int nDecimalIndex  = strValue.IndexOf('.');
            string strLeft = "";
            string strRight = "";


            // Make sure the input string has a decimal point
            if (nDecimalIndex < 0)
            {
                strValue = strValue + ".0";
                nDecimalIndex = strValue.IndexOf('.');
            }

            strLeft = strValue.Substring(0, nDecimalIndex);
            strRight = strValue.Substring(nDecimalIndex + 1,
                strValue.Length - nDecimalIndex - 1);

            // Initialize all the bytes to 0
            byArray.Initialize();

            // Fill the left of the decimal starting at the decimal
            for (nByteIndex = nTotalBytes - nDecimalBytes - 1; nByteIndex >= 0; nByteIndex--)
            {
                // Get the last digit, which will be the lower nibble.
				nRightDigit = int.Parse(strLeft.Substring(strLeft.Length - 1, 1), CultureInfo.InvariantCulture);

                nLeftDigit = 0;
                if (strLeft.Length > 1)
                {
                    // Get the next digit, which will be the upper nible.
					nLeftDigit = int.Parse(strLeft.Substring(strLeft.Length - 2, 1), CultureInfo.InvariantCulture);
                }

                // Now that we have both nibbles we can build the full byte.
                byArray[nByteIndex] = (byte)((nLeftDigit * 16) + nRightDigit);

                if (strLeft.Length <= 2)
                {
                    // We've processed all of the digits in the string.
                    break;
                }
                else
                {
                    // consume the digits we just processed
                    strLeft = strLeft.Substring(0, strLeft.Length - 2);
                }
            }
            
            // Fill the decimal starting at the decimal
            for (nByteIndex = nTotalBytes - nDecimalBytes; nByteIndex < nTotalBytes; nByteIndex++)
            {
                nLeftDigit = 0;
                if (0 < strRight.Length)
                {
                    // Get the next digit, which will be the upper nible.
					nLeftDigit = int.Parse(strRight.Substring(0, 1), CultureInfo.InvariantCulture);
                }

                nRightDigit = 0;
                if (strRight.Length > 1)
                {
                    // Get the last digit, which will be the lower nibble.
					nRightDigit = int.Parse(strRight.Substring(1, 1), CultureInfo.InvariantCulture);
                }

                // Now that we have both nibbles we can build the full byte.
                byArray[nByteIndex] = (byte)((nLeftDigit * 16) + nRightDigit);

                if (strRight.Length <= 2)
                {
                    // We've processed all of the digits in the string.
                    break;
                }
                else
                {
                    // consume the digits we just processed
                    strRight = strRight.Substring(2, strRight.Length - 2);
                }
            }

            return byArray;
        }

        /// <summary>
        /// Converts a fixed floating point BCD value into 
        /// a string representation of the floating point value
        /// </summary>
        /// <param name="bcdArray">
        /// The incoming BCD value in the format of XX XX XX. XX XX XX XX
        /// where the decimal point is implied
        /// </param>
        /// <param name="nDecimalBytes">Number of bytes of decimal digits in BCD array</param>
        /// <param name="nLength">Number of bytes in BCD array</param>
        /// <returns>
        /// The string representation of the BCD value
        /// </returns>
        static public String FixedBCDtoString(byte[] bcdArray, int nDecimalBytes, int nLength)
        {
            // will be converting XX XX XX . XX XX XX XX

            char chrMSN; // Most significant nibble
            char chrLSN; // Least significant nibble

            String strResult = "";

            // First get the integral part of the BCD value

            for (int nArrayIndex = 0; nArrayIndex < nLength - nDecimalBytes; nArrayIndex++)
            {
                chrMSN = (char)((bcdArray[nArrayIndex] >> 4) + '0');
                chrLSN = (char)((bcdArray[nArrayIndex] & 0x0F) + '0');

                strResult += chrMSN.ToString(CultureInfo.InvariantCulture) + chrLSN.ToString(CultureInfo.InvariantCulture);
            }

            // Add the decimal point

            strResult += ".";

            // Now translate the decimal digits

            for (int nArrayIndex = nLength - nDecimalBytes; nArrayIndex < nLength; nArrayIndex++)
            {
                chrMSN = (char)((bcdArray[nArrayIndex] >> 4) + '0');
                chrLSN = (char)((bcdArray[nArrayIndex] & 0x0F) + '0');

                strResult += chrMSN.ToString(CultureInfo.InvariantCulture) + chrLSN.ToString(CultureInfo.InvariantCulture);
            }

            return strResult;
        }



        /// <summary>
        /// This method converts BCD values in the form of :
        ///    M.X XX XX XX where M is a positve exponent, 0-6- and the decimal point is implied
        /// </summary>
        /// <param name="bcdArray">The array of BCD digits</param>
        /// <param name="nBCDLength">The length of the BCD value  - up to 6</param>
        /// <returns>A double precision floating point value</returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        ///- -------- --- ------- ------ ---------------------------------------
        /// 11/22/06 mah 8.00.00  N/A   Created
        /// 11/27/06 jrf 8.00.00  N/A   Modified to handle bcd length as low as 3
        /// 11/28/06 jrf 8.00.00  N/A   Changed computation of nExponent to divide by
        ///                             0x0F instead of 10 and also modified final 
        ///                             computation to use Math.Pow().
        /// </remarks>
        static public double FloatingBCDtoDouble(ref byte[] bcdArray, int nBCDLength )
        {
            double result = (double)0.0;

            int nExponent = bcdArray[0] / 0x0F;

            result = (double)((bcdArray[0] & 0x0F) / 10.0); // the lower nibble contains the first digit
            result += (double)(BCDtoByte(bcdArray[1]) / 1000.0);
            
            if (nBCDLength >= 3)
            {
                result += (double)(BCDtoByte(bcdArray[2]) / 100000.0);
            }
            
            if (nBCDLength >= 4)
            {
                result += (double)(BCDtoByte(bcdArray[3]) / 10000000.0);
            }
            
            if (nBCDLength >= 5)
            {
                result += (double)(BCDtoByte(bcdArray[4]) / 1000000000.0);
            }

            if (nBCDLength >= 6)
            {
                result += (double)(BCDtoByte(bcdArray[5]) / 100000000000.0);
            }

            if (nExponent > 0)
            {
                result = result * Math.Pow(10.0, (double)nExponent);
            }

            return result;
        }

        /// <summary>
        /// This method converts a double to a BCD values in the form of :
        ///    M.X XX XX XX where M is a positve exponent, 0-6- and the decimal point is implied
        /// </summary>
        /// <param name="dblValue">The double to convert</param>
        /// <param name="nLength">The length of the BCD value  - up to 6</param>
        /// <returns>A BCD Value</returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        ///- -------- --- ------- ------ ---------------------------------------
        /// 01/26/07 KRC 8.00.09         Adding Edit Registers
        /// </remarks>
        static public byte[] DoubleToFloatingBCD(double dblValue, int nLength)
        {
            byte[] byArray = new byte[nLength];
            int nArrayCount = 0;
            int nLeftDigit = 0;
            int nRightDigit = 0;
            int nExponent = 0;
            int nNextIndex = 0;
            int nStartIndex = 0;
            string strValue = dblValue.ToString("000000000000.00000000", CultureInfo.InvariantCulture );   // Enough zeros for shifting
            int nDecimalPostition = strValue.IndexOf('.');
                        
            // Loop through, looking for the first non-zero item before the decimal point
            for(int nNonZeroIndex = 0; nNonZeroIndex < nDecimalPostition; nNonZeroIndex++)
            {
                if("0" != strValue.Substring(nNonZeroIndex, 1))
                {
                    // We have found the index of the first non-zero item
                    nStartIndex = nNonZeroIndex;
                    // We can now calculate the exponent
                    nExponent = nDecimalPostition - nStartIndex;
                    break;
                }
            }

            // If the exponent still equals zero this means there was not a non-zero values to the left of the decimal.
            //  The exponent will stay zero and we need to set the starting position to right after the decimal to read
            //  the remaining numbers.
            if (0 == nExponent)
            {
                nStartIndex = nDecimalPostition + 1;
            }

            // At this point we have the exponent and we have the starting position of the remaining part of the number.  Now
			nRightDigit = int.Parse(strValue.Substring(nStartIndex, 1), CultureInfo.InvariantCulture);
            byArray[nArrayCount] = (byte)((nExponent * 16) + nRightDigit);

            nNextIndex = nStartIndex + 1;
            
            // Loop through the string until we have filled up our entire array
            //  don't forget to skip the decimal point when you come across it.
            for (nArrayCount = 1; nArrayCount < nLength; nArrayCount++)
            {
                if ("." == strValue.Substring(nNextIndex, 1))
                {
                    // If the next item to read is the deciaml point, lets skip it.
                    nNextIndex++;
                }
                // Get the next digit, which will be the upper nible.
				nLeftDigit = int.Parse(strValue.Substring(nNextIndex, 1), CultureInfo.InvariantCulture);
                nNextIndex++;
                
                if ("." == strValue.Substring(nNextIndex, 1))
                {
                    // If the next item to read is the deciaml point, lets skip it.
                    nNextIndex++;
                }
                // Get the next digit, which will be the lower nibble.
				nRightDigit = int.Parse(strValue.Substring(nNextIndex, 1), CultureInfo.InvariantCulture);
                nNextIndex++;

                // Now that we have both nibbles we can build the full byte.
                byArray[nArrayCount] = (byte)((nLeftDigit * 16) + nRightDigit);
            }

            return byArray;
        }
        /// <summary>
        /// This method converts BCD values in the form of :
        ///    M.X XX XX XX where M is a positve exponent, 0-6- and the decimal point is implied
        /// </summary>
        /// <param name="bcdArray">The array of BCD digits</param>
        /// <param name="nBCDLength">The length of the BCD value  - up to 6</param>
        /// <returns>
        /// A string representation of the BCD value
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        ///- -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        static public String FloatingBCDtoString(ref byte[] bcdArray, int nBCDLength)
        {
            String strResult;
            
            char chrMSN; // Most significant nibble
            char chrLSN; // Least significant nibble
            
            int nDecimalPosition = bcdArray[0] / 0x0F;
            chrLSN = (char)((bcdArray[0] & 0x0F) + '0');

            if (0 == nDecimalPosition)
            {
                strResult = "0." + chrLSN.ToString(CultureInfo.InvariantCulture); 
            }
            else
            {
                strResult = chrLSN.ToString(CultureInfo.InvariantCulture);

                if (1 == nDecimalPosition)
                {
                    strResult += ".";
                }
            }

            for (int nArrayIndex = 1; nArrayIndex < nBCDLength; nArrayIndex++)
            {
                chrMSN = (char)((bcdArray[ nArrayIndex ] >> 4) + '0');
                strResult += chrMSN.ToString(CultureInfo.InvariantCulture);

                if (strResult.Length == nDecimalPosition)
                {
                    strResult += ".";
                }

                chrLSN = (char)((bcdArray[nArrayIndex] & 0x0F) + '0');
                strResult += chrLSN.ToString(CultureInfo.InvariantCulture);

                if (strResult.Length == nDecimalPosition)
                {
                    strResult += ".";
                }
            }

            return strResult;
        }

        /// <summary>
        /// This method converts a single byte value into its equivalent BCD value
        /// </summary>
        /// <param name="byteValue">
        /// The value to be converted to a BCD value.  Must be between 0 and 99
        /// </param>
        /// <returns>
        /// A BCD value
        /// </returns>
        static public byte BytetoBCD(byte byteValue)
        {
            // Note that BCD values cannot be larger than 99 decimal

            if (byteValue > 99)
                return 0;
            else
            {
                byte upperNibble = (byte)(byteValue / 10);
                byte lowerNibble = (byte)(byteValue - (upperNibble * 10));

                return (byte)((upperNibble << 4) + lowerNibble);
            }
        }

		/// <summary>
		/// Converts an unsigned integer value to a BCD array
		/// </summary>
		/// <param name="nValue">
		/// The integer value to convert
		/// </param>
		/// <param name="lengthBCDArray">
		/// The number of bytes in the resulting BCD array - up to 3 bytes are allowed
		/// </param>
		/// <returns>
		/// The equivalent BCD array 
		/// </returns>
		static public byte[] InttoBCD(uint nValue, int lengthBCDArray)
		{
			if (lengthBCDArray <= 0 || lengthBCDArray > 3)
			{
				throw (new ArgumentOutOfRangeException("lengthBCDArray", "Illegal number of digits in BCD array"));
			}

			byte[] byArray = new byte[lengthBCDArray];            

            // mcm 03/27/07 - reimplemented because it didn't work.
            for (int Index = lengthBCDArray - 1; Index >= 0; Index--)
            {
                byArray[Index] = BytetoBCD((byte)(nValue % 100));
                nValue = nValue / 100;
			}

			return byArray;
		}

        /// <summary>
        /// 
        /// </summary>
        public enum BCDDateTimeFormat
        {
            /// <summary>
            /// 
            /// </summary>
            YrMoDaHrMiSeDow,
            /// <summary>
            /// 
            /// </summary>
            MoDaHrMi
        }

        /// <summary>
        /// Converts a byte array to a DateTime
        /// </summary>
        /// <param name="bcdDateTimeArray"></param>
        /// <param name="bcdFormat"></param>
        /// <returns>DateTime</returns>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///                              Created
        ///  10/18/06 mrj 7.35.05 44     Set the default time to 1/1/1980 to match
        ///                              Pc-Pro+
        ///
        /// </remarks>
        static public DateTime GetDateTime(ref byte[] bcdDateTimeArray, BCDDateTimeFormat bcdFormat)
        {
            DateTime dateTime;

            switch (bcdFormat)
            {
                case BCDDateTimeFormat.YrMoDaHrMiSeDow:
                {
                    try
                    {
                        dateTime = new DateTime(BCDtoByte(bcdDateTimeArray[0]) + 2000,
                                           BCDtoByte(bcdDateTimeArray[1]),
                                           BCDtoByte(bcdDateTimeArray[2]),
                                           BCDtoByte(bcdDateTimeArray[3]),
                                           BCDtoByte(bcdDateTimeArray[4]),
                                           BCDtoByte(bcdDateTimeArray[5]));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // The datetime have not been set - set it to 1/1/1980
                        // to match Pc-Pro+
                        dateTime = new DateTime(1980, 1, 1);
                    }

                    break;
                }
            case BCDDateTimeFormat.MoDaHrMi:
                {
                    try
                    {
                        dateTime = new DateTime(DateTime.Now.Year,
                                           BCDtoByte(bcdDateTimeArray[0]),
                                           BCDtoByte(bcdDateTimeArray[1]),
                                           BCDtoByte(bcdDateTimeArray[2]),
                                           BCDtoByte(bcdDateTimeArray[3]),
                                           0);

                        if (dateTime > DateTime.Now)
                        {
                            dateTime.AddYears(-1);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // The datetime have not been set - set it to 1/1/1980
                        // to match Pc-Pro+
                        dateTime = new DateTime(1980, 1, 1);
                    }

                    break;
                }

                default:
                {
                    //Set default to 1/1/1980 to match Pc-Pro+
                    dateTime = new DateTime(1980, 1, 1);
                    break;
                }
            }

            return dateTime;
        }
    }

}
