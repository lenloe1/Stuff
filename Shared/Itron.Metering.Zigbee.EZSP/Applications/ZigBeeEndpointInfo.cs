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

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// Endpoint Information for use with an EZSP based ZigBee Application
    /// </summary>
    public class ZigBeeEndpointInfo
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ZigBeeEndpointInfo()
        {
            m_Endpoint = 0;
            m_ProfileID = 0;
            m_DeviceID = 0;
            m_AppFlags = 0;
            m_ServerClusterList = new List<ushort>();
            m_ClientClusterList = new List<ushort>();
            m_ServerEndpointMapping = new Dictionary<ushort, List<byte>>();
            m_ClientEndpointMapping = new Dictionary<ushort, List<byte>>();
        }

        /// <summary>
        /// Finds the endpoint on the specified device where the specified cluster is located
        /// </summary>
        /// <param name="destination">The device to get the endpoint on</param>
        /// <param name="clusterID">The cluster ID to get the endpoint for</param>
        /// <returns>0 if the endpoint does not exist or the endpoint of the cluster</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte FindMatchingServerEndpoint(ushort destination, ushort clusterID)
        {
            // The value should never be 0 so lets use that to indicate that it's not present
            byte FoundEndpoint = 0;

            if(m_ServerEndpointMapping.ContainsKey(destination) && m_ServerClusterList.Contains(clusterID))
            {
                // We should have a mapping stored
                List<byte> DestinationMapping = m_ServerEndpointMapping[destination];

                FoundEndpoint = DestinationMapping[m_ServerClusterList.IndexOf(clusterID)];
            }

            return FoundEndpoint;
        }

        /// <summary>
        /// Finds the endpoint on the specified device where the specified cluster is located
        /// </summary>
        /// <param name="destination">The device to get the endpoint on</param>
        /// <param name="clusterID">The cluster ID to get the endpoint for</param>
        /// <returns>0 if the endpoint does not exist or the endpoint of the cluster</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte FindMatchingClientEndpoint(ushort destination, ushort clusterID)
        {
            // The value should never be 0 so lets use that to indicate that it's not present
            byte FoundEndpoint = 0;

            if (m_ClientEndpointMapping.ContainsKey(destination) && m_ClientClusterList.Contains(clusterID))
            {
                // We should have a mapping stored
                List<byte> DestinationMapping = m_ClientEndpointMapping[destination];

                FoundEndpoint = DestinationMapping[m_ClientClusterList.IndexOf(clusterID)];
            }

            return FoundEndpoint;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Endpoint number that hosts these clusters
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

        /// <summary>
        /// Gets the ZigBee Profile that defines the clusters
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
        /// Gets the ID of the hosted device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort DeviceID
        {
            get
            {
                return m_DeviceID;
            }
            set
            {
                m_DeviceID = value;
            }
        }

        /// <summary>
        /// Gets the Application Flags
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte AppFlags
        {
            get
            {
                return m_AppFlags;
            }
            set
            {
                m_AppFlags = value;
            }
        }

        /// <summary>
        /// Gets the list of server side Clusters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public List<ushort> ServerClusterList
        {
            get
            {
                return m_ServerClusterList;
            }
            set
            {
                m_ServerClusterList = value;
            }
        }

        /// <summary>
        /// Gets the list of client side clusters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public List<ushort> ClientClusterList
        {
            get
            {
                return m_ClientClusterList;
            }
            set
            {
                m_ClientClusterList = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of Matched Endpoints for each Client Cluster supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public Dictionary<ushort, List<byte>> ServerEndpointMapping
        {
            get
            {
                return m_ServerEndpointMapping;
            }
        }

        /// <summary>
        /// Gets or sets the list of Matched Endpoints for each Client Cluster supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public Dictionary<ushort, List<byte>> ClientEndpointMapping
        {
            get
            {
                return m_ClientEndpointMapping;
            }
        }

        #endregion

        #region Member Variables

        private byte m_Endpoint;
        private ushort m_ProfileID;
        private ushort m_DeviceID;
        private byte m_AppFlags;
        private List<ushort> m_ServerClusterList;
        private List<ushort> m_ClientClusterList;
        private Dictionary<ushort, List<byte>> m_ServerEndpointMapping;
        private Dictionary<ushort, List<byte>> m_ClientEndpointMapping;

        #endregion
    }
}
