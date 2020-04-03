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
//                           Copyright © 2010 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Comm Module object for an RFLAN module
    /// </summary>
    public class RFLANCommModule : CommModuleBase
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current sessions</param>
        /// <param name="amiDevice">The current device object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - replace CENTRON_AMI parameter 
        //                              with CANSIDevice
        //
        public RFLANCommModule(CPSEM psem, CANSIDevice amiDevice)
            : base(psem, amiDevice)
        {
            m_Table2068 = null;
            m_Table2078 = null;
            m_Table2113 = null;
            m_Table2121 = null;
        }

        /// <summary>
        /// This method sets the preferred cell ID for the RFLAN.  The comm module 
        /// will try for this cell for about 10 minutes before it will clear the cell
        /// ID out and try all cells.
        /// </summary>
        /// <param name="uiCellID">The preferred cell ID.</param>
        /// <returns>The result of the procedure.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/10 jrf 2.40.26	       Created
        // 02/17/14 jrf 3.00.35 460349 Updated method.
        public ProcedureResultCodes SetPreferredCellID(UInt32 uiCellID)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = new MemoryStream(5);
            byte[] ProcResponse;
            uint uiParam2 = 0;
            uint uiParam3 = 0;
           
            BinaryWriter ParamWriter = new BinaryWriter(ProcParam);

            ParamWriter.Write((byte)RFLANDebugProcedures.SET_PREFERRED_CELL);
            ParamWriter.Write(uiCellID);
            ParamWriter.Write(uiParam2);
            ParamWriter.Write(uiParam3);

            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.RFLAN_DEBUG_PROCEDURE,
                ProcParam.ToArray(), out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This method resets the C12.22 stack.  
        /// </summary>
        /// <returns>The result of the procedure.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/10 jrf 2.40.26	       Created
        public ProcedureResultCodes ResetC1222Stack()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)RFLANDebugProcedures.C1222_STACK_RESET;
            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.RFLAN_DEBUG_PROCEDURE,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This method forces a NET registration. 
        /// </summary>
        /// <returns>The result of the procedure.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/10 jrf 2.40.26	       Created
        public ProcedureResultCodes ForceNetRegistration()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)RFLANProcedure.FORCE_NET_REGISTER;
            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.RFLAN_PROCEDURE,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This method resets RFLAN MCU 
        /// </summary>
        /// <returns>The result of the procedure.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/08/10 MMD 2.41.07	       Created
        public ProcedureResultCodes ResetRFLANMCU()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)RFLANProcedure.RESET_RFLAN_MCU;
            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.RFLAN_PROCEDURE,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This method reads the RFLAN Info table, mfg. table 2068.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/15/12 jrf 2.70.30 240583 Created
        //
        public void RefreshRFLANInfo()
        {
            if (null != Table2068)
            {
                Table2068.Read();
            }
        }

        /// <summary>
        /// This method sets the RFLAN utility ID without writing to mfg. table 13 and 
        /// causing a 3-button reset.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/10/13 jrf 2.71.01 417021 Created
        //
        public ItronDeviceResult SetRFLANUtilityID(byte byUtilityID)
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            bool blnUtilityIDSet = false;

            //Either or both mfg. table 65/73 must be updated
            if (null != Table2113)
            {
                Table2113.UtilityID = byUtilityID;
                blnUtilityIDSet = true;
            }

            if (null != Table2121)
            {
                Table2121.UtilityID = byUtilityID;
                blnUtilityIDSet = true;
            }

            //Utility ID portion of native address must be updated.
            if (null != Table122)
            {
                Table122.UtilityID = byUtilityID;
            }
            else
            {
                blnUtilityIDSet = false;
            }

            if (true == blnUtilityIDSet)
            {
                Result = m_AMIDevice.ResetRFLAN();
            }

            return Result;
        }

        /// <summary>
        /// Sets the cell switch parameter selection in the expansion control bits of mfg table 73.
        /// We want to set bits 4 and 5 in the expansion control bits byte and leave the rest of the
        /// bits unchanged.  Of the 4 possible combinations, only 2 and 3 should be allowed.  Other
        /// values have not been tested.
        /// Hysteresis=400;Level=0;GPD=0;CSI=128 (00)
        /// Hysteresis=500;Level=64;GPD=1;CSI=128 (01)
        /// Hysteresis=400;Level=64;GPD=1;CSI=128 (10)
        /// Hysteresis=400;Level=0;GPD=1;CSI=128 (11)
        /// </summary>
        /// <param name="byCellSwitchParamByte">cell switch parameter value. Only 2 and 3 are allowed</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  06/30/14 AF  3.51.01 WR 518450 Created
        //
        public ItronDeviceResult SetRFLANLevel(byte byCellSwitchParamByte)
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte byExpansionControlBitsMask = 0xCF; // 11001111
            byte byExpansionControlBits = 0;

            if (null != Table2121)
            {
                byExpansionControlBits = Table2121.ExpansionControlBits;
                byExpansionControlBits &= byExpansionControlBitsMask;   // Clear out the cell switch parameter selection
                byExpansionControlBits |= (byte)(byCellSwitchParamByte << 4);         // Replace the cell switch parameter selection with the desired value

                Table2121.ExpansionControlBits = byExpansionControlBits;

                Result = m_AMIDevice.ResetRFLAN();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the SCM ERT ID from the RFLAN module.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/21/10 jrf 2.42.06		   Created

        public uint SCMERTID
        {
            get
            {
                uint uiSCMERTId = 0;

                if (Table2121 != null)
                {
                    try
                    {
                        uiSCMERTId = Table2121.SCMERTID;
                    }
                    catch (Exception)
                    {
                        // We could not read the SCM ERT ID.
                    }

                }

                return uiSCMERTId;
            }
        }

        /// <summary>
        /// Gets whether or not the RFLAN Module is using High Data Rate
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/10 RCG 2.40.28		   Created
        // 12/17/13 DLG 3.50.16        Updated to check for interface before calling properties.
        //
        public bool IsHighDataRate
        {
            get
            {
                bool bIsHDR = false;

                ICommModVersions CommModVer = m_AMIDevice as ICommModVersions;

                if (CommModVer != null)
                {
                    if (ITRL_DEVICE_CLASS.Equals(m_AMIDevice.CommModuleDeviceClass))
                    {
                        bIsHDR = CommModVer.CommModuleRevision >= 2;
                    }
                    else if (ITR2_DEVICE_CLASS.Equals(m_AMIDevice.CommModuleDeviceClass))
                    {
                        bIsHDR = CommModVer.CommModuleRevision == 9 || CommModVer.CommModuleRevision >= 12;
                    }
                }

                return bIsHDR;
            }
        }

        /// <summary>
        /// Gets the RFLAN MAC Address
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public override uint MACAddress
        {
            get
            {
                uint uiMAC = 0;

                if (Table2068 != null)
                {
                    try
                    {
                        uiMAC = Table2068.RFLANMACAddress;
                    }
                    catch (Exception)
                    {
                        // We could not read the MAC Address.
                    }
                }

                return uiMAC;
            }
            set
            {
                if ((VersionChecker.CompareTo(m_AMIDevice.HWRevision, CENTRON_AMI.HW_VERSION_2_0) < 0) && (m_AMIDevice.Table00.IsTableUsed(2113)))
                {
                    // The HW Version is less than 2, so we must set value in mfg. table 65

                    if (Table2113 != null)
                    {
                        Table2113.MacAddress = value;
                    }
                }
                else
                {
                    // The HW Version is greater than or equal to 2, so we must set value in 
                    // mfg. table 73
                    if (Table2121 != null)
                    {
                        Table2121.MacAddress = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the RFLAN MAC Address from a full table read rather than an offset read.
        /// This was written for automated testing. Otherwise, use MACAddress
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/10 AF  2.45.05 162320 Created
        //
        public override uint MACAddressFromFullRead
        {
            get
            {
                uint uiMAC = 0;

                if (Table2068 != null)
                {
                    try
                    {
                        uiMAC = Table2068.RFLANMACAddressFromFullRead;
                    }
                    catch (Exception)
                    {
                        // We could not read the MAC Address
                    }
                }

                return uiMAC;
            }
        }

        /// <summary>
        /// Property to retrieve the RFLAN MAC address without a table read.  This property expects
        /// an independant table read to have already been performed by calling RefreshRFLANInfo()
        /// first.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/15/12 jrf 2.70.30 240583    Created
        //
        public uint MACAddressNoRead
        {
            get
            {
                uint uiMAC = 0;

                if (Table2068 != null)
                {
                    try
                    {
                        uiMAC = Table2068.RFLANMACAddressNoRead;
                    }
                    catch (Exception)
                    {
                        // We could not read the MAC Address
                    }
                }

                return uiMAC;
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
        public override DateTime? ITPTime
        {
            get
            {
                if (Table2068 != null)
                {
                    try
                    {
                        return Table2068.ITP;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

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
        public override DateTime? ITPTimeFromFullRead
        {
            get
            {
                if (Table2068 != null)
                {
                    try
                    {
                        return Table2068.ITPFromFullRead;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets whether or not the Comm Module is currently registered.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/08/10 RCG 2.40.23	146449 Created

        public override bool IsRegistered
        {
            get
            {
                // The more accurate way to read this is to check the registration
                // status which is in Vender Display Data for Itron Comm Modules
                return CommModuleRegistrationStatus.Equals("reg");
            }
        }

        /// <summary>
        /// Gets whether or not the Comm Module is net registered.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/10 jrf 2.40.26	       Created
        public bool IsNetRegistered
        {
            get
            {
                bool blnNetRegistered = false;

                if (null != Table2068)
                {
                    blnNetRegistered = Table2068.IsNetRegistered;
                }

                return blnNetRegistered;
            }
        }

        /// <summary>
        /// Gets Net Routing Count.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/19/10 MMD 2.41.02	       Created
        public int NetRoutingCount
        {
            get
            {
                int iNetCount = 0;

                if (null != Table2068)
                {
                    iNetCount = Table2068.NetRoutingCount;
                }

                return iNetCount;
            }
        }

        /// <summary>
        /// Property to retrieve the RFLAN level directly instead of through the display 
        /// vendor field 1.  It should not be used elsewhere unless a conscience decision is
        /// made to use this value instead of the level returned from the vendor field.  Using 
        /// it could create a discrepancy in the level shown to the user.  Use the property, 
        /// CommModuleLevel, instead.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/29/09 jrf 2.30.04        Created.
        //  02/17/10 RCG 2.40.15		Moved from CENTRON_AMI
        //  10/15/12 jrf 2.70.30 240583 Renamed property.
        //
        public byte RFLANLevelFromOffsetRead
        {
            get
            {
                byte byValue = 0;

                if (Table2068 != null)
                {
                    byValue = Table2068.RFLANLevelFromOffsetRead;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Property to retrieve the RFLAN level without a table read.  This property expects
        /// an independant table read to have already been performed by calling RefreshRFLANInfo()
        /// first.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/15/12 jrf 2.70.30 240583 Created
        //
        public byte RFLANLevelNoRead
        {
            get
            {
                byte byValue = 0;

                if (Table2068 != null)
                {
                    byValue = Table2068.RFLANLevelNoRead;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the RFLAN neighbor list.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public List<RFLANNeighborEntryRcd> RFLANNeighbors
        {
            get
            {
                List<RFLANNeighborEntryRcd> Neighbors = null;

                if (Table2078 != null)
                {
                    Neighbors = Table2078.Neighbors;
                }

                return Neighbors;
            }
        }

        /// <summary>
        /// The Utility ID from the RFLAN Factory Config Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/22/09 jrf 2.30.12        Created
        // 02/17/10 RCG 2.40.15		   Moved from CENTRON_AMI
        // 02/11/14 AF  3.00.34 WR428639 The M2 Gateway has hw version < 2.0 but it supports mfg table 73,
        //                              not mfg 65
        // 02/18/14 AF  3.00.36 WR428639 Added the IsTableUsed check to the set
        //
        public byte? RFLANFactoryConfigUtilityID
        {
            get
            {
                if ((VersionChecker.CompareTo(m_AMIDevice.HWRevision, CENTRON_AMI.HW_VERSION_2_0) < 0) && (m_AMIDevice.Table00.IsTableUsed(2113)))
                {
                    // The HW Version is less than 2, so we must check mfg. table 65
                    if (Table2113 != null)
                    {
                        try
                        {
                            return Table2113.UtilityID;
                        }
                        catch { return null; }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    // The HW Version is greater than or equal to 2, so we must check mfg. table 73
                    if (Table2121 != null)
                    {
                        try
                        {
                            return Table2121.UtilityID;
                        }
                        catch { return null; }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                if ((VersionChecker.CompareTo(m_AMIDevice.HWRevision, CENTRON_AMI.HW_VERSION_2_0) < 0)  && (m_AMIDevice.Table00.IsTableUsed(2113)))
                {
                    // The HW Version is less than 2, so we must set value in mfg. table 65

                    if (Table2113 != null)
                    {
                        Table2113.UtilityID = value.Value;
                    }
                }
                else
                {
                    // The HW Version is greater than or equal to 2, so we must set value in 
                    // mfg. table 73
                    if (Table2121 != null)
                    {
                        Table2121.UtilityID = value.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the cell switch parameter selection from mfg table 73.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  06/30/14 AF  3.51.01 WR 518450 Created
        //
        public byte RFLANCellSwitchParameterSelection
        {
            get
            {
                byte byCellSwitchParameterSelection = 0;
                byte byExpansionControlBitsMask = 0x30; // 00110000

                if (null != Table2121)
                {
                    byCellSwitchParameterSelection = (byte)((Table2121.ExpansionControlBits & byExpansionControlBitsMask) >> 4);
                }

                return byCellSwitchParameterSelection;
            }
            set
            {
                byte byExpansionControlBitsMask = 0xCF; // 11001111
                byte byExpansionControlBits = 0;

                if (null != Table2121)
                {
                    byExpansionControlBits = Table2121.ExpansionControlBits;
                    byExpansionControlBits &= byExpansionControlBitsMask;   // Clear out the cell switch parameter selection
                    byExpansionControlBits |= (byte)(value << 4);         // Replace the cell switch parameter selection with the desired value

                    Table2121.ExpansionControlBits = byExpansionControlBits;

                }
            }
        }

        /// <summary>
        /// Gets whether or not the comm module radio is on
        /// </summary>
        public override bool? IsRadioOn
        {
            get
            {
                bool? IsOn = null;
                if (RFLANFactoryConfigUtilityID.HasValue)
                {
                    IsOn = (0 != RFLANFactoryConfigUtilityID.Value);
                }

                return IsOn;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the RFLAN Information table object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        // 12/20/10 AF  2.45.20 163738 Changed the input parameter for table 2068 to 
        //                             RFLAN f/w version
        // 10/08/12 jrf 2.61.00 273251 Switching IsInChoiceConnectMode check to check 
        //                             if meter is MSM meter then only create table 
        //                             when operating in OpenWay mode.
        // 12/02/13 jrf 3.50.10        Renaming IChoiceConnect interface to IBridge.
        // 12/17/13 DLG 3.50.16        Updated to use ICommModVersions interface.
        //
        private RFLANMfgTable2068 Table2068
        {
            get
            {
                bool blnCreateTable = true;
                IBridge ChoiceConnectDevice = m_AMIDevice as IBridge;

                if (m_AMIDevice is IBridge && ((IBridge)m_AMIDevice).CurrentRegisterCommOpMode != OpenWayMFGTable2428.ChoiceConnectCommOpMode.OpenWayOperationalMode)
                {
                    //Do not create table if meter is MSM and is not in OpenWay operation mode.
                    blnCreateTable = false;
                }

                ICommModVersions CommModVers = m_AMIDevice as ICommModVersions;

                if (m_Table2068 == null && true == blnCreateTable && CommModVers != null)
                {
                    string strRFLANVer = CommModVers.CommModVer + "." + CommModVers.CommModBuild;
                    m_Table2068 = new RFLANMfgTable2068(m_PSEM, strRFLANVer);
                }

                return m_Table2068;
            }
        }

        /// <summary>
        /// Gets the Table CRFLANMfgTable2078 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/08 jrf 1.50.23	114066 Created
        // 02/17/10 RCG 2.40.15		   Moved from CENTRON_AMI

        private OpenWayMfgTable2078 Table2078
        {
            get
            {
                if (null == m_Table2078)
                {
                    if (IsHighDataRate)
                    {
                        m_Table2078 = new OpenWayMfgTable2078HDR(m_PSEM);
                    }
                    else
                    {
                        m_Table2078 = new OpenWayMfgTable2078(m_PSEM);
                    }
                }

                return m_Table2078;
            }
        }

        /// <summary>
        /// Gets the Reg Copy RFLAN Factory Config Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        private RFLANMFGTable2113 Table2113
        {
            get
            {
                if (m_Table2113 == null)
                {
                    m_Table2113 = new RFLANMFGTable2113(m_PSEM);
                }

                return m_Table2113;
            }
        }

        /// <summary>
        /// Gets the RFLAN Factory Config Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        // 12/17/13 DLG 3.50.16        Updated to use ICommModVersions interface.
        //
        private RFLANMFGTable2121 Table2121
        {
            get
            {
                if (m_Table2121 == null)
                {
                    ICommModVersions CommModVers = m_AMIDevice as ICommModVersions;

                    if (CommModVers != null)
                    {
                        m_Table2121 = new RFLANMFGTable2121(m_PSEM, CommModVers.CommModuleRevision);                    
                    }
                }

                return m_Table2121;
            }
        }

        #endregion

        #region Member Variables

        private RFLANMfgTable2068 m_Table2068;
        private OpenWayMfgTable2078 m_Table2078;
        private RFLANMFGTable2113 m_Table2113;
        private RFLANMFGTable2121 m_Table2121;

        #endregion
    }
}
