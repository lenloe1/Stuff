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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface which needs to be implemented by devices capable of 
    /// supporting SiteScan.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 04/12/06 mrj 7.30.00    N/A Created
    ///
    public interface ISiteScan
    {
        /// <summary>
        /// Gets the toolbox data
        /// </summary>
        Toolbox ToolboxData
        {
            get;
        }

        /// <summary>
        /// Gets the diagnostics data
        /// </summary>
        CDiagnostics Diagnostics
        {
            get;
        }

        /// <summary>
        /// Gets the service type
        /// </summary>
        string ServiceType
        {
            get;
        }
				
		/// <summary>
		/// Gets the meter form
		/// </summary>
		string MeterForm
		{
			get;
		}

		/// <summary>
		/// Property can be used to determine if snapshots are supported.
		/// </summary>
		bool SnapshotsSupported
		{
			get;
		}

		/// <summary>
		/// Property to get the number of available snapshots.
		/// </summary>
		int SnapshotCount
		{
			get;
		}

		/// <summary>
		/// Returns the list of snapshots in the meter.
		/// </summary>
		List<SnapshotEntry> SiteScanSnapshots
		{
			get;
		}
		
        /// <summary>
        /// Resets the diagnostic counters in the meter.
        /// </summary>
        /// <returns>A ItronDeviceResult</returns>
        ItronDeviceResult ResetDiagCounters();

		/// <summary>
		/// Clears the sitescan snapshots in the meter.
		/// </summary>
		/// <returns>A ItronDeviceResult</returns>
		ItronDeviceResult ClearSiteScanSnapshots();
    }
}