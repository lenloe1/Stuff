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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The CENTRON_AMI class implementation of the ICommModVersions interface.
    /// </summary>
    public partial class CENTRON_AMI : ICommModVersions
    {
        #region Public Properties

        /// <summary>
        /// Gets the Comm module type (IP or RFLAN)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/15/06 AF  7.35.00			Created
        // 07/02/10 AF  2.42.01         Made virtual for M2 Gateway override
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        // 12/17/13 DLG 3.50.16         Moved from ANSIDevice to Centron_AMI.
        //
        public virtual string CommModType
        {
            get
            {
                return Table2108.CommModuleType;
            }
        }

        /// <summary>
        /// Gets the Comm module version.revision
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/15/06 AF  7.35.00			Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        // 03/19/12 JJJ 2.60.xx        Getting data from 2428 in case OpenWay
        //                              FW is not currently running and 2108
        //                              is not populated
        // 10/08/12 jrf 2.61.00 273251 Switching IsInChoiceConnectMode check to check IsChoiceConnectMsmMeter.
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        // 11/27/13 jrf 3.50.10         Moving Bridge meter check to OpenWayITRDBridge/OpenWayITRFBridge.
        // 12/17/13 DLG 3.50.16         Moving from ANSIDevice to Centron_AMI.
        //
        public virtual string CommModVer
        {
            get
            {
                string strValue = "0.000";

                if (null != Table2108)
                {
                    strValue = Table2108.CommModuleVersion;
                }

                return strValue;
            }
        }

        /// <summary>
        /// Gets the Comm Module Version as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/10 RCG 2.40.28			Created
        // 03/19/12 JJJ 2.60.xx        Getting data from 2428 in case OpenWay
        //                              FW is not currently running and 2108
        //                              is not populated.
        // 10/08/12 jrf 2.61.00 273251 Switching IsInChoiceConnectMode check to check IsChoiceConnectMsmMeter.
        // 11/27/13 jrf 3.50.10         Moving Bridge meter check to OpenWayITRDBridge/OpenWayITRFBridge.
        // 12/17/13 DLG 3.50.16         Moving from ANSIDevice to Centron_AMI.
        //
        public virtual byte CommModuleVersion
        {
            get
            {
                byte byValue = 0;

                if (Table2108 != null)
                {
                    byValue = Table2108.CommVersionOnly;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Comm Module Revision as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/10 RCG 2.40.28			Created
        // 03/19/12 JJJ 2.60.xx        Getting data from 2428 in case OpenWay
        //                              FW is not currently running and 2108
        //                              is not populated.
        // 10/08/12 jrf 2.61.00 273251 Switching IsInChoiceConnectMode check to check IsChoiceConnectMsmMeter.
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        // 11/27/13 jrf 3.50.10         Moving Bridge meter check to OpenWayITRDBridge/OpenWayITRFBridge.
        // 12/18/13 DLG 3.50.16         Moved from ANSIDevice to CENTRON_AMI.
        //
        public virtual byte CommModuleRevision
        {
            get
            {
                byte byValue = 0;

                if (Table2108 != null)
                {
                    byValue = Table2108.CommRevisionOnly;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Comm Module Build as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/10 RCG 2.40.28			Created
        // 03/19/12 JJJ 2.60.xx        Getting data from 2428 in case OpenWay
        //                              FW is not currently running and 2108
        //                              is not populated.
        // 10/08/12 jrf 2.61.00 273251 Switching IsInChoiceConnectMode check to check IsChoiceConnectMsmMeter.
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        // 11/27/13 jrf 3.50.10         Moving Bridge meter check to OpenWayITRDBridge/OpenWayITRFBridge.
        // 12/18/13 DLG 3.50.16         Moved from ANSIDevice to CENTRON_AMI.
        //
        public virtual byte CommModuleBuild
        {
            get
            {
                byte byValue = 0;

                if (Table2108 != null)
                {
                    byValue = Table2108.CommBuildOnly;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Comm module build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/15/07 RCG 8.10.05			Created
        // 07/02/10 AF  2.42.01        Made virtual for M2 Gateway override
        // 03/19/12 JJJ 2.60.xx        Getting data from 2428 in case OpenWay
        //                              FW is not currently running and 2108
        //                              is not populated.
        // 10/08/12 jrf 2.61.00 273251 Switching IsInChoiceConnectMode check to check IsChoiceConnectMsmMeter.
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        // 12/18/13 DLG 3.50.16         Moved from ANSIDevice to CENTRON_AMI.
        //
        public virtual string CommModBuild
        {
            get
            {
                return (Table2108 == null) ? null : Table2108.CommModuleBuild;
            }
        }

        #endregion Public Properties
    }
}
