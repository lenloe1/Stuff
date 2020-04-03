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
//                              Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    public partial class ICS_Gateway : IFirmwareDownload
    {
        #region Public Methods

        /// <summary>
        /// Downloads the firmware file to the meter and activates it.    
        /// The activation will cause the meter to drop the psem task so 
        /// meter log off must follow this function call
        /// </summary>
        /// <param name="path">Complete file path of the firmware file</param>
        /// <returns>Itron.Metering.Device.FWDownloadResult</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 07/08/13 jrf 2.80.51 TC 13201  Refactored to support retries of FWDL. Moved comments to refactored method.
        // 11/15/13 AF  3.50.04	        Class re-architecture - cloned from CENTRON_AMI
        // 
        public FWDownloadResult DownloadFW(string path)
        {
            ushort usBlockIndex = 0;

            return DownloadFW(path, ref usBlockIndex);
        }

        /// <summary>
        /// Downloads the firmware file to the meter and activates it.    
        /// The activation will cause the meter to drop the psem task so meter log off must
        /// follow this function call on success.  This method supports resuming
        /// a previous failed FWDL.
        /// </summary>
        /// <param name="path">Complete file path of the firmware file</param>
        /// <param name="usBlockIndex">Dual purpose parameter. The passed in value indicates 
        /// which block to begin downloading. The passed out parameter indicates which block to
        /// resume downloading in case there was a failure. This can then passed in again to 
        /// restart the download at the point where it left off.</param>
        /// <param name="blnRetry">Whether or not to leave the FWDL in a state
        /// to permit subsequent retries at point of faliure. If false the pending table 
        /// will be cleared on failure.</param>
        /// <param name="blnActivate">Whether or not to activate the firmware.</param>
        /// <returns>Itron.Metering.Device.FWDownloadResult</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 11/15/13 AF  3.50.03	            Class re-architecture - Cloned from CENTRON_AMI
        // 
        public FWDownloadResult DownloadFW(string path, ref ushort usBlockIndex, bool blnRetry = false, bool blnActivate = true)
        {
            FWDownloadResult Result = FWDownloadResult.UNKNOWN_DRIVER_ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)FWDLTableIds.CommModuleFWTbl | PENDING_BIT;
            ushort usNumberChunks = 0;
            //A non-zero starting block means we are need to pick up were we left off.
            bool blnResumeFWDL = (0 != usBlockIndex);

            System.IO.FileStream streamFile;
            System.IO.MemoryStream streamHeader = new System.IO.MemoryStream();
            System.IO.MemoryStream streamPSEM = new System.IO.MemoryStream();

            try
            {
                if (true == blnResumeFWDL)
                {
                    Result = FWDownloadResult.SUCCESS;
                }
                else
                {
                    // Tell the meter to enter firmware download mode
                    Result = EnterFirmwareDownloadMode(path);
                }

                if (FWDownloadResult.SUCCESS != Result)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Initiate F/W Download procedure failed with result = "
                            + Result);
                }
                else
                {
                    // Meter is ready to receive the firmware file
                    streamFile = new System.IO.FileStream(path, System.IO.FileMode.Open, FileAccess.Read);
                    byte[] bybuffer = new byte[streamFile.Length];

                    streamFile.Read(bybuffer, 0, (int)streamFile.Length);
                    streamFile.Position = 0;

                    switch (bybuffer[9])
                    {
                        case (byte)FirmwareType.ICSFW:
                        {
                            byEventNumber = COMM_EVENT_NUMBER;
                            idTable = (ushort)FWDLTableIds.CommModuleFWTbl | PENDING_BIT;
                            break;
                        }
                        default:
                        {
                            throw new NotImplementedException("Table not supported");
                        }
                    }

                    BuildPendingHeader(ref streamHeader, false, false,
                                       byEventNumber, PendingEventRecord.PendingEventCode.NonTimeTrigger);

                    usNumberChunks = (ushort)(streamFile.Length / BLOCK_SIZE);

                    if (streamFile.Length != BLOCK_SIZE * usNumberChunks)
                    {
                        usNumberChunks++;
                    }

                    OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                             "Firmware Download",
                                                             "Downloading..."));

                    ushort usSendSize = BLOCK_SIZE;

                    for (;
                        (usBlockIndex < usNumberChunks) && (PSEMResponse.Ok == ProtocolResponse);
                        usBlockIndex++)
                    {
                        // The last chunk could be smaller
                        if (usNumberChunks - 1 == usBlockIndex)
                        {
                            usSendSize = (ushort)(streamFile.Length % BLOCK_SIZE);
                            // If no remainder then it is a full packet
                            if (0 == usSendSize)
                            {
                                usSendSize = BLOCK_SIZE;
                            }
                        }

                        streamHeader.Position = 0;
                        streamPSEM.Position = 0;
                        streamPSEM.SetLength(0);
                        streamHeader.WriteTo(streamPSEM);
                        streamPSEM.Write(bybuffer, usBlockIndex * BLOCK_SIZE, usSendSize);

                        ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                                usBlockIndex * BLOCK_SIZE,
                                                                streamPSEM.ToArray());
                        OnStepProgress(new ProgressEventArgs());
                    }

                    // Translate Protocol result
                    Result = TranslateProtocolResult(ProtocolResponse);

                    streamFile.Close();

                    //Check on success and then activate the table
                    if (PSEMResponse.Ok == ProtocolResponse)
                    {
                        ProcResult = ProcedureResultCodes.COMPLETED;

                        if (ProcResult == ProcedureResultCodes.COMPLETED)
                        {
                            if (true == blnActivate)
                            {
                                // Activate the pending table using mfg proc 69
                                ProcResult = ActivateFWDLTable(idTable, (byte)FirmwareType.ICSFW, bybuffer[5], bybuffer[6], bybuffer[7]);

                                Result = TranslateProcedureResult(ProcResult);
                            }
                        }
                        else
                        {
                            //TODO - not sure this is the correct error
                            Result = FWDownloadResult.SECURITY_ERROR;
                        }

                        OnStepProgress(new ProgressEventArgs());
                    }
                    else //PSEMResponse.Ok != ProtocolResponse
                    {
                        //Decrement the block index so we make sure we restart on the block that we failed on.
                        usBlockIndex--;

                        Result = FWDownloadResult.WRITE_ERROR;
                    }
                    OnHideProgress(new EventArgs());
                }
            }

            catch (Exception e)
            {
                if (false == blnRetry)
                {
                    // Log it and pass it up
                    OnHideProgress(new EventArgs());
                    m_Logger.WriteException(this, e);
                    throw e;
                }
                else
                {
                    Result = FWDownloadResult.WRITE_ERROR;
                }
            }

            return Result;
        }

        /// <summary>
        /// Downloads the firmware file to the device but does NOT
        /// activate.  On download failure, the pending table is cleared.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <returns>FWDownloadResult</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#   Description
        // -------- --- ------- ------   ---------------------------------------
        // 11/15/13 AF  3.50.03	         Class re-architecture - Cloned from CENTRON_AMI
        //
        public FWDownloadResult DownloadFWNoActivate(string path)
        {
            return DownloadFWNoActivate(path, PendingEventRecord.PendingEventCode.NonTimeTrigger);
        }

        /// <summary>
        /// Downloads the firmware file to the device but does NOT
        /// activate.  On download failure, the pending table is cleared.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <param name="eventCode">event code activation method</param>
        /// <returns>FWDownloadResult</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/15/13 AF  3.50.03	       Class re-architecture - Cloned from CENTRON_AMI
        // 
        public FWDownloadResult DownloadFWNoActivate(string path, PendingEventRecord.PendingEventCode eventCode)
        {
            FWDownloadResult Result = FWDownloadResult.UNKNOWN_DRIVER_ERROR;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)FWDLTableIds.RegisterFWTbl | PENDING_BIT;
            ushort usNumberChunks = 0;
            ushort intIndex;

            System.IO.FileStream streamFile;
            System.IO.MemoryStream streamHeader = new System.IO.MemoryStream();
            System.IO.MemoryStream streamPSEM = new System.IO.MemoryStream();

            try
            {
                Result = EnterFirmwareDownloadMode(path);

                if (FWDownloadResult.SUCCESS != Result)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Initiate F/W Download procedure failed with result = "
                            + Result);
                }
                else
                {
                    // Meter is ready to receive the firmware file
                    streamFile = new System.IO.FileStream(path, System.IO.FileMode.Open, FileAccess.Read);
                    byte[] bybuffer = new byte[streamFile.Length];

                    streamFile.Read(bybuffer, 0, (int)streamFile.Length);
                    streamFile.Position = 0;

                    switch (bybuffer[9])
                    {
                        case (byte)FirmwareType.ICSFW:
                        {
                            byEventNumber = COMM_EVENT_NUMBER;
                            idTable = (ushort)FWDLTableIds.CommModuleFWTbl | PENDING_BIT;
                            break;
                        }
                        default:
                        {
                            throw new NotImplementedException("Table not yet supported");
                        }
                    }

                    BuildPendingHeader(ref streamHeader, false, false, byEventNumber, eventCode);

                    usNumberChunks = (ushort)(streamFile.Length / BLOCK_SIZE);

                    if (streamFile.Length != BLOCK_SIZE * usNumberChunks)
                    {
                        usNumberChunks++;
                    }

                    OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                             "Firmware Download",
                                                             "Downloading..."));

                    ushort usSendSize = BLOCK_SIZE;

                    for (intIndex = 0;
                        (intIndex < usNumberChunks) && (PSEMResponse.Ok == ProtocolResponse);
                        intIndex++)
                    {
                        // The last chunk could be smaller
                        if (usNumberChunks - 1 == intIndex)
                        {
                            usSendSize = (ushort)(streamFile.Length % BLOCK_SIZE);
                            // If no remainder then it is a full packet
                            if (0 == usSendSize)
                            {
                                usSendSize = BLOCK_SIZE;
                            }
                        }

                        streamHeader.Position = 0;
                        streamPSEM.Position = 0;
                        streamPSEM.SetLength(0);
                        streamHeader.WriteTo(streamPSEM);
                        streamPSEM.Write(bybuffer, intIndex * BLOCK_SIZE, usSendSize);

                        ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                                intIndex * BLOCK_SIZE,
                                                                streamPSEM.ToArray());
                        OnStepProgress(new ProgressEventArgs());
                    }

                    // Translate Protocol result
                    Result = TranslateProtocolResult(ProtocolResponse);

                    streamFile.Close();

                    // If any write failed, give up and clear the pending table
                    if (PSEMResponse.Ok != ProtocolResponse)
                    {
                        Result = FWDownloadResult.WRITE_ERROR;
                    }
                    OnHideProgress(new EventArgs());
                }
            }

            catch (Exception e)
            {
                // Log it and pass it up
                OnHideProgress(new EventArgs());
                m_Logger.WriteException(this, e);
                throw e;
            }

            return Result;
        }

        /// <summary>
        /// This method just downloads the firmware file blocks to the device for a 
        /// given range of blocks.  Use 1-based indexing for blocks.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <param name="usStartBlock">The first block to download.</param>
        /// <param name="usEndBlock">The last block to download.</param>
        /// <returns>FWDownloadResult</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/15/13 AF  3.50.03	       Class re-architecture - Cloned from CENTRON_AMI
        // 
        public FWDownloadResult DownloadFWBlocks(string path, ushort usStartBlock, ushort usEndBlock)
        {
            FWDownloadResult Result = FWDownloadResult.SUCCESS;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)FWDLTableIds.CommModuleFWTbl | PENDING_BIT;
            ushort usNumberChunks = 0;
            ushort usIndex;

            System.IO.FileStream streamFile;
            System.IO.MemoryStream streamHeader = new System.IO.MemoryStream();
            System.IO.MemoryStream streamPSEM = new System.IO.MemoryStream();

            try
            {

                streamFile = new System.IO.FileStream(path, System.IO.FileMode.Open, FileAccess.Read);
                byte[] bybuffer = new byte[streamFile.Length];

                streamFile.Read(bybuffer, 0, (int)streamFile.Length);
                streamFile.Position = 0;

                switch (bybuffer[9])
                {
                    case (byte)FirmwareType.ICSFW:
                    {
                        byEventNumber = COMM_EVENT_NUMBER;
                        idTable = (ushort)FWDLTableIds.CommModuleFWTbl | PENDING_BIT;
                        break;
                    }
                    default:
                    {
                        throw new NotImplementedException("Table not yet supported");
                    }
                }

                BuildPendingHeader(ref streamHeader, false, false,
                                   byEventNumber, PendingEventRecord.PendingEventCode.NonTimeTrigger);

                usNumberChunks = (ushort)(streamFile.Length / BLOCK_SIZE);

                if (streamFile.Length != BLOCK_SIZE * usNumberChunks)
                {
                    usNumberChunks++;
                }

                OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                         "Firmware Download",
                                                         "Downloading..."));

                ushort usSendSize = BLOCK_SIZE;

                //Make sure the start block is not less than 1
                if (usStartBlock < 1)
                {
                    usStartBlock = 1;
                }
                //Make sure the end block is not greater than the actual number of FW blocks.
                if (usEndBlock > usNumberChunks)
                {
                    usEndBlock = usNumberChunks;
                }

                for (usIndex = (ushort)(usStartBlock - 1);
                    (usIndex < usEndBlock) && (PSEMResponse.Ok == ProtocolResponse);
                    usIndex++)
                {
                    // The last chunk could be smaller
                    if (usNumberChunks - 1 == usIndex)
                    {
                        usSendSize = (ushort)(streamFile.Length % BLOCK_SIZE);
                        // If no remainder then it is a full packet
                        if (0 == usSendSize)
                        {
                            usSendSize = BLOCK_SIZE;
                        }
                    }

                    streamHeader.Position = 0;
                    streamPSEM.Position = 0;
                    streamPSEM.SetLength(0);
                    streamHeader.WriteTo(streamPSEM);
                    streamPSEM.Write(bybuffer, usIndex * BLOCK_SIZE, usSendSize);

                    ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                            usIndex * BLOCK_SIZE,
                                                            streamPSEM.ToArray());
                    OnStepProgress(new ProgressEventArgs());
                }

                // Translate Protocol result
                Result = TranslateProtocolResult(ProtocolResponse);

                streamFile.Close();


                OnHideProgress(new EventArgs());

            }

            catch (Exception e)
            {
                // Log it and pass it up
                OnHideProgress(new EventArgs());
                m_Logger.WriteException(this, e);
                throw e;
            }

            return Result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs a manufacturer procedure 37 to tell the meter to get ready for
        /// firmware download
        /// </summary>
        /// <param name="strFilePath">path to the f/w file to download</param>
        /// <returns>ProcedureResultCodes</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------
        //  11/15/13 AF  3.50.03	    Class re-architecture - Cloned from CENTRON_AMI
        //
        private FWDownloadResult EnterFirmwareDownloadMode(string strFilePath)
        {
            //Construct the parameters for mfg proc 37 and execute the procedure
            FileInfo fi = new FileInfo(strFilePath);
            byte[] bybuf = new byte[FW_HEADER_LENGTH];
            byte[] byParameters = null;
            byte[] byImageSize = new byte[IMAGE_SIZE_FIELD_LEN];
            byte[] byChunkSize = new byte[CHUNK_SIZE_FIELD_LEN];
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            FWDownloadResult FWResult = FWDownloadResult.UNKNOWN_DRIVER_ERROR;

            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                                   "Initiating Firmware Download");

                // Most of the procedure parameters are in the f/w file header
                FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                fs.Read(bybuf, 0, FW_HEADER_LENGTH);
                fs.Close();

                byParameters = new byte[INIT_FW_DOWNLOAD_THIRD_PARTY_LEN];

                byParameters.Initialize();

                // CRC LSB first
                byParameters[0] = bybuf[1];
                byParameters[1] = bybuf[0];

                // Retrieve the parameters out of the header
                Array.Copy(bybuf, 5, byParameters, 2, 9);

                // image size
                byImageSize = BitConverter.GetBytes((int)fi.Length);
                Array.Copy(byImageSize, 0, byParameters, 11, IMAGE_SIZE_FIELD_LEN);

                // chunk size -- 64 or 128 bytes; hard coded here to 128
                byChunkSize = BitConverter.GetBytes(BLOCK_SIZE);
                Array.Copy(byChunkSize, 0, byParameters, 15, CHUNK_SIZE_FIELD_LEN);

                // Add the device class
                Array.Copy(bybuf, 14, byParameters, 17, 4);

                ProcResult = ExecuteProcedure(Procedures.INITIATE_FW_LOADER_SETUP,
                                              byParameters,
                                              out ProcResponse);


                if (ProcedureResultCodes.INVALID_PARAM == ProcResult)
                {
                    // The Firmware load did not work.  At some point during development they added
                    //  more detail in the Response, so we can read the byte and see what the error is
                    switch (ProcResponse[0])
                    {
                        case 1:
                            FWResult = FWDownloadResult.FW_IMAGE_TOO_BIG;
                            break;
                        case 2:
                            FWResult = FWDownloadResult.HW_REVISION_OUTSIDE_RANGE;
                            break;
                        case 3:
                            FWResult = FWDownloadResult.HW_VERSION_OUTSIDE_RANGE;
                            break;
                        case 10:
                            FWResult = FWDownloadResult.FW_TYPE_IS_INVALID;
                            break;
                        case 11:
                            FWResult = FWDownloadResult.ZIGBEE_FW_TYPE_INVALID;
                            break;
                        default:
                            FWResult = FWDownloadResult.INVALID_CONFIG;
                            break;
                    }
                }
                else
                {
                    FWResult = TranslateProcedureResult(ProcResult);
                }
            }
            catch (PSEMException PSEMExp)
            {
                //TODO - This does not catch the error for same version when the device is an ITRU
                if (PSEMResponse.Err == PSEMExp.PSEMResponse
                    && (byte)FirmwareType.ICSFW == byParameters[6]
                    && CommModuleVersion == byParameters[2]
                    && CommModuleRevision == byParameters[3]
                    && CommModuleBuild == byParameters[4])
                {
                    //This is how an attempt to download the same OW ICS comm module firmware
                    //version will fail.
                    FWResult = FWDownloadResult.ICS_SAME_VERSION_REJECTION;
                }
                else
                {
                    // Log it and pass it up
                    m_Logger.WriteException(this, PSEMExp);
                    throw (PSEMExp);
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }


            return FWResult;
        }

        /// <summary>
        /// Translates a PSEMResponse into a FWDownloadResult
        /// </summary>
        /// <param name="ProtocolResponse">PSEMResponse - enumerates all possible
        /// results or errors that could be returned from the PSEM layer</param>
        /// <returns>result codes specific to firmware download</returns>
        // Revision History 
        // MM/DD/YY who Version Issue# Description 
        // -------- --- ------- ------ ---------------------------------------
        //  11/15/13 AF  3.50.03	    Class re-architecture - Cloned from CENTRON_AMI
        // 
        private FWDownloadResult TranslateProtocolResult(PSEMResponse ProtocolResponse)
        {
            FWDownloadResult Result;

            switch (ProtocolResponse)
            {
                case PSEMResponse.Ok:
                {
                    Result = FWDownloadResult.SUCCESS;
                    break;
                }
                case PSEMResponse.Isc:
                {
                    Result = FWDownloadResult.SECURITY_ERROR;
                    break;
                }
                case PSEMResponse.Onp:
                {
                    Result = FWDownloadResult.UNSUPPORTED_OPERATION;
                    break;
                }
                default:
                {
                    Result = FWDownloadResult.WRITE_ERROR;
                    break;
                }
            }

            return Result;
        }

        /// <summary>
        /// Translates a ProcedureResultCodes into a FWDownloadResult
        ///</summary> 
        /// <param name="ProcResult">Table 08 results codes</param>
        /// <returns>FWDownloadResult</returns>
        // Revision History 
        // MM/DD/YY who Version Issue# Description 
        // -------- --- ------- ------ ---------------------------------------
        //  11/15/13 AF  3.50.03	    Class re-architecture - Cloned from CENTRON_AMI
        // 
        private FWDownloadResult TranslateProcedureResult(ProcedureResultCodes ProcResult)
        {
            FWDownloadResult Result;

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    Result = FWDownloadResult.SUCCESS;
                    break;
                }
                case ProcedureResultCodes.INVALID_PARAM:
                {
                    Result = FWDownloadResult.INVALID_CONFIG;
                    break;
                }
                case ProcedureResultCodes.UNRECOGNIZED_PROC:
                {
                    Result = FWDownloadResult.UNSUPPORTED_OPERATION;
                    break;
                }
                case ProcedureResultCodes.TIMING_CONSTRAINT:
                {
                    Result = FWDownloadResult.DEVICE_BUSY;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    Result = FWDownloadResult.SECURITY_ERROR;
                    break;
                }
                default:
                {
                    Result = FWDownloadResult.WRITE_ERROR;
                    break;
                }
            }

            return Result;
        }

        #endregion
    }
}
