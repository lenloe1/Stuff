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
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// CANSIDevice class - This is the "device server" for the ANSI device.
    /// (ICustomSchedule implementation)
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ -------------------------------------------
    /// 07/13/05 mrj 7.13.00 N/A    Created
    ///
    public partial class SENTINEL : ANSIMeter, ICustomSchedule
    {
        /// <summary>
        /// Implements the ICustomSchedule interface.  Reconfigures the Custom Schedule
        /// based on the provided schedule.
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="sScheduleName"></param>
        /// <param name="bWriteUserData2"></param>
        /// <returns>CSReconfigResult</returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 05/31/06 mrj 7.30.00 N/A    Created
        ///
        CSReconfigResult ICustomSchedule.Reconfigure(string sPathName, string sScheduleName, bool bWriteUserData2)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            CSReconfigResult Result = CSReconfigResult.ERROR;
            byte[] ProcParam;
            byte[] ProcResponse;
            PSEMResponse PSEMResult = PSEMResponse.Ok;

            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting Custom Schedule Reconfigure");

                
                //Check to make sure the meter is configured correctly before performing
                //the reconfigure
                if (((ICustomSchedule)this).CustomScheduleConfigured)
                {
                    if (!((ICustomSchedule)this).MultipleCustomScheduleConfigured)
                    {
                        //Schedule is valid write the custom schedule dates to the 
                        //billing schedule (custom schedule) config block of table 2048
                        Result = WriteCustomSchedule(sPathName, sScheduleName);

                        if (CSReconfigResult.SUCCESS == Result ||
                            CSReconfigResult.SUCCESS_SCHEDULE_TRUNCATED == Result)
                        {
                            //Set the schedule name to user data 2 (Sentinel only)
                            if (bWriteUserData2 &&
                                !Table2048.BillingSchedConfig.ScheduleNameSupported)
                            {
                                //Write schedule name to user data 2 if the System Manager option
                                //is set and the meter is a SENTINEL (schedule name is not supported
                                //in CS config block)
                                Table2048.ConstantsConfig.UserData2 = sScheduleName;
                            }


                            //Send the MFG procedure to open the config file (2048)
                            ProcParam = new byte[0];
                            ProcResult = ExecuteProcedure(Procedures.OPEN_CONFIG_FILE, ProcParam, out ProcResponse);

                            if (ProcedureResultCodes.NO_AUTHORIZATION == ProcResult)
                            {
                                //Return ISC
                                PSEMResult = PSEMResponse.Isc;
                            }
                            else if (ProcedureResultCodes.COMPLETED != ProcResult)
                            {
                                //We had a problem with this procedure
                                PSEMResult = PSEMResponse.Err;
                            }


                            //NOTE: 2048 must be written in sequential order

                            //Write the User Data 2, custom schedule name, to the meter
                            if (bWriteUserData2 &&
                                !Table2048.BillingSchedConfig.ScheduleNameSupported &&
                                PSEMResponse.Ok == PSEMResult)
                            {
                                //Write schedule name to user data 2 if the System Manager option
                                //is set and the meter is a SENTINEL
                                PSEMResult = Table2048.ConstantsConfig.Write();
                            }


                            //Write the billing schedule (custom schedule) config block to the 
                            //meter
                            if (PSEMResponse.Ok == PSEMResult)
                            {
                                PSEMResult = Table2048.BillingSchedConfig.Write();
                            }


                            //Send the MFG procedure to close the config file (2048), which tell the meter to
                            //do the reconfigure
                            if (PSEMResponse.Ok == PSEMResult)
                            {
                                ProcParam = new byte[4];
                                ProcParam.Initialize();							//No reset bits needed

                                ProcResult = ExecuteProcedure(Procedures.CLOSE_CONFIG_FILE, ProcParam, out ProcResponse);

                                if (ProcedureResultCodes.INVALID_PARAM == ProcResult)
                                {
                                    //The procedure finished but there was an invalid parameter
                                    //error.  Most likely because the user is at level 3
                                    if (4 == ProcResponse.Length)
                                    {
                                        //Read the result data
                                        MemoryStream TempStream = new MemoryStream(ProcResponse);
                                        BinaryReader TempBReader = new BinaryReader(TempStream);
                                        uint uiResultData = TempBReader.ReadUInt32();

                                        if (CLOSE_CONFIG_CONST_ERROR_MASK == (uint)(uiResultData & CLOSE_CONFIG_CONST_ERROR_MASK) ||
                                            CLOSE_CONFIG_BILL_ERROR_MASK == (uint)(uiResultData & CLOSE_CONFIG_BILL_ERROR_MASK))
                                        {
                                            //The user is level 3, which does not support reconfiguring
                                            //the custom schedule, return security error																						
                                            Result = CSReconfigResult.SECURITY_ERROR;
                                        }
                                        else
                                        {
                                            //The reconfigure failed
                                            Result = CSReconfigResult.ERROR;
                                        }
                                    }
                                    else
                                    {
                                        //The reconfigure failed
                                        Result = CSReconfigResult.ERROR;
                                    }
                                }
                                else if (ProcedureResultCodes.COMPLETED != ProcResult)
                                {
                                    //The reconfigure failed
                                    Result = CSReconfigResult.ERROR;
                                }

                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reconfig Custom Schedule Result = " + ProcResult);
                            }
                            else if (PSEMResponse.Isc == PSEMResult)
                            {
                                //Return security error
                                Result = CSReconfigResult.SECURITY_ERROR;
                            }
                            else
                            {
                                //The reconfigure failed
                                Result = CSReconfigResult.ERROR;
                            }
                        }
                    }
                    else
                    {
                        //Multiple Custom Schedules is configured so return an error.  This 
                        //should never happen since HH-Pro will not call this method when
                        //MCS is enabled.
                        Result = CSReconfigResult.ERROR_MCS_ENALBED;
                    }
                }
                else
                {
                    //This should never happen since HH-Pro will not call unless configured
                    Result = CSReconfigResult.ERROR;
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }            

            return Result;
        }

        /// <summary>
        /// Implements the ICustomSchedule interface.  Returns true if custom schedule
        /// is configured, false if not.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 05/31/06 mrj 7.30.00 N/A    Created
        ///
        bool ICustomSchedule.CustomScheduleConfigured
        {
            get
            {
                bool bSupported = false;

                //Check to see if custom schedule is supported in demand reset snapshots
                if (Table2048.DemandConfig != null)
                {
                    bSupported = Table2048.DemandConfig.CustomSchedSupported;
                }

                if (false == bSupported)
                {
                    //Check to see if custom schedule is supported in self read config
                    if (Table2048.SelfReadConfig != null)
                    {
                        bSupported = Table2048.SelfReadConfig.CustomSchedSupported;
                    }
                }

                return bSupported;
            }
        }

        /// <summary>
        /// Implements the ICustomSchedule interface.  Returns true if multiple
        /// custom schedules are configured, false if not.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 05/31/06 mrj 7.30.00 N/A    Created
        ///
        bool ICustomSchedule.MultipleCustomScheduleConfigured
        {
            get
            {
                bool bMCSEnabled = false;

                //Check to see if the Multiple Custom Schedules MeterKey bit is set
                if (MeterKeyTable.MultipleCustomScheduleEnabled)
                {
                    //Check to see if Multiple Custom Schedules are configured in 2084					
                    bMCSEnabled = Table2084.MultipleCustomScheduleConfigured;
                }


                return bMCSEnabled;
            }
        }
    }
}