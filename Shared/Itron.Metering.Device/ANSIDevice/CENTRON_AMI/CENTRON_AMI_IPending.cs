///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                           Copyright © 2006 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the CENTRON_AMI, IPending interface
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 09/28/06 AF 7.40.00  N/A    Created
    // 
    public partial class CENTRON_AMI : IPending
    {
        #region Definitions

        /// <summary>
        /// Enum of the table ids of the supported pending tables
        /// </summary>
        protected enum PendingTableIds
        {
            /// <summary>
            /// Pending table id for TOU config table
            /// </summary>
            TOUConfigTbl = 2090,
            /// <summary>
            /// Pending table id for register firmware
            /// </summary>
            RegisterFWTbl = 2109,
            /// <summary>
            /// Pending table id for RFLAN firmware
            /// </summary>
            CommModuleFWTbl = 2110,
            /// <summary>
            /// Pending table id for Zigbee firmware
            /// </summary>
            HANModuleFWTbl = 2111,
            /// <summary>
            /// Pending table ID for HAN Pricing
            /// </summary>
            HANPricingTable = 2134,
            /// <summary>
            /// Pending table id for HAN device firmware
            /// </summary>
            HANDeviceFWTbl = 2181,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the pending table with the specified Event Record
        /// </summary>
        /// <param name="pendingEvent">The Event Record for the event to activate</param>
        /// <returns>ProcedureResultCodes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public ProcedureResultCodes ActivatePendingTable(PendingEventRecord pendingEvent)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;

            ProcResult = ExecuteProcedure(Procedures.ACTIVATE_PENDING_TABLE,
                                          pendingEvent.EntireRecord,
                                          out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Clears the pending table with the specified Event Record
        /// </summary>
        /// <param name="pendingEvent">The Event Record for the event to clear</param>
        /// <returns>ProcedureResultCodes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public ProcedureResultCodes ClearPendingTable(PendingEventRecord pendingEvent)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;

            ProcResult = ExecuteProcedure(Procedures.CLEAR_SPECIFIC_PENDING_TABLE,
                                          pendingEvent.EntireRecord,
                                          out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Clears a specific pending table by the table id
        /// </summary>
        /// <param name="TableID">the pending table to be cleared</param>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/06/17 AF  4.73.00  Task 469254 Created
        //
        public ProcedureResultCodes ClearPendingTableByTableID(UInt16 TableID)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[2];
            Array.Copy(BitConverter.GetBytes((UInt16)TableID), ProcParam, 2);

            ProcResult = ExecuteProcedure(Procedures.CLEAR_PENDING_TABLE_BY_ID, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Clears all pending tables from the meter.
        /// </summary>
        /// <returns>The result of the procedure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/08 RCG 2.00.03 122067 Created

        public ProcedureResultCodes ClearAllPendingTables()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[0];

            ProcResult = ExecuteProcedure(Procedures.CLEAR_ALL_PENDING_TABLES,
                                          ProcParam,
                                          out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Activates all pending tables in the meter.
        /// </summary>
        /// <returns>The result of the procedure.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/08 RCG 2.00.03 122067 Created

        public ProcedureResultCodes ActivateAllPendingTables()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[0];

            ProcResult = ExecuteProcedure(Procedures.ACTIVATE_ALL_PENDING_TABLES,
                                          ProcParam,
                                          out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Determines whether or not a firmware download pending table awaiting
        /// activation exists
        /// </summary>
        /// <returns>True if there is a f/w pending table awaiting activation; 
        /// false, otherwise</returns>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/01/06 AF 7.40.00  N/A    Created
        // 05/13/08 AF  1.50.26        Removed IPending from method signature.
        // 04/03/09 AF  2.20.00         Added 3rd party HAN device fw pending table
        //                              to the check
        // 11/11/13 AF  3.50.02 TQ9508,9514 Made virtual so that we can override in ICS_Gateway.
        //
        public virtual bool FWTableStillPendingExists()
        {
            bool blnExists = false;

            if (0 < Table04.NumberPendingTables)
            {
                List<PendingEventActivationRecord> lstEvtRcds = new List<PendingEventActivationRecord>();
                int iIndex;

                //go through the pending table records looking for a f/w table
                lstEvtRcds = Table04.PendingTableEntries;
                for (iIndex = 0; iIndex < lstEvtRcds.Count; iIndex++)
                {
                    if (((ushort)PendingTableIds.RegisterFWTbl == lstEvtRcds[iIndex].TableID) &&
                        (true == lstEvtRcds[iIndex].StillPending))
                    {
                        blnExists = true;
                        break;
                    }
                    else if (((ushort)PendingTableIds.CommModuleFWTbl == lstEvtRcds[iIndex].TableID) &&
                        (true == lstEvtRcds[iIndex].StillPending))
                    {
                        blnExists = true;
                        break;
                    }
                    else if (((ushort)PendingTableIds.HANModuleFWTbl == lstEvtRcds[iIndex].TableID) &&
                        (true == lstEvtRcds[iIndex].StillPending))
                    {
                        blnExists = true;
                        break;
                    }
                    else if (((ushort)PendingTableIds.HANDeviceFWTbl == lstEvtRcds[iIndex].TableID) &&
                        (true == lstEvtRcds[iIndex].StillPending))
                    {
                        blnExists = true;
                        break;
                    }
                }
            }

            return blnExists;
        }

        /// <summary>
        /// Determines whether or not there is an incomplete firmware pending 
        /// table
        /// </summary>
        /// <returns>
        /// true if all firmware pending tables are complete and false otherwise
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/05/07 AF  8.00.05  6     Created
        //  04/03/09 AF  2.20.00        Added support for HAN device firmware downloads
        //
        public virtual bool FWPendingTableDownloadComplete()
        {
            bool blnDownloadComplete = true;

            List<PendingEventActivationRecord> lstEntryActivation = new List<PendingEventActivationRecord>();
            PendingEventActivationRecord rcdEventActivation = new PendingEventActivationRecord();
            int intIndex;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
            byte[] ProcResponse;
            byte bytNumberPendingFound = 0;

            //This will get the data back from the meter in its raw form
            lstEntryActivation = Table04.PendingTableEntries;

            // Find out how many tables are actually pending
            byte bytNumberPending = Table04.NumberPendingTables;

            // Now we need to translate it into a form we can use
            for (intIndex = 0;
                (bytNumberPendingFound < bytNumberPending)
                    && (intIndex < Table04.NumberPendingSupported);
                intIndex++)
            {

                rcdEventActivation = lstEntryActivation[intIndex];

                if (true == rcdEventActivation.StillPending)
                {
                    switch (rcdEventActivation.TableID)
                    {
                        case (ushort)PendingTableIds.RegisterFWTbl:
                        {
                            // Use mfg proc 46 to find out if all blocks are present
                            ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.RegisterFW,
                                                out ProcResponse);
                            break;
                        }
                        case (ushort)PendingTableIds.CommModuleFWTbl:
                        {
                            // Use mfg proc 46 to find out if all blocks are present
                            ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.RFLANFW,
                                                out ProcResponse);
                            break;
                        }
                        case (ushort)PendingTableIds.HANModuleFWTbl:
                        {
                            // Use mfg proc 46 to find out if all blocks are present
                            ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.ZigbeeFW,
                                                   out ProcResponse);
                            break;
                        }
                        case (ushort)PendingTableIds.HANDeviceFWTbl:
                        {
                            // Use mfg proc 46 to find out if all blocks are present
                            ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.HANDevFW,
                                                    out ProcResponse);
                            break;
                        }
                        default:
                        {
                            ProcResponse = new byte[MFG_PROC_46_RESPONSE_LENGTH];
                            ProcResponse.Initialize();
                            break;
                        }
                    }

                    if (ProcedureResultCodes.COMPLETED == ProcResult)
                    {
                        // if the download is complete, then the number of blocks
                        // received will be non-zero and the array of missing blocks will
                        // all be zero
                        if (false == IsDownloadComplete(ref ProcResponse))
                        {
                            // table is still being transmitted
                            blnDownloadComplete = false;
                        }
                    }

                    bytNumberPendingFound++;
                }
            }
            return blnDownloadComplete;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Date of the last pending table activation
        /// </summary>
        /// <example>
        /// <code>
        /// DateTime dtLastDate = ((IPending)m_ItronDevice).LastActivationDate
        /// </code>
        /// </example>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/28/06 AF 7.40.00  N/A    Created
        //
        public DateTime LastActivationDate
        {
            get
            {
                m_dtLastActivationDate = Table04.LastActivationDate;
                return m_dtLastActivationDate;
            }
        }

        /// <summary>
        /// Retrieves the pending table activation records and places them
        /// in a list.  The table id is a short but all the rest of
        /// the fields are strings.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/28/06 AF 7.40.00  N/A    Created
        // 10/19/06 AF 7.40.00  N/A    corrected bug with bytNumberPendingFound
        // 04/03/09 AF 2.20.00          Added support for 3rd party HAN device firmware pending tables
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString")]
        public List<PendingEventActivationRecord> PendingTableData
        {
            get
            {
                List<PendingEventActivationRecord> lstEntryActivation = new List<PendingEventActivationRecord>();
                PendingEventActivationRecord CurrentPendingEvent;
                int intIndex;
                DateTime dtActivationTime;
                ProcedureResultCodes ProcResult = ProcedureResultCodes.UNRECOGNIZED_PROC;
                byte[] ProcResponse;
                byte bytNumberPendingFound = 0;

                m_lstPendingTableRecords = new List<PendingEventActivationRecord>();

                //This will get the data back from the meter in its raw form
                lstEntryActivation = Table04.PendingTableEntries;

                // Find out how many tables are actually pending
                byte bytNumberPending = Table04.NumberPendingTables;

                // Now we need to translate it into a form we can use
                for (intIndex = 0;
                    (bytNumberPendingFound < bytNumberPending)
                        && (intIndex < Table04.NumberPendingSupported);
                    intIndex++)
                {
                    CurrentPendingEvent = lstEntryActivation[intIndex];

                    if (true == CurrentPendingEvent.StillPending)
                    {
                        // Table Name
                        switch (CurrentPendingEvent.TableID)
                        {
                            case (ushort)PendingTableIds.RegisterFWTbl:
                            {
                                CurrentPendingEvent.TableName = m_rmStrings.GetString("REG_FW_TABLE");

                                // Use mfg proc 46 to find out if all blocks are present
                                ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.RegisterFW,
                                                    out ProcResponse);
                                break;
                            }
                            case (ushort)PendingTableIds.CommModuleFWTbl:
                            {
                                CurrentPendingEvent.TableName = m_rmStrings.GetString("RFLAN_FW_TABLE");

                                // Use mfg proc 46 to find out if all blocks are present
                                ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.RFLANFW,
                                                    out ProcResponse);
                                break;
                            }
                            case (ushort)PendingTableIds.HANModuleFWTbl:
                            {
                                CurrentPendingEvent.TableName = m_rmStrings.GetString("ZIGBEE_FW_TABLE");

                                // Use mfg proc 46 to find out if all blocks are present
                                ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.ZigbeeFW,
                                                       out ProcResponse);
                                break;
                            }
                            case (ushort)PendingTableIds.TOUConfigTbl:
                            {
                                CurrentPendingEvent.TableName = m_rmStrings.GetString("TOU_CONFIG_TABLE");
                                ProcResponse = new byte[MFG_PROC_46_RESPONSE_LENGTH];
                                ProcResponse.Initialize();
                                break;
                            }
                            case (ushort)PendingTableIds.HANDeviceFWTbl:
                            {
                                CurrentPendingEvent.TableName = m_rmStrings.GetString("HAN_DEVICE_FW_TABLE");

                                // Use mfg proc 46 to find out if all blocks are present
                                ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.HANDevFW,
                                                        out ProcResponse);
                                break;
                            }
                            case (ushort)PendingTableIds.HANPricingTable:
                            {
                                CurrentPendingEvent.TableName = m_rmStrings.GetString("HAN_PRICING_TABLE");
                                ProcResponse = new byte[MFG_PROC_46_RESPONSE_LENGTH];
                                ProcResponse.Initialize();
                                break;
                            }
                            default:
                            {
                                CurrentPendingEvent.TableName = m_rmStrings.GetString("UNKNOWN_TABLE");
                                ProcResponse = new byte[MFG_PROC_46_RESPONSE_LENGTH];
                                ProcResponse.Initialize();
                                break;
                            }
                        }

                        //Activation Type & activation trigger
                        if (PendingEventRecord.PendingEventCode.AbsoluteTimeTrigger == CurrentPendingEvent.Event.EventCode)
                        {
                            MemoryStream EventStorageStream = new MemoryStream(CurrentPendingEvent.Event.EventStorage);
                            PSEMBinaryReader EventStorageReader = new PSEMBinaryReader(EventStorageStream);

                            // Event Selector is 0 so Event Storage is an STIME_DATE
                            dtActivationTime = EventStorageReader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);

                            CurrentPendingEvent.Status = dtActivationTime.ToLocalTime().ToString();
                        }
                        else if (PendingEventRecord.PendingEventCode.NonTimeTrigger == CurrentPendingEvent.Event.EventCode)
                        {
                            if (ProcedureResultCodes.COMPLETED == ProcResult)
                            {
                                // if the download is complete, then the number of blocks
                                // received will be non-zero and the array of missing blocks will
                                // all be zero
                                if (true == IsDownloadComplete(ref ProcResponse))
                                {
                                    // table is ready for activation
                                    CurrentPendingEvent.Status = m_rmStrings.GetString("FW_DOWNLOAD_COMPLETE");
                                }
                                else
                                {
                                    // table is still being transmitted
                                    CurrentPendingEvent.Status = m_rmStrings.GetString("FW_DOWNLOAD_INCOMPLETE");
                                }
                            }
                            else
                            {
                                CurrentPendingEvent.Status = m_rmStrings.GetString("SW_INIT_EVENT");
                            }
                        }
                        else
                        {
                            CurrentPendingEvent.Status = m_rmStrings.GetString("UNKNOWN");
                        }

                        m_lstPendingTableRecords.Add(CurrentPendingEvent);
                        bytNumberPendingFound++;
                    }
                }

                return m_lstPendingTableRecords;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method parses out the results from mfg procedure 46 
        /// (GetFirstTwentyMissingBlocks()).  If the number of blocks received is non-zero
        /// and the array of missing blocks are all zero, then the download is complete.
        /// </summary>
        /// <param name="ProcResponse">byte array representing the procedure response</param>
        /// <returns>true if download is complete; false, otherwise</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 AF 7.40.00  N/A    Created
        //
        protected bool IsDownloadComplete(ref byte[] ProcResponse)
        {
            bool blnResult = true; ;
            int intNumberReceived = ProcResponse[0] | (ProcResponse[1] << 8);
            if (0 == intNumberReceived)
            {
                // Is this the case?  How would we get a pending table with no blocks received?
                blnResult = false;
            }
            else
            {
                for (int iIndex = 0; iIndex < 20; iIndex++)
                {
                    if (ProcResponse[iIndex + 2] != 0)
                    {
                        blnResult = false;
                        break;
                    }
                }
            }
            return blnResult;
        }

        /// <summary>
        /// Activates a specific pending table by calling standard procedure 13
        /// </summary>
        /// <param name="bSelfRead">Whether or not a self read should be performed 
        /// before activation</param>
        /// <param name="bDemandReset">Whether or not a demand reset should be 
        /// performed before activation</param>
        /// <param name="byMfgEventCode">Mfg assigned code identifying event for 
        /// activating pending table</param>
        /// <param name="eCode">Event code for status bitfield.  Use this field
        /// to identify the pending table</param>
        /// <returns>ProcedureResultCodes</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/25/06 AF  7.35.00  N/A   Created
        // 09/01/06 AF  7.35.00        Corrected transfer of data from stream to array
        // 10/04/06 AF  7.40.00  N/A   Moved from CENTRON_AMI.cs
        // 06/08/07 RCG 8.10.07        Changed to call ActivatePendingTable with PendingEventRecord

        protected ProcedureResultCodes ActivatePendingTable(bool bSelfRead,
                                                bool bDemandReset,
                                                byte byMfgEventCode,
                                                PendingEventRecord.PendingEventCode eCode)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            PendingEventRecord EventRecord;

            // Build the event record
            EventRecord = BuildPendingEventRecord(bSelfRead, bDemandReset, byMfgEventCode, eCode);

            // Clear the table
            ProcResult = ActivatePendingTable(EventRecord);

            return ProcResult;
        }

        /// <summary>
        /// Clears a specific pending table by calling standard procedure 15
        /// </summary>
        /// <param name="bSelfRead">Whether or not a self read should be performed 
        /// before activation</param>
        /// <param name="bDemandReset">Whether or not a demand reset should be 
        /// performed before activation</param>
        /// <param name="byMfgEventCode">Mfg assigned code identifying event for 
        /// activating pending table</param>
        /// <param name="eCode">Event code for status bitfield.  Use this field
        /// to identify the pending table</param>
        /// <returns>ProcedureResultCodes</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/25/06 AF 7.35.00  N/A     Created
        // 09/01/06 AF 7.35.00          Corrected code to copy stream to array
        // 10/04/06 AF 7.40.00  N/A     Moved from CENTRON_AMI.cs
        // 06/08/07 RCG 8.10.07        Changed to call ClearPendingTable with PendingEventRecord

        protected ProcedureResultCodes ClearPendingTable(bool bSelfRead,
                                                bool bDemandReset,
                                                byte byMfgEventCode,
                                                PendingEventRecord.PendingEventCode eCode)
        {

            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            PendingEventRecord EventRecord;

            // Build the event record
            EventRecord = BuildPendingEventRecord(bSelfRead, bDemandReset, byMfgEventCode, eCode);

            // Clear the pending table
            ProcResult = ClearPendingTable(EventRecord);

            return ProcResult;
        }

        /// <summary>
        /// Queries the meter for the first 20 missing blocks for a firmware
        /// download pending table.
        /// </summary>
        /// <param name="eFWType">Firmware type (0 = Register, 1 = RFLAN, 
        /// 2 = Zigbee)</param>
        /// <param name="ProcResponse">byte array containing the response from 
        /// the meter</param>
        /// <returns>Result code written to table 08 after the procedure is
        /// initiated</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 AF 7.40.00  N/A     Created
        // 10/04/06 AF 7.40.00  N/A     Moved from CENTRON_AMI.cs
        // 
        protected ProcedureResultCodes GetFirstTwentyMissingBlocks(FirmwareType eFWType,
                                                out byte[] ProcResponse)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            byte[] byParameters = new byte[1];  // The procedure takes 1 parameter
            byParameters[0] = (byte)eFWType;

            ProcResult = ExecuteProcedure(Procedures.GET_FIRST_20_MISSING_BLOCKS,
                                              byParameters,
                                              out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Queries the meter for the first 20 missing blocks for a firmware
        /// download pending table.
        /// </summary>
        /// <param name="eFWType">Firmware type (0 = Register, 1 = RFLAN, 
        /// 2 = Zigbee)</param>
        /// <param name="iNumberOfBlocksReceived"></param>
        /// <param name="lstMissingBlocks"></param>
        /// <returns>Result code written to table 08 after the procedure is
        /// initiated</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/10 jrf 2.40.12  N/A     Created
        // 
        public ProcedureResultCodes GetFirstTwentyMissingBlocks(FirmwareType eFWType,
                                                out int iNumberOfBlocksReceived, out List<int> lstMissingBlocks)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            int iBlockNumber = 0;

            ProcResult = GetFirstTwentyMissingBlocks(eFWType, out ProcResponse);

            lstMissingBlocks = new List<int>();
            iNumberOfBlocksReceived = 0;

            if (ProcedureResultCodes.COMPLETED == ProcResult)
            {
                iNumberOfBlocksReceived = ProcResponse[0] | (ProcResponse[1] << 8);

                for (int iIndex = 2; iIndex < 42; iIndex += 2)
                {
                    iBlockNumber = ProcResponse[iIndex] | (ProcResponse[iIndex + 1] << 8);
                    if (0 != iBlockNumber)
                    {
                        lstMissingBlocks.Add(iBlockNumber);
                    }
                }
            }

            return ProcResult;
        }

        #endregion

        #region Member Variables

        private DateTime m_dtLastActivationDate;
        private List<PendingEventActivationRecord> m_lstPendingTableRecords;

        #endregion
    }
}




