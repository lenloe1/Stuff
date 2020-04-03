///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any 
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                           Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
    interface ICentronAMIDeviceStatus
    {
        /// <summary>
        /// Property to retrieve the Number of Inversion tampers
        /// </summary>
        uint NumberOfInversionTampers
        {
            get;
        }

        /// <summary>
        /// Property to retrieve the Number of Removal tampers
        /// </summary>
        uint NumberOfRemovalTampers
        {
            get;
        }

        /// <summary>
        /// Gets the Comm module type (IP or RFLAN)
        /// </summary>
        string CommModType
        {
            get;
        }

        /// <summary>
        /// Gets the Comm module version.revision
        /// </summary>
        string CommModVer
        {
            get;
        }

        /// <summary>
        /// Gets the Comm module build number
        /// </summary>
        string CommModBuild
        {
            get;
        }

        /// <summary>
        /// Gets the Han module type (Zigbee)
        /// </summary>
        string HanModType
        {
            get;
        }

        /// <summary>
        /// Gets the Han module version.revision
        /// </summary>
        string HanModVer
        {
            get;
        }

        /// <summary>
        /// Gets the Han module build number
        /// </summary>
        string HanModBuild
        {
            get;
        }

        /// <summary>
        /// Gets the register module version.revision from MFG Table 2108
        /// </summary>
        string RegModVer
        {
            get;
        }

        /// <summary>
        /// Gets the register module build number
        /// </summary>
        string RegModBuild
        {
            get;
        }

        /// <summary>
        /// Gets the display version.revision fom MFG Table 2108
        /// </summary>
        string DisplayModVer
        {
            get;
        }

        /// <summary>
        /// Retrieves the instantaneous secondary Volts RMS Phase A from the meter.
        /// The firmware folks say this should be considered to be the service voltage.
        /// </summary>
        float ServiceVoltage
        {
            get;
        }
    }
}
