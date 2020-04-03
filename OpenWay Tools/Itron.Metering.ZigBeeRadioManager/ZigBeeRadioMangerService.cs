///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential  
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or  
//  otherwise. Including photocopying and recording or in connection with any 
//  information storage or retrieval system without the permission in writing 
//  from Itron, Inc.
//
//                              Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using Itron.Metering.ZigBeeRadioServerObjects;

namespace Itron.Metering.ZigBeeRadioManager
{
    /// <summary>
    /// Windows Service class that hosts a WCF service that will manage ZigBee radios for all
    /// other applications.
    /// </summary>
    public partial class ZigBeeRadioMangerService : ServiceBase
    {

        #region Public Methods

        /// <summary>
        /// Default constructor
        /// </summary>
 		//  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/31/08 RCG 1.00           Created

        public ZigBeeRadioMangerService()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs any operations needed to start the Windows Service.
        /// </summary>
        /// <param name="args">The arguments for starting the Windows Service</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/31/08 RCG 1.00           Created

        protected override void OnStart(string[] args)
        {
            m_RadioManagementService = new ZigBeeRadioService();
            m_ServiceHost = new ServiceHost(m_RadioManagementService);
            m_ServiceHost.Open();
        }

        /// <summary>
        /// Performs any operations needed to stop the Windows Service.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/31/08 RCG 1.00           Created

        protected override void OnStop()
        {
            m_ServiceHost.Close();
            m_RadioManagementService = null;
            m_ServiceHost = null;
        }

        #endregion

        #region Member Variables

        private ZigBeeRadioService m_RadioManagementService;
        private ServiceHost m_ServiceHost;

        #endregion
    }
}
