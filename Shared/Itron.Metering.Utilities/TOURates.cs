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
//                              Copyright © 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using Microsoft.Win32;
using System.Globalization;


namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Enumeration of TOU rates.
    /// </summary>
    public enum Rate: byte
    {
        /// <summary>
        /// Rate A
        /// </summary>
        [EnumDescription("Rate A")]
        A = 0,
        /// <summary>
        /// Rate B
        /// </summary>
        [EnumDescription("Rate B")]
        B = 1,
        /// <summary>
        /// Rate C
        /// </summary>
        [EnumDescription("Rate C")]
        C = 2,
        /// <summary>
        /// Rate D
        /// </summary>
        [EnumDescription("Rate D")]
        D = 3,
        /// <summary>
        /// Rate E
        /// </summary>
        [EnumDescription("Rate E")]
        E = 4,
        /// <summary>
        /// Rate F
        /// </summary>
        [EnumDescription("Rate F")]
        F = 5,
        /// <summary>
        /// Rate G
        /// </summary>
        [EnumDescription("Rate G")]
        G = 6,
        /// <summary>
        /// None
        /// </summary>
        [EnumDescription("No Rate")]
        None = 7,
    }
}
