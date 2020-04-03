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
//                           Copyright © 2006 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;
#if (!WindowsCE)
using Itron.Common.C1219Tables.LandisGyr.Gateway;
#endif
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Datafiles.Properties;
using Itron.Metering.Device;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Progressable;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;
using Itron.Metering.DST;

namespace Itron.Metering.Datafiles
{

    /// <summary>
    /// The enumeration for days of the week that can be read from 
    /// table 52, the clock table.
    /// </summary>
    public enum DaysOfWeek
    {
        /// <summary>
        /// SUNDAY
        /// </summary>
        SUNDAY = 0,
        /// <summary>
        /// MONDAY
        /// </summary>
        MONDAY = 1,
        /// <summary>
        /// TUESDAY
        /// </summary>
        TUESDAY = 2,
        /// <summary>
        /// WEDNESDAY
        /// </summary>
        WEDNESDAY = 3,
        /// <summary>
        /// THURSDAY
        /// </summary>
        THURSDAY = 4,
        /// <summary>
        /// FRIDAY
        /// </summary>
        FRIDAY = 5,
        /// <summary>
        /// SATURDAY
        /// </summary>
        SATURDAY = 6,
        /// <summary>
        /// UNASSIGNED
        /// </summary>
        UNASSIGNED = 7,
        /// <summary>
        /// UNREAD
        /// </summary>
        UNREAD = 8,
    }

    /// <summary>
    /// Device types.
    /// </summary>
    public enum EDLDeviceTypes
    {
        /// <summary>
        /// Device is an OpenWay Centron
        /// </summary>
        OpenWayCentron,
        /// <summary>
        /// Device is a Basic Polyphase meter.
        /// </summary>
        OpenWayCentronBasicPoly,
        /// <summary>
        /// Device is an Advanced Polyphase meter.
        /// </summary>
        OpenWayCentronAdvPoly,
        /// <summary>
        /// Device is a transparent device.
        /// </summary>
        TransparentDevice,
        /// <summary>
        /// Device is an M2 Gateway device.
        /// </summary>
        M2GatewayDevice,
        /// <summary>
        /// Device is an ITRD OpenWay Centron meter
        /// </summary>
        OpenWayCentronITRD,
        /// <summary>
        /// Device is an ITRE OpenWay Basic Poly meter
        /// </summary>
        OpenWayCentronBasicPolyITRE,
        /// <summary>
        /// Device is an ITRF OpenWay Advanced Poly meter
        /// </summary>
        OpenWayCentronAdvPolyITRF,
        /// <summary>
        /// Device is an ITRK OpenWay Poly meter
        /// </summary>
        OpenWayCentronPolyITRK,
        /// <summary>
        /// Device is an ITRH device, I-210 or kV2c
        /// </summary>
        ICSGatewayDevice,
    }

    /// <summary>
    /// Enumerates the possible return codes from a request to create a 25 year TOU export
    /// file from an EDL configuration file.
    /// </summary>
    public enum CreateTOUExportResult : byte
    {
        /// <summary>
        /// Success, The TOU export file was successfully created.
        /// </summary>
        [EnumDescription("Success")]
        Success,
        /// <summary>
        /// Invalid Path, The TOU export file path does not exist.
        /// </summary>
        [EnumDescription("Invalid Path, The TOU export file path does not exist.")]
        InvalidTOUPath,
        /// <summary>
        /// File Not Found, The specified EDL file was not found.
        /// </summary>
        [EnumDescription("File Not Found, The specified EDL file was not found.")]
        FileNotFound = 1,
        /// <summary>
        /// Invalid EDL File, The specified file was not a valid EDL file.
        /// </summary>
        [EnumDescription("Invalid EDL File, The specified file was not a valid EDL file.")]
        InvalidEDLFile,
        /// <summary>
        /// No TOU Data, The specified file does not contain a 25 year TOU schedule.
        /// </summary>
        [EnumDescription("No TOU Data, The specified file does not contain a 25 year TOU schedule.")]
        NoTOUData = 3,
        /// <summary>
        /// Error Reading TOU, There was an error reading the 25 year TOU schedule from the EDL file.
        /// </summary>
        [EnumDescription("Error Reading TOU, There was an error reading the 25 year TOU schedule from the EDL file.")]
        ErrorReadingTOU,
        /// <summary>
        /// Error Writing TOU, There was an error writing the 25 year TOU schedule to the export file.
        /// </summary>
        [EnumDescription("Error Writing TOU, There was an error writing the 25 year TOU schedule to the export file.")]
        ErrorWritingTOU,
        /// <summary>
        /// Error, A unspecified error has occurred.
        /// </summary>
        [EnumDescription("Error, A unspecified error has occurred.")]
        Error,
    }

    /// <summary>
    /// This class provides a representation of an OpenWay meter EDL file.  EDL stands for End Device
    /// Language and the file is essentially an XML file that defines the current state of
    /// a meter.  The EDL format is defined by the ANSI C12.19 format.  
    /// 
    /// This class does NOT provide a complete representation of a EDL file.  It provides
    /// read-only access to some of the data contained in the file, and it is intended
    /// to be used as means to validate meter operation.  The class should be extended as 
    /// needed to expose different data types.
    /// </summary>
    public class EDLFile : IItronDeviceConfiguration, IItronDeviceStatus, IProgressable
    {
        #region Public Events

        /// <summary>
        /// Event used to display a progress bar
        /// </summary>
        public virtual event ShowProgressEventHandler ShowProgressEvent;

        /// <summary>
        /// Event used to cause a progress bar to perform a step
        /// </summary>
        public virtual event StepProgressEventHandler StepProgressEvent;

        /// <summary>
        /// Event used to hide a progress bar
        /// </summary>
        public virtual event HideProgressEventHandler HideProgressEvent;

        #endregion

        #region Constants

        private const ushort FIRST_MFG_STATISTIC = 2048;

        /// <summary>Mask for the SEC Energy LID base</summary>
        protected const uint SEC_ENERGY_LID_BASE = 0x14000080;

        // Load Control Reconnect
        private const int LOAD_CONTROL_RECONNECT = 0x80;

        // Used for calculating multiplier.
        private const int KILO = 1000;

        /// <summary>
        /// The list of the TOU Rate modifiers for LIDs
        /// </summary>
        private readonly uint[] TOU_RATES = {(uint)DefinedLIDs.TOU_Data.RATE_A, (uint)DefinedLIDs.TOU_Data.RATE_B, 
                                             (uint)DefinedLIDs.TOU_Data.RATE_C, (uint)DefinedLIDs.TOU_Data.RATE_D, 
                                             (uint)DefinedLIDs.TOU_Data.RATE_E, (uint)DefinedLIDs.TOU_Data.RATE_F,
                                             (uint)DefinedLIDs.TOU_Data.RATE_G };
        /// <summary>
        /// Constant for Crystal sync
        /// </summary>
        protected const string CRYSTAL_SYNC = "Crystal Synchronization";
        /// <summary>
        /// Constant for Line sync
        /// </summary>
        protected const string LINE_SYNC = "Line Synchronization";

        /// <summary>Mask for the Daily Self Read Hour</summary>
        protected const byte DSRT_HR_MASK = 0x1F;
        /// <summary>Mask for the Daily Self Read Minutes</summary>
        protected const byte DSRT_MIN_MASK = 0xE0;
        /// <summary>Tariff ID max length</summary>
        protected const int TARRIF_ID_MAX_LENGTH = 8;
        /// <summary>the Daily Self Read Hour disable flag</summary>
        protected const byte DSRT_DISABLED = 0;
        /// <summary>the Daily Self Read Minutes Midnight hour</summary>
        protected const byte DSRT_MIDNIGHT = 24;
        /// <summary>
        /// The season entry index.
        /// </summary>
        protected const int SEASON_INDEX = 0;
        /// <summary>
        /// The day type entry index.
        /// </summary>
        protected const int DAY_TYPE_INDEX = 1;
        /// <summary>
        /// The day type event entry index.
        /// </summary>
        protected const int DAY_TYPE_EVENT_INDEX = 2;

        /// <summary>
        /// Device class for MAX Image single phase meters.
        /// </summary>
        protected const string ITRA_DEVICE_CLASS = "ITRA";
        /// <summary>
        /// Device class for MAX Image basic polyphase meters.
        /// </summary>
        protected const string ITRB_DEVICE_CLASS = "ITRB";
        /// <summary>
        /// Device class for MAX Image advanced polyphase meters.
        /// </summary>
        protected const string ITRC_DEVICE_CLASS = "ITRC";
        /// <summary>
        /// Device class string for M2 Gateway devices
        /// </summary>
        protected const string LIS1_DEVICE_CLASS = "LIS1";
        /// <summary>
        /// Lower case device class string for M2 Gateway devices
        /// </summary>
        protected const string lis1_DEVICE_CLASS = "lis1";

        /// <summary>
        /// Device class node in the EDL file
        /// </summary>
        protected const string GEN_CFG_TBL_DEV_CLASS = "DEVICE_CLASS";
        /// <summary>
        /// ASCII representation of the "LIS1" device class
        /// </summary>
        protected const string DEV_CLASS_ASCII_CODE_LIS1 = "4C495331";
        /// <summary>
        /// ASCII representation of the "lis1" device class
        /// </summary>
        protected const string DEV_CLASS_ASCII_CODE_lis1 = "6C697331";
        /// <summary>
        /// ASCII representation of the "ITRH" device class
        /// </summary>
        protected const string DEV_CLASS_ASCII_CODE_ITRH = "49545248";
        /// <summary>
        /// ASCII representation of the "ITRU" device class
        /// </summary>
        protected const string DEV_CLASS_ASCII_CODE_ITRU = "49545255";
        /// <summary>
        /// ASCII representation of the "ITRV" device class
        /// </summary>
        protected const string DEV_CLASS_ASCII_CODE_ITRV = "49545256";
        /// <summary>
        /// Hardware version of the PrismLite meter
        /// </summary>
        protected const float PRISM_LITE_REVISION = 128.0f;
        /// <summary>
        /// Prmary password index.
        /// </summary>
        protected const int PRIMARY_SEC_CODE_INDEX = 3;
        /// <summary>
        /// Quaternary password index.
        /// </summary>
        protected const int LIMITED_SEC_CODE_INDEX = 2;
        /// <summary>
        /// Secondary password index.
        /// </summary>
        protected const int SECONDARY_SEC_CODE_INDEX = 1;
        /// <summary>
        /// Tertiary password index.
        /// </summary>
        protected const int TERTIARY_SEC_CODE_INDEX = 0;

        private const int TABLE2260_HAN_EVENTS_OFFSET = 0x90;
        private const ushort TABLE2260_HAN_EVENTS_SIZE = 34;

        private const ushort CTE_CONFIG_TBL_OFFSET = 315;
        private const ushort CTE_CONFIG_TBL_SIZE = 6;

        private const ushort DATASET_CONFIGURATION_TBL_OFFSET = 330;
        private const ushort DATASET_CONFIGURATION_TBL_SIZE = 153;

        private const int NUMBER_EXT_SELF_READ_QUANTITIES = 16;

        private const UInt16 EXCEPTION_REPORT_SELECTOR_MASK = 0x1000;

        private const ushort TWENTY_FIVE_YEAR_TOU_TABLE_NUMBER = 2437;

        private const uint CRC_POLYNOMIAL_SEED = 0x04C11DB7;

        private const string TOU_ENABLED = "Enabled";

        private const byte ARITHMETIC = 0;
        private const byte VECTORIAL = 1;
        private const byte LAG = 2;
        private const short VA = 1;
        private const short VAR = 2;

        private const string strNullPassword = "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";

        private const byte LP_CLOSURE_STATUS_DST_MASK = 0x02;

        private const string UNASSIGNED = "Unassigned";
        private const string BLANK = "";

        #endregion Constants

        #region Definitions

        /// <summary>
        /// Enumeration for the values that describe how the clock is synchronized.
        /// </summary>
        public enum ClockSynchronization : int
        {
            /// <summary>
            /// Crystal synchronization
            /// </summary>
            Crystal = 0,
            /// <summary>
            /// Line synchronization
            /// </summary>
            Line = 1,
        }

        /// <summary>
        /// Enumeration to described how demand resets are scheduled.
        /// </summary>
        public enum DemandResetSchedule : int
        {
            /// <summary>
            /// Scheduled demand resets are disabled.
            /// </summary>
            Disabled = 0,
            /// <summary>
            /// Demand is reset N days from most recent demand reset.
            /// </summary>
            NDaysFromMostRecentDemandReset = 1,
            /// <summary>
            /// Demand is reset on the Nth day of the month.
            /// </summary>
            NthDayOfMonth = 2,
            /// <summary>
            /// Demand is reset on the Nth day before the end of the month.
            /// </summary>
            NthDayBeforeEndOfMonth = 3,
            /// <summary>
            /// Demand is reset according to a billing schedule.
            /// </summary>
            FollowsBillingSchedule = 4,
        }

        /// <summary>Defintion for the Daily Self Read Minutes</summary>
        private enum DSRTMinutes : byte
        {
            /// <summary>0 Minutes</summary>
            MIN_0 = 0x00,
            /// <summary>10 Minutes</summary>
            MIN_10 = 0x20,
            /// <summary>15 Minutes</summary>
            MIN_15 = 0x40,
            /// <summary>20 Minutes</summary>
            MIN_20 = 0x60,
            /// <summary>30 Minutes</summary>
            MIN_30 = 0x80,
            /// <summary>40 Minutes</summary>
            MIN_40 = 0xA0,
            /// <summary>45 Minutes</summary>
            MIN_45 = 0xC0,
            /// <summary>50 Minutes</summary>
            MIN_50 = 0xE0,
        }

        /// <summary>Defintion for specifiying the type of display list.</summary>
        public enum DisplayListType : byte
        {
            /// <summary>Normal Display List</summary>
            Normal = 0,
            /// <summary>Alternate Display List</summary>
            Alternate = 1,
            /// <summary>Test Display List</summary>
            Test = 2,
        }

        /// <summary>
        /// Definition for non fatal errors.
        /// </summary>
        public enum NonFatalErrors
        {
            /// <summary>
            /// Low battery
            /// </summary>
            LowBattery = 0,
            /// <summary>
            /// Loss of phase
            /// </summary>
            LossOfPhase = 1,
            /// <summary>
            /// Clock/TOU error
            /// </summary>
            ClockTOU = 2,
            /// <summary>
            /// Reverse power flow
            /// </summary>
            ReversePowerFlow = 3,
            /// <summary>
            /// Load Profile error
            /// </summary>
            MassMemory = 4,
            /// <summary>
            /// Register full-scale
            /// </summary>
            RegisterFullScale = 5,
            /// <summary>
            /// Sitescan error
            /// </summary>
            Sitescan = 6,
        }

        /// <summary>
        /// Definition for the options available for displaying an error.
        /// </summary>
        public enum ErrorDisplayOptions
        {
            /// <summary>
            /// 
            /// </summary>
            Ignore = 0,
            /// <summary>
            /// 
            /// </summary>
            Scroll = 1,
            /// <summary>
            ///     
            /// </summary>
            Lock = 2,
        }

        /// <summary>
        /// Definition for the demand reset options available when the season changes.
        /// </summary>
        public enum SeasonChangeOptions : byte
        {
            /// <summary>
            /// Delay season change until the demand is reset.
            /// </summary>
            DelaySeasonChangeUntilDemandReset = 0,
            /// <summary>
            /// Demand reset at the season change.
            /// </summary>
            DemandResetAtSeasonChange = 1,
            /// <summary>
            /// Season change without the demand reset.
            /// </summary>
            SeasonChangeWithoutDemandReset = 2,
        }

        /// <summary>
        /// The secondary quantity selection options.
        /// </summary>
        public enum SecondaryQuantitySelection : byte
        {
            /// <summary>
            /// No Secondary Quantity Selected
            /// </summary>
            [EnumDescription("N/A")]
            None = 0,
            /// <summary>
            /// VA Arithmetic
            /// </summary>
            [EnumDescription("VA Arithmetic")]
            VAArithmetic = 1,
            /// <summary>
            /// VA Vectorial
            /// </summary>
            [EnumDescription("VA Vectorial")]
            VAVectorial = 2,
            /// <summary>
            /// var
            /// </summary>
            [EnumDescription("VAR")]
            Var = 3,
            /// <summary>
            /// Unknown
            /// </summary>
            [EnumDescription("Unknown")]
            Unknown = 255,
        }

        /// <summary>
        /// Selectable options for Security Provider 
        /// </summary>
        public enum SecurityProviderSelection : byte
        {
            /// <summary>
            /// Standard C12.22 Security
            /// </summary>
            Standard = 2,
            /// <summary>
            /// Enhanced Itron Security
            /// </summary>
            Enhanced = 1,
        }

        /// <summary>
        /// Selectable options for Demand Calculation
        /// </summary>
        public enum DemandCalculationMethodSelection : int
        {
            /// <summary>
            /// Block demand calculation
            /// </summary>
            Block = 0,
            /// <summary>
            /// Sliding demand calculation
            /// </summary>
            Sliding = 1,
        }

        #endregion Definitions

        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/08/10 MMD				   Created
        public EDLFile()
        {

        }

        /// <summary>
        /// Constructor to make an EDLFile object
        /// </summary>
        /// <param name="FileName">full path to the EDL file</param>
        // Revision History	
        // MM/DD/YY who Version  ID Number   Description
        // -------- --- -------  -- ------   ---------------------------------------
        // 10/30/06 RDB				         Created
        // 07/01/13 AF  2.80.45  TR 7640     Added the ICS event dictionary
        // 12/29/15 AF  4.23.01  WR 559019   Set the time format for the event dictionary so that we
        //                                   can display the time changed event argument
        // 01/12/15 jrf 4.50.223 WR 645125   Modified to pull time format from property.
        // 08/05/16 MP  4.70.11  WR 674048   Set timeformat for other dictionaries
        public EDLFile(string FileName)
        {
            //set these up first. Time format property uses one.
            InitializeInstanceVariables(); 

            m_strEDLFile = FileName;
            LoadFile();
            m_EventDictionary = new CENTRON_AMI_EventDictionary();

            m_EventDictionary.TimeFormat = TimeFormat;
            
            m_GWEventDictionary = new M2_Gateway_EventDictionary();
            m_GWEventDictionary.TimeFormat = TimeFormat;

            m_ICSEventDictionary = new ICS_Gateway_EventDictionary();
            m_ICSEventDictionary.TimeFormat = TimeFormat;
            
        }

        /// <summary>
        /// This method separates the program or TOU file name from the version number
        /// from an EDL path name.  If the given path name does not have a version 
        /// number embedded in it, an empty string will be returned for the version
        /// </summary>
        /// <param name="strFullPathName" type="string">
        /// The full path name of the EDL file with or without a file extension
        /// </param>
        /// <param name="strName" type="string">
        /// The user given name of the program or TOU file
        /// </param>
        /// <param name="strVersion" type="string">
        /// The version number assigned to the program by the collection engine
        /// </param>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ -------------------------------------------
        ///  02/20/08 MAH  1.0           Created
        ///  06/16/08 MAH  1.50.36 00116202 Changed IndexOf to LastIndexOf
        /// </remarks>
        public static void ParseFileName(String strFullPathName, out String strName, out String strVersion)
        {
            String strFileName = Path.GetFileNameWithoutExtension(strFullPathName);
            int nVersionOffset = strFileName.LastIndexOf("_v", StringComparison.OrdinalIgnoreCase);

            // Assume that there is no version number on the file as a default
            strName = strFileName;
            strVersion = "";

            if (0 < nVersionOffset)
            {
                // Verify that the version string really is a version and that we are not using part of the
                // file name as the version identifier.
                int intVersion;

                if (Int32.TryParse(strFileName.Substring(nVersionOffset + 2), out intVersion))
                {
                    strName = strFileName.Substring(0, nVersionOffset);
                    strVersion = strFileName.Substring(nVersionOffset + 2);
                }
            }
        }

        /// <summary>
        /// Returns true if the file is an EDL file and false otherwise
        /// </summary>
        /// <param name="FileName">path of the file</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/14/06 RDB				   Created
        // 03/30/10 RCG 2.40.30        Modified to support Signature Verification
        // 09/06/12 jrf 2.70.15 TQ6710 Refactored out the code to create the XmlDocument and 
        //                             the EDL determination logic into their own methods.
        //
        public static bool IsEDLFile(string FileName)
        {
            bool bIsEDLFile = false;
            XmlDocument Document = CreateXMLDocument(FileName);

            if (null != Document)
            {
                try
                {
                    bIsEDLFile = IsEDLFile(Document);
                }
                catch (Exception)
                {
                    bIsEDLFile = false;
                }
            }

            return bIsEDLFile;
        }//IsEDLFile

        /// <summary>
        /// Retrieves the device class from an EDL file.
        /// </summary>
        /// <param name="FileName">Path of the file.</param>
        /// <returns>The device class from the EDL file or an empty string for non-EDL files.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/12 jrf 2.70.15 TQ6710 Created.
        public static string GetDeviceClass(string FileName)
        {
            XmlDocument Document = CreateXMLDocument(FileName);
            string strDeviceClass = "";

            try
            {
                if (null != Document && IsEDLFile(Document))
                {
                    XmlNodeList lstDeviceClassElements = Document.GetElementsByTagName("DEVICE_CLASS");
                    XmlAttribute DeviceClassAttribute = null;

                    //We want the first one
                    DeviceClassAttribute = lstDeviceClassElements[0].Attributes["binary"];

                    if (null != DeviceClassAttribute)
                    {
                        //ex. Need to convert "49545231" to "ITR1"
                        //There has to be a better way, but i am unaware of it.

                        //step 1. Divide string into char array. ex."49545231"  goes to { '4', '9', '5', '4', '5', '2', '3', '1' }
                        char[] dcchars = DeviceClassAttribute.Value.ToCharArray();

                        //step 2.  Iterate through every two characters.  ex. '4' & '9' then '5' & '4' then '5' and '2' then '3' & '1' 
                        for (int i = 0; i < dcchars.Length; i = i + 2)
                        {
                            if (i + 1 < dcchars.Length)
                            {
                                //Take the two characters and convert them from characters representing 
                                //hex digits to string representing hex ascii character to byte version of 
                                //the hex ascii character to char
                                //ex. "49" needs to be converted to 'I'
                                //                  char    <---     Byte  <--- Numeric Hex String Character  
                                //                  'I'     <---     73    <--- "49"
                                char chChar = Convert.ToChar(Convert.ToByte(new string(dcchars, i, 2), 16));

                                strDeviceClass += chChar.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //just return empty string
                strDeviceClass = "";
            }

            return strDeviceClass;
        }//GetDeviceClass

        /// <summary>
        /// Returns EDlFile object based on device class
        /// </summary>
        /// <param name="FileName">path of the file</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/8/10 MMD				   Created
        // 07/03/13 AF  2.80.45 TR7640 Added support for the I-210 and kV2c ICS modules
        //
        public static EDLFile CreateObject(string FileName)
        {

            EDLFile edl = null;
            if ((DetermineDeviceType(FileName) == EDLDeviceTypes.OpenWayCentron) ||
                (DetermineDeviceType(FileName) == EDLDeviceTypes.ICSGatewayDevice))
            {
                edl = new EDLFile(FileName);
            }
#if (!WindowsCE)
            else if (DetermineDeviceType(FileName) == EDLDeviceTypes.M2GatewayDevice)
            {
                edl = new EDLFile(FileName);
            }

#endif
            return edl;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            SaveFile();
        }

        /// <summary>
        /// This method imports passwords into the file.
        /// </summary>
        /// <param name="strPrimaryPassword">The primary password</param>
        /// <param name="strSecondaryPassword">The secondary password</param>
        /// <param name="TertiaryPassword">The tertiary password</param>
        /// <param name="strQuaternaryPassword">The quaternary password</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void ImportPasswords(string strPrimaryPassword, string strSecondaryPassword,
            string TertiaryPassword, string strQuaternaryPassword)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method checks to see if Passwords have been set in the configuration file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/17/16 CFB 4.50.231  	   Created
        //
        public List<bool> ArePasswordsConfigured()
        {
            List<bool> lstboolPasswords = new List<bool>();
            object objPassword;
            byte[] byPassword = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            string strPassword;

            for (int i = 3; i >= 0; i--)
            {
                int[] IndexArray = { i };
                objPassword = null;
                m_CenTables.GetValue(StdTableEnum.STDTBL42_PASSWORD, IndexArray, out objPassword);
                byPassword = (byte[])objPassword;
                strPassword = Encoding.ASCII.GetString(byPassword);


                if (strPassword == strNullPassword)
                {
                    lstboolPasswords.Add(false);
                }
                else
                {
                    lstboolPasswords.Add(true);
                }
            }

            return lstboolPasswords;
        }

        /// <summary>
        /// This method imports the TOU configuration into the file.
        /// </summary>
        /// <param name="strTOUFileName">The TOU schedule file name.</param>
        /// <param name="SeasonChgOption">The season change/demand reset option</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void ImportTOU(string strTOUFileName, SeasonChangeOptions SeasonChgOption)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method imports DST schedule from the replica files into the file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void ImportDST()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method clears DST schedule from the file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        // 
        public virtual void ClearDST()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method clears the TOU configuration from the file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void ClearTOU()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method enables/disables the history log event in the configuration.
        /// </summary>
        /// <param name="iEvent"></param>
        /// <param name="blnEnabled"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetEvent(int iEvent, bool blnEnabled)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets non fatal error display options.
        /// </summary>
        /// <param name="NonFatalErr">The non-fatal error being referred to.</param>
        /// <param name="ErrorDispOpt">How to display the error.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetNonFatalErrorDisplayOptions(NonFatalErrors NonFatalErr, ErrorDisplayOptions ErrorDispOpt)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method removes an item from the display list.
        /// </summary>
        /// <param name="DisplayList">Specifies which display list to remove the item from.</param>
        /// <param name="iIndex">The list specific index of the item to remove.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void RemoveDisplayItem(DisplayListType DisplayList, int iIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method adds an item to the display list.
        /// </summary>
        /// <param name="DisplayList">Specifies which display list to remove the item from.</param>
        /// <param name="NewDisplayItem">The display item to be added.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void AddDisplayItem(DisplayListType DisplayList, ANSIDisplayItem NewDisplayItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method updates a particular display item.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="DisplayItem">The item to update</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public void SetDisplayItem(DisplayListType DisplayList, int iListSpecificIndex, ANSIDisplayItem DisplayItem)
        {
            SetDisplayItemLID(DisplayList, iListSpecificIndex, DisplayItem.DisplayLID);
            SetDisplayItemID(DisplayList, iListSpecificIndex, DisplayItem.DisplayID.Trim());
            SetDisplayType(DisplayList, iListSpecificIndex, (byte)DisplayItem.Type);
            SetDisplayUnits(DisplayList, iListSpecificIndex, (byte)DisplayItem.Unit);
            SetDisplayItemDemandType(DisplayList, iListSpecificIndex, 0);
            SetDisplayItemRate(DisplayList, iListSpecificIndex, 0);
            SetDisplayItemRound(DisplayList, iListSpecificIndex, false);
            SetDisplayDecimalDigits(DisplayList, iListSpecificIndex, DisplayItem.DecimalDigits);
            SetDisplayTotalDigits(DisplayList, iListSpecificIndex, DisplayItem.TotalDigits);
        }

        /// <summary>
        /// This method sets the LID for a particular display item.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="lidDisplayItem">The LID to of the display item to update</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayItemLID(DisplayListType DisplayList, int iListSpecificIndex, LID lidDisplayItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets the display item's demand type.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="bytDemandType">The demand type of the display item to update</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayItemDemandType(DisplayListType DisplayList, int iListSpecificIndex, byte bytDemandType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets the display item's rate.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="bytRate">the rate of the display item to update</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayItemRate(DisplayListType DisplayList, int iListSpecificIndex, byte bytRate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets whether the display items will round values.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="blnRound">Whether or not to round.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayItemRound(DisplayListType DisplayList, int iListSpecificIndex, bool blnRound)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method gets the generic index of the display item.
        /// </summary>
        /// <param name="DisplayList">The list of the display item</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <returns>The generic index of the display item.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        protected virtual int GetDisplayItemIndex(DisplayListType DisplayList, int iListSpecificIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets the display item's ID.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="strID">The ID of the display item</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayItemID(DisplayListType DisplayList, int iListSpecificIndex, string strID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets the display item's decimal digits.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="iDecimalDigits">The display item's decimal digits</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayDecimalDigits(DisplayListType DisplayList, int iListSpecificIndex, int iDecimalDigits)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets the display item's total digits.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="iTotalDigits">The display item's total digits</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayTotalDigits(DisplayListType DisplayList, int iListSpecificIndex, int iTotalDigits)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets the display item's units.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="bytUnits">The display item's units.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayUnits(DisplayListType DisplayList, int iListSpecificIndex, byte bytUnits)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method sets the display item's type.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="bytType">The display item's type.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public virtual void SetDisplayType(DisplayListType DisplayList, int iListSpecificIndex, byte bytType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method determines if a given ICS status alarm is set.
        /// </summary>
        /// <param name="Alarm">The alarm to check.</param>
        /// <returns>Whether or not the alarm is set.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/13 jrf 2.80.20 TQ8278 Created
        //
        public bool IsICSStatusAlarmSet(ICSStatusAlarms Alarm)
        {
            bool blnResult = false;
            try
            {
                if (Table2512 != null)
                {
                    blnResult = Table2512.IsAlarmSet(Alarm);
                }
            }
            catch
            {
                blnResult = false;
            }

            return blnResult;
        }

        /// <summary>This method generates a 25 year TOU export binary data file for use in 
        /// reconfiguring TOU over a ChoiceConnect network. 
        /// </summary> 
        /// <param name="strTOUExportFileName">The full path of the binary TOU data file to 
        /// create.</param> 
        /// <returns>The result of generating the file.</returns> 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/14 jrf 3.50.19 TQ9629 Created
        //  01/15/14 jrf 3.50.26 TQ9629 Modified to write table number in file MSB to LSB.
        //  04/25/14 jrf 3.50.84 WR 490044 Modified to call method for creating the 25 year TOU calendar data
        //                                 from the config file, since the Supports25YearTOU property no longer
        //                                 does this.
        //
        public CreateTOUExportResult Generate25YearTOUExportFile(string strTOUExportFileName)
        {
            CreateTOUExportResult Result = CreateTOUExportResult.Success;
            FileStream TOUExportStream = null;
            BinaryWriter TOUExportWriter = null;

            try
            {
                FileInfo ExportFileInfo = new FileInfo(strTOUExportFileName);
                uint uiCRC;
                TableData[] PSEMData = null;

                if (false == ExportFileInfo.Directory.Exists)
                {
                    Result = CreateTOUExportResult.InvalidTOUPath;
                }
                else if (false == IsProgramFile)
                {
                    Result = CreateTOUExportResult.InvalidEDLFile;
                }
                else if (false == Supports25YearTOU)
                {
                    //No TOU schedule data was present
                    Result = CreateTOUExportResult.NoTOUData;
                }
                else
                {
                    m_CenTables.Create25YearCalendarFromStandardTables(DateTime.Now, true);
                }

                if (CreateTOUExportResult.Success == Result)
                {
                    TOUExportStream = new FileStream(strTOUExportFileName, FileMode.Create);                    
                    TOUExportWriter = new BinaryWriter(TOUExportStream);                    

                    // Build the streams for the 25 Year TOU table
                    PSEMData = m_CenTables.BuildPSEMStreams(TWENTY_FIVE_YEAR_TOU_TABLE_NUMBER);

                    // Write the TOU to the file
                    if (null == PSEMData || PSEMData.Length < 1)
                    {
                        //No TOU schedule data was present
                        Result = CreateTOUExportResult.NoTOUData;
                    }
                    else if (false == PSEMData[0].FullTable)
                    {
                        //It is an error if we have anything less than a full table's worth of data to read
                        Result = CreateTOUExportResult.ErrorReadingTOU;
                    }
                    else
                    {
                        try
                        {
                            byte[] abyTableNumber = BitConverter.GetBytes(TWENTY_FIVE_YEAR_TOU_TABLE_NUMBER);
                                
                            //Store table number MSB to LSB
                            Array.Reverse(abyTableNumber);

                            //Write TOU Calendar Table Number
                            TOUExportWriter.Write(abyTableNumber);

                            //Write C12.19 Table Data
                            TOUExportWriter.Write(PSEMData[0].PSEM.ToArray());

                            //Combine table number and table data for computing file CRC
                            byte[] abyFileData = abyTableNumber.Concat(PSEMData[0].PSEM.ToArray()).ToArray();

                            if (CRC.CalculateCRC32(CRC_POLYNOMIAL_SEED, abyFileData, out uiCRC))
                            {
                                byte[] abyCRC = BitConverter.GetBytes(uiCRC);
                                
                                //Store CRC MSB to LSB
                                Array.Reverse(abyCRC);

                                //Write File CRC
                                TOUExportWriter.Write(abyCRC);
                            }
                            else
                            {
                                Result = CreateTOUExportResult.ErrorWritingTOU;
                            }
                        }
                        catch
                        {
                            Result = CreateTOUExportResult.ErrorWritingTOU;
                        }
                    }                    
                }
            }
            catch
            {
                Result = CreateTOUExportResult.Error;
            }
            finally
            {
                if (null != TOUExportWriter)
                {
                    //Closing TOUExportWriter also closes TOUExportStream
                    TOUExportWriter.Close();
                }

                //If we did not succeed, remove the file.
                if (CreateTOUExportResult.Success != Result && File.Exists(strTOUExportFileName))
                {
                    File.Delete(strTOUExportFileName);
                }
            }

            return Result;
        }

        /// <summary>
        /// Gets whether or not the EDL file is for a single phase CENTRON meter.
        /// </summary>
        /// <returns>True if it is a mono Centron EDL file. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version  ID Issue# Description
        // -------- --- -------  -- ------ ------------------------------------------
        // 06/09/16 jrf 4.50.281 WR 633121 Created.
        //
        public bool IsMonoCentronMeter()
        {
            return (DeviceClass == CENTRON_AMI.ITR1_DEVICE_CLASS
                || DeviceClass == CENTRON_AMI.ITRD_DEVICE_CLASS
                || DeviceClass == CENTRON_AMI.ITRJ_DEVICE_CLASS);
        }

        /// <summary>
        /// Gets whether or not the EDL file is for a polyphase meter.
        /// </summary>
        /// <returns>True if it is a poly EDL file. False otherwise</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ------------------------------------------
        // 08/03/15 AF  4.20.19 586155 Taken from OpenWayProgramComparisonForm.cs
        // 10/31/16 jrf 4.70.28 230427 Made public.
        public bool IsPolyEDL()
        {
            return DeviceType == EDLDeviceTypes.OpenWayCentronBasicPoly
                || DeviceType == EDLDeviceTypes.OpenWayCentronAdvPoly
                || DeviceType == EDLDeviceTypes.OpenWayCentronBasicPolyITRE
                || DeviceType == EDLDeviceTypes.OpenWayCentronAdvPolyITRF
                || DeviceType == EDLDeviceTypes.OpenWayCentronPolyITRK;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines if the file is a program file
        /// </summary>
        /// <returns>true if the file is a program; false, otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/23/08 AF  10.0           Created for OpenWay Data Manager
        //  10/07/09 RCG 2.30.07 141466 Changed to a public property
        //  06/09/10 AF  2.41.08        M2 Gateway config files do not have table 2048 items so
        //                              we need another field to identify the file as a config file
        //  08/03/10 AF  2.42.11        Updated support for Gateway
        //
        public virtual bool IsProgramFile
        {
            get
            {
                bool bIsPgm = false;

                // Check on the existence of a field that will always be in a program
                // but not in a TOU or data file
                if (m_CenTables != null &&
                    (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH, null) ||
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null)))
                {
                    // We now know that the the file has configuration.  We only want this
                    // to return true if is not a data file, which also contains configuration
                    if (!m_CenTables.IsCached((long)StdTableEnum.STDTBL1_MFG_SERIAL_NUMBER, null))
                    {
                        bIsPgm = true;
                    }
                }
#if (!WindowsCE)
                else if ((m_GatewayTables != null) && (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null)))
                {
                    // We now know that the the file has configuration.  We only want this
                    // to return true if is not a data file, which also contains configuration
                    if (!m_GatewayTables.IsCached((long)StdTableEnum.STDTBL1_MFG_SERIAL_NUMBER, null))
                    {
                        bIsPgm = true;
                    }
                }
#endif

                return bIsPgm;
            }
        }//IsEDLProgramFile

        /// <summary>
        /// Determines whether the file is a TOU schedule file
        /// </summary>
        /// <returns>true if the file is a TOU schedule; false, otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/23/08 AF  10.0           Created for OpenWay Data Manager
        //  10/07/09 RCG 2.30.07 141466 Changed to a public property
        //  06/09/10 AF  2.41.08        M2 Gateway config files do not have table 2048 items so
        //                              we need another field to identify the file as a config file
        //
        public virtual bool IsTOUFile
        {
            get
            {
                bool bIsTOU = false;

                if (m_CenTables != null && m_CenTables.IsCached((long)StdTableEnum.STDTBL6_TARIFF_ID, null))
                {
                    // At this point, we have a file that is an EDL file and
                    // contains TOU.  Make sure it's not just a program file
                    if ((!(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH, null))) ||
                        (!(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null))))
                    {
                        bIsTOU = true;
                    }
                }
#if (!WindowsCE)
                else if ((m_GatewayTables != null) && (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL6_TARIFF_ID, null)))
                {
                    // At this point, we have a file that is an EDL file and
                    // contains TOU.  Make sure it's not just a program file
                    if (!(m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null)))
                    {
                        bIsTOU = true;
                    }
                }
#endif

                return bIsTOU;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Load Profile data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created

        public virtual bool ContainsLoadProfile
        {
            get
            {
                bool bContainsLP = false;
                object objValue = null;
                byte byChannels = 0;
                ushort usUsedBlocks = 0;
                ushort usUsedIntervals = 0;

                if (m_CenTables != null)
                {
                    if (m_CenTables.IsCached((long)StdTableEnum.STDTBL61_NBR_CHNS_SET1, null))
                    {
                        // Make sure that the LP Data contains some channels
                        m_CenTables.GetValue(StdTableEnum.STDTBL61_NBR_CHNS_SET1, null, out objValue);
                        byChannels = (byte)objValue;

                        if (byChannels > 0)
                        {
                            // Now make sure that there are valid blocks
                            if (m_CenTables.IsCached((long)StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null))
                            {
                                m_CenTables.GetValue(StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null, out objValue);
                                usUsedBlocks = (ushort)objValue;

                                if (usUsedBlocks > 0)
                                {
                                    // Finally make sure that there are intervals present.
                                    if (m_CenTables.IsCached((long)StdTableEnum.STDTBL63_NBR_VALID_INT, null))
                                    {
                                        m_CenTables.GetValue(StdTableEnum.STDTBL63_NBR_VALID_INT, null, out objValue);
                                        usUsedIntervals = (ushort)objValue;

                                        if (usUsedIntervals > 0)
                                        {
                                            bContainsLP = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return bContainsLP;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Extended Load Profile data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/15/11 RCG 2.53.20        Created

        public virtual bool ContainsExtendedLoadProfile
        {
            get
            {
                bool bContainsLP = false;
                object objValue = null;
                byte byChannels = 0;
                ushort usUsedBlocks = 0;
                ushort usUsedIntervals = 0;

                if (m_CenTables != null)
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl361_NBR_CHNS_SET1, null))
                    {
                        // Make sure that the LP Data contains some channels
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl361_NBR_CHNS_SET1, null, out objValue);
                        byChannels = (byte)objValue;

                        if (byChannels > 0)
                        {
                            // Now make sure that there are valid blocks
                            if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, null))
                            {
                                m_CenTables.GetValue(CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, null, out objValue);
                                usUsedBlocks = (ushort)objValue;

                                if (usUsedBlocks > 0)
                                {
                                    // Finally make sure that there are intervals present.
                                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl363_NBR_VALID_INT, null))
                                    {
                                        m_CenTables.GetValue(CentronTblEnum.MfgTbl363_NBR_VALID_INT, null, out objValue);
                                        usUsedIntervals = (ushort)objValue;

                                        if (usUsedIntervals > 0)
                                        {
                                            bContainsLP = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return bContainsLP;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Instrumentation Profile data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/15/11 RCG 2.53.18        Created
        //  10/10/13 AF  3.00.15 WR392286 Instrumentation Profile could be in the meter
        //                                but not in the EDL file
        //
        public virtual bool ContainsInstrumentationProfile
        {
            get
            {
                bool bContainsIP = false;
                object objValue = null;
                byte byChannels = 0;
                ushort usUsedBlocks = 0;
                ushort usUsedIntervals = 0;
                byte SetIndex = 1;

                if (m_CenTables != null)
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl361_NBR_CHNS_SET2, null))
                    {
                        // Make sure that the LP Data contains some channels
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl361_NBR_CHNS_SET2, null, out objValue);
                        byChannels = (byte)objValue;

                        if (byChannels > 0)
                        {
                            // Now make sure that there are valid blocks
                            if (m_CenTables.IsCached(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, SetIndex), null))
                            {
                                m_CenTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, SetIndex), null, out objValue);
                                usUsedBlocks = (ushort)objValue;

                                if (usUsedBlocks > 0)
                                {
                                    // Next make sure that there are intervals present.
                                    if (m_CenTables.IsCached(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_INT, SetIndex), null))
                                    {
                                        m_CenTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_INT, SetIndex), null, out objValue);
                                        usUsedIntervals = (ushort)objValue;

                                        if (usUsedIntervals > 0)
                                        {
                                            // Finally, check that the intervals are present in the EDL file
                                            // Check anything from mfg table 365 (2413)
                                            int[] aiBlock = { 0 };

                                            if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl365_BLK_END_TIME, aiBlock))
                                            {
                                                bContainsIP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return bContainsIP;
            }
        }

        /// <summary>
        /// Returns true if the EDL file contains Voltage Monitoring data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/08 RCG 1.50.23 N/A    Created
        //  12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        //  01/31/12 jrf 2.53.36 192969 Added code to correctly determine if the voltage
        //                              monitoring data table (either 2152 or 2157) exists
        //                              in the EDL file.  If not, this method must return false.
        //
        public virtual bool ContainsVoltageMonitoring
        {
            get
            {
                bool bContainsVMData = false;
                bool bVMEnabled = false;
                object objValue;
                ushort usUsedBlocks;

                if (m_CenTables != null)
                {
                    if(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL106_ENABLE_FLAG, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL106_ENABLE_FLAG, null, out objValue);
                        bVMEnabled = (bool)objValue;

                        if (bVMEnabled == true)
                        {
                            // Make sure there are valid blocks
                            if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL107_NBR_VALID_BLOCKS, null))
                            {
                                m_CenTables.GetValue(CentronTblEnum.MFGTBL107_NBR_VALID_BLOCKS, null, out objValue);
                                usUsedBlocks = (ushort)objValue;

                                if (usUsedBlocks > 0)
                                {
                                    //Make sure the data table is in the file.
                                    bContainsVMData = m_CenTables.IsTableKnown(2157) && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL109_BLK_END_TIME, new int[] { 0 });
                                }
                            }
                        }
                    }

                    if (false == bContainsVMData && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_ENABLE_FLAG, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, out objValue);
                        bVMEnabled = (bool)objValue;

                        if (bVMEnabled == true)
                        {
                            // Make sure there are valid blocks
                            if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null))
                            {
                                m_CenTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null, out objValue);
                                usUsedBlocks = (ushort)objValue;

                                if (usUsedBlocks > 0)
                                {
                                    //Make sure the data table is in the file.
                                    bContainsVMData = m_CenTables.IsTableKnown(2152) && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL104_BLK_END_TIME, new int[] { 0 });
                                }
                            }
                        }
                    }
                }

                return bContainsVMData;
            }
        }

        /// <summary>
        /// Determines whether or not the EDL File contains Enhanced Voltage Monitoring Config Items
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 02/14/12 RCG 2.53.40 TRQ2952 Created 
        // 08/01/12 jrf 2.70.01 197895  Updated to handle case where enhanced Voltage monitoring 
        //                              has never been configured.
        public bool ContainsEnhancedVoltageMonitoringConfig
        {
            get
            {
                bool bContainsEnhancedConfig = false;

                if (m_CenTables.IsAllCached(2153) && m_CenTables.IsAllCached(2154))
                {
                    //Kind of a hack, but I do not see a more straightforward way to tell if a configuration that supports
                    //enhanced voltage monitoring has been configured in the meter.
                    byte NumberOfChannels = 0;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileNumberOfChannels, null))
                    {
                        object objValue = null;

                        m_CenTables.GetValue(CentronTblEnum.MfgTbl217NonBillingLoadProfileNumberOfChannels, null, out objValue);
                        NumberOfChannels = (byte)objValue;

                        if (NumberOfChannels != 0xFF) //value will only be 0xFF if it has never been configured.
                        {
                            //A program that has configured enhanced load profile will have configured enhanced voltage monitoring too. 
                            bContainsEnhancedConfig = true;
                        }
                    }
                }

                return bContainsEnhancedConfig;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Register data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        public bool ContainsRegisterData
        {
            get
            {
                bool bContainsRegisterData = false;

                if (Table11 != null && Table14 != null && Table21 != null && Table22 != null && Table23 != null)
                {
                    bContainsRegisterData = true;
                }

                return bContainsRegisterData;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains Extended Register data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/18/12 RCG 2.53.31 TRQ3439 Created 

        public bool ContainsExtendedRegisterData
        {
            get
            {
                bool bContainsData = false;

                if (Table2419 != null && Table2422 != null)
                {
                    bContainsData = Table2422.CurrentExtEnergyData != null && Table2422.CurrentExtEnergyData.Count > 0;
                }

                return bContainsData;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains Extended Register data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 03/16/12 AF  2.53.50 TREQ5775 Created 
        //
        public bool ContainsExtendedInstantaneousData
        {
            get
            {
                bool bContainsData = false;

                if (Table2419 != null && Table2422 != null)
                {
                    bContainsData = Table2422.CurrentExtInstantaneousData != null && Table2422.CurrentExtInstantaneousData.Count > 0;
                }

                return bContainsData;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Configuration data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        //  06/09/10 AF  2.41.08        M2 Gateway config files do not have table 2048 items so
        //                              we need another field to identify the file as a config file
        //
        public virtual bool ContainsConfiguration
        {
            get
            {
                bool bContainsConfigurationData = false;

                if ((m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH, null)) ||
                    ((m_CenTables.IsCached((long)CentronTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null))))
                {
                    bContainsConfigurationData = true;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null))
                {
                    bContainsConfigurationData = true;
                }
#endif

                return bContainsConfigurationData;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains TOU data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/08/08 RCG 1.50.16        Created

        public bool ContainsTOU
        {
            get
            {
                bool bContainsTOUData = false;

                if (TOUScheduleID != null && TOUScheduleID != "")
                {
                    bContainsTOUData = true;
                }

                return bContainsTOUData;
            }
        }

        /// <summary>
        /// Returns true of the EDL has Device Status Data in it.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual bool ContainsDeviceStatus
        {
            get
            {
                bool bContainsDeviceStatus = false;

                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL1_MFG_SERIAL_NUMBER, null))
                {
                    bContainsDeviceStatus = true;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL1_MFG_SERIAL_NUMBER, null))
                {
                    bContainsDeviceStatus = true;
                }
#endif

                return bContainsDeviceStatus;
            }
        }

        /// <summary>
        /// Returns true of the EDL has Device Status Data in it.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/05/08 jrf 1.50.32 114519 Created.
        // 08/02/10 AF  2.42.11         Added support for M2 Gateway
        //    
        public virtual bool ContainsRFLANNeighbors
        {
            get
            {
                bool bContainsNeighbors = false;

                if (m_CenTables.IsAllCached(2078))
                {
                    bContainsNeighbors = true;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsAllCached(2078))
                {
                    bContainsNeighbors = true;
                }
#endif

                return bContainsNeighbors;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains the SiteScan Toolbox data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public virtual bool ContainsSiteScanToolbox
        {
            get
            {
                return Table2091 != null;
            }
        }

        /// <summary>
        /// Gets whether or not the file contains a 25 year DST Calendar
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public bool Contains25YearDST
        {
            get
            {
                bool bContainsDST = false;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212DstYear, new int[] { 0 }))
                {
                    object objValue;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212DstYear, new int[] { 0 }, out objValue);

                    bContainsDST = (byte)objValue != 0xFF;
                }

                return bContainsDST;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains Extended Self Read data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/20/12 jrf 2.53.32 TREQ3448 Created
        //
        public bool ContainsSelfReadData
        {
            get
            {
                bool blnContainsData = false;

                if (null != Table26 )
                {
                    blnContainsData = (Table26.NumberOfValidEntries > 0);
                }

                return blnContainsData;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains Extended Self Read data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/20/12 jrf 2.53.32 TREQ3448 Created
        //  02/10/12 AF  2.53.40 193676 Changed the second check on table 2319 null to
        //                              check on table 2423 not equal null
        //
        public bool ContainsExtendedSelfReadData
        {
            get
            {
                bool blnContainsData = false;

                if (null != Table2419 && null != Table2421
                    && null != Table2423)
                {
                    blnContainsData = (null != Table2423.ExtendedSelfReadData && Table2423.ExtendedSelfReadData.Count > 0);
                }

                return blnContainsData;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file supports Extended Self Reads
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/16 jrf 4.70.28 230427 Created.
        //
        public bool SupportsExtendedSelfRead
        {
            get
            {
                return DeviceClass == CENTRON_AMI.ITRD_DEVICE_CLASS
                || DeviceClass == CENTRON_AMI.ITRJ_DEVICE_CLASS
                || DeviceClass == CENTRON_AMI.ITRE_DEVICE_CLASS
                || DeviceClass == CENTRON_AMI.ITRF_DEVICE_CLASS
                || DeviceClass == CENTRON_AMI.ITRK_DEVICE_CLASS;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains Current Per Phase Threshold Exceeded
        /// configuration data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 04/26/13 AF  2.80.23 TR7590 Added for display of Current Per Phase Threshold Exceeded items.
        //
        public bool ContainsCurrentThresholdExceededConfig
        {
            get
            {
                bool blnContainsData = false;

                blnContainsData = m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217CurrentThresholdExceededEnable, null);

                return blnContainsData;
            }
        }

        /// <summary>
        /// Gets the Calendar ID from the file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/27/12 RCG 2.53.52 195665 Created

        public ushort CalendarID
        {
            get
            {
                ushort usCalendarID = 0;

                try
                {
                    UpdateTOUSchedule();

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL42_CALENDAR_ID, null))
                    {
                        object objValue;

                        m_CenTables.GetValue(CentronTblEnum.MFGTBL42_CALENDAR_ID, null, out objValue);
                        usCalendarID = (ushort)objValue;
                    }
                }
                catch (Exception)
                {
                    // If we get an exception trying to retrieve the value this must mean that it's not supported so set it to 0
                    usCalendarID = 0;
                }

                return usCalendarID;
            }
        }

        /// <summary>
        /// Gets whether or not the file enables load profile validation using self reads
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/11 AF  2.50.43 171706 Created
        //
        public bool EnableLPValidationUsingSelfReads
        {
            get
            {
                bool bEnabled = false;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212ZeroToEnableLoadProfileHourlySelfRead, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212ZeroToEnableLoadProfileHourlySelfRead, null, out objValue);
                    if ((byte)objValue == 0)
                    {
                        bEnabled = true;
                    }
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets the number of daily self reads in the file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/11 AF  2.50.43 171706 Created
        //
        public byte NumberOfDailySelfReads
        {
            get
            {
                byte NumReads = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212NumberOfDailySelfReads, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212NumberOfDailySelfReads, null, out objValue);
                    NumReads = (byte)objValue;
                }

                return NumReads;
            }
        }

        /// <summary>
        /// Gets whether or not the file configures the meter to align self reads with standard time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/11 AF  2.50.43 171706 Created
        //
        public bool AlignSRsWithStdTime
        {
            get
            {
                bool bAligned = false;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212AlignSelfReadsToStandardTime, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212AlignSelfReadsToStandardTime, null, out objValue);

                    bAligned = (bool)objValue;
                }

                return bAligned;
            }
        }

        /// <summary>
        /// Gets the Collection Engine Version
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/11/10 RCG 2.45.03        Created
        //  04/09/12 AF  2.53.54 196608 Added support for the M2 Gateway
        //
        public virtual string CollectionEngineVersion
        {
            get
            {
                string strValue = null;

                if (m_CenTables != null && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL2045_CE_VERSION_NUMBER, null))
                {
                    object objValue = null;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL2045_CE_VERSION_NUMBER, null, out objValue);
                    strValue = objValue as string;
                }
#if (!WindowsCE)
                else if ((m_GatewayTables != null) && (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL2045_CE_VERSION_NUMBER, null)))
                {
                    object objValue = null;
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL2045_CE_VERSION_NUMBER, null, out objValue);
                    strValue = objValue as string;
                }
#endif

                return strValue;
            }
        }

        /// <summary>
        /// Gets the Security Provider
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/16 jrf 4.70.28 230427 Created
        //
        public virtual int? SecurityProvider
        {
            get
            {
                int? IntValue = null;


                if (m_CenTables != null && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL2045_SECURITY_PROVIDER, null))
                {
                    object objValue = null;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL2045_SECURITY_PROVIDER, null, out objValue);
                    IntValue = objValue as int?;
                }
#if (!WindowsCE)
                else if ((m_GatewayTables != null) && (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL2045_SECURITY_PROVIDER, null)))
                {
                    object objValue = null;
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL2045_SECURITY_PROVIDER, null, out objValue);
                    IntValue = objValue as int?;
                }
#endif

                return IntValue;
            }
        }

        /// <summary>
        /// Gets the Security Provider
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/16 jrf 4.70.28 230427 Created
        //
        public virtual string SecurityProviderString
        {
            get
            {
                string StringValue = "";

                if (null != SecurityProvider)
                {
                    switch (SecurityProvider)
                    {
                        case (byte)SecurityProviderSelection.Standard:
                            {
                                StringValue = Resources.StandardC1222Security;
                                break;
                            }
                        case (byte)SecurityProviderSelection.Enhanced:
                            {
                                StringValue = Resources.EnhancedItronSecurity;
                                break;
                            }
                        default:
                            break;
                    }
                }
                return StringValue;
            }
        }

        /// <summary>
        /// Returns a LoadProfile object built from the information in the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/30/06 RDB				   Created
        // 12/15/11 RCG 2.53.20        Added check for contains load profile

        public LoadProfileData LPData
        {
            get
            {
                if (null == m_LoadProfile && ContainsLoadProfile)
                {
                    GetLoadProfileData();
                }
                return m_LoadProfile;

            }//get
        }

        /// <summary>
        /// Gets the Extend Load Profile data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/15/11 RCG	2.53.20		   Created

        public LoadProfileData ExtendedLoadProfileData
        {
            get
            {
                if (m_ExtendedLoadProfile == null && ContainsExtendedLoadProfile)
                {
                    GetExtendedLoadProfileData();
                }

                return m_ExtendedLoadProfile;
            }
        }

        /// <summary>
        /// Gets the Instrumentation Profile data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/15/11 RCG	2.53.18		   Created

        public LoadProfileData InstrumentationProfileData
        {
            get
            {
                if (m_InstrumentationProfile == null && ContainsInstrumentationProfile)
                {
                    GetInstrumentationProfileData();
                }

                return m_InstrumentationProfile;
            }
        }

        /// <summary>
        /// Gets/sets the Min Time to elapse before marking a Power Outage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.11        Added set.
        //
        public virtual int LPMinPowerOutage
        {
            get
            {
                if (!m_iLPMinPowerOutage.Cached)
                {
                    m_iLPMinPowerOutage.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_LP_MIN_POWER_OUTAGE);
                }

                return m_iLPMinPowerOutage.Value;
            }
            set
            {
                m_iLPMinPowerOutage.Value = value;

                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_LP_MIN_POWER_OUTAGE);
            }
        }

        /// <summary>
        /// Returns the load profile memory size
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //
        public virtual int LPMemorySize
        {
            get
            {
                if (!m_iLPMemorySize.Cached)
                {
                    m_iLPMemorySize.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_LP_MEMORY_SIZE);
                }

                return m_iLPMemorySize.Value;
            }
        }

        /// <summary>
        /// Gets the Voltage Monitoring Data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/08 RCG 1.50.23 N/A    Created

        public VMData VoltageMonitoringData
        {
            get
            {
                if (m_VMData == null)
                {
                    m_VMData = GetVoltageMonitoringData();
                }

                return m_VMData;
            }
        }

        /// <summary>
        /// Gets the TOU Schedule
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/12/08 KRC                Adding Current TOU Schedule to EDL Translation
        //  04/11/08 RCG 1.50.16        Fixing issue with displaying TOU Schedules from program/TOU files
        //  03/27/12 RCG 2.53.52 195665 Calling one method to update the TOU schedule in 2090
        //  11/18/13 jrf 3.50.06 TQ9479 Refactored majority of method to ReadTOUConfiguration().

        public virtual CTOUSchedule TOUSchedule
        {
            get
            {
                CTOUSchedule TOUSchedule = null;

                if (ContainsTOU)
                {   
                    TOUSchedule = CENTRON_AMI.ReadTOUSchedule(TOUConfiguration, CalendarConfiguration);
                }

                return TOUSchedule;
            }
        }

        /// <summary>
        /// Gets/sets the season change options for the TOU schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/17/10 jrf 2.45.13 N/A    Created
        //
        public virtual SeasonChangeOptions SeasonChangeOption
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get the Normal Display Configuration List
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/28/13 jrf 2.70.72 323232 Added method to handle display items with irregular descriptions.
        //
        public virtual List<OpenWayDisplayItem> NormalDisplayConfig
        {
            get
            {
                IList<TableData> DisplayTables = m_CenTables.BuildPSEMStreams(2048, ConfigHeader.DisplayOffset, CENTRON_AMI_DisplayConfig.DISPLAY_CONFIG_SIZE);
                List<OpenWayDisplayItem> DisplayItems = new List<OpenWayDisplayItem>();

                //Assembly the multiple streams that I received to one stream that I can use.
                Stream DisplayStream = BuildStream(DisplayTables, ConfigHeader.DisplayOffset, CENTRON_AMI_DisplayConfig.DISPLAY_CONFIG_SIZE);

                PSEMBinaryReader DisplayReader = new PSEMBinaryReader(DisplayStream);
                CENTRON_AMI_DisplayConfig DisplayConfigTable = new CENTRON_AMI_DisplayConfig(DisplayReader, ConfigHeader.DisplayOffset);

                foreach (ANSIDisplayData DisplayData in DisplayConfigTable.NormalDisplayData)
                {
                    OpenWayDisplayItem CurrentDisplayItem = new OpenWayDisplayItem(new CentronAMILID(DisplayData.NumericLID),
                        DisplayData.DisplayID, DisplayData.DisplayFormat, DisplayData.DisplayDimension);

                    HandleIrregularDescription(CurrentDisplayItem);

                    DisplayItems.Add(CurrentDisplayItem);
                }

                return DisplayItems;
            }
        }

#if (!WindowsCE)
        /// <summary>
        /// Get the Display Configuration List for the M2 Gateway
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/22/10 AF  2.45.06 161866 Created
        //
        public List<string> M2GatewayDisplayConfig
        {
            get
            {
                List<string> GatewayDisplayList = new List<string>();
                object Value;
                int[] anIndex1 = { 0 };

                for (int iIndex = 0; iIndex < 3; iIndex++)
                {
                    anIndex1[0] = iIndex;
                    m_GatewayTables.GetValue(StdTableEnum.STDTBL34_SEC_DISP_SOURCES, anIndex1, out Value);
                    GatewayDisplayList.Add(Value.ToString());
                }

                return GatewayDisplayList;
            }
        }
#endif

        /// <summary>
        /// Get the Test Display Configuration List
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/28/13 jrf 2.70.72 323232 Added method to handle display items with irregular descriptions.
        //
        public virtual List<OpenWayDisplayItem> TestDisplayConfig
        {
            get
            {
                IList<TableData> DisplayTables = m_CenTables.BuildPSEMStreams(2048, ConfigHeader.DisplayOffset, CENTRON_AMI_DisplayConfig.DISPLAY_CONFIG_SIZE);
                List<OpenWayDisplayItem> DisplayItems = new List<OpenWayDisplayItem>();

                //Assembly the multiple streams that I received to one stream that I can use.
                Stream DisplayStream = BuildStream(DisplayTables, ConfigHeader.DisplayOffset, CENTRON_AMI_DisplayConfig.DISPLAY_CONFIG_SIZE);

                PSEMBinaryReader DisplayReader = new PSEMBinaryReader(DisplayStream);
                CENTRON_AMI_DisplayConfig DisplayConfigTable = new CENTRON_AMI_DisplayConfig(DisplayReader, ConfigHeader.DisplayOffset);

                foreach (ANSIDisplayData DisplayData in DisplayConfigTable.TestDisplayData)
                {
                    OpenWayDisplayItem CurrentDisplayItem = new OpenWayDisplayItem(new CentronAMILID(DisplayData.NumericLID),
                        DisplayData.DisplayID, DisplayData.DisplayFormat, DisplayData.DisplayDimension);

                    HandleIrregularDescription(CurrentDisplayItem);

                    DisplayItems.Add(CurrentDisplayItem);
                }

                return DisplayItems;
            }
        }

        /// <summary>
        /// Get the Alternate Display Configuration List
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/28/13 jrf 2.70.72 323232 Added method to handle display items with irregular descriptions.
        //
        public virtual List<OpenWayDisplayItem> AlternateDisplayConfig
        {
            get
            {
                IList<TableData> DisplayTables = m_CenTables.BuildPSEMStreams(2048, ConfigHeader.DisplayOffset, CENTRON_AMI_DisplayConfig.DISPLAY_CONFIG_SIZE);
                List<OpenWayDisplayItem> DisplayItems = new List<OpenWayDisplayItem>();

                //Assembly the multiple streams that I received to one stream that I can use.
                Stream DisplayStream = BuildStream(DisplayTables, ConfigHeader.DisplayOffset, CENTRON_AMI_DisplayConfig.DISPLAY_CONFIG_SIZE);

                PSEMBinaryReader DisplayReader = new PSEMBinaryReader(DisplayStream);
                CENTRON_AMI_DisplayConfig DisplayConfigTable = new CENTRON_AMI_DisplayConfig(DisplayReader, ConfigHeader.DisplayOffset);

                foreach (ANSIDisplayData DisplayData in DisplayConfigTable.AlternateDisplayData)
                {
                    OpenWayDisplayItem CurrentDisplayItem = new OpenWayDisplayItem(new CentronAMILID(DisplayData.NumericLID),
                        DisplayData.DisplayID, DisplayData.DisplayFormat, DisplayData.DisplayDimension);

                    HandleIrregularDescription(CurrentDisplayItem);

                    DisplayItems.Add(CurrentDisplayItem);
                }

                return DisplayItems;
            }
        }

        /// <summary>Returns the current register data from table 23</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/15/07 mcm 8.10.05  	    Created
        // 07/29/09 RCG 2.20.19 134394 Rewriting to use new standard tables and support coincidents

        public virtual List<Quantity> CurrentRegisters
        {
            get
            {
                if (null == m_Registers)
                {
                    GetCurrentRegisters();
                }

                return m_Registers;
            } // get
        } // CurrentRegisters

        /// <summary>
        /// Gets the list of the Extended Energy Register values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/18/12 RCG 2.53.31 TRQ3439 Created 

        public List<ExtendedCurrentEntryRecord> ExtendedEnergyRegisters
        {
            get
            {
                List<ExtendedCurrentEntryRecord> RegisterData = null;

                if (ContainsExtendedRegisterData)
                {
                    RegisterData = Table2422.CurrentExtEnergyData.ToList();
                }

                return RegisterData;
            }
        }

        /// <summary>
        /// This property retrieves the extended self read data as a list of extended 
        /// self read records in descending date order from the most recent self read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 jrf 2.53.32 TREQ2904 Created
        //
        public List<ExtendedSelfReadRecord> ExtendedSelfReadData
        {
            get
            {
                List<ExtendedSelfReadRecord> ExtSRData = null;

                if (true == ContainsExtendedSelfReadData)
                {
                    ExtSRData = CENTRON_AMI.ReorderExtendedSelfReadData(Table2419, Table2421, Table2423);
                }

                return ExtSRData;
            }
        }

        /// <summary>
        /// Returns the TOU Calendar data from table 54 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/30/08 AF  10.0           Created
        //
        public C1219_CalendarRcd TOUCalendar
        {
            get
            {
                if (null == m_Calendar)
                {
                    GetTOUCalendar();
                }
                return m_Calendar;
            }
        }

        /// <summary>
        /// Returns the Clock Synchronization method
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        //
        public virtual string ClockSynch
        {
            get
            {
                if (!m_strClockSynch.Cached)
                {
                    int iClockSynch = GetMFGEDLInt(CentronTblEnum.MFGTBL0_CLOCK_SYNC);
                    if (iClockSynch == 0)
                    {
                        m_strClockSynch.Value = CRYSTAL_SYNC;
                    }
                    else
                    {
                        m_strClockSynch.Value = LINE_SYNC;
                    }
                }

                return m_strClockSynch.Value;
            }
        }

        /// <summary>
        /// Sets the clock synchronization method.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created
        //
        public virtual ClockSynchronization ClockSynchValue
        {
            set
            {
                switch (value)
                {
                    case ClockSynchronization.Crystal:
                    {
                        m_strClockSynch.Value = CRYSTAL_SYNC;
                        SetMFGEDLInt((int)value, CentronTblEnum.MFGTBL0_CLOCK_SYNC);
                        break;
                    }
                    default: //Assume all other are line synchronization
                    {
                        m_strClockSynch.Value = LINE_SYNC;
                        SetMFGEDLInt((int)value, CentronTblEnum.MFGTBL0_CLOCK_SYNC);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Get the DST Switch Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual string DSTSwitchTime
        {
            get
            {
                int iDSTHour = 0;
                int iDSTMin = 0;

                if (!m_strDSTSwitch.Cached)
                {
                    if (m_CenTables.IsAllCached(2090))
                    {
                        iDSTHour = GetMFGEDLInt(CentronTblEnum.MFGTBL42_DST_HOUR);
                        iDSTMin = GetMFGEDLInt(CentronTblEnum.MFGTBL42_DST_MINUTE);
                    }
#if (!WindowsCE)
                    else
                    {
                        iDSTHour = GetMFGEDLInt(GatewayTblEnum.MFGTBL42_DST_HOUR);
                        iDSTMin = GetMFGEDLInt(GatewayTblEnum.MFGTBL42_DST_MINUTE);
                    }
#endif

                    m_strDSTSwitch.Value = iDSTHour.ToString(CultureInfo.CurrentCulture)
                        + ":" + iDSTMin.ToString("00", CultureInfo.CurrentCulture);
                }

                return m_strDSTSwitch.Value;
            }
        }

        /// <summary>
        /// Tells us if the EOI indicator is set to be displayed
        /// </summary>
        public virtual bool DisplayEOI
        {
            get
            {
                if (!m_blnDisplayEOI.Cached)
                {
                    m_blnDisplayEOI.Value = GetMFGEDLBool(CentronTblEnum.MFGTBL0_DISPLAY_EOI);
                }

                return m_blnDisplayEOI.Value;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Tells us if the Watt Load Indicator is set to be displayed
        /// </summary>
        public virtual bool DisplayWattLoadIndicator
        {
            get
            {
                if (!m_blnWattIndicator.Cached)
                {
                    m_blnWattIndicator.Value = GetMFGEDLBool(CentronTblEnum.MFGTBL0_WATT_LOAD_INDICATOR);
                }

                return m_blnWattIndicator.Value;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Tells us if the Remote Disconnect OFF Message is Enabled
        /// </summary>
        public virtual bool DisplayRemoteDisconnectOFFMessage
        {
            get
            {
                if (!m_blnDisonnectOFFMessage.Cached)
                {
                    m_blnDisonnectOFFMessage.Value = GetMFGEDLBool(CentronTblEnum.MFGTBL0_DISPLAY_REMOTE_DISCONNECT_MESSAGE_FLAG);
                }

                return m_blnDisonnectOFFMessage.Value;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets whether or not the PF Load Indicator will be shown on the display
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created

        public virtual bool DisplayPFLoadIndicator
        {
            get
            {
                bool bDisplayIndicator = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_VAR_LOAD_INDICATOR, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_VAR_LOAD_INDICATOR, null, out objValue);

                    bDisplayIndicator = (bool)objValue;
                }

                return bDisplayIndicator;
            }
        }

        /// <summary>
        /// Gets whether or not the Missing Phase Indicators will be shown on the display
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created

        public virtual bool DisplayPhaseIndicators
        {
            get
            {
                bool bDisplayIndicator = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_ENABLE_PHASE_INDICATORS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_ENABLE_PHASE_INDICATORS, null, out objValue);

                    bDisplayIndicator = (bool)objValue;
                }

                return bDisplayIndicator;
            }
        }

        /// <summary>
        /// Gets whether or not the Display Phase A Voltage Indicator will be shown on the display
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/16 jrf 4.70.28 230427 Created
        public virtual bool DisplayPhaseAVoltageIndicator
        {
            get
            {
                bool bDisplayIndicator = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_PHASE_A_VOLTAGE_DISPLAYS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_PHASE_A_VOLTAGE_DISPLAYS, null, out objValue);

                    bDisplayIndicator = (bool)objValue;
                }

                return bDisplayIndicator;
            }
        }

        /// <summary>
        /// Gets whether or not the Missing Phase Indicator should blink
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created

        public virtual bool BlinkMissingPhaseIndicator
        {
            get
            {
                bool bBlinkIndicator = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_BLINK_MISSING_PHASES, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_BLINK_MISSING_PHASES, null, out objValue);

                    bBlinkIndicator = (bool)objValue;
                }

                return bBlinkIndicator;
            }
        }

        /// <summary>
        /// Returns the Time each display Item spends on display in seconds
        /// </summary>
        public virtual int DisplayOnTime
        {
            get
            {
                if (!m_iDisplayOnTime.Cached)
                {
                    m_iDisplayOnTime.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_ITEM_DISPLAY_TIME);
                    m_iDisplayOnTime.Value = m_iDisplayOnTime.Value / 4;
                }

                return m_iDisplayOnTime.Value;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get the setting for the Low Battery Error
        /// </summary>
        public virtual string LowBatteryError
        {
            get
            {
                if (!m_strLowBatteryError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_LOW_BATTERY))
                    {
                        m_strLowBatteryError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_LOW_BATTERY))
                    {
                        m_strLowBatteryError.Value = "Lock";
                    }
                    else
                    {
                        m_strLowBatteryError.Value = "Ignore";
                    }
                }

                return m_strLowBatteryError.Value;
            }
        }

        /// <summary>
        /// Get the setting for the Loss of Phase Error
        /// </summary>
        public virtual string LossOfPhaseError
        {
            get
            {
                if (!m_strLossOfPhaseError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_LOSS_PHASE))
                    {
                        m_strLossOfPhaseError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_LOSS_PHASE))
                    {
                        m_strLossOfPhaseError.Value = "Lock";
                    }
                    else
                    {
                        m_strLossOfPhaseError.Value = "Ignore";
                    }
                }

                return m_strLossOfPhaseError.Value;
            }
        }

        /// <summary>
        /// Get the setting of the Clock TOU Error
        /// </summary>
        public virtual string ClockTOUError
        {
            get
            {
                if (!m_strClockTOUError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_TOU_SCHEDULE_ERROR))
                    {
                        m_strClockTOUError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_TOU_SCHEDULE_ERROR))
                    {
                        m_strClockTOUError.Value = "Lock";
                    }
                    else
                    {
                        m_strClockTOUError.Value = "Ignore";
                    }
                }

                return m_strClockTOUError.Value;
            }
        }

        /// <summary>
        /// Get the setting for the Reverse Power Flow Error
        /// </summary>
        public virtual string ReversePowerError
        {
            get
            {
                if (!m_strReversePowerError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_REVERSE_POWER_FLOW))
                    {
                        m_strReversePowerError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_REVERSE_POWER_FLOW))
                    {
                        m_strReversePowerError.Value = "Lock";
                    }
                    else
                    {
                        m_strReversePowerError.Value = "Ignore";
                    }
                }

                return m_strReversePowerError.Value;
            }
        }

        /// <summary>
        /// Get the setting for the Load Profile (Mass Memory) error
        /// </summary>
        public virtual string LoadProfileError
        {
            get
            {
                if (!m_strLoadProfileError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_MASS_MEMORY))
                    {
                        m_strLoadProfileError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_MASS_MEMORY))
                    {
                        m_strLoadProfileError.Value = "Lock";
                    }
                    else
                    {
                        m_strLoadProfileError.Value = "Ignore";
                    }
                }

                return m_strLoadProfileError.Value;
            }
        }

        /// <summary>
        /// Get the Setting for the Register Full Scale Error
        /// </summary>
        public virtual string FullScaleError
        {
            get
            {
                if (!m_strFullScaleError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_REGISTER_FULL_SCALE))
                    {
                        m_strFullScaleError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_REGISTER_FULL_SCALE))
                    {
                        m_strFullScaleError.Value = "Lock";
                    }
                    else
                    {
                        m_strFullScaleError.Value = "Ignore";
                    }
                }

                return m_strFullScaleError.Value;
            }
        }

        /// <summary>
        /// Get the Setting for the Register Full Scale Error
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created
        //  12/01/09 AF  2.30.22 145961 Added a member variable to keep track of
        //                              SiteScan error.  (We were using m_strFullScaleError)

        public string SiteScanError
        {
            get
            {
                if (!m_strSiteScanError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_SITESCAN_ERROR))
                    {
                        m_strSiteScanError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_SITESCAN_ERROR))
                    {
                        m_strSiteScanError.Value = "Lock";
                    }
                    else
                    {
                        m_strSiteScanError.Value = "Ignore";
                    }
                }

                return m_strSiteScanError.Value;
            }
        }

        /// <summary>
        /// Gets the List of Energy LIDs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/31/16 jrf 4.70.28 230427 Modified unselected energy text to be consistent with CE UI.
        public List<string> EnergyConfigList
        {
            get
            {
                List<String> EnergyConfigList = new List<String>();

                foreach (LID EnergyLid in EnergyConfigLIDs)
                {
                    //Changing unselected energy to display as "None" for consistency with the CE UI.
                    if (UNASSIGNED != EnergyLid.lidDescription && false == string.IsNullOrEmpty(EnergyLid.lidDescription))
                    {
                        EnergyConfigList.Add(EnergyLid.lidDescription);
                    }
                    else
                    {
                        EnergyConfigList.Add(Resources.NONE);
                    }
                }

                return EnergyConfigList;
            }

        }

        /// <summary>
        /// Gets the list of configured Demands
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/31/16 jrf 4.70.28 230427 Modified unselected demand text to be consistent with CE UI.
        public List<string> DemandConfigList
        {
            get
            {
                List<String> DemandConfigList = new List<String>();

                foreach (LID DemandLID in DemandConfigLIDs)
                {
                    //Changing unselected demand to display as "None" for consistency with the CE UI.
                    if (UNASSIGNED != DemandLID.lidDescription && false == string.IsNullOrEmpty(DemandLID.lidDescription))
                    {
                        DemandConfigList.Add(DemandLID.lidDescription);
                    }
                    else
                    {
                        DemandConfigList.Add(Resources.NONE);
                    }
                }

                return DemandConfigList;
            }
        }

        /// <summary>
        /// Gets the list of Extended Energy LIDs configured into the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/19/12 RCG 2.53.32 TRQ3439 Created 
        
        public List<LID> ExtendedEnergyConfigLIDs
        {
            get
            {
                object objValue;
                List<LID> ConfigLIDs = new List<LID>();

                for (int CurrentIndex = 0; CurrentIndex < DetermineExtendedEnergyConfigCount(); CurrentIndex++ )
                {
                    int[] Indexer = new int[] { CurrentIndex };

                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217NonBillableEnergyId, Indexer))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl217NonBillableEnergyId, Indexer, out objValue);

                        if (objValue != null)
                        {
                            byte byEnergyID = (byte)objValue;

                            // This value may have been initialized to 0xFF which is an invalid item
                            if (byEnergyID == 0xFF)
                            {
                                byEnergyID = 0;
                            }

                            uint LIDValue = SEC_ENERGY_LID_BASE + (byte)objValue;

                            ConfigLIDs.Add(new CentronAMILID(LIDValue));
                        }
                    }
                }

                return ConfigLIDs;
            }
        }

        /// <summary>
        /// Gets the list of configured Extended Energy quantites as a string;
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/19/12 RCG 2.53.32 TRQ3439 Created 
        // 10/31/16 jrf 4.70.28 230427 Modified unselected energy text to be consistent with CE UI.
        public List<string> ExtendedEnergyConfigList
        {
            get
            {
                List<string> ConfigList = new List<string>();

                foreach (LID CurrentLID in ExtendedEnergyConfigLIDs)
                {
                    //Changing unselected energy to display as "None" for consistency with the CE UI.
                    if (UNASSIGNED != CurrentLID.lidDescription && false == string.IsNullOrEmpty(CurrentLID.lidDescription))
                    {
                        ConfigList.Add(CurrentLID.lidDescription);
                    }
                    else
                    {
                        ConfigList.Add(Resources.NONE);
                    }
                }

                return ConfigList;
            }
        }

        /// <summary>
        /// Gets the list of configured Demand Thresholds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created
        //
        public List<string> DemandThresholdQuantityList
        {
            get
            {
                List<String> DemandThresholdList = new List<String>();

                foreach (LID DemandLID in DemandThresholdLIDs)
                {
                    DemandThresholdList.Add(DemandLID.lidDescription);
                }

                return DemandThresholdList;
            }
        }

        /// <summary>
        /// Gets the list of Load Profile Quantities
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/31/16 jrf 4.70.28 230427 Modified unselected quantity text to be consistent with CE UI.
        public virtual List<string> LPQuantityList
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                CentronAMILID LPQuantityLID;
                List<String> LPConfigList = new List<String>();

                for (i = 0; i < NumberLPChannels; i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_LP_LID,
                        anIndex1, out Value);
                    LPQuantityLID = new CentronAMILID((uint)Value);
                    

                    //Changing unselected quantity to display as "None" for consistency with the CE UI.
                    if (UNASSIGNED != LPQuantityLID.lidDescription && false == string.IsNullOrEmpty(LPQuantityLID.lidDescription))
                    {
                        LPConfigList.Add(LPQuantityLID.lidDescription);
                    }
                    else
                    {
                        LPConfigList.Add(Resources.NONE);
                    }
                }

                return LPConfigList;
            }
        }

        /// <summary>
        /// Gets/sets a list of Load Profile Quantity LIDs.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created
        //  11/05/10 jrf 2.45.11        Added setting of unused channels to default values.
        //
        public virtual List<LID> LPQuantityLIDs
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                LID LPQuantityLID;
                List<LID> LPConfigList = new List<LID>();

                for (i = 0; i < NumberLPChannels; i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_LP_LID,
                        anIndex1, out Value);
                    LPQuantityLID = new LID((uint)Value);
                    LPConfigList.Add(LPQuantityLID);
                }

                return LPConfigList;
            }
            set
            {
                object Value = null;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays

                NumberLPChannels = value.Count;

                for (i = 0; i < NumberLPChannels; i++)
                {
                    Value = value[i].lidValue;
                    anIndex1[0] = i;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_LP_LID,
                        anIndex1, Value);
                }

                for (i = NumberLPChannels; i < DetermineMaximumLPChannels(); i++)
                {
                    Value = 0;
                    anIndex1[0] = i;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_LP_LID,
                        anIndex1, Value);
                }
            }
        }

        /// <summary>
        /// Gets the list of Load Profile Pulse Weight
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.11        Added set.
        //
        public virtual List<float> LPPulseWeightList
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                float fltPulseWeight;

                List<float> LPPulseWeightList = new List<float>();

                for (i = 0; i < NumberLPChannels; i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                        anIndex1, out Value);
                    fltPulseWeight = float.Parse(Value.ToString(), CultureInfo.InvariantCulture);
                    LPPulseWeightList.Add((float)(fltPulseWeight * 0.01));
                }

                return LPPulseWeightList;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays

                for (i = 0; i < NumberLPChannels; i++)
                {
                    anIndex1[0] = i;
                    int iPulseWeight = Convert.ToInt32(value[i] / 0.01);
                    Value = iPulseWeight;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                        anIndex1, Value);
                }

                for (i = NumberLPChannels; i < DetermineMaximumLPChannels(); i++)
                {
                    anIndex1[0] = i;
                    Value = 100;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                        anIndex1, Value);
                }
            }
        }

        /// <summary>
        /// Gets the list of sources used by the non metrological data tables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/11 RCG 2.53.20        Created

        public List<LID> NonMetrologicalSourceLIDs
        {
            get
            {
                List<LID> Sources = new List<LID>();

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl341_NBR_SOURCES, null))
                {
                    byte NumberOfSources = 0;
                    object Value = null;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl341_NBR_SOURCES, null, out Value);

                    if (Value != null)
                    {
                        NumberOfSources = (byte)Value;
                    }

                    if (NumberOfSources > 0 && m_CenTables.IsAllCached(2392))
                    {
                        for (int SourceIndex = 0; SourceIndex < NumberOfSources; SourceIndex++)
                        {
                            int[] Indexer = new int[] { SourceIndex };

                            m_CenTables.GetValue(CentronTblEnum.MfgTbl344_SOURCE_ID, Indexer, out Value);

                            // This value is returned as an array of 4 bytes so we need to pull them out into a uint
                            MemoryStream DataStream = new MemoryStream((byte[])Value);
                            PSEMBinaryReader DataReader = new PSEMBinaryReader(DataStream);

                            Sources.Add(new CentronAMILID(DataReader.ReadUInt32()));
                        }
                    }
                }

                return Sources;
            }
        }

        /// <summary>
        /// Gets the names of each source configured in the Non Metrological Source Tables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/11 RCG 2.53.20        Created

        public List<string> NonMetrologicalSourceNames
        {
            get
            {
                List<string> Names = new List<string>();

                foreach (LID CurrentLID in NonMetrologicalSourceLIDs)
                {
                    Names.Add(CurrentLID.lidDescription);
                }

                return Names;
            }
        }

        /// <summary>
        /// History Log Event Summary List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Read the event config out of standard table 73
        //                              for the M2 Gateway
        //
        public List<MFG2048EventItem> HistoryLogEventList
        {
            get
            {
                List<MFG2048EventItem> HistConfigItems = null;

                if (DeviceType == EDLDeviceTypes.M2GatewayDevice)
                {
                    HistConfigItems = Table73.HistoryLogEventList;
                }
                else if (HistoryConfig != null)
                {
                    HistConfigItems = HistoryConfig.HistoryLogEventList;
                }

                return HistConfigItems;
            }
        }

        /// <summary>
        /// History Log Configuration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Read the event config out of standard table 73
        //                              for the M2 Gateway
        //  06/23/10 AF  2.41.11        Added check for null before reading Table 73
        //  10/22/10 AF  2.45.06 161866 M2 Gateway event config is now stored in 2048
        //  03/12/12 jrf 2.53.49 192582/192583 Modified to return the HistoryLogEventList
        //                              so the Datafile Viewer and Field-Pro are consistent.
        //  07/03/13 AF  2.80.45 TR7640 The history log configuration is not in 2048 for the I-210 or kV2c
        //
        public List<MFG2048EventItem> HistoryLogConfiguration
        {
            get
            {
                List<MFG2048EventItem> HistConfigItems = null;

                if (DeviceType == EDLDeviceTypes.ICSGatewayDevice)
                {
                    // No event configuration in table 2048 for the I-210 or kV2c
                    HistConfigItems = null;
                }
                else if (DeviceType != EDLDeviceTypes.M2GatewayDevice)
                {
                    if (HistoryConfig != null)
                    {
                        HistConfigItems = HistoryConfig.HistoryLogEventList;
                    }
                }
#if (!WindowsCE)
                else if (M2_Gateway_HistoryConfig != null)
                {
                    HistConfigItems = M2_Gateway_HistoryConfig.HistoryConfiguration;
                }
#endif
                return HistConfigItems;
            }
        }

        /// <summary>
        /// Returns the Full Configuration Header Object
        /// </summary>
        public virtual CTable2048Header ConfigHeader
        {
            get
            {
                Stream strm2048Header = new MemoryStream();

                // Get a list of Stream data from the EDL file
                IList<TableData> lstTableData = m_CenTables.BuildPSEMStreams((long)CentronTblEnum.MFGTBL0_DATA_SIZE, null, (long)CentronTblEnum.MFGTBL0_SUBTABLE_OFFSETS, null);

                //Assembly the multiple streams that I received to one stream that I can use.
                strm2048Header = BuildStream(lstTableData, 0, CTable2048Header.HEADER_LENGTH_2048);
                // Now that I have a stream, I can create my Binary Reader
                PSEMBinaryReader EDLReader = new PSEMBinaryReader(strm2048Header);
                //Finally, I can send the Binary Reader to the 2048 class, where it can be used to read the header data.
                CTable2048Header ConfigurationHeader = new CTable2048Header(EDLReader);

                return ConfigurationHeader;
            }
        }

        /// <summary>
        /// Gets the Offset for the History Log Configuration in 2048
        /// </summary>
        public uint HistoryConfigOffset
        {
            get
            {
                return (uint)ConfigHeader.HistoryLogOffset;
            }
        }

        /// <summary>
        /// Get the length of the DST Switch (minutes)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual int DSTSwitchLength
        {
            get
            {
                if (!m_iDSTLength.Cached)
                {
                    if (m_CenTables.IsAllCached(2090))
                    {
                        m_iDSTLength.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL42_DST_OFFSET);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsAllCached(2090))
                    {
                        m_iDSTLength.Value = GetMFGEDLInt(GatewayTblEnum.MFGTBL42_DST_OFFSET);
                    }
#endif
                    else
                    {
                        m_iDSTLength.Value = 0;
                    }
                }

                return m_iDSTLength.Value;
            }
        }

        /// <summary>
        /// Gets whether or not Voltage Monitoring is enabled in the EDL file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created
        // 12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        // 08/01/12 jrf 2.70.01 197895  Updated to handle case where enhanced Voltage monitoring 
        //                              has never been configured.
        //
        public virtual bool VMEnabled
        {
            get
            {
                bool bEnabled = false;
                object objValue;

                if (true == ContainsEnhancedVoltageMonitoringConfig && 
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL106_ENABLE_FLAG, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL106_ENABLE_FLAG, null, out objValue);
                    bEnabled = (bool)objValue;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_ENABLE_FLAG, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, out objValue);
                    bEnabled = (bool)objValue;
                }

                return bEnabled;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets whether or not Legacy Voltage Monitoring is enabled in the EDL file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public virtual bool? LegacyVMEnabled
        {
            get
            {
                bool? bEnabled = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_ENABLE_FLAG, null) == true)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, out objValue);
                        bEnabled = (bool)objValue;
                    }
                }
                catch
                {
                    bEnabled = null;
                }

                return bEnabled;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets whether or not Enhanced Voltage Monitoring is enabled in the EDL file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public virtual bool? EnhancedVMEnabled
        {
            get
            {
                bool? bEnabled = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL106_ENABLE_FLAG, null) == true)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL106_ENABLE_FLAG, null, out objValue);
                        bEnabled = (bool)objValue;
                    }
                }
                catch
                {
                    bEnabled = null;
                }

                return bEnabled;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the number of phases used for Voltage Monitoring.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created
        // 12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        // 08/01/12 jrf 2.70.01 197895  Updated to handle case where enhanced Voltage monitoring 
        //                              has never been configured.
        //
        public virtual byte VMNumPhases
        {
            get
            {
                byte byNumPhases = 0;
                object objValue;

                if (true == ContainsEnhancedVoltageMonitoringConfig &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL108_MONITORING_PHASES, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL108_MONITORING_PHASES, null, out objValue);
                    byNumPhases = (byte)objValue;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL101_NBR_PHASES, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL101_NBR_PHASES, null, out objValue);
                    byNumPhases = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return byNumPhases;
            }
        }

        /// <summary>
        /// Gets the number of phases used for legacy Voltage Monitoring.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public virtual byte? LegacyVMNumPhases
        {
            get
            {
                byte? byNumPhases = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL101_NBR_PHASES, null) == true)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL101_NBR_PHASES, null, out objValue);
                        byNumPhases = (byte)objValue;
                    }
                }
                catch
                {
                    byNumPhases = null;
                }

                return byNumPhases;
            }
        }

        /// <summary>
        /// Gets the Voltage Monitoring interval length
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created
        // 12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        // 08/01/12 jrf 2.70.01 197895  Updated to handle case where enhanced Voltage monitoring 
        //                              has never been configured.
        //
        public virtual TimeSpan VMIntervalLength
        {
            get
            {
                byte byMinutes = 0;
                object objValue;

                if (true == ContainsEnhancedVoltageMonitoringConfig &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL105_VM_INT_LEN, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL105_VM_INT_LEN, null, out objValue);
                    byMinutes = (byte)objValue;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL101_VM_INT_LEN, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL101_VM_INT_LEN, null, out objValue);
                    byMinutes = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return TimeSpan.FromMinutes((double)byMinutes);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the legacy Voltage Monitoring interval length
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public virtual byte? LegacyVMIntervalLength
        {
            get
            {
                byte? byMinutes = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL101_VM_INT_LEN, null) == true)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL101_VM_INT_LEN, null, out objValue);
                        byMinutes = (byte)objValue;
                    }
                }
                catch
                {
                    byMinutes = null;
                }

                return byMinutes;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the enhanced Voltage Monitoring interval length
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public virtual byte? EnhancedVMIntervalLength
        {
            get
            {
                byte? byMinutes = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL101_VM_INT_LEN, null) == true)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL101_VM_INT_LEN, null, out objValue);
                        byMinutes = (byte)objValue;
                    }
                }
                catch
                {
                    byMinutes = null;
                }

                return byMinutes;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determine the Percentage of Nominal for VMVhLowThreshold
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#            Description
        // -------- --- ------- --------------- ---------------------------------------
        // 07/16/08 KRC	1.51.05	 itron00116930  Created
        // 02/18/11 RCG 2.50.04                 Adding support for ITRD, ITRE, ITRF meters
        // 12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        // 08/01/12 jrf 2.70.01 197895  Updated to handle case where enhanced Voltage monitoring 
        //                              has never been configured.
        // 08/03/15 AF  4.20.19 586155          Added ITRK to list of poly meters and call IsPolyEDL
        //
        public virtual ushort VMVhLowPercentage
        {
            get
            {
                ushort usPercentage = 0;
                object objValue;

                //Extended VM always stores the Vh low threshold as a percentage.
                if (true == ContainsEnhancedVoltageMonitoringConfig &&
                    m_CenTables.IsTableKnown(2154) && m_CenTables.IsAllCached(2154))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL106_VH_LOW_THRESHOLD_PERCENT, null, out objValue);
                    usPercentage = (ushort)objValue;
                }
                else
                {
                    if (IsPolyEDL())
                    {
                        if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null))
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null, out objValue);
                            usPercentage = (ushort)objValue;
                        }
                    }
                    else
                    {
                        float fltLowVhPerHour = (float)(60 / VMIntervalLength.TotalMinutes) * VMVhLowThreshold;
                        float fltPercentage = (fltLowVhPerHour / (float)DetermineNominalVoltage());
                        usPercentage = (ushort)Math.Round(fltPercentage * 100);
                    }
                }

                return usPercentage;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determine the legacy Percentage of Nominal for VMVhLowThreshold
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        // 08/03/15 AF  4.20.19 WR 586155 Added ITRK to list of poly meters and call IsPolyEDL
        //
        public virtual ushort? LegacyVMVhLowPercentage
        {
            get
            {
                ushort? usPercentage = null;
                object objValue;

                try
                {
                    if (IsPolyEDL())
                    {
                        if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null))
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null, out objValue);
                            usPercentage = (ushort)objValue;
                        }
                    }
                    else
                    {
                        if (null != LegacyVMIntervalLength && 0 != LegacyVMIntervalLength)
                        {
                            float fltLowVhPerHour = (float)(60 / (float)LegacyVMIntervalLength) * VMVhLowThreshold;
                            float fltPercentage = (fltLowVhPerHour / (float)DetermineNominalVoltage());
                            usPercentage = (ushort)Math.Round(fltPercentage * 100);
                        }
                    }
                }
                catch 
                {
                    usPercentage = null;
                }                                

                return usPercentage;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determine the enhanced Percentage of Nominal for VMVhLowThreshold
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public virtual ushort? EnhancedVMVhLowPercentage
        {
            get
            {
                ushort? usPercentage = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL106_VH_LOW_THRESHOLD_PERCENT, null) == true)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL106_VH_LOW_THRESHOLD_PERCENT, null, out objValue);
                        usPercentage = (ushort)objValue;
                    }
                }
                catch
                {
                    usPercentage = null;
                }

                return usPercentage;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determine the Percentage of Nominal for VMVhHighThreshold
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#            Description
        // -------- --- ------- --------------- ---------------------------------------
        // 07/16/08 KRC	1.51.05	 itron00116930  Created
        // 02/18/11 RCG 2.50.04                 Adding support for ITRD, ITRE, ITRF meters
        // 12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        // 08/01/12 jrf 2.70.01 197895  Updated to handle case where enhanced Voltage monitoring 
        //                              has never been configured.
        // 08/03/15 AF  4.20.19 586155  Added ITRK to list of poly meters and call IsPolyEDL
        //
        public virtual ushort VMVhHighPercentage
        {
            get
            {
                ushort usPercentage = 0;
                object objValue;

                //Extended VM always stores the Vh high threshold as a percentage.
                if (true == ContainsEnhancedVoltageMonitoringConfig &&
                    m_CenTables.IsTableKnown(2154) && m_CenTables.IsAllCached(2154))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL106_VH_HIGH_THRESHOLD_PERCENT, null, out objValue);
                    usPercentage = (ushort)objValue;
                }
                else
                {
                    if (IsPolyEDL())
                    {
                        if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null))
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null, out objValue);
                            usPercentage = (ushort)objValue;
                        }
                    }
                    else
                    {
                        float fltHighVhPerHour = (float)(60.0 / VMIntervalLength.TotalMinutes) * VMVhHighThreshold;
                        float fltPercentage = (fltHighVhPerHour / (float)DetermineNominalVoltage());
                        usPercentage = (ushort)Math.Round(fltPercentage * 100);
                    }
                }

                return usPercentage;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determine the legacy Percentage of Nominal for VMVhHighThreshold
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        // 08/03/15 AF  4.20.19 WR 586155 Added ITRK to list of poly meters and call IsPolyEDL
        //
        public virtual ushort? LegacyVMVhHighPercentage
        {
            get
            {
                ushort? usPercentage = null;
                object objValue;

                try
                {
                    if (IsPolyEDL())
                    {
                        if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null))
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null, out objValue);
                            usPercentage = (ushort)objValue;
                        }
                    }
                    else
                    {
                        if (null != LegacyVMIntervalLength && 0 != LegacyVMIntervalLength)
                        {
                            float fltHighVhPerHour = (float)(60 / (float)LegacyVMIntervalLength) * VMVhHighThreshold;
                            float fltPercentage = (fltHighVhPerHour / (float)DetermineNominalVoltage());
                            usPercentage = (ushort)Math.Round(fltPercentage * 100);
                        }
                    }
                }
                catch
                {
                    usPercentage = null;
                }

                return usPercentage;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determine the enhanced Percentage of Nominal for VMVhHighThreshold
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public virtual ushort? EnhancedVMVhHighPercentage
        {
            get
            {
                ushort? usPercentage = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL106_VH_HIGH_THRESHOLD_PERCENT, null) == true)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL106_VH_HIGH_THRESHOLD_PERCENT, null, out objValue);
                        usPercentage = (ushort)objValue;
                    }
                }
                catch
                {
                    usPercentage = null;
                }

                return usPercentage;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the Low RMS Threshold for Voltage Monitoring
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        public virtual ushort VMRMSLowThreshold
        {
            get
            {
                ushort usThreshold = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD, null, out objValue);
                    usThreshold = (ushort)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return usThreshold;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the High RMS Threshold for Voltage Monitoring
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        public virtual ushort VMRMSHighThreshold
        {
            get
            {
                ushort usThreshold = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_RMS_VOLT_HIGH_THRESHOLD, null, out objValue);
                    usThreshold = (ushort)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return usThreshold;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the RMS low threshold as a percentage.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/09 RCG 2.20.05 N/A    Created
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        //  12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        // 08/01/12 jrf 2.70.01 197895  Updated to handle case where enhanced Voltage monitoring 
        //                              has never been configured.
        // 08/03/15 AF  4.20.19 586155  Added ITRK to list of poly meters and call IsPolyEDL
        //
        public ushort VMRMSLowPercentage
        {
            get
            {
                ushort Value = 0;
                object objValue;

                //Extended VM always stores the RMS volt low threshold as a percentage.
                if (true == ContainsEnhancedVoltageMonitoringConfig &&
                    m_CenTables.IsTableKnown(2154) && m_CenTables.IsAllCached(2154))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL106_RMS_VOLT_LOW_THRESHOLD_PERCENT, null, out objValue);
                    Value = (ushort)objValue;
                }
                else
                {
                    if (IsPolyEDL())
                    {
                        // The value stored in the meter is the percentage.
                        Value = VMRMSLowThreshold;
                    }
                    else
                    {
                        // The value in the meter is an actual value so we need to convert to a percentage.
                        float fPercentage = (VMRMSLowThreshold / (float)DetermineNominalVoltage());
                        Value = (ushort)Math.Round(fPercentage * 100);
                    }
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the legacy RMS low threshold as a percentage.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        // 08/03/15 AF  4.20.19 WR 586155 Added ITRK to list of poly meters and call IsPolyEDL
        //
        public ushort? LegacyVMRMSLowPercentage
        {
            get
            {
                ushort? Value = null;

                try
                {
                    if (IsPolyEDL())
                    {
                        // The value stored in the meter is the percentage.
                        Value = VMRMSLowThreshold;
                    }
                    else
                    {
                        // The value in the meter is an actual value so we need to convert to a percentage.
                        float fPercentage = (VMRMSLowThreshold / (float)DetermineNominalVoltage());
                        Value = (ushort)Math.Round(fPercentage * 100);
                    }
                }
                catch
                {
                    Value = null;
                }
                

                return Value;
            }
        }

        /// <summary>
        /// Gets the enhanced RMS low threshold as a percentage.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public ushort? EnhancedVMRMSLowPercentage
        {
            get
            {
                ushort? Value = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL106_RMS_VOLT_LOW_THRESHOLD_PERCENT, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL106_RMS_VOLT_LOW_THRESHOLD_PERCENT, null, out objValue);
                        Value = (ushort)objValue;
                    }
                }
                catch
                {
                    Value = null;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the RMS high threshold as a percentage.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/09 RCG 2.20.05 N/A    Created
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        //  12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        //  08/01/12 jrf 2.70.01 197895  Updated to handle case where enhanced Voltage monitoring 
        //                               has never been configured.
        // 08/03/15 AF  4.20.19  586155  Added ITRK to list of poly meters and call IsPolyEDL
        //
        public ushort VMRMSHighPercentage
        {
            get
            {
                ushort Value = 0;
                object objValue;

                //Extended VM always stores the RMS volt high threshold as a percentage.
                if (true == ContainsEnhancedVoltageMonitoringConfig &&
                    m_CenTables.IsTableKnown(2154) && m_CenTables.IsAllCached(2154))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL106_RMS_VOLT_HIGH_THRESHOLD_PERCENT, null, out objValue);
                    Value = (ushort)objValue;
                }
                else
                {
                    if (IsPolyEDL())
                    {
                        // The value stored in the meter is the percentage.
                        Value = VMRMSHighThreshold;
                    }
                    else
                    {
                        // The value in the meter is an actual value so we need to convert to a percentage.
                        float fPercentage = (VMRMSHighThreshold / (float)DetermineNominalVoltage());
                        Value = (ushort)Math.Round(fPercentage * 100);
                    }
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the legacy RMS high threshold as a percentage.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        // 08/03/15 AF  4.20.19 WR 586155 Added ITRK to list of poly meters and call IsPolyEDL
        //
        public ushort? LegacyVMRMSHighPercentage
        {
            get
            {
                ushort? Value = null;

                try
                {
                    if (IsPolyEDL())
                    {
                        // The value stored in the meter is the percentage.
                        Value = VMRMSHighThreshold;
                    }
                    else
                    {
                        // The value in the meter is an actual value so we need to convert to a percentage.
                        float fPercentage = (VMRMSHighThreshold / (float)DetermineNominalVoltage());
                        Value = (ushort)Math.Round(fPercentage * 100);
                    }
                }
                catch
                {
                    Value = null;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the enhanced RMS high threshold as a percentage.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 458316 Created.
        //
        public ushort? EnhancedVMRMSHighPercentage
        {
            get
            {
                ushort? Value = null;
                object objValue;

                try
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL106_RMS_VOLT_HIGH_THRESHOLD_PERCENT, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL106_RMS_VOLT_HIGH_THRESHOLD_PERCENT, null, out objValue);
                        Value = (ushort)objValue;
                    }
                }
                catch
                {
                    Value = null;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Voltage High/Low Alarm Latency for Voltage Monitoring data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 RCG 2.53.38 N/A    Created
        //  08/01/12 jrf 2.70.01 197895 Updated to handle case where enhanced Voltage monitoring 
        //                              has never been configured.
        //
        public byte? VMRMSAlarmLatency
        {
            get
            {
                byte? byValue = null;
                object objValue = null;

                if (true == ContainsEnhancedVoltageMonitoringConfig &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL105_VRMS_ALARM_MIN_SEC, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL105_VRMS_ALARM_MIN_SEC, null, out objValue);
                    byValue = (byte)objValue;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets whether or not user intervention is required after a connection.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual bool SLConnectsUsingUserIntervention
        {
            get
            {
                bool bUsesIntervention = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_CONNECT_WITH_USER_INTERVENTION_FLAG, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_CONNECT_WITH_USER_INTERVENTION_FLAG, null, out objValue);
                    bUsesIntervention = (bool)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return bUsesIntervention;
            }
        }

        /// <summary>
        /// Gets whether or not the Override Connect/Disconnect Switch is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue#             Description
        //  -------- --- ------- ---------------  -------------------------------------------
        //  07/03/08 KRC 1.51.01  itron00116660    Created
        //
        public virtual bool SLOverrideSwitch
        {
            get
            {
                bool bOverrideSwitch = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL94_OVERRIDE_FLAG, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL94_OVERRIDE_FLAG, null, out objValue);
                    bOverrideSwitch = (bool)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return bOverrideSwitch;
            }
        }

        /// <summary>
        /// Gets the maximum number of disconnects allowed in the configured period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual byte SLMaxDisconnects
        {
            get
            {
                byte byMax = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_MAX_SWITCH_COUNT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_MAX_SWITCH_COUNT, null, out objValue);
                    byMax = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return byMax;
            }
        }

        /// <summary>
        /// Gets the period of time when the alarm will be raised after a disconnect.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual TimeSpan SLDisconnectRandomizationAlarmPeriod
        {
            get
            {
                DateTime dtValue;
                TimeSpan tsPeriod = new TimeSpan();
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_RANDOMIZATION_ALARM, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_RANDOMIZATION_ALARM, null, out objValue);
                    dtValue = (DateTime)objValue;
                    tsPeriod = dtValue.TimeOfDay;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsPeriod;
            }
        }

        /// <summary>
        /// Gets the minimum amount of time to wait before reconnecting.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual TimeSpan SLReconnectStartDelay
        {
            get
            {
                DateTime dtValue;
                TimeSpan tsDelay = new TimeSpan();
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_RESTORATION_START_DELAY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_RESTORATION_START_DELAY, null, out objValue);
                    dtValue = (DateTime)objValue;
                    tsDelay = dtValue.TimeOfDay;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsDelay;
            }
        }

        /// <summary>
        /// Gets the period of time where the meter will be reconnected a
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual TimeSpan SLReconnectRandomDelay
        {
            get
            {
                DateTime dtValue;
                TimeSpan tsDelay = new TimeSpan();
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_RESTORATION_RANDOM_DELAY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_RESTORATION_RANDOM_DELAY, null, out objValue);
                    dtValue = (DateTime)objValue;
                    tsDelay = dtValue.TimeOfDay;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsDelay;
            }
        }

        /// <summary>
        /// Gets the amount of time the switch will remain open after a service limiting disconnect.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual TimeSpan SLDisconnectOpenDelay
        {
            get
            {
                DateTime dtValue;
                TimeSpan tsDelay = new TimeSpan();
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_OPEN_TIME, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_OPEN_TIME, null, out objValue);
                    dtValue = (DateTime)objValue;
                    tsDelay = dtValue.TimeOfDay;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsDelay;
            }
        }

        /// <summary>
        /// Gets the number of Service Limiting disconnect retry attempts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/02/09 RCG 2.30.16 N/A    Created

        public virtual byte SLRetryAttemtps
        {
            get
            {
                byte byValue = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_RETRY_ATTEMPTS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_RETRY_ATTEMPTS, null, out objValue);
                    byValue = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the amount of time the failsafe is enabled after a failsafe event occurs.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/02/09 RCG 2.30.16 N/A    Created

        public virtual TimeSpan SLFailsafeDuration
        {
            get
            {
                ushort usDuration = 0;
                TimeSpan tsDuration = new TimeSpan();
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl95FailsafeDuration, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl95FailsafeDuration, null, out objValue);
                    usDuration = (ushort)objValue;
                    tsDuration = TimeSpan.FromMinutes(usDuration);
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsDuration;
            }
        }

        /// <summary>
        /// Gets the amount of minutes configured for the Load Side Voltage Delay.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  10/04/13 DLG 3.00.11 WR327168  Created.
        //  
        public virtual ushort SLLoadVoltageDetectionDelay
        {
            get
            {
                ushort usMinutes;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212LoadVoltageDetectionDelay, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212LoadVoltageDetectionDelay, null, out objValue);
                    usMinutes = (ushort)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return usMinutes;
            }
        }

        /// <summary>
        /// Gets the quantity for the normal mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual string SLNormalModeThresholdQuantity
        {
            get
            {
                string strDemand = "None";
                int[] iaThresholdIndex = { 0 };
                byte byIndex;
                byte byNumThresholds;
                object objValue;
                CentronAMILID LID;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex) &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null, out objValue);
                    byNumThresholds = (byte)objValue;

                    if (byNumThresholds > 0)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex, out objValue);
                        byIndex = (byte)objValue;

                        if (byIndex != 255)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, new int[] { byIndex }, out objValue);
                            LID = new CentronAMILID((uint)objValue);

                            strDemand = LID.lidDescription;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return strDemand;
            }
        }

        /// <summary>
        /// Gets the threshold value for normal mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual double SLNormalModeThreshold
        {
            get
            {
                double dThreshold = 0.0;
                object objValue;
                int[] iaThresholdIndex = { 0 };

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex, out objValue);
                    dThreshold = (double)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return dThreshold;
            }
        }

        /// <summary>
        /// Gets the quantity for the critical mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual string SLCriticalModeThresholdQuantity
        {
            get
            {
                string strDemand = "None";
                int[] iaThresholdIndex = { 1 };
                byte byIndex;
                byte byNumThresholds;
                object objValue;
                CentronAMILID LID;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex) &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null, out objValue);
                    byNumThresholds = (byte)objValue;

                    if (byNumThresholds > 1)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex, out objValue);
                        byIndex = (byte)objValue;

                        if (byIndex != 255)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, new int[] { byIndex }, out objValue);
                            LID = new CentronAMILID((uint)objValue);

                            strDemand = LID.lidDescription;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return strDemand;
            }
        }

        /// <summary>
        /// Gets the threshold value for critical mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual double SLCriticalModeThreshold
        {
            get
            {
                double dThreshold = 0.0;
                object objValue;
                int[] iaThresholdIndex = { 1 };

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex, out objValue);
                    dThreshold = (double)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return dThreshold;
            }
        }

        /// <summary>
        /// Gets the quantity for the emergency mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual string SLEmergencyModeThresholdQuantity
        {
            get
            {
                string strDemand = "None";
                int[] iaThresholdIndex = { 2 };
                byte byIndex;
                byte byNumThresholds;
                object objValue;
                CentronAMILID LID;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex) &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null, out objValue);
                    byNumThresholds = (byte)objValue;

                    if (byNumThresholds > 2)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex, out objValue);
                        byIndex = (byte)objValue;

                        if (byIndex != 255)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, new int[] { byIndex }, out objValue);
                            LID = new CentronAMILID((uint)objValue);

                            strDemand = LID.lidDescription;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return strDemand;
            }
        }

        /// <summary>
        /// Gets the threshold value for emergency mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual double SLEmergencyModeThreshold
        {
            get
            {
                double dThreshold = 0.0;
                object objValue;
                int[] iaThresholdIndex = { 2 };

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex, out objValue);
                    dThreshold = (double)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return dThreshold;
            }
        }

        /// <summary>
        /// Gets the number of optical login attempts before a lockout.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        // 11/05/10 jrf 2.45.11        Added set.
        //
        public virtual byte LockoutOpticalAttempts
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#endif
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
            set
            {
                Object objValue = value;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL, null, objValue);
            }
        }

        /// <summary>
        /// Gets the optical lockout time
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        // 11/05/10 jrf 2.45.11        Added set.
        //
        public virtual byte LockoutOpticalMinutes
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#endif
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
            set
            {
                Object objValue = value;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL, null, objValue);
            }
        }

        /// <summary>
        /// Gets the number of LAN attempts before a lockout.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual byte LockoutLanAttempts
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#endif
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
        }

        /// <summary>
        /// Gets the LAN lockout time
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual byte LockoutLanMinutes
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#endif
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
        }

        /// <summary>
        /// Gets the LAN message failure limit.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual byte LanMessageFailureLimit
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT, null, out objValue);
                    byAttempts = (byte)objValue;
                }
#endif
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
        }

        /// <summary>
        /// Gets the LAN link metric period
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual ushort LanLinkMetricPeriod
        {
            get
            {
                ushort usSeconds = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS, null, out objValue);
                    usSeconds = (ushort)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS, null, out objValue);
                    usSeconds = (ushort)objValue;
                }
#endif
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return usSeconds;
            }
        }

        /// <summary>
        /// Gets the RFLAN neighbor list.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/05/08 jrf 1.50.31 114519 Created.
        // 06/06/08 jrf 1.50.32 114519 Added check to make sure EDL contains RFLAN 
        //                             neighbors before trying to read them.
        public List<RFLANNeighborEntryRcd> RFLANNeighbors
        {
            get
            {
                List<RFLANNeighborEntryRcd> Records = null;

                if (ContainsRFLANNeighbors && Table2078 != null)
                {
                    Records = Table2078.Neighbors;
                }

                return Records;
            }
        }

        /// <summary>
        /// Gets the exception security model for the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/08 RCG 2.00.02        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public virtual OpenWayMFGTable2193.SecurityFormat? ExceptionSecurityModel
        {
            get
            {
                OpenWayMFGTable2193.SecurityFormat? SecurityFormat = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL, null, out objValue);
                    SecurityFormat = (OpenWayMFGTable2193.SecurityFormat)(byte)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL, null, out objValue);
                    SecurityFormat = (OpenWayMFGTable2193.SecurityFormat)(byte)objValue;
                }
#endif

                return SecurityFormat;
            }
        }

        /// <summary>
        /// Gets whether or not enhanced security is required.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/08 RCG 2.00.02        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public virtual bool? IsEnhancedSecurityRequired
        {
            get
            {
                bool? bRequiresEnhancedSecurity = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY, null, out objValue);
                    bRequiresEnhancedSecurity = (bool)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY, null, out objValue);
                    bRequiresEnhancedSecurity = (bool)objValue;
                }
#endif
                return bRequiresEnhancedSecurity;
            }
        }

        /// <summary>
        /// Gets whether or not C12.18 communications are enabled over ZigBee.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/03/09 RCG 2.21.06        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public virtual bool? IsC1218OverZigBeeEnabled
        {
            get
            {
                bool? bC1218OverZigBeeEnabled = null;
                object objValue;

#if (!WindowsCE)
                if (m_GatewayTables != null && m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl145C1218OverZigBee, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MfgTbl145C1218OverZigBee, null, out objValue);
                    bC1218OverZigBeeEnabled = (bool)objValue;
                }
                else if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5) < 0
                    || (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5) == 0 && FirmwareBuild < 56)))
                {
                    // This is a data file and it was created with a meter prior to FW 2.5.56 so
                    // we should always report this as true since it was always enabled in this FW
                    bC1218OverZigBeeEnabled = true;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145C1218OverZigBee, null))
                {
                    // This is a program file that has the value cached (created with SP5 CE) or 
                    // this is a data file created on a meter 2.5.56 or later.
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145C1218OverZigBee, null, out objValue);
                    bC1218OverZigBeeEnabled = (bool)objValue;
                }
#else
                if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5) < 0
                    || (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5) == 0 && FirmwareBuild < 56)))
                {
                    // This is a data file and it was created with a meter prior to FW 2.5.56 so
                    // we should always report this as true since it was always enabled in this FW
                    bC1218OverZigBeeEnabled = true;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145C1218OverZigBee, null))
                {
                    // This is a program file that has the value cached (created with SP5 CE) or 
                    // this is a data file created on a meter 2.5.56 or later.
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145C1218OverZigBee, null, out objValue);
                    bC1218OverZigBeeEnabled = (bool)objValue;
                }
#endif

                return bC1218OverZigBeeEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee Private Profile is Enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/09 MMD 2.30.15        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public virtual bool? DisableZigbeePrivateProfile
        {
            get
            {
                bool? bDisableZigBeePrivateProfile = null;
                object objValue;

#if (!WindowsCE)
                if (m_GatewayTables != null && m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl145DisableZigBeePrivateProfile, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MfgTbl145DisableZigBeePrivateProfile, null, out objValue);
                    bDisableZigBeePrivateProfile = (bool)objValue;
                }
                else if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5_1) < 0))
                {
                    bDisableZigBeePrivateProfile = false;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145DisableZigBeePrivateProfile, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145DisableZigBeePrivateProfile, null, out objValue);
                    bDisableZigBeePrivateProfile = (bool)objValue;
                }
#else
                if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5_1) < 0))
                {
                   bDisableZigBeePrivateProfile = false;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145DisableZigBeePrivateProfile, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145DisableZigBeePrivateProfile, null, out objValue);
                    bDisableZigBeePrivateProfile = (bool)objValue;
                }
#endif

                return bDisableZigBeePrivateProfile;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee Radio is Enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/09 MMD 2.30.15        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public virtual bool? DisableZigbeeRadio
        {
            get
            {
                bool? bDisableZigbeeRadio = null;
                object objValue;

#if (!WindowsCE)
                if (m_GatewayTables != null && m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl145DisableZigBeeRadio, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MfgTbl145DisableZigBeeRadio, null, out objValue);
                    bDisableZigbeeRadio = (bool)objValue;
                }
                else if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5_1) < 0))
                {
                    bDisableZigbeeRadio = false;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145DisableZigBeeRadio, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145DisableZigBeeRadio, null, out objValue);
                    bDisableZigbeeRadio = (bool)objValue;
                }
#else
                if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5_1) < 0))
                {
                    bDisableZigbeeRadio = false;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145DisableZigBeeRadio, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145DisableZigBeeRadio, null, out objValue);
                    bDisableZigbeeRadio = (bool)objValue;
                }
#endif

                return bDisableZigbeeRadio;
            }
        }

        /// <summary>
        /// IO Configuration Data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/09 jrf 2.20.03    N/A    Created
        //
        public virtual KYZData IOData
        {
            get
            {
                Stream strmIOConfig = new MemoryStream();
                m_CenTables.BuildPSEMStream(2048, strmIOConfig, ConfigHeader.IOOffset, CENTRON_AMI_IOConfig.IO_CONFIG_TBL_SIZE);
                PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmIOConfig);
                CENTRON_AMI_IOConfig IOConfig = new CENTRON_AMI_IOConfig(EDLReader);

                return IOConfig.IOData;
            }
        }

        /// <summary>
        /// IO Configuration Enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/09 jrf 2.20.03    N/A    Created
        //
        public bool IOEnabled
        {
            get
            {
                bool bIOEnabled = false;

                if (0 != ConfigHeader.IOOffset)
                {
                    bIOEnabled = true;
                }

                return bIOEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 1 is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public virtual bool? IsSiteScanDiag1Enabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_1_ENABLE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_1_ENABLE, null, out objValue);
                    bIsEnabled = (bool)objValue;
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 2 is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public virtual bool? IsSiteScanDiag2Enabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_2_ENABLE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_2_ENABLE, null, out objValue);
                    bIsEnabled = (bool)objValue;
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 3 is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public virtual bool? IsSiteScanDiag3Enabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_3_ENABLE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_3_ENABLE, null, out objValue);
                    bIsEnabled = (bool)objValue;
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 4 is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public virtual bool? IsSiteScanDiag4Enabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_4_ENABLE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_4_ENABLE, null, out objValue);
                    bIsEnabled = (bool)objValue;
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets the site scan service sensing delay.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/25/16 jrf 4.70.28 230427 Created
        public virtual byte? SiteScanServiceSensingDelay
        {
            get
            {
                byte? Delay = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_SERVICE_SENSING_DELAY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_SERVICE_SENSING_DELAY, null, out objValue);
                    Delay = (byte)objValue;
                }

                return Delay;
            }
        }

        /// <summary>
        /// Gets whether or not the Load Side Voltage Delay is enabled or not.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  10/07/13 DLG 3.00.14 WR327168  Created.
        //  
        public virtual bool? IsLoadSideVoltageDelayEnabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212LoadVoltageDetectionDelay, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212LoadVoltageDetectionDelay, null, out objValue);

                    if ((ushort)objValue == 65535 || objValue == null)
                    {
                        bIsEnabled = false;
                    }
                    else
                    {
                        bIsEnabled = true;
                    }
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 1 will scroll 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public virtual bool? ScrollSiteScanDiag1
        {
            get
            {
                bool? bWillScroll = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SCROLL_POLARITY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_POLARITY, null, out objValue);
                    bWillScroll = (bool)objValue;
                }

                return bWillScroll;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 2 will scroll 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public virtual bool? ScrollSiteScanDiag2
        {
            get
            {
                bool? bWillScroll = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SCROLL_PHASE_VOLTAGE_DEVIATION, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_PHASE_VOLTAGE_DEVIATION, null, out objValue);
                    bWillScroll = (bool)objValue;
                }

                return bWillScroll;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 3 will scroll 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public virtual bool? ScrollSiteScanDiag3
        {
            get
            {
                bool? bWillScroll = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SCROLL_INACTIVE_PHASE_CURRENT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_INACTIVE_PHASE_CURRENT, null, out objValue);
                    bWillScroll = (bool)objValue;
                }

                return bWillScroll;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 4 will scroll 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public virtual bool? ScrollSiteScanDiag4
        {
            get
            {
                bool? bWillScroll = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SCROLL_PHASE_ANGLE_DEVIATION, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_PHASE_ANGLE_DEVIATION, null, out objValue);
                    bWillScroll = (bool)objValue;
                }

                return bWillScroll;
            }
        }

        /// <summary>
        /// Gets the LED Normal mode quantity
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public virtual string LEDNormalModeQuantity
        {
            get
            {
                string strQuantity = "";
                object objValue;
                LEDQuantity Quantity;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_NORMAL, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_NORMAL, null, out objValue);
                    Quantity = new LEDQuantity((uint)objValue);
                    strQuantity = Quantity.Description;
                }

                return strQuantity;
            }
        }

        /// <summary>
        /// Gets the LED Test mode quantity
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public virtual string LEDTestModeQuantity
        {
            get
            {
                string strQuantity = "";
                object objValue;
                LEDQuantity Quantity;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_TEST, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_TEST, null, out objValue);
                    Quantity = new LEDQuantity((uint)objValue);
                    strQuantity = Quantity.Description;
                }

                return strQuantity;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 1S meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/29/11 AF  2.50.43 171706 Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight1S
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_1S_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_1S_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 2S Class 200 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/29/11 AF  2.50.43 171706 Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight2SClass200
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_2S_C200_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_2S_C200_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 2S Class 320 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/29/11 AF  2.50.43 171706 Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight2SClass320
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_2S_C320_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_2S_C320_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 3S meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/29/11 AF  2.50.43 171706 Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight3S
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_3S_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_3S_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 3S meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/29/11 AF  2.50.43 171706 Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight4S
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_4S_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_4S_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 9S meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight9S
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_9S_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_9S_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 12S Class 200 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight12SClass200
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_12S_C200_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_12S_C200_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 12S Class 320 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight12SClass320
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_12S_C320_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_12S_C320_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 16S Class 200 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight16SClass200
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_16S_C200_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_16S_C200_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 16S Class 320 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight16SClass320
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_16S_C320_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_16S_C320_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 45S meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? LEDPulseWeight45S
        {
            get
            {
                float? fPulseWeight = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_45S_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_45S_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the form of the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public string MeterForm
        {
            get
            {
                string strForm = "Unknown";

                if (Table2091 != null)
                {
                    strForm = EnumDescriptionRetriever.RetrieveDescription(Table2091.MeterForm);
                }

                return strForm;
            }
        }

        /// <summary>
        /// Gets the service type of the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public string ServiceType
        {
            get
            {
                string strService = "Unknown";

                if (Table2091 != null && Table2091.ServiceType != ServiceTypes.AutoServiceSense)
                {
                    strService = EnumDescriptionRetriever.RetrieveDescription(Table2091.ServiceType);
                }

                return strService;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Volts for Phase A
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsVoltsPhaseA
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsVoltsPhaseA;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Volts for Phase B
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsVoltsPhaseB
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsVoltsPhaseB;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Volts for Phase C
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsVoltsPhaseC
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsVoltsPhaseC;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase A
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsAmpsPhaseA
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsAmpsPhaseA;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase B
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsAmpsPhaseB
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsAmpsPhaseB;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Current for Phase C
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsAmpsPhaseC
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsAmpsPhaseC;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the phase angle for Phase B volts
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? VoltsPhaseBAngle
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.VoltsPhaseBAngle;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the phase angle for Phase C volts
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? VoltsPhaseCAngle
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.VoltsPhaseCAngle;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the phase angle for Phase A current
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? AmpsPhaseAAngle
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.AmpsPhaseAAngle;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the phase angle for Phase B current
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? AmpsPhaseBAngle
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.AmpsPhaseBAngle;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the phase angle for Phase C current
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? AmpsPhaseCAngle
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.AmpsPhaseCAngle;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Ins kilo W
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsKW
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsW / 1000.0f;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Ins kilo Var
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsKVar
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsVar / 1000.0f;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Ins kilo VA
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsKVA
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsVA / 1000.0f;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the Ins PF
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public float? InsPF
        {
            get
            {
                float? value = null;

                if (Table2091 != null)
                {
                    value = Table2091.InsPF;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the SiteScan Diagnostic 1 count
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public byte SSDiag1Count
        {
            get
            {
                byte Value = 0;

                if (Table2091 != null)
                {
                    Value = Table2091.Diag1Count;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the SiteScan Diagnostic 2 count
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public byte SSDiag2Count
        {
            get
            {
                byte Value = 0;

                if (Table2091 != null)
                {
                    Value = Table2091.Diag2Count;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the SiteScan Diagnostic 3 count
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public byte SSDiag3Count
        {
            get
            {
                byte Value = 0;

                if (Table2091 != null)
                {
                    Value = Table2091.Diag3Count;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the SiteScan Diagnostic 4 count
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public byte SSDiag4Count
        {
            get
            {
                byte Value = 0;

                if (Table2091 != null)
                {
                    Value = Table2091.Diag4Count;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diagnostic 1 is active.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public bool IsSSDiag1Active
        {
            get
            {
                bool Value = false;

                if (Table2091 != null)
                {
                    Value = Table2091.IsDiag1Active;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diagnostic 2 is active.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public bool IsSSDiag2Active
        {
            get
            {
                bool Value = false;

                if (Table2091 != null)
                {
                    Value = Table2091.IsDiag2Active;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diagnostic 3 is active.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public bool IsSSDiag3Active
        {
            get
            {
                bool Value = false;

                if (Table2091 != null)
                {
                    Value = Table2091.IsDiag3Active;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diagnostic 4 is active.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public bool IsSSDiag4Active
        {
            get
            {
                bool Value = false;

                if (Table2091 != null)
                {
                    Value = Table2091.IsDiag4Active;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the SiteScan Toolbox object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public Toolbox ToolboxData
        {
            get
            {
                Toolbox NewToolbox = new Toolbox();

                if (Table2091 != null)
                {
                    NewToolbox.m_dInsKVA = InsKVA.Value;
                    NewToolbox.m_dInsKVAArith = InsKVA.Value;
                    NewToolbox.m_dInsKVAVect = InsKVA.Value;
                    NewToolbox.m_dInsKVar = InsKVar.Value;
                    NewToolbox.m_dInsKW = InsKW.Value;
                    NewToolbox.m_dInsPF = InsPF.Value;

                    NewToolbox.m_fVoltsA = InsVoltsPhaseA.Value;
                    NewToolbox.m_fVoltsB = InsVoltsPhaseB.Value;
                    NewToolbox.m_fVoltsC = InsVoltsPhaseC.Value;

                    NewToolbox.m_fCurrentA = InsAmpsPhaseA.Value;
                    NewToolbox.m_fCurrentB = InsAmpsPhaseB.Value;
                    NewToolbox.m_fCurrentC = InsAmpsPhaseC.Value;

                    NewToolbox.m_fVAngleA = 0.0f;
                    NewToolbox.m_fVAngleB = VoltsPhaseBAngle.Value;
                    NewToolbox.m_fVAngleC = VoltsPhaseCAngle.Value;

                    NewToolbox.m_fIAngleA = AmpsPhaseAAngle.Value;
                    NewToolbox.m_fIAngleB = AmpsPhaseBAngle.Value;
                    NewToolbox.m_fIAngleC = AmpsPhaseCAngle.Value;
                }

                return NewToolbox;
            }
        }

        /// <summary>
        /// Gets whether or not Signed Authorization is required.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/09 RCG 2.30.16        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //  12/05/10 SCW 9.70.14        Promoted the property to be a virtual property

        public virtual bool? RequireSignedAuthorization
        {
            get
            {
                bool? bRequired = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145RequireSignedAuthorization, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145RequireSignedAuthorization, null, out objValue);
                    bRequired = (bool)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl145RequireSignedAuthorization, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MfgTbl145RequireSignedAuthorization, null, out objValue);
                    bRequired = (bool)objValue;
                }
#endif

                return bRequired;
            }
        }

        /// <summary>
        /// Gets the HAN Security Profile
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/09 RCG 2.30.16        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public virtual string HANSecurityProfile
        {
            get
            {
                string strValue = null;
                object objValue = null;

                byte bySecurityMode = 0;
                byte byDeviceAuthMode = 0;
                byte byCBKEMode = 0;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58SecurityMode, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58DeviceAuthMode, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58CbkeMode, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58SecurityMode, null, out objValue);
                    bySecurityMode = (byte)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58DeviceAuthMode, null, out objValue);
                    byDeviceAuthMode = (byte)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58CbkeMode, null, out objValue);
                    byCBKEMode = (byte)objValue;

                    strValue = CHANMfgTable2106.GetHANSecurityProfile(bySecurityMode, byDeviceAuthMode, byCBKEMode);
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl58SecurityMode, null)
                    && m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl58DeviceAuthMode, null)
                    && m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl58CbkeMode, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MfgTbl58SecurityMode, null, out objValue);
                    bySecurityMode = (byte)objValue;

                    m_GatewayTables.GetValue(GatewayTblEnum.MfgTbl58DeviceAuthMode, null, out objValue);
                    byDeviceAuthMode = (byte)objValue;

                    m_GatewayTables.GetValue(GatewayTblEnum.MfgTbl58CbkeMode, null, out objValue);
                    byCBKEMode = (byte)objValue;

                    strValue = CHANMfgTable2106.GetHANSecurityProfile(bySecurityMode, byDeviceAuthMode, byCBKEMode);
                }
#endif

                return strValue;
            }
        }

        /// <summary>
        /// Gets the Inter PAN Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/06/09 RCG 2.30.16 144719 Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public virtual string InterPANMode
        {
            get
            {
                string strValue = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58InterPanMode, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58InterPanMode, null, out objValue);
                    strValue = CHANMfgTable2106.GetInterPANMode((byte)objValue);
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl58InterPanMode, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MfgTbl58InterPanMode, null, out objValue);
                    strValue = CHANMfgTable2106.GetInterPANMode((byte)objValue);
                }
#endif

                return strValue;
            }
        }

        /// <summary>
        /// Gets the ZigBee output power level.  Value should be between -30 and 3.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 jrf 2.52.00 177455 Created
        //
        public virtual sbyte? ZigBeePowerLevel
        {
            get
            {
                sbyte? sbyPowerLevel = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58PowerLevel, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58PowerLevel, null, out objValue);
                    sbyPowerLevel = (sbyte)objValue;
                }

                return sbyPowerLevel;

            }
        }

        /// <summary>
        /// Gets whether or not Fatal Recovery Mode is configured to be enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/23/10 RCG 2.40.28        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //  03/02/11 RCG 2.50.05 168580 Adding check for Use Default

        public virtual bool? IsFatalRecoveryModeConfigured
        {
            get
            {
                bool bUseDefault = false;
                bool? bConfigured = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212FatalErrorRecoveryUseMeterDefaults, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212FatalErrorRecoveryEnabled, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212FatalErrorRecoveryUseMeterDefaults, null, out objValue);
                    bUseDefault = (bool)objValue;

                    // Make sure the default value is false before reading the value
                    if (bUseDefault == false)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl212FatalErrorRecoveryEnabled, null, out objValue);
                        bConfigured = (bool)objValue;
                    }
                    else
                    {
                        // Use the default which is disabled
                        bConfigured = false;
                    }
                }

                return bConfigured;
            }
        }

        /// <summary>
        /// Gets whether or not Asset Synch is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/23/10 RCG 2.40.28        Created
        //  11/05/10 jrf 2.45.10        Added set.
        //  03/02/11 RCG 2.50.05 168580 Adding check for Use Default

        public virtual bool? IsAssetSynchEnabled
        {
            get
            {
                bool bUseDefault = false;
                bool? bEnabled = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212AssetSynchronizationUseMeterDefaults, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212AssetSynchronizationEnabled, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212AssetSynchronizationUseMeterDefaults, null, out objValue);
                    bUseDefault = (bool)objValue;

                    // Make sure we are not using the default value
                    if (bUseDefault == false)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl212AssetSynchronizationEnabled, null, out objValue);
                        bEnabled = (bool)objValue;
                    }
                    else
                    {
                        // Use the default value which is disabled.
                        bEnabled = false;
                    }
                }

                return bEnabled;
            }
            set
            {
                object objValue = value;

                m_CenTables.SetValue(CentronTblEnum.MfgTbl212AssetSynchronizationEnabled, null, objValue);
            }
        }

        /// <summary>
        /// Gets whether or not the meter is connected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03        Created

        public virtual bool? IsConnected
        {
            get
            {
                bool? bEnabled = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL92_IS_CONNECTED_FLAG, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL92_IS_CONNECTED_FLAG, null, out objValue);
                    bEnabled = (bool)objValue;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is armed for connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03        Created

        public virtual bool? IsMeterArmed
        {
            get
            {
                bool? bArmed = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL92_IS_METER_ARMED, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL92_IS_METER_ARMED, null, out objValue);
                    bArmed = (bool)objValue;
                }

                return bArmed;
            }
        }

        /// <summary>
        /// Gets whether or not load voltage is currently present
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03        Created

        public virtual bool? IsLoadVoltagePresent
        {
            get
            {
                bool? bPresent = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL92_LOAD_VOLTAGE_PRESENT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL92_LOAD_VOLTAGE_PRESENT, null, out objValue);
                    bPresent = (bool)objValue;
                }

                return bPresent;
            }
        }

        /// <summary>
        /// Gets whether or not the last connect or disconnect switch attempt failed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03        Created

        public virtual bool? DidLastDisconnectAttemptFail
        {
            get
            {
                bool? bLastAttemptFailed = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL92_LAST_ATTEMPT_FAIL_FLAG, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL92_LAST_ATTEMPT_FAIL_FLAG, null, out objValue);
                    bLastAttemptFailed = (bool)objValue;
                }

                return bLastAttemptFailed;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains History entries. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        //  07/30/10 AF  2.42.09        Added support for the M2 Gateway meter
        //  07/03/13 AF  2.80.45 TR7640 Added support for ICS comm module events
        //
        public virtual bool ContainsHistoryEntries
        {
            get
            {
                bool bContainsHistoryEntries = false;
                ushort usNbrValidEntries = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null))
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null, out objValue);
                    usNbrValidEntries = (ushort)objValue;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL476_NBR_VALID_ENTRIES, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL476_NBR_VALID_ENTRIES, null, out objValue);
                    usNbrValidEntries = (ushort)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null))
                {
                    m_GatewayTables.GetValue(StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null, out objValue);
                    usNbrValidEntries = (ushort)objValue;
                }
#endif

                if (usNbrValidEntries > 0)
                {
                    bContainsHistoryEntries = true;
                }

                return bContainsHistoryEntries;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains History entries. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 MSC 2.80.15 TREQ7640 Created (clone of History Entries for now)
        //
        public virtual bool ContainsCellHistoryEntries
        {
            get
            {
                bool bContainsHistoryEntries = false;
                ushort usNbrValidEntries = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null))
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null, out objValue);
                    usNbrValidEntries = (ushort)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null))
                {
                    m_GatewayTables.GetValue(StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null, out objValue);
                    usNbrValidEntries = (ushort)objValue;
                }
#endif

                if (usNbrValidEntries > 0)
                {
                    bContainsHistoryEntries = true;
                }

                return bContainsHistoryEntries;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Event entries. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/27/08 jrf 1.50           Created
        //  07/30/10 AF  2.42.09        Added support for the M2 Gateway meter
        //
        public virtual bool ContainsEventEntries
        {
            get
            {
                bool bContainsEventEntries = false;
                ushort usNbrValidEntries = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL76_NBR_VALID_ENTRIES, null))
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL76_NBR_VALID_ENTRIES, null, out objValue);
                    usNbrValidEntries = (ushort)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL76_NBR_VALID_ENTRIES, null))
                {
                    m_GatewayTables.GetValue(StdTableEnum.STDTBL76_NBR_VALID_ENTRIES, null, out objValue);
                    usNbrValidEntries = (ushort)objValue;
                }
#endif

                if (usNbrValidEntries > 0)
                {
                    bContainsEventEntries = true;
                }

                return bContainsEventEntries;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Network Statistics data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        //  06/18/07 RCG 1.50.38        Adding check for 122
        //  07/30/10 AF  2.42.09        Added support for the M2 Gateway meter
        //
        public virtual bool ContainsNetworkStatistics
        {
            get
            {
                bool bContainsNetworkStats = false;
                ushort usNbrEntries = 0;
                object objValue = null;

                // Table 121 is now always included so we need to check table 122 as well.
                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL121_NBR_STATISTICS, null) &&
                    m_CenTables.IsAllCached(122))
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL121_NBR_STATISTICS, null, out objValue);
                    usNbrEntries = (ushort)objValue;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL121_NBR_STATISTICS, null) &&
                    m_GatewayTables.IsAllCached(122))
                {
                    m_GatewayTables.GetValue(StdTableEnum.STDTBL121_NBR_STATISTICS, null, out objValue);
                    usNbrEntries = (ushort)objValue;
                }
#endif

                if (usNbrEntries > 0)
                {
                    bContainsNetworkStats = true;
                }

                return bContainsNetworkStats;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Network Statistics data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/08 KRC 1.50.26        Created
        //  06/03/08 KRC 1.50.31        Check for actual log table, not config table.
        //  08/02/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual bool ContainsCommLogs
        {
            get
            {
                bool bResult = false;

                //Check to make sure that one of the log tables is in the file (if not then the other will not be)
                if (m_CenTables.IsTableKnown(2162) && m_CenTables.IsAllCached(2162))
                {
                    bResult = true;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsTableKnown(2162) && m_GatewayTables.IsAllCached(2162))
                {
                    bResult = true;
                }
#endif

                return bResult;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains the HAN Event Logs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 RCG 2.50.02        Created

        public virtual bool ContainsHANEventLogs
        {
            get
            {
                bool bContainsLogs = false;

                // All of these tables need to be present in order to be able to say the file contains
                // these values.
                if (Table2239 != null && Table2240 != null && Table2241 != null
                    && Table2242 != null && Table2243 != null)
                {
                    bContainsLogs = true;
                }

                return bContainsLogs;
            }
        }

        /// <summary>Returns the full path to the EDL file</summary>
        // 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/07 mcm 8.10.04		   Created
        // 11/17/10 jrf 2.45.13        Added set.
        //
        public string FileName
        {
            get { return m_strEDLFile; }//get
            set { m_strEDLFile = value; }//set

        } // FileName

        /// <summary>
        /// Gets Actual Meter Value for Device Class
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#          Description
        // -------- --- ------- --------------  ---------------------------------------
        // 07/04/08 KRC	1.51.02	itron00117146	Adding Device Class to Program View
        // 08/02/10 AF  2.42.11                 Added support for the M2 Gateway
        //
        public virtual string DeviceClass
        {
            get
            {
                if (!m_strDeviceClass.Cached)
                {
                    object Value;
                    m_strDeviceClass.Value = "";
                    if (m_CenTables.IsCached((long)StdTableEnum.STDTBL0_DEVICE_CLASS, null))
                    {
                        m_CenTables.GetValue(StdTableEnum.STDTBL0_DEVICE_CLASS, null, out Value);
                        ASCIIEncoding AE = new ASCIIEncoding();
                        m_strDeviceClass.Value = AE.GetString((byte[])Value, 0, ((byte[])Value).Length);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL0_DEVICE_CLASS, null))
                    {
                        m_GatewayTables.GetValue(StdTableEnum.STDTBL0_DEVICE_CLASS, null, out Value);
                        ASCIIEncoding AE = new ASCIIEncoding();
                        m_strDeviceClass.Value = AE.GetString((byte[])Value, 0, ((byte[])Value).Length);
                    }
#endif
                }

                return m_strDeviceClass.Value;
            }
        }

        /// <summary>
        /// Gets the device type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created
        //  06/03/10 AF  2.41.06        Added M2 Gateway device
        //  07/06/10 AF  2.42.02        Added support for "lis1" or "LIS1"
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        //  07/03/13 AF  2.80.45 TR7640 Added support for ITRH device class (I-210/kV2c)
        //  12/03/13 DLG 3.50.09 TR9530 Added support for ITRU and ITRV device classes.
        //  05/19/15 AF  4.20.08 585887 Added support for ITRK device class
        //
        public EDLDeviceTypes DeviceType
        {
            get
            {
                EDLDeviceTypes DeviceType = EDLDeviceTypes.OpenWayCentron;

                switch (DeviceClass)
                {
                    case CENTRON_AMI.ITRN_DEVICE_CLASS:
                    case CENTRON_AMI.ITR1_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentron;
                        break;
                    }
                    case CENTRON_AMI.ITR3_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentronBasicPoly;
                        break;
                    }
                    case CENTRON_AMI.ITR4_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentronAdvPoly;
                        break;
                    }
                    case CENTRON_AMI.ITRT_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.TransparentDevice;
                        break;
                    }
                    case LIS1_DEVICE_CLASS:
                    case lis1_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.M2GatewayDevice;
                        break;
                    }
                    case CENTRON_AMI.ITRD_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentronITRD;
                        break;
                    }
                    case CENTRON_AMI.ITRE_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentronBasicPolyITRE;
                        break;
                    }
                    case CENTRON_AMI.ITRF_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentronAdvPolyITRF;
                        break;
                    }
                    case CENTRON_AMI.ITRH_DEVICE_CLASS:
                    case CENTRON_AMI.ITRU_DEVICE_CLASS:
                    case CENTRON_AMI.ITRV_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.ICSGatewayDevice;
                        break;
                    }
                    case CENTRON_AMI.ITRK_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentronPolyITRK;
                        break;
                    }
                }

                return DeviceType;
            }
        }

        /// <summary>
        /// Gets the Human Readable Meter Value for Device Class
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#          Description
        // -------- --- ------- --------------  ---------------------------------------
        // 07/04/08 KRC	1.51.02	itron00117146	Adding Device Class to Program View
        // 05/19/09 AF  2.20.05                 Added ITRT device class
        // 06/03/10 AF  2.41.06                 Added M2 Gateway device and replaced hard coded strings
        //  07/06/10 AF  2.42.02                Added support for "lis1" or "LIS1"
        // 03/07/11 RCG 2.50.07                 Adding support for ITRD, ITRE, and ITRF device classes
        // 06/25/13 mah 2.80.42                 Adding support for ITRJ
        // 09/16/13 mah 2.85.44 423280          Changed the device class name from 'Direct Connect' to 'ICM'
        // 10/31/13 AF  3.00.25 WR326803        Changed the device class name to the actual device class
        // 03/23/15 AF  4.10.09 WR572986        Added support for ITRK
        //
        public string DeviceClassHumanReadable
        {
            get
            {
                string strDeviceClassHR = "";

                switch (DeviceClass)
                {
                    case CENTRON_AMI.ITRN_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRN";
                        break;
                    }
                    case CENTRON_AMI.ITR1_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITR1";
                        break;
                    }
                    case CENTRON_AMI.ITR3_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITR3";
                        break;
                    }
                    case CENTRON_AMI.ITR4_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITR4";
                        break;
                    }
                    case CENTRON_AMI.ITRT_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRT";
                        break;
                    }
                    case ITRA_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRA";
                        break;
                    }
                    case ITRB_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRB";
                        break;
                    }
                    case ITRC_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRC";
                        break;
                    }
                    case LIS1_DEVICE_CLASS:
                    case lis1_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "LIS1";
                        break;
                    }
                    case CENTRON_AMI.ITRD_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRD";
                        break;
                    }
                    case CENTRON_AMI.ITRE_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRE";
                        break;
                    }
                    case CENTRON_AMI.ITRF_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRF";
                        break;
                    }
                    case CENTRON_AMI.ITRJ_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRJ";
                        break;
                    }
                    case CENTRON_AMI.ITRK_DEVICE_CLASS:
                    {
                        strDeviceClassHR = "ITRK";
                        break;
                    }
                    default:
                    {
                        strDeviceClassHR = "Unknown";
                        break;
                    }
                }

                return strDeviceClassHR;
            }
        }

        /// <summary>
        /// Returns the array of history entries read from table 74
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/07 mcm 8.10.04		   Created
        // 03/31/10 RCG 2.40.31 151959 Changed to use Table objects

        public List<HistoryEntry> HistoryEntries
        {
            get
            {
                List<HistoryEntry> Entries = new List<HistoryEntry>();

                if (Table74 != null)
                {
                    Entries = Table74.HistoryLogEntries;
                }

                return Entries;

            } // get
        } // HistoryEntries

        /// <summary>
        /// Property that will return a List of HANEntries
        /// </summary>
        public List<HANEntry> HANEntries
        {
            get
            {
                if (null == m_HANEntries)
                {
                    GetHANEntries();
                }

                return m_HANEntries;
            }
        }

        /// <summary>
        /// Returns the array of the Comm Module Event entries read from table 2524
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/13/13 MSC 2.80.27	TQ7640 Created
        // 06/21/13 AF  2.80.40 TR7640 Using HistoryEntry class instead of CommModuleHistoryEntry
        //
        public List<HistoryEntry> CommModuleEventEntries
        {
            get
            {
                List<HistoryEntry> Entries = new List<HistoryEntry>();

                if (Table2524 != null)
                {
                    Entries = Table2524.CommModuleHistoryEventEntries;
                }

                return Entries;

            } // get
        } // HistoryEntries

        /// <summary>
        /// Returns the supported ICS comm module events.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/30/13 jrf 2.80.36	TQ8280 Created
        //
        public List<MFG2048EventItem> SupportedICSEventEntries
        {
            get
            {
                List<MFG2048EventItem> Entries = new List<MFG2048EventItem>();

                if (Table2523 != null)
                {
                    Entries = Table2523.ICSHistoryLogEventList;
                }

                return Entries;

            } // get
        }

        /// <summary>
        /// Returns the supported ICS comm module user events.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/10/13 jrf 2.80.37	TQ8280 Created
        //
        public IEnumerable<MFG2048EventItem> SupportedICSUserEventEntries
        {
            get
            {
                IEnumerable<MFG2048EventItem> Entries = null;

                if (Table2523 != null)
                {
                    Entries = Table2523.ICSHistoryLogSupportedUserEventList;
                }

                return Entries;

            } // get
        }

        /// <summary>
        /// Returns the monitored ICS comm module events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  07/22/16 AF  4.60.02  WR 623194  Created
        //
        public IEnumerable<MFG2048EventItem> MonitoredICSEvents
        {
            get
            {
                IEnumerable<MFG2048EventItem> Entries = null;

                if (Table2523 != null)
                {
                    Entries = Table2523.ICSHistoryLogMonitoredEventList;
                }

                return Entries;
            }
        }

        /// <summary>
        /// Returns the supported ICS comm module non-user events.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/10/13 jrf 2.80.37	TQ8280 Created
        //
        public IEnumerable<MFG2048EventItem> SupportedICSNonUserEventEntries
        {
            get
            {
                IEnumerable<MFG2048EventItem> Entries = null;

                if (Table2523 != null)
                {
                    Entries = Table2523.ICSHistoryLogSupportedNonUserEventList;
                }

                return Entries;

            } // get
        }

        /// <summary>
        /// Property that provides a list of the Supported Std Events
        /// </summary>
        public List<CommLogEvent> SupportedStdCommEvents
        {
            get
            {
                if (m_lstSupportedStdEvents == null)
                {
                    GetSupportedCommunicationsEvents();
                }

                return m_lstSupportedStdEvents;
            }
        }

        /// <summary>
        /// Property that provides a List of the Supported MFG Events
        /// </summary>
        public List<CommLogEvent> SupportedMfgCommEvents
        {
            get
            {
                if (m_lstSupportedMFGEvents == null)
                {
                    GetSupportedCommunicationsEvents();
                }

                return m_lstSupportedMFGEvents;
            }
        }

        /// <summary>
        /// Property to return the LAN Configuration Table which will allow us to check if Events are supported.
        /// </summary>
        public MFGTable2161 LANConfiguration
        {
            get
            {
                if (null == m_Table2161)
                {
                    GetLANConfig();
                }

                return m_Table2161;
            }
        }

        /// <summary>
        /// Property to return the HAN Configuration Table which will allow us to check if Events are supported
        /// </summary>
        public MFGTable2163 HANConfiguration
        {
            get
            {
                if (null == m_Table2163)
                {
                    GetHANConfig();
                }

                return m_Table2163;
            }
        }

        /// <summary>
        /// Returns the events configured in the HAN 2 log table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/04/11 AF  2.50.43 171706 Created
        //  03/26/12 AF  2.53.52 191613 Added M2 Gateway support
        //
        public List<MFG2260HANEventItem> HAN2Configuration
        {
            get
            {
                List<MFG2260HANEventItem> HAN2ConfigItems = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212Han2LoggerControl, new int[] { 0 }))
                {
                    Stream strmHAN2Config = new MemoryStream();
                    m_CenTables.BuildPSEMStream(2260, strmHAN2Config, TABLE2260_HAN_EVENTS_OFFSET, TABLE2260_HAN_EVENTS_SIZE);

                    OpenWayMFGTable2260HANEvents HAN2Events = new OpenWayMFGTable2260HANEvents(new PSEMBinaryReader(strmHAN2Config));

                    HAN2ConfigItems = HAN2Events.HAN2EventConfiguration;

                    strmHAN2Config.Dispose();
                }
                else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MfgTbl212Han2LoggerControl, new int[] { 0 }))
                {
                    Stream strmHAN2Config = new MemoryStream();
                    m_GatewayTables.BuildPSEMStream(2260, strmHAN2Config, TABLE2260_HAN_EVENTS_OFFSET, TABLE2260_HAN_EVENTS_SIZE);

                    OpenWayMFGTable2260HANEvents HAN2Events = new OpenWayMFGTable2260HANEvents(new PSEMBinaryReader(strmHAN2Config));

                    HAN2ConfigItems = HAN2Events.HAN2EventConfiguration;

                    strmHAN2Config.Dispose();
                }

                return HAN2ConfigItems;
            }
        }

        /// <summary>
        /// Property that will return a List of LANEntries
        /// </summary>
        public List<LANEntry> LANEntries
        {
            get
            {
                if (null == m_LANEntries)
                {
                    GetLANEntries();
                }

                return m_LANEntries;
            }
        }

        /// <summary>Returns the array of event entries read from table 76.</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/21/08 jrf 1.50  		    Created
        /// </remarks>
        public HistoryEntry[] EventEntries
        {
            get
            {
                if (null == m_EventEntries)
                {
                    GetEventEntries();
                }

                return m_EventEntries;

            }
        }

        /// <summary>
        /// Gets the Statistics data
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/11/08 KRC                Adding Networks Statistics to EDL translation
        //  06/06/08 jrf 1.50.32 114519 Added check to make sure EDL had network statistics 
        //                              before trying to read them.
        //
        public List<CStatistic> NetworkStats
        {
            get
            {
                if (null == m_lstNetworkStatistic && ContainsNetworkStatistics)
                {
                    BuildNetworkStatistics();
                }

                return m_lstNetworkStatistic;
            }
        }

        /// <summary>
        /// Native address used to access the C12.22 Relay on this
        /// route for the C12.22 Node's local C12.22 Network Segment
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/25/08 jrf                Added Registration Status to EDL translation.
        //
        public String RelayNativeAddress
        {
            get
            {
                if (!m_strRelayNativeAddress.Cached)
                {
                    GetRegistrationStatus();
                }

                return m_strRelayNativeAddress.Value;
            }
        }

        /// <summary>
        /// Relative or absolute object identifier assigned to the C12.22 Master
        /// Relay responsible for this C12.22 node.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/25/08 jrf                Added Registration Status to EDL translation.
        //
        public String MasterRelayAptitle
        {
            get
            {
                if (!m_strMasterRelayAptitle.Cached)
                {
                    GetRegistrationStatus();
                }

                return m_strMasterRelayAptitle.Value;
            }
        }

        /// <summary>
        /// Relative or absolute object identifier assigned to this C12.22 node.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/25/08 jrf                Added Registration Status to EDL translation.
        //
        public String NodeAptitle
        {
            get
            {
                if (!m_strNodeAptitle.Cached)
                {
                    GetRegistrationStatus();
                }

                return m_strNodeAptitle.Value;
            }
        }

        /// <summary>
        /// Maximum random delay, in seconds, between each power up
        /// and the automatic issuance of the first Registration Service
        /// request by the C12.22 node.  This function is disabled when
        /// this field is set to zero.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/25/08 jrf                Added Registration Status to EDL translation.
        //
        public UInt16 RegistrationDelay
        {
            get
            {
                if (!m_usRegistrationDelay.Cached)
                {
                    GetRegistrationStatus();
                }

                return m_usRegistrationDelay.Value;
            }
        }

        /// <summary>
        /// Maximum duration, in minutes, before the C12.22 Node's registration
        /// expires.  The C12.22 Node needs to reregister itself before this 
        /// period lapses in order to remain registered.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/25/08 jrf                Added Registration Status to EDL translation.
        //  06/04/08 KRC 2.50.31        Change to TimeSpan
        //
        public TimeSpan RegistrationPeriod
        {
            get
            {
                if (!m_tsRegistrationPeriod.Cached)
                {
                    GetRegistrationStatus();
                }

                return m_tsRegistrationPeriod.Value;
            }
        }

        /// <summary>
        /// The amount of time (TimeSpan) left before the registration period.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/25/08 jrf                Added Registration Status to EDL translation.
        //  06/04/08 KRC 2.50.31        Change to TimeSpan
        //
        public TimeSpan RegistrationCountDown
        {
            get
            {
                if (!m_tsRegistrationCountDown.Cached)
                {
                    GetRegistrationStatus();
                }

                return m_tsRegistrationCountDown.Value;
            }
        }

        /// <summary> 
        /// Gets the Time Zone applied Flag. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        public virtual bool TimeZoneEnabled
        {
            get
            {
                if (!m_blnTimeZoneEnabled.Cached)
                {
                    m_blnTimeZoneEnabled.Value = GetSTDEDLBool(StdTableEnum.STDTBL52_TM_ZN_APPLIED_FLAG);
                }

                return m_blnTimeZoneEnabled.Value;
            }
        }

        /// <summary> 
        /// Gets the GMT Flag. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        public virtual bool DeviceInGMT
        {
            get
            {
                if (!m_blnDeviceInGMT.Cached)
                {
                    m_blnDeviceInGMT.Value = GetSTDEDLBool(StdTableEnum.STDTBL52_GMT_FLAG);
                }

                return m_blnDeviceInGMT.Value;
            }
        }

        /// <summary> 
        /// Gets the current day of the week corresponding to the current 
        /// date of the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        public virtual DaysOfWeek DayOfWeek
        {
            get
            {
                if (DaysOfWeek.UNREAD == m_eDayOfWeek)
                {
                    m_eDayOfWeek = (DaysOfWeek)GetSTDEDLInt(StdTableEnum.STDTBL52_DAY_OF_WEEK);
                }

                return m_eDayOfWeek;
            }
        }

        /// <summary>
        /// Indicates whether the file contains Clock data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/08 jrf				   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        public virtual bool ContainsClockData
        {
            get
            {
                bool bContainsClockData = false;

                if (m_CenTables.IsAllCached(52))
                {
                    bContainsClockData = true;
                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsAllCached(52))
                {
                    bContainsClockData = true;
                }
#endif

                return bContainsClockData;
            }
        }

        /// <summary>
        /// Indicates whether the file contains Clock data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/08 jrf				   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        public virtual bool ContainsRegistrationStatus
        {
            get
            {
                bool bContainsRegistrationStatus = false;
                ushort usNbrEntries = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL121_NBR_REGISTRATIONS, null))
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL121_NBR_REGISTRATIONS, null, out objValue);
                    usNbrEntries = (byte)objValue;

                }
#if (!WindowsCE)
                else if (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL121_NBR_REGISTRATIONS, null))
                {
                    m_GatewayTables.GetValue(StdTableEnum.STDTBL121_NBR_REGISTRATIONS, null, out objValue);
                    usNbrEntries = (byte)objValue;
                }
#endif

                if (usNbrEntries > 0)
                {
                    bContainsRegistrationStatus = true;
                }

                return bContainsRegistrationStatus;
            }
        }

        /// <summary>
        /// Gets the list of supported Upstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 RCG 2.50.01        Created

        public ReadOnlyCollection<UpstreamHANLogEvent> SupportedUpstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<UpstreamHANLogEvent> Events = null;

                if (Table2240 != null)
                {
                    Events = Table2240.SupportedUpstreamEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of supported Downstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 RCG 2.50.01        Created

        public ReadOnlyCollection<DownstreamHANLogEvent> SupportedDownstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<DownstreamHANLogEvent> Events = null;

                if (Table2240 != null)
                {
                    Events = Table2240.SupportedDownstreamEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of Enabled Upstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 RCG 2.50.01        Created

        public ReadOnlyCollection<UpstreamHANLogEvent> EnabledUpstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<UpstreamHANLogEvent> Events = null;

                if (Table2241 != null)
                {
                    Events = Table2241.EnabledUpstreamEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of Enabled Downstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 RCG 2.50.01        Created

        public ReadOnlyCollection<DownstreamHANLogEvent> EnabledDownstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<DownstreamHANLogEvent> Events = null;

                if (Table2241 != null)
                {
                    Events = Table2241.EnabledDownstreamEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of Upstream HAN events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 RCG 2.50.01        Created

        public ReadOnlyCollection<UpstreamHANLogEvent> UpstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<UpstreamHANLogEvent> Events = null;

                if (Table2242 != null)
                {
                    Events = Table2242.Events;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of Downstream HAN Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 RCG 2.50.01        Created

        public ReadOnlyCollection<DownstreamHANLogEvent> DownstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<DownstreamHANLogEvent> Events = null;

                if (Table2243 != null)
                {
                    Events = Table2243.Events;
                }

                return Events;
            }
        }

        ///<summary>
        /// Gets the DST Dates from MFG Table 42. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/10/11 MMD                 Created

        public List<CDSTDatePair> CalendarConfigDSTDates
        {
            get
            {
                List<CDSTDatePair> DSTDates = new List<CDSTDatePair>();
                int[] ItemIndex = new int[2];
                for (int iIndex = 0; iIndex < 2; iIndex++)
                {

                    int StartDayOfMonth;
                    int StartMonth;
                    int EndDayOfMonth;
                    int EndMonth;
                    DateTime StartTime;
                    DateTime EndTime;
                    object objValue = null;

                    int[] YearIndex = new int[] { iIndex };
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL42_CALENDAR_YEAR, YearIndex, out objValue);
                    int Year = 2000 + (byte)objValue;

                    ItemIndex[0] = iIndex;
                    ItemIndex[1] = 0;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL42_CALENDAR_DAY_OF_MONTH, ItemIndex, out objValue);
                    StartDayOfMonth = (byte)objValue + 1;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL42_CALENDAR_MONTH, ItemIndex, out objValue);
                    StartMonth = (byte)objValue + 1;

                    ItemIndex[1] = 1;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL42_CALENDAR_DAY_OF_MONTH, ItemIndex, out objValue);
                    EndDayOfMonth = (byte)objValue + 1;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL42_CALENDAR_MONTH, ItemIndex, out objValue);
                    EndMonth = (byte)objValue + 1;

                    StartTime = new DateTime(Year, StartMonth, StartDayOfMonth);
                    EndTime = new DateTime(Year, EndMonth, EndDayOfMonth);

                    DSTDates.Add(new CDSTDatePair(StartTime, EndTime));
                }

                return DSTDates;
            }

        }

        /// <summary>
        /// Gets the DST Dates from MFG Table 212. This property should only be used for DST Reconfigure.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public List<CDSTDatePair> DST25YearDates
        {
            get
            {
                List<CDSTDatePair> DSTDates = new List<CDSTDatePair>();
                object objValue = null;

                for (int iIndex = 0; iIndex < 25; iIndex++)
                {
                    int[] ItemIndex = new int[] { iIndex };

                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212DstYear, ItemIndex))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl212DstYear, ItemIndex, out objValue);
                        int Year = 2000 + (byte)objValue;

                        // A year of 2255 (2000 + 0xFF) means the date is not configured
                        if (Year != 2255)
                        {
                            int StartDayOfMonth;
                            int StartMonth;
                            int EndDayOfMonth;
                            int EndMonth;
                            DateTime StartTime;
                            DateTime EndTime;

                            // Get the current year's Day and Month which are 0 based values so we need to add 1
                            m_CenTables.GetValue(CentronTblEnum.MfgTbl212DayStartDayOfMonth, ItemIndex, out objValue);
                            StartDayOfMonth = (byte)objValue + 1;

                            m_CenTables.GetValue(CentronTblEnum.MfgTbl212DayStartMonth, ItemIndex, out objValue);
                            StartMonth = (byte)objValue + 1;

                            m_CenTables.GetValue(CentronTblEnum.MfgTbl212DayEndDayOfMonth, ItemIndex, out objValue);
                            EndDayOfMonth = (byte)objValue + 1;

                            m_CenTables.GetValue(CentronTblEnum.MfgTbl212DayEndMonth, ItemIndex, out objValue);
                            EndMonth = (byte)objValue + 1;

                            StartTime = new DateTime(Year, StartMonth, StartDayOfMonth);
                            EndTime = new DateTime(Year, EndMonth, EndDayOfMonth);

                            DSTDates.Add(new CDSTDatePair(StartTime, EndTime));
                        }
                    }
                }

                return DSTDates;
            }
        }

        /// <summary>
        /// Gets the DST Hour from MFG Table 212. This property should only be used for DST Reconfigure.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public byte? DST25YearHour
        {
            get
            {
                byte? Hour = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212TimeConfigHour, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212TimeConfigHour, null, out objValue);
                    Hour = (byte)objValue;
                }

                return Hour;
            }
        }

        /// <summary>
        /// Gets the DST Minute from MFG Table 212. This property should only be used for DST Reconfigure.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public byte? DST25YearMinute
        {
            get
            {
                byte? Minute = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212TimeConfigMinute, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212TimeConfigMinute, null, out objValue);
                    Minute = (byte)objValue;
                }

                return Minute;
            }
        }

        /// <summary>
        /// Gets the DST Offset from MFG Table 212. This property should only be used for DST Reconfigure.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public byte? DST25YearOffset
        {
            get
            {
                byte? Offset = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212TimeConfigOffset, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212TimeConfigOffset, null, out objValue);
                    Offset = (byte)objValue;
                }

                return Offset;
            }
        }

        /// <summary>
        /// Provides access to a list of Self Read Collections
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/26/12 jrf 2.53.34 TRQ3438 Created 
        //
        public List<QuantityCollection> SelfReadRegisters
        {
            get
            {
                List<QuantityCollection> SelfReadQtys = new List<QuantityCollection>();
                Quantity Qty;
                uint uiNumSelfReads = Table26.NumberOfValidEntries;

                for (uint uiIndex = 0; uiIndex < uiNumSelfReads; uiIndex++)
                {
                    QuantityCollection SRQuantities = new QuantityCollection();
                    // Add Watts Del
                    Qty = SRWattsDelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Watts Rec
                    Qty = SRWattsReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Watts Net
                    Qty = SRWattsNet(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Watts Uni
                    Qty = SRWattsUni(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add VA Del
                    Qty = SRVADelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add VA Rec
                    Qty = SRVAReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add VA Lag
                    Qty = SRVALagging(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Del
                    Qty = SRVarDelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Rec
                    Qty = SRVarReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Net
                    Qty = SRVarNet(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Net Del
                    Qty = SRVarNetDelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Net Rec
                    Qty = SRVarNetReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Q1
                    Qty = SRVarQuadrant1(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Q2
                    Qty = SRVarQuadrant2(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Q3
                    Qty = SRVarQuadrant3(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Var Q4
                    Qty = SRVarQuadrant4(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add A (a)
                    Qty = SRAmpsPhaseA(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add A (b)
                    Qty = SRAmpsPhaseB(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add A (c)
                    Qty = SRAmpsPhaseC(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Neutral Amps
                    Qty = SRAmpsNeutral(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add A^2
                    Qty = SRAmpsSquared(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V (a)
                    Qty = SRVoltsPhaseA(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V (b)
                    Qty = SRVoltsPhaseB(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V (c)
                    Qty = SRVoltsPhaseC(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V Avg
                    Qty = SRVoltsAverage(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add V^2)
                    Qty = SRVoltsSquared(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add PF
                    Qty = SRPowerFactor(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Q d
                    Qty = SRQDelivered(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }
                    // Add Q r
                    Qty = SRQReceived(uiIndex);
                    if (null != Qty)
                    {
                        SRQuantities.Quantities.Add(Qty);
                    }

                    SRQuantities.Quantities.AddRange(SRCoincidentValues(uiIndex));

                    //Add the Time of the Self Read
                    SRQuantities.DateTimeOfReading = DateTimeOfSelfRead(uiIndex);

                    SelfReadQtys.Add(SRQuantities);
                }

                return SelfReadQtys;

            }
        }

        /// <summary>
        /// Provides access to a list of the twelve max demand records.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 04/09/12 jrf 2.53.54 196345  Created 
        //
        public AMIMDERCD[] TwelveMaxDemandRecords
        {
            get
            {
                AMIMDERCD[] aTwelveMaxDemandRcds = null;

                if (null != Table2175)
                {
                    aTwelveMaxDemandRcds = Table2175.AMIMDERCDs;
                }

                return aTwelveMaxDemandRcds;
            }
        }

        /// <summary>
        /// Gets the Bell Weather DataSet Configuration
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 11/03/15 PGH 4.50.212 577471  Created 
        //
        public DataSetConfigRcd[] BellWeatherDataSetConfiguration
        {
            get
            {
                DataSetConfigRcd[] aBellWeatherDataSetConfiguration = null;

                if (null != Table2265DataSetConfiguration)
                {
                    aBellWeatherDataSetConfiguration = Table2265DataSetConfiguration.DataSetConfiguration;
                }

                return aBellWeatherDataSetConfiguration;
            }
        }

        /// <summary>
        /// Gets the Bell Weather Configuration Record
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 11/03/15 PGH 4.50.212 577471  Created 
        //
        public PushConfigRcd[] BellWeatherConfigRcd
        {
            get
            {
                PushConfigRcd[] aBellWeatherConfigRcd = null;

                if (null != Table2185)
                {
                    aBellWeatherConfigRcd = Table2185.BellWeatherConfigRcd;
                }

                return aBellWeatherConfigRcd;
            }
        }

        /// <summary>
        /// Gets the Bell Weather Group Data Status Record
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 11/03/15 PGH 4.50.212 577471  Created 
        //
        public GroupDataStatusRcd BellWeatherGroupDataStatusRcd
        {
            get
            {
                GroupDataStatusRcd aBellWeatherGroupDataStatusRcd = null;

                if (null != Table2186)
                {
                    aBellWeatherGroupDataStatusRcd = Table2186.BellWeatherGroupDataStatusRcd;
                }

                return aBellWeatherGroupDataStatusRcd;
            }
        }

        /// <summary>
        /// Gets the Bell Weather Enable Record
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 11/03/15 PGH 4.50.212 577471  Created 
        //
        public BubbleUpEnableRcd BellWeatherEnableRcd
        {
            get
            {
                BubbleUpEnableRcd aBellWeatherEnableRcd = null;

                if (null != Table2187)
                {
                    aBellWeatherEnableRcd = Table2187.BellWeatherEnableRcd;
                }

                return aBellWeatherEnableRcd;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains 12 Max Demand data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 jrf 2.53.54 196345  Created 
        //
        public bool Contains12MaxDemandData
        {
            get
            {
                bool blnContainsData = false;

                if (null != Table2175)
                {
                    blnContainsData = (null != Table2175.AMIMDERCDs && Table2175.AMIMDERCDs.Length > 0);
                }

                return blnContainsData;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains a Temperature log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/16 PGH 4.50.225 RTT556309 Created
        //
        public bool ContainsTemperatureLog
        {
            get
            {
                bool blnContainsData = false;

                if (null != TemperatureLog)
                {
                    foreach (TemperatureLogEntry Entry in TemperatureLog)
                    {
                        if (Entry.CaptureDateTime.Year > 1970)
                        {
                            blnContainsData = true;
                            break;
                        }
                    }
                }

                return blnContainsData;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains the HAN Event Logs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 RCG 2.50.02        Created

        public virtual bool ContainsActiveBlockPriceSchedule
        {
            get
            {
                bool bActiveBlockPriceSchedule = false;

                // All of these tables need to be present in order to be able to say the file contains
                // these values.
                if (Table2439 != null && Table2440 != null)
                {
                    bActiveBlockPriceSchedule = true;
                }

                return bActiveBlockPriceSchedule;
            }
        }
        
        /// <summary>
        /// Device supports residential inclining block (RIB) pricing.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public bool SupportsRIB
        {
            get
            {
                bool bSupported = false;

                if (Table0.IsTableUsed(2438) && Table0.IsTableUsed(2439) && Table0.IsTableUsed(2440) && Table0.IsTableUsed(2441))
                {
                    bSupported = true;
                }

                return bSupported;
            }
        }

        /// <summary>
        /// Residential inclining block (RIB) pricing is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public bool RIBPricingEnabled
        {
            get
            {
                bool bEnabled = false;

                if (null != Table2440)
                {
                    bEnabled = Table2440.BlockPricingEnabled;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// The schedule ID for the active RIB schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public string ActiveRIBScheduleID
        {
            get
            {
                string strScheduleID = "";

                if (null != Table2440)
                {
                    strScheduleID = Table2440.ScheduleId;
                }

                return strScheduleID;
            }
        }

        /// <summary>
        /// The current HAN Time.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public DateTime CurrentHANDeviceTime
        {
            get
            {
                DateTime dtHANTime = DateTime.MinValue;

                if (null != Table2440)
                {
                    dtHANTime = Table2440.CurrentHANTime;
                }

                return dtHANTime;
            }
        }

        /// <summary>
        /// The most recent summed value of energy delivered and consumed in the premises during 
        /// the current block period.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public UInt64 CurrentRIBBlockPeriodConsumptionDelivered
        {
            get
            {
                UInt64 uiConsumption = 0;

                if (null != Table2440)
                {
                    uiConsumption = Table2440.CurrentBlockPeriodConsumptionDelivered;
                }

                return uiConsumption;
            }
        }

        /// <summary>
        /// The value of energy delivered and consumed in the premises during 
        /// the previous block period.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public UInt64 PreviousRIBBlockPeriodConsumptionDelivered
        {
            get
            {
                UInt64 uiConsumption = 0;

                if (null != Table2440)
                {
                    uiConsumption = Table2440.PreviousBlockPeriodConsumptionDelivered;
                }

                return uiConsumption;
            }
        }

        /// <summary>
        /// The value to be multiplied against the thresholds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public uint RIBBlockThresholdMultiplier
        {
            get
            {
                uint uiMultiplier = 0;

                if (null != Table2440)
                {
                    uiMultiplier = Table2440.Multiplier;
                }

                return uiMultiplier;
            }
        }

        /// <summary>
        /// The value to be divided against the thresholds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public uint RIBBlockThresholdDivisor
        {
            get
            {
                uint uiDivisor = 0;

                if (null != Table2440)
                {
                    uiDivisor = Table2440.Divisor;
                }

                return uiDivisor;
            }
        }

        /// <summary>
        /// The published price data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public PublishPriceDataEntryRcd RIBPublishedPriceData
        {
            get
            {
                PublishPriceDataEntryRcd PriceData = null;

                if (null != Table2440)
                {
                    PriceData = Table2440.PublishPriceData;
                }

                return PriceData;
            }
        }

        /// <summary>
        /// The billing period information currently being presented to the HAN.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public BillingPeriodRcd ActiveRIBBillingPeriod
        {
            get
            {
                BillingPeriodRcd BillingPeriod = null;

                if (null != Table2440)
                {
                    BillingPeriod = Table2440.ActiveBillingPeriod;
                }

                return BillingPeriod;
            }
        }

        /// <summary>
        /// The block period information currently being presented to the HAN.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public BlockPeriodRcd ActiveRIBBlockPeriod
        {
            get
            {
                BlockPeriodRcd BlockPeriod = null;

                if (null != Table2440)
                {
                    BlockPeriod = Table2440.ActiveBlockPeriod;
                }

                return BlockPeriod;
            }
        }

        /// <summary>
        /// The billing periods in the current schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public ReadOnlyCollection<BillingPeriodRcd> RIBBillingPeriods
        {
            get
            {
                ReadOnlyCollection<BillingPeriodRcd> BillingPeriods = null;

                if (null != Table2440)
                {
                    BillingPeriods = Table2440.BillingPeriods;
                }

                return BillingPeriods;
            }
        }

        /// <summary>
        /// The block periods in the current schedlule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ6032 Created
        //
        public ReadOnlyCollection<BlockPeriodRcd> RIBBlockPeriods
        {
            get
            {
                ReadOnlyCollection<BlockPeriodRcd> BlockPeriods = null;

                if (null != Table2440)
                {
                    BlockPeriods = Table2440.BlockPeriods;
                }

                return BlockPeriods;
            }
        }

        /// <summary>
        /// Determines if the given EDL file supports a 25 Year TOU schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/20/13 jrf 3.50.16 TQ9562 Created.
        //  01/03/14 jrf 3.50.18 TQ9629 Making sure 25 Year calendar table has a non-zero calendar ID.
        //  01/09/14 jrf 3.50.22 TQ9562 Caching value to prevent Create25YearCalendarFromStandardTables(...) 
        //                              from thrown an exception over and over again during program comparison
        //                              when program's TOU doesn't support 25 year TOU.
        //  02/18/14 jrf 3.50.36 322473 Bridge meters are only meters that support 25 year TOU. Setting false 
        //                              for non-Bridge device classes.
        //  04/22/14 jrf 3.50.81 490044 Modified check to call new method for detrmining if 25 year calendar is supported.
        public bool Supports25YearTOU
        {
            get
            {
                if (false == m_blnSupports25YearTOU.Cached)
                {
                    m_blnSupports25YearTOU.Value = false;

                    try
                    {
                        //Setting to false for devices classes that don't support the 25 Year TOU. 
                        if (DeviceClass != CENTRON_AMI.ITRD_DEVICE_CLASS && DeviceClass != CENTRON_AMI.ITRF_DEVICE_CLASS)
                        {
                            m_blnSupports25YearTOU.Value = false;
                        }
                        else if (IsProgramFile)
                        {
                            m_blnSupports25YearTOU.Value = m_CenTables.Supports25YearCalendarFromStandardTables(DateTime.Now);
                        }
                        else if (null != Table2437 && null != Table2437.CalendarConfig && 0 != Table2437.CalendarConfig.CalendarID)
                        {
                            m_blnSupports25YearTOU.Value = true;
                        }                        
                    }
                    catch (Exception)
                    {
                        // The TOU schedule defined in the program is not supported by the 25 year TOU schedule
                        m_blnSupports25YearTOU.Value = false;
                    }
                }

                return m_blnSupports25YearTOU.Value;
            }
        }

        /// <summary>
        /// List of TOU rates used in the TOU schedule.
        /// </summary>
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/14/14 jrf 3.50.24 TQ9563 Created to return the actual TOU rates used in schedule.
        //
        public List<Rate> TOURatesUsed
        {
            get
            {
                List<Rate> lstRates = new List<Rate>();

                if (TOU_ENABLED == TOUScheduleID)
                {
                    foreach (CPattern Pattern in TOUSchedule.Patterns)
                    {
                        foreach (CSwitchPoint Switchpoint in Pattern.SwitchPoints)
                        {
                            if (eSwitchPointType.RATE == Switchpoint.SwitchPointType && Enum.IsDefined(typeof(Rate), (byte)Switchpoint.RateOutputIndex))
                            {
                                if (false == lstRates.Contains((Rate)Switchpoint.RateOutputIndex))
                                {
                                    lstRates.Add((Rate)Switchpoint.RateOutputIndex);
                                }
                            }
                        }
                    }
                }

                return lstRates;
            }
        }

        /// <summary>
        /// Determines whether or not the power up threshold is supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/27/16 AF  4.50.224 RTT586620 Created
        //
        public bool? SupportsPowerUpThreshold
        {
            get
            {
                bool? blnSupported = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212PowerUpThreshold, null))
                {
                    m_CenTables.GetValue((long)CentronTblEnum.MfgTbl212PowerUpThreshold, null, out objValue);

                    if ((ushort)objValue == 0xFFFF || objValue == null)
                    {
                        blnSupported = false;
                    }
                    else
                    {
                        blnSupported = true;
                    }
                }

                return blnSupported;
            }
        }

        /// <summary>
        /// Reads the power up threshold from the EDL file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/27/16 AF  4.50.224 RTT586620 Created
        //
        public ushort PowerUpThreshold
        {
            get
            {
                ushort usPwrUpThreshold = 0xFFFF;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212PowerUpThreshold, null))
                {
                    m_CenTables.GetValue((long)CentronTblEnum.MfgTbl212PowerUpThreshold, null, out objValue);
                    usPwrUpThreshold = (ushort)objValue;
                }

                return usPwrUpThreshold;
            }
        }

        #endregion

        #region ICS Status

        /// <summary>
        /// Gets the manufacturer from table 2518
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string CellularManufacturer
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.Manufacturer;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the model from table 2518
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string CellularModel
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.Model;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the hardware version from table 2518
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string CellularHardwareVersion
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.HardwareVersion;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the firmware version from table 2518
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string CellularFirmwareVersion
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.FirmwareVersion;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the International Mobile Station Equipment Identity (IMEI) 
        /// OR 
        /// electronic serial number (ESN)
        /// OR 
        /// mobile equipment identifier (MEID)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string IMEI_or_ESN_or_MEID
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.IMEI_or_ESN_or_MEID;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the subscriber identity module integrated circuit card identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string SIM_ICC_ID
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.SIMICCID;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the International Mobile Subscriber Identity (IMSI) for GSM 
        /// OR 
        /// Mobile identification number (MIN) for Code division multiple access CDMA 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string IMSI_or_MIN
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.IMSIforGSMorMINforCDMA;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads signal strength
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method.
        //
        public sbyte? SignalStrength
        {
            get
            {
                sbyte? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471SignalStrength, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471SignalStrength, null, out Value);
                    returnValue = (sbyte)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads registration status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public RegistrationStatus? RegistrationStatus
        {
            get
            {
                RegistrationStatus? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471RegistrationStatus, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471RegistrationStatus, null, out Value);
                    returnValue = (RegistrationStatus)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads Network Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public string NetworkMode
        {
            get
            {
                string returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471NetworkMode, null))
                {
                    object Value;
                    byte[] abyValue;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471NetworkMode, null, out Value);
                    abyValue = (byte[])Value;
                    returnValue = Encoding.Default.GetString(abyValue);

                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Tower Identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt16? TowerIdentifier
        {
            get
            {
                UInt16? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471TowerIdentifier, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471TowerIdentifier, null, out Value);
                    returnValue = (UInt16)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Sector Identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt16? SectorIdentifier
        {
            get
            {
                UInt16? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471SectorIdentifier, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471SectorIdentifier, null, out Value);
                    returnValue = (UInt16)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Number of Cell Tower Changes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? NumberOfCellTowerChanges
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471CellTowerChangeCount, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471CellTowerChangeCount, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Link Connection State
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public LinkConnectionState? LinkConnectionState
        {
            get
            {
                LinkConnectionState? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471ConnectionState, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471ConnectionState, null, out Value);
                    returnValue = (LinkConnectionState)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Network Connection Up Time (seconds)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? NetworkConnectionUptime
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471ConnectionUptime, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471ConnectionUptime, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the IP Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public string IPAddress
        {
            get
            {
                string returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471IPAddress, null))
                {
                    object Value;
                    byte[] abyValue;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471IPAddress, null, out Value);
                    abyValue = (byte[])Value;
                    returnValue = Encoding.Default.GetString(abyValue);
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Gateway Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public string GatewayAddress
        {
            get
            {
                string returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471GatewayAddress, null))
                {
                    object Value;
                    byte[] abyValue;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471GatewayAddress, null, out Value);
                    abyValue = (byte[])Value;
                    returnValue = Encoding.Default.GetString(abyValue);
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Cumulative KiloBytes Sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? CumulativeKBytesSent
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471TotalKbytesSent, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471TotalKbytesSent, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Cumulative KiloBytes Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? CumulativeKBytesReceived
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471TotalKbytesReceived, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471TotalKbytesReceived, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Bytes Sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? BytesSent
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471BytesSent, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471BytesSent, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Bytes Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? BytesReceived
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471BytesReceived, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471BytesReceived, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Packets Delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? PacketsSent
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471PacketsSent, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471PacketsSent, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Packets Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? PacketsReceived
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471PacketsReceived, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471PacketsReceived, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Last Successful Tower Communication
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public DateTime LastSuccessfulTowerCommunication
        {
            get
            {
                DateTime returnValue = DateTime.MinValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471LastSuccessfulCommunicationTime, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471LastSuccessfulCommunicationTime, null, out Value);
                    returnValue = (DateTime)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Number Of Link Failures
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? NumberOfLinkFailures
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471LinkFailureCount, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471LinkFailureCount, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Number Of Link Failures
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public Int16? ModemTemperature
        {
            get
            {
                Int16? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471ModemTemperature, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471ModemTemperature, null, out Value);
                    returnValue = (Int16)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Last Modem Shutdown For Temperature
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public DateTime LastModemShutdownForTemperature
        {
            get
            {
                DateTime returnValue = DateTime.MinValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471LastHeatShutdownTime, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471LastHeatShutdownTime, null, out Value);
                    returnValue = (DateTime)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Last Modem Power Up After Temperature Shutdown
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public DateTime LastModemPowerUpAfterTemperatureShutdown
        {
            get
            {
                DateTime returnValue = DateTime.MinValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471LastHeatPowerUptime, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471LastHeatPowerUptime, null, out Value);
                    returnValue = (DateTime)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the MDM Radio Phone Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public string MDNRadioPhoneNumber
        {
            get
            {
                string returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471RadioPhoneNumber, null))
                {
                    object Value;
                    byte[] abyValue;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471RadioPhoneNumber, null, out Value);
                    abyValue = (byte[])Value;
                    returnValue = Encoding.Default.GetString(abyValue);

                    
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Number of Sector Identifier Changes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? NumberOfSectorIdentifierChanges
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471CellSectorChangeCount, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471CellSectorChangeCount, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Traffic Channels - good CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? TrafficChannelsGoodCRCCount
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471TrafficChannelsGoodCrcCount, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471TrafficChannelsGoodCrcCount, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Traffic Channels - bad CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? TrafficChannelsBadCRCCount
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471TrafficChannelsBadCrcCount, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471TrafficChannelsBadCrcCount, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Control Channels - good CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? ControlChannelsGoodCRCCount
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471ControlChannelsGoodCrcCount, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471ControlChannelsGoodCrcCount, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Control Channels - bad CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public UInt32? ControlChannelsBadCRCCount
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471ControlChannelsBadCrcCount, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471ControlChannelsBadCrcCount, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Figure of Merit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //  09/13/13 jrf 2.85.43 WR422355 Modified to pull value using GetValue method
        //
        public byte? FigureOfMerit
        {
            get
            {
                byte? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471FigureOfMerit, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471FigureOfMerit, null, out Value);
                    returnValue = (byte)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the name of the cellular carrier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/13/13 jrf 2.85.43 WR423256 Created
        //
        public string CellularCarrier
        {
            get
            {
                string returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl471CarrierName, null))
                {
                    object Value;
                    byte[] abyValue;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl471CarrierName, null, out Value);
                    abyValue = (byte[])Value;
                    returnValue = Encoding.Default.GetString(abyValue);
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Major part of the Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionMajor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMFirmwareVersionMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Minor part of the Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionMinor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMFirmwareVersionMinor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Revision part of the Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionRevision
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMFirmwareVersionRevision;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Extended Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string ICMExtendedFirmwareVersion
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMExtendedFirmwareVersion;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Hardware Version Major
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMHardwareVersionMajor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.HardwareVersionMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Hardware Version Minor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMHardwareVersionMinor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.HardwareVersionMinor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Number of Super Capacitors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMNumberSuperCapacitors
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.NumberSuperCapacitors;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Module Major part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberMajor
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMSerialNumberMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Module Minor part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberMinor
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMSerialNumberMinor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Module Build part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberBuild
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMSerialNumberBuild;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM CPU Identifier High 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMCPUIDHigh
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMCPUIDHigh;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM CPU Identifier Low
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMCPUIDLow
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMCPUIDLow;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Boot Loader Major part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMBootLoaderVersionMajor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.BootLoaderVersionMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Boot Loader Minor part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMBootLoaderVersionMinor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.BootLoaderVersionMinor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Boot Loader Revision part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMBootLoaderVersionRevision
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.BootLoaderVersionRevision;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Last Power Failure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime ICMLastPowerFailure
        {
            get
            {
                DateTime returnValue = DateTime.MinValue;

                if (Table2515 != null)
                {
                    returnValue = Table2515.LastPowerFailure;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Super Capacitor Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte? ICMSuperCapacitorStatus
        {
            get
            {
                byte? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.SuperCapacitorStatus;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reboot Count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? ICMRebootCount
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.RebootCount;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Uptime
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? ICMUptime
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.Uptime;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Module Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public ICMModuleStatus? ICMModuleStatus
        {
            get
            {
                ICMModuleStatus? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.ModuleStatus;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Module Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public Int16? ICMModuleTemerature
        {
            get
            {
                Int16? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.ModuleTemperature;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ERT statistics records from the ICS module.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //
        public List<ERTStatisticsRecord> ICMERTStatistics
        {
            get
            {
                List<ERTStatisticsRecord> StatisticsRecords = null;

                if (null != Table2510 && null != Table2511)
                {
                    if (Table2510.NumberOfStatisticsRecords > 0)
                    {
                        StatisticsRecords = Table2511.ERTStatisiticsRecords.ToList();
                    }
                }

                return StatisticsRecords;
            }
        }

        /// <summary>
        /// ERT consumption data records from the ICS module.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //
        public List<ERTConsumptionDataRecord> ICMERTData
        {
            get
            {
                List<ERTConsumptionDataRecord> DataRecords = null;

                if (null != Table2510 && null != Table2508)
                {
                    if (Table2510.NumberOfDataRecords > 0)
                    {
                        DataRecords = Table2508.ERTConsumptionDataRecords.ToList();
                    }
                }

                return DataRecords;
            }
        }

        #endregion // ICS Status

        #region ICS Configuration

        /// <summary>
        /// Whether or not EDL file supports ICS configuration table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/13 jrf 2.80.21 TQ8280 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Removing table check because it will not be all cached.
        //                              Device class should be sufficient.
        //
        public bool OpenWayICSModule
        {
            get
            {
                bool returnValue = false;

                if ((DeviceClass == CENTRON_AMI.ITRJ_DEVICE_CLASS
                    || DeviceClass == CENTRON_AMI.ITRK_DEVICE_CLASS))
                {
                    returnValue = true;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gateway Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/13 jrf 2.80.21 TQ8280 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public string ICMDisplayableGatewayAddress
        {
            get
            {
                string returnValue = null;
                DestinationAddressRecord GatewayAddress = null;
                byte[] abyAddress = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464GatewayAddress, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464GatewayAddress, null, out Value);
                    abyAddress = (byte[])Value;

                    GatewayAddress = new DestinationAddressRecord(DestinationAddressRecord.AddressType.IP, abyAddress);

                    returnValue = GatewayAddress.DisplayAddress;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// DNS Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/13 jrf 2.80.21 TQ8280 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public string ICMDisplayableDNSAddress
        {
            get
            {
                string returnValue = null;
                DestinationAddressRecord DNSAddress = null;
                byte[] abyAddress = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464DnsAddress, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464DnsAddress, null, out Value);
                    abyAddress = (byte[])Value;

                    DNSAddress = new DestinationAddressRecord(DestinationAddressRecord.AddressType.IP, abyAddress);

                    returnValue = DNSAddress.DisplayAddress;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// NTP Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/13 jrf 2.80.21 TQ8280 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public string ICMDisplayableNTPAddress
        {
            get
            {
                string returnValue = null;
                DestinationAddressRecord NTPAddress = null;
                byte[] abyAddress = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464NtpAddress, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464NtpAddress, null, out Value);
                    abyAddress = (byte[])Value;

                    NTPAddress = new DestinationAddressRecord(DestinationAddressRecord.AddressType.IP, abyAddress);

                    returnValue = NTPAddress.DisplayAddress;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// The power fail time or the minimum outage required before the ICS module 
        /// recognizes a power outage.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/13 jrf 2.80.21 TQ8280 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public UInt32? ICMPowerFailTime
        {
            get
            {
                UInt32? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464PowerFailTime, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464PowerFailTime, null, out Value);
                    returnValue = (UInt32)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// The NTP update frequency (in hours) is how often the ICS module asks the 
        /// SNTP server for the time.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/13 jrf 2.80.21 TQ8280 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public byte? ICMNTPUpdateFrequency
        {
            get
            {
                byte? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464NtpUpdateFrequency, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464NtpUpdateFrequency, null, out Value);
                    returnValue = (byte)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// The NTP valid time (in minutes) is how long the ICS time is valid after
        /// being recieved from the SNTP server.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/13 jrf 2.80.21 TQ8280 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public byte? ICMNTPValidTime
        {
            get
            {
                byte? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464NtpValidTime, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464NtpValidTime, null, out Value);
                    returnValue = (byte)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Link Failure Threshold
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public ushort? ICMLinkFailureThreshold
        {
            get
            {
                ushort? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464LinkFailuresThreshold, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464LinkFailuresThreshold, null, out Value);
                    returnValue = (ushort)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Tower Changes Threshold
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public ushort? ICMTowerChangesThreshold
        {
            get
            {
                ushort? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464TowerChangesThreshold, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464TowerChangesThreshold, null, out Value);
                    returnValue = (ushort)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Sector ID Changes Threshold
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public ushort? ICMSectorIDChangesThreshold
        {
            get
            {
                ushort? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464SectorIDChangesThreshold, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464SectorIDChangesThreshold, null, out Value);
                    returnValue = (ushort)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Link Failure Counter Reset Frequency
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.38 TQ8278 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public ushort? ICMLinkFailureCounterResetFrequency
        {
            get
            {
                ushort? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464LinkFailuresResetFrequency, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464LinkFailuresResetFrequency, null, out Value);
                    returnValue = (ushort)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Tower Changes Counter Reset Frequency
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.38 TQ8278 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public ushort? ICMTowerChangesCounterResetFrequency
        {
            get
            {
                ushort? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464TowerChangesResetFrequency, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464TowerChangesResetFrequency, null, out Value);
                    returnValue = (ushort)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Sector ID Changes Counter Reset Frequency
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.38 TQ8278 Created
        //  06/19/13 jrf 2.80.42 TQ8280 Reading just the value desired because full 
        //                              table may not be present.
        //
        public ushort? ICMSectorIDChangesCounterResetFrequency
        {
            get
            {
                ushort? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464SectorIDChangesResetFrequency, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464SectorIDChangesResetFrequency, null, out Value);
                    returnValue = (ushort)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Is ERT Data Populated in the ICS module.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/14/13 jrf 2.85.18 TQ 7655/
        //                       TQ 7656   Created.
        //
        public bool? ICMIsERTDataPopulated
        {
            get
            {
                bool? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl464IsERTPopulated, null))
                {
                    object Value;
                    byte byValue;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl464IsERTPopulated, null, out Value);
                    byValue = (byte)Value;

                    if (1 == byValue)
                    {
                        returnValue = true;
                    }
                    else if (0 == byValue)
                    {
                        returnValue = false;
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Cellular data timeout
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------  -------------------------------------------
        //  07/30/13 jkw 2.85.05 TC13198 Created
        //  08/02/13 btr 2.85.08         Now references the correct yet deceptively named enumeration
        //
        public byte? ICMCellularDataTimeout
        {
            get
            {
                byte? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl469CellularDataTimeoutTimeout, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl469CellularDataTimeoutTimeout, null, out Value);
                    returnValue = (byte)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Cellular data timeout units
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------  -------------------------------------------
        //  07/30/13 jkw 2.85.05 TC13198 Created
        //
        public ICMMfgTable2517CellularConfiguration.TimeoutUnits? ICMCellularDataTimeoutUnits
        {
            get
            {
                ICMMfgTable2517CellularConfiguration.TimeoutUnits? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl469CellularDataTimeoutUnits, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl469CellularDataTimeoutUnits, null, out Value);
                    returnValue = (ICMMfgTable2517CellularConfiguration.TimeoutUnits)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ERT Radio Enabled
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/14/13 jrf 2.85.18 TQ 7655/
        //                       TQ 7656   Created.
        //  12/11/14 AF  4.00.91 WR 551378 The EDL program files for ITRD meters will have the
        //                                 ERT radio item even though it's not supported so we are forced
        //                                 to use a device class check
        //
        public bool? ICMERTRadioEnabled
        {
            get
            {
                bool? returnValue = null;

                if (String.Equals(DeviceClass, CENTRON_AMI.ITRJ_DEVICE_CLASS, StringComparison.OrdinalIgnoreCase) || 
                    String.Equals(DeviceClass, CENTRON_AMI.ITRK_DEVICE_CLASS, StringComparison.OrdinalIgnoreCase))
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl461ERTRadio, null))
                    {
                        object Value;
                        byte byValue;
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl461ERTRadio, null, out Value);
                        byValue = (byte)Value;

                        if (1 == byValue)
                        {
                            returnValue = true;
                        }
                        else if (0 == byValue)
                        {
                            returnValue = false;
                        }
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ERT Data Lifetime
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/30/13 jrf 3.00.10 TC 15682  Created.
        //
        public byte? ICMERTDataLifetime
        {
            get
            {
                byte? returnValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl461DataLifetime, null))
                {
                    object Value;
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl461DataLifetime, null, out Value);
                    returnValue = (byte)Value;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ERT Gas supported
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/14/13 jrf 4.70.28 WR 230427 Created
        //
        public bool? SupportERTGas
        {
            get
            {
                bool? returnValue = null;

                if (String.Equals(DeviceClass, CENTRON_AMI.ITRJ_DEVICE_CLASS, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(DeviceClass, CENTRON_AMI.ITRK_DEVICE_CLASS, StringComparison.OrdinalIgnoreCase))
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl461100GMeterSupport, null))
                    {
                        object Value;
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl461100GMeterSupport, null, out Value);
                        returnValue = (bool)Value;
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ERT Water supported
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/14/13 jrf 4.70.28 WR 230427 Created
        //
        public bool? SupportERTWater
        {
            get
            {
                bool? returnValue = null;

                if (String.Equals(DeviceClass, CENTRON_AMI.ITRJ_DEVICE_CLASS, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(DeviceClass, CENTRON_AMI.ITRK_DEVICE_CLASS, StringComparison.OrdinalIgnoreCase))
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl461100WPlusMeterSupport, null))
                    {
                        object Value;
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl461100WPlusMeterSupport, null, out Value);
                        returnValue = (bool)Value;
                    }
                    else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl461100WMeterSupport, null))
                    {
                        object Value;
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl461100WMeterSupport, null, out Value);
                        returnValue = (bool)Value;
                    }
                }

                return returnValue;
            }
        }

        #endregion //ICS Configuration

        #region Itron Device Configuration

        /// <summary> Gets the DST Flag </summary>
        /// // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/10/08 KRC
        public virtual bool DSTEnabled
        {
            get
            {
                if (!m_blnDSTEnabled.Cached)
                {
                    m_blnDSTEnabled.Value = GetSTDEDLBool(StdTableEnum.STDTBL52_DST_APPLIED_FLAG);
                }

                return m_blnDSTEnabled.Value;
            }
        }

        /// <summary> 
        /// Gets the DST supported Flag, i.e. indicates if the meter will change 
        /// its time based on DST.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/17/10 jrf 2.45.13 N/A    Created
        //
        public virtual bool DSTSupported
        {
            get
            {
                return GetSTDEDLBool(StdTableEnum.STDTBL52_DST_SUPPORTED_FLAG);
            }
            set
            {
                SetSTDEDLBool(value, StdTableEnum.STDTBL52_DST_SUPPORTED_FLAG);
            }
        }

        /// <summary>
        /// Property used to get the device time (DateTime) from the meter
        /// </summary>
        public virtual DateTime DeviceTime
        {
            get
            {
                if (!m_dtCurrentTime.Cached)
                {
                    string strTemp = GetSTDEDLString(StdTableEnum.STDTBL52_CLOCK_CALENDAR);
                    m_dtCurrentTime.Value = (DateTime)DateTime.Parse(strTemp, CultureInfo.InvariantCulture);
                }

                return m_dtCurrentTime.Value;
            }
        }

        /// <summary>Returns the Full Firmware Version and Revision</summary>
        public virtual float FWRevision
        {
            get
            {
                if (!m_fltFWRevision.Cached)
                {
                    string strVersion;
                    string strRevision;

                    strVersion = GetSTDEDLString(StdTableEnum.STDTBL1_FW_VERSION_NUMBER);
                    strRevision = GetSTDEDLString(StdTableEnum.STDTBL1_FW_REVISION_NUMBER);
                    m_fltFWRevision.Value = (float)float.Parse(strVersion, CultureInfo.InvariantCulture) +
                        ((float)float.Parse(strRevision, CultureInfo.InvariantCulture) / (int)1000);
                }

                return m_fltFWRevision.Value;
            }
        }

        /// <summary>Returns the Full Firmware Version and Revision</summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        public virtual byte FirmwareBuild
        {
            get
            {
                object Value;

                if (!m_byFWBuild.Cached && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_REGISTER_FW_BUILD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL60_REGISTER_FW_BUILD, null, out Value);
                    m_byFWBuild.Value = (byte)Value;
                }
#if (!WindowsCE)
                else if (!m_byFWBuild.Cached && m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL60_GATEWAY_FW_BUILD, null))
                {
                    m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL60_GATEWAY_FW_BUILD, null, out Value);
                    m_byFWBuild.Value = (byte)Value;
                }
#endif

                return m_byFWBuild.Value;
            }
        }

        /// <summary>
        /// Get the exception configuration as a list of event descriptions
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/12 AF  2.53.52 191613 Created
        //  10/31/16 jrf 4.70.28 230427 Added check for ICS exceptions
        public List<string> ExceptionConfiguration
        {
            get
            {
                List<string> ExceptionConfigItems = new List<string>();
                List<ushort> ExceptionConfigIds = ExceptionConfig;
                string strEvent = "";

                foreach (ushort usEvent in ExceptionConfigIds)
                {
                    if (usEvent != 0)
                    {
                        if (m_EventDictionary.TryGetValue((int)usEvent, out strEvent) == false)
                        {
                            if (m_ICSEventDictionary.TryGetValue((int)usEvent, out strEvent) == false)
                            {
                                UInt16 usTempEvent = usEvent;
                                if ((usTempEvent & EXCEPTION_REPORT_SELECTOR_MASK) == EXCEPTION_REPORT_SELECTOR_MASK)
                                {
                                    usTempEvent -= EXCEPTION_REPORT_SELECTOR_MASK;
                                }

                                if (m_EventDictionary.TryGetValue((int)usTempEvent, out strEvent) == false)
                                {
                                    // The TryGetValue failed so say it is an unknown event.
                                    strEvent = "Unknown Event " + usEvent.ToString(CultureInfo.InvariantCulture);
                                }
                            }                            
                        }

                        ExceptionConfigItems.Add(strEvent);
                    }
                }

                return ExceptionConfigItems;
            }
        }

        /// <summary>
        /// Get the Exception Configuration as a list of event ids
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??       ??                 Created
        //  03/26/12 AF  2.53.52 191613 Added M2 Gateway support
        //  10/31/16 jrf 4.70.28 230427 Added retrieval of ICS exceptions
        //  11/17/16 AF  4.70.35 714618 Add the primary power down exception if it is not already there.
        //                              This exception is always enabled regardless of the config.
        //
        public virtual List<ushort> ExceptionConfig
        {
            get
            {
                List<ushort> lstAlarms = new List<ushort>();

                if (m_CenTables.IsAllCached(123))
                {
                    int[] anLimits = m_CenTables.GetElementLimits(StdTableEnum.STDTBL123_EVENT_REPORTED);
                    for (int ndxHost = 0; ndxHost < anLimits[0]; ndxHost++)
                    {
                        for (int ndxEvent = 0; ndxEvent < anLimits[1]; ndxEvent++)
                        {
                            ushort alarm = GetSTDEDLUShort(StdTableEnum.STDTBL123_EVENT_REPORTED, ndxHost, ndxEvent);
                            if (alarm != 0)
                            {
                                lstAlarms.Add(alarm);
                            }
                        }
                    }

                    if (!(lstAlarms.Contains((ushort)CANSIDevice.HistoryEvents.PRIMARY_POWER_DOWN)))
                    {
                        lstAlarms.Add((ushort)CANSIDevice.HistoryEvents.PRIMARY_POWER_DOWN);
                    }

                    if (m_CenTables.IsAllCached(2537))
                    {
                        int[] ICMExpLimits = m_CenTables.GetElementLimits(CentronTblEnum.MFGTBL489_EVENT_REPORTED);
                        for (int ndxHost = 0; ndxHost < ICMExpLimits[0]; ndxHost++)
                        {
                            for (int ndxEvent = 0; ndxEvent < ICMExpLimits[1]; ndxEvent++)
                            {
                                ushort alarm = GetMFGEDLUShort(CentronTblEnum.MFGTBL489_EVENT_REPORTED, ndxHost, ndxEvent);
                                if (alarm != 0)
                                {
                                    lstAlarms.Add(alarm);
                                }
                            }
                        }
                    }

                }
                else
                {
                    if (m_GatewayTables.IsAllCached(123))
                    {
                        int[] anLimits = m_GatewayTables.GetElementLimits(StdTableEnum.STDTBL123_EVENT_REPORTED);
                        for (int ndxHost = 0; ndxHost < anLimits[0]; ndxHost++)
                        {
                            for (int ndxEvent = 0; ndxEvent < anLimits[1]; ndxEvent++)
                            {
                                ushort alarm = GetSTDEDLUShort(StdTableEnum.STDTBL123_EVENT_REPORTED, ndxHost, ndxEvent);
                                if (alarm != 0)
                                {
                                    lstAlarms.Add(alarm);
                                }
                            }
                        }
                    }
                }

                // The primary power down exception is enabled by default in all OpenWay meters so make sure it is added to the list
                // even if the config doesn't show it enabled.  On the other hand, it's not clear that the same is true for the 
                // M2 Gateway, so exclude it there unless it is explicitly enabled.
                if (!(lstAlarms.Contains((ushort)CANSIDevice.HistoryEvents.PRIMARY_POWER_DOWN)) && !(m_GatewayTables.IsAllCached(123)))
                {
                    lstAlarms.Add((ushort)CANSIDevice.HistoryEvents.PRIMARY_POWER_DOWN);
                }

                return lstAlarms;
            }
        }

        /// <summary>Returns the device ID read from table 5</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/07 mcm 8.10.04		   Created
        // 01/23/08 AF  10.0           Added check to see if item exists - we
        //                             get an exception if it doesn't
        public virtual string UnitID
        {
            get
            {
                if (!m_strDeviceID.Cached)
                {
                    m_strDeviceID.Value = GetSTDEDLString(StdTableEnum.STDTBL5_IDENTIFICATION).Trim('\0');
                }

                return m_strDeviceID.Value.Trim();

            }//get
        }//UnitID

        /// <summary>
        /// Returns the Customer Serial Number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/10/08 KRC
        //
        public virtual string SerialNumber
        {
            get
            {
                if (!m_strSerialNumber.Cached)
                {
                    m_strSerialNumber.Value = GetSTDEDLString(StdTableEnum.STDTBL6_UTIL_SER_NO).Trim('\0');
                }

                return m_strSerialNumber.Value;
            }
        }

        /// <summary>
        /// Get the TOU Flag
        /// </summary>
        public bool TOUEnabled
        {
            get
            {
                //TODO: Figure out how to determine if TOU is Enabled from EDL file
                return false;
            }
        }

        /// <summary>
        /// Property used to get the program ID (int) from the meter
        /// </summary>
        public virtual int ProgramID
        {
            get
            {
                if (!m_iProgramID.Cached)
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_PROGRAM_ID, null))
                    {
                        m_iProgramID.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_PROGRAM_ID);
                    }
                }

                return m_iProgramID.Value;
            }
        }

        /// <summary>Returns the Full Software Version and Revision</summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/10 AF  2.40.49        If 2048 is unavailable make sure this doesn't crash
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual string SWRevision
        {
            get
            {
                if (!m_strSWRevision.Cached)
                {
                    string strTemp = ".";

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SW_VERSION, null))
                    {
                        strTemp = GetMFGEDLString(CentronTblEnum.MFGTBL0_SW_VERSION);
                        strTemp += "." + GetMFGEDLString(CentronTblEnum.MFGTBL0_SW_REVISION);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsCached((long)StdTableEnum.STDTBL6_EX1_SW_VERSION_NUMBER, null))
                    {
                        strTemp = GetSTDEDLString(StdTableEnum.STDTBL6_EX2_SW_VERSION_NUMBER);
                        strTemp += "." + GetSTDEDLString(StdTableEnum.STDTBL6_EX2_SW_REVISION_NUMBER);
                    }
#endif

                    if (strTemp == ".")
                    {
                        m_strSWRevision.Value = "Unavailable";
                    }
                    else
                    {
                        float fltSWVersion = (float)float.Parse(strTemp, CultureInfo.InvariantCulture);
                        m_strSWRevision.Value = fltSWVersion.ToString("F2", CultureInfo.CurrentCulture);
                    }
                }

                return m_strSWRevision.Value;
            }
        }

        /// <summary>
        /// Reads the standard table 06 tariff id out of the EDL file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/08 AF  10.0           Created for OpenWay DataManager
        //  06/11/08 KRC 1.50.34 116044 TOU ID does not make sense so just show it is enabled.
        //  03/27/12 RCG 2.53.52 195665 Changing the TOU ID so that it is set to "Enabled" when Tariff ID or Calendar ID is set

        public virtual string TOUScheduleID
        {
            get
            {
                if (!m_strTOUID.Cached)
                {
                    string TOUID;
                    TOUID = GetSTDEDLString(StdTableEnum.STDTBL6_TARIFF_ID);

                    // Remove any nulls that might be at the end
                    TOUID = TOUID.TrimEnd('\0');

                    if (TOUID.Length > 0 || CalendarID > 0)
                    {
                        m_strTOUID.Value = TOU_ENABLED;
                    }
                    else
                    {
                        m_strTOUID.Value = "";
                    }
                }

                return m_strTOUID.Value;
            }
        }

        /// <summary>
        /// Gets/sets the standard table 06 tariff id of the EDL file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- -------------- -------------------------------------------
        //  10/25/10 jrf 2.45.10                 Created
        //
        public virtual string TarrifID
        {
            get
            {
                if (!m_strTarrifID.Cached)
                {
                    string TOUID;
                    TOUID = GetSTDEDLString(StdTableEnum.STDTBL6_TARIFF_ID);

                    // Remove any nulls that might be at the end
                    TOUID = TOUID.TrimEnd('\0');
                    if (TOUID.Length > 0)
                    {
                        m_strTarrifID.Value = TOUID;
                    }
                    else
                    {
                        m_strTarrifID.Value = "";
                    }
                }
                return m_strTarrifID.Value;
            }
            set
            {
                string strValue = "";

                if (TARRIF_ID_MAX_LENGTH < value.Length)
                {
                    strValue = value.Substring(0, 8);
                }
                else
                {
                    strValue = value;
                }

                SetSTDEDLString(strValue, StdTableEnum.STDTBL6_TARIFF_ID);
                m_strTarrifID.Value = value;
            }
        }

        /// <summary>
        /// This property returns a list of user data strings.  If the meter has 3 user data fields
        /// then the list will contain 3 strings corresponding to each user data  field
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/06 MAH 8.00    N/A    Created
        // 11/05/10 jrf 2.45.11        Added set.
        //
        public virtual List<String> UserData
        {
            get
            {
                List<String> UserDataList = new List<String>();

                if (!m_strUserData1.Cached)
                {
                    m_strUserData1.Value = GetMFGEDLString(CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 0);
                    m_strUserData2.Value = GetMFGEDLString(CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 1);
                    m_strUserData3.Value = GetMFGEDLString(CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 2);
                }

                UserDataList.Add(m_strUserData1.Value);
                UserDataList.Add(m_strUserData2.Value);
                UserDataList.Add(m_strUserData3.Value);

                return UserDataList;
            }
            set
            {
                if (value.Count > 0)
                {
                    m_strUserData1.Value = value[0];
                    SetMFGEDLString(value[0], CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 0);
                }

                if (value.Count > 1)
                {
                    m_strUserData2.Value = value[1];
                    SetMFGEDLString(value[1], CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 1);
                }

                if (value.Count > 2)
                {
                    m_strUserData3.Value = value[2];
                    SetMFGEDLString(value[2], CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 2);
                }
            }
        }

        /// <summary>
        /// Gets/sets the Cold Load Pickup Time in minutes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public virtual uint ColdLoadPickupTime
        {
            get
            {
                if (!m_uiColdLoadPickupTime.Cached)
                {
                    m_uiColdLoadPickupTime.Value = (uint)GetMFGEDLInt(CentronTblEnum.MFGTBL0_COLD_LOAD_PICKUP);
                }

                return m_uiColdLoadPickupTime.Value;
            }
            set
            {
                m_uiColdLoadPickupTime.Value = value;
                SetMFGEDLInt((int)value, CentronTblEnum.MFGTBL0_COLD_LOAD_PICKUP);
            }
        }

        /// <summary>
        /// Gets/Sets the Interval Length for Demands
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public virtual int DemandIntervalLength
        {
            get
            {
                if (!m_iDemandIntervalLength.Cached)
                {
                    m_iDemandIntervalLength.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH);
                }

                return m_iDemandIntervalLength.Value;
            }
            set
            {
                m_iDemandIntervalLength.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH);
            }
        }

        /// <summary>
        /// Gets/Sets the Number of Sub Intervals for Demands
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public virtual int NumberOfSubIntervals
        {
            get
            {
                if (!m_iNumDemandSubIntervals.Cached)
                {
                    m_iNumDemandSubIntervals.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_NUM_SUB_INTERVALS);
                }

                return m_iNumDemandSubIntervals.Value;
            }
            set
            {
                m_iNumDemandSubIntervals.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_NUM_SUB_INTERVALS);
            }

        }

        /// <summary>
        /// Gets/Sets the demand reset schedule information.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public virtual DemandResetSchedule DemandSchedulingControl
        {
            get
            {
                DemandResetSchedule DRSchedule = DemandResetSchedule.Disabled;

                if (false == m_iDemandSchedulingControl.Cached)
                {
                    m_iDemandSchedulingControl.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_SCHEDULING_CONTROL);
                }

                if (true == Enum.IsDefined(typeof(DemandResetSchedule), m_iDemandSchedulingControl.Value))
                {
                    DRSchedule = (DemandResetSchedule)m_iDemandSchedulingControl.Value;
                }

                return DRSchedule;
            }
            set
            {
                m_iDemandSchedulingControl.Value = (int)value;

                if (true == Enum.IsDefined(typeof(DemandResetSchedule), value))
                {
                    SetMFGEDLInt((int)value, CentronTblEnum.MFGTBL0_SCHEDULING_CONTROL);
                }
                else
                {
                    SetMFGEDLInt((int)DemandResetSchedule.Disabled, CentronTblEnum.MFGTBL0_SCHEDULING_CONTROL);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the day the demand reset is scheduled on.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public virtual int DemandResetScheduledDay
        {
            get
            {
                if (false == m_iDemandResetScheduledDay.Cached)
                {
                    m_iDemandResetScheduledDay.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DR_SCHEDULED_DAY);
                }

                return m_iDemandResetScheduledDay.Value;
            }
            set
            {
                m_iDemandResetScheduledDay.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_DR_SCHEDULED_DAY);
            }
        }

        /// <summary>
        /// Gets/sets the hour the demand reset is scheduled on.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public virtual int DemandResetScheduledHour
        {
            get
            {
                if (false == m_iDemandResetScheduledHour.Cached)
                {
                    m_iDemandResetScheduledHour.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DR_SCHEDULED_HOUR);
                }

                return m_iDemandResetScheduledHour.Value;
            }
            set
            {
                m_iDemandResetScheduledHour.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_DR_SCHEDULED_HOUR);
            }
        }

        /// <summary>
        /// Gets/sets the hour the demand reset is scheduled on.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public virtual int DemandResetScheduledMinute
        {
            get
            {
                if (false == m_iDemandResetScheduledMinute.Cached)
                {
                    m_iDemandResetScheduledMinute.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DR_SCHEDULED_MINUTE);
                }

                return m_iDemandResetScheduledMinute.Value;
            }
            set
            {
                m_iDemandResetScheduledMinute.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_DR_SCHEDULED_MINUTE);
            }
        }

        /// <summary>
        /// Gets and Sets the list of configured Demands Thresholds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public virtual List<LID> DemandThresholdLIDs
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                LID DemandLID;
                List<LID> DemandThresholdLIDs = new List<LID>();

                for (i = 0; i < DetermineDemandThresholdCount(); i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_THRESHOLD_SOURCE,
                        anIndex1, out Value);

                    if (0 == (uint)Value)
                    {
                        //This means the threshold is not configured.  Let's return an not progrmammed LID.
                        DefinedLIDs LIDs = new DefinedLIDs();
                        DemandLID = LIDs.DEMAND_NOT_PROGRAMMED;
                    }
                    else
                    {
                        DemandLID = new LID((uint)Value);
                    }

                    DemandThresholdLIDs.Add(DemandLID);
                }

                return DemandThresholdLIDs;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };

                for (int i = 0; i < DetermineDemandThresholdCount(); i++)
                {
                    anIndex1[0] = i;

                    if (true == value[i].IsMaxDemand && false == value[i].IsCoincident)
                    {
                        Value = value[i].lidValue;
                    }
                    else
                    {
                        //Threshold must be set to 0 when it is not configured.
                        Value = 0;
                    }

                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_THRESHOLD_SOURCE, anIndex1, Value);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the demand threshold values.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public virtual List<double> DemandThresholdValues
        {
            get
            {
                object objValue;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                List<double> lstdblDemandThresholdValues = new List<double>();

                for (i = 0; i < DetermineDemandThresholdCount(); i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_THRESHOLD_LEVEL,
                        anIndex1, out objValue);

                    lstdblDemandThresholdValues.Add((double)objValue / 1000.0);
                }

                return lstdblDemandThresholdValues;
            }
            set
            {
                object objValue;
                int[] anIndex1 = { 0 };

                for (int i = 0; i < DetermineDemandThresholdCount(); i++)
                {
                    anIndex1[0] = i;

                    objValue = value[i] * 1000;

                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_THRESHOLD_LEVEL, anIndex1, objValue);
                }
            }
        }

        /// <summary>
        /// Gets the Number of Test Mode Sub Intervals for Demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //
        public virtual int NumberOfTestModeSubIntervals
        {
            get
            {
                if (!m_iNumTestModeDemandSubIntervals.Cached)
                {
                    m_iNumTestModeDemandSubIntervals.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_NUM_TEST_MODE_SUBINTERVALS);
                }

                return m_iNumTestModeDemandSubIntervals.Value;
            }
        }

        /// <summary>
        /// Gets the Test Mode Interval Length for Demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //
        public virtual int TestModeIntervalLength
        {
            get
            {
                if (!m_iTestModeDemandIntervalLength.Cached)
                {
                    m_iTestModeDemandIntervalLength.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_TEST_MODE_INTERVAL_LENGTH);
                }

                return m_iTestModeDemandIntervalLength.Value;
            }
        }

        /// <summary>
        /// Returns the number of minutes per load profile interval
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  12/05/06 MAH 8.00.00
        /// </remarks>
        public virtual int LPIntervalLength
        {
            get
            {
                if (!m_iLPIntervalLength.Cached)
                {
                    m_iLPIntervalLength.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_LP_INTERVAL_LENGTH);
                }

                return m_iLPIntervalLength.Value;
            }
            set
            {
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_LP_INTERVAL_LENGTH);
            }
        }

        /// <summary>
        /// Gets the Extended Load Profile Interval Length from the EDL file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/25/11 RCG 2.53.34 TC6885 Created

        public byte ExtendedLoadProfileIntervalLength
        {
            get
            {
                byte IntervalLength = 0;
                object Value = null;

                if(m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileIntervalLength, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217NonBillingLoadProfileIntervalLength, null, out Value);
                    IntervalLength = (byte)Value;
                }

                return IntervalLength;
            }
        }

        /// <summary>
        /// Gets the list of Extended Load Profile quantities configured in the meter from the configuration data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ2950 Created 
        // 10/31/16 jrf 4.70.28 230427 Modified unselected energy text to be consistent with CE UI.
        public List<string> ExtendedLoadProfileQuantityList
        {
            get
            {
                object Value;
                CentronAMILID QuantityLID;
                List<String> ConfigList = new List<String>();

                for (int CurrentIndex = 0; CurrentIndex < ExtendedLoadProfileNumberOfChannels; CurrentIndex++)
                {
                    int[] Indexer = { CurrentIndex };
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier, Indexer, out Value);
                    QuantityLID = new CentronAMILID((uint)Value);                    

                    //Changing unselected quantity to display as "None" for consistency with the CE UI.
                    if (UNASSIGNED != QuantityLID.lidDescription && false == string.IsNullOrEmpty(QuantityLID.lidDescription))
                    {
                        ConfigList.Add(QuantityLID.lidDescription);
                    }
                    else
                    {
                        ConfigList.Add(Resources.NONE);
                    }
                }

                return ConfigList;
            }
        }

        /// <summary>
        /// Gets the list of Extended Load Profile LIDs configured in the meter from the configuration data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 06/14/12 JKW 2.60.xx         Created 

        public List<LID> ExtendedLoadProfileQuantityLIDs
        {
            get
            {
                object Value;
                CentronAMILID QuantityLID;
                List<LID> LIDList = new List<LID>();

                for (int CurrentIndex = 0; CurrentIndex < ExtendedLoadProfileNumberOfChannels; CurrentIndex++)
                {
                    int[] Indexer = { CurrentIndex };
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier, Indexer, out Value);
                    QuantityLID = new CentronAMILID((uint)Value);
                    LIDList.Add(QuantityLID);
                }

                return LIDList;
            }
        }

        /// <summary>
        /// Gets the list of Extended Load Profile Pulse Weights from the configuration data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ2950 Created 

        public List<float> ExtendedLoadProfilePulseWeightList
        {
            get
            {
                object Value;
                List<float> PulseWeightList = new List<float>();

                for (int CurrentIndex = 0; CurrentIndex < ExtendedLoadProfileNumberOfChannels; CurrentIndex++)
                {
                    int[] Indexer = { CurrentIndex };
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeight, Indexer, out Value);
                    PulseWeightList.Add((ushort)Value * 0.01F);
                }

                return PulseWeightList;
            }
        }

        /// <summary>
        /// Gets the Extended Load Profile Outage length from the configuration data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ2950 Created 
        
        public ushort ExtendedLoadProfileOutageLength
        {
            get
            {
                ushort OutageLength = 0;

                if(m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileMinimumOutage, null))
                {
                    object objValue;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217NonBillingLoadProfileMinimumOutage, null, out objValue);
                    OutageLength = (byte)objValue;
                }

                return OutageLength;
            }
        }

        /// <summary>
        /// Gets the number of channels configured for Instrumentation Profile
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ2950 Created 
        
        public byte ExtendedLoadProfileNumberOfChannels
        {
            get
            {
                byte NumberOfChannels = 0;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217NonBillingLoadProfileNumberOfChannels, null))
                {
                    object objValue = null;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217NonBillingLoadProfileNumberOfChannels, null, out objValue);
                    NumberOfChannels = (byte)objValue;

                    if (NumberOfChannels == 0xFF)
                    {
                        // This means it has never been configured
                        NumberOfChannels = 0;
                    }
                }

                return NumberOfChannels;
            }
        }

        /// <summary>
        /// Gets the Instrumentation Profile Interval Length from the EDL file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/25/11 RCG 2.53.34 TC6885 Created

        public byte InstrumentationProfileIntervalLength
        {
            get
            {
                byte IntervalLength = 0;
                object Value = null;

                if(m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217InstrumentationProfileIntervalLength, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217InstrumentationProfileIntervalLength, null, out Value);
                    IntervalLength = (byte)Value;
                }

                return IntervalLength;
            }
        }

        /// <summary>
        /// Gets the list of Instrumentation Profile quantities configured in the meter from the configuration data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ2950 Created 

        public List<string> InstrumentationProfileQuantityList
        {
            get
            {
                object Value;
                CentronAMILID QuantityLID;
                List<String> ConfigList = new List<String>();

                for (int CurrentIndex = 0; CurrentIndex < InstrumentationProfileNumberOfChannels; CurrentIndex++)
                {
                    int[] Indexer = { CurrentIndex };
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier, Indexer, out Value);
                    QuantityLID = new CentronAMILID((uint)Value);
                    ConfigList.Add(QuantityLID.lidDescription);
                }

                return ConfigList;
            }
        }

        /// <summary>
        /// Gets the list of Instrumentation Profile quantity LIDs configured in the meter from the 
        /// configuration data AND OR'S IN THE CPC SECONDARY FORMAT (raw is what is configured but not very useful
        /// and even causes exceptions due to the fact that the LID retrieval mechanism assumes a float for an inst 
        /// value when raw returns pulses in UINT32 format)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 JKW 2.60.xx N/A     Created 

        public List<Device.LID> InstrumentationProfileQuantityLIDsSecondaryFormat
        {
            get
            {
                object Value;
                CentronAMILID QuantityLID;
                List<Device.LID> ConfigList = new List<Device.LID>();

                for (int CurrentIndex = 0; CurrentIndex < InstrumentationProfileNumberOfChannels; CurrentIndex++)
                {
                    int[] Indexer = { CurrentIndex };
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier, Indexer, out Value);
                    QuantityLID = new CentronAMILID((uint)Value | (uint)Itron.Metering.Device.DefinedLIDs.MetrologyDataFormat.CPC_FORMAT_SEC);
                    ConfigList.Add(QuantityLID);
                }

                return ConfigList;
            }
        }

        /// <summary>
        /// Gets the Instrumentation Profile Outage length from the configuration data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ2950 Created 

        public ushort InstrumentationProfileOutageLength
        {
            get
            {
                ushort OutageLength = 0;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217InstrumentationProfileMinimumOutage, null))
                {
                    object objValue;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217InstrumentationProfileMinimumOutage, null, out objValue);
                    OutageLength = (byte)objValue;
                }

                return OutageLength;
            }
        }

        /// <summary>
        /// Gets the Instrumentation Profile Read Offset length from the configuration data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ2950 Created 

        public ushort InstrumentationProfileReadOffsetLength
        {
            get
            {
                ushort OutageLength = 0;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217InstrumentationProfileInstrumentationProfileOffset, null))
                {
                    object objValue;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217InstrumentationProfileInstrumentationProfileOffset, null, out objValue);
                    OutageLength = (ushort)objValue;
                }

                return OutageLength;
            }
        }

        /// <summary>
        /// Gets the number of channels configured for Instrumentation Profile
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ2950 Created 

        public byte InstrumentationProfileNumberOfChannels
        {
            get
            {
                byte NumberOfChannels = 0;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217InstrumentationProfileNumberOfChannels, null))
                {
                    object objValue = null;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl217InstrumentationProfileNumberOfChannels, null, out objValue);
                    NumberOfChannels = (byte)objValue;

                    if (NumberOfChannels == 0xFF)
                    {
                        // This means it has never been configured
                        NumberOfChannels = 0;
                    }
                }

                return NumberOfChannels;
            }
        }

        /// <summary>
        /// Gets the list of Configured Extend Self Read Quantities
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ3448 Created 

        public List<ExtendedSelfReadConfigRecord> ExtendedSelfReadConfigList
        {
            get
            {
                List<ExtendedSelfReadConfigRecord> ConfigList = null;

                if(m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier, new int[] {0}))
                {
                    ConfigList = new List<ExtendedSelfReadConfigRecord>();

                    for(int iIndex = 0; iIndex < NUMBER_EXT_SELF_READ_QUANTITIES; iIndex++)
                    {
                        object objValue;
                        uint LIDValue;
                        byte QualifierValue;

                        m_CenTables.GetValue(CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier, new int[] {iIndex}, out objValue);
                        LIDValue = (uint)objValue;

                        m_CenTables.GetValue(CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifierQualifier, new int[] {iIndex}, out objValue);
                        QualifierValue = (byte)objValue;

                        if(LIDValue != 0 && LIDValue != uint.MaxValue)
                        {
                            ConfigList.Add(new ExtendedSelfReadConfigRecord(new CentronAMILID(LIDValue), (ExtendedSelfReadQualifier)QualifierValue));
                        }
                    }
                }

                return ConfigList;
            }
        }

        /// <summary>
        /// Gets/sets the number of load profile channels the meter is 
        /// currently recording
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public virtual int NumberLPChannels
        {
            get
            {
                if (!m_iNumLPChannels.Cached)
                {
                    m_iNumLPChannels.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_LP_NBR_CHANNELS);
                }

                return m_iNumLPChannels.Value;
            }
            set
            {
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_LP_NBR_CHANNELS);
                m_iNumLPChannels.Value = value;
            }
        }

        #endregion Itron Device Configuration

        #region ANSI Device Configuration

        /// <summary>Returns the Manufacturer Serial Number</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/10/08 KRC
        //
        public virtual string MFGSerialNumber
        {
            get
            {
                if (!m_strMFGSerialNumber.Cached)
                {
                    m_strMFGSerialNumber.Value = GetSTDEDLString(StdTableEnum.STDTBL1_MFG_SERIAL_NUMBER);
                }

                return m_strMFGSerialNumber.Value;
            }
        }

        /// <summary>Gets the CT Ratio for the current device</summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? CTRatio
        {
            get
            {
                float? fltCTRatio = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_CT_MULTIPLIER, null))
                {
                    if (!m_fltCTRatio.Cached)
                    {
                        m_fltCTRatio.Value = GetMFGEDLFloat(CentronTblEnum.MFGTBL0_CT_MULTIPLIER);
                    }

                    fltCTRatio = m_fltCTRatio.Value;
                }

                return fltCTRatio;
            }
        }

        /// <summary>Gets the VT Ratio for the current device</summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? VTRatio
        {
            get
            {
                float? fltVTRatio = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_VT_MULTIPLIER, null))
                {
                    if (!m_fltVTRatio.Cached)
                    {
                        m_fltVTRatio.Value = GetMFGEDLFloat(CentronTblEnum.MFGTBL0_VT_MULTIPLIER);
                    }

                    fltVTRatio = m_fltVTRatio.Value;
                }

                return fltVTRatio;
            }
        }

        /// <summary>Gets the Register Multiplier for the current device</summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 jrf 2.70.15 TQ6710 Made nullable.
        //
        public virtual float? RegisterMultiplier
        {
            get
            {
                float? fltRegMult = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_REGISTER_MULTIPLIER, null))
                {
                    if (!m_fltRegisterMultiplier.Cached)
                    {
                        m_fltRegisterMultiplier.Value = GetMFGEDLFloat(CentronTblEnum.MFGTBL0_REGISTER_MULTIPLIER);
                    }

                    fltRegMult = m_fltRegisterMultiplier.Value;
                }

                return fltRegMult;
            }
        }

        /// <summary>
        /// Gets the Register Fullscale value from the EDL file in KW.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/10/08 RCG	2.00.06	122632 Created
        //  11/05/10 jrf 2.45.11        Added set.
        //
        public virtual double? RegisterFullscale
        {
            get
            {
                double? dRegFullscale = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE, null, out objValue);

                    dRegFullscale = (double)objValue / 1000.0;
                }

                return dRegFullscale;
            }
            set
            {
                object objValue = null;

                if (null != value)
                {
                    objValue = value * 1000.0;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE, null, objValue);
                }
            }
        }

        /// <summary>
        /// Gets the Outage Length before Cold Load Pickup in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public virtual int OutageLength
        {
            get
            {
                if (!m_iCLPUOutageTime.Cached)
                {
                    m_iCLPUOutageTime.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_OUTAGE_LENGTH_BEFORE_CLPU);
                }

                return m_iCLPUOutageTime.Value;
            }
            set
            {
                m_iCLPUOutageTime.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_OUTAGE_LENGTH_BEFORE_CLPU);
            }
        }

        /// <summary>Gets the Display mode timeout in minutes</summary>
        public virtual int DisplayModeTimeout
        {
            get
            {
                if (!m_iModeTimeout.Cached)
                {
                    m_iModeTimeout.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_MODE_TIMEOUT);
                }

                return m_iModeTimeout.Value;
            }

        }

        /// <summary>
        /// Gets the Time Of Use Schedule's TOU configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  11/18/13 jrf 3.50.06 TQ 9479   Created
        // 
        public virtual TOUConfig TOUConfiguration
        {
            get
            {
                if (null == m_TOUConfig)
                {
                    ReadTOUConfiguration();
                }

                return m_TOUConfig;
            }
        }

        /// <summary>
        /// Gets the Time Of Use Schedule's Calendar configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  11/18/13 jrf 3.50.06 TQ 9479   Created
        // 
        public virtual CalendarConfig CalendarConfiguration
        {
            get
            {
                if (null == m_CalendarConfig)
                {
                    ReadTOUConfiguration();
                }

                return m_CalendarConfig;
            }
        }

        /// <summary>
        /// Gets the time format specified in table 0.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version  ID Number Description
        //  -------- --- -------- -- ------ ---------------------------------------
        //  01/12/15 jrf 4.50.223 WR 645125 Created
        public virtual int TimeFormat
        {
            get
            {
                //initialize to default 
                int TimeFormat = (int)PSEMBinaryReader.TM_FORMAT.UINT32_TIME;

                try
                {
                    if (false == m_TimeFormat.Cached)
                    {
                        object objectValue = null;
                        byte byteValue = 0;

                        m_CenTables.GetValue(StdTableEnum.STDTBL0_TM_FORMAT, null, out objectValue);

                        if (objectValue is byte)
                        {
                            byteValue = (byte)objectValue;
                            m_TimeFormat.Value = (int)byteValue;
                        }
                    }

                    if (true == m_TimeFormat.Cached)
                    {
                        TimeFormat = m_TimeFormat.Value;
                    }
                }
                catch { /*Keep going with default time format if we can't read it.*/}

                return TimeFormat;
            }
        }

        /// <summary>
        /// Gets the string interpretation of the demand calculation method. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version  ID Number Description
        //  -------- --- -------- -- ------ ---------------------------------------
        //  10/25/16 jrf 4.70.28  WR 203427 Created
        public string DemandCalculationMethodString
        {
            get
            {
                string CalculationString = "";

                int iDemandControl = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DEMAND_CONTROL);

                switch (iDemandControl)
                {
                    case (int)DemandCalculationMethodSelection.Block:
                        {
                            CalculationString = Resources.BlockDemandCalculation;
                            break;
                        }
                    case (int)DemandCalculationMethodSelection.Sliding:
                        {
                            CalculationString = Resources.SlidingDemandCalculation;
                            break;
                        }
                    default:
                        break;
                }

                return CalculationString;
            }
        }

        #endregion ANSI Device Configuration

        #region CENTRON AMI Device Configuration

        /// <summary>
        /// Determines if User Intervention is required after a load limiting disconnect
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/20/08 KRC 10.00.00        
        // 		
        public virtual string LoadLimitingConnectWithoutUserIntervetion
        {
            get
            {
                if (!m_strLoadControlReconnect.Cached)
                {
                    int iDemandControl = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DEMAND_CONTROL);
                    m_strLoadControlReconnect.Value = CENTRON_AMI.TranslateLoadLimitingConnectWithoutUserIntervetion(iDemandControl);
                }
                return m_strLoadControlReconnect.Value;
            }
        }

        /// <summary>
        /// Determines if Load Control is enabled and what the Threshold is if it is enabled
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/20/07 KRC 8.10.15        Adding Load Limiting summary support
        // 	06/23/08 RCG 1.50.41 116607 Changing to use correct enumeration for the threshold
        public virtual string LoadControlDisconnectThreshold
        {
            get
            {
                if (!m_strLoadControlThreshold.Cached)
                {
                    // This item is bit 7 of the Demand Type.
                    float fLoadControlThreshold = GetMFGEDLFloat(CentronTblEnum.MFGTBL0_THRESHOLD_LEVEL, 0);

                    m_strLoadControlThreshold.Value = CENTRON_AMI.TranslateLoadControlDisconnectThreshold(fLoadControlThreshold);
                }
                return m_strLoadControlThreshold.Value;
            }
        }

        /// <summary>
        /// Gets whether or not Daily Self read is configured.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/02/08 RCG	1.50.14		   Created
        // 11/05/10 jrf 2.45.10        Added set.
        //
        public virtual bool DailySelfReadEnabled
        {
            get
            {
                if (!m_bDailySelfReadEnabled.Cached)
                {
                    string strDailySelfRead = GetMFGEDLString(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME);

                    if (strDailySelfRead == "" || strDailySelfRead == "0")
                    {
                        m_bDailySelfReadEnabled.Value = false;
                    }
                    else
                    {
                        m_bDailySelfReadEnabled.Value = true;
                    }
                }

                return m_bDailySelfReadEnabled.Value;
            }
            set
            {
                object objValue;
                byte bytValue = 0;

                if (false == value)
                {
                    bytValue = DSRT_DISABLED;
                }
                else
                {
                    bytValue = DSRT_MIDNIGHT;
                }

                objValue = bytValue;
                m_bDailySelfReadEnabled.Value = value;

                //Need to update the cached time also.
                m_strDailySelfReadTime.Value = CENTRON_AMI.DetermineDailySelfRead(bytValue);

                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, objValue);
            }
        }

        /// <summary>
        /// Gets the configured daily self read time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/08 KRC 
        // 06/11/08 jrf 1.50.34               Switched to call byte.Parse() that used 
        //                                    the number styles parameter to be compatible
        //                                    with the compact framework.
        public virtual string DailySelfReadTime
        {
            get
            {
                if (!m_strDailySelfReadTime.Cached)
                {
                    byte bySelfReadTime = 0;
                    object objValue;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                        bySelfReadTime = (byte)objValue;
                    }

                    // Use a static method in the CENTRON AMI Device code to do the translation.
                    m_strDailySelfReadTime.Value = CENTRON_AMI.DetermineDailySelfRead(bySelfReadTime);
                }

                return m_strDailySelfReadTime.Value;
            }
        }

        /// <summary>
        /// Gets/sets the daily self read hour.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/21/10 jrf	2.45.10		   Created
        //
        public virtual byte DailySelfReadHour
        {
            get
            {
                if (false == m_bytDailySelfReadHour.Cached)
                {
                    byte bytSelfReadTime = 0;
                    byte bytSelfReadHour = 0;
                    object objValue;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                        bytSelfReadTime = (byte)objValue;

                    }

                    //Self read time byte contains both the hour and minute values.
                    bytSelfReadHour = (byte)(bytSelfReadTime & DSRT_HR_MASK);

                    //Midnight is stored internaly as 24 instead of 0
                    if (DSRT_MIDNIGHT == bytSelfReadHour)
                    {
                        bytSelfReadHour = 0;
                    }

                    m_bytDailySelfReadHour.Value = bytSelfReadHour;
                }

                return m_bytDailySelfReadHour.Value;
            }
            set
            {
                object objValue;
                byte bytSelfReadTime = 0;
                byte bytSelfReadMinute = 0;
                byte bytSelfReadHour = value;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                    bytSelfReadTime = (byte)objValue;
                    bytSelfReadMinute = (byte)(bytSelfReadTime & DSRT_MIN_MASK);
                }

                //Update the cached value of hour before changing it.
                m_bytDailySelfReadHour.Value = bytSelfReadHour;

                //Midnight is stored internally as 24 instead of 0
                if (0 == bytSelfReadHour)
                {
                    bytSelfReadHour = DSRT_MIDNIGHT;
                }

                bytSelfReadTime = (byte)(bytSelfReadHour | bytSelfReadMinute);

                objValue = bytSelfReadTime;

                //Need to update the cached time values.
                m_strDailySelfReadTime.Value = CENTRON_AMI.DetermineDailySelfRead(bytSelfReadTime);

                m_bytDailySelfReadMinute.Value = ConvertFromInternalDailySelfReadMinute(bytSelfReadMinute);

                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, objValue);
            }
        }

        /// <summary>
        /// Gets/sets the daily self read minute.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/21/10 jrf	2.45.10		   Created
        //
        public virtual byte DailySelfReadMinute
        {
            get
            {
                if (false == m_bytDailySelfReadMinute.Cached)
                {
                    byte bytSelfReadTime = 0;
                    byte bytSelfReadMinute = 0;
                    object objValue;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                        bytSelfReadTime = (byte)objValue;

                    }

                    //Self read time byte contains both the hour and minute values.
                    bytSelfReadMinute = (byte)(bytSelfReadTime & DSRT_MIN_MASK);
                    m_bytDailySelfReadMinute.Value = ConvertFromInternalDailySelfReadMinute(bytSelfReadMinute);
                }

                return m_bytDailySelfReadMinute.Value;
            }
            set
            {
                object objValue;
                byte bytSelfReadTime = 0;
                byte bytSelfReadMinute = ConvertToInternalDailySelfReadMinute(value);
                byte bytSelfReadHour = 0;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                    bytSelfReadTime = (byte)objValue;
                    bytSelfReadHour = (byte)(bytSelfReadTime & DSRT_HR_MASK);
                }

                bytSelfReadTime = (byte)(bytSelfReadHour | bytSelfReadMinute);

                objValue = bytSelfReadTime;

                //Midnight is stored as 24 instead of 0.
                //Need to change before caching below.
                if (DSRT_MIDNIGHT == bytSelfReadHour)
                {
                    bytSelfReadHour = 0;
                }

                //Need to update the cached time values.
                m_strDailySelfReadTime.Value = CENTRON_AMI.DetermineDailySelfRead(bytSelfReadTime);
                m_bytDailySelfReadHour.Value = bytSelfReadHour;
                m_bytDailySelfReadMinute.Value = value;

                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, objValue);
            }
        }

        /// <summary>
        /// Gets a TimeSpan object that represents the DST Change time in seconds since midnight
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/25/16 jrf 4.70.28 230427 Created
        //
        public virtual TimeSpan DSTChangeTime
        {
            get
            {
                if (false == m_DSTChangeTime.Cached)
                {
                    uint DSTChangeTime = GetSTDEDLUInt(StdTableEnum.STDTBL53_DST_TIME_EFF);

                    m_DSTChangeTime.Value =
                        TimeSpan.FromSeconds((double)DSTChangeTime);

                }

                return m_DSTChangeTime.Value;
            }
        }

        /// <summary>
        /// Gets a TimeSpan object that represents the DST offset in minutes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/25/16 jrf 4.70.28 230427 Created
        //
        public virtual TimeSpan DSTOffset
        {
            get
            {
                if (false == m_DSTOffset.Cached)
                {
                    byte DSTOffset = GetSTDEDLByte(StdTableEnum.STDTBL53_DST_TIME_AMT);

                    m_DSTOffset.Value =
                        TimeSpan.FromMinutes((double)DSTOffset);

                }

                return m_DSTOffset.Value;
            }
        }

        /// <summary>
        /// Gets a TimeSpan object that represents the Time Zone Offset
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/06 RCG 7.40.00 N/A    Created
        //
        public virtual TimeSpan TimeZoneOffset
        {
            get
            {
                if (!m_tsTimeZoneOffset.Cached)
                {
                    int iFlag = GetSTDEDLInt(StdTableEnum.STDTBL51_TIME_FUNC_FLAG2);
                    if (!StdTable51.TranslateIsTimeZoneAvailable((byte)iFlag))
                    {
                        throw new NotSupportedException("This device does not support time zone offset");
                    }
                    else
                    {
                        short sTimeZoneOffset = GetSTDEDLShort(StdTableEnum.STDTBL53_TIME_ZONE_OFFSET);

                        m_tsTimeZoneOffset.Value =
                            TimeSpan.FromMinutes((double)sTimeZoneOffset);
                    }
                }

                return m_tsTimeZoneOffset.Value;
            }

            set
            {
                short sTimeZoneOffset = (short)(value.TotalMinutes);
                int iFlag = GetSTDEDLInt(StdTableEnum.STDTBL51_TIME_FUNC_FLAG2);

                if (true == StdTable51.TranslateIsTimeZoneAvailable((byte)iFlag))
                {
                    SetSTDEDLShort(sTimeZoneOffset, StdTableEnum.STDTBL53_TIME_ZONE_OFFSET);
                }

                m_tsTimeZoneOffset.Flush();
            }
        }

        /// <summary>
        /// Gets the list of configured energy items.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG	2.20.19		   Created

        public virtual List<LID> EnergyConfigLIDs
        {
            get
            {

                CentronAMILID EnergyLID;
                object Value;
                int[] anIndex1 = { 0 };
                List<LID> EnergyConfig = new List<LID>();

                try
                {

                    for (int i = 0; i < DetermineEnergyConfigCount(); i++)
                    {
                        anIndex1[0] = i;
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL0_ENERGY_LID,
                            anIndex1, out Value);
                        EnergyLID = new CentronAMILID(SEC_ENERGY_LID_BASE + (byte)Value);
                        EnergyConfig.Add(EnergyLID);
                    }
                }
                catch (Exception)
                {
                    EnergyConfig = null;
                }

                return EnergyConfig;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };

                //foreach (LID lid in value)
                for (int i = 0; i < DetermineEnergyConfigCount(); i++)
                {
                    anIndex1[0] = i;
                    Value = value[i].lidValue - SEC_ENERGY_LID_BASE;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_ENERGY_LID, anIndex1, Value);
                }
            }

        }

        /// <summary>
        /// Gets the list of configured Demands LIDs
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG	2.20.19		   Created

        public virtual List<LID> DemandConfigLIDs
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                CentronAMILID DemandLID;
                List<LID> DemandConfigLID = new List<LID>();

                for (i = 0; i < DetermineDemandConfigCount(); i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION,
                        anIndex1, out Value);
                    DemandLID = new CentronAMILID((uint)Value);
                    DemandConfigLID.Add(DemandLID);
                }

                return DemandConfigLID;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };

                //foreach (LID lid in value)
                for (int i = 0; i < DetermineDemandConfigCount(); i++)
                {
                    anIndex1[0] = i;
                    Value = value[i].lidValue;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, anIndex1, Value);
                }
            }
        }

        /// <summary>
        /// Gets whether or not Power Monitoring is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.50.06 173353 Created
        //
        public bool? IsPowerMonitoringEnabled
        {
            get
            {
                bool? bEnabled = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl321EnablePowerMonitor, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl321EnablePowerMonitor, null, out objValue);
                    bEnabled = Convert.ToBoolean(objValue, CultureInfo.InvariantCulture);
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets power monitoring cold load time in seconds 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/08/11 jrf 2.50.08 173353 Created
        //
        public byte? PowerMonitoringColdLoadTime
        {
            get
            {
                byte? bytColdLoadTime = null;

                try
                {
                    if (null != Table2369)
                    {
                        bytColdLoadTime = Table2369.ColdLoadTime;
                    }
                }
                catch { }

                return bytColdLoadTime;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Watts Delivered quantity from the power monitoring tables tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  06/08/11 jrf 2.50.08 173353 Created
        //
        public InstantaneousQuantity InstantaneousWattsDelivered
        {
            get
            {
                InstantaneousQuantity InsWd = null;

                if (null != Table2370)
                {
                    InsWd = Table2370.InstantaneousWattsDelivered;
                }
                return InsWd;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Watts Received quantity from the power monitoring tables tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  06/08/11 jrf 2.50.08 173353 Created
        //
        public InstantaneousQuantity InstantaneousWattsReceived
        {
            get
            {
                InstantaneousQuantity InsWr = null;

                if (null != Table2370)
                {
                    InsWr = Table2370.InstantaneousWattsReceived;
                }
                return InsWr;
            }
        }

        /// <summary>
        /// Gets the collection of extended instantaneous values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/16/12 AF  2.53.50 TREQ5775 Created
        //
        public ReadOnlyCollection<ExtendedCurrentEntryRecord> CurrentExtInstantaneousData
        {
            get
            {
                ReadOnlyCollection<ExtendedCurrentEntryRecord> CurrExtInstDataCollection = null;
                if (null != Table2422)
                {
                    CurrExtInstDataCollection = Table2422.CurrentExtInstantaneousData;
                }

                return CurrExtInstDataCollection;
            }
        }

        /// <summary>
        /// Gets the Firmware Download Events from the firmware download event log.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/11/11 jrf 2.52.02 TREQ2706 Created
        // 11/24/15 PGH 4.50.218 REQ574469 Filter seal/unseal events
        //
        public ReadOnlyCollection<FWDownloadLogEvent> FWDLEvents
        {
            get
            {
                ReadOnlyCollection<FWDownloadLogEvent> FWDLEvents = null;

                if (null != Table2382)
                {
                    FWDLEvents = Table2382.Events.Where(o =>
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.AutoSealMeter &&
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.SealMeter &&
                        o.EventID != (ushort)FWDownloadLogEvent.FWDownloadLogEventID.UnsealMeter).ToList().AsReadOnly();
                }

                return FWDLEvents;
            }
        }

        /// <summary>
        /// Gets the Seal/Unseal Events from the firmware download event log.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/16/15 PGH 4.50.222 REQ574469 Created
        //
        public ReadOnlyCollection<FWDownloadLogEvent> SealUnsealEvents
        {
            get
            {
                ReadOnlyCollection<FWDownloadLogEvent> SealUnsealEvents = null;

                if (null != Table2382)
                {
                    SealUnsealEvents = Table2382.Events.Where(o =>
                        o.EventID == (ushort)FWDownloadLogEvent.FWDownloadLogEventID.AutoSealMeter ||
                        o.EventID == (ushort)FWDownloadLogEvent.FWDownloadLogEventID.SealMeter ||
                        o.EventID == (ushort)FWDownloadLogEvent.FWDownloadLogEventID.UnsealMeter).ToList().AsReadOnly();
                }

                return SealUnsealEvents;
            }
        }

        /// <summary>
        /// Retrieves the register boot loader firmware's CRC.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/11/11 jrf 2.52.02 TREQ2712 Created
        //
        public UInt32? RegisterBootLoaderCRC
        {
            get
            {
                UInt32? uiRegisterBootLoaderCRC = null;

                if (null != Table2383)
                {
                    uiRegisterBootLoaderCRC = Table2383.RegisterBootLoaderCRC;
                }

                return uiRegisterBootLoaderCRC;
            }
        }

        /// <summary>
        /// Retrieves the register application firmware's CRC.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/11/11 jrf 2.52.02 TREQ2712 Created
        //
        public UInt32? RegisterApplicationCRC
        {
            get
            {
                UInt32? uiRegisterApplicationCRC = null;

                if (null != Table2383)
                {
                    uiRegisterApplicationCRC = Table2383.ApplicationCRC;
                }

                return uiRegisterApplicationCRC;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is a Canadian meter.
        /// </summary>
        //  Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/11/11 jrf 2.52.02 TREQ2712 Created
        //
        public bool? IsCanadian
        {
            get
            {
                bool? blnCanadian = null;

                if (null != Table2220)
                {
                    blnCanadian = Table2220.IsCanadian;
                }

                return blnCanadian;
            }
        }

        /// <summary>
        /// Gets the list of Extended Self Read Instantaneous quantity LIDs configured into the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 02/01/12 jrf 2.53.37 TC7067 Created 
        //
        public List<LID> ExtendedSelfReadInstantaneousConfigLIDs
        {
            get
            {
                object objValue;
                List<LID> ConfigLIDs = new List<LID>();
                int[] aiExtSRElementLimits = m_CenTables.GetElementLimits(CentronTblEnum.MfgTbl217SelfReadTwoConfigurationItem);

                if (null != aiExtSRElementLimits && 0 < aiExtSRElementLimits.Length)
                {
                    for (int CurrentIndex = 0; CurrentIndex < aiExtSRElementLimits[0]; CurrentIndex++)
                    {
                        int[] aiIndexer = new int[] { CurrentIndex };

                        if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier, aiIndexer))
                        {
                            m_CenTables.GetValue(CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier, aiIndexer, out objValue);

                            if (objValue != null && 0 != (uint)objValue)
                            {
                                uint LIDValue = (uint)objValue;

                                ConfigLIDs.Add(new CentronAMILID(LIDValue));
                            }
                        }
                    }
                }

                return ConfigLIDs;
            }
        }

        /// <summary>
        /// Gets the list of configured Extended self read instantaneous quantites as a string;
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 02/01/12 jrf 2.53.37 TC7067 Created 
        //
        public List<string> ExtendedSelfReadInstantaneousConfigList
        {
            get
            {
                List<string> ConfigList = new List<string>();

                foreach (LID CurrentLID in ExtendedSelfReadInstantaneousConfigLIDs)
                {
                    ConfigList.Add(CurrentLID.lidDescription);
                }

                return ConfigList;
            }
        }

        /// <summary>
        /// Gets the minimum number of seconds difference the comm module time and 
        /// register time can be different for an automatic network time adjustment
        /// to occur.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/06/13 jrf 2.80.06 TQ6663 Created.
        //  05/22/13 jrf 2.80.32 TQ6663 Using GetValue instead of a table read since 
        //                              entire table is not present.
        //
        public byte? MinimumTimeAdjustmentSeconds
        {
            get
            {
                byte? byMinSecs = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL143_MIN_DELTA_SECONDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL143_MIN_DELTA_SECONDS, null, out objValue);
                    byMinSecs = Convert.ToByte(objValue, CultureInfo.InvariantCulture);
                }

                return byMinSecs;
            }

            
        }

        /// <summary>
        /// Gets the maximum number of seconds difference the comm module time and 
        /// register time can be different for an automatic network time adjustment
        /// to occur.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/06/13 jrf 2.80.06 TQ6663 Created.
        //  05/22/13 jrf 2.80.32 TQ6663 Using GetValue instead of a table read since 
        //                              entire table is not present.
        //
        public byte? MaximumTimeAdjustmentSeconds
        {
            get
            {
                byte? byMaxSecs = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL143_MAX_DELTA_SECONDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL143_MAX_DELTA_SECONDS, null, out objValue);
                    byMaxSecs = Convert.ToByte(objValue, CultureInfo.InvariantCulture);
                }

                return byMaxSecs;
            }
        }

        /// <summary>
        /// Gets whether the current per phase threshold exceeded is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 04/26/13 AF  2.80.23 TR7590 Added for display of Current Per Phase Threshold Exceeded items.
        //
        public bool? CTEEnabled
        {
            get
            {
                bool? returnValue = null;

                if (Table2265CTEConfig != null)
                {
                    returnValue = Table2265CTEConfig.CTEEnable;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets whether or not the current per phase threshold has been configured
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 04/26/13 AF  2.80.23 TR7590 Added for display of Current Per Phase Threshold Exceeded items.
        //
        public bool? CTEConfigured
        {
            get
            {
                bool? returnValue = null;

                if (Table2265CTEConfig != null)
                {
                    returnValue = Table2265CTEConfig.CTEConfigured;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the current per phase threshold exceeded configured threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 04/26/13 AF  2.80.23 TR7590 Added display of Current Per Phase Threshold Exceeded items.
        //
        public byte? CTEThreshold
        {
            get
            {
                byte? returnValue = null;

                if (Table2265CTEConfig != null)
                {
                    returnValue = Table2265CTEConfig.CTEThreshold;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the current per phase threshold exceeded hysteresis item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 04/26/13 AF  2.80.23 TR7590 Added display of Current Per Phase Threshold Exceeded items.
        //
        public byte? CTEHysteresis
        {
            get
            {
                byte? returnValue = null;

                if (Table2265CTEConfig != null)
                {
                    returnValue = Table2265CTEConfig.CTEHysteresis;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the current per phase threshold exceeded debounce item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 04/26/13 AF  2.80.23 TR7590 Added display of Current Per Phase Threshold Exceeded items.
        //
        public byte? CTEDebounce
        {
            get
            {
                byte? returnValue = null;

                if (Table2265CTEConfig != null)
                {
                    returnValue = Table2265CTEConfig.CTEDebounce;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the current per phase threshold minimum active duration item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 04/26/13 AF  2.80.23 TR7590 Added display of Current Per Phase Threshold Exceeded items.
        //
        public UInt16? CTEMinActiveDuration
        {
            get
            {
                UInt16? returnValue = null;

                if (Table2265CTEConfig != null)
                {
                    returnValue = Table2265CTEConfig.CTEMinActiveDuration;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Phase Current from MFG table 2377
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/14/15 PGH 4.50.221 627380 Created
        //
        public IPCDataRcd InstantaneousPhaseCurrent
        {
            get
            {
                IPCDataRcd IPCDataRecord = null;

                if (null != Table2377)
                {
                    IPCDataRecord = Table2377.IPCDataRecord;
                }

                return IPCDataRecord;
            }
        }

        /// <summary>
        /// Gets the Temperature Configuration from MFG table 2425
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/16 PGH 4.50.224 RTT556309 Created
        //
        public TemperatureConfigRcd TemperatureConfiguration
        {
            get
            {
                TemperatureConfigRcd TemperatureConfigurationRecord = null;

                if (null != Table2425)
                {
                    TemperatureConfigurationRecord = Table2425.TemperatureConfigRcd;
                }

                return TemperatureConfigurationRecord;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Temperature Data from MFG table 2426
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/16 PGH 4.50.224 RTT556309 Created
        //
        public TemperatureDataRcd InstantaneousTemperatureData
        {
            get
            {
                TemperatureDataRcd TemperatureDataRecord = null;

                if (null != Table2426)
                {
                    TemperatureDataRecord = Table2426.TemperatureDataRcd;
                }

                return TemperatureDataRecord;
            }
        }

        /// <summary>
        /// Gets the Temperature Log from MFG table 2427
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/16 PGH 4.50.224 RTT556309 Created
        //
        public ReadOnlyCollection<TemperatureLogEntry> TemperatureLog
        {
            get
            {
                ReadOnlyCollection<TemperatureLogEntry> TemperatureLogEntries = null;

                if (null != Table2427)
                {
                    TemperatureLogEntries = Table2427.TemperatureLog.TemperatureLogEntries.ToList().AsReadOnly();
                }

                return TemperatureLogEntries;
            }
        }

        /// <summary>
        /// Retrieves the demand reset lockout time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/02/13 AF  2.80.25 TR7885  Created
        //  05/02/13 AF  2.80.25 TR7885  Read the value out of 2048 rather than std table 13
        //
        public UInt16? DemandResetLockout
        {
            get
            {
                UInt16? DRLockoutMin = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DR_LOCKOUT_TIME, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DR_LOCKOUT_TIME, null, out objValue);
                    DRLockoutMin = (UInt16)objValue;
                }

                return DRLockoutMin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 01/27/14 jrf	3.00.29 WR 428640 Created.
        //
        public bool SupportsInstrumentationProfile
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table0.IsTableUsed(2409) && true == Table0.IsTableUsed(2410)
                    && true == Table0.IsTableUsed(2411) && true == Table0.IsTableUsed(2413)
                    && true == Table0.IsTableUsed(2417));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets the CachedValue variable indicating whether or not VA arithmetic was configured 
        /// in the instrumentation profile UI.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/16 jrf 4.70.32 230427 Created
        public virtual bool? UIInstrumentationProfileVAArithmeticConfigured
        {
            get
            {
                if (!m_UIInstrumentationProfileVAArithmeticConfigured.Cached)
                {
                    if (m_CenTables != null && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl2044UserInterfaceEnableInstrumentationProfileVA_Arithmetic, null))
                    {
                        object objValue = null;
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl2044UserInterfaceEnableInstrumentationProfileVA_Arithmetic, null, out objValue);
                        m_UIInstrumentationProfileVAArithmeticConfigured.Value = (bool)objValue;
                    }
                    else
                    {
                        m_UIInstrumentationProfileVAArithmeticConfigured.Value = null;
                    }

                }

                return m_UIInstrumentationProfileVAArithmeticConfigured.Value;
            }
        }

        /// <summary>
        /// Gets the CachedValue variable indicating whether or not VA vectorial was configured 
        /// in the instrumentation profile UI.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/16 jrf 4.70.32 230427 Created
        public virtual bool? UIInstrumentationProfileVAVectorialConfigured
        {
            get
            {
                if (!m_UIInstrumentationProfileVAVectorialConfigured.Cached)
                {
                    if (m_CenTables != null && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl2044UserInterfaceEnableInstrumentationProfileVA_Vectorial, null))
                    {
                        object objValue = null;
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl2044UserInterfaceEnableInstrumentationProfileVA_Vectorial, null, out objValue);
                        m_UIInstrumentationProfileVAVectorialConfigured.Value = (bool)objValue;
                    }
                    else
                    {
                        m_UIInstrumentationProfileVAVectorialConfigured.Value = null;
                    }

                }

                return m_UIInstrumentationProfileVAVectorialConfigured.Value;
            }
        }

        /// <summary>
        /// Whether or not we know the meter supports instrumentation profile VAR quantity.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 10/26/16 jrf	4.70.28 WR 230427 Created.
        public bool KnownToSupportInstrumentationProfileVARQuantity
        {
            get
            {
                bool Supported = false;

                if (CENTRON_AMI.ITR4_DEVICE_CLASS == DeviceClass
                    || CENTRON_AMI.ITRF_DEVICE_CLASS == DeviceClass
                    || CENTRON_AMI.ITRK_DEVICE_CLASS == DeviceClass)
                {
                    Supported = true;
                }
                else if ((CENTRON_AMI.ITRD_DEVICE_CLASS == DeviceClass
                    || CENTRON_AMI.ITRJ_DEVICE_CLASS == DeviceClass)
                    && SecondaryBaseEnergy == BaseEnergies.VarhVectorial)
                {
                    Supported = true;
                }

                return Supported;
            }
        }

        /// <summary>
        /// Whether or not we know the meter supports instrumentation profile VA Arithmetic quantity.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 10/26/16 jrf	4.70.28 WR 230427 Created.
        // 11/07/16 jrf 4.70.32 WR 230427 Adding check to see if IP UI settings are in EDL.
        public bool KnownToSupportInstrumentationProfileVAArithmeticQuantity
        {
            get
            {
                bool Supported = false;

                if (CENTRON_AMI.ITR1_DEVICE_CLASS == DeviceClass)
                {
                    Supported = true;
                }
                else if ((CENTRON_AMI.ITRD_DEVICE_CLASS == DeviceClass
                    || CENTRON_AMI.ITRJ_DEVICE_CLASS == DeviceClass)
                    && SecondaryBaseEnergy == BaseEnergies.VAhArithmetic)
                {
                    Supported = true;
                }
                else if (VACalculation != null
                    && ARITHMETIC == VACalculation)
                {
                    Supported = true;
                }
                else if (UIInstrumentationProfileVAArithmeticConfigured != null
                    && true == UIInstrumentationProfileVAArithmeticConfigured)
                {
                    Supported = true;
                }
                else
                {
                    foreach (LID EnergyLID in EnergyConfigLIDs)
                    {
                        switch (EnergyLID.lidQuantity)
                        {

                            case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH:
                            case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH:
                                {
                                    Supported = true;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }

                        if (true == Supported)
                        {
                            break;
                        }
                    }
                }
                    return Supported;
            }
        }

        /// <summary>
        /// Whether or not we know the meter supports  instrumentation profile VA Vectorial quantity.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 10/26/16 jrf	4.70.28 WR 230427 Created.
        // 11/03/16 jrf 4.70.29 WR 230427 Correcting ITRD/ITRJ support check.
        // 11/07/16 jrf 4.70.32 WR 230427 Adding check to see if IP UI settings are in EDL.
        public bool KnownToSupportInstrumentationProfileVAVectorialQuantity
        {
            get
            {
                bool Supported = false;

                if ((CENTRON_AMI.ITRD_DEVICE_CLASS == DeviceClass
                    || CENTRON_AMI.ITRJ_DEVICE_CLASS == DeviceClass)
                    && SecondaryBaseEnergy == BaseEnergies.VAhVectorial)
                {
                    Supported = true;
                }
                else if (VACalculation != null
                    && VECTORIAL == VACalculation)
                {
                    Supported = true;
                }
                else if (UIInstrumentationProfileVAVectorialConfigured != null
                    && true == UIInstrumentationProfileVAVectorialConfigured)
                {
                    Supported = true;
                }
                else
                {
                    foreach (LID EnergyLID in EnergyConfigLIDs)
                    {
                        switch (EnergyLID.lidQuantity)
                        {

                            case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT:
                            case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT:
                                {
                                    Supported = true;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }

                        if (true == Supported)
                        {
                            break;
                        }
                    }
                }
                return Supported;
            }
        }

        /// <summary>
        /// Gets the VA/VAR secondary base energy quantity for single phase EDL files
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/20/14 jrf 4.00.77 539709 Created
        public virtual BaseEnergies SecondaryBaseEnergy
        {
            get
            {
                BaseEnergies SecondaryQuantity = BaseEnergies.Unknown;

                if (DeviceClass == CENTRON_AMI.ITR1_DEVICE_CLASS)
                {
                    //ITR1 devices should only be set to VA arithmetic
                    SecondaryQuantity = BaseEnergies.VAhArithmetic;
                }                
                else if (VAVARSelection != null)
                {
                    //Let's see if EDL will tell us.
                    if (VA == VAVARSelection)
                    {
                        if (VACalculation != null)
                        {
                            if (ARITHMETIC == VACalculation)
                            {
                                SecondaryQuantity = BaseEnergies.VAhArithmetic;
                            }
                            else if (VECTORIAL == VACalculation)
                            {
                                SecondaryQuantity = BaseEnergies.VAhVectorial;
                            }
                        }
                    }
                    else if (VAR == VAVARSelection)
                    {
                        SecondaryQuantity = BaseEnergies.VarhVectorial;
                    }
                }

                if (SecondaryQuantity == BaseEnergies.Unknown)
                {
                    foreach (LID EnergyLID in EnergyConfigLIDs)
                    {
                        switch (EnergyLID.lidQuantity)
                        {

                            case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH:
                            case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH:
                                {
                                    SecondaryQuantity = BaseEnergies.VAhArithmetic;
                                    break;
                                }
                            case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT:
                            case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT:
                                {
                                    SecondaryQuantity = BaseEnergies.VAhVectorial;
                                    break;
                                }
                            case DefinedLIDs.WhichOneEnergyDemand.VARH_DEL:
                            case DefinedLIDs.WhichOneEnergyDemand.VARH_REC:
                            case DefinedLIDs.WhichOneEnergyDemand.VARH_NET:
                                {
                                    SecondaryQuantity = BaseEnergies.VarhVectorial;
                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                    }
                } 

                return SecondaryQuantity;
            }
        }

        /// <summary>
        /// Gets the VA/VAR secondary energy quantity selection
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/20/14 jrf 4.00.77 539709 Created
        //  11/12/15 jrf 4.22.04 630992 Renamed property.
        public virtual SecondaryQuantitySelection SelectedSecondaryEnergyQuantity
        {
            get
            {
                SecondaryQuantitySelection SecondaryQuantity = SecondaryQuantitySelection.None;

                if (true == SecondaryEnergyQuantitySelectionSupported)
                {
                    SecondaryQuantity = SecondaryQuantitySelection.Unknown;  //Default to unknown, we'll set it when we know.

                    if (VA == VAVARSelection)
                    {
                        if (m_CenTables != null && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl2044UserInterfaceVoltAmperesSelection, null))
                        {
                            if (ARITHMETIC == VACalculation)
                            {
                                SecondaryQuantity = SecondaryQuantitySelection.VAArithmetic;
                            }
                            else if (VECTORIAL == VACalculation)
                            {
                                SecondaryQuantity = SecondaryQuantitySelection.VAVectorial;
                            }
                        }
                    }
                    else if (VAR == VAVARSelection)
                    {
                        SecondaryQuantity = SecondaryQuantitySelection.Var;
                    }

                    if (SecondaryQuantitySelection.Unknown == SecondaryQuantity)
                    {
                        foreach (LID EnergyLID in EnergyConfigLIDs)
                        {
                            switch (EnergyLID.lidQuantity)
                            {

                                case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH:
                                case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH:
                                    {
                                        SecondaryQuantity = SecondaryQuantitySelection.VAArithmetic;
                                        break;
                                    }
                                case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT:
                                case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT:
                                    {
                                        SecondaryQuantity = SecondaryQuantitySelection.VAVectorial;
                                        break;
                                    }
                                case DefinedLIDs.WhichOneEnergyDemand.VARH_DEL:
                                case DefinedLIDs.WhichOneEnergyDemand.VARH_REC:
                                case DefinedLIDs.WhichOneEnergyDemand.VARH_NET:
                                    {
                                        SecondaryQuantity = SecondaryQuantitySelection.Var;
                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                }

                return SecondaryQuantity;
            }
        }

        /// <summary>
        /// Gets whether the meter supports selecting the secondary energy quantity.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/20/14 jrf 4.00.77 539709 Created
        //  11/12/15 jrf 4.22.04 630992 Renamed property.
        public virtual bool SecondaryEnergyQuantitySelectionSupported
        {
            get
            {
                bool Supported = false;

                if (CENTRON_AMI.ITRD_DEVICE_CLASS == DeviceClass 
                    || CENTRON_AMI.ITRJ_DEVICE_CLASS == DeviceClass)
                {
                    Supported = true;
                }

                return Supported;
            }
        }

        /// <summary>
        /// Gets the VA or VAR secondary quantity selection.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/20/14 jrf 4.00.77 539709 Created
        public virtual short? VAVARSelection
        {
            get
            {
                if (!m_VAVARSelection.Cached)
                {
                    if (m_CenTables != null && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl2044VAVARSelection, null))
                    {
                        object objValue = null;
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl2044VAVARSelection, null, out objValue);
                        m_VAVARSelection.Value = (short)objValue;
                    }
                    else
                    {
                        m_VAVARSelection.Value = null;
                    }

                }

                return m_VAVARSelection.Value;
            }
        }

        /// <summary>
        /// Gets the method that is used to calculate VA.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/20/14 jrf 4.00.77 539709 Created
        public virtual byte? VACalculation
        {
            get
            {
                if (!m_VACalculation.Cached)
                {
                    if (m_CenTables != null && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl2044UserInterfaceVoltAmperesSelection, null))
                    {
                        object objValue = null;
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl2044UserInterfaceVoltAmperesSelection, null, out objValue);
                        m_VACalculation.Value = (byte)objValue;
                    }
                    else
                    {
                        m_VACalculation.Value = null;
                    }

                }

                return m_VACalculation.Value;
            }
        }        

        /// <summary>
        /// Gets the translated string indicating the method that is used to calculate VA.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/25/16 jrf 4.70.28 230427 Created
        public virtual string VACalculationString
        {
            get
            {
                string VACalcString = "";

                if (VACalculation != null)
                {
                    if (ARITHMETIC == VACalculation)
                    {
                        VACalcString = Resources.Arithmetic;
                    }
                    else if (VECTORIAL == VACalculation)
                    {
                        VACalcString = Resources.Vectorial;
                    }
                    else if (LAG == VACalculation)
                    {
                        VACalcString = Resources.Lag;
                    }
                }

                return VACalcString;
            }
        }

        /// <summary>
        /// Property that determines if the Var Q2 and Var Q3 quantities are configured 
        /// as regular energy or extended energy quantites.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version ID Issue# Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 09/01/16 jrf 4.70.16 WI 708332 Created
        public bool VarhQ2VarhQ3Configured
        {
            get
            {
                bool Result = false;

                foreach (LID Quantity in EnergyConfigLIDs)
                {
                    if (LIDDefinitions.ENERGY_VARH_Q2 == Quantity
                        || LIDDefinitions.ENERGY_VARH_Q3 == Quantity)
                    {
                        Result = true;
                        break;
                    }
                }

                if (false == Result)
                {
                    foreach (LID Quantity in ExtendedEnergyConfigLIDs)
                    {
                        if (LIDDefinitions.ENERGY_VARH_Q2 == Quantity
                            || LIDDefinitions.ENERGY_VARH_Q3 == Quantity)
                        {
                            Result = true;
                            break;
                        }
                    }
                }

                return Result;
            }
        }

        #endregion CENTRON AMI Device Configuration

        #region Itron Device Status

        /// <summary>
        /// Gets the date that the device was programmed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/08/08 KRC 2.10.00        Fixed reference time to be frim 2000

        public virtual DateTime DateProgrammed
        {
            get
            {
                if (!m_dtDateProgrammed.Cached)
                {
                    uint uiTemp = 0;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_CONFIG_TIME, null))
                    {
                        uiTemp = GetMFGEDLUint(CentronTblEnum.MFGTBL0_CONFIG_TIME);

                        m_dtDateProgrammed.Value = MeterConfigurationReferenceTime;

                        // Value in 2048 is the number of seconds since Jan. 1, 2000, so to get
                        //  the value returned to Jan. 1, 2000.
                        m_dtDateProgrammed.Value = m_dtDateProgrammed.Value.AddSeconds((double)uiTemp);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsAllCached(2048))
                    {
                        object objValue;
                        m_GatewayTables.GetValue(GatewayTblEnum.MFGTBL0_CONFIGURATION_TIME_DATE, null, out objValue);
                        m_dtDateProgrammed.Value = (DateTime)objValue;
                    }
#endif
                    else
                    {
                        m_dtDateProgrammed.Value = MeterConfigurationReferenceTime;
                    }

                }

                return m_dtDateProgrammed.Value;
            }
        }

        /// <summary>
        /// Property used to get the Date of Last Demand Reset from the meter
        /// </summary>
        public DateTime DateLastDemandReset
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Property used to get the Date of Last Outage from the meter
        /// </summary>
        public DateTime DateLastOutage
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Property used to get the date of the TOU expiration
        /// </summary>
        public DateTime TOUExpirationDate
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Property used to get the Number of Times Programmed from the meter
        /// </summary>
        public int NumTimeProgrammed
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Property used to get the Number of Demand Resets from the meter
        /// </summary>
        public virtual int NumDemandResets
        {
            get
            {
                if (!m_iNumDemandResets.Cached)
                {
                    // This item is bit 7 of the Demand Type.
                    m_iNumDemandResets.Value = GetSTDEDLInt(StdTableEnum.STDTBL23_NBR_DEMAND_RESETS);
                }

                return m_iNumDemandResets.Value;
            }
        }

        /// <summary>
        /// Property used to get the Number of Outages from the meter
        /// </summary>
        public int NumOutages
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// This property returns the number of minutes that the
        /// meter run on battery power.  This is a read-only value.
        /// </summary>
        public uint NumberOfMinutesOnBattery
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Indicates whether or not the meter is currently recording
        /// load profile data
        /// </summary>
        public bool LPRunning
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Property to get the line frequency from the device.
        /// </summary>
        public float LineFrequency
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        #endregion Itron Device Status

        #region ANSI Device Status

        /// <summary>
        /// Property to get the hardware version from table 01. 
        /// need this item.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/10 AF  2.43.01 160368 The hardware version needs to mask off the Prism
        //                              Lite upper nibble
        //
        public virtual float HWRevision
        {
            get
            {
                if (!m_fltHWRevision.Cached)
                {
                    string strVersion;
                    string strRevision;

                    strVersion = GetSTDEDLString(StdTableEnum.STDTBL1_HW_VERSION_NUMBER);
                    strRevision = GetSTDEDLString(StdTableEnum.STDTBL1_HW_REVISION_NUMBER);
                    m_fltHWRevision.Value = (float)float.Parse(strVersion, CultureInfo.InvariantCulture) +
                            ((float)float.Parse(strRevision, CultureInfo.InvariantCulture) / (int)1000);

                    if (VersionChecker.CompareTo(m_fltHWRevision.Value, PRISM_LITE_REVISION) >= 0)
                    {
                        // It's a Prism Lite meter so subtract to get the real version
                        m_fltHWRevision.Value -= PRISM_LITE_REVISION;
                    }
                }

                return m_fltHWRevision.Value;
            }
        }

        /// <summary>
        /// Property to determine if the meter is in DST
        /// </summary>
        public virtual bool IsMeterInDST
        {
            get
            {
                if (!m_blnMeterInDST.Cached)
                {
                    m_blnMeterInDST.Value = GetSTDEDLBool(StdTableEnum.STDTBL52_DST_FLAG);
                }

                return m_blnMeterInDST.Value;
            }
        }

        /// <summary>
        /// Property to determine if the meter is in Test Mode
        /// </summary>
        public virtual bool IsMeterInTestMode
        {
            get
            {
                if (!m_blnMeterInTestMode.Cached)
                {
                    m_blnMeterInTestMode.Value = GetSTDEDLBool(StdTableEnum.STDTBL3_TEST_MODE_FLAG);
                }

                return m_blnMeterInTestMode.Value;
            }
        }

        /// <summary>
        /// Gets the Date of the Last 
        /// </summary>
        public DateTime DateLastTestMode
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Gets the list of errors occurring in the device.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 jrf 2.10.02 125997 Created
        //
        public string[] ErrorList
        {
            get
            {
                if (null == m_astrErrorList)
                {
                    GetErrorList();
                }

                return m_astrErrorList;
            }
        }

        #endregion ANSI Device Status

        #region CENTRON AMI Device Status

        /// <summary>
        /// Property to retrieve the Number of Inversion tampers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/18/16 AF  4.50.231 WR 419822  Added implementation
        //
        public byte? NumberOfInversionTampers
        {
            get
            {
                byte? byNumInversions = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl712InversionCount, null))
                {
                    byNumInversions = GetMFGEDLByte(CentronTblEnum.MfgTbl712InversionCount);
                }

                return byNumInversions;
            }
        }

        /// <summary>
        /// Property to retrieve the Number of Removal tampers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/18/16 AF  4.50.231 WR 419822  Added implementation
        // 
        public byte? NumberOfRemovalTampers
        {
            get
            {
                byte? byNumRemovals = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl712RemovalCount, null))
                {
                    byNumRemovals = GetMFGEDLByte(CentronTblEnum.MfgTbl712RemovalCount);
                }

                return byNumRemovals;
            }
        }

        /// <summary>
        /// Property to retrieve the number of magnetic tampers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/18/16 AF  4.50.231 WR 419822  Created
        //
        public byte? NumberOfMagneticTampers
        {
            get
            {
                byte? byNumMagneticTampers = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl712MagneticTamperCount, null))
                {
                    byNumMagneticTampers = GetMFGEDLByte(CentronTblEnum.MfgTbl712MagneticTamperCount);
                }

                return byNumMagneticTampers;
            }
        }

        /// <summary>
        /// Property to retrieve the number of magnetic tampers cleared
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/18/16 AF  4.50.231 WR 419822  Created
        //
        public byte? NumberOfMagneticTampersCleared
        {
            get
            {
                byte? byNumMagneticTampersCleared = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl712MagneticTamperClearedCount, null))
                {
                    byNumMagneticTampersCleared = GetMFGEDLByte(CentronTblEnum.MfgTbl712MagneticTamperClearedCount);
                }

                return byNumMagneticTampersCleared;
            }
        }

        /// <summary>
        /// Returns a boolean indicating if a Communications Module is present in the device
        /// </summary>
        public bool CommModulePresent
        {
            get
            {
                bool bResult = false;

                // A Communcation Module is always present in Firmware prior to 1.8
                if (FWRevision < CENTRON_AMI.VERSION_1_8_HARDWARE_2_0)
                {
                    bResult = true;
                }
                else
                {
                    // If we are dealing with firmware 1.80 or higher, we must check the 
                    // Device Class of the Comm Moudle
                    if (String.IsNullOrEmpty(CommModuleDeviceClass) == false)
                    {
                        // We received a Device Class, which means a Comm Module is installed
                        bResult = true;
                    }
                }
                return bResult;
            }
        }

        /// <summary>
        /// Gets the Device class for the Comm Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/29/10 RCG 2.40.30        Created.
        // 05/25/10 RCG 2.41.04 155141 Fixing error that could cause problems with HW 1.5 meters

        public string CommModuleDeviceClass
        {
            get
            {
                string strDeviceClass = "";

                if (VersionChecker.CompareTo(HWRevision, CENTRON_AMI.HW_VERSION_1_5) <= 0)
                {
                    // A bug in the RFLAN FW for HW 1.5 and earlier meters causes the read of 2064 to fail
                    // We can safely assume that all HW 1.5 meters have an RFLAN module so just return ITR2
                    strDeviceClass = CommModuleBase.ITR2_DEVICE_CLASS;
                }

                if (Table2064 != null)
                {
                    strDeviceClass = Table2064.DeviceClass;
                }

                return strDeviceClass;
            }
        }

        /// <summary>
        /// Boolean that indicates if an Itron Communication Module is present in the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 jrf 2.20.19 137693 Created.
        // 05/25/10 RCG 2.41.04 155141 Fixing error that could cause problems with HW 1.5 meters

        public bool ItronCommModulePresent
        {
            get
            {
                bool bResult = false;

                // A Communcation Module is always present in Firmware prior to 1.8
                if (FWRevision < CENTRON_AMI.VERSION_1_8_HARDWARE_2_0)
                {
                    bResult = true;
                }
                else
                {
                    // If we are dealing with firmware 1.80 or higher, we must check the 
                    // Device Class of the Comm Moudle
                    if (Table2064 != null)
                    {
                        if (string.Compare(CommModuleDeviceClass, "ITR2", StringComparison.Ordinal) == 0
                         || string.Compare(CommModuleDeviceClass, "ITRL", StringComparison.Ordinal) == 0)
                        {
                            // We received a Device Class, which means a Comm Module is installed
                            bResult = true;
                        }
                    }
                }
                return bResult;
            }
        }

        /// <summary>
        /// Gets the Comm module type (IP or RFLAN)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public string CommModType
        {
            get
            {
                byte byCommType = 0;

                if (!m_strCommModType.Cached)
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_COMM_MOD_TYPE, null))
                    {
                        byCommType = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_COMM_MOD_TYPE);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL60_COMM_MOD_TYPE, null))
                    {
                        byCommType = (byte)GetMFGEDLInt(GatewayTblEnum.MFGTBL60_COMM_MOD_TYPE);
                    }
#endif

                    m_strCommModType.Value = OpenWayMfgTable2108.TranslateCommModuleType(byCommType);
                }

                return m_strCommModType.Value;
            }
        }

        /// <summary>
        /// Gets the Comm module version.revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public virtual string CommModVer
        {
            get
            {
                byte byVersion = 0;
                byte byRevison = 0;

                if (!m_strCommModVer.Cached)
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_COMM_MOD_VER, null))
                    {
                        byVersion = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_COMM_MOD_VER);
                        byRevison = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_COMM_MOD_REV);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsCached((long)GatewayTblEnum.MFGTBL60_COMM_MOD_VER, null))
                    {
                        byVersion = (byte)GetMFGEDLInt(GatewayTblEnum.MFGTBL60_COMM_MOD_VER);
                        byRevison = (byte)GetMFGEDLInt(GatewayTblEnum.MFGTBL60_COMM_MOD_REV);
                    }
#endif

                    m_strCommModVer.Value = byVersion.ToString(CultureInfo.CurrentCulture) +
                        "." + byRevison.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strCommModVer.Value;
            }
        }

        /// <summary>
        /// Gets the Comm Module Version only
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/29/10 RCG 2.40.30		   Created

        public byte CommModuleVersion
        {
            get
            {
                byte byValue = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_COMM_MOD_VER, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL60_COMM_MOD_VER, null, out objValue);
                    byValue = (byte)objValue;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Comm Module Revision only
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/29/10 RCG 2.40.30		   Created

        public byte CommModuleRevision
        {
            get
            {
                byte byValue = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_COMM_MOD_REV, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL60_COMM_MOD_REV, null, out objValue);
                    byValue = (byte)objValue;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Comm Module Build only
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/29/10 RCG 2.40.30		   Created

        public byte CommModuleBuild
        {
            get
            {
                byte byValue = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_COMM_MOD_BUILD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL60_COMM_MOD_BUILD, null, out objValue);
                    byValue = (byte)objValue;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Comm module build number
        /// </summary>
        public string CommModBuild
        {
            get
            {
                if (!m_strCommModBuild.Cached)
                {
                    byte byBuild = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_COMM_MOD_BUILD);
                    m_strCommModBuild.Value = byBuild.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strCommModBuild.Value;
            }
        }

        /// <summary>
        /// Gets the Han module type (Zigbee)
        /// </summary>
        public virtual string HanModType
        {
            get
            {
                if (!m_strHanModType.Cached)
                {
                    byte byHANType = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_HAN_MOD_TYPE);
                    m_strHanModType.Value = OpenWayMfgTable2108.TranslationHanModType(byHANType);
                }

                return m_strHanModType.Value;
            }
        }

        /// <summary>
        /// Gets the Han module version.revision
        /// </summary>
        public virtual string HanModVer
        {
            get
            {
                if (!m_strHanModVer.Cached)
                {
                    byte byVersion = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_HAN_MOD_VER);
                    byte byRevison = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_HAN_MOD_REV);

                    m_strHanModVer.Value = byVersion.ToString(CultureInfo.CurrentCulture)
                            + "." + byRevison.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strHanModVer.Value;
            }
        }

        /// <summary>
        /// Gets the Han module build number
        /// </summary>
        public virtual string HanModBuild
        {
            get
            {
                if (!m_strHanModBuild.Cached)
                {
                    byte byBuild = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_HAN_MOD_BUILD);
                    m_strHanModBuild.Value = byBuild.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strHanModBuild.Value;
            }
        }

        /// <summary>
        /// Gets the HAN Channel Number.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 01/16/14 DLG 3.50.28 TR 9518   Created. Also related to TR 9519 and 9531.
        // 
        public string HanChannelNbr
        {
            get
            {
                if (!m_strHanChannelNbr.Cached)
                {
                    byte byChannelNbr = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL56_CH_NBR);
                    m_strHanChannelNbr.Value = byChannelNbr.ToString(CultureInfo.CurrentCulture);
                }

                return m_strHanChannelNbr.Value;
            }
        }

        /// <summary>
        /// Gets the PAN ID.
        /// </summary>
        /// <summary>
        /// Gets the PAN ID.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 01/16/14 DLG 3.50.28 TR 9518   Created. Also related to TR 9519 and 9531.
        // 
        public string PanId
        {
            get
            {
                if (!m_strPanId.Cached)
                {
                    int iPanId = GetMFGEDLInt(CentronTblEnum.MFGTBL56_PAN_ID);
                    m_strPanId.Value = iPanId.ToString("X4", CultureInfo.CurrentCulture);
                }

                return m_strPanId.Value;
            }
        }

        /// <summary>
        /// Gets the HAN StartUp Option
        /// </summary>
        /// <summary>
        /// Gets the PAN ID.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 01/17/14 DLG 3.50.28 TR 9518   Created. Also related to TR 9519 and 9531.
        // 05/09/14 jrf 3.50.91 WR 504003 Redefined HANStartUpOptions Unknown value to HANDisabled and 
        //                                had to use new resource string when value was truly undefined.
        public string HANStartUpOption
        {
            get
            {
                if (!m_strHanStartupOption.Cached)
                {
                    byte byStartUpOption = (byte)GetMFGEDLInt(CentronTblEnum.MfgTbl58StartupOptions);

                    if (Enum.IsDefined(typeof(HANStartupOptions), byStartUpOption))
                    {
                        m_strHanStartupOption.Value = ((HANStartupOptions)byStartUpOption).ToDescription();
                    }
                    else
                    {
                        m_strHanStartupOption.Value = Resources.Unknown;
                    }
                }

                return m_strHanStartupOption.Value;
            }
        }

        /// <summary>
        /// Gets the ZigBeePowerLevel
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 01/17/14 DLG 3.50.28 TR 9518   Created. Also related to TR 9519 and 9531.
        // 
        public string PowerLevel
        {
            get
            {
                if (!m_strZigBeePowerLevel.Cached)
                {
                    m_strZigBeePowerLevel.Value = ZigBeePowerLevel.ToString();
                }

                return m_strZigBeePowerLevel.Value;
            }
        }

        /// <summary>
        /// Gets the register module version.revision from MFG Table 2108
        /// </summary>
        public string RegModVer
        {
            get
            {
                // These were specifically not Implemented, because
                //  we do not use them in the Device.  (Should we delete them?)
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Gets the register module build number
        /// </summary>
        public string RegModBuild
        {
            get
            {
                // These were specifically not Implemented, because
                //  we do not use them in the Device.  (Should we delete them?)
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Gets the display version.revision fom MFG Table 2108
        /// </summary>
        public virtual string DisplayModVer
        {
            get
            {
                if (!m_strDispModVer.Cached)
                {
                    byte byVersion = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_DISPLAY_FW_VER);
                    byte byRevison = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_DISPLAY_FW_REV);

                    m_strDispModVer.Value = byVersion.ToString(CultureInfo.CurrentCulture)
                            + "." + byRevison.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strDispModVer.Value;
            }
        }

        /// <summary>
        /// Gets the display Build fom MFG Table 2108
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/08/08 RCG 2.10.00 N/A		Created

        public virtual string DisplayModBuild
        {
            get
            {
                if (!m_strDispModBuild.Cached)
                {
                    byte byBuild = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_DISPLAY_FW_BUILD);
                    m_strDispModBuild.Value = byBuild.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strDispModBuild.Value;
            }
        }

        /// <summary>
        /// Retrieves the instantaneous secondary Volts RMS Phase A from the meter.
        /// The firmware folks say this should be considered to be the service voltage.
        /// </summary>
        public float ServiceVoltage
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        /// <summary>
        /// Retrieves the HAN MAC Address from table 2104
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/09/08 AF  1.51.04 116680 Created
        //
        public virtual string HanMACAddress
        {
            get
            {
                string strMAC = "";

                // Check on the existence of the data
                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL56_SRV_MAC_ADDRESS, null))
                {
                    object objValue = null;
                    ulong ulMAC = 0;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL56_SRV_MAC_ADDRESS, null, out objValue);
                    ulMAC = (ulong)objValue;
                    strMAC = ulMAC.ToString("X16", CultureInfo.CurrentCulture);
                }

                return strMAC;
            }
        }

        /// <summary>
        /// Retrieves the RFLAN MAC Address from standard table 122.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/09/08 AF  1.51.04 116680 Created
        //
        public virtual string RFLANMACAddress
        {
            get
            {
                string strMAC = "";
                //array given to GetValue that represents the block index of the value
                //that is being accessed
                int[] IndexArray = { 0 };

                // Check on the existence of the data
                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL122_NATIVE_ADDRESS, IndexArray))
                {
                    object objValue = null;
                    byte[] byaData;

                    m_CenTables.GetValue(StdTableEnum.STDTBL122_NATIVE_ADDRESS, IndexArray, out objValue);
                    byaData = (byte[])objValue;
                    ulong ulMAC = (ulong)(byaData[0] + (byaData[1] << 8) + (byaData[2] << 16) + (byaData[3] << 24));
                    strMAC = ulMAC.ToString("X8", CultureInfo.CurrentCulture);
                }

                return strMAC;
            }
        }

        /// <summary>
        /// Gets the Fatal Recovery Data for the last fatal error.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/19/13 jrf 2.70.13 288152 Created.
        //
        public FatalErrorHistoryData LastFatalErrorData
        {
            get
            {
                FatalErrorHistoryData LastError = null;

                if (Table2261 != null)
                {
                    if (Table2261.FatalErrorHistory != null && Table2261.FatalErrorHistory.Count > 0)
                    {
                        LastError = Table2261.FatalErrorHistory[Table2261.FatalErrorHistory.Count - 1];
                    }
                }

                return LastError;
            }
        }

        /// <summary>
        /// Gets whether or not Fatal Error Recovery is enabled in the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/19/13 jrf 2.70.13 288152 Created.
        //
        public virtual bool IsFatalErrorRecoveryEnabled
        {
            get
            {
                bool bEnabled = false;

                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) >= 0)
                {
                    bEnabled = Table3.IsFatalErrorRecoveryEnabled;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently in Fatal Error Recovery Mode
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/19/13 jrf 2.70.13 288152 Created.
        //
        public virtual bool IsInFatalErrorRecoveryMode
        {
            get
            {
                bool bIsInRecoveryMode = false;

                // This only applies to SR 3.0 meters so we should first check the FW version
                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) >= 0)
                {
                    bIsInRecoveryMode = Table3.IsInFatalErrorRecoveryMode;
                }

                return bIsInRecoveryMode;
            }
        }

        /// <summary>
        /// Gets whether the meter is currently operating in ChoiceConnect mode.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/18/13 jrf 3.50.06 TQ 9479 Created 
        //  
        public bool IsInChoiceConnectMode
        {
            get
            {
                bool bIsInCCMode = false;

                if (null != Table2428 && OpenWayMFGTable2428.ChoiceConnectCommOpMode.ChoiceConnectOperationalMode == Table2428.CurrentRegisterMode)
                {
                    bIsInCCMode = true;
                }

                return bIsInCCMode;
            }
        }

        #endregion CENTRON AMI Device Status

        #region Previous Season

        /// <summary>
        /// Gets whether the meter has any previous season data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public bool HasPreviousSeasonData
        {
            get
            {
                return (null == PreviousSeasonRegisters) ? false : (PreviousSeasonRegisters.Count > 0);
            }
        }
        
        /// <summary>
        /// Proves access to a list of Energy Quantities from last season (Std table 24)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.
        // 01/17/14 jrf 3.50.26 TQ9558 Reordered quantities to be consistent with ordering 
        //                             of other quantity lists.
        public List<Quantity> PreviousSeasonRegisters
        {
            get
            {
                List<Quantity> QuantityList = new List<Quantity>();
                Quantity Qty;

                // Add Watts Del
                Qty = PreviousSeasonWattsDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Rec
                Qty = PreviousSeasonWattsReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Net
                Qty = PreviousSeasonWattsNet;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Uni
                Qty = PreviousSeasonWattsUni;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Del
                Qty = PreviousSeasonVADelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Rec
                Qty = PreviousSeasonVAReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Lag
                Qty = PreviousSeasonVALagging;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Del
                Qty = PreviousSeasonVarDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Rec
                Qty = PreviousSeasonVarReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net
                Qty = PreviousSeasonVarNet;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net Del
                Qty = PreviousSeasonVarNetDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net Rec
                Qty = PreviousSeasonVarNetReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q1
                Qty = PreviousSeasonVarQuadrant1;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q2
                Qty = PreviousSeasonVarQuadrant2;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q3
                Qty = PreviousSeasonVarQuadrant3;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q4
                Qty = PreviousSeasonVarQuadrant4;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (a)
                Qty = PreviousSeasonAmpsPhaseA;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (b)
                Qty = PreviousSeasonAmpsPhaseB;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (c)
                Qty = PreviousSeasonAmpsPhaseC;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A Neutral
                Qty = PreviousSeasonAmpsNeutral;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A^2
                Qty = PreviousSeasonAmpsSquared;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (a)
                Qty = PreviousSeasonVoltsPhaseA;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (b)
                Qty = PreviousSeasonVoltsPhaseB;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (c)
                Qty = PreviousSeasonVoltsPhaseC;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V Avg
                Qty = PreviousSeasonVoltsAverage;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V^2
                Qty = PreviousSeasonVoltsSquared;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add PF
                Qty = PreviousSeasonPowerFactor;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Q Del
                Qty = PreviousSeasonQDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Q Rec
                Qty = PreviousSeasonQReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }

                return QuantityList;
            }
        }

        /// <summary>
        /// Gets the Neutral Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonAmpsNeutral
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_AH_NEUTRAL, null,
                        "Neutral Amps", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase A Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonAmpsPhaseA
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_AH_PHA, null,
                    "Amps (a)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase B Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonAmpsPhaseB
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_AH_PHB, null,
                    "Amps (b)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase C Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonAmpsPhaseC
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_AH_PHC, null,
                    "Amps (c)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Amps squared from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonAmpsSquared
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_I2H_AGG, null,
                    "Amps Squared", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Q Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonQDelivered
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_QH_DEL, null,
                    "Q Delivered", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Qh Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonQReceived
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_QH_REC, null,
                    "Q Received", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the VA Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVADelivered
        {
            get
            {
                Quantity VA = null;

                if (null != Table24)
                {
                    // Try getting Arithmatic first.
                    VA = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_DEL_ARITH, null,
                        "VA Delivered", Table24.PreviousSeasonRegisterData);

                    // Try  getting Vectoral
                    if (VA == null)
                    {
                        VA = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_DEL_VECT, null,
                            "VA Delivered", Table24.PreviousSeasonRegisterData);
                    }
                }

                return VA;
            }
        }

        /// <summary>
        /// Gets the Lagging VA from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVALagging
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_LAG, null,
                    "VA Lagging", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarDelivered
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_DEL, null,
                    "Var Delivered", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the VA Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVAReceived
        {
            get
            {
                Quantity VA = null;

                if (null != Table24)
                {

                    // Try getting Arithmetic first.
                    VA = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_REC_ARITH, null,
                        "VA Received", Table24.PreviousSeasonRegisterData);

                    // Try  getting Vectorial
                    if (VA == null)
                    {
                        VA = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_REC_VECT, null,
                            "VA Received", Table24.PreviousSeasonRegisterData);
                    }
                }

                return VA;
            }
        }

        /// <summary>
        /// Gets the Var Net from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarNet
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_NET, null,
                    "Var Net", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Net delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarNetDelivered
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_NET_DEL, null,
                    "Var Net Delivered", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Net Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarNetReceived
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_NET_REC, null,
                    "Var Net Received", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Q1 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarQuadrant1
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_Q1, null,
                    "Var Quadrant 1", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Q2 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarQuadrant2
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_Q2, null,
                    "Var Quadrant 2", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Q3 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarQuadrant3
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_Q3, null,
                    "Var Quadrant 3", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Q4 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarQuadrant4
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_Q4, null,
                    "Var Quadrant 4", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVarReceived
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_REC, null,
                    "Var Received", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Average Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVoltsAverage
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VH_AVG, null,
                    "Volts Average", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase A Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVoltsPhaseA
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VH_PHA, null,
                    "Volts (a)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase B Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVoltsPhaseB
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VH_PHB, null,
                    "Volts (b)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase C Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVoltsPhaseC
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VH_PHC, null,
                    "Volts (c)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Volts squared from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonVoltsSquared
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_V2H_AGG, null,
                    "Volts Squared", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Watts Delivered quantity from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonWattsDelivered
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_WH_DEL, null,
                    "Watts Delivered", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Watts Received quantity from the standard tables
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonWattsReceived
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_WH_REC, null,
                    "Watts Received", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Watts Net quantity from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonWattsNet
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_WH_NET, null,
                    "Watts Net", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Unidirectional Watts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public Quantity PreviousSeasonWattsUni
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_WH_UNI, null,
                    "Unidirectional Watts", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Power Factor from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/17/14 jrf 3.50.26 TQ9558 Created.

        internal Quantity PreviousSeasonPowerFactor
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    // There is not PF energy so just check the demand
                    Qty = GetQuantityFromStandardTables(null, LIDDefinitions.DEMAND_MIN_PF_INTERVAL_ARITH,
                        "Power Factor", Table24.PreviousSeasonRegisterData);

                    // Also try the vectorial PF
                    if (Qty == null)
                    {
                        Qty = GetQuantityFromStandardTables(null, LIDDefinitions.DEMAND_MIN_PF_INTERVAL_VECT,
                            "Power Factor", Table24.PreviousSeasonRegisterData);
                    }
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the end date of the previous season.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/15/14 jrf 3.50.26 TQ9558 Created.

        public DateTime? PreviousSeasonEndDate
        {
            get
            {
                DateTime? dtEndDate = null;

                if (null != Table24)
                {
                    dtEndDate = Table24.PreviousSeasonEndDate;
                }

                return dtEndDate;
            }
        }

        #endregion


        #region Protected Methods

        /// <summary>
        /// This method sets a value in the Centron Tables.
        /// </summary>
        /// <param name="idElement">The ID of the element to set</param>
        /// <param name="anIndex">The index(es) to find the element</param>
        /// <param name="objValue">The value to set</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- -------------- -------------------------------------------
        //  11/16/10 jrf 2.45.13                 Created
        //
        protected virtual void SetValue(long idElement, int[] anIndex, object objValue)
        {
            m_CenTables.SetValue(idElement, anIndex, objValue);
        }

        /// <summary>
        /// Gets the short value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested short</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual short GetSTDEDLShort(StdTableEnum StdTableEnumValue)
        {
            short sTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                sTemp = (short)Value;
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsCached((long)StdTableEnumValue, null))
            {
                m_GatewayTables.GetValue(StdTableEnumValue, null, out Value);
                sTemp = (short)Value;
            }
#endif

            return sTemp;
        }

        /// <summary>
        /// Sets the short value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="sValue">Value to set.</param>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/23/10 jrf 2.44.04        Created
        //
        protected virtual void SetSTDEDLShort(short sValue, StdTableEnum StdTableEnumValue)
        {
            object Value = sValue;

            m_CenTables.SetValue(StdTableEnumValue, null, Value);
        }

        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested String</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual string GetSTDEDLString(StdTableEnum StdTableEnumValue)
        {
            string strTemp = "";
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                strTemp = Value.ToString();
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsCached((long)StdTableEnumValue, null))
            {
                m_GatewayTables.GetValue(StdTableEnumValue, null, out Value);
                strTemp = Value.ToString();
            }
#endif

            return strTemp;
        }

        /// <summary>
        /// Sets the string value of the EDL defined by the supplied enumeration.
        /// </summary>
        /// <param name="strValue">Value to set.</param>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested String</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/25/10 jrf 2.45.10        Created
        //
        protected virtual void SetSTDEDLString(String strValue, StdTableEnum StdTableEnumValue)
        {
            object Value = strValue;

            m_CenTables.SetValue(StdTableEnumValue, null, Value);
        }

        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="stdTableEnum">Centron AMI specific Enumberation Value</param>
        /// <param name="aiIndex">Parameterized index.</param>
        /// <returns>Requested ushort</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#        Description
        // -------- --- ------- ------------- ---------------------------------------
        // 06/11/08 jrf 1.50.34               Switched to call ushort.Parse() that used 
        //                                    the number styles parameter to be compatible
        //                                    with the compact framework.
        // 08/02/10 AF  2.42.11               Added support for M2 Gateway
        // 03/26/12 AF  2.53.52 191613        Corrected the M2 Gateway code
        //
        protected virtual ushort GetSTDEDLUShort(StdTableEnum stdTableEnum, params int[] aiIndex)
        {
            ushort usTemp = 0;
            object Value;

            if (m_CenTables.IsCached((long)stdTableEnum, aiIndex))
            {
                m_CenTables.GetValue(stdTableEnum, aiIndex, out Value);
                usTemp = ushort.Parse(Value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsCached((long)stdTableEnum, aiIndex))
            {
                m_GatewayTables.GetValue(stdTableEnum, aiIndex, out Value);
                usTemp = ushort.Parse(Value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
            }
#endif

            return usTemp;
        }

        /// <summary>
        /// Gets the int value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested int</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual int GetSTDEDLInt(StdTableEnum StdTableEnumValue)
        {
            int intTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                intTemp = (int)int.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsCached((long)StdTableEnumValue, null))
            {
                m_GatewayTables.GetValue(StdTableEnumValue, null, out Value);
                intTemp = (int)int.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }
#endif

            return intTemp;
        }

        /// <summary>
        /// Gets the uint value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested uint</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/25/16 jrf 4.70.28 230427 Created.
        protected virtual uint GetSTDEDLUInt(StdTableEnum StdTableEnumValue)
        {
            uint uintTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                uintTemp = (uint)uint.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsCached((long)StdTableEnumValue, null))
            {
                m_GatewayTables.GetValue(StdTableEnumValue, null, out Value);
                uintTemp = (uint)uint.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }
#endif

            return uintTemp;
        }

        /// <summary>
        /// Gets the float value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested float</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual float GetSTDEDLFloat(StdTableEnum StdTableEnumValue)
        {
            float fltTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                fltTemp = float.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsCached((long)StdTableEnumValue, null))
            {
                m_GatewayTables.GetValue(StdTableEnumValue, null, out Value);
                fltTemp = float.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }
#endif

            return fltTemp;
        }

        /// <summary>
        /// Gets the bool value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested bool</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual bool GetSTDEDLBool(StdTableEnum StdTableEnumValue)
        {
            bool blnTemp = false;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                blnTemp = (bool)Value;
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsCached((long)StdTableEnumValue, null))
            {
                m_GatewayTables.GetValue(StdTableEnumValue, null, out Value);
                blnTemp = (bool)Value;
            }
#endif

            return blnTemp;
        }

        /// <summary>
        /// Gets the byte value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested bool</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/25/16 jrf 4.70.28 230427 Created.
        protected virtual byte GetSTDEDLByte(StdTableEnum StdTableEnumValue)
        {
            byte Temp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                Temp = (byte)Value;
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsCached((long)StdTableEnumValue, null))
            {
                m_GatewayTables.GetValue(StdTableEnumValue, null, out Value);
                Temp = (byte)Value;
            }
#endif

            return Temp;
        }

        /// <summary>
        /// Sets the bool value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="blnValue">The value to set</param>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        private void SetSTDEDLBool(bool blnValue, StdTableEnum StdTableEnumValue)
        {
            m_CenTables.SetValue(StdTableEnumValue, null, blnValue);
        }

        /// <summary>
        /// Raises the event to show the progress bar.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/31/07 RCG 9.00.06        Adding progressable support	

        protected void OnShowProgress(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that causes the progress bar to perform a step
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/31/07 RCG 9.00.06        Adding progressable support	

        protected void OnStepProgress(ProgressEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that hides or closes the progress bar
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/31/07 RCG 9.00.06        Adding progressable support	

        protected void OnHideProgress(EventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Load the EDL file into the centron table variable
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/26/06 RDB				   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual void LoadFile()
        {
            XmlReader xmlReader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CheckCharacters = false;

            xmlReader = XmlReader.Create(m_strEDLFile, settings);

            m_CenTables = new CentronTables();
#if (!WindowsCE)
            m_GatewayTables = new GatewayTables();
#endif

            if (EDLDeviceTypes.M2GatewayDevice != EDLFile.DetermineDeviceType(m_strEDLFile))
            {
                m_CenTables.LoadEDLFile(xmlReader);
            }
#if (!WindowsCE)
            else
            {
                m_GatewayTables.LoadEDLFile(xmlReader);
            }
#endif
        }//LoadFile

        /// <summary>
        /// Save the EDL file from the centron table variable
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/26/06 RDB				   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual void SaveFile()
        {
            XmlWriter xmlWriter;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.ASCII;
            settings.Indent = true;
            settings.CheckCharacters = false;

            xmlWriter = XmlWriter.Create(m_strEDLFile, settings);

            m_CenTables.SaveEDLFile(xmlWriter, null, AllowTableExport, AllowFieldExport);

        }//LoadFile

        /// <summary>
        /// Used to determine which tables will be written to the EDL file.
        /// </summary>
        /// <param name="usTableID">Table ID to check.</param>
        /// <returns>True if the table can be written, false otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created

        protected bool AllowTableExport(ushort usTableID)
        {
            // We are going to control the tables that are written
            // to the EDL file by the tables that we read. This
            // way we only need to change one place whenever new
            // tables are added or removed.
            return true;
        }

        /// <summary>
        /// Initializes the Cached Data Items
        /// </summary>
        protected void InitializeInstanceVariables()
        {
            m_strSWRevision = new CachedString();
            m_strSerialNumber = new CachedString();
            m_strDeviceID = new CachedString();
            m_strTOUID = new CachedString();
            m_strTarrifID = new CachedString();
            m_strUserData1 = new CachedString();
            m_strUserData2 = new CachedString();
            m_strUserData3 = new CachedString();
            m_uiColdLoadPickupTime = new CachedUint();
            m_iDemandIntervalLength = new CachedInt();
            m_iNumDemandSubIntervals = new CachedInt();
            m_iDemandSchedulingControl = new CachedInt();
            m_iDemandResetScheduledDay = new CachedInt();
            m_iDemandResetScheduledMinute = new CachedInt();
            m_iDemandResetScheduledHour = new CachedInt();
            m_iNumTestModeDemandSubIntervals = new CachedInt();
            m_iTestModeDemandIntervalLength = new CachedInt();
            m_iLPIntervalLength = new CachedInt();
            m_iNumLPChannels = new CachedInt();
            m_iLPMinPowerOutage = new CachedInt();
            m_iLPMemorySize = new CachedInt();
            m_iProgramID = new CachedInt();
            m_fltCTRatio = new CachedFloat();
            m_fltVTRatio = new CachedFloat();
            m_fltRegisterMultiplier = new CachedFloat();
            m_iCLPUOutageTime = new CachedInt();
            m_iModeTimeout = new CachedInt();
            m_strDailySelfReadTime = new CachedString();
            m_bDailySelfReadEnabled = new CachedBool();
            m_bytDailySelfReadHour = new CachedByte();
            m_bytDailySelfReadMinute = new CachedByte();
            m_strLoadControlThreshold = new CachedString();
            m_strLoadControlReconnect = new CachedString();
            m_dtDateProgrammed = new CachedDate();
            m_iNumDemandResets = new CachedInt();
            m_tsTimeZoneOffset = new CachedTimeSpan();
            m_DSTChangeTime = new CachedTimeSpan();
            m_DSTOffset = new CachedTimeSpan();
            //m_uiNumberOfInverstionTampers = new CachedUint();
            //m_uiNumberOfRemovalTampers= new CachedUint();
            m_strCommModType = new CachedString();
            m_strCommModVer = new CachedString();
            m_strCommModBuild = new CachedString();
            m_strHanModType = new CachedString();
            m_strHanModVer = new CachedString();
            m_strHanModBuild = new CachedString();
            m_strHanChannelNbr = new CachedString();
            m_strPanId = new CachedString();
            m_strHanStartupOption = new CachedString();
            m_strZigBeePowerLevel = new CachedString();
            m_strDispModVer = new CachedString();
            m_strDispModBuild = new CachedString();
            m_strRegModVer = new CachedString();
            m_strRegModBuild = new CachedString();
            m_fltServiceVoltage = new CachedFloat();
            m_strClockSynch = new CachedString();
            m_strDSTSwitch = new CachedString();
            m_iDSTLength = new CachedInt();
            m_blnDisplayEOI = new CachedBool();
            m_blnWattIndicator = new CachedBool();
            m_blnDisonnectOFFMessage = new CachedBool();
            m_iDisplayOnTime = new CachedInt();
            m_strLowBatteryError = new CachedString();
            m_strLossOfPhaseError = new CachedString();
            m_strClockTOUError = new CachedString();
            m_strReversePowerError = new CachedString();
            m_strLoadProfileError = new CachedString();
            m_strFullScaleError = new CachedString();
            m_strSiteScanError = new CachedString();
            m_VMData = null;

            m_strMFGSerialNumber = new CachedString();
            m_fltFWRevision = new CachedFloat();
            m_blnDSTEnabled = new CachedBool();
            m_dtCurrentTime = new CachedDate();
            m_fltHWRevision = new CachedFloat();
            m_blnMeterInDST = new CachedBool();
            m_blnMeterInTestMode = new CachedBool();
            m_strRelayNativeAddress = new CachedString();
            m_strMasterRelayAptitle = new CachedString();
            m_strNodeAptitle = new CachedString();
            m_usRegistrationDelay = new CachedUshort();
            m_tsRegistrationPeriod = new CachedTimeSpan();
            m_tsRegistrationCountDown = new CachedTimeSpan();
            m_blnDeviceInGMT = new CachedBool();
            m_blnTimeZoneEnabled = new CachedBool();
            m_strDeviceClass = new CachedString();
            m_byFWBuild = new CachedByte();
            m_blnSupports25YearTOU = new CachedBool();
            m_VAVARSelection = new CachedValue<short?>();
            m_VACalculation = new CachedValue<byte?>();
            m_UIInstrumentationProfileVAArithmeticConfigured = new CachedValue<bool?>();
            m_UIInstrumentationProfileVAVectorialConfigured = new CachedValue<bool?>();
            m_TimeFormat = new CachedInt();
        }

        /// <summary>
        /// Determines which fields may be written to the EDL file.
        /// </summary>
        /// <param name="idElement">The field to check.</param>
        /// <param name="anIndex">The indexes into the field.</param>
        /// <returns>True if the field may  be written to the EDL file. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created

        protected bool AllowFieldExport(long idElement, int[] anIndex)
        {
            bool bAllowExport = false;

            // Currently there are no fields that we wish to exclude
            switch (idElement)
            {
                // These items are in 2048 but are also stored elsewhere
                // so we do not need to export them twice
                case (long)CentronTblEnum.MFGTBL0_UNKNOWN_BLOCK:
                case (long)CentronTblEnum.MFGTBL0_DECADE_0:
                case (long)CentronTblEnum.MFGTBL0_DECADE_8:
                {
                    bAllowExport = false;
                    break;
                }
                default:
                {
                    bAllowExport = true;
                    break;
                }
            }

            return bAllowExport;
        }

        /// <summary>
        /// Build sup the Network Statistic Structure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual void BuildNetworkStatistics()
        {
            IList<TableData> lsttblData121 = null;
            IList<TableData> lsttblData127 = null;

            //Get Table 121
            if (m_CenTables.IsAllCached(121))
            {
                lsttblData121 = m_CenTables.BuildPSEMStreams(121);
            }
#if (!WindowsCE)
            else
            {
                lsttblData121 = m_GatewayTables.BuildPSEMStreams(121);
            }
#endif
            lsttblData121[0].PSEM.Position = 0;
            PSEMBinaryReader Tbl121BinaryReader = new PSEMBinaryReader(lsttblData121[0].PSEM);
            CStdTable121 tbl121 = new CStdTable121(Tbl121BinaryReader);

            if (m_CenTables.IsAllCached(127))
            {
                lsttblData127 = m_CenTables.BuildPSEMStreams(127);
            }
#if (!WindowsCE)
            else
            {
                lsttblData127 = m_GatewayTables.BuildPSEMStreams(127);
            }
#endif

            lsttblData127[0].PSEM.Position = 0;
            PSEMBinaryReader Tbl127BinaryReader = new PSEMBinaryReader(lsttblData127[0].PSEM);
            CStdTable127 tbl127 = new CStdTable127(Tbl127BinaryReader, tbl121);

            m_lstNetworkStatistic = tbl127.GetStatistics();
            //Get Table 127
        }

        /// <summary>
        /// Get the list of supported Std and Mfg Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual void GetSupportedCommunicationsEvents()
        {
            // Make sure that the EDL file contains the Log tables
            if (m_CenTables.IsTableKnown(2159) && m_CenTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_CenTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_CenTables.IsTableKnown(2160) && m_CenTables.IsAllCached(2160))
                {
                    //Get Table 2160 - Supported Events
                    IList<TableData> lsttblData2160 = m_CenTables.BuildPSEMStreams(2160);
                    lsttblData2160[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2160BinaryReader = new PSEMBinaryReader(lsttblData2160[0].PSEM);
                    MFGTable2160 tbl2160 = new MFGTable2160(Tbl2160BinaryReader, tbl2159);

                    m_lstSupportedStdEvents = tbl2160.StdEventSupported;
                    m_lstSupportedMFGEvents = tbl2160.MfgEventSupported;
                }
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsTableKnown(2159) && m_GatewayTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_GatewayTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_GatewayTables.IsTableKnown(2160) && m_GatewayTables.IsAllCached(2160))
                {
                    //Get Table 2160 - Supported Events
                    IList<TableData> lsttblData2160 = m_GatewayTables.BuildPSEMStreams(2160);
                    lsttblData2160[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2160BinaryReader = new PSEMBinaryReader(lsttblData2160[0].PSEM);
                    MFGTable2160 tbl2160 = new MFGTable2160(Tbl2160BinaryReader, tbl2159);

                    m_lstSupportedStdEvents = tbl2160.StdEventSupported;
                    m_lstSupportedMFGEvents = tbl2160.MfgEventSupported;
                }
            }
#endif
        }

        /// <summary>
        /// Reads the array of event entries read from table 76.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        // 08/03/10 AF  2.42.11        Added M2 Gateway support
        // 08/10/10 AF  2.42.17        Added M2 Gateway event dictionary
        //
        protected virtual void GetEventEntries()
        {
            object Value = null;
            ushort NbrValidEntries = 0;
            int[] anIndex = { 0 };
            bool EventDateTimeFlag = true; //always true
            bool EventNbrFlag = false;
            bool EventSeqNbrFlag = true; //always true
            byte EventDataLength = 0;

            if (m_CenTables.IsAllCached(71))
            {
                m_CenTables.GetValue(StdTableEnum.STDTBL71_EVENT_NUMBER_FLAG,
                    null, out Value);
                EventNbrFlag = (bool)Value;

                m_CenTables.GetValue(StdTableEnum.STDTBL71_HIST_DATA_LENGTH,
                    null, out Value);
                EventDataLength = (byte)Value;
                m_CenTables.GetValue(StdTableEnum.STDTBL76_NBR_VALID_ENTRIES,
                    null, out Value);
                NbrValidEntries = (ushort)Value;
                m_EventEntries = new HistoryEntry[NbrValidEntries];

                for (int i = 0; i < NbrValidEntries; i++)
                {
                    anIndex[0] = i;
                    m_EventEntries[i] = new HistoryEntry(EventDateTimeFlag, EventNbrFlag,
                                EventSeqNbrFlag, EventDataLength,
                                m_EventDictionary);

                    m_CenTables.GetValue(StdTableEnum.STDTBL76_EVENT_TIME,
                        anIndex, out Value);
                    m_EventEntries[i].HistoryTime = (DateTime)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL76_EVENT_CODE,
                        anIndex, out Value);
                    m_EventEntries[i].HistoryCode = (ushort)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL76_EVENT_ARGUMENT,
                        anIndex, out Value);
                    m_EventEntries[i].HistoryArgument = (byte[])Value;
                }
            }
#if (!WindowsCE)
            else
            {
                m_GatewayTables.GetValue(StdTableEnum.STDTBL71_EVENT_NUMBER_FLAG,
                    null, out Value);
                EventNbrFlag = (bool)Value;

                m_GatewayTables.GetValue(StdTableEnum.STDTBL71_HIST_DATA_LENGTH,
                    null, out Value);
                EventDataLength = (byte)Value;
                m_GatewayTables.GetValue(StdTableEnum.STDTBL76_NBR_VALID_ENTRIES,
                    null, out Value);
                NbrValidEntries = (ushort)Value;
                m_EventEntries = new HistoryEntry[NbrValidEntries];

                for (int i = 0; i < NbrValidEntries; i++)
                {
                    anIndex[0] = i;
                    m_EventEntries[i] = new HistoryEntry(EventDateTimeFlag, EventNbrFlag,
                                EventSeqNbrFlag, EventDataLength,
                                m_GWEventDictionary);

                    m_GatewayTables.GetValue(StdTableEnum.STDTBL76_EVENT_TIME,
                        anIndex, out Value);
                    m_EventEntries[i].HistoryTime = (DateTime)Value;

                    m_GatewayTables.GetValue(StdTableEnum.STDTBL76_EVENT_CODE,
                        anIndex, out Value);
                    m_EventEntries[i].HistoryCode = (ushort)Value;

                    m_GatewayTables.GetValue(StdTableEnum.STDTBL76_EVENT_ARGUMENT,
                        anIndex, out Value);
                    m_EventEntries[i].HistoryArgument = (byte[])Value;
                }
            }
#endif
        }

        /// <summary>
        /// Peeks into the EDL file to determine whether the
        /// device type is an OpenWay CENTRON or an M2 Gateway
        /// </summary>
        /// <returns>Enumeration of the device type</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 08/02/10 AF  2.42.11           Created
        // 07/02/13 AF  2.80.45 TR 7640   Added ICS Gateway (I-210/kV2c) to the type recognized
        // 01/07/14 DLG 3.50.19 TR 9518   Added ICS Gateway ITRU and ITRV to the type recognized.
        // 
        protected static EDLDeviceTypes DetermineDeviceType(string FileName)
        {
            EDLDeviceTypes DevType = EDLDeviceTypes.OpenWayCentron;
            string strDevClass = "";
            bool bDone = false;

            XmlReader xmlReader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CheckCharacters = false;

            xmlReader = XmlReader.Create(FileName, settings);

            xmlReader.MoveToElement();

            while ((false == xmlReader.EOF) && (false == bDone))
            {
                xmlReader.Read();

                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (GEN_CFG_TBL_DEV_CLASS == xmlReader.Name)
                            {
                                xmlReader.MoveToNextAttribute();

                                if ("binary" == xmlReader.Name)
                                {
                                    strDevClass = xmlReader.Value;
                                }

                                if ((String.Compare(DEV_CLASS_ASCII_CODE_LIS1, strDevClass, StringComparison.OrdinalIgnoreCase) == 0) ||
                                    (String.Compare(DEV_CLASS_ASCII_CODE_lis1, strDevClass, StringComparison.OrdinalIgnoreCase) == 0))
                                {
                                    DevType = EDLDeviceTypes.M2GatewayDevice;
                                }
                                else if ((String.Compare(DEV_CLASS_ASCII_CODE_ITRH, strDevClass, StringComparison.OrdinalIgnoreCase) == 0) ||
                                         (String.Compare(DEV_CLASS_ASCII_CODE_ITRU, strDevClass, StringComparison.OrdinalIgnoreCase) == 0) ||
                                         (String.Compare(DEV_CLASS_ASCII_CODE_ITRV, strDevClass, StringComparison.OrdinalIgnoreCase) == 0))
                                {
                                    DevType = EDLDeviceTypes.ICSGatewayDevice;
                                }
                                else
                                {
                                    DevType = EDLDeviceTypes.OpenWayCentron;
                                }
                                bDone = true;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            xmlReader.Close();

            return DevType;
        }

        /// <summary>
        /// Puts the meaningful data from a status byte into a human-readable string
        /// </summary>
        /// <param name="bStatus">interval status byte</param>
        /// <param name="iNibble"></param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version    Issue#      Description
        // -------- --- -------    ------      ---------------------------------------
        // 10/26/06 RDB			   	           Created
        // 02/19/09 jrf 2.10.05    127954      Making change to time adjust interval statuses
        //                                     to be consistent with changes being made to firmware.
        // 03/09/11 RCG 2.50.07                Adding support for Power Restoration bit
        // 06/01/16 MP  4.50.276   WR392401    Changed Power Restoration from "SR" to "RS"   
        protected string GetChannelStatus(byte bStatus, int iNibble)
        {
            string strStatus = "";

            //get status from the interval status nibble
            if (iNibble == 2)
            {
                if ((bStatus & 0x10) != 0)
                {
                    strStatus += "D";
                }
                if ((bStatus & 0x20) != 0)
                {
                    strStatus += "O";
                }

                //Only show one A for either a backwards or forwards time adjust.
                if ((bStatus & 0x40) != 0)
                {
                    strStatus += "A";
                }
                else if ((bStatus & 0x80) != 0)
                {
                    strStatus += "A";
                }
            }
            //get status from channel status nibble
            else if (iNibble == 0)
            {
                switch (bStatus & 0x0F)
                {
                    case 0x01:
                        {
                            // Overflow
                            strStatus += "V";
                            break;
                        }
                    case 0x02:
                        {
                            // Partial Interval
                            strStatus += "S";
                            break;
                        }
                    case 0x03:
                        {
                            // Long Interval
                            strStatus += "L";
                            break;
                        }
                    case 0x04:
                        {
                            // Skipped Interval
                            strStatus += "K";
                            break;
                        }
                    case 0x05:
                        {
                            // Test Mode
                            strStatus += "T";
                            break;
                        }
                    case 0x08:
                        {
                            // Power Restoration
                            strStatus += "RS";
                            break;
                        }
                }
            }
            else
            {
                switch (bStatus & 0xF0)
                {
                    case 0x10:
                        {
                            // Overflow
                            strStatus += "V";
                            break;
                        }
                    case 0x20:
                        {
                            // Partial Interval
                            strStatus += "S";
                            break;
                        }
                    case 0x30:
                        {
                            // Long Interval
                            strStatus += "L";
                            break;
                        }
                    case 0x40:
                        {
                            // Skipped Interval
                            strStatus += "K";
                            break;
                        }
                    case 0x50:
                        {
                            // Test Mode
                            strStatus += "T";
                            break;
                        }
                    case 0x80:
                        {
                            // Power Restoration
                            strStatus += "RS";
                            break;
                        }
                }
            }
            return strStatus;

        }//GetStatus

        /// <summary>
        /// Creates a VMData object from the EDL file.
        /// </summary>
        /// <returns>The VMData object.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        //  12/21/11 jrf 2.53.21 TREQ2907 Adding support for extended voltage monitoring.
        //  01/31/12 jrf 2.53.36 192967 Fixing issue reading voltage monitoring data.  Must handle
        //                              the case where extended voltage monitoring tables are present
        //                              but legacy voltage monitoring is running.
        // 08/03/15 AF  4.20.19  586155 Added ITRK to list of poly meters with a call IsPolyEDL
        // 12/20/17 AF  4.73.00 Bug705768 Make sure number of steps for the progress bar is not negative
        //
        protected virtual VMData GetVoltageMonitoringData()
        {
            VMData VoltageData = null;
            object objValue;
            bool bEnabled = false;
            ushort usMaxBlocks;
            ushort usUsedBlocks;
            ushort usIntervalsPerBlock;
            ushort usValidIntervals;
            ushort usLastBlock;
            ushort usStartBlock;
            ushort usDivisor;
            ushort usScalar;
            byte[] byaStatus;
            byte byIntervalLength;
            byte byNumPhases;
            bool blnExtendedVMInUse = false;
            List<VMInterval> Intervals = new List<VMInterval>();
            VMStatusFlags LastIntervalStatus;

            // Make sure that the EDL file contains the VM tables
            if (m_CenTables.IsTableKnown(2149) && m_CenTables.IsAllCached(2149))
            {
                // Tables are there now check to see if VM is enabled.
                if (m_CenTables.IsTableKnown(2154) && m_CenTables.IsAllCached(2154))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL106_ENABLE_FLAG, null, out objValue);
                    bEnabled = (bool)objValue;
                    blnExtendedVMInUse = bEnabled;
                }

                //check if legacy voltage monitoring is enabled if extended voltage monitoring is not enabled.
                if (false == bEnabled)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, out objValue);
                    bEnabled = (bool)objValue;
                }

                if (bEnabled == true)
                {
                    // VM is enabled so we should be able to create the object.
                    VoltageData = new VMData();

                    // Get the data necessary for reading the data from 2152
                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL105_NBR_BLK_INTS, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL101_NBR_BLK_INTS, null, out objValue);
                    }
                    usIntervalsPerBlock = (ushort)objValue;

                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL105_VM_INT_LEN, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL101_VM_INT_LEN, null, out objValue);
                    }
                    byIntervalLength = (byte)objValue;
                    VoltageData.IntervalLength = TimeSpan.FromMinutes((double)byIntervalLength);

                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL108_MONITORING_PHASES, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL101_NBR_PHASES, null, out objValue);
                    }
                    byNumPhases = (byte)objValue;
                    VoltageData.NumberOfPhases = byNumPhases;

                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL107_NBR_BLOCKS, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL103_NBR_BLOCKS, null, out objValue);
                    }
                    usMaxBlocks = (ushort)objValue;

                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL107_NBR_VALID_BLOCKS, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null, out objValue);
                    }
                    usUsedBlocks = (ushort)objValue;

                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL107_NBR_VALID_INT, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_INT, null, out objValue);
                    }
                    usValidIntervals = (ushort)objValue;

                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL107_LAST_BLOCK_ELEMENT, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL103_LAST_BLOCK_ELEMENT, null, out objValue);
                    }
                    usLastBlock = (ushort)objValue;

                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL107_DIVISOR, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL103_DIVISOR, null, out objValue);
                    }
                    usDivisor = (ushort)objValue;

                    if (true == blnExtendedVMInUse)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL107_SCALAR, null, out objValue);
                    }
                    else
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL103_SCALAR, null, out objValue);
                    }
                    usScalar = (ushort)objValue;

                    if (IsPolyEDL() || true == blnExtendedVMInUse)
                    {
                        VoltageData.NominalVoltages = new ushort[byNumPhases];

                        for (int iIndex = 0; iIndex < byNumPhases; iIndex++)
                        {
                            if (true == blnExtendedVMInUse)
                            {
                                m_CenTables.GetValue(CentronTblEnum.MFGTBL108_NOMINAL_VOLTAGES, new int[] { iIndex }, out objValue);
                            }
                            else
                            {
                                m_CenTables.GetValue(CentronTblEnum.MfgTbl103NominalVoltages, new int[] { iIndex }, out objValue);
                            }
                            ushort NomVoltage = (ushort)objValue;

                            VoltageData.NominalVoltages[iIndex] = NomVoltage;
                        }
                    }
                    else //Extended VM is not supported or meter is ITR1/ITRD
                    {
                        ushort NomVoltage = DetermineNominalVoltage();
                        VoltageData.NominalVoltages = new ushort[byNumPhases];

                        for (int iIndex = 0; iIndex < byNumPhases; iIndex++)
                        {
                            VoltageData.NominalVoltages[iIndex] = NomVoltage;
                        }
                    }

                    VoltageData.VhLowPercentage = VMVhLowPercentage;
                    VoltageData.VhHighPercentage = VMVhHighPercentage;
                    VoltageData.RMSVoltageLowPercentage = VMRMSLowPercentage;
                    VoltageData.RMSVoltageHighPercentage = VMRMSHighPercentage;

                    // Determine the starting block (We are always assuming circular lists
                    if (usUsedBlocks == usMaxBlocks)
                    {
                        // The data has wrapped
                        usStartBlock = (ushort)((usLastBlock + 1) % usMaxBlocks);
                    }
                    else
                    {
                        // The data has not wrapped so start at 0
                        usStartBlock = 0;
                    }

                    // Make sure the number of steps is not negative
                    int iNumberOfSteps = (usUsedBlocks > 0) ? usUsedBlocks - 1 : 0;

                    OnShowProgress(new ShowProgressEventArgs(1, iNumberOfSteps * usIntervalsPerBlock + usValidIntervals,
                        "", "Retrieving Voltage Monitoring Data..."));

                    // Get the data
                    for (ushort usBlockIndex = 0; usBlockIndex < usUsedBlocks; usBlockIndex++)
                    {
                        DateTime dtBlockEndTime;
                        ushort usActualBlockIndex = (ushort)((usStartBlock + usBlockIndex) % usMaxBlocks);
                        ushort usNumIntervals;
                        int[] IndexArray;

                        IndexArray = new int[] { usActualBlockIndex };

                        if (true == blnExtendedVMInUse)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL109_BLK_END_TIME, IndexArray, out objValue);
                        }
                        else
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL104_BLK_END_TIME, IndexArray, out objValue);
                        }
                        dtBlockEndTime = (DateTime)objValue;

                        if (usActualBlockIndex != usLastBlock)
                        {
                            // Always usIntervalsPerBlock intervals in these blocks
                            usNumIntervals = usIntervalsPerBlock;
                        }
                        else
                        {
                            usNumIntervals = usValidIntervals;
                        }

                        // Determine whether the last interval is in DST or not so we know to adjust it properly.
                        IndexArray = new int[] { usActualBlockIndex, usNumIntervals - 1 };

                        // Get the status - Kevin's code returns these as an array of bytes
                        if (true == blnExtendedVMInUse)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL109_EXTENDED_INT_STATUS, IndexArray, out objValue);
                        }
                        else
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL104_EXTENDED_INT_STATUS, IndexArray, out objValue);
                        }
                        byaStatus = (byte[])objValue;

                        LastIntervalStatus = (VMStatusFlags)((byaStatus[1] << 8) | byaStatus[0]);

                        // Get the interval data
                        for (ushort usIntervalIndex = 0; usIntervalIndex < usNumIntervals; usIntervalIndex++)
                        {
                            List<float> fVhDataList = new List<float>();
                            List<float> fVminDataList = new List<float>();
                            List<float> fVmaxDataList = new List<float>();
                            TimeSpan tsTimeDifference = TimeSpan.FromMinutes((double)((usNumIntervals - usIntervalIndex - 1) * byIntervalLength));
                            DateTime dtIntervalEndTime = dtBlockEndTime - tsTimeDifference;
                            VMStatusFlags IntervalStatus;

                            IndexArray = new int[] { usActualBlockIndex, usIntervalIndex };

                            // Get the status - Kevin's code returns these as an array of bytes
                            if (true == blnExtendedVMInUse)
                            {
                                m_CenTables.GetValue(CentronTblEnum.MFGTBL109_EXTENDED_INT_STATUS, IndexArray, out objValue);
                            }
                            else
                            {
                                m_CenTables.GetValue(CentronTblEnum.MFGTBL104_EXTENDED_INT_STATUS, IndexArray, out objValue);
                            }
                            byaStatus = (byte[])objValue;

                            IntervalStatus = (VMStatusFlags)((byaStatus[1] << 8) | byaStatus[0]);

                            // Adjust the time if there is a difference in DST status
                            if ((IntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST
                                && (LastIntervalStatus & VMStatusFlags.DST) != VMStatusFlags.DST)
                            {
                                // We need to adjust forward an hour since the time we have has been
                                // adjusted backwards for DST
                                dtIntervalEndTime = dtIntervalEndTime.Add(new TimeSpan(1, 0, 0));
                            }
                            else if ((IntervalStatus & VMStatusFlags.DST) != VMStatusFlags.DST
                                && (LastIntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST)
                            {
                                // We need to adjust back an hour since the time we have has been
                                // adjusted forward for DST
                                dtIntervalEndTime = dtIntervalEndTime.Subtract(new TimeSpan(1, 0, 0));
                            }

                            // Get the values
                            for (byte byPhaseIndex = 0; byPhaseIndex < byNumPhases; byPhaseIndex++)
                            {
                                ushort usValue;

                                IndexArray = new int[] { usActualBlockIndex, usIntervalIndex, byPhaseIndex };

                                if (true == blnExtendedVMInUse)
                                {
                                    m_CenTables.GetValue(CentronTblEnum.MFGTBL109_VH_DATA_ITEM, IndexArray, out objValue);
                                }
                                else
                                {
                                    m_CenTables.GetValue(CentronTblEnum.MFGTBL104_VH_DATA_ITEM, IndexArray, out objValue);
                                }

                                usValue = (ushort)objValue;
                                fVhDataList.Add((float)usValue / (float)usDivisor);

                                if (true == blnExtendedVMInUse)
                                {
                                    m_CenTables.GetValue(CentronTblEnum.MFGTBL109_VMIN, IndexArray, out objValue);
                                    usValue = (ushort)objValue;
                                    fVminDataList.Add(((float)usValue * (float)usScalar) / (float)usDivisor);

                                    m_CenTables.GetValue(CentronTblEnum.MFGTBL109_VMAX, IndexArray, out objValue);
                                    usValue = (ushort)objValue;
                                    fVmaxDataList.Add(((float)usValue * (float)usScalar) / (float)usDivisor);
                                }
                            }

                            // The first interval of the DST change is always marked opposite of what we think so
                            // we need to go back and adjust that one if the previous DST status does not match the
                            // current DST status. The first check will prevent adjustments across blocks.
                            if (usIntervalIndex - 1 >= 0 && (IntervalStatus & VMStatusFlags.DST)
                               != (Intervals[Intervals.Count - 1].IntervalStatus & VMStatusFlags.DST))
                            {
                                VMInterval PreviousInterval = Intervals[Intervals.Count - 1];

                                if ((PreviousInterval.IntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST)
                                {
                                    // Interval was in DST so subtract an hour
                                    Intervals[Intervals.Count - 1] = new VMInterval(PreviousInterval.IntervalStatus, PreviousInterval.VhData,
                                        PreviousInterval.VminData, PreviousInterval.VmaxData, PreviousInterval.IntervalEndTime.Subtract(new TimeSpan(1, 0, 0)));
                                }
                                else
                                {
                                    // Interval was not in DST so add an hour
                                    Intervals[Intervals.Count - 1] = new VMInterval(PreviousInterval.IntervalStatus, PreviousInterval.VhData,
                                        PreviousInterval.VminData, PreviousInterval.VmaxData, PreviousInterval.IntervalEndTime.Add(new TimeSpan(1, 0, 0)));
                                }
                            }

                            Intervals.Add(new VMInterval(IntervalStatus, fVhDataList, fVminDataList, fVmaxDataList, dtIntervalEndTime));
                            OnStepProgress(new ProgressEventArgs());
                        }
                    }

                    VoltageData.Intervals = Intervals;
                }
            }

            OnHideProgress(new EventArgs());

            return VoltageData;
        }

        /// <summary>
        /// Returns a LoadProfile object built from the information in the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#        Description
        // -------- --- ------- ------------- ---------------------------------------
        // 10/30/06 RDB				          Created
        // 04/30/08 RCG 1.50.19 itron00114106 Fixing issues with wrapped Load Profile
        // 06/11/08 jrf 1.50.34               Switched to IndexOf() since Contains() is 
        //                                    not supported in the compact framework.
        // 09/23/16 jrf 4.70.19 WR442901      Handling special case when block end time 
        //                                    falls on a DST enter/exit time.
        protected virtual void GetLoadProfileData()
        {
            //used to store value returned from the CentronTables' GetValue method
            object objValue;

            //array given to GetValue that represents the block index, interval
            //index, and channel index of the value that is being accessed
            int[] aiBlockIntChannel = { 0, 0, 0 };

            //array given to GetValue that represents the block index of the value
            //that is being accessed
            int[] aiBlock = { 0 };

            DateTime dtIntEnd = new DateTime();
            DateTime dtBlockEnd;
            UInt16 iUsedBlocks;
            UInt16 iUsedIntervals;
            UInt16 iMaxBlocks;
            UInt16 iLastBlock;
            int iStartBlock;
            int iChannels;
            int iIntLength;
            UInt16 iScalar;
            UInt16 iDivisor;
            string strStatus;

            // read max number of blocks
            m_CenTables.GetValue(StdTableEnum.STDTBL61_NBR_BLKS_SET1, null, out objValue);
            iMaxBlocks = (UInt16)objValue;

            //read number of used blocks
            m_CenTables.GetValue(
                StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null, out objValue);
            iUsedBlocks = (UInt16)objValue;

            //read index of last block
            m_CenTables.GetValue(StdTableEnum.STDTBL63_LAST_BLOCK_ELEMENT, null, out objValue);
            iLastBlock = (UInt16)objValue;

            // Determine the first block in the table
            if (iUsedBlocks == iMaxBlocks)
            {
                // The circular list has rolled over so calculate the starting block
                iStartBlock = (iLastBlock + 1) % iMaxBlocks;
            }
            else
            {
                // The list has not rolled over so it is always zero
                iStartBlock = 0;
            }

            //read number of intervals in last block
            m_CenTables.GetValue(
                StdTableEnum.STDTBL63_NBR_VALID_INT, null, out objValue);
            iUsedIntervals = (UInt16)objValue;

            //read number of channels
            m_CenTables.GetValue(
                StdTableEnum.STDTBL61_NBR_CHNS_SET1, null, out objValue);
            iChannels = (int)((byte)objValue);

            //read length of one interval in minutes
            m_CenTables.GetValue(
                StdTableEnum.STDTBL61_MAX_INT_TIME_SET1, null, out objValue);
            iIntLength = (int)((byte)objValue);

            double[] aobjItem = new double[iChannels];
            string[] astrStatus = new string[iChannels];
            string strIntervalStatus = "";

            //Setup Progress Event
            OnShowProgress(new ShowProgressEventArgs(1, (iUsedBlocks * 128), "", "Retrieving Profile Data..."));

            m_LoadProfile = new LoadProfilePulseData(iIntLength);
            m_LoadProfile.DataSetName = "Load Profile";

            //make channels and add them to the load profile
            for (int i = 0; i < iChannels; i++)
            {
                aiBlock[0] = i;
                astrStatus[i] = "";

                //get scalar and divisor to calculate pulse multiplier
                m_CenTables.GetValue(
                    StdTableEnum.STDTBL62_SCALARS_SET1,
                    aiBlock, out objValue);
                iScalar = (UInt16)objValue;

                m_CenTables.GetValue(
                    StdTableEnum.STDTBL62_DIVISOR_SET1,
                    aiBlock, out objValue);
                iDivisor = (UInt16)objValue;

                float fPulseWeight = (float)iScalar / (float)iDivisor;

                m_LoadProfile.AddChannel(LPQuantityList[i], fPulseWeight, 1.0f);
            }

            //go through each block of memory that is being used
            for (int iRelativeBlock = 0; iRelativeBlock < iUsedBlocks; iRelativeBlock++)
            {
                int iActualBlock = (iStartBlock + iRelativeBlock) % iMaxBlocks;
                int iNumIntervals = 0;
                bool LastBlockIntervalTimeInDST = false;
                bool BlockEndTimeInDST = false;
                byte ClosureStatus = 0;

                //read the end time of the last interval in the current block
                aiBlock[0] = iActualBlock;
                m_CenTables.GetValue(StdTableEnum.STDTBL64_BLK_END_TIME, aiBlock, out objValue);
                dtBlockEnd = (DateTime)objValue;

                m_CenTables.GetValue(StdTableEnum.STDTBL64_STATUS, new int[] { iActualBlock, 0 }, out objValue);
                ClosureStatus = (byte)objValue;

                BlockEndTimeInDST = ((LP_CLOSURE_STATUS_DST_MASK & ClosureStatus) == LP_CLOSURE_STATUS_DST_MASK);

                //we're not in the last used block so there are 128 intervals in it -
                //subtract 127 * the length of one interval to get the end of the 
                //first interval
                if (iActualBlock != iLastBlock)
                {
                    iNumIntervals = 128;
                }
                //we are in the last block and so there are only iUsedIntervals 
                //intervals in it
                else
                {
                    iNumIntervals = iUsedIntervals;
                }

                // Determine if the end time is in DST
                m_CenTables.GetValue(StdTableEnum.STDTBL64_EXTENDED_INT_STATUS,
                    new int[] { iActualBlock, iNumIntervals - 1, 0 }, out objValue);          


                //Can't use Contains() in compact framework
                if (-1 != GetChannelStatus((byte)objValue, 2).IndexOf("D", StringComparison.OrdinalIgnoreCase))
                {
                    LastBlockIntervalTimeInDST = true;
                }

                //Handle Special Case when block end time is on DST boundary
                if (false == BlockEndTimeInDST && true == LastBlockIntervalTimeInDST)
                {
                    //Block end time falls on Fall DST boundary. 
                    //Need to add an hour to this time so that all other 
                    //interval times that are in DST are calculated correctly.
                    dtBlockEnd = dtBlockEnd.Add(new TimeSpan(1, 0, 0));
                }
                else if (true == BlockEndTimeInDST && false == LastBlockIntervalTimeInDST)
                {
                    //Block end time falls on Spring DST Boundary. 
                    //Need to subtract an hour to this time so that all other 
                    //interval times that are not in DST are calculated correctly.
                    dtBlockEnd = dtBlockEnd.Subtract(new TimeSpan(1, 0, 0));
                }

                //for each interval in the block
                for (int interval = 0; interval < iNumIntervals; interval++)
                {
                    dtIntEnd = dtBlockEnd.AddMinutes(-1 * (iNumIntervals - interval - 1) * iIntLength);

                    //specify which block and interval the data should be taken from
                    aiBlockIntChannel[0] = iActualBlock;
                    aiBlockIntChannel[1] = interval;
                    aiBlockIntChannel[2] = 0;

                    //get the interval status
                    m_CenTables.GetValue(
                            StdTableEnum.STDTBL64_EXTENDED_INT_STATUS,
                            aiBlockIntChannel, out objValue);

                    if (objValue != null)
                    {
                        strIntervalStatus =
                            GetChannelStatus((byte)objValue, 2);

                        // Do not do time adjustments unless Meter is using DST
                        if (DSTEnabled)
                        {
                            //Can't use Contains() in compact framework
                            if (-1 != strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase) && LastBlockIntervalTimeInDST == false)
                            {
                                // We need to adjust forward an hour since the time we have has been
                                // adjusted backwards for DST
                                dtIntEnd = dtIntEnd.Add(new TimeSpan(1, 0, 0));
                            }
                            else if (-1 == strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase) && LastBlockIntervalTimeInDST == true)
                            {
                                // We need to adjust back an hour since the time we have has been
                                // adjusted forward for DST
                                dtIntEnd = dtIntEnd.Subtract(new TimeSpan(1, 0, 0));
                            }

                            //Check to see if we are in the last interval of the block.
                            if (interval == iNumIntervals - 1)
                            {
                                if (false == BlockEndTimeInDST && true == LastBlockIntervalTimeInDST)
                                {
                                    //Block end time falls on Fall DST boundary. 
                                    //So remember we previously added an hour to this time so all other 
                                    //intervals would have the right time. Now we need to subtract the hour 
                                    //back out so the final interval has the correct time.
                                    dtIntEnd = dtIntEnd.Subtract(new TimeSpan(1, 0, 0));
                                }
                                else if (true == BlockEndTimeInDST && false == LastBlockIntervalTimeInDST)
                                {
                                    //Block end time falls on Spring DST Boundary.
                                    //So remember we previously subtracted an hour to this time so all other 
                                    //intervals would have the right time. Now we need to add the hour 
                                    //back in so the final interval has the correct time.
                                    dtIntEnd = dtIntEnd.Add(new TimeSpan(1, 0, 0));
                                }
                            }
                        }
                    }

                    //get the interval data and status for each channel and put them
                    //in aiItem and aiStatus so that they can be put in the load
                    //profile object
                    for (aiBlockIntChannel[2] = 0;
                         aiBlockIntChannel[2] < iChannels; aiBlockIntChannel[2]++)
                    {
                        //get value for channel
                        m_CenTables.GetValue(StdTableEnum.STDTBL64_ITEM,
                                             aiBlockIntChannel, out objValue);
                        if (objValue != null)
                        {
                            aobjItem[aiBlockIntChannel[2]] =
                                Convert.ToDouble(objValue, CultureInfo.InvariantCulture);
                        }

                        int[] aiIndexArray = {aiBlockIntChannel[0],
                                aiBlockIntChannel[1],
                                aiBlockIntChannel[2]};
                        aiIndexArray[2] = ((aiBlockIntChannel[2] + 1) / 2);

                        //get status for channel
                        m_CenTables.GetValue(
                            StdTableEnum.STDTBL64_EXTENDED_INT_STATUS,
                            aiIndexArray, out objValue);

                        if (objValue != null)
                        {
                            strStatus = GetChannelStatus((byte)objValue,
                                aiBlockIntChannel[2] % 2);

                            astrStatus[aiBlockIntChannel[2]] = strStatus;

                            //Can't use Contains() in compact framework
                            if (-1 == strIntervalStatus.IndexOf(strStatus, StringComparison.OrdinalIgnoreCase))
                            {
                                strIntervalStatus += strStatus;
                            }
                        }
                    }

                    // Do not do time adjustments unless Meter is using DST
                    if (DSTEnabled)
                    {
                        // The first interval of the DST change is always marked opposite of what we think so
                        // we need to go back and adjust that one if the previous DST status does not match the
                        // current DST status. The first check will prevent adjustments across blocks.
                        //Can't use Contains() in compact framework, so using IndexOf()
                        if (interval - 1 >= 0 && ((-1 != strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))
                            != (-1 != m_LoadProfile.Intervals[m_LoadProfile.Intervals.Count - 1].IntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))))
                        {
                            LPInterval PreviousInterval = m_LoadProfile.Intervals[m_LoadProfile.Intervals.Count - 1];

                            if (-1 != PreviousInterval.IntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))
                            {
                                // Interval was in DST so subtract an hour
                                m_LoadProfile.Intervals.Remove(PreviousInterval);
                                m_LoadProfile.AddInterval(PreviousInterval.Data, PreviousInterval.ChannelStatuses, PreviousInterval.IntervalStatus,
                                    PreviousInterval.Time.Subtract(new TimeSpan(1, 0, 0)), PreviousInterval.DisplayScale);
                            }
                            else
                            {
                                // Interval was in DST so subtract an hour
                                m_LoadProfile.Intervals.Remove(PreviousInterval);
                                m_LoadProfile.AddInterval(PreviousInterval.Data, PreviousInterval.ChannelStatuses, PreviousInterval.IntervalStatus,
                                    PreviousInterval.Time.Add(new TimeSpan(1, 0, 0)), PreviousInterval.DisplayScale);
                            }
                        }
                    }

                    OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                    //add the interval to the end of the interval list in the load
                    //profile object
                    m_LoadProfile.AddInterval(aobjItem, astrStatus, strIntervalStatus, dtIntEnd, DisplayScaleOptions.UNITS);

                    aobjItem = new double[iChannels];
                    astrStatus = new string[iChannels];
                }
            }

            OnHideProgress(new EventArgs());

        }//LP

        /// <summary>
        /// Returns a LoadProfile object built from the information in the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#        Description
        // -------- --- ------- ------------- ---------------------------------------
        // 12/15/11 RCG	2.53.20			      Created

        protected virtual void GetExtendedLoadProfileData()
        {
            //used to store value returned from the CentronTables' GetValue method
            object objValue;

            //array given to GetValue that represents the block index, interval
            //index, and channel index of the value that is being accessed
            int[] aiBlockIntChannel = { 0, 0, 0 };

            //array given to GetValue that represents the block index of the value
            //that is being accessed
            int[] aiBlock = { 0 };

            DateTime dtIntEnd = new DateTime();
            DateTime dtBlockEnd;
            UInt16 iUsedBlocks;
            UInt16 iUsedIntervals;
            UInt16 iMaxBlocks;
            UInt16 iLastBlock;
            int iStartBlock;
            int iChannels;
            int iIntLength;
            UInt16 iScalar;
            UInt16 iDivisor;
            string strStatus;

            // read max number of blocks
            m_CenTables.GetValue(CentronTblEnum.MfgTbl361_NBR_BLKS_SET1, null, out objValue);
            iMaxBlocks = (UInt16)objValue;

            //read number of used blocks
            m_CenTables.GetValue(CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, null, out objValue);
            iUsedBlocks = (UInt16)objValue;

            //read index of last block
            m_CenTables.GetValue(CentronTblEnum.MfgTbl363_LAST_BLOCK_ELEMENT, null, out objValue);
            iLastBlock = (UInt16)objValue;

            // Determine the first block in the table
            if (iUsedBlocks == iMaxBlocks)
            {
                // The circular list has rolled over so calculate the starting block
                iStartBlock = (iLastBlock + 1) % iMaxBlocks;
            }
            else
            {
                // The list has not rolled over so it is always zero
                iStartBlock = 0;
            }

            // Read number of intervals in last block
            m_CenTables.GetValue(CentronTblEnum.MfgTbl363_NBR_VALID_INT, null, out objValue);
            iUsedIntervals = (UInt16)objValue;

            // Read number of channels
            m_CenTables.GetValue(CentronTblEnum.MfgTbl361_NBR_CHNS_SET1, null, out objValue);
            iChannels = (int)((byte)objValue);

            // Read length of one interval in minutes
            m_CenTables.GetValue(CentronTblEnum.MfgTbl361_MAX_INT_TIME_SET1, null, out objValue);
            iIntLength = (int)((byte)objValue);

            double[] aobjItem = new double[iChannels];
            string[] astrStatus = new string[iChannels];
            string strIntervalStatus = "";

            // Setup Progress Event
            OnShowProgress(new ShowProgressEventArgs(1, (iUsedBlocks * 128), "", "Retrieving Extended Load Profile Data..."));

            m_ExtendedLoadProfile = new LoadProfilePulseData(iIntLength);
            m_ExtendedLoadProfile.DataSetName = "Extended Load Profile";

            // Make channels and add them to the load profile
            for (int iChannelIndex = 0; iChannelIndex < iChannels; iChannelIndex++)
            {
                byte bySourceIndex;
                aiBlock[0] = iChannelIndex;
                astrStatus[iChannelIndex] = "";

                // Get scalar and divisor to calculate pulse multiplier
                m_CenTables.GetValue(CentronTblEnum.MfgTbl362_SCALARS_SET1, aiBlock, out objValue);
                iScalar = (UInt16)objValue;

                m_CenTables.GetValue(CentronTblEnum.MfgTbl362_DIVISOR_SET1, aiBlock, out objValue);
                iDivisor = (UInt16)objValue;

                m_CenTables.GetValue(CentronTblEnum.MfgTbl362_END_BLK_RDG_SOURCE_INDEX, aiBlock, out objValue);
                bySourceIndex = (byte)objValue;

                float fPulseWeight = (float)iScalar / (float)iDivisor;

                m_ExtendedLoadProfile.AddChannel(NonMetrologicalSourceNames[bySourceIndex], fPulseWeight, 1.0f);
            }

            // Go through each block of memory that is being used
            for (int iRelativeBlock = 0; iRelativeBlock < iUsedBlocks; iRelativeBlock++)
            {
                int iActualBlock = (iStartBlock + iRelativeBlock) % iMaxBlocks;
                int iNumIntervals = 0;
                bool bEndTimeInDST = false;

                // Read the end time of the last interval in the current block
                aiBlock[0] = iActualBlock;
                m_CenTables.GetValue(CentronTblEnum.MfgTbl364_BLK_END_TIME, aiBlock, out objValue);
                dtBlockEnd = (DateTime)objValue;

                if (iActualBlock != iLastBlock)
                {
                    // We're not in the last used block so there are 128 intervals in it -
                    // subtract 127 * the length of one interval to get the end of the first interval
                    iNumIntervals = 128;
                }
                else
                {
                    // We are in the last block and so there are only iUsedIntervals intervals in it
                    iNumIntervals = iUsedIntervals;
                }

                // Determine if the end time is in DST
                m_CenTables.GetValue(CentronTblEnum.MfgTbl364_EXTENDED_INT_STATUS, new int[] { iActualBlock, iNumIntervals - 1, 0 }, out objValue);

                // Can't use Contains() in compact framework
                if (-1 != GetChannelStatus((byte)objValue, 2).IndexOf("D", StringComparison.OrdinalIgnoreCase))
                {
                    bEndTimeInDST = true;
                }

                // For each interval in the block
                for (int interval = 0; interval < iNumIntervals; interval++)
                {
                    dtIntEnd = dtBlockEnd.AddMinutes(-1 * (iNumIntervals - interval - 1) * iIntLength);

                    // Specify which block and interval the data should be taken from
                    aiBlockIntChannel[0] = iActualBlock;
                    aiBlockIntChannel[1] = interval;
                    aiBlockIntChannel[2] = 0;

                    // Get the interval status
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl364_EXTENDED_INT_STATUS, aiBlockIntChannel, out objValue);

                    if (objValue != null)
                    {
                        strIntervalStatus = GetChannelStatus((byte)objValue, 2);

                        // Do not do time adjustments unless Meter is using DST
                        if (DSTEnabled)
                        {
                            // Can't use Contains() in compact framework
                            if (-1 != strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase) && bEndTimeInDST == false)
                            {
                                // We need to adjust forward an hour since the time we have has been adjusted backwards for DST
                                dtIntEnd = dtIntEnd.Add(new TimeSpan(1, 0, 0));
                            }
                            else if (-1 == strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase) && bEndTimeInDST == true)
                            {
                                // We need to adjust back an hour since the time we have has been adjusted forward for DST
                                dtIntEnd = dtIntEnd.Subtract(new TimeSpan(1, 0, 0));
                            }
                        }
                    }

                    // Get the interval data and status for each channel and put them
                    // in aiItem and aiStatus so that they can be put in the load profile object
                    for (aiBlockIntChannel[2] = 0; aiBlockIntChannel[2] < iChannels; aiBlockIntChannel[2]++)
                    {
                        //get value for channel
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl364_ITEM, aiBlockIntChannel, out objValue);

                        if (objValue != null)
                        {
                            aobjItem[aiBlockIntChannel[2]] = Convert.ToDouble(objValue, CultureInfo.InvariantCulture);
                        }

                        int[] aiIndexArray = { aiBlockIntChannel[0], aiBlockIntChannel[1], aiBlockIntChannel[2] };
                        aiIndexArray[2] = ((aiBlockIntChannel[2] + 1) / 2);

                        //get status for channel
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl364_EXTENDED_INT_STATUS, aiIndexArray, out objValue);

                        if (objValue != null)
                        {
                            strStatus = GetChannelStatus((byte)objValue, aiBlockIntChannel[2] % 2);

                            astrStatus[aiBlockIntChannel[2]] = strStatus;

                            for (int iCharIndex = 0; iCharIndex < strStatus.Length; iCharIndex++)
                            {
                                //Can't use Contains() in compact framework
                                if (-1 == strIntervalStatus.IndexOf(strStatus[iCharIndex]))
                                {
                                    strIntervalStatus += strStatus[iCharIndex];
                                }
                            }
                        }
                    }

                    // Do not do time adjustments unless Meter is using DST
                    if (DSTEnabled)
                    {
                        // The first interval of the DST change is always marked opposite of what we think so
                        // we need to go back and adjust that one if the previous DST status does not match the
                        // current DST status. The first check will prevent adjustments across blocks.
                        //Can't use Contains() in compact framework, so using IndexOf()
                        if (interval - 1 >= 0 && ((-1 != strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))
                            != (-1 != m_ExtendedLoadProfile.Intervals[m_ExtendedLoadProfile.Intervals.Count - 1].IntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))))
                        {
                            LPInterval PreviousInterval = m_ExtendedLoadProfile.Intervals[m_ExtendedLoadProfile.Intervals.Count - 1];

                            if (-1 != PreviousInterval.IntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))
                            {
                                // Interval was in DST so subtract an hour
                                m_ExtendedLoadProfile.Intervals.Remove(PreviousInterval);
                                m_ExtendedLoadProfile.AddInterval(PreviousInterval.Data, PreviousInterval.ChannelStatuses, PreviousInterval.IntervalStatus,
                                    PreviousInterval.Time.Subtract(new TimeSpan(1, 0, 0)), PreviousInterval.DisplayScale);
                            }
                            else
                            {
                                // Interval was in DST so subtract an hour
                                m_ExtendedLoadProfile.Intervals.Remove(PreviousInterval);
                                m_ExtendedLoadProfile.AddInterval(PreviousInterval.Data, PreviousInterval.ChannelStatuses, PreviousInterval.IntervalStatus,
                                    PreviousInterval.Time.Add(new TimeSpan(1, 0, 0)), PreviousInterval.DisplayScale);
                            }
                        }
                    }

                    OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                    //add the interval to the end of the interval list in the load profile object
                    m_ExtendedLoadProfile.AddInterval(aobjItem, astrStatus, strIntervalStatus, dtIntEnd, DisplayScaleOptions.UNITS);

                    aobjItem = new double[iChannels];
                    astrStatus = new string[iChannels];
                }
            }

            OnHideProgress(new EventArgs());
        }

        /// <summary>
        /// Returns a LoadProfile object built from the information in the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#        Description
        // -------- --- ------- ------------- ---------------------------------------
        // 12/15/11 RCG	2.53.20			      Created

        protected virtual void GetInstrumentationProfileData()
        {
            //used to store value returned from the CentronTables' GetValue method
            object objValue;

            //array given to GetValue that represents the block index, interval
            //index, and channel index of the value that is being accessed
            int[] aiBlockIntChannel = { 0, 0, 0 };

            //array given to GetValue that represents the block index of the value
            //that is being accessed
            int[] aiBlock = { 0 };
            byte SetIndex = 1;

            DateTime dtIntEnd = new DateTime();
            DateTime dtBlockEnd;
            UInt16 iUsedBlocks;
            UInt16 iUsedIntervals;
            UInt16 iMaxBlocks;
            UInt16 iLastBlock;
            int iStartBlock;
            int iChannels;
            int iIntLength;
            UInt16 iScalar;
            UInt16 iDivisor;
            string strStatus;

            // read max number of blocks
            m_CenTables.GetValue(CentronTblEnum.MfgTbl361_NBR_BLKS_SET2, null, out objValue);
            iMaxBlocks = (UInt16)objValue;

            //read number of used blocks
            m_CenTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_BLOCKS, SetIndex), null, out objValue);
            iUsedBlocks = (UInt16)objValue;

            //read index of last block
            m_CenTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_LAST_BLOCK_ELEMENT, SetIndex), null, out objValue);
            iLastBlock = (UInt16)objValue;

            // Determine the first block in the table
            if (iUsedBlocks == iMaxBlocks)
            {
                // The circular list has rolled over so calculate the starting block
                iStartBlock = (iLastBlock + 1) % iMaxBlocks;
            }
            else
            {
                // The list has not rolled over so it is always zero
                iStartBlock = 0;
            }

            // Read number of intervals in last block
            m_CenTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl363_NBR_VALID_INT, SetIndex), null, out objValue);
            iUsedIntervals = (UInt16)objValue;

            // Read number of channels
            m_CenTables.GetValue(CentronTblEnum.MfgTbl361_NBR_CHNS_SET2, null, out objValue);
            iChannels = (int)((byte)objValue);

            // Read length of one interval in minutes
            m_CenTables.GetValue(CentronTblEnum.MfgTbl361_MAX_INT_TIME_SET2, null, out objValue);
            iIntLength = (int)((byte)objValue);

            double[] aobjItem = new double[iChannels];
            string[] astrStatus = new string[iChannels];
            string strIntervalStatus = "";

            // Setup Progress Event
            OnShowProgress(new ShowProgressEventArgs(1, (iUsedBlocks * 128), "", "Retrieving Extended Load Profile Data..."));

            m_InstrumentationProfile = new LoadProfilePulseData(iIntLength);
            m_InstrumentationProfile.DataSetName = "Instrumentation Profile";

            // Make channels and add them to the load profile
            for (int iChannelIndex = 0; iChannelIndex < iChannels; iChannelIndex++)
            {
                byte bySourceIndex;
                aiBlock[0] = iChannelIndex;
                astrStatus[iChannelIndex] = "";

                // Get scalar and divisor to calculate pulse multiplier
                m_CenTables.GetValue(CentronTblEnum.MfgTbl362_SCALARS_SET2, aiBlock, out objValue);
                iScalar = (UInt16)objValue;

                m_CenTables.GetValue(CentronTblEnum.MfgTbl362_DIVISOR_SET2, aiBlock, out objValue);
                iDivisor = (UInt16)objValue;

                m_CenTables.GetValue(TableSet.ApplyArrayToElement((long)CentronTblEnum.MfgTbl362_END_BLK_RDG_SOURCE_INDEX, SetIndex), aiBlock, out objValue);
                bySourceIndex = (byte)objValue;

                float fPulseWeight = (float)iScalar / (float)iDivisor;

                m_InstrumentationProfile.AddChannel(NonMetrologicalSourceNames[bySourceIndex], fPulseWeight, 1.0f);
            }

            // Go through each block of memory that is being used
            for (int iRelativeBlock = 0; iRelativeBlock < iUsedBlocks; iRelativeBlock++)
            {
                int iActualBlock = (iStartBlock + iRelativeBlock) % iMaxBlocks;
                int iNumIntervals = 0;
                bool bEndTimeInDST = false;

                // Read the end time of the last interval in the current block
                aiBlock[0] = iActualBlock;
                m_CenTables.GetValue(CentronTblEnum.MfgTbl365_BLK_END_TIME, aiBlock, out objValue);
                dtBlockEnd = (DateTime)objValue;

                if (iActualBlock != iLastBlock)
                {
                    // We're not in the last used block so there are 128 intervals in it -
                    // subtract 127 * the length of one interval to get the end of the first interval
                    iNumIntervals = 128;
                }
                else
                {
                    // We are in the last block and so there are only iUsedIntervals intervals in it
                    iNumIntervals = iUsedIntervals;
                }

                // Determine if the end time is in DST
                m_CenTables.GetValue(CentronTblEnum.MfgTbl365_EXTENDED_INT_STATUS, new int[] { iActualBlock, iNumIntervals - 1, 0 }, out objValue);

                // Can't use Contains() in compact framework
                if (-1 != GetChannelStatus((byte)objValue, 2).IndexOf("D", StringComparison.OrdinalIgnoreCase))
                {
                    bEndTimeInDST = true;
                }

                // For each interval in the block
                for (int interval = 0; interval < iNumIntervals; interval++)
                {
                    dtIntEnd = dtBlockEnd.AddMinutes(-1 * (iNumIntervals - interval - 1) * iIntLength);

                    // Specify which block and interval the data should be taken from
                    aiBlockIntChannel[0] = iActualBlock;
                    aiBlockIntChannel[1] = interval;
                    aiBlockIntChannel[2] = 0;

                    // Get the interval status
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl365_EXTENDED_INT_STATUS, aiBlockIntChannel, out objValue);

                    if (objValue != null)
                    {
                        strIntervalStatus = GetChannelStatus((byte)objValue, 2);

                        // Do not do time adjustments unless Meter is using DST
                        if (DSTEnabled)
                        {
                            // Can't use Contains() in compact framework
                            if (-1 != strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase) && bEndTimeInDST == false)
                            {
                                // We need to adjust forward an hour since the time we have has been adjusted backwards for DST
                                dtIntEnd = dtIntEnd.Add(new TimeSpan(1, 0, 0));
                            }
                            else if (-1 == strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase) && bEndTimeInDST == true)
                            {
                                // We need to adjust back an hour since the time we have has been adjusted forward for DST
                                dtIntEnd = dtIntEnd.Subtract(new TimeSpan(1, 0, 0));
                            }
                        }
                    }

                    // Get the interval data and status for each channel and put them
                    // in aiItem and aiStatus so that they can be put in the load profile object
                    for (aiBlockIntChannel[2] = 0; aiBlockIntChannel[2] < iChannels; aiBlockIntChannel[2]++)
                    {
                        //get value for channel
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl365_ITEM, aiBlockIntChannel, out objValue);

                        if (objValue != null)
                        {
                            aobjItem[aiBlockIntChannel[2]] = Convert.ToDouble(objValue, CultureInfo.InvariantCulture);
                        }

                        int[] aiIndexArray = { aiBlockIntChannel[0], aiBlockIntChannel[1], aiBlockIntChannel[2] };
                        aiIndexArray[2] = ((aiBlockIntChannel[2] + 1) / 2);

                        //get status for channel
                        m_CenTables.GetValue(CentronTblEnum.MfgTbl365_EXTENDED_INT_STATUS, aiIndexArray, out objValue);

                        if (objValue != null)
                        {
                            strStatus = GetChannelStatus((byte)objValue, aiBlockIntChannel[2] % 2);

                            astrStatus[aiBlockIntChannel[2]] = strStatus;

                            //Can't use Contains() in compact framework
                            if (-1 == strIntervalStatus.IndexOf(strStatus, StringComparison.OrdinalIgnoreCase))
                            {
                                strIntervalStatus += strStatus;
                            }
                        }
                    }

                    // Do not do time adjustments unless Meter is using DST
                    if (DSTEnabled)
                    {
                        // The first interval of the DST change is always marked opposite of what we think so
                        // we need to go back and adjust that one if the previous DST status does not match the
                        // current DST status. The first check will prevent adjustments across blocks.
                        //Can't use Contains() in compact framework, so using IndexOf()
                        if (interval - 1 >= 0 && ((-1 != strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))
                            != (-1 != m_InstrumentationProfile.Intervals[m_InstrumentationProfile.Intervals.Count - 1].IntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))))
                        {
                            LPInterval PreviousInterval = m_InstrumentationProfile.Intervals[m_InstrumentationProfile.Intervals.Count - 1];

                            if (-1 != PreviousInterval.IntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))
                            {
                                // Interval was in DST so subtract an hour
                                m_InstrumentationProfile.Intervals.Remove(PreviousInterval);
                                m_InstrumentationProfile.AddInterval(PreviousInterval.Data, PreviousInterval.ChannelStatuses, PreviousInterval.IntervalStatus,
                                    PreviousInterval.Time.Subtract(new TimeSpan(1, 0, 0)), PreviousInterval.DisplayScale);
                            }
                            else
                            {
                                // Interval was in DST so subtract an hour
                                m_InstrumentationProfile.Intervals.Remove(PreviousInterval);
                                m_InstrumentationProfile.AddInterval(PreviousInterval.Data, PreviousInterval.ChannelStatuses, PreviousInterval.IntervalStatus,
                                    PreviousInterval.Time.Add(new TimeSpan(1, 0, 0)), PreviousInterval.DisplayScale);
                            }
                        }
                    }

                    OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                    //add the interval to the end of the interval list in the load profile object
                    m_InstrumentationProfile.AddInterval(aobjItem, astrStatus, strIntervalStatus, dtIntEnd, DisplayScaleOptions.UNITS);

                    aobjItem = new double[iChannels];
                    astrStatus = new string[iChannels];
                }
            }

            OnHideProgress(new EventArgs());
        }

        /// <summary>Reads the current register data from table 23</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/15/07 mcm 8.10.05  	    Created
        // 07/29/09 RCG 2.20.19 134394 Rewriting to use new standard tables and support coincidents

        protected virtual void GetCurrentRegisters()
        {
            m_Registers = new List<Quantity>();

            foreach (LID EnergyLID in EnergyConfigLIDs)
            {
                Quantity NewQuantity = null;
                int? EnergySelectionIndex = null;
                double Value;

                if (EnergyLID != null)
                {
                    EnergySelectionIndex = FindEnergySelectionIndex(EnergyLID);
                }

                if (EnergySelectionIndex != null)
                {
                    NewQuantity = new Quantity(EnergyLID.lidDescription);

                    // Add the energy data items
                    Value = Table23.CurrentRegisters.TotalDataBlock.Summations[(int)EnergySelectionIndex];
                    NewQuantity.TotalEnergy = new Measurement(Value, EnergyLID.lidDescription);
                    NewQuantity.TOUEnergy = new List<Measurement>();

                    for (int iRate = 0; iRate < Table21.NumberOfTiers; iRate++)
                    {
                        Value = Table23.CurrentRegisters.TierDataBlocks[iRate].Summations[(int)EnergySelectionIndex];
                        NewQuantity.TOUEnergy.Add(new Measurement(Value, GetTOUEnergyLID(EnergyLID, iRate).lidDescription));
                    }

                    m_Registers.Add(NewQuantity);
                }
            }

            foreach (LID DemandLID in DemandConfigLIDs)
            {
                Quantity DemandQuantity = null;
                int? DemandSelectionIndex = null;
                int? CoincidentSelectionIndex = null;
                double Value;
                DateTime TimeOfOccurance;

                if (DemandLID != null)
                {
                    DemandSelectionIndex = FindDemandSelectionIndex(DemandLID);
                    CoincidentSelectionIndex = FindCoincidentSelectionIndex(DemandLID);
                }

                if (DemandSelectionIndex != null)
                {
                    DemandQuantity = GetDemandQuantity(DemandLID);

                    LID CumDemandLID = GetCumDemandLID(DemandLID);
                    LID CCumDemandLID = GetCCumDemandLID(DemandLID);
                    DemandRecord CurrentDemandRecord = Table23.CurrentRegisters.TotalDataBlock.Demands[(int)DemandSelectionIndex];

                    // Add the demand data items
                    // The quantity object only supports 1 occurence so always use occurence 0
                    Value = CurrentDemandRecord.Demands[0];
                    TimeOfOccurance = CurrentDemandRecord.TimeOfOccurances[0];

                    DemandQuantity.TotalMaxDemand = new DemandMeasurement(Value, DemandLID.lidDescription);
                    DemandQuantity.TotalMaxDemand.TimeOfOccurrence = TimeOfOccurance;

                    Value = CurrentDemandRecord.Cum;
                    DemandQuantity.CummulativeDemand = new Measurement(Value, CumDemandLID.lidDescription);

                    Value = CurrentDemandRecord.CCum;
                    DemandQuantity.ContinuousCummulativeDemand = new Measurement(Value, CCumDemandLID.lidDescription);

                    // Add TOU rates
                    if (Table21.NumberOfTiers > 0)
                    {
                        DemandQuantity.TOUMaxDemand = new List<DemandMeasurement>();
                        DemandQuantity.TOUCummulativeDemand = new List<Measurement>();
                        DemandQuantity.TOUCCummulativeDemand = new List<Measurement>();

                        for (int iRate = 0; iRate < Table21.NumberOfTiers; iRate++)
                        {
                            CurrentDemandRecord = Table23.CurrentRegisters.TierDataBlocks[iRate].Demands[(int)DemandSelectionIndex];

                            Value = CurrentDemandRecord.Demands[0];
                            TimeOfOccurance = CurrentDemandRecord.TimeOfOccurances[0];

                            DemandQuantity.TOUMaxDemand.Add(new DemandMeasurement(Value, GetTOUDemandLid(DemandLID, iRate).lidDescription));
                            DemandQuantity.TOUMaxDemand[iRate].TimeOfOccurrence = TimeOfOccurance;

                            Value = CurrentDemandRecord.Cum;
                            DemandQuantity.TOUCummulativeDemand.Add(new Measurement(Value, GetTOUDemandLid(CumDemandLID, iRate).lidDescription));

                            Value = CurrentDemandRecord.CCum;
                            DemandQuantity.TOUCCummulativeDemand.Add(new Measurement(Value, GetTOUDemandLid(CCumDemandLID, iRate).lidDescription));
                        }
                    }
                }

                if (CoincidentSelectionIndex != null)
                {
                    byte bySelection = Table22.CoincidentSelection[(int)CoincidentSelectionIndex];
                    byte byDemandSelection = Table22.CoincidentDemandAssocations[(int)CoincidentSelectionIndex];
                    Quantity CoincQuantity = new Quantity(DemandLID.lidDescription);

                    // Add the total values
                    CoincQuantity.TotalMaxDemand = new DemandMeasurement(Table23.CurrentRegisters.TotalDataBlock.Coincidents[(int)CoincidentSelectionIndex].Coincidents[0], DemandLID.lidDescription);
                    CoincQuantity.TotalMaxDemand.TimeOfOccurrence = Table23.CurrentRegisters.TotalDataBlock.Demands[byDemandSelection].TimeOfOccurances[0];

                    if (Table21.NumberOfTiers > 0)
                    {
                        CoincQuantity.TOUMaxDemand = new List<DemandMeasurement>();

                        // Add the rate values
                        for (int iRateIndex = 0; iRateIndex < Table21.NumberOfTiers; iRateIndex++)
                        {
                            LID RateLID = GetCoincLIDForRate(DemandLID, iRateIndex);

                            CoincQuantity.TOUMaxDemand.Add(new DemandMeasurement(Table23.CurrentRegisters.TierDataBlocks[iRateIndex].Coincidents[(int)CoincidentSelectionIndex].Coincidents[0], RateLID.lidDescription));
                            CoincQuantity.TOUMaxDemand[iRateIndex].TimeOfOccurrence = Table23.CurrentRegisters.TierDataBlocks[iRateIndex].Demands[byDemandSelection].TimeOfOccurances[0];
                        }
                    }

                    m_Registers.Add(CoincQuantity);

                }
            }
        }

        /// <summary>
        /// Gets a LID value for the specified rate.
        /// </summary>
        /// <param name="originalLID">The original LID that should be changed.</param>
        /// <param name="iRate">The rate to change to (0 = A, 1 = B, etc)</param>
        /// <returns>The resulting LID</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected LID GetCoincLIDForRate(LID originalLID, int iRate)
        {
            uint uiNewLIDNumber;

            // Clear the current rate value;
            uiNewLIDNumber = (originalLID.lidValue & ~(uint)DefinedLIDs.TOU_Data.TOU_DATA_MASK) | TOU_RATES[iRate];

            return new CentronAMILID(uiNewLIDNumber);
        }

        /// <summary>
        /// Finds the Coincident Selection index for the specified coincident
        /// </summary>
        /// <param name="coincidentLID">The coincident to find.</param>
        /// <returns>The selection index of the coincident.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected int? FindCoincidentSelectionIndex(LID coincidentLID)
        {
            byte[] CoincidentSelections = Table22.CoincidentSelection;
            int? CoincidentSourceIndex = null;
            int? CoincidentSelectionIndex = null;

            // Find the source index first.
            CoincidentSourceIndex = FindCoincidentSourceIndex(coincidentLID);

            // Make sure the demand source is used
            if (CoincidentSourceIndex != null)
            {
                for (int iSelectionIndex = 0; iSelectionIndex < CoincidentSelections.Length; iSelectionIndex++)
                {
                    if (CoincidentSelections[iSelectionIndex] == CoincidentSourceIndex)
                    {
                        CoincidentSelectionIndex = iSelectionIndex;
                    }
                }
            }

            return CoincidentSelectionIndex;
        }

        /// <summary>
        /// Finds the Source Index for the coincident LID
        /// </summary>
        /// <param name="coincidentLID">The LID of the coincident to find</param>
        /// <returns>The Source Index of the quantity</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected int? FindCoincidentSourceIndex(LID coincidentLID)
        {
            uint[] SourceIDs = Table14.SourceIDs;
            int? SourceIndex = null;

            // Check to see if the quantity is supported.
            for (int iSourceIndex = 0; iSourceIndex < SourceIDs.Length; iSourceIndex++)
            {
                // Table 14 stores the raw LID values but we use secondary everywhere else so we need to convert.
                if (coincidentLID.lidValue == SourceIDs[iSourceIndex])
                {
                    SourceIndex = iSourceIndex;
                }
            }

            return SourceIndex;
        }

        /// <summary>
        /// Gets the Quantity object for the current demand.
        /// </summary>
        /// <param name="DemandLID">The demand LID object</param>
        /// <returns>The Quantity object for the corresponding Energy or a new Quantity object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected Quantity GetDemandQuantity(LID DemandLID)
        {
            Quantity DemandQuantity = null;

            uint EnergyLIDValue = (uint)DefinedLIDs.BaseLIDs.ENERGY_DATA | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA
                | (DemandLID.lidValue & (uint)DefinedLIDs.WhichOneEnergyDemand.WHICH_ONE_MASK);

            for (int iIndex = 0; iIndex < EnergyConfigLIDs.Count; iIndex++)
            {
                if (EnergyConfigLIDs[iIndex].lidValue == EnergyLIDValue)
                {
                    DemandQuantity = m_Registers[iIndex];
                }
            }

            if (DemandQuantity == null)
            {
                // We don't seem to have a Quantity object for this so far so create a new one
                // and add it to the register list.
                DemandQuantity = new Quantity(DemandLID.lidDescription);
                m_Registers.Add(DemandQuantity);
            }

            return DemandQuantity;
        }

        /// <summary>
        /// Gets the TOU LID for the specified demand and rate.
        /// </summary>
        /// <param name="demandLID">The base demand LID for the quantity.</param>
        /// <param name="rate">The TOU rate to get.</param>
        /// <returns>The LID for the demand.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        protected LID GetTOUDemandLid(LID demandLID, int rate)
        {
            uint LIDNumber = (demandLID.lidValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT)
                                | (uint)DefinedLIDs.BaseLIDs.TOU_DATA
                                | (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND
                                | TOU_RATES[rate];

            return new CentronAMILID(LIDNumber);
        }

        /// <summary>
        /// Finds the selection index of the specified demand.
        /// </summary>
        /// <param name="demandLID">The LID for the quantity to search for.</param>
        /// <returns>Null if the quantity is not supported or the selection index if supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected int? FindDemandSelectionIndex(LID demandLID)
        {
            byte[] DemandSelections = Table22.DemandSelections;
            int? DemandSourceIndex = null;
            int? DemandSelectionIndex = null;

            // Find the source index first.
            DemandSourceIndex = FindDemandSourceIndex(demandLID);

            // Make sure the demand source is used
            if (DemandSourceIndex != null)
            {
                for (int iSelectionIndex = 0; iSelectionIndex < DemandSelections.Length; iSelectionIndex++)
                {
                    if (DemandSelections[iSelectionIndex] == DemandSourceIndex)
                    {
                        DemandSelectionIndex = iSelectionIndex;
                    }
                }
            }

            return DemandSelectionIndex;
        }

        /// <summary>
        /// Finds the index of the source if supported by the meter.
        /// </summary>
        /// <param name="sourceLID">The quantity to search for.</param>
        /// <returns>Null if the quantity is not supported or the index into the source definition if supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected int? FindDemandSourceIndex(LID sourceLID)
        {
            uint[] SourceIDs = Table14.SourceIDs;
            int? SourceIndex = null;
            uint MaxDemand;
            uint MinDemand;

            // Check to see if the quantity is supported.
            for (int iSourceIndex = 0; iSourceIndex < SourceIDs.Length; iSourceIndex++)
            {
                MaxDemand = SourceIDs[iSourceIndex] | (uint)DefinedLIDs.Demand_Data.MAX_DEMAND | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA;
                MinDemand = SourceIDs[iSourceIndex] | (uint)DefinedLIDs.Demand_Data.MIN_DEMAND | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA;

                // Table 14 stores the raw LID values but we use secondary everywhere else so we need to convert.
                if (MaxDemand == sourceLID.lidValue || MinDemand == sourceLID.lidValue)
                {
                    SourceIndex = iSourceIndex;
                }
            }

            return SourceIndex;
        }

        /// <summary>
        /// Gets the Cumulative Demand LID for the specified base Demand.
        /// </summary>
        /// <param name="demandLID">The base demand LID.</param>
        /// <returns>The Cumulative demand LID.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected LID GetCumDemandLID(LID demandLID)
        {
            uint LIDNumber = (demandLID.lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK_OUT)
                | (uint)DefinedLIDs.Demand_Data.CUM_DEMAND;

            return new CentronAMILID(LIDNumber);
        }

        /// <summary>
        /// Gets the Continuously Cumulative Demand LID for the specified base Demand.
        /// </summary>
        /// <param name="demandLID">The base demand LID.</param>
        /// <returns>The Continuously Cumulative demand LID.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected LID GetCCumDemandLID(LID demandLID)
        {
            uint LIDNumber = (demandLID.lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK_OUT)
                | (uint)DefinedLIDs.Demand_Data.CONT_CUM_DEMAND;

            return new CentronAMILID(LIDNumber);
        }

        /// <summary>
        /// Gets the TOU LID for the specified energy and rate.
        /// </summary>
        /// <param name="energyLID">The base energy LID for the quantity</param>
        /// <param name="rate">The TOU rate to get.</param>
        /// <returns>The LID for the energy.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected LID GetTOUEnergyLID(LID energyLID, int rate)
        {
            uint LIDNumber = (energyLID.lidValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT)
                                | (uint)DefinedLIDs.BaseLIDs.TOU_DATA
                                | (uint)DefinedLIDs.TOU_Rate_Data.TOU_ENERGY
                                | TOU_RATES[rate];

            return new CentronAMILID(LIDNumber);
        }

        /// <summary>
        /// Finds the selection index of the specified energy.
        /// </summary>
        /// <param name="energyLID">The LID for the quantity to search for.</param>
        /// <returns>Null if the quantity is not supported or the selection index if supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected int? FindEnergySelectionIndex(LID energyLID)
        {
            byte[] SummationSelections = Table22.SummationSelections;
            int? EnergySourceIndex = null;
            int? EnergySelectionIndex = null;

            // Find the source index first.
            EnergySourceIndex = FindEnergySourceIndex(energyLID);

            // Now make sure the energy source is used.
            if (EnergySourceIndex != null)
            {
                for (int iSelectionIndex = 0; iSelectionIndex < SummationSelections.Length; iSelectionIndex++)
                {
                    if (SummationSelections[iSelectionIndex] == EnergySourceIndex)
                    {
                        EnergySelectionIndex = iSelectionIndex;
                    }
                }
            }

            return EnergySelectionIndex;
        }

        /// <summary>
        /// Finds the index of the source if supported by the meter.
        /// </summary>
        /// <param name="sourceLID">The quantity to search for.</param>
        /// <returns>Null if the quantity is not supported or the index into the source definition if supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected int? FindEnergySourceIndex(LID sourceLID)
        {
            uint[] SourceIDs = Table14.SourceIDs;
            int? SourceIndex = null;

            // Check to see if the quantity is supported.
            for (int iSourceIndex = 0; iSourceIndex < SourceIDs.Length; iSourceIndex++)
            {
                // Table 14 stores the raw LID values but we use secondary everywhere else so we need to convert.
                if ((SourceIDs[iSourceIndex] | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA) == sourceLID.lidValue)
                {
                    SourceIndex = iSourceIndex;
                }
            }

            return SourceIndex;
        }

        /// <summary>
        /// Reads the current TOU Calendar data from table 54
        /// </summary>
        /// <remarks>
        /// TODO - Get the GetComment() working and break this into several
        /// functions.
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/30/08 AF  10.0           Created for OpenWay
        //
        protected virtual void GetTOUCalendar()
        {
            object Value;
            string strComment;

            bool blnSeparateSumDemandsFlag;
            bool blnAnchorDateFlag;
            bool blnSeparateWeekdaysFlag;

            byte byNbrNonRecurringDates;
            byte byNbrRecurringDates;
            UInt16 usNbrTierSwitches;
            byte byNbrSeasons;
            byte byNbrSpecialSchedules;

            //array given to GetValue that represents the block index of the value
            //that is being accessed
            int[] aiIndex = { 0 };
            byte[] aVal = { 0 };

            int iIndex;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_SEPARATE_SUM_DEMANDS_FLAG,
                null, out Value);
            blnSeparateSumDemandsFlag = (bool)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_ANCHOR_DATE_FLAG,
                null, out Value);
            blnAnchorDateFlag = (bool)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_SEPARATE_WEEKDAYS_FLAG,
                null, out Value);
            blnSeparateWeekdaysFlag = (bool)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_NON_RECURR_DATES,
                null, out Value);
            byNbrNonRecurringDates = (byte)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_RECURR_DATES,
                null, out Value);
            byNbrRecurringDates = (byte)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_SEASONS,
                null, out Value);
            byNbrSeasons = (byte)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_SPECIAL_SCHED,
                null, out Value);
            byNbrSpecialSchedules = (byte)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_TIER_SWITCHES,
                null, out Value);
            usNbrTierSwitches = (UInt16)Value;

            m_Calendar = new C1219_CalendarRcd(blnSeparateSumDemandsFlag,
                blnAnchorDateFlag, byNbrNonRecurringDates, byNbrRecurringDates,
                usNbrTierSwitches, blnSeparateWeekdaysFlag, byNbrSeasons,
                byNbrSpecialSchedules);

            if (null != m_Calendar)
            {
                if (blnAnchorDateFlag)
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL54_ANCHOR_DATE,
                        null, out Value);
                    m_Calendar.AnchorDate = (DateTime)Value;
                }

                for (iIndex = 0; iIndex < m_Calendar.NbrNonRecurringDates; iIndex++)
                {
                    aiIndex[0] = iIndex;

                    //TODO - this doesn't work - there are comments in the EDL file that give a
                    //title to the date.  It would be nice to be able to get it.
                    strComment = m_CenTables.GetComment((long)StdTableEnum.STDTBL54_NON_RECURR_DATE, aiIndex);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].Comment = strComment;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NON_RECURR_DATE,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].NonRecurrDate = (DateTime)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NON_RECURR_CALENDAR_CTRL,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].CalendarControl = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NON_RECURR_DEMAND_RESET_FLAG,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].DemandResetFlag = (bool)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NON_RECURR_SELF_READ_FLAG,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].SelfReadFlag = (bool)Value;
                }

                for (iIndex = 0; iIndex < m_Calendar.NbrRecurringDates; iIndex++)
                {
                    aiIndex[0] = iIndex;
                    m_CenTables.GetValue(StdTableEnum.STDTBL54_RECURR_DATE,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.RecurringDates[iIndex].RecurrDateBfld = (UInt16)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_RECURR_CALENDAR_CTRL,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.RecurringDates[iIndex].CalendarControl = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_RECURR_DEMAND_RESET_FLAG,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.RecurringDates[iIndex].DemandResetFlag = (bool)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_RECURR_SELF_READ_FLAG,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.RecurringDates[iIndex].SelfReadFlag = (bool)Value;
                }

                for (iIndex = 0; iIndex < m_Calendar.NbrTierSwitches; iIndex++)
                {
                    aiIndex[0] = iIndex;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NEW_TIER,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.TierSwitches[iIndex].NewTier = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_TIER_SWITCH_MIN,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.TierSwitches[iIndex].SwitchMinute = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_TIER_SWITCH_HOUR,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.TierSwitches[iIndex].SwitchHour = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_DAY_SCH_NUM,
                        aiIndex, out Value);
                    m_Calendar.CalendarRecord.TierSwitches[iIndex].DaySchedNum = (byte)Value;
                }

                if (blnSeparateWeekdaysFlag)
                {
                    //array size: # of seasons X (7 day types + number of special schedules)
                    for (iIndex = 0; iIndex < m_Calendar.NbrSeasons; iIndex++)
                    {
                        aiIndex[0] = iIndex;
                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_SUNDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 0] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_MONDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 1] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_TUESDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 2] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_WEDNESDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 3] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_THURSDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 4] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_FRIDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 5] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_SATURDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 6] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_SPECIAL_SCHEDULE,
                            aiIndex, out Value);
                        aVal = (byte[])Value;
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 7] = aVal[0];
                    }
                }
                else
                {
                    //array size: # of seasons X (3 day types + number of special schedules)
                    for (iIndex = 0; iIndex < m_Calendar.NbrSeasons; iIndex++)
                    {
                        aiIndex[0] = iIndex;
                        //strComment = m_CenTables.GetComment((long)StdTableEnum.STDTBL54_DAILY_SCHEDULE_ID_MATRIX, aiIndex);

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_WEEKDAY_SATURDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 0] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_WEEKDAY_SUNDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 1] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_WEEKDAY_WEEKDAY_SCHEDULE,
                            aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 2] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_WEEKDAY_SPECIAL_SCHEDULE,
                            aiIndex, out Value);
                        aVal = (byte[])Value;
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 3] = aVal[0];
                    }
                }
            }
        }

        /// <summary>
        /// This method reads standard table 3 and retrieves the fatal and non-fatal
        /// errors occurring in the device from it.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 jrf 2.10.02 125997 Created
        //  02/19/13 jrf 2.70.68 288152 Updated logic for getting the error list.
        //  02/21/13 jrf 2.70.68 288152 Not setting any fatal device errors when in 
        //                              fatal error recovery.
        //  04/18/13 jrf 2.70.74 288152 Correcting logic for determing when Fatal 7 is present.
        //
        protected virtual void GetErrorList()
        {
            m_astrErrorList = null;
            List<string> lstErrors = new List<string>();

            if (null != Table3)
            {
                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) >= 0)
                {
                    lstErrors.AddRange(Table3.NonFatalErrorsList);

                    //if in fatal error recovery mode then meter will not report any fatal errors
                    //in the device errors.
                    if (false == IsInFatalErrorRecoveryMode)
                    {
                        //If we have it use last fatal error data to make fatal errors reported 
                        //more accurate.
                        if (LastFatalErrorData != null)
                        {
                            FatalErrors Table3FatalErrors = Table3.FatalErrorsSet;

                            if ((Table3FatalErrors & FatalErrors.FatalError1) == FatalErrors.FatalError1)
                            {
                                lstErrors.Add(Properties.Resources.FATAL_1);
                            }

                            if ((Table3FatalErrors & FatalErrors.FatalError2) == FatalErrors.FatalError2)
                            {
                                lstErrors.Add(Properties.Resources.FATAL_2);
                            }

                            if ((Table3FatalErrors & FatalErrors.FatalError3) == FatalErrors.FatalError3)
                            {
                                //This could mean that fatal error 3 and/or 5 is set. What to do?
                                //Look at last fatal error data to break the tie.
                                if ((LastFatalErrorData.Error & FatalErrors.FatalError3) == FatalErrors.FatalError3)
                                {
                                    lstErrors.Add(Properties.Resources.FATAL_3);
                                }

                                if ((LastFatalErrorData.Error & FatalErrors.FatalError5) == FatalErrors.FatalError5)
                                {
                                    lstErrors.Add(Properties.Resources.FATAL_5);
                                }
                            }

                            if ((Table3FatalErrors & FatalErrors.FatalError4) == FatalErrors.FatalError4)
                            {
                                lstErrors.Add(Properties.Resources.FATAL_4);
                            }

                            //What about fatal error 5?  Check comments for fatal error 3.

                            if ((Table3FatalErrors & FatalErrors.FatalError6) == FatalErrors.FatalError6)
                            {
                                lstErrors.Add(Properties.Resources.FATAL_6);
                            }

                            //Table 3's Fatal 7 flag really just indicates core dump is available. 
                            //But if it is the only fatal flag that's set then there must be a fatal 7
                            if (Table3FatalErrors == FatalErrors.FatalError7
                                //Otherwise only way to tell if fatal 7 is set is to look at last fatal error data.
                                || (((Table3FatalErrors & FatalErrors.FatalError7) == FatalErrors.FatalError7)
                                    && (LastFatalErrorData.Error & FatalErrors.FatalError7) == FatalErrors.FatalError7))
                            {
                                lstErrors.Add(Properties.Resources.FATAL_7);
                            }

                        }
                        else
                        {
                            lstErrors.AddRange(Table3.FatalErrorsList);
                        }
                    }
                }
                else
                {
                    lstErrors = new List<string>(Table3.ErrorsList);
                }
            }

            m_astrErrorList = lstErrors.ToArray();
        }

        /// <summary>
        /// Determine the Nominal Voltage. This should only be used for ITRN / ITR1 devices.
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??                          Created
        //  05/26/11 AF  2.50.48 174218 Apparently some bases support a nominal voltage of 480
        //
        protected ushort DetermineNominalVoltage()
        {
            ushort usNominal = 240;

            if (VMRMSLowThreshold < 480 && VMRMSHighThreshold > 480)
            {
                usNominal = 480;
            }

            if (VMRMSLowThreshold < 240 && VMRMSHighThreshold > 240)
            {
                usNominal = 240;
            }

            if (VMRMSLowThreshold < 120 && VMRMSHighThreshold > 120)
            {
                usNominal = 120;
            }

            return usNominal;
        }

        /// <summary>
        /// This method converts from the internally stored daily self read minute to the actual
        /// minute value.
        /// </summary>
        /// <param name="bytInternalDSRMinute">The internally stored daily self read minute.</param>
        /// <returns>The actual daily self read minute.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created
        //
        protected byte ConvertFromInternalDailySelfReadMinute(byte bytInternalDSRMinute)
        {
            byte bytActualDSRMinute = 0;

            switch ((DSRTMinutes)(bytInternalDSRMinute))
            {
                case DSRTMinutes.MIN_0:
                    {
                        break;
                    }
                case DSRTMinutes.MIN_10:
                    {
                        bytActualDSRMinute = 10;
                        break;
                    }
                case DSRTMinutes.MIN_15:
                    {
                        bytActualDSRMinute = 15;
                        break;
                    }
                case DSRTMinutes.MIN_20:
                    {
                        bytActualDSRMinute = 20;
                        break;
                    }
                case DSRTMinutes.MIN_30:
                    {
                        bytActualDSRMinute = 30;
                        break;
                    }
                case DSRTMinutes.MIN_40:
                    {
                        bytActualDSRMinute = 40;
                        break;
                    }
                case DSRTMinutes.MIN_45:
                    {
                        bytActualDSRMinute = 45;
                        break;
                    }
                case DSRTMinutes.MIN_50:
                    {
                        bytActualDSRMinute = 50;
                        break;
                    }
            }

            return bytActualDSRMinute;
        }

        /// <summary>
        /// This method converts from the actual daily self read minute to the internally
        /// stored minute value.
        /// </summary>
        /// <param name="bytActualDSRMinute">The actual daily self read minute.</param>
        /// <returns>The internally stored daily self read minute.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created
        //
        protected byte ConvertToInternalDailySelfReadMinute(byte bytActualDSRMinute)
        {
            byte bytInternalDSRMinute = 0;

            switch (bytActualDSRMinute)
            {
                case 0:
                    {
                        break;
                    }
                case 10:
                    {
                        bytInternalDSRMinute = (byte)DSRTMinutes.MIN_10;
                        break;
                    }
                case 15:
                    {
                        bytInternalDSRMinute = (byte)DSRTMinutes.MIN_15;
                        break;
                    }
                case 20:
                    {
                        bytInternalDSRMinute = (byte)DSRTMinutes.MIN_20;
                        break;
                    }
                case 30:
                    {
                        bytInternalDSRMinute = (byte)DSRTMinutes.MIN_30;
                        break;
                    }
                case 40:
                    {
                        bytInternalDSRMinute = (byte)DSRTMinutes.MIN_40;
                        break;
                    }
                case 45:
                    {
                        bytInternalDSRMinute = (byte)DSRTMinutes.MIN_45;
                        break;
                    }
                case 50:
                    {
                        bytInternalDSRMinute = (byte)DSRTMinutes.MIN_50;
                        break;
                    }
            }

            return bytInternalDSRMinute;
        }

        /// <summary>
        /// Determines the number of Demands thresholds that can be configured.
        /// </summary>
        /// <returns>The number of demand thresholds that can be configured.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/20/10 jrf 2.45.10        Created
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters

        protected int DetermineDemandThresholdCount()
        {
            int iCount = 0;

            switch (DeviceType)
            {
                case EDLDeviceTypes.OpenWayCentron:
                case EDLDeviceTypes.TransparentDevice:
                case EDLDeviceTypes.OpenWayCentronBasicPoly:
                case EDLDeviceTypes.OpenWayCentronAdvPoly:
                case EDLDeviceTypes.OpenWayCentronITRD:
                case EDLDeviceTypes.OpenWayCentronBasicPolyITRE:
                case EDLDeviceTypes.OpenWayCentronAdvPolyITRF:
                {
                    iCount = 0;
                    break;
                }
            }

            return iCount;
        }

        /// <summary>
        /// Determines the number of load profile channels that can be configured.
        /// </summary>
        /// <returns>The number of load profile that can be configured.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/05/10 jrf 2.45.11        Created
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        //  06/24/15 AF  4.20.14 593126 Added ITRK, which supports 5 channels
        //
        protected int DetermineMaximumLPChannels()
        {
            int iCount = 0;

            switch (DeviceType)
            {
                case EDLDeviceTypes.OpenWayCentron:
                case EDLDeviceTypes.OpenWayCentronAdvPoly:
                case EDLDeviceTypes.OpenWayCentronBasicPoly:
                case EDLDeviceTypes.TransparentDevice:
                case EDLDeviceTypes.OpenWayCentronITRD:
                case EDLDeviceTypes.OpenWayCentronBasicPolyITRE:
                case EDLDeviceTypes.OpenWayCentronAdvPolyITRF:
                {
                    iCount = 4;
                    break;
                }
                case EDLDeviceTypes.OpenWayCentronPolyITRK:
                {
                    iCount = 5;
                    break;
                }
            }

            return iCount;
        }

        /// <summary>
        /// Determines the number of energies that can be configured.
        /// </summary>
        /// <returns>The number of energies that can be configured.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        //  06/24/15 AF  4.20.14 593126 Added ITRK, which supports 5 quantities
        //
        protected int DetermineEnergyConfigCount()
        {
            int iCount = 0;

            switch (DeviceType)
            {
                case EDLDeviceTypes.OpenWayCentron:
                case EDLDeviceTypes.OpenWayCentronAdvPoly:
                case EDLDeviceTypes.OpenWayCentronBasicPoly:
                case EDLDeviceTypes.TransparentDevice:
                case EDLDeviceTypes.OpenWayCentronITRD:
                case EDLDeviceTypes.OpenWayCentronBasicPolyITRE:
                {
                    iCount = 4;
                    break;
                }
                case EDLDeviceTypes.OpenWayCentronAdvPolyITRF:
                case EDLDeviceTypes.OpenWayCentronPolyITRK:
                {
                    iCount = 5;
                    break;
                }
            }

            return iCount;
        }

        /// <summary>
        /// Determines the number of Demands that can be configured.
        /// </summary>
        /// <returns>The number of demands that can be configured.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        //  06/24/15 AF  4.20.14 593126 Added ITRK, which supports 3 quantities
        //
        protected int DetermineDemandConfigCount()
        {
            int iCount = 0;

            switch (DeviceType)
            {
                case EDLDeviceTypes.OpenWayCentron:
                case EDLDeviceTypes.TransparentDevice:
                case EDLDeviceTypes.OpenWayCentronBasicPoly:
                case EDLDeviceTypes.OpenWayCentronITRD:
                case EDLDeviceTypes.OpenWayCentronBasicPolyITRE:
                {
                    iCount = 1;
                    break;
                }
                case EDLDeviceTypes.OpenWayCentronAdvPoly:
                case EDLDeviceTypes.OpenWayCentronAdvPolyITRF:
                case EDLDeviceTypes.OpenWayCentronPolyITRK:
                {
                    iCount = 3;
                    break;
                }
            }

            return iCount;
        }

        /// <summary>
        /// Gets the Self Read values for Neutral Amps
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRAmpsNeutral(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_AH_NEUTRAL, LIDDefinitions.DEMAND_MAX_A_NEUTRAL,
                    "Neutral Amps", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Amps (a)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRAmpsPhaseA(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_AH_PHA, LIDDefinitions.DEMAND_MAX_A_PHA,
                    "Amps (a)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Amps (b)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRAmpsPhaseB(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_AH_PHB, LIDDefinitions.DEMAND_MAX_A_PHB,
                    "Amps (b)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Amps (c)
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRAmpsPhaseC(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_AH_PHC, LIDDefinitions.DEMAND_MAX_A_PHC,
                    "Amps (c)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Amps Squared
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRAmpsSquared(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_I2H_AGG, LIDDefinitions.DEMAND_MAX_I2_AGG,
                    "Amps Squared", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Power Factor
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRPowerFactor(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                // There is not PF energy so just check the demand
                SRQuantity = GetQuantityFromStandardTables(null, LIDDefinitions.DEMAND_MIN_PF_INTERVAL_ARITH,
                    "Power Factor", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);

                // Also try the vertoral PF
                if (SRQuantity == null)
                {
                    SRQuantity = GetQuantityFromStandardTables(null, LIDDefinitions.DEMAND_MIN_PF_INTERVAL_VECT,
                        "Power Factor", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
                }
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Q Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRQDelivered(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_QH_DEL, LIDDefinitions.DEMAND_MAX_Q_DEL,
                    "Q Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Q Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRQReceived(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_QH_REC, LIDDefinitions.DEMAND_MAX_Q_REC,
                    "Q Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for VA Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVADelivered(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                // Try getting Arithmatic first.
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_DEL_ARITH, LIDDefinitions.DEMAND_MAX_VA_DEL_ARITH,
                    "VA Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);

                // Try  getting Vectoral
                if (SRQuantity == null)
                {
                    SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_DEL_VECT, LIDDefinitions.DEMAND_MAX_VA_DEL_VECT,
                        "VA Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
                }
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for VA Lagging
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVALagging(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_LAG, LIDDefinitions.DEMAND_MAX_VA_LAG,
                    "VA Lagging", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarDelivered(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_DEL, LIDDefinitions.DEMAND_MAX_VAR_DEL,
                    "Var Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for VA Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVAReceived(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                // Try getting Arithmatic first.
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_REC_ARITH, LIDDefinitions.DEMAND_MAX_VA_REC_ARITH,
                    "VA Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);

                // Try  getting Vectoral
                if (SRQuantity == null)
                {
                    SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VAH_REC_VECT, LIDDefinitions.DEMAND_MAX_VA_REC_VECT,
                        "VA Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
                }
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Net.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarNet(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_NET, LIDDefinitions.DEMAND_MAX_VAR_NET,
                    "Var Net", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Net Delivered
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarNetDelivered(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_NET_DEL, LIDDefinitions.DEMAND_MAX_VAR_NET_DEL,
                    "Var Net Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Net Received
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarNetReceived(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_NET_REC, LIDDefinitions.DEMAND_MAX_VAR_NET_REC,
                    "Var Net Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Q1.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarQuadrant1(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_Q1, LIDDefinitions.DEMAND_MAX_VAR_Q1,
                    "Var Quadrant 1", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Q2.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarQuadrant2(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_Q2, LIDDefinitions.DEMAND_MAX_VAR_Q2,
                    "Var Quadrant 2", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Q3.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarQuadrant3(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_Q3, LIDDefinitions.DEMAND_MAX_VAR_Q3,
                    "Var Quadrant 3", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Q4.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarQuadrant4(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_Q4, LIDDefinitions.DEMAND_MAX_VAR_Q4,
                    "Var Quadrant 4", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Var Received.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVarReceived(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VARH_REC, LIDDefinitions.DEMAND_MAX_VAR_REC,
                    "Var Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts Average.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVoltsAverage(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VH_AVG, LIDDefinitions.DEMAND_MAX_V_AVG,
                    "Volts Average", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts (a).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVoltsPhaseA(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VH_PHA, LIDDefinitions.DEMAND_MAX_V_PHA,
                    "Volts (a)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts(b).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVoltsPhaseB(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VH_PHB, LIDDefinitions.DEMAND_MAX_V_PHB,
                    "Volts (b)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts (c).
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVoltsPhaseC(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_VH_PHC, LIDDefinitions.DEMAND_MAX_V_PHC,
                    "Volts (c)", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Volts Squared.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRVoltsSquared(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_V2H_AGG, LIDDefinitions.DEMAND_MAX_V2_AGG,
                    "Volts Squared", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Watts Delivered.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRWattsDelivered(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_WH_DEL, LIDDefinitions.DEMAND_MAX_W_DEL,
                    "Watts Delivered", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Watts Net.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRWattsNet(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_WH_NET, LIDDefinitions.DEMAND_MAX_W_NET,
                    "Watts Net", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Watts Received.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRWattsReceived(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_WH_REC, LIDDefinitions.DEMAND_MAX_W_REC,
                    "Watts Received", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read values for Unidirectional Watts.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The self read quantity.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected Quantity SRWattsUni(uint uiIndex)
        {
            Quantity SRQuantity = null;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRQuantity = GetQuantityFromStandardTables(LIDDefinitions.ENERGY_WH_UNI, LIDDefinitions.DEMAND_MAX_W_UNI,
                    "Unidirectional Watts", Table26.SelfReadEntries[uiIndex].SelfReadRegisters);
            }

            return SRQuantity;
        }

        /// <summary>
        /// Gets the Self Read date for the specified index.
        /// </summary>
        /// <param name="uiIndex">The index of the self read to get.</param>
        /// <returns>The date of the Self Read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/08 RCG 2.00.00 N/A    Created

        protected DateTime DateTimeOfSelfRead(uint uiIndex)
        {
            DateTime SRDate = MeterReferenceTime;

            if (uiIndex >= 0 && uiIndex < Table26.NumberOfValidEntries)
            {
                SRDate = Table26.SelfReadEntries[uiIndex].SelfReadDate;
            }

            return SRDate;
        }

        /// <summary>
        /// Gets the Self Read Coincident Values for the specified Self Read.
        /// </summary>
        /// <param name="uiIndex">The index of the Self Read to get.</param>
        /// <returns>The coincident quantities</returns>
        /// 

        protected List<Quantity> SRCoincidentValues(uint uiIndex)
        {
            List<Quantity> Coincidents = new List<Quantity>();

            if (uiIndex < Table26.NumberOfValidEntries)
            {
                for (int iIndex = 0; iIndex < Table21.NumberOfCoincidentValues; iIndex++)
                {
                    byte bySelection = Table22.CoincidentSelection[iIndex];
                    byte byDemandSelection = Table22.CoincidentDemandAssocations[iIndex];
                    LID CoincidentLID = CreateLID(Table14.SourceIDs[bySelection]);
                    Quantity CoincQuantity = new Quantity(CoincidentLID.lidDescription);

                    RegisterDataRecord DataRecord = Table26.SelfReadEntries[uiIndex].SelfReadRegisters;

                    // Add the total values
                    CoincQuantity.TotalMaxDemand = new DemandMeasurement(DataRecord.TotalDataBlock.Coincidents[iIndex].Coincidents[0], CoincidentLID.lidDescription);
                    CoincQuantity.TotalMaxDemand.TimeOfOccurrence = DataRecord.TotalDataBlock.Demands[byDemandSelection].TimeOfOccurances[0];

                    if (Table21.NumberOfTiers > 0)
                    {
                        CoincQuantity.TOUMaxDemand = new List<DemandMeasurement>();

                        // Add the rate values
                        for (int iRateIndex = 0; iRateIndex < Table21.NumberOfTiers; iRateIndex++)
                        {
                            LID RateLID = GetDemandLIDForRate(CoincidentLID, iRateIndex);

                            CoincQuantity.TOUMaxDemand.Add(new DemandMeasurement(DataRecord.TierDataBlocks[iRateIndex].Coincidents[iIndex].Coincidents[0], RateLID.lidDescription));
                            CoincQuantity.TOUMaxDemand[iRateIndex].TimeOfOccurrence = DataRecord.TierDataBlocks[iRateIndex].Demands[byDemandSelection].TimeOfOccurances[0];
                        }
                    }

                    Coincidents.Add(CoincQuantity);
                }
            }

            return Coincidents;
        }

        /// <summary>
        /// Creates the LID object for specified LID number
        /// </summary>
        /// <param name="uiLIDNumber">The LID number to create</param>
        /// <returns>The object that represents the specified LID</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/21/11 RCG	2.45.24		   Allowing the LID type to be changed to fix issue with Ins VA and Var values
        // 12/17/15 PGH 4.50.222 577471 Changed protection level for use in EDL Viewer's Configuration view
        //                              and Shop Manager's Program Comparison form
        //
        public LID CreateLID(uint uiLIDNumber)
        {
            DefinedLIDs Defined = new DefinedLIDs();
            LID CreatedLID = new CentronAMILID(uiLIDNumber);

            // There is a case where the Data Type returned data type for the Instantaneous VA and Var
            // values returned a Uint32 rather than a Single so we need to make sure we interpret those
            // correctly
            if (Defined.INST_VA_ARITH.lidValue == uiLIDNumber
                || Defined.INST_VA_VECT.lidValue == uiLIDNumber
                || Defined.INST_VAR.lidValue == uiLIDNumber
                || Defined.INST_VA_LAG.lidValue == uiLIDNumber)
            {
                // Returned as a UINT between 3.0.140 and 3.7
                if ((VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) > 0
                    || (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_3) == 0 && FirmwareBuild >= 140))
                    && VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_HYDROGEN_3_7) < 0)
                {
                    CreatedLID.lidType = TypeCode.UInt32;
                }
            }

            return CreatedLID;
        }

        /// <summary>
        /// Sets the Description of Display Items that are not set or need to be something 
        /// other than what is returned from the LID.
        /// </summary>
        /// <param name="Item">The item to handle</param>
        /// <returns>True if the item was handled, false otherwise</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 02/28/13 jrf 2.70.72 323232 Created
        //
        protected bool HandleIrregularDescription(ANSIDisplayItem Item)
        {
            bool bResult = false;
            DefinedLIDs LIDDefinitions = new DefinedLIDs();
            LID DemandLID = null;

            //The previous max demand quantity LIDs do not specify which quantity,
            //so we have to tease out which quantity it is to provide a more meaningful description.
            if (LIDDefinitions.PREVIOUS_MAX_DEMAND_QTY_1 == Item.DisplayLID)
            {
                if (null != DemandConfigLIDs && 1 <= DemandConfigLIDs.Count)
                {
                    DemandLID = DemandConfigLIDs[0];
                }
            }
            else if (LIDDefinitions.PREVIOUS_MAX_DEMAND_QTY_2 == Item.DisplayLID)
            {
                if (null != DemandConfigLIDs && 2 <= DemandConfigLIDs.Count)
                {
                    DemandLID = DemandConfigLIDs[1];
                }
            }
            else if (LIDDefinitions.PREVIOUS_MAX_DEMAND_QTY_3 == Item.DisplayLID)
            {
                if (null != DemandConfigLIDs && 2 <= DemandConfigLIDs.Count)
                {
                    DemandLID = DemandConfigLIDs[2];
                }
            }

            if (null != DemandLID)
            {
                Item.Description = "previous " + DemandLID.lidDescription;
                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// This method handles creating and loading an XmlDocument.
        /// </summary>
        /// <param name="FileName">Path of the xml file.</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/12 jrf 2.70.15 TQ6710 Refactored the XMLDocument creation code
        //                             into this method.
        //
        protected static XmlDocument CreateXMLDocument(string FileName)
        {
            XmlDocument Document = null;

            // Make sure the file exists first
            if (File.Exists(FileName))
            {
                try
                {
                    // Load the Program
                    Document = new XmlDocument();
                    Document.PreserveWhitespace = true;
                    Document.Load(FileName);
                }
                catch (Exception)
                {
                    Document = null;
                }
            }

            return Document;
        }

        /// <summary>
        /// Returns true if the Xml document is an EDL file and false otherwise
        /// </summary>
        /// <param name="Document">A loaded Xml Document.</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/12 jrf 2.70.15 TQ6710 Refactored the EDL determination logic
        //                             into this method.
        // 05/02/14 AF  3.50.89 502841 Added additional filtering to exclude ITRU/V configuration files
        //
        protected static bool IsEDLFile(XmlDocument Document)
        {
            bool bIsEDLFile = false;

            try
            {
                if (null != Document)
                {
                    if (Document.DocumentElement.Name == "edl")
                    {
                        // The following will filter out ITRU and ITRV config files
                        XmlNodeList nodeList = Document.GetElementsByTagName("GEN_CONFIG_TBL");

                        if (nodeList.Count > 0)
                        {
                            bIsEDLFile = true;
                        }
                    }
                    else if (Document.DocumentElement.Name == "SignedData")
                    {
                        // We know it's a signed document now figure out if it's an edl file
                        foreach (XmlNode CurrentNode in Document.DocumentElement.ChildNodes)
                        {
                            if (CurrentNode.Name == "edl")
                            {
                                bIsEDLFile = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                bIsEDLFile = false;
            }

            return bIsEDLFile;
        }//IsEDLFile

        /// <summary>
        /// Reads the Time Of Use Schedule's calendar and TOU configurations from the EDL file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  11/18/13 jrf 3.50.06 TQ 9479   Created
        //  12/11/13 jrf 3.50.14 TQ 9479   Added call to method to generate the 25 year TOU calendar. 
        //  12/20/13 jrf 3.50.16 TQ 9562   Refactored check for 25 year TOU support into a property.
        //  01/03/14 jrf 3.50.19 TQ 9629   Removing unneeded check before reading 25 Year TOU.
        //  04/22/14 jrf 3.50.81 WR 490044 Modified to call method for creating the 25 year TOU calendar data
        //                                 from the config file, since the Supports25YearTOU property no longer
        //                                 does this.
        protected void ReadTOUConfiguration()
        {
            if (Supports25YearTOU)
            {
                m_CenTables.Create25YearCalendarFromStandardTables(DateTime.Now, true);
                m_CalendarConfig = Table2437.CalendarConfig;
                m_TOUConfig = Table2437.TOUConfig;
            }
            else
            {
                MemoryStream CalStream = new MemoryStream();
                MemoryStream TOUStream = new MemoryStream();

                // First determine if the TOU is in 2048
                if (ConfigHeader.CalendarOffset != 0 && ConfigHeader.TOUOffset != 0)
                {
                    m_CenTables.BuildPSEMStream(2048, CalStream, ConfigHeader.CalendarOffset, CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_SIZE);
                }

                if (CalStream.Length > 0)
                {
                    m_CenTables.BuildPSEMStream(2048, TOUStream, ConfigHeader.TOUOffset, CENTRON_AMI_TOUConfig.TOU_CONFIG_SIZE);

                    m_CalendarConfig = new CENTRON_AMI_CalendarConfig(new PSEMBinaryReader(CalStream), 0, CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_SIZE, CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_YEARS);
                    m_TOUConfig = new CENTRON_AMI_TOUConfig(new PSEMBinaryReader(TOUStream), 0);
                }
                else
                {
                    PSEMBinaryReader Reader;

                    // We must have a program file so we need to move it to 2090 first
                    UpdateTOUSchedule();

                    m_CenTables.BuildPSEMStream(2090, TOUStream);

                    Reader = new PSEMBinaryReader(TOUStream);

                    m_CalendarConfig = new CENTRON_AMI_CalendarConfig(Reader, 0, CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_SIZE, CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_YEARS);
                    m_TOUConfig = new CENTRON_AMI_TOUConfig(Reader, CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_SIZE);
                }
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the table 0 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created
        //  08/02/10 AF  2.42.19        Added M2 Gateway support

        protected virtual CTable00 Table0
        {
            get
            {
                if (m_Table00 == null)
                {
                    Stream TableStream = new MemoryStream();
                    if (m_CenTables.IsAllCached(0))
                    {
                        m_CenTables.BuildPSEMStream(0, TableStream);
                    }
#if (!WindowsCE)
                    else
                    {
                        m_GatewayTables.BuildPSEMStream(0, TableStream);
                    }
#endif
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table00 = new CTable00(Reader, (uint)TableStream.Length);
                }

                return m_Table00;
            }
        }

        /// <summary>
        /// Gets the table 3 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ ---------------------------------------------
        //  02/20/13 jrf 2.70.69 CQ 288152 Created
        //  01/20/14 AF  3.00.28 WR 457734 The M2 Gateway does not have a standard table 3,
        //                              so make sure Table3 returns null for the M2.
        //  07/27/15 jrf 4.20.18 WR 599965 Passing in parameter to determine how 
        //                                 many manufacturer status bytes there are
        //                                 since this is not constant.
        // 07/30/15 jrf 4.50.178 WR 599965 Per code review passing in table 0.
        protected virtual CTable03 Table3
        {
            get
            {
                if (m_Table03 == null)
                {
                    Stream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = null;

                    if (m_CenTables.IsTableKnown(3) && m_CenTables.IsAllCached(3))
                    {
                        m_CenTables.BuildPSEMStream(3, TableStream);
                        Reader = new PSEMBinaryReader(TableStream);

                        if (null != Reader)
                        {
                            m_Table03 = new CTable03(Reader, Table0);
                        }
                    }
                }

                return m_Table03;
            }
        }
        
        /// <summary>
        /// Gets the table 2091 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created
        //  10/24/12 jrf 2.70.33 238238 Adding passing in FW version to table for it to use
        //                              to determine its size.
        // 
        protected virtual OpenWayPolyMFGTable2091 Table2091
        {
            get
            {
                if (m_Table2091 == null && m_CenTables.IsAllCached(2091))
                {
                    Stream Table2091Stream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(2091, Table2091Stream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(Table2091Stream);

                    m_Table2091 = new OpenWayPolyMFGTable2091(Reader, FWRevision);
                }

                return m_Table2091;
            }
        }

        /// <summary>
        /// Gets the table 11 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable11 Table11
        {
            get
            {
                if (m_Table11 == null && m_CenTables.IsAllCached(11))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(11, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table11 = new StdTable11(Reader);
                }

                return m_Table11;
            }
        }

        /// <summary>
        /// Gets the table 14 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable14 Table14
        {
            get
            {
                if (m_Table14 == null && m_CenTables.IsAllCached(14))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(14, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table14 = new StdTable14(Reader, Table11);
                }

                return m_Table14;
            }
        }

        /// <summary>
        /// Gets the table 21 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable21 Table21
        {
            get
            {
                if (m_Table21 == null && m_CenTables.IsAllCached(21))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(21, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table21 = new StdTable21(Reader);
                }

                return m_Table21;
            }
        }

        /// <summary>
        /// Gets the table 22 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable22 Table22
        {
            get
            {
                if (m_Table22 == null && Table21 != null)
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(22, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table22 = new StdTable22(Reader, Table21);
                }

                return m_Table22;
            }
        }

        /// <summary>
        /// Gets the table 23 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable23 Table23
        {
            get
            {
                if (m_Table23 == null && m_CenTables.IsAllCached(23))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(23, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table23 = new StdTable23(Reader, Table0, Table21);
                }

                return m_Table23;
            }
        }

        /// <summary>
        /// Gets the table 24 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable24 Table24
        {
            get
            {
                if (m_Table24 == null && m_CenTables.IsAllCached(24))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(24, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table24 = new StdTable24(Reader, Table0, Table21);
                }

                return m_Table24;
            }
        }

        /// <summary>
        /// Gets the table 25 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable25 Table25
        {
            get
            {
                if (m_Table25 == null && m_CenTables.IsAllCached(25))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(25, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table25 = new StdTable25(Reader, Table0, Table21);
                }

                return m_Table25;
            }
        }

        /// <summary>
        /// Gets the table 26 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable26 Table26
        {
            get
            {
                if (m_Table26 == null && m_CenTables.IsAllCached(26))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(26, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table26 = new StdTable26(Reader, Table0, Table21);
                }

                return m_Table26;
            }
        }

        /// <summary>
        /// Gets the table 27 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable27 Table27
        {
            get
            {
                if (m_Table27 == null && m_CenTables.IsAllCached(27))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(27, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table27 = new StdTable27(Reader, Table21);
                }

                return m_Table27;
            }
        }

        /// <summary>
        /// Gets the table 28 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected virtual StdTable28 Table28
        {
            get
            {
                if (m_Table28 == null && m_CenTables.IsAllCached(28))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(28, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table28 = new StdTable28(Reader, Table0, Table21);
                }

                return m_Table28;
            }
        }

        /// <summary>
        /// Gets the Table2078 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/29/10 RCG 2.40.30        Created
        //  03/09/12 AF  2.53.48 194187 Added M2 Gateway support

        protected virtual OpenWayMfgTable2078 Table2078
        {
            get
            {
                if (m_Table2078 == null && m_CenTables.IsAllCached(2078))
                {
                    Stream strmRFLANNeighbors = new MemoryStream();
                    PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmRFLANNeighbors);

                    if (IsHighDataRate)
                    {
                        m_CenTables.BuildPSEMStream(2078, strmRFLANNeighbors, 0, OpenWayMfgTable2078HDR.HDR_TABLE_LENGTH);
                        m_Table2078 = new OpenWayMfgTable2078HDR(EDLReader);
                    }
                    else
                    {
                        m_CenTables.BuildPSEMStream(2078, strmRFLANNeighbors, 0, OpenWayMfgTable2078.ACT_RFLAN_NEIGHBOR_LIST_TBL_LENGTH);
                        m_Table2078 = new OpenWayMfgTable2078(EDLReader);
                    }
                }
                else if (m_Table2078 == null && m_GatewayTables.IsAllCached(2078))
                {
                    Stream strmRFLANNeighbors = new MemoryStream();
                    PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmRFLANNeighbors);

                    if (IsHighDataRate)
                    {
                        m_GatewayTables.BuildPSEMStream(2078, strmRFLANNeighbors, 0, OpenWayMfgTable2078HDR.HDR_TABLE_LENGTH);
                        m_Table2078 = new OpenWayMfgTable2078HDR(EDLReader);
                    }
                    else
                    {
                        m_GatewayTables.BuildPSEMStream(2078, strmRFLANNeighbors, 0, OpenWayMfgTable2078.ACT_RFLAN_NEIGHBOR_LIST_TBL_LENGTH);
                        m_Table2078 = new OpenWayMfgTable2078(EDLReader);
                    }
                }

                return m_Table2078;
            }
        }        

        /// <summary>
        /// Gets the Table 71 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/10 RCG 2.40.31 151959 Created
        // 08/02/10 AF  2.42.19        Added M2 Gateway support 

        protected virtual StdTable71 Table71
        {
            get
            {
                if (m_Table71 == null)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    if (m_CenTables.IsAllCached(71))
                    {
                        m_CenTables.BuildPSEMStream(71, TableStream);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsAllCached(71))
                    {
                        m_GatewayTables.BuildPSEMStream(71, TableStream);
                    }
#endif

                    m_Table71 = new StdTable71(Reader, Table0.StdVersion);
                }

                return m_Table71;
            }
        }

        /// <summary>
        /// Gets the standard table 72 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Created
        //
        protected virtual StdTable72 Table72
        {
            get
            {
                if (m_Table72 == null)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    if (m_CenTables.IsAllCached(72))
                    {
                        m_CenTables.BuildPSEMStream(72, TableStream);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsAllCached(72))
                    {
                        m_GatewayTables.BuildPSEMStream(72, TableStream);
                    }
#endif

                    m_Table72 = new StdTable72(Reader, Table71);
                }

                return m_Table72;
            }
        }

        /// <summary>
        /// Gets the standard table 73 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected virtual StdTable73 Table73
        {
            get
            {
                if (m_Table73 == null && m_CenTables.IsAllCached(73))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(73, TableStream);

                    m_Table73 = new StdTable73(Reader, Table72, Table71, Table0);
                }
#if (!WindowsCE)
                else if (m_Table73 == null && m_GatewayTables.IsAllCached(73))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(73, TableStream);

                    m_Table73 = new StdTable73(Reader, Table72, Table71, Table0);
                }
#endif

                return m_Table73;
            }
        }

        /// <summary>
        /// Gets the Table 74 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/10 RCG 2.40.31 151959 Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        // 08/10/10 AF  2.42.17        Added M2 Gateway event dictionary
        //
        protected virtual StdTable74 Table74
        {
            get
            {
                if (m_Table74 == null && m_CenTables.IsAllCached(74))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(74, TableStream);

                    m_Table74 = new StdTable74(Reader, Table71, m_EventDictionary, Table0.TimeFormat);
                }
#if (!WindowsCE)
                else if (m_Table74 == null && m_GatewayTables.IsAllCached(74))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(74, TableStream);

                    m_Table74 = new StdTable74(Reader, Table71, m_GWEventDictionary, Table0.TimeFormat);
                }
#endif

                return m_Table74;
            }
        }

        /// <summary>
        /// Gets the Table 2239 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  02/07/11 RCG 2.50.01          Created.
        //  03/09/12 AF  2.53.48 194187 Added M2 Gateway support

        protected OpenWayMFGTable2239 Table2239
        {
            get
            {
                if (m_Table2239 == null && m_CenTables.IsAllCached(2239))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2239, TableStream);

                    m_Table2239 = new OpenWayMFGTable2239(TableReader);
                }
                else if (m_Table2239 == null && m_GatewayTables.IsAllCached(2239))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(2239, TableStream);

                    m_Table2239 = new OpenWayMFGTable2239(TableReader);
                }

                return m_Table2239;
            }
        }

        /// <summary>
        /// Gets the Table 2240 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  02/07/11 RCG 2.50.01          Created.
        //  03/09/12 AF  2.53.48 194187 Added M2 Gateway support

        protected OpenWayMFGTable2240 Table2240
        {
            get
            {
                if (m_Table2240 == null && m_CenTables.IsAllCached(2240))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2240, TableStream);

                    m_Table2240 = new OpenWayMFGTable2240(TableReader, Table2239);
                }
                else if (m_Table2240 == null && m_GatewayTables.IsAllCached(2240))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(2240, TableStream);

                    m_Table2240 = new OpenWayMFGTable2240(TableReader, Table2239);
                }

                return m_Table2240;
            }
        }

        /// <summary>
        /// Gets the Table 2241 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  02/07/11 RCG 2.50.01          Created.
        //  03/09/12 AF  2.53.48 194187 Added M2 Gateway support

        protected OpenWayMFGTable2241 Table2241
        {
            get
            {
                if (m_Table2241 == null && m_CenTables.IsAllCached(2241))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2241, TableStream);

                    m_Table2241 = new OpenWayMFGTable2241(TableReader, Table2239);
                }
                else if (m_Table2241 == null && m_GatewayTables.IsAllCached(2241))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(2241, TableStream);

                    m_Table2241 = new OpenWayMFGTable2241(TableReader, Table2239);
                }

                return m_Table2241;
            }
        }

        /// <summary>
        /// Gets the Table 2242 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  02/07/11 RCG 2.50.01          Created.
        //  03/09/12 AF  2.53.48 194187 Added M2 Gateway support

        protected OpenWayMFGTable2242 Table2242
        {
            get
            {
                if (m_Table2242 == null && m_CenTables.IsAllCached(2242))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2242, TableStream);

                    m_Table2242 = new OpenWayMFGTable2242(TableReader, Table2239, Table0);
                }
                else if (m_Table2242 == null && m_GatewayTables.IsAllCached(2242))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(2242, TableStream);

                    m_Table2242 = new OpenWayMFGTable2242(TableReader, Table2239, Table0);
                }

                return m_Table2242;
            }
        }

        /// <summary>
        /// Gets the Table 2243 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  02/07/11 RCG 2.50.01          Created.
        //  03/09/12 AF  2.53.48 194187 Added M2 Gateway support

        protected OpenWayMFGTable2243 Table2243
        {
            get
            {
                if (m_Table2243 == null && m_CenTables.IsAllCached(2243))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2243, TableStream);

                    m_Table2243 = new OpenWayMFGTable2243(TableReader, Table2239, Table0);
                }
                else if (m_Table2243 == null && m_GatewayTables.IsAllCached(2243))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(2243, TableStream);

                    m_Table2243 = new OpenWayMFGTable2243(TableReader, Table2239, Table0);
                }

                return m_Table2243;
            }
        }

        /// <summary>
        /// Gets the Table 2261 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  02/19/13 jrf 2.70.68  288152  Created.
        //
        protected OpenWayMFGTable2261 Table2261
        {
            get
            {
                if (m_Table2261 == null && m_CenTables.IsAllCached(2261))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2261, TableStream);

                    m_Table2261 = new OpenWayMFGTable2261(TableReader);
                }

                return m_Table2261;
            }
        }

        /// <summary>
        /// Gets the Table 2265 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/13 AF  2.80.23 TR7590 Created
        //  05/31/13 AF  2.80.36 TR7590 Mfg table 217 is not all cached so have to check
        //                              on a specific field
        //
        protected MFGTable2265CTEConfig Table2265CTEConfig
        {
            get
            {
                if (m_Table2265CTEConfig == null && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl217CurrentThresholdExceededEnable, null))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2265, TableStream, CTE_CONFIG_TBL_OFFSET, CTE_CONFIG_TBL_SIZE);

                    m_Table2265CTEConfig = new MFGTable2265CTEConfig(TableReader);
                }

                return m_Table2265CTEConfig;
            }
        }

        /// <summary>
        /// Gets the Bell Weather DataSet Configuration from the Table 2265 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/15 PGH 4.50.212 577471 Created
        //
        protected MFGTable2265DataSetConfiguration Table2265DataSetConfiguration
        {
            get
            {
                try
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2265, TableStream, DATASET_CONFIGURATION_TBL_OFFSET, DATASET_CONFIGURATION_TBL_SIZE);

                    m_Table2265DataSetConfiguration = new MFGTable2265DataSetConfiguration(TableReader);
                }
                catch
                {
                }

                return m_Table2265DataSetConfiguration;
            }
        }

        /// <summary>
        /// Gets the Table 2368 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  06/08/11 jrf 2.50.08          Created.
        //
        protected OpenWayMFGTable2368 Table2368
        {
            get
            {
                if (m_Table2368 == null && m_CenTables.IsAllCached(2368))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2368, TableStream);

                    m_Table2368 = new OpenWayMFGTable2368(TableReader);
                }

                return m_Table2368;
            }
        }

        /// <summary>
        /// Gets the Table 2369 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  06/08/11 jrf 2.50.08          Created.
        //
        protected OpenWayMFGTable2369 Table2369
        {
            get
            {
                if (m_Table2369 == null && m_CenTables.IsAllCached(2369))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2369, TableStream);

                    m_Table2369 = new OpenWayMFGTable2369(TableReader);
                }

                return m_Table2369;
            }
        }

        /// <summary>
        /// Gets the Table 2370 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  06/08/11 jrf 2.50.08          Created.
        //
        protected OpenWayMFGTable2370 Table2370
        {
            get
            {
                if (m_Table2370 == null && m_CenTables.IsAllCached(2370))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2370, TableStream);

                    m_Table2370 = new OpenWayMFGTable2370(TableReader, Table0, Table2368);
                }

                return m_Table2370;
            }
        }

        /// <summary>
        /// Gets the Table 2377 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  12/07/15 PGH 4.50.219 627380  Created.
        //
        protected OpenWayMFGTable2377 Table2377
        {
            get
            {
                try
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2377, TableStream);

                    m_Table2377 = new OpenWayMFGTable2377(TableReader);
                }
                catch
                {
                }

                return m_Table2377;
            }
        }

        /// <summary>
        /// Gets the Table 2379 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  08/10/11 jrf 2.52.02 TREQ2706 Created
        //
        protected OpenWayMFGTable2379 Table2379
        {
            get
            {
                if (m_Table2379 == null && m_CenTables.IsAllCached(2379))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2379, TableStream);

                    m_Table2379 = new OpenWayMFGTable2379(TableReader);
                }

                return m_Table2379;
            }
        }

        /// <summary>
        /// Gets the Table 2382 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  08/10/11 jrf 2.52.02 TREQ2706 Created
        //
        protected OpenWayMFGTable2382 Table2382
        {
            get
            {
                if (m_Table2382 == null && m_CenTables.IsAllCached(2382))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2382, TableStream);

                    m_Table2382 = new OpenWayMFGTable2382(TableReader, Table2379, Table0);
                }

                return m_Table2382;
            }
        }

        /// <summary>
        /// Gets the Table 2383 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  08/11/11 jrf 2.52.02 TREQ2712 Created
        //
        protected OpenWayMFGTable2383 Table2383
        {
            get
            {
                if (m_Table2383 == null && m_CenTables.IsAllCached(2383))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2383, TableStream);

                    m_Table2383 = new OpenWayMFGTable2383(TableReader);
                }

                return m_Table2383;
            }
        }

        /// <summary>
        /// Gets the Table 2220 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  08/11/11 jrf 2.52.02 TREQ2712 Created
        //
        protected OpenWayMFGTable2220 Table2220
        {
            get
            {
                if (m_Table2220 == null && m_CenTables.IsAllCached(2220))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2220, TableStream);

                    m_Table2220 = new OpenWayMFGTable2220(TableReader);
                }

                return m_Table2220;
            }
        }

        /// <summary>
        /// Gets the Table 2419 (Actual Extended Self Read Limiting Table) object from the EDL file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/18/12 RCG 2.53.31 TRQ3439 Created 

        protected OpenWayMFGTable2419 Table2419
        {
            get
            {
                if (m_Table2419 == null && m_CenTables.IsAllCached(2419))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2419, TableStream);

                    m_Table2419 = new OpenWayMFGTable2419(TableReader);
                }

                return m_Table2419;
            }
        }

        /// <summary>
        /// Gets the Table 2421 (Extended Self Read and Energy Status Table) object from the EDL file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/20/12 jrf 2.53.32 TRQ3438 Created 

        protected OpenWayMFGTable2421 Table2421
        {
            get
            {
                if (m_Table2421 == null && m_CenTables.IsAllCached(2421))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2421, TableStream);

                    m_Table2421 = new OpenWayMFGTable2421(TableReader);
                }

                return m_Table2421;
            }
        }

        /// <summary>
        /// Gets the Table 2422 (Extended Energies and Instantaneous Values object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/18/12 RCG 2.53.31 TRQ3439 Created 

        protected OpenWayMFGTable2422 Table2422
        {
            get
            {
                if (m_Table2422 == null && m_CenTables.IsAllCached(2422) && Table2419 != null)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2422, TableStream);

                    m_Table2422 = new OpenWayMFGTable2422(TableReader, Table2419);
                }

                return m_Table2422;
            }
        }

        /// <summary>
        /// Gets the Table 2423 (Extended Self Read Data Table object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/20/12 jrf 2.53.32 TRQ3438 Created 

        protected OpenWayMFGTable2423 Table2423
        {
            get
            {
                if (m_Table2423 == null && m_CenTables.IsAllCached(2423) && Table2419 != null)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2423, TableStream);

                    m_Table2423 = new OpenWayMFGTable2423(TableReader, Table2419, Table0.TimeFormat);
                }

                return m_Table2423;
            }
        }

        /// <summary>
        /// Gets the Table 2425 Temperature Configuration object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/21/16 PGH 4.50.225 RTT556309 Created

        protected OpenWayMFGTable2425 Table2425
        {
            get
            {
                if (m_Table2425 == null && m_CenTables.IsAllCached(2425))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2425, TableStream);

                    m_Table2425 = new OpenWayMFGTable2425(TableReader);
                }

                return m_Table2425;
            }
        }

        /// <summary>
        /// Gets the Table 2426 Temperature Data object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/16 PGH 4.50.224 RTT556309 Created

        protected OpenWayMFGTable2426 Table2426
        {
            get
            {
                if (m_Table2426 == null && m_CenTables.IsAllCached(2426))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2426, TableStream);

                    m_Table2426 = new OpenWayMFGTable2426(TableReader);
                }

                return m_Table2426;
            }
        }

        /// <summary>
        /// Gets the Table 2427 Temperature Log object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/16 PGH 4.50.224 RTT556309 Created

        protected OpenWayMFGTable2427 Table2427
        {
            get
            {
                if (m_Table2427 == null && m_CenTables.IsAllCached(2427))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2427, TableStream);

                    m_Table2427 = new OpenWayMFGTable2427(TableReader);
                }

                return m_Table2427;
            }
        }

        /// <summary>
        /// Gets the Table 2175 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  04/09/12 jrf 2.53.54  196345  Created
        //
        protected OpenWayMFGTable2175 Table2175
        {
            get
            {
                if (m_Table2175 == null && m_CenTables.IsAllCached(2175))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2175, TableStream);

                    m_Table2175 = new OpenWayMFGTable2175(TableReader);
                }

                return m_Table2175;
            }
        }

        /// <summary>
        /// Gets the Table 2185 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  11/03/15 PGH 4.50.212 577471  Created
        //
        protected OpenWayMFGTable2185 Table2185
        {
            get
            {
                if (m_Table2185 == null && m_CenTables.IsAllCached(2185))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2185, TableStream);

                    m_Table2185 = new OpenWayMFGTable2185(TableReader);
                }

                return m_Table2185;
            }
        }

        /// <summary>
        /// Gets the Table 2186 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  11/03/15 PGH 4.50.212 577471  Created
        //
        protected OpenWayMFGTable2186 Table2186
        {
            get
            {
                if (m_Table2186 == null && m_CenTables.IsAllCached(2186))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2186, TableStream);

                    m_Table2186 = new OpenWayMFGTable2186(TableReader);
                }

                return m_Table2186;
            }
        }

        /// <summary>
        /// Gets the Table 2187 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  11/03/15 PGH 4.50.212 577471  Created
        //
        protected OpenWayMFGTable2187 Table2187
        {
            get
            {
                if (m_Table2187 == null && m_CenTables.IsAllCached(2187))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2187, TableStream);

                    m_Table2187 = new OpenWayMFGTable2187(TableReader);
                }

                return m_Table2187;
            }
        }

        /// <summary>
        /// Gets the Table 2191 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  03/06/13 jrf 2.80.06  TQ6663  Created
        //
        protected OpenWayMFGTable2191 Table2191
        {
            get
            {
                if (m_Table2191 == null && m_CenTables.IsAllCached(2191))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2191, TableStream);

                    m_Table2191 = new OpenWayMFGTable2191(TableReader);
                }

                return m_Table2191;
            }
        }

        /// <summary>
        /// Gets the Table 2428 (CENTRON Bridge operational state) object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 11/18/13 jrf 3.50.06 TQ 9479 Created 

        protected OpenWayMFGTable2428 Table2428
        {
            get
            {
                if (m_Table2428 == null && m_CenTables.IsAllCached(2428))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2428, TableStream);

                    m_Table2428 = new OpenWayMFGTable2428(TableReader);
                }

                return m_Table2428;
            }
        }

        /// <summary>
        /// Gets the Table 2437 (25 Year TOU Table) object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 11/18/13 jrf 3.50.06 TQ 9479 Created 

        protected OpenWayMFGTable2437 Table2437
        {
            get
            {
                if (m_Table2437 == null && m_CenTables.IsAllCached(2437))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2437, TableStream);

                    m_Table2437 = new OpenWayMFGTable2437(TableReader);
                }

                return m_Table2437;
            }
        }

        /// <summary>
        /// Gets the Table 2439 (Actual HAN RIB Limiting Table) object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 05/18/12 jrf 2.53.23 TRQ5997 Created 

        protected CHANMfgTable2439 Table2439
        {
            get
            {
                if (m_Table2439 == null && m_CenTables.IsAllCached(2439))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2439, TableStream);

                    m_Table2439 = new CHANMfgTable2439(TableReader);
                }

                return m_Table2439;
            }
        }

        /// <summary>
        /// Gets the Table 2440 (Active Block Price Schedule Table) object from the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 05/18/12 jrf 2.53.23 TRQ5997 Created 

        protected CHANMfgTable2440 Table2440
        {
            get
            {
                if (null != Table2439 && m_Table2440 == null && m_CenTables.IsAllCached(2440))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2440, TableStream);

                    m_Table2440 = new CHANMfgTable2440(TableReader, Table2439);
                }

                return m_Table2440;
            }
        }

        /// <summary>
        /// Gets the ERT Data table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/29/13 jrf 2.85.34 WR 418110 Created
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //
        private ICMMfgTable2508ERTData Table2508
        {
            get
            {
                if (m_Table2508 == null && m_CenTables.IsAllCached(2508) && null != Table2510)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2508, TableStream);

                    m_Table2508 = new ICMMfgTable2508ERTData(TableReader, Table0.TimeFormat, Table2510);
                }

                return m_Table2508;
            }
        }

        /// <summary>
        /// Gets the ERT actual limting table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //
        private ICMMfgTable2510ERTActual Table2510
        {
            get
            {
                if (m_Table2510 == null && m_CenTables.IsAllCached(2510))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2510, TableStream);

                    m_Table2510 = new ICMMfgTable2510ERTActual(TableReader);
                }

                return m_Table2510;
            }
        }

        /// <summary>
        /// Gets the ERT statistics table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //  09/09/13 jrf 2.85.39 WR 422369 Adding check to make sure 2511 is all cached in file.
        //
        private ICMMfgTable2511ERTStatistics Table2511
        {
            get
            {
                if (m_Table2511 == null && m_CenTables.IsAllCached(2511) && null != Table2510)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2511, TableStream);

                    m_Table2511 = new ICMMfgTable2511ERTStatistics(TableReader, Table2510);
                }

                return m_Table2511;
            }
        }

        /// <summary>
        /// Gets the Module configuration table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/13 jrf 2.80.21 TQ8280 Created
        //
        private ICMMfgTable2512ModuleConfiguration Table2512
        {
            get
            {
                if (m_Table2512 == null && m_CenTables.IsAllCached(2512))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2512, TableStream);

                    m_Table2512 = new ICMMfgTable2512ModuleConfiguration(TableReader);
                }

                return m_Table2512;
            }
        }

        /// <summary>
        /// Gets the Module Data table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08 n/a    Created
        //
        private ICMMfgTable2515ModuleData Table2515
        {
            get
            {
                if (m_Table2515 == null && m_CenTables.IsAllCached(2515))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2515, TableStream);

                    m_Table2515 = new ICMMfgTable2515ModuleData(TableReader);
                }

                return m_Table2515;
            }
        }

        /// <summary>
        /// Gets the Module Status table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08 n/a    Created
        //  12/12/13 AF  3.50.14 TQ9508 The size of table 2516 now depends on device class 
        //
        private ICMMfgTable2516ModuleStatus Table2516
        {
            get
            {
                if (m_Table2516 == null && m_CenTables.IsAllCached(2516))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2516, TableStream);

                    m_Table2516 = new ICMMfgTable2516ModuleStatus(TableReader, m_strDeviceClass.Value);
                }

                return m_Table2516;
            }
        }

        /// <summary>
        /// Gets the Cell Data table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08 n/a    Created
        //
        private ICMMfgTable2518CellularData Table2518
        {
            get
            {
                if (m_Table2518 == null && m_CenTables.IsAllCached(2518))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2518, TableStream);

                    m_Table2518 = new ICMMfgTable2518CellularData(TableReader);
                }

                return m_Table2518;
            }
        }

        /// <summary>
        /// Gets the Cell Status table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08 n/a    Created
        //
        private ICMMfgTable2519CellularStatus Table2519
        {
            get
            {
                if (m_Table2519 == null && m_CenTables.IsAllCached(2519))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader TableReader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2519, TableStream);

                    m_Table2519 = new ICMMfgTable2519CellularStatus(TableReader);
                }

                return m_Table2519;
            }
        }

        /// <summary>
        /// Gets the Table 2521 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  04/04/14 jrf 3.50.61 461982 Removed passing unneeded std version from table 0 to 
        //                              table 2521's constructor.
        protected virtual ICSMfgTable2521 Table2521
        {
            get
            {
                if (m_Table2521 == null)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    if (m_CenTables.IsAllCached(2521))
                    {
                        m_CenTables.BuildPSEMStream(2521, TableStream);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsAllCached(2521))
                    {
                        m_GatewayTables.BuildPSEMStream(2521, TableStream);
                    }
#endif

                    m_Table2521 = new ICSMfgTable2521(Reader);
                }

                return m_Table2521;
            }
        }

        /// <summary>
        /// Gets the standard table 2522 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        protected virtual ICSMfgTable2522 Table2522
        {
            get
            {
                if (m_Table2522 == null)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    if (m_CenTables.IsAllCached(2522))
                    {
                        m_CenTables.BuildPSEMStream(2522, TableStream);
                    }
#if (!WindowsCE)
                    else if (m_GatewayTables.IsAllCached(2522))
                    {
                        m_GatewayTables.BuildPSEMStream(2522, TableStream);
                    }
#endif

                    m_Table2522 = new ICSMfgTable2522(Reader, Table2521);
                }

                return m_Table2522;
            }
        }

        /// <summary>
        /// Gets the standard table 2523 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  07/29/16 AF  4.60.02 623194 Added a fw version parameter for table 2523 because the event
        //                              list supported is different between 3G and 4G meters.
        //
        protected virtual ICSMfgTable2523 Table2523
        {
            get
            {
                if (m_Table2523 == null && m_CenTables.IsAllCached(2523))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2523, TableStream);

                    m_Table2523 = new ICSMfgTable2523(Reader, Table2522, Table2521, CommModuleVersion);
                }
#if (!WindowsCE)
                else if (m_Table2523 == null && m_GatewayTables.IsAllCached(2523))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(2523, TableStream);

                    m_Table2523 = new ICSMfgTable2523(Reader, Table2522, Table2521, CommModuleVersion);
                }
#endif

                return m_Table2523;
            }
        }

        /// <summary>
        /// Gets the Table 2524 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        protected virtual ICSMfgTable2524 Table2524
        {
            get
            {
                if (m_Table2524 == null && m_CenTables.IsAllCached(2524))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(2524, TableStream);

                    m_Table2524 = new ICSMfgTable2524(Reader, Table2521, m_ICSEventDictionary, Table0.TimeFormat);
                }
#if (!WindowsCE)
                else if (m_Table2524 == null && m_GatewayTables.IsAllCached(2524))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_GatewayTables.BuildPSEMStream(2524, TableStream);

                    m_Table2524 = new ICSMfgTable2524(Reader, Table2521, m_ICSEventDictionary, Table0.TimeFormat);
                }
#endif

                return m_Table2524;
            }
        }

        /// <summary>
        /// Gets whether or not the RFLAN Module is using High Data Rate
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/29/10 RCG 2.40.30		   Created

        protected bool IsHighDataRate
        {
            get
            {
                bool bIsHDR = false;

                if (CommModuleBase.ITRL_DEVICE_CLASS.Equals(CommModuleDeviceClass))
                {
                    bIsHDR = CommModuleRevision >= 2;
                }
                else if (CommModuleBase.ITR2_DEVICE_CLASS.Equals(CommModuleDeviceClass))
                {
                    bIsHDR = CommModuleRevision == 9 || CommModuleRevision >= 12;
                }

                return bIsHDR;
            }
        }

        /// <summary>
        /// Gets the History Log Configuration table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03 N/A    Created
        //  10/14/10 jrf 2.45.04 N/A    Added ability to retrieve CENTRON II history config data.
        //  02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        //  06/24/15 AF  4.20.14 593126 Added support for ITRK meters
        //  08/03/15 AF  4.20.19 586155 Call IsPolyEDL
        //
        protected virtual CENTRON_AMI_HistoryLogConfig HistoryConfig
        {
            get
            {
                if (m_HistoryConfig == null)
                {
                    Stream strmHistory = new MemoryStream();

                    m_CenTables.BuildPSEMStream(2048, strmHistory, HistoryConfigOffset, CENTRON_AMI_HistoryLogConfig.EVENT_CONFIG_SIZE);

                    PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmHistory);

                    if (IsPolyEDL())
                    {
                        m_HistoryConfig = new OpenWayBasicPoly_HistoryLogConfig(EDLReader);
                    }
                    else
                    {
                        m_HistoryConfig = new CENTRON_AMI_HistoryLogConfig(EDLReader);
                    }
                }

                return m_HistoryConfig;
            }
        }

        /// <summary>
        /// Gets the History Log Configuration table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //
        protected virtual ICS_Gateway_HistoryLogConfig CommModuleConfig
        {
            get
            {
                /*if (m_CommModuleConfig == null)
               {
                   Stream strmHistory = new MemoryStream();
                   m_CenTables.BuildPSEMStream(2048, strmHistory, HistoryConfigOffset, CENTRON_AMI_HistoryLogConfig.EVENT_CONFIG_SIZE); //Bad code, ICSTables needed

                   PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmHistory);
                   m_CommModuleConfig = new ICS_Gateway_HistoryLogConfig(EDLReader);
               }*/

                //TODO: check logic for ICS Gateway Device

                return m_CommModuleConfig;
            }
        }

        /// <summary>
        /// The Base Date for calculating dates in for CENTRON AMI EDL Devices
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/22/08 KRC 
        // 
        protected DateTime MeterReferenceTime
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            }
        }

        /// <summary>
        /// The Base Date for calculating dates in the Configuration Portion (Tabel 2048) of AMI Device
        /// </summary>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/28/08 KRC 2.00.03 00121848    Created
        //
        protected DateTime MeterConfigurationReferenceTime
        {
            get
            {
                return new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
            }
        }

        /// <summary>
        /// Gets the Vh Low Threshold for Voltage Monitoring.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created
        // 02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        // 08/03/15 AF  4.20.19 586155 Call IsPolyEDL()
        //
        protected virtual float VMVhLowThreshold
        {
            get
            {
                ushort usDivisor;
                float fThreshold = 0.0F;
                object objValue;

                if (IsPolyEDL())
                {
                    throw new InvalidOperationException("This method not supported for the current device type");
                }

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL103_DIVISOR, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_DIVISOR, null, out objValue);
                    usDivisor = (ushort)objValue;

                    // Get the thresholds.
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null, out objValue);
                    fThreshold = (ushort)objValue / (float)usDivisor;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return fThreshold;
            }
        }

        /// <summary>
        /// Gets the Vh High Threshold for Voltage Monitoring.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created
        // 02/18/11 RCG 2.50.04        Adding support for ITRD, ITRE, ITRF meters
        // 08/03/15 AF  4.20.19 586155 Call IsPolyEDL()
        //
        protected virtual float VMVhHighThreshold
        {
            get
            {
                ushort usDivisor;
                float fThreshold = 0.0F;
                object objValue;

                if (IsPolyEDL())
                {
                    throw new InvalidOperationException("This method not supported for the current device type");
                }

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL103_DIVISOR, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_DIVISOR, null, out objValue);
                    usDivisor = (ushort)objValue;

                    // Get the thresholds.
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null, out objValue);
                    fThreshold = (ushort)objValue / (float)usDivisor;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return fThreshold;
            }
        }

        /// <summary>
        /// Retreives the correct defined lids object.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/26/12 jrf 2.53.34 TRQ3438 Created
        // 08/03/15 AF  4.20.19 586155  Added ITRK device class
        //
        protected DefinedLIDs LIDDefinitions
        {
            get
            {
                if (null == m_LID)
                {
                    if (CENTRON_AMI.ITR3_DEVICE_CLASS == DeviceClass
                        || CENTRON_AMI.ITR4_DEVICE_CLASS == DeviceClass
                        || CENTRON_AMI.ITRE_DEVICE_CLASS == DeviceClass
                        || CENTRON_AMI.ITRF_DEVICE_CLASS == DeviceClass
                        || CENTRON_AMI.ITRK_DEVICE_CLASS == DeviceClass)
                    {
                        m_LID = new CentronPolyDefinedLIDs();
                    }
                    else
                    {
                        m_LID = new CentronMonoDefinedLIDs();
                    }
                }

                return m_LID;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the TOU Schedule in 2090 to the current TOU schedule if we are using a program file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/27/12 RCG 2.53.52 195665 Created
        
        private void UpdateTOUSchedule()
        {
            if (IsProgramFile && m_UpdatedTOUSchedule == false)
            {
                DateTime SeasonStart;
                bool DemandReset;
                bool SelfRead;
                DateTime NextSeasonStart;

                m_CenTables.UpdateTOUSeasonFromStandardTables(DateTime.Now, 0, out SeasonStart, out DemandReset, out SelfRead, out NextSeasonStart);
            }
        }

        /// <summary>
        /// Gets the registration status information from table 126.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        // 08/03/10 AF  2.42.11        Added support for M2 Gateway
        //
        private void GetRegistrationStatus()
        {
            CStdTable126 tbl126 = null;

            //Get Table 121
            if (m_CenTables.IsAllCached(121))
            {
                IList<TableData> lsttblData121 = m_CenTables.BuildPSEMStreams(121);
                lsttblData121[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl121BinaryReader = new PSEMBinaryReader(lsttblData121[0].PSEM);
                CStdTable121 tbl121 = new CStdTable121(Tbl121BinaryReader);

                IList<TableData> lsttblData126 = m_CenTables.BuildPSEMStreams(126);
                lsttblData126[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl126BinaryReader = new PSEMBinaryReader(lsttblData126[0].PSEM);

                //if (FWRevision < OPENWAY_1_5)
                if (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_1_5_RELEASE_1) < 0)
                {
                    tbl126 = new CStdTable126(Tbl126BinaryReader, tbl121);
                }
                else
                {
                    tbl126 = new CStdTable126_2008(Tbl121BinaryReader, tbl121);
                }
            }
#if (!WindowsCE)
            else
            {
                IList<TableData> lsttblData121 = m_GatewayTables.BuildPSEMStreams(121);
                lsttblData121[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl121BinaryReader = new PSEMBinaryReader(lsttblData121[0].PSEM);
                CStdTable121 tbl121 = new CStdTable121(Tbl121BinaryReader);

                IList<TableData> lsttblData126 = m_GatewayTables.BuildPSEMStreams(126);
                lsttblData126[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl126BinaryReader = new PSEMBinaryReader(lsttblData126[0].PSEM);

                tbl126 = new CStdTable126_2008(Tbl121BinaryReader, tbl121);
            }
#endif

            m_strRelayNativeAddress.Value = tbl126.RelayNativeAddress;
            m_strMasterRelayAptitle.Value = tbl126.MasterRelayAptitle;
            m_strNodeAptitle.Value = tbl126.NodeAptitle;
            m_usRegistrationDelay.Value = tbl126.RegistrationDelay;
            m_tsRegistrationPeriod.Value = tbl126.RegistrationPeriod;
            m_tsRegistrationCountDown.Value = tbl126.RegistrationCountDown;
        }

        /// <summary>
        /// Get the Lan Configuration Table to determine which events are supported
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private void GetLANConfig()
        {
            // Make sure that the EDL file contains the Log tables
            if (m_CenTables.IsTableKnown(2159) && m_CenTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_CenTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_CenTables.IsTableKnown(2161) && m_CenTables.IsAllCached(2161))
                {
                    //Get Table 2161 - LAN Log Control Table
                    IList<TableData> lsttblData2161 = m_CenTables.BuildPSEMStreams(2161);
                    lsttblData2161[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2161BinaryReader = new PSEMBinaryReader(lsttblData2161[0].PSEM);
                    m_Table2161 = new MFGTable2161(Tbl2161BinaryReader, tbl2159);
                }
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsTableKnown(2159) && m_GatewayTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_GatewayTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_GatewayTables.IsTableKnown(2161) && m_GatewayTables.IsAllCached(2161))
                {
                    //Get Table 2161 - LAN Log Control Table
                    IList<TableData> lsttblData2161 = m_GatewayTables.BuildPSEMStreams(2161);
                    lsttblData2161[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2161BinaryReader = new PSEMBinaryReader(lsttblData2161[0].PSEM);
                    m_Table2161 = new MFGTable2161(Tbl2161BinaryReader, tbl2159);
                }
            }
#endif
        }

        /// <summary>
        /// Get the HAN Configuration Table to determine which events are supported
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private void GetHANConfig()
        {
            // Make sure that the EDL file contains the Log tables
            if (m_CenTables.IsTableKnown(2159) && m_CenTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_CenTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_CenTables.IsTableKnown(2163) && m_CenTables.IsAllCached(2163))
                {
                    //Get Table 2163 - HAN Log Control Table
                    IList<TableData> lsttblData2163 = m_CenTables.BuildPSEMStreams(2163);
                    lsttblData2163[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2163BinaryReader = new PSEMBinaryReader(lsttblData2163[0].PSEM);
                    m_Table2163 = new MFGTable2163(Tbl2163BinaryReader, tbl2159);
                }
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsTableKnown(2159) && m_GatewayTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_GatewayTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_GatewayTables.IsTableKnown(2163) && m_GatewayTables.IsAllCached(2163))
                {
                    //Get Table 2163 - HAN Log Control Table
                    IList<TableData> lsttblData2163 = m_GatewayTables.BuildPSEMStreams(2163);
                    lsttblData2163[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2163BinaryReader = new PSEMBinaryReader(lsttblData2163[0].PSEM);
                    m_Table2163 = new MFGTable2163(Tbl2163BinaryReader, tbl2159);
                }
            }
#endif
        }

        /// <summary>
        /// Build the necessary Tables to get the HAN Entries
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/13/08 KRC 1.50.24		   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private void GetHANEntries()
        {
            // Make sure that the EDL file contains the Log tables
            if (m_CenTables.IsTableKnown(2159) && m_CenTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_CenTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_CenTables.IsTableKnown(2164) && m_CenTables.IsAllCached(2164))
                {
                    //Get Table 2164 - HAN Event Log
                    IList<TableData> lsttblData2164 = m_CenTables.BuildPSEMStreams(2164);
                    lsttblData2164[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2164BinaryReader = new PSEMBinaryReader(lsttblData2164[0].PSEM);
                    MFGTable2164 tbl2164 = new MFGTable2164(Tbl2164BinaryReader, tbl2159);

                    m_HANEntries = tbl2164.HANEntries;
                }
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsTableKnown(2159) && m_GatewayTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_GatewayTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_GatewayTables.IsTableKnown(2164) && m_GatewayTables.IsAllCached(2164))
                {
                    //Get Table 2164 - HAN Event Log
                    IList<TableData> lsttblData2164 = m_GatewayTables.BuildPSEMStreams(2164);
                    lsttblData2164[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2164BinaryReader = new PSEMBinaryReader(lsttblData2164[0].PSEM);
                    MFGTable2164 tbl2164 = new MFGTable2164(Tbl2164BinaryReader, tbl2159);

                    m_HANEntries = tbl2164.HANEntries;
                }
            }
#endif
        }

        /// <summary>
        /// Build the necessary Tables to get the LAN Entries
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/13/08 KRC 1.50.24		   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private void GetLANEntries()
        {
            // Make sure that the EDL file contains the Log tables
            if (m_CenTables.IsTableKnown(2159) && m_CenTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_CenTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                if (m_CenTables.IsTableKnown(2162) && m_CenTables.IsAllCached(2162))
                {
                    //Get Table 2162 - LAN Event Log
                    IList<TableData> lsttblData2162 = m_CenTables.BuildPSEMStreams(2162);
                    lsttblData2162[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2162BinaryReader = new PSEMBinaryReader(lsttblData2162[0].PSEM);
                    MFGTable2162 tbl2162 = new MFGTable2162(Tbl2162BinaryReader, tbl2159);

                    m_LANEntries = tbl2162.LANEntries;
                }
            }
#if (!WindowsCE)
            else if (m_GatewayTables.IsTableKnown(2159) && m_GatewayTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_GatewayTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                if (m_GatewayTables.IsTableKnown(2162) && m_GatewayTables.IsAllCached(2162))
                {
                    //Get Table 2162 - LAN Event Log
                    IList<TableData> lsttblData2162 = m_GatewayTables.BuildPSEMStreams(2162);
                    lsttblData2162[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2162BinaryReader = new PSEMBinaryReader(lsttblData2162[0].PSEM);
                    MFGTable2162 tbl2162 = new MFGTable2162(Tbl2162BinaryReader, tbl2159);

                    m_LANEntries = tbl2162.LANEntries;
                }
            }
#endif
        }

        /// <summary>
        /// Gets the quantity object described by the Energy and Demand LIDs.
        /// </summary>
        /// <param name="energyLID">The energy LID for the quantity.</param>
        /// <param name="demandLID">The demand LID for the quantity.</param>
        /// <param name="quantityDescription">The description of the quantity.</param>
        /// <param name="registers">The registers to create the quantity from.</param>
        /// <returns>The quantity if it is supported by the meter. Null if the quantity is not supported.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/09/08 RCG 2.00.00 N/A    Created

        private Quantity GetQuantityFromStandardTables(LID energyLID, LID demandLID, string quantityDescription, RegisterDataRecord registers)
        {
            Quantity FoundQuantity = null;
            int? EnergySelectionIndex = null;
            int? DemandSelectionIndex = null;
            double Value;
            DateTime TimeOfOccurance;

            // First find the selection indexes so we know what to retrieve.
            if (energyLID != null)
            {
                EnergySelectionIndex = FindEnergySelectionIndex(energyLID);
            }

            if (demandLID != null)
            {
                DemandSelectionIndex = FindDemandSelectionIndex(demandLID);
            }

            if (EnergySelectionIndex != null || DemandSelectionIndex != null)
            {
                // The meter supports the Quantity so we can start creating it.
                FoundQuantity = new Quantity(quantityDescription);
                if (EnergySelectionIndex != null)
                {
                    // Add the energy data items
                    Value = registers.TotalDataBlock.Summations[(int)EnergySelectionIndex];
                    FoundQuantity.TotalEnergy = new Measurement(Value, energyLID.lidDescription);
                    FoundQuantity.TOUEnergy = new List<Measurement>();

                    for (int iRate = 0; iRate < Table21.NumberOfTiers; iRate++)
                    {
                        Value = registers.TierDataBlocks[iRate].Summations[(int)EnergySelectionIndex];
                        FoundQuantity.TOUEnergy.Add(new Measurement(Value, GetTOUEnergyLID(energyLID, iRate).lidDescription));
                    }
                }

                if (DemandSelectionIndex != null)
                {
                    LID CumDemandLID = GetCumDemandLID(demandLID);
                    LID CCumDemandLID = GetCCumDemandLID(demandLID);
                    DemandRecord CurrentDemandRecord = registers.TotalDataBlock.Demands[(int)DemandSelectionIndex];

                    // Add the demand data items
                    // The quantity object only supports 1 occurrence so always use occurrence 0
                    Value = CurrentDemandRecord.Demands[0];
                    TimeOfOccurance = CurrentDemandRecord.TimeOfOccurances[0];

                    FoundQuantity.TotalMaxDemand = new DemandMeasurement(Value, demandLID.lidDescription);
                    FoundQuantity.TotalMaxDemand.TimeOfOccurrence = TimeOfOccurance;

                    Value = CurrentDemandRecord.Cum;
                    FoundQuantity.CummulativeDemand = new Measurement(Value, CumDemandLID.lidDescription);

                    Value = CurrentDemandRecord.CCum;
                    FoundQuantity.ContinuousCummulativeDemand = new Measurement(Value, CCumDemandLID.lidDescription);

                    // Add TOU rates
                    if (Table21.NumberOfTiers > 0)
                    {
                        FoundQuantity.TOUMaxDemand = new List<DemandMeasurement>();
                        FoundQuantity.TOUCummulativeDemand = new List<Measurement>();
                        FoundQuantity.TOUCCummulativeDemand = new List<Measurement>();

                        for (int iRate = 0; iRate < Table21.NumberOfTiers; iRate++)
                        {
                            CurrentDemandRecord = registers.TierDataBlocks[iRate].Demands[(int)DemandSelectionIndex];

                            Value = CurrentDemandRecord.Demands[0];
                            TimeOfOccurance = CurrentDemandRecord.TimeOfOccurances[0];

                            FoundQuantity.TOUMaxDemand.Add(new DemandMeasurement(Value, GetTOUDemandLid(demandLID, iRate).lidDescription));
                            FoundQuantity.TOUMaxDemand[iRate].TimeOfOccurrence = TimeOfOccurance;

                            Value = CurrentDemandRecord.Cum;
                            FoundQuantity.TOUCummulativeDemand.Add(new Measurement(Value, GetTOUDemandLid(CumDemandLID, iRate).lidDescription));

                            Value = CurrentDemandRecord.CCum;
                            FoundQuantity.TOUCCummulativeDemand.Add(new Measurement(Value, GetTOUDemandLid(CCumDemandLID, iRate).lidDescription));
                        }
                    }
                }
            }

            return FoundQuantity;
        }

        /// <summary>
        /// Determines the number of Extended Energy Registers are supported by the device
        /// </summary>
        /// <returns>The number of Extended Energy registers supported</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/19/12 RCG 2.53.32 TRQ3439 Created 
        //  06/24/15 AF 4.20.14 593126 Added ITRK
        //
        private int DetermineExtendedEnergyConfigCount()
        {
            int iCount = 0;

            switch (DeviceType)
            {
                case EDLDeviceTypes.OpenWayCentronBasicPolyITRE:
                case EDLDeviceTypes.OpenWayCentronAdvPolyITRF:
                case EDLDeviceTypes.OpenWayCentronPolyITRK:
                {
                    iCount = 6;
                    break;
                }
            }

            return iCount;
        }

        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested String</returns>
        private string GetMFGEDLString(CentronTblEnum CentronTblEnumValue)
        {
            return GetMFGEDLString(CentronTblEnumValue, -1);
        }

        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested String</returns>
        private string GetMFGEDLString(CentronTblEnum CentronTblEnumValue, int Index)
        {
            string strTemp = "";
            object Value;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_CenTables.IsCached((long)CentronTblEnumValue, aiIndex))
            {
                m_CenTables.GetValue(CentronTblEnumValue, aiIndex, out Value);
                strTemp = Value.ToString();
            }

            return strTemp;
        }

        /// <summary>
        /// Sets the string value in the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="strValue">Value to set.</param>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested String</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.11        Created.
        //
        private void SetMFGEDLString(string strValue, CentronTblEnum CentronTblEnumValue, int Index)
        {
            object objValue = strValue;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            m_CenTables.SetValue(CentronTblEnumValue, aiIndex, objValue);
        }

        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="mfgTableEnum">Centron AMI specific Enumberation Value</param>
        /// <param name="aiIndex">Parameterized index.</param>
        /// <returns>Requested ushort</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#        Description
        // -------- --- ------- ------------- ---------------------------------------
        // 10/31/16 jrf 4.70.28               Created.
        protected virtual ushort GetMFGEDLUShort(CentronTblEnum mfgTableEnum, params int[] aiIndex)
        {
            ushort usTemp = 0;
            object Value;

            if (m_CenTables.IsCached((long)mfgTableEnum, aiIndex))
            {
                m_CenTables.GetValue(mfgTableEnum, aiIndex, out Value);
                usTemp = ushort.Parse(Value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
            }

            return usTemp;
        }

        /// <summary>
        /// Gets the int value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested int</returns>
        private int GetMFGEDLInt(CentronTblEnum CentronTblEnumValue)
        {
            return GetMFGEDLInt(CentronTblEnumValue, -1);
        }

        /// <summary>
        /// Sets the int value in the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="iValue"></param>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested int</returns>
        private void SetMFGEDLInt(int iValue, CentronTblEnum CentronTblEnumValue)
        {
            SetMFGEDLInt(iValue, CentronTblEnumValue, -1);
        }

#if (!WindowsCE)
        /// <summary>
        /// Gets the int value out of the EDL defined by the supplied enumeration
        /// </summary>
        /// <param name="GatewayTblEnumValue">M2 Gateway specific enumeration value</param>
        /// <returns>Requested int</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Created
        //
        private int GetMFGEDLInt(GatewayTblEnum GatewayTblEnumValue)
        {
            return GetMFGEDLInt(GatewayTblEnumValue, -1);
        }
#endif

        /// <summary>
        /// Gets the int value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested int</returns>
        private int GetMFGEDLInt(CentronTblEnum CentronTblEnumValue, int Index)
        {
            int intTemp = 0;
            object Value;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_CenTables.IsCached((long)CentronTblEnumValue, aiIndex))
            {
                m_CenTables.GetValue(CentronTblEnumValue, aiIndex, out Value);
                intTemp = int.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }

            return intTemp;
        }

        /// <summary>
        /// Sets the int value in the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="iValue"></param>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested int</returns>
        private void SetMFGEDLInt(int iValue, CentronTblEnum CentronTblEnumValue, int Index)
        {
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_CenTables.IsCached((long)CentronTblEnumValue, aiIndex))
            {
                m_CenTables.SetValue(CentronTblEnumValue, aiIndex, iValue);
            }
        }

#if (!WindowsCE)
        /// <summary>
        /// Gets the int value out of the EDL defined by the supplied enumeration
        /// </summary>
        /// <param name="GatewayTblEnumValue">M2 Gateway specific enumeration value</param>
        /// <param name="Index">The index of the value</param>
        /// <returns>Requested int</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Created
        //
        private int GetMFGEDLInt(GatewayTblEnum GatewayTblEnumValue, int Index)
        {
            int intTemp = 0;
            object Value;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_GatewayTables.IsCached((long)GatewayTblEnumValue, aiIndex))
            {
                m_GatewayTables.GetValue(GatewayTblEnumValue, aiIndex, out Value);
                intTemp = int.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }

            return intTemp;
        }
#endif

        /// <summary>
        /// Gets the float value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested float</returns>
        private float GetMFGEDLFloat(CentronTblEnum CentronTblEnumValue)
        {
            return GetMFGEDLFloat(CentronTblEnumValue, -1);
        }

        /// <summary>
        /// Gets the float value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested float</returns>
        private float GetMFGEDLFloat(CentronTblEnum CentronTblEnumValue, int Index)
        {
            float fltTemp = 0.0F;
            object Value;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_CenTables.IsCached((long)CentronTblEnumValue, aiIndex))
            {
                m_CenTables.GetValue(CentronTblEnumValue, aiIndex, out Value);
                fltTemp = float.Parse(Value.ToString(), CultureInfo.InvariantCulture);
            }

            return fltTemp;
        }

        /// <summary>
        /// Gets the bool value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested bool</returns>
        private bool GetMFGEDLBool(CentronTblEnum CentronTblEnumValue)
        {
            bool blnTemp = false;
            object Value;
            if (m_CenTables.IsCached((long)CentronTblEnumValue, null))
            {
                m_CenTables.GetValue(CentronTblEnumValue, null, out Value);
                blnTemp = (bool)Value;
            }

            return blnTemp;
        }

        /// <summary>
        /// Sets the bool value of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="blnValue">Value to set</param>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- -------------- -------------------------------------------
        //  11/16/10 jrf 2.45.13                 Created
        //
        private void SetMFGEDLBool(bool blnValue, CentronTblEnum CentronTblEnumValue)
        {
            m_CenTables.SetValue(CentronTblEnumValue, null, blnValue);
        }

        /// <summary>
        /// Gets the uint value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested uint</returns>
        private uint GetMFGEDLUint(CentronTblEnum CentronTblEnumValue)
        {
            uint uiTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)CentronTblEnumValue, null))
            {
                m_CenTables.GetValue(CentronTblEnumValue, null, out Value);
                uiTemp = (uint)Value;
            }

            return uiTemp;
        }

        /// <summary>
        /// Gets the UINT8 value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumeration Value</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/18/16 AF  4.50.231 WR 419822  Created
        //
        private byte GetMFGEDLByte(CentronTblEnum CentronTblEnumValue)
        {
            byte byTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)CentronTblEnumValue, null))
            {
                m_CenTables.GetValue(CentronTblEnumValue, null, out Value);
                byTemp = (byte)Value;
            }

            return byTemp;
        }

#if (!WindowsCE)
        /// <summary>
        /// Gets the uint value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="GatewayTblEnumValue">M2 Gateway specific enumeration value</param>
        /// <returns>Requested uint</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Created
        //
        private uint GetMFGEDLUint(GatewayTblEnum GatewayTblEnumValue)
        {
            uint uiTemp = 0;
            object Value;
            if (m_GatewayTables.IsCached((long)GatewayTblEnumValue, null))
            {
                m_GatewayTables.GetValue(GatewayTblEnumValue, null, out Value);
                uiTemp = (uint)Value;
            }

            return uiTemp;
        }
#endif

        /// <summary>
        /// This method will take a list of streams that have holes in them and create one full stream.
        /// The unknown parts will be filled with null data.
        /// </summary>
        /// <param name="lstTableData"></param>
        /// <param name="uiOffset"></param>
        /// <param name="uiSize"></param>
        /// <returns>Stream - A complete stream with unknown data set to null</returns>
        private Stream BuildStream(IList<TableData> lstTableData, uint uiOffset, uint uiSize)
        {
            MemoryStream strmNewStream = new MemoryStream();
            uint uiStreamOffset = uiOffset;

            foreach (TableData td in lstTableData)
            {
                if (td.Offset != uiStreamOffset)
                {
                    //The offset of our present stream does not match where we think we should be so we need to
                    //  add in some filler data.
                    int iByteCount = (int)td.Offset - (int)uiStreamOffset;
                    byte[] byFiller = new byte[iByteCount];

                    MemoryStream strmFiller = new MemoryStream(byFiller);

                    strmFiller.WriteTo(strmNewStream);
                    uiStreamOffset += (uint)strmFiller.Length;
                }

                td.PSEM.WriteTo(strmNewStream);
                uiStreamOffset += (uint)td.PSEM.Length;
            }

            // Now we have gone through each of the Streams passed in, we need to make sure there
            //  was no data missing from the end.
            if ((uint)strmNewStream.Length < uiSize)
            {
                int iByteCount = (int)uiSize - (int)strmNewStream.Length;
                byte[] byFiller = new byte[iByteCount];

                MemoryStream strmFiller = new MemoryStream(byFiller);

                strmFiller.WriteTo(strmNewStream);
                uiStreamOffset += (uint)strmFiller.Length;
            }

            strmNewStream.Position = 0;

            return strmNewStream;
        }

        /// <summary>
        /// Gets a LID value for the specified rate.
        /// </summary>
        /// <param name="originalLID">The original LID that should be changed.</param>
        /// <param name="iRate">The rate to change to (0 = A, 1 = B, etc)</param>
        /// <returns>The resulting LID</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/18/09 RCG 2.20.05        Created

        private LID GetDemandLIDForRate(LID originalLID, int iRate)
        {
            uint uiNewLIDNumber;

            // Clear the current rate value;
            uiNewLIDNumber = originalLID.lidValue & ~(uint)DefinedLIDs.TOU_Data.TOU_DATA_MASK;

            // Set the new value
            switch (iRate)
            {
                case 0:
                    {
                        uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_A;
                        break;
                    }
                case 1:
                    {
                        uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_B;
                        break;
                    }
                case 2:
                    {
                        uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_C;
                        break;
                    }
                case 3:
                    {
                        uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_D;
                        break;
                    }
                case 4:
                    {
                        uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_E;
                        break;
                    }
                case 5:
                    {
                        uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_F;
                        break;
                    }
                case 6:
                    {
                        uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_G;
                        break;
                    }
            }

            return CreateLID(uiNewLIDNumber);
        }

        #endregion

        #region Private Properties

#if (!WindowsCE)
        /// <summary>
        /// Gets the History Log Configuration table for the M2 Gateway
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/25/10 AF  2.50.06        Created
        //  04/21/11 AF  2.50.31 167587 Corrected the offset into the table
        //
        private M2_Gateway_HistoryLogConfig M2_Gateway_HistoryConfig
        {
            get
            {
                if (m_M2GatewayHistoryConfig == null)
                {
                    Stream strmHistory = new MemoryStream();

                    m_GatewayTables.BuildPSEMStream(2048, strmHistory, 0, 35);

                    PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmHistory);

                    m_M2GatewayHistoryConfig = new M2_Gateway_HistoryLogConfig(EDLReader);
                }

                return m_M2GatewayHistoryConfig;
            }
        }

        /// <summary>
        /// Returns the part of ITRN 16 (2064) that has the Comm Module Device Class
        /// </summary>
        private OpenWayCommModule_2064 Table2064
        {
            get
            {
                if (m_Table2064 == null && m_CenTables.IsAllCached(2064))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(2064, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table2064 = new OpenWayCommModule_2064(Reader, (uint)TableStream.Length);
                }

                return m_Table2064;
            }
        }
#endif

        #endregion

        #region Members

        /// <summary>C12.19 Standard Table 00</summary>
        protected CTable00 m_Table00 = null;
        /// <summary>C12.19 Standard Table 03</summary>
        protected CTable03 m_Table03 = null;
        /// <summary>C12.19 Standard Table 11</summary>
        protected StdTable11 m_Table11;
        /// <summary>C12.19 Standard Table 14</summary>
        protected StdTable14 m_Table14;
        /// <summary>C12.19 Standard Table 21</summary>
        protected StdTable21 m_Table21;
        /// <summary>C12.19 Standard Table 22</summary>
        protected StdTable22 m_Table22;
        /// <summary>C12.19 Standard Table 23</summary>
        protected StdTable23 m_Table23;
        /// <summary>C12.19 Standard Table 24</summary>
        protected StdTable24 m_Table24;
        /// <summary>C12.19 Standard Table 25</summary>
        protected StdTable25 m_Table25;
        /// <summary>C12.19 Standard Table 26</summary>
        protected StdTable26 m_Table26;
        /// <summary>C12.19 Standard Table 27</summary>
        protected StdTable27 m_Table27;
        /// <summary>C12.19 Standard Table 28</summary>
        protected StdTable28 m_Table28;
        /// <summary>C12.19 Standard Table 71</summary>
        protected StdTable71 m_Table71 = null;
        /// <summary>C12.19 Standard Table 72</summary>
        protected StdTable72 m_Table72 = null;
        /// <summary>C12.19 Standard Table 73</summary>
        protected StdTable73 m_Table73 = null;
        /// <summary>C12.19 Standard Table 74</summary>
        protected StdTable74 m_Table74 = null;
        /// <summary>C12.19 MFG Table 2161</summary>
        protected MFGTable2161 m_Table2161 = null;
        /// <summary>C12.19 MFG Table 2163</summary>
        protected MFGTable2163 m_Table2163 = null;
        /// <summary>C12.19 MFG Table 2175</summary>
        protected OpenWayMFGTable2175 m_Table2175 = null;
        /// <summary>C12.19 MFG Table 2185</summary>
        protected OpenWayMFGTable2185 m_Table2185 = null;
        /// <summary>C12.19 MFG Table 2186</summary>
        protected OpenWayMFGTable2186 m_Table2186 = null;
        /// <summary>C12.19 MFG Table 2187</summary>
        protected OpenWayMFGTable2187 m_Table2187 = null;
        /// <summary>C12.22 Config Table</summary>
        protected OpenWayMFGTable2191 m_Table2191 = null;
        /// <summary>Factory Data Info Table 2220</summary>
        protected OpenWayMFGTable2220 m_Table2220 = null;
        /// <summary>HAN Event Table 2239</summary>
        protected OpenWayMFGTable2239 m_Table2239 = null;
        /// <summary>HAN Event Table 2240</summary>
        protected OpenWayMFGTable2240 m_Table2240 = null;
        /// <summary>HAN Event Table 2241</summary>
        protected OpenWayMFGTable2241 m_Table2241 = null;
        /// <summary>HAN Event Table 2242</summary>
        protected OpenWayMFGTable2242 m_Table2242 = null;
        /// <summary>HAN Event Table 2243</summary>
        protected OpenWayMFGTable2243 m_Table2243 = null;
        /// <summary>HAN Event Table 2261</summary>
        protected OpenWayMFGTable2261 m_Table2261 = null;
        /// <summary>Current Per Phase Threshold Exceeded subtable of 2265</summary>
        protected MFGTable2265CTEConfig m_Table2265CTEConfig = null;
        /// <summary>Bell Weather DataSet Configuration of Table 2265</summary>
        protected MFGTable2265DataSetConfiguration m_Table2265DataSetConfiguration = null;

        /// <summary>Configuration of Comm Module Log</summary>
        protected ICS_Gateway_HistoryLogConfig m_CommModuleConfig;
        /// <summary>Configuration of AMI Histroy Log</summary>
        protected CENTRON_AMI_HistoryLogConfig m_HistoryConfig;
#if (!WindowsCE)
        /// <summary>Configuration of M2Gateway Hostory Log</summary>
        protected M2_Gateway_HistoryLogConfig m_M2GatewayHistoryConfig;
#endif
        /// <summary>Load Profile Data </summary>
        protected LoadProfileData m_LoadProfile;
        /// <summary>Extended Load Profile Data </summary>
        protected LoadProfileData m_ExtendedLoadProfile;
        /// <summary>
        /// Instrumentation Profile Data
        /// </summary>
        protected LoadProfileData m_InstrumentationProfile;
        /// <summary>C12.19 MFG Table 2064</summary>
        protected OpenWayCommModule_2064 m_Table2064;
        /// <summary>C12.19 MFG Table 2078</summary>
        protected OpenWayMfgTable2078 m_Table2078;
        /// <summary>C12.19 MFG Table 2091</summary>
        protected OpenWayPolyMFGTable2091 m_Table2091;
        /// <summary>
        /// Power Monitor Dimension Table
        /// </summary>
        protected OpenWayMFGTable2368 m_Table2368;
        /// <summary>
        /// Power Monitor Configuration Table
        /// </summary>
        protected OpenWayMFGTable2369 m_Table2369;
        /// <summary>
        /// Power Monitor Data Table
        /// </summary>
        protected OpenWayMFGTable2370 m_Table2370;
        /// <summary>
        /// Instantaneous Phase Current
        /// </summary>
        protected OpenWayMFGTable2377 m_Table2377;
        /// <summary>
        /// Actual FWDL Event Log Table
        /// </summary>
        protected OpenWayMFGTable2379 m_Table2379;
        /// <summary>
        /// FWDL Event Log Data Table
        /// </summary>
        protected OpenWayMFGTable2382 m_Table2382;
        /// <summary>
        /// FWDL CRC Table
        /// </summary>
        protected OpenWayMFGTable2383 m_Table2383;

        /// <summary>
        /// Actual Extended Self Read Limiting Table
        /// </summary>
        protected OpenWayMFGTable2419 m_Table2419;
        /// <summary>
        /// Extended Self Read and Energy Status Table
        /// </summary>
        protected OpenWayMFGTable2421 m_Table2421;
        /// <summary>
        /// Extended Energy Registers and Instantaneous Values
        /// </summary>
        protected OpenWayMFGTable2422 m_Table2422;
        /// <summary>
        /// Extended Self Read Data Table
        /// </summary>
        protected OpenWayMFGTable2423 m_Table2423;
        /// <summary>
        /// Temperature Configuration Table
        /// </summary>
        protected OpenWayMFGTable2425 m_Table2425;
        /// <summary>
        /// Temperature Data Table
        /// </summary>
        protected OpenWayMFGTable2426 m_Table2426;
        /// <summary>
        /// Temperature Log Table
        /// </summary>
        protected OpenWayMFGTable2427 m_Table2427;
        /// <summary>
        /// CENTRON Bridge Operational State
        /// </summary>
        protected OpenWayMFGTable2428 m_Table2428;
        /// <summary>
        /// 25 Year TOU Calendar
        /// </summary>
        protected OpenWayMFGTable2437 m_Table2437;
        /// <summary>
        /// Actual HAN RIB Limiting Table
        /// </summary>
        protected CHANMfgTable2439 m_Table2439;
        /// <summary>
        /// Active Block Price Schedule Table
        /// </summary>
        protected CHANMfgTable2440 m_Table2440;
        
        /// <summary>
        /// ERT Data Table
        /// </summary>
        protected ICMMfgTable2508ERTData m_Table2508;
        /// <summary>
        /// Actual ERT Limiting Table
        /// </summary>
        protected ICMMfgTable2510ERTActual m_Table2510;
        /// <summary>
        /// ERT Statistics Table
        /// </summary>
        protected ICMMfgTable2511ERTStatistics m_Table2511;
        /// <summary>
        /// ICM Module Configuration
        /// </summary>
        protected ICMMfgTable2512ModuleConfiguration m_Table2512;
        /// <summary>
        /// ICM Module Data
        /// </summary>
        protected ICMMfgTable2515ModuleData m_Table2515;
        /// <summary>
        /// ICM Module Status
        /// </summary>
        protected ICMMfgTable2516ModuleStatus m_Table2516;
        /// <summary>
        /// Cellular Data
        /// </summary>
        protected ICMMfgTable2518CellularData m_Table2518;
        /// <summary>
        /// Cellular Status
        /// </summary>
        protected ICMMfgTable2519CellularStatus m_Table2519;
        /// <summary>
        /// ACT Comm Module Table
        /// </summary>
        protected ICSMfgTable2521 m_Table2521;
        /// <summary>
        /// Comm Module Events ID Table
        /// </summary>
        protected ICSMfgTable2522 m_Table2522;
        /// <summary>
        /// Comm Module Event Log Control Table
        /// </summary>
        protected ICSMfgTable2523 m_Table2523;
        /// <summary>
        /// Comm Module Event Log Data Table
        /// </summary>
        protected ICSMfgTable2524 m_Table2524;
        /// <summary>Quantity</summary>
        protected List<Quantity> m_Registers;
        /// <summary>CalendareRcd</summary>
        protected C1219_CalendarRcd m_Calendar;
        /// <summary>1st Support Std Events</summary>
        protected List<CommLogEvent> m_lstSupportedStdEvents = null;
        /// <summary>1st MFG events</summary>
        protected List<CommLogEvent> m_lstSupportedMFGEvents = null;

        /// <summary>CachedString variable of SWRevision</summary>
        protected CachedString m_strSWRevision;
        /// <summary>CachedString variable of Serial Number</summary>
        protected CachedString m_strSerialNumber;
        /// <summary>CachedString variable of Device ID</summary>
        protected CachedString m_strDeviceID;
        /// <summary>CachedString variable of TOU ID</summary>
        protected CachedString m_strTOUID;
        /// <summary>CachedString variable of Tarrif ID</summary>
        protected CachedString m_strTarrifID;
        /// <summary>CachedString variable of UserData1</summary>
        protected CachedString m_strUserData1;
        /// <summary>CachedString variable of UserData2</summary>
        protected CachedString m_strUserData2;
        /// <summary>CachedString variable of UserData3</summary>
        protected CachedString m_strUserData3;
        /// <summary>CachedString variable of Cold Load Pickup Time</summary>
        protected CachedUint m_uiColdLoadPickupTime;
        /// <summary>CachedString variable of Demand Interval Length</summary>
        protected CachedInt m_iDemandIntervalLength;
        /// <summary>CachedString variable of Demand Sub Intervals</summary>
        protected CachedInt m_iNumDemandSubIntervals;
        /// <summary>CachedString variable of Demand Scheduling Control</summary>
        protected CachedInt m_iDemandSchedulingControl;
        /// <summary>CachedString variable of Demand Schedule Day</summary>
        protected CachedInt m_iDemandResetScheduledDay;
        /// <summary>CachedString variable of Demand Schedule Minute</summary>
        protected CachedInt m_iDemandResetScheduledMinute;
        /// <summary>CachedString variable of Demand Schedule Hour</summary>
        protected CachedInt m_iDemandResetScheduledHour;
        /// <summary>CachedString variable Test Mode Demand Interval Length</summary>
        protected CachedInt m_iTestModeDemandIntervalLength;
        /// <summary>CachedString variable of Number of Test Mode Demand Intervals</summary>
        protected CachedInt m_iNumTestModeDemandSubIntervals;
        /// <summary>CachedString variable of Load Profile Interval Length</summary>
        protected CachedInt m_iLPIntervalLength;
        /// <summary>CachedString variable of Number of Load Profile Channels</summary>
        protected CachedInt m_iNumLPChannels;
        /// <summary>CachedString variable of Load Profile Power Outage</summary>
        protected CachedInt m_iLPMinPowerOutage;
        /// <summary>CachedString variable of Load Profile Memory Size</summary>
        protected CachedInt m_iLPMemorySize;
        /// <summary>CachedString variable of Program ID</summary>
        protected CachedInt m_iProgramID;
        /// <summary>CachedString variable of CT Ratio</summary>
        protected CachedFloat m_fltCTRatio;
        /// <summary>CachedString variable of VT Ratio</summary>
        protected CachedFloat m_fltVTRatio;
        /// <summary>CachedString variable of Register Multiplier</summary>
        protected CachedFloat m_fltRegisterMultiplier;
        /// <summary>CachedString variable of Load Profile Outage Time</summary>
        protected CachedInt m_iCLPUOutageTime;
        /// <summary>CachedString variable of Mode Time Out</summary>
        protected CachedInt m_iModeTimeout;
        /// <summary>CachedString variable of Daily Self Read Time</summary>
        protected CachedString m_strDailySelfReadTime;
        /// <summary>CachedString variable of Daily Self Read Enable</summary>
        protected CachedBool m_bDailySelfReadEnabled;
        /// <summary>CachedString variable of Daily Self Read Hour</summary>
        protected CachedByte m_bytDailySelfReadHour;
        /// <summary>CachedString variable of Daily Self Read Minute</summary>
        protected CachedByte m_bytDailySelfReadMinute;
        /// <summary>CachedString variable of Load Control Threshold</summary>
        protected CachedString m_strLoadControlThreshold;
        /// <summary>CachedString variable of Load Control Reconnect</summary>
        protected CachedString m_strLoadControlReconnect;
        /// <summary>CachedString variable of Date Programmed</summary>
        protected CachedDate m_dtDateProgrammed;
        /// <summary>CachedString variable of Number of Demand Reset</summary>
        protected CachedInt m_iNumDemandResets;
        /// <summary>CachedString variable of Time Zone Offset</summary>
        protected CachedTimeSpan m_tsTimeZoneOffset;
        /// <summary>CachedString variable of DST Change Time</summary>
        protected CachedTimeSpan m_DSTChangeTime;
        /// <summary>CachedString variable of DST Offset</summary>
        protected CachedTimeSpan m_DSTOffset;
        /// <summary>CachedString variable of Comm Module Type</summary>
        protected CachedString m_strCommModType;
        /// <summary>CachedString variable of Comm Module Version</summary>
        protected CachedString m_strCommModVer;
        /// <summary>CachedString variable of Comm Module Build</summary>
        protected CachedString m_strCommModBuild;
        /// <summary>CachedString variable of HAN Module Type</summary>
        protected CachedString m_strHanModType;
        /// <summary>CachedString variable of HAN Module Version</summary>
        protected CachedString m_strHanModVer;
        /// <summary>CachedString variable of HAN Module Build</summary>
        protected CachedString m_strHanModBuild;
        /// <summary>CachedString variable of the HAN Channel Number</summary>
        protected CachedString m_strHanChannelNbr;
        /// <summary>CachedString variable of the PAN ID</summary>
        protected CachedString m_strPanId;
        /// <summary>CachedString variable of the HAN Startup Option</summary>
        protected CachedString m_strHanStartupOption;
        /// <summary>CachedString variable of the ZigBee Power Level</summary>
        protected CachedString m_strZigBeePowerLevel;
        /// <summary>CachedString variable of Display Module Version</summary>
        protected CachedString m_strDispModVer;
        /// <summary>CachedString variable of Display Module Build</summary>
        protected CachedString m_strDispModBuild;
        /// <summary>CachedString variable of Register Module Version</summary>
        protected CachedString m_strRegModVer;
        /// <summary>CachedString variable of Register Module Version</summary>
        protected CachedString m_strRegModBuild;
        /// <summary>CachedString variable of Service Voltage</summary>
        protected CachedFloat m_fltServiceVoltage;
        /// <summary>CachedString variable of Clock Synch</summary>
        protected CachedString m_strClockSynch;
        /// <summary>CachedString variable of DST Switch</summary>
        protected CachedString m_strDSTSwitch;
        /// <summary>CachedString variable of DST Length</summary>
        protected CachedInt m_iDSTLength;
        /// <summary>CachedString variable of Display EOI</summary>
        protected CachedBool m_blnDisplayEOI;
        /// <summary>CachedString variable of Watt Indicator</summary>
        protected CachedBool m_blnWattIndicator;
        /// <summary>CachedString variable of Disconnect Off Message</summary>
        protected CachedBool m_blnDisonnectOFFMessage;
        /// <summary>CachedString variable of Display On Time</summary>
        protected CachedInt m_iDisplayOnTime;
        /// <summary>CachedString variable of Low Battery Error</summary>
        protected CachedString m_strLowBatteryError;
        /// <summary>CachedString variable of Loss Of Phase Error</summary>
        protected CachedString m_strLossOfPhaseError;
        /// <summary>CachedString variable of Clock TOU Error</summary>
        protected CachedString m_strClockTOUError;
        /// <summary>CachedString variable of Reverse Power Error</summary>
        protected CachedString m_strReversePowerError;
        /// <summary>CachedString variable of Load Profile Error</summary>
        protected CachedString m_strLoadProfileError;
        /// <summary>CachedString variable of Full Scale Error</summary>
        protected CachedString m_strFullScaleError;
        /// <summary>CachedString variable of Site Scan Error</summary>
        protected CachedString m_strSiteScanError;
        /// <summary>CachedString variable of VM Data</summary>
        protected VMData m_VMData;
        /// <summary>CachedString variable of Error List</summary>
        protected string[] m_astrErrorList;
        /// <summary>The path to the EDL file</summary>
        protected string m_strEDLFile;   
        /// <summary>Variable of AMI Event Dictionary</summary>
        protected CENTRON_AMI_EventDictionary m_EventDictionary;
        /// <summary>Variable of M2 Gateway Event Dictionary</summary>
        protected M2_Gateway_EventDictionary m_GWEventDictionary;
        /// <summary>Variable of ICS Gateway Event Dictionary</summary>
        protected ICS_Gateway_EventDictionary m_ICSEventDictionary;
        /// <summary>CachedBool of Device In GMT</summary>
        protected CachedBool m_blnDeviceInGMT;
        /// <summary>CachedBool variable of DST Enable Flag</summary>
        protected CachedBool m_blnDSTEnabled;
        /// <summary>CachedBool variable of Meter In DST Flag</summary>
        protected CachedBool m_blnMeterInDST;
        /// <summary>CachedBool variable of Meter In Test Mode Flag</summary>
        protected CachedBool m_blnMeterInTestMode;
        /// <summary>CachedBool variable of Time Zone Enable Flag</summary>
        protected CachedBool m_blnTimeZoneEnabled;
        /// <summary>CachedDate variable of Current Time</summary>
        protected CachedDate m_dtCurrentTime;
        /// <summary>CachedFloat variable of FW Version</summary>
        protected CachedFloat m_fltFWRevision;
        /// <summary>CachedFloat variable of FW Build</summary>
        protected CachedByte m_byFWBuild;
        /// <summary>CachedFloat variable of HW Revision</summary>
        protected CachedFloat m_fltHWRevision;
        /// <summary>CachedString variable of Master Relay Aptitle</summary>
        protected CachedString m_strMasterRelayAptitle;
        /// <summary>CachedString variable of MFG Serial Number</summary>
        protected CachedString m_strMFGSerialNumber;
        /// <summary>CachedString variable of Relay Node Aptitle</summary>
        protected CachedString m_strNodeAptitle;
        /// <summary>CachedString variable of Relay Native Address</summary>
        protected CachedString m_strRelayNativeAddress;
        /// <summary>CachedString variable of Registration CountDown</summary>
        protected CachedTimeSpan m_tsRegistrationCountDown;
        /// <summary>CachedUshort variable of Registration Delay</summary>
        protected CachedUshort m_usRegistrationDelay;
        /// <summary>CachedTimeSpan variable of Registration Period</summary>
        protected CachedTimeSpan m_tsRegistrationPeriod;
        /// <summary>CachedString variable of Device Class</summary>
        protected CachedString m_strDeviceClass;

        /// <summary>Variable of Day Of Week</summary>
        protected DaysOfWeek m_eDayOfWeek = DaysOfWeek.UNREAD;

        /// <summary>Variable of Event Entries</summary>
        protected HistoryEntry[] m_EventEntries;

        /// <summary>Variable of HAN Event Entries</summary>
        protected List<HANEntry> m_HANEntries = null;
        /// <summary>Variable of LAN Event Entries</summary>
        protected List<LANEntry> m_LANEntries = null;

        /// <summary>Variable of Netork Statistic</summary>
        protected List<CStatistic> m_lstNetworkStatistic = null;

        /// <summary>Stores the tables from the EDL file</summary>
        protected CentronTables m_CenTables;
#if (!WindowsCE)
        /// <summary>Stores the tables from the EDL file for an M2 Gateway meter</summary>
        protected GatewayTables m_GatewayTables;
#endif

        /// <summary>
        /// LID object, the inherited device object creates the correct LIDs object
        /// </summary>
        protected DefinedLIDs m_LID = null;

        private bool m_UpdatedTOUSchedule = false;

        private CalendarConfig m_CalendarConfig;
        private TOUConfig m_TOUConfig;

        private CachedBool m_blnSupports25YearTOU;

        /// <summary>
        /// CachedValue variable for the VA/VAR selection.
        /// </summary>
        protected CachedValue<short?> m_VAVARSelection;
        /// <summary>
        /// CachedValue variable for the VA calculation method.
        /// </summary>
        protected CachedValue<byte?> m_VACalculation;

        /// <summary>
        /// CachedValue variable indicating whether or not VA arithmetic was configured 
        /// in the instrumentation profile UI.
        /// </summary>
        protected CachedValue<bool?> m_UIInstrumentationProfileVAArithmeticConfigured;

        /// <summary>
        /// CachedValue variable indicating whether or not VA vectorial was configured 
        /// in the instrumentation profile UI.
        /// </summary>
        protected CachedValue<bool?> m_UIInstrumentationProfileVAVectorialConfigured;

        private CachedInt m_TimeFormat;

        #endregion

    }//EDLFile
}//Itron.Metering.Datafiles
