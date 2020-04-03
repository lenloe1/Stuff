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
//                          Copyright © 2013 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Threading;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Result of configuring the cellular comm module.
    /// </summary>
    public enum CellularConfiguationResult
    {
        /// <summary>
        /// Success
        /// </summary>
        Success,
        /// <summary>
        /// Security Error
        /// </summary>
        SecurityError,
        /// <summary>
        /// Write Error
        /// </summary>
        WriteError,
        /// <summary>
        /// Commit Error
        /// </summary>
        CommitError,
        /// <summary>
        /// Unspecified Error
        /// </summary>
        UnspecifiedError,
    }
    
    /// <summary>
    /// Comm Module object for an ICS Comm Module
    /// </summary>
    public class ICSCommModule : CommModuleBase
    {
        #region Constants 

        private DateTime DEFAULT_DATE = new DateTime(1970, 1, 1, 0, 0, 0);
        private const int MAX_ITP_RETRIES = 5;
        private const ushort BLOCK_SIZE = 1024;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="amiDevice"></param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08           Created
        //  06/21/13 AF  2.80.40 TR 7640   Added initialization of comm module event tables and event dictionary
        //  08/28/13 jrf 2.85.30 WR 420526 Refactored initialization of tables.
        //  11/14/13 AF  3.50.03	        Class re-architecture - replace CENTRON_AMI parameter 
        //                                  with CANSIDevice
        //
        public ICSCommModule(CPSEM psem, CANSIDevice amiDevice)
            : base(psem, amiDevice)
        {
            m_AMIDevice = amiDevice;
            
            InitializeTables();

            m_EventDictionary = new ICS_Gateway_EventDictionary();
        }

        /// <summary>
        /// This method sets all comm module tables to null so they will have to be 
        /// reread the next time they are accessed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/13 jrf 2.85.30 WR 420526 Created.
        //
        public void Refresh()
        {
            InitializeTables();
        }

        /// <summary>
        /// Checks comm module's standard table 00 to see if the specified table is supported.
        /// </summary>
        /// <param name="usTableId">Identifier of the table we want to know about.</param>
        /// <returns>True if the table is listed in table 00; false, otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/13 jrf 2.85.30 WR 420526 Created.
        //
        public bool IsTableUsed(ushort usTableId)
        {
            bool tableUsed = false;

            if (Table00.IsTableUsed(usTableId))
            {
                tableUsed = true;
            }

            return tableUsed;
        }

        /// <summary>
        /// This method causes the ICS module to update the ERT data and statistics tables
        /// for subsequent read. 
        /// </summary>
        /// <returns>The result of the procedure.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/02/13 jkw 2.80.16	       Created
        // 09/09/13 jrf 2.85.39 WR 422369 Rereading mfg. tables 2508 and 2511 
        //                             when procedure to update them is successful.
        // 07/21/15 AF  4.50.169 WR 595583 Rereading tables 2508 and 2511 was throwing
        //                             exceptions in our automated tests, so just mark them
        //                             as dirty so that they will be reread the next time we need them.
        //
        public ProcedureResultCodes UpdateERTDataTables(out byte[] ProcResponse)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)ICSProcedure.UPDATE_ERT_DATA_TABLES;
            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.ICS_MODULE_PROCEDURE,
                ProcParam, out ProcResponse);

            if (ProcedureResultCodes.COMPLETED == ProcResult)
            {
                //Make sure ERT Data tables get reread since they have just been updated.
                if (null != Table2508)
                {
                    Table2508.State = AnsiTable.TableState.Dirty;
                }
                if (null != Table2511)
                {
                    Table2511.State = AnsiTable.TableState.Dirty;
                }
            }

            return ProcResult;
        }

        /// <summary>
        /// This method causes the ICS module to update the ERT data and statistics tables
        /// for subsequent read. 
        /// </summary>
        /// <param name="byDataRecordsCount">The count of ERT data records.</param>
        /// <param name="byStatRecordsCount">The count of ERT statistics records.</param>
        /// <param name="byDataRecordSize">The size of a data record.</param>
        /// <returns>The result of the procedure.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------
        //  07/31/13 jrf 2.85.06 TC 15058  Created
        //  09/03/13 jrf 2.85.34 WR 418110 Fixed error reading response record.
        // 09/09/13 jrf 2.85.39 WR 422369 Rereading mfg. tables 2508 and 2511 
        //                             when procedure to update them is successful.
        // 09/10/13 jrf 2.85.40 WR 422369 Removing call to reread mfg. tables 2508 and 2511, 
        //                                call to UpdateERTDataTables(...) already does this.  
        // 09/19/13 jrf 3.00.01 WR 422369 Corrected error reading response record.
        //
        public ProcedureResultCodes UpdateERTDataTables(out byte byDataRecordsCount, out byte byStatRecordsCount, out byte byDataRecordSize)
        {            
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = UpdateERTDataTables(out ProcResponse);

            byDataRecordsCount = 0;
            byStatRecordsCount = 0;
            byDataRecordSize = 0;

            //Interpreting the response
            //byte 0 of the ProcResponse is the function code
            if (null != ProcResponse)
            {
                if (ProcResponse.Length >= 2)
                {
                    byDataRecordsCount = ProcResponse[1];
                }

                if (ProcResponse.Length >= 3)
                {
                    byStatRecordsCount = ProcResponse[2];
                }

                if (ProcResponse.Length >= 4)
                {
                    byDataRecordSize = ProcResponse[3];
                }
            }

            return ProcResult;
        }

        /// <summary>
        /// This method sends a CLI (Diagnostic) command to the ICS comm module
        /// </summary>
        /// <param name="Command">the command that was entered in the UI</param>
        /// <param name="CommandResponse">the response we will receive from the meter</param>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/10/13 AF  2.80.18 TREQ7713, 7714, 7715  Created
        //  04/24/13 AF  2.80.22 TREQ7713, etc         Removed the padding for the parameters. They don't have to be
        //                                             255 bytes long.  Also corrected the code for the response - the
        //                                             length is not included
        //
        public ProcedureResultCodes SendDiagnosticsCommand(byte[] Command, out byte[] CommandResponse)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;

            ProcParam = new byte[Command.Length + 2];
            ProcParam[0] = (byte)ICSProcedure.EXECUTE_CLI_COMMAND;
            ProcParam[1] = (byte)Command.Length;
            Array.Copy(Command, 0, ProcParam, 2, Command.Length);

            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.ICS_MODULE_PROCEDURE, ProcParam, out CommandResponse);

            return ProcResult;
        }

        /// <summary>
        /// Clears the ICS comm module event log (table 2524).  Only available on the OW Centron
        /// </summary>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#     Description
        //  -------- --- ------- ---------  -------------------------------------------
        //  07/25/13 AF  2.85.02 WR 416508  Created
        //
        public ProcedureResultCodes ClearICSEventLog()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)ICSProcedure.RESET_EVENT_LOG;

            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.ICS_MODULE_PROCEDURE, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Creates a filter for the ICS events.
        /// </summary>
        /// <param name="filterSelection">Optional parameter used to filter the results. Defaults to Customer
        /// Filter.</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/07/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876  Created.
        //  
        public ProcedureResultCodes FilterICSEvents(ICSFilterSelection filterSelection = ICSFilterSelection.CUSTOMER_FILTER)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[2];
            ProcParam[0] = (byte)ICSProcedure.ICM_SELECT_EVENT_FILTER;
            ProcParam[1] = (byte)filterSelection;

            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.ICS_MODULE_PROCEDURE, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This method freezes the ICS comm module event tables
        /// </summary>
        /// <param name="startTime">the start of the time range for which we want events</param>
        /// <param name="endTime">the end of the time range for which we want events</param>
        /// <param name="CommandResponse">the response we will receive - should be the length of the data</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/14/13 AF  2.80.40 TR7586-7588   Created
        //  07/01/13 DLG 2.80.45 TC15467 Added a retry for when the result returns with the value of
        //                               NOT_FULLY_COMPLETED.
        //  07/17/13 DLG 2.80.54 TC15657 Removed the retry that was previously added because it should have been
        //                               added in ANSIDevice.cs
        //  08/23/13 AF  2.85.26 WR419631 Make sure that table 2521 is reread after executing the freeze procedure.
        //                                Otherwise, the event count will be off.
        //
        public ProcedureResultCodes UpdateEventTables(DateTime startTime, DateTime endTime, out byte[] CommandResponse)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            MemoryStream ProcParam = null;
            PSEMBinaryWriter ParamWriter = null;

            try
            {
                ProcParam = new MemoryStream(6);
                ParamWriter = new PSEMBinaryWriter(ProcParam);

                ParamWriter.Write((byte)ICSProcedure.UPDATE_EVENT_TABLES);
                ParamWriter.WriteSTIME(startTime, PSEMBinaryReader.TM_FORMAT.UINT32_TIME);
                ParamWriter.WriteSTIME(endTime, PSEMBinaryReader.TM_FORMAT.UINT32_TIME);

                ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.ICS_MODULE_PROCEDURE, ProcParam.ToArray(), out CommandResponse);

                Table2521.Refresh();
            }
            finally
            {
                if (null != ProcParam)
                {
                    ProcParam.Dispose();
                }
            }

            return ProcResult;
        }

        /// <summary>
        /// Creates a list of ERT statisitics records from the meter.
        /// </summary>
        /// <returns>The list.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.12        Created
        //  08/30/13 jrf 2.85.34 WR 418110 Updated variable name.
        //  09/19/13 jrf 2.90.01 WR 422369 Read of Table 2510 added to ensure we have 
        //                                 accurate count of statistics records
        //
        public virtual List<ERTStatisticsRecord> GetICSERTStatisticsRecords()
        {
            List<ERTStatisticsRecord> statsRecords = new List<ERTStatisticsRecord>();

            if (Table2510 != null)
            {
                //Make sure we reread this table.
                Table2510.Read();

                if (Table2510.NumberOfStatisticsRecords > 0)
                {
                    statsRecords = Table2511.ERTStatisiticsRecords.ToList();
                }
            }

            return statsRecords;
        }        

        /// <summary>
        /// Creates a list of ERT statisitics records from the meter.
        /// </summary>
        /// <returns>The list.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.12        Created
        //  09/19/13 jrf 2.90.01 WR 422369 Read of Table 2510 added to ensure we have 
        //                                 accurate count of statistics records
        //
        public virtual List<ERTConsumptionDataRecord> GetICSERTConsumptionDataRecords()
        {
            List<ERTConsumptionDataRecord> consumptionDataRecords = new List<ERTConsumptionDataRecord>();

            if (Table2510 != null)
            {
                //Make sure we reread this table.
                Table2510.Read();

                if (Table2510.NumberOfDataRecords > 0)
                {
                    consumptionDataRecords = Table2508.ERTConsumptionDataRecords.ToList();
                }
            }

            return consumptionDataRecords;
        }

        /// <summary>
        /// This method updates the cellular gateway address and port on the cellular comm module.
        /// </summary>
        /// <param name="Address">The IP address of the Cellular Gateway.</param>
        /// <param name="usPort">The port of the Cellular Gateway.</param>
        /// <returns>The result of configuring the cellular module.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/13 jrf 2.80.25 TQ8284 Created
        //  05/17/13 jrf 2.80.32 TQ8284 Modified to have individual try/catches around the write
        //                              and around the commit.
        //  06/14/13 jrf 2.80.38 TQ???? Refactored common code to be used to commit the configuration changes.
        //
        public CellularConfiguationResult UpdateCellularGateway(IPAddress Address, ushort usPort)
        {
            PSEMResponse WriteResult = PSEMResponse.Err;            
            
            if (Table2512 != null)
            {
                try
                {
                    //Set the value
                    Table2512.GatewayAddress = new DestinationAddressRecord(Address, usPort);

                    //Write value to module
                    WriteResult = Table2512.Write(ICMMfgTable2512ModuleConfiguration.ConfigFields.GatewayAddress);
                }
                catch
                {
                    WriteResult = PSEMResponse.Err;
                }
            }            

            return ConfigureCellularData(WriteResult);
        }

        /// <summary>
        /// This method updates the ERT utility ID on the cellular comm module.
        /// </summary>
        /// <param name="usERTUtilityID">The ERT utility ID.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/13 jrf 2.80.38 TQ???? Created.
        //
        public CellularConfiguationResult UpdateERTUtilityID(ushort usERTUtilityID)
        {
            PSEMResponse WriteResult = PSEMResponse.Err;
            
            if (Table2509 != null)
            {
                try
                {
                    //Set the value
                    Table2509.UtilityID = usERTUtilityID;

                    //Write value to module
                    WriteResult = Table2509.Write(ICMMfgTable2509ERTConfigurationTable.ConfigFields.ERTUtilityID);
                }
                catch
                {
                    WriteResult = PSEMResponse.Err;
                }
            }
            
            return ConfigureCellularData(WriteResult);
        }

        /// <summary>
        /// This method updates the cellular NTP address on the cellular comm module.
        /// </summary>
        /// <param name="NTPAddress">The NTP address to configure.</param>
        /// <returns>The result of configuring the cellular module.</returns>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  05/01/13 jrf 2.80.25 WR 419581 Created.
        //
        public CellularConfiguationResult UpdateNTPAddress(DestinationAddressRecord NTPAddress)
        {
            PSEMResponse WriteResult = PSEMResponse.Err;

            if (Table2512 != null)
            {
                try
                {
                    //Set the value
                    Table2512.NTPAddress = NTPAddress;

                    //Write value to module
                    WriteResult = Table2512.Write(ICMMfgTable2512ModuleConfiguration.ConfigFields.NTPAddress);
                    Logger.TheInstance.WriteLine(Logger.LoggingLevel.Detailed, "Table 2512 Write Response :" + WriteResult.ToDescription());
                }
                catch(Exception e)
                {
                    WriteResult = PSEMResponse.Err;
                    Logger.TheInstance.WriteLine(Logger.LoggingLevel.Detailed, "Table 2512 - Exception!\n" + e.ToString());
                }
            }
            else
            {
                Logger.TheInstance.WriteLine(Logger.LoggingLevel.Detailed, "Table 2512 is null!");
            }

            return ConfigureCellularData(WriteResult);
        }

        /// <summary>
        /// This method updates the ERT radio field on the cellular comm module.
        /// </summary>
        /// <param name="blnEnabled">Whether or not to enable the ERT Radio.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/30/13 jrf 3.00.10 TC15683 Created.
        //
        public CellularConfiguationResult UpdateERTRadio(bool blnEnabled)
        {
            PSEMResponse WriteResult = PSEMResponse.Err;

            if (Table2509 != null)
            {
                try
                {
                    //Set the value
                    Table2509.ERTRadioEnabled = blnEnabled;

                    //Write value to module
                    WriteResult = Table2509.Write(ICMMfgTable2509ERTConfigurationTable.ConfigFields.ERTRadio);
                }
                catch
                {
                    WriteResult = PSEMResponse.Err;
                }
            }

            return ConfigureCellularData(WriteResult);
        }

        /// <summary>
        /// Method that commits the cellular data to flash.
        /// </summary>
        /// <param name="WriteResult">The result of writing the cellular data to the comm module.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/13 jrf 2.80.38 TQ???? Created.
        //
        private CellularConfiguationResult ConfigureCellularData(PSEMResponse WriteResult)
        {
            CellularConfiguationResult Result = CellularConfiguationResult.UnspecifiedError;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
                        
            if (PSEMResponse.Isc == WriteResult)
            {
                Result = CellularConfiguationResult.SecurityError;
            }
            else if (PSEMResponse.Ok != WriteResult)
            {
                Result = CellularConfiguationResult.WriteError;
            }
            else//(PSEMResponse.Ok == WriteResult)
            {
                try
                {
                    //Commit values written to the configuration
                    ProcResult = CommitCellularConfiguration();

                    if (ProcedureResultCodes.COMPLETED == ProcResult)
                    {
                        Result = CellularConfiguationResult.Success;
                    }
                    else if (ProcedureResultCodes.NO_AUTHORIZATION == ProcResult)
                    {
                        Result = CellularConfiguationResult.SecurityError;
                    }
                    else
                    {
                        Result = CellularConfiguationResult.CommitError;
                    }
                }
                catch
                {
                    Result = CellularConfiguationResult.CommitError;
                }
            }

            return Result;
        }

        /// <summary>
        /// This method determines if a given ICS status alarm is set.
        /// </summary>
        /// <param name="Alarm">The alarm to check.</param>
        /// <returns>Whether or not the alarm is set.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/13 jrf 2.80.20 TQ8278 Created
        //
        public bool IsICSStatusAlarmSet(ICSStatusAlarms Alarm)
        {
            bool blnResult = false;
            try
            {
                if (Table2512 != null)
                {
                    blnResult = Table2512.IsAlarmSet(Alarm);
                }
            }
            catch
            {
                blnResult = false;
            }

            return blnResult;
        }

        /// <summary>
        /// This method calls the procedure to commit cellular configuration values to flash.
        /// </summary>
        /// <returns>The result of the procedure.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/13 jrf 2.80.25 TQ8284 Created
        //
        public ProcedureResultCodes CommitCellularConfiguration()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)ICSProcedure.COMMIT_CONFIGURATION;
            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.ICS_MODULE_PROCEDURE,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Method that initiates the retrieval of a selected ICM debug log
        /// </summary>
        /// <param name="FilePath">null terminated ASCII string containing the path and file name of the file
        /// on the ICM's file system that we want to retrieve. It's this file's data that will be rendered into
        /// MFG table 497</param>
        /// <param name="FileTotalSize">total size in bytes of the file being requested. If this parameter is zero,
        /// then the file being requested either does not exist, or otherwise cannot be retrieved</param>
        /// <param name="RenderedDataSize">total size in bytes of the valid data renedered into MFG table 497.
        /// If the FileTotalSize parameter is less than MFG table 494 FileRetrievalSize, then only the first 
        /// FileRetrievalSize bytes are rendered into MFG table 497. We will need to call ContinueFileRetrieval 
        /// to get the next "block" of bytes from the file</param>
        /// <param name="RenderedDataMD5">MD5 of the data rendered into MFG table 497. Use this to validate the 
        /// integrity of the file data.</param>
        /// <returns>The result of the procedure.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/06/15 AF  4.20.02  WR 579281  Created for debug log retrieval
        //
        public ProcedureResultCodes InitiateFileRetrieval(string FilePath, out UInt32 FileTotalSize, out UInt16 RenderedDataSize, out byte[] RenderedDataMD5)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;
            
            ProcParam = new byte[2 + (byte)FilePath.Length];
            ProcParam[0] = (byte)ICSProcedure.INITIATE_FILE_RETRIEVAL;
            ProcParam[1] = (byte)FilePath.Length;
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] byStringData = utf8.GetBytes(FilePath);
            byStringData.CopyTo(ProcParam, 2);

            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.ICS_MODULE_PROCEDURE,
                ProcParam, out ProcResponse);

            if ((ProcResult == ProcedureResultCodes.COMPLETED) && (ProcResponse.Length > 0))
            {
                byte[] temp = new byte[4]; ;
                Array.Copy(ProcResponse, 1, temp, 0, 4);
                FileTotalSize = BitConverter.ToUInt32(temp, 0);
                temp = new byte[2];
                Array.Copy(ProcResponse, 5, temp, 0, 2);
                RenderedDataSize = BitConverter.ToUInt16(temp, 0);
                RenderedDataMD5 = new byte[16];
                Array.Copy(ProcResponse, 7, RenderedDataMD5, 0, 16);
            }
            else
            {
                FileTotalSize = 0;
                RenderedDataSize = 0;
                RenderedDataMD5 = null;
            }

            return ProcResult;
        }

        /// <summary>
        /// Method to continue the retrieval of an ICM debug log in the case that the log is too
        /// large to retrieve in one go
        /// </summary>
        /// <param name="OperationCode">determines the operation requested. 1 = "Render Next", 2 = "Re-Render Current"</param>
        /// <param name="RenderedDataSize">total size in bytes of the valid data rendered into MFG table 497</param>
        /// <param name="RenderedDataMD5">MD5 of the dta rendered into MFG table 497</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/06/15 AF  4.20.02  WR 579281  Created for debug log retrieval
        //
        public ProcedureResultCodes ContinueFileRetrieval(byte OperationCode, out UInt16 RenderedDataSize, out byte[] RenderedDataMD5)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[2];
            ProcParam[0] = (byte)ICSProcedure.CONTINUE_FILE_RETRIEVAL;
            ProcParam[1] = OperationCode;

            ProcResult = m_AMIDevice.ExecuteProcedure(Procedures.ICS_MODULE_PROCEDURE,
                ProcParam, out ProcResponse);

            if ((ProcResult == ProcedureResultCodes.COMPLETED) && (ProcResponse.Length > 0))
            {
                byte[] temp = new byte[2];
                Array.Copy(ProcResponse, 1, temp, 0, 2);
                RenderedDataSize = BitConverter.ToUInt16(temp, 0);
                RenderedDataMD5 = new byte[16];
                Array.Copy(ProcResponse, 3, RenderedDataMD5, 0, 16);
            }
            else
            {
                RenderedDataSize = 0;
                RenderedDataMD5 = null;
            }

            return ProcResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RenderedDataSize"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/20/15 AF  4.20.02  WR 579281  Created for debug log retrieval
        //
        public PSEMResponse ReadTable2545(UInt16 RenderedDataSize, out byte[] Data)
        {
            PSEMResponse Response = PSEMResponse.Ok;

            Data = new byte[RenderedDataSize];
            byte[] temp;
            int offset = 0;

            while (offset < RenderedDataSize && Response == PSEMResponse.Ok)
            {
                if (BLOCK_SIZE + offset < RenderedDataSize)
                {
                    temp = new byte[BLOCK_SIZE];
                    Response = m_PSEM.OffsetRead(2545, offset, BLOCK_SIZE, out temp);
                    Array.Copy(temp, 0, Data, offset, BLOCK_SIZE);
                    offset += BLOCK_SIZE;
                }
                else
                {
                    temp = new byte[RenderedDataSize - offset];
                    Response = m_PSEM.OffsetRead(2545, offset, (ushort)(RenderedDataSize - offset), out temp);
                    Array.Copy(temp, 0, Data, offset, RenderedDataSize - offset);
                    offset = RenderedDataSize;
                }
            }

            return Response;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the manufacturer from table 2518
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string CellularManufacturer
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.Manufacturer;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the model from table 2518
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string CellularModel
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.Model;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the hardware version from table 2518
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string CellularHardwareVersion
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.HardwareVersion;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the firmware version from table 2518
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string CellularFirmwareVersion
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.FirmwareVersion;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the International Mobile Station Equipment Identity (IMEI) 
        /// OR 
        /// electronic serial number (ESN)
        /// OR 
        /// mobile equipment identifier (MEID)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string IMEI_or_ESN_or_MEID
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.IMEI_or_ESN_or_MEID;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the subscriber identity module integrated circuit card identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string SIM_ICC_ID
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.SIMICCID;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the International Mobile Subscriber Identity (IMSI) for GSM 
        /// OR 
        /// Mobile identification number (MIN) for Code division multiple access CDMA 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08        Created
        //
        public string IMSI_or_MIN
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2518 != null)
                {
                    returnValue = Table2518.IMSIforGSMorMINforCDMA;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads signal strength
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public sbyte? SignalStrength
        {
            get
            {
                sbyte? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.SignalStrength;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads registration status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public RegistrationStatus? RegistrationStatus
        {
            get
            {
                RegistrationStatus? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.RegistrationStatus;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads Network Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string NetworkMode
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2519 != null)
                {
                    returnValue = Table2519.NetworkMode;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads Network Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/12/13 jkw 2.85.16 n/a    Created
        //
        public string Carrier
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2519 != null)
                {
                    returnValue = Table2519.Carrier;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Tower Identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt16? TowerIdentifier
        {
            get
            {
                UInt16? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.TowerIdentifier;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Sector Identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt16? SectorIdentifier
        {
            get
            {
                UInt16? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.SectorIdentifier;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Number of Cell Tower Changes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? NumberOfCellTowerChanges
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.NumberOfCellTowerChanges;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Link Connection State
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public LinkConnectionState? LinkConnectionState
        {
            get
            {
                LinkConnectionState? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.LinkConnectionState;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Network Connection Up Time (seconds)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? NetworkConnectionUptime
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.NetworkConnectionUptime;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the IP Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string IPAddress
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2519 != null)
                {
                    returnValue = Table2519.IPAddress;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Gateway Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string GatewayAddress
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2519 != null)
                {
                    returnValue = Table2519.GatewayAddress;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Cumulative KiloBytes Sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? CumulativeKBytesSent
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.CumulativeKBytesSent;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Cumulative KiloBytes Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? CumulativeKBytesReceived
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.CumulativeKBytesReceived;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Bytes Sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? BytesSent
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.BytesSent;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Bytes Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? BytesReceived
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.BytesReceived;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Packets Delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? PacketsSent
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.PacketsSent;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Packets Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? PacketsReceived
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.PacketsReceived;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Last Successful Tower Communication
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime LastSuccessfulTowerCommunication
        {
            get
            {
                DateTime returnValue = DateTime.MinValue;

                if (Table2519 != null)
                {
                    returnValue = Table2519.LastSuccessfulTowerCommunication;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Number Of Link Failures
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? NumberOfLinkFailures
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.NumberOfLinkFailures;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Number Of Link Failures
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public Int16? ModemTemperature
        {
            get
            {
                Int16? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.ModemTemperature;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Last Modem Shutdown For Temperature
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime LastModemShutdownForTemperature
        {
            get
            {
                DateTime returnValue = DateTime.MinValue;

                if (Table2519 != null)
                {
                    returnValue = Table2519.LastModemShutdownForTemperature;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Last Modem Power Up After Temperature Shutdown
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime LastModemPowerUpAfterTemperatureShutdown
        {
            get
            {
                DateTime returnValue = DateTime.MinValue;

                if (Table2519 != null)
                {
                    returnValue = Table2519.LastModemPowerUpAfterTemperatureShutdown;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the MDM Radio Phone Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public string MDNRadioPhoneNumber
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2519 != null)
                {
                    returnValue = Table2519.MDNRadioPhoneNumber;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Number of Sector Identifier Changes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //
        public UInt32? NumberOfSectorIdentifierChanges
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.NumberOfSectorIdentifierChanges;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Traffic Channels - good CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //
        public UInt32? TrafficChannelsGoodCRCCount
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.TrafficChannelsGoodCRCCount;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Traffic Channels - bad CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //
        public UInt32? TrafficChannelsBadCRCCount
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.TrafficChannelsBadCRCCount;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Control Channels - good CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //
        public UInt32? ControlChannelsGoodCRCCount
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.ControlChannelsGoodCRCCount;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Control Channels - bad CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //
        public UInt32? ControlChannelsBadCRCCount
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.ControlChannelsBadCRCCount;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the Figure of Merit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 jkw 2.80.35 n/a    Created
        //
        public byte? FigureOfMerit
        {
            get
            {
                byte? returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.FigureOfMerit;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reads the name of the cellular carrier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/13/13 jrf 2.85.44 WR423526 Created.
        //
        public string CellularCarrier
        {
            get
            {
                string returnValue = null;

                if (Table2519 != null)
                {
                    returnValue = Table2519.CellularCarrier;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Major part of the Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionMajor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMFirmwareVersionMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Uncached ICM Major part of the Firmware Version 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/14 jrf 3.50.68 WR458134 Created.
        public byte? UncachedICMFirmwareVersionMajor
        {
            get
            {
                byte? returnValue = null;

                if (Table2515 != null)
                {
                    returnValue = Table2515.UncachedICMFirmwareVersionMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Minor part of the Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionMinor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMFirmwareVersionMinor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Revision part of the Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionRevision
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMFirmwareVersionRevision;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Extended Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string ICMExtendedFirmwareVersion
        {
            get
            {
                string returnValue = string.Empty;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMExtendedFirmwareVersion;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Hardware Version Major
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMHardwareVersionMajor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.HardwareVersionMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Hardware Version Minor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMHardwareVersionMinor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.HardwareVersionMinor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Number of Super Capacitors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMNumberSuperCapacitors
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.NumberSuperCapacitors;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Module Major part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberMajor
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMSerialNumberMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Module Minor part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberMinor
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMSerialNumberMinor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM Module Build part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberBuild
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMSerialNumberBuild;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM CPU Identifier High 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMCPUIDHigh
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMCPUIDHigh;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// ICM CPU Identifier Low
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMCPUIDLow
        {
            get
            {
                UInt32 returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.ICMCPUIDLow;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Boot Loader Major part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMBootLoaderVersionMajor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.BootLoaderVersionMajor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Boot Loader Minor part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMBootLoaderVersionMinor
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.BootLoaderVersionMinor;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Boot Loader Revision part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMBootLoaderVersionRevision
        {
            get
            {
                byte returnValue = 0;

                if (Table2515 != null)
                {
                    returnValue = Table2515.BootLoaderVersionRevision;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Last Power Failure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime ICMLastPowerFailure
        {
            get
            {
                DateTime returnValue = DateTime.MinValue;

                if (Table2515 != null)
                {
                    returnValue = Table2515.LastPowerFailure;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Super Capacitor Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte? ICMSuperCapacitorStatus
        {
            get
            {
                byte? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.SuperCapacitorStatus;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Reboot Count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? ICMRebootCount
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.RebootCount;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Uptime
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32? ICMUptime
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.Uptime;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Module Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public ICMModuleStatus? ICMModuleStatus
        {
            get
            {
                ICMModuleStatus? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.ModuleStatus;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets an uncached read of the module status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/19/16 AF  4.50.224 WR 647295  Created for use by firmware tests
        //
        public ICMModuleStatus? ICMModuleStatusUncached
        {
            get
            {
                ICMModuleStatus? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.ModuleStatusUncached;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Module Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public Int16? ICMModuleTemperature
        {
            get
            {
                Int16? returnValue = null;

                if (Table2516 != null)
                {
                    returnValue = Table2516.ModuleTemperature;
                }

                return returnValue;
            }
        }


        /// <summary>
        /// Socket Idle Timeout (in seconds).
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public ushort? SocketIdleTimeout
        {
            get
            {
                ushort? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.SocketIdleTimeout;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gateway Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public string DisplayableGatewayAddress
        {
            get
            {
                string returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.DisplayableGatewayAddress;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// DNS Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public string DisplayableDNSAddress
        {
            get
            {
                string returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.DisplayableDNSAddress;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// NTP Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public string DisplayableNTPAddress
        {
            get
            {
                string returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.DisplayableNTPAddress;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// The power fail time or the minimum outage required before the ICS module 
        /// recognizes a power outage.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public UInt32? PowerFailTime
        {
            get
            {
                UInt32? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.PowerFailTime;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// The NTP update frequency (in hours) is how often the ICS module asks the 
        /// SNTP server for the time.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public byte? NTPUpdateFrequency
        {
            get
            {
                byte? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.NTPUpdateFrequency;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// The NTP valid time (in minutes) is how long the ICS time is valid after
        /// being recieved from the SNTP server.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public byte? NTPValidTime
        {
            get
            {
                byte? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.NTPValidTime;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Link Failure Threshold
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //
        public ushort? LinkFailureThreshold
        {
            get
            {
                ushort? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.LinkFailuresThreshold;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Tower Changes Threshold
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //
        public ushort? TowerChangesThreshold
        {
            get
            {
                ushort? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.TowerChangesThreshold;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Sector ID Changes Threshold
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //
        public ushort? SectorIDChangesThreshold
        {
            get
            {
                ushort? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.SectorIDChangesThreshold;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Link Failure Counter Reset Frequency
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.38 TQ8278 Created
        //
        public ushort? LinkFailureCounterResetFrequency
        {
            get
            {
                ushort? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.LinkFailuresCounterResetFrequency;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Tower Changes Counter Reset Frequency
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.38 TQ8278 Created
        //
        public ushort? TowerChangesCounterResetFrequency
        {
            get
            {
                ushort? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.TowerChangesCounterResetFrequency;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Sector ID Changes Counter Reset Frequency
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.38 TQ8278 Created
        //
        public ushort? SectorIDChangesCounterResetFrequency
        {
            get
            {
                ushort? returnValue = null;

                if (Table2512 != null)
                {
                    returnValue = Table2512.SectorIDChangesCounterResetFrequency;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Indicates wheter or not ERT tables are populated.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  07/31/13 jrf 2.85.06 TQ 8278   Created
        //
        public bool? IsERTPopulated
        {
            get
            {
                bool? returnValue = null;

                if (Table2512 != null)
                {
                    if (1 == Table2512.IsERTPopulated)
                    {
                        returnValue = true;
                    }
                    else if (0 == Table2512.IsERTPopulated)
                    {
                        returnValue = false;
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee is enabled by the ZigBeeAccess field in table 464
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/14 AF  3.50.31 WR888843 Created
        //
        public bool? ZigBeeAccess
        {
            get
            {
                bool? returnValue = null;

                if (Table2512 != null)
                {
                    if (0 == Table2512.ZigBeeAccess)
                    {
                        returnValue = true;
                    }
                    else if (1 == Table2512.ZigBeeAccess)
                    {
                        returnValue = false;
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Radio's phone number.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //  05/06/13 jrf 2.80.26 TQ8278 Unit Testing bug fix.
        public string RadioPhoneNumber
        {
            get
            {
                string strPhoneNumber = null;                

                if (null != Table2517 && null != Table2517.RadioPhoneNumber)
                {
                    byte[] abyPhoneNumber = Table2517.RadioPhoneNumber;
                    //Going to build up displayed phone number from right to left
                    Array.Reverse(abyPhoneNumber);

                    strPhoneNumber = "";

                    for (int i = 0; i < abyPhoneNumber.Length; i++)
                    {
                        //The following indicies (4, 7 & 10) of the phone number should be 
                        //displayed with a dash to the right of them so phone number will 
                        //look like 1-864-718-6620
                        if (4 == i || 7 == i || 10 == i)
                        {
                            strPhoneNumber = (char)abyPhoneNumber[i] + "-" + strPhoneNumber;
                        }
                        else
                        {
                            strPhoneNumber = (char)abyPhoneNumber[i] + strPhoneNumber;
                        }
                    }
                }

                return strPhoneNumber;
            }
        }

        /// <summary>
        /// GSM's Primary APN Name.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        public string GSMPrimaryAPNName
        {
            get
            {
                string strName = null;


                if (null != Table2517 && null != Table2517.GSMPrimaryAPN)
                {
                    strName = Table2517.GSMPrimaryAPN.APN;
                }

                return strName;
            }
        }

        /// <summary>
        /// GSM's Primary APN's User Name.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        public string GSMPrimaryAPNUserName
        {
            get
            {
                string strName = null;


                if (null != Table2517 && null != Table2517.GSMPrimaryAPN)
                {
                    strName = Table2517.GSMPrimaryAPN.UserName;
                }

                return strName;
            }
        }

        /// <summary>
        /// Determines if the GSM's Primary APN's password is configured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //  05/21/13 jrf 2.80.32 TQ8278 Corrected logic for all null password.
        //
        public bool? GSMPrimaryAPNPasswordConfigured
        {
            get
            {
                bool? blnConfigured = null;
                string strPassword = Table2517.GSMPrimaryAPN.Password.Replace("\0", "");


                if (null != Table2517 && null != Table2517.GSMPrimaryAPN)
                {
                    if (true == string.IsNullOrEmpty(strPassword))
                    {
                        blnConfigured = false;
                    }
                    else
                    {
                        blnConfigured = true;
                    }
                }

                return blnConfigured;
            }
        }

        /// <summary>
        /// GSM's Primary APN Name.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        public string GSMSecondaryAPNName
        {
            get
            {
                string strName = null;


                if (null != Table2517 && null != Table2517.GSMSecondaryAPN)
                {
                    strName = Table2517.GSMSecondaryAPN.APN;
                }

                return strName;
            }
        }

        /// <summary>
        /// GSM's Secondary APN's User Name.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        public string GSMSecondaryAPNUserName
        {
            get
            {
                string strName = null;


                if (null != Table2517 && null != Table2517.GSMSecondaryAPN)
                {
                    strName = Table2517.GSMSecondaryAPN.UserName;
                }

                return strName;
            }
        }

        /// <summary>
        /// Determines if the GSM's Secondary APN's password is configured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //  05/21/13 jrf 2.80.32 TQ8278 Corrected logic for all null password.
        //
        public bool? GSMSecondaryAPNPasswordConfigured
        {
            get
            {
                bool? blnConfigured = null;
                string strPassword = Table2517.GSMPrimaryAPN.Password.Replace("\0", "");

                if (null != Table2517 && null != Table2517.GSMSecondaryAPN)
                {
                    if (true == string.IsNullOrEmpty(strPassword))
                    {
                        blnConfigured = false;
                    }
                    else
                    {
                        blnConfigured = true;
                    }
                }

                return blnConfigured;
            }
        }

        /// <summary>
        /// The units the Cellular Data Timeout is stored in.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        public ICMMfgTable2517CellularConfiguration.TimeoutUnits? CellularDataTimeoutUnits
        {
            get
            {
                ICMMfgTable2517CellularConfiguration.TimeoutUnits? TimeoutUnits = null;


                if (null != Table2517)
                {
                    TimeoutUnits = Table2517.CellularDataTimeoutUnits;
                }

                return TimeoutUnits;
            }
        }

        /// <summary>
        /// The Cellular Data Timeout.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        public byte? CellularDataTimeout
        {
            get
            {
                byte? byTimeout = null;

                if (null != Table2517)
                {
                    byTimeout = Table2517.CellularDataTimeout;
                }

                return byTimeout;
            }
        }

        /// <summary>
        /// Comm module is operating in SMS only mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        public bool? SMSOnlyMode
        {
            get
            {
                bool? blnSMSOnly = null;

                if (null != Table2517)
                {
                    blnSMSOnly = Table2517.SMSOnlyMode;
                }

                return blnSMSOnly;
            }
        }

        /// <summary>
        /// The configuration of SMS.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        public ICMMfgTable2517CellularConfiguration.SMSConfiguration? SMSConfiguration
        {
            get
            {
                ICMMfgTable2517CellularConfiguration.SMSConfiguration? SMSConfig = null;


                if (null != Table2517)
                {
                    SMSConfig = Table2517.SMSOperation;
                }

                return SMSConfig;
            }
        }       

        /// <summary>
        /// The max number of ERT data records
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/13 jrf 2.80.37 TQ8278 Created
        //
        public byte? MaxERTRecords
        {
            get
            {
                byte? byMaxRecords = null;

                if (null != Table2509)
                {
                    byMaxRecords = Table2509.MaxRecords;
                }

                return byMaxRecords;
            }
        }

        /// <summary>
        /// ERT Utility ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/13 jrf 2.80.37 TQ8278 Created
        //
        public ushort? ERTUtilityID
        {
            get
            {
                ushort? usUtilityID = null;

                if (null != Table2509)
                {
                    usUtilityID = Table2509.UtilityID;
                }

                return usUtilityID;
            }
        }

        /// <summary>
        /// ERT Data Lifetime (in hours).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/13 jrf 2.80.37 TQ8278 Created
        //
        public byte? ERTDataLifetime
        {
            get
            {
                byte? byDataLifetime = null;

                if (null != Table2509)
                {
                    byDataLifetime = Table2509.DataLifetime;
                }

                return byDataLifetime;
            }
        }

        /// <summary>
        /// ERT Radio, used to turn the ERT module's transceiver on/off.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //
        public bool? ERTRadioEnabled
        {
            get
            {
                bool? blnEnabled = null;

                if (null != Table2509)
                {
                    blnEnabled = Table2509.ERTRadioEnabled;
                }

                return blnEnabled;
            }            
        }

        /// <summary>
        /// Resting Channel Interval (in minutes).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //
        public ushort? ERTRestingChannelInterval
        {
            get
            {
                ushort? usRestingChannelInterval = null;

                if (null != Table2509)
                {
                    usRestingChannelInterval = Table2509.RestingChannelInterval;
                }

                return usRestingChannelInterval;
            } 
        }

        /// <summary>
        /// Maximum managed meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ERTMaxManagedMeters
        {
            get
            {
                byte? byMaxManagedMeters = null;

                if (null != Table2509)
                {
                    byMaxManagedMeters = Table2509.MaxManagedMeters;
                }

                return byMaxManagedMeters;
            }
        }

        /// <summary>
        /// Maximum unmanaged meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ERTMaxUnmanagedMeters
        {
            get
            {
                byte? byMaxUnmanagedMeters = null;

                if (null != Table2509)
                {
                    byMaxUnmanagedMeters = Table2509.MaxUnmanagedMeters;
                }

                return byMaxUnmanagedMeters;
            }
        }

        /// <summary>
        /// Maximum managed threshold attempts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ERTMaxManagedThresholdAttempts
        {
            get
            {
                byte? byMaxManagedThresholdAttempts = null;

                if (null != Table2509)
                {
                    byMaxManagedThresholdAttempts = Table2509.MaxManagedThresholdAttempts;
                }

                return byMaxManagedThresholdAttempts;
            }
        }

        /// <summary>
        /// Threshold RSSI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public sbyte? ERTThresholdRSSI
        {
            get
            {
                sbyte? sbyThresholdRSSI = null;

                if (null != Table2509)
                {
                    sbyThresholdRSSI = Table2509.ThresholdRSSI;
                }

                return sbyThresholdRSSI;
            }
        }
        
        /// <summary>
        /// RSSI samples
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ERTRSSISamples
        {
            get
            {
                byte? byRSSISamples = null;

                if (null != Table2509)
                {
                    byRSSISamples = Table2509.RSSISamples;
                }

                return byRSSISamples;
            }
        }

        /// <summary>
        /// Steal threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ERTStealThreshold
        {
            get
            {
                byte? byStealThreshold = null;

                if (null != Table2509)
                {
                    byStealThreshold = Table2509.StealThreshold;
                }

                return byStealThreshold;
            }
        }

        /// <summary>
        /// 100G meter support
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public bool? ERT100GMeterSupport
        {
            get
            {
                bool? bln100GSupport = null;

                if (null != Table2509)
                {
                    bln100GSupport = Table2509.Support100GMeter;
                }

                return bln100GSupport;
            }
        }

        /// <summary>
        /// 100W meter support
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public bool? ERT100WMeterSupport
        {
            get
            {
                bool? bln100WSupport = null;

                if (null != Table2509)
                {
                    bln100WSupport = Table2509.Support100WMeter;
                }

                return bln100WSupport;
            }
        }

        /// <summary>
        /// 100W Plus meter support
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public bool? ERT100WPlusMeterSupport
        {
            get
            {
                bool? bln100WPlusSupport = null;

                if (null != Table2509)
                {
                    bln100WPlusSupport = Table2509.Support100WPlusMeter;
                }

                return bln100WPlusSupport;
            }
        }

        /// <summary>
        /// Channel hop frequency multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ERTChannelHopFrequencyMultiplier
        {
            get
            {
                byte? byChannelHopFreqMultiplier = null;

                if (null != Table2509)
                {
                    byChannelHopFreqMultiplier = Table2509.ChannelHopFrequencyMultiplier;
                }

                return byChannelHopFreqMultiplier;
            }
        }

        /// <summary>
        /// Data store multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ERTDataStoreMultiplier
        {
            get
            {
                byte? byDataStoreMultiplier = null;

                if (null != Table2509)
                {
                    byDataStoreMultiplier = Table2509.DataStoreMultiplier;
                }

                return byDataStoreMultiplier;
            }
        }

        /// <summary>
        /// Conn down time. This value determines how long the ICM will wait in hours when there is 
        /// no data connectivity before releasing the ERTs it is managing.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/15/16 AF  4.50.236 WR 660484  Created
        //
        public byte? ERTConnDownTime
        {
            get
            {
                byte? byConnDownTime = null;

                if (null != Table2509)
                {
                    byConnDownTime = Table2509.ConnDownTime;
                }

                return byConnDownTime;
            }
        }

        /// <summary>
        /// Camping channel timer.   This value (in seconds) determines how long the ICM will wait
        /// on a “predicted” channel for a valid ERT packet to arrive at the radio.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ERTCampingChannelTimer
        {
            get
            {
                byte? byCampingChannelTimer = null;

                if (null != Table2509)
                {
                    byCampingChannelTimer = Table2509.CampingChannelTimer;
                }

                return byCampingChannelTimer;
            }
        }

        /// <summary>
        /// Determines if comm module has a MAC address. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public override bool HasMACAddress
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the Comm Module Events that were recorded from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  06/21/13 AF  2.80.40 TR7640 Replaced CommModuleHistoryEntry with HistoryEntry
        //
        public List<HistoryEntry> CommModuleEvents
        {
            get
            {
                return Table2524.CommModuleHistoryEventEntries;
            }
        }

        /// <summary>
        /// Retrieves the Comm Module history log configuration from the meter.  The list
        /// includes all possible supported events with a description and a boolean
        /// indicating whether or not the event is monitored in the meter.  This version
        /// reads the config from tables 2522 and 2523.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  06/24/13 AF  2.80.41 TR7640 Moved from the device class
        //
        public List<MFG2048EventItem> CommModuleEventConfigured
        {
            get
            {
                return Table2523.ICSHistoryLogEventList;
            }
        }

        /// <summary>
        /// Retrieves the Comm Module history log configuration from the meter.  The list
        /// includes all monitored events regardless of whether or not they are listed as supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/15 jrf 4.20.03 576493 Created
        //
        public List<MFG2048EventItem> CommModuleEventMonitored
        {
            get
            {
                return Table2523.ICSHistoryLogMonitoredEventList;
            }
        }

        /// <summary>	
        /// Returns the number of ICS event log entries from table 2521 (MFG 473).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  07/01/13 DLG 2.80.45 TC15657  Created.
        //   
        public uint? NumberOfICSEvents
        {
            get
            {
                uint? uiNumber = null;

                if (Table2521 != null)
                {
                    uiNumber = Table2521.NumberICSEntries;
                }

                return uiNumber;
            }
        }

        /// <summary>
        /// Gets the NXP (ERT radio) firmware version out of mfg table 491 in the
        /// form of a byte array.  The array has 3 elements: version, revision, and build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public byte[] NXPFirmwareVersion
        {
            get
            {
                byte[] Version = null;

                if (Table2539 != null)
                {
                    Version = Table2539.NXPFWVersion;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the NXP (ERT radio) firmware version out of mfg table 491 in the
        /// form of an x.x string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //  05/12/15 jrf 4.20.06  WR 584075  Switched to just show major and minor version. 
        //                                   Moved build to separate property.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string ERTRadioFirmwareVersion
        {
            get
            {
                string Version = "";

                if (NXPFirmwareVersion != null && 2 <= NXPFirmwareVersion.Length)
                {
                    Version = NXPFirmwareVersion[0].ToString() + "." 
                        + NXPFirmwareVersion[1].ToString("d3", CultureInfo.CurrentCulture);
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the NXP (ERT radio) firmware build number out of mfg table 491 in the
        /// form of a string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        //
        public string ERTRadioFirmwareBuild
        {
            get
            {
                string Build = "";

                if (NXPFirmwareVersion != null && 3 == NXPFirmwareVersion.Length)
                {
                    Build = NXPFirmwareVersion[2].ToString("d3", CultureInfo.CurrentCulture);
                }

                return Build;
            }
        }

        /// <summary>
        /// Gets the modem firmware version out of mfg table 491 in the
        /// form of an x.x string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string ModemFirmwareVersion
        {
            get
            {
                string Version = "";

                if (Table2539 != null && null != Table2539.ModemFWVersion && 2 <= Table2539.ModemFWVersion.Length)
                {
                    Version = Table2539.ModemFWVersion[0].ToString() + "."
                        + Table2539.ModemFWVersion[1].ToString("d3", CultureInfo.CurrentCulture);
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the modem firmware build number out of mfg table 491 in the
        /// form of a string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        public string ModemFirmwareBuild
        {
            get
            {
                string Build = "";

                if (Table2539 != null && null != Table2539.ModemFWVersion && 3 == Table2539.ModemFWVersion.Length)
                {
                    Build = Table2539.ModemFWVersion[2].ToString("d3", CultureInfo.CurrentCulture);
                }

                return Build;
            }
        }

        /// <summary>
        /// Gets the PIC firmware version out of mfg table 491 in the
        /// form of a x.x string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string PICFirmwareVersion
        {
            get
            {
                string Version = "";

                if (Table2539 != null && null != Table2539.PICFWVersion && 2 <= Table2539.PICFWVersion.Length)
                {
                    Version = Table2539.PICFWVersion[0].ToString() + "."
                        + Table2539.PICFWVersion[1].ToString("d3", CultureInfo.CurrentCulture);
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the PIC firmware version out of mfg table 491 in the
        /// form of an x.x.x string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        public string PICFirmwareBuild
        {
            get
            {
                string Build = "";

                if (Table2539 != null && null != Table2539.PICFWVersion && 3 == Table2539.PICFWVersion.Length)
                {
                    Build = Table2539.PICFWVersion[2].ToString("d3", CultureInfo.CurrentCulture);
                }

                return Build;
            }
        }

        /// <summary>
        /// Gets the ICM hardware version out of mfg table 491 in the
        /// form of an x.x string.
        /// </summary>
        public string ICMModuleHardwareVersion
        {
            get
            {
                string Version = "";

                if (Table2539 != null && null != Table2539.ICMModuleHWVersion && 2 <= Table2539.ICMModuleHWVersion.Length)
                {
                    Version = Table2539.ICMModuleHWVersion[0].ToString(CultureInfo.InvariantCulture) + "."
                        + Table2539.ICMModuleHWVersion[1].ToString("d3", CultureInfo.InvariantCulture);
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the list of microtype records from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        public MicrotypeRecord[] MicrotypeRecords
        {
            get
            {
                MicrotypeRecord[] Records = null;

                if (null != Table2539 && null != Table2539.MicrotypeRecords)
                {
                    Records = (MicrotypeRecord[])Table2539.MicrotypeRecords.Clone();
                }

                return Records;
            }
        }

        /// <summary>
        /// Returns whether the system logs in mfg table 496 are supported by this device.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  06/08/14 jrf 4.20.12  WR 589725  Created
        //
        public bool SupportsICMSystemLogs
        {
            get
            {
                bool SupportsSystemLogs = false;

                if (null != Table2544)
                {
                    SupportsSystemLogs = true;
                }

                return SupportsSystemLogs;
            }
        }

        /// <summary>
        /// Returns the number of system logs in mfg table 496
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/20/15 AF  4.20.02  WR 579281  Created
        //
        public UInt16? NumberOfICMSystemLogs
        {
            get
            {
                UInt16? uiNumber = null;

                if (Table2544 != null)
                {
                    uiNumber = Table2544.NumberOfEntries;
                }

                return uiNumber;
            }
        }

        /// <summary>
        /// List of the system logs available for download
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/20/15 AF  4.20.02  WR 579281  Created
        //
        public List<string> SystemLogEntries
        {
            get
            {
                List<string> lstEntries = new List<string>();

                if (Table2544 != null)
                {
                    lstEntries = Table2544.EntriesList;
                }

                return lstEntries;
            }
        }

        /// <summary>
        /// The comm module's network time.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version ID Number  Description
        //  -------- --- ------- -- ------  ---------------------------------------------
        //  07/18/13 jrf 2.80.54 WR 411418  Created.
        //  07/18/13 jrf 2.80.55 WR 411418  Returning null if default date is all that can be read.
        //  08/05/13 jrf 2.85.10 TC 12652   Retrieving network time from table that does not cause ICS module 
        //                                  to force a time sync.
        //
        public override DateTime? ITPTime
        {
            get
            {
                DateTime? dtNetworkTime = null;

                if (Table2751 != null)
                {
                    try
                    {
                        dtNetworkTime = Table2751.CurrentTime;
                    }
                    catch (PSEMException PSEMExp)
                    {
                        //A DNR (data not ready) resposne is expected if the module's validity
                        //period has expired.  Returning null to indicate this.
                        if (PSEMResponse.Dnr == PSEMExp.PSEMResponse)
                        {
                            dtNetworkTime = null;
                        }
                        else
                        {
                            throw PSEMExp;
                        }
                    }
                }

                if (null != dtNetworkTime)
                {
                    //Let's make it local!
                    if (m_AMIDevice.IsMeterInDST)
                    {
                        dtNetworkTime += m_AMIDevice.DSTAdjustAmount;
                    }

                    dtNetworkTime += m_AMIDevice.TimeZoneOffset;
                }

                return dtNetworkTime;
            }
        }

        /// <summary>
        /// Gets the count of ERT statistics records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/21/15 jrf 4.20.18 598314 Created
        public byte ERTStatisticsRecordsCount
        {
            get
            {
                byte Count = 0;

                if (Table2510 != null)
                {
                    //Make sure we reread this table.
                    Table2510.Read();

                    Count = Table2510.NumberOfStatisticsRecords;
                }

                return Count;
            }
        }

        /// <summary>
        /// Gets the count of ERT data records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/21/15 jrf 4.20.18 598314 Created
        public byte ERTDataRecordsCount
        {
            get
            {
                byte Count = 0;

                if (Table2510 != null)
                {
                    //Make sure we reread this table.
                    Table2510.Read();

                    Count = Table2510.NumberOfDataRecords;
                }

                return Count;
            }
        }

        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes all tables to null. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/13 jrf 2.85.30 WR 420526 Created.
        //  04/20/15 AF  4.20.02 WR 579281  Added tables 2542 and 2544 for handling system logs
        //
        private void InitializeTables()
        {
            m_Table0 = null;
            m_Table2509 = null;
            m_Table2510 = null;
            m_Table2511 = null;
            m_Table2512 = null;
            m_Table2515 = null;
            m_Table2516 = null;
            m_Table2517 = null;
            m_Table2518 = null;
            m_Table2519 = null;
            m_Table2521 = null;
            m_Table2522 = null;
            m_Table2523 = null;
            m_Table2524 = null;
            m_Table2525 = null;
            m_Table2542 = null;
            m_Table2544 = null;
            m_Table2751 = null;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Returns the Table00 Object; Creates it if it has not been created
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/06 KRC 8.00.00 N/A
        //
        private CTable00 Table00
        {
            get
            {
                if (null == m_Table0)
                {
                    m_Table0 = new CTable00(m_PSEM);
                }

                return m_Table0;
            }
        }

        /// <summary>
        /// Gets ICS ERT consumption data table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.12 n/a    Created
        //  08/30/13 jrf 2.85.34 WR 418110 Switched to pass in the correct number of data records.
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //
        private ICMMfgTable2508ERTData Table2508
        {
            get
            {
                if (null == m_Table2508 && Table00.IsTableUsed(2508) && Table2510 != null)
                {
                    m_Table2508 = new ICMMfgTable2508ERTData(m_PSEM, Table00.TimeFormat, Table2510);
                }

                return m_Table2508;
            }
        }

        /// <summary>
        /// Gets ICS ERT actual table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.12 n/a    Created
        //
        private ICMMfgTable2509ERTConfigurationTable Table2509
        {
            get
            {
                if (null == m_Table2509 && Table00.IsTableUsed(2509))
                {
                    m_Table2509 = new ICMMfgTable2509ERTConfigurationTable(m_PSEM);
                }

                return m_Table2509;
            }
        }

        /// <summary>
        /// Gets ICS ERT configuration table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.12 n/a    Created
        //
        private ICMMfgTable2510ERTActual Table2510
        {
            get
            {
                if (null == m_Table2510 && Table00.IsTableUsed(2510))
                {
                    m_Table2510 = new ICMMfgTable2510ERTActual(m_PSEM);
                }

                return m_Table2510;
            }
        }

        /// <summary>
        /// Gets ICS ERT statistics table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.12 n/a    Created
        //  08/30/13 jrf 2.85.34 WR 418110 Updated variable name.
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //
        private ICMMfgTable2511ERTStatistics Table2511
        {
            get
            {
                if (null == m_Table2511 && Table00.IsTableUsed(2511))
                {
                    m_Table2511 = new ICMMfgTable2511ERTStatistics(m_PSEM, Table2510);
                }

                return m_Table2511;
            }
        }

        /// <summary>
        /// Gets the Module Configuration table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        private ICMMfgTable2512ModuleConfiguration Table2512
        {
            get
            {
                if (m_Table2512 == null && Table00.IsTableUsed(2512))
                {
                    m_Table2512 = new ICMMfgTable2512ModuleConfiguration(m_PSEM);
                }

                return m_Table2512;
            }
        }

        /// <summary>
        /// Gets the Module Data table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08 n/a    Created
        //
        private ICMMfgTable2515ModuleData Table2515
        {
            get
            {
                if (m_Table2515 == null && Table00.IsTableUsed(2515))
                {
                    m_Table2515 = new ICMMfgTable2515ModuleData(m_PSEM);
                }

                return m_Table2515;
            }
        }

        /// <summary>
        /// Gets the Module Status table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08 n/a    Created
        //  12/12/13 AF  3.50.14 TQ9508 Table 2516's size now depends on device class
        //
        private ICMMfgTable2516ModuleStatus Table2516
        {
            get
            {
                if (m_Table2516 == null && Table00.IsTableUsed(2516))
                {
                    m_Table2516 = new ICMMfgTable2516ModuleStatus(m_PSEM, m_AMIDevice.DeviceClass);
                }

                return m_Table2516;
            }
        }

        /// <summary>
        /// Gets the Cellular Config table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/13 jrf 2.80.20 TQ8278 Created
        //
        private ICMMfgTable2517CellularConfiguration Table2517
        {
            get
            {
                if (m_Table2517 == null && Table00.IsTableUsed(2517))
                {
                    m_Table2517 = new ICMMfgTable2517CellularConfiguration(m_PSEM);
                }

                return m_Table2517;
            }
        }

        /// <summary>
        /// Gets the Cell Data table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08 n/a    Created
        //
        private ICMMfgTable2518CellularData Table2518
        {
            get
            {
                if (m_Table2518 == null && Table00.IsTableUsed(2518))
                {
                    m_Table2518 = new ICMMfgTable2518CellularData(m_PSEM);
                }

                return m_Table2518;
            }
        }

        /// <summary>
        /// Gets the Cell Status table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/13 jkw 2.80.08 n/a    Created
        //
        private ICMMfgTable2519CellularStatus Table2519
        {
            get
            {
                if (m_Table2519 == null && Table00.IsTableUsed(2519))
                {
                    m_Table2519 = new ICMMfgTable2519CellularStatus(m_PSEM);
                }

                return m_Table2519;
            }
        }

        /// <summary>
        /// Gets the Table 2521 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  06/21/13 AF  2.80.40 TR7640 Cloned from ICS Gateway class
        //  04/04/14 jrf 3.50.61 461982 Removed passing unneeded version to 
        //                              table 2521's constructor.
        private ICSMfgTable2521 Table2521
        {
            get
            {
                if (null == m_Table2521)
                {
                    m_Table2521 = new ICSMfgTable2521(m_PSEM); 
                }

                return m_Table2521;
            }
        }

        /// <summary>
        /// Gets the Table 2522 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  06/21/13 AF  2.80.40 TR7640 Cloned from ICS Gateway class
        //
        private ICSMfgTable2522 Table2522
        {
            get
            {
                if (null == m_Table2522)
                {
                    m_Table2522 = new ICSMfgTable2522(m_PSEM, Table2521);
                }

                return m_Table2522;
            }
        }

        /// <summary>
        /// Gets the Table 2523 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  06/21/13 AF  2.80.40 TR7640 Cloned from ICS Gateway class
        //  07/29/16 AF  4.60.02 623194 Added a fw version parameter for table 2523 because the event
        //                              list supported is different between 3G and 4G meters.
        //
        private ICSMfgTable2523 Table2523
        {
            get
            {
                if (null == m_Table2523)
                {
                    m_Table2523 = new ICSMfgTable2523(m_PSEM, Table2522, Table2521, ICMFirmwareVersionMajor);
                }

                return m_Table2523;
            }
        }

        /// <summary>
        /// Gets the Table 2524 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/13 MSC 2.80.34 TR7640 Created
        //  06/21/13 AF  2.80.40 TR7640 Cloned from ICS Gateway class
        //
        private ICSMfgTable2524 Table2524
        {
            get
            {
                if (null == m_Table2524)
                {
                    m_Table2524 = new ICSMfgTable2524(m_PSEM, Table2521, m_EventDictionary);
                }

                return m_Table2524;
            }
        }

        /// <summary>
        /// Gets the Table 2525 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 07/18/13 jrf 2.80.54 WR 411418 Created.
        //
        private ICSMfgTable2525 Table2525
        {
            get
            {
                if (null == m_Table2525)
                {
                    m_Table2525 = new ICSMfgTable2525(m_PSEM, Table00);
                }

                return m_Table2525;
            }

        }

          /// <summary>
        /// Gets the Table 2539 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //  05/12/15 jrf 4.20.06  WR 584075  Added check for table support.
        private ICMMfgTable2539FWDLComponent Table2539
        {
            get
            {
                if (IsTableUsed(2539) && null == m_Table2539)
                {
                    m_Table2539 = new ICMMfgTable2539FWDLComponent(m_PSEM);
                }

                return m_Table2539;
            }
        }            

        /// <summary>
        /// Gets the Table 2542 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        private ICMMfgTable2542FileRetrievalActual Table2542
        {
            get
            {
                if (null == m_Table2542)
                {
                    m_Table2542 = new ICMMfgTable2542FileRetrievalActual(m_PSEM);
                }

                return m_Table2542;
            }
        }

        /// <summary>
        /// Gets the Table 2544 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //  06/08/14 jrf 4.20.12  WR 589725  Added check for table 0 support.
        private ICMMfgTable2544FileListTableDefinition Table2544
        {
            get
            {
                if (null == m_Table2544 && Table00.IsTableUsed(2544))
                {
                    m_Table2544 = new ICMMfgTable2544FileListTableDefinition(m_PSEM, Table2542);
                }

                return m_Table2544;
            }
        }

          /// <summary>
        /// Gets the Table 2751 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/05/13 jrf 2.85.10 TC 12652  Created.
        //
        private ICSMfgTable2751 Table2751
        {
            get
            {
                if (null == m_Table2751)
                {
                    m_Table2751 = new ICSMfgTable2751(m_PSEM, Table00);
                }

                return m_Table2751;
            }
        }

        #endregion

        #region Members

        private CTable00 m_Table0;
        private ICMMfgTable2508ERTData m_Table2508;
        private ICMMfgTable2509ERTConfigurationTable m_Table2509;
        private ICMMfgTable2510ERTActual m_Table2510;
        private ICMMfgTable2511ERTStatistics m_Table2511;
        private ICMMfgTable2512ModuleConfiguration m_Table2512;
        private ICMMfgTable2515ModuleData m_Table2515;
        private ICMMfgTable2516ModuleStatus m_Table2516;
        private ICMMfgTable2517CellularConfiguration m_Table2517;
        private ICMMfgTable2518CellularData m_Table2518;
        private ICMMfgTable2519CellularStatus m_Table2519;
        private ICSMfgTable2521 m_Table2521;
        private ICSMfgTable2522 m_Table2522;
        private ICSMfgTable2523 m_Table2523;
        private ICSMfgTable2524 m_Table2524;
        private ICSMfgTable2525 m_Table2525;
        private ICMMfgTable2539FWDLComponent m_Table2539;
        private ICMMfgTable2542FileRetrievalActual m_Table2542;
        private ICMMfgTable2544FileListTableDefinition m_Table2544;
        private ICSMfgTable2751 m_Table2751;
        private ICS_Gateway_EventDictionary m_EventDictionary;

        #endregion
    }
}
