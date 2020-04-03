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
//                              Copyright © 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Device;
using Itron.Metering.ZigBeeRadioServerObjects;
using Itron.Metering.Zigbee;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Validation test that verifies the ZigBee radio is working correctly
    /// </summary>
    public class ZigBeeRadioTest : ValidationTest
    {
        #region Constants

        private TimeSpan MAX_WAIT_TIME = new TimeSpan(0, 3, 0);

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
        public ZigBeeRadioTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
            : base(comm, uiBaudRate, programFile, authKey)
        {
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Using new method to set test result string and 
        //                                 modified way test details are set.
        //  10/02/14 jrf 4.00.66 WR 431248 Making sure if logon is called and exception occurs then a logoff is 
        //                                 attempted so other tests may continue if possible.
        public override Test RunTest()
        {
            PSEMResponse Response = PSEMResponse.Ok;

            m_TestResults = new Test();
            m_TestResults.Name = TestName;

            m_bTestPassed = true;
            m_ulMACAddress = 0;
            m_byChannel = 0;
            m_bC1218OverZigBeeEnabled = false;

            try
            {

                // First we need to log on and get the HAN MAC address, channel, and ZigBee state optically
                Response = LogonToDevice();

                if (Response == PSEMResponse.Ok)
                {
                    try
                    {
                        m_ulMACAddress = m_AmiDevice.HANMACAddress;
                        AddTestDetail(TestResources.HANMACAddress, TestResources.OK, m_ulMACAddress.ToString("X16", CultureInfo.InvariantCulture));

                        m_byChannel = m_AmiDevice.HANChannel;
                        AddTestDetail(TestResources.HANChannel, TestResources.OK, m_byChannel.ToString(CultureInfo.InvariantCulture));

                        m_bJoiningEnabled = m_AmiDevice.IsHANJoiningEnabled;
                        AddTestDetail(TestResources.HANJoiningEnabled, TestResources.OK, ConvertYesOrNo(m_bJoiningEnabled));

                        m_bC1218OverZigBeeEnabled = m_AmiDevice.IsC1218OverZigBeeEnabled;
                        AddTestDetail(TestResources.SupportsANSIC1218OverZigBee, TestResources.OK, ConvertYesOrNo(m_bC1218OverZigBeeEnabled));

                        m_bZigBeeEnabled = m_AmiDevice.IsZigBeeEnabled;
                        AddTestDetail(TestResources.ZigBeeEnabled, TestResources.OK, ConvertYesOrNo(m_bZigBeeEnabled));

                        m_bPrivateProfileEnabled = m_AmiDevice.IsZigBeePrivateProfileEnabled;
                        AddTestDetail(TestResources.ZigBeePrivateProfileEnabled, TestResources.OK, ConvertYesOrNo(m_bPrivateProfileEnabled));
                    }
                    catch (Exception)
                    {
                        m_TestResults.Reason = TestResources.ReasonFailedToGetHANInfo;
                        m_bTestPassed = false;
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

            if (true == m_bTestPassed)
            {
                // Try to log on via ZigBee.
                TestZigBeeConnection();
            }
            else
            {
                AddTestDetail(TestResources.AllowedANSIC1218Connection, GetResultString(true, false), m_TestResults.Reason);
            }

            // Set the final result.
            m_TestResults.Result = GetTestResultString(m_bTestSkipped, m_bTestPassed);

            // This test like to cause communications problems with tests that follow
            // Giving the meter a little time to recover should help.
            Thread.Sleep(10000);

            return m_TestResults;
        }

        /// <summary>
        /// Tests the ZigBee Connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way test details are set
        //  10/03/14 jrf 4.00.67 WR 536944 Corrected expect logon success check. It doesn't 
        //                                 require joining to be enabled.
        //  02/10/17 jrf 4.72.00 WR 645582 Reworking to try all available zigbee radios from service
        //                                 and to retry zigbee logon on if the first attempt fails.
        private void TestZigBeeConnection()
        {
            ZigBeeRadioCallBack ServiceCallback = new ZigBeeRadioCallBack();
            ZigBeeRadioChannel ServiceChannel = new ZigBeeRadioChannel(ServiceCallback);
            ZigBeeRadioToken CurrentRadioToken = null;
            bool bLoggedOn = false;
            bool bRadioTested = false;
            bool bExpectLogOnSuccess = false;
            Random RandomNumGen = new Random();
            DateTime StartTime;
            string Details = "";

            try
            {
                // In order to prevent a possible case where multiple threads try to get radios at the same
                // time and possibly cause one of them to lock up we need to wait a random amount of time
                Thread.Sleep(RandomNumGen.Next(2000, 3000));

                StartTime = DateTime.Now;

                while (bRadioTested == false && (DateTime.Now - StartTime < MAX_WAIT_TIME) && IsAborted == false && null != ServiceChannel)
                {
                    List<ZigBeeRadioToken> AllRadios = ServiceChannel.GetRadioInformation();

                    if (null != AllRadios && AllRadios.Count > 0)
                    {
                        // Only attempt to run the test if there are no radios currently in use.
                        if (AllRadios.Where(r => r.Status == ZigBeeRadioToken.ZigBeeRadioStatus.InUse).Count() == 0)
                        {
                            List<ZigBeeRadioToken> RadioTokens = new List<ZigBeeRadioToken>();
                            CurrentRadioToken = ServiceChannel.RequestZigBeeRadio();

                            while (null != CurrentRadioToken && false == bLoggedOn)
                            {
                                RadioTokens.Add(CurrentRadioToken);

                                bRadioTested = true;
                                try
                                {
                                    bLoggedOn = LogonViaZigBee(CurrentRadioToken);

                                    if (false == bLoggedOn)
                                    {
                                        bLoggedOn = LogonViaZigBee(CurrentRadioToken);
                                    }
                                }
                                catch { bLoggedOn = false;}

                                if (false == bLoggedOn)
                                {
                                    CurrentRadioToken = ServiceChannel.RequestZigBeeRadio();
                                }
                            }

                            if (true == bRadioTested)
                            {
                                
                                bExpectLogOnSuccess = m_bC1218OverZigBeeEnabled && m_bZigBeeEnabled 
                                    && m_bPrivateProfileEnabled;

                                Details = ConvertYesOrNo(bLoggedOn);

                                if (bLoggedOn != bExpectLogOnSuccess)
                                {
                                    Details += ", " + TestResources.InconsistentWithHANSetup;
                                }

                                AddTestDetail(TestResources.AllowedANSIC1218Connection, GetResultString(bLoggedOn == bExpectLogOnSuccess), Details);
                            }

                            //Release all radios requested
                            foreach (ZigBeeRadioToken item in RadioTokens)
                            {
                                ServiceChannel.ReleaseZigBeeRadio(item);
                            }

                            RadioTokens.Clear();
                        }
                        else
                        {
                            // Wait a random amount of time between 1s and 3s
                            Thread.Sleep(RandomNumGen.Next(1000, 3000));
                        }
                    }
                    else
                    {
                        m_bTestSkipped = true;
                        bRadioTested = true;
                        m_TestResults.Reason = TestResources.NoRadiosAvailable;
                        AddTestDetail(TestResources.AllowedANSIC1218Connection, GetResultString(true, false), m_TestResults.Reason);
                    }
                }

                if (m_bTestSkipped == false && bRadioTested == false)
                {
                    m_bTestSkipped = true;
                    m_TestResults.Reason = TestResources.AllAvailableRadiosInUse;
                    AddTestDetail(TestResources.AllowedANSIC1218Connection, GetResultString(true, false), m_TestResults.Reason);
                }
            }
            catch (Exception e)
            {
                m_bTestSkipped = true;
                m_TestResults.Reason = e.Message;
            }
        }

        /// <summary>
        /// Connects to the meter using ZigBee
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created
        //  10/31/14 jrf 4.00.82  WR 542694 Added support for identifying Bridge meter with signed authorizaion.
        //  01/27/15 jrf 4.01.01 WR 557786 Adding ability for test to logon to meter using signed authorization.
        //  02/10/17 jrf 4.72.00 WR 645582 Adding try/catch around final logoff and close port.
        private bool LogonViaZigBee(ZigBeeRadioToken radioToken)
        {
            Radio ZigBeeRadio = CreateZigBeeRadio(radioToken);
            CPSEM ZigBeePSEM = null;
            CENTRON_AMI ZigBeeDevice = null;
            bool bCouldLogon = false;
            PSEMResponse Response;

            if (ZigBeeRadio != null)
            {
                ZigbeeResult ZBResult = ZigbeeResult.NOT_CONNECTED;
                ZigBeeRadio.OpenPort(radioToken.RadioIdentifier);

                if (ZigBeeRadio.IsOpen)
                {
                    int counter = 0;

                    while ((ZigbeeResult.SUCCESS != ZBResult) && (counter < 4))
                    {
                        try
                        {
                            ZBResult = ZigBeeRadio.Start(Radio.C177_HANDHELD_PROGRAMMER_MAC,
                                m_ulMACAddress, ZigbeeLogicalType.ENDDEVICE, m_byChannel);
                        }
                        catch
                        {
                            Thread.Sleep(5000);
                        }
                        counter++;
                    }
                }

                if (!ZigBeeRadio.IsOpen ||
                    ZBResult != ZigbeeResult.SUCCESS)
                {
                    // Make sure the radio is disconnected.
                    ZigBeeRadio.ClosePort();
                    ZigBeeRadio = null;
                }
                else
                {
                    try
                    {
                        // Logon to the meter
                        ZigBeePSEM = new CPSEM(ZigBeeRadio);

                        Response = ZigBeePSEM.Identify();

                        if (Response != PSEMResponse.Ok)
                        {
                            // Retry the identify in case the meter is in a bad state
                            Response = ZigBeePSEM.Identify();
                        }

                        if (Response == PSEMResponse.Ok)
                        {
                            Response = ZigBeePSEM.Negotiate(CPSEM.DEFAULT_MAX_PACKET_LEGNTH,
                                CPSEM.DEFAULT_MAX_NUMBER_OF_PACKETS, (uint)9600);
                        }

                        if (Response == PSEMResponse.Ok)
                        {
                            Response = ZigBeePSEM.Logon("", CPSEM.DEFAULT_HH_PRO_USER_ID);
                        }

                        if (Response == PSEMResponse.Ok)
                        {
                            ZigBeeDevice = CreateDevice(ZigBeePSEM, m_AuthorizationKey);

                            if (m_AuthorizationKey != null && m_AuthorizationKey.IsValid
                                && ZigBeeDevice.SignedAuthorizationState != null
                                && ZigBeeDevice.SignedAuthorizationState.Value != FeatureState.Disabled)
                            {
                                // We should use signed authorization to log on to the meter.
                                ZigBeeDevice.Authenticate(m_AuthorizationKey);
                            }
                            else
                            {
                                ZigBeeDevice.Security(GetPasswords());
                            }

                            if (ZigBeeDevice.HANMACAddress == m_ulMACAddress)
                            {
                                bCouldLogon = true;
                            }
                            else
                            {
                                bCouldLogon = false;
                            }
                        }
                    }
                    catch (Exception) { }
                    finally
                    {
                        try
                        {
                            ZigBeeDevice.Logoff();
                        }
                        catch { }
                    }
                }

                if (ZigBeeRadio != null)
                {
                    try
                    {
                        ZigBeeRadio.ClosePort();
                    }
                    catch { }
                }
            }

            return bCouldLogon;
        }

        /// <summary>
        /// Creates the communications object for the radio
        /// </summary>
        /// <param name="radioToken">The radio token to use.</param>
        /// <returns>The communications object.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        private Radio CreateZigBeeRadio(ZigBeeRadioToken radioToken)
        {
            Radio ZigBeeRadio = null;

            switch(radioToken.RadioType)
            {
                case ZigBeeRadioToken.ZigBeeRadioType.USBRadio:
                {
                    ZigBeeRadio = new BeltClipRadio();
                    ((BeltClipRadio)ZigBeeRadio).RadioManufacturer = BeltClipRadio.RadioMfg.ItronZBCR;
                    break;
                }
                case ZigBeeRadioToken.ZigBeeRadioType.TelegesisRadio:
                {
                    ZigBeeRadio = new BeltClipRadio();
                    ((BeltClipRadio)ZigBeeRadio).RadioManufacturer = BeltClipRadio.RadioMfg.TelegesisDongle;
                    break;
                }
            }

            return ZigBeeRadio;
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
                return ValidationTestID.ZigBeeRadioTest;
            }
        }

        #endregion

        #region Member Variables

        private ulong m_ulMACAddress;
        private byte m_byChannel;
        private bool m_bC1218OverZigBeeEnabled;
        private bool m_bJoiningEnabled;
        private bool m_bZigBeeEnabled;
        private bool m_bPrivateProfileEnabled;

        #endregion
    }
}