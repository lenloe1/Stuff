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
    /// Key Struct class for EZSP
    /// </summary>
    public class EmberKeyStruct
    {
        #region Definitions

        /// <summary>
        /// The Bitmask for the Key struct indicating what items are valid
        /// </summary>
        [Flags]
        public enum EmberKeyStructBitmask : ushort
        {
            /// <summary>None</summary>
            None = 0x0000,
            /// <summary>The Sequence Number is valid</summary>
            KeyHasSequenceNumber = 0x0001,
            /// <summary>The Outgoing Frame Counter is valid</summary>
            KeyHasOutgoingFrameCounter = 0x0002,
            /// <summary>The incoming Frame Counter is valid</summary>
            KeyHasIncomingFrameCounter = 0x0004,
            /// <summary>The Partner EUI is valid</summary>
            KeyHasPartnerEUI = 0x0008,
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
        
        public EmberKeyStruct()
        {
            m_KeyStructBitmask = EmberKeyStructBitmask.None;
            m_KeyType = EmberKeyType.ApplicationLinkKey;
            m_Key = new byte[16];
            m_OutgoingFrameCounter = 0;
            m_IncomingFrameCounter = 0;
            m_SequenceNumber = 0;
            m_PartnerEUI = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Key struct bitmask used to determine which values are valid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberKeyStructBitmask KeyStructBitmask
        {
            get
            {
                return m_KeyStructBitmask;
            }
            set
            {
                m_KeyStructBitmask = value;
            }
        }

        /// <summary>
        /// Gets or sets the key type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberKeyType KeyType
        {
            get
            {
                return m_KeyType;
            }
            set
            {
                m_KeyType = value;
            }
        }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte[] Key
        {
            get
            {
                return m_Key;
            }
            set
            {
                if (value != null && value.Length == 16)
                {
                    m_Key = value;
                }
                else
                {
                    throw new ArgumentException("The Key value can not be null and must be length 16");
                }
            }
        }

        /// <summary>
        /// Gets or sets the outgoing frame counter associated with the key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public uint OutgoingFrameCounter
        {
            get
            {
                return m_OutgoingFrameCounter;
            }
            set
            {
                m_OutgoingFrameCounter = value;
            }
        }

        /// <summary>
        /// Gets or sets the frame counter of the partner device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public uint IncomingFrameCounter
        {
            get
            {
                return m_IncomingFrameCounter;
            }
            set
            {
                m_IncomingFrameCounter = value;
            }
        }

        /// <summary>
        /// Gets or sets the sequence number associated with the key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte SequenceNumber
        {
            get
            {
                return m_SequenceNumber;
            }
            set
            {
                m_SequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Partner's EUI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ulong PartnerEUI
        {
            get
            {
                return m_PartnerEUI;
            }
            set
            {
                m_PartnerEUI = value;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the raw data for the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[36];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write((ushort)m_KeyStructBitmask);
                DataWriter.Write((byte)m_KeyType);
                DataWriter.Write(m_Key);
                DataWriter.Write(m_OutgoingFrameCounter);
                DataWriter.Write(m_IncomingFrameCounter);
                DataWriter.Write(m_SequenceNumber);
                DataWriter.Write(m_PartnerEUI);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 36)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_KeyStructBitmask = (EmberKeyStructBitmask)DataReader.ReadUInt16();
                    m_KeyType = (EmberKeyType)DataReader.ReadByte();
                    m_Key = DataReader.ReadBytes(16);
                    m_OutgoingFrameCounter = DataReader.ReadUInt32();
                    m_IncomingFrameCounter = DataReader.ReadUInt32();
                    m_SequenceNumber = DataReader.ReadByte();
                    m_PartnerEUI = DataReader.ReadUInt64();
                }
                else
                {
                    throw new ArgumentException("The RawData value cannot be null and must be 36 bytes long");
                }
            }
        }

        #endregion

        #region Member Variables

        private EmberKeyStructBitmask m_KeyStructBitmask;
        private EmberKeyType m_KeyType;
        private byte[] m_Key;
        private uint m_OutgoingFrameCounter;
        private uint m_IncomingFrameCounter;
        private byte m_SequenceNumber;
        private ulong m_PartnerEUI;

        #endregion
    }
}
