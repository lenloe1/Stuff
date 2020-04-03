///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
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
//                           Copyright © 2006 - 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Resources;
using System.Collections.Generic;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Constructs a dictionary of CENTRON Poly specific events
    /// </summary>
    public class CENTRON_POLY_EventDictionary : ANSIEventDictionary
    {
        /// <summary>Constructs a dictionary of CENTRON Poly specific events</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/08/07 KRC 8.10.07		Created
        /// 07/02/09 AF  2.20.11 135878 Corrected the naming of the pending table
        ///                             cleared event and added the SiteScan error evt
        /// </remarks>
        public CENTRON_POLY_EventDictionary()
            : base()
        {
            Add((int)CANSIDevice.HistoryEvents.CLOCK_RESET, m_rmStrings.GetString("CLOCK_RESET_BY_HOST"));
            Add((int)CANSIDevice.HistoryEvents.SECURITY_FAILED, m_rmStrings.GetString("SECURITY_FAILED"));
            Add((int)CANSIDevice.HistoryEvents.PENDING_TABLE_CLEAR, m_rmStrings.GetString("SITESCAN_ERROR"));
            Add((int)CANSIDevice.HistoryEvents.REVERSE_POWER_FLOW_RESTORE, m_rmStrings.GetString("REVERSE_POWER_FLOW_RESTORE"));
            Add((int)CANSIDevice.HistoryEvents.REVERSE_POWER_FLOW, m_rmStrings.GetString("REVERSE_POWER_FLOW"));
            Add((int)CANSIDevice.HistoryEvents.SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT, m_rmStrings.GetString("CUST_SCHEDULE_CHANGED"));
        }
    }
}
