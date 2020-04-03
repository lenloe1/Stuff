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
//                              Copyright © 2013 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Common.C1219Tables.Centron;
using Itron.Metering.Communications.PSEM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Common tables and properties related to ICS Comm Module.
    /// </summary>
    public class ICSTableReader
    {
        #region Constants

        /// <summary>
        /// Size of the invariant part of an ICS event entry.
        /// </summary>
        private const uint ICS_LOG_RCD_BASIC = 11;
        /// <summary>
        /// Size of the ERT statistics data record.
        /// </summary>
        private const int ERT_STAT_DATA_RCD_SIZE = 6;
        /// <summary>
        /// We can't read more than 1400 bytes in one offset read.  The following constant was 
        /// determined by trial and error
        /// </summary>
        private const int MAX_ENTRIES_IN_ONE_READ = 60;

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM"></param>
        /// <param name="device"></param>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 11/21/13 DLG 3.50.07           Created.
        // 01/09/14 DLG 3.50.23 TR 9993   Also related to TREQ 9996. Added ANSIDevice as parameter.
        // 
        public ICSTableReader(CPSEM PSEM, CANSIDevice device)
        {
            m_PSEM = PSEM;
            m_ANSIDevice = device;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Reads Table 2508 (mfg 460)
        /// </summary>
        /// <param name="MeterTables">The meter tables object</param>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/03/13 AF  2.85.34 WR418110 Created
        //
        public PSEMResponse ReadTable2508(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] PSEMData;
            ushort usTableSize = 0;
            object objData;
            byte bytNumberOfDataRecords = 0;
            byte bytDataRecordSize = 0;

            if (MeterTables.IsCached((long)CentronTblEnum.MfgTbl462NumberOfDataRecords, null))
            {
                MeterTables.GetValue((long)CentronTblEnum.MfgTbl462NumberOfDataRecords, null, out objData);
                bytNumberOfDataRecords = (byte)objData;
            }

            if (MeterTables.IsCached((long)CentronTblEnum.MfgTbl462DataRecordSize, null))
            {
                MeterTables.GetValue((long)CentronTblEnum.MfgTbl462DataRecordSize, null, out objData);
                bytDataRecordSize = (byte)objData;
            }

            usTableSize = (ushort)(bytNumberOfDataRecords * bytDataRecordSize);

            // Read the records
            PSEMResult = m_PSEM.OffsetRead(2508, 0, usTableSize, out PSEMData);

            if (PSEMResult == PSEMResponse.Ok)
            {
                PSEMDataStream = new MemoryStream(PSEMData, 0, PSEMData.Length, true, true);
                MeterTables.SavePSEMStream(2508, PSEMDataStream, 0);
            }

            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2511 (mfg 463)
        /// </summary>
        /// <param name="MeterTables">The meter tables object</param>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/03/13 AF  2.85.34 WR418110 Created
        //
        public PSEMResponse ReadTable2511(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] PSEMData;
            ushort usTableSize = 0;
            object objData;
            byte bytNumberStatRecords = 0;

            if (MeterTables.IsCached((long)CentronTblEnum.MfgTbl462NumberOfStatRecords, null))
            {
                MeterTables.GetValue((long)CentronTblEnum.MfgTbl462NumberOfStatRecords, null, out objData);
                bytNumberStatRecords = (byte)objData;
            }

            usTableSize = (ushort)(bytNumberStatRecords * ERT_STAT_DATA_RCD_SIZE);

            // Read the records
            PSEMResult = m_PSEM.OffsetRead(2511, 0, usTableSize, out PSEMData);

            if (PSEMResult == PSEMResponse.Ok)
            {
                PSEMDataStream = new MemoryStream(PSEMData, 0, PSEMData.Length, true, true);
                MeterTables.SavePSEMStream(2511, PSEMDataStream, 0);
            }

            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2524 (mfg 476)
        /// </summary>
        /// <param name="MeterTables">The meter tables object</param>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/27/13 AF  2.80.44 TR7648 Created
        //  07/16/13 AF  2.80.53 WR417522 Adjusted the size of the offset reads to make sure that each
        //                                reads an even multiple of entries.
        //  06/19/15 AF  4.20.14 591427 Calculate the table size instead of retrieving from the CE dlls
        //  07/23/15 AF  4.20.18 597509 The number of entries from table 2521 might not be correct so read from 2524
        //
        public PSEMResponse ReadTable2524(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] PSEMData;

            uint TableSize = 0;
            uint CurrentOffset = 0;

            uint NumberUnreadEntriesOffset;
            uint NumberUnreadEntriesLength;
            ushort NumberICSEntries = 0;
            uint HeaderSize;
            object objData;
            uint SizeOfEntry = ICS_LOG_RCD_BASIC;
            bool blnEventNumberFlag = false;

            if (MeterTables.IsCached((long)CentronTblEnum.MFGTBL473_EVENT_NUMBER_FLAG, null))
            {
                MeterTables.GetValue((long)CentronTblEnum.MFGTBL473_EVENT_NUMBER_FLAG, null, out objData);
                blnEventNumberFlag = (bool)objData;
            }

            if (MeterTables.IsCached((long)CentronTblEnum.MFGTBL473_ICS_DATA_LENGTH, null))
            {
                MeterTables.GetValue((long)CentronTblEnum.MFGTBL473_ICS_DATA_LENGTH, null, out objData);
                SizeOfEntry += (byte)objData;
            }

            if (blnEventNumberFlag)
            {
                SizeOfEntry += sizeof(UInt16);
            }

            MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL476_NBR_UNREAD_ENTRIES, null, out NumberUnreadEntriesOffset, out NumberUnreadEntriesLength);

            HeaderSize = NumberUnreadEntriesOffset + NumberUnreadEntriesLength;

            // Read the header
            PSEMResult = m_PSEM.OffsetRead(2524, 0, (ushort)HeaderSize, out PSEMData);

            if (PSEMResult == PSEMResponse.Ok)
            {
                PSEMDataStream = new MemoryStream(PSEMData);
                MeterTables.SavePSEMStream(2524, PSEMDataStream, 0);

                CurrentOffset += HeaderSize;
                ushort BytesToRead;

                if (MeterTables.IsCached((long)CentronTblEnum.MFGTBL476_NBR_VALID_ENTRIES, null))
                {
                    MeterTables.GetValue((long)CentronTblEnum.MFGTBL476_NBR_VALID_ENTRIES, null, out objData);
                    NumberICSEntries = (ushort)objData;
                }

                TableSize = HeaderSize + (SizeOfEntry * NumberICSEntries);

                // Read the entries
                while (CurrentOffset < TableSize)
                {
                    if ((TableSize - CurrentOffset) < MAX_ENTRIES_IN_ONE_READ * SizeOfEntry)
                    {
                        BytesToRead = (ushort)(TableSize - CurrentOffset);
                    }
                    else
                    {
                        BytesToRead = (ushort)(MAX_ENTRIES_IN_ONE_READ * SizeOfEntry);
                    }

                    PSEMResult = m_PSEM.OffsetRead(2524, (int)CurrentOffset, BytesToRead, out PSEMData);

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMDataStream = new MemoryStream(PSEMData);
                        MeterTables.SavePSEMStream(2524, PSEMDataStream, CurrentOffset);

                        CurrentOffset += BytesToRead;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return PSEMResult;
        }

        /// <summary>
        /// Returns the Diagnostic Events. These events are from the ICS Comm Module simliar to the
        /// "Events" property, however, these events have been filtered to only display the 
        /// diagnostic events and not the customer events.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 01/09/14 DLG 3.50.23 TR 9993   Created. Also related to TREQ 9996.
        // 
        public List<HistoryEntry> DiagnosticEvents
        {
            get
            {
                List<HistoryEntry> listOfCommEvents = new List<HistoryEntry>();
                ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;

                ICSCommModule ICSModule = m_ANSIDevice.CommModule as ICSCommModule;

                byte[] commandResponse;

                if (ICSModule != null)
                {
                    if (ICSModule.FilterICSEvents(ICSFilterSelection.DIAGNOSTIC_FILTER) == ProcedureResultCodes.COMPLETED)
                    {
                        Result = ICSModule.UpdateEventTables(new DateTime(1970, 1, 1), DateTime.MaxValue, out commandResponse);

                        if (Result == ProcedureResultCodes.COMPLETED)
                        {
                            listOfCommEvents.AddRange(ICSModule.CommModuleEvents);
                        }
                    }
                }

                return listOfCommEvents;
            }
        }

        #endregion Protected Methods

        #region Members

        /// <summary>
        /// The PSEM protocol object.
        /// </summary>
        protected CPSEM m_PSEM = null;
        /// <summary>
        /// The ANSIDevice object.
        /// </summary>
        protected CANSIDevice m_ANSIDevice = null;

        #endregion Members
    }
}