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
//                              Copyright © 2014 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Common.C1219Tables.Centron;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server class for the ITRK polyphase meter
    /// </summary>
    public partial class OpenWayPolyITRK : OpenWayAdvPolyITRF
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  10/03/14 AF  4.00.68  TQ  15236  Created
        //
        public OpenWayPolyITRK(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  10/03/14 AF  4.00.68  TQ  15236  Created
        //  07/23/15 jrf 4.20.18  WR 598314  Instantiating ICSTableReader object.
        public OpenWayPolyITRK(CPSEM PSEM)
            : base(PSEM)
        {
            m_ICSTableReader = new ICSTableReader(PSEM, this);
        }

        /// <summary>
        /// Get the list of configured events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  08/02/16 AF  4.60.02  WR 704676  Created
        //
        public override List<MFG2048EventItem> HistoryLogEventList
        {
            get
            {
                List<MFG2048EventItem> eventList = base.HistoryLogEventList;
                const int LowBatteryEventId = 4040;

                // Remove the inversion tamper event
                for (int index = 0; index < eventList.Count; index++)
                {
                    if (String.Compare(m_rmStrings.GetString("INVERSION_TAMPER"), eventList[index].Description, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        eventList.RemoveAt(index);
                        break;
                    }
                }

                // Remove the removal tamper event. This has to be a separate loop because the indexes are adjusted after the previous removal
                for (int index = 0; index < eventList.Count; index++)
                {
                    if (String.Compare(m_rmStrings.GetString("REMOVAL_TAMPER"), eventList[index].Description, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        eventList.RemoveAt(index);
                        break;
                    }
                }

                ICSCommModule ICSModule = CommModule as ICSCommModule;

                if (ICSModule != null)
                {
                    eventList.AddRange(ICSModule.CommModuleEventConfigured);

                    // Remove the low battery event. The ICM low battery event is only for ITRU and ITRV
                    for (int index = 0; index < eventList.Count; index++)
                    {
                        if (eventList[index].ID == LowBatteryEventId)
                        {
                            eventList.RemoveAt(index);
                            break;
                        }
                    }
                }

                return eventList;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Reads the specified table from the meter.
        /// </summary>
        /// <param name="usTableID">The table ID for the table to read.</param>
        /// <param name="MeterTables">The tables object to read the table into.</param>
        /// <returns>PSEMResponse code.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  07/23/15 jrf 4.20.18 598314   Created to handle ICM table reads.
        public override PSEMResponse ReadTable(ushort usTableID, ref CentronTables MeterTables)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            int iReadAttempt = 0;
            bool bRetry = true;

            while (bRetry)
            {
                switch (usTableID)
                {
                    case 2508:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = m_ICSTableReader.ReadTable2508(ref MeterTables);
                            }

                            break;
                        }
                    case 2511:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = m_ICSTableReader.ReadTable2511(ref MeterTables);
                            }

                            break;
                        }
                    case 2524:
                        {
                            if (PSEMResult == PSEMResponse.Ok)
                            {
                                PSEMResult = m_ICSTableReader.ReadTable2524(ref MeterTables);
                            }

                            break;
                        }
                    default:
                        {
                            PSEMResult = base.ReadTable(usTableID, ref MeterTables);
                            iReadAttempt = 3; //Skipping retries since they will be handled in base class

                            break;
                        }
                }

                iReadAttempt++;

                if (iReadAttempt < 3 && (PSEMResult == PSEMResponse.Bsy || PSEMResult == PSEMResponse.Dnr))
                {
                    bRetry = true;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    bRetry = false;
                }
            }

            return PSEMResult;
        }

        #endregion Protected Methods

        #region Public Properties

        /// <summary>
        ///  Gets the name of the device.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  10/03/14 AF  4.00.68  TQ  15236  Created
        //
        public override string MeterName
        {
            get
            {
                return "OpenWay CENTRON Polyphase Cellular";
            }
        }

        /// <summary>
        /// Get events from the Register (Table 74) and the Comm Module (Table 2524).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  03/26/15 AF  4.10.10 WR573860 Created
        //  
        public override List<HistoryEntry> Events
        {
            get
            {
                List<HistoryEntry> listOfCommEvents = new List<HistoryEntry>(base.Events);
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

        /// <summary>
        /// Gets the meter name that will be used in the activity log.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  10/03/14 AF  4.00.68  TQ  15236  Created
        //
        public override string ActivityLogMeterName
        {
            get
            {
                return "OW CENTRON Poly Cellular";
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Helper class that contains "read table" methods to obtain data from an ICM comm module.
        /// </summary>
        ICSTableReader m_ICSTableReader;

        #endregion Members
    }
}
