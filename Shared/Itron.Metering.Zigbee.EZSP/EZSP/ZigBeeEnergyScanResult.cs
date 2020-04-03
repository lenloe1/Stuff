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
    /// Class used to store the Energy Scan Results
    /// </summary>
    public class ZigBeeEnergyScanResult
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ZigBeeEnergyScanResult()
        {
            m_Channel = 0;
            m_MaxRSSIValue = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Channel the results are for.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte Channel
        {
            get
            {
                return m_Channel;
            }
            internal set
            {
                m_Channel = value;
            }
        }

        /// <summary>
        /// Gets the Max RSSI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public sbyte MaxRSSI
        {
            get
            {
                return m_MaxRSSIValue;
            }
            internal set
            {
                m_MaxRSSIValue = value;
            }
        }

        #endregion

        #region Member Variables

        private byte m_Channel;
        private sbyte m_MaxRSSIValue;

        #endregion
    }
}
