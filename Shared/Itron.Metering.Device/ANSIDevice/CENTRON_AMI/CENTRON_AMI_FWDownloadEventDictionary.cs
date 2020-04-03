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
//                           Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Resources;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using Itron.Metering.Utilities;
namespace Itron.Metering.Device
{
    
     /// <summary>
    /// Event Dictionary for the Firmware Download Event Log
    /// </summary>
    public class CENTRON_AMI_FWDownloadEventDictionary : Dictionary<ushort, string>
    {
          #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/10/11 MMD                Created
        //  08/19/11 AF  2.52.06        Added third party fw dl activated
        //  05/08/15 jrf 4.20.06 584076 Modified to make FWDL log event id for comm firmware generic.
        //  11/24/15 PGH 4.50.218 REQ574469 Added seal events
        public CENTRON_AMI_FWDownloadEventDictionary()
            : base()
        {
            Add((ushort)FWDownloadLogEvent.FWDownloadLogEventID.RegisterFirmwareActivated, "Register Firmware Activated");
            Add((ushort)FWDownloadLogEvent.FWDownloadLogEventID.CommFirmwareActivated, "Communication Module Firmware Activated");
            Add((ushort)FWDownloadLogEvent.FWDownloadLogEventID.ThirdPartyFirmwareActivated, "Third Party Firmware Activated");
            Add((ushort)FWDownloadLogEvent.FWDownloadLogEventID.AutoSealMeter, "Auto Seal Meter");
            Add((ushort)FWDownloadLogEvent.FWDownloadLogEventID.SealMeter, "Seal Meter");
            Add((ushort)FWDownloadLogEvent.FWDownloadLogEventID.UnsealMeter, "Unseal Meter");
        }

        #endregion
    }
}
