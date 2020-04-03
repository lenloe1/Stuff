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
    /// Interface which needs to be implemented by devices that retrieve HAN information.
    /// Examples are Retreive events, DRLC Msgs, Module Config, Client Info, and Pricing.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue#   Description
    //  -------- --- ------- -------- -------------------------------------------
    //  12/10/13 DLG 3.50.14          Created.
    //  
    public interface IHANInformation
    {
        #region Methods

        /// <summary>
        /// Add a HAN Device to the meter and return the Procedure Result with an out offset parameter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/24/14 AF  3.50.29 WR444483 Added
        //
        ProcedureResultCodes AddHANDevice(UInt64 deviceEUI, byte[] linkKey, out int offset);

        /// <summary>
        /// Writes all of the prices and tiers at one time
        /// </summary>
        /// <param name="prices">The list of prices to write to the meter.</param>
        /// <param name="tiers">The list of tiers to write to the meter.</param>
        /// <returns>The result of the write</returns>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  07/26/11 RCG 2.51.30          Created
        //  12/13/13 DLG 3.50.15          Updated to use HANInformation object to access Table2134.
        //  01/27/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        PSEMResponse PublishFullHANPricing(List<AMIHANPriceEntryRcd> prices,
                                                  List<AMITierLabelEntryRcd> tiers);

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
        // 03/09/11 jrf 2.50.08 n/a    Created.
        // 12/13/13 DLG 3.50.15        Updated to use the HANInformation object to access Table2129.
        // 01/27/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        CANSIDevice.ConfigCppResult ConfigCppWithHANPricing(UInt32 uiProviderID, byte byRateLabelLength,
            string strRateLabel, UInt32 uiIssuerEventID, byte byUnitOfMeasure, Int16 uiCurrency,
            byte byPriceTrailingDigit, DateTime startTime, UInt16 duration, UInt32 uiPrice);

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
        // 04/28/10 RCG 2.40.44			Created
        // 01/28/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        ItronDeviceResult ScheduleHANDRLCEvent(ulong clientAddress, DateTime startTime, ushort duration, DRLCDeviceClasses deviceClass, uint eventID);

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
        // 08/27/13 MP      		   Created
        // 01/28/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        ItronDeviceResult HANCancelDRLCEvent(ulong clientAddress, DateTime EffectiveTime, DRLCDeviceClasses deviceClass, uint eventID, byte cancelControl);

        /// <summary>
        /// Cancels the DR/LC event at the specified time
        /// </summary>
        /// <param name="clientAddress">The Address of the client the message is intended for</param>
        /// <param name="cancelControl">Enter 0 to cancel at EffectiveTime, 1 to cancel randomly</param>
        /// <returns>The result of the operation</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/27/13 MP      		   Created
        // 01/28/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        ItronDeviceResult HANCancelAllDRLCEvent(ulong clientAddress, byte cancelControl);

        /// <summary>
        /// Reconfigures the meter to use the specified HAN channels
        /// </summary>
        /// <param name="channels">The channels that should be used by the meter.</param>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/27/11 RCG 2.51.30        Created
        //  12/17/13 DLG 3.50.16        Updated to use HANInformation object to access table 2106.
        //  01/28/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        PSEMResponse ReconfigureHANChannels(HANChannels channels);

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
        //  02/12/14 AF  3.50.32 TC16388  Added to the IHANInformation interface to make available to the I-210 ITRU
        //
        PSEMResponse PublishHANMessage(uint MessageID, DateTime StartTime, ushort DurationInMinutes, byte Destination,
                                AMIHANMsgRcd.MessagePriority Priority, bool ConfirmationRequired, string MessageText);

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
        //  02/13/14 AF  3.50.32  WR444483 Refactored to make available for I-210/kV2c
        //
        PSEMResponse PublishSingleHANPrice(AMIHANPriceEntryRcd PriceInfo, int PriceIndex);

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
        ItronDeviceResult SetUtilityEnrollmentGroup(ulong clientAddress, byte utilityEnrollmentGroup);

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
        PSEMResponse CancelHANMessage(uint messageID);

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
        PSEMResponse SetHANSecurityMode(byte securityMode);

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
        PSEMResponse SetHanDeviceAuthMode(byte deviceAuthMode);

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
        PSEMResponse SetHanCbkeMode(byte cbkeMode);

        #endregion

        #region Properties

        /// <summary>
        /// Retrieves the number of HAN devices that are currently
        /// joined to the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.15          Created.
        //         
        byte ActualNumberOfHANDevicesJoined
        {
            get;
        }

        /// <summary>
        /// Gets all the AMI HAN Device Records from table 2130
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        AMIHANDevRcd[] AMIHANDevRecords
        {
            get;
        }

        /// <summary>
        /// Gets a dump of the entire Mfg Table 2244.  Table 2244 contains the HAN Diagnostic Status 
        /// Read Information.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.15          Created.
        //         
        AMIHANDiagnosticReadRcd AMIHANDiagnosticReadRecord
        {
            get;
        }

        /// <summary>
        /// Gets all the AMI HAN Manufacturer Info records from table 2137
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        AMIHANMfgInfoRcd[] AMIHANMfgInfoRecords
        {
            get;
        }

        /// <summary>
        /// Gets all the AMI HAN Response Log records from table 2131
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        AMIHANRspLogRcd[] AMIHANRspLogRecords
        {
            get;
        }

        /// <summary>
        /// The current HAN (Zigbee) binding entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.16          Created.
        //         
        List<HANBindingRcd> BindingEntries
        {
            get;
        }

        /// <summary>
        /// Gets the length of HAN downstream event data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //         
        byte DownstreamHANEventDataLength
        {
            get;
        }

        /// <summary>
        /// Gets the list of Downstream HAN Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        ReadOnlyCollection<DownstreamHANLogEvent> DownstreamHANLogEvents
        {
            get;
        }

        /// <summary>
        /// Gets the Sequence Number for the Last event in the Downstream HAN Log
        /// </summary>
        ////  Revision History	
        ////  MM/DD/YY Who Version Issue#   Description
        ////  -------- --- ------- -------- -------------------------------------------
        ////  12/12/13 DLG 3.50.14          Created.
        ////         
        uint DownstreamHANLogLastSequenceNumber
        {
            get;
        }

        /// <summary>
        /// Gets the list of DRLC Messages in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.15          Created.
        //         
        List<DRLCLogMessage> DRLCMessages
        {
            get;
        }

        /// <summary>
        /// Gets the list of DRLC Messages in the meter includeing the expired ones.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.15          Created.
        //         
        List<DRLCLogMessage> DRLCMessagesWithExpired
        {
            get;
        }

        /// <summary>
        /// Gets the list of Enabled Downstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        ReadOnlyCollection<DownstreamHANLogEvent> EnabledDownstreamHANLogEvents
        {
            get;
        }

        /// <summary>
        /// Gets the list of Enabled Upstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        ReadOnlyCollection<UpstreamHANLogEvent> EnabledUpstreamHANLogEvents
        {
            get;
        }

        /// <summary>
        /// The current HAN (Zigbee) channel number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        byte HANChannel
        {
            get;
        }

        /// <summary>
        /// The current HAN (Zigbee) channel number in text format
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        string HANChannelNbr
        {
            get;
        }

        /// <summary>
        /// Gets the HAN Channels used
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        HANChannels HANChannelsUsed
        {
            get;
        }

        /// <summary>
        /// Reads the startup options out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        byte HANConfigVersion
        {
            get;
        }

        /// <summary>
        /// The HAN divisor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        UInt32 HANDivisor
        {
            get;
        }

        /// <summary>
        /// The MAC address (in Hex) of the HAN (Zigbee) Server (Electric Meter)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        UInt64 HANMACAddress
        {
            get;
        }

        /// <summary>
        /// Gets the Han module build number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        string HanModBuild
        {
            get;
        }


        /// <summary>
        /// Gets the HAN module build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        byte HANModFirmwareBuild
        {
            get;
        }

        /// <summary>
        /// Gets the list of supported Upstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        ReadOnlyCollection<UpstreamHANLogEvent> SupportedUpstreamHANLogEvents
        {
            get;
        }

        /// <summary>
        /// Gets the list of supported Downstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        ReadOnlyCollection<DownstreamHANLogEvent> SupportedDownstreamHANLogEvents
        {
            get;
        }

        /// <summary>
        /// Gets the list of Upstream HAN events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        ReadOnlyCollection<UpstreamHANLogEvent> UpstreamHANLogEvents
        {
            get;
        }

        /// <summary>
        /// Gets the length of HAN upstream event data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/11/13 DLG 3.50.14          Created.
        //         
        byte UpstreamHANEventDataLength
        {
            get;
        }

        /// <summary>
        /// Gets the maximum number of HAN upstream event entries.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //         
        ushort MaxUpstreamHANEventEntries
        {
            get;
        }

        /// <summary>
        /// Gets the maximum number of HAN downstream event entries.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //         
        ushort MaxDownstreamHANEventEntries
        {
            get;
        }

        /// <summary>
        /// Gets the Sequence Number for the Last event in the Upstream HAN Log
        /// </summary>
        ////  Revision History	
        ////  MM/DD/YY Who Version Issue#   Description
        ////  -------- --- ------- -------- -------------------------------------------
        ////  12/12/13 DLG 3.50.14          Created.
        ////         
        uint UpstreamHANLogLastSequenceNumber
        {
            get;
        }

        /// <summary>
        /// Gets the Number of Valid DRLC Entries in the Meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        ushort NumberOfSupportDRLCEvents
        {
            get;
        }

        /// <summary>
        /// Gets the number of supported DRLC Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  03/17/14 AR  3.50.50          Created.
        //   
        ushort NumOfValidEntries
        {
            get;
        }

        /// <summary>
        /// The HAN tier records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        AMITierLabelEntryRcd[] HANTiers
        {
            get;
        }

        /// <summary>
        /// The HAN price records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.15          Created.
        //         
        AMIHANPriceEntryRcd[] HANPrices
        {
            get;
        }

        /// <summary>
        /// Get/Set Message Record
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/13/13 DLG 3.50.16          Created.
        //         
        AMIHANMsgRcd MessageRecord
        {
            get;
            set;
        }

        /// <summary>
        /// Reads the startup options out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        byte HANStartupOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Simple Metering Multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        uint SimpleMeteringMultiplier
        {
            get;
        }

        /// <summary>
        /// Gets the Simple Metering Divisor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        uint SimpleMeteringDivisor
        {
            get;
        }

        /// <summary>
        /// Reads the HANSecurityProfile out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        string HANSecurityProfile
        {
            get;
        }

        /// <summary>
        /// Gets/Sets the HAN Security Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        byte HANSecurityMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the HAN Security Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        string HANSecurityModeDescription
        {
            get;
        }

        /// <summary>
        /// Gets/Sets the HAN Device Auth Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        byte HanDeviceAuthMode
        {
            get;
        }

        /// <summary>
        /// Gets the HAN Device Auth Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        string HanDeviceAuthModeDescription
        {
            get;
        }

        /// <summary>
        /// Gets/Sets the HAN CBKE Mode in the HAN Configuration Paramaters Table.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        byte HanCbkeMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the HAN CBKE Mode Description.
        /// </summary>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 06/20/14 DLG 4.00.32 WR 518587 Created.
        // 
        string HanCbkeModeDescription
        {
            get;
        }
        /// <summary>
        /// Gets the Inter PAN mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        string InterPANMode
        {
            get;
        }

        /// <summary>
        /// Gets the ZigBee output power level.  Should be a value from -30 to 3.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        sbyte? ZigBeePowerLevel
        {
            get;
        }

        /// <summary>
        /// Gets the ZigBee key pair. The value will be null if not supported.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/18/14 DLG 4.00.29        Created.
        //
        List<ZigBeeSecurityKeyPair> ZigBeeKeyPairs
        {
            get;
        }

        /// <summary>
        /// Gets the Han module version.revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        string HanModVer
        {
            get;
        }

        /// <summary>
        /// Gets the Han module type (Zigbee)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        string HanModType
        {
            get;
        }

        /// <summary>
        /// Gets the HAN module version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        float HanModFirmwareVersion
        {
            get;
        }

        /// <summary>
        /// String version of the MAC address (in Hex) of the HAN (Zigbee) Server (Electric Meter)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        string HANServerMACAddr
        {
            get;
        }

        /// <summary>
        /// Personal area network identifier (in Hex) for the HAN (Zigbee) network
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        string HANNetworkID
        {
            get;
        }

        /// <summary>
        /// PAN Id for the HAN network. This is the same as the HANNetworkID but is numerical
        /// instead of a string representation.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  02/28/14 AF  3.50.40 WR 462119 Created
        //
        ushort HANPANID
        {
            get;
        }

        /// <summary>
        /// Gets whether the meter supports HAN.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/03/14 DLG 3.50.18        Created.
        //
        bool HANSupported
        {
            get;
        }

        /// <summary>
        /// Gets whether or not the current ZigBee firmware is compatible with the register FW
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        bool IsZigBeeFirmwareCompatible
        {
            get;
        }

        /// <summary>
        /// Gets the Minimum required ZigBee FW version for the current register FW
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        float MinZigBeeVersion
        {
            get;
        }

        /// <summary>
        /// Gets whether or not HAN joining is currently enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/18/13 DLG 3.50.16          Created.
        //         
        bool IsHANJoiningEnabled
        {
            get;
        }

        /// <summary>
        /// The HAN multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/10/13 DLG 3.50.16          Created.
        //         
        UInt32 HANMultiplier
        {
            get;
        }

        /// <summary>
        /// Gets whether the meter supports HAN event logs.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/03/14 DLG 3.50.18        Created.
        //
        bool HANEventsSupported
        {
            get;
        }

        /// <summary>
        /// Gets the current network status: 0 => Network up; 1 => Network down; 2 => Network forming; 3 => invalid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  03/03/14 AF  3.50.41           Created
        //
        byte CurrentNetworkStatus
        {
            get;
        }

        /// <summary>
        /// Gets whether or not ZigBee Private Profile is enabled in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  05/09/14 jrf 3.50.91 WR 504003 Created
        //
        bool IsC1218OverZigBeeEnabled
        {
            get;
        }

        /// <summary>
        /// Gets whether or not C12.18 over ZigBee is enabled in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  05/09/14 jrf 3.50.91 WR 504003 Created
        //
        bool IsZigBeePrivateProfileEnabled
        {
            get;
        }

        /// <summary>
        /// Gets whether or not ZigBee is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  05/09/14 jrf 3.50.91 WR 504003 Created
        bool IsZigBeeEnabled
        {
            get;
        }

        /// <summary>
        /// Gets the HAN App version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created
        //
        byte HANAppVersion
        {
            get;
        }

        /// <summary>
        /// Gets the HAN App revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created
        //
        byte HANAppRevision
        {
            get;
        }

        /// <summary>
        /// Gets the HAN App build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created
        //
        byte HANAppBuild
        {
            get;
        }

        /// <summary>
        ///  Gets whether or not HAN radio is off
        /// </summary>
        bool HANRadioOff
        {
            get;
        }

        #endregion Properties
        }
}
