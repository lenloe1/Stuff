///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential  
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or  
//  otherwise. Including photocopying and recording or in connection with any 
//  information storage or retrieval system without the permission in writing 
//  from Itron, Inc.
//
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// Represents a Zigbee node on a network.
    /// </summary>
    public class ZigbeeNode
    {
        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public ulong IEEEAddress
        {
            get { return m_IEEEAddress; }
            set { m_IEEEAddress = value; }
        }

        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public ushort ShortAddr
        {
            get { return m_ShortAddr; }
            set { m_ShortAddr = value; }
        }


        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public ZigbeeLogicalType LogicalType
        {
            get { return m_DeviceLogicalType; }
            set { m_DeviceLogicalType = value; }
        }


        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public bool AlternatePan
        {
            get { return m_AlternatePan; }
            set { m_AlternatePan = value; }
        }


        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public bool FFD
        {
            get { return m_FFD; }
            set { m_FFD = value; }
        }


        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public bool MainSource
        {
            get { return m_MainSource; }
            set { m_MainSource = value; }
        }


        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public bool OnIdle
        {
            get { return m_OnIdle; }
            set { m_OnIdle = value; }
        }


        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public bool Security
        {
            get { return m_Security; }
            set { m_Security = value; }
        }


        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public ushort ManufacturerCode
        {
            get { return m_ManufacturerCode; }
            set { m_ManufacturerCode = value; }
        }

        /// <summary>
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/14/06 mcm 8.00.00 N/A	   Created  
        //
        public ZigbeeDeviceType DeviceType
        {
            get
            {
                return (ZigbeeDeviceType)
                    ((m_IEEEAddress & DEVICE_TYPE_MASK) >> DEVICE_TYPE_SHIFT);
            }
        }


        private const ulong DEVICE_TYPE_MASK = 0x000000FF00000000;
        private const int DEVICE_TYPE_SHIFT = 32;


        private ulong m_IEEEAddress;
        private ushort m_ShortAddr;
        private ZigbeeLogicalType m_DeviceLogicalType;
        private bool m_AlternatePan;
        private bool m_FFD;
        private bool m_MainSource;
        private bool m_OnIdle;
        private bool m_Security;
        private ushort m_ManufacturerCode;
    }
}
