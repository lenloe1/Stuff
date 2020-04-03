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
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.CustomerValidation.Properties;
using Itron.Metering.Device;
using Itron.Metering.ReplicaSettings;
using Itron.Metering.SharedControls;
using Itron.Metering.Utilities;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Base class for all validation tests.
    /// </summary>
    public abstract class ValidationTest
    {
        #region Public Events

        /// <summary>
        /// Event that is raised when a fatal error occurs
        /// </summary>
        public virtual event FatalErrorEventHandler FatalErrorEvent;

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <returns>The test results</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        public abstract Test RunTest();

        /// <summary>
        /// Aborts the current test
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/22/10 RCG 2.40.28        Created

        public void Abort()
        {
            m_bAborted = true;
            m_bTestSkipped = true;
            m_TestResults.Reason = "Aborted";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the test.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        public virtual string TestName
        {
            get
            {
                return EnumDescriptionRetriever.RetrieveDescription(TestID);
            }
        }

        /// <summary>
        /// Gets the type of the test.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        public abstract ValidationTestID TestID
        {
            get;
        }

        /// <summary>
        /// Gets whether or not the test has been aborted
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/22/10 RCG 2.40.28        Created

        public bool IsAborted
        {
            get
            {
                return m_bAborted;
            }
        }

        #endregion

        #region Protected Methods

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
        protected ValidationTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
        {
            m_Comm = comm;
            m_uiBaudRate = uiBaudRate;
            m_strProgramFile = programFile;
            m_bTestPassed = true;
            m_bTestSkipped = false;
            m_AuthorizationKey = authKey;
        }

        /// <summary>
        /// Gets the results string for the boolean expression
        /// </summary>
        /// <param name="bPassed">Whether or not the test passed</param>
        /// <returns>Passed if the value is true. Failed otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/19/14 jrf 4.00.63 WR 534158 Created.
        //  
        protected string GetResultString(bool bPassed)
        {
            return GetResultString(false, bPassed);
        }
        
        /// <summary>
        /// Gets the results string for the boolean expression
        /// </summary>
        /// <param name="bSkipped">Whether or not the test was skipped</param>
        /// <param name="bPassed">Whether or not the test passed</param>
        /// <returns>Passed if the value is true. Failed otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534160 Modified result strings for pass and failure.
        protected string GetResultString(bool bSkipped, bool bPassed)
        {
            string strResult;

            if (bSkipped)
            {
                strResult = TestResources.Skipped;
            }
            else if (bPassed)
            {
                strResult = TestResources.OK;
            }
            else
            {
                strResult = TestResources.Error;
                m_bTestPassed = false;
            }

            return strResult;
        }        

        /// <summary>
        /// Gets the results string for the boolean expression
        /// </summary>
        /// <param name="bSkipped">Whether or not the test was skipped</param>
        /// <param name="bPassed">Whether or not the test passed</param>
        /// <returns>Passed if the value is true. Failed otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/19/14 jrf 4.00.63 WR 534160 Created.
        protected string GetTestResultString(bool bSkipped, bool bPassed)
        {
            string strResult;

            if (bSkipped)
            {
                strResult = TestResources.Skipped;
            }
            else if (bPassed)
            {
                strResult = TestResources.NoErrorsFound;
            }
            else
            {
                strResult = TestResources.ErrorsFound;
                m_bTestPassed = false;
            }

            return strResult;
        }   

        /// <summary>
        /// Logs on to the specified device.
        /// </summary>
        /// <returns>The response of the logon.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created
        //  04/09/14 jrf 3.50.68 WR458134 Generating a FatalError event and aborting test if we 
        //                                can't communicate with the ICS module.
        //  10/31/14 jrf 4.00.82  WR 542694 Added support for identifying Bridge meter with signed authorizaion.
        protected PSEMResponse LogonToDevice()
        {
            PSEMResponse Response = PSEMResponse.Ok;
            CPSEM PSEM = new CPSEM(m_Comm);

            Response = PSEM.Identify();

            if (Response != PSEMResponse.Ok)
            {
                // The meter may have been left in a bad state issuing Identify again
                // may reset the state in the meter.
                Response = PSEM.Identify();
            }

            if (Response == PSEMResponse.Ok)
            {
                Response = PSEM.Negotiate(CPSEM.DEFAULT_MAX_PACKET_LEGNTH,
                    CPSEM.DEFAULT_MAX_NUMBER_OF_PACKETS, m_uiBaudRate);
            }

            if(Response == PSEMResponse.Ok)
            {
                Response = PSEM.Logon("", CPSEM.DEFAULT_HH_PRO_USER_ID);
            }

            if (Response == PSEMResponse.Ok)
            {
                m_AmiDevice = CreateDevice(PSEM, m_AuthorizationKey);

                if (m_AuthorizationKey != null && m_AuthorizationKey.IsValid 
                    && m_AmiDevice.SignedAuthorizationState != null
                    && m_AmiDevice.SignedAuthorizationState.Value != FeatureState.Disabled)
                {
                    // We should use signed authorization to log on to the meter.
                    m_AmiDevice.Authenticate(m_AuthorizationKey);
                }
                else
                {
                    m_AmiDevice.Security(GetPasswords());
                }

                ICSCommModule ICSModule = (null != m_AmiDevice) ? m_AmiDevice.CommModule as ICSCommModule : null;
                bool ICSCommError = false;

                if (null != ICSModule)
                {
                    try
                    {
                        if (0 == ICSModule.ICMFirmwareVersionMajor)
                        {
                            ICSCommError = true;
                        }
                    }
                    catch
                    {
                        ICSCommError = true;
                    }

                    if (true == ICSCommError)
                    {
                        //Cannot communicate with ICS comm module so display error
                        OnFatalError(new ItronErrorEventArgs(Resources.LOGON_FAILED_ICS_COMM_ERROR, null));
                        Abort();
                    }
                }
            }

            return Response;
        }

        /// <summary>
        /// Creates a new device object
        /// </summary>
        /// <param name="psem">The current PSEM object</param>
        /// <param name="key">Signed authorization security key</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created
        //  02/17/11 RCG 2.50.04        Adding Support for ITRD, ITRE, ITRF meters
        //  03/22/11 jrf 2.80.10        Adding support for creating new ITRJ device object.
        //  12/04/13 jrf 3.50.13        Moved logic to create device into CANSIDevice.CreateDevice(...).
        //  10/31/14 jrf 4.00.82  WR 542694 Added support for identifying Bridge meter with signed authorizaion.
        protected CENTRON_AMI CreateDevice(CPSEM psem, SignedAuthorizationKey key)
        {
            CENTRON_AMI CreatedDevice = CANSIDevice.CreateDevice(m_Comm, psem, key) as CENTRON_AMI;

            if (null == CreatedDevice)
            {
                throw new InvalidOperationException(TestResources.MeterTypeNotSupported);
            }

            return CreatedDevice;
        }

        /// <summary>
        /// Gets the list of available passwords
        /// </summary>
        /// <returns>The list of passwords.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        protected List<string> GetPasswords()
        {
            List<string> Passwords = new List<string>();
            CXMLOpenWaySystemSettings SystemSettings = new CXMLOpenWaySystemSettings("");

            Passwords.Add(SystemSettings.CurrentPWD);

            if (Passwords.Contains(SystemSettings.PreviousPWD) == false)
            {
                Passwords.Add(SystemSettings.PreviousPWD);
            }

            if (Passwords.Contains("") == false)
            {
                Passwords.Add("");
            }

            return Passwords;
        }

        /// <summary>
        /// Converts a boolean to a yes or no string
        /// </summary>
        /// <param name="bYes">The boolean value</param>
        /// <returns>Yes if the value is true No otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        protected string ConvertYesOrNo(bool bYes)
        {
            string strYesOrNo = TestResources.No;

            if (true == bYes)
            {
                strYesOrNo = TestResources.Yes;
            }

            return strYesOrNo;
        }

        /// <summary>
        /// Adds a new test detail to the results file.
        /// </summary>
        /// <param name="name">The name of the detail</param>
        /// <param name="result">The meter value</param>
        /// <param name="details">The details of the detail</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Setting test details in a new way.
        protected void AddTestDetail(string name, string result, string details)
        {
            AddTestDetail(name, result, details, null);
        }

        /// <summary>
        /// Adds a new test detail to the results file.
        /// </summary>
        /// <param name="name">The name of the detail</param>
        /// <param name="result">The result</param>
        /// <param name="details">The details of the detail</param>
        /// <param name="reason">The reason for the result</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Setting test details in a new way.
        protected void AddTestDetail(string name, string result, string details, string reason)
        {
            m_TestResults.TestDetails.Add(new TestDetail(name, result, details, reason));
        }

        /// <summary>
        /// Adds a new test detail to the results file.
        /// </summary>
        /// <param name="name">The name of the detail</param>
        /// <param name="result">The result</param>
        /// <param name="details">The details of the detail</param>
        /// <param name="reason">The reason for the result</param>
        /// <param name="additionalDetails">The additional details for the result.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Setting test details in a new way.
        protected void AddTestDetail(string name, string result, string details, string reason, string additionalDetails)
        {
            m_TestResults.TestDetails.Add(new TestDetail(name, result, details, reason, additionalDetails));
        }

        /// <summary>
        /// Determines the reason for a failure
        /// </summary>
        /// <param name="response">The PSEM response</param>
        /// <returns>The reason for the failure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        protected string DetermineReason(PSEMResponse response)
        {
            string strResult = "";

            switch (response)
            {
                case PSEMResponse.Isc:
                {
                    strResult = TestResources.ReasonSecurityError;
                    break;
                }
                default:
                {
                    strResult = TestResources.ReasonProtocolError;
                    break;
                }
            }

            return strResult;
        }

        /// <summary>
        /// Determines the reason for a failure
        /// </summary>
        /// <param name="result">The result</param>
        /// <returns>The reason for the failure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        protected string DetermineReason(ItronDeviceResult result)
        {
            string strResult = "";

            switch (result) 
            {
                case ItronDeviceResult.SECURITY_ERROR:
                {
                    strResult = TestResources.ReasonSecurityError;
                    break;
                }
                case ItronDeviceResult.UNSUPPORTED_OPERATION:
                {
                    strResult = TestResources.ReasonOperationNotSupported;
                    break;
                }
            }

            return strResult;
        }

        /// <summary>
        /// Raises a FatalErrorEvent for this test.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/08/14 jrf 3.50.69 458134 Created
        protected virtual void OnFatalError(ItronErrorEventArgs e)
        {
            if (FatalErrorEvent != null)
            {
                FatalErrorEvent(this, e);
            }
        }//OnFatalError

        /// <summary>
        /// Displays the details for a clear operation.
        /// </summary>
        /// <param name="cleared">Whether the clear succeeded</param>
        /// <returns>Details from clearing operation</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/19/14 jrf 4.00.63 WR 534158 Created.
        protected string GetClearDetails(bool cleared)
        {
            string Details = "";

            if (cleared)
            {
                Details = TestResources.ClearSuccess;
            }
            else
            {
                Details = TestResources.ClearFailure;
            }

            return Details;
        }
        #endregion

        #region Member Variables

        /// <summary>
        /// Communications object
        /// </summary>
        protected ICommunications m_Comm;
        /// <summary>
        /// Baud rate for communications
        /// </summary>
        protected uint m_uiBaudRate;
        /// <summary>
        /// The program file that should be used to validate the meter.
        /// </summary>
        protected string m_strProgramFile;
        /// <summary>
        /// The device object
        /// </summary>
        protected CENTRON_AMI m_AmiDevice;
        /// <summary>
        /// Whether or not the test passed.
        /// </summary>
        protected bool m_bTestPassed;
        /// <summary>
        /// Whether or not the test was skipped.
        /// </summary>
        protected bool m_bTestSkipped;
        /// <summary>
        /// The current results for this test.
        /// </summary>
        protected Test m_TestResults;
        /// <summary>
        /// The Authorization key to use to log on to the meter.
        /// </summary>
        protected SignedAuthorizationKey m_AuthorizationKey;
        /// <summary>
        /// Whether or not the test has been aborted
        /// </summary>
        protected bool m_bAborted;

        #endregion
    }
}
