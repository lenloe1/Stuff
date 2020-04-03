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
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.Serialization;

namespace Itron.Metering.Zigbee
{
    #region Public Enumerations

    /// <summary>
    /// EZSP Frame ID's
    /// </summary>
    public enum EZSPFrameIDs : byte
    {
#pragma warning disable 1591
        Version = 0x00,
        GetLibraryStatus = 0x01,
        AddEndpoint = 0x02,
        Nop = 0x05,
        Callback = 0x06,
        NoCallbacks = 0x07,
        SetToken = 0x09,
        GetToken = 0x0A,
        GetMFGToken = 0x0B,
        SetTimer = 0x0E,
        TimerHandler = 0x0F,
        DebugWrite = 0x12,
        RequestLinkKey = 0x14,
        SetManufacturerCode = 0x15,
        SetPowerDescriptor = 0x16,
        NetworkInit = 0x17,
        NetworkState = 0x18,
        StackStatusHandler = 0x19,
        StartScan = 0x1A,
        NetworkFoundHandler = 0x1B,
        ScanCompleteHandler = 0x1C,
        StopScan = 0x1D,
        FormNetwork = 0x1E,
        JoinNetwork = 0x1F,
        LeaveNetwork = 0x20,
        FindAndRejoinNetwork = 0x21,
        PermitJoining = 0x22,
        ChildJoinHandler = 0x23,
        TrustCenterJoinHandler = 0x24,
        GetEUI64 = 0x26,
        GetNodeID = 0x27,
        GetNetworkParameters = 0x28,
        GetParentChildParameters = 0x29,
        ClearBindingTable = 0x2A,
        SetBinding = 0x2B,
        GetBinding = 0x2C,
        DeleteBinding = 0x2D,
        BindingIsActive = 0x2E,
        GetBindingRemoteNodeID = 0x2F,
        SetBindingRemoteNodeID = 0x30,
        RemoteSetBindingHandler = 0x31,
        RemoteDeleteBindingHandler = 0x32,
        MaximumPayloadLength = 0x33,
        SendUnicast = 0x34,
        SendBroadcast = 0x36,
        SendMulticast = 0x38,
        SendReply = 0x39,
        MessageSentHandler = 0x3F,
        SendManyToOneRouteRequest = 0x41,
        PollForData = 0x42,
        PollCompleteHandler = 0x43,
        PollHandler = 0x44,
        IncomingMessageHandler = 0x45,
        MacFilterMatchMessageHandler = 0x46,
        EnergyScanResultHandler = 0x48,
        GetRandomNumber = 0x49,
        GetChildData = 0x4A,
        GetTimer = 0x4E,
        GetConfigurationValue = 0x52,
        SetConfigurationValue = 0x53,
        SetPolicy = 0x55,
        GetPolicy = 0x56,
        InvalidCommand = 0x58,
        IncomingRouteRecordHandler = 0x59,
        SetSourceRoute = 0x5A,
        AddressTableEntryIsActive = 0x5B,
        SetAddressTableRemoteEUI = 0x5C,
        SetAddressTableRemoteNodeID = 0x5D,
        GetAddressTableRemoteEUI = 0x5E,
        GetAddressTableRemoteNodeID = 0x5F,
        LookupNodeIDByEUI = 0x60,
        LookipEUIByNodeID = 0x61,
        IncomingSenderEUIHandler = 0x62,
        GetMulticastTableEntry = 0x63,
        SetMulticastTableEntry = 0x64,
        ReadAndClearCounters = 0x65,
        AddOrUpdateKeyTableEntry = 0x66,
        SetInitialSecurityState = 0x68,
        GetCurrentSecurityState = 0x69,
        GetKey = 0x6A,
        SwitchNetworkKeyHandler = 0x6E,
        AesMmoHash = 0x6F,
        GetKeyTableEntry = 0x71,
        SetKeyTableEntry = 0x72,
        BroadcastNextNetworkKey = 0x73,
        BroadcastNetworkKeySwitch = 0x74,
        FindKeyTableEntry = 0x75,
        EraseKeyTableEntry = 0x76,
        BecomeTrustCenter = 0x77,
        DsaVerifyHandler = 0x78,
        GetNeighbor = 0x79,
        NeighborCount = 0x7A,
        GetRouteTableEntry = 0x7B,
        IDConflictHandler = 0x7C,
        SetExtendedTimeout = 0x7E,
        GetExtendedTimeout = 0x7F,
        IncomingManyToOneRouteRequestHandler = 0x7D,
        IncomingRouteErrorHandler = 0x80,
        Echo = 0x81,
        ReplaceAddressTableEntry = 0x82,
        SendRawMessage = 0x96,
        MacPassthroughMessageHandler = 0x97,
        RawTransmitCompleteHandler = 0x98,
        SetRadioPower = 0x99,
        SetRadioChannel = 0x9A,
        ZigBeeKeyEstablishmentHandler = 0x9B,
        EnergyScanRequest = 0x9C,
        DelayTest = 0x9D,
        GenerateCBKEKeysHandler = 0x9E,
        CalculateSmacs = 0x9F,
        CalculateSmacsHandler = 0xA0,
        ClearTemporaryDataMaybeStoreLinkKey = 0xA1,
        SetPreinstalledCBKEData = 0xA2,
        DsaVerify = 0xA3,
        GenerateCBKEKeys = 0xA4,
        GetCertificate = 0xA5,
        DsaSign = 0xA6,
        DsaSignHandler = 0xA7,
        RemoveDevice = 0xA8,
        GetValue = 0xAA,
        SetValue = 0xAB,
#pragma warning restore 1591
    }

    /// <summary>
    /// Network Scan types
    /// </summary>
    public enum EZSPScanType : byte
    {
        /// <summary>
        /// Scan for RSSI values
        /// </summary>
        EnergyScan = 0x00,
        /// <summary>
        /// Scan for available devices
        /// </summary>
        ActiveScan = 0x01,
    }

    /// <summary>
    /// The Sleep Modes used for Command Frames
    /// </summary>
    public enum EZSPSleepMode : byte
    {
        /// <summary>
        /// The device is idle
        /// </summary>
        Idle = 0x00,
        /// <summary>
        /// The device is in deep sleep mode
        /// </summary>
        DeepSleep = 0x01,
        /// <summary>
        /// The device is powered down
        /// </summary>
        PowerDown = 0x02,
    }

    /// <summary>
    /// The callback type for an EZSP response frame
    /// </summary>
    public enum EZSPCallbackType : byte
    {
        /// <summary>
        /// Response is not a Callback
        /// </summary>
        NotACallback = 0x00,
        /// <summary>
        /// Response is a synchronous callback
        /// </summary>
        SynchronousCallback = 0x08,
        /// <summary>
        /// Response is an asynchronous callback
        /// </summary>
        AsynchronousCallback = 0x10,
    }

    /// <summary>
    /// Status return value for Ember commands
    /// </summary>
    public enum EmberStatus : byte
    {
        /// <summary>Success</summary>
        Success = 0x00,
        /// <summary>Fatal Error</summary>
        FatalError = 0x01,
        /// <summary>Bad Argument</summary>
        BadArgument = 0x02,
        /// <summary>Manufacturing and Stack Token format is different than the stack expect</summary>
        EepromMFGStackVersionMismatch = 0x04,
        /// <summary>The static memory definitions are incompatible with the stack</summary>
        IncompatibleStaticMemoryDefinition = 0x05,
        /// <summary>Manufacturing token format different than what the stack expects</summary>
        EepromMFGVersionMismatch = 0x06,
        /// <summary>Stack toekn format different than what the stack expects</summary>
        EepromStackVersionMismatch = 0x07,
        /// <summary>No buffers available</summary>
        NoBuffers = 0x18,
        /// <summary>The serial baud rate is invalid</summary>
        InvalidSerialBaudRate = 0x20,
        /// <summary> An Invalid Port was specified</summary>
        InvalidSerialPort = 0x21,
        /// <summary>Transmit buffer overflow</summary>
        SerialTxOverflow = 0x22,
        /// <summary>Receive buffer overflow</summary>
        SerialRxOverflow = 0x23,
        /// <summary>UART framing error detected</summary>
        SerialRxFrameError = 0x24,
        /// <summary>UART parity error detected</summary>
        SerialRxParityError = 0x25,
        /// <summary>No data was received</summary>
        SerialRxEmpty = 0x26,
        /// <summary>The receive interrupt was not handled in time and characters were dropped</summary>
        SerialRxOverrunError = 0x27,
        /// <summary>No pending data exists</summary>
        MACNoData = 0x31,
        /// <summary>Attempted to scan when already joined to a network</summary>
        MACJoinedNetwork = 0x32,
        /// <summary>Scan duration is bad. Must be between 0 and 14</summary>
        MACBadScanDuration = 0x33,
        /// <summary>Incorrect scan type supplied</summary>
        MACIncorrectScanType = 0x34,
        /// <summary>Channel mask is invalid</summary>
        MACInvalidChannelMask = 0x35,
        /// <summary>Failed to scan current channel because we were unable to transmit the MAC command</summary>
        MACCommandTransmitFailure = 0x36,
        /// <summary>The MAC transmit queue is full</summary>
        MACTransmitQueueFull = 0x39,
        /// <summary>MAC Header error received</summary>
        MACUnknownHeaderType = 0x3A,
        /// <summary>Can't complete task because scanning is in progress</summary>
        MACScanning = 0x3D,
        /// <summary>No MAC level Ack was received.</summary>
        MACNoAckReceived = 0x40,
        /// <summary>Indirect data message timed out</summary>
        MACIndirectTimeout = 0x42,
        /// <summary>At least one simulated flash page can be erased. (Current page below erase critical threshold)</summary>
        SimEepromErasePageGreen = 0x43,
        /// <summary> At least one simulated flash page can be erased. (Current page above erase critical threshold)</summary>
        SimEepromErasePageRed = 0x44,
        /// <summary>The simulated EEPROM is out of space</summary>
        SimEepromFull = 0x45,
        /// <summary>Target memory is already programmed.</summary>
        ErrorFlashWriteInhibited = 0x46,
        /// <summary>Write verification has failed</summary>
        ErrorFlashVerifyFailed = 0x47,
        /// <summary>Attempt 1 to initialize has failed</summary>
        SimEepromInit1Failed = 0x48,
        /// <summary>Attempt 2 to initialize has failed</summary>
        SimEepromInit2Failed = 0x49,
        /// <summary>Attempt 3 to initialize has failed</summary>
        SimEepromInit3Failed = 0x4A,
        /// <summary>Error occurred trying to write data to the flash</summary>
        ErrorFlashProgramFail = 0x4B,
        /// <summary>Error occurred trying to erase flash</summary>
        ErrorFlashEraseFail = 0x4C,
        /// <summary>An incorrect size was specified when retrieving token data</summary>
        ErrorTokenInvalidSize = 0x4D,
        /// <summary>Couldn't write token because it is marked read only</summary>
        ErrorTokenReadOnly = 0x4E,
        /// <summary>Bootloader received an invalid message</summary>
        ErrorBootloaderTrapTableBad = 0x58,
        /// <summary>Bootloader received an invalid message</summary>
        ErrorBootloaderTrapUnknown = 0x59,
        /// <summary>Bootloader could not complete because the image was not found</summary>
        ErrorBootloaderNoImage = 0x5A,
        /// <summary>APS Layer message delivery failed</summary>
        DeliveryFailed = 0x66,
        /// <summary>Binding index out of range for the current binding table</summary>
        BindingImageOutOfRange = 0x69,
        /// <summary>Adress table index out of range for the current address table</summary>
        AddressTableIndexOutOfRange = 0x6A,
        /// <summary>Invalid binding index given to a function</summary>
        InvalidBindingIndex = 0x6C,
        /// <summary>The API call is not allowed for the current state of the stack</summary>
        InvalidCall = 0x70,
        /// <summary>The link cost to a node is not known</summary>
        CostNotKnown = 0x71,
        /// <summary>The maximum number of in flight messages has been reached</summary>
        MaxMessageLimitReached = 0x72,
        /// <summary>The message being transmitted is too big for a single packet</summary>
        MessageTooLong = 0x74,
        /// <summary>The application is trying to delete a binding that is in use</summary>
        BindingIsActive = 0x75,
        /// <summary>The application is trying to overwrite an address table entry that is in use</summary>
        AddressTableEntryIsActive = 0x76,
        /// <summary>Conversion is complete</summary>
        ADCConversionDone = 0x80,
        /// <summary>Conversion can't be done because a request is being processed</summary>
        ADCConversionBusy = 0x81,
        /// <summary>Conversion is deferred until the current request is complete</summary>
        ADCConversionDeferred = 0x82,
        /// <summary>No results are pending</summary>
        ADCNoConversionPending = 0x84,
        /// <summary>Sleeping has been interrupted prematurely</summary>
        SleepInterrupted = 0x85,
        /// <summary>Transmit buffer underflowed</summary>
        PhyTxUnderflow = 0x88,
        /// <summary>Transmit buffer did not finish transmitting the packet</summary>
        PhyTxIncomplete = 0x89,
        /// <summary>An unsupported channel was specified</summary>
        PhyInvalidChannel = 0x8A,
        /// <summary>Unsupported power setting was specified</summary>
        PhyInvalidPower = 0x8B,
        /// <summary>Cannot transmit because the physical layer is already transmitting</summary>
        PhyTxBusy = 0x8C,
        /// <summary>Transmit failed because all CCA attempts indicated the channel was busy</summary>
        PhyTxCCAFail = 0x8D,
        /// <summary>The software installed on the hardware does not recognize the hardware radio type</summary>
        PhyOscillatorCheckFailed = 0x8E,
        /// <summary>The expected ack was received</summary>
        PhyAckReceived = 0x8F,
        /// <summary>The network is up.</summary>
        NetworkUp = 0x90,
        /// <summary>The network is down.</summary>
        NetworkDown = 0x91,
        /// <summary>The node has not joined a network</summary>
        NotJoined = 0x93,
        /// <summary>The chosen security level is not supported</summary>
        InvalidSecurityLevel = 0x95,
        /// <summary>The attempt to join the network failed</summary>
        JoinFailed = 0x94,
        /// <summary>After moving the node could  not establish contact with the network</summary>
        MoveFailed = 0x96,
        /// <summary>Could not join as a router</summary>
        CannotJoinAsRouter = 0x98,
        /// <summary>the local node ID has changed.</summary>
        NodeIDChanged = 0x99,
        /// <summary>The local PAN ID has changed</summary>
        PanIDChanged = 0x9A,
        /// <summary>Message cannot be sent because the network is currently overloaded</summary>
        NetworkBusy = 0xA1,
        /// <summary>The application tried to send a message using an endpoint that is not defined</summary>
        InvalidEndpoint = 0xA3,
        /// <summary>Application tried to use a binding that ha been remotely modified</summary>
        BindingHasChanged = 0xA4,
        /// <summary>Attempt to generate random bytes failed because of insufficient random data from the radio</summary>
        InsufficientRandomData = 0xA5,
        /// <summary>Error trying to encrypt at the APS layer</summary>
        APSEncryptionError = 0xA6,
        /// <summary>There was an attempt to form a network using the commercial security without setting the Trust Center master key first</summary>
        TrustCenterMasterKeyNotSet = 0xA7,
        /// <summary>There was an attempt to firm or join a network using Standard or Commercial security without call emberSetSecurity first</summary>
        SecurityStateNotSet = 0xA8,
        /// <summary>A ZigBee route error command frame was received</summary>
        SourceRouteFailure = 0xA9,
        /// <summary>A ZigBee route error command frame was received indicating a many to one route failed</summary>
        ManyToOneRouteFailure = 0xAA,
        /// <summary>An attempt to join or rejoin failed because no router beacons could be heard</summary>
        NoBeacons = 0xAB,
        /// <summary>Attempt made to join secure network but received a network key in the clear</summary>
        ReceivedKeyInTheClear = 0xAC,
        /// <summary>No Network Key was received</summary>
        NoNetworkKeyReceived = 0xAD,
        /// <summary>No Link Key was received</summary>
        NoLinkKeyReceived = 0xAE,
        /// <summary>Attempt made to join a secure network without a preconfigured key but encrypted data was received</summary>
        PreconfiguredKeyRequired = 0xAF,
        /// <summary>The version of the stack trying to run does not match with the chip it is running on</summary>
        StackAndHardwareMismatch = 0xB0,
        /// <summary>Index out of range</summary>
        IndexOutOfRange = 0xB1,
        /// <summary>There are no empty entries left in the table</summary>
        TableFull = 0xB4,
        /// <summary>The requested function cannot be executed because the library that contains the functionality is not present.</summary>
        LibraryNotPresent = 0xB5,
        /// <summary>The requested entry has been erased.</summary>
        TableEntryErased = 0xB6,
        /// <summary>There was an attempt to set an entry in the key table using an invalid long address</summary>
        KeyTableInvalidAddress = 0xB3,
        /// <summary>Attempt to set a security configuration that is not valid given the other security settings</summary>
        SecurityConfigurationInvalid = 0xB7,
        /// <summary>There was an attempt to broadcast a key switch too quickly after broadcasting the next network key</summary>
        TooSoonForSwitchKey = 0xB8,
        /// <summary>Operation in progress</summary>
        OperationInProgress = 0xBA,
        /// <summary>The message could not be sent because the link key is not authorized</summary>
        KeyNotAuthorized = 0xBB,
        /// <summary>The security data provided was not valid</summary>
        SecurityDataInvalid = 0xBD,
        /// <summary>Application Error 0 - ASH Currently disconnected</summary>
        ASHDisconnected = 0xF0,
        /// <summary>Application Error 1</summary>
        ApplicationError1 = 0xF1,
        /// <summary>Application Error 2</summary>
        ApplicationError2 = 0xF2,
        /// <summary>Application Error 3</summary>
        ApplicationError3 = 0xF3,
        /// <summary>Application Error 4</summary>
        ApplicationError4 = 0xF4,
        /// <summary>Application Error 5</summary>
        ApplicationError5 = 0xF5,
        /// <summary>Application Error 6</summary>
        ApplicationError6 = 0xF6,
        /// <summary>Application Error 7</summary>
        ApplicationError7 = 0xF7,
        /// <summary>Application Error 8</summary>
        ApplicationError8 = 0xF8,
        /// <summary>Application Error 9</summary>
        ApplicationError9 = 0xF9,
        /// <summary>Application Error 10</summary>
        ApplicationError10 = 0xFA,
        /// <summary>Application Error 11</summary>
        ApplicationError11 = 0xFB,
        /// <summary>Application Error 12</summary>
        ApplicationError12 = 0xFC,
        /// <summary>Application Error 13</summary>
        ApplicationError13 = 0xFD,
        /// <summary>Application Error 14</summary>
        ApplicationError14 = 0xFE,
        /// <summary>Application Error 15 - Response Timeout</summary>
        ResponseTimeout = 0xFF,
    }

    /// <summary>
    /// Security Key types
    /// </summary>
    public enum EmberKeyType : byte
    {
        /// <summary>Shared key between the Trust Center and a device</summary>
        TrustCenterLinkKey = 0x01,
        /// <summary>Shared secret used for deriving keys between the Trust Center and a device</summary>
        TrustCenterMasterKey = 0x02,
        /// <summary>The current active Network Key used by all devices on the network</summary>
        CurrentNetworkKey = 0x03,
        /// <summary>The alternate Network Key that was previously in use or the newer key that will be switched to</summary>
        NextNetworkKey = 0x04,
        /// <summary>An Application Link Key shared with another device</summary>
        ApplicationLinkKey = 0x05,
        /// <summary>An Application Master key shared secret used to derive and Application Link Key</summary>
        ApplicationMasterKey = 0x06
    }

    /// <summary>
    /// Channels used by the HAN
    /// </summary>
    [DataContract]
    [Flags]
    public enum ZigBeeChannels : uint
    {
        /// <summary>
        /// None selected
        /// </summary>
        [EnumMember]
        None = 0,
        /// <summary>
        /// Channel 11
        /// </summary>
        [EnumMember]
        Channel11 = 0x00000800,
        /// <summary>
        /// Channel 12
        /// </summary>
        [EnumMember]
        Channel12 = 0x00001000,
        /// <summary>
        /// Channel 13
        /// </summary>
        [EnumMember]
        Channel13 = 0x00002000,
        /// <summary>
        /// Channel 14
        /// </summary>
        [EnumMember]
        Channel14 = 0x00004000,
        /// <summary>
        /// Channel 15
        /// </summary>
        [EnumMember]
        Channel15 = 0x00008000,
        /// <summary>
        /// Channel 16
        /// </summary>
        [EnumMember]
        Channel16 = 0x00010000,
        /// <summary>
        /// Channel 17
        /// </summary>
        [EnumMember]
        Channel17 = 0x00020000,
        /// <summary>
        /// Channel 18
        /// </summary>
        [EnumMember]
        Channel18 = 0x00040000,
        /// <summary>
        /// Channel 19
        /// </summary>
        [EnumMember]
        Channel19 = 0x00080000,
        /// <summary>
        /// Channel 20
        /// </summary>
        [EnumMember]
        Channel20 = 0x00100000,
        /// <summary>
        /// Channel 21
        /// </summary>
        [EnumMember]
        Channel21 = 0x00200000,
        /// <summary>
        /// Channel 22
        /// </summary>
        [EnumMember]
        Channel22 = 0x00400000,
        /// <summary>
        /// Channel 23
        /// </summary>
        [EnumMember]
        Channel23 = 0x00800000,
        /// <summary>
        /// Channel 24
        /// </summary>
        [EnumMember]
        Channel24 = 0x01000000,
        /// <summary>
        /// Channel 25
        /// </summary>
        [EnumMember]
        Channel25 = 0x02000000,
        /// <summary>
        /// Channel 26
        /// </summary>
        [EnumMember]
        Channel26 = 0x04000000,
        /// <summary>
        /// The default Itron Channels
        /// </summary>
        [EnumMember]
        ItronDefault = Channel11 | Channel15 | Channel20 | Channel25,
    }

    /// <summary>
    /// Scan durations for EZSP scanning
    /// </summary>
    public enum EZSPScanDuration : byte
    {
        // The formula for scan duration is (2^duration) + 1 periods
        // Valid values are from 0 - 14
        /// <summary>2 Scan Periods</summary>
        ScanPeriodX2 = 0,
        /// <summary>3 Scan Periods</summary>
        ScanPeriodX3 = 1,
        /// <summary>5 Scan Periods</summary>
        ScanPeriodX5 = 2,
        /// <summary>9 Scan Periods</summary>
        ScanPeriodX9 = 3,
        /// <summary>17 Scan Periods</summary>
        ScanPeriodX17 = 4,
        /// <summary>33 Scan Periods</summary>
        ScanPeriodX33 = 5,
        /// <summary>65 Scan Periods</summary>
        ScanPeriodX65 = 6,
        /// <summary>129 Scan Periods</summary>
        ScanPeriodX129 = 7,
        /// <summary>257 Scan Periods</summary>
        ScanPeriodX257 = 8,
        /// <summary>513 Scan Periods</summary>
        ScanPeriodX513 = 9,
        /// <summary>1025 Scan Periods</summary>
        ScanPeriodX1025 = 10,
        /// <summary>2049 Scan Periods</summary>
        ScanPeriodX2049 = 11,
        /// <summary>4097 Scan Periods</summary>
        ScanPeriodX4097 = 12,
        /// <summary>8193 Scan Periods</summary>
        ScanPeriodX8193 = 13,
        /// <summary>16385 Scan Periods</summary>
        ScanPeriodX16385 = 14,
    }

    /// <summary>
    /// Network Status enumeration
    /// </summary>
    public enum EmberNetworkStatus : byte
    {
        /// <summary>No network is associated to the node</summary>
        NoNetwork = 0x00,
        /// <summary>The node is currently attempting to join a network</summary>
        JoiningNetwork = 0x01,
        /// <summary>The node is currently joined to a network</summary>
        JoinedNetwork = 0x02,
        /// <summary>The node is joined to a network but the parent is not responding</summary>
        JoinedNetworkNoParent = 0x03,
        /// <summary>The node is leaving a network.</summary>
        LeavingNetwork = 0x04,
    }

    /// <summary>
    /// Outgoing message type
    /// </summary>
    public enum EmberOutgoingMessageType : byte
    {
        /// <summary>Message sent directly to a node ID</summary>
        Direct = 0x00,
        /// <summary>Message sent using an Address Table entry</summary>
        ViaAddressTable = 0x01,
        /// <summary>Message sent using a Binding Table entry</summary>
        ViaBinding = 0x02,
        /// <summary>Message sent as a multicast message</summary>
        Multicast = 0x03,
        /// <summary>Message sent as a broadcast message</summary>
        Broadcast = 0x04,
    }

    /// <summary>
    /// Concentrator types
    /// </summary>
    public enum EmberConcentratorType : ushort
    {
        /// <summary>A concentrator with insufficient memory to store source routes for the entire network</summary>
        LowRAMConcentrator = 0xFFF8,
        /// <summary>A concentrator with sufficient memory to store source routes for the entire network</summary>
        HighRAMConcentrator = 0xFFF9,
    }

    /// <summary>
    /// Event units
    /// </summary>
    public enum EmberEventUnits : byte
    {
        /// <summary>The event is not scheduled to run</summary>
        Inactive = 0x00,
        /// <summary>The event time is in milliseconds</summary>
        MillisecondTime = 0x01,
        /// <summary>The event time is in "binary" quarter seconds (approx 256 milliseconds)</summary>
        QuarterSecondTime = 0x02,
        /// <summary>The event time is in "binary" minutes (approx 65536 milliseconds)</summary>
        MinuteTime = 0x03,
    }

    /// <summary>
    /// Configuration value ID's
    /// </summary>
    public enum EzspConfigID : byte
    {
        /// <summary>The number of packet buffers available to the stack</summary>
        PacketBufferCount = 0x01,
        /// <summary>The maximum number of router neighbors the stack can keep track of</summary>
        NeighborTableSize = 0x02,
        /// <summary>Maximum number of APS retried messages the stack can be transmitting at one time</summary>
        APSUnicastMessageCount = 0x03,
        /// <summary>Maximum number of non volatile bindings supported</summary>
        BindingTableSize = 0x04,
        /// <summary>Maximum number of EUI to network address associations that can be maintained</summary>
        AddressTableSize = 0x05,
        /// <summary>Maximum number of multicast groups that the device may be a member of</summary>
        MulticastTableSize = 0x06,
        /// <summary>Maximum number of destinations to which a node may route messages</summary>
        RouteTableSize = 0x07,
        /// <summary>Number of simultaneous route discoveries</summary>
        DiscoveryTableSize = 0x08,
        /// <summary>The size of the alarm broadcast buffer</summary>
        BroadcastAlarmDataSize = 0x09,
        /// <summary>The size of the unicast alarm broadcast byffer</summary>
        UnicastAlarmDataSize = 0x0A,
        /// <summary>The stack profile in use</summary>
        StackProfile = 0x0C,
        /// <summary>The security level used at the MAC and network layers</summary>
        SecurityLevel = 0x0D,
        /// <summary>Maximum number of hops for a message</summary>
        MaxHops = 0x10,
        /// <summary>Maximum number of end device children a router supports</summary>
        MaxEndDeviceChildren = 0x11,
        /// <summary>Maximum amount of time that the MAC will hold a message for indirect transmission to a child</summary>
        IndirectTransmissionTimeout = 0x12,
        /// <summary>Maximum amount of time that an end device child can wait between polls.</summary>
        EndDevicePollTimeout = 0x13,
        /// <summary>Maximum amount of time that a mobile node can wait between polls</summary>
        MobileNodePollTimeout = 0x14,
        /// <summary>The number of child table entries reserved for mobile nodes</summary>
        ReservedMobileChildEntries = 0x15,
        /// <summary>Enables boost power mode on the radio</summary>
        TxPowerMode = 0x17,
        /// <summary>Disables the node from relaying messages</summary>
        DisableRelay = 0x18,
        /// <summary>Maximum number of EUI to network address associations the Trust Center can maintain</summary>
        TrustCenterAddressCacheSize = 0x19,
        /// <summary>Size of the source route table</summary>
        SourceRouteTableSize = 0x1A,
        /// <summary>Units used for timing out end devices</summary>
        EndDevicePollTimeoutShift = 0x1B,
        /// <summary>Number of blocks of fragmented data that can be sent in a single window</summary>
        FragmentWindowSize = 0x1C,
        /// <summary>The time the stack will wait between sending blocks of a fragmented message</summary>
        FragmentDelayMs = 0x1D,
        /// <summary>The size of the key table used to store link keys</summary>
        KeyTableSize = 0x1E,
        /// <summary>The APS Ack timeout</summary>
        APSAckTimeout = 0x1F,
        /// <summary>The duration of an active scan</summary>
        ActiveScanDuration = 0x20,
        /// <summary>The time the coordinator will wait for a second end device bind request to arrive</summary>
        EndDeviceBindTimeout = 0x21,
        /// <summary>The number of PAN ID conflict reports that must be received by the network manager in 1 minute to trigger a PAN ID change</summary>
        PANIDConflictReportThreshold = 0x22,
        /// <summary>The timeout value (minutes) for how long the Trust Center or normal node waits for the ZigBee Request Key to complete</summary>
        RequestKeyTimeout = 0x24,
        /// <summary>The size of the certificate table</summary>
        CertificateTableSize = 0x29,
        /// <summary>A bitmask that controls which incoming ZDO request messages are passed to the application.</summary>
        ApplicationZDOFlags = 0x2A,
        /// <summary>Maximum number of broadcasts during a single broadcast timeout period</summary>
        BroadcastTableSize = 0x2B,
        /// <summary>The size of the MAC filter list table</summary>
        MACFilterTableSize = 0x2C,
    }

    /// <summary>
    /// EZSP status
    /// </summary>
    public enum EzspStatus : byte
    {
        /// <summary>Success</summary>
        Success = 0x00,
        /// <summary>Fatal Error</summary>
        SPIErrorFatal = 0x10,
        /// <summary>NCP has Reset</summary>
        SPIErrorNCPReset = 0x11,
        /// <summary>The command frame is too large</summary>
        SPIErrorOversizedEZSPFrame = 0x12,
        /// <summary>The previous transaction was aborted</summary>
        SPIErrorAbortedTransaction = 0x13,
        /// <summary>The frame terminator is missing</summary>
        SPIErrorMissingFrameTerminator = 0x14,
        /// <summary>NCP had not provided a response within the time limit</summary>
        SPIErrorWaitSectionTimeout = 0x15,
        /// <summary>The NCP response is missing the frame terminator</summary>
        SPIErrorNoFrameTerminator = 0x16,
        /// <summary>The NCP attempted to send an oversized command</summary>
        SPIErrorEZSPCommandOversized = 0x17,
        /// <summary>The host attempted to send an oversized response</summary>
        SPIErrorEZSPResponseOversized = 0x18,
        /// <summary>The host is still waiting for the NCP to send a response</summary>
        SPIWaitingForResponse = 0x19,
        /// <summary>The NCP has not performed a handshake within the allowed time</summary>
        SPIErrorHandshakeTimeout = 0x1A,
        /// <summary>The NCP has not performed a handshake after a reset</summary>
        SPIErrorStartupTimeout = 0x1B,
        /// <summary>The host could not verify the SPI activity and version number</summary>
        SPIErrorStartupFail = 0x1C,
        /// <summary>The host sent a command that is not supported</summary>
        SPIErrorUnsupportedSPICommand = 0x1D,
        /// <summary>Operation is not yet complete</summary>
        ASHInProgress = 0x20,
        /// <summary>Fatal error detected by host</summary>
        ASHHostFatalError = 0x21,
        /// <summary>Fatal error detected by the NCP</summary>
        ASHNCPFatalError = 0x22,
        /// <summary>Tried to send a data frame that is too long</summary>
        ASHDataFrameTooLong = 0x23,
        /// <summary>Tried to send a data frame that is too short</summary>
        ASHDataFrameTooShort = 0x24,
        /// <summary>No space for a transmitted data frame</summary>
        ASHNoTxSpace = 0x25,
        /// <summary>No space for a received data frame</summary>
        ASHNoRxSpace = 0x26,
        /// <summary>No received data available</summary>
        ASHNoRxData = 0x27,
        /// <summary>Not in the connected state</summary>
        ASHNotConnected = 0x28,
        /// <summary>The NCP received a command before the EZSP version has been set</summary>
        ErrorVersionNotSet = 0x30,
        /// <summary>The NCP received a command containing an unsupported frame</summary>
        ErrorInvalidFrameID = 0x31,
        /// <summary>The direction flag in the frame control field is incorrect</summary>
        ErrorWrongDirection = 0x32,
        /// <summary>The truncated flag in the frame control field was incorrect</summary>
        ErrorTruncated = 0x33,
        /// <summary>The overflow flag in the frame control field was set</summary>
        ErrorOverflow = 0x34,
        /// <summary>Insufficient memory</summary>
        ErrorOutOfMemory = 0x35,
        /// <summary>The value was out of bounds</summary>
        ErrorInvalidValue = 0x36,
        /// <summary>The configuration id was not recognized</summary>
        ErrorInvalidID = 0x37,
        /// <summary>Configuration values can no longer be modified</summary>
        ErrorInvalidCall = 0x38,
        /// <summary>The NCP failed to respond to the command</summary>
        ErrorNoResponse = 0x39,
        /// <summary>The length of the command exceeded the maximum EZSP frame length</summary>
        ErrorCommandTooLong = 0x40,
        /// <summary>The UART receive queue is full causing a callback to be dropped</summary>
        ErrorQueueFull = 0x41,
        /// <summary>Incompatible ASH version</summary>
        ASHErrorVersion = 0x50,
        /// <summary>Exceeded max Ack Timeouts</summary>
        ASHErrorTimeouts = 0x51,
        /// <summary>Timeout out waiting for the Reset Ack</summary>
        ASHErrorResetFail = 0x52,
        /// <summary>Unexpected NCP reset</summary>
        ASHErrorNCPReset = 0x53,
        /// <summary>Serial Port initialization failed</summary>
        ASHErrorSerialInit = 0x54,
        /// <summary>Invalid NCP processor type</summary>
        ASHErrorNCPType = 0x55,
        /// <summary>Invalid NCP reset method</summary>
        ASHErrorResetMethod = 0x56,
        /// <summary>XON/XOFF not supported by host driver</summary>
        ASHErrorXonXoff = 0x57,
        /// <summary>ASH protocol started</summary>
        ASHStarted = 0x70,
        /// <summary>ASH protocol connected</summary>
        ASHConnected = 0x72,
        /// <summary>ASH protocol disconnected</summary>
        ASHDisconnected = 0x72,
        /// <summary>Timer expired waiting for ack</summary>
        ASHAckTimeout = 0x73,
        /// <summary>Frame in progress cancelled</summary>
        ASHCancelled = 0x74,
        /// <summary>Received a frame out of sequence</summary>
        ASHOutOfSequence = 0x75,
        /// <summary>Frame CRC is bad</summary>
        ASHBadCRC = 0x76,
        /// <summary>Received the frame with comm error</summary>
        ASHCommError = 0x77,
        /// <summary>Received a frame with a bad ack number</summary>
        ASHBadAckNum = 0x78,
        /// <summary>Received a frame shorter than minimum</summary>
        ASHTooShort = 0x79,
        /// <summary>Received a frame longer than maximum</summary>
        ASHTooLong = 0x7A,
        /// <summary>Received a frame with an illegal control byte</summary>
        ASHBadControl = 0x7B,
        /// <summary>Received a frame with an illegal length for its type</summary>
        ASHBasLength = 0x7C,
        /// <summary>No reset or error</summary>
        ASHNoError = 0xFF,
    }

    /// <summary>
    /// Policy ID's
    /// </summary>
    public enum EzspPolicyID : byte
    {
        /// <summary>Controls trust center behavior</summary>
        TrustCenterPolicy = 0x00,
        /// <summary>Controls how external binding modification requests are handled</summary>
        BindingModificationPolicy = 0x01,
        /// <summary>Controls whether the host supplies unicast replies</summary>
        UnicastRepliesPolicy = 0x02,
        /// <summary>Controls whether pollHandler callbacks are generated</summary>
        PollHandlerPolicy = 0x03,
        /// <summary>Controls whether the message contents are included in the messageSentHandler</summary>
        MessageContentsInCallbackPolicy = 0x04,
        /// <summary>Controls whether the Trust Center will respond to Trust Center link key requests</summary>
        TrustCenterKeyRequestPolicy = 0x05,
        /// <summary>Controls whether the Trust Center will respond to application link key requests</summary>
        ApplicationKeyRequestPolicy = 0x06,
    }

    /// <summary>
    /// Decision ID's
    /// </summary>
    public enum EzspDecisionID : byte
    {
        /// <summary>Allow joins and rejoins and send the network key in the clear</summary>
        AllowJoins = 0x00,
        /// <summary>Send the network key encrypted with the joining or rejoining and rejoining device's trust center link key</summary>
        AllowPreconfiguredKeyJoins = 0x01,
        /// <summary>Send the network key encrypted with the rejoining device's link key</summary>
        AllowRejoinsOnly = 0x02,
        /// <summary>Disallow all unsecured join and rejoin attempts</summary>
        DisallowAllJoinsAndRejoins = 0x03,
        /// <summary>Send the network key in the clear to joining devices and rejoining devices are sent encrypted with the link key</summary>
        AllowJoinsRejoinsHaveLinkKey = 0x04,
        /// <summary>Do not allow the Binding table to be modified by remote nodes (Default)</summary>
        DisallowBindingModification = 0x10,
        /// <summary>Allow remote nodes to change the binding table.</summary>
        AllowBindingModification = 0x11,
        /// <summary>NCP automatically sends an empty reply for every unicast received (Default)</summary>
        HostWillNotSupplyReply = 0x20,
        /// <summary>The NCP will only send a reply if it receives a sendReply command from the host</summary>
        HostWillSupplyReply = 0x21,
        /// <summary>Do not inform the host when a child polls (Default)</summary>
        PollHandlerIgnore = 0x30,
        /// <summary>Generate a callback when a child polls</summary>
        PollHandlerCallback = 0x31,
        /// <summary>Include only the message tag in the message sent callback (Default)</summary>
        MessageTagOnlyInCallback = 0x40,
        /// <summary>Include both the message tag and the message contents in the message sent callback</summary>
        MessageTageAndContentsInCallback = 0x41,
        /// <summary>Ignore requests for the link key</summary>
        DenyTrustCenterRequests = 0x50,
        /// <summary>Allow requests for the link key</summary>
        AllowTrustCenterRequests = 0x51,
        /// <summary>Ignore requests for an application link key</summary>
        DenyAppKeyRequests = 0x60,
        /// <summary>Allow requests for an application link key</summary>
        AllowAppKeyRequests = 0x61,
    }

    /// <summary>
    /// Value ID's
    /// </summary>
    public enum EzspValueID : byte
    {
        /// <summary>The contents of the node data stack token</summary>
        TokenStackNodeData = 0x00,
        /// <summary>The types of MAC passthrough messages that the host wishes to receive</summary>
        MACPassthroughFlags = 0x01,
        /// <summary>The source address used to filter legacy EmberNet messages</summary>
        EmberNetPassthroughSourceAddress = 0x02,
        /// <summary>The number of available message buffers</summary>
        FreeBuffers = 0x03,
        /// <summary>Selects sending synchronous callbacks in ezsp-uart</summary>
        UARTSynchCallbacks = 0x04,
        /// <summary>The maximum incoming transfer size for the local node</summary>
        MaximumIncomingTransferSize = 0x05,
        /// <summary>The maximum outgoing transfer size for the local node</summary>
        MaximumOutgoingTransferSize = 0x06,
        /// <summary>A boolean indicating whether stack token are written to persistent storage</summary>
        StackTokenWriting = 0x07,
        /// <summary>A read only value indicating whether the stack is currently performing a rejoin</summary>
        StackIsPerformingRejoin = 0x08,
        /// <summary>A list of EmberMacFilterMatchData values</summary>
        MACFilterList = 0x09,
    }

    /// <summary>
    /// Manufacturer Token ID's
    /// </summary>
    public enum EzspMfgTokenID : byte
    {
        /// <summary>Custom Version (2 bytes)</summary>
        CustomVersion = 0x00,
        /// <summary>Manufacturing string (16 bytes)</summary>
        MfgString = 0x01,
        /// <summary>Board Name (16 bytes)</summary>
        BoardName = 0x02,
        /// <summary>Manufacturing ID (2 bytes)</summary>
        MfgID = 0x03,
        /// <summary>Radio Configuration (2 bytes)</summary>
        PhyConfiguration = 0x04,
        /// <summary>Bootload AES key (16 bytes)</summary>
        BootloadAESKey = 0x05,
        /// <summary>ASH configuration (40 bytes)</summary>
        ASHConfiguration = 0x06,
        /// <summary>EZSP storage (8 bytes)</summary>
        EzspStorage = 0x07,
        /// <summary>Radio Calibration data (64 bytes)</summary>
        StackCalibrationData = 0x08,
        /// <summary>CBKE data (92 bytes)</summary>
        CBKEData = 0x09,
        /// <summary>Installation code (20 bytes)</summary>
        InstallationCode = 0x0A,
        /// <summary>Radio channel filter calibration data (1 byte)</summary>
        StackCalibrationFilter = 0x0B,
    }

    /// <summary>
    /// Incoming message types
    /// </summary>
    public enum EmberIncomingMessageType : byte
    {
        /// <summary>Unicast message</summary>
        Unicast = 0x00,
        /// <summary>Unicast reply</summary>
        UnicastReply = 0x01,
        /// <summary>Multicast message</summary>
        Multicast = 0x02,
        /// <summary>Multicast message sent by the local device</summary>
        MulticastLoopback = 0x03,
        /// <summary>Broadcast message</summary>
        Broadcast = 0x04,
        /// <summary>Broadcast message sent by the local device</summary>
        BroadcastLoopback = 0x05,
        /// <summary>Many to One Route Request message</summary>
        ManyToOneRouteRequest = 0x06,
    }

    /// <summary>
    /// MAC pass through message type
    /// </summary>
    public enum EmberMacPassthroughType : byte
    {
        /// <summary>No MAC pass through messages</summary>
        None = 0x00,
        /// <summary>SE InterPAN message</summary>
        SEInterPAN = 0x01,
        /// <summary>Legacy EmberNet message</summary>
        EmberNet = 0x02,
        /// <summary>Legacy EmberNet message filtered by source address</summary>
        EmberNetSource = 0x04,
    }

    /// <summary>
    /// Key Establishment status
    /// </summary>
    public enum EmberKeyStatus : byte
    {
        /// <summary>Link Key Established</summary>
        LinkKeyEstablished = 0x01,
        /// <summary>Master Key Established</summary>
        MasterKeyEstablished = 0x02,
        /// <summary>Trust Center Link Key Established</summary>
        TrustCenterLinkKeyEstablished = 0x03,
        /// <summary>Key Establishment Timed Out</summary>
        KeyEstablishmentTimeout = 0x04,
        /// <summary>The Key table is full</summary>
        KeyTableFull = 0x05,
        /// <summary>The Trust Center reponded to the key request</summary>
        TrustCenterRespondedToKeyRequest = 0x06,
        /// <summary>The trust center sent the application key</summary>
        TrustCenterApplicationKeySentToRequester = 0x07,
        /// <summary>The Trust Center failed to respond to the key request</summary>
        TrustCenterResponseToKeyRequestFailed = 0x08,
        /// <summary>The requested key type is not supported</summary>
        TrustCenterRequestKeyTypeNotSupported = 0x09,
        /// <summary>The Trust Center does not have a link key for the requester</summary>
        TrustCenterNoLinkKeyForRequester = 0x0A,
        /// <summary>The EUI of the requester is not known</summary>
        TrustCenterRequesterEUIUnknown = 0x0B,
        /// <summary>The Trust Center received the first application key request</summary>
        TrustCenterReceivedFirstApplicationKeyRequest = 0x0C,
        /// <summary>The Trust Center timed out while waiting for the second request</summary>
        TrustCenterTimeoutWaitingForSecondApplicationKeyRequest = 0x0D,
        /// <summary>The Trust Center Received a non match application key request</summary>
        TrustCenterNonMatchingApplicationKeyRequestReceived = 0x0E,
        /// <summary>The Trust Center failed to send the application key</summary>
        TrustCenterFailedToSendApplicationKeys = 0x0F,
        /// <summary>The Trust Center failed to store the application key request</summary>
        TrustCenterFailedToStoreApplicationKeyRequest = 0x10,
        /// <summary>The Trust Center rejected the key request</summary>
        TrustCenterRejectedApplicationKeyRequest = 0x11,
    }

    /// <summary>
    /// Join Status
    /// </summary>
    public enum EmberDeviceUpdate : byte
    {
        /// <summary>Secure Rejoin using standard security</summary>
        StandardSecuritySecuredRejoin = 0x00,
        /// <summary>Unsecure join using standard security</summary>
        StandardSecurityUnsecuredJoin = 0x01,
        /// <summary>The device left the network</summary>
        DeviceLeft = 0x02,
        /// <summary>Unsecure rejoin using standard security</summary>
        StandardSecurityUnsecuredRejoin = 0x03,
        /// <summary>Secure rejoin using high security</summary>
        HighSecuritySecuredRejoin = 0x04,
        /// <summary>Unsecured join using high security</summary>
        HighSecurityUnsecuredJoin = 0x05,
        /// <summary>Unsecured rejoin using high security</summary>
        HighSecurityUnsecuredRejoin = 0x07
    }

    /// <summary>
    /// Join Policy Decision
    /// </summary>
    public enum EmberJoinDecision : byte
    {
        /// <summary>Allow the node to join but it should have a preconfigured key</summary>
        UsePreconfiguredKey = 0x00,
        /// <summary>Allow the node to join and send the network key in the clear</summary>
        SendKeyInTheClear = 0x01,
        /// <summary>Deny Join</summary>
        DenyJoin = 0x02,
        /// <summary>Take no action</summary>
        NoAction = 0x03,
    }

    #endregion

    /// <summary>
    /// EZSP Protocol class for interacting with the Telegesis dongle
    /// </summary>
    public class EZSPProtocol
    {
        #region Constants

        private const uint DEFAULT_RESPONSE_TIMEOUT = 3000;
        private const uint CBKE_RESPONSE_TIMEOUT = 11000;
        private const int MAX_APS_RETRIES = 3;

        #endregion

        #region Public Events

        /// <summary>
        /// Event that occurs when a Network or Energy scan has completed.
        /// </summary>
        public event EventHandler ScanCompleted;

        /// <summary>
        /// Event that occurs when a timer occurs
        /// </summary>
        public event TimerEventHandler TimerOccurred;

        /// <summary>
        /// Event that occurs when the stack status is updated
        /// </summary>
        public event StackStatusUpdatedHandler StackStatusUpdated;

        /// <summary>
        /// Event that occurs when a child is joining or leaving the node
        /// </summary>
        public event ChildJoinedHandler ChildJoined;

        /// <summary>
        /// Event that occurs when a binding is set from a remote node
        /// </summary>
        public event BindingSetRemotelyHandler BindingSetRemotely;

        /// <summary>
        /// Event that occurs when a binding is deleted from a remote node
        /// </summary>
        public event BindingDeletedRemotelyHandler BindingDeletedRemotely;

        /// <summary>
        /// Event that occurs when a message has been sent
        /// </summary>
        public event MessageSentHandler MessageSent;

        /// <summary>
        /// Event that occurs when a poll has completed;
        /// </summary>
        public event PollCompleteHandler PollComplete;

        /// <summary>
        /// Event that occurs when a poll is received from a child
        /// </summary>
        public event PollReceivedHandler PollReceived;

        /// <summary>
        /// Event that occurs when a message has been received containing the EUI of the sender
        /// </summary>
        public event SenderEUIReceivedHandler SenderEUIReceived;

        /// <summary>
        /// Event that occurs when a message has been received
        /// </summary>
        public event MessageReceivedHandler MessageReceived;

        /// <summary>
        /// Event that occurs when a route record has been received
        /// </summary>
        public event RouteRecordReceivedHandler RouteRecordReceived;

        /// <summary>
        /// Event that occurs when a many to one route is available for use
        /// </summary>
        public event ManyToOneRouteAvailableHandler ManyToOneRouteAvailable;

        /// <summary>
        /// Event that occurs when a route error has occurred
        /// </summary>
        public event RouteErrorOccurredHandler RouteErrorOccurred;

        /// <summary>
        /// Event that occurs when two nodes on the network are using the same node ID
        /// </summary>
        public event IDConflictDetectedHandler IDConflictDetected;

        /// <summary>
        /// Event that occurs when a MAC passthrough message has been received
        /// </summary>
        public event MacPassthroughMessageReceivedHandler MacPassthroughMessageReceived;

        /// <summary>
        /// Event that occurs when a MAC passthrough message that matches an application filter has been received
        /// </summary>
        public event MacFilterMatchMessageReceivedHandler MacFilterMatchMessageReceived;

        /// <summary>
        /// Event that occurs when a Raw Message has been sent
        /// </summary>
        public event RawMessageSentHandler RawMessageSent;

        /// <summary>
        /// Event that occurs when the Network Key has been switched
        /// </summary>
        public event NetworkKeySwitchedHandler NetworkKeySwitched;

        /// <summary>
        /// Event that occurs when a ZigBee Key has been established
        /// </summary>
        public event ZigBeeKeyEstablisedHandler ZigBeeKeyEstablished;

        /// <summary>
        /// Event that occurs when the Trust Center has handled a join request
        /// </summary>
        public event TrustCenterJoinedHandler TrustCenterJoined;

        /// <summary>
        /// Event that occurs when a CBKE key has been generated
        /// </summary>
        public event CBKEKeyGeneratedHandler CBKEKeyGenerated;

        /// <summary>
        /// Event that occurs when the SMAC values have been calculated
        /// </summary>
        public event SmacsCalculatedHandler SmacsCalculated;

        /// <summary>
        /// Event that occurs when the DSA Sign has completed
        /// </summary>
        public event DsaSignedHandler DsaSigned;

        /// <summary>
        /// Event that occurs when the DSA Verify has completed
        /// </summary>
        public event DsaVerifiedHandler DsaVerified;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ash">ASH Protocol layer to use</param>
        /// <param name="logger">The EZSP communication logger</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EZSPProtocol(ASHProtocol ash, EZSPLogger logger)
        {
            m_ASH = ash;
            m_Connected = false;
            m_Joined = false;
            m_CurrentSequenceNumber = 0;
            m_FrameReceivedHandler = new EventHandler(m_ASH_FrameReceived);

            m_Logger = logger;

            m_HandlingFragmentedMessage = false;
            m_HaveFragmentConfigurationData = false;
            m_FragmentsReceived = new List<IncomingMessage>();

            m_CommandResponses = new List<EZSPResponseFrame>();
            m_AsyncCallbacks = new List<EZSPResponseFrame>();

            m_ActiveScanResults = new List<ZigbeeNetwork>();
            m_EnergyScanResults = new List<ZigBeeEnergyScanResult>();

            m_FragmentWindowTimer = new Timer(new TimerCallback(FragmentWindowTimerCallback));
        }

        /// <summary>
        /// Connects the EZSP protocol layer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void Connect()
        {
            if (m_ASH != null)
            {
                if (m_ASH.Connected == false)
                {
                    m_ASH.Connect();

                    if (m_ASH.Connected)
                    {
                        m_ASH.FrameReceived += m_FrameReceivedHandler;

                        m_CommandResponses.Clear();
                        m_AsyncCallbacks.Clear();

                        m_Joined = false;
                        m_CurrentSequenceNumber = 0;
                        m_Connected = true;
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("No ASH Layer object was specified to connect to");
            }
        }

        /// <summary>
        /// Disconnects the EZSP protocol layer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void Disconnect()
        {
            if (m_ASH != null && m_ASH.Connected)
            {
                m_ASH.Disconnect();
                m_ASH.FrameReceived -= m_FrameReceivedHandler;
            }

            m_Connected = false;
            m_Joined = false;
        }

        /// <summary>
        /// Gets the fragmentation configuration data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/31/12 RCG 2.70.01        Created

        public void GetFragmentationConfiguration()
        {
            EzspStatus Status;

            // Get the fragment config info
            GetConfigurationValue(EzspConfigID.APSAckTimeout, out Status, out m_FragmentTimeout);

            if (Status == EzspStatus.Success)
            {
                GetConfigurationValue(EzspConfigID.FragmentWindowSize, out Status, out m_FragmentWindowSize);
            }

            if (Status == EzspStatus.Success)
            {
                m_HaveFragmentConfigurationData = true;
                m_Logger.WriteLine(EZSPLogLevels.EZSPProtocol, "Successfully retrieved fragment handling configuration");
            }
            else
            {
                m_Logger.WriteLine(EZSPLogLevels.EZSPProtocol, "Failed to retrieve fragment handling configuration.");
            }
        }

        #region Configuration Frames

        /// <summary>
        /// Checks the EZSP protocol version
        /// </summary>
        /// <param name="desiredVersion">The version that is desired</param>
        /// <param name="protocolVersion">The protocol version</param>
        /// <param name="stackType">The stack type</param>
        /// <param name="stackVersion">The stack version</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void Version(byte desiredVersion, out byte protocolVersion, out byte stackType, out ushort stackVersion)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            protocolVersion = 0;
            stackType = 0;
            stackVersion = 0;

            // Set up the Parameter Data
            ParameterData[0] = desiredVersion;

            SendCommand(EZSPFrameIDs.Version, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 4)
            {
                MemoryStream ResponseParameterStream = new MemoryStream(ResponseData);
                BinaryReader ResponseParameterReader = new BinaryReader(ResponseParameterStream);

                protocolVersion = ResponseParameterReader.ReadByte();
                stackType = ResponseParameterReader.ReadByte();
                stackVersion = ResponseParameterReader.ReadUInt16();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the specified configuration value
        /// </summary>
        /// <param name="configID">The configuration value to get</param>
        /// <param name="status">The status of the command</param>
        /// <param name="value">The value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetConfigurationValue(EzspConfigID configID, out EzspStatus status, out ushort value)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = (byte)configID;

            SendCommand(EZSPFrameIDs.GetConfigurationValue, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 3)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EzspStatus)ResponseReader.ReadByte();
                value = ResponseReader.ReadUInt16();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the specified configuration value.
        /// </summary>
        /// <param name="configID">The ID of the configuration value</param>
        /// <param name="value">The new value</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void SetConfigurationValue(EzspConfigID configID, ushort value, out EzspStatus status)
        {
            byte[] ParameterData = new byte[3];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Set up the Parameter Data
            ParameterWriter.Write((byte)configID);
            ParameterWriter.Write(value);

            SendCommand(EZSPFrameIDs.SetConfigurationValue, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EzspStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Configures endpoint information on the NCP
        /// </summary>
        /// <param name="endpoint">The application endpoint to be added</param>
        /// <param name="profileID">The application profile ID</param>
        /// <param name="deviceID">The device ID within the application profile</param>
        /// <param name="appFlags">The device version and flags indicating description availability</param>
        /// <param name="inputClusterCount">The number of cluster IDs in the input cluster list</param>
        /// <param name="outputClusterCount">The number of cluster IDs in the output cluster list</param>
        /// <param name="inputClusterList">Input cluster IDs the endpoint will accept</param>
        /// <param name="outputClusterList">Output cluster IDs the endpoint will accept</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void AddEndpoint(byte endpoint, ushort profileID, ushort deviceID, byte appFlags, byte inputClusterCount,
            byte outputClusterCount, ushort[] inputClusterList, ushort[] outputClusterList, out EzspStatus status)
        {
            byte[] ParameterData = new byte[8 + 2*inputClusterCount + 2*outputClusterCount];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Validate the parameters
            if (inputClusterList == null || inputClusterCount != inputClusterList.Length)
            {
                throw new ArgumentException("The input cluster list cannot be null and the length must match the specified count", "inputClusterList");
            }

            if (outputClusterList == null || outputClusterCount != outputClusterList.Length)
            {
                throw new ArgumentException("The output cluster list cannot be null and the length must match the specified count", "outputClusterList");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(endpoint);
            ParameterWriter.Write(profileID);
            ParameterWriter.Write(deviceID);
            ParameterWriter.Write(appFlags);
            ParameterWriter.Write(inputClusterCount);
            ParameterWriter.Write(outputClusterCount);

            foreach (ushort CurrentCluster in inputClusterList)
            {
                ParameterWriter.Write(CurrentCluster);
            }

            foreach (ushort CurrentCluster in outputClusterList)
            {
                ParameterWriter.Write(CurrentCluster);
            }

            SendCommand(EZSPFrameIDs.AddEndpoint, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EzspStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Change the policies used by the NCP to make fast decisions
        /// </summary>
        /// <param name="policyID">The policy to modify</param>
        /// <param name="decisionID">The new decision for the specified policy</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetPolicy(EzspPolicyID policyID, EzspDecisionID decisionID, out EzspStatus status)
        {
            byte[] ParameterData = new byte[2];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = (byte)policyID;
            ParameterData[1] = (byte)decisionID;

            SendCommand(EZSPFrameIDs.SetPolicy, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EzspStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the policy decisions
        /// </summary>
        /// <param name="policyID">The policy to get</param>
        /// <param name="status">The status of the command</param>
        /// <param name="decisionID">The policy decision</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetPolicy(EzspPolicyID policyID, out EzspStatus status, out EzspDecisionID decisionID)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = (byte)policyID;

            SendCommand(EZSPFrameIDs.GetPolicy, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 2)
            {
                status = (EzspStatus)ResponseData[0];
                decisionID = (EzspDecisionID)ResponseData[1];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets a value from the NCP
        /// </summary>
        /// <param name="valueID">The value to get</param>
        /// <param name="status">The status of the command</param>
        /// <param name="valueLength">The length of the value</param>
        /// <param name="value">The value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetValue(EzspValueID valueID, out EzspStatus status, out byte valueLength, out byte[] value)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = (byte)valueID;

            SendCommand(EZSPFrameIDs.GetValue, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length > 2)
            {
                status = (EzspStatus)ResponseData[0];
                valueLength = ResponseData[1];

                value = new byte[valueLength];
                Array.Copy(ResponseData, 2, value, 0, valueLength);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the NCP value
        /// </summary>
        /// <param name="valueID">The ID of the value to set</param>
        /// <param name="valueLength">The length of the value data</param>
        /// <param name="value">The value</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetValue(EzspValueID valueID, byte valueLength, byte[] value, out EzspStatus status)
        {
            byte[] ParameterData = new byte[2 + valueLength];
            byte[] ResponseData = null;

            // Validate the parameter data
            if (value == null || valueLength != value.Length)
            {
                throw new ArgumentException("The value can not be null and the length must match the valueLength parameter", "value");
            }

            // Set up the Parameter Data
            ParameterData[0] = (byte)valueID;
            ParameterData[1] = valueLength;
            Array.Copy(value, 0, ParameterData, 2, valueLength);

            SendCommand(EZSPFrameIDs.SetValue, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EzspStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        #endregion

        #region Utility Frames

        /// <summary>
        /// Sends a command that does nothing
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void Nop()
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            SendCommand(EZSPFrameIDs.Nop, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null || ResponseData.Length != 0)
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Echo's the sent data back to the host
        /// </summary>
        /// <param name="dataLength">The length of the data sent</param>
        /// <param name="data">The data sent</param>
        /// <param name="echoLength">The length of the data echoed</param>
        /// <param name="echo">The echoed data</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void Echo(byte dataLength, byte[] data, out byte echoLength, out byte[] echo)
        {
            byte[] ParameterData = new byte[1 + dataLength];
            byte[] ResponseData = null;

            // Validate the parameters
            if (data == null || dataLength != data.Length)
            {
                throw new ArgumentException("The data cannot be null and must be the specified length", "data");
            }

            // Set up the Parameter Data
            ParameterData[0] = dataLength;
            Array.Copy(data, 0, ParameterData, 1, dataLength);

            SendCommand(EZSPFrameIDs.Echo, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length > 1)
            {
                echoLength = ResponseData[0];
                echo = new byte[echoLength];
                Array.Copy(ResponseData, 1, echo, 0, echoLength);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Allows the NCP to respond with a callback
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void Callback()
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            SendCommand(EZSPFrameIDs.Callback, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData == null || ResponseData.Length != 0)
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the specified token data
        /// </summary>
        /// <param name="tokenID">The ID of the token to set</param>
        /// <param name="tokenData">The token data (8 bytes)</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetToken(byte tokenID, byte[] tokenData, out EmberStatus status)
        {
            byte[] ParameterData = new byte[9];
            byte[] ResponseData = null;

            // Validate the parameter data
            if (tokenData == null || tokenData.Length != 8)
            {
                throw new ArgumentException("The token data must not be null and must be 8 bytes long", "tokenData");
            }

            // Set up the Parameter Data
            ParameterData[0] = tokenID;
            Array.Copy(tokenData, 0, ParameterData, 1, 8);

            SendCommand(EZSPFrameIDs.SetToken, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the specified token data
        /// </summary>
        /// <param name="tokenID">The ID of the token to get</param>
        /// <param name="status">The status of the command</param>
        /// <param name="tokenData">The token data</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetToken(byte tokenID, out EmberStatus status, out byte[] tokenData)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = tokenID;

            SendCommand(EZSPFrameIDs.GetToken, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 9)
            {
                status = (EmberStatus)ResponseData[0];
                tokenData = new byte[8];
                Array.Copy(ResponseData, 1, tokenData, 0, 8);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the specified Manufacturer Token
        /// </summary>
        /// <param name="tokenID">The token to get</param>
        /// <param name="tokenDataLength">The length of the data</param>
        /// <param name="tokenData">The token data</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetMfgToken(EzspMfgTokenID tokenID, out byte tokenDataLength, out byte[] tokenData)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = (byte)tokenID;

            SendCommand(EZSPFrameIDs.GetMFGToken, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length > 1)
            {
                tokenDataLength = ResponseData[0];
                tokenData = new byte[tokenDataLength];
                Array.Copy(ResponseData, 1, tokenData, 0, tokenDataLength);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets a random number from the NCP
        /// </summary>
        /// <param name="status">The status of the command</param>
        /// <param name="value">The random number</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetRandomNumber(out EmberStatus status, out ushort value)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            SendCommand(EZSPFrameIDs.GetRandomNumber, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 3)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                value = ResponseReader.ReadUInt16();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets a timer on the NCP
        /// </summary>
        /// <param name="timerID">The ID of the timer to set</param>
        /// <param name="time">The delay before the call back will be generated</param>
        /// <param name="units">The units for time</param>
        /// <param name="repeat">Whether or not the timer will repeat</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetTimer(byte timerID, ushort time, EmberEventUnits units, bool repeat, out EmberStatus status)
        {
            byte[] ParameterData = new byte[5];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Set up the Parameter Data
            ParameterWriter.Write(timerID);
            ParameterWriter.Write(time);
            ParameterWriter.Write((byte)units);
            ParameterWriter.Write(repeat);

            SendCommand(EZSPFrameIDs.SetTimer, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the specified timer
        /// </summary>
        /// <param name="timerID">The ID of the timer to get</param>
        /// <param name="time">The amount of time between callbacks</param>
        /// <param name="units">The units for time</param>
        /// <param name="repeat">Whether or not this timer repeats</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetTimer(byte timerID, out ushort time, out EmberEventUnits units, bool repeat)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = timerID;

            SendCommand(EZSPFrameIDs.GetTimer, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 4)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                time = ResponseReader.ReadUInt16();
                units = (EmberEventUnits)ResponseReader.ReadByte();
                repeat = ResponseReader.ReadBoolean();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Writes a debug message
        /// </summary>
        /// <param name="binaryMessage">Whether or not the message should be interpreted as binary data</param>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The message</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void DebugWrite(bool binaryMessage, byte messageLength, byte[] messageContents, out EmberStatus status)
        {
            byte[] ParameterData = new byte[2 + messageLength];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Validate the parameters
            if (messageContents == null || messageLength != messageContents.Length)
            {
                throw new ArgumentException("The message contented cannot be null and must be the specified length", "messageContents");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(binaryMessage);
            ParameterWriter.Write(messageLength);
            ParameterWriter.Write(messageContents);

            SendCommand(EZSPFrameIDs.DebugWrite, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Retrieves and clears the Ember Counters
        /// </summary>
        /// <param name="values">The counter values prior to being reset</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void ReadAndClearCounters(out ushort[] values)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            SendCommand(EZSPFrameIDs.ReadAndClearCounters, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 29*2)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                values = new ushort[29];

                for (int iIndex = 0; iIndex < 29; iIndex++)
                {
                    values[iIndex] = ResponseReader.ReadUInt16();
                }
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Tests that the UART flow control is working correctly
        /// </summary>
        /// <param name="delay">Amount of time that data will not be read from the host</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void DelayTest(ushort delay)
        {
            byte[] ParameterData = new byte[2];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Set up the Parameter Data
            ParameterWriter.Write(delay);

            SendCommand(EZSPFrameIDs.DelayTest, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData == null || ResponseData.Length != 0)
            {            
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Retrieves the status of the passed library ID to determine if it is compiled into the stack
        /// </summary>
        /// <param name="libraryID">The ID of the library to check</param>
        /// <param name="status">The status of the library</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetLibraryStatus(byte libraryID, out byte status)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = libraryID;

            SendCommand(EZSPFrameIDs.GetLibraryStatus, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        #endregion

        #region Networking Frames

        /// <summary>
        /// Sets the Manufacturer Code of the node
        /// </summary>
        /// <param name="code">The code to set</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetManufacturerCode(ushort code)
        {
            byte[] ParameterData = new byte[2];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = (byte)(code >> 1);
            ParameterData[1] = (byte)code;

            SendCommand(EZSPFrameIDs.SetManufacturerCode, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData == null || ResponseData.Length != 0)
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the power descriptor
        /// </summary>
        /// <param name="descriptor">The power descriptor to set</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetPowerDescriptor(ushort descriptor)
        {
            byte[] ParameterData = new byte[2];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            ParameterData[0] = (byte)(descriptor >> 8);
            ParameterData[1] = (byte)descriptor;

            SendCommand(EZSPFrameIDs.SetPowerDescriptor, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData == null || ResponseData.Length != 0)
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Scans for ZigBee Networks
        /// </summary>
        /// <param name="scanType">The type of network scan to perform</param>
        /// <param name="channels">The channels to scan</param>
        /// <param name="duration">The duration of the scan</param>
        /// <param name="status">The result of the start scan command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void StartScan(EZSPScanType scanType, ZigBeeChannels channels, EZSPScanDuration duration, out EmberStatus status)
        {
            byte[] ParameterData = new byte[6];
            byte[] ResponseData = null;

            status = EmberStatus.MACJoinedNetwork;

            if (scanType == EZSPScanType.ActiveScan)
            {
                m_ActiveScanResults.Clear();
            }
            else
            {
                m_EnergyScanResults.Clear();
            }

            // Set up the Parameter Data
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            ParameterWriter.Write((byte)scanType);
            ParameterWriter.Write((uint)channels);
            ParameterWriter.Write((byte)duration);

            SendCommand(EZSPFrameIDs.StartScan, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Stops the device from scanning
        /// </summary>
        /// <returns>The result of the command</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void StopScan(out EmberStatus status)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberStatus.InvalidCall;

            SendCommand(EZSPFrameIDs.StopScan, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Resumes network operation after a reboot and maintains it's original type
        /// </summary>
        /// <returns>The result of Network Init. NotJoined if the node is not part of a network</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void NetworkInit(out EmberStatus status)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberStatus.InvalidCall;

            SendCommand(EZSPFrameIDs.NetworkInit, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the Current Network State from the device
        /// </summary>
        /// <returns>The network state.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void NetworkState(out EmberNetworkStatus status)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberNetworkStatus.NoNetwork;

            SendCommand(EZSPFrameIDs.NetworkState, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberNetworkStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Forms a network by becoming the coordinator
        /// </summary>
        /// <param name="parameters">The specification for the new network</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void FormNetwork(EmberNetworkParameters parameters, out EmberStatus status)
        {
            byte[] ParameterData = null;
            byte[] ResponseData = null;

            status = EmberStatus.InvalidCall;

           // Set up the Parameter Data
            if (parameters != null)
            {
                ParameterData = parameters.RawData;
            }
            else
            {
                throw new ArgumentNullException("parameters", "The Network Parameters may not be null");
            }

            SendCommand(EZSPFrameIDs.FormNetwork, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Joins the specified network
        /// </summary>
        /// <param name="nodeType">The node type</param>
        /// <param name="parameters">The specification for the new network</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void JoinNetwork(EmberNodeType nodeType, EmberNetworkParameters parameters, out EmberStatus status)
        {
            byte[] ParameterData = new byte[21];
            byte[] ResponseData = null;

            status = EmberStatus.InvalidCall;

            // Set up the Parameter Data
            ParameterData[0] = (byte)nodeType;
            Array.Copy(parameters.RawData, 0, ParameterData, 1, 20);

            SendCommand(EZSPFrameIDs.JoinNetwork, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Leaves the current network
        /// </summary>
        /// <returns>The result of the operation.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void LeaveNetwork(out EmberStatus status)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberStatus.InvalidCall;

            SendCommand(EZSPFrameIDs.LeaveNetwork, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Scans for and rejoins the network
        /// </summary>
        /// <param name="haveCurrentNetworkKey">Whether or not we already have the current Network Key</param>
        /// <param name="channels">The channel mask to use when scanning for networks</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void FindAndRejoinNetwork(bool haveCurrentNetworkKey, ZigBeeChannels channels, out EmberStatus status)
        {
            byte[] ParameterData = new byte[5];
            byte[] ResponseData = null;

            status = EmberStatus.InvalidCall;

            // Set up the Parameter Data
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            ParameterWriter.Write(haveCurrentNetworkKey);
            ParameterWriter.Write((uint)channels);

            SendCommand(EZSPFrameIDs.FindAndRejoinNetwork, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Allows other nodes to join the network for a period of time
        /// </summary>
        /// <param name="duration">The number of seconds to allow joining. 0xFF permanently enables joining</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void PermitJoining(byte duration, out EmberStatus status)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.InvalidCall;

            // Set up the Parameter Data
            ParameterData[0] = duration;

            SendCommand(EZSPFrameIDs.PermitJoining, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the EUI64 ID of the local node
        /// </summary>
        /// <returns>The EUI64</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetEUI64(out ulong eui)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            eui = 0;

            SendCommand(EZSPFrameIDs.GetEUI64, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 8)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                eui = ResponseReader.ReadUInt64();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the Node ID of the local node
        /// </summary>
        /// <returns>The EUI64</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  10/31/12 PGH 2.70.36        Reversed Response Data

        public void GetNodeID(out ushort nodeID)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            nodeID = 0;

            SendCommand(EZSPFrameIDs.GetNodeID, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 2)
            {
                Array.Reverse(ResponseData);
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                nodeID = ResponseReader.ReadUInt16();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the current network parameters
        /// </summary>
        /// <param name="status">The status of the network</param>
        /// <param name="nodeType">The node type</param>
        /// <param name="networkParameters">The network parameters</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetNetworkParameters(out EmberStatus status, out EmberNodeType nodeType, out EmberNetworkParameters networkParameters)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            nodeType = EmberNodeType.UnknownDevice;
            networkParameters = null;

            SendCommand(EZSPFrameIDs.GetNetworkParameters, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 22)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                nodeType = (EmberNodeType)ResponseReader.ReadByte();

                networkParameters = new EmberNetworkParameters();
                networkParameters.RawData = ResponseReader.ReadBytes(20);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the parent child parameters
        /// </summary>
        /// <param name="childCount">The number of children the current node has</param>
        /// <param name="parentEUI">The EUI of the parent</param>
        /// <param name="parentNodeID">The parent's Node ID</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetParentChildParameters(out byte childCount, out ulong parentEUI, out ushort parentNodeID)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            childCount = 0;
            parentEUI = 0;
            parentNodeID = 0;

            SendCommand(EZSPFrameIDs.GetParentChildParameters, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 11)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                childCount = ResponseReader.ReadByte();
                parentEUI = ResponseReader.ReadUInt64();
                parentNodeID = ResponseReader.ReadUInt16();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the information about a child node
        /// </summary>
        /// <param name="index">The index of the child to get</param>
        /// <param name="status">The status of the child</param>
        /// <param name="childNodeID">The Node ID of the child</param>
        /// <param name="childEUI">The EUI of the child</param>
        /// <param name="childNodeType">The child's node type</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetChildData(byte index, out EmberStatus status, out ushort childNodeID, out ulong childEUI, out EmberNodeType childNodeType)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            childNodeID = 0;
            childEUI = 0;
            childNodeType = EmberNodeType.UnknownDevice;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.GetChildData, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 12)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                childNodeID = ResponseReader.ReadUInt16();
                childEUI = ResponseReader.ReadUInt64();
                childNodeType = (EmberNodeType)ResponseReader.ReadByte();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the specified neighbor
        /// </summary>
        /// <param name="index">The index of the neighbor to get</param>
        /// <param name="status">The status of the request</param>
        /// <param name="neighborTableEntry">The Neighbor Table entry for the specified neighbor</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetNeighbor(byte index, out EmberStatus status, out EmberNeighborTableEntry neighborTableEntry)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            neighborTableEntry = null;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.GetNeighbor, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 15)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                neighborTableEntry = new EmberNeighborTableEntry();
                neighborTableEntry.RawData = ResponseReader.ReadBytes(14);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the number of neighbors for the current node
        /// </summary>
        /// <param name="count">The number of neighbors</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void NeighborCount(out byte count)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            count = 0;

            // Set up the Parameter Data

            SendCommand(EZSPFrameIDs.NeighborCount, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                count = ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the requested route table entry
        /// </summary>
        /// <param name="index">The index of the route table entry</param>
        /// <param name="status">The status of the request</param>
        /// <param name="routeTableEntry">The route table entry</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetRouteTableEntry(byte index, out EmberStatus status, out EmberRouteTableEntry routeTableEntry)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            routeTableEntry = null;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.GetRouteTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 9)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                routeTableEntry = new EmberRouteTableEntry();
                routeTableEntry.RawData = ResponseReader.ReadBytes(8);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the radio power to the specified value
        /// </summary>
        /// <param name="power">The desired power</param>
        /// <param name="status">The result of the request</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetRadioPower(sbyte power, out EmberStatus status)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterWriter.Write(power);

            SendCommand(EZSPFrameIDs.SetRadioPower, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the channel of the radio
        /// </summary>
        /// <param name="channel">The channel that the radio is operating on</param>
        /// <param name="status">The result of the request</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetRadioChannel(byte channel, out EmberStatus status)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterData[0] = channel;

            SendCommand(EZSPFrameIDs.SetRadioChannel, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        #endregion

        #region Binding Frames

        /// <summary>
        /// Clears the binding table of all entries
        /// </summary>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void ClearBindingTable(out EmberStatus status)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            SendCommand(EZSPFrameIDs.ClearBindingTable, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the binding at the specified index.
        /// </summary>
        /// <param name="index">The index of the binding to set</param>
        /// <param name="entry">The binding entry</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetBinding(byte index, EmberBindingTableEntry entry, out EmberStatus status)
        {
            byte[] ParameterData = new byte[14];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterData[0] = index;
            Array.Copy(entry.RawData, 0, ParameterData, 1, 13);

            SendCommand(EZSPFrameIDs.SetBinding, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the specified binding table entry
        /// </summary>
        /// <param name="index">The index of the binding to get</param>
        /// <param name="status">The status of the command</param>
        /// <param name="entry">The binding entry requested</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetBinding(byte index, out EmberStatus status, out EmberBindingTableEntry entry)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            entry = null;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.GetBinding, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 14)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                entry = new EmberBindingTableEntry();
                entry.RawData = ResponseReader.ReadBytes(13);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Deletes the binding at the specified index.
        /// </summary>
        /// <param name="index">The index of the binding to delete</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void DeleteBinding(byte index, out EmberStatus status)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.DeleteBinding, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets whether or not the binding at the specified index is currently active
        /// </summary>
        /// <param name="index">The index of the binding to check</param>
        /// <param name="isActive">Whether or not the binding is active</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void BindingIsActive(byte index, out bool isActive)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            isActive = false;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.BindingIsActive, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                if (ResponseData[0] > 0)
                {
                    isActive = true;
                }
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the Node ID for the remote device associated with the specified binding
        /// </summary>
        /// <param name="index">The index of the binding</param>
        /// <param name="nodeID">The remote Node ID</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetBindingRemoteNodeID(byte index, ushort nodeID)
        {
            byte[] ParameterData = new byte[3];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            ParameterWriter.Write(index);
            ParameterWriter.Write(nodeID);

            SendCommand(EZSPFrameIDs.GetBindingRemoteNodeID, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData == null || ResponseData.Length != 0)
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the Node ID for the specified binding
        /// </summary>
        /// <param name="index">The index of the binding to set</param>
        /// <param name="nodeID">The Node ID of the remote device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetBindingRemoteID(byte index, ushort nodeID)
        {
            byte[] ParameterData = new byte[3];
            byte[] ResponseData = null;

            // Set up the Parameter Data
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            ParameterWriter.Write(index);
            ParameterWriter.Write(nodeID);

            SendCommand(EZSPFrameIDs.SetBindingRemoteNodeID, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData == null || ResponseData.Length != 0)
            {            
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        #endregion

        #region Messaging Frames

        /// <summary>
        /// Returns the maximum size of the payload
        /// </summary>
        /// <param name="length">The maximum APS payload length</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void MaximumPayloadLength(out byte length)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            length = 0;

            SendCommand(EZSPFrameIDs.MaximumPayloadLength, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                length = ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sends a unicast message 
        /// </summary>
        /// <param name="type">The outgoing message type. Must be Direct, via Address Table, or via Binding</param>
        /// <param name="indexOrDestination">Depending on the type this is the Node ID of the destination or the index into the Address/Binding table</param>
        /// <param name="apsFrame">The APS frame to be added to the message</param>
        /// <param name="messageTag">A host chosen value used to keep track of the message</param>
        /// <param name="messageLength">The length of the message contents</param>
        /// <param name="messageContents">The contents of the message</param>
        /// <param name="status">The status of the command</param>
        /// <param name="sequence">The sequence number that will be used to transmit the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SendUnicast(EmberOutgoingMessageType type, ushort indexOrDestination, EmberApsFrame apsFrame, byte messageTag, 
            byte messageLength, byte[] messageContents, out EmberStatus status, out byte sequence)
        {
            byte[] ParameterData = new byte[16 + messageLength];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;
            sequence = 0;

            // Verify that the parameters are valid

            if (type == EmberOutgoingMessageType.Broadcast || type == EmberOutgoingMessageType.Multicast)
            {
                throw new ArgumentException("Can not send a unicast message using Broadcast or Multicast message types", "type");
            }

            if (apsFrame == null)
            {
                throw new ArgumentNullException("apsFrame", "The apsFrame parameter can not be null when sending a message");
            }

            if (messageContents == null || messageContents.Length != messageLength)
            {
                throw new ArgumentException("The length of the message contents must match the specified message length", "messageContents");
            }

            // Set up the Parameter Data
            ParameterWriter.Write((byte)type);
            ParameterWriter.Write(indexOrDestination);
            ParameterWriter.Write(apsFrame.RawData);
            ParameterWriter.Write(messageTag);
            ParameterWriter.Write(messageLength);
            ParameterWriter.Write(messageContents);

            SendCommand(EZSPFrameIDs.SendUnicast, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 2)
            {
                status = (EmberStatus)ResponseData[0];
                sequence = ResponseData[1];
            }
            else
            {
                // throw new InvalidDataException("The response parameter data size is invalid");
                Console.WriteLine("The response parameter data size is invalid");
            }

        }

        /// <summary>
        /// Send a broadcast message
        /// </summary>
        /// <param name="destination">The destination where the broadcast should be sent</param>
        /// <param name="apsFrame">The APS frame for the message</param>
        /// <param name="radius">The number of hops from the sender the message should be sent</param>
        /// <param name="messageTag">A host chosen value used to keep track of the message</param>
        /// <param name="messageLength">The length of the message contents</param>
        /// <param name="messageContents">The contents of the message</param>
        /// <param name="status">The status of the command</param>
        /// <param name="sequence">The sequence number that will be used to transmit the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SendBroadcast(ushort destination, EmberApsFrame apsFrame, byte radius, byte messageTag, byte messageLength, 
            byte[] messageContents, out EmberStatus status, out byte sequence)
        {
            byte[] ParameterData = new byte[16 + messageLength];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;
            sequence = 0;

            // Verify that the parameters are valid

            if (apsFrame == null)
            {
                throw new ArgumentNullException("apsFrame", "The apsFrame parameter can not be null when sending a message");
            }            

            if (messageContents == null || messageContents.Length != messageLength)
            {
                throw new ArgumentException("The length of the message contents must match the specified message length", "messageContents");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(destination);
            ParameterWriter.Write(apsFrame.RawData);
            ParameterWriter.Write(radius);
            ParameterWriter.Write(messageTag);
            ParameterWriter.Write(messageLength);
            ParameterWriter.Write(messageContents);

            SendCommand(EZSPFrameIDs.SendBroadcast, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 2)
            {
                status = (EmberStatus)ResponseData[0];
                sequence = ResponseData[1];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Send a multicast message
        /// </summary>
        /// <param name="apsFrame">The APS frame for the message</param>
        /// <param name="hops">The number of hops from the sender the message should be sent</param>
        /// <param name="nonMemberRadius">The number of hops the message will be forwarded for devices that are not a member of the group</param>
        /// <param name="messageTag">A host chosen value used to keep track of the message</param>
        /// <param name="messageLength">The length of the message contents</param>
        /// <param name="messageContents">The contents of the message.</param>
        /// <param name="status">The status of the command</param>
        /// <param name="sequence">The sequence number used to transmit the messsage.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SendMulticast(EmberApsFrame apsFrame, byte hops, byte nonMemberRadius, byte messageTag, byte messageLength, byte[] messageContents,
            out EmberStatus status, out byte sequence)
        {
            byte[] ParameterData = new byte[15 + messageLength];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;
            sequence = 0;

            // Verify that the parameters are valid

            if (apsFrame == null)
            {
                throw new ArgumentNullException("apsFrame", "The apsFrame parameter can not be null when sending a message");
            }

            if (messageContents == null || messageContents.Length != messageLength)
            {
                throw new ArgumentException("The length of the message contents must match the specified message length", "messageContents");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(apsFrame.RawData);
            ParameterWriter.Write(hops);
            ParameterWriter.Write(nonMemberRadius);
            ParameterWriter.Write(messageTag);
            ParameterWriter.Write(messageLength);
            ParameterWriter.Write(messageContents);

            SendCommand(EZSPFrameIDs.SendMulticast, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 2)
            {
                status = (EmberStatus)ResponseData[0];
                sequence = ResponseData[1];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Send a reply message to the specified node
        /// </summary>
        /// <param name="senderNodeID">The Node ID of the device to reply to</param>
        /// <param name="apsFrame">The APS frame for the reply</param>
        /// <param name="messageLength">The length of the message contents</param>
        /// <param name="messageContents">The contents of the message</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SendReply(ushort senderNodeID, EmberApsFrame apsFrame, byte messageLength, byte[] messageContents, out EmberStatus status)
        {
            byte[] ParameterData = new byte[14 + messageLength];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            // Verify that the parameters are valid

            if (apsFrame == null)
            {
                throw new ArgumentNullException("apsFrame", "The apsFrame parameter can not be null when sending a message");
            }

            if (messageContents == null || messageContents.Length != messageLength)
            {
                throw new ArgumentException("The length of the message contents must match the specified message length", "messageContents");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(senderNodeID);
            ParameterWriter.Write(apsFrame.RawData);
            ParameterWriter.Write(messageLength);
            ParameterWriter.Write(messageContents);

            SendCommand(EZSPFrameIDs.SendReply, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sends a Many to One Route Request
        /// </summary>
        /// <param name="concentratorType">The type of concentrator</param>
        /// <param name="radius">The number of hops the route request will be relayed</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SendManyToOneRouteRequest(EmberConcentratorType concentratorType, byte radius, out EmberStatus status)
        {
            byte[] ParameterData = new byte[3];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterWriter.Write((ushort)concentratorType);
            ParameterWriter.Write(radius);

            SendCommand(EZSPFrameIDs.SendManyToOneRouteRequest, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Polls for data on the specified interval
        /// </summary>
        /// <param name="interval">The amount of time between polls.</param>
        /// <param name="units">The unit type of interval</param>
        /// <param name="failureLimit">The number of poll failures tolerated before the a pollCompleteHandler callback is generated</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void PollForData(ushort interval, EmberEventUnits units, byte failureLimit, out EmberStatus status)
        {
            byte[] ParameterData = new byte[4];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterWriter.Write(interval);
            ParameterWriter.Write((byte)units);
            ParameterWriter.Write(failureLimit);

            SendCommand(EZSPFrameIDs.PollForData, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets a source route for the next outgoing message
        /// </summary>
        /// <param name="destination">The destination of the source route</param>
        /// <param name="relayCount">The number of relays in the list</param>
        /// <param name="relayList">The source route</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void SetSourceRoute(ushort destination, byte relayCount, ushort[] relayList, out EmberStatus status)
        {
            byte[] ParameterData = new byte[3 + 2 * relayCount];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            // Validate the parameters
            if (relayList == null || relayCount != relayList.Length)
            {
                throw new ArgumentException("The length of the relay list must match the value specified as the relay count", "relayList");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(destination);
            ParameterWriter.Write(relayCount);

            foreach (ushort CurrentRelay in relayList)
            {
                ParameterWriter.Write(CurrentRelay);
            }

            SendCommand(EZSPFrameIDs.SetSourceRoute, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Indicates whether any messages are currently being sent using an address table entry
        /// </summary>
        /// <param name="index">The index of the address table entry</param>
        /// <param name="isActive">Whether or not the address entry is in use</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void AddressTableEntryIsActive(byte index, out bool isActive)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            isActive = false;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.AddressTableEntryIsActive, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                if (ResponseData[0] > 1)
                {
                    isActive = true;
                }
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the EUI of an address table entry and update the Node ID if it is known.
        /// </summary>
        /// <param name="index">The index of the address table to set</param>
        /// <param name="eui">The EUI to set</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetAddressTableRemoteEUI(byte index, ulong eui, out EmberStatus status)
        {
            byte[] ParameterData = new byte[9];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterWriter.Write(index);
            ParameterWriter.Write(eui);

            SendCommand(EZSPFrameIDs.SetAddressTableRemoteEUI, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the Node ID of an address table entry
        /// </summary>
        /// <param name="index">The index of the address table entry to set</param>
        /// <param name="nodeID">The Node ID to set</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetAddressTableRemoteNodeID(byte index, ushort nodeID)
        {
            byte[] ParameterData = new byte[3];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Set up the Parameter Data
            ParameterWriter.Write(index);
            ParameterWriter.Write(nodeID);

            SendCommand(EZSPFrameIDs.SetAddressTableRemoteNodeID, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData == null || ResponseData.Length != 0)
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the EUI of an address table entry
        /// </summary>
        /// <param name="index">The index of the address table entry</param>
        /// <param name="eui">The EUI associated with this entry</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetAddressTableRemoteEUI(byte index, out ulong eui)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            eui = 0;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.GetAddressTableRemoteEUI, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 8)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                eui = ResponseReader.ReadUInt64();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the Node ID of an Address Table Entry
        /// </summary>
        /// <param name="index">The index of the address table entry</param>
        /// <param name="nodeID">The Node ID of the entry</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetAddressTableRemoteNodeID(byte index, out ushort nodeID)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            nodeID = 0;

            // Set up the Parameter Data
            ParameterData[0] = 1;

            SendCommand(EZSPFrameIDs.GetAddressTableRemoteNodeID, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 2)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                nodeID = ResponseReader.ReadUInt16();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Tells the stack whether or not to increase the interval between retransmission of a retried unicast message
        /// </summary>
        /// <param name="remoteEUI">The EUI of the node to set</param>
        /// <param name="extendedTimeout">Whether or not to extend the timeout</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetExtendedTimeout(ulong remoteEUI, bool extendedTimeout)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Set up the Parameter Data
            ParameterWriter.Write(remoteEUI);
            ParameterWriter.Write(extendedTimeout);

            SendCommand(EZSPFrameIDs.SetExtendedTimeout, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData == null || ResponseData.Length != 0)
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets whether or not the device is set to use extended timeouts
        /// </summary>
        /// <param name="remoteEUI">The EUI of the device to check</param>
        /// <param name="extendedTimeout">Whether or not the device is using extended timeouts</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetExtendedTimeout(ulong remoteEUI, out bool extendedTimeout)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            extendedTimeout = false;

            // Set up the Parameter Data
            ParameterWriter.Write(remoteEUI);

            SendCommand(EZSPFrameIDs.GetExtendedTimeout, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                if (ResponseData[0] > 0)
                {
                    extendedTimeout = true;
                }
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Replaces and address table entry
        /// </summary>
        /// <param name="index">The index of the entry to replace</param>
        /// <param name="newEUI">The EUI of the new entry</param>
        /// <param name="newNodeID">The node ID of the new entry</param>
        /// <param name="newExtendedTimeout">Whether or not the device should use an extended timeout</param>
        /// <param name="status">The status of the command</param>
        /// <param name="oldEUI">The previous entry EUI</param>
        /// <param name="oldNodeID">The previous entry Node ID</param>
        /// <param name="oldExtendedTimeout">Whether or not the previous entry was using extended timeouts</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void ReplaceAddressTableEntry(byte index, ulong newEUI, ushort newNodeID, bool newExtendedTimeout,
            out EmberStatus status, out ulong oldEUI, out ushort oldNodeID, out bool oldExtendedTimeout)
        {
            byte[] ParameterData = new byte[12];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;
            oldEUI = 0;
            oldNodeID = 0;
            oldExtendedTimeout = false;

            // Set up the Parameter Data
            ParameterWriter.Write(index);
            ParameterWriter.Write(newEUI);
            ParameterWriter.Write(newNodeID);
            ParameterWriter.Write(newExtendedTimeout);

            SendCommand(EZSPFrameIDs.ReplaceAddressTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 12)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                oldEUI = ResponseReader.ReadUInt64();
                oldNodeID = ResponseReader.ReadUInt16();
                oldExtendedTimeout = ResponseReader.ReadBoolean();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Looks up the Node ID for the specified EUI
        /// </summary>
        /// <param name="eui">The EUI to look up</param>
        /// <param name="nodeID">The Node ID of the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void LookupNodeIDByEUI(ulong eui, out ushort nodeID)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            nodeID = 0;

            // Set up the Parameter Data
            ParameterWriter.Write(eui);

            SendCommand(EZSPFrameIDs.LookupNodeIDByEUI, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 2)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                nodeID = ResponseReader.ReadUInt16();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Looks up the EUI for the specified Node ID
        /// </summary>
        /// <param name="nodeID">The Node ID of the device to lookup</param>
        /// <param name="status">The status of the command</param>
        /// <param name="eui">The EUI of the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void LookupEUIByNodeID(ushort nodeID, out EmberStatus status, out ulong eui)
        {
            byte[] ParameterData = new byte[2];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;
            eui = 0;

            // Set up the Parameter Data
            ParameterWriter.Write(nodeID);

            SendCommand(EZSPFrameIDs.LookipEUIByNodeID, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 9)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                eui = ResponseReader.ReadUInt64();
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets an entry from the Multicast Table
        /// </summary>
        /// <param name="index">The index of the entry to get</param>
        /// <param name="status">The status of the command</param>
        /// <param name="entry">The multicast table entry</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetMulticastTableEntry(byte index, out EmberStatus status, out EmberMulticastTableEntry entry)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            entry = null;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.GetMulticastTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 37)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                entry = new EmberMulticastTableEntry();
                entry.RawData = ResponseReader.ReadBytes(3);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the multicast table entry
        /// </summary>
        /// <param name="index">The index of the multicast table to set</param>
        /// <param name="entry">The Multicast Table entry</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetMulticastTableEntry(byte index, EmberMulticastTableEntry entry, out EmberStatus status)
        {
            byte[] ParameterData = new byte[4];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Validate the parameters
            if (entry == null)
            {
                throw new ArgumentNullException("entry", "The Multicast Table Entry may not be null");
            }

            // Set up the Parameter Data
            ParameterData[0] = index;
            Array.Copy(entry.RawData, 0, ParameterData, 1, 3);

            SendCommand(EZSPFrameIDs.SetMulticastTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Transmits the given message without any modification
        /// </summary>
        /// <param name="messageLength">The length of the message to send</param>
        /// <param name="messageContents">The contents of the message</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SendRawMessage(byte messageLength, byte[] messageContents, out EmberStatus status)
        {
            byte[] ParameterData = new byte[1 + messageLength];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Validate the parameter data
            if (messageContents == null || messageLength != messageContents.Length)
            {
                throw new ArgumentException("The length of the message contents must match the specified message length", "messageContents");
            }

            // Set up the Parameter Data
            ParameterData[0] = messageLength;
            Array.Copy(messageContents, 0, ParameterData, 1, messageLength);

            SendCommand(EZSPFrameIDs.SendRawMessage, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        #endregion

        #region Security Frames

        /// <summary>
        /// Sets the security state that will be used by the device when it forms or joins a network
        /// </summary>
        /// <param name="state">The initial security state</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void SetInitialSecurityState(EmberInitialSecurityState state, out EmberStatus status)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state", "Initial Security State can not be null");
            }

            byte[] ParameterData = state.RawData;
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            SendCommand(EZSPFrameIDs.SetInitialSecurityState, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the current security state of the device
        /// </summary>
        /// <param name="status">The status of the command</param>
        /// <param name="state">The current security state</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetCurrentSecurityState(out EmberStatus status, out EmberCurrentSecurityState state)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            state = null;

            SendCommand(EZSPFrameIDs.GetCurrentSecurityState, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 11)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                state = new EmberCurrentSecurityState();
                state.RawData = ResponseReader.ReadBytes(10);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets a security key of the specified type 
        /// </summary>
        /// <param name="keyType">The type of the key to get.</param>
        /// <param name="status">The status of the command</param>
        /// <param name="key">The structure containing the key</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetKey(EmberKeyType keyType, out EmberStatus status, out EmberKeyStruct key)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            key = null;

            // Set up the Parameter Data
            ParameterData[0] = (byte)keyType;

            SendCommand(EZSPFrameIDs.GetKey, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 37)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                key = new EmberKeyStruct();
                key.RawData = ResponseReader.ReadBytes(36);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Gets the key table entry at the specified index.
        /// </summary>
        /// <param name="index">The index of the key to get</param>
        /// <param name="status">The status of the command</param>
        /// <param name="key">The key that was requested</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetKeyTableEntry(byte index, out EmberStatus status, out EmberKeyStruct key)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            key = null;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.GetKeyTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 37)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                key = new EmberKeyStruct();
                key.RawData = ResponseReader.ReadBytes(36);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the specified key value
        /// </summary>
        /// <param name="index">The index of the key to set</param>
        /// <param name="eui">The EUI of the partner device</param>
        /// <param name="isLinkKey">Whether or not the specified key is a link key</param>
        /// <param name="key">The key</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetKeyTableEntry(byte index, ulong eui, bool isLinkKey, byte[] key, out EmberStatus status)
        {
            byte[] ParameterData = new byte[26];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            if (key == null || key.Length != 16)
            {
                throw new ArgumentException("The key value cannot be null and must be 16 bytes long", "key");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(index);
            ParameterWriter.Write(eui);
            ParameterWriter.Write(isLinkKey);
            ParameterWriter.Write(key);

            SendCommand(EZSPFrameIDs.SetKeyTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Finds the Key Table index for the entry that matches the search criteria
        /// </summary>
        /// <param name="eui">The EUI of the device to search for</param>
        /// <param name="findLinkKey">Whether to search for a link key entry or master key</param>
        /// <param name="index">The index that contains the key</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void FindKeyTableEntry(ulong eui, bool findLinkKey, out byte index)
        {            
            byte[] ParameterData = new byte[9];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            index = 0;

            // Set up the Parameter Data
            ParameterWriter.Write(eui);
            ParameterWriter.Write(findLinkKey);

            SendCommand(EZSPFrameIDs.FindKeyTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                index = ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Adds a new key entry or updates an existing key entry
        /// </summary>
        /// <param name="eui">The EUI of partner device to add/replace</param>
        /// <param name="isLinkKey">Whether or not the key is a link key or a master key</param>
        /// <param name="key">The new key</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void AddOrUpdateKeyTableEntry(ulong eui, bool isLinkKey, byte[] key, out EmberStatus status)
        {
            byte[] ParameterData = new byte[25];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            if (key == null || key.Length != 16)
            {
                throw new ArgumentException("The key value cannot be null and must be 16 bytes long", "key");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(eui);
            ParameterWriter.Write(isLinkKey);
            ParameterWriter.Write(key);

            SendCommand(EZSPFrameIDs.AddOrUpdateKeyTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Erases the Key Table entry at the specified location
        /// </summary>
        /// <param name="index">The index of the key table entry to erase</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void EraseKeyTableEntry(byte index, out EmberStatus status)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterData[0] = index;

            SendCommand(EZSPFrameIDs.EraseKeyTableEntry, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Requests a link key from the Trust Center with another device on the network
        /// </summary>
        /// <param name="eui">The EUI of the partner device</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void RequestLinkKey(ulong eui, out EmberStatus status)
        {
            byte[] ParameterData = new byte[8];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterWriter.Write(eui);

            SendCommand(EZSPFrameIDs.RequestLinkKey, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        #endregion

        #region Trust Center Frames

        /// <summary>
        /// Broadcasts a new encryption key but does not tell the nodes in the network to start using it.
        /// </summary>
        /// <param name="key">The new network key to use. If the key is set to all zeros then a new key will be randomly generated</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void BroadcastNextNetworkKey(byte[] key, out EmberStatus status)
        {
            byte[] ParameterData = key;
            byte[] ResponseData = null;

            // Validate the Parameter Data
            if (key == null || key.Length != 16)
            {
                throw new ArgumentException("The key may not be null and must be 16 bytes long", "key");
            }

            SendCommand(EZSPFrameIDs.BroadcastNextNetworkKey, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Broadcasts a switch key message to tell all nodes to change to the previously sent network key
        /// </summary>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void BroadcastNetworkKeySwitch(out EmberStatus status)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            SendCommand(EZSPFrameIDs.BroadcastNetworkKeySwitch, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Causes a coordinator to become the Trust Center when operating on a network that is not using one and sends out an
        /// updated network key to all devices to indicate a transition of the network to use a Trust Center
        /// </summary>
        /// <param name="newNetworkKey">The new network key to use</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void BecomeTrustCenter(byte[] newNetworkKey, out EmberStatus status)
        {
            byte[] ParameterData = newNetworkKey;
            byte[] ResponseData = null;

            // Validate the Parameter Data
            if (newNetworkKey == null || newNetworkKey.Length != 16)
            {
                throw new ArgumentException("The new network key may not be null and must be 16 bytes long", "newNetworkKey");
            }

            SendCommand(EZSPFrameIDs.BecomeTrustCenter, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Processes the data and updates the hash context based on it.
        /// </summary>
        /// <param name="context">The hash context to update</param>
        /// <param name="finalize">Whether or not the final hash value should be calculated</param>
        /// <param name="length">The length of the data to hash</param>
        /// <param name="data">The data to hash</param>
        /// <param name="status">The result of the operation</param>
        /// <param name="returnContext">The updated hash context</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void AesMmoHash(byte[] context, bool finalize, byte length, byte[] data, out EmberStatus status, out byte[] returnContext)
        {
            byte[] ParameterData = new byte[18 + length];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Validate the Parameter Data
            if (context == null || context.Length != 16)
            {
                throw new ArgumentException("The context may not be null and must be 16 bytes long", "context");
            }

            if (data == null || length != data.Length)
            {
                throw new ArgumentException("The data parameter may not be null and must be the specified length", "data");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(context);
            ParameterWriter.Write(finalize);
            ParameterWriter.Write(length);
            ParameterWriter.Write(data);


            SendCommand(EZSPFrameIDs.AesMmoHash, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 17)
            {
                status = (EmberStatus)ResponseData[0];

                returnContext = new byte[16];
                Array.Copy(ResponseData, 1, returnContext, 0, 16);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sends an APS remove device to tell a device to remove itself from the network
        /// </summary>
        /// <param name="destinationShortAddress">The node ID of the device that will receive the message</param>
        /// <param name="destinationLongAddress">The long address of the device that will received the message</param>
        /// <param name="targetLongAddress">The long address of the device to be removed</param>
        /// <param name="status">The status of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void RemoveDevice(ushort destinationShortAddress, ulong destinationLongAddress, ulong targetLongAddress, out EmberStatus status)
        {
            byte[] ParameterData = new byte[26];
            byte[] ResponseData = null;
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            // Validate the Parameter Data
            if (targetLongAddress == 0)
            {
                throw new ArgumentException("Address may not zero", "targetLongAddress");
            }

            if (destinationLongAddress == 0)
            {
                throw new ArgumentException("Address may not zero", "destinationLongAddress");
            }

            // Set up the Parameter Data
            ParameterWriter.Write(destinationShortAddress);
            ParameterWriter.Write(destinationLongAddress);
            ParameterWriter.Write(targetLongAddress);

            SendCommand(EZSPFrameIDs.RemoveDevice, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        #endregion

        #region CBKE Frames

        /// <summary>
        /// Generates the ECC Key Pair and stores the private key when complete
        /// </summary>
        /// <param name="status">The status of the operation</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GenerateCBKEKeys(out EmberStatus status)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            SendCommand(EZSPFrameIDs.GenerateCBKEKeys, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Calculates the SMAC verification keys for the initiator and responder roles of CBKE
        /// </summary>
        /// <param name="isInitiator">The role of this device in the key establishment process</param>
        /// <param name="partnerCertificate">48  byte certificate</param>
        /// <param name="partnerPublicKey">22 byte public key</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void CalculateSmacs(bool isInitiator, byte[] partnerCertificate, byte[] partnerPublicKey, out EmberStatus status)
        {
            byte[] ParameterData = new byte[71];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            MemoryStream ParameterStream = new MemoryStream(ParameterData);
            BinaryWriter ParameterWriter = new BinaryWriter(ParameterStream);

            ParameterWriter.Write(isInitiator);

            if (partnerCertificate != null && partnerCertificate.Length == 48)
            {
                ParameterWriter.Write(partnerCertificate);
            }
            else
            {
                throw new ArgumentException("Partner Certificate must have a length of 48 bytes", "partnerCertificate");
            }

            if (partnerPublicKey != null && partnerPublicKey.Length == 22)
            {
                ParameterWriter.Write(partnerPublicKey);
            }
            else
            {
                throw new ArgumentException("Partner Public Key must have a length of 22 bytes", "partnerPublicKey");
            }

            SendCommand(EZSPFrameIDs.CalculateSmacs, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Clears the temporary data used for CBKE and key establishment and possibly stores the link key
        /// </summary>
        /// <param name="storeLinkKey">Whether or not to store the link key</param>
        /// <param name="status">The result of the operation</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void ClearTemporaryDataMaybeStoreLinkKey(bool storeLinkKey, out EmberStatus status)
        {
            byte[] ParameterData = new byte[1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            if (storeLinkKey)
            {
                ParameterData[0] = 1;
            }
            else
            {
                ParameterData[0] = 0;
            }

            SendCommand(EZSPFrameIDs.ClearTemporaryDataMaybeStoreLinkKey, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Retrieves the installed certificate
        /// </summary>
        /// <param name="status">The status of the command</param>
        /// <param name="localCert">48 byte certificate</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void GetCertificate(out EmberStatus status, out byte[] localCert)
        {
            byte[] ParameterData = new byte[0];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;
            localCert = null;

            SendCommand(EZSPFrameIDs.GetCertificate, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 49)
            {
                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                status = (EmberStatus)ResponseReader.ReadByte();
                localCert = ResponseReader.ReadBytes(48);
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// DSA signs a message
        /// </summary>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The message contents</param>
        /// <param name="status">The result of the operation</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void DsaSign(byte messageLength, byte[] messageContents, out EmberStatus status)
        {
            byte[] ParameterData = new byte[messageLength + 1];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            ParameterData[0] = messageLength;

            if (messageContents != null && messageContents.Length == messageLength)
            {
                Array.Copy(messageContents, 0, ParameterData, 1, messageLength);
            }
            else
            {
                throw new ArgumentException("Message Contents length must match specified message length", "messageContents");
            }

            SendCommand(EZSPFrameIDs.DsaSign, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Verify the signature of the associated message digest was signed by the private key of the certificate
        /// </summary>
        /// <param name="digest">16 byte digest of a message</param>
        /// <param name="signerCertificate">48 byte certificate of the signer</param>
        /// <param name="receivedSignature">42 byte signature of the signed data</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void DsaVerify(byte[] digest, byte[] signerCertificate, byte[] receivedSignature, out EmberStatus status)
        {
            byte[] ParameterData = new byte[106];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            if (digest != null && digest.Length == 16)
            {
                Array.Copy(digest, 0, ParameterData, 0, 16);
            }
            else
            {
                throw new ArgumentException("Digest length must be 16 bytes", "digest");
            }

            if (signerCertificate != null && signerCertificate.Length == 48)
            {
                Array.Copy(signerCertificate, 0, ParameterData, 16, 48);
            }
            else
            {
                throw new ArgumentException("Signer Certificate length must be 48 bytes", "signerCertificate");
            }

            if (receivedSignature != null && receivedSignature.Length == 42)
            {
                Array.Copy(receivedSignature, 0, ParameterData, 64, 42);
            }
            else
            {
                throw new ArgumentException("Received Signature length must be 42 bytes", "receivedSignature");
            }

            SendCommand(EZSPFrameIDs.DsaVerify, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        /// <summary>
        /// Sets the device's CA public key, local certificate and static private key
        /// </summary>
        /// <param name="certificateAuthorityPublicKey">22 byte public key for the Certificate Authority</param>
        /// <param name="certificate">48 byte certificate</param>
        /// <param name="privateKey">21 byte Private key for the node</param>
        /// <param name="status">The result of the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void SetPreinstalledCBKEData(byte[] certificateAuthorityPublicKey, byte[] certificate, byte[] privateKey, out EmberStatus status)
        {
            byte[] ParameterData = new byte[91];
            byte[] ResponseData = null;

            status = EmberStatus.FatalError;

            // Set up the Parameter Data
            if (certificateAuthorityPublicKey != null && certificateAuthorityPublicKey.Length == 22)
            {
                Array.Copy(certificateAuthorityPublicKey, 0, ParameterData, 0, 22);
            }
            else
            {
                throw new ArgumentException("Certificate Authority Public Key must be 22 bytes long", "certificateAuthorityPublicKey");
            }

            if (certificate != null && certificate.Length == 48)
            {
                Array.Copy(certificate, 0, ParameterData, 22, 48);
            }
            else
            {
                throw new ArgumentException("Certificate length must be 48 bytes long", "certificate");
            }

            if (privateKey != null && privateKey.Length == 21)
            {
                Array.Copy(privateKey, 0, ParameterData, 70, 21);
            }
            else
            {
                throw new ArgumentException("Private Key length must be 21 bytes long", "privateKey");
            }

            SendCommand(EZSPFrameIDs.SetPreinstalledCBKEData, ParameterData, out ResponseData);

            // Parse out the Response Data
            if (ResponseData != null && ResponseData.Length == 1)
            {
                status = (EmberStatus)ResponseData[0];
            }
            else
            {
                throw new InvalidDataException("The response parameter data size is invalid");
            }
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the radio is currently connected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool IsConnected
        {
            get
            {
                return m_Connected;
            }
        }

        /// <summary>
        /// Gets the results for the active scan
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public List<ZigbeeNetwork> ActiveScanResults
        {
            get
            {
                return m_ActiveScanResults;
            }
        }

        /// <summary>
        /// Gets the results for the energy scan
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public List<ZigBeeEnergyScanResult> EnergyScanResults
        {
            get
            {
                return m_EnergyScanResults;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sends the specified command and waits for the response
        /// </summary>
        /// <param name="frameID">The Frame ID of the command to send</param>
        /// <param name="parameterData">The parameter data for the command</param>
        /// <param name="responseData">The response received by the command</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void SendCommand(EZSPFrameIDs frameID, byte[] parameterData, out byte[] responseData)
        {
            uint Timeout = DEFAULT_RESPONSE_TIMEOUT;
            responseData = null;

            if (m_Connected)
            {
                if (m_Joined == false)
                {
                    if (m_ASH != null && m_ASH.Connected)
                    {
                        // Create the Command frame
                        EZSPCommandFrame CommandFrame = new EZSPCommandFrame();
                        CommandFrame.FrameID = frameID;
                        CommandFrame.ParameterData = parameterData;

                        if (frameID == EZSPFrameIDs.GenerateCBKEKeys)
                        {
                            Timeout = CBKE_RESPONSE_TIMEOUT;
                        }

                        SendFrame(CommandFrame);

                        // Wait for the Response
                        EZSPResponseFrame ResponseFrame = WaitForResponse(frameID, Timeout);

                        if (ResponseFrame != null)
                        {
                            responseData = ResponseFrame.ParameterData;
                        }
                        else
                        {
                            // throw new TimeoutException("Did not receive a response within the allowed time");
                            Console.WriteLine("Did not receive a response within the allowed time");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Could not perform the requested operation. ASH protocol object is null or not connected");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Can not perform EZSP operations while disconnected");
            }
        }

        /// <summary>
        /// Waits for a response for the specified frame
        /// </summary>
        /// <param name="frameID">The frame ID for the response to look for</param>
        /// <param name="timeout">The number of milliseconds to wait before timing out</param>
        /// <returns>Null if no response was received or the response frame</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private EZSPResponseFrame WaitForResponse(EZSPFrameIDs frameID, uint timeout)
        {
            EZSPResponseFrame Response = null;
            DateTime StartTime = DateTime.Now;

            while ((DateTime.Now - StartTime).TotalMilliseconds < timeout && Response == null)
            {
                if (m_CommandResponses.Where(f => f.FrameID == frameID).Count() > 0)
                {
                    Response = m_CommandResponses.First(f => f.FrameID == frameID);
                    m_CommandResponses.Remove(Response);
                }
                else
                {
                    // We haven't found the response so keep looking
                    Thread.Sleep(25);                    
                }
            }

            return Response;
        }

        /// <summary>
        /// Handles the frame received event from the ASH Layer
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void m_ASH_FrameReceived(object sender, EventArgs e)
        {
            List<ASHFrame> FramesToRemove = new List<ASHFrame>();

            foreach (ASHFrame CurrentFrame in m_ASH.ReceivedFrames)
            {
                EZSPResponseFrame CurrentResponse = new EZSPResponseFrame();
                CurrentResponse.RawFrame = CurrentFrame.Data;

                switch(CurrentResponse.CallbackType)
                {
                    case EZSPCallbackType.NotACallback:
                    {
                        // This is a normal command response and should get handled as part of the command
                        // that was issued
                        m_CommandResponses.Add(CurrentResponse);
                        break;
                    }
                    case EZSPCallbackType.SynchronousCallback:
                    case EZSPCallbackType.AsynchronousCallback:
                    {
                        // This is a callback that was sent as a data push so we need to handle the pushed data
                        m_AsyncCallbacks.Add(CurrentResponse);
                        break;
                    }
                }

                if (m_Logger != null)
                {
                    m_Logger.WriteEZSPFrame(CurrentResponse);
                }

                // We have handled the response so mark it to be removed from the list of received items
                FramesToRemove.Add(CurrentFrame);
            }

            // Remove the frames that have been handled
            m_ASH.ReceivedFrames.RemoveAll(f => FramesToRemove.Contains(f));

            // Handle any Asynchronous callbacks we may have read

            HandleCallbacks();
        }

        /// <summary>
        /// Sends the specified frame
        /// </summary>
        /// <param name="frame">The frame to send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void SendFrame(EZSPCommandFrame frame)
        {
            if (m_ASH != null && m_ASH.Connected)
            {
                frame.SequenceNumber = m_CurrentSequenceNumber;

                if (m_Logger != null)
                {
                    m_Logger.WriteEZSPFrame(frame);
                }

                m_ASH.SendData(frame.RawFrame);
                m_CurrentSequenceNumber++;
            }
            else
            {
                throw new InvalidOperationException("A frame cannot be sent unless the ASH layer is currently connected");
            }
        }

        /// <summary>
        /// Handles any asynchronous callbacks that have been received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void HandleCallbacks()
        {
            List<EZSPResponseFrame> FramesToRemove = new List<EZSPResponseFrame>();

            foreach (EZSPResponseFrame CurrentCallback in m_AsyncCallbacks)
            {
                MemoryStream ParameterStream = new MemoryStream(CurrentCallback.ParameterData);
                BinaryReader ParameterReader = new BinaryReader(ParameterStream);
                bool FrameHandled = true;

                switch(CurrentCallback.FrameID)
                {
                    case EZSPFrameIDs.TimerHandler:
                    {
                        OnTimerOccurred(ParameterReader.ReadByte());
                        break;
                    }
                    case EZSPFrameIDs.StackStatusHandler:
                    {
                        m_StackStatus = (EmberStatus)ParameterReader.ReadByte();
                        OnStackStatusUpdated(m_StackStatus);
                        break;
                    }
                    case EZSPFrameIDs.EnergyScanResultHandler:
                    {
                        ZigBeeEnergyScanResult EnergyResult = new ZigBeeEnergyScanResult();

                        EnergyResult.Channel = ParameterReader.ReadByte();
                        EnergyResult.MaxRSSI = ParameterReader.ReadSByte();

                        // If we have already seen the channel remove the old version in favor of the latest data
                        if (m_EnergyScanResults.Where(d => d.Channel == EnergyResult.Channel).Count() > 0)
                        {
                            m_EnergyScanResults.RemoveAll(d => d.Channel == EnergyResult.Channel);
                        }

                        m_EnergyScanResults.Add(EnergyResult);
                        break;
                    }
                    case EZSPFrameIDs.NetworkFoundHandler:
                    {
                        ZigbeeNetwork ZigBeeDevice = new ZigbeeNetwork();

                        ZigBeeDevice.LogicalChannel = ParameterReader.ReadByte();
                        ZigBeeDevice.PanId = ParameterReader.ReadUInt16();
                        ZigBeeDevice.ExPanID = ParameterReader.ReadUInt64();
                        ZigBeeDevice.ExpectingJoin = ParameterReader.ReadByte();
                        ZigBeeDevice.StackProfile_ZigbeeVersion = ParameterReader.ReadByte();
                        ZigBeeDevice.NetworkUpdateID = ParameterReader.ReadByte();
                        ZigBeeDevice.LastHopLqi = ParameterReader.ReadByte();
                        ZigBeeDevice.LastHopRssi = ParameterReader.ReadSByte();

                        // If we have already seen the device remove the old version in favor of the latest data
                        if (m_ActiveScanResults.Where(d => d.PanId == ZigBeeDevice.PanId && d.ExPanID == ZigBeeDevice.ExPanID).Count() > 0)
                        {
                            m_ActiveScanResults.RemoveAll(d => d.PanId == ZigBeeDevice.PanId && d.ExPanID == ZigBeeDevice.ExPanID);
                        }

                        m_ActiveScanResults.Add(ZigBeeDevice);
                        break;
                    }
                    case EZSPFrameIDs.ScanCompleteHandler:
                    {
                        OnScanCompleted();
                        break;
                    }
                    case EZSPFrameIDs.ChildJoinHandler:
                    {
                        byte Index = ParameterReader.ReadByte();
                        bool Joining = ParameterReader.ReadBoolean();
                        ushort ChildID = ParameterReader.ReadUInt16();
                        ulong ChildEUI = ParameterReader.ReadUInt64();
                        EmberNodeType ChildType = (EmberNodeType)ParameterReader.ReadByte();

                        OnChildJoined(Index, Joining, ChildID, ChildEUI, ChildType);
                        break;
                    }
                    case EZSPFrameIDs.RemoteSetBindingHandler:
                    {
                        EmberBindingTableEntry NewEntry = new EmberBindingTableEntry();
                        NewEntry.RawData = ParameterReader.ReadBytes(13);
                        byte Index = ParameterReader.ReadByte();
                        EmberStatus Status = (EmberStatus)ParameterReader.ReadByte();

                        OnBindingSetRemotely(NewEntry, Index, Status);
                        break;
                    }
                    case EZSPFrameIDs.RemoteDeleteBindingHandler:
                    {
                        byte Index = ParameterReader.ReadByte();
                        EmberStatus Status = (EmberStatus)ParameterReader.ReadByte();

                        OnBindingDeletedRemotely(Index, Status);
                        break;
                    }
                    case EZSPFrameIDs.MessageSentHandler:
                    {
                        EmberOutgoingMessageType MessageType = (EmberOutgoingMessageType)ParameterReader.ReadByte();
                        ushort IndexOrDestination = ParameterReader.ReadUInt16();
                        EmberApsFrame ApsFrame = new EmberApsFrame();
                        ApsFrame.RawData = ParameterReader.ReadBytes(11);
                        byte MessageTag = ParameterReader.ReadByte();
                        EmberStatus Status = (EmberStatus)ParameterReader.ReadByte();
                        byte MessageLength = ParameterReader.ReadByte();
                        byte[] MessageContents = ParameterReader.ReadBytes(MessageLength);

                        // TODO: Check to see if message length is 0 when the policy is set to not include message contents

                        OnMessageSent(MessageType, IndexOrDestination, ApsFrame, MessageTag, Status, MessageLength, MessageContents);
                        break;
                    }
                    case EZSPFrameIDs.PollCompleteHandler:
                    {
                        OnPollComplete((EmberStatus)ParameterReader.ReadByte());
                        break;
                    }
                    case EZSPFrameIDs.PollHandler:
                    {
                        OnPollReceived(ParameterReader.ReadUInt16());
                        break;
                    }
                    case EZSPFrameIDs.IncomingSenderEUIHandler:
                    {
                        OnSenderEUIReceived(ParameterReader.ReadUInt64());
                        break;
                    }
                    case EZSPFrameIDs.IncomingMessageHandler:
                    {
                        EmberIncomingMessageType MessageType = (EmberIncomingMessageType)ParameterReader.ReadByte();
                        EmberApsFrame APSFrame = new EmberApsFrame();
                        APSFrame.RawData = ParameterReader.ReadBytes(11);
                        byte LastHopLqi = ParameterReader.ReadByte();
                        byte LastHopRssi = ParameterReader.ReadByte();
                        ushort SenderNodeID = ParameterReader.ReadUInt16();
                        byte BindingIndex = ParameterReader.ReadByte();
                        byte AddressIndex = ParameterReader.ReadByte();
                        byte MessageLength = ParameterReader.ReadByte();
                        byte[] MessageContents = ParameterReader.ReadBytes(MessageLength);
                        IncomingMessage NewMessage = new IncomingMessage(MessageType, APSFrame, LastHopLqi, LastHopRssi, SenderNodeID, BindingIndex, AddressIndex, MessageLength, MessageContents);

                        string LogMessage = "Message Received: " + NewMessage.MessageType.ToString() + " from " + NewMessage.SenderNodeID.ToString("X4")
                            + " - Profile ID: " + NewMessage.APSFrame.ProfileID.ToString("X4") + " Cluster ID: " + NewMessage.APSFrame.ClusterID.ToString("X4") + " Message Contents: ";

                        foreach (byte MessageByte in NewMessage.MessageContents)
                        {
                            LogMessage += MessageByte.ToString("X2", CultureInfo.InvariantCulture) + " ";
                        }

                        m_Logger.WriteLine(EZSPLogLevels.EZSPProtocol, LogMessage);


                        // Check for fragmentation
                        if ((APSFrame.Options & EmberApsOptions.Fragment) == EmberApsOptions.Fragment)
                        {
                            Action<object> FragmentationAction = (object actionObject) =>
                            {
                                IncomingMessage ActionMessage = actionObject as IncomingMessage;

                                if (ActionMessage != null)
                                {
                                    HandleIncomingMessageFragment(ActionMessage);
                                }
                            };

                            Task.Factory.StartNew(FragmentationAction, (object)NewMessage);
                        }
                        else
                        {
                            OnMessageReceived(NewMessage);
                        }

                        break;
                    }
                    case EZSPFrameIDs.IncomingRouteRecordHandler:
                    {
                        ushort SourceNodeID = ParameterReader.ReadByte();
                        ulong SourceEUI = ParameterReader.ReadUInt64();
                        byte LastHopLqi = ParameterReader.ReadByte();
                        byte LastHopRssi = ParameterReader.ReadByte();
                        byte RelayCount = ParameterReader.ReadByte();
                        ushort[] RelayList = new ushort[RelayCount];

                        for (int Index = 0; Index < RelayCount; Index++)
                        {
                            RelayList[Index] = ParameterReader.ReadUInt16();
                        }

                        OnRouteRecordReceived(SourceNodeID, SourceEUI, LastHopLqi, LastHopRssi, RelayCount, RelayList);
                        break;
                    }
                    case EZSPFrameIDs.IncomingManyToOneRouteRequestHandler:
                    {
                        ushort SourceNodeID = ParameterReader.ReadUInt16();
                        ulong SourceEUI = ParameterReader.ReadUInt64();
                        byte Cost = ParameterReader.ReadByte();

                        OnManyToOneRouteAvailable(SourceNodeID, SourceEUI, Cost);
                        break;
                    }
                    case EZSPFrameIDs.IncomingRouteErrorHandler:
                    {
                        EmberStatus Status = (EmberStatus)ParameterReader.ReadByte();
                        ushort Target = ParameterReader.ReadUInt16();

                        OnRoutErrorOccurred(Status, Target);
                        break;
                    }
                    case EZSPFrameIDs.IDConflictHandler:
                    {
                        OnIDConflictDetected(ParameterReader.ReadUInt16());
                        break;
                    }
                    case EZSPFrameIDs.MacPassthroughMessageHandler:
                    {
                        EmberMacPassthroughType MessageType = (EmberMacPassthroughType)ParameterReader.ReadByte();
                        byte LastHopLqi = ParameterReader.ReadByte();
                        byte LastHopRssi = ParameterReader.ReadByte();
                        byte MessageLength = ParameterReader.ReadByte();
                        byte[] MessageContents = ParameterReader.ReadBytes(MessageLength);

                        OnMacPassthroughMessageReceived(MessageType, LastHopLqi, LastHopRssi, MessageLength, MessageContents);
                        break;
                    }
                    case EZSPFrameIDs.MacFilterMatchMessageHandler:
                    {
                        byte FilterIndexMatch = ParameterReader.ReadByte();
                        EmberMacPassthroughType MessageType = (EmberMacPassthroughType)ParameterReader.ReadByte();
                        byte LastHopLqi = ParameterReader.ReadByte();
                        byte LastHopRssi = ParameterReader.ReadByte();
                        byte MessageLength = ParameterReader.ReadByte();
                        byte[] MessageContents = ParameterReader.ReadBytes(MessageLength);

                        OnMacFilterMatchMessageReceived(FilterIndexMatch, MessageType, LastHopLqi, LastHopRssi, MessageLength, MessageContents);
                        break;
                    }
                    case EZSPFrameIDs.RawTransmitCompleteHandler:
                    {
                        OnRawMessageSent((EmberStatus)ParameterReader.ReadByte());
                        break;
                    }
                    case EZSPFrameIDs.SwitchNetworkKeyHandler:
                    {
                        OnNetworkKeySwitched(ParameterReader.ReadByte());
                        break;
                    }
                    case EZSPFrameIDs.ZigBeeKeyEstablishmentHandler:
                    {
                        ulong EUI = ParameterReader.ReadUInt64();
                        EmberKeyStatus Status = (EmberKeyStatus)ParameterReader.ReadByte();

                        OnZigBeeKeyEstablished(EUI, Status);
                        break;
                    }
                    case EZSPFrameIDs.TrustCenterJoinHandler:
                    {
                        ushort NewNodeID = ParameterReader.ReadUInt16();
                        ulong NewEUI = ParameterReader.ReadUInt64();
                        EmberDeviceUpdate Status = (EmberDeviceUpdate)ParameterReader.ReadByte();
                        EmberJoinDecision PolicyDecision = (EmberJoinDecision)ParameterReader.ReadByte();
                        ushort ParentNodeID = ParameterReader.ReadUInt16();

                        OnTrustCenterJoined(NewNodeID, NewEUI, Status, PolicyDecision, ParentNodeID);
                        break;
                    }
                    case EZSPFrameIDs.GenerateCBKEKeysHandler:
                    {
                        EmberStatus Status = (EmberStatus)ParameterReader.ReadByte();
                        byte[] PublicKey = ParameterReader.ReadBytes(22);

                        OnCBKEKeyGenerated(Status, PublicKey);
                        break;
                    }
                    case EZSPFrameIDs.CalculateSmacsHandler:
                    {
                        EmberStatus Status = (EmberStatus)ParameterReader.ReadByte();
                        byte[] InitiatorSmac = ParameterReader.ReadBytes(16);
                        byte[] ResponderSmac = ParameterReader.ReadBytes(16);

                        OnSmacsCalculated(Status, InitiatorSmac, ResponderSmac);
                        break;
                    }
                    case EZSPFrameIDs.DsaSignHandler:
                    {
                        EmberStatus Status = (EmberStatus)ParameterReader.ReadByte();
                        byte MessageLength = ParameterReader.ReadByte();
                        byte[] MessageContents = ParameterReader.ReadBytes(MessageLength);

                        OnDsaSigned(Status, MessageLength, MessageContents);
                        break;
                    }
                    case EZSPFrameIDs.DsaVerifyHandler:
                    {
                        EmberStatus Status = (EmberStatus)ParameterReader.ReadByte();

                        OnDsaVerified(Status);
                        break;
                    }
                    default:
                    {
                        // We don't know what to do with it so leave it alone
                        FrameHandled = false;
                        break;
                    }
                }

                if (FrameHandled)
                {
                    FramesToRemove.Add(CurrentCallback);
                }
            }

            // Remove all handled callbacks
            m_AsyncCallbacks.RemoveAll(f => FramesToRemove.Contains(f));
        }

        /// <summary>
        /// Handles an incoming message fragment
        /// </summary>
        /// <param name="newMessage">The message fragment</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 RCG 2.60.07        Created
        
        private void HandleIncomingMessageFragment(IncomingMessage newMessage)
        {
            byte TotalFragments = EmberApsFrame.GetNumberOfFragmentsFromGroupID(newMessage.APSFrame.GroupID);
            byte CurrentFragmentNumber = EmberApsFrame.GetFragmentNumberFromGroupID(newMessage.APSFrame.GroupID);

            if (m_HandlingFragmentedMessage)
            {
                if(m_FragmentSequenceNumber == newMessage.APSFrame.Sequence)
                {
                    if (m_FragmentsReceived.Where(f => EmberApsFrame.GetFragmentNumberFromGroupID(f.APSFrame.GroupID) == CurrentFragmentNumber).Count() == 0)
                    {
                        m_FragmentsReceived.Add(newMessage);

                        m_Logger.WriteLine(EZSPLogLevels.EZSPProtocol, "Message Fragment Received. Fragment " + CurrentFragmentNumber.ToString(CultureInfo.InvariantCulture)
                            + " of " + m_FragmentsExpected.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        // We received a duplicate fragment so lets just send the reply and move on
                        m_Logger.WriteLine(EZSPLogLevels.EZSPProtocol, "Duplicate fragment received. Fragment: " + CurrentFragmentNumber.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
            else
            {
                // Starting a new fragmented message
                if (CurrentFragmentNumber == 0 && TotalFragments > 0)
                {
                    ClearFragmentInformation();

                    m_HandlingFragmentedMessage = true;
                    m_FragmentSequenceNumber = newMessage.APSFrame.Sequence;
                    m_FragmentsReceived.Add(newMessage);
                    m_FragmentsExpected = TotalFragments;

                    if (m_HaveFragmentConfigurationData == false)
                    {
                        // Make sure we have the configuration data. This should really be done immediately after joining to avoid timing issues 
                        // but lets do it here just in case
                        GetFragmentationConfiguration();
                    }

                    // Start the receive timer
                    m_FragmentWindowTimer.Change(m_FragmentTimeout, Timeout.Infinite);

                    m_Logger.WriteLine(EZSPLogLevels.EZSPProtocol, "Message Fragment Received. Fragment " + CurrentFragmentNumber.ToString(CultureInfo.InvariantCulture)
                        + " of " + m_FragmentsExpected.ToString(CultureInfo.InvariantCulture));
                }
            }

            // Check to see if we need to send a reply to the meter
            if (m_HandlingFragmentedMessage && GetCurrentWindowBitfield() == 0xFF)
            {
                // We have the whole window so lets send the reply and move on to the next window
                SendFragmentReply();
                m_FragmentWindowStart += (byte)m_FragmentWindowSize;
                m_FragmentRetryCounter = 0;
                m_FragmentWindowTimer.Change(m_FragmentTimeout, Timeout.Infinite);
            }

            if (m_HandlingFragmentedMessage && m_FragmentsReceived.Count == m_FragmentsExpected)
            {
                m_Logger.WriteLine(EZSPLogLevels.EZSPProtocol, "All message fragments received.");
                m_HandlingFragmentedMessage = false;
                m_FragmentsExpected = 0;

                // We need to compile all of the fragments into one large message
                if (m_FragmentsReceived.Count > 0)
                {
                    IncomingMessage FirstMessage = m_FragmentsReceived[0];
                    byte TotalLength = 0;
                    byte[] TotalContents;
                    MemoryStream DataStream;
                    BinaryWriter DataWriter;

                    foreach (IncomingMessage CurrentFragment in m_FragmentsReceived)
                    {
                        TotalLength += CurrentFragment.MessageLength;
                    }

                    TotalContents = new byte[TotalLength];
                    DataStream = new MemoryStream(TotalContents);
                    DataWriter = new BinaryWriter(DataStream);

                    foreach (IncomingMessage CurrentFragment in m_FragmentsReceived)
                    {
                        DataWriter.Write(CurrentFragment.MessageContents);
                    }

                    IncomingMessage NewMessage = new IncomingMessage(FirstMessage.MessageType, FirstMessage.APSFrame, FirstMessage.LastHopLqi, FirstMessage.LastHopRssi,
                        FirstMessage.SenderNodeID, FirstMessage.BindingIndex, FirstMessage.AddressIndex, TotalLength, TotalContents);

                    // We're done so clear the info.
                    ClearFragmentInformation();

                    OnMessageReceived(NewMessage);
                }
            }
        }

        /// <summary>
        /// Time Callback for the Fragment timeout
        /// </summary>
        /// <param name="state">The state of the timer</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 RCG 2.60.07        Created

        private void FragmentWindowTimerCallback(object state)
        {
            if (m_HandlingFragmentedMessage)
            {
                if (m_FragmentRetryCounter < MAX_APS_RETRIES)
                {
                    SendFragmentReply();
                }
                else
                {
                    ClearFragmentInformation();
                }
            }
        }

        /// <summary>
        /// Clears any remaining fragmentation information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 RCG 2.60.07        Created
        
        private void ClearFragmentInformation()
        {
            m_HandlingFragmentedMessage = false;
            m_FragmentWindowTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_FragmentWindowStart = 0;
            m_FragmentsReceived.Clear();
            m_FragmentsExpected = 0;
            m_FragmentSequenceNumber = 0;
            m_FragmentRetryCounter = 0;
        }

        /// <summary>
        /// Send the fragment reply
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 RCG 2.60.07        Created

        private void SendFragmentReply()
        {
            EmberStatus Status;
            byte WindowBitfield;

            if (m_HandlingFragmentedMessage && m_FragmentsReceived.Count > 0)
            {
                EmberApsFrame ReplyAPSFrame = new EmberApsFrame();
                ReplyAPSFrame.RawData = m_FragmentsReceived[0].APSFrame.RawData;

                WindowBitfield = GetCurrentWindowBitfield();

                ReplyAPSFrame.GroupID = (ushort)(WindowBitfield << 8 | m_FragmentWindowStart);

                if (WindowBitfield != 0xFF)
                {
                    m_FragmentRetryCounter++;
                }
                else
                {
                    m_FragmentRetryCounter = 0;   
                }

                // Send the reply so we can get the next message fragment
                m_Logger.WriteLine(EZSPLogLevels.EZSPProtocol, "Sending fragmentation reply. Group ID: " + ReplyAPSFrame.GroupID.ToString("X4", CultureInfo.InvariantCulture));
                SendReply(m_FragmentsReceived[0].SenderNodeID, ReplyAPSFrame, 0, new byte[0], out Status);
            }
        }

        /// <summary>
        /// Gets the bitfield for the current Window Fragment
        /// </summary>
        /// <returns>The Window Bitfield</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 RCG 2.60.07        Created
        
        private byte GetCurrentWindowBitfield()
        {
            byte Bitfield = 0;

            if (m_HandlingFragmentedMessage)
            {
                // The Window is the number of fragments that will be sent before a reply is required from us. When this bitfield is set to 0xFF that means
                // that all fragments have been received for the current window. The window size is at max 8 and if it is smaller than 8 we need to fill in
                // the unused bits with 1's. We also need to fill in any bits that are higher than the total number of fragments that we are expecting.
                // The rest of the bits should be set based on whether or not we have received that message. Once we have received all of the fragments 
                // in a window we will send the response and move on to the next window.
                for (int CurrentBit = 0; CurrentBit < 8; CurrentBit++)
                {
                    if (CurrentBit >= m_FragmentWindowSize
                        || m_FragmentWindowStart + CurrentBit > m_FragmentsExpected
                        || m_FragmentsReceived.Where(f => EmberApsFrame.GetFragmentNumberFromGroupID(f.APSFrame.GroupID) == m_FragmentWindowStart + CurrentBit).Count() > 0)
                    {
                        Bitfield |= (byte)(1 << CurrentBit);
                    }
                }
            }

            return Bitfield;
        }

        /// <summary>
        /// Raises the DSA Verified event
        /// </summary>
        /// <param name="status">The status of the DSA Verify</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnDsaVerified(EmberStatus status)
        {
            if (DsaVerified != null)
            {
                DsaVerified(this, new EmberStatusEventArgs(status));
            }
        }

        /// <summary>
        /// Raises the DSA Signed event
        /// </summary>
        /// <param name="status">The status of the DSA Sign</param>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The message contents</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnDsaSigned(EmberStatus status, byte messageLength, byte[] messageContents)
        {
            if (DsaSigned != null)
            {
                DsaSigned(this, new DsaSignedEventArgs(status, messageLength, messageContents));
            }
        }

        /// <summary>
        /// Raises the SmacsCalculated event
        /// </summary>
        /// <param name="status">The status of the key generation</param>
        /// <param name="initiatorSmac">The initiator's SMAC</param>
        /// <param name="responderSmac">The responder's SMAC</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnSmacsCalculated(EmberStatus status, byte[] initiatorSmac, byte[] responderSmac)
        {
            if (SmacsCalculated != null)
            {
                SmacsCalculated(this, new SmacsCalculatedEventArgs(status, initiatorSmac, responderSmac));
            }
        }

        /// <summary>
        /// Raises the CBKE key generated event
        /// </summary>
        /// <param name="status">The status of the key generation</param>
        /// <param name="publicKey">The generate public key</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnCBKEKeyGenerated(EmberStatus status, byte[] publicKey)
        {
            if (CBKEKeyGenerated != null)
            {
                CBKEKeyGenerated(this, new CBKEKeyGeneratedEventArgs(status, publicKey));
            }
        }

        /// <summary>
        /// Raises the Trust Center Joined event
        /// </summary>
        /// <param name="newNodeID">The Node ID of the new device</param>
        /// <param name="newEUI">The EUI of the new device</param>
        /// <param name="status">The device status</param>
        /// <param name="policyDecision">The policy decision for the device join</param>
        /// <param name="parentNodeID">The Parent Node ID for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnTrustCenterJoined(ushort newNodeID, ulong newEUI, EmberDeviceUpdate status, EmberJoinDecision policyDecision, ushort parentNodeID)
        {
            if (TrustCenterJoined != null)
            {
                TrustCenterJoined(this, new TrustCenterJoinedEventArgs(newNodeID, newEUI, status, policyDecision, parentNodeID));
            }
        }

        /// <summary>
        /// Raises the ZigBee Key Established event
        /// </summary>
        /// <param name="eui">The partner EUI</param>
        /// <param name="status">The status of the key establishment</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnZigBeeKeyEstablished(ulong eui, EmberKeyStatus status)
        {
            if (ZigBeeKeyEstablished != null)
            {
                ZigBeeKeyEstablished(this, new ZigBeeKeyEstablishedEventArgs(eui, status));
            }
        }

        /// <summary>
        /// Raises the network key switched event
        /// </summary>
        /// <param name="sequenceNumber">The sequence number of the new network key</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnNetworkKeySwitched(byte sequenceNumber)
        {
            if (NetworkKeySwitched != null)
            {
                NetworkKeySwitched(this, new NetworkKeySwitchedEventArgs(sequenceNumber));
            }
        }

        /// <summary>
        /// Raises the Raw Message Sent event
        /// </summary>
        /// <param name="emberStatus">The status of the send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnRawMessageSent(EmberStatus emberStatus)
        {
            if (RawMessageSent != null)
            {
                RawMessageSent(this, new EmberStatusEventArgs(emberStatus));
            }
        }

        /// <summary>
        /// Raises the MAC Passthrough Filter Match message received
        /// </summary>
        /// <param name="filterIndexMatch">The index of the matched filter</param>
        /// <param name="messageType">The message type</param>
        /// <param name="lastHopLqi">The last hop LQI</param>
        /// <param name="lastHopRssi">The last hop RSSI</param>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The contents of the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnMacFilterMatchMessageReceived(byte filterIndexMatch, EmberMacPassthroughType messageType, byte lastHopLqi, byte lastHopRssi, byte messageLength, byte[] messageContents)
        {
            if (MacFilterMatchMessageReceived != null)
            {
                MacFilterMatchMessageReceived(this, new MacFilterMatchMessageReceivedEventArgs(filterIndexMatch, messageType, lastHopLqi, lastHopRssi, messageLength, messageContents));
            }
        }

        /// <summary>
        /// Raises the MAC passthrough message received event
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="lastHopLqi">The LQI of the last hop</param>
        /// <param name="lastHopRssi">The RSSI of the last hop</param>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The contents of the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnMacPassthroughMessageReceived(EmberMacPassthroughType messageType, byte lastHopLqi, byte lastHopRssi, byte messageLength, byte[] messageContents)
        {
            if (MacPassthroughMessageReceived != null)
            {
                MacPassthroughMessageReceived(this, new MacPassthroughMessageReceivedEventArgs(messageType, lastHopLqi, lastHopRssi, messageLength, messageContents));
            }
        }

        /// <summary>
        /// Raises the ID Conflict Detected event
        /// </summary>
        /// <param name="nodeID">The node ID that is conflicting.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnIDConflictDetected(ushort nodeID)
        {
            if (IDConflictDetected != null)
            {
                IDConflictDetected(this, new NodeIDEventArgs(nodeID));
            }
        }

        /// <summary>
        /// Raises the Route Error Occurred event
        /// </summary>
        /// <param name="status">The error status</param>
        /// <param name="target">The target node ID</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnRoutErrorOccurred(EmberStatus status, ushort target)
        {
            if (RouteErrorOccurred != null)
            {
                RouteErrorOccurred(this, new RouteErrorOccurredEventArgs(status, target));
            }
        }

        /// <summary>
        /// Raises the Many To One Route Available event
        /// </summary>
        /// <param name="sourceNodeID">The node ID of the concentrator</param>
        /// <param name="sourceEUI">The EUI of the concentrator</param>
        /// <param name="cost">The path cost to the concentrator</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnManyToOneRouteAvailable(ushort sourceNodeID, ulong sourceEUI, byte cost)
        {
            if (ManyToOneRouteAvailable != null)
            {
                ManyToOneRouteAvailable(this, new ManyToOneRouteAvailableEventArgs(sourceNodeID, sourceEUI, cost));
            }
        }

        /// <summary>
        /// Raises the Route Record Received event
        /// </summary>
        /// <param name="sourceNodeID">The Node ID of the source of the route record</param>
        /// <param name="sourceEUI">The EUI of the source of the route record</param>
        /// <param name="lastHopLqi">The last hop LQI of the message</param>
        /// <param name="lastHopRssi">The last hop RSSI of the message</param>
        /// <param name="relayCount">The number of relay</param>
        /// <param name="relayList">The list of Node IDs for the route</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnRouteRecordReceived(ushort sourceNodeID, ulong sourceEUI, byte lastHopLqi, byte lastHopRssi, byte relayCount, ushort[] relayList)
        {
            if (RouteRecordReceived != null)
            {
                RouteRecordReceived(this, new RouteRecordReceivedEventArgs(sourceNodeID, sourceEUI, lastHopLqi, lastHopRssi, relayCount, relayList));
            }
        }

        /// <summary>
        /// Raises the Message Received event
        /// </summary>
        /// <param name="newMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnMessageReceived(IncomingMessage newMessage)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageReceivedEventArgs(newMessage));
            }
        }

        /// <summary>
        /// Raises the SenderEUIReceived event
        /// </summary>
        /// <param name="eui">The EUI received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnSenderEUIReceived(ulong eui)
        {
            if (SenderEUIReceived != null)
            {
                SenderEUIReceived(this, new EUIEventArgs(eui));
            }
        }

        /// <summary>
        /// Raises the PollReceived event
        /// </summary>
        /// <param name="nodeID">The node ID of the child requesting data</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnPollReceived(ushort nodeID)
        {
            if (PollReceived != null)
            {
                PollReceived(this, new NodeIDEventArgs(nodeID));
            }
        }

        /// <summary>
        /// Raises the Poll Received event
        /// </summary>
        /// <param name="emberStatus">The status of the poll</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnPollComplete(EmberStatus emberStatus)
        {
            if (PollComplete != null)
            {
                PollComplete(this, new EmberStatusEventArgs(emberStatus));
            }
        }

        /// <summary>
        /// Raises the message sent event
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="indexOrDestination">The index or destination node ID of the message</param>
        /// <param name="apsFrame">The APS frame for the message</param>
        /// <param name="messageTag">The message tag</param>
        /// <param name="status">Whether or not an ack has been received from the destination</param>
        /// <param name="messageLength">The length of the message contents</param>
        /// <param name="messageContents">The contents of the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnMessageSent(EmberOutgoingMessageType messageType, ushort indexOrDestination, EmberApsFrame apsFrame, byte messageTag, EmberStatus status, byte messageLength, byte[] messageContents)
        {
            if (MessageSent != null)
            {
                MessageSent(this, new MessageSentEventArgs(messageType, indexOrDestination, apsFrame, messageTag, status, messageLength, messageContents));
            }
        }

        /// <summary>
        /// Raises the Binding Set Remotely event
        /// </summary>
        /// <param name="newEntry">The new binding table entry</param>
        /// <param name="index">The index of the new binding</param>
        /// <param name="status">The status of the binding</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnBindingSetRemotely(EmberBindingTableEntry newEntry, byte index, EmberStatus status)
        {
            if (BindingSetRemotely != null)
            {
                BindingSetRemotely(this, new BindingSetRemotelyEventArgs(newEntry, index, status));
            }
        }

        /// <summary>
        /// Raises the Binding Deleted Remotely event
        /// </summary>
        /// <param name="index">The index of the binding deleted</param>
        /// <param name="status">The status of the deletion</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnBindingDeletedRemotely(byte index, EmberStatus status)
        {
            if (BindingDeletedRemotely != null)
            {
                BindingDeletedRemotely(this, new BindingDeletedRemotelyEventArgs(index, status));
            }
        }

        /// <summary>
        /// Raises the Child Joined event
        /// </summary>
        /// <param name="index">The index of the child</param>
        /// <param name="joining">Whether the child is joining or leaving</param>
        /// <param name="childID">The Node ID of the child</param>
        /// <param name="childEUI">The EUI of the child</param>
        /// <param name="childType">The device type of the child</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnChildJoined(byte index, bool joining, ushort childID, ulong childEUI, EmberNodeType childType)
        {
            if (ChildJoined != null)
            {
                ChildJoined(this, new ChildJoinedEventArgs(index, joining, childID, childEUI, childType));
            }
        }

        /// <summary>
        /// Raises the Stack Status Updated Event
        /// </summary>
        /// <param name="stackStatus">The new stack status</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnStackStatusUpdated(EmberStatus stackStatus)
        {
            if (StackStatusUpdated != null)
            {
                StackStatusUpdated(this, new StackStatusUpdatedEventArgs(stackStatus));
            }
        }

        /// <summary>
        /// Raises the Timer Occurred event
        /// </summary>
        /// <param name="timerID">The timer ID that occurred</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void OnTimerOccurred(byte timerID)
        {
            if (TimerOccurred != null)
            {
                TimerOccurred(this, new TimerEventArgs(timerID));
            }
        }

        /// <summary>
        /// Raises the Scan Completed event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnScanCompleted()
        {
            if (ScanCompleted != null)
            {
                ScanCompleted(this, new EventArgs());
            }
        }

        #endregion

        #region Member Variables

        private ASHProtocol m_ASH;
        private bool m_Joined;
        private bool m_Connected;
        private byte m_CurrentSequenceNumber;
        private EventHandler m_FrameReceivedHandler;

        // Lists for keeping track of the responses received
        private List<EZSPResponseFrame> m_CommandResponses;
        private List<EZSPResponseFrame> m_AsyncCallbacks;

        // Callback Handler Info
        private List<ZigbeeNetwork> m_ActiveScanResults;
        private List<ZigBeeEnergyScanResult> m_EnergyScanResults;
        private EmberStatus m_StackStatus;

        // Fragment Handling
        private bool m_HandlingFragmentedMessage;
        private List<IncomingMessage> m_FragmentsReceived;
        private byte m_FragmentSequenceNumber;
        private byte m_FragmentsExpected;
        private ushort m_FragmentTimeout;
        private ushort m_FragmentWindowSize;
        private Timer m_FragmentWindowTimer;
        private byte m_FragmentWindowStart;
        private ushort m_FragmentRetryCounter;
        private bool m_HaveFragmentConfigurationData;

        private EZSPLogger m_Logger;

        #endregion
    }

    /// <summary>
    /// EZSP Frame for commands issues to the device
    /// </summary>
    public class EZSPCommandFrame
    {
        #region Constants

        private const byte FRAME_TYPE_MASK = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EZSPCommandFrame()
        {
            m_SequenceNumber = 0;
            m_FrameControl = 0;
            m_FrameID = 0;
            m_ParameterData = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Sequence Number of the frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte SequenceNumber
        {
            get
            {
                return m_SequenceNumber;
            }
            set
            {
                m_SequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the frame ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EZSPFrameIDs FrameID
        {
            get
            {
                return (EZSPFrameIDs)m_FrameID;
            }
            set
            {
                m_FrameID = (byte)value;
            }
        }

        /// <summary>
        /// Gets or sets the parameter data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] ParameterData
        {
            get
            {
                return m_ParameterData;
            }
            set
            {
                m_ParameterData = value;
            }
        }

        /// <summary>
        /// Gets or sets the sleep mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EZSPSleepMode SleepMode
        {
            get
            {
                return (EZSPSleepMode)m_FrameControl;
            }
            set
            {
                m_FrameControl = (byte)value;
            }
        }

        /// <summary>
        /// Gets or sets the raw frame data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] RawFrame
        {
            get
            {
                byte[] Frame = new byte[3 + m_ParameterData.Length];

                Frame[0] = m_SequenceNumber;
                Frame[1] = m_FrameControl;
                Frame[2] = m_FrameID;

                if (m_ParameterData.Length > 0)
                {
                    Array.Copy(m_ParameterData, 0, Frame, 3, m_ParameterData.Length);
                }

                return Frame;
            }
            set
            {
                if (value != null && value.Length >= 3)
                {
                    // Make sure the frame is a response type
                    if ((value[1] & FRAME_TYPE_MASK) == 0)
                    {
                        m_ParameterData = new byte[value.Length - 3];

                        m_SequenceNumber = value[0];
                        m_FrameControl = value[1];
                        m_FrameID = value[2];

                        if (m_ParameterData.Length > 0)
                        {
                            Array.Copy(value, 3, m_ParameterData, 0, m_ParameterData.Length);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The frame data is not a valid command frame");
                    }
                }
                else
                {
                    throw new ArgumentException("RawFrame can not be set to null and must have a length >= 3");
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_SequenceNumber;
        private byte m_FrameControl;
        private byte m_FrameID;
        private byte[] m_ParameterData;

        #endregion
    }

    /// <summary>
    /// EZSP Frame for responses from the device
    /// </summary>
    public class EZSPResponseFrame
    {
        #region Constants

        // Frame Control Masks
        private const byte FRAME_TYPE_MASK = 0x80;
        private const byte OVERFLOW_MASK = 0x01;
        private const byte TRUNCATED_MASK = 0x02;
        private const byte CALLBACK_PENDING_MASK = 0x04;
        private const byte CALLBACK_TYPE_MASK = 0x18;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EZSPResponseFrame()
        {
            m_SequenceNumber = 0;
            m_FrameControl = FRAME_TYPE_MASK;
            m_FrameID = 0;
            m_ParameterData = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Sequence Number of the frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte SequenceNumber
        {
            get
            {
                return m_SequenceNumber;
            }
            set
            {
                m_SequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the frame ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EZSPFrameIDs FrameID
        {
            get
            {
                return (EZSPFrameIDs)m_FrameID;
            }
            set
            {
                m_FrameID = (byte)value;
            }
        }

        /// <summary>
        /// Gets or sets the parameter data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] ParameterData
        {
            get
            {
                return m_ParameterData;
            }
            set
            {
                m_ParameterData = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the NCP has run out of memory
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool HasOverflown
        {
            get
            {
                return (m_FrameControl & OVERFLOW_MASK) == OVERFLOW_MASK;
            }
            set
            {
                if (value)
                {
                    m_FrameControl = (byte)(m_FrameControl | OVERFLOW_MASK);
                }
                else
                {
                    m_FrameControl = (byte)(m_FrameControl & ~OVERFLOW_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not the response was truncated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool ResponseTruncated
        {
            get
            {
                return (m_FrameControl & TRUNCATED_MASK) == TRUNCATED_MASK;
            }
            set
            {
                if (value)
                {
                    m_FrameControl = (byte)(m_FrameControl | TRUNCATED_MASK);
                }
                else
                {
                    m_FrameControl = (byte)(m_FrameControl & ~TRUNCATED_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not a callback is still pending
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool CallbackPending
        {
            get
            {
                return (m_FrameControl & CALLBACK_PENDING_MASK) == CALLBACK_PENDING_MASK;
            }
            set
            {
                if (value)
                {
                    m_FrameControl = (byte)(m_FrameControl | CALLBACK_PENDING_MASK);
                }
                else
                {
                    m_FrameControl = (byte)(m_FrameControl & ~CALLBACK_PENDING_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets the callback type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EZSPCallbackType CallbackType
        {
            get
            {
                return (EZSPCallbackType)(m_FrameControl & CALLBACK_TYPE_MASK);
            }
            set
            {
                m_FrameControl = (byte)(m_FrameControl & ~CALLBACK_TYPE_MASK);
                m_FrameControl = (byte)(m_FrameControl | (byte)value);
            }
        }

        /// <summary>
        /// Gets or sets the raw frame data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte[] RawFrame
        {
            get
            {
                byte[] Frame = new byte[3 + m_ParameterData.Length];

                Frame[0] = m_SequenceNumber;
                Frame[1] = m_FrameControl;
                Frame[2] = m_FrameID;

                if (m_ParameterData.Length > 0)
                {
                    Array.Copy(m_ParameterData, 0, Frame, 3, m_ParameterData.Length);
                }

                return Frame;
            }
            set
            {
                if (value != null && value.Length >= 3)
                {
                    // Make sure the frame is a response type
                    if ((value[1] & FRAME_TYPE_MASK) == FRAME_TYPE_MASK)
                    {
                        m_ParameterData = new byte[value.Length - 3];

                        m_SequenceNumber = value[0];
                        m_FrameControl = value[1];
                        m_FrameID = value[2];

                        if (m_ParameterData.Length > 0)
                        {
                            Array.Copy(value, 3, m_ParameterData, 0, m_ParameterData.Length);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The frame data is not a valid response frame");
                    }
                }
                else
                {
                    throw new ArgumentException("RawFrame can not be set to null and must have a length >= 3");
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_SequenceNumber;
        private byte m_FrameControl;
        private byte m_FrameID;
        private byte[] m_ParameterData;

        #endregion
    }
}
