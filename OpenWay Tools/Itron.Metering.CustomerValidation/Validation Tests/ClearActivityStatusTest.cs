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
    /// Validation test that clears the activity status
    /// </summary>
    public class ClearActivityStatusTest : ValidationTest
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
        public ClearActivityStatusTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
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
            bool bIsUserAllowed = UserAccess.CheckUserAccess(CXMLOpenWayUserAccessPolicy.FunctionalCapability.ResetActivityStatus);
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
                    m_TestResults.Reason = TestResources.ReasonActivityStatusSealedCanadian;
                }
                else if (false == bIsUserAllowed)
                {
                    m_bTestSkipped = true;
                    m_TestResults.Reason = TestResources.ReasonActivityStatusPermissions;
                }
                else if (Response == PSEMResponse.Ok)
                {
                    ClearDemandResetCount();
                    ClearOutageCount();
                    ClearProgrammedCount();
                    ClearInversionTampers();
                    ClearRemovalTampers();
                    ClearSiteScanDiagCounts();
                    ClearEventLogs();

                    Thread.Sleep(1000);
                    ShowCounters();
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
                return ValidationTestID.ClearActivityStatus;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Shows the activity counters.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        //  04/27/15 jrf 4.20.03 WR 576493 Added check for ICM event log cleared event. 
        private void ShowCounters()
        {
            ISiteScan SiteScanDevice = m_AmiDevice as ISiteScan;
            string strLogClearedEventName = "";
            int LogClearedEventCount = 0;

            if (IsAborted == false)
            {
                AddTestDetail(TestResources.DemandResetCount,
                    GetResultString(m_AmiDevice.NumDemandResets == 0),
                    GetClearCountDetails(m_AmiDevice.NumDemandResets));
                AddTestDetail(TestResources.PowerOutageCount,
                    GetResultString(m_AmiDevice.NumOutages == 0),
                    GetClearCountDetails(m_AmiDevice.NumOutages));
                AddTestDetail(TestResources.ProgrammedCount,
                    GetResultString(m_AmiDevice.NumTimeProgrammed == 0),
                    GetClearCountDetails(m_AmiDevice.NumTimeProgrammed));
                AddTestDetail(TestResources.InversionTamperCount,
                    GetResultString(m_AmiDevice.NumberOfInversionTampers == 0),
                    GetClearCountDetails((int)m_AmiDevice.NumberOfInversionTampers));
                AddTestDetail(TestResources.RemovalTamperCount,
                    GetResultString(m_AmiDevice.NumberOfRemovalTampers == 0),
                    GetClearCountDetails((int)m_AmiDevice.NumberOfRemovalTampers));
                
                if (SiteScanDevice != null)
                {
                    foreach (CDiagnostics.Diag CurrentDiag in SiteScanDevice.Diagnostics.m_Diags)
                    {
                        AddTestDetail(CurrentDiag.Name,
                            GetResultString(CurrentDiag.Count == 0),
                            GetClearCountDetails(CurrentDiag.Count));
                    }
                }

                //Retrieve the register event log cleared event name
                m_AmiDevice.EventDescriptions.TryGetValue((int)CANSIDevice.HistoryEvents.HIST_LOG_CLEARED, out strLogClearedEventName);
                
                LogClearedEventCount = m_AmiDevice.HistoryLogEventList.Where(e => e.Description == strLogClearedEventName && e.Enabled).Count();


                //We need to look for the ICM Event Log Cleared event.  Because of a bug 3G ICM comm module firmware, 
                //the ICM event log cleared event will show up as not supported when it actually is. This means it won't show up in 
                //the HistoryLogEventList property showing all supported events and whether they are monitored. To get around this the
                //code below is only checking ICM comm module monitored events, not supported ones, to see if ICM event log cleared
                //event is configured.
                if (m_AmiDevice.CommModule is ICSCommModule)
                {
                    strLogClearedEventName = "";

                    //Retrieve the ICS event log cleared event name
                    ICS_Gateway_EventDictionary ICMEventDescriptions = new ICS_Gateway_EventDictionary();
                    ICMEventDescriptions.TryGetValue((int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_EVENT_LOG_CLEARED, out strLogClearedEventName);

                    LogClearedEventCount += ((ICSCommModule)m_AmiDevice.CommModule).CommModuleEventMonitored.Where(e => e.Description == strLogClearedEventName).Count();
                }
                 
                AddTestDetail(TestResources.EventLogCount,
                            GetResultString(LogClearedEventCount == m_AmiDevice.Events.Count),
                            GetEventLogCountDetails(LogClearedEventCount, m_AmiDevice.Events.Count));

            }
        }

        /// <summary>
        /// Clears the event logs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void ClearEventLogs()
        {
            ItronDeviceResult Result;

            if (IsAborted == false)
            {
                // Events sometimes take a short amount of time to be posted to the event log so wait a short amount of
                // time to make sure we clear all of the events.
                Thread.Sleep(1500);
                Result = m_AmiDevice.ClearEventLog();

                AddTestDetail(TestResources.ClearEventLogs,
                    GetResultString(Result == ItronDeviceResult.SUCCESS),
                    GetClearDetails(Result == ItronDeviceResult.SUCCESS),
                    DetermineReason(Result));
            }
        }

        /// <summary>
        /// Clears the SiteScan Diag counters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void ClearSiteScanDiagCounts()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            ISiteScan SiteScanDevice = m_AmiDevice as ISiteScan;
            bool bSkipped = SiteScanDevice == null;
            string strReason = "";
            string strDetails = "";

            if (IsAborted == false)
            {
                if (bSkipped == false)
                {
                    Result = SiteScanDevice.ResetDiagCounters();
                    strReason = DetermineReason(Result);
                    strDetails = GetClearDetails(Result == ItronDeviceResult.SUCCESS);
                }
                else
                {
                    strReason = TestResources.ReasonSiteScanNotSupported;
                    strDetails = TestResources.ReasonSiteScanNotSupported;
                }

                AddTestDetail(TestResources.ClearSiteScanDiagnosticCounts,
                    GetResultString(bSkipped, Result == ItronDeviceResult.SUCCESS),
                    strDetails,
                    strReason);
            }
        }

        /// <summary>
        /// Clears the removal tampers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void ClearRemovalTampers()
        {
            ItronDeviceResult Result = m_AmiDevice.ResetNumberRemovalTampers();

            if (IsAborted == false)
            {
                AddTestDetail(TestResources.ClearRemovalTampers,
                    GetResultString(Result == ItronDeviceResult.SUCCESS),
                    GetClearDetails(Result == ItronDeviceResult.SUCCESS),
                    DetermineReason(Result));
            }
        }

        /// <summary>
        /// Clears the inversion tampers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void ClearInversionTampers()
        {
            ItronDeviceResult Result = m_AmiDevice.ResetNumberInversionTampers();

            if (IsAborted == false)
            {
                AddTestDetail(TestResources.ClearInversionTampers,
                    GetResultString(Result == ItronDeviceResult.SUCCESS),
                    GetClearDetails(Result == ItronDeviceResult.SUCCESS),
                    DetermineReason(Result));
            }

        }

        /// <summary>
        /// Clears the number of times programmed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void ClearProgrammedCount()
        {
            ItronDeviceResult Result = m_AmiDevice.ResetNumberTimesProgrammed();

            if (IsAborted == false)
            {
                AddTestDetail(TestResources.ClearProgrammedCount,
                    GetResultString(Result == ItronDeviceResult.SUCCESS),
                    GetClearDetails(Result == ItronDeviceResult.SUCCESS),
                    DetermineReason(Result));
            }
        }

        /// <summary>
        /// Clears the power outage count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void ClearOutageCount()
        {
            ItronDeviceResult Result = m_AmiDevice.ResetNumberPowerOutages();

            if (IsAborted == false)
            {
                AddTestDetail(TestResources.ClearPowerOutageCount,
                    GetResultString(Result == ItronDeviceResult.SUCCESS),
                    GetClearDetails(Result == ItronDeviceResult.SUCCESS),
                    DetermineReason(Result));
            }
        }

        /// <summary>
        /// Clears the demand reset count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way the test details are set.
        private void ClearDemandResetCount()
        {
            ItronDeviceResult Result = m_AmiDevice.ResetNumberDemandResets();

            if (IsAborted == false)
            {
                AddTestDetail(TestResources.ClearDemandResetCount,
                    GetResultString(Result == ItronDeviceResult.SUCCESS),
                    GetClearDetails(Result == ItronDeviceResult.SUCCESS),
                    DetermineReason(Result));
            }
        }

        /// <summary>
        /// Displays the details for the cleared count results.
        /// </summary>
        /// <param name="count">The count of items.</param>
        /// <returns>Details from clearing the count</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/19/14 jrf 4.00.63 WR 534158 Created.
        private string GetClearCountDetails(int count)
        {
            string Details = count.ToString(CultureInfo.CurrentCulture);

            if (0 == count)
            {
                Details += ", " + TestResources.CountCleared;
            }
            else
            {
                Details += ", " + TestResources.CountNotCleared;
            }

            return Details;
        }

        /// <summary>
        /// Displays the details for the cleared event log count results.
        /// </summary>
        /// <param name="clearedEventCount">The count of log cleared events in event log</param>
        /// <param name="totalEventCount">The count of items in event log.</param>
        /// <returns>Details from cleared event log count</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/19/14 jrf 4.00.63 WR 534158 Created.
        //  04/27/15 jrf 4.20.03 WR 576493 Modified to pass in a cleared event and total event count to 
        //                                 determine the appropriate test step details.
        private string GetEventLogCountDetails(int clearedEventCount, int totalEventCount)
        {
            string Details = totalEventCount.ToString(CultureInfo.CurrentCulture);

            if (clearedEventCount > 0)
            {
                // if it's enabled so we should expect cleared event count to equal total event count
                if (clearedEventCount == totalEventCount)
                {
                    Details += ", " + TestResources.EventsCleared;
                }
                else if (clearedEventCount < totalEventCount)
                {
                    Details += ", " + TestResources.EventsNotCleared;
                }
                else if (clearedEventCount > totalEventCount)
                {
                    Details += ", " + TestResources.EventNotFoundInEnabledLog;
                }
            }
            // It's disabled so we should expect 0 events
            else if (0 == totalEventCount)
            {
                Details += ", " + TestResources.LogDisabled;
            }
            else
            {
                Details += ", " + TestResources.EventsFoundInDisabledLog;
            }
            return Details;
        }

        #endregion
    }
}