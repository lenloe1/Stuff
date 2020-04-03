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
//                           Copyright © 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Itron.Metering.Progressable;
using Itron.Metering.Communications.PSEM;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.LandisGyr.Gateway;

namespace Itron.Metering.Device
{
    public partial class M2_Gateway : ICreateEDL
    {
        #region Public Methods

        /// <summary>
        /// Creates an EDL file with the specified sections.
        /// </summary>
        /// <param name="FileName">Path to the file where the EDL file will be written.</param>
        /// <param name="IncludedSections">The sections to include in the EDL file.</param>
        /// <returns>CreateEDLResult Code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/06/10 AF  2.42.02         Created
        //
        public override CreateEDLResult CreateEDLFromMeter(string FileName, EDLSections IncludedSections)
        {
            GatewayTables MeterTables = new GatewayTables();
            
            List<ushort> TablesToRead;
            int iFileNameStart;
            string strDirectory;
            CreateEDLResult Result = CreateEDLResult.SUCCESS;
            PSEMResponse PSEMResult = PSEMResponse.Ok;

            // First check to make sure we can create the file
            iFileNameStart = FileName.LastIndexOf(@"\", StringComparison.Ordinal);

            if (iFileNameStart > 0)
            {
                strDirectory = FileName.Substring(0, iFileNameStart);

                if (Directory.Exists(strDirectory) == false)
                {
                    Result = CreateEDLResult.INVALID_PATH;
                }
            }

            // Make sure we will be able to write to the file
            if (Result == CreateEDLResult.SUCCESS && File.Exists(FileName) == true)
            {
                FileInfo OutputFile = new FileInfo(FileName);

                if ((OutputFile.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    Result = CreateEDLResult.INVALID_PATH;
                }
            }

            if (Result == CreateEDLResult.SUCCESS)
            {
                // Read the data from the meter
                TablesToRead = GetTablesToRead(IncludedSections);

                OnShowProgress(new ShowProgressEventArgs(1, TablesToRead.Count, "Creating EDL file...", "Creating EDL file..."));

                foreach (ushort TableID in TablesToRead)
                {
                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        // Read the table if it exists
                        if ((Table00.IsTableUsed(TableID) == true))
                        {
                            if (MeterTables.GetTableDependencies(TableID).Contains(TableID) || MeterTables.GetTableLength(TableID) > 0)
                            {
                                PSEMResult = ReadTable(TableID, ref MeterTables);

                                if (PSEMResult == PSEMResponse.Bsy
                                    || PSEMResult == PSEMResponse.Dnr
                                    || PSEMResult == PSEMResponse.Iar
                                    || PSEMResult == PSEMResponse.Onp
                                    || PSEMResult == PSEMResponse.Err)
                                {
                                    // We can't read the table but we should be able to continue we just need to
                                    // clear out anything that is there.
                                    MeterTables.ClearTable(TableID);
                                    PSEMResult = PSEMResponse.Ok;
                                }                                 
                            }
                        }

                        OnStepProgress(new ProgressEventArgs());
                    }
                }

                if (PSEMResult == PSEMResponse.Isc)
                {
                    Result = CreateEDLResult.SECURITY_ERROR;
                }
                else if (PSEMResult != PSEMResponse.Ok)
                {
                    Result = CreateEDLResult.PROTOCOL_ERROR;
                }
            }

#if (WindowsCE)
			//The saving of the EDL file on the handheld can take over 6 seconds so we need
			//to send a wait before.
			m_PSEM.Wait(CPSEM.MAX_WAIT_TIME);			
#endif

            // Generate the EDL file
            if (Result == CreateEDLResult.SUCCESS)
            {
                XmlWriterSettings WriterSettings = new XmlWriterSettings();
                WriterSettings.Encoding = Encoding.ASCII;
                WriterSettings.Indent = true;
                WriterSettings.CheckCharacters = false;

                XmlWriter EDLWriter = XmlWriter.Create(FileName, WriterSettings);

                MeterTables.SaveEDLFile(EDLWriter, null, AllowTableExport, AllowFieldExport);
            }

            OnHideProgress(new EventArgs());

            return Result;
        }


        /// <summary>
        /// Creates an EDL file that contains all of the data for a meter.
        /// </summary>
        /// <param name="FileName">The file name that will be used to store the file.</param>
        /// <returns>CreateEDLResult code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/06/10 AF  2.42.02         Created
        //
        public override CreateEDLResult CreateEDLFromMeter(string FileName)
        {
            EDLSections AllSections = EDLSections.HistoryLog
                | EDLSections.NetworkTables
                | EDLSections.LANandHANLog;

            return CreateEDLFromMeter(FileName, AllSections);
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Reads the specified table from the meter.
        /// </summary>
        /// <param name="usTableID">The table ID for the table to read.</param>
        /// <param name="MeterTables">The tables object to read the table into.</param>
        /// <returns>PSEMResponse code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/06/10 AF  2.42.03         Created
        //
        protected PSEMResponse ReadTable(ushort usTableID, ref GatewayTables MeterTables)
        {
            MemoryStream PSEMDataStream;
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] byaData;
            int iReadAttempt = 0;
            bool bRetry = true;

            while (bRetry)
            {
                PSEMResult = m_PSEM.FullRead(usTableID, out byaData);

                if (PSEMResult == PSEMResponse.Ok)
                {
                    PSEMDataStream = new MemoryStream(byaData);
                    MeterTables.SavePSEMStream(usTableID, PSEMDataStream);
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

        /// <summary>
        /// Creates a list of tables to read from the meter.
        /// </summary>
        /// <returns>The list of tables to read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/06/10 AF  2.42.03         Created
        // 07/28/10 AF  2.42.08         Moved table 71 to always read because it is
        //                              needed by 2048 and removed unsupported tables.
        // 03/09/12 AF  2.53.48 194187  Added HAN2 log tables and other missing tables
        //
        protected override List<ushort> GetTablesToRead(EDLSections IncludedSections)
        {
            List<ushort> TableList = new List<ushort>();

            TableList.Add(0);       // General Configuration
            TableList.Add(1);       // Manufacturer Identification
            TableList.Add(4);       // Pending Status
            TableList.Add(5);       // Device Identification
            TableList.Add(6);       // Utility Information
            TableList.Add(30);      // Dimension Display
            TableList.Add(31);      // Actual Display
            TableList.Add(32);      // Display Source
            TableList.Add(34);      // Secondary Display List
            TableList.Add(51);      // Actual Time and TOU
            TableList.Add(52);      // Clock
            TableList.Add(53);      // Time offset
            TableList.Add(55);      // Clock State
            TableList.Add(71);      // Actual Log

            if ((IncludedSections & EDLSections.HistoryLog) == EDLSections.HistoryLog)
            {
                TableList.Add(72);      // Events Identification
                TableList.Add(73);      // History Logger Control
                TableList.Add(74);      // History Logger Data
                TableList.Add(75);      // Event logger control
                TableList.Add(76);      // Event logger data
            }
           
            TableList.Add(120);     // Dim Network Table
            TableList.Add(121);     // Actual Network Table
            TableList.Add(123);     // Exception Report Table

            if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            {
                TableList.Add(122);     // Interface Control Table
                TableList.Add(125);     // Interface Status Table
                TableList.Add(126);     // Registration Status Table
                TableList.Add(127);     // Network Statistics Table
            }

            TableList.Add(2048);    // Manufacturer Configuration Table

            TableList.Add(2062);    // C12.22 Status Table

            if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            {
                TableList.Add(2064);  //Comm Module General Config Table

                if (CommModule is RFLANCommModule)
                {
                    TableList.Add(2078);    // RFLAN Neighbor Table
                }
            }

            if ((IncludedSections & EDLSections.NetworkTables) == EDLSections.NetworkTables)
            {
                TableList.Add(2098);    // HAN Dimension Limiting Table
                TableList.Add(2099);    // Actual HAN Limiting Table
                TableList.Add(2100);    // HAN Client Configuration
                TableList.Add(2102);    // HAN Transmit Data Table
                TableList.Add(2103);    // HAN Recieve Data Table
                TableList.Add(2104);    // HAN Network Info Table
            }

            TableList.Add(2106);    // HAN Config Paramaters
            TableList.Add(2108);    // MCU Information


            if((IncludedSections & EDLSections.LANandHANLog) == EDLSections.LANandHANLog)
            {
                TableList.Add(2158);    // Communications Log Dimension Limiting Table
                TableList.Add(2159);    // Communications Log Actual Limiting Table
                TableList.Add(2160);    // Communications Events Identification Table
                TableList.Add(2161);    // Communications Log Control Table
                TableList.Add(2162);    // LAN Log Data Table
                TableList.Add(2163);    // HAN Communications Log Control Table
                TableList.Add(2164);    // HAN Log Data Table
            }

            TableList.Add(2190);    // Communications Config
            TableList.Add(2193);    // Enhanced Security Table

            TableList.Add(2239);    // Actual HAN Event Log Table
            TableList.Add(2240);    // HAN Events Identification Table
            TableList.Add(2241);    // HAN Events Control Table

            if ((IncludedSections & EDLSections.LANandHANLog) == EDLSections.LANandHANLog)
            {
                TableList.Add(2242);    // HAN Upstream Log Table
                TableList.Add(2243);    // HAN Downstream Log Table
            }

            TableList.Add(2260);    // SR 3.0 Config Table
            TableList.Add(2261);    // Fatal Error Recovery Status Table
            TableList.Add(2265);    // Non Metrological Configuration Data Table
            
            return TableList;
        }

        /// <summary>
        /// Used to determine which tables will be written to the EDL file.
        /// </summary>
        /// <param name="usTableID">Table ID to check.</param>
        /// <returns>True if the table can be written, false otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/06/10 AF  2.42.03         Created
        //
        protected override bool AllowTableExport(ushort usTableID)
        {
            // We are going to control the tables that are written
            // to the EDL file by the tables that we read. This
            // way we only need to change one place whenever new
            // tables are added or removed.
            return true;
        }

        /// <summary>
        /// Determines which fields may be written to the EDL file.
        /// </summary>
        /// <param name="idElement">The field to check.</param>
        /// <param name="anIndex">The indexes into the field.</param>
        /// <returns>True if the field may  be written to the EDL file. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/06/10 AF  2.42.03         Created
        //
        protected override bool AllowFieldExport(long idElement, int[] anIndex)
        {
            return true;
        }

        #endregion
    }
}
