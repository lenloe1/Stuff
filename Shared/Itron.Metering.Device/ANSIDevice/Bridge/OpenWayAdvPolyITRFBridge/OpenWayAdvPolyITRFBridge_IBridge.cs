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
//                         Copyright © 2013 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The ITRF polyphase bridge meter device server class implementation of the IBridge interface.
    /// </summary>
    public partial class COpenWayAdvPolyITRFBridge : IBridge
    {
        #region Public Methods

        /// <summary>
        /// Switches the Comm Operational Mode in an Bridge capable meter.
        /// </summary>
        /// <param name="opMode">The Comm Operational Mode to which the meter should switch</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/02/13 jrf 3.50.10        Created.

        public ProcedureResultCodes SwitchCommOperationMode(OpenWayMFGTable2428.ChoiceConnectCommOpMode opMode)
        {
            return (null == m_BridgeDevice) ? ProcedureResultCodes.DEVICE_SETUP_CONFLICT : m_BridgeDevice.CommOpModeSwitch(opMode);
        }

        /// <summary>
        /// Method causes state and/or time sensitive ChoiceConnect table data to be refreshed
        /// when their data is next accessed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  12/02/13 jrf 3.50.10        Created.

        public void RefreshChoiceConnectData()
        {
            if (null != m_BridgeDevice) { m_BridgeDevice.RefreshChoiceConnectTableData(); }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets whether the meter is a Bridge meter that was released 
        /// during the initial Bridge project (Phase 1).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/27/13 jrf 3.50.10 	    Created
        //
        public bool IsBridgePhase1Meter
        {
            get
            {
                return (null == m_BridgeDevice) ? false : m_BridgeDevice.IsBridgePhase1Meter;
            }
        }

        /// <summary>
        /// Gets the mode the Bridge meter was manufactured as.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/17/14 jrf 4.00.89 TBD    Created.
        //
        public OpenWayMFGTable2428.ChoiceConnectCommMfgMode ChoiceConnectManufacturedMode
        {
            get
            {
                return (null == m_BridgeDevice) ? OpenWayMFGTable2428.ChoiceConnectCommMfgMode.UnknownManufacturingMode : m_BridgeDevice.ChoiceConnectCommModuleManufacturedMode;
            }
        }

        /// <summary>
        /// Gets the register's current communications operating mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/13/13 jrf 3.50.03 	    Created
        //
        public OpenWayMFGTable2428.ChoiceConnectCommOpMode CurrentRegisterCommOpMode
        {
            get
            {
                OpenWayMFGTable2428.ChoiceConnectCommOpMode CommOpMode = OpenWayMFGTable2428.ChoiceConnectCommOpMode.UnknownOperationalMode;

                if (m_BridgeDevice != null)
                {
                    CommOpMode = m_BridgeDevice.CurrentRegisterCommOpMode;
                }

                return CommOpMode;
            }
        }

        /// <summary>
        /// Gets the register's current communications operating mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/13/13 jrf 3.50.03 	    Created
        //
        public OpenWayMFGTable2428.ChoiceConnectCommOpMode RequestedRegisterCommOpMode
        {
            get
            {
                return (null == m_BridgeDevice) ? OpenWayMFGTable2428.ChoiceConnectCommOpMode.UnknownOperationalMode
                    : m_BridgeDevice.RequestedRegisterCommOpMode;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Firmware Version.Revision string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 jrf 3.50.10        Created.

        public string ChoiceConnectFWVerRev
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.ChoiceConnectFWVerRev;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Firmware Build string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 jrf 3.50.10        Created.

        public string ChoiceConnectFWBuild
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.ChoiceConnectFWBuild;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM ERT ID as a formatted string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 jrf 3.50.10        Created.

        public string ChoiceConnectERTID
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.ChoiceConnectERTID;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Bubble-up LID translated as a string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/27/13 jrf 3.50.10 	    Created

        public string ChoiceConnectBubbleUpLIDDescription
        {
            get
            {
                string strValue = null;

                if (m_BridgeDevice != null)
                {
                    strValue = m_BridgeDevice.ChoiceConnectBubbleUpLIDDescription;
                }

                return strValue;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Security State as a formatted string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/13/13 jrf 3.50.03 	    Created

        public string ChoiceConnectSecurityStateDescription
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.ChoiceConnectSecurityStateDescription;
            }
        }

        /// <summary>
        ///  Gets whether or not the 25 Year TOU schedule is supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/08/14 AF  3.50.22 TQ9484 Created
        //
        public bool Supports25YearTOUSchedule
        {
            get
            {
                return (null == m_BridgeDevice) ? false : m_BridgeDevice.Supports25YearTOUSchedule;
            }
        }

        /// <summary>
        /// Checks the meter's configuration to make sure that it is compatible with
        /// ChoiceConnect
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/14 AF  3.50.25 TQ9489 Created
        //
        public bool IsConfigChoiceConnectCompatible
        {
            get
            {
                // If it's not a Bridge meter, then compatibility doesn't matter
                return (null == m_BridgeDevice) ? true : m_BridgeDevice.IsConfigChoiceConnectCompatible;
            }
        }

        #endregion
    }
}
