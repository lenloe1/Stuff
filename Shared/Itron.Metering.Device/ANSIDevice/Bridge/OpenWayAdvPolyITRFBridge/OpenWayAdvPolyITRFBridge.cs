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
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server class for the ITRF advanced poly meter
    /// </summary>
    public partial class COpenWayAdvPolyITRFBridge : OpenWayAdvPolyITRF, IVoltMonitorCounts
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/27/13 jrf 3.50.10        Created
        //
        public COpenWayAdvPolyITRFBridge(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
            m_BridgeDevice = BridgeDevice.CreateBridgeDevice(m_PSEM, this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/27/13 jrf 3.50.10        Created
        //
        public COpenWayAdvPolyITRFBridge(CPSEM PSEM)
            : base(PSEM)
        {
            m_BridgeDevice = BridgeDevice.CreateBridgeDevice(m_PSEM, this);
        }

        /// <summary>
        /// Method to determine if device is a bridge meter. Used to determine whether or not to
        /// instantiate this class.
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        ///<param name="securityError">An indication of whether a security error occurred while 
        /// verifying a bridge meter.</param>
        /// <returns>Whether or not device is a bridge meter.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/04/13 jrf 3.50.10        Created
        // 10/31/14 jrf 4.00.82  WR542694 Handling new method signature for IsBridgeMeter.
        public static bool IsBridgeMeter(CPSEM PSEM, out bool securityError)
        {
            return BridgeDevice.IsBridgeMeter(PSEM, out securityError);
        }

        /// <summary>
        /// Reconfigures TOU in the connected meter.
        /// </summary>
        /// <param name="TOUFileName">The filename including path for the 
        /// configuration containing the TOU schedule.</param>
        /// <param name="iSeasonIndex">The number of seasons from the current
        /// season to write.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/27/13 jrf 3.50.10 TQ9523 Created

        public override TOUReconfigResult ReconfigureTOU(string TOUFileName, int iSeasonIndex)
        {
            TOUReconfigResult ReconfigResult = TOUReconfigResult.ERROR;

            if (m_BridgeDevice != null)
            {
                ReconfigResult = m_BridgeDevice.ReconfigureTOU(TOUFileName, iSeasonIndex);
            }

            return ReconfigResult;
        }

#endregion

        #region Protected Methods

        /// <summary>
        /// This method allows derived classes to overried the firmware Type byte that will be passed 
        /// to either the authenticate FWDL procedure or the initiate FWDL procedure.
        /// </summary>
        /// <param name="byCurrentFWType">The firmware image's type.</param>
        /// <returns>The firmware type to use to pass to the authenticate FWDL procedure.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/02/13 jrf 3.50.10          Created.

        protected override byte SelectFWTypeByte(byte byCurrentFWType)
        {
            return ((null == m_BridgeDevice) ? byCurrentFWType : m_BridgeDevice.SelectFWTypeByte(byCurrentFWType));
        }

        /// <summary>
        /// Creates a list of tables to read from the meter when creating EDL file.
        /// </summary>
        /// <param name="IncludedSections">EDL Sections to include</param>
        /// <returns>The list of tables to read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/12/13 jrf 3.50.16 TQ9527   Created.
        //  06/13/14 jrf 3.51.00 WR519359 Bridge meters need to remove certain tables.
        protected override List<ushort> GetTablesToRead(EDLSections IncludedSections)
        {
            List<ushort> TableList = base.GetTablesToRead(IncludedSections);

            if (null != m_BridgeDevice)
            {
                TableList.AddRange(m_BridgeDevice.GetTablesToRead());

                foreach (ushort Table in m_BridgeDevice.GetTablesToRemove())
                {
                    TableList.Remove(Table);
                }
            }

            return TableList;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Comm module version.revision
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10        Created

        public override string CommModVer
        {
            get
            {
                string strVersion = "0.000";

                if (m_BridgeDevice != null)
                {
                    strVersion = m_BridgeDevice.OpenWayCommModVer;
                }

                return strVersion;
            }
        }

        /// <summary>
        /// Gets the Comm Module Version as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10        Created

        public override byte CommModuleVersion
        {
            get
            {
                byte byValue = 0;

                if (m_BridgeDevice != null)
                {
                    byValue = m_BridgeDevice.OpenWayCommModuleVersion;
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
        //  11/27/13 jrf 3.50.10        Created

        public override byte CommModuleRevision
        {
            get
            {
                return ((null == m_BridgeDevice) ? (byte)0 : m_BridgeDevice.OpenWayCommModuleRevision);
            }
        }

        /// <summary>
        /// Gets the Comm Module Build as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/27/13 jrf 3.50.10        Created
        //
        public override byte CommModuleBuild
        {
            get
            {
                return ((null == m_BridgeDevice) ? (byte)0 : m_BridgeDevice.OpenWayCommModuleBuild);
            }
        }

        /// <summary>
        /// Gets the Comm module build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/27/13 jrf 3.50.10        Created
        //
        public override string CommModBuild
        {
            get
            {
                return ((null == m_BridgeDevice) ? null : m_BridgeDevice.OpenWayCommModBuild);
            }
        }

        /// <summary>
        /// Gets the current TOU configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10           Created
        //  01/13/14 jrf 3.50.24 TQ 9478   Making sure base TOUConfiguration is extracted when necessary.
        // 
        public override TOUConfig TOUConfiguration
        {
            get
            {
                TOUConfig TOUConfigData = null;

                if (null != m_BridgeDevice)
                {
                    TOUConfigData = m_BridgeDevice.TOUConfiguration;
                }

                if (null == TOUConfigData)
                {
                    TOUConfigData = base.TOUConfiguration;
                }

                return TOUConfigData;
            }
        }

        /// <summary>
        /// Gets the current TOU configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10        Created
        //  01/13/14 jrf 3.50.24 TQ 9478   Making sure base CalendarConfiguration is extracted when necessary.
        // 
        public override CalendarConfig CalendarConfiguration
        {
            get
            {
                CalendarConfig CalendarConfigData = null;

                if (null != m_BridgeDevice)
                {
                    CalendarConfigData = m_BridgeDevice.CalendarConfiguration;
                }

                if (null == CalendarConfigData)
                {
                    CalendarConfigData = base.CalendarConfiguration;
                }

                return CalendarConfigData;
            }
        }

        /// <summary>
        /// Gets the OpenWayCommModuleRevision
        /// </summary>
        public byte OpenWayCommModuleRevision
        {
            get
            {
                byte returnValue = 0;
                if (m_BridgeDevice != null)
                {
                    returnValue = m_BridgeDevice.OpenWayCommModuleRevision;
                }

                return returnValue;
            }
        }
        #endregion

        #region Members

        private BridgeDevice m_BridgeDevice = null;

        #endregion

        #region IVoltMonitorCounts implementation

        #region Public Properties

        /// <summary>
        /// Gets the number of RMS below threshold counts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/05/13 DLG 3.50.12 TR9480   Created.
        //  
        public int RMSBelowThresholdCount
        {
            get
            {
                return m_BridgeDevice.RMSBelowThresholdCount; 
            }
        }

        /// <summary>
        /// Gets the number of RMS high threshold counts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/05/13 DLG 3.50.12 TR9480   Created.
        //  
        public int RMSHighThresholdCount
        {
            get
            {
                return m_BridgeDevice.RMSHighThresholdCount;
            }
        }

        /// <summary>
        /// Gets the number of Vh below threshold counts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/05/13 DLG 3.50.12 TR9480   Created.
        //  
        public int VhBelowThresholdCount
        {
            get
            {
                return m_BridgeDevice.VhBelowThresholdCount;
            }
        }

        /// <summary>
        /// Gets the number of Vh high threshold counts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/05/13 DLG 3.50.12 TR9480   Created.
        //  
        public int VhHighThresholdCount
        {
            get
            {
                return m_BridgeDevice.VhHighThresholdCount;
            }
        }

        #endregion Public Properties

        #endregion IVoltMonitorCounts implementation
    }
}
