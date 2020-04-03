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

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Interface for objects that are able to raise fatal and non-fatal error events
    /// </summary>
    public interface IErrorHandling
    {
        /// <summary>
        /// An event that is raised when a fatal error occurs
        /// </summary>
        event FatalErrorEventHandler FatalErrorEvent;

        /// <summary>
        /// An event that is raised when a non fatal error occurs
        /// </summary>
        event NonFatalErrorEventHandler NonFatalErrorEvent;
    }
}
