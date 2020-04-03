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
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Validation test the shows the normal display
    /// </summary>
    public class NormalDisplayTest : ValidationTest
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
        public NormalDisplayTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
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

            try
            {

                Response = LogonToDevice();

                if (Response == PSEMResponse.Ok)
                {
                    foreach (DisplayItem CurrentItem in m_AmiDevice.NormalDisplayList)
                    {
                        if (IsAborted == false)
                        {
                            AddTestDetail(CurrentItem.Description, TestResources.OK, CurrentItem.Value);
                        }
                    }

                    if (null == m_AmiDevice.NormalDisplayList || 0 == m_AmiDevice.NormalDisplayList.Count)
                    {
                        AddTestDetail(TestResources.NormalDisplayItems, TestResources.OK, TestResources.NonePresent);
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
                return ValidationTestID.ShowNormalDisplay;
            }
        }

        #endregion
    }
}