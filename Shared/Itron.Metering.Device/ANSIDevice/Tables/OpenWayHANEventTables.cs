///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
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
//                           Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{

    /// <summary>
    /// Enumeration of the rate tiers
    /// </summary>
    public enum TierLabel : byte
    {
        /// <summary>
        /// No tier
        /// </summary>
        [EnumDescription("No Tier")]
        RATE_NONE = 0x00,
        /// <summary>
        /// Rate A
        /// </summary>
        [EnumDescription("Rate A")]
        RATE_A = 0x01,
        /// <summary>
        /// Rate B
        /// </summary>
        [EnumDescription("Rate B")]
        RATE_B = 0x02,
        /// <summary>
        /// Rate C
        /// </summary>
        [EnumDescription("Rate C")]
        RATE_C = 0x03,
        /// <summary>
        /// Rate D
        /// </summary>
        [EnumDescription("Rate D")]
        RATE_D = 0x04,
        /// <summary>
        /// Rate E
        /// </summary>
        [EnumDescription("Rate E")]
        RATE_E = 0x05,
        /// <summary>
        /// Rate F
        /// </summary>
        [EnumDescription("Rate F")]
        RATE_F = 0x06,
        /// <summary>
        /// Rate G
        /// </summary>
        [EnumDescription("Rate G")]
        RATE_G = 0x07,
    }

    /// <summary>
    /// Upstream HAN Log Event
    /// </summary>
    public class UpstreamHANLogEvent : IEquatable<UpstreamHANLogEvent>
    {
        #region Definitions

        /// <summary>
        /// The event number that identifies a particular upstream HAN log event.
        /// </summary>
        /// <remarks>
        /// If you add a new value here please add a new value to the
        /// UpstreamHANEvents enum of the EndpointServerMeterEvents class
        /// in the ServiceResponseData.cs file of the
        /// Itron.Metering.MeterServer project and also update the
        /// CENTRON_AMI_UpstreamHANEventDictionary.
        /// </remarks>
        public enum UpstreamHANLogEventID : ushort
        {
            /// <summary>
            /// Message Confirmation
            /// </summary>
            MessageConfirmation = 2432,
            /// <summary>
            /// Price Acknowledgement
            /// </summary>
            PriceAcknowledgement = 2433,
            /// <summary>
            /// Load Control Opt Out
            /// </summary>
            LoadControlOptOut = 2434,
            /// <summary>
            /// Load Control Status
            /// </summary>
            LoadControlStatus = 2435,
            /// <summary>
            /// Device State Change
            /// </summary>
            DeviceStateChange = 2436,
            /// <summary>
            /// Device Added
            /// </summary>
            DeviceAdded = 2437,
            /// <summary>
            /// Device Dropped
            /// </summary>
            DeviceDropped = 2438,
            /// <summary>
            /// Device Not Heard
            /// </summary>
            DeviceNotHeard = 2439,
            /// <summary>
            /// Device Pinged
            /// </summary>
            DevicePinged = 2440,
            /// <summary>
            /// Message Delivery Failed
            /// </summary>
            MessageDeliveryFailed = 2441,
            /// <summary>
            /// Price Delivery Failed
            /// </summary>
            PriceDeliveryFailed = 2442,
            /// <summary>
            /// DRLC Delivery Failed
            /// </summary>
            DRLCDeliveryFailed = 2443,
            /// <summary>
            /// Submetering Bubble-up Data Missing
            /// </summary>
            SubmeteringBubbleUpDataMissing = 2444,
            /// <summary>
            /// Device UEG Changed
            /// </summary>
            DeviceUEGChanged = 2445,
            /// <summary>
            /// Device Joined
            /// </summary>
            DeviceJoined = 2446,
            /// <summary>
            /// Join Flag Disabled with Unjoined Device
            /// </summary>
            JoinedFlagDisabledWithUnjoinedDevice = 2447,
            /// <summary>
            /// Event Cache Overflowed
            /// </summary>
            EventCacheOverflow = 2559,
        }
        
        /// <summary>
        /// The status associated with the load control status event.
        /// </summary>
        public enum LoadControlEventStatus : byte
        {
            /// <summary>
            /// Load Control Event Received
            /// </summary>
            [EnumDescription("Load Control Event Received")]
            LoadControlEventReceived = 1,
            /// <summary>
            /// Event Started
            /// </summary>
            [EnumDescription("Event Started")]
            EventStarted = 2,
            /// <summary>
            /// Event Completed
            /// </summary>
            [EnumDescription("Event Complete")]
            EventCompleted = 3,
            /// <summary>
            /// User Opt-Out
            /// </summary>
            [EnumDescription("User Opt-Out")]
            UserOptedOut = 4,
            /// <summary>
            /// User Opt-In
            /// </summary>
            [EnumDescription("User Opt-In")]
            UserOptedIn = 5,
            /// <summary>
            /// Event Cancelled
            /// </summary>
            [EnumDescription("Event Cancelled")]
            EventCancelled = 6,
            /// <summary>
            /// Event Superseded
            /// </summary>
            [EnumDescription("Event Superseded")]
            EventSuperseded = 7,
            /// <summary>
            /// Partially Complete - User Opt-Out
            /// </summary>
            [EnumDescription("Partially Complete - User Opt-Out")]
            PartiallyCompletedOptOut = 8,
            /// <summary>
            /// Partially Complete - User Opt-In
            /// </summary>
            [EnumDescription("Partially Complete - User Opt-In")]
            PartiallyCompletedOptIn = 9,
            /// <summary>
            /// Event Complete - No User Participation
            /// </summary>
            [EnumDescription("Event Complete - No User Participation")]
            EventCompletedNoUser = 10,
            /// <summary>
            /// Rejected - Invalid Cancel Command
            /// </summary>
            [EnumDescription("Rejected - Invalid Cancel Command")]
            RejectedInvalidCancelCommand = 248,
            /// <summary>
            /// Rejected - Invalid Effective Time
            /// </summary>
            [EnumDescription("Rejected - Invalid Effective Time")]
            RejectedInvalidEffectiveTime = 249,
            /// <summary>
            /// Rejected - Event Expired
            /// </summary>
            [EnumDescription("Rejected - Event Expired")]
            RejectedEventExpired = 251,
            /// <summary>
            /// Rejected - Undefined Event
            /// </summary>
            [EnumDescription("Rejected - Undefined Event")]
            RejectedUndefinedEvent = 253,
            /// <summary>
            /// Load Control Event Rejected
            /// </summary>
            [EnumDescription("Load Control Event Rejected")]
            LoadControlEventRejected = 253,
        }

        /// <summary>
        /// The result of pinging the device.
        /// </summary>
        public enum PingResult : byte
        {
            /// <summary>
            /// Success
            /// </summary>
            [EnumDescription("Success")]
            Success = 0,
            /// <summary>
            /// Failed - Timeout
            /// </summary>
            [EnumDescription("Failed - Timeout")]
            Timeout = 1,
            /// <summary>
            /// Failed - Invalid Status in Response
            /// </summary>
            [EnumDescription("Failed - Invalid Status in Response")]
            InvalidStatus = 2,
            /// <summary>
            /// Failed - Invalid MAC Address
            /// </summary>
            [EnumDescription("Failed - Invalid MAC Address")]
            InvalidMACAddress = 3,
            /// <summary>
            /// Failed - Device Not Registered
            /// </summary>
            [EnumDescription("Failed - Device Not Registered")]
            DeviceNotRegistered = 4,
            /// <summary>
            /// Failed - IPP Device
            /// </summary>
            [EnumDescription("Failed - IPP Device")]
            IPPDevice = 5,
            /// <summary>
            /// In Progress
            /// </summary>
            [EnumDescription("In Progress")]
            InProgress = 6,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the Upstream HAN event object based on the event ID
        /// </summary>
        /// <param name="eventID">The event ID for the event to create</param>
        /// <returns>The new Upstream HAN event object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        //  08/07/13 AF  2.85.13 WR419080 Replaced EUG with UEG
        //
        public static UpstreamHANLogEvent Create(ushort eventID)
        {
            UpstreamHANLogEvent NewEvent = null;

            switch (eventID)
            {
                case (ushort)UpstreamHANLogEventID.DeviceStateChange:
                {
                    NewEvent = new DeviceStateChangeHANLogEvent();
                    break;
                }
                case (ushort)UpstreamHANLogEventID.DeviceUEGChanged:
                {
                    NewEvent = new DeviceUEGChangedHANLogEvent();
                    break;
                }
                default:
                {
                    NewEvent = new UpstreamHANLogEvent(eventID);
                    break;
                }
            }

            return NewEvent;
        }

        /// <summary>
        /// Determines if the Upstream events are equal
        /// </summary>
        /// <param name="other">The Upstream event to compare to</param>
        /// <returns>True if the event numbers are the same. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public virtual bool Equals(UpstreamHANLogEvent other)
        {
            bool bEqual = false;

            if (other != null)
            {
                bEqual = m_EventID == other.m_EventID;
            }

            return bEqual;
        }

        /// <summary>
        /// Translates the argument data to a readable string
        /// </summary>
        /// <returns>The argument data as a readable string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  02/27/12 jrf 2.53.44 194023 Adding translation of Management Leave Requested
        //                              event argument data.
        //  03/20/12 jrf 2.53.53 194023 Reverting previous change.
        //
        public virtual string TranslateArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            string strArgumentData = "";

            try
            {
                switch (m_EventID)
                {
                    case (ushort)UpstreamHANLogEventID.LoadControlOptOut:
                    {
                        HANCriticalityLevel ArgumentCriticalityLevel = (HANCriticalityLevel)ArgumentReader.ReadByte();

                        strArgumentData = "Criticality Level: " + EnumDescriptionRetriever.RetrieveDescription(ArgumentCriticalityLevel);
                        break;
                    }
                    case (ushort)UpstreamHANLogEventID.LoadControlStatus:
                    {
                        HANCriticalityLevel ArgumentCriticalityLevel = (HANCriticalityLevel)ArgumentReader.ReadByte();
                        LoadControlEventStatus ArgumentEventStatus = (LoadControlEventStatus)ArgumentReader.ReadByte();

                        strArgumentData = "Criticality Level: " + EnumDescriptionRetriever.RetrieveDescription(ArgumentCriticalityLevel)
                            + " Event Status: " + EnumDescriptionRetriever.RetrieveDescription(ArgumentEventStatus);

                        break;
                    }
                    case (ushort)UpstreamHANLogEventID.DevicePinged:
                    {
                        PingResult ArgumentPingResult = (PingResult)ArgumentReader.ReadByte();

                        strArgumentData = EnumDescriptionRetriever.RetrieveDescription(ArgumentPingResult);
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // If we can't read the data for some reason just go with what we have.
            }

            //Closing ArgumentReader also closes ArgumentStream
            ArgumentReader.Dispose();

            return strArgumentData;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Description of the event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public string Description
        {
            get
            {
                return m_strDescription;
            }
        }

        /// <summary>
        /// Gets the Event Number of the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort EventID
        {
            get
            {
                return m_EventID;
            }
            internal set
            {
                m_EventID = value;
                DetermineDescription();
            }
        }

        /// <summary>
        /// Gets the Date and Time the event occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public DateTime TimeOccurred
        {
            get
            {
                return m_TimeOccurred;
            }
            set
            {
                m_TimeOccurred = value;
            }
        }

        /// <summary>
        /// Gets the event number in the log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort EventNumber
        {
            get
            {
                return m_EventNumber;
            }
            internal set
            {
                m_EventNumber = value;
            }
        }

        /// <summary>
        /// Gets the sequence number in the log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort SequenceNumber
        {
            get
            {
                return m_SequenceNumber;
            }
            internal set
            {
                m_SequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets the User ID of the user that caused the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort UserID
        {
            get
            {
                return m_UserID;
            }
            internal set
            {
                m_UserID = value;
            }
        }

        /// <summary>
        /// Gets the HAN's Event ID for the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public uint HANEventID
        {
            get
            {
                return m_HANEventID;
            }
            set
            {
                m_HANEventID = value;
            }
        }

        /// <summary>
        /// Gets the Mac Address of the HAN device in the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ulong MACAddress
        {
            get
            {
                return m_MACAddress;
            }
            set
            {
                m_MACAddress = value;
            }
        }

        /// <summary>
        /// Gets the event arguments
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public byte[] Argument
        {
            get
            {
                return (byte[])m_Argument.Clone();
            }
            set
            {
                m_Argument = value;
                ParseArgumentData();
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventID">The event number</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        internal UpstreamHANLogEvent(ushort eventID)
        {
            m_EventID = eventID;
            DetermineDescription();
            m_TimeOccurred = DateTime.MinValue;
            m_EventNumber = 0;
            m_SequenceNumber = 0;
            m_UserID = 0;
            m_HANEventID = 0;
            m_MACAddress = 0;
            m_Argument = null;
        }

        /// <summary>
        /// Gets or sets the time format to use
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        internal PSEMBinaryReader.TM_FORMAT TimeFormat
        {
            get
            {
                return m_TimeFormat;
            }
            set
            {
                m_TimeFormat = value;
            }
        }

        /// <summary>
        /// Gets the size of an individual Upstream event log entry
        /// </summary>
        /// <param name="table2239">The Table 2239 object for the current device</param>
        /// <param name="ltimeSize">The size of an LTIME data type</param>
        /// <returns>The size of the entry in bytes.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        internal static uint GetEntrySize(OpenWayMFGTable2239 table2239, uint ltimeSize)
        {
            uint EntrySize = 0;

            if (table2239 != null)
            {
                if (table2239.IsLoggingDateAndTime)
                {
                    EntrySize += ltimeSize;
                }

                if (table2239.IsLoggingEventNumber)
                {
                    EntrySize += 2;
                }

                if (table2239.IsLoggingSequenceNumber)
                {
                    EntrySize += 2;
                }

                EntrySize += 16;

                EntrySize += table2239.UpstreamDataLength;
            }

            return EntrySize;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the argument data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created

        protected virtual void ParseArgumentData()
        {
            // This method will be overridden by classes that inherit from this class to parse
            // specific argument data
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the description for the event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private void DetermineDescription()
        {
            CENTRON_AMI_UpstreamHANEventDictionary EventDictionary = new CENTRON_AMI_UpstreamHANEventDictionary();

            if (EventDictionary.ContainsKey(m_EventID))
            {
                m_strDescription = EventDictionary[m_EventID];
            }
            else
            {
                m_strDescription = "Unknown Event " + m_EventID.ToString(CultureInfo.CurrentCulture);
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// Time Format
        /// </summary>
        protected PSEMBinaryReader.TM_FORMAT m_TimeFormat;
        /// <summary>
        /// Event ID
        /// </summary>
        protected ushort m_EventID;
        /// <summary>
        /// Description
        /// </summary>
        protected string m_strDescription;
        /// <summary>
        /// Time the event occurred
        /// </summary>
        protected DateTime m_TimeOccurred;
        /// <summary>
        /// The event number
        /// </summary>
        protected ushort m_EventNumber;
        /// <summary>
        /// The sequence number
        /// </summary>
        protected ushort m_SequenceNumber;
        /// <summary>
        /// The user ID
        /// </summary>
        protected ushort m_UserID;
        /// <summary>
        /// The HAN Event ID
        /// </summary>
        protected uint m_HANEventID;
        /// <summary>
        /// The MAC address of the device that caused the event
        /// </summary>
        protected ulong m_MACAddress;
        /// <summary>
        /// The event arguments
        /// </summary>
        protected byte[] m_Argument;

        #endregion

    }

    /// <summary>
    /// Device State Change HAN Log Event
    /// </summary>
    public class DeviceStateChangeHANLogEvent : UpstreamHANLogEvent
    {
        #region Definitions

        /// <summary>
        /// The status indicating the state of the network.
        /// </summary>
        public enum NetworkStatus : byte
        {
            /// <summary>
            /// Not Registered
            /// </summary>
            [EnumDescription("Not Registered")]
            NotRegistered = 0x00,
            /// <summary>
            /// Registration Failed
            /// </summary>
            [EnumDescription("Registration Failed")]
            RegistrationFailed = 0x01,
            /// <summary>
            /// Registration Success
            /// </summary>
            [EnumDescription("Registration Success")]
            RegistrationSuccess = 0x02,
            /// <summary>
            /// Invalid Certificate
            /// </summary>
            [EnumDescription("Invalid Certificate")]
            InvalidCertificate = 0x03,
            /// <summary>
            /// Pending
            /// </summary>
            [EnumDescription("Pending")]
            Pending = 0x04,
            /// <summary>
            /// Network Up
            /// </summary>
            [EnumDescription("Network Up")]
            NetworkUp = 0x05,
            /// <summary>
            /// Network Down
            /// </summary>
            [EnumDescription("Network Down")]
            NetworkDown = 0x06,
            /// <summary>
            /// Network Forming
            /// </summary>
            [EnumDescription("Network Forming")]
            NetworkForming = 0x07,
            /// <summary>
            /// Network Joining
            /// </summary>
            [EnumDescription("Network Joining")]
            NetworkJoining = 0x08,
            /// <summary>
            /// Private Profile
            /// </summary>
            [EnumDescription("Private Profile")]
            PrivateProfile = 0x09,
            /// <summary>
            /// Via Binding
            /// </summary>
            [EnumDescription("Via Binding")]
            ViaBinding = 0x0A,
            /// <summary>
            /// Register Fatal Error
            /// </summary>
            [EnumDescription("Register Fatal Error")]
            FatalError = 0x0B,
            /// <summary>
            /// Via Key Establishment
            /// </summary>
            [EnumDescription("Via Key Establishment")]
            ViaKeyEstablishment = 0x0C,
            /// <summary>
            /// Invalid
            /// </summary>
            [EnumDescription("Invalid")]
            Invalid = 0xFF,
        }

        /// <summary>
        /// Failure Reason Code
        /// </summary>
        public enum FailureReasonCode : byte
        {
            /// <summary>
            /// Success/Reserved
            /// </summary>
            [EnumDescription("Success")]
            Success = 0x00,
            /// <summary>
            /// Unknown Issuer
            /// </summary>
            [EnumDescription("Unknown Issuer")]
            UnknownIssuer = 0x01,
            /// <summary>
            /// Bad Key Confirm
            /// </summary>
            [EnumDescription("Bad Key Confirm")]
            BadKeyConfirm = 0x02,
            /// <summary>
            /// Bad Message
            /// </summary>
            [EnumDescription("Bad Message")]
            BadMessage = 0x03,
            /// <summary>
            /// No Resources
            /// </summary>
            [EnumDescription("No Resources")]
            NoResources = 0x04,
            /// <summary>
            /// Unsupported Suite
            /// </summary>
            [EnumDescription("Unsupported Suite")]
            UnsupportedSuite = 0x05,
            /// <summary>
            /// Invalid Parameters
            /// </summary>
            [EnumDescription("Invalid Parameters")]
            InvalidParameters = 0x06,
            /// <summary>
            /// Timeout
            /// </summary>
            [EnumDescription("Timeout")]
            Timeout = 0x07,
            /// <summary>
            /// TX Failure
            /// </summary>
            [EnumDescription("TX Failure")]
            TXFailure = 0x08,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Translates the argument data to a readable string
        /// </summary>
        /// <returns>The argument data as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/04/11 RCG 2.51.33        Created
        //  05/07/13 MSC 2.80.26 TQ6698 Added Support for Registration Failure Reason Code
        //
        public override string TranslateArgumentData()
        {
            return "Previous State: " + EnumDescriptionRetriever.RetrieveDescription(m_PreviousState)
                + " New State: " + EnumDescriptionRetriever.RetrieveDescription(m_NewState)
                + RegistrationFailed(m_RegistrationFailed);
        }

        /// <summary>
        /// Translates the Reason Code data to a readable string
        /// </summary>
        /// <param name="Reason">Reason Code of Failure</param>
        /// <returns>The Reason Code data as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/13 MSC 2.80.26 TQ6698 Created
        //
        public string RegistrationFailed(FailureReasonCode Reason)
        {
            if (Reason != FailureReasonCode.Success)
            {
                return " Failure Reason: " + EnumDescriptionRetriever.RetrieveDescription(Reason);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Determines whether or not the Device State Changed events are equal
        /// </summary>
        /// <param name="other">The event to compare against</param>
        /// <returns>True if the two events are equal. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/12 RCG 2.60.33        Created

        public override bool Equals(UpstreamHANLogEvent other)
        {
            bool IsEqual = base.Equals(other);

            if (IsEqual)
            {
                // At this point we know both events should be the same type (RIB Meter Event) now we need to make sure the sub event type is the same
                DeviceStateChangeHANLogEvent OtherDeviceStateChangeEvent = other as DeviceStateChangeHANLogEvent;

                // There are some cases where we are looking for just the event type such as the Event Log Summary Lists. In those cases
                // we don't care what sub type the event is so lets only check the sub event type if Sub Event type was specifically set
                // using the property.
                if (m_SpecificStateSpecifiedForComparison || OtherDeviceStateChangeEvent.m_SpecificStateSpecifiedForComparison)
                {
                    if (OtherDeviceStateChangeEvent != null)
                    {
                        IsEqual = this.NewState == OtherDeviceStateChangeEvent.NewState;
                    }
                    else
                    {
                        // This shouldn't happen but just in case...
                        IsEqual = false;
                    }
                }
            }

            return IsEqual;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Previous Device State
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/04/11 RCG 2.51.33        Created

        public NetworkStatus PreviousState
        {
            get
            {
                return m_PreviousState;
            }
        }

        /// <summary>
        /// Gets the New Device State
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/04/11 RCG 2.51.33        Created
        
        public NetworkStatus NewState
        {
            get
            {
                return m_NewState;
            }
            set
            {
                m_NewState = value;
                m_SpecificStateSpecifiedForComparison = true;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be used by the UpstreamHANLogEvent.Create method
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/04/11 RCG 2.51.33        Created
        //  05/07/13 MSC 2.80.26 TQ6698 Registration Failed Support added
        //
        internal DeviceStateChangeHANLogEvent()
            : base((ushort)UpstreamHANLogEventID.DeviceStateChange)
        {
            m_NewState = NetworkStatus.Invalid;
            m_PreviousState = NetworkStatus.Invalid;
            m_RegistrationFailed = FailureReasonCode.Success;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the argument data for the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/04/11 RCG 2.51.33        Created
        //  05/07/13 MSC 2.80.26 TQ6698 Added support for Failure Messages
        //
        protected override void ParseArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            try
            {
                m_PreviousState = (NetworkStatus)ArgumentReader.ReadByte();
                m_NewState = (NetworkStatus)ArgumentReader.ReadByte();
                m_RegistrationFailed = (FailureReasonCode)ArgumentReader.ReadByte();
            }
            catch (Exception)
            {
                // If there is an exception we should just use what we have rather than pass up the error
            }

            //Closing ArgumentReader also closes ArgumentStream
            ArgumentReader.Dispose();
        }

        #endregion

        #region Member Variables

        private NetworkStatus m_PreviousState;
        private NetworkStatus m_NewState;
        private bool m_SpecificStateSpecifiedForComparison;
        private FailureReasonCode m_RegistrationFailed;

        #endregion
    }

    /// <summary>
    /// Device Utility Enrollment Group Changed event
    /// </summary>
    public class DeviceUEGChangedHANLogEvent : UpstreamHANLogEvent
    {
        #region Public Methods

        /// <summary>
        /// Translates the argument data to a readable string
        /// </summary>
        /// <returns>The argument data as a string</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/25/12 RCG 2.60.36        Created

        public override string TranslateArgumentData()
        {
            return "New Utility Enrollment Group: " + m_NewUtilityEnrollmentGroup.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the new Utility Enrollment Group
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/25/12 RCG 2.60.36        Created
        
        public byte NewUtilityEnrollmentGroup
        {
            get
            {
                return m_NewUtilityEnrollmentGroup;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be used by the UpstreamHANLogEvent.Create method
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/25/12 RCG 2.60.36        Created
        //  08/07/13 AF  2.85.13 WR419080 Replaced EUG with UEG
        //
        internal DeviceUEGChangedHANLogEvent()
            : base((ushort)UpstreamHANLogEventID.DeviceUEGChanged)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the argument data for the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/25/12 RCG 2.60.36        Created
        
        protected override void ParseArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            try
            {
                m_NewUtilityEnrollmentGroup = ArgumentReader.ReadByte();
            }
            catch (Exception)
            {
                // If there is an exception we should just use what we have rather than pass up the error
            }

            //Closing ArgumentReader also closes ArgumentStream
            ArgumentReader.Dispose();
        }

        #endregion

        #region Member Variables

        private byte m_NewUtilityEnrollmentGroup;        

        #endregion
    }

    /// <summary>
    /// Downstream HAN Log Event
    /// </summary>
    public class DownstreamHANLogEvent : IEquatable<DownstreamHANLogEvent>
    {
        #region Definitions

        /// <summary>
        /// UTC reference time
        /// </summary>
        protected static readonly DateTime UTC_REFERENCE_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Downstream Event IDs
        /// </summary>
        /// /// <remarks>
        /// If you add a new value here please add a new value to the
        /// DownstreamHANEvents enum of the EndpointServerMeterEvents class
        /// in the ServiceResponseData.cs file of the
        /// Itron.Metering.MeterServer project and also update the
        /// CENTRON_AMI_DownstreamHANEventDictionary.
        /// </remarks>
        public enum DownstreamHANLogEventID : ushort
        {
            /// <summary>
            /// Message Published
            /// </summary>
            MessagePublished = 2305,
            /// <summary>
            /// Pricing Data Published
            /// </summary>
            PricingDataPublished = 2306,
            /// <summary>
            /// DRLC Data Published
            /// </summary>
            DRLCDataPublished = 2307,
            /// <summary>
            /// Price Tier Changed
            /// </summary>
            PriceTierChanged = 2308,
            /// <summary>
            /// New DRLC Event Dropped
            /// </summary>
            NewDRLCEventDropped = 2310,
            /// <summary>
            /// Existing DRLC Event Dropped
            /// </summary>
            ExistingDRLCEventDropped = 2311,
            /// <summary>
            /// Attribute Write
            /// </summary>
            AttributeWrite = 2312,
            /// <summary>
            /// Network Changed
            /// </summary>
            MeterNetworkChange = 2313,
            /// <summary>
            /// Security Profile Update Notification
            /// </summary>
            SecurityProfileUpdateNotification = 2314,
            /// <summary>
            /// Diagnostic System Event
            /// </summary>
            DiagnosticSystemEvent = 2315,
            /// <summary>
            /// Move Out
            /// </summary>
            MoveOut = 2316,
            /// <summary>
            /// RIB Meter Event
            /// </summary>
            RIBMeterEvent = 2317,
            /// <summary>
            /// Next Block Price Schedule Unsealed
            /// </summary>
            RIBCEEvent = 2318,
            /// <summary>
            /// RIB Error Event
            /// </summary>
            RIBErrorEvent = 2319,
            /// <summary>
            /// RIB Next Block Price Commit Timeout
            /// </summary>
            NextBlockPriceCommitTimeout = 2320,
            /// <summary>
            /// Price Disable occurred
            /// </summary>
            PriceDisabled = 2321,
            /// <summary>
            /// Event Cache Overflow
            /// </summary>
            EventCacheOverflow = 2431,
        }

        /// <summary>
        /// Diagnostic System Event IDs
        /// </summary>
        public enum DiagnosticSystemEventID : byte
        {
            /// <summary>
            /// Channel Changed
            /// </summary>
            [EnumDescription("Channel Changed")]
            ChannelChange = 0x00,
            /// <summary>
            /// PAN ID Changed
            /// </summary>
            [EnumDescription("PAN ID Changed")]
            PanIDChange = 0x01,
            /// <summary>
            /// ZigBee Halted Due To Reset Limiting
            /// </summary>
            [EnumDescription("ZigBee Halted Due To Reset Limiting")]
            ZigBeeHaltResetLimiting = 0x02,
            /// <summary>
            /// Configuration Changed
            /// </summary>
            [EnumDescription("Configuration Changed")]
            ConfigChange = 0x03,
            /// <summary>
            /// Gas Module Told To Leave
            /// </summary>
            [EnumDescription("Gas Module Told To Leave")]
            GasModuleLeaveRequest = 0x04,
            /// <summary>
            /// Management Leave Requested
            /// </summary>
            [EnumDescription("Management Leave Requested")]
            ManagementLeaveRequest = 0x05,
            /// <summary>
            /// Meter Cold Started
            /// </summary>
            [EnumDescription("Meter Cold Started")]
            MeterColdStart = 0x06,
            /// <summary>
            /// Invalid
            /// </summary>
            [EnumDescription("Invalid")]
            Invalid = 0xFF,
        }

        /// <summary>
        /// Event types for the RIB CE HAN Event
        /// </summary>
        private enum RIBCEEventID : byte
        {
            [EnumDescription("Next Block Schedule Written")]
            NextBlockScheduleWritten = 0x00,
            [EnumDescription("Next Block Schedule Became Active Schedule")]
            NextBlockScheduleBecameActiveSchedule = 0x01,
            [EnumDescription("Next Block Schedule Committed")]
            NextBlockScheduleCommitted = 0x02,
            [EnumDescription("Invalid")]
            Invalid = 0xFF,
        }

        /// <summary>
        /// Event types for the RIB Error HAN Event
        /// </summary>
        private enum RIBErrorEventID : byte
        {
            [EnumDescription("No Next Block Price Schedule Exists")]
            NoNextBlockPriceSchedule = 0x00,
            [EnumDescription("Invalid")]
            Invalid = 0xFF,
        }

        /// <summary>
        /// The price mode in use before it was cancelled
        /// </summary>
        private enum PreviousPriceMode : byte
        {
            [EnumDescription("RIB Pricing")]
            RIBPricing = 0x00,
            [EnumDescription("Invalid")]
            Invalid = 0xFF,
        }

        /// <summary>
        /// Items changed in the configuration.
        /// </summary>
        [Flags]
        public enum ConfigChangeFlags : uint
        {
            /// <summary>
            /// None
            /// </summary>
            [EnumDescription("None")]
            None = 0x00,
            /// <summary>
            /// Version
            /// </summary>
            [EnumDescription("Version")]
            Version = 0x01,
            /// <summary>
            /// Channel Mask
            /// </summary>
            [EnumDescription("Channel Mask")]
            ChannelMask = 0x02,
            /// <summary>
            /// Power Level
            /// </summary>
            [EnumDescription("Power Level")]
            PowerLevel = 0x04,
            /// <summary>
            /// Security Mode
            /// </summary>
            [EnumDescription("Security Mode")]
            SecurityMode = 0x08,
            /// <summary>
            /// CBKE Mode
            /// </summary>
            [EnumDescription("CBKE Mode")]
            CBKEMode = 0x10,
            /// <summary>
            /// Device Authorization Mode
            /// </summary>
            [EnumDescription("Device Authorization Mode")]
            DeviceAuthMode = 0x20,
            /// <summary>
            /// Link Key Authorization Mode
            /// </summary>
            [EnumDescription("Link Key Authorization Mode")]
            LinkKeyAuthMode = 0x40,
            /// <summary>
            /// Pricing Options
            /// </summary>
            [EnumDescription("Pricing Options")]
            PricingOptions = 0x80,
            /// <summary>
            /// Inter PAN Allowed
            /// </summary>
            [EnumDescription("Inter PAN Allowed")]
            InterPANAllowed = 0x100,
            /// <summary>
            /// Simple Metering Multiplier
            /// </summary>
            [EnumDescription("Simple Metering Multiplier")]
            SimpleMeteringMultiplier = 0x200,
            /// <summary>
            /// Simple Metering Divisor
            /// </summary>
            [EnumDescription("Simple Metering Divisor")]
            SimpleMeteringDivisor = 0x400,
            /// <summary>
            /// Network Formation Option
            /// </summary>
            [EnumDescription("Network Formation Option")]
            NetworkFormationOption = 0x800,
        }

        internal enum DeviceAttributes : uint
        {
            [EnumDescription("Reserved")]
            Reserved = 0,
            [EnumDescription("Utility Enrollment Group")]
            UtilityEnrollmentGroup = 1,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the Downstream HAN event object based on the event ID
        /// </summary>
        /// <param name="eventID">The event ID for the event to create</param>
        /// <returns>The new Downstream HAN event object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        //  11/07/12 MSC 2.70.36 240488 Added Support for DRLC Data Published event
        //
        public static DownstreamHANLogEvent Create(ushort eventID)
        {
            DownstreamHANLogEvent NewEvent = null;

            switch(eventID)
            {
                case (ushort)DownstreamHANLogEventID.MessagePublished:
                {
                    NewEvent = new MessagePublishedHANLogEvent();
                    break;
                }
                case (ushort)DownstreamHANLogEventID.DRLCDataPublished:
                {
                    NewEvent = new DRLCDataPublishedHANLogEvent();
                    break;
                }
                case (ushort)DownstreamHANLogEventID.MeterNetworkChange:
                {
                    NewEvent = new MeterNetworkChangedHANLogEvent();
                    break;
                }
                case (ushort)DownstreamHANLogEventID.PriceTierChanged:
                {
                    NewEvent = new PriceTierChangedHANLogEvent();
                    break;
                }
                case (ushort)DownstreamHANLogEventID.RIBMeterEvent:
                {
                    NewEvent = new RIBMeterEventHANLogEvent();
                    break;
                }
                default:
                {
                    NewEvent = new DownstreamHANLogEvent(eventID);
                    break;
                }
            }

            return NewEvent;
        }

        /// <summary>
        /// Determines if the Upstream events are equal
        /// </summary>
        /// <param name="other">The Upstream event to compare to</param>
        /// <returns>True if the event numbers are the same. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public virtual bool Equals(DownstreamHANLogEvent other)
        {
            bool bEqual = false;

            if (other != null)
            {
                bEqual = m_EventID == other.m_EventID;
            }

            return bEqual;
        }

        /// <summary>
        /// Translates the argument data to a readable string
        /// </summary>
        /// <returns>The argument data as a readable string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  06/01/11 AF  2.51.03 172967 Added translation of price tier changed event args
        //  03/20/12 jrf 2.53.51 194023 Adding translation of diagnostic system event argument data.

        public virtual string TranslateArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            string strArgumentData = "";

            try
            {
                switch (m_EventID)
                {
                    case (ushort)DownstreamHANLogEventID.AttributeWrite:
                    {
                        DeviceAttributes Attribute = (DeviceAttributes)ArgumentReader.ReadUInt32();

                        strArgumentData = "Attribute: " + EnumDescriptionRetriever.RetrieveDescription(Attribute);

                        break;
                    }
                    case (ushort)DownstreamHANLogEventID.DiagnosticSystemEvent:
                    {
                        //The first part of the argument is the subevent name
                        byte byEventNumber = ArgumentReader.ReadByte();

                        if (true == Enum.IsDefined(typeof(DiagnosticSystemEventID), byEventNumber))
                        {
                            strArgumentData = EnumDescriptionRetriever.RetrieveDescription((DiagnosticSystemEventID)byEventNumber);
                        }
                        else
                        {
                            strArgumentData = "Unknown Type: " + byEventNumber;
                        }
                        
                        //The second part of the argument is the subevent's argument data.
                        switch (byEventNumber)
                        {
                            case (byte)DiagnosticSystemEventID.ChannelChange:
                            {
                                byte byOldChannel = ArgumentReader.ReadByte();
                                byte byNewChannel = ArgumentReader.ReadByte();

                                strArgumentData += " From " + byOldChannel + " To " + byNewChannel;
                                break;
                            }
                            case (byte)DiagnosticSystemEventID.PanIDChange:
                            {
                                byte[] abytOldPID = ArgumentReader.ReadBytes(sizeof(ushort));
                                byte[] abytNewPID = ArgumentReader.ReadBytes(sizeof(ushort));


                                ushort usOldPID = 0; 
                                ushort usNewPID = 0;

                                //Converting from Big Endian to Little Endian byte order
                                Array.Reverse(abytOldPID);
                                Array.Reverse(abytNewPID);

                                usOldPID = BitConverter.ToUInt16(abytOldPID, 0);
                                usNewPID = BitConverter.ToUInt16(abytNewPID, 0);

                                strArgumentData += " From " + usOldPID + " To " + usNewPID;
                                break;
                            }
                            case (byte)DiagnosticSystemEventID.ConfigChange:
                            {
                                byte[] abytConfigChanges = ArgumentReader.ReadBytes(sizeof(UInt32));
                                ConfigChangeFlags ConfigChanges = ConfigChangeFlags.None;
                                List<string> lststrChanges = new List<string>();
                                string strChanges = "";

                                //Converting from Big Endian to Little Endian byte order
                                Array.Reverse(abytConfigChanges);

                                ConfigChanges = (ConfigChangeFlags)BitConverter.ToUInt32(abytConfigChanges, 0);


                                //Check each possible config change value to see if it's bit is set
                                foreach (ConfigChangeFlags ChangeFlag in Enum.GetValues(typeof(ConfigChangeFlags)))
                                {
                                    if (ChangeFlag == (ConfigChanges & ChangeFlag) && ConfigChangeFlags.None != ChangeFlag)
                                    {
                                        lststrChanges.Add(EnumDescriptionRetriever.RetrieveDescription(ChangeFlag));
                                    }
                                }

                                //Create a comma separated list of the changes
                                strChanges = String.Join(", ", lststrChanges.ToArray());

                                strArgumentData += " For The Following Item(s): " + strChanges;
                                
                                break;
                            }
                            case (byte)DiagnosticSystemEventID.GasModuleLeaveRequest:
                            {
                                byte[] abytGasModuleMAC = ArgumentReader.ReadBytes(sizeof(UInt32));
                                UInt32 uiGasModuleMAC = 0;

                                //Converting from Big Endian to Little Endian byte order
                                Array.Reverse(abytGasModuleMAC);

                                uiGasModuleMAC = BitConverter.ToUInt32(abytGasModuleMAC, 0);

                                strArgumentData += " (MAC Address: " + uiGasModuleMAC.ToString("X8", CultureInfo.InvariantCulture) + ")";

                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                        break;
                    }
                    case (ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.RIBCEEvent:
                    {
                        byte EventType = ArgumentReader.ReadByte();

                        if (true == Enum.IsDefined(typeof(RIBCEEventID), EventType))
                        {
                            strArgumentData = EnumDescriptionRetriever.RetrieveDescription((RIBCEEventID)EventType);
                        }
                        else
                        {
                            strArgumentData = "Unknown Type: " + EventType;
                        }

                        break;
                    }
                    case (ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.RIBErrorEvent:
                    {
                        byte EventType = ArgumentReader.ReadByte();

                        if (true == Enum.IsDefined(typeof(RIBErrorEventID), EventType))
                        {
                            strArgumentData = EnumDescriptionRetriever.RetrieveDescription((RIBErrorEventID)EventType);
                        }
                        else
                        {
                            strArgumentData += "Unknown Type: " + EventType;
                        }

                        break;
                    }
                    case (ushort)DownstreamHANLogEvent.DownstreamHANLogEventID.PriceDisabled:
                    {
                        strArgumentData = "";
                        byte PriceMode = ArgumentReader.ReadByte();

                        if (true == Enum.IsDefined(typeof(PreviousPriceMode), PriceMode))
                        {
                            strArgumentData = EnumDescriptionRetriever.RetrieveDescription((PreviousPriceMode)PriceMode);
                        }
                        else
                        {
                            strArgumentData += "Unknown Type: " + PriceMode.ToString(CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // If we can't read the data for some reason just go with what we have.
            }

            //Closing ArgumentReader also closes ArgumentStream
            ArgumentReader.Dispose();

            return strArgumentData;

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Description of the event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public string Description
        {
            get
            {
                return m_strDescription;
            }
        }

        /// <summary>
        /// Gets the Event Number of the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort EventID
        {
            get
            {
                return m_EventID;
            }
            internal set
            {
                m_EventID = value;
                DetermineDescription();
            }
        }

        /// <summary>
        /// Gets the Date and Time the event occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public DateTime TimeOccurred
        {
            get
            {
                return m_TimeOccurred;
            }
            set
            {
                m_TimeOccurred = value;
            }
        }

        /// <summary>
        /// Gets the event number in the log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort EventNumber 
        {
            get
            {
                return m_EventNumber;
            }
            internal set
            {
                m_EventNumber = value;
            }
        }

        /// <summary>
        /// Gets the sequence number in the log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort SequenceNumber 
        {
            get 
            {
                return m_SequenceNumber;
            }
            internal set 
            {
                m_SequenceNumber = value;
            } 
        }

        /// <summary>
        /// Gets the User ID of the user that caused the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort UserID 
        {
            get 
            {
                return m_UserID;
            }
            internal set 
            {
                m_UserID = value;
            } 
        }

        /// <summary>
        /// Gets the HAN's Event ID for the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public uint HANEventID 
        {
            get 
            {
                return m_HANEventID;
            }
            set 
            {
                m_HANEventID = value;
            } 
        }

        /// <summary>
        /// Gets the event arguments
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public byte[] Argument 
        {
            get 
            {
                return (byte[])m_Argument.Clone();
            }
            set 
            {
                m_Argument = value;
                ParseArgumentData();
            } 
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventID">The event number</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        internal DownstreamHANLogEvent(ushort eventID)
        {
            m_EventID = eventID;
            DetermineDescription();
            m_TimeOccurred = DateTime.MinValue;
            m_EventNumber = 0;
            m_SequenceNumber = 0;
            m_UserID = 0;
            m_HANEventID = 0;
            m_Argument = null;

        }

        /// <summary>
        /// Gets the size of an individual Upstream event log entry
        /// </summary>
        /// <param name="table2239">The Table 2239 object for the current device</param>
        /// <param name="ltimeSize">The size of an LTIME data type</param>
        /// <returns>The size of the entry in bytes.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        internal static uint GetEntrySize(OpenWayMFGTable2239 table2239, uint ltimeSize)
        {
            uint EntrySize = 0;

            if (table2239 != null)
            {
                if (table2239.IsLoggingDateAndTime)
                {
                    EntrySize += ltimeSize;
                }

                if (table2239.IsLoggingEventNumber)
                {
                    EntrySize += 2;
                }

                if (table2239.IsLoggingSequenceNumber)
                {
                    EntrySize += 2;
                }

                EntrySize += 8;

                EntrySize += table2239.DownstreamDataLength;
            }

            return EntrySize;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the argument data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        
        protected virtual void ParseArgumentData()
        {
            // This method will be overridden by classes that inherit from this class to parse
            // specific argument data
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the description for the event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private void DetermineDescription()
        {
            CENTRON_AMI_DownstreamHANEventDictionary EventDictionary = new CENTRON_AMI_DownstreamHANEventDictionary();

            if (EventDictionary.ContainsKey(m_EventID))
            {
                m_strDescription = EventDictionary[m_EventID];
            }
            else
            {
                m_strDescription = "Unknown Event " + m_EventID.ToString(CultureInfo.CurrentCulture);
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// Event ID
        /// </summary>
        protected ushort m_EventID;
        /// <summary>
        /// Description
        /// </summary>
        protected string m_strDescription;
        /// <summary>
        /// Time the event occurred
        /// </summary>
        protected DateTime m_TimeOccurred;
        /// <summary>
        /// The event number
        /// </summary>
        protected ushort m_EventNumber;
        /// <summary>
        /// The sequence number
        /// </summary>
        protected ushort m_SequenceNumber;
        /// <summary>
        /// The user ID 
        /// </summary>
        protected ushort m_UserID;
        /// <summary>
        /// The HAN Event number
        /// </summary>
        protected uint m_HANEventID;
        /// <summary>
        /// The event arguments
        /// </summary>
        protected byte[] m_Argument;

        #endregion
    }

    /// <summary>
    /// Message Published HAN Log Event
    /// </summary>
    public class MessagePublishedHANLogEvent : DownstreamHANLogEvent
    {
        #region Definitions

        /// <summary>
        /// The type of message
        /// </summary>
        public enum MessageTypes : byte
        {
            /// <summary>Normal Message</summary>
            [EnumDescription("Normal Message")]
            Normal = 0x00,
            /// <summary>Cancel Message</summary>
            [EnumDescription("Cancel Message")]
            Cancel = 0x01,
            /// <summary>*Invalid</summary>
            [EnumDescription("Invalid")]
            Invalid = 0x02
        }

        /// <summary>
        /// The message start types
        /// </summary>
        public enum StartTypes : byte
        {
            /// <summary>Starts immediately</summary>
            [EnumDescription("Immediate")]
            ImmediateStart = 0x00,
            /// <summary>The start of the message was scheduled</summary>
            [EnumDescription("Scheduled")]
            ScheduledStart = 0x01,
            /// <summary>Invalid</summary>
            [EnumDescription("Invalid")]
            Invalid = 0x02,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Translates the argument data to readable text
        /// </summary>
        /// <returns>The argument data as readable text</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/12 RCG 2.60.20        Created

        public override string TranslateArgumentData()
        {
            return "Message ID: " + MessageID.ToString(CultureInfo.InvariantCulture) + " Message Type: " + EnumDescriptionRetriever.RetrieveDescription(MessageType)
                + " Start Type: " + EnumDescriptionRetriever.RetrieveDescription(StartType);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be used by DownstreamHANEventLog.Create()
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/12 RCG 2.60.20        Created

        internal MessagePublishedHANLogEvent()
            : base((ushort)DownstreamHANLogEventID.MessagePublished)
        {
            m_MessageType = MessageTypes.Invalid;
            m_StartType = StartTypes.Invalid;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the argument data 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/12 RCG 2.60.20        Created

        protected override void ParseArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            m_MessageType = (MessageTypes)ArgumentReader.ReadByte();
            m_StartType = (StartTypes)ArgumentReader.ReadByte();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the message ID of the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/12 RCG 2.60.20        Created

        public uint MessageID
        {
            get
            {
                return HANEventID;
            }
        }

        /// <summary>
        /// Gets the type of message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/12 RCG 2.60.20        Created

        public MessageTypes MessageType
        {
            get
            {
                return m_MessageType;
            }
        }

        /// <summary>
        /// Gets the start type of the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/12 RCG 2.60.20        Created

        public StartTypes StartType
        {
            get
            {
                return m_StartType;
            }
        }

        #endregion

        #region Member Variables

        private MessageTypes m_MessageType;
        private StartTypes m_StartType;

        #endregion
    }

    /// <summary>
    /// DRLC Data Published HAN Log Event
    /// </summary>
    public class DRLCDataPublishedHANLogEvent : DownstreamHANLogEvent
    {
        #region Constants

        /// <summary>
        /// Mask to check last four bits
        /// </summary>
        protected readonly byte NIBBLE_MASK = 0x0F;

        /// <summary>
        /// Invalid (Hydrogen and Post Hydrogen builds)
        /// </summary>
        protected readonly byte INVALID = 0xFF;

        #endregion

        #region Definitions

        /// <summary>
        /// The type of DRLC message
        /// </summary>
        public enum MessageTypes : byte
        {
            /// <summary>DRLC Event</summary>
            [EnumDescription("DRLC Event")]
            Normal = 0x00,
            /// <summary>Cancel DRLC Event</summary>
            [EnumDescription("Cancel DRLC Event")]
            Cancel = 0x01,
            /// <summary>Cancel All DRLC Event</summary>
            [EnumDescription("Cancel All DRLC Event")]
            CancelAll = 0x02,
            /// <summary>*Invalid</summary>
            [EnumDescription("Invalid")]
            Invalid = 0xFF
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Translates the argument data to readable text
        /// </summary>
        /// <returns>The argument data as readable text</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/12 MSC 2.70.36 240488 Created
        //
        public override string TranslateArgumentData()
        {
            return "Event ID: " + MessageID + " Message Type: " + EnumDescriptionRetriever.RetrieveDescription(MessageType);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be used by DownstreamHANEventLog.Create()
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/12 MSC 2.70.36 240488 Created
        //
        internal DRLCDataPublishedHANLogEvent()
            : base((ushort)DownstreamHANLogEventID.DRLCDataPublished)
        {
            m_MessageType = MessageTypes.Invalid;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the argument data 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/12 MSC 2.70.36 240488 Created
        //  11/15/12 MSC 2.70.38 N/A    Hydrogen compatible with use of "Nibble Mask"
        //
        protected override void ParseArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);
            ArgumentReader.ReadByte(); //Sealing method
            ArgumentReader.ReadByte(); //Registration Table Entry #
            byte byMessageType = ArgumentReader.ReadByte();

            if (byMessageType != INVALID)
            {
                byMessageType &= NIBBLE_MASK;
            }

            m_MessageType = (MessageTypes)byMessageType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the message ID of the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/12 MSC 2.70.36 240488 Created
        //
        public uint MessageID
        {
            get
            {
                return HANEventID;
            }
        }

        /// <summary>
        /// Gets the type of message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/07/12 MSC 2.70.36 240488 Created
        //
        public MessageTypes MessageType
        {
            get
            {
                return m_MessageType;
            }
        }

        #endregion

        #region Member Variables

        private MessageTypes m_MessageType;

        #endregion
    }

    /// <summary>
    /// Price Tier Change HAN Log Event
    /// </summary>
    public class PriceTierChangedHANLogEvent : DownstreamHANLogEvent
    {
        #region Public Methods

        /// <summary>
        /// Translates the argument data to readable text
        /// </summary>
        /// <returns>The argument data as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created

        public override string TranslateArgumentData()
        {
            return "New Tier: " + DetermineTier(m_NewTier) + ", Old Tier: " + DetermineTier(m_OldTier);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the new price tier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        
        public byte NewTier
        {
            get
            {
                return m_NewTier;
            }
            set
            {
                m_NewTier = value;
            }
        }
        
        /// <summary>
        /// Gets the old price tier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        
        public byte OldTier
        {
            get
            {
                return m_OldTier;
            }
            internal set
            {
                m_OldTier = value;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be used by DownstreamHANLogEvent.Create
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        
        public PriceTierChangedHANLogEvent()
            : base((ushort)DownstreamHANLogEventID.PriceTierChanged)
        {
            m_NewTier = 0;
            m_OldTier = 0;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the argument data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        
        protected override void ParseArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            try
            {
                m_NewTier = ArgumentReader.ReadByte();
                m_OldTier = ArgumentReader.ReadByte();
            }
            catch (Exception)
            {
                // If there is an exception we should just go with what we have.
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Translates the tier into a string
        /// </summary>
        /// <param name="byTier">the tier as read from the event argument</param>
        /// <returns>a string representation of the tier</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/01/11 AF  2.51.03 172967 Created
        //
        private string DetermineTier(byte byTier)
        {
            string strTier = "";

            if (byTier == (byte)TierLabel.RATE_NONE)
            {
                strTier = EnumDescriptionRetriever.RetrieveDescription(TierLabel.RATE_NONE);
            }
            else if (byTier == (byte)TierLabel.RATE_A)
            {
                strTier = EnumDescriptionRetriever.RetrieveDescription(TierLabel.RATE_A);
            }
            else if (byTier == (byte)TierLabel.RATE_B)
            {
                strTier = EnumDescriptionRetriever.RetrieveDescription(TierLabel.RATE_B);
            }
            else if (byTier == (byte)TierLabel.RATE_C)
            {
                strTier = EnumDescriptionRetriever.RetrieveDescription(TierLabel.RATE_C);
            }
            else if (byTier == (byte)TierLabel.RATE_D)
            {
                strTier = EnumDescriptionRetriever.RetrieveDescription(TierLabel.RATE_D);
            }
            else if (byTier == (byte)TierLabel.RATE_E)
            {
                strTier = EnumDescriptionRetriever.RetrieveDescription(TierLabel.RATE_E);
            }
            else if (byTier == (byte)TierLabel.RATE_F)
            {
                strTier = EnumDescriptionRetriever.RetrieveDescription(TierLabel.RATE_F);
            }
            else if (byTier == (byte)TierLabel.RATE_G)
            {
                strTier = EnumDescriptionRetriever.RetrieveDescription(TierLabel.RATE_G);
            }

            return strTier;
        }

        #endregion

        #region Member Variables

        private byte m_NewTier;
        private byte m_OldTier;

        #endregion
    }

    /// <summary>
    /// Meter Network Changed HAN Log Event
    /// </summary>
    public class MeterNetworkChangedHANLogEvent : DownstreamHANLogEvent
    {
        #region Definitions

        /// <summary>
        /// The network status for the Meter Network Changed event
        /// </summary>
        public enum NetworkStatus : byte
        {
            /// <summary>
            /// The Network is Up
            /// </summary>
            [EnumDescription("Network Up")]
            NetworkUp = 0,
            /// <summary>
            /// The Network is Down
            /// </summary>
            [EnumDescription("Network Down")]
            NetworkDown = 1,
            /// <summary>
            /// The Network is forming
            /// </summary>
            [EnumDescription("Network Forming")]
            NetworkForming = 2,
            /// <summary>
            /// Invalid
            /// </summary>
            [EnumDescription("Invalid")]
            Invalid = 3,
        }

        /// <summary>
        /// The cause for Diagnostic System event
        /// </summary>
        public enum ZigBeeDiagnosticEvent : byte
        {
            /// <summary>
            /// Channel Change
            /// </summary>
            [EnumDescription("Channel Change")]
            ChannelChange = 0x00,
            /// <summary>
            /// Pan ID Change
            /// </summary>
            [EnumDescription("Pan ID Change")]
            PanIDChange = 0x01,
            /// <summary>
            /// ZigBee halted due to Reset Limiting
            /// </summary>
            [EnumDescription("ZigBee halted due to Reset Limiting")]
            ZigBeeHaltedResetLimiting = 0x02,
            /// <summary>
            /// Config Change
            /// </summary>
            [EnumDescription("Config Change")]
            ConfigChange = 0x03,
            /// <summary>
            /// Gas Module told to leave
            /// </summary>
            [EnumDescription("Gas Module told to leave")]
            GasModuleLeaving = 0x04,
            /// <summary>
            /// Management Leave Requested
            /// </summary>
            [EnumDescription("Management Leave Requested")]
            ManagementLeaveRequested = 0x05,
            /// <summary>
            /// Meter has been cold started
            /// </summary>
            [EnumDescription("Meter has been cold started")]
            MeterColdStart = 0x06,
            /// <summary>
            /// Config file validation failed
            /// </summary>
            [EnumDescription("Config file validation failed")]
            ConfigValidationFailed = 0x07,
            /// <summary>
            /// Network Key Generation Method
            /// </summary>
            [EnumDescription("Network Key Generation Method")]
            NetworkKeyGenerationMethod = 0x08,
            /// <summary>
            /// Network Form Config Error
            /// </summary>
            [EnumDescription("Network Form Config Error")]
            NetworkFormConfigError = 0x09,
            /// <summary>
            /// Invalid
            /// </summary>
            [EnumDescription("Invalid")]
            Invalid = 0xFF,
        }

        /// <summary>
        /// The cause for Network Change event
        /// </summary>
        public enum ZigBeeResetCause : byte
        {
            /// <summary>
            /// Network Restart
            /// </summary>
            [EnumDescription("Network Restart")]
            NetworkRestart = 0,
            /// <summary>
            /// Shut Down Network for 10 Minutes
            /// </summary>
            [EnumDescription("Network Halted for 10 minutes")]
            NetworkHalted = 1,
            /// <summary>
            /// Disable HAN via table 145
            /// </summary>
            [EnumDescription("Disable HAN via Table")]
            HANDisableViaTable145 = 2,
            /// <summary>
            /// Enable HAN via table 145
            /// </summary>
            [EnumDescription("Enable HAN via Table")]
            HANEnableViaTable145 = 3,
            /// <summary>
            /// Disable HAN via Procedure
            /// </summary>
            [EnumDescription("Disable HAN via Procedure")]
            HANDisableViaProcedure = 4,
            /// <summary>
            /// Enable HAN via Procedure
            /// </summary>
            [EnumDescription("Enable HAN via Procedure")]
            HANEnableViaProcedure = 5,
            /// <summary>
            /// Network Change due to Cold Start
            /// </summary>
            [EnumDescription("Network Change due to Cold Start")]
            ColdStart = 6,
            /// <summary>
            /// Network Change due to Firmware Download
            /// </summary>
            [EnumDescription("Network Change due to Firmware Download")]
            FirmwareDownload = 7,
            /// <summary>
            /// Network Change due to Power Cycle
            /// </summary>
            [EnumDescription("Network Change due to Power Cycle")]
            PowerCycle = 8,
            /// <summary>
            /// Network Restart due to Security Update
            /// </summary>
            [EnumDescription("Network Restart due to Security Update")]
            SecurityUpdated = 9,
            /// <summary>
            /// HAN Reset Non-routine
            /// </summary>
            [EnumDescription("HAN Reset Non-routine")]
            HANResetException = 10,
            /// <summary>
            /// Periodic HAN Reset
            /// </summary>
            [EnumDescription("Periodic HAN Reset")]
            HANResetRoutine = 11,
            /// <summary>
            /// None
            /// </summary>
            [EnumDescription("None")]
            None = 0xFF
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Translates the argument data to readable text
        /// </summary>
        /// <returns>The argument data as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        //  05/07/13 MSC 2.80.26 TQ6699 Added Support for the ZigBee Reset Reason Codes
        //
        public override string TranslateArgumentData()
        {
            return "Previous State: " + EnumDescriptionRetriever.RetrieveDescription(m_PreviousState)
                + " New State: " + EnumDescriptionRetriever.RetrieveDescription(m_NewState)
                + ZigBeeResetReason(m_ZigBeeReset);
        }

        /// <summary>
        /// Translates the ZigBee Reset Reason Code
        /// </summary>
        /// <param name="Reason">Reason Code to be translated</param>
        /// <returns>The translated ZigBee Reset data as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/13 MSC 2.80.26 TQ6699 Created
        //  06/19/14 AF  4.00.31 No WR  We were using the wrong enum for network change event
        //
        public string ZigBeeResetReason(ZigBeeResetCause Reason)
        {
            if (Reason != ZigBeeResetCause.None)
            {
                return " Reset Reason: " + EnumDescriptionRetriever.RetrieveDescription(Reason);
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get the previous state of the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        
        public NetworkStatus PreviousState
        {
            get
            {
                return m_PreviousState;
            }
            internal set
            {
                m_PreviousState = value;
            }
        }

        /// <summary>
        /// Gets the new state of the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        
        public NetworkStatus NewState
        {
            get
            {
                return m_NewState;
            }
            set
            {
                m_NewState = value;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        //  05/07/13 MSC 2.80.26 TQ6699 Added Support for ZigBee Reset
        //  06/19/14 AF  4.00.31 No WR  We were using the wrong enum for network change event
        //
        public MeterNetworkChangedHANLogEvent() :base ((ushort)DownstreamHANLogEventID.MeterNetworkChange)
        {
            m_PreviousState = NetworkStatus.Invalid;
            m_NewState = NetworkStatus.Invalid;
            m_ZigBeeReset = ZigBeeResetCause.None;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Parses the argument data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/11 RCG 2.51.32        Created
        //  05/07/13 MSC 2.80.26 TQ6699 Support for ZigBee Reset Added
        //
        protected override void ParseArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            try
            {
                m_PreviousState = (NetworkStatus)ArgumentReader.ReadByte();
                m_NewState = (NetworkStatus)ArgumentReader.ReadByte();
                m_ZigBeeReset = (ZigBeeResetCause)ArgumentReader.ReadByte();
            }
            catch (Exception)
            {
                // If we get an exception we should just go with what we have
            }
        }

        #endregion

        #region Member Variables

        private NetworkStatus m_PreviousState;
        private NetworkStatus m_NewState;
        private ZigBeeResetCause m_ZigBeeReset;

        #endregion
    }

    /// <summary>
    /// RIB Meter Event HAN Log Event
    /// </summary>
    public class RIBMeterEventHANLogEvent : DownstreamHANLogEvent
    {
        #region Definitions

        /// <summary>
        /// Event types for the RIB Meter HAN Event
        /// </summary>
        public enum RIBMeterEventID : byte
        {
            /// <summary>New Billing Period</summary>
            [EnumDescription("New Billing Period")]
            NewBillingPeriod = 0x00,
            /// <summary>New Block Period</summary>
            [EnumDescription("New Block Period")]
            NewBlockPeriod = 0x01,
            /// <summary>Block Threshold Crossed</summary>
            [EnumDescription("New Block Price in Effect")]
            NewBlockPriceInEffect = 0x02,
            /// <summary>End of Active Block Price Schedule</summary>
            [EnumDescription("End of Active Block Price Schedule")]
            EndOfActiveBlockPriceSchedule = 0x03,
            /// <summary>Invalid</summary>
            [EnumDescription("Invalid")]
            Invalid = 0xFF,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Translates the argument data to readable text
        /// </summary>
        /// <returns>The argument data as a string</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/23/12 RCG 2.60.25        Created

        public override string TranslateArgumentData()
        {
            return EnumDescriptionRetriever.RetrieveDescription(m_SubEventType) + " " + m_SubEventArgumentString;
        }

        /// <summary>
        /// Determines whether or not the RIB Meter Events are equal
        /// </summary>
        /// <param name="other">The event to compare against</param>
        /// <returns>True if the events are of the same type. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/23/12 RCG 2.60.25        Created

        public override bool Equals(DownstreamHANLogEvent other)
        {
            bool IsEqual = base.Equals(other);

            if (IsEqual)
            {
                // At this point we know both events should be the same type (RIB Meter Event) now we need to make sure the sub event type is the same
                RIBMeterEventHANLogEvent OtherRIBMeterEvent = other as RIBMeterEventHANLogEvent;

                // There are some cases where we are looking for just the event type such as the Event Log Summary Lists. In those cases
                // we don't care what sub type the event is so lets only check the sub event type if Sub Event type was specifically set
                // using the property.
                if (m_SpecificSubEventSpecifiedForComparison || OtherRIBMeterEvent.m_SpecificSubEventSpecifiedForComparison)
                {
                    if (OtherRIBMeterEvent != null)
                    {
                        IsEqual = this.SubEventType == OtherRIBMeterEvent.SubEventType;
                    }
                    else
                    {
                        // This shouldn't happen but just in case...
                        IsEqual = false;
                    }
                }
            }

            return IsEqual;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/23/12 RCG 2.60.25        Created

        public RIBMeterEventHANLogEvent() : base((ushort)DownstreamHANLogEventID.RIBMeterEvent)
        {
            m_SubEventType = RIBMeterEventID.Invalid;
            m_SubEventArgumentString = "";
            m_SpecificSubEventSpecifiedForComparison = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the RIM Meter Event Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/23/12 RCG 2.60.25        Created
        
        public RIBMeterEventID SubEventType
        {
            get
            {
                return m_SubEventType;
            }
            set
            {
                m_SubEventType = value;
                m_SpecificSubEventSpecifiedForComparison = true;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Parses the argument data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/23/12 RCG 2.60.25        Created

        protected override void ParseArgumentData()
        {
            MemoryStream ArgumentStream = new MemoryStream(m_Argument);
            PSEMBinaryReader ArgumentReader = new PSEMBinaryReader(ArgumentStream);

            try
            {
                m_SubEventType = RIBMeterEventID.Invalid;
                m_SubEventArgumentString = "";

                byte EventType = ArgumentReader.ReadByte();

                if (true == Enum.IsDefined(typeof(RIBMeterEventID), EventType))
                {
                    m_SubEventType = (RIBMeterEventID)EventType;

                    switch (m_SubEventType)
                    {
                        case RIBMeterEventID.NewBillingPeriod:
                        case RIBMeterEventID.NewBlockPeriod:
                        {
                            uint Seconds = ArgumentReader.ReadUInt32();

                            // A value of all 0xFF means the start time is invalid
                            if (Seconds != uint.MaxValue)
                            {
                                DateTime StartTime = UTC_REFERENCE_TIME.AddSeconds(Seconds);

                                m_SubEventArgumentString = " Start Time: " + StartTime.ToString("G", CultureInfo.CurrentCulture);
                            }

                            break;
                        }
                        case RIBMeterEventID.NewBlockPriceInEffect:
                        {
                            byte OldBlockNumber = ArgumentReader.ReadByte();
                            byte NewBlockNumber = ArgumentReader.ReadByte();
                            m_SubEventArgumentString = "Old Block Number: " + OldBlockNumber.ToString(CultureInfo.InvariantCulture)
                                + " New Block Number: " + NewBlockNumber.ToString(CultureInfo.InvariantCulture);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // If we get an exception we should just go with what we have
            }

            m_SpecificSubEventSpecifiedForComparison = false;
        }

        #endregion

        #region Member Variables

        private RIBMeterEventID m_SubEventType;
        private string m_SubEventArgumentString;
        private bool m_SpecificSubEventSpecifiedForComparison;

        #endregion
    }

    /// <summary>
    /// MFG Table 191 (2239) - Actual HAN Log Table
    /// </summary>
    public class OpenWayMFGTable2239 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 11;

        #endregion

        #region Definitions

        [Flags]
        private enum HANLogFlags : byte
        {
            LogEventNumber = 0x01,
            LogDateAndTime = 0x02,
            LogSequenceNumber = 0x04,
            UpstreamLogInhibitOverflow = 0x08,
            DownstreamLogInhibitOverflow = 0x10,
            SignatureSupported = 0x20,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public OpenWayMFGTable2239(CPSEM psem)
            : base(psem, 2239, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public OpenWayMFGTable2239(PSEMBinaryReader binaryReader)
            : base(2239, TABLE_SIZE)
        {
            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2239.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the HAN Event log is logging the event number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public bool IsLoggingEventNumber
        {
            get
            {
                ReadUnloadedTable();

                return (m_HANLogFlags & HANLogFlags.LogEventNumber) == HANLogFlags.LogEventNumber;
            }
        }

        /// <summary>
        /// Gets whether or not the HAN Event log is logging the date and time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public bool IsLoggingDateAndTime
        {
            get
            {
                ReadUnloadedTable();

                return (m_HANLogFlags & HANLogFlags.LogDateAndTime) == HANLogFlags.LogDateAndTime;
            }
        }

        /// <summary>
        /// Gets whether or not the HAN Event log is logging the sequence number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public bool IsLoggingSequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return (m_HANLogFlags & HANLogFlags.LogSequenceNumber) == HANLogFlags.LogSequenceNumber;
            }
        }

        /// <summary>
        /// Gets whether or not the Upstream log Inhibits Overflow
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public bool IsUpstreamLogInhibitingOverflow
        {
            get
            {
                ReadUnloadedTable();

                return (m_HANLogFlags & HANLogFlags.UpstreamLogInhibitOverflow) == HANLogFlags.UpstreamLogInhibitOverflow;
            }
        }

        /// <summary>
        /// Gets whether or not the Downstream Log inhibits overflow
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public bool IsDownstreamLogInhibitingOverflow
        {
            get
            {
                ReadUnloadedTable();

                return (m_HANLogFlags & HANLogFlags.DownstreamLogInhibitOverflow) == HANLogFlags.DownstreamLogInhibitOverflow;
            }
        }

        /// <summary>
        /// Gets whether or not the signature is supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public bool IsSignatureSupported
        {
            get
            {
                ReadUnloadedTable();

                return (m_HANLogFlags & HANLogFlags.SignatureSupported) == HANLogFlags.SignatureSupported;
            }
        }

        /// <summary>
        /// Gets the number of Upstream Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public byte NumberOfUpstreamEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_byNumUpstreamEvents;
            }
        }

        /// <summary>
        /// Gets the data length of an Upstream Event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public byte UpstreamDataLength
        {
            get
            {
                ReadUnloadedTable();

                return m_byUpstreamDataLength;
            }
        }

        /// <summary>
        /// Gets the number of entries in the Upstream Log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort NumberOfUpstreamLogEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_usNumUpstreamEntries;
            }
        }

        /// <summary>
        /// Gets the number of Downstream events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public byte NumberOfDownstreamEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_byNumDownstreamEvents;
            }
        }

        /// <summary>
        /// Gets the data length of a downstream event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public byte DownstreamDataLength
        {
            get
            {
                ReadUnloadedTable();

                return m_byDownStreamDataLength;
            }
        }

        /// <summary>
        /// Gets the number of entries in the Downstream Log
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort NumberOfDownstreamLogEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_usNumDownstreamEntries;
            }
        }

        /// <summary>
        /// Gets the number of Program Tables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public byte NumberOfProgramTables
        {
            get
            {
                ReadUnloadedTable();

                return m_byNumProgramTables;
            }
        }

        /// <summary>
        /// Gets the length of the signature.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public byte SignatureLength
        {
            get
            {
                ReadUnloadedTable();

                return m_bySignatureLength;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private void ParseData()
        {
            m_HANLogFlags = (HANLogFlags)m_Reader.ReadByte();
            m_byNumUpstreamEvents = m_Reader.ReadByte();
            m_byUpstreamDataLength = m_Reader.ReadByte();
            m_usNumUpstreamEntries = m_Reader.ReadUInt16();
            m_byNumDownstreamEvents = m_Reader.ReadByte();
            m_byDownStreamDataLength = m_Reader.ReadByte();
            m_usNumDownstreamEntries = m_Reader.ReadUInt16();
            m_byNumProgramTables = m_Reader.ReadByte();
            m_bySignatureLength = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private HANLogFlags m_HANLogFlags;
        private byte m_byNumUpstreamEvents;
        private byte m_byUpstreamDataLength;
        private ushort m_usNumUpstreamEntries;
        private byte m_byNumDownstreamEvents;
        private byte m_byDownStreamDataLength;
        private ushort m_usNumDownstreamEntries;
        private byte m_byNumProgramTables;
        private byte m_bySignatureLength;

        #endregion
    }

    /// <summary>
    /// MFG Table 192 (2240) - HAN Events Identification Table
    /// </summary>
    public class OpenWayMFGTable2240 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2239">The Actual HAN Log table for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public OpenWayMFGTable2240(CPSEM psem, OpenWayMFGTable2239 table2239)
            : base(psem, 2240, GetTableSize(table2239))
        {
            m_Table2239 = table2239;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        /// <param name="table2239">The Actual HAN Log table for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public OpenWayMFGTable2240(PSEMBinaryReader binaryReader, OpenWayMFGTable2239 table2239)
            : base(2240, GetTableSize(table2239))
        {
            m_Table2239 = table2239;

            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2240.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of supported Upstream Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ReadOnlyCollection<UpstreamHANLogEvent> SupportedUpstreamEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_SupportedUpstreamEvents.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the list of supported Downstream Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ReadOnlyCollection<DownstreamHANLogEvent> SupportedDownstreamEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_SupportedDownstreamEvents.AsReadOnly();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="table2239">The Actual HAN Log Table for the current meter.</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private static uint GetTableSize(OpenWayMFGTable2239 table2239)
        {
            uint TableSize = 0;

            if (table2239 != null)
            {
                TableSize = (uint)(table2239.NumberOfUpstreamEvents + table2239.NumberOfDownstreamEvents);
            }

            return TableSize;
        }

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private void ParseData()
        {
            m_UpstreamEvents = null;
            m_DownstreamEvents = null;

            m_SupportedUpstreamEvents = new List<UpstreamHANLogEvent>();
            m_SupportedDownstreamEvents = new List<DownstreamHANLogEvent>();

            if (m_Table2239 != null)
            {
                if (m_Table2239.NumberOfDownstreamEvents > 0)
                {
                    m_DownstreamEvents = m_Reader.ReadBytes(m_Table2239.NumberOfDownstreamEvents);

                    for (ushort EventNumber = 0; EventNumber < m_DownstreamEvents.Length * 8; EventNumber++)
                    {
                        int Index = EventNumber / 8;
                        byte EventMask = (byte)(1 << (EventNumber % 8));

                        // Check to see if the bit is set.
                        if ((m_DownstreamEvents[Index] & EventMask) == EventMask)
                        {
                            // Make sure we add in the Manufacturing bit and HAN2 bit (2048 + 256 = 2304)
                            m_SupportedDownstreamEvents.Add(DownstreamHANLogEvent.Create((ushort)(EventNumber + 2304)));
                        }
                    }
                }

                if (m_Table2239.NumberOfUpstreamEvents > 0)
                {
                    m_UpstreamEvents = m_Reader.ReadBytes(m_Table2239.NumberOfUpstreamEvents);

                    for (ushort EventNumber = 0; EventNumber < m_UpstreamEvents.Length * 8; EventNumber++)
                    {
                        int Index = EventNumber / 8;
                        byte EventMask = (byte)(1 << (EventNumber % 8));

                        // Check to see if the bit is set.
                        if ((m_UpstreamEvents[Index] & EventMask) == EventMask)
                        {
                            // Upstream events start at 128 so we need to make sure we add 128 to the value
                            // and we need to add the MFG bit and HAN2 bit (2048 + 256 = 2304)
                            m_SupportedUpstreamEvents.Add(UpstreamHANLogEvent.Create((ushort)(EventNumber + 128 + 2304)));
                        }
                    }
                }
            }
        }

        #endregion

        #region Member Variables

        private OpenWayMFGTable2239 m_Table2239;

        private byte[] m_UpstreamEvents;
        private byte[] m_DownstreamEvents;

        private List<UpstreamHANLogEvent> m_SupportedUpstreamEvents;
        private List<DownstreamHANLogEvent> m_SupportedDownstreamEvents;

        #endregion
    }

    /// <summary>
    /// MFG Table 193 (2241) - HAN Events Control Table
    /// </summary>
    public class OpenWayMFGTable2241 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2239">The Table 2239 object for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public OpenWayMFGTable2241(CPSEM psem, OpenWayMFGTable2239 table2239)
            : base(psem, 2241, GetTableSize(table2239))
        {
            m_Table2239 = table2239;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        /// <param name="table2239">The Table 2239 object for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public OpenWayMFGTable2241(PSEMBinaryReader binaryReader, OpenWayMFGTable2239 table2239)
            : base(2241, GetTableSize(table2239))
        {
            m_Table2239 = table2239;

            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2241.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of supported Upstream Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ReadOnlyCollection<UpstreamHANLogEvent> EnabledUpstreamEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_EnabledUpstreamEvents.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the list of supported Downstream Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ReadOnlyCollection<DownstreamHANLogEvent> EnabledDownstreamEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_EnabledDownstreamEvents.AsReadOnly();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="table2239">The Actual HAN Log Table for the current meter.</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private static uint GetTableSize(OpenWayMFGTable2239 table2239)
        {
            uint TableSize = 0;

            if (table2239 != null)
            {
                TableSize = (uint)(table2239.NumberOfUpstreamEvents + table2239.NumberOfDownstreamEvents);
            }

            return TableSize;
        }

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private void ParseData()
        {
            m_UpstreamEvents = null;
            m_DownstreamEvents = null;

            m_EnabledUpstreamEvents = new List<UpstreamHANLogEvent>();
            m_EnabledDownstreamEvents = new List<DownstreamHANLogEvent>();

            if (m_Table2239 != null)
            {
                if (m_Table2239.NumberOfDownstreamEvents > 0)
                {
                    m_DownstreamEvents = m_Reader.ReadBytes(m_Table2239.NumberOfDownstreamEvents);

                    for (ushort EventNumber = 0; EventNumber < m_DownstreamEvents.Length * 8; EventNumber++)
                    {
                        int Index = EventNumber / 8;
                        byte EventMask = (byte)(1 << (EventNumber % 8));

                        // Check to see if the bit is set.
                        if ((m_DownstreamEvents[Index] & EventMask) == EventMask)
                        {
                            // Make sure we add in the Manufacturing bit and HAN2 bit (2048 + 256 = 2304)
                            m_EnabledDownstreamEvents.Add(DownstreamHANLogEvent.Create((ushort)(EventNumber + 2304)));
                        }
                    }
                }

                if (m_Table2239.NumberOfUpstreamEvents > 0)
                {
                    m_UpstreamEvents = m_Reader.ReadBytes(m_Table2239.NumberOfUpstreamEvents);

                    for (ushort EventNumber = 0; EventNumber < m_UpstreamEvents.Length * 8; EventNumber++)
                    {
                        int Index = EventNumber / 8;
                        byte EventMask = (byte)(1 << (EventNumber % 8));

                        // Check to see if the bit is set.
                        if ((m_UpstreamEvents[Index] & EventMask) == EventMask)
                        {
                            // Upstream events start at 128 so we need to make sure we add 128 to the value
                            // and we need to make sure that we add in the Manufacturing bit and HAN2 bit (2048 + 256 = 2304)
                            m_EnabledUpstreamEvents.Add(UpstreamHANLogEvent.Create((ushort)(EventNumber + 128 + 2304)));
                        }
                    }
                }
            }
        }

        #endregion

        #region Member Variables

        private OpenWayMFGTable2239 m_Table2239;

        private byte[] m_UpstreamEvents;
        private byte[] m_DownstreamEvents;

        private List<UpstreamHANLogEvent> m_EnabledUpstreamEvents;
        private List<DownstreamHANLogEvent> m_EnabledDownstreamEvents;

        #endregion
    }

    /// <summary>
    /// MFG Table 194 (2242) - HAN Upstream Log Table
    /// </summary>
    public class OpenWayMFGTable2242 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const uint PACKET_OVERHEAD_SIZE = 8;
        private const uint PACKETS_PER_READ = 254;
        private const ushort HEADER_LENGTH = 11;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2239">The Table 2239 object for the current meter</param>
        /// <param name="table0">That Table 0 object for the current meter</param>
        /// <param name="maxOffsetReadSize">The maximum amount of data that can be read during an offset read.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        //  05/06/14 jrf 3.50.91 WR 503747 Allowing constructor to adjust the maximum offset read size.
        public OpenWayMFGTable2242(CPSEM psem, OpenWayMFGTable2239 table2239, CTable00 table0, ushort maxOffsetReadSize = ushort.MaxValue)
            : base(psem, 2242, GetTableSize(table2239, table0), TABLE_TIMEOUT)
        {
            m_Table2239 = table2239;
            m_Table0 = table0;
            m_MaxOffsetReadSize = maxOffsetReadSize;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        /// <param name="table2239">The Table 2239 object for the current meter</param>
        /// <param name="table0">That Table 0 object for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public OpenWayMFGTable2242(PSEMBinaryReader binaryReader, OpenWayMFGTable2239 table2239, CTable00 table0)
            : base(2242, GetTableSize(table2239, table0))
        {
            m_Table2239 = table2239;
            m_Table0 = table0;

            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseHeaderData();
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23           Created
        //  11/14/12 RCG 2.70.36           Updating so that this table will be read properly 
        //                                 over ZigBee which now uses 64 byte packets.
        //  05/06/14 jrf 3.50.91 WR 503747 Using member variable for determining the maximum offset read size.
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2242.Read");

            uint EntrySize = UpstreamHANLogEvent.GetEntrySize(m_Table2239, m_Table0.LTIMESize);
            uint MaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;
            uint MaxOffsetReadEntries = 0;
            int CurrentOffset = 0;
            int EntriesRead = 0;

            // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
            if (MaxOffsetReadBytes > m_MaxOffsetReadSize)
            {
                MaxOffsetReadBytes = m_MaxOffsetReadSize;
            }

            MaxOffsetReadEntries = MaxOffsetReadBytes / EntrySize;

            // Read the header portion of the table first to see how many entries need to be read
            PSEMResponse Result = base.Read(0, HEADER_LENGTH);

            if (Result == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;
                ParseHeaderData();

                CurrentOffset = HEADER_LENGTH;

                // Read the entries
                while (EntriesRead < m_NumUnreadEntries && Result == PSEMResponse.Ok)
                {
                    if (MaxOffsetReadEntries < (m_NumUnreadEntries - EntriesRead))
                    {
                        // We have more entries than can be read in one offset read
                        Result = base.Read(CurrentOffset, (ushort)(MaxOffsetReadEntries * EntrySize));
                        CurrentOffset += (int)(MaxOffsetReadEntries * EntrySize);
                        EntriesRead += (int)MaxOffsetReadEntries;
                    }
                    else
                    {
                        // We can read the rest of the entries in one read
                        Result = base.Read(CurrentOffset, (ushort)((m_NumUnreadEntries - EntriesRead) * EntrySize));
                        CurrentOffset += (int)(MaxOffsetReadEntries * EntrySize);
                        EntriesRead += (int)MaxOffsetReadEntries;
                    }
                }

                if (PSEMResponse.Ok == Result)
                {
                    m_DataStream.Position = HEADER_LENGTH;
                    ParseData();

                    m_TableState = TableState.Loaded;
                    m_ExpirationTimer.Change(m_iTimeout, 0);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of valid log entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort NumberOfValidEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_NumValidEntries;
            }
        }

        /// <summary>
        /// Gets the index of the last event entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort LastEntryIndex
        {
            get
            {
                ReadUnloadedTable();

                return m_LastEntryIndex;
            }
        }

        /// <summary>
        /// Gets the sequence number of the last event entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/05/12 RCG 2.60.00        Created

        public uint LastSequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return m_LastEntrySequenceNumber;
            }
        }

        /// <summary>
        /// Gets the list of events 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ReadOnlyCollection<UpstreamHANLogEvent> Events
        {
            get
            {
                ReadUnloadedTable();

                return m_Events.AsReadOnly();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="table2239">The Actual HAN Log Table for the current meter.</param>
        /// <param name="table0">That Table 0 object for the current meter</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private static uint GetTableSize(OpenWayMFGTable2239 table2239, CTable00 table0)
        {
            uint TableSize = HEADER_LENGTH;

            if (table2239 != null)
            {
                TableSize += table2239.NumberOfUpstreamLogEntries * UpstreamHANLogEvent.GetEntrySize(table2239, table0.LTIMESize);
            }

            return TableSize;
        }

        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/14/12 RCG 2.70.36        Created

        private void ParseHeaderData()
        {
            m_LogFlags = m_Reader.ReadByte();
            m_NumValidEntries = m_Reader.ReadUInt16();
            m_LastEntryIndex = m_Reader.ReadUInt16();
            m_LastEntrySequenceNumber = m_Reader.ReadUInt32();
            m_NumUnreadEntries = m_Reader.ReadUInt16();
        }

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private void ParseData()
        {
            m_Events = new List<UpstreamHANLogEvent>();

            for (int Entry = 0; Entry < m_NumValidEntries; Entry++)
            {
                DateTime TimeOccurred = DateTime.MinValue;
                ushort EventNumber = 0;
                ushort SequenceNumber = 0;
                ushort UserID;
                ushort EventID;
                uint HANEventID;
                ulong MACAddress;
                byte[] ArgumentData;

                UpstreamHANLogEvent CurrentEntry = null;

                if (m_Table2239.IsLoggingDateAndTime)
                {
                    TimeOccurred = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
                }

                if (m_Table2239.IsLoggingEventNumber)
                {
                    EventNumber = m_Reader.ReadUInt16();
                }

                if (m_Table2239.IsLoggingSequenceNumber)
                {
                    SequenceNumber = m_Reader.ReadUInt16();
                }

                UserID = m_Reader.ReadUInt16();
                EventID = m_Reader.ReadUInt16();
                HANEventID = m_Reader.ReadUInt32();
                MACAddress = m_Reader.ReadUInt64();
                ArgumentData = m_Reader.ReadBytes(m_Table2239.UpstreamDataLength);

                CurrentEntry = UpstreamHANLogEvent.Create(EventID);

                CurrentEntry.TimeFormat = (PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat;
                CurrentEntry.TimeOccurred = TimeOccurred;
                CurrentEntry.EventNumber = EventNumber;
                CurrentEntry.SequenceNumber = SequenceNumber;
                CurrentEntry.UserID = UserID;
                CurrentEntry.HANEventID = HANEventID;
                CurrentEntry.MACAddress = MACAddress;
                CurrentEntry.Argument = ArgumentData;

                m_Events.Add(CurrentEntry);
            }
        }

        #endregion

        #region Member Variables

        private OpenWayMFGTable2239 m_Table2239;
        private CTable00 m_Table0;

        private byte m_LogFlags;
        private ushort m_NumValidEntries;
        private ushort m_LastEntryIndex;
        private uint m_LastEntrySequenceNumber;
        private ushort m_NumUnreadEntries;
        private ushort m_MaxOffsetReadSize;

        private List<UpstreamHANLogEvent> m_Events;

        #endregion
    }

    /// <summary>
    /// MFG Table 195 (2243) - HAN Downstream Log Table
    /// </summary>
    public class OpenWayMFGTable2243 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const uint PACKET_OVERHEAD_SIZE = 8;
        private const uint PACKETS_PER_READ = 254;
        private const ushort HEADER_LENGTH = 11;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2239">The Table 2239 object for the current meter</param>
        /// <param name="table0">The Table 0 object for the current meter</param>
        /// <param name="maxOffsetReadSize">The maximum amount of data that can be read during an offset read.</param>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23           Created
        //  05/06/14 jrf 3.50.91 WR 503747 Allowing constructor to adjust the maximum offset read size.
        public OpenWayMFGTable2243(CPSEM psem, OpenWayMFGTable2239 table2239, CTable00 table0, ushort maxOffsetReadSize = ushort.MaxValue)
            : base(psem, 2243, GetTableSize(table2239, table0), TABLE_TIMEOUT)
        {
            m_Table2239 = table2239;
            m_Table0 = table0;
            m_MaxOffsetReadSize = maxOffsetReadSize;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        /// <param name="table2239">The Table 2239 object for the current meter</param>
        /// <param name="table0">The Table 0 object for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created
        
        public OpenWayMFGTable2243(PSEMBinaryReader binaryReader, OpenWayMFGTable2239 table2239, CTable00 table0)
            : base(2243, GetTableSize(table2239, table0))
        {
            m_Table2239 = table2239;
            m_Table0 = table0;

            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseHeaderData();
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23           Created
        //  11/14/12 RCG 2.70.36           Updating so that this table will be read properly 
        //                                 over ZigBee which now uses 64 byte packets.
        //  05/06/14 jrf 3.50.91 WR 503747 Using member variable for determining the maximum offset read size.
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2243.Read");

            uint EntrySize = DownstreamHANLogEvent.GetEntrySize(m_Table2239, m_Table0.LTIMESize);
            uint MaxOffsetReadBytes = (m_PSEM.PacketSize - PACKET_OVERHEAD_SIZE) * PACKETS_PER_READ;
            uint MaxOffsetReadEntries = 0;
            int CurrentOffset = 0;
            int EntriesRead = 0;

            // Because offset reads can be only up to ushort.MaxValue, limit to that if needed.
            if (MaxOffsetReadBytes > m_MaxOffsetReadSize)
            {
                MaxOffsetReadBytes = m_MaxOffsetReadSize;
            }

            MaxOffsetReadEntries = MaxOffsetReadBytes / EntrySize;

            // Read the header portion of the table first to see how many entries need to be read
            PSEMResponse Result = base.Read(0, HEADER_LENGTH);

            if (Result == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;
                ParseHeaderData();

                CurrentOffset = HEADER_LENGTH;

                // Read the entries
                while (EntriesRead < m_NumUnreadEntries && Result == PSEMResponse.Ok)
                {
                    if (MaxOffsetReadEntries < (m_NumUnreadEntries - EntriesRead))
                    {
                        // We have more entries than can be read in one offset read
                        Result = base.Read(CurrentOffset, (ushort)(MaxOffsetReadEntries * EntrySize));
                        CurrentOffset += (int)(MaxOffsetReadEntries * EntrySize);
                        EntriesRead += (int)MaxOffsetReadEntries;
                    }
                    else
                    {
                        // We can read the rest of the entries in one read
                        Result = base.Read(CurrentOffset, (ushort)((m_NumUnreadEntries - EntriesRead) * EntrySize));
                        CurrentOffset += (int)(MaxOffsetReadEntries * EntrySize);
                        EntriesRead += (int)MaxOffsetReadEntries;
                    }
                }

                if (PSEMResponse.Ok == Result)
                {
                    m_DataStream.Position = HEADER_LENGTH;
                    ParseData();

                    m_TableState = TableState.Loaded;
                    m_ExpirationTimer.Change(m_iTimeout, 0);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of valid log entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort NumberOfValidEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_NumValidEntries;
            }
        }

        /// <summary>
        /// Gets the index of the last event entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ushort LastEntryIndex
        {
            get
            {
                ReadUnloadedTable();

                return m_LastEntryIndex;
            }
        }

        /// <summary>
        /// Gets the sequence number of the last event entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/05/12 RCG 2.60.00        Created

        public uint LastSequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return m_LastEntrySequenceNumber;
            }
        }

        /// <summary>
        /// Gets the list of events 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        public ReadOnlyCollection<DownstreamHANLogEvent> Events
        {
            get
            {
                ReadUnloadedTable();

                return m_Events.AsReadOnly();
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="table2239">The Actual HAN Log Table for the current meter.</param>
        /// <param name="table0">The Table 0 object for the current meter</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private static uint GetTableSize(OpenWayMFGTable2239 table2239, CTable00 table0)
        {
            uint TableSize = 11;

            if (table2239 != null)
            {
                TableSize += table2239.NumberOfDownstreamLogEntries * DownstreamHANLogEvent.GetEntrySize(table2239, table0.LTIMESize);
            }

            return TableSize;
        }

        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/14/12 RCG 2.70.36        Created

        private void ParseHeaderData()
        {
            m_LogFlags = m_Reader.ReadByte();
            m_NumValidEntries = m_Reader.ReadUInt16();
            m_LastEntryIndex = m_Reader.ReadUInt16();
            m_LastEntrySequenceNumber = m_Reader.ReadUInt32();
            m_NumUnreadEntries = m_Reader.ReadUInt16();
        }

        /// <summary>
        /// Parses the data for the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/11 RCG 2.45.23        Created

        private void ParseData()
        {
            m_Events = new List<DownstreamHANLogEvent>();

            for (int Entry = 0; Entry < m_NumValidEntries; Entry++)
            {
                DateTime TimeOccurred = DateTime.MinValue;
                ushort EventNumber = 0;
                ushort SequenceNumber = 0;
                ushort UserID;
                ushort EventID;
                uint HANEventID;
                byte[] ArgumentData;

                DownstreamHANLogEvent CurrentEntry = null;

                if(m_Table2239.IsLoggingDateAndTime)
                {
                    TimeOccurred = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
                }

                if(m_Table2239.IsLoggingEventNumber)
                {
                    EventNumber = m_Reader.ReadUInt16();
                }

                if (m_Table2239.IsLoggingSequenceNumber)
                {
                    SequenceNumber = m_Reader.ReadUInt16();
                }

                UserID = m_Reader.ReadUInt16();
                EventID = m_Reader.ReadUInt16();
                HANEventID = m_Reader.ReadUInt32();
                ArgumentData = m_Reader.ReadBytes(m_Table2239.DownstreamDataLength);

                // Create the Event object
                CurrentEntry = DownstreamHANLogEvent.Create(EventID);

                CurrentEntry.TimeOccurred = TimeOccurred;
                CurrentEntry.EventNumber = EventNumber;
                CurrentEntry.SequenceNumber = SequenceNumber;
                CurrentEntry.UserID = UserID;
                CurrentEntry.HANEventID = HANEventID;
                CurrentEntry.Argument = ArgumentData;

                m_Events.Add(CurrentEntry);
            }
        }

        #endregion

        #region Member Variables

        private OpenWayMFGTable2239 m_Table2239;
        private CTable00 m_Table0;

        private byte m_LogFlags;
        private ushort m_NumValidEntries;
        private ushort m_LastEntryIndex;
        private uint m_LastEntrySequenceNumber;
        private ushort m_NumUnreadEntries;
        private ushort m_MaxOffsetReadSize;

        private List<DownstreamHANLogEvent> m_Events;
        #endregion
    }

    /// <summary>
    /// MFG Table 243 (2291) - HAN Event CE Log Table
    /// </summary>
    public class OpenWayMFGTable2291 : AnsiTable
    {

        #region Constants

        private const uint TABLE_SIZE = 770;
        private const int TABLE_TIMEOUT = 1000;

        private const int STATUS_RCD_1_OFFSET = 0;
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/13 AR  2.80.35 N/A    Created

        public OpenWayMFGTable2291(CPSEM psem)
            : base (psem, 2291, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/13 AR  2.80.35 392756 Created
        //
        public override PSEMResponse Read()
        {
            //WR 392756 - For Michigan FW, the value of the reset count is required to 
            //confirm whether or not a network reset has occured. For this we read the 
            //first byte out (offset read) of the table. Since, we only need the first
            //byte this table does not need to support full reads.
            throw new NotSupportedException("This table does not currently support full reads.");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of current entries in the table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/05/13 AR  2.80.35 N/A    Created
        //
        public ushort CurrentEntry
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_1_OFFSET, 2);

                if (PSEMResponse.Ok == Result)
                {
                    m_usCurrentEntry = m_Reader.ReadUInt16();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Current Entry."));
                }

                return m_usCurrentEntry;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_usCurrentEntry;

        #endregion

    }

    /// <summary>
    /// Class that represents a single HAN Event System Log record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/02/13 AR  2.80.45 411417 Created
    public class HANEventSystemLogRcd
    {

        #region Definitions

        /// <summary>
        /// Event Code Types
        /// </summary>
        public enum EventCodeType : ushort
        {
            /// <summary>
            /// Unknown Event Code Type
            /// </summary>
            [EnumDescription("Unknown")]
            Unknown = 0xFFFF,
            /// <summary>
            /// Routine Reset Performed Event
            /// </summary>
            [EnumDescription("Routine Reset Performed")]
            RoutineReset = 0x0315,
            /// <summary>
            /// Reset Exception Event
            /// </summary>
            [EnumDescription("Reset Exception")]
            ResetException = 0x0316,
            /// <summary>
            /// Form Dynamic Network Event
            /// </summary>
            [EnumDescription("Form Dynamic Network")]
            FormDynamicNetwork = 0x323,
            /// <summary>
            /// Network Form Fresh Start Event
            /// </summary>
            [EnumDescription("Network Form Frest Start")]
            NetworkFormFreshStart = 0x336,
            /// <summary>
            /// Network Restart After Firmware Download Event
            /// </summary>
            [EnumDescription("Network Restart After FWDL")]
            NetworkRestartAfterFWDL = 0x337,

            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public HANEventSystemLogRcd()
        {
            m_uiTimeInMs = 0;
            m_EventCode = EventCodeType.Unknown;
            m_uiParam16 = 0;
            m_uiParam32 = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Time in Ms
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public UInt32 TimeInMs
        {
            get
            {
                return m_uiTimeInMs;
            }
            internal set
            {
                m_uiTimeInMs = value;
            }
        }

        /// <summary>
        /// Gets or sets the Event Code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public EventCodeType EventCode
        {
            get
            {
                return m_EventCode;
            }
            internal set
            {
                m_EventCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the Param16
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public UInt16 Param16
        {
            get
            {
                return m_uiParam16;
            }
            internal set
            {
                m_uiParam16 = value;
            }
        }

        /// <summary>
        /// Gets or sets the Param32
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public UInt32 Param32
        {
            get
            {
                return m_uiParam32;
            }
            internal set
            {
                m_uiParam32 = value;
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_uiTimeInMs;
        private EventCodeType m_EventCode;
        private UInt16 m_uiParam16;
        private UInt32 m_uiParam32;
        
        #endregion
    }

    /// <summary>
    /// HAN Mfg Table 234 (2282) - Contains the HAN Event System Logs
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/02/13 AR  2.80.45 411417 Created
    public class CHANMfgTable2282 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 3074;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //
        public CHANMfgTable2282(CPSEM psem)
            : base(psem, 2282, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Full read of table 2282
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2282.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                m_uiCurrentEntry = m_Reader.ReadUInt16();
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current entry in the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //

        public UInt16 CurrentEntry
        {
            get
            {
                ReadUnloadedTable();

                return m_uiCurrentEntry;
            }
        }

        /// <summary>
        /// Gets the list of HAN Event System Logs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //
        public List<HANEventSystemLogRcd> HANEventSystemLogs
        {
            get
            {
                ReadUnloadedTable();

                return m_HANEventSystemLog;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //

        private void ParseData()
        {
            m_HANEventSystemLog = new List<HANEventSystemLogRcd>();

            for (int iIndex = 0; iIndex < 256; iIndex++)
            {
                HANEventSystemLogRcd NewHANEventSystemLog = new HANEventSystemLogRcd();

                NewHANEventSystemLog.TimeInMs = m_Reader.ReadUInt32();
                NewHANEventSystemLog.EventCode = (HANEventSystemLogRcd.EventCodeType)m_Reader.ReadUInt16();
                NewHANEventSystemLog.Param16 = m_Reader.ReadUInt16();
                NewHANEventSystemLog.Param32 = m_Reader.ReadUInt32();

                // An event code of 'Unknown Type (0xFFFF)' means value is not used.
                if (NewHANEventSystemLog.EventCode != HANEventSystemLogRcd.EventCodeType.Unknown)
                {
                    m_HANEventSystemLog.Add(NewHANEventSystemLog);
                }
            }
        }

        #endregion

        #region Members

        private UInt16 m_uiCurrentEntry;
        private List<HANEventSystemLogRcd> m_HANEventSystemLog;

        #endregion
    }
    // -------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Class that represents a single HAN Event System Log record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  10/04/13 MP                 Created

    public class HANEventErrorLogRcd
    {

        #region Definitions

        /// <summary>
        /// Event Code Types
        /// </summary>
        public enum EventCodeType : ushort
        {
            /// <summary>
            /// Unknown Event Code Type
            /// </summary>
            [EnumDescription("Unknown")]
            Unknown = 0xFFFF,
            /// <summary>
            /// Routine Reset Performed Event
            /// </summary>
            [EnumDescription("DRLC_Generic_Error - Invalid EventID")]
            DRLCGenericError = 0x0405,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created

        public HANEventErrorLogRcd()
        {
            m_uiTimeStamp = 0;
            m_EventCode = EventCodeType.Unknown;
            m_uiParam16 = 0;
            m_uiParam32 = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Time in Ms
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created

        public UInt32 TimeStamp
        {
            get
            {
                return m_uiTimeStamp;
            }
            internal set
            {
                m_uiTimeStamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the Event Code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created

        public EventCodeType EventCode
        {
            get
            {
                return m_EventCode;
            }
            internal set
            {
                m_EventCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the Param16
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created

        public UInt16 Param16
        {
            get
            {
                return m_uiParam16;
            }
            internal set
            {
                m_uiParam16 = value;
            }
        }

        /// <summary>
        /// Gets or sets the Param32
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created

        public UInt32 Param32
        {
            get
            {
                return m_uiParam32;
            }
            internal set
            {
                m_uiParam32 = value;
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_uiTimeStamp;
        private EventCodeType m_EventCode;
        private UInt16 m_uiParam16;
        private UInt32 m_uiParam32;

        #endregion
    }

    /// <summary>
    /// HAN Mfg Table 235 (2285) - Contains the HAN Event error Logs
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  10/04/13 MP                 Created

    public class CHANMfgTable2283 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 1538;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created
        //
        public CHANMfgTable2283(CPSEM psem)
            : base(psem, 2283, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Full read of table 2282
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2283.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                m_uiCurrentEntry = m_Reader.ReadUInt16();
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current entry in the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created
        //

        public UInt16 CurrentEntry
        {
            get
            {
                ReadUnloadedTable();

                return m_uiCurrentEntry;
            }
        }

        /// <summary>
        /// Gets the list of HAN Event System Logs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created
        //
        public List<HANEventErrorLogRcd> HANEventErrorLogs
        {
            get
            {
                ReadUnloadedTable();

                return m_HANEventErrorLog;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/04/13 MP                 Created
        //

        private void ParseData()
        {
            m_HANEventErrorLog = new List<HANEventErrorLogRcd>();

            for (int iIndex = 0; iIndex < 128; iIndex++)
            {
                HANEventErrorLogRcd NewHANEventErrorLog = new HANEventErrorLogRcd();

                NewHANEventErrorLog.TimeStamp = m_Reader.ReadUInt32();
                NewHANEventErrorLog.EventCode = (HANEventErrorLogRcd.EventCodeType)m_Reader.ReadUInt16();
                NewHANEventErrorLog.Param16 = m_Reader.ReadUInt16();
                NewHANEventErrorLog.Param32 = m_Reader.ReadUInt32();

                // An event code of 'Unknown Type (0xFFFF)' means value is not used.
                if (NewHANEventErrorLog.EventCode != HANEventErrorLogRcd.EventCodeType.Unknown)
                {
                    m_HANEventErrorLog.Add(NewHANEventErrorLog);
                }
            }
        }

        #endregion

        #region Members

        private UInt16 m_uiCurrentEntry;
        private List<HANEventErrorLogRcd> m_HANEventErrorLog;

        #endregion
    }

    // -------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Class that represents a single HAN Event App Log record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  08/23/13 MP                 Created
    public class HANEventAppLogRcd
    {

        #region Definitions

        /// <summary>
        /// Event Code Types
        /// </summary>
        public enum EventCodes: ushort
        {
            /// <summary>
            /// Unknown Event Code Type
            /// </summary>
            [EnumDescription("Unknown")]
            Unknown = 0xFFFF,
            /// <summary>
            /// DRLC cancel queued Event
            /// </summary>
            [EnumDescription("NEW_CANCEL_QUEUED_DRLC_EVENT")]
            Cancel_Queued_Event = 0x052B,
            /// <summary>
            /// DRLC cancel Event
            /// </summary>
            [EnumDescription("NEW_CANCEL_DRLC_EVENT")]
            Cancel_Event = 0x0503,
            /// <summary>
            /// DRLC Cancel all Event
            /// </summary>
            [EnumDescription("NEW_CANCEL_ALL_DRLC_EVENT")]
            Cancel_Queued_All = 0x052C,
            /// <summary>
            /// DRLC Cancel all queued Event
            /// </summary>
            [EnumDescription("NEW_CANCEL_ALL_DRLC_EVENT")]
            Cancel_All = 0x0504,
            /// <summary>
            /// DRLC Schedule new Event
            /// </summary>
            [EnumDescription("NEW_NORMAL_DRLC_EVENT")]
            Schedule_New = 0x0502,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/29/13 MP                 Created

        public HANEventAppLogRcd()
        {
            m_uiTimeInMs = 0;
            m_EventCode = EventCodes.Unknown;
            m_uiParam16 = 0;
            m_uiParam32 = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Time in Ms
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public UInt32 TimeInMs
        {
            get
            {
                return m_uiTimeInMs;
            }
            internal set
            {
                m_uiTimeInMs = value;
            }
        }

        /// <summary>
        /// Gets or sets the Event Code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public EventCodes EventCode
        {
            get
            {
                return m_EventCode;
            }
            internal set
            {
                m_EventCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the Param16
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public UInt16 Param16
        {
            get
            {
                return m_uiParam16;
            }
            internal set
            {
                m_uiParam16 = value;
            }
        }

        /// <summary>
        /// Gets or sets the Param32
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created

        public UInt32 Param32
        {
            get
            {
                return m_uiParam32;
            }
            internal set
            {
                m_uiParam32 = value;
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_uiTimeInMs;
        private EventCodes m_EventCode;
        private UInt16 m_uiParam16;
        private UInt32 m_uiParam32;

        #endregion
    }

    /// <summary>
    /// HAN Mfg Table 244 (2292) - Contains the HAN Event App Logs
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  08/23/13 MP                 Created
    public class CHANMfgTable2292 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 6146; // i hope.
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //
        public CHANMfgTable2292(CPSEM psem)
            : base(psem, 2292, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Full read of table 2282
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2292.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                m_uiCurrentEntry = m_Reader.ReadUInt16();
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current entry in the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //

        public UInt16 CurrentEntry
        {
            get
            {
                ReadUnloadedTable();

                return m_uiCurrentEntry;
            }
        }

        /// <summary>
        /// Gets the list of HAN Event System Logs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //
        public List<HANEventAppLogRcd> HANEventAppLogs
        {
            get
            {
                ReadUnloadedTable();

                return m_HANEventAppLog;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/02/13 AR  2.80.45 411417 Created
        //

        private void ParseData()
        {
            m_HANEventAppLog = new List<HANEventAppLogRcd>();

            for (int iIndex = 0; iIndex < 512; iIndex++)
            {
                HANEventAppLogRcd NewHANEventAppLog = new HANEventAppLogRcd();

                NewHANEventAppLog.TimeInMs = m_Reader.ReadUInt32();
                NewHANEventAppLog.EventCode = (HANEventAppLogRcd.EventCodes)m_Reader.ReadUInt16();
                NewHANEventAppLog.Param16 = m_Reader.ReadUInt16();
                NewHANEventAppLog.Param32 = m_Reader.ReadUInt32();

                // An event code of 'Unknown Type (0xFFFF)' means value is not used.
                if (NewHANEventAppLog.EventCode != HANEventAppLogRcd.EventCodes.Unknown)
                {
                    m_HANEventAppLog.Add(NewHANEventAppLog);
                }
            }
        }

        #endregion

        #region Members

        private UInt16 m_uiCurrentEntry;
        private List<HANEventAppLogRcd> m_HANEventAppLog;

        #endregion
    }
    
}
