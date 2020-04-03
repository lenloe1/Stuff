///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
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
//                           Copyright © 2006 - 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    public partial class CENTRON_AMI
    {
        #region Constants

        private const int LP_READ_RETRIES = 3;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the load profile data between the specified dates.
        /// </summary>
        /// <param name="startDate">The start date of the load profile data to get.</param>
        /// <param name="endDate">The end date of the load profile data to get.</param>
        /// <returns>The load profile data from the dates specified.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified to make it easier to add support for 
        //                              Instrumentation Profile and Extended Load Profile

        public LoadProfileData GetLoadProfileData(DateTime startDate, DateTime endDate)
        {
            return GetProfileData(startDate, endDate, Table61, Table62, Table63, Table64);
        }

        /// <summary>
        /// Gets the full load profile data object from the meter.
        /// </summary>
        /// <returns>The load profile data.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created
        // 11/06/08 AF  2.00.04        Changed the number of steps in the progress bar

        public LoadProfileData GetLoadProfileData()
        {
            return GetProfileData(Table61, Table62, Table63, Table64);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The number of valid load profile blocks in LP set 1.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public ushort NumberOfValidLoadProfileBlocks
        {
            get
            {
                return Table63.Set1StatusRecord.NumberOfValidBlocks;
            }
        }

        /// <summary>
        /// The maximum number of blocks that can be used in LP set 1.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public ushort MaximumNumberOfLoadProfileBlocks
        {
            get
            {
                return Table61.Set1ActualLimits.NumberOfBlocks;
            }
        }

        /// <summary>
        /// The index of the last block in LP set 1.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public ushort LastLoadProfileBlockIndex
        {
            get
            {
                return Table63.Set1StatusRecord.LastBlockElement;
            }
        }

        /// <summary>
        /// The index of the first block in LP set 1.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        public int FirstLoadProfileBlockIndex
        {
            get
            {
                LPSetStatusRecord StatusRecord;
                LPBlockDataRecord FirstBlock = GetFirstBlock(Table61, Table63, Table64, out StatusRecord);

                return DetermineStartBlockIndex(Table61.Set1ActualLimits, Table63.Set1StatusRecord, FirstBlock, DateProgrammed);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Get the all of the Profile Data from the specified set of tables
        /// </summary>
        /// <param name="actualLimitingTable">The actual limiting table for the data set</param>
        /// <param name="controlTable">The control table for the data set</param>
        /// <param name="statusTable">The status table for the data set</param>
        /// <param name="dataSetTable">The data set table for the data set</param>
        /// <returns>The profile data that has been read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created
        // 06/15/12 jrf 2.60.32 199972 Adding logging statements to help debug issue next time we see it.
        //
        protected LoadProfileData GetProfileData(StdTable61 actualLimitingTable, StdTable62 controlTable, StdTable63 statusTable, StdTable64 dataSetTable)
        {
            LoadProfileData LPData = null;
            LPSetActualLimits SetLimits = actualLimitingTable.GetSetLimits(dataSetTable.DataSet);
            LPSetDataSelection SetDataSelection = controlTable.GetDataSelection(dataSetTable.DataSet);
            LPBlockDataRecord[] Blocks;
            LPBlockDataRecord FirstBlock;
            LPBlockDataRecord LastBlock;
            LPSetStatusRecord SetStatus;
            ushort NumberOfBlocks;
            ushort FirstBlockIndex;

            m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Getting Profile Data");
            SetStatus = statusTable.GetSetStatusRecord(dataSetTable.DataSet);

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Number of Blocks: " + SetStatus.NumberOfValidBlocks.ToString(CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Number of Intervals: " + SetStatus.NumberOfValidIntervals.ToString(CultureInfo.InvariantCulture));
            if (SetStatus.NumberOfValidBlocks > 0 && SetStatus.NumberOfValidIntervals > 0)
            {
                OnShowProgress(new ShowProgressEventArgs(1, SetStatus.NumberOfValidBlocks, "Reading Load Profile data...", "Reading Load Profile data..."));

                // Read the first and last blocks
                GetFirstAndLastBlock(actualLimitingTable, statusTable, dataSetTable, out FirstBlock, out LastBlock, out SetStatus);

                // Read the rest of the blocks using the last block status
                NumberOfBlocks = SetStatus.NumberOfValidBlocks;
                Blocks = new LPBlockDataRecord[NumberOfBlocks];

                Blocks[0] = FirstBlock;

                if (NumberOfBlocks > 1)
                {
                    Blocks[NumberOfBlocks - 1] = LastBlock;

                    FirstBlockIndex = (ushort)((SetStatus.LastBlockElement + 1) % NumberOfBlocks);

                    for (ushort RelativeBlockIndex = 1; RelativeBlockIndex < NumberOfBlocks - 1; RelativeBlockIndex++)
                    {
                        ushort ActualBlockIndex = (ushort)((FirstBlockIndex + RelativeBlockIndex) % NumberOfBlocks);

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Block # " + ActualBlockIndex.ToString(CultureInfo.InvariantCulture));
                        Blocks[RelativeBlockIndex] = ReadLPBlock(dataSetTable, ActualBlockIndex, SetLimits.IntervalsPerBlock);

                        OnStepProgress(new ProgressEventArgs());
                    }

                }

                OnHideProgress(new EventArgs());

                // Create the LoadProfileData object.
                LPData = CreateLoadProfileDataObject(Blocks, SetLimits, SetDataSelection);
            }

            return LPData;
        }

        /// <summary>
        /// Gets the load profile data between the specified dates.
        /// </summary>
        /// <param name="startDate">The start date of the load profile data to get.</param>
        /// <param name="endDate">The end date of the load profile data to get.</param>
        /// <param name="actualLimitingTable">The actual limiting table for the data set</param>
        /// <param name="controlTable">The control table for the data set</param>
        /// <param name="statusTable">The status table for the data set</param>
        /// <param name="dataSetTable">The data set table for the data set</param>
        /// <returns>The load profile data from the dates specified.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/11 RCG 2.53.20 N/A    Created

        public LoadProfileData GetProfileData(DateTime startDate, DateTime endDate, StdTable61 actualLimitingTable, StdTable62 controlTable, StdTable63 statusTable, StdTable64 dataSetTable)
        {
            LPSetActualLimits SetLimits = actualLimitingTable.GetSetLimits(dataSetTable.DataSet);
            LPBlockDataRecord FirstBlock;
            LPBlockDataRecord LastBlock;
            LPBlockDataRecord[] Blocks;
            LPSetStatusRecord SetStatus;
            LoadProfileData LPData = null;

            int StartBlockIndex;
            int EndBlockIndex;
            int FirstBlockIndex;

            SetStatus = statusTable.GetSetStatusRecord(dataSetTable.DataSet);

            if (SetStatus != null)
            {

                if (SetStatus.NumberOfValidBlocks > 1 && SetStatus.NumberOfValidIntervals > 0)
                {
                    OnShowProgress(new ShowProgressEventArgs(1, 2, "Determining blocks to read...", "Determining blocks to read..."));

                    // Get the first and last blocks in order to determine the blocks we need to read.
                    GetFirstAndLastBlock(actualLimitingTable, statusTable, dataSetTable, out FirstBlock, out LastBlock, out SetStatus);

                    OnStepProgress(new ProgressEventArgs());

                    // Determine which blocks to read
                    StartBlockIndex = DetermineStartBlockIndex(SetLimits, SetStatus, FirstBlock, startDate);
                    EndBlockIndex = DetermineEndBlockIndex(SetLimits, SetStatus, LastBlock, endDate);

                    Blocks = new LPBlockDataRecord[EndBlockIndex - StartBlockIndex + 1];
                    FirstBlockIndex = (SetStatus.LastBlockElement + 1) % SetStatus.NumberOfValidBlocks;

                    OnHideProgress(new EventArgs());
                    OnShowProgress(new ShowProgressEventArgs(1, EndBlockIndex - StartBlockIndex, "Reading Load Profile data...", "Reading Load Profile data..."));

                    // Read the blocks
                    for (int RelativeBlockIndex = StartBlockIndex; RelativeBlockIndex <= EndBlockIndex; RelativeBlockIndex++)
                    {
                        int BlockArrayIndex = RelativeBlockIndex - StartBlockIndex;

                        OnStepProgress(new ProgressEventArgs());

                        // We already have the first and last blocks so just add those if included.
                        if (RelativeBlockIndex == 0)
                        {
                            Blocks[BlockArrayIndex] = FirstBlock;
                        }
                        else if (RelativeBlockIndex == SetStatus.NumberOfValidBlocks - 1)
                        {
                            // The last block
                            Blocks[BlockArrayIndex] = LastBlock;
                        }
                        else
                        {
                            // We need to read the block
                            ushort ActualBlockIndex = (ushort)((FirstBlockIndex + RelativeBlockIndex) % SetStatus.NumberOfValidBlocks);

                            Blocks[BlockArrayIndex] = ReadLPBlock(dataSetTable, ActualBlockIndex, SetLimits.IntervalsPerBlock);
                        }

                    }

                    OnStepProgress(new ProgressEventArgs("Creating Load Profile object..."));

                    // Create the LoadProfileData object.
                    LPData = CreateLoadProfileDataObject(Blocks, SetLimits, controlTable.GetDataSelection(dataSetTable.DataSet));

                    OnStepProgress(new ProgressEventArgs("Removing additional intervals..."));

                    // Trim out intervals that were not requested.
                    LPData.Intervals.RemoveAll(delegate(LPInterval interval) { return interval.Time < startDate || interval.Time > endDate; });

                    OnHideProgress(new EventArgs());
                }
                else if (SetStatus.NumberOfValidBlocks == 1)
                {
                    OnShowProgress(new ShowProgressEventArgs(1, 3, "Reading Load Profile data...", "Reading Load Profile data..."));

                    // Just get the first block the trim will take care of anything outside the range
                    FirstBlock = GetFirstBlock(actualLimitingTable, statusTable, dataSetTable, out SetStatus);

                    OnStepProgress(new ProgressEventArgs());

                    LPData = CreateLoadProfileDataObject(new LPBlockDataRecord[] { FirstBlock }, SetLimits, controlTable.GetDataSelection(dataSetTable.DataSet));

                    OnStepProgress(new ProgressEventArgs());

                    // Trim out intervals that were not requested.
                    LPData.Intervals.RemoveAll(delegate(LPInterval interval) { return interval.Time < startDate || interval.Time > endDate; });

                    OnStepProgress(new ProgressEventArgs());
                    OnHideProgress(new EventArgs());
                }
            }

            return LPData;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the relative index of the block that contains the start date specified.
        /// </summary>
        /// <param name="setLimits">The actual limits for the data set.</param>
        /// <param name="setStatus">The status of the data set when last block was read.</param>
        /// <param name="firstBlock">The last block in the data set.</param>
        /// <param name="startDate">The start date that is being searched for.</param>
        /// <returns>The relative index of the block that contains the start date.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/08/08 RCG 2.00.00 N/A    Created

        private int DetermineStartBlockIndex(LPSetActualLimits setLimits, LPSetStatusRecord setStatus, LPBlockDataRecord firstBlock, DateTime startDate)
        {
            int StartBlockIndex = 0;
            bool IncludeFirstBlock;

            IncludeFirstBlock = startDate <= firstBlock.BlockEndTime;

            if (!IncludeFirstBlock)
            {
                // We need to determine the starting block 
                for (int iBlock = 1; iBlock < setStatus.NumberOfValidBlocks; iBlock++)
                {
                    DateTime BlockEndTime = (DateTime)firstBlock.BlockEndTime;

                    BlockEndTime = BlockEndTime.AddMinutes(iBlock * setLimits.IntervalsPerBlock * setLimits.IntervalLength);

                    if (BlockEndTime >= startDate)
                    {
                        StartBlockIndex = iBlock;
                        break;
                    }
                }
            }
            else
            {
                StartBlockIndex = 0;
            }

            return StartBlockIndex;
        }

        /// <summary>
        /// Determines the relative index of the block that contains the end date specified.
        /// </summary>
        /// <param name="setLimits">The actual limits for the data set.</param>
        /// <param name="setStatus">The status of the data set when last block was read.</param>
        /// <param name="lastBlock">The last block in the data set.</param>
        /// <param name="endDate">The end date that is being searched for.</param>
        /// <returns>The relative index of the block that contains the end time.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/08/08 RCG 2.00.00 N/A    Created
        // 09/22/09 AF  2.30.02        Corrected the equation for determining last block

        private int DetermineEndBlockIndex(LPSetActualLimits setLimits, LPSetStatusRecord setStatus, LPBlockDataRecord lastBlock, DateTime endDate)
        {
            int EndBlockIndex = 0;
            DateTime LastBlockStartTime;
            bool IncludeLastBlock;

            LastBlockStartTime = DetermineIntervalTime(lastBlock, 0, setLimits.IntervalLength);

            if (lastBlock.Intervals.Length > 1)
            {
                LastBlockStartTime = AdjustTimeForDST(LastBlockStartTime, lastBlock.Intervals[0],
                    lastBlock.Intervals[1], lastBlock.Intervals[setStatus.NumberOfValidIntervals - 1]);
            }

            IncludeLastBlock = endDate >= LastBlockStartTime;

            if (!IncludeLastBlock)
            {
                // We need to determine the last block. Start at the next to last block
                for (int iBlock = setStatus.NumberOfValidBlocks - 2; iBlock >= 1; iBlock--)
                {
                    DateTime BlockStartTime = LastBlockStartTime;

                    BlockStartTime = BlockStartTime.AddMinutes(-1 * (setStatus.NumberOfValidBlocks - (iBlock + 1)) * setLimits.IntervalsPerBlock * setLimits.IntervalLength);

                    if (BlockStartTime <= endDate)
                    {
                        EndBlockIndex = iBlock;
                        break;
                    }
                }
            }
            else
            {
                EndBlockIndex = setStatus.NumberOfValidBlocks - 1;
            }

            return EndBlockIndex;
        }

        /// <summary>
        /// Gets the first and last blocks of the load profile data.
        /// </summary>
        /// <param name="actualLimitingTable">The limiting table for the data set</param>
        /// <param name="statusTable">The status table for the data set</param>
        /// <param name="dataSetTable">The set to get the blocks from.</param>
        /// <param name="firstBlock">The first block.</param>
        /// <param name="lastBlock">The last block.</param>
        /// <param name="setStatus">The set status of the load profile data at the time of read.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/07/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support
        // 06/15/12 jrf 2.60.32 199972 Adding logging statements to help debug issue next time we see it.
        //
        private void GetFirstAndLastBlock(StdTable61 actualLimitingTable, StdTable63 statusTable, StdTable64 dataSetTable, out LPBlockDataRecord firstBlock, out LPBlockDataRecord lastBlock, out LPSetStatusRecord setStatus)
        {
            LPSetStatusRecord FirstBlockStatus;
            LPSetStatusRecord LastBlockStatus;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read First Block");
            firstBlock = GetFirstBlock(actualLimitingTable, statusTable, dataSetTable, out FirstBlockStatus);

            OnStepProgress(new ProgressEventArgs());

            if (FirstBlockStatus.NumberOfValidBlocks > 1)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Last Block");
                lastBlock = GetLastBlock(statusTable, dataSetTable, out LastBlockStatus);

                OnStepProgress(new ProgressEventArgs());

                // Make sure the first block is still valid.
                if (HasLastBlockRolledOver(FirstBlockStatus, LastBlockStatus))
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read First Block, Again");
                    firstBlock = GetFirstBlock(actualLimitingTable, statusTable, dataSetTable, out FirstBlockStatus);
                }

                // At this point we do not need to check the last block again as we should never
                // have multiple block rollovers in such a short period of time. We will use the 
                // last blocks status in order to ensure the values match the last block.
                setStatus = LastBlockStatus;
            }
            else
            {
                // We only have one block so we should be ok.
                OnStepProgress(new ProgressEventArgs());

                lastBlock = null;
                setStatus = FirstBlockStatus;
            }
        }

        /// <summary>
        /// Gets the last block of Load Profile data.
        /// </summary>
        /// <param name="statusTable">The status table for the data set</param>
        /// <param name="dataSetTable">The data set to get the last block of.</param>
        /// <param name="setStatus">The set status at the time of reading.</param>
        /// <returns>The last load profile block.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/07/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        private LPBlockDataRecord GetLastBlock(StdTable63 statusTable, StdTable64 dataSetTable, out LPSetStatusRecord setStatus)
        {
            LPBlockDataRecord LastBlock = null;
            LPSetStatusRecord SetStatusBeforeRead = null;
            LPSetStatusRecord SetStatusAfterRead = null;
            int BlockReadRetries = 0;

            do
            {
                SetStatusBeforeRead = statusTable.GetSetStatusRecord(dataSetTable.DataSet);

                LastBlock = ReadLPBlock(dataSetTable, SetStatusBeforeRead.LastBlockElement, SetStatusBeforeRead.NumberOfValidIntervals);

                SetStatusAfterRead = statusTable.GetSetStatusRecord(dataSetTable.DataSet);
            }
            while (BlockReadRetries < LP_READ_RETRIES && (HasLastBlockRolledOver(SetStatusBeforeRead, SetStatusAfterRead)
                    || HasNewIntervalOccured(SetStatusBeforeRead, SetStatusAfterRead)));

            setStatus = SetStatusAfterRead;

            return LastBlock;
        }


        /// <summary>
        /// Gets the first block of the load profile data
        /// </summary>
        /// <param name="actualLimitingTable">The actual limiting table that applies to the data set</param>
        /// <param name="statusTable">The status table that applies to the data set</param>
        /// <param name="dataSetTable">The data set</param>
        /// <param name="setStatus">The status record for the data set at the time of the read</param>
        /// <returns>The first block of Load Profile Data</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/07/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        private LPBlockDataRecord GetFirstBlock(StdTable61 actualLimitingTable, StdTable63 statusTable, StdTable64 dataSetTable, out LPSetStatusRecord setStatus)
        {
            LPBlockDataRecord FirstBlock = null;
            LPSetActualLimits SetLimits = actualLimitingTable.GetSetLimits(dataSetTable.DataSet);
            LPSetStatusRecord SetStatusBeforeRead = null;
            LPSetStatusRecord SetStatusAfterRead = null;
            int BlockReadRetries = 0;
            ushort FirsBlockIndex = 0;

            do
            {
                SetStatusBeforeRead = statusTable.GetSetStatusRecord(dataSetTable.DataSet);

                if (SetStatusBeforeRead.DataListType == LPSetStatusRecord.ListType.Circular)
                {
                    FirsBlockIndex = (ushort)((SetStatusBeforeRead.LastBlockElement + 1) % SetStatusBeforeRead.NumberOfValidBlocks);
                }

                if (SetStatusBeforeRead.NumberOfValidBlocks > 1)
                {
                    FirstBlock = ReadLPBlock(dataSetTable, FirsBlockIndex, SetLimits.IntervalsPerBlock);
                }
                else
                {
                    // The first block is the last block so we need to use the number of valid intervals
                    FirstBlock = ReadLPBlock(dataSetTable, FirsBlockIndex, SetStatusBeforeRead.NumberOfValidIntervals);
                }

                SetStatusAfterRead = statusTable.GetSetStatusRecord(dataSetTable.DataSet);
            }
            while (BlockReadRetries < LP_READ_RETRIES && HasLastBlockRolledOver(SetStatusBeforeRead, SetStatusAfterRead));

            setStatus = SetStatusAfterRead;

            return FirstBlock;
        }

        /// <summary>
        /// Determines if a new interval has occurred since the initial reading.
        /// </summary>
        /// <param name="initialSetStatus">The initial set status for the load profile data.</param>
        /// <param name="currentSetStatus">The current set status for the load profile data.</param>
        /// <returns>True if a new interval has occurred. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/07/08 RCG 2.00.00 N/A    Created

        private bool HasNewIntervalOccured(LPSetStatusRecord initialSetStatus, LPSetStatusRecord currentSetStatus)
        {
            return initialSetStatus.NumberOfValidIntervals != currentSetStatus.NumberOfValidIntervals;
        }

        /// <summary>
        /// Determines if the last block has rolled over in the data set.
        /// </summary>
        /// <param name="initialSetStatus">The initial set status for the data set.</param>
        /// <param name="currentSetStatus">The new set status for the data set.</param>
        /// <returns>True if the last block has rolled over since the initial status. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/07/08 RCG 2.00.00 N/A    Created

        private bool HasLastBlockRolledOver(LPSetStatusRecord initialSetStatus, LPSetStatusRecord currentSetStatus)
        {
            bool bHasBlockIndexChanged = initialSetStatus.LastBlockElement != currentSetStatus.LastBlockElement;
            bool bHasIntervalCountBeenReset = initialSetStatus.NumberOfValidIntervals > currentSetStatus.NumberOfValidIntervals;

            // In the case of a FIFO list we also need to make sure that the number of valid intervals has not been reset.
            return bHasBlockIndexChanged || bHasIntervalCountBeenReset;
        }

        /// <summary>
        /// Reads a block from the specified data set.
        /// </summary>
        /// <param name="dataSetTable">The data set to read from.</param>
        /// <param name="blockToRead">The block index for the block to read.</param>
        /// <param name="validIntervals">The number of valid intervals in the block</param>
        /// <returns>The block.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/07/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        private LPBlockDataRecord ReadLPBlock(StdTable64 dataSetTable, ushort blockToRead, ushort validIntervals)
        {
            PSEMResponse Response = PSEMResponse.Ok;
            LPBlockDataRecord Block = null;

            Response = dataSetTable.ReadBlock(blockToRead, validIntervals, out Block);

            // Handle any communication errors.
            if (Response != PSEMResponse.Ok)
            {
                throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                    "Error reading Load Profile block " + blockToRead.ToString(CultureInfo.CurrentCulture));
            }

            return Block;
        }

        /// <summary>
        /// Creates the LoadProfileData object from the specified blocks.
        /// </summary>
        /// <param name="loadProfileBlocks">The list of blocks to use to create the object ordered by date.</param>
        /// <param name="setLimits">The set limits for the data set</param>
        /// <param name="setDataSelection">The data selection for the data set</param>
        /// <returns>The LoadProfileData object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        private LoadProfileData CreateLoadProfileDataObject(LPBlockDataRecord[] loadProfileBlocks, LPSetActualLimits setLimits, LPSetDataSelection setDataSelection)
        {
            LoadProfileData LPData = new LoadProfilePulseData(setLimits.IntervalLength);

            AddChannels(ref LPData, setLimits, setDataSelection);
            AddIntervals(ref LPData, loadProfileBlocks);

            return LPData;
        }

        /// <summary>
        /// Adds the channels to the LoadProfilData object.
        /// </summary>
        /// <param name="loadProfileData">The LoadProfileData object to add the channels to.</param>
        /// <param name="setLimits">The set limits for the data set</param>
        /// <param name="setDataSelection">The data selection for the data set</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        private void AddChannels(ref LoadProfileData loadProfileData, LPSetActualLimits setLimits, LPSetDataSelection setDataSelection)
        {
            List<string> ChannelNames = DetermineChannelNames(setDataSelection);
            List<float> PulseWeights = DeterminePulseWeights(setLimits, setDataSelection);

            for (int iChannel = 0; iChannel < setLimits.NumberOfChannels; iChannel++)
            {
                loadProfileData.AddChannel(ChannelNames[iChannel], PulseWeights[iChannel], 1.0f);
            }
        }

        /// <summary>
        /// Adds all of the intervals in the specified blocks.
        /// </summary>
        /// <param name="loadProfileData">The load profile object to add the intervals to.</param>
        /// <param name="loadProfileBlocks">The load profile blocks to add.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/06/08 RCG 2.00.00 N/A    Created

        private void AddIntervals(ref LoadProfileData loadProfileData, LPBlockDataRecord[] loadProfileBlocks)
        {
            for (int iBlock = 0; iBlock < loadProfileBlocks.Length; iBlock++)
            {
                LPBlockDataRecord CurrentBlock = loadProfileBlocks[iBlock];
                LPIntervalDataRecord LastInterval = CurrentBlock.Intervals[CurrentBlock.Intervals.Length - 1];

                for (int iInterval = 0; iInterval < CurrentBlock.Intervals.Length; iInterval++)
                {
                    LPIntervalDataRecord CurrentInterval = CurrentBlock.Intervals[iInterval];
                    DateTime IntervalTime;

                    if (iInterval + 1 < CurrentBlock.Intervals.Length)
                    {
                        // Figure out the time of the interval.
                        IntervalTime = DetermineIntervalTime(CurrentBlock, iInterval, loadProfileData.IntervalDuration);
                        IntervalTime = AdjustTimeForDST(IntervalTime, CurrentInterval, CurrentBlock.Intervals[iInterval + 1], LastInterval);
                    }
                    else
                    {
                        // We already know the time of the last interval.
                        IntervalTime = (DateTime)CurrentBlock.BlockEndTime;
                    }

                    loadProfileData.AddInterval(CurrentInterval.IntervalData,
                                                ConvertChannelStatuses(CurrentInterval),
                                                ConvertIntervalStatus(CurrentInterval),
                                                IntervalTime,
                                                DisplayScaleOptions.UNITS);
                }
            }
        }

        /// <summary>
        /// Converts the channel statuses to string format.
        /// </summary>
        /// <param name="interval">The interval to convert.</param>
        /// <returns>The channel status as strings.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/06/08 RCG 2.00.00 N/A    Created
        // 03/09/11 RCG 2.50.08        Adding Power Restoration status

        private string[] ConvertChannelStatuses(LPIntervalDataRecord interval)
        {
            string[] ChannelStatuses = new string[interval.ChannelStatuses.Length];

            for (int iChannel = 0; iChannel < interval.ChannelStatuses.Length; iChannel++)
            {
                ExtendedChannelStatus CurrentStatus = interval.ChannelStatuses[iChannel];
                string strStatus = "";

                switch (CurrentStatus)
                {
                    case ExtendedChannelStatus.Overflow:
                    {
                        strStatus = "V";
                        break;
                    }
                    case ExtendedChannelStatus.Partial:
                    {
                        strStatus = "S";
                        break;
                    }
                    case ExtendedChannelStatus.Long:
                    {
                        strStatus = "L";
                        break;
                    }
                    case ExtendedChannelStatus.Skipped:
                    {
                        strStatus = "K";
                        break;
                    }
                    case ExtendedChannelStatus.Test:
                    {
                        strStatus = "T";
                        break;
                    }
                    case ExtendedChannelStatus.PowerRestoration:
                    {
                        strStatus = "SR";
                        break;
                    }
                }

                ChannelStatuses[iChannel] = strStatus;
            }

            return ChannelStatuses;
        }

        /// <summary>
        /// Converts the interval status to string format.
        /// </summary>
        /// <param name="CurrentInterval">The interval to convert.</param>
        /// <returns>The interval status in string format.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/06/08 RCG 2.00.00 N/A    Created
        // 02/19/09 jrf 2.10.05 127954 Making change to time adjust interval statuses
        //                             to be consistent with changes being made to firmware.

        private string ConvertIntervalStatus(LPIntervalDataRecord CurrentInterval)
        {
            string strStatus = "";
            ExtendedIntervalStatus CurrentStatus = (ExtendedIntervalStatus)CurrentInterval.IntervalStatus;

            if ((CurrentStatus & ExtendedIntervalStatus.DSTChange) == ExtendedIntervalStatus.DSTChange)
            {
                strStatus += "D";
            }

            if ((CurrentStatus & ExtendedIntervalStatus.PowerFailure) == ExtendedIntervalStatus.PowerFailure)
            {
                strStatus += "O";
            }

            //Only show one A for either a backwards or forwards time adjust.
            if ((CurrentStatus & ExtendedIntervalStatus.ClockAdjustForward) == ExtendedIntervalStatus.ClockAdjustForward)
            {
                strStatus += "A";
            }
            else if ((CurrentStatus & ExtendedIntervalStatus.ClockAdjustBackward) == ExtendedIntervalStatus.ClockAdjustBackward)
            {
                strStatus += "A";
            }

            return strStatus;
        }

        /// <summary>
        /// Determines the time of the interval but does not account for DST changes.
        /// </summary>
        /// <param name="block">The current block.</param>
        /// <param name="intervalIndex">The index of the interval in the block.</param>
        /// <param name="intervalLength">The length of one interval</param>
        /// <returns>The date and time of the interval.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/06/08 RCG 2.00.00 N/A    Created

        private DateTime DetermineIntervalTime(LPBlockDataRecord block, int intervalIndex, int intervalLength)
        {
            DateTime IntervalTime = (DateTime)block.BlockEndTime;

            IntervalTime = IntervalTime.AddMinutes(-1 * (block.Intervals.Length - intervalIndex - 1) * intervalLength);

            return IntervalTime;
        }

        /// <summary>
        /// Adjusts the time for DST as needed.
        /// </summary>
        /// <param name="intervalTime">The raw time for the current interval.</param>
        /// <param name="currentInterval">The current interval object.</param>
        /// <param name="nextInterval">The next interval object.</param>
        /// <param name="lastInterval">The last interval object.</param>
        /// <returns>The adjusted time.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#          Description
        // -------- --- ------- -------------  ---------------------------------------
        // 10/06/08 RCG 2.00.00 N/A             Created
        // 01/07/09 KRC 2.10.01 itron00124215   Fixed issues with DST time being off
        //
        private DateTime AdjustTimeForDST(DateTime intervalTime, LPIntervalDataRecord currentInterval, LPIntervalDataRecord nextInterval, LPIntervalDataRecord lastInterval)
        {
            DateTime AdjustedTime = intervalTime;

            // Only adjust if DST is enabled in the meter.
            if (DSTEnabled == true)
            {
                // There are two case where we need to adjust. The first and most common is when the 
                // current interval's flag does not match the last interval's flag.
                if (IsDSTFlagSet(currentInterval) == true && IsDSTFlagSet(lastInterval) == false)
                {
                    // Adjust the time forward
                    AdjustedTime = AdjustedTime.Add(Table53.DSTAdjustAmount);
                }
                else if (IsDSTFlagSet(currentInterval) == false && IsDSTFlagSet(lastInterval) == true)
                {
                    // Adjust the time backward
                    AdjustedTime = AdjustedTime.Subtract(Table53.DSTAdjustAmount);
                }

                // The second is when the current interval's flag does not match the next since the interval
                // ending at the DST change still has the previous flag.
                if (nextInterval != null && IsDSTFlagSet(currentInterval) == true && IsDSTFlagSet(nextInterval) == false)
                {
                    // Adjust backwards since the current interval end time is really no longer in DST
                    AdjustedTime = AdjustedTime.Subtract(Table53.DSTAdjustAmount);
                }
                else if (nextInterval != null && IsDSTFlagSet(currentInterval) == false && IsDSTFlagSet(nextInterval) == true)
                {
                    // Adjust forwards since the current interval end time is really in DST
                    AdjustedTime = AdjustedTime.Add(Table53.DSTAdjustAmount);
                }
            }

            return AdjustedTime;
        }

        /// <summary>
        /// Gets whether or not the DST flag is set for the specified interval
        /// </summary>
        /// <param name="interval">The interval to check.</param>
        /// <returns>True if the DST flag is set. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created

        private bool IsDSTFlagSet(LPIntervalDataRecord interval)
        {
            return (interval.IntervalStatus & ExtendedIntervalStatus.DSTChange) == ExtendedIntervalStatus.DSTChange;
        }

        /// <summary>
        /// Determines the channel name for each of the channels.
        /// </summary>
        /// <param name="setDataSelection">The data set to determine the names for.</param>
        /// <returns>The list of channel names.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        private List<string> DetermineChannelNames(LPSetDataSelection setDataSelection)
        {
            List<string> ChannelNames = new List<string>();

            foreach (uint CurrentLID in setDataSelection.SourceLIDs)
            {
                uint ModifiedLID = CurrentLID;

                if ((CurrentLID & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK) == (uint)DefinedLIDs.BaseLIDs.ENERGY_DATA)
                {
                    // The source LID numbers are for the raw values we need them to be secondary for this
                    // to work correctly.
                    ModifiedLID = (CurrentLID & (uint)DefinedLIDs.WhichEnergyFormat.WHICH_FORMAT_MASK_OUT) | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA;
                }

                LID ChannelLid = CreateLID(ModifiedLID);
                ChannelNames.Add(ChannelLid.lidDescription);
            }

            return ChannelNames;
        }

        /// <summary>
        /// Determines the pulse weights for each channel.
        /// </summary>
        /// <param name="setLimits">The set limits for the data set</param>
        /// <param name="setDataSelection">The data selection for the data set</param>
        /// <returns>The list of pulse weights.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/03/08 RCG 2.00.00 N/A    Created
        // 12/12/11 RCG 2.53.20        Modified for Extended LP and IP support

        private List<float> DeterminePulseWeights(LPSetActualLimits setLimits, LPSetDataSelection setDataSelection)
        {
            List<float> PulseWeights = new List<float>();

            if (setDataSelection != null && setLimits.IncludeScalarDivisor)
            {
                // We have all the data we need to determine the pulse weights
                for (int iChannel = 0; iChannel < setLimits.NumberOfChannels; iChannel++)
                {
                    // Make sure that we get a float value.
                    PulseWeights.Add((float)setDataSelection.Scalars[iChannel] / (float)setDataSelection.Divisors[iChannel]);
                }
            }
            else
            {
                // We don't have the data we need so just use 1.0 so that the raw data is used.
                for (int iChannel = 0; iChannel < setLimits.NumberOfChannels; iChannel++)
                {
                    PulseWeights.Add(1.0f);
                }
            }

            return PulseWeights;
        }

        #endregion
    }
}
