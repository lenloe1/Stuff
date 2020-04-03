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
    #region Definitions

    /// <summary>
    /// APS Frame options
    /// </summary>
    [Flags]
    public enum EmberApsOptions : ushort
    {
        /// <summary>No options selected</summary>
        None = 0x0000,
        /// <summary>Signs the message using DSA</summary>
        DSASign = 0x0010,
        /// <summary>Send the message using APS encryption</summary>
        Encryption = 0x0020,
        /// <summary>Resend the message using the APS retry</summary>
        Retry = 0x0040,
        /// <summary>Cause a route discovery to be initiated if no route to the destination is known</summary>
        EnableRouteDiscovery = 0x0100,
        /// <summary>Cause a route discovery to be initiated even if one is known</summary>
        ForceRouteDiscovery = 0x0200,
        /// <summary>Include the source EUI in the Network Frame</summary>
        SourceEUI = 0x0400,
        /// <summary>Include the destination EUI in the Network Frame</summary>
        DestinationEUI = 0x0800,
        /// <summary>Send a ZDO request to discover the node ID of the destination if its not known</summary>
        EnableAddressDiscovery = 0x1000,
        /// <summary></summary>
        PollResponse = 0x2000,
        /// <summary>The incoming message is a ZDO request and the application is responsible for sending the ZDO response</summary>
        ZdoResponseRequired = 0x4000,
        /// <summary>This message is part of a fragmented message</summary>
        Fragment = 0x8000,
    }

    #endregion

    /// <summary>
    /// Ember Aps frame object
    /// </summary>
    public class EmberApsFrame
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberApsFrame()
        {
            m_ProfileID = 0;
            m_ClusterID = 0;
            m_SourceEndpoint = 0;
            m_DestinationEndpoint = 0;
            m_Options = EmberApsOptions.None;
            m_GroupID = 0;
            m_Sequence = 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the Current Fragment number from the group ID of a fragmented message.
        /// </summary>
        /// <param name="groupID">The group ID of the fragmented message</param>
        /// <returns>The current fragment number</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 RCG 2.60.07        Created
        
        public static byte GetFragmentNumberFromGroupID(ushort groupID)
        {
            // The Low order byte is the current fragment number
            return (byte)groupID;
        }

        /// <summary>
        /// Gets the total number of fragments in a fragmented message.
        /// </summary>
        /// <param name="groupID">The group ID of the fragmented message</param>
        /// <returns>The current fragment number</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/29/12 RCG 2.60.07        Created

        public static byte GetNumberOfFragmentsFromGroupID(ushort groupID)
        {
            // The High order byte is the total number of fragments.
            return (byte)(groupID >> 8);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the application profile ID that describes the format of the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort ProfileID
        {
            get
            {
                return m_ProfileID;
            }
            set
            {
                m_ProfileID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Cluster ID for the message
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
        /// Gets or sets the source endpoint number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte SourceEndpoint
        {
            get
            {
                return m_SourceEndpoint;
            }
            set
            {
                m_SourceEndpoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the destination endpoint number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte DestinationEndpoint
        {
            get
            {
                return m_DestinationEndpoint;
            }
            set
            {
                m_DestinationEndpoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the APS frame options
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberApsOptions Options
        {
            get
            {
                return m_Options;
            }
            set
            {
                m_Options = value;
            }
        }

        /// <summary>
        /// Gets or sets the group ID (multicast message)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort GroupID
        {
            get
            {
                return m_GroupID;
            }
            set
            {
                m_GroupID = value;
            }
        }

        /// <summary>
        /// Gets or set the sequence number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte Sequence
        {
            get
            {
                return m_Sequence;
            }
            set
            {
                m_Sequence = value;
            }
        }


        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the raw data for the APS frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        internal byte[] RawData
        {
            get
            {
                byte[] Data = new byte[11];
                MemoryStream DataStream = new MemoryStream(Data);
                BinaryWriter DataWriter = new BinaryWriter(DataStream);

                DataWriter.Write(m_ProfileID);
                DataWriter.Write(m_ClusterID);
                DataWriter.Write(m_SourceEndpoint);
                DataWriter.Write(m_DestinationEndpoint);
                DataWriter.Write((ushort)m_Options);
                DataWriter.Write(m_GroupID);
                DataWriter.Write(m_Sequence);

                return Data;
            }
            set
            {
                if (value != null && value.Length == 11)
                {
                    MemoryStream DataStream = new MemoryStream(value);
                    BinaryReader DataReader = new BinaryReader(DataStream);

                    m_ProfileID = DataReader.ReadUInt16();
                    m_ClusterID = DataReader.ReadUInt16();
                    m_SourceEndpoint = DataReader.ReadByte();
                    m_DestinationEndpoint = DataReader.ReadByte();
                    m_Options = (EmberApsOptions)DataReader.ReadUInt16();
                    m_GroupID = DataReader.ReadUInt16();
                    m_Sequence = DataReader.ReadByte();
                }
                else
                {
                    throw new ArgumentException("The RawData value cannot be null and must be 11 bytes long");
                }
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ProfileID;
        private ushort m_ClusterID;
        private byte m_SourceEndpoint;
        private byte m_DestinationEndpoint;
        private EmberApsOptions m_Options;
        private ushort m_GroupID;
        private byte m_Sequence;

        #endregion
    }
}
