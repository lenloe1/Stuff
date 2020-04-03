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
//                              Copyright © 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server for the OpenWay Advanced Polyphase meter.
    /// </summary>

    public partial class OpenWayAdvPoly : OpenWayBasicPoly
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/09 RCG 2.20.00 N/A    Created

        public OpenWayAdvPoly(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/09 RCG 2.20.00 N/A    Created

        public OpenWayAdvPoly(CPSEM PSEM)
            : base(PSEM)
        {
        }

        /// <summary>
        /// Reconfigures SiteScan information
        /// </summary>
        /// <param name="serviceType">The new service type to use.</param>
        /// <param name="nominalVoltage">The new nominal voltage to use.</param>
        /// <param name="diag1CurrentTolerance">The Diag 1 Current Tolerance</param>
        /// <param name="diag2VoltDeviation">The Diag 2 Voltage Deviation</param>
        /// <param name="diag3CurrentThreshold">The Diag 3 Current Threshold</param>
        /// <param name="diag4PhaseDeviation">The Diag 4 Phase Deviation</param>
        /// <param name="diag4CurrentThreshold">The Diag 4 Current Threshold</param>
        /// <returns>The result of the reconfigure</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/29/09 RCG 2.20.03 N/A    Created
        // 07/08/10 jrf 2.42.02 157552 Setting clear sitescan flag on close config.

        public SiteScanReconfigResult ReconfigureSiteScan(ServiceTypes serviceType, float nominalVoltage, float diag1CurrentTolerance,
            byte diag2VoltDeviation, float diag3CurrentThreshold, float diag4PhaseDeviation, float diag4CurrentThreshold)
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

                    // Diag 1 values
                    SiteScanConfig.Diag1CurrentLeadTolerance = diag1CurrentTolerance;
                    SiteScanConfig.Diag1CurrentLagTolerance = diag1CurrentTolerance;

                    // Diag 2 values
                    SiteScanConfig.Diag2VoltagePercentDeviation = diag2VoltDeviation;

                    // Diag 3 values
                    SiteScanConfig.Diag3CurrentThreshold = diag3CurrentThreshold;

                    // Diag 4 values
                    SiteScanConfig.Diag4CurrentPhaseDeviation = diag4PhaseDeviation;
                    SiteScanConfig.Diag4MinCurrent = diag4CurrentThreshold;

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

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Coincident Values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/09 RCG 2.20.05 N/A    Created

        public override List<Quantity> CoincidentValues
        {
            get
            {
                List<Quantity> Coincidents = new List<Quantity>();
                
                for(int iIndex = 0; iIndex < Table21.NumberOfCoincidentValues; iIndex++)
                {
                    byte bySelection = Table22.CoincidentSelection[iIndex];
                    byte byDemandSelection = Table22.CoincidentDemandAssocations[iIndex];
                    LID CoincidentLID = CreateLID(Table14.SourceIDs[bySelection]);
                    Quantity CoincQuantity = new Quantity(CoincidentLID.lidDescription);

                    // Add the total values
                    CoincQuantity.TotalMaxDemand = new DemandMeasurement(Table23.CurrentRegisters.TotalDataBlock.Coincidents[iIndex].Coincidents[0], CoincidentLID.lidDescription);
                    CoincQuantity.TotalMaxDemand.TimeOfOccurrence = Table23.CurrentRegisters.TotalDataBlock.Demands[byDemandSelection].TimeOfOccurances[0];

                    if (Table21.NumberOfTiers > 0)
                    {
                        CoincQuantity.TOUMaxDemand = new List<DemandMeasurement>();

                        // Add the rate values
                        for (int iRateIndex = 0; iRateIndex < Table21.NumberOfTiers; iRateIndex++)
                        {
                            LID RateLID = GetDemandLIDForRate(CoincidentLID, iRateIndex);

                            CoincQuantity.TOUMaxDemand.Add(new DemandMeasurement(Table23.CurrentRegisters.TierDataBlocks[iRateIndex].Coincidents[iIndex].Coincidents[0], RateLID.lidDescription));
                            CoincQuantity.TOUMaxDemand[iRateIndex].TimeOfOccurrence = Table23.CurrentRegisters.TierDataBlocks[iRateIndex].Demands[byDemandSelection].TimeOfOccurances[0];
                        }
                    }

                    Coincidents.Add(CoincQuantity);
                }

                return Coincidents;
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
                return "OpenWay CENTRON Advanced Poly";
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
                return "OW CENTRON Adv Poly";
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diagnostic 1 is enabled
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsSSDiag1Enabled
        {
            get
            {
                return SiteScanConfig.IsDiag1Enabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diagnostic 2 is enabled
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsSSDiag2Enabled
        {
            get
            {
                return SiteScanConfig.IsDiag2Enabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diagnostic 3 is enabled
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsSSDiag3Enabled
        {
            get
            {
                return SiteScanConfig.IsDiag3Enabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diagnostic 4 is enabled
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public bool IsSSDiag4Enabled
        {
            get
            {
                return SiteScanConfig.IsDiag4Enabled;
            }
        }

        /// <summary>
        /// Gets the Current Tolerance for SiteScan Diag 1
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag1CurrentTolerance
        {
            get
            {
                // The Current Lead and Lag Tolerances should be symmetric
                // so we shall only expose the lead tolerance.
                return SiteScanConfig.Diag1CurrentLeadTolerance;
            }
        }

        /// <summary>
        /// Gets the Phase Voltage Deviation for SiteScan Diag 2 as a percentage.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public byte Diag2VoltagePhasePercentDeviation
        {
            get
            {
                return SiteScanConfig.Diag2VoltagePercentDeviation;
            }
        }

        /// <summary>
        /// Gets the Current Tolerance for SiteScan Diag 3
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag3CurrentThreshold
        {
            get
            {
                return SiteScanConfig.Diag3CurrentThreshold;
            }
        }

        /// <summary>
        /// Gets the Current Phase Deviation for SiteScan Diag 4
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag4CurrentPhaseDeviation
        {
            get
            {
                return SiteScanConfig.Diag4CurrentPhaseDeviation;
            }
        }

        /// <summary>
        /// Gets the Minimum Current for SiteScan Diag 4
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public float Diag4MinimumCurrent
        {
            get
            {
                return SiteScanConfig.Diag4MinCurrent;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the Self Read Coincident Values for the specified Self Read.
        /// </summary>
        /// <param name="uiIndex">The index of the Self Read to get.</param>
        /// <returns>The coincident quantities</returns>
        /// 

        protected override List<Quantity> SRCoincidentValues(uint uiIndex)
        {
            List<Quantity> Coincidents = new List<Quantity>();

            if (uiIndex < Table26.NumberOfValidEntries)
            {
                for (int iIndex = 0; iIndex < Table21.NumberOfCoincidentValues; iIndex++)
                {
                    byte bySelection = Table22.CoincidentSelection[iIndex];
                    byte byDemandSelection = Table22.CoincidentDemandAssocations[iIndex];
                    LID CoincidentLID = CreateLID(Table14.SourceIDs[bySelection]);
                    Quantity CoincQuantity = new Quantity(CoincidentLID.lidDescription);

                    RegisterDataRecord DataRecord = Table26.SelfReadEntries[uiIndex].SelfReadRegisters;

                    // Add the total values
                    CoincQuantity.TotalMaxDemand = new DemandMeasurement(DataRecord.TotalDataBlock.Coincidents[iIndex].Coincidents[0], CoincidentLID.lidDescription);
                    CoincQuantity.TotalMaxDemand.TimeOfOccurrence = DataRecord.TotalDataBlock.Demands[byDemandSelection].TimeOfOccurances[0];

                    if (Table21.NumberOfTiers > 0)
                    {
                        CoincQuantity.TOUMaxDemand = new List<DemandMeasurement>();

                        // Add the rate values
                        for (int iRateIndex = 0; iRateIndex < Table21.NumberOfTiers; iRateIndex++)
                        {
                            LID RateLID = GetDemandLIDForRate(CoincidentLID, iRateIndex);

                            CoincQuantity.TOUMaxDemand.Add(new DemandMeasurement(DataRecord.TierDataBlocks[iRateIndex].Coincidents[iIndex].Coincidents[0], RateLID.lidDescription));
                            CoincQuantity.TOUMaxDemand[iRateIndex].TimeOfOccurrence = DataRecord.TierDataBlocks[iRateIndex].Demands[byDemandSelection].TimeOfOccurances[0];
                        }
                    }

                    Coincidents.Add(CoincQuantity);
                }
            }

            return Coincidents;
        }

        /// <summary>
        /// Gets the Demand Reset Coincident Values for the specified Demand Reset.
        /// </summary>
        /// <param name="uiIndex">The index of the Demand Reset to get.</param>
        /// <returns>The list of coincidents.</returns>
        /// 

        protected override List<Quantity> DRCoincidentValues(uint uiIndex)
        {
            List<Quantity> Coincidents = new List<Quantity>();

            if (uiIndex == 0)
            {
                for (int iIndex = 0; iIndex < Table21.NumberOfCoincidentValues; iIndex++)
                {
                    byte bySelection = Table22.CoincidentSelection[iIndex];
                    byte byDemandSelection = Table22.CoincidentDemandAssocations[iIndex];
                    LID CoincidentLID = CreateLID(Table14.SourceIDs[bySelection]);
                    Quantity CoincQuantity = new Quantity(CoincidentLID.lidDescription);

                    RegisterDataRecord DataRecord = Table25.DemandResetRegisterData;

                    // Add the total values
                    CoincQuantity.TotalMaxDemand = new DemandMeasurement(DataRecord.TotalDataBlock.Coincidents[iIndex].Coincidents[0], CoincidentLID.lidDescription);
                    CoincQuantity.TotalMaxDemand.TimeOfOccurrence = DataRecord.TotalDataBlock.Demands[byDemandSelection].TimeOfOccurances[0];

                    if (Table21.NumberOfTiers > 0)
                    {
                        CoincQuantity.TOUMaxDemand = new List<DemandMeasurement>();

                        // Add the rate values
                        for (int iRateIndex = 0; iRateIndex < Table21.NumberOfTiers; iRateIndex++)
                        {
                            LID RateLID = GetDemandLIDForRate(CoincidentLID, iRateIndex);

                            CoincQuantity.TOUMaxDemand.Add(new DemandMeasurement(DataRecord.TierDataBlocks[iRateIndex].Coincidents[iIndex].Coincidents[0], RateLID.lidDescription));
                            CoincQuantity.TOUMaxDemand[iRateIndex].TimeOfOccurrence = DataRecord.TierDataBlocks[iRateIndex].Demands[byDemandSelection].TimeOfOccurances[0];
                        }
                    }

                    Coincidents.Add(CoincQuantity);
                }
            }

            return Coincidents;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a LID value for the specified rate.
        /// </summary>
        /// <param name="originalLID">The original LID that should be changed.</param>
        /// <param name="iRate">The rate to change to (0 = A, 1 = B, etc)</param>
        /// <returns>The resulting LID</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/18/09 RCG 2.20.05        Created

        private LID GetDemandLIDForRate(LID originalLID, int iRate)
        {
            uint uiNewLIDNumber;

            // Clear the current rate value;
            uiNewLIDNumber = originalLID.lidValue & ~(uint)DefinedLIDs.TOU_Data.TOU_DATA_MASK;

            // Set the new value
            switch(iRate)
            {
                case 0:
                {
                    uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_A;
                    break;
                }
                case 1:
                {
                    uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_B;
                    break;
                }
                case 2:
                {
                    uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_C;
                    break;
                }
                case 3:
                {
                    uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_D;
                    break;
                }
                case 4:
                {
                    uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_E;
                    break;
                }
                case 5:
                {
                    uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_F;
                    break;
                }
                case 6:
                {
                    uiNewLIDNumber |= (uint)DefinedLIDs.TOU_Data.RATE_G;
                    break;
                }
            }

            return CreateLID(uiNewLIDNumber);
        }

        #endregion

    }
}
