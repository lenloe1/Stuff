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
//                            Copyright © 2013 - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class to handle all functionality common retrieving HAN Information.
    /// </summary>
    public class HANInformation
    {
        #region Constants

        private const byte MAX_RATE_LABEL_LEN = 12;
        private const byte MIN_RATE_LABEL_LEN = 1;
        private const byte MAX_PRICE_TRAILING_DIGIT_LEN = 15;
        private const byte MIN_PRICE_TRAILING_DIGIT_LEN = 0;

        private const byte INVALID_NETWORK_STATUS = 3;
        private const byte NETWORK_DOWN_STATUS = 1;

        //ICS comm module restricts the offset read size.
        private const ushort ICS_MAX_OFFSET_READ_SIZE = 1400;

        private const byte HAN_DISABLED = 0xFF;

        #endregion


        #region Public Methods

        /// <summary>
        /// Gets the ZigBee key pair. The value will be null if not supported.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created

        public List<ZigBeeSecurityKeyPair> ZigBeeKeyPairs
        {
            get
            {
                List<ZigBeeSecurityKeyPair> Keys = null;

                if (Table2281 != null)
                {
                    try
                    {
                        Keys = Table2281.ZigBeeKeys;
                    }
                    catch (PSEMException e)
                    {
                        if (e.PSEMResponse == PSEMResponse.Isc)
                        {
                            // This probably means that the Meterkey bit that is required to read this table
                            // is not set so just return null to indicate this
                        }
                        else
                        {
                            // There is some unknown error so throw the exception to be handled elsewhere
                            throw;
                        }
                    }
                }

                return Keys;
            }
        }

        /// <summary>
        /// The Constructor. Used to access common HAN methods and properites. Usually instantiated
        /// by classes that are implementing the IHANInformation Interface.
        /// </summary>
        /// <param name="PSEM">The PSEM object to be used in the class.</param>
        /// <param name="ANSIDevice">The ANSIDevice to be used in the class.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 DLG 3.50.14          Created.
        //  
        public HANInformation(CPSEM PSEM, CANSIDevice ANSIDevice)
        {
            m_PSEM = PSEM;
            m_ANSIDevice = ANSIDevice;
        }

        /// <summary>
        /// Add a HAN Device to the meter and return the Procedure Result with an out offset parameter
        /// </summary>
        /// <param name="deviceEUI">Device EUI</param>
        /// <param name="linkKey">linkKey derived from installation code</param>
        /// <param name="offset">offset received once the procedure is executed successfully, used in order to get the offset</param>
        /// <returns>The result of the procedure.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/09/09 MMD 2.30.26        Created
        //  01/27/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        public ProcedureResultCodes AddHANDevice(UInt64 deviceEUI, byte[] linkKey, out int offset)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.COMPLETED;
            byte[] ProcParam = new byte[24];
            byte[] ProcResponse;
            PSEMBinaryWriter PSEMWriter = new PSEMBinaryWriter(new MemoryStream(ProcParam));
            PSEMWriter.Write(deviceEUI);
            PSEMWriter.Write(linkKey);
            byte[] bParam = ProcParam;
            ProcResult = m_ANSIDevice.ExecuteProcedure(Procedures.ADD_AMI_HAN_DEVICE, ProcParam, out ProcResponse);
            if (ProcResult == ProcedureResultCodes.COMPLETED)
            {
                offset = Convert.ToInt32(ProcResponse[8]);
            }
            else
            {
                offset = -1;
            }
            return ProcResult;
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
        //  07/26/11 RCG 2.51.30          Created
        //  12/13/13 DLG 3.50.15          Updated to use HANInformation object to access Table2134.
        //  01/27/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        public PSEMResponse PublishFullHANPricing(List<AMIHANPriceEntryRcd> prices,
                                                  List<AMITierLabelEntryRcd> tiers)
        {
            // Read the table first in case we miss something
            Table2134.Read();

            // Set the Prices
            for (int iIndex = 0; iIndex < Table2134.Prices.Length; iIndex++)
            {
                if (prices != null && iIndex < prices.Count)
                {
                    // We have been given the new price so just load that
                    Table2134.Prices[iIndex] = prices[iIndex];
                }
                else
                {
                    // We don't have a price so lets fill this price with 0's using the constructor
                    Table2134.Prices[iIndex] = new AMIHANPriceEntryRcd();
                }
            }

            // Set the Tiers
            for (int iIndex = 0; iIndex < Table2134.Tiers.Length; iIndex++)
            {
                if (tiers != null && iIndex < tiers.Count)
                {
                    // We have been given the tier so we should use what we were given
                    Table2134.Tiers[iIndex] = tiers[iIndex];
                }
                else
                {
                    // We don't have a tier so set it to all 0's by using the constructor
                    Table2134.Tiers[iIndex] = new AMITierLabelEntryRcd();
                }
            }

            return Table2134.Write();
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
        // 03/09/11 jrf 2.50.08 n/a    Created.
        // 12/13/13 DLG 3.50.15        Updated to use the HANInformation object to access Table2129.
        //
        public CANSIDevice.ConfigCppResult ConfigCppWithHANPricing(UInt32 uiProviderID, byte byRateLabelLength,
            string strRateLabel, UInt32 uiIssuerEventID, byte byUnitOfMeasure, Int16 uiCurrency,
            byte byPriceTrailingDigit, DateTime startTime, UInt16 duration, UInt32 uiPrice)
        {
            CANSIDevice.ConfigCppResult Result = CANSIDevice.ConfigCppResult.ErrorOrCPPInvalidDuration;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            MemoryStream ProcParam = null;
            PSEMBinaryWriter ParamWriter = null;
            TimeSpan tsStartTimeinSecondsSinceUTC2000 = new TimeSpan();
            DateTime dtUTCStartOf2000 = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            try
            {
                ProcParam = new MemoryStream(6);
                ParamWriter = new PSEMBinaryWriter(ProcParam);

                //Make sure rate label length is in bounds.
                if (MAX_RATE_LABEL_LEN < byRateLabelLength)
                {
                    byRateLabelLength = MAX_RATE_LABEL_LEN;
                }
                if (MIN_RATE_LABEL_LEN > byRateLabelLength)
                {
                    byRateLabelLength = MIN_RATE_LABEL_LEN;
                }

                //Make sure price trailing digit length is in bounds.
                if (MAX_PRICE_TRAILING_DIGIT_LEN < byPriceTrailingDigit)
                {
                    byPriceTrailingDigit = MAX_PRICE_TRAILING_DIGIT_LEN;
                }
                if (MIN_PRICE_TRAILING_DIGIT_LEN > byRateLabelLength)
                {
                    byPriceTrailingDigit = MIN_PRICE_TRAILING_DIGIT_LEN;
                }

                //Make sure start date is in UTC
                startTime = startTime.ToUniversalTime();
                tsStartTimeinSecondsSinceUTC2000 = startTime - dtUTCStartOf2000;

                ParamWriter.Write(uiProviderID);
                ParamWriter.Write(byRateLabelLength);

                ParamWriter.Write(strRateLabel, (int)(Table2129.RateLabelLength));

                ParamWriter.Write(uiIssuerEventID);
                ParamWriter.Write(byUnitOfMeasure);
                ParamWriter.Write(uiCurrency);
                ParamWriter.Write(byPriceTrailingDigit);
                ParamWriter.Write((UInt32)tsStartTimeinSecondsSinceUTC2000.TotalSeconds);
                ParamWriter.Write((UInt16)duration);
                ParamWriter.Write(uiPrice);

                ProcResult = m_ANSIDevice.ExecuteProcedure(Procedures.CONFIG_CPP_WITH_HAN_DATA, ProcParam.ToArray(), out ProcResponse);

                switch (ProcResult)
                {
                    case ProcedureResultCodes.COMPLETED:
                        {
                            switch (ProcResponse[0])
                            {
                                case 0:
                                    Result = CANSIDevice.ConfigCppResult.ConfiguredOk;
                                    break;
                                case 1:
                                    Result = CANSIDevice.ConfigCppResult.InvalidStartTime;
                                    break;
                                case 2:
                                    Result = CANSIDevice.ConfigCppResult.ClearedOk;
                                    break;
                                case 3:
                                    Result = CANSIDevice.ConfigCppResult.CppIsActive;
                                    break;
                                default:
                                    Result = CANSIDevice.ConfigCppResult.ErrorOrCPPInvalidDuration;
                                    break;
                            }
                            break;
                        }
                    default:
                        {
                            //General Error
                            Result = CANSIDevice.ConfigCppResult.ErrorOrCPPInvalidDuration;
                            break;
                        }
                }

            }
            finally
            {
                if (null != ProcParam)
                {
                    ProcParam.Dispose();
                }
            }

            return Result;
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
        // 04/28/10 RCG 2.40.44			Created
        // 01/28/14 AF  3.50.28 WR444483 Added to the IHANInformation interface
        //
        public ItronDeviceResult ScheduleHANDRLCEvent(ulong clientAddress, DateTime startTime, ushort duration, DRLCDeviceClasses deviceClass, uint eventID)
        {
            PSEMResponse Response = PSEMResponse.Err;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            HANScheduleDRLCCommand Command = new HANScheduleDRLCCommand(clientAddress, startTime, duration, deviceClass, eventID);

            if (Table2102 != null)
            {
                Table2102.Command = Command;
                Response = Table2102.Write();

                if (Response == PSEMResponse.Ok)
                {
                    Result = ItronDeviceResult.SUCCESS;
                }
                else if (Response == PSEMResponse.Isc)
                {
                    Result = ItronDeviceResult.SECURITY_ERROR;
                }
                else if (Response == PSEMResponse.Onp
                    || Response == PSEMResponse.Iar
                    || Response == PSEMResponse.Sns)
                {
                    Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
                }
            }

            return Result;
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
        // 08/27/13 MP      		   Created
        // 01/28/14 AF  3.50.28 WR444483 Added to the IHANInformation interface
        //
        public ItronDeviceResult HANCancelDRLCEvent(ulong clientAddress, DateTime EffectiveTime, DRLCDeviceClasses deviceClass, uint eventID, byte cancelControl)
        {
            PSEMResponse Response = PSEMResponse.Err;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            HANCancelDRLCCommand Command = new HANCancelDRLCCommand(clientAddress, EffectiveTime, deviceClass, eventID, cancelControl);

            if (Table2102 != null)
            {
                Table2102.Command = Command;
                Response = Table2102.Write();

                if (Response == PSEMResponse.Ok)
                {
                    Result = ItronDeviceResult.SUCCESS;
                }
                else if (Response == PSEMResponse.Isc)
                {
                    Result = ItronDeviceResult.SECURITY_ERROR;
                }
                else if (Response == PSEMResponse.Onp
                    || Response == PSEMResponse.Iar
                    || Response == PSEMResponse.Sns)
                {
                    Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
                }
            }

            return Result;
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
        // 08/27/13 MP      		   Created
        // 01/28/14 AF  3.50.29 WR444483 Added to the IHANInformation interface
        //
        public ItronDeviceResult HANCancelAllDRLCEvent(ulong clientAddress, byte cancelControl)
        {
            PSEMResponse Response = PSEMResponse.Err;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            HANCancelAllDRLCCommand Command = new HANCancelAllDRLCCommand(clientAddress, cancelControl);

            if (Table2102 != null)
            {
                Table2102.Command = Command;
                Response = Table2102.Write();

                if (Response == PSEMResponse.Ok)
                {
                    Result = ItronDeviceResult.SUCCESS;
                }
                else if (Response == PSEMResponse.Isc)
                {
                    Result = ItronDeviceResult.SECURITY_ERROR;
                }
                else if (Response == PSEMResponse.Onp
                    || Response == PSEMResponse.Iar
                    || Response == PSEMResponse.Sns)
                {
                    Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
                }
            }

            return Result;
        }

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
        public PSEMResponse ReconfigureHANChannels(HANChannels channels)
        {
            PSEMResponse Response = PSEMResponse.Onp;

            if (Table2106 != null)
            {
                Table2106.ChannelsUsed = channels;
                Response = Table2106.WriteChannels();
            }

            return Response;
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
            PSEMResponse Response = PSEMResponse.Onp;

            if (Table2106 != null)
            {
                Table2106.SecurityMode = securityMode;
                Response = Table2106.WriteSecurityMode();
            }

            return Response;
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
            PSEMResponse Response = PSEMResponse.Onp;

            if (Table2106 != null)
            {
                Table2106.DeviceAuthMode = deviceAuthMode;
                Response = Table2106.WriteDeviceAuthMode();
            }

            return Response;
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
            PSEMResponse Response = PSEMResponse.Onp;

            if (Table2106 != null)
            {
                Table2106.CbkeMode = cbkeMode;
                Response = Table2106.WriteCbkeMode();
            }

            return Response;
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
        ///Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  06/17/11 WW                   Created
        //  12/13/13 DLG 3.50.15          Updated to use HANInformation object to access Table2133.
        //  12/17/13 DLG 3.50.16          Updated to access a property from the interface instead of 
        //                                the table directly.
        //  02/12/14 AF  3.50.32 TC16388  Added to the IHANInformation interface to make available to the I-210 ITRU
        //  02/20/14 jrf 3.50.36 WR459402 Modifying to save message record to table's property and check for null.
        //
        public PSEMResponse PublishHANMessage(uint MessageID, DateTime StartTime, ushort DurationInMinutes, byte Destination, AMIHANMsgRcd.MessagePriority Priority, bool ConfirmationRequired, string MessageText)
        {
            PSEMResponse Response = PSEMResponse.Err;
            
            if (MessageText.Length > 100)
            {
                throw new ArgumentException("Message Text must be 100 Characters or less: Length is: " +
                    MessageText.Length, "MessageText");
            }

            TimeSpan TimeSpanDuration = new TimeSpan(0, DurationInMinutes, 0);

            if (null != Table2133 && null != Table2133.MessageRecord)
            {
                Table2133.MessageRecord.MessageId = MessageID;
                Table2133.MessageRecord.MessageStart = StartTime;
                Table2133.MessageRecord.Duration = TimeSpanDuration;
                Table2133.MessageRecord.DisplayMessage = MessageText;
                Table2133.MessageRecord.IsConfirmationRequired = ConfirmationRequired;
                Table2133.MessageRecord.MessageLength = (ushort)(MessageText.Length);
                Table2133.MessageRecord.PriorityLevel = Priority;

                Response = Table2133.Write();
            }

            return Response;
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
        //  06/29/11 WW  2.51.15          Created
        //  12/13/13 DLG 3.50.15          Updated to use HANInformation object to access Table2134.
        //  02/13/14 AF  3.50.32 WR444483 Refactored to make available to I-210/kV2c
        //
        public PSEMResponse PublishSingleHANPrice(AMIHANPriceEntryRcd PriceInfo, int PriceIndex)
        {

            Table2134.Read();
            Table2134.Prices[PriceIndex] = PriceInfo;

            return Table2134.Write();
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
        //  06/25/12 RCG 2.60.36        Created
        //  02/13/14 AF  3.50.32 WR444483 Refactored to make available to I-210/kV2c
        //
        public ItronDeviceResult SetUtilityEnrollmentGroup(ulong clientAddress, byte utilityEnrollmentGroup)
        {
            PSEMResponse Response = PSEMResponse.Err;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            HANSetUtilityEnrollmentGroupCommand Command = new HANSetUtilityEnrollmentGroupCommand(clientAddress, utilityEnrollmentGroup);

            if (Table2102 != null)
            {
                Table2102.Command = Command;
                Response = Table2102.Write();

                if (Response == PSEMResponse.Ok)
                {
                    Result = ItronDeviceResult.SUCCESS;
                }
                else if (Response == PSEMResponse.Isc)
                {
                    Result = ItronDeviceResult.SECURITY_ERROR;
                }
                else if (Response == PSEMResponse.Onp
                    || Response == PSEMResponse.Iar
                    || Response == PSEMResponse.Sns)
                {
                    Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
                }
            }

            return Result;
        }

        /// <summary>
        /// Cancels the message with the specified message ID
        /// </summary>
        /// <param name="messageID">The ID of the message to cancel</param>
        /// <returns>The response from the cancel message</returns>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  03/19/12 RCG 2.60.02  TC8883  Created
        //  02/14/14 AF  3.50.32 WR444483 Refactored to make available to I-210/kV2c
        //
        public PSEMResponse CancelHANMessage(uint messageID)
        {
            return PublishHANMessage(messageID, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), ushort.MaxValue, 0, AMIHANMsgRcd.MessagePriority.Low, false, "");
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets the list of Enabled Downstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public ReadOnlyCollection<DownstreamHANLogEvent> EnabledDownstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<DownstreamHANLogEvent> Events = null;

                if (Table2241 != null)
                {
                    Events = Table2241.EnabledDownstreamEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of Enabled Upstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public ReadOnlyCollection<UpstreamHANLogEvent> EnabledUpstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<UpstreamHANLogEvent> Events = null;

                if (Table2241 != null)
                {
                    Events = Table2241.EnabledUpstreamEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of Upstream HAN events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public ReadOnlyCollection<UpstreamHANLogEvent> UpstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<UpstreamHANLogEvent> Events = null;

                if (Table2242 != null)
                {
                    Events = Table2242.Events;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of Downstream HAN Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public ReadOnlyCollection<DownstreamHANLogEvent> DownstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<DownstreamHANLogEvent> Events = null;

                if (Table2243 != null)
                {
                    Events = Table2243.Events;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of supported Upstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public ReadOnlyCollection<UpstreamHANLogEvent> SupportedUpstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<UpstreamHANLogEvent> Events = null;

                if (Table2240 != null)
                {
                    Events = Table2240.SupportedUpstreamEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the list of supported Downstream HAN log events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public ReadOnlyCollection<DownstreamHANLogEvent> SupportedDownstreamHANLogEvents
        {
            get
            {
                ReadOnlyCollection<DownstreamHANLogEvent> Events = null;

                if (Table2240 != null)
                {
                    Events = Table2240.SupportedDownstreamEvents;
                }

                return Events;
            }
        }

        /// <summary>
        /// Gets the Sequence Number for the Last event in the Downstream HAN Log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/05/12 RCG 2.60.00        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public uint DownstreamHANLogLastSequenceNumber
        {
            get
            {
                uint uiValue = 0;

                if (Table2243 != null)
                {
                    uiValue = Table2243.LastSequenceNumber;
                }

                return uiValue;
            }
        }

        /// <summary>
        /// Gets the Sequence Number for the Last event in the Upstream HAN Log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/05/12 RCG 2.60.00        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public uint UpstreamHANLogLastSequenceNumber
        {
            get
            {
                uint uiValue = 0;

                if (Table2243 != null)
                {
                    uiValue = Table2242.LastSequenceNumber;
                }

                return uiValue;
            }
        }

        /// <summary>
        /// Gets the number of supported DRLC Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/16/11 RCG 2.53.08        Created
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation.
        //
        public ushort NumberOfSupportDRLCEvents
        {
            get
            {
                ushort DRLCEvents = 0;

                if (Table2129 != null)
                {
                    DRLCEvents = Table2129.NumberDRLCEvents;
                }

                return DRLCEvents;
            }
        }

        /// <summary>
        /// Gets the Number of Valid DRLC Entries in the Meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/5/13  DG  2.53.08        Created
        //  03/17/14 AR  3.50.50        Moved from CENTRON_AMI to IHANInformation.
        //
        public ushort NumOfValidEntries
        {
            get
            {
                ushort NumOfValidEntries = 0;

                if (Table2132 != null)
                {
                    NumOfValidEntries = Table2132.NumberOfValidEntries;
                }

                return NumOfValidEntries;
            }
        }

        /// <summary>
        /// Gets whether the meter supports HAN.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/08/13 jrf 2.70.66 288156 Created
        //  01/03/14 DLG 3.50.18        Moved from CENTRON_AMI to IHANInformation.
        //
        public bool HANSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == m_ANSIDevice.Table00.IsTableUsed(2098)
                             && true == m_ANSIDevice.Table00.IsTableUsed(2099) 
                             && true == m_ANSIDevice.Table00.IsTableUsed(2102) 
                             && true == m_ANSIDevice.Table00.IsTableUsed(2104));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets whether the meter supports HAN event logs.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/08/13 jrf 2.70.66 288156 Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  01/03/14 DLG 3.50.18        Moved from ANSIDevice to IHANInformation.
        //
        public bool HANEventsSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == m_ANSIDevice.Table00.IsTableUsed(2242) 
                             && true == m_ANSIDevice.Table00.IsTableUsed(2243));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets the list of DRLC Messages in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation.
        //
        public List<DRLCLogMessage> DRLCMessages
        {
            get
            {
                List<DRLCLogMessage> DRLCMessages = null;

                if (Table2132 != null)
                {
                    DRLCMessages = Table2132.DRLCMessages;
                }

                return DRLCMessages;
            }
        }

        /// <summary>
        /// Gets the list of DRLC Messages in the meter includeing the expired ones.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/13 MP        N/A    Created
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation.
        //
        public List<DRLCLogMessage> DRLCMessagesWithExpired
        {
            get
            {
                List<DRLCLogMessage> DRLCMessagesWithExpired = null;

                if (Table2132 != null)
                {
                    DRLCMessagesWithExpired = Table2132.GetDRLCEventsWithExpired();
                }

                return DRLCMessagesWithExpired;
            }
        }

        /// <summary>
        /// The HAN price records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  ??/??/??                      Created.
        //  12/13/13 DLG 3.50.15          Moved from ANSIDevice to HANInformation.
        //  
        public AMIHANPriceEntryRcd[] HANPrices
        {
            get
            {
                return Table2134.Prices;
            }
        }

        /// <summary>
        /// The HAN tier records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  ??/??/??                      Created.
        //  12/13/13 DLG 3.50.15          Moved from ANSIDevice to HANInformation.
        //  
        public AMITierLabelEntryRcd[] HANTiers
        {
            get
            {
                return Table2134.Tiers;
            }
        }

        /// <summary>
        /// Gets all the AMI HAN Device Records from table 2130
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 AF  1.50.36        Created
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to HANInformation.
        //
        public AMIHANDevRcd[] AMIHANDevRecords
        {
            get
            {
                return Table2130.AMIHANDevRcds;
            }
        }

        /// <summary>
        /// Gets all the AMI HAN Response Log records from table 2131
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 AF  1.50.36        Created
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to HANInformation.
        //
        public AMIHANRspLogRcd[] AMIHANRspLogRecords
        {
            get
            {
                return Table2131.AMIHANRspLogRcds;
            }
        }

        /// <summary>
        /// Gets all the AMI HAN Manufacturer Info records from table 2137
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/12/12 PGH 2.70.16        Created
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to HANInformation.
        //
        public AMIHANMfgInfoRcd[] AMIHANMfgInfoRecords
        {
            get
            {
                AMIHANMfgInfoRcd[] mfgInfoRecords = null;

                if (Table2137 != null)
                {
                    mfgInfoRecords = Table2137.AMIHANMfgInfoRcds;
                }

                return mfgInfoRecords;
            }
        }

        /// <summary>
        /// Gets a dump of the entire Mfg Table 2244.  Table 2244 contains the HAN Diagnostic Status 
        /// Read Information.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16        Created
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to HANInformation.
        //
        public AMIHANDiagnosticReadRcd AMIHANDiagnosticReadRecord
        {
            get
            {
                AMIHANDiagnosticReadRcd record = null;

                if (Table2244 != null)
                {
                    record = Table2244.AMIHANDiagnosticReadRecord;
                }

                return record;
            }
        }

        /// <summary>
        /// Retrieves the number of HAN devices that are currently joined to the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/21/09 AF  2.30.01        Created
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to HANInformation.
        //
        public byte ActualNumberOfHANDevicesJoined
        {
            get
            {
                if (null != Table2129)
                {
                    return Table2129.NumberHANClientsJoined;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the length of HAN upstream event data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/23/11 jrf 2.50.06        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public byte UpstreamHANEventDataLength
        {
            get
            {
                byte bytLength = 0;

                if (null != Table2239)
                {
                    bytLength = Table2239.UpstreamDataLength;
                }

                return bytLength;
            }
        }

        /// <summary>
        /// Gets the length of HAN downstream event data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/23/11 jrf 2.50.06        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public byte DownstreamHANEventDataLength
        {
            get
            {
                byte bytLength = 0;

                if (null != Table2239)
                {
                    bytLength = Table2239.DownstreamDataLength;
                }

                return bytLength;
            }
        }

        /// <summary>
        /// Gets the maximum number of HAN upstream event entries.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/23/11 jrf 2.50.06        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public ushort MaxUpstreamHANEventEntries
        {
            get
            {
                ushort usMaxEntries = 0;

                if (null != Table2239)
                {
                    usMaxEntries = Table2239.NumberOfUpstreamLogEntries;
                }

                return usMaxEntries;
            }
        }

        /// <summary>
        /// Gets the maximum number of HAN downstream event entries.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/23/11 jrf 2.50.06        Created
        //  12/09/13 AF  3.50.14	    Class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        public ushort MaxDownstreamHANEventEntries
        {
            get
            {
                ushort usMaxEntries = 0;

                if (null != Table2239)
                {
                    usMaxEntries = Table2239.NumberOfDownstreamLogEntries;
                }

                return usMaxEntries;
            }
        }

        /// <summary>
        /// Get/Set Message Record
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/13 DLG 3.50.16        Created.
        //  02/19/14 jrf 3.50.36 459402 Modified to retrieve this value from table 2133.
        //
        public AMIHANMsgRcd MessageRecord
        {
            get
            {
                AMIHANMsgRcd MsgRcd = null;

                if (null != Table2133)
                {
                    MsgRcd = Table2133.MessageRecord;
                }

                return MsgRcd;
            }
            set
            {
                if (null != Table2133)
                {
                    Table2133.MessageRecord = value;
                }
            }
        }

        /// <summary>
        /// Reads the startup options out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/11/09 AF  2.20.04        Created
        //  03/08/13 MP  2.80.??        Added set to test read only attribute of StartupOptions. Value is read only in meter, this set is just for testing.
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public byte HANStartupOptions
        {
            get
            {
                return Table2106.StartupOptions;
            }
            set
            {
                // For testing only; Meter won't let this value change because it is read-only.
                Table2106.StartupOptions = value;

                //TODO - this won't be written to the meter without a Write.
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
                return Table2106.SecurityMode;
            }
            set
            {
                Table2106.SecurityMode = value;
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
                return Table2106.SecurityModeDescription;
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
                return Table2106.DeviceAuthMode;
            }
            set
            {
                Table2106.DeviceAuthMode = value;
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
                return Table2106.DeviceAuthModeDescription;
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
                return Table2106.CbkeMode;
            }
            set
            {
                Table2106.CbkeMode = value;
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
                return Table2106.CbkeModeDescription;
            }
        }

        /// <summary>
        /// Gets the HAN Channels used
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/12 jrf 2.60.31 TC10006 Created
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        public HANChannels HANChannelsUsed
        {
            get
            {
                HANChannels Channels = HANChannels.None;

                if (Table2106 != null)
                {
                    Channels = Table2106.ChannelsUsed;
                }

                return Channels;
            }
        }

        /// <summary>
        /// Gets the Simple Metering Multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/12 RCG 2.60.28        Created
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public uint SimpleMeteringMultiplier
        {
            get
            {
                uint Multiplier = 1;

                if (Table2106 != null)
                {
                    Multiplier = Table2106.SimpleMeteringMultiplier;
                }

                return Multiplier;
            }
        }

        /// <summary>
        /// Gets the Simple Metering Divisor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/12 RCG 2.60.28        Created
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public uint SimpleMeteringDivisor
        {
            get
            {
                uint Divisor = 1;

                if (Table2106 != null)
                {
                    Divisor = Table2106.SimpleMeteringDivisor;
                }

                return Divisor;
            }
        }

        /// <summary>
        /// Reads the startup options out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/27/13 MP  2.80.??        Created
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public byte HANConfigVersion
        {
            get
            {
                byte bVersion = 0;
                if (Table2106 != null)
                {
                    bVersion = Table2106.ConfigVersion;
                }

                return bVersion;
            }
        }

        /// <summary>
        /// Reads the HANSecurityProfile out of Mfg table 2106
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/09 MMD 2.30.15        Created
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public string HANSecurityProfile
        {
            get
            {
                string strSecurityProfile = "";

                if (Table2106 != null)
                {
                    strSecurityProfile = Table2106.HANSecurityProfile;
                }

                return strSecurityProfile;
            }
        }

        /// <summary>
        /// Gets the Inter PAN mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/06/09 RCG 2.30.16 144719 Created
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public string InterPANMode
        {
            get
            {
                string strInterPANMode = "";

                if (Table2106 != null)
                {
                    strInterPANMode = Table2106.InterPANMode;
                }

                return strInterPANMode;
            }
        }

        /// <summary>
        /// Gets the ZigBee output power level.  Should be a value from -30 to 3.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/08/11 jrf 2.52.00 177455 Created
        //  12/09/13 AF  3.50.14        Class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public sbyte? ZigBeePowerLevel
        {
            get
            {
                sbyte? sbyPowerLevel = null;

                if (Table2106 != null)
                {
                    sbyPowerLevel = Table2106.PowerLevel;
                }

                return sbyPowerLevel;
            }
        }

        /// <summary>
        /// Gets the Han module version.revision
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created
        // 11/13/13 DLG 3.50.03 TR9505 Updated to read value from Table 2529.
        // 12/17/13 DLG 3.50.16        Moved from ICS_Gateway to HANInformation.
        //
        public string HanModVer
        {
            get
            {
                return Table2529.HanModuleVersion;
            }
        }

        /// <summary>
        /// Gets the Han module build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created
        // 11/13/13 DLG 3.50.03 TR9505 Updated to read value from Table 2529.
        //
        public string HanModBuild
        {
            get
            {
                return Table2529.HanModuleBuild;
            }
        }

        /// <summary>
        /// Gets the Han module type (Zigbee)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/18/13 DLG 3.50.16        Created.
        //
        public string HanModType
        {
            get
            {
                return Table2529.HanModuleType;
            }
        }

        /// <summary>
        /// Gets the HAN module version
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/18/13 DLG 3.50.16        Created.
        //
        public float HanModFirmwareVersion
        {
            get
            {
                return Table2529.HANVersionOnly + Table2529.HANRevisionOnly / 1000.0f;
            }
        }

        /// <summary>
        /// Gets the HAN module build
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/18/13 DLG 3.50.16        Created.
        //
        public byte HANModFirmwareBuild
        {
            get
            {
                return Table2529.HANBuildOnly;
            }
        }

        /// <summary>
        /// String version of the MAC address (in Hex) of the HAN (Zigbee) Server (Electric Meter)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ??/??/??                    Created.
        // 12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public string HANServerMACAddr
        {
            get
            {
                return (Table2104.ServerMACAddress).ToString("X16", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Personal area network identifier (in Hex) for the HAN (Zigbee) network
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ??/??/??                    Created.
        // 12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public string HANNetworkID
        {
            get
            {
                return (Table2104.NetworkID).ToString("X4", CultureInfo.InvariantCulture);
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
                if (null != Table2104)
                {
                    return Table2104.NetworkID;
                }
                else
                {
                    return 0xFFFF;
                }
            }
        }

        /// <summary>
        /// The MAC address (in Hex) of the HAN (Zigbee) Server (Electric Meter)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/08/09 AF  2.21.08        Created
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public UInt64 HANMACAddress
        {
            get
            {
                return Table2104.ServerMACAddress;
            }
        }

        /// <summary>
        /// The current HAN (Zigbee) channel number in text format
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ??/??/??                    Created.
        // 12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public string HANChannelNbr
        {
            get
            {
                return (Table2104.ChannelNumber).ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// The current HAN (Zigbee) channel number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/08/09 AF  2.21.08        Created
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public byte HANChannel
        {
            get
            {
                return Table2104.ChannelNumber;
            }
        }

        /// <summary>
        /// The current HAN (Zigbee) binding entries
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ??/??/??                    Created.
        // 12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public List<HANBindingRcd> BindingEntries
        {
            get
            {
                return Table2104.HANBindingEntries;
            }
        }

        /// <summary>
        /// The HAN divisor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ??/??/??                    Created.
        // 12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public UInt32 HANDivisor
        {
            get
            {
                return Table2107.HANDivisor;
            }
        }

        /// <summary>
        /// Gets whether or not the current ZigBee firmware is compatible with the register FW
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/10 RCG 2.41.01	    Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public bool IsZigBeeFirmwareCompatible
        {
            get
            {
                bool bCompatible = true;

                if (Table2107 != null)
                {
                    bCompatible = Table2107.IsZigBeeFWCompatible;
                }

                return bCompatible;
            }
        }

        /// <summary>
        /// Gets the Minimum required ZigBee FW version for the current register FW
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/10 RCG 2.41.01	    Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public float MinZigBeeVersion
        {
            get
            {
                float fMinVersion = 0.0f;

                if (Table2107 != null)
                {
                    fMinVersion = Table2107.MinZigBeeVersion + Table2107.MinZigBeeRevision / 1000.0f;
                }

                return fMinVersion;
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
                if (Table2107 != null)
                {
                    return Table2107.CurrentNetworkStatus;
                }
                else
                {
                    return INVALID_NETWORK_STATUS;
                }
            }
        }

        /// <summary>
        /// Gets whether or not HAN joining is currently enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ??/??/??                    Created.
        // 12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public bool IsHANJoiningEnabled
        {
            get
            {
                return Table2107.IsHANJoiningEnabled;
            }
        }

        /// <summary>
        /// The HAN multiplier
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ??/??/??                    Created.
        // 12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        public UInt32 HANMultiplier
        {
            get
            {
                return Table2107.HANMultiplier;
            }
        }

        /// <summary>
        /// Gets whether or not C12.18 over ZigBee is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/09/14 jrf 3.50.91 504003 Refactored property here for use by multiple devices.
        //
        public bool IsC1218OverZigBeeEnabled
        {
            get
            {
                bool bEnabled = false;

                bEnabled = (Table2193 != null) && Table2193.IsC1218OverZigBeeEnabled;

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee Private Profile is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -----------------------------------------------------------------
        // 05/09/14 jrf 3.50.91 504003 Refactored property here for use by multiple devices.
        public virtual bool IsZigBeePrivateProfileEnabled
        {
            get
            {
                bool bEnabled = false;

                if (Table2193 != null)
                {
                    if (Table2193.IsZigBeePrivateProfileDisabled == true)
                    {
                        bEnabled = false;
                    }
                    else
                    {
                        bEnabled = true;
                    }
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee is enabled in the meter via configuration.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/09/14 jrf 3.50.91 504003 Refactored property here for use by multiple devices.
        public virtual bool IsZigBeeEnabled
        {
            get
            {
                bool bEnabled = false;

                if (Table2193 != null)
                {
                    if (Table2193.IsZigBeeDisabled == true)
                    {
                        bEnabled = false;
                    }
                    else
                    {
                        bEnabled = true;
                    }
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not HAN radio is off
        /// </summary>
        public virtual bool IsHANRadioOff
        {
            get
            {
                bool IsOff = false;

                if (null != Table2106 && HAN_DISABLED == Table2106.StartupOptions
                    && null != Table2107 && (NETWORK_DOWN_STATUS == Table2107.CurrentNetworkStatus || INVALID_NETWORK_STATUS == Table2107.CurrentNetworkStatus))
                {
                    IsOff = true;
                }

                return IsOff;
            }
        }
        
        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Gets the Enhanced security table and creates it if needed. If the meter does not support
        /// this table null will be returned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/09/14 jrf 3.50.91 504003 Created.
        protected OpenWayMFGTable2193 Table2193
        {
            get
            {
                if (null == m_Table2193 && true == m_ANSIDevice.Table00.IsTableUsed(2193))
                {
                    m_Table2193 = new OpenWayMFGTable2193(m_PSEM);
                }

                return m_Table2193;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the Table CHANMfgTable2099 object (Creates it if needed)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/20/06 KRC 8.00.00			Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        internal CHANMfgTable2099 Table2099
        {
            get
            {
                if (null == m_Table2099)
                {
                    m_Table2099 = new CHANMfgTable2099(m_PSEM);
                }

                return m_Table2099;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2106 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/11/09 AF  2.20.04        Created
        //  09/17/09 AF  2.30.02 140524 The size of the table changed starting with
        //                              SR 2.0 SP5.1
        //  04/29/10 AF  2.40.45        Made protected and virtual so that M2 Gateway
        //                              can override
        //  12/09/13 AF  3.50.14        Device class re-architecture - promoted from CENTRON_AMI
        //  12/17/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        internal CHANMfgTable2106 Table2106
        {
            get
            {
                if (null == m_Table2106 && m_ANSIDevice.Table00.IsTableUsed(2106))
                {
                    m_Table2106 = new CHANMfgTable2106(m_PSEM, m_ANSIDevice.FWRevisionForTableCreation);
                }

                return m_Table2106;
            }
        }

        /// <summary>
        /// Gets the Table 2107 object.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07 133864 Changed the table to use tables 2098 and
        //                              2128 per the spec
        //  07/07/10 AF  2.42.02        Added a check for M2 Gateway which also has the
        //                              extra FW info fields
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //  01/02/14 AF  3.50.17 TQ9512 Eliminated the booleans for determining presence of extra fields. We will 
        //                              handle it in the table code rather than here.
        //  06/17/15 AF  4.20.14 591841 Changed the access level to internal
        //
        internal CHANMfgTable2107 Table2107
        {
            get
            {
                if (m_Table2107 == null)
                {
                    m_Table2107 = new CHANMfgTable2107(m_PSEM, m_ANSIDevice.Table00, Table2098, Table2128);
                }

                return m_Table2107;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2129 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 AF  1.50.36        Created
        //  04/21/09 AF  2.20.02 132587 The size of table 2129 changed starting with
        //                              SR 2.0 SP5
        //  04/29/10 AF  2.40.45        Made protected and virtual so that M2 Gateway
        //                              can override
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to HANInformation.
        //
        internal CHANMfgTable2129 Table2129
        {
            get
            {
                if (null == m_Table2129 && m_ANSIDevice.Table00.IsTableUsed(2129))
                {
                    m_Table2129 = new CHANMfgTable2129(m_PSEM,
                                                       m_ANSIDevice.FWRevisionForTableCreation);
                }

                return m_Table2129;
            }
        }

        /// <summary>
        /// Gets the Table 2133 object.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/12/11 MSC 2.51.10 N/A    Created
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation.
        //
        internal CHANMfgTable2133 Table2133
        {
            get
            {
                if (m_Table2133 == null && m_ANSIDevice.Table00.IsTableUsed(2133))
                {
                    m_Table2133 = new CHANMfgTable2133(m_PSEM, Table2129);
                }

                return m_Table2133;
            }
        }

        /// <summary>
        /// Gets the Table 2134 object.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.09        Updated for Smart Pricing
        //  01/31/12 AF  2.53.36 185670 Boron meters do not have the expiration date field
        //  03/08/12 AF  2.53.48 184388 M2 Gateway Lithium meters were not meeting the min f/w version
        //                              check but support the extra bytes.
        //  03/09/12 RCG 2.53.49 195359 The expiration field is being removed from this table as of 3.012.075
        //  12/13/13 DLG 3.50.15        Moved from ANSIDevice to HANInformation.
        //
        internal CHANMfgTable2134 Table2134
        {
            get
            {
                if (m_Table2134 == null && m_ANSIDevice.Table00.IsTableUsed(2134))
                {
                    bool IncludeExpirationDate = VersionChecker.CompareTo(m_ANSIDevice.FWRevision, CANSIDevice.VERSION_LITHIUM_3_12) == 0
                                                 && m_ANSIDevice.FirmwareBuild < 75;

                    if (String.Equals(m_ANSIDevice.MeterType, CANSIDevice.AMI_GATEWAY, StringComparison.OrdinalIgnoreCase))
                    {
                        IncludeExpirationDate = false;
                    }

                    m_Table2134 = new CHANMfgTable2134(m_PSEM, Table2129, IncludeExpirationDate);
                }

                return m_Table2134;
            }
        }

        /// <summary>
        /// Gets the Monitored HAN Events Table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14        Device class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        internal OpenWayMFGTable2241 Table2241
        {
            get
            {
                if (m_Table2241 == null && m_ANSIDevice.Table00.IsTableUsed(2241))
                {
                    m_Table2241 = new OpenWayMFGTable2241(m_PSEM, Table2239);
                }

                return m_Table2241;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the Table CHANMfgTable2098 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        private CHANMfgTable2098 Table2098
        {
            get
            {
                if (null == m_Table2098)
                {
                    m_Table2098 = new CHANMfgTable2098(m_PSEM);
                }

                return m_Table2098;
            }
        }

        /// <summary>
        /// Gets the Table object for Table 2102
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/28/10 RCG 2.40.44			Created
        // 01/28/14 AF  3.50.28 WR444483 Moved from CENTRON_AMI to HANInformation
        //
        private CHANMfgTable2102 Table2102
        {
            get
            {
                if (m_Table2102 == null && m_ANSIDevice.Table00.IsTableUsed(2102))
                {
                    m_Table2102 = new CHANMfgTable2102(m_PSEM);
                }

                return m_Table2102;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2104 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/06 AF  8.00.00        Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        private CHANMfgTable2104 Table2104
        {
            get
            {
                if (null == m_Table2104)
                {
                    m_Table2104 = new CHANMfgTable2104(m_PSEM, Table2099);
                }

                return m_Table2104;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2128 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/09 AF  2.20.07        Created
        //  04/29/10 AF  2.40.45        Made protected and virtual so that M2 Gateway
        //                              can override
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  12/18/13 DLG 3.50.16        Moved from ANSIDevice to HANInformation.
        //
        private CHANMfgTable2128 Table2128
        {
            get
            {
                if (null == m_Table2128 && m_ANSIDevice.Table00.IsTableUsed(2128))
                {
                    m_Table2128 = new CHANMfgTable2128(m_PSEM,
                                                       m_ANSIDevice.FWRevisionForTableCreation,
                                                       m_ANSIDevice.HWRevisionForTableCreation);
                }

                return m_Table2128;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2130 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 AF  1.50.36        Created
        //  08/05/09 AF  2.20.20        Added FW rev param to constructor because
        //                              the table size changed with SR 2.0 SP 5
        //  04/29/10 AF  2.40.45        Made protected and virtual so that M2 Gateway
        //                              can override
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation.
        //
        private CHANMfgTable2130 Table2130
        {
            get
            {
                if (null == m_Table2130 && m_ANSIDevice.Table00.IsTableUsed(2130))
                {
                    m_Table2130 = new CHANMfgTable2130(m_PSEM,
                                                       m_ANSIDevice.FWRevisionForTableCreation,
                                                       Table2129);
                }

                return m_Table2130;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2131 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/08 AF  1.50.36        Created
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation.
        //
        private CHANMfgTable2131 Table2131
        {
            get
            {
                if (null == m_Table2131 && m_ANSIDevice.Table00.IsTableUsed(2131))
                {
                    m_Table2131 = new CHANMfgTable2131(m_PSEM, Table2129);
                }

                return m_Table2131;
            }
        }

        /// <summary>
        /// Gets the Table 2132 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/11 RCG 2.50.01 N/A    Created
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation.
        //
        private CHANMfgTable2132 Table2132
        {
            get
            {
                if (m_Table2132 == null && m_ANSIDevice.Table00.IsTableUsed(2132))
                {
                    m_Table2132 = new CHANMfgTable2132(m_PSEM, m_ANSIDevice.Table00, Table2099,
                                                       Table2129);
                }

                return m_Table2132;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2137 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/12/12 PGH 2.70.16        Created
        //  12/13/13 DLG 3.50.15        Moved from Centron_AMI to HANInformation.
        //
        private CHANMfgTable2137 Table2137
        {
            get
            {
                if (null == m_Table2137 && m_ANSIDevice.Table00.IsTableUsed(2137))
                {
                    m_Table2137 = new CHANMfgTable2137(m_PSEM, Table2129,
                                                       m_ANSIDevice.Table00.STIMESize);
                }

                return m_Table2137;
            }
        }

        /// <summary>
        /// Gets the Actual HAN Log Table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14        Device class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        private OpenWayMFGTable2239 Table2239
        {
            get
            {
                if (m_Table2239 == null && m_ANSIDevice.Table00.IsTableUsed(2239))
                {
                    m_Table2239 = new OpenWayMFGTable2239(m_PSEM);
                }

                return m_Table2239;
            }
        }

        /// <summary>
        /// Gets the Supported HAN Events Table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/09/13 AF  3.50.14        Device class re-architecture - promoted from CENTRON_AMI
        //  12/12/13 DLG 3.50.14        Moved from ANSIDevice to HANInformation.
        //
        private OpenWayMFGTable2240 Table2240
        {
            get
            {
                if (m_Table2240 == null && m_ANSIDevice.Table00.IsTableUsed(2240))
                {
                    m_Table2240 = new OpenWayMFGTable2240(m_PSEM, Table2239);
                }

                return m_Table2240;
            }
        }

        /// <summary>
        /// Gets the Upstream HAN Log Table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/12/13 DLG 3.50.14        Moved from Centron_AMI to HANInformation.
        //  05/06/14 jrf 3.50.91 WR 503747 ICS devices have a max offset read size, so we 
        //                                 need to let the table know because the amount 
        //                                 of data retrieved can get quite large.
        private OpenWayMFGTable2242 Table2242
        {
            get
            {
                if (m_Table2242 == null && m_ANSIDevice.Table00.IsTableUsed(2242))
                {
                    if (m_ANSIDevice is ICS_Gateway)
                    {
                        m_Table2242 = new OpenWayMFGTable2242(m_PSEM, Table2239, m_ANSIDevice.Table00, ICS_MAX_OFFSET_READ_SIZE);
                    }
                    else
                    {
                        m_Table2242 = new OpenWayMFGTable2242(m_PSEM, Table2239, m_ANSIDevice.Table00);
                    }
                }

                return m_Table2242;
            }
        }

        /// <summary>
        /// Gets the Supported HAN Events Table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  12/12/13 DLG 3.50.14        Moved from Centron_AMI to HANInformation.
        //  05/06/14 jrf 3.50.91 WR 503747 ICS devices have a max offset read size, so we 
        //                                 need to let the table know because the amount 
        //                                 of data retrieved can get quite large.
        private OpenWayMFGTable2243 Table2243
        {
            get
            {
                if (m_Table2243 == null && m_ANSIDevice.Table00.IsTableUsed(2243))
                {
                    if (m_ANSIDevice is ICS_Gateway)
                    {
                        m_Table2243 = new OpenWayMFGTable2243(m_PSEM, Table2239, m_ANSIDevice.Table00, ICS_MAX_OFFSET_READ_SIZE);
                    }
                    else
                    {
                        m_Table2243 = new OpenWayMFGTable2243(m_PSEM, Table2239, m_ANSIDevice.Table00);
                    }
                }

                return m_Table2243;
            }
        }

        /// <summary>
        /// Gets the Table CHANMfgTable2244 object (Creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/12 PGH 2.70.16        Created
        //  12/12/13 DLG 3.50.14        Moved from Centron_AMI to HANInformation.
        //
        private CHANMfgTable2244 Table2244
        {
            get
            {
                if (null == m_Table2244 && m_ANSIDevice.Table00.IsTableUsed(2244))
                {
                    m_Table2244 = new CHANMfgTable2244(m_PSEM, m_ANSIDevice.Table00.STIMESize);
                }

                return m_Table2244;
            }
        }

        /// <summary>
        /// Gets the HAN network link key table
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created

        private OpenWayMFGTable2281 Table2281
        {
            get
            {
                if (m_Table2281 == null && m_ANSIDevice.Table00.IsTableUsed(2281))
                {
                    m_Table2281 = new OpenWayMFGTable2281(m_PSEM, Table2098);
                }

                return m_Table2281;
            }
        }

        /// <summary>
        /// Gets the Table 2529 object and creates it if needed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  12/18/13 DLG 3.50.16          Moved from ICS_Gateway to HANInformation.
        //  
        private ICMMfgTable2529CommLanInfo Table2529
        {
            get
            {
                if (m_Table2529 == null)
                {
                    m_Table2529 = new ICMMfgTable2529CommLanInfo(m_PSEM);
                }

                return m_Table2529;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// The PSEM protocol object.
        /// </summary>
        private CPSEM m_PSEM = null;
        /// <summary>
        /// The ANSIDevice object.
        /// </summary>
        private CANSIDevice m_ANSIDevice = null;
        /// <summary>
        /// The table 2098 object. The Dimension HAN Limiting table (read only).
        /// </summary>
        private CHANMfgTable2098 m_Table2098 = null;
        /// <summary>
        /// The table 2099 object. The Actual HAN Limiting table (writable).
        /// </summary>
        private CHANMfgTable2099 m_Table2099 = null;
        /// <summary>
        /// The table 2102 object. The HAN Server Transmit Table (read/write)
        /// </summary>
        private CHANMfgTable2102 m_Table2102 = null;
        /// <summary>
        /// The table 2104 object. The HAN Network Info table (read only).
        /// </summary>
        private CHANMfgTable2104 m_Table2104 = null;
        /// <summary>
        /// The table 2106 object. The HAN Config Parameters table (writable).
        /// </summary>
        protected CHANMfgTable2106 m_Table2106 = null;
        /// <summary>
        /// The table 2107 object. The HAN Statistics table (read only).
        /// </summary>
        private CHANMfgTable2107 m_Table2107 = null;
        /// <summary>
        /// The table 2128 object. The Dimension AMI HAN Limiting table (read only).
        /// </summary>
        protected CHANMfgTable2128 m_Table2128 = null;
        /// <summary>
        /// The table 2129 object. The Actual AMI HAN Limiting table (writable).
        /// </summary>
        private CHANMfgTable2129 m_Table2129 = null;
        /// <summary>
        /// The table 2130 object. The AMI HAN Registration table (read only).
        /// </summary>
        protected CHANMfgTable2130 m_Table2130 = null;
        /// <summary>
        /// The table 2131 object. The AMI HAN Response Log table (read only).
        /// </summary>
        private CHANMfgTable2131 m_Table2131 = null;
        /// <summary>
        /// The table 2132 object. The AMI HAN DRLC Log table (read only).
        /// </summary>
        private CHANMfgTable2132 m_Table2132 = null;
        /// <summary>
        /// The table 2133 object. The AMI HAN Messaging table (writable).
        /// </summary>
        private CHANMfgTable2133 m_Table2133 = null;
        /// <summary>
        /// The table 2134 object. The AMI HAN Pricing table (writable).
        /// </summary>
        private CHANMfgTable2134 m_Table2134 = null;
        /// <summary>
        /// The table 2137 object. The HAN Device Manufacturer Info table (read only).
        /// </summary>
        private CHANMfgTable2137 m_Table2137 = null;
        /// <summary>
        /// The table 2193 object. The security model table (writable).
        /// </summary>
        private OpenWayMFGTable2193 m_Table2193 = null;
        /// <summary>
        /// The table 2239 object. The HAN2 Actual Log table (read only).
        /// </summary>
        private OpenWayMFGTable2239 m_Table2239 = null;
        /// <summary>
        /// The table 2240 object. The HAN2 Event Identification table (read only).
        /// </summary>
        private OpenWayMFGTable2240 m_Table2240 = null;
        /// <summary>
        /// The table 2241 object. The HAN2 Logger Control table (read only).
        /// </summary>
        private OpenWayMFGTable2241 m_Table2241 = null;
        /// <summary>
        /// The table 2242 object. The HAN2 Upstream Logger Data table (read only).
        /// </summary>
        private OpenWayMFGTable2242 m_Table2242 = null;
        /// <summary>
        /// The table 2243 object. The HAN2 Downstream Logger Data table (read only).
        /// </summary>
        private OpenWayMFGTable2243 m_Table2243 = null;
        /// <summary>
        /// The table 2244 object. The HAN Diagnostic Read table (read only).
        /// </summary>
        private CHANMfgTable2244 m_Table2244 = null;
        /// <summary>
        /// The table 2281 object. The Zigbee Link Key Table.
        /// </summary>
        private OpenWayMFGTable2281 m_Table2281 = null;
        /// <summary>
        /// The table 2529 object. The ICS LAN Information table (read only).
        /// </summary>
        private ICMMfgTable2529CommLanInfo m_Table2529;

        #endregion Members
    }
}
