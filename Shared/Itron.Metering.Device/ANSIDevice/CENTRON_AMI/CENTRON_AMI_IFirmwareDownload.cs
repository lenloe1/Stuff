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
//                              Copyright © 2006 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Threading;
using Itron.Metering.Communications.PSEM;
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
    public partial class CENTRON_AMI : IFirmwareDownload
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
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 07/08/13 jrf 2.80.51 TC 13201  Refactored to support retries of FWDL. Moved comments to refactored method.
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
        // 08/28/06 AF  7.35.00    N/A    Created
        // 09/15/06 AF  7.35.00    N/A    Added Catch for TimeOutException
        // 10/18/06 AF  7.40.00    N/A    Removed wait within the main loop
        // 05/13/08 AF  1.50.24           Removed IFirmwareDownload from the method name
        // 04/19/10 AF  2.40.39           Added M2 Gateway support
        // 08/18/11 AF  2.52.05           Added support for authentication using a hash code
        // 08/26/11 AF  2.52.08           Added support for Cisco f/w
        // 09/22/11 AF  2.52.21    N/A    Added support for Cisco config file f/w d/l - TODO remove when no longer needed
        // 10/12/11 AF  2.53.00           Changed the Cisco Comm fw enum name
        // 03/22/12 JJJ 2.60.xx           Added support for ChoiceConnect FW
        // 05/10/12 JJJ 2.60.xx           Tweaked FW Type passed to AuthenticateFWDL if ChoiceConnect, make RFLAN
        // 04/19/13 jrf 2.80.21 TQ 7639   Adding support for ICS comm module FWDL.
        // 07/08/13 jrf 2.80.51 TC 13201  Created to support retries of FWDL. 
        // 07/15/13 jrf 2.80.?? TC 15062  Added parameter to control activation.
        // 08/22/13 jrf 2.85.26 WR 420902 Decrementing the block index on a failed block write, so 
        //                                on a retry we will start back on the correct block.
        // 11/07/13 AF  3.50.02 TQ9508,9514 Have to activate the pending table using mfg proc 69 rather than std proc 13
        //                                  for I-210 and kV2c ICM firmware
        //  11/15/13 AF  3.50.03	    Class re-architecture - removed code specific to ICS_Gateway 
        // 12/02/13 jrf 3.50.10            Refactored code used to modify the FWType byte to it's own method.
        // 02/07/14 jrf 3.50.32 WR 419257  Modified to use a dynamic FWDL block size based on the negotiated 
        //                                 PSEM packet size.
        // 08/24/16 PGH 4.70.15 701952     Added HAN OTA Firmware
        // 02/06/17 AF  4.71.07 743128     Added supported for ICM modem firmware
        // 03/10/17 AF  4.71.09 749833     Renamed the firmware type because 37 is for Verizon LTE only
        // 12/05/17 AF  4.73.00 Task 469253 Added back the verizon modem fwdl
        // 12/06/17 AF  4.73.00 Task 469254 Added ATT/Rogers modem fwdl
        // 
        public FWDownloadResult DownloadFW(string path, ref ushort usBlockIndex, bool blnRetry = false, bool blnActivate = true)
        {
            FWDownloadResult Result = FWDownloadResult.UNKNOWN_DRIVER_ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            PSEMResponse ProtocolResponse = PSEMResponse.Ok;
            byte byEventNumber = 0;
            ushort idTable = (ushort)PendingTableIds.RegisterFWTbl | PENDING_BIT;
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
                        case (byte)FirmwareType.ICSFW:
                        case (byte)FirmwareType.ICS_MODEM_FW_Sierra_Verizon_LTE:
                        case (byte)FirmwareType.ICS_MODEM_FW_Sierra_ATT_Rogers_Bell_LTE:
                        {
                            byEventNumber = COMM_EVENT_NUMBER;
                            idTable = (ushort)PendingTableIds.CommModuleFWTbl | PENDING_BIT;
                            break;
                        }
                        case (byte)FirmwareType.HAN_OTA_FW:
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

                    usNumberChunks = (ushort)(streamFile.Length / FWDLBlockSize);

                    if (streamFile.Length != FWDLBlockSize * usNumberChunks)
                    {
                        usNumberChunks++;
                    }

                    OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                             "Firmware Download",
                                                             "Downloading..."));

                    ushort usSendSize = FWDLBlockSize;

                    for (;
                        (usBlockIndex < usNumberChunks) && (PSEMResponse.Ok == ProtocolResponse);
                        usBlockIndex++)
                    {
                        // The last chunk could be smaller
                        if (usNumberChunks - 1 == usBlockIndex)
                        {
                            usSendSize = (ushort)(streamFile.Length % FWDLBlockSize);
                            // If no remainder then it is a full packet
                            if (0 == usSendSize)
                            {
                                usSendSize = FWDLBlockSize;
                            }
                        }

                        streamHeader.Position = 0;
                        streamPSEM.Position = 0;
                        streamPSEM.SetLength(0);
                        streamHeader.WriteTo(streamPSEM);
                        streamPSEM.Write(bybuffer, usBlockIndex * FWDLBlockSize, usSendSize);

                        ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                                usBlockIndex * FWDLBlockSize,
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

                            if ((bybuffer[9] != (byte)FirmwareType.ICSFW) && (bybuffer[9] != (byte)FirmwareType.ICS_MODEM_FW_Sierra_Verizon_LTE))
                            {
                                //Some devices may require this byte to be adjusted.
                                bybuffer[9] = SelectFWTypeByte(bybuffer[9]);

                                ProcResult = AuthenticateFWDL(idTable, bybuffer[9], FWHashCode);
                            }
                            else //Skip authenticate for ICS comm module and modem FWDL
                            {
                                ProcResult = ProcedureResultCodes.COMPLETED;
                            }
                        }
                        else
                        {
                            ProcResult = ProcedureResultCodes.COMPLETED;
                        }

                        if (ProcResult == ProcedureResultCodes.COMPLETED)
                        {
                            if (true == blnActivate)
                            {
                                // Activate the pending table using std proc 13                       
                                ProcResult = ActivatePendingTable(false, false, byEventNumber,
                                                                    PendingEventRecord.PendingEventCode.NonTimeTrigger);

                                Result = TranslateProcedureResult(ProcResult);
                            }
                        }
                        else 
                        {
                            if (false == blnRetry)
                            {
                                //We couldn't authenticate using the hash code so activation will fail
                                ProcResult = ClearPendingTable(false, false, byEventNumber,
                                                           PendingEventRecord.PendingEventCode.NonTimeTrigger);
                            }

                            //TODO - not sure this is the correct error
                            Result = FWDownloadResult.SECURITY_ERROR;
                        }

                        OnStepProgress(new ProgressEventArgs());
                    }
                    else //PSEMResponse.Ok != ProtocolResponse
                    {
                        //Decrement the block index so we make sure we restart on the block that we failed on.
                        usBlockIndex--;

                        if (false == blnRetry)
                        {
                            // Write failed, so clear the pending table
                            ProcResult = ClearPendingTable(false, false, byEventNumber,
                                                           PendingEventRecord.PendingEventCode.NonTimeTrigger);
                        }

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
        /// This method activates the firmware download's pending table.
        /// </summary>
        /// <param name="FWType">The type of firmware to activate.</param>
        /// <returns>The result of the activate pending table procedure.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  07/09/13 jrf 2.80.51 TC 15063  Created.
        //  08/24/16 PGH 4.70.15    701952 Added HAN OTA Firmware
        //
        public ProcedureResultCodes ActivateFW(FirmwareType FWType)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte byEventNumber = REGISTER_EVENT_NUMBER;

            switch (FWType)
            {
                case FirmwareType.RegisterFW:
                case FirmwareType.M2GTWY:
                case FirmwareType.DisplayFW:
                    {
                        byEventNumber = REGISTER_EVENT_NUMBER;
                        break;
                    }
                case FirmwareType.ZigbeeFW:
                    {
                        byEventNumber = ZIGBEE_EVENT_NUMBER;
                        break;
                    }
                case FirmwareType.RFLANFW:
                case FirmwareType.PLANFW:
                case FirmwareType.CiscoCommFW:
                case FirmwareType.CiscoCfgFW:
                case FirmwareType.ChoiceConnectFW:
                case FirmwareType.ICSFW:
                    {
                        byEventNumber = COMM_EVENT_NUMBER;
                        break;
                    }
                case FirmwareType.HAN_OTA_FW:
                case FirmwareType.HANDevFW:
                    {
                        byEventNumber = HAN_DEV_EVENT_NUMBER;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid Parameter","FWType");
                    }
            }
            
            // Activate the pending table                        
            ProcResult = ActivatePendingTable(false, false, byEventNumber,
                                              PendingEventRecord.PendingEventCode.NonTimeTrigger);

            return ProcResult;
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
        // 05/22/13 jkw 2.80.33  TC11248 pass through to methos with event code parameter
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
        // 10/05/06 AF  7.35.00  N/A   Created
        // 10/18/06 AF  7.40.00  N/A   Removed wait within the main loop
        // 05/13/08 AF  1.50.24        Removed IFirmwareDownload from the method name
        // 04/19/10 AF  2.40.39        Added M2 Gateway support
        // 03/21/12 JJJ 2.60.xx        Added support for ChoiceConnect FW
        // 08/17/12 AF  2.60.55        Added support for RF Mesh FW and RF Mesh Config
        // 02/07/14 jrf 3.50.32 419257 Modified to use a dynamic FWDL block size based on the negotiated 
        //                             PSEM packet size.
        // 06/15/15 mah 4.50.140 577669 Added a retry if a busy response was received
        // 08/24/16 PGH 4.70.15 701952     Added HAN OTA Firmware
        public FWDownloadResult DownloadFWNoActivate(string path, PendingEventRecord.PendingEventCode eventCode)
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
                        case (byte)FirmwareType.HAN_OTA_FW:
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

                    BuildPendingHeader(ref streamHeader, false, false, byEventNumber, eventCode);

                    usNumberChunks = (ushort)(streamFile.Length / FWDLBlockSize);

                    if (streamFile.Length != FWDLBlockSize * usNumberChunks)
                    {
                        usNumberChunks++;
                    }

                    OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                             "Firmware Download",
                                                             "Downloading..."));

                    ushort usSendSize = FWDLBlockSize;

                    for (intIndex = 0;
                        (intIndex < usNumberChunks) && (PSEMResponse.Ok == ProtocolResponse);
                        intIndex++)
                    {
                        // The last chunk could be smaller
                        if (usNumberChunks - 1 == intIndex)
                        {
                            usSendSize = (ushort)(streamFile.Length % FWDLBlockSize);
                            // If no remainder then it is a full packet
                            if (0 == usSendSize)
                            {
                                usSendSize = FWDLBlockSize;
                            }
                        }

                        streamHeader.Position = 0;
                        streamPSEM.Position = 0;
                        streamPSEM.SetLength(0);
                        streamHeader.WriteTo(streamPSEM);
                        streamPSEM.Write(bybuffer, intIndex * FWDLBlockSize, usSendSize);

                        ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                                intIndex * FWDLBlockSize,
                                                                streamPSEM.ToArray());

                        // WR 577669 - The fwdl process is failing on ICS meters with a device busy status.  The intent of the next
                        // paragraph is to recognize that the meter may be off doing other operations when we first tried to download
                        // a block.
                        
                        if (ProtocolResponse == PSEMResponse.Bsy)
                        {
                            // Wait for the device to complete it's current task, then retry to download the same block
                            
                            Thread.Sleep(1500); // 1.5 seconds is an arbitary value - the intent is to give the meter enough time to
                                                // complete it's work
                                
                            ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                                    intIndex * FWDLBlockSize,
                                                                    streamPSEM.ToArray());
                        }
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
        // 02/07/14 jrf 3.50.32 419257 Modified to use a dynamic FWDL block size based on the negotiated 
        //                             PSEM packet size.
        // 08/24/16 PGH 4.70.15 701952 Added HAN OTA Firmware
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
                    case (byte)FirmwareType.HAN_OTA_FW:
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

                usNumberChunks = (ushort)(streamFile.Length / FWDLBlockSize);

                if (streamFile.Length != FWDLBlockSize * usNumberChunks)
                {
                    usNumberChunks++;
                }

                OnShowProgress(new ShowProgressEventArgs(1, usNumberChunks + 1,
                                                         "Firmware Download",
                                                         "Downloading..."));

                ushort usSendSize = FWDLBlockSize;

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
                        usSendSize = (ushort)(streamFile.Length % FWDLBlockSize);
                        // If no remainder then it is a full packet
                        if (0 == usSendSize)
                        {
                            usSendSize = FWDLBlockSize;
                        }
                    }

                    streamHeader.Position = 0;
                    streamPSEM.Position = 0;
                    streamPSEM.SetLength(0);
                    streamHeader.WriteTo(streamPSEM);
                    streamPSEM.Write(bybuffer, usIndex * FWDLBlockSize, usSendSize);

                    ProtocolResponse = m_PSEM.OffsetWrite((ushort)idTable,
                                                            usIndex * FWDLBlockSize,
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

        #region Protected Methods
        
        /// <summary>
        /// This method allows derived classes to overried the firmware Type byte that will be passed 
        /// to either the authenticate FWDL procedure or the initiate FWDL procedure.
        /// </summary>
        /// <param name="byCurrentFWType">The firmware image's type.</param>
        /// <returns>The firmware type to use to pass to the authenticate FWDL procedure.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/02/13 jrf 3.50.10          Created.

        protected virtual byte SelectFWTypeByte(byte byCurrentFWType)
        {
            return byCurrentFWType;
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
        // 08/21/06 AF  7.35.00     N/A    Created
        // 09/05/06 AF  7.35.00            Corrected the order of the data in the
        //                                 procedure parameters
        // 10/04/06 AF  7.40.00     N/A    Moved from CENTRON_AMI.cs
        // 07/31/07 KRC 8.10.16     3058   Adding more detailed errors;
        // 01/23/12 RCG 2.53.33 CQ 191589  Adding the Device Class to the parameter list in order to support Gas Ranger Extended FW DL
        // 05/10/12 JJJ 2.60.xx            Tweaking FW Type so Register sees ChoiceConnect FW as RFLAN FW
        // 07/08/13 jrf 2.80.51 TC 15063   Adding special failure case for ICS FW same version.
        // 08/16/13 jrf 2.85.19 WR 420068  Modified special failure case for ICS FW same version.
        // 12/02/13 jrf 3.50.10            Refactored code used to modify the FWType byte to it's own method.
        // 02/07/14 jrf 3.50.32 WR 419257  Modified to use a dynamic FWDL block size based on the negotiated 
        //                                 PSEM packet size.
        //
        private FWDownloadResult EnterFirmwareDownloadMode(string strFilePath)
        {
            //Construct the parameters for mfg proc 37 and execute the procedure
            FileInfo fi = new FileInfo(strFilePath);
            bool bIsThirdPartyFWDownload = (this is M2_Gateway == false) && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_LITHIUM_3_12) >= 0);
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

                if (bIsThirdPartyFWDownload == true)
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

                //Some devices may require this byte to be adjusted.
                byParameters[6] = SelectFWTypeByte(byParameters[6]);

                // image size
                byImageSize = BitConverter.GetBytes((int)fi.Length);
                Array.Copy(byImageSize, 0, byParameters, 11, IMAGE_SIZE_FIELD_LEN);

                byChunkSize = BitConverter.GetBytes(FWDLBlockSize);
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
