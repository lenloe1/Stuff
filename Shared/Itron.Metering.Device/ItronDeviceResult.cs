///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//   embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//storage or retrieval system without the permission in writing from Itron, Inc.
//
//                           Copyright © 2006 - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
	/// <summary>
	/// Enumerates the common return codes from many different meter operations
	/// </summary>
	public enum ItronDeviceResult : byte
	{		
		/// <summary>
		/// SUCCESS = 0
		/// </summary>
        [EnumDescription("Success")]
        SUCCESS  = 0, 	
		/// <summary>
		/// ERROR = 1
		/// </summary>
        [EnumDescription("Error")]
        ERROR = 1,
		/// <summary>
		/// UNSUPPORTED_OPERATION = 2
		/// </summary>
        [EnumDescription("Unsupported Operation")]
        UNSUPPORTED_OPERATION = 2,
		/// <summary>
		/// SECURITY_ERROR = 3, insufficient security clearance
		/// </summary>
        [EnumDescription("Security Error")]
        SECURITY_ERROR = 3,
	}

	
	/// <summary>
	/// Enumerates the possible return codes from a DST update operation
	/// </summary>
	public enum DSTUpdateResult : byte
	{		
		/// <summary>
		/// SUCCESS = 0
		/// </summary>
		SUCCESS  = 0, 
		/// <summary>
		/// DST dates have already been updated = 1
		/// </summary>
		SUCCESS_PREVIOUSLY_UPDATED = 1,
		/// <summary>
		/// Logging failed, but everything else succeeded = 2
		/// </summary>
		SUCCESS_NO_LOGGING = 2,
		/// <summary>
		/// Meter is not configured for DST = 4
		/// </summary>
		SUCCESS_NOT_CONFIGURED_FOR_DST = 4,			
		/// <summary>
		/// Unrecoverable, fatal error = 20
		/// </summary>
		ERROR = 20,
		/// <summary>
		/// Insufficient disc space = 22
		/// </summary>
		INSUFFICIENT_DISC_SPACE = 22,
		/// <summary>
		/// I/O Timeout = 23
		/// </summary>
		IO_TIMEOUT = 23,
		/// <summary>
		/// Clock not running = 24
		/// </summary>
		CLOCK_ERROR = 24,
		/// <summary>
		/// Protocol Error = 25
		/// </summary>
		PROTOCOL_ERROR = 25,
		/// <summary>
		/// Insufficient security clearance = 27
		/// </summary>
		INSUFFICIENT_SECURITY_ERROR = 27,
		/// <summary>
		/// DST file was not found or not valid = 29
		/// </summary>
		ERROR_DST_DATA_MISSING = 29,
		/// <summary>
		/// DST dates in the meter have expired so an update is not
		/// possible.  Meter needs to be initialized or TOU should
		/// be reconfigured = 30.
		/// </summary>
		ERROR_DST_DATES_EXPIRED = 30,
		/// <summary>
		/// ERROR_RETRY = 31, operation failed at a critical time. The meter 
		/// may be left in a bad state. Try operation again.
		/// </summary>
		ERROR_RETRY = 31,
	}


	/// <summary>
	/// Enumerates the possible return codes from a Time of Use reconfiguration
	/// request.
	/// </summary>
	public enum TOUReconfigResult : byte
	{		
		/// <summary>
		/// SUCCESS = 0
		/// </summary>
		SUCCESS  = 0, 	
		/// <summary>
		/// DST dates have already been updated = 1
		/// </summary>
		SUCCESS_PREVIOUSLY_UPDATED = 1,
		/// <summary>
		/// Logging failed, but everything else succeeded = 2
		/// </summary>
		SUCCESS_NO_LOGGING = 2,
		/// <summary>
		/// Meter is not configured for DST = 4
		/// </summary>
		SUCCESS_NOT_CONFIGURED_FOR_TOU = 4,			
		/// <summary>
		/// Unrecoverable, fatal error = 20
		/// </summary>
		SUCCESS_DST_NOT_SUPPORTED = 5,
		/// <summary>
		/// Unrecoverable, fatal error = 20
		/// </summary>
		ERROR = 20,
		/// <summary>
		/// Insufficient disc space = 22
		/// </summary>
		INSUFFICIENT_DISC_SPACE = 22,
		/// <summary>
		/// I/O Timeout = 23
		/// </summary>
		IO_TIMEOUT = 23,
		/// <summary>
		/// Clock not running = 24
		/// </summary>
		CLOCK_ERROR = 24,
		/// <summary>
		/// Protocol Error = 25
		/// </summary>
		PROTOCOL_ERROR = 25,
		/// <summary>
		/// Insufficient security clearance = 27
		/// </summary>
		INSUFFICIENT_SECURITY_ERROR = 27,
		/// <summary>
		/// DST file was not found or not valid = 29
		/// </summary>
		ERROR_DST_DATA_MISSING = 29,
		/// <summary>
		/// DST dates in the meter have expired so an update is not
		/// possible.  Meter needs to be initialized or TOU should
		/// be reconfigured = 30.
		/// </summary>
		ERROR_TOU_EXPIRED = 30,
		/// ERROR_RETRY = 31, operation failed at a critical time. The meter 
		/// may be left in a bad state. Try operation again.
		ERROR_RETRY = 31,
		/// <summary>
		/// ERROR_TOU_NOT_VALID = 34, TOU file was not found or not valid
		/// </summary>
		ERROR_TOU_NOT_VALID = 34,
		/// <summary>
		/// ERROR_SCHED_NOT_SUPPORTED = 35, TOU schedule not supported by this meter type
		/// </summary>
		ERROR_SCHED_NOT_SUPPORTED = 35,
		/// <summary>
		/// ERROR_NO_FUNCTIONS_REQUESTED = 38, no TOU or DST filename was sent
		/// </summary>
		ERROR_NO_FUNCTIONS_REQUESTED = 38,        
        /// <summary>
        /// FILE_NOT_FOUND, the specified file was not found
        /// </summary>
        FILE_NOT_FOUND,       
        /// <summary>
        /// PENDING_BUFFERS_FULL, there are not enough Pending table buffers available to perform the operation
        /// </summary>
        PENDING_BUFFERS_FULL,
        
	}

    /// <summary>
    /// Enumerates the possible return codes from a request to write pending 
	/// Time of Use tables.  Note that pending tables are only supported by OpenWay
	/// meters
    /// </summary>
    public enum WritePendingTOUResult : byte
    {
        /// <summary>
        /// SUCCESS = 0, the operation completed succesfully
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// FILE_NOT_FOUND = 1, the specified file was not found
        /// </summary>
        FILE_NOT_FOUND = 1,
        /// <summary>
        /// INVALID_EDL_FILE = 2, the specified file was not a valid EDL file
        /// </summary>
        INVALID_EDL_FILE = 2,
        /// <summary>
        /// NO_TOU_DATA = 3, the specified file does not contain TOU data
        /// </summary>
        NO_TOU_DATA = 3,
        /// <summary>
        /// NO_BILLING_SCHEDULE_DATA = 4, the specified file does not contain Billing Schedule data
        /// </summary>
        NO_BILLING_SCHEDULE_DATA = 4,
        /// <summary>
        /// NO_CALENDAR_DATA = 5, the specified file does not contain Calendar data
        /// </summary>
        NO_CALENDAR_DATA = 5,
        /// <summary>
        /// PENDING_BUFFERS_FULL = 10, there are not enough Pending table buffers available to perform the operation
        /// </summary>
        PENDING_BUFFERS_FULL = 10,
        /// <summary>
        /// PROTOCOL_ERROR = 20, a protocol error has occurred
        /// </summary>
        PROTOCOL_ERROR = 20,
        /// <summary>
        /// INSUFFICIENT_SECURITY_ERROR = 21, the user has insufficient clearance to perform this operation
        /// </summary>
        INSUFFICIENT_SECURITY_ERROR = 21,
    }
    
	/// <summary>
	/// Enumerates the possible return codes from a request to reconfigure a meter's
	/// password(s)
	/// </summary>
    public enum PasswordReconfigResult : byte
	{		
		/// <summary>
		/// SUCCESS = 0
		/// </summary>
		SUCCESS  = 0,
        /// <summary>
        /// ERROR = 1
        /// </summary>
        ERROR = 1,
		/// <summary>
		/// Protocol Error = 2
		/// </summary>
		PROTOCOL_ERROR = 2,
        /// <summary>
        /// SECURITY_ERROR = 3, insufficient security clearance
        /// </summary>
        SECURITY_ERROR = 3,
        /// <summary>
		/// I/O Timeout = 4
		/// </summary>
		IO_TIMEOUT = 4,
		/// <summary>
		/// DUPLICATE_SECURITY_ERROR = 5, Pre-Saturn Sentinels did not allow
		/// duplicate security codes.
		/// </summary>
		DUPLICATE_SECURITY_ERROR = 5,
	}

	
	/// <summary>
	/// Enumerates the possible return codes from a request to adjust the clock on a meter.
	/// </summary>
	public enum ClockAdjustResult : byte
	{		
		/// <summary>
		/// SUCCESS = 0
		/// </summary>
		SUCCESS  = 0, 	
		/// <summary>
		/// SUCCESS_24_HOUR_MAXIMUM_ADJUST = 1, success but clock was only adjusted 24 hours.
		/// This is a limitation of the ANSI meters because of the potential for timeouts and 
		/// thus should only be return by them.
		/// </summary>
		SUCCESS_24_HOUR_MAXIMUM_ADJUST = 1,
		/// <summary>
		/// ERROR = 2
		/// </summary>
		ERROR = 2,
		/// <summary>
		/// UNSUPPORTED_OPERATION = 3
		/// </summary>
		UNSUPPORTED_OPERATION = 3,
		/// <summary>
		/// SECURITY_ERROR = 4, insufficient security clearance
		/// </summary>
		SECURITY_ERROR = 4,
		/// <summary>
		/// ERROR_CROSSES_INTERVAL = 5, clock adjust crosses the interval boundary
		/// </summary>
		ERROR_CROSSES_INTERVAL = 5,
		/// <summary>
		/// ERROR_CLOCK_NOT_RUNNING = 6, clock is not running in the meter
		/// </summary>
		ERROR_CLOCK_NOT_RUNNING = 6,
        /// <summary>
        /// ERROR_NO_ADJUST_OVER_DST = 7, can not adjust clock over dst change
        /// </summary>
        ERROR_NO_ADJUST_OVER_DST = 7,
        /// <summary>
        /// ERROR_TIMEOUT = 8, meter timed out, clock adjust result unknown
        /// </summary>
        ERROR_TIMEOUT = 8,
	}

	
	/// <summary>
	/// Enumerates the possible return codes from a request to reconfigure
	/// a meter's custom schedule.  Note that custom schedules are currently
	/// only supported on ANSI meters.
	/// </summary>
	public enum CSReconfigResult : byte
	{		
		/// <summary>
		/// SUCCESS = 0
		/// </summary>
		SUCCESS  = 0, 	
		/// <summary>
		/// ERROR = 1
		/// </summary>
		ERROR = 1,
		/// <summary>
		/// SUCCESS_SCHEDULE_TRUNCATED = 2, success but the custom schedule was truncated
		/// </summary>
		SUCCESS_SCHEDULE_TRUNCATED = 2,
		/// <summary>
		/// SECURITY_ERROR = 3, insufficient security clearance
		/// </summary>
		SECURITY_ERROR = 3,
		/// <summary>
		/// ERROR_CS_FILE_NOT_FOUND = 4, CS file was not found or not valid
		/// </summary>
		ERROR_CS_FILE_NOT_FOUND = 4,
		/// <summary>
		/// ERROR_MCS_ENALBED = 5, cannot reconfigure CS when MCS is enabled
		/// </summary>
		ERROR_MCS_ENALBED = 5,
	}

	/// <summary>
	/// Firmware download results enumeration
	/// </summary>
	public enum FWDownloadResult : byte
	{		
		/// <summary>
		/// SUCCESS = 0
		/// </summary>
		SUCCESS  = 0, 	
		/// <summary>
		/// WRITE_ERROR = 1
		/// </summary>
		WRITE_ERROR = 1,
		/// <summary>
		/// UNKNOWN_DRIVER_ERROR = 2
		/// </summary>
		UNKNOWN_DRIVER_ERROR = 2,
		/// <summary>
		/// INVALID_CONFIG = 3
		/// </summary>
		INVALID_CONFIG = 3,
		/// <summary>
		/// UNSUPPORTED_OPERATION = 4
		/// </summary>
		UNSUPPORTED_OPERATION = 4,
		/// <summary>
		/// SECURITY_ERROR = 5, insufficient security clearance
		/// </summary>
		SECURITY_ERROR = 5,
        /// <summary>
        /// FW_IMAGE_TOO_BIG = 6 - The Firmware Image is too big to fit into flash
        /// </summary>
        FW_IMAGE_TOO_BIG = 6,
        /// <summary>
        /// HW_REVISION_OUTSIDE_RANGE = 7 - HW Revision is outside permitted register hardware revision
        /// </summary>
        HW_REVISION_OUTSIDE_RANGE = 7,
        /// <summary>
        /// HW_VERSION_OUTSIDE_RANGE = 8 - HW Version is outside permitted register hardware version
        /// </summary>
        HW_VERSION_OUTSIDE_RANGE = 8,
        /// <summary>
        /// FW_TYPE_IS_INVALID = 9 - The Firmware Type parameter is invalid
        /// </summary>
        FW_TYPE_IS_INVALID = 9,
        /// <summary>
        /// ZIGBEE_FW_TYPE_INVALID = 10 - Firmware Version does not match the Zigbee device supported.
        /// </summary>
        ZIGBEE_FW_TYPE_INVALID = 10,
        /// <summary>
        /// DEVICE_BUSY = 11 - The initiate failed due to a timing constraint.  The meter was busy performing other operations.
        /// </summary>
        DEVICE_BUSY = 11,
        /// <summary>
        /// ICS_SAME_VERSION_REJECTION = 12 - The initiate failed because ICS firmware does not allow the same version to be downloaded.
        /// </summary>
        ICS_SAME_VERSION_REJECTION = 12,
	}

    /// <summary>
    /// Enumerates the possible return codes from a request to initialize a meter
    /// </summary>
    public enum ConfigurationResult : byte
    {
        /// <summary>
        /// SUCCES = 0
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// ERROR = 1
        /// </summary>
        ERROR = 1,
        /// <summary>
        /// PROGRAM_NOT_FOUND = 2, program name or file was not found
        /// </summary>
        PROGRAM_NOT_FOUND = 2,
        /// <summary>
        /// PROGRAM_NOT_VALID = 3, the specified program is not valid
        /// </summary>
        PROGRAM_NOT_VALID = 3,
        /// <summary>
        /// USER_ABORT = 4, the user has aborted configuration
        /// </summary>
        USER_ABORT = 4, 
        /// <summary>
        /// IO_TIMEOUT = 5, the device has timed out 
        /// </summary>
        IO_TIMEOUT = 5,
        /// <summary>
        /// NETWORK_ERROR = 6, a networking error has occurred
        /// </summary>
        NETWORK_ERROR = 6,
        /// <summary>
        /// PROTOCOL_ERROR = 7, a protocol error has occurred
        /// </summary>
        PROTOCOL_ERROR = 7,
        /// <summary>
        /// SECURITY_ERROR = 8, a security error has occurred
        /// </summary>
        SECURITY_ERROR = 8,
        /// <summary>
        /// MEMORY_ERROR = 9, there was a problem either allocating or freeing memory
        /// </summary>
        MEMORY_ERROR = 9,
        /// <summary>
        /// OFFLINE = 10, there is no communication with the device
        /// </summary>
        OFFLINE = 10,
        /// <summary>
        /// IO_ERROR = 11, an I/O error has occcured
        /// </summary>
        IO_ERROR = 11,
        /// <summary>
        /// UNKNOWN_DRIVER_ERROR = 12, this error occurs when driver errors cannot be 
        /// mapped to defined errors
        /// </summary>
        UNKNOWN_DRIVER_ERROR = 12,
        /// <summary>
        /// UNSUPPORTED_FUNCTION = 13, the function is not supported
        /// </summary>
        UNSUPPORTED_FUNCTION = 13,
        /// <summary>
        /// MISMATCH_ID = 14, the supplied device ID does not match the device ID in the meter
        /// </summary>
        MISMATCH_ID = 14,
        /// <summary>
        /// DB_ACCESS_ERROR = 15, there was a problem accessing the database
        /// </summary>
        DB_ACCESS_ERROR = 15,
        /// <summary>
        /// INVALID_CONFIG = 16, the device program is invalid
        /// </summary>
        INVALID_CONFIG = 16,
        /// <summary>
        /// MEMORY_MAP_ERROR = 17, there was a memory map error
        /// </summary>
        MEMORY_MAP_ERROR = 17,
    }

    /// <summary>
    /// Enumerates the possible return codes from a request to create a meter image file (MIF). 
    /// </summary>
    public enum MIFCreationResult 
    {
        /// <summary>
        /// SUCCES = 0
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// ERROR = 1
        /// </summary>
        ERROR = 1,
        /// <summary>
        /// PROGRAM_NOT_VALID = 3, the specified program is not valid
        /// </summary>
        IO_TIMEOUT = 5,
        /// <summary>
        /// NETWORK_ERROR = 6, a networking error has occurred
        /// </summary>
        NETWORK_ERROR = 6,
        /// <summary>
        /// SECURITY_ERROR = 8, a security error has occurred
        /// </summary>
        SECURITY_ERROR = 8,
        /// <summary>
        /// MEMORY_ERROR = 9, there was a problem either allocating or freeing memory
        /// </summary>
        MEMORY_ERROR = 9,
        /// <summary>
        /// OFFLINE = 10, there is no communication with the device
        /// </summary>
        OFFLINE = 10,
        /// <summary>
        /// IO_ERROR = 11, an I/O error has occcured
        /// </summary>
        IO_ERROR = 11,
        /// <summary>
        /// UNSUPPORTED_FUNCTION = 13, the function is not supported
        /// </summary>
        UNSUPPORTED_FUNCTION = 13,
        /// <summary>
        /// MEMORY_MAP_ERROR = 17, there was a memory map error
        /// </summary>
        MEMORY_MAP_ERROR = 17,
        /// <summary>
        /// CANNOT_OPEN_MIF = 18, there was an error opening the MIF
        /// </summary>
        CANNOT_OPEN_MIF = 18,
        /// <summary>
        /// CANNOT_READ_MIF = 19, there was an error reading the MIF
        /// </summary>
        CANNOT_READ_MIF = 19
    }

    /// <summary>
	/// Enumerates the possible return codes from a request to create an EDL file from
	/// a meter.  Note that EDL files are only supported on OpenWay meters and are
	/// roughly equivalent to HHF files.
	/// </summary>
    public enum CreateEDLResult : byte
    {
        /// <summary>
        /// SUCCES = 0, Operation succeeded
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// INVALID_PATH = 1, The file path does not exist
        /// </summary>
        INVALID_PATH = 1,
        /// <summary>
        /// INSUFFICIENT_DISC_SPACE = 2, There is not enough disc space to complete the operation
        /// </summary>
        INSUFFICIENT_DISC_SPACE = 2,
        /// <summary>
        /// PROTOCOL_ERROR = 3, An error occurred while reading the data
        /// </summary>
        PROTOCOL_ERROR = 3,
        /// <summary>
        /// SECURITY_ERROR = 4, There is insufficient security to perform the operation
        /// </summary>
        SECURITY_ERROR = 4,
        /// <summary>
        /// ERROR = 5, A General error has occurred.
        /// </summary>
        ERROR = 5,
    }

    /// <summary>
    /// Result enumeration for HAN client commands
    /// </summary>
    public enum CommandErrorResult : byte
    {
        /// <summary>
        /// Command not recognized
        /// </summary>
        UNKNOWN_COMMAND  = 0xFE,
        /// <summary>
        /// Invalid parameter size or count
        /// </summary>
        INVALID_PARAM_SIZE_OR_COUNT = 0xFD,
        /// <summary>
        /// Invalid parameter value
        /// </summary>
        INVALID_PARAM_VALUE = 0xFC,
        /// <summary>
        /// Invalid timestamp
        /// </summary>
        INVALID_TIMESTAMP = 0xFB,
        /// <summary>
        /// Invalid security key change sequence
        /// </summary>
        INVALID_SECURITY_KEY_CHANGE_SEQ = 0xFA,
        /// <summary>
        /// Invalid packet version
        /// </summary>
        INVALID_PACKET_VERSION = 0xF9,
        /// <summary>
        /// Unknown error
        /// </summary>
        UNKNOWN_ERROR = 0xF8,
    }

    /// <summary>
    /// Result enumeration
    /// </summary>
    public enum ClientMeterCmdResult : byte
    {
        /// <summary>
        /// Operation succeeded
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// Client's config command area is already full
        /// </summary>
        TOO_MANY_COMMANDS = 1,
        /// <summary>
        /// Meter timed out
        /// </summary>
        TIMEOUT = 2,
        /// <summary>
        /// Support for this command is not yet available
        /// </summary>
        UNSUPPORTED_COMMAND = 3,
        /// <summary>
        /// Unspecified error
        /// </summary>
        ERROR = 4,
    }

    /// <summary>
    /// Enumerates the possible return codes from a request to force a time sync on a meter.
    /// </summary>
    public enum ForceTimeSyncResult : byte
    {
        /// <summary>
        /// The operation succeeded.
        /// </summary>
        [EnumDescription("Success")]
        SUCCESS = 0,
        /// <summary>
        /// An unspecified error occurred.
        /// </summary>
        [EnumDescription("Unspecified Error")]
        ERROR  = 1,
        /// <summary>
        /// Insufficient security clearance.
        /// </summary>
        [EnumDescription("Security Error")]
        SECURITY_ERROR = 2,
        /// <summary>
        /// The amount of time the meter was out of sync was less than 
        /// the supplied hysteresis.
        /// </summary>
        [EnumDescription("Out of Sync Less Than Hysteresis")]
        OUT_OF_SYNC_LESS_THAN_HYSTERESIS = 3,
        /// <summary>
        /// Time synchronization request is in progress.
        /// </summary>
        [EnumDescription("Time Sync in Progress")]
        TIME_SYNC_IN_PROGRESS = 4,
        /// <summary>
        /// Invalid parameters were used.
        /// </summary>
        [EnumDescription("Invalid Parameters")]
        INVALID_PARAMETERS = 5,
        /// <summary>
        /// The device was busy and could not process the time sync.
        /// </summary>
        [EnumDescription("Device Busy")]
        DEVICE_BUSY = 6,
        /// <summary>
        /// The time sync was not supported.
        /// </summary>
        [EnumDescription("Unsupported Operation")]
        UNSUPPORTED_OPERATION = 7,
    }

    /// <summary>
    /// Results for SiteScan Reconfigure.
    /// </summary>
    public enum SiteScanReconfigResult : byte
    {
        /// <summary>
        /// Reconfigure was successful 
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// Reconfigure failed due to a general error.
        /// </summary>
        ERROR = 1,
        /// <summary>
        /// Reconfigure failed due to a security error.
        /// </summary>
        SECURITY_ERROR = 2,
        /// <summary>
        /// Reconfigure failed due to a protocol error.
        /// </summary>
        PROTOCOL_ERROR = 3,
    }

    /// <summary>
    /// Enumerates the return codes from toggling radio (RF/HAN) communications to be enabled or disabled.
    /// </summary>
    public enum ToggleRadioCommsResult : byte
    {
        /// <summary>
        /// SUCCESS = 0
        /// </summary>
        [EnumDescription("Success")]
        SUCCESS = 0,
        /// <summary>
        /// ERROR = 1
        /// </summary>
        [EnumDescription("Error")]
        ERROR = 1,        
        /// <summary>
        /// SECURITY_ERROR = 3, insufficient security clearance
        /// </summary>
        [EnumDescription("Security Error")]
        SECURITY_ERROR = 2,        
        /// <summary>
        /// Reconfigure failed due to error while reconfiguring RF radio.
        /// </summary>
        [EnumDescription("RF Radio Configuration Error")]
        RF_RADIO_CONFIG_ERROR = 3,
        /// <summary>
        /// Reconfigure failed due to error while reconfiguring HAN radio.
        /// </summary>
        [EnumDescription("HAN Radio Configuration Error")]
        HAN_RADIO_CONFIG_ERROR = 4,
        /// <summary>
        /// Reconfigure failed to update the config tag.
        /// </summary>
        [EnumDescription("Config Tag Write Error")]
        CONFIG_TAG_WRITE_ERROR = 5,
    }
}