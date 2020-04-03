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
//                            Copyright © 2013 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The ICS_Gateway class implementation of the ICommModVersions interface.
    /// </summary>
    public partial class ICS_Gateway : ICommModVersions
    {
        #region Public Properties

        /// <summary>
        /// Gets the Comm module type (IP or RFLAN)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created.
        // 02/27/14 AF  3.50.39 TR7578 Updated the string returned
        //
        public string CommModType
        {
            get
            {
                return "ICS";
            }
        }

        /// <summary>
        /// Gets the Comm module version.revision
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string CommModVer
        {
            get
            {
                string strCommModVer = "";
                ICSCommModule ICSModule = CommModule as ICSCommModule;

                if (ICSModule != null)
                {
                    strCommModVer = ICSModule.ICMFirmwareVersionMajor.ToString() + "."
                        + ICSModule.ICMFirmwareVersionRevision.ToString("d3", CultureInfo.CurrentCulture);
                }

                return strCommModVer;
            }
        }

        /// <summary>
        /// Gets the Comm Module Version as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/17/13 DLG 3.50.16        Created.
        // 02/27/14 AF  3.50.39 TR7578 Updated to return the actual value
        //
        public byte CommModuleVersion
        {
            get
            {
                ICSCommModule ICSModule = CommModule as ICSCommModule;
                byte version = 0;

                if (ICSModule != null)
                {
                    version = ICSModule.ICMFirmwareVersionMajor;
                }

                return version;
            }
        }

        // TODO: Need to find out where to get this value.
        /// <summary>
        /// Gets the Comm Module Revision as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/17/13 DLG 3.50.16        Created.
        // 02/27/14 AF  3.50.39 TR7578 Updated to return the actual value
        //
        public byte CommModuleRevision
        {
            get
            {
                ICSCommModule ICSModule = CommModule as ICSCommModule;
                byte version = 0;

                if (ICSModule != null)
                {
                    version = ICSModule.ICMFirmwareVersionRevision;
                }

                return version;
            }
        }

        /// <summary>
        /// Gets the Comm Module Build as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/17/13 DLG 3.50.16        Created.
        // 02/27/14 AF  3.50.39 TR7578 Updated to return the actual value
        //
        public byte CommModuleBuild
        {
            get
            {
                ICSCommModule ICSModule = CommModule as ICSCommModule;
                byte version = 0;

                if (ICSModule != null)
                {
                    version = ICSModule.ICMFirmwareVersionMinor;
                }

                return version;
            }
        }

        /// <summary>
        /// Gets the Comm module build number.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/18/13 AF  2.80.08 TR7578 Created.
        //
        public string CommModBuild
        {
            get
            {
                string strCommModBuild = "";
                ICSCommModule ICSModule = CommModule as ICSCommModule;

                if (ICSModule != null)
                {
                    strCommModBuild += ICSModule.ICMFirmwareVersionMinor.ToString("d3", CultureInfo.CurrentCulture);
                }

                return strCommModBuild;
            }
        }

        #endregion Public Properties
    }
}
