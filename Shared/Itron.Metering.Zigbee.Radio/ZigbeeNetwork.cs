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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// 
    /// </summary>
    public class ZigbeeNetwork : System.IEquatable<ZigbeeNetwork>, 
                                     System.IComparable<ZigbeeNetwork>
    {
        #region Properties supported by both radios

        /// <summary></summary>
        public ulong ExPanID
        {
            get { return m_ExPanID; }
            set { m_ExPanID = value; }
        }

        /// <summary></summary>
        public byte LogicalChannel
        {
            get { return m_LogicalChannel; }
            set { m_LogicalChannel = value; }
        }

        /// <summary></summary>
        public byte StackProfile_ZigbeeVersion
        {
            get { return m_StackProfile_ZigbeeVersion; }
            set { m_StackProfile_ZigbeeVersion = value; }
        }

        /// <summary></summary>
        public byte StackProfile
        {
            get { return (byte)((m_StackProfile_ZigbeeVersion & 0xF0)>>4); }
        }

        /// <summary></summary>
        public byte ZigBeeVersion
        {
            get { return (byte)(m_StackProfile_ZigbeeVersion&0x0F); }
        }

        #endregion Properties supported by both radios

        #region IA Dongle specific properties

        /// <summary></summary>
        public byte Capabilities
        {
            get { return m_Capabilities; }
            set { m_Capabilities = value; }
        }

        /// <summary></summary>
        public bool AllowJoining
        {
            get { return ((m_Capabilities & 0x01) > 0); }
        }

        /// <summary>Guessing here. The specs' aren't much help</summary>
        public bool Router
        {
            get { return ((m_Capabilities & 0x02) > 0); }
        }

        /// <summary>Guessing here. The specs' aren't much help</summary>
        public bool EndDevice
        {
            get { return ((m_Capabilities & 0x04) > 0); }
        }

        /// <summary> </summary>
        public byte BeaconSuperframeOrder
        {
            get { return m_BeaconSuperframeOrder; }
            set { m_BeaconSuperframeOrder = value; }
        }

        /// <summary> </summary>
        public byte BeaconOrder
        {
            get { return (byte)(m_BeaconSuperframeOrder & 0x0F); }
        }

        /// <summary> </summary>
        public byte SuperframeOrder
        {
            get { return (byte)((m_BeaconSuperframeOrder & 0xF0)>>4); }
        }

        /// <summary>Compares another network to this one.  Two networks are
        /// equivalent if they have the same Pan ID</summary>
        /// <param name="OtherNetwork">The other network we're comparing this 
        /// instance to</param>
        public bool Equals(ZigbeeNetwork OtherNetwork)
        {
            return OtherNetwork.m_ExPanID == m_ExPanID;
        }

        /// <summary>Compares two networks for generic list sorting.
        ///    Less than zero - This instance is less than the OtherNetwork.
        ///              Zero - This instance is equal to OtherNetwork. 
        /// Greater than zero - This instance is greater than OtherNetwork. 
        ///</summary>
        /// <param name="OtherNetwork">The other network we're comparing this 
        /// instance to</param>
        public int CompareTo(ZigbeeNetwork OtherNetwork)
        {
            return m_LogicalChannel - OtherNetwork.m_LogicalChannel;
        }

        #endregion IA Dongle specific properties

        #region Belt Clip Radio specific properties
        /// <summary>
        /// 
        /// </summary>
        public byte ExpectingJoin
        {
            get { return m_ExpectingJoin; }
            set { m_ExpectingJoin = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte LastHopLqi
        {
            get { return m_LastHopLqi; }
            set { m_LastHopLqi = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ushort PanId
        {
            get { return m_PanId; }
            set { m_PanId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public sbyte LastHopRssi
        {
            get { return m_LastHopRssi; }
            set { m_LastHopRssi = value; }
        }

        /// <summary>
        /// Gets or sets the Network Update ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte NetworkUpdateID
        {
            get
            {
                return m_NetworkUpdateID;
            }
            set
            {
                m_NetworkUpdateID = value;
            }
        }

        #endregion Belt Clip Radio specific properties

        #region Private Members

        private ulong m_ExPanID;
        private byte m_LogicalChannel;
        private byte m_StackProfile_ZigbeeVersion;

        // IA Dongle specific properties
        private byte m_BeaconSuperframeOrder;
        private byte m_Capabilities;

        // Belt Clip Radio specific properties
        private byte m_ExpectingJoin;
        private byte m_LastHopLqi;
        private ushort m_PanId;
        private sbyte m_LastHopRssi;
        private byte m_NetworkUpdateID;

        #endregion Private Members

    }
}
