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
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Class for CRC validation
    /// </summary>
    public static class CRC
    {
        #region Constants

        private static readonly ushort[] RevCCITT =
	    {
	         0x0000, 0x1189, 0x2312, 0x329B, 0x4624, 0x57AD, 0x6536, 0x74BF,
	         0x8C48, 0x9DC1, 0xAF5A, 0xBED3, 0xCA6C, 0xDBE5, 0xE97E, 0xF8F7,
	         0x1081, 0x0108, 0x3393, 0x221A, 0x56A5, 0x472C, 0x75B7, 0x643E,
	         0x9CC9, 0x8D40, 0xBFDB, 0xAE52, 0xDAED, 0xCB64, 0xF9FF, 0xE876,
	         0x2102, 0x308B, 0x0210, 0x1399, 0x6726, 0x76AF, 0x4434, 0x55BD,
	         0xAD4A, 0xBCC3, 0x8E58, 0x9FD1, 0xEB6E, 0xFAE7, 0xC87C, 0xD9F5,
	         0x3183, 0x200A, 0x1291, 0x0318, 0x77A7, 0x662E, 0x54B5, 0x453C,
	         0xBDCB, 0xAC42, 0x9ED9, 0x8F50, 0xFBEF, 0xEA66, 0xD8FD, 0xC974,
	         0x4204, 0x538D, 0x6116, 0x709F, 0x0420, 0x15A9, 0x2732, 0x36BB,
	         0xCE4C, 0xDFC5, 0xED5E, 0xFCD7, 0x8868, 0x99E1, 0xAB7A, 0xBAF3,
	         0x5285, 0x430C, 0x7197, 0x601E, 0x14A1, 0x0528, 0x37B3, 0x263A,
	         0xDECD, 0xCF44, 0xFDDF, 0xEC56, 0x98E9, 0x8960, 0xBBFB, 0xAA72,
	         0x6306, 0x728F, 0x4014, 0x519D, 0x2522, 0x34AB, 0x0630, 0x17B9,
	         0xEF4E, 0xFEC7, 0xCC5C, 0xDDD5, 0xA96A, 0xB8E3, 0x8A78, 0x9BF1,
	         0x7387, 0x620E, 0x5095, 0x411C, 0x35A3, 0x242A, 0x16B1, 0x0738,
	         0xFFCF, 0xEE46, 0xDCDD, 0xCD54, 0xB9EB, 0xA862, 0x9AF9, 0x8B70,
	         0x8408, 0x9581, 0xA71A, 0xB693, 0xC22C, 0xD3A5, 0xE13E, 0xF0B7,
	         0x0840, 0x19C9, 0x2B52, 0x3ADB, 0x4E64, 0x5FED, 0x6D76, 0x7CFF,
	         0x9489, 0x8500, 0xB79B, 0xA612, 0xD2AD, 0xC324, 0xF1BF, 0xE036,
	         0x18C1, 0x0948, 0x3BD3, 0x2A5A, 0x5EE5, 0x4F6C, 0x7DF7, 0x6C7E,
	         0xA50A, 0xB483, 0x8618, 0x9791, 0xE32E, 0xF2A7, 0xC03C, 0xD1B5,
	         0x2942, 0x38CB, 0x0A50, 0x1BD9, 0x6F66, 0x7EEF, 0x4C74, 0x5DFD,
	         0xB58B, 0xA402, 0x9699, 0x8710, 0xF3AF, 0xE226, 0xD0BD, 0xC134,
	         0x39C3, 0x284A, 0x1AD1, 0x0B58, 0x7FE7, 0x6E6E, 0x5CF5, 0x4D7C,
	         0xC60C, 0xD785, 0xE51E, 0xF497, 0x8028, 0x91A1, 0xA33A, 0xB2B3,
	         0x4A44, 0x5BCD, 0x6956, 0x78DF, 0x0C60, 0x1DE9, 0x2F72, 0x3EFB,
	         0xD68D, 0xC704, 0xF59F, 0xE416, 0x90A9, 0x8120, 0xB3BB, 0xA232,
	         0x5AC5, 0x4B4C, 0x79D7, 0x685E, 0x1CE1, 0x0D68, 0x3FF3, 0x2E7A,
	         0xE70E, 0xF687, 0xC41C, 0xD595, 0xA12A, 0xB0A3, 0x8238, 0x93B1,
	         0x6B46, 0x7ACF, 0x4854, 0x59DD, 0x2D62, 0x3CEB, 0x0E70, 0x1FF9,
	         0xF78F, 0xE606, 0xD49D, 0xC514, 0xB1AB, 0xA022, 0x92B9, 0x8330,
	         0x7BC7, 0x6A4E, 0x58D5, 0x495C, 0x3DE3, 0x2C6A, 0x1EF1, 0x0F78
	    };

        private const ushort CRC_SEED = 0xFFFF;
        private const ushort CCIT_FINAL = 0xFFFF;

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates that the CRC read from the file matches the calculated 
        /// CRC.  Used to validate that the file has not been corrupted.  The 
        /// file could be for any of the 3 micros of the OpenWay meter, so the 
        /// file size can vary.
        /// </summary>
        /// <param name="strFilePath">Complete path to the firmware file</param>
        /// <param name="iFileSize">Size of the file</param>
        /// <remarks>Assumes that the CRC is located at the first 2 bytes of 
        /// the file and that the 1st byte is the MSB and the 2nd byte the 
        /// LSB
        /// </remarks>
        /// <returns>True if the CRC validates.  Otherwise, false.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 09/13/06 AF  7.35.00 N/A		Created
        // 
        public static bool ValidateCRC(string strFilePath, int iFileSize)
        {
            ushort crca;
            byte[] bybuf = new byte[iFileSize];
            ushort CRCFile;
            int iBytesRead = 0;

            FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            iBytesRead = fs.Read(bybuf, 0, iFileSize);
            fs.Close();

            //Parse the CRC out of the file
            CRCFile = (ushort)((bybuf[0] << 8) | (bybuf[1]));

            crca = CRC_SEED;

            for (int iIndex = 2; iIndex < iBytesRead; iIndex++)
            {
                crca = (ushort)((crca >> 8) ^ RevCCITT[(crca ^ (bybuf[iIndex])) & 0xFF]);
            }

            return (CRCFile == crca);
        }

        /// <summary>
        /// Calculates CRC
        /// </summary>
        /// <remarks>Calculates CRC 
        /// </remarks>
        /// <returns>Returns CRC.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/30/10 MA   N/A		Created
        // 
        public static ushort CalculateCRC(byte[] bybuf)
        {
            ushort crca;

            int iBytesRead = bybuf.Length;        

            crca = CRC_SEED;

            for (int iIndex = 0; iIndex < iBytesRead; iIndex++)
            {
                crca = (ushort)((crca >> 8) ^ RevCCITT[(crca ^ (bybuf[iIndex])) & 0xFF]);
            }

            return crca; 
            
        }

        /// <summary>
        /// Calculates CRC
        /// </summary>
        /// <remarks>Calculates CRC 
        /// </remarks>
        /// <returns>Returns CRC.</returns>
        /// 
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/29/13 RCG 2.80.35	       Created
        // 
        public static ushort CalculateCRCCCIT(byte[] bybuf)
        {
            ushort crca;

            int iBytesRead = bybuf.Length;

            crca = CRC_SEED;

            for (int iIndex = 0; iIndex < iBytesRead; iIndex++)
            {
                crca = (ushort)((crca >> 8) ^ RevCCITT[(crca ^ (bybuf[iIndex])) & 0xFF]);
            }

            crca ^= CCIT_FINAL;

            return crca;

        }

        /// <summary>
        /// Calculates the CRC-16-CCIT for the given data
        /// </summary>
        /// <param name="data">The data to calculate the CRC for</param>
        /// <returns>The crc</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public static ushort CalculateCRC16(byte[] data)
        {
            ushort CRC = CRC_SEED;

            for (int DataIndex = 0; DataIndex < data.Length; DataIndex++)
            {
                CRC ^= (ushort)(data[DataIndex] << 8);

                for (int BitIndex = 0; BitIndex < 8; BitIndex++)
                {
                    if((CRC & (ushort)0x8000) != 0x0000)
                    {
                        CRC = (ushort)((CRC << 1) ^ 0x1021);
                    }
                    else
                    {
                        CRC <<= 1;
                    }
                }
            }

            return CRC;
        }

        /// <summary>
        /// Calculates the CRC32 for a firmware file whose path is passed in.
        /// This is not a general use CRC32 because it skips over the header
        /// before starting the calculation
        /// </summary>
        /// <param name="strFilePath">Path to the f/w file to be checked</param>
        /// <param name="crc">The calculated CRC32</param>
        /// <returns>true if the CRC32 was successfully calculated; false, otherwise</returns>
        /// <remarks>
        /// This code is modified from a version sent by Scott Collins who implemented
        /// it for the collection engine.
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/15/09 AF  2.20.02        Created
        //
        public static bool CalculateFirmwareCRCForHanActivation(string strFilePath, out UInt32 crc)
        {
            bool result = true;
            //0x04C11DB7 is the official polynomial used by PKZip, WinZip and Ethernet
            uint crc32Poly = 0x04C11DB7;
            crc = UInt32.MaxValue;

            if (File.Exists(strFilePath))
            {
                FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] data = new byte[fs.Length];

                fs.Read(data, 0, (int)fs.Length);
                fs.Close();

                // Check on the header revision
                if (data[2] == 0x01)
                {
                    for (int index = 19; index < data.Length; index++)
                    {
                        crc ^= (((uint)data[index]) << 24);

                        for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                        {
                            if ((crc & (uint)0x80000000) != 0x00000000)
                            {
                                crc = (crc << 1) ^ crc32Poly;
                            }
                            else
                            {
                                crc <<= 1;
                            }
                        }
                    }

                    crc = ~crc;
                }
                else
                {
                    //Callback.WriteToLogFile("Error: unknown firmware file version. Cannot calculate CRC");

                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Calculates the CRC32 for a byte array that is passed in.
        /// </summary>
        /// <param name="uiPolynomialSeed">The polynomial seed to use in CRC calculation.</param>
        /// <param name="data">The data to use to compute CRC.</param>
        /// <param name="crc">The calculated CRC32</param>
        /// <returns>true if the CRC32 was successfully calculated; false, otherwise</returns>
        /// <remarks>
        /// This code is modified from a version sent by Scott Collins who implemented
        /// it for the collection engine.
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/03/14 jrf 3.50.19 TQ9629 Created
        //
        public static bool CalculateCRC32(uint uiPolynomialSeed, byte[] data, out UInt32 crc)
        {
            bool result = true;            
            uint crc32Poly = uiPolynomialSeed;
            crc = UInt32.MaxValue;

            if (null != data)
            {
                for (int index = 0; index < data.Length; index++)
                {
                    crc ^= (((uint)data[index]) << 24);

                    for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                    {
                        if ((crc & (uint)0x80000000) != 0x00000000)
                        {
                            crc = (crc << 1) ^ crc32Poly;
                        }
                        else
                        {
                            crc <<= 1;
                        }
                    }
                }

                crc = ~crc;
            }
            else
            {
                result = false;
            }

            return result;
        }

        #endregion
    }
}

