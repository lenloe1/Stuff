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
//                              Copyright © 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class to handle all functionality common using Firmware Download for ICS Gateway Devices.
    /// </summary>
    public class CommonFirmwareDownload
    {
        #region Public Methods

        /// <summary>
        /// The Constructor. Used to access common FWDL methods and properites. Usually instantiated
        /// by classes that are implementing the IFirmwareDownload Interface. At this time it only
        /// applies to ICS Gateway devices.
        /// </summary>
        /// <param name="PSEM">The PSEM object to be used in the class.</param>
        /// <param name="ANSIDevice">The ANSIDevice to be used in the class.</param>
        /// <param name="DebugLogger">The DebugLogger to be used in the class.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  01/06/14 DLG 3.50.14          Created.
        //  
        public CommonFirmwareDownload(CPSEM PSEM, CANSIDevice ANSIDevice, Logger DebugLogger)
        {
            m_PSEM = PSEM;
            m_ANSIDevice = ANSIDevice;
            m_Logger = DebugLogger;
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
        // 01/06/14 DLG 3.50.19        Class re-architecture - Moved from ICS_Gateway to 
        //                             CommonFirmwareDownload.
        // 02/07/14 jrf 3.50.32 419257 Modified to use a dynamic FWDL block size based on the negotiated 
        //                             PSEM packet size.
        // 
        public FWDownloadResult DownloadFWBlocks(string path, ushort usStartBlock, ushort usEndBlock)
        {
            FWDownloadResult Result = FWDownloadResult.SUCCESS;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)CANSIDevice.FWDLTableIds.CommModuleFWTbl | CANSIDevice.PENDING_BIT;
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
                            byEventNumber = CANSIDevice.COMM_EVENT_NUMBER;
                            idTable = (ushort)CANSIDevice.FWDLTableIds.CommModuleFWTbl | CANSIDevice.PENDING_BIT;
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException("Table not yet supported");
                        }
                }

                m_ANSIDevice.BuildPendingHeader(ref streamHeader, false, false,
                                   byEventNumber, PendingEventRecord.PendingEventCode.NonTimeTrigger);

                usNumberChunks = (ushort)(streamFile.Length / m_ANSIDevice.FWDLBlockSize);

                if (streamFile.Length != m_ANSIDevice.FWDLBlockSize * usNumberChunks)
                {
                    usNumberChunks++;
                }

                m_ANSIDevice.OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                         "Firmware Download",
                                                         "Downloading..."));

                ushort usSendSize = m_ANSIDevice.FWDLBlockSize;

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
                        usSendSize = (ushort)(streamFile.Length % m_ANSIDevice.FWDLBlockSize);
                        // If no remainder then it is a full packet
                        if (0 == usSendSize)
                        {
                            usSendSize = m_ANSIDevice.FWDLBlockSize;
                        }
                    }

                    streamHeader.Position = 0;
                    streamPSEM.Position = 0;
                    streamPSEM.SetLength(0);
                    streamHeader.WriteTo(streamPSEM);
                    streamPSEM.Write(bybuffer, usIndex * m_ANSIDevice.FWDLBlockSize, usSendSize);

                    ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                            usIndex * m_ANSIDevice.FWDLBlockSize,
                                                            streamPSEM.ToArray());
                    m_ANSIDevice.OnStepProgress(new ProgressEventArgs());
                }

                // Translate Protocol result
                Result = TranslateProtocolResult(ProtocolResponse);

                streamFile.Close();

                m_ANSIDevice.OnHideProgress(new EventArgs());

            }

            catch (Exception e)
            {
                // Log it and pass it up
                m_ANSIDevice.OnHideProgress(new EventArgs());
                m_Logger.WriteException(this, e);
                throw e;
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
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/15/13 AF  3.50.03	       Class re-architecture - Cloned from CENTRON_AMI
        // 01/06/14 DLG 3.50.19        Class re-architecture - Moved from ICS_Gateway to 
        //                             CommonFirmwareDownload.
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
        // 01/06/14 DLG 3.50.19        Class re-architecture - Moved from ICS_Gateway to 
        //                             CommonFirmwareDownload.
        // 02/07/14 jrf 3.50.32 419257 Modified to use a dynamic FWDL block size based on the negotiated 
        //                             PSEM packet size.
        //
        public FWDownloadResult DownloadFWNoActivate(string path, PendingEventRecord.PendingEventCode eventCode)
        {
            FWDownloadResult Result = FWDownloadResult.UNKNOWN_DRIVER_ERROR;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)CANSIDevice.FWDLTableIds.RegisterFWTbl | CANSIDevice.PENDING_BIT;
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
                                byEventNumber = CANSIDevice.COMM_EVENT_NUMBER;
                                idTable = (ushort)CANSIDevice.FWDLTableIds.CommModuleFWTbl | CANSIDevice.PENDING_BIT;
                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException("Table not yet supported");
                            }
                    }

                    m_ANSIDevice.BuildPendingHeader(ref streamHeader, false, false, byEventNumber, eventCode);

                    usNumberChunks = (ushort)(streamFile.Length / m_ANSIDevice.FWDLBlockSize);

                    if (streamFile.Length != m_ANSIDevice.FWDLBlockSize * usNumberChunks)
                    {
                        usNumberChunks++;
                    }

                    m_ANSIDevice.OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                             "Firmware Download",
                                                             "Downloading..."));

                    ushort usSendSize = m_ANSIDevice.FWDLBlockSize;

                    for (intIndex = 0;
                        (intIndex < usNumberChunks) && (PSEMResponse.Ok == ProtocolResponse);
                        intIndex++)
                    {
                        // The last chunk could be smaller
                        if (usNumberChunks - 1 == intIndex)
                        {
                            usSendSize = (ushort)(streamFile.Length % m_ANSIDevice.FWDLBlockSize);
                            // If no remainder then it is a full packet
                            if (0 == usSendSize)
                            {
                                usSendSize = m_ANSIDevice.FWDLBlockSize;
                            }
                        }

                        streamHeader.Position = 0;
                        streamPSEM.Position = 0;
                        streamPSEM.SetLength(0);
                        streamHeader.WriteTo(streamPSEM);
                        streamPSEM.Write(bybuffer, intIndex * m_ANSIDevice.FWDLBlockSize, usSendSize);

                        ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                                intIndex * m_ANSIDevice.FWDLBlockSize,
                                                                streamPSEM.ToArray());
                        m_ANSIDevice.OnStepProgress(new ProgressEventArgs());
                    }

                    // Translate Protocol result
                    Result = TranslateProtocolResult(ProtocolResponse);

                    streamFile.Close();

                    // If any write failed, give up and clear the pending table
                    if (PSEMResponse.Ok != ProtocolResponse)
                    {
                        Result = FWDownloadResult.WRITE_ERROR;
                    }
                    m_ANSIDevice.OnHideProgress(new EventArgs());
                }
            }

            catch (Exception e)
            {
                // Log it and pass it up
                m_ANSIDevice.OnHideProgress(new EventArgs());
                m_Logger.WriteException(this, e);
                throw e;
            }

            return Result;
        }

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
        // 11/15/13 AF  3.50.04	          Class re-architecture - cloned from CENTRON_AMI
        // 01/06/14 DLG 3.50.19           Class re-architecture - Moved from ICS_Gateway to 
        //                                CommonFirmwareDownload.
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
        // 11/15/13 AF  3.50.03	          Class re-architecture - Cloned from CENTRON_AMI
        // 01/06/14 DLG 3.50.19           Class re-architecture - Moved from ICS_Gateway to 
        //                                CommonFirmwareDownload.
        // 02/07/14 jrf 3.50.32 419257 Modified to use a dynamic FWDL block size based on the negotiated 
        //                             PSEM packet size.
        // 02/18/14 AF  3.50.35 WR459123 Removed commented out debug code
        // 02/21/14 jrf  3.50.36 460307 Improved firmware download logging.
        // 02/24/14 AF  3.50.36 WR460307 Added another parameter to the activate procedure - the pad byte is being used as a patch value
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString")]
        public FWDownloadResult DownloadFW(string path, ref ushort usBlockIndex, bool blnRetry = false, bool blnActivate = true)
        {
            FWDownloadResult Result = FWDownloadResult.UNKNOWN_DRIVER_ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)CANSIDevice.FWDLTableIds.CommModuleFWTbl | CANSIDevice.PENDING_BIT;
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

                    m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                                   "Resuming Firmware Download @ Block " + usBlockIndex.ToString());
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
                                byEventNumber = CANSIDevice.COMM_EVENT_NUMBER;
                                idTable = (ushort)CANSIDevice.FWDLTableIds.CommModuleFWTbl | CANSIDevice.PENDING_BIT;
                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException("Table not supported");
                            }
                    }

                    m_ANSIDevice.BuildPendingHeader(ref streamHeader, false, false,
                                       byEventNumber, PendingEventRecord.PendingEventCode.NonTimeTrigger);

                    usNumberChunks = (ushort)(streamFile.Length / m_ANSIDevice.FWDLBlockSize);

                    if (streamFile.Length != m_ANSIDevice.FWDLBlockSize * usNumberChunks)
                    {
                        usNumberChunks++;
                    }

                    m_ANSIDevice.OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                             "Firmware Download",
                                                             "Downloading..."));

                    ushort usSendSize = m_ANSIDevice.FWDLBlockSize;

                    for (;
                        (usBlockIndex < usNumberChunks) && (PSEMResponse.Ok == ProtocolResponse);
                        usBlockIndex++)
                    {
                        // The last chunk could be smaller
                        if (usNumberChunks - 1 == usBlockIndex)
                        {
                            usSendSize = (ushort)(streamFile.Length % m_ANSIDevice.FWDLBlockSize);
                            // If no remainder then it is a full packet
                            if (0 == usSendSize)
                            {
                                usSendSize = m_ANSIDevice.FWDLBlockSize;
                            }
                        }

                        m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                                   "Firmware Download - Sending Block " + usBlockIndex.ToString());

                        streamHeader.Position = 0;
                        streamPSEM.Position = 0;
                        streamPSEM.SetLength(0);
                        streamHeader.WriteTo(streamPSEM);
                        streamPSEM.Write(bybuffer, usBlockIndex * m_ANSIDevice.FWDLBlockSize, usSendSize);

                        ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                                usBlockIndex * m_ANSIDevice.FWDLBlockSize,
                                                                streamPSEM.ToArray());
                        m_ANSIDevice.OnStepProgress(new ProgressEventArgs());
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
                                m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                                   "Activating Firmware Download");

                                // Activate the pending table using mfg proc 69
                                ProcResult = m_ANSIDevice.ActivateFWDLTable(idTable, (byte)FirmwareType.ICSFW, bybuffer[5], bybuffer[6], bybuffer[7], bybuffer[8]);

                                Result = TranslateProcedureResult(ProcResult);
                            }
                        }
                        else
                        {
                            //TODO - not sure this is the correct error
                            Result = FWDownloadResult.SECURITY_ERROR;
                        }

                        m_ANSIDevice.OnStepProgress(new ProgressEventArgs());
                    }
                    else //PSEMResponse.Ok != ProtocolResponse
                    {
                        //Decrement the block index so we make sure we restart on the block that we failed on.
                        usBlockIndex--;

                        Result = FWDownloadResult.WRITE_ERROR;
                    }
                    m_ANSIDevice.OnHideProgress(new EventArgs());
                }
            }

            catch (Exception e)
            {
                m_Logger.WriteException(this, e);

                if (false == blnRetry)
                {
                    // Log it and pass it up
                    m_ANSIDevice.OnHideProgress(new EventArgs());

                    throw e;
                }
                else
                {
                    Result = FWDownloadResult.WRITE_ERROR;
                }
            }

            return Result;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Translates a PSEMResponse into a FWDownloadResult
        /// </summary>
        /// <param name="ProtocolResponse">PSEMResponse - enumerates all possible
        /// results or errors that could be returned from the PSEM layer</param>
        /// <returns>result codes specific to firmware download</returns>
        // Revision History 
        // MM/DD/YY who Version Issue# Description 
        // -------- --- ------- ------ ---------------------------------------
        // 11/15/13 AF  3.50.03	       Class re-architecture - Cloned from CENTRON_AMI
        // 01/06/14 DLG 3.50.19        Class re-architecture - Moved from ICS_Gateway to 
        //                             CommonFirmwareDownload.
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
        // 11/15/13 AF  3.50.03	       Class re-architecture - Cloned from CENTRON_AMI
        // 01/06/14 DLG 3.50.19        Class re-architecture - Moved from ICS_Gateway to 
        //                             CommonFirmwareDownload.
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

        /// <summary>
        /// Performs a manufacturer procedure 37 to tell the meter to get ready for
        /// firmware download
        /// </summary>
        /// <param name="strFilePath">path to the f/w file to download</param>
        /// <returns>ProcedureResultCodes</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/15/13 AF  3.50.03	       Class re-architecture - Cloned from CENTRON_AMI
        // 01/06/14 DLG 3.50.19        Class re-architecture - Moved from ICS_Gateway to 
        //                             CommonFirmwareDownload.
        // 02/03/14 AF  3.50.30 TQ9508 We needed to reverse the order of the bytes in the device class
        //                             before adding it to the parameter list
        // 02/07/14 jrf 3.50.32 419257 Modified to use a dynamic FWDL block size based on the negotiated 
        //                             PSEM packet size.
        //
        private FWDownloadResult EnterFirmwareDownloadMode(string strFilePath)
        {
            //Construct the parameters for mfg proc 37 and execute the procedure
            FileInfo fi = new FileInfo(strFilePath);
            byte[] bybuf = new byte[CANSIDevice.FW_HEADER_LENGTH];
            byte[] byParameters = null;
            byte[] byImageSize = new byte[CANSIDevice.IMAGE_SIZE_FIELD_LEN];
            byte[] byChunkSize = new byte[CANSIDevice.CHUNK_SIZE_FIELD_LEN];
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            FWDownloadResult FWResult = FWDownloadResult.UNKNOWN_DRIVER_ERROR;
            ICommModVersions CommModVers = m_ANSIDevice as ICommModVersions;

            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                                   "Initiating Firmware Download");

                // Most of the procedure parameters are in the f/w file header
                FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                fs.Read(bybuf, 0, CANSIDevice.FW_HEADER_LENGTH);
                fs.Close();

                byParameters = new byte[CANSIDevice.INIT_FW_DOWNLOAD_THIRD_PARTY_LEN];

                byParameters.Initialize();

                // CRC LSB first
                byParameters[0] = bybuf[1];
                byParameters[1] = bybuf[0];

                // Retrieve the parameters out of the header
                Array.Copy(bybuf, 5, byParameters, 2, 9);

                // image size
                byImageSize = BitConverter.GetBytes((int)fi.Length);
                Array.Copy(byImageSize, 0, byParameters, 11, CANSIDevice.IMAGE_SIZE_FIELD_LEN);

                byChunkSize = BitConverter.GetBytes(m_ANSIDevice.FWDLBlockSize);                
                Array.Copy(byChunkSize, 0, byParameters, 15, CANSIDevice.CHUNK_SIZE_FIELD_LEN);

                // Add the device class
                // The meter needs the Device Class in the reverse order that it is stored in the firmware file
                byParameters[17] = bybuf[17];
                byParameters[18] = bybuf[16];
                byParameters[19] = bybuf[15];
                byParameters[20] = bybuf[14];

                ProcResult = m_ANSIDevice.ExecuteProcedure(Procedures.INITIATE_FW_LOADER_SETUP,
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
                if (CommModVers != null)
                {
                    if (PSEMResponse.Err == PSEMExp.PSEMResponse
                        && (byte)FirmwareType.ICSFW == byParameters[6]
                        && CommModVers.CommModuleVersion == byParameters[2]
                        && CommModVers.CommModuleRevision == byParameters[3]
                        && CommModVers.CommModuleBuild == byParameters[4])
                    {                  
                        //This is how an attempt to download the same OW ICS comm module firmware
                        //version will fail.
                        FWResult = FWDownloadResult.ICS_SAME_VERSION_REJECTION;                    
                    }
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

        #endregion Private Methods

        #region Members

        /// <summary>
        /// The PSEM protocol object.
        /// </summary>
        private CPSEM m_PSEM = null;
        /// <summary>
        /// The ANSIDevice object.
        /// </summary>
        private CANSIDevice m_ANSIDevice = null;
        /// <summary>
        /// Debug file logger
        /// </summary>
        protected Logger m_Logger;

        #endregion Members
    }
}
