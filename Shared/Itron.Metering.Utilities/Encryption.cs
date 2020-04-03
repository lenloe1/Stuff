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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// This class provides static methods to encrypt and decrypt streams.
    /// </summary>
    public class Encryption
    {
        #region Constants

        const int BUFFER_SIZE = 4096;

        #endregion

        #region Public Methods

        /// <summary>
        /// This method decrypts a stream of data based on a given encryption algorithm.
        /// </summary>
        /// <param name="EncryptionAlgorithm">The encryption algorithm that should be used 
        /// to decrypt the stream.</param>
        /// <param name="InStream">The stream that contains the encrypted data.</param>
        /// <param name="OutStream">The stream that will contain the decrypted data.</param>
        /// <remarks>This method assumes the encryption algorithm has be set with an 
        /// appopriate key and initialization vector prior to its calling.</remarks>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created
        //
        public static void DecryptData(SymmetricAlgorithm EncryptionAlgorithm, Stream InStream, Stream OutStream)
        {
            CryptoStream EncryptedStream = null;
            byte[] abyBuffer = new byte[BUFFER_SIZE];
            int iBytesRead = 0;

            try
            {
                EncryptedStream = new CryptoStream(InStream, EncryptionAlgorithm.CreateDecryptor(), CryptoStreamMode.Read);

                while (0 != (iBytesRead = EncryptedStream.Read(abyBuffer, 0, abyBuffer.Length)))
                {
                    OutStream.Write(abyBuffer, 0, iBytesRead);
                }
            }
            catch
            {
                //Do nothing.
            }
            finally
            {
                if (EncryptedStream != null)
                {
                    EncryptedStream.Close();
                }
            }
        }

        /// <summary>
        /// This method encrypts a stream of data based on a given encryption algorithm.
        /// </summary>
        /// <param name="EncryptionAlgorithm">The encryption algorithm that should be used 
        /// to encrypt the stream.</param>
        /// <param name="InStream">The stream that contains the decrypted data.</param>
        /// <param name="OutStream">The stream that will contain the encrypted data.</param>
        /// <remarks>This method assumes the encryption algorithm has be set with an 
        /// appopriate key and initialization vector prior to its calling.</remarks>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created
        //
        public static void EncryptData(SymmetricAlgorithm EncryptionAlgorithm, Stream InStream, Stream OutStream)
        {
            CryptoStream EncryptedStream = null;
            byte[] abyBuffer = new byte[BUFFER_SIZE];
            int iBytesRead = 0;

            try
            {
                EncryptedStream = new CryptoStream(OutStream, EncryptionAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);

                while (0 != (iBytesRead = InStream.Read(abyBuffer, 0, abyBuffer.Length)))
                {
                    EncryptedStream.Write(abyBuffer, 0, iBytesRead);
                }

                EncryptedStream.FlushFinalBlock();
            }
            catch
            {
                //Do nothing.
            }
            finally
            {
                if (EncryptedStream != null)
                {
                    EncryptedStream.Close();
                }
            }
        }

        #endregion 
    }
}
