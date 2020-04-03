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
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server class for the ITRD single phase meter
    /// </summary>
    public partial class OpenWayITRD : CENTRON_AMI
    {
        #region Constants
        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/17/11 AF  2.50.04        Created
        //
        public OpenWayITRD(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  02/17/11 AF  2.50.04        Created
        //
        public OpenWayITRD(CPSEM PSEM)
            : base(PSEM)
        {
        }        

        #endregion

        #region Public Properties        

        #endregion

        #region Events
        #endregion

        #region Internal Methods
        #endregion

        #region Internal Properties
        #endregion

        #region Protected Methods
        #endregion

        #region Protected Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Members

        #endregion

    }
}
