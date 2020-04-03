///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                           Copyright © 2006 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;
using System.Globalization;


namespace Itron.Metering.Device
{
    /// <summary>
    /// The CHANMfgTable2093 class gives the OTA header
    /// format details
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    public class CHANMfgTable2093 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const byte VERSION_MASK = 0xF0;
        private const byte REVISION_MASK = 0x0F;
        private const byte APP_RELEASE_INDEX = 0;
        private const byte APP_BUILD_INDEX = 1;
        private const byte STACK_RELEASE_INDEX = 2;
        private const byte STACK_BUILD_INDEX = 3;
        private const ushort MANUFACTURER_SPECIFIC_HIGH_IMAGE_TYPE = 0xFFBF;
        private const ushort MANUFACTURER_SPECIFIC_LOW_IMAGE_TYPE = 0x0000;

        #endregion

        #region Definitions

        /// <summary>
        /// Available options for the Field Control bits.
        /// </summary>
        [Flags]
        public enum FieldControlBits : ushort
        {
            /// <summary>
            /// No options selected
            /// </summary>
            [EnumDescription("None")]
            None = 0x00,
            /// <summary>
            /// Security Credentials Version Present
            /// </summary>
            [EnumDescription("Security Credential Version Present")]
            SecurityCredentialVersionPresent = 0x01,
            /// <summary>
            /// Device Specific File
            /// </summary>
            [EnumDescription("Device Specific File")]
            DeviceSpecificFile = 0x02,
            /// <summary>
            /// Hardware Versions Present
            /// </summary>
            [EnumDescription("Hardware Versions Present")]
            HardwareVersionsPresent = 0x04,
        }

        /// <summary>
        /// ZigBee Stack Version Values.
        /// </summary>
        public enum ZigBeeStackVersions : ushort
        {
            /// <summary>
            /// ZigBee 2006
            /// </summary>
            [EnumDescription("ZigBee 2006")]
            ZigBee2006 = 0x00,
            /// <summary>
            /// ZigBee 2007
            /// </summary>
            [EnumDescription("ZigBee 2007")]
            ZigBee2007 = 0x01,
            /// <summary>
            /// ZigBee Pro
            /// </summary>
            [EnumDescription("ZigBee Pro")]
            ZigBeePro = 0x02,
            /// <summary>
            /// ZigBee IP
            /// </summary>
            [EnumDescription("ZigBee IP")]
            ZigBeeIP = 0x03,
        }

        /// <summary>
        /// Security Credential Version Values.
        /// </summary>
        public enum SecurityCredentialVersions : ushort
        {
            /// <summary>
            /// SE 1.0
            /// </summary>
            [EnumDescription("SE 1.0")]
            SE1_0 = 0x00,
            /// <summary>
            /// SE 1.1
            /// </summary>
            [EnumDescription("SE 1.1")]
            SE1_1 = 0x01,
            /// <summary>
            /// SE 2.0
            /// </summary>
            [EnumDescription("SE 2.0")]
            SE2_0 = 0x02,
            /// <summary>
            /// SE 1.2
            /// </summary>
            [EnumDescription("SE 1.2")]
            SE1_2 = 0x03,
        }

        /// <summary>
        /// File Type Values.
        /// </summary>
        public enum FileTypeValues : ushort
        {
            /// <summary>
            /// Client Security Credentials
            /// </summary>
            [EnumDescription("Client Security Credentials")]
            ClientSecurityCredentials = 0xFFC0,
            /// <summary>
            /// Client Configuration
            /// </summary>
            [EnumDescription("Client Configuration")]
            ClientConfiguration = 0xFFC1,
            /// <summary>
            /// Server Log
            /// </summary>
            [EnumDescription("Server Log")]
            ServerLog = 0xFFC2,
            /// <summary>
            /// Picture
            /// </summary>
            [EnumDescription("Picture")]
            Picture = 0xFFC3,
            /// <summary>
            /// Wild Card
            /// </summary>
            [EnumDescription("Wild Card")]
            WildCard = 0xFFFF,
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        //
        public CHANMfgTable2093(CPSEM psem)
            : base(psem, 2093, GetTableLength(), TABLE_TIMEOUT)
        {
            m_uiFileID = 0;
            m_uiHeaderVersion = 0;
            m_uiHeaderLength = 0;
            m_uiHeaderFieldControl = 0;
            m_uiManufacturerCode = 0;
            m_uiImageType = 0;
            m_uiFileVersion = 0;
            m_uiZigBeeStackVersion = 0;
            m_uiImageSize = 0;
            m_bySecurityCredVersion = 0;
            m_uiIEEEAddress = 0;
            m_uiMinHardwareVersion = 0;
            m_uiMaxHardwareVersion = 0;
            m_IsImageValid = 0;
            m_IsGEMeter = false;
        }

        /// <summary>
        /// Full read of table 2093 (HAN OTA Header Table) out of the meter
        /// </summary>
        /// <returns>
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        //
        public override PSEMResponse Read()
        {

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2093s.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_uiFileID = m_Reader.ReadUInt32();
                m_uiHeaderVersion = m_Reader.ReadUInt16();
                m_uiHeaderLength = m_Reader.ReadUInt16();
                m_uiHeaderFieldControl = m_Reader.ReadUInt16();
                m_uiManufacturerCode = m_Reader.ReadUInt16();
                m_uiImageType = m_Reader.ReadUInt16();
                m_uiFileVersion = m_Reader.ReadUInt32();
                m_uiZigBeeStackVersion = m_Reader.ReadUInt16();
                m_abyHeaderString = m_Reader.ReadBytes(32);
                m_uiImageSize = m_Reader.ReadUInt32();
                m_bySecurityCredVersion = m_Reader.ReadByte();
                m_uiIEEEAddress = m_Reader.ReadUInt64();
                m_uiMinHardwareVersion = m_Reader.ReadUInt16();
                m_uiMaxHardwareVersion = m_Reader.ReadUInt16();

                if (m_IsGEMeter)
                {
                    m_IsImageValid = m_Reader.ReadByte();
                }
            }
            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Current HAN OTA Field ID's
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        //
        public UInt32 FieldID
        {
            get
            {
                ReadUnloadedTable();

                return m_uiFileID;
            }
        }

        /// <summary>
        /// Current HAN OTA Header Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt16 HeaderVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_uiHeaderVersion;
            }
        }

        /// <summary>
        /// Formatted header version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string FormattedHeaderVersion
        {
            get
            {
                byte[] HeaderVersionBytes = BitConverter.GetBytes(HeaderVersion);
                string FormattedVersion = "0.000";

                try
                {
                    if (null != HeaderVersionBytes && 2 <= HeaderVersionBytes.Length)
                    {
                        Array.Reverse(HeaderVersionBytes);

                        FormattedVersion = HeaderVersionBytes[0].ToString() + "." + HeaderVersionBytes[1].ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }

        /// <summary>
        /// Current HAN OTA Header Length
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt16 HeaderLength
        {
            get
            {
                ReadUnloadedTable();

                return m_uiHeaderLength;
            }
        }

        /// <summary>
        /// Current HAN OTA Header Field Control
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt16 HeaderFieldControl
        {
            get
            {
                ReadUnloadedTable();

                return m_uiHeaderFieldControl;
            }
        }

        /// <summary>
        /// Current HAN OTA Header Field Control
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString")]
        public FieldControlBits HeaderFieldControlValues
        {
            get
            {
                FieldControlBits FieldControlValues = FieldControlBits.None;

                if (true == Enum.IsDefined(typeof(FieldControlBits), HeaderFieldControl))
                {
                    FieldControlValues = (FieldControlBits)Enum.Parse(typeof(FieldControlBits), HeaderFieldControl.ToString());
                }

                return FieldControlValues;
            }
        }

        /// <summary>
        /// Current HAN OTA Header Security Credential Version Present
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public bool HeaderSecurityCredentialVersionPresent
        {
            get
            {
                bool Present = false;

                try
                {
                    Present = (HeaderFieldControlValues & FieldControlBits.SecurityCredentialVersionPresent)
                      == FieldControlBits.SecurityCredentialVersionPresent;
                }
                catch { }

                return Present;
            }
        }

        /// <summary>
        /// Current HAN OTA Header Device Specific File
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public bool HeaderDeviceSpecificFile
        {
            get
            {
                bool DeviceSpecific = false;

                try
                {
                    DeviceSpecific = (HeaderFieldControlValues & FieldControlBits.DeviceSpecificFile)
                      == FieldControlBits.DeviceSpecificFile;
                }
                catch { }

                return DeviceSpecific;
            }
        }

        /// <summary>
        /// Current HAN OTA Header Hardware Versions Present
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        public bool HeaderHardwareVersionsPresent
        {
            get
            {
                bool Present = false;

                try
                {
                    Present = (HeaderFieldControlValues & FieldControlBits.HardwareVersionsPresent)
                      == FieldControlBits.HardwareVersionsPresent;
                }
                catch { }

                return Present;
            }
        }

        /// <summary>
        /// Current HAN OTA Manufacturer Code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt16 ManufacturerCode
        {
            get
            {
                ReadUnloadedTable();

                return m_uiManufacturerCode;
            }
        }

        /// <summary>
        /// Current HAN OTA Image Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt16 ImageType
        {
            get
            {
                ReadUnloadedTable();

                return m_uiImageType;
            }
        }

        /// <summary>
        /// Current HAN OTA Image Type Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString(System.String)")]
        public string ImageTypeName
        {
            get
            {
                string Name = "Unknown";

                switch (ImageType)
                {
                    case (ushort)FileTypeValues.ClientSecurityCredentials:
                        {
                            Name = FileTypeValues.ClientSecurityCredentials.ToDescription();
                            break;
                        }
                    case (ushort)FileTypeValues.ClientConfiguration:
                        {
                            Name = FileTypeValues.ClientConfiguration.ToDescription();
                            break;
                        }
                    case (ushort)FileTypeValues.ServerLog:
                        {
                            Name = FileTypeValues.ServerLog.ToDescription();
                            break;
                        }
                    case (ushort)FileTypeValues.Picture:
                        {
                            Name = FileTypeValues.Picture.ToDescription();
                            break;
                        }
                    case (ushort)FileTypeValues.WildCard:
                        {
                            Name = FileTypeValues.WildCard.ToDescription();
                            break;
                        }
                    default:
                        {
                            if (MANUFACTURER_SPECIFIC_LOW_IMAGE_TYPE <= ImageType && MANUFACTURER_SPECIFIC_HIGH_IMAGE_TYPE >= ImageType)
                            {
                                Name = "Manufacturer Specific (0x" + ImageType.ToString("X4") + ")";
                            }
                            else
                            {
                                Name = "Unknown (0x" + ImageType.ToString("X4") + ")";
                            }
                            break;
                        }
                }

                return Name;
            }
        }

        /// <summary>
        /// Current HAN OTA File Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt32 FileVersion
        {
            get
            {

                m_TableState = TableState.Expired;

                ReadUnloadedTable();

                return m_uiFileVersion;
            }
        }

        /// <summary>
        /// Formatted application release version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string ApplicationReleaseVersion
        {
            get
            {
                byte[] FileVersionBytes = BitConverter.GetBytes(FileVersion);
                string FormattedVersion = "0.000";
                byte AppReleaseVersion = 0;
                byte AppReleaseRevision = 0;

                try
                {
                    if (null != FileVersionBytes && APP_RELEASE_INDEX+1 <= FileVersionBytes.Length)
                    {
                        Array.Reverse(FileVersionBytes);

                        AppReleaseVersion = (byte)((FileVersionBytes[APP_RELEASE_INDEX] & VERSION_MASK) >> 4);
                        AppReleaseRevision = (byte)(FileVersionBytes[APP_RELEASE_INDEX] & REVISION_MASK);

                        FormattedVersion = AppReleaseVersion.ToString() + "." + AppReleaseRevision.ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }

        /// <summary>
        /// Formatted application build number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        public byte ApplicationBuild
        {
            get
            {
                byte[] FileVersionBytes = BitConverter.GetBytes(FileVersion);
                byte Build = 0;

                try
                {
                    if (null != FileVersionBytes && APP_BUILD_INDEX + 1 <= FileVersionBytes.Length)
                    {
                        Array.Reverse(FileVersionBytes);

                        Build = FileVersionBytes[APP_BUILD_INDEX];
                    }
                }
                catch { }

                return Build;
            }
        }

        /// <summary>
        /// Formatted stack release version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string StackReleaseVersion
        {
            get
            {
                byte[] FileVersionBytes = BitConverter.GetBytes(FileVersion);
                string FormattedVersion = "0.000";
                byte StackReleaseVersion = 0;
                byte StackReleaseRevision = 0;

                try
                {
                    if (null != FileVersionBytes && STACK_RELEASE_INDEX + 1 <= FileVersionBytes.Length)
                    {
                        Array.Reverse(FileVersionBytes);

                        StackReleaseVersion = (byte)((FileVersionBytes[STACK_RELEASE_INDEX] & VERSION_MASK) >> 4);
                        StackReleaseRevision = (byte)(FileVersionBytes[STACK_RELEASE_INDEX] & REVISION_MASK);

                        FormattedVersion = StackReleaseVersion.ToString() + "." + StackReleaseRevision.ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }

        /// <summary>
        /// Formatted stack build number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        public byte StackBuild
        {
            get
            {
                byte[] FileVersionBytes = BitConverter.GetBytes(FileVersion);
                byte Build = 0;

                try
                {
                    if (null != FileVersionBytes && STACK_BUILD_INDEX + 1 <= FileVersionBytes.Length)
                    {
                        Array.Reverse(FileVersionBytes);

                        Build = FileVersionBytes[STACK_BUILD_INDEX];
                    }
                }
                catch { }

                return Build;
            }
        }

        /// <summary>
        /// Current HAN OTA ZigBee Stack Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt16 ZigBeeStackVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_uiZigBeeStackVersion;
            }
        }

        /// <summary>
        /// Current HAN OTA ZigBee Stack Version Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString(System.String)")]
        public string ZigBeeStackVersionName
        {
            get
            {
                string Name = "Unknown";

                switch (ZigBeeStackVersion)
                {
                    case (ushort)ZigBeeStackVersions.ZigBee2006:
                        {
                            Name = ZigBeeStackVersions.ZigBee2006.ToDescription();
                            break;
                        }
                    case (ushort)ZigBeeStackVersions.ZigBee2007:
                        {
                            Name = ZigBeeStackVersions.ZigBee2007.ToDescription();
                            break;
                        }
                    case (ushort)ZigBeeStackVersions.ZigBeePro:
                        {
                            Name = ZigBeeStackVersions.ZigBeePro.ToDescription();
                            break;
                        }
                    case (ushort)ZigBeeStackVersions.ZigBeeIP:
                        {
                            Name = ZigBeeStackVersions.ZigBeeIP.ToDescription();
                            break;
                        }
                    default:
                        {
                            Name = "Unknown (0x" + ZigBeeStackVersion.ToString("X") + ")";
                            break;
                        }
                }

                return Name;
            }
        }

        /// <summary>
        /// Current HAN OTA Header String
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public byte[] HeaderString
        {
            get
            {
                ReadUnloadedTable();

                return m_abyHeaderString;
            }
        }

        /// <summary>
        /// Current HAN OTA Header String as a string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/12/16 jrf 4.70.17 WI 714620 Created
        public string InterpretedHeaderString
        {
            get
            {
                string HeaderStringField = "";

                try
                {
                    HeaderStringField = System.Text.ASCIIEncoding.ASCII.GetString(HeaderString);
                }
                catch { }

                return HeaderStringField;
            }
        }

        /// <summary>
        /// Current HAN OTA Image Size in bytes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt32 ImageSize
        {
            get
            {
                ReadUnloadedTable();

                return m_uiImageSize;
            }
        }

        /// <summary>
        /// Current HAN OTA Security Cred Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public byte SecurityCredVerion
        {
            get
            {
                ReadUnloadedTable();

                return m_bySecurityCredVersion;
            }
        }

        /// <summary>
        /// Current HAN OTA Security Cred Version Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        public string SecurityCredentialVersionName
        {
            get
            {
                string Name = "Unknown";

                switch (SecurityCredVerion)
                {
                    case (byte)SecurityCredentialVersions.SE1_0:
                        {
                            Name = SecurityCredentialVersions.SE1_0.ToDescription();
                            break;
                        }
                    case (byte)SecurityCredentialVersions.SE1_1:
                        {
                            Name = SecurityCredentialVersions.SE1_1.ToDescription();
                            break;
                        }
                    case (byte)SecurityCredentialVersions.SE2_0:
                        {
                            Name = SecurityCredentialVersions.SE2_0.ToDescription();
                            break;
                        }
                    case (byte)SecurityCredentialVersions.SE1_2:
                        {
                            Name = SecurityCredentialVersions.SE1_2.ToDescription();
                            break;
                        }
                    default:
                        {
                            Name = "Unknown (0x" + SecurityCredVerion.ToString("X") + ")";
                            break;
                        }
                }

                return Name;
            }
        }

        /// <summary>
        /// Current HAN OTA IEEE Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt64 IEEEAddress
        {
            get
            {
                ReadUnloadedTable();

                return m_uiIEEEAddress;
            }
        }

        /// <summary>
        /// Current HAN OTA Min Hardware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt16 MinHardwareVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_uiMinHardwareVersion;
            }
        }

        /// <summary>
        /// Formatted min hardware version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string FormattedMinHardwareVersion
        {
            get
            {
                byte[] VersionBytes = BitConverter.GetBytes(MinHardwareVersion);
                string FormattedVersion = "0.000";

                try
                {
                    if (null != VersionBytes && 2 <= VersionBytes.Length)
                    {
                        Array.Reverse(VersionBytes);

                        FormattedVersion = VersionBytes[0].ToString() + "." + VersionBytes[1].ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }

        /// <summary>
        /// Current HAN OTA Max Hardware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        public UInt16 MaxHardwareVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_uiMaxHardwareVersion;
            }
        }

        /// <summary>
        /// Formatted max hardware version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/13/16 jrf 4.70.17 WI 714620 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string FormattedMaxHardwareVersion
        {
            get
            {
                byte[] VersionBytes = BitConverter.GetBytes(MaxHardwareVersion);
                string FormattedVersion = "0.000";

                try
                {
                    if (null != VersionBytes && 2 <= VersionBytes.Length)
                    {
                        Array.Reverse(VersionBytes);

                        FormattedVersion = VersionBytes[0].ToString() + "." + VersionBytes[1].ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }

        /// <summary>
        /// Is the image valid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/30/16 JBH                Created
        public Byte IsImageValid
        {
            get
            {
                ReadUnloadedTable();

                return m_IsImageValid;
            }
        }

        /// <summary>
        /// Flag for if the meter is a GE meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/30/16 JBH                Created
        public Boolean IsGEMeter
        {
            get
            {
                return m_IsGEMeter;
            }
            set
            {
                m_IsGEMeter = value;
                if (m_IsGEMeter)
                {
                    ChangeTableSize(GetTableLength() + sizeof(Byte));
                }
                else
                {
                    ChangeTableSize(GetTableLength());
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table
        /// </summary>
        /// <returns>length in bytes of table 2093</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/16 WPL                Created
        //
        static private uint GetTableLength()
        {
            uint HeaderTableLen = sizeof(UInt32) + sizeof(UInt16) + sizeof(UInt16)
                            + sizeof(UInt16) + sizeof(UInt16) + sizeof(UInt16)
                            + sizeof(UInt32) + sizeof(UInt16) + sizeof(byte) * 32
                            + sizeof(UInt32) + sizeof(byte) + sizeof(UInt64)
                            + sizeof(UInt16) + sizeof(UInt16);
           
            return (uint) HeaderTableLen;
        }

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
        private byte m_IsImageValid;
        private bool m_IsGEMeter;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2094 class gives the HAN OTA parameters details
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    public class CHANMfgTable2094 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const int TABLE_SIZE = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/17 jrf 4.70.16 WI 714619 Created
        public CHANMfgTable2094(CPSEM psem)
            : base(psem, 2094, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_QueryJitter = 0;
            m_DataSize = 0;
            m_CurrentTime = 0;
            m_UpgradeTime = 0;
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/17 jrf 4.70.16 WI 714619 Created
        public CHANMfgTable2094(PSEMBinaryReader reader)
            : base(2094, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Full read of table 2094 (HAN OTA Parameters Table) out of the meter
        /// </summary>
        /// <returns>
        /// PSEMResponse of the outcome of the read.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/17 jrf 4.70.16 WI 714619 Created
        public override PSEMResponse Read()
        {

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2094.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }
            return Result;
        }

        /// <summary>
        /// Full write of table 2094 (HAN OTA Parameters Table) to the meter
        /// </summary>
        /// <returns>
        /// PSEMResponse of the outcome of the write.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/21/16 JBH                   Created
        public override PSEMResponse Write()
        {

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2094.Write");

            // Write all of the data back to the stream so we
            // write the data that we have stored.
            m_DataStream.Position = 0;

            m_Writer.Write(m_QueryJitter);
            m_Writer.Write(m_DataSize);
            m_Writer.Write(m_CurrentTime);
            m_Writer.Write(m_UpgradeTime);

            return base.Write();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Query Jitter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/17 jrf 4.70.16 WI 714619 Created
        public byte QueryJitter
        {
            get
            {
                ReadUnloadedTable();

                return m_QueryJitter;
            }
            set
            {
                m_QueryJitter = value;
            }
        }

        /// <summary>
        /// Data Size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/17 jrf 4.70.16 WI 714619 Created
        public byte DataSize
        {
            get
            {
                ReadUnloadedTable();

                return m_DataSize;
            }
            set
            {
                m_DataSize = value;
            }
        }

        /// <summary>
        /// Current Time in UTC.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/17 jrf 4.70.16 WI 714619 Created
        //  09/14/17 jrf 4.70.17 WI 714619 Corrected time to be seconds from 1/1/2000
        public DateTime CurrentTime
        {
            get
            {
                ReadUnloadedTable();

                DateTime Time = new DateTime(2000, 1, 1);

                Time = Time.AddSeconds(m_CurrentTime);

                return Time;
            }
            set
            {
                DateTime Time = new DateTime(2000, 1, 1);
                m_CurrentTime = (uint)value.Subtract(Time).TotalSeconds;
            }
        }

        /// <summary>
        /// Upgrade Time in UTC.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/17 jrf 4.70.16 WI 714619 Created
        //  09/14/17 jrf 4.70.17 WI 714619 Corrected time to be seconds from 1/1/2000
        public DateTime UpgradeTime
        {
            get
            {
                ReadUnloadedTable();

                DateTime Time = new DateTime(2000, 1, 1);

                Time = Time.AddSeconds(m_UpgradeTime);

                return Time;
            }
            set
            {
                DateTime Time = new DateTime(2000, 1, 1);
                m_UpgradeTime = (uint)value.Subtract(Time).TotalSeconds;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the data out of the binary reader and into the member variables.
        /// </summary>
        private void ParseData()
        {
            //Populate the member variable that represent the table
            m_QueryJitter = m_Reader.ReadByte();
            m_DataSize = m_Reader.ReadByte();
            m_CurrentTime = m_Reader.ReadUInt32();
            m_UpgradeTime = m_Reader.ReadUInt32();
        }

        #endregion


        #region Members

        private byte m_QueryJitter = 0;
        private byte m_DataSize = 0;
        private UInt32 m_CurrentTime = 0;
        private UInt32 m_UpgradeTime = 0;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2095 class handles the reading of the OTA 
    /// Diagnostics Table
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    public class CHANMfgTable2095 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const int EXTENDED_TABLE_SIZE = 357;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/16 WPL                Created
        //
        public CHANMfgTable2095(CPSEM psem)
            : base(psem, 2095, GetTableLength(), TABLE_TIMEOUT)
        {
            m_HANOTAActivateFlag = 0;
            m_ImageNotifyBitMask = 0;
            m_ImageActivationBitMask = 0;
            m_NDevices = 0;
            m_lstHanDiagElmtRcd = new List<HANDiagnosticsElementRcd>();
            m_lstHANClusterDestEP = new List<HANClusterDestEndpointElement>();
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Full read of table 2095 (HAN OTA Poll Message Table) out of the meter
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/25/16 WPL                Created
        //  11/17/16 AF  4.70.34 730099 Added the cluster destination endpoint element fields
        //
        public override PSEMResponse Read()
        {
            HANDiagnosticsElementRcd rcdDiagElmt;
            HANClusterDestEndpointElement elmClusterEP;
            //UInt16 Tmp_NDevices;
            m_lstHanDiagElmtRcd.Clear();

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2095.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                m_HANOTAActivateFlag = m_Reader.ReadByte();
                m_ImageNotifyBitMask = m_Reader.ReadUInt16();
                m_ImageActivationBitMask = m_Reader.ReadUInt16();
                //Tmp_NDevices = m_Reader.ReadUInt16();
                m_NDevices = m_Reader.ReadUInt16();

                for (int i = 0; i < 10; i++)
                {
                    rcdDiagElmt = new HANDiagnosticsElementRcd();
                    
                    rcdDiagElmt.ZED_EUI = m_Reader.ReadUInt64();
                    rcdDiagElmt.ActivationCommandSent = m_Reader.ReadByte();
                    rcdDiagElmt.LastFwdlBlockSizeUsed = m_Reader.ReadByte();
                    rcdDiagElmt.CurrentFirmwareVersion = m_Reader.ReadUInt32();
                    rcdDiagElmt.NewFirmwareVersion = m_Reader.ReadUInt32();
                    rcdDiagElmt.NBytesSent = m_Reader.ReadUInt32();
                    rcdDiagElmt.NBytesRemaining = m_Reader.ReadUInt32();
                    rcdDiagElmt.LastCommandRXTimestamp = m_Reader.ReadUInt32();
                    rcdDiagElmt.LastCommandTXTimestamp = m_Reader.ReadUInt32();

                    m_lstHanDiagElmtRcd.Add(rcdDiagElmt);
                }

                if (m_DataStream.Length >= EXTENDED_TABLE_SIZE)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        elmClusterEP = new HANClusterDestEndpointElement();
                        elmClusterEP.EndpointID = m_Reader.ReadByte();

                        m_lstHANClusterDestEP.Add(elmClusterEP);
                    }
                }
            }
            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// HAN Cluster Endpoint IDs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/10/16 JBH                Created
        //
        public List<HANClusterDestEndpointElement> EndPointIDs
        {
            get
            {
                ReadUnloadedTable();

                return m_lstHANClusterDestEP;
            }
        }

        /// <summary>
        /// HAN OTA Activate Flag
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/08/16 JBH                Created
        //
        public Byte HAN_OTA_ActivateFlag
        {
            get
            {
                ReadUnloadedTable();

                return m_HANOTAActivateFlag;
            }
        }

        /// <summary>
        /// HAN OTA Activate
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.17 WI 714616 Created     
        public bool HAN_OTA_Activate
        {
            get
            {
                bool Activate = false;

                if (1 == HAN_OTA_ActivateFlag)
                {
                    Activate = true;
                }

                return Activate;
            }
        }

        /// <summary>
        /// Image Notify Bit Mask
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/08/16 JBH                Created
        //
        public UInt16 ImageNotifyBitMask
        {
            get
            {
                ReadUnloadedTable();

                return m_ImageNotifyBitMask;
            }
        }

        /// <summary>
        /// Image Activation Bit Mask
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/08/16 JBH                Created
        //
        public UInt16 ImageActivationBitMask
        {
            get
            {
                ReadUnloadedTable();

                return m_ImageActivationBitMask;
            }
        }

        /// <summary>
        /// Number of Poll Messages Currently Active
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/07/16 JBH                Created
        //
        public UInt16 nDevices
        {
            get
            {
                ReadUnloadedTable();

                return m_NDevices;
            }
        }

        /// <summary>
        /// Dump of the HAN OTA Poll Message table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/16 WPL                Created
        //
        public List<HANDiagnosticsElementRcd> rcdDiagElmt
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Network Information Table"));
                    }
                }

                return m_lstHanDiagElmtRcd;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table
        /// </summary>
        /// <returns>length in bytes of table 2095</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/16 WPL                Created
        //  08/11/16 WPL                Adjusted for new change in time type
        //
        static private uint GetTableLength()
        {
            /*
            uint uiDiagElmtRcdLen = sizeof(byte) + sizeof(UInt16) + sizeof(UInt32)
                                     + sizeof(UInt32) + sizeof(UInt32) + sizeof(UInt32) 
                                     + sizeof(UInt32) + sizeof(UInt32) + sizeof(UInt32)
                                     + sizeof(UInt32);
             */
            uint uiDiagElmtRcdLen = sizeof(UInt64) + sizeof(byte) + sizeof(byte) + sizeof(UInt32)
                         + sizeof(UInt32) + sizeof(UInt32) + sizeof(UInt32)
                         + sizeof(UInt32) + sizeof(UInt32) ;

            return (uint)(sizeof(byte) + sizeof(UInt16) + sizeof(UInt16) + sizeof(UInt16) + (10 * uiDiagElmtRcdLen));
        }

        #endregion

        #region Members

        private Byte m_HANOTAActivateFlag;
        private UInt16 m_ImageNotifyBitMask;
        private UInt16 m_ImageActivationBitMask;
        private UInt16 m_NDevices;
        private List<HANDiagnosticsElementRcd> m_lstHanDiagElmtRcd;
        private List<HANClusterDestEndpointElement> m_lstHANClusterDestEP;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2096 class handles the reading of the OTA 
    /// Poll Table
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    public class CHANMfgTable2096 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/25/16 WPL                Created
        //  08/11/16 WPL                Added in Number of Poll Messages support
        //
        public CHANMfgTable2096(CPSEM psem)
            : base(psem, 2096, GetTableLength(), TABLE_TIMEOUT)
        {
            m_uiNumberOfPollMessages = 0;
            m_lstHanOtaPollMsg = new List<HAN_OTA_Poll_Msg>();
        }


        /// <summary>
        /// Full read of table 2096 (HAN OTA Poll Message Table) out of the meter
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/25/16 WPL                Created
        //  08/11/16 WPL                Added in Number of Poll Message support
        //
        public override PSEMResponse Read()
        {
            HAN_OTA_Poll_Msg rcdPollMsg;
            m_lstHanOtaPollMsg.Clear();

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2096s.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_uiNumberOfPollMessages = m_Reader.ReadUInt16();

                for (int i = 0; i < 20; i++)
                {
                    rcdPollMsg = new HAN_OTA_Poll_Msg();
                    rcdPollMsg.ReceivedTimestamp = m_Reader.ReadUInt32();
                    rcdPollMsg.ZedEui = m_Reader.ReadUInt64();
                    rcdPollMsg.FieldControl = m_Reader.ReadByte();
                    rcdPollMsg.ManufCode = m_Reader.ReadUInt16();
                    rcdPollMsg.ImageType = m_Reader.ReadUInt16();
                    rcdPollMsg.CurrentFileVersion = m_Reader.ReadUInt32();
                    rcdPollMsg.HardwareVersion = m_Reader.ReadUInt16();

                    m_lstHanOtaPollMsg.Add(rcdPollMsg);
                }
            }
            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Number of Poll Messages Currently Active
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/11/16 WPL                Created
        //
        public UInt16 NumberOfPollMessages
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNumberOfPollMessages;
            }
        }

        /// <summary>
        /// Dump of the HAN OTA Poll Message table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/25/16 WPL                Created
        //
        public List<HAN_OTA_Poll_Msg> HANOtaPollMsg
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Network Information Table"));
                    }
                }

                return m_lstHanOtaPollMsg;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table
        /// </summary>
        /// <returns>length in bytes of table 2096</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/25/16 WPL                Created
        //  08/11/16 WPL                Added in Number of Poll Message support and
        //                              adjusted for time variable type
        //
        static private uint GetTableLength()
        {
            uint uiPollTableRcdLen = sizeof(UInt32) + sizeof(UInt64) +sizeof(byte) 
                            + sizeof(UInt16) + sizeof(UInt16) + sizeof(UInt32) 
                            + sizeof(UInt16);

            return (uint) sizeof(UInt16) + (20 * uiPollTableRcdLen);
        }

        #endregion

        #region Members

        private UInt16 m_uiNumberOfPollMessages;
        private List<HAN_OTA_Poll_Msg> m_lstHanOtaPollMsg;

        #endregion
    }

    /// <summary>
    /// HAN Mfg Table 2097 is a read only table that holds device registration
    /// information
    /// </summary>
    public class CHANMfgTable2097 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 100;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/16 WPL                Created
        //
        public CHANMfgTable2097(CPSEM psem)
            : base(psem, 2097, GetTableLength(), TABLE_TIMEOUT)
        {
            m_uiNumberofDevicesInFWDLProgress = 0;
            m_lstHanOtaStatRcd = new List<HANOTAStatRcd>();
        }

        /// <summary>
        /// Full read of table 2097
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/16 WPL                Created
        //  08/11/16 WPL                Added in Num of Devices support and time
        //                              variable type changes
        //
        public override PSEMResponse Read()
        {
            HANOTAStatRcd rcdOtaPollRcd;
            m_lstHanOtaStatRcd.Clear();

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2097.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_uiNumberofDevicesInFWDLProgress = m_Reader.ReadUInt16();

                for (int i = 0; i < 10; i++)
                {
                    rcdOtaPollRcd = new HANOTAStatRcd();
                    rcdOtaPollRcd.ZedEui = m_Reader.ReadUInt64();
                    rcdOtaPollRcd.ZedStateStatus = (HANOTAStatRcd.eZEDState)m_Reader.ReadByte();
                    rcdOtaPollRcd.NumBytesTrans = m_Reader.ReadUInt32();
                    rcdOtaPollRcd.TimeStamp = m_Reader.ReadUInt32();
                    rcdOtaPollRcd.PercentComp = m_Reader.ReadByte();
                    rcdOtaPollRcd.CoordinatorStateStatus = (HANOTAStatRcd.eCoordinatorState)m_Reader.ReadByte();

                    m_lstHanOtaStatRcd.Add(rcdOtaPollRcd);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Number of Devices in FWDL progress
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/11/16 WPL                Created
        //
        public UInt16 NumberOfDevicesInFWDLProgress
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNumberofDevicesInFWDLProgress;
            }
        }

        /// <summary>
        /// This is essentially a dump of the entire Mfg Table 2097.  Table 2097
        /// contains the HAN OTA Statistics records
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/29/16 WPL                Created
        //
        public List<HANOTAStatRcd> HanOtaPollRcd
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading AMI HAN OTA Record Table"));
                    }
                }

                return m_lstHanOtaStatRcd;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the length of table 2097
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/16 WPL                Created
        //  08/11/16 WPL                Added in Num of Devices support and changes
        //                              in time variable type
        //
        static private uint GetTableLength()
        {
            uint uiPollTableRcdLen = sizeof(UInt64) + sizeof(byte) + sizeof(UInt32)
                            + sizeof(UInt32) + sizeof(byte) + sizeof(byte);

            return (uint) sizeof(UInt16) + (10 * uiPollTableRcdLen);
        }

        #endregion

        #region Members

        private UInt16 m_uiNumberofDevicesInFWDLProgress;
        private List<HANOTAStatRcd> m_lstHanOtaStatRcd;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2098 class handles the reading of the Dimension HAN Limiting
    /// table.  This table is used by CHANMfgTable2107
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/11/09 AF  2.20.07 N/A    Created
    //
    public class CHANMfgTable2098 : AnsiTable
    {
        #region Constants

        private const int DIM_HAN_LIM_TBL_LENGTH = 9;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CHANMfgTable2098 Table2098 = new CHANMfgTable2098(m_PSEM);
        /// </code></example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07 N/A    Created
        //
        public CHANMfgTable2098(CPSEM psem)
            : base(psem, 2098, DIM_HAN_LIM_TBL_LENGTH, TABLE_TIMEOUT)
        {
            m_bytNbrClients = 0;
            m_bytNbrConfigCmds = 0;
            m_usDataSize = 0;
            m_usTxDataSize = 0;
            m_usRxDataSize = 0;
            m_bytNbrSecurityKeys = 0;
        }

        /// <summary>
        /// Full read of table 2098 (Dimension HAN Limiting Table) out of the meter
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07 N/A    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2098.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the actual number of HAN clients this server is capable of
        /// supporting.
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07 N/A    Created
        //
        public byte NumberHANClients
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrClients;
            }
        }

        /// <summary>
        /// Returns the actual number of commands each HAN client configuration
        /// record is capable of supporting.
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07 N/A    Created
        //
        public byte NumberHANConfigCmds
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrConfigCmds;
            }
        }

        /// <summary>
        /// Returns the actual size, in octets, each HAN client data record is
        /// capable of supporting.
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07 N/A    Created
        //
        public UInt16 DataSize
        {
            get
            {
                ReadUnloadedTable();

                return m_usDataSize;
            }
        }

        /// <summary>
        /// Gets the Transmit Data Size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public ushort TxDataSize
        {
            get
            {
                ReadUnloadedTable();

                return m_usTxDataSize;
            }
        }

        /// <summary>
        /// Gets the Receive Data Size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public ushort RxDataSize
        {
            get
            {
                ReadUnloadedTable();

                return m_usRxDataSize;
            }
        }

        /// <summary>
        /// Actual number of security keys the HAN Security table is capable of
        /// supporting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07 N/A    Created
        //
        public byte NumberSecurityKeys
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrSecurityKeys;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //
        private void ParseData()
        {
            m_bytNbrClients = m_Reader.ReadByte();
            m_bytNbrConfigCmds = m_Reader.ReadByte();
            m_usDataSize = m_Reader.ReadUInt16();
            m_usTxDataSize = m_Reader.ReadUInt16();
            m_usRxDataSize = m_Reader.ReadUInt16();
            m_bytNbrSecurityKeys = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        private byte m_bytNbrClients;
        private byte m_bytNbrConfigCmds;
        private ushort m_usDataSize;
        private ushort m_usTxDataSize;
        private ushort m_usRxDataSize;
        private byte m_bytNbrSecurityKeys;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2099 class handles the reading of the Actual HAN Limiting
    /// table.
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/03/06 AF  8.00.00 N/A    Created
    //  05/16/08 jrf 1.50.23 114449 Made class public.
    //
    public class CHANMfgTable2099 : AnsiTable
    {
        #region Constants

        private const int ACT_HAN_LIM_TBL_LENGTH = 9;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CHANMfgTable2099 Table2099 = new CHANMfgTable2099(m_PSEM);
        /// </code></example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/06 AF  8.00.00 N/A    Created
        //
        public CHANMfgTable2099(CPSEM psem)
            : base(psem, 2099, ACT_HAN_LIM_TBL_LENGTH, TABLE_TIMEOUT)
        {
            m_bytNbrClients = 0;
            m_bytNbrConfigCmds = 0;
            m_usDataSize = 0;
            m_usTxDataSize = 0;
            m_usRxDataSize = 0;
            m_bytNbrSecurityKeys = 0;
        }

        /// <summary>
        /// Full read of table 2099 (Actual HAN Limiting Table) out of the meter
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/06 AF  8.00.00 N/A    Created
        //  06/12/09 AF  2.20.07        Pulled data reading into a Parse() method
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2099.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the actual number of HAN clients this server is capable of
        /// supporting.
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/06 AF  7.40.00 N/A    Created
        //
        public byte NumberHANClients
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading number of HAN clients"));
                    }
                }

                return m_bytNbrClients;
            }
        }

        /// <summary>
        /// Returns the actual number of commands each HAN client configuration
        /// record is capable of supporting.
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/06 AF  7.40.00 N/A    Created
        //
        public byte NumberHANConfigCmds
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading number of HAN config commands"));
                    }
                }

                return m_bytNbrConfigCmds;
            }
        }

        /// <summary>
        /// Returns the actual size, in octets, each HAN client data record is
        /// capable of supporting.
        /// </summary>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/06 AF  7.40.00 N/A    Created
        //
        public UInt16 DataSize
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading size of HAN client data record"));
                    }
                }

                return m_usDataSize;
            }
        }

        /// <summary>
        /// Gets the Transmit Data Size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public ushort TxDataSize
        {
            get
            {
                ReadUnloadedTable();

                return m_usTxDataSize;
            }
        }

        /// <summary>
        /// Actual number of security keys the HAN Security table is capable of
        /// supporting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/08 AF  1.50.33        Created
        //
        public byte NumberSecurityKeys
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading number of HAN security keys"));
                    }
                }

                return m_bytNbrSecurityKeys;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //
        private void ParseData()
        {
            m_bytNbrClients = m_Reader.ReadByte();
            m_bytNbrConfigCmds = m_Reader.ReadByte();
            m_usDataSize = m_Reader.ReadUInt16();
            m_usTxDataSize = m_Reader.ReadUInt16();
            m_usRxDataSize = m_Reader.ReadUInt16();
            m_bytNbrSecurityKeys = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        private byte m_bytNbrClients;
        private byte m_bytNbrConfigCmds;
        private ushort m_usDataSize;
        private ushort m_usTxDataSize;
        private ushort m_usRxDataSize;
        private byte m_bytNbrSecurityKeys;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2100 class handles the reading of the HAN Client
    /// Configuration table.
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/09/06 AF  8.00.00 N/A    Created
    //
    internal class CHANMfgTable2100 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="table2099">Table 2099 object</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CHANMfgTable2099 Table2099 = new CHANMfgTable2099(m_PSEM);
        /// CHANMfgTable2100 Table2100 = new CHANMfgTable2100(m_PSEM, Table2099); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/09/06 AF  8.00.00 N/A    Created
        //
        public CHANMfgTable2100(CPSEM psem, CHANMfgTable2099 table2099)
            : base(psem, 2100, GetTableLength(table2099), TABLE_TIMEOUT)
        {
            m_ClientCmdRcd = new ClientCmdRcd();
            m_ClientCmdRcd.CmdData = new byte[8];
            for (int iIndex = 0; iIndex < m_ClientCmdRcd.CmdData.Length; iIndex++)
            {
                m_ClientCmdRcd.CmdData[iIndex] = 0xFF;
            }
            m_table2099 = table2099;
            m_ClientCfgRcd = new ClientCfgRcd();
            m_ClientCfgRcd.ClientCmdList = new List<ClientCmdRcd>();
            m_lstHANCfgData = new List<ClientCfgRcd>();
        }

        /// <summary>
        /// Full read of table 2100 (HAN Client Configuration Table) out of the meter
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/09/06 AF  8.00.00 N/A    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2100.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_lstHANCfgData.Clear();

                for (int iCounter = 0; iCounter < m_table2099.NumberHANClients; iCounter++)
                {
                    m_ClientCfgRcd = new ClientCfgRcd();
                    m_ClientCfgRcd.ClientAddress = m_Reader.ReadUInt64();
                    m_ClientCfgRcd.NumberCfgCmds = m_Reader.ReadByte();
                    m_ClientCfgRcd.ClientCmdList = new List<ClientCmdRcd>();
                    for (int iIndex = 0; iIndex < m_table2099.NumberHANConfigCmds; iIndex++)
                    {
                        m_ClientCmdRcd = new ClientCmdRcd();
                        m_ClientCmdRcd.CmdID = m_Reader.ReadByte();
                        m_ClientCmdRcd.PacketVer = m_Reader.ReadByte();
                        m_ClientCmdRcd.SequenceNum = m_Reader.ReadUInt16();
                        m_ClientCmdRcd.CmdData = m_Reader.ReadBytes(8);

                        m_ClientCfgRcd.ClientCmdList.Add(m_ClientCmdRcd);
                    }
                    m_lstHANCfgData.Add(m_ClientCfgRcd);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns a list that is essentially a dump of table 2100
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/09/06 AF  8.00.00 N/A    Created
        //
        public List<ClientCfgRcd> HANConfigData
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Client Configuration Table"));
                    }
                }

                return m_lstHANCfgData;
            }
        }

        /// <summary>
        /// Length in bytes of table 2100
        /// </summary>
        public uint TableLength
        {
            get
            {
                return GetTableLength(m_table2099);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table.  The length depends
        /// upon the number of HAN clients and the number of config commands as
        /// read from table 2099.
        /// </summary>
        /// <param name="table2099">Mfg Table 2099 object</param>
        /// <returns>the length in bytes of table 2100</returns>
        static private uint GetTableLength(CHANMfgTable2099 table2099)
        {
            uint uiClientCmdRcd = (uint)(2 * sizeof(byte) + sizeof(UInt16) + (8 * sizeof(byte)));

            uint uiClientCfgRcd = sizeof(UInt64) + sizeof(byte) + 
                                    (table2099.NumberHANConfigCmds * uiClientCmdRcd);

            return (uint)(table2099.NumberHANClients * uiClientCfgRcd);
        }

        #endregion

        #region Members

        private ClientCmdRcd m_ClientCmdRcd;
        private ClientCfgRcd m_ClientCfgRcd;
        private List<ClientCfgRcd> m_lstHANCfgData;
        private CHANMfgTable2099 m_table2099;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2101 class handles the reading of the HAN Client Data Table
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/07/06 AF  8.00.00 N/A    Created
    //  05/16/08 jrf 1.50.23 114449 Made class public.
    //
    public class CHANMfgTable2101 : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_STIME_DATE = 4;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="table2099">MFG Table 2099 object</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CHANMfgTable2099 Table2099 = new CHANMfgTable2099(m_PSEM);
        /// CHANMfgTable2101 Table2101 = new CHANMfgTable2101(m_PSEM, Table2099);
        /// </code></example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/06 AF  8.00.00 N/A    Created
        //  05/16/08 jrf 1.50.26 114449 Modifed to pass individual values to the GetTableLength
        //                              method instead of the entire table 2099.
        //
        public CHANMfgTable2101(CPSEM psem, CHANMfgTable2099 table2099)
            : base(psem, 2101, GetTableLength(table2099.NumberHANClients, table2099.DataSize), TABLE_TIMEOUT)
        {
            m_lstHANClientData = new List<HANClientDataRcd>();
            m_table2099 = table2099;
        }

        /// <summary>
        /// Table 121 Constructor used when parsing a data file
        /// </summary>
        /// <param name="BinaryReader">A PSEM binary Reader contains the stream for table 2101</param>
        /// <param name="byNumberHANClients">The number of HAN clients</param>
        /// <param name="uiDataSize">The size of the client data record</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 jrf 1.50.26 114449 Created.
        //
        public CHANMfgTable2101(PSEMBinaryReader BinaryReader, byte byNumberHANClients, UInt16 uiDataSize)
            : base(2101, GetTableLength(byNumberHANClients, uiDataSize))
        {
            m_lstHANClientData = new List<HANClientDataRcd>();
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            ParseData(byNumberHANClients, uiDataSize, PSEMBinaryReader.TM_FORMAT.UINT32_TIME);
        }

        /// <summary>
        /// Full read of table 2101 (HAN Client Data Table) out of the meter
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/06 AF  8.00.00 N/A    Created
        //  05/16/08 jrf 1.50.26 114449 Moved the reading of the table to a ParseData method.
        //  03/11/10 AF  2.40.25 146809 Added a check on the number of clients the meter
        //                              is capable of supporting.  If zero, this table
        //                              will be empty.
        //
        public override PSEMResponse Read()
        {
            m_lstHANClientData.Clear();
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2101.Read");

            if (m_table2099.NumberHANClients > 0)
            {
                // If table 2099 shows 0 clients, then this table will be empty 
                // and we shouldn't try to read it.  Just make the client list empty
                Result = base.Read();

                //Populate the member variables that represent the table
                if (PSEMResponse.Ok == Result)
                {
                    m_DataStream.Position = 0;

                    ParseData(m_table2099.NumberHANClients, m_table2099.DataSize, (PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// List that essentially contains a dump of table 2101
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/06 AF  8.00.00 N/A    Created
        //
        public List<HANClientDataRcd> HANClientDataList
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Client Data Table"));
                    }
                }

                return m_lstHANClientData;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 jrf 1.50.26 114449 Created.
        //  06/25/08 AF  1.50.44 116853 This is a hack.  There is a firmware bug
        //                              that initializes the client data size with
        //                              0xFFFF instead of 0 for unused clients. It
        //                              will cause us a lot of pain if we try to read 
        //                              that many bytes so make sure it doesn't happen
        //  04/02/09 AF  2.20.00 116853 This CQ has been fixed but I'm leaving the hack
        //                              in place in case it ever comes up again.
        //
        private void ParseData(byte byNumberHANClients, UInt16 uiDataSize, PSEMBinaryReader.TM_FORMAT TimeFormat)
        {
            HANClientDataRcd rcdClientData;

            for (int iIndex = 0; iIndex < byNumberHANClients; iIndex++)
            {
                rcdClientData = new HANClientDataRcd(uiDataSize);
                rcdClientData.ClientAddress = m_Reader.ReadUInt64();
                rcdClientData.TimeRecorded = m_Reader.ReadSTIME(TimeFormat);
                rcdClientData.ClientDataSize = m_Reader.ReadUInt16();

                // If there is a value in this field that is greater than
                // the max allowed, assume the client is invalid
                if (uiDataSize < rcdClientData.ClientDataSize)
                {
                    rcdClientData.ClientDataSize = 0;
                }

                rcdClientData.ClientData = m_Reader.ReadBytes(uiDataSize);
                m_lstHANClientData.Add(rcdClientData);
            }
        }
        
        
        /// <summary>
        /// Helper method to determine the length of the table depending on the 
        /// data size field of table 2099
        /// </summary>
        /// <param name="byNumberHANClients">The number of HAN clients</param>
        /// <param name="uiDataSize">The size of each HAN client data record</param>
        /// <returns>the length in bytes of table 2101</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 jrf 1.50.26 114449 Modified to pass in number of clients and data size
        //                              instead of the entire table 2099.
        //
        static private uint GetTableLength(byte byNumberHANClients, UInt16 uiDataSize)
        {
            uint uiClientDataRcd = (uint)(sizeof(UInt64) + SIZE_OF_STIME_DATE + sizeof(UInt16)
                                    + (uiDataSize * sizeof(byte)));

            return (uint)(byNumberHANClients * uiClientDataRcd);
        }

        #endregion

        #region Members

        private List<HANClientDataRcd> m_lstHANClientData;
        private CHANMfgTable2099 m_table2099;

        #endregion
    }

    /// <summary>
    /// Handles the writing of the HAN Server Transmit Table
    /// </summary>
    internal class CHANMfgTable2102 : AnsiTable
    {
        #region Constants

        private const int HEADER_SIZE = 11;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public CHANMfgTable2102(CPSEM psem)
            : base (psem, 2102, HEADER_SIZE)
        {
            m_Command = null;
        }

        /// <summary>
        /// Reads the table from the meter. This operation is not supported for this table.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public override PSEMResponse Read()
        {
            throw new InvalidOperationException("Only writes to Table 2102 are supported");
        }

        /// <summary>
        /// Performs an offset read of the table. This operation is not supported for this table
        /// </summary>
        /// <param name="Offset">The offset to read.</param>
        /// <param name="Count">The number of bytes to read.</param>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public override PSEMResponse Read(int Offset, ushort Count)
        {
            throw new InvalidOperationException("Only writes to Table 2102 are supported");
        }

        /// <summary>
        /// Writes the table to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "CHANMfgTable2102.Write");

            m_DataStream.Position = 0;

            if (m_Command != null)
            {
                m_Command.WriteCommand(m_Writer);
            }

            return base.Write();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets the command to write to the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public HANTransmitCommand Command
        {
            set
            {
                m_Command = value;

                if (m_Command != null)
                {
                    ChangeTableSize(m_Command.Size);                    
                }

                State = TableState.Dirty;
            }
        }

        #endregion

        #region Member Variables

        private HANTransmitCommand m_Command;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2104 class handles the reading of the HAN Network 
    /// Information Table
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    public class CHANMfgTable2104 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const UInt64 UNASSIGNED_MAC_ADDR = 0xFFFFFFFFFFFFFFFF;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="table2099">Mfg Table 2099 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/06 AF  8.00.00        Created
        //
        public CHANMfgTable2104(CPSEM psem, CHANMfgTable2099 table2099)
            : base(psem, 2104, GetTableLength(table2099), TABLE_TIMEOUT)
        {
            m_lstServerBinding = new List<HANBindingRcd>();
            m_table2099 = table2099;
        }

        /// <summary>
        /// Full read of table 2104 (HAN Network Information Table) out of the meter
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/06 AF  8.00.00        Created
        //
        public override PSEMResponse Read()
        {
            HANBindingRcd rcdBindingRcd;
            m_lstServerBinding.Clear();

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2104.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_ulServerMACAddr = m_Reader.ReadUInt64();
                m_byChannelNbr = m_Reader.ReadByte();
                m_usPANId = m_Reader.ReadUInt16();

                for (int iIndex = 0; iIndex < m_table2099.NumberHANClients; iIndex++)
                {
                    rcdBindingRcd = new HANBindingRcd();
                    rcdBindingRcd.LongAddress = m_Reader.ReadUInt64();
                    rcdBindingRcd.ShortAddress = m_Reader.ReadUInt16();
                    if (UNASSIGNED_MAC_ADDR != rcdBindingRcd.LongAddress)
                    {
                        m_lstServerBinding.Add(rcdBindingRcd);
                    }
                }
            }
            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// MAC address of the HAN server (Electric Meter)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/06 AF  8.00.00        Created
        //  04/05/11 jrf 2.50.21        Changed to always reread value from meter if 
        //                              it is zero.
        //
        public UInt64 ServerMACAddress
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                
                if (0 == m_ulServerMACAddr)
                {
                    m_TableState = TableState.Expired;
                }

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Network Information Table"));
                    }
                }

                return m_ulServerMACAddr;
            }
        }

        /// <summary>
        /// Current HAN Channel Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/06 AF  8.00.00        Created
        //
        public byte ChannelNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Network Information Table"));
                    }
                }

                return m_byChannelNbr;
            }
        }

        /// <summary>
        /// Current HAN PAN ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/06 AF  8.00.00        Created
        //
        public UInt16 NetworkID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Network Information Table"));
                    }
                }

                return m_usPANId;
            }
        }

        /// <summary>
        /// Current HAN Binding Entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/06 AF  8.00.00        Created
        //
        public List<HANBindingRcd> HANBindingEntries
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Network Information Table"));
                    }
                }

                return m_lstServerBinding;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table depending on the
        /// number of HAN clients specified in table 2099
        /// </summary>
        /// <param name="table2099">MFG Table 2099 object</param>
        /// <returns>length in bytes of table 2104</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/06 AF  8.00.00        Created
        //
        static private uint GetTableLength(CHANMfgTable2099 table2099)
        {
            uint uiBindingRcdLen = sizeof(UInt64) + sizeof(UInt16);

            return (uint)(sizeof(UInt64) + sizeof(byte) + sizeof(UInt16) 
                            + table2099.NumberHANClients * uiBindingRcdLen);
        }

        #endregion

        #region Members

        private CHANMfgTable2099 m_table2099;
        private UInt64 m_ulServerMACAddr;
        private byte m_byChannelNbr;
        private UInt16 m_usPANId;
        private List<HANBindingRcd> m_lstServerBinding;

        #endregion

    }

    /// <summary>
    /// CHANMfgTable2105 is a read/write table that holds the 128 bit encryption 
    /// keys for the HAN.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //
    //
    internal class CHANMfgTable2105 : AnsiTable
    {
        #region Constants

        /// <summary>
        /// A HAN security key records consists of a 16-byte key and a 1-byte key type, 
        /// making the total size of the record 17 bytes.
        /// </summary>
        private const int SIZEOF_HAN_SECURITY_KEY_RECORD = 17;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="table2099"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/08 AF  1.50.34        Created
        //
        public CHANMfgTable2105(CPSEM psem, CHANMfgTable2099 table2099)
            : base(psem, 2105, GetTableLength(table2099.NumberSecurityKeys), TABLE_TIMEOUT)
        {
            m_HANSecurityKeys = new HANKeyRcd[table2099.NumberSecurityKeys];
            m_NumberSecurityKeys = table2099.NumberSecurityKeys;
            for (int iIndex = 0; iIndex < m_NumberSecurityKeys; iIndex++)
            {
                m_HANSecurityKeys[iIndex] = new HANKeyRcd();
            }
        }

        /// <summary>
        /// Writes the table to the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/08 AF  1.50.34        Created
        //  10/24/16 AF  4.70.27 699119 Write only the network key. The link key is no longer used.
        //
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "CHANMfgTable2105.Write");

            // Have to resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write((byte)(m_HANSecurityKeys[0].KeyType));
            for (int iIndex2 = 0; iIndex2 < m_HANSecurityKeys[0].HANKey.Length; iIndex2++)
            {
                m_Writer.Write(m_HANSecurityKeys[0].HANKey[iIndex2]);
            }

            // Let the base class handle the actual writing to the meter
            return base.Write();
        }
        
        /// <summary>
        /// Sets the network and link key member variables.  Should
        /// be used to prepare for writing the 2105 table to the meter.
        /// </summary>
        /// <param name="abyNetworkKey">Unencrypted network security key</param>
        /// <param name="abyLinkKey">Unencrypted global link key</param>
        /// <returns>void</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/08 AF  1.50.37        Created
        //
        public void SetHANSecurityKeys(byte[] abyNetworkKey, byte[] abyLinkKey)
        {
            m_HANSecurityKeys[0].KeyType = HANKeyRcd.HANKeyType.NetworkKey;
            m_HANSecurityKeys[0].HANKey = abyNetworkKey;

            m_HANSecurityKeys[1].KeyType = HANKeyRcd.HANKeyType.GlobalLinkKey;
            m_HANSecurityKeys[1].HANKey = abyLinkKey;
        }

        /// <summary>
        /// Sets the network key member variable.  Should
        /// be used to prepare for writing the 2105 table to the meter.
        /// </summary>
        /// <param name="abyNetworkKey"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  10/24/16 AF  4.70.27  WR 699119  Created. No longer need to update the link key
        //
        public void SetHANNetworkSecurityKey(byte[] abyNetworkKey)
        {
            m_HANSecurityKeys[0].KeyType = HANKeyRcd.HANKeyType.NetworkKey;
            m_HANSecurityKeys[0].HANKey = abyNetworkKey;
        }

        #endregion

        #region Public Properties
        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the size, in bytes, of table 2105.  The size will be the
        /// number of security keys multiplied by the size of one key (128 bits) 
        /// plus the key type (1 byte);
        /// </summary>
        /// <param name="usNumberSecurityKeys">The number of HAN security keys</param>
        /// <returns>the length of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/08 AF  1.50.34        Created
        //
        static private uint GetTableLength(ushort usNumberSecurityKeys)
        {
            return (uint)(usNumberSecurityKeys * SIZEOF_HAN_SECURITY_KEY_RECORD * sizeof(byte));
        }

        #endregion

        #region Members

        private HANKeyRcd[] m_HANSecurityKeys;
        private byte m_NumberSecurityKeys;

        #endregion

    }

    /// <summary>
    /// Channels used by the HAN
    /// </summary>
    [Flags]
    public enum HANChannels : uint
    {
        /// <summary>
        /// None selected
        /// </summary>
        None = 0,
        /// <summary>
        /// Channel 11
        /// </summary>
        Channel11 = 0x00000800,
        /// <summary>
        /// Channel 12
        /// </summary>
        Channel12 = 0x00001000,
        /// <summary>
        /// Channel 13
        /// </summary>
        Channel13 = 0x00002000,
        /// <summary>
        /// Channel 14
        /// </summary>
        Channel14 = 0x00004000,
        /// <summary>
        /// Channel 15
        /// </summary>
        Channel15 = 0x00008000,
        /// <summary>
        /// Channel 16
        /// </summary>
        Channel16 = 0x00010000,
        /// <summary>
        /// Channel 17
        /// </summary>
        Channel17 = 0x00020000,
        /// <summary>
        /// Channel 18
        /// </summary>
        Channel18 = 0x00040000,
        /// <summary>
        /// Channel 19
        /// </summary>
        Channel19 = 0x00080000,
        /// <summary>
        /// Channel 20
        /// </summary>
        Channel20 = 0x00100000,
        /// <summary>
        /// Channel 21
        /// </summary>
        Channel21 = 0x00200000,
        /// <summary>
        /// Channel 22
        /// </summary>
        Channel22 = 0x00400000,
        /// <summary>
        /// Channel 23
        /// </summary>
        Channel23 = 0x00800000,
        /// <summary>
        /// Channel 24
        /// </summary>
        Channel24 = 0x01000000,
        /// <summary>
        /// Channel 25
        /// </summary>
        Channel25 = 0x02000000,
        /// <summary>
        /// Channel 26
        /// </summary>
        Channel26 = 0x04000000,
        /// <summary>
        /// The Default Itron HAN Channels
        /// </summary>
        ItronDefault = Channel11 | Channel15 | Channel20 | Channel25,
        /// <summary>
        /// The non-Default Itron HAN Channels
        /// </summary>
        NonItronDefault = Channel12 | Channel13 | Channel14 | Channel16 | Channel17 
            | Channel18 | Channel19 | Channel21 | Channel22 | Channel23 | Channel24,
    }

    /// <summary>
    /// Possible ways a device can start up using HAN.
    /// </summary>
    // Revision History	
    // MM/DD/YY Who Version ID Number Description
    // -------- --- ------- -- ------ ----------------------------------------------------------
    // 05/09/14 jrf 3.50.91 WR 504003 Redefined HANStartUpOptions Unknown value to HANDisabled.
    public enum HANStartupOptions : byte
    {
        /// <summary>
        /// Coordinatior Only
        /// </summary>
        [EnumDescription("Coordinator")]
        CoordinatorOnly = 0,
        /// <summary>
        /// Coordinatior Only
        /// </summary>
        [EnumDescription("Router/Coordinator")]
        RouterCoordinator = 1,
        /// <summary>
        /// Router Only
        /// </summary>
        [EnumDescription("Router")]
        RouterOnly = 2,
        /// <summary>
        /// Hardcoded Form
        /// </summary>
        [EnumDescription("Hardcoded Form")]
        HardcodedForm = 3,
        /// <summary>
        /// HAN Disabled
        /// </summary>
        [EnumDescription("HAN Disabled")]
        HANDisabled = 255,
    }

    /// <summary>
    /// CHANMfgTable2106 is a read/write table that holds the configuration 
    /// parameters for the HAN.
    /// </summary>
    public class CHANMfgTable2106 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 1000;
        private const int HAN_CONFIG_PARAMS_TBL_LENGTH = 27;
        private const int HAN_CONFIG_PARAMS_TBL_LEN_SP5_1 = 35;
        private const int HAN_CONFIG_VERSION_4 = 4;

        private const ushort CHANNEL_OFFSET = 1;
        private const ushort CHANNEL_LENGTH = 4;

        /// <summary>
        /// The number of bytes to offset the table write when setting the security mode.
        /// </summary>
        private const ushort SECURITY_MODE_OFFSET = 8;
        /// <summary>
        /// The number of bytes that make up the security mode setting.
        /// </summary>
        private const ushort SECURITY_MODE_LENGTH = 1;
        /// <summary>
        /// The number of bytes to offset the table write when setting the CBKE mode.
        /// </summary>
        private const ushort CBKE_MODE_OFFSET = 9;
        /// <summary>
        /// The number of bytes that make up the CBKE mode setting.
        /// </summary>
        private const ushort CBKE_MODE_LENGTH = 1;
        /// <summary>
        /// The number of bytes to offset the table write when setting the Device Auth mode.
        /// </summary>
        private const ushort DEVICE_AUTH_MODE_OFFSET = 10;
        /// <summary>
        /// The number of bytes that make up the Device Auth mode setting.
        /// </summary>
        private const ushort DEVICE_AUTH_MODE_LENGTH = 1;

        private const ushort MULTIPLIER_AND_DIVISOR_OFFSET = 27;
        private const ushort MULTIPLIER_AND_DIVISOR_LENGTH = 8;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="fltFWRev">The register f/w version running in the meter</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/11/09 AF  2.20.04        Created
        //  09/17/09 AF  2.30.02 140524 The size of the table changed starting in
        //                              SR 2.0 SP5.1
        //
        public CHANMfgTable2106(CPSEM psem, float fltFWRev)
            : base(psem, 2106, GetTableLength(fltFWRev), TABLE_TIMEOUT)
        {
            m_fltFWRev = fltFWRev;

            // Initialize the Multiplier and Divisor to 1
            m_uiSimpleMeteringMultiplier = 1;
            m_uiSimpleMeteringDivisor = 1;
        }

        /// <summary>
        /// Full read of table 2106 (HAN Configuration Parameters Table) out of the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/11/09 AF  2.20.04        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2106.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_bytVersion = m_Reader.ReadByte();

                if (HAN_CONFIG_VERSION_4 == m_bytVersion)
                {
                    ParseData();
                }
            }

            return Result;
        }

        /// <summary>
        /// Gets the name of the security profile that was selected based on the Security Mode,
        /// Device Authentication Mode, and the CBKE Mode.
        /// </summary>
        /// <param name="bySecurityMode">The security mode selected.</param>
        /// <param name="byDeviceAuthMode">The device authentication mode selected.</param>
        /// <param name="byCBKEMode">The CBKE mode selected.</param>
        /// <returns>The name of the security profile</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/30/09 RCG 2.30.16        Created

        public static string GetHANSecurityProfile(byte bySecurityMode, byte byDeviceAuthMode, byte byCBKEMode)
        {
            string strHANSecProfile = "Unknown";

            if (bySecurityMode == 2 && byDeviceAuthMode == 2 && byCBKEMode == 2)
            {
                strHANSecProfile = "HAN IPP Default Security Profile";
            }
            else if (bySecurityMode == 5 && byDeviceAuthMode == 3 && byCBKEMode == 0)
            {
                strHANSecProfile = "HAN SE Security Profile 1";
            }
            else if (bySecurityMode == 5 && byDeviceAuthMode == 0 && byCBKEMode == 0)
            {
                strHANSecProfile = "HAN SE Security Profile 2";
            }
            else if (bySecurityMode == 5 && byDeviceAuthMode == 3 && byCBKEMode == 1)
            {
                strHANSecProfile = "HAN IPP Controlled Security Profile";
            }

            return strHANSecProfile;
        }

        /// <summary>
        /// Gets the HAN Security Mode Description using the provided byte.
        /// </summary>
        /// <param name="securityMode">The Security Mode byte</param>
        /// <returns>The description of the security mode.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public static string GetHANSecurityMode(byte securityMode)
        {
            string HanSecurityMode = "Unknown";

            switch (securityMode)
            {
                case 0:
                    HanSecurityMode = "HA Security";
                    break;
                case 1:
                    HanSecurityMode = "Hashed Link Key";
                    break;
                case 2:
                    HanSecurityMode = "Global Link Key w/Preconfig nwk key support";
                    break;
                case 3:
                    HanSecurityMode = "Global Link Key";
                    break;
                case 4:
                    HanSecurityMode = "Preconfig Nwk Key";
                    break;
                case 5:
                    HanSecurityMode = "Std Link Key w/Preconfig nwk key support";
                    break;
                case 6:
                    HanSecurityMode = "Std Link Key";
                    break;
                default:
                    break;
            }

            return HanSecurityMode;
        }

        /// <summary>
        /// Gets the HAN Device Auth Mode Description using the provided byte.
        /// </summary>
        /// <param name="deviceAuthMode">The Device Auth Mode byte</param>
        /// <returns>The description of the Device Auth mode.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public static string GetHanDeviceAuthMode(byte deviceAuthMode)
        {
            string HanDeviceAuthMode = "Unknown";

            switch (deviceAuthMode)
            {
                case 0:
                    HanDeviceAuthMode = "Explicit";
                    break;
                case 1:
                    HanDeviceAuthMode = "Deny";
                    break;
                case 2:
                    HanDeviceAuthMode = "Allow All";
                    break;
                case 3:
                    HanDeviceAuthMode = "Explicit and Itron Policy";
                    break;
                default:
                    break;
            }

            return HanDeviceAuthMode;
        }

        /// <summary>
        /// Gets the HAN CBKE Mode Description using the provided byte.
        /// </summary>
        /// <param name="cbkeMode">The CBKE Mode byte</param>
        /// <returns>The description of the CBKE mode.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public static string GetHanCbkeMode(byte cbkeMode)
        {
            string HanCbkeMode = "Unknown";

            switch (cbkeMode)
            {
                case 0:
                    HanCbkeMode = "SE Required";
                    break;
                case 1:
                    HanCbkeMode = "SE Restricted";
                    break;
                case 2:
                    HanCbkeMode = "SE Unrestricted";
                    break;
                default:
                    break;
            }

            return HanCbkeMode;
        }

        /// <summary>
        /// Gets the description for the inter PAN mode
        /// </summary>
        /// <param name="byInterPanMode">The Inter PAN mode value</param>
        /// <returns>The description of the Inter PAN mode</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/06/09 RCG 2.30.16 144719 Created

        public static string GetInterPANMode(byte byInterPanMode)
        {
            string strInterPANMode = "";

            switch(byInterPanMode)
            {
                case 0:
                {
                    strInterPANMode = "Deny All";
                    break;
                }
                case 1:
                {
                    strInterPANMode = "Allow Price Cluster";
                    break;
                }
                case 2:
                {
                    strInterPANMode = "Allow Messaging Cluster";
                    break;
                }
                case 255:
                {
                    strInterPANMode = "Allow All Clusters";
                    break;
                }
            }

            return strInterPANMode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the channels that have been used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/27/11 RCG 2.51.30        Created

        public HANChannels ChannelsUsed
        {
            get
            {
                ReadUnloadedTable();

                return (HANChannels)m_uiChannelMask;
            }
            set
            {
                m_uiChannelMask = (uint)value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Returns the start up options field from table 2106.  We can determine
        /// whether or not the HAN has been disabled from this field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/11/09 AF  2.20.04        Created
        //  03/19/13 MP  2.80.09        Added set. Value is read-only in meter. Set was added for testing only.
        //
        public byte StartupOptions
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                //Read Table
                Result = Read();
                if (PSEMResponse.Ok != Result)
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading HAN Configuration Parameters Table"));
                }

                return m_bytStartupOptions;
            }
            set
            {
                // ****** Set is for testing only. Even if value is changed here, the meter will change it right back because the value is read only.
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    m_bytStartupOptions = value;
                    Result = Write();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE,Result,
                            "Errow writing HAN Configureation Parameters Table"));
                    }
                }              
            }
        }

        /// <summary>
        /// Returns the start up options field from table 2106.  We can determine
        /// whether or not the HAN has been disabled from this field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ------------------------------------------
        //  08/27/13 MP  2.80.09        Created

        public byte ConfigVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HAN Configuration Parameters Table"));
                    }
                }
                return m_bytVersion;
            }
        }

        /// <summary>
        /// Returns the HANSecurityProfile based on CBKE mode, DeviceAuth mode, Security mode 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/09 MMD  2.30.15        Created
        //
        public string HANSecurityProfile
        {

            get
            {
                string strHANSecProfile = "";
                ReadUnloadedTable();

                strHANSecProfile = GetHANSecurityProfile(m_bytSecurityMode, m_bytDeviceAuthMode, m_bytCBKEMode);

                return strHANSecProfile;
            }
        }

        /// <summary>
        /// Gets the Inter PAN mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/06/09 RCG 2.30.16 144719 Created

        public string InterPANMode
        {
            get
            {
                string strInterPANMode = "";
                ReadUnloadedTable();

                strInterPANMode = GetInterPANMode(m_bytInterPanMode);

                return strInterPANMode;
            }
        }

        /// <summary>
        /// Gets/Sets the HAN Security Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public byte SecurityMode
        {
            get
            {
                ReadUnloadedTable();
                return m_bytSecurityMode;
            }
            set
            {
                m_bytSecurityMode = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets the HAN Security Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public string SecurityModeDescription
        {
            get
            {
                return GetHANSecurityMode(SecurityMode);            
            }
        }

        /// <summary>
        /// Gets/Sets the HAN Device Auth Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public byte DeviceAuthMode
        {
            get
            {
                ReadUnloadedTable();
                return m_bytDeviceAuthMode;
            }
            set
            {
                m_bytDeviceAuthMode = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets the HAN Device Auth Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public string DeviceAuthModeDescription
        {
            get
            {
                return GetHanDeviceAuthMode(DeviceAuthMode);
            }
        }

        /// <summary>
        /// Gets/Sets the HAN CBKE Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public byte CbkeMode
        {
            get
            {
                ReadUnloadedTable();
                return m_bytCBKEMode;
            }
            set
            {
                m_bytCBKEMode = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets the HAN Security Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public string CbkeModeDescription
        {
            get
            {
                return GetHanCbkeMode(CbkeMode); 
            }
        }

        /// <summary>
        /// Gets the ZigBee output power level.  Should be a value from -30 to 3.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 jrf 2.52.00 177455 Created
        //
        public sbyte? PowerLevel
        {
            get
            {
                ReadUnloadedTable();

                return m_sbyPowerLevel;
            }
        }
      
        /// <summary>
        /// Gets or sets the Multiplier used for Simple Metering values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/12 RCG 2.60.28        Created

        public uint SimpleMeteringMultiplier
        {
            get
            {
                ReadUnloadedTable();

                return m_uiSimpleMeteringMultiplier;
            }
            set
            {
                m_uiSimpleMeteringMultiplier = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Divisor used for Simple Metering values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/12 RCG 2.60.28        Created

        public uint SimpleMeteringDivisor
        {
            get
            {
                ReadUnloadedTable();

                return m_uiSimpleMeteringDivisor;
            }
            set
            {
                m_uiSimpleMeteringDivisor = value;
                State = TableState.Dirty;
            }
        }
      
        #endregion

        #region Internal Methods

        /// <summary>
        /// Writes the channels to the meter.
        /// </summary>
        /// <returns>The result of the write</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/27/11 RCG 2.51.30        Created
        
        public PSEMResponse WriteChannels()
        {
            m_DataStream.Position = CHANNEL_OFFSET;
            m_Writer.Write(m_uiChannelMask);

            return Write(CHANNEL_OFFSET, CHANNEL_LENGTH);
        }


        /// <summary>
        /// Sets the HAN Security Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        /// <returns>The PSEM Response code.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public PSEMResponse WriteSecurityMode()
        {
            m_DataStream.Position = SECURITY_MODE_OFFSET;
            m_Writer.Write(m_bytSecurityMode);

            return Write(SECURITY_MODE_OFFSET, SECURITY_MODE_LENGTH);
        }

        /// <summary>
        /// Sets the HAN Device Auth Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        /// <returns>The PSEM Response code.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public PSEMResponse WriteDeviceAuthMode()
        {
            m_DataStream.Position = DEVICE_AUTH_MODE_OFFSET;
            m_Writer.Write(m_bytDeviceAuthMode);

            return Write(DEVICE_AUTH_MODE_OFFSET, DEVICE_AUTH_MODE_LENGTH);
        }

        /// <summary>
        /// Sets the HAN Security Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        /// <returns>The PSEM Response code.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public PSEMResponse WriteCbkeMode()
        {
            m_DataStream.Position = CBKE_MODE_OFFSET;
            m_Writer.Write(m_bytCBKEMode);

            return Write(CBKE_MODE_OFFSET, CBKE_MODE_LENGTH);
        }


        /// <summary>
        /// Writes the Multiplier and Divisor fields to the meter using an offset write.
        /// </summary>
        /// <returns>The Result of the write</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/12 RCG 2.60.27        Created
        
        internal PSEMResponse WriteSimpleMeteringMultiplierAndDivisor()
        {
            PSEMResponse Response = PSEMResponse.Onp;

            // These values did not exist prior to SP 5.1 so return ONP
            if (VersionChecker.CompareTo(m_fltFWRev, CENTRON_AMI.VERSION_2_SP5_1) >= 0)
            {
                m_DataStream.Position = MULTIPLIER_AND_DIVISOR_OFFSET;

                m_Writer.Write(m_uiSimpleMeteringMultiplier);
                m_Writer.Write(m_uiSimpleMeteringDivisor);

                Response = Write(MULTIPLIER_AND_DIVISOR_OFFSET, MULTIPLIER_AND_DIVISOR_LENGTH);
            }

            return Response;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/11/09 AF  2.20.04        Created
        //  09/17/09 AF  2.30.02 140524 New fields were added to 2106 starting with
        //                              SR 2.0 SP5.1
        //
        private void ParseData()
        {
            m_uiChannelMask = m_Reader.ReadUInt32();
            m_sbyPowerLevel = m_Reader.ReadSByte();
            m_bytStartupOptions = m_Reader.ReadByte();
            m_bytMinLQIToJoin = m_Reader.ReadByte();
            m_bytSecurityMode = m_Reader.ReadByte();
            m_bytCBKEMode = m_Reader.ReadByte();
            m_bytDeviceAuthMode = m_Reader.ReadByte();
            m_bytLinkKeyAuthMode = m_Reader.ReadByte();
            m_bytInterPanMode = m_Reader.ReadByte();
            byte[] abyTemp = m_Reader.ReadBytes(14);

            if (VersionChecker.CompareTo(m_fltFWRev, CENTRON_AMI.VERSION_2_SP5_1) >= 0)
            {
                m_uiSimpleMeteringMultiplier = m_Reader.ReadUInt32();
                m_uiSimpleMeteringDivisor = m_Reader.ReadUInt32();
            }
        }

        /// <summary>
        /// Determines the length of table 2106 based on the firmware version
        /// in the meter
        /// </summary>
        /// <param name="fltFWRev">the firmware version of the register f/w</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 AF  2.30.01 140524 Created to handle new fields added in SR2.0 SP5.1
        //
        static private uint GetTableLength(float fltFWRev)
        {
            uint uiTableLength;

            if (VersionChecker.CompareTo(fltFWRev, CENTRON_AMI.VERSION_2_SP5_1) >= 0)
            {
                uiTableLength = HAN_CONFIG_PARAMS_TBL_LEN_SP5_1;
            }
            else
            {
                uiTableLength = HAN_CONFIG_PARAMS_TBL_LENGTH;
            }

            return uiTableLength;
        }

        #endregion

        #region Members

        private byte m_bytVersion;
        private UInt32 m_uiChannelMask;
        private sbyte? m_sbyPowerLevel = null;
        private byte m_bytStartupOptions;
        private byte m_bytMinLQIToJoin;
        private byte m_bytSecurityMode;
        private byte m_bytCBKEMode;
        private byte m_bytDeviceAuthMode;  
        private byte m_bytLinkKeyAuthMode;
        private byte m_bytInterPanMode;
        private float m_fltFWRev;
        private uint m_uiSimpleMeteringMultiplier;
        private uint m_uiSimpleMeteringDivisor;

        #endregion
    }

    /// <summary>
    /// OpenWay MFG Table 2107 - HAN Stat Table
    /// </summary>
    internal class CHANMfgTable2107 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device.</param>
        /// <param name="table0">The table 0 object for the current device</param>
        /// <param name="table2098">The MFG Table 2098 table for the current device.</param>
        /// <param name="table2128">The MFG Table 2128 table for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 RCG 2.20.02 N/A    Created
        // 06/11/09 AF  2.20.07 133864 Corrected the tables on which this one depends
        // 01/02/14 AF  3.50.17 TQ9512 Removed the bool parameters for determining presence of extra fields
        //                             and added boolean for allowing table resizing
        //
        public CHANMfgTable2107(CPSEM psem, CTable00 table0, CHANMfgTable2098 table2098, CHANMfgTable2128 table2128)
            : base(psem, 2107, DetermineSize(table2098, table2128, table0.LTIMESize), TABLE_TIMEOUT)
        {
            m_Table0 = table0;
            m_Table2098 = table2098;
            m_Table2128 = table2128;

            m_CurrentMeterTime = null;
            m_HANJoiningExpirationDate = null;

            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 RCG 2.20.02 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2107.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current state of the HAN
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/14 AF  3.50.41 na     Created as a way to check on the status of the HAN
        //
        public byte CurrentNetworkStatus
        {
            get
            {
                //The network status can change.  Making sure we get the most up to date value.
                m_TableState = TableState.Expired;
                ReadUnloadedTable();

                return m_CurrentNetworkStatus;
            }
        }

        /// <summary>
        /// Gets whether or not HAN joining is currently enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 RCG 2.20.02 N/A    Created
        // 06/10/09 jrf 2.20.08 n/a    Modified to ensure this value is always read
        //                             anew from the table each time it is accessed.
        //
        public bool IsHANJoiningEnabled
        {
            get
            {
                //The join window length can change.  Making sure we get the most up to date value.
                m_TableState = TableState.Expired;
                ReadUnloadedTable();

                // Joining is enabled if the Join Window length is something other than 0
                return m_JoinWindowLength > 0;
            }
        }

        /// <summary>
        /// Gets the HAN multiplier.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/06/09 jrf 2.20.03 n/a    Created.
        //
        public UInt32 HANMultiplier
        {
            get
            {
                ReadUnloadedTable();

                return m_uiMultiplier;
            }
        }

        /// <summary>
        /// Gets the HAN divisor.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/06/09 jrf 2.20.03 n/a    Created.
        //
        public UInt32 HANDivisor
        {
            get
            {
                ReadUnloadedTable();

                return m_uiDivisor;
            }
        }

        /// <summary>
        /// Gets whether or not the current ZigBee FW is compatible with the register FW
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/12/10 RCG 2.41.01 n/a    Created.
        // 01/02/14 AF  3.50.17 TQ9512 Use the data stream length for determining presence of the field
        //
        public bool IsZigBeeFWCompatible
        {
            get
            {
                // Older FW should always be compatible
                bool bCompatible = true;

                if (m_DataStream.Length >= m_MinimumTableSize + 4)
                {
                    ReadUnloadedTable();

                    bCompatible = m_ZigBeeFWCompatible;
                }

                return bCompatible;
            }
        }

        /// <summary>
        /// Gets the minimum ZigBee FW version compatible with the register FW
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/12/10 RCG 2.41.01 n/a    Created.
        // 01/02/14 AF  3.50.17 TQ9512 Use the data stream length for determining presence of the field
        //
        public byte MinZigBeeVersion
        {
            get
            {
                byte byVersion = 0;

                if (m_DataStream.Length >= m_MinimumTableSize + 4)
                {
                    ReadUnloadedTable();

                    byVersion = m_MinZigBeeVersion;
                }

                return byVersion;
            }
        }

        /// <summary>
        /// Gets the minimum ZigBee FW revision compatible with the register FW
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/12/10 RCG 2.41.01 n/a    Created.
        // 01/02/14 AF  3.50.17 TQ9512 Use the data stream length for determining presence of the field
        //
        public byte MinZigBeeRevision
        {
            get
            {
                byte byRevision = 0;

                if (m_DataStream.Length >= m_MinimumTableSize + 4)
                {
                    ReadUnloadedTable();

                    byRevision = m_MinZigBeeRevision;
                }

                return byRevision;
            }

        }

        /// <summary>
        /// Gets the minimum ZigBee FW build compatible with the register FW
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/12/10 RCG 2.41.01 n/a    Created.
        // 01/02/14 AF  3.50.17 TQ9512 Use the data stream length for determining presence of the field
        //
        public byte MinZigBeeBuild
        {
            get
            {
                byte byBuild = 0;

                if (m_DataStream.Length >= m_MinimumTableSize + 4)
                {
                    ReadUnloadedTable();

                    byBuild = m_MinZigBeeBuild;
                }

                return byBuild;
            }
        }

        /// <summary>
        /// Gets the Date and Time that HAN Joining will expire
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/01/10 RCG 2.70.35        Created.
        
        public DateTime? HANJoiningExpirationDate
        {
            get
            {
                ReadUnloadedTable();

                return m_HANJoiningExpirationDate;
            }
        }

        /// <summary>
        /// Gets the Current Meter Time reported to HAN Devices
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/01/10 RCG 2.70.35        Created.
        
        public DateTime? CurrentMeterTime
        {
            get
            {
                ReadUnloadedTable();

                return m_CurrentMeterTime;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to get data out of the Binary Reader and into member variables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 RCG 2.20.02 N/A    Created
        // 05/06/09 jrf 2.20.03 n/a    Adding HAN multiplier and divisor to table.
        // 06/11/09 AF  2.20.07 133864 Corrected the tables on which this one depends
        // 01/02/14 AF  3.50.17 TQ9512 Use the data stream length for determining presence of fields
        //
        private void ParseData()
        {
            m_TimeToComeUp = m_Reader.ReadUInt16();
            m_CommFailures = m_Reader.ReadByte();
            m_MCUResets = m_Reader.ReadByte();
            m_Active = m_Reader.ReadByte();
            m_BuildUpdated = m_Reader.ReadByte();
            m_CurrentNetworkStatus = m_Reader.ReadByte();
            m_OldNetworkStatus = m_Reader.ReadByte();
            m_HeartbeatTimer = m_Reader.ReadUInt16();
            m_TotalNumResets = m_Reader.ReadUInt16();
            m_SerialMsgTx = m_Reader.ReadUInt16();
            m_SerialMsgTxFailed = m_Reader.ReadByte();
            m_SerialMsgRx = m_Reader.ReadUInt16();
            m_SerialMsgRxDiscarded = m_Reader.ReadByte();
            m_SerialMsgRxFailed = m_Reader.ReadByte();
            m_SerialMsgRxOverflow = m_Reader.ReadByte();
            m_LastOTATypes = m_Reader.ReadBytes(m_Table2098.NumberHANClients);
            m_ClientBindingTableIndexes = m_Reader.ReadBytes(m_Table2128.NumberAMIDevices);
            m_DebugCommand = m_Reader.ReadByte();
            m_DebugData = m_Reader.ReadBytes(2);
            m_InsDemand = m_Reader.ReadInt32();
            m_InsVA = m_Reader.ReadUInt32();
            m_JoinWindowLength = m_Reader.ReadByte();
            m_uiMultiplier = m_Reader.ReadUInt32();
            m_uiDivisor = m_Reader.ReadUInt32();

            if (m_DataStream.Length >= m_MinimumTableSize + 4)
            {
                m_MinZigBeeVersion = m_Reader.ReadByte();
                m_MinZigBeeRevision = m_Reader.ReadByte();
                m_MinZigBeeBuild = m_Reader.ReadByte();
                m_ZigBeeFWCompatible = m_Reader.ReadBoolean();
            }

            if (m_DataStream.Length >= m_MinimumTableSize + 4 + (2 * m_Table0.LTIMESize))
            {
                m_HANJoiningExpirationDate = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
                m_CurrentMeterTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
            }
        }

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="table2098">The MFG Table 2098 table for the current device.</param>
        /// <param name="table2128">The MFG Table 2129 table for the current device.</param>
        /// <param name="sizeOfLTIME">The size of an LTIME date</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 RCG 2.20.02 N/A    Created
        // 05/06/09 jrf 2.20.03 n/a    Increased static table size for HAN multiplier and divisor.
        // 06/11/09 AF  2.20.07 133864 Corrected the tables on which this one depends
        // 01/02/14 AF  3.50.17 TW9512 The ZigBee f/w and time stamp fields will not be there pre-Carbon
        //                             but we now allow this table to be an unexpected size.
        //
        private static uint DetermineSize(CHANMfgTable2098 table2098, CHANMfgTable2128 table2128, uint sizeOfLTIME)
        {
            uint uiTableSize = 40; // Static data size

            // Add in Last OTA type
            uiTableSize += table2098.NumberHANClients;

            // Add in Client Binding Table index
            uiTableSize += table2128.NumberAMIDevices;

            m_MinimumTableSize = uiTableSize;

            // Add the ZigBee f/w fields and the time stamps - present in Carbon and above f/w
            uiTableSize += 4 + (2 * sizeOfLTIME);

            return uiTableSize;
        }

        #endregion

        #region Member Variables

        private CHANMfgTable2098 m_Table2098;
        private CHANMfgTable2128 m_Table2128;
        private CTable00 m_Table0;

        private ushort m_TimeToComeUp;
        private byte m_CommFailures;
        private byte m_MCUResets;
        private byte m_Active;
        private byte m_BuildUpdated;
        private byte m_CurrentNetworkStatus;
        private byte m_OldNetworkStatus;
        private ushort m_HeartbeatTimer;
        private ushort m_TotalNumResets;
        private ushort m_SerialMsgTx;
        private byte m_SerialMsgTxFailed;
        private ushort m_SerialMsgRx;
        private byte m_SerialMsgRxDiscarded;
        private byte m_SerialMsgRxFailed;
        private byte m_SerialMsgRxOverflow;
        private byte[] m_LastOTATypes;
        private byte[] m_ClientBindingTableIndexes;
        private byte m_DebugCommand;
        private byte[] m_DebugData;
        private int m_InsDemand;
        private uint m_InsVA;
        private byte m_JoinWindowLength;
        private UInt32 m_uiMultiplier;
        private UInt32 m_uiDivisor;

        private byte m_MinZigBeeVersion;
        private byte m_MinZigBeeRevision;
        private byte m_MinZigBeeBuild;
        private bool m_ZigBeeFWCompatible;

        private DateTime? m_HANJoiningExpirationDate;
        private DateTime? m_CurrentMeterTime;
        private static UInt32 m_MinimumTableSize;

        #endregion
    }

    /// <summary>
    /// The CHANMfgTable2128 class handles the reading of the Dimension AMI HAN 
    /// Limiting table
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/11/09 AF  2.20.07        Created
    //
    public class CHANMfgTable2128 : AnsiTable
    {
        #region Constants

        private const int DIM_AMI_HAN_LIM_TBL_LENGTH_PRE_SP5 = 11;
        private const int DIM_AMI_HAN_LIM_TBL_LENGTH = 15;
        private const int DIM_AMI_HAN_LIM_TBL_LENGTH_BERYLLIUM = 16;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// HAN Mfg Table 2128 is a read/write table that defines the maximum values
        /// for the AMI HAN decade
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="fltFWRev">The firmware version and revision of the meter</param>
        /// <param name="hardwareVersion">The hardware version of the meter</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CHANMfgTable2128 Table2128 = new CHANMfgTable2128(m_PSEM);
        /// </code></example>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //  09/18/12 MAH 2.70.17        Added support for Beryllium extensions
        //
        public CHANMfgTable2128(CPSEM psem, float fltFWRev, float hardwareVersion)
            : base(psem, 2128, GetTableLength(fltFWRev, hardwareVersion), TABLE_TIMEOUT)
        {
            m_bytNbrAMIDevices = 0;
            m_bytNbrRegServices = 0;
            m_usNbrDRLCMsgs = 0;
            m_usMessagingMsgLen = 0;
            m_usEncryptCertLen = 0;
            m_bytNbrRsps = 0;
            m_bytRateLabelLen = 0;
            m_bytNbrHANPrices = 0;
            m_bytActTbl2136RecLen = 0;
            m_bytActTbl2136RecCnt = 0;
            m_bytNbrTierLabels = 0;
            m_bytTierLabelLen = 0;
            m_bytActTbl2136ExtendedRecLen = 0;
            m_fltFWRev = fltFWRev;
            m_HWVersion = hardwareVersion;
        }

        /// <summary>
        /// Full read of table 2128
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //  09/17/09 AF  2.30.01        Removed the CE code. It was not needed
        //  09/18/12 MAH 2.70.17        Added support for Beryllium extensions
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2128.Read");
            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_bytNbrAMIDevices = m_Reader.ReadByte();
                m_bytNbrRegServices = m_Reader.ReadByte();
                m_usNbrDRLCMsgs = m_Reader.ReadUInt16();
                m_usMessagingMsgLen = m_Reader.ReadUInt16();
                m_usEncryptCertLen = m_Reader.ReadUInt16();
                m_bytNbrRsps = m_Reader.ReadByte();
                m_bytRateLabelLen = m_Reader.ReadByte();
                m_bytNbrHANPrices = m_Reader.ReadByte();

                if (VersionChecker.CompareTo(m_fltFWRev, CENTRON_AMI.VERSION_2_SP5) >= 0)
                {
                    m_bytActTbl2136RecLen = m_Reader.ReadByte();
                    m_bytActTbl2136RecCnt = m_Reader.ReadByte();
                    m_bytNbrTierLabels = m_Reader.ReadByte();
                    m_bytTierLabelLen = m_Reader.ReadByte();
                }

                if (VersionChecker.CompareTo(m_fltFWRev, CENTRON_AMI.VERSION_6_0_MICHIGAN) >= 0
                    && VersionChecker.CompareTo(m_HWVersion, CENTRON_AMI.HW_VERSION_3_0) >= 0)
                {
                    m_bytActTbl2136ExtendedRecLen = m_Reader.ReadByte();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Actual number of AMI clients this server is capable of supporting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  1.50.33        Created
        //
        public byte NumberAMIDevices
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrAMIDevices;
            }
        }

        /// <summary>
        /// Actual number of services each AMI HAN device is capable of registering for
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //
        public byte NumberRegServices
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrRegServices;
            }
        }

        /// <summary>
        /// Actual length, in octets, of encryption certificates
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //
        public ushort EncryptCertLen
        {
            get
            {
                ReadUnloadedTable();

                return m_usEncryptCertLen;
            }
        }

        /// <summary>
        /// Actual number of responses the AMI_HAN_RSP_LOG_TBL (table 2131) is 
        /// capable of supporting per device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //
        public byte NbrRsps
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrRsps;
            }
        }

        /// <summary>
        /// Actual number of prices the AMI_HAN_PRICE_TBL (table 2134) can
        /// store.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF 2.20.07       Created
        //
        public byte NumberHANPrices
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrHANPrices;
            }
        }

        /// <summary>
        /// Actual number of tier labels in the AMI_HAN_PRICE_TBL (table 2134).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF 2.20.07       Created
        //
        public byte NumberTierLabels
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrTierLabels;
            }
        }

        /// <summary>
        /// Actual size of the rate label in the AMI_HAN_PRICE_TBL (table 2134).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF 2.20.07       Created
        //
        public byte RateLabelLength
        {
            get
            {
                ReadUnloadedTable();

                return m_bytRateLabelLen;
            }
        }

        /// <summary>
        /// Actual length of the tier labels in the AMI_HAN_PRICE_TBL (table 2134).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF 2.20.07       Created
        //
        public byte TierLabelLength
        {
            get
            {
                ReadUnloadedTable();

                return m_bytTierLabelLen;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the length of table 2128 (ITRON 80) based on f/w version.
        /// The length increased by 4 bytes starting with SP5.
        /// </summary>
        /// <param name="fltFWRev">The version.revision of the register f/w
        /// in the meter.</param>
        /// <param name="hardwareVersion">The hardware version of the meter</param>
        /// <returns>The length of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF 2.20.07         Created
        //  09/18/12 MAH 2.70.17        Added support for Beryllium extensions
        static private uint GetTableLength(float fltFWRev, float hardwareVersion)
        {
            uint uiTableLength;

            if (VersionChecker.CompareTo(fltFWRev, CENTRON_AMI.VERSION_6_0_MICHIGAN) >= 0
                && VersionChecker.CompareTo(hardwareVersion, CENTRON_AMI.HW_VERSION_3_0) >= 0)
            {
                uiTableLength = DIM_AMI_HAN_LIM_TBL_LENGTH_BERYLLIUM;
            }
            else if (VersionChecker.CompareTo(fltFWRev, CENTRON_AMI.VERSION_2_SP5) >= 0)
            {
                uiTableLength = DIM_AMI_HAN_LIM_TBL_LENGTH;
            }
            else
            {
                uiTableLength = DIM_AMI_HAN_LIM_TBL_LENGTH_PRE_SP5;
            }

            return uiTableLength;
        }

        #endregion

        #region Members

        private byte m_bytNbrAMIDevices;
        private byte m_bytNbrRegServices;
        private ushort m_usNbrDRLCMsgs;
        private ushort m_usMessagingMsgLen;
        private ushort m_usEncryptCertLen;
        private byte m_bytNbrRsps;
        private byte m_bytRateLabelLen;
        private byte m_bytNbrHANPrices;
        private byte m_bytActTbl2136RecLen;
        private byte m_bytActTbl2136RecCnt;
        private byte m_bytNbrTierLabels;
        private byte m_bytTierLabelLen;
        private byte m_bytActTbl2136ExtendedRecLen;
        private float m_fltFWRev;
        private float m_HWVersion;

        #endregion

    }

    /// <summary>
    /// The CHANMfgTable2129 class handles the reading of the Actual AMI HAN 
    /// Limiting table
    /// </summary>
    /// <remarks>
    /// This table is supported only by OpenWay meters.
    /// </remarks>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/10/08 AF  1.50.34        Created
    //  04/21/09 AF  2.20.02 132587 The size of table 2129 changed starting with
    //                              SR 2.0 SP5
    //  09/19/12 PGH 2.70.18        The size of table 2129 changes again with HW 3.x Beryllium
    //
    public class CHANMfgTable2129 : AnsiTable
    {
        #region Constants

        private const int ACT_AMI_HAN_LIM_TBL_LENGTH_PRE_SP5 = 11;
        private const int ACT_AMI_HAN_LIM_TBL_LENGTH_PRE_BERYLLIUM = 15;
        private const int ACT_AMI_HAN_LIM_TBL_LENGTH = 16;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// HAN Mfg Table 2129 is a read/write table that defines the maximum values
        /// for the AMI HAN decade
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="fltFWRev">The firmware version and revision of the meter</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CHANMfgTable2129 Table2129 = new CHANMfgTable2129(m_PSEM);
        /// </code></example>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/09/08 AF  1.50.33        Created
        //  04/21/09 AF  2.20.02 132587 Added new fields
        //
        public CHANMfgTable2129(CPSEM psem, float fltFWRev)
            : base(psem, 2129, GetTableLength(fltFWRev), TABLE_TIMEOUT)
        {
            m_bytNbrAMIDevices = 0;
            m_bytNbrRegServices = 0;
            m_usNbrDRLCMsgs = 0;
            m_usMessagingMsgLen = 0;
            m_usEncryptCertLen = 0;
            m_bytNbrRsps = 0;
            m_bytRateLabelLen = 0;
            m_bytNbrHANPrices = 0;
            m_bytActTbl2136RecLen = 0;
            m_bytActTbl2136RecCnt = 0;
            m_bytNbrTierLabels = 0;
            m_bytTierLabelLen = 0;
            m_fltFWRev = fltFWRev;
        }

        /// <summary>
        /// Full read of table 2129
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/09/08 AF  1.50.33        Created
        //  04/21/09 AF  2.20.02 132587 The size of table 2129 changed starting with
        //                              SR 2.0 SP5
        //  09/17/09 AF  2.30.01        Removed the CE code. It was not needed
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2129.Read");
            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_bytNbrAMIDevices = m_Reader.ReadByte();
                m_bytNbrRegServices = m_Reader.ReadByte();
                m_usNbrDRLCMsgs = m_Reader.ReadUInt16();
                m_usMessagingMsgLen = m_Reader.ReadUInt16();
                m_usEncryptCertLen = m_Reader.ReadUInt16();
                m_bytNbrRsps = m_Reader.ReadByte();
                m_bytRateLabelLen = m_Reader.ReadByte();
                m_bytNbrHANPrices = m_Reader.ReadByte();

                if (VersionChecker.CompareTo(m_fltFWRev, CENTRON_AMI.VERSION_2_SP5) >= 0)
                {
                    m_bytActTbl2136RecLen = m_Reader.ReadByte();
                    m_bytActTbl2136RecCnt = m_Reader.ReadByte();
                    m_bytNbrTierLabels = m_Reader.ReadByte();
                    m_bytTierLabelLen = m_Reader.ReadByte();
                }

                if (VersionChecker.CompareTo(m_fltFWRev, CENTRON_AMI.VERSION_6_0_MICHIGAN) >= 0)
                {
                    m_bytActTbl2136ExtRecLen = m_Reader.ReadByte();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Actual number of AMI clients this server is capable of supporting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/08 AF  1.50.33        Created
        // 04/27/09 AF   2.20.02        Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public byte NumberAMIDevices
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading number of AMI devices"));
                    }
                }

                return m_bytNbrAMIDevices;
            }
        }

        /// <summary>
        /// Actual number of services each AMI HAN device is capable of registering for
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/08 AF  1.50.33        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public byte NumberRegServices
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading number of services each AMI HAN device is capable of registering for"));
                    }
                }

                return m_bytNbrRegServices;
            }
        }

        /// <summary>
        /// Gets the number of DRLC events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01        Created
        
        public ushort NumberDRLCEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_usNbrDRLCMsgs;
            }
        }

        /// <summary>
        /// Actual length, in octets, of encryption certificates
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/08 AF  1.50.33        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public ushort EncryptCertLen
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading length, in octets, of encryption certificates"));
                    }
                }

                return m_usEncryptCertLen;
            }
        }

        /// <summary>
        /// Actual number of responses the AMI_HAN_RSP_LOG_TBL (table 2131) is 
        /// capable of supporting per device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.35        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public byte NbrRsps
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading number of responses the AMI_HAN_RSP_LOG_TBL" +
                            " is capable of supporting per device"));
                    }
                }

                return m_bytNbrRsps;
            }
        }

        /// <summary>
        /// Actual number of prices the AMI_HAN_PRICE_TBL (table 2134) can
        /// store.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/09 jrf 2.20.03       Created
        //
        public byte NumberHANPrices
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrHANPrices;
            }
        }

        /// <summary>
        /// Actual number of tier labels in the AMI_HAN_PRICE_TBL (table 2134).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/09 jrf 2.20.03       Created
        //
        public byte NumberTierLabels
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNbrTierLabels;
            }
        }

        /// <summary>
        /// Actual size of the rate label in the AMI_HAN_PRICE_TBL (table 2134).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/09 jrf 2.20.03       Created
        //
        public byte RateLabelLength
        {
            get
            {
                ReadUnloadedTable();

                return m_bytRateLabelLen;
            }
        }

        /// <summary>
        /// Actual length of the tier labels in the AMI_HAN_PRICE_TBL (table 2134).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/09 jrf 2.20.03       Created
        //
        public byte TierLabelLength
        {
            get
            {
                ReadUnloadedTable();

                return m_bytTierLabelLen;
            }
        }

        /// <summary>
        /// Actual size of the Message Length in the AMI_HAN_MSG_TBL (table 2133).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/11 MSC 2.51.07       Created
        //
        public uint MessageLength
        {
            get
            {
                ReadUnloadedTable();

                return m_usMessagingMsgLen;
            }
        }

        /// <summary>
        /// The number of HAN devices actually joined to this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/21/09 AF  2.30.01        Created for CRF file needs
        //
        public byte NumberHANClientsJoined
        {
            get
            {
                ReadUnloadedTable();

                return m_bytActTbl2136RecCnt;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the length of table 2129 (ITRON 81) based on f/w version.
        /// The length increased by 4 bytes starting with SP5.
        /// </summary>
        /// <param name="fltFWRev">The version.revision of the register f/w
        /// in the meter.</param>
        /// <returns>The length of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/20/09 AF  2.20.02 132587 Created
        //
        static private uint GetTableLength(float fltFWRev)
        {
            uint uiTableLength;

            if (VersionChecker.CompareTo(fltFWRev, CENTRON_AMI.VERSION_6_0_MICHIGAN) >= 0)
            {
                uiTableLength = ACT_AMI_HAN_LIM_TBL_LENGTH;
            }
            else if (VersionChecker.CompareTo(fltFWRev, CENTRON_AMI.VERSION_2_SP5) >= 0)
            {
                uiTableLength = ACT_AMI_HAN_LIM_TBL_LENGTH_PRE_BERYLLIUM;
            }
            else
            {
                uiTableLength = ACT_AMI_HAN_LIM_TBL_LENGTH_PRE_SP5;
            }

            return uiTableLength;
        }

        #endregion

        #region Members

        private byte m_bytNbrAMIDevices;
        private byte m_bytNbrRegServices;
        private ushort m_usNbrDRLCMsgs;
        private ushort m_usMessagingMsgLen;
        private ushort m_usEncryptCertLen;
        private byte m_bytNbrRsps;
        private byte m_bytRateLabelLen;
        private byte m_bytNbrHANPrices;
        private byte m_bytActTbl2136RecLen;
        private byte m_bytActTbl2136RecCnt;
        private byte m_bytNbrTierLabels;
        private byte m_bytTierLabelLen;
        private byte m_bytActTbl2136ExtRecLen;
        private float m_fltFWRev;

        #endregion

    }

    /// <summary>
    /// HAN Mfg Table 2130 is a read only table that holds device registration
    /// information
    /// </summary>
    public class CHANMfgTable2130 : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_STIME_DATE = 4;
        private const int SIZE_OF_LTIME_DATE = 5;
        private const int TABLE_TIMEOUT = 100;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="fltFWRev">The firmware version and revision of the meter</param>
        /// <param name="table2129">HAN Mfg table 2129 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.35        Created
        //  08/05/09 AF  2.20.20        Added a fw ver param to deal with extra field
        //                              in this table for SR 2.0 SP 5 and above
        //
        public CHANMfgTable2130(CPSEM psem, float fltFWRev, CHANMfgTable2129 table2129)
            : base(psem, 2130, GetTableLength(table2129, fltFWRev), TABLE_TIMEOUT)
        {
            m_table2129 = table2129;
            m_aAMIHANDevRcds = new AMIHANDevRcd[m_table2129.NumberAMIDevices];
            m_fltFWRev = fltFWRev;
        }

        /// <summary>
        /// Full read of table 2130
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.35        Created
        //  06/24/08 AF  1.50.42        Changed the KeyType property to return an enum,
        //                              which required a cast here in the read
        //  04/20/09 AF  2.20.02 132587 Added read of field Table 2130 last change time
        //  08/05/09 AF  2.20.20        Only read the last change time for SR 2.0 SP 5 and above
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2130.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                for (int DeviceIndex = 0; DeviceIndex < m_table2129.NumberAMIDevices; DeviceIndex++)
                {
                    m_aAMIHANDevRcds[DeviceIndex] = new AMIHANDevRcd(m_table2129.NumberRegServices,
                        m_table2129.EncryptCertLen);
                    m_aAMIHANDevRcds[DeviceIndex].DeviceEUI = m_Reader.ReadUInt64();

                    m_aAMIHANDevRcds[DeviceIndex].LastStatusChange =
                        m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);

                    m_aAMIHANDevRcds[DeviceIndex].RegStatus = (AMIHANDevRcd.HANRegStatus)m_Reader.ReadByte();

                    m_aAMIHANDevRcds[DeviceIndex].RegServiceCount = m_Reader.ReadByte();

                    for (int RegSrvIndex = 0; RegSrvIndex < m_table2129.NumberRegServices; RegSrvIndex++)
                    {
                        m_aAMIHANDevRcds[DeviceIndex].RegServiceList[RegSrvIndex] = m_Reader.ReadUInt16();
                    }

                    m_aAMIHANDevRcds[DeviceIndex].EncryptKey.KeyType = (HANKeyRcd.HANKeyType)m_Reader.ReadByte();
                    m_aAMIHANDevRcds[DeviceIndex].EncryptKey.HANKey = m_Reader.ReadBytes(16);
                    m_aAMIHANDevRcds[DeviceIndex].EncryptCertLen = m_Reader.ReadUInt16();
                    m_aAMIHANDevRcds[DeviceIndex].EncryptCert = 
                        m_Reader.ReadBytes(m_table2129.EncryptCertLen);
                }

                if (VersionChecker.CompareTo(m_fltFWRev, CENTRON_AMI.VERSION_2_SP5) >= 0)
                {
                    m_dtLastChangeTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This is essentially a dump of the entire Mfg Table 2130.  Table 2130
        /// contains the device registration information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.35        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public AMIHANDevRcd[] AMIHANDevRcds
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading AMI HAN Registration Table"));
                    }
                }

                return m_aAMIHANDevRcds;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the length of table 2130
        /// </summary>
        /// <param name="table2129"></param>
        /// <param name="fltFWRev">The firmware version and revision in the meter</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/20/09 AF  2.20.02 132587 Added length of the time of last change of table 2130
        //  08/05/09 AF  2.20.20        Only add the last change time if we have SP 5 or above
        //
        static private uint GetTableLength(CHANMfgTable2129 table2129, float fltFWRev)
        {
            uint uiAMIHANDevRcdLen = (uint)(sizeof(UInt64) + SIZE_OF_STIME_DATE + 
                2 * sizeof(byte) + table2129.NumberRegServices * sizeof(UInt16) +
                17 * sizeof(byte) + sizeof(UInt16) + table2129.EncryptCertLen * sizeof(byte));

            //Multiply the size of one record by the number of records in the table
            if (VersionChecker.CompareTo(fltFWRev, CENTRON_AMI.VERSION_2_SP5) >= 0)
            {
                return ((uiAMIHANDevRcdLen * table2129.NumberAMIDevices) + SIZE_OF_LTIME_DATE);
            }
            else
            {
                return (uiAMIHANDevRcdLen * table2129.NumberAMIDevices);
            }
        }

        #endregion

        #region Members

        private AMIHANDevRcd[] m_aAMIHANDevRcds;
        private CHANMfgTable2129 m_table2129;
        private DateTime m_dtLastChangeTime;
        private float m_fltFWRev;

        #endregion
    }

    /// <summary>
    /// AMI HAN table 2131 is a read only table that contains information on the
    /// link status and response history of devices in the AMI HAN Registration Table.
    /// </summary>
    public class CHANMfgTable2131 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="table2129">HAN Mfg table 2129 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.35        Created
        //
        public CHANMfgTable2131(CPSEM psem, CHANMfgTable2129 table2129)
            : base(psem, 2131, GetTableLength(table2129), TABLE_TIMEOUT)
        {
            m_aAMIHANRspLogRcds = new AMIHANRspLogRcd[table2129.NumberAMIDevices];
            m_table2129 = table2129;
        }

        /// <summary>
        /// Full read of table 2131
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.35        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2131.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                for (int iDeviceIndex = 0; iDeviceIndex < m_table2129.NumberAMIDevices; iDeviceIndex++)
                {
                    m_aAMIHANRspLogRcds[iDeviceIndex] = new AMIHANRspLogRcd(m_table2129.NbrRsps);
                    m_aAMIHANRspLogRcds[iDeviceIndex].DeviceEUI = m_Reader.ReadUInt64();
                    m_aAMIHANRspLogRcds[iDeviceIndex].LastMsgRSSI = m_Reader.ReadSByte();
                    m_aAMIHANRspLogRcds[iDeviceIndex].LastMsgLQI = m_Reader.ReadByte();
                    m_aAMIHANRspLogRcds[iDeviceIndex].AvgRSSI = m_Reader.ReadSByte();
                    m_aAMIHANRspLogRcds[iDeviceIndex].AvgLQI = m_Reader.ReadByte();
                    m_aAMIHANRspLogRcds[iDeviceIndex].NbrMsgsRx = m_Reader.ReadUInt16();
                    m_aAMIHANRspLogRcds[iDeviceIndex].NbrMsgsTx = m_Reader.ReadUInt16();
                    m_aAMIHANRspLogRcds[iDeviceIndex].NbrTxFailures = m_Reader.ReadUInt16();

                    for (int iRspIndex = 0; iRspIndex < m_table2129.NbrRsps; iRspIndex++)
                    {
                        m_aAMIHANRspLogRcds[iDeviceIndex].RxMsgSeqNbrs[iRspIndex] = m_Reader.ReadUInt32();
                    }
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This is essentially a dump of table 2131.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.35        Created
        //
        public AMIHANRspLogRcd[] AMIHANRspLogRcds
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading AMI HAN Response Log Table"));
                    }
                }

                return m_aAMIHANRspLogRcds;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the length of table 2131
        /// </summary>
        /// <param name="Table2129">The table 2129 object.</param>
        /// <returns>Length of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.35        Created
        //
        static private uint GetTableLength(CHANMfgTable2129 Table2129)
        {
            uint uiAMIHANRspLogEntryRcd = 0;

            // DEVICE_EUI
            uiAMIHANRspLogEntryRcd += 8;
            // LAST_MSG_RSSI
            uiAMIHANRspLogEntryRcd += 1;
            // LAST_MSG_LQI
            uiAMIHANRspLogEntryRcd += 1;
            // AVG_RSSI
            uiAMIHANRspLogEntryRcd += 1;
            // AVG_LQI
            uiAMIHANRspLogEntryRcd += 1;
            // NBR_MSGS_RX
            uiAMIHANRspLogEntryRcd += 2;
            // NBR_MSGS_TX
            uiAMIHANRspLogEntryRcd += 2;
            // NBR_TX_FAILURES
            uiAMIHANRspLogEntryRcd += 2;
            // RX_MSG_SEQ_NBRS
            uiAMIHANRspLogEntryRcd += (uint)(4 * Table2129.NbrRsps);


            return uiAMIHANRspLogEntryRcd * Table2129.NumberAMIDevices;
        }

        #endregion

        #region Members

        private AMIHANRspLogRcd[] m_aAMIHANRspLogRcds;
        private CHANMfgTable2129 m_table2129; 

        #endregion

    }

    /// <summary>
    /// HAN Table 2132 - Contains the DRLC events
    /// </summary>
    public class CHANMfgTable2132 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table0">The Table 0 object for the current device</param>
        /// <param name="table2099">The Table 2099 object for the current device</param>
        /// <param name="table2129">The Table 2129 object for the current device</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public CHANMfgTable2132(CPSEM psem, CTable00 table0, CHANMfgTable2099 table2099, CHANMfgTable2129 table2129)
            : base(psem, 2132, GetTableSize(table0, table2099, table2129), TABLE_TIMEOUT)
        {
            m_Table2099 = table2099;
            m_Table2129 = table2129;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The Result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public override PSEMResponse Read()
        {
            PSEMResponse Response = base.Read();

            if (Response == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Response;
        }

        /// <summary>
        /// Reads the table from the meter. This method is to be used for testing only
        /// </summary>
        /// <returns>The Result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/13 MP          N/A    Created

        public List<DRLCLogMessage> GetDRLCEventsWithExpired()
        {
            PSEMResponse Response = base.Read();

            if (Response == PSEMResponse.Ok)
            {
                m_byLogFlags = m_Reader.ReadByte();
                m_usNumValidEntries = m_Reader.ReadUInt16();
                m_usLastEntryElement = m_Reader.ReadUInt16();
                m_uiLastEntrySequenceNumber = m_Reader.ReadUInt32();

                m_DRLCMessages = new List<DRLCLogMessage>();

                for (int iIndex = 0; iIndex < m_Table2129.NumberDRLCEvents; iIndex++)
                {
                    DRLCLogMessage NewMessage = new DRLCLogMessage();

                    NewMessage.MessageTime = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                    NewMessage.SequenceNumber = m_Reader.ReadUInt32();
                    NewMessage.Type = m_Reader.ReadByte();
                    NewMessage.MessageLength = m_Reader.ReadUInt16();
                    NewMessage.TxData = m_Reader.ReadBytes(m_Table2099.TxDataSize);


                    m_DRLCMessages.Add(NewMessage);
                }
            }

            return m_DRLCMessages;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of valid entries in the log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public ushort NumberOfValidEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_usNumValidEntries;
            }
        }

        /// <summary>
        /// Gets the index of the Last DRLC entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public ushort LastEntryElement
        {
            get
            {
                ReadUnloadedTable();

                return m_usLastEntryElement;
            }
        }

        /// <summary>
        /// Gets the sequence number of the last entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public uint LastEntrySequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return m_uiLastEntrySequenceNumber;
            }
        }

        /// <summary>
        /// Gets the list of DRLC Messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public List<DRLCLogMessage> DRLCMessages
        {
            get
            {
                ReadUnloadedTable();

                return m_DRLCMessages;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="table0">The Table 0 object for the current device</param>
        /// <param name="table2099">The Table 2099 object for the current device</param>
        /// <param name="table2129">The Table 2129 object for the current device</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private static uint GetTableSize(CTable00 table0, CHANMfgTable2099 table2099, CHANMfgTable2129 table2129)
        {
            return 9 + table2129.NumberDRLCEvents * (table0.STIMESize + 7 + table2099.TxDataSize);
        }

        /// <summary>
        /// Parses the data from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created
        //  03/27/12 RCG 2.53.52 178405 Changing code to use the Sequence Number to determine if the message is valid

        private void ParseData()
        {
            m_byLogFlags = m_Reader.ReadByte();
            m_usNumValidEntries = m_Reader.ReadUInt16();
            m_usLastEntryElement = m_Reader.ReadUInt16();
            m_uiLastEntrySequenceNumber = m_Reader.ReadUInt32();

            m_DRLCMessages = new List<DRLCLogMessage>();

            for (int iIndex = 0; iIndex < m_Table2129.NumberDRLCEvents; iIndex++)
            {
                DRLCLogMessage NewMessage = new DRLCLogMessage();

                NewMessage.MessageTime = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                NewMessage.SequenceNumber = m_Reader.ReadUInt32();
                NewMessage.Type = m_Reader.ReadByte();
                NewMessage.MessageLength = m_Reader.ReadUInt16();
                NewMessage.TxData = m_Reader.ReadBytes(m_Table2099.TxDataSize);

                // A Sequence number of all F's means the value is not used.
                if (NewMessage.SequenceNumber != uint.MaxValue)
                {
                     m_DRLCMessages.Add(NewMessage);
                }
            }
        }

        #endregion

        #region Member Variables

        private CHANMfgTable2099 m_Table2099;
        private CHANMfgTable2129 m_Table2129;

        private byte m_byLogFlags;
        private ushort m_usNumValidEntries;
        private ushort m_usLastEntryElement;
        private uint m_uiLastEntrySequenceNumber;

        private List<DRLCLogMessage> m_DRLCMessages;
        #endregion
    }

    
    /// <summary>
    /// The CHANMfgTable2133 class handles the reading of the AMI HAN Messaging Table
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/09/11 MSC 2.51.09        Created
    //
    public class CHANMfgTable2133 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private static readonly DateTime REFERENCE_UTC_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2129">The Table 2129 object for the current device</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created
         public CHANMfgTable2133(CPSEM psem, CHANMfgTable2129 table2129)
            : base(psem, 2133, GetTableLength(table2129), TABLE_TIMEOUT)
        {
            m_table2129 = table2129;
            m_AMIHANMsgRcd = new AMIHANMsgRcd();
        }

         /// <summary>
         /// Parses the table data out of the stream and assigns it to member variables
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  06/13/11 MSC 2.51.10          Created.
         //
         public override PSEMResponse Read()
         {

             m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2133.Read");

             UInt32 StartTime;
             UInt16 Duration;

             PSEMResponse Result = base.Read();

             //Populate the member variables that represent the table
             if (PSEMResponse.Ok == Result)
             {
                 m_DataStream.Position = 0;
                 
                 m_AMIHANMsgRcd.MessageId = m_Reader.ReadUInt32();
                 m_AMIHANMsgRcd.MessageControl = m_Reader.ReadByte();
                 StartTime = m_Reader.ReadUInt32();
                 m_AMIHANMsgRcd.MessageStart = REFERENCE_UTC_TIME.AddSeconds(StartTime);
                 Duration = m_Reader.ReadUInt16();
                 m_AMIHANMsgRcd.Duration = new TimeSpan(0, Duration, 0);
                 m_AMIHANMsgRcd.MessageLength = m_Reader.ReadUInt16();
                 m_AMIHANMsgRcd.DisplayMessage = m_Reader.ReadString(m_AMIHANMsgRcd.MessageLength);

             }

             return Result;
         }
         /// <summary>
         /// Write table to Meter
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  06/13/11 MSC 2.51.10          Created.
         public override PSEMResponse Write()
         {
             m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "CHANMfgTable2133.Write");
             m_DataStream.Position = 0;

             TimeSpan StartTime;

             m_Writer.Write((UInt32)m_AMIHANMsgRcd.MessageId);
             m_Writer.Write((Byte)m_AMIHANMsgRcd.MessageControl);
             StartTime = m_AMIHANMsgRcd.MessageStart - REFERENCE_UTC_TIME;

             m_Writer.Write((UInt32)StartTime.TotalSeconds);
             m_Writer.Write((UInt16)m_AMIHANMsgRcd.Duration.TotalMinutes);
             m_Writer.Write((UInt16)m_AMIHANMsgRcd.MessageLength);

             m_Writer.Write(m_AMIHANMsgRcd.DisplayMessage, m_AMIHANMsgRcd.MessageLength);

             return base.Write();
         }

        #endregion

         #region Public Properties

         /// <summary>
         /// Get/Set Message Record
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  02/20/14 jrf 3.50.36 459402 Added property back.
         public AMIHANMsgRcd MessageRecord
         {
             get
             {
                 return m_AMIHANMsgRcd;
             }
             set
             {
                 m_AMIHANMsgRcd = value;
             }
         }


         #endregion

        
        
        #region Private Methods

         /// <summary>
         /// Length of table 2133
         /// </summary>
         /// <param name="Table2129">The table 2129 object.</param>
         /// <returns>Length of the table in bytes</returns>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  06/10/11 MSC 2.51.09        Created
         //
         static private uint GetTableLength(CHANMfgTable2129 Table2129)
         {
             uint uiTable2133Len = 0;
             uint uiMSGLEN = 13;

             uiTable2133Len = uiMSGLEN + Table2129.MessageLength; // Returns 13 + the constant display message size (100) 
             
             return uiTable2133Len;
         }


        #endregion

        #region Member Variables

        private CHANMfgTable2129 m_table2129; //Actual AMI HAN Limiting Table
        private AMIHANMsgRcd m_AMIHANMsgRcd;

        #endregion
    }

    /// <summary>
    /// AMI HAN table 2131 is a read only table that contains information on the
    /// firmware download status and progress of 3rd party devices
    /// </summary>
    public class CHANMfgTable2134 : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_LTIME_DATE = 5;
        private const int TABLE_TIMEOUT = 5000;
        /// <summary>
        /// UTC Reference date
        /// </summary>
        protected readonly DateTime REFERENCE_DATE = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc); 

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="table2129">HAN Mfg table 2129 object</param>
        /// <param name="includeExpirationDate">Whether or not the expiration date should be included</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/02/09 AF  2.20.00        Created
        //
        public CHANMfgTable2134(CPSEM psem, CHANMfgTable2129 table2129, bool includeExpirationDate)
            : this(psem, 2134, GetTableLength(table2129, includeExpirationDate), table2129)
        {
            m_IncludeExpirationDate = includeExpirationDate;
        }

        /// <summary>
        /// Reads the data from the meter
        /// </summary>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/?? ??? ?.??.??        Created
        
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2134.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Write table to Meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/29/11 WW  2.51.15          Created.
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
               "CHANMfgTable2134.Write");

            m_DataStream.Position = 0;

            WriteData();

            return base.Write();
        }

        /// <summary>
        /// Writes the pending table to the meter
        /// </summary>
        /// <param name="pendingRecord">The pending table header to use</param>
        /// <returns>The result of the write</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created
        
        public override PSEMResponse PendingTableWrite(PendingEventRecord pendingRecord)
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "CHANMfgTable2134.PendingTableWrite");

            m_DataStream.Position = 0;

            WriteData();

            return base.PendingTableWrite(pendingRecord);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The HAN price records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03 n/a    Created.
        //
        public AMIHANPriceEntryRcd[] Prices
        {
            get
            {
                ReadUnloadedTable();

                return m_aAMIHANPriceEntryRcds;
            }
            set
            {
                m_aAMIHANPriceEntryRcds = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// The tier label records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03 n/a    Created.
        //
        public AMITierLabelEntryRcd[] Tiers
        {
            get
            {
                ReadUnloadedTable();

                return m_aAMITierLabelEntryRcds;
            }
            set
            {
                m_aAMITierLabelEntryRcds = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Expiration Date for the price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created

        public DateTime? ExpirationDate
        {
            get
            {
                ReadUnloadedTable();

                return m_ExpirationDate;
            }
            set
            {
                m_ExpirationDate = value;
            }
        }
        

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="tableID">The ID of the table</param>
        /// <param name="size">The size of the table</param>
        /// <param name="table2129">HAN Mfg table 2129 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.00        Created

        protected CHANMfgTable2134(CPSEM psem, ushort tableID, uint size, CHANMfgTable2129 table2129)
            : base(psem, tableID, size, TABLE_TIMEOUT)
        {
            m_table2129 = table2129;
            m_aAMIHANPriceEntryRcds = new AMIHANPriceEntryRcd[table2129.NumberHANPrices];
            m_aAMITierLabelEntryRcds = new AMITierLabelEntryRcd[table2129.NumberTierLabels];
            m_ExpirationDate = null;
        }

        /// <summary>
        /// Calculates the length of table 2134
        /// </summary>
        /// <param name="Table2129">The table 2129 object.</param>
        /// <param name="includeExpirationDate">Whether or not to include the expiration date</param>
        /// <returns>Length of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03 n/a    Created.
        //
        protected static uint GetTableLength(CHANMfgTable2129 Table2129, bool includeExpirationDate)
        {
            uint uiTable2134Len = 0;
            uint uiHANPriceEntryRcdLen = 30;  //static record size
            uint uiTierLabelEntryRcdLen = 2; //static record size

            //Add in the variable size to each record
            uiHANPriceEntryRcdLen += Table2129.RateLabelLength;
            uiTierLabelEntryRcdLen += Table2129.TierLabelLength;

            // Use the number of each record type to compute the table's size
            uiTable2134Len = Table2129.NumberHANPrices * uiHANPriceEntryRcdLen;
            uiTable2134Len += Table2129.NumberTierLabels * uiTierLabelEntryRcdLen;

            if (includeExpirationDate)
            {
                // Starting with Register FW 3.012 an expiration date is included in the table
                uiTable2134Len += 4;
            }

            return uiTable2134Len;
        }

        /// <summary>
        /// Parses the table data out of the stream and assigns it to member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03 n/a    Created.
        //
        protected virtual void ParseData()
        {
            int iRateLabelLen = m_table2129.RateLabelLength;
            int iTierLabelLen = m_table2129.TierLabelLength;

            for (int iHANPriceIndex = 0; iHANPriceIndex < m_table2129.NumberHANPrices; iHANPriceIndex++)
            {
                m_aAMIHANPriceEntryRcds[iHANPriceIndex] = new AMIHANPriceEntryRcd(); 
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].ProviderID = m_Reader.ReadUInt32();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].RateLabelLength = m_Reader.ReadByte();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].RateLabel = m_Reader.ReadString(iRateLabelLen);
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].IssuerEventID = m_Reader.ReadUInt32();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].UnitOfMeasureByte = m_Reader.ReadByte();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].Currency = m_Reader.ReadUInt16();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].PriceTrailandTier = m_Reader.ReadByte();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].NumberTiersAndRegisterTier = m_Reader.ReadByte();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].StartTime = m_Reader.ReadUInt32();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].Duration = m_Reader.ReadUInt16();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].Price = m_Reader.ReadUInt32();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].PriceRatio = m_Reader.ReadByte();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].GenerationPrice = m_Reader.ReadUInt32();
                m_aAMIHANPriceEntryRcds[iHANPriceIndex].GenerationPriceRatio = m_Reader.ReadByte();
            }

            for (int iTierLabelIndex = 0; iTierLabelIndex < m_table2129.NumberTierLabels; iTierLabelIndex++)
            {
                m_aAMITierLabelEntryRcds[iTierLabelIndex] = new AMITierLabelEntryRcd();
                m_aAMITierLabelEntryRcds[iTierLabelIndex].TierID = m_Reader.ReadByte();
                m_aAMITierLabelEntryRcds[iTierLabelIndex].TierLabelLength = m_Reader.ReadByte();
                m_aAMITierLabelEntryRcds[iTierLabelIndex].TierLabel = m_Reader.ReadString(iTierLabelLen);                
            }

            if (m_IncludeExpirationDate)
            {
                uint Seconds = m_Reader.ReadUInt32();

                if (Seconds > 0)
                {
                    m_ExpirationDate = REFERENCE_DATE.AddSeconds(Seconds);
                }
                else
                {
                    // We aren't using the Expiration Date
                    m_ExpirationDate = null;
                }
            }
        }

        /// <summary>
        /// Writes the current data to the stream
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created

        protected virtual void WriteData()
        {
            int iRateLabelLen = m_table2129.RateLabelLength;
            int iTierLabelLen = m_table2129.TierLabelLength;

            for (int iHANPriceIndex = 0; iHANPriceIndex < m_table2129.NumberHANPrices; iHANPriceIndex++)
            {

                m_Writer.Write((UInt32)m_aAMIHANPriceEntryRcds[iHANPriceIndex].ProviderID);
                m_Writer.Write((Byte)m_aAMIHANPriceEntryRcds[iHANPriceIndex].RateLabelLength);
                m_Writer.Write(m_aAMIHANPriceEntryRcds[iHANPriceIndex].RateLabel, iRateLabelLen);
                m_Writer.Write((UInt32)m_aAMIHANPriceEntryRcds[iHANPriceIndex].IssuerEventID);
                m_Writer.Write((Byte)m_aAMIHANPriceEntryRcds[iHANPriceIndex].UnitOfMeasureByte);
                m_Writer.Write((UInt16)m_aAMIHANPriceEntryRcds[iHANPriceIndex].Currency);
                m_Writer.Write((Byte)m_aAMIHANPriceEntryRcds[iHANPriceIndex].PriceTrailandTier);
                m_Writer.Write((Byte)m_aAMIHANPriceEntryRcds[iHANPriceIndex].NumberTiersAndRegisterTier);
                m_Writer.Write((UInt32)m_aAMIHANPriceEntryRcds[iHANPriceIndex].StartTime);
                m_Writer.Write((UInt16)m_aAMIHANPriceEntryRcds[iHANPriceIndex].Duration);
                m_Writer.Write((UInt32)m_aAMIHANPriceEntryRcds[iHANPriceIndex].Price);
                m_Writer.Write((Byte)m_aAMIHANPriceEntryRcds[iHANPriceIndex].PriceRatio);
                m_Writer.Write((UInt32)m_aAMIHANPriceEntryRcds[iHANPriceIndex].GenerationPrice);
                m_Writer.Write((Byte)m_aAMIHANPriceEntryRcds[iHANPriceIndex].GenerationPriceRatio);

            }

            for (int iTierLabelIndex = 0; iTierLabelIndex < m_table2129.NumberTierLabels; iTierLabelIndex++)
            {
                m_Writer.Write((Byte)m_aAMITierLabelEntryRcds[iTierLabelIndex].TierID);
                m_Writer.Write((Byte)m_aAMITierLabelEntryRcds[iTierLabelIndex].TierLabelLength);
                m_Writer.Write(m_aAMITierLabelEntryRcds[iTierLabelIndex].TierLabel, iTierLabelLen);
            }

            if (m_IncludeExpirationDate)
            {
                uint Seconds = 0;

                if (m_ExpirationDate != null)
                {
                    Seconds = (uint)(m_ExpirationDate.Value - REFERENCE_DATE).TotalSeconds;
                }

                m_Writer.Write(Seconds);
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Table 2129 object
        /// </summary>
        protected CHANMfgTable2129 m_table2129;
        /// <summary>
        /// Price entry records
        /// </summary>
        protected AMIHANPriceEntryRcd[] m_aAMIHANPriceEntryRcds;
        /// <summary>
        /// Tier entry records
        /// </summary>
        protected AMITierLabelEntryRcd[] m_aAMITierLabelEntryRcds;
        /// <summary>
        /// Whether or not to include the expiration date
        /// </summary>
        protected bool m_IncludeExpirationDate;
        /// <summary>
        /// The expiration date of a recurring schedule
        /// </summary>
        protected DateTime? m_ExpirationDate;        

        #endregion
    }

    /// <summary>
    /// AMI HAN table 2131 is a read only table that contains information on the
    /// firmware download status and progress of 3rd party devices
    /// </summary>
    public class CHANMfgTable2135 : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_LTIME_DATE = 5;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="table2129">HAN Mfg table 2129 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/02/09 AF  2.20.00        Created
        //
        public CHANMfgTable2135(CPSEM psem, CHANMfgTable2129 table2129)
            : base(psem, 2135, GetTableLength(table2129), TABLE_TIMEOUT)
        {
            m_table2129 = table2129;
            m_ImageVer = 0;
            m_ImageRev = 0;
            m_ImageBld = 0;
            m_TotalPagesActive = 0;
            m_LastPageSentActive = 0;
            m_RecordCount = 0;
            m_LastChangeTime = new DateTime(1970, 1, 1, 0, 0, 0);
            m_HanFwDlStatuses = new CENTRON_AMI.HAN_FW_DL_STATUS[table2129.NumberAMIDevices];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2135.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The version and revision of the HAN device firmware file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/09 AF  2.20.00        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public string TransferableImageVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    //Read the HAN Firmware Download (2135) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2135.  " +
                                        "F/W build is " + m_ImageBld.ToString("d3", CultureInfo.CurrentCulture) + ".";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_ImageVer.ToString(CultureInfo.CurrentCulture) + "." + m_ImageRev.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// The total number of pages in the currently active download
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/09 AF  2.20.00        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public ushort TotalPagesActiveTransfer
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read the HAN Firmware Download (2135) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2135.  " +
                                        "F/W build is " + m_ImageBld.ToString("d3", CultureInfo.CurrentCulture) + ".";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                return m_TotalPagesActive;
            }
        }

        /// <summary>
        /// The page number of the last page sent to the HAN device in the 
        /// firmware download
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/09 AF  2.20.00        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public ushort LastPageSentActiveTransfer
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read the HAN Firmware Download (2135) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2135.  " +
                                        "F/W build is " + m_ImageBld.ToString("d3", CultureInfo.CurrentCulture) + ".";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                return m_LastPageSentActive;
            }
        }

        /// <summary>
        /// The number of HAN devices in table 2130
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/09 AF  2.20.00        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public byte NumberHANDevices
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read the HAN Firmware Download (2135) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2135.  " +
                                        "F/W build is " + m_ImageBld.ToString("d3", CultureInfo.CurrentCulture) + ".";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                return m_RecordCount;
            }
        }

        /// <summary>
        /// Time of the last change in Table 2130.  The change could be a device
        /// added, removed, or registration status changed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/09 AF  2.20.00        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public DateTime LastChangeTime
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read the HAN Firmware Download (2135) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2135.  " +
                                        "F/W build is " + m_ImageBld.ToString("d3", CultureInfo.CurrentCulture) + ".";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                return m_LastChangeTime;
            }
        }

        /// <summary>
        /// Array of statuses of the HAN devices attached to this meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/09 AF  2.20.00        Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public CENTRON_AMI.HAN_FW_DL_STATUS[] HANFwDlStatuses
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read the HAN Firmware Download (2135) table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2135.  " +
                                        "F/W build is " + m_ImageBld.ToString("d3", CultureInfo.CurrentCulture) + ".";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                return m_HanFwDlStatuses;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the length of table 2135
        /// </summary>
        /// <param name="Table2129">The table 2129 object.</param>
        /// <returns>Length of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/02/09 AF  2.20.00        Created
        //
        static private uint GetTableLength(CHANMfgTable2129 Table2129)
        {
            uint uiHANFwDlTblRcd = 0;

            // Transferable image version
            uiHANFwDlTblRcd += 1;
            // Transferable image revision
            uiHANFwDlTblRcd += 1;
            // Transferable image build
            uiHANFwDlTblRcd += 1;
            // Pad byte
            uiHANFwDlTblRcd += 1;
            // Active transfer total pages
            uiHANFwDlTblRcd += 2;
            // Active Transfer last page sent
            uiHANFwDlTblRcd += 2;
            // Record count (number of HAN devices in Table 2130
            uiHANFwDlTblRcd += 1;
            // Time of last change in Table 2130 (LTIME)
            uiHANFwDlTblRcd += SIZE_OF_LTIME_DATE;
            // HAN firmware download entries - each status is one byte
            uiHANFwDlTblRcd += Table2129.NumberAMIDevices;

            return uiHANFwDlTblRcd;
        }

        /// <summary>
        /// Parses the table data out of the stream and assigns it to member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/02/09 AF  2.20.00        Created
        //
        private void ParseData()
        {
            m_ImageVer = m_Reader.ReadByte();
            m_ImageRev = m_Reader.ReadByte();
            m_ImageBld = m_Reader.ReadByte();
            byte byPadByte = m_Reader.ReadByte();
            m_TotalPagesActive = m_Reader.ReadUInt16();
            m_LastPageSentActive = m_Reader.ReadUInt16();
            m_RecordCount = m_Reader.ReadByte();
            m_LastChangeTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            for (int index = 0; index < m_table2129.NumberAMIDevices; index++)
            {
                m_HanFwDlStatuses[index] = (CENTRON_AMI.HAN_FW_DL_STATUS)m_Reader.ReadByte();
            }
        }

        #endregion

        #region Members

        private CHANMfgTable2129 m_table2129;
        private byte m_ImageVer;
        private byte m_ImageRev;
        private byte m_ImageBld;
        private UInt16 m_TotalPagesActive;
        private UInt16 m_LastPageSentActive;
        private byte m_RecordCount;
        private DateTime m_LastChangeTime;
        private CENTRON_AMI.HAN_FW_DL_STATUS[] m_HanFwDlStatuses;

        #endregion
    }

    /// <summary>
    /// HAN Mfg Table 2137 is a read only table that holds HAN device manufacturer info
    /// information
    /// </summary>
    public class CHANMfgTable2137 : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_ENTRY_LEN = 1;
        private const int SIZE_OF_NBR_ENTRIES = 1;
        private const int SIZE_OF_INDEX_IN_TABLE_2130 = 1;
        private const int SIZE_OF_DEVICE_EUI = 8;
        private const int SIZE_OF_MFG_NAME = 32;
        private const int SIZE_OF_MODEL_ID = 32;
        private const int SIZE_OF_DATE_CODE = 16;
        private const int SIZE_OF_LAST_UPDATE = 1;
        private const int SIZE_OF_LAST_REQUEST = 1;
        private const int TABLE_TIMEOUT = 100;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="table2129">HAN Mfg table 2129 object</param>
        /// <param name="STIMEsize">STIME size from Table 0</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/12/12  PGH 2.70.16        Created
        //
        public CHANMfgTable2137(CPSEM psem, CHANMfgTable2129 table2129, uint STIMEsize)
            : base(psem, 2137, GetTableLength(table2129, STIMEsize), TABLE_TIMEOUT)
        {
            m_table2129 = table2129;
            m_aAMIHANMfgInfoRcds = new AMIHANMfgInfoRcd[m_table2129.NumberHANClientsJoined];
        }

        /// <summary>
        /// Full read of table 2137
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/12/12 PGH 2.70.16        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2137.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                byte index_in_table_2130 = m_Reader.ReadByte();
                byte nbr_entries = m_Reader.ReadByte();

                for (int DeviceIndex = 0; DeviceIndex < m_table2129.NumberHANClientsJoined; DeviceIndex++)
                {
                    m_aAMIHANMfgInfoRcds[DeviceIndex] = new AMIHANMfgInfoRcd();

                    m_aAMIHANMfgInfoRcds[DeviceIndex].IndexInTable2130 = m_Reader.ReadByte();
                    m_aAMIHANMfgInfoRcds[DeviceIndex].DeviceEUI = m_Reader.ReadUInt64();
                    m_aAMIHANMfgInfoRcds[DeviceIndex].ManufacturerName = m_Reader.ReadString(SIZE_OF_MFG_NAME);
                    m_aAMIHANMfgInfoRcds[DeviceIndex].ModelIdentifier = m_Reader.ReadString(SIZE_OF_MODEL_ID);
                    m_aAMIHANMfgInfoRcds[DeviceIndex].DateCode = m_Reader.ReadString(SIZE_OF_DATE_CODE);
                    m_aAMIHANMfgInfoRcds[DeviceIndex].LastUpdate = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                    m_aAMIHANMfgInfoRcds[DeviceIndex].LastRequest = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This is essentially a dump of the entire Mfg Table 2137.  Table 2137
        /// contains the HAN device manufacturer information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/12/12 PGH 2.70.16        Created
        //
        public AMIHANMfgInfoRcd[] AMIHANMfgInfoRcds
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading AMI HAN Manufacturer Information Table"));
                    }
                }

                return m_aAMIHANMfgInfoRcds;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the length of table 2137
        /// </summary>
        /// <param name="table2129">Mfg Table 2129</param>
        /// <param name="STIMEsize">STIME size from Table 0</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/12/12 PGH 2.70.16        Created
        //
        private static uint GetTableLength(CHANMfgTable2129 table2129, uint STIMEsize)
        {
            uint uiAMIHANMfgInfoRcdLen = SIZE_OF_INDEX_IN_TABLE_2130 + SIZE_OF_DEVICE_EUI + SIZE_OF_MFG_NAME +
                SIZE_OF_MODEL_ID + SIZE_OF_DATE_CODE + (SIZE_OF_LAST_UPDATE * STIMEsize) + (SIZE_OF_LAST_REQUEST * STIMEsize);

            uint uiAMIHANMfgInfoTableLen = (uint)(SIZE_OF_ENTRY_LEN + SIZE_OF_NBR_ENTRIES +
               (table2129.NumberHANClientsJoined * uiAMIHANMfgInfoRcdLen));

            return (uiAMIHANMfgInfoTableLen);
        }

        #endregion

        #region Members

        private AMIHANMfgInfoRcd[] m_aAMIHANMfgInfoRcds;
        private CHANMfgTable2129 m_table2129;

        #endregion
    }

    /// <summary>
    /// HAN Mfg Table 2244 is a read only table that holds HAN Diagnostic Status Read
    /// information
    /// </summary>
    public class CHANMfgTable2244 : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_RESET_STATE = 1;
        private const int SIZE_OF_RESET_COUNT = 2;
        private const int SIZE_OF_RESET_LIMIT = 2;
        private const int SIZE_OF_LAST_RESET_TYPE = 1;
        private const int SIZE_OF_LAST_RESET_TIME = 1;
       
        private const int SIZE_OF_JOIN_IS_ENABLED = 1;
        private const int SIZE_OF_EXTENDED_DURATION = 2;
       
        private const int SIZE_OF_IS_RADIO_ENABLED = 1;
        private const int SIZE_OF_IS_QUIETMODE_ENABLED = 1;

        private const int TABLE_TIMEOUT = 100;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="STIMEsize">STIME size from Table 0</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12  PGH 2.70.16        Created
        //
        public CHANMfgTable2244(CPSEM psem, uint STIMEsize)
            : base(psem, 2244, GetTableLength(STIMEsize), TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Full read of table 2244
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16        Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2244.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_aAMIHANResetStatusRcd = new AMIHANResetStatusRcd();
                m_aAMIHANResetStatusRcd.ResetState = (AMIHANResetStatusRcd.HANResetState)m_Reader.ReadByte();
                m_aAMIHANResetStatusRcd.ResetCount = m_Reader.ReadUInt16();
                m_aAMIHANResetStatusRcd.ResetLimit = m_Reader.ReadUInt16();
                m_aAMIHANResetStatusRcd.LastResetType = (AMIHANResetStatusRcd.ZigBeeResetType)m_Reader.ReadByte();
                m_aAMIHANResetStatusRcd.LastResetTime = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);

                m_aAMIHANJoinStatusRcd = new AMIHANJoinStatusRcd();
                m_aAMIHANJoinStatusRcd.JoinIsEnabled = m_Reader.ReadBoolean();
                m_aAMIHANJoinStatusRcd.ExtendedDuration = m_Reader.ReadUInt16();

                m_aAMIHANRadioStatusRcd = new AMIHANRadioStatusRcd();
                m_aAMIHANRadioStatusRcd.IsRadioEnabled = m_Reader.ReadBoolean();
                m_aAMIHANRadioStatusRcd.IsQuietModeEnabled = m_Reader.ReadBoolean();

                m_aAMIHANDiagnosticReadRcd = new AMIHANDiagnosticReadRcd();
                m_aAMIHANDiagnosticReadRcd.ResetStatus = m_aAMIHANResetStatusRcd;
                m_aAMIHANDiagnosticReadRcd.JoinStatus = m_aAMIHANJoinStatusRcd;
                m_aAMIHANDiagnosticReadRcd.RadioStatus = m_aAMIHANRadioStatusRcd;

            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This is essentially a dump of the entire Mfg Table 2244.  Table 2244
        /// contains the HAN Diagnostic Status Read Information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16        Created
        //
        public AMIHANDiagnosticReadRcd AMIHANDiagnosticReadRecord
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading AMI HAN Diagnostic Status Read Information Table"));
                    }
                }

                return m_aAMIHANDiagnosticReadRcd;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the length of table 2244
        /// </summary>
        /// <param name="STIMEsize">STIME size from Table 0</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/12/12 PGH 2.70.16        Created
        //
        private static uint GetTableLength(uint STIMEsize)
        {
            uint uiAMIHANResetStatusRcdLen = SIZE_OF_RESET_STATE + SIZE_OF_RESET_COUNT + SIZE_OF_RESET_LIMIT +
                SIZE_OF_LAST_RESET_TYPE + (SIZE_OF_LAST_RESET_TIME * STIMEsize);

            uint uiAMIHANJoinStatusRcdLen = SIZE_OF_JOIN_IS_ENABLED + SIZE_OF_EXTENDED_DURATION;

            uint uiAMIHANRadioStatusRcdLen = SIZE_OF_IS_RADIO_ENABLED + SIZE_OF_IS_QUIETMODE_ENABLED;

            uint uiAMIHANDiagnosticReadRcdLen = uiAMIHANResetStatusRcdLen + uiAMIHANJoinStatusRcdLen + uiAMIHANRadioStatusRcdLen;

            return (uiAMIHANDiagnosticReadRcdLen);
        }

        #endregion

        #region Members

        AMIHANResetStatusRcd m_aAMIHANResetStatusRcd;
        AMIHANJoinStatusRcd m_aAMIHANJoinStatusRcd;
        AMIHANRadioStatusRcd m_aAMIHANRadioStatusRcd;
        AMIHANDiagnosticReadRcd m_aAMIHANDiagnosticReadRcd;

        #endregion
    }

    /// <summary>
    /// Table 2288 - HAN Stats Two Table
    /// </summary>
    public class OpenWayMFGTable2288 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 30;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public OpenWayMFGTable2288(CPSEM psem)
            : base(psem, 2288, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_UnicastMessagesSent = 0;
            m_BroadcastMessagesSent = 0;
            m_ViaBindingMessagesSent = 0;
            m_PermitJoinRequestMessagesSent = 0;
            m_IncomingMessagesReceived = 0;
            m_LeaveRequestMessagesSent = 0;
            m_IsQuietModeActive = false;
            m_DroppedResponseMessages = 0;
            m_StackResets = 0;
        }

        /// <summary>
        /// Reads the table from the meter
        /// </summary>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2288.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        private void ParseData()
        {
            m_UnicastMessagesSent = m_Reader.ReadUInt32();
            m_InterPANMessagesSent = m_Reader.ReadUInt32();
            m_BroadcastMessagesSent = m_Reader.ReadUInt32();
            m_ViaBindingMessagesSent = m_Reader.ReadUInt32();
            m_PermitJoinRequestMessagesSent = m_Reader.ReadUInt32();
            m_IncomingMessagesReceived = m_Reader.ReadUInt32();
            m_LeaveRequestMessagesSent = m_Reader.ReadByte();
            m_IsQuietModeActive = m_Reader.ReadBoolean();
            m_DroppedResponseMessages = m_Reader.ReadUInt16();
            m_StackResets = m_Reader.ReadUInt16();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of Unicast messages sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public uint UnicastMessagesSent
        {
            get
            {
                ReadUnloadedTable();

                return m_UnicastMessagesSent;
            }
        }

        /// <summary>
        /// Gets the number of InterPAN messages sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public uint InterPANMessagesSent
        {
            get
            {
                ReadUnloadedTable();

                return m_InterPANMessagesSent;
            }
        }

        /// <summary>
        /// Gets the number of broadcast messages sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public uint BroadcastMessagesSent
        {
            get
            {
                ReadUnloadedTable();

                return m_BroadcastMessagesSent;
            }
        }

        /// <summary>
        /// Gets the number of messages sent via binding
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public uint ViaBindingMessagesSent
        {
            get
            {
                ReadUnloadedTable();

                return m_ViaBindingMessagesSent;
            }
        }

        /// <summary>
        /// Gets the number of Permit Join Request messages sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public uint PermitJoinRequestMessagesSent
        {
            get
            {
                ReadUnloadedTable();

                return m_PermitJoinRequestMessagesSent;
            }
        }

        /// <summary>
        /// Gets the number of incoming messages that have been received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public uint IncomingMessagesReceived
        {
            get
            {
                ReadUnloadedTable();

                return m_IncomingMessagesReceived;
            }
        }

        /// <summary>
        /// Gets the number of Leave Request Message sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public byte LeaveRequestMessagesSent
        {
            get
            {
                ReadUnloadedTable();

                return m_LeaveRequestMessagesSent;
            }
        }

        /// <summary>
        /// Gets whether or not quiet mode is currently active
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public bool IsQuietModeActive
        {
            get
            {
                ReadUnloadedTable();

                return m_IsQuietModeActive;
            }
        }

        /// <summary>
        /// Gets the number of response messages that have been dropped
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public ushort DroppedResponseMessages
        {
            get
            {
                ReadUnloadedTable();

                return m_DroppedResponseMessages;
            }
        }

        /// <summary>
        /// Gets the number of stack resets that have occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/06/12 RCG 2.60.29 TRQ6065 Created
        
        public ushort StackResets
        {
            get
            {
                ReadUnloadedTable();

                return m_StackResets;
            }
        }

        #endregion

        #region Member Variables

        private uint m_UnicastMessagesSent;
        private uint m_InterPANMessagesSent;
        private uint m_BroadcastMessagesSent;
        private uint m_ViaBindingMessagesSent;
        private uint m_PermitJoinRequestMessagesSent;
        private uint m_IncomingMessagesReceived;
        private byte m_LeaveRequestMessagesSent;
        private bool m_IsQuietModeActive;
        private ushort m_DroppedResponseMessages;
        private ushort m_StackResets;

        #endregion
    }

    // Table 2291 - HAN Event CE Log Table present under OpenWayHANEventTables.cs

    /// <summary>
    /// HAN Recurring Price Schedule table
    /// </summary>
    public class OpenWayMFGTable2297 : CHANMfgTable2134
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM </param>
        /// <param name="table2129">The table 2129 object for the current device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.00        Created

        public OpenWayMFGTable2297(CPSEM psem, CHANMfgTable2129 table2129)
            : base(psem, 2297, GetTableLength(table2129, true), table2129)
        {
            // This needs to be set to false so that the recurring price does not get read at the wrong time.
            m_IncludeExpirationDate = false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.00        Created

        protected override void ParseData()
        {
            m_ExpirationDate = REFERENCE_DATE.AddSeconds(m_Reader.ReadUInt32());

            base.ParseData();
        }

        /// <summary>
        /// Writes the data to the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.00        Created

        protected override void WriteData()
        {
            if (m_ExpirationDate != null)
            {
                m_Writer.Write((uint)(m_ExpirationDate.Value - REFERENCE_DATE).TotalSeconds);
            }
            else
            {
                // Write a 0 value
                m_Writer.Write((uint)0);
            }

            base.WriteData();
        }

        #endregion
    }

    /// <summary>
    /// DRLC Message Data item used to summarize the message details
    /// </summary>
    public class DRLCMessageData
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemName">The name of the data item</param>
        /// <param name="itemValue">The value of the data item</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public DRLCMessageData(string itemName, string itemValue)
        {
            m_ItemName = itemName;
            m_ItemValue = itemValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the data item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public string Name
        {
            get
            {
                return m_ItemName;
            }
        }

        /// <summary>
        /// Gets the value of the data item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public string Value
        {
            get
            {
                return m_ItemValue;
            }
        }

        #endregion

        #region Member Variables

        private string m_ItemName;
        private string m_ItemValue;

        #endregion
    }

    /// <summary>
    /// Represents a DRLC Log Message
    /// </summary>
    public class DRLCLogMessage
    {
        #region Constants

        private const byte FRAME_TYPE_MASK = 0x07;
        private const byte FRAME_TYPE_ALL = 0x00;
        private const byte FRAME_TYPE_SPECIFIC = 0x01;

        private const byte MFG_CONTROL_MASK = 0x08;
        private const byte DIRECTION_MASK = 0x10;
        private const byte DEFAULT_RESPONSE_MASK = 0x20;

        #endregion

        #region Definitions

        /// <summary>
        /// DRLC Message Types
        /// </summary>
        public enum MessageType : byte
        {
            /// <summary>
            /// Unknown Message Type
            /// </summary>
            [EnumDescription("Unknown")]
            Unknown = 0x00,
            /// <summary>
            /// Passthrough Message
            /// </summary>
            [EnumDescription("Passthrough")]
            Passthrough = 0x06,
            /// <summary>
            /// Load Control Message
            /// </summary>
            [EnumDescription("Load Control")]
            SmartEnergyLoadControl = 0x10,
            /// <summary>
            /// Cancel Load Control Message
            /// </summary>
            [EnumDescription("Cancel Load Control")]
            SmartEnergyCancelLoadControl = 0x11,
            /// <summary>
            /// Cancel All Load Control Message
            /// </summary>
            [EnumDescription("Cancel All Load Control")]
            SmartEnergyCancelAllLoadControl = 0x12,
            /// <summary>
            /// Generic ZCL Message
            /// </summary>
            [EnumDescription("Generic")]
            GenericZCL = 0x20,
            /// <summary>
            /// ZigBee Write Attribute Message
            /// </summary>
            [EnumDescription("Write Attribute")]
            ZigBeeWriteAttribute = 0x21,
        }

        private enum DataType : byte
        {
            [EnumDescription("No Data")]
            NoData = 0x00,
            [EnumDescription("8-bit Data")]
            Data8 = 0x08,
            [EnumDescription("16-bit Data")]
            Data16 = 0x09,
            [EnumDescription("24-bit Data")]
            Data24 = 0x0A,
            [EnumDescription("32-bit Data")]
            Data32 = 0x0B,
            [EnumDescription("Bool")]
            Bool = 0x10,
            [EnumDescription("8-bit Bitmap")]
            Bitmap8 = 0x18,
            [EnumDescription("16-bit Bitmap")]
            Bitmap16 = 0x19,
            [EnumDescription("24-bit Bitmap")]
            Bitmap24 = 0x1A,
            [EnumDescription("32-bit Bitmap")]
            Bitmap32 = 0x1B,
            [EnumDescription("UInt8")]
            UInt8 = 0x20,
            [EnumDescription("UInt16")]
            UInt16 = 0x21,
            [EnumDescription("UInt24")]
            UInt24 = 0x22,
            [EnumDescription("UInt32")]
            UInt32 = 0x23,
            [EnumDescription("Int8")]
            Int8 = 0x28,
            [EnumDescription("Int16")]
            Int16 = 0x29,
            [EnumDescription("Int24")]
            Int24 = 0x2A,
            [EnumDescription("Int32")]
            Int32 = 0x2B,
            [EnumDescription("8-bit Enum")]
            Enum8 = 0x30,
            [EnumDescription("16-bit Enum")]
            Enum16 = 0x31,
            [EnumDescription("Float16")]
            Float16 = 0x38,
            [EnumDescription("Float32")]
            Float32 = 0x39,
            [EnumDescription("Float64")]
            Float64 = 0x3A,
            [EnumDescription("Octet String")]
            OctetString = 0x41,
            [EnumDescription("Character String")]
            CharString = 0x42,
            [EnumDescription("Time")]
            TimeOfDay = 0xE0,
            [EnumDescription("Date")]
            Date = 0xE1,
            [EnumDescription("Cluster ID")]
            ClusterID = 0xE8,
            [EnumDescription("Attribute ID")]
            AttributeID = 0xE9,
            [EnumDescription("BACnet OID")]
            BACnetOID = 0xEA,
            [EnumDescription("IEEE Address")]
            IEEEAddress = 0xF0,
            [EnumDescription("Unknown")]
            Unknown = 0xFF,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public DRLCLogMessage()
        {
            m_Time = DateTime.MinValue;
            m_uiSequenceNumber = 0;
            m_byType = 0;
            m_usMessageLength = 0;
            m_TxData = null;
            m_MessageData = null;
            m_EventID = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Message Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public DateTime MessageTime
        {
            get
            {
                return m_Time;
            }
            internal set
            {
                m_Time = value;
            }
        }

        /// <summary>
        /// Gets or sets the Sequence Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public uint SequenceNumber
        {
            get
            {
                return m_uiSequenceNumber;
            }
            internal set
            {
                m_uiSequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Message Type as a byte value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public byte Type
        {
            get
            {
                return m_byType;
            }
            internal set
            {
                m_byType = value;
            }
        }

        /// <summary>
        /// Gets the enumerated Message Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public MessageType EnumeratedType
        {
            get
            {
                MessageType EnumType = MessageType.Unknown;

                if (Enum.IsDefined(typeof(MessageType), m_byType))
                {
                    EnumType = (MessageType)m_byType;
                }

                return EnumType;
            }
        }

        /// <summary>
        /// Gets the description of the DRLC Message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public string Description
        {
            get
            {
                string strDescription = "Unknown " + m_byType.ToString(CultureInfo.CurrentCulture);

                if(EnumeratedType != MessageType.Unknown)
                {
                    strDescription = EnumDescriptionRetriever.RetrieveDescription(EnumeratedType);
                }

                return strDescription;
            }
        }

        /// <summary>
        /// Gets or sets the Message Length
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public ushort MessageLength
        {
            get
            {
                return m_usMessageLength;
            }
            internal set
            {
                m_usMessageLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the Message Data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public byte[] TxData
        {
            get
            {
                return m_TxData;
            }
            internal set
            {
                m_TxData = value;
                ParseTxData();
            }
        }

        /// <summary>
        /// Gets a list of the message data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        public List<DRLCMessageData> MessageData
        {
            get
            {
                return m_MessageData;
            }
        }

        /// <summary>
        /// Gets the ID of the event. This is only applicable to DRLC events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/16/11 RCG 2.53.08        Created
        
        public uint EventID
        {
            get
            {
                return m_EventID;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Tx Data into a readable form for display to the user
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private void ParseTxData()
        {
            MemoryStream TxDataStream = new MemoryStream(m_TxData);
            PSEMBinaryReader TxDataReader = new PSEMBinaryReader(TxDataStream);

            m_MessageData = new List<DRLCMessageData>();

            switch(EnumeratedType)
            {
                case MessageType.SmartEnergyLoadControl:
                {
                    m_EventID = TxDataReader.ReadUInt32();
                    ushort DeviceGroup = TxDataReader.ReadUInt16();
                    byte UtilityEnrollGroup = TxDataReader.ReadByte();
                    uint StartTimeSeconds = TxDataReader.ReadUInt32();
                    ushort Duration = TxDataReader.ReadUInt16();
                    byte CriticalityLevel = TxDataReader.ReadByte();
                    byte CoolTempOff = TxDataReader.ReadByte();
                    byte HeatTempOff = TxDataReader.ReadByte();
                    short CoolTempSetPt = TxDataReader.ReadInt16();
                    short HeatTempSetPt = TxDataReader.ReadInt16();
                    sbyte AvgLoadAdjust = TxDataReader.ReadSByte();
                    byte DutyCycle = TxDataReader.ReadByte();
                    byte EventControl = TxDataReader.ReadByte();

                    m_MessageData.Add(new DRLCMessageData("Issuer Event ID", m_EventID.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Device Groups", GetDeviceGroupNames((DRLCDeviceClasses)DeviceGroup)));
                    m_MessageData.Add(new DRLCMessageData("Utility Enrollment Group", UtilityEnrollGroup.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Start Time", FormatTime(StartTimeSeconds)));
                    m_MessageData.Add(new DRLCMessageData("Duration", Duration.ToString(CultureInfo.InvariantCulture) + " minutes"));
                    m_MessageData.Add(new DRLCMessageData("Criticality Level", EnumDescriptionRetriever.RetrieveDescription((HANCriticalityLevel)CriticalityLevel)));
                    m_MessageData.Add(new DRLCMessageData("Cooling Temperature Offset", FormatTempOffset(CoolTempOff)));
                    m_MessageData.Add(new DRLCMessageData("Heating Temperature Offset", FormatTempOffset(HeatTempOff)));
                    m_MessageData.Add(new DRLCMessageData("Cooling Temperature Set Point", FormatTempSetPoint(CoolTempSetPt)));
                    m_MessageData.Add(new DRLCMessageData("Heating Temperature Set Point", FormatTempSetPoint(HeatTempSetPt)));
                    m_MessageData.Add(new DRLCMessageData("Average Load Adjustment", FormatAvgLoadAdjust(AvgLoadAdjust)));
                    m_MessageData.Add(new DRLCMessageData("Duty Cycle", FormatDutyCycle(DutyCycle)));
                    m_MessageData.Add(new DRLCMessageData("Event Control", FormatEventControl(EventControl)));

                    break;
                }
                case MessageType.SmartEnergyCancelLoadControl:
                {
                    m_EventID = TxDataReader.ReadUInt32();
                    ushort DeviceGroup = TxDataReader.ReadUInt16();
                    byte UtilityEnrollGroup = TxDataReader.ReadByte();
                    byte CancelControl = TxDataReader.ReadByte();
                    uint EffectiveTimeSeconds = TxDataReader.ReadUInt32();

                    m_MessageData.Add(new DRLCMessageData("Issuer Event ID", m_EventID.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Device Groups", GetDeviceGroupNames((DRLCDeviceClasses)DeviceGroup)));
                    m_MessageData.Add(new DRLCMessageData("Utility Enrollment Group", UtilityEnrollGroup.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Cancel Control", FormatCancelControl(CancelControl)));
                    m_MessageData.Add(new DRLCMessageData("Effective Time", FormatTime(EffectiveTimeSeconds)));

                    break;
                }
                case MessageType.SmartEnergyCancelAllLoadControl:
                {
                    byte CancelControl = TxDataReader.ReadByte();

                    m_MessageData.Add(new DRLCMessageData("Cancel Control", FormatCancelControl(CancelControl)));

                    break;
                }
                case MessageType.GenericZCL:
                {
                    ushort ProfileNumber = TxDataReader.ReadUInt16();
                    ushort ClusterNumber = TxDataReader.ReadUInt16();
                    byte FrameControl = TxDataReader.ReadByte();
                    bool MfgSpecific = (byte)(FrameControl & MFG_CONTROL_MASK) == MFG_CONTROL_MASK;
                    bool DisableDefaultResponse = (byte)(FrameControl & DEFAULT_RESPONSE_MASK) == DEFAULT_RESPONSE_MASK;
                    ushort ManufacturerCode =  ManufacturerCode = TxDataReader.ReadUInt16();
                    byte CommandID = TxDataReader.ReadByte();
                    byte CommandLength = TxDataReader.ReadByte();
                    byte[] Command = TxDataReader.ReadBytes(CommandLength);

                    m_MessageData.Add(new DRLCMessageData("Profile Number", ProfileNumber.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Cluster Number", ClusterNumber.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Frame Type", FormatFrameType(FrameControl)));
                    m_MessageData.Add(new DRLCMessageData("Manufacturer Specific", FormatBool(MfgSpecific)));
                    m_MessageData.Add(new DRLCMessageData("Direction", FormatDirection(FrameControl)));
                    m_MessageData.Add(new DRLCMessageData("Disable Default Response", FormatBool(DisableDefaultResponse)));
                    m_MessageData.Add(new DRLCMessageData("Manufacturer Code", ManufacturerCode.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Command ID", CommandID.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Command", FormatBytes(Command)));

                    break;
                }
                case MessageType.ZigBeeWriteAttribute:
                {
                    ushort ProfileNumber = TxDataReader.ReadUInt16();
                    ushort ClusterNumber = TxDataReader.ReadUInt16();
                    ushort AttributeID = TxDataReader.ReadUInt16();
                    byte AttributeType = TxDataReader.ReadByte();
                    string AttributeValue = ReadAttributeValue((DataType)AttributeType, TxDataReader);

                    m_MessageData.Add(new DRLCMessageData("Profile Number", ProfileNumber.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Cluster Number", ClusterNumber.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Attribute ID", AttributeID.ToString(CultureInfo.InvariantCulture)));
                    m_MessageData.Add(new DRLCMessageData("Attribute Type", EnumDescriptionRetriever.RetrieveDescription((DataType)AttributeType)));
                    m_MessageData.Add(new DRLCMessageData("Attribute Value", AttributeValue));

                    break;
                }
                case MessageType.Passthrough:
                case MessageType.Unknown:
                default:
                {
                    byte[] Message = TxDataReader.ReadBytes(m_usMessageLength);

                    m_MessageData.Add(new DRLCMessageData("Tx Data", FormatBytes(Message)));
                    break;
                }
            }
        }

        /// <summary>
        /// Reads the Attribute Value and returns a readable string
        /// </summary>
        /// <param name="attributeType">The attribute data type</param>
        /// <param name="reader">The binary reader storing the data</param>
        /// <returns>The formatted value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created
        private string ReadAttributeValue(DataType attributeType, PSEMBinaryReader reader)
        {
            string FormattedValue = null;

            switch (attributeType)
            {
                case DataType.NoData:
                {
                    FormattedValue = "";
                    break;
                }
                case DataType.Data8:
                case DataType.Enum8:
                case DataType.Bitmap8:
                {
                    byte byValue = reader.ReadByte();
                    FormattedValue = byValue.ToString("X2", CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Data16:
                case DataType.Enum16:
                case DataType.Bitmap16:
                case DataType.ClusterID:
                case DataType.AttributeID:
                {
                    ushort usValue = reader.ReadUInt16();
                    FormattedValue = usValue.ToString("X4", CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Data24:
                case DataType.Bitmap24:
                {
                    long lValue = reader.ReadUInt24();
                    FormattedValue = lValue.ToString("X6", CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Data32:
                case DataType.Bitmap32:
                {
                    uint uiValue = reader.ReadUInt32();
                    FormattedValue = uiValue.ToString("X8", CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Bool:
                {
                    bool bValue = reader.ReadBoolean();
                    FormattedValue = bValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case DataType.UInt8:
                {
                    byte byValue = reader.ReadByte();
                    FormattedValue = byValue.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.UInt16:
                {
                    ushort usValue = reader.ReadUInt16();
                    FormattedValue = usValue.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.UInt24:
                {
                    long lValue = reader.ReadUInt24();
                    FormattedValue = lValue.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.UInt32:
                {
                    uint uiValue = reader.ReadUInt32();
                    FormattedValue = uiValue.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Int8:
                {
                    sbyte byValue = reader.ReadSByte();
                    FormattedValue = byValue.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Int16:
                {
                    short sValue = reader.ReadInt16();
                    FormattedValue = sValue.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Int24:
                {
                    int iValue = reader.ReadInt24();
                    FormattedValue = iValue.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Int32:
                {
                    int iValue = reader.ReadInt32();
                    FormattedValue = iValue.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Float16:
                {
                    FormattedValue = "Not Supported";
                    break;
                }
                case DataType.Float32:
                {
                    float fValue = reader.ReadSingle();
                    FormattedValue = fValue.ToString("F3", CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.Float64:
                {
                    double dValue = reader.ReadDouble();
                    FormattedValue = dValue.ToString("F3", CultureInfo.InvariantCulture);
                    break;
                }
                case DataType.OctetString:
                {
                    byte Length = reader.ReadByte();
                    byte[] Data = reader.ReadBytes(Length);
                    FormattedValue = FormatBytes(Data);
                    break;
                }
                case DataType.CharString:
                {
                    byte Length = reader.ReadByte();
                    FormattedValue = reader.ReadString(Length);
                    break;
                }
                case DataType.TimeOfDay:
                {
                    byte Hours = reader.ReadByte();
                    byte Minutes = reader.ReadByte();
                    byte Seconds = reader.ReadByte();
                    byte Hundredths = reader.ReadByte();

                    DateTime Time = new DateTime(1900, 1, 1, Hours, Minutes, Seconds, Hundredths * 10);
                    FormattedValue = Time.ToShortTimeString();
                    break;
                }
                case DataType.Date:
                {
                    byte Year = reader.ReadByte();
                    byte Month = reader.ReadByte();
                    byte Day = reader.ReadByte();

                    DateTime Date = new DateTime(1900 + Year, Month, Day);
                    FormattedValue = Date.ToShortDateString();
                    break;
                }
                case DataType.BACnetOID:
                {
                    byte[] Data = reader.ReadBytes(4);
                    FormattedValue = FormatBytes(Data);
                    break;
                }
                case DataType.IEEEAddress:
                {
                    byte[] Data = reader.ReadBytes(8);
                    FormattedValue = FormatBytes(Data);
                    break;
                }
                case DataType.Unknown:
                default:
                {
                    byte[] Data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    FormattedValue = FormatBytes(Data);
                    break;
                }
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats a boolean value
        /// </summary>
        /// <param name="booleanValue">The boolean value</param>
        /// <returns>Yes or No</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatBool(bool booleanValue)
        {
            string FormattedValue = null;

            if (booleanValue)
            {
                FormattedValue = "Yes";
            }
            else
            {
                FormattedValue = "No";
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats the Frame Control direction
        /// </summary>
        /// <param name="frameControl">The Frame Control bitfield</param>
        /// <returns>The direction as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatDirection(byte frameControl)
        {
            string FormattedValue = null;

            if ((byte)(frameControl & DIRECTION_MASK) == DIRECTION_MASK)
            {
                FormattedValue = "Server to Client";
            }
            else
            {
                FormattedValue = "Client to Server";
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats the Frame Control frame type
        /// </summary>
        /// <param name="frameControl">The Frame Control bitfield</param>
        /// <returns>The frame type as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatFrameType(byte frameControl)
        {
            string FormattedValue = null;
            byte FrameType = (byte)(frameControl & FRAME_TYPE_MASK);

            switch(FrameType)
            {
                case FRAME_TYPE_ALL:
                {
                    FormattedValue = "Entire Profile";
                    break;
                }
                case FRAME_TYPE_SPECIFIC:
                {
                    FormattedValue = "Specific Cluster";
                    break;
                }
                default:
                {
                    FormattedValue = "Invalid Value";
                    break;
                }
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats a byte array into a hex string
        /// </summary>
        /// <param name="data">The data to format</param>
        /// <returns>The hex string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatBytes(byte[] data)
        {
            string FormattedValue = "";

            for (int iIndex = 0; iIndex < data.Length; iIndex++)
            {
                FormattedValue += data[iIndex].ToString("X2", CultureInfo.InvariantCulture);
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats a uint time value to a readable format
        /// </summary>
        /// <param name="seconds">The number of seconds since 2000 GMT</param>
        /// <returns>The date as a formatted value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created
        //  08/23/13 AF  2.85.27 WR418534 Display the time in local time to be consistent with HAN event list
        //                              and other Field-Pro features.
        //
        private string FormatTime(uint seconds)
        {
            string FormattedValue = null;
            DateTime Time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);

            if (seconds == 0)
            {
                // Start right now
                FormattedValue = "Now";
            }
            else
            {
                FormattedValue = Time.ToLocalTime().ToString("G", CultureInfo.CurrentCulture);
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats the Cancel control
        /// </summary>
        /// <param name="cancelControl">The cancel event control value</param>
        /// <returns>The Cancel Control as a formatted string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatCancelControl(byte cancelControl)
        {
            string FormattedValue = null;

            if(cancelControl == 0)
            {
                FormattedValue = "Terminated Immediately";
            }
            else
            {
                FormattedValue = "Termination Time Randomized";
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats the eventControl data
        /// </summary>
        /// <param name="eventControl">Event Control Data</param>
        /// <returns>The event control data formatted</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatEventControl(byte eventControl)
        {
            string FormattedValue = null;

            switch ((DRLCEventControl)eventControl)
            {
                case DRLCEventControl.None:
                {
                    FormattedValue = "Start and End Times not randomized";
                    break;
                }
                case DRLCEventControl.RandomizeBoth:
                {
                    FormattedValue = "Start and End Times randomized";
                    break;
                }
                case DRLCEventControl.RandomizeStartTime:
                {
                    FormattedValue = "Start Time randomized";
                    break;
                }
                case DRLCEventControl.RandomizeEndTime:
                {
                    FormattedValue = "End Time randomized";
                    break;
                }
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats the Duty Cycle
        /// </summary>
        /// <param name="dutyCycle">The Duty Cycle value</param>
        /// <returns>The Duty Cycle as a formatted string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatDutyCycle(byte dutyCycle)
        {
            string FormattedValue = null;

            if (dutyCycle != 0xFF)
            {
                if (dutyCycle <= 100)
                {
                    FormattedValue = dutyCycle.ToString(CultureInfo.InvariantCulture) + "%";
                }
                else
                {
                    FormattedValue = "Invalid Value";
                }
            }
            else
            {
                FormattedValue = "Not Used";
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats the Average Load Adjustment value
        /// </summary>
        /// <param name="avgLoadAdjust">The Average Load Adjustment value</param>
        /// <returns>The Average Load Adjustment as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatAvgLoadAdjust(sbyte avgLoadAdjust)
        {
            string FormattedValue = null;

            if (avgLoadAdjust != unchecked((sbyte)0x80))
            {
                if (avgLoadAdjust >= -100 && avgLoadAdjust <= 100)
                {
                    FormattedValue = avgLoadAdjust.ToString(CultureInfo.InvariantCulture) + "%";
                }
                else
                {
                    FormattedValue = "Invalid Value";
                }
            }
            else
            {
                FormattedValue = "Not Used";
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats the temperature set point to a readable format
        /// </summary>
        /// <param name="temperatureSetPoint">The raw temperature set point</param>
        /// <returns>The temperature set point formatted as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatTempSetPoint(short temperatureSetPoint)
        {
            string FormattedValue = null;

            // A value of 0x8000 means the value is not used 
            if (temperatureSetPoint != unchecked((short)0x8000))
            {
                // The temp set point is 1/100th of a degree C
                float ActualSetPoint = temperatureSetPoint / 100.0f;
                FormattedValue = ActualSetPoint.ToString("F3", CultureInfo.InvariantCulture) + " °C";
            }
            else
            {
                FormattedValue = "Not Used";
            }

            return FormattedValue;
        }

        /// <summary>
        /// Formats the temperature offset values to a readable format
        /// </summary>
        /// <param name="temperatureOffset">The raw offset value</param>
        /// <returns>The temperature offset formatted as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string FormatTempOffset(byte temperatureOffset)
        {
            // The offset is 1/10th of a degree C
            string FormattedValue = null;

            if (temperatureOffset != 0xFF)
            {
                float ActualOffset = temperatureOffset / 10.0f;
                FormattedValue = ActualOffset.ToString("F2", CultureInfo.InvariantCulture) + " °C";
            }
            else
            {
                FormattedValue = "Not Used";
            }

            return FormattedValue;
        }

        /// <summary>
        /// Gets the list of Device Groups
        /// </summary>
        /// <param name="groups">The groups to get</param>
        /// <returns>The list of deviceGroups as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created

        private string GetDeviceGroupNames(DRLCDeviceClasses groups)
        {
            string Groups = "";

            // Go through each of the possible values in the enumeration
            foreach(DRLCDeviceClasses CurrentValue in EnumDescriptionRetriever.GetValues<DRLCDeviceClasses>())
            {
                // Check to see if that Flag is set
                if (CurrentValue != DRLCDeviceClasses.None && (groups & CurrentValue) == CurrentValue)
                {
                    // If we already have a value add a comma to separate the values
                    if (Groups.Length > 0)
                    {
                        Groups += ", ";
                    }

                    Groups += EnumDescriptionRetriever.RetrieveDescription(CurrentValue);
                }
            }

            return Groups;
        }

        #endregion

        #region Member Variables

        private DateTime m_Time;
        private uint m_uiSequenceNumber;
        private byte m_byType;
        private ushort m_usMessageLength;
        private byte[] m_TxData;
        private List<DRLCMessageData> m_MessageData;
        private uint m_EventID;

        #endregion
    }

    /// <summary>
    /// Class that represents a single HAN client data record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/07/06 AF  8.00.00 N/A    Created
    //
    public class HANClientDataRcd
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="usDataSize">Size of the client data</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/06 AF  8.00.00 N/A    Created
        //
        public HANClientDataRcd(ushort usDataSize)
        {
            m_ulClientAddress = 0;
            m_dtTimeRecorded = new DateTime(1970, 1, 1, 0, 0, 0);
            m_usClientDataSize = 0;
            m_abyClientData = new byte[usDataSize];
        }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// MAC address of the HAN client, which is made up of the Open Way meter 
        /// serial number, Itron's Organizational Unique Identifier, and the node type 
        /// of the client (Node type 2 = Gas meter)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/06 AF  8.00.00 N/A    Created
        //
        public UInt64 ClientAddress
        {
            get
            {
                return m_ulClientAddress;
            }
            set
            {
                m_ulClientAddress = value;
            }
        }

        /// <summary>
        /// Date/time that the HAN module received the client data from the client meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/06 AF  8.00.00 N/A    Created
        //
        public DateTime TimeRecorded
        {
            get
            {
                return m_dtTimeRecorded;
            }
            set
            {
                m_dtTimeRecorded = value;
            }
        }

        /// <summary>
        /// Number of bytes of data received from the client
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/06 AF  8.00.00 N/A    Created
        //
        public UInt16 ClientDataSize
        {
            get
            {
                return m_usClientDataSize;
            }
            set
            {
                m_usClientDataSize = value;
            }
        }

        /// <summary>
        /// Byte array containing the HAN client data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/06 AF  8.00.00 N/A    Created
        //
        public byte[] ClientData
        {
            get
            {
                return m_abyClientData;
            }
            set
            {
                m_abyClientData = value;
            }
        }

        #endregion

        #region Members

        private UInt64 m_ulClientAddress;
        private DateTime m_dtTimeRecorded;
        private UInt16 m_usClientDataSize;
        private byte[] m_abyClientData;

        #endregion
    }

    /// <summary>
    /// Class that holds a single command to be sent to the client meter
    /// </summary>
    public class ClientCmdRcd
    {
        #region Public Properties

        /// <summary>
        /// Numerical value of the command identifier
        /// </summary>
        public byte CmdID
        {
            get
            {
                return m_byCmdID;
            }
            set
            {
                m_byCmdID = value;
            }
        }

        /// <summary>
        /// Packet version
        /// </summary>
        public byte PacketVer
        {
            get
            {
                return m_byPktVer;
            }
            set
            {
                m_byPktVer = value;
            }
        }

        /// <summary>
        /// Sequence number
        /// </summary>
        public UInt16 SequenceNum
        {
            get
            {
                return m_usSeqenceNbr;
            }
            set
            {
                m_usSeqenceNbr = value;
            }
        }

        /// <summary>
        /// Any data that might be included in the command.  The content will
        /// vary with the command
        /// </summary>
        public byte[] CmdData
        {
            get
            {
                return m_abyCmdData;
            }
            set
            {
                m_abyCmdData = value;
            }
        }

        #endregion

        #region Members

        private byte m_byCmdID;
        private byte m_byPktVer;
        private UInt16 m_usSeqenceNbr;
        private byte[] m_abyCmdData;

        #endregion
    }

    /// <summary>
    /// Class that holds the list of commands to be sent to the client meter
    /// </summary>
    public class ClientCfgRcd
    {
        #region Public Properties

        /// <summary>
        /// MAC address of the client meter
        /// </summary>
        public UInt64 ClientAddress
        {
            get
            {
                return m_ulClientAddr;
            }
            set
            {
                m_ulClientAddr = value;
            }
        }

        /// <summary>
        /// Number of command records to be passed to the client
        /// </summary>
        public byte NumberCfgCmds
        {
            get
            {
                return m_byNbrClientCfgCmds;
            }
            set
            {
                m_byNbrClientCfgCmds = value;
            }
        }

        /// <summary>
        /// List of command records to be passed to the client
        /// </summary>
        public List<ClientCmdRcd> ClientCmdList
        {
            get
            {
                return m_lstClientCmdRcds;
            }
            set
            {
                m_lstClientCmdRcds = value;
            }
        }

        #endregion

        #region Members

        private UInt64 m_ulClientAddr;
        private byte m_byNbrClientCfgCmds;
        private List<ClientCmdRcd> m_lstClientCmdRcds;

        #endregion
    }


        /// <summary>
    /// Class that represents a single OTA Poll Message
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/25/16 WPL                Created
    //
    public class HAN_OTA_Poll_Msg
    {
        #region Constants

        private const byte FIELD_CONTROL_HARDWARE_VERSION_PRESENT_MASK = 0x01;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        //
        public HAN_OTA_Poll_Msg()
        {
            ZedEui = 0xFFFFFFFFFFFFFFFF;
            FieldControl = 0xFF;
            ManufCode = 0xFFFF;
            ImageType = 0xFFFF;
            CurrentFileVersion = 0xFFFFFFFF;
            HardwareVersion = 0xFFFF;
            ReceivedTimestamp = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public UInt32 ReceivedTimestamp
        {
            get
            {
                return m_uiRecievedTimestamp;
            }
            set
            {
                m_uiRecievedTimestamp = value;
            }
        }

        /// <summary>
        /// The datetime that the message was received in UTC.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/06/16 jrf 4.70.16 WI 710078 Created
        //  09/14/17 jrf 4.70.17 WI 710078 Corrected time to be seconds from 1/1/2000
        public DateTime DateReceived
        {
            get
            {
                DateTime ReceivedDate = new DateTime(2000, 1, 1);

                ReceivedDate = ReceivedDate.AddSeconds(ReceivedTimestamp);

                return ReceivedDate;
            }
        }

        /// <summary>
        /// ZED EUI element of the bound node
        /// </summary>
        public UInt64 ZedEui
        {
            get
            {
                return m_uiZED_EUI;
            }
            set
            {
                m_uiZED_EUI = value;
            }
        }

        /// <summary>
        /// Field Control element of the bound node
        /// </summary>
        public byte FieldControl
        {
            get
            {
                return m_byFieldControl;
            }
            set
            {
                m_byFieldControl = value;
            }
        }

        /// <summary>
        /// Manufacturing Code of the bound node
        /// </summary>
        public UInt16 ManufCode
        {
            get
            {
                return m_usManufCode;
            }
            set
            {
                m_usManufCode = value;
            }
        }

        /// <summary>
        /// Image Type of the bound node
        /// </summary>
        public UInt16 ImageType
        {
            get
            {
                return m_usImageType;
            }
            set
            {
                m_usImageType = value;
            }
        }

        /// <summary>
        /// Current File Version of the bound node
        /// </summary>
        public UInt32 CurrentFileVersion
        {
            get
            {
                return m_uiCurrentFileVersion;
            }
            set
            {
                m_uiCurrentFileVersion = value;
            }
        }

        /// <summary>
        /// Formatted Current File Version of the bound node
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/06/16 jrf 4.70.16 WI 710078 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string FormattedCurrentFileVersion
        {
            get
            {
                byte[] FileVersionBytes = BitConverter.GetBytes(m_uiCurrentFileVersion);
                string FormattedVersion = "0.000.000";

                try
                {
                    if (null != FileVersionBytes && 3 <= FileVersionBytes.Length)
                    {
                        Array.Reverse(FileVersionBytes);

                        FormattedVersion = FileVersionBytes[0].ToString() + "." + FileVersionBytes[1].ToString("D3") + "." + FileVersionBytes[2].ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }

        /// <summary>
        /// Extracts from the Field Control byte whether the hardware version is present.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/06/16 jrf 4.70.16 WI 710078 Created
        public bool IsHardwareVersionPresent
        {
            get
            {
                bool Present = false;

                if ((FieldControl & FIELD_CONTROL_HARDWARE_VERSION_PRESENT_MASK) == FIELD_CONTROL_HARDWARE_VERSION_PRESENT_MASK)
                {
                    Present = true;
                }

                return Present;
            }
        }

        /// <summary>
        /// Hardware Version of the bound node
        /// </summary>
        public UInt16 HardwareVersion
        {
            get
            {
                return m_usHardwareVersion;
            }
            set
            {
                m_usHardwareVersion = value;
            }
        }

        /// <summary>
        /// Formatted hardware version of the bound node
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/06/16 jrf 4.70.16 WI 710078 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string FormattedHardwareVersion
        {
            get
            {
                byte[] FileVersionBytes = BitConverter.GetBytes(m_usHardwareVersion);
                string FormattedVersion = "0.000";

                try
                {
                    if (null != FileVersionBytes && 2 <= FileVersionBytes.Length)
                    {
                        Array.Reverse(FileVersionBytes);

                        FormattedVersion = FileVersionBytes[0].ToString() + "." + FileVersionBytes[1].ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }

        #endregion

        #region Members

        private UInt32 m_uiRecievedTimestamp;
        private UInt64 m_uiZED_EUI;
        private byte m_byFieldControl;
        private UInt16 m_usManufCode;
        private UInt16 m_usImageType;
        private UInt32 m_uiCurrentFileVersion;
        private UInt16 m_usHardwareVersion;

        #endregion
    }

    /// <summary>
    /// Class that represents a single HAN binding record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/26/16 WPL                Created
    //
    public class HANBindingRcd
    {
        #region Public Properties

        /// <summary>
        /// MAC address of the bound node
        /// </summary>
        public UInt64 LongAddress
        {
            get
            {
                return m_ulLongAddr;
            }
            set
            {
                m_ulLongAddr = value;
            }
        }

        /// <summary>
        /// Network address of the bound node
        /// </summary>
        public UInt16 ShortAddress
        {
            get
            {
                return m_usShortAddr;
            }
            set
            {
                m_usShortAddr = value;
            }
        }

        #endregion

        #region Members

        private UInt64 m_ulLongAddr;
        private UInt16 m_usShortAddr;

        #endregion
    }

    /// <summary>
    /// Class that represents a single HAN binding record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  08/02/16 WPL                Created
    //
    public class HANDiagnosticsElementRcd
    {
        #region Public Properties

        
        /// <summary>
        /// ZED_EUI
        /// </summary>
        public UInt64 ZED_EUI
        {
            get
            {
                return m_ZED_EUI;
            }
            set
            {
                m_ZED_EUI = value;
            }
        }

        /// <summary>
        /// Activation Command sent to the device
        /// </summary>
        public byte ActivationCommandSent
        {
            get
            {
                return m_byActivationCommandSent;
            }
            set
            {
                m_byActivationCommandSent = value;
            }
        }

        /// <summary>
        /// Last Firmware Download Block Size Used
        /// </summary>
        public Byte LastFwdlBlockSizeUsed
        {
            get
            {
                return m_uiLastFwdlBlockSizeUsed;
            }
            set
            {
                m_uiLastFwdlBlockSizeUsed = value;
            }
        }

        /// <summary>
        /// Get the current firmware version
        /// </summary>
        public UInt32 CurrentFirmwareVersion
        {
            get
            {
                return m_uiCurrentFirmwareVersion;
            }
            set
            {
                m_uiCurrentFirmwareVersion = value;
            }
        }

        /// <summary>
        /// Formatted Current File Version of the bound node
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.16 WI 714616 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string FormattedCurrentFileVersion
        {
            get
            {
                byte[] FileVersionBytes = BitConverter.GetBytes(CurrentFirmwareVersion);
                string FormattedVersion = "0.000.000";

                try
                {
                    if (null != FileVersionBytes && 3 <= FileVersionBytes.Length)
                    {
                        Array.Reverse(FileVersionBytes);

                        FormattedVersion = FileVersionBytes[0].ToString() + "." + FileVersionBytes[1].ToString("D3") + "." + FileVersionBytes[2].ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }

        /// <summary>
        /// Get the new firmware version
        /// </summary>
        public UInt32 NewFirmwareVersion
        {
            get
            {
                return m_uiNewFirmwareVersion;
            }
            set
            {
                m_uiNewFirmwareVersion = value;
            }
        }

        /// <summary>
        /// Formatted new File Version of the bound node
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.16 WI 714616 Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string FormattedNewFileVersion
        {
            get
            {
                byte[] FileVersionBytes = BitConverter.GetBytes(NewFirmwareVersion);
                string FormattedVersion = "0.000.000";

                try
                {
                    if (null != FileVersionBytes && 3 <= FileVersionBytes.Length)
                    {
                        Array.Reverse(FileVersionBytes);

                        FormattedVersion = FileVersionBytes[0].ToString() + "." + FileVersionBytes[1].ToString("D3") + "." + FileVersionBytes[2].ToString("D3");
                    }
                }
                catch { }

                return FormattedVersion;
            }
        }


        /// <summary>
        /// Number of bytes sent
        /// </summary>
        public UInt32 NBytesSent
        {
            get
            {
                return m_uiNBytesSent;
            }
            set
            {
                m_uiNBytesSent = value;
            }
        }

        /// <summary>
        /// Number of bytes remaining
        /// </summary>
        public UInt32 NBytesRemaining
        {
            get
            {
                return m_uiNBytesRemaining;
            }
            set
            {
                m_uiNBytesRemaining = value;
            }
        }

        /// <summary>
        /// RX Timestamp
        /// </summary>
        public UInt32 LastCommandRXTimestamp
        {
            get
            {
                return m_uiLastCommandRXTimestamp;
            }
            set
            {
                m_uiLastCommandRXTimestamp = value;
            }
        }

        /// <summary>
        /// The datetime that the last command was received in UTC.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.16 WI 714616 Created
        //  09/14/17 jrf 4.70.17 WI 714616 Corrected time to be seconds from 1/1/2000
        public DateTime DateLastCommandReceived
        {
            get
            {
                DateTime ReceivedDate = new DateTime(2000, 1, 1);

                ReceivedDate = ReceivedDate.AddSeconds(LastCommandRXTimestamp);

                return ReceivedDate;
            }
        }

        /// <summary>
        /// TX Timestamp
        /// </summary>
        public UInt32 LastCommandTXTimestamp
        {
            get
            {
                return m_uiLastCommandTXTimestamp;
            }
            set
            {
                m_uiLastCommandTXTimestamp = value;
            }
        }

        /// <summary>
        /// The datetime that the last command was transmitted in UTC.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/09/16 jrf 4.70.16 WI 714616 Created
        //  09/14/17 jrf 4.70.17 WI 714616 Corrected time to be seconds from 1/1/2000
        public DateTime DateLastCommandTransmitted
        {
            get
            {
                DateTime TransmitDate = new DateTime(2000, 1, 1);

                TransmitDate = TransmitDate.AddSeconds(LastCommandTXTimestamp);

                return TransmitDate;
            }
        }

        #endregion

        #region Members
        private UInt64 m_ZED_EUI;
        private byte m_byActivationCommandSent;
        private byte m_uiLastFwdlBlockSizeUsed;
        private UInt32 m_uiCurrentFirmwareVersion;
        private UInt32 m_uiNewFirmwareVersion;
        private UInt32 m_uiNBytesSent;
        private UInt32 m_uiNBytesRemaining;
        private UInt32 m_uiLastCommandRXTimestamp;
        private UInt32 m_uiLastCommandTXTimestamp;

        #endregion
    }

    /// <summary>
    /// HAN OTA Cluster Destination Endpoint Element
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  ID Number  Description
    //  -------- --- -------  -- ------  --------------------------------------------
    //  11/17/16 AF  4.70.34  WR 730099  Created
    //
    public class HANClusterDestEndpointElement
    {
        #region Public Properties

        /// <summary>
        /// Id of the endpoint
        /// </summary>
        public byte EndpointID
        {
            get
            {
                return m_EndpointID;
            }
            set
            {
                m_EndpointID = value;
            }
        }

        #endregion

        #region Members

        private byte m_EndpointID;

        #endregion
    }

    /// <summary>
    /// Class that represents a single HAN key record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/11/08 AF  1.50.34        Created
    //
    public class HANKeyRcd : IEquatable<HANKeyRcd>
    {
        #region Definitions

        /// <summary>
        /// Enumeration of the currently valid key types for the HAN security table
        /// </summary>
        public enum HANKeyType
        {
            /// <summary>
            /// The HAN network security key type
            /// </summary>
            NetworkKey = 1,
            /// <summary>
            /// The HAN network global link key type
            /// </summary>
            GlobalLinkKey = 2,
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public HANKeyRcd()
        {
            m_abyHANKey = new byte[16];
            m_KeyType = 0;
        }

        #region Public Properties

        /// <summary>
        /// Type of security key:
        /// 0 -- unused
        /// 1 -- Network Key
        /// 2 -- Master Link Key
        /// 3 -- Device Link Key
        /// 4 - 255 -- Undefined
        /// </summary>
        public HANKeyType KeyType
        {
            get
            {
                return m_KeyType;
            }
            set
            {
                m_KeyType = value;
            }
        }

        /// <summary>
        /// 128 bit encryption key
        /// </summary>
        public byte[] HANKey
        {
            get
            {
                return m_abyHANKey;
            }
            set
            {
                m_abyHANKey = value;
            }
        }

        /// <summary>
        /// Determines whether or not the current key is equal to the specified key.
        /// </summary>
        /// <param name="other">The key to compare to.</param>
        /// <returns>True if the keys are the same. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/16/08 RCG 1.50.36        Created

        public bool Equals(HANKeyRcd other)
        {
            bool bIsEqual = true;

            if (this.KeyType == other.KeyType && this.HANKey.Length == other.HANKey.Length)
            {
                for (int iIndex = 0; iIndex < this.HANKey.Length; iIndex++)
                {
                    if (this.HANKey[iIndex] != other.HANKey[iIndex])
                    {
                        bIsEqual = false;
                        break;
                    }
                }
            }
            else
            {
                bIsEqual = false;
            }

            return bIsEqual;
        }

        #endregion

        #region Members

        private HANKeyType m_KeyType;
        private byte[] m_abyHANKey;

        #endregion
    }

    /// <summary>
    /// Class that represents a single HAN OTA Status record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------

    public class HANOTAStatRcd
    {
        #region Definitions

        /// <summary>
        /// Enumeration for the registration status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        //
        public enum eZEDState : byte
        {
            /// <summary>
            /// No download in progress
            /// </summary>
            NoFWDLandWaiting = 0x00,
            /// <summary>
            /// Blocks of data are being exchanged
            /// </summary>
            BlocksBeingExchanged = 0x01,
            /// <summary>
            /// All the blocks are downloaded
            /// </summary>
            BlokcsDownloaded = 0x02,
            /// <summary>
            /// Waiting to upgrade
            /// </summary>
            WaitingToUpgrade = 0x03,
            /// <summary>
            /// Upgrade count down timer started
            /// </summary>
            UpgradeTimerStarted = 0x04,
            /// <summary>
            /// More than one image needed to upgrade
            /// </summary>
            MultipleImageUpgrade = 0x05,
            /// <summary>
            /// Upgrade will be initiated by an external event
            /// </summary>
            ExternalUpgrade = 0x06,
        }

        /// <summary>
        /// Enumeration for the HAN OTA Coordinator State
        /// </summary>
        public enum eCoordinatorState : byte
        {
            /// <summary>
            /// No FWDL in progress and waiting for FWDL command
            /// </summary>
            NoFWDLinProgress = 0x00,
            /// <summary>
            /// The CRC check will trigger HAN task to start FW download
            /// </summary>
            ImageAvailable = 0x01,
            /// <summary>
            /// FW blocks are being sent
            /// </summary>
            FWDLinProgress = 0x02,
            /// <summary>
            /// The HAN task has received confirmation that download 
            /// complete status has been received from the client
            /// </summary>
            DownloadComplete = 0x03,
            /// <summary>
            /// The CE has sent the activation command
            /// </summary>
            ActivateSent = 0x04,
            /// <summary>
            /// The HAN task has received a countdown status attribute 
            /// from the client
            /// </summary>
            ActivateAck = 0x05,
            /// <summary>
            /// The HAN Task has received a status poll from the 
            /// device with the new FW information
            /// </summary>
            FWDLSuccess = 0x06,
            /// <summary>
            /// The HAN Coordinator received an Abort request from
            /// the ZED
            /// </summary>
            HANOTACoordinatorAbort = 0x09,
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public UInt64 ZedEui
        {
            get
            {
                return m_uiZEDEUI;
            }
            set
            {
                m_uiZEDEUI = value;
            }
        }

        /// <summary>
        /// Gets or sets the registration status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 

        public eZEDState ZedStateStatus
        {
            get
            {
                return (eZEDState)m_byZEDState;
            }
            set
            {
                m_byZEDState = (byte)value;
            }
        }

        /// <summary>
        /// Gets the registration status as a string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 

        public string ZedStatusString
        {
            get
            {
                string strStatus = "";

                switch (ZedStateStatus)
                {
                    case eZEDState.BlocksBeingExchanged:
                        {
                            strStatus = "Blocks Being Exchanged";
                            break;
                        }
                    case eZEDState.BlokcsDownloaded:
                        {
                            strStatus = "Blocks Downloaded";
                            break;
                        }
                    case eZEDState.ExternalUpgrade:
                        {
                            strStatus = "External Upgrade";
                            break;
                        }
                    case eZEDState.MultipleImageUpgrade:
                        {
                            strStatus = "Multiple Image Upgrade";
                            break;
                        }
                    case eZEDState.NoFWDLandWaiting:
                        {
                            strStatus = "No FWDL and Waiting";
                            break;
                        }
                    case eZEDState.UpgradeTimerStarted:
                        {
                            strStatus = "Upgrade Timer Started";
                            break;
                        }
                    case eZEDState.WaitingToUpgrade:
                        {
                            strStatus = "Waiting to Upgrade";
                            break;
                        }
                }

                return strStatus;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UInt32 NumBytesTrans
        {
            get
            {
                return m_uiNumberOfBytesTransmitted;
            }
            set
            {
                m_uiNumberOfBytesTransmitted = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UInt32 TimeStamp
        {
            get
            {
                return m_uiTimestampOfLasTransmit;
            }
            set
            {
                m_uiTimestampOfLasTransmit = value;
            }
        }

        /// <summary>
        /// The datetime of the last transmission to the ZigBee device in UTC.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/08/16 jrf 4.70.16 WI 710078 Created
        //  09/14/17 jrf 4.70.17 WI 710078 Corrected time to be seconds from 1/1/2000
        public DateTime TimeLastTransmission
        {
            get
            {
                DateTime TransmitDate = new DateTime(2000, 1, 1);

                TransmitDate = TransmitDate.AddSeconds(TimeStamp);

                return TransmitDate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte PercentComp
        {
            get
            {
                return m_byPercentComplete;
            }
            set
            {
                m_byPercentComplete = value;
            }
        }

        /// <summary>
        /// Gets or sets the registration status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 

        public eCoordinatorState CoordinatorStateStatus
        {
            get
            {
                return (eCoordinatorState)m_byCoordinatorState;
            }
            set
            {
                m_byCoordinatorState = (byte)value;
            }
        }

        /// <summary>
        /// Gets the registration status as a string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 

        public string CoordinatorStatusString
        {
            get
            {
                string strStatus = "";

                switch (CoordinatorStateStatus)
                {
                    case eCoordinatorState.HANOTACoordinatorAbort:
                        {
                            strStatus = "HAN OTA Coordinator Abort";
                            break;
                        }
                    case eCoordinatorState.ActivateAck:
                        {
                            strStatus = "Activation Acknowledged";
                            break;
                        }
                    case eCoordinatorState.ActivateSent:
                        {
                            strStatus = "Activation Sent";
                            break;
                        }
                    case eCoordinatorState.DownloadComplete:
                        {
                            strStatus = "Download Complete";
                            break;
                        }
                    case eCoordinatorState.FWDLinProgress:
                        {
                            strStatus = "FWDL in Progress";
                            break;
                        }
                    case eCoordinatorState.FWDLSuccess:
                        {
                            strStatus = "FWDL Success";
                            break;
                        }
                    case eCoordinatorState.ImageAvailable:
                        {
                            strStatus = "Image Available";
                            break;
                        }
                    case eCoordinatorState.NoFWDLinProgress:
                        {
                            strStatus = "No FWDL in Progress";
                            break;
                        }
                }

                return strStatus;
            }
        }

        #endregion

        #region Members

        private UInt64 m_uiZEDEUI;
        private byte m_byZEDState;
        private UInt32 m_uiNumberOfBytesTransmitted;
        private UInt32 m_uiTimestampOfLasTransmit;
        private byte m_byPercentComplete;
        private byte m_byCoordinatorState;

        #endregion
    }

    /// <summary>
    /// Class that represents a single AMI HAN device registration record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/12/08 AF  1.50.35        Created
    //  06/19/08 AF  1.50.39        Added additional enumerations according to a
    //                              conversation with Logan.
    //  04/29/10 AF  2.40.45 140957 Added additional enumerations
    //
    public class AMIHANDevRcd
    {
        #region Definitions

        /// <summary>
        /// Enumeration for the registration status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/29/10 AF  2.40.45 140957 Added additional statuses
        //
        public enum HANRegStatus : byte
        {
            /// <summary>
            /// The device is not registered.  This state follows PendingRegistration
            /// in the registration process.
            /// </summary>
            NotRegistered = 0x00,
            /// <summary>
            /// The registration attempt failed.
            /// </summary>
            RegistrationFailed = 0x01,
            /// <summary>
            /// The registration attempt succeeded.
            /// </summary>
            RegistrationSuccess = 0x02,
            /// <summary>
            /// The device does not have a valid certificate.
            /// </summary>
            InvalidCertificate = 0x03,
            /// <summary>
            /// The next state after invalid in the registration process
            /// </summary>
            PendingRegistration = 0x04,
            /// <summary>
            /// The network is up.  This state follows NotRegistered.
            /// </summary>
            NetworkUp = 0x05,
            /// <summary>
            /// The network is down
            /// </summary>
            NetworkDown = 0x06,
            /// <summary>
            /// The network is forming
            /// </summary>
            NetworkForming = 0x07,
            /// <summary>
            /// The device is joining
            /// </summary>
            Joining = 0x08,
            /// <summary>
            /// The network is using the private profile
            /// </summary>
            PrivateProfile = 0x09,
            /// <summary>
            /// The device added itself to the table via binding
            /// </summary>
            ViaBinding = 0x0A,
            /// <summary>
            /// Registration failed either because table was full
            /// or registration not allowed. Should never happen
            /// </summary>
            RegFatalError = 0x0B,
            /// <summary>
            /// The device added itself via key establishment
            /// </summary>
            ViaKE = 0x0C,
            /// <summary>
            /// Initial state in the registration process
            /// </summary>
            InvalidRegistration = 0xFF,
        }

        /// <summary>
        /// Enumeration of the services a device can be registered to receive
        /// </summary>
        public  enum HANRegService : ushort
        {
            /// <summary>
            /// Price cluster
            /// </summary>
            Price = 0x700,
            /// <summary>
            /// Demand Response and Load Control cluster
            /// </summary>
            DemandResponseAndLoadControl = 0x701,
            /// <summary>
            /// Simple metering cluster
            /// </summary>
            SimpleMetering = 0x702,
            /// <summary>
            /// Messaging cluster
            /// </summary>
            Messaging = 0x703,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iRegServiceCount">The number of service in the list.</param>
        /// <param name="iEncryptCertLength">The length of the encryption certificate.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 RCG 1.50.36        Changed to return a HANRegStatus

        public AMIHANDevRcd(int iRegServiceCount, int iEncryptCertLength)
        {
            m_EncryptKey = new HANKeyRcd();
            m_ausRegServiceLst = new ushort[iRegServiceCount];
            m_abyEncryptCert = new byte[iEncryptCertLength];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public UInt64 DeviceEUI
        {
            get
            {
                return m_ulDeviceEUI;
            }
            set
            {
                m_ulDeviceEUI = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastStatusChange
        {
            get
            {
                return m_dtLastStatusChangeTime;
            }
            set
            {
                m_dtLastStatusChangeTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the registration status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 RCG 1.50.36        Changed to return a HANRegStatus

        public HANRegStatus RegStatus
        {
            get
            {
                return (HANRegStatus)m_byRegStatus;
            }
            set
            {
                m_byRegStatus = (byte)value;
            }
        }

        /// <summary>
        /// Gets the registration status as a string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 RCG 1.50.36        Created
        //  06/19/08 AF  1.50.39        Added additional cases after conversation with
        //                              Logan
        //  04/29/10 AF  2.40.45 140957 Added missing cases

        public string RegStatusString
        {
            get
            {
                string strStatus = "";

                switch(RegStatus)
                {
                    case HANRegStatus.NotRegistered:
                    {
                        strStatus = "Not Registered";
                        break;
                    }
                    case HANRegStatus.RegistrationFailed:
                    {
                        strStatus = "Registration Failed";
                        break;
                    }
                    case HANRegStatus.RegistrationSuccess:
                    {
                        strStatus = "Registration Successful";
                        break;
                    }
                    case HANRegStatus.InvalidCertificate:
                    {
                        strStatus = "Invalid Certificate";
                        break;
                    }
                    case HANRegStatus.PendingRegistration:
                    {
                        strStatus = "Pending Registration";
                        break;
                    }
                    case HANRegStatus.NetworkUp:
                    {
                        strStatus = "Network Up";
                        break;
                    }
                    case HANRegStatus.NetworkDown:
                    {
                        strStatus = "Network Down";
                        break;
                    }
                    case HANRegStatus.NetworkForming:
                    {
                        strStatus = "Network Forming";
                        break;
                    }
                    case HANRegStatus.Joining:
                    {
                        strStatus = "Joining Network";
                        break;
                    }
                    case HANRegStatus.PrivateProfile:
                    {
                        strStatus = "Private Profile";
                        break;
                    }
                    case HANRegStatus.InvalidRegistration:
                    {
                        strStatus = "Invalid Registration";
                        break;
                    }
                    case HANRegStatus.ViaBinding:
                    {
                        strStatus = "Device Added by Binding";
                        break;
                    }
                    case HANRegStatus.RegFatalError:
                    {
                        strStatus = "Registration Fatal Error";
                        break;
                    }
                    case HANRegStatus.ViaKE:
                    {
                        strStatus = "Device Added by Key Establishment";
                        break;
                    }
                }

                return strStatus;
            }
        }

        /// <summary>
        /// Number of services in the registered service list
        /// </summary>
        public byte RegServiceCount
        {
            get
            {
                return m_byRegServiceCount;
            }
            set
            {
                m_byRegServiceCount = value;
            }
        }

        /// <summary>
        /// List of services this device is registered to receive
        /// </summary>
        public UInt16[] RegServiceList
        {
            get
            {
                return m_ausRegServiceLst;
            }
            set
            {
                m_ausRegServiceLst = value;
            }
        }

        /// <summary>
        /// Translates the RegServiceList into an array of strings
        /// </summary>
        public string[] RegServiceStrings
        {
            get
            {
                string[] astrServices = new string[RegServiceCount];

                for (int iIndex = 0; iIndex < RegServiceCount; iIndex++)
                {
                    switch (RegServiceList[iIndex])
                    {
                        case (ushort)HANRegService.Price:
                        {
                            astrServices[iIndex] = "Price";
                            break;
                        }
                        case (ushort)HANRegService.DemandResponseAndLoadControl:
                        {
                            astrServices[iIndex] = "Demand Response and Load Control";
                            break;
                        }
                        case (ushort)HANRegService.SimpleMetering:
                        {
                            astrServices[iIndex] = "Simple Metering";
                            break;
                        }
                        case (ushort)HANRegService.Messaging:
                        {
                            astrServices[iIndex] = "Messaging";
                            break;
                        }
                    }
                }

                return astrServices;
            }
        }

        /// <summary>
        /// Device specific link key record
        /// </summary>
        public HANKeyRcd EncryptKey
        {
            get
            {
                return m_EncryptKey;
            }
            set
            {
                m_EncryptKey = value;
            }
        }

        /// <summary>
        /// Length of the encryption
        /// </summary>
        public UInt16 EncryptCertLen
        {
            get
            {
                return m_usEncryptCertLen;
            }
            set
            {
                m_usEncryptCertLen = value;
            }
        }

        /// <summary>
        /// Encryption certificate used to authenticate the device on the network
        /// </summary>
        public byte[] EncryptCert
        {
            get
            {
                return m_abyEncryptCert;
            }
            set
            {
                m_abyEncryptCert = value;
            }
        }

        #endregion

        #region Members

        private UInt64 m_ulDeviceEUI;
        private DateTime m_dtLastStatusChangeTime;
        private byte m_byRegStatus;
        private byte m_byRegServiceCount;
        private UInt16[] m_ausRegServiceLst;
        private HANKeyRcd m_EncryptKey;
        private UInt16 m_usEncryptCertLen;
        private byte[] m_abyEncryptCert;

        #endregion
    }

    /// <summary>
    /// Class that represents a single AMI HAN Device Manufacturer Info Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  09/12/12 PGH 2.70.16        Created
    public class AMIHANMfgInfoRcd
    {
        #region Definitions

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/12/12 PGH 2.70.16       Created

        public AMIHANMfgInfoRcd()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Index in table Mfg table 2130
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/12/12 PGH 2.70.16        Created.
        //
        public byte IndexInTable2130
        {
            get
            {
                return m_byIndexInTable2130;
            }
            set
            {
                m_byIndexInTable2130 = value;
            }
        }

        /// <summary>
        /// HAN Device EUI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/12/12 PGH 2.70.16        Created.
        //
        public UInt64 DeviceEUI
        {
            get
            {
                return m_ulDeviceEUI;
            }
            set
            {
                m_ulDeviceEUI = value;
            }
        }

        /// <summary>
        /// HAN Device Manufacturer Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/12/12 PGH 2.70.16        Created.
        //
        public string ManufacturerName
        {
            get
            {
                return m_strManufacturerName;
            }
            set
            {
                m_strManufacturerName = value;
            }
        }

        /// <summary>
        /// HAN Device Model Identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/12/12 PGH 2.70.16        Created.
        //
        public string ModelIdentifier
        {
            get
            {
                return m_strModelIdentifier;
            }
            set
            {
                m_strModelIdentifier = value;
            }
        }

        /// <summary>
        /// HAN Device FW Date Code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/12/12 PGH 2.70.16        Created.
        //
        public string DateCode
        {
            get
            {
                return m_strDateCode;
            }
            set
            {
                m_strDateCode = value;
            }
        }

        /// <summary>
        /// Last time the HAN Device Manufacturer Info was updated.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/12/12 PGH 2.70.16        Created.
        //
        public DateTime LastUpdate
        {
            get
            {
                return m_dtLastUpdate;
            }
            set
            {
                m_dtLastUpdate = value;
            }
        }

        /// <summary>
        /// Last time the HAN Device Manufacturer Info was requested.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/18/12 PGH 2.70.17         Created.
        //
        public DateTime LastRequest
        {
            get
            {
                return m_dtLastRequest;
            }
            set
            {
                m_dtLastRequest = value;
            }
        }

        #endregion

        #region Members

        private byte m_byIndexInTable2130;
        private UInt64 m_ulDeviceEUI;
        private string m_strManufacturerName;
        private string m_strModelIdentifier;
        private string m_strDateCode;
        private DateTime m_dtLastUpdate;
        private DateTime m_dtLastRequest;

        #endregion
    }

    /// <summary>
    /// Class that represents a AMI HAN Reset Status Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  09/14/12 PGH 2.70.16        Created
    public class AMIHANResetStatusRcd
    {
        #region Definitions

        /// <summary>
        /// Enumeration for the HAN Reset State
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16        Created
        //  09/18/12 PGH 2.70.17        Modified from supplied Beryllium tbd file
        //
        public enum HANResetState
        {
            /// <summary>
            /// The device is running.
            /// </summary>
            Running = 0,
            /// <summary>
            /// The device is halted.
            /// </summary>
            Halted = 1,
            /// <summary>
            /// Unknown state.
            /// </summary>
            Unknown = 2,
        }

        /// <summary>
        /// Enumeration for ZigBee Reset Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH  2.70.16       Created
        //  09/18/23 PGH  2.70.17       Modified from Beryllium tbd file
        //
        public enum ZigBeeResetType
        {
            /// <summary>
            /// Normal Reset
            /// </summary>
            NormalReset = 0,
            /// <summary>
            /// Detected Fat Error
            /// </summary>
            DetectedFatError = 1,
            /// <summary>
            /// Watchdog
            /// </summary>
            Watchdog = 2,
            /// <summary>
            /// Core Fault
            /// </summary>
            CoreFault = 3,
            /// <summary>
            /// Stack Lookup Detect
            /// </summary>
            StackLookupDetect = 4,
            /// <summary>
            /// First Used Reset
            /// </summary>
            FirstUsedReset = 5,
            /// <summary>
            /// Periodic Reset
            /// </summary>
            PeriodicReset = 6,
            /// <summary>
            /// Disable ZigBee
            /// </summary>
            DisableZigBee = 7,
            /// <summary>
            /// Table Empty Periodic Reset
            /// </summary>
            TableEmptyPeriodicReset = 8,
            /// <summary>
            /// Diagnostic Stop
            /// </summary>
            DiagnosticStop = 9,
            /// <summary>
            /// Network Restart
            /// </summary>
            NetworkRestart = 10,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16       Created

        public AMIHANResetStatusRcd()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Reset State
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public HANResetState ResetState
        {
            get
            {
                return (HANResetState)m_byResetState;
            }
            set
            {
                m_byResetState = (byte)value;
            }
        }

        /// <summary>
        /// Reset Count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public UInt16 ResetCount
        {
            get
            {
                return m_usResetCount;
            }
            set
            {
                m_usResetCount = value;
            }
        }

        /// <summary>
        /// Reset Limit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public UInt16 ResetLimit
        {
            get
            {
                return m_usResetLimit;
            }
            set
            {
                m_usResetLimit = value;
            }
        }

        /// <summary>
        /// Last Reset Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public ZigBeeResetType LastResetType
        {
            get
            {
                return (ZigBeeResetType)m_byLastResetType;
            }
            set
            {
                m_byLastResetType = (byte)value;
            }
        }

        /// <summary>
        /// Last Reset Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public DateTime LastResetTime
        {
            get
            {
                return m_dtLastResetTime;
            }
            set
            {
                m_dtLastResetTime = value;
            }
        }

        #endregion

        #region Members

        private byte m_byResetState;
        private UInt16 m_usResetCount;
        private UInt16 m_usResetLimit;
        private byte m_byLastResetType;
        private DateTime m_dtLastResetTime;

        #endregion
    }

    /// <summary>
    /// Class that represents a AMI HAN Join Status Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  09/14/12 PGH 2.70.16        Created
    public class AMIHANJoinStatusRcd
    {
        #region Definitions

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16       Created

        public AMIHANJoinStatusRcd()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Join is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public bool JoinIsEnabled
        {
            get
            {
                return m_bJoinIsEnabled;
            }
            set
            {
                m_bJoinIsEnabled = value;
            }
        }

        /// <summary>
        /// Extended duration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public UInt16 ExtendedDuration
        {
            get
            {
                return m_usExtendedDuration;
            }
            set
            {
                m_usExtendedDuration = value;
            }
        }

        #endregion

        #region Members

        private bool m_bJoinIsEnabled;
        private UInt16 m_usExtendedDuration;

        #endregion
    }

    /// <summary>
    /// Class that represents a AMI HAN Radio Status Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  09/14/12 PGH 2.70.16        Created
    public class AMIHANRadioStatusRcd
    {
        #region Definitions

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16       Created

        public AMIHANRadioStatusRcd()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Is Radio Enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public bool IsRadioEnabled
        {
            get
            {
                return m_bIsRadioEnabled;
            }
            set
            {
                m_bIsRadioEnabled = value;
            }
        }

        /// <summary>
        /// Is Quiet Mode Enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public bool IsQuietModeEnabled
        {
            get
            {
                return m_bIsQuietModeEnabled;
            }
            set
            {
                m_bIsQuietModeEnabled = value;
            }
        }

        #endregion

        #region Members

        private bool m_bIsRadioEnabled;
        private bool m_bIsQuietModeEnabled;

        #endregion
    }

    /// <summary>
    /// Class that represents a AMI HAN Diagnostic Read Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  09/14/12 PGH 2.70.16        Created
    public class AMIHANDiagnosticReadRcd
    {
        #region Definitions

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16       Created

        public AMIHANDiagnosticReadRcd()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// HAN Reset Status Record
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public AMIHANResetStatusRcd ResetStatus
        {
            get
            {
                return m_HANResetStatusRcd;
            }
            set
            {
                m_HANResetStatusRcd = value;
            }
        }

        /// <summary>
        /// HAN Join Status Record
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public AMIHANJoinStatusRcd JoinStatus
        {
            get
            {
                return m_HANJoinStatusRcd;
            }
            set
            {
                m_HANJoinStatusRcd = value;
            }
        }

        /// <summary>
        /// HAN Radio Status Record
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 09/14/12 PGH 2.70.16        Created.
        //
        public AMIHANRadioStatusRcd RadioStatus
        {
            get
            {
                return m_HANRadioStatusRcd;
            }
            set
            {
                m_HANRadioStatusRcd = value;
            }
        }
        

        #endregion

        #region Members

        private AMIHANResetStatusRcd m_HANResetStatusRcd;
        private AMIHANJoinStatusRcd m_HANJoinStatusRcd;
        private AMIHANRadioStatusRcd m_HANRadioStatusRcd;

        #endregion
    }

    /// <summary>
    /// Class that represents a single AMI HAN response log record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/12/08 AF  1.50.35        Created
    //
    public class AMIHANRspLogRcd
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byNumResponses">The number of responses.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 RCG 1.50.36        Created

        public AMIHANRspLogRcd(byte byNumResponses)
        {
            m_auiRxMsgSeqNbrs = new uint[byNumResponses];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// EUI-64 of the registered device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public UInt64 DeviceEUI
        {
            get
            {
                return m_ulDeviceEUI;
            }
            set
            {
                m_ulDeviceEUI = value;
            }
        }

        /// <summary>
        /// RSSI of the last message received from this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public sbyte LastMsgRSSI
        {
            get
            {
                return m_sbyLastMsgRSSI;
            }
            set
            {
                m_sbyLastMsgRSSI = value;
            }
        }

        /// <summary>
        /// LQI of the last message received from this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public byte LastMsgLQI
        {
            get
            {
                return m_bytLastMsgLQI;
            }
            set
            {
                m_bytLastMsgLQI = value;
            }
        }

        /// <summary>
        /// Average RSSI of all messages sent by this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public sbyte AvgRSSI
        {
            get
            {
                return m_sbyAvgRSSI;
            }
            set
            {
                m_sbyAvgRSSI = value;
            }
        }

        /// <summary>
        /// Average LQI of all messages sent by this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public byte AvgLQI
        {
            get
            {
                return m_bytAvgLQI;
            }
            set
            {
                m_bytAvgLQI = value;
            }
        }

        /// <summary>
        /// Number of messages received from this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public ushort NbrMsgsRx
        {
            get
            {
                return m_usNbrMsgsRx;
            }
            set
            {
                m_usNbrMsgsRx = value;
            }
        }

        /// <summary>
        /// Number of messages transmitted to this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public ushort NbrMsgsTx
        {
            get
            {
                return m_usNbrMsgsTx;
            }
            set
            {
                m_usNbrMsgsTx = value;
            }
        }

        /// <summary>
        /// Number of attempted transmission that this device failed to receive
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public ushort NbrTxFailures
        {
            get
            {
                return m_usNbrTxFailures;
            }
            set
            {
                m_usNbrTxFailures = value;
            }
        }

        /// <summary>
        /// Sequence number in the HAN communication log of the most recent
        /// messages received from this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/08 AF  1.50.36        Created
        //
        public uint[] RxMsgSeqNbrs
        {
            get
            {
                return m_auiRxMsgSeqNbrs;
            }
            set
            {
                m_auiRxMsgSeqNbrs = value;
            }
        }

        #endregion

        #region Members

        private UInt64 m_ulDeviceEUI;
        private sbyte m_sbyLastMsgRSSI;
        private byte m_bytLastMsgLQI;
        private sbyte m_sbyAvgRSSI;
        private byte m_bytAvgLQI;
        private ushort m_usNbrMsgsRx;
        private ushort m_usNbrMsgsTx;
        private ushort m_usNbrTxFailures;
        private uint[] m_auiRxMsgSeqNbrs;

        #endregion
    }

    /// <summary>
    /// Class that represents a single AMI HAN price entry record.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  05/07/09 jrf 2.20.03        Created.
    //
    public class AMIHANPriceEntryRcd
    {

        #region Constants

        private byte PRICE_TRAILING_DIGIT_MASK = 0xF0;
        private byte PRICE_TIER_MASK = 0x0F;
        private byte PRICE_TRAILING_DIGIT_SHIFT = 4;
        private byte NBR_PRICE_TIERS_MASK = 0xF0;
        private byte REGISTER_TIER_MASK = 0x0F;
        private byte NBR_PRICE_TIERS_SHIFT = 4;
        private int ZERO = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.

        public AMIHANPriceEntryRcd()
        {
            m_byRateLabelLen = 0;
            m_uiProviderID = 0;
            m_strRateLabel = "";
            m_uiIssuerEventID = 0;
            m_byUnitOfMeasure = 0;
            m_usCurrency = 0;
            m_byPriceTrailandTier = 0;
            m_byNbrTiersAndRegTier = 0;
            m_uiStartTime = 0;
            m_usDuration = 0;
            m_uiPrice = 0;
            m_byPriceRatio = 0;
            m_uiGenPrice = 0;
            m_byGenPriceRatio = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The unique ID of the commodity provider.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public UInt32 ProviderID
        {
            get
            {
                return m_uiProviderID;
            }
            set
            {
                m_uiProviderID = value;
            }
        }

        /// <summary>
        /// The label for the rate as a string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //  07/20/11 RCG 2.50.26        Simplifying code for Rate Label to use strings

        public string RateLabel
        {
            get
            {
                return m_strRateLabel;
            }
            set
            {
                m_strRateLabel = value;
            }
        }

        /// <summary>
        /// The unique ID generated by the commodity provider.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public UInt32 IssuerEventID
        {
            get
            {
                return m_uiIssuerEventID;
            }
            set
            {
                m_uiIssuerEventID = value;
            }
        }

        /// <summary>
        /// The text representing the commodity that is being measured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public string UnitOfMeasureText
        {
            get
            {
                UnitOfMeasure UOM = UnitOfMeasure.Undefined;

                if (Enum.IsDefined(typeof(UnitOfMeasure), m_byUnitOfMeasure))
                {
                    UOM = (UnitOfMeasure)m_byUnitOfMeasure;
                }

                return GetUnitOfMeasureText(UOM);
            }
        }

        /// <summary>
        /// The text representing the symbol for the local unit of currency 
        /// used in the price field.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //  07/19/12 jrf 2.60.46 199803 Updated to return correct currency symbol for Canada.
        //
        public string CurrencySymbol
        {
            get
            {
                UnitOfCurrency UOC = UnitOfCurrency.UndefinedCurrency;
                string strUOCText = "";

                if (Enum.IsDefined(typeof(UnitOfCurrency), m_usCurrency))
                {
                    UOC = (UnitOfCurrency)m_usCurrency;
                }

                switch (UOC)
                {
                    case UnitOfCurrency.CanadianDollar:
                    case UnitOfCurrency.USDollar:
                        {
                            strUOCText = "$";
                            break;
                        }
                    //Add in more currency types as needed...  
                    default:
                        {
                            strUOCText = "";
                            break;
                        }
                }

                return strUOCText;
            }
        }

        /// <summary>
        /// The text representing the symbol for the local unit of currency 
        /// used in the price field.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/19/12 jrf 2.60.46 199803 Created.
        //
        public string CurrencyName
        {
            get
            {
                UnitOfCurrency UOC = UnitOfCurrency.UndefinedCurrency;
                string strUOCText = "";

                if (Enum.IsDefined(typeof(UnitOfCurrency), m_usCurrency))
                {
                    UOC = (UnitOfCurrency)m_usCurrency;
                }

                switch (UOC)
                {
                    case UnitOfCurrency.UICFranc :
                        {
                            strUOCText = "UIC Franc ";
                            break;
                        }
                    case UnitOfCurrency.Lek :
                        {
                            strUOCText = "Lek ";
                            break;
                        }
                    case UnitOfCurrency.AlgerianDinar :
                        {
                            strUOCText = "Algerian Dinar ";
                            break;
                        }
                    case UnitOfCurrency.ArgentinePeso :
                        {
                            strUOCText = "Argentine Peso ";
                            break;
                        }
                    case UnitOfCurrency.AustralianDollar :
                        {
                            strUOCText = "Australian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.BahamianDollar :
                        {
                            strUOCText = "Bahamian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.BahrainiDinar :
                        {
                            strUOCText = "Bahraini Dinar ";
                            break;
                        }
                    case UnitOfCurrency.Taka :
                        {
                            strUOCText = "Taka ";
                            break;
                        }
                    case UnitOfCurrency.ArmenianDram :
                        {
                            strUOCText = "Armenian Dram ";
                            break;
                        }
                    case UnitOfCurrency.BarbadosDollar :
                        {
                            strUOCText = "Barbados Dollar ";
                            break;
                        }
                    case UnitOfCurrency.BermudianDollar :
                        {
                            strUOCText = "Bermudian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Ngultrum :
                        {
                            strUOCText = "Ngultrum ";
                            break;
                        }
                    case UnitOfCurrency.Boliviano :
                        {
                            strUOCText = "Boliviano ";
                            break;
                        }
                    case UnitOfCurrency.Pula :
                        {
                            strUOCText = "Pula ";
                            break;
                        }
                    case UnitOfCurrency.BelizeDollar :
                        {
                            strUOCText = "Belize Dollar ";
                            break;
                        }
                    case UnitOfCurrency.SolomonIslandsDollar :
                        {
                            strUOCText = "Solomon Islands Dollar ";
                            break;
                        }
                    case UnitOfCurrency.BruneiDollar :
                        {
                            strUOCText = "Brunei Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Kyat :
                        {
                            strUOCText = "Kyat ";
                            break;
                        }
                    case UnitOfCurrency.BurundiFranc :
                        {
                            strUOCText = "Burundi Franc ";
                            break;
                        }
                    case UnitOfCurrency.Riel :
                        {
                            strUOCText = "Riel ";
                            break;
                        }
                    case UnitOfCurrency.CanadianDollar :
                        {
                            strUOCText = "Canadian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.CapeVerdeEscudo :
                        {
                            strUOCText = "Cape Verde Escudo ";
                            break;
                        }
                    case UnitOfCurrency.CaymanIslandsDollar :
                        {
                            strUOCText = "Cayman Islands Dollar ";
                            break;
                        }
                    case UnitOfCurrency.SriLankaRupee :
                        {
                            strUOCText = "Sri Lanka Rupee ";
                            break;
                        }
                    case UnitOfCurrency.ChileanPeso :
                        {
                            strUOCText = "Chilean Peso ";
                            break;
                        }
                    case UnitOfCurrency.YuanRenminbi :
                        {
                            strUOCText = "Yuan Renminbi ";
                            break;
                        }
                    case UnitOfCurrency.ColombianPeso :
                        {
                            strUOCText = "Colombian Peso ";
                            break;
                        }
                    case UnitOfCurrency.ComoroFranc :
                        {
                            strUOCText = "Comoro Franc ";
                            break;
                        }
                    case UnitOfCurrency.CostaRicanColon :
                        {
                            strUOCText = "Costa Rican Colon ";
                            break;
                        }
                    case UnitOfCurrency.CroatianKuna :
                        {
                            strUOCText = "Croatian Kuna ";
                            break;
                        }
                    case UnitOfCurrency.CubanPeso :
                        {
                            strUOCText = "Cuban Peso ";
                            break;
                        }
                    case UnitOfCurrency.CzechKoruna :
                        {
                            strUOCText = "Czech Koruna ";
                            break;
                        }
                    case UnitOfCurrency.DanishKrone :
                        {
                            strUOCText = "Danish Krone ";
                            break;
                        }
                    case UnitOfCurrency.DominicanPeso :
                        {
                            strUOCText = "Dominican Peso ";
                            break;
                        }
                    case UnitOfCurrency.ElSalvadorColon :
                        {
                            strUOCText = "El Salvador Colon ";
                            break;
                        }
                    case UnitOfCurrency.EthiopianBirr :
                        {
                            strUOCText = "Ethiopian Birr ";
                            break;
                        }
                    case UnitOfCurrency.Nakfa :
                        {
                            strUOCText = "Nakfa ";
                            break;
                        }
                    case UnitOfCurrency.Kroon :
                        {
                            strUOCText = "Kroon ";
                            break;
                        }
                    case UnitOfCurrency.FalklandIslandsPound :
                        {
                            strUOCText = "Falkland Islands Pound ";
                            break;
                        }
                    case UnitOfCurrency.FijiDollar :
                        {
                            strUOCText = "Fiji Dollar ";
                            break;
                        }
                    case UnitOfCurrency.DjiboutiFranc :
                        {
                            strUOCText = "Djibouti Franc ";
                            break;
                        }
                    case UnitOfCurrency.Dalasi :
                        {
                            strUOCText = "Dalasi ";
                            break;
                        }
                    case UnitOfCurrency.GibraltarPound :
                        {
                            strUOCText = "Gibraltar Pound ";
                            break;
                        }
                    case UnitOfCurrency.Quetzal :
                        {
                            strUOCText = "Quetzal ";
                            break;
                        }
                    case UnitOfCurrency.GuineaFranc :
                        {
                            strUOCText = "Guinea Franc ";
                            break;
                        }
                    case UnitOfCurrency.GuyanaDollar :
                        {
                            strUOCText = "Guyana Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Gourde :
                        {
                            strUOCText = "Gourde ";
                            break;
                        }
                    case UnitOfCurrency.Lempira :
                        {
                            strUOCText = "Lempira ";
                            break;
                        }
                    case UnitOfCurrency.HongKongDollar :
                        {
                            strUOCText = "Hong Kong Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Forint :
                        {
                            strUOCText = "Forint ";
                            break;
                        }
                    case UnitOfCurrency.IcelandKrona :
                        {
                            strUOCText = "Iceland Krona ";
                            break;
                        }
                    case UnitOfCurrency.IndianRupee :
                        {
                            strUOCText = "Indian Rupee ";
                            break;
                        }
                    case UnitOfCurrency.Rupiah :
                        {
                            strUOCText = "Rupiah ";
                            break;
                        }
                    case UnitOfCurrency.IranianRial :
                        {
                            strUOCText = "Iranian Rial ";
                            break;
                        }
                    case UnitOfCurrency.IraqiDinar :
                        {
                            strUOCText = "Iraqi Dinar ";
                            break;
                        }
                    case UnitOfCurrency.NewIsraeliSheqel :
                        {
                            strUOCText = "New Israeli Sheqel ";
                            break;
                        }
                    case UnitOfCurrency.JamaicanDollar :
                        {
                            strUOCText = "Jamaican Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Yen :
                        {
                            strUOCText = "Yen ";
                            break;
                        }
                    case UnitOfCurrency.Tenge :
                        {
                            strUOCText = "Tenge ";
                            break;
                        }
                    case UnitOfCurrency.JordanianDinar :
                        {
                            strUOCText = "Jordanian Dinar ";
                            break;
                        }
                    case UnitOfCurrency.KenyanShilling :
                        {
                            strUOCText = "Kenyan Shilling ";
                            break;
                        }
                    case UnitOfCurrency.NorthKoreanWon :
                        {
                            strUOCText = "North Korean Won ";
                            break;
                        }
                    case UnitOfCurrency.Won :
                        {
                            strUOCText = "Won ";
                            break;
                        }
                    case UnitOfCurrency.KuwaitiDinar :
                        {
                            strUOCText = "Kuwaiti Dinar ";
                            break;
                        }
                    case UnitOfCurrency.Som :
                        {
                            strUOCText = "Som ";
                            break;
                        }
                    case UnitOfCurrency.Kip :
                        {
                            strUOCText = "Kip ";
                            break;
                        }
                    case UnitOfCurrency.LebanesePound :
                        {
                            strUOCText = "Lebanese Pound ";
                            break;
                        }
                    case UnitOfCurrency.Loti :
                        {
                            strUOCText = "Loti ";
                            break;
                        }
                    case UnitOfCurrency.LatvianLats :
                        {
                            strUOCText = "Latvian Lats ";
                            break;
                        }
                    case UnitOfCurrency.LiberianDollar :
                        {
                            strUOCText = "Liberian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.LibyanDinar :
                        {
                            strUOCText = "Libyan Dinar ";
                            break;
                        }
                    case UnitOfCurrency.LithuanianLitas :
                        {
                            strUOCText = "Lithuanian Litas ";
                            break;
                        }
                    case UnitOfCurrency.Pataca :
                        {
                            strUOCText = "Pataca ";
                            break;
                        }
                    case UnitOfCurrency.Kwacha :
                        {
                            strUOCText = "Kwacha ";
                            break;
                        }
                    case UnitOfCurrency.MalaysianRinggit :
                        {
                            strUOCText = "Malaysian Ringgit ";
                            break;
                        }
                    case UnitOfCurrency.Rufiyaa :
                        {
                            strUOCText = "Rufiyaa ";
                            break;
                        }
                    case UnitOfCurrency.Ouguiya :
                        {
                            strUOCText = "Ouguiya ";
                            break;
                        }
                    case UnitOfCurrency.MauritiusRupee :
                        {
                            strUOCText = "Mauritius Rupee ";
                            break;
                        }
                    case UnitOfCurrency.MexicanPeso :
                        {
                            strUOCText = "Mexican Peso ";
                            break;
                        }
                    case UnitOfCurrency.Tugrik :
                        {
                            strUOCText = "Tugrik ";
                            break;
                        }
                    case UnitOfCurrency.MoldovanLeu :
                        {
                            strUOCText = "Moldovan Leu ";
                            break;
                        }
                    case UnitOfCurrency.MoroccanDirham :
                        {
                            strUOCText = "Moroccan Dirham ";
                            break;
                        }
                    case UnitOfCurrency.RialOmani :
                        {
                            strUOCText = "Rial Omani ";
                            break;
                        }
                    case UnitOfCurrency.NamibiaDollar :
                        {
                            strUOCText = "Namibia Dollar ";
                            break;
                        }
                    case UnitOfCurrency.NepaleseRupee :
                        {
                            strUOCText = "Nepalese Rupee ";
                            break;
                        }
                    case UnitOfCurrency.NetherlandsAntillianGuilder :
                        {
                            strUOCText = "Netherlands Antillian Guilder ";
                            break;
                        }
                    case UnitOfCurrency.ArubanGuilder :
                        {
                            strUOCText = "Aruban Guilder ";
                            break;
                        }
                    case UnitOfCurrency.Vatu :
                        {
                            strUOCText = "Vatu ";
                            break;
                        }
                    case UnitOfCurrency.NewZealandDollar :
                        {
                            strUOCText = "New Zealand Dollar ";
                            break;
                        }
                    case UnitOfCurrency.CordobaOro :
                        {
                            strUOCText = "Cordoba Oro ";
                            break;
                        }
                    case UnitOfCurrency.Naira :
                        {
                            strUOCText = "Naira ";
                            break;
                        }
                    case UnitOfCurrency.NorwegianKrone :
                        {
                            strUOCText = "Norwegian Krone ";
                            break;
                        }
                    case UnitOfCurrency.PakistanRupee :
                        {
                            strUOCText = "Pakistan Rupee ";
                            break;
                        }
                    case UnitOfCurrency.Balboa :
                        {
                            strUOCText = "Balboa ";
                            break;
                        }
                    case UnitOfCurrency.Kina :
                        {
                            strUOCText = "Kina ";
                            break;
                        }
                    case UnitOfCurrency.Guarani :
                        {
                            strUOCText = "Guarani ";
                            break;
                        }
                    case UnitOfCurrency.NuevoSol :
                        {
                            strUOCText = "Nuevo Sol ";
                            break;
                        }
                    case UnitOfCurrency.PhilippinePeso :
                        {
                            strUOCText = "Philippine Peso ";
                            break;
                        }
                    case UnitOfCurrency.GuineaBissauPeso :
                        {
                            strUOCText = "Guinea Bissau Peso ";
                            break;
                        }
                    case UnitOfCurrency.QatariRial :
                        {
                            strUOCText = "Qatari Rial ";
                            break;
                        }
                    case UnitOfCurrency.RussianRuble :
                        {
                            strUOCText = "Russian Ruble ";
                            break;
                        }
                    case UnitOfCurrency.RwandaFranc :
                        {
                            strUOCText = "Rwanda Franc ";
                            break;
                        }
                    case UnitOfCurrency.SaintHelenaPound :
                        {
                            strUOCText = "Saint Helena Pound ";
                            break;
                        }
                    case UnitOfCurrency.Dobra :
                        {
                            strUOCText = "Dobra ";
                            break;
                        }
                    case UnitOfCurrency.SaudiRiyal :
                        {
                            strUOCText = "Saudi Riyal ";
                            break;
                        }
                    case UnitOfCurrency.SeychellesRupee :
                        {
                            strUOCText = "Seychelles Rupee ";
                            break;
                        }
                    case UnitOfCurrency.Leone :
                        {
                            strUOCText = "Leone ";
                            break;
                        }
                    case UnitOfCurrency.SingaporeDollar :
                        {
                            strUOCText = "Singapore Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Dong :
                        {
                            strUOCText = "Dong ";
                            break;
                        }
                    case UnitOfCurrency.SomaliShilling :
                        {
                            strUOCText = "Somali Shilling ";
                            break;
                        }
                    case UnitOfCurrency.Rand :
                        {
                            strUOCText = "Rand ";
                            break;
                        }
                    case UnitOfCurrency.Lilangeni :
                        {
                            strUOCText = "Lilangeni ";
                            break;
                        }
                    case UnitOfCurrency.SwedishKrona :
                        {
                            strUOCText = "Swedish Krona ";
                            break;
                        }
                    case UnitOfCurrency.SwissFranc :
                        {
                            strUOCText = "Swiss Franc ";
                            break;
                        }
                    case UnitOfCurrency.SyrianPound :
                        {
                            strUOCText = "Syrian Pound ";
                            break;
                        }
                    case UnitOfCurrency.Baht :
                        {
                            strUOCText = "Baht ";
                            break;
                        }
                    case UnitOfCurrency.Paanga :
                        {
                            strUOCText = "Paanga ";
                            break;
                        }
                    case UnitOfCurrency.TrinidadTobagoDollar :
                        {
                            strUOCText = "Trinidad Tobago Dollar ";
                            break;
                        }
                    case UnitOfCurrency.UAEDirham :
                        {
                            strUOCText = "UAE Dirham ";
                            break;
                        }
                    case UnitOfCurrency.TunisianDinar :
                        {
                            strUOCText = "Tunisian Dinar ";
                            break;
                        }
                    case UnitOfCurrency.UgandaShilling :
                        {
                            strUOCText = "Uganda Shilling ";
                            break;
                        }
                    case UnitOfCurrency.Denar :
                        {
                            strUOCText = "Denar ";
                            break;
                        }
                    case UnitOfCurrency.EgyptianPound :
                        {
                            strUOCText = "Egyptian Pound ";
                            break;
                        }
                    case UnitOfCurrency.PoundSterling :
                        {
                            strUOCText = "Pound Sterling ";
                            break;
                        }
                    case UnitOfCurrency.TanzanianShilling :
                        {
                            strUOCText = "Tanzanian Shilling ";
                            break;
                        }
                    case UnitOfCurrency.USDollar :
                        {
                            strUOCText = "US Dollar ";
                            break;
                        }
                    case UnitOfCurrency.PesoUruguayo :
                        {
                            strUOCText = "Peso Uruguayo ";
                            break;
                        }
                    case UnitOfCurrency.UzbekistanSum :
                        {
                            strUOCText = "Uzbekistan Sum ";
                            break;
                        }
                    case UnitOfCurrency.Tala :
                        {
                            strUOCText = "Tala ";
                            break;
                        }
                    case UnitOfCurrency.YemeniRial :
                        {
                            strUOCText = "Yemeni Rial ";
                            break;
                        }
                    case UnitOfCurrency.ZambianKwacha :
                        {
                            strUOCText = "Zambian Kwacha ";
                            break;
                        }
                    case UnitOfCurrency.NewTaiwanDollar :
                        {
                            strUOCText = "New Taiwan Dollar ";
                            break;
                        }
                    case UnitOfCurrency.PesoConvertible :
                        {
                            strUOCText = "Peso Convertible ";
                            break;
                        }
                    case UnitOfCurrency.ZimbabweDollar :
                        {
                            strUOCText = "Zimbabwe Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Manat :
                        {
                            strUOCText = "Manat ";
                            break;
                        }
                    case UnitOfCurrency.Cedi :
                        {
                            strUOCText = "Cedi ";
                            break;
                        }
                    case UnitOfCurrency.BolivarFuerte :
                        {
                            strUOCText = "Bolivar Fuerte ";
                            break;
                        }
                    case UnitOfCurrency.SudanesePound :
                        {
                            strUOCText = "Sudanese Pound ";
                            break;
                        }
                    case UnitOfCurrency.UruguayPeso :
                        {
                            strUOCText = "Uruguay Peso ";
                            break;
                        }
                    case UnitOfCurrency.SerbianDinar :
                        {
                            strUOCText = "Serbian Dinar ";
                            break;
                        }
                    case UnitOfCurrency.Metical :
                        {
                            strUOCText = "Metical ";
                            break;
                        }
                    case UnitOfCurrency.AzerbaijanianManat :
                        {
                            strUOCText = "Azerbaijanian Manat ";
                            break;
                        }
                    case UnitOfCurrency.NewLeu :
                        {
                            strUOCText = "New Leu ";
                            break;
                        }
                    case UnitOfCurrency.WIREuro :
                        {
                            strUOCText = "WIR Euro ";
                            break;
                        }
                    case UnitOfCurrency.WIRFranc :
                        {
                            strUOCText = "WIR Franc ";
                            break;
                        }
                    case UnitOfCurrency.TurkishLira :
                        {
                            strUOCText = "Turkish Lira ";
                            break;
                        }
                    case UnitOfCurrency.CFAFrancBEAC :
                        {
                            strUOCText = "CFA Franc BEAC ";
                            break;
                        }
                    case UnitOfCurrency.EastCaribbeanDollar :
                        {
                            strUOCText = "East Caribbean Dollar ";
                            break;
                        }
                    case UnitOfCurrency.CFAFrancBCEAO :
                        {
                            strUOCText = "CFA Franc BCEAO ";
                            break;
                        }
                    case UnitOfCurrency.CFPFranc :
                        {
                            strUOCText = "CFP Franc ";
                            break;
                        }
                    case UnitOfCurrency.EuropeanCompositeUnit :
                        {
                            strUOCText = "European Composite Unit ";
                            break;
                        }
                    case UnitOfCurrency.EuropeanMonetaryUnit :
                        {
                            strUOCText = "European Monetary Unit ";
                            break;
                        }
                    case UnitOfCurrency.EuropeanUnitOfAccount9 :
                        {
                            strUOCText = "European Unit Of Account9 ";
                            break;
                        }
                    case UnitOfCurrency.EuropeanUnitOfAccount17 :
                        {
                            strUOCText = "European Unit Of Account17 ";
                            break;
                        }
                    case UnitOfCurrency.Gold :
                        {
                            strUOCText = "Gold ";
                            break;
                        }
                    case UnitOfCurrency.SDR :
                        {
                            strUOCText = "SDR ";
                            break;
                        }
                    case UnitOfCurrency.Silver :
                        {
                            strUOCText = "Silver ";
                            break;
                        }
                    case UnitOfCurrency.Platinum :
                        {
                            strUOCText = "Platinum ";
                            break;
                        }
                    case UnitOfCurrency.TestCurrency :
                        {
                            strUOCText = "Test Currency ";
                            break;
                        }
                    case UnitOfCurrency.Palladium :
                        {
                            strUOCText = "Palladium ";
                            break;
                        }
                    case UnitOfCurrency.SurinamDollar :
                        {
                            strUOCText = "Surinam Dollar ";
                            break;
                        }
                    case UnitOfCurrency.MalagasyAriary :
                        {
                            strUOCText = "Malagasy Ariary ";
                            break;
                        }
                    case UnitOfCurrency.UnidadDeValorReal :
                        {
                            strUOCText = "Unidad De Valor Real ";
                            break;
                        }
                    case UnitOfCurrency.Afghani  :
                        {
                            strUOCText = "Afghani  ";
                            break;
                        }
                    case UnitOfCurrency.Somoni :
                        {
                            strUOCText = "Somoni ";
                            break;
                        }
                    case UnitOfCurrency.Kwanza :
                        {
                            strUOCText = "Kwanza ";
                            break;
                        }
                    case UnitOfCurrency.BelarussianRuble :
                        {
                            strUOCText = "Belarussian Ruble ";
                            break;
                        }
                    case UnitOfCurrency.BulgarianLev :
                        {
                            strUOCText = "Bulgarian Lev ";
                            break;
                        }
                    case UnitOfCurrency.CongoleseFranc :
                        {
                            strUOCText = "Congolese Franc ";
                            break;
                        }
                    case UnitOfCurrency.ConvertibleMarks :
                        {
                            strUOCText = "Convertible Marks ";
                            break;
                        }
                    case UnitOfCurrency.Euro :
                        {
                            strUOCText = "Euro ";
                            break;
                        }
                    case UnitOfCurrency.MexicanUnidadDeInversion :
                        {
                            strUOCText = "Mexican Unidad De Inversion ";
                            break;
                        }
                    case UnitOfCurrency.Hryvnia :
                        {
                            strUOCText = "Hryvnia ";
                            break;
                        }
                    case UnitOfCurrency.Lari :
                        {
                            strUOCText = "Lari ";
                            break;
                        }
                    case UnitOfCurrency.Mvdol :
                        {
                            strUOCText = "Mvdol ";
                            break;
                        }
                    case UnitOfCurrency.Zloty :
                        {
                            strUOCText = "Zloty ";
                            break;
                        }
                    case UnitOfCurrency.BrazilianReal :
                        {
                            strUOCText = "Brazilian Real ";
                            break;
                        }
                    case UnitOfCurrency.UnidadesDeFomento :
                        {
                            strUOCText = "Unidades De Fomento ";
                            break;
                        }
                    case UnitOfCurrency.USDollarNextDay :
                        {
                            strUOCText = "US Dollar Next Day ";
                            break;
                        }
                    case UnitOfCurrency.USDollarSameDay :
                        {
                            strUOCText = "US Dollar Same Day ";
                            break;
                        }
                    case UnitOfCurrency.NoCurrency :
                        {
                            strUOCText = "No Currency ";
                            break;
                        }
                    case UnitOfCurrency.UndefinedCurrency:
                        {
                            strUOCText = "Undefined Currency ";
                            break;
                        }
                    default:
                        {
                            strUOCText = "";
                            break;
                        }
                }

                return strUOCText;
            }
        }

        /// <summary>
        /// Price trailing digit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte PriceTrailingDigit
        {
            get
            {
                return (byte)((m_byPriceTrailandTier & PRICE_TRAILING_DIGIT_MASK) >> PRICE_TRAILING_DIGIT_SHIFT);
            }
            set
            {
                m_byPriceTrailandTier = (byte)((value << PRICE_TRAILING_DIGIT_SHIFT) | (~PRICE_TRAILING_DIGIT_MASK & m_byPriceTrailandTier));
            }
        }

        /// <summary>
        /// Price tier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte PriceTier
        {
            get
            {
                return (byte)(m_byPriceTrailandTier & PRICE_TIER_MASK);
            }
            set
            {
                m_byPriceTrailandTier = (byte)((value & PRICE_TIER_MASK) | (~PRICE_TIER_MASK & m_byPriceTrailandTier));
            }
        }

        /// <summary>
        /// Price trailing digit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte NumberOfPriceTiers
        {
            get
            {
                return (byte)((m_byNbrTiersAndRegTier & NBR_PRICE_TIERS_MASK) >> NBR_PRICE_TIERS_SHIFT);
            }
            set
            {
                m_byNbrTiersAndRegTier = (byte)((value << NBR_PRICE_TIERS_SHIFT) | (~NBR_PRICE_TIERS_MASK & m_byNbrTiersAndRegTier));
            }
        }

        /// <summary>
        /// Price tier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte RegisterTier
        {
            get
            {
                return (byte)(m_byNbrTiersAndRegTier & REGISTER_TIER_MASK);
            }
            set
            {
                m_byNbrTiersAndRegTier = (byte)((value & REGISTER_TIER_MASK) | (~REGISTER_TIER_MASK & m_byNbrTiersAndRegTier));
            }

        }

        /// <summary>
        /// Time, in UTC 2000, at which the price signal becomes valid.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //  10/19/10 RCG 2.45.06 158622 Converting pricing time to display in local time

        public DateTime PriceSignalStartTime
        {
            get
            {
                DateTime dtStartTime;

                // A max or min value means the start time is right now.
                if (UInt32.MinValue == m_uiStartTime || UInt32.MaxValue == m_uiStartTime)
                {
                    dtStartTime = DateTime.UtcNow;
                }
                else
                {
                    dtStartTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    dtStartTime = dtStartTime.AddSeconds((double)m_uiStartTime);
                }

                return dtStartTime;
            }
        }

        /// <summary>
        /// Denotes the number of minutes the price is valid.  A value of 65535 
        /// denotes the price is valid until changed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public UInt16 Duration
        {
            get
            {
                return m_usDuration;
            }
            set
            {
                m_usDuration = value;
            }
        }

        
        /// <summary>
        /// The text representing the price of the commodity. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.05        Created.
        //
        public string CommodityPrice
        {
            get
            {
                return GeneratePriceString(Price);
            }
        }

        /// <summary>
        /// The text representing the generation price of the commodity. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.05        Created.
        //
        public string CommodityGenerationPrice
        {
            get
            {
                return GeneratePriceString(GenerationPrice);
            }
        }
       
        #endregion

        #region Internal Properties

        /// <summary>
        /// Bitfield representing the number of price tiers and associated register tier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte NumberTiersAndRegisterTier
        {
            get
            {
                return m_byNbrTiersAndRegTier;
            }
            set
            {
                m_byNbrTiersAndRegTier = value;
            }
        }

        /// <summary>
        /// Bitfield representing the price trailing digit and tier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte PriceTrailandTier
        {
            get
            {
                return m_byPriceTrailandTier;
            }
            set
            {
                m_byPriceTrailandTier = value;
            }
        }

        /// <summary>
        /// Seconds since 1/1/2000 at which the price signal becomes valid.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public UInt32 StartTime
        {
            get
            {
                return m_uiStartTime;
            }
            set
            {
                m_uiStartTime = value;
            }
        }

        /// <summary>
        /// A byte indicating the commodity that is being measured and its 
        /// format (binary or BCD).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte UnitOfMeasureByte
        {
            get
            {
                return m_byUnitOfMeasure;
            }
            set
            {
                m_byUnitOfMeasure = value;
            }
        }

        /// <summary>
        /// The length of the rate label.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte RateLabelLength
        {
            get
            {
                return m_byRateLabelLen;
            }
            set
            {
                m_byRateLabelLen = value;
            }
        }

        /// <summary>
        /// Identifying information concerning the local unit of currency used in the
        /// price field.  The value of the currency field should match the values defined 
        /// by ISO 4217.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public UInt16 Currency
        {
            get
            {
                return m_usCurrency;
            }
            set
            {
                m_usCurrency = value;
            }
        }

        /// <summary>
        /// Price of the commodity.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public UInt32 Price
        {
            get
            {
                return m_uiPrice;
            }
            set
            {
                m_uiPrice = value;
            }
        }

        /// <summary>
        /// Price of a commodity generated.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public UInt32 GenerationPrice
        {
            get
            {
                return m_uiGenPrice;
            }
            set
            {
                m_uiGenPrice = value;
            }
        }

        /// <summary>
        /// Ratio of the price denoted in the price field to the 
        /// "normal" price chosen by the commodity provider.  This 
        /// value should be scaled by a factor of 0.1 giving a range
        /// of ratios from 0.1 to 25.5.  A value of 0xFF indicates the 
        /// field is not used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte PriceRatio
        {
            get
            {
                return m_byPriceRatio;
            }
            set
            {
                m_byPriceRatio = value;
            }
        }

        /// <summary>
        /// Ratio of the price denoted in the generation price field to the 
        /// "normal" price chosen by the commodity provider. This 
        /// value should be scaled by a factor of 0.1 giving a range
        /// of ratios from 0.1 to 25.5.  A value of 0xFF indicates the 
        /// field is not used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte GenerationPriceRatio
        {
            get
            {
                return m_byGenPriceRatio;
            }
            set
            {
                m_byGenPriceRatio = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method gets text that describes the unit of measure given.
        /// </summary>
        /// <param name="UOM">The unit of measure.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        private static string GetUnitOfMeasureText(UnitOfMeasure UOM)
        {
            string strUOMText = "Undefined";

            switch (UOM)
            {
                case UnitOfMeasure.KiloWattsBinary:
                case UnitOfMeasure.KiloWattsBCD:
                    {
                        strUOMText = "kW & kWh";
                        break;
                    }
                case UnitOfMeasure.MetersCubedBinary:
                case UnitOfMeasure.MetersCubedBCD:
                    {
                        strUOMText = @"m³ & m³/h";
                        break;
                    }
                case UnitOfMeasure.FeetCubedBinary:
                case UnitOfMeasure.FeetCubedBCD:
                    {
                        strUOMText = @"ft³ & ft³/h";
                        break;
                    }
                case UnitOfMeasure.HundredCubicFeetBinary:
                case UnitOfMeasure.HundredCubicFeetBCD:
                    {
                        strUOMText = @"ccf & ccf/h";
                        break;
                    }
                case UnitOfMeasure.USGallonsBinary:
                case UnitOfMeasure.USGallonsBCD:
                    {
                        strUOMText = @"US gl & US gl/h";
                        break;
                    }
                case UnitOfMeasure.ImperialGallonsBinary:
                case UnitOfMeasure.ImperialGallonsBCD:
                    {
                        strUOMText = @"IMP gl & IMP gl/h";
                        break;
                    }
                case UnitOfMeasure.BritishThermalUnitsBinary:
                case UnitOfMeasure.BritishThermalUnitsBCD:
                    {
                        strUOMText = @"BTUs & BTU/h";
                        break;
                    }
                case UnitOfMeasure.LitersBinary:
                case UnitOfMeasure.LitersBCD:
                    {
                        strUOMText = @"Liters & l/h";
                        break;
                    }
                case UnitOfMeasure.KiloPascalsGaugeBinary:
                case UnitOfMeasure.KiloPascalsGaugeBCD:
                    {
                        strUOMText = @"Gauge (kPA)";
                        break;
                    }
                case UnitOfMeasure.KiloPascalsAbsoluteBinary:
                case UnitOfMeasure.KiloPascalsAbsoluteBCD:
                    {
                        strUOMText = @"Absolute (kPA)";
                        break;
                    }
                default:
                    {
                        strUOMText = "Undefined";
                        break;
                    }
            }

            return strUOMText;
        }

        /// <summary>
        /// This method gets text that describes the price.
        /// </summary>
        /// <param name="uiPrice">The price to convert.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/12/09 jrf 2.20.04        Created.
        //  06/02/11 RCG 2.51.03 173225 Adding case for all FF's which indicates it's not used

        private string GeneratePriceString(UInt32 uiPrice)
        {
            string strPrice = "Not Used";

            // If the value is all FF's it's not used. Otherwise determine the value.
            if (uiPrice != uint.MaxValue)
            {
                strPrice = uiPrice.ToString(CultureInfo.CurrentCulture);

                int iDecimalIndex = strPrice.Length - PriceTrailingDigit;
                int iZerosToAdd = PriceTrailingDigit - strPrice.Length;

                if (0 < PriceTrailingDigit)
                {
                    // The decimal should be placed at the very beginning of the price
                    if (0 == iDecimalIndex)
                    {
                        strPrice = "0." + strPrice;
                    }
                    // The decimal occurs within the body of the price
                    else if (0 < iDecimalIndex)
                    {
                        strPrice = strPrice.Insert(iDecimalIndex, ".");
                    }
                    // More zeros should be added to show the correct number of decimal places
                    // in the price.
                    else
                    {
                        strPrice = "0." + ZERO.ToString("D" + iZerosToAdd.ToString("D", CultureInfo.CurrentCulture), CultureInfo.CurrentCulture)
                            + strPrice;
                    }
                }

                //ex.             $           0.14                 kW & kWh    
                strPrice = CurrencySymbol + strPrice + " / " + UnitOfMeasureText;
            }

            return strPrice;
        }

        #endregion

        #region Members

        private byte m_byRateLabelLen;
        private UInt32 m_uiProviderID;
        private string m_strRateLabel;
        private UInt32 m_uiIssuerEventID;
        private byte m_byUnitOfMeasure;
        private UInt16 m_usCurrency;
        private byte m_byPriceTrailandTier;
        private byte m_byNbrTiersAndRegTier;
        private UInt32 m_uiStartTime;
        private UInt16 m_usDuration;
        private UInt32 m_uiPrice;
        private byte m_byPriceRatio;
        private UInt32 m_uiGenPrice;
        private byte m_byGenPriceRatio;

        #endregion
    }

    /// <summary>
    /// Class that represents a single AMI Tier label entry record.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  05/07/09 jrf 2.20.03        Created.
    //
    public class AMITierLabelEntryRcd
    {

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.

        public AMITierLabelEntryRcd()
        {
            m_byTierID = 0;
            m_byTierLabelLen = 0;
            m_strTierLabel = "";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The price tier sub-field.  Values are from 1 to 6 reflecting the least
        /// expensive tier (1) to the most expensive tier (6).  All other values are 
        /// reserved.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte TierID
        {
            get
            {
                return m_byTierID;
            }
            set
            {
                m_byTierID = value;
            }
        }

        /// <summary>
        /// The label for the tier as a string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public string TierLabel
        {
            get
            {
                return m_strTierLabel;
            }
            set
            {
                m_strTierLabel = value;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// The length of the tier label.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 jrf 2.20.03        Created.
        //
        public byte TierLabelLength
        {
            get
            {
                return m_byTierLabelLen;
            }
            set
            {
                m_byTierLabelLen = value;
            }
        }

        #endregion

        #region Members

        private byte m_byTierID;
        private byte m_byTierLabelLen;
        private string m_strTierLabel;

        #endregion
    }

    /// <summary>
    /// HAN Transmit Command for Table 2102
    /// </summary>
    internal class HANTransmitCommand
    {
        #region Constants

        private const uint HEADER_SIZE = 11;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the command using the specified binary writer
        /// </summary>
        /// <param name="writer">The writer to use to write the command</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public void WriteCommand(PSEMBinaryWriter writer)
        {
            writer.Write(m_ClientAddress);
            writer.Write(m_MessageType);
            writer.Write(m_DataSize);
            writer.Write(m_Data);
        }

        /// <summary>
        /// Gets the total size of the command
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public uint Size
        {
            get
            {
                return HEADER_SIZE + m_DataSize;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientAddress">The address of the client to transmit to.</param>
        /// <param name="messageType">The message type</param>
        /// <param name="dataSize">The size of the data to send.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        protected HANTransmitCommand(ulong clientAddress, byte messageType, ushort dataSize)
        {
            m_ClientAddress = clientAddress;
            m_MessageType = messageType;
            m_DataSize = dataSize;
            m_Data = new byte[m_DataSize];
        }


        #endregion

        #region Member Variables

        /// <summary>
        /// The Client's Address
        /// </summary>
        protected ulong m_ClientAddress;
        /// <summary>
        /// The Message Type
        /// </summary>
        protected byte m_MessageType;
        /// <summary>
        /// The size of the data to transmit
        /// </summary>
        protected ushort m_DataSize;
        /// <summary>
        /// The data to transmit
        /// </summary>
        protected byte[] m_Data;

        #endregion
    }

    /// <summary>
    /// DR/LC Command to be sent via Table 2102
    /// </summary>
    internal class HANScheduleDRLCCommand : HANTransmitCommand
    {
        #region Constants

        private const byte MESSAGE_TYPE = 0x10;
        private const ushort DATA_SIZE = 23;
        private const uint ISSUER_EVENT_ID = 1;
        private const byte UTIL_ENROLMENT_GROUP = 0;
        private const byte CRITICALITY_LEVEL = (byte)HANCriticalityLevel.One;
        private const byte COOLING_TEMP_OFFSET = 0xFF;
        private const byte HEATING_TEMP_OFFSET = 0xFF;
        private const short COOLING_TEMP_SET_POINT = unchecked((short)0x8000);
        private const short HEATING_TEMP_SET_POINT = unchecked((short)0x8000);
        private const sbyte AVG_LOAD_ADJUST_PERCENT = unchecked((sbyte)0x80);
        private const byte DUTY_CYCLE = 0xFF;
        private const byte EVENT_CONTROL = 0;
        private readonly DateTime REF_DATE = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientAddress">The address of the client to send the command to.</param>
        /// <param name="startTime">The start time of the DR/LC event</param>
        /// <param name="duration">The duration of the event</param>
        /// <param name="deviceClass">The device classes the event should apply to.</param>
        /// <param name="eventID">The event ID for the DRLC event</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public HANScheduleDRLCCommand(ulong clientAddress, DateTime startTime, ushort duration, DRLCDeviceClasses deviceClass, uint eventID)
            : base(clientAddress, MESSAGE_TYPE, DATA_SIZE)
        {
            m_StartTime = startTime;
            m_Duration = duration;
            m_DeviceClass = deviceClass;
            m_EventID = eventID;

            GenerateData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates the Transmit Data for the command
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        private void GenerateData()
        {
            MemoryStream Stream = new MemoryStream(m_Data);
            PSEMBinaryWriter Writer = new PSEMBinaryWriter(Stream);

            Writer.Write(m_EventID);
            Writer.Write((ushort)m_DeviceClass);
            Writer.Write(UTIL_ENROLMENT_GROUP);
            Writer.Write(Convert.ToUInt32((m_StartTime - REF_DATE).TotalSeconds));
            Writer.Write(m_Duration);
            Writer.Write(CRITICALITY_LEVEL);
            Writer.Write(COOLING_TEMP_OFFSET);
            Writer.Write(HEATING_TEMP_OFFSET);
            Writer.Write(COOLING_TEMP_SET_POINT);
            Writer.Write(HEATING_TEMP_SET_POINT);
            Writer.Write(AVG_LOAD_ADJUST_PERCENT);
            Writer.Write(DUTY_CYCLE);
            Writer.Write(EVENT_CONTROL);
        }

        #endregion

        #region Member Variables

        private DateTime m_StartTime;
        private ushort m_Duration;
        private DRLCDeviceClasses m_DeviceClass;
        private uint m_EventID;

        #endregion

    }

    /// <summary>
    /// DR/LC Cancel Command to be sent via Table 2102
    /// </summary>
    internal class HANCancelDRLCCommand : HANTransmitCommand
    {
        #region Constants

        private const byte MESSAGE_TYPE = 0x11;
        private const ushort DATA_SIZE = 23;
        private const uint ISSUER_EVENT_ID = 1;
        private const byte UTIL_ENROLMENT_GROUP = 0;
        private const byte CRITICALITY_LEVEL = (byte)HANCriticalityLevel.One;
        private const byte COOLING_TEMP_OFFSET = 0xFF;
        private const byte HEATING_TEMP_OFFSET = 0xFF;
        private const short COOLING_TEMP_SET_POINT = unchecked((short)0x8000);
        private const short HEATING_TEMP_SET_POINT = unchecked((short)0x8000);
        private const sbyte AVG_LOAD_ADJUST_PERCENT = unchecked((sbyte)0x80);
        private const byte DUTY_CYCLE = 0xFF;
        private const byte EVENT_CONTROL = 0;
        private readonly DateTime REF_DATE = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientAddress">The address of the client to send the command to.</param>
        /// <param name="EffectiveTime">The start time of the DR/LC Cancel event</param>
        /// <param name="deviceClass">The device classes the event should apply to.</param>
        /// <param name="eventID">The event ID for the DRLC Cancel event</param>
        /// <param name="CancelControl">Enter 0 to start at effective time, or 1 to start at random time</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/27/13 MP  2.40.43 N/A    Created

        public HANCancelDRLCCommand(ulong clientAddress, DateTime EffectiveTime, DRLCDeviceClasses deviceClass, uint eventID, byte CancelControl)
            : base(clientAddress, MESSAGE_TYPE, DATA_SIZE)
        {
            m_StartTime = EffectiveTime;
            m_DeviceClass = deviceClass;
            m_EventID = eventID;
            m_CancelControl = CancelControl;

            GenerateData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates the Transmit Data for the command
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/27/13 MP                 Created

        private void GenerateData()
        {
            MemoryStream Stream = new MemoryStream(m_Data);
            PSEMBinaryWriter Writer = new PSEMBinaryWriter(Stream);

            Writer.Write(m_EventID);
            Writer.Write((ushort)m_DeviceClass);
            Writer.Write(UTIL_ENROLMENT_GROUP);
            Writer.Write(Convert.ToUInt32((m_StartTime - REF_DATE).TotalSeconds));
            Writer.Write(CRITICALITY_LEVEL);
            Writer.Write(COOLING_TEMP_OFFSET);
            Writer.Write(HEATING_TEMP_OFFSET);
            Writer.Write(COOLING_TEMP_SET_POINT);
            Writer.Write(HEATING_TEMP_SET_POINT);
            Writer.Write(AVG_LOAD_ADJUST_PERCENT);
            Writer.Write(DUTY_CYCLE);
            Writer.Write(EVENT_CONTROL);
            Writer.Write(m_CancelControl);
        }

        #endregion

        #region Member Variables

        private DateTime m_StartTime;
        private DRLCDeviceClasses m_DeviceClass;
        private uint m_EventID;
        private byte m_CancelControl;

        #endregion

    }

    /// <summary>
    /// DR/LC Cancel All Events Command to be sent via Table 2102
    /// </summary>
    internal class HANCancelAllDRLCCommand : HANTransmitCommand
    {
        #region Constants

        private const byte MESSAGE_TYPE = 0x12;
        private const ushort DATA_SIZE = 23;
        private const uint ISSUER_EVENT_ID = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientAddress">The address of the client to send the command to.</param>
        /// <param name="CancelControl">Enter 0 to start at effective time, or 1 to start at random time</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/29/13 MP  2.40.43 N/A    Created

        public HANCancelAllDRLCCommand(ulong clientAddress, byte CancelControl)
            : base(clientAddress, MESSAGE_TYPE, DATA_SIZE)
        {
            m_CancelControl = CancelControl;

            GenerateData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates the Transmit Data for the command
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/29/13 MP 2.40.43 N/A    Created

        private void GenerateData()
        {
            MemoryStream Stream = new MemoryStream(m_Data);
            PSEMBinaryWriter Writer = new PSEMBinaryWriter(Stream);

            Writer.Write(m_CancelControl);
        }

        #endregion

        #region Member Variables

        private byte m_CancelControl;

        #endregion

    }

    /// <summary>
    /// Set Utility Enrollment Group Command to be sent via Table 2102
    /// </summary>
    internal class HANSetUtilityEnrollmentGroupCommand : HANTransmitCommand
    {
        #region Constants

        private const byte MESSAGE_TYPE = 0x21; // Attribute Write
        private const ushort DATA_SIZE = 8;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientAddress">The address of the client to update the Utility Enrollment Group</param>
        /// <param name="utilityEnrollmentGroup">The new Utility Enrollment Group</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/25/12 RCG 2.60.36        Created
        
        public HANSetUtilityEnrollmentGroupCommand(ulong clientAddress, byte utilityEnrollmentGroup)
            : base(clientAddress, MESSAGE_TYPE, DATA_SIZE)
        {
            m_Data = new byte[] { 0x09, 0x01, 0x01, 0x07, 0x00, 0x00, 0x20, utilityEnrollmentGroup};
        }

        #endregion
    }
}
