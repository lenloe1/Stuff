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
//                              Copyright © 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Device
{
    public partial class OpenWayAdvPoly
    {
        #region Protected Methods

        /// <summary>
        /// Creates a list of tables to read from the meter.
        /// </summary>
        /// <returns>The list of tables to read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/12/09 RCG	2.20.04			Created

        protected override List<ushort> GetTablesToRead(EDLSections IncludedSections)
        {
            List<ushort> TableList = base.GetTablesToRead(IncludedSections);

            TableList.Add(2091); // SiteScan Toolbox Table

            return TableList;
        }


        #endregion
    }
}
