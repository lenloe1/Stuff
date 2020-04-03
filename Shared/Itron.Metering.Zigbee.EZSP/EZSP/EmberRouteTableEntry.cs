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
    /// Route Table Entry class for EZSP
    /// </summary>
    public class EmberRouteTableEntry
    {
        #region Definitions

        /// <summary>
        /// The route table entry status
        /// </summary>
        public enum EntryStatus : byte
        {
            /// <summary>
            /// Active
            /// </summary>
            Active = 0,
            /// <summary>
            /// Currently being discovered
            /// </summary>
            BeingDiscovered = 1,
            /// <summary>
            /// Entry unused
            /// </summary>
            Unused = 3,
            /// <summary>
            /// The route is validating
            /// </summary>
            Validating = 4,
        }

        /// <summary>
        /// Concentrator Types
        /// </summary>
        public enum ConcentratorTypes : byte
        {
            /// <summary>
            /// Node is not a concentrator
            /// </summary>
            NotAConcentrator = 0,
            /// <summary>
            /// Node is a low RAM concentrator
            /// </summary>
            LowRAMConcentrator = 1,
            /// <summary>
            /// Node is a High RAM concentrator
            /// </summary>
            HighRAMConcentrator = 2,
        }

        /// <summary>
        /// The Route Record state
        /// </summary>
        public enum RouteRecordStates : byte
        {
            /// <summary>
            /// No longer needed
            /// </summary>
            NotNeeded = 0,
            /// <summary>
            /// Has been sent
            /// </summary>
            Sent = 1,
            /// <summary>
            /// Route Record is needed
            /// </summary>
            Needed = 2,
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
        
        public EmberRouteTableEntry()
        {
            m_Destination = 0;
            m_NextHop = 0;
            m_Status = EntryStatus.Unused;
            m_Age = 0;
            m_ConcentratorType = ConcentratorTypes.NotAConcentrator;
            m_RouteRecordState = RouteRecordStates.Needed;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Destination
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort Destination
        {
            get
            {
                return m_Destination;
            }
            set
            {
                m_Destination = value;
            }
        }

        /// <summary>
        /// Gets or sets the next hop
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort NextHop
        {
            get
            {
                return m_NextHop;
            }
            set
            {
                m_NextHop = value;
            }
        }

        /// <summary>
        /// Gets or sets the status of the entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EntryStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Gets or sets the age of the entry
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
        /// Gets or sets the concentrator type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ConcentratorTypes ConcentratorType
        {
            get
            {
                return m_ConcentratorType;
            }
            set
            {
                m_ConcentratorType = value;
            }
        }

        /// <summary>
        /// Gets or sets the Route Record State
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public RouteRecordStates RouteRecordState
        {
            get
            {
                return m_RouteRecordState;
            }
            set
            {
                m_RouteRecordState = value;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the raw data for the Route Table entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[8];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write(m_Destination);
                DataWriter.Write(m_NextHop);
                DataWriter.Write((byte)m_Status);
                DataWriter.Write(m_Age);
                DataWriter.Write((byte)m_ConcentratorType);
                DataWriter.Write((byte)m_RouteRecordState);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 8)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_Destination = DataReader.ReadUInt16();
                    m_NextHop = DataReader.ReadUInt16();
                    m_Status = (EntryStatus)DataReader.ReadByte();
                    m_Age = DataReader.ReadByte();
                    m_ConcentratorType = (ConcentratorTypes)DataReader.ReadByte();
                    m_RouteRecordState = (RouteRecordStates)DataReader.ReadByte();
                }
                else
                {
                    throw new ArgumentException("The RawData value can not be set to null and the length must be 8");
                }
            }
        }

        #endregion

        #region Member Variables

        private ushort m_Destination;
        private ushort m_NextHop;
        private EntryStatus m_Status;
        private byte m_Age;
        private ConcentratorTypes m_ConcentratorType;
        private RouteRecordStates m_RouteRecordState;

        #endregion
    }
}
