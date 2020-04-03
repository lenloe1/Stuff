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
    /// General Itron Device Results
    /// </summary>
    public enum ZigbeeResult : byte
    {
        /// <summary>
        /// SUCCESS = 0
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// ERROR = 1
        /// </summary>
        ERROR = 1,
        /// <summary>
        /// UNSUPPORTED_OPERATION = 2
        /// </summary>
        ACKNOWLEDGED_NO_DATA = 2,
        /// <summary>
        /// SECURITY_ERROR = 3, insufficient security clearance
        /// </summary>
        SECURITY_ERROR = 3,
        /// <summary>
        /// ACKNOWLEDGED_INSUFFICIENT_DATA = 4
        /// </summary>
        ACKNOWLEDGED_INSUFFICIENT_DATA = 4,
        /// <summary>
        /// NOT_CONNECTED = 99
        /// Must connect to hardware before using it
        /// </summary>
        NOT_CONNECTED = 99,
    }
}
