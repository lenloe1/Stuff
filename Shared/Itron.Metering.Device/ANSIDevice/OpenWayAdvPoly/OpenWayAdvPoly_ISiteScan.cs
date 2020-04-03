///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//   embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//storage or retrieval system without the permission in writing from Itron, Inc.
//
//                              Copyright © 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    public partial class OpenWayAdvPoly : ISiteScan
    {
        #region Public Methods

        /// <summary>
        /// Resets the number of diagnostic counters.
        /// </summary>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created

        public ItronDeviceResult ResetDiagCounters()
        {
            return ResetDiagnosticCounters();
        }

        /// <summary>
        /// Clears the SiteScan Snapshots
        /// </summary>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created

        public ItronDeviceResult ClearSiteScanSnapshots()
        {
            throw new NotSupportedException("SiteScan Snapshots are not supported in this meter.");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current diagnostics data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created

        public CDiagnostics Diagnostics
        {
            get 
            {
                CDiagnostics Diags = new CDiagnostics(false, false);

                if (Table2091 != null)
                {
                    Diags.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_1].Active = Table2091.IsDiag1Active;
                    Diags.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_1].Count = Table2091.Diag1Count;

                    Diags.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_2].Active = Table2091.IsDiag2Active;
                    Diags.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_2].Count = Table2091.Diag2Count;

                    Diags.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_3].Active = Table2091.IsDiag3Active;
                    Diags.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_3].Count = Table2091.Diag3Count;

                    Diags.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_4].Active = Table2091.IsDiag4Active;
                    Diags.m_Diags[(int)CDiagnostics.DiagEnum.DIAG_4].Count = Table2091.Diag4Count;
                }

                return Diags;
            }
        }

        /// <summary>
        /// Gets the service type of the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created

        public string ServiceType
        {
            get 
            {
                string strServiceType = m_rmStrings.GetString("UNKNOWN");

                if (Table2091 != null)
                {
                    strServiceType = GetServiceTypeDescription(Table2091.ServiceType);
                }

                return strServiceType;
            }
        }

        /// <summary>
        /// Gets whether or not the meter supports snapshots.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created

        public bool SnapshotsSupported
        {
            get 
            { 
                return false; 
            }
        }

        /// <summary>
        /// Gets the number of available snapshots.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created

        public int SnapshotCount
        {
            get 
            { 
                return 0; 
            }
        }

        /// <summary>
        /// Gets the list of snapshots.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/09 RCG 2.20.00 N/A    Created

        public List<SnapshotEntry> SiteScanSnapshots
        {
            get 
            {
                return null;
            }
        }

        #endregion

        #region Private Methdos

        /// <summary>
        /// Returns a string representing the service type
        /// </summary>
        /// <param name="service"></param>
        /// <returns>Service Type</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/01/09 RCG 2.20.03 N/A    Created

        private string GetServiceTypeDescription(ServiceTypes service)
        {
            string strServiceType;

            //Create the service type string
            switch (service)
            {
                case ServiceTypes.ThreeElem3Phase4WireWYE:
                case ServiceTypes.TwoAndHalfElem3Phase4WireWYE6S46S:
                case ServiceTypes.TwoElemNetwork:
                case ServiceTypes.ThreeElem3Phase4WireDelta:
                case ServiceTypes.TwoElem3Phase4WireWYE:
                case ServiceTypes.TwoElem3Phase3WireDelta:
                case ServiceTypes.TwoElem3Phase4WireDelta:
                case ServiceTypes.TwoElemSinglePhase:
                case ServiceTypes.OneElemSinglePhase3Wire:
                case ServiceTypes.OneElemSinglePhase2Wire:
                case ServiceTypes.TwoAndHalfElem3Phase4WireWYE9S:
                {
                    strServiceType = EnumDescriptionRetriever.RetrieveDescription(service);
                    break;
                }
                case ServiceTypes.AutoServiceSense:
                default:
                {
                    // Since this is the value in use we want to show unknown if 255
                    strServiceType = m_rmStrings.GetString("UNKNOWN");
                    break;
                }
            }

            return strServiceType;
        }

        #endregion
    }
}
