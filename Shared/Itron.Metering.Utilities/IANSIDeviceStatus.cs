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
//                           Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Interface to the Device Status properties exposed by ANSI Device
    /// </summary>
    public interface IANSIDeviceStatus
    {
        /// <summary>
        /// Property to get the hardware version from table 01. The hardware
        /// version is specific to ANSI meters; SCS meters do not
        /// need this item.
        /// </summary>
        float HWRevision
        {
            get;
        }

        /// <summary>
        /// Property to determine if the meter is in DST
        /// </summary>
        bool IsMeterInDST
        {
            get;
        }

        /// <summary>
        /// Gets the Date of the Last 
        /// </summary>
        DateTime DateLastTestMode
        {
            get;
        }
    }
}
