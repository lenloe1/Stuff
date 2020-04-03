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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Datafiles;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Device;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Validation test that validates the program
    /// </summary>
    public class ValidateProgramTest : ValidationTest
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
        public ValidateProgramTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
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
            List<ProgramValidationItem> InvalidItems = new List<ProgramValidationItem>();

            m_TestResults = new Test();

            m_TestResults.Name = TestName;
            m_bTestPassed = true;

            // Check to see if the program is valid
            if (File.Exists(m_strProgramFile) && EDLFile.IsEDLFile(m_strProgramFile))
            {
                try
                {
                    // Validate the program
                    Response = LogonToDevice();

                    if (Response == PSEMResponse.Ok && IsAborted == false)
                    {
                        InvalidItems = m_AmiDevice.ValidateProgram(m_strProgramFile);

                        if (InvalidItems.Count > 0)
                        {
                            m_bTestPassed = false;

                            if (InvalidItems.Count == 1)
                            {
                                m_TestResults.Reason = TestResources.ReasonOneInvalidItemFound;
                            }
                            else
                            {
                                m_TestResults.Reason = InvalidItems.Count.ToString(CultureInfo.CurrentCulture)
                                    + TestResources.ReasonInvalidItemsFound;
                            }
                        }
                        else
                        {
                            AddTestDetail(TestResources.ProgramValidationErrors, TestResources.OK, TestResources.NonePresent);
                        }
                    }
                    else
                    {
                        m_bTestPassed = false;
                        m_TestResults.Reason = TestResources.ReasonLogonFailed;
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

                // Display the invalid items
                foreach (ProgramValidationItem CurrentItem in InvalidItems)
                {
                    string strMeterValue = "";
                    string strProgramValue = "";
                    string Details = "";
                    string AdditionalDetails = "";

                    if (CurrentItem.MeterValue != null)
                    {
                        strMeterValue = CurrentItem.MeterValue;
                    }

                    if (CurrentItem.ProgramValue != null)
                    {
                        strProgramValue = CurrentItem.ProgramValue;
                    }

                    Details = TestResources.Meter + ":  " + strMeterValue;
                    AdditionalDetails = TestResources.Program + ":  " + strProgramValue;

                    AddTestDetail(CurrentItem.Name, TestResources.Error, Details, "", AdditionalDetails);
                }
            }
            else
            {
                m_bTestSkipped = true;
                m_TestResults.Reason = TestResources.ReasonNoProgramFile;
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
                return ValidationTestID.ValidateProgram;
            }
        }

        #endregion
    }
}