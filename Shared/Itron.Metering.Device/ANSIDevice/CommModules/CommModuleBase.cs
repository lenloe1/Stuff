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
//                           Copyright © 2010 - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Communications;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Basic Communications Module data object
    /// </summary>
    public class CommModuleBase
    {
        #region Constants

        /// <summary>
        /// Device Class for RFLAN Modules
        /// </summary>
        public const string ITR2_DEVICE_CLASS = "ITR2";
        /// <summary>
        /// Device Class for PrismLite Modules
        /// </summary>
        public const string ITRL_DEVICE_CLASS = "ITRL";
        /// <summary>
        /// Device Class for PLAN (PLC) Modules
        /// </summary>
        public const string ITRP_DEVICE_CLASS = "ITRP";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device.</param>
        /// <param name="amiDevice">The device object for the current device</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        // 11/14/13 AF  3.50.03	        Class re-architecture - replace CENTRON_AMI parameter 
        //                              with CANSIDevice
        //
        public CommModuleBase(CPSEM psem, CANSIDevice amiDevice)
        {
            m_PSEM = psem;
            m_AMIDevice = amiDevice;

            m_Table121 = null;
            m_Table122 = null;
            m_Table123 = null;
            m_Table126 = null;
            m_Table127 = null;
        }

        /// <summary>
        /// Creates the appropriate Comm Module class based on the Comm Module device class
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="amiDevice">The Device Object for the current device</param>
        /// <returns>The Comm Module object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        // 10/11/11 AF  2.53.00        Added code for Cisco comm module.
        // 07/20/12 AF  2.60.46 201155 Removed the firmware version check and substituted a check
        //                              on the comm module id from Mfg table 17
        // 01/16/13 mah 2.70.58        Added support for SL7000 Gateway
        // 03/26/13 mah 2.80.11        Added support for ICS devices and comm modules
        // 11/14/13 AF  3.50.03	        Class re-architecture - replace CENTRON_AMI parameter 
        //                              with CANSIDevice
        // 12/26/13 AF  3.50.16 TQ9484 Added code to deal with a Bridge meter in ChoiceConnect mode. Try
        //                             to avoid reading RFLAN tables
        // 11/17/14 jrf 4.00.89 TBD    Added special identification of comm module types for Bridge meters in ChoicConnect mode.
        // 03/10/15 AF  4.10.05 WR569364 Added support for ITRK
        //
        public static CommModuleBase CreateCommModule(CPSEM psem, CANSIDevice amiDevice)
        {
            CommModuleBase NewCommModule = null;
            string CG_MESH_MODULE = "CISCO CGMesh Module ";
            string strCommModuleID = "";

            if (amiDevice is SL7000_Gateway)
            {
                // This is a Cisco module
                NewCommModule = new CiscoCommModule(psem, amiDevice);
            }
            else if ((amiDevice is OpenWayITRJ) || (amiDevice is OpenWayPolyITRK) || (amiDevice is ICS_Gateway))
            {
                // This is an ICS communications module
                NewCommModule = new ICSCommModule(psem, amiDevice);
            }
            else if ((amiDevice is IBridge) && (((IBridge)amiDevice).CurrentRegisterCommOpMode == OpenWayMFGTable2428.ChoiceConnectCommOpMode.ChoiceConnectOperationalMode))
            {                
                if (OpenWayMFGTable2428.ChoiceConnectCommMfgMode.ChoiceConnectManufacturingModeRFMesh == ((IBridge)amiDevice).ChoiceConnectManufacturedMode)
                {
                    // This is a Cisco module
                    NewCommModule = new CiscoCommModule(psem, amiDevice);
                }
                else if (OpenWayMFGTable2428.ChoiceConnectCommMfgMode.ChoiceConnectManufacturingModeRFLAN == ((IBridge)amiDevice).ChoiceConnectManufacturedMode)
                {
                    // This is a RFLAN Module
                    NewCommModule = new RFLANCommModule(psem, amiDevice);
                }
            }
            else
            {
                switch (amiDevice.CommModuleDeviceClass)
                {
                    case ITR2_DEVICE_CLASS:
                    case ITRL_DEVICE_CLASS:
                        {
                            try
                            {
                                RFLANMfgTable2065 table2065 = new RFLANMfgTable2065(psem);
                                strCommModuleID = table2065.RFLANIdentification;
                            }
                            catch (PSEMException)
                            {
                                // It's possible that reading table 2065 could fail in a meter whose Comm Module
                                // is not working.
                                strCommModuleID = null;
                            }
                            catch (TimeOutException)
                            {
                                // It's possible that reading table 2065 could fail in a meter whose Comm Module
                                // is not working.
                                strCommModuleID = null;
                            }

                            if (null != strCommModuleID)
                            {
                                if (String.CompareOrdinal(strCommModuleID, CG_MESH_MODULE) == 0)
                                {
                                    // This is a Cisco module
                                    NewCommModule = new CiscoCommModule(psem, amiDevice);
                                }
                                else
                                {
                                    // This is a RFLAN or PrismLite Module
                                    NewCommModule = new RFLANCommModule(psem, amiDevice);
                                }
                            }
                            else
                            {
                                // This is a Third Party or Unknown Comm Module
                                NewCommModule = new CommModuleBase(psem, amiDevice);
                            }
                            break;
                        }
                    case ITRP_DEVICE_CLASS:
                        {
                            // This is a PLAN Comm Module
                            NewCommModule = new PLANCommModule(psem, amiDevice);
                            break;
                        }
                    default:
                        {
                            // This is a Third Party or Unknown Comm Module
                            NewCommModule = new CommModuleBase(psem, amiDevice);
                            break;
                        }
                }
            }
            return NewCommModule;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of network interfaces supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/22/13 AF  3.00.22 WR426166 Added as a way to check whether table 121 has actual
        //                              or default values (0xFFs). Since it is used to determine the size
        //                              of other tables, we will get errors on reading other tables if this
        //                              table has defaults.
        //
        public byte NumberOfInterfaces
        {
            get
            {
                byte byNumInterfaces = 0;

                if (Table121 != null)
                {
                    byNumInterfaces = Table121.NumberOfInterfaces;
                }

                return byNumInterfaces;
            }
        }

        /// <summary>
        /// Returns whether or not standard table 121 has default values (all 0xFFs)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/22/13 AF  3.00.22 WR426166 Created to provide a more intuitive way of determining
        //                              whether we should try to read any of the network tables decade
        //
        public bool Table121HasDefaultValues
        {
            get
            {
                bool blnDefault;

                if (NumberOfInterfaces < 0xFF)
                {
                    blnDefault = false;
                }
                else
                {
                    blnDefault = true;
                }

                return blnDefault;
            }
        }

        /// <summary>
        /// Gets the Electronic Serial Number for the Comm Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public string ElectronicSerialNumber
        {
            get
            {
                string strESN = "";

                if (Table122 != null)
                {
                    strESN = Table122.ElectronicSerialNumber;
                }

                return strESN;
            }
        }

        /// <summary>
        /// Gets the Native Address for the Comm Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public string NativeAddress
        {
            get
            {
                string strNativeAddress = "";

                if (Table122 != null)
                {
                    strNativeAddress = Table122.NativeAddress;
                }

                return strNativeAddress;
            }
        }

        /// <summary>
        /// Gets/Sets the Raw Native Address for the Comm Module.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/10 jrf 2.40.27		   Created

        public byte[] RawNativeAddress
        {
            get
            {
                byte[] abytRawNativeAddress = new byte[0];

                if (Table122 != null)
                {
                    abytRawNativeAddress = Table122.RawNativeAddress;
                }

                return abytRawNativeAddress;
            }
            set
            {
                if (Table122 != null)
                {
                    Table122.RawNativeAddress = value;
                }
            }
        }

        /// <summary>
        /// Gets the Decade 12 Statistics for each interface supported by the 
        /// implementation.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public List<CStatistic> NetworkStats
        {
            get
            {
                List<CStatistic> StatCollection = new List<CStatistic>();

                if (Table127 != null)
                {
                    StatCollection = Table127.GetStatistics();
                }

                return StatCollection;
            }
        }

        /// <summary>
        /// Gets the Native Address used to access the C12.22 Relay on this
        /// route for the C12.22 Node's local C12.22 Network Segment
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public string RelayNativeAddress
        {
            get
            {
                string strRelayNativeAddress = "";

                if (Table126 != null)
                {
                    strRelayNativeAddress = Table126.RelayNativeAddress;
                }

                return strRelayNativeAddress;
            }
        }

        /// <summary>
        /// Gets the Aptitle for the Cell Relay responsible for this node.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public string MasterRelayAptitle
        {
            get
            {
                string strMasterRelayAptitle = "";

                if (Table126 != null)
                {
                    strMasterRelayAptitle = Table126.MasterRelayAptitle;
                }

                return strMasterRelayAptitle;
            }
        }

        /// <summary>
        /// Gets the Comm Module's Aptitle
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public string NodeAptitle
        {
            get
            {
                string strNodeAptitle = "";

                if (Table126 != null)
                {
                    strNodeAptitle = Table126.NodeAptitle;
                }

                return strNodeAptitle;
            }
        }

        /// <summary>
        /// Maximum random delay, in seconds, between each power up
        /// and the automatic issuance of the first Registration Service
        /// request by the C12.22 node.  This function is disabled when
        /// this field is set to zero.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public ushort RegistrationDelay
        {
            get
            {
                ushort usDelay = 0;

                if (Table126 != null)
                {
                    usDelay = Table126.RegistrationDelay;
                }

                return usDelay;
            }
        }

        /// <summary>
        /// Maximum duration, TimeSpan, before the C12.22 Node's registration
        /// expires.  The C12.22 Node needs to reregister itself before this 
        /// period lapses in order to remain registered.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public TimeSpan RegistrationPeriod
        {
            get
            {
                TimeSpan Period = new TimeSpan();

                if (Table126 != null)
                {
                    Period = Table126.RegistrationPeriod;
                }

                return Period;
            }
        }

        /// <summary>
        /// The amount of time (TimeSpan) left before the registration period expires
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public TimeSpan RegistrationCountDown
        {
            get
            {
                TimeSpan CountDown = new TimeSpan();

                if (Table126 != null)
                {
                    CountDown = Table126.RegistrationCountDown;
                }

                return CountDown;
            }
        }

        /// <summary>
        /// Gets the MAC Address of the Comm Module.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public virtual string FormattedMACAddress
        {
            get
            {
                return MACAddress.ToString("X8", CultureInfo.InvariantCulture);
            }            
        }

        /// <summary>
        /// Gets the MAC Address of the Comm Module from a full table read. For automated
        /// testing.  Otherwise, use FormattedMACAddress.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/10 AF  2.45.05 162320 Created
        //
        public virtual string FormattedMACAddressFromFullRead
        {
            get
            {
                return MACAddressFromFullRead.ToString("X8", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  12/15/10 AF  2.45.20  163738  Function created.
        //
        public virtual DateTime? ITPTime
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  12/15/10 AF  2.45.20  163738  Function created.
        //
        public virtual DateTime? ITPTimeFromFullRead
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the MAC Address
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public virtual uint MACAddress
        {
            get
            {
                // While the MAC address is common to all Comm Modules the location it is stored is not
                // so we will return null here to indicate that it was not retrieved
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the MAC Address from a full table read. This property is for automated
        /// testing.  All other uses should use MACAddress.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/10 AF  2.45.05 162320 Created
        //
        public virtual uint MACAddressFromFullRead
        {
            get
            {
                // While the MAC address is common to all Comm Modules the location it is stored is not
                // so we will return null here to indicate that it was not retrieved
                return 0;
            }
        }

        /// <summary>
        /// Gets the multicast addresses for the Comm Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public List<string> MulticastAddresses
        {
            get
            {
                List<string> Addresses = null;

                if (Table122 != null)
                {
                    Addresses = Table122.MulticastAddresses;
                }

                return Addresses;
            }
        }

        /// <summary>
        /// Gets the exception host records for the Comm Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public ExceptionReportEntryRecord[] ExceptionHostRecords
        {
            get
            {
                ExceptionReportEntryRecord[] Records = null;

                if (Table123 != null)
                {
                    Records = Table123.ExceptionHostRecords;
                }

                return Records;
            }
        }

        /// <summary>
        /// The Utility ID from the Factory Config Table.  Setting this value
        /// will cause the meter to 3 button reset after logging off.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/22/09 jrf 2.30.12        Created
        // 02/17/10 RCG 2.40.15		   Moved From CENTRON_AMI

        public byte FactoryConfigUtilityID
        {
            get
            {
                if (Table2061RFLANConfig != null)
                {
                    return Table2061RFLANConfig.UtilityID;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (Table2061RFLANConfig != null)
                {
                    Table2061RFLANConfig.UtilityID = value;
                }
            }
        }

        /// <summary>
        ///  The RFLAN MAC address from the Factory Config Table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/14/11 AF  2.53.06        Created
        //
        public UInt32 FactoryConfigRFLANMACAddress
        {
            get
            {
                if (Table2061RFLANConfig != null)
                {
                    return Table2061RFLANConfig.RFLANMacAddress;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the RFLAN Identification from Mfg Table 17
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/11 AF  2.53.04        Created
        //
        public string CommModuleIdentification
        {
            get
            {
                string CommModId = "";

                if (Table2065 != null)
                {
                    CommModId = Table2065.RFLANIdentification;
                }

                return CommModId;
            }
        }

        /// <summary>
        /// Gets whether or not the Comm Module is currently registered.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/08/10 RCG 2.40.23	146449 Created

        public virtual bool IsRegistered
        {
            get
            {
                // If the Node aptitle is empty then we know the meter has not been registered
                // Unfortunately the meter does not always clear this item when deregistered.
                return String.IsNullOrEmpty(NodeAptitle) == false;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is synced to the network
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/08/10 RCG 2.40.23	146449 Created
        // 06/01/12 AF  2.60.28 197712 Refactored from the child classes
        //
        public bool IsNetSynced
        {
            get
            {
                return CommModuleSynchStatus.Equals("sync net");
            }
        }

        /// <summary>
        /// Gets the Comm Module Synch Status from Vender Field 2
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        // 06/01/12 AF  2.60.28 197712 Refactored from the child classes
        //
        public string CommModuleSynchStatus
        {
            get
            {
                string strValue = "";

                if (m_AMIDevice is ANSIMeter)
                {
                    strValue = ((ANSIMeter)m_AMIDevice).DisplayVenderField2;
                }

                // Since the option board display fields are meter displayable items the
                // text can be in mixed case - sometimes in a very unusual pattern.  So
                // the value is changed to lower case to make the string a little more readable
                strValue = strValue.ToLower(CultureInfo.CurrentCulture);

                return strValue;
            }
        }

        /// <summary>
        /// Determines if comm module has a MAC address. Method should be overridden by those 
        /// comm modules that do not support it.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public virtual bool HasMACAddress
        {
            get
            {
                // Defaulting to true since most comm modules have a mac address.
                return true;
            }
        }



        /// <summary>
        /// Gets the Registration Status from Vender Field 3
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        // 08/12/13 jrf 2.85.16 TQ8365 Moved to base class.
        //
        public string CommModuleRegistrationStatus
        {
            get
            {
                string strValue = "";

                if (m_AMIDevice is ANSIMeter)
                {
                    strValue = ((ANSIMeter)m_AMIDevice).DisplayVenderField3;
                }

                // Since the option board display fields are meter displayable items the
                // text can be in mixed case - sometimes in a very unusual pattern.  So
                // the value is changed to lower case to make the string a little more readable
                strValue = strValue.ToLower(CultureInfo.CurrentCulture);

                return strValue;
            }
        }

        /// <summary>
        /// Gets the Comm Module Level from the Vender field.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        // 04/07/14 AF  3.50.62 WR457827 Moved to base class to make available to RF Mesh
        //
        public string CommModuleLevel
        {
            get
            {
                string strValue = "";

                if (m_AMIDevice is ANSIMeter)
                {
                    strValue = ((ANSIMeter)m_AMIDevice).DisplayVenderField1;
                }

                // Since the option board display fields are meter displayable items the
                // text can be in mixed case - sometimes in a very unusual pattern.  So
                // the value is changed to lower case to make the string a little more readable
                strValue = strValue.ToLower(CultureInfo.CurrentCulture);

                // The meter returns "leuel #" since it cannot show a 'v'.  The users would like to see "level #",
                //  so change the 'u' to a 'v'.
                strValue = strValue.Replace('u', 'v'); // This is safe since there will only be one u in this string.

                return strValue;
            }
        }

        /// <summary>
        /// Gets whether or not the comm module radio is on
        /// </summary>
        public virtual bool? IsRadioOn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the Table 121 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Moved from CENTRON_AMI

        protected CStdTable121 Table121
        {
            get
            {
                if (null == m_Table121)
                {
                    m_Table121 = new CStdTable121(m_PSEM);
                }

                return m_Table121;
            }
        }

        /// <summary>
        /// Gets the Table 122 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Moved from CENTRON_AMI

        protected CStdTable122 Table122
        {
            get
            {
                if (null == m_Table122)
                {
                    m_Table122 = new CStdTable122(m_PSEM, Table121);
                }

                return m_Table122;
            }
        }

        /// <summary>
        /// Gets the Table 123 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Moved from CENTRON_AMI

        protected CStdTable123 Table123
        {
            get
            {
                if (null == m_Table123)
                {
                    m_Table123 = new CStdTable123(m_PSEM, Table121, m_AMIDevice.EventDescriptions);
                }

                return m_Table123;
            }
        }

        /// <summary>
        /// Gets the Table 126 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Moved from CENTRON_AMI
        // 04/27/10 AF  2.40.43        M2 Gateway is f/w version 1.0 but supports
        //                             the 2008 version of standard
        // 01/16/13 mah 2.70.58        Added support for SL7000 Gateway

        protected CStdTable126 Table126
        {
            get
            {
                if (null == m_Table126)
                {
                    // if (FWRevision < VERSION_1_5_RELEASE_1)
                    if ((VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_1_5_RELEASE_1) < 0) &&
                        !(m_AMIDevice is M2_Gateway) && !(m_AMIDevice is SL7000_Gateway))
                    {
                        m_Table126 = new CStdTable126(m_PSEM, Table121);
                    }
                    else
                    {
                        m_Table126 = new CStdTable126_2008(m_PSEM, Table121);
                    }
                }

                return m_Table126;
            }
        }

        /// <summary>
        /// Gets the Table 127 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Moved from CENTRON_AMI

        protected CStdTable127 Table127
        {
            get
            {
                if (null == m_Table127)
                {
                    m_Table127 = new CStdTable127(m_PSEM, Table121);
                }

                return m_Table127;
            }
        }

        /// <summary>
        /// Gets the RFLAN Config sub table object from the Factory Config Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/22/09 jrf 2.30.12		   Created
        // 02/17/10 RCG 2.40.15		   Moved from CENTRON_AMI

        protected MFGTable2061RFLANConfig Table2061RFLANConfig
        {
            get
            {
                if (null == m_Table2061RFLANConfig)
                {
                    m_Table2061RFLANConfig = new MFGTable2061RFLANConfig(m_PSEM);
                }

                return m_Table2061RFLANConfig;
            }
        }

        /// <summary>
        /// Gets the RFLAN Identification table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/11 AF  2.53.04        Created for Boron development
        //
        protected RFLANMfgTable2065 Table2065
        {
            get
            {
                if (null == m_Table2065)
                {
                    m_Table2065 = new RFLANMfgTable2065(m_PSEM);
                }

                return m_Table2065;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// The PSEM communications object for the current device.
        /// </summary>
        protected CPSEM m_PSEM;
        /// <summary>
        /// The current Device Object
        /// </summary>
        protected CANSIDevice m_AMIDevice;

        private CStdTable121 m_Table121;
        private CStdTable122 m_Table122;
        private CStdTable123 m_Table123;
        private CStdTable126 m_Table126;
        private CStdTable127 m_Table127;

        private MFGTable2061RFLANConfig m_Table2061RFLANConfig = null;
        private RFLANMfgTable2065 m_Table2065 = null;

        #endregion
    }
}
