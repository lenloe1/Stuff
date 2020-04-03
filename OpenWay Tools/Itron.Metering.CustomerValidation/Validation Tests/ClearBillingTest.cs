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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.ReplicaSettings;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Validation test that clears the billing registers
    /// </summary>
    public class ClearBillingTest : ValidationTest
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
        public ClearBillingTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
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
        //  09/19/14 jrf 4.00.63 WR 534158 Using new method to set test result string.
        //  10/02/14 jrf 4.00.66 WR 431248 Making sure if logon is called and exception occurs then a logoff is 
        //                                 attempted so other tests may continue if possible.
        //  10/13/14 jrf 4.00.72 WR 537980 Skipping test on sealed canadian meter.
        public override Test RunTest()
        {
            CXMLOpenWayUserAccessPolicy UserAccess = new CXMLOpenWayUserAccessPolicy();
            bool bIsUserAllowed = UserAccess.CheckUserAccess(CXMLOpenWayUserAccessPolicy.FunctionalCapability.ResetBillingRegisters);
            PSEMResponse Response = PSEMResponse.Ok;
            m_TestResults = new Test();
            m_TestResults.Name = TestName;

            m_bTestPassed = true;

            try
            {
                Response = LogonToDevice();

                if (true == m_AmiDevice.IsSealedCanadian)
                {
                    m_bTestSkipped = true;
                    m_TestResults.Reason = TestResources.ReasonBillingResetSealedCanadian;
                }
                else if (false == bIsUserAllowed)
                {
                    m_bTestSkipped = true;
                    m_TestResults.Reason = TestResources.ReasonBillingResetPermissions;
                }
                else if (Response == PSEMResponse.Ok)
                {
                    // Reset the billing registers
                    DisplayRegisterValues(TestResources.PrefixInitial);
                    ClearBilling();

                    // This can take a little while so we should wait and make sure it's cleared before
                    // we display the values again.
                    if (IsAborted == false)
                    {
                        m_AmiDevice.SendWait();
                        Thread.Sleep(5000);
                    }

                    DisplayRegisterValues(TestResources.PrefixFinal);
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
        /// Clears the billing registers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        //  12/03/14 jrf 4.00.90 WR 548525 Adding the correct name for the test detail.
        private void ClearBilling()
        {
            ItronDeviceResult Result;

            if (IsAborted == false)
            {
                Result = m_AmiDevice.ClearBillingDataAndWaitForResult();

                AddTestDetail(TestResources.ClearBillingData,
                    GetResultString(Result == ItronDeviceResult.SUCCESS),
                    GetClearDetails(Result == ItronDeviceResult.SUCCESS),
                    DetermineReason(Result));
            }
        }

        /// <summary>
        /// Displays the registers values.
        /// </summary>
        /// <param name="strPrefix">The prefix to show before the register value name.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created
        //  09/08/11 jrf 2.52.14 179655 Checking TOU max and cumulative demand lists for null before trying to access
        //                              their rate values.  Also corrected else if for checking TOUMaxDemand
        //                              list values to check TOUMaxDemand for null and obtain the TOU rate count
        //                              from the TOUMaxDemand list instead.
        //
        private void DisplayRegisterValues(string strPrefix)
        {
            if (IsAborted == false)
            {
                foreach (Quantity CurrentQuantity in m_AmiDevice.CurrentRegisters)
                {
                    AddMeasurementDetail(strPrefix, CurrentQuantity.TotalEnergy);
                    AddMeasurementDetail(strPrefix, CurrentQuantity.TotalMaxDemand);
                    AddMeasurementDetail(strPrefix, CurrentQuantity.CummulativeDemand);
                    AddMeasurementDetail(strPrefix, CurrentQuantity.ContinuousCummulativeDemand);

                    if (CurrentQuantity.TOUEnergy != null)
                    {
                        // We have TOU values so add those to the list
                        for (int iRate = 0; iRate < CurrentQuantity.TOUEnergy.Count; iRate++)
                        {
                            AddMeasurementDetail(strPrefix, CurrentQuantity.TOUEnergy[iRate]);
                            if (null != CurrentQuantity.TOUMaxDemand)
                            {
                                AddMeasurementDetail(strPrefix, CurrentQuantity.TOUMaxDemand[iRate]);
                            }
                            if (null != CurrentQuantity.TOUCummulativeDemand)
                            {
                                AddMeasurementDetail(strPrefix, CurrentQuantity.TOUCummulativeDemand[iRate]);
                            }
                            if (null != CurrentQuantity.TOUCCummulativeDemand)
                            {
                                AddMeasurementDetail(strPrefix, CurrentQuantity.TOUCCummulativeDemand[iRate]);
                            }
                        }
                    }
                    else if (CurrentQuantity.TOUMaxDemand != null)
                    {
                        // This is probably a Coincident value so just populate the demand values
                        for (int iRate = 0; iRate < CurrentQuantity.TOUMaxDemand.Count; iRate++)
                        {
                            AddMeasurementDetail(strPrefix, CurrentQuantity.TOUMaxDemand[iRate]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a test detail for the specified measurement.
        /// </summary>
        /// <param name="strPrefix">The prefix to use.</param>
        /// <param name="currentMeasurement">The measurement to add.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void AddMeasurementDetail(string strPrefix, Measurement currentMeasurement)
        {
            if (currentMeasurement != null)
            {
                AddTestDetail(strPrefix + currentMeasurement.Description,
                    TestResources.OK,
                    currentMeasurement.Value.ToString("F3", CultureInfo.CurrentCulture));
            }
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
                return ValidationTestID.ClearBilling;
            }
        }

        #endregion
    }
}