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
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server class for the ITRH single phase cellular I-210+c meter with Michigan firmware
    /// </summary>
    public partial class OpenWayITRH : ICS_Gateway
    {
        #region Constants

        private const string OPENWAYICS_NAME = "OpenWay Single Phase ICM";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/05/13 AF  3.50.01 TR9507,9508 Created
        //
        public OpenWayITRH(CPSEM PSEM)
            : base(PSEM)
        {
        }

        #endregion

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

        /// <summary>
        /// Get events from the Comm Module (Table 2524).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/06/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876  Created.
        //  
        public override List<HistoryEntry> Events
        {
            get
            {
                List<HistoryEntry> listOfCommEvents = new List<HistoryEntry>();
                ProcedureResultCodes Result = ProcedureResultCodes.UNRECOGNIZED_PROC;
                ICSCommModule ICSModule = CommModule as ICSCommModule;
                byte[] commandResponse;

                if (ICSModule != null)
                {
                    Result = ICSModule.UpdateEventTables(new DateTime(1970, 1, 1), DateTime.MaxValue, out commandResponse);

                    if (Result == ProcedureResultCodes.COMPLETED)
                    {
                        listOfCommEvents.AddRange(ICSModule.CommModuleEvents);
                    }
                }

                return listOfCommEvents;
            }
        }

        #endregion
    }
}