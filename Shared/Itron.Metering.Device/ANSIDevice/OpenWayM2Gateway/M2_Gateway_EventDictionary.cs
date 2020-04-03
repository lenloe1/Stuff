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
//                        Copyright © 2010 - 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Resources;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Constructs a dictionary of Centron AMI specific events
    /// </summary>
    public class M2_Gateway_EventDictionary : CENTRON_AMI_EventDictionary
    {
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
        }

        #endregion

        #region Public Methods

        /// <summary>Constructs a dictionary of M2 Gateway specific events</summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#      Description
        //  -------- --- ------- ------      -------------------------------------------
        //  08/10/10 AF  2.42.17             Created
        //  10/21/10 AF  2.45.06             Updated with more events
        //  11/04/10 AF  2.45.10             Removed unsupported events
        //  04/27/11 AF  2.50.35 172845      Changed the parent class to CENTRON_AMI_EventDictionary
        //                                   and cleaned up the event list
        //  07/12/16 MP  4.70.7  WR688986    Commented out because it's part of centron dictionary
       
        public M2_Gateway_EventDictionary()
            : base()
        {
            //Add((int)CANSIDevice.HistoryEvents.GW_CONFIGURATION_DOWNLOAD, m_rmStrings.GetString("GW_CONFIGURATION_DOWNLOAD"));
        }

        #endregion
    }
}
