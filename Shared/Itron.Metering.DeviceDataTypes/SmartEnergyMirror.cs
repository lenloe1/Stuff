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
//                              Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.DeviceDataTypes
{

    /// <summary>
    /// Smart Energy Mirror command and Mirror Response
    /// </summary>
    /// 
    public class SmartEnergyMirrorResponse
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 MP                 Created
        
        public SmartEnergyMirrorResponse()
        {
            m_endPointID = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the end point ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 MP                 Created

        public uint EndPointID
        {
            get
            {
                return m_endPointID;
            }
            set
            {
                m_endPointID = value;
            }
        }



        #endregion
        
        #region Member Variables

        private uint m_endPointID;

        #endregion

    }
}
