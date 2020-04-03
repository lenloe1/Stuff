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
//                           Copyright © 2008 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Itron.Metering.Utilities;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// Enum for type of micro in the OpenWay meter
    /// </summary>
    public enum FirmwareType : byte
    {
        /// <summary>
        /// Register firmware type
        /// </summary>
        [EnumDescription("Register Firmware")]
        RegisterFW = 0,
        /// <summary>
        /// Comm firmware type
        /// </summary>
        [EnumDescription("RFLAN Firmware")]
        RFLANFW = 1,
        /// <summary>
        /// HAN firmware type
        /// </summary>
        [EnumDescription("ZigBee Firmware")]
        ZigbeeFW = 2,
        /// <summary>
        /// Display firmware type
        /// </summary>
        [EnumDescription("Display Firmware")]
        DisplayFW = 3,
        /// <summary>
        /// HAN Device firmware type
        /// </summary>
        [EnumDescription("Third Party Firmware")]
        HANDevFW = 4,
        /// <summary>
        /// PLAN firmware type
        /// </summary>
        [EnumDescription("PLAN Firmware")]
        PLANFW = 5,
        /// <summary>
        /// M2 Gateway firmware type
        /// </summary>
        [EnumDescription("M2 Gateway Firmware")]
        M2GTWY = 6,
        /// <summary>
        /// L+G firmware type
        /// </summary>
        [EnumDescription("Landis+Gyr Firmware")]
        LGFW = 7,
        /// <summary>
        /// Comm firmware type 
        /// </summary>
        [EnumDescription("RF Mesh Firmware")]
        CiscoCommFW = 8,
        /// <summary>
        /// Cisco config file type 
        /// </summary>
        [EnumDescription("RF Mesh Configuration")]
        CiscoCfgFW = 9,
        /// <summary>
        /// ChoiceConnect FW file type 
        /// </summary>
        [EnumDescription("ChoiceConnect FW")]
        ChoiceConnectFW = 10,
        /// <summary>
        /// HAN OTA firmware type
        /// </summary>
        [EnumDescription("HAN OTA Firmware")]
        HAN_OTA_FW = 17,
        /// <summary>
        /// ICS FW file type 
        /// </summary>
        [EnumDescription("ICS FW")]
        ICSFW = 32,
        /// <summary>
        /// ICS Modem FW Sierra Verizon LTE file type
        /// </summary>
        [EnumDescription("Modem FW")]
        ICS_MODEM_FW_Sierra_Verizon_LTE = 37,
        /// <summary>
        /// ICS Modem FW Sierra ATT/Rogers/Bell LTE file type
        /// </summary>
        ICS_MODEM_FW_Sierra_ATT_Rogers_Bell_LTE = 38,
        /// <summary>
        /// Placeholder for any firmware we don't support
        /// </summary>
        [EnumDescription("Unrecognized FW")]
        UnrecognizedFW = 255,
    }

	/// <summary>
	/// This class represents an OpenWay Firmware file.  Its primary use is
	/// to retrieve select properties from the binary file so that the user
	/// can determine which files are which.
	/// </summary>
	/// <remarks>
	/// Revision History
	/// MM/DD/YY Who Version Issue# Description
	/// -------- --- ------- ------ -----------------------------------------
	/// 02/04/08 mah 10.00.00 N/A	Created
	/// </remarks>
	public class CENTRON_AMI_FW_File
	{

		#region Definitions

		private const int HEADERLENGTH = 256;
		private const int VERSIONOFFSET = 5;
		private const int REVISIONOFFSET = 6;
		private const int BUILDNUMBEROFFSET = 7;
		private const int FILETYPEOFFSET = 9;
		private const int MINHWVERSIONOFFSET = 10;
		private const int MINHWREVISIONOFFSET = 11;
		private const int MAXHWVERSIONOFFSET = 12;
		private const int MAXHWREVISIONOFFSET = 13;
        private const int C1222DEVICECLASSOFFSET = 14;
        private const byte COMM_MODULE_MASK = 0x0F;
        private const int CONTENTOFFSET = 19;
        private const int BLOCKSIZE = 64;
        private const int HASHLENGTH = 32;
        private const UInt32 CRC32OFFSET = 0xC3;

		#endregion //Definitions

		#region Public Methods
		/// <summary>
		/// This constructor opens and reads the firmware file to extract the
		/// files properties.  The file is closed prior to returning control from
		/// the constructor.  Note that the constructor can throw exceptions
		/// if file I/O errors are encountered or if the file is not an OpenWay
		/// firmware file
		/// </summary>
		/// <param name="strFilePath" type="string">
		/// The fully qualified path name of the binary firmware file to be 
		/// read and examined.
		/// </param>
		// Revision History
		// MM/DD/YY Who Version Issue# Description
		// -------- --- ------- ------ -----------------------------------------
		// 02/04/08 mah 10.00.00 N/A	Created
        // 09/15/09 AF  2.30.00 140967 Added some member variables to store min
        //                             and max h/w versions for use in dealing
        //                             with PrismLite files
        // 04/09/10 RCG 2.40.34        Moved Header parsing out to simplify constructor

        public CENTRON_AMI_FW_File(string strFilePath)
		{
            m_strFilePath = strFilePath;

            ParseHeader();

            // Do not read the entire file contents nor initialize the hash codes until requested

            m_byHashCode = null;
		}

		/// <summary>
		/// This method identifies the target processor for a firmware file given
		/// the file type as read from the firmware file
		/// </summary>
		/// <returns>
		///     A Itron.Metering.Device.FirmwareType value...
		/// </returns>
		// Revision History
		// MM/DD/YY Who Version Issue# Description
		// -------- --- ------- ------ -----------------------------------------
		// 02/04/08 mah 10.00.00 N/A	Created
        // 10/07/08 AF  2.00.00        Added support for Display firmware
        // 04/03/09 AF  2.20.00        Added support for HAN device firmware
        // 02/09/10 RCG 2.40.12        Simplified so that this does not have to be updated in the future
        // 06/30/16 AF  4.50.294 697649 Set the firmware type to unrecognized if it's not defined
        //                              rather than throwing an exception. We will filter those out later
        //
		static public FirmwareType TranslateFileType(byte byteFileType)
		{
            if (Enum.IsDefined(typeof(FirmwareType), byteFileType))
            {
                return (FirmwareType)byteFileType;
            }
            else
            {
                return FirmwareType.UnrecognizedFW;
            }
		}

		#endregion //Public Methods

		#region Public Properties

		/// <summary>
		/// This read-only property returns the version number of the firmware 
		/// file in the following string format "V.RR.BBB" where V is the version
		/// number, RR is the minor revision number, and BBB is the build number
		/// </summary>
		/// <remarks>
		/// Revision History
		/// MM/DD/YY Who Version Issue# Description
		/// -------- --- ------- ------ -----------------------------------------
		/// 02/04/08 mah 10.00.00 N/A	Created
		/// </remarks>
		public String CompleteVersion  
		{ 
			get
			{ 
				return m_strBuildVersion; 
			}
		}

		/// <summary>
		/// This read-only property returns the version number of the firmware 
		/// file in the following string format "V.RR" where V is the version
		/// number, RR is the minor revision number
		/// </summary>
		/// <remarks>
		/// Revision History
		/// MM/DD/YY Who Version Issue# Description
		/// -------- --- ------- ------ -----------------------------------------
		/// 02/04/08 mah 10.00.00 N/A	Created
		/// </remarks>
		public String VersionWOBuild
		{
			get
			{
				return m_strVersion;
			}
		}

		/// <summary>
		/// This read-only property returns the maximum version number of the 
		/// meter that the current firmware file can be used in
		/// </summary>
		/// <remarks>
		/// Revision History
		/// MM/DD/YY Who Version Issue# Description
		/// -------- --- ------- ------ -----------------------------------------
		/// 02/28/08 mah 10.00.00 N/A	Created
		/// </remarks>
		public String MaximumHardwareVersion 
		{
			get
			{
				return m_strMaxHWVersion;
			}
		}

        /// <summary>
        /// This read only property returns the maximum hardware version of
        /// the meter with the upper nibble masked off.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/15/09 AF  2.30.00 137695 Created
        //
        public String MaximumHardwareVersionMasked
        {
            get
            {
                byte byVer = (byte)(m_byMaxHWVer & COMM_MODULE_MASK);
                return (byVer.ToString(CultureInfo.InvariantCulture) + "." + m_byMaxHWRev.ToString("D3", CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// This read only property returns the max h/w version in numeric form
        /// for the meter that the current firmware file can be used in
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 AF  2.30.00 137695 Created
        //
        public byte MaxHWVersion
        {
            get
            {
                return m_byMaxHWVer;
            }
        }


		/// <summary>
		/// This read-only property returns the minimum version number of the 
		/// meter that the current firmware file can be used in
		/// </summary>
		/// <remarks>
		/// Revision History
		/// MM/DD/YY Who Version Issue# Description
		/// -------- --- ------- ------ -----------------------------------------
		/// 02/28/08 mah 10.00.00 N/A	Created
		/// </remarks>
		public String MinimumHardwareVersion
		{
			get
			{
				return m_strMinHWVersion;
			}
		}

        /// <summary>
        /// This read only property returns the minimum hardware version of
        /// the meter with the upper nibble masked off.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/15/09 AF  2.30.00 137695 Created
        //
        public String MinimumHardwareVersionMasked
        {
            get
            {
                byte byVer = (byte)(m_byMinHWVer & COMM_MODULE_MASK);
                return (byVer.ToString(CultureInfo.InvariantCulture) + "." + m_byMinHWRev.ToString("D3", CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// This read only property returns the min h/w version in numeric
        /// form for the meter that the current firmware file can be used in
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 AF  2.30.00 137695 Created
        //
        public byte MinHWVersion
        {
            get
            {
                return m_byMinHWVer;
            }
        }

		/// <summary>
		/// This read only property identifies the target processor for the firmware
		/// image
		/// </summary>
		/// <remarks>
		/// Revision History
		/// MM/DD/YY Who Version Issue# Description
		/// -------- --- ------- ------ -----------------------------------------
		/// 02/04/08 mah 10.00.00 N/A	Created
		/// </remarks>
		public FirmwareType FileType
		{
			get
			{
				return TranslateFileType(m_byFirmwareType);
			}
		}

        /// <summary>
        /// This read only property identifies the C1222 Device Class for the
        /// firmware image
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/09 AF  2.20.00        Created to help filter f/w files by h/w type
        //
        public String DeviceClass
        {
            get
            {
                return m_strDeviceClass;
            }
        }

        /// <summary>
        /// Gets the Firmware Version as a float
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 RCG 2.41.01        Created

        public float FirmwareVersion
        {
            get
            {
                return m_byFirmwareVersion + m_byFirmwareRevision / 1000.0f;
            }
        }

        /// <summary>
        /// Gets the Firmware build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 RCG 2.41.01        Created
        public byte FirmwareBuild
        {
            get
            {
                return m_byFirmwareBuild;
            }
        }

        /// <summary>
        /// Gets the SHA-256 hash code of the fw image
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/29/11 MAH 2.51.32+        Created
        public byte[] HashCode
        {
            get
            {
                if (m_byHashCode == null)
                {
                    CalculateHashCode();
                }
                
                return m_byHashCode;
            }
        }

        /// <summary>
        /// Gets the file path of the fw file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/25/11 jrf 2.52.07 TC4239 Created.
        public string FilePath
        {
            get
            {
                return m_strFilePath;
            }
        }

        /// <summary>
        /// Gets a text version of the SHA-256 hash code of the fw image
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/29/11 MAH 2.51.32+        Created
        static public String FormatHashCode( byte[] byHashCode )
        {
            string strHashCode = "";
            int nHashOffset = 0;

            // First validate the given byte array

            if (byHashCode == null)
            {
                return strHashCode;
            }

            if (byHashCode.Length == 0 || byHashCode.Length > HASHLENGTH)
            {
                return strHashCode;
            }

            // Now that we know the given hash code is the proper size go ahead
            // and render it as a text string

            foreach (byte byHashByte in byHashCode)
            {
                strHashCode += byHashByte.ToString("X2", CultureInfo.InvariantCulture);

                // We want to add a visual separator every 4 bytes so we
                // keep track of where we are in the array
                nHashOffset++; 

                if ((nHashOffset % (int)4) == 0 && nHashOffset < 31)
                {
                    strHashCode += "-";
                }
            }

            return strHashCode;
        }

        /// <summary>
        /// Reads the register application CRC32 out of the firmware file
        /// </summary>
        /// <returns>the register application CRC32 for this file</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/29/11 AF  2.52.10        Created
        //
        public UInt32 RegisterApplicationCRC32
        {
            get
            {
                UInt32 RegAppCRC32 = 0;
                FileStream streamFile = null;

                try
                {
                    streamFile = new FileStream(m_strFilePath, FileMode.Open, FileAccess.Read);

                    BinaryReader Reader = new BinaryReader(streamFile);

                    if (Reader.BaseStream.Seek(CRC32OFFSET, SeekOrigin.Begin) == CRC32OFFSET)
                    {
                        RegAppCRC32 = Reader.ReadUInt32();
                    }
                }
                catch
                {
                    RegAppCRC32 = 0;
                }
                finally
                {
                    if (streamFile != null)
                    {
                        streamFile.Close();
                    }
                }

                return RegAppCRC32;
            }
        }

		#endregion //Public Properties

        #region Private Methods

        /// <summary>
        /// Parses the data from the header
        /// </summary>
        // Revision History
        // MM/DD/YY Who Version Issue# Description
        // -------- --- ------- ------ -----------------------------------------
        // 04/09/10 RCG 02.40.34 N/A	Created

        private void ParseHeader()
        {
            FileStream streamFile = new FileStream(m_strFilePath, FileMode.Open, FileAccess.Read);
            byte[] bybuffer = new byte[HEADERLENGTH];

            streamFile.Read(bybuffer, 0, HEADERLENGTH);

            m_byFirmwareVersion = bybuffer[VERSIONOFFSET];
            m_byFirmwareRevision = bybuffer[REVISIONOFFSET];
            m_byFirmwareBuild = bybuffer[BUILDNUMBEROFFSET];

            m_strVersion = m_byFirmwareVersion.ToString(CultureInfo.InvariantCulture) + "." +
                            m_byFirmwareRevision.ToString("D3", CultureInfo.InvariantCulture);

            m_strBuildVersion = m_strVersion + "." +
                            m_byFirmwareBuild.ToString("D3", CultureInfo.InvariantCulture);

            m_byFirmwareType = bybuffer[FILETYPEOFFSET];

            m_strMinHWVersion = bybuffer[MINHWVERSIONOFFSET].ToString(CultureInfo.InvariantCulture) + "." +
                            bybuffer[MINHWREVISIONOFFSET].ToString("D3", CultureInfo.InvariantCulture);

            m_byMinHWVer = bybuffer[MINHWVERSIONOFFSET];
            m_byMinHWRev = bybuffer[MINHWREVISIONOFFSET];

            m_strMaxHWVersion = bybuffer[MAXHWVERSIONOFFSET].ToString(CultureInfo.InvariantCulture) + "." +
                            bybuffer[MAXHWREVISIONOFFSET].ToString("D3", CultureInfo.InvariantCulture);

            m_byMaxHWVer = bybuffer[MAXHWVERSIONOFFSET];
            m_byMaxHWRev = bybuffer[MAXHWREVISIONOFFSET];

            Encoding ascii = Encoding.ASCII;
            char[] asciiChars = new char[4];
            ascii.GetChars(bybuffer, C1222DEVICECLASSOFFSET, 4, asciiChars, 0);
            m_strDeviceClass = new string(asciiChars);

            streamFile.Position = 0;

            streamFile.Close();
        }

        /// <summary>
        /// Reads the FW Image from the file and 
        /// </summary>
        // Revision History
        // MM/DD/YY Who Version Issue# Description
        // -------- --- ------- ------ -----------------------------------------
        //  07/29/11 MAH 2.51.32+        Created
        private void CalculateHashCode()
        {
            int blockStart = 0;
            int blockNumber = 0;
            FileStream streamFile = null;

            try
            {
                streamFile = new FileStream(m_strFilePath, FileMode.Open, FileAccess.Read);

                long lContentSize = streamFile.Length;

                m_byHashCode = new byte[HASHLENGTH];

                while (blockStart < lContentSize)
                {
                    byte[] block = GetBlock(streamFile, blockNumber);
                    byte[] blockWithHash = AppendHashToBlock(block, m_byHashCode);
                    byte[] hash = CalcHash(blockWithHash);

                    m_byHashCode = hash;
                    blockStart = blockStart + block.Length;
                    blockNumber++;
                }
            }
            catch
            {
                m_byHashCode = null;    
            }
            finally
            {
                if (streamFile != null)
                {
                    streamFile.Close();
                }
            }
        }

        /// <summary>
        /// this gets an individual block worth of data.
        /// </summary>
        /// <param name="streamFile">The open file stream for the firmware image</param>
        /// <param name="blockNumber">the block number to get</param>
        /// <returns>a byte array with the contents of the block</returns>
        // Revision History
        // MM/DD/YY Who Version Issue# Description
        // -------- --- ------- ------ -----------------------------------------
        //  07/29/11 MAH 2.51.32+        Created
        private byte[] GetBlock(FileStream streamFile, int blockNumber)
        {
            int firstByte = blockNumber * BLOCKSIZE;
            int nReadLength = BLOCKSIZE;

            if (blockNumber < 0)
            {
                throw new ArgumentOutOfRangeException("blockNumber", "The block number must between 0 and the total number of blocks available.");
            }

            if (firstByte >= streamFile.Length)
            {
                throw new ArgumentOutOfRangeException("blockNumber", "The block number must between 0 and the total number of blocks available.");
            }

            if (nReadLength > streamFile.Length)
            {
                throw new ArgumentOutOfRangeException("blockSize", "The block size must between 1 and the total number of btyes in the content.");
            }

            // make sure we don't go past the end of the content
            if (firstByte + nReadLength >= streamFile.Length)
            {
                nReadLength = (int)streamFile.Length - firstByte;
            }

            byte[] block = new byte[BLOCKSIZE];

            streamFile.Read(block, 0, nReadLength);

            return block;
        }

        /// <summary>
        /// 
        /// </summary>
        // Revision History
        // MM/DD/YY Who Version Issue# Description
        // -------- --- ------- ------ -----------------------------------------
        //  07/29/11 MAH 2.51.32+        Created
        private byte[] AppendHashToBlock(byte[] block, byte[] hash)
        {
            if (block == null)
            {
                throw new ArgumentNullException("block", "The block byte array must not be null");
            }

            if (block.Length < 1)
            {
                throw new ArgumentOutOfRangeException("block", "The block byte array must not be blank");
            }

            if (hash == null)
            {
                throw new ArgumentNullException("hash", "The hash byte array must not be null");
            }

            if (hash.Length < 1)
            {
                throw new ArgumentOutOfRangeException("hash", "The hash byte array must not be blank");
            }


            int blockLen = block.Length;
            int hashLen = hash.Length;
            int newSize = blockLen + hashLen;
            byte[] newBlock = new byte[newSize];
            // Console.WriteLine("blockLen={0}, hashLen={1}, newSize={2}", blockLen, hashLen, newSize);

            int curPositon = 0;
            for (int i = 0; i < blockLen; i++)
            {
                // Console.WriteLine("{0}: Setting block bit {1} to {2}", i, curPositon, block[i]); 
                newBlock[curPositon] = block[i];
                curPositon++;
            }

            for (int i = 0; i < hashLen; i++)
            {
                // Console.WriteLine("{0}: Setting hash bit {1} to {2}", i, curPositon, hash[i]);
                newBlock[curPositon] = hash[i];
                curPositon++;
            }

            return newBlock;
        }

        /// <summary>
        /// 
        /// </summary>
        // Revision History
        // MM/DD/YY Who Version Issue# Description
        // -------- --- ------- ------ -----------------------------------------
        //  07/29/11 MAH 2.51.32+        Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private byte[] CalcHash(byte[] block)
        {
            if (block == null)
            {
                throw new ArgumentNullException("block", "The block byte array must not be null");
            }

            if (block.Length < 1)
            {
                throw new ArgumentOutOfRangeException("block", "The block byte array must not be blank");
            }


            SHA256 sha = new SHA256Managed();
            var hash = sha.ComputeHash(block);

            if (sha != null)
            {
                //dispose object
                sha.Clear();
            }

            return hash;
        }

        #endregion

        #region Private Members

        private byte m_byFirmwareType;
        private byte m_byFirmwareVersion;
        private byte m_byFirmwareRevision;
        private byte m_byFirmwareBuild;
        private string m_strVersion;
        private string m_strBuildVersion;
        private string m_strMinHWVersion;
        private string m_strMaxHWVersion;
        private string m_strDeviceClass;
        private byte m_byMinHWVer;
        private byte m_byMaxHWVer;
        private byte m_byMinHWRev;
        private byte m_byMaxHWRev;
        private string m_strFilePath;

        private byte[] m_byHashCode;


		#endregion //Private Members
	}

    /// <summary>
    /// HAN OTA FW File
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  08/25/16 PGH 4.70.15 701952 Created
    //
    public class HAN_OTA_FW_File
    {
        #region Constants

        private const int OTA_HEADER_OFFSET = 19;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strFilePath">The firmware file</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/25/16 PGH 4.70.15 701952 Created
        //
        public HAN_OTA_FW_File(string strFilePath)
        {
            m_strFilePath = strFilePath;

            ParseHeader();
        }

        /// <summary>
        /// Parses the HAN OTA firmware file header
        /// </summary>
        /// <returns>
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/25/16 PGH                Created
        //
        private void ParseHeader()
        {
            FileStream streamFile = new FileStream(m_strFilePath, FileMode.Open, FileAccess.Read);
            streamFile.Position = OTA_HEADER_OFFSET;

            BinaryReader Reader = new BinaryReader(streamFile);

            m_uiFileID = Reader.ReadUInt32();
            m_uiHeaderVersion = Reader.ReadUInt16();
            m_uiHeaderLength = Reader.ReadUInt16();
            m_uiHeaderFieldControl = Reader.ReadUInt16();
            m_uiManufacturerCode = Reader.ReadUInt16();
            m_uiImageType = Reader.ReadUInt16();
            m_uiFileVersion = Reader.ReadUInt32();
            m_uiZigBeeStackVersion = Reader.ReadUInt16();
            m_abyHeaderString = Reader.ReadBytes(32);
            m_uiImageSize = Reader.ReadUInt32();
            m_bySecurityCredVersion = Reader.ReadByte();
            m_uiIEEEAddress = Reader.ReadUInt64();
            m_uiMinHardwareVersion = Reader.ReadUInt16();
            m_uiMaxHardwareVersion = Reader.ReadUInt16();

            streamFile.Position = 0;

            streamFile.Close();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// HAN OTA Field ID
        /// </summary>
        public UInt32 FieldID
        {
            get
            {
                return m_uiFileID;
            }
        }

        /// <summary>
        /// HAN OTA Header Version
        /// </summary>
        public UInt16 HeaderVersion
        {
            get
            {
                return m_uiHeaderVersion;
            }
        }

        /// <summary>
        /// HAN OTA Header Length
        /// </summary>
        public UInt16 HeaderLength
        {
            get
            {
                return m_uiHeaderLength;
            }
        }

        /// <summary>
        /// HAN OTA Header Field Control
        /// </summary>
        public UInt16 HeaderFieldControl
        {
            get
            {
                return m_uiHeaderFieldControl;
            }
        }

        /// <summary>
        /// HAN OTA Manufacturer Code
        /// </summary>
        public UInt16 ManufacturerCode
        {
            get
            {
                return m_uiManufacturerCode;
            }
        }

        /// <summary>
        /// HAN OTA Image Type
        /// </summary>
        public UInt16 ImageType
        {
            get
            {
                return m_uiImageType;
            }
        }

        /// <summary>
        /// HAN OTA File Version
        /// </summary>
        public UInt32 FileVersion
        {
            get
            {
                return m_uiFileVersion;
            }
        }

        /// <summary>
        /// HAN OTA ZigBee Stack Version
        /// </summary>
        public UInt16 ZigBeeStackVersion
        {
            get
            {
                return m_uiZigBeeStackVersion;
            }
        }

        /// <summary>
        /// HAN OTA Header String
        /// </summary>
        public byte[] HeaderString
        {
            get
            {
                return m_abyHeaderString;
            }
        }

        /// <summary>
        /// HAN OTA Image Size
        /// </summary>
        public UInt32 ImageSize
        {
            get
            {
                return m_uiImageSize;
            }
        }

        /// <summary>
        /// HAN OTA Security Cred Version
        /// </summary>
        public byte SecurityCredVerion
        {
            get
            {
                return m_bySecurityCredVersion;
            }
        }

        /// <summary>
        /// HAN OTA IEEE Address
        /// </summary>
        public UInt64 IEEEAddress
        {
            get
            {
                return m_uiIEEEAddress;
            }
        }

        /// <summary>
        /// HAN OTA Min Hardware Version
        /// </summary>
        public UInt16 MinHardwareVersion
        {
            get
            {
                return m_uiMinHardwareVersion;
            }
        }

        /// <summary>
        /// HAN OTA Max Hardware Version
        /// </summary>
        public UInt16 MaxHardwareVersion
        {
            get
            {
                return m_uiMaxHardwareVersion;
            }
        }

        /// <summary>
        /// The pathname of the firmware file
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_strFilePath;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Members

        private UInt32 m_uiFileID;
        private UInt16 m_uiHeaderVersion;
        private UInt16 m_uiHeaderLength;
        private UInt16 m_uiHeaderFieldControl;
        private UInt16 m_uiManufacturerCode;
        private UInt16 m_uiImageType;
        private UInt32 m_uiFileVersion;
        private UInt16 m_uiZigBeeStackVersion;
        private byte[] m_abyHeaderString;
        private UInt32 m_uiImageSize;
        private byte m_bySecurityCredVersion;
        private UInt64 m_uiIEEEAddress;
        private UInt16 m_uiMinHardwareVersion;
        private UInt16 m_uiMaxHardwareVersion;
        private string m_strFilePath;

        #endregion
    }
}
