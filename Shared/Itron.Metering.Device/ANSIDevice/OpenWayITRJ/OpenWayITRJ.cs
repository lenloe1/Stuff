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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.Communications.PSEM;
using System.IO;
using Itron.Common.C1219Tables.Centron;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device server class for the ITRJ single phase cellular meter
    /// </summary>
    public partial class OpenWayITRJ : CENTRON_AMI
    {
        #region Constants

        // 09/16/13 mah per WR423280 - Changed the device class name from 'Direct Connect' to 'ICM'
        private const string CENTRONICS_NAME = "OpenWay CENTRON ICM";

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ceComm"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/22/13 jrf 2.80.10        Created
        //
        public OpenWayITRJ(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to identify the meter</param>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 03/22/13 jrf 2.80.10           Created
        // 01/09/14 DLG 3.50.23 TR 9993   Also related to TREQ 9996. Added argument to new parameter
        //                                for ICSTableReader when it's instantiated.
        //
        public OpenWayITRJ(CPSEM PSEM)
            : base(PSEM)
        {
            m_ICSTableReader = new ICSTableReader(PSEM, this);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Creates a list of tables to read from the meter.
        /// </summary>
        /// <param name="IncludedSections">EDL Sections to include</param>
        /// <returns>The list of tables to read.</returns>
        // Revision History	
        // MM/DD/YY Who Version ID Number Description
        // -------- --- ------- -- ------ ----------------------------------------------------------
        // 11/21/13 DLG 3.50.07           Overriding to add ICS related tables.
        // 01/15/14 DLG 3.50.25           Added table 2529.
        // 05/05/15 jrf 4.50.115 WR 583319 Added code to only add ERT tables/update ERT tables 
        //                                 if ERT data is populated.
        // 07/28/15 jrf 4.20.18  WR 599209 Setting ICM event configuration tables to always be included.
        protected override List<ushort> GetTablesToRead(EDLSections IncludedSections)
        {
            List<ushort> TableList = base.GetTablesToRead(IncludedSections);
            DateTime startTime = new DateTime(2000, 1, 1);
            byte byDataRecordCount = 0;
            byte byStatRecordCount = 0;
            byte byDataRecordSize = 0;

            ICSCommModule ICSModule = CommModule as ICSCommModule;

            if (ICSModule != null)
            {
                if (null != ICSModule.IsERTPopulated && true == ICSModule.IsERTPopulated)
                {
                    TableList.Add(2510);            // ICS ERT Dimension Table                 
                    TableList.Add(2509);            // ICS ERT Configuration Table   

                    try
                    {
                        // Send the command to the meter to update the ERT data tables.
                        ICSModule.UpdateERTDataTables(out byDataRecordCount, out byStatRecordCount, out byDataRecordSize);
                    }
                    catch
                    {
                        //Something went wrong. Make sure counts are zero.
                        byDataRecordCount = 0;
                        byStatRecordCount = 0;
                        byDataRecordSize = 0;
                    }

                    //Only add the tables if procedure indicates that there is data in them.
                    if (0 < byDataRecordCount)
                    {
                        TableList.Add(2508);        // ICS ERT Data Table
                    }

                    if (0 < byStatRecordCount)
                    {
                        TableList.Add(2511);        // ICS ERT Statistics Table
                    }
                }

                //Event configuraton should always be included
                TableList.Add(2521);    // ICS Events Actual
                TableList.Add(2522);    // ICS Events ID
                TableList.Add(2523);    // ICS Events Log Control

                if ((IncludedSections & EDLSections.HistoryLog) == EDLSections.HistoryLog)
                {
                    if (Events != null)
                    {                       
                        TableList.Add(2524);    // ICS Log Data Table
                    }
                }

                TableList.Add(2512);            // ICS Module Configuration Table
                TableList.Add(2515);            // ICS Module Data Table
                TableList.Add(2516);            // ICS Module Status Table
                TableList.Add(2517);            // ICS Cellular Configuration Table
                TableList.Add(2518);            // ICS Cellular Data Table
                TableList.Add(2519);            // ICS Cellular Status Table
                TableList.Add(2529);            // LAN Info Table (HAN and ERT)
            }

            return TableList;
        }

        /// <summary>
        /// Reads the specified table from the meter.
        /// </summary>
        /// <param name="usTableID">The table ID for the table to read.</param>
        /// <param name="MeterTables">The tables object to read the table into.</param>
        /// <returns>PSEMResponse code.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/21/13 DLG 3.50.07 No WR    Overriding to handle ICS related tables
        //  05/08/14 AF  3.50.91 WR503773 Added a case for table 2242
        //  05/09/14 AF  3.50.91 WR503773 Removed the case for table 2242. We should be consistent with
        //                                CENTRON_AMI which also does not have a case for 2242.
        //  06/11/14 AF  4.00.25 WR442864 Added back the case for table 2242, due to test failures
        //  07/23/15 jrf 4.20.18 598314   Removing code that is duplicated from base class and 
        //                                calling base.ReadTable(...) in default case to handle those cases.
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
        /// Gets the name of the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/23/13 mah 2.80.41 N/A    Created
        public override string MeterName
        {
            get
            {
                return CENTRONICS_NAME;
            }
        }

        /// <summary>
        /// Get events from the Register (Table 74) and the Comm Module (Table 2524).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/06/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876  Created.
        //  06/10/14 AF  4.00.24 WR518367 Changed the assignment of base.Events to a parameter to the new. Otherwise,
        //                                as reference objects, base.Events was getting the comm module events appended.
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
        /// Get the list of configured events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/07/13 DLG 3.50.01 TREQs 7587, 9509, 9520, 7876  Created.
        //  04/08/14 AF  3.50.67 WR488011 Removed the removal tamper and inversion tamper from the history log
        //                                event list.  Those will be handled by the comm module and we don't want
        //                                duplicate events to be shown.
        //  05/05/14 AF  3.50.91 WR488011 Removed the ICM low battery event. It is valid only for ITRU and ITRV and should
        //                                not be shown for ITRJ.
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

        #endregion Public Properties

        #region Members

        /// <summary>
        /// Helper class that contains "read table" methods to obtain ERT data from an ICM.
        /// </summary>
        ICSTableReader m_ICSTableReader;

        #endregion Members
    }
}
