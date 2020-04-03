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
//                            Copyright © 2013 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;

using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class to handle all functionality common to Bridge devices.
    /// </summary>
    public class BridgeDevice
    {
        #region Constants

        private const ushort TOU_25_YEAR_TABLE = 2437;

        /// <summary>
        /// Constant describing the firmware version for Bridge Phase 1 meters
        /// </summary>
        private const float VERSION_3_32_BRIDGE_PHASE_1 = 3.032F;

        /// <summary>
        /// The maximum number of years in the calendar configuration.
        /// </summary>
        public const int MAX_CALENDAR_YEARS = 25;
        /// <summary>
        /// The maximum number of events per year in the calendar configuration.
        /// </summary>
        public const int MAX_CALENDAR_EVENTS_PER_YEAR = 32;
        /// <summary>
        /// The base year to use for events in the calendar configuration.
        /// </summary>
        public const int CALENDAR_YEAR_BASE = 2000;
        /// <summary>
        /// The maximum number of seasons in the TOU configuration.
        /// </summary>
        public const int MAX_SEASONS = 8;
        /// <summary>
        /// The maximum number of patterns in the TOU configuration.
        /// </summary>
        public const int MAX_DAY_OF_EVENTS = 4;
        /// <summary>
        /// The maximum nuber of switchpoints in the TOU configuration
        /// </summary>
        public const int MAX_TIME_OF_DAY_EVENTS = 6;
        /// <summary>
        /// 
        /// </summary>
        public const string UNUSED_CALENDAR_EVENT = "Undefined Event - 0 - 01/01";
        /// <summary>
        /// 
        /// </summary>
        public const string UNUSED_SWITCHPOINT = "Undefined Event - 0 - 00:00";
        
        private const byte MONTHS_IN_YEAR = 12;
        private const byte MAX_DAYS_IN_MONTH = 31;

        private const string TOU_25_YEAR_SCHEDULE_CATEGORY = "25 Year TOU Schedule";
        

        #endregion

        #region Definitions

        private enum CalendarControlType
        {
            DelaySeasonChangeUntilDemandReset = 0,
            ForceDemandResetOnSeasonChange = 1,
            SeasonChangeWithoutDemandReset = 2,
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PSEM">Protocol obj used to communicate with the meter</param>
        /// <param name="OpenWayDevice">CENTRON_AMI device object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/26/13 jrf 3.50.10        Created
        // 12/05/13 DLG 3.50.12 TR9480 Added member variables to initialize.
        //
        internal BridgeDevice(CPSEM PSEM, CENTRON_AMI OpenWayDevice)            
        {
            m_PSEM = PSEM;
            m_OpenWayDevice = OpenWayDevice;
            m_RMSBelowThreshold = new CachedInt();
            m_RMSHighThreshold = new CachedInt();
            m_VhBelowThreshold = new CachedInt();
            m_VhHighThreshold = new CachedInt();
        }

        /// <summary>
        /// Creates the bridge device if appropriate. 
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="OpenWayDevice">The Device Object for the current device</param>
        /// <returns>The Comm Module object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/26/13 jrf 3.50.10        Created
        // 10/31/14 jrf 4.00.82  WR542694 Handling new method signature for IsBridgeMeter.
        internal static BridgeDevice CreateBridgeDevice(CPSEM psem, CENTRON_AMI OpenWayDevice)
        {
            BridgeDevice NewBridgeDevice = null;
            bool SecurityError;

            if (IsBridgeMeter(psem, out SecurityError))
            {
                NewBridgeDevice = new BridgeDevice(psem, OpenWayDevice);
            }

            return NewBridgeDevice;
        }

        /// <summary>
        /// Method to determine if device is a bridge meter. 
        /// </summary>
        /// <param name="psem">Protocol obj used to identify the meter</param>
        /// <param name="securityError">An indication of whether a security error occurred while 
        /// verifying a bridge meter.</param>
        /// <returns>Whether or not device is a bridge meter.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/04/13 jrf 3.50.10        Created
        // 10/31/14 jrf 4.00.82  WR542694 Handling exceptions and returning an indication of when a PSEM security error occurs.
        // 11/07/14 jrf 4.00.87 WR TBD Handling new manufacutring mode indicator for Bridge direct RF Mesh migration.
        // 11/19/14 jrf 4.00.89 TBD    Renamed value in ChoiceConnectCommMfgMode enum.
        internal static bool IsBridgeMeter(CPSEM psem, out bool securityError)
        {
            bool blnIsBridgeMeter = false;
            securityError = false;

            try
            {     
                CTable00 Table0 = new CTable00(psem);
                OpenWayMFGTable2428 Table2428 = null;

                if (null != Table0 && Table0.IsTableUsed(2428))
                {
                    Table2428 = new OpenWayMFGTable2428(psem);

                    if (OpenWayMFGTable2428.ChoiceConnectCommMfgMode.ChoiceConnectManufacturingModeRFLAN == Table2428.ManufacturedMode
                        || OpenWayMFGTable2428.ChoiceConnectCommMfgMode.ChoiceConnectManufacturingModeRFMesh == Table2428.ManufacturedMode)
                    {
                        blnIsBridgeMeter = true;
                    }
                }
            }
            catch (PSEMException PSEMErr)
            {
                if (PSEMResponse.Isc == PSEMErr.PSEMResponse)
                {
                    securityError = true;
                }

                blnIsBridgeMeter = false; 
            }
            catch
            {
                blnIsBridgeMeter = false;
            }

            return blnIsBridgeMeter;
        }

        /// <summary>
        /// Switches the Comm Operational Mode in an MSM capable meter.
        /// </summary>
        /// <param name="opMode">The Comm Operational Mode to which the meter should switch</param>
        /// <returns>The result of the procedure call</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/12/12 JJJ 2.60.xx	    Created
        //  12/02/13 jrf 3.50.10        Moved here from CENTRON_AMI.

        internal ProcedureResultCodes CommOpModeSwitch(OpenWayMFGTable2428.ChoiceConnectCommOpMode opMode)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcResponse;
            byte[] ProcParam = new byte[1];

            ProcParam[0] = (byte)opMode;

            ProcResult = m_OpenWayDevice.ExecuteProcedure(Procedures.SWITCH_CHOICE_CONNECT_OPENWAY_COMM_MODE, ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Method causes state and/or time sensitive ChoiceConnect table data to be refreshed
        /// when their data is next accessed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/14/12 JJJ 2.60.xx        Created
        // 12/02/13 jrf 3.50.10        Moved here from CENTRON_AMI.

        internal void RefreshChoiceConnectTableData()
        {
            if (null != Table2428) { Table2428.Refresh(); }         
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

        internal TOUReconfigResult ReconfigureTOU(string TOUFileName, int iSeasonIndex)
        {
            TOUReconfigResult ReconfigResult = TOUReconfigResult.ERROR;

            if (Supports25YearTOUSchedule)
            {
                ReconfigResult = Write25YearTOU(TOUFileName);
            }

            if (null != m_OpenWayDevice)
            {
                ReconfigResult = CENTRON_AMI.ConvertWritePendingTOUResult(m_OpenWayDevice.WritePendingTOU(TOUFileName, iSeasonIndex));
            }

                     

            return ReconfigResult;
        }

        /// <summary>
        /// Creates a list of tables to read from the meter when creating EDL file.
        /// </summary>
        /// <returns>The list of tables to read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/02/13 jrf 3.50.10 TQ9527   Created.
        //  12/11/13 jrf 3.50.14 TQ9479   Added bridge operational state table.
        //  12/30/13 jrf 3.50.16 TQ9557   Added last season register data.
        //
        internal List<ushort> GetTablesToRead()
        {
            List<ushort> TableList = new List<ushort>();

            TableList.Add(24); //Last Season Register Data

            TableList.Add(2428);    //Bridge Operational State 
            
            if (OpenWayMFGTable2428.ChoiceConnectCommOpMode.ChoiceConnectOperationalMode 
                == CurrentRegisterCommOpMode)
            {
                TableList.Add(2437);    //25 Year TOU Calendar                
            }

            return TableList;
        }        

        /// <summary>
        /// Creates a list of tables to read from the meter when creating EDL file.
        /// </summary>
        /// <returns>The list of tables to read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/13/14 jrf 3.51.00 WR519359 Created.
        //
        internal List<ushort> GetTablesToRemove()
        {
            List<ushort> TableList = new List<ushort>();

            if (OpenWayMFGTable2428.ChoiceConnectCommOpMode.ChoiceConnectOperationalMode 
                == CurrentRegisterCommOpMode)
            {
                //Removing RFLAN specific tables when in CC mode. They aren't supported.
                
                TableList.Add(2062); // C12.22 Status Table
                TableList.Add(2064); //Comm Module General Config Table
                TableList.Add(2078); // RFLAN Neighbor Table
                TableList.Add(2161); // LAN Log Control Table
                TableList.Add(2162); // LAN Log Data Table              
            }

            return TableList;
        }

        /// <summary>
        /// This method allows Bridge devices to modify the Bridge firmware type byte used in 
        /// to either the authenticate FWDL procedure or the initiate FWDL procedure.
        /// </summary>
        /// <param name="byCurrentFWType">The firmware image's type.</param>
        /// <returns>The firmware type to use to pass to the authenticate FWDL procedure.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/02/13 jrf 3.50.10          Created.

        internal byte SelectFWTypeByte(byte byCurrentFWType)
        {
            byte byFWTytpe = byCurrentFWType;

            if (byFWTytpe == (byte)FirmwareType.ChoiceConnectFW)
            {
                byFWTytpe = (byte)FirmwareType.RFLANFW;
            }

            return byFWTytpe;
        }

        /// <summary>
        /// Gets the list of tables to read from the meter during program validation.
        /// </summary>
        /// <returns>The list of Table IDs</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/13 jrf 3.50.16 TQ 9560   Created.

        internal List<ushort> GetValidationTablesToRead()
        {
            List<ushort> TablesToRead = new List<ushort>();            

            TablesToRead.Add(2437);

            return TablesToRead;

        }

        /// <summary>
        /// Adds 25 Year TOU validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/12/13 jrf 3.50.16 TQ 9560   Created.
        // 08/19/14 jrf 3.70.04 WR 529685 Using constant for EDL validation item category.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        internal void Get25YearTOUValidationItems(List<EDLValidationItem> ValidationList)
        {
            if (Supports25YearTOUSchedule)
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389SeasonChangeOptions,
                                            null,
                                            "Season Change Option",
                                            TOU_25_YEAR_SCHEDULE_CATEGORY));

                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389DstHour,
                                            null,
                                            "DST Switch Time Hour",
                                            TOU_25_YEAR_SCHEDULE_CATEGORY));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389DstMinute,
                                            null,
                                            "DST Switch Time Minute",
                                            TOU_25_YEAR_SCHEDULE_CATEGORY));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389DstOffset,
                                            null,
                                            "DST Switch Length",
                                            TOU_25_YEAR_SCHEDULE_CATEGORY));

                //Calendar Years
                for (int y = 0; y < MAX_CALENDAR_YEARS; y++)
                {
                    int iYear = y + 1;
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389CalendarYear,
                                                new int[] { y },
                                                "Calendar Year " + iYear.ToString(),
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    //Calendar Events
                    for (int e = 0; e < MAX_CALENDAR_EVENTS_PER_YEAR; e++)
                    {
                        int iEvent = e + 1;
                        ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389CalendarEvent,
                                                    new int[] { y, e },
                                                    "Calendar Event " + iEvent.ToString() + " (Year " + iYear.ToString() + ")",
                                                    TOU_25_YEAR_SCHEDULE_CATEGORY));
                    }
                }

                //Seasons
                for (int s = 0; s < MAX_SEASONS; s++)
                {
                    int iSeason = s + 1;
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389SeasonProgrammed,
                                                new int[] { s },
                                                "Season " + iSeason.ToString() + " Programmed",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Sunday,
                                                new int[] { s },
                                                "Sunday Pattern" + " (Season " + iSeason.ToString() + ")",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Monday,
                                                new int[] { s },
                                                "Monday Pattern" + " (Season " + iSeason.ToString() + ")",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Tuesday,
                                                new int[] { s },
                                                "Tuesday Pattern" + " (Season " + iSeason.ToString() + ")",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Wednesday,
                                                new int[] { s },
                                                "Wednesday Pattern" + " (Season " + iSeason.ToString() + ")",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Thursday,
                                                new int[] { s },
                                                "Thursday Pattern" + " (Season " + iSeason.ToString() + ")",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Friday,
                                                new int[] { s },
                                                "Friday Pattern" + " (Season " + iSeason.ToString() + ")",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Saturday,
                                                new int[] { s },
                                                "Saturday Pattern" + " (Season " + iSeason.ToString() + ")",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Holiday,
                                                new int[] { s },
                                                "Holiday Pattern" + " (Season " + iSeason.ToString() + ")",
                                                TOU_25_YEAR_SCHEDULE_CATEGORY));

                    //Patterns(Day of Events)/Switchpoints(Time of Day Events)
                    for (int p = 0; p < MAX_DAY_OF_EVENTS; p++)
                    {
                        int iPattern = p + 1;

                        for (int sp = 0; sp < MAX_TIME_OF_DAY_EVENTS; sp++)
                        {
                            int iSwitchPoint = sp + 1;

                            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl389Event,
                                                    new int[] { s, p, sp },
                                                    "Switchpoint " + iSwitchPoint.ToString() + " - Pattern " + iPattern.ToString() + " (Season " + iSeason.ToString() + ")",
                                                    TOU_25_YEAR_SCHEDULE_CATEGORY));
                        }
                    }

                }
            }

        }

        /// <summary>
        /// Checks to see if the item matches and then creates a ProgramValidationItem if it does not.
        /// </summary>
        /// <param name="item">The item to validate</param>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <param name="InvalidItem">When mismatch is discovered then this will be the ProgramValidationItem for the Invalid value</param>
        /// <returns>Returns whether or not the item was found.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/13 jrf 3.50.16 TQ 9560   Created.
        // 01/03/14 jrf 3.50.19 TQ 9629 Adding additional checks for unused calendar events and switchpoints.
        // 08/19/14 jrf 3.70.04 WR 529685 Adding special case to ignore 25 Year TOU Calendar validation items when neither the meter nor the program 
        //                                support TOU.
        internal bool ValidateItem(EDLValidationItem item, CentronTables meterTables, CentronTables programTables, out ProgramValidationItem InvalidItem)
        {
            bool blnFoundItem = true;
            bool blnFoundInvalidItem = false;

            string strDisplayMeterValue = "";
            string strDisplayProgramValue = "";

            object objMeterValue;
            object objProgramValue;            

            InvalidItem = null;

            // Get the values
            objMeterValue = CENTRON_AMI.GetTableValue(item, meterTables);

            objProgramValue = CENTRON_AMI.GetTableValue(item, programTables);

            switch (item.Item)
            {
                case (long)CentronTblEnum.MfgTbl389TouScheduleID:
                {
                    
                    if (objMeterValue != null)
                    {
                        if (1 == (ushort)objMeterValue)
                        {
                            strDisplayMeterValue = "Enabled";
                        }
                        else if (0 == (ushort)objMeterValue)
                        {
                            strDisplayMeterValue = "Disabled";
                        }
                    }

                    if (objProgramValue != null)
                    {
                        if (1 == (ushort)objProgramValue)
                        {
                            strDisplayProgramValue = "Enabled";
                        }
                        else if (0 == (ushort)objProgramValue)
                        {
                            strDisplayProgramValue = "Disabled";
                        }
                    }                       

                    if (true != strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        //Program and meter values do not match, we have an invalid item!
                        blnFoundInvalidItem = true;
                    }

                    break;
                }                
                case (long)CentronTblEnum.MfgTbl389SeasonChangeOptions:
                {
                    if (null != objMeterValue)
                    {
                        strDisplayMeterValue = DecodeSeasonChangeOptions((byte)objMeterValue);
                    }

                    if (null != objProgramValue)
                    {
                        strDisplayProgramValue = DecodeSeasonChangeOptions((byte)objProgramValue);
                    }
                    
                    if (true != strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        //Program and meter values do not match, we have an invalid item!
                        blnFoundInvalidItem = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MfgTbl389CalendarYear:
                {
                    int iMeterCalYear = 0;
                    int iProgramCalYear = 0;

                    if (objMeterValue != null)
                    {
                        iMeterCalYear = CALENDAR_YEAR_BASE + (byte)objMeterValue;
                    }

                    if (objProgramValue != null)
                    {
                        iProgramCalYear = CALENDAR_YEAR_BASE + (byte)objProgramValue;
                    }

                    if (0 != iMeterCalYear)
                    {
                        strDisplayMeterValue = iMeterCalYear.ToString(CultureInfo.CurrentCulture);
                    }

                    if (0 != iProgramCalYear)
                    {
                        strDisplayProgramValue = iProgramCalYear.ToString(CultureInfo.CurrentCulture);
                    }

                    if (true != strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        //Program and meter values do not match, we have an invalid item!
                        blnFoundInvalidItem = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MfgTbl389CalendarEvent:
                {
                    if (objMeterValue != null)
                    {
                        strDisplayMeterValue = DecodeCalendarEvent((byte)objMeterValue) + " - ";
                    }

                    if (objProgramValue != null)
                    {
                        strDisplayProgramValue = DecodeCalendarEvent((byte)objProgramValue) + " - ";
                    }

                    // Get the month values
                    item.Item = (long)CentronTblEnum.MfgTbl389CalendarMonth;

                    objMeterValue = CENTRON_AMI.GetTableValue(item, meterTables);
                    objProgramValue = CENTRON_AMI.GetTableValue(item, programTables);

                    int iMeterVal = 0;
                    int iProgramVal = 0;

                    if (objMeterValue != null)
                    {
                        iMeterVal = (MONTHS_IN_YEAR > (byte)objMeterValue) ? (byte)objMeterValue + 1 : (byte)objMeterValue;
                        strDisplayMeterValue += iMeterVal.ToString("D2", CultureInfo.CurrentCulture);
                    }

                    if (objProgramValue != null)
                    {
                        iProgramVal = (MONTHS_IN_YEAR > (byte)objProgramValue) ? (byte)objProgramValue + 1 : (byte)objProgramValue;
                        strDisplayProgramValue += iProgramVal.ToString("D2", CultureInfo.CurrentCulture);
                    }

                    // Get the day values
                    item.Item = (long)CentronTblEnum.MfgTbl389CalendarDayOfMonth;

                    objMeterValue = CENTRON_AMI.GetTableValue(item, meterTables);
                    objProgramValue = CENTRON_AMI.GetTableValue(item, programTables);

                    iMeterVal = 0;
                    iProgramVal = 0;

                    if (objMeterValue != null)
                    {
                        iMeterVal = (MAX_DAYS_IN_MONTH > (byte)objMeterValue) ? (byte)objMeterValue + 1 : (byte)objMeterValue;
                        strDisplayMeterValue += "/" + iMeterVal.ToString("D2", CultureInfo.CurrentCulture);

                        CheckForUnusedItem(UNUSED_CALENDAR_EVENT, ref strDisplayMeterValue);
                    }

                    if (objProgramValue != null)
                    {
                        iProgramVal = (MAX_DAYS_IN_MONTH > (byte)objProgramValue) ? (byte)objProgramValue + 1 : (byte)objProgramValue;
                        strDisplayProgramValue += "/" + iProgramVal.ToString("D2", CultureInfo.CurrentCulture);

                        CheckForUnusedItem(UNUSED_CALENDAR_EVENT, ref strDisplayProgramValue);
                    }

                    if (true != strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        //Program and meter values do not match, we have an invalid item!
                        blnFoundInvalidItem = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MfgTbl389Sunday:
                case (long)CentronTblEnum.MfgTbl389Monday:
                case (long)CentronTblEnum.MfgTbl389Tuesday:
                case (long)CentronTblEnum.MfgTbl389Wednesday:
                case (long)CentronTblEnum.MfgTbl389Thursday:
                case (long)CentronTblEnum.MfgTbl389Friday:
                case (long)CentronTblEnum.MfgTbl389Saturday:
                case (long)CentronTblEnum.MfgTbl389Holiday:
                {
                    int iMeterVal = 0;
                    int iProgramVal = 0;

                    if (objMeterValue != null)
                    {
                        iMeterVal = (byte)objMeterValue + 1;
                        strDisplayMeterValue = iMeterVal.ToString(CultureInfo.CurrentCulture);
                    }

                    if (objProgramValue != null)
                    {
                        iProgramVal = (byte)objProgramValue + 1;
                        strDisplayProgramValue = iProgramVal.ToString(CultureInfo.CurrentCulture);
                    }

                    if (true != strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        //Program and meter values do not match, we have an invalid item!
                        blnFoundInvalidItem = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MfgTbl389SeasonProgrammed:
                {
                    if (objMeterValue != null)
                    {
                        if (1 == (byte)objMeterValue)
                        {
                            strDisplayMeterValue = true.ToString(CultureInfo.CurrentCulture);
                        }
                        else if (0 == (byte)objMeterValue)
                        {
                            strDisplayMeterValue = false.ToString(CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            strDisplayMeterValue = objMeterValue.ToString();
                        }

                    }

                    if (objProgramValue != null)
                    {
                        if (1 == (byte)objProgramValue)
                        {
                            strDisplayProgramValue = true.ToString(CultureInfo.CurrentCulture);
                        }
                        else if (0 == (byte)objProgramValue)
                        {
                            strDisplayProgramValue = false.ToString(CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            strDisplayProgramValue = objProgramValue.ToString();
                        }
                    }

                    if (true != strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        //Program and meter values do not match, we have an invalid item!
                        blnFoundInvalidItem = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MfgTbl389Event:
                {
                    if (objMeterValue != null)
                    {
                        strDisplayMeterValue = DecodeEvent((byte)objMeterValue) + " - ";
                    }

                    if (objProgramValue != null)
                    {
                        strDisplayProgramValue = DecodeEvent((byte)objProgramValue) + " - ";
                    }

                    if (true != strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        //Program and meter values do not match, we have an invalid item!
                        blnFoundInvalidItem = true;
                    }

                    // Get the hour values
                    item.Item = (long)CentronTblEnum.MfgTbl389Hour;

                    objMeterValue = CENTRON_AMI.GetTableValue(item, meterTables);
                    objProgramValue = CENTRON_AMI.GetTableValue(item, programTables);

                    int iMeterVal = 0;
                    int iProgramVal = 0;

                    if (objMeterValue != null)
                    {
                        iMeterVal = (byte)objMeterValue;
                        strDisplayMeterValue += iMeterVal.ToString("D2", CultureInfo.CurrentCulture);
                    }

                    if (objProgramValue != null)
                    {
                        iProgramVal = (byte)objProgramValue;
                        strDisplayProgramValue += iProgramVal.ToString("D2", CultureInfo.CurrentCulture);
                    }

                    // Get the minute values
                    item.Item = (long)CentronTblEnum.MfgTbl389Minute;

                    objMeterValue = CENTRON_AMI.GetTableValue(item, meterTables);
                    objProgramValue = CENTRON_AMI.GetTableValue(item, programTables);

                    iMeterVal = 0;
                    iProgramVal = 0;

                    if (objMeterValue != null)
                    {
                        iMeterVal = (byte)objMeterValue;
                        strDisplayMeterValue += ":" + iMeterVal.ToString("D2", CultureInfo.CurrentCulture);

                        CheckForUnusedItem(UNUSED_SWITCHPOINT, ref strDisplayMeterValue);
                    }

                    if (objProgramValue != null)
                    {
                        iProgramVal = (byte)objProgramValue;
                        strDisplayProgramValue += ":" + iProgramVal.ToString("D2", CultureInfo.CurrentCulture);

                        CheckForUnusedItem(UNUSED_SWITCHPOINT, ref strDisplayProgramValue);
                    }

                    if (true != strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        //Program and meter values do not match, we have an invalid item!
                        blnFoundInvalidItem = true;
                    }

                    break;
                }
                default:
                {
                    //If we make it here we know we didn't find item.
                    blnFoundItem = false;
                    break;
                }
            }

            if (true == blnFoundInvalidItem)
            {
                // There is a mismatch so add the item.
                InvalidItem = new ProgramValidationItem(item.Category, item.Name, strDisplayProgramValue, strDisplayMeterValue);
            }

            //Special case! If neither the configuration file nor the meter is configured for TOU then we do not want to validate 25 year TOU schedule items
            if (TOU_25_YEAR_SCHEDULE_CATEGORY == item.Category && false == m_OpenWayDevice.TOUEnabled && false == IsTOUEnabled(programTables))
            {
                blnFoundItem = true;
                InvalidItem = null;
            }

            return blnFoundItem;
        }

        /// <summary>
        /// Updates the TOU for the program file.
        /// </summary>
        /// <param name="ProgramTables">Program data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/17/13 jrf 3.50.16 TQ 9560 Created.
        // 04/11/14 jrf 3.50.72 WR 489051 Added wait before performing EDL file operation to genearate 
        //                                25 year calendar data.
        internal void UpdateTOU(CentronTables ProgramTables)
        {
            //Only bother with 25 year TOU if meter supports it.
            if (Supports25YearTOUSchedule)
            {
                try
                {
                    //Sending wait because the call to create the 25 year calendar can take 7-8 seconds
                    //and cause a time out.
                    m_OpenWayDevice.SendWait();

                    //Call CE Dll method to generate the 25 year TOU schedule in mfg. table 2437
                    ProgramTables.Create25YearCalendarFromStandardTables(m_OpenWayDevice.DateProgrammed, true);
                }
                catch (Exception)
                {
                    // The TOU schedule defined in the program is not supported by the 25 year TOU schedule
                }
            }
        }

        /// <summary>
        /// Determines if the table data indicates that TOU is configured.
        /// </summary>
        /// <param name="tableData">C12.19 table data</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/19/14 jrf 3.70.04 WR 529685 Created.
        internal bool IsTOUEnabled(CentronTables tableData)
        {
            bool Enabled = false;
            string TarrifID = GetTarrifID(tableData);
            ushort CalendarID = GetCalendarID(tableData);

            if ((null != TarrifID && TarrifID.Length > 0) || CalendarID > 0)
            {
                Enabled = true;
            }

            return Enabled;
        }

        /// <summary>
        /// Retrieves the tarrif ID from the table data.
        /// </summary>
        /// <param name="tableData">C12.19 table data</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/19/14 jrf 3.70.04 WR 529685 Created.
        internal string GetTarrifID(CentronTables tableData)
        {
            string TarrifID = null;
            object Value = null;

            try
            {
                if (tableData.IsCached((long)StdTableEnum.STDTBL6_TARIFF_ID, null))
                {
                    tableData.GetValue(StdTableEnum.STDTBL6_TARIFF_ID, null, out Value);
                    TarrifID = Value.ToString();
                    TarrifID = TarrifID.TrimEnd('\0');
                }
            }
            catch
            {
                TarrifID = null;
            }

            return TarrifID;
        }

        /// <summary>
        /// Retrieves the Calendar ID from the table data.
        /// </summary>
        /// <param name="tableData">C12.19 table data</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/19/14 jrf 3.70.04 WR 529685 Created.
        internal ushort GetCalendarID(CentronTables tableData)
        {
            ushort CalendarID = 0;
            object Value = null;

            try
            {
                if (tableData.IsCached((long)CentronTblEnum.MFGTBL42_CALENDAR_ID, null))
                {
                    tableData.GetValue(CentronTblEnum.MFGTBL42_CALENDAR_ID, null, out Value);
                    CalendarID = (ushort)Value;
                }
            }
            catch
            {
                CalendarID = 0;
            }

            return CalendarID;
        }

        

        #endregion

        #region Internal Properties

        #region Bridge Functionality

        /// <summary>
        /// Gets whether the meter is a Bridge meter that was released 
        /// during the initial Bridge project (Phase 1).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/26/13 jrf 3.50.10        Created
        //  11/19/14 jrf 4.00.89 TBD    Changes from design review, don't use FW version.
        internal bool IsBridgePhase1Meter
        {
            get
            {
                bool blnBridgeP1 = false;
                                
                if (null != m_OpenWayDevice && false == Supports25YearTOUSchedule)
                {
                    blnBridgeP1 = true;
                }
                return blnBridgeP1;
            }
        }

        /// <summary>
        /// Gets the register's current communications operating mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/26/13 jrf 3.50.10        Created.
        //
        internal OpenWayMFGTable2428.ChoiceConnectCommOpMode CurrentRegisterCommOpMode
        {
            get
            {
                OpenWayMFGTable2428.ChoiceConnectCommOpMode CommOpMode = OpenWayMFGTable2428.ChoiceConnectCommOpMode.UnknownOperationalMode;

                if (null != Table2428)
                {
                    CommOpMode = Table2428.CurrentRegisterMode;
                }

                return CommOpMode;
            }
        }

        /// <summary>
        /// Gets the register's requested communications operating mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 jrf 3.50.10        Created.
        //
        internal OpenWayMFGTable2428.ChoiceConnectCommOpMode RequestedRegisterCommOpMode
        {
            get
            {
                return (null == Table2428) ? OpenWayMFGTable2428.ChoiceConnectCommOpMode.UnknownOperationalMode : Table2428.RequestedMode;
            }
        }

        /// <summary>
        /// Gets the Bridge meter's manufactured mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/17/14 jrf 4.00.89 TBD    Created.
        //
        internal OpenWayMFGTable2428.ChoiceConnectCommMfgMode ChoiceConnectCommModuleManufacturedMode
        {
            get
            {
                return (null == Table2428) ? OpenWayMFGTable2428.ChoiceConnectCommMfgMode.UnknownManufacturingMode : Table2428.ManufacturedMode;
            }
        }

        #endregion

        #region ChoiceConnect Comm Module

        /// <summary>
        /// Gets the ChoiceConnect MSM Firmware Version.Revision string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 jrf 3.50.10        Created.

        internal string ChoiceConnectFWVerRev
        {
            get
            {
                return (null == Table2428) ? null : Table2428.CCChoiceConnectCommFwVerRev;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Firmware build string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 jrf 3.50.10        Created.

        internal string ChoiceConnectFWBuild
        {
            get
            {
                return (null == Table2428) ? null : Table2428.CCChoiceConnectCommFwBuildString;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM ERT ID as a formatted string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/02/13 jrf 3.50.10        Created.

        internal string ChoiceConnectERTID
        {
            get
            {
                return (null == Table2429) ? null : Table2429.ErtIdString;
            }
        } 

        /// <summary>
        /// Gets the ChoiceConnect MSM Bubble-up LID translated as a string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/19/12 JJJ 2.60.xx   NA   Created

        internal string ChoiceConnectBubbleUpLIDDescription
        {
            get
            {
                string strBubbleUpLid = null;

                if (null != Table2429)
                {
                    strBubbleUpLid = OpenWayMFGTable2429.GetBubbleUpLIDString(Table2429.BubbleUpLIDValue);
                }

                return strBubbleUpLid;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Security State as a formatted string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/20/12 JJJ 2.60.xx   NA   Created

        internal string ChoiceConnectSecurityStateDescription
        {
            get
            {
                return (null == Table2429) ? null : Table2429.CCSecurityStateString;
            }
        }

        /// <summary>
        /// Checks the meter's configuration to make sure that it is compatible with
        /// ChoiceConnect
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/10/14 AF  3.50.25 TQ9489 Created to allow restrictions if a configuration
        //                              is not compatible
        //  06/12/14 jrf 3.50.99 No WR  Test code to allow non-TOU Bridge meters to be CC compatible.
        internal bool IsConfigChoiceConnectCompatible
        {
            get
            {
                bool blnCompatible = true;

                if (CurrentRegisterCommOpMode == OpenWayMFGTable2428.ChoiceConnectCommOpMode.OpenWayOperationalMode)
                {
                    // We need to check the configuration only if we are switching to ChoiceConnect mode
                    if (m_OpenWayDevice != null)
                    {
                        // check to see if at least one of the acceptable quantities is configured
                        if ((m_OpenWayDevice.WattsDelivered != null) || (m_OpenWayDevice.WattsNet != null) || (m_OpenWayDevice.WattsUni != null)
                            || (m_OpenWayDevice.WattsReceived != null) || (m_OpenWayDevice.VADelivered != null) || (m_OpenWayDevice.VAReceived != null)
                            || (m_OpenWayDevice.VarDelivered != null) || (m_OpenWayDevice.VAReceived != null))
                        {
                            // Now check on Load Profile.  LP doesn't have to be running but, if it is, the interval length must be 15 minutes
                            if (m_OpenWayDevice.LPRunning && m_OpenWayDevice.LPIntervalLength != 15)
                            {
                                blnCompatible = false;
                            }

                            // And the firmware must support a 25 year TOU schedule if TOU is enabled
                            if (m_OpenWayDevice.TOUEnabled && !(Supports25YearTOUSchedule))
                            {
                                blnCompatible = false;
                            }
                        }
                        else
                        {
                            blnCompatible = false;
                        }
                    }
                }

                return blnCompatible;
            }
        }

        #endregion

        #region OpenWay Comm Module

        /// <summary>
        /// Gets the OpenWay Comm module version.revision from 
        /// the ChoiceConnect state table.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10        Created

        internal string OpenWayCommModVer
        {
            get
            {
                string strVersion = "0.000";

                if (null != Table2428)
                {
                    strVersion = Table2428.CCOpenWayCommFwVerRev;
                }

                return strVersion;
            }
        }

        /// <summary>
        /// Gets the OpenWay Comm Module Version as a byte
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10        Created

        internal byte OpenWayCommModuleVersion
        {
            get
            {
                byte byValue = 0;

                if (null != Table2428)
                {
                    byValue = Table2428.CCOpenWayCommFwVer;
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
        //  11/26/13 jrf 3.50.10        Created

        internal byte OpenWayCommModuleRevision
        {
            get
            {
                byte byValue = 0;

                if (null != Table2428)
                {
                    byValue = Table2428.CCOpenWayCommFwRev;
                }

                return byValue;
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
        internal byte OpenWayCommModuleBuild
        {
            get
            {
                byte byValue = 0;

                if (null != Table2428)
                {
                    byValue = Table2428.CCOpenWayCommFwBuild;
                }

                return byValue;
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
        internal string OpenWayCommModBuild
        {
            get
            {
                return (null == Table2428) ? null : Table2428.CCOpenWayCommFwBuildString; 
            }
        }

        #endregion

        #region TOU

        /// <summary>
        /// Gets the current TOU configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10        Created
        //  01/13/14 jrf 3.50.24 TQ 9478 Returning null if can't retrieve 25 year TOU.
        // 
        internal TOUConfig TOUConfiguration
        {
            get
            {
                TOUConfig TOUConfigData = null;

                if (Supports25YearTOUSchedule 
                    && OpenWayMFGTable2428.ChoiceConnectCommOpMode.ChoiceConnectOperationalMode == CurrentRegisterCommOpMode)
                {
                    TOUConfigData = Table2437.TOUConfig;
                }

                return TOUConfigData;
            }
        }

        /// <summary>
        /// Gets the current Calendar configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10        Created
        //  01/13/14 jrf 3.50.24 TQ 9478 Returning null if can't retrieve 25 year TOU.
        // 
        internal CalendarConfig CalendarConfiguration
        {
            get
            {
                CalendarConfig CalendarConfigData = null;

                if (Supports25YearTOUSchedule
                    && OpenWayMFGTable2428.ChoiceConnectCommOpMode.ChoiceConnectOperationalMode == CurrentRegisterCommOpMode)
                {
                    CalendarConfigData = Table2437.CalendarConfig;
                }

                return CalendarConfigData;
            }
        }

        /// <summary>
        /// Gets whether or not the 25 Year TOU schedule is supported.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/26/13 jrf 3.50.10        Created

        internal bool Supports25YearTOUSchedule
        {
            get
            {
                return (null != Table2437);
            }
        }

        #endregion

        #region Voltage Monitoring

        /// <summary>
        /// Gets the number of RMS below threshold counts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/04/13 DLG 3.50.12 TR9480   Created.
        //  
        internal int RMSBelowThresholdCount
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;

                if (!m_RMSBelowThreshold.Cached)
                {
                    Result = m_OpenWayDevice.m_lidRetriever.RetrieveLID(m_OpenWayDevice.m_LID.RMS_BELOW_THRESHOLD_COUNT, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        m_RMSBelowThreshold.Value = (int)Data[0];
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading the number of RMS Below Threshold counts."));
                    }
                }

                return m_RMSBelowThreshold.Value;
            }
        }

        /// <summary>
        /// Gets the number of RMS high threshold counts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/04/13 DLG 3.50.12 TR9480   Created.
        //  
        internal int RMSHighThresholdCount
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;

                if (!m_RMSHighThreshold.Cached)
                {
                    Result = m_OpenWayDevice.m_lidRetriever.RetrieveLID(m_OpenWayDevice.m_LID.RMS_HIGH_THRESHOLD_COUNT, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        m_RMSHighThreshold.Value = (int)Data[0];
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading the number of RMS High Threshold counts."));
                    }
                }

                return m_RMSHighThreshold.Value;
            }
        }

        /// <summary>
        /// Gets the number of Vh below threshold counts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/04/13 DLG 3.50.12 TR9480   Created.
        //  
        internal int VhBelowThresholdCount
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;

                if (!m_VhBelowThreshold.Cached)
                {
                    Result = m_OpenWayDevice.m_lidRetriever.RetrieveLID(m_OpenWayDevice.m_LID.VH_BELOW_THRESHOLD_COUNT, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        m_VhBelowThreshold.Value = (int)Data[0];
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading the number of Vh Below Threshold counts."));
                    }
                }

                return m_VhBelowThreshold.Value;
            }
        }

        /// <summary>
        /// Gets the number of Vh high threshold counts.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/04/13 DLG 3.50.12 TR9480   Created.
        //  
        internal int VhHighThresholdCount
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;

                if (!m_VhHighThreshold.Cached)
                {
                    Result = m_OpenWayDevice.m_lidRetriever.RetrieveLID(m_OpenWayDevice.m_LID.VH_HIGH_THRESHOLD_COUNT, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        m_VhHighThreshold.Value = (int)Data[0];
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading the number of Vh High Threshold counts."));
                    }
                }

                return m_VhHighThreshold.Value;
            }
        }

        #endregion

        #region Previous Season

        /// <summary>
        /// Proves access to a list of Energy Quantities from last season (Std table 24)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        //  01/17/14 jrf 3.50.26 TQ9556 Reordered quantities to be consistent with ordering 
        //                              of other quantity lists.

        internal List<Quantity> PreviousSeasonRegisters
        {
            get
            {
                List<Quantity> QuantityList = new List<Quantity>();
                Quantity Qty;

                // Add Watts Del
                Qty = PreviousSeasonWattsDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Rec
                Qty = PreviousSeasonWattsReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Net
                Qty = PreviousSeasonWattsNet;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Uni
                Qty = PreviousSeasonWattsUni;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Del
                Qty = PreviousSeasonVADelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Rec
                Qty = PreviousSeasonVAReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Lag
                Qty = PreviousSeasonVALagging;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Del
                Qty = PreviousSeasonVarDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Rec
                Qty = PreviousSeasonVarReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net
                Qty = PreviousSeasonVarNet;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net Del
                Qty = PreviousSeasonVarNetDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net Rec
                Qty = PreviousSeasonVarNetReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q1
                Qty = PreviousSeasonVarQuadrant1;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q2
                Qty = PreviousSeasonVarQuadrant2;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q3
                Qty = PreviousSeasonVarQuadrant3;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q4
                Qty = PreviousSeasonVarQuadrant4;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (a)
                Qty = PreviousSeasonAmpsPhaseA;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (b)
                Qty = PreviousSeasonAmpsPhaseB;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (c)
                Qty = PreviousSeasonAmpsPhaseC;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A Neutral
                Qty = PreviousSeasonAmpsNeutral;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A^2
                Qty = PreviousSeasonAmpsSquared;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (a)
                Qty = PreviousSeasonVoltsPhaseA;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (b)
                Qty = PreviousSeasonVoltsPhaseB;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (c)
                Qty = PreviousSeasonVoltsPhaseC;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V Avg
                Qty = PreviousSeasonVoltsAverage;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V^2
                Qty = PreviousSeasonVoltsSquared;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add PF
                Qty = PreviousSeasonPowerFactor;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Q Del
                Qty = PreviousSeasonQDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Q Rec
                Qty = PreviousSeasonQReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }

                return QuantityList;
            }
        }

        /// <summary>
        /// Gets the Neutral Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonAmpsNeutral
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_AH_NEUTRAL, m_OpenWayDevice.m_LID.DEMAND_MAX_A_NEUTRAL,
                        "Neutral Amps", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase A Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonAmpsPhaseA
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_AH_PHA, m_OpenWayDevice.m_LID.DEMAND_MAX_A_PHA,
                    "Amps (a)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase B Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonAmpsPhaseB
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_AH_PHB, m_OpenWayDevice.m_LID.DEMAND_MAX_A_PHB,
                    "Amps (b)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase C Amps from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonAmpsPhaseC
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_AH_PHC, m_OpenWayDevice.m_LID.DEMAND_MAX_A_PHC,
                    "Amps (c)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Amps squared from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonAmpsSquared
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_I2H_AGG, m_OpenWayDevice.m_LID.DEMAND_MAX_I2_AGG,
                    "Amps Squared", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Q Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonQDelivered
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_QH_DEL, m_OpenWayDevice.m_LID.DEMAND_MAX_Q_DEL,
                    "Q Delivered", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Qh Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonQReceived
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_QH_REC, m_OpenWayDevice.m_LID.DEMAND_MAX_Q_REC,
                    "Q Received", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the VA Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVADelivered
        {
            get
            {
                Quantity VA = null;

                if (null != Table24)
                {
                    // Try getting Arithmatic first.
                    VA = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VAH_DEL_ARITH, m_OpenWayDevice.m_LID.DEMAND_MAX_VA_DEL_ARITH,
                        "VA Delivered", Table24.PreviousSeasonRegisterData);

                    // Try  getting Vectoral
                    if (VA == null)
                    {
                        VA = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VAH_DEL_VECT, m_OpenWayDevice.m_LID.DEMAND_MAX_VA_DEL_VECT,
                            "VA Delivered", Table24.PreviousSeasonRegisterData);
                    }
                }

                return VA;
            }
        }

        /// <summary>
        /// Gets the Lagging VA from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVALagging
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VAH_LAG, m_OpenWayDevice.m_LID.DEMAND_MAX_VA_LAG,
                    "VA Lagging", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarDelivered
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_DEL, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_DEL,
                    "Var Delivered", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the VA Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVAReceived
        {
            get
            {
                Quantity VA = null;

                if (null != Table24)
                {

                    // Try getting Arithmetic first.
                    VA = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VAH_REC_ARITH, m_OpenWayDevice.m_LID.DEMAND_MAX_VA_REC_ARITH,
                        "VA Received", Table24.PreviousSeasonRegisterData);

                    // Try  getting Vectorial
                    if (VA == null)
                    {
                        VA = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VAH_REC_VECT, m_OpenWayDevice.m_LID.DEMAND_MAX_VA_REC_VECT,
                            "VA Received", Table24.PreviousSeasonRegisterData);
                    }
                }

                return VA;
            }
        }

        /// <summary>
        /// Gets the Var Net from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarNet
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_NET, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_NET,
                    "Var Net", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Net delivered from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarNetDelivered
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_NET_DEL, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_NET_DEL,
                    "Var Net Delivered", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Net Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarNetReceived
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_NET_REC, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_NET_REC,
                    "Var Net Received", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Q1 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarQuadrant1
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_Q1, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_Q1,
                    "Var Quadrant 1", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Q2 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarQuadrant2
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_Q2, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_Q2,
                    "Var Quadrant 2", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Q3 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarQuadrant3
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_Q3, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_Q3,
                    "Var Quadrant 3", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Q4 from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarQuadrant4
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_Q4, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_Q4,
                    "Var Quadrant 4", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Var Received from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVarReceived
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VARH_REC, m_OpenWayDevice.m_LID.DEMAND_MAX_VAR_REC,
                    "Var Received", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Average Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVoltsAverage
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VH_AVG, m_OpenWayDevice.m_LID.DEMAND_MAX_V_AVG,
                    "Volts Average", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase A Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVoltsPhaseA
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VH_PHA, m_OpenWayDevice.m_LID.DEMAND_MAX_V_PHA,
                    "Volts (a)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase B Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVoltsPhaseB
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VH_PHB, m_OpenWayDevice.m_LID.DEMAND_MAX_V_PHB,
                    "Volts (b)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Phase C Volts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVoltsPhaseC
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_VH_PHC, m_OpenWayDevice.m_LID.DEMAND_MAX_V_PHC,
                    "Volts (c)", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Volts squared from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonVoltsSquared
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_V2H_AGG, m_OpenWayDevice.m_LID.DEMAND_MAX_V2_AGG,
                    "Volts Squared", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Watts Delivered quantity from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonWattsDelivered
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_WH_DEL, m_OpenWayDevice.m_LID.DEMAND_MAX_W_DEL,
                    "Watts Delivered", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Watts Received quantity from the standard tables
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonWattsReceived
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_WH_REC, m_OpenWayDevice.m_LID.DEMAND_MAX_W_REC,
                    "Watts Received", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Watts Net quantity from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonWattsNet
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_WH_NET, m_OpenWayDevice.m_LID.DEMAND_MAX_W_NET,
                    "Watts Net", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Unidirectional Watts from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.
        // 12/08/14 jrf 4.00.91 544415 Setting appropriate demand lid.
        internal Quantity PreviousSeasonWattsUni
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(m_OpenWayDevice.m_LID.ENERGY_WH_UNI, m_OpenWayDevice.m_LID.DEMAND_MAX_W_UNI,
                    "Unidirectional Watts", Table24.PreviousSeasonRegisterData);
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the Power Factor from the standard tables.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/17/14 jrf 3.50.26 TQ9556 Created.

        internal Quantity PreviousSeasonPowerFactor
        {
            get
            {
                Quantity Qty = null;

                if (null != Table24)
                {
                    // There is no PF energy so just check the demand
                    Qty = m_OpenWayDevice.GetQuantityFromStandardTables(null, m_OpenWayDevice.m_LID.DEMAND_MIN_PF_INTERVAL_ARITH,
                        "Power Factor", Table24.PreviousSeasonRegisterData);

                    // Also try the vectorial PF
                    if (Qty == null)
                    {
                        Qty = m_OpenWayDevice.GetQuantityFromStandardTables(null, m_OpenWayDevice.m_LID.DEMAND_MIN_PF_INTERVAL_VECT,
                            "Power Factor", Table24.PreviousSeasonRegisterData);
                    }
                }

                return Qty;
            }
        }

        /// <summary>
        /// Gets the end date of the previous season.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/06/13 jrf 3.50.06 TQ9556 Created.

        internal DateTime? PreviousSeasonEndDate
        {
            get
            {
                DateTime? dtEndDate = null;

                if (null != Table24)
                {
                    dtEndDate = Table24.PreviousSeasonEndDate;
                }

                return dtEndDate;
            }
        }

        #endregion

        #endregion

        #region Protected Properties

        #region Tables

        /// <summary>
        /// Gets the Table 24 object and creates it if needed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/08 RCG 2.00.00		   Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 11/26/13 jrf 3.50.10 TQ9556 Moved here from CENTRON_AMI.
        //
        private StdTable24 Table24
        {
            get
            {
                // If the table has expired, we need to rebuild it because its size may have changed.
                if (m_OpenWayDevice.Table21.State == AnsiTable.TableState.Expired)
                {
                    m_Table24 = null;
                }

                if (null == m_Table24 && m_OpenWayDevice.Table00.IsTableUsed(24))
                {
                    m_Table24 = new StdTable24(m_PSEM, m_OpenWayDevice.Table00, m_OpenWayDevice.Table21);
                }

                return m_Table24;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect State table and creates it if needed.
        /// If the meter does not support this table null will be returned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/06/12 JJJ 2.60.xx  N/A   Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - promoted from CENTRON_AMI
        //  11/26/13 jrf 3.50.10 TQ9556 Moved here CANSIDevice.
        //
        private OpenWayMFGTable2428 Table2428
        {
            get
            {
                if (null == m_Table2428 && true == m_OpenWayDevice.Table00.IsTableUsed(2428))
                {
                    m_Table2428 = new OpenWayMFGTable2428(m_PSEM);
                }

                return m_Table2428;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Configuration table and creates it if needed. 
        /// If the meter does not support this table null will be returned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/06/12 JJJ 2.60.xx N/A    Created

        private OpenWayMFGTable2429 Table2429
        {
            get
            {
                if (null == m_Table2429 && true == m_OpenWayDevice.Table00.IsTableUsed(2429))
                {
                    m_Table2429 = new OpenWayMFGTable2429(m_PSEM);
                }

                return m_Table2429;
            }
        }   

        /// <summary>
        /// Gets the 25 Year TOU Schedule table and creates it if needed. 
        /// If the meter does not support this table null will be returned.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  11/15/13 jrf 3.50.04 TQ 9478   Created
        //  11/27/13 jrf 3.50.10 TQ9556 Moved here from CENTRON_AMI.

        private OpenWayMFGTable2437 Table2437
        {
            get
            {
                if (null == m_Table2437 && true == m_OpenWayDevice.Table00.IsTableUsed(2437))
                {
                    m_Table2437 = new OpenWayMFGTable2437(m_PSEM);
                }

                return m_Table2437;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Reconfigures 25 Year TOU schedule using the specified EDL file
        /// </summary>
        /// <param name="strFileName">The EDL file that contains the TOU data.</param>
        /// <returns>TOUReconfigResult code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/22/13 jrf 3.50.07 TQ9523 Created
        // 11/27/13 jrf 3.50.10 TQ9523 Moved here from CENTRON_AMI.
        // 12/11/13 jrf 3.50.14 TQ9523 Added call to method to generate the 25 year TOU calendar.  
        // 04/11/14 jrf 3.50.72 WR 489051 Added wait before performing EDL file operation to genearate 
        //                                25 year calendar data.
        protected TOUReconfigResult Write25YearTOU(string strFileName)
        {
            TOUReconfigResult Result = TOUReconfigResult.SUCCESS;
            CentronTables DeviceTables = new CentronTables();
            TableData[] PSEMData = null;

            // Load the file into the tables
            if (File.Exists(strFileName) == true)
            {
                try
                {
                    DeviceTables.LoadEDLFile(XmlReader.Create(strFileName));

                    //Sending wait because the call to create the 25 year calendar can take 7-8 seconds
                    //and cause a time out.
                    m_OpenWayDevice.SendWait();

                    //Call CE Dll method to generate the 25 year TOU schedule in mfg. table 2437
                    DeviceTables.Create25YearCalendarFromStandardTables(DateTime.Now, true);
                }
                catch (Exception)
                {
                    Result = TOUReconfigResult.ERROR_TOU_NOT_VALID;
                }
            }
            else
            {
                Result = TOUReconfigResult.FILE_NOT_FOUND;
            }

            if (Result == TOUReconfigResult.SUCCESS)
            {
                // Build the streams for the 25 Year TOU table
                PSEMData = DeviceTables.BuildPSEMStreams(TOU_25_YEAR_TABLE);
            }

            if (Result == TOUReconfigResult.SUCCESS)
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;

                // Write the TOU to the meter
                foreach (TableData DataBlock in PSEMData)
                {
                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        if (DataBlock.FullTable == true)
                        {
                            PSEMResult = m_PSEM.FullWrite(DataBlock.TableID, DataBlock.PSEM.ToArray());

                        }
                        else
                        {
                            PSEMResult = m_PSEM.OffsetWrite(DataBlock.TableID, (int)DataBlock.Offset,
                                                            DataBlock.PSEM.ToArray());
                        }
                    }
                }

                // Check the PSEM Result
                if (PSEMResult == PSEMResponse.Isc)
                {
                    Result = TOUReconfigResult.INSUFFICIENT_SECURITY_ERROR;
                }
                else if (PSEMResult != PSEMResponse.Ok)
                {
                    Result = TOUReconfigResult.PROTOCOL_ERROR;
                }
            }

            return Result;
        }

        /// <summary>
        /// This method decodes the given byte value for the season change option
        /// to a string.
        /// </summary>
        /// <param name="byOption">Season change option</param>
        /// <returns>Decoded value.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/12/13 jrf 3.50.16 TQ 9560   Created.
        //
        public static string DecodeSeasonChangeOptions(byte byOption)
        {
            string strReturn = "";

            switch (byOption)
            {
                case (byte)CalendarControlType.DelaySeasonChangeUntilDemandReset:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("DelaySeasonChangeUntilDemandReset");
                        break;
                    }
                case (byte)CalendarControlType.ForceDemandResetOnSeasonChange:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("ForceDemandResetOnSeasonChange");
                        break;
                    }
                case (byte)CalendarControlType.SeasonChangeWithoutDemandReset:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("SeasonChangeWithoutDemandReset");
                        break;
                    }
                default:
                    {
                        strReturn = "Undefined Season Change Option - " + byOption.ToString(CultureInfo.CurrentCulture);
                        break;
                    }
            }

            return strReturn;
        }

        /// <summary>
        /// This method decodes the given value for the calendar event
        /// into a string.
        /// </summary>
        /// <param name="byEvent">Calendar event.</param>
        /// <returns>Decoded value.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/16/13 jrf 3.50.16 TQ 9560   Created.
        //
        public static string DecodeCalendarEvent(byte byEvent)
        {
            string strReturn = "";

            switch (byEvent)
            {
                case (byte)AMICalendarEvent.AMICalendarEventType.DST_ON:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("EnterDST");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.DST_OFF:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("ExitDST");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.HOLIDAY:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Holiday");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.SEASON_1:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Season1");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.SEASON_2:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Season2");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.SEASON_3:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Season3");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.SEASON_4:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Season4");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.SEASON_5:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Season5");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.SEASON_6:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Season6");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.SEASON_7:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Season7");
                        break;
                    }
                case (byte)AMICalendarEvent.AMICalendarEventType.SEASON_8:
                    {
                        strReturn = CENTRON_AMI.m_rmStrings.GetString("Season8");
                        break;
                    }
                default:
                    {
                        strReturn = "Undefined Event - " + byEvent.ToString(CultureInfo.CurrentCulture);
                        break;
                    }
            }

            return strReturn;
        }

        /// <summary>
        /// This method decodes the given value for the Time Of Day event
        /// into a string.
        /// </summary>
        /// <param name="byEvent">Time of Day event, i.e new rate to use.</param>
        /// <returns>Decoded value.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/16/13 jrf 3.50.16 TQ 9560   Created.
        //
        public static string DecodeEvent(byte byEvent)
        {
            string strReturn = "";
            byte byZeroBasedEvent = (byte)(byEvent - 1);

            switch (byZeroBasedEvent)
            {
                case (byte)Rate.A:
                    {
                        strReturn = Rate.A.ToDescription();
                        break;
                    }
                case (byte)Rate.B:
                    {
                        strReturn = Rate.B.ToDescription();
                        break;
                    }
                case (byte)Rate.C:
                    {
                        strReturn = Rate.C.ToDescription();
                        break;
                    }
                case (byte)Rate.D:
                    {
                        strReturn = Rate.D.ToDescription();
                        break;
                    }
                case (byte)Rate.E:
                    {
                        strReturn = Rate.E.ToDescription();
                        break;
                    }
                case (byte)Rate.F:
                    {
                        strReturn = Rate.F.ToDescription();
                        break;
                    }
                case (byte)Rate.G:
                    {
                        strReturn = Rate.G.ToDescription();
                        break;
                    }
                default:
                    {
                        strReturn = "Undefined Event - " + byEvent.ToString(CultureInfo.CurrentCulture);
                        break;
                    }
            }

            return strReturn;
        }

        /// <summary>
        /// This method checks for unused items.
        /// </summary>
        /// <param name="strUnusedValue">The value an unused item will have.</param>
        /// <param name="strItemToCheck">The item to check if unused.</param>
        /// <returns>Returns boolean indicating whether or not item is unused.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/03/14 jrf 3.50.19 TQ 9629 Created.
        //
        public static bool CheckForUnusedItem(string strUnusedValue, ref string strItemToCheck)
        {
            bool blnUnusedItem = false;

            if (strUnusedValue == strItemToCheck)
            {
                blnUnusedItem = true;
                strItemToCheck = CENTRON_AMI.m_rmStrings.GetString("UNASSIGNED");
            }
            else
            {
                blnUnusedItem = false;
            }

            return blnUnusedItem;
        }

        #endregion

        #region Members

        private CPSEM m_PSEM;
        private CENTRON_AMI m_OpenWayDevice;

        private StdTable24 m_Table24 = null;
        private OpenWayMFGTable2428 m_Table2428 = null;
        private OpenWayMFGTable2429 m_Table2429 = null;
        private OpenWayMFGTable2437 m_Table2437 = null;

        /// <summary>
        /// Counter for the Number of RMS Below Threshold
        /// </summary>
        private CachedInt m_RMSBelowThreshold;

        /// <summary>
        /// Counter for the Number of RMS High Threshold
        /// </summary>
        private CachedInt m_RMSHighThreshold;

        /// <summary>
        /// Counter for the Number of Vh Below Threshold
        /// </summary>
        private CachedInt m_VhBelowThreshold;

        /// <summary>
        /// Counter for the Number of Vh High Threshold
        /// </summary>
        private CachedInt m_VhHighThreshold;

        #endregion
    }
}
