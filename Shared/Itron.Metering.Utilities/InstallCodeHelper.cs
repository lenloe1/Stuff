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
//                              Copyright © 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// A helper class that will convert HAN device installation codes to a 16 byte link key which can then be used to add the
    /// device to a specific meter's network.
    /// </summary>
    public class InstallCodeHelper
    {
        #region Definitions

        /// <summary>
        /// Possible Install Code Sizes
        /// </summary>
        public enum InstallCodeSize
        {
            /// <summary>48 Bits - 6 Bytes</summary>
            Size48Bits = 6,
            /// <summary>64 Bits - 8 Bytes</summary>
            Size64Bits = 8,
            /// <summary>96 Bits - 12 Bytes</summary>
            Size96Bits = 12,
            /// <summary>128 Bits - 16 Bytes</summary>
            Size128Bits = 16,
        }

        #endregion

        #region Constants

        private const int SECURITY_BLOCK_SIZE = 16;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Generates a new Install Code with the specified length
        /// </summary>
        /// <param name="size">The size of the Install Code to generate</param>
        /// <returns>The new install code</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/11 RCG 2.52.10        Created
        
        public static byte[] GenerateInstallCode(InstallCodeSize size)
        {
            RNGCryptoServiceProvider RandomGenerator = new RNGCryptoServiceProvider();
            byte[] NewCode = new byte[(int)size];
            byte[] InstallCode = new byte[(int)size + 2];
            CrcHelper CrcHelper = new CrcHelper(CrcAlgorithmType.Crc16Ccitt);
            ushort CRC = 0;

            RandomGenerator.GetBytes(NewCode);

            CRC = (ushort)CrcHelper.CalculateCrc(NewCode);

            Array.Copy(NewCode, 0, InstallCode, 0, NewCode.Length);

            InstallCode[InstallCode.Length - 2] = (byte)CRC;
            InstallCode[InstallCode.Length - 1] = (byte)(CRC >> 8);

            return InstallCode;
        }

        /// <summary>
        /// Generates the link key from the specified Installation Code
        /// </summary>
        /// <param name="installationCode">The installation Code</param>
        /// <returns>The resulting Link Key</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/11 RCG 2.52.10        Created

        public static byte[] GenerateLinkKey(byte[] installationCode)
        {
            byte[] LinkKey = null;

            if (installationCode != null)
            {
                switch (installationCode.Length)
                {
                    case 8:
                    case 10:
                    case 14:
                    case 18:
                    {
                        CrcHelper CrcHelper = new CrcHelper(CrcAlgorithmType.Crc16Ccitt);
                        ushort CRC = (ushort)(installationCode[installationCode.Length - 2] + (installationCode[installationCode.Length - 1] << 8));
                        byte[] Code = new byte[installationCode.Length - 2];

                        Array.Copy(installationCode, Code, Code.Length);

                        // Validate the CRC
                        if (CRC == CrcHelper.CalculateCrc(Code))
                        {
                            LinkKey = MMOHash(Code);
                        }
                        else
                        {
                            throw new ArgumentException("The provided installation code has an invalid CRC.", "installationCode");
                        }

                        break;
                    }
                    default:
                    {
                        throw new ArgumentException("The provided installation code is invalid.", "installationCode");
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("installationCode", "The Installation Code may not be null");
            }

            return LinkKey;
        }

        /// <summary>
        /// Generates a 16 byte link key from the provided installation code.
        /// </summary>
        /// <param name="installationCode">A valid 48, 64, 96 or 128 bit hex installation code with a 16 bit CRC checksum.</param>
        /// <returns>A 16 byte link key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <i>installationCode</i> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <i>installationCode</i> is not a valid installation code.</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/17/09 MMD 2.40.01        Created
        //  08/30/11 RCG 2.52.10        Modified to call GenerateLinkKey(byte[])

        public static byte[] GenerateLinkKey(string installationCode)
        {
            byte[] LinkKey = null;

            if (String.IsNullOrEmpty(installationCode.Trim()) == false)
            {
                LinkKey = GenerateLinkKey(HexStringToByteArray(installationCode.Replace(" ", "")));
            }
            else
            {
                throw new ArgumentException("The installation code must not be null, empty, or contain only whitespace", "installationCode");
            }

            return LinkKey;
        }

        /// <summary>
        /// Returns a <see cref="Byte"/> array that is the byte of the provided input.
        /// </summary>
        /// <param name="hexString">The source that will be used to generate the byte value.</param>
        /// <returns>A <see cref="Byte"/> array that contains the byte value of the provided input.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/17/09 MMD 2.40.01        Created

        public static byte[] HexStringToByteArray(string hexString)
        {
            byte[] ar = new byte[hexString.Length / 2];
            char[] car = hexString.ToCharArray();

            for (int i = 0, j = ar.Length, k = 0; i < j; i++, k += 2)
            {
                ar[i] = byte.Parse(string.Format(CultureInfo.InvariantCulture, "{0}{1}", car[k], car[k + 1]), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }

            return ar;
        }

        #endregion

        # region Private Static Methods

        /// <summary>
        /// Returns a <see cref="Byte"/> array that is the MMO hash of the provided input.
        /// </summary>
        /// <param name="input">The source that will be used to generate the MMO hash return value.</param>
        /// <returns>A <see cref="Byte"/> array that contains the MMO hash of the provided input.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/17/09 MMD 2.40.01        Created
        
        private static byte[] MMOHash(byte[] input)
        {
            byte[] hashResult = new byte[InstallCodeHelper.SECURITY_BLOCK_SIZE];
            byte[] codeAndCrc = new byte[18];

            Array.Copy(input, codeAndCrc, input.Length);

            CrcHelper crcHelper = new CrcHelper(CrcAlgorithmType.Crc16Ccitt);
            ulong crcVal = crcHelper.CalculateCrc(input);

            codeAndCrc[input.Length] = (byte)(crcVal & 0xFF);
            codeAndCrc[input.Length + 1] = (byte)(crcVal >> 8);

            InstallCodeHelper.AesHash(codeAndCrc, input.Length + 2, hashResult);

            return hashResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="totalLength"></param>
        /// <param name="result"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/17/09 MMD 2.40.01        Created
        
        private static void AesHash(byte[] input, int totalLength, byte[] result)
        {
            byte[] temp = new byte[InstallCodeHelper.SECURITY_BLOCK_SIZE];
            int moreDataLength = totalLength;

            int index = 0;
            for (; InstallCodeHelper.SECURITY_BLOCK_SIZE <= moreDataLength; index += InstallCodeHelper.SECURITY_BLOCK_SIZE, moreDataLength -= InstallCodeHelper.SECURITY_BLOCK_SIZE)
            {
                InstallCodeHelper.AesHashNextBlock(input, result);
            }

            Array.Copy(input, index, temp, 0, moreDataLength);
            temp[moreDataLength] = 0x80;

            if ((InstallCodeHelper.SECURITY_BLOCK_SIZE - moreDataLength) < 3)
            {
                InstallCodeHelper.AesHashNextBlock(temp, result);
                Array.Clear(temp, 0, temp.Length);
            }

            temp[temp.Length - 2] = (byte)(totalLength >> 5);
            temp[temp.Length - 1] = (byte)(totalLength << 3);

            InstallCodeHelper.AesHashNextBlock(temp, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="result"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/17/09 MMD 2.40.01        Created
        
        private static void AesHashNextBlock(byte[] block, byte[] result)
        {
            byte[] key = new byte[InstallCodeHelper.SECURITY_BLOCK_SIZE];

            Array.Copy(result, key, InstallCodeHelper.SECURITY_BLOCK_SIZE);
            Array.Copy(block, result, InstallCodeHelper.SECURITY_BLOCK_SIZE);

            InstallCodeHelper.StandAloneEncryptBlock(key, result);

            for (int i = 0; i < InstallCodeHelper.SECURITY_BLOCK_SIZE; i++)
            {
                result[i] ^= block[i];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="block"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/17/09 MMD 2.40.01        Created
        
        private static void StandAloneEncryptBlock(byte[] key, byte[] block)
        {
            byte[] outBlock = InstallCodeHelper.EncryptAes(block, key, new byte[16], InstallCodeHelper.SECURITY_BLOCK_SIZE * 8);
            Array.Copy(outBlock, block, outBlock.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="key"></param>
        /// <param name="IV"></param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/17/09 MMD 2.40.01        Created
        
        private static byte[] EncryptAes(byte[] buffer, byte[] key, byte[] IV, int keySize)
        {
            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;

            RijndaelManaged aesAlg = null;

            try
            {
                aesAlg = new RijndaelManaged();
                aesAlg.BlockSize = keySize;
                aesAlg.KeySize = keySize;
                aesAlg.Key = key;
                aesAlg.IV = IV;
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Mode = CipherMode.CBC;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

                //Write all data to the stream.
                csEncrypt.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                if (csEncrypt != null)
                {
                    csEncrypt.Close();
                }

                if (msEncrypt != null)
                {
                    msEncrypt.Close();
                }

                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();
        }

        #endregion
    }
}
