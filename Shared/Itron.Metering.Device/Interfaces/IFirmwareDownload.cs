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
//                           Copyright © 2006 - 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface which needs to be implemented by devices capable of
    /// supporting the download of firmware to the meter. 
    /// </summary>
    /// Revision History
    /// MM/DD/YY Who Version Issue# Description
    /// -------- --- ------- ------ -----------------------------------------
    /// 08/09/06 AF  7.35.00 N/A	Created
    public interface IFirmwareDownload
    {
        /// <summary>
        /// Downloads the firmware file to the meter and activates it.    
        /// On download failure, the pending table is cleared, if possible.
        /// The activation will cause the meter to drop the psem task; therefore,
        /// a log off the meter is necessary after calling this method.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <returns>Result code for firmware downloads</returns>
        FWDownloadResult DownloadFW(string path);

        /// <summary>
        /// Downloads the firmware file to the device but does NOT
        /// activate.  On download failure, the pending table is cleared, if.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <returns>Itron.Metering.Device.FWDownloadResult</returns>
        FWDownloadResult DownloadFWNoActivate(string path);

        /// <summary>
        /// This method just downloads the firmware file blocks to the device for a 
        /// given range of blocks.  Use 1-based indexing for blocks.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <param name="usStartBlock">The first block to download.</param>
        /// <param name="usEndBlock">The last block to download.</param>
        /// <returns>FWDownloadResult</returns>
        FWDownloadResult DownloadFWBlocks(string path, ushort usStartBlock, ushort usEndBlock);

        /// <summary>
        /// Downloads the firmware file to the meter and activates it.    
        /// The activation will cause the meter to drop the psem task so meter log off must
        /// follow this function call on success.  This method supports resuming
        /// a previous failed FWDL attempt.
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
        FWDownloadResult DownloadFW(string path, ref ushort usBlockIndex, bool blnRetry = false, bool blnActivate = true);

    }

}