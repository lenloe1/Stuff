///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//   embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//storage or retrieval system without the permission in writing from Itron, Inc.
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
    /// 
    /// </summary>
    public class ZigbeeException : ApplicationException
    {
        private int ErrorCode_c;

        /// <summary>
        /// 
        /// </summary>
        ///<exception></exception>
        /// <example><code></code></example>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ???????? Integration Associates, Jocelyn Bilodeau, Logan Lucas
        // 
        public ZigbeeException()
           :base() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <exception></exception>
        /// <example><code></code></example>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ???????? Integration Associates, Jocelyn Bilodeau, Logan Lucas
        // 
        public ZigbeeException(string message)
           :base(message) {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ErrorCode"></param>
        /// <exception></exception>
        /// <example><code></code></example>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ???????? Integration Associates, Jocelyn Bilodeau, Logan Lucas
        // 
        public ZigbeeException(string message, int ErrorCode)
            : base(message)
        {
            ErrorCode_c = ErrorCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <exception></exception>
        /// <example><code></code></example>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // ???????? Integration Associates, Jocelyn Bilodeau, Logan Lucas
        // 
        public ZigbeeException(string message, Exception inner)
           :base(message, inner) {}

   }
}
