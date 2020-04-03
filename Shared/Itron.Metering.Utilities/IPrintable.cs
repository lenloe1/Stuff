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
    /// Interface for printable sub controls
    /// </summary>
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 10/10/06 RDB 7.35.00 N/A	   Created
    public interface IPrintable
    {

        /// <summary>
        /// Prints the subcontrol
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/10/06 RDB 7.35.00 N/A	   Created
        void Print();

        /// <summary>
        /// Shows a print preview for the subcontrol
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/10/06 RDB 7.35.00 N/A	   Created
        void PrintPreview();

        /// <summary>
        /// Shows the page setup for the subcontrol
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/10/06 RDB 7.35.00 N/A	   Created
        void PageSetup();

    }//IPrintable
}//Itron.Metering.FieldPro
