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
using System.IO;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Device;
using Itron.Metering.ReplicaSettings;
using Itron.Metering.Datafiles;
using Itron.Metering.Utilities;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Validation test that validates the security keys
    /// </summary>
    public class ValidateSecurityKeysTest : ValidationTest
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
        public ValidateSecurityKeysTest(ICommunications comm, uint uiBaudRate, string programFile, SignedAuthorizationKey authKey)
            : base(comm, uiBaudRate, programFile, authKey)
        {
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Using new method to set test result string 
        //  
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
                    if (VersionChecker.CompareTo(m_AmiDevice.FWRevision, CENTRON_AMI.VERSION_2_SP5) >= 0)
                    {
                        ValidateOpticalPasswords();
                        ValidateDESKeys();
                        ValidateEnhancedSecurityKeys();
                        ValidateHANKeys();
                    }
                    else
                    {
                        m_bTestSkipped = true;
                        m_TestResults.Reason = TestResources.ReasonSP5OrLaterRequired;
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
                return ValidationTestID.ValidateSecurityKeys;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the optical passwords
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void ValidateOpticalPasswords()
        {
            string Details = "";
            string Reason = "";
            bool Skipped = File.Exists(m_strProgramFile) == false 
                || EDLFile.IsEDLFile(m_strProgramFile) == false;
            ProcedureResultCodes ValidationResult = ProcedureResultCodes.COMPLETED;

            if (IsAborted == false)
            {
                foreach (CENTRON_AMI.OpticalPasswords PasswordType in Enum.GetValues(typeof(CENTRON_AMI.OpticalPasswords)))
                {
                    if (Skipped == false)
                    {
                        ValidationResult = m_AmiDevice.ValidateOpticalPasswords(m_strProgramFile, PasswordType);
                        Details = GetSecurityValidationDetails(ValidationResult);

                        if (ProcedureResultCodes.INVALID_PARAM == ValidationResult)
                        {
                            Details += ", " + TestResources.PasswordNotConsistentWithProgram;
                        }
                    }
                    else
                    {
                        Reason = TestResources.ReasonProgramFileNeededToValidate;
                        Details = TestResources.NoProgram;
                    }

                    AddTestDetail(PasswordType.ToDescription(), GetResultString(Skipped, ProcedureResultCodes.COMPLETED == ValidationResult),
                            Details, Reason);
                }
            }
        }        

        /// <summary>
        /// Validates the DES Keys
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void ValidateDESKeys()
        {
            string Details = "";
            string Reason = "";
            bool Skipped = File.Exists(m_strProgramFile) == false
                || EDLFile.IsEDLFile(m_strProgramFile) == false;
            ProcedureResultCodes ValidationResult = ProcedureResultCodes.COMPLETED;

            if (IsAborted == false)
            {
                foreach (CENTRON_AMI.DESKeys KeyType in Enum.GetValues(typeof(CENTRON_AMI.DESKeys)))
                {
                    if (Skipped == false)
                    {
                        ValidationResult = m_AmiDevice.ValidateDESKeys(m_strProgramFile, KeyType);
                        Details = GetSecurityValidationDetails(ValidationResult);

                        if (ProcedureResultCodes.INVALID_PARAM == ValidationResult)
                        {
                            Details += ", " + TestResources.KeyNotConsistentWithProgram;
                        }
                    }
                    else
                    {
                        Reason = TestResources.ReasonProgramFileNeededToValidate;
                        Details = TestResources.NoProgram;
                    }

                    AddTestDetail(KeyType.ToDescription(), GetResultString(Skipped, ProcedureResultCodes.COMPLETED == ValidationResult),
                            Details, Reason);
                }
            }
        }

        /// <summary>
        /// Validates the enhanced security keys
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private void ValidateEnhancedSecurityKeys()
        {
            CXMLOpenWaySystemSettings SystemSettings = new CXMLOpenWaySystemSettings("");
            bool Skipped = File.Exists(SystemSettings.EnhancedSecurityFilePath) == false;
            string Details = "";
            string Reason = "";
            ProcedureResultCodes ValidationResult = ProcedureResultCodes.COMPLETED;

            if (IsAborted == false)
            {
                foreach (CENTRON_AMI.EnhancedKeys KeyType in Enum.GetValues(typeof(CENTRON_AMI.EnhancedKeys)))
                {
                    if (Skipped == false)
                    {
                        ValidationResult = m_AmiDevice.ValidateEnhancedSecurityKey(SystemSettings.EnhancedSecurityFilePath, KeyType);
                        Details = GetSecurityValidationDetails(ValidationResult);

                        if (ProcedureResultCodes.INVALID_PARAM == ValidationResult)
                        {
                            Details += ", " + TestResources.KeyNotConsistentWithSecurityFile;
                        }
                    }
                    else
                    {
                        Reason = TestResources.ReasonProgramFileNeededToValidate;
                        Details = TestResources.NoProgram;
                    }

                    AddTestDetail(KeyType.ToDescription(), GetResultString(Skipped, ProcedureResultCodes.COMPLETED == ValidationResult),
                            Details, Reason);
                }
            }
        }

        /// <summary>
        /// Validates the HAN keys
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/17/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        //  05/23/17 CFB 4.72.00 WR 741323 Removed validation of HAN link key as it is no longer used in the meter
        private void ValidateHANKeys()
        {
            string NetworkKey = (string)CRegistryHelper.GetApplicationValue("GasPro", "Key");
            string LinkKey = (string)CRegistryHelper.GetApplicationValue("GasPro", "GlobalLinkKey");
            ProcedureResultCodes ValidationResult = ProcedureResultCodes.COMPLETED;
            string Details = "";

            if (IsAborted == false)
            {
                //Network Key validation
                ValidationResult = m_AmiDevice.ValidateHANSecurityKeys(NetworkKey, CENTRON_AMI.HANKeys.NetworkKey);
                Details = GetSecurityValidationDetails(ValidationResult);

                if (ProcedureResultCodes.INVALID_PARAM == ValidationResult)
                {
                    Details += ", " + TestResources.KeyNotConsistentWithRegistry;
                }

                AddTestDetail(TestResources.HANNetworkKey, GetResultString(ProcedureResultCodes.COMPLETED == ValidationResult),
                            Details);
            }
        }

        /// <summary>
        /// Displays the details for a security validation.
        /// </summary>
        /// <param name="ValidationResult">The result of the security validation.</param>
        /// <returns>Details from security validation.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/23/14 jrf 4.00.63 WR 534158 Modified way test details are set.
        private string GetSecurityValidationDetails(ProcedureResultCodes ValidationResult)
        {
            string Details = "";

            if (ProcedureResultCodes.COMPLETED == ValidationResult)
            {
                Details = TestResources.Valid;
            }
            else if (ProcedureResultCodes.INVALID_PARAM == ValidationResult)
            {
                Details = TestResources.Invalid;
            }
            else
            {
                Details = TestResources.ValidationFailed;
            }

            return Details;
        }

        #endregion
    }
}