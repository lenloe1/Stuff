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
using Itron.Metering.ReplicaSettings;
using Itron.Metering.SharedControls;
using Itron.Metering.Utilities;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Validation test that the connect disconnect switch works
    /// </summary>
    public class ConnectDisconnectTest : ValidationTest
    {
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
        public ConnectDisconnectTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
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
        //  08/15/12 MAH 2.60.54 WR 201902 Added more time between switch operations to allow the meter to recharge its capacitor
        //  09/19/14 jrf 4.00.63 WR 534158 Using new method to set test result string and also checking user permissions.
        //  10/02/14 jrf 4.00.66 WR 431248 Making sure if logon is called and exception occurs then a logoff is 
        //                                 attempted so other tests may continue if possible.
        //  04/27/15 jrf 4.20.03 WR 574470 Increased wait between disconnect/connect to allow capacitor more time to charge 
        //                                 for hw 2.0 meter.
        public override Test RunTest()
        {
            PSEMResponse Response = PSEMResponse.Ok;
            CXMLOpenWayUserAccessPolicy UserAccess = new CXMLOpenWayUserAccessPolicy();
            bool bIsUserAllowed = UserAccess.CheckUserAccess(CXMLOpenWayUserAccessPolicy.FunctionalCapability.MeterSwitchOperations);

            m_TestResults = new Test();
            m_TestResults.Name = TestName;

            m_bTestPassed = true;

            try
            {
                Response = LogonToDevice();

                if (false == bIsUserAllowed)
                {
                    m_bTestSkipped = true;
                    m_TestResults.Reason = TestResources.ReasonSwitchOperationPermissions;
                }
                else if (Response == PSEMResponse.Ok)
                {
                    // First make sure the meter supports connect disconnect.
                    if (m_AmiDevice.IsServiceLimitingTablePresent
                        && m_AmiDevice.DisconnectHardwareExists
                        && m_AmiDevice.IsDisconnectHardwareFunctioning)
                    {
                        DisconnectMeter();

                        if (IsAborted == false)
                        {
                            m_AmiDevice.SendWait();
                            // Thread.Sleep(2000);
                            // MAH - 8/15/12 - Increased the time between switch operations to allow the capacitor to recharge
                            //Thread.Sleep(12500);
                            // jrf - 4/27/15 - Increased the time between switch operations slightly more to allow the capacitor 
                            //                 to recharge for HW 2.0 meter.
                            Thread.Sleep(15000);
                        }

                        CheckStatus(false);

                        ConnectMeter();

                        if (IsAborted == false)
                        {
                            m_AmiDevice.SendWait();
                            Thread.Sleep(2000);
                        }

                        CheckStatus(true);
                    }
                    else
                    {
                        // Disconnect is not supported
                        m_bTestSkipped = true;
                        m_TestResults.Reason = TestResources.ReasonDisconnectNotSupported;
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
                return ValidationTestID.ConnectDisconnect;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Issues the connect command to the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
		//  01/20/11 RCG 2.45.23           Updating to support Connect/Disconnect Procedure Enhancement
        //  09/22/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void ConnectMeter()
        {
            RemoteConnectResult ConnectResult;
            ConnectDisconnectResponse ConnectRespose;

            if (IsAborted == false)
            {
                ConnectResult = m_AmiDevice.RemoteConnect(ConnectType.CONNECT_NO_USER_INTERVENTION, out ConnectRespose);

                AddTestDetail(TestResources.RemoteConnect,
                    GetResultString(ConnectResult, ConnectRespose),
                    DetermineDetails(ConnectResult, ConnectRespose),
                    DetermineReason(ConnectResult, ConnectRespose));
            }
        }

        /// <summary>
        /// Checks the connection status of the meter.
        /// </summary>
        /// <param name="bConnected">Whether or not the meter should be connected</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/22/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void CheckStatus(bool bConnected)
        {
            if (IsAborted == false)
            {
                bool bIsLoadVoltagePresent = m_AmiDevice.IsLoadVoltagePresent;
                bool bIsMeterConnected = m_AmiDevice.IsConnected;

                AddTestDetail(TestResources.Connected,
                    GetResultString(bIsMeterConnected == bConnected),
                    GetConnectedDetails(bIsMeterConnected, bConnected));

                AddTestDetail(TestResources.LoadVoltagePresent,
                    GetResultString(bIsLoadVoltagePresent == bConnected),
                    GetLoadVoltageDetails(bIsLoadVoltagePresent, bConnected));
            }
        }

        /// <summary>
        /// Displays the details for the connected status results.
        /// </summary>
        /// <param name="connectedActual">Whether or not the meter is in a connected state.</param>
        /// <param name="connectedExpected">Whether or not the meter should be in a connected state.</param>
        /// <returns>Details from cleared event log count</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/22/14 jrf 4.00.63 WR 534158 Created.
        private string GetConnectedDetails(bool connectedActual, bool connectedExpected)
        {
            string Details = ConvertYesOrNo(connectedActual);

            if (true == connectedExpected && false == connectedActual)
            {
                Details += ", " + TestResources.MeterShouldBeConnected;
            }
            else if (false == connectedExpected && true == connectedActual)
            {
                Details += ", " + TestResources.MeterShouldBeDisconnected;
            }
            return Details;
        }

        /// <summary>
        /// Displays the details for the Load Voltage Present status results.
        /// </summary>
        /// <param name="loadVoltageActual">Whether or not the meter does have load voltage present.</param>
        /// <param name="loadVoltageExpected">Whether or not the meter should have load voltage present.</param>
        /// <returns>Details from cleared event log count</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/22/14 jrf 4.00.63 WR 534158 Created.
        private string GetLoadVoltageDetails(bool loadVoltageActual, bool loadVoltageExpected)
        {
            string Details = ConvertYesOrNo(loadVoltageActual);

            if (true == loadVoltageExpected && false == loadVoltageActual)
            {
                Details += ", " + TestResources.MeterShouldHaveLoadVoltage;
            }
            else if (false == loadVoltageExpected && true == loadVoltageActual)
            {
                Details += ", " + TestResources.MeterShouldNotHaveLoadVoltage;
            }
            return Details;
        }

        /// <summary>
        /// Issues the disconnect command to the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
		//  01/20/11 RCG 2.45.23           Updating to support Connect/Disconnect Procedure Enhancement
        //  09/22/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void DisconnectMeter()
        {
            RemoteConnectResult ConnectResult;
            ConnectDisconnectResponse DisconnectResponse;

            if (IsAborted == false)
            {
                ConnectResult = m_AmiDevice.RemoteDisconnect(out DisconnectResponse);

                AddTestDetail(TestResources.RemoteDisconnect,
                    GetResultString(ConnectResult, DisconnectResponse),
                    DetermineDetails(ConnectResult, DisconnectResponse),
                    DetermineReason(ConnectResult, DisconnectResponse));
            }
        }

        /// <summary>
        /// Gets the reason for the specified result.
        /// </summary>
        /// <param name="connectResult">The result to get the reason for.</param>
        /// <param name="connectResponse">The response to get the reason for.</param>
        /// <returns>The reason for the result as a string.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created
		//  01/20/11 RCG 2.45.23        Updating to support Connect/Disconnect Procedure Enhancement

        private string DetermineReason(RemoteConnectResult connectResult, ConnectDisconnectResponse connectResponse)
        {
            string strReason = null;

            switch(connectResult)
            {
                case RemoteConnectResult.LOAD_VOLTAGE_NOT_DETECTED:
                {
                    strReason = TestResources.ReasonLoadVoltageNotDetected;
                    break;
                }
                case RemoteConnectResult.LOAD_VOLTAGE_PRESENT:
                {
                    strReason = TestResources.ReasonLoadVoltagePresent;
                    break;
                }
                case RemoteConnectResult.REMOTE_CONNECT_FAILED:
                {
                    strReason = TestResources.ReasonRemoteConnectFailed;
                    break;
                }
                case RemoteConnectResult.SECURITY_VIOLATION:
                {
                    strReason = TestResources.ReasonSecurityError;
                    break;
                }
                case RemoteConnectResult.UNRECOGNIZED_PROCEDURE:
                {
                    strReason = TestResources.ReasonUnrecognizedProcedure;
                    break;
                }
                case RemoteConnectResult.REMOTE_ACTION_SUCCESS:
                {
                    // The procedure can now have an extra response which we need to interpret
                    switch(connectResponse)
                    {
                        case ConnectDisconnectResponse.Successful:
                        case ConnectDisconnectResponse.SuccessfulInterventionRequired:
                        {
                            // It succeeded so we don't need a reason
                            strReason = null;
                            break;
                        }
                        default:
                        {
                            strReason = EnumDescriptionRetriever.RetrieveDescription(connectResponse);
                            break;
                        }
                    }
                    break;
                }
            }

            return strReason;
        }

        /// <summary>
        /// Gets the details for the specified result.
        /// </summary>
        /// <param name="connectResult">The result to get the reason for.</param>
        /// <param name="connectResponse">The response to get the reason for.</param>
        /// <returns>Details for the result.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/22/14 jrf 4.00.63 WR 534158 Created.
        private string DetermineDetails(RemoteConnectResult connectResult, ConnectDisconnectResponse connectResponse)
        {
            string strReason = null;

            if (RemoteConnectResult.REMOTE_ACTION_SUCCESS == connectResult
                && (ConnectDisconnectResponse.Successful == connectResponse || 
                    ConnectDisconnectResponse.SuccessfulInterventionRequired == connectResponse))
                    {
                        strReason = TestResources.OperationSuccess;
                    }
            else
            {
                strReason = TestResources.OperationFailure + ": " + DetermineReason(connectResult, connectResponse);
            }
            

            return strReason;
        }

        /// <summary>
        /// Gets the results string for the boolean expression
        /// </summary>
        /// <param name="connectResult">The result to get the reason for.</param>
        /// <param name="connectResponse">The response to get the reason for.</param>
        /// <returns>Passed if the value is true. Failed otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/19/14 jrf 4.00.63 WR 534158 Created.
        //  
        protected string GetResultString(RemoteConnectResult connectResult, ConnectDisconnectResponse connectResponse)
        {
            bool Passed = false;

            if (RemoteConnectResult.REMOTE_ACTION_SUCCESS == connectResult
                && (ConnectDisconnectResponse.Successful == connectResponse ||
                    ConnectDisconnectResponse.SuccessfulInterventionRequired == connectResponse))
            {
                Passed = true;
            }

            return GetResultString(Passed);
        }

        #endregion
    }
}
