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
    /// Binding Table Entry
    /// </summary>
    public class EmberBindingTableEntry
    {
        #region Definitions

        /// <summary>
        /// Binding Types
        /// </summary>
        public enum EmberBindingType : byte
        {
            /// <summary>The binding is not used</summary>
            UnusedBinding = 0x00,
            /// <summary>Unicast binding</summary>
            UnicastBinding = 0x01,
            /// <summary>Many to One binding</summary>
            ManyToOneBinding = 0x02,
            /// <summary>Multicast binding</summary>
            MultiCastBinding = 0x03,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberBindingTableEntry()
        {
            m_BindingType = EmberBindingType.UnusedBinding;
            m_LocalEndpoint = 0;
            m_ClusterID = 0;
            m_RemoteEndpoint = 0;
            m_EUI = 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets or sets the binding type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberBindingType BindingType
        {
            get
            {
                return m_BindingType;
            }
            set
            {
                m_BindingType = value;
            }
        }

        /// <summary>
        /// Gets or sets the endpoint on the local node
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte LocalEndpoint
        {
            get
            {
                return m_LocalEndpoint;
            }
            set
            {
                m_LocalEndpoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the cluster ID 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort ClusterID
        {
            get
            {
                return m_ClusterID;
            }
            set
            {
                m_ClusterID = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the remote endpoint
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte RemoteEndpoint
        {
            get
            {
                return m_RemoteEndpoint;
            }
            set
            {
                m_RemoteEndpoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the EUI identifier. For Unicast binding this is the destination EUI.
        /// For Many To One it is the aggregator EUI and for Multicast it is the group address.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ulong EUI
        {
            get
            {
                return m_EUI;
            }
            set
            {
                m_EUI = value;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets or sets the object as the raw data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[13];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write((byte)m_BindingType);
                DataWriter.Write(m_LocalEndpoint);
                DataWriter.Write(m_ClusterID);
                DataWriter.Write(m_RemoteEndpoint);
                DataWriter.Write(m_EUI);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 13)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_BindingType = (EmberBindingType)DataReader.ReadByte();
                    m_LocalEndpoint = DataReader.ReadByte();
                    m_ClusterID = DataReader.ReadUInt16();
                    m_RemoteEndpoint = DataReader.ReadByte();
                    m_EUI = DataReader.ReadUInt64();
                }
                else
                {
                    throw new ArgumentException("The RawData value cannot be null and must be 13 bytes long");
                }
            }
        }

        #endregion

        #region Member Variables

        private EmberBindingType m_BindingType;
        private byte m_LocalEndpoint;
        private ushort m_ClusterID;
        private byte m_RemoteEndpoint;
        private ulong m_EUI;

        #endregion
    }
}
