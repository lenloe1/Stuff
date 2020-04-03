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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The OpenWayITRV class implementation of the IFirmwareDownload interface.
    /// </summary>
    public partial class OpenWayITRV : IFirmwareDownload
    {
        #region Public Methods

        /// <summary>
        /// This method just downloads the firmware file blocks to the device for a 
        /// given range of blocks.  Use 1-based indexing for blocks.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <param name="usStartBlock">The first block to download.</param>
        /// <param name="usEndBlock">The last block to download.</param>
        /// <returns>FWDownloadResult</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  01/06/14 DLG 3.50.19          Created.
        //  
        public FWDownloadResult DownloadFWBlocks(string path, ushort usStartBlock, ushort usEndBlock)
        {
            return m_CommonFWDL.DownloadFWBlocks(path, usStartBlock, usEndBlock);
        }

        /// <summary>
        /// Downloads the firmware file to the device but does NOT
        /// activate.  On download failure, the pending table is cleared, if.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <returns>Itron.Metering.Device.FWDownloadResult</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  01/06/14 DLG 3.50.19          Created.
        //  
        public FWDownloadResult DownloadFWNoActivate(string path)
        {
            return m_CommonFWDL.DownloadFWNoActivate(path);
        }

        /// <summary>
        /// Downloads the firmware file to the meter and activates it.    
        /// On download failure, the pending table is cleared, if possible.
        /// The activation will cause the meter to drop the psem task; therefore,
        /// a log off the meter is necessary after calling this method.
        /// </summary>
        /// <param name="path">Complete path to the firmware file</param>
        /// <returns>Result code for firmware downloads</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  01/06/14 DLG 3.50.19          Created.
        //  
        public FWDownloadResult DownloadFW(string path)
        {
            return m_CommonFWDL.DownloadFW(path);
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
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        // 02/07/14 jrf 3.50.32  WR419257 Created
        public FWDownloadResult DownloadFW(string path, ref ushort usBlockIndex, bool blnRetry = false, bool blnActivate = true)
        {
            return m_CommonFWDL.DownloadFW(path, ref usBlockIndex, blnRetry, blnActivate);
        }

        #endregion Public Methods
    }
}