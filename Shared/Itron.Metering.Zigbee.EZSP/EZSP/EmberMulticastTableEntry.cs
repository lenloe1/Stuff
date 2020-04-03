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
    /// Multicast Table Entry object
    /// </summary>
    public class EmberMulticastTableEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberMulticastTableEntry()
        {
            m_MulticastID = 0;
            m_Endpoint = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Multicast ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort MulticastID
        {
            get
            {
                return m_MulticastID;
            }
            set
            {
                m_MulticastID = value;
            }
        }

        /// <summary>
        /// Gets or sets the endpoint. If this value is 0 this indicates the entry is not used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte Endpoint
        {
            get
            {
                return m_Endpoint;
            }
            set
            {
                m_Endpoint = value;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the raw byte data for the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[3];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write(m_MulticastID);
                DataWriter.Write(m_Endpoint);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 3)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_MulticastID = DataReader.ReadUInt16();
                    m_Endpoint = DataReader.ReadByte();
                }
                else
                {
                    throw new ArgumentException("The RawData value must be 3 bytes long");
                }
            }
        }

        #endregion

        #region Member Variables

        private ushort m_MulticastID;
        private byte m_Endpoint;

        #endregion
    }
}
