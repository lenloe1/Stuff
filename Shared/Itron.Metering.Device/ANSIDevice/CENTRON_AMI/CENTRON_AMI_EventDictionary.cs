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
//                           Copyright © 2006 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Constructs a dictionary of Centron AMI specific events
    /// </summary>
    public class CENTRON_AMI_EventDictionary : ANSIEventDictionary
    {
        #region Constants

        private const UInt32 TAMPER_COUNT_FLAG = 0xFF;

        #endregion

        #region Definitions

        private enum FWLoadFailures : byte
        {
            [EnumDescription("Boot Loader Failed")]
            BootLoaderFailed = 0,
            [EnumDescription("Bad File Size")]
            BadFileSize = 1,
            [EnumDescription("Bad Hardware Revision")]
            BadHWRevision = 2,
            [EnumDescription("Bad Hardware Version")]
            BadHWVersion = 3,
            [EnumDescription("Bad Firmware Type")]
            BadFWType = 4,
            [EnumDescription("Bad CRC")]
            BadCRC = 5,
            [EnumDescription("Retry Failed")]
            RetryFailed = 6,
            [EnumDescription("Insufficient Blocks")]
            InsufficientBlocks = 7,
            [EnumDescription("Download in Progress")]
            DownloadInProgress = 8,
            [EnumDescription("Invalid Hardware Version")]
            InvalidHardwareVersion = 9,
            [EnumDescription("Invalid Firmware Type")]
            InvalidFWType = 10,
            [EnumDescription("ZigBee Hardware Mismatch")]
            ZigBeeHWMismatch = 11,
            [EnumDescription("Comm Module not in Boot Loader Mode")]
            RFLANNotInBootload = 12,
            [EnumDescription("Comm Module Send Interrupted by Powerup")]
            RFLANSendInterruptedByPowerup = 13,
            [EnumDescription("Processing Hash256 Failed")]
            ProcessingHash256Failed = 14,
            [EnumDescription("Authentication Failed")]
            ProcessingAuthenticationFailed = 15,
            [EnumDescription("Bad Signature")]
            BadSignature = 16,
            [EnumDescription("Validation in Progress")]
            ValidateInProgress = 17,
            [EnumDescription("Key not Found")]
            KeyNotFound = 18,
            [EnumDescription("Lockbit Clear Failed")]
            LockbitsClearFailed = 19,
            [EnumDescription("Download Blocked by Fatal Error Recovery")]
            BlockedByCoreDump = 20,
            [EnumDescription("Unknown Comm Module Device Class")]
            CommModuleUnknownClass = 21,
            [EnumDescription("External Flash Write Failed")]
            ExternalFlashWriteFailed = 23,
            [EnumDescription("Invalid Hash")]
            BadHash = 24,
            [EnumDescription("Failure Reason Unknown")]
            FailureReasonUnknown = 255,
        }

        private enum ConnectDisconnectOrigin : byte
        {
            [EnumDescription("Issued Externally")]
            OpticalOrCollectionEngine = 0,
            [EnumDescription("Holding Cap Not Charged")]
            HoldingCapNotCharged = 1,
            [EnumDescription("Issued by Service Limiting")]
            ServiceLimiting = 2,
        }

        private enum ConnectDisconnectReason : byte
        {
            [EnumDescription("Normal Operation")]
            Normal = 0,
            [EnumDescription("Holding Cap Not Charged")]
            HoldingCapNotCharged = 1,
            [EnumDescription("Load Voltage Present")]
            LoadVoltagePresent = 2,
            [EnumDescription("Holdup Cap Not Charged")]
            HoldupCapNotCharged = 3,
            [EnumDescription("Load Voltage Not Detected")]
            LoadVoltageNotDetected = 4,
            [EnumDescription("Connect Retries Exhausted")]
            ConnectRetriesExhausted = 5,
            [EnumDescription("Disconnect Retries Exhausted")]
            DisconnectRetriesExhausted = 6,
            [EnumDescription("Blocked by Failsafe")]
            Failsafe = 7,
            [EnumDescription("Disconnect Retry Successful")]
            DisconnectedDuringRetry = 8,
            [EnumDescription("Connect Retry Successful")]
            ConnectedDuringRetry = 9,
            [EnumDescription("Disconnected with Load Voltage Detected")]
            DisconnectedLoadVoltagePresent = 10,
        }

        private enum LoadVoltagePresentReason : byte
        {
            [EnumDescription("Normal Operation")]
            Normal = 0,
            [EnumDescription("Disconnect Failed: Holding Cap Not Charged")]
            DisconnectFailedHoldingCapNotCharged = 1,
            [EnumDescription("Disconnect Failed: Load Voltage Detected")]
            LoadVoltagePresent = 2,
            [EnumDescription("Connect Failed: Holding Cap Not Charged")]
            HoldupCapNotCharged = 3,
            [EnumDescription("Connect Failed: Load Voltage Not Detected")]
            LoadVoltageNotDetected = 4,
            [EnumDescription("Connect Retries Exhausted")]
            ConnectRetriesExhausted = 5,
            [EnumDescription("Disconnect Retries Exhausted")]
            DisconnectRetriesExhausted = 6,
            [EnumDescription("Disconnect Failed: Blocked by Failsafe")]
            Failsafe = 7,
            [EnumDescription("Disconnect Retry Successful")]
            DisconnectedDuringRetry = 8,
            [EnumDescription("Connect Retry Successful")]
            ConnectedDuringRetry = 9,
            [EnumDescription("Disconnected with Load Voltage Detected")]
            DisconnectedLoadVoltagePresent = 10,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructs a dictionary of Centron AMI specific events
        /// </summary>
        // Revision History	
        // MM/DD/YY who  Version    Issue#     Description
        // -------- ---  -------    ------     ---------------------------------------
        // 05/11/07 mcm  8.10.04  	 	       Created
        // 07/02/09 AF   2.20.11    135878     Corrected the naming of std event 29, 
        //                                        pending tabled cleared event and added
        //                                        SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT, which
        //                                        is the SiteScan Error event for OpenWay Poly Adv
        // 10/12/09 MMD  2.30.09    141987     Removed the Loss of Phase manufactured event from the dictionary
        //                                        as it is no longer used
        // 03/08/11 jrf  2.50.07               Added new HAN event Cache overflow event and removed commented out events.
        // 03/10/11 jrf  2.50.08               Added new CPP event.
        // 04/28/11 AF   2.50.36    172845     Added HAN device added or removed event
        // 05/26/11 AF   2.50.48    174393     Added HAN device added exception and HAN load control opt out exception
        // 06/03/11 AF   2.50.05    175033     Added Extended Outage Recovery event
        // 08/11/11 jrf  2.52.02    TREQ2709   Changing Register Firmware Download Status event
        //                                       to Firmware Download Event Log Full event.
        // 11/28/11 jrf  2.52.02    TC5361     Added new voltage monitoring events.
        // 09/20/12 jrf  2.70.18    TQ6658     Added magnetic tamper events.
        // 10/12/12 MSC  2.70.28    TQ6684     Added Support for Power Up and Power Down events.
        // 04/30/13 PGH  2.80.24    327121     Added additional events common to the CE for use with the End-Point-Server
        // 05/30/14 jrf  3.50.97    517744     The name of mfg. event 163 changed from HAN Load Control Event Sent to ICS ERT Event.
        // 02/10/15 AF   4.10.03    561372     Added the HAN Next Block Price Commit Timeout event
        // 02/04/16 PGH  4.50.226   RTT556309  Added Temperature events
        // 05/12/16 MP   4.50.266   WR685323   Changed ON_DEMAND_PERIOD_READ to ON_DEMAND_PERIODIC_READ to match ANSIDeviceStrings resource file
        // 05/20/16 MP   4.50.270   WR685690   Added support for EVENT_HARDWARE_ERROR_DETECTION
        // 06/15/16 MP   4.50.284   WR680128   Removed On Demand Periodic Read to move to base dictionary
        // 07/12/16 MP   4.70.7     WR688986   Changed how event descriptions were accessed
        // 07/14/16 MP   4.70.7     WR688986   Removed commented code
        //  10/05/16 AF  4.70.21    WR716010    Added check to make sure event is not already in the list
        //                                      and check the description for null in case it's not in the enum
        //
        public CENTRON_AMI_EventDictionary()
            : base()
        {
            foreach (CANSIDevice.HistoryEvents Event in Enum.GetValues(typeof(CANSIDevice.HistoryEvents)))
            {
                if (!ContainsKey((int)Event))
                {
                    string strEventDescription = m_rmStrings.GetString(Enum.GetName(typeof(CANSIDevice.HistoryEvents), Event));

                    if (null == strEventDescription)
                    {
                        strEventDescription = m_rmStrings.GetString("UNKNOWN_EVENT");
                    }

                    Add((int)Event, strEventDescription);
                }
            }
        }

        /// <summary>
        /// TranslateEventData - Takes the Event Data and translates it to something human readable
        /// </summary>
        /// <param name="HistoryEvent">The Event Code of the Event the Data belongs to</param>
        /// <param name="ArgumentReader">The raw data we are translating</param>
        /// <returns>The Human Readable text</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/01/08 KRC 1.50.20		   Created
        // 04/27/09 AF  2.20.03 133061 Exchanged the case statements for METER_DWLD_FAILED and
        //                             METER_FW_DWLD_SUCCEDED.  So that the processing is correct
        //                             for the event.
        // 07/02/09 AF  2.20.11 135878 Corrected the naming of the pending table cleared event
        // 03/11/10 RCG 2.40.24 145140 Fixing translation issues with FW DL events
        // 03/10/11 jrf 2.50.08        Added interpretation of CPP event data.
        //                             Checking ArgumentReader for null to quiet compiler warning.
        // 08/11/11 jrf 2.52.02 TREQ2709 Removing interpretation of obsolete Register Firmware Download Status event
        //                                argument data.
        // 11/28/11 jrf 2.52.02 TC5361 Added translation of new voltage monitoring event data.
        // 02/16/12 jrf 2.53.41 TREQ2901 Added translation of load voltage present event data.
        // 03/15/12 jrf 2.53.50 TREQ5571 Switched to pass as argument a HistoryEntry instead of just 
        //                               the history code.  Also setting argument data for demand reset event
        //                               based on the user ID of the event.
        // 03/22/12 RCG 2.53.51 195340 Updating the CPP Event arguments to use the enum description retriever
        // 03/28/12 jrf 2.53.52 196071 Adding a demand reset argument of "Initiated Locally" when user ID is 
        //                             2-65534 inclusive.
        // 09/20/12 jrf 2.70.18 TQ6658 Translating the Magnetic Tamper Detect event argument.
        // 03/20/13 MSC 2.80.08 TR7477 Added Support for the Full Scale Exceeded Event
        // 05/06/13 MSC 2.80.25 TR7477 Updated Argument Data to be more understandable for the user.
        // 06/07/13 MSC 2.80.36 TR7477 Some format changes for CTE Event display from Unit Test.
        // 06/13/13 AF  2.80.37 TR7477 Modified the CTE Event display
        // 06/14/13 AF  2.80.38 TR7477 Modified CTE display again to make it consistent with other events that show phase(s)
        // 10/03/13 AF  3.00.13 WR391210 Adding more Generic event sub events and argument translations
        // 05/27/15 AF  4.20.08 WR581985 Added the ICS_ERT_EVENT (aka ERT_242_COMMAND_REQUEST)
        // 02/04/16 PGH 4.50.226 RTT556309 Added Temperature events
        // 05/03/16 PGH 4.50.260 680221 Argument data backwards for temperature events (Re: 677881)
        // 07/29/16 MP   4.70.11  WR704220   Added support for events 213 (WRONG_CONFIG_CRC)
        // 09/01/16 jrf 4.70.16   WI708332 Added subevent 9 for generic event and updated event arguments to not reflect the subevent name,
        //                                 since the generic event is now renamed to the sub event name per the InterceptEventDescription(...)
        //                                 method.
        // 11/10/16 PGH 4.70.33   WR726575 Added Event Logging Suspended and Logging Resumed
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)")]
        public override string TranslatedEventData(HistoryEntry HistoryEvent, PSEMBinaryReader ArgumentReader)
        {
            String strData = "";

            if (null != ArgumentReader)
            {
                switch (HistoryEvent.HistoryCode)
                {
                    case (ushort)(CANSIDevice.HistoryEvents.THIRD_PARTY_HAN_FW_DWLD_STATUS):
                    {
                        byte FWType = ArgumentReader.ReadByte();
                        byte Status = ArgumentReader.ReadByte();

                        if (Enum.IsDefined(typeof(FirmwareType), FWType))
                        {
                            strData += EnumDescriptionRetriever.RetrieveDescription((FirmwareType)FWType);
                        }
                        else
                        {
                            // We don't have a definition for this failure type so add a generic error
                            strData += "Unknown FW Type " + FWType.ToString(CultureInfo.CurrentCulture);
                        }

                        strData += " - ";

                        switch (Status)
                        {
                            case 0x01:
                            strData += "Bad File Size";
                            break;
                            case 0x02:
                            strData += "Bad Hardware Revision";
                            break;
                            case 0x03:
                            strData += "Bad Hardware Version";
                            break;
                            case 0x04:
                            strData += "Bad Firmware Type";
                            break;
                            case 0x05:
                            strData += "Bad CRC";
                            break;
                            case 0x06:
                            strData += "Retry Failed";
                            break;
                            case 0x07:
                            strData += "Insufficient Blocks";
                            break;
                            case 0x08:
                            strData += "Download in Progress";
                            break;
                            case 0x0E:
                            strData += "Invalid Device Class";
                            break;
                            case 0x80:
                            strData += "Init Complete - Version Running";
                            break;
                            case 0x81:
                            strData += "Init Complete - Version Downloaded";
                            break;
                            case 0x82:
                            strData += "No Download Active";
                            break;
                            case 0x86:
                            strData += "Voltage Out of Range";
                            break;
                            case 0x88:
                            strData += "Flash Write Failure";
                            break;
                            case 0x8C:
                            strData += "Pending";
                            break;
                            case 0x8D:
                            strData += "Download Setup";
                            break;
                            case 0x8E:
                            strData += "Download Completed";
                            break;
                            case 0x8F:
                            strData += "Activation Sent";
                            break;
                            case 0x90:
                            strData += "Activating";
                            break;
                            case 0x91:
                            strData += "Pause Sent";
                            break;
                            case 0x92:
                            strData += "Paused";
                            break;
                            case 0x93:
                            strData += "First Timeout";
                            break;
                            case 0x94:
                            strData += "Second Timeout";
                            break;
                            case 0x95:
                            strData += "Ready for Activation";
                            break;
                            case 0x96:
                            strData += "Head End Cancellation Sent";
                            break;
                            case 0x97:
                            strData += "Head End Cancellation";
                            break;
                        }
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.METER_FW_DWLD_SUCCEDED):
                    {
                        byte FWType = ArgumentReader.ReadByte();
                        byte Status = ArgumentReader.ReadByte();
                        byte Version = ArgumentReader.ReadByte();
                        byte Revision = ArgumentReader.ReadByte();
                        byte Build = ArgumentReader.ReadByte();

                        if (Enum.IsDefined(typeof(FirmwareType), FWType))
                        {
                            strData += EnumDescriptionRetriever.RetrieveDescription((FirmwareType)FWType);
                        }
                        else
                        {
                            // We don't have a definition for this failure type so add a generic error
                            strData += "Unknown FW Type " + FWType.ToString(CultureInfo.CurrentCulture);
                        }

                        strData += " - ";

                        switch (Status)
                        {
                            case 0:
                            {
                                strData += "Initiated";
                                break;
                            }
                            case 1:
                            {
                                strData += "Success; ";

                                // If the version is all zeros, then don't show anything, else show the version
                                if (0 == Version && 0 == Revision && 0 == Build)
                                {
                                    strData = "";
                                }
                                else
                                {
                                    strData = "Version: ";
                                    strData += Version.ToString("d3", CultureInfo.InvariantCulture);
                                    strData += "." + Revision.ToString("d3", CultureInfo.InvariantCulture);
                                    strData += "." + Build.ToString("d3", CultureInfo.InvariantCulture);
                                }
                                break;
                            }
                        }
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.METER_DWLD_FAILED):
                    {
                        byte FWType = ArgumentReader.ReadByte();
                        byte FailureStage = ArgumentReader.ReadByte();
                        byte FailureDetails = ArgumentReader.ReadByte();


                        if (Enum.IsDefined(typeof(FirmwareType), FWType))
                        {
                            strData += EnumDescriptionRetriever.RetrieveDescription((FirmwareType)FWType);
                        }
                        else
                        {
                            // We don't have a definition for this failure type so add a generic error
                            strData += "Unknown FW Type " + FWType.ToString(CultureInfo.CurrentCulture);
                        }

                        strData += " - ";

                        switch (FailureStage)
                        {
                            case 0:
                            strData += "Download Initiation Failed; ";
                            break;
                            case 1:
                            strData += "Download Terminated; ";
                            break;
                            case 2:
                            strData += "Download Aborted; ";
                            break;
                            case 3:
                            strData += "Download Failed; ";
                            break;
                        }

                        if (Enum.IsDefined(typeof(FWLoadFailures), FailureDetails))
                        {
                            strData += EnumDescriptionRetriever.RetrieveDescription((FWLoadFailures)FailureDetails);
                        }
                        else
                        {
                            // We don't have a definition for this failure type so add a generic error
                            strData += "Unknown Failure " + FailureDetails.ToString(CultureInfo.CurrentCulture);
                        }

                        break;
                    }

                    case (ushort)(CANSIDevice.HistoryEvents.PENDING_TABLE_ACTIVATION):
                    case (ushort)(CANSIDevice.HistoryEvents.PENDING_TABLE_CLEAR):
                    case (ushort)(CANSIDevice.HistoryEvents.PENDING_TABLE_CLEAR_FAIL):
                    case (ushort)(CANSIDevice.HistoryEvents.FIRMWARE_PENDING_TABLE_FULL):
                    case (ushort)(CANSIDevice.HistoryEvents.TABLE_WRITTEN):
                    {
                        string Data = ArgumentReader.ReadInt16().ToString(CultureInfo.InvariantCulture);
                        int TableValue = int.Parse(Data);

                        if (TableValue == 122)
                        {
                            strData = "Network Address has been updated.";
                        }
                        else if (TableValue == 123)
                        {
                            strData = "Application Group membership has been updated.";
                        }
                        else
                        {
                            strData = "Table: ";
                            strData += Data;
                        }
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.REG_DWLD_SUCCEEDED):
                    case (ushort)(CANSIDevice.HistoryEvents.ZIGBEE_DWLD_SUCCEEDED):
                    case (ushort)(CANSIDevice.HistoryEvents.RFLAN_DWLD_SUCCEEDED):
                    {
                        byte MajorVersion = 0;
                        byte MinorVersion = 0;
                        byte BuildNumber = 0;

                        MajorVersion = ArgumentReader.ReadByte();
                        MinorVersion = ArgumentReader.ReadByte();
                        BuildNumber = ArgumentReader.ReadByte();

                        // If the version is all zeros, then don't show anything, else show the version
                        if (0 == MajorVersion && 0 == MinorVersion && 0 == BuildNumber)
                        {
                            strData = "";
                        }
                        else
                        {
                            strData = "Version: ";
                            strData += MajorVersion.ToString("d3", CultureInfo.InvariantCulture);
                            strData += "." + MinorVersion.ToString("d3", CultureInfo.InvariantCulture);
                            strData += "." + BuildNumber.ToString("d3", CultureInfo.InvariantCulture);
                        }

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.REG_DWLD_FAILED):
                    case (ushort)(CANSIDevice.HistoryEvents.ZIGBEE_DWLD_FAILED):
                    case (ushort)(CANSIDevice.HistoryEvents.RFLAN_DWLD_FAILED):
                    case (ushort)(CANSIDevice.HistoryEvents.REG_DWLD_INITIATION_FAILED):
                    case (ushort)(CANSIDevice.HistoryEvents.ZIGBEE_DWLD_INITIATION_FAILED):
                    case (ushort)(CANSIDevice.HistoryEvents.RFLAN_DWLD_INITIATION_FAILED):
                    {
                        byte byArgument = ArgumentReader.ReadByte();

                        if (Enum.IsDefined(typeof(FWLoadFailures), byArgument))
                        {
                            strData += EnumDescriptionRetriever.RetrieveDescription((FWLoadFailures)byArgument);
                        }
                        else
                        {
                            // We don't have a definition for this failure type so add a generic error
                            strData += "Unknown Failure " + byArgument.ToString(CultureInfo.CurrentCulture);
                        }

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.FATAL_ERROR):
                    {
                        byte FatErrorBitField = ArgumentReader.ReadByte();
                        byte RecoveryStatusBitField = ArgumentReader.ReadByte();
                        ushort ReasonCode = ArgumentReader.ReadUInt16();

                        strData = "Fatal Error: ";

                        // This argument is returned as a bit field so we need to extract which
                        // fatal errors are set. Bit 0 is fatal 1, bit 1 is fatal 2, etc.
                        for (int iBitIndex = 0; iBitIndex < 7; iBitIndex++)
                        {
                            byte CurrentMask = (byte)(0x01 << iBitIndex);

                            if ((FatErrorBitField & CurrentMask) == CurrentMask)
                            {
                                strData += (iBitIndex + 1).ToString(CultureInfo.CurrentCulture) + " ";
                            }
                        }

                        strData += " Reason: " + ReasonCode.ToString("X4", CultureInfo.CurrentCulture);
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.ZIGBEE_FW_DWLD_STATUS):
                    case (ushort)(CANSIDevice.HistoryEvents.RFLAN_FW_DWLD_STATUS):
                    {
                        strData = "Blocks Received: ";
                        strData += ArgumentReader.ReadInt16().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.FIRMWARE_DWLD_ABORTED):
                    {
                        strData = "Firmware Type: ";
                        int iArgument = ArgumentReader.ReadInt16();

                        if (iArgument == 0)
                        {
                            strData += "Register";
                        }
                        else if (iArgument == 1)
                        {
                            strData += "RFLAN";
                        }
                        else if (iArgument == 2)
                        {
                            strData += "HAN";
                        }
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.SERVICE_LIMITING_ACTIVE_TIER_CHANGED):
                    {
                        strData = "Tier: ";
                        byte byArgument = ArgumentReader.ReadByte();

                        if (byArgument == 0)
                        {
                            strData += "Normal";
                        }
                        else if (byArgument == 1)
                        {
                            strData += "Critical";
                        }
                        else if (byArgument == 2)
                        {
                            strData += "Emergency";
                        }
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.REMOTE_CONNECT_FAILED):
                    case (ushort)(CANSIDevice.HistoryEvents.REMOTE_DISCONNECT_FAILED):
                    case (ushort)(CANSIDevice.HistoryEvents.REMOTE_CONNECT_RELAY_INITIATED):
                    case (ushort)(CANSIDevice.HistoryEvents.REMOTE_CONNECT_RELAY_ACTIVATED):
                    case (ushort)(CANSIDevice.HistoryEvents.REMOTE_DISCONNECT_RELAY_ACTIVATED):
                    {
                        // Reading first parameter byte
                        byte byArgument = ArgumentReader.ReadByte();
                        byte byArgument2 = ArgumentReader.ReadByte();

                        strData = "Origin: ";

                        if (Enum.IsDefined(typeof(ConnectDisconnectOrigin), byArgument))
                        {
                            strData += EnumDescriptionRetriever.RetrieveDescription((ConnectDisconnectOrigin)byArgument);
                        }
                        else
                        {
                            // We don't have a definition for this failure type so add a generic error
                            strData += "Unknown Origin " + byArgument.ToString(CultureInfo.CurrentCulture);
                        }

                        strData += " Reason: ";

                        if (Enum.IsDefined(typeof(ConnectDisconnectReason), byArgument2))
                        {
                            strData += EnumDescriptionRetriever.RetrieveDescription((ConnectDisconnectReason)byArgument2);
                        }
                        else
                        {
                            // We don't have a definition for this failure type so add a generic error
                            strData += "Unknown Reason " + byArgument2.ToString(CultureInfo.CurrentCulture);
                        }

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.C12_22_RFLAN_ID_CHANGED):
                    {
                        strData = "New Cell ID: " + ArgumentReader.ReadInt32().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.VOLT_RMS_ABOVE_THRESHOLD):
                    case (ushort)(CANSIDevice.HistoryEvents.RMS_VOLTAGE_BELOW_LOW_THRESHOLD):
                    case (ushort)(CANSIDevice.HistoryEvents.RMS_VOLTAGE_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL):
                    case (ushort)(CANSIDevice.HistoryEvents.RMS_VOLTAGE_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL):
                    {
                        string strMaxVoltage = ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        string strMinVoltage = ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        byte byPhase = ArgumentReader.ReadByte();
                        string strPhase = "";

                        if (byPhase == 0)
                        {
                            strPhase = "; Phase (A)";
                        }
                        else if (byPhase == 1)
                        {
                            strPhase = "; Phase (B)";
                        }
                        else if (byPhase == 2)
                        {
                            strPhase = "; Phase (C)";
                        }

                        strData = "Min Voltage: " + strMinVoltage + "; Max Voltage: " + strMaxVoltage + strPhase;

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.VOLT_HOUR_ABOVE_THRESHOLD):
                    case (ushort)(CANSIDevice.HistoryEvents.VOLT_HOUR_BELOW_LOW_THRESHOLD):
                    case (ushort)(CANSIDevice.HistoryEvents.VOLT_HOUR_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL):
                    case (ushort)(CANSIDevice.HistoryEvents.VOLT_HOUR_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL):
                    {
                        // It seems that firmware is populating this field backward (Filler Filler UINT16 UINT8)
                        //  I need to read past the filler data to get to the good stuff.
                        byte byFiller1 = ArgumentReader.ReadByte();
                        byte byPhase = ArgumentReader.ReadByte();
                        float fltVoltHour = ArgumentReader.ReadUInt16() / 40;
                        byte byFiller2 = ArgumentReader.ReadByte();
                        string strVoltHour = fltVoltHour.ToString("f2", CultureInfo.CurrentCulture);
                        string strPhase = "";

                        if (byPhase == 0)
                        {
                            strPhase = "; Phase (A)";
                        }
                        else if (byPhase == 1)
                        {
                            strPhase = "; Phase (B)";
                        }
                        else if (byPhase == 2)
                        {
                            strPhase = "; Phase (C)";
                        }

                        strData = "Volt Hour: " + strVoltHour + strPhase;

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.SECURITY_EVENT):
                    {
                        // There is one byte of data that is an enumeration for Security Errors
                        byte byError = ArgumentReader.ReadByte();

                        switch (byError)
                        {
                            case 1:
                            {
                                strData = m_rmStrings.GetString("REPLAY_ATTACK");
                                break;
                            }
                            case 2:
                            {
                                strData = m_rmStrings.GetString("KEY_ROLLOVER_NO_KEY");
                                break;
                            }
                            case 3:
                            {
                                strData = m_rmStrings.GetString("HASH256_ERROR");
                                break;
                            }
                            case 4:
                            {
                                strData = m_rmStrings.GetString("ECDSA_VERIFY_FAIL");
                                break;
                            }
                            case 5:
                            {
                                strData = m_rmStrings.GetString("ECDSA_PROCESSING_FAIL");
                                break;
                            }
                            case 6:
                            {
                                strData = m_rmStrings.GetString("ECDSA_INCORRECT_KEY");
                                break;
                            }
                            case 7:
                            {
                                strData = m_rmStrings.GetString("ECIES_DECRYPTION_PROCESSING_FAIL");
                                break;
                            }
                            case 8:
                            {
                                strData = m_rmStrings.GetString("AES_DECRYPTION_PROCESSING_FAIL");
                                break;
                            }
                            case 9:
                            {
                                strData = m_rmStrings.GetString("AES_ENCRYPTION_PROCESSING_FAIL");
                                break;
                            }
                            case 10:
                            {
                                strData = m_rmStrings.GetString("HMAC_PROCESSING_FAIL");
                                break;
                            }
                            case 11:
                            {
                                strData = m_rmStrings.GetString("IV_GENERATE_PROCESSING_FAIL");
                                break;
                            }
                            case 12:
                            {
                                strData = m_rmStrings.GetString("ANSIX931_PROCESSING_FAIL");
                                break;
                            }
                            case 13:
                            {
                                strData = m_rmStrings.GetString("UNKNOWN_ASYMMETRIC_SIGN_KEY");
                                break;
                            }
                            case 14:
                            {
                                strData = m_rmStrings.GetString("UNKNOWN_SYMMETRIC_SIGN_KEY");
                                break;
                            }
                            case 15:
                            {
                                strData = m_rmStrings.GetString("UNKNOWN_KEY");
                                break;
                            }
                            case 16:
                            {
                                strData = m_rmStrings.GetString("STORING_KEYS_FAIL");
                                break;
                            }
                            case 17:
                            {
                                strData = m_rmStrings.GetString("REAING_KEYS_FAIL");
                                break;
                            }
                            case 18:
                            {
                                byte byKey = ArgumentReader.ReadByte();
                                strData = m_rmStrings.GetString("KEY_ROLLOVER_FAIL");
                                strData += byKey.ToString(CultureInfo.CurrentCulture);
                                break;
                            }
                            case 19:
                            {
                                byte byKey = ArgumentReader.ReadByte();
                                strData = m_rmStrings.GetString("REPLACE_METER_KEY_FAIL");
                                strData += byKey.ToString(CultureInfo.CurrentCulture);
                                break;
                            }
                            case 20:
                            {
                                byte byKey = ArgumentReader.ReadByte();
                                strData = m_rmStrings.GetString("REPLACE_SIGN_KEY_FAIL");
                                strData += byKey.ToString(CultureInfo.CurrentCulture);
                                break;
                            }
                            case 21:
                            {
                                strData = m_rmStrings.GetString("REPLAY_ATTACK_TIME_SYNC");
                                break;
                            }
                            case 22:
                            {
                                strData = m_rmStrings.GetString("FAILURE_PROCESS_TIME_SYNC");
                                break;
                            }
                            case 23:
                            {
                                strData = m_rmStrings.GetString("UNRECOGNIZED_TABLE_ID");
                                break;
                            }
                            case 24:
                            {
                                strData = m_rmStrings.GetString("START_TIME_AHEAD_METER");
                                break;
                            }
                            case 25:
                            {
                                strData = m_rmStrings.GetString("END_TIME_OLDER_METER");
                                break;
                            }
                            case 26:
                            {
                                strData = m_rmStrings.GetString("UNRECOGNIZED_PASSWORD_LEVEL");
                                break;
                            }
                            case 27:
                            {
                                strData = m_rmStrings.GetString("UNRECOGNIZED_SERIAL_NUMBER");
                                break;
                            }
                            case 28:
                            {
                                strData = m_rmStrings.GetString("DISABLED_CONFIG_BIT_SIGNED_AUTHORIZATION");
                                break;
                            }
                            case 29:
                            {
                                strData = m_rmStrings.GetString("SIGNED_AUTH_ENABLED");
                                break;
                            }
                            case 30:
                            {
                                strData = m_rmStrings.GetString("SIGNED_AUTH_TEMP_DISABLED");
                                break;
                            }
                        }
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.KEY_ROLLOVER_PASS):
                    case (ushort)(CANSIDevice.HistoryEvents.SIGN_KEY_REPLACE_PROCESSING_PASS):
                    case (ushort)(CANSIDevice.HistoryEvents.SYMMETRIC_KEY_REPLACE_PROCESSING_PASS):
                    {
                        int iTimeStamp = ArgumentReader.ReadInt32();
                        byte byKeyId = ArgumentReader.ReadByte();

                        strData = "Key Id: " + byKeyId.ToString(CultureInfo.CurrentCulture);
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.TIME_ADJUSTMENT_FAILED):
                    {
                        int TimeDiff = ArgumentReader.ReadInt32();

                        if (TimeDiff != -1)
                        {
                            strData = "Time Difference: " + TimeDiff.ToString(CultureInfo.CurrentCulture) + " seconds";
                        }
                        else
                        {
                            // -1 is a special case that indicates that the time difference is greater than the threshold.
                            strData = "Time Difference: Out of Range";
                        }

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.CPP_EVENT):
                    {
                        strData = "Status: ";
                        byte byCPPStatus = ArgumentReader.ReadByte();

                        strData += EnumDescriptionRetriever.RetrieveDescription((CppStatus)byCPPStatus);

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.LOAD_VOLT_PRESENT):
                    {

                        //First byte is unused.
                        ArgumentReader.ReadByte();

                        //Second byte has the data we are looking for.
                        byte byReason = ArgumentReader.ReadByte();

                        if (Enum.IsDefined(typeof(LoadVoltagePresentReason), byReason))
                        {
                            strData += EnumDescriptionRetriever.RetrieveDescription((LoadVoltagePresentReason)byReason);
                        }
                        else
                        {
                            // We don't have a definition for this so add a generic error
                            strData += "Unknown Reason: " + byReason.ToString(CultureInfo.CurrentCulture);
                        }

                        break;

                    }
                    case (ushort)(CANSIDevice.HistoryEvents.DEMAND_RESET):
                    {
                        switch (HistoryEvent.UserID)
                        {
                            case 0:
                            {
                                strData += m_rmStrings.GetString("SCHEDULED");
                                break;
                            }
                            case 1:
                            {
                                strData += m_rmStrings.GetString("BUTTON_PRESSED");
                                break;
                            }
                            case 65535:
                            {
                                strData += m_rmStrings.GetString("INITIATED_REMOTELY");
                                break;
                            }
                            default: //2-65534 is initiated locally.
                            {
                                strData += m_rmStrings.GetString("INITIATED_LOCALLY");
                                break;
                            }
                        }

                        break;

                    }
                    case (ushort)(CANSIDevice.HistoryEvents.MAGNETIC_TAMPER_DETECTED):
                    {

                        UInt32 uiCounts = ArgumentReader.ReadUInt32();
                        UInt32 uiTamperCount = uiCounts & TAMPER_COUNT_FLAG;

                        strData += "Tamper Count: " + uiTamperCount.ToString(CultureInfo.CurrentCulture);

                        break;

                    }
                    case (ushort)(CANSIDevice.HistoryEvents.CTE_EVENT):
                    {
                        // one event per phase even if all 3 phases transition at once
                        UInt16 uiCount = ArgumentReader.ReadUInt16();
                        byte bEventDataPhase = ArgumentReader.ReadByte();   //First byte was the phase that either went high or low, NO LONGER USED but we need to read the byte
                        byte bEventData = ArgumentReader.ReadByte();        //Second byte indicates which way it transitioned

                        bool activePhaseA = (0x01 & bEventData) == 0x01;
                        bool activePhaseB = (0x02 & bEventData) == 0x02;
                        bool activePhaseC = (0x04 & bEventData) == 0x04;

                        bool previouslyActivePhaseA = (0x10 & bEventData) == 0x10;
                        bool previouslyActivePhaseB = (0x20 & bEventData) == 0x20;
                        bool previouslyActivePhaseC = (0x40 & bEventData) == 0x40;

                        string strPhasesThatExceededTolerance = string.Empty;
                        string strPhasesThatReturnedToWithinTolerance = string.Empty;

                        if (activePhaseA && !previouslyActivePhaseA)
                        {
                            strPhasesThatExceededTolerance += "A";
                        }

                        if (activePhaseB && !previouslyActivePhaseB)
                        {
                            strPhasesThatExceededTolerance += "B";
                        }

                        if (activePhaseC && !previouslyActivePhaseC)
                        {
                            strPhasesThatExceededTolerance += "C";
                        }


                        if (!activePhaseA && previouslyActivePhaseA)
                        {
                            strPhasesThatReturnedToWithinTolerance += "A";
                        }

                        if (!activePhaseB && previouslyActivePhaseB)
                        {
                            strPhasesThatReturnedToWithinTolerance += "B";
                        }

                        if (!activePhaseC && previouslyActivePhaseC)
                        {
                            strPhasesThatReturnedToWithinTolerance += "C";
                        }


                        // if you change the string here make sure you change it in the CTETestBase as well.
                        // The tests that use CTETestBase search the translated event data string for both
                        // hardcoded strings listed below.

                        if (string.IsNullOrEmpty(strPhasesThatExceededTolerance) && string.IsNullOrEmpty(strPhasesThatReturnedToWithinTolerance))
                        {
                            strData += "Unable to interpret argument data";

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strPhasesThatExceededTolerance))
                            { 
                                strData += "Exceeded tolerance phase(s) [" + strPhasesThatExceededTolerance + "].";
                            }

                            if (!string.IsNullOrEmpty(strPhasesThatReturnedToWithinTolerance))
                            {
                                strData += "Returned to within tolerance phase(s) [" + strPhasesThatReturnedToWithinTolerance + "].";
                            }
                        }

                        bEventData = ArgumentReader.ReadByte(); //currently unused

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.EVENT_GENERIC_HISTORY_EVENT):
                    {
                        byte bySubEvent = ArgumentReader.ReadByte();

                        switch (bySubEvent)
                        {
                            case 1: //Firmware Activation Canceled
                                    {
                                strData += "FW Type: ";

                                byte fwType = ArgumentReader.ReadByte();
                                switch (fwType)
                                {
                                    case 0:
                                    {
                                        strData += "Register FW";
                                        break;
                                    }
                                    case 1:
                                    {
                                        strData += "Comm Module FW";
                                        break;
                                    }
                                    case 4:
                                    {
                                        strData += "3rd Party FW";
                                        break;
                                    }
                                    default:
                                    {
                                        strData += "Unknown (" + fwType.ToString(CultureInfo.InvariantCulture) + ")";
                                        break;
                                    }
                                }
                                break;
                            }
                            case 2: //Performed Firmware CRC
                            {
                                
                                byte byState = ArgumentReader.ReadByte();
                                UInt16 calculatedCRC = ArgumentReader.ReadUInt16();

                                strData += "Result: ";

                                switch (byState)
                                {
                                    case 1:
                                        {
                                            strData += "Waiting";
                                            break;
                                        }
                                    case 2:
                                        {
                                            strData += "Downloading";
                                            break;
                                        }
                                    case 3:
                                        {
                                            strData += "Calculating";
                                            break;
                                        }
                                    case 4:
                                        {
                                            strData += "CRC Pass";
                                            break;
                                        }
                                    case 5:
                                        {
                                            strData += "CRC Fail";
                                            break;
                                        }
                                    default:
                                        {
                                            strData += "Unknown";
                                            break;
                                        }
                                }

                                strData += ", CRC: 0x" + calculatedCRC.ToString("X4");

                                break;
                            }
                            case 3: //IPv6 Comm Module Upgrade
                            {
                                        strData += "Switched From RFLAN to IPv6";
                                        break;
                            }
                            case 4: //IPv6 Comm Module SPPP Connection
                                    {
                                byte connState = ArgumentReader.ReadByte();

                                switch (connState)
                                {
                                    case 1:
                                    {
                                        strData += "Connection Unsecure";
                                        break;
                                    }
                                    case 2:
                                    {
                                        strData += "Connection Secure";
                                        break;
                                    }
                                    case 3:
                                    {
                                        strData += "Connection Locked Secure";
                                        break;
                                    }
                                }
                                break;
                            }
                            case 5: //IPv6 Comm Module Sync
                            {
                                strData += "IP Address Obtained";
                                break;
                            }
                            case 6: //802.1x State Change
                            {
                                strData += "State: ";
                                byte connState = ArgumentReader.ReadByte();

                                switch (connState)
                                {
                                    case 1:
                                    {
                                        strData += "No Cert or Startup";
                                        break;
                                    }
                                    case 2:
                                    {
                                        strData += "Global Cert Assumed";
                                        break;
                                    }
                                    case 3:
                                    {
                                        strData += "New Cert Write Started";
                                        break;
                                    }
                                    case 4:
                                    {
                                        strData += "Cert Write Failed";
                                        break;
                                    }
                                    case 5:
                                    {
                                        strData += "Cert Write Success";
                                        break;
                                    }
                                    case 6:
                                    {
                                        strData += "Cert Read Back Check Ok";
                                        break;
                                    }
                                    case 7:
                                    {
                                        strData += "Cert Read Back Check Fail";
                                        break;
                                    }
                                    case 8:
                                    {
                                        strData += "Cert Verify Timeout";
                                        break;
                                    }
                                    case 9:
                                    {
                                        strData += "Cert Verified";
                                        break;
                                    }
                                    case 10:
                                    {
                                        strData += "Cert Verify Assumed";
                                        break;
                                    }
                                    case 11:
                                    {
                                        strData += "Cert Committed to Flash";
                                        break;
                                    }
                                }

                                break;
                            }
                            case 7: //IPv6 Link Local PSK Generated
                                    {
                                strData += "State: ";
                                byte pskState = ArgumentReader.ReadByte();

                                switch (pskState)
                                {
                                    case 1:
                                    {
                                        strData += "PSK Auto-generated";
                                        break;
                                    }
                                    case 2:
                                    {
                                        strData += "PSK Sent to Module";
                                        break;
                                    }
                                }

                                break;
                            }
                            case 8: //MSM State Change
                            {
                                strData += "New Mode: ";

                                byte msmMode = ArgumentReader.ReadByte();

                                switch (msmMode)
                                {
                                    case 1:
                                    {
                                        strData += "OpenWay Mode";
                                        break;
                                    }
                                    case 2:
                                    {
                                        strData += "Choice Connect Mode";
                                        break;
                                    }
                                }

                                break;
                            }
                            case 9: //Energy Quantity Change - Nitrogen upgrade enabled var Q2/Q3 meterkey bits
                            {
                                //No argument data
                                break;
                            }
                            case 100:
                            {
                                strData += "COSEM Failure";
                                break;
                            }
                            case 200:
                            {
                                strData += "SN Read Failure";
                                break;
                            }
                            default:
                            {
                                strData += string.Format("Unknown sub-event type {0}", bySubEvent);
                                break;
                            }
                        }

                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.ICS_ERT_EVENT):
                    {
                        strData = "Serial Number: " + ArgumentReader.ReadUInt32().ToString(CultureInfo.InvariantCulture);
                        break;
                    } 
                    case (ushort)(CANSIDevice.HistoryEvents.EVENT_HARDWARE_ERROR_DETECTION):
                    {
                        byte arg = ArgumentReader.ReadByte();
                        switch (arg)
                        {
                            case 0x01:
                                {
                                    strData = m_rmStrings.GetString("HWERR_DISCONNECT_NO_LSV_CURRENT_FLOWING");
                                    break;
                                }
                            default:
                                {
                                    strData = "Unsupported argument value - Arg = " + arg.ToString("X");
                                    break;
                                }
                        }
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.WRONG_CONFIG_CRC):
                        {
                            byte arg = ArgumentReader.ReadByte();
                            switch (arg)
                                { 
                                case 0x00: // CRC is wrong when checked in other normal cases
                                    {
                                        strData = "Configuration CRC Incorrect";
                                        break;
                                    }
                                case 0x01: // CRC is wrong on firmware download
                                    {
                                        strData = "Configuration CRC Incorrect on Firmware Download";
                                        break;
                                    }
                                default:
                                    {
                                        strData = "Unsupported argument value - Arg = " + arg.ToString("X");
                                        break;
                                    }
                                }
                            break;
                        } 
                    case (ushort)(CANSIDevice.HistoryEvents.TEMPERATURE_EXCEEDS_THRESHOLD1):
                    case (ushort)(CANSIDevice.HistoryEvents.TEMPERATURE_EXCEEDS_THRESHOLD2):
                    case (ushort)(CANSIDevice.HistoryEvents.TEMPERATURE_RETURNED_TO_NORMAL):
                    {
                        strData += "Instantaneous Offset Temperature (C): " + ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        strData += "; Average Aggregate Current (Amps): " + ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                    case (ushort)(CANSIDevice.HistoryEvents.EVENT_LOGGING_SUSPENDED):
                        {
                            byte Minutes = ArgumentReader.ReadByte();
                            byte Seconds = ArgumentReader.ReadByte();
                            strData += "Time left in suspension period " + Minutes + " minutes " + Seconds + " seconds";
                            break;
                        }
                    case (ushort)(CANSIDevice.HistoryEvents.LOGGING_RESUMED):
                        {
                            byte Missing = ArgumentReader.ReadByte();
                            if (Missing == 255)
                            {
                                strData += "Number of events missing during suspension : More than 254";
                            }
                            else
                            {
                                strData += "Number of events missing during suspension : " + Missing;
                            }
                            break;
                        }
                    default:
                    {
                        strData = base.TranslatedEventData(HistoryEvent, ArgumentReader);
                        break;
                    }
                }
            }

            return strData;
        }

        #endregion
    }
}
