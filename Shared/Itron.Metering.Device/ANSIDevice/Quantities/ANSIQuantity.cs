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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class represents ANSI Quantities.  Contains all information needed to 
    /// 	/retrieve meansurements associated with a given quantity.
    /// </summary>
    internal class ANSIQuantity : Quantity
    {
        #region Public Properties

        /// <summary>
        /// Provides read access to the Energy Data
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //
        public override Measurement TotalEnergy
        {
            get
            {
                if (null == m_EnergyMeasurement)
                {
                    if (true == m_ANSIDevice.ValidateEnergy(TotalEnergyLID))
                    {
                        // If the Energy exits and it has not been created, create it.
                        m_EnergyMeasurement = new Measurement();
                        // Set the Description Fields for the Quantities to be retrieved
                        m_EnergyMeasurement.Description = TotalEnergyLID.lidDescription;
            
                        // Do Read of Energy Quantity and set the data 
                        //	into the Energy Measurement
                        ReadTotalEnergy();
                    }
                }

                return m_EnergyMeasurement;
            }
        }

        /// <summary>
        /// Provides read access to the Max Demand data.
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //
        public override DemandMeasurement TotalMaxDemand
        {
            get
            {
                if (null == m_MaxDemandMeasurement)
                {
                    if (true == m_ANSIDevice.ValidateDemand(TotalMaxDemandLID))
                    {
                        // If the Energy exits and it has not been created, create it.
                        m_MaxDemandMeasurement = new DemandMeasurement();
                        m_MaxDemandMeasurement.Description = TotalMaxDemandLID.lidDescription;

                        // Do Read of Max Demand Quantity and set the 
                        //	data into the Demand Measurement
                        ReadTotalMaxDemand();
                        ReadTOOTotalMaxDemand();
                    }
                }

                return m_MaxDemandMeasurement;
            }
        }

        /// <summary>
        /// Provides read access to the TOU Energy Data
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //
        public override List<Measurement> TOUEnergy
        {
            get
            {
                if (null == m_TOUEnergyMeasurements)
                {
                    if (true == m_ANSIDevice.ValidateEnergy(TotalEnergyLID))
                    {
                        // If the Energy exits and it has not been created, create it.
                        m_TOUEnergyMeasurements = new List<Measurement>();

                        // Do Read of TOU Energy Quantities and set the data 
                        //	into the Energy Measurement List
                        ReadTOUEnergy();
                    }
                }

                return m_TOUEnergyMeasurements;
            }
        }

        /// <summary>
        /// Provides read access to the TOU Max Demand data.
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //
        public override List<DemandMeasurement> TOUMaxDemand
        {
            get
            {
                if (null == m_TOUMaxDemandMeasurements)
                {
                    if (true == m_ANSIDevice.ValidateDemand(TotalMaxDemandLID))
                    {
                        // If the Energy exits and it has not been created, create it.
                        m_TOUMaxDemandMeasurements = new List<DemandMeasurement>();

                        // Do Read of TOU Max Demand Quantities and set the 
                        //	data into the Demand Measurement
                        ReadTOUMaxDemand();
                        ReadTOOTOUMaxDemand();
                    }
                }

                return m_TOUMaxDemandMeasurements;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// ANSI Quantity Constructor - Base Class for all ANSI Quantities
        /// </summary>
        /// <param name="strDescription"> A user displayable description of the quantity instance</param>
        /// <param name="psem">PSEM Object</param>
        /// <param name="ANSIDevice">ANSIDevice Object</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //
        internal ANSIQuantity(String strDescription, CPSEM psem, ANSIMeter ANSIDevice)
            : base( strDescription )
        {
            m_ANSIDevice = ANSIDevice;
            m_PSEM = psem;
            m_uiTOURates = m_ANSIDevice.NumberTOURates;
        }

        #endregion

        #region Internal Properties
        /// <summary>
        /// This property provides access to the LID used to
        /// 	retrieve Total Energy.
        /// </summary>
        internal LID TotalEnergyLID
        {
            get
            {
                return m_TotalEnergyLID;
            }
            set
            {
                m_TotalEnergyLID = value;
            }
        }

        /// <summary>
        /// This property provies access to the LID used to 
        /// 	retrieve Total Max Demand.
        /// </summary>
        internal LID TotalMaxDemandLID
        {
            get
            {
                return m_TotalMaxDemandLID;
            }
            set
            {
                m_TotalMaxDemandLID = value;
            }
        }

        /// <summary>
        /// This property provides access to the Base LID used to
        /// 	retrieve TOU Energy.
        /// </summary>
        internal LID TOUBaseEnergyLID
        {
            get
            {
                return m_TOUBaseEnergyLID;
            }
            set
            {
                m_TOUBaseEnergyLID = value;
            }
        }

        /// <summary>
        /// This property provies access to the Base LID used to 
        /// 	retrieve TOU Max Demand.
        /// </summary>
        internal LID TOUBaseMaxDemandLID
        {
            get
            {
                return m_TOUBaseMaxDemandLID;
            }
            set
            {
                m_TOUBaseMaxDemandLID = value;
            }
        }
            
        #endregion

        #region Private Methods
        /// <summary>
        /// Reads the Energy LID and set the value into the Energy Measurement
        /// </summary>
        /// <returns>bool - True if it succeeds; False if it fails</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.35			Created
        //
        private void ReadTotalEnergy()
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;

            PSEMResult = m_ANSIDevice.m_lidRetriever.RetrieveLID(TotalEnergyLID, out Data);

            if (PSEMResponse.Ok == PSEMResult)
            {
                m_EnergyMeasurement.Value = m_ANSIDevice.m_lidRetriever.DataReader.ReadDouble();
            }
            else
            {
                throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                            "Error Reading Total Energy Quantities; LID = " + TotalEnergyLID.ToString());
            }
        }

        /// <summary>
        /// Reads the Max Demand LID and set the value into the Demand Measurement
        /// </summary>
        /// <returns>bool - True if it succeeds; False if it fails</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.35			Created
        //
        private void ReadTotalMaxDemand()
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;

            PSEMResult = m_ANSIDevice.m_lidRetriever.RetrieveLID(TotalMaxDemandLID, out Data);

            if (PSEMResponse.Ok == PSEMResult)
            {
                // Demand Values are stored in the meter as Singles, but we are going
                //  to deal with all data as double
                m_MaxDemandMeasurement.Value = (double)m_ANSIDevice.m_lidRetriever.DataReader.ReadSingle();
            }
            else
            {
                throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                            "Error Reading Total Demand Quantities; LID = " + TotalMaxDemandLID.ToString());
            }
        }

        /// <summary>
        /// Reads the Max Demand LID and set the value into the Demand Measurement
        /// </summary>
        /// <exception cref="PSEMException">Exception on read</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/27/06 KRC 7.35			Created
		//	05/15/08 mrj 1.50.25		Bug itron00107508, set date to UTC
        //
        private void ReadTOOTotalMaxDemand()
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;
            uint uiMeterSeconds;
            DateTime dtTOODemand = m_ANSIDevice.UTCMeterReferenceTime;

            LID TOOMaxDemandLID = m_ANSIDevice.CreateLID((TotalMaxDemandLID.lidValue & (uint)DefinedLIDs.WhichDemandFormat.WHICH_FORMAT_MASK_OUT) |
                                           (uint)DefinedLIDs.WhichDemandFormat.TOO_DATA);
            PSEMResult = m_ANSIDevice.m_lidRetriever.RetrieveLID(TOOMaxDemandLID, out Data);

            if (PSEMResponse.Ok == PSEMResult)
            {
                uiMeterSeconds = m_ANSIDevice.m_lidRetriever.DataReader.ReadUInt32();

                // Add the seconds to the reference time
                dtTOODemand = dtTOODemand.AddSeconds((double)uiMeterSeconds);

                // Covert the time to Local (if needed - Method checks firmware version)
                dtTOODemand = m_ANSIDevice.GetLocalDeviceTime(dtTOODemand);

                m_MaxDemandMeasurement.TimeOfOccurrence = dtTOODemand;
            }
            else
            {
                throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                            "Error Reading Time of Occurence Value; LID = " + TOOMaxDemandLID.ToString());
            }
        }

        /// <summary>
        /// Reads the TOU Energy LIDs and set the value into the Measurement List
        /// </summary>
        /// <exception cref="PSEMException">Exception on read</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.35			Created
        //
        private void ReadTOUEnergy()
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;
            LID[] arLids = new LID[m_uiTOURates];

            if (m_uiTOURates > 0)
            {
                arLids = BuildEnergyTOULIDArray(TOUBaseEnergyLID);

                // Retrieve the TOU LID's
                PSEMResult = m_ANSIDevice.m_lidRetriever.RetrieveMulitpleLIDs(arLids, out Data);

                if (PSEMResponse.Ok == PSEMResult)
                {
                    for (uint uiCount = 0; uiCount < m_uiTOURates; uiCount++)
                    {
                        // This adds the value and the Description at the same time.
                        m_TOUEnergyMeasurements.Add(new Measurement(
                                                m_ANSIDevice.m_lidRetriever.DataReader.ReadDouble(),
                                                arLids[uiCount].lidDescription));
                    }
                }
                else
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                                "Error Reading TOU Energy Quantities; Base LID = " + TOUBaseEnergyLID.ToString());
                }
            }
        }

        /// <summary>
        /// Reads the Max Demand LID and set the value into the Demand Measurement
        /// </summary>
        /// <exception cref="PSEMException">Exception on read</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.36			Created
        //
        private void ReadTOUMaxDemand()
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;
            LID[] arLids = new LID[m_uiTOURates];

            if (m_uiTOURates > 0)
            {
                arLids = BuildDemandTOULIDArray(TOUBaseMaxDemandLID);
                
                // Retrieve TOU Demand LIDs
                PSEMResult = m_ANSIDevice.m_lidRetriever.RetrieveMulitpleLIDs(arLids, out Data);

                if (PSEMResponse.Ok == PSEMResult)
                {
                    for (uint uiCount = 0; uiCount < m_uiTOURates; uiCount++)
                    {
                        // Demand Values are stored in the meter as Singles, but we are going
                        //  to deal with all data as double
                        m_TOUMaxDemandMeasurements.Add(new DemandMeasurement(
                                            (double)m_ANSIDevice.m_lidRetriever.DataReader.ReadSingle(),
                                            arLids[uiCount].lidDescription));
                    }
                }
                else
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                                "Error Reading TOU Demand Quantities; Base LID = " + TOUBaseMaxDemandLID.ToString());
                }
            }
        }

        /// <summary>
        /// Reads the TOO for the TOU Demand LIDs and set the value into the Demand Measurement
        /// </summary>
        /// <exception cref="PSEMException">Exception on read</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/27/06 KRC 7.36			Created
        //  12/01/06 jrf 8.00           The dtTOOTOUDemand must be reset to the 
        //                              MeterReferenceTime for each TOU rate.
		//	05/15/08 mrj 1.50.25		Bug itron00107508, set date to UTC
        //
        private void ReadTOOTOUMaxDemand()
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;
            LID[] arLids = new LID[m_uiTOURates];
            DateTime dtTOOTOUDemand = m_ANSIDevice.UTCMeterReferenceTime;
            uint uiMeterSeconds = 0;

            if (true == m_ANSIDevice.ValidateDemand(TotalMaxDemandLID) && m_uiTOURates > 0)
            {
                // Calculate the Base LID for the TOO of the TOU LID
                LID TOOTOUBaseMaxDemandLID = m_ANSIDevice.CreateLID((TOUBaseMaxDemandLID.lidValue & (uint)DefinedLIDs.WhichDemandFormat.WHICH_FORMAT_MASK_OUT) |
                                               (uint)DefinedLIDs.WhichDemandFormat.TOO_DATA);
                arLids = BuildTOOTOULIDArray(TOOTOUBaseMaxDemandLID);

                // Retrieve TOU Demand LIDs
                PSEMResult = m_ANSIDevice.m_lidRetriever.RetrieveMulitpleLIDs(arLids, out Data);

                PSEMBinaryReader TOOValues = m_ANSIDevice.m_lidRetriever.DataReader;

                if (PSEMResponse.Ok == PSEMResult)
                {
                    for (uint uiCount = 0; uiCount < m_uiTOURates; uiCount++)
                    {
                        // Reset to reference time
                        dtTOOTOUDemand = m_ANSIDevice.UTCMeterReferenceTime;

                        uiMeterSeconds = TOOValues.ReadUInt32();

                        // Add the seconds to the reference time
                        dtTOOTOUDemand = dtTOOTOUDemand.AddSeconds((double)uiMeterSeconds);
                        
                        // Now convert to Local Time (Handles cases where Time is in UTC)
                        dtTOOTOUDemand = m_ANSIDevice.GetLocalDeviceTime(dtTOOTOUDemand);

                        m_TOUMaxDemandMeasurements[(int)uiCount].TimeOfOccurrence = dtTOOTOUDemand;
                    }
                }
                else
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                                "Error Reading TOU TOO Values; Base LID = " + TOOTOUBaseMaxDemandLID.ToString());
                }
            }
        }

        /// <summary>
        /// Builds the TOU Energy LID Array to be retrieved
        /// </summary>
        /// <param name="BaseTOULID">Base LID to append Rates to</param>
        /// <returns>LID[] - Array of TOU LIDs to retrieve</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/27/06 KRC 7.35.00 N/A    Created
        //
        private LID[] BuildEnergyTOULIDArray(LID BaseTOULID)
        {
            uint[] auiRates = {(uint)DefinedLIDs.TOU_Data.RATE_A, (uint)DefinedLIDs.TOU_Data.RATE_B, 
                                (uint)DefinedLIDs.TOU_Data.RATE_C, (uint)DefinedLIDs.TOU_Data.RATE_D, 
                                (uint)DefinedLIDs.TOU_Data.RATE_E, (uint)DefinedLIDs.TOU_Data.RATE_F,
                                (uint)DefinedLIDs.TOU_Data.RATE_G };
            LID[] arLids = new LID[m_uiTOURates];

            for (uint uiIndex = 0; uiIndex < m_uiTOURates; uiIndex++)
            {
                arLids[uiIndex] = m_ANSIDevice.CreateLID(BaseTOULID.lidValue | auiRates[uiIndex]);
            }

            return arLids;
        }

        /// <summary>
        /// Builds the TOU Demand LID Array to be retrieved
        /// </summary>
        /// <param name="BaseTOULID">Base LID to append Rates to</param>
        /// <returns>LID[] - Array of TOU LIDs to retrieve</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/27/06 KRC 7.35.00 N/A    Created
        //
        private LID[] BuildDemandTOULIDArray(LID BaseTOULID)
        {
            uint[] auiRates = {(uint)DefinedLIDs.TOU_Data.RATE_A, (uint)DefinedLIDs.TOU_Data.RATE_B, 
                                (uint)DefinedLIDs.TOU_Data.RATE_C, (uint)DefinedLIDs.TOU_Data.RATE_D, 
                                (uint)DefinedLIDs.TOU_Data.RATE_E, (uint)DefinedLIDs.TOU_Data.RATE_F,
                                (uint)DefinedLIDs.TOU_Data.RATE_G };
            LID[] arLids = new LID[m_uiTOURates];

            for (uint uiIndex = 0; uiIndex < m_uiTOURates; uiIndex++)
            {
                arLids[uiIndex] = m_ANSIDevice.CreateLID(BaseTOULID.lidValue | auiRates[uiIndex]);
            }

            return arLids;
        }

        /// <summary>
        /// Builds the TOU TOO LID Array to be retrieved
        /// </summary>
        /// <param name="BaseTOULID">Base LID to append Rates to</param>
        /// <returns>LID[] - Array of TOU LIDs to retrieve</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/27/06 KRC 7.35.00 N/A    Created
        //
        private LID[] BuildTOOTOULIDArray(LID BaseTOULID)
        {
            uint[] auiRates = {(uint)DefinedLIDs.TOU_Data.RATE_A, (uint)DefinedLIDs.TOU_Data.RATE_B, 
                                (uint)DefinedLIDs.TOU_Data.RATE_C, (uint)DefinedLIDs.TOU_Data.RATE_D, 
                                (uint)DefinedLIDs.TOU_Data.RATE_E, (uint)DefinedLIDs.TOU_Data.RATE_F,
                                (uint)DefinedLIDs.TOU_Data.RATE_G };
            LID[] arLids = new LID[m_uiTOURates];

            for (uint uiIndex = 0; uiIndex < m_uiTOURates; uiIndex++)
            {
                arLids[uiIndex] = m_ANSIDevice.CreateLID(BaseTOULID.lidValue | auiRates[uiIndex]);
            }

            return arLids;
        }

 
        #endregion

        #region Members
    
        /// <summary>
        /// Number of TOU Rates supported by current device (size of TOU List)
        /// </summary>
        protected uint m_uiTOURates;

        /// <summary>
        /// ANSIDevice Object
        /// </summary>
        protected ANSIMeter m_ANSIDevice;

        private CPSEM m_PSEM;

        private LID m_TotalEnergyLID;
        private LID m_TotalMaxDemandLID;
        private LID m_TOUBaseEnergyLID;
        private LID m_TOUBaseMaxDemandLID;

        #endregion

    }

	/// <summary>
	/// This object represents an ANSI instantaneous quantity.  It contains
	/// all information to retrieve the quantity.
	/// </summary>
	internal class ANSIInstantaneousQuantity : InstantaneousQuantity
	{	
		#region Internal Methods

		/// <summary>
        /// Constructor
        /// </summary>                
        /// <param name="ANSIDevice">ANSIDevice Object</param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/14/07 mrj 8.00.12		Created
        //  06/09/11 jrf 2.51.10 173353 Making changes required due to update to Instantaneous Quantity class.
		//  
		internal ANSIInstantaneousQuantity(CANSIDevice ANSIDevice)
            : base("")
        {
            m_ANSIDevice = ANSIDevice;            
			m_PhaseALID = null;
			m_PhaseBLID = null;
			m_PhaseCLID = null;
			m_AggregateLID = null;
        }
		
		/// <summary>
		/// Reads the LIDs and fills in the measurements for the instantaneous
		/// quantity.
		/// </summary>
		/// <exception cref="Exception">Thrown if Read fails</exception>\
		/// <remarks>
		/// </remarks>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/14/07 mrj 8.00.12		Created
        //  06/09/11 jrf 2.51.10 173353 Making changes required due to update to Instantaneous Quantity class.
		//  
		internal void ReadQuantity()
		{
			PSEMResponse Result;			
			List<object> lstQuantityData = null;
			LID[] QuantityLIDs;

			//Check to see if this quantity supports the aggregate quantity
			if (m_AggregateLID == null)
			{
				QuantityLIDs = new LID[3];				
			}
			else
			{
				QuantityLIDs = new LID[4];
				QuantityLIDs[3] = m_AggregateLID;
			}

			//Set the phase quantity to the array
			QuantityLIDs[0] = m_PhaseALID;
			QuantityLIDs[1] = m_PhaseBLID;
			QuantityLIDs[2] = m_PhaseCLID;

			//Read the LIDs
			Result = m_ANSIDevice.m_lidRetriever.RetrieveMulitpleLIDs(QuantityLIDs, out lstQuantityData);

			if (Result == PSEMResponse.Ok && 
				null != lstQuantityData && 
				lstQuantityData.Count >= 3)
			{
                MostRecentMeasurementsPerPhase = new List<DemandMeasurement>();

                if (TypeCode.Single == m_PhaseALID.lidType)
				{
					//Create the measurements with the data. Adding phases A, B and C.
                    MostRecentMeasurementsPerPhase.Add(new DemandMeasurement((double)((float)lstQuantityData[0]), m_PhaseALID.lidDescription));
                    MostRecentMeasurementsPerPhase.Add(new DemandMeasurement((double)((float)lstQuantityData[1]), m_PhaseBLID.lidDescription));
                    MostRecentMeasurementsPerPhase.Add(new DemandMeasurement((double)((float)lstQuantityData[2]), m_PhaseCLID.lidDescription));

					if (lstQuantityData.Count == 4)
					{
                        //Adding the aggregate value
                        MostRecentMeasurement = new DemandMeasurement((double)((float)lstQuantityData[3]), m_AggregateLID.lidDescription);
					}
				}
				else if (TypeCode.Double == m_PhaseALID.lidType)
				{
                    //Create the measurements with the data. Adding phases A, B and C.
                    MostRecentMeasurementsPerPhase.Add(new DemandMeasurement((double)lstQuantityData[0], m_PhaseALID.lidDescription));
                    MostRecentMeasurementsPerPhase.Add(new DemandMeasurement((double)lstQuantityData[1], m_PhaseBLID.lidDescription));
                    MostRecentMeasurementsPerPhase.Add(new DemandMeasurement((double)lstQuantityData[2], m_PhaseCLID.lidDescription));

					if (lstQuantityData.Count == 4)
					{
                        //Adding the aggregate value
                        MostRecentMeasurement = new DemandMeasurement((double)lstQuantityData[3], m_AggregateLID.lidDescription);
					}
				}
			}
			else
			{
				throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
							"Error reading instantaneous quantity"));
			}
		}
		
		#endregion Internal Methods

		#region Internal Properties
		/// <summary>
		/// This property provides access to the LID used to retrieve
		/// the phase A instantaneous quantity.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/13/07 mrj 8.00.12		Created
		//  
		internal LID PhaseALID
		{
			get
			{
				return m_PhaseALID;
			}
			set
			{
				m_PhaseALID = value;
			}
		}

		/// <summary>
		/// This property provides access to the LID used to retrieve
		/// the phase B instantaneous quantity.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/13/07 mrj 8.00.12		Created
		//  
		internal LID PhaseBLID
		{
			get
			{
				return m_PhaseBLID;
			}
			set
			{
				m_PhaseBLID = value;
			}
		}

		/// <summary>
		/// This property provides access to the LID used to retrieve
		/// the phase C instantaneous quantity.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/13/07 mrj 8.00.12		Created
		//  
		internal LID PhaseCLID
		{
			get
			{
				return m_PhaseCLID;
			}
			set
			{
				m_PhaseCLID = value;
			}
		}

		/// <summary>
		/// This property provides access to the LID used to retrieve
		/// the aggregate instantaneous quantity.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/13/07 mrj 8.00.12		Created
		//  
		internal LID AggregateLID
		{
			get
			{
				return m_AggregateLID;
			}
			set
			{
				m_AggregateLID = value;
			}
		}
		#endregion Internal Properties

		#region Members

		/// <summary>
		/// ANSIDevice Object
		/// </summary>
		protected CANSIDevice m_ANSIDevice;
		
		private LID m_PhaseALID;
		private LID m_PhaseBLID;
		private LID m_PhaseCLID;
		private LID m_AggregateLID;

		#endregion Members
	}
}