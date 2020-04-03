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
using System.Globalization;
using System.Windows.Forms;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

#region Public enum used by every one
namespace Itron.Metering.Zigbee.Enums
{

    /// <summary>
    /// OTAStatusCodes enum for OTA status
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/25/16 Hetsh             Created
    public enum OTAStatusCodes
    {
        /// <summary>Success</summary>
        Success = 0x00,
        /// <summary>ABORT</summary>
        ABORT = 0x95,
        /// <summary>NOT AUTHORIZED</summary>
        NOT_AUTHORIZED = 0x7E,
        /// <summary>INVALID IMAGE</summary>
        INVALID_IMAGE = 0x96,
        /// <summary>WAIT FOR DATA</summary>
        WAIT_FOR_DATA = 0x97,
        /// <summary>NO IMAGE AVAILABLE</summary>
        NO_IMAGE_AVAILABLE = 0x98,
        /// <summary>MALFORMED COMMAND</summary>
        MALFORMED_COMMAND = 0x80,
        /// <summary>UNSUP CLUSTER COMMAND</summary>
        UNSUP_CLUSTER_COMMAND = 0x81,
        /// <summary>REQUIRE MORE IMAGE</summary>
        REQUIRE_MORE_IMAGE = 0x99
    };

    /// <summary>
    /// OTAPayloadType enum for OTA status
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/25/16 Hetsh             Created
    public enum OTAPayloadType
    {
        /// <summary>Query jitter</summary>
        Query_jitter = 0x00,
        /// <summary>Query jitter and manufacturer code</summary>
        Query_jitter_and_manufacturer_code = 0x01,
        /// <summary>Query jitter and manufacturer code and image type</summary>
        Query_jitter_and_manufacturer_code_and_image_type = 0x02,
        /// <summary>Query jitter manufacturer code_image type and new file version</summary>
        Query_jitter_manufacturer_code_image_type_and_new_file_version = 0x03,
        /// <summary>Reserved</summary>
        Reserved = 0x04,
    };



    /// <summary>
    /// OTAFlowStatus enum for which state in processe of downloading an image
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/25/16 Hetsh             Created
    public enum OTAFlowStatus
    {
        /// <summary> Image_not_vailble or Image_querry has not begun yet</summary>
        Image_not_available,
        /// <summary>Image_availble</summary>
        Image_available,
        /// <summary>Next Image requested</summary>
        Next_Image_requested ,
        /// <summary>Next Image received</summary>
        Next_Image_received,
        /// <summary>Next Image Block requested</summary>
        Next_Image_Block_requested ,
        /// <summary>Next_Image_Block_received</summary>
        Next_Image_Block_received,
        /// <summary>Image Block error</summary>
        Image_Block_error ,
        /// <summary>Next Image blocks end</summary>
        Next_Image_blocks_end,
        /// <summary>Next_Image_Abort</summary>
        Next_Image_Abort,
     
     
    };


    /// <summary>
    /// Payload enum
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/25/16 Hetsh             Created
    public enum Payload
    {
        /// <summary> Query jitter</summary>
        Query_jitter,
        /// <summary>Query jitter and manufacturer code</summary>
        Query_jitter_and_manufacturer,
        /// <summary>Query jitter, manufacturer code, and image type</summary>
        Query_jitter_manufacturer_and_image,
        /// <summary>Query jitter, manufacturer code, image type, and new file version</summary>
        Query_jitter_manufacturer_image_and_file_version,

    };

    

}

#endregion


namespace Itron.Metering.Zigbee
{
    #region Definitions

    /// <summary>
    /// The Smart Energy ZigBee Cluster ID's
    /// </summary>
    public enum SmartEnergyClusters : ushort
    {
        /// <summary>Pricing</summary>
        Price = 0x0700,
        /// <summary>Demand Response Load Control</summary>
        DRLC = 0x0701,
        /// <summary>Simple Metering</summary>
        SimpleMetering = 0x0702,
        /// <summary>Messaging</summary>
        Messaging = 0x0703,
        /// <summary>SE Tunneling</summary>
        SmartEnergyTunneling = 0x0704,
        /// <summary>Pre-Payment</summary>
        PrePayment = 0x0705,
        /// <summary>Key Establishment</summary>
        KeyEstablishment = 0x0800,
    }
    
    /// <summary>
    /// The General ZigBee Clusters ID's
    /// </summary>
    public enum GeneralClusters : ushort
    {
        /// <summary>Basic</summary>
        Basic = 0x0000,
        /// <summary>Power Configuration</summary>
        PowerConfiguration = 0x0001,
        /// <summary>Device Temperature Configuration</summary>
        DeviceTemperatureConfiguration = 0x0002,
        /// <summary>Identify</summary>
        Identify = 0x0003,
        /// <summary>Groups</summary>
        Groups = 0x0004,
        /// <summary>Scenes</summary>
        Scenes = 0x0005,
        /// <summary>On/Off</summary>
        OnOff = 0x0006,
        /// <summary>On/Off Switch Control</summary>
        OnOffSwitchConfiguration = 0x0007,
        /// <summary>Level Control</summary>
        LevelControl = 0x0008,
        /// <summary>Alarms</summary>
        Alarms = 0x0009,
        /// <summary>Time</summary>
        Time = 0x000A,
        /// <summary>RSSI Location</summary>
        RSSILocation = 0x000B,
        /// <summary>Commissioning</summary>
        Commissioning = 0x015,
        /// <summary>Bind to the OTA Cluster</summary>
        OTA = 0x0019,
    }

    /// <summary>
    /// ZCL Status Codes
    /// </summary>
    public enum ZCLStatus : byte
    {
        /// <summary>Success</summary>
        Success = 0x00,
        /// <summary>Failure</summary>
        Failure = 0x01,
        /// <summary>Not Authorized</summary>
        NotAuthorized = 0x7E,
        /// <summary>Reserved Field Not Zero</summary>
        ReservedFieldNotZero = 0x7F,
        /// <summary>Malformed Command</summary>
        MalformedCommand = 0x80,
        /// <summary>Unsupported Cluster Command</summary>
        UnsupportedClusterCommand = 0x81,
        /// <summary>Unsupported General Command</summary>
        UnsupportedGeneralCommand = 0x82,
        /// <summary>Unsupported Manufacturer Cluster Command</summary>
        UnsupportedManufacturerClusterCommand = 0x83,
        /// <summary>Unsupported Manufacturer General Command</summary>
        UnsupportedManufacturerGeneralCommand = 0x84,
        /// <summary>Invalid Field</summary>
        InvalidField = 0x85,
        /// <summary>Unsupported Attribute</summary>
        UnsupportedAttribute = 0x86,
        /// <summary>Invalid Value</summary>
        InvalidValue = 0x87,
        /// <summary>Read Only</summary>
        ReadOnly = 0x88,
        /// <summary>Insufficient Space</summary>
        InsufficientSpace = 0x89,
        /// <summary>Duplicate Exists</summary>
        DuplicateExists = 0x8A,
        /// <summary>Not Found</summary>
        NotFound = 0x8B,
        /// <summary>Unreportable Attribute</summary>
        UnreportableAttribute = 0x8C,
        /// <summary>Invalid Data Type</summary>
        InvalidDataType = 0x8D,
        /// <summary>Invalid Selector</summary>
        InvalidSelector = 0x8E,
        /// <summary>Write Only</summary>
        WriteOnly = 0x8F,
        /// <summary>Inconsistent Startup State</summary>
        InconsistentStartupState = 0x90,
        /// <summary>Defined Out Of Band</summary>
        DefinedOutOfBand = 0x91,
        /// <summary>Abort</summary>
        Abort = 0x95,
        /// <summary>Invalid Image</summary>
        InvalidImage = 0x96,
        /// <summary>Wait for Data</summary>
        WaitForData = 0x97,
        /// <summary>No Image Available</summary>
        NoImageAvailable = 0x98,
        /// <summary>Require More Image</summary>
        RequireMoreImage = 0x99,
        /// <summary>Hardware Failure</summary>
        HardwareFailure = 0xC0,
        /// <summary>Software Failure</summary>
        SoftwareFailure = 0xC1,
        /// <summary>Calibration Error</summary>
        CalibrationError = 0xC2,
    }

    /// <summary>
    /// Used to specify which Smart Energy Clusters the device should bind to
    /// </summary>
    [Flags]
    public enum SmartEnergyBindingClusters : byte
    {
        /// <summary>Don't bind to any clusters</summary>
        None = 0x00,
        /// <summary>Bind to the Simple Metering Cluster</summary>
        SimpleMetering = 0x01,
        /// <summary>Bind to the Messaging Cluster</summary>
        Messaging = 0x02,
        /// <summary>Bind to the Pricing Cluster</summary>
        Price = 0x04,
        /// <summary>Bind to the DRLC Cluster</summary>
        DRLC = 0x08,
        /// <summary>Bind to the Time Cluster</summary>
        Time = 0x10,
        /// <summary>Bind to the OTA Cluster</summary>
        OTA = 0x20,
    }

    /// <summary>
    /// Server side DRLC Command ID's
    /// </summary>
    public enum DRLCServerCommands : byte
    {
        /// <summary>Schedule a load control event</summary>
        LoadControlEvent = 0x00,
        /// <summary>Cancel an individual load control event</summary>
        CancelLoadControlEvent = 0x01,
        /// <summary>Cancel all load control events</summary>
        CancelAllLoadControlEvents = 0x02,
    }

    /// <summary>
    /// Client side DRLC Command ID's
    /// </summary>

    public enum DRLCClientCommands : byte
    {
        /// <summary>Reports the status of an event back to the DRLC server</summary>
        ReportEventStatus = 0x00,
        /// <summary>Requests the list of scheduled events</summary>
        GetScheduledEvents = 0x01,
    }

    /// <summary>
    /// OTA Client Commands
    /// </summary>

    public enum OTAClientCommands : byte
    {
        /// <summary>QueryNextImage</summary>
        QueryNextImage = 0x01,
        /// <summary>ImageBlockRequest</summary>
        ImageBlockRequest = 0x03,
        /// <summary>ImagePageRequest</summary>
        ImagePageRequest = 0x04,
        /// <summary>UpgradeEndRequest</summary>
        UpgradeEndRequest = 0x06,
        /// <summary>QueryDeviceSpecificFileRequest .</summary>
        QueryDeviceSpecificFileRequest = 0x08,
    }


    /// <summary>
    /// OTA Server Commands
    /// </summary>
    public enum OTAServerCommands : byte
    {
        /// <summary>ImageNotify</summary>
        ImageNotify = 0x00,
        /// <summary>QueryNextImageResponse</summary>
        QueryNextImageResponse = 0x02,
        /// <summary>ImageBlockResponse</summary>
        ImageBlockResponse = 0x05,
        /// <summary>UpgradeEndResponse</summary>
        UpgradeEndResponse = 0x07,
        /// <summary>QueryDeviceSpecificFileResponse</summary>
        QueryDeviceSpecificFileResponse = 0x09,
    }

    /// <summary>
    /// Image Upgrade Status
    /// </summary>
    public enum ImageUpgradeStatus : byte
    {
        /// <summary>Nothing yet began</summary>
        Normal = 0x00,
        /// <summary>Download_in_progress</summary>
        Download_in_progress = 0x01,
        /// <summary>Download_complete</summary>
        Download_complete = 0x02,
        /// <summary>Waiting_to_upgrade</summary>
        Waiting_to_upgrade = 0x03,
        /// <summary>Count_down</summary>
        Count_down = 0x04,
        /// <summary>Wait_for_more</summary>
        Wait_for_more = 0x05,
        /// <summary>Waiting to Upgrade via External Event</summary>
        Waiting_to_Upgrade_via_External_Event = 0x06,
    }

    /// <summary>
    /// The Client side OTA Attributes
    /// </summary>
    public enum OTAClientAttributes : ushort
    {
        /// <summary>Upgrade Server Id</summary>
        UpgradeServerId = 0x0000,
        /// <summary>File Offset</summary>
        FileOffset = 0x0001,
        /// <summary>Current File Version</summary>
        CurrentFileVersion = 0x0002,
        /// <summary>Current ZigBee Stack Version</summary>
        CurrentZigBeeStackVersion = 0x0003,
        /// <summary>Download File Version</summary>
        DownloadedFileVersion = 0x0004,
        /// <summary>Download ZigBee Stack Version</summary>
        DownloadedZigBeeStackVersion = 0x0005,
        /// <summary>Image Upgrade Status</summary>
        ImageUpgradeStatus = 0x0006,
        /// <summary>Manufacturer Id</summary>
        ManufacturerId = 0x0007,
        /// <summary>Image Type Id</summary>
        ImageTypeId = 0x0008,
    }

    /// <summary>
    /// The Client side DRLC attributes
    /// </summary>
    public enum DRLCClientAttributes : ushort
    {
        /// <summary>The Utility Enrollment group that defines which group of DRLC devices it belongs to (0x00 means all groups)</summary>
        UtilityEnrollmentGroup = 0x0000,
        /// <summary>The maximum number of minutes to be used when randomizing the start of an event</summary>
        StartRandomizeMinutes = 0x0001,
        /// <summary>The maximum number of minutes to be used when randomizing the end of an event</summary>
        StopRandomizeMinutes = 0x0002,
        /// <summary>The bitfield describing which device classes will be handled</summary>
        DeviceClassValue = 0x0003,
    }

    /// <summary>
    /// Identify cluster specific commands enum
    /// </summary>
    public enum IdentifyClusterCommands : byte
    {
        /// <summary>
        /// identify command (client to server)
        /// </summary>
        Identify = 0x00,

        /// <summary>
        /// identify query command (client to server)
        /// </summary>
        IdentifyQuery = 0x01
    }

    /// <summary>
    /// The attributes available for the Simple Metering Cluster
    /// </summary>
    public enum SimpleMeteringAttributes : ushort
    {
        #region Reading Information Set

        /// <summary>The current energy summation delivered value</summary>
        CurrentSummationDelivered = 0x0000,
        /// <summary>The current energy summation received value</summary>
        CurrentSummationReceived = 0x0001,
        /// <summary>The maximum demand delivered value</summary>
        CurrentMaxDemandDelivered = 0x0002,
        /// <summary>The maximum demand received value</summary>
        CurrentMaxDemandReceived = 0x0003,
        /// <summary>The summation delivered value at the daily freeze time</summary>
        DailyFreezeTimeSummation = 0x0004,
        /// <summary>The daily freeze time of day</summary>
        DailyFreezeTime = 0x0005,
        /// <summary>The current power factor</summary>
        PowerFactor = 0x0006,
        /// <summary>The last time all of the current values were updated.</summary>
        ReadingSnapShotTime = 0x0007,
        /// <summary>The time of occurrence for the max demand delivered value</summary>
        CurrentMaxDemandDeliveredTime = 0x0008,
        /// <summary>The time of occurrence for the max demand received value</summary>
        CurrentMaxDemandReceivedTime = 0x0009,
        /// <summary>The interval at which the instantaneous demand attribute is updated when not in fast poll mode</summary>
        DefaultUpdatePeriod = 0x000A,
        /// <summary>The interval at which the instantaneous demand attribute is updated when in fast poll mode</summary>
        FastPollUpdatePeriod = 0x000B,
        /// <summary>The most recent summation delivered value for the Block Tariff Period</summary>
        CurrentBlockPeriodConsumptionDelivered = 0x000C,
        /// <summary>The daily target consumption value</summary>
        DailyConsumptionTarget = 0x000D,
        /// <summary>The currently active block</summary>
        CurrentBlock = 0x000E,
        /// <summary>The duration of a single profile interval</summary>
        ProfileIntervalPeriod = 0x000F,
        /// <summary>How often the device is woken up to provide interval data</summary>
        IntervalReadReportingPeriod = 0x0010,
        /// <summary>A specific time of day when the device is woken up to report register readings</summary>
        PresetReadingTime = 0x0011,
        /// <summary>The volume per report increment from water or gas meters</summary>
        VolumePerReport = 0x0012,
        /// <summary>The volume per minute limit in a flow restrictor.</summary>
        FlowRestriction = 0x0013,
        /// <summary>The state of the supply at the customer's premise</summary>
        SupplyStatus = 0x0014,
        /// <summary>Current volume on the inlet</summary>
        CurrentInletEnergyCarrierSummation = 0x0015,
        /// <summary>Current volume on the outlet</summary>
        CurrentOutletEnergyCarrierSummation = 0x0016,
        /// <summary>The temperature at the inlet</summary>
        InletTemperature = 0x0017,
        /// <summary>The temperature at the outlet</summary>
        OutletTemperature = 0x0018,
        /// <summary>Reference temperature used to validate the inlet and outlet temperatures</summary>
        ControlTemperature = 0x0019,
        /// <summary>Current absolute demand on the energy carrier inlet</summary>
        CurrentInletEnergyCarrierDemand = 0x001A,
        /// <summary>Current absolute demand on the energy carrier outlet</summary>
        CurrentOutletEnergyCarrierDemand = 0x001B,
        /// <summary>The summation delivered value for the previous Block Tariff Period</summary>
        PreviousBlockPeriodConsumptionDelivered = 0x001C,

        #endregion

        #region TOU Information Set

        /// <summary>Current Summation Delivered for Tier 1</summary>
        CurrentSummationDeliveredTier1 = 0x0100,
        /// <summary>Current Summation Received for Tier 1</summary>
        CurrentSummationReceivedTier1 = 0x0101,
        /// <summary>Current Summation Delivered for Tier 2</summary>
        CurrentSummationDeliveredTier2 = 0x0102,
        /// <summary>Current Summation Received for Tier 2</summary>
        CurrentSummationReceivedTier2 = 0x0103,
        /// <summary>Current Summation Delivered for Tier 3</summary>
        CurrentSummationDeliveredTier3 = 0x0104,
        /// <summary>Current Summation Received for Tier 3</summary>
        CurrentSummationReceivedTier3 = 0x0105,
        /// <summary>Current Summation Delivered for Tier 4</summary>
        CurrentSummationDeliveredTier4 = 0x0106,
        /// <summary>Current Summation Received for Tier 4</summary>
        CurrentSummationReceivedTier4 = 0x0107,
        /// <summary>Current Summation Delivered for Tier 5</summary>
        CurrentSummationDeliveredTier5 = 0x0108,
        /// <summary>Current Summation Received for Tier 5</summary>
        CurrentSummationReceivedTier5 = 0x0109,
        /// <summary>Current Summation Delivered for Tier 6</summary>
        CurrentSummationDeliveredTier6 = 0x010A,
        /// <summary>Current Summation Received for Tier 6</summary>
        CurrentSummationReceivedTier6 = 0x010B,
        /// <summary>Current Summation Delivered for Tier 7</summary>
        CurrentSummationDeliveredTier7 = 0x010C,
        /// <summary>Current Summation Received for Tier 7</summary>
        CurrentSummationReceivedTier7 = 0x010D,
        /// <summary>Current Summation Delivered for Tier 8</summary>
        CurrentSummationDeliveredTier8 = 0x010E,
        /// <summary>Current Summation Received for Tier 8</summary>
        CurrentSummationReceivedTier8 = 0x010F,
        /// <summary>Current Summation Delivered for Tier 9</summary>
        CurrentSummationDeliveredTier9 = 0x0110,
        /// <summary>Current Summation Received for Tier 9</summary>
        CurrentSummationReceivedTier9 = 0x0111,
        /// <summary>Current Summation Delivered for Tier 10</summary>
        CurrentSummationDeliveredTier10 = 0x0112,
        /// <summary>Current Summation Received for Tier 10</summary>
        CurrentSummationReceivedTier10 = 0x0113,
        /// <summary>Current Summation Delivered for Tier 11</summary>
        CurrentSummationDeliveredTier11 = 0x0114,
        /// <summary>Current Summation Received for Tier 11</summary>
        CurrentSummationReceivedTier11 = 0x0115,
        /// <summary>Current Summation Delivered for Tier 12</summary>
        CurrentSummationDeliveredTier12 = 0x0116,
        /// <summary>Current Summation Received for Tier 12</summary>
        CurrentSummationReceivedTier12 = 0x0117,
        /// <summary>Current Summation Delivered for Tier 13</summary>
        CurrentSummationDeliveredTier13 = 0x0118,
        /// <summary>Current Summation Received for Tier 13</summary>
        CurrentSummationReceivedTier13 = 0x0119,
        /// <summary>Current Summation Delivered for Tier 14</summary>
        CurrentSummationDeliveredTier14 = 0x011A,
        /// <summary>Current Summation Received for Tier 14</summary>
        CurrentSummationReceivedTier14 = 0x011B,
        /// <summary>Current Summation Delivered for Tier 15</summary>
        CurrentSummationDeliveredTier15 = 0x011C,
        /// <summary>Current Summation Received for Tier 15</summary>
        CurrentSummationReceivedTier15 = 0x011D,

        #endregion

        #region Meter Status

        /// <summary>The current status of the metering device</summary>
        Status = 0x0200,
        /// <summary>Estimated battery percent remaining</summary>
        RemainingBatteryLife = 0x0201,
        /// <summary>The total number of hours the meter has been in operation</summary>
        HoursInOperation = 0x0202,
        /// <summary>The total number of hours the device has been in a faulted state</summary>
        HoursInFault = 0x0203,

        #endregion

        #region Formatting
        /// <summary>The Unit of measure applied to summation, consumptions, profile interval and demand</summary>
        UnitOfMeasure = 0x0300,
        /// <summary>The value to multiply against the raw value</summary>
        Multiplier = 0x0301,
        /// <summary>The divisor used to divide a raw value.</summary>
        Divisor = 0x0302,
        /// <summary>Formatting used for Summation, TOU, and Block attributes</summary>
        SummationFormatting = 0x0303,
        /// <summary>Formatting used for Max and Instantaneous Demand attributes</summary>
        DemandFormatting = 0x0304,
        /// <summary>Formatting used for historical consumption attributes</summary>
        HistoricalConsumptionFormatting = 0x0305,
        /// <summary>Identifies the type of metering device.</summary>
        MeteringDeviceType = 0x0306,
        /// <summary>Text string  used to identify the location of the meter</summary>
        SiteID = 0x0307,
        /// <summary>The Serial Number of the meter</summary>
        MeterSerialNumber = 0x0308,
        /// <summary>Unit of measure for energy carrier summation attributes</summary>
        EnergyCarrierUnitOfMeasure = 0x0309,
        /// <summary>Formatting used for energy carrier summation attributes</summary>
        EnergyCarrierSummationFormatting = 0x030A,
        /// <summary>Formatting used for energy carrier demand attributes</summary>
        EnergyCarrierDemandFormatting = 0x030B,
        /// <summary>The Unit of measure for temperature based attributes</summary>
        TemperatureUnitOfMeasure = 0x030C,
        /// <summary>The formatting for temperature base attributes</summary>
        TemperatureFormatting = 0x0D,

        #endregion

        #region Historical Consumption

        /// <summary>The instantaneous demand value</summary>
        InstantaneousDemand = 0x0400,
        /// <summary>The total consumption delivered for the current day</summary>
        CurrentDayConsumptionDelivered = 0x0401,
        /// <summary>The total consumption received for the current day</summary>
        CurrentDayConsumptionReceived = 0x0402,
        /// <summary>The total consumption delivered for the previous day</summary>
        PreviousDayConsumptionDelivered = 0x0403,
        /// <summary>The total consumption received for the previous day</summary>
        PreviousDayConsumptionReceived = 0x0404,
        /// <summary>The start time of the current load profile interval for consumption delivered</summary>
        CurrentPartialProfileIntervalStartTimeDelivered = 0x0405,
        /// <summary>The start time of the current load profile interval for consumption received</summary>
        CurrentPartialProfileIntervalStartTimeReceived = 0x0406,
        /// <summary>The current value of the consumption delivered load profile interval being accumulated</summary>
        CurrentPartialProfileIntervalValueDelivered = 0x0407,
        /// <summary>The current value of the consumption received load profile interval being accumulated</summary>
        CurrentPartialProfileIntervalValueReceived = 0x0408,
        /// <summary>The maximum pressure reported for the current day</summary>
        CurrentDayMaxPressure = 0x0409,
        /// <summary>The minimum pressure reported for the current day</summary>
        CurrentDayMinPressure = 0x040A,
        /// <summary>The maximum pressure reported for the previous day</summary>
        PreviousDayMaxPressure = 0x040B,
        /// <summary>The minimum pressure reported for the previous day</summary>
        PreviousDayMinPressure = 0x040C,
        /// <summary>The maximum demand for the current day</summary>
        CurrentDayMaxDemand = 0x040D,
        /// <summary>The maximum demand for the previous day</summary>
        PreviousDayMaxDemand = 0x040E,
        /// <summary>The maximum demand for the current month</summary>
        CurrentMonthMaxDemand = 0x040F,
        /// <summary>The maximum demand for the current year</summary>
        CurrentYearMaxDemand = 0x0410,
        /// <summary>The maximum energy carrier demand for the current day</summary>
        CurrentDayMaxEnergyCarrierDemand = 0x0411,
        /// <summary>The maximum energy carrier demand for the previous day</summary>
        PreviousDayMaxEnergyCarrierDemand = 0x0412,
        /// <summary>The maximum energy carrier demand for the current month</summary>
        CurrentMonthMaxEnergyCarrierDemand = 0x0413,
        /// <summary>The minimum energy carrier demand for the current month</summary>
        CurrentMonthMinEnergyCarrierDemand = 0x0414,
        /// <summary>The maximum energy carrier demand for the current year</summary>
        CurrentYearMaxEnergyCarrierDemand = 0x0415,
        /// <summary>The minimum energy carrier demand for the current year</summary>
        CurrentYearMinEnergyCarrierDemand = 0x0416,

        #endregion

        #region Load Profile Configuration

        /// <summary>The maximum number of intervals the device is capable of returning</summary>
        MaxNumberOfPeriodsDelivered = 0x0500,

        #endregion

        #region Supply Limit

        /// <summary>The current demand delivered</summary>
        SupplyLimitCurrentDemandDelivered = 0x0600,
        /// <summary>The current supply demand limit set in the meter</summary>
        DemandLimit = 0x0601,
        /// <summary>The number of minutes over which the demand is calculated</summary>
        DemandIntegrationPeriod = 0x0602,
        /// <summary>The number of subintervals used within the integration period</summary>
        NumberOfDemandSubintervals = 0x0603,

        #endregion

        #region Block Information

        /// <summary>Current Summation Delivered for Block 1</summary>
        CurrentSummationDeliveredBlock1 = 0x0700,
        /// <summary>Current Summation Delivered for Block 2</summary>
        CurrentSummationDeliveredBlock2 = 0x0701,
        /// <summary>Current Summation Delivered for Block 3</summary>
        CurrentSummationDeliveredBlock3 = 0x0702,
        /// <summary>Current Summation Delivered for Block 4</summary>
        CurrentSummationDeliveredBlock4 = 0x0703,
        /// <summary>Current Summation Delivered for Block 5</summary>
        CurrentSummationDeliveredBlock5 = 0x0704,
        /// <summary>Current Summation Delivered for Block 6</summary>
        CurrentSummationDeliveredBlock6 = 0x0705,
        /// <summary>Current Summation Delivered for Block 7</summary>
        CurrentSummationDeliveredBlock7 = 0x0706,
        /// <summary>Current Summation Delivered for Block 8</summary>
        CurrentSummationDeliveredBlock8 = 0x0707,
        /// <summary>Current Summation Delivered for Block 9</summary>
        CurrentSummationDeliveredBlock9r = 0x0708,
        /// <summary>Current Summation Delivered for Block 10</summary>
        CurrentSummationDeliveredBlock10 = 0x0709,
        /// <summary>Current Summation Delivered for Block 11</summary>
        CurrentSummationDeliveredBlock11 = 0x070A,
        /// <summary>Current Summation Delivered for Block 12</summary>
        CurrentSummationDeliveredBlock12 = 0x070B,
        /// <summary>Current Summation Delivered for Block 13</summary>
        CurrentSummationDeliveredBlock13 = 0x070C,
        /// <summary>Current Summation Delivered for Block 14</summary>
        CurrentSummationDeliveredBlock14 = 0x070D,
        /// <summary>Current Summation Delivered for Block 15</summary>
        CurrentSummationDeliveredBlock15 = 0x070E,
        /// <summary>Current Summation Delivered for Block 16</summary>
        CurrentSummationDeliveredBlock16 = 0x070F,

        /// <summary>Current Summation Delivered for Block 1 Tier 1</summary>
        CurrentSummationDeliveredBlock1Tier1 = 0x0710,
        /// <summary>Current Summation Delivered for Block 2 Tier 1</summary>
        CurrentSummationDeliveredBlock2Tier1 = 0x0711,
        /// <summary>Current Summation Delivered for Block 3 Tier 1</summary>
        CurrentSummationDeliveredBlock3Tier1 = 0x0712,
        /// <summary>Current Summation Delivered for Block 4 Tier 1</summary>
        CurrentSummationDeliveredBlock4Tier1 = 0x0713,
        /// <summary>Current Summation Delivered for Block 5 Tier 1</summary>
        CurrentSummationDeliveredBlock5Tier1 = 0x0714,
        /// <summary>Current Summation Delivered for Block 6 Tier 1</summary>
        CurrentSummationDeliveredBlock6Tier1 = 0x0715,
        /// <summary>Current Summation Delivered for Block 7 Tier 1</summary>
        CurrentSummationDeliveredBlock7Tier1 = 0x0716,
        /// <summary>Current Summation Delivered for Block 8 Tier 1</summary>
        CurrentSummationDeliveredBlock8Tier1 = 0x0717,
        /// <summary>Current Summation Delivered for Block 9 Tier 1</summary>
        CurrentSummationDeliveredBlock9Tier1 = 0x0718,
        /// <summary>Current Summation Delivered for Block 10 Tier 1</summary>
        CurrentSummationDeliveredBlock10Tier1 = 0x0719,
        /// <summary>Current Summation Delivered for Block 11 Tier 1</summary>
        CurrentSummationDeliveredBlock11Tier1 = 0x071A,
        /// <summary>Current Summation Delivered for Block 12 Tier 1</summary>
        CurrentSummationDeliveredBlock12Tier1 = 0x071B,
        /// <summary>Current Summation Delivered for Block 13 Tier 1</summary>
        CurrentSummationDeliveredBlock13Tier1 = 0x071C,
        /// <summary>Current Summation Delivered for Block 14 Tier 1</summary>
        CurrentSummationDeliveredBlock14Tier1 = 0x071D,
        /// <summary>Current Summation Delivered for Block 15 Tier 1</summary>
        CurrentSummationDeliveredBlock15Tier1 = 0x071E,
        /// <summary>Current Summation Delivered for Block 16 Tier 1</summary>
        CurrentSummationDeliveredBlock16Tier1 = 0x071F,

        /// <summary>Current Summation Delivered for Block 1 Tier 2</summary>
        CurrentSummationDeliveredBlock1Tier2 = 0x0720,
        /// <summary>Current Summation Delivered for Block 2 Tier 2</summary>
        CurrentSummationDeliveredBlock2Tier2 = 0x0721,
        /// <summary>Current Summation Delivered for Block 3 Tier 2</summary>
        CurrentSummationDeliveredBlock3Tier2 = 0x0722,
        /// <summary>Current Summation Delivered for Block 4 Tier 2</summary>
        CurrentSummationDeliveredBlock4Tier2 = 0x0723,
        /// <summary>Current Summation Delivered for Block 5 Tier 2</summary>
        CurrentSummationDeliveredBlock5Tier2 = 0x0724,
        /// <summary>Current Summation Delivered for Block 6 Tier 2</summary>
        CurrentSummationDeliveredBlock6Tier2 = 0x0725,
        /// <summary>Current Summation Delivered for Block 7 Tier 2</summary>
        CurrentSummationDeliveredBlock7Tier2 = 0x0726,
        /// <summary>Current Summation Delivered for Block 8 Tier 2</summary>
        CurrentSummationDeliveredBlock8Tier2 = 0x0727,
        /// <summary>Current Summation Delivered for Block 9 Tier 2</summary>
        CurrentSummationDeliveredBlock9Tier2 = 0x0728,
        /// <summary>Current Summation Delivered for Block 10 Tier 2</summary>
        CurrentSummationDeliveredBlock10Tier2 = 0x0729,
        /// <summary>Current Summation Delivered for Block 11 Tier 2</summary>
        CurrentSummationDeliveredBlock11Tier2 = 0x072A,
        /// <summary>Current Summation Delivered for Block 12 Tier 2</summary>
        CurrentSummationDeliveredBlock12Tier2 = 0x072B,
        /// <summary>Current Summation Delivered for Block 13 Tier 2</summary>
        CurrentSummationDeliveredBlock13Tier2 = 0x072C,
        /// <summary>Current Summation Delivered for Block 14 Tier 2</summary>
        CurrentSummationDeliveredBlock14Tier2 = 0x072D,
        /// <summary>Current Summation Delivered for Block 15 Tier 2</summary>
        CurrentSummationDeliveredBlock15Tier2 = 0x072E,
        /// <summary>Current Summation Delivered for Block 16 Tier 2</summary>
        CurrentSummationDeliveredBlock16Tier2 = 0x072F,

        /// <summary>Current Summation Delivered for Block 1 Tier 3</summary>
        CurrentSummationDeliveredBlock1Tier3 = 0x0730,
        /// <summary>Current Summation Delivered for Block 2 Tier 3</summary>
        CurrentSummationDeliveredBlock2Tier3 = 0x0731,
        /// <summary>Current Summation Delivered for Block 3 Tier 3</summary>
        CurrentSummationDeliveredBlock3Tier3 = 0x0732,
        /// <summary>Current Summation Delivered for Block 4 Tier 3</summary>
        CurrentSummationDeliveredBlock4Tier3 = 0x0733,
        /// <summary>Current Summation Delivered for Block 5 Tier 3</summary>
        CurrentSummationDeliveredBlock5Tier3 = 0x0734,
        /// <summary>Current Summation Delivered for Block 6 Tier 3</summary>
        CurrentSummationDeliveredBlock6Tier3 = 0x0735,
        /// <summary>Current Summation Delivered for Block 7 Tier 3</summary>
        CurrentSummationDeliveredBlock7Tier3 = 0x0736,
        /// <summary>Current Summation Delivered for Block 8 Tier 3</summary>
        CurrentSummationDeliveredBlock8Tier3 = 0x0737,
        /// <summary>Current Summation Delivered for Block 9 Tier 3</summary>
        CurrentSummationDeliveredBlock9Tier3 = 0x0738,
        /// <summary>Current Summation Delivered for Block 10 Tier 3</summary>
        CurrentSummationDeliveredBlock10Tier3 = 0x0739,
        /// <summary>Current Summation Delivered for Block 11 Tier 3</summary>
        CurrentSummationDeliveredBlock11Tier3 = 0x073A,
        /// <summary>Current Summation Delivered for Block 12 Tier 3</summary>
        CurrentSummationDeliveredBlock12Tier3 = 0x073B,
        /// <summary>Current Summation Delivered for Block 13 Tier 3</summary>
        CurrentSummationDeliveredBlock13Tier3 = 0x073C,
        /// <summary>Current Summation Delivered for Block 14 Tier 3</summary>
        CurrentSummationDeliveredBlock14Tier3 = 0x073D,
        /// <summary>Current Summation Delivered for Block 15 Tier 3</summary>
        CurrentSummationDeliveredBlock15Tier3 = 0x073E,
        /// <summary>Current Summation Delivered for Block 16 Tier 3</summary>
        CurrentSummationDeliveredBlock16Tier3 = 0x073F,

        /// <summary>Current Summation Delivered for Block 1 Tier 4</summary>
        CurrentSummationDeliveredBlock1Tier4 = 0x0740,
        /// <summary>Current Summation Delivered for Block 2 Tier 4</summary>
        CurrentSummationDeliveredBlock2Tier4 = 0x0741,
        /// <summary>Current Summation Delivered for Block 3 Tier 4</summary>
        CurrentSummationDeliveredBlock3Tier4 = 0x0742,
        /// <summary>Current Summation Delivered for Block 4 Tier 4</summary>
        CurrentSummationDeliveredBlock4Tier4 = 0x0743,
        /// <summary>Current Summation Delivered for Block 5 Tier 4</summary>
        CurrentSummationDeliveredBlock5Tier4 = 0x0744,
        /// <summary>Current Summation Delivered for Block 6 Tier 4</summary>
        CurrentSummationDeliveredBlock6Tier4 = 0x0745,
        /// <summary>Current Summation Delivered for Block 7 Tier 4</summary>
        CurrentSummationDeliveredBlock7Tier4 = 0x0746,
        /// <summary>Current Summation Delivered for Block 8 Tier 4</summary>
        CurrentSummationDeliveredBlock8Tier4 = 0x0747,
        /// <summary>Current Summation Delivered for Block 9 Tier 4</summary>
        CurrentSummationDeliveredBlock9Tier4 = 0x0748,
        /// <summary>Current Summation Delivered for Block 10 Tier 4</summary>
        CurrentSummationDeliveredBlock10Tier4 = 0x0749,
        /// <summary>Current Summation Delivered for Block 11 Tier 4</summary>
        CurrentSummationDeliveredBlock11Tier4 = 0x074A,
        /// <summary>Current Summation Delivered for Block 12 Tier 4</summary>
        CurrentSummationDeliveredBlock12Tier4 = 0x074B,
        /// <summary>Current Summation Delivered for Block 13 Tier 4</summary>
        CurrentSummationDeliveredBlock13Tier4 = 0x074C,
        /// <summary>Current Summation Delivered for Block 14 Tier 4</summary>
        CurrentSummationDeliveredBlock14Tier4 = 0x074D,
        /// <summary>Current Summation Delivered for Block 15 Tier 4</summary>
        CurrentSummationDeliveredBlock15Tier4 = 0x074E,
        /// <summary>Current Summation Delivered for Block 16 Tier 4</summary>
        CurrentSummationDeliveredBlock16Tier4 = 0x074F,

        /// <summary>Current Summation Delivered for Block 1 Tier 5</summary>
        CurrentSummationDeliveredBlock1Tier5 = 0x0750,
        /// <summary>Current Summation Delivered for Block 2 Tier 5</summary>
        CurrentSummationDeliveredBlock2Tier5 = 0x0751,
        /// <summary>Current Summation Delivered for Block 3 Tier 5</summary>
        CurrentSummationDeliveredBlock3Tier5 = 0x0752,
        /// <summary>Current Summation Delivered for Block 4 Tier 5</summary>
        CurrentSummationDeliveredBlock4Tier5 = 0x0753,
        /// <summary>Current Summation Delivered for Block 5 Tier 5</summary>
        CurrentSummationDeliveredBlock5Tier5 = 0x0754,
        /// <summary>Current Summation Delivered for Block 6 Tier 5</summary>
        CurrentSummationDeliveredBlock6Tier5 = 0x0755,
        /// <summary>Current Summation Delivered for Block 7 Tier 5</summary>
        CurrentSummationDeliveredBlock7Tier5 = 0x0756,
        /// <summary>Current Summation Delivered for Block 8 Tier 5</summary>
        CurrentSummationDeliveredBlock8Tier5 = 0x0757,
        /// <summary>Current Summation Delivered for Block 9 Tier 5</summary>
        CurrentSummationDeliveredBlock9Tier5 = 0x0758,
        /// <summary>Current Summation Delivered for Block 10 Tier 5</summary>
        CurrentSummationDeliveredBlock10Tier5 = 0x0759,
        /// <summary>Current Summation Delivered for Block 11 Tier 5</summary>
        CurrentSummationDeliveredBlock11Tier5 = 0x075A,
        /// <summary>Current Summation Delivered for Block 12 Tier 5</summary>
        CurrentSummationDeliveredBlock12Tier5 = 0x075B,
        /// <summary>Current Summation Delivered for Block 13 Tier 5</summary>
        CurrentSummationDeliveredBlock13Tier5 = 0x075C,
        /// <summary>Current Summation Delivered for Block 14 Tier 5</summary>
        CurrentSummationDeliveredBlock14Tier5 = 0x075D,
        /// <summary>Current Summation Delivered for Block 15 Tier 5</summary>
        CurrentSummationDeliveredBlock15Tier5 = 0x075E,
        /// <summary>Current Summation Delivered for Block 16 Tier 5</summary>
        CurrentSummationDeliveredBlock16Tier5 = 0x075F,

        /// <summary>Current Summation Delivered for Block 1 Tier 6</summary>
        CurrentSummationDeliveredBlock1Tier6 = 0x0760,
        /// <summary>Current Summation Delivered for Block 2 Tier 6</summary>
        CurrentSummationDeliveredBlock2Tier6 = 0x0761,
        /// <summary>Current Summation Delivered for Block 3 Tier 6</summary>
        CurrentSummationDeliveredBlock3Tier6 = 0x0762,
        /// <summary>Current Summation Delivered for Block 4 Tier 6</summary>
        CurrentSummationDeliveredBlock4Tier6 = 0x0763,
        /// <summary>Current Summation Delivered for Block 5 Tier 6</summary>
        CurrentSummationDeliveredBlock5Tier6 = 0x0764,
        /// <summary>Current Summation Delivered for Block 6 Tier 6</summary>
        CurrentSummationDeliveredBlock6Tier6 = 0x0765,
        /// <summary>Current Summation Delivered for Block 7 Tier 6</summary>
        CurrentSummationDeliveredBlock7Tier6 = 0x0766,
        /// <summary>Current Summation Delivered for Block 8 Tier 6</summary>
        CurrentSummationDeliveredBlock8Tier6 = 0x0767,
        /// <summary>Current Summation Delivered for Block 9 Tier 6</summary>
        CurrentSummationDeliveredBlock9Tier6 = 0x0768,
        /// <summary>Current Summation Delivered for Block 10 Tier 6</summary>
        CurrentSummationDeliveredBlock10Tier6 = 0x0769,
        /// <summary>Current Summation Delivered for Block 11 Tier 6</summary>
        CurrentSummationDeliveredBlock11Tier6 = 0x076A,
        /// <summary>Current Summation Delivered for Block 12 Tier 6</summary>
        CurrentSummationDeliveredBlock12Tier6 = 0x076B,
        /// <summary>Current Summation Delivered for Block 13 Tier 6</summary>
        CurrentSummationDeliveredBlock13Tier6 = 0x076C,
        /// <summary>Current Summation Delivered for Block 14 Tier 6</summary>
        CurrentSummationDeliveredBlock14Tier6 = 0x076D,
        /// <summary>Current Summation Delivered for Block 15 Tier 6</summary>
        CurrentSummationDeliveredBlock15Tier6 = 0x076E,
        /// <summary>Current Summation Delivered for Block 16 Tier 6</summary>
        CurrentSummationDeliveredBlock16Tier6 = 0x076F,

        /// <summary>Current Summation Delivered for Block 1 Tier 7</summary>
        CurrentSummationDeliveredBlock1Tier7 = 0x0770,
        /// <summary>Current Summation Delivered for Block 2 Tier 7</summary>
        CurrentSummationDeliveredBlock2Tier7 = 0x0771,
        /// <summary>Current Summation Delivered for Block 3 Tier 7</summary>
        CurrentSummationDeliveredBlock3Tier7 = 0x0772,
        /// <summary>Current Summation Delivered for Block 4 Tier 7</summary>
        CurrentSummationDeliveredBlock4Tier7 = 0x0773,
        /// <summary>Current Summation Delivered for Block 5 Tier 7</summary>
        CurrentSummationDeliveredBlock5Tier7 = 0x0774,
        /// <summary>Current Summation Delivered for Block 6 Tier 7</summary>
        CurrentSummationDeliveredBlock6Tier7 = 0x0775,
        /// <summary>Current Summation Delivered for Block 7 Tier 7</summary>
        CurrentSummationDeliveredBlock7Tier7 = 0x0776,
        /// <summary>Current Summation Delivered for Block 8 Tier 7</summary>
        CurrentSummationDeliveredBlock8Tier7 = 0x0777,
        /// <summary>Current Summation Delivered for Block 9 Tier 7</summary>
        CurrentSummationDeliveredBlock9Tier7 = 0x0778,
        /// <summary>Current Summation Delivered for Block 10 Tier 7</summary>
        CurrentSummationDeliveredBlock10Tier7 = 0x0779,
        /// <summary>Current Summation Delivered for Block 11 Tier 7</summary>
        CurrentSummationDeliveredBlock11Tier7 = 0x077A,
        /// <summary>Current Summation Delivered for Block 12 Tier 7</summary>
        CurrentSummationDeliveredBlock12Tier7 = 0x077B,
        /// <summary>Current Summation Delivered for Block 13 Tier 7</summary>
        CurrentSummationDeliveredBlock13Tier7 = 0x077C,
        /// <summary>Current Summation Delivered for Block 14 Tier 7</summary>
        CurrentSummationDeliveredBlock14Tier7 = 0x077D,
        /// <summary>Current Summation Delivered for Block 15 Tier 7</summary>
        CurrentSummationDeliveredBlock15Tier7 = 0x077E,
        /// <summary>Current Summation Delivered for Block 16 Tier 7</summary>
        CurrentSummationDeliveredBlock16Tier7 = 0x077F,

        /// <summary>Current Summation Delivered for Block 1 Tier 8</summary>
        CurrentSummationDeliveredBlock1Tier8 = 0x0780,
        /// <summary>Current Summation Delivered for Block 2 Tier 8</summary>
        CurrentSummationDeliveredBlock2Tier8 = 0x0781,
        /// <summary>Current Summation Delivered for Block 3 Tier 8</summary>
        CurrentSummationDeliveredBlock3Tier8 = 0x0782,
        /// <summary>Current Summation Delivered for Block 4 Tier 8</summary>
        CurrentSummationDeliveredBlock4Tier8 = 0x0783,
        /// <summary>Current Summation Delivered for Block 5 Tier 8</summary>
        CurrentSummationDeliveredBlock5Tier8 = 0x0784,
        /// <summary>Current Summation Delivered for Block 6 Tier 8</summary>
        CurrentSummationDeliveredBlock6Tier8 = 0x0785,
        /// <summary>Current Summation Delivered for Block 7 Tier 8</summary>
        CurrentSummationDeliveredBlock7Tier8 = 0x0786,
        /// <summary>Current Summation Delivered for Block 8 Tier 8</summary>
        CurrentSummationDeliveredBlock8Tier8 = 0x0787,
        /// <summary>Current Summation Delivered for Block 9 Tier 8</summary>
        CurrentSummationDeliveredBlock9Tier8 = 0x0788,
        /// <summary>Current Summation Delivered for Block 10 Tier 8</summary>
        CurrentSummationDeliveredBlock10Tier8 = 0x0789,
        /// <summary>Current Summation Delivered for Block 11 Tier 8</summary>
        CurrentSummationDeliveredBlock11Tier8 = 0x078A,
        /// <summary>Current Summation Delivered for Block 12 Tier 8</summary>
        CurrentSummationDeliveredBlock12Tier8 = 0x078B,
        /// <summary>Current Summation Delivered for Block 13 Tier 8</summary>
        CurrentSummationDeliveredBlock13Tier8 = 0x078C,
        /// <summary>Current Summation Delivered for Block 14 Tier 8</summary>
        CurrentSummationDeliveredBlock14Tier8 = 0x078D,
        /// <summary>Current Summation Delivered for Block 15 Tier 8</summary>
        CurrentSummationDeliveredBlock15Tier8 = 0x078E,
        /// <summary>Current Summation Delivered for Block 16 Tier 8</summary>
        CurrentSummationDeliveredBlock16Tier8 = 0x078F,

        /// <summary>Current Summation Delivered for Block 1 Tier 9</summary>
        CurrentSummationDeliveredBlock1Tier9 = 0x0790,
        /// <summary>Current Summation Delivered for Block 2 Tier 9</summary>
        CurrentSummationDeliveredBlock2Tier9 = 0x0791,
        /// <summary>Current Summation Delivered for Block 3 Tier 9</summary>
        CurrentSummationDeliveredBlock3Tier9 = 0x0792,
        /// <summary>Current Summation Delivered for Block 4 Tier 9</summary>
        CurrentSummationDeliveredBlock4Tier9 = 0x0793,
        /// <summary>Current Summation Delivered for Block 5 Tier 9</summary>
        CurrentSummationDeliveredBlock5Tier9 = 0x0794,
        /// <summary>Current Summation Delivered for Block 6 Tier 9</summary>
        CurrentSummationDeliveredBlock6Tier9 = 0x0795,
        /// <summary>Current Summation Delivered for Block 7 Tier 9</summary>
        CurrentSummationDeliveredBlock7Tier9 = 0x0796,
        /// <summary>Current Summation Delivered for Block 8 Tier 9</summary>
        CurrentSummationDeliveredBlock8Tier9 = 0x0797,
        /// <summary>Current Summation Delivered for Block 9 Tier 9</summary>
        CurrentSummationDeliveredBlock9Tier9 = 0x0798,
        /// <summary>Current Summation Delivered for Block 10 Tier 9</summary>
        CurrentSummationDeliveredBlock10Tier9 = 0x0799,
        /// <summary>Current Summation Delivered for Block 11 Tier 9</summary>
        CurrentSummationDeliveredBlock11Tier9 = 0x079A,
        /// <summary>Current Summation Delivered for Block 12 Tier 9</summary>
        CurrentSummationDeliveredBlock12Tier9 = 0x079B,
        /// <summary>Current Summation Delivered for Block 13 Tier 9</summary>
        CurrentSummationDeliveredBlock13Tier9 = 0x079C,
        /// <summary>Current Summation Delivered for Block 14 Tier 9</summary>
        CurrentSummationDeliveredBlock14Tier9 = 0x079D,
        /// <summary>Current Summation Delivered for Block 15 Tier 9</summary>
        CurrentSummationDeliveredBlock15Tier9 = 0x079E,
        /// <summary>Current Summation Delivered for Block 16 Tier 9</summary>
        CurrentSummationDeliveredBlock16Tier9 = 0x079F,

        /// <summary>Current Summation Delivered for Block 1 Tier 10</summary>
        CurrentSummationDeliveredBlock1Tier10 = 0x07A0,
        /// <summary>Current Summation Delivered for Block 2 Tier 10</summary>
        CurrentSummationDeliveredBlock2Tier10 = 0x07A1,
        /// <summary>Current Summation Delivered for Block 3 Tier 10</summary>
        CurrentSummationDeliveredBlock3Tier10 = 0x07A2,
        /// <summary>Current Summation Delivered for Block 4 Tier 10</summary>
        CurrentSummationDeliveredBlock4Tier10 = 0x07A3,
        /// <summary>Current Summation Delivered for Block 5 Tier 10</summary>
        CurrentSummationDeliveredBlock5Tier10 = 0x07A4,
        /// <summary>Current Summation Delivered for Block 6 Tier 10</summary>
        CurrentSummationDeliveredBlock6Tier10 = 0x07A5,
        /// <summary>Current Summation Delivered for Block 7 Tier 10</summary>
        CurrentSummationDeliveredBlock7Tier10 = 0x07A6,
        /// <summary>Current Summation Delivered for Block 8 Tier 10</summary>
        CurrentSummationDeliveredBlock8Tier10 = 0x07A7,
        /// <summary>Current Summation Delivered for Block 9 Tier 10</summary>
        CurrentSummationDeliveredBlock9Tier10 = 0x07A8,
        /// <summary>Current Summation Delivered for Block 10 Tier 10</summary>
        CurrentSummationDeliveredBlock10Tier10 = 0x07A9,
        /// <summary>Current Summation Delivered for Block 11 Tier 10</summary>
        CurrentSummationDeliveredBlock11Tier10 = 0x07AA,
        /// <summary>Current Summation Delivered for Block 12 Tier 10</summary>
        CurrentSummationDeliveredBlock12Tier10 = 0x07AB,
        /// <summary>Current Summation Delivered for Block 13 Tier 10</summary>
        CurrentSummationDeliveredBlock13Tier10 = 0x07AC,
        /// <summary>Current Summation Delivered for Block 14 Tier 10</summary>
        CurrentSummationDeliveredBlock14Tier10 = 0x07AD,
        /// <summary>Current Summation Delivered for Block 15 Tier 10</summary>
        CurrentSummationDeliveredBlock15Tier10 = 0x07AE,
        /// <summary>Current Summation Delivered for Block 16 Tier 10</summary>
        CurrentSummationDeliveredBlock16Tier10 = 0x07AF,

        /// <summary>Current Summation Delivered for Block 1 Tier 11</summary>
        CurrentSummationDeliveredBlock1Tier11 = 0x07B0,
        /// <summary>Current Summation Delivered for Block 2 Tier 11</summary>
        CurrentSummationDeliveredBlock2Tier11 = 0x07B1,
        /// <summary>Current Summation Delivered for Block 3 Tier 11</summary>
        CurrentSummationDeliveredBlock3Tier11 = 0x07B2,
        /// <summary>Current Summation Delivered for Block 4 Tier 11</summary>
        CurrentSummationDeliveredBlock4Tier11 = 0x07B3,
        /// <summary>Current Summation Delivered for Block 5 Tier 11</summary>
        CurrentSummationDeliveredBlock5Tier11 = 0x07B4,
        /// <summary>Current Summation Delivered for Block 6 Tier 11</summary>
        CurrentSummationDeliveredBlock6Tier11 = 0x07B5,
        /// <summary>Current Summation Delivered for Block 7 Tier 11</summary>
        CurrentSummationDeliveredBlock7Tier11 = 0x07B6,
        /// <summary>Current Summation Delivered for Block 8 Tier 11</summary>
        CurrentSummationDeliveredBlock8Tier11 = 0x07B7,
        /// <summary>Current Summation Delivered for Block 9 Tier 11</summary>
        CurrentSummationDeliveredBlock9Tier11 = 0x07B8,
        /// <summary>Current Summation Delivered for Block 10 Tier 11</summary>
        CurrentSummationDeliveredBlock10Tier11 = 0x07B9,
        /// <summary>Current Summation Delivered for Block 11 Tier 11</summary>
        CurrentSummationDeliveredBlock11Tier11 = 0x07BA,
        /// <summary>Current Summation Delivered for Block 12 Tier 11</summary>
        CurrentSummationDeliveredBlock12Tier11 = 0x07BB,
        /// <summary>Current Summation Delivered for Block 13 Tier 11</summary>
        CurrentSummationDeliveredBlock13Tier11 = 0x07BC,
        /// <summary>Current Summation Delivered for Block 14 Tier 11</summary>
        CurrentSummationDeliveredBlock14Tier11 = 0x07BD,
        /// <summary>Current Summation Delivered for Block 15 Tier 11</summary>
        CurrentSummationDeliveredBlock15Tier11 = 0x07BE,
        /// <summary>Current Summation Delivered for Block 16 Tier 11</summary>
        CurrentSummationDeliveredBlock16Tier11 = 0x07BF,

        /// <summary>Current Summation Delivered for Block 1 Tier 12</summary>
        CurrentSummationDeliveredBlock1Tier12 = 0x07C0,
        /// <summary>Current Summation Delivered for Block 2 Tier 12</summary>
        CurrentSummationDeliveredBlock2Tier12 = 0x07C1,
        /// <summary>Current Summation Delivered for Block 3 Tier 12</summary>
        CurrentSummationDeliveredBlock3Tier12 = 0x07C2,
        /// <summary>Current Summation Delivered for Block 4 Tier 12</summary>
        CurrentSummationDeliveredBlock4Tier12 = 0x07C3,
        /// <summary>Current Summation Delivered for Block 5 Tier 12</summary>
        CurrentSummationDeliveredBlock5Tier12 = 0x07C4,
        /// <summary>Current Summation Delivered for Block 6 Tier 12</summary>
        CurrentSummationDeliveredBlock6Tier12 = 0x07C5,
        /// <summary>Current Summation Delivered for Block 7 Tier 12</summary>
        CurrentSummationDeliveredBlock7Tier12 = 0x07C6,
        /// <summary>Current Summation Delivered for Block 8 Tier 12</summary>
        CurrentSummationDeliveredBlock8Tier12 = 0x07C7,
        /// <summary>Current Summation Delivered for Block 9 Tier 12</summary>
        CurrentSummationDeliveredBlock9Tier12 = 0x07C8,
        /// <summary>Current Summation Delivered for Block 10 Tier 12</summary>
        CurrentSummationDeliveredBlock10Tier12 = 0x07C9,
        /// <summary>Current Summation Delivered for Block 11 Tier 12</summary>
        CurrentSummationDeliveredBlock11Tier12 = 0x07CA,
        /// <summary>Current Summation Delivered for Block 12 Tier 12</summary>
        CurrentSummationDeliveredBlock12Tier12 = 0x07CB,
        /// <summary>Current Summation Delivered for Block 13 Tier 12</summary>
        CurrentSummationDeliveredBlock13Tier12 = 0x07CC,
        /// <summary>Current Summation Delivered for Block 14 Tier 12</summary>
        CurrentSummationDeliveredBlock14Tier12 = 0x07CD,
        /// <summary>Current Summation Delivered for Block 15 Tier 12</summary>
        CurrentSummationDeliveredBlock15Tier12 = 0x07CE,
        /// <summary>Current Summation Delivered for Block 16 Tier 12</summary>
        CurrentSummationDeliveredBlock16Tier12 = 0x07CF,


        /// <summary>Current Summation Delivered for Block 1 Tier 13</summary>
        CurrentSummationDeliveredBlock1Tier13 = 0x07D0,
        /// <summary>Current Summation Delivered for Block 2 Tier 13</summary>
        CurrentSummationDeliveredBlock2Tier13 = 0x07D1,
        /// <summary>Current Summation Delivered for Block 3 Tier 13</summary>
        CurrentSummationDeliveredBlock3Tier13 = 0x07D2,
        /// <summary>Current Summation Delivered for Block 4 Tier 13</summary>
        CurrentSummationDeliveredBlock4Tier13 = 0x07D3,
        /// <summary>Current Summation Delivered for Block 5 Tier 13</summary>
        CurrentSummationDeliveredBlock5Tier13 = 0x07D4,
        /// <summary>Current Summation Delivered for Block 6 Tier 13</summary>
        CurrentSummationDeliveredBlock6Tier13 = 0x07D5,
        /// <summary>Current Summation Delivered for Block 7 Tier 13</summary>
        CurrentSummationDeliveredBlock7Tier13 = 0x07D6,
        /// <summary>Current Summation Delivered for Block 8 Tier 13</summary>
        CurrentSummationDeliveredBlock8Tier13 = 0x07D7,
        /// <summary>Current Summation Delivered for Block 9 Tier 13</summary>
        CurrentSummationDeliveredBlock9Tier13 = 0x07D8,
        /// <summary>Current Summation Delivered for Block 10 Tier 13</summary>
        CurrentSummationDeliveredBlock10Tier13 = 0x07D9,
        /// <summary>Current Summation Delivered for Block 11 Tier 13</summary>
        CurrentSummationDeliveredBlock11Tier13 = 0x07DA,
        /// <summary>Current Summation Delivered for Block 12 Tier 13</summary>
        CurrentSummationDeliveredBlock12Tier13 = 0x07DB,
        /// <summary>Current Summation Delivered for Block 13 Tier 13</summary>
        CurrentSummationDeliveredBlock13Tier13 = 0x07DC,
        /// <summary>Current Summation Delivered for Block 14 Tier 13</summary>
        CurrentSummationDeliveredBlock14Tier13 = 0x07DD,
        /// <summary>Current Summation Delivered for Block 15 Tier 13</summary>
        CurrentSummationDeliveredBlock15Tier13 = 0x07DE,
        /// <summary>Current Summation Delivered for Block 16 Tier 13</summary>
        CurrentSummationDeliveredBlock16Tier13 = 0x07DF,


        /// <summary>Current Summation Delivered for Block 1 Tier 14</summary>
        CurrentSummationDeliveredBlock1Tier14 = 0x07E0,
        /// <summary>Current Summation Delivered for Block 2 Tier 14</summary>
        CurrentSummationDeliveredBlock2Tier14 = 0x07E1,
        /// <summary>Current Summation Delivered for Block 3 Tier 14</summary>
        CurrentSummationDeliveredBlock3Tier14 = 0x07E2,
        /// <summary>Current Summation Delivered for Block 4 Tier 14</summary>
        CurrentSummationDeliveredBlock4Tier14 = 0x07E3,
        /// <summary>Current Summation Delivered for Block 5 Tier 14</summary>
        CurrentSummationDeliveredBlock5Tier14 = 0x07E4,
        /// <summary>Current Summation Delivered for Block 6 Tier 14</summary>
        CurrentSummationDeliveredBlock6Tier14 = 0x07E5,
        /// <summary>Current Summation Delivered for Block 7 Tier 14</summary>
        CurrentSummationDeliveredBlock7Tier14 = 0x07E6,
        /// <summary>Current Summation Delivered for Block 8 Tier 14</summary>
        CurrentSummationDeliveredBlock8Tier14 = 0x07E7,
        /// <summary>Current Summation Delivered for Block 9 Tier 14</summary>
        CurrentSummationDeliveredBlock9Tier14 = 0x07E8,
        /// <summary>Current Summation Delivered for Block 10 Tier 14</summary>
        CurrentSummationDeliveredBlock10Tier14 = 0x07E9,
        /// <summary>Current Summation Delivered for Block 11 Tier 14</summary>
        CurrentSummationDeliveredBlock11Tier14 = 0x07EA,
        /// <summary>Current Summation Delivered for Block 12 Tier 14</summary>
        CurrentSummationDeliveredBlock12Tier14 = 0x07EB,
        /// <summary>Current Summation Delivered for Block 13 Tier 14</summary>
        CurrentSummationDeliveredBlock13Tier14 = 0x07EC,
        /// <summary>Current Summation Delivered for Block 14 Tier 14</summary>
        CurrentSummationDeliveredBlock14Tier14 = 0x07ED,
        /// <summary>Current Summation Delivered for Block 15 Tier 14</summary>
        CurrentSummationDeliveredBlock15Tier14 = 0x07EE,
        /// <summary>Current Summation Delivered for Block 16 Tier 14</summary>
        CurrentSummationDeliveredBlock16Tier14 = 0x07EF,


        /// <summary>Current Summation Delivered for Block 1 Tier 15</summary>
        CurrentSummationDeliveredBlock1Tier15 = 0x07F0,
        /// <summary>Current Summation Delivered for Block 2 Tier 15</summary>
        CurrentSummationDeliveredBlock2Tier15 = 0x07F1,
        /// <summary>Current Summation Delivered for Block 3 Tier 15</summary>
        CurrentSummationDeliveredBlock3Tier15 = 0x07F2,
        /// <summary>Current Summation Delivered for Block 4 Tier 15</summary>
        CurrentSummationDeliveredBlock4Tier15 = 0x07F3,
        /// <summary>Current Summation Delivered for Block 5 Tier 15</summary>
        CurrentSummationDeliveredBlock5Tier15 = 0x07F4,
        /// <summary>Current Summation Delivered for Block 6 Tier 15</summary>
        CurrentSummationDeliveredBlock6Tier15 = 0x07F5,
        /// <summary>Current Summation Delivered for Block 7 Tier 15</summary>
        CurrentSummationDeliveredBlock7Tier15 = 0x07F6,
        /// <summary>Current Summation Delivered for Block 8 Tier 15</summary>
        CurrentSummationDeliveredBlock8Tier15 = 0x07F7,
        /// <summary>Current Summation Delivered for Block 9 Tier 15</summary>
        CurrentSummationDeliveredBlock9Tier15 = 0x07F8,
        /// <summary>Current Summation Delivered for Block 10 Tier 15</summary>
        CurrentSummationDeliveredBlock10Tier15 = 0x07F9,
        /// <summary>Current Summation Delivered for Block 11 Tier 15</summary>
        CurrentSummationDeliveredBlock11Tier15 = 0x07FA,
        /// <summary>Current Summation Delivered for Block 12 Tier 15</summary>
        CurrentSummationDeliveredBlock12Tier15 = 0x07FB,
        /// <summary>Current Summation Delivered for Block 13 Tier 15</summary>
        CurrentSummationDeliveredBlock13Tier15 = 0x07FC,
        /// <summary>Current Summation Delivered for Block 14 Tier 15</summary>
        CurrentSummationDeliveredBlock14Tier15 = 0x07FD,
        /// <summary>Current Summation Delivered for Block 15 Tier 15</summary>
        CurrentSummationDeliveredBlock15Tier15 = 0x07FE,
        /// <summary>Current Summation Delivered for Block 16 Tier 15</summary>
        CurrentSummationDeliveredBlock16Tier15 = 0x07FF,

        #endregion

        #region Alarms
        /// <summary>Generic Alarm Mask</summary>
        GenericAlarmMask = 0x0800,
        /// <summary>Electricity Alarm Mask</summary>
        ElectricityAlarmMask = 0x0801,
        /// <summary>Generic Flow/Pressure Alarm Mask</summary>
        GenericFlowAndPressureAlarmMask = 0x0802,
        /// <summary>Water Alarm Mask</summary>
        WaterSpecificAlarmMask = 0x0803,
        /// <summary>Heating and Cooling Specific Alarm Mask</summary>
        HeatAndCoolingSpecificAlarmMask = 0x0804,
        /// <summary>Gas Specific Alarm Mask</summary>
        GasSpecificAlarmMask = 0x0805,

        #endregion
    }

    /// <summary>
    /// Simple Metering commands available to the server
    /// </summary>
    public enum SimpleMeteringServerCommands : byte
    {
        /// <summary>Response command for a get load profile request</summary>
        GetProfileResponse = 0x00,
        /// <summary>Request to mirror the metering device data</summary>
        RequestMirror = 0x01,
        /// <summary>Request to remove the mirror of the metering device data</summary>
        RemoveMirror = 0x02,
        /// <summary>Response to the fast poll mode request</summary>
        RequestFastPollModeResponse = 0x03,
    }

    /// <summary>
    /// Simple Metering commands available to the client
    /// </summary>
    public enum SimpleMeteringClientCommands : byte
    {
        /// <summary>Request load profile data from the meter</summary>
        GetProfile = 0x00,
        /// <summary>Response to the Request Mirror command</summary>
        RequestMirrorResponse = 0x01,
        /// <summary>Command to inform that the device has stopped mirroring metering data</summary>
        MirrorRemoved = 0x02,
        /// <summary>Request fast poll mode</summary>
        RequestFastPollMode = 0x03,
    }

    /// <summary>
    /// The Load Profile Channels which can be retrieved using the Simple Metering Commands
    /// </summary>
    public enum SimpleMeteringProfileChannel : byte
    {
        /// <summary>Consumption Delivered</summary>
        ConsumptionDelivered = 0x00,
        /// <summary>Consumption Received</summary>
        ConsumptionReceived = 0x01,
    }

    /// <summary>
    /// Server side Messaging Command ID's
    /// </summary>
    public enum MessagingServerCommands : byte
    {
        /// <summary>Display Message</summary>
        DisplayMessage = 0x00,
        /// <summary>Cancel Message</summary>
        CancelMessage = 0x01,
    }

    /// <summary>
    /// Client side Messaging Command ID's
    /// </summary>
    public enum MessagingClientCommands : byte
    {
        /// <summary>Get Last Message</summary>
        GetLastMessage = 0x00,
        /// <summary>Message Confirmation</summary>
        MessageConfirmation = 0x01,
    }

    /// <summary>
    /// Server side Price Command ID's
    /// </summary>
    public enum PriceServerCommands : byte
    {
        /// <summary>Publish Price</summary>
        PublishPrice = 0x00,
        /// <summary>Publish Block Period</summary>
        PublishBlockPeriod = 0x01,
    }

    /// <summary>
    /// Client side Price Command ID's
    /// </summary>
    public enum PriceClientCommands : byte
    {
        /// <summary>Get Current Price</summary>
        GetCurrentPrice = 0x00,
        /// <summary>Get Scheduled Prices</summary>
        GetScheduledPrices = 0x01,
        /// <summary>Price Acknowlegement</summary>
        PriceAcknowledgement = 0x02,
        /// <summary>Get Block Period(s)</summary>
        GetBlockPeriods = 0x03,
    }

    #endregion

    #region Event Delegates

    /// <summary>
    /// Delegate for the OTA Cluster Attribute Request Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void OTAAttributeRequestEventHandler(object sender, AttributeResponsePayload e);

    /// <summary>
    /// Delegate for the Messaging Cluster Display Message Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void DisplayMessageEventHandler(object sender, DisplayMessageEventArgs e);


    /// <summary>
    /// Delegate for the OTA Cluster Message Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void OTARespEventHandler(object sender, OTARespEventArgs e);



    /// <summary>
    /// Delegate for the OTA Cluster Notification Image Message Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void OTANotificationImageEventHandler(object sender, ImageNotifyPayload e);


    /// <summary>
    /// Delegate for the OTA Cluster Notification Upgrade end response Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void OTAUpgradeEndResponseEventHandler(object sender, UpgradeEndResponsePayload e);


    /// <summary>
    /// Delegate for the OTA Cluster Next Image Message received Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void OTAQueryNextImageRespEventHandler(object sender, QueryNextImageRespPayload e);


    /// <summary>
    /// Delegate for the OTA Cluster Next Block Message recived Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void OTAQueryNextBlockRespEventHandler(object sender, QueryNextBlockRespPayload e);


    /// <summary>
    /// Delegate for the Messaging Cluster Cancel Message Event Handler
    /// </summary>
    public delegate void CancelMessageEventHandler(object sender, CancelMessageEventArgs e);

    /// <summary>
    /// Delegate for the Price Cluster Publish Price Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void PublishPriceEventHandler(object sender, PublishPriceEventArgs e);

    /// <summary>
    /// Delegate for the Price Cluster Publish Block Period Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void PublishBlockPeriodEventHandler(object sender, PublishBlockPeriodEventArgs e);

    /// <summary>
    /// Delegate for the Mirror Response Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    /// 
    public delegate void MirrorResponseEventHandler(object sender, MirrorResponseEventArgs e);

    /// <summary>
    /// Delegate for the Mirror Removed Response Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    /// 
    public delegate void MirrorRemovedResponseEventHandler(object sender, MirrorRemovedResponseEventArgs e);

    /// <summary>
    /// Delegate for the Fast Polling request Response Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    /// 
    public delegate void FastPollingRequestResponseEventHandler(object sender, FastPollingRequestResponseEventArgs e);

    /// <summary>
    /// Delegate for the DRLCResponse Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    /// 
    public delegate void DRLCResponseEventHandler(object sender, DRLCResponseEventArgs e);

    #endregion

    /// <summary>
    /// Smart Energy 1.1 ZigBee Application Layer
    /// </summary>
    public class SE11ZigBeeApplication : ZigBeeApplication
    {
        #region Public Events

        /// <summary>
        /// Event to fire when an OTA Attribute Request message command is received.
        /// </summary>
        public event OTAAttributeRequestEventHandler OTAAttributeRequest;

        /// <summary>
        /// Event to fire when a Messaging Cluster Display message command is received.
        /// </summary>
        public event DisplayMessageEventHandler DisplayMessageReceived;

        /// <summary>
        /// Event to fire when a Messaging Cluster Cancel message command is received.
        /// </summary>
        public event CancelMessageEventHandler CancelMessageReceived;

        /// <summary>
        /// Event to fire when a Messaging Cluster Cancel message command is received.
        /// </summary>
        public event OTARespEventHandler OTARespReceived;

        /// <summary>
        /// Event to fire when a Messaging Cluster OTA with ImageNotify command is received.
        /// </summary>
        public event OTANotificationImageEventHandler OTANotificationImageReceived;

        /// <summary>
        /// Event to fire when a Messaging Cluster OTA with Upgrade End response command is received.
        /// </summary>
        public event OTAUpgradeEndResponseEventHandler OTAUpgradeEndResponseReceived;

        /// <summary>
        /// Event to fire when a Messaging Cluster OTA with ImageNotify command is received.
        /// </summary>
        public event OTAQueryNextImageRespEventHandler OTAQueryNextImageRespReceived;

        /// <summary>
        /// Event to fire when a Messaging Cluster OTA with ImageNotify command is received.
        /// </summary>
        public event OTAQueryNextBlockRespEventHandler OTAQueryNextBlockRespReceived;

        /// <summary>
        /// Event to fire when a Price Cluster Publish Price command is received.
        /// </summary>
        public event PublishPriceEventHandler PublishPriceReceived;

        /// <summary>
        /// Event to fire when a Price Cluster Publish Block Period command is received.
        /// </summary>
        public event PublishBlockPeriodEventHandler PublishBlockPeriodReceived;

        /// <summary>
        /// Event to fire when a Mirror Response command is received.
        /// </summary>
        public event MirrorResponseEventHandler MirrorResponseReceived;

        /// <summary>
        /// Event to fire when a Mirror Response command is received.
        /// </summary>
        public event MirrorRemovedResponseEventHandler MirrorRemovedResponseReceived;

        /// <summary>
        /// Event to fire when a Fast Polling command is received.
        /// </summary>
        public event FastPollingRequestResponseEventHandler FastPollingRequestResponseReceived;

        /// <summary>
        /// Event to fire when a DRLC command is received.
        /// </summary>
        public event DRLCResponseEventHandler DRLCResponseReceived;

        #endregion

        #region Constants

        private const byte DEFAULT_SE_ENDPOINT = 0x0A;
        private const ushort SE_PROFILE_ID = 0x0109;
        private const byte DEFAULT_SE_DEVICE_VERSION = 2;
        private const ushort DEFAULT_SE_DEVICE_ID = 0x0502;

        private const byte DRLC_SIGNATURE_TYPE = 0x01; // ECDSA signature type

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public SE11ZigBeeApplication()
            : base()
        {
            m_DRLCScheduledEventHandler = new EventHandler(HandleDRLCScheduledEvent);

            m_DRLCDeviceClass = DRLCDeviceClasses.All;
            m_DRLCEnrollmentGroup = 0;
            m_DRLCStartRandomizeMinutes = 0;
            m_DRLCStopRandomizeMinutes = 0;
            MeterPhysicalEnvironment = 0x01; // <-- Need to set before key establishment, so it is here in the constructor.

            m_CurrentSummationDelivered = 0;
            m_CurrentSummationReceived = 0;
            m_CurrentMaxDemandDelivered = 0;
            m_CurrentMaxDemandReceived = 0;
            m_DFTSummation = 0;
            m_DailyFreezeTime = 0;
            m_PowerFactor = 0;
            m_ReadingSnapShotTime = UTC_REFERENCE_TIME;
            m_CurrentMaxDemandDeliveredTime = UTC_REFERENCE_TIME;
            m_CurrentMaxDemandReceivedTime = UTC_REFERENCE_TIME;
            m_DefaultUpdatePeriod = 0;
            m_FastPollUpdatePeriod = 0;

            m_UnitOfMeasure = 0x8B; // unitless in BCD format
            m_Multiplier = 0;
            m_Divisor = 0;
            m_SummationFormatting = 0x00;
            m_DemandFormatting = 0x00;
            m_HistoricalConsumptionFormatting = 0x00;
            m_MeteringDeviceType = 0;

            m_InstantaneousDemand = 10;
            m_CurrentDayConsumptionDelivered = 10;
            m_CurrentDayConsumptionReceived = 10;
            m_PreviousDayConsumptionDelivered = 10;
            m_PreviousDayConsumptionReceived = 10;
            m_CurrentPartialProfileIntervalStartTimeDelivered = UTC_REFERENCE_TIME;
            m_CurrentPartialProfileIntervalStartTimeReceived = UTC_REFERENCE_TIME;
            m_CurrentPartialProfileIntervalValueDelivered = 10;
            m_CurrentPartialProfileIntervalValueReceived = 10;

            m_CurrentTier1SummationDelivered = 10;
            m_CurrentTier1SummationReceived = 10;
            m_CurrentTier2SummationDelivered = 10;
            m_CurrentTier2SummationReceived = 10;
            m_CurrentTier3SummationDelivered = 10;
            m_CurrentTier3SummationReceived = 10;
            m_CurrentTier4SummationDelivered = 10;
            m_CurrentTier4SummationReceived = 10;
            m_CurrentTier5SummationDelivered = 10;
            m_CurrentTier5SummationReceived = 10;
            m_Status = 0x01;
            m_OTAFlowStatus = (byte)Itron.Metering.Zigbee.Enums.OTAFlowStatus.Image_not_available;

            // OTA Attributes
            m_UpgradeServerId = 0xffffffffffffffff;
            m_FileOffset = 0xffffffff;
            m_CurrentFileVersion = 0xffffffff;
            m_CurrentZigBeeStackVersion = 0xffff;
            m_DownloadFileVersion = 0xffffffff;
            m_DownloadZigBeeStackVersion = 0xffff;
            m_ImageUpgradeStatus = ImageUpgradeStatus.Normal;
            m_ManufacturerId = 0xffff;
            m_ImageTypeId = 0xffff;
        }

        /// <summary>
        /// 
        /// Connects to the radio through the specified COM port
        /// </summary>
        /// <param name="portName">The name of the COM port the radio is on</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public override void Connect(string portName)
        {
            m_KeyEstablishmentState = KeyEstablishmentState.NotEstablished;
            m_PartnerCertificate = null;
            m_LocalEphemeralKey = null;
            m_PartnerEphermeralKey = null;

            m_ScheduledDRLCEvents = new List<DRLCEvent>();
            m_CompletedDRLCEvents = new List<DRLCEvent>();
            m_RunningDRLCEvents = new Dictionary<DRLCDeviceClasses, DRLCEvent>();

            m_Multiplier = 1;
            m_Divisor = 1;
            m_CurrentSummationDelivered = 0;
            m_CurrentSummationReceived = 0;
            m_CurrentMaxDemandDelivered = 0;
            m_CurrentMaxDemandReceived = 0;
            m_CurrentMaxDemandDeliveredTime = UTC_REFERENCE_TIME;
            m_CurrentMaxDemandReceivedTime = UTC_REFERENCE_TIME;

            base.Connect(portName);
        }

        /// <summary>
        /// Joins the meter
        /// </summary>
        /// <param name="nodeType">The type of device to join as.</param>
        /// <param name="extendedPanID">The extended PAN ID of the network to join</param>
        /// <param name="channel">The channel that the network is on</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public override void Join(EmberNodeType nodeType, ulong extendedPanID, byte channel)
        {
            base.Join(nodeType, extendedPanID, channel);
            // We are currently joined with the meter we need to continue with the key establishment process
            if (IsJoined)
            {
                PerformKeyEstablishment(TRUST_CENTER_NODE_ID, false, KeyEstablishmentState.Established);

                if (m_KeyEstablishmentState != KeyEstablishmentState.Established)
                {
                    IsJoined = false;
                }
            }
        }

        /// <summary>
        /// Joins the meter but causes an error to occur during CBKE for testing purposes
        /// </summary>
        /// <param name="nodeType">The type of device to join as.</param>
        /// <param name="extendedPanID">The extended PAN ID of the network to join</param>
        /// <param name="channel">The channel that the network is on</param>
        /// <param name="errorState">The state in which the error should occur</param>
        /// <returns>True if the radio joined the meter to cause a CBKE failure. False if the radio failed to join the meter.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/08/12 RCG 2.60.33        Created

        public bool JoinWithCBKEError(EmberNodeType nodeType, ulong extendedPanID, byte channel, KeyEstablishmentState errorState)
        {
            bool JoinSuccess = false;

            base.Join(nodeType, extendedPanID, channel);

            // We are currently joined with the meter we need to continue with the key establishment process
            if (IsJoined)
            {
                JoinSuccess = true;

                PerformKeyEstablishment(TRUST_CENTER_NODE_ID, true, errorState);

                if (m_KeyEstablishmentState != KeyEstablishmentState.Established)
                {
                    IsJoined = false;
                }
            }

            return JoinSuccess;
        }

        /// <summary>
        /// Binds the radio to the specified clusters
        /// </summary>
        /// <param name="clusters">The clusters to bind to</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void BindToClusters(SmartEnergyBindingClusters clusters)
        {
            EmberStatus Status;

            if (clusters == SmartEnergyBindingClusters.None)
            {
                // We don't want to be bound to any clusters so clear the bindings
                m_EZSP.ClearBindingTable(out Status);
            }
            else
            {
                

                if ((clusters & SmartEnergyBindingClusters.Time) == SmartEnergyBindingClusters.Time)
                {
                    Bind(TRUST_CENTER_NODE_ID, (ushort)GeneralClusters.Time);
                }

                // Will always Bind the OTA
                Bind(TRUST_CENTER_NODE_ID, (ushort)GeneralClusters.OTA);


                if ((clusters & SmartEnergyBindingClusters.SimpleMetering) == SmartEnergyBindingClusters.SimpleMetering)
                {
                    Bind(TRUST_CENTER_NODE_ID, (ushort)SmartEnergyClusters.SimpleMetering);
                }

                // Will always Bind the OTA
                // Bind(TRUST_CENTER_NODE_ID, (ushort)GeneralClusters.OTA);

                if ((clusters & SmartEnergyBindingClusters.Messaging) == SmartEnergyBindingClusters.Messaging)
                {
                    Bind(TRUST_CENTER_NODE_ID, (ushort)SmartEnergyClusters.Messaging);
                }

                if ((clusters & SmartEnergyBindingClusters.Price) == SmartEnergyBindingClusters.Price)
                {
                    Bind(TRUST_CENTER_NODE_ID, (ushort)SmartEnergyClusters.Price);
                }

                if ((clusters & SmartEnergyBindingClusters.DRLC) == SmartEnergyBindingClusters.DRLC)
                {
                    Bind(TRUST_CENTER_NODE_ID, (ushort)SmartEnergyClusters.DRLC);
                }


            }
        }


        /// <summary>
        /// Opts out of the specified DRLC Event
        /// </summary>
        /// <param name="optOutEvent">The DRLC Event to opt out of</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void DRLCEventOptOut(DRLCEvent optOutEvent)
        {
            // Make sure the event is currently running or scheduled to run
            if (m_ScheduledDRLCEvents.Contains(optOutEvent) || m_RunningDRLCEvents.Values.Contains(optOutEvent))
            {
                // Make sure that the event is still valid and not already opted out
                if (optOutEvent.CurrentStatus == DRLCEventStatus.CommandReceived
                    || optOutEvent.CurrentStatus == DRLCEventStatus.EventStarted
                    || optOutEvent.CurrentStatus == DRLCEventStatus.UserOptIn)
                {
                    optOutEvent.CurrentStatus = DRLCEventStatus.UserOptOut;

                    SendDRLCEventStatusUpdate(optOutEvent);
                }
            }
        }

        /// <summary>
        /// Opts in to the specified DRLC Event
        /// </summary>
        /// <param name="optInEvent">The DRLC event to opt in to</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void DRLCEventOptIn(DRLCEvent optInEvent)
        {
            // Make sure the event is currently running or scheduled to run
            if (m_ScheduledDRLCEvents.Contains(optInEvent) || m_RunningDRLCEvents.Values.Contains(optInEvent))
            {
                // Make sure that the event is still valid and not already opted out
                if (optInEvent.CurrentStatus == DRLCEventStatus.CommandReceived
                    || optInEvent.CurrentStatus == DRLCEventStatus.EventStarted
                    || optInEvent.CurrentStatus == DRLCEventStatus.UserOptOut)
                {
                    optInEvent.CurrentStatus = DRLCEventStatus.UserOptIn;

                    SendDRLCEventStatusUpdate(optInEvent);
                }
            }
        }


        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        bool FieldControl_auto;
        string Manfacture_auto;
        string ImageType_auto;
        string CurrFW_auto;
        string Hw_auto;
        bool useSecurity_auto;
        bool RouteEnab_auto;
        bool IEEE_auto; 
        UInt32 Offset_auto;
        Byte MaxData_auto; 
        int DelayNumOfBytes_auto; 
        string IEEEV_auto;
        string Delay_auto;
        bool OTA_automate_now_flag = false;
        UInt32 Image_size_auto = 0;



        /// <summary>
        /// This code is not ready yet, it was supposed to do download automation on the dll level but know is handled from main (exe) level
        /// </summary>
        /// <param name="FieldControl">Bool for FieldControl where 1 means HW version will be provided</param>
        /// <param name="Manfacture">String of hex value of Manfacture example 1234 represents 0x1234</param>
        /// <param name="ImageType">String of hex value of Image Type example 1234 represents 0x1234</param>
        /// <param name="CurrFW">String of hex value of Currnt FW example 1234 represents 0x1234</param>
        /// <param name="Hw">String of hex value of Hardware example 1234 represents 0x1234</param>
        /// <param name="IEEE">Bool for security</param>
        /// <param name="RouteEnab">Route Enable through other devices </param>
        /// <param name="useSecurity">Bool for security</param>
        /// <param name="MaxData">String of hex value representing a byte example 12 represents 0x12</param>
        /// <param name="DelayNumOfBytes">int of value 0 or 2 representing if we have Block Request Delay where 2 means we have them</param>
        /// <param name="IEEEV">Value of IEEE Addr Hex</param>
        /// <param name="Delay">Value of Delay Hex</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/16 HM  4.70.13        Created

        public void Download_Auto(bool FieldControl, string Manfacture, string ImageType, string CurrFW, string Hw, bool useSecurity, bool RouteEnab,  bool IEEE, string MaxData, int DelayNumOfBytes, string IEEEV, string Delay)
        {
            FieldControl_auto = FieldControl;
            Manfacture_auto = Manfacture;
            ImageType_auto = ImageType;
            CurrFW_auto = CurrFW;
            Hw_auto = Hw;
            useSecurity_auto = true;
            RouteEnab_auto = RouteEnab;
            MaxData_auto = Convert.ToByte(MaxData);
            DelayNumOfBytes_auto = DelayNumOfBytes;
            IEEE_auto = IEEE;
            IEEEV_auto = IEEEV;
            Delay_auto = Delay;
            OTA_automate_now_flag = true;

            GetNextImage(FieldControl_auto, Manfacture_auto, ImageType_auto, CurrFW_auto, Hw_auto, useSecurity_auto, RouteEnab_auto);
            
        }

        /// <summary>
        /// Makes a request to the meter Request for next image. Option Manfacture code, SW version, use security included
        /// </summary>
        /// <param name="FieldControl">Bool for FieldControl where 1 means HW version will be provided</param>
        /// <param name="Manfacture">String of hex value of Manfacture example 1234 represents 0x1234</param>
        /// <param name="ImageType">String of hex value of Image Type example 1234 represents 0x1234</param>
        /// <param name="CurrFW">String of hex value of Currnt FW example 1234 represents 0x1234</param>
        /// <param name="Hw">String of hex value of Hardware example 1234 represents 0x1234</param>
        /// <param name="useSecurity">Bool for security</param>
        /// <param name="RouteEnab">Route Enable through other devices </param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/16 HM  4.70.13        Created

        public void GetNextImage(bool FieldControl, string Manfacture, string ImageType, string CurrFW, string Hw, bool useSecurity,bool RouteEnab)
        {
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);
            ZCLFrame ZclFrame = new ZCLFrame();
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ZigBeeEndpointInfo OTAEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID);
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.DestinationEndpoint = OTAEndpoint.FindMatchingClientEndpoint(TRUST_CENTER_NODE_ID, (ushort)GeneralClusters.OTA);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)GeneralClusters.OTA;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
               | EmberApsOptions.Retry;

            // if useSecurity is true, use APS encryption. Otherwise, don't.
            if (useSecurity)
            {
                ApsFrame.Options |= EmberApsOptions.Encryption;
            }

            if (RouteEnab)
            {
                ApsFrame.Options |= EmberApsOptions.EnableRouteDiscovery;
            }

            byte[] Message = new byte[(FieldControl?11:9)];

            Message[0] = Convert.ToByte(FieldControl);

            Byte[] Manfacture_arr  = StringToByteArray(Manfacture);
            Message[1] = Manfacture_arr[1]; //0x34; // Manfacturare code High
            Message[2] = Manfacture_arr[0]; // Manfacturare code low

            Byte[] ImageType_arr = StringToByteArray(ImageType);
            Message[3] = ImageType_arr[1]; // Image Type High
            Message[4] = ImageType_arr[0]; // Image Type low

            Byte[] CurrFW_arr = StringToByteArray(CurrFW);
            Message[5] = CurrFW_arr[3]; // Current File version
            Message[6] = CurrFW_arr[2]; // Current File version
            Message[7] = CurrFW_arr[1]; // Current File version
            Message[8] = CurrFW_arr[0]; // Current File version

            if (FieldControl)
            {
                Byte[] Hw_arr = StringToByteArray(Hw);
                Message[9] = Hw_arr[1]; // Hw version
                Message[10] = Hw_arr[0]; // Hw version
            }
            
            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = false;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)OTAClientCommands.QueryNextImage;
            ZclFrame.Data = Message;
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Request next Image. Manfacturer, CurrFW, HW, useSecurity: " + Manfacture
                + " Curr FW: " + CurrFW + " HW:" + Hw + (useSecurity ? " Secured" : " Unsecured"));
            SendUnicastMessage(TRUST_CENTER_NODE_ID, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Makes a request to the meter for Block Request. Option Manfacture code, SW version, use security included
        /// </summary>
        /// <param name="IEEE">Bool for FieldControl where True means IEEE node will be will be provided</param>
        /// <param name="Manfacture">String of hex value of Manfacture example 1234 represents 0x1234</param>
        /// <param name="ImageType">String of hex value of Image Type example 1234 represents 0x1234</param>
        /// <param name="CurrFW">String of hex value of Currnt FW example 1234 represents 0x1234</param>
        /// <param name="Offset">String of hex value of Offset example 1234 represents 0x1234</param>
        /// <param name="MaxData">String of hex value representing a byte example 12 represents 0x12</param>
        /// <param name="DelayNumOfBytes">int of value 0 or 2 representing if we have Block Request Delay where 2 means we have them</param>
        /// <param name="IEEEV">Value of IEEE Addr Hex</param>
        /// <param name="Delay">Value of Delay Hex</param>
        /// <param name="useSecurity">Bool for security</param>
        /// <param name="RouteEnab">Route Enable through other devices </param>

        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/16  HM 4.70.13        Created

        public void ImageBlockRequest(bool IEEE, string Manfacture, string ImageType, string CurrFW, string Offset, string MaxData, int DelayNumOfBytes, string IEEEV, string Delay, bool useSecurity, bool RouteEnab)
        {
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ZigBeeEndpointInfo OTAEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = OTAEndpoint.FindMatchingClientEndpoint(TRUST_CENTER_NODE_ID, (ushort)GeneralClusters.OTA);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)GeneralClusters.OTA;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
               | EmberApsOptions.Retry;

            if (useSecurity)
            {
                ApsFrame.Options |= EmberApsOptions.Encryption;
            }

            if (RouteEnab)
            {
                ApsFrame.Options |= EmberApsOptions.EnableRouteDiscovery;
            }

            // if useSecurity is true, use APS encryption. Otherwise, don't.
 
            byte[] Message = new byte[(IEEE ? 22+DelayNumOfBytes : 14+DelayNumOfBytes)];

            Message[0] = (Convert.ToByte(IEEE)); 
            Message[0] |=  (Convert.ToByte(DelayNumOfBytes));
            int Temp_Indx = 1;
            Byte[] Manfacture_arr = StringToByteArray(Manfacture);
            Message[Temp_Indx++ ] = Manfacture_arr[1]; //0x34; // Manfacturare code High
            Message[Temp_Indx++] = Manfacture_arr[0]; // Manfacturare code low

            Byte[] ImageType_arr = StringToByteArray(ImageType);
            Message[Temp_Indx++] = ImageType_arr[1]; // Image Type High
            Message[Temp_Indx++] = ImageType_arr[0]; // Image Type low

            Byte[] CurrFW_arr = StringToByteArray(CurrFW);
            Message[Temp_Indx++] = CurrFW_arr[3]; // Current File version
            Message[Temp_Indx++] = CurrFW_arr[2]; // Current File version
            Message[Temp_Indx++] = CurrFW_arr[1]; // Current File version
            Message[Temp_Indx++] = CurrFW_arr[0]; // Current File version

            Byte[] Offset_arr = StringToByteArray(Offset);
            Message[Temp_Indx++] = Offset_arr[3]; // Current File Offset
            Message[Temp_Indx++] = Offset_arr[2]; // Current File Offset
            Message[Temp_Indx++] = Offset_arr[1]; // Current File Offset
            Message[Temp_Indx++] = Offset_arr[0]; // Current File Offset

            Byte[] MaxData_arr = StringToByteArray(MaxData);
            Message[Temp_Indx++] = MaxData_arr[0]; // Max data size

           
            if (IEEE)
            {
                Byte[] IEEEV_arr = StringToByteArray(IEEEV);
                Message[Temp_Indx++] = IEEEV_arr[7]; // IEEE
                Message[Temp_Indx++] = IEEEV_arr[6]; // IEEE
                Message[Temp_Indx++] = IEEEV_arr[5]; // IEEE
                Message[Temp_Indx++] = IEEEV_arr[4]; // IEEE
                Message[Temp_Indx++] = IEEEV_arr[3]; // IEEE
                Message[Temp_Indx++] = IEEEV_arr[2]; // IEEE
                Message[Temp_Indx++] = IEEEV_arr[1]; // IEEE 20
                Message[Temp_Indx++] = IEEEV_arr[0]; // IEEE
            }

            if (DelayNumOfBytes==2)
            {
                Byte[] Delay_arr = StringToByteArray(Delay);
                Message[Temp_Indx++] = Delay_arr[1]; // Delay
                Message[Temp_Indx++] = Delay_arr[0]; // Delay
                
            }

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = false;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)OTAClientCommands.ImageBlockRequest;
            ZclFrame.Data = Message;
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Request next Block, Manfacturer, CurrFW, HW, useSecurity: " + Manfacture
                + " Curr FW: " + CurrFW + " Offset:" + Offset +" MaxData:"+ MaxData);
            SendUnicastMessage(TRUST_CENTER_NODE_ID, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// EndUpgrade makes a request to End upgrade. This is intiated after all blocks were received right
        /// </summary>
        /// <param name="status">Byte for status of upgrade</param>
        /// <param name="Manfacture">String of hex value of Manfacture example 1234 represents 0x1234</param>
        /// <param name="ImageType">String of hex value of Image Type example 1234 represents 0x1234</param>
        /// <param name="CurrFW">String of hex value of Currnt FW example 1234 represents 0x1234</param>
        /// <param name="useSecurity">Bool for security</param>
        /// <param name="RouteEnab">Route Enable through other devices </param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/16 HM  4.70.13        Created

        public void EndUpgradeRequest(byte status, string Manfacture, string ImageType, string CurrFW, bool useSecurity, bool RouteEnab)
        {
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ZigBeeEndpointInfo OTAEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = OTAEndpoint.FindMatchingClientEndpoint(TRUST_CENTER_NODE_ID, (ushort)GeneralClusters.OTA);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)GeneralClusters.OTA;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
               | EmberApsOptions.Retry;

            // if useSecurity is true, use APS encryption. Otherwise, don't.
            if (useSecurity)
            {
                ApsFrame.Options |= EmberApsOptions.Encryption;
            }

            if (RouteEnab)
            {
                ApsFrame.Options |= EmberApsOptions.EnableRouteDiscovery;
            }

            byte[] Message = new byte[9];

            Message[0] = status;

            Byte[] Manfacture_arr = StringToByteArray(Manfacture);
            Message[1] = Manfacture_arr[1]; //0x34; // Manfacturare code High
            Message[2] = Manfacture_arr[0]; // Manfacturare code low

            Byte[] ImageType_arr = StringToByteArray(ImageType);
            Message[3] = ImageType_arr[1]; // Image Type High
            Message[4] = ImageType_arr[0]; // Image Type low

            Byte[] CurrFW_arr = StringToByteArray(CurrFW);
            Message[5] = CurrFW_arr[3]; // Current File version
            Message[6] = CurrFW_arr[2]; // Current File version
            Message[7] = CurrFW_arr[1]; // Current File version
            Message[8] = CurrFW_arr[0]; // Current File version

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = false;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)OTAClientCommands.UpgradeEndRequest;
            ZclFrame.Data = Message;
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Request End Upgrade. Status, Manfacturer, CurrFW, HW, useSecurity: " + (status).ToString()+ Manfacture
                + " Curr FW: " + CurrFW +  (useSecurity ? " Secured" : " Unsecured"));
            SendUnicastMessage(TRUST_CENTER_NODE_ID, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Get Scheduled DRLC Event
        /// </summary>
        /// <param name="startTime">The UTC time indicating the earliest events that should be sent. A date of 1/1/2000 00:00:00 UTC indicates now.</param>
        /// <param name="numberOfEvents">The maximum number of events to send. A zero value means send all events.</param>
        /// <param name="useSecurity">Option to use APS encryption or not.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  11/25/13 MP                 added option to use encryption or not, for testing

        public void GetScheduledDRLCEvents(DateTime startTime, byte numberOfEvents, bool useSecurity)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            byte[] Message = new byte[5];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZigBeeEndpointInfo DRLCEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID);
            uint StartTimeSeconds = (uint)(startTime.ToUniversalTime() - UTC_REFERENCE_TIME).TotalSeconds;

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.DestinationEndpoint = DRLCEndpoint.FindMatchingClientEndpoint(TRUST_CENTER_NODE_ID, (ushort)SmartEnergyClusters.DRLC);
            ApsFrame.SourceEndpoint = DRLCEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.DRLC;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // if useSecurity is true, use APS encryption. Otherwise, don't.
            if (useSecurity)
            {
                ApsFrame.Options |= EmberApsOptions.Encryption;
            }

            // Create the message
            MessageWriter.Write(StartTimeSeconds);
            MessageWriter.Write(numberOfEvents);

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)DRLCClientCommands.GetScheduledEvents;
            ZclFrame.Data = Message;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Request next Image. Start Time: " + startTime.ToString("G", CultureInfo.CurrentCulture)
                + " Number of Events: " + numberOfEvents.ToString(CultureInfo.InvariantCulture));

            SendUnicastMessage(TRUST_CENTER_NODE_ID, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Makes a request to the meter to send any DRLC events any DRLC events starting from the current time
        /// </summary>
        /// <param name="numberOfEvents">The number of events to send. 0 indicates all events</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void GetScheduledDRLCEvents(byte numberOfEvents)
        {
            GetScheduledDRLCEvents(UTC_REFERENCE_TIME, numberOfEvents, true);
        }

        /// <summary>
        /// Gets the Current Energy and Demand Values from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        public void GetCurrentEnergyAndDemand()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentSummationDelivered);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentSummationReceived);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentMaxDemandDelivered);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentMaxDemandDeliveredTime);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentMaxDemandReceived);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentMaxDemandReceivedTime);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentBlockPeriodConsumptionDelivered);
            Attributes.Add((ushort)SimpleMeteringAttributes.PreviousBlockPeriodConsumptionDelivered);

            // Before reading these values we should reset them to the default values in case we can't read them for some reason
            m_CurrentSummationDelivered = 0;
            m_CurrentSummationReceived = 0;
            m_CurrentMaxDemandDelivered = 0;
            m_CurrentMaxDemandReceived = 0;
            m_CurrentMaxDemandDeliveredTime = UTC_REFERENCE_TIME;
            m_CurrentMaxDemandReceivedTime = UTC_REFERENCE_TIME;
            m_CurrentBlockPeriodConsumptionDelivered = null;
            m_PreviousBlockPeriodConsumptionDelivered = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Energy and Demand Attributes");

            if (IsJoined)
            {
                // We should always make sure that we have the Multiplier and Divisor first
                GetSimpleMeteringMultiplierAndDivisor();

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)SimpleMeteringAttributes.CurrentSummationDelivered:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    m_CurrentSummationDelivered = (ulong)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Delivered: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationReceived:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    m_CurrentSummationReceived = (ulong)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Received: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandDelivered:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    m_CurrentMaxDemandDelivered = (ulong)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Delivered: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandReceived:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    m_CurrentMaxDemandReceived = (ulong)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Received: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandDeliveredTime:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                {
                                    m_CurrentMaxDemandDeliveredTime = (DateTime)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Delivered Time: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandReceivedTime:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                {
                                    m_CurrentMaxDemandDeliveredTime = (DateTime)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Received Time: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)SimpleMeteringAttributes.CurrentBlockPeriodConsumptionDelivered:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    m_CurrentBlockPeriodConsumptionDelivered = (ulong)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Block Period Consumption Delivered: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)SimpleMeteringAttributes.PreviousBlockPeriodConsumptionDelivered:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    m_PreviousBlockPeriodConsumptionDelivered = (ulong)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Previous Block Period Consumption Delivered: " + CurrentAttribute.DataType.ToString());
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
        /// Gets all simple metering attributes from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public void GetSimpleMeteringGeneralAttributes(SimpleMeteringAttributes desiredAttribute)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)desiredAttribute);

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Energy and Demand Attributes");

            if (IsJoined)
            {
                // We should always make sure that we have the Multiplier and Divisor first
                GetSimpleMeteringMultiplierAndDivisor();

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)SimpleMeteringAttributes.CurrentSummationDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentSummationDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationReceived:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentSummationReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentMaxDemandDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandReceived:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentMaxDemandReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandDeliveredTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_CurrentMaxDemandDeliveredTime = (DateTime)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Delivered Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandReceivedTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_CurrentMaxDemandDeliveredTime = (DateTime)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Received Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.DailyFreezeTimeSummation:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_DFTSummation = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Daily Freeze Time Summation: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.DailyFreezeTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint16)
                                    {
                                        m_DailyFreezeTime = (UInt16)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Daily Freeze Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.PowerFactor:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Int8)
                                    {
                                        m_PowerFactor = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Power Factor: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.ReadingSnapShotTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_ReadingSnapShotTime = (DateTime)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Reading Snap Shot Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.DefaultUpdatePeriod:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_DefaultUpdatePeriod = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Default Update Period: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.FastPollUpdatePeriod:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_FastPollUpdatePeriod = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Fast Polling Update Period: " + CurrentAttribute.DataType.ToString());
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
        /// Gets all simple metering attributes from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/25/13 MP                 Created, copied from above but with option of using APS encryption or not

        public void GetSimpleMeteringGeneralAttributes(bool useSecurity)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentSummationDelivered);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentSummationReceived);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentMaxDemandDelivered);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentMaxDemandReceived);
            Attributes.Add((ushort)SimpleMeteringAttributes.DailyFreezeTimeSummation);
            Attributes.Add((ushort)SimpleMeteringAttributes.DailyFreezeTime);
            Attributes.Add((ushort)SimpleMeteringAttributes.PowerFactor);
            Attributes.Add((ushort)SimpleMeteringAttributes.ReadingSnapShotTime);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentMaxDemandDeliveredTime);
            Attributes.Add((ushort)SimpleMeteringAttributes.CurrentMaxDemandReceivedTime);
            Attributes.Add((ushort)SimpleMeteringAttributes.DefaultUpdatePeriod);
            Attributes.Add((ushort)SimpleMeteringAttributes.FastPollUpdatePeriod);

            // Before reading these values we should reset them to the default values in case we can't read them for some reason
            m_CurrentSummationDelivered = 0;
            m_CurrentSummationReceived = 0;
            m_CurrentMaxDemandDelivered = 0;
            m_CurrentMaxDemandReceived = 0;
            m_DFTSummation = 0;
            m_DailyFreezeTime = 0;
            m_PowerFactor = 0;
            m_ReadingSnapShotTime = UTC_REFERENCE_TIME;
            m_CurrentMaxDemandDeliveredTime = UTC_REFERENCE_TIME;
            m_CurrentMaxDemandReceivedTime = UTC_REFERENCE_TIME;
            m_DefaultUpdatePeriod = 0;
            m_FastPollUpdatePeriod = 0;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Energy and Demand Attributes");

            if (IsJoined)
            {
                // We should always make sure that we have the Multiplier and Divisor first
                GetSimpleMeteringMultiplierAndDivisor();

                AttributeData = ReadAttributes(useSecurity, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)SimpleMeteringAttributes.CurrentSummationDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentSummationDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationReceived:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentSummationReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentMaxDemandDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandReceived:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentMaxDemandReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandDeliveredTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_CurrentMaxDemandDeliveredTime = (DateTime)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Demand Delivered Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentMaxDemandReceivedTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_CurrentMaxDemandDeliveredTime = (DateTime)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Max Received Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.DailyFreezeTimeSummation:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_DFTSummation = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Daily Freeze Time Summation: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.DailyFreezeTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint16)
                                    {
                                        m_DailyFreezeTime = (UInt16)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Daily Freeze Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.PowerFactor:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Int8)
                                    {
                                        m_PowerFactor = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Power Factor: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.ReadingSnapShotTime:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_ReadingSnapShotTime = (DateTime)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Reading Snap Shot Time: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.DefaultUpdatePeriod:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_DefaultUpdatePeriod = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Default Update Period: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.FastPollUpdatePeriod:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_FastPollUpdatePeriod = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Fast Polling Update Period: " + CurrentAttribute.DataType.ToString());
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
        /// Gets all simple metering attributes from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public void GetSimpleMeteringTOUInfoAttributes(SimpleMeteringAttributes desiredAttribute)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)desiredAttribute);
            
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Energy and Demand Attributes");

            if (IsJoined)
            {
                // Create attribute data
                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)SimpleMeteringAttributes.CurrentSummationDeliveredTier1:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier1SummationDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationReceivedTier1:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier1SummationReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationDeliveredTier2:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier2SummationDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationReceivedTier2:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier2SummationReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationDeliveredTier3:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier3SummationDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationReceivedTier3:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier3SummationReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationDeliveredTier4:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier4SummationDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationReceivedTier4:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier4SummationReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationDeliveredTier5:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier5SummationDelivered = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentSummationReceivedTier5:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                    {
                                        m_CurrentTier5SummationReceived = (ulong)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Summation Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.Status:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                    {
                                        m_Status = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Status: " + CurrentAttribute.DataType.ToString());
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
        /// Gets all simple metering attributes from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public void GetSimpleMeteringHistoricalAttributes(SimpleMeteringAttributes desiredAttribute)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)desiredAttribute);

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Energy and Demand Attributes");

            if (IsJoined)
            {

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)SimpleMeteringAttributes.InstantaneousDemand:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Int24)
                                    {
                                        m_InstantaneousDemand = (Int32)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Instantaneous Demand: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentDayConsumptionDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                    {
                                        m_CurrentDayConsumptionDelivered = (uint)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Day Consumption Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentDayConsumptionReceived:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                    {
                                        m_CurrentDayConsumptionReceived = (uint)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Current Day Consumption Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.PreviousDayConsumptionDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                    {
                                        m_PreviousDayConsumptionDelivered = (uint)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Previous Day Consumption Delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.PreviousDayConsumptionReceived:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                    {
                                        m_PreviousDayConsumptionReceived = (uint)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Previous Day Consumption Received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentPartialProfileIntervalStartTimeDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_CurrentPartialProfileIntervalStartTimeDelivered = (DateTime)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading current partial profile interval start time delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentPartialProfileIntervalStartTimeReceived:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                    {
                                        m_CurrentPartialProfileIntervalStartTimeReceived = (DateTime)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading current partial profile interval start time received: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentPartialProfileIntervalValueDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                    {
                                        m_CurrentPartialProfileIntervalValueDelivered = (uint)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading current partial profile interval value delivered: " + CurrentAttribute.DataType.ToString());
                                        throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                    }

                                    break;
                                }
                            case (ushort)SimpleMeteringAttributes.CurrentPartialProfileIntervalValueReceived:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                    {
                                        m_CurrentPartialProfileIntervalValueReceived = (uint)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading current partial profile interval value received: " + CurrentAttribute.DataType.ToString());
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
        /// Gets the Maximum Number of Periods Delivered from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/30/12 PGH 2.60.49        Created

        public void GetMaxNumberOfPeriodsDelivered()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)SimpleMeteringAttributes.MaxNumberOfPeriodsDelivered);

            // Before reading these values we should reset them to the default values in case we can't read them for some reason
            m_MaxNumberOfPeriodsDelivered = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Maximum Number of Periods Delivered Attribute");

            if (IsJoined)
            {
                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)SimpleMeteringAttributes.MaxNumberOfPeriodsDelivered:
                                {
                                    if (CurrentAttribute.DataType == ZCLDataType.Uint8)
                                    {
                                        m_MaxNumberOfPeriodsDelivered = (byte)CurrentAttribute.Value;
                                    }
                                    else
                                    {
                                        // We can't continue reading because we don't know how much data to read
                                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Maximum Number of Periods Delivered: " + CurrentAttribute.DataType.ToString());
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
        /// Gets Tier Price Labels from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/31/12 PGH 2.60.30        Created

        public void GetTierPriceLabels()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            Attributes.Add((ushort)TierLabelAttributeSet.Tier1PriceLabel);
            Attributes.Add((ushort)TierLabelAttributeSet.Tier2PriceLabel);
            Attributes.Add((ushort)TierLabelAttributeSet.Tier3PriceLabel);
            Attributes.Add((ushort)TierLabelAttributeSet.Tier4PriceLabel);
            Attributes.Add((ushort)TierLabelAttributeSet.Tier5PriceLabel);

            m_Tier1PriceLabel = "UnsupportedAttribute";
            m_Tier2PriceLabel = "UnsupportedAttribute";
            m_Tier3PriceLabel = "UnsupportedAttribute";
            m_Tier4PriceLabel = "UnsupportedAttribute";
            m_Tier5PriceLabel = "UnsupportedAttribute";

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Tier Label Attributes");

            if (IsJoined)
            {
                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)TierLabelAttributeSet.Tier1PriceLabel:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.OctetString)
                                {
                                    m_Tier1PriceLabel = (string)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1PriceLabel: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)TierLabelAttributeSet.Tier2PriceLabel:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.OctetString)
                                {
                                    m_Tier2PriceLabel = (string)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2PriceLabel: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)TierLabelAttributeSet.Tier3PriceLabel:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.OctetString)
                                {
                                    m_Tier3PriceLabel = (string)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3PriceLabel: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)TierLabelAttributeSet.Tier4PriceLabel:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.OctetString)
                                {
                                    m_Tier4PriceLabel = (string)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4PriceLabel: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)TierLabelAttributeSet.Tier5PriceLabel:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.OctetString)
                                {
                                    m_Tier5PriceLabel = (string)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier5PriceLabel: " + CurrentAttribute.DataType.ToString());
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
        /// Get Block Period attributes from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/31/12 PGH 2.60.30        Created

        public void GetBlockPeriodAttributes()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            Attributes.Add((ushort)BlockPeriodAttributeSet.StartOfBlockPeriod);
            Attributes.Add((ushort)BlockPeriodAttributeSet.BlockPeriodDuration);
            Attributes.Add((ushort)BlockPeriodAttributeSet.ThresholdDivisor);
            Attributes.Add((ushort)BlockPeriodAttributeSet.ThresholdMultiplier);

            m_StartOfBlockPeriod = null;
            m_BlockPeriodDuration = null;
            m_ThresholdDivisor = null;
            m_ThresholdMultiplier = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Block Period Attributes");

            if (IsJoined)
            {
                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)BlockPeriodAttributeSet.StartOfBlockPeriod:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                {
                                    m_StartOfBlockPeriod = (DateTime)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Start of Block Period: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPeriodAttributeSet.BlockPeriodDuration:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                {
                                    m_BlockPeriodDuration = (uint)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block Period Duration: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPeriodAttributeSet.ThresholdDivisor:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                {
                                    m_ThresholdDivisor = (uint)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Threshold Divisor: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPeriodAttributeSet.ThresholdMultiplier:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                {
                                    m_ThresholdMultiplier = (uint)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Threshold Multiplier: " + CurrentAttribute.DataType.ToString());
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
        /// Get Block Thresholds from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created
        //  11/20/12 RCG 2.70.40 243543 Adding short delay between attribute reads to avoid timing issues

        public void GetBlockThresholds()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;

            List<ushort> AttributesPart1 = new List<ushort>();
            AttributesPart1.Add((ushort)BlockThresholdAttributeSet.Block1Threshold);
            AttributesPart1.Add((ushort)BlockThresholdAttributeSet.Block2Threshold);
            AttributesPart1.Add((ushort)BlockThresholdAttributeSet.Block3Threshold);
            AttributesPart1.Add((ushort)BlockThresholdAttributeSet.Block4Threshold);
            AttributesPart1.Add((ushort)BlockThresholdAttributeSet.Block5Threshold);
            AttributesPart1.Add((ushort)BlockThresholdAttributeSet.Block6Threshold);
            AttributesPart1.Add((ushort)BlockThresholdAttributeSet.Block7Threshold);
            AttributesPart1.Add((ushort)BlockThresholdAttributeSet.Block8Threshold);

            List<ushort> AttributesPart2 = new List<ushort>();
            AttributesPart2.Add((ushort)BlockThresholdAttributeSet.Block9Threshold);
            AttributesPart2.Add((ushort)BlockThresholdAttributeSet.Block10Threshold);
            AttributesPart2.Add((ushort)BlockThresholdAttributeSet.Block11Threshold);
            AttributesPart2.Add((ushort)BlockThresholdAttributeSet.Block12Threshold);
            AttributesPart2.Add((ushort)BlockThresholdAttributeSet.Block13Threshold);
            AttributesPart2.Add((ushort)BlockThresholdAttributeSet.Block14Threshold);
            AttributesPart2.Add((ushort)BlockThresholdAttributeSet.Block15Threshold);

            m_Block1Threshold = null;
            m_Block2Threshold = null;
            m_Block3Threshold = null;
            m_Block4Threshold = null;
            m_Block5Threshold = null;
            m_Block6Threshold = null;
            m_Block7Threshold = null;
            m_Block8Threshold = null;
            m_Block9Threshold = null;
            m_Block10Threshold = null;
            m_Block11Threshold = null;
            m_Block12Threshold = null;
            m_Block13Threshold = null;
            m_Block14Threshold = null;
            m_Block15Threshold = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Block Threshold Attributes");

            if (IsJoined)
            {

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart1);

                // Short delay to avoid possible timing problems when reading attributes
                Thread.Sleep(100);

                AttributeData.AddRange(ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart2));

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)BlockThresholdAttributeSet.Block1Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block1Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block1Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block2Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block2Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block2Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block3Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block3Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block3Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block4Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block4Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block4Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block5Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block5Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block5Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block6Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block6Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block6Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block7Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block7Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block7Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block8Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block8Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block8Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block9Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block9Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block9Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block10Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block10Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block10Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block11Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block11Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block11Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block12Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block12Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block12Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block13Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block13Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block13Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block14Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block14Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block14Threshold: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockThresholdAttributeSet.Block15Threshold:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint48)
                                {
                                    ulong AttributeValue = (ulong)CurrentAttribute.Value;

                                    // A value of all 0xFF means the value is not used
                                    if (AttributeValue != UInt48.MaxValue)
                                    {
                                        m_Block15Threshold = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Block15Threshold: " + CurrentAttribute.DataType.ToString());
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
        /// Get Billing Information from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/12 PGH 2.60.30        Created

        public void GetBillingInformation()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();

            Attributes.Add((ushort)BillingInformationAttributes.CurrentBillingPeriodStart);
            Attributes.Add((ushort)BillingInformationAttributes.CurrentBillingPeriodDuration);

            m_CurrentBillingPeriodDuration = null;
            m_CurrentBillingPeriodStart = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Billing Information Attributes");

            if (IsJoined)
            {
                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, Attributes);

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)BillingInformationAttributes.CurrentBillingPeriodStart:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.UTCTime)
                                {
                                    m_CurrentBillingPeriodStart = (DateTime)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading CurrentBillingPeriodStart: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BillingInformationAttributes.CurrentBillingPeriodDuration:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                {
                                    m_CurrentBillingPeriodDuration = (uint)CurrentAttribute.Value;
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading CurrentBillingPeriodDuration: " + CurrentAttribute.DataType.ToString());
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
        /// Get NoTier Block Prices from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created
        //  11/20/12 RCG 2.70.40 243543 Adding short delay between attribute reads to avoid timing issues

        public void GetNoTierBlockPrices()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;

            List<ushort> AttributesPart1 = new List<ushort>();
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock1Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock2Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock3Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock4Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock5Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock6Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock7Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock8Price);

            List<ushort> AttributesPart2 = new List<ushort>();
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock9Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock10Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock11Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock12Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock13Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock14Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock15Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.NoTierBlock16Price);

            m_NoTierBlock1Price = null;
            m_NoTierBlock2Price = null;
            m_NoTierBlock3Price = null;
            m_NoTierBlock4Price = null;
            m_NoTierBlock5Price = null;
            m_NoTierBlock6Price = null;
            m_NoTierBlock7Price = null;
            m_NoTierBlock8Price = null;
            m_NoTierBlock9Price = null;
            m_NoTierBlock10Price = null;
            m_NoTierBlock11Price = null;
            m_NoTierBlock12Price = null;
            m_NoTierBlock13Price = null;
            m_NoTierBlock14Price = null;
            m_NoTierBlock15Price = null;
            m_NoTierBlock16Price = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting NoTier Block Price Information Attributes");

            if (IsJoined)
            {

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart1);

                // Short delay to avoid possible timing problems when reading attributes
                Thread.Sleep(100);

                AttributeData.AddRange(ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart2));

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock1Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock1Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock1Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock2Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock2Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock2Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock3Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock3Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock3Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock4Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock4Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock4Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock5Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock5Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBloc5kPrice: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock6Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock6Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock6Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock7Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock7Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock7Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock8Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock8Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock8Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock9Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock9Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock9Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock10Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock10Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock10Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock11Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock11Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock11Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock12Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock12Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock12Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock13Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock13Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock13Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock14Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock14Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock14Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock15Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock15Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock15Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.NoTierBlock16Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_NoTierBlock16Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading NoTierBlock16Price: " + CurrentAttribute.DataType.ToString());
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
        /// Get Tier1 Block Prices from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created
        //  11/20/12 RCG 2.70.40 243543 Adding short delay between attribute reads to avoid timing issues

        public void GetTier1BlockPrices()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;

            List<ushort> AttributesPart1 = new List<ushort>();
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block1Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block2Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block3Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block4Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block5Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block6Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block7Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block8Price);

            List<ushort> AttributesPart2 = new List<ushort>();
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block9Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block10Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block11Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block12Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block13Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block14Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block15Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier1Block16Price);

            m_Tier1Block1Price = null;
            m_Tier1Block2Price = null;
            m_Tier1Block3Price = null;
            m_Tier1Block4Price = null;
            m_Tier1Block5Price = null;
            m_Tier1Block6Price = null;
            m_Tier1Block7Price = null;
            m_Tier1Block8Price = null;
            m_Tier1Block9Price = null;
            m_Tier1Block10Price = null;
            m_Tier1Block11Price = null;
            m_Tier1Block12Price = null;
            m_Tier1Block13Price = null;
            m_Tier1Block14Price = null;
            m_Tier1Block15Price = null;
            m_Tier1Block16Price = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Tier1 Block Price Information Attributes");

            if (IsJoined)
            {

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart1);

                // Short delay to avoid possible timing problems when reading attributes
                Thread.Sleep(100);

                AttributeData.AddRange(ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart2));

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block1Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block1Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block1Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block2Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block2Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block2Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block3Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block3Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block3Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block4Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block4Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block4Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block5Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block5Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Bloc5kPrice: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block6Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block6Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block6Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block7Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block7Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block7Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block8Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block8Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block8Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block9Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block9Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block9Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block10Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block10Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block10Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block11Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block11Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block11Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block12Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block12Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block12Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block13Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block13Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block13Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block14Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block14Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block14Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block15Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block15Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block15Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier1Block16Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier1Block16Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier1Block16Price: " + CurrentAttribute.DataType.ToString());
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
        /// Get Tier2 Block Prices from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created
        //  11/20/12 RCG 2.70.40 243543 Adding short delay between attribute reads to avoid timing issues

        public void GetTier2BlockPrices()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;

            List<ushort> AttributesPart1 = new List<ushort>();
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block1Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block2Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block3Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block4Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block5Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block6Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block7Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block8Price);

            List<ushort> AttributesPart2 = new List<ushort>();
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block9Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block10Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block11Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block12Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block13Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block14Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block15Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier2Block16Price);

            m_Tier2Block1Price = null;
            m_Tier2Block2Price = null;
            m_Tier2Block3Price = null;
            m_Tier2Block4Price = null;
            m_Tier2Block5Price = null;
            m_Tier2Block6Price = null;
            m_Tier2Block7Price = null;
            m_Tier2Block8Price = null;
            m_Tier2Block9Price = null;
            m_Tier2Block10Price = null;
            m_Tier2Block11Price = null;
            m_Tier2Block12Price = null;
            m_Tier2Block13Price = null;
            m_Tier2Block14Price = null;
            m_Tier2Block15Price = null;
            m_Tier2Block16Price = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Tier2 Block Price Information Attributes");

            if (IsJoined)
            {

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart1);

                // Short delay to avoid possible timing problems when reading attributes
                Thread.Sleep(100);

                AttributeData.AddRange(ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart2));

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block1Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block1Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block1Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block2Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block2Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block2Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block3Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block3Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block3Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block4Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block4Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block4Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block5Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block5Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Bloc5kPrice: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block6Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block6Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block6Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block7Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block7Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block7Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block8Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block8Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block8Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block9Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block9Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block9Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block10Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block10Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block10Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block11Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block11Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block11Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block12Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block12Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block12Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block13Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block13Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block13Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block14Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block14Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block14Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block15Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block15Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block15Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier2Block16Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier2Block16Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier2Block16Price: " + CurrentAttribute.DataType.ToString());
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
        /// Get Tier3 Block Prices from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created
        //  11/20/12 RCG 2.70.40 243543 Adding short delay between attribute reads to avoid timing issues

        public void GetTier3BlockPrices()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;

            List<ushort> AttributesPart1 = new List<ushort>();
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block1Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block2Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block3Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block4Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block5Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block6Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block7Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block8Price);

            List<ushort> AttributesPart2 = new List<ushort>();
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block9Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block10Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block11Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block12Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block13Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block14Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block15Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier3Block16Price);

            m_Tier3Block1Price = null;
            m_Tier3Block2Price = null;
            m_Tier3Block3Price = null;
            m_Tier3Block4Price = null;
            m_Tier3Block5Price = null;
            m_Tier3Block6Price = null;
            m_Tier3Block7Price = null;
            m_Tier3Block8Price = null;
            m_Tier3Block9Price = null;
            m_Tier3Block10Price = null;
            m_Tier3Block11Price = null;
            m_Tier3Block12Price = null;
            m_Tier3Block13Price = null;
            m_Tier3Block14Price = null;
            m_Tier3Block15Price = null;
            m_Tier3Block16Price = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Tier3 Block Price Information Attributes");

            if (IsJoined)
            {

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart1);

                // Short delay to avoid possible timing problems when reading attributes
                Thread.Sleep(100);

                AttributeData.AddRange(ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart2));

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block1Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block1Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block1Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block2Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block2Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block2Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block3Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block3Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block3Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block4Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block4Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block4Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block5Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block5Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Bloc5kPrice: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block6Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block6Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block6Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block7Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block7Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block7Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block8Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block8Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block8Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block9Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block9Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block9Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block10Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block10Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block10Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block11Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block11Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block11Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block12Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block12Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block12Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block13Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block13Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block13Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block14Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block14Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block14Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block15Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block15Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block15Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier3Block16Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier3Block16Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier3Block16Price: " + CurrentAttribute.DataType.ToString());
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
        /// Get Tier4 Block Prices from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created
        //  11/20/12 RCG 2.70.40 243543 Adding short delay between attribute reads to avoid timing issues

        public void GetTier4BlockPrices()
        {
            List<ZigBeeAttributeResponse> AttributeData = null;

            List<ushort> AttributesPart1 = new List<ushort>();
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block1Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block2Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block3Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block4Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block5Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block6Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block7Price);
            AttributesPart1.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block8Price);

            List<ushort> AttributesPart2 = new List<ushort>();
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block9Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block10Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block11Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block12Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block13Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block14Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block15Price);
            AttributesPart2.Add((ushort)BlockPriceInformationAttributeSet.Tier4Block16Price);

            m_Tier4Block1Price = null;
            m_Tier4Block2Price = null;
            m_Tier4Block3Price = null;
            m_Tier4Block4Price = null;
            m_Tier4Block5Price = null;
            m_Tier4Block6Price = null;
            m_Tier4Block7Price = null;
            m_Tier4Block8Price = null;
            m_Tier4Block9Price = null;
            m_Tier4Block10Price = null;
            m_Tier4Block11Price = null;
            m_Tier4Block12Price = null;
            m_Tier4Block13Price = null;
            m_Tier4Block14Price = null;
            m_Tier4Block15Price = null;
            m_Tier4Block16Price = null;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Tier4 Block Price Information Attributes");

            if (IsJoined)
            {

                AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart1);

                // Short delay to avoid possible timing problems when reading attributes
                Thread.Sleep(100);

                AttributeData.AddRange(ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price, AttributesPart2));

                foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                {
                    if (CurrentAttribute.Status == ZCLStatus.Success)
                    {
                        switch (CurrentAttribute.AttributeID)
                        {
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block1Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block1Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block1Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block2Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block2Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block2Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block3Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block3Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block3Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block4Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block4Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block4Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block5Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block5Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Bloc5kPrice: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block6Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block6Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block6Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block7Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block7Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block7Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block8Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block8Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block8Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block9Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block9Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block9Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block10Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block10Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block10Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block11Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block11Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block11Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block12Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block12Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block12Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block13Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block13Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block13Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block14Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block14Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block14Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block15Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block15Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block15Price: " + CurrentAttribute.DataType.ToString());
                                    throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                }

                                break;
                            }
                            case (ushort)BlockPriceInformationAttributeSet.Tier4Block16Price:
                            {
                                if (CurrentAttribute.DataType == ZCLDataType.Uint32)
                                {
                                    uint AttributeValue = (uint)CurrentAttribute.Value;

                                    if (AttributeValue != uint.MaxValue)
                                    {
                                        m_Tier4Block16Price = AttributeValue;
                                    }
                                }
                                else
                                {
                                    // We can't continue reading because we don't know how much data to read
                                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Tier4Block16Price: " + CurrentAttribute.DataType.ToString());
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
        /// Gets the load profile data from the meter
        /// </summary>
        /// <param name="channel">The load profile channel to get</param>
        /// <param name="endTime">The end time of the data to get</param>
        /// <param name="periods">The number of periods to get</param>
        /// <returns>The requested Load Profile data</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created

        public SmartEnergyLoadProfile GetLoadProfileData(SimpleMeteringProfileChannel channel, DateTime endTime, byte periods)
        {
            return GetProfileRequest(TRUST_CENTER_NODE_ID, channel, endTime, periods);
        }

        /// <summary>
        /// Gets the load profile consumption from the meter
        /// </summary>
        /// <param name="channel">The load profile channel to get</param>
        /// <param name="endTime">The end time of the data to get</param>
        /// <param name="periods">The number of periods to get</param>
        /// <returns>The requested Load Profile data</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/31/12 PGH 2.70.01        Created

        public SmartEnergyLoadProfile GetLoadProfileConsumption(SimpleMeteringProfileChannel channel, DateTime endTime, byte periods)
        {

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Get Profile for " + channel.ToString() + " with end time " + endTime.ToString() + " and number of periods equal to " + periods.ToString() + ".");

            SmartEnergyLoadProfile ProfileData = new SmartEnergyLoadProfile();
            byte periodsOfPreviousBlock = 0;
            TimeSpan profileIntervalPeriod = new TimeSpan();
           
            GetProfileConsumptionRequest(TRUST_CENTER_NODE_ID, channel, endTime, periods, ref ProfileData, ref periodsOfPreviousBlock, ref profileIntervalPeriod);

            if (ProfileData.Intervals.Count > 0)
            {
                while (((periods -= periodsOfPreviousBlock) > 0) && (periodsOfPreviousBlock > 0))
                {

                    // ZigBee states to wait 7.5 seconds before subsequent requests
                    Thread.Sleep(7500);
                    TimeSpan duration = TimeSpan.FromTicks(profileIntervalPeriod.Ticks * periodsOfPreviousBlock);
                    endTime -= duration;
                    periodsOfPreviousBlock = 0;

                    GetProfileConsumptionRequest(TRUST_CENTER_NODE_ID, channel, endTime, periods, ref ProfileData, ref periodsOfPreviousBlock, ref profileIntervalPeriod);

                }
            }

            return (ProfileData);
        }

        /// <summary>
        /// send mirror request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 MP                 Created

        public void SendMirrorRequest()
        {
            SendMirrorRequest(TRUST_CENTER_NODE_ID);
        }

        /// <summary>
        /// send mirror remove request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/18/13 MP                 Created

        public void SendRemoveMirrorRequest()
        {
            SendRemoveMirrorRequest(TRUST_CENTER_NODE_ID);
        }

        /// <summary>
        /// send fast polling request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/13 MP                 Created

        public void SendFastPollRequest(byte pollRate, int duration)
        {
            SendFastPollRequest(TRUST_CENTER_NODE_ID, pollRate, duration);
        }

        /// <summary>
        /// sends write attribute command
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/26/13 MP                 Created

        public void SendWriteAttributeCommand(GeneralClusters ClusterID, ushort AttributeID, ZCLDataType DataType, byte data)
        {
            SendWriteAttributeCommand(TRUST_CENTER_NODE_ID, ClusterID, AttributeID, DataType, data );
        }



        /// <summary>
        /// Get Last Message from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/22/12 PGH 2.53.45  Created

        public void GetLastMessage()
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Last Message:");

            GetLastMessageRequest(TRUST_CENTER_NODE_ID, true);
        }

        /// <summary>
        /// Get Last Message from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/25/13 MP                 created, copied from above but with option of using security.

        public void GetLastMessage(bool useSecurity)
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Last Message:");

            GetLastMessageRequest(TRUST_CENTER_NODE_ID, useSecurity);
        }

        /// <summary>
        /// Send Message Confirmation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/28/12 PGH 2.53.45  Created

        public void SendMessageConfirmation(string MessageID)
        {
            uint MsgID = Convert.ToUInt32(MessageID);
            uint MsgConfirmTimeInSeconds = (UInt32)(CurrentUTCTime - UTC_REFERENCE_TIME).TotalSeconds;
            DateTime MsgConfirmDateTime = UTC_REFERENCE_TIME.AddSeconds((double)MsgConfirmTimeInSeconds);
            string MessageInfo = "Message ID: " + MsgID.ToString() + " Confirmation Time: " + MsgConfirmDateTime.ToString();

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Display Message Confirmation:");
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, MessageInfo);

            SendMessageConfirmationRequest(TRUST_CENTER_NODE_ID, MsgID, MsgConfirmTimeInSeconds);
        }

        /// <summary>
        /// Get Current Price from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created

        public void GetCurrentPrice()
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Current Price:");

            GetCurrentPriceRequest(TRUST_CENTER_NODE_ID, true);
        }

        /// <summary>
        /// Get Current Price from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/22/13 MP                 Created, copied from above, but have option of choosing security

        public void GetCurrentPrice(bool useSecurity)
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Current Price:");

            GetCurrentPriceRequest(TRUST_CENTER_NODE_ID, useSecurity);
        }

        /// <summary>
        /// Get Block Periods from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/10/12 PGH 2.60.13        Created

        public void GetBlockPeriods()
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Block Period:");

            // numEvents of 0 would return all available Publish Block Periods
            byte numEvents = 0;
            // start time is now
            uint UTCStartTimeInSeconds = (UInt32)(CurrentUTCTime - UTC_REFERENCE_TIME).TotalSeconds;
            DateTime UTCDateTime = UTC_REFERENCE_TIME.AddSeconds((double)UTCStartTimeInSeconds);
            string LogInfo = "UTC Start Time: " + UTCDateTime.ToString() + " Number of Events: " + numEvents.ToString();

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, LogInfo);

            // make request
            GetBlockPeriodsRequest(TRUST_CENTER_NODE_ID, UTCStartTimeInSeconds, numEvents);
        }

        /// <summary>
        /// Get Scheduled Prices from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/10/12 PGH 2.60.13        Created

        public void GetScheduledPrices()
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Scheduled Prices:");

            // numEvents of 0 would return all available events
            byte numEvents = 0;
            // start time is now
            uint UTCStartTimeInSeconds = (UInt32)(CurrentUTCTime - UTC_REFERENCE_TIME).TotalSeconds;
            DateTime UTCDateTime = UTC_REFERENCE_TIME.AddSeconds((double)UTCStartTimeInSeconds);
            string LogInfo = "UTC Start Time: " + UTCDateTime.ToString() + " Number of Events: " + numEvents.ToString();

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, LogInfo);

            // make request
            GetScheduledPricesRequest(TRUST_CENTER_NODE_ID, UTCStartTimeInSeconds, numEvents);
        }

        /// <summary>
        /// Get Scheduled Prices from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/12/14 AR  3.50.50        Created

        public void GetScheduledPricesUsingZeroStartTime()
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting Scheduled Prices (Using Zero Start Time):");

            // numEvents of 0 would return all available events
            byte numEvents = 0;
            // start time is zero
            uint UTCStartTimeInSeconds = 0;
            string LogInfo = "UTC Start Time (Using Zero Start Time): 0" + " Number of Events: " + numEvents.ToString();

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, LogInfo);

            // make request
            GetScheduledPricesRequest(TRUST_CENTER_NODE_ID, UTCStartTimeInSeconds, numEvents);
        }

        /// <summary>
        /// Send Price Acknowledgement
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/10/12 PGH 2.60.13        Created

        public void SendPriceAcknowledgement(uint ProviderId, uint IssuerEventId)
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Price Acknowledgement:");

            // Ack time is now
            uint UTCPriceAckTimeInSeconds = (UInt32)(CurrentUTCTime - UTC_REFERENCE_TIME).TotalSeconds;

            // control field
            byte controlField = 0;

            DateTime UTCDateTime = UTC_REFERENCE_TIME.AddSeconds((double)UTCPriceAckTimeInSeconds);
            string LogInfo = "Provider ID: " + ProviderId.ToString() + " Issuer Event ID: " + IssuerEventId.ToString();
            LogInfo += " UTC Price Ack Time: " + UTCDateTime.ToString() + " Control Field: " + controlField.ToString("X2");
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, LogInfo);

            // send acknowledgement
            SendPriceAcknowledgementRequest(TRUST_CENTER_NODE_ID, ProviderId, IssuerEventId, UTCPriceAckTimeInSeconds, controlField);
        }

        /// <summary>
        /// Send simulated DRLC event status update to meter. This method is to be used for testing only.
        /// </summary>
        /// <param name="drlcEvent">The event to update</param>
        /// <param name="Status">The status we want to send</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/13 MP                 Created

        public void SendSimulatedDRLCEventStatusUpdate(DRLCEvent drlcEvent, byte Status)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            byte[] Message = new byte[60];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.DestinationEndpoint = drlcEvent.ServerEndpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID).Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.DRLC;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the message
            MessageWriter.Write(drlcEvent.IssuerEventID);
            MessageWriter.Write(Status);
            MessageWriter.Write((uint) (CurrentUTCTime - UTC_REFERENCE_TIME).TotalSeconds); // Event Status time - We should always be sending the update right when the change occurs
            MessageWriter.Write((byte) drlcEvent.CriticalityLevel); // Will always just set as 'emergency'
            MessageWriter.Write(drlcEvent.CoolingTemperatureSetPoint); // not going to worry about this one. 
            MessageWriter.Write(drlcEvent.HeatingTemperatureSetPoint); // not going to worry about this one. 
            MessageWriter.Write(drlcEvent.AverageLoadAdjustmentPercentage); // not going to worry about this one. 
            MessageWriter.Write(drlcEvent.DutyCycle); // not going to worry about this one. 
            MessageWriter.Write((byte) drlcEvent.EventControl); 
            //MessageWriter.Write(0x01); // ECDSA signature type.

            // The Signature portion of this message is optional in SE 1.1 but it's not clear from the spec how to send this for now we will just fill it all in with FF's
            for (int Index = 0; Index < 43; Index++)
            {
                MessageWriter.Write((byte)0xFF);
            }

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)DRLCClientCommands.ReportEventStatus;
            ZclFrame.Data = Message;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "DRLC Event Status Update - Event ID : " + drlcEvent.IssuerEventID.ToString() + " Status: " + drlcEvent.CurrentStatus.ToString());

            SendUnicastMessage(drlcEvent.ServerNodeID, ApsFrame, ZclFrame.FrameData);
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Device Classes that will be accepted for DRLC events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DRLCDeviceClasses DRLCDeviceClass
        {
            get
            {
                return m_DRLCDeviceClass;
            }
            set
            {
                if (IsJoined == false)
                {
                    m_DRLCDeviceClass = value;
                }
                else
                {
                    throw new InvalidOperationException("The DRLC Device Class cannot be changed while joined to a device");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Enrollment Group that will be used to accept DRLC events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte DRLCEnrollmentGroup
        {
            get
            {
                return m_DRLCEnrollmentGroup;
            }
            set
            {
                if (IsJoined == false)
                {
                    m_DRLCEnrollmentGroup = value;
                }
                else
                {
                    throw new InvalidOperationException("The DRLC Enrollment Group cannot be changed while joined to a device");
                }
            }
        }

        /// <summary>
        /// Gets the List of Scheduled DRLC Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public List<DRLCEvent> ScheduledDRLCEvents
        {
            get
            {
                return m_ScheduledDRLCEvents;
            }
        }

        /// <summary>
        /// Gets the list of Completed DRLC Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public List<DRLCEvent> CompletedDRLCEvents
        {
            get
            {
                return m_CompletedDRLCEvents;
            }
        }

        /// <summary>
        /// Gets the list of currently running DRLC Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  11/25/13 MP                 Added set. To nullify list for testing.

        public List<DRLCEvent> RunningDRLCEvents
        {
            get
            {
                // The running events are stored in a dictionary in order to keep track of cases where an event that has been started 
                // could be partially superseded. We will send the distinct list in case there are events that apply to multiple device classes
                return m_RunningDRLCEvents.Values.Distinct().ToList();
            }
            set 
            {
                // for testing only
                if (value == null)
                {
                    m_RunningDRLCEvents.Clear();
                }
                else
                {
                    // Going to keep the list the same, will not attempt to change it unless it is back to null
                    throw new InvalidOperationException("Cannot change list to anything besides null");
                }
            }
        }

        /// <summary>
        /// Gets unit of measure
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public byte UnitOfMeasure
        {
            get
            {
                return m_UnitOfMeasure;
            }
        }

        /// <summary>
        /// Gets multiplier
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public uint Multiplier
        {
            get
            {
                return m_Multiplier;
            }
        }

        /// <summary>
        /// Gets divisor
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public uint Divisor
        {
            get
            {
                return m_Divisor;
            }
        }

        /// <summary>
        /// Gets Summation format
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public byte SummationFormatting
        {
            get
            {
                return m_SummationFormatting;
            }
        }

        /// <summary>
        /// Gets Demand Formatting
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public byte DemandFormatting
        {
            get
            {
                return m_DemandFormatting;
            }
        }
        /// <summary>
        /// Gets historical consumption formatting
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public byte HistoricalConsumptionFormatting
        {
            get
            {
                return m_HistoricalConsumptionFormatting;
            }
        }
        /// <summary>
        /// Gets Metering device type
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public byte MeteringDeviceType
        {
            get
            {
                return m_MeteringDeviceType;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier1SummationDelivered
        {
            get
            {
                return m_CurrentTier1SummationDelivered;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Received
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier1SummationReceived
        {
            get
            {
                return m_CurrentTier1SummationReceived;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier2SummationDelivered
        {
            get
            {
                return m_CurrentTier2SummationDelivered;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier2SummationReceived
        {
            get
            {
                return m_CurrentTier2SummationReceived;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier3SummationDelivered
        {
            get
            {
                return m_CurrentTier3SummationDelivered;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Received
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier3SummationReceived
        {
            get
            {
                return m_CurrentTier3SummationReceived;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier4SummationDelivered
        {
            get
            {
                return m_CurrentTier4SummationDelivered;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Received
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier4SummationReceived
        {
            get
            {
                return m_CurrentTier4SummationReceived;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier5SummationDelivered
        {
            get
            {
                return m_CurrentTier5SummationDelivered;
            }
        }

        /// <summary>
        /// GetsCurrent Tier Summation Received
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public ulong CurrentTier5SummationReceived
        {
            get
            {
                return m_CurrentTier5SummationReceived;
            }
        }

        /// <summary>
        /// Gets status
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public byte MeterStatus
        {
            get
            {
                return m_Status;
            }
        }

        /// <summary>
        /// Gets/Set OTAFlowStatusVar
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  7/26/16 HM   4.70.13        Created
        //
        public byte OTAFlowStatusVar
        {
            get
            {
                return m_OTAFlowStatus;
            }
            set
            {

                m_OTAFlowStatus = value;
            }
        }

        /// <summary>
        /// Get OTA Image Upgrade Status
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  8/18/16 PGH  4.70.14        Created
        //
        public ImageUpgradeStatus OTAImageUpgradeStatus
        {
            get
            {
                return m_ImageUpgradeStatus;
            }
            set
            {
                m_ImageUpgradeStatus = value;
            }
        }

        /// <summary>
        /// Gets instantaneous demand
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public int InstantaneousDemand
        {
            get
            {
                return m_InstantaneousDemand;
            }
        }

        /// <summary>
        /// Gets current day consumption delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public uint CurrentDayConsumptionDelivered
        {
            get
            {
                return m_CurrentDayConsumptionDelivered;
            }
        }

        /// <summary>
        /// Gets current day consumption received
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public uint CurrentDayConsumptionReceived
        {
            get
            {
                return m_CurrentDayConsumptionReceived;
            }
        }

        /// <summary>
        /// Gets Previous day consumption delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public uint PreviousDayConsumptionDelivered
        {
            get
            {
                return m_PreviousDayConsumptionDelivered;
            }
        }

        /// <summary>
        /// Gets Previous day consumption received
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public uint PreviousDayConsumptionReceived
        {
            get
            {
                return m_PreviousDayConsumptionReceived;
            }
        }

        /// <summary>
        /// Gets Current partial profile interval start time delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public DateTime CurrentPartialProfileIntervalStartTimeDelivered
        {
            get
            {
                return m_CurrentPartialProfileIntervalStartTimeDelivered;
            }
        }

        /// <summary>
        /// Gets Current partial profile interval start time received
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public DateTime CurrentPartialProfileIntervalStartTimeReceived
        {
            get
            {
                return m_CurrentPartialProfileIntervalStartTimeReceived;
            }
        }

        /// <summary>
        /// Gets Current partial profile interval value delivered
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public uint CurrentPartialProfileIntervalValueDelivered
        {
            get
            {
                return m_CurrentPartialProfileIntervalValueDelivered;
            }
        }

        /// <summary>
        /// Gets Current partial profile interval value received
        /// </summary>
        /// Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/20/13 MP                 Created

        public uint CurrentPartialProfileIntervalValueReceived
        {
            get
            {
                return m_CurrentPartialProfileIntervalValueReceived;
            }
        }

        /// <summary>
        /// Gets the last retrieved Current Summation delivered value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        public double CurrentSummationDelivered
        {
            get
            {
                double Value = 0.0;

                if (m_Divisor != 0)
                {
                    Value = (double)m_CurrentSummationDelivered * m_Multiplier / m_Divisor;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Current Summation received value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        public double CurrentSummationReceived
        {
            get
            {
                double Value = 0.0;

                if (m_Divisor != 0)
                {
                    Value = (double)m_CurrentSummationReceived * m_Multiplier / m_Divisor;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Current Max Demand delivered value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        public double CurrentMaxDemandDelivered
        {
            get
            {
                double Value = 0.0;

                if (m_Divisor != 0)
                {
                    Value = (double)m_CurrentMaxDemandDelivered * m_Multiplier / m_Divisor;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Current Max Demand received value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        public double CurrentMaxDemandReceived
        {
            get
            {
                double Value = 0.0;

                if (m_Divisor != 0)
                {
                    Value = (double)m_CurrentMaxDemandReceived * m_Multiplier / m_Divisor;
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Current Block Period Consumption Delivered value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public ulong? CurrentBlockPeriodConsumptionDelivered
        {
            get
            {
                return m_CurrentBlockPeriodConsumptionDelivered;
            }
        }

        /// <summary>
        /// Gets the last retrieved Previous Block Period Consumption Delivered value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/06/12 PGH 2.60.30        Created

        public ulong? PreviousBlockPeriodConsumptionDelivered
        {
            get
            {
                return m_PreviousBlockPeriodConsumptionDelivered;
            }
        }

        /// <summary>
        /// Gets the last DFT summation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public ulong DFTSummation
        {
            get
            {
                return m_DFTSummation;
            }
        }

        /// <summary>
        /// Gets the last DFT
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public UInt16 DailyFreezeTime
        {
            get
            {
                return m_DailyFreezeTime;
            }
        }

        /// <summary>
        /// Gets the last Power Factor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public byte PowerFactor
        {
            get
            {
                return m_PowerFactor;
            }
        }

        /// <summary>
        /// Gets the last Reading Snap Shot Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public DateTime ReadingSnapShotTime
        {
            get
            {
                return m_ReadingSnapShotTime;
            }
        }

        /// <summary>
        /// Gets the last Default Update Period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public byte DefaultUpdatePeriod
        {
            get
            {
                return m_DefaultUpdatePeriod;
            }
        }

        /// <summary>
        /// Gets the last Fast Poll Update Period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created

        public byte FastPollUpdatePeriod
        {
            get
            {
                return m_FastPollUpdatePeriod;
            }
        }

        /// <summary>
        /// Gets the Maximum Number of Periods Delivered value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/30/12 PGH 2.60.49        Created

        public byte? MaxNumberOfPeriodsDelivered
        {
            get
            {
                return m_MaxNumberOfPeriodsDelivered;
            }
        }

        /// <summary>
        /// Gets the time of occurrence for the Current Max Demand Delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        public DateTime CurrentMaxDemandDeliveredTime
        {
            get
            {
                return m_CurrentMaxDemandDeliveredTime;
            }
        }

        /// <summary>
        /// Gets the time of occurrence for the Current Max Demand Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.28        Created

        public DateTime CurrentMaxDemandReceivedTime
        {
            get
            {
                return m_CurrentMaxDemandReceivedTime;
            }
        }

        /// <summary>
        /// Gets the Tier1PriceLabel
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public string Tier1PriceLabel
        {
            get
            {
                return m_Tier1PriceLabel;
            }
        }

        /// <summary>
        /// Gets the Tier2PriceLabel
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public string Tier2PriceLabel
        {
            get
            {
                return m_Tier2PriceLabel;
            }
        }

        /// <summary>
        /// Gets the Tier3PriceLabel
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public string Tier3PriceLabel
        {
            get
            {
                return m_Tier3PriceLabel;
            }
        }

        /// <summary>
        /// Gets the Tier4PriceLabel
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public string Tier4PriceLabel
        {
            get
            {
                return m_Tier4PriceLabel;
            }
        }

        /// <summary>
        /// Gets the Tier5PriceLabel
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public string Tier5PriceLabel
        {
            get
            {
                return m_Tier5PriceLabel;
            }
        }

        /// <summary>
        /// Gets the Current Billing Period Start
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/06/12 PGH 2.60.30        Created

        public DateTime? CurrentBillingPeriodStart
        {
            get
            {
                return m_CurrentBillingPeriodStart;
            }
        }

        /// <summary>
        /// Gets the Current Billing Period Duration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/06/12 PGH 2.60.30        Created

        public uint? CurrentBillingPeriodDuration
        {
            get
            {
                return m_CurrentBillingPeriodDuration;
            }
        }

        /// <summary>
        /// Gets the time of occurrence for the Start of Block Period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public DateTime? CurrentBlockPeriodStart
        {
            get
            {
                return m_StartOfBlockPeriod;
            }
        }

        /// <summary>
        /// Gets Block Period Duration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public uint? CurrentBlockPeriodDuration
        {
            get
            {
                return m_BlockPeriodDuration;
            }
        }

        /// <summary>
        /// Gets the Threshold Multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public uint? ThresholdMultiplier
        {
            get
            {
                return m_ThresholdMultiplier;
            }
        }

        /// <summary>
        /// Gets the Threshold Divisor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public uint? ThresholdDivisor
        {
            get
            {
                return m_ThresholdDivisor;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block1Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block1Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block1Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block1Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block2Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block2Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block2Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block2Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block3Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block3Threshold
        {
            get
            {

                double? Value = null;

                if ((m_Block3Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block3Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block4Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block4Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block4Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block4Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block5Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block5Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block5Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block5Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block6Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block6Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block6Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block6Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block7Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block7Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block7Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block7Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block8Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block8Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block8Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block8Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block9Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block9Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block9Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block9Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block10Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block10Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block10Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block10Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block11Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block11Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block11Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block11Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block12Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block12Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block12Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block12Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block13Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block13Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block13Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block13Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block14Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block14Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block14Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block14Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the last retrieved Block15Threshold value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/12 PGH 2.60.30        Created

        public double? Block15Threshold
        {
            get
            {
                double? Value = null;

                if ((m_Block15Threshold != null) && (m_ThresholdDivisor != null) && (m_ThresholdMultiplier != null) && (m_ThresholdDivisor != 0))
                {
                    Value = (double)(m_Block15Threshold * m_ThresholdMultiplier / m_ThresholdDivisor);
                }

                return Value;
            }
        }

        /// <summary>
        /// Gets the NoTierBlock1Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock1Price
        {
            get
            {
                return m_NoTierBlock1Price;
            }
        }

        /// <summary>
        /// Gets the NoTierBlock2Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock2Price
        {
            get
            {
                return m_NoTierBlock2Price;
            }
        }

        /// <summary>
        /// Gets the NoTierBlock3Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock3Price
        {
            get
            {
                return m_NoTierBlock3Price;
            }
        }

        /// <summary>
        /// Gets the NoTierBlock4Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock4Price
        {
            get
            {
                return m_NoTierBlock4Price;
            }
        }

        /// <summary>
        /// Gets the NoTierBlock5Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock5Price
        {
            get
            {
                return m_NoTierBlock5Price;
            }
        }

        /// <summary>
        /// Gets the NoTierBlock6Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock6Price
        {
            get
            {
                return m_NoTierBlock6Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock7Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock7Price
        {
            get
            {
                return m_NoTierBlock7Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock8Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock8Price
        {
            get
            {
                return m_NoTierBlock8Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock9Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock9Price
        {
            get
            {
                return m_NoTierBlock9Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock10Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock10Price
        {
            get
            {
                return m_NoTierBlock10Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock11Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock11Price
        {
            get
            {
                return m_NoTierBlock11Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock12Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock12Price
        {
            get
            {
                return m_NoTierBlock12Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock13Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock13Price
        {
            get
            {
                return m_NoTierBlock13Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock14Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock14Price
        {
            get
            {
                return m_NoTierBlock14Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock15Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock15Price
        {
            get
            {
                return m_NoTierBlock15Price;
            }
        }
        /// <summary>
        /// Gets the NoTierBlock16Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? NoTierBlock16Price
        {
            get
            {
                return m_NoTierBlock16Price;
            }
        }

        /// <summary>
        /// Gets the Tier1Block1Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block1Price
        {
            get
            {
                return m_Tier1Block1Price;
            }
        }

        /// <summary>
        /// Gets the Tier1Block2Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block2Price
        {
            get
            {
                return m_Tier1Block2Price;
            }
        }

        /// <summary>
        /// Gets the Tier1Block3Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block3Price
        {
            get
            {
                return m_Tier1Block3Price;
            }
        }

        /// <summary>
        /// Gets the Tier1Block4Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block4Price
        {
            get
            {
                return m_Tier1Block4Price;
            }
        }

        /// <summary>
        /// Gets the Tier1Block5Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block5Price
        {
            get
            {
                return m_Tier1Block5Price;
            }
        }

        /// <summary>
        /// Gets the Tier1Block6Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block6Price
        {
            get
            {
                return m_Tier1Block6Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block7Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block7Price
        {
            get
            {
                return m_Tier1Block7Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block8Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block8Price
        {
            get
            {
                return m_Tier1Block8Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block9Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block9Price
        {
            get
            {
                return m_Tier1Block9Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block10Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block10Price
        {
            get
            {
                return m_Tier1Block10Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block11Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block11Price
        {
            get
            {
                return m_Tier1Block11Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block12Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block12Price
        {
            get
            {
                return m_Tier1Block12Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block13Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block13Price
        {
            get
            {
                return m_Tier1Block13Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block14Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block14Price
        {
            get
            {
                return m_Tier1Block14Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block15Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block15Price
        {
            get
            {
                return m_Tier1Block15Price;
            }
        }
        /// <summary>
        /// Gets the Tier1Block16Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier1Block16Price
        {
            get
            {
                return m_Tier1Block16Price;
            }
        }

        /// <summary>
        /// Gets the Tier2Block1Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block1Price
        {
            get
            {
                return m_Tier2Block1Price;
            }
        }

        /// <summary>
        /// Gets the Tier2Block2Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block2Price
        {
            get
            {
                return m_Tier2Block2Price;
            }
        }

        /// <summary>
        /// Gets the Tier2Block3Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block3Price
        {
            get
            {
                return m_Tier2Block3Price;
            }
        }

        /// <summary>
        /// Gets the Tier2Block4Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block4Price
        {
            get
            {
                return m_Tier2Block4Price;
            }
        }

        /// <summary>
        /// Gets the Tier2Block5Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block5Price
        {
            get
            {
                return m_Tier2Block5Price;
            }
        }

        /// <summary>
        /// Gets the Tier2Block6Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block6Price
        {
            get
            {
                return m_Tier2Block6Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block7Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block7Price
        {
            get
            {
                return m_Tier2Block7Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block8Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block8Price
        {
            get
            {
                return m_Tier2Block8Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block9Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block9Price
        {
            get
            {
                return m_Tier2Block9Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block10Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block10Price
        {
            get
            {
                return m_Tier2Block10Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block11Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block11Price
        {
            get
            {
                return m_Tier2Block11Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block12Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block12Price
        {
            get
            {
                return m_Tier2Block12Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block13Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block13Price
        {
            get
            {
                return m_Tier2Block13Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block14Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block14Price
        {
            get
            {
                return m_Tier2Block14Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block15Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block15Price
        {
            get
            {
                return m_Tier2Block15Price;
            }
        }
        /// <summary>
        /// Gets the Tier2Block16Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier2Block16Price
        {
            get
            {
                return m_Tier2Block16Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block1Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block1Price
        {
            get
            {
                return m_Tier3Block1Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block2Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block2Price
        {
            get
            {
                return m_Tier3Block2Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block3Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block3Price
        {
            get
            {
                return m_Tier3Block3Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block4Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block4Price
        {
            get
            {
                return m_Tier3Block4Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block5Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block5Price
        {
            get
            {
                return m_Tier3Block5Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block6Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block6Price
        {
            get
            {
                return m_Tier3Block6Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block7Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block7Price
        {
            get
            {
                return m_Tier3Block7Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block8Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block8Price
        {
            get
            {
                return m_Tier3Block8Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block9Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block9Price
        {
            get
            {
                return m_Tier3Block9Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block10Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block10Price
        {
            get
            {
                return m_Tier3Block10Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block11Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block11Price
        {
            get
            {
                return m_Tier3Block11Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block12Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block12Price
        {
            get
            {
                return m_Tier3Block12Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block13Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block13Price
        {
            get
            {
                return m_Tier3Block13Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block14Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block14Price
        {
            get
            {
                return m_Tier3Block14Price;
            }
        }
        /// <summary>
        /// Gets the Tier3Block15Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block15Price
        {
            get
            {
                return m_Tier3Block15Price;
            }
        }

        /// <summary>
        /// Gets the Tier3Block16Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier3Block16Price
        {
            get
            {
                return m_Tier3Block16Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block1Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block1Price
        {
            get
            {
                return m_Tier4Block1Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block2Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block2Price
        {
            get
            {
                return m_Tier4Block2Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block3Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block3Price
        {
            get
            {
                return m_Tier4Block3Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block4Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block4Price
        {
            get
            {
                return m_Tier4Block4Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block5Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block5Price
        {
            get
            {
                return m_Tier4Block5Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block6Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block6Price
        {
            get
            {
                return m_Tier4Block6Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block7Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block7Price
        {
            get
            {
                return m_Tier4Block7Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block8Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block8Price
        {
            get
            {
                return m_Tier4Block8Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block9Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block9Price
        {
            get
            {
                return m_Tier4Block9Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block10Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block10Price
        {
            get
            {
                return m_Tier4Block10Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block11Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block11Price
        {
            get
            {
                return m_Tier4Block11Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block12Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block12Price
        {
            get
            {
                return m_Tier4Block12Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block13Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block13Price
        {
            get
            {
                return m_Tier4Block13Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block14Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block14Price
        {
            get
            {
                return m_Tier4Block14Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block15Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block15Price
        {
            get
            {
                return m_Tier4Block15Price;
            }
        }

        /// <summary>
        /// Gets the Tier4Block16Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/05/12 PGH 2.60.30        Created

        public uint? Tier4Block16Price
        {
            get
            {
                return m_Tier4Block16Price;
            }
        }

        /// <summary>
        /// Gets/Sets the last message received from the meter
        /// </summary>
        public AMIHANMsgRcd LastMessage
        {
            get
            {
                return m_LastMessage;
            }
            set
            {
                m_LastMessage = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Last Price Received
        /// </summary>
        public PublishPriceRcd LastPriceReceived
        {
            get
            {
                return m_LastPriceReceived;
            }
            set
            {
                m_LastPriceReceived = value;
            }
        }




        #endregion

        #region Protected Methods

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
        //  08/18/16 PGH 4.70.14        Added OTA

        protected override byte[] GetAttributeResponse(ushort clusterID, ushort attribute)
        {
            byte[] Response = null;

            switch (clusterID)
            {
                case (ushort)SmartEnergyClusters.DRLC:
                {
                    Response = GetDRLCAttributeResponse(attribute);
                    break;
                }
                case (ushort)GeneralClusters.OTA:
                {
                    Response = GetOTAAttributeResponse(attribute);
                    break;
                }
                default:
                {
                    Response = base.GetAttributeResponse(clusterID, attribute);
                    break;
                }
            }

            return Response;
        }

        /// <summary>
        /// Handles the Received Messages
        /// </summary>
        /// <param name="receivedMessage">The message that was received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected override void HandleZCLMessage(IncomingMessage receivedMessage)
        {
            switch (receivedMessage.APSFrame.ProfileID)
            {
                case (ushort)ZigBeeProfileIDs.SmartEnergy:
                {
                    HandleSmartEnergyMessage(receivedMessage);
                    break;
                }
                case (ushort)ZigBeeProfileIDs.ItronPrivateProfile:
                {
                    base.HandleItronPrivateProfileMessage(receivedMessage);
                    break;
                }
                default:
                {
                    base.HandleZCLMessage(receivedMessage);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets up any clusters that the device is hosting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        protected override void SetUpClusterLists()
        {
            base.SetUpClusterLists();

            ZigBeeEndpointInfo NewEndpoint = new ZigBeeEndpointInfo();

            NewEndpoint.Endpoint = DEFAULT_SE_ENDPOINT;
            NewEndpoint.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            NewEndpoint.AppFlags = DEFAULT_SE_DEVICE_VERSION;
            NewEndpoint.DeviceID = DEFAULT_SE_DEVICE_ID;

            // Set up the Client Side List
            NewEndpoint.ClientClusterList.Add((ushort)GeneralClusters.Basic);
            NewEndpoint.ClientClusterList.Add((ushort)GeneralClusters.Time);
            NewEndpoint.ClientClusterList.Add((ushort)GeneralClusters.OTA);
            NewEndpoint.ClientClusterList.Add((ushort)SmartEnergyClusters.KeyEstablishment);
            NewEndpoint.ClientClusterList.Add((ushort)SmartEnergyClusters.DRLC);
            NewEndpoint.ClientClusterList.Add((ushort)SmartEnergyClusters.Price);
            NewEndpoint.ClientClusterList.Add((ushort)SmartEnergyClusters.SimpleMetering);
            NewEndpoint.ClientClusterList.Add((ushort)SmartEnergyClusters.Messaging);

            // Set up the Server Side List
            NewEndpoint.ServerClusterList.Add((ushort)GeneralClusters.Basic);
            NewEndpoint.ServerClusterList.Add((ushort)SmartEnergyClusters.KeyEstablishment);

            m_Endpoints.Add(NewEndpoint);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles incoming Smart Energy Messages
        /// </summary>
        /// <param name="receivedMessage">The incoming message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void HandleSmartEnergyMessage(IncomingMessage receivedMessage)
        {
            switch (receivedMessage.APSFrame.ClusterID)
            {
                case (ushort)SmartEnergyClusters.KeyEstablishment:
                {
                    HandleKeyEstablishmentMessage(receivedMessage);
                    break;
                }
                case (ushort)SmartEnergyClusters.DRLC:
                {
                    HandleDRLCMessage(receivedMessage);
                    break;
                }
                case (ushort)SmartEnergyClusters.SimpleMetering:
                {
                    HandleSimpleMeteringMessage(receivedMessage);
                    break;
                }
                case (ushort)SmartEnergyClusters.Messaging:
                {
                    HandleMessagingMessage(receivedMessage);
                    break;
                }
                case (ushort)SmartEnergyClusters.Price:
                {
                    HandlePriceMessage(receivedMessage);
                    break;
                }
                case (ushort)GeneralClusters.Identify:
                {
                    HandleIdentifyMessage(receivedMessage);
                    break;
                }
                case (ushort)GeneralClusters.OTA:
                {
                    HandleOTAMessage(receivedMessage);
                    break;
                } 
                default:
                {
                    // Not sure what to do with this message so add it to the list of unhandled messages
                    m_UnhandledMessages.Add(receivedMessage);
                    break;
                }
            }
        }

        #endregion       

        #region DRLC

        /// <summary>
        /// Handles incoming DRLC Messages
        /// </summary>
        /// <param name="receivedMessage">The DRLC message that was received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void HandleDRLCMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);

            switch (ZclFrame.CommandID)
            {
                case (byte)DRLCServerCommands.LoadControlEvent:
                {
                    DRLCEvent NewEvent = new DRLCEvent();
                    NewEvent.EventData = ZclFrame.Data;

                    NewEvent.ServerNodeID = receivedMessage.SenderNodeID;
                    NewEvent.ServerEndpoint = receivedMessage.APSFrame.SourceEndpoint;

                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "DRLC Event Message Received - ID: " + NewEvent.IssuerEventID.ToString()
                        + " Start Time: " + NewEvent.StartTime.ToString("G") + " Duration: " + NewEvent.Duration.ToString());

                    if (ShouldDeviceHandleDRLCEvent(NewEvent.UtilityEnrollmentGroup, NewEvent.DeviceClasses) == false)
                    {
                        // This event does not apply to this device
                        NewEvent.CurrentStatus = DRLCEventStatus.EventRejected;
                        m_CompletedDRLCEvents.Add(NewEvent);
                    }
                    else if (m_ScheduledDRLCEvents.Where(e => e.IssuerEventID == NewEvent.IssuerEventID).Count() == 0
                        && m_RunningDRLCEvents.Values.Where(e => e.IssuerEventID == NewEvent.IssuerEventID).Count() == 0)
                    {
                        // This is a new event that we don't know about so we should handle it.
                        if (NewEvent.StartTime <= CurrentUTCTime)
                        {
                            StartDRLCEvent(NewEvent);
                            OnDRLCEventReceived(NewEvent); // If the handler is registered then we want to see the event being handled with that
                        }
                        else if (NewEvent.StartTime.AddMinutes(NewEvent.Duration) < CurrentUTCTime)
                        {
                            NewEvent.CurrentStatus = DRLCEventStatus.RejectedEventReceivedAfterExpiration;
                            m_CompletedDRLCEvents.Add(NewEvent);

                            SendDRLCEventStatusUpdate(NewEvent);
                        }
                        else
                        {
                            NewEvent.CurrentStatus = DRLCEventStatus.CommandReceived;
                            m_ScheduledDRLCEvents.Add(NewEvent);

                            // If the handler is registered then we want to see the event being handled with that
                            OnDRLCEventReceived(NewEvent); 

                            // Schedule the event to start
                            NewEvent.ScheduledEventOccurred += m_DRLCScheduledEventHandler;
                            NewEvent.ScheduleEvent((long)(NewEvent.StartTime - CurrentUTCTime).TotalMilliseconds);
                        }
                    }

                    break;
                }
                case (byte)DRLCServerCommands.CancelLoadControlEvent:
                {
                    uint CancelledEventID = DataReader.ReadUInt32();
                    DRLCDeviceClasses DeviceClass = (DRLCDeviceClasses)DataReader.ReadUInt16();
                    byte EnrollmentGroup = DataReader.ReadByte();
                    DRLCEventControl CancelControl = (DRLCEventControl)DataReader.ReadByte();
                    uint EffectiveTimeSeconds = DataReader.ReadUInt32();
                    DateTime EffectiveTime = UTC_REFERENCE_TIME.AddSeconds(EffectiveTimeSeconds);

                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Cancel DRLC Event Message Received - ID: " + CancelledEventID.ToString() + " Effective Time: " + EffectiveTime.ToString("G"));

                    if (ShouldDeviceHandleDRLCEvent(EnrollmentGroup, DeviceClass))
                    {
                        if (m_ScheduledDRLCEvents.Where(e => e.IssuerEventID == CancelledEventID).Count() > 0)
                        {
                            DRLCEvent CancelledEvent = m_ScheduledDRLCEvents.First(e => e.IssuerEventID == CancelledEventID);

                            if (EffectiveTime > CancelledEvent.StartTime.AddMinutes(CancelledEvent.Duration))
                            {
                                // The cancel is scheduled to occur after the event completes so we should reject the cancellation
                                DRLCEvent FailedEvent = new DRLCEvent();
                                FailedEvent.EventData = CancelledEvent.EventData;
                                FailedEvent.CurrentStatus = DRLCEventStatus.RejectedInvalidCancelEffectiveTime;

                                SendDRLCEventStatusUpdate(FailedEvent);
                            }
                            else if (EffectiveTime <= CurrentUTCTime)
                            {
                                // Cancel it now
                                CancelledEvent.CurrentStatus = DRLCEventStatus.Cancelled;
                                CancelledEvent.CancelScheduledEvent();
                                CancelledEvent.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;
                                m_ScheduledDRLCEvents.Remove(CancelledEvent);
                                m_CompletedDRLCEvents.Add(CancelledEvent);

                                SendDRLCEventStatusUpdate(CancelledEvent);
                            }
                            else
                            {
                                // We need to schedule the cancellation
                                CancelledEvent.CancelPending = true;
                                CancelledEvent.ScheduleEvent((long)(EffectiveTime - CurrentUTCTime).TotalMilliseconds);
                            }
                        }
                        else if (m_RunningDRLCEvents.Values.Where(e => e.IssuerEventID == CancelledEventID).Count() > 0)
                        {
                            DRLCEvent CancelledEvent = m_RunningDRLCEvents.Values.First(e => e.IssuerEventID == CancelledEventID);

                            if (EffectiveTime > CancelledEvent.StartTime.AddMinutes(CancelledEvent.Duration))
                            {
                                // The cancel is scheduled to occur after the event completes so we should reject the cancellation
                                DRLCEvent FailedEvent = new DRLCEvent();
                                FailedEvent.EventData = CancelledEvent.EventData;
                                FailedEvent.CurrentStatus = DRLCEventStatus.RejectedInvalidCancelEffectiveTime;

                                SendDRLCEventStatusUpdate(FailedEvent);
                            }
                            else if (EffectiveTime <= CurrentUTCTime)
                            {
                                // Cancel it now
                                CancelledEvent.CurrentStatus = DRLCEventStatus.Cancelled;
                                RemoveRunningDRLCEvent(CancelledEvent);

                                SendDRLCEventStatusUpdate(CancelledEvent);
                            }
                            else
                            {
                                // We need to schedule the cancellation
                                CancelledEvent.CancelPending = true;
                                CancelledEvent.ScheduleEvent((long)(EffectiveTime - CurrentUTCTime).TotalMilliseconds);
                            }
                        }
                        else
                        {
                            // We don't know of this event so we need to notify the meter of that
                            DRLCEvent FailureEvent = new DRLCEvent(CancelledEventID, DeviceClass, EnrollmentGroup, DRLCEventStatus.RejectedInvalidCancelUndefinedEvent);

                            SendDRLCEventStatusUpdate(FailureEvent);
                        }
                    }

                    break;
                }
                case (byte)DRLCServerCommands.CancelAllLoadControlEvents:
                {
                    DRLCCancelControl CancelControl = (DRLCCancelControl)DataReader.ReadByte();

                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Cancel All DRLC Events Message Received");

                    // Go through each scheduled event and cancel it
                    foreach (DRLCEvent CurrentEvent in m_ScheduledDRLCEvents)
                    {
                        // Cancel the event
                        CurrentEvent.CurrentStatus = DRLCEventStatus.Cancelled;
                        CurrentEvent.CancelScheduledEvent();
                        CurrentEvent.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;
                        m_CompletedDRLCEvents.Add(CurrentEvent);

                        SendDRLCEventStatusUpdate(CurrentEvent);
                    }

                    m_ScheduledDRLCEvents.Clear();

                    // Cancel all of the running events
                    foreach (DRLCEvent CurrentEvent in m_RunningDRLCEvents.Values)
                    {
                        // Cancel the event
                        CurrentEvent.CurrentStatus = DRLCEventStatus.Cancelled;
                        CurrentEvent.CancelScheduledEvent();
                        CurrentEvent.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;
                        m_CompletedDRLCEvents.Add(CurrentEvent);

                        SendDRLCEventStatusUpdate(CurrentEvent);
                    }

                    m_RunningDRLCEvents.Clear();

                    break;
                }
                default:
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unknown DRLC command received. Command ID: " + ZclFrame.CommandID.ToString("X2", CultureInfo.InvariantCulture));
                    // Unknown command so we should send a default response
                    SendDefaultResponse((ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.DRLC,
                        receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, ZCLStatus.UnsupportedClusterCommand);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the response value for the DRLC Client Side Attributes
        /// </summary>
        /// <param name="attribute">The attribute to get</param>
        /// <returns>The response for the attribute read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private byte[] GetDRLCAttributeResponse(ushort attribute)
        {
            byte[] AttributeResponse = null;
            MemoryStream ResponseStream = null;
            BinaryWriter ResponseWriter = null;

            bool Supported = true;

            switch (attribute)
            {
                case (ushort)DRLCClientAttributes.DeviceClassValue:
                {
                    AttributeResponse = new byte[6];
                    ResponseStream = new MemoryStream(AttributeResponse);
                    ResponseWriter = new BinaryWriter(ResponseStream);

                    ResponseWriter.Write(attribute);
                    ResponseWriter.Write((byte)ZCLStatus.Success);
                    ResponseWriter.Write((byte)ZCLDataType.Uint8);
                    ResponseWriter.Write((ushort)m_DRLCDeviceClass);

                    break;
                }
                case (ushort)DRLCClientAttributes.StartRandomizeMinutes:
                {
                    AttributeResponse = new byte[5];
                    ResponseStream = new MemoryStream(AttributeResponse);
                    ResponseWriter = new BinaryWriter(ResponseStream);

                    ResponseWriter.Write(attribute);
                    ResponseWriter.Write((byte)ZCLStatus.Success);
                    ResponseWriter.Write((byte)ZCLDataType.Uint8);
                    ResponseWriter.Write(m_DRLCStartRandomizeMinutes);

                    break;
                }
                case (ushort)DRLCClientAttributes.StopRandomizeMinutes:
                {
                    AttributeResponse = new byte[5];
                    ResponseStream = new MemoryStream(AttributeResponse);
                    ResponseWriter = new BinaryWriter(ResponseStream);

                    ResponseWriter.Write(attribute);
                    ResponseWriter.Write((byte)ZCLStatus.Success);
                    ResponseWriter.Write((byte)ZCLDataType.Uint8);
                    ResponseWriter.Write(m_DRLCStopRandomizeMinutes);

                    break;
                }
                case (ushort)DRLCClientAttributes.UtilityEnrollmentGroup:
                {
                    AttributeResponse = new byte[5];
                    ResponseStream = new MemoryStream(AttributeResponse);
                    ResponseWriter = new BinaryWriter(ResponseStream);

                    ResponseWriter.Write(attribute);
                    ResponseWriter.Write((byte)ZCLStatus.Success);
                    ResponseWriter.Write((byte)ZCLDataType.Uint8);
                    ResponseWriter.Write(m_DRLCEnrollmentGroup);

                    break;
                }
                default:
                {
                    Supported = false;
                    break;
                }
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
        /// Gets the response value for the OTA Upgrade Cluster Client Side Attributes
        /// </summary>
        /// <param name="attribute">The attribute to get</param>
        /// <returns>The response for the attribute read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/18/16 PGH 4.70.14 701952  Created
        //
        private byte[] GetOTAAttributeResponse(ushort attribute)
        {
            byte[] AttributeResponse = null;
            MemoryStream ResponseStream = null;
            BinaryWriter ResponseWriter = null;

            bool Supported = true;

            switch (attribute)
            {
                case (ushort)OTAClientAttributes.UpgradeServerId:
                    {
                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_UpgradeServerId));

                        AttributeResponse = new byte[12];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.IEEEAddress);
                        ResponseWriter.Write((ulong)m_UpgradeServerId);

                        break;
                    }
                case (ushort)OTAClientAttributes.FileOffset:
                    {
                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_FileOffset));

                        AttributeResponse = new byte[8];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.Uint32);
                        ResponseWriter.Write((uint)m_FileOffset);

                        break;
                    }
                case (ushort)OTAClientAttributes.CurrentFileVersion:
                    {
                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_CurrentFileVersion));

                        AttributeResponse = new byte[8];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.Uint32);
                        ResponseWriter.Write((uint)m_CurrentFileVersion);

                        break;
                    }
                case (ushort)OTAClientAttributes.CurrentZigBeeStackVersion:
                    {
                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_CurrentZigBeeStackVersion));

                        AttributeResponse = new byte[6];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.Uint16);
                        ResponseWriter.Write((ushort)m_CurrentZigBeeStackVersion);

                        break;
                    }
                case (ushort)OTAClientAttributes.DownloadedFileVersion:
                    {
                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_DownloadFileVersion));

                        AttributeResponse = new byte[8];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.Uint32);
                        ResponseWriter.Write((uint)m_DownloadFileVersion);

                        break;
                    }
                case (ushort)OTAClientAttributes.DownloadedZigBeeStackVersion:
                    {
                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_DownloadZigBeeStackVersion));

                        AttributeResponse = new byte[6];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.Uint16);
                        ResponseWriter.Write((ushort)m_DownloadZigBeeStackVersion);

                        break;
                    }
                case (ushort)OTAClientAttributes.ImageUpgradeStatus:
                    {

                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_ImageUpgradeStatus));

                        AttributeResponse = new byte[5];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.Enum8);
                        ResponseWriter.Write((byte)m_ImageUpgradeStatus);

                        break;
                    }
                case (ushort)OTAClientAttributes.ManufacturerId:
                    {
                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_ManufacturerId));

                        AttributeResponse = new byte[6];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.Uint16);
                        ResponseWriter.Write((ushort)m_ManufacturerId);

                        break;
                    }
                case (ushort)OTAClientAttributes.ImageTypeId:
                    {
                        OTAAttributeRequest(this, new AttributeResponsePayload(attribute, (object)m_ImageTypeId));

                        AttributeResponse = new byte[6];
                        ResponseStream = new MemoryStream(AttributeResponse);
                        ResponseWriter = new BinaryWriter(ResponseStream);

                        ResponseWriter.Write(attribute);
                        ResponseWriter.Write((byte)ZCLStatus.Success);
                        ResponseWriter.Write((byte)ZCLDataType.Uint16);
                        ResponseWriter.Write((ushort)m_ImageTypeId);

                        break;
                    }
                default:
                    {
                        Supported = false;
                        break;
                    }
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
        /// Send the message to update the DRLC server with the current status of an event
        /// </summary>
        /// <param name="drlcEvent">The event to update</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void SendDRLCEventStatusUpdate(DRLCEvent drlcEvent)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            ushort ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            byte[] Message = new byte[60];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = ProfileID;
            ApsFrame.DestinationEndpoint = drlcEvent.ServerEndpoint;
            ApsFrame.SourceEndpoint = m_Endpoints.First(e => e.ProfileID == ProfileID).Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.DRLC;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the message
            MessageWriter.Write(drlcEvent.IssuerEventID);
            MessageWriter.Write((byte)drlcEvent.CurrentStatus);
            MessageWriter.Write((uint)(CurrentUTCTime - UTC_REFERENCE_TIME).TotalSeconds); // Event Status time - We should always be sending the update right when the change occurs
            MessageWriter.Write((byte)drlcEvent.CriticalityLevel);
            MessageWriter.Write(drlcEvent.CoolingTemperatureSetPoint);
            MessageWriter.Write(drlcEvent.HeatingTemperatureSetPoint);
            MessageWriter.Write(drlcEvent.AverageLoadAdjustmentPercentage);
            MessageWriter.Write(drlcEvent.DutyCycle);
            MessageWriter.Write((byte)drlcEvent.EventControl);

            // The Signature portion of this message is optional in SE 1.1 but it's not clear from the spec how to send this for now we will just fill it all in with FF's
            for (int Index = 0; Index < 43; Index++)
            {
                MessageWriter.Write((byte)0xFF);
            }

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)DRLCClientCommands.ReportEventStatus;
            ZclFrame.Data = Message;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "DRLC Event Status Update - Event ID : " + drlcEvent.IssuerEventID.ToString() + " Status: " + drlcEvent.CurrentStatus.ToString());

            SendUnicastMessage(drlcEvent.ServerNodeID, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Determines whether or not this device 
        /// </summary>
        /// <param name="enrollmentGroup">The enrollment group for the DRLC event</param>
        /// <param name="deviceClass">The device classes for the DRLC event</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private bool ShouldDeviceHandleDRLCEvent(byte enrollmentGroup, DRLCDeviceClasses deviceClass)
        {
            bool HandleEvent = false;

            // If the enrollment group doesn't match or either the device or event Enrollment group is not zero we shouldn't handle it
            if (m_DRLCEnrollmentGroup == 0 || enrollmentGroup == 0 || m_DRLCEnrollmentGroup == enrollmentGroup)
            {
                // Make sure that the event is for our device class
                foreach (DRLCDeviceClasses CurrentClass in Enum.GetValues(typeof(DRLCDeviceClasses)))
                {
                    if (m_DRLCDeviceClass.HasFlag(CurrentClass) && deviceClass.HasFlag(CurrentClass))
                    {
                        HandleEvent = true;
                        break;
                    }
                }
            }

            return HandleEvent;
        }

        /// <summary>
        /// Handles scheduled event changes for DRLC events
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  12/30/14 jrf 4.50.28 551733 Making sure application is connected to the radio before handling DRLC event.
        private void HandleDRLCScheduledEvent(object sender, EventArgs e)
        {
            DRLCEvent Event = sender as DRLCEvent;

            //The telegesis radio should be connected in order to handle the DRLC event
            if (Event != null && false != IsConnected)
            {
                // This event should only occur when the DRLC Event is scheduled to start and when it is scheduled to end
                if (Event.CancelPending == true)
                {
                    // We should cancel the event
                    Event.CurrentStatus = DRLCEventStatus.Cancelled;

                    if (m_ScheduledDRLCEvents.Contains(Event))
                    {
                        m_ScheduledDRLCEvents.Remove(Event);
                        m_CompletedDRLCEvents.Add(Event);
                    }
                    else if (m_RunningDRLCEvents.Values.Contains(Event))
                    {
                        RemoveRunningDRLCEvent(Event);
                    }
                    else
                    {
                        // It is already in the completed list for some reason so lets make sure it doesn't notify us again
                        Event.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;
                    }

                    SendDRLCEventStatusUpdate(Event);
                }
                else if (m_ScheduledDRLCEvents.Contains(Event))
                {
                    // This means that we should be starting the event
                    switch (Event.CurrentStatus)
                    {
                        case DRLCEventStatus.CommandReceived:
                        case DRLCEventStatus.UserOptIn:
                        {
                            StartDRLCEvent(Event);
                            break;
                        }
                        case DRLCEventStatus.UserOptOut:
                        {
                            // The user has opted out prior to the start so don't notify of the event started but mark it running
                            if (Event.IsMandatory)
                            {
                                StartDRLCEvent(Event);
                            }
                            else
                            {
                                // TODO: Check to see if the start supersedes an event
                                Event.UserOptedOutPriorToStart = true;
                                Event.ScheduleEvent(Event.Duration * 60000);

                                SendDRLCEventStatusUpdate(Event);
                            }

                            break;
                        }
                        default:
                        {
                            // This really should never happen but it's here just in case
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected DRLC Scheduled Event occurred. Event ID: "
                                + Event.IssuerEventID.ToString("X8", CultureInfo.InvariantCulture) + " Status: " + Event.CurrentStatus.ToString() + " Running: No");
                            break;
                        }
                    }
                }
                else if (m_RunningDRLCEvents.ContainsValue(Event))
                {
                    // Event is currently running so we should be at the end of the event
                    switch (Event.CurrentStatus)
                    {
                        case DRLCEventStatus.EventStarted:
                        {
                            Event.CurrentStatus = DRLCEventStatus.EventCompleted;
                            Event.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;

                            RemoveRunningDRLCEvent(Event);

                            SendDRLCEventStatusUpdate(Event);
                            break;
                        }
                        case DRLCEventStatus.UserOptOut:
                        {
                            if (Event.UserOptedOutPriorToStart)
                            {
                                Event.CurrentStatus = DRLCEventStatus.EventCompleteNoParticipation;
                            }
                            else
                            {
                                Event.CurrentStatus = DRLCEventStatus.PartialCompletionOptOut;
                            }

                            Event.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;

                            RemoveRunningDRLCEvent(Event);

                            SendDRLCEventStatusUpdate(Event);
                            break;
                        }
                        case DRLCEventStatus.UserOptIn:
                        {
                            Event.CurrentStatus = DRLCEventStatus.PartialCompletionOptIn;
                            Event.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;

                            RemoveRunningDRLCEvent(Event);

                            SendDRLCEventStatusUpdate(Event);
                            break;
                        }
                        default:
                        {
                            // This really should never happen but it's here just in case
                            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected DRLC Scheduled Event occurred. Event ID: "
                                + Event.IssuerEventID.ToString("X8", CultureInfo.InvariantCulture) + " Status: " + Event.CurrentStatus.ToString() + " Running: Yes");
                            break;
                        }
                    }
                }
                else
                {
                    // The event must have been raised from an event that has already been moved to the completed list so we should ignore it and remove the handler
                    Event.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;
                }
            }
        }

        /// <summary>
        /// Removes the specified event from the list of running events
        /// </summary>
        /// <param name="eventToRemove">The event to remove</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void RemoveRunningDRLCEvent(DRLCEvent eventToRemove)
        {
            // The event can be running under several device classes so make sure we remove them all
            while (m_RunningDRLCEvents.ContainsValue(eventToRemove))
            {
                KeyValuePair<DRLCDeviceClasses, DRLCEvent> CurrentKeyPair = m_RunningDRLCEvents.First(e => e.Value == eventToRemove);

                m_RunningDRLCEvents.Remove(CurrentKeyPair.Key);
            }

            // Since we removed it we should add it to the list of inactive events
            m_CompletedDRLCEvents.Add(eventToRemove);
        }

        /// <summary>
        /// Starts the specified DRLC Event
        /// </summary>
        /// <param name="eventToStart">The event to start</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void StartDRLCEvent(DRLCEvent eventToStart)
        {
            List<DRLCDeviceClasses> EventDeviceClasses = new List<DRLCDeviceClasses>();

            // Get the list of device classes this applies to
            foreach (DRLCDeviceClasses CurrentDeviceClass in Enum.GetValues(typeof(DRLCDeviceClasses)))
            {
                if (eventToStart.DeviceClasses.HasFlag(CurrentDeviceClass) && CurrentDeviceClass != DRLCDeviceClasses.All && CurrentDeviceClass != DRLCDeviceClasses.None)
                {
                    EventDeviceClasses.Add(CurrentDeviceClass);
                }
            }

            foreach (DRLCDeviceClasses CurrentClass in EventDeviceClasses)
            {
                // Make sure this class is supported
                if (DRLCDeviceClass.HasFlag(CurrentClass))
                {
                    // Check to see if we are superseding an already running event
                    if (m_RunningDRLCEvents.ContainsKey(CurrentClass))
                    {
                        DRLCEvent SupersededEvent = m_RunningDRLCEvents[CurrentClass];
                        m_RunningDRLCEvents.Remove(CurrentClass);

                        // It's possible that only part of the running event has been superseded so we need to check to see if the
                        // event is running on other device classes
                        if (m_RunningDRLCEvents.ContainsValue(SupersededEvent) == false)
                        {
                            // The event has been completely removed for all device classes so it needs to be marked superseded
                            SupersededEvent.CancelScheduledEvent();
                            SupersededEvent.ScheduledEventOccurred -= m_DRLCScheduledEventHandler;
                            SupersededEvent.CurrentStatus = DRLCEventStatus.Superseded;

                            m_CompletedDRLCEvents.Add(SupersededEvent);
                            SendDRLCEventStatusUpdate(SupersededEvent);
                        }
                    }

                    m_RunningDRLCEvents.Add(CurrentClass, eventToStart);
                }
            }

            if (m_RunningDRLCEvents.ContainsValue(eventToStart))
            {
                // Start the new event
                eventToStart.ScheduledEventOccurred += m_DRLCScheduledEventHandler;
                eventToStart.CurrentStatus = DRLCEventStatus.EventStarted;
                eventToStart.ScheduleEvent(eventToStart.Duration * 60000);

                SendDRLCEventStatusUpdate(eventToStart);
            }
        }

        /// <summary>
        /// Raises the ZigBee DRLC Event Received event
        /// </summary>
        /// <param name="incomingEvent">the event that set off the event</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/03/14 MP                 Created
        private void OnDRLCEventReceived(DRLCEvent incomingEvent)
        {
            if (DRLCResponseReceived != null)
            {
                DRLCResponseReceived(this, new DRLCResponseEventArgs(incomingEvent));
            }
        }

        #endregion

        #region Simple Metering

        /// <summary>
        /// Handles Simple Metering Messages
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created

        private void HandleSimpleMeteringMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);

            switch (ZclFrame.CommandID)
            {
                case (byte)SimpleMeteringServerCommands.GetProfileResponse:
                {
                    // This should only ever be sent to us when we request it so let the requesting method handle it
                    lock (m_ZCLResponseMessages)
                    {
                        m_ZCLResponseMessages.Add(receivedMessage);
                    }
                    break;
                }
                case (byte)SimpleMeteringServerCommands.RequestMirror:
                {
                    // This should only ever be sent to us when we request it so let the requesting method handle it
                    lock (m_ZCLResponseMessages)
                    {
                        m_ZCLResponseMessages.Add(receivedMessage);
                    }
                    break;
                }
                case (byte)SimpleMeteringServerCommands.RemoveMirror:
                {
                    // This should only ever be sent to us when we request it so let the requesting method handle it
                    lock (m_ZCLResponseMessages)
                    {
                        m_ZCLResponseMessages.Add(receivedMessage);
                    }
                    break;
                }
                case (byte)SimpleMeteringServerCommands.RequestFastPollModeResponse:
                {
                    // This should only ever be sent to us when we request it so let the requesting method handle it
                    lock (m_ZCLResponseMessages)
                    {
                        m_ZCLResponseMessages.Add(receivedMessage);
                    }
                    break;
                }
                default:
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unsupported Simple Metering command received. Command ID: " + ZclFrame.CommandID.ToString("X2", CultureInfo.InvariantCulture));
                    // Unknown command so we should send a default response
                    SendDefaultResponse((ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering,
                        receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, ZCLStatus.UnsupportedClusterCommand);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the Multiplier and Divisor used to calculate values for Simple Metering values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/13 MP                 Created. More or less copied from method below, but with more attributes.
        //
        public void GetSimpleMeteringFormattingAttributes(SimpleMeteringAttributes desiredAttribute)
        {
            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();
            bool blnRetry = true;

            // lets get what we want.
            Attributes.Add((ushort)desiredAttribute);

            for (int i = 3; blnRetry && i > 0; i--) 
            {
                try
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Multiplier and Divisor");

                    if (IsJoined)
                    {
                        AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering, Attributes);

                        foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                        {
                            if (CurrentAttribute.Status == ZCLStatus.Success)
                            {
                                switch (CurrentAttribute.AttributeID)
                                {
                                    case (ushort)SimpleMeteringAttributes.Multiplier:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                            {
                                                m_Multiplier = (uint)CurrentAttribute.Value;
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "HAN Multiplier: " + m_Multiplier.ToString(CultureInfo.InvariantCulture));
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Multiplier: " + CurrentAttribute.DataType.ToString());
                                                throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                            }

                                            break;
                                        }
                                    case (ushort)SimpleMeteringAttributes.Divisor:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                            {
                                                m_Divisor = (uint)CurrentAttribute.Value;
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "HAN Divisor: " + m_Divisor.ToString(CultureInfo.InvariantCulture));
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Divisor: " + CurrentAttribute.DataType.ToString());
                                                throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                            }

                                            break;
                                        }
                                    case (ushort)SimpleMeteringAttributes.UnitOfMeasure:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Enum8)
                                            {
                                                m_UnitOfMeasure = (byte)CurrentAttribute.Value;      
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Unit of Measure: " + CurrentAttribute.DataType.ToString());
                                                throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                            }

                                            break;
                                        }
                                    case (ushort)SimpleMeteringAttributes.SummationFormatting:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                            {
                                                m_SummationFormatting = (byte)CurrentAttribute.Value;
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Summation Formatting attribute: " + CurrentAttribute.DataType.ToString());
                                                throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                            }

                                            break;
                                        }
                                    case (ushort)SimpleMeteringAttributes.DemandFormatting:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                            {
                                                m_DemandFormatting = (byte)CurrentAttribute.Value;
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Demand formatting attribute: " + CurrentAttribute.DataType.ToString());
                                                throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                            }

                                            break;
                                        }
                                    case (ushort)SimpleMeteringAttributes.HistoricalConsumptionFormatting:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                            {
                                                m_HistoricalConsumptionFormatting = (byte)CurrentAttribute.Value;
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading Historical Consumption formatting attribute: " + CurrentAttribute.DataType.ToString());
                                                throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                            }

                                            break;
                                        }
                                    case (ushort)SimpleMeteringAttributes.MeteringDeviceType:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Bitmap8)
                                            {
                                                m_MeteringDeviceType = (byte)CurrentAttribute.Value;
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the metering Device Type: " + CurrentAttribute.DataType.ToString());
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

                    //if we make it through successfully then there is no need for a retry.
                    blnRetry = false;
                }
                catch (TimeoutException TOExp)
                {
                    //Throw exception if we are out of retries.
                    if (1 == i) 
                    {
                        throw TOExp;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Multiplier and Divisor used to calculate values for Simple Metering values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        //  01/21/13 jrf 2.70.59 287460 Adding in a retry to attempt to recover from a timeout exception.
        //
        private void GetSimpleMeteringMultiplierAndDivisor()
        {

            List<ZigBeeAttributeResponse> AttributeData = null;
            List<ushort> Attributes = new List<ushort>();
            bool blnRetry = true;

            // We should try to get all of the Time Attributes at once so that the times are synchronized
            Attributes.Add((ushort)SimpleMeteringAttributes.Multiplier);
            Attributes.Add((ushort)SimpleMeteringAttributes.Divisor);

            // Before reading these values we should reset them to the default values in case we can't read them for some reason
            m_Multiplier = 1;
            m_Divisor = 1;

            for (int i = 3; blnRetry && i > 0; i--)
            {
                try
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Requesting the Multiplier and Divisor");

                    if (IsJoined)
                    {
                        AttributeData = ReadAttributes(true, (ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.SimpleMetering, Attributes);

                        foreach (ZigBeeAttributeResponse CurrentAttribute in AttributeData)
                        {
                            if (CurrentAttribute.Status == ZCLStatus.Success)
                            {
                                switch (CurrentAttribute.AttributeID)
                                {
                                    case (ushort)SimpleMeteringAttributes.Multiplier:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                            {
                                                m_Multiplier = (uint)CurrentAttribute.Value;
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "HAN Multiplier: " + m_Multiplier.ToString(CultureInfo.InvariantCulture));
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Multiplier: " + CurrentAttribute.DataType.ToString());
                                                throw new InvalidOperationException("Unexpected Attribute Data type. Can't read remaining attributes");
                                            }

                                            break;
                                        }
                                    case (ushort)SimpleMeteringAttributes.Divisor:
                                        {
                                            if (CurrentAttribute.DataType == ZCLDataType.Uint24)
                                            {
                                                m_Divisor = (uint)CurrentAttribute.Value;
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "HAN Divisor: " + m_Divisor.ToString(CultureInfo.InvariantCulture));
                                            }
                                            else
                                            {
                                                // We can't continue reading because we don't know how much data to read
                                                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unexpected Data Type retrieved when reading the Divisor: " + CurrentAttribute.DataType.ToString());
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

                    //if we make it through successfully then there is no need for a retry.
                    blnRetry = false;
                }
                catch (TimeoutException TOExp)
                {
                    //Throw exception if we are out of retries.
                    if (1 == i)
                    {
                        throw TOExp;
                    }
                }
            }
        }

        /// <summary>
        /// Requests the load profile data from the specified device
        /// </summary>
        /// <param name="destination">The destination node ID to request from</param>
        /// <param name="channel">The Load Profile channel to request</param>
        /// <param name="endTime">The end time to request load profile data from</param>
        /// <param name="periods">The number of periods to request</param>
        /// <returns>The requested Load Profile data</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created

        private SmartEnergyLoadProfile GetProfileRequest(ushort destination, SimpleMeteringProfileChannel channel, DateTime endTime, byte periods)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[6];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);
            SmartEnergyLoadProfile ProfileData = null;
            TimeSpan IntervalDuration;
            TimeSpan IntervalOffset;

            IncomingMessage Response = null;
            byte[] ResponseData = null;

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.SimpleMetering);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.SimpleMetering;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the message
            MessageWriter.Write((byte)channel);
            MessageWriter.Write((uint)(endTime.ToUniversalTime() - UTC_REFERENCE_TIME).TotalSeconds);
            MessageWriter.Write(periods);

            

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)SimpleMeteringClientCommands.GetProfile;
            ZclFrame.Data = MessageData;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);

            if (Response != null)
            {
                ZCLFrame ResponseZcl = new ZCLFrame();
                ResponseZcl.FrameData = Response.MessageContents;

                ResponseData = ResponseZcl.Data;

                ProfileData = new SmartEnergyLoadProfile();

                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                ProfileData.EndTime = UTC_REFERENCE_TIME.AddSeconds(ResponseReader.ReadUInt32());
                ProfileData.Status = (SmartEnergyLoadProfileStatus)ResponseReader.ReadByte();
                ProfileData.Duration = (SmartEnergyLoadProfilePeriod)ResponseReader.ReadByte();
                ProfileData.NumberOfPeriods = ResponseReader.ReadByte();

                IntervalDuration = SmartEnergyLoadProfile.GetPeriodDuration(ProfileData.Duration);
                IntervalOffset = new TimeSpan();

                for (int Period = 0; Period < ProfileData.NumberOfPeriods; Period++)
                {
                    DateTime IntervalTime = ProfileData.EndTime - IntervalOffset;
                    uint IntervalValue = (uint)ParseDataType(ResponseReader, ZCLDataType.Uint24);

                    SmartEnergyLoadProfileInterval NewInterval = new SmartEnergyLoadProfileInterval(IntervalTime, IntervalValue);
                    ProfileData.Intervals.Add(NewInterval);

                    // Increment the offset
                    IntervalOffset += IntervalDuration;
                }
            }

            return ProfileData;
        }

        /// <summary>
        /// Requests the load profile consumption data from the specified device
        /// </summary>
        /// <param name="destination">The destination node ID to request from</param>
        /// <param name="channel">The Load Profile channel to request</param>
        /// <param name="endTime">The end time to request load profile data from</param>
        /// <param name="periods">The number of periods to request</param>
        /// <param name="ProfileData">Load Profile Consumption Data</param>
        /// <param name="periodsOfPreviousBlock">Number of periods of previous block</param>
        /// <param name="profileIntervalPeriod">Profile Interval Period</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/31/12 PGH 2.70.01        Created

        private void GetProfileConsumptionRequest(ushort destination, SimpleMeteringProfileChannel channel, DateTime endTime, byte periods, ref SmartEnergyLoadProfile ProfileData, ref byte periodsOfPreviousBlock, ref TimeSpan profileIntervalPeriod)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[6];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);
            TimeSpan IntervalDuration;
            TimeSpan IntervalOffset;

            IncomingMessage Response = null;
            byte[] ResponseData = null;

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.SimpleMetering);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.SimpleMetering;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the message
            MessageWriter.Write((byte)channel);
            MessageWriter.Write((uint)(endTime.ToUniversalTime() - UTC_REFERENCE_TIME).TotalSeconds);
            MessageWriter.Write(periods);

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)SimpleMeteringClientCommands.GetProfile;
            ZclFrame.Data = MessageData;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);

            if (Response != null)
            {
                ZCLFrame ResponseZcl = new ZCLFrame();
                ResponseZcl.FrameData = Response.MessageContents;

                ResponseData = ResponseZcl.Data;

                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                ProfileData.EndTime = UTC_REFERENCE_TIME.AddSeconds(ResponseReader.ReadUInt32());
                ProfileData.Status = (SmartEnergyLoadProfileStatus)ResponseReader.ReadByte();
                ProfileData.Duration = (SmartEnergyLoadProfilePeriod)ResponseReader.ReadByte();
                ProfileData.NumberOfPeriods = ResponseReader.ReadByte();

                IntervalDuration = SmartEnergyLoadProfile.GetPeriodDuration(ProfileData.Duration);
                IntervalOffset = new TimeSpan();

                periodsOfPreviousBlock = ProfileData.NumberOfPeriods;
                profileIntervalPeriod = IntervalDuration;

                for (int Period = 0; Period < ProfileData.NumberOfPeriods; Period++)
                {
                    DateTime IntervalTime = ProfileData.EndTime - IntervalOffset;
                    uint IntervalValue = (uint)ParseDataType(ResponseReader, ZCLDataType.Uint24);

                    SmartEnergyLoadProfileInterval NewInterval = new SmartEnergyLoadProfileInterval(IntervalTime, IntervalValue);
                    ProfileData.Intervals.Add(NewInterval);

                    // Increment the offset
                    IntervalOffset += IntervalDuration;
                }
            }
        }

        /// <summary>
        /// Requests Mirror 
        /// </summary>
        /// <param name="destination">The destination node ID to request from</param>
        /// <returns>The requested Mirror Response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/05/13 MP                 Created

        private void SendMirrorRequest(ushort destination)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[6];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);
            SmartEnergyMirrorResponse ResponseMirror = null;

            IncomingMessage Response = null;
            byte[] ResponseData = null;

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.SimpleMetering);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.SimpleMetering;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromServer;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)SimpleMeteringServerCommands.RequestMirror;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);

            if (Response != null)
            {
                ZCLFrame ResponseZcl = new ZCLFrame();
                ResponseZcl.FrameData = Response.MessageContents;

                ResponseData = ResponseZcl.Data;

                ResponseMirror = new SmartEnergyMirrorResponse();

                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                // Read in payload
                ResponseMirror.EndPointID = ResponseReader.ReadUInt16();

                OnMirrorResponseReceived((UInt16)ResponseMirror.EndPointID);              
            }
        }

        /// <summary>
        /// Requests Mirror Removal 
        /// </summary>
        /// <param name="destination">The destination node ID to request from</param>
        /// <returns>The requested Mirror Response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/18/13 MP                 Created

        private void SendRemoveMirrorRequest(ushort destination)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[6];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);
            SmartEnergyMirrorResponse MirrorResponse = new SmartEnergyMirrorResponse();
            byte[] ResponseData = null;

            IncomingMessage Response = null;

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.SimpleMetering);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.SimpleMetering;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromServer;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)SimpleMeteringServerCommands.RemoveMirror;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);

            if (Response != null)
            {
                ZCLFrame ResponseZcl = new ZCLFrame();
                ResponseZcl.FrameData = Response.MessageContents;

                ResponseData = ResponseZcl.Data;

                MirrorResponse = new SmartEnergyMirrorResponse();

                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                MirrorResponse.EndPointID = ResponseReader.ReadUInt16();

                OnMirrorRemovedResponseReceived((UInt16)MirrorResponse.EndPointID);
            }
        }

        /// <summary>
        /// Requests Fast Poll Response 
        /// </summary>
        /// <param name="destination">The destination node ID to request from</param>
        /// <param name="Duration">length of time client polls at fast poll rate</param>
        /// <param name="pollRate">rate at which the client polls during fast poll rate</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/13 MP                 Created

        private void SendFastPollRequest(ushort destination, byte pollRate, int Duration)
        {
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[5];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            IncomingMessage Response = null;
            byte[] ResponseData = null;

            //TimeSpan pollDuration = new TimeSpan(0, Duration, 0);

            // Create the message
            MessageWriter.Write(pollRate);
            MessageWriter.Write((byte)Duration);
            
            // Set up the APS Frame for the message
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.SimpleMetering);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.SimpleMetering;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)SimpleMeteringClientCommands.RequestFastPollMode;
            ZclFrame.Data = MessageData;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);

            if (Response != null)
            {
                ZCLFrame ResponseZcl = new ZCLFrame();
                ResponseZcl.FrameData = Response.MessageContents;

                ResponseData = ResponseZcl.Data;

                MemoryStream ResponseStream = new MemoryStream(ResponseData);
                BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                byte ReceivedpollRate;
                DateTime ReceivedEndTime;

                // Read in the payload
                ReceivedpollRate = ResponseReader.ReadByte();
                ReceivedEndTime = UTC_REFERENCE_TIME.AddSeconds(ResponseReader.ReadUInt32());

                // Start Handling.
                OnFastPollRequestResponseReceived(ReceivedpollRate, ReceivedEndTime);
            }           
        }

        /// <summary>
        /// Writes attribute 
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/26/13 MP                 Created

        private void SendWriteAttributeCommand(ushort destination, GeneralClusters ClusterID, ushort AttributeID, ZCLDataType DataType, byte data)
        {

            EmberApsFrame ApsFrame = new EmberApsFrame();
            ZCLFrame ZclFrame = new ZCLFrame();
            byte[] MessageData = new byte[5];
            MemoryStream MessageStream = new MemoryStream(MessageData);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            IncomingMessage Response = null;

            // Set up the APS Frame for the message
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.SimpleMetering);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort) ClusterID;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Not a great practice, but I only made it capable of writing something with this exact size
            // payload. In the future it'd probably be best to update it to work with other attributes- I made
            // it specifically for the identify time attribute in the identify cluster.
            MessageWriter.Write((ushort) AttributeID);
            MessageWriter.Write((byte) DataType);
            MessageWriter.Write((ushort) data);

            // Create the ZCL Frame
            ZclFrame.FrameType = ZCLFrameType.EntireProfile; 
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient; 
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;
            ZclFrame.CommandID = (byte)GeneralZCLCommandIDs.WriteAttributes;
            ZclFrame.Data = MessageData;

            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);

            Response = WaitForZCLResponse(ZclFrame.SequenceNumber);
        }

        /// <summary>
        /// Raises the ZigBee Mirror Response Received event
        /// </summary>
        /// <param name="EndPointID">The Endpoint Id received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/13 MP                 Created
        private void OnMirrorResponseReceived(UInt16 EndPointID)
        {
            if (MirrorResponseReceived != null)
            {
                MirrorResponseReceived(this, new MirrorResponseEventArgs(EndPointID));
            }
        }

        /// <summary>
        /// Raises the ZigBee Mirror Removed Response Received event
        /// </summary>
        /// <param name="RemovedEndPointID">The Removed Endpoint Id received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/18/13 MP                 Created
        private void OnMirrorRemovedResponseReceived(UInt16 RemovedEndPointID)
        {
            if (MirrorRemovedResponseReceived != null)
            {
                MirrorRemovedResponseReceived(this, new MirrorRemovedResponseEventArgs(RemovedEndPointID));
            }
        }

        /// <summary>
        /// Raises the ZigBee Fast Polling Request Response Received event
        /// </summary>
        /// <param name="pollEndTime">The received Poll end time</param>
        /// <param name="pollRate"> The received poll Rate</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/26/13 MP                 Created
        private void OnFastPollRequestResponseReceived(byte pollRate, DateTime pollEndTime)
        {
            if (FastPollingRequestResponseReceived != null)
            {
                FastPollingRequestResponseReceived(this, new FastPollingRequestResponseEventArgs(pollRate, pollEndTime));
            }
        }

        #endregion

        #region Messaging

        /// <summary>
        /// Requests the last message from the specified device
        /// </summary>
        /// <param name="destination">The destination node ID to request from</param>
        /// <param name="useSecurity">Boolean used to determine whether APS encryption is set or not</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/22/12 PGH 2.53.45       Created
        //  11/25/13 MP                added option of using APS encryption or not for testing

        private void GetLastMessageRequest(ushort destination, bool useSecurity)
        {
            // Get source endpoint
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            // Set up the APS Frame for the message
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.Messaging);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.Messaging;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            // Use security or not
            if (useSecurity)
            {
                ApsFrame.Options |= EmberApsOptions.Encryption;
            }

            // Create the ZCL Frame
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;

            // set the client command
            ZclFrame.CommandID = (byte)MessagingClientCommands.GetLastMessage;

            // ZclFrame.Data holds the command payload
            // No payload for the GetLastMessage command

            byte[] CommandPayload = new byte[0];
            ZclFrame.Data = CommandPayload;

            // Send Unicast Message down to the meter
            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Send display message confirmation to the specified device
        /// </summary>
        /// <param name="destination">The destination node ID</param>
        /// <param name="MessageID">Message ID</param>
        /// <param name="ConfirmationTime">Confirmation Time</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/28/12 PGH 2.53.45       Created

        private void SendMessageConfirmationRequest(ushort destination, uint MessageID, uint ConfirmationTime)
        {

            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            // Set up the APS Frame for the message
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.Messaging);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.Messaging;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the ZCL Frame
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;

            // set the client command
            ZclFrame.CommandID = (byte)MessagingClientCommands.MessageConfirmation;

            // setup command payload
            byte[] CommandPayload = new byte[8];
            MemoryStream MessageStream = new MemoryStream(CommandPayload);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            MessageWriter.Write((UInt32)MessageID);
            MessageWriter.Write((UInt32)ConfirmationTime);

            // ZclFrame.Data holds the command payload
            ZclFrame.Data = CommandPayload;

            // Send Unicast Message down to the meter
            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Handles Messaging Messages
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/24/12 PGH 2.53.45        Created

        private void HandleMessagingMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);

            switch (ZclFrame.CommandID)
            {
                case (byte)MessagingServerCommands.DisplayMessage:
                {
                    HandleMessagingDisplayMessage(receivedMessage);
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received a display message command.");
                    break;
                }
                case (byte)MessagingServerCommands.CancelMessage:
                {
                    HandleMessagingCancelMessage(receivedMessage);
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received a cancel message command.");
                    break;
                }
                default:
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unsupported Messaging command received. Command ID: " + ZclFrame.CommandID.ToString("X2", CultureInfo.InvariantCulture));
                    // Unknown command so we should send a default response
                    SendDefaultResponse((ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Messaging,
                        receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, ZCLStatus.UnsupportedClusterCommand);
                    break;
                }
            }
        }

        /// <summary>
        /// Handles Messaging Cancel Message
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/24/12 PGH 2.53.45        Created

        private void HandleMessagingCancelMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;

            uint CancelMessageID = 0;
            byte MessageControl = 0;

            string LogMessage = "";

            int pos = 0;
            int length = (int)DataReader.BaseStream.Length;

            pos += sizeof(UInt32);
            if (pos <= length)
            {
                CancelMessageID = (UInt32)DataReader.ReadUInt32();
                LogMessage += "Cancel Message ID: " + CancelMessageID.ToString();
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                MessageControl = (byte)DataReader.ReadByte();
                LogMessage += " Message Control: " + MessageControl.ToString("X2");

            }

            if (m_LastMessage != null)
            {
                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, LogMessage);

                if (CancelMessageID == m_LastMessage.MessageId)
                {
                    // Cancel Message Received
                    OnCancelMessageReceived(m_LastMessage);
                }
                else
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Cancel Message ID does not match our display message.  Nothing to cancel.");
                }
            }
            else
            {
                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "We have no display message.  Nothing to cancel.");
            }

        }

        /// <summary>
        /// Handles Messaging Display Message
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/24/12 PGH 2.53.45        Created

        private void HandleMessagingDisplayMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "*** Display Message Details: APS Options: " + receivedMessage.APSFrame.Options.ToString()
                + " Disable Response: " + ZclFrame.DisableDefaultResponse.ToString());

            // AMI HAN Message Record
            AMIHANMsgRcd MessageRcd = new AMIHANMsgRcd();

            string LogMessage = "";

            int pos = 0;
            int length = (int)DataReader.BaseStream.Length;

            pos += sizeof(UInt32);
            if (pos <= length)
            {
                MessageRcd.MessageId = (UInt32)DataReader.ReadUInt32();
                LogMessage += "Message ID: " + MessageRcd.MessageId.ToString();
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                MessageRcd.MessageControl = (byte)DataReader.ReadByte();
                LogMessage += " Message Control: " + MessageRcd.MessageControl.ToString("X2");
            }

            pos += sizeof(UInt32);
            if (pos <= length)
            {
                MessageRcd.MessageStart = UTC_REFERENCE_TIME.AddSeconds(DataReader.ReadUInt32());
                LogMessage += " Message Start: " + MessageRcd.MessageStart.ToString();
            }

            pos += sizeof(UInt16);
            if (pos <= length)
            {
                ushort period = DataReader.ReadUInt16();
                MessageRcd.Duration = TimeSpan.FromMinutes((double)period);
                LogMessage += " Message Duration: " + MessageRcd.Duration.ToString();
            }

            if (pos <= length)
            {
                ushort len = DataReader.ReadByte();

                MessageRcd.MessageLength = len;

                byte[] MsgArray = DataReader.ReadBytes(len);

                UTF8Encoding enc = new UTF8Encoding();
                MessageRcd.DisplayMessage = enc.GetString(MsgArray);

                LogMessage += " Display Message: " + MessageRcd.DisplayMessage;

                m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, LogMessage);

                // Set the last message
                m_LastMessage = MessageRcd;

                // Display Message Received
                OnDisplayMessageReceived(MessageRcd);

            }

        }

        /// <summary>
        /// Raises the ZigBee Message Cluster Cancel Message event
        /// </summary>
        /// <param name="MessageRcd">The message to cancel</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 PGH 2.60.07       Created
        private void OnCancelMessageReceived(AMIHANMsgRcd MessageRcd)
        {
            if (CancelMessageReceived != null)
            {
                CancelMessageReceived(this, new CancelMessageEventArgs(MessageRcd));
            }
        }

        /// <summary>
        /// Raises the ZigBee Message Cluster Display Message Received event
        /// </summary>
        /// <param name="MessageRcd">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 PGH 2.60.07       Created
        private void OnDisplayMessageReceived(AMIHANMsgRcd MessageRcd)
        {
            if (DisplayMessageReceived != null)
            {
                DisplayMessageReceived(this, new DisplayMessageEventArgs(MessageRcd));
            }
        }

        /// <summary>
        /// Raises the ZigBee OTA Cluster Resp Received events
        /// </summary>
        /// <param name="MessageRcd">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/15/16 Hetsh                Created

        private void OnOTARespReceived(IncomingMessage MessageRcd)
        {
            if (OTARespReceived != null)
            {
                //OTARespReceived += new OTARespEventHandler(
                OTARespReceived(this, new OTARespEventArgs(MessageRcd));
            }
        }



        #endregion

        #region Pricing

        /// <summary>
        /// Requests the current price from the meter using different kinds of security. For testing only
        /// </summary>
        /// <param name="destination">The destination node ID of the meter</param>
        /// <param name="useSecurity">boolean representing the use of APS security encryption, or not</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        //  11/22/13 MP                 added option of choosing APS or no security

        private void GetCurrentPriceRequest(ushort destination, bool useSecurity)
        {
            // Get source endpoint
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            // Set up the APS Frame for the message
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.Price);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.Price;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;

            if (useSecurity)
            {
                ApsFrame.Options |= EmberApsOptions.Encryption;
            }

            // Create the ZCL Frame
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;

            // set the client command
            ZclFrame.CommandID = (byte)PriceClientCommands.GetCurrentPrice;

            // ZclFrame.Data holds the command payload
            // setup command payload
            byte[] CommandPayload = new byte[1];
            MemoryStream MessageStream = new MemoryStream(CommandPayload);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            MessageWriter.Write((byte)m_RequestorRxOnWhenIdle);

            // ZclFrame.Data holds the command payload
            ZclFrame.Data = CommandPayload;

            // Send Unicast Message down to the meter
            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Requests the block periods from the meter
        /// </summary>
        /// <param name="destination">The destination node ID of the meter</param>
        /// <param name="UTCStartTimeInSeconds">UTC Start Time in seconds</param>
        /// <param name="numEvents">Number of Block Period commands to be sent</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/10/12 PGH 2.60.13        Created

        private void GetBlockPeriodsRequest(ushort destination, uint UTCStartTimeInSeconds, byte numEvents)
        {
            // Get source endpoint
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            // Set up the APS Frame for the message
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.Price);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.Price;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the ZCL Frame
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;

            // set the client command
            ZclFrame.CommandID = (byte)PriceClientCommands.GetBlockPeriods;

            // ZclFrame.Data holds the command payload
            // setup command payload
            byte[] CommandPayload = new byte[5];
            MemoryStream MessageStream = new MemoryStream(CommandPayload);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            MessageWriter.Write((uint)UTCStartTimeInSeconds);
            MessageWriter.Write((byte)numEvents);

            // ZclFrame.Data holds the command payload
            ZclFrame.Data = CommandPayload;

            // Send Unicast Message down to the meter
            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Requests the the scheduled prices from the meter
        /// </summary>
        /// <param name="destination">The destination node ID of the meter</param>
        /// <param name="UTCStartTimeInSeconds">UTC Start Time in seconds</param>
        /// <param name="numEvents">The maximum number of events to be sent</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/10/12 PGH 2.60.13        Created

        private void GetScheduledPricesRequest(ushort destination, uint UTCStartTimeInSeconds, byte numEvents)
        {
            // Get source endpoint
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            // Set up the APS Frame for the message
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.Price);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.Price;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the ZCL Frame
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;

            // set the client command
            ZclFrame.CommandID = (byte)PriceClientCommands.GetScheduledPrices;

            // ZclFrame.Data holds the command payload
            // setup command payload
            byte[] CommandPayload = new byte[5];
            MemoryStream MessageStream = new MemoryStream(CommandPayload);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            MessageWriter.Write((uint)UTCStartTimeInSeconds);
            MessageWriter.Write((byte)numEvents);

            // ZclFrame.Data holds the command payload
            ZclFrame.Data = CommandPayload;

            // Send Unicast Message down to the meter
            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Send Price Acknowledgement request to the meter
        /// </summary>
        /// <param name="destination">The destination node ID of the meter</param>
        /// <param name="ProviderId">Provider ID</param>
        /// <param name="IssuerEventId">Issuer Event ID</param>
        /// <param name="UTCPriceAckTimeInSeconds">UTC Price Ack Time in seconds</param>
        /// <param name="controlField">Control Field</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/10/12 PGH 2.60.13        Created

        private void SendPriceAcknowledgementRequest(ushort destination, uint ProviderId, uint IssuerEventId, uint UTCPriceAckTimeInSeconds, byte controlField)
        {
            // Get source endpoint
            ZigBeeEndpointInfo SmartEnergyEndpoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.SmartEnergy);

            // Set up the APS Frame for the message
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            ApsFrame.DestinationEndpoint = SmartEnergyEndpoint.FindMatchingClientEndpoint(destination, (ushort)SmartEnergyClusters.Price);
            ApsFrame.SourceEndpoint = SmartEnergyEndpoint.Endpoint;
            ApsFrame.ClusterID = (ushort)SmartEnergyClusters.Price;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry
                | EmberApsOptions.Encryption;

            // Create the ZCL Frame
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameType = ZCLFrameType.ClusterSpecific;
            ZclFrame.ManufacturerSpecific = false;
            ZclFrame.Direction = ZCLDirection.SentFromClient;
            ZclFrame.DisableDefaultResponse = true;
            ZclFrame.SequenceNumber = m_ZCLSequenceNumber++;

            // set the client command
            ZclFrame.CommandID = (byte)PriceClientCommands.PriceAcknowledgement;

            // ZclFrame.Data holds the command payload
            // setup command payload
            byte[] CommandPayload = new byte[13];
            MemoryStream MessageStream = new MemoryStream(CommandPayload);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            MessageWriter.Write((uint)ProviderId);
            MessageWriter.Write((uint)IssuerEventId);
            MessageWriter.Write((uint)UTCPriceAckTimeInSeconds);
            MessageWriter.Write((byte)controlField);

            // ZclFrame.Data holds the command payload
            ZclFrame.Data = CommandPayload;

            // Send Unicast Message down to the meter
            SendUnicastMessage(destination, ApsFrame, ZclFrame.FrameData);
        }

        /// <summary>
        /// Handles Price Messages
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created

        private void HandlePriceMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);

            switch (ZclFrame.CommandID)
            {
                case (byte)PriceServerCommands.PublishPrice:
                {
                    HandlePublishPriceMessage(receivedMessage);
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received a Publish Price command.");
                    break;
                }
                case (byte)PriceServerCommands.PublishBlockPeriod:
                {
                    HandlePublishBlockPeriodMessage(receivedMessage);
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received a Publish Block Period command.");
                    break;
                }
                default:
                {
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unsupported Price command received. Command ID: " + ZclFrame.CommandID.ToString("X2", CultureInfo.InvariantCulture));
                    // Unknown command so we should send a default response
                    SendDefaultResponse((ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)SmartEnergyClusters.Price,
                        receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, ZCLStatus.UnsupportedClusterCommand);
                    break;
                }
            }
        }

        /// <summary>
        /// Handles OTA Messages
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/14/16 heTsh  `           Created

        private void HandleOTAMessage(IncomingMessage receivedMessage)
        {

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Received a Response for OTA command ");

            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            
            switch (ZclFrame.CommandID)
            {
                case (byte)OTAServerCommands.ImageNotify: // Identify Query Response.
                    {
                        OTANotificationImageReceived(this, new ImageNotifyPayload(receivedMessage.MessageContents));
                        OTAFlowStatusVar = (byte)Itron.Metering.Zigbee.Enums.OTAFlowStatus.Image_available;
                        if (OTA_automate_now_flag)
                        {
                            this.GetNextImage(FieldControl_auto, Manfacture_auto, ImageType_auto, CurrFW_auto, Hw_auto, useSecurity_auto, RouteEnab_auto);
                        }
                        break;
                    }
                case (byte)OTAServerCommands.QueryNextImageResponse: // Identify Query Response.
                    {
                        QueryNextImageRespPayload Obj_Tmp = new QueryNextImageRespPayload(receivedMessage.MessageContents);
                        Image_size_auto = Obj_Tmp.Image_size;

                        Offset_auto = 0;

                        // OTAQueryNextImageRespReceived(this, new QueryNextImageRespPayload(receivedMessage.MessageContents));
                        OTAQueryNextImageRespReceived(this, Obj_Tmp);

                        if (Obj_Tmp.Status == (byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.Success && OTA_automate_now_flag)
                        {
                           
                            this.ImageBlockRequest(IEEE_auto, Manfacture_auto, ImageType_auto, CurrFW_auto, Offset_auto.ToString("X2"), MaxData_auto.ToString("X2"), DelayNumOfBytes_auto, IEEEV_auto, Delay_auto, useSecurity_auto, RouteEnab_auto);

                        }

                        break;
                    }
                case (byte)OTAServerCommands.ImageBlockResponse: // Identify Query Response.
                    {
                        QueryNextBlockRespPayload Temp_OBJ  = new QueryNextBlockRespPayload(receivedMessage.MessageContents);
                        OTAQueryNextBlockRespReceived(this, Temp_OBJ);
                        // OTAQueryNextBlockRespReceived(this, new QueryNextBlockRespPayload(receivedMessage.MessageContents));

                        //switch (Temp_OBJ.Status)
                        //{
                        //    case (byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.Success:
                        //        //m_ImageUpgradeStatus = ImageUpgradeStatus.Download_in_progress;
                        //        if (OTA_automate_now_flag)
                        //       {
                        //            Offset_auto += Temp_OBJ.Data_Size;
                        //            if (Offset_auto < Image_size_auto) // ???
                        //            {
                        //                //this.ImageBlockRequest(IEEE_auto, Manfacture_auto, ImageType_auto, CurrFW_auto, Offset_auto.ToString("X2"), MaxData_auto.ToString("X2"), DelayNumOfBytes_auto, IEEEV_auto, Delay_auto, useSecurity_auto, RouteEnab_auto);
                        //            }
                        //            else
                        //            {
                        //                //this.EndUpgradeRequest((byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.ABORT, Manfacture_auto, ImageType_auto, CurrFW_auto, useSecurity_auto, RouteEnab_auto);
                                        
                        //                m_ImageUpgradeStatus = ImageUpgradeStatus.Download_complete;
                        //            }
                        //        }
                        //        break;

                        //    case (byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.ABORT:
                        //    case (byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.NO_IMAGE_AVAILABLE:
                        //          Offset_auto = 0;
                        //          MaxData_auto = 0; 
                        //          DelayNumOfBytes_auto= 0; 
                        //          IEEEV_auto = "";
                        //          Delay_auto = "";
                        //          OTA_automate_now_flag = false;
                        //          Image_size_auto = 0;
                        //          m_ImageUpgradeStatus = ImageUpgradeStatus.Normal;
                        //     break;

                        //    case (byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.WAIT_FOR_DATA:
                        //        // Not implemented
                        //        break;
                    
                        //}


                        break; 
                    }
                case (byte)OTAServerCommands.UpgradeEndResponse: // Identify Query Response.
                    {

                       // OTANotificationImageReceived(this, new ImageNotifyPayload(receivedMessage.MessageContents));
                       OTAUpgradeEndResponseReceived(this, new UpgradeEndResponsePayload(receivedMessage.MessageContents));
                       m_ImageUpgradeStatus = ImageUpgradeStatus.Normal;
                        break; 
                    }
                case (byte)OTAServerCommands.QueryDeviceSpecificFileResponse: // Identify Query Response.
                    { break; }

                default:
                    {
                        // Not sure what to do with this message so add it to the list of unhandled messages
                        m_UnhandledMessages.Add(receivedMessage);
                        break;
                    }

            }


        }



        /// <summary>
        /// Handles Publish Price Message
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created

        private void HandlePublishPriceMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "*** Publish Price Details: APS Options: " + receivedMessage.APSFrame.Options.ToString()
                + " Disable Response: " + ZclFrame.DisableDefaultResponse.ToString());

            // Publish Price Record
            PublishPriceRcd PriceRecord = new PublishPriceRcd();

            Console.WriteLine("Published price received:");

            string LogMessage = "";

            int pos = 0;
            int length = (int)DataReader.BaseStream.Length;

            pos += sizeof(uint);
            if (pos <= length)
            {
                PriceRecord.ProviderId = (uint)DataReader.ReadUInt32();
                LogMessage += "\r\n\tProvider ID: " + PriceRecord.ProviderId.ToString() + "\r\n";
                Console.WriteLine("Provider ID: " + PriceRecord.ProviderId.ToString());
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                ushort len = DataReader.ReadByte();

                byte[] label = DataReader.ReadBytes(len);

                UTF8Encoding enc = new UTF8Encoding();
                PriceRecord.RateLabel = enc.GetString(label);

                LogMessage += "\tRate Label: " + PriceRecord.RateLabel + "\r\n";
                Console.WriteLine("Rate Label: " + PriceRecord.RateLabel);
                pos += (sizeof(byte) * len);
            }

            pos += sizeof(uint);
            if (pos <= length)
            {
                PriceRecord.IssuerEventId = (uint)DataReader.ReadUInt32();
                LogMessage += "\tIssuer Event ID: " + PriceRecord.IssuerEventId.ToString() + "\r\n";
                Console.WriteLine("Issuer Event ID: " + PriceRecord.IssuerEventId.ToString());
            }

            pos += sizeof(uint);
            if (pos <= length)
            {
                PriceRecord.CurrentTime = UTC_REFERENCE_TIME.AddSeconds(DataReader.ReadUInt32());
                LogMessage += "\tCurrent Time: " + PriceRecord.CurrentTime.ToString() + "\r\n";
                Console.WriteLine("Current Time: " + PriceRecord.CurrentTime.ToString());
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.UnitOfMeasure = DataReader.ReadByte();
                LogMessage += "\tUnit of Measure: " + PriceRecord.UnitOfMeasure.ToString("X2") + "\r\n";
                Console.WriteLine("Unit of Measure: " + PriceRecord.UnitOfMeasure.ToString("X2"));
            }

            pos += sizeof(ushort);
            if (pos <= length)
            {
                PriceRecord.Currency = (ushort)DataReader.ReadUInt16();
                LogMessage += "\tCurrency: " + PriceRecord.Currency.ToString() + "\r\n";
                Console.WriteLine("Currency: " + PriceRecord.Currency.ToString());
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.PriceTrailingDigitAndPriceTier = DataReader.ReadByte();
                LogMessage += "\tPrice Trailing Digit and Price Tier: " + PriceRecord.PriceTrailingDigitAndPriceTier.ToString("X2") + "\r\n";
                Console.WriteLine("Price Trailing Digit and Price Tier: " + PriceRecord.PriceTrailingDigitAndPriceTier.ToString("X2"));
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.NumberOfPriceTiersAndRegisterTier = DataReader.ReadByte();
                LogMessage += "\tNumber of Price Tiers and Register Tier: " + PriceRecord.NumberOfPriceTiersAndRegisterTier.ToString("X2") + "\r\n";
                Console.WriteLine("Number of Price Tiers and Register Tier: " + PriceRecord.NumberOfPriceTiersAndRegisterTier.ToString("X2"));
            }

            pos += sizeof(uint);
            if (pos <= length)
            {
                PriceRecord.StartTime = UTC_REFERENCE_TIME.AddSeconds(DataReader.ReadUInt32());
                LogMessage += "\tStart Time: " + PriceRecord.StartTime.ToString() + "\r\n";
                Console.WriteLine("Start Time: " + PriceRecord.StartTime.ToString());
            }

            pos += sizeof(ushort);
            if (pos <= length)
            {
                ushort period = (ushort)DataReader.ReadUInt16();
                PriceRecord.Duration = TimeSpan.FromMinutes((double)period);
                LogMessage += "\tDuration: " + PriceRecord.Duration.ToString() + "\r\n";
                Console.WriteLine("Duration: " + PriceRecord.Duration.ToString());
            }

            pos += sizeof(uint);
            if (pos <= length)
            {
                PriceRecord.Price = (uint)DataReader.ReadUInt32();
                LogMessage += "\tPrice: " + PriceRecord.Price.ToString() + "\r\n";
                Console.WriteLine("Price: " + PriceRecord.Price.ToString());
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.PriceRatio = DataReader.ReadByte();
                LogMessage += "\tPrice Ratio: " + PriceRecord.PriceRatio.ToString("X2") + "\r\n";
                Console.WriteLine("Price Ratio: " + PriceRecord.PriceRatio.ToString("X2"));
            }

            pos += sizeof(uint);
            if (pos <= length)
            {
                PriceRecord.GenerationPrice = (uint)DataReader.ReadUInt32();
                LogMessage += "\tGeneration Price: " + PriceRecord.GenerationPrice.ToString() + "\r\n";
                Console.WriteLine("Generation Price: " + PriceRecord.GenerationPrice.ToString());
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.GenerationPriceRatio = DataReader.ReadByte();
                LogMessage += "\tGeneration Price Ratio: " + PriceRecord.GenerationPriceRatio.ToString("X2") + "\r\n";
                Console.WriteLine("Generation Price Ratio: " + PriceRecord.GenerationPriceRatio.ToString("X2"));
            }

            pos += sizeof(uint);
            if (pos <= length)
            {
                PriceRecord.AlternateCostDelivered = (uint)DataReader.ReadUInt32();
                LogMessage += "\tAlternate Cost Delivered: " + PriceRecord.AlternateCostDelivered.ToString("X8") + "\r\n";
                Console.WriteLine("Alternate Cost Delivered: " + PriceRecord.AlternateCostDelivered.ToString("X8"));
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.AlternateCostUnit = DataReader.ReadByte();
                LogMessage += "\tAlternate Cost Unit: " + PriceRecord.AlternateCostUnit.ToString("X2") + "\r\n";
                Console.WriteLine("Alternate Cost Unit: " + PriceRecord.AlternateCostUnit.ToString("X2"));
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.AlternateCostTrailingDigit = DataReader.ReadByte();
                LogMessage += "\tAlternate Cost Trailing Digit: " + PriceRecord.AlternateCostTrailingDigit.ToString("X2") + "\r\n";
                Console.WriteLine("Alternate Cost Trailing Digit: " + PriceRecord.AlternateCostTrailingDigit.ToString("X2"));
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.NumberOfBlockThresholds = DataReader.ReadByte();
                LogMessage += "\tNumber of Block Thresholds: " + PriceRecord.NumberOfBlockThresholds.ToString("X2");
                Console.WriteLine("Number of Block Thresholds: " + PriceRecord.NumberOfBlockThresholds.ToString("X2"));
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                PriceRecord.PriceControl = DataReader.ReadByte();
                LogMessage += "\r\n\tPrice Control: " + PriceRecord.PriceControl.ToString("X2");
                Console.WriteLine("Price Control: " + PriceRecord.PriceControl.ToString("X2"));
            }

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, LogMessage);

            // Set the last price received
            m_LastPriceReceived = PriceRecord;

            // Publish Price Received
            OnPublishPriceReceived(PriceRecord);
        }

        /// <summary>
        /// Handles Publish Block Period Message
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created

        private void HandlePublishBlockPeriodMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "*** Publish Block Period Details: APS Options: " + receivedMessage.APSFrame.Options.ToString()
                + " Disable Response: " + ZclFrame.DisableDefaultResponse.ToString());

            // Publish Block Period Record
            PublishBlockPeriodRcd BlockRecord = new PublishBlockPeriodRcd();

            string LogMessage = "";

            int pos = 0;
            int length = (int)DataReader.BaseStream.Length;

            pos += sizeof(uint);
            if (pos <= length)
            {
                BlockRecord.ProviderId = (uint)DataReader.ReadUInt32();
                LogMessage += "\r\n\tProvider ID: " + BlockRecord.ProviderId.ToString() + "\r\n";
            }

            pos += sizeof(uint);
            if (pos <= length)
            {
                BlockRecord.IssuerEventId = (uint)DataReader.ReadUInt32();
                LogMessage += "\tIssuer Event ID: " + BlockRecord.IssuerEventId.ToString() + "\r\n";
            }

            pos += sizeof(uint);
            if (pos <= length)
            {
                BlockRecord.StartTime = UTC_REFERENCE_TIME.AddSeconds(DataReader.ReadUInt32());
                LogMessage += "\tStart Time: " + BlockRecord.StartTime.ToString() + "\r\n";
            }

            pos += (sizeof(byte) * BlockRecord.SizeOfDurationField);
            if (pos <= length)
            {
                byte[] duration = DataReader.ReadBytes(BlockRecord.SizeOfDurationField);

                UTF8Encoding enc = new UTF8Encoding();
                string minutes = enc.GetString(duration);
                int period = Int32.Parse(minutes);
                BlockRecord.Duration = TimeSpan.FromMinutes((double)period);
                LogMessage += "\tDuration: " + BlockRecord.Duration.ToString() + "\r\n";
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                BlockRecord.NumberOfPriceTiersAndNumberOfBlockThresholds = DataReader.ReadByte();
                LogMessage += "\tNumber of Block Thresholds: " + BlockRecord.NumberOfPriceTiersAndNumberOfBlockThresholds.ToString("X2");
            }

            pos += sizeof(byte);
            if (pos <= length)
            {
                BlockRecord.BlockPeriodControl = DataReader.ReadByte();
                LogMessage += "\r\n\tBlock Period Control: " + BlockRecord.BlockPeriodControl.ToString("X2");
            }

            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, LogMessage);

            // Publish Block Period Received
            OnPublishBlockPeriodReceived(BlockRecord);

        }

        /// <summary>
        /// Raises the ZigBee Price Cluster Publish Price Received event
        /// </summary>
        /// <param name="PriceRcd">The publish price received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created
        private void OnPublishPriceReceived(PublishPriceRcd PriceRcd)
        {
            if (PublishPriceReceived != null)
            {
                PublishPriceReceived(this, new PublishPriceEventArgs(PriceRcd));
            }
        }

        /// <summary>
        /// Raises the ZigBee Price Cluster Publish BLock Period Received event
        /// </summary>
        /// <param name="BlockPeriodRcd">The publish block period received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created
        private void OnPublishBlockPeriodReceived(PublishBlockPeriodRcd BlockPeriodRcd)
        {
            if (PublishBlockPeriodReceived != null)
            {
                PublishBlockPeriodReceived(this, new PublishBlockPeriodEventArgs(BlockPeriodRcd));
            }
        }

        #endregion Pricing

        #region Identify

        /// <summary>
        /// Handles Simple Metering Messages
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/27/13 MP                 Created

        private void HandleIdentifyMessage(IncomingMessage receivedMessage)
        {
            ZCLFrame ZclFrame = new ZCLFrame();
            ZclFrame.FrameData = receivedMessage.MessageContents;
            MemoryStream DataStream = new MemoryStream(ZclFrame.Data);
            BinaryReader DataReader = new BinaryReader(DataStream);

            switch (ZclFrame.CommandID)
            {
                case (byte) 0x00: // Identify Query Response.
                    {
                        // This should only ever be sent to us when we request it so let the requesting method handle it
                        lock (m_ZCLResponseMessages)
                        {
                            m_ZCLResponseMessages.Add(receivedMessage);
                        }

                        byte[] ResponseData = null;
                        ZCLFrame ResponseZcl = new ZCLFrame();
                        ResponseZcl.FrameData = receivedMessage.MessageContents;

                        ResponseData = ResponseZcl.Data;

                        MemoryStream ResponseStream = new MemoryStream(ResponseData);
                        BinaryReader ResponseReader = new BinaryReader(ResponseStream);

                        ushort receivedTimeout;

                        // Read in the payload
                        receivedTimeout = ResponseReader.ReadUInt16();

                        // Start Handling.
                        OnIdentifyQueryResponseReceived(receivedTimeout);

                        break;
                    }
                default:
                    {
                        m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Unsupported Identify command received. Command ID: " + ZclFrame.CommandID.ToString("X2", CultureInfo.InvariantCulture));
                        // Unknown command so we should send a default response
                        SendDefaultResponse((ushort)ZigBeeProfileIDs.SmartEnergy, (ushort)GeneralClusters.Identify,
                            receivedMessage.SenderNodeID, receivedMessage.APSFrame.SourceEndpoint, ZCLStatus.UnsupportedClusterCommand);
                        break;
                    }
            }
        }


        #endregion


/*

        #region OTA

        /// <summary>
        /// Handles Next Image Response
        /// </summary>
        /// <param name="receivedMessage">The message received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/15/16 heTsh                Created
        
        private void HandleOTAResponse(IncomingMessage receivedMessage)
        {

            OnOTARespReceived(receivedMessage);
            
        }


        #endregion

*/


        #region Member Variables

        // EZSP Event handler
        private EventHandler m_DRLCScheduledEventHandler;

        // DRLC Member Variables
        private List<DRLCEvent> m_ScheduledDRLCEvents;
        private List<DRLCEvent> m_CompletedDRLCEvents;
        private Dictionary<DRLCDeviceClasses, DRLCEvent> m_RunningDRLCEvents;
        private byte m_DRLCEnrollmentGroup;
        private DRLCDeviceClasses m_DRLCDeviceClass;
        private byte m_DRLCStartRandomizeMinutes;
        private byte m_DRLCStopRandomizeMinutes;

        // Simple Metering Variables
        private ulong m_CurrentSummationDelivered;
        private ulong m_CurrentSummationReceived;
        private ulong m_CurrentMaxDemandDelivered;
        private ulong m_CurrentMaxDemandReceived;
        private DateTime m_CurrentMaxDemandDeliveredTime;
        private DateTime m_CurrentMaxDemandReceivedTime;
        private ulong m_DFTSummation;
        private UInt16 m_DailyFreezeTime;
        private byte m_PowerFactor;
        private DateTime m_ReadingSnapShotTime;
        private byte m_DefaultUpdatePeriod;
        private byte m_FastPollUpdatePeriod;
        private ulong? m_CurrentBlockPeriodConsumptionDelivered;
        private ulong? m_PreviousBlockPeriodConsumptionDelivered;

        private byte? m_MaxNumberOfPeriodsDelivered;

        private ulong m_CurrentTier1SummationDelivered;
        private ulong m_CurrentTier1SummationReceived;
        private ulong m_CurrentTier2SummationDelivered;
        private ulong m_CurrentTier2SummationReceived;
        private ulong m_CurrentTier3SummationDelivered;
        private ulong m_CurrentTier3SummationReceived;
        private ulong m_CurrentTier4SummationDelivered;
        private ulong m_CurrentTier4SummationReceived;
        private ulong m_CurrentTier5SummationDelivered;
        private ulong m_CurrentTier5SummationReceived;

        private byte m_Status;
        private byte m_OTAFlowStatus;

        private byte m_UnitOfMeasure;
        private uint m_Multiplier;
        private uint m_Divisor;
        private byte m_SummationFormatting;
        private byte m_DemandFormatting;
        private byte m_HistoricalConsumptionFormatting;
        private byte m_MeteringDeviceType;

        private int m_InstantaneousDemand;
        private uint m_CurrentDayConsumptionDelivered;
        private uint m_CurrentDayConsumptionReceived;
        private uint m_PreviousDayConsumptionDelivered;
        private uint m_PreviousDayConsumptionReceived;
        private DateTime m_CurrentPartialProfileIntervalStartTimeDelivered;
        private DateTime m_CurrentPartialProfileIntervalStartTimeReceived;
        private uint m_CurrentPartialProfileIntervalValueDelivered;
        private uint m_CurrentPartialProfileIntervalValueReceived;

        // Price Variables
        private string m_Tier1PriceLabel;
        private string m_Tier2PriceLabel;
        private string m_Tier3PriceLabel;
        private string m_Tier4PriceLabel;
        private string m_Tier5PriceLabel;

        private DateTime? m_CurrentBillingPeriodStart;
        private uint? m_CurrentBillingPeriodDuration;

        private DateTime? m_StartOfBlockPeriod;
        private uint? m_BlockPeriodDuration;
        private uint? m_ThresholdMultiplier = null;
        private uint? m_ThresholdDivisor = null;

        private ulong? m_Block1Threshold = null;
        private ulong? m_Block2Threshold = null;
        private ulong? m_Block3Threshold = null;
        private ulong? m_Block4Threshold = null;
        private ulong? m_Block5Threshold = null;
        private ulong? m_Block6Threshold = null;
        private ulong? m_Block7Threshold = null;
        private ulong? m_Block8Threshold = null;
        private ulong? m_Block9Threshold = null;
        private ulong? m_Block10Threshold = null;
        private ulong? m_Block11Threshold = null;
        private ulong? m_Block12Threshold = null;
        private ulong? m_Block13Threshold = null;
        private ulong? m_Block14Threshold = null;
        private ulong? m_Block15Threshold = null;

        private uint? m_NoTierBlock1Price = null;
        private uint? m_NoTierBlock2Price = null;
        private uint? m_NoTierBlock3Price = null;
        private uint? m_NoTierBlock4Price = null;
        private uint? m_NoTierBlock5Price = null;
        private uint? m_NoTierBlock6Price = null;
        private uint? m_NoTierBlock7Price = null;
        private uint? m_NoTierBlock8Price = null;
        private uint? m_NoTierBlock9Price = null;
        private uint? m_NoTierBlock10Price = null;
        private uint? m_NoTierBlock11Price = null;
        private uint? m_NoTierBlock12Price = null;
        private uint? m_NoTierBlock13Price = null;
        private uint? m_NoTierBlock14Price = null;
        private uint? m_NoTierBlock15Price = null;
        private uint? m_NoTierBlock16Price = null;

        private uint? m_Tier1Block1Price = null;
        private uint? m_Tier1Block2Price = null;
        private uint? m_Tier1Block3Price = null;
        private uint? m_Tier1Block4Price = null;
        private uint? m_Tier1Block5Price = null;
        private uint? m_Tier1Block6Price = null;
        private uint? m_Tier1Block7Price = null;
        private uint? m_Tier1Block8Price = null;
        private uint? m_Tier1Block9Price = null;
        private uint? m_Tier1Block10Price = null;
        private uint? m_Tier1Block11Price = null;
        private uint? m_Tier1Block12Price = null;
        private uint? m_Tier1Block13Price = null;
        private uint? m_Tier1Block14Price = null;
        private uint? m_Tier1Block15Price = null;
        private uint? m_Tier1Block16Price = null;

        private uint? m_Tier2Block1Price = null;
        private uint? m_Tier2Block2Price = null;
        private uint? m_Tier2Block3Price = null;
        private uint? m_Tier2Block4Price = null;
        private uint? m_Tier2Block5Price = null;
        private uint? m_Tier2Block6Price = null;
        private uint? m_Tier2Block7Price = null;
        private uint? m_Tier2Block8Price = null;
        private uint? m_Tier2Block9Price = null;
        private uint? m_Tier2Block10Price = null;
        private uint? m_Tier2Block11Price = null;
        private uint? m_Tier2Block12Price = null;
        private uint? m_Tier2Block13Price = null;
        private uint? m_Tier2Block14Price = null;
        private uint? m_Tier2Block15Price = null;
        private uint? m_Tier2Block16Price = null;

        private uint? m_Tier3Block1Price = null;
        private uint? m_Tier3Block2Price = null;
        private uint? m_Tier3Block3Price = null;
        private uint? m_Tier3Block4Price = null;
        private uint? m_Tier3Block5Price = null;
        private uint? m_Tier3Block6Price = null;
        private uint? m_Tier3Block7Price = null;
        private uint? m_Tier3Block8Price = null;
        private uint? m_Tier3Block9Price = null;
        private uint? m_Tier3Block10Price = null;
        private uint? m_Tier3Block11Price = null;
        private uint? m_Tier3Block12Price = null;
        private uint? m_Tier3Block13Price = null;
        private uint? m_Tier3Block14Price = null;
        private uint? m_Tier3Block15Price = null;
        private uint? m_Tier3Block16Price = null;

        private uint? m_Tier4Block1Price = null;
        private uint? m_Tier4Block2Price = null;
        private uint? m_Tier4Block3Price = null;
        private uint? m_Tier4Block4Price = null;
        private uint? m_Tier4Block5Price = null;
        private uint? m_Tier4Block6Price = null;
        private uint? m_Tier4Block7Price = null;
        private uint? m_Tier4Block8Price = null;
        private uint? m_Tier4Block9Price = null;
        private uint? m_Tier4Block10Price = null;
        private uint? m_Tier4Block11Price = null;
        private uint? m_Tier4Block12Price = null;
        private uint? m_Tier4Block13Price = null;
        private uint? m_Tier4Block14Price = null;
        private uint? m_Tier4Block15Price = null;
        private uint? m_Tier4Block16Price = null;

        // Messaging Variables
        private AMIHANMsgRcd m_LastMessage;

        // Pricing Variables
        private PublishPriceRcd m_LastPriceReceived;
        private byte m_RequestorRxOnWhenIdle = 1;

        // OTA Attributes
        private ulong m_UpgradeServerId;
        private uint m_FileOffset;
        private uint m_CurrentFileVersion;
        private ushort m_CurrentZigBeeStackVersion;
        private uint m_DownloadFileVersion;
        private ushort m_DownloadZigBeeStackVersion;
        private ImageUpgradeStatus m_ImageUpgradeStatus;
        private ushort m_ManufacturerId;
        private ushort m_ImageTypeId;

        #endregion
    }

    /// <summary>
    /// DisplayMessageEventArgs class for use with Display Message event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  03/09/12 PGH 2.60.01        Created
    public class DisplayMessageEventArgs : EventArgs
    {
        /// <summary>
        /// DisplayMessageEventArgs Constructor
        /// </summary>
        public DisplayMessageEventArgs(AMIHANMsgRcd message)
        {
            m_Message = message;
        }

        /// <summary>
        /// DisplayMessageEventArgs Message property
        /// </summary>
        public AMIHANMsgRcd Message
        {
            get
            {
                return m_Message;
            }
        }

        private AMIHANMsgRcd m_Message;
    }


    /// <summary>
    /// OTARespEventArgs class for use with OTA response event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  03/09/12 Hetsh 2.60.01        Created
    public class OTARespEventArgs : EventArgs
    {
        /// <summary>
        /// DisplayMessageEventArgs Constructor
        /// </summary>
        public OTARespEventArgs(IncomingMessage message)
        {
            m_Message = message;
        }

        /// <summary>
        /// DisplayMessageEventArgs Message property
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
    /// AttributeResponsePayload class to decode Attribute Response
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  9/8/16   JBH                 Created
    public class AttributeResponsePayload : EventArgs
    {
        private ushort m_AttributeType = 0xFFFF;
        private object m_Attribute = null;

        #region Public Properties

        /// <summary>
        /// AttributeType
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  9/8/16 JBH        		Created
        public ushort AttributeType
        {
            get
            {
                return m_AttributeType;
            }
        } 
        
        /// <summary>
        /// Attribute
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  9/8/16 JBH        		Created
        public Object Attribute
        {
            get
            {
                return m_Attribute;
            }
        }

        #endregion

        /// <summary>
        /// AttributeResponsePayload Constructor
        /// </summary>
        public AttributeResponsePayload(ushort attributeType, Object attribute)
        {
            m_AttributeType = attributeType;
            m_Attribute = attribute;
        }
    }

    /// <summary>
    /// ImageNotifyPayload class to decode Notification message of New Image availble
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  7/22/16   Hetsh                 Created
    public class ImageNotifyPayload : EventArgs
    {
        private Byte m_PayloadType = 0xFF;
        private Byte m_QueryJitter = 0xFF;
        private UInt16 m_Manufacturer;
        private UInt16 m_Image_type;
        private UInt32 m_File_version;

        #region Public Properties

        /// <summary>
        /// PayloadType
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public Byte PayloadType
        {
            get
            {
                return m_PayloadType;
            }

        }

        /// <summary>
        /// QueryJitter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public Byte QueryJitter
        {
            get
            {
                return m_QueryJitter;
            }

        }

        /// <summary>
        /// Manufacturer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 Manufacturer
        {
            get
            {
                return m_Manufacturer;
            }

        }

        /// <summary>
        /// Image_type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 Image_type
        {
            get
            {
                return m_Image_type;
            }

        }


        /// <summary>
        /// File_version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt32 File_version
        {
            get
            {
                return m_File_version;
            }

        }

        #endregion

        /// <summary>
        /// ImageNotifyPayload Constructor
        /// </summary>
        public ImageNotifyPayload(Byte[] Respm_DataArr)
        {
            m_Manufacturer = 0xFFFF;
            m_Image_type = 0xFFFF;
            m_File_version = 0xFFFFFFFF;

            MemoryStream DataStream = new MemoryStream(Respm_DataArr);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;
            DataReader.ReadUInt16(); // not needed 16 bit
            DataReader.ReadByte(); // not needed 8 bit

            // int length = (int)DataReader.BaseStream.Length;
            m_PayloadType = (Byte)DataReader.ReadByte();
            m_QueryJitter = (Byte)DataReader.ReadByte();
            try
            {

                if (m_PayloadType > 0)
                {
                    m_Manufacturer = (UInt16)DataReader.ReadUInt16();
                }
                if (m_PayloadType > 1)
                {
                    m_Image_type = (UInt16)DataReader.ReadUInt16();
                }
                if (m_PayloadType > 2)
                {
                    m_File_version = (UInt32)DataReader.ReadUInt32();
                }
            }
            catch
            {
                Console.WriteLine("Wrong Formate Notification");
            }
        }

    }




    /// <summary>
    /// End Upgrade Payload class to decode response message
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  7/22/16   Hetsh                 Created
    public class UpgradeEndResponsePayload : EventArgs
    {

        private UInt16 m_Manufacturer;
        private UInt16 m_Image_type;
        private UInt32 m_File_version;
        private UInt32 m_Current_time;
        private UInt32 m_Upgrade_time;

        #region Public Properties


        /// <summary>
        /// Manufacturer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 Manufacturer
        {
            get
            {
                return m_Manufacturer;
            }

        }

        /// <summary>
        /// Image_type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 Image_type
        {
            get
            {
                return m_Image_type;
            }

        }


        /// <summary>
        /// File_version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt32 File_version
        {
            get
            {
                return m_File_version;
            }

        }


        /// <summary>
        /// Current_time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt32 Current_time
        {
            get
            {
                return m_Current_time;
            }

        }


        /// <summary>
        /// Upgrade_time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt32 Upgrade_time
        {
            get
            {
                return m_Upgrade_time;
            }

        }



        #endregion

        /// <summary>
        /// UpgradeEndResponsePayload Constructor
        /// </summary>
        public UpgradeEndResponsePayload(Byte[] Respm_DataArr)
        {

            MemoryStream DataStream = new MemoryStream(Respm_DataArr);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;
            DataReader.ReadUInt16(); // not needed 16 bit
            DataReader.ReadByte(); // not needed 8 bit
            try
            {
                m_Manufacturer = (UInt16)DataReader.ReadUInt16();
                m_Image_type = (UInt16)DataReader.ReadUInt16();
                m_File_version = (UInt32)DataReader.ReadUInt32();   
                m_Current_time = (UInt32)DataReader.ReadUInt32();
                m_Upgrade_time = (UInt32)DataReader.ReadUInt32();
            }
            catch   
            {
                    m_Manufacturer = 0xFFFF;
                    m_Image_type = 0xFFFF;
                    m_File_version = 0xFFFFFFFF;
                    m_Current_time = 0xFFFFFFFF;
                    m_Upgrade_time = 0xFFFFFFFF;

        }
        }

    }











    /// <summary>
    /// QueryNextImageRespPayload class to decode Next Image message
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  7/22/16   Hetsh                 Created

    public class QueryNextImageRespPayload: EventArgs
    {
        Byte m_Status = 0xFF;
        UInt16 m_Manufacturer = 0xFFFF;
        UInt16 m_Image_type = 0xFFFF;
        UInt32 m_File_version = 0xFFFFFFFF;
        UInt32 m_Image_size = 0xFFFFFFFF;

        #region Public Properties

        /// <summary>
        /// Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public Byte Status
        {
            get
            {
                return m_Status;
            }

        }

        /// <summary>
        /// Image_size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt32 Image_size
        {
            get
            {
                return m_Image_size;
            }

        }

        /// <summary>
        /// Manufacturer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 Manufacturer
        {
            get
            {
                return m_Manufacturer;
            }

        }

        /// <summary>
        /// Image_type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 Image_type
        {
            get
            {
                return m_Image_type;
            }

        }


        /// <summary>
        /// File_version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt32 File_version
        {
            get
            {
                return m_File_version;
            }

        }

        #endregion

        /// <summary>
        /// QueryNextImageRespPayload Constructor
        /// </summary>
        public QueryNextImageRespPayload(Byte[] Respm_DataArr)
        {
            MemoryStream DataStream = new MemoryStream(Respm_DataArr);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;
            DataReader.ReadUInt16(); // not needed 16 bit
            DataReader.ReadByte(); // not needed 8 bit
            // int length = (int)DataReader.BaseStream.Length;

            m_Status = (Byte)DataReader.ReadByte();
            if (m_Status == (Byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.Success)
            {
                m_Manufacturer = (UInt16)DataReader.ReadUInt16();
                m_Image_type = (UInt16)DataReader.ReadUInt16();
                m_File_version = (UInt32)DataReader.ReadUInt32();
                m_Image_size = (UInt32)DataReader.ReadUInt32();
            }
        }

    }




    /// <summary>
    /// QueryNextBlockRespPayload class to decode Next Block message and the block sent is part of it
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  7/22/16   Hetsh                 Created

    public class QueryNextBlockRespPayload : EventArgs
    {

        Byte m_Status = 0xFF;
        UInt16 m_Manufacturer = 0xFFFF;
        UInt16 m_Image_type = 0xFFFF;
        UInt32 m_File_version = 0xFFFFFFFF;
        UInt32 m_File_Offset = 0xFFFFFFFF;
        Byte m_Data_Size = 0x00;
        Byte[] m_DataArr;
        DateTime m_CurrentTime; //= Convert.ToDateTime(0);
        DateTime m_RequestTime; //= Convert.ToDateTime(0);
        UInt16 m_BlockRequestDelay = 0;

        #region Public Properties

        /// <summary>
        /// Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public Byte Status
        {
            get
            {
                return m_Status;
            }

        }



        /// <summary>
        /// Manufacturer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 Manufacturer
        {
            get
            {
                return m_Manufacturer;
            }

        }

        /// <summary>
        /// Image_type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 Image_type
        {
            get
            {
                return m_Image_type;
            }

        }


        /// <summary>
        /// File_version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt32 File_version
        {
            get
            {
                return m_File_version;
            }

        }


        /// <summary>
        /// File_Offset
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt32 File_Offset
        {
            get
            {
                return m_File_Offset;
            }

        }


        /// <summary>
        /// Data_Size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public Byte Data_Size
        {
            get
            {
                return m_Data_Size;
            }

        }


        /// <summary>
        /// DataArr contains the data sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public Byte[] DataArr
        {
            get
            {
                return m_DataArr;
            }

        }


        /// <summary>
        /// CurrentTime
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public DateTime CurrentTime
        {
            get
            {
                return m_CurrentTime;
            }

        }


        /// <summary>
        /// RequestTime
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public DateTime RequestTime
        {
            get
            {
                return m_RequestTime;
            }

        }


        /// <summary>
        /// BlockRequestDelay
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/22/16 Hetsh        		Created
        public UInt16 BlockRequestDelay
        {
            get
            {
                return m_BlockRequestDelay;
            }

        }



        #endregion


        /// <summary>
        /// QueryNextBlockRespPayload Constructor
        /// </summary>
        public QueryNextBlockRespPayload(Byte[] Respm_DataArr)
        {
            MemoryStream DataStream = new MemoryStream(Respm_DataArr);
            BinaryReader DataReader = new BinaryReader(DataStream);
            DataReader.BaseStream.Position = 0;
            DataReader.ReadUInt16(); // not needed 16 bit
            DataReader.ReadByte(); // not needed 8 bit

            m_Status = (Byte)DataReader.ReadByte();
            switch (m_Status)
            {

                case (Byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.Success:
                    {
                        m_Manufacturer = (UInt16)DataReader.ReadUInt16();
                        m_Image_type = (UInt16)DataReader.ReadUInt16();
                        m_File_version = (UInt32)DataReader.ReadUInt32();
                        m_File_Offset = (UInt32)DataReader.ReadUInt32();
                        m_Data_Size = (Byte)DataReader.ReadByte();
                        m_DataArr = DataReader.ReadBytes(m_Data_Size);
                        //m_CurrentTime = Convert.ToDateTime(0);
                        //m_RequestTime = Convert.ToDateTime(0);
                        m_BlockRequestDelay = 0;
                        break;
                    }


                case (Byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.WAIT_FOR_DATA:
                    {
                        m_CurrentTime = Convert.ToDateTime(DataReader.ReadUInt32());
                        m_RequestTime = Convert.ToDateTime(DataReader.ReadUInt32());
                        m_Image_type = (UInt16)DataReader.ReadUInt16();
                        m_File_version = (UInt32)DataReader.ReadUInt32();
                        m_File_Offset = (UInt32)DataReader.ReadUInt32();
                        m_BlockRequestDelay = (UInt16)DataReader.ReadUInt16();

                        m_Manufacturer = 0xFFFF;
                        m_Image_type = 0xFFFF;
                        m_Data_Size = 0x00;
                        break;
                    }

                case (Byte)Itron.Metering.Zigbee.Enums.OTAStatusCodes.ABORT:
                    {
                        m_Manufacturer = 0xFFFF;
                        m_Image_type = 0xFFFF;
                        m_File_version = 0xFFFFFFFF;
                        m_File_Offset = 0xFFFFFFFF;
                        m_Data_Size = 0x00;

                        m_BlockRequestDelay = 0;
                        break;
                    }

            }
        }

    }










    /// <summary>
    /// CancelMessageEventArgs class for use with Cancel Message event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  03/29/12 PGH 2.60.07        Created
    public class CancelMessageEventArgs : EventArgs
    {
        /// <summary>
        /// CancelMessageEventArgs Constructor
        /// </summary>
        public CancelMessageEventArgs(AMIHANMsgRcd message)
        {
            m_Message = message;
        }

        /// <summary>
        /// CancelMessageEventArgs Message property
        /// </summary>
        public AMIHANMsgRcd Message
        {
            get
            {
                return m_Message;
            }
        }

        private AMIHANMsgRcd m_Message;
    }

    /// <summary>
    /// PublishPriceEventArgs class for use with Publish Price event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  04/04/12 PGH 2.60.13        Created
    public class PublishPriceEventArgs : EventArgs
    {
        /// <summary>
        /// PublishPriceEventArgs Constructor
        /// </summary>
        public PublishPriceEventArgs(PublishPriceRcd PriceRcd)
        {
            m_PriceRcd = PriceRcd;
        }

        /// <summary>
        /// PublishPriceEventArgs Price Record property
        /// </summary>
        public PublishPriceRcd PriceRcd
        {
            get
            {
                return m_PriceRcd;
            }
        }

        private PublishPriceRcd m_PriceRcd;
    }

    /// <summary>
    /// PublishBlockPeriodEventArgs class for use with Publish Block Period event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  04/09/12 PGH 2.60.13        Created
    public class PublishBlockPeriodEventArgs : EventArgs
    {
        /// <summary>
        /// PublishBlockPeriodEventArgs Constructor
        /// </summary>
        public PublishBlockPeriodEventArgs(PublishBlockPeriodRcd BlockPeriodRcd)
        {
            m_BlockPeriodRcd = BlockPeriodRcd;
        }

        /// <summary>
        /// PublishBlockPeriodEventArgs Block Period Record property
        /// </summary>
        public PublishBlockPeriodRcd BlockPeriodRcd
        {
            get
            {
                return m_BlockPeriodRcd;
            }
        }

        private PublishBlockPeriodRcd m_BlockPeriodRcd;
    }

    /// <summary>
    /// MirrorResponseEventArgs class for use with Mirror Response event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  12/12/13 MP                 Created

    public class MirrorResponseEventArgs : EventArgs
    {
        /// <summary>
        /// MirrorResponseEventArgs Constructor
        /// </summary>
        public  MirrorResponseEventArgs(UInt16 EndPointID)
        {
            m_EndPointID = EndPointID;
        }

        /// <summary>
        /// MirrorResponsedEventArgs Endpoint ID property
        /// </summary>
        public UInt16 EndpointID
        {
            get
            {
                return m_EndPointID;
            }
        }

        private UInt16 m_EndPointID;
    }

    /// <summary>
    /// MirrorRemovedResponseEventArgs class for use with Mirror Removed Response event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  12/18/13 MP                 Created

    public class MirrorRemovedResponseEventArgs : EventArgs
    {
        /// <summary>
        /// MirrorRemovedResponseEventArgs Constructor
        /// </summary>
        public MirrorRemovedResponseEventArgs(UInt16 RemovedEndPointID)
        {
            m_RemovedEndPointID = RemovedEndPointID;
        }

        /// <summary>
        /// MirrorRemovedResponsedEventArgs Endpoint ID property
        /// </summary>
        public UInt16 RemovedEndpointID
        {
            get
            {
                return m_RemovedEndPointID;
            }
        }

        private UInt16 m_RemovedEndPointID;
    }

    /// <summary>
    /// FastPollingRequestResponseEventArgs class for use with Fast Polling Response event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  12/26/13 MP                 Created

    public class FastPollingRequestResponseEventArgs : EventArgs
    {

        #region Constructor

        /// <summary>
        /// FastPollingRequestResponseEventArgs Constructor
        /// </summary>
        public FastPollingRequestResponseEventArgs(byte pollRate, DateTime pollEndtime)
        {
            m_pollRate = pollRate;
            m_EndTime = pollEndtime;
        }

        #endregion

        #region Properties
        /// <summary>
        /// poll Rate property
        /// </summary>
        public byte PollRate
        {
            get
            {
                return m_pollRate;
            }
        }

        /// <summary>
        /// EndTime property
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return m_EndTime;
            }
        }

        #endregion

        #region Members

        private byte m_pollRate;
        private DateTime m_EndTime;

        #endregion

    }





    /// <summary>
    /// DRLCResponseEventArgs class for use with DRLC Response event
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  1/3/14   MP                 Created

    public class DRLCResponseEventArgs : EventArgs
    {

        #region Constructor

        /// <summary>
        /// FastPollingRequestResponseEventArgs Constructor
        /// </summary>
        public DRLCResponseEventArgs(DRLCEvent incomingEvent)
        {
            receivedEvent = incomingEvent; 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Drlc event property
        /// </summary>
        public DRLCEvent EventInfo
        {
            get
            {
                return receivedEvent;
            }
        }

        #endregion

        #region Members

        private DRLCEvent receivedEvent;

        #endregion

    }



}
