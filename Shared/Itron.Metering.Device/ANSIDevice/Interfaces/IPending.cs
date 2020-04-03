///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface which needs to be implemented by a device capable of 
    /// supporting pending tables
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 09/26/06 AF 7.40.00  N/A    Created
    // 
    public interface IPending
    {
        /// <summary>
        /// Date of last pending table activation
        /// </summary>
        DateTime LastActivationDate
        {
            get;
        }

        /// <summary>
        /// Returns a list representing the pending table activation records
        /// </summary>
        List<PendingEventActivationRecord> PendingTableData
        {
            get;
        }

        /// <summary>
        /// Activates the pending table with the specified Event Record
        /// </summary>
        /// <param name="pendingEvent">The Event Record for the event to activate</param>
        /// <returns>ProcedureResultCodes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        ProcedureResultCodes ActivatePendingTable(PendingEventRecord pendingEvent);

        /// <summary>
        /// Clears the pending table with the specified Event Record
        /// </summary>
        /// <param name="pendingEvent">The Event Record for the event to clear</param>
        /// <returns>ProcedureResultCodes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        ProcedureResultCodes ClearPendingTable(PendingEventRecord pendingEvent);

        /// <summary>
        /// Clears all pending tables from the meter.
        /// </summary>
        /// <returns>The result of the procedure</returns>
        ProcedureResultCodes ClearAllPendingTables();

        /// <summary>
        /// Activates all pending tables in the meter.
        /// </summary>
        /// <returns>The result of the procedure.</returns>
        ProcedureResultCodes ActivateAllPendingTables();

        /// <summary>
        /// Checks std table 04 to see if there is a firmware pending table waiting
        /// activation.
        /// </summary>
        /// <returns>True if a pending firmware table exists in table 04; false,
        /// otherwise</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/01/06 AF  7.40.00 N/A    Created
        //
        bool FWTableStillPendingExists();

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
        //  01/05/07 AF  8.00.05 6      Created for SCR 6, 8.0 Unit Testing database
        //
        bool FWPendingTableDownloadComplete();
    }
}
