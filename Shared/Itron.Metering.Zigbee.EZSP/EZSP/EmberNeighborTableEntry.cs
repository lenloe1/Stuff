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
    /// Neighbor Table Entry class for EZSP
    /// </summary>
    public class EmberNeighborTableEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberNeighborTableEntry()
        {
            m_ShortID = 0;
            m_AverageLQI = 0;
            m_IncomingCost = 0;
            m_OutgoingCost = 0;
            m_Age = 0;
            m_EUI = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Short ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort ShortID
        {
            get
            {
                return m_ShortID;
            }
            set
            {
                m_ShortID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Average LQI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte AverageLQI
        {
            get
            {
                return m_AverageLQI;
            }
            set
            {
                m_AverageLQI = value;
            }
        }

        /// <summary>
        /// Gets or sets the Incoming Cost of the neighbor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte IncomingCost
        {
            get
            {
                return m_IncomingCost;
            }
            set
            {
                m_IncomingCost = value;
            }
        }

        /// <summary>
        /// Gets or sets the outgoing cost of the neighbor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte OutgoingCost
        {
            get
            {
                return m_OutgoingCost;
            }
            set
            {
                m_OutgoingCost = value;
            }
        }

        /// <summary>
        /// Gets or sets the age of the neighbor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte Age
        {
            get
            {
                return m_Age;
            }
            set
            {
                m_Age = value;
            }
        }

        /// <summary>
        /// Gets or sets the neighbor EUI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        private ulong EUI
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

        #region Internal Properties

        /// <summary>
        /// Gets or sets the raw binary data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[14];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write(m_ShortID);
                DataWriter.Write(m_AverageLQI);
                DataWriter.Write(m_IncomingCost);
                DataWriter.Write(m_OutgoingCost);
                DataWriter.Write(m_Age);
                DataWriter.Write(m_EUI);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 14)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_ShortID = DataReader.ReadUInt16();
                    m_AverageLQI = DataReader.ReadByte();
                    m_IncomingCost = DataReader.ReadByte();
                    m_OutgoingCost = DataReader.ReadByte();
                    m_Age = DataReader.ReadByte();
                    m_EUI = DataReader.ReadUInt64();
                }
                else
                {
                    throw new ArgumentException("The RawData value can not be null and must be length 14.");
                }
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ShortID;
        private byte m_AverageLQI;
        private byte m_IncomingCost;
        private byte m_OutgoingCost;
        private byte m_Age;
        private ulong m_EUI;

        #endregion
    }
}
