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
//                            Copyright © 2013 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The OpenWayITRU class implementation of the IHANInformation interface.
    /// </summary>
    public partial class OpenWayITRU : IHANInformation
    {
        #region Public Methods

        /// <summary>
        ///  Add a HAN Device to the meter and return the Procedure Result with an out offset parameter
        /// </summary>
        /// <param name="deviceEUI">Device EUI</param>
        /// <param name="linkKey">linkKey derived from installation code</param>
        /// <param name="offset">offset received once the procedure is executed successfully, used in order to get the offset</param>
        /// <returns>The result of the procedure</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        public ProcedureResultCodes AddHANDevice(UInt64 deviceEUI, byte[] linkKey, out int offset)
        {
            return m_HANInfo.AddHANDevice(deviceEUI, linkKey, out offset);
            
        }

        /// <summary>
        /// Writes all of the prices and tiers at one time
        /// </summary>
        /// <param name="prices">The list of prices to write to the meter.</param>
        /// <param name="tiers">The list of tiers to write to the meter.</param>
        /// <returns>The result of the write</returns>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  01/27/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        public PSEMResponse PublishFullHANPricing(List<AMIHANPriceEntryRcd> prices,
                                                  List<AMITierLabelEntryRcd> tiers)
        {
            return m_HANInfo.PublishFullHANPricing(prices, tiers);
        }

        /// <summary>
        /// Configure CPP with HAN Pricing Data
        /// </summary>
        /// <param name="uiProviderID">The ID of the provider</param>
        /// <param name="byRateLabelLength">The length of the rate label + 1</param>
        /// <param name="strRateLabel">The label for the rate</param>
        /// <param name="uiIssuerEventID">The issuer event ID</param>
        /// <param name="byUnitOfMeasure">The unit of measure of the rate</param>
        /// <param name="uiCurrency">The currency for the rate</param>
        /// <param name="byPriceTrailingDigit">The trailing digit for the price</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="duration">Duration in minute.  The length must be at least a demand sub-interval length.</param>
        /// <param name="uiPrice">The price being charged</param>
        /// <returns>The CPP configuration result.</returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/27/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        public ConfigCppResult ConfigCppWithHANPricing(UInt32 uiProviderID, byte byRateLabelLength,
            string strRateLabel, UInt32 uiIssuerEventID, byte byUnitOfMeasure, Int16 uiCurrency,
            byte byPriceTrailingDigit, DateTime startTime, UInt16 duration, UInt32 uiPrice)
        {
            return m_HANInfo.ConfigCppWithHANPricing(uiProviderID, byRateLabelLength,
                    strRateLabel, uiIssuerEventID, byUnitOfMeasure, uiCurrency,
                    byPriceTrailingDigit, startTime, duration, uiPrice);
        }

        /// <summary>
        /// Schedules the DR/LC event at the specified time and duration
        /// </summary>
        /// <param name="clientAddress">The Address of the client the message is intended for</param>
        /// <param name="startTime">The time the event should start</param>
        /// <param name="duration">The duration of the event</param>
        /// <param name="deviceClass">The devices that the event should apply to</param>
        /// <param name="eventID">The event ID for the DRLC event</param>
        /// <returns>The result of the operation</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/14 AF  3.50.28 WR444483 Added to the IHANInformation interface
        //
        public ItronDeviceResult ScheduleHANDRLCEvent(ulong clientAddress, DateTime startTime, ushort duration, DRLCDeviceClasses deviceClass, uint eventID)
        {
            return m_HANInfo.ScheduleHANDRLCEvent(clientAddress, startTime, duration, deviceClass, eventID);
        }

        /// <summary>
        /// Cancels the DR/LC event at the specified time
        /// </summary>
        /// <param name="clientAddress">The Address of the client the message is intended for</param>
        /// <param name="EffectiveTime">The time the event should start</param>
        /// <param name="deviceClass">The devices that the event should apply to</param>
        /// <param name="eventID">The event ID for the DRLC event</param>
        /// <param name="cancelControl">Enter 0 to cancel at EffectiveTime, 1 to cancel randomly</param>
        /// <returns>The result of the operation</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/14 AF  3.50.28 WR444483 Added to the IHANInformation interface
        //
        public ItronDeviceResult HANCancelDRLCEvent(ulong clientAddress, DateTime EffectiveTime, DRLCDeviceClasses deviceClass, uint eventID, byte cancelControl)
        {
            return m_HANInfo.HANCancelDRLCEvent(clientAddress, EffectiveTime, deviceClass, eventID, cancelControl);
        }

        /// <summary>
        /// Cancels the DR/LC event at the specified time
        /// </summary>
        /// <param name="clientAddress">The Address of the client the message is intended for</param>
        /// <param name="cancelControl">Enter 0 to cancel at EffectiveTime, 1 to cancel randomly</param>
        /// <returns>The result of the operation</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/28/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        public ItronDeviceResult HANCancelAllDRLCEvent(ulong clientAddress, byte cancelControl)
        {
            return m_HANInfo.HANCancelAllDRLCEvent(clientAddress, cancelControl);
        }

        /// <summary>
        /// Reconfigures the meter to use the specified HAN channels
        /// </summary>
        /// <param name="channels">The channels that should be used by the meter.</param>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/28/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        public PSEMResponse ReconfigureHANChannels(HANChannels channels)
        {
            return m_HANInfo.ReconfigureHANChannels(channels);
        }

        /// <summary>
        /// Publishes a HAN message to any devices bound to the Messaging cluster
        /// </summary>
        /// <param name="MessageID">Number to identify the message (duplicate Message IDs may be ignored by the end devices)</param>
        /// <param name="StartTime">When the message will become active</param>
        /// <param name="DurationInMinutes">How long from the start time the message will be able to be retrieved by an end device</param>
        /// <param name="Destination">Message Transmission values as defined in the ZigBee Smart Energy 
        /// profile spec. Bits 0 and 1 of MSG_CTRL</param>
        /// <param name="Priority"> Message Priority values as defined in the ZigBee Smart Energy 
        /// profile spec. Bits 2 and 3 of MSG_CTRL</param>
        /// <param name="ConfirmationRequired">Whether confirmation is required from end devices upon receipt of this message</param>
        /// <param name="MessageText">The actual text to send to the end device</param>
        /// <returns>PSEM Response from the Table write. (This table write is caught by the Zigbee tick and is not a Procedure)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/12/14 AF  3.50.32 TC16388  Refactored to the IHANInformation interface to make available to the I-210 ITRU
        //
        public PSEMResponse PublishHANMessage(uint MessageID, DateTime StartTime, ushort DurationInMinutes, byte Destination,
                                                AMIHANMsgRcd.MessagePriority Priority, bool ConfirmationRequired, string MessageText)
        {
            return m_HANInfo.PublishHANMessage(MessageID, StartTime, DurationInMinutes, Destination, Priority, ConfirmationRequired, MessageText);
        }

        /// <summary>
        /// Publishes a HAN pricing table to any devices bound to the Pricing cluster by doing a full write
        /// to the Pricing table after a full read and overwriting the price at index - PriceIndex
        /// </summary>
        ///<param name="PriceInfo">An instance of an AMIHANPriceEntryRcd which contains the pricing data to send</param>
        ///<param name="PriceIndex">Index of which price to write over in the HAN pricing table</param>
        /// <returns>PSEM Response from the Table write. (This table write is caught by the Zigbee tick and is not a Procedure)</returns>
        ///Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  02/13/14 AF  3.50.32 WR444483 Refactored to make available to I-210/kV2c
        //
        public PSEMResponse PublishSingleHANPrice(AMIHANPriceEntryRcd PriceInfo, int PriceIndex)
        {
            return m_HANInfo.PublishSingleHANPrice(PriceInfo, PriceIndex);
        }

        /// <summary>
        /// Cancels the message with the specified message ID
        /// </summary>
        /// <param name="messageID">The ID of the message to cancel</param>
        /// <returns>The response from the cancel message</returns>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  02/14/14 AF  3.50.32 WR444483 Refactored to make available to I-210/kV2c
        //
        public PSEMResponse CancelHANMessage(uint messageID)
        {
            return m_HANInfo.CancelHANMessage(messageID);
        }

        /// <summary>
        /// Sets the HAN Security Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        /// <param name="securityMode">The Security Mode byte to set.</param>
        /// <returns>The PSEM Response code.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public PSEMResponse SetHANSecurityMode(byte securityMode)
        {
            return m_HANInfo.SetHANSecurityMode(securityMode);
        }

        /// <summary>
        /// Sets the HAN Device Auth Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        /// <param name="deviceAuthMode">The Device Auth Mode byte to set.</param>
        /// <returns>The PSEM Response code.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public PSEMResponse SetHanDeviceAuthMode(byte deviceAuthMode)
        {
            return m_HANInfo.SetHanDeviceAuthMode(deviceAuthMode);
        }

        /// <summary>
        /// Sets the HAN CBKE Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        /// <param name="cbkeMode">The CBKE Mode byte to set.</param>
        /// <returns>The PSEM Response code.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public PSEMResponse SetHanCbkeMode(byte cbkeMode)
        {
            return m_HANInfo.SetHanCbkeMode(cbkeMode);
        }

        /// <summary>
        /// Sets the Utility Enrollment Group for a HAN device registered to the meter.
        /// </summary>
        /// <param name="clientAddress">The address of the client the message is intended for</param>
        /// <param name="utilityEnrollmentGroup">The new Utility Enrollment Group for the device</param>
        /// <returns>The result of the operation</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/13/14 AF  3.50.32 WR444483 Refactored to make available to I-210/kV2c
        //
        public ItronDeviceResult SetUtilityEnrollmentGroup(ulong clientAddress, byte utilityEnrollmentGroup)
        {
            return m_HANInfo.SetUtilityEnrollmentGroup(clientAddress, utilityEnrollmentGroup);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of Enabled Downstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //  
        public ReadOnlyCollection<DownstreamHANLogEvent> EnabledDownstreamHANLogEvents
        {
            get
            {
                return m_HANInfo.EnabledDownstreamHANLogEvents;
            }
        }

        /// <summary>
        /// Gets the list of Enabled Upstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        public ReadOnlyCollection<UpstreamHANLogEvent> EnabledUpstreamHANLogEvents
        {
            get
            {
                return m_HANInfo.EnabledUpstreamHANLogEvents;
            }
        }

        /// <summary>
        /// Gets the list of Upstream HAN events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        public ReadOnlyCollection<UpstreamHANLogEvent> UpstreamHANLogEvents
        {
            get
            {
                return m_HANInfo.UpstreamHANLogEvents;
            }
        }

        /// <summary>
        /// Gets the list of supported Upstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        public ReadOnlyCollection<UpstreamHANLogEvent> SupportedUpstreamHANLogEvents
        {
            get
            {
                return m_HANInfo.SupportedUpstreamHANLogEvents;
            }
        }

        /// <summary>
        /// Gets the list of Downstream HAN Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        public ReadOnlyCollection<DownstreamHANLogEvent> DownstreamHANLogEvents
        {
            get
            {
                return m_HANInfo.DownstreamHANLogEvents;
            }
        }

        /// <summary>
        /// Gets the list of supported Downstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        public ReadOnlyCollection<DownstreamHANLogEvent> SupportedDownstreamHANLogEvents
        {
            get
            {
                return m_HANInfo.SupportedDownstreamHANLogEvents;
            }
        }

        /// <summary>
        /// Gets the length of HAN upstream event data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        public byte UpstreamHANEventDataLength
        {
            get
            {
                return m_HANInfo.UpstreamHANEventDataLength;
            }
        }

        /// <summary>
        /// Gets the length of HAN downstream event data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //         
        public byte DownstreamHANEventDataLength
        {
            get
            {
                return m_HANInfo.DownstreamHANEventDataLength;
            }
        }

        /// <summary>
        /// Gets the maximum number of HAN upstream event entries.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //         
        public ushort MaxUpstreamHANEventEntries
        {
            get
            {
                return m_HANInfo.MaxUpstreamHANEventEntries;
            }
        }

        /// <summary>
        /// Gets the maximum number of HAN downstream event entries.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //         
        public ushort MaxDownstreamHANEventEntries
        {
            get
            {
                return m_HANInfo.MaxDownstreamHANEventEntries;
            }
        }

        /// <summary>
        /// Gets the Sequence Number for the Last event in the Downstream HAN Log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        public uint DownstreamHANLogLastSequenceNumber
        {
            get
            {
                return m_HANInfo.DownstreamHANLogLastSequenceNumber;
            }
        }

        /// <summary>
        /// Gets the Sequence Number for the Last event in the Upstream HAN Log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //         
        public uint UpstreamHANLogLastSequenceNumber
        {
            get
            {
                return m_HANInfo.UpstreamHANLogLastSequenceNumber;
            }
        }

        /// <summary>
        /// Gets the number of supported DRLC Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        public ushort NumberOfSupportDRLCEvents
        {
            get
            {
                return m_HANInfo.NumberOfSupportDRLCEvents;
            }
        }

        /// <summary>
        /// Gets the Number of Valid DRLC Entries in the Meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  03/17/14 AR  3.50.50          Created.
        //         
        public ushort NumOfValidEntries
        {
            get
            {
                return m_HANInfo.NumOfValidEntries;
            }
        }

        /// <summary>
        /// The HAN price records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        public AMIHANPriceEntryRcd[] HANPrices
        {
            get
            {
                return m_HANInfo.HANPrices;
            }
        }

        /// <summary>
        /// The HAN tier records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.15          Created.
        //         
        public AMITierLabelEntryRcd[] HANTiers
        {
            get
            {
                return m_HANInfo.HANTiers;
            }
        }

        /// <summary>
        /// Gets all the AMI HAN Device Records from table 2130
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.15          Created.
        //         
        public AMIHANDevRcd[] AMIHANDevRecords
        {
            get
            {
                return m_HANInfo.AMIHANDevRecords;
            }
        }

        /// <summary>
        /// Gets all the AMI HAN Response Log records from table 2131
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.15          Created.
        //         
        public AMIHANRspLogRcd[] AMIHANRspLogRecords
        {
            get
            {
                return m_HANInfo.AMIHANRspLogRecords;
            }
        }

        /// <summary>
        /// Gets all the AMI HAN Manufacturer Info records from table 2137
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.15          Created.
        //         
        public AMIHANMfgInfoRcd[] AMIHANMfgInfoRecords
        {
            get
            {
                return m_HANInfo.AMIHANMfgInfoRecords;
            }
        }

        /// <summary>
        /// Gets a dump of the entire Mfg Table 2244.  Table 2244 contains the HAN Diagnostic Status 
        /// Read Information.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/10/13 DLG 3.50.15          Created.
        //
        public AMIHANDiagnosticReadRcd AMIHANDiagnosticReadRecord
        {
            get
            {
                return m_HANInfo.AMIHANDiagnosticReadRecord;
            }
        }

        /// <summary>
        /// Retrieves the number of HAN devices that are currently
        /// joined to the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.15          Created.
        //         
        public byte ActualNumberOfHANDevicesJoined
        {
            get
            {
                return m_HANInfo.ActualNumberOfHANDevicesJoined;
            }
        }

        /// <summary>
        /// Gets the list of DRLC Messages in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        public List<DRLCLogMessage> DRLCMessages
        {
            get
            {
                return m_HANInfo.DRLCMessages;
            }
        }

        /// <summary>
        /// Gets the list of DRLC Messages in the meter includeing the expired ones.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        public List<DRLCLogMessage> DRLCMessagesWithExpired
        {
            get
            {
                return m_HANInfo.DRLCMessagesWithExpired;
            }
        }

        /// <summary>
        /// Get/Set Message Record
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.16          Created.
        //         
        public AMIHANMsgRcd MessageRecord
        {
            get
            {
                return m_HANInfo.MessageRecord;
            }
            set
            {
                m_HANInfo.MessageRecord = value;
            }
        }

        /// <summary>
        /// Reads the startup options out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public byte HANStartupOptions
        {
            get
            {
                return m_HANInfo.HANStartupOptions;
            }
            set
            {
                m_HANInfo.HANStartupOptions = value;
            }
        }

        /// <summary>
        /// Gets the HAN Channels used
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public HANChannels HANChannelsUsed
        {
            get
            {
                return m_HANInfo.HANChannelsUsed;
            }
        }

        /// <summary>
        /// Gets the Simple Metering Multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public uint SimpleMeteringMultiplier
        {
            get
            {
                return m_HANInfo.SimpleMeteringMultiplier;
            }
        }

        /// <summary>
        /// Gets the Simple Metering Divisor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public uint SimpleMeteringDivisor
        {
            get
            {
                return m_HANInfo.SimpleMeteringDivisor;
            }
        }

        /// <summary>
        /// Reads the startup options out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public byte HANConfigVersion
        {
            get
            {
                return m_HANInfo.HANConfigVersion;
            }
        }

        /// <summary>
        /// Reads the HANSecurityProfile out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public string HANSecurityProfile
        {
            get
            {
                return m_HANInfo.HANSecurityProfile;
            }
        }

        /// <summary>
        /// Gets/Sets the HAN Security Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public byte HANSecurityMode
        {
            get
            {
                return m_HANInfo.HANSecurityMode;
            }
            set
            {
                m_HANInfo.HANSecurityMode = value;
            }
        }

        /// <summary>
        /// Gets the HAN Security Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public string HANSecurityModeDescription
        {
            get
            {
                return m_HANInfo.HANSecurityModeDescription;
            }
        }

        /// <summary>
        /// Gets/Sets the HAN Device Auth Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public byte HanDeviceAuthMode
        {
            get
            {
                return m_HANInfo.HanDeviceAuthMode;
            }
        }

        /// <summary>
        /// Gets the HAN Device Auth Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public string HanDeviceAuthModeDescription
        {
            get
            {
                return m_HANInfo.HanDeviceAuthModeDescription;
            }
        }

        /// <summary>
        /// Gets/Sets the HAN CBKE Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public byte HanCbkeMode
        {
            get
            {
                return m_HANInfo.HanCbkeMode;
            }
            set
            {
                m_HANInfo.HanCbkeMode = value;
            }
        }

        /// <summary>
        /// Gets the HAN Security Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        public string HanCbkeModeDescription
        {
            get
            {
                return m_HANInfo.HanCbkeModeDescription;
            }
        }

        /// <summary>
        /// Gets the Inter PAN mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public string InterPANMode
        {
            get
            {
                return m_HANInfo.InterPANMode;
            }
        }

        /// <summary>
        /// Gets the ZigBee key pair. The value will be null if not supported.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/18/14 DLG 4.00.29        Created.
        //
        public List<ZigBeeSecurityKeyPair> ZigBeeKeyPairs
        {
            get
            {
                return m_HANInfo.ZigBeeKeyPairs;
            }

        }

        /// <summary>
        /// Gets the ZigBee output power level.  Should be a value from -30 to 3.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public sbyte? ZigBeePowerLevel
        {
            get
            {
                return m_HANInfo.ZigBeePowerLevel;
            }
        }

        /// <summary>
        /// Gets the Han module version.revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        public string HanModVer
        {
            get
            {
                return m_HANInfo.HanModVer;
            }
        }

        /// <summary>
        /// Gets the Han module build number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.16          Created.
        //         
        public string HanModBuild
        {
            get
            {
                return m_HANInfo.HanModBuild;
            }
        }

        /// <summary>
        /// Gets the Han module type (Zigbee)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.16          Created.
        //         
        public string HanModType
        {
            get
            {
                return m_HANInfo.HanModType;
            }
        }

        /// <summary>
        /// Gets the HAN module version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.16          Created.
        //         
        public float HanModFirmwareVersion
        {
            get
            {
                return m_HANInfo.HanModFirmwareVersion;
            }
        }

        /// <summary>
        /// Gets the HAN module build
        /// </summary>
        //  Revision History	
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/10/13 DLG 3.50.16        Created.
        //
        public byte HANModFirmwareBuild
        {
            get
            {
                return m_HANInfo.HANModFirmwareBuild;
            }
        }

        /// <summary>
        /// String version of the MAC address (in Hex) of the HAN (Zigbee) Server (Electric Meter)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.16          Created.
        //         
        public string HANServerMACAddr
        {
            get
            {
                return m_HANInfo.HANServerMACAddr;
            }
        }

        /// <summary>
        /// Personal area network identifier (in Hex) for the HAN (Zigbee) network
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        public string HANNetworkID
        {
            get
            {
                return m_HANInfo.HANNetworkID;
            }
        }

        /// <summary>
        /// PAN Id for the HAN network
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  02/28/14 AF  3.50.40 WR 462119 Created
        //
        public ushort HANPANID
        {
            get
            {
                return m_HANInfo.HANPANID;
            }
        }

        /// <summary>
        /// The MAC address (in Hex) of the HAN (Zigbee) Server (Electric Meter)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        public UInt64 HANMACAddress
        {
            get
            {
                return m_HANInfo.HANMACAddress;
            }
        }

        /// <summary>
        /// The current HAN (Zigbee) channel number in text format
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        public string HANChannelNbr
        {
            get
            {
                return m_HANInfo.HANChannelNbr;
            }
        }

        /// <summary>
        /// The current HAN (Zigbee) channel number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        public byte HANChannel
        {
            get
            {
                return m_HANInfo.HANChannel;
            }
        }

        /// <summary>
        /// The current HAN (Zigbee) binding entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.16          Created.
        //         
        public List<HANBindingRcd> BindingEntries
        {
            get
            {
                return m_HANInfo.BindingEntries;
            }
        }

        /// <summary>
        /// The HAN divisor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        public UInt32 HANDivisor
        {
            get
            {
                return m_HANInfo.HANDivisor;
            }
        }

        /// <summary>
        /// Gets whether or not the current ZigBee firmware is compatible with the register FW
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        public bool IsZigBeeFirmwareCompatible
        {
            get
            {
                return m_HANInfo.IsZigBeeFirmwareCompatible;
            }
        }

        /// <summary>
        /// Gets the Minimum required ZigBee FW version for the current register FW
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        public float MinZigBeeVersion
        {
            get
            {
                return m_HANInfo.MinZigBeeVersion;
            }
        }

        /// <summary>
        /// Gets whether or not HAN joining is currently enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        public bool IsHANJoiningEnabled
        {
            get
            {
                return m_HANInfo.IsHANJoiningEnabled;
            }
        }

        /// <summary>
        /// Gets the current network status: 0 => Network up; 1 => Network down; 2 => Network forming; 3 => invalid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  03/03/14 AF  3.50.41           Created
        //
        public byte CurrentNetworkStatus
        {
            get
            {
                return m_HANInfo.CurrentNetworkStatus;
            }
        }

        /// <summary>
        /// The HAN multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.xx          Created.
        //         
        public UInt32 HANMultiplier
        {
            get
            {
                return m_HANInfo.HANMultiplier;
            }
        }

        /// <summary>
        /// Gets whether the meter supports HAN.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/03/14 DLG 3.50.18        Created.
        //
        public bool HANSupported
        {
            get
            {
                return m_HANInfo.HANSupported;
            }
        }

        /// <summary>
        /// Gets whether the meter supports HAN event logs.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/03/14 DLG 3.50.18        Created.
        //
        public bool HANEventsSupported
        {
            get
            {
                return m_HANInfo.HANEventsSupported;
            }
        }

        /// <summary>
        /// Gets whether or not C12.18 over ZigBee is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/09/14 jrf 3.50.91 504003 Made part of IHANInformation interface
        public virtual bool IsC1218OverZigBeeEnabled
        {
            get
            {
                return (null != m_HANInfo) && m_HANInfo.IsC1218OverZigBeeEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee Private Profile is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -----------------------------------------------------------------
        // 05/09/14 jrf 3.50.91 504003 Made part of IHANInformation interface
        public virtual bool IsZigBeePrivateProfileEnabled
        {
            get
            {
                return (null != m_HANInfo) && m_HANInfo.IsZigBeePrivateProfileEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/09/14 jrf 3.50.91 504003 Made part of IHANInformation interface
        public virtual bool IsZigBeeEnabled
        {
            get
            {
                bool bEnabled = false;

                bEnabled = (null != m_HANInfo) && m_HANInfo.IsZigBeeEnabled;

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets the HAN App version from table 2529
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created.
        public virtual byte HANAppVersion
        {
            get
            {
                byte Version = 0;

                if (null != Table2529)
                {
                    Version = Table2529.HANAppVersionOnly;
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the HAN App revision from table 2529
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created.
        //
        public virtual byte HANAppRevision
        {
            get
            {
                byte Revision = 0;

                if (null != Table2529)
                {
                    Revision = Table2529.HANAppRevisionOnly;
                }

                return Revision;
            }
        }

        /// <summary>
        /// Gets the HAN App build from table 2529
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created.
        public virtual byte HANAppBuild
        {
            get
            {
                byte Build = 0;

                if (null != Table2529)
                {
                    Build = Table2529.HANAppBuildOnly;
                }

                return Build;
            }
        }

        /// <summary>
        ///  Gets whether or not HAN radio is off
        /// </summary>
        public virtual bool HANRadioOff
        {
            get
            {
                return (null != m_HANInfo && m_HANInfo.IsHANRadioOff);
            }
        }

        #endregion Public Properties
    }
}
