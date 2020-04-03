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
//                            Copyright © 2009 - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Threading;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server for the OpenWay Basic Polyphase meter.
    /// </summary>

    public partial class OpenWayBasicPoly : CENTRON_AMI
    {
        #region Constants

        private const double CPC_MULTIPLIER = 0.1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/09 RCG 2.20.00 N/A    Created

        public OpenWayBasicPoly(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
            m_LID = new CentronPolyDefinedLIDs();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/09 RCG 2.20.00 N/A    Created

        public OpenWayBasicPoly(CPSEM PSEM)
            : base(PSEM)
        {
            m_LID = new CentronPolyDefinedLIDs();
        }

        /// <summary>
        /// Reconfigures SiteScan information
        /// </summary>
        /// <param name="serviceType">The new service type to use.</param>
        /// <param name="nominalVoltage">The new nominal voltage to use.</param>
        /// <returns>The result of the reconfigure</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/29/09 RCG 2.20.03 N/A    Created

        public SiteScanReconfigResult ReconfigureSiteScan(ServiceTypes serviceType, float nominalVoltage)
        {
            CloseConfigErrors Errors = CloseConfigErrors.None;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;
            PSEMResponse Response = PSEMResponse.Ok;
            SiteScanReconfigResult ReconfigResult = SiteScanReconfigResult.SUCCESS;

            // First make sure we have read the SiteScan config so we have the correct data.
            Response = SiteScanConfig.Read();

            if (Response == PSEMResponse.Ok)
            {
                // Open the config so that we can write to it.
                ProcResult = OpenConfig();

                if (ProcResult == ProcedureResultCodes.COMPLETED)
                {
                    // Set the new values.
                    SiteScanConfig.ServiceType = serviceType;
                    SiteScanConfig.NominalVoltage = nominalVoltage;

                    // Write the new values
                    Response = SiteScanConfig.Write();

                    if (Response == PSEMResponse.Ok)
                    {
                        ProcResult = CloseConfig(CloseConfigOptions.SiteScan, out Errors);
                    }
                }
            }

            if (ProcResult == ProcedureResultCodes.NO_AUTHORIZATION)
            {
                ReconfigResult = SiteScanReconfigResult.SECURITY_ERROR;
            }
            else if (ProcResult != ProcedureResultCodes.COMPLETED)
            {
                ReconfigResult = SiteScanReconfigResult.ERROR;
            }

            if (Response == PSEMResponse.Isc)
            {
                ReconfigResult = SiteScanReconfigResult.SECURITY_ERROR;
            }
            else if (Response != PSEMResponse.Ok)
            {
                ReconfigResult = SiteScanReconfigResult.PROTOCOL_ERROR;
            }

            return ReconfigResult;
        }

        /// <summary>
        /// Reconfigures the LED Quantity on a Polyphase meter.
        /// </summary>
        /// <param name="quantity">The quantity to reconfigure.</param>
        /// <param name="pulseWeight">The pulse weight to use. Note: The pulse weight will be divided by 40 to get the actual value.</param>
        /// <returns>The result of the procedure call.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/07/11 RCG 2.50.06 N/A    Created
        
        public ProcedureResultCodes ReconfigurePolyLEDQuantity(LEDQuantity quantity, ushort pulseWeight)
        {
            ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcResponse = null;
            MemoryStream ParameterStream = new MemoryStream(new byte[6]);
            PSEMBinaryWriter ParameterWriter = new PSEMBinaryWriter(ParameterStream);

            ParameterWriter.Write(quantity.QuantityID);

            if (VersionChecker.CompareTo(FWRevision, VERSION_HYDROGEN_3_10) == 0)
            {
                // 3.10 builds had a bug in this procedure so we need to set this based on the Meter Class
                ParameterWriter.Write(CalculatePulseWeightValueForLEDReconfigureBug(pulseWeight));
            }
            else
            {
                ParameterWriter.Write(pulseWeight);
            }

            Result = ExecuteProcedure(Procedures.POLY_LED_RECONFIGURE, ParameterStream.ToArray(), out ProcResponse);

            return Result;
        }

        /// <summary>
        /// Gets the current LED Quantity
        /// </summary>
        /// <returns>The currently pulsing quantity or null if the procedure fails.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/07/11 RCG 2.50.06 N/A    Created

        public LEDQuantity GetCurrentPolyLEDQuantity()
        {
            LEDQuantity CurrentQuantity = null;
            ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcResponse = null;
            byte[] ProcParam = new byte[] {0x00, 0x00, 0x00, 0x80, 0x00, 0x00};
            MemoryStream ResponseStream = null;
            PSEMBinaryReader ResponseReader = null;

            Result = ExecuteProcedure(Procedures.POLY_LED_RECONFIGURE, ProcParam, out ProcResponse);

            if(Result == ProcedureResultCodes.COMPLETED && ProcResponse != null && ProcResponse.Length >= 4)
            {
                ResponseStream = new MemoryStream(ProcResponse);
                ResponseReader = new PSEMBinaryReader(ResponseStream);

                CurrentQuantity = new LEDQuantity(ResponseReader.ReadUInt32());
            }

            return CurrentQuantity;
        }

        /// <summary>
        /// Gets the current pulse weight
        /// </summary>
        /// <returns>The current pulse weight or null if the procedure fails</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/15/15 AF  4.00.92  WR 535491  Created
        //  03/25/15 AF  4.10.09  WR 573774  The scale factor for the response depends on the meter class
        //
        public float? GetCurrentPolyPulseWeight()
        {
            float? PulseWeight = null;
            ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcResponse = null;
            byte[] ProcParam = new byte[] { 0x00, 0x00, 0x00, 0x80, 0x00, 0x00 };
            MemoryStream ResponseStream = null;
            PSEMBinaryReader ResponseReader = null;

            Result = ExecuteProcedure(Procedures.POLY_LED_RECONFIGURE, ProcParam, out ProcResponse);

            if (Result == ProcedureResultCodes.COMPLETED && ProcResponse != null && ProcResponse.Length >= 6)
            {
                ResponseStream = new MemoryStream(ProcResponse);
                ResponseReader = new PSEMBinaryReader(ResponseStream);

                ResponseReader.ReadUInt32();
                PulseWeight = (float)(ResponseReader.ReadUInt16()) / (float)DetermineEnergyDivisor();  // Scale factor depends on the meter class
            }

            return PulseWeight;
        }

        /// <summary>
        /// Configures the meter to use the specified base energies
        /// </summary>
        /// <param name="baseEnergies">The base energy values to use.</param>
        /// <returns>The result of the procedure call</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/22/11 RCG 2.50.12 N/A    Created

        public override ProcedureResultCodes ConfigureBaseEnergies(List<BaseEnergies> baseEnergies)
        {
            ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcResponse = null;
            byte[] ProcParam = new byte[6];

            // Validate the energies parameter
            if (baseEnergies == null)
            {
                throw new ArgumentNullException("baseEnergies");
            }
            else if (baseEnergies.Count > ProcParam.Length)
            {
                throw new ArgumentException("Only " + ProcParam.Length.ToString(CultureInfo.InvariantCulture) + " energies may be configured", "baseEnergies");
            }

            // Add the energy values
            for (int iIndex = 0; iIndex < baseEnergies.Count; iIndex++)
            {
                ProcParam[iIndex] = (byte)baseEnergies[iIndex];
            }

            Result = ExecuteProcedure(Procedures.CONFIGURE_BASE_ENERGIES, ProcParam, out ProcResponse);

            // Give the meter a second to make sure that the base changes before we do anything else
            Thread.Sleep(1000);

            return Result;
        }


        /// <summary>
        /// This method executes the enter/exit test mode.
        /// </summary>
        /// <param name="modeType">TestMode Type</param>
        /// <param name="timeInTestMode">time for the meter to remain in test minutes</param>
        /// <returns>The result of the test mode operation.</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/29/09 MMD           N/A   Created
        // 06/05/09 jrf 2.20.08 135495  Added a check to wait up to 5 seconds until
        //                              the device enters or exits test mode.
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        //                              
        public virtual ItronDeviceResult EnterExitTestMode(TestMode modeType, byte timeInTestMode)
        {
            //EnterExitTestModeResult Result = EnterExitTestModeResult.BUSY;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcParam = new byte[8];
            OpenWayMFGTable2170 Table2170 = new OpenWayMFGTable2170(m_PSEM, Table00);
            byte[] ProcResponse;
            PSEMBinaryWriter PSEMWriter = new PSEMBinaryWriter(new MemoryStream(ProcParam));
            bool blnWaitResponse = true;
            int iSeconds = 0;

            PSEMWriter.Write((byte)modeType);
            PSEMWriter.Write(timeInTestMode);
            PSEMWriter.Write(Table2170.PulseQuantityTestID);
            PSEMWriter.Write(Table2170.PulseWeightTest);

            ProcResult = ExecuteProcedure(Procedures.ENTER_EXIT_TEST_MODE, ProcParam, out ProcResponse);

            if (ProcedureResultCodes.COMPLETED == ProcResult)
            {
                if (TestMode.ENTER_TEST_MODE == modeType)
                {
                    //If we are entering test mode then we will wait
                    //while IsInTestMode property is false.
                    blnWaitResponse = false;
                }

                while (blnWaitResponse == IsInTestMode && 5 > iSeconds)
                {
                    //Check mode every 1 sec. for 5 sec.
                    System.Threading.Thread.Sleep(1000);
                    iSeconds += 1;
                }
            }

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }
            return Result;
        }

        /// <summary>
        /// This method executes the enter/exit test mode.
        /// </summary>
        /// <param name="modeType">TestMode Type</param>
        /// <param name="timeInTestMode">time for the meter to remain in test minutes</param>
        /// <param name="pulseWeight">actual test mode kh</param>
        /// <returns>The result of the test mode operation.</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/29/09 MMD           N/A   Created
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        // 01/20/15 AF  4.00.92 556942  Moved the multiplication by 40 here. Scaling of the parameter should 
        //                              happen here and not in the application or test code.
        // 01/28/15 AF  4.10.00 556942  Updated the pulse weight parameter comment per code review
        //
        public virtual ItronDeviceResult EnterExitTestMode(TestMode modeType, byte timeInTestMode, float pulseWeight)
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam = new byte[8];
            OpenWayMFGTable2170 Table2170 = new OpenWayMFGTable2170(m_PSEM, Table00);
            byte[] ProcResponse;
            PSEMBinaryWriter PSEMWriter = new PSEMBinaryWriter(new MemoryStream(ProcParam));
            bool blnWaitResponse = true;
            int iSeconds = 0;
            //Pulse Weight Test value should be in increments of 0.025 so multiply by 40
            float fPulseWeightParam = pulseWeight * 40;

            PSEMWriter.Write((byte)modeType);
            PSEMWriter.Write(timeInTestMode);
            PSEMWriter.Write(Table2170.PulseQuantityTestID);
            PSEMWriter.Write((ushort)fPulseWeightParam);

            ProcResult = ExecuteProcedure(Procedures.ENTER_EXIT_TEST_MODE, ProcParam, out ProcResponse);

            if (ProcedureResultCodes.COMPLETED == ProcResult)
            {
                if (TestMode.ENTER_TEST_MODE == modeType)
                {
                    //If we are entering test mode then we will wait
                    //while IsInTestMode property is false.
                    blnWaitResponse = false;
                }

                while (blnWaitResponse == IsInTestMode && 5 > iSeconds)
                {
                    //Check mode every 1 sec. for 5 sec.
                    System.Threading.Thread.Sleep(1000);
                    iSeconds += 1;
                }
            }

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;
        }

        /// <summary>
        /// This method executes the enter/exit test mode.
        /// </summary>
        /// <param name="modeType">TestMode Type</param>
        /// <param name="timeInTestMode">time for the meter to remain in test minutes</param>
        /// <param name="pulseWeight">actual test mode kh</param>
        /// /// <param name="pulseQuantity">Quantity to be measured in test mode</param>
        /// <returns>The result of the test mode operation.</returns>
        // MM/DD/YY who Version Issue#   Description
        // -------- --- ------- -------- ---------------------------------------
        // 06/23/17 CFB 4.72.02 WR767058 Created
        //
        public virtual ItronDeviceResult EnterExitTestMode(TestMode modeType, byte timeInTestMode, float pulseWeight, uint pulseQuantity)
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam = new byte[8];
            OpenWayMFGTable2170 Table2170 = new OpenWayMFGTable2170(m_PSEM, Table00);
            byte[] ProcResponse;
            PSEMBinaryWriter PSEMWriter = new PSEMBinaryWriter(new MemoryStream(ProcParam));
            bool blnWaitResponse = true;
            int iSeconds = 0;
            //Pulse Weight Test value should be in increments of 0.025 so multiply by 40
            float fPulseWeightParam = pulseWeight * 40;

            PSEMWriter.Write((byte)modeType);
            PSEMWriter.Write(timeInTestMode);
            PSEMWriter.Write(pulseQuantity);
            PSEMWriter.Write((ushort)fPulseWeightParam);

            ProcResult = ExecuteProcedure(Procedures.ENTER_EXIT_TEST_MODE, ProcParam, out ProcResponse);

            if (ProcedureResultCodes.COMPLETED == ProcResult)
            {
                if (TestMode.ENTER_TEST_MODE == modeType)
                {
                    //If we are entering test mode then we will wait
                    //while IsInTestMode property is false.
                    blnWaitResponse = false;
                }

                while (blnWaitResponse == IsInTestMode && 5 > iSeconds)
                {
                    //Check mode every 1 sec. for 5 sec.
                    System.Threading.Thread.Sleep(1000);
                    iSeconds += 1;
                }
            }

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;
        }

        /// <summary>
        /// This method updates the IO configuration in the meter.
        /// </summary>
        /// <param name="IOConfig">KYZ configuration data object.</param>
        /// <returns>The result of the configuration.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/22/09 jrf 2.20.02 n/a	   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 07/08/10 jrf 2.42.02 157552 Setting the CPC reset bit on close config to fix
        //                             LED lock issue.
        //
        public virtual ItronDeviceResult ConfigureIO(KYZData IOConfig)
        {
            byte[] ProcParam = new byte[0];
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult =
                ProcedureResultCodes.INVALID_PARAM;
            PSEMResponse Result = PSEMResponse.Err;

            ItronDeviceResult ConfigResult = ItronDeviceResult.ERROR;
            CTable2048_OpenWay OW2048 = Table2048 as CTable2048_OpenWay;
            CENTRON_AMI_IOConfig IOTable = OW2048.IOConfig as CENTRON_AMI_IOConfig;

            // Open the Config
            ProcResult = ExecuteProcedure(Procedures.OPEN_CONFIG_FILE, ProcParam, out ProcResponse);

            // Execute Write of IO Table in 2048
            if (ProcedureResultCodes.COMPLETED == ProcResult)
            {
                IOTable.IOData = IOConfig;
                Result = IOTable.Write();
            }
            else if (ProcedureResultCodes.NO_AUTHORIZATION == ProcResult)
            {
                Result = PSEMResponse.Isc;
            }
            else
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "Open config procedure failed with result = " +
                    ProcResult);
                Result = PSEMResponse.Err;
            }

            if (Result == PSEMResponse.Ok)
            {
                // Close the Config
                // Data reset bits - we don't want to reset any data, so 
                // just initialize them to 0
                ProcParam = new byte[4];
                ProcParam.Initialize();

                // Okay we do want to reset data if we are a HW2.0 Poly.  Need to do this to
                // prevent the worm on the display from freezing.
                if (0 == VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_2_5)
                    || 0 == VersionChecker.CompareTo(HWRevisionFiltered, HW_VERSION_2_6))
                {
                    //We need to reset CPC
                    MemoryStream ParamStream = new MemoryStream(ProcParam);
                    BinaryWriter BinWriter = new BinaryWriter(ParamStream);

                    BinWriter.Write((uint)CloseConfigOptions.CPC);
                }

                ProcResult = ExecuteProcedure(Procedures.CLOSE_CONFIG_FILE, ProcParam, out ProcResponse);

                if (ProcedureResultCodes.COMPLETED != ProcResult)
                {
                    ConfigResult = ItronDeviceResult.ERROR;
                }
                else
                {
                    ConfigResult = ItronDeviceResult.SUCCESS;
                }
            }
            else
            {
                if (Result == PSEMResponse.Isc)
                {
                    ConfigResult = ItronDeviceResult.SECURITY_ERROR;
                }
                else
                {
                    ConfigResult = ItronDeviceResult.ERROR;
                }
            }

            return ConfigResult;
        }

        /// <summary>
        /// Gets the list of Energies required by the program.
        /// </summary>
        /// <param name="programFile">The path to the program file</param>
        /// <returns>The list of required energies</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/22/11 RCG 2.50.12 N/A    Created
        // 10/15/14 jrf 4.00.73 539220 Made method public for use in QC Tool.
        public override List<BaseEnergies> GetRequiredEnergiesFromProgram(string programFile)
        {
            List<BaseEnergies> RequiredEnergies = new List<BaseEnergies>();
            XmlReader Reader;
            XmlReaderSettings ReaderSettings = new XmlReaderSettings();

            // Create the CentronTables object to read the file
            ReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
            ReaderSettings.IgnoreWhitespace = true;
            ReaderSettings.IgnoreComments = true;
            ReaderSettings.CheckCharacters = false;

            Reader = XmlReader.Create(programFile, ReaderSettings);

            CentronTables ProgramTables = new CentronTables();

            ProgramTables.LoadEDLFile(Reader);

            // Get the energy configuration
            for (int iIndex = 0; iIndex < NumberOfSupportedEnergies; iIndex++)
            {
                object objValue = null;
                int[] Indicies = new int[] { iIndex };

                if (ProgramTables.IsCached((long)CentronTblEnum.MFGTBL0_ENERGY_LID, Indicies))
                {
                    ProgramTables.GetValue(CentronTblEnum.MFGTBL0_ENERGY_LID, Indicies, out objValue);

                    // We need to add the Secondary Energy Base value to the byte returned to get the 
                    // actual LID value
                    LID EnergyLid = CreateLID(SEC_ENERGY_LID_BASE + (byte)objValue);

                    switch (EnergyLid.lidQuantity)
                    {
                        case DefinedLIDs.WhichOneEnergyDemand.WH_DELIVERED:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyWhDel) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyWhDel);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.WH_RECEIVED:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyWhRec) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyWhRec);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.WH_UNI:
                        case DefinedLIDs.WhichOneEnergyDemand.WH_NET:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyWhDel) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyWhDel);
                                }

                                if (RequiredEnergies.Contains(BaseEnergies.PolyWhRec) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyWhRec);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVAhArithDel) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVAhArithDel);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVAhArithRec) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVAhArithRec);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVAhVectDel) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVAhVectDel);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVAhVectRec) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVAhVectRec);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VAH_LAG:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVAhLag) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVAhLag);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VARH_DEL:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVarhDel) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVarhDel);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VARH_REC:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVarhRec) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVarhRec);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VARH_NET:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVarhDel) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVarhDel);
                                }

                                if (RequiredEnergies.Contains(BaseEnergies.PolyVarhRec) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVarhRec);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VARH_Q1:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVarhQ1) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVarhQ1);
                                }

                                break;
                            }
                        case DefinedLIDs.WhichOneEnergyDemand.VARH_Q4:
                            {
                                if (RequiredEnergies.Contains(BaseEnergies.PolyVarhQ4) == false)
                                {
                                    RequiredEnergies.Add(BaseEnergies.PolyVarhQ4);
                                }

                                break;
                            }
                    }
                }
            }

            ProgramTables = null;
            Reader.Close();

            return RequiredEnergies;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the form of the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created

        public string MeterForm
        {
            get
            {
                string strForm;

                if (Table2091 != null)
                {
                    strForm = EnumDescriptionRetriever.RetrieveDescription(Table2091.MeterForm);
                }
                else
                {
                    // TODO: Remove this code when meter has support for 2091
                    PSEMResponse Result;
                    object objValue;
                    strForm = m_rmStrings.GetString("UNKNOWN");

                    Result = m_lidRetriever.RetrieveLID(m_LID.METER_FORM_FACTOR, out objValue);

                    if (PSEMResponse.Ok == Result && objValue != null && objValue.GetType() == typeof(byte))
                    {
                        strForm = ((byte)objValue).ToString(CultureInfo.InvariantCulture) + "S";
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Meter Form"));
                    }
                }

                return strForm;
            }
        }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/23/09 RCG 2.20.00 N/A    Created

        public override string MeterName
        {
            get
            {
                return "OpenWay CENTRON Basic Poly";
            }
        }

        /// <summary>
        /// Gets the meter name that will be used in the activity log.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/08/09 RCG 2.20.11		Created

        public override string ActivityLogMeterName
        {
            get
            {
                return "OW CENTRON Basic Poly";
            }
        }

        /// <summary>
        /// Gets the configured Service Type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public ServiceTypes ConfiguredServiceType
        {
            get
            {
                return SiteScanConfig.ServiceType;
            }
        }

        /// <summary>
        /// Gets the configured Nominal Voltage.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float ConfiguredNominalVoltage
        {
            get
            {
                return SiteScanConfig.NominalVoltage;
            }
        }

        /// <summary>
        /// Gets the number seconds to delay untill service sense.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public byte DelayUntillServiceSense
        {
            get
            {
                return SiteScanConfig.DelayUntillServiceSense;
            }
        }


        /// <summary>
        /// Property to retrieve Test Quantity out of the Metrology Table
        /// </summary>
        public string TestQuantity
        {
            get
            {
                if (Table2170 != null)
                {
                    return Table2170.PulseQuantityTest.Description;
                }
                else
                {
                    throw new InvalidOperationException("Meter CPC is not supported in this meter.");
                }

            }
        }

        /// <summary>
        /// Property to retrieve Test Kh out of the Metrology Table
        /// </summary>
        public double PulseWeightTestKh
        {
            get
            {
                if (Table2170 != null)
                {
                    return Convert.ToDouble(Table2170.PulseWeightTest/(double)40);
                }
                else
                {
                    throw new InvalidOperationException("Meter CPC is not supported in this meter.");
                }

            }
        }

        /// <summary>
        /// Gets the current toolbox data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created
        // 06/06/12 jrf 2.60.28 199358 Moved property here so basic poly phase could use it.
        //
        public Toolbox ToolboxData
        {
            get
            {
                Toolbox NewToolbox = new Toolbox();

                if (Table2091 != null)
                {
                    // Instantaneous Values
                    NewToolbox.m_dInsKW = Table2091.InsW / 1000.0;
                    NewToolbox.m_dInsKVA = Table2091.InsVA / 1000.0;
                    NewToolbox.m_dInsKVAArith = NewToolbox.m_dInsKVA;
                    NewToolbox.m_dInsKVAVect = NewToolbox.m_dInsKVA;
                    NewToolbox.m_dInsKVar = Table2091.InsVar / 1000.0;
                    NewToolbox.m_dInsPF = Table2091.InsPF;

                    // Voltage Values
                    NewToolbox.m_fVoltsA = Table2091.InsVoltsPhaseA;
                    NewToolbox.m_fVoltsB = Table2091.InsVoltsPhaseB;
                    NewToolbox.m_fVoltsC = Table2091.InsVoltsPhaseC;
                    NewToolbox.m_fVAngleA = 0.0;
                    NewToolbox.m_fVAngleB = Table2091.VoltsPhaseBAngle;
                    NewToolbox.m_fVAngleC = Table2091.VoltsPhaseCAngle;

                    // Current Values
                    NewToolbox.m_fCurrentA = Table2091.InsAmpsPhaseA;
                    NewToolbox.m_fCurrentB = Table2091.InsAmpsPhaseB;
                    NewToolbox.m_fCurrentC = Table2091.InsAmpsPhaseC;
                    NewToolbox.m_fIAngleA = Table2091.AmpsPhaseAAngle;
                    NewToolbox.m_fIAngleB = Table2091.AmpsPhaseBAngle;
                    NewToolbox.m_fIAngleC = Table2091.AmpsPhaseCAngle;
                }

                return NewToolbox;
            }
        }


        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the Table 2048 object for the current device.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        internal override CTable2048 Table2048
        {
            get
            {
                if (null == m_Table2048)
                {
                    m_Table2048 = new CTable2048_OpenWayPoly(m_PSEM);
                }

                return m_Table2048;
            }
        }

        /// <summary>
        /// Gets the SiteScan configuration object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        internal OpenWayPolySiteScanConfig SiteScanConfig
        {
            get
            {
                if (null == m_SiteScanConfig)
                {
                    CTable2048_OpenWayPoly PolyTable2048 = Table2048 as CTable2048_OpenWayPoly;

                    if (PolyTable2048 != null)
                    {
                        m_SiteScanConfig = PolyTable2048.SiteScanConfig;
                    }
                }

                return m_SiteScanConfig;
            }
        }

        /// <summary>
        /// Gets the SiteScan Toolbox table 2091 and creates it if necessary
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created
        //  10/24/12 jrf 2.70.33 238238 Adding passing in FW version to table for it to use
        //                              to determine its size.
        //
        internal OpenWayPolyMFGTable2091 Table2091
        {
            get
            {
                if (m_Table2091 == null && Table00.IsTableUsed(2091))
                {
                    m_Table2091 = new OpenWayPolyMFGTable2091(m_PSEM, FWRevision);
                }

                return m_Table2091;
            }
        }

        /// <summary>
        /// Gets the Meter CPC table and creates it if needed. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/09 MMD           N/A    Created

        private OpenWayMFGTable2170 Table2170
        {
            get
            {
                if (null == m_Table2170 && true == Table00.IsTableUsed(2170))
                {
                    m_Table2170 = new OpenWayMFGTable2170(m_PSEM, Table00);
                }

                return m_Table2170;
            }
        }



        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the MFG Table 2088 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/11 RCG 2.52.29        Created
        
        protected Table2088 Table2088
        {
            get
            {
                if (m_Table2088 == null && Table00.IsTableUsed(2088))
                {
                    m_Table2088 = new Table2088(m_PSEM);
                }

                return m_Table2088;
            }
        }

        /// <summary>
        /// Determines the Energy Divisor value for the meter
        /// </summary>
        /// <returns>The Energy Divisor to use</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/11 RCG 2.52.29        Created

        protected double DetermineEnergyDivisor()
        {
            double EnergyDivisor = 1.0;

            if (Table2088 != null)
            {
                switch(Table2088.SnapshotMeterClass)
                {
                    case MeterClass.CLASS_2:
                    {
                        EnergyDivisor = 1280.0;
                        break;
                    }
                    case MeterClass.CLASS_10:
                    {
                        EnergyDivisor = 320.0;
                        break;
                    }
                    case MeterClass.CLASS_20:
                    {
                        EnergyDivisor = 160.0;
                        break;
                    }
                    case MeterClass.CLASS_100:
                    case MeterClass.CLASS_150:
                    {
                        EnergyDivisor = 20.0;
                        break;
                    }
                    case MeterClass.CLASS_480:
                    {
                        EnergyDivisor = 5.0;
                        break;
                    }
                    default:
                    {
                        EnergyDivisor = 10.0;
                        break;
                    }
                }
            }

            return EnergyDivisor;
        }

        /// <summary>
        /// Calculates the pulse weight that should be used in cases where the LED Reconfigure bug was present
        /// </summary>
        /// <param name="unmodifiedPulseWeight">The unmodified pulse weight.</param>
        /// <returns>The modified pulse weight</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/11 RCG 2.52.29        Created
        
        protected ushort CalculatePulseWeightValueForLEDReconfigureBug(ushort unmodifiedPulseWeight)
        {
            return (ushort)(unmodifiedPulseWeight * CPC_MULTIPLIER * DetermineEnergyDivisor() / 4);
        }

        /// <summary>
        /// Gets the number of energies that can be configured to the base by this device
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/11 RCG 2.50.18        Created

        protected override int NumberofBaseConfigurableEnergies
        {
            get
            {
                return 6;
            }
        }

        #endregion

        #region Member Variables

        private OpenWayPolySiteScanConfig m_SiteScanConfig;
        private OpenWayPolyMFGTable2091 m_Table2091;
        private OpenWayMFGTable2170 m_Table2170 = null;
        private Table2088 m_Table2088 = null;
        //CTable03 m_Table03

        #endregion
    }
}
