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
    /// Bit masks for the initial security field
    /// </summary>
    [Flags]
    public enum EmberInitialSecurityBitmask : ushort
    {
        /// <summary>Running a network with standard security</summary>
        StandardSecurityMode = 0x0000,
        /// <summary>Enables high security mode</summary>
        HighSecurityMode = 0x0001,
        /// <summary>Enables distributed trust center mode</summary>
        DistributedTrustCenterMode = 0x0002,
        /// <summary>Enables a global link key for the trust center</summary>
        GlobalLinkKey = 0x0004,
        /// <summary>Allows devices that perform MAC association with a preconfigured key to join the network</summary>
        PreconfiguredNetworkKeyMode = 0x0008,
        /// <summary>Denotes the preconfigured key is not the Link Key but a secret key</summary>
        TrustCenterUsesHashedLinkKey = 0x0084,
        /// <summary>Denotes that the preconfigured key element has valid data</summary>
        HavePreconfiguredKey = 0x0100,
        /// <summary>Denotes that the network key element has valid data</summary>
        HaveNetworkKey = 0x0200,
        /// <summary>Denotes that a joining device should attempt to acquire a Trust Center link key</summary>
        GetLinkKeyWhenJoining = 0x0400,
        /// <summary>Denotes that a joining device should only accept an encrypted network key</summary>
        RequireEncryptedKey = 0x0800,
        /// <summary>Denotes whether the device should not reset its outgoing frame counters</summary>
        NoFrameCounterReset = 0x1000,
        /// <summary>Denotes the device should obtain its preconfigured key from an installation code stored in the manufacturer token</summary>
        GetPreconfiguredKeyFromInstallCode = 0x2000,
        /// <summary>Denotes that the trust center EUI is preconfigured</summary>
        HaveTrustCenterEUI = 0x0040,
    }

    /// <summary>
    /// Initial Security State
    /// </summary>
    public class EmberInitialSecurityState
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberInitialSecurityState()
        {
            m_InitialSecurityBitmask = EmberInitialSecurityBitmask.StandardSecurityMode;
            m_PreconfiguredKey = new byte[16];
            m_NetworkKey = new byte[16];
            m_NetworkKeySequenceNumber = 0;
            m_PreconfiguredTrustCenterEUI = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Initial Security Bitmask
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberInitialSecurityBitmask InitialSecurityBitmask
        {
            get
            {
                return m_InitialSecurityBitmask;
            }
            set
            {
                m_InitialSecurityBitmask = value;
            }
        }

        /// <summary>
        /// Gets or sets the preconfigured key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] PreconfiguredKey
        {
            get
            {
                return m_PreconfiguredKey;
            }
            set
            {
                if (value != null && value.Length == 16)
                {
                    m_PreconfiguredKey = value;
                }
                else
                {
                    throw new ArgumentException("The preconfigured key can not be null and must be 16 bytes long");
                }
            }
        }

        /// <summary>
        /// Gets or sets the network key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] NetworkKey
        {
            get
            {
                return m_NetworkKey;
            }
            set
            {
                if (value != null && value.Length == 16)
                {
                    m_NetworkKey = value;
                }
                else
                {
                    throw new ArgumentException("The network key can not be null and must be 16 bytes long");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Network Key Sequence Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte NetworkKeySequenceNumber
        {
            get
            {
                return m_NetworkKeySequenceNumber;
            }
            set
            {
                m_NetworkKeySequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Preconfigured Trust Center EUI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ulong PreconfiguredTrustCenterEUI
        {
            get
            {
                return m_PreconfiguredTrustCenterEUI;
            }
            set
            {
                m_PreconfiguredTrustCenterEUI = value;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the raw data for this object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[43];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write((ushort)m_InitialSecurityBitmask);
                DataWriter.Write(m_PreconfiguredKey);
                DataWriter.Write(m_NetworkKey);
                DataWriter.Write(m_NetworkKeySequenceNumber);
                DataWriter.Write(m_PreconfiguredTrustCenterEUI);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 43)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_InitialSecurityBitmask = (EmberInitialSecurityBitmask)DataReader.ReadUInt16();
                    m_PreconfiguredKey = DataReader.ReadBytes(16);
                    m_NetworkKey = DataReader.ReadBytes(16);
                    m_NetworkKeySequenceNumber = DataReader.ReadByte();
                    m_PreconfiguredTrustCenterEUI = DataReader.ReadUInt64();
                }
                else
                {
                    throw new ArgumentException("The raw data value must not be null and must be length 43");
                }
            }
        }

        #endregion

        #region Member Variables

        private EmberInitialSecurityBitmask m_InitialSecurityBitmask;
        private byte[] m_PreconfiguredKey;
        private byte[] m_NetworkKey;
        private byte m_NetworkKeySequenceNumber;
        private ulong m_PreconfiguredTrustCenterEUI;

        #endregion
    }
}
