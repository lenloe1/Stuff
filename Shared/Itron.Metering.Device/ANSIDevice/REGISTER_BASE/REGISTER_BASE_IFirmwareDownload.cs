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
//                              Copyright © 2006 - 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the CENTRON AMI. (IFirmwareDownload implementation)
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 07/26/06 KRC				Created
    //
    public partial class REGISTER_BASE : CANSIDevice, IFirmwareDownload
    {
        #region Definitions

        /// <summary>
        /// Enum for the target hardware
        /// </summary>
        public enum HWType
        {
            /// <summary>
            /// Hardware 1.0 type
            /// </summary>
            HW1_0 = 0,
            /// <summary>
            /// Hardware 1.5 type
            /// </summary>
            HW1_5 = 1,
            /// <summary>
            /// Hardware 2.0 type
            /// </summary>
            HW2_0 = 2,
            /// <summary>
            /// Hardware Polyphase Basic type
            /// </summary>
            HWPolyBasic = 3,
            /// <summary>
            /// Hardware Polyphase Advanced type
            /// </summary>
            HWPolyAdvanced = 4,
            /// <summary>
            /// Hardware HAN device type
            /// </summary>
            HWHANDevice = 5,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Downloads the firmware file to the meter and activates it.    
        /// On download failure, the pending table is cleared.  The activation
        /// will cause the meter to drop the psem task so meter log off must
        /// follow this function call
        /// </summary>
        /// <param name="path">Complete file path of the firmware file</param>
        /// <returns>Itron.Metering.Device.FWDownloadResult</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/28/06 AF  7.35.00  N/A   Created
        // 09/15/06 AF  7.35.00  N/A   Added Catch for TimeOutException
        // 10/18/06 AF  7.40.00  N/A   Removed wait within the main loop
        // 05/13/08 AF  1.50.24        Removed IFirmwareDownload from the method name
        // 04/19/10 AF  2.40.39        Added M2 Gateway support
        // 08/18/11 AF  2.52.05        Added support for authentication using a hash code
        // 08/26/11 AF  2.52.08        Added support for Cisco f/w
        // 09/22/11 AF  2.52.21  N/A   Added support for Cisco config file f/w d/l - TODO remove when no longer needed
        // 10/12/11 AF  2.53.00        Changed the Cisco Comm fw enum name
        // 03/22/12 JJJ 2.60.xx        Added support for ChoiceConnect FW
        // 05/10/12 JJJ 2.60.xx        Tweaked FW Type passed to AuthenticateFWDL if ChoiceConnect, make RFLAN
        // 
        public FWDownloadResult DownloadFW(string path)
        {
            FWDownloadResult Result = FWDownloadResult.UNKNOWN_DRIVER_ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)PendingTableIds.RegisterFWTbl | PENDING_BIT;
            ushort usNumberChunks = 0;
            ushort intIndex;

            System.IO.FileStream streamFile;
            System.IO.MemoryStream streamHeader = new System.IO.MemoryStream();
            System.IO.MemoryStream streamPSEM = new System.IO.MemoryStream();

            try
            {
                // Tell the meter to enter firmware download mode
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
                        case (byte)FirmwareType.RegisterFW:
                        case (byte)FirmwareType.M2GTWY:
                        case (byte)FirmwareType.DisplayFW:
                        {
                            byEventNumber = REGISTER_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.RegisterFWTbl | PENDING_BIT;
                            break;
                        }
                        case (byte)FirmwareType.ZigbeeFW:
                        {
                            byEventNumber = ZIGBEE_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.HANModuleFWTbl | PENDING_BIT;
                            break;
                        }
                        case (byte)FirmwareType.RFLANFW:
                        case (byte)FirmwareType.PLANFW:
                        case (byte)FirmwareType.CiscoCommFW:
                        case (byte)FirmwareType.CiscoCfgFW:
                        case (byte)FirmwareType.ChoiceConnectFW:
                        {
                            byEventNumber = COMM_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.CommModuleFWTbl | PENDING_BIT;
                            break;
                        }
                        case (byte)FirmwareType.HANDevFW:
                        {
                            byEventNumber = HAN_DEV_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.HANDeviceFWTbl | PENDING_BIT;
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

                    //Check on success and then activate the table
                    if (PSEMResponse.Ok == ProtocolResponse)
                    {
                        if (FWDLLogSupported)
                        {
                            //Construct the hash code and call the procedure to authenticate
                            CENTRON_AMI_FW_File FWFile = new CENTRON_AMI_FW_File(path);
                            byte[] FWHashCode = FWFile.HashCode;

                            // if MSM ChoiceConnect meter and ChoiceConnect FWDL request, spoof RFLAN FWDL             
                            CENTRON_AMI AmiDevice = this as CENTRON_AMI;
                            if (AmiDevice != null &&
                                bybuffer[9] == (byte)FirmwareType.ChoiceConnectFW &&
                                AmiDevice.IsChoiceConnectMsmMeter)
                            {
                                bybuffer[9] = (byte)FirmwareType.RFLANFW;
                            }

                            ProcResult = AuthenticateFWDL(idTable, bybuffer[9], FWHashCode);
                        }
                        else
                        {
                            ProcResult = ProcedureResultCodes.COMPLETED;
                        }

                        if (ProcResult == ProcedureResultCodes.COMPLETED)
                        {
                            // Activate the pending table                        
                            ProcResult = ActivatePendingTable(false, false, byEventNumber,
                                                              PendingEventRecord.PendingEventCode.NonTimeTrigger);

                            Result = TranslateProcedureResult(ProcResult);
                        }
                        else
                        {
                            //We couldn't authenticate using the hash code so activation will fail
                            ProcResult = ClearPendingTable(false, false, byEventNumber,
                                                       PendingEventRecord.PendingEventCode.NonTimeTrigger);

                            //TODO - not sure this is the correct error
                            Result = FWDownloadResult.SECURITY_ERROR;
                        }

                        OnStepProgress(new ProgressEventArgs());
                    }
                    else
                    {
                        // Write failed, so clear the pending table
                        ProcResult = ClearPendingTable(false, false, byEventNumber,
                                                       PendingEventRecord.PendingEventCode.NonTimeTrigger);

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
        /// Downloads the firmware file to the device but does NOT
        /// activate.  On download failure, the pending table is cleared.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <returns>FWDownloadResult</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/05/06 AF  7.35.00  N/A   Created
        // 10/18/06 AF  7.40.00  N/A   Removed wait within the main loop
        // 05/13/08 AF  1.50.24        Removed IFirmwareDownload from the method name
        // 04/19/10 AF  2.40.39        Added M2 Gateway support
        // 03/21/12 JJJ 2.60.xx        Added support for ChoiceConnect FW
        // 08/17/12 AF  2.60.55        Added support for RF Mesh FW and RF Mesh Config
        // 
        public FWDownloadResult DownloadFWNoActivate(string path)
        {
            FWDownloadResult Result = FWDownloadResult.UNKNOWN_DRIVER_ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)PendingTableIds.RegisterFWTbl | PENDING_BIT;
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
                        case (byte)FirmwareType.RegisterFW:
                        case (byte)FirmwareType.M2GTWY:
                        case (byte)FirmwareType.DisplayFW:
                        {
                            byEventNumber = REGISTER_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.RegisterFWTbl | PENDING_BIT;
                            break;
                        }
                        case (byte)FirmwareType.ZigbeeFW:
                        {
                            byEventNumber = ZIGBEE_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.HANModuleFWTbl | PENDING_BIT;
                            break;
                        }
                        case (byte)FirmwareType.RFLANFW:
                        case (byte)FirmwareType.PLANFW:
                        case (byte)FirmwareType.CiscoCommFW:
                        case (byte)FirmwareType.CiscoCfgFW:
                        case (byte)FirmwareType.ChoiceConnectFW:
                        {
                            byEventNumber = COMM_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.CommModuleFWTbl | PENDING_BIT;
                            break;
                        }
                        case (byte)FirmwareType.HANDevFW:
                        {
                            byEventNumber = HAN_DEV_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.HANDeviceFWTbl | PENDING_BIT;
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
                        // Write failed, so clear the pending table
                        ProcResult = ClearPendingTable(false, false, byEventNumber,
                                                       PendingEventRecord.PendingEventCode.NonTimeTrigger);

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
        // 01/20/10 jrf 2.40.08  N/A    Created
        // 03/22/12 JJJ 2.60.xx        Added support for ChoiceConnect FW
        // 
        public FWDownloadResult DownloadFWBlocks(string path, ushort usStartBlock, ushort usEndBlock)
        {
            FWDownloadResult Result = FWDownloadResult.SUCCESS;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)PendingTableIds.RegisterFWTbl | PENDING_BIT;
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
                    case (byte)FirmwareType.RegisterFW:
                    {
                        byEventNumber = REGISTER_EVENT_NUMBER;
                        idTable = (ushort)PendingTableIds.RegisterFWTbl | PENDING_BIT;
                        break;
                    }
                    case (byte)FirmwareType.ZigbeeFW:
                    {
                        byEventNumber = ZIGBEE_EVENT_NUMBER;
                        idTable = (ushort)PendingTableIds.HANModuleFWTbl | PENDING_BIT;
                        break;
                    }
                    case (byte)FirmwareType.RFLANFW:
                    case (byte)FirmwareType.PLANFW:
                    case (byte)FirmwareType.ChoiceConnectFW:
                    {
                        byEventNumber = COMM_EVENT_NUMBER;
                        idTable = (ushort)PendingTableIds.CommModuleFWTbl | PENDING_BIT;
                        break;
                    }
                    case (byte)FirmwareType.DisplayFW:
                    {
                        //TODO - Firmware wants to use the same event number.  Need to test
                        //that it works
                        byEventNumber = REGISTER_EVENT_NUMBER;
                        idTable = (ushort)PendingTableIds.RegisterFWTbl | PENDING_BIT;
                        break;
                    }
                    case (byte)FirmwareType.HANDevFW:
                    {
                        byEventNumber = HAN_DEV_EVENT_NUMBER;
                        idTable = (ushort)PendingTableIds.HANDeviceFWTbl | PENDING_BIT;
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
        // 08/21/06 AF  7.35.00 N/A     Created
        // 09/05/06 AF  7.35.00         Corrected the order of the data in the
        //                             procedure parameters
        // 10/04/06 AF  7.40.00 N/A     Moved from CENTRON_AMI.cs
        // 07/31/07 KRC 8.10.16 3058    Adding more detailed errors;
        // 01/23/12 RCG 2.53.33 191589 Adding the Device Class to the parameter list in order to support Gas Ranger Extended FW DL
        // 05/10/12 JJJ 2.60.xx        Tweaking FW Type so Register sees ChoiceConnect FW as RFLAN FW

        private FWDownloadResult EnterFirmwareDownloadMode(string strFilePath)
        {
            //Construct the parameters for mfg proc 37 and execute the procedure
            FileInfo fi = new FileInfo(strFilePath);
            bool bIsThirdPartyFWDownload = this is M2_Gateway == false && VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_LITHIUM_3_12) >= 0;
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

                if(bIsThirdPartyFWDownload == true)
                {
                    byParameters = new byte[INIT_FW_DOWNLOAD_THIRD_PARTY_LEN];
                }
                else
                {
                    byParameters = new byte[INIT_FW_DOWNLOAD_PARAM_LEN];
                }

                byParameters.Initialize();

                // CRC LSB first
                byParameters[0] = bybuf[1];
                byParameters[1] = bybuf[0];

                // Retrieve the parameters out of the header
                Array.Copy(bybuf, 5, byParameters, 2, 9);

                // if MSM ChoiceConnect meter and ChoiceConnect FWDL request, spoof RFLAN FWDL             
                CENTRON_AMI AmiDevice = this as CENTRON_AMI;
                if (AmiDevice != null && 
                    byParameters[6] == (byte)FirmwareType.ChoiceConnectFW &&
                    AmiDevice.IsChoiceConnectMsmMeter)
                {
                    byParameters[6] = (byte)FirmwareType.RFLANFW;
                }

                // image size
                byImageSize = BitConverter.GetBytes((int)fi.Length);
                Array.Copy(byImageSize, 0, byParameters, 11, IMAGE_SIZE_FIELD_LEN);

                // chunk size -- 64 or 128 bytes; hard coded here to 128
                byChunkSize = BitConverter.GetBytes(BLOCK_SIZE);
                Array.Copy(byChunkSize, 0, byParameters, 15, CHUNK_SIZE_FIELD_LEN);

                if (bIsThirdPartyFWDownload == true)
                {
                    // The Device Class is a required parameter for 3rd Party Firmware Downloads
                    // The meter needs this in the reverse order that it is stored in the firmware file
                    byParameters[17] = bybuf[17];
                    byParameters[18] = bybuf[16];
                    byParameters[19] = bybuf[15];
                    byParameters[20] = bybuf[14];
                }

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
        // 09/06/06 AF 7.35.00  N/A 	Created
        // 09/18/06 AF 7.35.00  N/A		Added additional PSEMResponse codes
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
        // 09/15/06 AF 7.35.00  N/A 	Created
        // 09/18/06 AF 7.35.00  N/A		Added additional ProcedureResultCodes
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