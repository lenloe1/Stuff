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
//                              Copyright © 2013 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server class for the ITRU single phase cellular I-210+c meter with Superior firmware
    /// </summary>
    public partial class OpenWayITRU : ICS_Gateway
    {
        #region Constants

        private const string OPENWAYICS_NAME = "OpenWay Single Phase ICM";

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/05/13 AF  3.50.01 TR9507, 9508 Created
        // 01/06/14 DLG 3.50.19        Added member variable to instantiate the Common Firmware
        //                             Download object.
        //
        public OpenWayITRU(CPSEM PSEM)
            : base(PSEM)
        {
            m_HANInfo = new HANInformation(PSEM, this);
            m_CommonFWDL = new CommonFirmwareDownload(PSEM, this, m_Logger);
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/05/13 AF  3.50.01  TR9507,9508  Created
        //
        public override string MeterName
        {
            get
            {
                return OPENWAYICS_NAME;
            }
        }

        #endregion Public Properties

        #region Members

        /// <summary>
        /// The HANInformation object.
        /// </summary>
        private HANInformation m_HANInfo = null;
        /// <summary>
        /// The CommonFirmwareDownload object.
        /// </summary>
        private CommonFirmwareDownload m_CommonFWDL = null;

        #endregion Members
    }
}