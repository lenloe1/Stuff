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
    /// The OpenWayITRU class implementation of the IDiagnosticEvents interface.
    /// </summary>
    public partial class OpenWayITRU : IDiagnosticEvents
    {
        #region Public Properties

        /// <summary>
        /// Returns the Diagnostic Events. These events are from the ICS Comm Module simliar to the
        /// "Events" property, however, these events have been filtered to only display the 
        /// diagnostic events and not the customer events.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 01/09/14 DLG 3.50.23 TR 9993   Created. Also related to TREQ 9996.
        // 
        public List<HistoryEntry> DiagnosticEvents
        {
            get
            {
                return m_ICSTableReader.DiagnosticEvents;
            }
        }

        #endregion Public Properties
    }
}