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

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// Method to use for Network Joining
    /// </summary>
    public enum EmberJoinMethod : byte
    {
        /// <summary>Use MAC Association</summary>
        MACAssociation = 0x00,
        /// <summary>Use Network Rejoin</summary>
        NetworkRejoin = 0x01,
        /// <summary>Use Network Rejoin with the Network Key</summary>
        NetworkRejoinHaveNetworkKey = 0x02,
        /// <summary>Use Network Commissioning</summary>
        NetworkCommissioning = 0x03,
    }

    /// <summary>
    /// The Node Types for ZigBee devices
    /// </summary>
    public enum EmberNodeType : byte
    {
        /// <summary>Unknown - Typically this means the device is not joined</summary>
        UnknownDevice = 0x00,
        /// <summary>Coordinator</summary>
        Coordinator = 0x01,
        /// <summary>Router</summary>
        Router = 0x02,
        /// <summary>End Device</summary>
        EndDevice = 0x03,
        /// <summary>Sleepy End Device</summary>
        SleepyEndDevice = 0x04,
        /// <summary>Mobile End Device</summary>
        MobileEndDevice = 0x05,
    }

    /// <summary>
    /// Network Parameters
    /// </summary>
    public class EmberNetworkParameters
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberNetworkParameters()
        {
            m_ExtendedPANID = 0;
            m_PANID = 0;
            m_RadioTransmitPower = 0;
            m_RadioChannel = 0;
            m_JoinMethod = EmberJoinMethod.MACAssociation;
            m_NetworkManagerID = 0;
            m_NetworkUpdateID = 0;
            m_Channels = ZigBeeChannels.None;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Extended PAN ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ulong ExtendedPANID
        {
            get
            {
                return m_ExtendedPANID;
            }
            set
            {
                m_ExtendedPANID = value;
            }
        }

        /// <summary>
        /// Gets or sets the PAN ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort PANID
        {
            get
            {
                return m_PANID;
            }
            set
            {
                m_PANID = value;
            }
        }

        /// <summary>
        /// Gets or sets the radio transmit power
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public sbyte RadioTransmitPower
        {
            get
            {
                return m_RadioTransmitPower;
            }
            set
            {
                m_RadioTransmitPower = value;
            }
        }

        /// <summary>
        /// Gets or sets radio channel
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte RadioChannel
        {
            get
            {
                return m_RadioChannel;
            }
            set
            {
                m_RadioChannel = value;
            }
        }

        /// <summary>
        /// Gets or sets method used to join the network
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberJoinMethod JoinMethod
        {
            get
            {
                return m_JoinMethod;
            }
            set
            {
                m_JoinMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the Network Manager
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort NetworkManagerID
        {
            get
            {
                return m_NetworkManagerID;
            }
            set
            {
                m_NetworkManagerID = value;
            }
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

        /// <summary>
        /// Gets or sets the preferred channel mask
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ZigBeeChannels Channels
        {
            get
            {
                return m_Channels;
            }
            set
            {
                m_Channels = value;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the Network Parameters as a byte[] to use when sending to the radio.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[20];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write(m_ExtendedPANID);
                DataWriter.Write(m_PANID);
                DataWriter.Write(m_RadioTransmitPower);
                DataWriter.Write(m_RadioChannel);
                DataWriter.Write((byte)m_JoinMethod);
                DataWriter.Write(m_NetworkManagerID);
                DataWriter.Write(m_NetworkUpdateID);
                DataWriter.Write((uint)m_Channels);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 20)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_ExtendedPANID = DataReader.ReadUInt64();
                    m_PANID = DataReader.ReadUInt16();
                    m_RadioTransmitPower = DataReader.ReadSByte();
                    m_RadioChannel = DataReader.ReadByte();
                    m_JoinMethod = (EmberJoinMethod)DataReader.ReadByte();
                    m_NetworkManagerID = DataReader.ReadUInt16();
                    m_NetworkUpdateID = DataReader.ReadByte();
                    m_Channels = (ZigBeeChannels)DataReader.ReadInt32();
                }
                else
                {
                    throw new ArgumentException("RawData can not be null and must be length 20");
                }
            }
        }

        #endregion

        #region Member Variables

        private ulong m_ExtendedPANID;
        private ushort m_PANID;
        private sbyte m_RadioTransmitPower;
        private byte m_RadioChannel;
        private EmberJoinMethod m_JoinMethod;
        private ushort m_NetworkManagerID;
        private byte m_NetworkUpdateID;
        private ZigBeeChannels m_Channels;

        #endregion
    }
}
