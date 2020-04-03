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
    /// Constructs a dictionary of SENTINEL specific events
    /// </summary>
    internal class SENTINEL_EventDictionary : ANSIEventDictionary
    {
        /// <summary>Constructs a dictionary of SENTINEL specific events</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/11/07 mcm 8.10.04		Created
        /// 07/02/09 AF  2.20.11 135878 Renamed the CUST_SCHED_CHANGED event
        ///                             since this refers to a different event 
        ///                             for OpenWay meters
        /// 07/12/16 MP  4.70.7  WR688986 Changed how event descriptions were accessed
        /// </remarks>
        public SENTINEL_EventDictionary()
            : base()
        {
            // The External Event item is really an Option Board event for the SENTINEL meter.
            this[(int)CANSIDevice.HistoryEvents.EXTERNAL_EVENT] = m_rmStrings.GetString("OPTION_BOARD_EVENT");
            Add((int)CANSIDevice.HistoryEvents.CLOCK_RESET, m_rmStrings.GetString(Enum.GetName(typeof(CANSIDevice.HistoryEvents), CANSIDevice.HistoryEvents.CLOCK_RESET)));
            Add((int)CANSIDevice.HistoryEvents.SECURITY_FAILED, m_rmStrings.GetString(Enum.GetName(typeof(CANSIDevice.HistoryEvents), CANSIDevice.HistoryEvents.SECURITY_FAILED)));
            Add((int)CANSIDevice.HistoryEvents.PENDING_TABLE_CLEAR,m_rmStrings.GetString(Enum.GetName(typeof(CANSIDevice.HistoryEvents), CANSIDevice.HistoryEvents.SITESCAN_ERROR))); //  SITESCAN_ERROR for sentinel
            Add((int)CANSIDevice.HistoryEvents.REVERSE_POWER_FLOW_RESTORE, m_rmStrings.GetString(Enum.GetName(typeof(CANSIDevice.HistoryEvents), CANSIDevice.HistoryEvents.REVERSE_POWER_FLOW_RESTORE)));
            Add((int)CANSIDevice.HistoryEvents.REVERSE_POWER_FLOW, m_rmStrings.GetString(Enum.GetName(typeof(CANSIDevice.HistoryEvents), CANSIDevice.HistoryEvents.REVERSE_POWER_FLOW)));
            Add((int)CANSIDevice.HistoryEvents.SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT, m_rmStrings.GetString("CUST_SCHEDULE_CHANGED")); // keeping hardcoded because different from centron
        }
    }
}
