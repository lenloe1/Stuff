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
//                              Copyright © 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    public partial class OpenWayPolyITRK
    {
        #region Protected Methods

        /// <summary>
        /// Creates a list of tables to read from the meter.
        /// </summary>
        /// <param name="IncludedSections">The sections to include in the file</param>
        /// <returns>The list of tables to read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  06/24/15 AF  4.20.14  WR 593034,593045 Created to include the ICM tables in the EDL file
        //  07/28/15 jrf 4.20.18  WR 599209 Setting ICM event configuration tables to always be included.
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

        #endregion
    }
}
