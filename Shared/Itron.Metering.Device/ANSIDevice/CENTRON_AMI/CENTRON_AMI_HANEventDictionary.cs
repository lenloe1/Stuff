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
//                          Copyright © 2011 - 2013
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
    /// Event Dictionary for the HAN Event Log
    /// </summary>
    public class CENTRON_AMI_DownstreamHANEventDictionary : Dictionary<ushort, string>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  05/25/11 AF  2.50.48 173750 Removed Load Control Queue Updated event - no longer used
        //                              and changed Current Price Published to Price Tier Changed
        //  02/27/12 jrf 2.53.44 183075 Added definition for missing Security Profile Update Notification event.
        //
        public CENTRON_AMI_DownstreamHANEventDictionary()
            : base()
        {
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.MessagePublished, "Message Published");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.PricingDataPublished, "Pricing Data Published");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.DRLCDataPublished, "DRLC Data Published");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.PriceTierChanged, "Price Tier Changed");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.NewDRLCEventDropped, "New DRLC Event Dropped");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.ExistingDRLCEventDropped, "Existing DRLC Event Dropped");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.AttributeWrite, "Attribute Write");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.MeterNetworkChange, "Meter Network Change");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.SecurityProfileUpdateNotification, "Security Profile Update Notification");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.DiagnosticSystemEvent, "Diagnostic System Event");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.MoveOut, "Move Out");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.RIBMeterEvent, "RIB Meter Event");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.RIBCEEvent, "RIB CE Event");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.RIBErrorEvent, "RIB Error Event");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.NextBlockPriceCommitTimeout, "RIB Next Block Price Commit Timeout");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.PriceDisabled, "Price Disable occurred");
            Add((ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.EventCacheOverflow, "Event Cache Overflow");
        }

        #endregion
    }

    /// <summary>
    /// Event Dictionary for the HAN Event Log
    /// </summary>
    public class CENTRON_AMI_UpstreamHANEventDictionary : Dictionary<ushort, string>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  02/27/12 jrf 2.53.44 194023 Adding Management Leave Requested event.
        //  03/20/12 jrf 2.53.51 194023 Reverting previous change.
        //  04/02/13 MSC 2.80.11 TQ6698 Changing message display for Device State Change Event.
        //  08/07/13 AF  2.85.13 WR419080 Replaced EUG with UEG
        //  10/23/13 AF  3.00.22 WR242244 Replaced "HAN Communication has Exceeded Defined Threshold" with
        //                                "HAN Device Not Heard Event"
        //
        public CENTRON_AMI_UpstreamHANEventDictionary()
            : base()
        {
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.MessageConfirmation, "Message Confirmation");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.PriceAcknowledgement, "Price Acknowledgment");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.LoadControlOptOut, "Load Control Opt Out");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.LoadControlStatus, "Load Control Status");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.DeviceStateChange, "HAN Device Status Change");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.DeviceAdded, "Device Added");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.DeviceDropped, "Device Dropped");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.DeviceNotHeard, "HAN Device Not Heard Event");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.DevicePinged, "Ping Event");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.MessageDeliveryFailed, "Message Delivery Failed");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.PriceDeliveryFailed, "Price Delivery Failed");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.DRLCDeliveryFailed, "DRLC Delivery Failed");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.SubmeteringBubbleUpDataMissing, "Submetering Bubble-up Data Missing");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.DeviceUEGChanged, "Device UEG Changed");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.DeviceJoined, "Device Joined");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.JoinedFlagDisabledWithUnjoinedDevice, "Device Not Joined at End of Join Period");
            Add((ushort)UpstreamHANLogEvent.UpstreamHANLogEventID.EventCacheOverflow, "Event Cache Overflow");
       }

        #endregion
    }

}
