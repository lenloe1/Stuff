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
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using Itron.Metering.Utilities;

namespace Itron.Metering.Zigbee
{
    #region Definitions

    /// <summary>
    /// Profile IDs used by ZigBee
    /// </summary>
    public enum ZigBeeProfileIDs : ushort
    {
        /// <summary>ZigBee Device Object (ZDO)</summary>
        ZigBeeDeviceObject = 0x0000,
        /// <summary>Smart Energy Profile</summary>
        SmartEnergy = 0x0109,
        /// <summary>Itron Private Profile</summary>
        ItronPrivateProfile = 0xC177,
    }

    /// <summary>
    /// The Clusters available to the ZDO Profile
    /// </summary>
    public enum ZDOClusters : ushort
    {
        /// <summary>Network Address Request</summary>
        NetworkAddressRequest = 0x0000,
        /// <summary>Network Address Response</summary>
        NetworkAddressResponse = 0x8000,
        /// <summary>IEEE Address Request</summary>
        IEEEAddressRequest = 0x0001,
        /// <summary>IEEE Address Response</summary>
        IEEEAddressResponse = 0x8001,
        /// <summary>Node Descriptor Request</summary>
        NodeDescriptorRequest = 0x0002,
        /// <summary>Node Descriptor Response</summary>
        NodeDescriptorResponse = 0x8002,
        /// <summary>Power Descriptor Request</summary>
        PowerDescriptorRequest = 0x0003,
        /// <summary>Power Descriptor Response</summary>
        PowerDescriptorResponse = 0x8003,
        /// <summary>Simple Descriptor Request</summary>
        SimpleDescriptorRequest = 0x0004,
        /// <summary>Simple Descriptor Response</summary>
        SimpleDescriptorResponse = 0x8004,
        /// <summary>Active EndpointsRequest</summary>
        ActiveEndpointsRequest = 0x0005,
        /// <summary>Active Endpoints Response</summary>
        ActiveEndpointsResponse = 0x8005,
        /// <summary>Match Descriptors Request</summary>
        MatchDescriptorsRequest = 0x0006,
        /// <summary>Match Descriptors Response</summary>
        MatchDescriptorsResponse = 0x8006,
        /// <summary>Discovery Cache Request</summary>
        DiscoveryCacheRequest = 0x0012,
        /// <summary>Discovery Cache Response</summary>
        DiscoveryCacheResponse = 0x8012,
        /// <summary>End Device Announce</summary>
        EndDeviceAnnounce = 0x0013,
        /// <summary>End Device Announce Response</summary>
        EndDeviceAnnounceResponse = 0x8013,
        /// <summary>System Server Discovery Request</summary>
        SystemServerDiscoveryRequest = 0x0015,
        /// <summary>System Server Discover Response</summary>
        SystemServerDiscoveryResponse = 0x8015,
        /// <summary>Find Node Cache Request</summary>
        FindNodeCacheRequest = 0x001C,
        /// <summary>Find Node Cache Response</summary>
        FindNodeCacheResponse = 0x801C,
        /// <summary>End Device Bind Request</summary>
        EndDeviceBindRequest = 0x0020,
        /// <summary>End Device Bind Response</summary>
        EndDeviceBindResponse = 0x8020,
        /// <summary>Bind Request</summary>
        BindRequest = 0x0021,
        /// <summary>Bind Response</summary>
        BindResponse = 0x8021,
        /// <summary>Unbind Request</summary>
        UnbindRequest = 0x0022,
        /// <summary>Unbind Response</summary>
        UnbindResponse = 0x8022,
        /// <summary>LQI Table Request</summary>
        LqiTableRequest = 0x0031,
        /// <summary>LQI Table Response</summary>
        LqiTableResponse = 0x8031,
        /// <summary>Routing Table Request</summary>
        RoutingTableRequest = 0x0032,
        /// <summary>Routing Table Response</summary>
        RoutingTableResponse = 0x8032,
        /// <summary>Binding Table Request</summary>
        BindingTableRequest = 0x0033,
        /// <summary>Binding Table Response</summary>
        BindingTableResponse = 0x8033,
        /// <summary>Leave Request</summary>
        LeaveRequest = 0x0034,
        /// <summary>Leave Response</summary>
        LeaveResponse = 0x8034,
        /// <summary>Permit Joining Request</summary>
        PermitJoiningRequest = 0x0036,
        /// <summary>Permit Joining Response</summary>
        PermitJoiningResponse = 0x8036,
        /// <summary>Network Update Request</summary>
        NetworkUpdateRequest = 0x0038,
        /// <summary>Network Update Response</summary>
        NetworkUpdateResponse = 0x8038,
    }

    /// <summary>
    /// ZDO Statuses
    /// </summary>
    public enum ZDOStatus : byte
    {
        /// <summary>Success</summary>
        Success = 0x00,
        /// <summary>Invalid Request Type</summary>
        InvalidRequestType = 0x80,
        /// <summary>Device Not Found</summary>
        DeviceNotFound = 0x81,
        /// <summary>Invalid Endpoint</summary>
        InvalidEndpoint = 0x82,
        /// <summary>Not Active</summary>
        NotActive = 0x83,
        /// <summary>Not Supported</summary>
        NotSupported = 0x84,
        /// <summary>Timeout</summary>
        Timeout = 0x85,
        /// <summary>No Match</summary>
        NoMatch = 0x86,
        /// <summary>No Entry</summary>
        NoEntry = 0x88,
        /// <summary>No Descriptor</summary>
        NoDescriptor = 0x89,
        /// <summary>Insufficient Space</summary>
        InsufficientSpace = 0x8A,
        /// <summary>Not Permitted</summary>
        NotPermitted = 0x8B,
        /// <summary>Table Full</summary>
        TableFull = 0x8C,
        /// <summary>Not Authorized</summary>
        NotAuthorized = 0x8D,
    }

    /// <summary>
    /// Enum of attribute from Identify cluster
    /// </summary>
    public enum ZCLIdentifyAttribute : ushort
    {
        /// <summary>
        /// The identify time from the meter
        /// </summary>
        IdentifyTime = 0x0000
    }

    /// <summary>
    /// The list of ZCL Attributes
    /// </summary>
    public enum ZCLAttributes : ushort
    {
        /// <summary>The ZCL Version used</summary>
        ZCLVersion = 0x0000,
        /// <summary>The Application Version</summary>
        ApplicationVersion = 0x0001,
        /// <summary>The Stack Version</summary>
        StackVersion = 0x0002,
        /// <summary>The HW Version</summary>
        HWVersion = 0x0003,
        /// <summary>The Manufacturer Name</summary>
        ManufacturerName = 0x0004,
        /// <summary>The Model Identifier</summary>
        ModelIdentifier = 0x0005,
        /// <summary>The Date Code</summary>
        DateCode = 0x0006,
        /// <summary>The Power Source used</summary>
        PowerSource = 0x0007,
        /// <summary>The location description</summary>
        LocationDescription = 0x0010,
        /// <summary>The Physical Environment description</summary>
        PhysicalEnvironment = 0x0011,
        /// <summary>Whether or not the device is enabled</summary>
        DeviceEnabled = 0x0012,
        /// <summary>The Alarm Mask</summary>
        AlarmMask = 0x0013,
    }


    /// <summary>
    /// The list of ZCL OTA Attributes
    /// </summary>
    public enum ZCLOTAAttributes : ushort
    {
        /// <summary>The status upgrade</summary>
        ImageUpgradeStatus = 0x0006,
 
    }

    /// <summary>
    /// Attribute ID's used for retrieving the time values
    /// </summary>
    public enum ZCLTimeAttributes : ushort
    {
        /// <summary>Retrieve the current time in UTC</summary>
        Time = 0x0000,
        /// <summary>Gets status information about the time</summary>
        TimeStatus = 0x0001,
        /// <summary>Gets the Time Zone offset</summary>
        TimeZone = 0x0002,
        /// <summary>Gets the start time of DST</summary>
        DstStart = 0x0003,
        /// <summary>Gets the end time of DST</summary>
        DstEnd = 0x0004,
        /// <summary>Gets the length of the adjustment for DST</summary>
        DstShift = 0x0005,
        /// <summary>Gets the local standard time (not adjusted for DST)</summary>
        StandardTime = 0x0006,
        /// <summary>Gets the local time</summary>
        LocalTime = 0x0007,
    }

    /// <summary>
    /// Time Status bitfield
    /// </summary>
    [Flags]
    public enum ZCLTimeStatus : byte
    {
        /// <summary>No Flags are set</summary>
        None = 0x00,
        /// <summary>Whether or not the clock corresponds to the time standard</summary>
        MasterClock = 0x01,
        /// <summary>Whether or not the time is synchronized to the network</summary>
        Synchronized = 0x02,
        /// <summary>Whether or not the Time Zone and DST fields are set</summary>
        MasterZoneDST = 0x04,
    }

    /// <summary>
    /// The Power Source Attribute values
    /// </summary>
    public enum ZCLPowerSource : byte
    {
        /// <summary>Unknown</summary>
        Unknown = 0x00,
        /// <summary>Mains (Single Phase)</summary>
        SinglePhase = 0x01,
        /// <summary>Mains (3 Phase)</summary>
        PolyPhase = 0x02,
        /// <summary>Battery</summary>
        Battery = 0x03,
        /// <summary>DC Source</summary>
        DCSource = 0x04,
        /// <summary>Emergency Mains constantly powered</summary>
        EmergencyMainsWithConstantPower = 0x05,
        /// <summary>Emergency mains and transfer switch</summary>
        EmergencyMainsWithTransferSwitch = 0x06,
    }

    /// <summary>
    /// ZCL Data Types
    /// </summary>
    public enum ZCLDataType : byte
    {
        /// <summary>No Value</summary>
        Null = 0x00,
        /// <summary>8-bit Generic Data</summary>
        Data8 = 0x08,
        /// <summary>16-bit Generic Data</summary>
        Data16 = 0x09,
        /// <summary>24-bit Generic Data</summary>
        Data24 = 0x0A,
        /// <summary>32-bit Generic Data</summary>
        Data32 = 0x0B,
        /// <summary>40-bit Generic Data</summary>
        Data40 = 0x0C,
        /// <summary>48-bit Generic Data</summary>
        Data48 = 0x0D,
        /// <summary>56-bit Generic Data</summary>
        Data56 = 0x0E,
        /// <summary>64-bit Generic Data</summary>
        Data64 = 0x0F,
        /// <summary>Boolean</summary>
        Boolean = 0x10,
        /// <summary>8-bit Bitmap</summary>
        Bitmap8 = 0x18,
        /// <summary>16-bit Bitmap</summary>
        Bitmap16 = 0x019,
        /// <summary>24-bit Bitmap</summary>
        Bitmap24 = 0x1A,
        /// <summary>32-bit Bitmap</summary>
        Bitmap32 = 0x1B,
        /// <summary>40-bit Bitmap</summary>
        Bitmap40 = 0x1C,
        /// <summary>48-bit Bitmap</summary>
        Bitmap48 = 0x1D,
        /// <summary>56-bit Bitmap</summary>
        Bitmap56 = 0x1E,
        /// <summary>64-bit Bitmap</summary>
        Bitmap64 = 0x1F,
        /// <summary>8-bit Unsigned Integer</summary>
        Uint8 = 0x20,
        /// <summary>16-bit Unsigned Integer</summary>
        Uint16 = 0x21,
        /// <summary>24-bit Unsigned Integer</summary>
        Uint24 = 0x22,
        /// <summary>31-bit Unsigned Integer</summary>
        Uint32 = 0x23,
        /// <summary>40-bit Unsigned Integer</summary>
        Uint40 = 0x24,
        /// <summary>48-bit Unsigned Integer</summary>
        Uint48 = 0x25,
        /// <summary>56-bit Unsigned Integer</summary>
        Uint56 = 0x26,
        /// <summary>64-bit Unsigned Integer</summary>
        Uint64 = 0x27,
        /// <summary>8-bit Signed Integer</summary>
        Int8 = 0x28,
        /// <summary>16-bit Signed Integer</summary>
        Int16 = 0x29,
        /// <summary>24-bit Signed Integer</summary>
        Int24 = 0x2A,
        /// <summary>32-bit Signed Integer</summary>
        Int32 = 0x2B,
        /// <summary>40-bit Signed Integer</summary>
        Int40 = 0x2C,
        /// <summary>48-bit Signed Integer</summary>
        Int48 = 0x2D,
        /// <summary>56-bit Signed Integer</summary>
        Int56 = 0x2E,
        /// <summary>64-bit Signed Integer</summary>
        Int64 = 0x2F,
        /// <summary>8-bit Enum</summary>
        Enum8 = 0x30,
        /// <summary>16-bit Enum</summary>
        Enum16 = 0x31,
        /// <summary>16-bit Float</summary>
        Float16 = 0x38,
        /// <summary>32-bit Float</summary>
        Float32 = 0x39,
        /// <summary>64-bit Float</summary>
        Float64 = 0x3A,
        /// <summary>Octet String</summary>
        OctetString = 0x41,
        /// <summary>Character String</summary>
        CharacterString = 0x42,
        /// <summary>2-byte octet string</summary>
        LongOctetString = 0x43,
        /// <summary>2-byte character string</summary>
        LongCharacterString = 0x44,
        /// <summary>Array</summary>
        Array = 0x48,
        /// <summary>Structure</summary>
        Structure = 0x4C,
        /// <summary>Set</summary>
        Set = 0x50,
        /// <summary>Bag</summary>
        Bag = 0x51,
        /// <summary>Time of day</summary>
        TimeOfDay = 0xE0,
        /// <summary>Date</summary>
        Date = 0xE1,
        /// <summary>UTC Time</summary>
        UTCTime = 0x0E2,
        /// <summary>Cluster ID</summary>
        ClusterID = 0xE8,
        /// <summary>Attribute ID</summary>
        AttributeID = 0xE9,
        /// <summary>BAC Net OID</summary>
        BACnetOID = 0xEA,
        /// <summary>IEEE Address</summary>
        IEEEAddress = 0xF0,
        /// <summary>Security Key</summary>
        SecurityKey = 0xF1,
        /// <summary>Unknown</summary>
        Unknown = 0xFF,
    }

    /// <summary>
    /// General ZCL Command ID's
    /// </summary>
    public enum GeneralZCLCommandIDs : byte
    {
        /// <summary>Read Attributes</summary>
        ReadAttributes = 0x00,
        /// <summary>Read Attributes Response</summary>
        ReadAttributesResponse = 0x01,
        /// <summary>Write Attributes</summary>
        WriteAttributes = 0x02,
        /// <summary>Write Attributes Undivided</summary>
        WriteAttributesUndivided = 0x03,
        /// <summary>Write Attributes Response</summary>
        WriteAttributesResponse = 0x04,
        /// <summary>Write Attributes No Response</summary>
        WriteAttributesNoResponse = 0x05,
        /// <summary>Configure Reporting</summary>
        ConfigureReporting = 0x06,
        /// <summary>Configure Reporting Response</summary>
        ConfigureReportingResponse = 0x07,
        /// <summary>Read Reporting Configuration</summary>
        ReadReportingConfiguration = 0x08,
        /// <summary>Read Reporting Configuration Response</summary>
        ReadReportingConfigurationResponse = 0x09,
        /// <summary>Report Attributes</summary>
        ReportAttributes = 0x0A,
        /// <summary>Default Response</summary>
        DefaultResponse = 0x0B,
        /// <summary>Discover Attributes</summary>
        DiscoverAttributes = 0x0C,
        /// <summary>Discover Attributes Response</summary>
        DiscoverAttributesResponse = 0x0D,
        /// <summary>Read Attributes Structured</summary>
        ReadAttributesStructured = 0x0E,
        /// <summary>Write Attributes Structured</summary>
        WriteAttributesStructured = 0x0F,
        /// <summary>Write Attributes Structured Response</summary>
        WriteAttributesStructuredResponse = 0x10,
    }

    /// <summary>
    /// The direction the data will be sent when using Attribute Reporting
    /// </summary>
    public enum ReportingDirection : byte
    {
        /// <summary>The data will be sent</summary>
        Send = 0x00,
        /// <summary>The data will be received</summary>
        Receive = 0x01,
    }

    #endregion

    #region Event Delegates
    /// <summary>
    /// Delegate for the Default Response Event Handler
    /// </summary>
    public delegate void DefaultResponseEventHandler(object sender, DefaultResponseEventArgs e);

    /// <summary>
    /// Delegate for the IPP Data Response Event Handler
    /// </summary>
    public delegate void IPPDataResponseEventHandler(object sender, IPPDataResponseEventArgs e);

    /// <summary>
    /// Delegate for the Read Response Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    /// 
    public delegate void ReadResponseEventHandler(object sender, ReadResponseEventArgs e);

    /// <summary>
    /// Delegate for the Write Response Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    /// 
    public delegate void WriteResponseEventHandler(object sender, WriteResponseEventArgs e);

    /// <summary>
    /// Delegate for the Write Response Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    /// 
    public delegate void IdentifyQueryResponseEventHandler(object sender, IdentifyQueryResponseEventArgs e);

    #endregion

    /// <summary>
    /// Base class for the ZigBee Application Layer
    /// </summary>
    public class ZigBeeApplication
    {
        #region Public events

        /// <summary>
        /// Event to fire when a Default Response is returned from a General ZCL Command.
        /// </summary>
        public event DefaultResponseEventHandler DefaultResponseReceived;

        /// <summary>
        /// Event to fire when a IPP Data Response is received as an incoming message.
        /// </summary>
        public event IPPDataResponseEventHandler IPPDataResponseReceived;

        /// <summary>
        /// Event to fire when a Read Response command is received.
        /// </summary>
        public event ReadResponseEventHandler ReadResponseReceived;

        /// <summary>
        /// Event to fire when a Write Response command is received.
        /// </summary>
        public event WriteResponseEventHandler WriteResponseReceived;

        /// <summary>
        /// Event to fire when a identify query Response command is received.
        /// </summary>
        public event IdentifyQueryResponseEventHandler IdentifyQueryResponseReceived;

        #endregion

        #region Constants

        /// <summary>
        /// EZSP Protocol version expected
        /// </summary>
        protected const byte EZSP_PROTOCOL_VERSION = 4;
        private const ushort DEFAULT_BINDING_TABLE_SIZE = 6;
        private const byte ZCL_VERSION = 0x01;

        private readonly TimeSpan MAX_SCAN_TIME = new TimeSpan(0, 3, 0);
        private readonly TimeSpan JOIN_TIMEOUT = new TimeSpan(0, 0, 30);
        private readonly TimeSpan MESSAGE_SEND_TIMEOUT = new TimeSpan(0, 0, 3);
        private readonly TimeSpan MESSAGE_RESPONSE_TIMEOUT = new TimeSpan(0, 0, 3);

        private const ushort ITRON_MFG_CODE = 0x1028;

        private const byte EUI_DEST_ADDRESS_MODE = 0x03;
        private const byte NODE_DEST_ADDRESS_MODE = 0x01;

        /// <summary>
        /// The node ID used by the trust center (usually the meter)
        /// </summary>
        protected const ushort TRUST_CENTER_NODE_ID = 0x0000;

        private const int MAX_APS_PAYLOAD_SIZE = 73;

        private const ushort DEFAULT_POWER_DESCRIPTOR = 0x10C1; // 
        /// <summary>
        /// Reference time used for reading UTC Time objects
        /// </summary>
        protected static readonly DateTime UTC_REFERENCE_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Reference time used for reading Local Time objects
        /// </summary>
        protected static readonly DateTime LOCAL_REFERENCE_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);

        private const ushort CBKE_ESTABLISHMENT = 0x0001;
        private const ushort FAKE_CERT_ESTABLISHMENT = 0x0080;
        private const byte CBKE_GENERATE_TIME = 11;
        private const byte CBKE_CONFIRM_TIME = 16;
        private const byte DEFAULT_OPERATION_TIMEOUT = 3;

        /// <summary>
        /// The pre-configured key data that should be used when rejoining the network
        /// </summary>
        protected static readonly byte[] PRE_CONFIGURED_KEY = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        #endregion

        #region Definitions

        /// <summary>
        /// Key Establishment Commands
        /// </summary>
        protected enum KeyEstablishmentCommands : byte
        {
            /// <summary>Initiate Key Establishment</summary>
            InitiateKeyEstablishment = 0x00,
            /// <summary>Ephemeral data Request</summary>
            EphemeralDataRequest = 0x01,
            /// <summary>Confirm Key Data Request</summary>
            ConfirmKeyDataRequest = 0x02,
            /// <summary>Terminate Key Establishment</summary>
            TerminateKeyEstablishment = 0x03,
        }

        /// <summary>
        /// The various states within the key establishment process
        /// </summary>
        public enum KeyEstablishmentState
        {
            /// <summary>Not Established</summary>
            NotEstablished,
            /// <summary>Initiate Key Establishment message sent</summary>
            SentInitiateKeyEstablishment,
            /// <summary>Initiate Key Establishment message received</summary>
            ReceivedInitiateKeyEstablishment,
            /// <summary>Ephemeral Key sent</summary>
            SentEphemeralKey,
            /// <summary>Ephemeral Key received</summary>
            ReceivedEphemeralKey,
            /// <summary>Key Confirmation sent</summary>
            SentKeyConfirmation,
            /// <summary>Key Confirmation received</summary>
            ReceivedKeyConfirmation,
            /// <summary>The Key has been established</summary>
            Established,
        }

        /// <summary>
        /// Key Establishment Errors
        /// </summary>
        protected enum KeyEstablishmentErrors : byte
        {
            /// <summary>Unknown Issuer</summary>
            UnknownIssuer = 0x01,
            /// <summary>Bad Key Confirmation</summary>
            BadKeyConfirm = 0x02,
            /// <summary>Bad Message</summary>
            BadMessage = 0x03,
            /// <summary>No Resources</summary>
            NoResources = 0x04,
            /// <summary>Unsupported Suite</summary>
            UnsupportedSuite = 0x05,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts logging the EZSP communications
        /// </summary>
        /// <param name="logFileName">The path of the log file</param>
        /// <param name="loggedLevels">The levels to log</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void StartLogging(string logFileName, EZSPLogLevels loggedLevels)
        {
            m_Logger.StartLogging(logFileName, loggedLevels);
        }

        /// <summary>
        /// Stops logging the EZSP communications
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void StopLogging()
        {
            m_Logger.StopLogging();
        }

        /// <summary>
        /// Send a APS_CMD_REMOVE_DEVICE to specified router device. 
        /// </summary>
        /// <param name="destinationShortAddress"></param>
        /// <param name="destinationLongAddress"></param>
        /// <param name="childLongAddress"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/10/12 DEC ?.??.00        Created
        public EmberStatus RemoveDevice(ushort destinationShortAddress, ulong destinationLongAddress, ulong childLongAddress)
        {
            EmberStatus result;

            m_EZSP.RemoveDevice(destinationShortAddress, destinationLongAddress, childLongAddress, out result);

            return result;
        }

        /// <summary>
        /// Set the ZigBee Node Power Descriptor for this device. 
        /// </summary>
        /// <param name="availablePowerSources"></param>
        /// <param name="currentPowerMode"></param>
        /// <param name="currentPowerSource"></param>
        /// <param name="currentPowerSourceLevel"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/10/12 DEC ?.??.00        Created
        public void SetPowerDescriptor(byte availablePowerSources, byte currentPowerMode, byte currentPowerSourceLevel, byte currentPowerSource)
        {
            UInt16 value = 0;
            value |= (UInt16)((availablePowerSources & 0xf) << 12);
            value |= (UInt16)((currentPowerMode & 0xf) << 8);
            value |= (UInt16)((currentPowerSourceLevel & 0xf) << 4);
            value |= (UInt16)((currentPowerSource & 0xf) << 0);
            m_PowerDescriptor = value;

            if (m_EZSP != null && m_EZSP.IsConnected)
            {
                m_EZSP.SetPowerDescriptor(m_PowerDescriptor);
            }
        }

        /// <summary>
        /// Connects to the radio through the specified COM port
        /// </summary>
        /// <param name="portName">The name of the COM port the radio is on</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public virtual void Connect(string portName)
        {
            byte ProtocolVersion = 0;
            byte StackType = 0;
            ushort StackVersion = 0;
            EmberStatus Status = EmberStatus.FatalError;

            m_Comm = new EZSPSerialCommunications(m_Logger);
            m_ASH = new ASHProtocol(m_Comm, m_Logger);
            m_EZSP = new EZSPProtocol(m_ASH, m_Logger);

            m_ZDOSequence = 0;

            m_Comm.OpenPort(portName);

            m_EZSP.Connect();

            m_SentMessageTags.Clear();
            m_UnhandledMessages.Clear();

            m_LastRetrievedUTCTime = UTC_REFERENCE_TIME;
            m_UTCTimeOffset = new TimeSpan();
            m_TimeStatus = ZCLTimeStatus.None;
            m_DSTStart = LOCAL_REFERENCE_TIME;
            m_DSTEnd = LOCAL_REFERENCE_TIME;
            m_DSTShift = 0;
            m_StandardTime = LOCAL_REFERENCE_TIME;
            m_StandardTimeOffset = new TimeSpan();
            m_LocalTime = LOCAL_REFERENCE_TIME;
            m_LocalTimeOffset = new TimeSpan();

            m_EZSP.StackStatusUpdated += m_StackStatusHandler;
            m_EZSP.MessageReceived += m_MessageReceivedHandler;
            m_EZSP.MessageSent += m_MessageSentHandler;

            if (m_EZSP.IsConnected)
            {
                // Check to make sure the protocol version is valid
                m_EZSP.Version(EZSP_PROTOCOL_VERSION, out ProtocolVersion, out StackType, out StackVersion);

                if (ProtocolVersion == EZSP_PROTOCOL_VERSION)
                {
                    m_IsConnected = true;

                    SetEzspConfigurationValues();

                    // Get the MAC Address
                    m_EZSP.GetEUI64(out m_MACAddress);
                    m_EZSP.GetNodeID(out m_NodeID);

                    AddEndpoints();

                    m_EZSP.NetworkInit(out Status);

                    m_EZSP.SetManufacturerCode(ITRON_MFG_CODE);

                    m_EZSP.LeaveNetwork(out Status);

                    ClearKeyTables();

                    m_EZSP.PermitJoining(0xFF, out Status);

                    if (m_PowerDescriptor != DEFAULT_POWER_DESCRIPTOR)
                    {
                        m_EZSP.SetPowerDescriptor(m_PowerDescriptor);
                    }
                }
                else
                {
                    Disconnect();
                    throw new InvalidOperationException("EZSP Protocol Version Mismatch. Cannot connect to this radio");
                }
            }
            else
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Disconnects from the radio
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void Disconnect()
        {
            m_EZSP.StackStatusUpdated -= m_StackStatusHandler;
            m_EZSP.MessageReceived -= m_MessageReceivedHandler;
            m_EZSP.MessageSent -= m_MessageSentHandler;

            m_IsConnected = false;
            m_IsJoined = false;

            if (m_EZSP.IsConnected)
            {
                m_EZSP.Disconnect();
                m_EZSP = null;
            }

            if (m_ASH.Connected)
            {
                m_ASH.Disconnect();
                m_ASH = null;
            }

            if (m_Comm.IsOpen)
            {
                m_Comm.ClosePort();
                m_Comm = null;
            }
        }

        /// <summary>
        /// Scans the network for available devices
        /// </summary>
        /// <param name="channels">The channels to scan</param>
        /// <param name="duration">The duration of the scan</param>
        /// <returns>The list of devices found</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public List<ZigbeeNetwork> ScanForDevices(ZigBeeChannels channels, EZSPScanDuration duration)
        {
            List<ZigbeeNetwork> NetworksFound = null;
            EmberStatus Status;
            DateTime ScanStartTime = DateTime.Now;

            if (IsConnected)
            {
                m_ScanComplete = false;
                m_EZSP.ScanCompleted += m_ScanCompleteHandler;

                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Scanning for Devices:");

                m_EZSP.StartScan(EZSPScanType.ActiveScan, channels, duration, out Status);

                if (Status == EmberStatus.Success)
                {
                    // Scan until complete or our maximum time limit is reached
                    while (m_ScanComplete == false && (DateTime.Now - ScanStartTime) < MAX_SCAN_TIME)
                    {
                        Thread.Sleep(50);
                    }

                    if (m_ScanComplete == false)
                    {
                        // We hit our time limit so stop the scan
                        m_EZSP.StopScan(out Status);
                    }

                    NetworksFound = m_EZSP.ActiveScanResults;
                }

                m_EZSP.ScanCompleted -= m_ScanCompleteHandler;
            }
            else
            {
                throw new InvalidOperationException("Cannot scan for devices while disconnected from the radio");
            }

            return NetworksFound;
        }

        /// <summary>
        /// Joins the specified network
        /// </summary>
        /// <param name="nodeType">The type of device to join as</param>
        /// <param name="extendedPanID">The extended PAN ID of the network</param>
        /// <param name="channel">The channel the network is on</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public virtual void Join(EmberNodeType nodeType, ulong extendedPanID, byte channel)
        {
            EmberStatus SecurityStatus;
            EmberStatus JoinStatus;

            if (nodeType == EmberNodeType.Coordinator || nodeType == EmberNodeType.UnknownDevice)
            {
                throw new ArgumentException("A join may not be performed by a Coordinator or an Unknown Device", "nodeType");
            }

            if (m_IsConnected)
            {
                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Setting Initial Security State");

                // Tell the radio to join using a Preconfigured Key from the Installation Code

                EmberInitialSecurityState InitialSecurity = new EmberInitialSecurityState();
                InitialSecurity.InitialSecurityBitmask = EmberInitialSecurityBitmask.StandardSecurityMode
                    | EmberInitialSecurityBitmask.HavePreconfiguredKey
                    | EmberInitialSecurityBitmask.RequireEncryptedKey;

                InitialSecurity.NetworkKeySequenceNumber = 0;
                InitialSecurity.PreconfiguredKey = m_PreconfiguredLinkKey;

                m_EZSP.SetInitialSecurityState(InitialSecurity, out SecurityStatus);

                if (SecurityStatus == EmberStatus.Success)
                {
                    //m_EZSP.PermitJoining(60, out JoinStatus);

                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Attempting to Join " + extendedPanID.ToString("X16", CultureInfo.InvariantCulture) + " on Channel " + channel.ToString(CultureInfo.InvariantCulture));

                    // Join the meter
                    EmberNetworkParameters NetworkParameters = new EmberNetworkParameters();

                    NetworkParameters.ExtendedPANID = extendedPanID;
                    NetworkParameters.PANID = 0xFFFF;
                    NetworkParameters.RadioChannel = channel;
                    NetworkParameters.RadioTransmitPower = TransmitPower;
                    NetworkParameters.JoinMethod = EmberJoinMethod.MACAssociation;

                    m_JoinFailed = false;

                    m_EZSP.JoinNetwork(nodeType, NetworkParameters, out JoinStatus);

                    if (JoinStatus == EmberStatus.Success)
                    {
                        // Wait for a Network Up status change
                        DateTime JoinStartTime = DateTime.Now;

                        while (m_JoinFailed == false && m_IsJoined == false && (DateTime.Now - JoinStartTime) < JOIN_TIMEOUT)
                        {
                            Thread.Sleep(50);
                        }

                        if (m_IsJoined == true)
                        {
                            // Give it a little bit of time for any callbacks to come through
                            Thread.Sleep(100);

                            // Get the new Node ID
                            m_EZSP.GetNodeID(out m_NodeID);
                            m_EZSP.GetEUI64(out m_LocalEUI);

                            m_EZSP.LookupEUIByNodeID(TRUST_CENTER_NODE_ID, out JoinStatus, out m_TrustCenterEUI);

                            m_EZSP.SetAddressTableRemoteEUI(0, m_TrustCenterEUI, out JoinStatus);
                            m_EZSP.SetAddressTableRemoteNodeID(0, TRUST_CENTER_NODE_ID);

                            // Make sure that we get the values needed for handling fragmentation
                            m_EZSP.GetFragmentationConfiguration();

                            // Perform a device discovery to find the endpoints for all supported clusters
                            DiscoverEndpoints(TRUST_CENTER_NODE_ID);
                        }
                        else if (m_IsJoined == false && m_JoinFailed == false)
                        {
                            throw new TimeoutException("The Network Join Operation timed out");
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Failed to issue the Join Network Command. Result: " + JoinStatus.ToString());
                    }
                }
                else
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Failed to set the initial Security State. Result: " + SecurityStatus.ToString());
                }

            }
        }

        /// <summary>
        /// Rejoins the specified network
        /// </summary>
        /// <param name="nodeType">The type of device to join as</param>
        /// <param name="extendedPanID">The extended PAN ID of the network</param>
        /// <param name="channel">The channel the network is on</param>
        /// <param name="networkKey">network key</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public virtual void Rejoin(EmberNodeType nodeType, ulong extendedPanID, byte channel, byte[] networkKey)
        {
            EmberStatus SecurityStatus;
            EmberStatus JoinStatus;

            if (nodeType == EmberNodeType.Coordinator || nodeType == EmberNodeType.UnknownDevice)
            {
                throw new ArgumentException("A rejoin may not be performed by a Coordinator or an Unknown Device", "nodeType");
            }

            if (m_IsConnected && IsJoined == false)
            {
                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Setting Initial Security State");

                EmberInitialSecurityState InitialSecurity = new EmberInitialSecurityState();
                InitialSecurity.InitialSecurityBitmask = EmberInitialSecurityBitmask.StandardSecurityMode
                    | EmberInitialSecurityBitmask.HaveNetworkKey;

                InitialSecurity.NetworkKeySequenceNumber = networkKey[0];
                InitialSecurity.NetworkKey = networkKey;

                m_EZSP.SetInitialSecurityState(InitialSecurity, out SecurityStatus);

                if (SecurityStatus == EmberStatus.Success)
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Attempting to rejoin " + extendedPanID.ToString("X16", CultureInfo.InvariantCulture) + " on Channel " + channel.ToString(CultureInfo.InvariantCulture));

                    // rejoin the meter
                    EmberNetworkParameters NetworkParameters = new EmberNetworkParameters();

                    NetworkParameters.ExtendedPANID = extendedPanID;
                    NetworkParameters.PANID = 0xFFFF;
                    NetworkParameters.RadioChannel = channel;
                    NetworkParameters.RadioTransmitPower = TransmitPower;
                    NetworkParameters.JoinMethod = EmberJoinMethod.NetworkRejoinHaveNetworkKey;
                    
                    m_JoinFailed = false;                    

                    m_EZSP.JoinNetwork(nodeType, NetworkParameters, out JoinStatus);

                    if (JoinStatus == EmberStatus.Success)
                    {
                        // Wait for a Network Up status change
                        DateTime JoinStartTime = DateTime.Now;

                        while (m_JoinFailed == false && m_IsJoined == false && (DateTime.Now - JoinStartTime) < JOIN_TIMEOUT)
                        {
                            Thread.Sleep(50);
                        }

                        if (m_IsJoined == true)
                        {
                            // Give it a little bit of time for any callbacks to come through
                            Thread.Sleep(100);

                            // Get the new Node ID
                            m_EZSP.GetNodeID(out m_NodeID);
                            m_EZSP.GetEUI64(out m_LocalEUI);

                            m_EZSP.LookupEUIByNodeID(TRUST_CENTER_NODE_ID, out JoinStatus, out m_TrustCenterEUI);

                            m_EZSP.SetAddressTableRemoteEUI(0, m_TrustCenterEUI, out JoinStatus);
                            m_EZSP.SetAddressTableRemoteNodeID(0, TRUST_CENTER_NODE_ID);

                            /*
                            EmberStatus ReplaceStatus;
                            ulong OldEUI;
                            ushort OldNodeID;
                            bool OldExtendedTimeout;

                            m_EZSP.ReplaceAddressTableEntry(0, m_TrustCenterEUI, TRUST_CENTER_NODE_ID, true, out ReplaceStatus, out OldEUI, out OldNodeID, out OldExtendedTimeout);

                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Replacing Address Table Entry...");
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Status: " + ReplaceStatus.ToString());
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Index: 0");
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "New EUI: " + m_TrustCenterEUI.ToString());
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "New Node: " + TRUST_CENTER_NODE_ID.ToString());
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Old EUI: " + OldEUI.ToString());
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Old Node: " + OldNodeID.ToString());
                            */

                            // Make sure that we get the values needed for handling fragmentation
                            m_EZSP.GetFragmentationConfiguration();

                            // Perform a device discovery to find the endpoints for all supported clusters
                            DiscoverEndpoints(TRUST_CENTER_NODE_ID);
                        }
                        else if (m_IsJoined == false && m_JoinFailed == false)
                        {
                            throw new TimeoutException("The Network Join Operation timed out");
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Failed to issue the Join Network Command. Result: " + JoinStatus.ToString());
                    }
                }
                else
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Failed to set the initial Security State. Result: " + SecurityStatus.ToString());
                }

            }
        }

        /// <summary>
        /// Leaves the currently joined network
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void LeaveNetwork()
        {
            if (IsJoined)
            {
                EmberStatus Status;

                m_EZSP.LeaveNetwork(out Status);
                Thread.Sleep(1000);
                m_IsJoined = false;
            }
        }

        /// <summary>
        /// Reads the specified attributes from the meter
        /// </summary>
        /// <param name="useSecurity">Whether or not the request should be made using link key security</param>
        /// <param name="profileID">The Profile ID of the attributes to read</param>
        /// <param name="clusterID">The Cluster ID of the attributes to read</param>
        /// <param name="attributes">The list of attributes to read</param>
        /// <returns>The responses for each attribute that was read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        public List<ZigBeeAttributeResponse> ReadAttributes(bool useSecurity, ushort profileID, ushort clusterID, List<ushort> attributes)
        {
            List<ZigBeeAttributeResponse> AttributeResponses = new List<ZigBeeAttributeResponse>();
            ZigBeeEndpointInfo EndpointInfo = null;
            byte Endpoint = 0;
            byte[] ResponseData = null;

            if(m_IsJoined)
            {
                // First make sure that the cluster is supported
              if(m_Endpoints.Where(e => e.ProfileID == profileID && e.ClientClusterList.Contains(clusterID)).Count() > 0)
              {

                    EndpointInfo = m_Endpoints.First(e => e.ProfileID == profileID && e.ClientClusterList.Contains(clusterID));
                    Endpoint = EndpointInfo.FindMatchingClientEndpoint(TRUST_CENTER_NODE_ID, clusterID);

                    if(Endpoint != 0)
                    {
                        ResponseData = ReadAttributesRequest(useSecurity, profileID, clusterID, TRUST_CENTER_NODE_ID, Endpoint, attributes);

                        if(ResponseData != null)
                        {
                            MemoryStream DataStream = new MemoryStream(ResponseData);
                            BinaryReader DataReader = new BinaryReader(DataStream);

                            for(int Index = 0; Index < attributes.Count; Index++)
                            {
                                ZigBeeAttributeResponse NewResponse = new ZigBeeAttributeResponse();

                                NewResponse.AttributeID = DataReader.ReadUInt16();
                                NewResponse.Status = (ZCLStatus)DataReader.ReadByte();

                                // The data type is only included if successful
                                if(NewResponse.Status == ZCLStatus.Success)
                                {
                                    NewResponse.DataType = (ZCLDataType)DataReader.ReadByte();

                                    NewResponse.Value = ParseDataType(DataReader, NewResponse.DataType);
                                }

                                AttributeResponses.Add(NewResponse);
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("The corresponding endpoint could not be found on the Trust Center");
                    }
                }
                else
                {
                    throw new InvalidOperationException("This ZigBee device does not support the cluster " + clusterID.ToString("X4", CultureInfo.InvariantCulture));
                }
            }
            else
            {
                throw new InvalidOperationException("This operation may only be performed while Joined with a meter");
            }

            return AttributeResponses;
        }

        /// <summary>
        /// Gets the Basic Cluster Attributes, from the Trust Center -- meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/27/12 DC  ?.??.00        Created 
        //
        public void GetBasicClusterAttributesFromMeter(bool useSecurity)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            Attributes.Add((ushort)ZCLAttributes.ZCLVersion);
            Attributes.Add((ushort)ZCLAttributes.ApplicationVersion);
            Attributes.Add((ushort)ZCLAttributes.StackVersion);
            Attributes.Add((ushort)ZCLAttributes.HWVersion);
            Attributes.Add((ushort)ZCLAttributes.ManufacturerName);
            Attributes.Add((ushort)ZCLAttributes.ModelIdentifier);
            Attributes.Add((ushort)ZCLAttributes.DateCode);
            Attributes.Add((ushort)ZCLAttributes.PowerSource);
            Attributes.Add((ushort)ZCLAttributes.LocationDescription);
            Attributes.Add((ushort)ZCLAttributes.PhysicalEnvironment);
            Attributes.Add((ushort)ZCLAttributes.DeviceEnabled);
            Attributes.Add((ushort)ZCLAttributes.AlarmMask);

            if (m_IsJoined)
            {
                AttributeData = ReadAttributes(
                    useSecurity,
                    (ushort)ZigBeeProfileIDs.SmartEnergy,
                    (ushort)GeneralClusters.Basic,
                    Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)ZCLAttributes.ZCLVersion:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MeterZCLVersion = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ZCLVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;

                            case (ushort)ZCLAttributes.ApplicationVersion:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MeterApplicationVersion = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ApplicationVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;

                            case (ushort)ZCLAttributes.StackVersion:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MeterStackVersion = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading StackVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.HWVersion:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MeterHWVersion = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading HWVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.ManufacturerName:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.CharacterString)
                                    {
                                        m_MeterManufacturerName = new String((char[])CurrentAttribute.Value);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ManufacturerName: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.ModelIdentifier:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.CharacterString)
                                    {
                                        m_MeterModelIdentifier = new String((char[])CurrentAttribute.Value);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ModelIdentifier: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.DateCode:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.CharacterString)
                                    {
                                        m_MeterDateCode = new String((char[])CurrentAttribute.Value);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DateCode: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.PowerSource:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Enum8)
                                    {
                                        m_MeterPowerSource = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading PowerSource: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.LocationDescription:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.CharacterString)
                                    {
                                        m_MeterLocationDescription = new String((char[])CurrentAttribute.Value);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading LocationDescription: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.PhysicalEnvironment:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Enum8)
                                    {
                                        m_MeterPhysicalEnvironment = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading PhysicalEnvironment: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.DeviceEnabled:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Boolean)
                                    {
                                        m_MeterDeviceEnabled = (bool)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DeviceEnabled: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.AlarmMask:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                    {
                                        m_MeterAlarmMask = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading AlarmMask: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Attribute Response for Attribute " + CurrentAttribute.AttributeID.ToString("X4", CultureInfo.InvariantCulture) + ": " + CurrentAttribute.Status.ToString());
                    }
                }
            }

        }

        /// <summary>
        /// Gets the Basic Cluster Attributes, from the Trust Center -- meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------     
        //  12/16/13 MP                 Created. Added to the method above a parameter to choose which attribute to get. Not a great way to do it,
        //                              but by getting all of them we are getting half of the attributes in a response, then the other
        //                              half in a fragmented message.
        //
        public void GetBasicClusterAttributesFromMeter(bool useSecurity, ZCLAttributes desiredAttribute)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            Attributes.Add((ushort)desiredAttribute);

            // add the cluster to the supported cluster list if it isn't there.
            ushort prev = m_Endpoints[0].ClientClusterList.ElementAt(0);
            m_Endpoints[0].ClientClusterList.RemoveAt(0);
            m_Endpoints[0].ClientClusterList.Insert(0, (ushort)GeneralClusters.Basic);

            if (m_IsJoined)
            {
                AttributeData = ReadAttributes(
                    useSecurity,
                    (ushort)ZigBeeProfileIDs.SmartEnergy,
                    (ushort)GeneralClusters.Basic,
                    Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)ZCLAttributes.ZCLVersion:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MeterZCLVersion = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ZCLVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;

                            case (ushort)ZCLAttributes.ApplicationVersion:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MeterApplicationVersion = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ApplicationVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;

                            case (ushort)ZCLAttributes.StackVersion:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MeterStackVersion = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading StackVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.HWVersion:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MeterHWVersion = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading HWVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.ManufacturerName:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.CharacterString)
                                    {
                                        m_MeterManufacturerName = new String((char[])CurrentAttribute.Value);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ManufacturerName: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.ModelIdentifier:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.CharacterString)
                                    {
                                        m_MeterModelIdentifier = new String((char[])CurrentAttribute.Value);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ModelIdentifier: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.DateCode:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.CharacterString)
                                    {
                                        m_MeterDateCode = new String((char[])CurrentAttribute.Value);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DateCode: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.PowerSource:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Enum8)
                                    {
                                        m_MeterPowerSource = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading PowerSource: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.LocationDescription:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.CharacterString)
                                    {
                                        m_MeterLocationDescription = new String((char[])CurrentAttribute.Value);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading LocationDescription: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.PhysicalEnvironment:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Enum8)
                                    {
                                        m_MeterPhysicalEnvironment = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading PhysicalEnvironment: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.DeviceEnabled:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Boolean)
                                    {
                                        m_MeterDeviceEnabled = (bool)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DeviceEnabled: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                            case (ushort)ZCLAttributes.AlarmMask:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                    {
                                        m_MeterAlarmMask = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading AlarmMask: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Attribute Response for Attribute " + CurrentAttribute.AttributeID.ToString("X4", CultureInfo.InvariantCulture) + ": " + CurrentAttribute.Status.ToString());
                    }
                }
            }

            // Remove the cluster from the list. Should removed the endpoint at the first index.
            m_Endpoints[0].ClientClusterList.RemoveAt(0);
            m_Endpoints[0].ClientClusterList.Insert(0, prev);

        }

        /// <summary>
        /// Gets the Identify Cluster Attributes, from the Trust Center -- meter. Testing only.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/14/13 MP  ?.??.00        Created        
        //
        public void GetIdentifyClusterAttributesFromMeter(bool useSecurity)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            Attributes.Add((ushort)ZCLIdentifyAttribute.IdentifyTime); // Identify Time. It's the only attribute in the cluster, so yeah.

            // Add the cluster to the list of supported clusters. For some reason this isn't done automatically, if it is a problem
            // please change it, but for the sake of testing im going to leave it here.
            ushort prev = m_Endpoints[0].ClientClusterList.ElementAt(0);
            m_Endpoints[0].ClientClusterList.RemoveAt(0);
            m_Endpoints[0].ClientClusterList.Insert(0, (ushort)GeneralClusters.Identify);           

            // Set an initial value for identify time. Don't want to set to 0 because that is the default. I kind of
            // want to know if it changes at all, so I'm going with an odd ball.
            m_IdentifyTime = 0xAAAA;

            if (m_IsJoined)
            {
                AttributeData = ReadAttributes(
                    useSecurity,
                    (ushort)ZigBeeProfileIDs.SmartEnergy,
                    (ushort)GeneralClusters.Identify,
                    Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)ZCLIdentifyAttribute.IdentifyTime: // once again, it's the only attribute in the cluster.
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint16)
                                    {
                                        m_IdentifyTime = (ushort)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading ZCLVersion: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }
                                }

                                break;                         
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Attribute Response for Attribute " + CurrentAttribute.AttributeID.ToString("X4", CultureInfo.InvariantCulture) + ": " + CurrentAttribute.Status.ToString());
                    }
                }
            }

            // Remove the cluster from the list. Should removed the endpoint at the first index.
            m_Endpoints[0].ClientClusterList.RemoveAt(0);
            m_Endpoints[0].ClientClusterList.Insert(0, prev);

        }
     
        /// <summary>
        /// Gets the Trust Center's device time
        /// </summary>
        /// <returns>The Trust Center's current device time</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetTime()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)ZCLTimeAttributes.Time);
            Attributes.Add((ushort)ZCLTimeAttributes.TimeStatus);
            Attributes.Add((ushort)ZCLTimeAttributes.TimeZone);
            Attributes.Add((ushort)ZCLTimeAttributes.DstStart);
            Attributes.Add((ushort)ZCLTimeAttributes.DstEnd);
            Attributes.Add((ushort)ZCLTimeAttributes.DstShift);
            Attributes.Add((ushort)ZCLTimeAttributes.StandardTime);
            Attributes.Add((ushort)ZCLTimeAttributes.LocalTime);

            // Typically we would reset the values here but we really shouldn't for time because some features
            // rely on the time being set to something for them to work properly

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Time Attributes");

            if (IsJoined)
            {
                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)GeneralClusters.Time, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)ZCLTimeAttributes.Time:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                {
                                    m_LastRetrievedUTCTime = (DateTime)CurrentAttribute.Value;
                                    m_UTCTimeOffset = m_LastRetrievedUTCTime - DateTime.UtcNow;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading UTC Time: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)ZCLTimeAttributes.TimeStatus:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                {
                                    m_TimeStatus = (ZCLTimeStatus)(byte)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Time Status: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)ZCLTimeAttributes.TimeZone:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Int32)
                                {
                                    m_TimeZone = (int)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Time Zone: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)ZCLTimeAttributes.DstStart:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint Seconds = (uint)CurrentAttribute.Value;

                                    m_DSTStart = LOCAL_REFERENCE_TIME.AddSeconds(Seconds);
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DST Start Time: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)ZCLTimeAttributes.DstEnd:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint Seconds = (uint)CurrentAttribute.Value;

                                    m_DSTEnd = LOCAL_REFERENCE_TIME.AddSeconds(Seconds);
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DST End Time: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)ZCLTimeAttributes.DstShift:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Int32)
                                {
                                    m_DSTShift = (int)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DST Shift: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)ZCLTimeAttributes.StandardTime:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint Seconds = (uint)CurrentAttribute.Value;

                                    m_StandardTime = LOCAL_REFERENCE_TIME.AddSeconds(Seconds);
                                    m_StandardTimeOffset = m_StandardTime - DateTime.Now;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Standard Time: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)ZCLTimeAttributes.LocalTime:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint Seconds = (uint)CurrentAttribute.Value;

                                    m_LocalTime = LOCAL_REFERENCE_TIME.AddSeconds(Seconds);
                                    m_LocalTimeOffset = m_LocalTime - DateTime.Now;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Local Time: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Attribute Response for Attribute " + CurrentAttribute.AttributeID.ToString("X4", CultureInfo.InvariantCulture) + ": " + CurrentAttribute.Status.ToString());
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("This operation may only be performed while Joined with a meter");
            }
        }

        /// <summary>
        /// Gets the Trust Center's device time with security option
        /// </summary>
        /// <returns>The Trust Center's current device time</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/22/13 MP  2.52.00        Created, copied from method above, but wanted option of using security or not
        //  12/18/13 MP                 added parameter to choose which attribute to get.

        public void GetTime(bool useSecurity, ZCLTimeAttributes desirdeAttribute)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)desirdeAttribute);

            // Typically we would reset the values here but we really shouldn't for time because some features
            // rely on the time being set to something for them to work properly

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Time Attributes");

            if (IsJoined)
            {
                AttributeData = ReadAttributes(useSecurity, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)GeneralClusters.Time, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)ZCLTimeAttributes.Time:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_LastRetrievedUTCTime = (DateTime)CurrentAttribute.Value;
                                        m_UTCTimeOffset = m_LastRetrievedUTCTime - DateTime.UtcNow;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading UTC Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)ZCLTimeAttributes.TimeStatus:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                    {
                                        m_TimeStatus = (ZCLTimeStatus)(byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Time Status: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)ZCLTimeAttributes.TimeZone:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Int32)
                                    {
                                        m_TimeZone = (int)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Time Zone: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)ZCLTimeAttributes.DstStart:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                    {
                                        uint Seconds = (uint)CurrentAttribute.Value;

                                        m_DSTStart = LOCAL_REFERENCE_TIME.AddSeconds(Seconds);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DST Start Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)ZCLTimeAttributes.DstEnd:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                    {
                                        uint Seconds = (uint)CurrentAttribute.Value;

                                        m_DSTEnd = LOCAL_REFERENCE_TIME.AddSeconds(Seconds);
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DST End Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)ZCLTimeAttributes.DstShift:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Int32)
                                    {
                                        m_DSTShift = (int)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading DST Shift: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)ZCLTimeAttributes.StandardTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                    {
                                        uint Seconds = (uint)CurrentAttribute.Value;

                                        m_StandardTime = LOCAL_REFERENCE_TIME.AddSeconds(Seconds);
                                        m_StandardTimeOffset = m_StandardTime - DateTime.Now;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Standard Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)ZCLTimeAttributes.LocalTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                    {
                                        uint Seconds = (uint)CurrentAttribute.Value;

                                        m_LocalTime = LOCAL_REFERENCE_TIME.AddSeconds(Seconds);
                                        m_LocalTimeOffset = m_LocalTime - DateTime.Now;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Local Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                        }
                    }
                    else
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Attribute Response for Attribute " + CurrentAttribute.AttributeID.ToString("X4", CultureInfo.InvariantCulture) + ": " + CurrentAttribute.Status.ToString());
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("This operation may only be performed while Joined with a meter");
            }
        }

        /// <summary>
        /// Send a ZDO Leave Request to specified device
        /// </summary>
        /// <param name="useEncryption">Optionally Enable encryption for this command</param>
        /// <param name="remoteEUI">EUI64 of Device</param>
        /// <param name="destinationNodeID">Short address of destination</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/26/12 DEC ?.??.00        Created
        //
        public ZDOStatus SendLeaveRequest(bool useEncryption, ushort destinationNodeID, ulong remoteEUI)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            byte[] Message = new byte[10];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZDOStatus RequestSuccessful = ZDOStatus.Success;
            byte Sequence = m_ZDOSequence++;
            IncomingMessage ReceivedMessage;

            // Set up the APS Frame
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.ZigBeeDeviceObject;
            ApsFrame.DestinationEndpoint = 0x00;
            ApsFrame.SourceEndpoint = 0x00;
            ApsFrame.ClusterID = (ushort)ZDOClusters.LeaveRequest;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            if (useEncryption)
            {
                ApsFrame.Options |= EmberApsOptions.Encryption;
            }

            // Create the Message
            remoteEUI = 0;
            MessageWriter.Write(Sequence);
            MessageWriter.Write(remoteEUI);
            MessageWriter.Write((Byte)0xc0);      // RemoveChildren=0, Rejoin=0

            SendUnicastMessage(destinationNodeID, ApsFrame, Message);

            ReceivedMessage = WaitForZDOMessage(Sequence);

            if (ReceivedMessage != null)
            {
                RequestSuccessful = ParseLeaveRequestResponse(ReceivedMessage);

                // Remove the message from the list of received items
                m_ReceivedZDOMessages.Remove(ReceivedMessage);
            }

            return RequestSuccessful;
        }

        /// <summary>
        /// Print the Link Key Table data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/05/16 PGH 4.70.21        Created

        public void PrintKeyTable()
        {
            EzspStatus Status;
            EmberStatus KeyStatus;
            EmberKeyStruct Key;
            List<EmberKeyStruct> Keys = new List<EmberKeyStruct>();
            List<byte> Statuses = new List<byte>();
            ushort KeyCount;

            m_EZSP.GetConfigurationValue(EzspConfigID.KeyTableSize, out Status, out KeyCount);

            if (Status == EzspStatus.Success)
            {

                for (byte Index = 0; Index < KeyCount; Index++)
                {
                    m_EZSP.GetKeyTableEntry(Index, out KeyStatus, out Key);
                    Keys.Add(Key);
                    Statuses.Add((byte)KeyStatus);
                }
            }

            int Count = 0;
            if (Keys.Count > 0)
            {
                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Printing Key Table...");
                for (int i = 0; i < Keys.Count; i++)
                {
                    EmberKeyStruct MyKey = Keys[i];
                    byte MyStatus = Statuses[i];
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Key at Index " + Count.ToString() + ":");
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Status: " + String.Format("{0:X}", MyStatus) + " (" + ((EmberStatus)MyStatus).ToString() + ")");
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Key: " + BitConverter.ToString(MyKey.Key));
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Key Type: " + String.Format("{0:X}", MyKey.KeyType) + " (" + MyKey.KeyType.ToString() + ")");
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Partner EUI: " + MyKey.PartnerEUI.ToString());
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "");
                    Count++;
                }
            }
        }

        /// <summary>
        /// Print the Address Table data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/05/16 PGH 4.70.21        Created

        public void PrintAddressTable()
        {
            EzspStatus Status;
            ulong Address;
            List<ulong> Addresses = new List<ulong>();
            List<byte> Statuses = new List<byte>();
            ushort AddressCount;

            m_EZSP.GetConfigurationValue(EzspConfigID.AddressTableSize, out Status, out AddressCount);

            if (Status == EzspStatus.Success)
            {

                for (byte Index = 0; Index < AddressCount; Index++)
                {
                    m_EZSP.GetAddressTableRemoteEUI(Index, out Address);
                    Addresses.Add(Address);
                }
            }

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Printing Address Table...");
            for (int i = 0; i < Addresses.Count; i++)
            {
                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Address at Index " + i.ToString() + ": " + Addresses[i].ToString());
            }
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the ZigBee Application is currently connected to the ZigBee Radio
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool IsConnected
        {
            get
            {
                return m_IsConnected;
            }
        }

        /// <summary>
        /// Gets whether or not the ZigBee Application is currently joined to a ZigBee Network
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool IsJoined
        {
            get
            {
                return m_IsJoined;
            }
            protected set
            {
                m_IsJoined = value;
            }
        }

        /// <summary>
        /// Gets the Installation Code that will be used to join the network
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] InstallationCode
        {
            get
            {
                return m_InstallationCode;
            }
        }

        /// <summary>
        /// Gets the Installation Code as a string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public string InstallationCodeString
        {
            get
            {
                string InstallCode = "";

                foreach (byte CurrentByte in InstallationCode)
                {
                    InstallCode += CurrentByte.ToString("X2");
                }

                return InstallCode;
            }
        }

        /// <summary>
        /// Gets the MAC Address for the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ulong MACAddress
        {
            get
            {
                return m_MACAddress;
            }
        }

        /// <summary>
        /// Gets the Node ID for the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort NodeID
        {
            get
            {
                return m_NodeID;
            }
        }

        /// <summary>
        /// Gets the last retrieved UTC Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime LastRetrievedUTCTime
        {
            get
            {
                return m_LastRetrievedUTCTime;
            }
        }

        /// <summary>
        /// Gets the estimated current device UTC time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime CurrentUTCTime
        {
            get
            {
                return DateTime.UtcNow + m_UTCTimeOffset;
            }
        }

        /// <summary>
        /// Gets the last retrieved Standard Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime LastRetrievedStandardTime
        {
            get
            {
                return m_StandardTime;
            }
        }

        /// <summary>
        /// Gets the estimated current device Standard time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime CurrentStandardTime
        {
            get
            {
                return DateTime.Now + m_StandardTimeOffset;
            }
        }

        /// <summary>
        /// Gets the last retrieved Local Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime LastRetrievedLocalTime
        {
            get
            {
                return m_LocalTime;
            }
        }

        /// <summary>
        /// Gets the estimated current device Local time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime CurrentLocalTime
        {
            get
            {
                return DateTime.Now + m_LocalTimeOffset;
            }
        }

        /// <summary>
        /// Gets the DST start date and time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime DSTStart
        {
            get
            {
                return m_DSTStart;
            }
        }

        /// <summary>
        /// Gets the DST end  date and time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime DSTEnd
        {
            get
            {
                return m_DSTEnd;
            }
        }

        /// <summary>
        /// Gets the DST Shift in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public int DSTShift
        {
            get
            {
                return m_DSTShift;
            }
        }

        /// <summary>
        /// Gets the time zone offset in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public int TimeZone
        {
            get
            {
                return m_TimeZone;
            }
        }

        /// <summary>
        /// Gets the Time Status information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ZCLTimeStatus TimeStatus
        {
            get
            {
                return m_TimeStatus;
            }
        }

        /// <summary>
        /// Gets or set the Transmit Power that will be used when Joining and Rejoining the device. The value is in dBm.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/09/11 RCG 2.53.04        Created
        
        public sbyte TransmitPower
        {
            get
            {
                return m_TransmitPower;
            }
            set
            {
                if (value >= -32 && value <= 3)
                {
                    m_TransmitPower = value;

                    // If the meter is not joined to the meter the transmit power will be set when we join the meter
                    // If it is joined we need to adjust the transmit power by calling the EZSP command.
                    if (IsJoined)
                    {
                        EmberStatus Status;
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Adjusting Transmit Power to " + value.ToString(CultureInfo.InvariantCulture) + " dBm");
                        m_EZSP.SetRadioPower(value, out Status);

                        if (Status != EmberStatus.Success)
                        {
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Transmit Power Adjustment failed: " + Status.ToString());
                        }
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "The Transmit Power must be a value between -32 and 3");
                }
            }
        }

        /// <summary>
        /// String representing ZCL Basic Cluster Attribute
        /// </summary>
        public String ZCLManufacturerName
        {
            get
            {
                return m_ZCLManufacturerName;
            }
            set
            {
                m_ZCLManufacturerName = value;
            }
        }

        /// <summary>
        /// String representing ZCL Basic Cluster Attribute
        /// </summary>
        public String ZCLModelIdentifier
        {
            get
            {
                return m_ZCLModelIdentifier;
            }
            set
            {
                m_ZCLModelIdentifier = value;
            }
        }

        /// <summary>
        /// String representing ZCL Basic Cluster Attribute
        /// </summary>
        public String ZCLDateCode
        {
            get
            {
                return m_ZCLDateCode;
            }
            set
            {
                m_ZCLDateCode = value;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public byte MeterZCLVersion
        {
            get
            {
                return m_MeterZCLVersion;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public byte MeterApplicationVersion
        {
            get
            {
                return m_MeterApplicationVersion;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public byte MeterStackVersion
        {
            get
            {
                return m_MeterStackVersion;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public byte MeterHWVersion
        {
            get
            {
                return m_MeterHWVersion;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public String MeterManufacturerName
        {
            get
            {
                return m_MeterManufacturerName;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public String MeterModelIdentifier
        {
            get
            {
                return m_MeterModelIdentifier;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public String MeterDateCode
        {
            get
            {
                return m_MeterDateCode;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public byte MeterPowerSource
        {
            get
            {
                return m_MeterPowerSource;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public string MeterLocationDescription
        {
            get
            {
                try
                {
                    return m_MeterLocationDescription;
                }
                catch (NullReferenceException e)
                {
                    return "Null Reference to Location Description. Attribute Not supported. Exception: " + e.ToString();
                }
            }
            set
            {
                m_MeterLocationDescription = value;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public byte MeterPhysicalEnvironment
        {
            get
            {
                try
                {
                    return m_MeterPhysicalEnvironment;
                }
                catch (NullReferenceException)
                {
                    return 0x00;
                }
            }
            set
            {
                m_MeterPhysicalEnvironment = value;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public bool MeterDeviceEnabled
        {
            get
            {
                try
                {
                    return m_MeterDeviceEnabled;
                }
                catch (NullReferenceException)
                {
                    return true;
                }
            }
            set
            {
                m_MeterDeviceEnabled = value;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public byte MeterAlarmMask
        {
            get
            {
                try
                {
                    return m_MeterAlarmMask;
                }
                catch (NullReferenceException)
                {
                    return 0x00;
                }
            }
            set
            {
                m_MeterAlarmMask = value;
            }
        }

        /// <summary>
        /// ZCL Basic Cluster Attribute (read from Meter)
        /// </summary>
        public UInt16 IdentifyTime
        {
            get
            {
                return m_IdentifyTime;
            }
            set
            {
                m_IdentifyTime = value;
            }
        }

        /// <summary>
        /// Gets/Sets the OTA Download status
        /// </summary>
        public byte OTADownLoadStatus
        {
            get
            {
                return m_OTADownLoadStatus;
            }
            set
            {
                m_OTADownLoadStatus = value;
            }
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected ZigBeeApplication()
        {
            m_IsConnected = false;
            m_IsJoined = false;

            m_Comm = null;
            m_ASH = null;
            m_EZSP = null;

            m_Endpoints = new List<ZigBeeEndpointInfo>();
            m_SentMessageTags = new List<byte>();

            SetUpClusterLists();

            m_Logger = new EZSPLogger();

            m_ScanCompleteHandler = new EventHandler(m_EZSP_ScanCompleted);
            m_StackStatusHandler = new StackStatusUpdatedHandler(m_EZSP_StackStatusUpdated);
            m_MessageReceivedHandler = new MessageReceivedHandler(m_EZSP_MessageReceived);
            m_MessageSentHandler = new MessageSentHandler(m_EZSP_MessageSent);

            m_CBKEHandler = new CBKEKeyGeneratedHandler(m_EZSP_CBKEKeyGenerated);
            m_SMACSHandler = new SmacsCalculatedHandler(m_EZSP_SmacsCalculated);

            m_UnhandledMessages = new List<IncomingMessage>();
            m_ReceivedZDOMessages = new List<IncomingMessage>();
            m_ZCLResponseMessages = new List<IncomingMessage>();

            // Generate a new Installation Code and Preconfigured Link Key
            //m_InstallationCode = InstallCodeHelper.GenerateInstallCode(InstallCodeHelper.InstallCodeSize.Size128Bits);
            //m_InstallationCode = new byte[] { 0xD9, 0x33, 0x70, 0xFA, 0x85, 0xCA, 0x5F, 0x23, 0x33, 0xD8, 0xC2, 0xB9, 0xE0, 0xFE, 0xAC, 0xE3, 0x65, 0xD6 };
            m_InstallationCode = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x0E, 0xB8 };
            m_PreconfiguredLinkKey = InstallCodeHelper.GenerateLinkKey(m_InstallationCode);

            m_MACAddress = 0;
            m_TransmitPower = 3;

            // Set some initial values to the attributes
            m_MeterZCLVersion = 0;
            m_MeterApplicationVersion = 0;
            m_MeterStackVersion = 0;
            m_MeterHWVersion = 0;
            m_MeterPowerSource = 0;
            m_MeterLocationDescription = "";
            m_MeterPhysicalEnvironment = 1;
            m_MeterDeviceEnabled = false;
            m_MeterAlarmMask = 0;

            // Set defaults for my ZCL attribute strings
            m_ZCLManufacturerName = "Itron";
            m_ZCLModelIdentifier = "Telegesis Dongle";
            m_ZCLDateCode = "YYYYMMDD HHMM";

            // Set defaults for meter ZCL attribute strings
            m_MeterManufacturerName = "";
            m_MeterModelIdentifier = "";
            m_MeterDateCode = "";

            m_PowerDescriptor = DEFAULT_POWER_DESCRIPTOR;
        }

        /// <summary>
        /// Sets up the list of Clusters that will be supported by this device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected virtual void SetUpClusterLists()
        {
            m_Endpoints.Clear();
        }

        /// <summary>
        /// Parses the specified data type from the Binary Reader
        /// </summary>
        /// <param name="dataReader">The Binary Reader containing the data to read</param>
        /// <param name="dataType">The data type to read</param>
        /// <returns>The parsed data</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        protected object ParseDataType(BinaryReader dataReader, ZCLDataType dataType)
        {
            object ParsedValue = null;

            switch(dataType)
            {
                case ZCLDataType.Boolean:
                {
                    ParsedValue = dataReader.ReadBoolean();
                    break;
                }
                case ZCLDataType.Data8:
                case ZCLDataType.Enum8:
                case ZCLDataType.Bitmap8:
                case ZCLDataType.Uint8:
                {
                    ParsedValue = dataReader.ReadByte();
                    break;
                }
                case ZCLDataType.Data16:
                case ZCLDataType.Enum16:
                case ZCLDataType.Bitmap16:
                case ZCLDataType.Uint16:
                case ZCLDataType.ClusterID:
                case ZCLDataType.AttributeID:
                {
                    ParsedValue = dataReader.ReadUInt16();
                    break;
                }
                case ZCLDataType.Data24:
                case ZCLDataType.Bitmap24:
                case ZCLDataType.Uint24:
                {
                    byte[] Data = dataReader.ReadBytes(3);
                    uint Value = (uint)(Data[2] << 16 | Data[1] << 8 | Data[0]);

                    ParsedValue = Value;

                    break;
                }
                case ZCLDataType.Data32:
                case ZCLDataType.Bitmap32:
                case ZCLDataType.Uint32:
                case ZCLDataType.BACnetOID:
                {
                    ParsedValue = dataReader.ReadUInt32();
                    break;
                }
                case ZCLDataType.Data40 :
                case ZCLDataType.Bitmap40:
                case ZCLDataType.Uint40:
                {
                    byte[] Data = dataReader.ReadBytes(5);
                    ulong Value = (ulong)(Data[4] << 32 | Data[3] << 24 | Data[2] << 16 | Data[1] << 8 | Data[0]);

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Data48:
                case ZCLDataType.Bitmap48:
                case ZCLDataType.Uint48:
                {
                    byte[] Data = dataReader.ReadBytes(6);
                    ulong Value = (ulong)(Data[5] << 40 | Data[4] << 32 | Data[3] << 24 | Data[2] << 16 | Data[1] << 8 | Data[0]);

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Data56:
                case ZCLDataType.Bitmap56:
                case ZCLDataType.Uint56:
                {
                    byte[] Data = dataReader.ReadBytes(7);
                    ulong Value = (ulong)(Data[6] << 48 | Data[5] << 40 | Data[4] << 32 | Data[3] << 24 | Data[2] << 16 | Data[1] << 8 | Data[0]);
                                        
                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Data64:
                case ZCLDataType.Bitmap64:
                case ZCLDataType.Uint64:
                case ZCLDataType.IEEEAddress:
                {
                    ParsedValue = dataReader.ReadUInt64();
                    break;
                }
                case ZCLDataType.Int8:
                {
                    ParsedValue = dataReader.ReadSByte();
                    break;
                }
                case ZCLDataType.Int16:
                {
                    ParsedValue = dataReader.ReadInt16();
                    break;
                }
                case ZCLDataType.Int24:
                {
                    byte[] Data = dataReader.ReadBytes(3);
                    int Value = (int)(Data[2] << 16 | Data[1] << 8 | Data[0]);

                    if((Data[2] & 0x80) == 0x80)
                    {
                        // It's a negative value so we need to set the highest order bit to 0xFF
                        Value |= (int)(0xFF << 24);
                    }

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Int32:
                {
                    ParsedValue = dataReader.ReadInt32();
                    break;
                }
                case ZCLDataType.Int40:
                {
                    byte[] Data = dataReader.ReadBytes(5);
                    long Value = (long)(Data[4] << 32 | Data[3] << 24 | Data[2] << 16 | Data[1] << 8 | Data[0]);

                    if((Data[4] & 0x80) == 0x80)
                    {
                        // It's a negative value so we need to set the highest order bit to 0xFF
                        Value |= (long)(0xFFFFFF << 40);
                    }

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Int48:
                {
                    byte[] Data = dataReader.ReadBytes(6);
                    long Value = (long)(Data[5] << 40 | Data[4] << 32 | Data[3] << 24 | Data[2] << 16 | Data[1] << 8 | Data[0]);

                    if((Data[5] & 0x80) == 0x80)
                    {
                        // It's a negative value so we need to set the highest order bit to 0xFF
                        Value |= (long)(0xFFFF << 48);
                    }

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Int56:
                {
                    byte[] Data = dataReader.ReadBytes(7);
                    long Value = (long)(Data[6] << 48 | Data[5] << 40 | Data[4] << 32 | Data[3] << 24 | Data[2] << 16 | Data[1] << 8 | Data[0]);

                    if((Data[6] & 0x80) == 0x80)
                    {
                        // It's a negative value so we need to set the highest order bit to 0xFF
                        Value |= (long)(0xFF << 56);
                    }

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Int64:
                {
                    ParsedValue = dataReader.ReadInt64();
                    break;
                }
                case ZCLDataType.Float16:
                {
                    // There is no data type for reading half precision floats so we have to figure it out ourselves
                    // Doing the math this way may not get us the exact value we would see using a real Float16 but it should be very close.
                    // Float16 breaks down to: 1 sign bit, 5 exponent bits, and 10 mantissa bits.

                    ushort RawValue = dataReader.ReadUInt16();
                    bool IsNegative = (0x8000 & RawValue) == 0x8000; 
                    ushort Exponent = (ushort)((0x7C00 & RawValue) >> 10);
                    ushort Mantissa = (ushort)(0x03FF & RawValue);                                            
                    float Value = 0;
                                            
                    if(Exponent == 0x001F)
                    {
                        // This is a special case for infinite and NaN values
                        if(Mantissa != 0)
                        {
                            Value = float.NaN;
                        }
                        else if(IsNegative)
                        {
                            Value = float.NegativeInfinity;
                        }
                        else
                        {
                            Value = float.PositiveInfinity;
                        }

                    }
                    else if(Exponent == 0x0000)
                    {
                        // Exponent 0 is a special case where the equation should be  sign * 2^-14 * 0.Mantissa
                        Value = (float)(Math.Pow(2, -24) * Mantissa); 
                    }
                    else
                    {
                        // Anything else the equation is sign * 2^(Exponent - 15) * 1.Mantissa
                        Value = (float)(Math.Pow(2, Exponent - 15) * (1.0 + Mantissa * Math.Pow(2, -10)));
                    }

                    if(IsNegative)
                    {
                        Value += (float)-1.0;
                    }

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Float32:
                {
                    ParsedValue = dataReader.ReadSingle();
                    break;
                }
                case ZCLDataType.Float64:
                {
                    ParsedValue = dataReader.ReadDouble();
                    break;
                }
                case ZCLDataType.OctetString:
                {
                    byte Length = dataReader.ReadByte();
                    byte[] Value = dataReader.ReadBytes(Length);

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.CharacterString:
                {
                    byte Length = dataReader.ReadByte();
                    char[] Value = dataReader.ReadChars(Length);

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.LongOctetString:
                {
                    ushort Length = dataReader.ReadUInt16();
                    byte[] Value = dataReader.ReadBytes(Length);

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.LongCharacterString:
                {
                    ushort Length = dataReader.ReadUInt16();
                    char[] Value = dataReader.ReadChars(Length);

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Array:
                case ZCLDataType.Structure:
                case ZCLDataType.Set:
                case ZCLDataType.Bag:
                {
                    // TODO: Define how to read these since they may vary based on what attribute has been requested.
                    throw new NotImplementedException("Can't currently support reading Array, Structure, Set, or Bag data types");
                }
                case ZCLDataType.TimeOfDay:
                {
                    byte Hours = dataReader.ReadByte();
                    byte Minutes = dataReader.ReadByte();
                    byte Seconds = dataReader.ReadByte();
                    byte Hundredths = dataReader.ReadByte();

                    TimeSpan Value = new TimeSpan(0, Hours, Minutes, Seconds, Hundredths * 10);
                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.Date:
                {
                    byte Year = dataReader.ReadByte();
                    byte Month = dataReader.ReadByte();
                    byte DayOfMonth = dataReader.ReadByte();
                    byte DayOfWeek = dataReader.ReadByte();

                    DateTime Value = new DateTime(1900 + Year, Month, DayOfMonth);
                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.UTCTime:
                {
                    uint Seconds = dataReader.ReadUInt32();
                    DateTime Value = UTC_REFERENCE_TIME.AddSeconds(Seconds);

                    ParsedValue = Value;
                    break;
                }
                case ZCLDataType.SecurityKey:
                {
                    byte[] Value = dataReader.ReadBytes(16);
                    ParsedValue = Value;
                    break;
                }
            }

            return ParsedValue;
        }

        /// <summary>
        /// Sends a Unicast Message to the specified Node
        /// </summary>
        /// <param name="destinationNodeID">The destination Node ID</param>
        /// <param name="apsFrame">The APS Frame that describes the message</param>
        /// <param name="message">The message contents</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        protected void SendUnicastMessage(ushort destinationNodeID, EmberApsFrame apsFrame, byte[] message)
        {
            DateTime StartTime;
            EmberStatus Status = EmberStatus.FatalError;
            byte Sequence = 0;
            List<byte[]> MessageFragments = FragmentMessage(message);

            if (MessageFragments.Count > 1)
            {
                // We need to send a fragmented message so set the appropriate APS Frame flag
                apsFrame.Options |= EmberApsOptions.Fragment;
                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Message requires fragmentation. Message Length: " + message.Length.ToString(CultureInfo.InvariantCulture) 
                    + " Fragments: " + MessageFragments.Count.ToString(CultureInfo.InvariantCulture));
            }

            foreach (byte[] CurrentMessageData in MessageFragments)
            {
                byte MessageTag = m_CurrentMessageTag;

                m_CurrentMessageTag++;

                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Unicast Message to " + 
                    destinationNodeID.ToString("X4", CultureInfo.InvariantCulture) + " with Message Tag " + 
                    MessageTag.ToString("X2", CultureInfo.InvariantCulture) + " Message Contents: " + 
                    EZSPLogger.ConvertDataToString(CurrentMessageData));

                string logMsg = "ClusterID: " + apsFrame.ClusterID.ToString("X") + " ";
                logMsg += "DestinationEndpoint: " + apsFrame.DestinationEndpoint.ToString("X") + " ";
                logMsg += "SourceEndpoint: " + apsFrame.SourceEndpoint.ToString("X") + " ";
                logMsg += "GroupID: " + apsFrame.GroupID.ToString("X") + " ";
                logMsg += "ProfileID: " + apsFrame.ProfileID.ToString("X") + " ";
                logMsg += "Sequence: " + apsFrame.Sequence.ToString();

                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, logMsg);

                // Add the tag before we send the message because sometimes we will get the Message sent response before it makes it to the list
                // if we do this after sending the message.
                m_SentMessageTags.Add(MessageTag);

                m_EZSP.SendUnicast(EmberOutgoingMessageType.Direct, destinationNodeID, apsFrame, MessageTag, (byte)CurrentMessageData.Length, CurrentMessageData, out Status, out Sequence);

                //m_EZSP.SendUnicast(EmberOutgoingMessageType.ViaAddressTable, destinationNodeID, apsFrame, MessageTag, (byte)CurrentMessageData.Length, CurrentMessageData, out Status, out Sequence);

                if (Status == EmberStatus.Success)
                {
                    StartTime = DateTime.Now;

                    while (m_SentMessageTags.Contains(MessageTag) && (DateTime.Now - StartTime) <= MESSAGE_SEND_TIMEOUT)
                    {
                        Thread.Sleep(25);
                    }

                    if (m_SentMessageTags.Contains(MessageTag))
                    {
                        // We didn't get the message sent confirmation
                        m_SentMessageTags.Remove(MessageTag);
                        Console.WriteLine("Did not receive the Message Sent Confirmation within the allowed time");
                        // throw new TimeoutException("Did not receive the Message Sent Confirmation within the allowed time");
                    }
                }
                else
                {
                    // The message was not sent so make sure we remove the tag
                    m_SentMessageTags.Remove(MessageTag);

                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Send Unicast Message Failed: " + Status.ToString());
                    // throw new Exception("Failed to send the unicast message. Reason: " + Status.ToString());
                    Console.WriteLine("Failed to send the unicast message. Reason: " + Status.ToString());
                }
            }
        }

        /// <summary>
        /// Handles incoming ZDO message
        /// </summary>
        /// <param name="ReceivedMessage">The incoming message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected virtual void HandleZDOMessage(IncomingMessage ReceivedMessage)
        {
            // Make sure we remove any messages that already exist with the same sequence number
            m_ReceivedZDOMessages.RemoveAll(m => m.MessageContents[0] == ReceivedMessage.MessageContents[0]);

            m_ReceivedZDOMessages.Add(ReceivedMessage);
        }

        /// <summary>
        /// Handles the Received Messages
        /// </summary>
        /// <param name="receivedMessage">The message that was received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  11/01/12 PGH 2.70.36        Added handle for IPP

        protected void HandleReceivedMessage(IncomingMessage receivedMessage)
        {
            switch (receivedMessage.APSFrame.ProfileID)
            {
                case (ushort)ZigBeeProfileIDs.ZigBeeDeviceObject:
                {
                    HandleZDOMessage(receivedMessage);
                    break;
                }
                case (ushort)ZigBeeProfileIDs.ItronPrivateProfile:
                {
                    HandleItronPrivateProfileMessage(receivedMessage);
                    break;
                }
                default:
                {
                    try
                    {
                        // ZCL message
                        ZCLFrame ZclMessage = new ZCLFrame();
                        ZclMessage.FrameData = receivedMessage.MessageContents;

                        if (ZclMessage.FrameType == ZCLFrameType.EntireProfile)
                        {
                            HandleGeneralZCLMessage(receivedMessage);
                        }
                        else
                        {                         
                            // Make sure that we support the endpoint that has been requested
                            if (IsClusterSupported(receivedMessage.APSFrame.ProfileID, receivedMessage.APSFrame.ClusterID, receivedMessage.APSFrame.DestinationEndpoint))
                            {
                                HandleZCLMessage(receivedMessage);
                            }
                            else
                            {
                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Endpoint " + receivedMessage.APSFrame.DestinationEndpoint.ToString("X2", CultureInfo.InvariantCulture)
                                    + " does not handle messages for Profile ID " + receivedMessage.APSFrame.ProfileID.ToString("X4", CultureInfo.InvariantCulture) + " and Cluster ID "
                                    + receivedMessage.APSFrame.ClusterID.ToString("X4", CultureInfo.InvariantCulture) + ". The message was ignored.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Exception occurred while receiving a ZCL Message: " + e.Message);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Handles ZigBee Cluster Library Messages
        /// </summary>
        /// <param name="receivedMessage">The message that was received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected virtual void HandleZCLMessage(IncomingMessage receivedMessage)
        {
            switch(receivedMessage.APSFrame.ProfileID)
            {
                default:
                {
                    m_UnhandledMessages.Add(receivedMessage);
                    break;
                }
            }
        }

        /// <summary>
        /// Handles general ZCL Command messages
        /// </summary>
        /// <param name="receivedMessage">The message that was received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        protected void HandleGeneralZCLMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;

            if (ZclFrame.FrameType == ZCLFrameType.EntireProfile)
            {
                switch(ZclFrame.CommandID)
                {
                    case (byte)GeneralZCLCommandIDs.DefaultResponse:
                    {
                        byte CommandID = ZclFrame.Data[0];
                        ZCLStatus Status = (ZCLStatus)ZclFrame.Data[1];

                        // Default Response Received
                        OnDefaultResponseReceived(Status);

                        // For now just print the status to the log
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Default Response received for command " + CommandID.ToString("X2", CultureInfo.InvariantCulture) + ". Status: " + Status.ToString());
                        // TODO: Figure out how to handle default responses 
                        break;
                    }
                    case (byte)GeneralZCLCommandIDs.ReadAttributes:
                    {
                        RespondToReadAttributeRequest(receivedMessage);
                        break;
                    }
                    case (byte)GeneralZCLCommandIDs.ReadAttributesResponse:
                    {
                        HandleBasicClusterMessage(receivedMessage);
                        // We received a response to a command that has been issued so we should let the caller handle the response
                        lock (m_ZCLResponseMessages)
                        {
                            m_ZCLResponseMessages.Insert(0, receivedMessage);
                        }
                        break;
                    }
                    case (byte)GeneralZCLCommandIDs.ReportAttributes:
                    {
                        HandleReportedAttributes(receivedMessage);
                        break;
                    }
                    case (byte)GeneralZCLCommandIDs.WriteAttributesResponse:
                    {
                        HandleBasicClusterMessage(receivedMessage);
                        // We received a response to a command that has been issued so we should let the caller handle the response
                        lock (m_ZCLResponseMessages)
                        {
                            m_ZCLResponseMessages.Insert(0, receivedMessage);
                        }
                        break;
                    }
                    default:
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unhandled General ZCL message received: " + ZclFrame.CommandID.ToString());
                        m_UnhandledMessages.Add(receivedMessage);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles incoming ZigBee C177 (IPP) Profile Messages
        /// </summary>
        /// <param name="receivedMessage">The incoming message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/01/12 PGH 2.70.36        Created

        protected virtual void HandleItronPrivateProfileMessage(IncomingMessage receivedMessage)
        {
            switch (receivedMessage.APSFrame.ClusterID)
            {
                case (ushort)ItronClusters.DATA_RESPONSE:
                {
                    // Try to avoid cases where we could handle Data Response Messages out of order if they are sent very close together
                    lock (m_IPPDataRequestLocker)
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received an IPP data response message.");
                        OnIPPDataResponseReceived(receivedMessage);
                    }
                    break;
                }
                default:
                {
                    m_UnhandledMessages.Add(receivedMessage);
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received an unsupported message. Cluster ID: " + receivedMessage.APSFrame.ClusterID.ToString("X2", CultureInfo.InvariantCulture));
                    break;
                }
            }
        }

        /// <summary>
        /// Handles Report Attributes messages
        /// </summary>
        /// <param name="receivedMessage">The Report Attributes message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/11 RCG 2.52.28        Created
        
        protected virtual void HandleReportedAttributes(IncomingMessage receivedMessage)
        {
            // Currently there are no generic attributes that should be reported
            m_UnhandledMessages.Add(receivedMessage);
        }

        /// <summary>
        /// Responds to a Read Attribute Request
        /// </summary>
        /// <param name="receivedMessage">The Read Attribute Request</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        protected void RespondToReadAttributeRequest(IncomingMessage receivedMessage)
        {
            int ResponseLength = 0;
            List<ushort> RequestedAttributes = new List<ushort>();
            List<byte[]> AttributeResponses = new List<byte[]>();
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            byte[] ResponseData = new byte[0];

            MemoryStream MessageStream = new MemoryStream(ZclFrame.Data);
            BinaryReader MessageReader = new BinaryReader(MessageStream);

            // The read attribute message contains a list of ushort attributes to read
            for (int Index = 0; Index < (ZclFrame.Data.Count() / 2); Index++)
            {
                RequestedAttributes.Add(MessageReader.ReadUInt16());
            }

            // Build up the responses for each attribute
            foreach (ushort CurrentAttribute in RequestedAttributes)
            {
                byte[] AttributeResponse = GetAttributeResponse(receivedMessage.APSFrame.ClusterID, CurrentAttribute);

                ResponseLength += AttributeResponse.Length;
                AttributeResponses.Add(AttributeResponse);
            }

            // Build the Response
            ResponseData = new byte[ResponseLength];
            MemoryStream ResponseStream = new MemoryStream(ResponseData);
            BinaryWriter ResponseWriter = new BinaryWriter(ResponseStream);

            foreach (byte[] CurrentAttributeData in AttributeResponses)
            {
                ResponseWriter.Write(CurrentAttributeData);
            }

            // Finally send the response
            SendReadAttributeResponse(receivedMessage.APSFrame.ProfileID, receivedMessage.APSFrame.ClusterID, receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, ZclFrame.SequenceNumber, ResponseData);
        }

        /// <summary>
        /// Gets the response value for the specified attribute
        /// </summary>
        /// <param name="clusterID">The cluster ID of the attributes to get</param>
        /// <param name="attribute">The attribute ID</param>
        /// <returns>The byte array containing the data to include in the response for the specified attribute</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        protected virtual byte[] GetAttributeResponse(ushort clusterID, ushort attribute)
        {
            byte[] AttributeResponse = null;
            MemoryStream ResponseStream = null;
            BinaryWriter ResponseWriter = null;

            bool Supported = true;

            if (clusterID == (ushort)GeneralClusters.Basic)
            {
                switch (attribute)
                {
                    case (ushort)ZCLAttributes.ZCLVersion:
                        {
                            AttributeResponse = new byte[5];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);
                            ResponseWriter.Write((byte)ZCLDataType.Uint8);
                            ResponseWriter.Write(ZCL_VERSION);

                            break;
                        }
                    case (ushort)ZCLAttributes.ApplicationVersion:
                        {
                            AttributeResponse = new byte[5];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);
                            ResponseWriter.Write((byte)ZCLDataType.Uint8);
                            ResponseWriter.Write((byte)3);

                            break;
                        }
                    case (ushort)ZCLAttributes.StackVersion:
                        {
                            AttributeResponse = new byte[5];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);
                            ResponseWriter.Write((byte)ZCLDataType.Uint8);
                            ResponseWriter.Write((byte)2);

                            break;
                        }
                    case (ushort)ZCLAttributes.HWVersion:
                        {
                            AttributeResponse = new byte[5];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);
                            ResponseWriter.Write((byte)ZCLDataType.Uint8);
                            ResponseWriter.Write((byte)1);

                            break;
                        }

                    case (ushort)ZCLAttributes.ManufacturerName:
                        {
                            AttributeResponse = new byte[4 + 1 + m_ZCLManufacturerName.Length];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);
                            ResponseWriter.Write((byte)ZCLDataType.CharacterString);
                            ResponseWriter.Write(m_ZCLManufacturerName);
                        }
                        break;

                    case (ushort)ZCLAttributes.ModelIdentifier:
                        {
                            AttributeResponse = new byte[4 + 1 + m_ZCLModelIdentifier.Length];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);
                            ResponseWriter.Write((byte)ZCLDataType.CharacterString);
                            ResponseWriter.Write(m_ZCLModelIdentifier);
                        }
                        break;

                    case (ushort)ZCLAttributes.DateCode:
                        {
                            AttributeResponse = new byte[4 + 1 + m_ZCLDateCode.Length];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);
                            ResponseWriter.Write((byte)ZCLDataType.CharacterString);
                            ResponseWriter.Write(m_ZCLDateCode);
                        }
                        break;

                    case (ushort)ZCLAttributes.PowerSource:
                        {
                            AttributeResponse = new byte[5];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);
                            ResponseWriter.Write((byte)ZCLDataType.Enum8);
                            ResponseWriter.Write((byte)ZCLPowerSource.SinglePhase);

                            break;
                        }
                    case (ushort)ZCLAttributes.DeviceEnabled:
                        {
                            AttributeResponse = new byte[5];
                            ResponseStream = new MemoryStream(AttributeResponse);
                            ResponseWriter = new BinaryWriter(ResponseStream);

                            ResponseWriter.Write(attribute);
                            ResponseWriter.Write((byte)ZCLStatus.Success);

                            // We are going to assume the device is always enabled
                            ResponseWriter.Write((byte)ZCLDataType.Boolean);
                            ResponseWriter.Write((byte)0x01); // Enabled;

                            break;
                        }
                    default:
                        {
                            Supported = false;
                            break;
                        }
                }
            }
            else
            {
                Supported = false;
            }

            // Handle any attributes that are not supported
            if (Supported == false)
            {
                AttributeResponse = new byte[3];
                ResponseStream = new MemoryStream(AttributeResponse);
                ResponseWriter = new BinaryWriter(ResponseStream);

                ResponseWriter.Write(attribute);
                ResponseWriter.Write((byte)ZCLStatus.UnsupportedAttribute);
            }

            return AttributeResponse;
        }

        /// <summary>
        /// Sends the Read Attribute Response
        /// </summary>
        /// <param name="profileID">The Profile ID that the message is sent from</param>
        /// <param name="clusterID">The Cluster ID that the message is sent from</param>
        /// <param name="destination">The destination Node ID</param>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="sequenceNumber">The sequence number used for the response</param>
        /// <param name="responseData">The response data for the Read Attribute</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  09/01/16 PGH 4.70.16        Added encryption option to APS frame
        
        protected void SendReadAttributeResponse(ushort profileID, ushort clusterID, ushort destination, byte endpoint, byte sequenceNumber, byte[] responseData)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = profileID;
            ApsFrame.DestinationEndpoint = endpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == profileID).Endpoint;
            ApsFrame.ClusterID = clusterID;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the ZCL Frame

            ZclFrame.FrameType = ZCLFrameType.EntireProfile;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = false;
            ZclFrame.SequenceNumber = sequenceNumber;
            ZclFrame.CommandID = (byte)GeneralZCLCommandIDs.ReadAttributesResponse;
            ZclFrame.Data = responseData;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Sends the default response message with the specified status
        /// </summary>
        /// <param name="profileID">The Profile ID that the message is sent from</param>
        /// <param name="clusterID">The Cluster ID that the message is sent from</param>
        /// <param name="destination">The destination Node ID</param>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="status">The status to send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void SendDefaultResponse(ushort profileID, ushort clusterID, ushort destination, byte endpoint, ZCLStatus status)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] Message = new byte[1];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = profileID;
            ApsFrame.DestinationEndpoint = endpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == profileID).Endpoint;
            ApsFrame.ClusterID = clusterID;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Create the message
            MessageWriter.Write((byte)status);

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.EntireProfile;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)GeneralZCLCommandIDs.DefaultResponse;
            ZclFrame.Data = Message;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Sends the request to the specified Node to bind to a cluster
        /// </summary>
        /// <param name="localEndpoint">The local endpoint requesting the bind</param>
        /// <param name="clusterID">The Cluster ID to bind to</param>
        /// <param name="destinationNodeID">The Node ID of the destination used to send the message</param>
        /// <param name="remoteEUI">The EUI of the destination</param>
        /// <param name="remoteEndpoint">The endpoint to bind to on the destination</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        protected bool RequestBind(byte localEndpoint, ushort clusterID, ushort destinationNodeID, ulong remoteEUI, byte remoteEndpoint)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            byte[] Message = new byte[22];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            bool BindingSuccessful = false;
            byte Sequence = m_ZDOSequence++;
            IncomingMessage ReceivedMessage;

            // Set up the APS Frame
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.ZigBeeDeviceObject;
            ApsFrame.DestinationEndpoint = 0x00;
            ApsFrame.SourceEndpoint = 0x00;
            ApsFrame.ClusterID = (ushort)ZDOClusters.BindRequest;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Create the Message
            MessageWriter.Write(Sequence);
            MessageWriter.Write(remoteEUI);
            MessageWriter.Write(remoteEndpoint);
            MessageWriter.Write(clusterID);
            MessageWriter.Write(EUI_DEST_ADDRESS_MODE);
            MessageWriter.Write(m_LocalEUI);
            MessageWriter.Write(localEndpoint);

            SendUnicastMessage(destinationNodeID, ApsFrame, Message);

            ReceivedMessage = WaitForZDOMessage(Sequence);

            if (ReceivedMessage != null)
            {
                BindingSuccessful = ParseBindResponse(ReceivedMessage);

                // Remove the message from the list of received items
                m_ReceivedZDOMessages.Remove(ReceivedMessage);
            }

            return BindingSuccessful;
        }

        /// <summary>
        /// Binds to the specified cluster on the specified device
        /// </summary>
        /// <param name="destination">The Node ID of the device to bind to</param>
        /// <param name="clusterID">The cluster ID to bind to</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void Bind(ushort destination, ushort clusterID)
        {
            EmberStatus Status;
            EmberBindingTableEntry NewEntry = new EmberBindingTableEntry();
            ZigBeeEndpointInfo LocalEndpointInfo;
            byte RemoteEndpoint;
            ulong DestinationEUI;
            byte BindingIndex = byte.MaxValue;

            // Find the endpoint information for the cluster ID
            if (m_Endpoints.Where(e => e.ClientClusterList.Contains(clusterID)).Count() > 0)
            {
                LocalEndpointInfo = m_Endpoints.First(e => e.ClientClusterList.Contains(clusterID));
            }
            else
            {
                throw new ArgumentException("This cluster is not supported", "clusterID");
            }

            // Find the remote endpoint
            RemoteEndpoint = LocalEndpointInfo.FindMatchingClientEndpoint(destination, clusterID);

            if (RemoteEndpoint != 0)
            {
                m_EZSP.LookupEUIByNodeID(destination, out Status, out DestinationEUI);
                
                // Check to see if we have an available binding table entry
                for(byte Index = 0; Index < DEFAULT_BINDING_TABLE_SIZE; Index++)
                {
                    EmberBindingTableEntry CurrentEntry;
                    bool BindingActive;

                    m_EZSP.BindingIsActive(Index, out BindingActive);

                    if(BindingActive == false)
                    {
                        // We found an empty entry so let's save that but continue looking in case we already have an entry
                        BindingIndex = Index;
                    }
                    else
                    {
                        m_EZSP.GetBinding(Index, out Status, out CurrentEntry);

                        if(CurrentEntry != null && CurrentEntry.ClusterID == clusterID && CurrentEntry.EUI == DestinationEUI)
                        {
                            // We found an in use entry
                            BindingIndex = Index;
                            break;
                        }
                    }
                }

                if (BindingIndex != byte.MaxValue)
                {
                    // Bind to the device
                    if (RequestBind(LocalEndpointInfo.Endpoint, clusterID, destination, DestinationEUI, RemoteEndpoint))
                    {
                        // Create the new entry so that we can save the binding to the binding table
                        NewEntry.BindingType = EmberBindingTableEntry.EmberBindingType.UnicastBinding;
                        NewEntry.ClusterID = clusterID;
                        NewEntry.EUI = DestinationEUI;
                        NewEntry.LocalEndpoint = LocalEndpointInfo.Endpoint;
                        NewEntry.RemoteEndpoint = RemoteEndpoint;

                        m_EZSP.SetBinding(BindingIndex, NewEntry, out Status);
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to bind to the requested device");
                    }
                }
                else
                {
                    throw new InvalidOperationException("No Binding Table Entries are available to be set");
                }
            }
            else
            {
                throw new InvalidOperationException("No mapping could be found for the requested cluster on the remote endpoint");
            }
        }

        /// <summary>
        /// Waits for the ZDO Message with the specified sequence number
        /// </summary>
        /// <param name="zdoSequence">The ZDO sequence number to receive</param>
        /// <returns>The ZDO Message</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  07/24/15 jrf 4.20.18 553650 Removing exception when ZDO message with matching sequence number 
        //                              is not found. This caused meters with older ZigBee stack to fail to logon.  
        protected IncomingMessage WaitForZDOMessage(byte zdoSequence)
        {
            DateTime StartTime = DateTime.Now;
            IncomingMessage FoundMessage = null;

            while (m_ReceivedZDOMessages.Where(m => m.MessageContents[0] == zdoSequence).Count() == 0
                && DateTime.Now - StartTime < MESSAGE_RESPONSE_TIMEOUT)
            {
                Thread.Sleep(25);
            }

            if (m_ReceivedZDOMessages.Where(m => m.MessageContents[0] == zdoSequence).Count() > 0)
            {
                FoundMessage = m_ReceivedZDOMessages.First(m => m.MessageContents[0] == zdoSequence);
            }

            return FoundMessage;
        }

        /// <summary>
        /// Reads the specified attributes
        /// </summary>
        /// <param name="useSecurity">Whether or not to use security to send the message</param>
        /// <param name="profileID">The profile ID to use in the request</param>
        /// <param name="clusterID">The cluster ID of the attributes to read</param>
        /// <param name="destination">The Node ID of the destination</param>
        /// <param name="endpoint">The destination's endpoint</param>
        /// <param name="attributes">The list of attributes to read.</param>
        /// <returns>The raw attribute response data from the destination</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        protected byte[] ReadAttributesRequest(bool useSecurity, ushort profileID, ushort clusterID, ushort destination, byte endpoint, List<ushort> attributes)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[2 * attributes.Count];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            IncomingMessage Response = null;
            byte[] ResponseData = null;

            foreach (ushort CurrentAttribute in attributes)
            {
                MessageWriter.Write(CurrentAttribute);
            }

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = profileID;
            ApsFrame.DestinationEndpoint = endpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == profileID).Endpoint;
            ApsFrame.ClusterID = clusterID;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            if (useSecurity)
            {
                ApsFrame.Options |= EmberApsOptions.Encryption;
                
            }

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.EntireProfile;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)GeneralZCLCommandIDs.ReadAttributes;
            ZclFrame.Data = MessageData;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);

            if (Response != null)
            {
                ZCLFrame ResponseZcl = new ZCLFrame();
                ResponseZcl.FrameData = Response.MessageContents;

                ResponseData = ResponseZcl.Data;
            }

            return ResponseData;
        }

        /// <summary>
        /// Configures attribute reporting
        /// </summary>
        /// <param name="profileID">The profile ID of the attributes to configure</param>
        /// <param name="clusterID">The cluster ID of the attributes to configure</param>
        /// <param name="destination">The Node ID of the destination device</param>
        /// <param name="attributes">The list of attributes to configure</param>
        /// <param name="timeout">The timeout to use</param>
        /// <returns>The response data</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/19/11 RCG 2.52.28        Created
        
        protected byte[] ConfigureAttributeReporting(ushort profileID, ushort clusterID, ushort destination, List<ushort> attributes, ushort timeout)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[5 * attributes.Count];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            IncomingMessage Response = null;
            byte[] ResponseData = null;
            ZigBeeEndpointInfo EndpointInfo = m_Endpoints.First(e => e.ProfileID == profileID);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = profileID;
            ApsFrame.DestinationEndpoint = EndpointInfo.FindMatchingClientEndpoint(destination, clusterID);
            ApsFrame.SourceEndpoint = EndpointInfo.Endpoint;
            ApsFrame.ClusterID = clusterID;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Generate the message
            foreach (ushort CurrentAttribute in attributes)
            {
                // When the timeout is specified we will always be receiving the data
                MessageWriter.Write((byte)ReportingDirection.Receive);
                MessageWriter.Write(CurrentAttribute);
                MessageWriter.Write(timeout);
            }

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.EntireProfile;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)GeneralZCLCommandIDs.ConfigureReporting;
            ZclFrame.Data = MessageData;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            // Most of the time we probably won't care what the response is but lets return the data in case someone wants it
            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);

            if (Response != null)
            {
                ZCLFrame ResponseZcl = new ZCLFrame();
                ResponseZcl.FrameData = Response.MessageContents;

                ResponseData = ResponseZcl.Data;
            }

            return ResponseData;
        }

        /// <summary>
        /// Waits for the response to the ZCL command with the specified sequence number
        /// </summary>
        /// <param name="sequenceNumber">The sequence number </param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        protected IncomingMessage WaitForZCLResponse(byte sequenceNumber)
        {
            DateTime StartTime = DateTime.Now;
            IncomingMessage FoundMessage = null;

            while (FoundMessage == null
                && DateTime.Now - StartTime < MESSAGE_RESPONSE_TIMEOUT)
            {
                Thread.Sleep(25);

                lock (m_ZCLResponseMessages)
                {
                    foreach (IncomingMessage CurrentMessage in m_ZCLResponseMessages)
                    {
                        ZCLFrame Zcl = new ZCLFrame();
                        Zcl.FrameData = CurrentMessage.MessageContents;

                        if (Zcl.SequenceNumber == sequenceNumber)
                        {
                            FoundMessage = CurrentMessage;
                            break;
                        }
                    }
                }
            }

            
           if (FoundMessage == null)
            {
                // throw new TimeoutException("The ZCL Response with sequence number " + sequenceNumber.ToString(CultureInfo.InvariantCulture) + " was not received in the allowed time.");
                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "The ZCL Response with sequence number " + sequenceNumber.ToString(CultureInfo.InvariantCulture) + " was not received in the allowed time.");
            }
            
            return FoundMessage;
        }

        /// <summary>
        /// Add or update key table entry
        /// </summary>
        /// <param name="eui">eui</param>
        /// <param name="isLinkKey">isLinkKey</param>
        /// <param name="key">key</param>
        /// <returns>status of the command</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 PGH 2.70.36  Created
        protected EmberStatus AddOrUpdateKeyTableEntry(ulong eui, bool isLinkKey, byte[] key)
        {
            EmberStatus status;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Add Or Update KeyTable Entry:");

            m_EZSP.AddOrUpdateKeyTableEntry(eui, isLinkKey, key, out status);

            return (status);
        }

        #endregion

        #region Key Establishment

        /// <summary>
        /// Perform the Key Establishment Process
        /// </summary>
        /// <param name="destination">The destination node ID</param>
        /// <param name="causeError">Whether or not an error should be caused during CBKE</param>
        /// <param name="errorState">The point in the key establishment process the error should occur.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void PerformKeyEstablishment(ushort destination, bool causeError, KeyEstablishmentState errorState)
        {
            DateTime StartTime;
            TimeSpan Timeout;
            EmberStatus Status = EmberStatus.FatalError;
            ulong PartnerEUI;
            m_KeyEstablishmentInitiator = true;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Beginning Key Establishment Process");

            // We should have discovered where we can do Key Establishment
            if (m_Endpoints.Where(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy).Count() > 0)
            {
                ZigBeeEndpointInfo SEEndpointInfo = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);
                byte KeyEstablishmentEndpoint = SEEndpointInfo.FindMatchingServerEndpoint(destination, (ushort)SmartEnergyClusters.KeyEstablishment);

                if (KeyEstablishmentEndpoint != 0)
                {
                    // Look up the partner's EUI
                    m_EZSP.LookupEUIByNodeID(destination, out Status, out PartnerEUI);

                    if (Status == EmberStatus.Success)
                    {
                        m_PartnerEUI = PartnerEUI;
                    }
                    else
                    {
                        throw new InvalidOperationException("The partner EUI could not be obtained in order to perform Key Establishment");
                    }

                    // Make sure there is a key table entry available
                    if (CheckKeyTableAvailability(m_PartnerEUI) == false)
                    {
                        throw new InvalidOperationException("No key table entries are currently available in order to perform Key Establishment");
                    }

                    // Get the Certificate from the radio
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Retrieving Device Certificate");
                    m_EZSP.GetCertificate(out Status, out m_Certificate);

                    if (Status != EmberStatus.Success)
                    {
                        throw new InvalidOperationException("Could not obtain the Certificate from the radio in order to perform Key Establishment");
                    }
                    else if (m_Certificate != null && m_Certificate.Length == 48)
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Initiating Key Establishment");

                        StartTime = DateTime.Now;
                        Timeout = new TimeSpan(0, 0, DEFAULT_OPERATION_TIMEOUT);
                        InitiateKeyEstablishment(destination, KeyEstablishmentEndpoint);

                        // We've sent the Initiate Key Establishment now we need to wait for the response
                        while (m_KeyEstablishmentState != KeyEstablishmentState.ReceivedInitiateKeyEstablishment && DateTime.Now - StartTime < Timeout)
                        {
                            Thread.Sleep(25);
                        }

                        if (m_KeyEstablishmentState == KeyEstablishmentState.ReceivedInitiateKeyEstablishment && causeError && errorState == KeyEstablishmentState.ReceivedInitiateKeyEstablishment)
                        {
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Faking an error during CBKE by sending a Terminate Key Establishment Message");

                            // We want to cause CBKE to fail for testing so lets just send a termination message indicating that the cert is bad
                            TerminateKeyEstablishment(destination, KeyEstablishmentEndpoint, KeyEstablishmentErrors.UnknownIssuer);
                            return;
                        }
                        else if (m_KeyEstablishmentState == KeyEstablishmentState.ReceivedInitiateKeyEstablishment)
                        {
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Generating CBKE keys");

                            // We now have their certificate so we need to calculate the CBKE Keys and then send our Ephemeral Data
                            GenerateCBKEKeys();
                            StartTime = DateTime.Now;

                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Ephemeral key");

                            SendEphemeralKey(destination, KeyEstablishmentEndpoint);
                        }
                        else
                        {
                            TerminateKeyEstablishment(destination, KeyEstablishmentEndpoint, KeyEstablishmentErrors.BadMessage);
                            throw new TimeoutException("Did not receive the Key Establishment Response within the allowed time");
                        }

                        // Now we wait for the Partner to send it's ephemeral key
                        Timeout = new TimeSpan(0, 0, m_PartnerEphemeralKeyGenerateTime);

                        while (m_KeyEstablishmentState != KeyEstablishmentState.ReceivedEphemeralKey && DateTime.Now - StartTime < Timeout)
                        {
                            Thread.Sleep(25);
                        }

                        if (m_KeyEstablishmentState == KeyEstablishmentState.ReceivedEphemeralKey && causeError && errorState == KeyEstablishmentState.ReceivedEphemeralKey)
                        {
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Faking an error during CBKE by sending a Terminate Key Establishment Message");

                            // We want to cause CBKE to fail for testing so lets just send a termination message indicating that the cert is bad
                            TerminateKeyEstablishment(destination, KeyEstablishmentEndpoint, KeyEstablishmentErrors.BadMessage);
                            return;
                        }
                        else if (m_KeyEstablishmentState == KeyEstablishmentState.ReceivedEphemeralKey)
                        {
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Calculating SMAC");

                            // We now have their ephemeral key so we need to calculate the SMACS
                            CalculateSMACS();

                            // Then Send the key confirmation
                            StartTime = DateTime.Now;

                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Key Confirmation");

                            ConfirmKey(destination, KeyEstablishmentEndpoint, m_LocalSMAC);
                        }
                        else
                        {
                            TerminateKeyEstablishment(destination, KeyEstablishmentEndpoint, KeyEstablishmentErrors.BadMessage);
                            throw new TimeoutException("Did not receive the Ephemeral Key Response within the allowed time");
                        }

                        // Finally Wait for the key confirmation
                        Timeout = new TimeSpan(0, 0, m_PartnerConfirmKeyTime);

                        while (m_KeyEstablishmentState != KeyEstablishmentState.ReceivedKeyConfirmation && DateTime.Now - StartTime < Timeout)
                        {
                            Thread.Sleep(25);
                        }

                        if (m_KeyEstablishmentState == KeyEstablishmentState.ReceivedKeyConfirmation && causeError && errorState == KeyEstablishmentState.ReceivedKeyConfirmation)
                        {
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Faking an error during CBKE by sending a Terminate Key Establishment Message");

                            // We want to cause CBKE to fail for testing so lets just send a termination message indicating that the cert is bad
                            TerminateKeyEstablishment(destination, KeyEstablishmentEndpoint, KeyEstablishmentErrors.BadKeyConfirm);
                            return;
                        }
                        else if (m_KeyEstablishmentState == KeyEstablishmentState.ReceivedKeyConfirmation)
                        {
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Verifying SMAC from Partner");

                            // We have the key confirmation so verify that it is correct
                            if (VerifySmacs(m_PartnerSMAC, m_ReceivedSMAC))
                            {
                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Key Established");

                                // Successfully established the keys
                                m_KeyEstablishmentState = KeyEstablishmentState.Established;
                                m_EZSP.ClearTemporaryDataMaybeStoreLinkKey(true, out Status);
                            }
                            else
                            {
                                TerminateKeyEstablishment(destination, KeyEstablishmentEndpoint, KeyEstablishmentErrors.BadKeyConfirm);
                            }
                        }
                        else
                        {
                            TerminateKeyEstablishment(destination, KeyEstablishmentEndpoint, KeyEstablishmentErrors.BadMessage);
                            throw new TimeoutException("Did not receive the Confirm Key Response within the allowed time");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("The certificate retrieved is invalid");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Could not perform key establishment because the destination device does not support Key Establishment");
                }
            }
        }

        /// <summary>
        /// Checks to see if there is an available Key Table Entry for a new link key for the specified partner
        /// </summary>
        /// <param name="partnerEUI">The EUI of the partner to check</param>
        /// <returns>True if there is an available key table entry. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected bool CheckKeyTableAvailability(ulong partnerEUI)
        {
            EmberStatus Status = EmberStatus.FatalError;
            EmberKeyStruct TrustCenterKey;
            byte KeyIndex;
            bool AvailableEntry = true;

            m_EZSP.GetKey(EmberKeyType.TrustCenterLinkKey, out Status, out TrustCenterKey);

            // Make sure we have room for a new key
            if (Status != EmberStatus.Success
                || (TrustCenterKey.KeyStructBitmask & EmberKeyStruct.EmberKeyStructBitmask.KeyHasPartnerEUI) != EmberKeyStruct.EmberKeyStructBitmask.KeyHasPartnerEUI
                || m_PartnerEUI != TrustCenterKey.PartnerEUI)
            {
                // We couldn't find a valid Trust Center Key lets see if we already have a link key for the Partner
                m_EZSP.FindKeyTableEntry(m_PartnerEUI, true, out KeyIndex);

                if (KeyIndex == 0xFF)
                {
                    // We didn't find an existing Link Key for the device so lets see if there is an empty key slot
                    m_EZSP.FindKeyTableEntry(0, true, out KeyIndex);

                    if (KeyIndex == 0xFF)
                    {
                        // No Key Slots are available so we can't continue
                        AvailableEntry = false;
                    }
                }
            }

            return AvailableEntry;
        }

        /// <summary>
        /// Initiates the Key Establishment Process.
        /// </summary>
        /// <param name="destination">The destination</param>
        /// <param name="endpoint">The destination endpoint</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void InitiateKeyEstablishment(ushort destination, byte endpoint)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            byte[] Message = new byte[52];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.DestinationEndpoint = endpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID).Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.KeyEstablishment;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Create the message
            MessageWriter.Write(CBKE_ESTABLISHMENT);
            MessageWriter.Write(CBKE_GENERATE_TIME);
            MessageWriter.Write(CBKE_CONFIRM_TIME);
            MessageWriter.Write(m_Certificate);

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;

            if (m_KeyEstablishmentState == KeyEstablishmentState.ReceivedInitiateKeyEstablishment)
            {
                ZclFrame.Direction = ZCLDirection.SentFromServer;
            }
            else
            {
                ZclFrame.Direction = ZCLDirection.SentFromClient;
            }

            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)KeyEstablishmentCommands.InitiateKeyEstablishment;
            ZclFrame.Data = Message;

            m_KeyEstablishmentState = KeyEstablishmentState.SentInitiateKeyEstablishment;
            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Sends the calculated ephemeral key to the specified device
        /// </summary>
        /// <param name="destination">The destination Node ID</param>
        /// <param name="endpoint">The destination endpoint</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void SendEphemeralKey(ushort destination, byte endpoint)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            byte[] Message = new byte[22];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.DestinationEndpoint = endpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID).Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.KeyEstablishment;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Create the message
            MessageWriter.Write(m_LocalEphemeralKey);

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;

            if (m_KeyEstablishmentInitiator)
            {
                ZclFrame.Direction = ZCLDirection.SentFromClient;
            }
            else
            {
                ZclFrame.Direction = ZCLDirection.SentFromServer;
            }

            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)KeyEstablishmentCommands.EphemeralDataRequest;
            ZclFrame.Data = Message;

            m_KeyEstablishmentState = KeyEstablishmentState.SentEphemeralKey;
            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Handles incoming Key Establishment messages
        /// </summary>
        /// <param name="receivedMessage">The message that was received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void HandleKeyEstablishmentMessage(IncomingMessage receivedMessage)
        {
            EmberStatus Status = EmberStatus.FatalError;
            ZCLFrame ZclFrame = new ZCLFrame();

            ZclFrame.FrameData = receivedMessage.MessageContents;

            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);

            if (ZclFrame.FrameType == ZCLFrameType.ClusterSpecific)
            {
                switch (ZclFrame.CommandID)
                {
                    case (byte)KeyEstablishmentCommands.InitiateKeyEstablishment:
                        {
                            ushort KeyEstablishmentSuite = DataReader.ReadUInt16();
                            byte EphemeralDataGenerateTime = DataReader.ReadByte();
                            byte ConfirmKeyGenerateTime = DataReader.ReadByte();
                            byte[] PartnerCert = DataReader.ReadBytes(48);

                            if (m_KeyEstablishmentInitiator)
                            {
                                if (m_KeyEstablishmentState == KeyEstablishmentState.SentInitiateKeyEstablishment)
                                {
                                    m_PartnerEphemeralKeyGenerateTime = EphemeralDataGenerateTime;
                                    m_PartnerConfirmKeyTime = ConfirmKeyGenerateTime;
                                    m_PartnerCertificate = PartnerCert;

                                    m_KeyEstablishmentState = KeyEstablishmentState.ReceivedInitiateKeyEstablishment;
                                }
                                else
                                {
                                    TerminateKeyEstablishment(receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, KeyEstablishmentErrors.BadMessage);
                                }
                            }
                            else
                            {
                                // TODO:
                            }

                            break;
                        }
                    case (byte)KeyEstablishmentCommands.EphemeralDataRequest:
                        {
                            byte[] EphemeralKey = DataReader.ReadBytes(22);

                            if (m_KeyEstablishmentInitiator)
                            {
                                if (m_KeyEstablishmentState == KeyEstablishmentState.SentEphemeralKey)
                                {
                                    m_PartnerEphermeralKey = EphemeralKey;

                                    m_KeyEstablishmentState = KeyEstablishmentState.ReceivedEphemeralKey;
                                }
                                else
                                {
                                    TerminateKeyEstablishment(receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, KeyEstablishmentErrors.BadMessage);
                                }
                            }
                            else
                            {
                                // TODO:
                            }

                            break;
                        }
                    case (byte)KeyEstablishmentCommands.ConfirmKeyDataRequest:
                        {
                            byte[] PartnerSMAC = DataReader.ReadBytes(16);

                            if (m_KeyEstablishmentInitiator)
                            {
                                if (m_KeyEstablishmentState == KeyEstablishmentState.SentKeyConfirmation)
                                {
                                    m_ReceivedSMAC = PartnerSMAC;

                                    m_KeyEstablishmentState = KeyEstablishmentState.ReceivedKeyConfirmation;
                                }
                                else
                                {
                                    TerminateKeyEstablishment(receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, KeyEstablishmentErrors.BadMessage);
                                }
                            }
                            else
                            {
                                // TODO:
                            }

                            break;
                        }
                    case (byte)KeyEstablishmentCommands.TerminateKeyEstablishment:
                        {
                            KeyEstablishmentErrors Error = (KeyEstablishmentErrors)ZclFrame.Data[0];
                            // Terminated so clear out any data we have gotten
                            m_KeyEstablishmentState = KeyEstablishmentState.NotEstablished;
                            m_EZSP.ClearTemporaryDataMaybeStoreLinkKey(false, out Status);
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Key Establishment terminated. Reason: " + Error.ToString());
                            break;
                        }
                    default:
                        {
                            // Unknown command so we should send a default response
                            SendDefaultResponse((ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.KeyEstablishment,
                                receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, ZCLStatus.UnsupportedClusterCommand);
                            break;
                        }
                }
            }
            else // We Received a general ZCL command
            {
                HandleGeneralZCLMessage(receivedMessage);
            }
        }

        /// <summary>
        /// Generates the CBKE Keys
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void GenerateCBKEKeys()
        {
            DateTime StartTime = DateTime.Now;
            TimeSpan CBKETimeout = new TimeSpan(0, 0, CBKE_GENERATE_TIME);
            EmberStatus Status;

            m_CBKEKeyGenerated = false;
            m_EZSP.CBKEKeyGenerated += m_CBKEHandler;
            m_EZSP.GenerateCBKEKeys(out Status);

            // Wait for CBKE generation to complete
            while (m_CBKEKeyGenerated == false && (DateTime.Now - StartTime) <= CBKETimeout)
            {
                Thread.Sleep(25);
            }

            m_EZSP.CBKEKeyGenerated -= m_CBKEHandler;

            if (m_CBKEKeyGenerated == false)
            {
                throw new TimeoutException("CBKE Key Generation did not complete within the allowed time");
            }
        }

        /// <summary>
        /// Calculates the SMACS key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void CalculateSMACS()
        {
            EmberStatus Status;
            TimeSpan SMACTimeout = new TimeSpan(0, 0, CBKE_CONFIRM_TIME);
            DateTime StartTime = DateTime.Now;

            m_SMACSCalculated = false;
            m_EZSP.SmacsCalculated += m_SMACSHandler;
            m_EZSP.CalculateSmacs(true, m_PartnerCertificate, m_PartnerEphermeralKey, out Status);

            while (m_SMACSCalculated == false && (DateTime.Now - StartTime) < SMACTimeout)
            {
                Thread.Sleep(25);
            }

            if (m_SMACSCalculated == false)
            {
                throw new TimeoutException("SMACS Calculation did not complete within the allowed time");
            }
        }

        /// <summary>
        /// Verifies that the SMAC keys match
        /// </summary>
        /// <param name="firstSMAC">The first SMAC to validate</param>
        /// <param name="secondSMAC">The second SMAC to validate</param>
        /// <returns>True of the SMAC keys are valid. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected bool VerifySmacs(byte[] firstSMAC, byte[] secondSMAC)
        {
            bool Valid = true;

            if (firstSMAC != null && firstSMAC.Length == 16 && secondSMAC != null && secondSMAC.Length == 16)
            {
                // The SMAC values are valid size so make sure they match
                for (int Index = 0; Index < firstSMAC.Length; Index++)
                {
                    if (firstSMAC[Index] != secondSMAC[Index])
                    {
                        Valid = false;
                    }
                }
            }
            else
            {
                Valid = false;
            }

            return Valid;
        }

        /// <summary>
        /// Confirms the key establishment key
        /// </summary>
        /// <param name="destination">The destination Node ID</param>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="smac">The SMAC to send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void ConfirmKey(ushort destination, byte endpoint, byte[] smac)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            byte[] Message = new byte[16];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.DestinationEndpoint = endpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID).Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.KeyEstablishment;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Create the message
            MessageWriter.Write(smac);

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;

            if (m_KeyEstablishmentInitiator)
            {
                ZclFrame.Direction = ZCLDirection.SentFromClient;
            }
            else
            {
                ZclFrame.Direction = ZCLDirection.SentFromServer;
            }

            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)KeyEstablishmentCommands.ConfirmKeyDataRequest;
            ZclFrame.Data = Message;

            m_KeyEstablishmentState = KeyEstablishmentState.SentKeyConfirmation;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Handles the SMACS Calculated event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void m_EZSP_SmacsCalculated(object sender, SmacsCalculatedEventArgs e)
        {
            m_LocalSMAC = e.InitiatorSmac;
            m_PartnerSMAC = e.ResponderSmac;

            m_SMACSCalculated = true;
        }

        /// <summary>
        /// Sends the terminate key establishment command
        /// </summary>
        /// <param name="destination">The destination to send the message to</param>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="error">The error to send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void TerminateKeyEstablishment(ushort destination, byte endpoint, KeyEstablishmentErrors error)
        {
            EmberStatus Status;
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            byte[] Message = new byte[4];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.DestinationEndpoint = endpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID).Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.KeyEstablishment;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Create the message
            MessageWriter.Write((byte)error);
            MessageWriter.Write((byte)0); // Wait Time
            MessageWriter.Write(CBKE_ESTABLISHMENT);

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;

            if (m_KeyEstablishmentInitiator)
            {
                ZclFrame.Direction = ZCLDirection.SentFromClient;
            }
            else
            {
                ZclFrame.Direction = ZCLDirection.SentFromServer;
            }

            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)KeyEstablishmentCommands.TerminateKeyEstablishment;
            ZclFrame.Data = Message;

            // Don't send the message if we have already terminated.
            if (m_KeyEstablishmentState != KeyEstablishmentState.NotEstablished)
            {
                SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
            }

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Key Establishment terminated. Reason: " + error.ToString());
            m_KeyEstablishmentState = KeyEstablishmentState.NotEstablished;
            m_EZSP.ClearTemporaryDataMaybeStoreLinkKey(false, out Status);
        }

        /// <summary>
        /// Handles the CBKE Key Generated Message
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected void m_EZSP_CBKEKeyGenerated(object sender, CBKEKeyGeneratedEventArgs e)
        {
            if (e.Status == EmberStatus.Success)
            {
                m_LocalEphemeralKey = e.PublicKey;
                m_CBKEKeyGenerated = true;
            }
            else
            {
                m_LocalEphemeralKey = null;
                m_CBKEKeyGenerated = false;
            }
        }

        #endregion

        #region Basic Cluster

        /// <summary>
        /// Handles basic cluster Messages
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/13 MP                 Created

        private void HandleBasicClusterMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);

            switch (ZclFrame.CommandID)
            {
                case (byte)GeneralZCLCommandIDs.ReadAttributesResponse:
                    {
                        HandleReadResponseMessage(receivedMessage);
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received a ReadResponse command.");
                        break;
                    }
                case (byte)GeneralZCLCommandIDs.WriteAttributesResponse:
                    {
                        HandleWriteResponseMessage(receivedMessage);
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received a ReadResponse command.");
                        break;
                    }
                default:
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unsupported basic cluster command received. Command ID: " + ZclFrame.CommandID.ToString("X2", CultureInfo.InvariantCulture));
                        // Unknown command so we should send a default response
                        SendDefaultResponse((ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering,
                            receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, ZCLStatus.UnsupportedClusterCommand);
                        break;
                    }
            }
        }

        /// <summary>
        /// Handles Read Attributes response
        /// </summary>
        /// <param name="receivedMessage">the incoming message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/13 MP                 Created
        private void HandleReadResponseMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;
            bool bUnsupportedAttribute = false;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "*** Read Response Details: APS Options: " + receivedMessage.APSFrame.Options.ToString()
                + " Disable Response: " + ZclFrame.DisableDefaultResponse.ToString());

            // Attributes read and received
            BasicAttributesReceived receivedAttributes = new BasicAttributesReceived();

            int pos = 0;
            int length = (int)DataReader.BaseStream.Length;

            pos += sizeof(ushort);
            if (pos <= length)
            {
                receivedAttributes.Identifier = (uint)DataReader.ReadUInt16();
                Console.WriteLine("Identifier: " + receivedAttributes.Identifier.ToString());
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                receivedAttributes.Status = DataReader.ReadByte();

                // if the status is equal to "unsupported attribute" (0x86), then there won't be
                // any more fields to read, so we should just stop here.
                if (receivedAttributes.Status == 0x86)
                {
                    bUnsupportedAttribute = true;
                }

                Console.WriteLine("Status: " + receivedAttributes.Status.ToString());
            }

            // only read this if we are sure that the attribute is not unsupported
            if (!bUnsupportedAttribute)
            {
                pos += sizeof(byte);
                if (pos <= length)
                {
                    receivedAttributes.DataType = DataReader.ReadByte();
                    Console.WriteLine("Status: " + receivedAttributes.DataType.ToString());
                }

                // Get the rest of the message.
                if ((length - pos) == 4)
                {
                    pos = pos + (length - pos);
                    if (pos <= length)
                    {
                        receivedAttributes.Value = DataReader.ReadUInt32();
                        Console.WriteLine("Status: " + receivedAttributes.Value.ToString());
                    }
                }
                else if ((length - pos) == 3) // <--- Need to find out why divisor and multiplier are not working.
                {
                    pos = pos + (length - pos);
                    if (pos <= length)
                    {
                        byte[] value = DataReader.ReadBytes(3);

                        receivedAttributes.Value = 0x00000000; // Really, we only need the first 3 bytes of this.
                        receivedAttributes.Value = (receivedAttributes.Value | value[2]) << 8;
                        receivedAttributes.Value = (receivedAttributes.Value | value[1]) << 8;
                        receivedAttributes.Value = (receivedAttributes.Value | value[0]);

                        Console.WriteLine("Status: " + receivedAttributes.Value.ToString());
                    }
                }
                else if ((length - pos) == 2)
                {
                    pos = pos + (length - pos);
                    if (pos <= length)
                    {
                        receivedAttributes.Value = DataReader.ReadUInt16();
                        Console.WriteLine("Status: " + receivedAttributes.Value.ToString());
                    }
                }
                else if ((length - pos) == 1)
                {
                    pos = pos + (length - pos);
                    if (pos <= length)
                    {
                        receivedAttributes.Value = DataReader.ReadByte();
                        Console.WriteLine("Status: " + receivedAttributes.Value.ToString());
                    }
                }
            }

            // read Received
            OnReadResponseReceived(receivedAttributes);
        }

        /// <summary>
        /// Raises the ZigBee Read Response Received event
        /// </summary>
        /// <param name="AllAttributes">array of attributes received from message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/13 MP                 Created
        private void OnReadResponseReceived(BasicAttributesReceived AllAttributes)
        {
            if (ReadResponseReceived != null)
            {
                ReadResponseReceived(this, new ReadResponseEventArgs(AllAttributes));
            }
        }

        /// <summary>
        /// Handles Write Attributes response
        /// </summary>
        /// <param name="receivedMessage"> the incoming message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/27/13 MP                 Created
        private void HandleWriteResponseMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "*** Read Response Details: APS Options: " + receivedMessage.APSFrame.Options.ToString()
                + " Disable Response: " + ZclFrame.DisableDefaultResponse.ToString());

            // status from the write
            byte receivedStatus = 0;

            int pos = 0;
            int length = (int)DataReader.BaseStream.Length;

            pos += sizeof(byte);
            if (pos <= length)
            {
                receivedStatus = (byte)DataReader.ReadByte();
                Console.WriteLine("Status: " + receivedStatus.ToString());
            }

            // read Received
            OnWriteResponseReceived((ZCLStatus)receivedStatus);
        }

        /// <summary>
        /// Raises the ZigBee Read Response Received event
        /// </summary>
        /// <param name="Status">The status rceived</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/16/13 MP                 Created
        private void OnWriteResponseReceived(ZCLStatus Status)
        {
            if (WriteResponseReceived != null)
            {
                WriteResponseReceived(this, new WriteResponseEventArgs(Status));
            }
        }

        /// <summary>
        /// Send Identify command (specific to the identify cluster) 
        /// </summary>
        /// <param name="Command"> The identify command we want to send</param>
        /// <param name="data">The data that will be sent out as the payload, don't worry about it
        /// if sending identifyquery command. Just put 0x0000 or something.</param>
        /// <param name="destination">usually the meter</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/27/13 MP                 Created

        private void SendIdentifyCommand(ushort destination, IdentifyClusterCommands Command, ushort data)
        {
            // Add the cluster to the list of supported clusters. For some reason this isn't done automatically, if it is a problem
            // please change it, but for the sake of testing im going to leave it here.
            foreach (ZigBeeEndpointInfo endpoint in m_Endpoints)
            {
                // will hopefully insert this cluster into the last spot of the list.
                endpoint.ClientClusterList.Insert(m_Endpoints.Count, (ushort)GeneralClusters.Identify);
                break;
            }

            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[2];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            IncomingMessage Response = null;
            //byte[] ResponseData = null;

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)GeneralClusters.Identify);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)GeneralClusters.Identify;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // write message. Only write if it is the identify command.
            if (Command == IdentifyClusterCommands.Identify)
            {
                MessageWriter.Write((ushort)data);
            }

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)Command;

            // write message. Only write if it is the identify command.
            if (Command == IdentifyClusterCommands.Identify)
            {
                ZclFrame.Data = MessageData;
            }

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);

            // Remove the cluster from the list. Should removed the endpoint at the first index.
            foreach (ZigBeeEndpointInfo endpoint in m_Endpoints)
            {
                endpoint.ClientClusterList.RemoveAt(m_Endpoints.Count - 1);
                break;
            }
        }

        /// <summary>
        /// sends identify command (specific to identify cluster). This probably won't be used very often.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/27/13 MP                 Created

        public void SendIdentifyCommand(IdentifyClusterCommands Command, ushort data)
        {
            SendIdentifyCommand(TRUST_CENTER_NODE_ID, Command, data);
        }

        /// <summary>
        /// Raises the ZigBee Fast Polling Request Response Received event
        /// </summary>
        /// <param name="Timeout">The current value of IdentifyTime attribute</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/27/13 MP                 Created
        protected void OnIdentifyQueryResponseReceived(ushort Timeout)
        {
            if (IdentifyQueryResponseReceived != null)
            {
                IdentifyQueryResponseReceived(this, new IdentifyQueryResponseEventArgs(Timeout));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the match descriptor response
        /// </summary>
        /// <param name="receivedMessage">The received message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private byte[] ParseMatchDescriptorResponse(IncomingMessage receivedMessage)
        {
            MemoryStream MessageStream = new MemoryStream(receivedMessage.MessageContents);
            BinaryReader MessageReader = new BinaryReader(MessageStream);

            // Parse the response message
            byte TransactionSequence = MessageReader.ReadByte();
            ZDOStatus Status = (ZDOStatus)MessageReader.ReadByte();
            ushort NetworkAddress = MessageReader.ReadUInt16();
            byte Length = MessageReader.ReadByte();
            byte[] MatchList = new byte[Length];

            MatchList = new byte[Length];

            for (int iIndex = 0; iIndex < Length; iIndex++)
            {
                MatchList[iIndex] = MessageReader.ReadByte();
            }

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Match Descriptor Response Received with result: " + Status.ToString());

            return MatchList;
        }

        /// <summary>
        /// Parses the Bind Request Response Message
        /// </summary>
        /// <param name="receivedMessage">The Response Message</param>
        /// <returns>True if the bind was successful. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private bool ParseBindResponse(IncomingMessage receivedMessage)
        {
            MemoryStream MessageStream = new MemoryStream(receivedMessage.MessageContents);
            BinaryReader MessageReader = new BinaryReader(MessageStream);

            byte Sequence = MessageReader.ReadByte();
            ZDOStatus Status = (ZDOStatus)MessageReader.ReadByte();

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Binding Response Message received with result: " + Status.ToString());
            
            return Status == ZDOStatus.Success;
        }

        /// <summary>
        /// Parse Reponse to Leave Request Message.
        /// </summary>
        /// <param name="receivedMessage"></param>
        /// <returns>ZDOStatus</returns>
        private ZDOStatus ParseLeaveRequestResponse(IncomingMessage receivedMessage)
        {
            MemoryStream MessageStream = new MemoryStream(receivedMessage.MessageContents);
            BinaryReader MessageReader = new BinaryReader(MessageStream);

            byte Sequence = MessageReader.ReadByte();
            ZDOStatus Status = (ZDOStatus)MessageReader.ReadByte();

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Leave Request Response Message received with result: " + Status.ToString());

            return Status;
        }


        /// <summary>
        /// Fragments a message so that it will fit within a send message
        /// </summary>
        /// <param name="message">The message to fragment</param>
        /// <returns>The message fragments</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private List<byte[]> FragmentMessage(byte[] message)
        {
            List<byte[]> Fragments = new List<byte[]>();
            int Index = 0;

            while (Index < message.Length)
            {
                if (Index + MAX_APS_PAYLOAD_SIZE < message.Length)
                {
                    // We have more than a full block remaining
                    byte[] NewFragment = new byte[MAX_APS_PAYLOAD_SIZE];

                    Array.Copy(message, Index, NewFragment, 0, MAX_APS_PAYLOAD_SIZE);
                    Fragments.Add(NewFragment);
                }
                else
                {
                    // We only have a partial block left
                    int RemainingLength = message.Length - Index;
                    byte[] NewFragement = new byte[RemainingLength];

                    Array.Copy(message, Index, NewFragement, 0, RemainingLength);
                    Fragments.Add(NewFragement);
                }

                Index += MAX_APS_PAYLOAD_SIZE;
            }

            return Fragments;
        }

        /// <summary>
        /// Discovers the matching endpoint information of the specified device
        /// </summary>
        /// <param name="destination">The Node ID of the device to discover</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void DiscoverEndpoints(ushort destination)
        {
            foreach (ZigBeeEndpointInfo CurrentEndpoint in m_Endpoints)
            {
                List<byte> ServerClusterMapping = new List<byte>();
                List<byte> ClientClusterMapping = new List<byte>();

                // Clear any previous mappings
                if (CurrentEndpoint.ClientEndpointMapping.ContainsKey(destination))
                {
                    CurrentEndpoint.ClientEndpointMapping.Remove(destination);
                }

                if (CurrentEndpoint.ServerEndpointMapping.ContainsKey(destination))
                {
                    CurrentEndpoint.ServerEndpointMapping.Remove(destination);
                }

                // Look for endpoints supporting each of the client clusters
                for (int Index = 0; Index < CurrentEndpoint.ClientClusterList.Count; Index++)
                {
                    ushort[] ServerClustersToMatch = new ushort[] { CurrentEndpoint.ClientClusterList[Index] };
                    ushort[] ClientClustersToMatch = new ushort[0];

                    byte[] MatchingEndpoints = RequestMatchDescriptor(destination, CurrentEndpoint.ProfileID, ServerClustersToMatch, ClientClustersToMatch);

                    if (MatchingEndpoints.Length > 0)
                    {
                        ClientClusterMapping.Add(MatchingEndpoints[0]);
                    }
                    else
                    {
                        ClientClusterMapping.Add(0);
                    }
                }

                CurrentEndpoint.ClientEndpointMapping.Add(destination, ClientClusterMapping);

                // Look for endpoints supporting each of the server clusters
                for (int Index = 0; Index < CurrentEndpoint.ServerClusterList.Count; Index++)
                {
                    ushort[] ServerClustersToMatch = new ushort[0];
                    ushort[] ClientClustersToMatch = new ushort[] {CurrentEndpoint.ServerClusterList[Index]};

                    byte[] MatchingEndpoints = RequestMatchDescriptor(destination, CurrentEndpoint.ProfileID, ServerClustersToMatch, ClientClustersToMatch);

                    if (MatchingEndpoints.Length > 0)
                    {
                        ServerClusterMapping.Add(MatchingEndpoints[0]);
                    }
                    else
                    {
                        ServerClusterMapping.Add(0);
                    }
                }

                CurrentEndpoint.ServerEndpointMapping.Add(destination, ServerClusterMapping);
            }
        }

        /// <summary>
        /// Requests 
        /// </summary>
        /// <param name="destination">The destination Node ID</param>
        /// <param name="profileID">The profile ID to match</param>
        /// <param name="serverClusters">The list of server clusters to match</param>
        /// <param name="clientClusters">The list of client clusters to match</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private byte[] RequestMatchDescriptor(ushort destination, ushort profileID, ushort[] serverClusters, ushort[] clientClusters)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            byte[] Message = new byte[7 + 2 * serverClusters.Length + 2 * clientClusters.Length];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            byte[] MatchingEndpoints = new byte[0];
            byte Sequence = m_ZDOSequence++;
            IncomingMessage ReceivedMessage;

            // Set up the APS Frame
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.ZigBeeDeviceObject;
            ApsFrame.DestinationEndpoint = 0x00;
            ApsFrame.SourceEndpoint = 0x00;
            ApsFrame.ClusterID = (ushort)ZDOClusters.MatchDescriptorsRequest;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Create the Message
            MessageWriter.Write(Sequence);
            MessageWriter.Write(destination);
            MessageWriter.Write(profileID);
            MessageWriter.Write((byte)serverClusters.Length);

            foreach (ushort CurrentCluster in serverClusters)
            {
                MessageWriter.Write(CurrentCluster);
            }

            MessageWriter.Write((byte)clientClusters.Length);

            foreach (ushort CurrentCluster in clientClusters)
            {
                MessageWriter.Write(CurrentCluster);
            }

            SendUnicastMessage(destination, ApsFrame, Message);

            ReceivedMessage = WaitForZDOMessage(Sequence);

            if (ReceivedMessage != null)
            {
                MatchingEndpoints = ParseMatchDescriptorResponse(ReceivedMessage);

                // Remove the message from the list
                m_ReceivedZDOMessages.Remove(ReceivedMessage);
            }

            return MatchingEndpoints;
        }

        /// <summary>
        /// Handles the message sent event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void m_EZSP_MessageSent(object sender, MessageSentEventArgs e)
        {
            // We just keep track of the messages that have been sent out by keeping a list of message tags
            // Once we get confirmation that a message was sent we can remove the tag from the list.
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Message Sent Event occurred - Tag: " + e.MessageTag.ToString("X2", CultureInfo.InvariantCulture)
                + " Status: " + e.Status.ToString());
            if (m_SentMessageTags.Contains(e.MessageTag))
            {
                m_SentMessageTags.Remove(e.MessageTag);
            }
        }

        /// <summary>
        /// Handles the Message Received event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void m_EZSP_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (m_IsJoined)
            {
                // Make sure the Messages Received are handled in the order received
                lock (m_MessageReceivedHandler)
                {
                    IncomingMessage ReceivedMessage = e.Message;

                    // We can go ahead and ignore any loopback messages
                    if (ReceivedMessage.MessageType != EmberIncomingMessageType.BroadcastLoopback && ReceivedMessage.MessageType != EmberIncomingMessageType.MulticastLoopback)
                    {
                        // The .NET event is blocking so that only one "instance" of an event can be running at one time. Due to the asynchronous nature of ZigBee we can very easily
                        // run into a deadlock when we receive messages that require us to send a response. If we use a Task object we can handle the messages asynchronously
                        // which will allow this event handler itself to complete before the message is actually handled and therefore remove any chances for deadlock.

                        // Create the Task Action
                        Action<object> MessageReceivedAction = (object actionObject) =>
                        {
                            IncomingMessage Message = actionObject as IncomingMessage;

                            if (Message != null)
                            {
                                HandleReceivedMessage(Message);
                            }
                        };

                        // Kick off the task and immediately fall out of the event handler
                        Task.Factory.StartNew(MessageReceivedAction, (object)ReceivedMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the cluster is supported by the device
        /// </summary>
        /// <param name="profileID">The profile ID of the cluster</param>
        /// <param name="clusterID">The cluster ID</param>
        /// <param name="endpoint">The endpoint</param>
        /// <returns>The endpoint to check</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private bool IsClusterSupported(ushort profileID, ushort clusterID, byte endpoint)
        {
            return (profileID == 0x0000 && endpoint == 0x00) 
                || m_Endpoints.Where(e => e.ProfileID == profileID && e.Endpoint == endpoint && e.ClientClusterList.Contains(clusterID)).Count() > 0;
        }

        /// <summary>
        /// Sets up all of the EZSP Configuration Values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void SetEzspConfigurationValues()
        {
            EzspStatus Status;
            ushort PacketBufferCount = 24;

            m_EZSP.SetConfigurationValue(EzspConfigID.FragmentWindowSize, 1, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.FragmentDelayMs, 50, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.IndirectTransmissionTimeout, 30000, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.EndDevicePollTimeout, 12, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.MobileNodePollTimeout, 240, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.AddressTableSize, 3, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.BindingTableSize, DEFAULT_BINDING_TABLE_SIZE, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.SourceRouteTableSize, 32, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.SecurityLevel, 5, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.TrustCenterAddressCacheSize, 2, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.StackProfile, 2, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.KeyTableSize, 3, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.RequestKeyTimeout, 0, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.MaxHops, 5, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.APSAckTimeout, 350, out Status);
            m_EZSP.SetConfigurationValue(EzspConfigID.APSUnicastMessageCount, 10, out Status);

            m_EZSP.SetPolicy(EzspPolicyID.BindingModificationPolicy, EzspDecisionID.AllowBindingModification, out Status);
            m_EZSP.SetPolicy(EzspPolicyID.TrustCenterPolicy, EzspDecisionID.DisallowAllJoinsAndRejoins, out Status);
            m_EZSP.SetPolicy(EzspPolicyID.TrustCenterKeyRequestPolicy, EzspDecisionID.DenyTrustCenterRequests, out Status);
            m_EZSP.SetPolicy(EzspPolicyID.ApplicationKeyRequestPolicy, EzspDecisionID.DenyAppKeyRequests, out Status);
            m_EZSP.SetPolicy(EzspPolicyID.MessageContentsInCallbackPolicy, EzspDecisionID.MessageTagOnlyInCallback, out Status);
            m_EZSP.SetPolicy(EzspPolicyID.PollHandlerPolicy, EzspDecisionID.PollHandlerIgnore, out Status);
            m_EZSP.SetPolicy(EzspPolicyID.UnicastRepliesPolicy, EzspDecisionID.HostWillNotSupplyReply, out Status);

            Status = EzspStatus.Success;

            while (Status == EzspStatus.Success && PacketBufferCount < ushort.MaxValue)
            {
                m_EZSP.SetConfigurationValue(EzspConfigID.PacketBufferCount, PacketBufferCount, out Status);
                PacketBufferCount++;
            }
        }

        /// <summary>
        /// Clears all of the Key Table data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void ClearKeyTables()
        {
            EzspStatus Status;
            EmberStatus EraseStatus;
            ushort KeyCount;

            m_EZSP.GetConfigurationValue(EzspConfigID.KeyTableSize, out Status, out KeyCount);

            if(Status == EzspStatus.Success)
            {
                for(byte Index = 0; Index < KeyCount; Index++)
                {
                    m_EZSP.EraseKeyTableEntry(Index, out EraseStatus);
                }

                m_EZSP.ClearTemporaryDataMaybeStoreLinkKey(false, out EraseStatus);
            }
        }

        /// <summary>
        /// Adds the Endpoint for the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void AddEndpoints()
        {
            EzspStatus Status = EzspStatus.Success;

            if (m_IsConnected && m_IsJoined == false)
            {
                foreach (ZigBeeEndpointInfo CurrentEndpoint in m_Endpoints)
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Adding Endpoint " + CurrentEndpoint.Endpoint.ToString(CultureInfo.InvariantCulture)
                        + "- Profile ID: " + CurrentEndpoint.ProfileID.ToString("X4", CultureInfo.InvariantCulture) 
                        + " Device ID: " + CurrentEndpoint.DeviceID.ToString("X4", CultureInfo.InvariantCulture));

                    m_EZSP.AddEndpoint(CurrentEndpoint.Endpoint, CurrentEndpoint.ProfileID, CurrentEndpoint.DeviceID, CurrentEndpoint.AppFlags,
                        (byte)CurrentEndpoint.ServerClusterList.Count, (byte)CurrentEndpoint.ClientClusterList.Count, 
                        CurrentEndpoint.ServerClusterList.ToArray(), CurrentEndpoint.ClientClusterList.ToArray(), out Status);

                    if (Status != EzspStatus.Success)
                    {
                        throw new Exception("Could not create the endpoint. Status: " + Status.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Handles the scan complete event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void m_EZSP_ScanCompleted(object sender, EventArgs e)
        {
            m_ScanComplete = true;
        }

        /// <summary>
        /// Handles the Stack Status Updated Event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void  m_EZSP_StackStatusUpdated(object sender, StackStatusUpdatedEventArgs e)
        {
            m_IsJoined = e.Status == EmberStatus.NetworkUp;
            m_JoinFailed = e.Status == EmberStatus.JoinFailed;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Stack Status Changed: " + e.Status.ToString());
        }

        /// <summary>
        /// Raises Default Response Received Event
        /// </summary>
        /// <param name="status">ZCLStatus of the Response</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 PGH 2.60.07        Created
        private void OnDefaultResponseReceived(ZCLStatus status)
        {
            if (DefaultResponseReceived != null)
            {
                DefaultResponseReceived(this, new DefaultResponseEventArgs(status));
            }
        }

        /// <summary>
        /// Raises IPP Data Response Received Event
        /// </summary>
        /// <param name="message">The incoming message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/01/12 PGH 2.70.36        Created
        private void OnIPPDataResponseReceived(IncomingMessage message)
        {
            if (IPPDataResponseReceived != null)
            {
                IPPDataResponseReceived(this, new IPPDataResponseEventArgs(message));
            }
        }

        #endregion

        #region Member Variables

        // Communications objects

        /// <summary>
        /// Serial Communications object
        /// </summary>
        protected EZSPSerialCommunications m_Comm;
        /// <summary>
        /// ASH Protocol Layer
        /// </summary>
        protected ASHProtocol m_ASH;
        /// <summary>
        /// EZSP Protocol Layer
        /// </summary>
        protected EZSPProtocol m_EZSP;
        /// <summary>
        /// Communications logger
        /// </summary>
        protected EZSPLogger m_Logger;
        /// <summary>
        /// The Device ID that should be used when creating the endpoints
        /// </summary>
        protected ushort m_DeviceTypeID;
        /// <summary>
        /// Gets the list of hosted endpoints
        /// </summary>
        protected List<ZigBeeEndpointInfo> m_Endpoints;
        /// <summary>
        /// The list of messages that have been received
        /// </summary>
        protected List<IncomingMessage> m_UnhandledMessages;
        /// <summary>
        /// Sequence number used for sending ZCL messages
        /// </summary>
        protected byte m_ZCLSequenceNumber;
        /// <summary>
        /// The device certificate
        /// </summary>
        protected byte[] m_Certificate;
        /// <summary>
        /// The Trust Center's EUI
        /// </summary>
        protected ulong m_TrustCenterEUI;
        /// <summary>
        /// The local EUI
        /// </summary>
        protected ulong m_LocalEUI;

        // ZCL Basic Cluster -- Attributes
        //
        private String m_ZCLManufacturerName;
        private String m_ZCLModelIdentifier;
        private String m_ZCLDateCode;

        private ushort m_PowerDescriptor;

        // State variables
        private bool m_IsConnected;
        private bool m_IsJoined;
        private bool m_JoinFailed;
        private bool m_ScanComplete;

        private List<IncomingMessage> m_ReceivedZDOMessages;
        /// <summary>
        /// ZCL Response Messages
        /// </summary>
        protected List<IncomingMessage> m_ZCLResponseMessages;

        // Event Handlers
        private EventHandler m_ScanCompleteHandler;
        private StackStatusUpdatedHandler m_StackStatusHandler;
        private MessageReceivedHandler m_MessageReceivedHandler;
        private MessageSentHandler m_MessageSentHandler;

        private List<byte> m_SentMessageTags;

        private byte[] m_InstallationCode;
        private byte[] m_PreconfiguredLinkKey;

        private ulong m_MACAddress;
        private ushort m_NodeID;
        private byte m_CurrentMessageTag;
        private byte m_ZDOSequence;
        private sbyte m_TransmitPower;

        private DateTime m_LastRetrievedUTCTime;
        private TimeSpan m_UTCTimeOffset;
        private ZCLTimeStatus m_TimeStatus;
        private int m_TimeZone;
        private DateTime m_DSTStart;
        private DateTime m_DSTEnd;
        private int m_DSTShift;
        private DateTime m_StandardTime;
        private TimeSpan m_StandardTimeOffset;
        private DateTime m_LocalTime;
        private TimeSpan m_LocalTimeOffset;

        // Meter's Basic Cluster Attributes, read via HAN network
        private byte m_MeterZCLVersion;
        private byte m_MeterApplicationVersion;
        private byte m_MeterStackVersion;
        private byte m_MeterHWVersion;
        private String m_MeterManufacturerName;
        private String m_MeterModelIdentifier;
        private String m_MeterDateCode;
        private byte m_MeterPowerSource;
        private string m_MeterLocationDescription;
        private byte m_MeterPhysicalEnvironment;
        private bool m_MeterDeviceEnabled;
        private byte m_MeterAlarmMask;
        private byte m_OTADownLoadStatus;

        // Meter's Identify cluster attribute
        private UInt16 m_IdentifyTime;

        // EZSP Event handlers
        private CBKEKeyGeneratedHandler m_CBKEHandler;
        private SmacsCalculatedHandler m_SMACSHandler;

        // CBKE values

        /// <summary>
        /// Key Establishment State
        /// </summary>
        protected KeyEstablishmentState m_KeyEstablishmentState;
        /// <summary>
        /// Partner Certificate
        /// </summary>
        protected byte[] m_PartnerCertificate;
        /// <summary>
        /// Local Ephemeral Key
        /// </summary>
        protected byte[] m_LocalEphemeralKey;
        /// <summary>
        /// Partner Ephemeral Key
        /// </summary>
        protected byte[] m_PartnerEphermeralKey;

        private ulong m_PartnerEUI;
        private byte[] m_LocalSMAC;
        private byte[] m_PartnerSMAC;
        private byte[] m_ReceivedSMAC;
        private byte m_PartnerEphemeralKeyGenerateTime;
        private byte m_PartnerConfirmKeyTime;

        private bool m_CBKEKeyGenerated;
        private bool m_SMACSCalculated;
        private bool m_KeyEstablishmentInitiator;

        private object m_IPPDataRequestLocker = new object();
        private object m_MessageReceivedLocker = new object();

        #endregion
    }

    /// <summary>
    /// DefaultResponseEventArgs class for use with DefaultResponseEventHandler
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  03/29/12 PGH 2.60.07        Created
    public class DefaultResponseEventArgs : EventArgs
    {
        /// <summary>
        /// DefaultResponseEventArgs Constructor
        /// </summary>
        public DefaultResponseEventArgs(ZCLStatus status)
        {
            m_Status = status;
        }

        /// <summary>
        /// DefaultResponseEventArgs Status property
        /// </summary>
        public ZCLStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        private ZCLStatus m_Status;
    }

    /// <summary>
    /// IPPDataResponseEventArgs class for use with Itron Private Profile (IPP) Data Response Cluster event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/01/12 PGH 2.70.36        Created
    public class IPPDataResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IPPDataResponseEventArgs(IncomingMessage message)
        {
            m_Message = message;
        }

        /// <summary>
        /// IPPDataResponseEventArgs Message property
        /// </summary>
        public IncomingMessage Message
        {
            get
            {
                return m_Message;
            }
        }

        private IncomingMessage m_Message;
    }

    /// <summary>
    /// ReadResponseEventArgs class for use with Read Response event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  12/16/13 MP                 Created

    public class ReadResponseEventArgs : EventArgs
    {
        /// <summary>
        /// ReadResponseEventArgs Constructor
        /// </summary>
        public ReadResponseEventArgs(BasicAttributesReceived AllAttributes)
        {
            receivedAttributes = new BasicAttributesReceived();
            receivedAttributes = AllAttributes;
        }

        /// <summary>
        /// ReadResponsedEventArgs Attributes property
        /// </summary>
        public BasicAttributesReceived AttributesReceived
        {
            get
            {
                return receivedAttributes;
            }
        }

        private BasicAttributesReceived receivedAttributes;
    }

    /// <summary>
    /// WriteResponseEventArgs class for use with Write Response event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  12/27/13 MP                 Created

    public class WriteResponseEventArgs : EventArgs
    {
        /// <summary>
        /// WriteResponseEventArgs Constructor
        /// </summary>
        public WriteResponseEventArgs(ZCLStatus Status)
        {
            m_status = Status;     
        }

        /// <summary>
        /// WriteResponsedEventArgs status property
        /// </summary>
        public ZCLStatus Status
        {
            get
            {
                return m_status;
            }
        }

        private ZCLStatus m_status;
    }

    /// <summary>
    /// IdentifyQueryResponseEventArgs class for use with IdentifyQuery command Response event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  12/27/13 MP                 Created

    public class IdentifyQueryResponseEventArgs : EventArgs
    {
        /// <summary>
        /// IdentifyQueryResponseEventArgs Constructor
        /// </summary>
        public IdentifyQueryResponseEventArgs(ushort Timeout)
        {
            m_Timeout = Timeout;
        }

        /// <summary>
        /// IdentifyQueryResponsedEventArgs status property
        /// </summary>
        public ushort Timeout
        {
            get
            {
                return m_Timeout;
            }
        }

        private ushort m_Timeout;
    }

    /// <summary>
    /// Class for individual attributes fields
    /// </summary>
    /// 
    public class BasicAttributesReceived
    {
        #region Members

        private uint m_Identifier;
        private byte m_Status;
        private byte m_DataType;
        private uint m_Value;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public BasicAttributesReceived()
        {
            Identifier = 0;
            Status = 0;
            DataType = 0;
            Value = 0;

        }

        /// <summary>
        /// Property for the identifier
        /// </summary>
        public uint Identifier
        {
            get
            {
                return m_Identifier;
            }
            set
            {
                m_Identifier = value;
            }
        }

        /// <summary>
        ///  Property for the status
        /// </summary>
        public byte Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Property for the DataType
        /// </summary>
        public byte DataType
        {
            get
            {
                return m_DataType;
            }
            set
            {
                m_DataType = value;
            }
        }

        /// <summary>
        /// Property for the value
        /// </summary>
        public uint Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        #endregion
    }
}
