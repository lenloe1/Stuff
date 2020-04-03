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
//                           Copyright © 2006 - 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Itron.Common.C1219Tables.ANSIStandardII;
using Itron.Common.C1219Tables.CentronII;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;
using System.Globalization;
using Itron.Metering.TOU;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// This class is the base class for all EDL file classes.  EDL stands for End Device
	/// Language and the file is essentially an XML file that defines the current state of
	/// a device.  The EDL format is defined by the ANSI C12.19 format.
    /// </summary>
    public abstract class CentronIIEDLFileBase
    {
        #region Constants

        /// <summary>
        /// Device class string for HW 1.0 OpenWay Centron meters.
        /// </summary>
        protected const string ITRN_DEVICE_CLASS = "ITRN";
        /// <summary>
        /// Device class string for HW 1.5 OpenWay Centron meters.
        /// </summary>
        protected const string ITR1_DEVICE_CLASS = "ITR1";
        /// <summary>
        /// Device class string for Basic Polyphase meters.
        /// </summary>
        protected const string ITR3_DEVICE_CLASS = "ITR3";
        /// <summary>
        /// Device class string for Advanced Polyphase meters.
        /// </summary>
        protected const string ITR4_DEVICE_CLASS = "ITR4";
        /// <summary>
        /// Device class string for transparent devices.
        /// </summary>
        protected const string ITRT_DEVICE_CLASS = "ITRT";
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
        /// Hardware version of the PrismLite meter
        /// </summary>
        private const float PRISM_LITE_REVISION = 128.0f;


        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="FileName">full path to the EDL file</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/30/06 RDB				   Created
        // 08/10/10 AF  2.42.17        Added M2 Gateway event dictionary
        //
        public CentronIIEDLFileBase(string FileName)
        {
            m_strEDLFile = FileName;
            LoadFile();
            m_EventDictionary = new CENTRON_AMI_EventDictionary();
            InitializeInstanceVariables();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            SaveFile();
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

        public static bool IsEDLFile(string FileName)
        {
            bool bIsEDLFile = false;
            XmlDocument Document = new XmlDocument();

            // Make sure the file exists first
            if (File.Exists(FileName))
            {
                try
                {
                    // Load the Program
                    Document.PreserveWhitespace = true;
                    Document.Load(FileName);

                    if (Document.DocumentElement.Name == "edl")
                    {
                        bIsEDLFile = true;
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
                catch (Exception)
                {
                    bIsEDLFile = false;
                }
            }

            return bIsEDLFile;
        }//IsEDLFile


        #endregion

        #region Public Properties

        /// <summary>
        /// Return true if the EDL file contains History entries. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        //  07/30/10 AF  2.42.09        Added support for the M2 Gateway meter
        //
        public bool ContainsHistoryEntries
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
        public bool ContainsEventEntries
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
        public bool ContainsNetworkStatistics
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
        public bool ContainsCommLogs
        {
            get
            {
                bool bResult = false;

                //Check to make sure that one of the log tables is in the file (if not then the other will not be)
                if (m_CenTables.IsTableKnown(2162) && m_CenTables.IsAllCached(2162))
                {
                    bResult = true;
                }

                return bResult;
            }
        }

        /// <summary>Returns the full path to the EDL file</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/11/07 mcm 8.10.04		   Created
        /// </remarks>
        public string FileName
        {
            get { return m_strEDLFile; }//get

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
        public string DeviceClass
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

        public EDLDeviceTypes DeviceType
        {
            get
            {
                EDLDeviceTypes DeviceType = EDLDeviceTypes.OpenWayCentron;

                switch(DeviceClass)
                {
                    case ITRN_DEVICE_CLASS:
                    case ITR1_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentron;
                        break;
                    }
                    case ITR3_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentronBasicPoly;
                        break;
                    }
                    case ITR4_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.OpenWayCentronAdvPoly;
                        break;
                    }
                    case ITRT_DEVICE_CLASS:
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
                    case ITRA_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.CentronII;
                        break;
                    }
                    case ITRB_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.CentronIIBasicPoly;
                        break;
                    }
                    case ITRC_DEVICE_CLASS:
                    {
                        DeviceType = EDLDeviceTypes.CentronIIAdvancedPoly;
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
        //
        public string DeviceClassHumanReadable
        {
            get
            {
                string strDeviceClassHR = "";

                switch (DeviceClass)
                {
                    case ITRN_DEVICE_CLASS:
                        {
                            strDeviceClassHR = "v1.0-v1.3";
                            break;
                        }
                    case ITR1_DEVICE_CLASS:
                        {
                            strDeviceClassHR = "v1.5";
                            break;
                        }
                    case ITR3_DEVICE_CLASS:
                        {
                            strDeviceClassHR = "Basic Poly";
                            break;
                        }
                    case ITR4_DEVICE_CLASS:
                        {
                            strDeviceClassHR = "Adv Poly";
                            break;
                        }
                    case ITRT_DEVICE_CLASS:
                        {
                            strDeviceClassHR = "Transparent";
                            break;
                        }
                    case ITRA_DEVICE_CLASS:
                    case ITRB_DEVICE_CLASS:
                    case ITRC_DEVICE_CLASS:
                        {
                            strDeviceClassHR = "Centron II (C12.19)";
                            break; 
                        }
                    case LIS1_DEVICE_CLASS:
                    case lis1_DEVICE_CLASS:
                        {
                            strDeviceClassHR = "M2 Gateway";
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
        /// Property that provides a List of the Supprted MFG Events
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
        /// Property to return the LAN Configuration Table which will allow us to cehck if Events are supported.
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
        public bool TimeZoneEnabled
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
        public bool DeviceInGMT
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
        /// Gets the current day of the week cooresponding to the current 
        /// date of the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        public DaysOfWeek DayOfWeek
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
        public bool ContainsClockData
        {
            get
            {
                bool bContainsClockData = false;

                if (m_CenTables.IsAllCached(52))
                {
                    bContainsClockData = true;
                }

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
        public bool ContainsRegistrationStatus
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

                if (usNbrEntries > 0)
                {
                    bContainsRegistrationStatus = true;
                }

                return bContainsRegistrationStatus;
            }
        }
        
        #endregion

        #region Itron Device Configuration

        /// <summary> Gets the DST Flag </summary>
        /// // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/10/08 KRC
        public bool DSTEnabled
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
        /// Property used to get the device time (DateTime) from the meter
        /// </summary>
        public DateTime DeviceTime
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
        public float FWRevision
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
                        ((float)float.Parse(strRevision,CultureInfo.InvariantCulture) / (int)1000);
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
        public byte FirmwareBuild
        {
            get
            {
                object Value;

                if (!m_byFWBuild.Cached && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_REGISTER_FW_BUILD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL60_REGISTER_FW_BUILD, null, out Value);
                    m_byFWBuild.Value = (byte)Value;
                }

                return m_byFWBuild.Value;
            }
        }


        /// <summary>
        /// Get the Exception Configuration
        /// </summary>
        public List<ushort> ExceptionConfig
        {
            get
            {
                int[] anLimits = m_CenTables.GetElementLimits(StdTableEnum.STDTBL123_EVENT_REPORTED);
                List<ushort> lstAlarms = new List<ushort>();
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
                return lstAlarms;
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
        public string MFGSerialNumber
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
       
        #endregion ANSI Device Configuration

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
        public float HWRevision
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
        public bool IsMeterInDST
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
        public bool IsMeterInTestMode
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


        #endregion ANSI Device Status

        #region Protected Methods

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
        protected short GetSTDEDLShort(StdTableEnum StdTableEnumValue)
        {
            short sTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                sTemp = (short)Value;
            }

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
        protected void SetSTDEDLShort(short sValue, StdTableEnum StdTableEnumValue)
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
        protected string GetSTDEDLString(StdTableEnum StdTableEnumValue)
        {
            string strTemp = "";
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                strTemp = Value.ToString();
            }

            return strTemp;
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
        //  08/02/10 AF  2.42.11              Added support for M2 Gateway
        //
        protected ushort GetSTDEDLUShort(StdTableEnum stdTableEnum, params int[] aiIndex)
        {
            ushort usTemp = 0;
            object Value;

            if (m_CenTables.IsCached((long)stdTableEnum, aiIndex))
            {
                m_CenTables.GetValue(stdTableEnum, aiIndex, out Value); 
                usTemp = ushort.Parse(Value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
            }

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
        protected int GetSTDEDLInt(StdTableEnum StdTableEnumValue)
        {
            int intTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                intTemp = (int)int.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }

            return intTemp;
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
        protected float GetSTDEDLFloat(StdTableEnum StdTableEnumValue)
        {
            float fltTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                fltTemp = float.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }

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
        protected bool GetSTDEDLBool(StdTableEnum StdTableEnumValue)
        {
            bool blnTemp = false;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                blnTemp = (bool)Value;
            }

            return blnTemp;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load the EDL file into the centron table variable
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/26/06 RDB				   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private void LoadFile()
        {
            XmlReader xmlReader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CheckCharacters = false;

            xmlReader = XmlReader.Create(m_strEDLFile, settings);

            m_CenTables = new CentronTables();

            if (EDLDeviceTypes.M2GatewayDevice != DetermineDeviceType())
            {
                m_CenTables.LoadEDLFile(xmlReader);
            }
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
        private void SaveFile()
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

        protected virtual bool AllowTableExport(ushort usTableID)
        {
            // We are going to control the tables that are written
            // to the EDL file by the tables that we read. This
            // way we only need to change one place whenever new
            // tables are added or removed.
            return true;
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

        protected virtual bool AllowFieldExport(long idElement, int[] anIndex)
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
        private void BuildNetworkStatistics()
        {
            IList<TableData> lsttblData121 = null;
            IList<TableData> lsttblData127 = null;

            //Get Table 121
            if (m_CenTables.IsAllCached(121))
            {
                lsttblData121 = m_CenTables.BuildPSEMStreams(121);
            }

            lsttblData121[0].PSEM.Position = 0;
            PSEMBinaryReader Tbl121BinaryReader = new PSEMBinaryReader(lsttblData121[0].PSEM);
            CStdTable121 tbl121 = new CStdTable121(Tbl121BinaryReader);


            if (m_CenTables.IsAllCached(127))
            {
                lsttblData127 = m_CenTables.BuildPSEMStreams(127);
            }


            lsttblData127[0].PSEM.Position = 0;
            PSEMBinaryReader Tbl127BinaryReader = new PSEMBinaryReader(lsttblData127[0].PSEM);
            CStdTable127 tbl127 = new CStdTable127(Tbl127BinaryReader, tbl121);

            m_lstNetworkStatistic = tbl127.GetStatistics();
            //Get Table 127
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

            m_strRelayNativeAddress.Value = tbl126.RelayNativeAddress;
            m_strMasterRelayAptitle.Value = tbl126.MasterRelayAptitle;
            m_strNodeAptitle.Value = tbl126.NodeAptitle;
            m_usRegistrationDelay.Value = tbl126.RegistrationDelay;
            m_tsRegistrationPeriod.Value = tbl126.RegistrationPeriod;
            m_tsRegistrationCountDown.Value = tbl126.RegistrationCountDown;
        }

        /// <summary>
        /// Get the list of supported Std and Mfg Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private void GetSupportedCommunicationsEvents()
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
        private void GetEventEntries()
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
        } 

        /// <summary>
        /// Initializes the Cached Data Items
        /// </summary>
        private void InitializeInstanceVariables()
        {
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
        }

        /// <summary>
        /// Peeks into the EDL file to determine whether the
        /// device type is an OpenWay CENTRON or an M2 Gateway
        /// </summary>
        /// <returns>Enumeration of the device type</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Created
        //
        private EDLDeviceTypes DetermineDeviceType()
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

            xmlReader = XmlReader.Create(m_strEDLFile, settings);

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

        protected CTable00 Table0
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
 
					PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);
                    m_Table00 = new CTable00(Reader, (uint)TableStream.Length);
                }

                return m_Table00;
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

        protected StdTable71 Table71
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
        protected StdTable72 Table72
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
        protected StdTable73 Table73
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
        protected StdTable74 Table74
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

                return m_Table74;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Stores the tables from the EDL file
        /// </summary>
        protected CentronTables m_CenTables;

        /// <summary>
        /// The path to the EDL file
        /// </summary>
        private string m_strEDLFile;

        private CachedBool m_blnDeviceInGMT;
        private CachedBool m_blnDSTEnabled;
        private CachedBool m_blnMeterInDST;
        private CachedBool m_blnMeterInTestMode;  
        private CachedBool m_blnTimeZoneEnabled;
        private CachedDate m_dtCurrentTime;
        private CachedFloat m_fltFWRevision;
        private CachedByte m_byFWBuild;
        private CachedFloat m_fltHWRevision;
        private CachedString m_strMasterRelayAptitle;
        private CachedString m_strMFGSerialNumber;
        private CachedString m_strNodeAptitle;
        private CachedString m_strRelayNativeAddress;
        private CachedTimeSpan m_tsRegistrationCountDown;
        private CachedUshort m_usRegistrationDelay;
        private CachedTimeSpan m_tsRegistrationPeriod;
        private CachedString m_strDeviceClass;

		private CENTRON_AMI_EventDictionary m_EventDictionary;
     
        private DaysOfWeek m_eDayOfWeek = DaysOfWeek.UNREAD;

        private HistoryEntry[] m_EventEntries;

        private List<HANEntry> m_HANEntries = null;
        private List<LANEntry> m_LANEntries = null;

        private List<CStatistic> m_lstNetworkStatistic = null;

        private CTable00 m_Table00 = null;
        private StdTable71 m_Table71 = null;
        private StdTable72 m_Table72 = null;
        private StdTable73 m_Table73 = null;
        private StdTable74 m_Table74 = null;
        private MFGTable2161 m_Table2161 = null;
        private MFGTable2163 m_Table2163 = null;

        private List<CommLogEvent> m_lstSupportedStdEvents = null;
        private List<CommLogEvent> m_lstSupportedMFGEvents = null;

        #endregion

    } //CentronIIEDLFileBase
} //Itron.Metering.Datafiles
