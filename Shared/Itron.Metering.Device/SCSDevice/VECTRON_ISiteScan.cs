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
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.IO;
using Itron.Metering.Utilities;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the VECTRON meter. (ISiteScan implemenation)
    /// </summary>
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 04/27/06 mrj 7.30.00  N/A	Created
    /// 05/25/06 jrf 7.30.00  N/A	Modified
    public partial class VECTRON : SCSDevice, ISiteScan
    {
        #region Methods
        /// <summary>
        /// Implements the ISiteScan interface.  Resets the diagnostic counters
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the diagnostic counters cannot be reset in the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/27/06 mrj 7.30.00  N/A   Created
        /// 05/26/06 jrf 7.30.00  N/A   Modified
        /// 
        ItronDeviceResult ISiteScan.ResetDiagCounters()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            byte[] byDiagnostic1To4Counters =
                new byte[VEC_DIAGNOSTIC_1_TO_4_COUNTS_LENGTH];
            byte[] byDiagnostic5Counters =
                new byte[VEC_DIAGNOSTIC_5_COUNTS_LENGTH];
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;
            int iExceptionAddress = (int)VECAddresses.DIAGNOSTICS_COUNTS;

            m_Logger.WriteLine(
                Logger.LoggingLevel.Functional,
                "Starting Reset Diagnositc Counters");

            // Prepare arrays to reset all diagnostic counters
            byDiagnostic1To4Counters.Initialize();
            byDiagnostic5Counters.Initialize();

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reset Diagnostic Counters 1-4");
            // Reset diagnostic 1 to 4 counters 
            ProtocolResponse = m_SCSProtocol.Download(
                (int)VECAddresses.DIAGNOSTICS_COUNTS,
                VEC_DIAGNOSTIC_1_TO_4_COUNTS_LENGTH,
                ref byDiagnostic1To4Counters);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reset Diagnostic 5 Counters");
                // Reset diagnostic 5 counters 
                ProtocolResponse = m_SCSProtocol.Download(
                    (int)VECAddresses.DIAGNOSTIC_5_COUNTS,
                    VEC_DIAGNOSTIC_5_COUNTS_LENGTH,
                    ref byDiagnostic5Counters);
                iExceptionAddress = (int)VECAddresses.DIAGNOSTIC_5_COUNTS;
            }

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                Result = ItronDeviceResult.SUCCESS;
            }
            else if (SCSProtocolResponse.SCS_CAN == ProtocolResponse)
            {
                Result = ItronDeviceResult.ERROR;
            }
            else if (SCSProtocolResponse.NoResponse == ProtocolResponse)
            {
                Result = ItronDeviceResult.ERROR;
            }
            else
            {
                SCSException objSCSException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    iExceptionAddress,
                    m_rmStrings.GetString("RESET_DIAG"));
                throw objSCSException;
            }

            return Result;
        } //End ISiteScan.ResetDiagCounters()

		/// <summary>
		/// Clears the sitescan snapshots in the meter.  This is not supported by
		/// the Vectron.
		/// </summary>
		/// <returns>A ItronDeviceResult</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/21/07 mrj 8.00.13		Created
		//  
		ItronDeviceResult ISiteScan.ClearSiteScanSnapshots()
		{			
			throw( new NotSupportedException("SiteScan Snapshots are not supported"));
		}

        #endregion

        #region Properties
        /// <summary>
        /// Implements the ISiteScan interface. 
        /// </summary>
        /// <returns>A Toolbox object containing
        /// all the toolbox data.</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/27/06 mrj 7.30.00  N/A   Created
        /// 05/26/06 jrf 7.30.00  N/A   Modified
        /// 
        Toolbox ISiteScan.ToolboxData
        {
            get
            {                
                ReadToolboxData();
                return m_Toolbox;
            }
        }

        /// <summary>
        /// Implements the ISiteScan interface.  Returns a Diag object containing
        /// all the diagnostic data.
        /// </summary>
        /// <returns>A CDiagnostics object containing
        /// all the diagnostics data.</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/27/06 mrj 7.30.00  N/A   Created
        /// 05/26/06 jrf 7.30.00  N/A   Modified
        /// 
        CDiagnostics ISiteScan.Diagnostics
        {
            get
            {
                m_Logger.WriteLine(
                    Logger.LoggingLevel.Functional,
                    "Reading Diagnostics");
                ReadDiagnostics();
                return m_Diag;
            }
        }

        /// <summary>
        /// Implements the ISiteScan interface.  Returns the the service type.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/27/06 mrj 7.30.00  N/A   Created
        /// 05/26/06 jrf 7.30.00  N/A   Modified
        /// 06/20/06 jrf 7.30.00  N/A   Made service type a cached value.
        /// 
        string ISiteScan.ServiceType
        {
            get
            {
                if (!m_serviceType.Cached)
                {                    
                    ReadServiceType();
                }
                return m_serviceType.Value;
            }
        }

		/// <summary>
		/// Implements the ISiteScan interface.  Returns the the meter's form.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/06/07 mrj 8.00.10		Created
		//  
		string ISiteScan.MeterForm
		{
			get
			{
				string strForm;

				if (!m_MeterForm.Cached)
				{
					ReadMeterForm();
				}

				switch (m_MeterForm.Value)
				{
					case 0:
					{
						strForm = m_rmStrings.GetString("FORM_9S");
						break;
					}
					case 1:
					{
						strForm = m_rmStrings.GetString("FORM_6S");
						break;
					}
					case 2:
					{
						strForm = m_rmStrings.GetString("FORM_5S");
						break;
					}
					default:
					{
						strForm = m_rmStrings.GetString("SERVICE_TYPE_UNKNOWN");
						break;
					}
				}

				return strForm;
			}
		}

		/// <summary>
		/// Proporty can be used to determine if snapshots are supported.  The
		/// Vectron does not support snapshots.  If this property is ignored
		/// the object will return null for the other properties.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		bool ISiteScan.SnapshotsSupported
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Property to get the number of available snapshots.  The Vectron does
		/// not support snapshots.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/20/07 mrj 8.00.13		Created
		//  
		int ISiteScan.SnapshotCount
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Returns the list of snapshots in the meter.  The Vectron does not support
		/// snapshots.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/20/07 mrj 8.00.13		Created
		//  
		List<SnapshotEntry> ISiteScan.SiteScanSnapshots
		{
			get
			{				
				throw( new NotSupportedException("SiteScan Snapshots are not supported"));
			}
		}

        #endregion       
    }
}