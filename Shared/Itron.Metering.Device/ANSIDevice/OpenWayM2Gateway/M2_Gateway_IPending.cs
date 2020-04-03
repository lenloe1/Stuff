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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    public partial class M2_Gateway : IPending
    {
        #region Public Methods

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
        //  11/11/10 AF  2.45.12 164490 Created  
        //
        public override bool FWPendingTableDownloadComplete()
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
                                ProcResult = GetFirstTwentyMissingBlocks(FirmwareType.M2GTWY,
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
                    else
                    {
                        // If the procedure did not complete, something must be wrong
                        // - don't allow activation
                        blnDownloadComplete = false;
                    }

                    bytNumberPendingFound++;
                }
            }
            return blnDownloadComplete;
        }

        #endregion
    }
}
