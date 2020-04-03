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
    /// Ember Current Security State
    /// </summary>
    public class EmberCurrentSecurityState
    {
        #region Definitions

        /// <summary>
        /// Bitmask for the Current Security
        /// </summary>
        public enum EmberCurrentSecurityBitmask : ushort
        {
            /// <summary>The device is running in a network with ZigBee Standard Security</summary>
            StandardSecurityMode = 0x0000,
            /// <summary>The device is running in a network with ZigBee High Security</summary>
            HighSecurityMode = 0x0001,
            /// <summary>The device is running in a network without a centralized Trust Center</summary>
            DistributedTrustCenterMode = 0x0002,
            /// <summary>The device has a global link key</summary>
            GlobalLinkKey = 0x0004,
            /// <summary>The node had a trust center link key</summary>
            HaveTrustCenterLinkKey = 0x0010,
            /// <summary>The trust center is using a hashed link key</summary>
            TrustCenterUsesHashedLinkKey = 0x0084,
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
        
        public EmberCurrentSecurityState()
        {
            m_CurrentSecurityBitmask = EmberCurrentSecurityBitmask.StandardSecurityMode;
            m_TrustCenterEUI = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Current Security Bitmask
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberCurrentSecurityBitmask CurrentSecurityBitmask
        {
            get
            {
                return m_CurrentSecurityBitmask;
            }
            set
            {
                m_CurrentSecurityBitmask = value;
            }
        }

        /// <summary>
        /// Gets or sets the Trust Center EUI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ulong TrustCenterEUI
        {
            get
            {
                return m_TrustCenterEUI;
            }
            set
            {
                m_TrustCenterEUI = value;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the raw byte data of the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[10];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write((ushort)m_CurrentSecurityBitmask);
                DataWriter.Write(m_TrustCenterEUI);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 10)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_CurrentSecurityBitmask = (EmberCurrentSecurityBitmask)DataReader.ReadUInt16();
                    m_TrustCenterEUI = DataReader.ReadUInt64();
                }
                else
                {
                    throw new ArgumentException("The RawData value can not be null and must be length 10");
                }
            }
        }

        #endregion

        #region Member Variables

        private EmberCurrentSecurityBitmask m_CurrentSecurityBitmask;
        private ulong m_TrustCenterEUI;

        #endregion
    }
}
