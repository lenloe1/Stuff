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
    /// Interface which needs to be implemented by devices that use a Comm Module. 
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue#   Description
    //  -------- --- ------- -------- -------------------------------------------
    //  12/17/13 DLG 3.50.16          Created.
    //  
    public interface ICommModVersions
    {
        #region Properties

        /// <summary>
        /// Gets the Comm module type (IP or RFLAN).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        string CommModType
        {
            get;
        }

        /// <summary>
        /// Gets the Comm module version.revision.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        string CommModVer
        {
            get;
        }

        /// <summary>
        /// Gets the Comm Module Version as a byte.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        byte CommModuleVersion
        {
            get;
        }

        /// <summary>
        /// Gets the Comm Module Revision as a byte.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        byte CommModuleRevision
        {
            get;
        }

        /// <summary>
        /// Gets the Comm Module Build as a byte.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        byte CommModuleBuild
        {
            get;
        }

        /// <summary>
        /// Gets the Comm module build number.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/17/13 DLG 3.50.16          Created.
        //         
        string CommModBuild
        {
            get;
        }

        #endregion Properties
    }
}
