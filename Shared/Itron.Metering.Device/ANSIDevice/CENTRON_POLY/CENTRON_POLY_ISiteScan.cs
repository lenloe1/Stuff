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
//                              Copyright © 2006-2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Resources;
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the CENTRON (V and I). (ISiteScan implementation)
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 05/22/06 mrj 7.30.00 N/A    Created
    ///
    public partial class CENTRON_POLY : CANSIDevice, ISiteScan
    {
        #region Methods
        /// <summary>
        /// Implements the ISiteScan interface.  Resets the diagnostic counters
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/24/06 mrj 7.30.00 N/A    Created
        ///
        ItronDeviceResult ISiteScan.ResetDiagCounters()
        {
            return ResetDiagnosticCounters();
        }

		/// <summary>
		/// Clears the sitescan snapshots in the meter.  This is not supported by
		/// the Vectron.
		/// </summary>
		/// <returns>A ItronDeviceResult</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/21/07 mrj 8.00.13		Created
		//  
		ItronDeviceResult ISiteScan.ClearSiteScanSnapshots()
		{
			ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
			ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
			byte[] ProcParam;
			byte[] ProcResponse;

			ProcParam = new byte[0];  // No parameters for this procedure
			ProcResult = ExecuteProcedure(Procedures.CLEAR_SITESCAN_SNAPSHOTS,
				ProcParam, out ProcResponse);

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

        #endregion

        #region Properties
        /// <summary>
        /// Implements the ISiteScan interface.  Returns a Toolbox object containing
        /// all the toolbox data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/26/06 mrj 7.30.00 N/A    Created
		//  02/13/07 mrj 8.00.12		Added support for instantaneous toolbox
		//  							items.
		//  	
        Toolbox ISiteScan.ToolboxData
        {
			get
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading ToolBox Data");

				Toolbox toolbox = new Toolbox();
				PSEMResponse Result = PSEMResponse.Err;
				byte[] Data = null;
				LID[] lids = new LID[7];
				byte byVACalcMethod;

				lids[0] = m_LID.ALL_SITESCAN;
				lids[1] = m_LID.VA_CALC_METHOD;
				lids[2] = m_LID.INST_W;
				lids[3] = m_LID.INST_VAR;
				lids[4] = m_LID.INST_VA_ARITH;
				lids[5] = m_LID.INST_VA_VECT;
				lids[6] = m_LID.INST_PF;


				Result = m_lidRetriever.RetrieveMulitpleLIDs(lids, out Data);
				if (PSEMResponse.Ok == Result)
				{
					MemoryStream TempStream = new MemoryStream(Data);
					BinaryReader TempBReader = new BinaryReader(TempStream);

					//Set the values to the toolbox structure
					toolbox.m_fVoltsA = (double)TempBReader.ReadSingle();
					toolbox.m_fVAngleA = 0.0;								//This is always zero
					toolbox.m_fCurrentA = (double)TempBReader.ReadSingle();
					toolbox.m_fIAngleA = (double)TempBReader.ReadSingle();
					toolbox.m_fVoltsB = (double)TempBReader.ReadSingle();
					toolbox.m_fVAngleB = (double)TempBReader.ReadSingle();
					toolbox.m_fCurrentB = (double)TempBReader.ReadSingle();
					toolbox.m_fIAngleB = (double)TempBReader.ReadSingle();
					toolbox.m_fVoltsC = (double)TempBReader.ReadSingle();
					toolbox.m_fVAngleC = (double)TempBReader.ReadSingle();
					toolbox.m_fCurrentC = (double)TempBReader.ReadSingle();
					toolbox.m_fIAngleC = (double)TempBReader.ReadSingle();


					//Trash these 8 bytes that are not needed
					TempBReader.ReadSingle();
					TempBReader.ReadSingle();


					//Get the VA calculation method, this is needed to determine which VA to 
					//return
					byVACalcMethod = TempBReader.ReadByte();

					//Get the readings, and convert to kilo
					toolbox.m_dInsKW = (double)TempBReader.ReadSingle() / 1000.0;
					toolbox.m_dInsKVar = (double)TempBReader.ReadSingle() / 1000.0;

					//Figure out which VA to return
					toolbox.m_dInsKVAArith = TempBReader.ReadSingle() / 1000.0;
					toolbox.m_dInsKVAVect = TempBReader.ReadSingle() / 1000.0;
					if (0 == byVACalcMethod)
					{
						//Arithmetic calculations
                        toolbox.m_dInsKVA = toolbox.m_dInsKVAArith;
					}
					else
					{
						//Vectorial calculations
                        toolbox.m_dInsKVA = toolbox.m_dInsKVAVect;
					}

					toolbox.m_dInsPF = (double)TempBReader.ReadSingle();
				}				

				return toolbox;
			}
        }

        /// <summary>
        /// Implements the ISiteScan interface.  Returns a Diag object containing
        /// all the diagnostic data.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/24/06 mrj 7.30.00 N/A    Created
        ///
        CDiagnostics ISiteScan.Diagnostics
        {
            get
            {
                CDiagnostics Diag = new CDiagnostics(false);
                PSEMResponse Result = PSEMResponse.Err;
                byte[] Data = null;
                LID[] LIDs = new LID[8];


                //Create the array of LIDs for the diagnotics
                LIDs[0] = m_LID.SITESCAN_DIAG_1_COUNT;
                LIDs[1] = m_LID.SITESCAN_DIAG_2_COUNT;
                LIDs[2] = m_LID.SITESCAN_DIAG_3_COUNT;
                LIDs[3] = m_LID.SITESCAN_DIAG_4_COUNT;
                LIDs[4] = m_LID.SITESCAN_DIAG_5_COUNT;
                LIDs[5] = m_LID.SITESCAN_DIAG_5_COUNT_A;
                LIDs[6] = m_LID.SITESCAN_DIAG_5_COUNT_B;
                LIDs[7] = m_LID.SITESCAN_DIAG_5_COUNT_C;

                //Get the values from the meter
                Result = m_lidRetriever.RetrieveMulitpleLIDs(LIDs, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    //Get the Diag 1 count
                    Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_1].Count = Data[0];

                    //Get the Diag 2 count				
                    Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_2].Count = Data[1];

                    //Get the Diag 3 count				
                    Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_3].Count = Data[2];

                    //Get the Diag 4 count
                    Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_4].Count = Data[3];

                    //Get the Diag 5 count
                    Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5T].Count = Data[4];

                    //Get the Diag 5a count
                    Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5A].Count = Data[5];

                    //Get the Diag 5b count
                    Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5B].Count = Data[6];

                    //Get the Diag 5c count
                    Diag.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_5C].Count = Data[7];
                }

                //Get the Diagnostic present byte from the meter
                Result = m_lidRetriever.RetrieveLID(m_LID.STATEMON_DIAG_ERRORS, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    //Check to see if any of the diagnostics are active
                    CheckActiveDiags(Data[0], ref Diag);
                }         


                return Diag;
            }
        }

        /// <summary>
        /// Implements the ISiteScan interface.  Returns the the service type.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/26/06 mrj 7.30.00 N/A    Created
        /// 09/12/06 mrj 7.35.00        Changed to use type-safe static string
        ///                             class.
        ///
        string ISiteScan.ServiceType
        {
            get
            {
                byte[] Data = null;
                string strServiceType = m_rmStrings.GetString("UNKNOWN");

                //Get the firmware revision from the LID				
                if (PSEMResponse.Ok == m_lidRetriever.RetrieveLID(m_LID.SITESCAN_SERVICE_TYPE, out Data))
                {
                    //Create the service type string
					strServiceType = ServiceType(Data[0]); 
                }
				

                return strServiceType;
            }
        }
				
		/// <summary>
		/// Implements the ISiteScan interface.  Returns the the meter's form.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/06/07 mrj 8.00.10		Created
		//  
		string ISiteScan.MeterForm
		{
			get
			{
				PSEMResponse Result;
				object objValue;
				string strForm = m_rmStrings.GetString("UNKNOWN");

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

				return strForm;
			}
		}

		/// <summary>
		/// Proporty can be used to determine if snapshots are supported.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		bool ISiteScan.SnapshotsSupported
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Property to get the number of available snapshots.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/20/07 mrj 8.00.13		Created
		//  
		int ISiteScan.SnapshotCount
		{
			get
			{
				Table2088 table2088 = new Table2088(m_PSEM);				
				return table2088.NumberSnapshots;
			}
		}

		/// <summary>
		/// Returns the list of snapshots in the meter.
		/// </summary>
		/// <remarks>
		/// This property will cause the snapshots to be read each time this is called.
		/// Snapshots are not cached in the device object.
		/// </remarks>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/20/07 mrj 8.00.13		Created
		//  
		List<SnapshotEntry> ISiteScan.SiteScanSnapshots
		{
			get
			{
				bool bPQSupported = MeterKeyTable.PQSupported;

				Table2088 table2088 = new Table2088(m_PSEM);
				Table2089 table2089 = new Table2089(m_PSEM, table2088, bPQSupported, false);
				List<Table2089Snapshot> Table2089SnapshotList = table2089.Table2089SnapshotData;
				List<SnapshotEntry> SnapshotList = new List<SnapshotEntry>();

				//Loop through table 2089 and create the snapshots to be returned
				foreach (Table2089Snapshot Table2089SnapshotData in Table2089SnapshotList)
				{
					SnapshotEntry SnapshotData = new SnapshotEntry();

					//Set the snapshot data and convert if necessary
					SnapshotData.SnapshotTime = MeterReferenceTime.AddSeconds(Table2089SnapshotData.SnapshotTimeSeconds);
					SnapshotData.SnapshotTrigger = (int)Table2089SnapshotData.SnapshotTrigger;
					SnapshotData.ProgramID = (int)table2088.SnapshotProgramID;
                    SnapshotData.MeterForm = table2088.SnapshotMeterForm.ToString(CultureInfo.InvariantCulture) + "S";
					SnapshotData.MeterService = ServiceType(Table2089SnapshotData.MeterService);
					SnapshotData.FirmwareVersion = table2088.SnapshotFirmwareVer;
					SnapshotData.PeakCurrentSupported = false;					
					SnapshotData.InsWatt = (double)Table2089SnapshotData.InsWatt / 1000000.0;
					SnapshotData.InsVA = (double)Table2089SnapshotData.InsVA / 1000000.0;
					SnapshotData.InsVar = (double)Table2089SnapshotData.InsVar / 1000000.0;
					SnapshotData.InsPF = (double)Table2089SnapshotData.InsPF / 10000.0;
					SnapshotData.IRMSSupported = false;					
					SnapshotData.LineFrequency = (double)Table2089SnapshotData.LineFrequency / 100.0;
					SnapshotData.VoltagePhaseA = (double)Table2089SnapshotData.VoltagePhaseA / 40.0;
					SnapshotData.VoltagePhaseB = (double)Table2089SnapshotData.VoltagePhaseB / 40.0;
					SnapshotData.VoltagePhaseC = (double)Table2089SnapshotData.VoltagePhaseC / 40.0;
					SnapshotData.VoltageAnglePhaseA = (double)Table2089SnapshotData.VoltageAnglePhaseA / 10.0;
					SnapshotData.VoltageAnglePhaseB = (double)Table2089SnapshotData.VoltageAnglePhaseB / 10.0;
					SnapshotData.VoltageAnglePhaseC = (double)Table2089SnapshotData.VoltageAnglePhaseC / 10.0;
					SnapshotData.CurrentPhaseA = ConvertCurrent(table2088.SnapshotMeterClass, Table2089SnapshotData.CurrentPhaseA);
					SnapshotData.CurrentPhaseB = ConvertCurrent(table2088.SnapshotMeterClass, Table2089SnapshotData.CurrentPhaseB);
					SnapshotData.CurrentPhaseC = ConvertCurrent(table2088.SnapshotMeterClass, Table2089SnapshotData.CurrentPhaseC);
					SnapshotData.CurrentAnglePhaseA = (double)Table2089SnapshotData.CurrentAnglePhaseA / 10.0;
					SnapshotData.CurrentAnglePhaseB = (double)Table2089SnapshotData.CurrentAnglePhaseB / 10.0;
					SnapshotData.CurrentAnglePhaseC = (double)Table2089SnapshotData.CurrentAnglePhaseC / 10.0;
					SnapshotData.Diag1Counter = (int)Table2089SnapshotData.Diag1Counter;
					SnapshotData.Diag2Counter = (int)Table2089SnapshotData.Diag2Counter;
					SnapshotData.Diag3Counter = (int)Table2089SnapshotData.Diag3Counter;
					SnapshotData.Diag4Counter = (int)Table2089SnapshotData.Diag4Counter;
					SnapshotData.Diag5ACounter = (int)Table2089SnapshotData.Diag5ACounter;
					SnapshotData.Diag5BCounter = (int)Table2089SnapshotData.Diag5BCounter;
					SnapshotData.Diag5CCounter = (int)Table2089SnapshotData.Diag5CCounter;
					SnapshotData.Diag5TCounter = (int)Table2089SnapshotData.Diag5TCounter;
					SnapshotData.Diag6Counter = (int)Table2089SnapshotData.Diag6Counter;
					SnapshotData.PowerOutageCount = (int)Table2089SnapshotData.PowerOutageCount;

					if (bPQSupported)
					{
						SnapshotData.PQSupported = true;
						SnapshotData.VQSagCounter = (int)Table2089SnapshotData.VQSagCounter;
						SnapshotData.VQSwellCounter = (int)Table2089SnapshotData.VQSwellCounter;
						SnapshotData.VQInterruptionCounter = (int)Table2089SnapshotData.VQInterruptionCounter;
						SnapshotData.VQImbalanceVCounter = (int)Table2089SnapshotData.VQImbalanceVCounter;
						SnapshotData.VQImbalanceICounter = (int)Table2089SnapshotData.VQImbalanceICounter;
						SnapshotData.THDPhaseA = (double)Table2089SnapshotData.THDPhaseA;
						SnapshotData.THDPhaseB = (double)Table2089SnapshotData.THDPhaseB;
						SnapshotData.THDPhaseC = (double)Table2089SnapshotData.THDPhaseC;
						SnapshotData.TDDPhaseA = (double)Table2089SnapshotData.TDDPhaseA;
						SnapshotData.TDDPhaseB = (double)Table2089SnapshotData.TDDPhaseB;
						SnapshotData.TDDPhaseC = (double)Table2089SnapshotData.TDDPhaseC;
					}
					else
					{
						SnapshotData.PQSupported = false;
					}

					//Add the snapshot to the list
					SnapshotList.Add(SnapshotData);
				}

				return SnapshotList;
			}
		}
		
        #endregion

		#region Private Methods

		/// <summary>
		/// Returns a string representing the service type
		/// </summary>
		/// <param name="byService"></param>
		/// <returns>Service Type</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/20/07 mrj 8.00.13		Created
		//  
		private string ServiceType(byte byService)
		{
			string strServiceType;

			//Create the service type string
			switch (byService)
			{
                case 0:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_3ELEM_3PHASE_4WIRE_WYE");
                        break;
                    }
                case 1:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_25ELEM_3PHASE_4WIRE_WYE");
                        break;
                    }
                case 2:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_2ELEM_NETWORK");
                        break;
                    }
                case 3:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_3ELEM_3PHASE_4WIRE_DELTA");
                        break;
                    }
                case 4:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_2ELEM_3PHASE_4WIRE_WYE");
                        break;
                    }
                case 5:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_2ELEM_3PHASE_3WIRE_DELTA");
                        break;
                    }
                case 6:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_2ELEM_3PHASE_4WIRE_DELTA");
                        break;
                    }
                case 7:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_2ELEM_SINGLE_PHASE");
                        break;
                    }
                case 8:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_1ELEM_SINGLE_PHASE");
                        break;
                    }
                case 9:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_1ELEM_SINGLE_PHASE_2_WIRE");
                        break;
                    }
                case 10:
                    {
                        strServiceType = m_rmStrings.GetString("SERV_2_5_ELEMENT_4_WIRE_WYE_9S");
                        break;
                    }
                case 255:
                default:
                    {
                        strServiceType = m_rmStrings.GetString("UNKNOWN");
                        break;
                    }
            }

			return strServiceType;
		}

		/// <summary>
		/// Converts the current value based on the meter class
		/// </summary>
		/// <param name="byMeterClass"></param>
		/// <param name="sCurrent"></param>
		/// <returns>The new current value</returns>
		/// <remarks>
		/// The units of the instantaneous currents depend on the Meter Class as follows:
		///	Class 10	0.001 Amps
		///	Class 20	0.002 Amps
		///	Class 100	0.008 Amps
		///	Class 150	0.016 Amps
		///	Class 200	0.016 Amps
		///	Class 320	0.032 Amps
		/// 
		/// Note: Class 2 and Class 480 will not be supported (as stated by marketing).
		/// </remarks>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/20/07 mrj 8.00.13		Created
		//	03/15/07 mrj 8.00.19 2552	Added missing classes
		//  
		private double ConvertCurrent(MeterClass byMeterClass, ushort sCurrent)
		{
			double dConvertedValue = 0.0;
			double dDivisor = 1.0;

			switch ((MeterClass)byMeterClass)
			{
				case MeterClass.CLASS_2:
				case MeterClass.CLASS_10:
				{
					dDivisor = 1000.0;
					break;
				}
				case MeterClass.CLASS_20:
				{
					dDivisor = 500.0;
					break;
				}
				case MeterClass.CLASS_100:
				{
					dDivisor = 125.0;
					break;
				}
				case MeterClass.CLASS_150:
				case MeterClass.CLASS_200:
				{
					dDivisor = 62.5;
					break;
				}				
				case MeterClass.CLASS_320:
				case MeterClass.CLASS_480:
				{
					dDivisor = 31.25;
					break;
				}
			}

			dConvertedValue = (double)sCurrent / dDivisor;

			return dConvertedValue;
		}

		#endregion
    }
}