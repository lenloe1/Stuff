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

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface that needs to be implemented for devices support Voltage Monitor Counts
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue#   Description
    //  -------- --- ------- -------- -------------------------------------------
    //  12/05/13 DLG 3.50.12 TR9480   Created.
    //  
    public interface IVoltMonitorCounts
    {
        #region Properties

        /// <summary>
        /// Gets the number of RMS below threshold counts.
        /// </summary>
        int RMSBelowThresholdCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of RMS high threshold counts.
        /// </summary>
        int RMSHighThresholdCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of Vh below threshold counts.
        /// </summary>
        int VhBelowThresholdCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of Vh high threshold counts.
        /// </summary>
        int VhHighThresholdCount
        {
            get;
        }

        #endregion Properties
    }
}