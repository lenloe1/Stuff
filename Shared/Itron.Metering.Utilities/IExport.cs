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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Interface for exporting and persisting subcontrol data 
    /// </summary>
    /// <remarks>
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ -------------------------------------------
    /// 10/11/06 MAH 7.35.00 N/A	   Created
    /// </remarks>
    public interface IExportable
    {
        /// <summary>
        /// This method persists all subcontrol data in an XLS 
        /// file.  The user determines the file name and location
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -------------------------------------------
        /// 10/11/06 MAH 7.35.00 N/A	   Created
        /// </remarks>
        void ExportToXLS();

    }
}
