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
//                           Copyright © 2009 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using Itron.Metering.Progressable;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.CustomerValidationTool.Properties;
using Itron.Metering.CustomerValidation;
using Itron.Metering.Utilities;
using Itron.Metering.Device;
using Itron.Metering.SharedControls;
using Itron.Metering.ReplicaSettings;

namespace Itron.Metering.CustomerValidationTool
{
    /// <summary>
    /// Worker class used to run tests.
    /// </summary>
    public class ValidationWorker : IProgressable
    {
        #region Constants

        private const string TEST_FILE = "OpenWayValidationTests.xml";
        private const string REG_REPLICA = "OpenWay Replica";

        private const string AMI_CENT = "AMI CENT";

        #endregion

        #region Public Events

        /// <summary>
        /// Event used to indicate the the task has completed.
        /// </summary>
        public event HideProgressEventHandler HideProgressEvent;
        /// <summary>
        /// Event used to indicate that the task has started.
        /// </summary>
        public event ShowProgressEventHandler ShowProgressEvent;
        /// <summary>
        /// Event used to indicate that task has taken a step
        /// </summary>
        public event StepProgressEventHandler StepProgressEvent;

        /// <summary>
        /// Event that is raised when a fatal error occurs
        /// </summary>
        public virtual event FatalErrorEventHandler FatalErrorEvent;

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strName">
        /// The identifier for the current test run. This is used to identify which probe the test is
        /// being run on and is for informational purposes.
        /// </param>
        /// <param name="strPort">The port to use for the test.</param>
        /// <param name="uiBaudRate">The baud rate to use for the test.</param>
        /// <param name="probeType">The optical probe type for the test.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public ValidationWorker(string strName, string strPort, uint uiBaudRate, OpticalProbeTypes probeType)
        {
            m_bCancelled = false;
            m_strName = strName;
            m_strResultDir = Settings.Default.ResultsDir + "\\";
            m_strPort = strPort;
            m_uiBaudRate = uiBaudRate;
            m_ProbeType = probeType;

            m_Comm = new SerialCommDesktop();
            m_Comm.OpticalProbe = m_ProbeType;
            m_Comm.BaudRate = m_uiBaudRate;

            m_strProgramFile = Settings.Default.ProgramFile;
            m_strValidationTestsFile = CRegistryHelper.GetFilePath(REG_REPLICA) + TEST_FILE;

            if (Directory.Exists(m_strResultDir) == false)
            {
                Directory.CreateDirectory(m_strResultDir);
            }
            
        }

        /// <summary>
        /// Runs the validation tests.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created
        //  04/09/13 jrf 3.50.69 458134 Adding handler for the ValidationTest's FatalErrorEvent.
        public void RunValidationTests()
        {
            TestResults CurrentResults = new TestResults();
            TestRun CurrentTestRun = new TestRun();
            List<ValidationTest> SelectedTests = null;
            TestStepResult PreviousStepResult = TestStepResult.None;
            string[] SplitMeterID;
            string strTitle = "";
            string strMeterID = "";

            m_bCancelled = false;

            OnShowProgressEvent(new ShowProgressEventArgs(1, StepCount, m_strName, Resources.StatusRetrievingDeviceInfo));

            try
            {
                // Open the communications port for the duration of the test.
                m_Comm.OpenPort(m_strPort);

                // Get the basic device information from the meter.
                CurrentTestRun = GetDeviceInfo();
                SelectedTests = GetSelectedTests();

                // Since we are using the ESN for meter ID we need to trim it down to a 
                // reasonable identifier for use in the status and file name
                SplitMeterID = CurrentTestRun.MeterID.Split('.');

                if (SplitMeterID.Length > 0)
                {
                    strMeterID = SplitMeterID[SplitMeterID.Length - 1];
                }

                strTitle = m_strName + " - " + strMeterID;
                m_strResultFile = m_strResultDir + strMeterID + " "
                    + CurrentTestRun.TestDate.ToString("MMddyyyyhhmmss", CultureInfo.CurrentCulture) + ".xml";

                // Run each of the tests
                foreach (ValidationTest CurrentTest in SelectedTests)
                {
                    Test TestResult = null;

                    OnStepProgressEvent(new TestStepEventArgs(Resources.StatusRunningTest + CurrentTest.TestName + "...", strTitle, PreviousStepResult));

                    if (m_bCancelled == false)
                    {
                        m_RunningTest = CurrentTest;
                        m_RunningTest.FatalErrorEvent += new FatalErrorEventHandler(HandleFatalError);

                        try
                        {
                            TestResult = CurrentTest.RunTest();
                        }
                        catch (Exception e)
                        {
                            TestResult = new Test();
                            TestResult.Name = CurrentTest.TestName;
                            TestResult.Reason = Resources.ReasonExceptionOccurred + e.Message;
                            TestResult.Result = Resources.Failed;
                        }

                        m_RunningTest = null;
                    }
                    else
                    {
                        TestResult = new Test();
                        TestResult.Name = CurrentTest.TestName;
                        TestResult.Result = Resources.Skipped;
                        TestResult.Reason = Resources.Cancelled;
                    }

                    PreviousStepResult = GetStepResult(TestResult);
                    CurrentTestRun.Tests.Add(TestResult);
                }

                m_Comm.ClosePort();
                OnStepProgressEvent(new TestStepEventArgs(Resources.StatusSavingResults, strTitle, PreviousStepResult));

                // Save the results
                CurrentResults.TestRuns.Add(CurrentTestRun);
                CurrentResults.SaveAs(m_strResultFile);

                if (m_bCancelled == false)
                {
                    OnHideProgressEvent(new TestCompleteEventArgs(Resources.StatusComplete, m_strResultFile));
                }
                else
                {
                    OnHideProgressEvent(new TestCompleteEventArgs(Resources.Cancelled, m_strResultFile));
                }
            }
            catch (ThreadAbortException)
            {
                // The thread was killed no need to display an error
                if (m_Comm != null && m_Comm.IsOpen)
                {
                    m_Comm.ClosePort();
                }
            }
            catch (PSEMException e)
            {
                // An error occurred while running the tests so we should notify the user.
                if (m_Comm != null && m_Comm.IsOpen)
                {
                    m_Comm.ClosePort();
                }

                ErrorForm.DisplayError(m_strName + " - " + "A protocol error occurred: " + e.PSEMResponse.ToDescription(), e);
                OnHideProgressEvent(new TestCompleteEventArgs(Resources.FailedExceptionOccurred, null));
            }
            catch (Exception e)
            {
                // An error occurred while running the tests so we should notify the user.
                if (m_Comm != null && m_Comm.IsOpen)
                {
                    m_Comm.ClosePort();
                }

                ErrorForm.DisplayError(m_strName + " - " + e.Message, e);
                OnHideProgressEvent(new TestCompleteEventArgs(Resources.FailedExceptionOccurred, null));
            }
        }

        /// <summary>
        /// Tells the worker to cancel it's operation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void CancelTests()
        {
            m_bCancelled = true;
        }

        /// <summary>
        /// Aborts the current test and cancels any scheduled tests
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/22/10 RCG 2.40.28        Created

        public void AbortTests()
        {
            m_bCancelled = true;

            if (m_RunningTest != null)
            {
                m_RunningTest.Abort();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the device information from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified to use the CANSIDevice.CreateDevice()
        //                                 method to instantiate the correct device and switched
        //                                 to store the name of the meter instead of device class.
        //  10/31/14 jrf 4.00.82  WR 542694 Added support for identifying Bridge meter with signed authorizaion.
        private TestRun GetDeviceInfo()
        {
            CXMLOpenWaySystemSettings SystemSettings = new CXMLOpenWaySystemSettings("");
            CPSEM PSEM = new CPSEM(m_Comm);
            PSEMResponse Response = PSEMResponse.Ok;
            string strModel = null;
            TestRun NewTestRun = new TestRun();
            string MeterType = "";

            Response = PSEM.Identify();

            if(Response == PSEMResponse.Ok)
            {
                Response = PSEM.Negotiate(CPSEM.DEFAULT_MAX_PACKET_LEGNTH, 
                    CPSEM.DEFAULT_MAX_NUMBER_OF_PACKETS, m_uiBaudRate);
            }

            if (Response == PSEMResponse.Ok)
            {
                Response = PSEM.Logon("", CPSEM.DEFAULT_HH_PRO_USER_ID);
            }

            if (Response == PSEMResponse.Ok)
            {
                CTable00 Table0 = new CTable00(PSEM);
                CTable01 Table1 = new CTable01(PSEM, Table0.StdVersion);
                strModel = Table1.Model;

                if (strModel == AMI_CENT)
                {
                    CANSIDevice ANSIDevice = CANSIDevice.CreateDevice(m_Comm, PSEM, Settings.Default.AuthenticationKey);
                    CENTRON_AMI AMIDevice = ANSIDevice as CENTRON_AMI;

                    if (null != AMIDevice)
                    {
                        if (SystemSettings.UseSignedAuthorization && AMIDevice.SignedAuthorizationState != null
                            && AMIDevice.SignedAuthorizationState.Value != FeatureState.Disabled
                            && Settings.Default.AuthenticationKey != null)
                        {
                            // Use Signed Authenticaiton
                            AMIDevice.Authenticate(Settings.Default.AuthenticationKey);
                        }
                        else
                        {
                            // Use standard security
                            AMIDevice.Security(GetPasswords());
                        }

                        MeterType = AMIDevice.MeterName;

                        if (AMIDevice.CommModule != null)
                        {
                            m_strMeterID = AMIDevice.CommModule.ElectronicSerialNumber;
                        }
                    }
                }
            }

            try
            {
                PSEM.Logoff();
                PSEM.Terminate();
            }
            catch (Exception)
            {
                // Make sure we log off.
            }

            // Handle any errors that may have occurred
            if (Response != PSEMResponse.Ok)
            {
                throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, Resources.ErrorRetrievingDeviceIdentification);
            }
            else if (strModel != AMI_CENT)
            {
                throw new InvalidOperationException(Resources.MeterTypeNotSupported);
            }

            // Set up the TestRun results
            NewTestRun.MeterType = MeterType;
            NewTestRun.MeterID = m_strMeterID;
            NewTestRun.TestDate = DateTime.Now;
            NewTestRun.ProgramName = m_strProgramFile;
            NewTestRun.SWVersion = Application.ProductVersion;

            return NewTestRun;
        }

        /// <summary>
        /// Gets the list of available passwords
        /// </summary>
        /// <returns>The list of passwords.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00        Created

        private List<string> GetPasswords()
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
        /// Gets the step result based on the result of the test.
        /// </summary>
        /// <param name="testResult">The result of the test.</param>
        /// <returns>The translated step result.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534160 Setting translated result appropriately for new results.
        private TestStepResult GetStepResult(Test testResult)
        {
            TestStepResult TranslatedResult = TestStepResult.None;

            if (testResult != null)
            {
                if (Resources.Passed.Equals(testResult.Result)
                    || Resources.NoErrorsFound.Equals(testResult.Result))
                {
                    TranslatedResult = TestStepResult.Passed;
                }
                else if (Resources.Skipped.Equals(testResult.Result))
                {
                    TranslatedResult = TestStepResult.Skipped;
                }
                else if (Resources.Failed.Equals(testResult.Result)
                    || Resources.ErrorsFound.Equals(testResult.Result))
                {
                    TranslatedResult = TestStepResult.Failed;
                }
            }

            return TranslatedResult;
        }

        /// <summary>
        /// Gets the list of selected tests
        /// </summary>
        /// <returns>The list of selected tests</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 removed unnecessary device class paramater.
        private List<ValidationTest> GetSelectedTests()
        {
            CXMLOpenWaySystemSettings SystemSettings = new CXMLOpenWaySystemSettings("");
            ValidationTestRun TestRun = new ValidationTestRun(m_strValidationTestsFile);
            List<ValidationTest> SelectedTests = new List<ValidationTest>();
            SignedAuthorizationKey AuthorizationKey = null;

            if (SystemSettings.UseSignedAuthorization)
            {
                AuthorizationKey = Settings.Default.AuthenticationKey;
            }

            foreach (ValidationTestID CurrentID in TestRun.SelectedTestIDs)
            {
                switch(CurrentID)
                {
                    case ValidationTestID.ClearActivityStatus:
                    {
                        SelectedTests.Add(new ClearActivityStatusTest(m_Comm, m_uiBaudRate, m_strProgramFile, AuthorizationKey));
                        break;
                    }
                    case ValidationTestID.ClearBilling:
                    {
                        SelectedTests.Add(new ClearBillingTest(m_Comm, m_uiBaudRate, m_strProgramFile, AuthorizationKey));
                        break;
                    }
                    case ValidationTestID.ConnectDisconnect:
                    {
                        SelectedTests.Add(new ConnectDisconnectTest(m_Comm, m_uiBaudRate, m_strProgramFile, AuthorizationKey));
                        break;
                    }
                    case ValidationTestID.DeviceStatus:
                    {
                        SelectedTests.Add(new DeviceStatusTest(m_Comm, m_uiBaudRate, m_strProgramFile, AuthorizationKey));
                        break;
                    }
                    case ValidationTestID.ZigBeeRadioTest:
                    {
                        SelectedTests.Add(new ZigBeeRadioTest(m_Comm, m_uiBaudRate, m_strProgramFile, AuthorizationKey));
                        break;
                    }
                    case ValidationTestID.ShowNormalDisplay:
                    {
                        SelectedTests.Add(new NormalDisplayTest(m_Comm, m_uiBaudRate, m_strProgramFile, AuthorizationKey));
                        break;
                    }
                    case ValidationTestID.ValidateProgram:
                    {
                        SelectedTests.Add(new ValidateProgramTest(m_Comm, m_uiBaudRate, m_strProgramFile, AuthorizationKey));
                        break;
                    }
                    case ValidationTestID.ValidateSecurityKeys:
                    {
                        SelectedTests.Add(new ValidateSecurityKeysTest(m_Comm, m_uiBaudRate, m_strProgramFile, AuthorizationKey));
                        break;
                    }
                }
            }

            return SelectedTests;
        }

        /// <summary>
        /// Raises the Hide Progress event
        /// </summary>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void OnHideProgressEvent(TestCompleteEventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the Show Progress event
        /// </summary>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void OnShowProgressEvent(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the Step Progress event
        /// </summary>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void OnStepProgressEvent(TestStepEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Event handler that handles a fatal error
        /// </summary>
        /// <param name="sender">The control that sent the event</param>
        /// <param name="e">The event arguments</param>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ------------------------------------------
        // 04/08/13 jrf 3.50.69 458134 Created
        private void HandleFatalError(object sender, ItronErrorEventArgs e)
        {
            //Cancel all tests running on this worker thread.
            CancelTests();

            if (FatalErrorEvent != null)
            {
                if (e != null)
                {
                    e.Message = m_strName + ": " + e.Message;
                }

                FatalErrorEvent(this, e);
            }

        }//HandleFatalError

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the number of steps in the test run.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created
        private int StepCount
        {
            get
            {
                ValidationTestRun TestRun = new ValidationTestRun(m_strValidationTestsFile);
                return TestRun.SelectedTestIDs.Count + 1;
            }
        }

        #endregion

        #region Member Variables

        private bool m_bCancelled;
        private string m_strName;
        private string m_strResultDir;
        private string m_strProgramFile;
        private string m_strPort;
        private string m_strResultFile;
        private uint m_uiBaudRate;
        private OpticalProbeTypes m_ProbeType;
        private string m_strValidationTestsFile;
        private ICommunications m_Comm;
        private string m_strMeterID;
        private ValidationTest m_RunningTest;

        #endregion
    }
}
