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
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server class for the ITRF advanced poly meter
    /// </summary>
    public partial class OpenWayAdvPolyITRF : OpenWayAdvPoly
    {
        
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/16/11 AF  2.50.04        Created
        //
        public OpenWayAdvPolyITRF(Itron.Metering.Communications.ICommunications ceComm)
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
        public OpenWayAdvPolyITRF(CPSEM PSEM)
            : base(PSEM)
        {
        }        

        #endregion


        #region Public Properties
        

        #endregion


        #region Protected Properties

        /// <summary>
        /// Gets the number of energies supported by this device
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/31/11 RCG 2.50.18        Created

        protected override int NumberOfSupportedEnergies
        {
            get
            {
                return 5;
            }
        }

        #endregion

        #region Members

        #endregion

    }
}
