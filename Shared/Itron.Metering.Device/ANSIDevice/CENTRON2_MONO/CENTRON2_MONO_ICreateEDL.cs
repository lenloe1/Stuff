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
//                           Copyright © 2006 - 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Itron.Metering.Progressable;
using Itron.Metering.Communications.PSEM;
using Itron.Common.C1219Tables.ANSIStandardII;
using Itron.Common.C1219Tables.CentronII;

namespace Itron.Metering.Device
{
    public partial class CENTRON2_MONO 
    {
        #region Constants
        private const uint PACKET_OVERHEAD_SIZE = 8;

        private const uint PACKETS_PER_READ = 254;
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates an EDL file with the specified sections.
        /// </summary>
        /// <param name="FileName">Path to the file where the EDL file will be written.</param>
        /// <param name="IncludedSections">The sections to include in the EDL file.</param>
        /// <returns>CreateEDLResult Code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/25/08 RCG	1.50.19			Created
        // 07/06/10 AF  2.42.02         Made virtual for use in the M2 Gateway

        public override CreateEDLResult CreateEDLFromMeter(string FileName, EDLSections IncludedSections)
        {
            CentronTables MeterTables = new CentronTables();
            List<ushort> TablesToRead;
            int iFileNameStart;
            string strDirectory;
            CreateEDLResult Result = CreateEDLResult.SUCCESS;
            PSEMResponse PSEMResult = PSEMResponse.Ok;

            // First check to make sure we can create the file
            iFileNameStart = FileName.LastIndexOf(@"\", StringComparison.Ordinal);

            if (iFileNameStart > 0)
            {
                strDirectory = FileName.Substring(0, iFileNameStart);

                if (Directory.Exists(strDirectory) == false)
                {
                    Result = CreateEDLResult.INVALID_PATH;
                }
            }

            // Make sure we will be able to write to the file
            if (Result == CreateEDLResult.SUCCESS && File.Exists(FileName) == true)
            {
                FileInfo OutputFile = new FileInfo(FileName);

                if ((OutputFile.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    Result = CreateEDLResult.INVALID_PATH;
                }
            }

            if (Result == CreateEDLResult.SUCCESS)
            {

                // Read the data from the meter
                TablesToRead = GetTablesToRead(IncludedSections);

                OnShowProgress(new ShowProgressEventArgs(1, TablesToRead.Count, "Creating EDL file...", "Creating EDL file..."));

                foreach (ushort TableID in TablesToRead)
                {
                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        // Read the table if it exists
                        if ((Table00.IsTableUsed(TableID) == true))
                        {
                            if (MeterTables.GetTableDependencies(TableID).Contains(TableID) || MeterTables.GetTableLength(TableID) > 0)
                            {
                                PSEMResult = ReadTable(TableID, ref MeterTables);
                              
                                if (PSEMResult == PSEMResponse.Bsy
                                    || PSEMResult == PSEMResponse.Dnr
                                    || PSEMResult == PSEMResponse.Iar
                                    || PSEMResult == PSEMResponse.Onp
                                    || PSEMResult == PSEMResponse.Err)
                                {
                                    // We can't read the table but we should be able to continue we just need to
                                    // clear out anything that is there.
                                    MeterTables.ClearTable(TableID);
                                    PSEMResult = PSEMResponse.Ok;
                                }                                 
                            }
                        }

                        OnStepProgress(new ProgressEventArgs());
                    }
                }

                if (PSEMResult == PSEMResponse.Isc)
                {
                    Result = CreateEDLResult.SECURITY_ERROR;
                }
                else if (PSEMResult != PSEMResponse.Ok)
                {
                    Result = CreateEDLResult.PROTOCOL_ERROR;
                }
            }

#if (WindowsCE)
			//The saving of the EDL file on the handheld can take over 6 seconds so we need
			//to send a wait before.
			m_PSEM.Wait(CPSEM.MAX_WAIT_TIME);			
#endif

            // Generate the EDL file
            if (Result == CreateEDLResult.SUCCESS)
            {
                XmlWriterSettings WriterSettings = new XmlWriterSettings();
                WriterSettings.Encoding = Encoding.ASCII;
                WriterSettings.Indent = true;
                WriterSettings.CheckCharacters = false;

                XmlWriter EDLWriter = XmlWriter.Create(FileName, WriterSettings);

                MeterTables.SaveEDLFile(EDLWriter, null, AllowTableExport, AllowFieldExport);
                System.Threading.Thread.Sleep(2000); 
            }

            OnHideProgress(new EventArgs());

            return Result;
        }


        /// <summary>
        /// Creates an EDL file that contains all of the data for a meter.
        /// </summary>
        /// <param name="FileName">The file name that will be used to store the file.</param>
        /// <returns>CreateEDLResult code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created
        // 07/06/10 AF  2.42.02         Made virtual for use in the M2 Gateway

        public override CreateEDLResult CreateEDLFromMeter(string FileName)
        {
            // EDLSections AllSections = EDLSections.HistoryLog
            //    | EDLSections.LoadProfile
            //    | EDLSections.NetworkTables
            //    | EDLSections.VoltageMonitoring
            //    | EDLSections.LANandHANLog;

            EDLSections AllSections = (EDLSections.HistoryLog | EDLSections.LoadProfile | EDLSections.VoltageMonitoring);
            return CreateEDLFromMeter(FileName, AllSections);
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Reads the specified table from the meter.
        /// </summary>
        /// <param name="usTableID">The table ID for the table to read.</param>
        /// <param name="MeterTables">The tables object to read the table into.</param>
        /// <returns>PSEMResponse code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created
        // 01/11/07 RCG 8.00.05         Removed code that would do a full read since
        //                              the meter no longer supports full reads of 64

        protected PSEMResponse ReadTable(ushort usTableID, ref CentronTables MeterTables)
        {
            MemoryStream PSEMDataStream;
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] byaData;
            int iReadAttempt = 0;
            bool bRetry = true;

            while (bRetry)
            {
                switch (usTableID)
                {
                    case 64:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable64(ref MeterTables);
                            }

                            break;
                        }
                    case 2152:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = ReadTable2152(ref MeterTables);
                            }

                            break;
                        }
                    default:
                        {
                            if (0x0800 == usTableID)
                                PSEMResult = PSEMResponse.Ok;

                            PSEMResult = m_PSEM.FullRead(usTableID, out byaData);

                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMDataStream = new MemoryStream(byaData);
                                MeterTables.SavePSEMStream(usTableID, PSEMDataStream);
                            }

                            break;
                        }
                }

                iReadAttempt++;

                if (iReadAttempt < 3 && (PSEMResult == PSEMResponse.Bsy || PSEMResult == PSEMResponse.Dnr))
                {
                    bRetry = true;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    bRetry = false;
                }

            }

            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 64 from the meter.
        /// </summary>
        /// <param name="MeterTables">The table object for the meter.</param>
        /// <returns>The PSEM response code.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/08 RCG 1.50.24 N/A    Created

        protected PSEMResponse ReadTable64(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] byaData;

            CentronTables TempTables = new CentronTables();
            uint uiMaxOffsetReadBytes;
            uint uiReadMemorySize;
            uint uiNumberOfReads;
            uint uiBytesToRead;
            uint uiCurrentOffset;
            uint uiBlockOffset;
            uint uiBlockLength;
            uint uiMaxBlocksToRead;
            uint uiMaxBytesToRead;
            ushort usValidBlocks;
            ushort usNumberIntervals;

            ushort usNewValidBlocks;
            ushort usNewNumberIntervals;
            ushort usNewLastBlock;
            int iBlockToRead;

            // This must be initialized to false or you will break the retry logic.
            bool bBlocksReRead = false;

            object objData;

            // Since Load Profile can be very large (144k) it may not be able
            // to be read completely when doing a full read so we need to break
            // it up into multiple offset reads. Table 61 must be read prior to this.

            if (MeterTables.IsCached((long)StdTableEnum.STDTBL61_LP_MEMORY_LEN, null) == true)
            {
                uiMaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                if (uiMaxOffsetReadBytes > ushort.MaxValue)
                {
                    uiMaxOffsetReadBytes = ushort.MaxValue;
                }

                MeterTables.GetValue((long)StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null, out objData);
                usValidBlocks = (ushort)objData;

                MeterTables.GetValue((long)StdTableEnum.STDTBL63_NBR_VALID_INT, null, out objData);
                usNumberIntervals = (ushort)objData;

                // Determine the size of a Load Profile data block
                MeterTables.GetFieldOffset((long)StdTableEnum.STDTBL64_LP_DATA_SETS, new int[] { 0 },
                    out uiBlockOffset, out uiBlockLength);

                // Determine how many blocks can be read in an offset read
                uiMaxBlocksToRead = uiMaxOffsetReadBytes / uiBlockLength;
                uiMaxBytesToRead = uiMaxBlocksToRead * uiBlockLength;

                // Determine total amount to read
                uiReadMemorySize = usValidBlocks * uiBlockLength;

                // Determine how many reads need to be done
                uiNumberOfReads = usValidBlocks / uiMaxBlocksToRead;

                // Add in a read for any remaining data
                if (usValidBlocks % uiMaxBlocksToRead > 0)
                {
                    uiNumberOfReads++;
                }

                uiCurrentOffset = 0;

                for (uint iIndex = 0; iIndex < uiNumberOfReads && PSEMResult == PSEMResponse.Ok; iIndex++)
                {
                    uiBytesToRead = uiReadMemorySize - uiCurrentOffset;

                    if (uiBytesToRead > uiMaxBytesToRead)
                    {
                        uiBytesToRead = uiMaxBytesToRead;
                    }

                    PSEMResult = m_PSEM.OffsetRead(64, (int)uiCurrentOffset, (ushort)uiBytesToRead, out byaData);

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMDataStream = new MemoryStream(byaData);
                        MeterTables.SavePSEMStream(64, PSEMDataStream, uiCurrentOffset);
                        uiCurrentOffset += uiBytesToRead;
                    }

                    OnStepProgress(new ProgressEventArgs());
                }

                // Reread table 63 and make sure no new intervals have occurred while reading
                CentronTables.CopyTable(0, MeterTables, TempTables);
                CentronTables.CopyTable(1, MeterTables, TempTables);
                CentronTables.CopyTable(60, MeterTables, TempTables);
                CentronTables.CopyTable(61, MeterTables, TempTables);
                CentronTables.CopyTable(62, MeterTables, TempTables);

                do
                {
                    ReadTable(63, ref TempTables);

                    TempTables.GetValue((long)StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null, out objData);
                    usNewValidBlocks = (ushort)objData;

                    TempTables.GetValue((long)StdTableEnum.STDTBL63_NBR_VALID_INT, null, out objData);
                    usNewNumberIntervals = (ushort)objData;

                    if (usNewNumberIntervals != usNumberIntervals || usNewValidBlocks != usValidBlocks)
                    {
                        // This will limit us to only two tries at this. (if it is already true it will be set
                        // to false which means we won't try this again.)
                        bBlocksReRead = !bBlocksReRead;

                        // A new interval has occurred so we need to reread atleast one block
                        CentronTables.CopyTable(63, TempTables, MeterTables);

                        MeterTables.GetValue((long)StdTableEnum.STDTBL63_LAST_BLOCK_ELEMENT, null, out objData);
                        usNewLastBlock = (ushort)objData;

                        // Determine the offset of the block
                        iBlockToRead = (int)usNewLastBlock;
                        MeterTables.GetFieldOffset((long)StdTableEnum.STDTBL64_LP_DATA_SETS, new int[] { iBlockToRead },
                            out uiBlockOffset, out uiBlockLength);

                        PSEMResult = m_PSEM.OffsetRead(64, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(byaData);
                            MeterTables.SavePSEMStream(64, PSEMDataStream, uiBlockOffset);

                            // Now if there was also a new block we need to reread the previous block as well.
                            if (usNewValidBlocks != usValidBlocks)
                            {
                                if (usNewLastBlock - 1 < 0)
                                {
                                    iBlockToRead = usNewValidBlocks - 1;
                                }
                                else
                                {
                                    iBlockToRead = usNewLastBlock - 1;
                                }

                                // Determine the offset of the block
                                MeterTables.GetFieldOffset((long)StdTableEnum.STDTBL64_LP_DATA_SETS, new int[] { iBlockToRead },
                                    out uiBlockOffset, out uiBlockLength);

                                PSEMResult = m_PSEM.OffsetRead(64, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                                if (PSEMResult == PSEMResponse.Ok)
                                {
                                    PSEMDataStream = new MemoryStream(byaData);
                                    MeterTables.SavePSEMStream(64, PSEMDataStream, uiBlockOffset);
                                }
                            }
                        }

                        // Make sure that we save the new data to the old.
                        usValidBlocks = usNewValidBlocks;
                        usNumberIntervals = usNewNumberIntervals;
                    }
                    else // No new interval occurred
                    {
                        bBlocksReRead = false;
                    }
                } while (bBlocksReRead == true);
            }
            else
            {
                throw new Exception("Table 61 must be read prior to Table 64.");
            }
            return PSEMResult;
        }

        /// <summary>
        /// Reads Table 2152
        /// </summary>
        /// <param name="MeterTables">The tables for the meter.</param>
        /// <returns>The PSEM response for communications.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/08 RCG 1.50.24 N/A    Created

        protected PSEMResponse ReadTable2152(ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            MemoryStream PSEMDataStream;
            byte[] byaData;

            CentronTables TempTables = new CentronTables();
            uint uiMaxOffsetReadBytes;
            uint uiReadMemorySize;
            uint uiNumberOfReads;
            uint uiBytesToRead;
            uint uiCurrentOffset;
            uint uiBlockOffset;
            uint uiBlockLength;
            uint uiMaxBlocksToRead;
            uint uiMaxBytesToRead;
            ushort usValidBlocks;
            ushort usNumberIntervals;

            ushort usNewValidBlocks;
            ushort usNewNumberIntervals;
            ushort usNewLastBlock;
            int iBlockToRead;

            // This must be initialized to false or you will break the retry logic.
            bool bBlocksReRead = false;
            object objData;

            // Since Voltage Monitoring Data can be very large (144k) it may not be able
            // to be read completely when doing a full read so we need to break
            // it up into multiple offset reads. Table 2149 must be read prior to this.

            if (MeterTables.IsCached((long)CentronTblEnum.MFGTBL101_MEMORY_LEN, null) == true)
            {
                uiMaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;

                // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
                if (uiMaxOffsetReadBytes > ushort.MaxValue)
                {
                    uiMaxOffsetReadBytes = ushort.MaxValue;
                }

                MeterTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null, out objData);
                usValidBlocks = (ushort)objData;

                MeterTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_INT, null, out objData);
                usNumberIntervals = (ushort)objData;

                // Determine the size of a Voltage monitoring data block
                MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL104_VM_DATA, new int[] { 0 },
                    out uiBlockOffset, out uiBlockLength);

                // Determine how many blocks can be read in an offset read
                uiMaxBlocksToRead = uiMaxOffsetReadBytes / uiBlockLength;
                uiMaxBytesToRead = uiMaxBlocksToRead * uiBlockLength;

                // Determine total amount to read
                uiReadMemorySize = usValidBlocks * uiBlockLength;

                // Determine how many reads need to be done
                uiNumberOfReads = usValidBlocks / uiMaxBlocksToRead;

                // Add in a read for any remaining data
                if (usValidBlocks % uiMaxBlocksToRead > 0)
                {
                    uiNumberOfReads++;
                }

                uiCurrentOffset = 0;

                for (uint iIndex = 0; iIndex < uiNumberOfReads && PSEMResult == PSEMResponse.Ok; iIndex++)
                {
                    uiBytesToRead = uiReadMemorySize - uiCurrentOffset;

                    if (uiBytesToRead > uiMaxBytesToRead)
                    {
                        uiBytesToRead = uiMaxBytesToRead;
                    }

                    PSEMResult = m_PSEM.OffsetRead(2152, (int)uiCurrentOffset, (ushort)uiBytesToRead, out byaData);

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMDataStream = new MemoryStream(byaData);
                        MeterTables.SavePSEMStream(2152, PSEMDataStream, uiCurrentOffset);
                        uiCurrentOffset += uiBytesToRead;
                    }

                    OnStepProgress(new ProgressEventArgs());
                }

                // Reread table 63 and make sure no new intervals have occurred while reading
                CentronTables.CopyTable(0, MeterTables, TempTables);
                CentronTables.CopyTable(1, MeterTables, TempTables);
                CentronTables.CopyTable(2148, MeterTables, TempTables);
                CentronTables.CopyTable(2149, MeterTables, TempTables);
                CentronTables.CopyTable(2150, MeterTables, TempTables);

                do
                {
                    ReadTable(2151, ref TempTables);

                    TempTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null, out objData);
                    usNewValidBlocks = (ushort)objData;

                    TempTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_INT, null, out objData);
                    usNewNumberIntervals = (ushort)objData;

                    if (usNewNumberIntervals != usNumberIntervals || usNewValidBlocks != usValidBlocks)
                    {
                        // This will limit us to only two tries at this. (if it is already true it will be set
                        // to false which means we won't try this again.)
                        bBlocksReRead = !bBlocksReRead;

                        // A new interval has occurred so we need to reread atleast one block
                        CentronTables.CopyTable(2151, TempTables, MeterTables);

                        MeterTables.GetValue(CentronTblEnum.MFGTBL103_LAST_BLOCK_ELEMENT, null, out objData);
                        usNewLastBlock = (ushort)objData;

                        // Determine the offset of the block
                        iBlockToRead = (int)usNewLastBlock;
                        MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL104_VM_DATA, new int[] { iBlockToRead },
                            out uiBlockOffset, out uiBlockLength);

                        PSEMResult = m_PSEM.OffsetRead(2152, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                        if (PSEMResult == PSEMResponse.Ok)
                        {
                            PSEMDataStream = new MemoryStream(byaData);
                            MeterTables.SavePSEMStream(2152, PSEMDataStream, uiBlockOffset);

                            // Now if there was also a new block we need to reread the previous block as well.
                            if (usNewValidBlocks != usValidBlocks)
                            {
                                if (usNewLastBlock - 1 < 0)
                                {
                                    iBlockToRead = usNewValidBlocks - 1;
                                }
                                else
                                {
                                    iBlockToRead = usNewLastBlock - 1;
                                }

                                // Determine the offset of the block
                                MeterTables.GetFieldOffset((long)CentronTblEnum.MFGTBL104_VM_DATA, new int[] { iBlockToRead },
                                    out uiBlockOffset, out uiBlockLength);

                                PSEMResult = m_PSEM.OffsetRead(2152, (int)uiBlockOffset, (ushort)uiBlockLength, out byaData);

                                if (PSEMResult == PSEMResponse.Ok)
                                {
                                    PSEMDataStream = new MemoryStream(byaData);
                                    MeterTables.SavePSEMStream(2152, PSEMDataStream, uiBlockOffset);
                                }
                            }
                        }

                        // Make sure that we save the new data to the old.
                        usValidBlocks = usNewValidBlocks;
                        usNumberIntervals = usNewNumberIntervals;
                    }
                    else // No new interval occurred
                    {
                        bBlocksReRead = false;
                    }
                } while (bBlocksReRead == true);
            }
            else
            {
                throw new Exception("Table 2149 must be read prior to Table 2152.");
            }

            return PSEMResult;
        }

        /// <summary>
        /// Creates a list of tables to read from the meter.
        /// </summary>
        /// <returns>The list of tables to read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created
        // 05/05/08 jrf 1.50.22 114166  When network tables are not included 
        //                              tables 121 and 123 still need to be included
        //                              since they are part of the EDL configuration.
        //                              This keeps the EDL viewer from throwing an exception.
        // 05/20/08 KRC 1.50.26 115111  Remove Table 24 from EDL creation since it causes an error
        //                              in the firmware and is not needed since it is not supported.
        // 10/10/08 KRC 2.00.00         Add 2062 - C12.22 Status Table
        // 08/04/09 jrf 2.20.20 137693  Adding 2064 - Comm Module General Config Table.
        //
        protected override List<ushort> GetTablesToRead(EDLSections IncludedSections)
        {
            List<ushort> TableList = new List<ushort>();

            TableList.Add(0);       // General Configuration
            TableList.Add(1);       // Manufacturer Identification
            TableList.Add(3);       // Mode and Status
            TableList.Add(4);       // Pending Status
            TableList.Add(5);       // Device Identification
            TableList.Add(6);       // Utility Information
            TableList.Add(11);      // Actual Sources
            TableList.Add(12);      // Unit of Measure
            TableList.Add(13);      // Demand Control
            TableList.Add(14);      // Data Control
            TableList.Add(15);      // Constant
            TableList.Add(16);      // Source Definition
            TableList.Add(21);      // Actual Registers
            TableList.Add(22);      // Data selection
            TableList.Add(23);      // Current Register Data
            TableList.Add(24);      // Previous Season Data
            TableList.Add(25);      // Previous Demand Reset Data
            TableList.Add(26);      // Self Read Data
            TableList.Add(27);      // Present Register Selection
            TableList.Add(28);      // Present Register Data
            TableList.Add(51);      // Actual Time and TOU
            TableList.Add(52);      // Clock
            TableList.Add(53);      // Time offset
            TableList.Add(54);      // Calendar
            TableList.Add(55);      // Clock State

            if ((IncludedSections & EDLSections.LoadProfile) == EDLSections.LoadProfile)
            {
                TableList.Add(61);      // Actual Load Profile
                TableList.Add(62);      // Load Profile Control
                TableList.Add(63);      // Load Profile status
                TableList.Add(64);      // Load Profile data set 1
            }

            if ((IncludedSections & EDLSections.HistoryLog) == EDLSections.HistoryLog)
            {
                TableList.Add(71);      // Actual Log
                TableList.Add(72);      // Events Identification
                TableList.Add(73);      // History Logger Control
                TableList.Add(74);      // History Logger Data
                TableList.Add(75);      // Event logger control
                TableList.Add(76);      // Event logger data
            }
           
            // TableList.Add(120);     // Dim Network Table
            // TableList.Add(121);     // Actual Network Table
            TableList.Add(123);     // Exception Report Table

            if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            {
                TableList.Add(122);     // Interface Control Table
                TableList.Add(125);     // Interface Status Table
                TableList.Add(126);     // Registration Status Table
                TableList.Add(127);     // Network Statistics Table
            }

            TableList.Add(2048);    // Manufacturer Configuration Table
            TableList.Add(2053);    // Next to last demand reset snap shot 

            // TableList.Add(2062);    // C12.22 Status Table
            
            //if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            //{
            //    TableList.Add(2064);  //Comm Module General Config Table

            //    if (CommModule is RFLANCommModule)
            //    {
            //        TableList.Add(2078);    // RFLAN Neighbor Table
            //    }
            //}

            // TableList.Add(2090);    // Calendar Config

            if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            {
                TableList.Add(2098);    // HAN Dimension Limiting Table
                TableList.Add(2099);    // Actual HAN Limiting Table
                TableList.Add(2100);    // HAN Client Configuration
                TableList.Add(2102);    // HAN Transmit Data Table
                TableList.Add(2103);    // HAN Recieve Data Table
                TableList.Add(2104);    // HAN Network Info Table
            }

            // TableList.Add(2106);    // HAN Config Paramaters
            TableList.Add(2108);    // MCU Information

            // TableList.Add(2138);    // Dimension Limiting Disconnect Switch Table
            // TableList.Add(2139);    // Actual Limiting Disconnect Switch Table
            // TableList.Add(2140);    // Disconnect Switch Status Table
            // TableList.Add(2141);    // Disconnect Seitch Configuration Table
            // TableList.Add(2142);    // Disconnect Override Table
            // TableList.Add(2143);    // Service Limiting Failsafe Table

            if ((IncludedSections & EDLSections.VoltageMonitoring) == EDLSections.VoltageMonitoring)
            {
                TableList.Add(2148);    // Voltage Monitoring Dimension Limiting Table
                TableList.Add(2149);    // Voltage Monitoring Actual Limiting Table
                TableList.Add(2150);    // Voltage Monitoring Control Table
                TableList.Add(2151);    // Voltage Monitoring Status Table
                TableList.Add(2152);    // Voltage Monitoring Data Set Table 
            }

            if((IncludedSections & EDLSections.LANandHANLog) == EDLSections.LANandHANLog)
            {
                TableList.Add(2158);    // Communications Log Dimension Limiting Table
                TableList.Add(2159);    // Communications Log Actual Limiting Table
                TableList.Add(2160);    // Communications Events Identification Table
                TableList.Add(2161);    // Communications Log Control Table
                TableList.Add(2162);    // LAN Log Data Table
                TableList.Add(2163);    // HAN Communications Log Control Table
                TableList.Add(2164);    // HAN Log Data Table
            }

            TableList.Add(2168);    // Meter Swap Out Table
            TableList.Add(2190);    // Communications Config
            TableList.Add(2193);    // Enhanced Security Table
            TableList.Add(2260);    // SR 3.0 Config Table
            TableList.Add(2261);    // Fatal Error Recovery Status Table
            
            //Temporary solution to table 2262 timing out when trying to read it in non-HW3.0 meter.  
            //Accelerometer tables are only valid for HW 3.0 and up anyway.  
            if (0 <= Utilities.VersionChecker.CompareTo(HWRevision, HW_VERSION_3_0))
            {
                TableList.Add(2262);    // Tamper/Tap Status Table
                TableList.Add(2263);    // Tamper/Tap Data Table
            }
            TableList.Add(2264);    // Program State Table

            return TableList;
        }

        /// <summary>
        /// Used to determine which tables will be written to the EDL file.
        /// </summary>
        /// <param name="usTableID">Table ID to check.</param>
        /// <returns>True if the table can be written, false otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created

        protected override bool AllowTableExport(ushort usTableID)
        {
            // We are going to control the tables that are written
            // to the EDL file by the tables that we read. This
            // way we only need to change one place whenever new
            // tables are added or removed.
            return true;
        }

        /// <summary>
        /// Determines which fields may be written to the EDL file.
        /// </summary>
        /// <param name="idElement">The field to check.</param>
        /// <param name="anIndex">The indexes into the field.</param>
        /// <returns>True if the field may  be written to the EDL file. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/19/06 RCG	7.40.00			Created

        protected override bool AllowFieldExport(long idElement, int[] anIndex)
        {
            bool bAllowExport = false;

            // Currently there are no fields that we wish to exclude
            switch(idElement)
            {
                // These items are in 2048 but are also stored elsewhere
                // so we do not need to export them twice
                case (long)CentronTblEnum.MFGTBL0_UNKNOWN_BLOCK:
                case (long)CentronTblEnum.MFGTBL0_DECADE_0:
                case (long) CentronTblEnum.MFGTBL0_DECADE_8:
                {
                    bAllowExport = false;
                    break;
                }
                default:
                {
                    bAllowExport = true;
                    break;
                }
            }

            return bAllowExport;
        }

        #endregion
    }
}
