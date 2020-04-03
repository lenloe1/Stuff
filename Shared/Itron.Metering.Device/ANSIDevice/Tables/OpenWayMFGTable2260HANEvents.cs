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
//                           Copyright © 2011 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;


namespace Itron.Metering.Device
{
    /// <summary>
    /// Sub Table class for the HAN Events Configuration in MFG Table 212 (2260)
    /// </summary>
    public class OpenWayMFGTable2260HANEvents : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 0x90;
        private const ushort TABLE_SIZE = 34;

        private const int HAN_EVENT_BYTES = 32;
        private const ushort LOG_TYPE_OFFSET = 0xB0;

        #endregion

        #region Definitions

        /// <summary>
        /// How the log is structured.
        /// </summary>
        public enum LogType : byte
        {
            /// <summary>
            /// Linear Log
            /// </summary>
            [EnumDescription("Linear Log")]
            Linear = 0,
            /// <summary>
            /// Circular Log
            /// </summary>
            [EnumDescription("Circular Log")]
            Circular = 1,
            /// <summary>
            /// Circular No Over Log
            /// </summary>
            [EnumDescription("Circular No Over Log")]
            CircularNoOver = 2,
            /// <summary>
            /// Not Configured
            /// </summary>
            [EnumDescription("Not Configured")]
            NotConfigured = 255,

        }

        /// <summary>
        /// Enum of HAN 2 log event ids to be used with EDL files
        /// </summary>
        public enum HAN2LogEvents : byte
        {
            // Downstream HAN Log events
            /// <summary>
            /// Message Published
            /// </summary>
            [EnumDescription("HAN Message Published")]
            MessagePublished = 1,
            /// <summary>
            /// Pricing Data Published
            /// </summary>
            [EnumDescription("HAN Pricing Data Published")]
            PricingDataPublished = 2,
            /// <summary>
            /// DRLC Data Published
            /// </summary>
            [EnumDescription("HAN DRLC Data Published")]
            DRLCDataPublished = 3,
            /// <summary>
            /// Price Tier Changed
            /// </summary>
            [EnumDescription("HAN Price Tier Changed")]
            PriceTierChanged = 4,
            /// <summary>
            /// New DRLC Event Dropped
            /// </summary>
            [EnumDescription("HAN New DRLC Event Dropped")]
            NewDRLCEventDropped = 6,
            /// <summary>
            /// Existing DRLC Event Dropped
            /// </summary>
            [EnumDescription("HAN Existing DRLC Event Dropped")]
            ExistingDRLCEventDropped = 7,
            /// <summary>
            /// Attribute Write
            /// </summary>
            [EnumDescription("HAN Attribute Write")]
            AttributeWrite = 8,
            /// <summary>
            /// Network Changed
            /// </summary>
            [EnumDescription("HAN Meter Network Changed")]
            MeterNetworkChange = 9,
            /// <summary>
            /// Security Profile Update Notification
            /// </summary>
            [EnumDescription("HAN Security Profile Update Notification")]
            SecurityProfileUpdateNotification = 10,
            /// <summary>
            /// Diagnostic System Event
            /// </summary>
            [EnumDescription("HAN Diagnostic System Event")]
            DiagnosticSystemEvent = 11,
            /// <summary>
            /// Move Out
            /// </summary>
            [EnumDescription("HAN Move Out")]
            MoveOut = 12,
            /// <summary>
            /// RIB Meter Event
            /// </summary>
             [EnumDescription("HAN RIB Meter Event")]
           RIBMeterEvent = 13,
            /// <summary>
            /// RIB CE Event
            /// </summary>
            [EnumDescription("HAN RIB CE Event")]
            RIBCEEvent = 14,
            /// <summary>
            /// RIB Error Event
            /// </summary>
            [EnumDescription("HAN RIB Error Event")]
            RIBErrorEvent = 15,
            /// <summary>
            /// RIB Next Block Price Commit Timeout
            /// </summary>
            [EnumDescription("HAN RIB Next Block Price Commit Timeout")]
            NextBlockPriceCommitTimeout = 16,
            /// <summary>
            /// Price Disable occurred
            /// </summary>
            [EnumDescription("HAN Price Disable occurred")]
            PriceDisabled = 17,
            /// <summary>
            /// Event Cache Overflow
            /// </summary>
            [EnumDescription("HAN Event Cache Overflow Downstream")]
            EventCacheOverflowDownstream = 127,
            //Upstream HAN log events
            /// <summary>
            /// Message Confirmation
            /// </summary>
            [EnumDescription("HAN Message Confirmation")]
            MessageConfirmation = 128,
            /// <summary>
            /// Price Acknowledgement
            /// </summary>
            [EnumDescription("HAN Price Acknowledgement")]
            PriceAcknowledgement = 129,
            /// <summary>
            /// Load Control Opt Out
            /// </summary>
            [EnumDescription("HAN Load Control Opt Out")]
            LoadControlOptOut = 130,
            /// <summary>
            /// Load Control Status
            /// </summary>
            [EnumDescription("HAN Load Control Status")]
            LoadControlStatus = 131,
            /// <summary>
            /// Device State Change
            /// </summary>
            [EnumDescription("HAN State Changed")]
            DeviceStateChange = 132,
            /// <summary>
            /// Device Added
            /// </summary>
            [EnumDescription("HAN Device Added")]
            DeviceAdded = 133,
            /// <summary>
            /// Device Dropped
            /// </summary>
            [EnumDescription("HAN Device Dropped")]
            DeviceDropped = 134,
            /// <summary>
            /// Device Not Heard
            /// </summary>
            [EnumDescription("HAN Device Not Heard Event")]
            DeviceNotHeard = 135,
            /// <summary>
            /// Device Pinged
            /// </summary>
            [EnumDescription("HAN Ping Event")]
            DevicePinged = 136,
            /// <summary>
            /// Message Delivery Failed
            /// </summary>
            [EnumDescription("HAN Message Delivery Failed")]
            MessageDeliveryFailed = 137,
            /// <summary>
            /// Price Delivery Failed
            /// </summary>
            [EnumDescription("HAN Price Delivery Failed")]
            PriceDeliveryFailed = 138,
            /// <summary>
            /// DRLC Delivery Failed
            /// </summary>
            [EnumDescription("HAN DRLC Delivery Failed")]
            DRLCDeliveryFailed = 139,
            /// <summary>
            /// Submetering Bubble-up Data Missing
            /// </summary>
            [EnumDescription("HAN Submetering Bubble-up Data Missing")]
            SubmeteringBubbleUpDataMissing = 140,
            /// <summary>
            /// Device UEG Changed
            /// </summary>
            [EnumDescription("HAN Device UEG Changed")]
            DeviceUEGChanged = 141,
            /// <summary>
            /// Device Added
            /// </summary>
            [EnumDescription("HAN Device Joined")]
            DeviceJoined = 142,
            /// <summary>
            /// Device Not Joined at End of Join Period
            /// </summary>
            [EnumDescription("HAN Device Not Joined at End of Join Period")]
            JoinFlagDisabledWithUnjoinedDevice = 143,
            /// <summary>
            /// Event Cache Overflowed
            /// </summary>
            [EnumDescription("HAN Event Cache Overflow Upstream")]
            EventCacheOverflowUpstream = 255,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 jrf 2.50.02        Created
        //
        public OpenWayMFGTable2260HANEvents(CPSEM psem)
            : base(psem, 2260, TABLE_OFFSET, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Constructor used to get Event Data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/04/11 AF  2.50.43 171706 Created
        //
        public OpenWayMFGTable2260HANEvents(PSEMBinaryReader reader)
            : base(2260, TABLE_SIZE)
        {
            m_Reader = reader;
            m_lstHANEventConfiguration = new List<MFG2260HANEventItem>();
            ParseEventData();
            m_TableState = TableState.Loaded;          
        }

        /// <summary>
        /// Reads the sub table from the meter.
        /// </summary>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 jrf 2.50.02        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Response = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2260HANEvents.Read()");

            Response = base.Read();

            if (Response == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Response;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// How the HAN log is configured in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 jrf 2.50.02        Created
        //
        public LogType HANLogType
        {
            get
            {
                ReadUnloadedTable();
                LogType HANLogType = LogType.NotConfigured;

                switch (m_byLogType)
                {
                    case (byte)LogType.Linear:
                        {
                            HANLogType = LogType.Linear;
                            break;
                        }
                    case (byte)LogType.Circular:
                        {
                            HANLogType = LogType.Circular;
                            break;
                        }
                    case (byte)LogType.CircularNoOver:
                        {
                            HANLogType = LogType.CircularNoOver;
                            break;
                        }
                    default:
                        HANLogType = LogType.NotConfigured;
                        break;
                }

                return HANLogType;
            }
            set
            {
                m_byLogType = (byte)value;

                m_DataStream.Position = LOG_TYPE_OFFSET;
                m_Writer.Write(m_byLogType);

                base.Write(LOG_TYPE_OFFSET, 1);
            }
        }

        /// <summary>
        /// Each bit position in the byte array returned represents a HAN event.
        /// If the event is configured, the bit value is 1; otherwise, 0.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 jrf 2.50.02        Created
        //
        public byte[] HANEventsConfigured
        {
            get
            {
                ReadUnloadedTable();

                return m_abyHANEvents;
            }
            set
            {
                m_abyHANEvents = value;

                m_DataStream.Position = 0;
                m_Writer.Write(m_abyHANEvents);

                base.Write(0, (ushort)m_abyHANEvents.Length);
            }
        }

        /// <summary>
        /// Gets the HAN 2 event configuration from Mfg table 212
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/11 AF  2.50.43 171706 Created
        //  05/25/11 AF  2.50.48 173750 Removed Load Control Queue Updated event - no longer used
        //                              and changed Current Price Published to Price Tier Changed
        //
        public List<MFG2260HANEventItem> HAN2EventConfiguration
        {
            get
            {
                ReadUnloadedTable();

                m_lstHANEventConfiguration.Clear();

                byte[] abyEvents = HANEventsConfigured;
                bool[] ablnEvents = new bool[abyEvents.Length * 8];
                int nBitMask = 0x01;

                for (int iIndex = 0; iIndex < abyEvents.Length; iIndex++)
                {
                    nBitMask = 0x01;
                    for (int jIndex = 0; jIndex < 8; jIndex++)
                    {
                        ablnEvents[(iIndex * 8) + jIndex] = (abyEvents[iIndex] & nBitMask) != 0;
                        nBitMask = nBitMask << 1;
                    }
                }

                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.MessagePublished), ablnEvents[1], 1);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.PricingDataPublished), ablnEvents[2], 2);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DRLCDataPublished), ablnEvents[3], 3);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.PriceTierChanged), ablnEvents[4], 4);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.NewDRLCEventDropped), ablnEvents[6], 6);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.ExistingDRLCEventDropped), ablnEvents[7], 7);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.AttributeWrite), ablnEvents[8], 8);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.MeterNetworkChange), ablnEvents[9], 9);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DiagnosticSystemEvent), ablnEvents[11], 11);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.MoveOut), ablnEvents[12], 12);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.RIBMeterEvent), ablnEvents[13], 13);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.RIBCEEvent), ablnEvents[14], 14);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.RIBErrorEvent), ablnEvents[15], 15);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.NextBlockPriceCommitTimeout), ablnEvents[16], 16);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.PriceDisabled), ablnEvents[17], 17);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.MessageConfirmation), ablnEvents[128], 128);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.PriceAcknowledgement), ablnEvents[129], 129);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.LoadControlOptOut), ablnEvents[130], 130);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.LoadControlStatus), ablnEvents[131], 131);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DeviceStateChange), ablnEvents[132], 132);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DeviceAdded), ablnEvents[133], 133);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DeviceDropped), ablnEvents[134], 134);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DeviceNotHeard), ablnEvents[135], 135);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DevicePinged), ablnEvents[136], 136);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.MessageDeliveryFailed), ablnEvents[137], 137);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.PriceDeliveryFailed), ablnEvents[138], 138);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DRLCDeliveryFailed), ablnEvents[139], 139);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.SubmeteringBubbleUpDataMissing), ablnEvents[140], 140);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DeviceUEGChanged), ablnEvents[141], 141);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.DeviceJoined), ablnEvents[142], 142);
                AddHANEventConfigItem(EnumDescriptionRetriever.RetrieveDescription(HAN2LogEvents.JoinFlagDisabledWithUnjoinedDevice), ablnEvents[143], 143);
                
                return m_lstHANEventConfiguration;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the sub table from the data that was just read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/11 jrf 2.50.02        Created

        private void ParseData()
        {
            m_abyHANEvents = m_Reader.ReadBytes(HAN_EVENT_BYTES);
            m_byLogType = m_Reader.ReadByte();
            m_Reader.ReadByte(); //Dummy Byte
        }

        /// <summary>
        /// Parses the sub table from the data that was just read from an EDL file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/04/11 AF  2.50.43 171706 Created
        //
        private void ParseEventData()
        {
            m_abyHANEvents = m_Reader.ReadBytes(HAN_EVENT_BYTES);
        }

        /// <summary>
        /// Constructs a new MFG2260HANEventItem
        /// </summary>
        /// <param name="Description">A description of the event</param>
        /// <param name="Enabled">Whether or not the event is enabled</param>
        /// <param name="EventID">An identifier for the event</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/11 AF  2.50.43 171706 Created
        //
        private void AddHANEventConfigItem(string Description, bool Enabled, byte EventID)
        {
            MFG2260HANEventItem eventItem = new MFG2260HANEventItem(Description, Enabled, EventID);
            m_lstHANEventConfiguration.Add(eventItem);
        }

        #endregion

        #region Member Variables

        private byte[] m_abyHANEvents = new byte[HAN_EVENT_BYTES];
        private byte m_byLogType;
        /// <summary>
        /// List of Mfg table 212 HAN events
        /// </summary>
        protected List<MFG2260HANEventItem> m_lstHANEventConfiguration;

        #endregion
    }

    /// <summary>
    /// Simple class to represent an MFG table 2260 event item as a description
    /// and a boolean, which tells whether or not the event is enabled in
    /// the meter.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 05/06/11 AF  2.50.43 171706    Created - cloned from MFG2048EventItem
    //
    public class MFG2260HANEventItem : IEquatable<MFG2260HANEventItem>, IComparable<MFG2260HANEventItem>
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/11 AF  2.50.43 171706 Created - cloned from MFG2048EventItem
        //
        public MFG2260HANEventItem()
        {
            m_strDescription = "";
            m_blnEnabled = false;
            m_iID = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="Enabled"></param>
        /// <param name="ID"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/11 AF  2.50.43 171706 Created
        //
        public MFG2260HANEventItem(string Description, bool Enabled, int ID)
        {
            m_strDescription = Description;
            m_blnEnabled = Enabled;
            m_iID = ID;
        }

        /// <summary>
        /// Determines whether the two Events are equal
        /// </summary>
        /// <param name="other">The event to compare to</param>
        /// <returns>True if the evenst are equal. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/11 AF  2.50.43 171706 Created - cloned from MFG2048EventItem
        //
        public bool Equals(MFG2260HANEventItem other)
        {
            return Description.Equals(other.Description);
        }

        /// <summary>
        /// Determines whether the values are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if they are equal. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/11 AF  2.50.43 171706 Created - cloned from MFG2048EventItem
        //
        public override bool Equals(object obj)
        {
            bool bEquals = false;
            MFG2260HANEventItem Other = obj as MFG2260HANEventItem;

            if (Other != null)
            {
                bEquals = Equals(Other);
            }

            return bEquals;
        }

        /// <summary>
        /// Gets the hash code for the event.
        /// </summary>
        /// <returns>The hash code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/11 AF	2.50.43 171706 Created

        public override int GetHashCode()
        {
            return Description.GetHashCode();
        }

        /// <summary>
        /// Compares the two events.
        /// </summary>
        /// <param name="other">The event to compare to.</param>
        /// <returns>0 if the values are equal. A negative number if less than or a positve number if greater than.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/11 AF  2.50.43 171706 Created - cloned from MFG2048EventItem
        //
        public int CompareTo(MFG2260HANEventItem other)
        {
            return String.Compare(Description, other.Description, StringComparison.CurrentCulture);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event's description
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/11 AF  2.50.43 171706 Created - cloned from MFG2048EventItem
        //
        public string Description
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the event has been enabled
        /// in the meter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/11 AF  2.50.43 171706 Created - cloned from MFG2048EventItem
        //
        public bool Enabled
        {
            get
            {
                return m_blnEnabled;
            }
            set
            {
                m_blnEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the event's ID code.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/06/11 AF  2.50.43 171706 Created - cloned from MFG2048EventItem
        //
        public int ID
        {
            get
            {
                return m_iID;
            }
            set
            {
                m_iID = value;
            }
        }

        #endregion

        #region Members

        private string m_strDescription;
        private bool m_blnEnabled;
        private int m_iID;

        #endregion
    }
}
