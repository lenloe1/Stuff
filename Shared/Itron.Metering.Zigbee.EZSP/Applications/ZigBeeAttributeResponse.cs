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
    /// Class used to store the the response to an individual attribute
    /// </summary>
    public class ZigBeeAttributeResponse
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.27        Created
        
        public ZigBeeAttributeResponse()
        {
            m_AttributeID = 0;
            m_Status = ZCLStatus.UnsupportedAttribute;
            m_DataType = ZCLDataType.Null;
            m_Value = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the ID of the attribute that was read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.27        Created
        
        public ushort AttributeID
        {
            get
            {
                return m_AttributeID;
            }
            internal set
            {
                m_AttributeID = value;
            }
        }

        /// <summary>
        /// Gets the status of the attribute read
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.27        Created
        
        public ZCLStatus Status
        {
            get
            {
                return m_Status;
            }
            internal set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Gets the data type of the attribute
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.27        Created
        
        public ZCLDataType DataType
        {
            get
            {
                return m_DataType;
            }
            internal set
            {
                m_DataType = value;
            }
        }

        /// <summary>
        /// Gets the value of the attribute
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/14/11 RCG 2.52.27        Created
        
        public object Value
        {
            get
            {
                return m_Value;
            }
            internal set
            {
                m_Value = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_AttributeID;
        private ZCLStatus m_Status;
        private ZCLDataType m_DataType;
        private object m_Value;

        #endregion
    }
}
