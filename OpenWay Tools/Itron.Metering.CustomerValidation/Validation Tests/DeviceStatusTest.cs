///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                           Copyright © 2009 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.IO;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;
using Itron.Metering.DST;
using Itron.Metering.ReplicaSettings;
using Itron.Metering.Utilities;
using Itron.Metering.Datafiles;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Validation test the checks the device status
    /// </summary>
    public class DeviceStatusTest : ValidationTest
    {
        #region Constants

        private const string REPLICA = "OpenWay Replica";
        private const string FIRMWARE_FOLDER = @"Firmware\";
        private const byte PRISM_LITE_HW_MASK = 0x80;
        private readonly string FW_PATH = CRegistryHelper.GetFilePath(REPLICA) + FIRMWARE_FOLDER;
        private const string RFLAN_FW_0_009_044 = "0.009.044";
        private const string CG_MESH_MODULE = "CISCO CGMesh Module ";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comm">The communications object.</param>
        /// <param name="uiBaudRate">The baud rate to use.</param>
        /// <param name="programFile">The program file to use for the test.</param>
        /// <param name="authKey">The signed authorization key to use.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 removed unnecessary device class paramater.
        public DeviceStatusTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
            : base(comm, uiBaudRate, programFile, authKey)
        {
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created
        //  12/09/09 jrf 2.30.26        Removing upper nibble of hardware version before 
        //                              displaying for PrismLite devices.
        //  02/19/13 AF  2.70.69 322427 Show InterPAN Mode only for pre-Lithium meters
        //  09/19/14 jrf 4.00.63 WR 534158 Using new method to set test result string and 
        //                                 modified way test details are set.
        //  10/02/14 jrf 4.00.66 WR 431248 Making sure if logon is called and exception occurs then a logoff is 
        //                                 attempted so other tests may continue if possible.
        public override Test RunTest()
        {
            PSEMResponse Response = PSEMResponse.Ok;
            string strValue = "";
            m_TestResults = new Test();
            m_TestResults.Name = TestName;


            m_bTestPassed = true;

            try
            {
                Response = LogonToDevice();

                if (Response == PSEMResponse.Ok)
                {
                    if (IsAborted == false)
                    {
                        AddTestDetail(TestResources.MeterID, TestResources.OK, m_AmiDevice.UnitID);
                        AddTestDetail(TestResources.SerialNumber, TestResources.OK, m_AmiDevice.SerialNumber);
                        AddTestDetail(TestResources.MFGSerialNumber, TestResources.OK, m_AmiDevice.MFGSerialNumber);


                        if (m_AmiDevice.CommModule != null)
                        {
                            strValue = m_AmiDevice.CommModule.ElectronicSerialNumber;
                        }
                        else
                        {
                            strValue = TestResources.NotAvailable;
                        }

                        AddTestDetail(TestResources.ElectronicSerialNumber, TestResources.OK, strValue);
                    }

                    if (IsAborted == false)
                    {
                        CheckRegisterFWVersion();
                        CheckRFLANFWVersion();
                        CheckZigBeeFWVersion();
                        CheckDisplayFWVersion();

                        float fltHWRev = m_AmiDevice.HWRevision;

                        // If this is a PrismLite device we need to ignore the upper nibble of the hardware version
                        if (((byte)(fltHWRev) & PRISM_LITE_HW_MASK) == PRISM_LITE_HW_MASK)
                        {
                            fltHWRev -= (float)PRISM_LITE_HW_MASK;
                        }

                        AddTestDetail(TestResources.HardwareVersion, TestResources.OK, fltHWRev.ToString("F3", CultureInfo.CurrentCulture));
                    }

                    if (IsAborted == false)
                    {
                        AddTestDetail(TestResources.TimeZone, TestResources.OK, m_AmiDevice.TimeZoneOffset.Hours + ":00");
                        AddTestDetail(TestResources.DeviceTime, TestResources.OK, m_AmiDevice.DeviceTime.ToString("G", CultureInfo.CurrentCulture));
                        AddTestDetail(TestResources.TOUEnabled, TestResources.OK, m_AmiDevice.TOUEnabled.ToString(CultureInfo.CurrentCulture));
                        AddTestDetail(TestResources.DSTEnabled, TestResources.OK, m_AmiDevice.DSTEnabled.ToString(CultureInfo.CurrentCulture));


                        for (int iIndex = 0; iIndex < m_AmiDevice.DST.Count; iIndex++)
                        {
                            AddTestDetail(TestResources.DSTFromDate + (iIndex + 1).ToString(CultureInfo.CurrentCulture),
                                TestResources.OK, m_AmiDevice.DST[iIndex].FromDate.ToString("G", CultureInfo.CurrentCulture));
                            AddTestDetail(TestResources.DSTToDate + (iIndex + 1).ToString(CultureInfo.CurrentCulture),
                                TestResources.OK, m_AmiDevice.DST[iIndex].ToDate.ToString("G", CultureInfo.CurrentCulture));
                        }
                    }

                    if (IsAborted == false)
                    {
                        AddTestDetail(TestResources.MinutesOnBattery, TestResources.OK, m_AmiDevice.NumberOfMinutesOnBattery.ToString(CultureInfo.CurrentCulture));
                        AddTestDetail(TestResources.DateLastProgrammed, TestResources.OK, m_AmiDevice.DateProgrammed.ToString("G", CultureInfo.CurrentCulture));
                        AddTestDetail(TestResources.DateOfLastDemandReset, TestResources.OK, m_AmiDevice.DateLastDemandReset.ToString("G", CultureInfo.CurrentCulture));
                        AddTestDetail(TestResources.DateOfLastOutage, TestResources.OK, m_AmiDevice.DateLastOutage.ToString("G", CultureInfo.CurrentCulture));
                        AddTestDetail(TestResources.DateOfLastTest, TestResources.OK, m_AmiDevice.DateLastTestMode.ToString("G", CultureInfo.CurrentCulture));


                        if ((m_AmiDevice is OpenWayBasicPoly == true) || (m_AmiDevice is OpenWayAdvPoly == true))
                        {
                            AddTestDetail(TestResources.NormalKh, TestResources.OK, (m_AmiDevice.NormalKh / 40.0).ToString("F3", CultureInfo.CurrentCulture));
                        }
                        else
                        {
                            AddTestDetail(TestResources.NormalKh, TestResources.OK, (m_AmiDevice.NormalKh / 10.0).ToString("F3", CultureInfo.CurrentCulture));
                        }


                    }

                    if (IsAborted == false)
                    {
                        CheckLPRunning();

                        CheckDeviceErrors();

                        GetRFLANMac();

                        if (m_AmiDevice.CommModule != null)
                        {
                            RFLANCommModule RFLANModule = m_AmiDevice.CommModule as RFLANCommModule;

                            if (RFLANModule != null)
                            {
                                strValue = RFLANModule.CommModuleLevel;
                            }
                            else
                            {
                                strValue = TestResources.NotAvailable;
                            }
                        }
                        else
                        {
                            strValue = TestResources.NotAvailable;
                        }

                        AddTestDetail(TestResources.RFLANSynchLevel, TestResources.OK, strValue);
                    }

                    if (IsAborted == false)
                    {
                        AddTestDetail(TestResources.HANMACAddress, TestResources.OK, m_AmiDevice.HANServerMACAddr);
                        AddTestDetail(TestResources.HANSecurityProfile, TestResources.OK, m_AmiDevice.HANSecurityProfile);

                        if (VersionChecker.CompareTo(m_AmiDevice.FWRevision, CENTRON_AMI.VERSION_3_12_LITHIUM) < 0)
                        {
                            AddTestDetail(TestResources.InterPANMode, TestResources.OK, m_AmiDevice.InterPANMode);
                        }

                        AddTestDetail(TestResources.ZigBeeEnabled, TestResources.OK, ConvertYesOrNo(m_AmiDevice.IsZigBeeEnabled));
                        AddTestDetail(TestResources.ANSIC1218OverZigBeeEnabled, TestResources.OK, ConvertYesOrNo(m_AmiDevice.IsC1218OverZigBeeEnabled));
                        AddTestDetail(TestResources.ZigBeePrivateProfileEnabled, TestResources.OK, ConvertYesOrNo(m_AmiDevice.IsZigBeePrivateProfileEnabled));

                        AddTestDetail(TestResources.EnhancedBlurtsEnabled, TestResources.OK, ConvertYesOrNo(m_AmiDevice.MeterKey_EnhancedBlurtsSupported));
                        AddTestDetail(TestResources.AdvancedPolyMeter, TestResources.OK, ConvertYesOrNo(m_AmiDevice.MeterKey_AdvancedPolySupported));
                    }
                }
                else
                {
                    m_TestResults.Reason = TestResources.ReasonLogonFailed;
                    m_bTestPassed = false;
                }

            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (m_AmiDevice != null)
                {
                    m_AmiDevice.Logoff();
                }
            }

            // Set the final result.
            m_TestResults.Result = GetTestResultString(m_bTestSkipped, m_bTestPassed);

            return m_TestResults;
        }

        /// <summary>
        /// Checks the Load Profile running flag and verifies that it should be running.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void CheckLPRunning()
        {
            string MeterValue = ConvertYesOrNo(m_AmiDevice.LoadProfileStatus.IsRunning);
            string ExpectedValue = "";
            string Result = "";
            string Reason = "";
            string Details = "";
            bool Skipped = true;

            Details = MeterValue;
            
            if (File.Exists(m_strProgramFile) && EDLFile.IsEDLFile(m_strProgramFile))
            {
                try
                {
                    EDLFile ProgramFile = new EDLFile(m_strProgramFile);

                    ExpectedValue = ConvertYesOrNo(ProgramFile.LPQuantityList.Count > 0);
                    Skipped = false;

                    if (MeterValue.Equals(ExpectedValue))
                    {
                        Details += ", " + TestResources.ProgramMatch;
                    }
                    else
                    {
                        Details += ", " + TestResources.ProgramMismatch;
                    }
                }
                catch (Exception)
                {
                    // This file must not be a program file.
                    Reason = TestResources.ReasonInvalidProgramFile;
                    Details += ", " + TestResources.InvalidProgram;
                }
            }
            else
            {
                Reason = TestResources.ReasonNoProgramFile;
                Details += ", " + TestResources.NoProgram;
            }

            Result = GetResultString(Skipped, MeterValue.Equals(ExpectedValue));

            AddTestDetail(TestResources.LoadProfileRunning, Result, Details, Reason);
        }

        /// <summary>
        /// Gets the RFLAN MAC address from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  11/05/10 AF  2.45.10 CQ 164157 RFLAN version 0.9.44 has a bug that prevents an offset read of
        //                                 Mfg table 20, which causes us to report the RFLAN MAC addr. as 0.
        //                                 To work around, do a full read of table 20 to get the MAC addr.
        //  09/23/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void GetRFLANMac()
        {
            string RFLANMac = TestResources.NotAvailable;
            bool Passed = true;

            try
            {
                if (m_AmiDevice.CommModule != null)
                {
                    if (m_AmiDevice.CommModule.MACAddress != 0)
                    {
                        RFLANMac = m_AmiDevice.CommModule.FormattedMACAddress;
                    }
                    else
                    {
                        if ((m_AmiDevice.CommModVer + "." + m_AmiDevice.CommModBuild) == RFLAN_FW_0_009_044)
                        {
                            RFLANMac = m_AmiDevice.CommModule.FormattedMACAddressFromFullRead;
                        }
                        else
                        {
                            // The full read of Mfg table 20 was designed for RFLAN 0.9.44.
                            // If that's not the version we are talking to, we'd better not try
                            // a full read.
                            RFLANMac = m_AmiDevice.CommModule.FormattedMACAddress;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Passed = false;
                RFLANMac = ", " + TestResources.CannotReadValue;
            }

            AddTestDetail(TestResources.RFLANMACAddress, GetResultString(Passed), RFLANMac);
        }

        /// <summary>
        /// Checks the Register FW version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created        
        //  09/23/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void CheckRegisterFWVersion()
        {
            XMLOpenWayActiveFiles ActiveFiles = new XMLOpenWayActiveFiles();
            FirmwareSet MeterFWSet = ActiveFiles.GetFirmwareSet(FirmwareSet.DetermineMeterType(m_AmiDevice.DeviceClass, 
                m_AmiDevice.HWRevision, m_AmiDevice.MeterKey_TransparentDeviceSupported));

            string MeterValue = m_AmiDevice.FWRevision.ToString("F3", CultureInfo.CurrentCulture) 
                + "." + m_AmiDevice.FirmwareBuild.ToString("d3", CultureInfo.CurrentCulture);
            string ExpectedValue = "";
            string Result = "";
            string Details = "";
            string Reason = "";

            if (MeterFWSet != null && File.Exists(FW_PATH + MeterFWSet.RegisterFWFile))
            {
                CENTRON_AMI_FW_File FWFile = new CENTRON_AMI_FW_File(FW_PATH + MeterFWSet.RegisterFWFile);
                ExpectedValue = FWFile.CompleteVersion;
            }
            else
            {
                Reason = TestResources.ReasonNoFirmwareFile;
            }

            // Get the result string. We skip if the expected value is empty string. Pass if the values are equal
            Result = GetResultString(String.IsNullOrEmpty(ExpectedValue), MeterValue.Equals(ExpectedValue));

            Details = GetFWVersionDetails(MeterValue, ExpectedValue);

            AddTestDetail(TestResources.RegisterFirmwareVersion, Result, Details, Reason);
        }        

        /// <summary>
        /// Checks the RFLAN FW Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created        
        //  09/23/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        //  11/03/14 jrf 4.00.84 WR 542970 Making sure correct type of comm module firmware version is checked.
        private void CheckRFLANFWVersion()
        {
            XMLOpenWayActiveFiles ActiveFiles = new XMLOpenWayActiveFiles();
            FirmwareSet MeterFWSet = ActiveFiles.GetFirmwareSet(FirmwareSet.DetermineMeterType(m_AmiDevice.DeviceClass,
                m_AmiDevice.HWRevision, m_AmiDevice.MeterKey_TransparentDeviceSupported));

            string MeterValue = m_AmiDevice.CommModVer + "." + m_AmiDevice.CommModBuild;
            string ExpectedValue = "";
            string Result = "";
            string Details = "";
            string Reason = "";
            string FileName = "";

            if (MeterFWSet != null)
            {
                //Determine which comm module firmware type we are looking for.
                switch (m_AmiDevice.CommModuleDeviceClass)
                {
                    case CommModuleBase.ITR2_DEVICE_CLASS:
                    {
                        string strCommModuleID = null;

                        try
                        {
                            if (null != m_AmiDevice.CommModule)
                            {
                                strCommModuleID = m_AmiDevice.CommModule.CommModuleIdentification;
                            }
                        }
                        catch (PSEMException)
                        {
                            // This read may fail on some meters whose Comm Module is down
                        }

                        if (String.CompareOrdinal(strCommModuleID, CG_MESH_MODULE) == 0)
                        {
                            FileName = MeterFWSet.CiscoCommFWFile;
                        }
                        else if (m_AmiDevice is IBridge 
                            && ((IBridge)m_AmiDevice).CurrentRegisterCommOpMode == OpenWayMFGTable2428.ChoiceConnectCommOpMode.ChoiceConnectOperationalMode)
                        {
                            MeterValue = ((IBridge)m_AmiDevice).ChoiceConnectFWVerRev + "." + ((IBridge)m_AmiDevice).ChoiceConnectFWBuild;

                            FileName = MeterFWSet.ChoiceConnectFWFile;
                        }
                        else
                        {
                            FileName = MeterFWSet.LANFWFile;
                        }
                        break;
                    }
                    case CommModuleBase.ITRL_DEVICE_CLASS:
                    {
                        FileName = MeterFWSet.LANFWFile;
                        break;
                    }
                    case CommModuleBase.ITRP_DEVICE_CLASS:
                    {
                        FileName = MeterFWSet.PLANFWFile;
                        break;
                    }
                    case CENTRON_AMI.ITRH_DEVICE_CLASS:
                    case CENTRON_AMI.ITRU_DEVICE_CLASS:
                    case CENTRON_AMI.ITRV_DEVICE_CLASS:
                    {
                        FileName = MeterFWSet.ICSFWFile;
                        break;
                    }
                    case CENTRON_AMI.ITRS_DEVICE_CLASS:
                    {
                        FileName = MeterFWSet.CiscoCommFWFile;
                        break;
                    }
                    default:
                    {
                        FileName = MeterFWSet.LANFWFile;
                        break;
                    }
                }

                if (File.Exists(FW_PATH + FileName))
                {
                    CENTRON_AMI_FW_File FWFile = new CENTRON_AMI_FW_File(FW_PATH + FileName);
                    ExpectedValue = FWFile.CompleteVersion;
                }
                else
                {
                    Reason = TestResources.ReasonNoFirmwareFile;
                }
            }

            // Get the result string. We skip if the expected value is empty string. Pass if the values are equal
            Result = GetResultString(String.IsNullOrEmpty(ExpectedValue), MeterValue.Equals(ExpectedValue));

            Details = GetFWVersionDetails(MeterValue, ExpectedValue);

            AddTestDetail(TestResources.RFLANFirmwareVersion, Result, Details, Reason);
        }

        /// <summary>
        /// Checks the ZigBee FW Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created        
        //  09/23/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void CheckZigBeeFWVersion()
        {
            XMLOpenWayActiveFiles ActiveFiles = new XMLOpenWayActiveFiles();
            FirmwareSet MeterFWSet = ActiveFiles.GetFirmwareSet(FirmwareSet.DetermineMeterType(m_AmiDevice.DeviceClass,
                m_AmiDevice.HWRevision, m_AmiDevice.MeterKey_TransparentDeviceSupported));

            string MeterValue = "";
            string ExpectedValue = "";
            string Result = "";
            string Details = "";
            string Reason = "";

            // Hardware 3.0 meters do not have ZigBee FW
            if (VersionChecker.CompareTo(m_AmiDevice.HWRevision, CENTRON_AMI.HW_VERSION_3_0) < 0)
            {
                MeterValue = m_AmiDevice.HanModVer + "." + m_AmiDevice.HanModBuild;

                if (MeterFWSet != null && File.Exists(FW_PATH + MeterFWSet.ZigBeeFWFile))
                {
                    CENTRON_AMI_FW_File FWFile = new CENTRON_AMI_FW_File(FW_PATH + MeterFWSet.ZigBeeFWFile);
                    ExpectedValue = FWFile.CompleteVersion;
                }
                else
                {
                    Reason = TestResources.ReasonNoFirmwareFile;
                }

                // Get the result string. We skip if the expected value is empty string. Pass if the values are equal
                Result = GetResultString(String.IsNullOrEmpty(ExpectedValue), MeterValue.Equals(ExpectedValue));

                Details = GetFWVersionDetails(MeterValue, ExpectedValue);

                AddTestDetail(TestResources.ZigBeeFirmwareVersion, Result, Details, Reason);
            }
        }

        /// <summary>
        /// Checks the display FW Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created        
        //  09/23/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void CheckDisplayFWVersion()
        {
            // Only check this value if it's HW 2.0 or later and earlier than HW 3.0
            if (VersionChecker.CompareTo(m_AmiDevice.HWRevision, CENTRON_AMI.HW_VERSION_2_0) >= 0 
                && VersionChecker.CompareTo(m_AmiDevice.HWRevision, CENTRON_AMI.HW_VERSION_3_0) < 0)
            {
                XMLOpenWayActiveFiles ActiveFiles = new XMLOpenWayActiveFiles();
                FirmwareSet MeterFWSet = ActiveFiles.GetFirmwareSet(FirmwareSet.DetermineMeterType(m_AmiDevice.DeviceClass,
                    m_AmiDevice.HWRevision, m_AmiDevice.MeterKey_TransparentDeviceSupported));

                string MeterValue = m_AmiDevice.DisplayModVer + "." + m_AmiDevice.DisplayModBuild;
                string ExpectedValue = "";
                string Result = "";
                string Details = "";
                string Reason = "";

                if (MeterFWSet != null && File.Exists(FW_PATH + MeterFWSet.DisplayFWFile))
                {
                    CENTRON_AMI_FW_File FWFile = new CENTRON_AMI_FW_File(FW_PATH + MeterFWSet.DisplayFWFile);
                    ExpectedValue = FWFile.CompleteVersion;
                }
                else
                {
                    Reason = TestResources.ReasonNoFirmwareFile;
                }

                // Get the result string. We skip if the expected value is empty string. Pass if the values are equal
                Result = GetResultString(String.IsNullOrEmpty(ExpectedValue), MeterValue.Equals(ExpectedValue));

                Details = GetFWVersionDetails(MeterValue, ExpectedValue);

                AddTestDetail(TestResources.DisplayFirmwareVersion, Result, Details, Reason);
            }
        }

        /// <summary>
        /// Checks for device errors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created        
        //  09/23/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void CheckDeviceErrors()
        {
            string[] ErrorList = m_AmiDevice.ErrorsList;

            if (ErrorList != null && ErrorList.Length > 0)
            {
                for (int iIndex = 0; iIndex < ErrorList.Length; iIndex++)
                {
                    AddTestDetail(ErrorList[iIndex], GetResultString(false), TestResources.ErrorPresent);
                }
            }
            else
            {
                AddTestDetail(TestResources.DeviceErrors, GetResultString(true), TestResources.NonePresent);
            }
        }

        /// <summary>
        /// Displays the details for the FW version check results.
        /// </summary>
        /// <param name="actualVersion">Meter's FW version.</param>
        /// <param name="expectedVersion">Active FW file's version</param>
        /// <returns>Details of the FW version check results.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/23/14 jrf 4.00.63 WR 534158 Created.
        private string GetFWVersionDetails(string actualVersion, string expectedVersion)
        {
            string Details = actualVersion;

            if (String.IsNullOrEmpty(expectedVersion))
            {
                Details += ", " + TestResources.NoActiveFile;
            }
            else if (actualVersion.Equals(expectedVersion))
            {
                Details += ", " + TestResources.MatchesActiveVersion;
            }
            else
            {
                Details += ", " + TestResources.MismatchWithActiveVersion + " (" + expectedVersion + ")";
            }

            return Details;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the type of the test.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        public override ValidationTestID TestID
        {
            get 
            { 
                return ValidationTestID.DeviceStatus; 
            }
        }

        #endregion
    }
}
