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
using Itron.Metering.Utilities;

namespace Itron.Metering.DeviceDataTypes
{
    #region Public Definitions

    /// <summary>
    /// The Device classes for the event
    /// </summary>
    [Flags]
    public enum DRLCDeviceClasses : ushort
    {
        /// <summary>No Devices</summary>
        [EnumDescription("None")]
        None = 0x0000,
        /// <summary>HVAC compressor or furnace</summary>
        [EnumDescription("HVAC Compressor or Furnace")]
        HVACOrFurnace = 0x0001,
        /// <summary>Strip or Baseboard Heaters</summary>
        [EnumDescription("Strip Heater")]
        StripOrBaseHeater = 0x0002,
        /// <summary>Water Heaters</summary>
        [EnumDescription("Water Heater")]
        WaterHeater = 0x0004,
        /// <summary>Pool Pumps, Spas, or Jacuzzis</summary>
        [EnumDescription("Pool Pump")]
        PoolPump = 0x0008,
        /// <summary>Smart Appliances</summary>
        [EnumDescription("Smart Appliances")]
        SmartAppliances = 0x0010,
        /// <summary>Irrigation Pumps</summary>
        [EnumDescription("Irrigation Pump")]
        IrrigationPump = 0x0020,
        /// <summary>Managed Commercial and Industrial Loads</summary>
        [EnumDescription("Managed C&I Loads")]
        ManagedCommercialOrIndustrial = 0x0040,
        /// <summary>Simple Residential (On/Off) Loads</summary>
        [EnumDescription("Simple Miscellaneous Loads")]
        SimpleMisc = 0x0080,
        /// <summary>Exterior Lighting</summary>
        [EnumDescription("Exterior Lighting")]
        ExteriorLighting = 0x0100,
        /// <summary>Interior Lighting</summary>
        [EnumDescription("Interior Lighting")]
        InteriorLighting = 0x0200,
        /// <summary>Electric Vehicle</summary>
        [EnumDescription("Electric Vehicle")]
        ElectricVehicle = 0x0400,
        /// <summary>Generation Systems</summary>
        [EnumDescription("Generations System")]
        GenerationSystem = 0x0800,
        /// <summary>Reserved</summary>
        [EnumDescription("Reserved 1")]
        Reserved1 = 0x1000,
        /// <summary>Reserved</summary>
        [EnumDescription("Reserved 2")]
        Reserved2 = 0x2000,
        /// <summary>Reserved</summary>
        [EnumDescription("Reserved 3")]
        Reserved3 = 0x4000,
        /// <summary>Reserved</summary>
        [EnumDescription("Reserved 4")]
        Reserved4 = 0x8000,
        /// <summary>All Device Classes</summary>
        [EnumDescription("All Devices")]
        All = 0xFFFF,
    }

    /// <summary>
    /// An indication of the importance of a particular HAN load control event.
    /// </summary>
    public enum HANCriticalityLevel : byte
    {
        /// <summary>
        /// Green
        /// </summary>
        [EnumDescription("Green")]
        Green = 1,
        /// <summary>
        /// 1
        /// </summary>
        [EnumDescription("1")]
        One = 2,
        /// <summary>
        /// 2
        /// </summary>
        [EnumDescription("2")]
        Two = 3,
        /// <summary>
        /// 3
        /// </summary>
        [EnumDescription("3")]
        Three = 4,
        /// <summary>
        /// 4
        /// </summary>
        [EnumDescription("4")]
        Four = 5,
        /// <summary>
        /// 5
        /// </summary>
        [EnumDescription("5")]
        Five = 6,
        /// <summary>
        /// Emergency
        /// </summary>
        [EnumDescription("Emergency")]
        Emergency = 7,
        /// <summary>
        /// Planned Outage
        /// </summary>
        [EnumDescription("Planned Outage")]
        PlannedOutage = 8,
        /// <summary>
        /// Service Disconnect
        /// </summary>
        [EnumDescription("Service Disconnect")]
        ServiceDisconnect = 9,
        /// <summary>
        /// Utility 10
        /// </summary>
        [EnumDescription("Utility 1")]
        Utility1 = 10,
        /// <summary>
        /// Utility 11
        /// </summary>
        [EnumDescription("Utility 2")]
        Utility2 = 11,
        /// <summary>
        /// Utility 12
        /// </summary>
        [EnumDescription("Utility 3")]
        Utility3 = 12,
        /// <summary>
        /// Utility 13
        /// </summary>
        [EnumDescription("Utility 4")]
        Utility4 = 13,
        /// <summary>
        /// Utility 14
        /// </summary>
        [EnumDescription("Utility 5")]
        Utility5 = 14,
        /// <summary>
        /// Utility 15
        /// </summary>
        [EnumDescription("Utility 6")]
        Utility6 = 15,
    }

    /// <summary>
    /// DRLC Event Control bit field
    /// </summary>
    [Flags]
    public enum DRLCEventControl : byte
    {
        /// <summary>None</summary>
        None = 0x00,
        /// <summary>Randomize Start Time</summary>
        RandomizeStartTime = 0x01,
        /// <summary>Randomize End Time</summary>
        RandomizeEndTime = 0x02,
        /// <summary>Randomize Both</summary>
        RandomizeBoth = RandomizeStartTime + RandomizeEndTime,
    }

    /// <summary>
    /// DRLC Cancel Control bit field
    /// </summary>
    [Flags]
    public enum DRLCCancelControl : byte
    {
        /// <summary>None</summary>
        None = 0x00,
        /// <summary>Active events should be cancelled using the randomization settings of the original event</summary>
        EndUsingRandomization = 0x01,
    }

    /// <summary>
    /// Event Statuses
    /// </summary>
    public enum DRLCEventStatus : byte
    {
        /// <summary>Command Received</summary>
        CommandReceived = 0x01,
        /// <summary>Event has started</summary>
        EventStarted = 0x02,
        /// <summary>Event has completed</summary>
        EventCompleted = 0x03,
        /// <summary>User has chosen to opt out</summary>
        UserOptOut = 0x04,
        /// <summary>User has chosen to opt in</summary>
        UserOptIn = 0x05,
        /// <summary>Event has been cancelled</summary>
        Cancelled = 0x06,
        /// <summary>Event has been superseded</summary>
        Superseded = 0x07,
        /// <summary>The event partially completed because the user opted out</summary>
        PartialCompletionOptOut = 0x08,
        /// <summary>The event partially completed because the user opted in after it had already started</summary>
        PartialCompletionOptIn = 0x09,
        /// <summary>Event complete but the user did not participate</summary>
        EventCompleteNoParticipation = 0x0A,
        /// <summary>The cancel was rejected</summary>
        RejectedInvalidCancel = 0xF8,
        /// <summary>The cancel was rejected due to an invalid effective time</summary>
        RejectedInvalidCancelEffectiveTime = 0xF9,
        /// <summary>The load control event was rejected because it was received after it had expired</summary>
        RejectedEventReceivedAfterExpiration = 0xFB,
        /// <summary>The cancel was rejected because an undefined event was requested</summary>
        RejectedInvalidCancelUndefinedEvent = 0xFD,
        /// <summary>The load control event was rejected</summary>
        EventRejected = 0xFE,
    }

    #endregion

    /// <summary>
    /// DRLC Event object
    /// </summary>
    public class DRLCEvent
    {
        #region Constants

        /// <summary>Value that indicates the Temperature Offset is not used</summary>
        public const byte TEMP_OFFSET_NOT_USED = 0xFF;
        /// <summary>Value that indicates the Temperature Set Point is not used</summary>
        public const short TEMP_SET_POINT_NOT_USED = unchecked((short)0x8000);
        /// <summary>Value that indicates Average the Load Adjustment is not used</summary>
        public const sbyte LOAD_ADJUST_NOT_USED = unchecked((sbyte)0x80);
        /// <summary>Value that indicates the Duty Cycle is not used</summary>
        public const byte DUTY_CYCLE_NOT_USED = 0xFF;
        /// <summary>Reference Date</summary>
        private readonly DateTime REFERENCE_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Events

        /// <summary>
        /// Event that is raised when a DRLC event is scheduled
        /// </summary>
        public event EventHandler ScheduledEventOccurred;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public DRLCEvent()
        {
            m_IssuerEventID = 0;
            m_DeviceClass = DRLCDeviceClasses.None;
            m_EnrollmentGroup = 0;
            m_StartTimeSeconds = 0;
            m_Duration = 0;
            m_CriticalityLevel = HANCriticalityLevel.One;
            m_CoolingTemperatureOffset = TEMP_OFFSET_NOT_USED;
            m_HeatingTemperatureOffset = TEMP_OFFSET_NOT_USED;
            m_CoolingTemperatureSetPoint = TEMP_SET_POINT_NOT_USED;
            m_HeatingTemperatureSetPoint = TEMP_SET_POINT_NOT_USED;
            m_AvgLoadAdjustment = LOAD_ADJUST_NOT_USED;
            m_EventControl = DRLCEventControl.None;

            m_CurrentStatus = DRLCEventStatus.CommandReceived;
            m_EventStartTimer = new Timer(new TimerCallback(HandleScheduledEvent));
            m_UserOptedOutPriorToStart = false;
            m_CancellingPending = false;
        }

        /// <summary>
        /// Constructor that should be used when creating a DRLC event object to be used for sending error responses
        /// </summary>
        /// <param name="eventID">The event ID of the event request that failed</param>
        /// <param name="deviceClass">The device class of the event request that failed</param>
        /// <param name="enrollmentGroup">The enrollment group of the event request that failed</param>
        /// <param name="failureStatus">The failure status of the event request that failed.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public DRLCEvent(uint eventID, DRLCDeviceClasses deviceClass, byte enrollmentGroup, DRLCEventStatus failureStatus)
            : this()
        {
            m_IssuerEventID = eventID;
            m_DeviceClass = deviceClass;
            m_EnrollmentGroup = enrollmentGroup;
            m_CurrentStatus = failureStatus;
        }

        /// <summary>
        /// Schedules an event to occur after the specified number of milliseconds
        /// </summary>
        /// <param name="milliseconds">The amount of time in milliseconds the event will occur</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public void ScheduleEvent(long milliseconds)
        {
            m_EventStartTimer.Change(milliseconds, Timeout.Infinite);
        }

        /// <summary>
        /// Cancels any scheduled events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public void CancelScheduledEvent()
        {
            m_EventStartTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Issuer Event ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public uint IssuerEventID
        {
            get
            {
                return m_IssuerEventID;
            }
        }

        /// <summary>
        /// Gets the device classes that this event applies to
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DRLCDeviceClasses DeviceClasses
        {
            get
            {
                return m_DeviceClass;
            }
        }

        /// <summary>
        /// Gets the Utility Enrollment Group. A 0 value means all groups
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte UtilityEnrollmentGroup
        {
            get
            {
                return m_EnrollmentGroup;
            }
        }

        /// <summary>
        /// Gets the start time in seconds since 1/1/2000
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public uint StartTimeSeconds
        {
            get
            {
                return m_StartTimeSeconds;
            }
        }

        /// <summary>
        /// Gets whether or not the event should start now
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public bool StartNow
        {
            get
            {
                return m_StartTimeSeconds == 0;
            }
        }

        /// <summary>
        /// Gets the start time of the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DateTime StartTime
        {
            get
            {
                return REFERENCE_TIME.AddSeconds(m_StartTimeSeconds);
            }
        }

        /// <summary>
        /// Gets the duration of the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort Duration
        {
            get
            {
                return m_Duration;
            }
        }

        /// <summary>
        /// Gets the criticality level
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public HANCriticalityLevel CriticalityLevel
        {
            get
            {
                return m_CriticalityLevel;
            }
        }

        /// <summary>
        /// Gets the Cooling Temperature Offset raw value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte CoolingTemperatureOffset
        {
            get
            {
                return m_CoolingTemperatureOffset;
            }
        }

        /// <summary>
        /// Gets the Cooling Temperature Offset in degrees. A null value indicates the value is not used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public double? CoolingTemperatureOffsetDegrees
        {
            get
            {
                double? Degrees = null;

                if (m_CoolingTemperatureOffset != TEMP_OFFSET_NOT_USED)
                {
                    Degrees = m_CoolingTemperatureOffset / 10.0;
                }

                return Degrees;
            }
        }

        /// <summary>
        /// Gets the Heating Temperature Offset raw value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte HeatingTemperatureOffset
        {
            get
            {
                return m_HeatingTemperatureOffset;
            }
        }

        /// <summary>
        /// Gets the Heating Temperature Offset in degrees. A null value indicates the value is not used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public double? HeatingTemperatureOffsetDegrees
        {
            get
            {
                double? Degrees = null;

                if (m_HeatingTemperatureOffset != TEMP_OFFSET_NOT_USED)
                {
                    Degrees = m_HeatingTemperatureOffset / 10.0;
                }

                return Degrees;
            }
        }

        /// <summary>
        /// Gets the Cooling Temperature Set Point raw value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public short CoolingTemperatureSetPoint
        {
            get
            {
                return m_CoolingTemperatureSetPoint;
            }
        }

        /// <summary>
        /// Gets the Cooling Temperature Set Point in degrees. A null value indicates this value is not used
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public double? CoolingTemperatureSetPointDegrees
        {
            get
            {
                double? Degrees = null;

                if (m_CoolingTemperatureSetPoint != TEMP_SET_POINT_NOT_USED)
                {
                    Degrees = m_CoolingTemperatureSetPoint / 100.0;
                }

                return Degrees;
            }
        }

        /// <summary>
        /// Gets the Heating Temperature Set Point raw value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public short HeatingTemperatureSetPoint
        {
            get
            {
                return m_HeatingTemperatureSetPoint;
            }
        }

        /// <summary>
        /// Gets the Heating Temperature Set Point in degrees. A null value indicates not used
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public double? HeatingTemperatureSetPointDegrees
        {
            get
            {
                double? Degrees = null;

                if (m_HeatingTemperatureSetPoint != TEMP_SET_POINT_NOT_USED)
                {
                    Degrees = m_HeatingTemperatureSetPoint / 100.0;
                }

                return Degrees;
            }
        }

        /// <summary>
        /// Gets the Average Load Adjustment Percentage. Valid range is -100 to 100. A value of 0x8000 indicates this value is not used 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public sbyte AverageLoadAdjustmentPercentage
        {
            get
            {
                return m_AvgLoadAdjustment;
            }
        }

        /// <summary>
        /// Gets the Duty Cycle. A value of 0xFF indicates that this value is not used
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte DutyCycle
        {
            get
            {
                return m_DutyCycle;
            }
        }

        /// <summary>
        /// Gets the Event Control bitfield
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DRLCEventControl EventControl
        {
            get
            {
                return m_EventControl;
            }
        }

        /// <summary>
        /// Gets or sets the current status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DRLCEventStatus CurrentStatus
        {
            get
            {
                return m_CurrentStatus;
            }
            set
            {
                m_CurrentStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets the raw DRLC event data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] EventData
        {
            get
            {
                byte[] Data = new byte[23];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write(m_IssuerEventID);
                DataWriter.Write((ushort)m_DeviceClass);
                DataWriter.Write(m_EnrollmentGroup);
                DataWriter.Write(m_StartTimeSeconds);
                DataWriter.Write(m_Duration);
                DataWriter.Write((byte)m_CriticalityLevel);
                DataWriter.Write(m_CoolingTemperatureOffset);
                DataWriter.Write(m_HeatingTemperatureOffset);
                DataWriter.Write(m_CoolingTemperatureSetPoint);
                DataWriter.Write(m_HeatingTemperatureSetPoint);
                DataWriter.Write(m_AvgLoadAdjustment);
                DataWriter.Write(m_DutyCycle);
                DataWriter.Write((byte)m_EventControl);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 23)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_IssuerEventID = DataReader.ReadUInt32();
                    m_DeviceClass = (DRLCDeviceClasses)DataReader.ReadInt16();
                    m_EnrollmentGroup = DataReader.ReadByte();
                    m_StartTimeSeconds = DataReader.ReadUInt32();
                    m_Duration = DataReader.ReadUInt16();
                    m_CriticalityLevel = (HANCriticalityLevel)DataReader.ReadByte();
                    m_CoolingTemperatureOffset = DataReader.ReadByte();
                    m_HeatingTemperatureOffset = DataReader.ReadByte();
                    m_CoolingTemperatureSetPoint = DataReader.ReadInt16();
                    m_HeatingTemperatureSetPoint = DataReader.ReadInt16();
                    m_AvgLoadAdjustment = DataReader.ReadSByte();
                    m_DutyCycle = DataReader.ReadByte();
                    m_EventControl = (DRLCEventControl)DataReader.ReadByte();
                }
                else
                {
                    throw new ArgumentException("Event Data can not be set to null and must be 23 bytes long.");
                }
            }
        }

        /// <summary>
        /// Gets whether or not the event is mandatory
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool IsMandatory
        {
            get
            {
                bool Mandatory = false;

                switch(CriticalityLevel)
                {
                    case HANCriticalityLevel.Emergency:
                    case HANCriticalityLevel.PlannedOutage:
                    case HANCriticalityLevel.ServiceDisconnect:
                    {
                        Mandatory = true;
                        break;
                    }
                }

                return Mandatory;
            }
        }

        /// <summary>
        /// Gets whether or not the event is currently pending cancellation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool CancelPending
        {
            get
            {
                return m_CancellingPending;
            }
            set
            {
                m_CancellingPending = value;
            }
        }

        /// <summary>
        /// Gets whether or not the user opted out of the event prior to the event starting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool UserOptedOutPriorToStart
        {
            get
            {
                return m_UserOptedOutPriorToStart;
            }
            set
            {
                m_UserOptedOutPriorToStart = value;
            }
        }

        /// <summary>
        /// Gets or sets the Node ID of the device that issued the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort ServerNodeID
        {
            get
            {
                return m_ServerNodeID;
            }
            set
            {
                m_ServerNodeID = value;
            }
        }

        /// <summary>
        /// Gets or sets the endpoint of the device that issued the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte ServerEndpoint
        {
            get
            {
                return m_ServerEndpoint;
            }
            set
            {
                m_ServerEndpoint = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Timer callback that occurs at the scheduled event time
        /// </summary>
        /// <param name="stateInfo">The state information</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        private void HandleScheduledEvent(object stateInfo)
        {
            OnScheduledEventOccurred();
        }

        /// <summary>
        /// Raises the Scheduled Event Occurred event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private void OnScheduledEventOccurred()
        {
            if (ScheduledEventOccurred != null)
            {
                ScheduledEventOccurred(this, new EventArgs());
            }
        }

        #endregion

        #region Member Variables

        private uint m_IssuerEventID;
        private DRLCDeviceClasses m_DeviceClass;
        private byte m_EnrollmentGroup;
        private uint m_StartTimeSeconds;
        private ushort m_Duration;
        private HANCriticalityLevel m_CriticalityLevel;
        private byte m_CoolingTemperatureOffset;
        private byte m_HeatingTemperatureOffset;
        private short m_CoolingTemperatureSetPoint;
        private short m_HeatingTemperatureSetPoint;
        private sbyte m_AvgLoadAdjustment;
        private byte m_DutyCycle;
        private DRLCEventControl m_EventControl;

        private DRLCEventStatus m_CurrentStatus;
        private Timer m_EventStartTimer;
        private bool m_UserOptedOutPriorToStart;
        private bool m_CancellingPending;
        private ushort m_ServerNodeID;
        private byte m_ServerEndpoint;

        #endregion
    }
}
