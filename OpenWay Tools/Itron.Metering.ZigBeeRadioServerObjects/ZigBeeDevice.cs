///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential  
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or  
//  otherwise. Including photocopying and recording or in connection with any 
//  information storage or retrieval system without the permission in writing 
//  from Itron, Inc.
//
//                              Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Itron.Metering.Zigbee;

namespace Itron.Metering.ZigBeeRadioServerObjects
{
    /// <summary>
    /// Class that represents an individual device on the ZigBee network.
    /// </summary>
    [DataContract]
    public class ZigBeeDevice : IEquatable<ZigBeeDevice>
    {
        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/12/08 RCG 1.00           Created

        public ZigBeeDevice(ulong ulExPANID, byte byLogicalChannel, sbyte sbyRSSI, DateTime dtScanTime)
        {
            m_ulExPANID = ulExPANID;
            m_byLogicalChannel = byLogicalChannel;
            m_dtScanTime = dtScanTime;
            m_sbyRSSI = sbyRSSI;
        }

        /// <summary>
        /// Determines if the ZigBeeDevice object is equal to the current object
        /// </summary>
        /// <param name="other">The ZigBeeDevice object to compare</param>
        /// <returns>True if the objects are equal false otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public bool Equals(ZigBeeDevice other)
        {
            bool bIsEqual = false;

            if (ExPANID == other.ExPANID)
            {
                bIsEqual = true;
            }

            return bIsEqual;
        }

        /// <summary>
        /// Gets or sets the PAN ID for the current device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        [DataMember]
        public ulong ExPANID
        {
            get
            {
                return m_ulExPANID;
            }
            set
            {
                m_ulExPANID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Channel used for scanning of the current device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        [DataMember]
        public byte LogicalChannel
        {
            get
            {
                return m_byLogicalChannel;
            }
            set
            {
                m_byLogicalChannel = value;
            }
        }

        /// <summary>
        /// Gets or set the last hop rssi
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/09/08 RCG 1.50.33        Created

        [DataMember]
        public sbyte LastHopRSSI
        {
            get
            {
                return m_sbyRSSI;
            }
            set
            {
                m_sbyRSSI = value;
            }
        }

        /// <summary>
        /// Gets or sets the time that the device was last scanned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/29/08 RCG 1.00           Created

        [DataMember]
        public DateTime ScanTime
        {
            get
            {
                return m_dtScanTime;
            }
            set
            {
                m_dtScanTime = value;
            }
        }

        /// <summary>
        /// Gets whether or not the current device is an Itron device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/25/08 RCG 1.00           Created

        public bool IsItronDevice
        {
            get
            {
                return (m_ulExPANID & Radio.ITRON_DEVICE_MAC_MASK) == Radio.ITRON_DEVICE_MAC_BASE; 
            }
        }

        /// <summary>
        /// Gets the device type of the device assuming that it is an Itron device.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/25/08 RCG 1.00           Created

        public ZigbeeDeviceType DeviceType
        {
            get
            {
                return (ZigbeeDeviceType)((m_ulExPANID & Radio.ITRON_DEVICE_TYPE_MASK) >> Radio.ITRON_DEVICE_TYPE_SHIFT);
            }
        }

        /// <summary>
        /// Gets the Channel number that is to be used when issuing a start command.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/25/08 RCG 1.00           Created

        public uint ScanChannel
        {
            get
            {
                return (uint)(0x1 << m_byLogicalChannel);
            }
        }

        #endregion

        #region Member Variables

        private ulong m_ulExPANID;
        private byte m_byLogicalChannel;
        private DateTime m_dtScanTime;
        private sbyte m_sbyRSSI;

        #endregion
    }
}
