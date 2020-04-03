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
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 341 (2389) - MFG Actual Sources Limiting Table
    /// </summary>
    public class OpenWayMFGTable2389 : StdTable11
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.17 N/A    Created

        public OpenWayMFGTable2389(CPSEM psem)
            : base(psem, 2389)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">The binary reader containing the table data</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.17 N/A    Created

        public OpenWayMFGTable2389(PSEMBinaryReader reader)
            : base(reader, 2389)
        {
        }

        #endregion
    }

    /// <summary>
    /// MFG Table 344 (2392) - MFG Data Control Table
    /// </summary>
    public class OpenWayMFGTable2392 : StdTable14
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2389">The table 2389 object for the current device</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.17 N/A    Created

        public OpenWayMFGTable2392(CPSEM psem, OpenWayMFGTable2389 table2389)
            : base(psem, table2389, 2392)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">The binary reader containing the table data</param>
        /// <param name="table2389">The table 2389 object for the current device</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/11 RCG 2.53.17 N/A    Created

        public OpenWayMFGTable2392(PSEMBinaryReader reader, OpenWayMFGTable2389 table2389)
            : base(reader, table2389, 2392)
        {
        }

        #endregion
    }
}
